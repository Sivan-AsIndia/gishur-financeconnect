using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public class BudgetPeriodViewModel
    {
        // ── Enums ──────────────────────────────────────────────────────────────

        public enum PeriodTypeEnum
        {
            Monthly = 1,
            Quarterly = 2,
            HalfYearly = 3,
            Yearly = 4,
            Custom = 5
        }

        public enum PeriodStatusEnum
        {
            Draft = 1,
            Open = 2,
            Released = 3,
            Locked = 4,
            Closed = 5,
            Revised = 6,
            Archived = 7
        }

        // ── Model ──────────────────────────────────────────────────────────────

        public class BudgetPeriod
        {
            // ─── Section 1: Core Identity ────────────────────────────────────
            public Guid BudgetPeriodId { get; set; } = Guid.NewGuid();
            public Guid TenantId { get; set; }
            public Guid CompanyId { get; set; }

            [Required(ErrorMessage = "Budget is required")]
            public Guid BudgetId { get; set; }

            [Required(ErrorMessage = "Period Sequence is required")]
            [Range(1, int.MaxValue, ErrorMessage = "Period Sequence must be > 0")]
            public int PeriodSequenceNo { get; set; }

            [Required(ErrorMessage = "Period Code is required")]
            [MaxLength(30)]
            public string PeriodCode { get; set; } = string.Empty;

            [Required(ErrorMessage = "Period Name is required")]
            [MaxLength(100)]
            public string PeriodName { get; set; } = string.Empty;

            // ─── Section 2: Period Classification ────────────────────────────
            [Required(ErrorMessage = "Period Type is required")]
            public PeriodTypeEnum PeriodType { get; set; } = PeriodTypeEnum.Monthly;

            public int? FiscalMonthNo { get; set; }
            public int? FiscalQuarterNo { get; set; }
            public int? FiscalHalfNo { get; set; }

            [Required(ErrorMessage = "Fiscal Year is required")]
            public Guid FiscalYearId { get; set; }

            // ─── Section 3: Date Range ───────────────────────────────────────
            [Required(ErrorMessage = "Start Date is required")]
            public DateTime StartDate { get; set; } = DateTime.Today;

            [Required(ErrorMessage = "End Date is required")]
            public DateTime EndDate { get; set; } = DateTime.Today.AddMonths(1).AddDays(-1);

            public int PeriodLengthDays => (EndDate - StartDate).Days + 1;
            public bool IsWithinBudgetRange { get; set; } = true;

            // ─── Section 4: Planned Values ───────────────────────────────────
            [Required(ErrorMessage = "Planned Amount is required")]
            [Range(0, double.MaxValue, ErrorMessage = "Planned Amount must be >= 0")]
            public decimal PlannedBudgetAmount { get; set; }

            public decimal? RevisedBudgetAmount { get; set; }

            public decimal EffectiveBudgetAmount =>
                RevisedBudgetAmount ?? PlannedBudgetAmount;

            public decimal? ReleasedBudgetAmount { get; set; }
            public decimal? ReservedCommitmentAmount { get; set; }

            // ─── Section 5: Actual Consumption & Control ─────────────────────
            public decimal ActualConsumedAmount { get; set; }
            public decimal? CommittedAmount { get; set; }

            public decimal AvailableBalanceAmount =>
                EffectiveBudgetAmount - ActualConsumedAmount - (CommittedAmount ?? 0);

            public decimal UtilizationPercent =>
                EffectiveBudgetAmount > 0
                    ? Math.Round(ActualConsumedAmount / EffectiveBudgetAmount * 100, 2)
                    : 0;

            public bool IsOverspent => ActualConsumedAmount > EffectiveBudgetAmount;

            public decimal OverspendAmount =>
                Math.Max(ActualConsumedAmount - EffectiveBudgetAmount, 0);

            // ─── Section 6: Workflow & Period Control ─────────────────────────
            [Required(ErrorMessage = "Period Status is required")]
            public PeriodStatusEnum PeriodStatus { get; set; } = PeriodStatusEnum.Draft;

            public bool IsLocked { get; set; }
            public bool IsClosed { get; set; }
            public bool OpenForConsumptionFlag { get; set; } = true;

            public DateTime? LockedOn { get; set; }
            public Guid? LockedBy { get; set; }
            public DateTime? ClosedOn { get; set; }
            public Guid? ClosedBy { get; set; }

            [MaxLength(500)]
            public string? RevisionReason { get; set; }

            // ─── Section 7: Linkage to Planning Intelligence ─────────────────
            public Guid? ForecastReferenceId { get; set; }
            public Guid? VarianceAnalysisReferenceId { get; set; }
            public Guid? CarryForwardFromPeriodId { get; set; }
            public Guid? CarryForwardToPeriodId { get; set; }

            // ─── Section 8: Notes & Assumptions ──────────────────────────────
            [MaxLength(1000)]
            public string? PeriodNotes { get; set; }

            [MaxLength(1000)]
            public string? PlanningAssumptionSummary { get; set; }

            public int AttachmentCount { get; set; }

            // ─── Section 9: System Audit Fields ──────────────────────────────
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public Guid? CreatedBy { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public Guid? UpdatedBy { get; set; }
            public bool IsDeleted { get; set; }
        }
    }
}
