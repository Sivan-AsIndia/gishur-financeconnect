using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.FixedAssets.AssetDisposal
{
    public partial class AssetDisposalDetails
    {

        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] AssetDisposalService DisposalService { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;

        [Parameter] public Guid Id { get; set; }

        private bool isInitialized = false;
        private AssetDisposalViewModel? Disposal;

        protected override async Task OnInitializedAsync()
        {
            Disposal = await DisposalService.GetByIdAsync(Id);

            isInitialized = true;
        }

        private async Task PrintPage()
        {
            await JS.InvokeVoidAsync("window.print");
        }

        private string GetStatusBadge(AssetDisposalStatus status)
        {
            return status switch
            {
                AssetDisposalStatus.Draft => "bg-secondary-transparent",
                AssetDisposalStatus.Submitted => "bg-info-transparent",
                AssetDisposalStatus.Approved => "bg-primary-transparent",
                AssetDisposalStatus.Posted => "bg-success-transparent",
                AssetDisposalStatus.Cancelled => "bg-warning-transparent",
                AssetDisposalStatus.Reversed => "bg-danger-transparent",
                _ => "bg-secondary-transparent"
            };
        }
    }
}
