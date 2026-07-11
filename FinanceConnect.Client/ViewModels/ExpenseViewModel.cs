using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public class ExpenseViewModel
    {
        // ── Enums ──────────────────────────────────────────────────────────────

        public enum ExpenseStatusEnum
        {
            Draft = 1,
            Submitted = 2,
            UnderReview = 3,
            Approved = 4,
            Posted = 5,
            PartiallyPosted = 6,
            Rejected = 7,
            Cancelled = 8,
            Closed = 9
        }

        public enum ExpenseTypeEnum
        {
            Supplier = 1,
            EmployeeReimbursement = 2,
            CompanyCard = 3,
            Cash = 4,
            AccrualOnly = 5,
            Prepayment = 6,
            ManualAdjustment = 7
        }

        public enum PayeeTypeEnum
        {
            Supplier = 1,
            Employee = 2,
            Other = 3
        }

        public enum SourceDocumentTypeEnum
        {
            VendorBill = 1,
            ExpenseClaim = 2,
            CardTransaction = 3,
            CashVoucher = 4,
            ManualExpense = 5,
            PayrollFeed = 6,
            Other = 7
        }

        public enum TimingTreatmentEnum
        {
            ImmediateExpense = 1,
            Accrual = 2,
            Prepayment = 3,
            Mixed = 4
        }

        public enum TimingTreatmentStatusEnum
        {
            NotRequired = 1,
            Pending = 2,
            Created = 3,
            PartiallyReleased = 4,
            FullyReleased = 5,
            Closed = 6
        }

        public enum BudgetCheckStatusEnum
        {
            NotChecked = 1,
            Ok = 2,
            Warning = 3,
            Blocked = 4,
            Overridden = 5
        }

        public enum PostingStatusEnum
        {
            NotPosted = 1,
            Queued = 2,
            Posted = 3,
            Failed = 4,
            Reversed = 5
        }

        public enum AccrualTreatmentEnum
        {
            Immediate = 1,
            Accrue = 2,
            Prepay = 3,
            Mixed = 4
        }

        // ── Expense Line Model ─────────────────────────────────────────────────

        public class ExpenseLine
        {
            public Guid ExpenseLineId { get; set; } = Guid.NewGuid();
            public Guid ExpenseId { get; set; }
            public int LineNumber { get; set; }

            [Required(ErrorMessage = "Line Description is required")]
            [MaxLength(500)]
            public string LineDescription { get; set; } = string.Empty;

            [Required(ErrorMessage = "Expense Category is required")]
            public Guid ExpenseCategoryId { get; set; }
            public string? ExpenseCategoryName { get; set; }

            [Required(ErrorMessage = "GL Account is required")]
            public Guid GLAccountId { get; set; }
            public string? GLAccountName { get; set; }

            public Guid? TaxCodeId { get; set; }
            public decimal? Quantity { get; set; }
            public decimal? UnitPrice { get; set; }

            [Required(ErrorMessage = "Net Amount is required")]
            [Range(0, double.MaxValue, ErrorMessage = "Net Amount must be >= 0")]
            public decimal NetAmount { get; set; }

            public decimal TaxAmount { get; set; }

            [Required(ErrorMessage = "Gross Amount is required")]
            [Range(0, double.MaxValue, ErrorMessage = "Gross Amount must be >= 0")]
            public decimal GrossAmount { get; set; }

            public Guid? CostCenterId { get; set; }
            public Guid? DepartmentId { get; set; }
            public Guid? BranchId { get; set; }
            public Guid? ProjectId { get; set; }
            public Guid? BudgetLineId { get; set; }
            public DateTime? ServiceStartDate { get; set; }
            public DateTime? ServiceEndDate { get; set; }

            public AccrualTreatmentEnum AccrualTreatment { get; set; } = AccrualTreatmentEnum.Immediate;

            public Guid? AccrualId { get; set; }
            public Guid? PrepaymentId { get; set; }
            public BudgetCheckStatusEnum BudgetCheckStatus { get; set; } = BudgetCheckStatusEnum.NotChecked;

            [MaxLength(500)]
            public string? LineNotes { get; set; }
        }

        // ── Expense Model ──────────────────────────────────────────────────────

        public class Expense
        {
            // ─── Section 1: Core Identity (Header) ───────────────────────────
            public Guid ExpenseId { get; set; } = Guid.NewGuid();
            public Guid TenantId { get; set; }
            public Guid CompanyId { get; set; }

            [Required(ErrorMessage = "Expense Code is required")]
            [MaxLength(30, ErrorMessage = "Expense Code cannot exceed 30 characters")]
            public string ExpenseCode { get; set; } = string.Empty;

            [Required(ErrorMessage = "Expense Title is required")]
            [MaxLength(200, ErrorMessage = "Expense Title cannot exceed 200 characters")]
            public string ExpenseTitle { get; set; } = string.Empty;

            [MaxLength(1000)]
            public string? Description { get; set; }

            [Required(ErrorMessage = "Status is required")]
            public ExpenseStatusEnum ExpenseStatus { get; set; } = ExpenseStatusEnum.Draft;

            [Required(ErrorMessage = "Expense Type is required")]
            public ExpenseTypeEnum ExpenseType { get; set; } = ExpenseTypeEnum.Supplier;

            // ─── Section 2: Source / Payee Context ───────────────────────────
            [Required(ErrorMessage = "Payee Type is required")]
            public PayeeTypeEnum PayeeType { get; set; } = PayeeTypeEnum.Supplier;

            public Guid? PayeeId { get; set; }

            [MaxLength(30)]
            public string? PayeeCodeSnapshot { get; set; }
            [MaxLength(200)]
            public string? PayeeNameSnapshot { get; set; }

            [MaxLength(50)]
            public string? SupplierInvoiceNumber { get; set; }

            public Guid? APInvoiceId { get; set; }
            public Guid? ExpenseClaimId { get; set; }

            [Required(ErrorMessage = "Source Document Type is required")]
            public SourceDocumentTypeEnum SourceDocumentType { get; set; } = SourceDocumentTypeEnum.ManualExpense;

            public Guid? SourceDocumentId { get; set; }

            [MaxLength(50)]
            public string? SourceDocumentNumber { get; set; }

            // ─── Section 3: Date & Period Context ────────────────────────────
            [Required(ErrorMessage = "Expense Date is required")]
            public DateTime? ExpenseDate { get; set; }

            public DateTime? PostingDate { get; set; }

            [Required(ErrorMessage = "Fiscal Year is required")]
            public Guid FiscalYearId { get; set; }

            public Guid? AccountingPeriodId { get; set; }
            public DateTime? CoverageStartDate { get; set; }
            public DateTime? CoverageEndDate { get; set; }

            // ─── Section 4: Header Amounts ───────────────────────────────────
            [Required(ErrorMessage = "Currency is required")]
            public Guid CurrencyId { get; set; }

            public Guid? ExchangeRateId { get; set; }
            public bool TaxInclusiveFlag { get; set; }
            public decimal TotalNetAmount { get; set; }
            public decimal TotalTaxAmount { get; set; }
            public decimal TotalGrossAmount { get; set; }
            public decimal? AdjustmentAmount { get; set; }

            // ─── Section 5: Timing Treatment Readiness ───────────────────────
            public bool AccrualRequiredFlag { get; set; }
            public bool PrepaymentRequiredFlag { get; set; }
            public Guid? AccrualId { get; set; }
            public Guid? PrepaymentId { get; set; }

            [Required(ErrorMessage = "Timing Treatment is required")]
            public TimingTreatmentEnum TimingTreatment { get; set; } = TimingTreatmentEnum.ImmediateExpense;

            [Required(ErrorMessage = "Timing Treatment Status is required")]
            public TimingTreatmentStatusEnum TimingTreatmentStatus { get; set; } = TimingTreatmentStatusEnum.NotRequired;

            // ─── Section 6: Dimensions & Ownership ──────────────────────────
            public Guid? CostCenterId { get; set; }
            public string? CostCenterName { get; set; }
            public Guid? DepartmentId { get; set; }
            public string? DepartmentName { get; set; }
            public Guid? BranchId { get; set; }
            public string? BranchName { get; set; }
            public Guid? ProjectId { get; set; }
            public string? ProjectName { get; set; }

            [MaxLength(100)]
            public string? ExpenseOwnerUserText { get; set; }

            public Guid? BudgetId { get; set; }
            public Guid? BudgetLineId { get; set; }

            public BudgetCheckStatusEnum BudgetCheckStatus { get; set; } = BudgetCheckStatusEnum.NotChecked;

            public string? DimensionScopeJson { get; set; }

            // ─── Section 7: Workflow & Posting ──────────────────────────────
            [Required(ErrorMessage = "Prepared By is required")]
            [MaxLength(100)]
            public string PreparedByUserId { get; set; } = string.Empty;

            public DateTime? SubmittedOn { get; set; }
            [MaxLength(100)]
            public string? ReviewedByUserId { get; set; }
            public DateTime? ReviewedOn { get; set; }
            [MaxLength(100)]
            public string? ApprovedByUserId { get; set; }
            public DateTime? ApprovedOn { get; set; }

            [Required(ErrorMessage = "Posting Status is required")]
            public PostingStatusEnum PostingStatus { get; set; } = PostingStatusEnum.NotPosted;

            public Guid? GLJournalEntryId { get; set; }
            public bool IsLocked { get; set; }
            public DateTime? LockedOn { get; set; }
            public Guid? LockedBy { get; set; }

            [MaxLength(500)]
            public string? RejectionReason { get; set; }
            [MaxLength(500)]
            public string? CancellationReason { get; set; }

            // ─── Section 8: Attachments & Supporting Evidence ────────────────
            public bool ReceiptRequiredFlag { get; set; }
            public int AttachmentCount { get; set; }

            [MaxLength(1500)]
            public string? Notes { get; set; }
            [MaxLength(1500)]
            public string? SupportingCommentary { get; set; }

            // ─── Section 9: Expense Lines (Child) ────────────────────────────
            public List<ExpenseLine> Lines { get; set; } = new();

            // ─── Section 10: System Audit Fields (Hidden) ────────────────────
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public Guid? CreatedBy { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public Guid? UpdatedBy { get; set; }
            public bool IsDeleted { get; set; }
        }
    }
}
