using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.AR.CustomerDebitNote
{
    public partial class CustomerDebitNoteDetails : ComponentBase
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] CustomerDebitNoteService DebitNoteService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] IJSRuntime JSRuntime { get; set; } = default!;

        [Parameter] public Guid Id { get; set; }

        private bool isInitialized = false;
        private CustomerDebitNoteViewModel? DebitNote;

        // Modal inputs
        private string cancelReasonInput = string.Empty;
        private string reverseReasonInput = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await LoadDebitNote();
            isInitialized = true;
        }

        private async Task LoadDebitNote()
        {
            DebitNote = DebitNoteService.GetById(Id);
            await Task.CompletedTask;
        }

        private void GoBack()
        {
            Nav.NavigateTo("/customer-debit-notes");
        }

        private async Task SubmitDebitNote()
        {
            if (DebitNote == null) return;

            var result = DebitNoteService.Submit(DebitNote.Id, "Current User");
            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                await LoadDebitNote();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        private async Task PostDebitNote()
        {
            if (DebitNote == null) return;

            var result = DebitNoteService.Post(
                DebitNote.Id,
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                "Current User");

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                await LoadDebitNote();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        private async Task CancelDebitNote()
        {
            if (DebitNote == null) return;

            if (string.IsNullOrWhiteSpace(cancelReasonInput))
            {
                ToastService.ShowWarning("Cancellation reason is required.");
                return;
            }

            var result = DebitNoteService.Cancel(
                DebitNote.Id,
                cancelReasonInput,
                "Current User");

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                cancelReasonInput = string.Empty;
                await LoadDebitNote();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        private async Task ReverseDebitNote()
        {
            if (DebitNote == null) return;

            if (string.IsNullOrWhiteSpace(reverseReasonInput))
            {
                ToastService.ShowWarning("Reversal reason is required.");
                return;
            }

            var result = DebitNoteService.Reverse(
                DebitNote.Id,
                reverseReasonInput,
                "Current User");

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                reverseReasonInput = string.Empty;
                await LoadDebitNote();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        // Helper methods - badge classes matching VendorDebitNote UI pattern
        private string GetStatusBadgeClass(string status) => status switch
        {
            DebitNoteStatuses.Draft => "badge-draft",
            DebitNoteStatuses.Submitted => "badge-submitted",
            DebitNoteStatuses.Approved => "bg-primary-transparent",
            DebitNoteStatuses.Posted => "badge-posted",
            DebitNoteStatuses.Cancelled => "badge-cancelled",
            DebitNoteStatuses.Reversed => "badge-reversed",
            _ => "badge-draft"
        };

        private string GetReasonBadgeClass(string reasonCode) => reasonCode switch
        {
            DebitReasonCodes.UnderbillingCorrection => "bg-primary-transparent",
            DebitReasonCodes.AdditionalCharges => "bg-info-transparent",
            DebitReasonCodes.LateFee => "bg-danger-transparent",
            DebitReasonCodes.FreightDelivery => "badge-goods",
            DebitReasonCodes.TaxShortCharged => "bg-purple-transparent text-purple",
            DebitReasonCodes.RateRevision => "bg-success-transparent",
            DebitReasonCodes.Other => "badge-draft",
            _ => "badge-draft"
        };

        private string GetLineTypeBadgeClass(string lineType) => lineType switch
        {
            "Item" => "bg-primary-transparent",
            "Service" => "bg-info-transparent",
            "Charge" => "badge-goods",
            "Fee" => "bg-danger-transparent",
            _ => "badge-draft"
        };
    }
}
