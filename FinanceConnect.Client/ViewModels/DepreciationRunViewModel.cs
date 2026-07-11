using System;
using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{

    public enum DepreciationRunType
    {
        Monthly,
        OnDemand,
        YearEndFinal,
        CatchUp
    }

    public enum DepreciationRunStatus
    {
        Draft,
        Generated,
        Submitted,
        Approved,
        Posted,
        Finalized,
        Cancelled,
        Failed,
        Reversed
    }

    public class DepreciationRunViewModel
    {

        // ==============================
        // CORE IDENTITY
        // ==============================

        [Required]
        public Guid DepreciationRunId { get; set; }

        [Required(ErrorMessage = "Tenant is required.")]
        public Guid TenantId { get; set; }

        [Required(ErrorMessage = "Company is required.")]
        public Guid? CompanyId { get; set; }

        public Guid? BranchId { get; set; }

        [Required]
        [StringLength(40)]
        public string RunNumber { get; set; } = "";

        [Required]
        public DepreciationRunStatus RunStatus { get; set; } = DepreciationRunStatus.Draft;

        [Required]
        public DepreciationRunType RunType { get; set; } = DepreciationRunType.Monthly;



        // ==============================
        // PERIOD & SCOPE
        // ==============================

        [Required(ErrorMessage = "Accounting period is required.")]
        public Guid? AccountingPeriodId { get; set; }

        [StringLength(50)]
        public string AccountingPeriodName { get; set; } = "";

        public DateTime? PeriodStartDateSnapshot { get; set; }

        public DateTime? PeriodEndDateSnapshot { get; set; }

        [Required(ErrorMessage = "As Of Date is required.")]
        public DateTime? AsOfDate { get; set; }

        public bool IncludeSuspendedAssets { get; set; } = false;

        public bool IncludeZeroDepreciationAssets { get; set; } = false;

        public Guid? AssetCategoryFilterId { get; set; }



        // ==============================
        // GENERATION SETTINGS SNAPSHOT
        // ==============================

        [StringLength(50)]
        public string SelectionMode { get; set; } = "FromScheduleLines";

        [StringLength(50)]
        public string ScheduleVersionPolicy { get; set; } = "UseActiveOnly";

        [StringLength(100)]
        public string? RoundingPolicySnapshot { get; set; }

        [StringLength(2000)]
        public string? GenerationSettingsJson { get; set; }

        public DateTime? GeneratedOn { get; set; }

        [StringLength(100)]
        public string? GeneratedBy { get; set; }



        // ==============================
        // METRICS & TOTALS (DERIVED)
        // ==============================

        public int EligibleAssetsCount { get; set; }

        public int GeneratedLineCount { get; set; }

        public int ExcludedAssetCount { get; set; }

        public decimal TotalDepreciationAmount { get; set; }

        public decimal TotalExpenseAmount { get; set; }

        public decimal TotalAccumDepAmount { get; set; }

        public bool HasExceptions { get; set; }

        [StringLength(30)]
        public string ExceptionSeverityLevel { get; set; } = "None";



        // ==============================
        // POSTING EVIDENCE
        // ==============================

        public Guid? JournalEntryId { get; set; }

        public DateTime? PostedOn { get; set; }

        [StringLength(100)]
        public string? PostedBy { get; set; }

        public DateTime? FinalizedOn { get; set; }

        [StringLength(100)]
        public string? FinalizedBy { get; set; }

        public Guid? ReversalJournalEntryId { get; set; }

        [StringLength(300)]
        public string? RunLockReason { get; set; }



        // ==============================
        // NOTES
        // ==============================

        [StringLength(1000)]
        public string? RunNotes { get; set; }

        public int AttachmentCount { get; set; }



        // ==============================
        // SYSTEM AUDIT
        // ==============================

        [Required]
        public DateTime CreatedAt { get; set; }

        [StringLength(100)]
        public string CreatedBy { get; set; } = "system";

        public DateTime? UpdatedAt { get; set; }

        [StringLength(100)]
        public string? UpdatedBy { get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [StringLength(100)]
        public string? DeletedBy { get; set; }



        // ==============================
        // HELPER FLAGS
        // ==============================

        public bool IsDraft => RunStatus == DepreciationRunStatus.Draft;

        public bool IsGenerated => RunStatus == DepreciationRunStatus.Generated;

        public bool IsSubmitted => RunStatus == DepreciationRunStatus.Submitted;

        public bool IsApproved => RunStatus == DepreciationRunStatus.Approved;

        public bool IsPosted => RunStatus == DepreciationRunStatus.Posted;

        public bool IsFinalized => RunStatus == DepreciationRunStatus.Finalized;

        public bool IsCancelled => RunStatus == DepreciationRunStatus.Cancelled;

        public bool IsReversed => RunStatus == DepreciationRunStatus.Reversed;

    }



    public class DepreciationRunStatistics
    {
        public int TotalRuns { get; set; }

        public int DraftRuns { get; set; }

        public int GeneratedRuns { get; set; }

        public int ApprovedRuns { get; set; }

        public int PostedRuns { get; set; }

        public int FinalizedRuns { get; set; }

        public decimal TotalDepreciationAmount { get; set; }
    }

}