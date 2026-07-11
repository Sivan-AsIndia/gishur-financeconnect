using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.AP.APAdjustment
{
    public partial class APAdjustmentDetails : ComponentBase
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] APAdjustmentService AdjustmentService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] AuthService AuthService { get; set; } = default!;
        [Inject] IJSRuntime JSRuntime { get; set; } = default!;

        [Parameter] public Guid Id { get; set; }

        private bool isInitialized = false;
        private APAdjustmentViewModel? Adjustment;

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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("feather.replace");
            }
        }

        private async Task LoadAdjustment()
        {
            Adjustment = AdjustmentService.GetById(Id);
            await Task.CompletedTask;
        }

        private void GoBack()
        {
            Nav.NavigateTo("/ap-adjustments");
        }

        private async Task SubmitAdjustment()
        {
            if (Adjustment == null) return;

            var result = AdjustmentService.Submit(Adjustment.APAdjustmentId, AuthService.CurrentUser?.UserName ?? "Current User");
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
                Adjustment.APAdjustmentId,
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
                Adjustment.APAdjustmentId,
                rejectionReasonInput,
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                AuthService.CurrentUser?.UserName ?? "Current User");

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
                Adjustment.APAdjustmentId,
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
                Adjustment.APAdjustmentId,
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
                Adjustment.APAdjustmentId,
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

        // Helper methods
        private string GetStatusBadgeClass(string status) => status switch
        {
            APAdjustmentStatuses.Draft => "bg-secondary-transparent",
            APAdjustmentStatuses.Submitted => "bg-info-transparent",
            APAdjustmentStatuses.Approved => "bg-primary-transparent",
            APAdjustmentStatuses.Posted => "bg-success-transparent",
            APAdjustmentStatuses.Cancelled => "bg-warning-transparent text-dark",
            APAdjustmentStatuses.Reversed => "bg-danger-transparent",
            APAdjustmentStatuses.Rejected => "bg-danger-transparent",
            _ => "bg-secondary-transparent"
        };

        private string GetTypeBadgeClass(string type) => type switch
        {
            APAdjustmentTypes.WriteOff => "bg-danger-transparent",
            APAdjustmentTypes.RoundOffCorrection => "bg-info-transparent",
            APAdjustmentTypes.DisputeSettlement => "bg-warning-transparent text-dark",
            APAdjustmentTypes.Reclassification => "bg-primary-transparent",
            APAdjustmentTypes.VendorBalanceTransfer => "bg-secondary-transparent",
            APAdjustmentTypes.FXDifference => "bg-dark-transparent",
            APAdjustmentTypes.Other => "bg-secondary-transparent",
            _ => "bg-secondary-transparent"
        };

        private string GetDirectionBadgeClass(string direction) => direction switch
        {
            APAdjustmentDirections.ReducePayable => "bg-success-transparent",
            APAdjustmentDirections.IncreasePayable => "bg-warning-transparent text-dark",
            _ => "bg-secondary-transparent"
        };

        private string GetPolicyBadgeClass(string category) => category switch
        {
            APPolicyLimitCategories.SmallWriteOff => "bg-success-transparent",
            APPolicyLimitCategories.Medium => "bg-warning-transparent text-dark",
            APPolicyLimitCategories.HighRisk => "bg-danger-transparent",
            _ => "bg-secondary-transparent"
        };

        private string GetScopeBadgeClass(string scope) => scope switch
        {
            APAdjustmentScopes.VendorLevel => "bg-primary-transparent",
            APAdjustmentScopes.BillLevel => "bg-info-transparent",
            APAdjustmentScopes.AdvanceLevel => "bg-warning-transparent text-dark",
            _ => "bg-secondary-transparent"
        };
    }
}
