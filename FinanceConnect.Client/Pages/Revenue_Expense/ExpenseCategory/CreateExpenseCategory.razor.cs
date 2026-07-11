using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using FinanceConnect.Client.Services;
using FinanceConnect.Client.Data;
using static FinanceConnect.Client.ViewModels.ExpenseCategoryViewModel;
using ExpenseCategoryModel = FinanceConnect.Client.ViewModels.ExpenseCategoryViewModel.ExpenseCategory;

namespace FinanceConnect.Client.Pages.Revenue_Expense.ExpenseCategory
{
    public partial class CreateExpenseCategory : ComponentBase
    {
        [Parameter] public Guid? Id { get; set; }
        [Inject] private ExpenseCategoryService Service { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        private ExpenseCategoryModel Model = new();
        private EditContext _editContext = default!;
        private bool IsEdit => Id.HasValue;
        private List<ExpenseCategoryModel> ParentCategories = new();

        private Dictionary<Guid, string> GLAccounts = new()
        {
            { MasterDataIds.Accounts.SalariesWages, "6001 – Salaries & Wages" },
            { MasterDataIds.Accounts.RentExpense, "6002 – Rent Expense" },
            { MasterDataIds.Accounts.UtilitiesExpense, "6003 – Utilities Expense" },
            { MasterDataIds.Accounts.CostOfMaterials, "5001 – Cost of Materials" },
            { MasterDataIds.Accounts.ServiceRevenue, "4002 – Service Revenue" },
            { MasterDataIds.Accounts.AccountsPayable, "2001 – Accounts Payable" },
            { MasterDataIds.Accounts.FurnitureFixtures, "1100 – Furniture & Fixtures" },
        };

        private Dictionary<Guid, string> TaxCodes = new()
        {
            { Guid.Parse("a1b2c3d4-0001-0001-0001-000000000001"), "GST-18 – GST 18%" },
            { Guid.Parse("a1b2c3d4-0002-0002-0002-000000000002"), "GST-12 – GST 12%" },
            { Guid.Parse("a1b2c3d4-0003-0003-0003-000000000003"), "GST-5 – GST 5%" },
            { Guid.Parse("a1b2c3d4-0004-0004-0004-000000000004"), "EXEMPT – Tax Exempt" },
        };

        protected override void OnInitialized()
        {
            ParentCategories = Service.GetAll();
            if (IsEdit)
            {
                var e = Service.GetById(Id!.Value);
                if (e != null) Model = e;
                ParentCategories = ParentCategories.Where(c => c.ExpenseCategoryId != Id!.Value).ToList();
            }
            else
            {
                Model.CompanyId = MasterDataIds.Companies.SofaCraft;
                Model.TenantId = MasterDataIds.Tenants.Default;
                Model.CategoryCode = Service.GenerateCode(MasterDataIds.Companies.SofaCraft);
                Model.PreparedByUserId = "finance.admin";
                Model.PreparedOn = DateTime.Today;
                Model.EffectiveFrom = DateTime.Today;
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
            if (Model.EffectiveTo.HasValue && Model.EffectiveFrom.HasValue && Model.EffectiveTo < Model.EffectiveFrom)
            { ToastService.ShowError("Effective To cannot be earlier than Effective From.", "Validation"); return; }
            if (Model.PrepaymentAllowedFlag && Model.PrepaymentAssetGLId == null)
            { ToastService.ShowError("Prepayment Asset GL is required when prepayment is allowed.", "Validation"); return; }
            if (Model.AccrualAllowedFlag && Model.AccrualLiabilityGLId == null)
            { ToastService.ShowError("Accrual Liability GL is required when accrual is allowed.", "Validation"); return; }
            if (!Model.ImmediateExpenseAllowedFlag && !Model.AccrualAllowedFlag && !Model.PrepaymentAllowedFlag)
            { ToastService.ShowError("At least one timing treatment must be allowed.", "Validation"); return; }

            try
            {
                // Populate display names from lookup
                if (GLAccounts.TryGetValue(Model.DefaultGLAccountId, out var glName)) Model.DefaultGLAccountName = glName;
                if (Model.AlternateGLAccountId.HasValue && GLAccounts.TryGetValue(Model.AlternateGLAccountId.Value, out var altGl)) Model.AlternateGLAccountName = altGl;
                if (Model.AccrualLiabilityGLId.HasValue && GLAccounts.TryGetValue(Model.AccrualLiabilityGLId.Value, out var accGl)) Model.AccrualLiabilityGLName = accGl;
                if (Model.PrepaymentAssetGLId.HasValue && GLAccounts.TryGetValue(Model.PrepaymentAssetGLId.Value, out var ppGl)) Model.PrepaymentAssetGLName = ppGl;
                if (Model.TaxDefaultCodeId.HasValue && TaxCodes.TryGetValue(Model.TaxDefaultCodeId.Value, out var tcName)) Model.TaxDefaultCodeName = tcName;

                if (IsEdit) await Service.UpdateAsync(Model);
                else await Service.CreateAsync(Model);
                ToastService.ShowSuccess(IsEdit ? "Category updated" : "Category created", "Success");
                Nav.NavigateTo("/expense-categories");
            }
            catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message, "Error"); }
        }
    }
}
