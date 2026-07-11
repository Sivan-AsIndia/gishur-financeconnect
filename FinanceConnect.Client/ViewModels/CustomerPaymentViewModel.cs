using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    #region Model #29: CustomerPayment

    /// <summary>
    /// Model #29: CustomerPayment – AR receipt/payment document (header) representing money received from a customer
    /// via Cash/Bank/UPI/NEFT/Cheque/Card/Gateway.
    /// If CustomerInvoice = "money customer owes us", then CustomerPayment = "money customer paid us"
    /// </summary>
    public class CustomerPaymentViewModel
    {
        // Section 1: Core Payment Identity Fields (Header)

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

        /// <summary>Receipt Number - max 40 chars, unique per CompanyId + FinancialYear</summary>
        [Required(ErrorMessage = "Receipt Number is required")]
        [StringLength(40, ErrorMessage = "Receipt Number cannot exceed 40 characters")]
        public string ReceiptNumber { get; set; } = string.Empty;

        /// <summary>Receipt Date - date picker</summary>
        [Required(ErrorMessage = "Receipt Date is required")]
        public DateTime ReceiptDate { get; set; } = DateTime.Today;

        /// <summary>Posting Date - conditional required at posting</summary>
        public DateTime? PostingDate { get; set; }

        /// <summary>FK → CurrencyMaster - from Customer.DefaultCurrencyId or multi-currency dropdown</summary>
        [Required(ErrorMessage = "Currency is required")]
        public Guid CurrencyId { get; set; }
        public string? CurrencyCode { get; set; }
        public string? CurrencyName { get; set; }

        /// <summary>Exchange Rate - required if foreign currency (decimal 18,6)</summary>
        [Range(0.000001, 9999999999, ErrorMessage = "Exchange Rate must be positive")]
        public decimal ExchangeRate { get; set; } = 1;

        /// <summary>Notes / Narration - textarea (max 1000)</summary>
        [StringLength(1000, ErrorMessage = "Narration cannot exceed 1000 characters")]
        public string? PaymentNarration { get; set; }

        // Section 2: Payment Instrument Details

        /// <summary>Payment Method - dropdown</summary>
        [Required(ErrorMessage = "Payment Method is required")]
        public string PaymentMethod { get; set; } = PaymentMethods.Cash;

        /// <summary>FK → GLAccountMaster - Deposit To (Cash/Bank Account)</summary>
        [Required(ErrorMessage = "Payment Account is required")]
        public Guid PaymentAccountId { get; set; }
        public string? PaymentAccountCode { get; set; }
        public string? PaymentAccountName { get; set; }

        /// <summary>Instrument Date - date picker (nullable, required for Cheque/Transfer)</summary>
        public DateTime? InstrumentDate { get; set; }

        /// <summary>Cheque No / UTR / Ref No - max 50 chars</summary>
        [StringLength(50, ErrorMessage = "Instrument Number cannot exceed 50 characters")]
        public string? InstrumentNumber { get; set; }

        /// <summary>Bank Name - max 100 chars (required for cheque)</summary>
        [StringLength(100, ErrorMessage = "Bank Name cannot exceed 100 characters")]
        public string? BankName { get; set; }

        /// <summary>Bank Account Last 4 digits - max 4 chars</summary>
        [StringLength(4, ErrorMessage = "Bank Account Last 4 cannot exceed 4 characters")]
        public string? BankAccountLast4 { get; set; }

        /// <summary>Payer Name - max 150 chars (if payment done by different party)</summary>
        [StringLength(150, ErrorMessage = "Payer Name cannot exceed 150 characters")]
        public string? PayerName { get; set; }

        /// <summary>Gateway Provider - enum/string (Razorpay/Stripe/etc.) - Future-ready</summary>
        [StringLength(50, ErrorMessage = "Gateway Provider cannot exceed 50 characters")]
        public string? GatewayProvider { get; set; }

        /// <summary>Gateway Transaction Id - max 100 chars (idempotency + reconciliation) - Future-ready</summary>
        [StringLength(100, ErrorMessage = "Gateway Transaction Id cannot exceed 100 characters")]
        public string? GatewayTransactionId { get; set; }

        /// <summary>Attachment Count / Has Attachments - badge (proof uploaded)</summary>
        public int AttachmentCount { get; set; } = 0;
        public bool HasAttachments => AttachmentCount > 0;

        // Section 3: Amount Fields (System Controlled Splits)

        /// <summary>Amount Received - numeric input, must be > 0</summary>
        [Required(ErrorMessage = "Payment Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Payment Amount must be greater than zero")]
        public decimal PaymentAmountTotal { get; set; } = 0;

        /// <summary>Allocated Amount - Sum of allocation rows (read-only)</summary>
        public decimal AllocatedAmountTotal { get; set; } = 0;

        /// <summary>On Account / Unapplied Amount - Total - Allocated - Advance (read-only)</summary>
        public decimal UnallocatedAmountTotal { get; set; } = 0;

        /// <summary>Advance Amount - read-only or editable if user explicitly marks advance</summary>
        public decimal AdvanceAmountTotal { get; set; } = 0;

        // Section 4: Allocation Grid (Child rows under CustomerPayment)

        /// <summary>Payment Allocations collection</summary>
        public List<CustomerPaymentAllocationViewModel> Allocations { get; set; } = new List<CustomerPaymentAllocationViewModel>();

        // Section 5: Workflow & Status

        /// <summary>Payment Status - Draft/Submitted/Approved/Posted/Reversed/Cancelled</summary>
        [Required(ErrorMessage = "Status is required")]
        public string PaymentStatus { get; set; } = PaymentStatuses.Draft;

        /// <summary>Submitted On - datetime</summary>
        public DateTime? SubmittedOn { get; set; }

        /// <summary>FK → User who submitted</summary>
        public Guid? SubmittedByUserId { get; set; }
        public string? SubmittedBy { get; set; }

        /// <summary>Approved On - datetime</summary>
        public DateTime? ApprovedOn { get; set; }

        /// <summary>FK → User who approved</summary>
        public Guid? ApprovedByUserId { get; set; }
        public string? ApprovedBy { get; set; }

        /// <summary>Posted On - datetime</summary>
        public DateTime? PostedOn { get; set; }

        /// <summary>FK → User who posted</summary>
        public Guid? PostedByUserId { get; set; }
        public string? PostedBy { get; set; }

        /// <summary>Reversed On - datetime</summary>
        public DateTime? ReversedOn { get; set; }

        /// <summary>FK → User who reversed</summary>
        public Guid? ReversedByUserId { get; set; }
        public string? ReversedBy { get; set; }

        /// <summary>Reversal Reason - required if Reversed (max 250)</summary>
        [StringLength(250, ErrorMessage = "Reversal Reason cannot exceed 250 characters")]
        public string? ReversalReason { get; set; }

        /// <summary>Reversal Reference Document No - bank reversal ref / bounce memo id (max 50)</summary>
        [StringLength(50, ErrorMessage = "Reversal Reference cannot exceed 50 characters")]
        public string? ReversalReference { get; set; }

        /// <summary>Cancelled On - datetime (pre-post only)</summary>
        public DateTime? CancelledOn { get; set; }

        /// <summary>FK → User who cancelled</summary>
        public Guid? CancelledByUserId { get; set; }
        public string? CancelledBy { get; set; }

        /// <summary>Cancellation Reason - required if Cancelled</summary>
        [StringLength(250, ErrorMessage = "Cancellation Reason cannot exceed 250 characters")]
        public string? CancellationReason { get; set; }

        // Section 6: Posting Classification (Accounting)

        /// <summary>FK → GLAccountMaster - AR Receivable Account snapshot at posting</summary>
        public Guid? ReceivableAccountIdSnapshot { get; set; }
        public string? ReceivableAccountCode { get; set; }
        public string? ReceivableAccountName { get; set; }

        /// <summary>FK → GLAccountMaster - Advance From Customer Account snapshot at posting</summary>
        public Guid? AdvanceFromCustomerAccountIdSnapshot { get; set; }
        public string? AdvanceFromCustomerAccountCode { get; set; }
        public string? AdvanceFromCustomerAccountName { get; set; }

        /// <summary>Is System Generated - integration created</summary>
        public bool IsSystemGenerated { get; set; } = false;

        /// <summary>Is Reconciled - bank reconciliation (Future)</summary>
        public bool IsReconciled { get; set; } = false;

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

        /// <summary>Recalculate amount splits from allocations</summary>
        public void RecalculateAmounts()
        {
            AllocatedAmountTotal = Allocations.Sum(a => a.AllocatedAmount);
            // Unallocated = Total - Allocated - Advance
            UnallocatedAmountTotal = PaymentAmountTotal - AllocatedAmountTotal - AdvanceAmountTotal;
            if (UnallocatedAmountTotal < 0)
            {
                UnallocatedAmountTotal = 0;
            }
        }

        /// <summary>Check if payment can be edited</summary>
        public bool CanEdit => PaymentStatus == PaymentStatuses.Draft;

        /// <summary>Check if payment can be posted</summary>
        public bool CanPost => PaymentStatus == PaymentStatuses.Draft || PaymentStatus == PaymentStatuses.Approved;

        /// <summary>Check if payment can be cancelled (pre-post only)</summary>
        public bool CanCancel => PaymentStatus == PaymentStatuses.Draft || PaymentStatus == PaymentStatuses.Submitted;

        /// <summary>Check if payment can be reversed (post-only)</summary>
        public bool CanReverse => PaymentStatus == PaymentStatuses.Posted;

        // Alias properties for UI convenience
        public string Status => PaymentStatus;
        public decimal TotalAmount => PaymentAmountTotal;
        public decimal AllocatedAmount => AllocatedAmountTotal;
        public decimal UnallocatedAmount => UnallocatedAmountTotal;
        public decimal AdvanceAmount => AdvanceAmountTotal;
        public string? Narration => PaymentNarration;
        public DateTime CreatedDate => CreatedAt;
        public DateTime? ModifiedDate => UpdatedAt;
        public string? ModifiedBy => UpdatedBy;
        public DateTime? PostedDate => PostedOn;
        public DateTime? ReversedDate => ReversedOn;
    }

    #endregion

    #region CustomerPaymentAllocation (Child rows)

    /// <summary>
    /// CustomerPaymentAllocation - Allocation of payment to invoices
    /// </summary>
    public class CustomerPaymentAllocationViewModel
    {
        /// <summary>PK - Allocation Id</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>FK → CustomerPayment</summary>
        [Required]
        public Guid CustomerPaymentId { get; set; }

        /// <summary>FK → CustomerInvoice</summary>
        [Required(ErrorMessage = "Invoice is required")]
        public Guid CustomerInvoiceId { get; set; }
        public string? InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal InvoiceOutstanding { get; set; } = 0;

        /// <summary>Amount allocated to this invoice</summary>
        [Required(ErrorMessage = "Allocated Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Allocated Amount must be greater than zero")]
        public decimal AllocatedAmount { get; set; } = 0;

        // Audit Fields
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }

    #endregion

    #region Payment-related Enums and Static Classes

    /// <summary>Payment Status values</summary>
    public static class PaymentStatuses
    {
        public const string Draft = "Draft";
        public const string Submitted = "Submitted";
        public const string Approved = "Approved";
        public const string Posted = "Posted";
        public const string Reversed = "Reversed";
        public const string Cancelled = "Cancelled";
        public static readonly string[] All = new[] { Draft, Submitted, Approved, Posted, Reversed, Cancelled };

        public static string GetDisplayName(string status) => status switch
        {
            Draft => "Draft",
            Submitted => "Submitted",
            Approved => "Approved",
            Posted => "Posted",
            Reversed => "Reversed",
            Cancelled => "Cancelled",
            _ => status
        };
    }

    /// <summary>Gateway Provider values (Future-ready)</summary>
    public static class GatewayProviders
    {
        public const string Razorpay = "Razorpay";
        public const string Stripe = "Stripe";
        public const string PayU = "PayU";
        public const string CCAvenue = "CCAvenue";
        public const string PayPal = "PayPal";
        public const string Other = "Other";
        public static readonly string[] All = new[] { Razorpay, Stripe, PayU, CCAvenue, PayPal, Other };

        public static string GetDisplayName(string provider) => provider switch
        {
            Razorpay => "Razorpay",
            Stripe => "Stripe",
            PayU => "PayU",
            CCAvenue => "CCAvenue",
            PayPal => "PayPal",
            Other => "Other",
            _ => provider
        };
    }

    #endregion
}
