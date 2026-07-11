using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    #region Model #27: CustomerInvoice

    /// <summary>
    /// Model #27: CustomerInvoice – The AR billing document (header) issued to a customer.
    /// Controls workflow, totals, posting, and compliance.
    /// </summary>
    public class CustomerInvoiceViewModel
    {
        // Section 1: Core Invoice Identity Fields

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

        /// <summary>Invoice Number - max 40 chars, unique per CompanyId + FinancialYear + Series</summary>
        [Required(ErrorMessage = "Invoice Number is required")]
        [StringLength(40, ErrorMessage = "Invoice Number cannot exceed 40 characters")]
        public string InvoiceNumber { get; set; } = string.Empty;

        /// <summary>Invoice Type - Standard/Proforma/Export/SEZ/AdjustmentInvoice</summary>
        [Required(ErrorMessage = "Invoice Type is required")]
        public string InvoiceType { get; set; } = "";

        /// <summary>Invoice Date - date picker</summary>
        [Required(ErrorMessage = "Invoice Date is required")]
        public DateTime InvoiceDate { get; set; } = DateTime.Today;

        /// <summary>Posting Date - conditional required at posting</summary>
        public DateTime? PostingDate { get; set; }

        /// <summary>FK → PaymentTermMaster - dropdown</summary>
        [Required(ErrorMessage = "Payment Terms is required")]
        public Guid PaymentTermId { get; set; }
        public string? PaymentTermName { get; set; }
        public int? PaymentTermDays { get; set; }

        /// <summary>Due Date - derived from InvoiceDate + PaymentTerm.Days</summary>
        [Required(ErrorMessage = "Due Date is required")]
        public DateTime DueDate { get; set; } = DateTime.Today.AddDays(30);

        /// <summary>FK → CurrencyMaster - from Customer.DefaultCurrencyId</summary>
        [Required(ErrorMessage = "Currency is required")]
        public Guid CurrencyId { get; set; }
        public string? CurrencyCode { get; set; }
        public string? CurrencyName { get; set; }

        /// <summary>Exchange Rate - required if foreign currency</summary>
        [Range(0.000001, 9999999999, ErrorMessage = "Exchange Rate must be positive")]
        public decimal ExchangeRate { get; set; } = 1;

        /// <summary>Reference - PO number, contract ref, ticket id, etc.</summary>
        [StringLength(100, ErrorMessage = "Reference cannot exceed 100 characters")]
        public string? ReferenceText { get; set; }

        /// <summary>Notes / Narration - textarea</summary>
        [StringLength(1000, ErrorMessage = "Narration cannot exceed 1000 characters")]
        public string? InvoiceNarration { get; set; }

        // Section 2: Totals (System Controlled - Read-only)

        /// <summary>Subtotal - Sum of line taxable values (before tax)</summary>
        [Required]
        public decimal SubTotalAmount { get; set; } = 0;

        /// <summary>Discount - Sum of line discounts</summary>
        public decimal DiscountTotalAmount { get; set; } = 0;

        /// <summary>Total Tax - Sum of all line tax components</summary>
        public decimal TaxTotalAmount { get; set; } = 0;

        /// <summary>Round Off - derived based on rounding precision</summary>
        public decimal RoundOffAmount { get; set; } = 0;

        /// <summary>Invoice Total = SubTotal - Discount + Tax + RoundOff</summary>
        [Required]
        public decimal GrandTotalAmount { get; set; } = 0;

        /// <summary>Paid - from allocations/payments applied</summary>
        public decimal AmountPaidToDate { get; set; } = 0;

        /// <summary>Outstanding = GrandTotal - AmountPaidToDate - CreditAdjustmentsApplied</summary>
        public decimal AmountOutstanding => GrandTotalAmount - AmountPaidToDate;

        /// <summary>Is Fully Paid - badge (Paid/Partially Paid/Unpaid)</summary>
        public bool IsFullyPaid => AmountOutstanding <= 0;

        // Section 3: Status & Workflow Control

        /// <summary>Invoice Status - Draft/Submitted/Approved/Posted/PartiallyPaid/Paid/Cancelled/Voided</summary>
        [Required(ErrorMessage = "Status is required")]
        public string InvoiceStatus { get; set; } = InvoiceStatuses.Draft;

        /// <summary>Approval Status - NotRequired/Pending/Approved/Rejected</summary>
        public string ApprovalStatus { get; set; } = ApprovalStatuses.NotRequired;

        /// <summary>Posted On - datetime</summary>
        public DateTime? PostedOn { get; set; }

        /// <summary>FK → User who posted</summary>
        public Guid? PostedByUserId { get; set; }
        public string? PostedByUserName { get; set; }

        /// <summary>Cancelled On - datetime</summary>
        public DateTime? CancelledOn { get; set; }

        /// <summary>FK → User who cancelled</summary>
        public Guid? CancelledByUserId { get; set; }
        public string? CancelledByUserName { get; set; }

        /// <summary>Cancellation Reason - required if Cancelled</summary>
        [StringLength(250, ErrorMessage = "Cancellation Reason cannot exceed 250 characters")]
        public string? CancellationReason { get; set; }

        // Section 4: Posting Classification (Accounting)

        /// <summary>FK → GLAccountMaster - AR Receivable Account snapshot at posting</summary>
        public Guid? ReceivableAccountId { get; set; }
        public string? ReceivableAccountCode { get; set; }
        public string? ReceivableAccountName { get; set; }

        /// <summary>FK → PostingProfile - dropdown if using posting engine rules</summary>
        public Guid? RevenuePostingProfileId { get; set; }
        public string? RevenuePostingProfileName { get; set; }

        /// <summary>FK → DocumentSequence - hidden/read-only</summary>
        public Guid? DocumentSequenceId { get; set; }

        /// <summary>Is System Generated - invoice created from automation/subscription</summary>
        public bool IsSystemGenerated { get; set; } = false;

        // Section 5: Compliance (India-ready, future-ready)

        /// <summary>FK → State - Place of Supply</summary>
        public Guid? PlaceOfSupplyStateId { get; set; }
        public string? PlaceOfSupplyStateName { get; set; }
        public string? PlaceOfSupplyStateCode { get; set; }

        /// <summary>Supply Type - IntraState/InterState/Export/SEZ</summary>
        public string SupplyType { get; set; } = "";

        /// <summary>E-Invoice Status - NotApplicable/Pending/Generated/Failed</summary>
        public string EInvoiceStatus { get; set; } = EInvoiceStatuses.NotApplicable;

        /// <summary>E-Invoice IRN - max 100 chars</summary>
        [StringLength(100, ErrorMessage = "E-Invoice IRN cannot exceed 100 characters")]
        public string? EInvoiceIRN { get; set; }

        /// <summary>E-Invoice Acknowledgement Number</summary>
        [StringLength(50)]
        public string? EInvoiceAckNo { get; set; }

        /// <summary>E-Invoice Acknowledgement Date</summary>
        public DateTime? EInvoiceAckDate { get; set; }

        /// <summary>QR Code Data for invoice</summary>
        [StringLength(2000)]
        public string? QRCodeData { get; set; }

        // Section 6: Invoice Lines (Navigation property)

        /// <summary>Invoice Lines collection</summary>
        public List<CustomerInvoiceLineViewModel> Lines { get; set; } = new List<CustomerInvoiceLineViewModel>();

        // Section 7: System Audit Fields

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

        /// <summary>Calculate Due Date from Invoice Date and Payment Terms</summary>
        public void CalculateDueDate()
        {
            if (PaymentTermDays.HasValue)
            {
                DueDate = InvoiceDate.AddDays(PaymentTermDays.Value);
            }
        }

        /// <summary>Check if invoice can be edited</summary>
        public bool CanEdit => InvoiceStatus == InvoiceStatuses.Draft;

        /// <summary>Check if invoice can be posted</summary>
        public bool CanPost => InvoiceStatus == InvoiceStatuses.Draft || InvoiceStatus == InvoiceStatuses.Approved;

        /// <summary>Check if invoice can be cancelled</summary>
        public bool CanCancel => InvoiceStatus == InvoiceStatuses.Draft || InvoiceStatus == InvoiceStatuses.Submitted;
    }

    #endregion

    #region Invoice-related Enums and Static Classes

    public static class InvoiceTypes
    {
        public const string Standard = "Standard";
        public const string Proforma = "Proforma";
        public const string Export = "Export";
        public const string SEZ = "SEZ";
        public const string AdjustmentInvoice = "AdjustmentInvoice";
        public static readonly string[] All = new[] { Standard, Proforma, Export, SEZ, AdjustmentInvoice };

        public static string GetDisplayName(string type) => type switch
        {
            Standard => "Standard Invoice",
            Proforma => "Proforma Invoice",
            Export => "Export Invoice",
            SEZ => "SEZ Invoice",
            AdjustmentInvoice => "Adjustment Invoice",
            _ => type
        };
    }

    public static class InvoiceStatuses
    {
        public const string Draft = "Draft";
        public const string Submitted = "Submitted";
        public const string Approved = "Approved";
        public const string Posted = "Posted";
        public const string PartiallyPaid = "PartiallyPaid";
        public const string Paid = "Paid";
        public const string Cancelled = "Cancelled";
        public const string Voided = "Voided";
        public static readonly string[] All = new[] { Draft, Submitted, Approved, Posted, PartiallyPaid, Paid, Cancelled, Voided };

        public static string GetDisplayName(string status) => status switch
        {
            Draft => "Draft",
            Submitted => "Submitted",
            Approved => "Approved",
            Posted => "Posted",
            PartiallyPaid => "Partially Paid",
            Paid => "Paid",
            Cancelled => "Cancelled",
            Voided => "Voided",
            _ => status
        };
    }

    public static class ApprovalStatuses
    {
        public const string NotRequired = "NotRequired";
        public const string Pending = "Pending";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
        public static readonly string[] All = new[] { NotRequired, Pending, Approved, Rejected };
    }

    public static class SupplyTypes
    {
        public const string IntraState = "IntraState";
        public const string InterState = "InterState";
        public const string Export = "Export";
        public const string SEZ = "SEZ";
        public static readonly string[] All = new[] { IntraState, InterState, Export, SEZ };

        public static string GetDisplayName(string type) => type switch
        {
            IntraState => "Intra-State",
            InterState => "Inter-State",
            Export => "Export",
            SEZ => "SEZ",
            _ => type
        };
    }

    public static class EInvoiceStatuses
    {
        public const string NotApplicable = "NotApplicable";
        public const string Pending = "Pending";
        public const string Generated = "Generated";
        public const string Failed = "Failed";
        public static readonly string[] All = new[] { NotApplicable, Pending, Generated, Failed };
    }

    #endregion
}
