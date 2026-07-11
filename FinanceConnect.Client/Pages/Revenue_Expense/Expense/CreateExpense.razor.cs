using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using FinanceConnect.Client.Services;
using FinanceConnect.Client.Data;
using static FinanceConnect.Client.ViewModels.ExpenseViewModel;
using ExpenseModel = FinanceConnect.Client.ViewModels.ExpenseViewModel.Expense;
using ExpenseCategoryModel = FinanceConnect.Client.ViewModels.ExpenseCategoryViewModel.ExpenseCategory;

namespace FinanceConnect.Client.Pages.Revenue_Expense.Expense
{
    public partial class CreateExpense : ComponentBase
    {
        [Parameter] public Guid? Id { get; set; }
        [Inject] private ExpenseService Service { get; set; } = default!;
        [Inject] private ExpenseCategoryService CategoryService { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        private ExpenseModel Model = new();
        private EditContext _editContext = default!;
        private bool IsEdit => Id.HasValue;
        private List<ExpenseCategoryModel> Categories = new();

        private Dictionary<Guid, string> CurrencyList = new()
        {
            { MasterDataIds.Currencies.INR, "INR – Indian Rupee" },
            { MasterDataIds.Currencies.USD, "USD – US Dollar" },
            { MasterDataIds.Currencies.GBP, "GBP – British Pound" },
            { MasterDataIds.Currencies.EUR, "EUR – Euro" },
            { MasterDataIds.Currencies.AED, "AED – UAE Dirham" },
        };

        private Dictionary<Guid, string> FiscalYears = new()
        {
            { MasterDataIds.FiscalYears.FY2025_26, "FY 2025-26" },
            { MasterDataIds.FiscalYears.FY2024_25, "FY 2024-25" },
        };

        private Dictionary<Guid, string> GLAccounts = new()
        {
            { MasterDataIds.Accounts.SalariesWages, "6001 – Salaries & Wages" },
            { MasterDataIds.Accounts.RentExpense, "6002 – Rent Expense" },
            { MasterDataIds.Accounts.UtilitiesExpense, "6003 – Utilities Expense" },
            { MasterDataIds.Accounts.CostOfMaterials, "5001 – Cost of Materials" },
            { MasterDataIds.Accounts.ServiceRevenue, "4002 – Service Revenue" },
        };

        private Dictionary<Guid, string> BranchList = new()
        {
            { MasterDataIds.Branches.SofaCraftHQ, "SofaCraft Head Office – Chennai" },
        };

        protected override void OnInitialized()
        {
            Categories = CategoryService.GetAll();
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
                Model.ExpenseCode = Service.GenerateCode(MasterDataIds.Companies.SofaCraft);
                Model.PreparedByUserId = "finance.admin";
                Model.ExpenseDate = DateTime.Today;
                AddLine();
            }
            _editContext = new EditContext(Model);
        }

        private void AddLine()
        {
            var nextNum = Model.Lines.Any() ? Model.Lines.Max(l => l.LineNumber) + 10 : 10;
            Model.Lines.Add(new ExpenseLine { LineNumber = nextNum, AccrualTreatment = AccrualTreatmentEnum.Immediate });
        }

        private void RemoveLine(ExpenseLine line)
        {
            Model.Lines.Remove(line);
            RecalcTotals();
        }

        private void RecalcTotals(ChangeEventArgs? e = null)
        {
            foreach (var l in Model.Lines) l.GrossAmount = l.NetAmount + l.TaxAmount;
            Model.TotalNetAmount = Model.Lines.Sum(l => l.NetAmount);
            Model.TotalTaxAmount = Model.Lines.Sum(l => l.TaxAmount);
            Model.TotalGrossAmount = Model.Lines.Sum(l => l.GrossAmount);
            StateHasChanged();
        }

        private async Task Save()
        {
            RecalcTotals();

            if (!_editContext.Validate())
            {
                await JS.InvokeVoidAsync("eval", "document.querySelector('.validation-message')?.scrollIntoView({behavior:'smooth',block:'center'})");
                return;
            }

            if (!Model.Lines.Any())
            {
                ToastService.ShowError("At least one expense line is required.", "Validation");
                return;
            }

            if (Model.CoverageEndDate.HasValue && Model.CoverageStartDate.HasValue && Model.CoverageEndDate < Model.CoverageStartDate)
            {
                ToastService.ShowError("Coverage End Date must be >= Start Date.", "Validation");
                return;
            }

            if (Model.PrepaymentRequiredFlag && (!Model.CoverageStartDate.HasValue || !Model.CoverageEndDate.HasValue))
            {
                ToastService.ShowError("Coverage dates are required for prepayment treatment.", "Validation");
                return;
            }

            if (Model.ExpenseStatus == ExpenseStatusEnum.Rejected && string.IsNullOrWhiteSpace(Model.RejectionReason))
            {
                ToastService.ShowError("Rejection reason is required when status is Rejected.", "Validation");
                return;
            }

            try
            {
                if (IsEdit) await Service.UpdateAsync(Model);
                else await Service.CreateAsync(Model);
                ToastService.ShowSuccess(IsEdit ? "Expense updated" : "Expense created", "Success");
                Nav.NavigateTo("/expenses");
            }
            catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message, "Error"); }
        }
    }
}
