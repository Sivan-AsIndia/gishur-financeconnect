using Microsoft.AspNetCore.Components;
using FinanceConnect.Client.Services;
using static FinanceConnect.Client.ViewModels.FinancialPlanViewModel;

namespace FinanceConnect.Client.Pages.Budgeting_CostControl.FinancialPlan
{
    public partial class PlanDetails : ComponentBase
    {
        [Parameter] public Guid Id { get; set; }
        [Inject] private FinancialPlanService Service { get; set; } = default!;

        private FinancialPlanListDto? Item;
        private bool isInitialized = false;

        protected override async Task OnParametersSetAsync()
        {
            isInitialized = false;
            Item = Service.GetById(Id);
            isInitialized = true;
            await Task.CompletedTask;
        }

        private bool HasAssumptions() => Item != null && (
            !string.IsNullOrWhiteSpace(Item.RevenueAssumptionText) ||
            !string.IsNullOrWhiteSpace(Item.ExpenseAssumptionText) ||
            !string.IsNullOrWhiteSpace(Item.CapexAssumptionText) ||
            !string.IsNullOrWhiteSpace(Item.MarketAssumptionText) ||
            !string.IsNullOrWhiteSpace(Item.RiskAssumptionText) ||
            !string.IsNullOrWhiteSpace(Item.OpportunityAssumptionText) ||
            !string.IsNullOrWhiteSpace(Item.StrategicNarrative));

        private string GetStatusBadge(PlanStatusEnum s) => s switch
        {
            PlanStatusEnum.Draft => "bg-secondary-transparent",
            PlanStatusEnum.UnderPreparation => "bg-info-transparent",
            PlanStatusEnum.UnderReview => "bg-warning-transparent",
            PlanStatusEnum.Approved => "bg-success-transparent",
            PlanStatusEnum.Locked => "bg-danger-transparent",
            PlanStatusEnum.Superseded => "bg-secondary-transparent",
            PlanStatusEnum.Archived => "bg-dark",
            _ => "bg-secondary-transparent"
        };
    }
}
