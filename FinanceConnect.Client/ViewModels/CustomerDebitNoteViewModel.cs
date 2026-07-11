using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    #region Model #31: CustomerDebitNote

    /// <summary>
    /// Model #31: CustomerDebitNote – AR increase document issued to customer.
    /// Used for under-billing corrections, additional charges, late fees, tax corrections.
    /// Controls workflow, totals, posting, and compliance.
    /// </summary>
    public class CustomerDebitNoteViewModel
    {
        // Section 1: Core Debit Note Identity Fields (Header)

        /// <summary>PK - hidden in UI</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Tenant identifier - hidden in UI</summary>
        public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

        /// <summary>FK → Company - hidden</summary>
        [Required(ErrorMessage = "Company is required")]
        public Guid CompanyId { get; set; }
        public string? CompanyName { get; set; }

        /// <summary>FK → Branch - Dropdown filtered by CompanyId</summary>
        [Required(ErrorMessage = "Branch is required")]
        public Guid BranchId { get; set; }
        public string? BranchCode { get; set; }
        public string? BranchName { get; set; }

        /// <summary>FK → Customer - Search dropdown (typeahead)</summary>
        [Required(ErrorMessage = "Customer is required")]
        public Guid CustomerId { get; set; }
        public string? CustomerCode { get; set; }
        public string? CustomerName { get; set; }

        /// <summary>FK → CustomerAccount - Read-only (system derived)</summary>
        public Guid? CustomerAccountId { get; set; }
        public string? CustomerAccountName { get; set; }

        /// <summary>Debit Note Number - max 40 chars, unique per CompanyId + FinancialYear</summary>
        [Required(ErrorMessage = "Debit Note Number is required")]
        [StringLength(40, ErrorMessage = "Debit Note Number cannot exceed 40 characters")]
        public string DebitNoteNumber { get; set; } = string.Empty;

        /// <summary>Debit Note Date - date picker</summary>
        [Required(ErrorMessage = "Debit Note Date is required")]
        public DateTime DebitNoteDate { get; set; } = DateTime.Today;

        /// <summary>Posting Date - conditional required at posting</summary>
        public DateTime? PostingDate { get; set; }

        /// <summary>FK → CurrencyMaster - from Customer.DefaultCurrencyId</summary>
        [Required(ErrorMessage = "Currency is required")]
        public Guid CurrencyId { get; set; }
        public string? CurrencyCode { get; set; }
        public string? CurrencyName { get; set; }

        /// <summary>Exchange Rate - required if foreign currency</summary>
        [Range(0.000001, 9999999999, ErrorMessage = "Exchange Rate must be positive")]
        public decimal ExchangeRate { get; set; } = 1;

        /// <summary>Reference - PO number, dispute id, contract ref, etc.</summary>
        [StringLength(100, ErrorMessage = "Reference cannot exceed 100 characters")]
        public string? ReferenceText { get; set; }

        /// <summary>Notes / Narration - textarea</summary>
        [StringLength(1000, ErrorMessage = "Narration cannot exceed 1000 characters")]
        public string? DebitNoteNarration { get; set; }

        // Section 2: Reference (Linking to Invoice)

        /// <summary>Against Invoice? - toggle</summary>
        public bool IsAgainstInvoice { get; set; } = true;

        /// <summary>FK → CustomerInvoice - optional, required if IsAgainstInvoice = true</summary>
        public Guid? CustomerInvoiceId { get; set; }
        public string? CustomerInvoiceNumber { get; set; }

        /// <summary>Invoice Number Snapshot - read-only</summary>
        [StringLength(40)]
        public string? InvoiceNumberSnapshot { get; set; }

        /// <summary>Invoice Date Snapshot - read-only</summary>
        public DateTime? InvoiceDateSnapshot { get; set; }

        // Section 3: Debit Reason & Classification (Mandatory)

        /// <summary>Reason Code - dropdown</summary>
        [Required(ErrorMessage = "Reason Code is required")]
        public string DebitReasonCode { get; set; } = DebitReasonCodes.UnderbillingCorrection;

        /// <summary>Reason Description - auto-fill from master; editable optional</summary>
        [StringLength(250, ErrorMessage = "Reason Description cannot exceed 250 characters")]
        public string? DebitReasonDescription { get; set; }

        /// <summary>Affects Tax? - toggle</summary>
        public bool IsTaxImpacting { get; set; } = true;

        /// <summary>Revenue Recognized - toggle</summary>
        public bool IsRevenueRecognized { get; set; } = true;

        // Section 4: Totals (System Controlled - Read-only)

        /// <summary>Subtotal - Sum of line values (before tax)</summary>
        [Required]
        public decimal SubTotalAmount { get; set; } = 0;

        /// <summary>Discount - Sum of line discounts</summary>
        public decimal DiscountTotalAmount { get; set; } = 0;

        /// <summary>Total Tax - Sum of all line tax components</summary>
        public decimal TaxTotalAmount { get; set; } = 0;

        /// <summary>Round Off - derived based on rounding precision</summary>
        public decimal RoundOffAmount { get; set; } = 0;

        /// <summary>Grand Total = SubTotal - Discount + Tax + RoundOff</summary>
        [Required]
        public decimal GrandTotalAmount { get; set; } = 0;

        /// <summary>Applied to Invoice Amount - DN effect applied to referenced invoice</summary>
        public decimal AppliedToInvoiceAmount { get; set; } = 0;

        // Section 5: Posting Classification (Accounting Mapping)

        /// <summary>FK → GLAccountMaster - AR Receivable Account snapshot at posting</summary>
        public Guid? ReceivableAccountIdSnapshot { get; set; }
        public string? ReceivableAccountCode { get; set; }
        public string? ReceivableAccountName { get; set; }

        /// <summary>FK → GLAccountMaster - Default Revenue/Other Income Account</summary>
        public Guid? RevenueAccountId { get; set; }
        public string? RevenueAccountCode { get; set; }
        public string? RevenueAccountName { get; set; }

        /// <summary>FK → GLAccountMaster - Tax Account snapshot</summary>
        public Guid? TaxAccountIdSnapshot { get; set; }
        public string? TaxAccountCode { get; set; }
        public string? TaxAccountName { get; set; }

        /// <summary>Is System Generated - late fee automation / integration</summary>
        public bool IsSystemGenerated { get; set; } = false;

        // Section 6: Workflow & Status

        /// <summary>Debit Note Status - Draft/Submitted/Approved/Posted/Cancelled/Reversed</summary>
        [Required(ErrorMessage = "Status is required")]
        public string DebitNoteStatus { get; set; } = DebitNoteStatuses.Draft;

        /// <summary>Posted On - datetime</summary>
        public DateTime? PostedOn { get; set; }

        /// <summary>FK → User who posted</summary>
        public Guid? PostedByUserId { get; set; }
        public string? PostedByUserName { get; set; }

        /// <summary>Cancelled On - datetime (pre-post only)</summary>
        public DateTime? CancelledOn { get; set; }

        /// <summary>FK → User who cancelled</summary>
        public Guid? CancelledByUserId { get; set; }
        public string? CancelledByUserName { get; set; }

        /// <summary>Cancellation Reason</summary>
        [StringLength(250, ErrorMessage = "Cancellation Reason cannot exceed 250 characters")]
        public string? CancellationReason { get; set; }

        /// <summary>Reversed On - datetime (if enabled)</summary>
        public DateTime? ReversedOn { get; set; }

        /// <summary>FK → User who reversed</summary>
        public Guid? ReversedByUserId { get; set; }
        public string? ReversedByUserName { get; set; }

        /// <summary>Reversal Reason</summary>
        [StringLength(250, ErrorMessage = "Reversal Reason cannot exceed 250 characters")]
        public string? ReversalReason { get; set; }

        // Section 7: Debit Note Lines (Navigation property)

        /// <summary>Debit Note Lines collection</summary>
        public List<CustomerDebitNoteLineViewModel> Lines { get; set; } = new List<CustomerDebitNoteLineViewModel>();

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
            SubTotalAmount = Lines.Sum(l => l.LineSubTotalAmount);
            DiscountTotalAmount = Lines.Sum(l => l.DiscountAmount);
            TaxTotalAmount = Lines.Sum(l => l.TaxAmount);
            var total = SubTotalAmount - DiscountTotalAmount + TaxTotalAmount;
            RoundOffAmount = Math.Round(total) - total;
            GrandTotalAmount = total + RoundOffAmount;
        }

        /// <summary>Check if debit note can be edited</summary>
        public bool CanEdit => DebitNoteStatus == DebitNoteStatuses.Draft;

        /// <summary>Check if debit note can be posted</summary>
        public bool CanPost => DebitNoteStatus == DebitNoteStatuses.Draft || DebitNoteStatus == DebitNoteStatuses.Approved;

        /// <summary>Check if debit note can be cancelled (pre-post only)</summary>
        public bool CanCancel => DebitNoteStatus == DebitNoteStatuses.Draft || DebitNoteStatus == DebitNoteStatuses.Submitted;

        /// <summary>Check if debit note can be reversed (post-posting only)</summary>
        public bool CanReverse => DebitNoteStatus == DebitNoteStatuses.Posted;
    }

    #endregion

    #region DebitNote-related Enums and Static Classes

    public static class DebitNoteStatuses
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

    public static class DebitReasonCodes
    {
        public const string UnderbillingCorrection = "UnderbillingCorrection";
        public const string AdditionalCharges = "AdditionalCharges";
        public const string LateFee = "LateFee";
        public const string FreightDelivery = "FreightDelivery";
        public const string TaxShortCharged = "TaxShortCharged";
        public const string RateRevision = "RateRevision";
        public const string Other = "Other";
        public static readonly string[] All = new[] { UnderbillingCorrection, AdditionalCharges, LateFee, FreightDelivery, TaxShortCharged, RateRevision, Other };

        public static string GetDisplayName(string code) => code switch
        {
            UnderbillingCorrection => "Underbilling Correction",
            AdditionalCharges => "Additional Charges",
            LateFee => "Late Fee / Penalty",
            FreightDelivery => "Freight / Delivery Charge",
            TaxShortCharged => "Tax Short Charged",
            RateRevision => "Rate Revision",
            Other => "Other",
            _ => code
        };

        public static string GetDescription(string code) => code switch
        {
            UnderbillingCorrection => "Correction for missed items or services in original invoice",
            AdditionalCharges => "Additional charges for services or items not in original billing",
            LateFee => "Late payment fee or penalty charges",
            FreightDelivery => "Freight, shipping, or delivery charges",
            TaxShortCharged => "Correction for tax under-charged in original invoice",
            RateRevision => "Rate revision or price adjustment",
            Other => "Other reason (specify in notes)",
            _ => string.Empty
        };
    }

    #endregion
}
