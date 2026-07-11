using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public class PrepaymentViewModel
    {
        // ── Enums ──────────────────────────────────────────────────────────────

        public enum PrepaymentStatusEnum
        {
            Draft = 1, Submitted = 2, Approved = 3, Posted = 4,
            InProgress = 5, PartiallyReleased = 6, FullyReleased = 7,
            Cancelled = 8, Closed = 9
        }

        public enum SourceTypeEnum
        {
            Expense = 1, VendorBill = 2, Payment = 3, Contract = 4,
            ManualFinanceAdjustment = 5, Other = 6
        }

        public enum ReleaseMethodEnum
        {
            StraightLine = 1, MonthlyEqual = 2, QuarterlyEqual = 3,
            CustomSchedule = 4, ManualRelease = 5
        }

        public enum ReleaseFrequencyEnum { Monthly = 1, Quarterly = 2, Custom = 3 }

        public enum MaterialityLevelEnum { Low = 1, Medium = 2, High = 3, Critical = 4 }

        public enum PostingStatusEnum
        {
            NotPosted = 1, Posted = 2, PartiallyReleased = 3,
            FullyReleased = 4, Failed = 5
        }

        public enum ReleaseStatusEnum
        {
            NotStarted = 1, Pending = 2, InProgress = 3,
            PartiallyReleased = 4, FullyReleased = 5, Waived = 6
        }

        public enum ReleaseLineStatusEnum
        {
            Planned = 1, Ready = 2, Released = 3,
            PartiallyReleased = 4, Skipped = 5, Cancelled = 6
        }

        // ── Child Model: PrepaymentReleaseLine ─────────────────────────────────

        public class PrepaymentReleaseLine
        {
            public Guid PrepaymentReleaseLineId { get; set; } = Guid.NewGuid();
            public Guid PrepaymentId { get; set; }

            [Required(ErrorMessage = "Line Number is required")]
            public int LineNumber { get; set; }

            [Required(ErrorMessage = "Schedule Date is required")]
            public DateTime? ScheduleDate { get; set; }

            public Guid? AccountingPeriodId { get; set; }
            public string? AccountingPeriodName { get; set; }

            [Required(ErrorMessage = "Scheduled Release Amount is required")]
            public decimal ScheduledReleaseAmount { get; set; }

            public decimal ReleasedAmount { get; set; }
            public decimal RemainingLineBalance => ScheduledReleaseAmount - ReleasedAmount;

            [Required(ErrorMessage = "Release Line Status is required")]
            public ReleaseLineStatusEnum ReleaseLineStatus { get; set; } = ReleaseLineStatusEnum.Planned;

            public DateTime? ReleasedOn { get; set; }
            public string? ReleasedBy { get; set; }
            public Guid? ReleaseJournalEntryId { get; set; }

            [MaxLength(500)]
            public string? LineNotes { get; set; }
        }

        // ── Model ──────────────────────────────────────────────────────────────

        public class Prepayment
        {
            // 1. Core Identity (Header)
            public Guid PrepaymentId { get; set; } = Guid.NewGuid();
            public Guid TenantId { get; set; }
            public Guid CompanyId { get; set; }

            [Required(ErrorMessage = "Prepayment Code is required")]
            [MaxLength(30)]
            public string PrepaymentCode { get; set; } = string.Empty;

            [Required(ErrorMessage = "Prepayment Title is required")]
            [MaxLength(200)]
            public string PrepaymentTitle { get; set; } = string.Empty;

            [MaxLength(1000)]
            public string? Description { get; set; }

            [Required(ErrorMessage = "Status is required")]
            public PrepaymentStatusEnum PrepaymentStatus { get; set; } = PrepaymentStatusEnum.Draft;

            // 2. Source Context
            [Required(ErrorMessage = "Source Type is required")]
            public SourceTypeEnum SourceType { get; set; } = SourceTypeEnum.Expense;

            public Guid? SourceExpenseId { get; set; }
            public Guid? SourceVendorBillId { get; set; }
            public Guid? SourcePaymentId { get; set; }

            [MaxLength(50)]
            public string? SourceDocumentNumber { get; set; }

            public Guid? SupplierId { get; set; }
            public string? SupplierName { get; set; }
            public Guid? ContractId { get; set; }
            public string? ContractName { get; set; }
            public Guid? ExpenseCategoryId { get; set; }
            public string? ExpenseCategoryName { get; set; }

            [Required(ErrorMessage = "Basis Reference is required")]
            [MaxLength(500)]
            public string BasisReferenceText { get; set; } = string.Empty;

            // 3. Coverage & Release Method
            [Required(ErrorMessage = "Coverage Start Date is required")]
            public DateTime? PrepaymentStartDate { get; set; }

            [Required(ErrorMessage = "Coverage End Date is required")]
            public DateTime? PrepaymentEndDate { get; set; }

            public int CoverageDays => (PrepaymentStartDate.HasValue && PrepaymentEndDate.HasValue)
                ? (int)(PrepaymentEndDate.Value - PrepaymentStartDate.Value).TotalDays : 0;

            [Required(ErrorMessage = "Release Method is required")]
            public ReleaseMethodEnum ReleaseMethod { get; set; } = ReleaseMethodEnum.StraightLine;

            public ReleaseFrequencyEnum? ReleaseFrequency { get; set; }

            [Required(ErrorMessage = "Release Start Date is required")]
            public DateTime? ReleaseStartDate { get; set; }

            public DateTime? ReleaseEndDate { get; set; }
            public bool IsScheduleGenerated { get; set; }
            public bool CoverageProofRequiredFlag { get; set; }

            // 4. Amounts & Balances
            [Required(ErrorMessage = "Currency is required")]
            public Guid? CurrencyId { get; set; }

            public Guid? ExchangeRateId { get; set; }

            [Required(ErrorMessage = "Original Prepaid Amount is required")]
            [Range(0.01, double.MaxValue, ErrorMessage = "Original Prepaid Amount must be > 0")]
            public decimal OriginalPrepaidAmount { get; set; }

            public decimal ReleasedAmountToDate { get; set; }
            public decimal RemainingPrepaidBalance => OriginalPrepaidAmount - ReleasedAmountToDate;
            public decimal CurrentPeriodReleaseAmount { get; set; }
            public decimal? AdjustmentAmount { get; set; }
            public decimal? RoundingDifferenceAmount { get; set; }
            public MaterialityLevelEnum? MaterialityLevel { get; set; }

            // 5. Accounting Context
            [Required(ErrorMessage = "Fiscal Year is required")]
            public Guid? FiscalYearId { get; set; }

            public Guid? CurrentAccountingPeriodId { get; set; }
            public string? CurrentAccountingPeriodName { get; set; }

            [Required(ErrorMessage = "Expense GL Account is required")]
            public Guid? ExpenseGLAccountId { get; set; }
            public string? ExpenseGLAccountName { get; set; }

            [Required(ErrorMessage = "Prepayment Asset GL Account is required")]
            public Guid? PrepaymentAssetGLId { get; set; }
            public string? PrepaymentAssetGLName { get; set; }

            public Guid? JournalEntryId { get; set; }
            public Guid? LastReleaseJournalEntryId { get; set; }

            [Required(ErrorMessage = "Posting Status is required")]
            public PostingStatusEnum PostingStatus { get; set; } = PostingStatusEnum.NotPosted;

            [Required(ErrorMessage = "Release Status is required")]
            public ReleaseStatusEnum ReleaseStatus { get; set; } = ReleaseStatusEnum.NotStarted;

            public DateTime? NextReleaseDueDate { get; set; }
            public DateTime? LastReleaseDate { get; set; }

            // 6. Release Schedule (Child Lines)
            public List<PrepaymentReleaseLine> ReleaseLines { get; set; } = new();

            // 7. Dimensions & Ownership
            public Guid? CostCenterId { get; set; }
            public string? CostCenterName { get; set; }
            public Guid? DepartmentId { get; set; }
            public string? DepartmentName { get; set; }
            public Guid? BranchId { get; set; }
            public string? BranchName { get; set; }
            public Guid? ProjectId { get; set; }
            public string? ProjectName { get; set; }
            public string? ExpenseOwnerUserId { get; set; }
            public string? ExpenseOwnerName { get; set; }

            // 8. Workflow & Governance
            [Required(ErrorMessage = "Prepared By is required")]
            public string? PreparedByUserId { get; set; }

            public DateTime? SubmittedOn { get; set; }
            public string? ReviewedByUserId { get; set; }
            public DateTime? ReviewedOn { get; set; }
            public string? ApprovedByUserId { get; set; }
            public DateTime? ApprovedOn { get; set; }
            public bool IsLocked { get; set; }
            public DateTime? LockedOn { get; set; }
            public string? LockedBy { get; set; }

            [MaxLength(500)]
            public string? CancellationReason { get; set; }

            // 9. Notes & Supporting Evidence
            [MaxLength(1500)]
            public string? AssumptionText { get; set; }

            [MaxLength(1500)]
            public string? FinanceNotes { get; set; }

            public int AttachmentCount { get; set; }
            public bool PolicyExceptionFlag { get; set; }

            [MaxLength(1000)]
            public string? PolicyExceptionReason { get; set; }

            // 10. System Audit
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public string? CreatedBy { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public string? UpdatedBy { get; set; }
            public int RowVersion { get; set; }
            public bool IsDeleted { get; set; }
        }
    }
}
