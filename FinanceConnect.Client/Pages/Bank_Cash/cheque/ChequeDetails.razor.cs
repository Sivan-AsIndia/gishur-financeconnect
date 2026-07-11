using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Bank_Cash.cheque
{
    public partial class ChequeDetails
    {
        [Parameter]
        public Guid ChequeId { get; set; }

        private ChequeModel? SelectedCheque;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("initTooltips");
            }
        }

        protected override void OnInitialized()
        {
            SelectedCheque = Service.GetById(ChequeId);

            if (SelectedCheque == null)
            {
                Nav.NavigateTo("/cheques");
            }
        }

        void GoBack()
        {
            Nav.NavigateTo("/cheques");
        }

        private string GetStatusBadge(ChequeStatus status)
        {
            return status switch
            {
                ChequeStatus.Draft => "bg-warning-transparent text-warning",
                ChequeStatus.Prepared => "bg-info-transparent text-info",
                ChequeStatus.Printed => "bg-info-transparent text-info",
                ChequeStatus.Issued => "bg-primary-transparent text-primary",
                ChequeStatus.Received => "bg-success-transparent text-success",
                ChequeStatus.Deposited => "bg-success-transparent text-success",
                ChequeStatus.Presented => "bg-info-transparent text-info",
                ChequeStatus.Cleared => "bg-success-transparent text-success",
                ChequeStatus.Bounced => "bg-danger-transparent text-danger",
                ChequeStatus.Stopped => "bg-secondary-transparent text-secondary",
                ChequeStatus.Cancelled => "bg-dark-transparent text-dark",
                ChequeStatus.Stale => "bg-secondary-transparent text-secondary",
                ChequeStatus.Reissued => "bg-primary-transparent text-primary",
                _ => "bg-secondary-transparent text-secondary"
            };
        }

        private string GetStatusIcon(ChequeStatus status)
        {
            return status switch
            {
                ChequeStatus.Draft => "ti ti-pencil",
                ChequeStatus.Prepared => "ti ti-clipboard-check",
                ChequeStatus.Printed => "ti ti-printer",
                ChequeStatus.Issued => "ti ti-send",
                ChequeStatus.Received => "ti ti-inbox",
                ChequeStatus.Deposited => "ti ti-building-bank",
                ChequeStatus.Presented => "ti ti-arrow-right",
                ChequeStatus.Cleared => "ti ti-check",
                ChequeStatus.Bounced => "ti ti-alert-triangle",
                ChequeStatus.Stopped => "ti ti-player-stop",
                ChequeStatus.Cancelled => "ti ti-x",
                ChequeStatus.Stale => "ti ti-clock",
                ChequeStatus.Reissued => "ti ti-refresh",
                _ => "ti ti-circle"
            };
        }

        private string GetStatusHeaderClass(ChequeStatus status)
        {
            return status switch
            {
                ChequeStatus.Draft => "status-draft",
                ChequeStatus.Cleared or ChequeStatus.Deposited or ChequeStatus.Received => "status-active",
                ChequeStatus.Cancelled or ChequeStatus.Bounced or ChequeStatus.Stopped => "status-inactive",
                _ => "status-draft"
            };
        }
    }
}
