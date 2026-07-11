using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    /// <summary>
    /// Seed data for CustomerDebitNote model (Model #31)
    /// </summary>
    public static class CustomerDebitNoteSeedData
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
        private static readonly Guid Customer4Id = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000004");
        private static readonly Guid Customer6Id = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000006");
        private static readonly Guid Customer7Id = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000007");

        // Currency GUIDs (matching MasterDataIds.Currencies)
        private static readonly Guid InrCurrencyId = MasterDataIds.Currencies.INR;

        // GL Account GUIDs (matching COADataService accounts)
        private static readonly Guid ReceivableAccountId = MasterDataIds.Accounts.AccountsReceivable;
        private static readonly Guid RevenueAccountId = MasterDataIds.Accounts.SalesRevenue;
        private static readonly Guid ServiceRevenueAccountId = MasterDataIds.Accounts.ServiceRevenue;
        private static readonly Guid OtherIncomeAccountId = MasterDataIds.Accounts.UtilitiesExpense;
        private static readonly Guid TaxAccountId = MasterDataIds.Accounts.GSTPayable;

        // Tax Code GUIDs
        private static readonly Guid GST18TaxCodeId = Guid.Parse("00000000-0000-0000-0000-000000000101");
        private static readonly Guid GST12TaxCodeId = Guid.Parse("00000000-0000-0000-0000-000000000102");
        private static readonly Guid ZeroRatedTaxCodeId = Guid.Parse("00000000-0000-0000-0000-000000000103");

        // Invoice GUIDs (matching CustomerInvoiceSeedData)
        private static readonly Guid Invoice1Id = Guid.Parse("a1a1a1a1-0001-0001-0001-000000000001");
        private static readonly Guid Invoice2Id = Guid.Parse("a1a1a1a1-0001-0001-0001-000000000002");
        private static readonly Guid Invoice3Id = Guid.Parse("a1a1a1a1-0001-0001-0001-000000000003");

        // Predefined Debit Note GUIDs
        public static readonly Guid DebitNote1Id = Guid.Parse("d2d2d2d2-0001-0001-0001-000000000001");
        public static readonly Guid DebitNote2Id = Guid.Parse("d2d2d2d2-0001-0001-0001-000000000002");
        public static readonly Guid DebitNote3Id = Guid.Parse("d2d2d2d2-0001-0001-0001-000000000003");
        public static readonly Guid DebitNote4Id = Guid.Parse("d2d2d2d2-0001-0001-0001-000000000004");
        public static readonly Guid DebitNote5Id = Guid.Parse("d2d2d2d2-0001-0001-0001-000000000005");
        public static readonly Guid DebitNote6Id = Guid.Parse("d2d2d2d2-0001-0001-0001-000000000006");
        public static readonly Guid DebitNote7Id = Guid.Parse("d2d2d2d2-0001-0001-0001-000000000007");
        public static readonly Guid DebitNote8Id = Guid.Parse("d2d2d2d2-0001-0001-0001-000000000008");
        public static readonly Guid DebitNote9Id = Guid.Parse("d2d2d2d2-0001-0001-0001-000000000009");
        public static readonly Guid DebitNote10Id = Guid.Parse("d2d2d2d2-0001-0001-0001-000000000010");
        public static readonly Guid DebitNote11Id = Guid.Parse("d2d2d2d2-0001-0001-0001-000000000011");
        public static readonly Guid DebitNote12Id = Guid.Parse("d2d2d2d2-0001-0001-0001-000000000012");

        public static List<CustomerDebitNoteViewModel> GetSeedDebitNotes()
        {
            var debitNotes = new List<CustomerDebitNoteViewModel>
            {
                // Debit Note 1: Posted - Underbilling correction against invoice
                new CustomerDebitNoteViewModel
                {
                    Id = DebitNote1Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftDubai),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftDubai),
                    CustomerId = Customer1Id,
                    CustomerCode = "CUST-001",
                    CustomerName = "ABC Traders Private Limited",
                    DebitNoteNumber = "DN-2024-0001",
                    DebitNoteDate = DateTime.Today.AddDays(-30),
                    PostingDate = DateTime.Today.AddDays(-30),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    ReferenceText = "Ref: INV-2024-0001",
                    DebitNoteNarration = "Additional service charges missed in original invoice",
                    IsAgainstInvoice = true,
                    CustomerInvoiceId = Invoice1Id,
                    CustomerInvoiceNumber = "INV-2024-0001",
                    InvoiceNumberSnapshot = "INV-2024-0001",
                    InvoiceDateSnapshot = DateTime.Today.AddDays(-45),
                    DebitReasonCode = DebitReasonCodes.UnderbillingCorrection,
                    DebitReasonDescription = "Underbilling Correction - Missed service line",
                    IsTaxImpacting = true,
                    IsRevenueRecognized = true,
                    SubTotalAmount = 15000.00m,
                    DiscountTotalAmount = 0.00m,
                    TaxTotalAmount = 2700.00m,
                    RoundOffAmount = 0.00m,
                    GrandTotalAmount = 17700.00m,
                    AppliedToInvoiceAmount = 17700.00m,
                    ReceivableAccountIdSnapshot = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.GSTPayable),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.GSTPayable),
                    RevenueAccountId = ServiceRevenueAccountId,
                    RevenueAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.GSTPayable),
                    RevenueAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.GSTPayable),
                    TaxAccountIdSnapshot = TaxAccountId,
                    TaxAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.GSTPayable),
                    TaxAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.GSTPayable),
                    IsSystemGenerated = false,
                    DebitNoteStatus = DebitNoteStatuses.Posted,
                    PostedOn = DateTime.Today.AddDays(-30),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedByUserName = "Finance Controller",
                    CreatedAt = DateTime.Today.AddDays(-31),
                    CreatedBy = "AR Clerk"
                },

                // Debit Note 2: Posted - Late fee (standalone, system generated)
                new CustomerDebitNoteViewModel
                {
                    Id = DebitNote2Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftDubai),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftDubai),
                    CustomerId = Customer1Id,
                    CustomerCode = "CUST-001",
                    CustomerName = "ABC Traders Private Limited",
                    DebitNoteNumber = "DN-2024-0002",
                    DebitNoteDate = DateTime.Today.AddDays(-20),
                    PostingDate = DateTime.Today.AddDays(-20),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    ReferenceText = "Auto-generated late fee",
                    DebitNoteNarration = "Late payment penalty - Invoice overdue 60+ days",
                    IsAgainstInvoice = false,
                    CustomerInvoiceId = null,
                    CustomerInvoiceNumber = null,
                    InvoiceNumberSnapshot = null,
                    InvoiceDateSnapshot = null,
                    DebitReasonCode = DebitReasonCodes.LateFee,
                    DebitReasonDescription = "Late Fee / Penalty - Overdue payment",
                    IsTaxImpacting = true,
                    IsRevenueRecognized = false,
                    SubTotalAmount = 5000.00m,
                    DiscountTotalAmount = 0.00m,
                    TaxTotalAmount = 900.00m,
                    RoundOffAmount = 0.00m,
                    GrandTotalAmount = 5900.00m,
                    AppliedToInvoiceAmount = 0.00m,
                    ReceivableAccountIdSnapshot = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.GSTPayable),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.GSTPayable),
                    RevenueAccountId = OtherIncomeAccountId,
                    RevenueAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.GSTPayable),
                    RevenueAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.GSTPayable),
                    TaxAccountIdSnapshot = TaxAccountId,
                    TaxAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.GSTPayable),
                    TaxAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.GSTPayable),
                    IsSystemGenerated = true,
                    DebitNoteStatus = DebitNoteStatuses.Posted,
                    PostedOn = DateTime.Today.AddDays(-20),
                    PostedByUserId = MasterDataIds.PaymentTerms.Net45,
                    PostedByUserName = "System",
                    CreatedAt = DateTime.Today.AddDays(-20),
                    CreatedBy = "System"
                },

                // Debit Note 3: Posted - Tax short charged correction
                new CustomerDebitNoteViewModel
                {
                    Id = DebitNote3Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = BangaloreBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftDubai),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftDubai),
                    CustomerId = Customer2Id,
                    CustomerCode = "CUST-002",
                    CustomerName = "State Government Education Dept",
                    DebitNoteNumber = "DN-2024-0003",
                    DebitNoteDate = DateTime.Today.AddDays(-15),
                    PostingDate = DateTime.Today.AddDays(-15),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    ReferenceText = "Tax correction: INV-2024-0002",
                    DebitNoteNarration = "Tax was charged at 12% instead of 18%. Additional 6% tax correction.",
                    IsAgainstInvoice = true,
                    CustomerInvoiceId = Invoice2Id,
                    CustomerInvoiceNumber = "INV-2024-0002",
                    InvoiceNumberSnapshot = "INV-2024-0002",
                    InvoiceDateSnapshot = DateTime.Today.AddDays(-30),
                    DebitReasonCode = DebitReasonCodes.TaxShortCharged,
                    DebitReasonDescription = "Tax Short Charged - 6% differential",
                    IsTaxImpacting = true,
                    IsRevenueRecognized = false,
                    SubTotalAmount = 0.00m,
                    DiscountTotalAmount = 0.00m,
                    TaxTotalAmount = 12000.00m,
                    RoundOffAmount = 0.00m,
                    GrandTotalAmount = 12000.00m,
                    AppliedToInvoiceAmount = 12000.00m,
                    ReceivableAccountIdSnapshot = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.GSTPayable),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.GSTPayable),
                    RevenueAccountId = null,
                    RevenueAccountCode = null,
                    RevenueAccountName = null,
                    TaxAccountIdSnapshot = TaxAccountId,
                    TaxAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.GSTPayable),
                    TaxAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.GSTPayable),
                    IsSystemGenerated = false,
                    DebitNoteStatus = DebitNoteStatuses.Posted,
                    PostedOn = DateTime.Today.AddDays(-15),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedByUserName = "Finance Controller",
                    CreatedAt = DateTime.Today.AddDays(-16),
                    CreatedBy = "AR Supervisor"
                },

                // Debit Note 4: Draft - Additional charges
                new CustomerDebitNoteViewModel
                {
                    Id = DebitNote4Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftDubai),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftDubai),
                    CustomerId = Customer3Id,
                    CustomerCode = "CUST-003",
                    CustomerName = "XYZ Electronics Hub",
                    DebitNoteNumber = "DN-2024-0004",
                    DebitNoteDate = DateTime.Today.AddDays(-5),
                    PostingDate = null,
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    ReferenceText = "Add. charges for expedited delivery",
                    DebitNoteNarration = "Expedited delivery and priority support charges",
                    IsAgainstInvoice = true,
                    CustomerInvoiceId = Invoice3Id,
                    CustomerInvoiceNumber = "INV-2024-0003",
                    InvoiceNumberSnapshot = "INV-2024-0003",
                    InvoiceDateSnapshot = DateTime.Today.AddDays(-25),
                    DebitReasonCode = DebitReasonCodes.AdditionalCharges,
                    DebitReasonDescription = "Additional Charges - Expedited service",
                    IsTaxImpacting = true,
                    IsRevenueRecognized = true,
                    SubTotalAmount = 22500.00m,
                    DiscountTotalAmount = 0.00m,
                    TaxTotalAmount = 4050.00m,
                    RoundOffAmount = 0.00m,
                    GrandTotalAmount = 26550.00m,
                    AppliedToInvoiceAmount = 0.00m,
                    ReceivableAccountIdSnapshot = null,
                    ReceivableAccountCode = null,
                    ReceivableAccountName = null,
                    RevenueAccountId = ServiceRevenueAccountId,
                    RevenueAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.GSTPayable),
                    RevenueAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.GSTPayable),
                    TaxAccountIdSnapshot = null,
                    TaxAccountCode = null,
                    TaxAccountName = null,
                    IsSystemGenerated = false,
                    DebitNoteStatus = DebitNoteStatuses.Draft,
                    PostedOn = null,
                    PostedByUserId = null,
                    PostedByUserName = null,
                    CreatedAt = DateTime.Today.AddDays(-5),
                    CreatedBy = "AR Clerk"
                },

                // Debit Note 5: Submitted - Freight charges
                new CustomerDebitNoteViewModel
                {
                    Id = DebitNote5Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = HyderabadBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftDubai),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftDubai),
                    CustomerId = Customer5Id,
                    CustomerCode = "CUST-005",
                    CustomerName = "TechPark SEZ Solutions Pvt Ltd",
                    DebitNoteNumber = "DN-2024-0005",
                    DebitNoteDate = DateTime.Today.AddDays(-3),
                    PostingDate = null,
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    ReferenceText = "Freight charges - Hardware delivery",
                    DebitNoteNarration = "Freight and handling charges for hardware equipment delivery",
                    IsAgainstInvoice = false,
                    CustomerInvoiceId = null,
                    CustomerInvoiceNumber = null,
                    InvoiceNumberSnapshot = null,
                    InvoiceDateSnapshot = null,
                    DebitReasonCode = DebitReasonCodes.FreightDelivery,
                    DebitReasonDescription = "Freight / Delivery Charge",
                    IsTaxImpacting = true,
                    IsRevenueRecognized = true,
                    SubTotalAmount = 8500.00m,
                    DiscountTotalAmount = 0.00m,
                    TaxTotalAmount = 1530.00m,
                    RoundOffAmount = 0.00m,
                    GrandTotalAmount = 10030.00m,
                    AppliedToInvoiceAmount = 0.00m,
                    ReceivableAccountIdSnapshot = null,
                    ReceivableAccountCode = null,
                    ReceivableAccountName = null,
                    RevenueAccountId = OtherIncomeAccountId,
                    RevenueAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.GSTPayable),
                    RevenueAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.GSTPayable),
                    TaxAccountIdSnapshot = null,
                    TaxAccountCode = null,
                    TaxAccountName = null,
                    IsSystemGenerated = false,
                    DebitNoteStatus = DebitNoteStatuses.Submitted,
                    PostedOn = null,
                    PostedByUserId = null,
                    PostedByUserName = null,
                    CreatedAt = DateTime.Today.AddDays(-3),
                    CreatedBy = "AR Clerk"
                },

                // Debit Note 6: Cancelled - Rate revision
                new CustomerDebitNoteViewModel
                {
                    Id = DebitNote6Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftDubai),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftDubai),
                    CustomerId = Customer1Id,
                    CustomerCode = "CUST-001",
                    CustomerName = "ABC Traders Private Limited",
                    DebitNoteNumber = "DN-2024-0006",
                    DebitNoteDate = DateTime.Today.AddDays(-10),
                    PostingDate = null,
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    ReferenceText = "Rate revision - Cancelled",
                    DebitNoteNarration = "Rate revision cancelled as per customer agreement",
                    IsAgainstInvoice = true,
                    CustomerInvoiceId = Invoice1Id,
                    CustomerInvoiceNumber = "INV-2024-0001",
                    InvoiceNumberSnapshot = "INV-2024-0001",
                    InvoiceDateSnapshot = DateTime.Today.AddDays(-45),
                    DebitReasonCode = DebitReasonCodes.RateRevision,
                    DebitReasonDescription = "Rate Revision - Price adjustment",
                    IsTaxImpacting = true,
                    IsRevenueRecognized = true,
                    SubTotalAmount = 10000.00m,
                    DiscountTotalAmount = 0.00m,
                    TaxTotalAmount = 1800.00m,
                    RoundOffAmount = 0.00m,
                    GrandTotalAmount = 11800.00m,
                    AppliedToInvoiceAmount = 0.00m,
                    ReceivableAccountIdSnapshot = null,
                    ReceivableAccountCode = null,
                    ReceivableAccountName = null,
                    RevenueAccountId = ServiceRevenueAccountId,
                    RevenueAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.GSTPayable),
                    RevenueAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.GSTPayable),
                    TaxAccountIdSnapshot = null,
                    TaxAccountCode = null,
                    TaxAccountName = null,
                    IsSystemGenerated = false,
                    DebitNoteStatus = DebitNoteStatuses.Cancelled,
                    PostedOn = null,
                    PostedByUserId = null,
                    PostedByUserName = null,
                    CancelledOn = DateTime.Today.AddDays(-8),
                    CancelledByUserId = MasterDataIds.Tenants.Default,
                    CancelledByUserName = "Finance Controller",
                    CancellationReason = "Customer negotiated original contract terms - rate revision not applicable",
                    CreatedAt = DateTime.Today.AddDays(-10),
                    CreatedBy = "AR Clerk"
                },

                // Debit Note 7: Posted - Interest charges
                new CustomerDebitNoteViewModel
                {
                    Id = DebitNote7Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftDubai),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftDubai),
                    CustomerId = Customer4Id,
                    CustomerCode = "CUST-004",
                    CustomerName = "Rajesh Kumar",
                    DebitNoteNumber = "DN-2024-0007",
                    DebitNoteDate = DateTime.Today.AddDays(-25),
                    PostingDate = DateTime.Today.AddDays(-25),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    ReferenceText = "Interest on overdue invoice INV-2024-0001",
                    DebitNoteNarration = "Interest charges at 18% p.a. for overdue payment",
                    IsAgainstInvoice = true,
                    CustomerInvoiceId = Invoice1Id,
                    CustomerInvoiceNumber = "INV-2024-0001",
                    InvoiceNumberSnapshot = "INV-2024-0001",
                    InvoiceDateSnapshot = DateTime.Today.AddDays(-90),
                    DebitReasonCode = DebitReasonCodes.LateFee,
                    DebitReasonDescription = "Late Fee / Interest",
                    IsTaxImpacting = false,
                    IsRevenueRecognized = true,
                    SubTotalAmount = 12500.00m,
                    DiscountTotalAmount = 0.00m,
                    TaxTotalAmount = 0.00m,
                    RoundOffAmount = 0.00m,
                    GrandTotalAmount = 12500.00m,
                    AppliedToInvoiceAmount = 12500.00m,
                    ReceivableAccountIdSnapshot = null,
                    ReceivableAccountCode = null,
                    ReceivableAccountName = null,
                    RevenueAccountId = OtherIncomeAccountId,
                    RevenueAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.GSTPayable),
                    RevenueAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.GSTPayable),
                    TaxAccountIdSnapshot = null,
                    TaxAccountCode = null,
                    TaxAccountName = null,
                    IsSystemGenerated = false,
                    DebitNoteStatus = DebitNoteStatuses.Posted,
                    PostedOn = DateTime.Today.AddDays(-25),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedByUserName = "Finance Controller",
                    CreatedAt = DateTime.Today.AddDays(-25),
                    CreatedBy = "Finance Manager"
                },

                // Debit Note 8: Draft - Price increase
                new CustomerDebitNoteViewModel
                {
                    Id = DebitNote8Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = BangaloreBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftDubai),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftDubai),
                    CustomerId = Customer6Id,
                    CustomerCode = "CUST-006",
                    CustomerName = "Discontinued Enterprises",
                    DebitNoteNumber = "DN-2024-0008",
                    DebitNoteDate = DateTime.Today.AddDays(-2),
                    PostingDate = null,
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    ReferenceText = "Rate revision Q4",
                    DebitNoteNarration = "Price increase effective from next quarter",
                    IsAgainstInvoice = false,
                    CustomerInvoiceId = null,
                    CustomerInvoiceNumber = null,
                    InvoiceNumberSnapshot = null,
                    InvoiceDateSnapshot = null,
                    DebitReasonCode = DebitReasonCodes.RateRevision,
                    DebitReasonDescription = "Rate Revision / Price Increase",
                    IsTaxImpacting = true,
                    IsRevenueRecognized = false,
                    SubTotalAmount = 35000.00m,
                    DiscountTotalAmount = 0.00m,
                    TaxTotalAmount = 6300.00m,
                    RoundOffAmount = 0.00m,
                    GrandTotalAmount = 41300.00m,
                    AppliedToInvoiceAmount = 0.00m,
                    ReceivableAccountIdSnapshot = null,
                    ReceivableAccountCode = null,
                    ReceivableAccountName = null,
                    RevenueAccountId = OtherIncomeAccountId,
                    RevenueAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.GSTPayable),
                    RevenueAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.GSTPayable),
                    TaxAccountIdSnapshot = null,
                    TaxAccountCode = null,
                    TaxAccountName = null,
                    IsSystemGenerated = false,
                    DebitNoteStatus = DebitNoteStatuses.Draft,
                    PostedOn = null,
                    PostedByUserId = null,
                    PostedByUserName = null,
                    CreatedAt = DateTime.Today.AddDays(-2),
                    CreatedBy = "Sales Manager"
                },

                // Debit Note 9: Submitted - Additional charges
                new CustomerDebitNoteViewModel
                {
                    Id = DebitNote9Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = HyderabadBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftDubai),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftDubai),
                    CustomerId = Customer7Id,
                    CustomerCode = "CUST-007",
                    CustomerName = "Silicon Valley Partners LLC",
                    DebitNoteNumber = "DN-2024-0009",
                    DebitNoteDate = DateTime.Today.AddDays(-5),
                    PostingDate = null,
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    ReferenceText = "Additional setup charges",
                    DebitNoteNarration = "Additional server setup and configuration charges",
                    IsAgainstInvoice = false,
                    CustomerInvoiceId = null,
                    CustomerInvoiceNumber = null,
                    InvoiceNumberSnapshot = null,
                    InvoiceDateSnapshot = null,
                    DebitReasonCode = DebitReasonCodes.AdditionalCharges,
                    DebitReasonDescription = "Additional Charges",
                    IsTaxImpacting = true,
                    IsRevenueRecognized = true,
                    SubTotalAmount = 22000.00m,
                    DiscountTotalAmount = 0.00m,
                    TaxTotalAmount = 3960.00m,
                    RoundOffAmount = 0.00m,
                    GrandTotalAmount = 25960.00m,
                    AppliedToInvoiceAmount = 0.00m,
                    ReceivableAccountIdSnapshot = null,
                    ReceivableAccountCode = null,
                    ReceivableAccountName = null,
                    RevenueAccountId = OtherIncomeAccountId,
                    RevenueAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.GSTPayable),
                    RevenueAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.GSTPayable),
                    TaxAccountIdSnapshot = null,
                    TaxAccountCode = null,
                    TaxAccountName = null,
                    IsSystemGenerated = false,
                    DebitNoteStatus = DebitNoteStatuses.Submitted,
                    PostedOn = null,
                    PostedByUserId = null,
                    PostedByUserName = null,
                    CreatedAt = DateTime.Today.AddDays(-5),
                    CreatedBy = "AR Clerk"
                },

                // Debit Note 10: Posted - Tax short-charged
                new CustomerDebitNoteViewModel
                {
                    Id = DebitNote10Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftDubai),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftDubai),
                    CustomerId = Customer2Id,
                    CustomerCode = "CUST-002",
                    CustomerName = "State Government Education Dept",
                    DebitNoteNumber = "DN-2024-0010",
                    DebitNoteDate = DateTime.Today.AddDays(-18),
                    PostingDate = DateTime.Today.AddDays(-18),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    ReferenceText = "GST correction for INV-2024-0002",
                    DebitNoteNarration = "Tax correction - IGST charged instead of CGST+SGST",
                    IsAgainstInvoice = true,
                    CustomerInvoiceId = Invoice2Id,
                    CustomerInvoiceNumber = "INV-2024-0002",
                    InvoiceNumberSnapshot = "INV-2024-0002",
                    InvoiceDateSnapshot = DateTime.Today.AddDays(-60),
                    DebitReasonCode = DebitReasonCodes.TaxShortCharged,
                    DebitReasonDescription = "Tax Short Charged",
                    IsTaxImpacting = true,
                    IsRevenueRecognized = false,
                    SubTotalAmount = 0.00m,
                    DiscountTotalAmount = 0.00m,
                    TaxTotalAmount = 4500.00m,
                    RoundOffAmount = 0.00m,
                    GrandTotalAmount = 4500.00m,
                    AppliedToInvoiceAmount = 4500.00m,
                    ReceivableAccountIdSnapshot = null,
                    ReceivableAccountCode = null,
                    ReceivableAccountName = null,
                    RevenueAccountId = OtherIncomeAccountId,
                    RevenueAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.GSTPayable),
                    RevenueAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.GSTPayable),
                    TaxAccountIdSnapshot = null,
                    TaxAccountCode = null,
                    TaxAccountName = null,
                    IsSystemGenerated = false,
                    DebitNoteStatus = DebitNoteStatuses.Posted,
                    PostedOn = DateTime.Today.AddDays(-18),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedByUserName = "Finance Controller",
                    CreatedAt = DateTime.Today.AddDays(-18),
                    CreatedBy = "Finance Manager"
                },

                // Debit Note 11: Posted - Underbilling correction
                new CustomerDebitNoteViewModel
                {
                    Id = DebitNote11Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = BangaloreBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftDubai),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftDubai),
                    CustomerId = Customer3Id,
                    CustomerCode = "CUST-003",
                    CustomerName = "XYZ Electronics Hub",
                    DebitNoteNumber = "DN-2024-0011",
                    DebitNoteDate = DateTime.Today.AddDays(-30),
                    PostingDate = DateTime.Today.AddDays(-30),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    ReferenceText = "Underbilling for INV-2024-0003",
                    DebitNoteNarration = "Correction for underbilled implementation services",
                    IsAgainstInvoice = true,
                    CustomerInvoiceId = Invoice3Id,
                    CustomerInvoiceNumber = "INV-2024-0003",
                    InvoiceNumberSnapshot = "INV-2024-0003",
                    InvoiceDateSnapshot = DateTime.Today.AddDays(-50),
                    DebitReasonCode = DebitReasonCodes.UnderbillingCorrection,
                    DebitReasonDescription = "Underbilling Correction",
                    IsTaxImpacting = true,
                    IsRevenueRecognized = true,
                    SubTotalAmount = 45000.00m,
                    DiscountTotalAmount = 0.00m,
                    TaxTotalAmount = 8100.00m,
                    RoundOffAmount = 0.00m,
                    GrandTotalAmount = 53100.00m,
                    AppliedToInvoiceAmount = 53100.00m,
                    ReceivableAccountIdSnapshot = null,
                    ReceivableAccountCode = null,
                    ReceivableAccountName = null,
                    RevenueAccountId = OtherIncomeAccountId,
                    RevenueAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.GSTPayable),
                    RevenueAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.GSTPayable),
                    TaxAccountIdSnapshot = null,
                    TaxAccountCode = null,
                    TaxAccountName = null,
                    IsSystemGenerated = false,
                    DebitNoteStatus = DebitNoteStatuses.Posted,
                    PostedOn = DateTime.Today.AddDays(-30),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedByUserName = "Finance Controller",
                    CreatedAt = DateTime.Today.AddDays(-30),
                    CreatedBy = "Finance Controller"
                },

                // Debit Note 12: Cancelled - Freight charges
                new CustomerDebitNoteViewModel
                {
                    Id = DebitNote12Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = HyderabadBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftDubai),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftDubai),
                    CustomerId = Customer1Id,
                    CustomerCode = "CUST-001",
                    CustomerName = "ABC Traders Private Limited",
                    DebitNoteNumber = "DN-2024-0012",
                    DebitNoteDate = DateTime.Today.AddDays(-12),
                    PostingDate = null,
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    ReferenceText = "Freight charges - cancelled",
                    DebitNoteNarration = "Freight charges waived as part of customer retention",
                    IsAgainstInvoice = false,
                    CustomerInvoiceId = null,
                    CustomerInvoiceNumber = null,
                    InvoiceNumberSnapshot = null,
                    InvoiceDateSnapshot = null,
                    DebitReasonCode = DebitReasonCodes.FreightDelivery,
                    DebitReasonDescription = "Freight / Delivery Charge",
                    IsTaxImpacting = true,
                    IsRevenueRecognized = false,
                    SubTotalAmount = 15000.00m,
                    DiscountTotalAmount = 0.00m,
                    TaxTotalAmount = 2700.00m,
                    RoundOffAmount = 0.00m,
                    GrandTotalAmount = 17700.00m,
                    AppliedToInvoiceAmount = 0.00m,
                    ReceivableAccountIdSnapshot = null,
                    ReceivableAccountCode = null,
                    ReceivableAccountName = null,
                    RevenueAccountId = OtherIncomeAccountId,
                    RevenueAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.GSTPayable),
                    RevenueAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.GSTPayable),
                    TaxAccountIdSnapshot = null,
                    TaxAccountCode = null,
                    TaxAccountName = null,
                    IsSystemGenerated = false,
                    DebitNoteStatus = DebitNoteStatuses.Cancelled,
                    PostedOn = null,
                    PostedByUserId = null,
                    PostedByUserName = null,
                    CancelledOn = DateTime.Today.AddDays(-11),
                    CancelledByUserId = MasterDataIds.Tenants.Default,
                    CancelledByUserName = "Finance Controller",
                    CancellationReason = "Freight charges waived for customer retention",
                    CreatedAt = DateTime.Today.AddDays(-12),
                    CreatedBy = "AR Clerk"
                }
            };

            // Add lines to each debit note
            AddLinesToDebitNote1(debitNotes[0]);
            AddLinesToDebitNote2(debitNotes[1]);
            AddLinesToDebitNote3(debitNotes[2]);
            AddLinesToDebitNote4(debitNotes[3]);
            AddLinesToDebitNote5(debitNotes[4]);
            AddLinesToDebitNote6(debitNotes[5]);

            return debitNotes;
        }

        private static void AddLinesToDebitNote1(CustomerDebitNoteViewModel debitNote)
        {
            debitNote.Lines = new List<CustomerDebitNoteLineViewModel>
            {
                new CustomerDebitNoteLineViewModel
                {
                    Id = Guid.NewGuid(),
                    CustomerDebitNoteId = debitNote.Id,
                    LineNumber = 10,
                    LineType = LineTypes.Service,
                    Description = "Additional Implementation Support - 10 hours",
                    Quantity = 10,
                    UnitPrice = 1500.00m,
                    DiscountPercent = 0,
                    DiscountAmount = 0.00m,
                    TaxCodeId = GST18TaxCodeId,
                    TaxCodeCode = "GST18",
                    TaxCodeName = "GST 18%",
                    TaxRatePercent = 18,
                    TaxAmount = 2700.00m,
                    RevenueAccountId = ServiceRevenueAccountId,
                    RevenueAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.GSTPayable),
                    RevenueAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.GSTPayable),
                    SACCode = "998314",
                    CreatedAt = debitNote.CreatedAt,
                    CreatedBy = debitNote.CreatedBy
                }
            };
        }

        private static void AddLinesToDebitNote2(CustomerDebitNoteViewModel debitNote)
        {
            debitNote.Lines = new List<CustomerDebitNoteLineViewModel>
            {
                new CustomerDebitNoteLineViewModel
                {
                    Id = Guid.NewGuid(),
                    CustomerDebitNoteId = debitNote.Id,
                    LineNumber = 10,
                    LineType = LineTypes.Charge,
                    Description = "Late Payment Penalty - 60+ days overdue",
                    Quantity = 1,
                    UnitPrice = 5000.00m,
                    DiscountPercent = 0,
                    DiscountAmount = 0.00m,
                    TaxCodeId = GST18TaxCodeId,
                    TaxCodeCode = "GST18",
                    TaxCodeName = "GST 18%",
                    TaxRatePercent = 18,
                    TaxAmount = 900.00m,
                    RevenueAccountId = OtherIncomeAccountId,
                    RevenueAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.GSTPayable),
                    RevenueAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.GSTPayable),
                    SACCode = "997159",
                    IsSystemGenerated = true,
                    CreatedAt = debitNote.CreatedAt,
                    CreatedBy = debitNote.CreatedBy
                }
            };
        }

        private static void AddLinesToDebitNote3(CustomerDebitNoteViewModel debitNote)
        {
            debitNote.Lines = new List<CustomerDebitNoteLineViewModel>
            {
                new CustomerDebitNoteLineViewModel
                {
                    Id = Guid.NewGuid(),
                    CustomerDebitNoteId = debitNote.Id,
                    LineNumber = 10,
                    LineType = LineTypes.Charge,
                    Description = "Tax Differential - GST 6% on ₹200,000",
                    Quantity = 1,
                    UnitPrice = 0.00m,
                    DiscountPercent = 0,
                    DiscountAmount = 0.00m,
                    TaxCodeId = GST18TaxCodeId,
                    TaxCodeCode = "GST-DIFF",
                    TaxCodeName = "GST Differential 6%",
                    TaxRatePercent = 0,
                    TaxAmount = 12000.00m,
                    RevenueAccountId = ServiceRevenueAccountId,
                    RevenueAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.GSTPayable),
                    RevenueAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.GSTPayable),
                    SACCode = "998314",
                    CreatedAt = debitNote.CreatedAt,
                    CreatedBy = debitNote.CreatedBy
                }
            };
        }

        private static void AddLinesToDebitNote4(CustomerDebitNoteViewModel debitNote)
        {
            debitNote.Lines = new List<CustomerDebitNoteLineViewModel>
            {
                new CustomerDebitNoteLineViewModel
                {
                    Id = Guid.NewGuid(),
                    CustomerDebitNoteId = debitNote.Id,
                    LineNumber = 10,
                    LineType = LineTypes.Charge,
                    Description = "Expedited Delivery Surcharge",
                    Quantity = 1,
                    UnitPrice = 7500.00m,
                    DiscountPercent = 0,
                    DiscountAmount = 0.00m,
                    TaxCodeId = GST18TaxCodeId,
                    TaxCodeCode = "GST18",
                    TaxCodeName = "GST 18%",
                    TaxRatePercent = 18,
                    TaxAmount = 1350.00m,
                    RevenueAccountId = ServiceRevenueAccountId,
                    RevenueAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.GSTPayable),
                    RevenueAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.GSTPayable),
                    SACCode = "996812",
                    CreatedAt = debitNote.CreatedAt,
                    CreatedBy = debitNote.CreatedBy
                },
                new CustomerDebitNoteLineViewModel
                {
                    Id = Guid.NewGuid(),
                    CustomerDebitNoteId = debitNote.Id,
                    LineNumber = 20,
                    LineType = LineTypes.Service,
                    Description = "Priority Support Package - 1 month",
                    Quantity = 1,
                    UnitPrice = 15000.00m,
                    DiscountPercent = 0,
                    DiscountAmount = 0.00m,
                    TaxCodeId = GST18TaxCodeId,
                    TaxCodeCode = "GST18",
                    TaxCodeName = "GST 18%",
                    TaxRatePercent = 18,
                    TaxAmount = 2700.00m,
                    RevenueAccountId = ServiceRevenueAccountId,
                    RevenueAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.GSTPayable),
                    RevenueAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.GSTPayable),
                    SACCode = "998316",
                    CreatedAt = debitNote.CreatedAt,
                    CreatedBy = debitNote.CreatedBy
                }
            };
        }

        private static void AddLinesToDebitNote5(CustomerDebitNoteViewModel debitNote)
        {
            debitNote.Lines = new List<CustomerDebitNoteLineViewModel>
            {
                new CustomerDebitNoteLineViewModel
                {
                    Id = Guid.NewGuid(),
                    CustomerDebitNoteId = debitNote.Id,
                    LineNumber = 10,
                    LineType = LineTypes.Charge,
                    Description = "Freight Charges - Server Equipment",
                    Quantity = 1,
                    UnitPrice = 6000.00m,
                    DiscountPercent = 0,
                    DiscountAmount = 0.00m,
                    TaxCodeId = GST18TaxCodeId,
                    TaxCodeCode = "GST18",
                    TaxCodeName = "GST 18%",
                    TaxRatePercent = 18,
                    TaxAmount = 1080.00m,
                    RevenueAccountId = OtherIncomeAccountId,
                    RevenueAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.GSTPayable),
                    RevenueAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.GSTPayable),
                    SACCode = "996511",
                    CreatedAt = debitNote.CreatedAt,
                    CreatedBy = debitNote.CreatedBy
                },
                new CustomerDebitNoteLineViewModel
                {
                    Id = Guid.NewGuid(),
                    CustomerDebitNoteId = debitNote.Id,
                    LineNumber = 20,
                    LineType = LineTypes.Charge,
                    Description = "Handling & Insurance",
                    Quantity = 1,
                    UnitPrice = 2500.00m,
                    DiscountPercent = 0,
                    DiscountAmount = 0.00m,
                    TaxCodeId = GST18TaxCodeId,
                    TaxCodeCode = "GST18",
                    TaxCodeName = "GST 18%",
                    TaxRatePercent = 18,
                    TaxAmount = 450.00m,
                    RevenueAccountId = OtherIncomeAccountId,
                    RevenueAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.GSTPayable),
                    RevenueAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.GSTPayable),
                    SACCode = "996512",
                    CreatedAt = debitNote.CreatedAt,
                    CreatedBy = debitNote.CreatedBy
                }
            };
        }

        private static void AddLinesToDebitNote6(CustomerDebitNoteViewModel debitNote)
        {
            debitNote.Lines = new List<CustomerDebitNoteLineViewModel>
            {
                new CustomerDebitNoteLineViewModel
                {
                    Id = Guid.NewGuid(),
                    CustomerDebitNoteId = debitNote.Id,
                    LineNumber = 10,
                    LineType = LineTypes.Service,
                    Description = "Rate Revision - License Fee Adjustment",
                    Quantity = 1,
                    UnitPrice = 10000.00m,
                    DiscountPercent = 0,
                    DiscountAmount = 0.00m,
                    TaxCodeId = GST18TaxCodeId,
                    TaxCodeCode = "GST18",
                    TaxCodeName = "GST 18%",
                    TaxRatePercent = 18,
                    TaxAmount = 1800.00m,
                    RevenueAccountId = ServiceRevenueAccountId,
                    RevenueAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.GSTPayable),
                    RevenueAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.GSTPayable),
                    SACCode = "998314",
                    CreatedAt = debitNote.CreatedAt,
                    CreatedBy = debitNote.CreatedBy
                }
            };
        }
    }
}
