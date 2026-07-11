using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    /// <summary>
    /// Seed data for VendorBill (Model #36) and VendorBillLine (Model #37)
    /// Provides demo data for AP vendor bills
    /// </summary>
    public static class VendorBillSeedData
    {
        // Fixed GUIDs for consistency
        private static readonly Guid TenantId = MasterDataIds.Tenants.Default;
        private static readonly Guid CompanyId = MasterDataIds.Companies.SofaCraft;
        private static readonly Guid BranchId1 = MasterDataIds.Branches.SofaCraftHQ;
        private static readonly Guid BranchId2 = MasterDataIds.Branches.SofaCraftBengaluru;
        private static readonly Guid CurrencyINR = MasterDataIds.Currencies.INR;
        private static readonly Guid CurrencyUSD = MasterDataIds.Currencies.USD;

        // Vendor IDs (from VendorSeedData - must match exactly)
        private static readonly Guid VendorTechComponents = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000001"); // Tech Components India Pvt Ltd
        private static readonly Guid VendorCloudTech = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000002"); // CloudTech Solutions
        private static readonly Guid VendorGlobalFreight = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000003"); // Reliable Supplies Co
        private static readonly Guid VendorProfessionalConsulting = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000004"); // BuildRight Constructions
        private static readonly Guid VendorCityUtilities = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000005"); // Tamil Nadu Electricity Board
        private static readonly Guid VendorPrimeProperties = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000006"); // Prime Properties

        // Expense Account IDs
        private static readonly Guid AccountSoftwareLicense = MasterDataIds.Accounts.CostOfMaterials;
        private static readonly Guid AccountITServices = MasterDataIds.Accounts.CostOfMaterials;
        private static readonly Guid AccountOfficeRent = MasterDataIds.Accounts.RentExpense;
        private static readonly Guid AccountUtilities = MasterDataIds.Accounts.UtilitiesExpense;
        private static readonly Guid AccountProfessionalFees = MasterDataIds.Accounts.SalariesWages;
        private static readonly Guid AccountOfficeSupplies = MasterDataIds.Accounts.CostOfMaterials;
        private static readonly Guid AccountEquipment = MasterDataIds.Accounts.FurnitureFixtures;
        private static readonly Guid AccountMaintenance = MasterDataIds.Accounts.UtilitiesExpense;

        // Payment Term IDs
        private static readonly Guid PaymentTermNet30 = MasterDataIds.PaymentTerms.Net30;
        private static readonly Guid PaymentTermNet45 = MasterDataIds.PaymentTerms.Net45;
        private static readonly Guid PaymentTermNet60 = MasterDataIds.PaymentTerms.Net60;

        public static List<VendorBillViewModel> GetSeedBills()
        {
            var bills = new List<VendorBillViewModel>();
            var today = DateTime.Today;

            // Bill 1: Posted - IT Services from CloudTech (with outstanding balance)
            var bill1 = new VendorBillViewModel
            {
                Id = Guid.Parse("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e001"),
                TenantId = TenantId,
                CompanyId = CompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                BranchId = BranchId1,
                BranchCode = "HO",
                BranchName = "SofaCraft Head Office & Factory - Chennai",
                VendorId = VendorCloudTech,
                VendorCode = "VND-000002",
                VendorName = "CloudTech Solutions",
                BillNumber = "APB-2025-0001",
                VendorInvoiceNumber = "CT/INV/2025/0001",
                VendorInvoiceDate = today.AddDays(-30),
                BillDate = today.AddDays(-28),
                DueDate = today.AddDays(2),
                PostingDate = today.AddDays(-28),
                CurrencyId = CurrencyINR,
                CurrencyCode = "INR",
                CurrencyName = "Indian Rupee",
                ExchangeRate = 1,
                BillType = BillTypes.ServiceExpense,
                BillNarration = "Cloud infrastructure and hosting services",
                IsGSTApplicable = true,
                VendorGSTINSnapshot = "29AABCC5678B1ZB",
                BillStatus = VendorBillStatuses.Posted,
                PostedOn = today.AddDays(-28),
                PostedByUserName = "Admin User",
                PaymentTermId = PaymentTermNet30,
                PaymentTermName = "Net 30 Days",
                PaymentTermDays = 30,
                SubTotalAmount = 500000,
                TaxTotalAmount = 90000,
                GrandTotalAmount = 590000,
                PaidAmount = 0,
                HasInvoiceAttachment = true,
                InvoiceAttachmentCount = 1,
                CreatedAt = today.AddDays(-28),
                CreatedBy = "Admin User"
            };
            bill1.Lines = new List<VendorBillLineViewModel>
            {
                CreateLine(bill1.Id, 10, VendorBillLineTypes.Service, "ERP License Annual Subscription", 1, 400000, 18, AccountSoftwareLicense, "5000", "Software License"),
                CreateLine(bill1.Id, 20, VendorBillLineTypes.Service, "Support & Maintenance", 1, 100000, 18, AccountMaintenance, "5001", "Maintenance Expense")
            };
            bill1.RecalculateTotals();
            bills.Add(bill1);

            // Bill 2: Posted - Components from Tech Components (with outstanding balance)
            var bill2 = new VendorBillViewModel
            {
                Id = Guid.Parse("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e002"),
                TenantId = TenantId,
                CompanyId = CompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                BranchId = BranchId1,
                BranchCode = "HO",
                BranchName = "SofaCraft Head Office & Factory - Chennai",
                VendorId = VendorTechComponents,
                VendorCode = "VND-000001",
                VendorName = "Tech Components India Pvt Ltd",
                BillNumber = "APB-2025-0002",
                VendorInvoiceNumber = "TC/2025/00234",
                VendorInvoiceDate = today.AddDays(-20),
                BillDate = today.AddDays(-18),
                DueDate = today.AddDays(12),
                PostingDate = today.AddDays(-18),
                CurrencyId = CurrencyINR,
                CurrencyCode = "INR",
                CurrencyName = "Indian Rupee",
                ExchangeRate = 1,
                BillType = BillTypes.GoodsPurchase,
                BillNarration = "Hardware components for server upgrade",
                IsGSTApplicable = true,
                VendorGSTINSnapshot = "33AABCT1234A1ZA",
                BillStatus = VendorBillStatuses.Posted,
                PostedOn = today.AddDays(-18),
                PostedByUserName = "Finance Manager",
                PaymentTermId = PaymentTermNet30,
                PaymentTermName = "Net 30 Days",
                PaymentTermDays = 30,
                SubTotalAmount = 250000,
                TaxTotalAmount = 45000,
                GrandTotalAmount = 295000,
                PaidAmount = 0,
                HasInvoiceAttachment = true,
                InvoiceAttachmentCount = 1,
                CreatedAt = today.AddDays(-18),
                CreatedBy = "Finance Manager"
            };
            bill2.Lines = new List<VendorBillLineViewModel>
            {
                CreateLine(bill2.Id, 10, VendorBillLineTypes.Service, "Cloud Architecture Consulting - 50 Hours", 50, 4000, 18, AccountITServices, "5002", "IT Services Expense"),
                CreateLine(bill2.Id, 20, VendorBillLineTypes.Service, "Migration Support Services", 1, 50000, 18, AccountITServices, "5002", "IT Services Expense")
            };
            bill2.RecalculateTotals();
            bills.Add(bill2);

            // Bill 3: Posted - Office Rent from Prime Properties (with outstanding balance)
            var bill3 = new VendorBillViewModel
            {
                Id = Guid.Parse("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e003"),
                TenantId = TenantId,
                CompanyId = CompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                BranchId = BranchId1,
                BranchCode = "HO",
                BranchName = "SofaCraft Head Office & Factory - Chennai",
                VendorId = VendorPrimeProperties,
                VendorCode = "VND-000006",
                VendorName = "Prime Properties",
                BillNumber = "APB-2025-0003",
                VendorInvoiceNumber = "PP/RENT/2025/001",
                VendorInvoiceDate = today.AddDays(-5),
                BillDate = today.AddDays(-4),
                DueDate = today.AddDays(26),
                PostingDate = today.AddDays(-4),
                CurrencyId = CurrencyINR,
                CurrencyCode = "INR",
                CurrencyName = "Indian Rupee",
                ExchangeRate = 1,
                BillType = BillTypes.Rent,
                BillNarration = "Office space rent for January 2025",
                IsGSTApplicable = true,
                VendorGSTINSnapshot = "33AAFPP1234F1ZF",
                IsTDSApplicable = true,
                TDSSectionCodeSnapshot = "194I",
                TDSRatePercentSnapshot = 10,
                BillStatus = VendorBillStatuses.Posted,
                PostedOn = today.AddDays(-4),
                PostedByUserName = "Finance Manager",
                PaymentTermId = PaymentTermNet30,
                PaymentTermName = "Net 30 Days",
                PaymentTermDays = 30,
                SubTotalAmount = 165000,
                TaxTotalAmount = 29700,
                GrandTotalAmount = 194700,
                PaidAmount = 0,
                CreatedAt = today.AddDays(-4),
                CreatedBy = "AP Clerk"
            };
            bill3.Lines = new List<VendorBillLineViewModel>
            {
                CreateLine(bill3.Id, 10, VendorBillLineTypes.Expense, "Office Rent - January 2025", 1, 150000, 18, AccountOfficeRent, "5003", "Office Rent Expense"),
                CreateLine(bill3.Id, 20, VendorBillLineTypes.Expense, "Maintenance Charges", 1, 15000, 18, AccountMaintenance, "5001", "Maintenance Expense")
            };
            bill3.RecalculateTotals();
            bills.Add(bill3);

            // Bill 4: Posted - Utilities from City Utilities Board (with outstanding balance)
            var bill4 = new VendorBillViewModel
            {
                Id = Guid.Parse("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e004"),
                TenantId = TenantId,
                CompanyId = CompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                BranchId = BranchId1,
                BranchCode = "HO",
                BranchName = "SofaCraft Head Office & Factory - Chennai",
                VendorId = VendorCityUtilities,
                VendorCode = "VND-000005",
                VendorName = "Tamil Nadu Electricity Board",
                BillNumber = "APB-2025-0004",
                VendorInvoiceNumber = "CUB/2025/0056",
                VendorInvoiceDate = today.AddDays(-10),
                BillDate = today.AddDays(-8),
                DueDate = today.AddDays(22),
                PostingDate = today.AddDays(-7),
                CurrencyId = CurrencyINR,
                CurrencyCode = "INR",
                CurrencyName = "Indian Rupee",
                ExchangeRate = 1,
                BillType = BillTypes.Utility,
                BillNarration = "Electricity and water for January 2025",
                IsGSTApplicable = true,
                VendorGSTINSnapshot = "33AABCE1234E1ZE",
                BillStatus = VendorBillStatuses.Posted,
                PostedOn = today.AddDays(-7),
                PostedByUserName = "AP Clerk",
                PaymentTermId = PaymentTermNet30,
                PaymentTermName = "Net 30 Days",
                PaymentTermDays = 30,
                SubTotalAmount = 57000,
                TaxTotalAmount = 10260,
                GrandTotalAmount = 67260,
                PaidAmount = 0,
                HasInvoiceAttachment = true,
                InvoiceAttachmentCount = 1,
                CreatedAt = today.AddDays(-8),
                CreatedBy = "AP Clerk"
            };
            bill4.Lines = new List<VendorBillLineViewModel>
            {
                CreateLine(bill4.Id, 10, VendorBillLineTypes.Expense, "Internet Services - Leased Line", 1, 45000, 18, AccountUtilities, "5004", "Utilities Expense"),
                CreateLine(bill4.Id, 20, VendorBillLineTypes.Expense, "VOIP Phone Services", 1, 12000, 18, AccountUtilities, "5004", "Utilities Expense")
            };
            bill4.RecalculateTotals();
            bills.Add(bill4);

            // Bill 5: Posted - Consulting Services from Professional Consulting Group (with outstanding)
            var bill5 = new VendorBillViewModel
            {
                Id = Guid.Parse("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e005"),
                TenantId = TenantId,
                CompanyId = CompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                BranchId = BranchId1,
                BranchCode = "HO",
                BranchName = "SofaCraft Head Office & Factory - Chennai",
                VendorId = VendorProfessionalConsulting,
                VendorCode = "VND-000004",
                VendorName = "BuildRight Constructions",
                BillNumber = "APB-2025-0005",
                VendorInvoiceNumber = "PCG/2025/087",
                VendorInvoiceDate = today.AddDays(-15),
                BillDate = today.AddDays(-14),
                DueDate = today.AddDays(16),
                PostingDate = today.AddDays(-12),
                CurrencyId = CurrencyINR,
                CurrencyCode = "INR",
                CurrencyName = "Indian Rupee",
                ExchangeRate = 1,
                BillType = BillTypes.ServiceExpense,
                BillNarration = "Professional consulting services - Q1 2025",
                IsGSTApplicable = true,
                VendorGSTINSnapshot = "33AABCP1234D1ZD",
                IsTDSApplicable = true,
                TDSSectionCodeSnapshot = "194J",
                TDSRatePercentSnapshot = 10,
                BillStatus = VendorBillStatuses.Posted,
                PostedOn = today.AddDays(-12),
                PostedByUserName = "Finance Manager",
                PaymentTermId = PaymentTermNet30,
                PaymentTermName = "Net 30 Days",
                PaymentTermDays = 30,
                SubTotalAmount = 380000,
                TaxTotalAmount = 68400,
                GrandTotalAmount = 448400,
                PaidAmount = 200000,
                HasInvoiceAttachment = true,
                InvoiceAttachmentCount = 2,
                CreatedAt = today.AddDays(-14),
                CreatedBy = "AP Clerk"
            };
            bill5.Lines = new List<VendorBillLineViewModel>
            {
                CreateLine(bill5.Id, 10, VendorBillLineTypes.Service, "Senior Developer - 160 Hours", 160, 1500, 18, AccountProfessionalFees, "5005", "Professional Fees"),
                CreateLine(bill5.Id, 20, VendorBillLineTypes.Service, "QA Engineer - 80 Hours", 80, 1000, 18, AccountProfessionalFees, "5005", "Professional Fees"),
                CreateLine(bill5.Id, 30, VendorBillLineTypes.Service, "Project Management", 1, 50000, 18, AccountProfessionalFees, "5005", "Professional Fees")
            };
            bill5.RecalculateTotals();
            bills.Add(bill5);

            // Bill 6: Posted - Freight Services from Global Freight (Fully Paid)
            var bill6 = new VendorBillViewModel
            {
                Id = Guid.Parse("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e006"),
                TenantId = TenantId,
                CompanyId = CompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                BranchId = BranchId1,
                BranchCode = "HO",
                BranchName = "SofaCraft Head Office & Factory - Chennai",
                VendorId = VendorGlobalFreight,
                VendorCode = "VND-000003",
                VendorName = "Reliable Supplies Co",
                BillNumber = "APB-2025-0006",
                VendorInvoiceNumber = "GFS/2025/0012",
                VendorInvoiceDate = today.AddDays(-45),
                BillDate = today.AddDays(-44),
                DueDate = today.AddDays(-14),
                PostingDate = today.AddDays(-44),
                CurrencyId = CurrencyINR,
                CurrencyCode = "INR",
                CurrencyName = "Indian Rupee",
                ExchangeRate = 1,
                BillType = BillTypes.ServiceExpense,
                BillNarration = "Freight and logistics services",
                IsGSTApplicable = true,
                VendorGSTINSnapshot = "27AABCG1234C1ZC",
                BillStatus = VendorBillStatuses.Posted,
                PostedOn = today.AddDays(-44),
                PostedByUserName = "Admin User",
                PaymentTermId = PaymentTermNet30,
                PaymentTermName = "Net 30 Days",
                PaymentTermDays = 30,
                SubTotalAmount = 85000,
                TaxTotalAmount = 15300,
                GrandTotalAmount = 100300,
                PaidAmount = 100300,
                HasInvoiceAttachment = true,
                InvoiceAttachmentCount = 3,
                CreatedAt = today.AddDays(-44),
                CreatedBy = "Admin User"
            };
            bill6.Lines = new List<VendorBillLineViewModel>
            {
                CreateLine(bill6.Id, 10, VendorBillLineTypes.Service, "Freight Charges - Domestic", 1, 50000, 18, AccountITServices, "5002", "Freight Expense"),
                CreateLine(bill6.Id, 20, VendorBillLineTypes.Service, "Handling Charges", 1, 25000, 18, AccountITServices, "5002", "Handling Expense"),
                CreateLine(bill6.Id, 30, VendorBillLineTypes.Service, "Insurance", 1, 10000, 18, AccountITServices, "5002", "Insurance Expense")
            };
            bill6.RecalculateTotals();
            bills.Add(bill6);

            // Bill 7: Posted - Additional components from Tech Components (outstanding)
            var bill7 = new VendorBillViewModel
            {
                Id = Guid.Parse("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e007"),
                TenantId = TenantId,
                CompanyId = CompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                BranchId = BranchId1,
                BranchCode = "HO",
                BranchName = "SofaCraft Head Office & Factory - Chennai",
                VendorId = VendorTechComponents,
                VendorCode = "VND-000001",
                VendorName = "Tech Components India Pvt Ltd",
                BillNumber = "APB-2025-0007",
                VendorInvoiceNumber = "TC/2025/0045",
                VendorInvoiceDate = today.AddDays(-2),
                BillDate = today.AddDays(-1),
                DueDate = today.AddDays(29),
                PostingDate = today.AddDays(-1),
                CurrencyId = CurrencyINR,
                CurrencyCode = "INR",
                CurrencyName = "Indian Rupee",
                ExchangeRate = 1,
                BillType = BillTypes.GoodsPurchase,
                BillNarration = "Computer peripherals and accessories",
                IsGSTApplicable = true,
                VendorGSTINSnapshot = "33AABCT1234A1ZA",
                BillStatus = VendorBillStatuses.Posted,
                PostedOn = today.AddDays(-1),
                PostedByUserName = "AP Clerk",
                PaymentTermId = PaymentTermNet30,
                PaymentTermName = "Net 30 Days",
                PaymentTermDays = 30,
                SubTotalAmount = 110000,
                TaxTotalAmount = 19800,
                GrandTotalAmount = 129800,
                PaidAmount = 0,
                CreatedAt = today.AddDays(-1),
                CreatedBy = "AP Clerk"
            };
            bill7.Lines = new List<VendorBillLineViewModel>
            {
                CreateLine(bill7.Id, 10, VendorBillLineTypes.Goods, "Printer Paper A4 - 100 Reams", 100, 350, 12, AccountOfficeSupplies, "5006", "Office Supplies"),
                CreateLine(bill7.Id, 20, VendorBillLineTypes.Goods, "Ink Cartridges", 20, 2500, 18, AccountOfficeSupplies, "5006", "Office Supplies"),
                CreateLine(bill7.Id, 30, VendorBillLineTypes.Goods, "Office Stationery Kit", 50, 500, 12, AccountOfficeSupplies, "5006", "Office Supplies")
            };
            bill7.RecalculateTotals();
            bills.Add(bill7);

            // Bill 8: Posted - Overdue bill from CloudTech (partially paid)
            var bill8 = new VendorBillViewModel
            {
                Id = Guid.Parse("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e008"),
                TenantId = TenantId,
                CompanyId = CompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                BranchId = BranchId2,
                BranchCode = "BLR",
                BranchName = "SofaCraft Experience Store - Bengaluru",
                VendorId = VendorCloudTech,
                VendorCode = "VND-000002",
                VendorName = "CloudTech Solutions",
                BillNumber = "APB-2025-0008",
                VendorInvoiceNumber = "CT/2025/00189",
                VendorInvoiceDate = today.AddDays(-60),
                BillDate = today.AddDays(-58),
                DueDate = today.AddDays(-28),
                PostingDate = today.AddDays(-58),
                CurrencyId = CurrencyINR,
                CurrencyCode = "INR",
                CurrencyName = "Indian Rupee",
                ExchangeRate = 1,
                BillType = BillTypes.ServiceExpense,
                BillNarration = "Database administration services",
                IsGSTApplicable = true,
                VendorGSTINSnapshot = "29AABCC5678B1ZB",
                BillStatus = VendorBillStatuses.Posted,
                PostedOn = today.AddDays(-58),
                PostedByUserName = "Finance Manager",
                PaymentTermId = PaymentTermNet30,
                PaymentTermName = "Net 30 Days",
                PaymentTermDays = 30,
                SubTotalAmount = 180000,
                TaxTotalAmount = 32400,
                GrandTotalAmount = 212400,
                PaidAmount = 100000,
                HasInvoiceAttachment = true,
                InvoiceAttachmentCount = 1,
                CreatedAt = today.AddDays(-58),
                CreatedBy = "Finance Manager"
            };
            bill8.Lines = new List<VendorBillLineViewModel>
            {
                CreateLine(bill8.Id, 10, VendorBillLineTypes.Service, "DBA Services - Production Support", 1, 120000, 18, AccountITServices, "5002", "IT Services Expense"),
                CreateLine(bill8.Id, 20, VendorBillLineTypes.Service, "Database Optimization", 1, 60000, 18, AccountITServices, "5002", "IT Services Expense")
            };
            bill8.RecalculateTotals();
            bills.Add(bill8);

            // Bill 9: Cancelled bill from Global Freight
            var bill9 = new VendorBillViewModel
            {
                Id = Guid.Parse("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e009"),
                TenantId = TenantId,
                CompanyId = CompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                BranchId = BranchId1,
                BranchCode = "HO",
                BranchName = "SofaCraft Head Office & Factory - Chennai",
                VendorId = VendorGlobalFreight,
                VendorCode = "VND-000003",
                VendorName = "Reliable Supplies Co",
                BillNumber = "APB-2025-0009",
                VendorInvoiceNumber = "GFS/CANCEL/2025/001",
                VendorInvoiceDate = today.AddDays(-25),
                BillDate = today.AddDays(-24),
                DueDate = today.AddDays(6),
                CurrencyId = CurrencyINR,
                CurrencyCode = "INR",
                CurrencyName = "Indian Rupee",
                ExchangeRate = 1,
                BillType = BillTypes.ServiceExpense,
                BillNarration = "Cancelled due to duplicate entry",
                IsGSTApplicable = true,
                BillStatus = VendorBillStatuses.Cancelled,
                CancellationReason = "Duplicate invoice entry - original invoice already processed",
                PaymentTermId = PaymentTermNet30,
                PaymentTermName = "Net 30 Days",
                PaymentTermDays = 30,
                CreatedAt = today.AddDays(-24),
                CreatedBy = "AP Clerk"
            };
            bill9.Lines = new List<VendorBillLineViewModel>
            {
                CreateLine(bill9.Id, 10, VendorBillLineTypes.Service, "Consulting Services", 1, 75000, 18, AccountProfessionalFees, "5005", "Professional Fees")
            };
            bill9.RecalculateTotals();
            bills.Add(bill9);

            // Bill 10: Posted - Second bill from City Utilities (fully paid)
            var bill10 = new VendorBillViewModel
            {
                Id = Guid.Parse("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e010"),
                TenantId = TenantId,
                CompanyId = CompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                BranchId = BranchId1,
                BranchCode = "HO",
                BranchName = "SofaCraft Head Office & Factory - Chennai",
                VendorId = VendorCityUtilities,
                VendorCode = "VND-000005",
                VendorName = "Tamil Nadu Electricity Board",
                BillNumber = "APB-2025-0010",
                VendorInvoiceNumber = "CUB/DEC/2024/001",
                VendorInvoiceDate = today.AddDays(-35),
                BillDate = today.AddDays(-34),
                DueDate = today.AddDays(-4),
                PostingDate = today.AddDays(-34),
                CurrencyId = CurrencyINR,
                CurrencyCode = "INR",
                CurrencyName = "Indian Rupee",
                ExchangeRate = 1,
                BillType = BillTypes.Utility,
                BillNarration = "Electricity December 2024",
                IsGSTApplicable = true,
                VendorGSTINSnapshot = "33AABCE1234E1ZE",
                BillStatus = VendorBillStatuses.Posted,
                PostedOn = today.AddDays(-34),
                PostedByUserName = "Finance Manager",
                PaymentTermId = PaymentTermNet30,
                PaymentTermName = "Net 30 Days",
                PaymentTermDays = 30,
                SubTotalAmount = 45000,
                TaxTotalAmount = 8100,
                GrandTotalAmount = 53100,
                PaidAmount = 53100,
                HasInvoiceAttachment = true,
                InvoiceAttachmentCount = 1,
                CreatedAt = today.AddDays(-34),
                CreatedBy = "Finance Manager"
            };
            bill10.Lines = new List<VendorBillLineViewModel>
            {
                CreateLine(bill10.Id, 10, VendorBillLineTypes.Expense, "Electricity Charges - December 2024", 1, 40000, 18, AccountUtilities, "5004", "Utilities Expense"),
                CreateLine(bill10.Id, 20, VendorBillLineTypes.Expense, "Fixed Charges", 1, 5000, 18, AccountUtilities, "5004", "Utilities Expense")
            };
            bill10.RecalculateTotals();
            bills.Add(bill10);


            // Bill 11: Draft - New bill from DataCenter Solutions
            var bill11 = new VendorBillViewModel
            {
                Id = Guid.Parse("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e011"),
                TenantId = TenantId,
                CompanyId = CompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                BranchId = BranchId1,
                BranchCode = "HO",
                BranchName = "SofaCraft Head Office & Factory - Chennai",
                VendorId = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000009"),
                VendorCode = "VND-000009",
                VendorName = "DataCenter Solutions Ltd",
                BillNumber = "APB-2025-0011",
                VendorInvoiceNumber = "DCS/2025/0034",
                VendorInvoiceDate = today.AddDays(-3),
                BillDate = today.AddDays(-2),
                DueDate = today.AddDays(28),
                CurrencyId = CurrencyINR,
                CurrencyCode = "INR",
                CurrencyName = "Indian Rupee",
                ExchangeRate = 1,
                BillType = BillTypes.ServiceExpense,
                BillNarration = "Server hosting and colocation services",
                IsGSTApplicable = true,
                BillStatus = VendorBillStatuses.Draft,
                PaymentTermId = PaymentTermNet30,
                PaymentTermName = "Net 30 Days",
                PaymentTermDays = 30,
                SubTotalAmount = 120000,
                TaxTotalAmount = 21600,
                GrandTotalAmount = 141600,
                PaidAmount = 0,
                CreatedAt = today.AddDays(-2),
                CreatedBy = "AP Clerk"
            };
            bill11.Lines = new List<VendorBillLineViewModel>
            {
                CreateLine(bill11.Id, 10, VendorBillLineTypes.Service, "Server Rack Space - Monthly", 1, 80000, 18, AccountITServices, "5002", "IT Services Expense"),
                CreateLine(bill11.Id, 20, VendorBillLineTypes.Service, "Bandwidth Charges", 1, 40000, 18, AccountITServices, "5002", "IT Services Expense")
            };
            bill11.RecalculateTotals();
            bills.Add(bill11);

            // Bill 12: Posted - Office supplies from Office Essentials (outstanding)
            var bill12 = new VendorBillViewModel
            {
                Id = Guid.Parse("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e012"),
                TenantId = TenantId,
                CompanyId = CompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                BranchId = BranchId2,
                BranchCode = "BLR",
                BranchName = "SofaCraft Experience Store - Bengaluru",
                VendorId = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000010"),
                VendorCode = "VND-000010",
                VendorName = "CloudTech Solutions",
                BillNumber = "APB-2025-0012",
                VendorInvoiceNumber = "OEI/2025/0089",
                VendorInvoiceDate = today.AddDays(-12),
                BillDate = today.AddDays(-10),
                DueDate = today.AddDays(20),
                PostingDate = today.AddDays(-9),
                CurrencyId = CurrencyINR,
                CurrencyCode = "INR",
                CurrencyName = "Indian Rupee",
                ExchangeRate = 1,
                BillType = BillTypes.GoodsPurchase,
                BillNarration = "Office furniture and supplies for new team",
                IsGSTApplicable = true,
                VendorGSTINSnapshot = "33AABCO1234G1ZG",
                BillStatus = VendorBillStatuses.Posted,
                PostedOn = today.AddDays(-9),
                PostedByUserName = "Finance Manager",
                PaymentTermId = PaymentTermNet30,
                PaymentTermName = "Net 30 Days",
                PaymentTermDays = 30,
                SubTotalAmount = 95000,
                TaxTotalAmount = 17100,
                GrandTotalAmount = 112100,
                PaidAmount = 0,
                HasInvoiceAttachment = true,
                InvoiceAttachmentCount = 1,
                CreatedAt = today.AddDays(-10),
                CreatedBy = "AP Clerk"
            };
            bill12.Lines = new List<VendorBillLineViewModel>
            {
                CreateLine(bill12.Id, 10, VendorBillLineTypes.Goods, "Office Desk - Ergonomic", 5, 12000, 18, AccountOfficeSupplies, "5006", "Office Supplies"),
                CreateLine(bill12.Id, 20, VendorBillLineTypes.Goods, "Office Chairs", 5, 7000, 18, AccountOfficeSupplies, "5006", "Office Supplies")
            };
            bill12.RecalculateTotals();
            bills.Add(bill12);

            return bills;
        }

        private static VendorBillLineViewModel CreateLine(Guid billId, int lineNo, string lineType, string description,
            decimal qty, decimal rate, decimal taxRate, Guid accountId, string accountCode, string accountName)
        {
            var line = new VendorBillLineViewModel
            {
                Id = Guid.NewGuid(),
                VendorBillId = billId,
                LineNumber = lineNo,
                LineType = lineType,
                Description = description,
                Quantity = qty,
                UnitRate = rate,
                TaxRatePercentSnapshot = taxRate,
                TaxTypeSnapshot = taxRate > 0 ? TaxTypes.CGST_SGST : TaxTypes.None,
                ExpenseOrAssetAccountId = accountId,
                ExpenseOrAssetAccountCode = accountCode,
                ExpenseOrAssetAccountName = accountName,
                CreatedAt = DateTime.Now,
                CreatedBy = "System"
            };
            line.RecalculateAmounts();
            return line;
        }
    }
}
