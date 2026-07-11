using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    /// <summary>
    /// Seed data for VendorAccount model
    /// VendorAccount is the AP subledger balance record showing amounts owed to vendors
    /// </summary>
    public static class VendorAccountSeedData
    {
        // Company GUIDs (matching existing company seed data)
        private static readonly Guid AscendingSoftwareCompanyId = MasterDataIds.Companies.SofaCraft;
        private static readonly Guid GlobalTechCompanyId = MasterDataIds.Companies.SofaCraftUSA;

        // Currency GUIDs (matching existing currency seed data)
        private static readonly Guid InrCurrencyId = MasterDataIds.Currencies.INR;
        private static readonly Guid UsdCurrencyId = MasterDataIds.Currencies.USD;

        // GL Account GUIDs (placeholder for AP Control Account)
        private static readonly Guid APControlAccountId = Guid.Parse("00000000-0000-0000-0000-000000000010");

        // Predefined VendorAccount GUIDs
        public static readonly Guid VendorAccount1Id = Guid.Parse("a6a6a6a6-0001-0001-0001-000000000001");
        public static readonly Guid VendorAccount2Id = Guid.Parse("a6a6a6a6-0001-0001-0001-000000000002");
        public static readonly Guid VendorAccount3Id = Guid.Parse("a6a6a6a6-0001-0001-0001-000000000003");
        public static readonly Guid VendorAccount4Id = Guid.Parse("a6a6a6a6-0001-0001-0001-000000000004");
        public static readonly Guid VendorAccount5Id = Guid.Parse("a6a6a6a6-0001-0001-0001-000000000005");
        public static readonly Guid VendorAccount6Id = Guid.Parse("a6a6a6a6-0001-0001-0001-000000000006");
        public static readonly Guid VendorAccount7Id = Guid.Parse("a6a6a6a6-0001-0001-0001-000000000007");
        public static readonly Guid VendorAccount8Id = Guid.Parse("a6a6a6a6-0001-0001-0001-000000000008");

        public static readonly Guid VendorAccount9Id = Guid.Parse("a6a6a6a6-0001-0001-0001-000000000009");
        public static readonly Guid VendorAccount10Id = Guid.Parse("a6a6a6a6-0001-0001-0001-000000000010");
        public static readonly Guid VendorAccount11Id = Guid.Parse("a6a6a6a6-0001-0001-0001-000000000011");
        public static readonly Guid VendorAccount12Id = Guid.Parse("a6a6a6a6-0001-0001-0001-000000000012");

        public static List<VendorAccountViewModel> GetSeedVendorAccounts()
        {
            return new List<VendorAccountViewModel>
            {
                // VendorAccount 1: Tech Components India - Active with outstanding payable
                new VendorAccountViewModel
                {
                    Id = VendorAccount1Id,
                    CompanyId = AscendingSoftwareCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                    VendorId = VendorSeedData.Vendor1Id,
                    VendorCode = "VND-000001",
                    VendorName = "Tech Components India Pvt Ltd",
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    AccountStatus = VendorAccountStatuses.Active,
                    OutstandingPayableAmount = 245000.00m,
                    AdvancePaidAmount = 15000.00m,
                    TotalBillsPostedAmount = 850000.00m,
                    TotalPaymentsPostedAmount = 605000.00m,
                    TotalCreditNotesPostedAmount = 0,
                    TotalDebitNotesPostedAmount = 0,
                    TotalAdjustmentsPostedAmount = 0,
                    IsPaymentBlocked = false,
                    IsPostingBlocked = false,
                    LastTransactionOn = DateTime.Now.AddDays(-3),
                    LastBillPostedOn = DateTime.Now.AddDays(-7),
                    LastPaymentPostedOn = DateTime.Now.AddDays(-3),
                    LastDocumentReference = "PAY-000087",
                    APControlAccountIdSnapshot = APControlAccountId,
                    APControlAccountCode = "2001",
                    APControlAccountName = "Accounts Payable",
                    CreatedAt = DateTime.Now.AddDays(-90),
                    CreatedBy = "System"
                },

                // VendorAccount 2: CloudTech Solutions - Active with larger payable
                new VendorAccountViewModel
                {
                    Id = VendorAccount2Id,
                    CompanyId = AscendingSoftwareCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                    VendorId = VendorSeedData.Vendor2Id,
                    VendorCode = "VND-000002",
                    VendorName = "CloudTech Solutions",
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    AccountStatus = VendorAccountStatuses.Active,
                    OutstandingPayableAmount = 475000.00m,
                    AdvancePaidAmount = 0,
                    TotalBillsPostedAmount = 1250000.00m,
                    TotalPaymentsPostedAmount = 775000.00m,
                    TotalCreditNotesPostedAmount = 0,
                    TotalDebitNotesPostedAmount = 0,
                    TotalAdjustmentsPostedAmount = 0,
                    IsPaymentBlocked = false,
                    IsPostingBlocked = false,
                    LastTransactionOn = DateTime.Now.AddDays(-5),
                    LastBillPostedOn = DateTime.Now.AddDays(-12),
                    LastPaymentPostedOn = DateTime.Now.AddDays(-5),
                    LastDocumentReference = "BILL-000245",
                    APControlAccountIdSnapshot = APControlAccountId,
                    APControlAccountCode = "2001",
                    APControlAccountName = "Accounts Payable",
                    CreatedAt = DateTime.Now.AddDays(-75),
                    CreatedBy = "System"
                },

                // VendorAccount 3: Office Supplies Co - Payment Blocked due to bank verification pending
                new VendorAccountViewModel
                {
                    Id = VendorAccount3Id,
                    CompanyId = AscendingSoftwareCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                    VendorId = VendorSeedData.Vendor3Id,
                    VendorCode = "VND-000003",
                    VendorName = "Reliable Supplies Co",
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    AccountStatus = VendorAccountStatuses.Active,
                    OutstandingPayableAmount = 32500.00m,
                    AdvancePaidAmount = 0,
                    TotalBillsPostedAmount = 125000.00m,
                    TotalPaymentsPostedAmount = 92500.00m,
                    TotalCreditNotesPostedAmount = 0,
                    TotalDebitNotesPostedAmount = 0,
                    TotalAdjustmentsPostedAmount = 0,
                    IsPaymentBlocked = true,
                    PaymentBlockReason = "Bank details verification pending. New account details received, awaiting confirmation.",
                    IsPostingBlocked = false,
                    BlockedOn = DateTime.Now.AddDays(-5),
                    BlockedByUserId = MasterDataIds.Tenants.Default,
                    BlockedByUserName = "Finance Controller",
                    LastTransactionOn = DateTime.Now.AddDays(-15),
                    LastBillPostedOn = DateTime.Now.AddDays(-15),
                    LastPaymentPostedOn = DateTime.Now.AddDays(-45),
                    LastDocumentReference = "BILL-000198",
                    APControlAccountIdSnapshot = APControlAccountId,
                    APControlAccountCode = "2001",
                    APControlAccountName = "Accounts Payable",
                    CreatedAt = DateTime.Now.AddDays(-120),
                    CreatedBy = "System"
                },

                // VendorAccount 4: Freelancer - Zero balance, all paid up
                new VendorAccountViewModel
                {
                    Id = VendorAccount4Id,
                    CompanyId = AscendingSoftwareCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                    VendorId = VendorSeedData.Vendor4Id,
                    VendorCode = "VND-000004",
                    VendorName = "BuildRight Constructions",
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    AccountStatus = VendorAccountStatuses.Active,
                    OutstandingPayableAmount = 0,
                    AdvancePaidAmount = 0,
                    TotalBillsPostedAmount = 175000.00m,
                    TotalPaymentsPostedAmount = 175000.00m,
                    TotalCreditNotesPostedAmount = 0,
                    TotalDebitNotesPostedAmount = 0,
                    TotalAdjustmentsPostedAmount = 0,
                    IsPaymentBlocked = false,
                    IsPostingBlocked = false,
                    LastTransactionOn = DateTime.Now.AddDays(-10),
                    LastBillPostedOn = DateTime.Now.AddDays(-25),
                    LastPaymentPostedOn = DateTime.Now.AddDays(-10),
                    LastDocumentReference = "PAY-000072",
                    APControlAccountIdSnapshot = APControlAccountId,
                    APControlAccountCode = "2001",
                    APControlAccountName = "Accounts Payable",
                    CreatedAt = DateTime.Now.AddDays(-60),
                    CreatedBy = "System"
                },

                // VendorAccount 5: Utility Company - Has advance payment (we paid before bill)
                new VendorAccountViewModel
                {
                    Id = VendorAccount5Id,
                    CompanyId = AscendingSoftwareCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                    VendorId = VendorSeedData.Vendor5Id,
                    VendorCode = "VND-000005",
                    VendorName = "Tamil Nadu Electricity Board",
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    AccountStatus = VendorAccountStatuses.Active,
                    OutstandingPayableAmount = 18000.00m,
                    AdvancePaidAmount = 25000.00m, // Advance surplus scenario
                    TotalBillsPostedAmount = 240000.00m,
                    TotalPaymentsPostedAmount = 247000.00m,
                    TotalCreditNotesPostedAmount = 0,
                    TotalDebitNotesPostedAmount = 0,
                    TotalAdjustmentsPostedAmount = 0,
                    IsPaymentBlocked = false,
                    IsPostingBlocked = false,
                    LastTransactionOn = DateTime.Now.AddDays(-2),
                    LastBillPostedOn = DateTime.Now.AddDays(-2),
                    LastPaymentPostedOn = DateTime.Now.AddDays(-8),
                    LastDocumentReference = "BILL-000256",
                    APControlAccountIdSnapshot = APControlAccountId,
                    APControlAccountCode = "2001",
                    APControlAccountName = "Accounts Payable",
                    CreatedAt = DateTime.Now.AddDays(-100),
                    CreatedBy = "System"
                },

                // VendorAccount 6: Prime Properties (Landlord) - Frozen account, legacy vendor
                new VendorAccountViewModel
                {
                    Id = VendorAccount6Id,
                    CompanyId = AscendingSoftwareCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                    VendorId = VendorSeedData.Vendor6Id,
                    VendorCode = "VND-000006",
                    VendorName = "Prime Properties",
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    AccountStatus = VendorAccountStatuses.Frozen,
                    OutstandingPayableAmount = 150000.00m,
                    AdvancePaidAmount = 0,
                    TotalBillsPostedAmount = 1800000.00m,
                    TotalPaymentsPostedAmount = 1650000.00m,
                    TotalCreditNotesPostedAmount = 0,
                    TotalDebitNotesPostedAmount = 0,
                    TotalAdjustmentsPostedAmount = 0,
                    IsPaymentBlocked = true,
                    PaymentBlockReason = "Vendor account frozen due to lease dispute. Legal review pending.",
                    IsPostingBlocked = true,
                    PostingBlockReason = "All posting blocked until dispute resolution. Contact legal@company.com",
                    BlockedOn = DateTime.Now.AddDays(-30),
                    BlockedByUserId = MasterDataIds.Tenants.Default,
                    BlockedByUserName = "Finance Controller",
                    LastTransactionOn = DateTime.Now.AddDays(-45),
                    LastBillPostedOn = DateTime.Now.AddDays(-45),
                    LastPaymentPostedOn = DateTime.Now.AddDays(-60),
                    LastDocumentReference = "BILL-000189",
                    LastReconciledOn = DateTime.Now.AddDays(-35),
                    LastReconciledByUserId = MasterDataIds.PaymentTerms.Net45,
                    LastReconciledByUserName = "AP Supervisor",
                    APControlAccountIdSnapshot = APControlAccountId,
                    APControlAccountCode = "2001",
                    APControlAccountName = "Accounts Payable",
                    CreatedAt = DateTime.Now.AddDays(-365),
                    CreatedBy = "System",
                    UpdatedAt = DateTime.Now.AddDays(-30),
                    UpdatedBy = "Finance Controller"
                },

                // VendorAccount 7: TechVentures USA (USD) - Foreign currency vendor
                new VendorAccountViewModel
                {
                    Id = VendorAccount7Id,
                    CompanyId = GlobalTechCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                    VendorId = VendorSeedData.Vendor7Id,
                    VendorCode = "VND-000007",
                    VendorName = "TechVentures USA Inc",
                    CurrencyId = UsdCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.USD),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.USD),
                    AccountStatus = VendorAccountStatuses.Active,
                    OutstandingPayableAmount = 35000.00m,
                    AdvancePaidAmount = 5000.00m,
                    TotalBillsPostedAmount = 120000.00m,
                    TotalPaymentsPostedAmount = 90000.00m,
                    TotalCreditNotesPostedAmount = 0,
                    TotalDebitNotesPostedAmount = 0,
                    TotalAdjustmentsPostedAmount = 0,
                    IsPaymentBlocked = false,
                    IsPostingBlocked = false,
                    LastTransactionOn = DateTime.Now.AddDays(-1),
                    LastBillPostedOn = DateTime.Now.AddDays(-5),
                    LastPaymentPostedOn = DateTime.Now.AddDays(-1),
                    LastDocumentReference = "PAY-000095",
                    APControlAccountIdSnapshot = APControlAccountId,
                    APControlAccountCode = "2100-USD",
                    APControlAccountName = "Accounts Payable - USD",
                    CreatedAt = DateTime.Now.AddDays(-40),
                    CreatedBy = "System"
                },

                // VendorAccount 8: New Supplier - Fresh account, no transactions yet
                new VendorAccountViewModel
                {
                    Id = VendorAccount8Id,
                    CompanyId = AscendingSoftwareCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                    VendorId = VendorSeedData.Vendor8Id,
                    VendorCode = "VND-000008",
                    VendorName = "New Supplier Pending Approval",
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    AccountStatus = VendorAccountStatuses.Active,
                    OutstandingPayableAmount = 0,
                    AdvancePaidAmount = 0,
                    TotalBillsPostedAmount = 0,
                    TotalPaymentsPostedAmount = 0,
                    TotalCreditNotesPostedAmount = 0,
                    TotalDebitNotesPostedAmount = 0,
                    TotalAdjustmentsPostedAmount = 0,
                    IsPaymentBlocked = false,
                    IsPostingBlocked = false,
                    LastTransactionOn = null,
                    LastBillPostedOn = null,
                    LastPaymentPostedOn = null,
                    LastDocumentReference = null,
                    APControlAccountIdSnapshot = APControlAccountId,
                    APControlAccountCode = "2001",
                    APControlAccountName = "Accounts Payable",
                    CreatedAt = DateTime.Now.AddDays(-2),
                    CreatedBy = "System"
                },

                // VendorAccount 9: DataCenter Solutions - Active
                new VendorAccountViewModel
                {
                    Id = VendorAccount9Id,
                    CompanyId = AscendingSoftwareCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                    VendorId = VendorSeedData.Vendor9Id,
                    VendorCode = "VND-000009",
                    VendorName = "DataCenter Solutions Ltd",
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    AccountStatus = VendorAccountStatuses.Active,
                    OutstandingPayableAmount = 180000.00m,
                    AdvancePaidAmount = 0,
                    TotalBillsPostedAmount = 380000,
                    TotalPaymentsPostedAmount = 200000,
                    TotalCreditNotesPostedAmount = 0,
                    TotalDebitNotesPostedAmount = 0,
                    TotalAdjustmentsPostedAmount = 0,
                    IsPaymentBlocked = false,
                    IsPostingBlocked = false,
                    LastTransactionOn = DateTime.Now.AddDays(-10),
                    LastBillPostedOn = DateTime.Now.AddDays(-10),
                    LastPaymentPostedOn = DateTime.Now.AddDays(-15),
                    APControlAccountIdSnapshot = APControlAccountId,
                    APControlAccountCode = "2001",
                    APControlAccountName = "Accounts Payable",
                    CreatedAt = DateTime.Now.AddDays(-90),
                    CreatedBy = "System"
                },

                // VendorAccount 10: Office Essentials - Active
                new VendorAccountViewModel
                {
                    Id = VendorAccount10Id,
                    CompanyId = AscendingSoftwareCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                    VendorId = VendorSeedData.Vendor10Id,
                    VendorCode = "VND-000010",
                    VendorName = "CloudTech Solutions",
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    AccountStatus = VendorAccountStatuses.Active,
                    OutstandingPayableAmount = 42500.00m,
                    AdvancePaidAmount = 0,
                    TotalBillsPostedAmount = 95000,
                    TotalPaymentsPostedAmount = 47500,
                    TotalCreditNotesPostedAmount = 5000,
                    TotalDebitNotesPostedAmount = 0,
                    TotalAdjustmentsPostedAmount = 0,
                    IsPaymentBlocked = false,
                    IsPostingBlocked = false,
                    LastTransactionOn = DateTime.Now.AddDays(-5),
                    LastBillPostedOn = DateTime.Now.AddDays(-5),
                    LastPaymentPostedOn = DateTime.Now.AddDays(-12),
                    APControlAccountIdSnapshot = APControlAccountId,
                    APControlAccountCode = "2001",
                    APControlAccountName = "Accounts Payable",
                    CreatedAt = DateTime.Now.AddDays(-75),
                    CreatedBy = "System"
                },

                // VendorAccount 11: SecureGuard Services - Active
                new VendorAccountViewModel
                {
                    Id = VendorAccount11Id,
                    CompanyId = AscendingSoftwareCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                    VendorId = VendorSeedData.Vendor11Id,
                    VendorCode = "VND-000011",
                    VendorName = "SecureGuard Services Pvt Ltd",
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    AccountStatus = VendorAccountStatuses.Active,
                    OutstandingPayableAmount = 65000.00m,
                    AdvancePaidAmount = 10000.00m,
                    TotalBillsPostedAmount = 150000,
                    TotalPaymentsPostedAmount = 75000,
                    TotalCreditNotesPostedAmount = 0,
                    TotalDebitNotesPostedAmount = 10000,
                    TotalAdjustmentsPostedAmount = 0,
                    IsPaymentBlocked = false,
                    IsPostingBlocked = false,
                    LastTransactionOn = DateTime.Now.AddDays(-3),
                    LastBillPostedOn = DateTime.Now.AddDays(-8),
                    APControlAccountIdSnapshot = APControlAccountId,
                    APControlAccountCode = "2001",
                    APControlAccountName = "Accounts Payable",
                    CreatedAt = DateTime.Now.AddDays(-60),
                    CreatedBy = "System"
                },

                // VendorAccount 12: CloudNet Systems - Frozen
                new VendorAccountViewModel
                {
                    Id = VendorAccount12Id,
                    CompanyId = AscendingSoftwareCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                    VendorId = VendorSeedData.Vendor12Id,
                    VendorCode = "VND-000012",
                    VendorName = "CloudNet Systems India",
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    AccountStatus = VendorAccountStatuses.Frozen,
                    OutstandingPayableAmount = 0,
                    AdvancePaidAmount = 0,
                    TotalBillsPostedAmount = 250000,
                    TotalPaymentsPostedAmount = 250000,
                    TotalCreditNotesPostedAmount = 0,
                    TotalDebitNotesPostedAmount = 0,
                    TotalAdjustmentsPostedAmount = 0,
                    IsPaymentBlocked = true,
                    IsPostingBlocked = true,
                    LastTransactionOn = DateTime.Now.AddDays(-120),
                    LastBillPostedOn = DateTime.Now.AddDays(-150),
                    LastPaymentPostedOn = DateTime.Now.AddDays(-120),
                    APControlAccountIdSnapshot = APControlAccountId,
                    APControlAccountCode = "2001",
                    APControlAccountName = "Accounts Payable",
                    CreatedAt = DateTime.Now.AddDays(-200),
                    CreatedBy = "System"
                }
            };
        }
    }
}
