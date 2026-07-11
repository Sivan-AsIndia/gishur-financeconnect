using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    #region Model #41: APAdjustment

    /// <summary>
    /// Model #41: APAdjustment – Controlled accounting document for AP balance corrections.
    /// Used for write-offs, rounding adjustments, dispute settlements, reclassifications, vendor balance transfers.
    /// Controls workflow, approval, posting, and audit compliance.
    /// </summary>
    public class APAdjustmentViewModel
    {
        // Section 1: Core Identity (Header)

        /// <summary>PK - hidden in UI</summary>
        public Guid APAdjustmentId { get; set; } = Guid.NewGuid();

        /// <summary>Tenant identifier - hidden in UI</summary>
        public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

        /// <summary>FK → Company - hidden</summary>
        [Required(ErrorMessage = "Company is required")]
        public Guid CompanyId { get; set; }
        public string? CompanyName { get; set; }

        /// <summary>FK → Branch - Dropdown, mandatory</summary>
        [Required(ErrorMessage = "Branch is required")]
        public Guid BranchId { get; set; }
        public string? BranchCode { get; set; }
        public string? BranchName { get; set; }

        /// <summary>FK → Vendor - Search dropdown, required. Vendor must be Active and not blocked</summary>
        [Required(ErrorMessage = "Vendor is required")]
        public Guid VendorId { get; set; }
        public string? VendorCode { get; set; }
        public string? VendorName { get; set; }

        /// <summary>FK → VendorAccount - Read-only derived from VendorId + CurrencyId</summary>
        public Guid? VendorAccountId { get; set; }
        public string? VendorAccountName { get; set; }

        /// <summary>Adjustment Number - max 40 chars, unique per CompanyId + FinancialYear, read-only system generated (e.g., APADJ-000001)</summary>
        [Required(ErrorMessage = "Adjustment Number is required")]
        [StringLength(40, ErrorMessage = "Adjustment Number cannot exceed 40 characters")]
        public string AdjustmentNumber { get; set; } = string.Empty;

        /// <summary>Adjustment Date (Entry) - date picker, required, default today</summary>
        [Required(ErrorMessage = "Adjustment Date is required")]
        public DateTime AdjustmentDate { get; set; } = DateTime.Today;

        /// <summary>Posting Date - conditional required at posting. Rule: open period only</summary>
        public DateTime? PostingDate { get; set; }

        /// <summary>FK → CurrencyMaster - Badge, default from vendor, required</summary>
        [Required(ErrorMessage = "Currency is required")]
        public Guid CurrencyId { get; set; }
        public string? CurrencyCode { get; set; }
        public string? CurrencyName { get; set; }

        /// <summary>Exchange Rate - numeric, required if Currency != base currency (Future-ready)</summary>
        public decimal ExchangeRate { get; set; } = 1;

        /// <summary>Notes / Explanation - textarea, max 1500 chars. Required (for adjustments always)</summary>
        [Required(ErrorMessage = "Narration is required for adjustments")]
        [StringLength(1500, ErrorMessage = "Narration cannot exceed 1500 characters")]
        public string? Narration { get; set; }

        // Section 2: Adjustment Classification

        /// <summary>Adjustment Type - dropdown, required. Values: WriteOff, RoundOffCorrection, DisputeSettlement, Reclassification, VendorBalanceTransfer, FXDifference, Other</summary>
        [Required(ErrorMessage = "Adjustment Type is required")]
        public string AdjustmentType { get; set; } = "";

        /// <summary>Adjustment Direction - dropdown, required, default: ReducePayable. Values: ReducePayable, IncreasePayable</summary>
        [Required(ErrorMessage = "Adjustment Direction is required")]
        public string AdjustmentDirection { get; set; } = "";

        /// <summary>FK → AdjustmentReasonMaster - Dropdown, required</summary>
        [Required(ErrorMessage = "Reason Code is required")]
        public Guid? ReasonCodeId { get; set; }
        public string? ReasonCode { get; set; }
        public string? ReasonDescription { get; set; }

        /// <summary>Policy Limit Category - Derived, read-only. Values: SmallWriteOff, Medium, HighRisk. Drives required approval level</summary>
        public string PolicyLimitCategory { get; set; } = "";

        // Section 3: Target & Impact (What exactly we are adjusting)

        /// <summary>Adjustment Scope - dropdown, required. Values: VendorLevel (overall payable), BillLevel (specific invoice), AdvanceLevel (advance/open credit bucket)</summary>
        [Required(ErrorMessage = "Adjustment Scope is required")]
        public string AdjustmentScope { get; set; } = "";

        /// <summary>FK → VendorBill (if BillLevel) - dropdown/search, required if AdjustmentScope = BillLevel. Filter: vendor + outstanding > 0</summary>
        public Guid? TargetVendorBillId { get; set; }
        public string? TargetVendorBillNumber { get; set; }

        /// <summary>Target Bill Outstanding Snapshot - read-only, show outstanding at time of adjustment creation</summary>
        public decimal? TargetBillOutstandingSnapshot { get; set; }

        /// <summary>Target Reference Text - textbox, max 100. Use: dispute id, email ref, ticket no, etc.</summary>
        [StringLength(100, ErrorMessage = "Target Reference Text cannot exceed 100 characters")]
        public string? TargetReferenceText { get; set; }

        /// <summary>Adjustment Amount - numeric, required, > 0</summary>
        [Required(ErrorMessage = "Adjustment Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Adjustment Amount must be greater than 0")]
        public decimal AdjustmentAmount { get; set; } = 0;

        /// <summary>Impact On Vendor Payable Amount - Derived, read-only. If ReducePayable → -Amount, If IncreasePayable → +Amount</summary>
        public decimal ImpactOnVendorPayableAmount => AdjustmentDirection == APAdjustmentDirections.ReducePayable
            ? -AdjustmentAmount
            : AdjustmentAmount;

        /// <summary>Impact On Vendor Advance Amount - Derived/Conditional, read-only. If adjusting advances/open credits</summary>
        public decimal ImpactOnVendorAdvanceAmount { get; set; } = 0;

        // Section 4: Accounting Mapping (GL Posting)

        /// <summary>AP Control Account snapshot at posting - FK → GLAccountMaster, read-only</summary>
        public Guid? APControlAccountIdSnapshot { get; set; }
        public string? APControlAccountCode { get; set; }
        public string? APControlAccountName { get; set; }

        /// <summary>Adjustment GL Account - FK → GLAccountMaster, search dropdown, required. Examples: Bad Debts/Write-off Expense, Rounding Income/Expense, Dispute Settlement Expense, Reclassification Liability</summary>
        [Required(ErrorMessage = "Adjustment GL Account is required")]
        public Guid? AdjustmentGLAccountId { get; set; }
        public string? AdjustmentGLAccountCode { get; set; }
        public string? AdjustmentGLAccountName { get; set; }

        /// <summary>Adjustment GL Account Type - Derived, read-only. Values: Expense, Income, Liability, Asset. For rule enforcement</summary>
        public string? AdjustmentGLAccountType { get; set; }

        /// <summary>Is Tax Impacting Adjustment - Optional bool, default false. Rare; generally tax is handled via notes</summary>
        public bool IsTaxImpactingAdjustment { get; set; } = false;

        /// <summary>FK → TaxCodeMaster - Optional, only if tax impacting (future policy)</summary>
        public Guid? TaxCodeId { get; set; }
        public string? TaxCodeName { get; set; }

        /// <summary>Posting Narration Snapshot - read-only, max 500. Narration passed into GL entry</summary>
        [StringLength(500, ErrorMessage = "Posting Narration cannot exceed 500 characters")]
        public string? PostingNarrationSnapshot { get; set; }

        // Section 5: Workflow & Status

        /// <summary>Adjustment Status - badge + timeline. Values: Draft, Submitted, Approved, Rejected, Posted, Cancelled (pre-post), Reversed (post)</summary>
        [Required(ErrorMessage = "Status is required")]
        public string AdjustmentStatus { get; set; } = APAdjustmentStatuses.Draft;

        /// <summary>Submitted On datetime - read-only</summary>
        public DateTime? SubmittedOn { get; set; }
        public Guid? SubmittedByUserId { get; set; }
        public string? SubmittedByUserName { get; set; }

        /// <summary>Approved On datetime - read-only</summary>
        public DateTime? ApprovedOn { get; set; }
        public Guid? ApprovedByUserId { get; set; }
        public string? ApprovedByUserName { get; set; }

        /// <summary>Posted On datetime - read-only</summary>
        public DateTime? PostedOn { get; set; }
        public Guid? PostedByUserId { get; set; }
        public string? PostedByUserName { get; set; }

        /// <summary>Rejection Reason</summary>
        [StringLength(500, ErrorMessage = "Rejection Reason cannot exceed 500 characters")]
        public string? RejectionReason { get; set; }

        /// <summary>Cancellation Reason</summary>
        [StringLength(500, ErrorMessage = "Cancellation Reason cannot exceed 500 characters")]
        public string? CancellationReason { get; set; }
        public DateTime? CancelledOn { get; set; }
        public Guid? CancelledByUserId { get; set; }
        public string? CancelledByUserName { get; set; }

        /// <summary>Reversal Reason</summary>
        [StringLength(500, ErrorMessage = "Reversal Reason cannot exceed 500 characters")]
        public string? ReversalReason { get; set; }
        public DateTime? ReversedOn { get; set; }
        public Guid? ReversedByUserId { get; set; }
        public string? ReversedByUserName { get; set; }

        // Section 6: Attachments & Evidence

        /// <summary>Has Attachments - bool badge</summary>
        public bool HasAttachments { get; set; } = false;

        /// <summary>Attachment Count - int badge</summary>
        public int AttachmentCount { get; set; } = 0;

        /// <summary>Evidence Required - Derived from type + amount threshold policy</summary>
        public bool EvidenceRequired { get; set; } = false;

        // Section 7: System Audit Fields (Hidden)

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        // Computed Properties

        /// <summary>Can Edit - only Draft adjustments can be edited</summary>
        public bool CanEdit => AdjustmentStatus == APAdjustmentStatuses.Draft;

        /// <summary>Can Submit - only Draft adjustments can be submitted</summary>
        public bool CanSubmit => AdjustmentStatus == APAdjustmentStatuses.Draft && AdjustmentAmount > 0;

        /// <summary>Can Approve - only Submitted adjustments that require approval</summary>
        public bool CanApprove => AdjustmentStatus == APAdjustmentStatuses.Submitted;

        /// <summary>Can Post - Approved or (Submitted and not requiring approval)</summary>
        public bool CanPost => AdjustmentStatus == APAdjustmentStatuses.Approved ||
                              (AdjustmentStatus == APAdjustmentStatuses.Submitted && PolicyLimitCategory == APPolicyLimitCategories.SmallWriteOff);

        /// <summary>Can Cancel - only Draft or Submitted (pre-post) adjustments</summary>
        public bool CanCancel => AdjustmentStatus == APAdjustmentStatuses.Draft ||
                                AdjustmentStatus == APAdjustmentStatuses.Submitted;

        /// <summary>Can Reverse - only Posted adjustments</summary>
        public bool CanReverse => AdjustmentStatus == APAdjustmentStatuses.Posted;

        /// <summary>Is Narration Required - true for write-off and dispute types</summary>
        public bool IsNarrationRequired => AdjustmentType == APAdjustmentTypes.WriteOff ||
                                          AdjustmentType == APAdjustmentTypes.DisputeSettlement;
    }

    #endregion

    #region APAdjustment Reason Master Model

    /// <summary>
    /// APAdjustmentReasonMaster - Lookup table for AP adjustment reasons.
    /// </summary>
    public class APAdjustmentReasonViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CompanyId { get; set; }

        [Required(ErrorMessage = "Reason Code is required")]
        [StringLength(20, ErrorMessage = "Reason Code cannot exceed 20 characters")]
        public string ReasonCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Reason Description is required")]
        [StringLength(100, ErrorMessage = "Reason Description cannot exceed 100 characters")]
        public string ReasonDescription { get; set; } = string.Empty;

        /// <summary>Applicable Adjustment Types for this reason</summary>
        public string[] ApplicableTypes { get; set; } = Array.Empty<string>();

        /// <summary>Default GL Offset Account for this reason</summary>
        public Guid? DefaultOffsetAccountId { get; set; }
        public string? DefaultOffsetAccountCode { get; set; }
        public string? DefaultOffsetAccountName { get; set; }

        /// <summary>Requires Approval for this reason</summary>
        public bool RequiresApproval { get; set; } = false;

        /// <summary>Requires Evidence/Attachment for this reason</summary>
        public bool RequiresEvidence { get; set; } = false;

        /// <summary>Amount Threshold above which approval is required</summary>
        public decimal? ApprovalThreshold { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
    }

    #endregion

    #region APAdjustment-related Enums and Static Classes

    /// <summary>AP Adjustment Status values</summary>
    public static class APAdjustmentStatuses
    {
        public const string Draft = "Draft";
        public const string Submitted = "Submitted";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
        public const string Posted = "Posted";
        public const string Cancelled = "Cancelled";
        public const string Reversed = "Reversed";
        public static readonly string[] All = new[] { Draft, Submitted, Approved, Rejected, Posted, Cancelled, Reversed };

        public static string GetDisplayName(string status) => status switch
        {
            Draft => "Draft",
            Submitted => "Submitted",
            Approved => "Approved",
            Rejected => "Rejected",
            Posted => "Posted",
            Cancelled => "Cancelled",
            Reversed => "Reversed",
            _ => status
        };
    }

    /// <summary>AP Adjustment Type values</summary>
    public static class APAdjustmentTypes
    {
        public const string WriteOff = "WriteOff";
        public const string RoundOffCorrection = "RoundOffCorrection";
        public const string DisputeSettlement = "DisputeSettlement";
        public const string Reclassification = "Reclassification";
        public const string VendorBalanceTransfer = "VendorBalanceTransfer";
        public const string FXDifference = "FXDifference";
        public const string Other = "Other";
        public static readonly string[] All = new[] { WriteOff, RoundOffCorrection, DisputeSettlement, Reclassification, VendorBalanceTransfer, FXDifference, Other };

        public static string GetDisplayName(string type) => type switch
        {
            WriteOff => "Write-Off",
            RoundOffCorrection => "Round-Off Correction",
            DisputeSettlement => "Dispute Settlement",
            Reclassification => "Reclassification",
            VendorBalanceTransfer => "Vendor Balance Transfer",
            FXDifference => "FX Difference",
            Other => "Other",
            _ => type
        };

        public static string GetDescription(string type) => type switch
        {
            WriteOff => "Write off small remaining balances or uncollectible amounts",
            RoundOffCorrection => "Adjust for rounding differences",
            DisputeSettlement => "Settlement of vendor disputes",
            Reclassification => "Move AP to other liability or expense",
            VendorBalanceTransfer => "Transfer balance between vendors (rare, strict)",
            FXDifference => "Foreign exchange difference (future)",
            Other => "Other adjustment (specify in notes)",
            _ => string.Empty
        };
    }

    /// <summary>AP Adjustment Direction values</summary>
    public static class APAdjustmentDirections
    {
        public const string ReducePayable = "ReducePayable";
        public const string IncreasePayable = "IncreasePayable";
        public static readonly string[] All = new[] { ReducePayable, IncreasePayable };

        public static string GetDisplayName(string direction) => direction switch
        {
            ReducePayable => "Reduce Payable",
            IncreasePayable => "Increase Payable",
            _ => direction
        };
    }

    /// <summary>AP Adjustment Scope values</summary>
    public static class APAdjustmentScopes
    {
        public const string VendorLevel = "VendorLevel";
        public const string BillLevel = "BillLevel";
        public const string AdvanceLevel = "AdvanceLevel";
        public static readonly string[] All = new[] { VendorLevel, BillLevel, AdvanceLevel };

        public static string GetDisplayName(string scope) => scope switch
        {
            VendorLevel => "Vendor Level",
            BillLevel => "Bill Level",
            AdvanceLevel => "Advance Level",
            _ => scope
        };

        public static string GetDescription(string scope) => scope switch
        {
            VendorLevel => "Overall vendor payable adjustment",
            BillLevel => "Specific vendor bill adjustment",
            AdvanceLevel => "Advance/open credit bucket adjustment",
            _ => string.Empty
        };
    }

    /// <summary>Policy Limit Category values</summary>
    public static class APPolicyLimitCategories
    {
        public const string SmallWriteOff = "SmallWriteOff";
        public const string Medium = "Medium";
        public const string HighRisk = "HighRisk";
        public static readonly string[] All = new[] { SmallWriteOff, Medium, HighRisk };

        public static string GetDisplayName(string category) => category switch
        {
            SmallWriteOff => "Small Write-Off",
            Medium => "Medium",
            HighRisk => "High Risk",
            _ => category
        };
    }

    /// <summary>GL Account Type values for adjustment account</summary>
    public static class APAdjustmentGLAccountTypes
    {
        public const string Expense = "Expense";
        public const string Income = "Income";
        public const string Liability = "Liability";
        public const string Asset = "Asset";
        public static readonly string[] All = new[] { Expense, Income, Liability, Asset };
    }

    #endregion
}
