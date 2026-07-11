using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{

    public enum DepreciationRunLineStatus
    {
        Generated,
        Excluded,
        Error,
        Posted,
        Reversed
    }

    public enum DepreciationRunLineExclusionReason
    {
        Suspended,
        Disposed,
        NotInService,
        FullyDepreciated,
        MissingSchedule,
        PolicyBlocked,
        ManualExclusion
    }

    public class DepreciationRunLineViewModel
    {

        // CORE IDENTITY

        [Required]
        public Guid DepreciationRunLineId { get; set; }

        [Required]
        public Guid TenantId { get; set; }

        [Required]
        public Guid? CompanyId { get; set; }

        [Required]
        public Guid DepreciationRunId { get; set; }

        [Required]
        public int LineNumber { get; set; }


        // ASSET SNAPSHOT

        [Required]
        public Guid? FixedAssetId { get; set; }

        [Required]
        [StringLength(40)]
        public string AssetNumberSnapshot { get; set; } = "";

        [Required]
        [StringLength(200)]
        public string AssetNameSnapshot { get; set; } = "";

        public Guid? AssetCategoryIdSnapshot { get; set; }

        [StringLength(30)]
        public string CategoryCodeSnapshot { get; set; } = "";

        public Guid? BranchIdSnapshot { get; set; }

        public Guid? CostCenterIdSnapshot { get; set; }


        // PERIOD & SCHEDULE

        [Required]
        public Guid AccountingPeriodId { get; set; }

        [Required]
        public Guid ScheduleId { get; set; }

        [Required]
        public Guid ScheduleLineId { get; set; }

        [Required]
        public int ScheduleVersionSnapshot { get; set; }


        // AMOUNTS

        [Required]
        public decimal DepreciationBaseCostSnapshot { get; set; }

        [Required]
        public decimal? NBVBeforeAmountSnapshot { get; set; }

        [Required]
        public decimal PlannedDepreciationAmount { get; set; }

        [Required]
        public decimal ActualDepreciationAmount { get; set; }

        [Required]
        public decimal? NBVAfterAmountSnapshot { get; set; }

        [Required]
        public decimal ResidualValueAmountSnapshot { get; set; }

        [Required]
        public Guid CurrencyId { get; set; }

        public decimal RoundingDifferenceAmount { get; set; }


        // METHOD SNAPSHOT

        [Required]
        public Guid DepreciationMethodIdSnapshot { get; set; }

        [Required]
        public string MethodTypeSnapshot { get; set; } = "";

        public string InputModeSnapshot { get; set; } = "";

        public decimal? RatePercentSnapshot { get; set; }

        public int UsefulLifeMonthsSnapshot { get; set; }

        public string StartConventionSnapshot { get; set; } = "";

        public string? CalculationBasisSnapshotJson { get; set; }


        // GL POSTING SNAPSHOT

        [Required]
        public Guid DepreciationExpenseGLAccountIdSnapshot { get; set; }

        [Required]
        public Guid AccumulatedDepreciationGLAccountIdSnapshot { get; set; }

        public Guid? JournalEntryId { get; set; }

        [StringLength(50)]
        public string? GeneralLedgerBatchRef { get; set; }


        // ================================
        // STATUS & EXCEPTIONS
        // ================================

        [Required]
        public DepreciationRunLineStatus LineStatus { get; set; }

        public DepreciationRunLineExclusionReason? ExclusionReasonCode { get; set; }

        [StringLength(500)]
        public string? ExclusionReasonText { get; set; }

        [StringLength(50)]
        public string? ErrorCode { get; set; }

        [StringLength(1000)]
        public string? ErrorMessage { get; set; }

        public DateTime? ResolvedOn { get; set; }

        [StringLength(100)]
        public string? ResolvedBy { get; set; }

        public bool IsManuallyAdjusted { get; set; }

        [StringLength(500)]
        public string? ManualAdjustmentReason { get; set; }


        // POSTING & REVERSAL

        public bool IsLocked { get; set; }

        public DateTime? PostedOn { get; set; }

        [StringLength(100)]
        public string? PostedBy { get; set; }

        public DateTime? ReversedOn { get; set; }

        [StringLength(100)]
        public string? ReversedBy { get; set; }


        // SYSTEM AUDIT

        [Required]
        public DateTime CreatedAt { get; set; }

        [StringLength(100)]
        public string CreatedBy { get; set; } = "system";

        public DateTime? UpdatedAt { get; set; }

        [StringLength(100)]
        public string? UpdatedBy { get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        public bool IsDeleted { get; set; }


        // HELPER PROPERTIES

        public bool IsGenerated => LineStatus == DepreciationRunLineStatus.Generated;
        public bool IsPosted => LineStatus == DepreciationRunLineStatus.Posted;
        public bool IsExcluded => LineStatus == DepreciationRunLineStatus.Excluded;
        public bool IsError => LineStatus == DepreciationRunLineStatus.Error;
        public bool IsReversed => LineStatus == DepreciationRunLineStatus.Reversed;

    }
}