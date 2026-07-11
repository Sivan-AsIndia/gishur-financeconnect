using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using FinanceConnect.Client.Services;
using static FinanceConnect.Client.ViewModels.BudgetLineViewModel;

namespace FinanceConnect.Client.Pages.Budgeting_CostControl.BudgetLine
{
    public partial class CreateBudgetLine : ComponentBase
    {
        [Parameter] public Guid? Id { get; set; }
        [Inject] private BudgetLineService Service { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;

        private BudgetLineViewModel.BudgetLine Model = new();
        private EditContext _editContext = default!;
        private bool IsEdit => Id.HasValue;

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
                Model.BudgetId = Guid.Parse("73000000-0000-0000-0000-000000000001");
            }
            _editContext = new EditContext(Model);
        }

        private async Task Save()
        {
            if (!_editContext.Validate()) return;

            if (Model.OriginalPlannedAmount < 0)
            {
                ToastService.ShowError("Planned Amount must be >= 0", "Validation");
                return;
            }

            if (Model.RevisedAmount.HasValue && Model.RevisedAmount < 0)
            {
                ToastService.ShowError("Revised Amount must be >= 0", "Validation");
                return;
            }

            if (Model.ReleasedAmount.HasValue && Model.ReleasedAmount > Model.EffectiveBudgetAmount)
            {
                ToastService.ShowError("Released Amount cannot exceed Effective Budget", "Validation");
                return;
            }

            if (Model.LineStatus == LineStatusEnum.Revised && string.IsNullOrWhiteSpace(Model.RevisionReason))
            {
                ToastService.ShowError("Revision Reason is required when status is Revised", "Validation");
                return;
            }

            if (Model.LineType == LineTypeEnum.Capex && !Model.IsCapexFlag)
            {
                Model.IsCapexFlag = true;
            }

            try
            {
                if (IsEdit)
                    await Service.UpdateAsync(Model);
                else
                    await Service.CreateAsync(Model);

                ToastService.ShowSuccess(IsEdit ? "Budget line updated" : "Budget line created", "Success");
                Nav.NavigateTo("/budget-lines");
            }
            catch (InvalidOperationException ex)
            {
                ToastService.ShowError(ex.Message, "Error");
            }
        }
    }
}
