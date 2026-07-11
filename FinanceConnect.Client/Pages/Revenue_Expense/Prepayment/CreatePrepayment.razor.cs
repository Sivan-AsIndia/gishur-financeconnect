using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using FinanceConnect.Client.Services;
using FinanceConnect.Client.Data;
using static FinanceConnect.Client.ViewModels.PrepaymentViewModel;

namespace FinanceConnect.Client.Pages.Revenue_Expense.Prepayment
{
    public partial class CreatePrepayment : ComponentBase
    {
        [Parameter] public Guid? Id { get; set; }
        [Inject] private PrepaymentService Service { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        private PrepaymentViewModel.Prepayment Model = new();
        private EditContext _editContext = default!;
        private bool IsEdit => Id.HasValue;

        private Dictionary<Guid, string> CurrencyList = new() { { MasterDataIds.Currencies.INR, "INR – Indian Rupee" }, { MasterDataIds.Currencies.USD, "USD – US Dollar" }, { MasterDataIds.Currencies.GBP, "GBP – British Pound" }, { MasterDataIds.Currencies.EUR, "EUR – Euro" }, { MasterDataIds.Currencies.AED, "AED – UAE Dirham" }, { MasterDataIds.Currencies.SGD, "SGD – Singapore Dollar" } };
        private Dictionary<Guid, string> FiscalYears = new() { { MasterDataIds.FiscalYears.FY2025_26, "FY 2025-26" }, { MasterDataIds.FiscalYears.FY2024_25, "FY 2024-25" } };
        private Dictionary<Guid, string> AccountingPeriods = new() { { MasterDataIds.AccountingPeriods.Apr2025, "Apr 2025" }, { MasterDataIds.AccountingPeriods.May2025, "May 2025" }, { MasterDataIds.AccountingPeriods.Apr2024, "Apr 2024" } };
        private Dictionary<Guid, string> GLAccounts = new() { { MasterDataIds.Accounts.SalariesWages, "6001 – Salaries & Wages" }, { MasterDataIds.Accounts.RentExpense, "6002 – Rent Expense" }, { MasterDataIds.Accounts.UtilitiesExpense, "6003 – Utilities Expense" }, { MasterDataIds.Accounts.CostOfMaterials, "5001 – Cost of Materials" }, { MasterDataIds.Accounts.FurnitureFixtures, "1100 – Furniture & Fixtures" }, { MasterDataIds.Accounts.AccountsPayable, "2001 – Accounts Payable" } };
        private Dictionary<Guid, string> Branches = new() { { MasterDataIds.Branches.SofaCraftHQ, "SofaCraft HQ - Chennai" }, { MasterDataIds.Branches.SofaCraftBengaluru, "SofaCraft - Bengaluru" } };

        protected override void OnInitialized()
        {
            if (IsEdit) { var e = Service.GetById(Id!.Value); if (e != null) Model = e; }
            else { Model.CompanyId = MasterDataIds.Companies.SofaCraft; Model.TenantId = MasterDataIds.Tenants.Default; Model.CurrencyId = MasterDataIds.Currencies.INR; Model.FiscalYearId = MasterDataIds.FiscalYears.FY2025_26; Model.PrepaymentCode = Service.GenerateCode(MasterDataIds.Companies.SofaCraft); Model.PreparedByUserId = "finance.admin"; }
            _editContext = new EditContext(Model);
        }

        private async Task Save()
        {
            if (!_editContext.Validate()) { await JS.InvokeVoidAsync("eval", "document.querySelector('.validation-message')?.scrollIntoView({behavior:'smooth',block:'center'})"); return; }
            try { if (IsEdit) { await Service.UpdateAsync(Model); ToastService.ShowSuccess("Prepayment updated."); } else { await Service.CreateAsync(Model); ToastService.ShowSuccess("Prepayment created."); } Nav.NavigateTo("/prepayments"); }
            catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message); }
        }
    }
}
