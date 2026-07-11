using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.AR.CustomerPayment
{
    public partial class CustomerPaymentDetails
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] CustomerPaymentService PaymentService { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        [Parameter] public Guid Id { get; set; }

        private bool isInitialized = false;
        private CustomerPaymentViewModel? Payment;

        protected override async Task OnInitializedAsync()
        {
            Payment = PaymentService.GetById(Id);
            isInitialized = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("initTooltips");
        }

        private async Task PrintPage()
        {
            await JS.InvokeVoidAsync("window.print");
        }

        private void GoBack()
        {
            Nav.NavigateTo("/customer-payments");
        }

        #region UI Helpers

        private string GetStatusBadgeClass(string status) => status switch
        {
            PaymentStatuses.Draft => "bg-secondary-transparent",
            PaymentStatuses.Submitted => "bg-info-transparent",
            PaymentStatuses.Approved => "bg-primary-transparent",
            PaymentStatuses.Posted => "bg-success-transparent",
            PaymentStatuses.Reversed => "bg-danger-transparent",
            PaymentStatuses.Cancelled => "bg-warning-transparent text-dark",
            _ => "bg-secondary-transparent"
        };

        private string GetMethodChipClass(string method) => method switch
        {
            PaymentMethods.Cash => "bg-success-transparent",
            PaymentMethods.BankTransfer => "bg-primary-transparent",
            PaymentMethods.UPI => "bg-info-transparent",
            PaymentMethods.Cheque => "bg-warning-transparent text-dark",
            PaymentMethods.Card => "bg-purple-transparent",
            PaymentMethods.Gateway => "bg-indigo-transparent",
            PaymentMethods.Wallet => "bg-teal-transparent",
            _ => "bg-secondary-transparent"
        };

        private string GetMethodIcon(string method) => method switch
        {
            PaymentMethods.Cash => "ti ti-cash",
            PaymentMethods.BankTransfer => "ti ti-building-bank",
            PaymentMethods.UPI => "ti ti-qrcode",
            PaymentMethods.Cheque => "ti ti-file-check",
            PaymentMethods.Card => "ti ti-credit-card",
            PaymentMethods.Gateway => "ti ti-world",
            PaymentMethods.Wallet => "ti ti-wallet",
            _ => "ti ti-coin"
        };

        private string GetInstrumentLabel() => Payment?.PaymentMethod switch
        {
            PaymentMethods.BankTransfer => "UTR Number",
            PaymentMethods.UPI => "UPI Reference",
            PaymentMethods.Cheque => "Cheque Number",
            PaymentMethods.Card => "Transaction Reference",
            PaymentMethods.Gateway => "Gateway Transaction ID",
            _ => "Reference Number"
        };

        #endregion
    }
}
