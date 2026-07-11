using static FinanceConnect.Client.ViewModels.DeferredRevenueViewModel;

namespace FinanceConnect.Client.Data
{
    public static class DeferredRevenueSeedData
    {
        public static List<DeferredRevenue> GetAll()
        {
            return new List<DeferredRevenue>
            {
                new DeferredRevenue
                {
                    DeferredRevenueId = Guid.Parse("db880001-0001-0001-0001-000000000001"),
                    TenantId = MasterDataIds.Tenants.Default, CompanyId = MasterDataIds.Companies.SofaCraft,
                    DeferredRevenueCode = "DEFREV-2026-0001", DeferredRevenueTitle = "Annual Subscription – Premium Support Package",
                    Description = "Customer billed ₹12,00,000 upfront for 12-month premium support subscription.",
                    DeferredRevenueStatus = DeferredRevenueStatusEnum.InProgress,
                    SourceType = SourceTypeEnum.Subscription, SourceDocumentNumber = "INV-SUB-2026-001",
                    CustomerId = MasterDataIds.Accounts.AccountsReceivable, CustomerName = "Luxury Interiors Pvt Ltd",
                    BasisReferenceText = "Annual support invoice billed in advance – coverage Apr 2026 to Mar 2027",
                    DeferredStartDate = new DateTime(2026, 4, 1), DeferredEndDate = new DateTime(2027, 3, 31),
                    ReleaseMethod = ReleaseMethodEnum.MonthlyEqual, ReleaseFrequency = ReleaseFrequencyEnum.Monthly,
                    ReleaseStartDate = new DateTime(2026, 4, 1), ReleaseEndDate = new DateTime(2027, 3, 31),
                    IsScheduleGenerated = true,
                    CurrencyId = MasterDataIds.Currencies.INR, OriginalDeferredAmount = 1200000m,
                    ReleasedToRevenueAmount = 300000m, CurrentPeriodReleaseAmount = 100000m,
                    FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                    RevenueGLAccountId = MasterDataIds.Accounts.ServiceRevenue, RevenueGLAccountName = "4002 – Service Revenue",
                    DeferredRevenueLiabilityGLId = MasterDataIds.Accounts.AccountsPayable, DeferredRevenueLiabilityGLName = "2001 – Accounts Payable",
                    PostingStatus = PostingStatusEnum.PartiallyReleased, ReleaseStatus = ReleaseStatusEnum.InProgress,
                    NextReleaseDueDate = new DateTime(2026, 7, 1), LastReleaseDate = new DateTime(2026, 6, 1),
                    PreparedByUserId = "finance.admin", MaterialityLevel = MaterialityLevelEnum.High,
                    CreatedAt = new DateTime(2026, 3, 28),
                    ReleaseLines = GenerateMonthlyLines(Guid.Parse("db880001-0001-0001-0001-000000000001"), 1200000m, new DateTime(2026, 4, 1), 12, 3)
                },
                new DeferredRevenue
                {
                    DeferredRevenueId = Guid.Parse("db880002-0002-0002-0002-000000000002"),
                    TenantId = MasterDataIds.Tenants.Default, CompanyId = MasterDataIds.Companies.SofaCraft,
                    DeferredRevenueCode = "DEFREV-2026-0002", DeferredRevenueTitle = "Training Fee Deferral – Corporate Batch May",
                    Description = "Training fee collected before training delivery. To be earned over May-June.",
                    DeferredRevenueStatus = DeferredRevenueStatusEnum.Posted,
                    SourceType = SourceTypeEnum.Receipt, SourceDocumentNumber = "REC-TRN-2026-005",
                    CustomerId = MasterDataIds.Accounts.AccountsReceivable, CustomerName = "Elite Home Décor",
                    BasisReferenceText = "Training advance received for future delivery – May-June 2026 batch",
                    DeferredStartDate = new DateTime(2026, 5, 1), DeferredEndDate = new DateTime(2026, 6, 30),
                    ReleaseMethod = ReleaseMethodEnum.MonthlyEqual, ReleaseFrequency = ReleaseFrequencyEnum.Monthly,
                    ReleaseStartDate = new DateTime(2026, 5, 1), ReleaseEndDate = new DateTime(2026, 6, 30),
                    CurrencyId = MasterDataIds.Currencies.INR, OriginalDeferredAmount = 150000m,
                    FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                    RevenueGLAccountId = MasterDataIds.Accounts.ServiceRevenue, RevenueGLAccountName = "4002 – Service Revenue",
                    DeferredRevenueLiabilityGLId = MasterDataIds.Accounts.AccountsPayable, DeferredRevenueLiabilityGLName = "2001 – Accounts Payable",
                    PostingStatus = PostingStatusEnum.Posted, ReleaseStatus = ReleaseStatusEnum.NotStarted,
                    NextReleaseDueDate = new DateTime(2026, 5, 1),
                    PreparedByUserId = "finance.admin", MaterialityLevel = MaterialityLevelEnum.Medium,
                    CreatedAt = new DateTime(2026, 4, 5)
                },
                new DeferredRevenue
                {
                    DeferredRevenueId = Guid.Parse("db880003-0003-0003-0003-000000000003"),
                    TenantId = MasterDataIds.Tenants.Default, CompanyId = MasterDataIds.Companies.SofaCraft,
                    DeferredRevenueCode = "DEFREV-2026-0003", DeferredRevenueTitle = "AMC Contract Deferral – Office Furniture Care",
                    DeferredRevenueStatus = DeferredRevenueStatusEnum.FullyReleased,
                    SourceType = SourceTypeEnum.Contract, SourceDocumentNumber = "AMC-2025-0012",
                    CustomerId = MasterDataIds.Accounts.AccountsReceivable, CustomerName = "Corporate Solutions India",
                    BasisReferenceText = "Annual maintenance contract billed at start – fully earned by Mar 2026",
                    DeferredStartDate = new DateTime(2025, 4, 1), DeferredEndDate = new DateTime(2026, 3, 31),
                    ReleaseMethod = ReleaseMethodEnum.MonthlyEqual, ReleaseFrequency = ReleaseFrequencyEnum.Monthly,
                    ReleaseStartDate = new DateTime(2025, 4, 1), ReleaseEndDate = new DateTime(2026, 3, 31),
                    IsScheduleGenerated = true,
                    CurrencyId = MasterDataIds.Currencies.INR, OriginalDeferredAmount = 480000m,
                    ReleasedToRevenueAmount = 480000m,
                    FiscalYearId = MasterDataIds.FiscalYears.FY2024_25,
                    RevenueGLAccountId = MasterDataIds.Accounts.ServiceRevenue, RevenueGLAccountName = "4002 – Service Revenue",
                    DeferredRevenueLiabilityGLId = MasterDataIds.Accounts.AccountsPayable, DeferredRevenueLiabilityGLName = "2001 – Accounts Payable",
                    PostingStatus = PostingStatusEnum.FullyReleased, ReleaseStatus = ReleaseStatusEnum.FullyReleased,
                    LastReleaseDate = new DateTime(2026, 3, 1), IsLocked = true, LockedOn = new DateTime(2026, 4, 1),
                    PreparedByUserId = "finance.admin", MaterialityLevel = MaterialityLevelEnum.High,
                    CreatedAt = new DateTime(2025, 3, 28)
                },
                new DeferredRevenue
                {
                    DeferredRevenueId = Guid.Parse("db880004-0004-0004-0004-000000000004"),
                    TenantId = MasterDataIds.Tenants.Default, CompanyId = MasterDataIds.Companies.SofaCraft,
                    DeferredRevenueCode = "DEFREV-2026-0004", DeferredRevenueTitle = "Project Advance – Custom Sofa Order Batch",
                    DeferredRevenueStatus = DeferredRevenueStatusEnum.Approved,
                    SourceType = SourceTypeEnum.CustomerInvoice, SourceDocumentNumber = "PI-ADV-2026-008",
                    CustomerId = MasterDataIds.Accounts.AccountsReceivable, CustomerName = "Grand Hotel Group",
                    BasisReferenceText = "Advance invoiced before milestone completion – 3-phase delivery project",
                    DeferredStartDate = new DateTime(2026, 4, 1), DeferredEndDate = new DateTime(2026, 9, 30),
                    ReleaseMethod = ReleaseMethodEnum.CustomSchedule, ReleaseFrequency = ReleaseFrequencyEnum.Custom,
                    ReleaseStartDate = new DateTime(2026, 4, 1), ReleaseEndDate = new DateTime(2026, 9, 30),
                    CurrencyId = MasterDataIds.Currencies.INR, OriginalDeferredAmount = 750000m,
                    FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                    RevenueGLAccountId = MasterDataIds.Accounts.SalesRevenue, RevenueGLAccountName = "4001 – Sales Revenue - Sofas",
                    DeferredRevenueLiabilityGLId = MasterDataIds.Accounts.AccountsPayable, DeferredRevenueLiabilityGLName = "2001 – Accounts Payable",
                    PostingStatus = PostingStatusEnum.NotPosted, ReleaseStatus = ReleaseStatusEnum.NotStarted,
                    PreparedByUserId = "finance.admin", MaterialityLevel = MaterialityLevelEnum.High,
                    CreatedAt = new DateTime(2026, 3, 25)
                },
                new DeferredRevenue
                {
                    DeferredRevenueId = Guid.Parse("db880005-0005-0005-0005-000000000005"),
                    TenantId = MasterDataIds.Tenants.Default, CompanyId = MasterDataIds.Companies.SofaCraft,
                    DeferredRevenueCode = "DEFREV-2026-0005", DeferredRevenueTitle = "Service Retainer – Interior Consulting",
                    DeferredRevenueStatus = DeferredRevenueStatusEnum.Draft,
                    SourceType = SourceTypeEnum.Revenue,
                    CustomerId = MasterDataIds.Accounts.AccountsReceivable, CustomerName = "Prestige Builders",
                    BasisReferenceText = "Monthly retainer collected before consulting service month is consumed",
                    DeferredStartDate = new DateTime(2026, 5, 1), DeferredEndDate = new DateTime(2026, 10, 31),
                    ReleaseMethod = ReleaseMethodEnum.MonthlyEqual, ReleaseFrequency = ReleaseFrequencyEnum.Monthly,
                    ReleaseStartDate = new DateTime(2026, 5, 1),
                    CurrencyId = MasterDataIds.Currencies.INR, OriginalDeferredAmount = 180000m,
                    FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                    RevenueGLAccountId = MasterDataIds.Accounts.ServiceRevenue, RevenueGLAccountName = "4002 – Service Revenue",
                    DeferredRevenueLiabilityGLId = MasterDataIds.Accounts.AccountsPayable, DeferredRevenueLiabilityGLName = "2001 – Accounts Payable",
                    PostingStatus = PostingStatusEnum.NotPosted, ReleaseStatus = ReleaseStatusEnum.NotStarted,
                    PreparedByUserId = "finance.admin", MaterialityLevel = MaterialityLevelEnum.Medium,
                    CreatedAt = new DateTime(2026, 4, 8)
                },
                new DeferredRevenue
                {
                    DeferredRevenueId = Guid.Parse("db880006-0006-0006-0006-000000000006"),
                    TenantId = MasterDataIds.Tenants.Default, CompanyId = MasterDataIds.Companies.SofaCraft,
                    DeferredRevenueCode = "DEFREV-2026-0006", DeferredRevenueTitle = "Warranty Extension Fee – Collected Upfront",
                    DeferredRevenueStatus = DeferredRevenueStatusEnum.InProgress,
                    SourceType = SourceTypeEnum.Receipt, SourceDocumentNumber = "REC-WAR-2026-003",
                    CustomerId = MasterDataIds.Accounts.AccountsReceivable, CustomerName = "Comfort Living LLC",
                    BasisReferenceText = "Extended warranty fee collected at sale – coverage 24 months",
                    DeferredStartDate = new DateTime(2026, 1, 1), DeferredEndDate = new DateTime(2027, 12, 31),
                    ReleaseMethod = ReleaseMethodEnum.QuarterlyEqual, ReleaseFrequency = ReleaseFrequencyEnum.Quarterly,
                    ReleaseStartDate = new DateTime(2026, 1, 1), ReleaseEndDate = new DateTime(2027, 12, 31),
                    IsScheduleGenerated = true,
                    CurrencyId = MasterDataIds.Currencies.INR, OriginalDeferredAmount = 240000m,
                    ReleasedToRevenueAmount = 30000m,
                    FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                    RevenueGLAccountId = MasterDataIds.Accounts.ServiceRevenue, RevenueGLAccountName = "4002 – Service Revenue",
                    DeferredRevenueLiabilityGLId = MasterDataIds.Accounts.AccountsPayable, DeferredRevenueLiabilityGLName = "2001 – Accounts Payable",
                    PostingStatus = PostingStatusEnum.PartiallyReleased, ReleaseStatus = ReleaseStatusEnum.InProgress,
                    NextReleaseDueDate = new DateTime(2026, 4, 1), LastReleaseDate = new DateTime(2026, 1, 1),
                    PreparedByUserId = "finance.admin", MaterialityLevel = MaterialityLevelEnum.Medium,
                    CreatedAt = new DateTime(2025, 12, 28)
                },
                new DeferredRevenue
                {
                    DeferredRevenueId = Guid.Parse("db880007-0007-0007-0007-000000000007"),
                    TenantId = MasterDataIds.Tenants.Default, CompanyId = MasterDataIds.Companies.SofaCraft,
                    DeferredRevenueCode = "DEFREV-2026-0007", DeferredRevenueTitle = "Gift Card Sales – Redeemable Over 6 Months",
                    DeferredRevenueStatus = DeferredRevenueStatusEnum.Submitted,
                    SourceType = SourceTypeEnum.Other,
                    CustomerId = MasterDataIds.Accounts.AccountsReceivable, CustomerName = "Various – Gift Card Pool",
                    BasisReferenceText = "Gift card revenue recognized upon redemption – 6-month validity",
                    DeferredStartDate = new DateTime(2026, 4, 1), DeferredEndDate = new DateTime(2026, 9, 30),
                    ReleaseMethod = ReleaseMethodEnum.ManualRelease,
                    ReleaseStartDate = new DateTime(2026, 4, 1),
                    CurrencyId = MasterDataIds.Currencies.INR, OriginalDeferredAmount = 95000m,
                    FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                    RevenueGLAccountId = MasterDataIds.Accounts.SalesRevenue, RevenueGLAccountName = "4001 – Sales Revenue - Sofas",
                    DeferredRevenueLiabilityGLId = MasterDataIds.Accounts.AccountsPayable, DeferredRevenueLiabilityGLName = "2001 – Accounts Payable",
                    PostingStatus = PostingStatusEnum.NotPosted, ReleaseStatus = ReleaseStatusEnum.NotStarted,
                    PreparedByUserId = "finance.admin", MaterialityLevel = MaterialityLevelEnum.Low,
                    CreatedAt = new DateTime(2026, 3, 30)
                },
                new DeferredRevenue
                {
                    DeferredRevenueId = Guid.Parse("db880008-0008-0008-0008-000000000008"),
                    TenantId = MasterDataIds.Tenants.Default, CompanyId = MasterDataIds.Companies.SofaCraft,
                    DeferredRevenueCode = "DEFREV-2026-0008", DeferredRevenueTitle = "Franchise Fee Deferral – New Dealer Onboarding",
                    DeferredRevenueStatus = DeferredRevenueStatusEnum.Posted,
                    SourceType = SourceTypeEnum.Contract,
                    CustomerId = MasterDataIds.Accounts.AccountsReceivable, CustomerName = "South Zone Dealers Pvt Ltd",
                    BasisReferenceText = "Franchise onboarding fee recognized over 12-month support period",
                    DeferredStartDate = new DateTime(2026, 4, 1), DeferredEndDate = new DateTime(2027, 3, 31),
                    ReleaseMethod = ReleaseMethodEnum.StraightLine, ReleaseFrequency = ReleaseFrequencyEnum.Monthly,
                    ReleaseStartDate = new DateTime(2026, 4, 1), ReleaseEndDate = new DateTime(2027, 3, 31),
                    IsScheduleGenerated = true,
                    CurrencyId = MasterDataIds.Currencies.INR, OriginalDeferredAmount = 600000m,
                    FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                    RevenueGLAccountId = MasterDataIds.Accounts.ServiceRevenue, RevenueGLAccountName = "4002 – Service Revenue",
                    DeferredRevenueLiabilityGLId = MasterDataIds.Accounts.AccountsPayable, DeferredRevenueLiabilityGLName = "2001 – Accounts Payable",
                    PostingStatus = PostingStatusEnum.Posted, ReleaseStatus = ReleaseStatusEnum.NotStarted,
                    NextReleaseDueDate = new DateTime(2026, 4, 1),
                    PreparedByUserId = "finance.admin", MaterialityLevel = MaterialityLevelEnum.High,
                    CreatedAt = new DateTime(2026, 3, 26)
                },
                new DeferredRevenue
                {
                    DeferredRevenueId = Guid.Parse("db880009-0009-0009-0009-000000000009"),
                    TenantId = MasterDataIds.Tenants.Default, CompanyId = MasterDataIds.Companies.SofaCraft,
                    DeferredRevenueCode = "DEFREV-2026-0009", DeferredRevenueTitle = "Closed Deferral – FY24 Subscription",
                    DeferredRevenueStatus = DeferredRevenueStatusEnum.Closed,
                    SourceType = SourceTypeEnum.Subscription,
                    CustomerId = MasterDataIds.Accounts.AccountsReceivable, CustomerName = "Classic Furnishings",
                    BasisReferenceText = "FY24 subscription fully earned and closed",
                    DeferredStartDate = new DateTime(2024, 4, 1), DeferredEndDate = new DateTime(2025, 3, 31),
                    ReleaseMethod = ReleaseMethodEnum.MonthlyEqual, ReleaseFrequency = ReleaseFrequencyEnum.Monthly,
                    ReleaseStartDate = new DateTime(2024, 4, 1), ReleaseEndDate = new DateTime(2025, 3, 31),
                    IsScheduleGenerated = true,
                    CurrencyId = MasterDataIds.Currencies.INR, OriginalDeferredAmount = 360000m,
                    ReleasedToRevenueAmount = 360000m,
                    FiscalYearId = MasterDataIds.FiscalYears.FY2024_25,
                    RevenueGLAccountId = MasterDataIds.Accounts.ServiceRevenue, RevenueGLAccountName = "4002 – Service Revenue",
                    DeferredRevenueLiabilityGLId = MasterDataIds.Accounts.AccountsPayable, DeferredRevenueLiabilityGLName = "2001 – Accounts Payable",
                    PostingStatus = PostingStatusEnum.FullyReleased, ReleaseStatus = ReleaseStatusEnum.FullyReleased,
                    LastReleaseDate = new DateTime(2025, 3, 1), IsLocked = true, LockedOn = new DateTime(2025, 4, 2),
                    PreparedByUserId = "finance.admin", MaterialityLevel = MaterialityLevelEnum.Medium,
                    CreatedAt = new DateTime(2024, 3, 25)
                },
                new DeferredRevenue
                {
                    DeferredRevenueId = Guid.Parse("db880010-0010-0010-0010-000000000010"),
                    TenantId = MasterDataIds.Tenants.Default, CompanyId = MasterDataIds.Companies.SofaCraft,
                    DeferredRevenueCode = "DEFREV-2026-0010", DeferredRevenueTitle = "Maintenance Bundle – Hotel Chain",
                    DeferredRevenueStatus = DeferredRevenueStatusEnum.InProgress,
                    SourceType = SourceTypeEnum.CustomerInvoice, SourceDocumentNumber = "INV-MNT-2026-015",
                    CustomerId = MasterDataIds.Accounts.AccountsReceivable, CustomerName = "Grandeur Hotels Group",
                    BasisReferenceText = "Furniture maintenance bundle invoiced annually – hotel chain",
                    DeferredStartDate = new DateTime(2026, 1, 1), DeferredEndDate = new DateTime(2026, 12, 31),
                    ReleaseMethod = ReleaseMethodEnum.MonthlyEqual, ReleaseFrequency = ReleaseFrequencyEnum.Monthly,
                    ReleaseStartDate = new DateTime(2026, 1, 1), ReleaseEndDate = new DateTime(2026, 12, 31),
                    IsScheduleGenerated = true,
                    CurrencyId = MasterDataIds.Currencies.INR, OriginalDeferredAmount = 540000m,
                    ReleasedToRevenueAmount = 135000m,
                    FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                    RevenueGLAccountId = MasterDataIds.Accounts.ServiceRevenue, RevenueGLAccountName = "4002 – Service Revenue",
                    DeferredRevenueLiabilityGLId = MasterDataIds.Accounts.AccountsPayable, DeferredRevenueLiabilityGLName = "2001 – Accounts Payable",
                    PostingStatus = PostingStatusEnum.PartiallyReleased, ReleaseStatus = ReleaseStatusEnum.InProgress,
                    NextReleaseDueDate = new DateTime(2026, 4, 1), LastReleaseDate = new DateTime(2026, 3, 1),
                    PreparedByUserId = "finance.admin", MaterialityLevel = MaterialityLevelEnum.High,
                    CreatedAt = new DateTime(2025, 12, 20)
                },
                new DeferredRevenue
                {
                    DeferredRevenueId = Guid.Parse("db880011-0011-0011-0011-000000000011"),
                    TenantId = MasterDataIds.Tenants.Default, CompanyId = MasterDataIds.Companies.SofaCraft,
                    DeferredRevenueCode = "DEFREV-2026-0011", DeferredRevenueTitle = "Licensing Fee – Design Patterns IP",
                    DeferredRevenueStatus = DeferredRevenueStatusEnum.Approved,
                    SourceType = SourceTypeEnum.Contract,
                    CustomerId = MasterDataIds.Accounts.AccountsReceivable, CustomerName = "DesignCraft International",
                    BasisReferenceText = "IP licensing fee collected upfront for 2-year design pattern usage rights",
                    DeferredStartDate = new DateTime(2026, 4, 1), DeferredEndDate = new DateTime(2028, 3, 31),
                    ReleaseMethod = ReleaseMethodEnum.QuarterlyEqual, ReleaseFrequency = ReleaseFrequencyEnum.Quarterly,
                    ReleaseStartDate = new DateTime(2026, 4, 1), ReleaseEndDate = new DateTime(2028, 3, 31),
                    CurrencyId = MasterDataIds.Currencies.USD, OriginalDeferredAmount = 24000m,
                    FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                    RevenueGLAccountId = MasterDataIds.Accounts.ServiceRevenue, RevenueGLAccountName = "4002 – Service Revenue",
                    DeferredRevenueLiabilityGLId = MasterDataIds.Accounts.AccountsPayable, DeferredRevenueLiabilityGLName = "2001 – Accounts Payable",
                    PostingStatus = PostingStatusEnum.NotPosted, ReleaseStatus = ReleaseStatusEnum.NotStarted,
                    PreparedByUserId = "finance.admin", MaterialityLevel = MaterialityLevelEnum.Medium,
                    CreatedAt = new DateTime(2026, 3, 22)
                },
                new DeferredRevenue
                {
                    DeferredRevenueId = Guid.Parse("db880012-0012-0012-0012-000000000012"),
                    TenantId = MasterDataIds.Tenants.Default, CompanyId = MasterDataIds.Companies.SofaCraft,
                    DeferredRevenueCode = "DEFREV-2026-0012", DeferredRevenueTitle = "Cancelled Deferral – Customer Refund",
                    DeferredRevenueStatus = DeferredRevenueStatusEnum.Cancelled,
                    SourceType = SourceTypeEnum.CustomerInvoice,
                    CustomerId = MasterDataIds.Accounts.AccountsReceivable, CustomerName = "Refund Case – Cancelled Order",
                    BasisReferenceText = "Original deferral cancelled due to customer order cancellation and full refund",
                    DeferredStartDate = new DateTime(2026, 4, 1), DeferredEndDate = new DateTime(2026, 9, 30),
                    ReleaseMethod = ReleaseMethodEnum.MonthlyEqual, ReleaseFrequency = ReleaseFrequencyEnum.Monthly,
                    ReleaseStartDate = new DateTime(2026, 4, 1),
                    CancellationReason = "Customer order cancelled. Full advance refund processed.",
                    CurrencyId = MasterDataIds.Currencies.INR, OriginalDeferredAmount = 200000m,
                    FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                    RevenueGLAccountId = MasterDataIds.Accounts.SalesRevenue, RevenueGLAccountName = "4001 – Sales Revenue - Sofas",
                    DeferredRevenueLiabilityGLId = MasterDataIds.Accounts.AccountsPayable, DeferredRevenueLiabilityGLName = "2001 – Accounts Payable",
                    PostingStatus = PostingStatusEnum.NotPosted, ReleaseStatus = ReleaseStatusEnum.NotStarted,
                    PreparedByUserId = "finance.admin", MaterialityLevel = MaterialityLevelEnum.Low,
                    CreatedAt = new DateTime(2026, 3, 15)
                }
            };
        }

        private static List<DeferredRevenueReleaseLine> GenerateMonthlyLines(Guid parentId, decimal total, DateTime start, int months, int releasedCount)
        {
            var perMonth = Math.Round(total / months, 2);
            var lines = new List<DeferredRevenueReleaseLine>();
            for (int i = 0; i < months; i++)
            {
                var dt = start.AddMonths(i);
                var released = i < releasedCount;
                lines.Add(new DeferredRevenueReleaseLine
                {
                    DeferredRevenueReleaseLineId = Guid.NewGuid(),
                    DeferredRevenueId = parentId,
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
