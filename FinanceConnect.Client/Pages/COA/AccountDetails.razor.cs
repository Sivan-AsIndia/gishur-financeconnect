using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.COA
{
    public partial class AccountDetails
    {
        [Parameter]
        public Guid AccountId { get; set; }

        [Inject]
        private COADataService COADataService { get; set; } = default!;

        [Inject]
        private NavigationManager Nav { get; set; } = default!;

        [Inject]
        private IJSRuntime JS { get; set; } = default!;

        private AccountViewModel? SelectedAccount { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadAccountAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("feather.replace");
            }
        }

        private async Task LoadAccountAsync()
        {
            var accounts = await COADataService.GetAccountsAsync();
            SelectedAccount = accounts.FirstOrDefault(a => a.Id == AccountId);
        }

        private void GoBack()
        {
            Nav.NavigateTo("/gl-accounts");
        }

        private string GetNatureBadgeClass(string? nature)
        {
            return nature switch
            {
                AccountNatures.Asset => "bg-success-transparent text-success",
                AccountNatures.Liability => "bg-danger-transparent text-danger",
                AccountNatures.Equity => "bg-purple-transparent text-purple",
                AccountNatures.Income => "bg-primary-transparent text-primary",
                AccountNatures.Expense => "bg-warning-transparent text-warning",
                _ => "bg-secondary-transparent text-secondary"
            };
        }

        private string GetStatusBadgeClass(string? status)
        {
            return status switch
            {
                AccountStatuses.Draft => "bg-secondary-transparent text-secondary",
                AccountStatuses.Active => "bg-success-transparent text-success",
                AccountStatuses.Inactive => "bg-secondary-transparent text-secondary",
                AccountStatuses.Suspended => "bg-warning-transparent text-warning",
                AccountStatuses.Closed => "bg-danger-transparent text-danger",
                _ => "bg-secondary-transparent text-secondary"
            };
        }

        private string GetLockStatusBadgeClass(string? lockStatus)
        {
            return lockStatus switch
            {
                LockStatuses.Unlocked or null or "" => "bg-success-transparent text-success",
                LockStatuses.LockedAfterPosting => "bg-warning-transparent text-warning",
                LockStatuses.LockedByController => "bg-danger-transparent text-danger",
                _ => "bg-secondary-transparent text-secondary"
            };
        }
    }
}
