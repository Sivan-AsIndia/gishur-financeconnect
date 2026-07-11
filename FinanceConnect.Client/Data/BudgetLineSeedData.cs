using static FinanceConnect.Client.ViewModels.BudgetLineViewModel;

namespace FinanceConnect.Client.Data
{
    public static class BudgetLineSeedData
    {
        private static readonly Guid CompanyId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        private static readonly Guid TenantId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
        private static readonly Guid BudgetId1 = Guid.Parse("73000000-0000-0000-0000-000000000001");
        private static readonly Guid BudgetId2 = Guid.Parse("73000000-0000-0000-0000-000000000002");
        private static readonly Guid CostCenter1 = Guid.Parse("76000000-0000-0000-0000-000000000001");
        private static readonly Guid CostCenter2 = Guid.Parse("76000000-0000-0000-0000-000000000002");

        public static List<BudgetLine> GetAll()
        {
            return new List<BudgetLine>
            {
                new() {
                    BudgetLineId = Guid.Parse("75000000-0000-0000-0000-000000000001"),
                    TenantId = TenantId, CompanyId = CompanyId, BudgetId = BudgetId1,
                    LineNumber = 10, LineCode = "BL-SAL-001", LineName = "Monthly Salary Budget",
                    Description = "All-inclusive salary cost for permanent employees.",
                    LineType = LineTypeEnum.Expense, BudgetCategoryCode = "Salary",
                    CostCenterId = CostCenter1, CostCenterName = "Head Office",
                    OriginalPlannedAmount = 1200000, ActualConsumedAmount = 600000,
                    DistributionMode = DistributionModeEnum.EvenSpread, HasPeriodDistribution = true,
                    ActualMatchMode = ActualMatchModeEnum.ByGLAndCostCenter,
                    ActualSourceScope = ActualSourceScopeEnum.AllPostedActuals,
                    LineStatus = LineStatusEnum.Active, IncludeAllocatedActuals = true,
                    PlanningAssumptionText = "Headcount stable at 45 for FY.",
                    CreatedAt = new DateTime(2026, 3, 15)
                },
                new() {
                    BudgetLineId = Guid.Parse("75000000-0000-0000-0000-000000000002"),
                    TenantId = TenantId, CompanyId = CompanyId, BudgetId = BudgetId1,
                    LineNumber = 20, LineCode = "BL-TRV-001", LineName = "Sales Travel Expense",
                    Description = "Domestic and international travel for the sales team.",
                    LineType = LineTypeEnum.Expense, BudgetCategoryCode = "Travel",
                    CostCenterId = CostCenter2, CostCenterName = "Sales",
                    OriginalPlannedAmount = 300000, ActualConsumedAmount = 335000,
                    DistributionMode = DistributionModeEnum.Manual, HasPeriodDistribution = true,
                    ActualMatchMode = ActualMatchModeEnum.ByGLAndCostCenter,
                    ActualSourceScope = ActualSourceScopeEnum.APBills,
                    LineStatus = LineStatusEnum.Active,
                    PlanningAssumptionText = "Travel cost expected to increase due to regional expansion.",
                    Notes = "Overspend flagged — under review.",
                    CreatedAt = new DateTime(2026, 3, 15)
                },
                new() {
                    BudgetLineId = Guid.Parse("75000000-0000-0000-0000-000000000003"),
                    TenantId = TenantId, CompanyId = CompanyId, BudgetId = BudgetId1,
                    LineNumber = 30, LineCode = "BL-RNT-001", LineName = "Office Rent",
                    LineType = LineTypeEnum.Expense, BudgetCategoryCode = "Rent",
                    CostCenterId = CostCenter1, CostCenterName = "Head Office",
                    OriginalPlannedAmount = 480000, ActualConsumedAmount = 240000,
                    DistributionMode = DistributionModeEnum.EvenSpread, HasPeriodDistribution = true,
                    ActualMatchMode = ActualMatchModeEnum.ByGLAccountOnly,
                    ActualSourceScope = ActualSourceScopeEnum.APBills,
                    LineStatus = LineStatusEnum.Locked, IsLocked = true,
                    LockedOn = new DateTime(2026, 4, 10),
                    CreatedAt = new DateTime(2026, 3, 15)
                },
                new() {
                    BudgetLineId = Guid.Parse("75000000-0000-0000-0000-000000000004"),
                    TenantId = TenantId, CompanyId = CompanyId, BudgetId = BudgetId1,
                    LineNumber = 40, LineCode = "BL-UTL-001", LineName = "Utilities (Power & Water)",
                    LineType = LineTypeEnum.Expense, BudgetCategoryCode = "Utilities",
                    CostCenterId = CostCenter1, CostCenterName = "Head Office",
                    OriginalPlannedAmount = 120000, ActualConsumedAmount = 55000,
                    DistributionMode = DistributionModeEnum.SeasonalTemplate,
                    DistributionTemplateCode = "SEASONAL_SUMMER_HIGH", HasPeriodDistribution = true,
                    ActualMatchMode = ActualMatchModeEnum.ByGLAccountOnly,
                    ActualSourceScope = ActualSourceScopeEnum.APBills,
                    LineStatus = LineStatusEnum.Active,
                    CreatedAt = new DateTime(2026, 3, 15)
                },
                new() {
                    BudgetLineId = Guid.Parse("75000000-0000-0000-0000-000000000005"),
                    TenantId = TenantId, CompanyId = CompanyId, BudgetId = BudgetId1,
                    LineNumber = 50, LineCode = "BL-SW-001", LineName = "Software Subscription",
                    Description = "Annual software licenses — ERP, CRM, Office 365.",
                    LineType = LineTypeEnum.Expense, BudgetCategoryCode = "Software",
                    CostCenterId = CostCenter1, CostCenterName = "Head Office",
                    OriginalPlannedAmount = 360000, RevisedAmount = 400000,
                    ActualConsumedAmount = 200000,
                    DistributionMode = DistributionModeEnum.Manual, HasPeriodDistribution = true,
                    ActualMatchMode = ActualMatchModeEnum.ByGLAndCostCenter,
                    ActualSourceScope = ActualSourceScopeEnum.APBills,
                    LineStatus = LineStatusEnum.Revised,
                    RevisionReason = "Added new CRM license mid-year.",
                    PlanningAssumptionText = "Software budget annual contract renewal in Q2.",
                    CreatedAt = new DateTime(2026, 3, 15)
                },
                new() {
                    BudgetLineId = Guid.Parse("75000000-0000-0000-0000-000000000006"),
                    TenantId = TenantId, CompanyId = CompanyId, BudgetId = BudgetId1,
                    LineNumber = 60, LineCode = "BL-MKT-001", LineName = "Marketing Digital Ads",
                    LineType = LineTypeEnum.Expense, BudgetCategoryCode = "Marketing",
                    CostCenterId = CostCenter2, CostCenterName = "Sales",
                    OriginalPlannedAmount = 500000, ActualConsumedAmount = 280000,
                    DistributionMode = DistributionModeEnum.WeightedSpread, HasPeriodDistribution = true,
                    ActualMatchMode = ActualMatchModeEnum.ByGLAndCostCenter,
                    ActualSourceScope = ActualSourceScopeEnum.AllPostedActuals,
                    LineStatus = LineStatusEnum.Active,
                    PlanningAssumptionText = "Heavy Q3 push for festival season campaigns.",
                    CreatedAt = new DateTime(2026, 3, 15)
                },
                new() {
                    BudgetLineId = Guid.Parse("75000000-0000-0000-0000-000000000007"),
                    TenantId = TenantId, CompanyId = CompanyId, BudgetId = BudgetId1,
                    LineNumber = 70, LineCode = "BL-CAP-001", LineName = "Office Furniture Capex",
                    Description = "Capital expenditure for new office furniture and ergonomic chairs.",
                    LineType = LineTypeEnum.Capex, BudgetCategoryCode = "Furniture", IsCapexFlag = true,
                    CostCenterId = CostCenter1, CostCenterName = "Head Office",
                    OriginalPlannedAmount = 200000, ActualConsumedAmount = 0,
                    DistributionMode = DistributionModeEnum.Manual,
                    ActualMatchMode = ActualMatchModeEnum.ByGLAccountOnly,
                    ActualSourceScope = ActualSourceScopeEnum.APBills,
                    LineStatus = LineStatusEnum.Draft,
                    PlanningAssumptionText = "Procurement planned for Q3.",
                    CreatedAt = new DateTime(2026, 3, 15)
                },
                new() {
                    BudgetLineId = Guid.Parse("75000000-0000-0000-0000-000000000008"),
                    TenantId = TenantId, CompanyId = CompanyId, BudgetId = BudgetId2,
                    LineNumber = 10, LineCode = "BL-REV-001", LineName = "Regional Product Sales",
                    Description = "Revenue from Chennai branch product sales.",
                    LineType = LineTypeEnum.Revenue, BudgetCategoryCode = "RevenueSales",
                    BranchName = "Chennai",
                    OriginalPlannedAmount = 5000000, ActualConsumedAmount = 2800000,
                    DistributionMode = DistributionModeEnum.ActualTrendBased, HasPeriodDistribution = true,
                    ActualMatchMode = ActualMatchModeEnum.ByGLAndBranch,
                    ActualSourceScope = ActualSourceScopeEnum.ARInvoices,
                    LineStatus = LineStatusEnum.Active,
                    PlanningAssumptionText = "Revenue growth at 12% YoY based on market trends.",
                    CreatedAt = new DateTime(2026, 3, 1)
                },
                new() {
                    BudgetLineId = Guid.Parse("75000000-0000-0000-0000-000000000009"),
                    TenantId = TenantId, CompanyId = CompanyId, BudgetId = BudgetId2,
                    LineNumber = 20, LineCode = "BL-REV-002", LineName = "Service Revenue — Consulting",
                    LineType = LineTypeEnum.Revenue, BudgetCategoryCode = "RevenueService",
                    OriginalPlannedAmount = 1500000, ActualConsumedAmount = 700000,
                    DistributionMode = DistributionModeEnum.EvenSpread, HasPeriodDistribution = true,
                    ActualMatchMode = ActualMatchModeEnum.ByGLAccountOnly,
                    ActualSourceScope = ActualSourceScopeEnum.ARInvoices,
                    LineStatus = LineStatusEnum.Active,
                    CreatedAt = new DateTime(2026, 3, 1)
                },
                new() {
                    BudgetLineId = Guid.Parse("75000000-0000-0000-0000-00000000000A"),
                    TenantId = TenantId, CompanyId = CompanyId, BudgetId = BudgetId2,
                    LineNumber = 30, LineCode = "BL-HRX-001", LineName = "HR Shared Services Allocation",
                    Description = "Head Office HR cost allocated to revenue budget as shared service.",
                    LineType = LineTypeEnum.Transfer, BudgetCategoryCode = "SharedService",
                    CostCenterId = CostCenter1, CostCenterName = "Head Office",
                    ResponsibilityType = ResponsibilityTypeEnum.AllocatedOwner,
                    OriginalPlannedAmount = 180000, ActualConsumedAmount = 90000,
                    DistributionMode = DistributionModeEnum.EvenSpread, HasPeriodDistribution = true,
                    ActualMatchMode = ActualMatchModeEnum.ByDimensionSet,
                    IncludeAllocatedActuals = true,
                    ActualSourceScope = ActualSourceScopeEnum.AllPostedActuals,
                    LineStatus = LineStatusEnum.Active,
                    CreatedAt = new DateTime(2026, 3, 1)
                },
                new() {
                    BudgetLineId = Guid.Parse("75000000-0000-0000-0000-00000000000B"),
                    TenantId = TenantId, CompanyId = CompanyId, BudgetId = BudgetId2,
                    LineNumber = 40, LineCode = "BL-STAT-001", LineName = "Headcount Statistical Line",
                    Description = "Statistical tracking line for employee headcount planning.",
                    LineType = LineTypeEnum.Statistical, BudgetCategoryCode = "Headcount",
                    OriginalPlannedAmount = 50,
                    DistributionMode = DistributionModeEnum.EvenSpread,
                    ActualMatchMode = ActualMatchModeEnum.ByDimensionSet,
                    ActualSourceScope = ActualSourceScopeEnum.AllPostedActuals,
                    LineStatus = LineStatusEnum.Active,
                    Notes = "Statistical line — not financial. Tracks FTE count.",
                    CreatedAt = new DateTime(2026, 3, 1)
                },
                new() {
                    BudgetLineId = Guid.Parse("75000000-0000-0000-0000-00000000000C"),
                    TenantId = TenantId, CompanyId = CompanyId, BudgetId = BudgetId1,
                    LineNumber = 80, LineCode = "BL-INS-001", LineName = "Insurance Premium",
                    Description = "Annual insurance policy premium — health and property.",
                    LineType = LineTypeEnum.Expense, BudgetCategoryCode = "Insurance",
                    CostCenterId = CostCenter1, CostCenterName = "Head Office",
                    OriginalPlannedAmount = 96000, ActualConsumedAmount = 96000,
                    DistributionMode = DistributionModeEnum.Manual,
                    ActualMatchMode = ActualMatchModeEnum.ByGLAccountOnly,
                    ActualSourceScope = ActualSourceScopeEnum.APBills,
                    LineStatus = LineStatusEnum.Closed, ClosedOn = new DateTime(2026, 5, 1),
                    PlanningAssumptionText = "Lump sum annual premium — paid in April.",
                    CreatedAt = new DateTime(2026, 3, 15)
                }
            };
        }
    }
}
