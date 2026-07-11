using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using FinanceConnect.Client.Services;
using static FinanceConnect.Client.ViewModels.BudgetPeriodViewModel;

namespace FinanceConnect.Client.Pages.Budgeting_CostControl.BudgetPeriod
{
    public partial class CreateBudgetPeriod : ComponentBase
    {
        [Parameter] public Guid? Id { get; set; }
        [Inject] private BudgetPeriodService Service { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;

        private BudgetPeriodViewModel.BudgetPeriod Model = new();
        private EditContext _editContext = default!;
        private bool IsEdit => Id.HasValue;

        // Display helpers for optional FK GUIDs
        private string forecastRefDisplay
        {
            get => Model.ForecastReferenceId?.ToString() ?? "";
            set => Model.ForecastReferenceId = Guid.TryParse(value, out var g) ? g : null;
        }
        private string varianceRefDisplay
        {
            get => Model.VarianceAnalysisReferenceId?.ToString() ?? "";
            set => Model.VarianceAnalysisReferenceId = Guid.TryParse(value, out var g) ? g : null;
        }
        private string carryFromDisplay
        {
            get => Model.CarryForwardFromPeriodId?.ToString() ?? "";
            set => Model.CarryForwardFromPeriodId = Guid.TryParse(value, out var g) ? g : null;
        }
        private string carryToDisplay
        {
            get => Model.CarryForwardToPeriodId?.ToString() ?? "";
            set => Model.CarryForwardToPeriodId = Guid.TryParse(value, out var g) ? g : null;
        }

        protected override void OnInitialized()
        {
            if (IsEdit)
            {
                var e = Service.GetById(Id!.Value);
                if (e != null) Model = e;
            }
            else
            {
                Model.CompanyId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
                Model.TenantId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
                Model.FiscalYearId = Guid.Parse("22222222-2222-2222-2222-222222222222");
                Model.BudgetId = Guid.Parse("73000000-0000-0000-0000-000000000001");
            }
            _editContext = new EditContext(Model);
        }

        private async Task Save()
        {
            if (!_editContext.Validate()) return;

            if (Model.EndDate < Model.StartDate)
            {
                ToastService.ShowError("End Date must be >= Start Date", "Validation");
                return;
            }

            if (Model.PlannedBudgetAmount < 0)
            {
                ToastService.ShowError("Planned Amount must be >= 0", "Validation");
                return;
            }

            if (Model.ReleasedBudgetAmount.HasValue && Model.ReleasedBudgetAmount > Model.EffectiveBudgetAmount)
            {
                ToastService.ShowError("Released Amount cannot exceed Effective Amount", "Validation");
                return;
            }

            if (Model.PeriodStatus == PeriodStatusEnum.Revised && string.IsNullOrWhiteSpace(Model.RevisionReason))
            {
                ToastService.ShowError("Revision Reason is required when status is Revised", "Validation");
                return;
            }

            try
            {
                if (IsEdit)
                    await Service.UpdateAsync(Model);
                else
                    await Service.CreateAsync(Model);

                ToastService.ShowSuccess(IsEdit ? "Budget period updated" : "Budget period created", "Success");
                Nav.NavigateTo("/budget-periods");
            }
            catch (InvalidOperationException ex)
            {
                ToastService.ShowError(ex.Message, "Error");
            }
        }
    }
}
