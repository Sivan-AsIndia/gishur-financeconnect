using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public class AccrualViewModel
    {
        // ── Enums ──────────────────────────────────────────────────────────────

        public enum AccrualStatusEnum
        {
            Draft = 1, Submitted = 2, Approved = 3, Posted = 4,
            PartiallyReversed = 5, FullyReversed = 6,
            PartiallyCleared = 7, FullyCleared = 8,
            Cancelled = 9, Closed = 10
        }

        public enum AccrualTypeEnum { ExpenseAccrual = 1, RevenueAccrual = 2 }

        public enum SourceTypeEnum
        {
            Expense = 1, Revenue = 2, Contract = 3, Project = 4,
            ManualCloseAdjustment = 5, Other = 6
        }

        public enum MaterialityLevelEnum { Low = 1, Medium = 2, High = 3, Critical = 4 }

        public enum AccrualBasisTypeEnum
        {
            ContractualEstimate = 1, HistoricalTrend = 2, ManualEstimate = 3,
            ActualSupportPending = 4, ScheduleBased = 5, Other = 6
        }

        public enum ReversalStrategyEnum
        {
            AutoReverseNextPeriod = 1, ManualReverse = 2,
            ClearAgainstActual = 3, PartialReverseAndClear = 4
        }

        public enum ExpectedClearanceModeEnum
        {
            AgainstVendorBill = 1, AgainstRevenueBilling = 2,
            AgainstJournalAdjustment = 3, ManualClose = 4, NotApplicable = 5
        }

        public enum ReversalStatusEnum
        {
            NotStarted = 1, Pending = 2, PartiallyReversed = 3,
            FullyReversed = 4, Waived = 5
        }

        public enum ClearanceStatusEnum
        {
            NotStarted = 1, Pending = 2, PartiallyCleared = 3,
            FullyCleared = 4, Waived = 5
        }

        public enum PostingStatusEnum
        {
            NotPosted = 1, Posted = 2, PartiallyReversed = 3,
            FullyReversed = 4, Failed = 5
        }

        // ── Model ──────────────────────────────────────────────────────────────

        public class Accrual
        {
            // 1. Core Identity (Header)
            public Guid AccrualId { get; set; } = Guid.NewGuid();
            public Guid TenantId { get; set; }
            public Guid CompanyId { get; set; }

            [Required(ErrorMessage = "Accrual Code is required")]
            [MaxLength(30)]
            public string AccrualCode { get; set; } = string.Empty;

            [Required(ErrorMessage = "Accrual Title is required")]
            [MaxLength(200)]
            public string AccrualTitle { get; set; } = string.Empty;

            [MaxLength(1000)]
            public string? Description { get; set; }

            [Required(ErrorMessage = "Status is required")]
            public AccrualStatusEnum AccrualStatus { get; set; } = AccrualStatusEnum.Draft;

            [Required(ErrorMessage = "Accrual Type is required")]
            public AccrualTypeEnum AccrualType { get; set; } = AccrualTypeEnum.ExpenseAccrual;

            // 2. Source Context
            [Required(ErrorMessage = "Source Type is required")]
            public SourceTypeEnum SourceType { get; set; } = SourceTypeEnum.Expense;

            public Guid? SourceExpenseId { get; set; }
            public Guid? SourceRevenueId { get; set; }
            public string? SourceDocumentType { get; set; }
            public Guid? SourceDocumentId { get; set; }

            [MaxLength(50)]
            public string? SourceDocumentNumber { get; set; }

            public Guid? SupplierId { get; set; }
            public string? SupplierName { get; set; }
            public Guid? CustomerId { get; set; }
            public string? CustomerName { get; set; }
            public Guid? ProjectId { get; set; }
            public string? ProjectName { get; set; }
            public Guid? ContractId { get; set; }
            public string? ContractName { get; set; }

            [Required(ErrorMessage = "Basis Reference is required")]
            [MaxLength(500)]
            public string BasisReferenceText { get; set; } = string.Empty;

            // 3. Period & Date Context
            [Required(ErrorMessage = "Accrual Date is required")]
            public DateTime? AccrualDate { get; set; }

            [Required(ErrorMessage = "Accounting Period is required")]
            public Guid? AccountingPeriodId { get; set; }
            public string? AccountingPeriodName { get; set; }

            [Required(ErrorMessage = "Fiscal Year is required")]
            public Guid? FiscalYearId { get; set; }

            public DateTime? ServiceOrCoverageFrom { get; set; }
            public DateTime? ServiceOrCoverageTo { get; set; }
            public DateTime? ExpectedActualDocumentDate { get; set; }

            // 4. Amounts & Basis
            [Required(ErrorMessage = "Currency is required")]
            public Guid? CurrencyId { get; set; }

            public Guid? ExchangeRateId { get; set; }

            [Required(ErrorMessage = "Original Accrual Amount is required")]
            [Range(0.01, double.MaxValue, ErrorMessage = "Original Accrual Amount must be > 0")]
            public decimal OriginalAccrualAmount { get; set; }

            public decimal ReversedAmountToDate { get; set; }
            public decimal ClearedAmountToDate { get; set; }
            public decimal OpenAccrualBalance => OriginalAccrualAmount - ReversedAmountToDate - ClearedAmountToDate;
            public decimal? EstimatedActualAmount { get; set; }
            public MaterialityLevelEnum? MaterialityLevel { get; set; }

            [Required(ErrorMessage = "Accrual Basis Type is required")]
            public AccrualBasisTypeEnum AccrualBasisType { get; set; } = AccrualBasisTypeEnum.ManualEstimate;

            [MaxLength(500)]
            public string? SupportingAmountReference { get; set; }

            // 5. Reversal & Clearance Strategy
            [Required(ErrorMessage = "Reversal Strategy is required")]
            public ReversalStrategyEnum ReversalStrategy { get; set; } = ReversalStrategyEnum.AutoReverseNextPeriod;

            public DateTime? AutoReverseDate { get; set; }

            [Required(ErrorMessage = "Expected Clearance Mode is required")]
            public ExpectedClearanceModeEnum ExpectedClearanceMode { get; set; } = ExpectedClearanceModeEnum.AgainstVendorBill;

            public Guid? ClearanceReferenceId { get; set; }

            [MaxLength(100)]
            public string? ClearanceReferenceNumber { get; set; }

            [Required(ErrorMessage = "Reversal Status is required")]
            public ReversalStatusEnum ReversalStatus { get; set; } = ReversalStatusEnum.NotStarted;

            [Required(ErrorMessage = "Clearance Status is required")]
            public ClearanceStatusEnum ClearanceStatus { get; set; } = ClearanceStatusEnum.NotStarted;

            public DateTime? LastReversalDate { get; set; }
            public DateTime? LastClearanceDate { get; set; }

            // 6. Dimensions & Ownership
            public Guid? CostCenterId { get; set; }
            public string? CostCenterName { get; set; }
            public Guid? DepartmentId { get; set; }
            public string? DepartmentName { get; set; }
            public Guid? BranchId { get; set; }
            public string? BranchName { get; set; }
            public Guid? ProjectCostOwnerId { get; set; }
            public string? ProjectCostOwnerName { get; set; }
            public Guid? ExpenseCategoryId { get; set; }
            public string? ExpenseCategoryName { get; set; }

            [Required(ErrorMessage = "GL Account is required")]
            public Guid? GLAccountId { get; set; }
            public string? GLAccountName { get; set; }

            [Required(ErrorMessage = "Accrual Liability/Asset GL Account is required")]
            public Guid? AccrualLiabilityOrAssetGLId { get; set; }
            public string? AccrualLiabilityOrAssetGLName { get; set; }

            // 7. Workflow & Posting
            [Required(ErrorMessage = "Prepared By is required")]
            public string? PreparedByUserId { get; set; }

            public DateTime? SubmittedOn { get; set; }
            public string? ReviewedByUserId { get; set; }
            public DateTime? ReviewedOn { get; set; }
            public string? ApprovedByUserId { get; set; }
            public DateTime? ApprovedOn { get; set; }

            [Required(ErrorMessage = "Posting Status is required")]
            public PostingStatusEnum PostingStatus { get; set; } = PostingStatusEnum.NotPosted;

            public Guid? JournalEntryId { get; set; }
            public Guid? ReversalJournalEntryId { get; set; }
            public bool IsLocked { get; set; }
            public DateTime? LockedOn { get; set; }
            public string? LockedBy { get; set; }

            [MaxLength(500)]
            public string? CancellationReason { get; set; }

            // 8. Notes & Supporting Evidence
            [MaxLength(1500)]
            public string? AssumptionText { get; set; }

            [MaxLength(1500)]
            public string? FinanceNotes { get; set; }

            public int AttachmentCount { get; set; }
            public bool PolicyExceptionFlag { get; set; }

            [MaxLength(1000)]
            public string? PolicyExceptionReason { get; set; }

            // 9. System Audit
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public string? CreatedBy { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public string? UpdatedBy { get; set; }
            public int RowVersion { get; set; }
            public bool IsDeleted { get; set; }
        }
    }
}
