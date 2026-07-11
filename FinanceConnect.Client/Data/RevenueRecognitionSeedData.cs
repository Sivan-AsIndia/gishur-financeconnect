using FinanceConnect.Client.Data;
using static FinanceConnect.Client.ViewModels.RevenueRecognitionViewModel;

namespace FinanceConnect.Client.Data
{
    public static class RevenueRecognitionSeedData
    {
        // ── Well-known IDs ─────────────────────────────────────────────────────
        public static readonly Guid RR01 = Guid.Parse("82000001-0001-0001-0001-000000000001");
        public static readonly Guid RR02 = Guid.Parse("82000002-0002-0002-0002-000000000002");
        public static readonly Guid RR03 = Guid.Parse("82000003-0003-0003-0003-000000000003");
        public static readonly Guid RR04 = Guid.Parse("82000004-0004-0004-0004-000000000004");
        public static readonly Guid RR05 = Guid.Parse("82000005-0005-0005-0005-000000000005");
        public static readonly Guid RR06 = Guid.Parse("82000006-0006-0006-0006-000000000006");
        public static readonly Guid RR07 = Guid.Parse("82000007-0007-0007-0007-000000000007");
        public static readonly Guid RR08 = Guid.Parse("82000008-0008-0008-0008-000000000008");
        public static readonly Guid RR09 = Guid.Parse("82000009-0009-0009-0009-000000000009");
        public static readonly Guid RR10 = Guid.Parse("82000010-0010-0010-0010-000000000010");
        public static readonly Guid RR11 = Guid.Parse("82000011-0011-0011-0011-000000000011");
        public static readonly Guid RR12 = Guid.Parse("82000012-0012-0012-0012-000000000012");

        public static List<RevenueRecognition> GetAll()
        {
            return new List<RevenueRecognition>
            {
                // 01 – Monthly Subscription Recognition (Scheduled, StraightLineOverTime)
                new RevenueRecognition
                {
                    RevenueRecognitionId      = RR01,
                    TenantId                  = MasterDataIds.Tenants.Default,
                    CompanyId                 = MasterDataIds.Companies.SofaCraft,
                    RecognitionCode           = "REVREC-2026-001",
                    RecognitionName           = "Revenue Recognition – Annual Subscription ABC Traders",
                    Description               = "Monthly straight-line recognition of annual SaaS subscription revenue.",
                    RecognitionStatus         = RecognitionStatusEnum.InProgress,
                    RevenueId                 = RevenueSeedData.Rev01,
                    RevenueCodeSnapshot       = "REV-2026-0001",
                    RevenueNameSnapshot       = "Annual Subscription – ABC Traders Pvt Ltd",
                    CustomerId                = CustomerSeedData.Customer1Id,
                    CustomerNameSnapshot      = "ABC Traders Private Limited",
                    SourceDocumentTypeSnapshot = "Subscription",
                    SourceDocumentNumberSnapshot = "SUB-2026-001",
                    RevenueTypeSnapshot       = "Subscription",
                    RevenueNatureSnapshot     = "EarnedOverTime",
                    SourceGrossRevenueAmount  = 120000m,
                    CurrencyId                = MasterDataIds.Currencies.INR,
                    RecognitionMethod         = RecognitionMethodEnum.Scheduled,
                    RecognitionBasis          = RecognitionBasisEnum.StraightLineOverTime,
                    RecognitionStartDate      = new DateTime(2026, 4, 1),
                    RecognitionEndDate        = new DateTime(2027, 3, 31),
                    RecognitionFrequency      = RecognitionFrequencyEnum.Monthly,
                    ScheduleTemplateCode      = "STRAIGHT_LINE_MONTHLY",
                    IsScheduleGenerated       = true,
                    TotalRecognizableAmount   = 120000m,
                    RecognizedAmountToDate    = 10000m,
                    CurrentPeriodRecognitionAmount = 10000m,
                    FiscalYearId              = MasterDataIds.FiscalYears.FY2025_26,
                    RecognitionPostingDate    = new DateTime(2026, 4, 30),
                    LastRecognitionRunDate    = new DateTime(2026, 4, 30),
                    NextRecognitionDueDate    = new DateTime(2026, 5, 31),
                    RecognizedPeriodsCount    = 1,
                    PendingPeriodsCount       = 11,
                    PreparedByUserId          = "finance.admin",
                    PreparedOn                = new DateTime(2026, 4, 1),
                    Notes                     = "12 monthly equal installments of ₹10,000 each.",
                    CreatedAt                 = new DateTime(2026, 4, 1),
                    ScheduleLines = GenerateMonthlyLines(RR01, new DateTime(2026, 4, 1), 12, 10000m, 1)
                },

                // 02 – Immediate Recognition (Consulting)
                new RevenueRecognition
                {
                    RevenueRecognitionId      = RR02,
                    TenantId                  = MasterDataIds.Tenants.Default,
                    CompanyId                 = MasterDataIds.Companies.SofaCraft,
                    RecognitionCode           = "REVREC-2026-002",
                    RecognitionName           = "Immediate Recognition – Consulting Govt Education Dept",
                    Description               = "One-time consulting revenue recognized immediately at delivery.",
                    RecognitionStatus         = RecognitionStatusEnum.FullyRecognized,
                    RevenueId                 = RevenueSeedData.Rev02,
                    RevenueCodeSnapshot       = "REV-2026-0002",
                    RevenueNameSnapshot       = "Consulting Services – State Government Education Dept",
                    CustomerId                = CustomerSeedData.Customer2Id,
                    CustomerNameSnapshot      = "State Government Education Dept",
                    SourceDocumentTypeSnapshot = "CustomerInvoice",
                    SourceDocumentNumberSnapshot = "INV-2026-0032",
                    RevenueTypeSnapshot       = "OneTime",
                    RevenueNatureSnapshot     = "EarnedImmediately",
                    SourceGrossRevenueAmount  = 85000m,
                    CurrencyId                = MasterDataIds.Currencies.INR,
                    RecognitionMethod         = RecognitionMethodEnum.Immediate,
                    RecognitionBasis          = RecognitionBasisEnum.PointInTime,
                    RecognitionStartDate      = new DateTime(2026, 3, 15),
                    TotalRecognizableAmount   = 85000m,
                    RecognizedAmountToDate    = 85000m,
                    CurrentPeriodRecognitionAmount = 85000m,
                    FiscalYearId              = MasterDataIds.FiscalYears.FY2025_26,
                    RecognitionPostingDate    = new DateTime(2026, 3, 15),
                    LastRecognitionRunDate    = new DateTime(2026, 3, 15),
                    RecognizedPeriodsCount    = 1,
                    PendingPeriodsCount       = 0,
                    IsLocked                  = true,
                    LockedOn                  = new DateTime(2026, 3, 16),
                    LockedBy                  = "finance.admin",
                    PreparedByUserId          = "finance.admin",
                    ReviewedByUserId          = "controller",
                    ApprovedByUserId          = "controller",
                    PreparedOn                = new DateTime(2026, 3, 15),
                    ReviewedOn                = new DateTime(2026, 3, 15),
                    ApprovedOn                = new DateTime(2026, 3, 15),
                    Notes                     = "Full recognition on service delivery completion.",
                    CreatedAt                 = new DateTime(2026, 3, 15),
                    ScheduleLines = new List<RevenueRecognitionLine>
                    {
                        new RevenueRecognitionLine
                        {
                            RevenueRecognitionLineId = Guid.Parse("82010001-0001-0001-0001-000000000001"),
                            RevenueRecognitionId = RR02,
                            LineNumber = 10,
                            ScheduleDate = new DateTime(2026, 3, 15),
                            ScheduledAmount = 85000m,
                            RecognizedAmount = 85000m,
                            RecognitionLineStatus = RecognitionLineStatusEnum.Recognized,
                            ManualApprovalStatus = ManualApprovalStatusEnum.NotRequired,
                            RecognizedOn = new DateTime(2026, 3, 15),
                            RecognizedBy = "finance.admin"
                        }
                    }
                },

                // 03 – Milestone-Triggered Recognition (Project Alpha)
                new RevenueRecognition
                {
                    RevenueRecognitionId      = RR03,
                    TenantId                  = MasterDataIds.Tenants.Default,
                    CompanyId                 = MasterDataIds.Companies.SofaCraft,
                    RecognitionCode           = "REVREC-2026-003",
                    RecognitionName           = "Milestone Recognition – Project Alpha Infrastructure",
                    Description               = "Revenue recognized on milestone completion (30-30-40 split).",
                    RecognitionStatus         = RecognitionStatusEnum.PartiallyRecognized,
                    RevenueId                 = RevenueSeedData.Rev03,
                    RevenueCodeSnapshot       = "REV-2026-0003",
                    RevenueNameSnapshot       = "Project Alpha – Infrastructure Build",
                    CustomerId                = CustomerSeedData.Customer3Id,
                    CustomerNameSnapshot      = "Metro Infrastructure Corp",
                    SourceDocumentTypeSnapshot = "Milestone",
                    SourceDocumentNumberSnapshot = "MS-ALPHA-001",
                    RevenueTypeSnapshot       = "MilestoneBased",
                    RevenueNatureSnapshot     = "EarnedOnMilestone",
                    SourceGrossRevenueAmount  = 500000m,
                    CurrencyId                = MasterDataIds.Currencies.INR,
                    RecognitionMethod         = RecognitionMethodEnum.MilestoneTriggered,
                    RecognitionBasis          = RecognitionBasisEnum.MilestoneCompletion,
                    RecognitionStartDate      = new DateTime(2026, 1, 15),
                    RecognitionEndDate        = new DateTime(2026, 12, 31),
                    ScheduleTemplateCode      = "MILESTONE_30_30_40",
                    MilestoneTriggerRequired  = true,
                    IsScheduleGenerated       = true,
                    TotalRecognizableAmount   = 500000m,
                    RecognizedAmountToDate    = 150000m,
                    CurrentPeriodRecognitionAmount = 0m,
                    FiscalYearId              = MasterDataIds.FiscalYears.FY2025_26,
                    LastRecognitionRunDate    = new DateTime(2026, 3, 1),
                    RecognizedPeriodsCount    = 1,
                    PendingPeriodsCount       = 2,
                    PreparedByUserId          = "finance.admin",
                    PreparedOn                = new DateTime(2026, 1, 15),
                    Notes                     = "Milestone 1 completed and approved. Milestone 2 and 3 pending.",
                    CreatedAt                 = new DateTime(2026, 1, 15),
                    ScheduleLines = new List<RevenueRecognitionLine>
                    {
                        new RevenueRecognitionLine
                        {
                            RevenueRecognitionLineId = Guid.Parse("82030001-0001-0001-0001-000000000001"),
                            RevenueRecognitionId = RR03, LineNumber = 10,
                            ScheduleDate = new DateTime(2026, 3, 1),
                            ScheduledAmount = 150000m, RecognizedAmount = 150000m,
                            RecognitionLineStatus = RecognitionLineStatusEnum.Recognized,
                            MilestoneReference = "Milestone 1 – Foundation Complete",
                            TriggerEventReference = "Project signoff dated 01-Mar-2026",
                            ManualApprovalStatus = ManualApprovalStatusEnum.Approved,
                            RecognizedOn = new DateTime(2026, 3, 1), RecognizedBy = "controller"
                        },
                        new RevenueRecognitionLine
                        {
                            RevenueRecognitionLineId = Guid.Parse("82030002-0002-0002-0002-000000000002"),
                            RevenueRecognitionId = RR03, LineNumber = 20,
                            ScheduleDate = new DateTime(2026, 7, 1),
                            ScheduledAmount = 150000m, RecognizedAmount = 0m,
                            RecognitionLineStatus = RecognitionLineStatusEnum.Planned,
                            MilestoneReference = "Milestone 2 – Structure Complete",
                            ManualApprovalStatus = ManualApprovalStatusEnum.Pending
                        },
                        new RevenueRecognitionLine
                        {
                            RevenueRecognitionLineId = Guid.Parse("82030003-0003-0003-0003-000000000003"),
                            RevenueRecognitionId = RR03, LineNumber = 30,
                            ScheduleDate = new DateTime(2026, 12, 1),
                            ScheduledAmount = 200000m, RecognizedAmount = 0m,
                            RecognitionLineStatus = RecognitionLineStatusEnum.Planned,
                            MilestoneReference = "Milestone 3 – Final Delivery",
                            ManualApprovalStatus = ManualApprovalStatusEnum.Pending
                        }
                    }
                },

                // 04 – Manual Approval Recognition (Training Revenue)
                new RevenueRecognition
                {
                    RevenueRecognitionId      = RR04,
                    TenantId                  = MasterDataIds.Tenants.Default,
                    CompanyId                 = MasterDataIds.Companies.SofaCraft,
                    RecognitionCode           = "REVREC-2026-004",
                    RecognitionName           = "Manual Approval – Corporate Training Programme",
                    Description               = "Recognition requires manual approval after each training batch.",
                    RecognitionStatus         = RecognitionStatusEnum.Ready,
                    RevenueId                 = RevenueSeedData.Rev04,
                    RevenueCodeSnapshot       = "REV-2026-0004",
                    RevenueNameSnapshot       = "Corporate Training – National Bank of India",
                    CustomerId                = CustomerSeedData.Customer4Id,
                    CustomerNameSnapshot      = "National Bank of India",
                    SourceDocumentTypeSnapshot = "Contract",
                    SourceDocumentNumberSnapshot = "CTR-TRAIN-2026-001",
                    RevenueTypeSnapshot       = "ServiceBased",
                    RevenueNatureSnapshot     = "EarnedOverTime",
                    SourceGrossRevenueAmount  = 240000m,
                    CurrencyId                = MasterDataIds.Currencies.INR,
                    RecognitionMethod         = RecognitionMethodEnum.ManualApprovalRequired,
                    RecognitionBasis          = RecognitionBasisEnum.DeliveryBased,
                    RecognitionStartDate      = new DateTime(2026, 4, 1),
                    RecognitionEndDate        = new DateTime(2026, 9, 30),
                    ManualApprovalRequiredFlag = true,
                    IsScheduleGenerated       = true,
                    TotalRecognizableAmount   = 240000m,
                    RecognizedAmountToDate    = 0m,
                    FiscalYearId              = MasterDataIds.FiscalYears.FY2025_26,
                    NextRecognitionDueDate    = new DateTime(2026, 4, 30),
                    PendingPeriodsCount       = 6,
                    PreparedByUserId          = "finance.admin",
                    PreparedOn                = new DateTime(2026, 3, 25),
                    Notes                     = "Awaiting first batch delivery confirmation.",
                    CreatedAt                 = new DateTime(2026, 3, 25),
                    ScheduleLines = GenerateMonthlyLines(RR04, new DateTime(2026, 4, 1), 6, 40000m, 0)
                },

                // 05 – Deferred Then Release (Annual Support Contract)
                new RevenueRecognition
                {
                    RevenueRecognitionId      = RR05,
                    TenantId                  = MasterDataIds.Tenants.Default,
                    CompanyId                 = MasterDataIds.Companies.SofaCraft,
                    RecognitionCode           = "REVREC-2026-005",
                    RecognitionName           = "Deferred Release – Annual Support Contract",
                    Description               = "Advance-billed support contract released monthly from deferred revenue.",
                    RecognitionStatus         = RecognitionStatusEnum.InProgress,
                    RevenueId                 = RevenueSeedData.Rev05,
                    RevenueCodeSnapshot       = "REV-2026-0005",
                    RevenueNameSnapshot       = "Annual Support Contract – TechServ Solutions",
                    CustomerId                = CustomerSeedData.Customer1Id,
                    CustomerNameSnapshot      = "ABC Traders Private Limited",
                    SourceDocumentTypeSnapshot = "Contract",
                    SourceDocumentNumberSnapshot = "SUP-2026-001",
                    RevenueTypeSnapshot       = "Recurring",
                    RevenueNatureSnapshot     = "UnearnedAdvance",
                    SourceGrossRevenueAmount  = 180000m,
                    CurrencyId                = MasterDataIds.Currencies.INR,
                    RecognitionMethod         = RecognitionMethodEnum.DeferredThenRelease,
                    RecognitionBasis          = RecognitionBasisEnum.ServiceCoveragePeriod,
                    RecognitionStartDate      = new DateTime(2026, 1, 1),
                    RecognitionEndDate        = new DateTime(2026, 12, 31),
                    RecognitionFrequency      = RecognitionFrequencyEnum.Monthly,
                    DeferredRevenueId         = Guid.Parse("88000001-0001-0001-0001-000000000001"),
                    DeferredRevenueReference  = "DEFREV-2026-001",
                    IsScheduleGenerated       = true,
                    TotalRecognizableAmount   = 180000m,
                    RecognizedAmountToDate    = 45000m,
                    CurrentPeriodRecognitionAmount = 15000m,
                    FiscalYearId              = MasterDataIds.FiscalYears.FY2025_26,
                    RecognitionPostingDate    = new DateTime(2026, 3, 31),
                    LastRecognitionRunDate    = new DateTime(2026, 3, 31),
                    NextRecognitionDueDate    = new DateTime(2026, 4, 30),
                    RecognizedPeriodsCount    = 3,
                    PendingPeriodsCount       = 9,
                    PreparedByUserId          = "finance.admin",
                    PreparedOn                = new DateTime(2026, 1, 1),
                    ReviewedByUserId          = "controller",
                    ReviewedOn                = new DateTime(2026, 1, 2),
                    Notes                     = "Monthly release of ₹15,000 from deferred revenue.",
                    CreatedAt                 = new DateTime(2026, 1, 1),
                    ScheduleLines = GenerateMonthlyLines(RR05, new DateTime(2026, 1, 1), 12, 15000m, 3)
                },

                // 06 – Quarterly Recognition (Franchise Fee)
                new RevenueRecognition
                {
                    RevenueRecognitionId      = RR06,
                    TenantId                  = MasterDataIds.Tenants.Default,
                    CompanyId                 = MasterDataIds.Companies.SofaCraft,
                    RecognitionCode           = "REVREC-2026-006",
                    RecognitionName           = "Quarterly Recognition – Franchise Royalty Revenue",
                    Description               = "Quarterly recognition of franchise royalty income.",
                    RecognitionStatus         = RecognitionStatusEnum.Scheduled,
                    RevenueId                 = RevenueSeedData.Rev06,
                    RevenueCodeSnapshot       = "REV-2026-0006",
                    RevenueNameSnapshot       = "Franchise Royalty – South Region Dealer Network",
                    CustomerId                = CustomerSeedData.Customer2Id,
                    CustomerNameSnapshot      = "State Government Education Dept",
                    SourceDocumentTypeSnapshot = "Contract",
                    SourceDocumentNumberSnapshot = "FRAN-2026-001",
                    RevenueTypeSnapshot       = "Recurring",
                    RevenueNatureSnapshot     = "EarnedOverTime",
                    SourceGrossRevenueAmount  = 400000m,
                    CurrencyId                = MasterDataIds.Currencies.INR,
                    RecognitionMethod         = RecognitionMethodEnum.Scheduled,
                    RecognitionBasis          = RecognitionBasisEnum.StraightLineOverTime,
                    RecognitionStartDate      = new DateTime(2026, 4, 1),
                    RecognitionEndDate        = new DateTime(2027, 3, 31),
                    RecognitionFrequency      = RecognitionFrequencyEnum.Quarterly,
                    IsScheduleGenerated       = true,
                    TotalRecognizableAmount   = 400000m,
                    RecognizedAmountToDate    = 0m,
                    FiscalYearId              = MasterDataIds.FiscalYears.FY2025_26,
                    NextRecognitionDueDate    = new DateTime(2026, 6, 30),
                    PendingPeriodsCount       = 4,
                    PreparedByUserId          = "finance.admin",
                    PreparedOn                = new DateTime(2026, 3, 28),
                    Notes                     = "4 quarterly installments of ₹1,00,000 each.",
                    CreatedAt                 = new DateTime(2026, 3, 28),
                    ScheduleLines = new List<RevenueRecognitionLine>
                    {
                        new RevenueRecognitionLine { RevenueRecognitionLineId = Guid.Parse("82060001-0001-0001-0001-000000000001"), RevenueRecognitionId = RR06, LineNumber = 10, ScheduleDate = new DateTime(2026, 6, 30), ScheduledAmount = 100000m, RecognizedAmount = 0, RecognitionLineStatus = RecognitionLineStatusEnum.Planned, ManualApprovalStatus = ManualApprovalStatusEnum.NotRequired },
                        new RevenueRecognitionLine { RevenueRecognitionLineId = Guid.Parse("82060002-0002-0002-0002-000000000002"), RevenueRecognitionId = RR06, LineNumber = 20, ScheduleDate = new DateTime(2026, 9, 30), ScheduledAmount = 100000m, RecognizedAmount = 0, RecognitionLineStatus = RecognitionLineStatusEnum.Planned, ManualApprovalStatus = ManualApprovalStatusEnum.NotRequired },
                        new RevenueRecognitionLine { RevenueRecognitionLineId = Guid.Parse("82060003-0003-0003-0003-000000000003"), RevenueRecognitionId = RR06, LineNumber = 30, ScheduleDate = new DateTime(2026, 12, 31), ScheduledAmount = 100000m, RecognizedAmount = 0, RecognitionLineStatus = RecognitionLineStatusEnum.Planned, ManualApprovalStatus = ManualApprovalStatusEnum.NotRequired },
                        new RevenueRecognitionLine { RevenueRecognitionLineId = Guid.Parse("82060004-0004-0004-0004-000000000004"), RevenueRecognitionId = RR06, LineNumber = 40, ScheduleDate = new DateTime(2027, 3, 31), ScheduledAmount = 100000m, RecognizedAmount = 0, RecognitionLineStatus = RecognitionLineStatusEnum.Planned, ManualApprovalStatus = ManualApprovalStatusEnum.NotRequired }
                    }
                },

                // 07 – Draft Recognition (Pending Setup)
                new RevenueRecognition
                {
                    RevenueRecognitionId      = RR07,
                    TenantId                  = MasterDataIds.Tenants.Default,
                    CompanyId                 = MasterDataIds.Companies.SofaCraft,
                    RecognitionCode           = "REVREC-2026-007",
                    RecognitionName           = "Draft – Licensing Agreement Revenue",
                    Description               = "Recognition schedule not yet generated for licensing revenue.",
                    RecognitionStatus         = RecognitionStatusEnum.Draft,
                    RevenueId                 = RevenueSeedData.Rev07,
                    RevenueCodeSnapshot       = "REV-2026-0007",
                    RevenueNameSnapshot       = "Software License – Premium Enterprise Package",
                    CustomerId                = CustomerSeedData.Customer3Id,
                    CustomerNameSnapshot      = "Metro Infrastructure Corp",
                    SourceDocumentTypeSnapshot = "Contract",
                    RevenueTypeSnapshot       = "Subscription",
                    RevenueNatureSnapshot     = "EarnedOverTime",
                    SourceGrossRevenueAmount  = 360000m,
                    CurrencyId                = MasterDataIds.Currencies.INR,
                    RecognitionMethod         = RecognitionMethodEnum.Scheduled,
                    RecognitionBasis          = RecognitionBasisEnum.StraightLineOverTime,
                    TotalRecognizableAmount   = 360000m,
                    FiscalYearId              = MasterDataIds.FiscalYears.FY2025_26,
                    PreparedByUserId          = "finance.admin",
                    PreparedOn                = new DateTime(2026, 3, 27),
                    Notes                     = "Awaiting contract finalization before schedule generation.",
                    CreatedAt                 = new DateTime(2026, 3, 27)
                },

                // 08 – On Hold (Disputed Revenue)
                new RevenueRecognition
                {
                    RevenueRecognitionId      = RR08,
                    TenantId                  = MasterDataIds.Tenants.Default,
                    CompanyId                 = MasterDataIds.Companies.SofaCraft,
                    RecognitionCode           = "REVREC-2026-008",
                    RecognitionName           = "On Hold – Disputed Product Sales Revenue",
                    Description               = "Recognition paused due to customer dispute on product quality.",
                    RecognitionStatus         = RecognitionStatusEnum.OnHold,
                    RevenueId                 = RevenueSeedData.Rev08,
                    RevenueCodeSnapshot       = "REV-2026-0008",
                    RevenueNameSnapshot       = "Bulk Furniture Sales – Quality Dispute",
                    CustomerId                = CustomerSeedData.Customer4Id,
                    CustomerNameSnapshot      = "National Bank of India",
                    SourceDocumentTypeSnapshot = "CustomerInvoice",
                    SourceDocumentNumberSnapshot = "INV-2026-0088",
                    RevenueTypeSnapshot       = "OneTime",
                    RevenueNatureSnapshot     = "EarnedImmediately",
                    SourceGrossRevenueAmount  = 95000m,
                    CurrencyId                = MasterDataIds.Currencies.INR,
                    RecognitionMethod         = RecognitionMethodEnum.Immediate,
                    RecognitionBasis          = RecognitionBasisEnum.PointInTime,
                    TotalRecognizableAmount   = 95000m,
                    RecognizedAmountToDate    = 0m,
                    FiscalYearId              = MasterDataIds.FiscalYears.FY2025_26,
                    PreparedByUserId          = "finance.admin",
                    PreparedOn                = new DateTime(2026, 3, 10),
                    Notes                     = "Customer raised quality complaint. Recognition on hold until resolution.",
                    CreatedAt                 = new DateTime(2026, 3, 10)
                },

                // 09 – Cancelled Recognition
                new RevenueRecognition
                {
                    RevenueRecognitionId      = RR09,
                    TenantId                  = MasterDataIds.Tenants.Default,
                    CompanyId                 = MasterDataIds.Companies.SofaCraft,
                    RecognitionCode           = "REVREC-2026-009",
                    RecognitionName           = "Cancelled – Event Sponsorship Revenue",
                    Description               = "Event was cancelled; recognition reversed and cancelled.",
                    RecognitionStatus         = RecognitionStatusEnum.Cancelled,
                    RevenueId                 = RevenueSeedData.Rev09,
                    RevenueCodeSnapshot       = "REV-2026-0009",
                    RevenueNameSnapshot       = "Event Sponsorship – Industry Summit 2026",
                    CustomerId                = CustomerSeedData.Customer1Id,
                    CustomerNameSnapshot      = "ABC Traders Private Limited",
                    SourceDocumentTypeSnapshot = "ManualRevenueEvent",
                    RevenueTypeSnapshot       = "OneTime",
                    RevenueNatureSnapshot     = "EarnedImmediately",
                    SourceGrossRevenueAmount  = 50000m,
                    CurrencyId                = MasterDataIds.Currencies.INR,
                    RecognitionMethod         = RecognitionMethodEnum.Immediate,
                    RecognitionBasis          = RecognitionBasisEnum.PointInTime,
                    TotalRecognizableAmount   = 50000m,
                    RecognizedAmountToDate    = 0m,
                    FiscalYearId              = MasterDataIds.FiscalYears.FY2025_26,
                    CancellationReason        = "Event cancelled by organizer. Refund issued to customer.",
                    PreparedByUserId          = "finance.admin",
                    PreparedOn                = new DateTime(2026, 2, 15),
                    Notes                     = "Full cancellation. No revenue to recognize.",
                    CreatedAt                 = new DateTime(2026, 2, 15)
                },

                // 10 – Closed & Locked (Fully Recognized, historical)
                new RevenueRecognition
                {
                    RevenueRecognitionId      = RR10,
                    TenantId                  = MasterDataIds.Tenants.Default,
                    CompanyId                 = MasterDataIds.Companies.SofaCraft,
                    RecognitionCode           = "REVREC-2025-010",
                    RecognitionName           = "Closed – FY25 Annual Maintenance Contract Revenue",
                    Description               = "Prior year maintenance contract fully recognized and closed.",
                    RecognitionStatus         = RecognitionStatusEnum.Closed,
                    RevenueId                 = RevenueSeedData.Rev10,
                    RevenueCodeSnapshot       = "REV-2025-0010",
                    RevenueNameSnapshot       = "Annual Maintenance – GreenField Industries",
                    CustomerId                = CustomerSeedData.Customer2Id,
                    CustomerNameSnapshot      = "State Government Education Dept",
                    SourceDocumentTypeSnapshot = "Contract",
                    RevenueTypeSnapshot       = "Recurring",
                    RevenueNatureSnapshot     = "EarnedOverTime",
                    SourceGrossRevenueAmount  = 96000m,
                    CurrencyId                = MasterDataIds.Currencies.INR,
                    RecognitionMethod         = RecognitionMethodEnum.Scheduled,
                    RecognitionBasis          = RecognitionBasisEnum.StraightLineOverTime,
                    RecognitionStartDate      = new DateTime(2025, 4, 1),
                    RecognitionEndDate        = new DateTime(2026, 3, 31),
                    RecognitionFrequency      = RecognitionFrequencyEnum.Monthly,
                    IsScheduleGenerated       = true,
                    TotalRecognizableAmount   = 96000m,
                    RecognizedAmountToDate    = 96000m,
                    RecognizedPeriodsCount    = 12,
                    PendingPeriodsCount       = 0,
                    FiscalYearId              = MasterDataIds.FiscalYears.FY2024_25,
                    IsLocked                  = true,
                    LockedOn                  = new DateTime(2026, 3, 31),
                    LockedBy                  = "controller",
                    PreparedByUserId          = "finance.admin",
                    ReviewedByUserId          = "controller",
                    ApprovedByUserId          = "controller",
                    PreparedOn                = new DateTime(2025, 4, 1),
                    ReviewedOn                = new DateTime(2026, 3, 31),
                    ApprovedOn                = new DateTime(2026, 3, 31),
                    Notes                     = "All 12 months recognized. Closed for FY2024-25 audit.",
                    CreatedAt                 = new DateTime(2025, 4, 1)
                },

                // 11 – Delivery-Based Recognition (Product Revenue)
                new RevenueRecognition
                {
                    RevenueRecognitionId      = RR11,
                    TenantId                  = MasterDataIds.Tenants.Default,
                    CompanyId                 = MasterDataIds.Companies.SofaCraft,
                    RecognitionCode           = "REVREC-2026-011",
                    RecognitionName           = "Delivery-Based – Furniture Order Batch Recognition",
                    Description               = "Revenue recognized upon delivery confirmation of furniture batches.",
                    RecognitionStatus         = RecognitionStatusEnum.PartiallyRecognized,
                    RevenueId                 = RevenueSeedData.Rev11,
                    RevenueCodeSnapshot       = "REV-2026-0011",
                    RevenueNameSnapshot       = "Bulk Sofa Order – Corporate Office Fit-out",
                    CustomerId                = CustomerSeedData.Customer3Id,
                    CustomerNameSnapshot      = "Metro Infrastructure Corp",
                    SourceDocumentTypeSnapshot = "ProjectDeliverable",
                    SourceDocumentNumberSnapshot = "DEL-2026-045",
                    RevenueTypeSnapshot       = "ProjectBased",
                    RevenueNatureSnapshot     = "EarnedOnMilestone",
                    SourceGrossRevenueAmount  = 320000m,
                    CurrencyId                = MasterDataIds.Currencies.INR,
                    RecognitionMethod         = RecognitionMethodEnum.MilestoneTriggered,
                    RecognitionBasis          = RecognitionBasisEnum.DeliveryBased,
                    RecognitionStartDate      = new DateTime(2026, 2, 1),
                    RecognitionEndDate        = new DateTime(2026, 6, 30),
                    MilestoneTriggerRequired  = true,
                    IsScheduleGenerated       = true,
                    TotalRecognizableAmount   = 320000m,
                    RecognizedAmountToDate    = 128000m,
                    RecognizedPeriodsCount    = 2,
                    PendingPeriodsCount       = 2,
                    FiscalYearId              = MasterDataIds.FiscalYears.FY2025_26,
                    LastRecognitionRunDate    = new DateTime(2026, 3, 15),
                    NextRecognitionDueDate    = new DateTime(2026, 4, 30),
                    PreparedByUserId          = "finance.admin",
                    PreparedOn                = new DateTime(2026, 2, 1),
                    Notes                     = "2 of 4 delivery batches confirmed and recognized.",
                    CreatedAt                 = new DateTime(2026, 2, 1),
                    ScheduleLines = new List<RevenueRecognitionLine>
                    {
                        new RevenueRecognitionLine { RevenueRecognitionLineId = Guid.Parse("82110001-0001-0001-0001-000000000001"), RevenueRecognitionId = RR11, LineNumber = 10, ScheduleDate = new DateTime(2026, 2, 15), ScheduledAmount = 80000m, RecognizedAmount = 80000m, RecognitionLineStatus = RecognitionLineStatusEnum.Recognized, MilestoneReference = "Batch 1 – Living Room Set", TriggerEventReference = "Delivery note DN-2026-101", ManualApprovalStatus = ManualApprovalStatusEnum.Approved, RecognizedOn = new DateTime(2026, 2, 15), RecognizedBy = "finance.admin" },
                        new RevenueRecognitionLine { RevenueRecognitionLineId = Guid.Parse("82110002-0002-0002-0002-000000000002"), RevenueRecognitionId = RR11, LineNumber = 20, ScheduleDate = new DateTime(2026, 3, 15), ScheduledAmount = 48000m, RecognizedAmount = 48000m, RecognitionLineStatus = RecognitionLineStatusEnum.Recognized, MilestoneReference = "Batch 2 – Office Chairs", TriggerEventReference = "Delivery note DN-2026-115", ManualApprovalStatus = ManualApprovalStatusEnum.Approved, RecognizedOn = new DateTime(2026, 3, 15), RecognizedBy = "finance.admin" },
                        new RevenueRecognitionLine { RevenueRecognitionLineId = Guid.Parse("82110003-0003-0003-0003-000000000003"), RevenueRecognitionId = RR11, LineNumber = 30, ScheduleDate = new DateTime(2026, 4, 30), ScheduledAmount = 96000m, RecognizedAmount = 0, RecognitionLineStatus = RecognitionLineStatusEnum.Planned, MilestoneReference = "Batch 3 – Conference Tables", ManualApprovalStatus = ManualApprovalStatusEnum.Pending },
                        new RevenueRecognitionLine { RevenueRecognitionLineId = Guid.Parse("82110004-0004-0004-0004-000000000004"), RevenueRecognitionId = RR11, LineNumber = 40, ScheduleDate = new DateTime(2026, 6, 30), ScheduledAmount = 96000m, RecognizedAmount = 0, RecognitionLineStatus = RecognitionLineStatusEnum.Planned, MilestoneReference = "Batch 4 – Custom Loungers", ManualApprovalStatus = ManualApprovalStatusEnum.Pending }
                    }
                },

                // 12 – Custom Rule Recognition with Adjustment
                new RevenueRecognition
                {
                    RevenueRecognitionId      = RR12,
                    TenantId                  = MasterDataIds.Tenants.Default,
                    CompanyId                 = MasterDataIds.Companies.SofaCraft,
                    RecognitionCode           = "REVREC-2026-012",
                    RecognitionName           = "Custom Rule – Advance Receipt Partial Recognition",
                    Description               = "Custom recognition rule applied for partial advance receipt.",
                    RecognitionStatus         = RecognitionStatusEnum.InProgress,
                    RevenueId                 = RevenueSeedData.Rev12,
                    RevenueCodeSnapshot       = "REV-2026-0012",
                    RevenueNameSnapshot       = "Advance Receipt – Interior Design Consultation",
                    CustomerId                = CustomerSeedData.Customer4Id,
                    CustomerNameSnapshot      = "National Bank of India",
                    SourceDocumentTypeSnapshot = "ManualRevenueEvent",
                    RevenueTypeSnapshot       = "AdvanceReceiptBased",
                    RevenueNatureSnapshot     = "Mixed",
                    SourceGrossRevenueAmount  = 200000m,
                    CurrencyId                = MasterDataIds.Currencies.INR,
                    RecognitionMethod         = RecognitionMethodEnum.DeferredThenRelease,
                    RecognitionBasis          = RecognitionBasisEnum.CustomRule,
                    RecognitionStartDate      = new DateTime(2026, 2, 1),
                    RecognitionEndDate        = new DateTime(2026, 8, 31),
                    DeferredRevenueId         = Guid.Parse("88000002-0002-0002-0002-000000000002"),
                    DeferredRevenueReference  = "DEFREV-2026-002",
                    IsScheduleGenerated       = true,
                    TotalRecognizableAmount   = 200000m,
                    RecognizedAmountToDate    = 60000m,
                    CurrentPeriodRecognitionAmount = 30000m,
                    AdjustmentAmount          = -5000m,
                    RoundingDifferenceAmount  = 200m,
                    FiscalYearId              = MasterDataIds.FiscalYears.FY2025_26,
                    RecognitionPostingDate    = new DateTime(2026, 3, 31),
                    LastRecognitionRunDate    = new DateTime(2026, 3, 31),
                    NextRecognitionDueDate    = new DateTime(2026, 4, 30),
                    RecognizedPeriodsCount    = 2,
                    PendingPeriodsCount       = 5,
                    PreparedByUserId          = "finance.admin",
                    PreparedOn                = new DateTime(2026, 2, 1),
                    RecognitionAssumptionText = "Custom allocation: 30% upfront consultation, remaining spread over project duration.",
                    Notes                     = "Adjustment of ₹5,000 for scope change. Rounding difference ₹200.",
                    CreatedAt                 = new DateTime(2026, 2, 1),
                    ScheduleLines = new List<RevenueRecognitionLine>
                    {
                        new RevenueRecognitionLine { RevenueRecognitionLineId = Guid.Parse("82120001-0001-0001-0001-000000000001"), RevenueRecognitionId = RR12, LineNumber = 10, ScheduleDate = new DateTime(2026, 2, 28), ScheduledAmount = 30000m, RecognizedAmount = 30000m, RecognitionLineStatus = RecognitionLineStatusEnum.Recognized, ManualApprovalStatus = ManualApprovalStatusEnum.NotRequired, RecognizedOn = new DateTime(2026, 2, 28), RecognizedBy = "finance.admin", RecognitionLineNotes = "Initial consultation phase" },
                        new RevenueRecognitionLine { RevenueRecognitionLineId = Guid.Parse("82120002-0002-0002-0002-000000000002"), RevenueRecognitionId = RR12, LineNumber = 20, ScheduleDate = new DateTime(2026, 3, 31), ScheduledAmount = 30000m, RecognizedAmount = 30000m, RecognitionLineStatus = RecognitionLineStatusEnum.Recognized, ManualApprovalStatus = ManualApprovalStatusEnum.NotRequired, RecognizedOn = new DateTime(2026, 3, 31), RecognizedBy = "finance.admin", RecognitionLineNotes = "Design phase delivery" },
                        new RevenueRecognitionLine { RevenueRecognitionLineId = Guid.Parse("82120003-0003-0003-0003-000000000003"), RevenueRecognitionId = RR12, LineNumber = 30, ScheduleDate = new DateTime(2026, 4, 30), ScheduledAmount = 28000m, RecognizedAmount = 0, RecognitionLineStatus = RecognitionLineStatusEnum.Planned, ManualApprovalStatus = ManualApprovalStatusEnum.NotRequired },
                        new RevenueRecognitionLine { RevenueRecognitionLineId = Guid.Parse("82120004-0004-0004-0004-000000000004"), RevenueRecognitionId = RR12, LineNumber = 40, ScheduleDate = new DateTime(2026, 5, 31), ScheduledAmount = 28000m, RecognizedAmount = 0, RecognitionLineStatus = RecognitionLineStatusEnum.Planned, ManualApprovalStatus = ManualApprovalStatusEnum.NotRequired },
                        new RevenueRecognitionLine { RevenueRecognitionLineId = Guid.Parse("82120005-0005-0005-0005-000000000005"), RevenueRecognitionId = RR12, LineNumber = 50, ScheduleDate = new DateTime(2026, 6, 30), ScheduledAmount = 28000m, RecognizedAmount = 0, RecognitionLineStatus = RecognitionLineStatusEnum.Planned, ManualApprovalStatus = ManualApprovalStatusEnum.NotRequired },
                        new RevenueRecognitionLine { RevenueRecognitionLineId = Guid.Parse("82120006-0006-0006-0006-000000000006"), RevenueRecognitionId = RR12, LineNumber = 60, ScheduleDate = new DateTime(2026, 7, 31), ScheduledAmount = 28000m, RecognizedAmount = 0, RecognitionLineStatus = RecognitionLineStatusEnum.Planned, ManualApprovalStatus = ManualApprovalStatusEnum.NotRequired },
                        new RevenueRecognitionLine { RevenueRecognitionLineId = Guid.Parse("82120007-0007-0007-0007-000000000007"), RevenueRecognitionId = RR12, LineNumber = 70, ScheduleDate = new DateTime(2026, 8, 31), ScheduledAmount = 28000m, RecognizedAmount = 0, RecognitionLineStatus = RecognitionLineStatusEnum.Planned, ManualApprovalStatus = ManualApprovalStatusEnum.NotRequired }
                    }
                }
            };
        }

        // ── Helper: Generate monthly schedule lines ────────────────────────────
        private static List<RevenueRecognitionLine> GenerateMonthlyLines(
            Guid parentId, DateTime startDate, int months, decimal amountPerMonth, int recognizedMonths)
        {
            var lines = new List<RevenueRecognitionLine>();
            for (int i = 0; i < months; i++)
            {
                var lineDate = startDate.AddMonths(i);
                var lastDay = new DateTime(lineDate.Year, lineDate.Month, DateTime.DaysInMonth(lineDate.Year, lineDate.Month));
                bool isRecognized = i < recognizedMonths;
                lines.Add(new RevenueRecognitionLine
                {
                    RevenueRecognitionLineId = Guid.Parse($"820{parentId.ToString().Substring(5, 1)}0{(i + 1):D3}-{(i + 1):D4}-{(i + 1):D4}-{(i + 1):D4}-0000000000{(i + 1):D2}"),
                    RevenueRecognitionId = parentId,
                    LineNumber = (i + 1) * 10,
                    ScheduleDate = lastDay,
                    ScheduledAmount = amountPerMonth,
                    RecognizedAmount = isRecognized ? amountPerMonth : 0m,
                    RecognitionLineStatus = isRecognized ? RecognitionLineStatusEnum.Recognized : RecognitionLineStatusEnum.Planned,
                    ManualApprovalStatus = ManualApprovalStatusEnum.NotRequired,
                    RecognizedOn = isRecognized ? lastDay : null,
                    RecognizedBy = isRecognized ? "finance.admin" : null
                });
            }
            return lines;
        }
    }
}
