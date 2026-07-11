using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;

namespace FinanceConnect.Client.Pages.AP.VendorCreditNote
{
    public partial class VendorCreditNoteDetails
    {
        [Parameter] public Guid Id { get; set; }

        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] VendorCreditNoteService CreditNoteService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;

        private bool isInitialized = false;
        private VendorCreditNoteViewModel CreditNote = new();

        private string rejectReason = string.Empty;
        private string cancelReason = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await LoadCreditNote();
            isInitialized = true;
        }

        private async Task LoadCreditNote()
        {
            await Task.Delay(50); // Simulate async

            var existing = CreditNoteService.GetById(Id);
            if (existing != null)
            {
                CreditNote = existing;
            }
            else
            {
                ToastService.ShowError("Credit Note not found.");
                Nav.NavigateTo("/vendor-credit-notes");
            }
        }

        private async Task Approve()
        {
            var result = CreditNoteService.Approve(Id, "System");
            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                await LoadCreditNote();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        private async Task Reject()
        {
            if (string.IsNullOrWhiteSpace(rejectReason))
            {
                ToastService.ShowWarning("Rejection reason is required.");
                return;
            }

            var result = CreditNoteService.Reject(Id, rejectReason, "System");
            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                rejectReason = string.Empty;
                await LoadCreditNote();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        private async Task Post()
        {
            var defaultPeriodId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var result = CreditNoteService.Post(Id, defaultPeriodId, "System");
            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                await LoadCreditNote();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        private async Task CancelCreditNote()
        {
            if (string.IsNullOrWhiteSpace(cancelReason))
            {
                ToastService.ShowWarning("Cancellation reason is required.");
                return;
            }

            var result = CreditNoteService.Cancel(Id, cancelReason, "System");
            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                cancelReason = string.Empty;
                await LoadCreditNote();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        #region Helper Methods

        private static string GetStatusBadgeClass(string status) => status switch
        {
            VendorCreditNoteStatuses.Draft => "bg-secondary-transparent text-secondary",
            VendorCreditNoteStatuses.Submitted => "bg-info-transparent text-info",
            VendorCreditNoteStatuses.Approved => "bg-primary-transparent text-primary",
            VendorCreditNoteStatuses.Rejected => "bg-danger-transparent text-danger",
            VendorCreditNoteStatuses.Posted => "bg-success-transparent text-success",
            VendorCreditNoteStatuses.Cancelled => "bg-warning-transparent text-warning",
            VendorCreditNoteStatuses.Reversed => "bg-dark-transparent text-dark",
            _ => "bg-secondary-transparent text-secondary"
        };

        private static string GetTypeBadgeClass(string type) => type switch
        {
            VendorCreditNoteTypes.PurchaseReturn => "bg-warning-transparent text-warning",
            VendorCreditNoteTypes.PriceReduction => "bg-info-transparent text-info",
            VendorCreditNoteTypes.DiscountRebate => "bg-primary-transparent text-primary",
            VendorCreditNoteTypes.BillingCorrection => "bg-danger-transparent text-danger",
            VendorCreditNoteTypes.DamageClaim => "bg-secondary-transparent text-secondary",
            VendorCreditNoteTypes.Other => "bg-light text-dark",
            _ => "bg-secondary-transparent text-secondary"
        };

        private static string GetSettlementBadgeClass(string status) => status switch
        {
            CreditSettlementStatuses.Unapplied => "bg-warning-transparent text-warning",
            CreditSettlementStatuses.PartiallyApplied => "bg-info-transparent text-info",
            CreditSettlementStatuses.FullyApplied => "bg-success-transparent text-success",
            _ => "bg-secondary-transparent text-secondary"
        };

        #endregion
    }
}
