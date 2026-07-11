using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;

namespace FinanceConnect.Client.Pages
{
    public partial class Dashboard
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] DashboardService DashboardService { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] BranchService BranchService { get; set; } = default!;
        [Inject] JournalEntryService JournalEntryService { get; set; } = default!;
        [Inject] VendorBillService BillService { get; set; } = default!;
        [Inject] FiscalYearService FiscalYearService { get; set; } = default!;
        [Inject] AccountingPeriodService AccountingPeriodService { get; set; } = default!;

        List<CompanyModel> Companies = new();
        List<BranchModel> Branches = new();
        private List<(string Type, decimal Amount)> TopExpenseTypes = new();
        List<(string Type, decimal Amount)> ChartExpenseTypes = new();
        List<(string Type, decimal Amount)> AdditionalExpenseTypes = new();
        private List<VendorBillViewModel> allBills = new();

        Guid? SelectedCompanyId;
        Guid? SelectedBranchId;
        string CurrencySymbol = "";
        string CurrentPeriod = "";
        bool PeriodStatus = true;
        private FiscalYearModel? fiscalYr;

        int CompaniesWithoutActiveBranch;
        int CompaniesWithoutOpenFY;
        public class DashboardSummaryVm
        {
            public int CompanyCount { get; set; }
            public int BranchCount { get; set; }
            public int LedgerCount { get; set; }
            public int JournalCount { get; set; }

            public int CountryCount { get; set; }
            public int StateCount { get; set; }
            public int CityCount { get; set; }
            public int CurrencyCount { get; set; }
            public int ExchangeRateCount { get; set; }

            public int PendingOpeningBalanceBranches { get; set; }
        }

        DashboardSummaryVm Summary = new();

        public class ModuleSummaryRow
        {
            public string Module { get; set; } = "";
            public string Route { get; set; } = "";
            public int Active { get; set; }
            public int Inactive { get; set; }
            public int Draft { get; set; }

            public int Total => Active + Inactive + Draft;

            public decimal ActivePercent => Total == 0 ? 0 : (decimal)Active / Total * 100;
            public decimal InactivePercent => Total == 0 ? 0 : (decimal)Inactive / Total * 100;
            public decimal DraftPercent => Total == 0 ? 0 : (decimal)Draft / Total * 100;
        }

        private List<ModuleSummaryRow> ModuleSummaries = new();

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

            Summary.CompanyCount = Companies.Count;

            //Summary.BranchCount = SelectedCompanyId.HasValue
            //    ? BranchService.GetByCompanyId(SelectedCompanyId.Value)
            //        .Count(b => b.Status == "Active")
            //    : 0;
            Summary.BranchCount = BranchService.GetAll().Count(b => b.Status == "Active");

            // 🔹 Master data counts
            Summary.CountryCount = MasterDataService.GetAllCountries().Count();
            Summary.StateCount = MasterDataService.GetAllStateProvinces().Count();
            Summary.CityCount = MasterDataService.GetAllCities().Count();
            Summary.CurrencyCount = MasterDataService.GetAllCurrencies().Count();
            Summary.ExchangeRateCount = MasterDataService.GetAllExchangeRates().Count();

            // 🔹 Finance data (example — adjust to your services)
            Summary.LedgerCount = MasterDataService.GetAllLedgers().Count();
            Summary.JournalCount = JournalEntryService.GetAll().Count();

            // 🔹 Opening balance pending branches
            //Summary.PendingOpeningBalanceBranches =
            //    BranchService.GetBranchesWithoutOpeningBalance(SelectedCompanyId).Count();
            // ---------- Current Period ----------
            fiscalYr = FiscalYearService.GetAll()
                .FirstOrDefault(fy => fy.CompanyId == companyId && fy.Status == FiscalYearStatus.Open);

            PeriodStatus = fiscalYr != null;

            CurrentPeriod = AccountingPeriodService.GetAll()
                .FirstOrDefault(acc =>
                    acc.FiscalYearId == fiscalYr?.Id &&
                    DateTime.Today >= acc.StartDate &&
                    DateTime.Today <= acc.EndDate &&
                    acc.CompanyId == companyId &&
                    acc.Status == AccountingPeriodStatus.Open
                )?.PeriodName ?? string.Empty;

            CompaniesWithoutActiveBranch = Companies.Count(c => !BranchService.GetAll()
                .Any(b => b.CompanyId == c.Id && b.Status == "Active"));

            CompaniesWithoutOpenFY = Companies
                .Count(c => !FiscalYearService.GetAll()
                .Any(f => f.CompanyId == c.Id
               && f.Status == FiscalYearStatus.Open));
            await LoadTopExpenseTypes();
            await LoadModuleSummaries();
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

        async Task LoadTopExpenseTypes()
        {
            allBills = BillService.GetAll().ToList();

            TopExpenseTypes = allBills
                .GroupBy(b => b.BillType)
                .Select(g => new
                {
                    Type = g.Key,
                    Amount = g.Sum(x => x.GrandTotalAmount)
                })
                .OrderByDescending(x => x.Amount)
                .Take(5)   // 👈 TAKE 5
                .Select(x => (x.Type, x.Amount))
                .ToList();

            // First 3 → Chart
            ChartExpenseTypes = TopExpenseTypes.Take(3).ToList();

            // Remaining 2 → Bottom Section
            AdditionalExpenseTypes = TopExpenseTypes.Skip(3).ToList();
        }

        async Task LoadModuleSummaries()
        {
            var countries = MasterDataService.GetAllCountries();
            var states = MasterDataService.GetAllStateProvinces();
            var cities = MasterDataService.GetAllCities();
            var currencies = MasterDataService.GetAllCurrencies();
            var exchangeRates = MasterDataService.GetAllExchangeRates();

            ModuleSummaries = new List<ModuleSummaryRow>
    {
        new()
        {
            Module = "Country",
            Route = "/country",
            Active = countries.Count(c => c.IsActive),
            Inactive = countries.Count(c => !c.IsActive),
            Draft = 0
        },

        new()
        {
            Module = "State / Province",
            Route = "/stateprovince",
            Active = states.Count(s => s.Status == "Active"),
            Inactive = states.Count(s => !s.IsActive),
            Draft = states.Count(s => s.Status == "Draft")
        },

        new()
        {
            Module = "City",
            Route = "/city",
            Active = cities.Count(c => c.IsActive),
            Inactive = cities.Count(c => !c.IsActive),
            Draft = cities.Count(c => c.Status == "Draft")
        },

        new()
        {
            Module = "Currency",
            Route = "/currency",
            Active = currencies.Count(c => c.IsActive),
            Inactive = currencies.Count(c => !c.IsActive),
            Draft = 0
        },

        new()
        {
            Module = "Exchange Rate",
            Route = "/exchange-rate",
            Active = exchangeRates.Count(e => e.IsActive),
            Inactive = exchangeRates.Count(e => !e.IsActive),
            Draft = exchangeRates.Count(e => e.Status == "Draft")
        }
    };
        }



    }
}
