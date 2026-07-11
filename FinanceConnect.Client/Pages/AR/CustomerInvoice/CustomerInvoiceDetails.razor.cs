using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.AR.CustomerInvoice
{
    public partial class CustomerInvoiceDetails : ComponentBase
    {
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        [Inject] private CustomerInvoiceService InvoiceService { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;
        [Inject] private AuthService AuthService { get; set; } = default!;

        [Parameter] public Guid Id { get; set; }

        private bool isInitialized = false;
        private CustomerInvoiceViewModel? Invoice { get; set; }
        private string cancelReasonInput = string.Empty;

        private bool IsOverdue => Invoice != null &&
                                  Invoice.DueDate < DateTime.Today &&
                                  Invoice.AmountOutstanding > 0 &&
                                  (Invoice.InvoiceStatus == InvoiceStatuses.Posted ||
                                   Invoice.InvoiceStatus == InvoiceStatuses.PartiallyPaid);

        protected override async Task OnInitializedAsync()
        {
            Invoice = InvoiceService.GetById(Id);
            isInitialized = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("feather.replace");
            }
        }

        private async Task PrintPage()
        {
            await JS.InvokeVoidAsync("window.print");
        }

        private async Task OpenPostModal()
        {
            await JS.InvokeVoidAsync("eval", "$('#postModal').modal('show')");
        }

        private async Task OpenCancelModal()
        {
            cancelReasonInput = string.Empty;
            await JS.InvokeVoidAsync("eval", "$('#cancelModal').modal('show')");
        }

        private async Task PostInvoice()
        {
            if (Invoice == null) return;

            var result = InvoiceService.Post(
                Invoice.Id,
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                AuthService.CurrentUser?.UserName ?? "System");

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                Invoice = InvoiceService.GetById(Id);
                StateHasChanged();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }

            await JS.InvokeVoidAsync("eval", "$('#postModal').modal('hide')");
        }

        private async Task CancelInvoice()
        {
            if (Invoice == null) return;

            if (string.IsNullOrWhiteSpace(cancelReasonInput))
            {
                ToastService.ShowWarning("Cancellation reason is required.");
                return;
            }

            var result = InvoiceService.Cancel(
                Invoice.Id,
                cancelReasonInput,
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                AuthService.CurrentUser?.UserName ?? "System");

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                Invoice = InvoiceService.GetById(Id);
                StateHasChanged();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }

            await JS.InvokeVoidAsync("eval", "$('#cancelModal').modal('hide')");
        }

        private string GetTypeBadgeClass(string type) => type switch
        {
            InvoiceTypes.Standard => "bg-primary-transparent",
            InvoiceTypes.Proforma => "bg-info-transparent",
            InvoiceTypes.Export => "bg-success-transparent",
            InvoiceTypes.SEZ => "bg-warning-transparent",
            InvoiceTypes.AdjustmentInvoice => "bg-secondary-transparent",
            _ => "bg-secondary-transparent"
        };

        private string GetStatusBadgeClass(string status) => status switch
        {
            InvoiceStatuses.Draft => "bg-secondary-transparent",
            InvoiceStatuses.Submitted => "bg-info-transparent",
            InvoiceStatuses.Approved => "bg-primary-transparent",
            InvoiceStatuses.Posted => "bg-success-transparent",
            InvoiceStatuses.PartiallyPaid => "bg-warning-transparent",
            InvoiceStatuses.Paid => "bg-success-transparent",
            InvoiceStatuses.Cancelled => "bg-danger-transparent",
            InvoiceStatuses.Voided => "bg-dark-transparent",
            _ => "bg-secondary-transparent"
        };

        private string GetSettlementBadgeClass()
        {
            if (Invoice == null) return "bg-secondary-transparent";
            if (Invoice.AmountOutstanding <= 0) return "bg-success-transparent";
            if (Invoice.AmountPaidToDate > 0) return "bg-warning-transparent";
            return "bg-danger-transparent";
        }

        private string GetSettlementStatusText()
        {
            if (Invoice == null) return "Unknown";
            if (Invoice.AmountOutstanding <= 0) return "Paid";
            if (Invoice.AmountPaidToDate > 0) return "Partially Paid";
            return "Unpaid";
        }

        private string GetLineTypeBadgeClass(string lineType) => lineType switch
        {
            "Goods" => "bg-primary-transparent text-primary",
            "Service" => "bg-info-transparent text-info",
            "Expense" => "bg-warning-transparent text-warning",
            "Asset" => "bg-success-transparent text-success",
            "Other" => "bg-secondary-transparent text-secondary",
            _ => "bg-light text-dark"
        };

        private string GetApprovalBadgeClass(string? status) => status switch
        {
            ApprovalStatuses.NotRequired => "bg-secondary-transparent",
            ApprovalStatuses.Pending => "bg-warning-transparent",
            ApprovalStatuses.Approved => "bg-success-transparent",
            ApprovalStatuses.Rejected => "bg-danger-transparent",
            _ => "bg-secondary-transparent"
        };

        private string GetEInvoiceStatusBadgeClass(string? status) => status switch
        {
            EInvoiceStatuses.NotApplicable => "bg-secondary-transparent",
            EInvoiceStatuses.Pending => "bg-warning-transparent",
            EInvoiceStatuses.Generated => "bg-success-transparent",
            EInvoiceStatuses.Failed => "bg-danger-transparent",
            _ => "bg-secondary-transparent"
        };

        private string GetApprovalStatusDisplayName(string? status) => status switch
        {
            ApprovalStatuses.NotRequired => "Not Required",
            ApprovalStatuses.Pending => "Pending",
            ApprovalStatuses.Approved => "Approved",
            ApprovalStatuses.Rejected => "Rejected",
            _ => status ?? "-"
        };

        private void GoBack()
        {
            Nav.NavigateTo("/customer-invoices");
        }
    }
}
