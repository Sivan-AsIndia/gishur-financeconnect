using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public class ExpenseClaimViewModel
    {
        // ── Enums ──────────────────────────────────────────────────────────────

        public enum ClaimStatusEnum
        {
            Draft = 1,
            Submitted = 2,
            UnderReview = 3,
            Approved = 4,
            PartiallyApproved = 5,
            Rejected = 6,
            Reimbursed = 7,
            PartiallyReimbursed = 8,
            Cancelled = 9,
            Closed = 10
        }

        public enum ReimbursementStatusEnum
        {
            NotStarted = 1,
            Pending = 2,
            PartiallyReimbursed = 3,
            FullyReimbursed = 4,
            OnHold = 5
        }

        public enum ReceiptComplianceStatusEnum
        {
            NotChecked = 1,
            Complete = 2,
            Missing = 3,
            Partial = 4,
            ExceptionApproved = 5
        }

        public enum PolicyCheckStatusEnum
        {
            NotChecked = 1,
            Ok = 2,
            Warning = 3,
            Blocked = 4,
            Overridden = 5
        }

        public enum DuplicateCheckStatusEnum
        {
            NotChecked = 1,
            NoDuplicate = 2,
            SuspectedDuplicate = 3,
            ConfirmedDuplicate = 4,
            Overridden = 5
        }

        public enum LineApprovalStatusEnum
        {
            Pending = 1,
            Approved = 2,
            PartiallyApproved = 3,
            Rejected = 4
        }

        public enum ReimbursementMethodEnum
        {
            BankTransfer = 1,
            Payroll = 2,
            Cash = 3,
            PettyCash = 4,
            Other = 5
        }

        public enum PostingStatusEnum
        {
            NotLinked = 1,
            ExpenseCreated = 2,
            ReimbursementProcessed = 3,
            Closed = 4
        }

        // ── ExpenseClaim Line Model ────────────────────────────────────────────

        public class ExpenseClaimLine
        {
            public Guid ExpenseClaimLineId { get; set; } = Guid.NewGuid();
            public Guid ExpenseClaimId { get; set; }
            public int LineNumber { get; set; }

            [Required(ErrorMessage = "Expense Date is required")]
            public DateTime? ExpenseDate { get; set; }

            [Required(ErrorMessage = "Line Description is required")]
            [MaxLength(500)]
            public string LineDescription { get; set; } = string.Empty;

            [Required(ErrorMessage = "Expense Category is required")]
            public Guid ExpenseCategoryId { get; set; }
            public string? ExpenseCategoryName { get; set; }

            [Required(ErrorMessage = "Claimed Amount is required")]
            [Range(0.01, double.MaxValue, ErrorMessage = "Claimed Amount must be > 0")]
            public decimal ClaimedAmount { get; set; }

            public decimal ApprovedAmount { get; set; }
            public decimal RejectedAmount { get; set; }
            public decimal? TaxAmount { get; set; }

            [Required(ErrorMessage = "Gross Amount is required")]
            public decimal GrossAmount { get; set; }

            [MaxLength(200)]
            public string? MerchantOrVendorName { get; set; }

            [MaxLength(200)]
            public string? ReceiptReferenceText { get; set; }

            public bool ReceiptAttachedFlag { get; set; }

            public PolicyCheckStatusEnum PolicyCheckStatus { get; set; } = PolicyCheckStatusEnum.NotChecked;
            public LineApprovalStatusEnum ApprovalStatus { get; set; } = LineApprovalStatusEnum.Pending;

            [MaxLength(500)]
            public string? RejectionReason { get; set; }

            public Guid? LinkedExpenseId { get; set; }
            public decimal ReimbursedAmount { get; set; }

            [MaxLength(200)]
            public string? ReimbursementReference { get; set; }

            [MaxLength(500)]
            public string? LineNotes { get; set; }
        }

        // ── ExpenseClaim Model ─────────────────────────────────────────────────

        public class ExpenseClaim
        {
            // ─── Section 1: Core Identity (Header) ───────────────────────────
            public Guid ExpenseClaimId { get; set; } = Guid.NewGuid();
            public Guid TenantId { get; set; }
            public Guid CompanyId { get; set; }

            [Required(ErrorMessage = "Claim Code is required")]
            [MaxLength(30, ErrorMessage = "Claim Code cannot exceed 30 characters")]
            public string ClaimCode { get; set; } = string.Empty;

            [Required(ErrorMessage = "Claim Title is required")]
            [MaxLength(200, ErrorMessage = "Claim Title cannot exceed 200 characters")]
            public string ClaimTitle { get; set; } = string.Empty;

            [MaxLength(1000)]
            public string? Description { get; set; }

            [Required(ErrorMessage = "Claim Status is required")]
            public ClaimStatusEnum ClaimStatus { get; set; } = ClaimStatusEnum.Draft;

            // ─── Section 2: Claimant & Business Context ─────────────────────
            [Required(ErrorMessage = "Claimant is required")]
            [MaxLength(100)]
            public string ClaimantEmployeeId { get; set; } = string.Empty;

            [MaxLength(30)]
            public string? ClaimantCodeSnapshot { get; set; }
            [MaxLength(200)]
            public string? ClaimantNameSnapshot { get; set; }

            public DateTime? ClaimSubmissionDate { get; set; }
            public DateTime? ClaimPeriodFrom { get; set; }
            public DateTime? ClaimPeriodTo { get; set; }

            [Required(ErrorMessage = "Business Purpose is required")]
            [MaxLength(1000)]
            public string BusinessPurpose { get; set; } = string.Empty;

            public Guid? DepartmentId { get; set; }
            public string? DepartmentName { get; set; }
            public Guid? BranchId { get; set; }
            public string? BranchName { get; set; }
            public Guid? CostCenterId { get; set; }
            public string? CostCenterName { get; set; }
            public Guid? ProjectId { get; set; }
            public string? ProjectName { get; set; }

            [MaxLength(100)]
            public string? ManagerApproverId { get; set; }

            // ─── Section 3: Claim Amount Summary ────────────────────────────
            [Required(ErrorMessage = "Currency is required")]
            public Guid CurrencyId { get; set; }

            public Guid? ExchangeRateId { get; set; }
            public decimal TotalClaimedAmount { get; set; }
            public decimal TotalApprovedAmount { get; set; }
            public decimal TotalRejectedAmount { get; set; }
            public decimal TotalReimbursedAmount { get; set; }

            public decimal OutstandingReimbursementAmount =>
                TotalApprovedAmount - TotalReimbursedAmount;

            [Required(ErrorMessage = "Reimbursement Status is required")]
            public ReimbursementStatusEnum ReimbursementStatus { get; set; } = ReimbursementStatusEnum.NotStarted;

            // ─── Section 4: Policy & Receipt Compliance ─────────────────────
            [Required(ErrorMessage = "Receipt Compliance is required")]
            public ReceiptComplianceStatusEnum ReceiptComplianceStatus { get; set; } = ReceiptComplianceStatusEnum.NotChecked;

            [Required(ErrorMessage = "Policy Check Status is required")]
            public PolicyCheckStatusEnum PolicyCheckStatus { get; set; } = PolicyCheckStatusEnum.NotChecked;

            [Required(ErrorMessage = "Duplicate Check Status is required")]
            public DuplicateCheckStatusEnum DuplicateCheckStatus { get; set; } = DuplicateCheckStatusEnum.NotChecked;

            [MaxLength(1000)]
            public string? PolicyOverrideReason { get; set; }

            public bool ReceiptRequiredFlag { get; set; }
            public int AttachmentCount { get; set; }

            // ─── Section 5: Claim Lines (Child) ─────────────────────────────
            public List<ExpenseClaimLine> Lines { get; set; } = new();

            // ─── Section 6: Approval & Review ───────────────────────────────
            [MaxLength(100)]
            public string? SubmittedByUserId { get; set; }
            public DateTime? SubmittedOn { get; set; }
            [MaxLength(100)]
            public string? ReviewedByUserId { get; set; }
            public DateTime? ReviewedOn { get; set; }
            [MaxLength(100)]
            public string? ApprovedByUserId { get; set; }
            public DateTime? ApprovedOn { get; set; }
            [MaxLength(100)]
            public string? RejectedByUserId { get; set; }
            public DateTime? RejectedOn { get; set; }

            [MaxLength(1500)]
            public string? ApprovalNotes { get; set; }

            public bool IsLocked { get; set; }
            public DateTime? LockedOn { get; set; }
            public Guid? LockedBy { get; set; }

            [MaxLength(500)]
            public string? CancellationReason { get; set; }

            // ─── Section 7: Reimbursement & Downstream Accounting ───────────
            public Guid? ExpenseId { get; set; }
            public ReimbursementMethodEnum? ReimbursementMethod { get; set; }
            public DateTime? ReimbursementDate { get; set; }

            [MaxLength(100)]
            public string? ReimbursementReferenceNumber { get; set; }

            public Guid? PaymentTransactionId { get; set; }

            [Required(ErrorMessage = "Posting Status is required")]
            public PostingStatusEnum PostingStatus { get; set; } = PostingStatusEnum.NotLinked;

            // ─── Section 8: Notes & Attachments ─────────────────────────────
            [MaxLength(1500)]
            public string? Notes { get; set; }
            [MaxLength(1500)]
            public string? SupportingCommentary { get; set; }

            public bool PolicyExceptionFlag { get; set; }
            [MaxLength(100)]
            public string? PolicyExceptionApprovedBy { get; set; }

            // ─── Section 9: System Audit Fields (Hidden) ────────────────────
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public Guid? CreatedBy { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public Guid? UpdatedBy { get; set; }
            public bool IsDeleted { get; set; }
        }
    }
}
