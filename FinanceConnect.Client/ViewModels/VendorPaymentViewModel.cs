using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    #region Model #38: VendorPayment

    /// <summary>
    /// Model #38: VendorPayment – AP cash-outflow document recording money paid to a vendor
    /// via Bank Transfer/UPI/Cheque/Cash/Gateway.
    /// If VendorBill = "money we owe vendor", then VendorPayment = "money we paid to vendor"
    /// </summary>
    public class VendorPaymentViewModel
    {
        // Section 1: Core Payment Identity Fields (Header)

        /// <summary>PK - VendorPaymentId - hidden in UI</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Tenant identifier - hidden in UI</summary>
        public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

        /// <summary>FK → Company - hidden</summary>
        [Required(ErrorMessage = "Company is required")]
        public Guid CompanyId { get; set; }
        public string? CompanyName { get; set; }

        /// <summary>FK → Branch - Dropdown (default: user branch)</summary>
        [Required(ErrorMessage = "Branch is required")]
        public Guid BranchId { get; set; }
        public string? BranchCode { get; set; }
        public string? BranchName { get; set; }

        /// <summary>FK → Vendor - Search dropdown (typeahead)</summary>
        [Required(ErrorMessage = "Vendor is required")]
        public Guid VendorId { get; set; }
        public string? VendorCode { get; set; }
        public string? VendorName { get; set; }

        /// <summary>FK → VendorAccount - Read-only (system derived from VendorId + CurrencyId)</summary>
        public Guid? VendorAccountId { get; set; }
        public string? VendorAccountName { get; set; }

        /// <summary>Payment Number - max 40 chars, unique per CompanyId + FinancialYear</summary>
        [Required(ErrorMessage = "Payment Number is required")]
        [StringLength(40, ErrorMessage = "Payment Number cannot exceed 40 characters")]
        public string PaymentNumber { get; set; } = string.Empty;

        /// <summary>Payment Date - date picker</summary>
        [Required(ErrorMessage = "Payment Date is required")]
        public DateTime PaymentDate { get; set; } = DateTime.Today;

        /// <summary>Posting Date - conditional required at posting</summary>
        public DateTime? PostingDate { get; set; }

        /// <summary>FK → CurrencyMaster - Badge (default from vendor)</summary>
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

        // Section 2: Payment Method & Instrument (Critical Controls)

        /// <summary>Payment Method - dropdown</summary>
        [Required(ErrorMessage = "Payment Method is required")]
        public string PaymentMethod { get; set; } = VendorPaymentMethods.BankTransfer;

        /// <summary>FK → GLAccountMaster - Paying Account (Bank/Cash GL)</summary>
        [Required(ErrorMessage = "Payment Account is required")]
        public Guid PaymentAccountId { get; set; }
        public string? PaymentAccountCode { get; set; }
        public string? PaymentAccountName { get; set; }

        /// <summary>UTR / Cheque No / Transaction Ref - max 60 chars</summary>
        [StringLength(60, ErrorMessage = "Payment Reference cannot exceed 60 characters")]
        public string? PaymentReferenceNumber { get; set; }

        /// <summary>Reference Date - cheque date or transaction date</summary>
        public DateTime? ReferenceDate { get; set; }

        /// <summary>Bank Name Snapshot - max 150 chars (optional)</summary>
        [StringLength(150, ErrorMessage = "Bank Name cannot exceed 150 characters")]
        public string? BankNameSnapshot { get; set; }

        /// <summary>Instrument Status - badge</summary>
        public string InstrumentStatus { get; set; } = VendorInstrumentStatuses.Initiated;

        /// <summary>Gateway Transaction Id - max 100 chars (for idempotency tracking)</summary>
        [StringLength(100, ErrorMessage = "Gateway Transaction Id cannot exceed 100 characters")]
        public string? GatewayTransactionId { get; set; }

        /// <summary>Idempotency Key - max 100 chars (avoid duplicate posting)</summary>
        [StringLength(100, ErrorMessage = "Idempotency Key cannot exceed 100 characters")]
        public string? IdempotencyKey { get; set; }

        // Section 3: Payment Amount Breakdown

        /// <summary>Payment Amount (Gross) - numeric input, must be > 0</summary>
        [Required(ErrorMessage = "Payment Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Payment Amount must be greater than zero")]
        public decimal PaymentGrossAmount { get; set; } = 0;

        /// <summary>Allocated Amount - Sum of allocation rows (read-only)</summary>
        public decimal AllocatedAmount { get; set; } = 0;

        /// <summary>Advance / Unapplied Amount - PaymentGrossAmount - AllocatedAmount (read-only)</summary>
        public decimal UnallocatedAdvanceAmount { get; set; } = 0;

        /// <summary>Net Bank Outflow - PaymentGrossAmount - TDSWithheldAmount + BankChargesAmount (read-only)</summary>
        public decimal NetBankOutflowAmount { get; set; } = 0;

        // Section 4: Withholding / TDS (India-ready)

        /// <summary>Is TDS Applicable - toggle</summary>
        public bool IsTDSApplicable { get; set; } = false;

        /// <summary>TDS Section Code Snapshot - max 20 chars</summary>
        [StringLength(20, ErrorMessage = "TDS Section Code cannot exceed 20 characters")]
        public string? TDSSectionCodeSnapshot { get; set; }

        /// <summary>TDS Rate Percent Snapshot - decimal(6,3)</summary>
        [Range(0, 100, ErrorMessage = "TDS Rate must be between 0 and 100")]
        public decimal TDSRatePercentSnapshot { get; set; } = 0;

        /// <summary>TDS Base Amount - typically AllocatedAmount</summary>
        public decimal TDSBaseAmount { get; set; } = 0;

        /// <summary>TDS Withheld Amount - numeric</summary>
        [Range(0, double.MaxValue, ErrorMessage = "TDS Withheld Amount must be non-negative")]
        public decimal TDSWithheldAmount { get; set; } = 0;

        /// <summary>FK → GLAccountMaster - TDS Payable Account (future-ready)</summary>
        public Guid? TDSGLAccountId { get; set; }
        public string? TDSGLAccountCode { get; set; }
        public string? TDSGLAccountName { get; set; }

        /// <summary>Bank Charges Amount - optional</summary>
        [Range(0, double.MaxValue, ErrorMessage = "Bank Charges must be non-negative")]
        public decimal BankChargesAmount { get; set; } = 0;

        /// <summary>FK → GLAccountMaster - Bank Charges Account</summary>
        public Guid? BankChargesAccountId { get; set; }
        public string? BankChargesAccountCode { get; set; }
        public string? BankChargesAccountName { get; set; }

        // Section 5: Allocations (Child rows)

        /// <summary>Payment Allocations collection</summary>
        public List<VendorPaymentAllocationModel> Allocations { get; set; } = new List<VendorPaymentAllocationModel>();

        // Section 6: Workflow & Status

        /// <summary>Payment Status - Draft/Submitted/Approved/Rejected/Posted/Cancelled/Reversed</summary>
        [Required(ErrorMessage = "Status is required")]
        public string PaymentStatus { get; set; } = VendorPaymentStatuses.Draft;

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

        /// <summary>Rejection Reason - max 250</summary>
        [StringLength(250, ErrorMessage = "Rejection Reason cannot exceed 250 characters")]
        public string? RejectionReason { get; set; }

        /// <summary>Cancellation Reason - max 250</summary>
        [StringLength(250, ErrorMessage = "Cancellation Reason cannot exceed 250 characters")]
        public string? CancellationReason { get; set; }

        /// <summary>Cancelled On - datetime (pre-post only)</summary>
        public DateTime? CancelledOn { get; set; }

        /// <summary>FK → User who cancelled</summary>
        public Guid? CancelledByUserId { get; set; }
        public string? CancelledBy { get; set; }

        /// <summary>Reversal Reason - max 250</summary>
        [StringLength(250, ErrorMessage = "Reversal Reason cannot exceed 250 characters")]
        public string? ReversalReason { get; set; }

        /// <summary>Reversal Reference - bank reversal ref / bounce memo id (max 50)</summary>
        [StringLength(50, ErrorMessage = "Reversal Reference cannot exceed 50 characters")]
        public string? ReversalReference { get; set; }

        /// <summary>Reversed On - datetime</summary>
        public DateTime? ReversedOn { get; set; }

        /// <summary>FK → User who reversed</summary>
        public Guid? ReversedByUserId { get; set; }
        public string? ReversedBy { get; set; }

        // Section 7: Attachments & Evidence

        /// <summary>Has Attachments - badge</summary>
        public bool HasAttachments => AttachmentCount > 0;

        /// <summary>Attachment Count - int badge</summary>
        public int AttachmentCount { get; set; } = 0;

        // Section 8: Posting Classification (Accounting)

        /// <summary>FK → GLAccountMaster - AP Payable Account snapshot at posting</summary>
        public Guid? PayableAccountIdSnapshot { get; set; }
        public string? PayableAccountCode { get; set; }
        public string? PayableAccountName { get; set; }

        /// <summary>FK → GLAccountMaster - Advance To Vendor Account snapshot at posting</summary>
        public Guid? AdvanceToVendorAccountIdSnapshot { get; set; }
        public string? AdvanceToVendorAccountCode { get; set; }
        public string? AdvanceToVendorAccountName { get; set; }

        /// <summary>Is System Generated - integration created</summary>
        public bool IsSystemGenerated { get; set; } = false;

        /// <summary>Is Reconciled - bank reconciliation (Future)</summary>
        public bool IsReconciled { get; set; } = false;

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

        /// <summary>Recalculate amount splits from allocations and TDS</summary>
        public void RecalculateAmounts()
        {
            AllocatedAmount = Allocations.Sum(a => a.AllocatedToBillAmount);
            
            // UnallocatedAdvance = Gross - Allocated
            UnallocatedAdvanceAmount = PaymentGrossAmount - AllocatedAmount;
            if (UnallocatedAdvanceAmount < 0)
            {
                UnallocatedAdvanceAmount = 0;
            }

            // Calculate TDS if applicable
            if (IsTDSApplicable && TDSRatePercentSnapshot > 0)
            {
                TDSBaseAmount = AllocatedAmount; // Typically TDS is on allocated amount
                TDSWithheldAmount = Math.Round(TDSBaseAmount * (TDSRatePercentSnapshot / 100), 2);
            }
            else
            {
                TDSBaseAmount = 0;
                TDSWithheldAmount = 0;
            }

            // NetBankOutflow = Gross - TDS Withheld + Bank Charges
            NetBankOutflowAmount = PaymentGrossAmount - TDSWithheldAmount + BankChargesAmount;
        }

        /// <summary>Check if payment can be edited</summary>
        public bool CanEdit => PaymentStatus == VendorPaymentStatuses.Draft;

        /// <summary>Check if payment can be submitted</summary>
        public bool CanSubmit => PaymentStatus == VendorPaymentStatuses.Draft;

        /// <summary>Check if payment can be approved</summary>
        public bool CanApprove => PaymentStatus == VendorPaymentStatuses.Submitted;

        /// <summary>Check if payment can be rejected</summary>
        public bool CanReject => PaymentStatus == VendorPaymentStatuses.Submitted;

        /// <summary>Check if payment can be posted</summary>
        public bool CanPost => PaymentStatus == VendorPaymentStatuses.Draft || PaymentStatus == VendorPaymentStatuses.Approved;

        /// <summary>Check if payment can be cancelled (pre-post only)</summary>
        public bool CanCancel => PaymentStatus == VendorPaymentStatuses.Draft || PaymentStatus == VendorPaymentStatuses.Submitted;

        /// <summary>Check if payment can be reversed (post-only)</summary>
        public bool CanReverse => PaymentStatus == VendorPaymentStatuses.Posted;

        // Alias properties for UI convenience
        public string Status => PaymentStatus;
        public decimal TotalAmount => PaymentGrossAmount;
        public string? Narration => PaymentNarration;
        public DateTime CreatedDate => CreatedAt;
        public DateTime? ModifiedDate => UpdatedAt;
        public string? ModifiedBy => UpdatedBy;
        public DateTime? PostedDate => PostedOn;
        public DateTime? ReversedDate => ReversedOn;
    }

    #endregion

    #region VendorPaymentAllocation (Child rows)

    /// <summary>
    /// VendorPaymentAllocation - Allocation of payment to vendor bills
    /// </summary>
    public class VendorPaymentAllocationModel
    {
        /// <summary>PK - VendorPaymentAllocationId</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>FK → VendorPayment</summary>
        [Required]
        public Guid VendorPaymentId { get; set; }

        /// <summary>FK → VendorBill</summary>
        [Required(ErrorMessage = "Bill is required")]
        public Guid VendorBillId { get; set; }

        /// <summary>Bill Number Snapshot - read-only</summary>
        public string? BillNumberSnapshot { get; set; }

        /// <summary>Bill Date Snapshot - read-only</summary>
        public DateTime? BillDateSnapshot { get; set; }

        /// <summary>Bill Due Date Snapshot - read-only</summary>
        public DateTime? BillDueDateSnapshot { get; set; }

        /// <summary>Bill Outstanding Snapshot - read-only</summary>
        public decimal BillOutstandingSnapshot { get; set; } = 0;

        /// <summary>Amount allocated to this bill</summary>
        [Required(ErrorMessage = "Allocated Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Allocated Amount must be greater than zero")]
        public decimal AllocatedToBillAmount { get; set; } = 0;

        /// <summary>Allocation Narration - optional (max 250)</summary>
        [StringLength(250, ErrorMessage = "Allocation Narration cannot exceed 250 characters")]
        public string? AllocationNarration { get; set; }

        /// <summary>Allocation Order - for FIFO trace</summary>
        public int AllocationOrder { get; set; } = 0;

        // Audit Fields
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }

    #endregion

    #region VendorPayment-related Enums and Static Classes

    /// <summary>Vendor Payment Status values</summary>
    public static class VendorPaymentStatuses
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



    /// <summary>Vendor Instrument Status values</summary>
    public static class VendorInstrumentStatuses
    {
        public const string Initiated = "Initiated";
        public const string SentToBank = "SentToBank";
        public const string Completed = "Completed";
        public const string Failed = "Failed";
        public const string Reversed = "Reversed";
        public static readonly string[] All = new[] { Initiated, SentToBank, Completed, Failed, Reversed };

        public static string GetDisplayName(string status) => status switch
        {
            Initiated => "Initiated",
            SentToBank => "Sent to Bank",
            Completed => "Completed",
            Failed => "Failed",
            Reversed => "Reversed",
            _ => status
        };
    }

    #endregion
}
