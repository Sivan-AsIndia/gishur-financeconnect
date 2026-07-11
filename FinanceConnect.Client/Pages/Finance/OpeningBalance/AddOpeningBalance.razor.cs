using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Finance.OpeningBalance
{
    public partial class AddOpeningBalance : ComponentBase
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;

        [Parameter] public Guid? Id { get; set; }

        public bool isInitialized = false;
        public OpeningBalanceModel OpeningBalance = new();
        public List<CompanyModel> Companies = new();
        public List<LedgerModel> Ledgers = new();
        public List<(Guid Id, string Code, string Name)> Branches = new();
        public List<(Guid Id, string Name)> FiscalYears = new();
        public List<(Guid Id, string Code, string Name)> Accounts = new();
        public List<(Guid Id, string Name)> AccountingPeriods = new();
        public List<CurrencyModel> Currencies = new();

        public bool IsEdit => Id.HasValue;
        public bool CanEdit => OpeningBalance.Status == "Draft";
        public bool IsMultiCurrency => OpeningBalance.CurrencyMode == CurrencyModes.MultiCurrencyAllowed;
        public string PageTitle => IsEdit ? "Edit Opening Balance" : "Create Opening Balance";
        public string PageSubTitle => IsEdit ? $"OB #{OpeningBalance.OpeningBalanceNumber}" : "Create new opening balance entry";

        protected override async Task OnInitializedAsync()
        {
            Companies = MasterDataService.GetAllCompanies().Where(c => c.Status == "Active").ToList();
            Ledgers = MasterDataService.GetAllLedgers().Where(l => l.Status == "Active").ToList();

            Branches = new List<(Guid, string, string)>
            {
                (Guid.Parse("b0000001-0001-0001-0001-000000000001"), "HQ-MUM", "Head Office Mumbai"),
                (Guid.Parse("b0000002-0002-0002-0002-000000000002"), "BR-DEL", "Delhi Branch"),
                (Guid.Parse("b0000003-0003-0003-0003-000000000003"), "BR-BLR", "Bangalore Branch")
            };

            FiscalYears = new List<(Guid, string)>
            {
                (Guid.Parse("f0000001-0001-0001-0001-000000000001"), "FY 2025-26"),
                (Guid.Parse("f0000002-0002-0002-0002-000000000002"), "FY 2024-25")
            };

            Accounts = new List<(Guid, string, string)>
            {
                (Guid.Parse("acc00001-0001-0001-0001-000000000001"), "1000", "Cash at Bank"),
                (Guid.Parse("acc00002-0002-0002-0002-000000000002"), "1100", "Cash in Hand"),
                (Guid.Parse("acc00004-0004-0004-0004-000000000004"), "1200", "Accounts Receivable"),
                (Guid.Parse("acc00005-0005-0005-0005-000000000005"), "1500", "Fixed Assets"),
                (Guid.Parse("acc00010-0010-0010-0010-000000000010"), "2100", "Accounts Payable"),
                (Guid.Parse("acc00011-0011-0011-0011-000000000011"), "2500", "Term Loan"),
                (Guid.Parse("acc00012-0012-0012-0012-000000000012"), "3000", "Share Capital"),
                (Guid.Parse("acc00013-0013-0013-0013-000000000013"), "3500", "Retained Earnings")
            };

            AccountingPeriods = new List<(Guid, string)>
            {
                (Guid.Parse("a0000001-0001-0001-0001-000000000001"), "April 2025"),
                (Guid.Parse("a0000002-0002-0002-0002-000000000002"), "May 2025"),
                (Guid.Parse("a0000003-0003-0003-0003-000000000003"), "June 2025"),
                (Guid.Parse("a0000004-0004-0004-0004-000000000004"), "July 2025")
            };

            Currencies = MasterDataService.GetAllCurrencies().Where(c => c.IsActive).ToList();

            if (IsEdit)
            {
                var existing = MasterDataService.GetOpeningBalanceById(Id!.Value);
                if (existing != null) { OpeningBalance = existing; }
                else { Nav.NavigateTo("/opening-balance"); return; }
            }

            isInitialized = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender) => await JS.InvokeVoidAsync("feather.replace");

        public string GetStatusBadge(string status) => status switch
        {
            "Posted" => "bg-success",
            "Approved" => "bg-info",
            "Submitted" => "bg-warning",
            "Draft" => "bg-secondary",
            "Cancelled" => "bg-danger",
            _ => "bg-secondary"
        };

        public void AddLine()
        {
            OpeningBalance.Lines.Add(new OpeningBalanceLineModel { Id = Guid.NewGuid(), OpeningBalanceId = OpeningBalance.Id });
        }

        public void RemoveLine(int index)
        {
            if (index >= 0 && index < OpeningBalance.Lines.Count) { OpeningBalance.Lines.RemoveAt(index); RecalculateTotals(); }
        }

        public void RecalculateTotals()
        {
            // TotalDebit and TotalCredit are computed properties, no need to assign them
            // They automatically calculate from Lines.Sum(l => l.DebitAmountBase) and Lines.Sum(l => l.CreditAmountBase)

            foreach (var line in OpeningBalance.Lines)
            {
                var account = Accounts.FirstOrDefault(a => a.Id == line.AccountId);
                if (account.Id != Guid.Empty) { line.AccountCode = account.Code; line.AccountName = account.Name; }
            }
        }

        public void Save()
        {
            RecalculateTotals();

            var company = Companies.FirstOrDefault(c => c.Id == OpeningBalance.CompanyId);
            OpeningBalance.CompanyCode = company?.CompanyCode;
            OpeningBalance.CompanyName = company?.LegalName;

            var branch = Branches.FirstOrDefault(b => b.Id == OpeningBalance.BranchId);
            OpeningBalance.BranchCode = branch.Code;
            OpeningBalance.BranchName = branch.Name;

            var ledger = Ledgers.FirstOrDefault(l => l.Id == OpeningBalance.LedgerId);
            OpeningBalance.LedgerCode = ledger?.LedgerCode;
            OpeningBalance.LedgerName = ledger?.LedgerName;

            var fy = FiscalYears.FirstOrDefault(f => f.Id == OpeningBalance.FiscalYearId);
            OpeningBalance.FiscalYearName = fy.Name;

            var period = AccountingPeriods.FirstOrDefault(p => p.Id == OpeningBalance.OpeningAccountingPeriodId);
            OpeningBalance.OpeningAccountingPeriodName = period.Name;

            // Derive CurrencyMode from selected Ledger
            var selectedLedger = Ledgers.FirstOrDefault(l => l.Id == OpeningBalance.LedgerId);
            OpeningBalance.CurrencyMode = selectedLedger?.CurrencyMode ?? CurrencyModes.SingleCurrencyOnly;

            if (IsEdit) { MasterDataService.UpdateOpeningBalance(OpeningBalance); ToastService.ShowSuccess("Opening Balance updated", "Updated"); }
            else { MasterDataService.AddOpeningBalance(OpeningBalance); ToastService.ShowSuccess("Opening Balance created", "Created"); }

            Nav.NavigateTo("/opening-balance");
        }
    }
}
