using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public class FinancialPlanViewModel
    {
        // ── Enums ──────────────────────────────────────────────────────────────

        public enum PlanStatusEnum
        {
            Draft = 1,
            UnderPreparation = 2,
            UnderReview = 3,
            Approved = 4,
            Locked = 5,
            Superseded = 6,
            Archived = 7
        }

        public enum PlanTypeEnum
        {
            AnnualStrategicPlan = 1,
            MidYearRevision = 2,
            MultiYearPlan = 3,
            ScenarioPlan = 4,
            RollingStrategicPlan = 5
        }

        public enum ScenarioTypeEnum
        {
            Base = 1,
            Optimistic = 2,
            Conservative = 3,
            Stretch = 4,
            OfficialApprovedStrategy = 5
        }

        public enum PlanNatureEnum
        {
            RevenueFocused = 1,
            CostFocused = 2,
            ProfitabilityFocused = 3,
            GrowthFocused = 4,
            Mixed = 5
        }

        public enum PlanningScopeLevelEnum
        {
            Company = 1,
            BusinessUnit = 2,
            Region = 3,
            BranchCluster = 4,
            Group = 5
        }

        public enum PlanHorizonModeEnum
        {
            OneYear = 1,
            MultiYear = 2,
            CustomRange = 3
        }

        public enum BudgetTranslationStatusEnum
        {
            NotStarted = 1,
            InProgress = 2,
            Completed = 3,
            PartiallyCompleted = 4
        }

        // ── Main Model ─────────────────────────────────────────────────────────

        public class FinancialPlan
        {
            // Section 1: Core Identity (Header)
            public Guid FinancialPlanId { get; set; }
            public Guid TenantId { get; set; }
            public Guid CompanyId { get; set; }

            [Required(ErrorMessage = "Financial Plan Code is required")]
            [StringLength(30)]
            public string PlanCode { get; set; } = "";

            [Required(ErrorMessage = "Financial Plan Name is required")]
            [StringLength(200)]
            public string PlanName { get; set; } = "";

            [StringLength(1500)]
            public string? Description { get; set; }

            [Required(ErrorMessage = "Status is required")]
            public PlanStatusEnum PlanStatus { get; set; } = PlanStatusEnum.Draft;

            // Section 2: Planning Classification
            [Required(ErrorMessage = "Plan Type is required")]
            public PlanTypeEnum? PlanType { get; set; }

            [Required(ErrorMessage = "Scenario is required")]
            public ScenarioTypeEnum? ScenarioType { get; set; }

            [Required(ErrorMessage = "Nature is required")]
            public PlanNatureEnum? PlanNature { get; set; }

            [Required(ErrorMessage = "Scope Level is required")]
            public PlanningScopeLevelEnum? PlanningScopeLevel { get; set; }

            [Required(ErrorMessage = "Currency is required")]
            public Guid? CurrencyId { get; set; }

            // Section 3: Horizon & Scope
            public Guid? FiscalYearId { get; set; }

            [Required(ErrorMessage = "Plan Start Date is required")]
            public DateTime? FromDate { get; set; }

            [Required(ErrorMessage = "Plan End Date is required")]
            public DateTime? ToDate { get; set; }

            [Required(ErrorMessage = "Horizon Mode is required")]
            public PlanHorizonModeEnum? PlanHorizonMode { get; set; }

            [StringLength(50)]
            public string? BusinessUnitCode { get; set; }

            [StringLength(50)]
            public string? RegionCode { get; set; }

            [StringLength(50)]
            public string? BranchGroupCode { get; set; }

            public string? ScopeFilterJson { get; set; }

            // Section 4: Strategic Financial Targets
            public decimal? TargetRevenueAmount { get; set; }
            public decimal? TargetExpenseAmount { get; set; }
            public decimal? TargetGrossProfitAmount { get; set; }
            public decimal? TargetOperatingProfitAmount { get; set; }
            public decimal? TargetNetProfitAmount { get; set; }
            public decimal? TargetCapexAmount { get; set; }
            public decimal? TargetCashPositionAmount { get; set; }
            public decimal? TargetWorkingCapitalAmount { get; set; }
            public decimal? TargetGrowthPercent { get; set; }
            public decimal? TargetEBITDAPercent { get; set; }
            public decimal? TargetMarginPercent { get; set; }
            public int? TargetHeadcount { get; set; }
            public decimal? TargetInvestmentAmount { get; set; }

            // Section 5: Strategic Assumptions
            [StringLength(2000)]
            public string? RevenueAssumptionText { get; set; }

            [StringLength(2000)]
            public string? ExpenseAssumptionText { get; set; }

            [StringLength(2000)]
            public string? CapexAssumptionText { get; set; }

            [StringLength(2000)]
            public string? MarketAssumptionText { get; set; }

            [StringLength(2000)]
            public string? RiskAssumptionText { get; set; }

            [StringLength(2000)]
            public string? OpportunityAssumptionText { get; set; }

            [StringLength(3000)]
            public string? StrategicNarrative { get; set; }

            public string? AssumptionJson { get; set; }

            // Section 6: Versioning & Governance
            [Required(ErrorMessage = "Version Number is required")]
            [Range(1, int.MaxValue)]
            public int VersionNumber { get; set; } = 1;

            [Required]
            [Range(0, int.MaxValue)]
            public int RevisionNumber { get; set; } = 0;

            public Guid? PreviousFinancialPlanId { get; set; }
            public bool IsOfficialApprovedVersion { get; set; } = false;
            public Guid? SupersededByPlanId { get; set; }

            [StringLength(1500)]
            public string? RevisionReason { get; set; }

            public bool IsLocked { get; set; } = false;
            public DateTime? LockedOn { get; set; }
            public Guid? LockedBy { get; set; }

            // Section 7: Linked Operational Planning
            public int LinkedBudgetCount { get; set; }
            public int LinkedForecastCount { get; set; }
            public BudgetTranslationStatusEnum? BudgetTranslationStatus { get; set; }
            public Guid? LatestForecastReferenceId { get; set; }
            public bool StrategicBaselineVarianceFlag { get; set; } = false;

            // Section 8: Workflow & Approval
            public Guid? PreparedByUserId { get; set; }
            public Guid? ReviewedByUserId { get; set; }
            public Guid? ApprovedByUserId { get; set; }
            public DateTime? PreparedOn { get; set; }
            public DateTime? ReviewedOn { get; set; }
            public DateTime? ApprovedOn { get; set; }

            [StringLength(200)]
            public string? BoardApprovalReference { get; set; }

            [StringLength(1500)]
            public string? ApprovalNotes { get; set; }

            // Section 9: Notes & Evidence
            [StringLength(2000)]
            public string? ManagementNotes { get; set; }
            public int AttachmentCount { get; set; }

            // System Audit Fields
            public DateTime CreatedAt { get; set; }
            public Guid CreatedBy { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public Guid? UpdatedBy { get; set; }
            public byte[]? RowVersion { get; set; }
            public bool IsDeleted { get; set; } = false;
        }

        // ── List DTO ───────────────────────────────────────────────────────────

        public class FinancialPlanListDto
        {
            public Guid FinancialPlanId { get; set; }
            public string PlanCode { get; set; } = "";
            public string PlanName { get; set; } = "";
            public PlanStatusEnum PlanStatus { get; set; }
            public PlanTypeEnum? PlanType { get; set; }
            public ScenarioTypeEnum? ScenarioType { get; set; }
            public PlanNatureEnum? PlanNature { get; set; }
            public PlanningScopeLevelEnum? PlanningScopeLevel { get; set; }
            public PlanHorizonModeEnum? PlanHorizonMode { get; set; }
            public DateTime? FromDate { get; set; }
            public DateTime? ToDate { get; set; }
            public decimal? TargetRevenueAmount { get; set; }
            public decimal? TargetExpenseAmount { get; set; }
            public decimal? TargetNetProfitAmount { get; set; }
            public decimal? TargetCapexAmount { get; set; }
            public int VersionNumber { get; set; }
            public int RevisionNumber { get; set; }
            public bool IsOfficialApprovedVersion { get; set; }
            public bool IsLocked { get; set; }
            public int LinkedBudgetCount { get; set; }
            public int LinkedForecastCount { get; set; }
            public string? Description { get; set; }
            public string? StrategicNarrative { get; set; }
            public string? RevenueAssumptionText { get; set; }
            public string? ExpenseAssumptionText { get; set; }
            public string? CapexAssumptionText { get; set; }
            public string? MarketAssumptionText { get; set; }
            public string? RiskAssumptionText { get; set; }
            public string? OpportunityAssumptionText { get; set; }
            public decimal? TargetGrossProfitAmount { get; set; }
            public decimal? TargetOperatingProfitAmount { get; set; }
            public decimal? TargetCashPositionAmount { get; set; }
            public decimal? TargetWorkingCapitalAmount { get; set; }
            public decimal? TargetGrowthPercent { get; set; }
            public decimal? TargetEBITDAPercent { get; set; }
            public decimal? TargetMarginPercent { get; set; }
            public int? TargetHeadcount { get; set; }
            public decimal? TargetInvestmentAmount { get; set; }
            public Guid? CurrencyId { get; set; }
            public Guid? FiscalYearId { get; set; }
            public string? BusinessUnitCode { get; set; }
            public string? RegionCode { get; set; }
            public string? BranchGroupCode { get; set; }
            public string? RevisionReason { get; set; }
            public BudgetTranslationStatusEnum? BudgetTranslationStatus { get; set; }
            public bool StrategicBaselineVarianceFlag { get; set; }
            public string? BoardApprovalReference { get; set; }
            public string? ApprovalNotes { get; set; }
            public string? ManagementNotes { get; set; }
            public Guid? PreparedByUserId { get; set; }
            public DateTime? PreparedOn { get; set; }
            public DateTime? ReviewedOn { get; set; }
            public DateTime? ApprovedOn { get; set; }
            public Guid? CompanyId { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public bool IsDeleted { get; set; }
        }
    }
}
