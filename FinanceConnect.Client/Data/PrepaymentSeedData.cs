using static FinanceConnect.Client.ViewModels.PrepaymentViewModel;

namespace FinanceConnect.Client.Data
{
    public static class PrepaymentSeedData
    {
        public static List<Prepayment> GetAll()
        {
            return new List<Prepayment>
            {
                new Prepayment
                {
                    PrepaymentId = Guid.Parse("ab870001-0001-0001-0001-000000000001"),
                    TenantId = MasterDataIds.Tenants.Default, CompanyId = MasterDataIds.Companies.SofaCraft,
                    PrepaymentCode = "PREP-2026-0001", PrepaymentTitle = "Annual Insurance Premium – Factory & Warehouse",
                    Description = "Fire and machinery insurance premium paid upfront for 12 months.",
                    PrepaymentStatus = PrepaymentStatusEnum.InProgress, SourceType = SourceTypeEnum.VendorBill,
                    SourceDocumentNumber = "INS-VB-2026-001",
                    BasisReferenceText = "Annual insurance premium paid upfront – coverage Apr 2026 to Mar 2027",
                    PrepaymentStartDate = new DateTime(2026, 4, 1), PrepaymentEndDate = new DateTime(2027, 3, 31),
                    ReleaseMethod = ReleaseMethodEnum.MonthlyEqual, ReleaseFrequency = ReleaseFrequencyEnum.Monthly,
                    ReleaseStartDate = new DateTime(2026, 4, 1), ReleaseEndDate = new DateTime(2027, 3, 31),
                    IsScheduleGenerated = true,
                    CurrencyId = MasterDataIds.Currencies.INR, OriginalPrepaidAmount = 360000m,
                    ReleasedAmountToDate = 90000m, CurrentPeriodReleaseAmount = 30000m,
                    FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                    ExpenseGLAccountId = MasterDataIds.Accounts.RentExpense, ExpenseGLAccountName = "6002 – Rent Expense",
                    PrepaymentAssetGLId = MasterDataIds.Accounts.FurnitureFixtures, PrepaymentAssetGLName = "1100 – Furniture & Fixtures",
                    PostingStatus = PostingStatusEnum.PartiallyReleased, ReleaseStatus = ReleaseStatusEnum.InProgress,
                    NextReleaseDueDate = new DateTime(2026, 7, 1), LastReleaseDate = new DateTime(2026, 6, 1),
                    BranchId = MasterDataIds.Branches.SofaCraftHQ, BranchName = "SofaCraft Head Office & Factory - Chennai",
                    PreparedByUserId = "finance.admin", MaterialityLevel = MaterialityLevelEnum.High,
                    CreatedAt = new DateTime(2026, 3, 28),
                    ReleaseLines = GenerateMonthlyLines(Guid.Parse("ab870001-0001-0001-0001-000000000001"), 360000m, new DateTime(2026, 4, 1), 12, 3)
                },
                new Prepayment
                {
                    PrepaymentId = Guid.Parse("ab870002-0002-0002-0002-000000000002"),
                    TenantId = MasterDataIds.Tenants.Default, CompanyId = MasterDataIds.Companies.SofaCraft,
                    PrepaymentCode = "PREP-2026-0002", PrepaymentTitle = "SaaS Subscription – ERP License FY26",
                    Description = "Annual ERP software subscription license paid upfront.",
                    PrepaymentStatus = PrepaymentStatusEnum.Posted, SourceType = SourceTypeEnum.Payment,
                    BasisReferenceText = "One-year ERP subscription contract paid in advance – Apr 2026 to Mar 2027",
                    PrepaymentStartDate = new DateTime(2026, 4, 1), PrepaymentEndDate = new DateTime(2027, 3, 31),
                    ReleaseMethod = ReleaseMethodEnum.StraightLine, ReleaseFrequency = ReleaseFrequencyEnum.Monthly,
                    ReleaseStartDate = new DateTime(2026, 4, 1), ReleaseEndDate = new DateTime(2027, 3, 31),
                    IsScheduleGenerated = true,
                    CurrencyId = MasterDataIds.Currencies.INR, OriginalPrepaidAmount = 480000m,
                    ReleasedAmountToDate = 0m,
                    FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                    ExpenseGLAccountId = MasterDataIds.Accounts.UtilitiesExpense, ExpenseGLAccountName = "6003 – Utilities Expense",
                    PrepaymentAssetGLId = MasterDataIds.Accounts.FurnitureFixtures, PrepaymentAssetGLName = "1100 – Furniture & Fixtures",
                    PostingStatus = PostingStatusEnum.Posted, ReleaseStatus = ReleaseStatusEnum.NotStarted,
                    NextReleaseDueDate = new DateTime(2026, 4, 1),
                    PreparedByUserId = "finance.admin", MaterialityLevel = MaterialityLevelEnum.High,
                    CreatedAt = new DateTime(2026, 3, 25)
                },
                new Prepayment
                {
                    PrepaymentId = Guid.Parse("ab870003-0003-0003-0003-000000000003"),
                    TenantId = MasterDataIds.Tenants.Default, CompanyId = MasterDataIds.Companies.SofaCraft,
                    PrepaymentCode = "PREP-2026-0003", PrepaymentTitle = "Office Rent Advance – Apr to Jun 2026",
                    PrepaymentStatus = PrepaymentStatusEnum.FullyReleased, SourceType = SourceTypeEnum.Expense,
                    BasisReferenceText = "3 months rent paid in advance for Bengaluru Experience Store",
                    PrepaymentStartDate = new DateTime(2026, 4, 1), PrepaymentEndDate = new DateTime(2026, 6, 30),
                    ReleaseMethod = ReleaseMethodEnum.MonthlyEqual, ReleaseFrequency = ReleaseFrequencyEnum.Monthly,
                    ReleaseStartDate = new DateTime(2026, 4, 1), ReleaseEndDate = new DateTime(2026, 6, 30),
                    IsScheduleGenerated = true,
                    CurrencyId = MasterDataIds.Currencies.INR, OriginalPrepaidAmount = 360000m,
                    ReleasedAmountToDate = 360000m,
                    FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                    ExpenseGLAccountId = MasterDataIds.Accounts.RentExpense, ExpenseGLAccountName = "6002 – Rent Expense",
                    PrepaymentAssetGLId = MasterDataIds.Accounts.FurnitureFixtures, PrepaymentAssetGLName = "1100 – Furniture & Fixtures",
                    PostingStatus = PostingStatusEnum.FullyReleased, ReleaseStatus = ReleaseStatusEnum.FullyReleased,
                    LastReleaseDate = new DateTime(2026, 6, 1),
                    BranchId = MasterDataIds.Branches.SofaCraftBengaluru, BranchName = "SofaCraft Experience Store - Bengaluru",
                    PreparedByUserId = "finance.admin", MaterialityLevel = MaterialityLevelEnum.Medium,
                    CreatedAt = new DateTime(2026, 3, 20)
                },
                new Prepayment
                {
                    PrepaymentId = Guid.Parse("ab870004-0004-0004-0004-000000000004"),
                    TenantId = MasterDataIds.Tenants.Default, CompanyId = MasterDataIds.Companies.SofaCraft,
                    PrepaymentCode = "PREP-2026-0004", PrepaymentTitle = "AMC – AC Maintenance Contract",
                    PrepaymentStatus = PrepaymentStatusEnum.Approved, SourceType = SourceTypeEnum.Contract,
                    BasisReferenceText = "Annual AMC for air conditioning maintenance – factory floor",
                    PrepaymentStartDate = new DateTime(2026, 4, 1), PrepaymentEndDate = new DateTime(2027, 3, 31),
                    ReleaseMethod = ReleaseMethodEnum.QuarterlyEqual, ReleaseFrequency = ReleaseFrequencyEnum.Quarterly,
                    ReleaseStartDate = new DateTime(2026, 4, 1), ReleaseEndDate = new DateTime(2027, 3, 31),
                    CurrencyId = MasterDataIds.Currencies.INR, OriginalPrepaidAmount = 200000m,
                    FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                    ExpenseGLAccountId = MasterDataIds.Accounts.UtilitiesExpense, ExpenseGLAccountName = "6003 – Utilities Expense",
                    PrepaymentAssetGLId = MasterDataIds.Accounts.FurnitureFixtures, PrepaymentAssetGLName = "1100 – Furniture & Fixtures",
                    PostingStatus = PostingStatusEnum.NotPosted, ReleaseStatus = ReleaseStatusEnum.NotStarted,
                    PreparedByUserId = "finance.admin", MaterialityLevel = MaterialityLevelEnum.Medium,
                    CreatedAt = new DateTime(2026, 3, 22)
                },
                new Prepayment
                {
                    PrepaymentId = Guid.Parse("ab870005-0005-0005-0005-000000000005"),
                    TenantId = MasterDataIds.Tenants.Default, CompanyId = MasterDataIds.Companies.SofaCraft,
                    PrepaymentCode = "PREP-2026-0005", PrepaymentTitle = "Software Maintenance – CAD/CAM Tools",
                    PrepaymentStatus = PrepaymentStatusEnum.Draft, SourceType = SourceTypeEnum.VendorBill,
                    BasisReferenceText = "Annual software maintenance for furniture design CAD/CAM tools",
                    PrepaymentStartDate = new DateTime(2026, 5, 1), PrepaymentEndDate = new DateTime(2027, 4, 30),
                    ReleaseMethod = ReleaseMethodEnum.StraightLine, ReleaseFrequency = ReleaseFrequencyEnum.Monthly,
                    ReleaseStartDate = new DateTime(2026, 5, 1),
                    CurrencyId = MasterDataIds.Currencies.INR, OriginalPrepaidAmount = 180000m,
                    FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                    ExpenseGLAccountId = MasterDataIds.Accounts.UtilitiesExpense, ExpenseGLAccountName = "6003 – Utilities Expense",
                    PrepaymentAssetGLId = MasterDataIds.Accounts.FurnitureFixtures, PrepaymentAssetGLName = "1100 – Furniture & Fixtures",
                    PostingStatus = PostingStatusEnum.NotPosted, ReleaseStatus = ReleaseStatusEnum.NotStarted,
                    PreparedByUserId = "finance.admin", MaterialityLevel = MaterialityLevelEnum.Low,
                    CreatedAt = new DateTime(2026, 4, 5)
                },
                new Prepayment
                {
                    PrepaymentId = Guid.Parse("ab870006-0006-0006-0006-000000000006"),
                    TenantId = MasterDataIds.Tenants.Default, CompanyId = MasterDataIds.Companies.SofaCraft,
                    PrepaymentCode = "PREP-2026-0006", PrepaymentTitle = "Advertising Slot – Magazine H2 FY26",
                    PrepaymentStatus = PrepaymentStatusEnum.InProgress, SourceType = SourceTypeEnum.Payment,
                    BasisReferenceText = "Prepaid advertising slot in lifestyle magazine – Oct 2025 to Mar 2026",
                    PrepaymentStartDate = new DateTime(2025, 10, 1), PrepaymentEndDate = new DateTime(2026, 3, 31),
                    ReleaseMethod = ReleaseMethodEnum.MonthlyEqual, ReleaseFrequency = ReleaseFrequencyEnum.Monthly,
                    ReleaseStartDate = new DateTime(2025, 10, 1), ReleaseEndDate = new DateTime(2026, 3, 31),
                    IsScheduleGenerated = true,
                    CurrencyId = MasterDataIds.Currencies.INR, OriginalPrepaidAmount = 300000m,
                    ReleasedAmountToDate = 250000m,
                    FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                    ExpenseGLAccountId = MasterDataIds.Accounts.RentExpense, ExpenseGLAccountName = "6002 – Rent Expense",
                    PrepaymentAssetGLId = MasterDataIds.Accounts.FurnitureFixtures, PrepaymentAssetGLName = "1100 – Furniture & Fixtures",
                    PostingStatus = PostingStatusEnum.PartiallyReleased, ReleaseStatus = ReleaseStatusEnum.PartiallyReleased,
                    NextReleaseDueDate = new DateTime(2026, 3, 1), LastReleaseDate = new DateTime(2026, 2, 1),
                    PreparedByUserId = "finance.admin", MaterialityLevel = MaterialityLevelEnum.Medium,
                    CreatedAt = new DateTime(2025, 9, 28)
                },
                new Prepayment
                {
                    PrepaymentId = Guid.Parse("ab870007-0007-0007-0007-000000000007"),
                    TenantId = MasterDataIds.Tenants.Default, CompanyId = MasterDataIds.Companies.SofaCraft,
                    PrepaymentCode = "PREP-2026-0007", PrepaymentTitle = "Domain & Hosting – Web Platform FY26",
                    PrepaymentStatus = PrepaymentStatusEnum.Submitted, SourceType = SourceTypeEnum.Payment,
                    BasisReferenceText = "Annual domain registration and cloud hosting fees",
                    PrepaymentStartDate = new DateTime(2026, 4, 1), PrepaymentEndDate = new DateTime(2027, 3, 31),
                    ReleaseMethod = ReleaseMethodEnum.MonthlyEqual, ReleaseFrequency = ReleaseFrequencyEnum.Monthly,
                    ReleaseStartDate = new DateTime(2026, 4, 1),
                    CurrencyId = MasterDataIds.Currencies.INR, OriginalPrepaidAmount = 72000m,
                    FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                    ExpenseGLAccountId = MasterDataIds.Accounts.UtilitiesExpense, ExpenseGLAccountName = "6003 – Utilities Expense",
                    PrepaymentAssetGLId = MasterDataIds.Accounts.FurnitureFixtures, PrepaymentAssetGLName = "1100 – Furniture & Fixtures",
                    PostingStatus = PostingStatusEnum.NotPosted, ReleaseStatus = ReleaseStatusEnum.NotStarted,
                    PreparedByUserId = "finance.admin", MaterialityLevel = MaterialityLevelEnum.Low,
                    CreatedAt = new DateTime(2026, 3, 30)
                },
                new Prepayment
                {
                    PrepaymentId = Guid.Parse("ab870008-0008-0008-0008-000000000008"),
                    TenantId = MasterDataIds.Tenants.Default, CompanyId = MasterDataIds.Companies.SofaCraft,
                    PrepaymentCode = "PREP-2026-0008", PrepaymentTitle = "Security Services – Annual Guard Contract",
                    PrepaymentStatus = PrepaymentStatusEnum.Closed, SourceType = SourceTypeEnum.Contract,
                    BasisReferenceText = "Annual security guard contract – factory premises",
                    PrepaymentStartDate = new DateTime(2025, 4, 1), PrepaymentEndDate = new DateTime(2026, 3, 31),
                    ReleaseMethod = ReleaseMethodEnum.MonthlyEqual, ReleaseFrequency = ReleaseFrequencyEnum.Monthly,
                    ReleaseStartDate = new DateTime(2025, 4, 1), ReleaseEndDate = new DateTime(2026, 3, 31),
                    IsScheduleGenerated = true,
                    CurrencyId = MasterDataIds.Currencies.INR, OriginalPrepaidAmount = 600000m,
                    ReleasedAmountToDate = 600000m,
                    FiscalYearId = MasterDataIds.FiscalYears.FY2024_25,
                    ExpenseGLAccountId = MasterDataIds.Accounts.SalariesWages, ExpenseGLAccountName = "6001 – Salaries & Wages",
                    PrepaymentAssetGLId = MasterDataIds.Accounts.FurnitureFixtures, PrepaymentAssetGLName = "1100 – Furniture & Fixtures",
                    PostingStatus = PostingStatusEnum.FullyReleased, ReleaseStatus = ReleaseStatusEnum.FullyReleased,
                    LastReleaseDate = new DateTime(2026, 3, 1), IsLocked = true, LockedOn = new DateTime(2026, 4, 2),
                    PreparedByUserId = "finance.admin", MaterialityLevel = MaterialityLevelEnum.High,
                    CreatedAt = new DateTime(2025, 3, 28)
                },
                new Prepayment
                {
                    PrepaymentId = Guid.Parse("ab870009-0009-0009-0009-000000000009"),
                    TenantId = MasterDataIds.Tenants.Default, CompanyId = MasterDataIds.Companies.SofaCraft,
                    PrepaymentCode = "PREP-2026-0009", PrepaymentTitle = "Trade Show Booth – Q3 Exhibition",
                    PrepaymentStatus = PrepaymentStatusEnum.Draft, SourceType = SourceTypeEnum.Payment,
                    BasisReferenceText = "Advance payment for trade show booth reservation Oct-Dec 2026",
                    PrepaymentStartDate = new DateTime(2026, 10, 1), PrepaymentEndDate = new DateTime(2026, 12, 31),
                    ReleaseMethod = ReleaseMethodEnum.MonthlyEqual, ReleaseFrequency = ReleaseFrequencyEnum.Monthly,
                    ReleaseStartDate = new DateTime(2026, 10, 1),
                    CurrencyId = MasterDataIds.Currencies.INR, OriginalPrepaidAmount = 150000m,
                    FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                    ExpenseGLAccountId = MasterDataIds.Accounts.RentExpense, ExpenseGLAccountName = "6002 – Rent Expense",
                    PrepaymentAssetGLId = MasterDataIds.Accounts.FurnitureFixtures, PrepaymentAssetGLName = "1100 – Furniture & Fixtures",
                    PostingStatus = PostingStatusEnum.NotPosted, ReleaseStatus = ReleaseStatusEnum.NotStarted,
                    PreparedByUserId = "finance.admin", MaterialityLevel = MaterialityLevelEnum.Low,
                    CreatedAt = new DateTime(2026, 4, 10)
                },
                new Prepayment
                {
                    PrepaymentId = Guid.Parse("ab870010-0010-0010-0010-000000000010"),
                    TenantId = MasterDataIds.Tenants.Default, CompanyId = MasterDataIds.Companies.SofaCraft,
                    PrepaymentCode = "PREP-2026-0010", PrepaymentTitle = "Pest Control – Annual Contract",
                    PrepaymentStatus = PrepaymentStatusEnum.InProgress, SourceType = SourceTypeEnum.VendorBill,
                    BasisReferenceText = "Annual pest control service for factory and warehouse",
                    PrepaymentStartDate = new DateTime(2026, 1, 1), PrepaymentEndDate = new DateTime(2026, 12, 31),
                    ReleaseMethod = ReleaseMethodEnum.QuarterlyEqual, ReleaseFrequency = ReleaseFrequencyEnum.Quarterly,
                    ReleaseStartDate = new DateTime(2026, 1, 1), ReleaseEndDate = new DateTime(2026, 12, 31),
                    IsScheduleGenerated = true,
                    CurrencyId = MasterDataIds.Currencies.INR, OriginalPrepaidAmount = 48000m,
                    ReleasedAmountToDate = 12000m,
                    FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                    ExpenseGLAccountId = MasterDataIds.Accounts.UtilitiesExpense, ExpenseGLAccountName = "6003 – Utilities Expense",
                    PrepaymentAssetGLId = MasterDataIds.Accounts.FurnitureFixtures, PrepaymentAssetGLName = "1100 – Furniture & Fixtures",
                    PostingStatus = PostingStatusEnum.PartiallyReleased, ReleaseStatus = ReleaseStatusEnum.InProgress,
                    NextReleaseDueDate = new DateTime(2026, 4, 1), LastReleaseDate = new DateTime(2026, 1, 1),
                    PreparedByUserId = "finance.admin", MaterialityLevel = MaterialityLevelEnum.Low,
                    CreatedAt = new DateTime(2025, 12, 28)
                },
                new Prepayment
                {
                    PrepaymentId = Guid.Parse("ab870011-0011-0011-0011-000000000011"),
                    TenantId = MasterDataIds.Tenants.Default, CompanyId = MasterDataIds.Companies.SofaCraft,
                    PrepaymentCode = "PREP-2026-0011", PrepaymentTitle = "Custom Schedule – Weighted Training Program",
                    PrepaymentStatus = PrepaymentStatusEnum.Approved, SourceType = SourceTypeEnum.ManualFinanceAdjustment,
                    BasisReferenceText = "Staff training program with heavier initial investment – custom release",
                    PrepaymentStartDate = new DateTime(2026, 4, 1), PrepaymentEndDate = new DateTime(2026, 9, 30),
                    ReleaseMethod = ReleaseMethodEnum.CustomSchedule, ReleaseFrequency = ReleaseFrequencyEnum.Custom,
                    ReleaseStartDate = new DateTime(2026, 4, 1), ReleaseEndDate = new DateTime(2026, 9, 30),
                    CurrencyId = MasterDataIds.Currencies.INR, OriginalPrepaidAmount = 240000m,
                    FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                    ExpenseGLAccountId = MasterDataIds.Accounts.SalariesWages, ExpenseGLAccountName = "6001 – Salaries & Wages",
                    PrepaymentAssetGLId = MasterDataIds.Accounts.FurnitureFixtures, PrepaymentAssetGLName = "1100 – Furniture & Fixtures",
                    PostingStatus = PostingStatusEnum.NotPosted, ReleaseStatus = ReleaseStatusEnum.NotStarted,
                    PreparedByUserId = "finance.admin", MaterialityLevel = MaterialityLevelEnum.Medium,
                    CreatedAt = new DateTime(2026, 3, 28)
                },
                new Prepayment
                {
                    PrepaymentId = Guid.Parse("ab870012-0012-0012-0012-000000000012"),
                    TenantId = MasterDataIds.Tenants.Default, CompanyId = MasterDataIds.Companies.SofaCraft,
                    PrepaymentCode = "PREP-2026-0012", PrepaymentTitle = "Cancelled Prepayment – Vendor Refund",
                    PrepaymentStatus = PrepaymentStatusEnum.Cancelled, SourceType = SourceTypeEnum.VendorBill,
                    BasisReferenceText = "Original prepayment cancelled due to vendor contract termination",
                    PrepaymentStartDate = new DateTime(2026, 4, 1), PrepaymentEndDate = new DateTime(2027, 3, 31),
                    ReleaseMethod = ReleaseMethodEnum.MonthlyEqual, ReleaseFrequency = ReleaseFrequencyEnum.Monthly,
                    ReleaseStartDate = new DateTime(2026, 4, 1),
                    CurrencyId = MasterDataIds.Currencies.INR, OriginalPrepaidAmount = 96000m,
                    CancellationReason = "Vendor contract terminated. Full refund received.",
                    FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                    ExpenseGLAccountId = MasterDataIds.Accounts.RentExpense, ExpenseGLAccountName = "6002 – Rent Expense",
                    PrepaymentAssetGLId = MasterDataIds.Accounts.FurnitureFixtures, PrepaymentAssetGLName = "1100 – Furniture & Fixtures",
                    PostingStatus = PostingStatusEnum.NotPosted, ReleaseStatus = ReleaseStatusEnum.NotStarted,
                    PreparedByUserId = "finance.admin", MaterialityLevel = MaterialityLevelEnum.Low,
                    CreatedAt = new DateTime(2026, 3, 15)
                }
            };
        }

        private static List<PrepaymentReleaseLine> GenerateMonthlyLines(Guid parentId, decimal total, DateTime start, int months, int releasedCount)
        {
            var perMonth = Math.Round(total / months, 2);
            var lines = new List<PrepaymentReleaseLine>();
            for (int i = 0; i < months; i++)
            {
                var dt = start.AddMonths(i);
                var released = i < releasedCount;
                lines.Add(new PrepaymentReleaseLine
                {
                    PrepaymentReleaseLineId = Guid.NewGuid(),
                    PrepaymentId = parentId,
                    LineNumber = (i + 1) * 10,
                    ScheduleDate = dt,
                    ScheduledReleaseAmount = (i == months - 1) ? total - perMonth * (months - 1) : perMonth,
                    ReleasedAmount = released ? perMonth : 0,
                    ReleaseLineStatus = released ? ReleaseLineStatusEnum.Released : ReleaseLineStatusEnum.Planned,
                    ReleasedOn = released ? dt.AddDays(1) : null,
                    ReleasedBy = released ? "finance.admin" : null
                });
            }
            return lines;
        }
    }
}
