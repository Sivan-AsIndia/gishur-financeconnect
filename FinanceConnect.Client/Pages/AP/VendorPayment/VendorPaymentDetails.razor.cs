using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.AP.VendorPayment
{
    public partial class VendorPaymentDetails
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] VendorPaymentService PaymentService { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        [Parameter] public Guid Id { get; set; }

        private bool isInitialized = false;
        private VendorPaymentViewModel? Payment;

        protected override async Task OnInitializedAsync()
        {
            Payment = PaymentService.GetById(Id);
            isInitialized = true;
        }

        private async Task PrintPage()
        {
            await JS.InvokeVoidAsync("window.print");
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("initTooltips");

        }


        private void GoBack()
        {
            Nav.NavigateTo("/vendor-payments");
        }

        #region UI Helpers

        private string GetStatusBadgeClass(string status) => status switch
        {
            VendorPaymentStatuses.Draft => "bg-secondary-transparent",
            VendorPaymentStatuses.Submitted => "bg-info-transparent",
            VendorPaymentStatuses.Approved => "bg-primary-transparent",
            VendorPaymentStatuses.Posted => "bg-success-transparent",
            VendorPaymentStatuses.Reversed => "bg-danger-transparent",
            VendorPaymentStatuses.Cancelled => "bg-warning-transparent text-dark",
            VendorPaymentStatuses.Rejected => "bg-danger-transparent",
            _ => "bg-secondary-transparent"
        };

        private string GetPaymentMethodIcon(string method)
        {
            return method switch
            {
                VendorPaymentMethods.BankTransfer => "ti ti-building-bank",
                VendorPaymentMethods.UPI => "ti ti-qrcode",
                VendorPaymentMethods.Cheque => "ti ti-file-check",
                VendorPaymentMethods.Cash => "ti ti-cash",
                VendorPaymentMethods.Gateway => "ti ti-credit-card",
                VendorPaymentMethods.Other => "ti ti-dots",

                _ => "ti ti-help"
            };
        }


        private string GetMethodBadgeClass(string method) => method switch
        {
            VendorPaymentMethods.BankTransfer => "bg-primary-transparent",
            VendorPaymentMethods.UPI => "bg-info-transparent",
            VendorPaymentMethods.Cheque => "bg-warning-transparent text-dark",
            VendorPaymentMethods.Cash => "bg-success-transparent",
            VendorPaymentMethods.Gateway => "bg-purple-transparent",
            _ => "bg-secondary-transparent"
        };

        private string GetMethodIcon(string method) => method switch
        {
            VendorPaymentMethods.BankTransfer => "ti ti-building-bank",
            VendorPaymentMethods.UPI => "ti ti-device-mobile",
            VendorPaymentMethods.Cheque => "ti ti-file-certificate",
            VendorPaymentMethods.Cash => "ti ti-cash",
            VendorPaymentMethods.Gateway => "ti ti-credit-card",
            _ => "ti ti-wallet"
        };

        private string GetReferenceLabel()
        {
            return Payment?.PaymentMethod switch
            {
                VendorPaymentMethods.BankTransfer => "UTR / Reference No",
                VendorPaymentMethods.UPI => "UPI Transaction ID",
                VendorPaymentMethods.Cheque => "Cheque Number",
                VendorPaymentMethods.Gateway => "Transaction Reference",
                _ => "Reference Number"
            };
        }

        #endregion
    }
}
