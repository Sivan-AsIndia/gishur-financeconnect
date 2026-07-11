using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using FinanceConnect.Client.Services;
using static FinanceConnect.Client.ViewModels.ForecastViewModel;

namespace FinanceConnect.Client.Pages.Budgeting_CostControl.Forecast
{
    public partial class ForecastView : ComponentBase
    {
        [Parameter] public Guid Id { get; set; }
        [Inject] private ForecastService Service { get; set; } = default!;

        private ForecastViewModel.Forecast? Item;
        private bool isInitialized = false;

        protected override async Task OnParametersSetAsync()
        {
            isInitialized = false;
            Item = Service.GetById(Id);
            isInitialized = true;
            await Task.CompletedTask;
        }

        private string GetStatusBadge(ForecastStatusEnum s) => s switch
        {
            ForecastStatusEnum.Draft => "bg-secondary-transparent text-secondary",
            ForecastStatusEnum.Generated => "bg-info-transparent",
            ForecastStatusEnum.UnderReview => "bg-warning-transparent",
            ForecastStatusEnum.Reviewed => "bg-primary-transparent",
            ForecastStatusEnum.Approved => "bg-success-transparent",
            ForecastStatusEnum.Locked => "bg-danger-transparent",
            ForecastStatusEnum.Archived => "bg-dark",
            ForecastStatusEnum.Superseded => "bg-secondary-transparent text-secondary",
            _ => "bg-secondary-transparent"
        };

        private MarkupString BoolBadge(bool value) =>
            value
                ? new MarkupString("<span class=\"badge badge-green\">Yes</span>")
                : new MarkupString("<span class=\"badge badge-gray\">No</span>");
    }
}
