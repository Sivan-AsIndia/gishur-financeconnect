using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using FinanceConnect.Client.Services;
using static FinanceConnect.Client.ViewModels.VarianceAnalysisViewModel;

namespace FinanceConnect.Client.Pages.Budgeting_CostControl.VarianceAnalysis
{
    public partial class VarianceAnalysisView : ComponentBase
    {
        [Parameter] public Guid Id { get; set; }
        [Inject] private VarianceAnalysisService Service { get; set; } = default!;

        private VarianceAnalysisViewModel.VarianceAnalysis? Item;
        private bool isInitialized = false;

        protected override async Task OnParametersSetAsync()
        {
            isInitialized = false;
            Item = Service.GetById(Id);
            isInitialized = true;
            await Task.CompletedTask;
        }

        private string GetModeLabel(ComparisonModeEnum? mode) => mode switch
        {
            ComparisonModeEnum.BudgetVsActual => "Budget vs Actual",
            ComparisonModeEnum.BudgetVsForecast => "Budget vs Forecast",
            ComparisonModeEnum.ActualVsForecast => "Actual vs Forecast",
            ComparisonModeEnum.BudgetVsCommitted => "Budget vs Committed",
            ComparisonModeEnum.RevisedBudgetVsActual => "Revised Budget vs Actual",
            _ => "–"
        };

        private string GetStatusBadge(AnalysisStatusEnum status) => status switch
        {
            AnalysisStatusEnum.Draft => "bg-secondary-transparent",
            AnalysisStatusEnum.Generated => "bg-info-transparent",
            AnalysisStatusEnum.UnderReview => "bg-warning-transparent",
            AnalysisStatusEnum.Reviewed => "bg-primary-transparent",
            AnalysisStatusEnum.Approved => "bg-success-transparent",
            AnalysisStatusEnum.Locked => "bg-danger-transparent",
            AnalysisStatusEnum.Archived => "bg-dark",
            _ => "bg-secondary-transparent"
        };

        private MarkupString BoolBadge(bool value) =>
            value
                ? new MarkupString("<span class=\"badge badge-green\">Yes</span>")
                : new MarkupString("<span class=\"badge badge-gray\">No</span>");
    }
}
