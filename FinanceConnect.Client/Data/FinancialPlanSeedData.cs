using static FinanceConnect.Client.ViewModels.FinancialPlanViewModel;

namespace FinanceConnect.Client.Data
{
    public static class FinancialPlanSeedData
    {
        private static readonly Guid CompanyId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        private static readonly Guid TenantId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

        public static List<FinancialPlanListDto> GetAll()
        {
            return new List<FinancialPlanListDto>
            {
                new() {
                    FinancialPlanId = Guid.Parse("80000000-0000-0000-0000-000000000001"),
                    CompanyId = CompanyId,
                    PlanCode = "FINPLAN-2026-001", PlanName = "FY 2026 Strategic Financial Plan",
                    Description = "Annual strategic plan approved by leadership for FY 2026.",
                    PlanStatus = PlanStatusEnum.Approved, PlanType = PlanTypeEnum.AnnualStrategicPlan,
                    ScenarioType = ScenarioTypeEnum.OfficialApprovedStrategy,
                    PlanNature = PlanNatureEnum.Mixed, PlanningScopeLevel = PlanningScopeLevelEnum.Company,
                    PlanHorizonMode = PlanHorizonModeEnum.OneYear,
                    FromDate = new DateTime(2026, 4, 1), ToDate = new DateTime(2027, 3, 31),
                    TargetRevenueAmount = 25000000, TargetExpenseAmount = 18000000,
                    TargetNetProfitAmount = 4000000, TargetCapexAmount = 2000000,
                    TargetGrowthPercent = 18, TargetEBITDAPercent = 22, TargetMarginPercent = 16,
                    TargetHeadcount = 250,
                    VersionNumber = 1, RevisionNumber = 0, IsOfficialApprovedVersion = true,
                    IsLocked = true, LinkedBudgetCount = 5, LinkedForecastCount = 3,
                    BudgetTranslationStatus = BudgetTranslationStatusEnum.Completed,
                    RevenueAssumptionText = "Revenue growth driven by regional expansion in South India.",
                    StrategicNarrative = "Focus on profitability through operational efficiency.",
                    ApprovedOn = new DateTime(2026, 3, 25), PreparedOn = new DateTime(2026, 3, 1),
                    CreatedAt = new DateTime(2026, 2, 15)
                },
                new() {
                    FinancialPlanId = Guid.Parse("80000000-0000-0000-0000-000000000002"),
                    CompanyId = CompanyId,
                    PlanCode = "FINPLAN-2026-MYR-001", PlanName = "Mid-Year Revised Financial Plan FY 2026",
                    PlanStatus = PlanStatusEnum.UnderReview, PlanType = PlanTypeEnum.MidYearRevision,
                    ScenarioType = ScenarioTypeEnum.Base, PlanNature = PlanNatureEnum.Mixed,
                    PlanningScopeLevel = PlanningScopeLevelEnum.Company,
                    PlanHorizonMode = PlanHorizonModeEnum.OneYear,
                    FromDate = new DateTime(2026, 4, 1), ToDate = new DateTime(2027, 3, 31),
                    TargetRevenueAmount = 23000000, TargetExpenseAmount = 17500000,
                    TargetNetProfitAmount = 3200000, TargetCapexAmount = 1800000,
                    VersionNumber = 2, RevisionNumber = 1,
                    RevisionReason = "Market conditions revised due to economic slowdown.",
                    LinkedBudgetCount = 5, LinkedForecastCount = 2,
                    BudgetTranslationStatus = BudgetTranslationStatusEnum.InProgress,
                    CreatedAt = new DateTime(2026, 10, 1)
                },
                new() {
                    FinancialPlanId = Guid.Parse("80000000-0000-0000-0000-000000000003"),
                    CompanyId = CompanyId,
                    PlanCode = "STRATPLAN-FY26-27", PlanName = "Growth & Profitability Plan FY 2026-28",
                    PlanStatus = PlanStatusEnum.Locked, PlanType = PlanTypeEnum.MultiYearPlan,
                    ScenarioType = ScenarioTypeEnum.OfficialApprovedStrategy,
                    PlanNature = PlanNatureEnum.GrowthFocused,
                    PlanningScopeLevel = PlanningScopeLevelEnum.Group,
                    PlanHorizonMode = PlanHorizonModeEnum.MultiYear,
                    FromDate = new DateTime(2026, 4, 1), ToDate = new DateTime(2029, 3, 31),
                    TargetRevenueAmount = 100000000, TargetExpenseAmount = 70000000,
                    TargetNetProfitAmount = 18000000, TargetCapexAmount = 10000000,
                    TargetGrowthPercent = 25, TargetHeadcount = 500,
                    TargetInvestmentAmount = 15000000,
                    VersionNumber = 1, RevisionNumber = 0, IsOfficialApprovedVersion = true,
                    IsLocked = true, LinkedBudgetCount = 12,
                    StrategicNarrative = "Three-year growth trajectory with focus on market expansion.",
                    ApprovedOn = new DateTime(2026, 2, 28),
                    CreatedAt = new DateTime(2026, 1, 10)
                },
                new() {
                    FinancialPlanId = Guid.Parse("80000000-0000-0000-0000-000000000004"),
                    CompanyId = CompanyId,
                    PlanCode = "FINPLAN-2026-OPTIM", PlanName = "FY 2026 Optimistic Scenario Plan",
                    PlanStatus = PlanStatusEnum.Approved, PlanType = PlanTypeEnum.ScenarioPlan,
                    ScenarioType = ScenarioTypeEnum.Optimistic, PlanNature = PlanNatureEnum.RevenueFocused,
                    PlanningScopeLevel = PlanningScopeLevelEnum.Company,
                    PlanHorizonMode = PlanHorizonModeEnum.OneYear,
                    FromDate = new DateTime(2026, 4, 1), ToDate = new DateTime(2027, 3, 31),
                    TargetRevenueAmount = 30000000, TargetNetProfitAmount = 6000000,
                    TargetGrowthPercent = 25,
                    VersionNumber = 1, RevisionNumber = 0,
                    RevenueAssumptionText = "New product line captures 15% market share.",
                    ApprovedOn = new DateTime(2026, 3, 20),
                    CreatedAt = new DateTime(2026, 3, 1)
                },
                new() {
                    FinancialPlanId = Guid.Parse("80000000-0000-0000-0000-000000000005"),
                    CompanyId = CompanyId,
                    PlanCode = "FINPLAN-2026-CONSV", PlanName = "FY 2026 Conservative Scenario Plan",
                    PlanStatus = PlanStatusEnum.Approved, PlanType = PlanTypeEnum.ScenarioPlan,
                    ScenarioType = ScenarioTypeEnum.Conservative, PlanNature = PlanNatureEnum.CostFocused,
                    PlanningScopeLevel = PlanningScopeLevelEnum.Company,
                    PlanHorizonMode = PlanHorizonModeEnum.OneYear,
                    FromDate = new DateTime(2026, 4, 1), ToDate = new DateTime(2027, 3, 31),
                    TargetRevenueAmount = 20000000, TargetExpenseAmount = 16000000,
                    TargetNetProfitAmount = 2000000, TargetGrowthPercent = 8,
                    VersionNumber = 1, RevisionNumber = 0,
                    RiskAssumptionText = "Market contraction and delayed customer orders.",
                    ApprovedOn = new DateTime(2026, 3, 20),
                    CreatedAt = new DateTime(2026, 3, 1)
                },
                new() {
                    FinancialPlanId = Guid.Parse("80000000-0000-0000-0000-000000000006"),
                    CompanyId = CompanyId,
                    PlanCode = "FINPLAN-2026-BU-SOUTH", PlanName = "South India Business Unit Plan FY 2026",
                    PlanStatus = PlanStatusEnum.Approved, PlanType = PlanTypeEnum.AnnualStrategicPlan,
                    ScenarioType = ScenarioTypeEnum.Base, PlanNature = PlanNatureEnum.ProfitabilityFocused,
                    PlanningScopeLevel = PlanningScopeLevelEnum.BusinessUnit,
                    PlanHorizonMode = PlanHorizonModeEnum.OneYear,
                    FromDate = new DateTime(2026, 4, 1), ToDate = new DateTime(2027, 3, 31),
                    BusinessUnitCode = "BU-SOUTH",
                    TargetRevenueAmount = 12000000, TargetExpenseAmount = 8500000,
                    TargetNetProfitAmount = 2000000,
                    VersionNumber = 1, RevisionNumber = 0, LinkedBudgetCount = 3,
                    ApprovedOn = new DateTime(2026, 3, 28),
                    CreatedAt = new DateTime(2026, 3, 10)
                },
                new() {
                    FinancialPlanId = Guid.Parse("80000000-0000-0000-0000-000000000007"),
                    CompanyId = CompanyId,
                    PlanCode = "FINPLAN-2026-REG-WEST", PlanName = "West Region Strategic Plan FY 2026",
                    PlanStatus = PlanStatusEnum.Draft, PlanType = PlanTypeEnum.AnnualStrategicPlan,
                    ScenarioType = ScenarioTypeEnum.Base, PlanNature = PlanNatureEnum.RevenueFocused,
                    PlanningScopeLevel = PlanningScopeLevelEnum.Region,
                    PlanHorizonMode = PlanHorizonModeEnum.OneYear,
                    FromDate = new DateTime(2026, 4, 1), ToDate = new DateTime(2027, 3, 31),
                    RegionCode = "REG-WEST",
                    TargetRevenueAmount = 8000000,
                    VersionNumber = 1, RevisionNumber = 0,
                    CreatedAt = new DateTime(2026, 3, 20)
                },
                new() {
                    FinancialPlanId = Guid.Parse("80000000-0000-0000-0000-000000000008"),
                    CompanyId = CompanyId,
                    PlanCode = "FINPLAN-2026-ROLLING", PlanName = "Rolling Strategic Plan — Updated May 2026",
                    PlanStatus = PlanStatusEnum.UnderPreparation, PlanType = PlanTypeEnum.RollingStrategicPlan,
                    ScenarioType = ScenarioTypeEnum.Base, PlanNature = PlanNatureEnum.Mixed,
                    PlanningScopeLevel = PlanningScopeLevelEnum.Company,
                    PlanHorizonMode = PlanHorizonModeEnum.MultiYear,
                    FromDate = new DateTime(2026, 5, 1), ToDate = new DateTime(2028, 4, 30),
                    TargetRevenueAmount = 55000000, TargetExpenseAmount = 40000000,
                    VersionNumber = 3, RevisionNumber = 2,
                    CreatedAt = new DateTime(2026, 5, 1)
                },
                new() {
                    FinancialPlanId = Guid.Parse("80000000-0000-0000-0000-000000000009"),
                    CompanyId = CompanyId,
                    PlanCode = "FINPLAN-2026-STRETCH", PlanName = "FY 2026 Stretch Target Plan",
                    PlanStatus = PlanStatusEnum.Approved, PlanType = PlanTypeEnum.ScenarioPlan,
                    ScenarioType = ScenarioTypeEnum.Stretch, PlanNature = PlanNatureEnum.GrowthFocused,
                    PlanningScopeLevel = PlanningScopeLevelEnum.Company,
                    PlanHorizonMode = PlanHorizonModeEnum.OneYear,
                    FromDate = new DateTime(2026, 4, 1), ToDate = new DateTime(2027, 3, 31),
                    TargetRevenueAmount = 35000000, TargetNetProfitAmount = 8000000,
                    TargetGrowthPercent = 35, TargetHeadcount = 300,
                    VersionNumber = 1, RevisionNumber = 0,
                    StrategicNarrative = "Aggressive growth scenario targeting 35% revenue growth.",
                    ApprovedOn = new DateTime(2026, 3, 22),
                    CreatedAt = new DateTime(2026, 3, 5)
                },
                new() {
                    FinancialPlanId = Guid.Parse("80000000-0000-0000-0000-000000000010"),
                    CompanyId = CompanyId,
                    PlanCode = "FINPLAN-2026-BRC-CLUSTER", PlanName = "Tamil Nadu Branch Cluster Plan",
                    PlanStatus = PlanStatusEnum.Approved, PlanType = PlanTypeEnum.AnnualStrategicPlan,
                    ScenarioType = ScenarioTypeEnum.Base, PlanNature = PlanNatureEnum.ProfitabilityFocused,
                    PlanningScopeLevel = PlanningScopeLevelEnum.BranchCluster,
                    PlanHorizonMode = PlanHorizonModeEnum.OneYear,
                    FromDate = new DateTime(2026, 4, 1), ToDate = new DateTime(2027, 3, 31),
                    BranchGroupCode = "BRC-TN",
                    TargetRevenueAmount = 15000000, TargetExpenseAmount = 10000000,
                    TargetNetProfitAmount = 3000000,
                    VersionNumber = 1, RevisionNumber = 0, LinkedBudgetCount = 4,
                    ApprovedOn = new DateTime(2026, 3, 30),
                    CreatedAt = new DateTime(2026, 3, 15)
                },
                new() {
                    FinancialPlanId = Guid.Parse("80000000-0000-0000-0000-000000000011"),
                    CompanyId = CompanyId,
                    PlanCode = "FINPLAN-2025-SUPERSEDED", PlanName = "FY 2025 Strategic Plan — Superseded",
                    PlanStatus = PlanStatusEnum.Superseded, PlanType = PlanTypeEnum.AnnualStrategicPlan,
                    ScenarioType = ScenarioTypeEnum.OfficialApprovedStrategy,
                    PlanNature = PlanNatureEnum.Mixed, PlanningScopeLevel = PlanningScopeLevelEnum.Company,
                    PlanHorizonMode = PlanHorizonModeEnum.OneYear,
                    FromDate = new DateTime(2025, 4, 1), ToDate = new DateTime(2026, 3, 31),
                    TargetRevenueAmount = 22000000, TargetExpenseAmount = 16500000,
                    VersionNumber = 1, RevisionNumber = 0,
                    IsLocked = true,
                    CreatedAt = new DateTime(2025, 2, 1)
                },
                new() {
                    FinancialPlanId = Guid.Parse("80000000-0000-0000-0000-000000000012"),
                    CompanyId = CompanyId,
                    PlanCode = "FINPLAN-2024-ARCHIVED", PlanName = "FY 2024 Strategic Plan — Archived",
                    PlanStatus = PlanStatusEnum.Archived, PlanType = PlanTypeEnum.AnnualStrategicPlan,
                    ScenarioType = ScenarioTypeEnum.OfficialApprovedStrategy,
                    PlanNature = PlanNatureEnum.Mixed, PlanningScopeLevel = PlanningScopeLevelEnum.Company,
                    PlanHorizonMode = PlanHorizonModeEnum.OneYear,
                    FromDate = new DateTime(2024, 4, 1), ToDate = new DateTime(2025, 3, 31),
                    TargetRevenueAmount = 20000000,
                    VersionNumber = 1, RevisionNumber = 0,
                    IsLocked = true,
                    CreatedAt = new DateTime(2024, 2, 1)
                }
            };
        }
    }
}
