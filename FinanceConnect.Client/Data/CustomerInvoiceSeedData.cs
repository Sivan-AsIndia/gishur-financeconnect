using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    /// <summary>
    /// Seed data for CustomerInvoice model (Model #27)
    /// </summary>
    public static class CustomerInvoiceSeedData
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
        private static readonly Guid Customer5Id = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000005");
        private static readonly Guid Customer7Id = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000007");
        private static readonly Guid Customer4Id = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000004");
        private static readonly Guid Customer6Id = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000006");

        // Currency GUIDs (matching MasterDataIds.Currencies)
        private static readonly Guid InrCurrencyId = MasterDataIds.Currencies.INR;
        private static readonly Guid UsdCurrencyId = MasterDataIds.Currencies.USD;

        // Payment Term GUIDs (matching MasterDataIds.PaymentTerms)
        private static readonly Guid Net30TermId = MasterDataIds.PaymentTerms.Net30;
        private static readonly Guid Net45TermId = MasterDataIds.PaymentTerms.Net45;
        private static readonly Guid Net60TermId = MasterDataIds.PaymentTerms.Net60;

        // GL Account GUIDs (matching COADataService accounts)
        private static readonly Guid ReceivableAccountId = MasterDataIds.Accounts.AccountsReceivable;
        private static readonly Guid RevenueAccountId = MasterDataIds.Accounts.SalesRevenue;
        private static readonly Guid ServiceRevenueAccountId = MasterDataIds.Accounts.ServiceRevenue;
        private static readonly Guid TaxAccountId = MasterDataIds.Accounts.GSTPayable;

        // Tax Code GUIDs
        private static readonly Guid GST18TaxCodeId = Guid.Parse("00000000-0000-0000-0000-000000000101");
        private static readonly Guid GST12TaxCodeId = Guid.Parse("00000000-0000-0000-0000-000000000102");
        private static readonly Guid ZeroRatedTaxCodeId = Guid.Parse("00000000-0000-0000-0000-000000000103");

        // State GUIDs for Place of Supply
        private static readonly Guid TamilNaduStateId = Guid.Parse("00000000-0000-0000-0000-000000000033");
        private static readonly Guid KarnatakaStateId = Guid.Parse("00000000-0000-0000-0000-000000000029");
        private static readonly Guid TelanganaStateId = Guid.Parse("00000000-0000-0000-0000-000000000036");

        // Predefined Invoice GUIDs
        public static readonly Guid Invoice1Id = Guid.Parse("a1a1a1a1-0001-0001-0001-000000000001");
        public static readonly Guid Invoice2Id = Guid.Parse("a1a1a1a1-0001-0001-0001-000000000002");
        public static readonly Guid Invoice3Id = Guid.Parse("a1a1a1a1-0001-0001-0001-000000000003");
        public static readonly Guid Invoice4Id = Guid.Parse("a1a1a1a1-0001-0001-0001-000000000004");
        public static readonly Guid Invoice5Id = Guid.Parse("a1a1a1a1-0001-0001-0001-000000000005");
        public static readonly Guid Invoice6Id = Guid.Parse("a1a1a1a1-0001-0001-0001-000000000006");
        public static readonly Guid Invoice7Id = Guid.Parse("a1a1a1a1-0001-0001-0001-000000000007");
        public static readonly Guid Invoice8Id = Guid.Parse("a1a1a1a1-0001-0001-0001-000000000008");
        public static readonly Guid Invoice9Id = Guid.Parse("a1a1a1a1-0001-0001-0001-000000000009");
        public static readonly Guid Invoice10Id = Guid.Parse("a1a1a1a1-0001-0001-0001-000000000010");
        public static readonly Guid Invoice11Id = Guid.Parse("a1a1a1a1-0001-0001-0001-000000000011");
        public static readonly Guid Invoice12Id = Guid.Parse("a1a1a1a1-0001-0001-0001-000000000012");

        public static List<CustomerInvoiceViewModel> GetSeedInvoices()
        {
            var invoices = new List<CustomerInvoiceViewModel>
            {
                // Invoice 1: Posted invoice - fully paid
                new CustomerInvoiceViewModel
                {
                    Id = Invoice1Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftDubai),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftDubai),
                    CustomerId = Customer1Id,
                    CustomerCode = "CUST-001",
                    CustomerName = "ABC Traders Private Limited",
                    InvoiceNumber = "INV-2024-0001",
                    InvoiceType = InvoiceTypes.Standard,
                    InvoiceDate = DateTime.Today.AddDays(-45),
                    PostingDate = DateTime.Today.AddDays(-45),
                    PaymentTermId = Net30TermId,
                    PaymentTermName = "Net 30 Days",
                    PaymentTermDays = 30,
                    DueDate = DateTime.Today.AddDays(-15),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    ReferenceText = "PO-2024-ABC-001",
                    InvoiceNarration = "Software License - Annual Subscription",
                    SubTotalAmount = 50000.00m,
                    DiscountTotalAmount = 2500.00m,
                    TaxTotalAmount = 8550.00m,
                    RoundOffAmount = 0.00m,
                    GrandTotalAmount = 56050.00m,
                    AmountPaidToDate = 56050.00m,
                    InvoiceStatus = InvoiceStatuses.Paid,
                    ApprovalStatus = ApprovalStatuses.Approved,
                    PostedOn = DateTime.Today.AddDays(-45),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedByUserName = "Finance Controller",
                    ReceivableAccountId = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsReceivable),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsReceivable),
                    PlaceOfSupplyStateId = TamilNaduStateId,
                    PlaceOfSupplyStateName = "Tamil Nadu",
                    PlaceOfSupplyStateCode = "33",
                    SupplyType = SupplyTypes.IntraState,
                    EInvoiceStatus = EInvoiceStatuses.Generated,
                    EInvoiceIRN = "IRN202400000001234567890123456789012345678901234567",
                    CreatedAt = DateTime.Today.AddDays(-46),
                    CreatedBy = "AR Clerk"
                },

                // Invoice 2: Posted invoice - partially paid
                new CustomerInvoiceViewModel
                {
                    Id = Invoice2Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftDubai),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftDubai),
                    CustomerId = Customer1Id,
                    CustomerCode = "CUST-001",
                    CustomerName = "ABC Traders Private Limited",
                    InvoiceNumber = "INV-2024-0002",
                    InvoiceType = InvoiceTypes.Standard,
                    InvoiceDate = DateTime.Today.AddDays(-30),
                    PostingDate = DateTime.Today.AddDays(-30),
                    PaymentTermId = Net30TermId,
                    PaymentTermName = "Net 30 Days",
                    PaymentTermDays = 30,
                    DueDate = DateTime.Today,
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    ReferenceText = "PO-2024-ABC-002",
                    InvoiceNarration = "Implementation Services - Phase 1",
                    SubTotalAmount = 150000.00m,
                    DiscountTotalAmount = 0.00m,
                    TaxTotalAmount = 27000.00m,
                    RoundOffAmount = 0.00m,
                    GrandTotalAmount = 177000.00m,
                    AmountPaidToDate = 100000.00m,
                    InvoiceStatus = InvoiceStatuses.PartiallyPaid,
                    ApprovalStatus = ApprovalStatuses.Approved,
                    PostedOn = DateTime.Today.AddDays(-30),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedByUserName = "Finance Controller",
                    ReceivableAccountId = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsReceivable),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsReceivable),
                    PlaceOfSupplyStateId = TamilNaduStateId,
                    PlaceOfSupplyStateName = "Tamil Nadu",
                    PlaceOfSupplyStateCode = "33",
                    SupplyType = SupplyTypes.IntraState,
                    EInvoiceStatus = EInvoiceStatuses.Generated,
                    EInvoiceIRN = "IRN202400000002234567890123456789012345678901234567",
                    CreatedAt = DateTime.Today.AddDays(-31),
                    CreatedBy = "AR Clerk"
                },

                // Invoice 3: Posted invoice - unpaid (overdue)
                new CustomerInvoiceViewModel
                {
                    Id = Invoice3Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = BangaloreBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftDubai),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftDubai),
                    CustomerId = Customer3Id,
                    CustomerCode = "CUST-003",
                    CustomerName = "XYZ Electronics Hub",
                    InvoiceNumber = "INV-2024-0003",
                    InvoiceType = InvoiceTypes.Standard,
                    InvoiceDate = DateTime.Today.AddDays(-60),
                    PostingDate = DateTime.Today.AddDays(-60),
                    PaymentTermId = Net30TermId,
                    PaymentTermName = "Net 30 Days",
                    PaymentTermDays = 30,
                    DueDate = DateTime.Today.AddDays(-30),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    ReferenceText = "PO-2024-XYZ-001",
                    InvoiceNarration = "Hardware Supply - Computer Systems",
                    SubTotalAmount = 185000.00m,
                    DiscountTotalAmount = 9250.00m,
                    TaxTotalAmount = 31635.00m,
                    RoundOffAmount = 0.15m,
                    GrandTotalAmount = 207385.15m,
                    AmountPaidToDate = 0.00m,
                    InvoiceStatus = InvoiceStatuses.Posted,
                    ApprovalStatus = ApprovalStatuses.Approved,
                    PostedOn = DateTime.Today.AddDays(-60),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedByUserName = "Finance Controller",
                    ReceivableAccountId = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsReceivable),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsReceivable),
                    PlaceOfSupplyStateId = KarnatakaStateId,
                    PlaceOfSupplyStateName = "Karnataka",
                    PlaceOfSupplyStateCode = "29",
                    SupplyType = SupplyTypes.InterState,
                    EInvoiceStatus = EInvoiceStatuses.Generated,
                    EInvoiceIRN = "IRN202400000003234567890123456789012345678901234567",
                    CreatedAt = DateTime.Today.AddDays(-61),
                    CreatedBy = "AR Clerk"
                },

                // Invoice 4: Draft invoice
                new CustomerInvoiceViewModel
                {
                    Id = Invoice4Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftDubai),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftDubai),
                    CustomerId = Customer2Id,
                    CustomerCode = "CUST-002",
                    CustomerName = "State Government Education Dept",
                    InvoiceNumber = "INV-2024-0004",
                    InvoiceType = InvoiceTypes.Standard,
                    InvoiceDate = DateTime.Today,
                    PaymentTermId = Net60TermId,
                    PaymentTermName = "Net 60 Days",
                    PaymentTermDays = 60,
                    DueDate = DateTime.Today.AddDays(60),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    ReferenceText = "GOV-TENDER-2024-EDU-001",
                    InvoiceNarration = "Educational Software License - 500 Users",
                    SubTotalAmount = 250000.00m,
                    DiscountTotalAmount = 25000.00m,
                    TaxTotalAmount = 40500.00m,
                    RoundOffAmount = 0.00m,
                    GrandTotalAmount = 265500.00m,
                    AmountPaidToDate = 0.00m,
                    InvoiceStatus = InvoiceStatuses.Draft,
                    ApprovalStatus = ApprovalStatuses.NotRequired,
                    ReceivableAccountId = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsReceivable),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsReceivable),
                    PlaceOfSupplyStateId = TamilNaduStateId,
                    PlaceOfSupplyStateName = "Tamil Nadu",
                    PlaceOfSupplyStateCode = "33",
                    SupplyType = SupplyTypes.IntraState,
                    EInvoiceStatus = EInvoiceStatuses.NotApplicable,
                    CreatedAt = DateTime.Today,
                    CreatedBy = "AR Clerk"
                },

                // Invoice 5: SEZ Invoice - Posted
                new CustomerInvoiceViewModel
                {
                    Id = Invoice5Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = HyderabadBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftDubai),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftDubai),
                    CustomerId = Customer5Id,
                    CustomerCode = "CUST-005",
                    CustomerName = "TechPark SEZ Solutions Pvt Ltd",
                    InvoiceNumber = "INV-2024-0005",
                    InvoiceType = InvoiceTypes.SEZ,
                    InvoiceDate = DateTime.Today.AddDays(-15),
                    PostingDate = DateTime.Today.AddDays(-15),
                    PaymentTermId = Net45TermId,
                    PaymentTermName = "Net 45 Days",
                    PaymentTermDays = 45,
                    DueDate = DateTime.Today.AddDays(30),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    ReferenceText = "SEZ-PO-2024-001",
                    InvoiceNarration = "Cloud Infrastructure Services - SEZ Unit",
                    SubTotalAmount = 500000.00m,
                    DiscountTotalAmount = 0.00m,
                    TaxTotalAmount = 0.00m,
                    RoundOffAmount = 0.00m,
                    GrandTotalAmount = 500000.00m,
                    AmountPaidToDate = 0.00m,
                    InvoiceStatus = InvoiceStatuses.Posted,
                    ApprovalStatus = ApprovalStatuses.Approved,
                    PostedOn = DateTime.Today.AddDays(-15),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedByUserName = "Finance Controller",
                    ReceivableAccountId = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsReceivable),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsReceivable),
                    PlaceOfSupplyStateId = TelanganaStateId,
                    PlaceOfSupplyStateName = "Telangana",
                    PlaceOfSupplyStateCode = "36",
                    SupplyType = SupplyTypes.SEZ,
                    EInvoiceStatus = EInvoiceStatuses.Generated,
                    EInvoiceIRN = "IRN202400000005234567890123456789012345678901234567",
                    CreatedAt = DateTime.Today.AddDays(-16),
                    CreatedBy = "AR Clerk"
                },

                // Invoice 6: Proforma Invoice
                new CustomerInvoiceViewModel
                {
                    Id = Invoice6Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftDubai),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftDubai),
                    CustomerId = Customer1Id,
                    CustomerCode = "CUST-001",
                    CustomerName = "ABC Traders Private Limited",
                    InvoiceNumber = "PI-2024-0001",
                    InvoiceType = InvoiceTypes.Proforma,
                    InvoiceDate = DateTime.Today.AddDays(-5),
                    PaymentTermId = Net30TermId,
                    PaymentTermName = "Net 30 Days",
                    PaymentTermDays = 30,
                    DueDate = DateTime.Today.AddDays(25),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    ReferenceText = "QUOTE-2024-ABC-003",
                    InvoiceNarration = "Proforma for Upcoming Project Phase 2",
                    SubTotalAmount = 300000.00m,
                    DiscountTotalAmount = 15000.00m,
                    TaxTotalAmount = 51300.00m,
                    RoundOffAmount = 0.00m,
                    GrandTotalAmount = 336300.00m,
                    AmountPaidToDate = 0.00m,
                    InvoiceStatus = InvoiceStatuses.Draft,
                    ApprovalStatus = ApprovalStatuses.NotRequired,
                    ReceivableAccountId = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsReceivable),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsReceivable),
                    PlaceOfSupplyStateId = TamilNaduStateId,
                    PlaceOfSupplyStateName = "Tamil Nadu",
                    PlaceOfSupplyStateCode = "33",
                    SupplyType = SupplyTypes.IntraState,
                    EInvoiceStatus = EInvoiceStatuses.NotApplicable,
                    CreatedAt = DateTime.Today.AddDays(-5),
                    CreatedBy = "Sales Team"
                },

                // Invoice 7: Export Invoice (USD)
                new CustomerInvoiceViewModel
                {
                    Id = Invoice7Id,
                    CompanyId = SofaCraftUSACompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftDubai),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftDubai),
                    CustomerId = Customer7Id,
                    CustomerCode = "CUST-007",
                    CustomerName = "Silicon Valley Partners LLC",
                    InvoiceNumber = "EXP-2024-0001",
                    InvoiceType = InvoiceTypes.Export,
                    InvoiceDate = DateTime.Today.AddDays(-20),
                    PostingDate = DateTime.Today.AddDays(-20),
                    PaymentTermId = Net30TermId,
                    PaymentTermName = "Net 30 Days",
                    PaymentTermDays = 30,
                    DueDate = DateTime.Today.AddDays(10),
                    CurrencyId = UsdCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.USD),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.USD),
                    ExchangeRate = 83.25m,
                    ReferenceText = "SVP-CONTRACT-2024-001",
                    InvoiceNarration = "Software Development Services - Q1 2024",
                    SubTotalAmount = 25000.00m,
                    DiscountTotalAmount = 0.00m,
                    TaxTotalAmount = 0.00m,
                    RoundOffAmount = 0.00m,
                    GrandTotalAmount = 25000.00m,
                    AmountPaidToDate = 0.00m,
                    InvoiceStatus = InvoiceStatuses.Posted,
                    ApprovalStatus = ApprovalStatuses.Approved,
                    PostedOn = DateTime.Today.AddDays(-20),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedByUserName = "Finance Controller",
                    ReceivableAccountId = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsReceivable),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsReceivable),
                    SupplyType = SupplyTypes.Export,
                    EInvoiceStatus = EInvoiceStatuses.NotApplicable,
                    CreatedAt = DateTime.Today.AddDays(-21),
                    CreatedBy = "Export Team"
                },

                // Invoice 8: Cancelled Invoice
                new CustomerInvoiceViewModel
                {
                    Id = Invoice8Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftDubai),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftDubai),
                    CustomerId = Customer1Id,
                    CustomerCode = "CUST-001",
                    CustomerName = "ABC Traders Private Limited",
                    InvoiceNumber = "INV-2024-0006",
                    InvoiceType = InvoiceTypes.Standard,
                    InvoiceDate = DateTime.Today.AddDays(-10),
                    PaymentTermId = Net30TermId,
                    PaymentTermName = "Net 30 Days",
                    PaymentTermDays = 30,
                    DueDate = DateTime.Today.AddDays(20),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    ReferenceText = "PO-2024-ABC-ERR",
                    InvoiceNarration = "Invoice created in error - cancelled",
                    SubTotalAmount = 75000.00m,
                    DiscountTotalAmount = 0.00m,
                    TaxTotalAmount = 13500.00m,
                    RoundOffAmount = 0.00m,
                    GrandTotalAmount = 88500.00m,
                    AmountPaidToDate = 0.00m,
                    InvoiceStatus = InvoiceStatuses.Cancelled,
                    ApprovalStatus = ApprovalStatuses.NotRequired,
                    CancelledOn = DateTime.Today.AddDays(-9),
                    CancelledByUserId = MasterDataIds.Tenants.Default,
                    CancelledByUserName = "Finance Controller",
                    CancellationReason = "Invoice created with incorrect customer details. New invoice INV-2024-0007 to be raised.",
                    ReceivableAccountId = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsReceivable),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsReceivable),
                    PlaceOfSupplyStateId = TamilNaduStateId,
                    PlaceOfSupplyStateName = "Tamil Nadu",
                    PlaceOfSupplyStateCode = "33",
                    SupplyType = SupplyTypes.IntraState,
                    EInvoiceStatus = EInvoiceStatuses.NotApplicable,
                    CreatedAt = DateTime.Today.AddDays(-10),
                    CreatedBy = "AR Clerk"
                },

                // Invoice 9: Posted - Partially paid
                new CustomerInvoiceViewModel
                {
                    Id = Invoice9Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftDubai),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftDubai),
                    CustomerId = Customer4Id,
                    CustomerCode = "CUST-004",
                    CustomerName = "Rajesh Kumar",
                    InvoiceNumber = "INV-2024-0007",
                    InvoiceType = InvoiceTypes.Standard,
                    InvoiceDate = DateTime.Today.AddDays(-20),
                    PaymentTermId = Net30TermId,
                    PaymentTermName = "Net 30 Days",
                    PaymentTermDays = 30,
                    DueDate = DateTime.Today.AddDays(10),
                    PostingDate = DateTime.Today.AddDays(-20),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    InvoiceNarration = "Software development services - Phase 2",
                    SubTotalAmount = 280000.00m,
                    DiscountTotalAmount = 0.00m,
                    TaxTotalAmount = 50400.00m,
                    RoundOffAmount = 0.00m,
                    GrandTotalAmount = 330400.00m,
                    AmountPaidToDate = 150000.00m,
                    InvoiceStatus = InvoiceStatuses.PartiallyPaid,
                    ApprovalStatus = ApprovalStatuses.Approved,
                    PostedOn = DateTime.Today.AddDays(-20),
                    PostedByUserName = "Finance Controller",
                    ReceivableAccountId = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsReceivable),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsReceivable),
                    PlaceOfSupplyStateId = TamilNaduStateId,
                    PlaceOfSupplyStateName = "Tamil Nadu",
                    PlaceOfSupplyStateCode = "33",
                    SupplyType = SupplyTypes.IntraState,
                    EInvoiceStatus = EInvoiceStatuses.Generated,
                    CreatedAt = DateTime.Today.AddDays(-20),
                    CreatedBy = "Finance Manager"
                },

                // Invoice 10: Draft
                new CustomerInvoiceViewModel
                {
                    Id = Invoice10Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = BangaloreBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftDubai),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftDubai),
                    CustomerId = Customer6Id,
                    CustomerCode = "CUST-006",
                    CustomerName = "Discontinued Enterprises",
                    InvoiceNumber = "INV-2024-0008",
                    InvoiceType = InvoiceTypes.Standard,
                    InvoiceDate = DateTime.Today.AddDays(-3),
                    PaymentTermId = Net30TermId,
                    PaymentTermName = "Net 30 Days",
                    PaymentTermDays = 30,
                    DueDate = DateTime.Today.AddDays(27),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    InvoiceNarration = "Cloud migration consulting services",
                    SubTotalAmount = 175000.00m,
                    DiscountTotalAmount = 0.00m,
                    TaxTotalAmount = 31500.00m,
                    RoundOffAmount = 0.00m,
                    GrandTotalAmount = 206500.00m,
                    AmountPaidToDate = 0.00m,
                    InvoiceStatus = InvoiceStatuses.Draft,
                    ApprovalStatus = ApprovalStatuses.NotRequired,
                    ReceivableAccountId = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsReceivable),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsReceivable),
                    PlaceOfSupplyStateId = KarnatakaStateId,
                    PlaceOfSupplyStateName = "Karnataka",
                    PlaceOfSupplyStateCode = "29",
                    SupplyType = SupplyTypes.IntraState,
                    EInvoiceStatus = EInvoiceStatuses.NotApplicable,
                    CreatedAt = DateTime.Today.AddDays(-3),
                    CreatedBy = "Sales Manager"
                },

                // Invoice 11: Posted - Overdue
                new CustomerInvoiceViewModel
                {
                    Id = Invoice11Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftDubai),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftDubai),
                    CustomerId = Customer2Id,
                    CustomerCode = "CUST-002",
                    CustomerName = "State Government Education Dept",
                    InvoiceNumber = "INV-2024-0009",
                    InvoiceType = InvoiceTypes.Standard,
                    InvoiceDate = DateTime.Today.AddDays(-65),
                    PaymentTermId = Net30TermId,
                    PaymentTermName = "Net 30 Days",
                    PaymentTermDays = 30,
                    DueDate = DateTime.Today.AddDays(-35),
                    PostingDate = DateTime.Today.AddDays(-65),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    InvoiceNarration = "ERP customization and training",
                    SubTotalAmount = 450000.00m,
                    DiscountTotalAmount = 0.00m,
                    TaxTotalAmount = 81000.00m,
                    RoundOffAmount = 0.00m,
                    GrandTotalAmount = 531000.00m,
                    AmountPaidToDate = 200000.00m,
                    InvoiceStatus = InvoiceStatuses.PartiallyPaid,
                    ApprovalStatus = ApprovalStatuses.Approved,
                    PostedOn = DateTime.Today.AddDays(-65),
                    PostedByUserName = "Admin User",
                    ReceivableAccountId = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsReceivable),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsReceivable),
                    PlaceOfSupplyStateId = TamilNaduStateId,
                    PlaceOfSupplyStateName = "Tamil Nadu",
                    PlaceOfSupplyStateCode = "33",
                    SupplyType = SupplyTypes.IntraState,
                    EInvoiceStatus = EInvoiceStatuses.Generated,
                    CreatedAt = DateTime.Today.AddDays(-65),
                    CreatedBy = "Admin User"
                },

                // Invoice 12: Cancelled
                new CustomerInvoiceViewModel
                {
                    Id = Invoice12Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftDubai),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftDubai),
                    CustomerId = Customer1Id,
                    CustomerCode = "CUST-001",
                    CustomerName = "ABC Traders Private Limited",
                    InvoiceNumber = "INV-2024-0010",
                    InvoiceType = InvoiceTypes.Standard,
                    InvoiceDate = DateTime.Today.AddDays(-15),
                    PaymentTermId = Net30TermId,
                    PaymentTermName = "Net 30 Days",
                    PaymentTermDays = 30,
                    DueDate = DateTime.Today.AddDays(15),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    InvoiceNarration = "Cancelled - Duplicate invoice",
                    SubTotalAmount = 95000.00m,
                    DiscountTotalAmount = 0.00m,
                    TaxTotalAmount = 17100.00m,
                    RoundOffAmount = 0.00m,
                    GrandTotalAmount = 112100.00m,
                    AmountPaidToDate = 0.00m,
                    InvoiceStatus = InvoiceStatuses.Cancelled,
                    ApprovalStatus = ApprovalStatuses.NotRequired,
                    CancelledOn = DateTime.Today.AddDays(-14),
                    CancelledByUserId = MasterDataIds.Tenants.Default,
                    CancelledByUserName = "Finance Controller",
                    CancellationReason = "Duplicate invoice entry",
                    ReceivableAccountId = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsReceivable),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsReceivable),
                    PlaceOfSupplyStateId = TamilNaduStateId,
                    PlaceOfSupplyStateName = "Tamil Nadu",
                    PlaceOfSupplyStateCode = "33",
                    SupplyType = SupplyTypes.IntraState,
                    EInvoiceStatus = EInvoiceStatuses.NotApplicable,
                    CreatedAt = DateTime.Today.AddDays(-15),
                    CreatedBy = "AR Clerk"
                }
            };

            // Add lines to each invoice
            AddLinesToInvoice1(invoices[0]);
            AddLinesToInvoice2(invoices[1]);
            AddLinesToInvoice3(invoices[2]);
            AddLinesToInvoice4(invoices[3]);
            AddLinesToInvoice5(invoices[4]);
            AddLinesToInvoice6(invoices[5]);
            AddLinesToInvoice7(invoices[6]);
            AddLinesToInvoice8(invoices[7]);

            return invoices;
        }

        private static void AddLinesToInvoice1(CustomerInvoiceViewModel invoice)
        {
            invoice.Lines = new List<CustomerInvoiceLineViewModel>
            {
                new CustomerInvoiceLineViewModel
                {
                    Id = Guid.NewGuid(),
                    CustomerInvoiceId = invoice.Id,
                    LineNumber = 10,
                    LineType = LineTypes.Service,
                    Description = "Enterprise Software License - Annual",
                    Quantity = 1,
                    UnitPrice = 50000.00m,
                    DiscountPercent = 5,
                    DiscountAmount = 2500.00m,
                    TaxCodeId = GST18TaxCodeId,
                    TaxCodeCode = "GST18",
                    TaxCodeName = "GST 18%",
                    TaxRatePercent = 18,
                    TaxAmount = 8550.00m,
                    RevenueAccountId = ServiceRevenueAccountId,
                    RevenueAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsReceivable),
                    RevenueAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsReceivable),
                    HSNCode = "998314",
                    CreatedAt = invoice.CreatedAt,
                    CreatedBy = invoice.CreatedBy
                }
            };
        }

        private static void AddLinesToInvoice2(CustomerInvoiceViewModel invoice)
        {
            invoice.Lines = new List<CustomerInvoiceLineViewModel>
            {
                new CustomerInvoiceLineViewModel
                {
                    Id = Guid.NewGuid(),
                    CustomerInvoiceId = invoice.Id,
                    LineNumber = 10,
                    LineType = LineTypes.Service,
                    Description = "Implementation Services - Configuration",
                    Quantity = 80,
                    UnitPrice = 1000.00m,
                    DiscountPercent = 0,
                    DiscountAmount = 0.00m,
                    TaxCodeId = GST18TaxCodeId,
                    TaxCodeCode = "GST18",
                    TaxCodeName = "GST 18%",
                    TaxRatePercent = 18,
                    TaxAmount = 14400.00m,
                    RevenueAccountId = ServiceRevenueAccountId,
                    RevenueAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsReceivable),
                    RevenueAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsReceivable),
                    SACCode = "998314",
                    CreatedAt = invoice.CreatedAt,
                    CreatedBy = invoice.CreatedBy
                },
                new CustomerInvoiceLineViewModel
                {
                    Id = Guid.NewGuid(),
                    CustomerInvoiceId = invoice.Id,
                    LineNumber = 20,
                    LineType = LineTypes.Service,
                    Description = "Training Services - On-site",
                    Quantity = 5,
                    UnitPrice = 14000.00m,
                    DiscountPercent = 0,
                    DiscountAmount = 0.00m,
                    TaxCodeId = GST18TaxCodeId,
                    TaxCodeCode = "GST18",
                    TaxCodeName = "GST 18%",
                    TaxRatePercent = 18,
                    TaxAmount = 12600.00m,
                    RevenueAccountId = ServiceRevenueAccountId,
                    RevenueAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsReceivable),
                    RevenueAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsReceivable),
                    SACCode = "998393",
                    CreatedAt = invoice.CreatedAt,
                    CreatedBy = invoice.CreatedBy
                }
            };
        }

        private static void AddLinesToInvoice3(CustomerInvoiceViewModel invoice)
        {
            invoice.Lines = new List<CustomerInvoiceLineViewModel>
            {
                new CustomerInvoiceLineViewModel
                {
                    Id = Guid.NewGuid(),
                    CustomerInvoiceId = invoice.Id,
                    LineNumber = 10,
                    LineType = LineTypes.Item,
                    Description = "Desktop Computer - High Performance",
                    Quantity = 10,
                    UnitPrice = 12000.00m,
                    DiscountPercent = 5,
                    DiscountAmount = 6000.00m,
                    TaxCodeId = GST18TaxCodeId,
                    TaxCodeCode = "GST18",
                    TaxCodeName = "GST 18%",
                    TaxRatePercent = 18,
                    TaxAmount = 20520.00m,
                    RevenueAccountId = RevenueAccountId,
                    RevenueAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsReceivable),
                    RevenueAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsReceivable),
                    HSNCode = "847130",
                    CreatedAt = invoice.CreatedAt,
                    CreatedBy = invoice.CreatedBy
                },
                new CustomerInvoiceLineViewModel
                {
                    Id = Guid.NewGuid(),
                    CustomerInvoiceId = invoice.Id,
                    LineNumber = 20,
                    LineType = LineTypes.Item,
                    Description = "LED Monitor 24 inch",
                    Quantity = 10,
                    UnitPrice = 6500.00m,
                    DiscountPercent = 5,
                    DiscountAmount = 3250.00m,
                    TaxCodeId = GST18TaxCodeId,
                    TaxCodeCode = "GST18",
                    TaxCodeName = "GST 18%",
                    TaxRatePercent = 18,
                    TaxAmount = 11115.00m,
                    RevenueAccountId = RevenueAccountId,
                    RevenueAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsReceivable),
                    RevenueAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsReceivable),
                    HSNCode = "852872",
                    CreatedAt = invoice.CreatedAt,
                    CreatedBy = invoice.CreatedBy
                }
            };
        }

        private static void AddLinesToInvoice4(CustomerInvoiceViewModel invoice)
        {
            invoice.Lines = new List<CustomerInvoiceLineViewModel>
            {
                new CustomerInvoiceLineViewModel
                {
                    Id = Guid.NewGuid(),
                    CustomerInvoiceId = invoice.Id,
                    LineNumber = 10,
                    LineType = LineTypes.Service,
                    Description = "Educational Software License - 500 Users",
                    Quantity = 500,
                    UnitPrice = 500.00m,
                    DiscountPercent = 10,
                    DiscountAmount = 25000.00m,
                    TaxCodeId = GST18TaxCodeId,
                    TaxCodeCode = "GST18",
                    TaxCodeName = "GST 18%",
                    TaxRatePercent = 18,
                    TaxAmount = 40500.00m,
                    RevenueAccountId = ServiceRevenueAccountId,
                    RevenueAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsReceivable),
                    RevenueAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsReceivable),
                    SACCode = "998314",
                    CreatedAt = invoice.CreatedAt,
                    CreatedBy = invoice.CreatedBy
                }
            };
        }

        private static void AddLinesToInvoice5(CustomerInvoiceViewModel invoice)
        {
            invoice.Lines = new List<CustomerInvoiceLineViewModel>
            {
                new CustomerInvoiceLineViewModel
                {
                    Id = Guid.NewGuid(),
                    CustomerInvoiceId = invoice.Id,
                    LineNumber = 10,
                    LineType = LineTypes.Service,
                    Description = "Cloud Infrastructure - Monthly Subscription",
                    Quantity = 12,
                    UnitPrice = 25000.00m,
                    DiscountPercent = 0,
                    DiscountAmount = 0.00m,
                    TaxCodeId = ZeroRatedTaxCodeId,
                    TaxCodeCode = "ZERO",
                    TaxCodeName = "Zero Rated",
                    TaxRatePercent = 0,
                    TaxAmount = 0.00m,
                    RevenueAccountId = ServiceRevenueAccountId,
                    RevenueAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsReceivable),
                    RevenueAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsReceivable),
                    SACCode = "998315",
                    CreatedAt = invoice.CreatedAt,
                    CreatedBy = invoice.CreatedBy
                },
                new CustomerInvoiceLineViewModel
                {
                    Id = Guid.NewGuid(),
                    CustomerInvoiceId = invoice.Id,
                    LineNumber = 20,
                    LineType = LineTypes.Service,
                    Description = "Technical Support - Premium",
                    Quantity = 12,
                    UnitPrice = 16666.67m,
                    DiscountPercent = 0,
                    DiscountAmount = 0.00m,
                    TaxCodeId = ZeroRatedTaxCodeId,
                    TaxCodeCode = "ZERO",
                    TaxCodeName = "Zero Rated",
                    TaxRatePercent = 0,
                    TaxAmount = 0.00m,
                    RevenueAccountId = ServiceRevenueAccountId,
                    RevenueAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsReceivable),
                    RevenueAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsReceivable),
                    SACCode = "998316",
                    CreatedAt = invoice.CreatedAt,
                    CreatedBy = invoice.CreatedBy
                }
            };
        }

        private static void AddLinesToInvoice6(CustomerInvoiceViewModel invoice)
        {
            invoice.Lines = new List<CustomerInvoiceLineViewModel>
            {
                new CustomerInvoiceLineViewModel
                {
                    Id = Guid.NewGuid(),
                    CustomerInvoiceId = invoice.Id,
                    LineNumber = 10,
                    LineType = LineTypes.Service,
                    Description = "Project Phase 2 - Development",
                    Quantity = 200,
                    UnitPrice = 1500.00m,
                    DiscountPercent = 5,
                    DiscountAmount = 15000.00m,
                    TaxCodeId = GST18TaxCodeId,
                    TaxCodeCode = "GST18",
                    TaxCodeName = "GST 18%",
                    TaxRatePercent = 18,
                    TaxAmount = 51300.00m,
                    RevenueAccountId = ServiceRevenueAccountId,
                    RevenueAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsReceivable),
                    RevenueAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsReceivable),
                    SACCode = "998314",
                    CreatedAt = invoice.CreatedAt,
                    CreatedBy = invoice.CreatedBy
                }
            };
        }

        private static void AddLinesToInvoice7(CustomerInvoiceViewModel invoice)
        {
            invoice.Lines = new List<CustomerInvoiceLineViewModel>
            {
                new CustomerInvoiceLineViewModel
                {
                    Id = Guid.NewGuid(),
                    CustomerInvoiceId = invoice.Id,
                    LineNumber = 10,
                    LineType = LineTypes.Service,
                    Description = "Software Development Services - Q1 2024",
                    Quantity = 500,
                    UnitPrice = 50.00m,
                    DiscountPercent = 0,
                    DiscountAmount = 0.00m,
                    TaxCodeId = ZeroRatedTaxCodeId,
                    TaxCodeCode = "ZERO",
                    TaxCodeName = "Zero Rated (Export)",
                    TaxRatePercent = 0,
                    TaxAmount = 0.00m,
                    RevenueAccountId = ServiceRevenueAccountId,
                    RevenueAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsReceivable),
                    RevenueAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsReceivable),
                    SACCode = "998314",
                    CreatedAt = invoice.CreatedAt,
                    CreatedBy = invoice.CreatedBy
                }
            };
        }

        private static void AddLinesToInvoice8(CustomerInvoiceViewModel invoice)
        {
            invoice.Lines = new List<CustomerInvoiceLineViewModel>
            {
                new CustomerInvoiceLineViewModel
                {
                    Id = Guid.NewGuid(),
                    CustomerInvoiceId = invoice.Id,
                    LineNumber = 10,
                    LineType = LineTypes.Service,
                    Description = "Consulting Services - Error Entry",
                    Quantity = 50,
                    UnitPrice = 1500.00m,
                    DiscountPercent = 0,
                    DiscountAmount = 0.00m,
                    TaxCodeId = GST18TaxCodeId,
                    TaxCodeCode = "GST18",
                    TaxCodeName = "GST 18%",
                    TaxRatePercent = 18,
                    TaxAmount = 13500.00m,
                    RevenueAccountId = ServiceRevenueAccountId,
                    RevenueAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsReceivable),
                    RevenueAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsReceivable),
                    SACCode = "998314",
                    CreatedAt = invoice.CreatedAt,
                    CreatedBy = invoice.CreatedBy
                }
            };
        }
    }
}
