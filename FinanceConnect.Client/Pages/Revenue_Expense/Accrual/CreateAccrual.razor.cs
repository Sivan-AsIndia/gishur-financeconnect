using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using FinanceConnect.Client.Services;
using FinanceConnect.Client.Data;
using static FinanceConnect.Client.ViewModels.AccrualViewModel;

namespace FinanceConnect.Client.Pages.Revenue_Expense.Accrual
{
    public partial class CreateAccrual : ComponentBase
    {
        [Parameter] public Guid? Id { get; set; }
        [Inject] private AccrualService Service { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        private AccrualViewModel.Accrual Model = new();
        private EditContext _editContext = default!;
        private bool IsEdit => Id.HasValue;

        private Dictionary<Guid, string> CurrencyList = new()
        {
            { MasterDataIds.Currencies.INR, "INR – Indian Rupee" }, { MasterDataIds.Currencies.USD, "USD – US Dollar" },
            { MasterDataIds.Currencies.GBP, "GBP – British Pound" }, { MasterDataIds.Currencies.EUR, "EUR – Euro" },
            { MasterDataIds.Currencies.AED, "AED – UAE Dirham" }, { MasterDataIds.Currencies.SGD, "SGD – Singapore Dollar" }
        };
        private Dictionary<Guid, string> FiscalYears = new()
        {
            { MasterDataIds.FiscalYears.FY2025_26, "FY 2025-26" }, { MasterDataIds.FiscalYears.FY2024_25, "FY 2024-25" }
        };
        private Dictionary<Guid, string> AccountingPeriods = new()
        {
            { MasterDataIds.AccountingPeriods.Apr2025, "Apr 2025" }, { MasterDataIds.AccountingPeriods.May2025, "May 2025" },
            { MasterDataIds.AccountingPeriods.Apr2024, "Apr 2024" }
        };
        private Dictionary<Guid, string> GLAccounts = new()
        {
            { MasterDataIds.Accounts.SalesRevenue, "4001 – Sales Revenue" }, { MasterDataIds.Accounts.ServiceRevenue, "4002 – Service Revenue" },
            { MasterDataIds.Accounts.CostOfMaterials, "5001 – Cost of Materials" }, { MasterDataIds.Accounts.SalariesWages, "6001 – Salaries & Wages" },
            { MasterDataIds.Accounts.RentExpense, "6002 – Rent Expense" }, { MasterDataIds.Accounts.UtilitiesExpense, "6003 – Utilities Expense" },
            { MasterDataIds.Accounts.AccountsPayable, "2001 – Accounts Payable" }, { MasterDataIds.Accounts.AccountsReceivable, "1003 – Accounts Receivable" },
            { MasterDataIds.Accounts.FurnitureFixtures, "1100 – Furniture & Fixtures" }
        };
        private Dictionary<Guid, string> Branches = new()
        {
            { MasterDataIds.Branches.SofaCraftHQ, "SofaCraft HQ - Chennai" }, { MasterDataIds.Branches.SofaCraftBengaluru, "SofaCraft - Bengaluru" },
            { MasterDataIds.Branches.SofaCraftDubai, "SofaCraft - Dubai" }
        };

        protected override void OnInitialized()
        {
            if (IsEdit)
            {
                var e = Service.GetById(Id!.Value);
                if (e != null) Model = e;
            }
            else
            {
                Model.CompanyId = MasterDataIds.Companies.SofaCraft;
                Model.TenantId = MasterDataIds.Tenants.Default;
                Model.CurrencyId = MasterDataIds.Currencies.INR;
                Model.FiscalYearId = MasterDataIds.FiscalYears.FY2025_26;
                Model.AccrualCode = Service.GenerateCode(MasterDataIds.Companies.SofaCraft);
                Model.PreparedByUserId = "finance.admin";
                Model.AccrualDate = DateTime.Today;
            }
            _editContext = new EditContext(Model);
        }

        private async Task Save()
        {
            if (!_editContext.Validate())
            {
                await JS.InvokeVoidAsync("eval", "document.querySelector('.validation-message')?.scrollIntoView({behavior:'smooth',block:'center'})");
                return;
            }
            try
            {
                if (IsEdit) { await Service.UpdateAsync(Model); ToastService.ShowSuccess("Accrual updated."); }
                else { await Service.CreateAsync(Model); ToastService.ShowSuccess("Accrual created."); }
                Nav.NavigateTo("/accruals");
            }
            catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message); }
        }
    }
}
