using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public class VarianceAnalysisViewModel
    {
        // ── Enums ──────────────────────────────────────────────────────────────

        public enum AnalysisStatusEnum
        {
            Draft = 1,
            Generated = 2,
            UnderReview = 3,
            Reviewed = 4,
            Approved = 5,
            Locked = 6,
            Archived = 7
        }

        public enum ComparisonModeEnum
        {
            BudgetVsActual = 1,
            BudgetVsForecast = 2,
            ActualVsForecast = 3,
            BudgetVsCommitted = 4,
            RevisedBudgetVsActual = 5
        }

        public enum AnalysisTimeModeEnum
        {
            PeriodOnly = 1,
            YearToDate = 2,
            QuarterToDate = 3,
            FullCycle = 4,
            CustomRange = 5
        }

        public enum AnalysisScopeLevelEnum
        {
            Company = 1,
            Budget = 2,
            BudgetPeriod = 3,
            BudgetLine = 4,
            CostCenter = 5,
            Department = 6,
            Branch = 7,
            Project = 8,
            Category = 9,
            GLAccount = 10
        }

        public enum LineNatureModeEnum
        {
            ExpenseSensitive = 1,
            RevenueSensitive = 2,
            MixedSensitive = 3
        }

        public enum ActualSourceModeEnum
        {
            PostedJournalsOnly = 1,
            AllPostedActuals = 2,
            ActualsPlusAdjustments = 3
        }

        public enum ReferenceTypeEnum
        {
            BudgetLine = 1,
            CostCenter = 2,
            Branch = 3,
            Department = 4,
            Project = 5,
            Category = 6,
            GLAccount = 7,
            Mixed = 8
        }

        public enum LineTypeSnapshotEnum
        {
            Expense = 1,
            Revenue = 2,
            Capex = 3,
            Mixed = 4,
            Statistical = 5
        }

        public enum VarianceDirectionEnum
        {
            Favorable = 1,
            Unfavorable = 2,
            Neutral = 3
        }

        public enum MaterialityLevelEnum
        {
            Low = 1,
            Medium = 2,
            High = 3,
            Critical = 4
        }

        public enum TrendIndicatorEnum
        {
            Improving = 1,
            Stable = 2,
            Worsening = 3,
            NotAvailable = 4
        }

        public enum ExplanationStatusEnum
        {
            NotRequired = 1,
            Pending = 2,
            Provided = 3,
            Reviewed = 4
        }

        public enum RootCauseCategoryEnum
        {
            VolumeChange = 1,
            RateChange = 2,
            TimingDifference = 3,
            PlanningError = 4,
            AllocationImpact = 5,
            OneTimeEvent = 6,
            OperationalIssue = 7,
            RevenueShortfall = 8,
            SavingsRealized = 9,
            Other = 10
        }

        public enum LineReviewStatusEnum
        {
            Open = 1,
            UnderReview = 2,
            Closed = 3,
            Escalated = 4
        }

        // ── Main Model ─────────────────────────────────────────────────────────

        public class VarianceAnalysis
        {
            // Section 1: Core Identity (Header)
            public Guid VarianceAnalysisId { get; set; }
            public Guid TenantId { get; set; }
            public Guid CompanyId { get; set; }

            [Required(ErrorMessage = "Analysis Code is required")]
            [StringLength(30)]
            public string AnalysisCode { get; set; } = "";

            [Required(ErrorMessage = "Analysis Name is required")]
            [StringLength(200)]
            public string AnalysisName { get; set; } = "";

            [StringLength(1000)]
            public string? Description { get; set; }

            [Required(ErrorMessage = "Status is required")]
            public AnalysisStatusEnum AnalysisStatus { get; set; } = AnalysisStatusEnum.Draft;

            // Section 2: Comparison Definition
            [Required(ErrorMessage = "Comparison Mode is required")]
            public ComparisonModeEnum? ComparisonMode { get; set; }

            [Required(ErrorMessage = "Time Mode is required")]
            public AnalysisTimeModeEnum? AnalysisTimeMode { get; set; }

            [Required(ErrorMessage = "Scope Level is required")]
            public AnalysisScopeLevelEnum? AnalysisScopeLevel { get; set; }

            [Required(ErrorMessage = "Nature Basis is required")]
            public LineNatureModeEnum? LineNatureMode { get; set; }

            [Required(ErrorMessage = "Currency is required")]
            public Guid? CurrencyId { get; set; }

            // Section 3: Period & Scope Filters
            [Required(ErrorMessage = "Fiscal Year is required")]
            public Guid? FiscalYearId { get; set; }

            public Guid? BudgetId { get; set; }
            public Guid? BudgetPeriodId { get; set; }
            public Guid? ForecastId { get; set; }
            public Guid? BranchId { get; set; }
            public Guid? DepartmentId { get; set; }
            public Guid? CostCenterId { get; set; }
            public Guid? ProjectId { get; set; }

            [Required(ErrorMessage = "From Date is required")]
            public DateTime? FromDate { get; set; }

            [Required(ErrorMessage = "To Date is required")]
            public DateTime? ToDate { get; set; }

            public string? ScopeFilterJson { get; set; }

            // Section 4: Source Inclusion Rules
            public bool IncludeAllocatedValues { get; set; } = true;
            public bool IncludeCommittedAmounts { get; set; } = false;
            public bool IncludeTaxInActuals { get; set; } = false;
            public bool IncludeClosedPeriodsOnly { get; set; } = false;

            [Required(ErrorMessage = "Actual Source Mode is required")]
            public ActualSourceModeEnum? ActualSourceMode { get; set; }

            public decimal? MaterialityThresholdAmount { get; set; }
            public decimal? MaterialityThresholdPercent { get; set; }
            public bool RequireExplanationAboveThreshold { get; set; } = true;

            // Section 5: Summary Totals (Derived)
            public decimal TotalBudgetAmount { get; set; }
            public decimal TotalActualAmount { get; set; }
            public decimal TotalForecastAmount { get; set; }
            public decimal TotalCommittedAmount { get; set; }
            public decimal TotalVarianceAmount { get; set; }
            public decimal OverallVariancePercent { get; set; }
            public decimal FavorableVarianceAmount { get; set; }
            public decimal UnfavorableVarianceAmount { get; set; }
            public int SignificantVarianceLineCount { get; set; }
            public int TotalLineCount { get; set; }

            // Section 7: Review & Approval
            public DateTime? GeneratedOn { get; set; }
            public Guid? GeneratedBy { get; set; }
            public DateTime? ReviewedOn { get; set; }
            public Guid? ReviewedBy { get; set; }
            public DateTime? ApprovedOn { get; set; }
            public Guid? ApprovedBy { get; set; }
            public bool IsLocked { get; set; } = false;
            public DateTime? LockedOn { get; set; }
            public Guid? LockedBy { get; set; }

            [StringLength(1500)]
            public string? ReviewNotes { get; set; }

            // Section 8: Notes & Evidence
            [StringLength(2000)]
            public string? ManagementCommentary { get; set; }
            public int AttachmentCount { get; set; }

            // Section 9: System Audit Fields
            public DateTime CreatedAt { get; set; }
            public Guid CreatedBy { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public Guid? UpdatedBy { get; set; }
            public byte[]? RowVersion { get; set; }
            public bool IsDeleted { get; set; } = false;

            // Navigation
            public List<VarianceAnalysisLine> Lines { get; set; } = new();
        }

        // ── Variance Analysis Line ─────────────────────────────────────────────

        public class VarianceAnalysisLine
        {
            public Guid VarianceAnalysisLineId { get; set; }
            public Guid VarianceAnalysisId { get; set; }
            public int LineNumber { get; set; }

            [Required]
            public ReferenceTypeEnum ReferenceType { get; set; }

            public string? ReferenceId { get; set; }

            [StringLength(50)]
            public string? ReferenceCodeSnapshot { get; set; }

            [Required]
            [StringLength(200)]
            public string ReferenceNameSnapshot { get; set; } = "";

            [Required]
            public LineTypeSnapshotEnum LineTypeSnapshot { get; set; }

            public decimal BudgetAmount { get; set; }
            public decimal ActualAmount { get; set; }
            public decimal ForecastAmount { get; set; }
            public decimal CommittedAmount { get; set; }
            public decimal VarianceAmount { get; set; }
            public decimal VariancePercent { get; set; }

            [Required]
            public VarianceDirectionEnum VarianceDirection { get; set; }

            [Required]
            public MaterialityLevelEnum MaterialityLevel { get; set; }

            public bool ThresholdBreachFlag { get; set; }
            public TrendIndicatorEnum? TrendIndicator { get; set; }

            [Required]
            public ExplanationStatusEnum ExplanationStatus { get; set; }

            [StringLength(2000)]
            public string? ExplanationText { get; set; }

            public RootCauseCategoryEnum? RootCauseCategory { get; set; }

            [StringLength(1500)]
            public string? CorrectiveActionText { get; set; }

            public Guid? ActionOwnerUserId { get; set; }
            public DateTime? ActionDueDate { get; set; }

            [Required]
            public LineReviewStatusEnum LineReviewStatus { get; set; }

            [StringLength(1000)]
            public string? LineNotes { get; set; }
        }
    }
}
