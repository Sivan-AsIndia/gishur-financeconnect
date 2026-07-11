using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;

namespace FinanceConnect.Client.Pages.FixedAssets.AssetAcquisition
{
    public partial class AssetAcquisitionView
    {
        [Parameter] public Guid Id { get; set; }

        [Inject] private AssetAcquisitionService Service { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;

        private AssetAcquisitionViewModel.AssetAcquisition? Item;
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

        private string GetStatusBadgeClass(AssetAcquisitionViewModel.AcquisitionStatusEnum status)
        {
            return status switch
            {
                AssetAcquisitionViewModel.AcquisitionStatusEnum.Draft => "bg-warning-transparent",
                AssetAcquisitionViewModel.AcquisitionStatusEnum.Submitted => "bg-info-transparent",
                AssetAcquisitionViewModel.AcquisitionStatusEnum.Approved => "bg-primary-transparent",
                AssetAcquisitionViewModel.AcquisitionStatusEnum.Rejected => "bg-danger-transparent",
                AssetAcquisitionViewModel.AcquisitionStatusEnum.Posted => "bg-success-transparent",
                AssetAcquisitionViewModel.AcquisitionStatusEnum.Cancelled => "bg-secondary-transparent",
                AssetAcquisitionViewModel.AcquisitionStatusEnum.Reversed => "bg-danger-transparent",
                _ => "bg-secondary-transparent"
            };
        }
    }
}
