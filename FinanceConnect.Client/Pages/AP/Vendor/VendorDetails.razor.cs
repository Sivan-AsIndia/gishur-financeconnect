using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.AP.Vendor
{
    public partial class VendorDetails
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject] VendorService VendorService { get; set; } = default!;

        [Parameter] public Guid Id { get; set; }

        private bool isInitialized = false;
        private VendorViewModel? Vendor;

        protected override async Task OnInitializedAsync()
        {
            Vendor = VendorService.GetById(Id);
            isInitialized = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("feather.replace");
                await JS.InvokeVoidAsync("initTooltips");
            }
        }

        private void GoBack()
        {
            Nav.NavigateTo("/vendors");
        }

        private static string GetStatusBadgeClass(string? status) => status switch
        {
            VendorStatuses.Active => "bg-success-transparent text-success",
            VendorStatuses.Inactive => "bg-secondary-transparent text-secondary",
            VendorStatuses.OnHold => "bg-warning-transparent text-warning",
            VendorStatuses.Blacklisted => "bg-danger-transparent text-danger",
            VendorStatuses.Draft => "bg-info-transparent text-info",
            _ => "bg-secondary-transparent text-secondary"
        };

        private static string GetTypeBadgeClass(string type) => type switch
        {
            VendorTypes.Supplier => "bg-primary-transparent text-primary",
            VendorTypes.ServiceProvider => "bg-info-transparent text-info",
            VendorTypes.Contractor => "bg-warning-transparent text-warning",
            VendorTypes.Freelancer => "bg-success-transparent text-success",
            VendorTypes.Utility => "bg-secondary-transparent text-secondary",
            VendorTypes.Landlord => "bg-purple-transparent text-purple",
            VendorTypes.Government => "bg-danger-transparent text-danger",
            _ => "bg-secondary-transparent text-secondary"
        };

        private static string FormatFileSize(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:N1} KB";
            return $"{bytes / (1024.0 * 1024.0):N2} MB";
        }
    }
}
