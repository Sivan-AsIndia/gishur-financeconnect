using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using FinanceConnect.Client.Services;
using FinanceConnect.Client.Data;
using static FinanceConnect.Client.ViewModels.ExpenseClaimViewModel;
using ExpenseClaimModel = FinanceConnect.Client.ViewModels.ExpenseClaimViewModel.ExpenseClaim;
using ExpenseCategoryModel = FinanceConnect.Client.ViewModels.ExpenseCategoryViewModel.ExpenseCategory;
using UsageScopeEnum = FinanceConnect.Client.ViewModels.ExpenseCategoryViewModel.UsageScopeEnum;

namespace FinanceConnect.Client.Pages.Revenue_Expense.ExpenseClaim
{
    public partial class CreateExpenseClaim : ComponentBase
    {
        [Parameter] public Guid? Id { get; set; }
        [Inject] private ExpenseClaimService Service { get; set; } = default!;
        [Inject] private ExpenseCategoryService CategoryService { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        private ExpenseClaimModel Model = new();
        private EditContext _editContext = default!;
        private bool IsEdit => Id.HasValue;
        private List<ExpenseCategoryModel> Categories = new();

        private Dictionary<Guid, string> CurrencyList = new()
        {
            { MasterDataIds.Currencies.INR, "INR – Indian Rupee" },
            { MasterDataIds.Currencies.USD, "USD – US Dollar" },
            { MasterDataIds.Currencies.GBP, "GBP – British Pound" },
            { MasterDataIds.Currencies.EUR, "EUR – Euro" },
        };

        private Dictionary<Guid, string> BranchList = new()
        {
            { MasterDataIds.Branches.SofaCraftHQ, "SofaCraft Head Office – Chennai" },
        };

        protected override void OnInitialized()
        {
            Categories = CategoryService.GetAll().Where(c => c.EmployeeClaimAllowedFlag || c.UsageScope == UsageScopeEnum.General).ToList();
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
                Model.ClaimCode = Service.GenerateCode(MasterDataIds.Companies.SofaCraft);
                Model.ClaimSubmissionDate = DateTime.Today;
                AddLine();
            }
            _editContext = new EditContext(Model);
        }

        private void AddLine()
        {
            var nextNum = Model.Lines.Any() ? Model.Lines.Max(l => l.LineNumber) + 10 : 10;
            Model.Lines.Add(new ExpenseClaimLine { LineNumber = nextNum, ExpenseDate = DateTime.Today });
        }

        private void RemoveLine(ExpenseClaimLine line)
        {
            Model.Lines.Remove(line);
            RecalcTotals();
        }

        private void RecalcTotals(ChangeEventArgs? e = null)
        {
            foreach (var l in Model.Lines)
            {
                l.GrossAmount = l.ClaimedAmount;
                l.RejectedAmount = Math.Max(0, l.ClaimedAmount - l.ApprovedAmount);
            }
            Model.TotalClaimedAmount = Model.Lines.Sum(l => l.ClaimedAmount);
            Model.TotalApprovedAmount = Model.Lines.Sum(l => l.ApprovedAmount);
            Model.TotalRejectedAmount = Model.Lines.Sum(l => l.RejectedAmount);
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
                ToastService.ShowError("At least one claim line is required.", "Validation");
                return;
            }

            if (Model.ClaimPeriodTo.HasValue && Model.ClaimPeriodFrom.HasValue && Model.ClaimPeriodTo < Model.ClaimPeriodFrom)
            {
                ToastService.ShowError("Claim Period To must be >= Period From.", "Validation");
                return;
            }

            foreach (var l in Model.Lines)
            {
                if (l.ApprovedAmount > l.ClaimedAmount)
                {
                    ToastService.ShowError($"Line {l.LineNumber}: Approved amount cannot exceed claimed amount.", "Validation");
                    return;
                }
            }

            if (Model.ClaimStatus == ClaimStatusEnum.Cancelled && string.IsNullOrWhiteSpace(Model.CancellationReason))
            {
                ToastService.ShowError("Cancellation reason is required.", "Validation");
                return;
            }

            try
            {
                if (IsEdit) await Service.UpdateAsync(Model);
                else await Service.CreateAsync(Model);
                ToastService.ShowSuccess(IsEdit ? "Claim updated" : "Claim created", "Success");
                Nav.NavigateTo("/expense-claims");
            }
            catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message, "Error"); }
        }
    }
}
