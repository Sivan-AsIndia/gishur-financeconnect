using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using FinanceConnect.Client.Services;
using static FinanceConnect.Client.ViewModels.BudgetLineViewModel;

namespace FinanceConnect.Client.Pages.Budgeting_CostControl.BudgetLine
{
    public partial class BudgetLineView : ComponentBase
    {
        [Parameter] public Guid Id { get; set; }
        [Inject] private BudgetLineService Service { get; set; } = default!;

        private BudgetLineViewModel.BudgetLine? Item;
        private bool isInitialized = false;

        protected override async Task OnParametersSetAsync()
        {
            isInitialized = false;
            Item = Service.GetById(Id);
            isInitialized = true;
            await Task.CompletedTask;
        }

        private string GetStatusBadge(LineStatusEnum s) => s switch
        {
            LineStatusEnum.Draft => "bg-secondary-transparent",
            LineStatusEnum.Active => "bg-success-transparent",
            LineStatusEnum.Revised => "bg-warning-transparent",
            LineStatusEnum.Locked => "bg-danger-transparent",
            LineStatusEnum.Closed => "bg-dark",
            LineStatusEnum.Archived => "bg-secondary-transparent",
            _ => "bg-secondary-transparent"
        };

        private string GetTypeBadgeColor(LineTypeEnum t) => t switch
        {
            LineTypeEnum.Expense => "bg-danger-transparent",
            LineTypeEnum.Revenue => "bg-success-transparent",
            LineTypeEnum.Capex => "bg-warning-transparent",
            LineTypeEnum.Statistical => "bg-info-transparent",
            LineTypeEnum.Transfer => "bg-primary-transparent",
            _ => "bg-secondary-transparent"
        };

        private MarkupString BoolBadge(bool value) =>
            value
                ? new MarkupString("<span class=\"badge badge-green\">Yes</span>")
                : new MarkupString("<span class=\"badge badge-gray\">No</span>");
    }
}
