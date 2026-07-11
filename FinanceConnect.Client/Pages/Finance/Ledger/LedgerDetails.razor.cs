using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Finance.Ledger
{
    public partial class LedgerDetails
    {
        [Parameter]
        public Guid LedgerId { get; set; }

        private LedgerModel? SelectedLedger;


        //protected override void OnInitialized()
        //{
        //}

        //private void GoBack()
        //{
        //    Nav.NavigateTo("/ledger");
        //}

        //private string GetStatusBadge(string? status) => status switch
        //{
        //    "Active" => "bg-success-transparent text-success",
        //    "Draft" => "bg-warning-transparent text-warning",
        //    "Inactive" => "bg-secondary-transparent text-secondary",
        //    _ => "bg-secondary-transparent text-secondary"
        //};

        //private string GetLockStatusBadge(string? lockStatus) => lockStatus switch
        //{
        //    "Unlocked" => "bg-success-transparent text-success",
        //    "LockedAfterPosting" => "bg-warning-transparent text-warning",
        //    "ManuallyLocked" => "bg-danger-transparent text-danger",
        //    _ => "bg-secondary-transparent text-secondary"
        //};

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("initTooltips");
            }
        }

        protected override void OnInitialized()
        {
            SelectedLedger = FinanceDataService
                .GetAllLedgers()
                .FirstOrDefault(l => l.Id == LedgerId);

            if (SelectedLedger == null)
            {
                Nav.NavigateTo("/ledgers");
            }
        }

        void GoBack()
        {
            Nav.NavigateTo("/ledgers");
        }

        private string GetStatusBadge(string status)
        {
            return status switch
            {
                LedgerStatus.Draft => "bg-warning-transparent text-warning",
                LedgerStatus.Active => "bg-success-transparent text-success",
                LedgerStatus.Inactive => "bg-danger-transparent text-danger",
                _ => "bg-secondary-transparent text-secondary"
            };
        }

        private string GetLockStatusBadge(string lockStatus)
        {
            return lockStatus switch
            {
                LockStatuses.Unlocked => "bg-success-transparent text-success",
                LockStatuses.LockedAfterPosting => "bg-warning-transparent text-warning",
                LockStatuses.LockedByController => "bg-danger-transparent text-danger",
                _ => "bg-light text-dark"
            };
        }

        private string GetLedgerTypeBadgeTag(string type)
        {
            return type switch
            {
                LedgerTypes.Primary => "tag-default",
                LedgerTypes.Management => "tag-default",
                LedgerTypes.IFRS => "tag-default",
                LedgerTypes.Tax => "tag-default",
                _ => "tag-default"
            };
        }
    }
}
