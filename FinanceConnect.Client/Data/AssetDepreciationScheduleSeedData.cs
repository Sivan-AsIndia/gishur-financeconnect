using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    public class AssetDepreciationScheduleSeedData
    {
        private static void GenerateLines(AssetDepreciationScheduleViewModel.AssetDepreciationSchedule sch,
            decimal depBase, decimal totalCost, decimal residual, int months, DateTime start, int postedCount)
        {
            decimal perMonth = Math.Round(depBase / months, 2);
            decimal accumDep = 0;
            for (int i = 0; i < months; i++)
            {
                var periodStart = start.AddMonths(i);
                var periodEnd = periodStart.AddMonths(1).AddDays(-1);
                decimal depAmt = (i == months - 1) ? (depBase - accumDep) : perMonth;
                accumDep += depAmt;
                decimal nbv = totalCost - accumDep;
                sch.ScheduleLines.Add(new AssetDepreciationScheduleViewModel.AssetDepreciationScheduleLine
                {
                    AssetDepreciationScheduleLineId = Guid.NewGuid(),
                    AssetDepreciationScheduleId = sch.AssetDepreciationScheduleId,
                    LineNumber = (i + 1) * 10,
                    AccountingPeriodId = Guid.NewGuid(),
                    PeriodLabel = periodStart.ToString("MMM yyyy"),
                    PeriodStartDate = periodStart,
                    PeriodEndDate = periodEnd,
                    PlannedDepreciationAmount = depAmt,
                    PlannedAccumulatedDepreciationAmount = accumDep,
                    PlannedNetBookValueAmount = nbv < residual ? residual : nbv,
                    IsPosted = i < postedCount,
                    PostedOn = i < postedCount ? periodEnd.AddDays(5) : null,
                    LockStatus = i < postedCount
                        ? AssetDepreciationScheduleViewModel.LineLockStatusEnum.LockedPosted
                        : AssetDepreciationScheduleViewModel.LineLockStatusEnum.Open
                });
            }
        }

        public static List<AssetDepreciationScheduleViewModel.AssetDepreciationSchedule> GetAll()
        {
            var schedules = new List<AssetDepreciationScheduleViewModel.AssetDepreciationSchedule>();
            var tenantId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            var currencyId = Guid.Parse("d0000000-0000-0000-0000-000000000001");

            // ── 1: Dell Latitude Laptop – SLM 36 months ───────
            var sch1 = new AssetDepreciationScheduleViewModel.AssetDepreciationSchedule
            {
                AssetDepreciationScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000001"),
                TenantId = tenantId, CompanyId = tenantId,
                FixedAssetId = Guid.Parse("50000000-0000-0000-0000-000000000001"),
                ScheduleNumber = "FASCH-000001",
                ScheduleStatus = AssetDepreciationScheduleViewModel.ScheduleStatusEnum.Active,
                ScheduleVersion = 1, GeneratedOn = new DateTime(2023, 7, 1), GeneratedBy = "System",
                DepreciationMethodIdSnapshot = Guid.Parse("20000000-0000-0000-0000-000000000001"),
                MethodTypeSnapshot = "StraightLine", InputModeSnapshot = "LifeBased",
                UsefulLifeMonthsSnapshot = 36, ResidualValuePercentSnapshot = 5.000m, ResidualValueAmountSnapshot = 3250m,
                DepreciationStartConventionSnapshot = "FromInServiceDate_ProRata",
                StartDate = new DateTime(2023, 7, 1), EndDate = new DateTime(2026, 6, 30),
                DepreciationBaseAmountSnapshot = 61750m, TotalCapitalizedCostSnapshot = 65000m,
                CurrencyId = currencyId,
                AssetCodeDisplay = "FA-LAP-001", AssetNameDisplay = "Dell Latitude Laptop",
                MethodNameDisplay = "Straight Line – Monthly Pro-Rata",
                CreatedAt = new DateTime(2023, 7, 1)
            };
            GenerateLines(sch1, 61750m, 65000m, 3250m, 36, new DateTime(2023, 7, 1), 18);
            schedules.Add(sch1);

            // ── 2: HP Desktop – SLM-NMS 48 months ─────────────
            var sch2 = new AssetDepreciationScheduleViewModel.AssetDepreciationSchedule
            {
                AssetDepreciationScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000002"),
                TenantId = tenantId, CompanyId = tenantId,
                FixedAssetId = Guid.Parse("50000000-0000-0000-0000-000000000002"),
                ScheduleNumber = "FASCH-000002",
                ScheduleStatus = AssetDepreciationScheduleViewModel.ScheduleStatusEnum.Active,
                ScheduleVersion = 1, GeneratedOn = new DateTime(2022, 4, 1), GeneratedBy = "System",
                DepreciationMethodIdSnapshot = Guid.Parse("20000000-0000-0000-0000-000000000002"),
                MethodTypeSnapshot = "StraightLine", InputModeSnapshot = "LifeBased",
                UsefulLifeMonthsSnapshot = 48, ResidualValuePercentSnapshot = 10.000m, ResidualValueAmountSnapshot = 4800m,
                DepreciationStartConventionSnapshot = "NextMonthStart",
                StartDate = new DateTime(2022, 4, 1), EndDate = new DateTime(2026, 3, 31),
                DepreciationBaseAmountSnapshot = 43200m, TotalCapitalizedCostSnapshot = 48000m,
                CurrencyId = currencyId,
                AssetCodeDisplay = "FA-DESK-001", AssetNameDisplay = "HP Desktop System",
                MethodNameDisplay = "Straight Line – Next Month Start",
                CreatedAt = new DateTime(2022, 4, 1)
            };
            GenerateLines(sch2, 43200m, 48000m, 4800m, 48, new DateTime(2022, 4, 1), 30);
            schedules.Add(sch2);

            // ── 3: Office Table – Superseded ────────────
            var sch3 = new AssetDepreciationScheduleViewModel.AssetDepreciationSchedule
            {
                AssetDepreciationScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000003"),
                TenantId = tenantId, CompanyId = tenantId,
                FixedAssetId = Guid.Parse("50000000-0000-0000-0000-000000000003"),
                ScheduleNumber = "FASCH-000003",
                ScheduleStatus = AssetDepreciationScheduleViewModel.ScheduleStatusEnum.Superseded,
                ScheduleVersion = 1, GeneratedOn = new DateTime(2023, 5, 1), GeneratedBy = "System",
                DepreciationMethodIdSnapshot = Guid.Parse("20000000-0000-0000-0000-000000000001"),
                MethodTypeSnapshot = "StraightLine", InputModeSnapshot = "LifeBased",
                UsefulLifeMonthsSnapshot = 60, ResidualValuePercentSnapshot = 5.000m, ResidualValueAmountSnapshot = 1250m,
                DepreciationStartConventionSnapshot = "FromInServiceDate_ProRata",
                StartDate = new DateTime(2023, 5, 1), EndDate = new DateTime(2028, 4, 30),
                DepreciationBaseAmountSnapshot = 23750m, TotalCapitalizedCostSnapshot = 25000m,
                CurrencyId = currencyId,
                SupersededByScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000004"),
                AssetCodeDisplay = "FA-FURN-001", AssetNameDisplay = "Office Workstation Table",
                MethodNameDisplay = "Straight Line – Monthly Pro-Rata",
                CreatedAt = new DateTime(2023, 5, 1)
            };
            schedules.Add(sch3);

            // ── 4: Office Table – v2 Active ─────────────
            var sch4 = new AssetDepreciationScheduleViewModel.AssetDepreciationSchedule
            {
                AssetDepreciationScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000004"),
                TenantId = tenantId, CompanyId = tenantId,
                FixedAssetId = Guid.Parse("50000000-0000-0000-0000-000000000003"),
                ScheduleNumber = "FASCH-000004",
                ScheduleStatus = AssetDepreciationScheduleViewModel.ScheduleStatusEnum.Active,
                ScheduleVersion = 2, GeneratedOn = new DateTime(2024, 1, 1), GeneratedBy = "System",
                ScheduleGenerationReason = "Regenerated after revaluation event FAREV-000003",
                DepreciationMethodIdSnapshot = Guid.Parse("20000000-0000-0000-0000-000000000001"),
                MethodTypeSnapshot = "StraightLine", InputModeSnapshot = "LifeBased",
                UsefulLifeMonthsSnapshot = 48, ResidualValuePercentSnapshot = 5.000m, ResidualValueAmountSnapshot = 1100m,
                DepreciationStartConventionSnapshot = "FromInServiceDate_ProRata",
                StartDate = new DateTime(2024, 1, 1), EndDate = new DateTime(2028, 4, 30),
                DepreciationBaseAmountSnapshot = 20900m, TotalCapitalizedCostSnapshot = 22000m,
                CurrencyId = currencyId,
                AssetCodeDisplay = "FA-FURN-001", AssetNameDisplay = "Office Workstation Table",
                MethodNameDisplay = "Straight Line – Monthly Pro-Rata",
                CreatedAt = new DateTime(2024, 1, 1)
            };
            GenerateLines(sch4, 20900m, 22000m, 1100m, 48, new DateTime(2024, 1, 1), 12);
            schedules.Add(sch4);

            // ── 5: Toyota Fleet Vehicle – WDV ──
            var sch5 = new AssetDepreciationScheduleViewModel.AssetDepreciationSchedule
            {
                AssetDepreciationScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000005"),
                TenantId = tenantId, CompanyId = tenantId,
                FixedAssetId = Guid.Parse("50000000-0000-0000-0000-000000000004"),
                ScheduleNumber = "FASCH-000005",
                ScheduleStatus = AssetDepreciationScheduleViewModel.ScheduleStatusEnum.Active,
                ScheduleVersion = 1, GeneratedOn = new DateTime(2024, 2, 1), GeneratedBy = "System",
                DepreciationMethodIdSnapshot = Guid.Parse("20000000-0000-0000-0000-000000000006"),
                MethodTypeSnapshot = "WrittenDownValue", InputModeSnapshot = "RateBased",
                RatePercentSnapshot = 25.000m,
                UsefulLifeMonthsSnapshot = 60, ResidualValuePercentSnapshot = 10.000m, ResidualValueAmountSnapshot = 185000m,
                DepreciationStartConventionSnapshot = "FromInServiceDate_ProRata",
                StartDate = new DateTime(2024, 2, 1), EndDate = new DateTime(2029, 1, 31),
                DepreciationBaseAmountSnapshot = 1665000m, TotalCapitalizedCostSnapshot = 1850000m,
                CurrencyId = currencyId,
                AssetCodeDisplay = "FA-VEH-001", AssetNameDisplay = "Toyota Innova Fleet Vehicle",
                MethodNameDisplay = "Written Down Value – 25% Rate",
                CreatedAt = new DateTime(2024, 2, 1)
            };
            GenerateLines(sch5, 1665000m, 1850000m, 185000m, 60, new DateTime(2024, 2, 1), 10);
            schedules.Add(sch5);

            // ── 6: Dell Server – SLM 60 months ──
            var sch6 = new AssetDepreciationScheduleViewModel.AssetDepreciationSchedule
            {
                AssetDepreciationScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000006"),
                TenantId = tenantId, CompanyId = tenantId,
                FixedAssetId = Guid.Parse("50000000-0000-0000-0000-000000000005"),
                ScheduleNumber = "FASCH-000006",
                ScheduleStatus = AssetDepreciationScheduleViewModel.ScheduleStatusEnum.Active,
                ScheduleVersion = 1, GeneratedOn = new DateTime(2024, 6, 1), GeneratedBy = "System",
                DepreciationMethodIdSnapshot = Guid.Parse("20000000-0000-0000-0000-000000000001"),
                MethodTypeSnapshot = "StraightLine", InputModeSnapshot = "LifeBased",
                UsefulLifeMonthsSnapshot = 60, ResidualValuePercentSnapshot = 5.000m, ResidualValueAmountSnapshot = 23500m,
                DepreciationStartConventionSnapshot = "FromInServiceDate_ProRata",
                StartDate = new DateTime(2024, 6, 1), EndDate = new DateTime(2029, 5, 31),
                DepreciationBaseAmountSnapshot = 446500m, TotalCapitalizedCostSnapshot = 470000m,
                CurrencyId = currencyId,
                AssetCodeDisplay = "FA-SERV-001", AssetNameDisplay = "Dell PowerEdge R750 Server",
                MethodNameDisplay = "Straight Line – Monthly Pro-Rata",
                CreatedAt = new DateTime(2024, 6, 1)
            };
            GenerateLines(sch6, 446500m, 470000m, 23500m, 60, new DateTime(2024, 6, 1), 8);
            schedules.Add(sch6);

            // ── 7: MacBook Pro – SLM 36 months ──
            var sch7 = new AssetDepreciationScheduleViewModel.AssetDepreciationSchedule
            {
                AssetDepreciationScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000007"),
                TenantId = tenantId, CompanyId = tenantId,
                FixedAssetId = Guid.Parse("50000000-0000-0000-0000-000000000007"),
                ScheduleNumber = "FASCH-000007",
                ScheduleStatus = AssetDepreciationScheduleViewModel.ScheduleStatusEnum.Active,
                ScheduleVersion = 1, GeneratedOn = new DateTime(2024, 10, 1), GeneratedBy = "System",
                DepreciationMethodIdSnapshot = Guid.Parse("20000000-0000-0000-0000-000000000001"),
                MethodTypeSnapshot = "StraightLine", InputModeSnapshot = "LifeBased",
                UsefulLifeMonthsSnapshot = 36, ResidualValuePercentSnapshot = 5.000m, ResidualValueAmountSnapshot = 9250m,
                DepreciationStartConventionSnapshot = "FromInServiceDate_ProRata",
                StartDate = new DateTime(2024, 10, 1), EndDate = new DateTime(2027, 9, 30),
                DepreciationBaseAmountSnapshot = 175750m, TotalCapitalizedCostSnapshot = 185000m,
                CurrencyId = currencyId,
                AssetCodeDisplay = "FA-LAP-002", AssetNameDisplay = "MacBook Pro 14-inch",
                MethodNameDisplay = "Straight Line – Monthly Pro-Rata",
                CreatedAt = new DateTime(2024, 10, 1)
            };
            GenerateLines(sch7, 175750m, 185000m, 9250m, 36, new DateTime(2024, 10, 1), 5);
            schedules.Add(sch7);

            // ── 8: Ergonomic Chair Set – SLM 60 months ──
            var sch8 = new AssetDepreciationScheduleViewModel.AssetDepreciationSchedule
            {
                AssetDepreciationScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000008"),
                TenantId = tenantId, CompanyId = tenantId,
                FixedAssetId = Guid.Parse("50000000-0000-0000-0000-000000000010"),
                ScheduleNumber = "FASCH-000008",
                ScheduleStatus = AssetDepreciationScheduleViewModel.ScheduleStatusEnum.Active,
                ScheduleVersion = 1, GeneratedOn = new DateTime(2024, 3, 1), GeneratedBy = "System",
                DepreciationMethodIdSnapshot = Guid.Parse("20000000-0000-0000-0000-000000000001"),
                MethodTypeSnapshot = "StraightLine", InputModeSnapshot = "LifeBased",
                UsefulLifeMonthsSnapshot = 60, ResidualValuePercentSnapshot = 5.000m, ResidualValueAmountSnapshot = 7750m,
                DepreciationStartConventionSnapshot = "FromInServiceDate_ProRata",
                StartDate = new DateTime(2024, 3, 1), EndDate = new DateTime(2029, 2, 28),
                DepreciationBaseAmountSnapshot = 147250m, TotalCapitalizedCostSnapshot = 155000m,
                CurrencyId = currencyId,
                AssetCodeDisplay = "FA-FURN-002", AssetNameDisplay = "Ergonomic Office Chair Set",
                MethodNameDisplay = "Straight Line – Monthly Pro-Rata",
                CreatedAt = new DateTime(2024, 3, 1)
            };
            GenerateLines(sch8, 147250m, 155000m, 7750m, 60, new DateTime(2024, 3, 1), 12);
            schedules.Add(sch8);

            // ── 9: Daikin AC – Draft ──
            var sch9 = new AssetDepreciationScheduleViewModel.AssetDepreciationSchedule
            {
                AssetDepreciationScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000009"),
                TenantId = tenantId, CompanyId = tenantId,
                FixedAssetId = Guid.Parse("50000000-0000-0000-0000-000000000008"),
                ScheduleNumber = "FASCH-000009",
                ScheduleStatus = AssetDepreciationScheduleViewModel.ScheduleStatusEnum.Draft,
                ScheduleVersion = 1, GeneratedOn = new DateTime(2025, 2, 1), GeneratedBy = "System",
                DepreciationMethodIdSnapshot = Guid.Parse("20000000-0000-0000-0000-000000000001"),
                MethodTypeSnapshot = "StraightLine", InputModeSnapshot = "LifeBased",
                UsefulLifeMonthsSnapshot = 60, ResidualValuePercentSnapshot = 5.000m, ResidualValueAmountSnapshot = 3525m,
                DepreciationStartConventionSnapshot = "NextMonthStart",
                StartDate = new DateTime(2025, 3, 1), EndDate = new DateTime(2030, 2, 28),
                DepreciationBaseAmountSnapshot = 66975m, TotalCapitalizedCostSnapshot = 70500m,
                CurrencyId = currencyId,
                AssetCodeDisplay = "FA-HVAC-001", AssetNameDisplay = "Daikin Split AC 2 Ton",
                MethodNameDisplay = "Straight Line – Next Month Start",
                CreatedAt = new DateTime(2025, 2, 1)
            };
            schedules.Add(sch9);

            // ── 10: Projector – Cancelled ──
            var sch10 = new AssetDepreciationScheduleViewModel.AssetDepreciationSchedule
            {
                AssetDepreciationScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000010"),
                TenantId = tenantId, CompanyId = tenantId,
                FixedAssetId = Guid.Parse("50000000-0000-0000-0000-000000000006"),
                ScheduleNumber = "FASCH-000010",
                ScheduleStatus = AssetDepreciationScheduleViewModel.ScheduleStatusEnum.Cancelled,
                ScheduleVersion = 1, GeneratedOn = new DateTime(2024, 8, 1), GeneratedBy = "System",
                DepreciationMethodIdSnapshot = Guid.Parse("20000000-0000-0000-0000-000000000002"),
                MethodTypeSnapshot = "StraightLine", InputModeSnapshot = "LifeBased",
                UsefulLifeMonthsSnapshot = 36, ResidualValuePercentSnapshot = 5.000m, ResidualValueAmountSnapshot = 3600m,
                DepreciationStartConventionSnapshot = "NextMonthStart",
                StartDate = new DateTime(2024, 9, 1), EndDate = new DateTime(2027, 8, 31),
                DepreciationBaseAmountSnapshot = 68400m, TotalCapitalizedCostSnapshot = 72000m,
                CurrencyId = currencyId,
                AssetCodeDisplay = "FA-PROJ-001", AssetNameDisplay = "Epson EB-X51 Projector",
                MethodNameDisplay = "Straight Line – Next Month Start",
                CreatedAt = new DateTime(2024, 8, 1)
            };
            schedules.Add(sch10);

            // ── 11: Dell Laptop v2 – Locked ──
            var sch11 = new AssetDepreciationScheduleViewModel.AssetDepreciationSchedule
            {
                AssetDepreciationScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000011"),
                TenantId = tenantId, CompanyId = tenantId,
                FixedAssetId = Guid.Parse("50000000-0000-0000-0000-000000000001"),
                ScheduleNumber = "FASCH-000011",
                ScheduleStatus = AssetDepreciationScheduleViewModel.ScheduleStatusEnum.Locked,
                ScheduleVersion = 2, GeneratedOn = new DateTime(2025, 1, 1), GeneratedBy = "Auditor",
                ScheduleGenerationReason = "Locked after FY2024 audit close",
                DepreciationMethodIdSnapshot = Guid.Parse("20000000-0000-0000-0000-000000000001"),
                MethodTypeSnapshot = "StraightLine", InputModeSnapshot = "LifeBased",
                UsefulLifeMonthsSnapshot = 36, ResidualValuePercentSnapshot = 5.000m, ResidualValueAmountSnapshot = 3250m,
                DepreciationStartConventionSnapshot = "FromInServiceDate_ProRata",
                StartDate = new DateTime(2023, 7, 1), EndDate = new DateTime(2026, 6, 30),
                DepreciationBaseAmountSnapshot = 61750m, TotalCapitalizedCostSnapshot = 65000m,
                CurrencyId = currencyId,
                LockedOn = new DateTime(2025, 1, 15), LockedBy = "Admin", LockReason = "FY2024 audit lock",
                AssetCodeDisplay = "FA-LAP-001", AssetNameDisplay = "Dell Latitude Laptop",
                MethodNameDisplay = "Straight Line – Monthly Pro-Rata",
                CreatedAt = new DateTime(2025, 1, 1)
            };
            GenerateLines(sch11, 61750m, 65000m, 3250m, 36, new DateTime(2023, 7, 1), 36);
            schedules.Add(sch11);

            // ── 12: Printer – Active short schedule ──
            var sch12 = new AssetDepreciationScheduleViewModel.AssetDepreciationSchedule
            {
                AssetDepreciationScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000012"),
                TenantId = tenantId, CompanyId = tenantId,
                FixedAssetId = Guid.Parse("50000000-0000-0000-0000-000000000009"),
                ScheduleNumber = "FASCH-000012",
                ScheduleStatus = AssetDepreciationScheduleViewModel.ScheduleStatusEnum.Active,
                ScheduleVersion = 1, GeneratedOn = new DateTime(2024, 7, 1), GeneratedBy = "System",
                DepreciationMethodIdSnapshot = Guid.Parse("20000000-0000-0000-0000-000000000002"),
                MethodTypeSnapshot = "StraightLine", InputModeSnapshot = "LifeBased",
                UsefulLifeMonthsSnapshot = 24, ResidualValuePercentSnapshot = 5.000m, ResidualValueAmountSnapshot = 1900m,
                DepreciationStartConventionSnapshot = "NextMonthStart",
                StartDate = new DateTime(2024, 8, 1), EndDate = new DateTime(2026, 7, 31),
                DepreciationBaseAmountSnapshot = 36100m, TotalCapitalizedCostSnapshot = 38000m,
                CurrencyId = currencyId,
                AssetCodeDisplay = "FA-PRNT-001", AssetNameDisplay = "HP LaserJet Pro MFP",
                MethodNameDisplay = "Straight Line – Next Month Start",
                CreatedAt = new DateTime(2024, 7, 1)
            };
            GenerateLines(sch12, 36100m, 38000m, 1900m, 24, new DateTime(2024, 8, 1), 6);
            schedules.Add(sch12);

            return schedules;
        }
    }
}
