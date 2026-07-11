using FinanceConnect.Client.Pages.Finance.OpeningBalance;
using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.ComponentModel.Design;
using static FinanceConnect.Client.Data.MasterDataIds;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FinanceConnect.Client.Pages
{
    public partial class Dashboard1
    {
        [Inject] DashboardService DashboardService { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] CustomerAgingService AgingService { get; set; } = default!;
        [Inject] VendorAgingService VendorAgingService { get; set; } = default!;
        [Inject] BranchService BranchService { get; set; } = default!;

        [Inject] JournalEntryService JournalEntryService { get; set; } = default!;
        [Inject] FinancialTransactionService TransactionService { get; set; } = default!;
        [Inject] BankReconciliationService ReconService { get; set; } = default!;
        [Inject] BankAccountService BankAccountService { get; set; } = default!;
        [Inject] CashAccountService CashAccountService { get; set; } = default!;
        [Inject] CustomerInvoiceService InvoiceService { get; set; } = default!;
        [Inject] FiscalYearService FiscalYearService { get; set; } = default!;
        [Inject] AccountingPeriodService AccountingPeriodService { get; set; } = default!;
        [Inject] COADataService COAService { get; set; } = default!;
        [Inject] FinanceDataService FinanceDataService { get; set; } = default!;
        [Inject] CustomerAccountService CustomerAccountService { get; set; } = default!;
        [Inject] VendorAccountService VendorAccountService { get; set; } = default!;
        [Inject] VendorBillService BillService { get; set; } = default!;

        List<JournalEntryModel> TopUnpostedJournalEntries = new();
        List<FinancialTransactionModel> TodaysTransactions = new();
        List<BankReconciliationModel> PendingRecons = new();
        List<BankAccountModel> BankAccounts = new();
        private AccountSummaryStatsViewModel CustomerAccountStats = new();
        private VendorAccountSummaryStatsViewModel VendorAccountStats = new();
        BankAccountModel? SelectedBankAccount;
        private List<OpeningBalanceModel> AllOpeningBalances = new();
        private List<ClosingBalanceModel> AllClosingBalances = new();
        private VendorAgingStatisticsViewModel VendorStatistics = new();
        private CustomerAgingStatisticsViewModel CustomerAgingStatistics = new();
        private BucketSummaryViewModel BucketSummaryViewModel = new();
        private CustomerAgingViewModel? LatestSnapshot = new();
        private FiscalYearModel? fiscalYr = new();
        private List<CustomerAgingViewModel> Snapshots = new();
        List<(string BranchName, decimal Amount)> BranchPerformance = new();
        private List<(string Type, decimal Amount)> TopExpenseTypes = new();
        public CashAccountModels? CashAccounts = new();
        public ChartOfAccountsViewModel? SelectedChartOfAccount = new();
        private List<VendorBillViewModel> allBills = new();
        private List<CustomerInvoiceViewModel> TopOverdueInvoices = new();
        string CurrencySymbol="";
        string CurrentPeriod = "";
        string OpeningBalance = "";
        string ClosingBalance = "";
        int PendingJournals;
        int TotalCustomers;
        int TotalVendors;
        int CurrentPeriodNumber;
        int TotalPeriods;
        decimal FiscalProgressPercent;


        //Live Financial Pulse Data
        int TodaysTransactionCount;
        int ActiveCOA;
        int ActiveAccountGroups;
        int ActiveGLAccounts;
        int ActiveLedgers;
        int PendingReconciliations;
        int TodaysGLEntries;


        decimal WorkingCapital =>
    CustomerAccountStats.TotalOutstanding
    - VendorAccountStats.TotalOutstandingPayable;

        decimal TotalExposure =>
            CustomerAccountStats.TotalOutstanding
            + VendorAccountStats.TotalOutstandingPayable;

        decimal ReceivablePercent =>
            TotalExposure == 0 ? 0 :
            (CustomerAccountStats.TotalOutstanding / TotalExposure) * 100;

        decimal PayablePercent =>
            TotalExposure == 0 ? 0 :
            (VendorAccountStats.TotalOutstandingPayable / TotalExposure) * 100;


        [Inject] IJSRuntime JS { get; set; } = default!;

        List<CompanyModel> Companies = new();
        List<BranchModel> Branches = new();

        Guid? SelectedCompanyId;
        Guid? SelectedBranchId;



        protected override async Task OnInitializedAsync()
        {
            // 🔹 listen to global dashboard state change
            DashboardService.OnChange += HandleDashboardChanged;

            // 🔹 get current saved selection
            var saved = await DashboardService.GetDashboardDataAsync();

            SelectedCompanyId = saved.SelectedCompanyId;
            SelectedBranchId = saved.SelectedBranchId;

            LoadCompanies();

            await LoadDashboardData();
        }
        private async void HandleDashboardChanged()
        {
            var saved = await DashboardService.GetDashboardDataAsync();

            SelectedCompanyId = saved.SelectedCompanyId;
            SelectedBranchId = saved.SelectedBranchId;

            LoadCompanies();
            await LoadDashboardData();

            await InvokeAsync(StateHasChanged);
        }

        void LoadCompanies()
        {
            Companies = MasterDataService
                .GetAllCompanies()
                .Where(c => c.Status == "Active")
                .ToList();

            // Keep saved company if exists
            if (!SelectedCompanyId.HasValue)
            {
                SelectedCompanyId = Companies.FirstOrDefault()?.Id;
            }

            if (SelectedCompanyId.HasValue)
            {
                var selectedCompany = Companies.FirstOrDefault(c => c.Id == SelectedCompanyId);

                // Currency
                CurrencySymbol = MasterDataService
                    .GetAllCurrencies()
                    .FirstOrDefault(c => c.Id == selectedCompany?.BaseCurrencyId)
                    ?.Symbol ?? "";

                // Branch list
                Branches = BranchService
                    .GetByCompanyId(SelectedCompanyId.Value)
                    .Where(b => b.Status == "Active")
                    .ToList();

                // Keep saved branch if exists
                if (!SelectedBranchId.HasValue)
                {
                    SelectedBranchId =
                        Branches.FirstOrDefault(b => b.IsDefaultBranch)?.Id
                        ?? Branches.FirstOrDefault()?.Id;
                }
            }
        }


        async Task LoadDashboardData()
        {
            if (!SelectedCompanyId.HasValue)
                return;

            var companyId = SelectedCompanyId.Value;

            // ---------- Currency ----------
            var company = Companies.FirstOrDefault(c => c.Id == companyId);

            CurrencySymbol = MasterDataService
                .GetAllCurrencies()
                .FirstOrDefault(c => c.Id == company?.BaseCurrencyId)
                ?.Symbol ?? "-";

            // ---------- Opening / Closing ----------
            MasterDataService.ResetOpeningBalancesToSeed();

            var openingBal = MasterDataService
                .GetOpeningBalancesByCompany(companyId)
                .FirstOrDefault()?.TotalDebit ?? 0;

            var closingBal = MasterDataService
                .GetClosingBalancesByCompany(companyId)
                .FirstOrDefault()?.ClosingBalanceAmount ?? 0;

            OpeningBalance = FormatAmountInternational(openingBal);
            ClosingBalance = FormatAmountInternational(closingBal);

            // ---------- Current Period ----------
             fiscalYr = FiscalYearService.GetAll()
                .FirstOrDefault(fy => fy.CompanyId == companyId && fy.Status == FiscalYearStatus.Open);

            CurrentPeriod = AccountingPeriodService.GetAll()
                .FirstOrDefault(acc =>
                    acc.FiscalYearId == fiscalYr?.Id &&
                    DateTime.Today >= acc.StartDate &&
                    DateTime.Today <= acc.EndDate &&
                    acc.CompanyId == companyId &&
                    acc.Status == AccountingPeriodStatus.Open
                )?.PeriodName ?? string.Empty;

            // ---------- Pending journals ----------
            PendingJournals = JournalEntryService.GetAll()
                .Count(j =>
                    j.CompanyId == companyId &&
                    (j.Status == JournalEntryStatus.Draft ||
                     j.Status == JournalEntryStatus.Submitted ||
                     j.Status == JournalEntryStatus.Approved));
            CustomerAccountStats = CustomerAccountService.GetSummaryStats(companyId);
            VendorAccountStats = VendorAccountService.GetSummaryStats(companyId);
            TotalCustomers = CustomerAccountService.GetByCompanyId(companyId).Count();
            TotalVendors = VendorAccountService.GetByCompanyId(companyId).Count();

            // ---------- Top data ----------
            await LoadTopUnpostedJournals();
            await LoadTodaysTransactions();
            await LoadPendingRecons();
            await LoadTopExpenseTypes();
            await LoadBankAccounts();
            await LoadAgingData();
            await LoadBranchPerformance();
            await LoadFiscalProgressData();
            await LoadFinancialPulseData();
            if(Branches.Count == 1)
            {
                await LoadTopOverdueInvoices();

            }

            StateHasChanged();
        }


        public static string FormatAmountInternational(decimal? amount)
        {
            if (amount >= 1_000_000_000)      // Billion
                return $"{(amount / 1_000_000_000):0.#}B";

            if (amount >= 1_000_000)          // Million
                return $"{(amount / 1_000_000):0.#}M";

            if (amount >= 1_000)              // Thousand
                return $"{(amount / 1_000):0.#}K";

            return $"{amount:0}";
        }



        async Task LoadTopUnpostedJournals()
        {
            TopUnpostedJournalEntries = JournalEntryService
                .GetAll()
                .Where(j =>
                    j.CompanyId == SelectedCompanyId &&
                    j.BranchId == SelectedBranchId)
                .OrderByDescending(j => j.CreatedAt)
                .Take(5)
                .ToList();

            if(!TopUnpostedJournalEntries.Any())
            {
                TopUnpostedJournalEntries = JournalEntryService
                    .GetAll()
                    .Where(j =>
                        j.CompanyId == SelectedCompanyId)
                    .OrderByDescending(j => j.CreatedAt)
                    .Take(5)
                    .ToList();
            }
        }
        async Task LoadFiscalProgressData()
        {
            var periods = AccountingPeriodService.GetAll()
                .Where(p => p.CompanyId == SelectedCompanyId && p.FiscalYearId == fiscalYr?.Id)
                .OrderBy(p => p.StartDate)
                .ToList();

            TotalPeriods = periods.Count;

            var currentPeriod = periods
                .FirstOrDefault(p =>
                    DateTime.Now >= p.StartDate &&
                    DateTime.Now <= p.EndDate);

            CurrentPeriodNumber = currentPeriod != null
                ? periods.IndexOf(currentPeriod) + 1
                : 0;

            FiscalProgressPercent = TotalPeriods > 0
                ? Math.Round((decimal)CurrentPeriodNumber / TotalPeriods * 100, 0)
                : 0;
        }

        async Task LoadBranchPerformance()
        {
            var branches = BranchService
                .GetByCompanyId(SelectedCompanyId.Value)
                .Where(b => b.Status == "Active")
                .ToList();

            BranchPerformance = branches
                .Select(b => new
                {
                    b.BranchName,
                    Amount = TransactionService
                        .GetAll()
                        .Where(t =>
                            t.CompanyId == SelectedCompanyId &&
                            t.BranchId == b.Id)
                        .Sum(t => t.TransactionAmount)
                })
                .OrderByDescending(x => x.Amount)
                .Take(3)
                .Select(x => (x.BranchName, x.Amount))
                .ToList();
        }

        async Task LoadTopExpenseTypes()
        {
            allBills = BillService.GetAll()
                .Where(b => b.CompanyId == SelectedCompanyId && b.BranchId == SelectedBranchId)
                .ToList();

            TopExpenseTypes = allBills
                .GroupBy(b => b.BillType)
                .Select(g => new
                {
                    Type = g.Key,
                    Amount = g.Sum(x => x.GrandTotalAmount)
                })
                .OrderByDescending(x => x.Amount)
                .Take(3)
                .Select(x => (x.Type, x.Amount))
                .ToList();
        }

        async Task LoadAgingData()
        {
            Snapshots = AgingService.GetAll();
            CustomerAgingStatistics = AgingService.GetStatistics();
            LatestSnapshot = Snapshots
            .Where(s => s.SnapshotStatus == SnapshotStatuses.Completed && s.CompanyId ==SelectedCompanyId)
            .OrderByDescending(s => s.AsOfDate)
            .FirstOrDefault();

            if (LatestSnapshot != null)
            {
                BucketSummaryViewModel = AgingService.GetBucketSummary(LatestSnapshot.CustomerAgingId);
            }
        }
        async Task LoadTodaysTransactions()
        {
            TodaysTransactions = TransactionService
                .GetAll()
                .Where(t =>
                    t.CompanyId == SelectedCompanyId &&
                    (!SelectedBranchId.HasValue || t.BranchId == SelectedBranchId) && t.TransactionDate == DateTime.Today)
                .OrderByDescending(t => t.TransactionDate)
                .Take(5)
                .ToList();
        }

        async Task LoadPendingRecons()
        {
            PendingRecons = ReconService
                .GetAll()
                .Where(r =>
                    r.CompanyId == SelectedCompanyId &&
                    (r.ReconciliationStatus == ReconciliationStatus.Draft ||
                     r.ReconciliationStatus == ReconciliationStatus.InProgress))
                .OrderBy(r => r.PreparedOn)
                .Take(5)
                .ToList();
        }
        async Task OpenUnpostedJournalsModal()
        {
            TopUnpostedJournalEntries = JournalEntryService
                .GetAll()
                .Where(j =>
                    j.Status == JournalEntryStatus.Draft ||
                    j.Status == JournalEntryStatus.Submitted || j.Status == JournalEntryStatus.Approved)
                .OrderByDescending(j => j.CreatedAt)
                .Take(5)
                .ToList();

        }



        void OpenAccountModal(BankAccountModel Data)
        {
            SelectedBankAccount = Data;
        }

        async Task LoadTopOverdueInvoices()
        {
            var today = DateTime.Today;

            var invoices = InvoiceService.GetAll()
                .Where(i =>
                    i.CompanyId == SelectedCompanyId &&
                    (!SelectedBranchId.HasValue || i.BranchId == SelectedBranchId))
                .ToList();

            // 🔴 Overdue invoices
            var overdue = invoices
                .Where(i => i.AmountOutstanding > 0 && i.DueDate < today)
                .OrderByDescending(i => i.AmountOutstanding)
                .ToList();

            if (overdue.Any())
            {
                // 🟠 Remaining (non-overdue)
                var remaining = invoices
                    .Where(i => !(i.AmountOutstanding > 0 && i.DueDate < today))
                    .OrderByDescending(i => i.InvoiceDate) // latest invoices
                    .ToList();

                TopOverdueInvoices = overdue
                    .Concat(remaining)
                    .Take(5)
                    .ToList();
            }
            else
            {
                // 🟢 No overdue → show latest 5 invoices
                TopOverdueInvoices = invoices
                    .OrderByDescending(i => i.InvoiceDate)
                    .Take(5)
                    .ToList();
            }

            await Task.CompletedTask;
        }


        async Task LoadBankAccounts()
        {
            CashAccounts = CashAccountService.GetAll().First();
            BankAccounts = BankAccountService.GetAll()
                .Where(a =>
                    a.BranchId == SelectedBranchId &&
                    a.BankAccountStatus == "Active")
                .Take(2)
                .ToList();
        }
        async Task LoadFinancialPulseData()
        {
            // Today's transactions
            TodaysTransactionCount = TransactionService.GetAll()
                .Count(t =>
                    t.CompanyId == SelectedCompanyId &&
                    (!SelectedBranchId.HasValue || t.BranchId == SelectedBranchId) &&
                    t.TransactionDate == DateTime.Today);

            // 🔹 Get active COA list
            var coaList = (await COAService.GetChartOfAccountsAsync())
                .Where(a => a.CompanyId == SelectedCompanyId && a.Status == "Active")
                .ToList();

            // 🔹 Active COA count
            ActiveCOA = coaList.Count;

            // 🔹 Selected COA (safe)
            SelectedChartOfAccount = coaList.FirstOrDefault();

            // 🔹 Active Account Groups
            if (SelectedChartOfAccount != null)
            {
                var accountGroups = await COAService.GetAccountGroupsAsync();

                ActiveAccountGroups = accountGroups
                    .Count(g => g.ChartOfAccountsId == SelectedChartOfAccount.Id && g.Status == "Active");
            }
            else
            {
                ActiveAccountGroups = 0;
            }


            // Active GL Accounts
            ActiveGLAccounts = COAService.GetAllAccounts()
                .Count(g => g.ChartOfAccountsId == SelectedChartOfAccount.Id && g.Status == "Active");

            // Active Ledgers (Customer + Vendor or Ledger master)
            ActiveLedgers = FinanceDataService.GetAllLedgers()
                .Count(l => l.CompanyId == SelectedCompanyId && l.Status == "Active");

            // Pending reconciliations
            PendingReconciliations = ReconService.GetAll()
                .Count(r =>
                    r.CompanyId == SelectedCompanyId &&
                    (r.ReconciliationStatus == ReconciliationStatus.Draft ||
                     r.ReconciliationStatus == ReconciliationStatus.InProgress));
            TodaysGLEntries = MasterDataService.GetAllGeneralLedgerEntries()
                .Count(g =>
                    g.CompanyId == SelectedCompanyId &&
                    (!SelectedBranchId.HasValue || g.BranchId == SelectedBranchId) &&
                    g.EntryDate.Date == DateTime.Today);

        }


        void ViewAll(string path)
        {
            Nav.NavigateTo($"/{path}");

        }


        string GetStatusBadge(ReconciliationStatus status)
        {
            return status switch
            {
                ReconciliationStatus.Draft => "bg-secondary-transparent text-secondary",
                ReconciliationStatus.InProgress => "bg-info-transparent text-info",
                ReconciliationStatus.Completed => "bg-warning-transparent text-warning",
                ReconciliationStatus.Finalized => "bg-success-transparent text-success",
                ReconciliationStatus.Reopened => "bg-danger-transparent text-danger",
                ReconciliationStatus.Cancelled => "bg-dark-transparent text-dark",
                ReconciliationStatus.Failed => "bg-danger-transparent text-danger",
                _ => "bg-dark"
            };
        }

        private string GetAccStatusBadge(string status)
        {
            return status switch
            {

                "Active" => "bg-success-transparent text-success",
                "Inactive" => "bg-danger-transparent text-danger",
                "Closed" => "bg-warning-transparent text-warning",
                "Draft" => "bg-warning-transparent text-warning",
                _ => "bg-secondary-transparent text-secondary"
            };
        }
        private string GetStatusBadge(JournalEntryStatus status)
        {
            return status switch
            {
                JournalEntryStatus.Posted =>
                    "bg-success-transparent text-success",

                JournalEntryStatus.Cancelled =>
                    "bg-danger-transparent text-danger",

                JournalEntryStatus.Draft =>
                    "bg-warning-transparent text-warning",

                JournalEntryStatus.Approved =>
                    "bg-info-transparent text-info",

                JournalEntryStatus.Rejected =>
                    "bg-primary-transparent text-primary",

                _ =>
                    "bg-secondary-transparent text-secondary"
            };
        }

        public void Dispose()
        {
            DashboardService.OnChange -= HandleDashboardChanged;
        }
    }
}
