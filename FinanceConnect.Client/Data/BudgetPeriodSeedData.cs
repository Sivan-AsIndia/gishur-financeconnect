using static FinanceConnect.Client.ViewModels.BudgetPeriodViewModel;

namespace FinanceConnect.Client.Data
{
    public static class BudgetPeriodSeedData
    {
        private static readonly Guid CompanyId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        private static readonly Guid TenantId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
        private static readonly Guid FiscalYearId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        private static readonly Guid BudgetId1 = Guid.Parse("73000000-0000-0000-0000-000000000001");
        private static readonly Guid BudgetId2 = Guid.Parse("73000000-0000-0000-0000-000000000002");

        public static List<BudgetPeriod> GetAll()
        {
            return new List<BudgetPeriod>
            {
                new() {
                    BudgetPeriodId = Guid.Parse("74000000-0000-0000-0000-000000000001"),
                    TenantId = TenantId, CompanyId = CompanyId, BudgetId = BudgetId1,
                    PeriodSequenceNo = 1, PeriodCode = "FY26-M01", PeriodName = "April 2026",
                    PeriodType = PeriodTypeEnum.Monthly, FiscalMonthNo = 1, FiscalYearId = FiscalYearId,
                    StartDate = new DateTime(2026, 4, 1), EndDate = new DateTime(2026, 4, 30),
                    PlannedBudgetAmount = 250000, ActualConsumedAmount = 245000,
                    PeriodStatus = PeriodStatusEnum.Closed, IsClosed = true, IsLocked = true,
                    LockedOn = new DateTime(2026, 5, 2), ClosedOn = new DateTime(2026, 5, 5),
                    PeriodNotes = "Q1 opening month — marketing push as planned.",
                    CreatedAt = new DateTime(2026, 3, 15)
                },
                new() {
                    BudgetPeriodId = Guid.Parse("74000000-0000-0000-0000-000000000002"),
                    TenantId = TenantId, CompanyId = CompanyId, BudgetId = BudgetId1,
                    PeriodSequenceNo = 2, PeriodCode = "FY26-M02", PeriodName = "May 2026",
                    PeriodType = PeriodTypeEnum.Monthly, FiscalMonthNo = 2, FiscalYearId = FiscalYearId,
                    StartDate = new DateTime(2026, 5, 1), EndDate = new DateTime(2026, 5, 31),
                    PlannedBudgetAmount = 220000, ActualConsumedAmount = 230000,
                    PeriodStatus = PeriodStatusEnum.Closed, IsClosed = true, IsLocked = true,
                    LockedOn = new DateTime(2026, 6, 2), ClosedOn = new DateTime(2026, 6, 5),
                    PeriodNotes = "Slight overspend due to unplanned IT procurement.",
                    CreatedAt = new DateTime(2026, 3, 15)
                },
                new() {
                    BudgetPeriodId = Guid.Parse("74000000-0000-0000-0000-000000000003"),
                    TenantId = TenantId, CompanyId = CompanyId, BudgetId = BudgetId1,
                    PeriodSequenceNo = 3, PeriodCode = "FY26-M03", PeriodName = "June 2026",
                    PeriodType = PeriodTypeEnum.Monthly, FiscalMonthNo = 3, FiscalYearId = FiscalYearId,
                    StartDate = new DateTime(2026, 6, 1), EndDate = new DateTime(2026, 6, 30),
                    PlannedBudgetAmount = 230000, ActualConsumedAmount = 180000,
                    PeriodStatus = PeriodStatusEnum.Locked, IsLocked = true,
                    LockedOn = new DateTime(2026, 7, 1),
                    PeriodNotes = "Under-utilization due to delayed project start.",
                    CreatedAt = new DateTime(2026, 3, 15)
                },
                new() {
                    BudgetPeriodId = Guid.Parse("74000000-0000-0000-0000-000000000004"),
                    TenantId = TenantId, CompanyId = CompanyId, BudgetId = BudgetId1,
                    PeriodSequenceNo = 4, PeriodCode = "FY26-M04", PeriodName = "July 2026",
                    PeriodType = PeriodTypeEnum.Monthly, FiscalMonthNo = 4, FiscalYearId = FiscalYearId,
                    StartDate = new DateTime(2026, 7, 1), EndDate = new DateTime(2026, 7, 31),
                    PlannedBudgetAmount = 260000, ActualConsumedAmount = 150000,
                    PeriodStatus = PeriodStatusEnum.Open, OpenForConsumptionFlag = true,
                    PeriodNotes = "Mid-year period — actuals still accumulating.",
                    CreatedAt = new DateTime(2026, 3, 15)
                },
                new() {
                    BudgetPeriodId = Guid.Parse("74000000-0000-0000-0000-000000000005"),
                    TenantId = TenantId, CompanyId = CompanyId, BudgetId = BudgetId1,
                    PeriodSequenceNo = 5, PeriodCode = "FY26-M05", PeriodName = "August 2026",
                    PeriodType = PeriodTypeEnum.Monthly, FiscalMonthNo = 5, FiscalYearId = FiscalYearId,
                    StartDate = new DateTime(2026, 8, 1), EndDate = new DateTime(2026, 8, 31),
                    PlannedBudgetAmount = 240000, RevisedBudgetAmount = 270000,
                    ActualConsumedAmount = 50000,
                    PeriodStatus = PeriodStatusEnum.Revised,
                    RevisionReason = "Additional headcount approved for sales expansion.",
                    CreatedAt = new DateTime(2026, 3, 15)
                },
                new() {
                    BudgetPeriodId = Guid.Parse("74000000-0000-0000-0000-000000000006"),
                    TenantId = TenantId, CompanyId = CompanyId, BudgetId = BudgetId1,
                    PeriodSequenceNo = 6, PeriodCode = "FY26-M06", PeriodName = "September 2026",
                    PeriodType = PeriodTypeEnum.Monthly, FiscalMonthNo = 6, FiscalYearId = FiscalYearId,
                    StartDate = new DateTime(2026, 9, 1), EndDate = new DateTime(2026, 9, 30),
                    PlannedBudgetAmount = 250000,
                    PeriodStatus = PeriodStatusEnum.Released, OpenForConsumptionFlag = true,
                    ReleasedBudgetAmount = 250000,
                    CreatedAt = new DateTime(2026, 3, 15)
                },
                new() {
                    BudgetPeriodId = Guid.Parse("74000000-0000-0000-0000-000000000007"),
                    TenantId = TenantId, CompanyId = CompanyId, BudgetId = BudgetId1,
                    PeriodSequenceNo = 7, PeriodCode = "FY26-M07", PeriodName = "October 2026",
                    PeriodType = PeriodTypeEnum.Monthly, FiscalMonthNo = 7, FiscalYearId = FiscalYearId,
                    StartDate = new DateTime(2026, 10, 1), EndDate = new DateTime(2026, 10, 31),
                    PlannedBudgetAmount = 280000,
                    PeriodStatus = PeriodStatusEnum.Draft,
                    PlanningAssumptionSummary = "Festival season — higher marketing spend expected.",
                    CreatedAt = new DateTime(2026, 3, 15)
                },
                new() {
                    BudgetPeriodId = Guid.Parse("74000000-0000-0000-0000-000000000008"),
                    TenantId = TenantId, CompanyId = CompanyId, BudgetId = BudgetId1,
                    PeriodSequenceNo = 8, PeriodCode = "FY26-M08", PeriodName = "November 2026",
                    PeriodType = PeriodTypeEnum.Monthly, FiscalMonthNo = 8, FiscalYearId = FiscalYearId,
                    StartDate = new DateTime(2026, 11, 1), EndDate = new DateTime(2026, 11, 30),
                    PlannedBudgetAmount = 240000,
                    PeriodStatus = PeriodStatusEnum.Draft,
                    CreatedAt = new DateTime(2026, 3, 15)
                },
                new() {
                    BudgetPeriodId = Guid.Parse("74000000-0000-0000-0000-000000000009"),
                    TenantId = TenantId, CompanyId = CompanyId, BudgetId = BudgetId2,
                    PeriodSequenceNo = 1, PeriodCode = "FY26-Q1", PeriodName = "Q1 FY 2026-27",
                    PeriodType = PeriodTypeEnum.Quarterly, FiscalQuarterNo = 1, FiscalYearId = FiscalYearId,
                    StartDate = new DateTime(2026, 4, 1), EndDate = new DateTime(2026, 6, 30),
                    PlannedBudgetAmount = 1500000, ActualConsumedAmount = 1450000,
                    PeriodStatus = PeriodStatusEnum.Closed, IsClosed = true, IsLocked = true,
                    LockedOn = new DateTime(2026, 7, 5), ClosedOn = new DateTime(2026, 7, 10),
                    PeriodNotes = "Q1 completed within tolerance.",
                    CreatedAt = new DateTime(2026, 3, 1)
                },
                new() {
                    BudgetPeriodId = Guid.Parse("74000000-0000-0000-0000-00000000000A"),
                    TenantId = TenantId, CompanyId = CompanyId, BudgetId = BudgetId2,
                    PeriodSequenceNo = 2, PeriodCode = "FY26-Q2", PeriodName = "Q2 FY 2026-27",
                    PeriodType = PeriodTypeEnum.Quarterly, FiscalQuarterNo = 2, FiscalYearId = FiscalYearId,
                    StartDate = new DateTime(2026, 7, 1), EndDate = new DateTime(2026, 9, 30),
                    PlannedBudgetAmount = 1600000, ActualConsumedAmount = 800000,
                    PeriodStatus = PeriodStatusEnum.Open, OpenForConsumptionFlag = true,
                    PeriodNotes = "Mid-quarter — actuals tracking on budget.",
                    CreatedAt = new DateTime(2026, 3, 1)
                },
                new() {
                    BudgetPeriodId = Guid.Parse("74000000-0000-0000-0000-00000000000B"),
                    TenantId = TenantId, CompanyId = CompanyId, BudgetId = BudgetId2,
                    PeriodSequenceNo = 3, PeriodCode = "FY26-Q3", PeriodName = "Q3 FY 2026-27",
                    PeriodType = PeriodTypeEnum.Quarterly, FiscalQuarterNo = 3, FiscalYearId = FiscalYearId,
                    StartDate = new DateTime(2026, 10, 1), EndDate = new DateTime(2026, 12, 31),
                    PlannedBudgetAmount = 1700000,
                    PeriodStatus = PeriodStatusEnum.Draft,
                    PlanningAssumptionSummary = "Q3 expected higher due to Diwali and year-end push.",
                    CreatedAt = new DateTime(2026, 3, 1)
                },
                new() {
                    BudgetPeriodId = Guid.Parse("74000000-0000-0000-0000-00000000000C"),
                    TenantId = TenantId, CompanyId = CompanyId, BudgetId = BudgetId2,
                    PeriodSequenceNo = 4, PeriodCode = "FY26-Q4", PeriodName = "Q4 FY 2026-27",
                    PeriodType = PeriodTypeEnum.Quarterly, FiscalQuarterNo = 4, FiscalYearId = FiscalYearId,
                    StartDate = new DateTime(2027, 1, 1), EndDate = new DateTime(2027, 3, 31),
                    PlannedBudgetAmount = 1200000,
                    PeriodStatus = PeriodStatusEnum.Draft,
                    PlanningAssumptionSummary = "Q4 conservative — capex deferred to next FY.",
                    CreatedAt = new DateTime(2026, 3, 1)
                }
            };
        }
    }
}
