using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public class DeferredRevenueViewModel
    {
        // ── Enums ──────────────────────────────────────────────────────────────

        public enum DeferredRevenueStatusEnum
        {
            Draft = 1, Submitted = 2, Approved = 3, Posted = 4,
            InProgress = 5, PartiallyReleased = 6, FullyReleased = 7,
            Cancelled = 8, Closed = 9
        }

        public enum SourceTypeEnum
        {
            Revenue = 1, CustomerInvoice = 2, Receipt = 3, Contract = 4,
            Subscription = 5, ManualFinanceAdjustment = 6, Other = 7
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

        // ── Child Model: DeferredRevenueReleaseLine ────────────────────────────

        public class DeferredRevenueReleaseLine
        {
            public Guid DeferredRevenueReleaseLineId { get; set; } = Guid.NewGuid();
            public Guid DeferredRevenueId { get; set; }

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
            public Guid? LinkedRevenueRecognitionLineId { get; set; }

            [MaxLength(500)]
            public string? LineNotes { get; set; }
        }

        // ── Model ──────────────────────────────────────────────────────────────

        public class DeferredRevenue
        {
            // 1. Core Identity (Header)
            public Guid DeferredRevenueId { get; set; } = Guid.NewGuid();
            public Guid TenantId { get; set; }
            public Guid CompanyId { get; set; }

            [Required(ErrorMessage = "Deferred Revenue Code is required")]
            [MaxLength(30)]
            public string DeferredRevenueCode { get; set; } = string.Empty;

            [Required(ErrorMessage = "Deferred Revenue Title is required")]
            [MaxLength(200)]
            public string DeferredRevenueTitle { get; set; } = string.Empty;

            [MaxLength(1000)]
            public string? Description { get; set; }

            [Required(ErrorMessage = "Status is required")]
            public DeferredRevenueStatusEnum DeferredRevenueStatus { get; set; } = DeferredRevenueStatusEnum.Draft;

            // 2. Source Context
            [Required(ErrorMessage = "Source Type is required")]
            public SourceTypeEnum SourceType { get; set; } = SourceTypeEnum.Revenue;

            public Guid? SourceRevenueId { get; set; }
            public Guid? SourceInvoiceId { get; set; }
            public Guid? SourceReceiptId { get; set; }

            [MaxLength(50)]
            public string? SourceDocumentNumber { get; set; }

            [Required(ErrorMessage = "Customer is required")]
            public Guid? CustomerId { get; set; }
            public string? CustomerName { get; set; }

            public Guid? ContractId { get; set; }
            public string? ContractName { get; set; }
            public Guid? SubscriptionId { get; set; }
            public string? SubscriptionName { get; set; }

            [MaxLength(50)]
            public string? RevenueCategoryCode { get; set; }

            [Required(ErrorMessage = "Basis Reference is required")]
            [MaxLength(500)]
            public string BasisReferenceText { get; set; } = string.Empty;

            // 3. Coverage & Release Method
            [Required(ErrorMessage = "Coverage Start Date is required")]
            public DateTime? DeferredStartDate { get; set; }

            [Required(ErrorMessage = "Coverage End Date is required")]
            public DateTime? DeferredEndDate { get; set; }

            public int CoverageDays => (DeferredStartDate.HasValue && DeferredEndDate.HasValue)
                ? (int)(DeferredEndDate.Value - DeferredStartDate.Value).TotalDays : 0;

            [Required(ErrorMessage = "Release Method is required")]
            public ReleaseMethodEnum ReleaseMethod { get; set; } = ReleaseMethodEnum.StraightLine;

            public ReleaseFrequencyEnum? ReleaseFrequency { get; set; }

            [Required(ErrorMessage = "Release Start Date is required")]
            public DateTime? ReleaseStartDate { get; set; }

            public DateTime? ReleaseEndDate { get; set; }
            public bool IsScheduleGenerated { get; set; }
            public bool ServiceProofRequiredFlag { get; set; }

            // 4. Amounts & Balances
            [Required(ErrorMessage = "Currency is required")]
            public Guid? CurrencyId { get; set; }

            public Guid? ExchangeRateId { get; set; }

            [Required(ErrorMessage = "Original Deferred Amount is required")]
            [Range(0.01, double.MaxValue, ErrorMessage = "Original Deferred Amount must be > 0")]
            public decimal OriginalDeferredAmount { get; set; }

            public decimal ReleasedToRevenueAmount { get; set; }
            public decimal RemainingDeferredBalance => OriginalDeferredAmount - ReleasedToRevenueAmount;
            public decimal CurrentPeriodReleaseAmount { get; set; }
            public decimal? AdjustmentAmount { get; set; }
            public decimal? RoundingDifferenceAmount { get; set; }
            public MaterialityLevelEnum? MaterialityLevel { get; set; }

            // 5. Accounting Context
            [Required(ErrorMessage = "Fiscal Year is required")]
            public Guid? FiscalYearId { get; set; }

            public Guid? CurrentAccountingPeriodId { get; set; }
            public string? CurrentAccountingPeriodName { get; set; }

            [Required(ErrorMessage = "Revenue GL Account is required")]
            public Guid? RevenueGLAccountId { get; set; }
            public string? RevenueGLAccountName { get; set; }

            [Required(ErrorMessage = "Deferred Revenue Liability GL Account is required")]
            public Guid? DeferredRevenueLiabilityGLId { get; set; }
            public string? DeferredRevenueLiabilityGLName { get; set; }

            public Guid? JournalEntryId { get; set; }
            public Guid? LastReleaseJournalEntryId { get; set; }

            [Required(ErrorMessage = "Posting Status is required")]
            public PostingStatusEnum PostingStatus { get; set; } = PostingStatusEnum.NotPosted;

            [Required(ErrorMessage = "Release Status is required")]
            public ReleaseStatusEnum ReleaseStatus { get; set; } = ReleaseStatusEnum.NotStarted;

            public DateTime? NextReleaseDueDate { get; set; }
            public DateTime? LastReleaseDate { get; set; }
            public Guid? RevenueRecognitionId { get; set; }

            // 6. Release Schedule (Child Lines)
            public List<DeferredRevenueReleaseLine> ReleaseLines { get; set; } = new();

            // 7. Dimensions & Ownership
            public Guid? CostCenterId { get; set; }
            public string? CostCenterName { get; set; }
            public Guid? DepartmentId { get; set; }
            public string? DepartmentName { get; set; }
            public Guid? BranchId { get; set; }
            public string? BranchName { get; set; }
            public Guid? ProjectId { get; set; }
            public string? ProjectName { get; set; }
            public string? RevenueOwnerUserId { get; set; }
            public string? RevenueOwnerName { get; set; }

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
