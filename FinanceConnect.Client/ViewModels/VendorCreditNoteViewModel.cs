using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    #region Model #39: VendorCreditNote

    /// <summary>
    /// Model #39: VendorCreditNote – AP reduction document issued by a vendor (or recorded from vendor's credit memo) 
    /// to reduce what we owe them. Used for: purchase returns, price reductions, discount rebates, billing corrections, 
    /// damage claims, and other adjustments.
    /// </summary>
    public class VendorCreditNoteViewModel
    {
        // Section 1: Core Identity (Header)

        /// <summary>PK - VendorCreditNoteId - hidden in UI</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

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

        /// <summary>FK → Vendor - Search dropdown, mandatory, must be Active and not blocked</summary>
        [Required(ErrorMessage = "Vendor is required")]
        public Guid VendorId { get; set; }
        public string? VendorCode { get; set; }
        public string? VendorName { get; set; }

        /// <summary>FK → VendorAccount - Read-only (system derived from VendorId + CurrencyId)</summary>
        public Guid? VendorAccountId { get; set; }
        public string? VendorAccountName { get; set; }

        /// <summary>Credit Note Number (System) - max 40 chars, read-only, auto-generated (e.g., APCN-000001)</summary>
        [Required(ErrorMessage = "Credit Note Number is required")]
        [StringLength(40, ErrorMessage = "Credit Note Number cannot exceed 40 characters")]
        public string CreditNoteNumber { get; set; } = string.Empty;

        /// <summary>Vendor Credit Note Reference Number (Critical) - max 50 chars, textbox, required, 
        /// unique per Vendor within Company (anti-duplicate fraud control)</summary>
        [Required(ErrorMessage = "Vendor Credit Note Reference Number is required")]
        [StringLength(50, ErrorMessage = "Vendor Credit Note Reference Number cannot exceed 50 characters")]
        public string VendorCreditNoteReferenceNumber { get; set; } = string.Empty;

        /// <summary>Vendor Credit Note Date - date picker, required</summary>
        [Required(ErrorMessage = "Vendor Credit Note Date is required")]
        public DateTime VendorCreditNoteDate { get; set; } = DateTime.Today;

        /// <summary>Credit Entry Date - date picker, required, default today</summary>
        [Required(ErrorMessage = "Credit Entry Date is required")]
        public DateTime CreditEntryDate { get; set; } = DateTime.Today;

        /// <summary>Posting Date - controller/system, conditional required at posting, open period only</summary>
        public DateTime? PostingDate { get; set; }

        /// <summary>FK → CurrencyMaster - badge (default vendor currency)</summary>
        [Required(ErrorMessage = "Currency is required")]
        public Guid CurrencyId { get; set; }
        public string? CurrencyCode { get; set; }
        public string? CurrencyName { get; set; }

        /// <summary>Exchange Rate - numeric, required if currency != base currency</summary>
        [Range(0.000001, 9999999999, ErrorMessage = "Exchange Rate must be positive")]
        public decimal ExchangeRate { get; set; } = 1;

        /// <summary>Credit Note Narration / Notes - textarea, max 1000 chars, optional</summary>
        [StringLength(1000, ErrorMessage = "Narration cannot exceed 1000 characters")]
        public string? CreditNoteNarration { get; set; }

        // Section 2: Credit Note Classification

        /// <summary>Credit Note Type - dropdown, required
        /// Values: PurchaseReturn, PriceReduction, DiscountRebate, BillingCorrection, DamageClaim, Other</summary>
        [Required(ErrorMessage = "Credit Type is required")]
        public string CreditNoteType { get; set; } = "";

        /// <summary>FK → APReasonCodeMaster - dropdown (future)</summary>
        public Guid? ReasonCodeId { get; set; }
        public string? ReasonCodeName { get; set; }

        /// <summary>Is Against Bill - toggle, default true (recommended)</summary>
        public bool IsAgainstBill { get; set; } = true;

        /// <summary>FK → VendorBill - Primary Vendor Bill Id - dropdown/search, optional (quick single-bill reference)</summary>
        public Guid? PrimaryVendorBillId { get; set; }
        public string? PrimaryVendorBillNumber { get; set; }
        public string? BillNumberSnapshot { get; set; }
        public DateTime? BillDateSnapshot { get; set; }

        // Section 3: Totals (System Controlled - Read-only)

        /// <summary>Subtotal Credit Amount - sum of line values</summary>
        [Required]
        public decimal SubTotalCreditAmount { get; set; } = 0;

        /// <summary>Tax Credit Amount - sum of line tax amounts</summary>
        public decimal TaxCreditAmount { get; set; } = 0;

        /// <summary>Round Off Amount - derived based on rounding precision</summary>
        public decimal RoundOffAmount { get; set; } = 0;

        /// <summary>Total Credit Amount = SubTotalCredit + TaxCredit + RoundOff</summary>
        [Required]
        public decimal TotalCreditAmount { get; set; } = 0;

        /// <summary>Applied Amount (Derived) - sum of posted application rows</summary>
        public decimal AppliedAmount { get; set; } = 0;

        /// <summary>Remaining Open Credit Amount (Derived) = TotalCreditAmount - AppliedAmount</summary>
        public decimal RemainingOpenCreditAmount => TotalCreditAmount - AppliedAmount;

        /// <summary>Credit Settlement Status (Derived) - Unapplied/PartiallyApplied/FullyApplied</summary>
        public string CreditSettlementStatus => AppliedAmount <= 0 ? CreditSettlementStatuses.Unapplied
            : (AppliedAmount >= TotalCreditAmount ? CreditSettlementStatuses.FullyApplied : CreditSettlementStatuses.PartiallyApplied);

        // Section 4: Tax & Compliance (India-ready)

        /// <summary>Is GST Applicable - toggle, default from original bill/vendor</summary>
        public bool IsGSTApplicable { get; set; } = true;

        /// <summary>Vendor GSTIN Snapshot - max 15 chars, read-only, snapshot at posting for audit</summary>
        [StringLength(15, ErrorMessage = "GSTIN cannot exceed 15 characters")]
        public string? VendorGSTINSnapshot { get; set; }

        /// <summary>FK → State - Place of Supply State Id (optional), for IGST vs CGST/SGST logic</summary>
        public Guid? PlaceOfSupplyStateId { get; set; }
        public string? PlaceOfSupplyStateName { get; set; }
        public string? PlaceOfSupplyStateCode { get; set; }

        /// <summary>Is Reverse Charge Applicable - toggle, default false</summary>
        public bool IsReverseChargeApplicable { get; set; } = false;

        // Section 5: Workflow & Status

        /// <summary>Credit Note Status - Draft/Submitted/Approved/Rejected/Posted/Cancelled/Reversed</summary>
        [Required(ErrorMessage = "Status is required")]
        public string CreditNoteStatus { get; set; } = VendorCreditNoteStatuses.Draft;

        /// <summary>Submitted On - datetime</summary>
        public DateTime? SubmittedOn { get; set; }

        /// <summary>FK → User who submitted</summary>
        public Guid? SubmittedByUserId { get; set; }
        public string? SubmittedByUserName { get; set; }

        /// <summary>Approved On - datetime</summary>
        public DateTime? ApprovedOn { get; set; }

        /// <summary>FK → User who approved</summary>
        public Guid? ApprovedByUserId { get; set; }
        public string? ApprovedByUserName { get; set; }

        /// <summary>Posted On - datetime</summary>
        public DateTime? PostedOn { get; set; }

        /// <summary>FK → User who posted</summary>
        public Guid? PostedByUserId { get; set; }
        public string? PostedByUserName { get; set; }

        /// <summary>Rejection Reason - required if Rejected</summary>
        [StringLength(250, ErrorMessage = "Rejection Reason cannot exceed 250 characters")]
        public string? RejectionReason { get; set; }

        /// <summary>Cancellation Reason - required if Cancelled</summary>
        [StringLength(250, ErrorMessage = "Cancellation Reason cannot exceed 250 characters")]
        public string? CancellationReason { get; set; }

        /// <summary>Cancelled On - datetime</summary>
        public DateTime? CancelledOn { get; set; }

        /// <summary>FK → User who cancelled</summary>
        public Guid? CancelledByUserId { get; set; }
        public string? CancelledByUserName { get; set; }

        /// <summary>Reversal Reason - required if Reversed</summary>
        [StringLength(250, ErrorMessage = "Reversal Reason cannot exceed 250 characters")]
        public string? ReversalReason { get; set; }

        /// <summary>Reversed On - datetime</summary>
        public DateTime? ReversedOn { get; set; }

        /// <summary>FK → User who reversed</summary>
        public Guid? ReversedByUserId { get; set; }
        public string? ReversedByUserName { get; set; }

        // Section 6: Attachments & Evidence

        /// <summary>Has Attachments - bool badge</summary>
        public bool HasAttachments { get; set; } = false;

        /// <summary>Attachment Count - int badge</summary>
        public int AttachmentCount { get; set; } = 0;

        // Section 7: Credit Note Lines (Navigation property)

        /// <summary>Credit Note Lines collection</summary>
        public List<VendorCreditNoteLineModel> Lines { get; set; } = new List<VendorCreditNoteLineModel>();

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
            SubTotalCreditAmount = Lines.Sum(l => l.LineSubTotalAmount);
            TaxCreditAmount = Lines.Sum(l => l.TaxAmount);
            var total = SubTotalCreditAmount + TaxCreditAmount;
            RoundOffAmount = Math.Round(total) - total;
            TotalCreditAmount = total + RoundOffAmount;
        }

        /// <summary>Check if credit note can be edited</summary>
        public bool CanEdit => CreditNoteStatus == VendorCreditNoteStatuses.Draft;

        /// <summary>Check if credit note can be posted</summary>
        public bool CanPost => CreditNoteStatus == VendorCreditNoteStatuses.Draft || CreditNoteStatus == VendorCreditNoteStatuses.Approved;

        /// <summary>Check if credit note can be cancelled</summary>
        public bool CanCancel => CreditNoteStatus == VendorCreditNoteStatuses.Draft || CreditNoteStatus == VendorCreditNoteStatuses.Submitted;

        // Alias properties for UI convenience
        public string Status => CreditNoteStatus;
        public decimal SubTotal => SubTotalCreditAmount;
        public decimal TaxTotal => TaxCreditAmount;
        public decimal GrandTotal => TotalCreditAmount;
        public string? Narration => CreditNoteNarration;
        public DateTime CreatedDate => CreatedAt;
        public DateTime? ModifiedDate => UpdatedAt;
        public string? ModifiedBy => UpdatedBy;
        public DateTime? ApprovedAt => ApprovedOn;
        public DateTime? ApprovedDate => ApprovedOn;
        public string? ApprovedBy => ApprovedByUserName;
        public DateTime? PostedAt => PostedOn;
        public DateTime? PostedDate => PostedOn;
        public string? PostedBy => PostedByUserName;
    }

    #endregion

    #region VendorCreditNote-related Enums and Static Classes

    public static class VendorCreditNoteStatuses
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

    public static class VendorCreditNoteTypes
    {
        public const string PurchaseReturn = "PurchaseReturn";
        public const string PriceReduction = "PriceReduction";
        public const string DiscountRebate = "DiscountRebate";
        public const string BillingCorrection = "BillingCorrection";
        public const string DamageClaim = "DamageClaim";
        public const string Other = "Other";
        public static readonly string[] All = new[] { PurchaseReturn, PriceReduction, DiscountRebate, BillingCorrection, DamageClaim, Other };

        public static string GetDisplayName(string type) => type switch
        {
            PurchaseReturn => "Purchase Return",
            PriceReduction => "Price Reduction",
            DiscountRebate => "Discount/Rebate",
            BillingCorrection => "Billing Correction",
            DamageClaim => "Damage Claim",
            Other => "Other",
            _ => type
        };
    }

    public static class CreditSettlementStatuses
    {
        public const string Unapplied = "Unapplied";
        public const string PartiallyApplied = "PartiallyApplied";
        public const string FullyApplied = "FullyApplied";
        public static readonly string[] All = new[] { Unapplied, PartiallyApplied, FullyApplied };

        public static string GetDisplayName(string status) => status switch
        {
            Unapplied => "Unapplied",
            PartiallyApplied => "Partially Applied",
            FullyApplied => "Fully Applied",
            _ => status
        };
    }

    #endregion
}
