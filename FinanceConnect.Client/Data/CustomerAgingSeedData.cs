using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    /// <summary>
    /// Seed data for CustomerAging model (Model #33)
    /// Provides aging snapshots with customer summaries and invoice details
    /// </summary>
    public static class CustomerAgingSeedData
    {
        // Company GUIDs (matching MasterDataIds.Companies)
        private static readonly Guid SofaCraftCompanyId = MasterDataIds.Companies.SofaCraft;
        private static readonly Guid SofaCraftUSACompanyId = MasterDataIds.Companies.SofaCraftUSA;

        // Branch GUIDs (matching MasterDataIds.Branches)
        private static readonly Guid ChennaiHQBranchId = MasterDataIds.Branches.SofaCraftHQ;
        private static readonly Guid BangaloreBranchId = MasterDataIds.Branches.SofaCraftBengaluru;
        private static readonly Guid HyderabadBranchId = MasterDataIds.Branches.SofaCraftDubai;

        // Customer GUIDs (matching CustomerSeedData)
        private static readonly Guid Customer1Id = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000001");
        private static readonly Guid Customer2Id = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000002");
        private static readonly Guid Customer3Id = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000003");
        private static readonly Guid Customer4Id = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000004");
        private static readonly Guid Customer5Id = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000005");

        // Currency GUIDs (matching MasterDataIds.Currencies)
        private static readonly Guid InrCurrencyId = MasterDataIds.Currencies.INR;
        private static readonly Guid UsdCurrencyId = MasterDataIds.Currencies.USD;

        // Invoice GUIDs (matching CustomerInvoiceSeedData)
        private static readonly Guid Invoice1Id = Guid.Parse("a1a1a1a1-0001-0001-0001-000000000001");
        private static readonly Guid Invoice2Id = Guid.Parse("a1a1a1a1-0001-0001-0001-000000000002");
        private static readonly Guid Invoice3Id = Guid.Parse("a1a1a1a1-0001-0001-0001-000000000003");
        private static readonly Guid Invoice4Id = Guid.Parse("a1a1a1a1-0001-0001-0001-000000000004");
        private static readonly Guid Invoice5Id = Guid.Parse("a1a1a1a1-0001-0001-0001-000000000005");
        private static readonly Guid Invoice6Id = Guid.Parse("a1a1a1a1-0001-0001-0001-000000000006");
        private static readonly Guid Invoice7Id = Guid.Parse("a1a1a1a1-0001-0001-0001-000000000007");
        private static readonly Guid Invoice8Id = Guid.Parse("a1a1a1a1-0001-0001-0001-000000000008");

        // Predefined Aging Snapshot GUIDs
        public static readonly Guid Aging1Id = Guid.Parse("e3e3e3e3-0001-0001-0001-000000000001");
        public static readonly Guid Aging2Id = Guid.Parse("e3e3e3e3-0001-0001-0001-000000000002");
        public static readonly Guid Aging3Id = Guid.Parse("e3e3e3e3-0001-0001-0001-000000000003");
        public static readonly Guid Aging4Id = Guid.Parse("e3e3e3e3-0001-0001-0001-000000000004");

        public static List<CustomerAgingViewModel> GetSeedAgingSnapshots()
        {
            var snapshots = new List<CustomerAgingViewModel>
            {
                // Snapshot 1: Today's snapshot - All Branches - Completed
                CreateTodaySnapshot(),
                
                // Snapshot 2: Month-end snapshot - Last month end - Completed
                CreateMonthEndSnapshot(),
                
                // Snapshot 3: Branch-specific snapshot - Chennai - Completed
                CreateBranchSpecificSnapshot(),
                
                // Snapshot 4: Failed snapshot - for testing retry
                CreateFailedSnapshot(),
                CreateWeeklyAgingSnapshot5(),
                CreateWeeklyAgingSnapshot6(),
                CreateWeeklyAgingSnapshot7(),
                CreateWeeklyAgingSnapshot8(),
                CreateWeeklyAgingSnapshot9(),
                CreateWeeklyAgingSnapshot10(),
                CreateWeeklyAgingSnapshot11(),
                CreateWeeklyAgingSnapshot12()
            };

            return snapshots;
        }

        private static CustomerAgingViewModel CreateTodaySnapshot()
        {
            var agingId = Aging1Id;
            var asOfDate = DateTime.Today;

            var snapshot = new CustomerAgingViewModel
            {
                CustomerAgingId = agingId,
                CompanyId = SofaCraftCompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                TenantId = MasterDataIds.Tenants.Default,
                BranchId = null, // All branches
                CurrencyId = InrCurrencyId,
                CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                AsOfDate = asOfDate,
                AgingBasis = AgingBasisTypes.DueDate,
                BucketPolicyVersion = 1,
                BucketDefinitionJson = "{\"Current\":\"<=0\",\"1-30\":\"1-30\",\"31-60\":\"31-60\",\"61-90\":\"61-90\",\"90+\":\">90\"}",
                SnapshotStatus = SnapshotStatuses.Completed,
                GeneratedOn = DateTime.Now.AddHours(-2),
                GeneratedByUserId = MasterDataIds.Tenants.Default,
                GeneratedByUserName = "System",
                JobRunId = $"JOB-{DateTime.Today:yyyyMMdd}-001",
                CreatedAt = DateTime.Now.AddHours(-2),
                CreatedBy = "System"
            };

            // Customer 1: ABC Traders - High overdue
            var customer1Row = new CustomerAgingCustomerRowModel
            {
                CustomerAgingCustomerRowId = Guid.NewGuid(),
                CustomerAgingId = agingId,
                CustomerId = Customer1Id,
                CustomerCodeSnapshot = "CUST-001",
                CustomerNameSnapshot = "ABC Traders Private Limited",
                TotalOutstanding = 285000.00m,
                BucketCurrentAmount = 50000.00m,
                Bucket1To30Amount = 75000.00m,
                Bucket31To60Amount = 60000.00m,
                Bucket61To90Amount = 50000.00m,
                Bucket90PlusAmount = 50000.00m,
                OldestDueDate = asOfDate.AddDays(-120),
                MaxOverdueDays = 120,
                InvoiceCountOpen = 5,
                CollectionsPriorityScore = 95,
                CreatedAt = DateTime.Now.AddHours(-2),
                CreatedBy = "System"
            };

            // Customer 2: XYZ Corporation - Medium overdue
            var customer2Row = new CustomerAgingCustomerRowModel
            {
                CustomerAgingCustomerRowId = Guid.NewGuid(),
                CustomerAgingId = agingId,
                CustomerId = Customer2Id,
                CustomerCodeSnapshot = "CUST-002",
                CustomerNameSnapshot = "State Government Education Dept",
                TotalOutstanding = 175000.00m,
                BucketCurrentAmount = 75000.00m,
                Bucket1To30Amount = 50000.00m,
                Bucket31To60Amount = 50000.00m,
                Bucket61To90Amount = 0.00m,
                Bucket90PlusAmount = 0.00m,
                OldestDueDate = asOfDate.AddDays(-55),
                MaxOverdueDays = 55,
                InvoiceCountOpen = 3,
                CollectionsPriorityScore = 65,
                CreatedAt = DateTime.Now.AddHours(-2),
                CreatedBy = "System"
            };

            // Customer 3: Global Solutions - Low overdue
            var customer3Row = new CustomerAgingCustomerRowModel
            {
                CustomerAgingCustomerRowId = Guid.NewGuid(),
                CustomerAgingId = agingId,
                CustomerId = Customer3Id,
                CustomerCodeSnapshot = "CUST-003",
                CustomerNameSnapshot = "XYZ Electronics Hub",
                TotalOutstanding = 125000.00m,
                BucketCurrentAmount = 100000.00m,
                Bucket1To30Amount = 25000.00m,
                Bucket31To60Amount = 0.00m,
                Bucket61To90Amount = 0.00m,
                Bucket90PlusAmount = 0.00m,
                OldestDueDate = asOfDate.AddDays(-20),
                MaxOverdueDays = 20,
                InvoiceCountOpen = 2,
                CollectionsPriorityScore = 30,
                CreatedAt = DateTime.Now.AddHours(-2),
                CreatedBy = "System"
            };

            // Customer 4: Tech Innovations - Current only
            var customer4Row = new CustomerAgingCustomerRowModel
            {
                CustomerAgingCustomerRowId = Guid.NewGuid(),
                CustomerAgingId = agingId,
                CustomerId = Customer4Id,
                CustomerCodeSnapshot = "CUST-004",
                CustomerNameSnapshot = "Rajesh Kumar",
                TotalOutstanding = 85000.00m,
                BucketCurrentAmount = 85000.00m,
                Bucket1To30Amount = 0.00m,
                Bucket31To60Amount = 0.00m,
                Bucket61To90Amount = 0.00m,
                Bucket90PlusAmount = 0.00m,
                OldestDueDate = asOfDate.AddDays(10), // Due in future
                MaxOverdueDays = 0,
                InvoiceCountOpen = 1,
                CollectionsPriorityScore = 10,
                CreatedAt = DateTime.Now.AddHours(-2),
                CreatedBy = "System"
            };

            // Customer 5: Premium Services - 90+ only
            var customer5Row = new CustomerAgingCustomerRowModel
            {
                CustomerAgingCustomerRowId = Guid.NewGuid(),
                CustomerAgingId = agingId,
                CustomerId = Customer5Id,
                CustomerCodeSnapshot = "CUST-005",
                CustomerNameSnapshot = "TechPark SEZ Solutions Pvt Ltd",
                TotalOutstanding = 150000.00m,
                BucketCurrentAmount = 0.00m,
                Bucket1To30Amount = 0.00m,
                Bucket31To60Amount = 0.00m,
                Bucket61To90Amount = 50000.00m,
                Bucket90PlusAmount = 100000.00m,
                OldestDueDate = asOfDate.AddDays(-150),
                MaxOverdueDays = 150,
                InvoiceCountOpen = 2,
                CollectionsPriorityScore = 98,
                CreatedAt = DateTime.Now.AddHours(-2),
                CreatedBy = "System"
            };

            snapshot.CustomerRows = new List<CustomerAgingCustomerRowModel>
            {
                customer1Row, customer2Row, customer3Row, customer4Row, customer5Row
            };

            // Invoice Details
            snapshot.InvoiceRows = CreateInvoiceDetails(agingId, asOfDate,
                (Customer1Id, 5), (Customer2Id, 3), (Customer3Id, 2), (Customer4Id, 1), (Customer5Id, 2));

            snapshot.RecalculateTotals();
            return snapshot;
        }

        private static CustomerAgingViewModel CreateMonthEndSnapshot()
        {
            var agingId = Aging2Id;
            var asOfDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1); // Last day of previous month

            var snapshot = new CustomerAgingViewModel
            {
                CustomerAgingId = agingId,
                CompanyId = SofaCraftCompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                TenantId = MasterDataIds.Tenants.Default,
                BranchId = null, // All branches
                CurrencyId = InrCurrencyId,
                CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                AsOfDate = asOfDate,
                AgingBasis = AgingBasisTypes.DueDate,
                BucketPolicyVersion = 1,
                BucketDefinitionJson = "{\"Current\":\"<=0\",\"1-30\":\"1-30\",\"31-60\":\"31-60\",\"61-90\":\"61-90\",\"90+\":\">90\"}",
                SnapshotStatus = SnapshotStatuses.Completed,
                GeneratedOn = asOfDate.AddHours(23),
                GeneratedByUserId = MasterDataIds.Tenants.Default,
                GeneratedByUserName = "System",
                JobRunId = $"JOB-{asOfDate:yyyyMMdd}-MONTHEND",
                CreatedAt = asOfDate.AddHours(23),
                CreatedBy = "System"
            };

            // Simplified customer rows for month-end
            var customer1Row = new CustomerAgingCustomerRowModel
            {
                CustomerAgingCustomerRowId = Guid.NewGuid(),
                CustomerAgingId = agingId,
                CustomerId = Customer1Id,
                CustomerCodeSnapshot = "CUST-001",
                CustomerNameSnapshot = "ABC Traders Private Limited",
                TotalOutstanding = 320000.00m,
                BucketCurrentAmount = 80000.00m,
                Bucket1To30Amount = 90000.00m,
                Bucket31To60Amount = 70000.00m,
                Bucket61To90Amount = 40000.00m,
                Bucket90PlusAmount = 40000.00m,
                OldestDueDate = asOfDate.AddDays(-100),
                MaxOverdueDays = 100,
                InvoiceCountOpen = 6,
                CollectionsPriorityScore = 92,
                CreatedAt = asOfDate.AddHours(23),
                CreatedBy = "System"
            };

            var customer2Row = new CustomerAgingCustomerRowModel
            {
                CustomerAgingCustomerRowId = Guid.NewGuid(),
                CustomerAgingId = agingId,
                CustomerId = Customer2Id,
                CustomerCodeSnapshot = "CUST-002",
                CustomerNameSnapshot = "State Government Education Dept",
                TotalOutstanding = 200000.00m,
                BucketCurrentAmount = 100000.00m,
                Bucket1To30Amount = 60000.00m,
                Bucket31To60Amount = 40000.00m,
                Bucket61To90Amount = 0.00m,
                Bucket90PlusAmount = 0.00m,
                OldestDueDate = asOfDate.AddDays(-45),
                MaxOverdueDays = 45,
                InvoiceCountOpen = 4,
                CollectionsPriorityScore = 55,
                CreatedAt = asOfDate.AddHours(23),
                CreatedBy = "System"
            };

            snapshot.CustomerRows = new List<CustomerAgingCustomerRowModel> { customer1Row, customer2Row };
            snapshot.InvoiceRows = CreateInvoiceDetails(agingId, asOfDate, (Customer1Id, 6), (Customer2Id, 4));
            snapshot.RecalculateTotals();
            return snapshot;
        }

        private static CustomerAgingViewModel CreateBranchSpecificSnapshot()
        {
            var agingId = Aging3Id;
            var asOfDate = DateTime.Today.AddDays(-7);

            var snapshot = new CustomerAgingViewModel
            {
                CustomerAgingId = agingId,
                CompanyId = SofaCraftCompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                TenantId = MasterDataIds.Tenants.Default,
                BranchId = ChennaiHQBranchId,
                BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftDubai),
                BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftDubai),
                CurrencyId = InrCurrencyId,
                CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                AsOfDate = asOfDate,
                AgingBasis = AgingBasisTypes.DueDate,
                BucketPolicyVersion = 1,
                BucketDefinitionJson = "{\"Current\":\"<=0\",\"1-30\":\"1-30\",\"31-60\":\"31-60\",\"61-90\":\"61-90\",\"90+\":\">90\"}",
                SnapshotStatus = SnapshotStatuses.Completed,
                GeneratedOn = asOfDate.AddHours(10),
                GeneratedByUserId = MasterDataIds.Tenants.Default,
                GeneratedByUserName = "Admin User",
                JobRunId = $"JOB-{asOfDate:yyyyMMdd}-CHN",
                CreatedAt = asOfDate.AddHours(10),
                CreatedBy = "Admin User"
            };

            var customer1Row = new CustomerAgingCustomerRowModel
            {
                CustomerAgingCustomerRowId = Guid.NewGuid(),
                CustomerAgingId = agingId,
                CustomerId = Customer1Id,
                CustomerCodeSnapshot = "CUST-001",
                CustomerNameSnapshot = "ABC Traders Private Limited",
                TotalOutstanding = 180000.00m,
                BucketCurrentAmount = 50000.00m,
                Bucket1To30Amount = 50000.00m,
                Bucket31To60Amount = 40000.00m,
                Bucket61To90Amount = 20000.00m,
                Bucket90PlusAmount = 20000.00m,
                OldestDueDate = asOfDate.AddDays(-95),
                MaxOverdueDays = 95,
                InvoiceCountOpen = 4,
                CollectionsPriorityScore = 88,
                CreatedAt = asOfDate.AddHours(10),
                CreatedBy = "Admin User"
            };

            var customer3Row = new CustomerAgingCustomerRowModel
            {
                CustomerAgingCustomerRowId = Guid.NewGuid(),
                CustomerAgingId = agingId,
                CustomerId = Customer3Id,
                CustomerCodeSnapshot = "CUST-003",
                CustomerNameSnapshot = "XYZ Electronics Hub",
                TotalOutstanding = 90000.00m,
                BucketCurrentAmount = 70000.00m,
                Bucket1To30Amount = 20000.00m,
                Bucket31To60Amount = 0.00m,
                Bucket61To90Amount = 0.00m,
                Bucket90PlusAmount = 0.00m,
                OldestDueDate = asOfDate.AddDays(-15),
                MaxOverdueDays = 15,
                InvoiceCountOpen = 2,
                CollectionsPriorityScore = 25,
                CreatedAt = asOfDate.AddHours(10),
                CreatedBy = "Admin User"
            };

            snapshot.CustomerRows = new List<CustomerAgingCustomerRowModel> { customer1Row, customer3Row };
            snapshot.InvoiceRows = CreateInvoiceDetails(agingId, asOfDate, (Customer1Id, 4), (Customer3Id, 2));
            snapshot.RecalculateTotals();
            return snapshot;
        }

        private static CustomerAgingViewModel CreateFailedSnapshot()
        {
            var agingId = Aging4Id;
            var asOfDate = DateTime.Today.AddDays(-3);

            return new CustomerAgingViewModel
            {
                CustomerAgingId = agingId,
                CompanyId = SofaCraftUSACompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                TenantId = MasterDataIds.Tenants.Default,
                BranchId = null,
                CurrencyId = UsdCurrencyId,
                CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.USD),
                CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.USD),
                AsOfDate = asOfDate,
                AgingBasis = AgingBasisTypes.InvoiceDate,
                BucketPolicyVersion = 1,
                BucketDefinitionJson = "{\"Current\":\"<=0\",\"1-30\":\"1-30\",\"31-60\":\"31-60\",\"61-90\":\"61-90\",\"90+\":\">90\"}",
                SnapshotStatus = SnapshotStatuses.Failed,
                GeneratedOn = asOfDate.AddHours(8),
                GeneratedByUserId = MasterDataIds.Tenants.Default,
                GeneratedByUserName = "System",
                JobRunId = $"JOB-{asOfDate:yyyyMMdd}-FAILED",
                RecordCountCustomers = 0,
                RecordCountInvoices = 0,
                TotalOutstandingAmount = 0,
                CustomerRows = new List<CustomerAgingCustomerRowModel>(),
                InvoiceRows = new List<CustomerAgingInvoiceRowModel>(),
                CreatedAt = asOfDate.AddHours(8),
                CreatedBy = "System"
            };
        }

        private static List<CustomerAgingInvoiceRowModel> CreateInvoiceDetails(Guid agingId, DateTime asOfDate, params (Guid CustomerId, int Count)[] customerInvoiceCounts)
        {
            var invoices = new List<CustomerAgingInvoiceRowModel>();
            var random = new Random(42); // Fixed seed for reproducibility

            foreach (var (customerId, count) in customerInvoiceCounts)
            {
                for (int i = 0; i < count; i++)
                {
                    var dueDate = asOfDate.AddDays(-random.Next(-10, 160)); // Random due date
                    var overdueDays = Math.Max(0, (asOfDate - dueDate).Days);
                    var bucketCode = AgingBucketCodes.GetBucketCode(overdueDays);
                    var invoiceDate = dueDate.AddDays(-30); // Invoice date 30 days before due

                    invoices.Add(new CustomerAgingInvoiceRowModel
                    {
                        CustomerAgingInvoiceRowId = Guid.NewGuid(),
                        CustomerAgingId = agingId,
                        CustomerId = customerId,
                        CustomerInvoiceId = Guid.NewGuid(),
                        InvoiceNumberSnapshot = $"INV-{invoiceDate:yyyyMM}-{random.Next(1000, 9999)}",
                        InvoiceDateSnapshot = invoiceDate,
                        DueDateSnapshot = dueDate,
                        OutstandingAmount = Math.Round((decimal)(random.NextDouble() * 80000 + 10000), 2),
                        OverdueDays = overdueDays,
                        BucketCode = bucketCode,
                        LastPaymentDateSnapshot = random.Next(0, 2) == 1 ? asOfDate.AddDays(-random.Next(5, 30)) : null,
                        SourceDocumentType = SourceDocumentTypes.Invoice,
                        CreatedAt = DateTime.Now.AddHours(-2),
                        CreatedBy = "System"
                    });
                }
            }

            return invoices;
        }
        private static CustomerAgingViewModel CreateWeeklyAgingSnapshot5() =>
            new CustomerAgingViewModel { CustomerAgingId = Guid.NewGuid(), TenantId = MasterDataIds.Tenants.Default, CompanyId = SofaCraftCompanyId, CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft), CurrencyId = InrCurrencyId, CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR), CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR), AsOfDate = DateTime.Today.AddDays(-7), AgingBasis = AgingBasisTypes.InvoiceDate, BucketPolicyVersion = 1, BucketDefinitionJson = "{\"Current\":\"<=0\",\"1-30\":\"1-30\",\"31-60\":\"31-60\",\"61-90\":\"61-90\",\"90+\":\">90\"}", SnapshotStatus = SnapshotStatuses.Completed, GeneratedOn = DateTime.Today.AddDays(-7), GeneratedByUserId = MasterDataIds.Tenants.Default, GeneratedByUserName = "Finance Manager", JobRunId = $"JOB-{DateTime.Today.AddDays(-7):yyyyMMdd}-005", CreatedAt = DateTime.Today.AddDays(-7), CreatedBy = "Finance Manager" };

        private static CustomerAgingViewModel CreateWeeklyAgingSnapshot6() =>
            new CustomerAgingViewModel { CustomerAgingId = Guid.NewGuid(), TenantId = MasterDataIds.Tenants.Default, CompanyId = SofaCraftCompanyId, CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft), CurrencyId = InrCurrencyId, CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR), CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR), AsOfDate = DateTime.Today.AddDays(-14), AgingBasis = AgingBasisTypes.DueDate, BucketPolicyVersion = 1, BucketDefinitionJson = "{\"Current\":\"<=0\",\"1-30\":\"1-30\",\"31-60\":\"31-60\",\"61-90\":\"61-90\",\"90+\":\">90\"}", SnapshotStatus = SnapshotStatuses.Completed, GeneratedOn = DateTime.Today.AddDays(-14), GeneratedByUserId = MasterDataIds.Tenants.Default, GeneratedByUserName = "System", JobRunId = $"JOB-{DateTime.Today.AddDays(-14):yyyyMMdd}-006", CreatedAt = DateTime.Today.AddDays(-14), CreatedBy = "System" };

        private static CustomerAgingViewModel CreateWeeklyAgingSnapshot7() =>
            new CustomerAgingViewModel { CustomerAgingId = Guid.NewGuid(), TenantId = MasterDataIds.Tenants.Default, CompanyId = SofaCraftCompanyId, CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft), BranchId = BangaloreBranchId, CurrencyId = InrCurrencyId, CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR), CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR), AsOfDate = DateTime.Today.AddDays(-21), AgingBasis = AgingBasisTypes.DueDate, BucketPolicyVersion = 1, BucketDefinitionJson = "{\"Current\":\"<=0\",\"1-30\":\"1-30\",\"31-60\":\"31-60\",\"61-90\":\"61-90\",\"90+\":\">90\"}", SnapshotStatus = SnapshotStatuses.Completed, GeneratedOn = DateTime.Today.AddDays(-21), GeneratedByUserId = MasterDataIds.Tenants.Default, GeneratedByUserName = "AR Manager", JobRunId = $"JOB-{DateTime.Today.AddDays(-21):yyyyMMdd}-007", CreatedAt = DateTime.Today.AddDays(-21), CreatedBy = "AR Manager" };

        private static CustomerAgingViewModel CreateWeeklyAgingSnapshot8() =>
            new CustomerAgingViewModel { CustomerAgingId = Guid.NewGuid(), TenantId = MasterDataIds.Tenants.Default, CompanyId = SofaCraftCompanyId, CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft), CurrencyId = InrCurrencyId, CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR), CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR), AsOfDate = DateTime.Today.AddDays(-28), AgingBasis = AgingBasisTypes.DueDate, BucketPolicyVersion = 1, BucketDefinitionJson = "{\"Current\":\"<=0\",\"1-30\":\"1-30\",\"31-60\":\"31-60\",\"61-90\":\"61-90\",\"90+\":\">90\"}", SnapshotStatus = SnapshotStatuses.Completed, GeneratedOn = DateTime.Today.AddDays(-28), GeneratedByUserId = MasterDataIds.Tenants.Default, GeneratedByUserName = "System", JobRunId = $"JOB-{DateTime.Today.AddDays(-28):yyyyMMdd}-008", CreatedAt = DateTime.Today.AddDays(-28), CreatedBy = "System" };

        private static CustomerAgingViewModel CreateWeeklyAgingSnapshot9() =>
            new CustomerAgingViewModel { CustomerAgingId = Guid.NewGuid(), TenantId = MasterDataIds.Tenants.Default, CompanyId = SofaCraftCompanyId, CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft), CurrencyId = MasterDataIds.Currencies.USD, CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.USD), CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.USD), AsOfDate = DateTime.Today.AddDays(-35), AgingBasis = AgingBasisTypes.DueDate, BucketPolicyVersion = 1, BucketDefinitionJson = "{\"Current\":\"<=0\",\"1-30\":\"1-30\",\"31-60\":\"31-60\",\"61-90\":\"61-90\",\"90+\":\">90\"}", SnapshotStatus = SnapshotStatuses.Failed, GeneratedOn = DateTime.Today.AddDays(-35), GeneratedByUserId = MasterDataIds.Tenants.Default, GeneratedByUserName = "Finance Manager", JobRunId = $"JOB-{DateTime.Today.AddDays(-35):yyyyMMdd}-009-FAILED", CreatedAt = DateTime.Today.AddDays(-35), CreatedBy = "Finance Manager" };

        private static CustomerAgingViewModel CreateWeeklyAgingSnapshot10() =>
            new CustomerAgingViewModel { CustomerAgingId = Guid.NewGuid(), TenantId = MasterDataIds.Tenants.Default, CompanyId = SofaCraftCompanyId, CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft), CurrencyId = InrCurrencyId, CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR), CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR), AsOfDate = DateTime.Today.AddDays(-42), AgingBasis = AgingBasisTypes.InvoiceDate, BucketPolicyVersion = 1, BucketDefinitionJson = "{\"Current\":\"<=0\",\"1-30\":\"1-30\",\"31-60\":\"31-60\",\"61-90\":\"61-90\",\"90+\":\">90\"}", SnapshotStatus = SnapshotStatuses.Completed, GeneratedOn = DateTime.Today.AddDays(-42), GeneratedByUserId = MasterDataIds.Tenants.Default, GeneratedByUserName = "System", JobRunId = $"JOB-{DateTime.Today.AddDays(-42):yyyyMMdd}-010", CreatedAt = DateTime.Today.AddDays(-42), CreatedBy = "System" };

        private static CustomerAgingViewModel CreateWeeklyAgingSnapshot11() =>
            new CustomerAgingViewModel { CustomerAgingId = Guid.NewGuid(), TenantId = MasterDataIds.Tenants.Default, CompanyId = SofaCraftCompanyId, CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft), BranchId = ChennaiHQBranchId, CurrencyId = InrCurrencyId, CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR), CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR), AsOfDate = DateTime.Today.AddDays(-49), AgingBasis = AgingBasisTypes.DueDate, BucketPolicyVersion = 1, BucketDefinitionJson = "{\"Current\":\"<=0\",\"1-30\":\"1-30\",\"31-60\":\"31-60\",\"61-90\":\"61-90\",\"90+\":\">90\"}", SnapshotStatus = SnapshotStatuses.Completed, GeneratedOn = DateTime.Today.AddDays(-49), GeneratedByUserId = MasterDataIds.Tenants.Default, GeneratedByUserName = "Finance Controller", JobRunId = $"JOB-{DateTime.Today.AddDays(-49):yyyyMMdd}-011", CreatedAt = DateTime.Today.AddDays(-49), CreatedBy = "Finance Controller" };

        private static CustomerAgingViewModel CreateWeeklyAgingSnapshot12() =>
            new CustomerAgingViewModel { CustomerAgingId = Guid.NewGuid(), TenantId = MasterDataIds.Tenants.Default, CompanyId = SofaCraftCompanyId, CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft), CurrencyId = InrCurrencyId, CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR), CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR), AsOfDate = DateTime.Today.AddDays(-56), AgingBasis = AgingBasisTypes.DueDate, BucketPolicyVersion = 1, BucketDefinitionJson = "{\"Current\":\"<=0\",\"1-30\":\"1-30\",\"31-60\":\"31-60\",\"61-90\":\"61-90\",\"90+\":\">90\"}", SnapshotStatus = SnapshotStatuses.Completed, GeneratedOn = DateTime.Today.AddDays(-56), GeneratedByUserId = MasterDataIds.Tenants.Default, GeneratedByUserName = "System", JobRunId = $"JOB-{DateTime.Today.AddDays(-56):yyyyMMdd}-012", CreatedAt = DateTime.Today.AddDays(-56), CreatedBy = "System" };

    }
}
