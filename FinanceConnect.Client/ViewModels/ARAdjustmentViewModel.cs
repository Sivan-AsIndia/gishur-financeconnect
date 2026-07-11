using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    #region Model #32: ARAdjustment

    /// <summary>
    /// Model #32: ARAdjustment – Controlled financial correction document for AR balances.
    /// Used for write-offs, rounding adjustments, dispute settlements, reclassifications.
    /// Controls workflow, approval, posting, and audit compliance.
    /// </summary>
    public class ARAdjustmentViewModel
    {
        // Section 1: Core Adjustment Identity Fields (Header)

        /// <summary>PK - hidden in UI</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Tenant identifier - hidden in UI</summary>
        public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

        /// <summary>FK → Company - hidden</summary>
        [Required(ErrorMessage = "Company is required")]
        public Guid CompanyId { get; set; }
        public string? CompanyName { get; set; }

        /// <summary>FK → Branch - Dropdown, required</summary>
        [Required(ErrorMessage = "Branch is required")]
        public Guid BranchId { get; set; }
        public string? BranchCode { get; set; }
        public string? BranchName { get; set; }

        /// <summary>FK → Customer - Search dropdown, required</summary>
        [Required(ErrorMessage = "Customer is required")]
        public Guid CustomerId { get; set; }
        public string? CustomerCode { get; set; }
        public string? CustomerName { get; set; }

        /// <summary>FK → CustomerAccount - Read-only derived from (CompanyId + CustomerId + CurrencyId)</summary>
        public Guid? CustomerAccountId { get; set; }
        public string? CustomerAccountName { get; set; }

        /// <summary>Adjustment Number - max 40 chars, unique per CompanyId + FinancialYear, read-only auto-generated</summary>
        [Required(ErrorMessage = "Adjustment Number is required")]
        [StringLength(40, ErrorMessage = "Adjustment Number cannot exceed 40 characters")]
        public string AdjustmentNumber { get; set; } = string.Empty;

        /// <summary>Adjustment Date - date picker, required</summary>
        [Required(ErrorMessage = "Adjustment Date is required")]
        public DateTime AdjustmentDate { get; set; } = DateTime.Today;

        /// <summary>Posting Date - conditional required at posting. Rule: PostingDate >= AdjustmentDate, open period only</summary>
        public DateTime? PostingDate { get; set; }

        /// <summary>FK → CurrencyMaster - read-only derived from customer account</summary>
        [Required(ErrorMessage = "Currency is required")]
        public Guid CurrencyId { get; set; }
        public string? CurrencyCode { get; set; }
        public string? CurrencyName { get; set; }

        /// <summary>Notes / Narration - textarea, max 1000 chars. Mandatory for write-off and dispute types</summary>
        [StringLength(1000, ErrorMessage = "Narration cannot exceed 1000 characters")]
        public string? AdjustmentNarration { get; set; }

        // Section 2: Adjustment Classification (Policy Control)

        /// <summary>Adjustment Type - dropdown, required. Values: WriteOff, Rounding, DisputeSettlement, ShortPaymentSettlement, Reclassification, BadDebtProvision, Other</summary>
        [Required(ErrorMessage = "Adjustment Type is required")]
        public string AdjustmentType { get; set; } = "";

        /// <summary>FK → ARAdjustmentReasonMaster - dropdown, required</summary>
        [Required(ErrorMessage = "Reason Code is required")]
        public Guid ReasonCodeId { get; set; }
        public string? ReasonCode { get; set; }
        public string? ReasonDescription { get; set; }

        /// <summary>Requires Approval - read-only, derived from type + amount threshold policy</summary>
        public bool RequiresApproval { get; set; } = false;

        /// <summary>Approval Status - badge + timeline. Values: NotRequired, Pending, Approved, Rejected</summary>
        public string ApprovalStatus { get; set; } = ARAdjustmentApprovalStatuses.NotRequired;

        /// <summary>FK → User who approved - read-only</summary>
        public Guid? ApprovedByUserId { get; set; }
        public string? ApprovedByUserName { get; set; }

        /// <summary>Approved On datetime - read-only</summary>
        public DateTime? ApprovedOn { get; set; }

        /// <summary>Approval Comment - optional textarea, max 250 chars</summary>
        [StringLength(250, ErrorMessage = "Approval Comment cannot exceed 250 characters")]
        public string? ApprovalComment { get; set; }

        // Section 3: Amount & Impact Summary (System Derived)

        /// <summary>Total Adjustment - read-only sum of lines</summary>
        public decimal TotalAdjustmentAmount { get; set; } = 0;

        /// <summary>Direction - badge, derived from line impact. Values: ReduceAR, IncreaseAR</summary>
        public string AdjustmentDirection { get; set; } = "";

        // Section 4: Adjustment Lines (Grid)
        public List<ARAdjustmentLineViewModel> Lines { get; set; } = new List<ARAdjustmentLineViewModel>();

        // Section 5: Posting Classification (Accounting Mapping)

        /// <summary>FK → GLAccountMaster - AR Receivable Account snapshot at posting (read-only)</summary>
        public Guid? ReceivableAccountIdSnapshot { get; set; }
        public string? ReceivableAccountCode { get; set; }
        public string? ReceivableAccountName { get; set; }

        /// <summary>FK → GLAccountMaster - Default Write-Off Account (read-only fallback if line offset not specified)</summary>
        public Guid? DefaultWriteOffAccountId { get; set; }
        public string? DefaultWriteOffAccountCode { get; set; }
        public string? DefaultWriteOffAccountName { get; set; }

        /// <summary>Is System Generated - read-only bool</summary>
        public bool IsSystemGenerated { get; set; } = false;

        // Section 6: Workflow & Status

        /// <summary>Adjustment Status - badge + timeline. Values: Draft, Submitted, Approved, Posted, Cancelled, Reversed</summary>
        [Required(ErrorMessage = "Status is required")]
        public string AdjustmentStatus { get; set; } = AdjustmentStatuses.Draft;

        /// <summary>Posted On datetime - read-only</summary>
        public DateTime? PostedOn { get; set; }

        /// <summary>FK → User who posted - read-only</summary>
        public Guid? PostedByUserId { get; set; }
        public string? PostedByUserName { get; set; }

        /// <summary>Cancelled On datetime - pre-post only</summary>
        public DateTime? CancelledOn { get; set; }

        /// <summary>FK → User who cancelled</summary>
        public Guid? CancelledByUserId { get; set; }
        public string? CancelledByUserName { get; set; }

        /// <summary>Cancellation Reason</summary>
        [StringLength(250, ErrorMessage = "Cancellation Reason cannot exceed 250 characters")]
        public string? CancellationReason { get; set; }

        /// <summary>Reversed On datetime - optional</summary>
        public DateTime? ReversedOn { get; set; }

        /// <summary>FK → User who reversed</summary>
        public Guid? ReversedByUserId { get; set; }
        public string? ReversedByUserName { get; set; }

        /// <summary>Reversal Reason</summary>
        [StringLength(250, ErrorMessage = "Reversal Reason cannot exceed 250 characters")]
        public string? ReversalReason { get; set; }

        // Section 7: Attachments & Evidence

        /// <summary>Evidence Required - read-only bool, derived from type + amount threshold</summary>
        public bool EvidenceRequired { get; set; } = false;

        /// <summary>Evidence Attachment Count - badge</summary>
        public int EvidenceAttachmentCount { get; set; } = 0;

        // Section 8: System Audit Fields

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        // Helper Methods

        /// <summary>Recalculate totals from lines</summary>
        public void RecalculateTotals()
        {
            TotalAdjustmentAmount = Lines.Sum(l => l.AdjustmentAmount);
            
            // Determine direction based on most common line direction
            var reduceCount = Lines.Count(l => l.AdjustmentAmount > 0);
            AdjustmentDirection = reduceCount >= Lines.Count / 2.0 
                ? AdjustmentDirections.ReduceAR 
                : AdjustmentDirections.IncreaseAR;
        }

        /// <summary>Check if adjustment can be edited (Draft only)</summary>
        public bool CanEdit => AdjustmentStatus == AdjustmentStatuses.Draft;

        /// <summary>Check if adjustment can be submitted</summary>
        public bool CanSubmit => AdjustmentStatus == AdjustmentStatuses.Draft && Lines.Any() && TotalAdjustmentAmount > 0;

        /// <summary>Check if adjustment can be approved</summary>
        public bool CanApprove => AdjustmentStatus == AdjustmentStatuses.Submitted && RequiresApproval;

        /// <summary>Check if adjustment can be posted</summary>
        public bool CanPost => (AdjustmentStatus == AdjustmentStatuses.Draft && !RequiresApproval) ||
                               (AdjustmentStatus == AdjustmentStatuses.Approved) ||
                               (AdjustmentStatus == AdjustmentStatuses.Submitted && !RequiresApproval);

        /// <summary>Check if adjustment can be cancelled (pre-post only)</summary>
        public bool CanCancel => AdjustmentStatus == AdjustmentStatuses.Draft || AdjustmentStatus == AdjustmentStatuses.Submitted;

        /// <summary>Check if adjustment can be reversed (post-posting only)</summary>
        public bool CanReverse => AdjustmentStatus == AdjustmentStatuses.Posted;

        /// <summary>Check if narration is required based on type</summary>
        public bool IsNarrationRequired => AdjustmentType == AdjustmentTypes.WriteOff || 
                                           AdjustmentType == AdjustmentTypes.DisputeSettlement;
    }

    #endregion

    #region ARAdjustment Line Model

    /// <summary>
    /// ARAdjustmentLine - Child entity for adjustment lines (grid rows).
    /// Each line can optionally reference an invoice.
    /// </summary>
    public class ARAdjustmentLineViewModel
    {
        /// <summary>PK - hidden</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>FK → ARAdjustment</summary>
        public Guid ARAdjustmentId { get; set; }

        /// <summary>Line Number - auto-generated</summary>
        public int LineNumber { get; set; }

        /// <summary>FK → CustomerInvoice - optional. If set, reduces that invoice outstanding</summary>
        public Guid? CustomerInvoiceId { get; set; }
        public string? CustomerInvoiceNumber { get; set; }
        public decimal? InvoiceOutstanding { get; set; }

        /// <summary>Line Type - enum: WriteOff, Rounding, DiscountAllowed, Dispute, Reclassification, Other</summary>
        [Required(ErrorMessage = "Line Type is required")]
        public string LineType { get; set; } = "";

        /// <summary>Adjustment Amount - decimal 18,2, must be > 0</summary>
        [Required(ErrorMessage = "Adjustment Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Adjustment Amount must be greater than 0")]
        public decimal AdjustmentAmount { get; set; } = 0;

        /// <summary>FK → GLAccountMaster - Offset Account (expense/rounding/discount), required</summary>
        [Required(ErrorMessage = "Offset Account is required")]
        public Guid OffsetAccountId { get; set; }
        public string? OffsetAccountCode { get; set; }
        public string? OffsetAccountName { get; set; }

        /// <summary>Line Narration - optional, max 500 chars</summary>
        [StringLength(500, ErrorMessage = "Line Narration cannot exceed 500 characters")]
        public string? LineNarration { get; set; }

        // Audit fields
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }

    #endregion
    #region ARAdjustment Reason Master Model

    #endregion

    #region ARAdjustment-related Enums and Static Classes

    /// <summary>Adjustment Status values</summary>
    public static class AdjustmentStatuses
    {
        public const string Draft = "Draft";
        public const string Submitted = "Submitted";
        public const string Approved = "Approved";
        public const string Posted = "Posted";
        public const string Cancelled = "Cancelled";
        public const string Reversed = "Reversed";
        public static readonly string[] All = new[] { Draft, Submitted, Approved, Posted, Cancelled, Reversed };

        public static string GetDisplayName(string status) => status switch
        {
            Draft => "Draft",
            Submitted => "Submitted",
            Approved => "Approved",
            Posted => "Posted",
            Cancelled => "Cancelled",
            Reversed => "Reversed",
            _ => status
        };
    }

    /// <summary>Adjustment Type values</summary>
    public static class AdjustmentTypes
    {
        public const string WriteOff = "WriteOff";
        public const string Rounding = "Rounding";
        public const string DisputeSettlement = "DisputeSettlement";
        public const string ShortPaymentSettlement = "ShortPaymentSettlement";
        public const string Reclassification = "Reclassification";
        public const string BadDebtProvision = "BadDebtProvision";
        public const string Other = "Other";
        public static readonly string[] All = new[] { WriteOff, Rounding, DisputeSettlement, ShortPaymentSettlement, Reclassification, BadDebtProvision, Other };

        public static string GetDisplayName(string type) => type switch
        {
            WriteOff => "Write-Off",
            Rounding => "Rounding Adjustment",
            DisputeSettlement => "Dispute Settlement",
            ShortPaymentSettlement => "Short Payment Settlement",
            Reclassification => "Reclassification",
            BadDebtProvision => "Bad Debt Provision",
            Other => "Other",
            _ => type
        };

        public static string GetDescription(string type) => type switch
        {
            WriteOff => "Write off uncollectible amounts or small balances",
            Rounding => "Adjust for rounding differences in payments",
            DisputeSettlement => "Settlement of customer disputes",
            ShortPaymentSettlement => "Settle short payment discrepancies",
            Reclassification => "Move outstanding between invoices",
            BadDebtProvision => "Provision for bad debts",
            Other => "Other adjustment (specify in notes)",
            _ => string.Empty
        };
    }

    /// <summary>Adjustment Line Type values</summary>
    public static class AdjustmentLineTypes
    {
        public const string WriteOff = "WriteOff";
        public const string Rounding = "Rounding";
        public const string DiscountAllowed = "DiscountAllowed";
        public const string Dispute = "Dispute";
        public const string Reclassification = "Reclassification";
        public const string Other = "Other";
        public static readonly string[] All = new[] { WriteOff, Rounding, DiscountAllowed, Dispute, Reclassification, Other };

        public static string GetDisplayName(string type) => type switch
        {
            WriteOff => "Write-Off",
            Rounding => "Rounding",
            DiscountAllowed => "Discount Allowed",
            Dispute => "Dispute",
            Reclassification => "Reclassification",
            Other => "Other",
            _ => type
        };
    }

    /// <summary>Adjustment Direction values</summary>
    public static class AdjustmentDirections
    {
        public const string ReduceAR = "ReduceAR";
        public const string IncreaseAR = "IncreaseAR";

        public static string GetDisplayName(string direction) => direction switch
        {
            ReduceAR => "Reduce AR",
            IncreaseAR => "Increase AR",
            _ => direction
        };
    }

    /// <summary>AR Adjustment Approval Status values</summary>
    public static class ARAdjustmentApprovalStatuses
    {
        public const string NotRequired = "NotRequired";
        public const string Pending = "Pending";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
        public static readonly string[] All = new[] { NotRequired, Pending, Approved, Rejected };

        public static string GetDisplayName(string status) => status switch
        {
            NotRequired => "Not Required",
            Pending => "Pending Approval",
            Approved => "Approved",
            Rejected => "Rejected",
            _ => status
        };
    }

    #endregion
}
