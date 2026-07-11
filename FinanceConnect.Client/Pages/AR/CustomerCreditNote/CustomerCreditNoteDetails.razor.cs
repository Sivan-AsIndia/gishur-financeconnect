using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.AR.CustomerCreditNote
{
    public partial class CustomerCreditNoteDetails : ComponentBase
    {
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        [Inject] private CustomerCreditNoteService CreditNoteService { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;
        [Inject] private AuthService AuthService { get; set; } = default!;

        [Parameter] public Guid Id { get; set; }

        private bool isInitialized = false;
        private CustomerCreditNoteViewModel? CreditNote { get; set; }
        private string cancelReasonInput = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            CreditNote = CreditNoteService.GetById(Id);
            isInitialized = true;
        }

        private async Task ApproveCreditNote()
        {
            if (CreditNote == null) return;

            var result = CreditNoteService.Approve(
                CreditNote.Id,
                AuthService.CurrentUser?.UserName ?? "System");

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                CreditNote = CreditNoteService.GetById(Id);
                StateHasChanged();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        private async Task PostCreditNote()
        {
            if (CreditNote == null) return;

            var result = CreditNoteService.Post(
                CreditNote.Id,
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                AuthService.CurrentUser?.UserName ?? "System");

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                CreditNote = CreditNoteService.GetById(Id);
                StateHasChanged();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        private async Task CancelCreditNote()
        {
            if (CreditNote == null) return;

            if (string.IsNullOrWhiteSpace(cancelReasonInput))
            {
                ToastService.ShowWarning("Cancellation reason is required.");
                return;
            }

            var result = CreditNoteService.Cancel(
                CreditNote.Id,
                cancelReasonInput,
                AuthService.CurrentUser?.UserName ?? "System");

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                cancelReasonInput = string.Empty;
                CreditNote = CreditNoteService.GetById(Id);
                StateHasChanged();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        private string GetReasonBadgeClass(string reason) => reason switch
        {
            CreditReasonCodes.SalesReturn => "bg-warning-transparent text-warning",
            CreditReasonCodes.PriceCorrection => "bg-info-transparent text-info",
            CreditReasonCodes.DiscountAfterInvoice => "bg-primary-transparent text-primary",
            CreditReasonCodes.ServiceCancellation => "bg-danger-transparent text-danger",
            CreditReasonCodes.DamageDefect => "bg-secondary-transparent text-secondary",
            CreditReasonCodes.TaxCorrection => "bg-success-transparent text-success",
            CreditReasonCodes.WriteOffSettlement => "bg-dark-transparent text-dark",
            CreditReasonCodes.Other => "bg-light text-dark",
            _ => "bg-secondary-transparent text-secondary"
        };

        private string GetStatusBadgeClass(string status) => status switch
        {
            CreditNoteStatuses.Draft => "bg-secondary-transparent text-secondary",
            CreditNoteStatuses.Submitted => "bg-info-transparent text-info",
            CreditNoteStatuses.Approved => "bg-primary-transparent text-primary",
            CreditNoteStatuses.Posted => "bg-success-transparent text-success",
            CreditNoteStatuses.Cancelled => "bg-danger-transparent text-danger",
            CreditNoteStatuses.Reversed => "bg-dark-transparent text-dark",
            _ => "bg-secondary-transparent text-secondary"
        };
    }
}
