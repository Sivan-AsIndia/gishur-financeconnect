using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.AR.CustomerAccount
{
    public partial class CustomerAccountDetails
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject] CustomerAccountService CustomerAccountService { get; set; } = default!;

        [Parameter] public Guid Id { get; set; }

        private bool isInitialized = false;
        private CustomerAccountViewModel? Account;

        protected override async Task OnInitializedAsync()
        {
            Account = CustomerAccountService.GetById(Id);
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

        private static string GetStatusBadgeClass(string status) => status switch
        {
            CustomerAccountStatuses.Active => "bg-success-transparent text-success",
            CustomerAccountStatuses.Frozen => "bg-danger-transparent text-danger",
            CustomerAccountStatuses.Closed => "bg-secondary-transparent text-secondary",
            CustomerAccountStatuses.Inactive => "bg-warning-transparent text-warning",
            _ => "bg-secondary-transparent text-secondary"
        };

        private static string GetCollectionsBadgeClass(string? stage) => stage switch
        {
            CollectionsStages.None => "bg-secondary-transparent text-secondary",
            CollectionsStages.Reminder => "bg-info-transparent text-info",
            CollectionsStages.FirstNotice => "bg-warning-transparent text-warning",
            CollectionsStages.SecondNotice => "bg-warning-transparent text-warning",
            CollectionsStages.FinalNotice => "bg-danger-transparent text-danger",
            CollectionsStages.Legal => "bg-danger-transparent text-danger",
            CollectionsStages.WriteOff => "bg-dark-transparent text-dark",
            _ => "bg-secondary-transparent text-secondary"
        };
    }
}
