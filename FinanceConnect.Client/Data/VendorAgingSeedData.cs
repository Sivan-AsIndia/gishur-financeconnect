using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    /// <summary>
    /// Seed data for VendorAging model (Model #42)
    /// Provides aging snapshots with vendor summaries and bill details
    /// Supports 8 aging buckets: Current, 0-30, 31-60, 61-90, 91-120, 121-180, 181-365, 365+
    /// </summary>
    public static class VendorAgingSeedData
    {
        // Company GUIDs (matching existing company seed data)
        private static readonly Guid AscendingSoftwareCompanyId = MasterDataIds.Companies.SofaCraft;
        private static readonly Guid GlobalTechCompanyId = MasterDataIds.Companies.SofaCraftUSA;

        // Branch GUIDs (matching existing branch seed data)
        private static readonly Guid ChennaiHQBranchId = MasterDataIds.Branches.SofaCraftHQ;
        private static readonly Guid BangaloreBranchId = MasterDataIds.Branches.SofaCraftBengaluru;
        private static readonly Guid HyderabadBranchId = MasterDataIds.Branches.CozyCraftHyderabad;

        // Vendor GUIDs (matching VendorSeedData)
        private static readonly Guid Vendor1Id = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000001");
        private static readonly Guid Vendor2Id = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000002");
        private static readonly Guid Vendor3Id = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000003");
        private static readonly Guid Vendor4Id = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000004");
        private static readonly Guid Vendor5Id = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000005");

        // Currency GUIDs
        private static readonly Guid InrCurrencyId = MasterDataIds.Currencies.INR;
        private static readonly Guid UsdCurrencyId = MasterDataIds.Currencies.USD;

        // Predefined Aging Snapshot GUIDs
        public static readonly Guid VendorAging1Id = Guid.Parse("f4f4f4f4-0001-0001-0001-000000000001");
        public static readonly Guid VendorAging2Id = Guid.Parse("f4f4f4f4-0001-0001-0001-000000000002");
        public static readonly Guid VendorAging3Id = Guid.Parse("f4f4f4f4-0001-0001-0001-000000000003");
        public static readonly Guid VendorAging4Id = Guid.Parse("f4f4f4f4-0001-0001-0001-000000000004");
        public static readonly Guid VendorAging5Id = Guid.Parse("f4f4f4f4-0001-0001-0001-000000000005");
        public static readonly Guid VendorAging6Id = Guid.Parse("f4f4f4f4-0001-0001-0001-000000000006");
        public static readonly Guid VendorAging7Id = Guid.Parse("f4f4f4f4-0001-0001-0001-000000000007");
        public static readonly Guid VendorAging8Id = Guid.Parse("f4f4f4f4-0001-0001-0001-000000000008");
        public static readonly Guid VendorAging9Id = Guid.Parse("f4f4f4f4-0001-0001-0001-000000000009");
        public static readonly Guid VendorAging10Id = Guid.Parse("f4f4f4f4-0001-0001-0001-000000000010");
        public static readonly Guid VendorAging11Id = Guid.Parse("f4f4f4f4-0001-0001-0001-000000000011");
        public static readonly Guid VendorAging12Id = Guid.Parse("f4f4f4f4-0001-0001-0001-000000000012");

        public static List<VendorAgingViewModel> GetSeedAgingSnapshots()
        {
            var snapshots = new List<VendorAgingViewModel>
            {
                // Snapshot 1: Today's snapshot - All Branches - Completed (Nightly)
                CreateTodaySnapshot(),
                
                // Snapshot 2: Month-end snapshot - Last month end - Finalized
                CreateMonthEndSnapshot(),
                
                // Snapshot 3: Branch-specific snapshot - Chennai - Completed (OnDemand)
                CreateBranchSpecificSnapshot(),
                
                // Snapshot 4: Failed snapshot - for testing retry
                CreateFailedSnapshot(),

                // Snapshot 5: On-Demand snapshot with credits excluded
                CreateOnDemandNoCreditsSnapshot(),
                CreateWeeklySnapshot6(),
                CreateWeeklySnapshot7(),
                CreateWeeklySnapshot8(),
                CreateWeeklySnapshot9(),
                CreateWeeklySnapshot10(),
                CreateWeeklySnapshot11(),
                CreateWeeklySnapshot12()
            };

            return snapshots;
        }

        private static VendorAgingViewModel CreateTodaySnapshot()
        {
            var agingId = VendorAging1Id;
            var asOfDate = DateTime.Today;

            var snapshot = new VendorAgingViewModel
            {
                VendorAgingId = agingId,
                TenantId = MasterDataIds.Tenants.Default,
                CompanyId = AscendingSoftwareCompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                BranchId = null, // All branches
                CurrencyId = InrCurrencyId,
                CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                AsOfDate = asOfDate,
                AgingRunId = Guid.NewGuid(),
                RunType = VendorAgingRunTypes.Nightly,
                RunStatus = VendorAgingRunStatuses.Completed,
                GeneratedAt = DateTime.Now.AddHours(-2),
                GeneratedByUserId = MasterDataIds.Tenants.Default,
                GeneratedByUserName = "System",
                IncludeOpenCredits = true,
                CreatedAt = DateTime.Now.AddHours(-2),
                CreatedBy = "System"
            };

            // Vendor 1: TechSupply Solutions - High overdue across multiple buckets
            var vendor1Row = new VendorAgingVendorRowViewModel
            {
                VendorAgingVendorRowId = Guid.NewGuid(),
                VendorAgingId = agingId,
                VendorId = Vendor1Id,
                VendorCodeSnapshot = "VND-000001",
                VendorNameSnapshot = "Tech Components India Pvt Ltd",
                CurrencyId = InrCurrencyId,
                CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                TotalOutstandingAmount = 485000.00m,
                CurrentNotDueAmount = 85000.00m,
                Bucket_0_30 = 120000.00m,
                Bucket_31_60 = 95000.00m,
                Bucket_61_90 = 75000.00m,
                Bucket_91_120 = 50000.00m,
                Bucket_121_180 = 35000.00m,
                Bucket_181_365 = 25000.00m,
                Bucket_366_Plus = 0.00m,
                OpenCreditAmount = 25000.00m,
                NetPayableAmount = 460000.00m,
                CreditBalanceExcessAmount = 0.00m,
                OpenDocumentCount = 8,
                OldestDueDate = asOfDate.AddDays(-175),
                LatestBillDate = asOfDate.AddDays(-5),
                LatestPaymentDate = asOfDate.AddDays(-15),
                ReconciliationStatus = VendorAgingReconciliationStatuses.Matched,
                ReconciliationDifferenceAmount = 0.00m,
                CreatedAt = DateTime.Now.AddHours(-2),
                CreatedBy = "System"
            };

            // Vendor 2: Global Hardware Corp - Medium overdue
            var vendor2Row = new VendorAgingVendorRowViewModel
            {
                VendorAgingVendorRowId = Guid.NewGuid(),
                VendorAgingId = agingId,
                VendorId = Vendor2Id,
                VendorCodeSnapshot = "VND-000002",
                VendorNameSnapshot = "CloudTech Solutions",
                CurrencyId = InrCurrencyId,
                CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                TotalOutstandingAmount = 275000.00m,
                CurrentNotDueAmount = 125000.00m,
                Bucket_0_30 = 75000.00m,
                Bucket_31_60 = 50000.00m,
                Bucket_61_90 = 25000.00m,
                Bucket_91_120 = 0.00m,
                Bucket_121_180 = 0.00m,
                Bucket_181_365 = 0.00m,
                Bucket_366_Plus = 0.00m,
                OpenCreditAmount = 15000.00m,
                NetPayableAmount = 260000.00m,
                CreditBalanceExcessAmount = 0.00m,
                OpenDocumentCount = 5,
                OldestDueDate = asOfDate.AddDays(-85),
                LatestBillDate = asOfDate.AddDays(-8),
                LatestPaymentDate = asOfDate.AddDays(-20),
                ReconciliationStatus = VendorAgingReconciliationStatuses.Matched,
                ReconciliationDifferenceAmount = 0.00m,
                CreatedAt = DateTime.Now.AddHours(-2),
                CreatedBy = "System"
            };

            // Vendor 3: Office Supplies India - Low overdue
            var vendor3Row = new VendorAgingVendorRowViewModel
            {
                VendorAgingVendorRowId = Guid.NewGuid(),
                VendorAgingId = agingId,
                VendorId = Vendor3Id,
                VendorCodeSnapshot = "VND-000003",
                VendorNameSnapshot = "Reliable Supplies Co",
                CurrencyId = InrCurrencyId,
                CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                TotalOutstandingAmount = 125000.00m,
                CurrentNotDueAmount = 100000.00m,
                Bucket_0_30 = 25000.00m,
                Bucket_31_60 = 0.00m,
                Bucket_61_90 = 0.00m,
                Bucket_91_120 = 0.00m,
                Bucket_121_180 = 0.00m,
                Bucket_181_365 = 0.00m,
                Bucket_366_Plus = 0.00m,
                OpenCreditAmount = 0.00m,
                NetPayableAmount = 125000.00m,
                CreditBalanceExcessAmount = 0.00m,
                OpenDocumentCount = 2,
                OldestDueDate = asOfDate.AddDays(-20),
                LatestBillDate = asOfDate.AddDays(-3),
                LatestPaymentDate = asOfDate.AddDays(-30),
                ReconciliationStatus = VendorAgingReconciliationStatuses.Matched,
                ReconciliationDifferenceAmount = 0.00m,
                CreatedAt = DateTime.Now.AddHours(-2),
                CreatedBy = "System"
            };

            // Vendor 4: Logistics Express - Current only with excess credit
            var vendor4Row = new VendorAgingVendorRowViewModel
            {
                VendorAgingVendorRowId = Guid.NewGuid(),
                VendorAgingId = agingId,
                VendorId = Vendor4Id,
                VendorCodeSnapshot = "VND-000004",
                VendorNameSnapshot = "BuildRight Constructions",
                CurrencyId = InrCurrencyId,
                CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                TotalOutstandingAmount = 45000.00m,
                CurrentNotDueAmount = 45000.00m,
                Bucket_0_30 = 0.00m,
                Bucket_31_60 = 0.00m,
                Bucket_61_90 = 0.00m,
                Bucket_91_120 = 0.00m,
                Bucket_121_180 = 0.00m,
                Bucket_181_365 = 0.00m,
                Bucket_366_Plus = 0.00m,
                OpenCreditAmount = 60000.00m,
                NetPayableAmount = 0.00m,
                CreditBalanceExcessAmount = 15000.00m,
                OpenDocumentCount = 1,
                OldestDueDate = asOfDate.AddDays(10),
                LatestBillDate = asOfDate.AddDays(-2),
                LatestPaymentDate = asOfDate.AddDays(-5),
                ReconciliationStatus = VendorAgingReconciliationStatuses.Matched,
                ReconciliationDifferenceAmount = 0.00m,
                CreatedAt = DateTime.Now.AddHours(-2),
                CreatedBy = "System"
            };

            // Vendor 5: IT Services Pro - Very old overdue (365+)
            var vendor5Row = new VendorAgingVendorRowViewModel
            {
                VendorAgingVendorRowId = Guid.NewGuid(),
                VendorAgingId = agingId,
                VendorId = Vendor5Id,
                VendorCodeSnapshot = "VND-000005",
                VendorNameSnapshot = "Tamil Nadu Electricity Board",
                CurrencyId = InrCurrencyId,
                CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                TotalOutstandingAmount = 195000.00m,
                CurrentNotDueAmount = 0.00m,
                Bucket_0_30 = 25000.00m,
                Bucket_31_60 = 20000.00m,
                Bucket_61_90 = 15000.00m,
                Bucket_91_120 = 25000.00m,
                Bucket_121_180 = 30000.00m,
                Bucket_181_365 = 40000.00m,
                Bucket_366_Plus = 40000.00m,
                OpenCreditAmount = 0.00m,
                NetPayableAmount = 195000.00m,
                CreditBalanceExcessAmount = 0.00m,
                OpenDocumentCount = 7,
                OldestDueDate = asOfDate.AddDays(-400),
                LatestBillDate = asOfDate.AddDays(-25),
                LatestPaymentDate = asOfDate.AddDays(-60),
                ReconciliationStatus = VendorAgingReconciliationStatuses.MinorDifference,
                ReconciliationDifferenceAmount = 150.00m,
                CreatedAt = DateTime.Now.AddHours(-2),
                CreatedBy = "System"
            };

            snapshot.VendorRows = new List<VendorAgingVendorRowViewModel> { vendor1Row, vendor2Row, vendor3Row, vendor4Row, vendor5Row };
            snapshot.BillRows = CreateBillDetails(agingId, asOfDate, 
                (Vendor1Id, 8), (Vendor2Id, 5), (Vendor3Id, 2), (Vendor4Id, 1), (Vendor5Id, 7));
            snapshot.RecalculateTotals();
            return snapshot;
        }

        private static VendorAgingViewModel CreateMonthEndSnapshot()
        {
            var agingId = VendorAging2Id;
            var lastMonthEnd = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);

            var snapshot = new VendorAgingViewModel
            {
                VendorAgingId = agingId,
                TenantId = MasterDataIds.Tenants.Default,
                CompanyId = AscendingSoftwareCompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                BranchId = null,
                CurrencyId = InrCurrencyId,
                CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                AsOfDate = lastMonthEnd,
                AgingRunId = Guid.NewGuid(),
                RunType = VendorAgingRunTypes.MonthEndFinal,
                RunStatus = VendorAgingRunStatuses.Finalized,
                GeneratedAt = lastMonthEnd.AddHours(23),
                GeneratedByUserId = MasterDataIds.Tenants.Default,
                GeneratedByUserName = "Finance Controller",
                IncludeOpenCredits = true,
                CreatedAt = lastMonthEnd.AddHours(23),
                CreatedBy = "Finance Controller"
            };

            var vendor1Row = new VendorAgingVendorRowViewModel
            {
                VendorAgingVendorRowId = Guid.NewGuid(),
                VendorAgingId = agingId,
                VendorId = Vendor1Id,
                VendorCodeSnapshot = "VND-000001",
                VendorNameSnapshot = "Tech Components India Pvt Ltd",
                CurrencyId = InrCurrencyId,
                CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                TotalOutstandingAmount = 420000.00m,
                CurrentNotDueAmount = 100000.00m,
                Bucket_0_30 = 110000.00m,
                Bucket_31_60 = 80000.00m,
                Bucket_61_90 = 60000.00m,
                Bucket_91_120 = 40000.00m,
                Bucket_121_180 = 30000.00m,
                Bucket_181_365 = 0.00m,
                Bucket_366_Plus = 0.00m,
                OpenCreditAmount = 20000.00m,
                NetPayableAmount = 400000.00m,
                CreditBalanceExcessAmount = 0.00m,
                OpenDocumentCount = 7,
                OldestDueDate = lastMonthEnd.AddDays(-145),
                LatestBillDate = lastMonthEnd.AddDays(-3),
                ReconciliationStatus = VendorAgingReconciliationStatuses.Matched,
                CreatedAt = lastMonthEnd.AddHours(23),
                CreatedBy = "Finance Controller"
            };

            var vendor2Row = new VendorAgingVendorRowViewModel
            {
                VendorAgingVendorRowId = Guid.NewGuid(),
                VendorAgingId = agingId,
                VendorId = Vendor2Id,
                VendorCodeSnapshot = "VND-000002",
                VendorNameSnapshot = "CloudTech Solutions",
                CurrencyId = InrCurrencyId,
                CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                TotalOutstandingAmount = 250000.00m,
                CurrentNotDueAmount = 120000.00m,
                Bucket_0_30 = 70000.00m,
                Bucket_31_60 = 45000.00m,
                Bucket_61_90 = 15000.00m,
                Bucket_91_120 = 0.00m,
                Bucket_121_180 = 0.00m,
                Bucket_181_365 = 0.00m,
                Bucket_366_Plus = 0.00m,
                OpenCreditAmount = 10000.00m,
                NetPayableAmount = 240000.00m,
                CreditBalanceExcessAmount = 0.00m,
                OpenDocumentCount = 4,
                OldestDueDate = lastMonthEnd.AddDays(-75),
                ReconciliationStatus = VendorAgingReconciliationStatuses.Matched,
                CreatedAt = lastMonthEnd.AddHours(23),
                CreatedBy = "Finance Controller"
            };

            snapshot.VendorRows = new List<VendorAgingVendorRowViewModel> { vendor1Row, vendor2Row };
            snapshot.BillRows = CreateBillDetails(agingId, lastMonthEnd, (Vendor1Id, 7), (Vendor2Id, 4));
            snapshot.RecalculateTotals();
            return snapshot;
        }

        private static VendorAgingViewModel CreateBranchSpecificSnapshot()
        {
            var agingId = VendorAging3Id;
            var asOfDate = DateTime.Today.AddDays(-7);

            var snapshot = new VendorAgingViewModel
            {
                VendorAgingId = agingId,
                TenantId = MasterDataIds.Tenants.Default,
                CompanyId = AscendingSoftwareCompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                BranchId = ChennaiHQBranchId,
                BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.CozyCraftHyderabad),
                BranchName = SeedLookup.BranchName(MasterDataIds.Branches.CozyCraftHyderabad),
                CurrencyId = InrCurrencyId,
                CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                AsOfDate = asOfDate,
                AgingRunId = Guid.NewGuid(),
                RunType = VendorAgingRunTypes.OnDemand,
                RunStatus = VendorAgingRunStatuses.Completed,
                GeneratedAt = asOfDate.AddHours(10),
                GeneratedByUserId = MasterDataIds.Tenants.Default,
                GeneratedByUserName = "Admin User",
                IncludeOpenCredits = true,
                CreatedAt = asOfDate.AddHours(10),
                CreatedBy = "Admin User"
            };

            var vendor1Row = new VendorAgingVendorRowViewModel
            {
                VendorAgingVendorRowId = Guid.NewGuid(),
                VendorAgingId = agingId,
                VendorId = Vendor1Id,
                VendorCodeSnapshot = "VND-000001",
                VendorNameSnapshot = "Tech Components India Pvt Ltd",
                CurrencyId = InrCurrencyId,
                CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                TotalOutstandingAmount = 280000.00m,
                CurrentNotDueAmount = 80000.00m,
                Bucket_0_30 = 75000.00m,
                Bucket_31_60 = 55000.00m,
                Bucket_61_90 = 40000.00m,
                Bucket_91_120 = 30000.00m,
                Bucket_121_180 = 0.00m,
                Bucket_181_365 = 0.00m,
                Bucket_366_Plus = 0.00m,
                OpenCreditAmount = 15000.00m,
                NetPayableAmount = 265000.00m,
                CreditBalanceExcessAmount = 0.00m,
                OpenDocumentCount = 5,
                OldestDueDate = asOfDate.AddDays(-115),
                ReconciliationStatus = VendorAgingReconciliationStatuses.Matched,
                CreatedAt = asOfDate.AddHours(10),
                CreatedBy = "Admin User"
            };

            var vendor3Row = new VendorAgingVendorRowViewModel
            {
                VendorAgingVendorRowId = Guid.NewGuid(),
                VendorAgingId = agingId,
                VendorId = Vendor3Id,
                VendorCodeSnapshot = "VND-000003",
                VendorNameSnapshot = "Reliable Supplies Co",
                CurrencyId = InrCurrencyId,
                CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                TotalOutstandingAmount = 90000.00m,
                CurrentNotDueAmount = 70000.00m,
                Bucket_0_30 = 20000.00m,
                Bucket_31_60 = 0.00m,
                Bucket_61_90 = 0.00m,
                Bucket_91_120 = 0.00m,
                Bucket_121_180 = 0.00m,
                Bucket_181_365 = 0.00m,
                Bucket_366_Plus = 0.00m,
                OpenCreditAmount = 0.00m,
                NetPayableAmount = 90000.00m,
                CreditBalanceExcessAmount = 0.00m,
                OpenDocumentCount = 2,
                OldestDueDate = asOfDate.AddDays(-15),
                ReconciliationStatus = VendorAgingReconciliationStatuses.Matched,
                CreatedAt = asOfDate.AddHours(10),
                CreatedBy = "Admin User"
            };

            snapshot.VendorRows = new List<VendorAgingVendorRowViewModel> { vendor1Row, vendor3Row };
            snapshot.BillRows = CreateBillDetails(agingId, asOfDate, (Vendor1Id, 5), (Vendor3Id, 2));
            snapshot.RecalculateTotals();
            return snapshot;
        }

        private static VendorAgingViewModel CreateFailedSnapshot()
        {
            var agingId = VendorAging4Id;
            var asOfDate = DateTime.Today.AddDays(-3);

            return new VendorAgingViewModel
            {
                VendorAgingId = agingId,
                TenantId = MasterDataIds.Tenants.Default,
                CompanyId = GlobalTechCompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                BranchId = null,
                CurrencyId = UsdCurrencyId,
                CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.USD),
                CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.USD),
                AsOfDate = asOfDate,
                AgingRunId = Guid.NewGuid(),
                RunType = VendorAgingRunTypes.Nightly,
                RunStatus = VendorAgingRunStatuses.Failed,
                GeneratedAt = asOfDate.AddHours(8),
                GeneratedByUserId = MasterDataIds.Tenants.Default,
                GeneratedByUserName = "System",
                IncludeOpenCredits = true,
                RecordCountVendors = 0,
                RecordCountBills = 0,
                TotalOutstandingAmount = 0,
                VendorRows = new List<VendorAgingVendorRowViewModel>(),
                BillRows = new List<VendorAgingBillRowViewModel>(),
                CreatedAt = asOfDate.AddHours(8),
                CreatedBy = "System"
            };
        }

        private static VendorAgingViewModel CreateOnDemandNoCreditsSnapshot()
        {
            var agingId = VendorAging5Id;
            var asOfDate = DateTime.Today.AddDays(-14);

            var snapshot = new VendorAgingViewModel
            {
                VendorAgingId = agingId,
                TenantId = MasterDataIds.Tenants.Default,
                CompanyId = AscendingSoftwareCompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                BranchId = BangaloreBranchId,
                BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.CozyCraftHyderabad),
                BranchName = SeedLookup.BranchName(MasterDataIds.Branches.CozyCraftHyderabad),
                CurrencyId = InrCurrencyId,
                CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                AsOfDate = asOfDate,
                AgingRunId = Guid.NewGuid(),
                RunType = VendorAgingRunTypes.OnDemand,
                RunStatus = VendorAgingRunStatuses.Completed,
                GeneratedAt = asOfDate.AddHours(14),
                GeneratedByUserId = MasterDataIds.Tenants.Default,
                GeneratedByUserName = "CFO User",
                IncludeOpenCredits = false,
                CreatedAt = asOfDate.AddHours(14),
                CreatedBy = "CFO User"
            };

            var vendor2Row = new VendorAgingVendorRowViewModel
            {
                VendorAgingVendorRowId = Guid.NewGuid(),
                VendorAgingId = agingId,
                VendorId = Vendor2Id,
                VendorCodeSnapshot = "VND-000002",
                VendorNameSnapshot = "CloudTech Solutions",
                CurrencyId = InrCurrencyId,
                CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                TotalOutstandingAmount = 180000.00m,
                CurrentNotDueAmount = 80000.00m,
                Bucket_0_30 = 50000.00m,
                Bucket_31_60 = 30000.00m,
                Bucket_61_90 = 20000.00m,
                Bucket_91_120 = 0.00m,
                Bucket_121_180 = 0.00m,
                Bucket_181_365 = 0.00m,
                Bucket_366_Plus = 0.00m,
                OpenCreditAmount = 0.00m, // Credits excluded
                NetPayableAmount = 180000.00m,
                CreditBalanceExcessAmount = 0.00m,
                OpenDocumentCount = 3,
                OldestDueDate = asOfDate.AddDays(-65),
                ReconciliationStatus = VendorAgingReconciliationStatuses.Matched,
                CreatedAt = asOfDate.AddHours(14),
                CreatedBy = "CFO User"
            };

            snapshot.VendorRows = new List<VendorAgingVendorRowViewModel> { vendor2Row };
            snapshot.BillRows = CreateBillDetails(agingId, asOfDate, (Vendor2Id, 3));
            snapshot.RecalculateTotals();
            return snapshot;
        }

        private static List<VendorAgingBillRowViewModel> CreateBillDetails(Guid agingId, DateTime asOfDate, params (Guid VendorId, int Count)[] vendorBillCounts)
        {
            var bills = new List<VendorAgingBillRowViewModel>();
            var random = new Random(42); // Fixed seed for reproducibility

            foreach (var (vendorId, count) in vendorBillCounts)
            {
                for (int i = 0; i < count; i++)
                {
                    var dueDate = asOfDate.AddDays(-random.Next(-15, 420)); // Random due date spanning all buckets
                    var overdueDays = Math.Max(0, (asOfDate - dueDate).Days);
                    var bucketCode = VendorAgingBucketCodes.GetBucketCode(overdueDays);
                    var billDate = dueDate.AddDays(-random.Next(15, 45)); // Bill date 15-45 days before due

                    bills.Add(new VendorAgingBillRowViewModel
                    {
                        VendorAgingBillRowId = Guid.NewGuid(),
                        VendorAgingId = agingId,
                        VendorId = vendorId,
                        VendorBillId = Guid.NewGuid(),
                        BillNumberSnapshot = $"BILL-{billDate:yyyyMM}-{random.Next(1000, 9999)}",
                        BillDateSnapshot = billDate,
                        DueDateSnapshot = dueDate,
                        OutstandingAmount = Math.Round((decimal)(random.NextDouble() * 60000 + 5000), 2),
                        OverdueDays = overdueDays,
                        BucketCode = bucketCode,
                        LastPaymentDateSnapshot = random.Next(0, 3) == 1 ? asOfDate.AddDays(-random.Next(5, 60)) : null,
                        SourceDocumentType = random.Next(0, 10) < 8 ? VendorAgingSourceDocumentTypes.Bill : VendorAgingSourceDocumentTypes.DebitNote,
                        CreatedAt = DateTime.Now.AddHours(-2),
                        CreatedBy = "System"
                    });
                }
            }

            return bills;
        }

        private static VendorAgingViewModel CreateWeeklySnapshot6()
        {
            return new VendorAgingViewModel
            {
                VendorAgingId = VendorAging6Id,
                TenantId = MasterDataIds.Tenants.Default,
                CompanyId = AscendingSoftwareCompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                CurrencyId = InrCurrencyId, CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR), CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                AsOfDate = DateTime.Today.AddDays(-7),
                AgingRunId = Guid.NewGuid(),
                RunType = VendorAgingRunTypes.OnDemand,
                RunStatus = VendorAgingRunStatuses.Completed,
                GeneratedAt = DateTime.Today.AddDays(-7),
                GeneratedByUserId = MasterDataIds.Tenants.Default,
                GeneratedByUserName = "Finance Manager",
                IncludeOpenCredits = true,
                CreatedAt = DateTime.Today.AddDays(-7), CreatedBy = "Finance Manager"
            };
        }

        private static VendorAgingViewModel CreateWeeklySnapshot7()
        {
            return new VendorAgingViewModel
            {
                VendorAgingId = VendorAging7Id,
                TenantId = MasterDataIds.Tenants.Default,
                CompanyId = AscendingSoftwareCompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                CurrencyId = InrCurrencyId, CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR), CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                AsOfDate = DateTime.Today.AddDays(-14),
                AgingRunId = Guid.NewGuid(),
                RunType = VendorAgingRunTypes.Nightly,
                RunStatus = VendorAgingRunStatuses.Completed,
                GeneratedAt = DateTime.Today.AddDays(-14),
                GeneratedByUserId = MasterDataIds.Tenants.Default,
                GeneratedByUserName = "System",
                IncludeOpenCredits = true,
                CreatedAt = DateTime.Today.AddDays(-14), CreatedBy = "System"
            };
        }

        private static VendorAgingViewModel CreateWeeklySnapshot8()
        {
            return new VendorAgingViewModel
            {
                VendorAgingId = VendorAging8Id,
                TenantId = MasterDataIds.Tenants.Default,
                CompanyId = AscendingSoftwareCompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                BranchId = BangaloreBranchId,
                CurrencyId = InrCurrencyId, CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR), CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                AsOfDate = DateTime.Today.AddDays(-21),
                AgingRunId = Guid.NewGuid(),
                RunType = VendorAgingRunTypes.OnDemand,
                RunStatus = VendorAgingRunStatuses.Completed,
                GeneratedAt = DateTime.Today.AddDays(-21),
                GeneratedByUserId = MasterDataIds.Tenants.Default,
                GeneratedByUserName = "AP Manager",
                IncludeOpenCredits = false,
                CreatedAt = DateTime.Today.AddDays(-21), CreatedBy = "AP Manager"
            };
        }

        private static VendorAgingViewModel CreateWeeklySnapshot9()
        {
            return new VendorAgingViewModel
            {
                VendorAgingId = VendorAging9Id,
                TenantId = MasterDataIds.Tenants.Default,
                CompanyId = AscendingSoftwareCompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                CurrencyId = InrCurrencyId, CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR), CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                AsOfDate = DateTime.Today.AddDays(-28),
                AgingRunId = Guid.NewGuid(),
                RunType = VendorAgingRunTypes.Nightly,
                RunStatus = VendorAgingRunStatuses.Completed,
                GeneratedAt = DateTime.Today.AddDays(-28),
                GeneratedByUserId = MasterDataIds.Tenants.Default,
                GeneratedByUserName = "System",
                IncludeOpenCredits = true,
                CreatedAt = DateTime.Today.AddDays(-28), CreatedBy = "System"
            };
        }

        private static VendorAgingViewModel CreateWeeklySnapshot10()
        {
            return new VendorAgingViewModel
            {
                VendorAgingId = VendorAging10Id,
                TenantId = MasterDataIds.Tenants.Default,
                CompanyId = AscendingSoftwareCompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                CurrencyId = UsdCurrencyId, CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.USD), CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.USD),
                AsOfDate = DateTime.Today.AddDays(-35),
                AgingRunId = Guid.NewGuid(),
                RunType = VendorAgingRunTypes.OnDemand,
                RunStatus = VendorAgingRunStatuses.Failed,
                GeneratedAt = DateTime.Today.AddDays(-35),
                GeneratedByUserId = MasterDataIds.Tenants.Default,
                GeneratedByUserName = "Finance Manager",
                IncludeOpenCredits = true,
                CreatedAt = DateTime.Today.AddDays(-35), CreatedBy = "Finance Manager"
            };
        }

        private static VendorAgingViewModel CreateWeeklySnapshot11()
        {
            return new VendorAgingViewModel
            {
                VendorAgingId = VendorAging11Id,
                TenantId = MasterDataIds.Tenants.Default,
                CompanyId = AscendingSoftwareCompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                CurrencyId = InrCurrencyId, CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR), CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                AsOfDate = DateTime.Today.AddDays(-42),
                AgingRunId = Guid.NewGuid(),
                RunType = VendorAgingRunTypes.Nightly,
                RunStatus = VendorAgingRunStatuses.Completed,
                GeneratedAt = DateTime.Today.AddDays(-42),
                GeneratedByUserId = MasterDataIds.Tenants.Default,
                GeneratedByUserName = "System",
                IncludeOpenCredits = true,
                CreatedAt = DateTime.Today.AddDays(-42), CreatedBy = "System"
            };
        }

        private static VendorAgingViewModel CreateWeeklySnapshot12()
        {
            return new VendorAgingViewModel
            {
                VendorAgingId = VendorAging12Id,
                TenantId = MasterDataIds.Tenants.Default,
                CompanyId = AscendingSoftwareCompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                BranchId = ChennaiHQBranchId,
                CurrencyId = InrCurrencyId, CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR), CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                AsOfDate = DateTime.Today.AddDays(-49),
                AgingRunId = Guid.NewGuid(),
                RunType = VendorAgingRunTypes.OnDemand,
                RunStatus = VendorAgingRunStatuses.Completed,
                GeneratedAt = DateTime.Today.AddDays(-49),
                GeneratedByUserId = MasterDataIds.Tenants.Default,
                GeneratedByUserName = "Finance Controller",
                IncludeOpenCredits = false,
                CreatedAt = DateTime.Today.AddDays(-49), CreatedBy = "Finance Controller"
            };
        }
    }
}
