using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public class TaxSettlementViewModel
    {
        public class TaxSettlementModel
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

            [Required]
            public Guid CompanyId { get; set; }
            public string? CompanyName { get; set; }

            [Required(ErrorMessage = "Branch is required")]
            public Guid BranchId { get; set; }
            public string? BranchName { get; set; }

            [StringLength(40)]
            public string? SettlementNumber { get; set; }

            [Required(ErrorMessage = "Settlement Status is required")]
            public string SettlementStatus { get; set; } = "Draft";

            [Required(ErrorMessage = "Settlement Type is required")]
            public string SettlementType { get; set; } = string.Empty;

            [Required(ErrorMessage = "Settlement Date is required")]
            public DateTime SettlementDate { get; set; } = DateTime.Now;

            public DateTime? PostingDate { get; set; }

            [StringLength(1000)]
            public string? Narration { get; set; }

            // ── Period & Scope ──
            [Required(ErrorMessage = "Accounting Period is required")]
            public Guid AccountingPeriodId { get; set; }
            public string? AccountingPeriodName { get; set; }

            [Required(ErrorMessage = "Tax Period is required")]
            [StringLength(20)]
            public string TaxPeriodKey { get; set; } = string.Empty;

            [Required(ErrorMessage = "Tax Type Scope is required")]
            public string TaxTypeScope { get; set; } = string.Empty;

            public Guid? ReturnRunId { get; set; }
            public string? ReturnRunNumber { get; set; }

            [Required(ErrorMessage = "Government Authority Type is required")]
            public string GovernmentAuthorityType { get; set; } = string.Empty;

            [StringLength(20)]
            public string? JurisdictionCode { get; set; }

            // ── Allocation Header Summary (Derived) ──
            public decimal TotalOutstandingAmount { get; set; }
            public decimal TotalSettlementAmount { get; set; }
            public decimal TotalCashOrBankPaidAmount { get; set; }
            public decimal TotalCreditOffsetAmount { get; set; }
            public decimal RemainingUnsettledAmount { get; set; }
            public int AllocationCount { get; set; }

            // ── Payment / Challan / Remittance Details ──
            [Required(ErrorMessage = "Payment Mode is required")]
            public string PaymentMode { get; set; } = string.Empty;

            public Guid? BankAccountId { get; set; }
            public string? BankAccountName { get; set; }

            public Guid? CashAccountId { get; set; }
            public string? CashAccountName { get; set; }

            public Guid? BankTransactionId { get; set; }
            public Guid? FundTransferId { get; set; }

            [StringLength(100)]
            public string? ChallanNumber { get; set; }
            public DateTime? ChallanDate { get; set; }

            [StringLength(100)]
            public string? GovernmentReferenceNumber { get; set; }

            [StringLength(100)]
            public string? PaymentReferenceNumber { get; set; }

            public DateTime? RemittedOn { get; set; }
            public bool IsRemittanceProofAttached { get; set; }

            // ── GST Credit Offset Details ──
            public decimal InputCreditOffsetCGSTAmount { get; set; }
            public decimal InputCreditOffsetSGSTAmount { get; set; }
            public decimal InputCreditOffsetIGSTAmount { get; set; }
            public decimal InputCreditOffsetCESSAmount { get; set; }

            [StringLength(2000)]
            public string? OffsetRuleSnapshot { get; set; }

            // ── Posting & GL Evidence ──
            public Guid? JournalEntryId { get; set; }
            public DateTime? PostedOn { get; set; }
            public string? PostedBy { get; set; }

            public Guid? ReversalJournalEntryId { get; set; }

            [StringLength(500)]
            public string? ReversalReason { get; set; }

            public Guid? TDSPayableGLAccountSnapshot { get; set; }
            public Guid? TCSPayableGLAccountSnapshot { get; set; }
            public Guid? GSTPayableGLAccountSnapshot { get; set; }
            public Guid? InputTaxCreditGLAccountSnapshot { get; set; }
            public Guid? BankOrCashGLAccountSnapshot { get; set; }

            // ── Reconciliation & Closure ──
            public bool IsFullyAllocated { get; set; }
            public bool IsFullyReconciled { get; set; }

            [Required(ErrorMessage = "Reconciliation Status is required")]
            public string ReconciliationStatus { get; set; } = "NotReconciled";

            public DateTime? ClosedOn { get; set; }
            public string? ClosedBy { get; set; }

            // ── Attachments & Notes ──
            public int AttachmentCount { get; set; }

            [StringLength(1000)]
            public string? SettlementNotes { get; set; }

            // ── System Audit Fields ──
            public DateTime CreatedAt { get; set; } = DateTime.Now;
            public string? CreatedBy { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public string? UpdatedBy { get; set; }
            public byte[]? RowVersion { get; set; }
            public bool IsDeleted { get; set; } = false;

            // ── Child: Allocation Lines ──
            public List<TaxSettlementAllocationModel> Allocations { get; set; } = new();
        }

        public class TaxSettlementAllocationModel
        {
            public Guid Id { get; set; } = Guid.NewGuid();

            [Required]
            public Guid TaxSettlementId { get; set; }

            [Required(ErrorMessage = "Line Number is required")]
            public int LineNumber { get; set; }

            [Required(ErrorMessage = "Allocation Target Type is required")]
            public string AllocationTargetType { get; set; } = string.Empty;

            [Required(ErrorMessage = "Allocation Target Id is required")]
            public Guid AllocationTargetId { get; set; }

            [Required]
            [StringLength(50)]
            public string ReferenceNumberSnapshot { get; set; } = string.Empty;

            [StringLength(30)]
            public string? TaxCodeSnapshot { get; set; }

            [Required(ErrorMessage = "Liability Component Type is required")]
            public string LiabilityComponentType { get; set; } = string.Empty;

            [Required(ErrorMessage = "Outstanding Before Allocation is required")]
            public decimal OutstandingBeforeAllocation { get; set; }

            [Required(ErrorMessage = "Allocated Amount is required")]
            [Range(0.01, double.MaxValue, ErrorMessage = "Allocated Amount must be greater than 0")]
            public decimal AllocatedAmount { get; set; }

            [Required(ErrorMessage = "Settlement Mode is required")]
            public string SettlementMode { get; set; } = string.Empty;

            public decimal OutstandingAfterAllocation { get; set; }

            [Required(ErrorMessage = "Allocation Status is required")]
            public string AllocationStatus { get; set; } = "Pending";

            [StringLength(300)]
            public string? AllocationNotes { get; set; }
        }
    }

    // ── Enums / Constants ──
    public static class SettlementStatusEnum
    {
        public const string Draft = "Draft";
        public const string Submitted = "Submitted";
        public const string Approved = "Approved";
        public const string Posted = "Posted";
        public const string PartiallyReconciled = "PartiallyReconciled";
        public const string Reconciled = "Reconciled";
        public const string Closed = "Closed";
        public const string Reversed = "Reversed";
        public const string Cancelled = "Cancelled";
    }

    public static class SettlementTypeEnum
    {
        public const string GSTCashPayment = "GSTCashPayment";
        public const string GSTInputCreditOffset = "GSTInputCreditOffset";
        public const string GSTMixedSettlement = "GSTMixedSettlement";
        public const string TDSRemittance = "TDSRemittance";
        public const string TCSRemittance = "TCSRemittance";
        public const string TaxAdjustment = "TaxAdjustment";
    }

    public static class PaymentModeEnum
    {
        public const string Bank = "Bank";
        public const string Cash = "Cash";
        public const string CreditOffsetOnly = "CreditOffsetOnly";
        public const string Mixed = "Mixed";
    }
}
