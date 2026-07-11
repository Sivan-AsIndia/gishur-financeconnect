using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.AP.VendorDebitNote
{
    public partial class VendorDebitNoteDetails : ComponentBase
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] VendorDebitNoteService DebitNoteService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] IJSRuntime JSRuntime { get; set; } = default!;

        [Parameter] public Guid Id { get; set; }

        private bool isInitialized = false;
        private VendorDebitNoteViewModel? DebitNote;

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
            Nav.NavigateTo("/vendor-debit-notes");
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

        // Helper methods
        private string GetStatusBadgeClass(string status) => status switch
        {
            VendorDebitNoteStatuses.Draft => "bg-secondary-transparent",
            VendorDebitNoteStatuses.Submitted => "bg-info-transparent",
            VendorDebitNoteStatuses.Approved => "bg-primary-transparent",
            VendorDebitNoteStatuses.Posted => "bg-success-transparent",
            VendorDebitNoteStatuses.Cancelled => "bg-warning-transparent text-dark",
            VendorDebitNoteStatuses.Reversed => "bg-danger-transparent",
            _ => "bg-secondary-transparent"
        };

        private string GetTypeBadgeClass(string type) => type switch
        {
            VendorDebitNoteTypes.PriceIncrease => "bg-primary-transparent",
            VendorDebitNoteTypes.FreightCharges => "bg-info-transparent",
            VendorDebitNoteTypes.PenaltyCharges => "bg-danger-transparent",
            VendorDebitNoteTypes.ServiceAddOn => "bg-warning-transparent text-warning",
            VendorDebitNoteTypes.TaxDifference => "bg-purple-transparent text-purple",
            VendorDebitNoteTypes.BillingCorrection => "bg-success-transparent",
            VendorDebitNoteTypes.Other => "bg-secondary-transparent",
            _ => "bg-secondary-transparent"
        };

        private string GetLineTypeBadgeClass(string lineType) => lineType switch
        {
            VendorDebitNoteLineTypes.Expense => "bg-primary-transparent",
            VendorDebitNoteLineTypes.Asset => "bg-info-transparent",
            VendorDebitNoteLineTypes.Service => "bg-warning-transparent text-dark",
            VendorDebitNoteLineTypes.Charge => "bg-danger-transparent",
            _ => "bg-secondary-transparent"
        };

        private string GetSettlementBadgeClass(string status) => status switch
        {
            VendorDebitNoteSettlementStatuses.Unapplied => "bg-warning-transparent text-dark",
            VendorDebitNoteSettlementStatuses.PartiallyApplied => "bg-info-transparent",
            VendorDebitNoteSettlementStatuses.FullyApplied => "bg-success-transparent",
            _ => "bg-secondary-transparent"
        };
    }
}
