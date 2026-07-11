using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using FinanceConnect.Client.Services;
using static FinanceConnect.Client.ViewModels.BudgetPeriodViewModel;

namespace FinanceConnect.Client.Pages.Budgeting_CostControl.BudgetPeriod
{
    public partial class BudgetPeriodView : ComponentBase
    {
        [Parameter] public Guid Id { get; set; }
        [Inject] private BudgetPeriodService Service { get; set; } = default!;

        private BudgetPeriodViewModel.BudgetPeriod? Item;
        private bool isInitialized = false;

        protected override async Task OnParametersSetAsync()
        {
            isInitialized = false;
            Item = Service.GetById(Id);
            isInitialized = true;
            await Task.CompletedTask;
        }

        private string GetStatusBadge(PeriodStatusEnum s) => s switch
        {
            PeriodStatusEnum.Draft => "bg-secondary-transparent",
            PeriodStatusEnum.Open => "bg-info-transparent",
            PeriodStatusEnum.Released => "bg-primary-transparent",
            PeriodStatusEnum.Locked => "bg-danger-transparent",
            PeriodStatusEnum.Closed => "bg-dark",
            PeriodStatusEnum.Revised => "bg-warning-transparent",
            PeriodStatusEnum.Archived => "bg-secondary-transparent",
            _ => "bg-secondary-transparent"
        };

        private MarkupString BoolBadge(bool value) =>
            value
                ? new MarkupString("<span class=\"badge badge-green\">Yes</span>")
                : new MarkupString("<span class=\"badge badge-gray\">No</span>");
    }
}
