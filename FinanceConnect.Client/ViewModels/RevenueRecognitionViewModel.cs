using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public class RevenueRecognitionViewModel
    {
        // ── Enums ──────────────────────────────────────────────────────────────

        public enum RecognitionStatusEnum
        {
            Draft = 1,
            Ready = 2,
            Scheduled = 3,
            InProgress = 4,
            PartiallyRecognized = 5,
            FullyRecognized = 6,
            OnHold = 7,
            Cancelled = 8,
            Closed = 9
        }

        public enum RecognitionMethodEnum
        {
            Immediate = 1,
            Scheduled = 2,
            MilestoneTriggered = 3,
            ManualApprovalRequired = 4,
            DeferredThenRelease = 5
        }

        public enum RecognitionBasisEnum
        {
            PointInTime = 1,
            StraightLineOverTime = 2,
            MilestoneCompletion = 3,
            DeliveryBased = 4,
            ServiceCoveragePeriod = 5,
            CustomRule = 6
        }

        public enum RecognitionFrequencyEnum
        {
            Daily = 1,
            Monthly = 2,
            Quarterly = 3,
            Custom = 4
        }

        public enum RecognitionLineStatusEnum
        {
            Planned = 1,
            Ready = 2,
            Recognized = 3,
            PartiallyRecognized = 4,
            Deferred = 5,
            Skipped = 6,
            Cancelled = 7
        }

        public enum ManualApprovalStatusEnum
        {
            NotRequired = 1,
            Pending = 2,
            Approved = 3,
            Rejected = 4
        }

        // ── Child Model: RevenueRecognitionLine ────────────────────────────────

        public class RevenueRecognitionLine
        {
            public Guid RevenueRecognitionLineId { get; set; } = Guid.NewGuid();
            public Guid RevenueRecognitionId { get; set; }

            [Required(ErrorMessage = "Line Number is required")]
            public int LineNumber { get; set; }

            [Required(ErrorMessage = "Schedule Date is required")]
            public DateTime? ScheduleDate { get; set; }

            public Guid? AccountingPeriodId { get; set; }
            [MaxLength(50)]
            public string? AccountingPeriodReference { get; set; }

            [Required(ErrorMessage = "Scheduled Amount is required")]
            [Range(0, double.MaxValue, ErrorMessage = "Scheduled Amount must be >= 0")]
            public decimal ScheduledAmount { get; set; }

            [Range(0, double.MaxValue, ErrorMessage = "Recognized Amount must be >= 0")]
            public decimal RecognizedAmount { get; set; }

            public decimal RemainingAmount => ScheduledAmount - RecognizedAmount;

            [Required(ErrorMessage = "Line Status is required")]
            public RecognitionLineStatusEnum RecognitionLineStatus { get; set; } = RecognitionLineStatusEnum.Planned;

            [MaxLength(100)]
            public string? MilestoneReference { get; set; }

            [MaxLength(200)]
            public string? TriggerEventReference { get; set; }

            [Required(ErrorMessage = "Manual Approval Status is required")]
            public ManualApprovalStatusEnum ManualApprovalStatus { get; set; } = ManualApprovalStatusEnum.NotRequired;

            public DateTime? RecognizedOn { get; set; }
            public string? RecognizedBy { get; set; }

            [MaxLength(500)]
            public string? RecognitionLineNotes { get; set; }
        }

        // ── Main Model ─────────────────────────────────────────────────────────

        public class RevenueRecognition
        {
            // ─── Section 1: Core Identity (Header) ───────────────────────────
            public Guid RevenueRecognitionId { get; set; } = Guid.NewGuid();
            public Guid TenantId { get; set; }
            public Guid CompanyId { get; set; }

            [Required(ErrorMessage = "Recognition Code is required")]
            [MaxLength(30, ErrorMessage = "Recognition Code cannot exceed 30 characters")]
            public string RecognitionCode { get; set; } = string.Empty;

            [Required(ErrorMessage = "Recognition Name is required")]
            [MaxLength(200, ErrorMessage = "Recognition Name cannot exceed 200 characters")]
            public string RecognitionName { get; set; } = string.Empty;

            [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
            public string? Description { get; set; }

            [Required(ErrorMessage = "Recognition Status is required")]
            public RecognitionStatusEnum RecognitionStatus { get; set; } = RecognitionStatusEnum.Draft;

            // ─── Section 2: Source Revenue Context ───────────────────────────
            [Required(ErrorMessage = "Source Revenue is required")]
            public Guid RevenueId { get; set; }

            [MaxLength(30)]
            public string? RevenueCodeSnapshot { get; set; }

            [MaxLength(200)]
            public string? RevenueNameSnapshot { get; set; }

            [Required(ErrorMessage = "Customer is required")]
            public Guid CustomerId { get; set; }

            [MaxLength(200)]
            public string? CustomerNameSnapshot { get; set; }

            [Required(ErrorMessage = "Source Document Type is required")]
            [MaxLength(50)]
            public string SourceDocumentTypeSnapshot { get; set; } = string.Empty;

            [MaxLength(50)]
            public string? SourceDocumentNumberSnapshot { get; set; }

            [Required(ErrorMessage = "Revenue Type is required")]
            [MaxLength(50)]
            public string RevenueTypeSnapshot { get; set; } = string.Empty;

            [Required(ErrorMessage = "Revenue Nature is required")]
            [MaxLength(50)]
            public string RevenueNatureSnapshot { get; set; } = string.Empty;

            [Required(ErrorMessage = "Source Revenue Amount is required")]
            [Range(0, double.MaxValue, ErrorMessage = "Source Revenue Amount must be >= 0")]
            public decimal SourceGrossRevenueAmount { get; set; }

            [Required(ErrorMessage = "Currency is required")]
            public Guid CurrencyId { get; set; }

            // ─── Section 3: Recognition Method & Basis ───────────────────────
            [Required(ErrorMessage = "Recognition Method is required")]
            public RecognitionMethodEnum RecognitionMethod { get; set; } = RecognitionMethodEnum.Immediate;

            [Required(ErrorMessage = "Recognition Basis is required")]
            public RecognitionBasisEnum RecognitionBasis { get; set; } = RecognitionBasisEnum.PointInTime;

            public DateTime? RecognitionStartDate { get; set; }
            public DateTime? RecognitionEndDate { get; set; }

            public RecognitionFrequencyEnum? RecognitionFrequency { get; set; }

            [MaxLength(50)]
            public string? ScheduleTemplateCode { get; set; }

            public bool MilestoneTriggerRequired { get; set; }
            public bool ManualApprovalRequiredFlag { get; set; }

            public Guid? DeferredRevenueId { get; set; }
            /// <summary>Text reference for DeferredRevenueId — used in UI since DeferredRevenue may not have a browseable screen.</summary>
            [MaxLength(50)]
            public string? DeferredRevenueReference { get; set; }
            public bool IsScheduleGenerated { get; set; }

            // ─── Section 4: Amounts & Balances ───────────────────────────────
            [Required(ErrorMessage = "Total Recognizable Amount is required")]
            [Range(0, double.MaxValue, ErrorMessage = "Total Recognizable Amount must be >= 0")]
            public decimal TotalRecognizableAmount { get; set; }

            public decimal RecognizedAmountToDate { get; set; }
            public decimal CurrentPeriodRecognitionAmount { get; set; }

            public decimal RemainingRecognitionAmount =>
                TotalRecognizableAmount - RecognizedAmountToDate;

            public decimal RecognitionCompletionPercent =>
                TotalRecognizableAmount > 0
                    ? Math.Round((RecognizedAmountToDate / TotalRecognizableAmount) * 100m, 2)
                    : 0m;

            public decimal? AdjustmentAmount { get; set; }
            public decimal? RoundingDifferenceAmount { get; set; }

            // ─── Section 5: Period & Accounting Impact ───────────────────────
            [Required(ErrorMessage = "Fiscal Year is required")]
            public Guid FiscalYearId { get; set; }

            public Guid? CurrentAccountingPeriodId { get; set; }
            /// <summary>Text reference for CurrentAccountingPeriodId — used in UI.</summary>
            [MaxLength(50)]
            public string? CurrentAccountingPeriodReference { get; set; }
            public DateTime? RecognitionPostingDate { get; set; }
            public DateTime? LastRecognitionRunDate { get; set; }
            public DateTime? NextRecognitionDueDate { get; set; }

            public int RecognizedPeriodsCount { get; set; }
            public int PendingPeriodsCount { get; set; }

            // ─── Section 6: Recognition Schedule (Child Lines) ───────────────
            public List<RevenueRecognitionLine> ScheduleLines { get; set; } = new();

            // ─── Section 7: Workflow & Governance ────────────────────────────
            public string? PreparedByUserId { get; set; }
            public string? ReviewedByUserId { get; set; }
            public string? ApprovedByUserId { get; set; }

            public DateTime? PreparedOn { get; set; }
            public DateTime? ReviewedOn { get; set; }
            public DateTime? ApprovedOn { get; set; }

            public bool IsLocked { get; set; }
            public DateTime? LockedOn { get; set; }
            public string? LockedBy { get; set; }

            [MaxLength(500)]
            public string? CancellationReason { get; set; }

            // ─── Section 8: Notes & Evidence ─────────────────────────────────
            [MaxLength(1500)]
            public string? RecognitionAssumptionText { get; set; }

            [MaxLength(1500)]
            public string? Notes { get; set; }

            public int AttachmentCount { get; set; }

            // ─── Section 9: System Audit Fields ──────────────────────────────
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public string? CreatedBy { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public string? UpdatedBy { get; set; }
            public bool IsDeleted { get; set; }
        }
    }
}
