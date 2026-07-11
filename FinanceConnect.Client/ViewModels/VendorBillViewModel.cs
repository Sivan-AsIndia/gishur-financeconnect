using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    #region Model #36: VendorBill

    /// <summary>
    /// Model #36: VendorBill – The AP billing document (header) received from a vendor.
    /// Controls workflow, totals, posting, and compliance.
    /// </summary>
    public class VendorBillViewModel
    {
        // Section 1: Core Bill Identity Fields

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

        /// <summary>FK → Vendor - Search dropdown (typeahead)</summary>
        [Required(ErrorMessage = "Vendor is required")]
        public Guid ? VendorId { get; set; }
        public string? VendorCode { get; set; }
        public string? VendorName { get; set; }

        /// <summary>FK → VendorAccount - Read-only (system derived)</summary>
        public Guid? VendorAccountId { get; set; }
        public string? VendorAccountName { get; set; }

        /// <summary>Bill Number (System) - max 40 chars, unique per CompanyId + FinancialYear + Series</summary>
        [Required(ErrorMessage = "Bill Number is required")]
        [StringLength(40, ErrorMessage = "Bill Number cannot exceed 40 characters")]
        public string BillNumber { get; set; } = string.Empty;

        /// <summary>Vendor Invoice Number (Critical Anti-Fraud) - max 50 chars, unique per vendor per company</summary>
        [Required(ErrorMessage = "Vendor Invoice Number is required")]
        [StringLength(50, ErrorMessage = "Vendor Invoice Number cannot exceed 50 characters")]
        public string VendorInvoiceNumber { get; set; } = string.Empty;

        /// <summary>Vendor Invoice Date - date picker</summary>
        [Required(ErrorMessage = "Vendor Invoice Date is required")]
        public DateTime VendorInvoiceDate { get; set; } = DateTime.Today;

        /// <summary>Bill Entry Date - date picker</summary>
        [Required(ErrorMessage = "Bill Date is required")]
        public DateTime BillDate { get; set; } = DateTime.Today;

        /// <summary>Due Date - derived from VendorInvoiceDate + PaymentTerm.Days</summary>
        [Required(ErrorMessage = "Due Date is required")]
        public DateTime DueDate { get; set; } = DateTime.Today.AddDays(30);

        /// <summary>Posting Date - conditional required at posting</summary>
        public DateTime? PostingDate { get; set; }

        /// <summary>Bill Type - GoodsPurchase/ServiceExpense/Utility/Rent/Contractor/Other</summary>
        [Required(ErrorMessage = "Bill Type is required")]
        public string BillType { get; set; } = "";

        /// <summary>FK → PaymentTermMaster - dropdown</summary>
        [Required(ErrorMessage = "Payment Terms is required")]
        public Guid PaymentTermId { get; set; }
        public string? PaymentTermName { get; set; }
        public int? PaymentTermDays { get; set; }

        /// <summary>FK → CurrencyMaster - from Vendor.DefaultCurrencyId</summary>
        [Required(ErrorMessage = "Currency is required")]
        public Guid CurrencyId { get; set; }
        public string? CurrencyCode { get; set; }
        public string? CurrencyName { get; set; }

        /// <summary>Exchange Rate - required if foreign currency</summary>
        [Range(0.000001, 9999999999, ErrorMessage = "Exchange Rate must be positive")]
        public decimal ExchangeRate { get; set; } = 1;

        /// <summary>Notes / Narration - textarea</summary>
        [StringLength(1000, ErrorMessage = "Narration cannot exceed 1000 characters")]
        public string? BillNarration { get; set; }

        // Section 2: Bill Classification

        /// <summary>Source Reference Type - PO/GRN/Contract/None (future)</summary>
        public string SourceReferenceType { get; set; } = "";

        /// <summary>Source Reference Id - FK to PO/GRN/Contract (future)</summary>
        public Guid? SourceReferenceId { get; set; }

        /// <summary>Is Recurring Bill - toggle (optional)</summary>
        public bool IsRecurringBill { get; set; } = false;

        // Section 3: Totals (System Controlled - Read-only)

        /// <summary>Subtotal - Sum of line taxable values (before tax)</summary>
        [Required]
        public decimal SubTotalAmount { get; set; } = 0;

        /// <summary>Discount - Sum of line discounts</summary>
        public decimal DiscountTotalAmount { get; set; } = 0;

        /// <summary>Total Tax - Sum of all line tax components</summary>
        public decimal TaxTotalAmount { get; set; } = 0;

        /// <summary>Round Off - derived based on rounding precision</summary>
        public decimal RoundOffAmount { get; set; } = 0;

        /// <summary>Bill Total = SubTotal - Discount + Tax + RoundOff</summary>
        [Required]
        public decimal GrandTotalAmount { get; set; } = 0;

        /// <summary>Paid - from allocations/payments applied</summary>
        public decimal PaidAmount { get; set; } = 0;

        /// <summary>Outstanding = GrandTotal - PaidAmount - AppliedCredits - AppliedAdjustments</summary>
        public decimal OutstandingAmount => GrandTotalAmount - PaidAmount;

        /// <summary>Alias for OutstandingAmount (for compatibility)</summary>
        public decimal AmountOutstanding => OutstandingAmount;

        /// <summary>Alias for GrandTotalAmount (for compatibility)</summary>
        public decimal TotalAmount => GrandTotalAmount;

        /// <summary>Settlement Status - Unpaid/PartiallyPaid/Paid (derived)</summary>
        public string SettlementStatus => OutstandingAmount <= 0 ? SettlementStatuses.Paid 
            : (PaidAmount > 0 ? SettlementStatuses.PartiallyPaid : SettlementStatuses.Unpaid);

        // Section 4: Tax & Withholding (India-ready)

        /// <summary>Is GST Applicable - toggle</summary>
        public bool IsGSTApplicable { get; set; } = true;

        /// <summary>Is Reverse Charge Applicable (RCM) - toggle</summary>
        public bool IsReverseChargeApplicable { get; set; } = false;

        /// <summary>Vendor GSTIN Snapshot - max 15 chars, captured at posting</summary>
        [StringLength(15, ErrorMessage = "GSTIN cannot exceed 15 characters")]
        public string? VendorGSTINSnapshot { get; set; }

        /// <summary>FK → State - Place of Supply</summary>
        public Guid? PlaceOfSupplyStateId { get; set; }
        public string? PlaceOfSupplyStateName { get; set; }
        public string? PlaceOfSupplyStateCode { get; set; }

        /// <summary>Is TDS Applicable - toggle</summary>
        public bool IsTDSApplicable { get; set; } = false;

        /// <summary>TDS Section Code Snapshot - max 20 chars</summary>
        [StringLength(20)]
        public string? TDSSectionCodeSnapshot { get; set; }

        /// <summary>TDS Rate Percent Snapshot</summary>
        public decimal? TDSRatePercentSnapshot { get; set; }

        /// <summary>TDS Base Amount - derived</summary>
        public decimal TDSBaseAmount { get; set; } = 0;

        /// <summary>TDS Amount - derived</summary>
        public decimal TDSAmount { get; set; } = 0;

        // Section 5: Posting Classification (Accounting)

        /// <summary>FK → GLAccountMaster - AP Payable Account snapshot at posting</summary>
        public Guid? PayableAccountIdSnapshot { get; set; }
        public string? PayableAccountCode { get; set; }
        public string? PayableAccountName { get; set; }

        /// <summary>Input Tax Account Mapping Snapshot - for audit trail</summary>
        public string? InputTaxAccountMappingSnapshot { get; set; }

        /// <summary>Is System Generated - bill created from automation</summary>
        public bool IsSystemGenerated { get; set; } = false;

        // Section 6: Workflow & Status

        /// <summary>Bill Status - Draft/Submitted/Approved/Rejected/Posted/Cancelled/Reversed</summary>
        [Required(ErrorMessage = "Status is required")]
        public string BillStatus { get; set; } = VendorBillStatuses.Draft;

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

        // Section 7: Attachments & Evidence

        /// <summary>Has Invoice Attachment - badge</summary>
        public bool HasInvoiceAttachment { get; set; } = false;

        /// <summary>Invoice Attachment Count</summary>
        public int InvoiceAttachmentCount { get; set; } = 0;

        // Section 8: Bill Lines (Navigation property)

        /// <summary>Bill Lines collection</summary>
        public List<VendorBillLineViewModel> Lines { get; set; } = new List<VendorBillLineViewModel>();

        // Section 9: System Audit Fields

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
            SubTotalAmount = Lines.Sum(l => l.TaxableAmount);
            DiscountTotalAmount = Lines.Sum(l => l.DiscountAmount);
            TaxTotalAmount = Lines.Sum(l => l.LineTaxAmount);
            var total = SubTotalAmount + TaxTotalAmount;
            RoundOffAmount = Math.Round(total) - total;
            GrandTotalAmount = total + RoundOffAmount;
        }

        /// <summary>Calculate Due Date from Vendor Invoice Date and Payment Terms</summary>
        public void CalculateDueDate()
        {
            if (PaymentTermDays.HasValue)
            {
                DueDate = VendorInvoiceDate.AddDays(PaymentTermDays.Value);
            }
        }

        /// <summary>Check if bill can be edited</summary>
        public bool CanEdit => BillStatus == VendorBillStatuses.Draft;

        /// <summary>Check if bill can be posted</summary>
        public bool CanPost => BillStatus == VendorBillStatuses.Draft || BillStatus == VendorBillStatuses.Approved;

        /// <summary>Check if bill can be cancelled</summary>
        public bool CanCancel => BillStatus == VendorBillStatuses.Draft || BillStatus == VendorBillStatuses.Submitted;
    }

    #endregion

    #region VendorBill-related Enums and Static Classes

    public static class BillTypes
    {
        public const string GoodsPurchase = "GoodsPurchase";
        public const string ServiceExpense = "ServiceExpense";
        public const string Utility = "Utility";
        public const string Rent = "Rent";
        public const string Contractor = "Contractor";
        public const string Other = "Other";
        public static readonly string[] All = new[] { GoodsPurchase, ServiceExpense, Utility, Rent, Contractor, Other };

        public static string GetDisplayName(string type) => type switch
        {
            GoodsPurchase => "Goods Purchase",
            ServiceExpense => "Service/Expense",
            Utility => "Utility",
            Rent => "Rent",
            Contractor => "Contractor",
            Other => "Other",
            _ => type
        };
    }

    public static class VendorBillStatuses
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

    public static class SettlementStatuses
    {
        public const string Unpaid = "Unpaid";
        public const string PartiallyPaid = "PartiallyPaid";
        public const string Paid = "Paid";
        public static readonly string[] All = new[] { Unpaid, PartiallyPaid, Paid };

        public static string GetDisplayName(string status) => status switch
        {
            Unpaid => "Unpaid",
            PartiallyPaid => "Partially Paid",
            Paid => "Paid",
            _ => status
        };
    }

    public static class SourceReferenceTypes
    {
        public const string None = "None";
        public const string PurchaseOrder = "PurchaseOrder";
        public const string GRN = "GRN";
        public const string Contract = "Contract";
        public static readonly string[] All = new[] { None, PurchaseOrder, GRN, Contract };
    }

    #endregion
}
