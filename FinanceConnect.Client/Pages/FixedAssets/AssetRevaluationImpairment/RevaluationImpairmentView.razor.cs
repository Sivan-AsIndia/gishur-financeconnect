using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;

namespace FinanceConnect.Client.Pages.FixedAssets.AssetRevaluationImpairment
{
    public partial class RevaluationImpairmentView
    {
        [Parameter] public Guid Id { get; set; }

        [Inject] private AssetRevaluationImpairmentService Service { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;

        private AssetRevaluationImpairmentViewModel.AssetRevaluationImpairment? Item;
        private bool isInitialized = false;

        protected override async Task OnParametersSetAsync()
        {
            isInitialized = false;
            Item = await Service.GetByIdAsync(Id);
            isInitialized = true;
        }

        private string FormatEnumName(string? name)
        {
            if (string.IsNullOrEmpty(name)) return "";
            return string.Concat(name.Select((c, i) => i > 0 && char.IsUpper(c) ? " " + c : c.ToString()));
        }

        private string GetStatusBadgeClass(AssetRevaluationImpairmentViewModel.EventStatusEnum status) => status switch
        {
            AssetRevaluationImpairmentViewModel.EventStatusEnum.Draft => "bg-warning-transparent",
            AssetRevaluationImpairmentViewModel.EventStatusEnum.Submitted => "bg-info-transparent",
            AssetRevaluationImpairmentViewModel.EventStatusEnum.Approved => "bg-primary-transparent",
            AssetRevaluationImpairmentViewModel.EventStatusEnum.Rejected => "bg-danger-transparent",
            AssetRevaluationImpairmentViewModel.EventStatusEnum.Posted => "bg-success-transparent",
            AssetRevaluationImpairmentViewModel.EventStatusEnum.Cancelled => "bg-secondary-transparent",
            AssetRevaluationImpairmentViewModel.EventStatusEnum.Reversed => "bg-danger-transparent",
            AssetRevaluationImpairmentViewModel.EventStatusEnum.Closed => "bg-dark-transparent",
            _ => "bg-secondary-transparent"
        };

        private string GetEventTypeBadge(AssetRevaluationImpairmentViewModel.EventTypeEnum? type) => type switch
        {
            AssetRevaluationImpairmentViewModel.EventTypeEnum.RevaluationIncrease => "bg-success-transparent",
            AssetRevaluationImpairmentViewModel.EventTypeEnum.RevaluationDecrease => "bg-warning-transparent",
            AssetRevaluationImpairmentViewModel.EventTypeEnum.ImpairmentLoss => "bg-danger-transparent",
            AssetRevaluationImpairmentViewModel.EventTypeEnum.ImpairmentReversal => "bg-info-transparent",
            _ => "bg-secondary-transparent"
        };
    }
}
