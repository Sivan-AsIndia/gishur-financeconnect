using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    #region Model #40: VendorDebitNote

    /// <summary>
    /// Model #40: VendorDebitNote – AP increase document received from vendor.
    /// Used for price increases, freight charges, penalty charges, tax differences, billing corrections.
    /// Controls workflow, totals, posting, and compliance.
    /// </summary>
    public class VendorDebitNoteViewModel
    {
        // Section 1: Core Identity Fields (Header)

        /// <summary>PK - VendorDebitNoteId - hidden in UI</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>TenantId - hidden in UI</summary>
        public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

        /// <summary>FK → Company - hidden</summary>
        [Required(ErrorMessage = "Company is required")]
        public Guid CompanyId { get; set; }
        public string? CompanyName { get; set; }

        /// <summary>FK → Branch - Dropdown - Mandatory</summary>
        [Required(ErrorMessage = "Branch is required")]
        public Guid BranchId { get; set; }
        public string? BranchCode { get; set; }
        public string? BranchName { get; set; }

        /// <summary>FK → Vendor - Search dropdown (typeahead) - Mandatory</summary>
        [Required(ErrorMessage = "Vendor is required")]
        public Guid VendorId { get; set; }
        public string? VendorCode { get; set; }
        public string? VendorName { get; set; }

        /// <summary>FK → VendorAccount - Read-only (system derived by VendorId + CurrencyId)</summary>
        public Guid? VendorAccountId { get; set; }
        public string? VendorAccountName { get; set; }

        /// <summary>DebitNoteNumber (System) - max 40 chars, unique per CompanyId + FinancialYear, read-only</summary>
        [Required(ErrorMessage = "Debit Note Number is required")]
        [StringLength(40, ErrorMessage = "Debit Note Number cannot exceed 40 characters")]
        public string DebitNoteNumber { get; set; } = string.Empty;

        /// <summary>VendorDebitNoteReferenceNumber (Critical) - max 50 chars - textbox</summary>
        [Required(ErrorMessage = "Vendor Debit Note Reference Number is required")]
        [StringLength(50, ErrorMessage = "Reference Number cannot exceed 50 characters")]
        public string VendorDebitNoteReferenceNumber { get; set; } = string.Empty;

        /// <summary>VendorDebitNoteDate - date picker</summary>
        [Required(ErrorMessage = "Vendor Debit Note Date is required")]
        public DateTime VendorDebitNoteDate { get; set; } = DateTime.Today;

        /// <summary>DebitEntryDate - date picker, default today</summary>
        [Required(ErrorMessage = "Entry Date is required")]
        public DateTime DebitEntryDate { get; set; } = DateTime.Today;

        /// <summary>PostingDate - conditional required at posting, open period</summary>
        public DateTime? PostingDate { get; set; }

        /// <summary>FK → CurrencyMaster - badge, required</summary>
        [Required(ErrorMessage = "Currency is required")]
        public Guid CurrencyId { get; set; }
        public string? CurrencyCode { get; set; }
        public string? CurrencyName { get; set; }

        /// <summary>ExchangeRate - decimal(18,6), numeric, required if Currency != base currency</summary>
        [Range(0.000001, 9999999999, ErrorMessage = "Exchange Rate must be positive")]
        public decimal ExchangeRate { get; set; } = 1;

        /// <summary>DebitNoteNarration / Notes - max 1000 chars - textarea, optional</summary>
        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
        public string? DebitNoteNarration { get; set; }

        // Section 2: Debit Note Classification

        /// <summary>DebitNoteType - Enum dropdown - required</summary>
        [Required(ErrorMessage = "Debit Note Type is required")]
        public string DebitNoteType { get; set; } = "";

        /// <summary>ReasonCode - FK → APReasonCodeMaster (future) - dropdown - optional</summary>
        public string? ReasonCode { get; set; }

        /// <summary>IsAgainstBill - toggle - default true (recommended)</summary>
        public bool IsAgainstBill { get; set; } = true;

        /// <summary>PrimaryVendorBillId - FK → VendorBill - dropdown/search - optional</summary>
        public Guid? PrimaryVendorBillId { get; set; }
        public string? PrimaryVendorBillNumber { get; set; }

        /// <summary>Bill Number Snapshot - read-only</summary>
        [StringLength(50)]
        public string? BillNumberSnapshot { get; set; }

        /// <summary>Bill Date Snapshot - read-only</summary>
        public DateTime? BillDateSnapshot { get; set; }

        // Section 3: Totals (System Controlled - Read-only)

        /// <summary>SubTotalDebitAmount - Sum of line values (before tax)</summary>
        [Required]
        public decimal SubTotalDebitAmount { get; set; } = 0;

        /// <summary>TaxDebitAmount - Sum of all line tax amounts</summary>
        public decimal TaxDebitAmount { get; set; } = 0;

        /// <summary>RoundOffAmount - derived based on rounding precision</summary>
        public decimal RoundOffAmount { get; set; } = 0;

        /// <summary>TotalDebitAmount = SubTotalDebit + TaxDebit + RoundOff</summary>
        [Required]
        public decimal TotalDebitAmount { get; set; } = 0;

        /// <summary>AppliedAmount (Derived) - sum of posted application rows</summary>
        public decimal AppliedAmount { get; set; } = 0;

        /// <summary>RemainingUnappliedAmount (Derived) = TotalDebitAmount - AppliedAmount</summary>
        public decimal RemainingUnappliedAmount => TotalDebitAmount - AppliedAmount;

        /// <summary>DebitSettlementStatus (Derived) - Unapplied/PartiallyApplied/FullyApplied - badge</summary>
        public string DebitSettlementStatus
        {
            get
            {
                if (AppliedAmount <= 0) return VendorDebitNoteSettlementStatuses.Unapplied;
                if (AppliedAmount >= TotalDebitAmount) return VendorDebitNoteSettlementStatuses.FullyApplied;
                return VendorDebitNoteSettlementStatuses.PartiallyApplied;
            }
        }

        // Section 4: Tax & Compliance (India-ready)

        /// <summary>IsGSTApplicable - toggle, default from vendor</summary>
        public bool IsGSTApplicable { get; set; } = true;

        /// <summary>VendorGSTINSnapshot - max 15 chars - read-only snapshot at posting</summary>
        [StringLength(15)]
        public string? VendorGSTINSnapshot { get; set; }

        /// <summary>PlaceOfSupplyStateId - FK → State - dropdown (future)</summary>
        public Guid? PlaceOfSupplyStateId { get; set; }
        public string? PlaceOfSupplyStateCode { get; set; }
        public string? PlaceOfSupplyStateName { get; set; }

        /// <summary>IsReverseChargeApplicable - toggle - default false (future)</summary>
        public bool IsReverseChargeApplicable { get; set; } = false;

        /// <summary>IsTDSApplicable - toggle (optional future)</summary>
        public bool IsTDSApplicable { get; set; } = false;

        // Section 5: Posting Classification (Accounting Mapping)

        /// <summary>FK → GLAccountMaster - AP Payable Account snapshot at posting</summary>
        public Guid? PayableAccountIdSnapshot { get; set; }
        public string? PayableAccountCode { get; set; }
        public string? PayableAccountName { get; set; }

        /// <summary>FK → GLAccountMaster - Default Expense/Asset Account</summary>
        public Guid? ExpenseAccountId { get; set; }
        public string? ExpenseAccountCode { get; set; }
        public string? ExpenseAccountName { get; set; }

        /// <summary>FK → GLAccountMaster - Tax Account snapshot</summary>
        public Guid? TaxAccountIdSnapshot { get; set; }
        public string? TaxAccountCode { get; set; }
        public string? TaxAccountName { get; set; }

        // Section 6: Workflow & Status

        /// <summary>DebitNoteStatus - Draft/Submitted/Approved/Rejected/Posted/Cancelled/Reversed - badge + timeline</summary>
        [Required(ErrorMessage = "Status is required")]
        public string DebitNoteStatus { get; set; } = VendorDebitNoteStatuses.Draft;

        /// <summary>SubmittedOn - datetime</summary>
        public DateTime? SubmittedOn { get; set; }

        /// <summary>FK → User who submitted</summary>
        public Guid? SubmittedByUserId { get; set; }
        public string? SubmittedByUserName { get; set; }

        /// <summary>ApprovedOn - datetime</summary>
        public DateTime? ApprovedOn { get; set; }

        /// <summary>FK → User who approved</summary>
        public Guid? ApprovedByUserId { get; set; }
        public string? ApprovedByUserName { get; set; }

        /// <summary>PostedOn - datetime</summary>
        public DateTime? PostedOn { get; set; }

        /// <summary>FK → User who posted</summary>
        public Guid? PostedByUserId { get; set; }
        public string? PostedByUserName { get; set; }

        /// <summary>RejectionReason</summary>
        [StringLength(250, ErrorMessage = "Rejection Reason cannot exceed 250 characters")]
        public string? RejectionReason { get; set; }

        /// <summary>CancelledOn - datetime (pre-post only)</summary>
        public DateTime? CancelledOn { get; set; }

        /// <summary>FK → User who cancelled</summary>
        public Guid? CancelledByUserId { get; set; }
        public string? CancelledByUserName { get; set; }

        /// <summary>CancellationReason</summary>
        [StringLength(250, ErrorMessage = "Cancellation Reason cannot exceed 250 characters")]
        public string? CancellationReason { get; set; }

        /// <summary>ReversedOn - datetime (post only)</summary>
        public DateTime? ReversedOn { get; set; }

        /// <summary>FK → User who reversed</summary>
        public Guid? ReversedByUserId { get; set; }
        public string? ReversedByUserName { get; set; }

        /// <summary>ReversalReason</summary>
        [StringLength(250, ErrorMessage = "Reversal Reason cannot exceed 250 characters")]
        public string? ReversalReason { get; set; }

        // Section 7: Attachments & Evidence

        /// <summary>HasAttachments - bool badge</summary>
        public bool HasAttachments { get; set; } = false;

        /// <summary>AttachmentCount - int badge</summary>
        public int AttachmentCount { get; set; } = 0;

        // Section 8: Debit Note Lines (Navigation property)

        /// <summary>Debit Note Lines collection</summary>
        public List<VendorDebitNoteLineViewModel> Lines { get; set; } = new List<VendorDebitNoteLineViewModel>();

        // Section 9: System Audit Fields (Hidden)

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
            SubTotalDebitAmount = Lines.Sum(l => l.TaxableAmount);
            TaxDebitAmount = Lines.Sum(l => l.TaxAmount);
            var total = SubTotalDebitAmount + TaxDebitAmount;
            RoundOffAmount = Math.Round(total) - total;
            TotalDebitAmount = total + RoundOffAmount;
        }

        /// <summary>Check if debit note can be edited</summary>
        public bool CanEdit => DebitNoteStatus == VendorDebitNoteStatuses.Draft;

        /// <summary>Check if debit note can be posted</summary>
        public bool CanPost => DebitNoteStatus == VendorDebitNoteStatuses.Draft || 
                               DebitNoteStatus == VendorDebitNoteStatuses.Approved;

        /// <summary>Check if debit note can be cancelled (pre-post only)</summary>
        public bool CanCancel => DebitNoteStatus == VendorDebitNoteStatuses.Draft || 
                                 DebitNoteStatus == VendorDebitNoteStatuses.Submitted;

        /// <summary>Check if debit note can be reversed (post-posting only)</summary>
        public bool CanReverse => DebitNoteStatus == VendorDebitNoteStatuses.Posted;
    }

    #endregion

    #region VendorDebitNote-related Enums and Static Classes

    public static class VendorDebitNoteStatuses
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

    public static class VendorDebitNoteTypes
    {
        public const string PriceIncrease = "PriceIncrease";
        public const string FreightCharges = "FreightCharges";
        public const string PenaltyCharges = "PenaltyCharges";
        public const string ServiceAddOn = "ServiceAddOn";
        public const string TaxDifference = "TaxDifference";
        public const string BillingCorrection = "BillingCorrection";
        public const string Other = "Other";
        public static readonly string[] All = new[] { PriceIncrease, FreightCharges, PenaltyCharges, ServiceAddOn, TaxDifference, BillingCorrection, Other };

        public static string GetDisplayName(string code) => code switch
        {
            PriceIncrease => "Price Increase",
            FreightCharges => "Freight Charges",
            PenaltyCharges => "Penalty Charges",
            ServiceAddOn => "Service Add-On",
            TaxDifference => "Tax Difference",
            BillingCorrection => "Billing Correction",
            Other => "Other",
            _ => code
        };

        public static string GetDescription(string code) => code switch
        {
            PriceIncrease => "Additional charge due to price increase from vendor",
            FreightCharges => "Additional freight, shipping, or delivery charges",
            PenaltyCharges => "Penalty charges from vendor (late receipt, damage, etc.)",
            ServiceAddOn => "Additional service charges not in original billing",
            TaxDifference => "Tax differential or correction from vendor",
            BillingCorrection => "Correction for under-billed items in original invoice",
            Other => "Other reason (specify in notes)",
            _ => string.Empty
        };
    }

    public static class VendorDebitNoteSettlementStatuses
    {
        public const string Unapplied = "Unapplied";
        public const string PartiallyApplied = "PartiallyApplied";
        public const string FullyApplied = "FullyApplied";

        public static string GetDisplayName(string status) => status switch
        {
            Unapplied => "Unapplied",
            PartiallyApplied => "Partially Applied",
            FullyApplied => "Fully Applied",
            _ => status
        };
    }

    public static class VendorDebitNoteLineTypes
    {
        public const string Expense = "Expense";
        public const string Asset = "Asset";
        public const string Service = "Service";
        public const string Charge = "Charge";
        public static readonly string[] All = new[] { Expense, Asset, Service, Charge };
    }

    #endregion
}
