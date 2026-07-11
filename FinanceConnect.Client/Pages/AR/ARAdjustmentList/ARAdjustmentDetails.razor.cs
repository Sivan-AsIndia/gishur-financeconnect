using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.AR.ARAdjustmentList
{
    public partial class ARAdjustmentDetails : ComponentBase
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] ARAdjustmentService AdjustmentService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] AuthService AuthService { get; set; } = default!;
        [Inject] IJSRuntime JSRuntime { get; set; } = default!;

        [Parameter] public Guid Id { get; set; }

        private bool isInitialized = false;
        private ARAdjustmentViewModel? Adjustment;

        // Modal inputs
        private string approvalCommentInput = string.Empty;
        private string rejectionReasonInput = string.Empty;
        private string cancelReasonInput = string.Empty;
        private string reverseReasonInput = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await LoadAdjustment();
            isInitialized = true;
        }

        private async Task LoadAdjustment()
        {
            Adjustment = AdjustmentService.GetById(Id);
            await Task.CompletedTask;
        }

        private void GoBack()
        {
            Nav.NavigateTo("/ar-adjustments");
        }

        private async Task SubmitAdjustment()
        {
            if (Adjustment == null) return;

            var result = AdjustmentService.Submit(Adjustment.Id, AuthService.CurrentUser?.UserName ?? "Current User");
            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                await LoadAdjustment();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        private async Task ApproveAdjustment()
        {
            if (Adjustment == null) return;

            var result = AdjustmentService.Approve(
                Adjustment.Id,
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                AuthService.CurrentUser?.UserName ?? "Current User",
                approvalCommentInput);

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                approvalCommentInput = string.Empty;
                await LoadAdjustment();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        private async Task RejectAdjustment()
        {
            if (Adjustment == null) return;

            if (string.IsNullOrWhiteSpace(rejectionReasonInput))
            {
                ToastService.ShowWarning("Rejection reason is required.");
                return;
            }

            var result = AdjustmentService.Reject(
                Adjustment.Id,
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                AuthService.CurrentUser?.UserName ?? "Current User",
                rejectionReasonInput);

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                rejectionReasonInput = string.Empty;
                await LoadAdjustment();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        private async Task PostAdjustment()
        {
            if (Adjustment == null) return;

            var result = AdjustmentService.Post(
                Adjustment.Id,
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                AuthService.CurrentUser?.UserName ?? "Current User");

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                await LoadAdjustment();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        private async Task CancelAdjustment()
        {
            if (Adjustment == null) return;

            if (string.IsNullOrWhiteSpace(cancelReasonInput))
            {
                ToastService.ShowWarning("Cancellation reason is required.");
                return;
            }

            var result = AdjustmentService.Cancel(
                Adjustment.Id,
                cancelReasonInput,
                AuthService.CurrentUser?.UserName ?? "Current User");

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                cancelReasonInput = string.Empty;
                await LoadAdjustment();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        private async Task ReverseAdjustment()
        {
            if (Adjustment == null) return;

            if (string.IsNullOrWhiteSpace(reverseReasonInput))
            {
                ToastService.ShowWarning("Reversal reason is required.");
                return;
            }

            var result = AdjustmentService.Reverse(
                Adjustment.Id,
                reverseReasonInput,
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                AuthService.CurrentUser?.UserName ?? "Current User");

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                reverseReasonInput = string.Empty;
                await LoadAdjustment();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        // Badge helpers
        private string GetStatusBadgeClass(string status) => status switch
        {
            AdjustmentStatuses.Draft => "bg-secondary",
            AdjustmentStatuses.Submitted => "bg-info",
            AdjustmentStatuses.Approved => "bg-primary",
            AdjustmentStatuses.Posted => "bg-success",
            AdjustmentStatuses.Cancelled => "bg-warning text-dark",
            AdjustmentStatuses.Reversed => "bg-danger",
            _ => "bg-secondary"
        };

        private string GetTypeBadgeClass(string type) => type switch
        {
            AdjustmentTypes.WriteOff => "bg-danger",
            AdjustmentTypes.Rounding => "bg-info",
            AdjustmentTypes.DisputeSettlement => "bg-warning text-dark",
            AdjustmentTypes.ShortPaymentSettlement => "bg-secondary",
            AdjustmentTypes.Reclassification => "bg-primary",
            AdjustmentTypes.BadDebtProvision => "bg-dark",
            AdjustmentTypes.Other => "bg-secondary",
            _ => "bg-secondary"
        };

        private string GetDirectionBadgeClass(string direction) => direction switch
        {
            AdjustmentDirections.ReduceAR => "bg-success",
            AdjustmentDirections.IncreaseAR => "bg-warning text-dark",
            _ => "bg-secondary"
        };

        private string GetApprovalBadgeClass(string status) => status switch
        {
            ARAdjustmentApprovalStatuses.NotRequired => "bg-secondary",
            ARAdjustmentApprovalStatuses.Pending => "bg-warning text-dark",
            ARAdjustmentApprovalStatuses.Approved => "bg-success",
            ARAdjustmentApprovalStatuses.Rejected => "bg-danger",
            _ => "bg-secondary"
        };

        private string GetLineTypeBadgeClass(string lineType) => lineType switch
        {
            AdjustmentLineTypes.WriteOff => "bg-danger",
            AdjustmentLineTypes.Rounding => "bg-info",
            AdjustmentLineTypes.DiscountAllowed => "bg-success",
            AdjustmentLineTypes.Dispute => "bg-warning text-dark",
            AdjustmentLineTypes.Reclassification => "bg-primary",
            AdjustmentLineTypes.Other => "bg-secondary",
            _ => "bg-secondary"
        };

        // Tag color helpers for header
        private string GetTypeTagBorder(string type) => type switch
        {
            AdjustmentTypes.WriteOff => "#dc3545",
            AdjustmentTypes.Rounding => "#0dcaf0",
            AdjustmentTypes.DisputeSettlement => "#ffc107",
            AdjustmentTypes.BadDebtProvision => "#343a40",
            AdjustmentTypes.Reclassification => "#0d6efd",
            _ => "#6c757d"
        };

        private string GetTypeTagBg(string type) => type switch
        {
            AdjustmentTypes.WriteOff => "#fdecea",
            AdjustmentTypes.Rounding => "#e8fffe",
            AdjustmentTypes.DisputeSettlement => "#fff8e1",
            AdjustmentTypes.BadDebtProvision => "#e9ecef",
            AdjustmentTypes.Reclassification => "#e8f0fe",
            _ => "#f5f5f5"
        };

        private string GetTypeTagFg(string type) => type switch
        {
            AdjustmentTypes.WriteOff => "#dc3545",
            AdjustmentTypes.Rounding => "#087990",
            AdjustmentTypes.DisputeSettlement => "#856404",
            AdjustmentTypes.BadDebtProvision => "#343a40",
            AdjustmentTypes.Reclassification => "#0d6efd",
            _ => "#555"
        };
    }
}
