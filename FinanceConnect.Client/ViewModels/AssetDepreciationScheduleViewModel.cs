using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public class AssetDepreciationScheduleViewModel
    {
        // ── Enums ──────────────────────────────────────────────────────
        public enum ScheduleStatusEnum
        {
            Draft = 1,
            Active = 2,
            Superseded = 3,
            Locked = 4,
            Cancelled = 5
        }

        public enum LineLockStatusEnum
        {
            Open = 1,
            LockedPosted = 2,
            LockedSuperseded = 3
        }

        // ── Schedule Line (child) ──────────────────────────────────────
        public class AssetDepreciationScheduleLine
        {
            public Guid AssetDepreciationScheduleLineId { get; set; } = Guid.NewGuid();

            [Required]
            public Guid AssetDepreciationScheduleId { get; set; }

            [Required]
            public int LineNumber { get; set; }

            [Required]
            public Guid AccountingPeriodId { get; set; }

            public string? PeriodLabel { get; set; }

            [Required]
            public DateTime PeriodStartDate { get; set; }

            [Required]
            public DateTime PeriodEndDate { get; set; }

            [Required]
            [Range(0, double.MaxValue)]
            public decimal PlannedDepreciationAmount { get; set; }

            [Required]
            public decimal PlannedAccumulatedDepreciationAmount { get; set; }

            [Required]
            [Range(0, double.MaxValue)]
            public decimal PlannedNetBookValueAmount { get; set; }

            public bool IsPosted { get; set; } = false;

            public Guid? PostedDepreciationRunLineId { get; set; }

            public DateTime? PostedOn { get; set; }

            public LineLockStatusEnum LockStatus { get; set; } = LineLockStatusEnum.Open;

            [StringLength(300)]
            public string? LineNotes { get; set; }
        }

        // ── Schedule Header ────────────────────────────────────────────
        public class AssetDepreciationSchedule
        {
            // ── Section 1: Schedule Header (Core) ──────────────────────
            public Guid AssetDepreciationScheduleId { get; set; }
            public Guid TenantId { get; set; }
            public Guid CompanyId { get; set; }

            [Required(ErrorMessage = "Fixed Asset is required")]
            public Guid? FixedAssetId { get; set; }

            [StringLength(40)]
            public string ScheduleNumber { get; set; } = "";

            [Required]
            public ScheduleStatusEnum ScheduleStatus { get; set; } = ScheduleStatusEnum.Draft;

            [Required]
            public int ScheduleVersion { get; set; } = 1;

            public DateTime GeneratedOn { get; set; }
            public string? GeneratedBy { get; set; }

            // ── Section 2: Snapshot of Depreciation Inputs ─────────────
            [Required]
            public Guid? DepreciationMethodIdSnapshot { get; set; }

            public string? MethodTypeSnapshot { get; set; }
            public string? InputModeSnapshot { get; set; }
            public decimal? RatePercentSnapshot { get; set; }

            [Required]
            [Range(1, int.MaxValue)]
            public int UsefulLifeMonthsSnapshot { get; set; }

            [Required]
            [Range(0, 100)]
            public decimal ResidualValuePercentSnapshot { get; set; }

            [Required]
            public decimal ResidualValueAmountSnapshot { get; set; }

            public string? DepreciationStartConventionSnapshot { get; set; }

            [Required]
            public DateTime StartDate { get; set; }

            [Required]
            public DateTime EndDate { get; set; }

            [Required]
            public decimal DepreciationBaseAmountSnapshot { get; set; }

            [Required]
            public decimal TotalCapitalizedCostSnapshot { get; set; }

            public Guid? CurrencyId { get; set; }

            [StringLength(500)]
            public string? ScheduleGenerationReason { get; set; }

            // ── Section 3: Posting Progress (Derived) ──────────────────
            public decimal TotalPlannedDepreciationAmount => ScheduleLines?.Sum(l => l.PlannedDepreciationAmount) ?? 0;
            public decimal TotalPostedDepreciationAmount => ScheduleLines?.Where(l => l.IsPosted).Sum(l => l.PlannedDepreciationAmount) ?? 0;
            public Guid? LastPostedPeriodId { get; set; }
            public Guid? NextUnpostedPeriodId { get; set; }
            public bool IsFullyPosted => ScheduleLines?.All(l => l.IsPosted) ?? false;
            public bool IsAssetSuspendedSnapshot { get; set; }

            // ── Section 4: Schedule Lines ──────────────────────────────
            public List<AssetDepreciationScheduleLine> ScheduleLines { get; set; } = new();

            // ── Section 5: Locking & Superseding ───────────────────────
            public Guid? SupersededByScheduleId { get; set; }
            public DateTime? LockedOn { get; set; }
            public string? LockedBy { get; set; }

            [StringLength(300)]
            public string? LockReason { get; set; }

            // ── Section 6: System Audit Fields ─────────────────────────
            public DateTime CreatedAt { get; set; }
            public Guid CreatedBy { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public Guid? UpdatedBy { get; set; }
            public byte[]? RowVersion { get; set; }
            public bool IsDeleted { get; set; } = false;

            // ── Display helpers ────────────────────────────────────────
            public string? AssetCodeDisplay { get; set; }
            public string? AssetNameDisplay { get; set; }
            public string? MethodNameDisplay { get; set; }
        }
    }
}
