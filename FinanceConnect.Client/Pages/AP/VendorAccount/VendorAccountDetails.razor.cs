using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.AP.VendorAccount
{
    public partial class VendorAccountDetails
    {
        [Parameter] public Guid Id { get; set; }

        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject] VendorAccountService VendorAccountService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;

        private bool isInitialized = false;
        private VendorAccountViewModel? Account = null;

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
            isInitialized = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("feather.replace");
            }
        }

        private async Task LoadDataAsync()
        {
            await Task.Delay(100); // Simulate async load
            Account = VendorAccountService.GetById(Id);
        }

        #region Helper Methods

        private string GetStatusBadgeClass(string status)
        {
            return status switch
            {
                VendorAccountStatuses.Active => "bg-success-transparent text-success",
                VendorAccountStatuses.Frozen => "bg-danger-transparent text-danger",
                VendorAccountStatuses.Closed => "bg-secondary-transparent text-secondary",
                _ => "bg-light text-dark"
            };
        }

        #endregion
    }
}
