using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public class ForecastViewModel
    {
        // ── Enums ──────────────────────────────────────────────────────────────

        public enum ForecastStatusEnum
        {
            Draft = 1,
            Generated = 2,
            UnderReview = 3,
            Reviewed = 4,
            Approved = 5,
            Locked = 6,
            Archived = 7,
            Superseded = 8
        }

        public enum ForecastTypeEnum
        {
            RollingForecast = 1,
            RevisedAnnualForecast = 2,
            QuarterlyForecast = 3,
            MonthlyLatestEstimate = 4,
            ScenarioForecast = 5,
            ProjectForecast = 6
        }

        public enum ScenarioTypeEnum
        {
            Base = 1,
            Optimistic = 2,
            Conservative = 3,
            Stretch = 4,
            OfficialLatest = 5
        }

        public enum ForecastLevelEnum
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

        public enum ForecastNatureEnum
        {
            Expense = 1,
            Revenue = 2,
            Capex = 3,
            Mixed = 4
        }

        public enum ForecastTimeModeEnum
        {
            RemainingPeriodsOnly = 1,
            FullYearProjection = 2,
            CurrentQuarterProjection = 3,
            CustomRange = 4
        }

        public enum BaselineReferenceTypeEnum
        {
            Budget = 1,
            RevisedBudget = 2,
            PriorForecast = 3,
            ActualTrend = 4,
            ManualIndependent = 5
        }

        public enum ForecastMethodEnum
        {
            Manual = 1,
            ActualPlusRemainingEstimate = 2,
            TrendBased = 3,
            DriverBased = 4,
            CopyBudgetThenAdjust = 5,
            PercentageUpliftOrReduction = 6
        }

        public enum ConfidenceLevelEnum
        {
            Low = 1,
            Medium = 2,
            High = 3,
            VeryHigh = 4
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

        public enum ForecastDirectionEnum
        {
            Improving = 1,
            Stable = 2,
            Deteriorating = 3,
            Opportunity = 4,
            Risk = 5
        }

        public enum RiskLevelEnum
        {
            Low = 1,
            Medium = 2,
            High = 3,
            Critical = 4
        }

        public enum DriverCategoryEnum
        {
            Trend = 1,
            Volume = 2,
            Rate = 3,
            Seasonality = 4,
            ContractedCommitment = 5,
            ProjectDelay = 6,
            ManagementDecision = 7,
            AllocationImpact = 8,
            Other = 9
        }

        public enum LineReviewStatusEnum
        {
            Draft = 1,
            Reviewed = 2,
            Approved = 3,
            Escalated = 4,
            Closed = 5
        }

        // ── Main Model ─────────────────────────────────────────────────────────

        public class Forecast
        {
            // Section 1: Core Identity (Header)
            public Guid ForecastId { get; set; }
            public Guid TenantId { get; set; }
            public Guid CompanyId { get; set; }

            [Required(ErrorMessage = "Forecast Code is required")]
            [StringLength(30)]
            public string ForecastCode { get; set; } = "";

            [Required(ErrorMessage = "Forecast Name is required")]
            [StringLength(200)]
            public string ForecastName { get; set; } = "";

            [StringLength(1000)]
            public string? Description { get; set; }

            [Required(ErrorMessage = "Status is required")]
            public ForecastStatusEnum ForecastStatus { get; set; } = ForecastStatusEnum.Draft;

            // Section 2: Forecast Classification
            [Required(ErrorMessage = "Forecast Type is required")]
            public ForecastTypeEnum? ForecastType { get; set; }

            [Required(ErrorMessage = "Scenario is required")]
            public ScenarioTypeEnum? ScenarioType { get; set; }

            [Required(ErrorMessage = "Scope Level is required")]
            public ForecastLevelEnum? ForecastLevel { get; set; }

            [Required(ErrorMessage = "Nature is required")]
            public ForecastNatureEnum? ForecastNature { get; set; }

            [Required(ErrorMessage = "Currency is required")]
            public Guid? CurrencyId { get; set; }

            // Section 3: Horizon & Period Scope
            [Required(ErrorMessage = "Fiscal Year is required")]
            public Guid? FiscalYearId { get; set; }

            [Required(ErrorMessage = "Forecast Start Date is required")]
            public DateTime? FromDate { get; set; }

            [Required(ErrorMessage = "Forecast End Date is required")]
            public DateTime? ToDate { get; set; }

            [Required(ErrorMessage = "Time Mode is required")]
            public ForecastTimeModeEnum? ForecastTimeMode { get; set; }

            public Guid? BudgetId { get; set; }
            public Guid? BudgetPeriodId { get; set; }
            public Guid? VarianceAnalysisId { get; set; }
            public Guid? BranchId { get; set; }
            public Guid? DepartmentId { get; set; }
            public Guid? CostCenterId { get; set; }
            public Guid? ProjectId { get; set; }
            public string? ScopeFilterJson { get; set; }

            // Section 4: Baseline & Methodology
            [Required(ErrorMessage = "Baseline Type is required")]
            public BaselineReferenceTypeEnum? BaselineReferenceType { get; set; }

            public string? BaselineReferenceId { get; set; }

            [Required(ErrorMessage = "Forecast Method is required")]
            public ForecastMethodEnum? ForecastMethod { get; set; }

            public string? MethodDetailJson { get; set; }
            public DateTime? ActualAsOfDate { get; set; }
            public bool UseAllocatedValues { get; set; } = true;
            public bool UseCommittedAmounts { get; set; } = false;
            public bool IncludeTaxInForecast { get; set; } = false;
            public ConfidenceLevelEnum? ConfidenceLevel { get; set; }

            // Section 5: Forecast Summary Totals (Derived)
            public decimal BudgetBaselineAmount { get; set; }
            public decimal ActualToDateAmount { get; set; }
            public decimal ForecastRemainingAmount { get; set; }
            public decimal ForecastFullPeriodAmount { get; set; }
            public decimal VarianceVsBudgetAmount { get; set; }
            public decimal VarianceVsBudgetPercent { get; set; }
            public decimal ExpectedOverrunAmount { get; set; }
            public decimal ExpectedUnderrunAmount { get; set; }
            public int TotalForecastLineCount { get; set; }
            public int SignificantForecastRiskLineCount { get; set; }

            // Section 7: Assumptions & Scenario Control
            [StringLength(2000)]
            public string? ForecastAssumptionSummary { get; set; }

            [StringLength(2000)]
            public string? KeyRiskSummary { get; set; }

            [StringLength(2000)]
            public string? KeyOpportunitySummary { get; set; }

            public string? ScenarioComparisonJson { get; set; }
            public bool ManagementAdjustmentFlag { get; set; } = false;

            [StringLength(1000)]
            public string? ManagementAdjustmentReason { get; set; }

            // Section 8: Workflow & Approval
            public DateTime? GeneratedOn { get; set; }
            public Guid? GeneratedBy { get; set; }
            public DateTime? ReviewedOn { get; set; }
            public Guid? ReviewedBy { get; set; }
            public DateTime? ApprovedOn { get; set; }
            public Guid? ApprovedBy { get; set; }
            public bool IsLocked { get; set; } = false;
            public DateTime? LockedOn { get; set; }
            public Guid? LockedBy { get; set; }
            public Guid? SupersededByForecastId { get; set; }

            // Section 9: Notes & Evidence
            [StringLength(1500)]
            public string? Notes { get; set; }
            public int AttachmentCount { get; set; }

            // System Audit Fields
            public DateTime CreatedAt { get; set; }
            public Guid CreatedBy { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public Guid? UpdatedBy { get; set; }
            public byte[]? RowVersion { get; set; }
            public bool IsDeleted { get; set; } = false;

            // Navigation
            public List<ForecastLine> Lines { get; set; } = new();
        }

        // ── Forecast Line ──────────────────────────────────────────────────────

        public class ForecastLine
        {
            public Guid ForecastLineId { get; set; }
            public Guid ForecastId { get; set; }
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

            public decimal BudgetBaselineAmount { get; set; }
            public decimal ActualToDateAmount { get; set; }
            public decimal ForecastRemainingAmount { get; set; }
            public decimal ForecastFullPeriodAmount { get; set; }
            public decimal VarianceVsBudgetAmount { get; set; }
            public decimal VarianceVsBudgetPercent { get; set; }

            public ForecastDirectionEnum? ForecastDirection { get; set; }

            [Required]
            public RiskLevelEnum RiskLevel { get; set; }

            public ConfidenceLevelEnum? ConfidenceLevel { get; set; }

            [StringLength(1500)]
            public string? AssumptionText { get; set; }

            public DriverCategoryEnum? DriverCategory { get; set; }
            public bool ManualOverrideFlag { get; set; } = false;

            [StringLength(500)]
            public string? ManualOverrideReason { get; set; }

            public bool ActionRequiredFlag { get; set; } = false;
            public Guid? ActionOwnerUserId { get; set; }
            public DateTime? ActionDueDate { get; set; }

            [Required]
            public LineReviewStatusEnum LineReviewStatus { get; set; }

            [StringLength(1000)]
            public string? LineNotes { get; set; }
        }
    }
}
