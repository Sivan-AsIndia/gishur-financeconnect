using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using static FinanceConnect.Client.ViewModels.ExpenseClaimViewModel;
using ExpenseClaimModel = FinanceConnect.Client.ViewModels.ExpenseClaimViewModel.ExpenseClaim;

namespace FinanceConnect.Client.Pages.Revenue_Expense.ExpenseClaim
{
    public partial class ExpenseClaimDetails : ComponentBase
    {
        [Parameter] public Guid Id { get; set; }
        [Inject] private ExpenseClaimService Service { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        private ExpenseClaimModel? Claim;
        private bool isInitialized;

        protected override void OnInitialized() { Claim = Service.GetById(Id); isInitialized = true; }

        private static string GetStatusBadge(ClaimStatusEnum s) => s switch { ClaimStatusEnum.Draft => "bg-warning-transparent", ClaimStatusEnum.Submitted => "bg-info-transparent", ClaimStatusEnum.UnderReview => "bg-info-transparent", ClaimStatusEnum.Approved => "bg-primary-transparent", ClaimStatusEnum.PartiallyApproved => "bg-warning-transparent", ClaimStatusEnum.Rejected => "bg-danger-transparent", ClaimStatusEnum.Reimbursed => "bg-success-transparent", ClaimStatusEnum.PartiallyReimbursed => "bg-warning-transparent", ClaimStatusEnum.Cancelled => "bg-secondary-transparent text-secondary", ClaimStatusEnum.Closed => "bg-success-transparent", _ => "bg-light" };
        private static string GetReimbBadge(ReimbursementStatusEnum s) => s switch { 
            ReimbursementStatusEnum.FullyReimbursed => "bg-success-transparent", ReimbursementStatusEnum.PartiallyReimbursed => "bg-warning-transparent", ReimbursementStatusEnum.Pending => "bg-info-transparent", ReimbursementStatusEnum.OnHold => "bg-danger-transparent", _ => "bg-secondary-transparent text-secondary"
        };
        private static string GetCompBadge(ReceiptComplianceStatusEnum s) => s switch { ReceiptComplianceStatusEnum.Complete => "bg-success-transparent", ReceiptComplianceStatusEnum.Missing => "bg-danger-transparent", ReceiptComplianceStatusEnum.Partial => "bg-warning-transparent", _ => "bg-secondary-transparent text-secondary" };
        private static string GetPolBadge(PolicyCheckStatusEnum s) => s switch { PolicyCheckStatusEnum.Ok => "bg-success-transparent", PolicyCheckStatusEnum.Warning => "bg-warning-transparent", PolicyCheckStatusEnum.Blocked => "bg-danger-transparent", PolicyCheckStatusEnum.Overridden => "bg-info-transparent", _ => "bg-secondary-transparent text-secondary" };
        private static string GetLineApprovalBadge(LineApprovalStatusEnum s) => s switch { LineApprovalStatusEnum.Approved => "bg-success-transparent", LineApprovalStatusEnum.PartiallyApproved => "bg-warning-transparent", LineApprovalStatusEnum.Rejected => "bg-danger-transparent", _ => "bg-secondary-transparent text-secondary" };
    }
}
