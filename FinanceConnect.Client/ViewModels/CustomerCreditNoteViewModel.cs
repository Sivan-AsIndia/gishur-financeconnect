using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    #region Model #30: CustomerCreditNote

    /// <summary>
    /// Model #30: CustomerCreditNote – AR reduction document issued to a customer to reduce what they owe.
    /// Used for: returns, pricing corrections, discounts after invoice, damaged items, dispute settlement, tax corrections.
    /// </summary>
    public class CustomerCreditNoteViewModel
    {
        // Section 1: Core Credit Note Identity Fields (Header)

        /// <summary>PK - hidden in UI</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>FK → Company - hidden</summary>
        [Required(ErrorMessage = "Company is required")]
        public Guid CompanyId { get; set; }
        public string? CompanyName { get; set; }

        /// <summary>Tenant identifier - hidden in UI</summary>
        public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

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

        /// <summary>FK → CustomerAccount - Read-only (system derived from CompanyId + CustomerId + CurrencyId)</summary>
        public Guid? CustomerAccountId { get; set; }
        public string? CustomerAccountName { get; set; }

        /// <summary>Credit Note Number - max 40 chars, unique per CompanyId + FinancialYear</summary>
        [Required(ErrorMessage = "Credit Note Number is required")]
        [StringLength(40, ErrorMessage = "Credit Note Number cannot exceed 40 characters")]
        public string CreditNoteNumber { get; set; } = string.Empty;

        /// <summary>Credit Note Date - date picker</summary>
        [Required(ErrorMessage = "Credit Note Date is required")]
        public DateTime CreditNoteDate { get; set; } = DateTime.Today;

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

        /// <summary>Reference - PO number, dispute ticket id, etc.</summary>
        [StringLength(100, ErrorMessage = "Reference cannot exceed 100 characters")]
        public string? ReferenceText { get; set; }

        /// <summary>Notes / Narration - textarea</summary>
        [StringLength(1000, ErrorMessage = "Narration cannot exceed 1000 characters")]
        public string? CreditNoteNarration { get; set; }

        // Section 2: Reference (Linking to Invoice)

        /// <summary>Is Against Invoice - toggle, default true (recommended)</summary>
        public bool IsAgainstInvoice { get; set; } = true;

        /// <summary>FK → CustomerInvoice - dropdown/search filtered by CustomerId and Posted status</summary>
        public Guid? CustomerInvoiceId { get; set; }
        public string? CustomerInvoiceNumber { get; set; }

        /// <summary>Invoice Number Snapshot - preserve reference</summary>
        [StringLength(40)]
        public string? InvoiceNumberSnapshot { get; set; }

        /// <summary>Invoice Date Snapshot - preserve reference</summary>
        public DateTime? InvoiceDateSnapshot { get; set; }

        // Section 3: Credit Reason & Classification (Mandatory)

        /// <summary>Reason Code - dropdown, required</summary>
        [Required(ErrorMessage = "Credit Reason Code is required")]
        public string CreditReasonCode { get; set; } = CreditReasonCodes.Other;

        /// <summary>Credit Reason Description - auto-filled from reason master, editable optional</summary>
        [StringLength(250, ErrorMessage = "Credit Reason Description cannot exceed 250 characters")]
        public string? CreditReasonDescription { get; set; }

        /// <summary>Is Tax Impacting - toggle, default true</summary>
        public bool IsTaxImpacting { get; set; } = true;

        /// <summary>Is Revenue Reversal - toggle, default true. Determines whether CN reverses revenue or posts to allowance account</summary>
        public bool IsRevenueReversal { get; set; } = true;

        // Section 4: Totals (System Controlled - Read-only)

        /// <summary>Subtotal - Sum of line values</summary>
        [Required]
        public decimal SubTotalAmount { get; set; } = 0;

        /// <summary>Discount - Sum of line discounts</summary>
        public decimal DiscountTotalAmount { get; set; } = 0;

        /// <summary>Total Tax - Sum of all line tax components</summary>
        public decimal TaxTotalAmount { get; set; } = 0;

        /// <summary>Round Off - derived based on rounding precision</summary>
        public decimal RoundOffAmount { get; set; } = 0;

        /// <summary>Credit Note Total = SubTotal - Discount + Tax + RoundOff</summary>
        [Required]
        public decimal GrandTotalAmount { get; set; } = 0;

        /// <summary>Applied to Invoice Amount - how much CN applied to invoice outstanding (if linked)</summary>
        public decimal AppliedToInvoiceAmount { get; set; } = 0;

        // Section 5: Posting Classification (Accounting Mapping)

        /// <summary>FK → GLAccountMaster - AR Receivable Account snapshot at posting</summary>
        public Guid? ReceivableAccountIdSnapshot { get; set; }
        public string? ReceivableAccountCode { get; set; }
        public string? ReceivableAccountName { get; set; }

        /// <summary>FK → GLAccountMaster - Revenue Reversal / Sales Return Account</summary>
        public Guid? RevenueReversalAccountId { get; set; }
        public string? RevenueReversalAccountCode { get; set; }
        public string? RevenueReversalAccountName { get; set; }

        /// <summary>FK → GLAccountMaster - Tax reversal mapping</summary>
        public Guid? TaxAccountIdSnapshot { get; set; }
        public string? TaxAccountCode { get; set; }
        public string? TaxAccountName { get; set; }

        /// <summary>Is System Generated - credit note created from automation</summary>
        public bool IsSystemGenerated { get; set; } = false;

        // Section 6: Workflow & Status

        /// <summary>Credit Note Status - Draft/Submitted/Approved/Posted/Cancelled/Reversed</summary>
        [Required(ErrorMessage = "Status is required")]
        public string CreditNoteStatus { get; set; } = CreditNoteStatuses.Draft;

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

        /// <summary>Cancelled On - datetime (pre-post only)</summary>
        public DateTime? CancelledOn { get; set; }

        /// <summary>FK → User who cancelled</summary>
        public Guid? CancelledByUserId { get; set; }
        public string? CancelledByUserName { get; set; }

        /// <summary>Cancellation Reason - required if Cancelled</summary>
        [StringLength(250, ErrorMessage = "Cancellation Reason cannot exceed 250 characters")]
        public string? CancellationReason { get; set; }

        // Section 7: Credit Note Lines (Navigation property)

        /// <summary>Credit Note Lines collection</summary>
        public List<CustomerCreditNoteLineModel> Lines { get; set; } = new List<CustomerCreditNoteLineModel>();

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

        /// <summary>Check if credit note can be edited</summary>
        public bool CanEdit => CreditNoteStatus == CreditNoteStatuses.Draft;

        /// <summary>Check if credit note can be posted</summary>
        public bool CanPost => CreditNoteStatus == CreditNoteStatuses.Draft || CreditNoteStatus == CreditNoteStatuses.Approved;

        /// <summary>Check if credit note can be cancelled</summary>
        public bool CanCancel => CreditNoteStatus == CreditNoteStatuses.Draft || CreditNoteStatus == CreditNoteStatuses.Submitted;

        // Alias properties for UI convenience
        public string Status => CreditNoteStatus;
        public decimal SubTotal => SubTotalAmount;
        public decimal DiscountTotal => DiscountTotalAmount;
        public decimal TaxTotal => TaxTotalAmount;
        public decimal GrandTotal => GrandTotalAmount;
        public string? Narration => CreditNoteNarration;
        public string? CustomerAccountCode => CustomerAccountId?.ToString().Substring(0, 8); // Simplified
        public DateTime CreatedDate => CreatedAt;
        public DateTime? ModifiedDate => UpdatedAt;
        public string? ModifiedBy => UpdatedBy;
        public DateTime? ApprovedDate => ApprovedOn;
        public string? ApprovedBy => ApprovedByUserName;
        public DateTime? PostedDate => PostedOn;
        public string? PostedBy => PostedByUserName;
    }

    #endregion

    #region CreditNote-related Enums and Static Classes

    public static class CreditNoteStatuses
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

    public static class CreditReasonCodes
    {
        public const string SalesReturn = "SalesReturn";
        public const string PriceCorrection = "PriceCorrection";
        public const string DiscountAfterInvoice = "DiscountAfterInvoice";
        public const string ServiceCancellation = "ServiceCancellation";
        public const string DamageDefect = "DamageDefect";
        public const string TaxCorrection = "TaxCorrection";
        public const string WriteOffSettlement = "WriteOffSettlement";
        public const string Other = "Other";
        public static readonly string[] All = new[] { SalesReturn, PriceCorrection, DiscountAfterInvoice, ServiceCancellation, DamageDefect, TaxCorrection, WriteOffSettlement, Other };

        public static string GetDisplayName(string code) => code switch
        {
            SalesReturn => "Sales Return",
            PriceCorrection => "Price Correction",
            DiscountAfterInvoice => "Discount After Invoice",
            ServiceCancellation => "Service Cancellation",
            DamageDefect => "Damage / Defect",
            TaxCorrection => "Tax Correction",
            WriteOffSettlement => "Write-off Settlement",
            Other => "Other",
            _ => code
        };
    }

    #endregion
}
