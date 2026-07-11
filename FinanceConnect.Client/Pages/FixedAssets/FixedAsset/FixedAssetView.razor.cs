using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.FixedAssetViewModel;

namespace FinanceConnect.Client.Pages.FixedAssets.FixedAsset
{
    public partial class FixedAssetView
    {
        [Parameter] public Guid Id { get; set; }

        [Inject] private FixedAssetService FixedAssetService { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;

        private FixedAssetListDto? Asset { get; set; }
        private bool isInitialized = false;

        protected override async Task OnParametersSetAsync()
        {
            isInitialized = false;
            Asset = await FixedAssetService.GetByIdAsync(Id);
            isInitialized = true;

        }

        private async Task PrintPage()
            => await JS.InvokeVoidAsync("window.print");

        private static string GetStatusLabel(AssetStatus status) => status switch
        {
            AssetStatus.Draft => "Draft",
            AssetStatus.Active => "Active",
            AssetStatus.Inactive => "Inactive",
            AssetStatus.Disposed => "Disposed",
            _ => "Unknown"
        };

        private static string GetStatusBadgeClass(AssetStatus status) => status switch
        {
            AssetStatus.Draft => "bg-warning-transparent",
            AssetStatus.Active => "bg-success-transparent",
            AssetStatus.Inactive => "bg-danger-transparent",
            AssetStatus.Disposed => "bg-secondary-transparent",
            _ => "bg-light"
        };
    }
}
