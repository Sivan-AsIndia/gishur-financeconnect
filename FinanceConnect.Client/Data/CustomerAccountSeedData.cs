using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    /// <summary>
    /// Seed data for CustomerAccount model
    /// </summary>
    public static class CustomerAccountSeedData
    {
        // Company GUIDs (matching MasterDataIds.Companies)
        private static readonly Guid SofaCraftCompanyId = MasterDataIds.Companies.SofaCraft;
        private static readonly Guid SofaCraftUSACompanyId = MasterDataIds.Companies.SofaCraftUSA;

        // Currency GUIDs (matching MasterDataIds.Currencies)
        private static readonly Guid InrCurrencyId = MasterDataIds.Currencies.INR;
        private static readonly Guid UsdCurrencyId = MasterDataIds.Currencies.USD;

        // Predefined CustomerAccount GUIDs
        public static readonly Guid CustomerAccount1Id = Guid.Parse("c6c6c6c6-0001-0001-0001-000000000001");
        public static readonly Guid CustomerAccount2Id = Guid.Parse("c6c6c6c6-0001-0001-0001-000000000002");
        public static readonly Guid CustomerAccount3Id = Guid.Parse("c6c6c6c6-0001-0001-0001-000000000003");
        public static readonly Guid CustomerAccount4Id = Guid.Parse("c6c6c6c6-0001-0001-0001-000000000004");
        public static readonly Guid CustomerAccount5Id = Guid.Parse("c6c6c6c6-0001-0001-0001-000000000005");
        public static readonly Guid CustomerAccount6Id = Guid.Parse("c6c6c6c6-0001-0001-0001-000000000006");
        public static readonly Guid CustomerAccount7Id = Guid.Parse("c6c6c6c6-0001-0001-0001-000000000007");
        public static readonly Guid CustomerAccount8Id = Guid.Parse("c6c6c6c6-0001-0001-0001-000000000008");
        public static readonly Guid CustomerAccount9Id = Guid.Parse("c6c6c6c6-0001-0001-0001-000000000009");
        public static readonly Guid CustomerAccount10Id = Guid.Parse("c6c6c6c6-0001-0001-0001-000000000010");
        public static readonly Guid CustomerAccount11Id = Guid.Parse("c6c6c6c6-0001-0001-0001-000000000011");
        public static readonly Guid CustomerAccount12Id = Guid.Parse("c6c6c6c6-0001-0001-0001-000000000012");

        public static List<CustomerAccountViewModel> GetSeedCustomerAccounts()
        {
            return new List<CustomerAccountViewModel>
            {
                // CustomerAccount 1: ABC Traders - Active with outstanding balance
                new CustomerAccountViewModel
                {
                    Id = CustomerAccount1Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    CustomerId = CustomerSeedData.Customer1Id,
                    CustomerCode = "CUST-001",
                    CustomerName = "ABC Traders Private Limited",
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    AccountStatus = CustomerAccountStatuses.Active,
                    OutstandingReceivableAmount = 125000.00m,
                    UnappliedPaymentAmount = 5000.00m,
                    AdvanceBalanceAmount = 0,
                    CreditLimitAmountSnapshot = 500000,
                    CreditLimitEnforcedSnapshot = true,
                    LastActivityOn = DateTime.Now.AddDays(-5),
                    LastInvoiceOn = DateTime.Now.AddDays(-10),
                    LastPaymentOn = DateTime.Now.AddDays(-5),
                    IsPostingBlocked = false,
                    FreezeType = FreezeTypes.None,
                    CollectionsStage = CollectionsStages.None,
                    RiskScore = 25,
                    CreatedAt = DateTime.Now.AddDays(-90),
                    CreatedBy = "System"
                },

                // CustomerAccount 2: Government - Large outstanding, no credit limit enforcement
                new CustomerAccountViewModel
                {
                    Id = CustomerAccount2Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    CustomerId = CustomerSeedData.Customer2Id,
                    CustomerCode = "CUST-002",
                    CustomerName = "State Government Education Dept",
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    AccountStatus = CustomerAccountStatuses.Active,
                    OutstandingReceivableAmount = 850000.00m,
                    UnappliedPaymentAmount = 0,
                    AdvanceBalanceAmount = 25000.00m,
                    CreditLimitAmountSnapshot = 1000000,
                    CreditLimitEnforcedSnapshot = false,
                    LastActivityOn = DateTime.Now.AddDays(-20),
                    LastInvoiceOn = DateTime.Now.AddDays(-30),
                    LastPaymentOn = DateTime.Now.AddDays(-20),
                    IsPostingBlocked = false,
                    FreezeType = FreezeTypes.None,
                    CollectionsStage = CollectionsStages.None,
                    RiskScore = 10,
                    CreatedAt = DateTime.Now.AddDays(-60),
                    CreatedBy = "System"
                },

                // CustomerAccount 3: XYZ Electronics - On Hold, posting blocked
                new CustomerAccountViewModel
                {
                    Id = CustomerAccount3Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    CustomerId = CustomerSeedData.Customer3Id,
                    CustomerCode = "CUST-003",
                    CustomerName = "XYZ Electronics Hub",
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    AccountStatus = CustomerAccountStatuses.Active,
                    OutstandingReceivableAmount = 185000.00m,
                    UnappliedPaymentAmount = 0,
                    AdvanceBalanceAmount = 0,
                    CreditLimitAmountSnapshot = 200000,
                    CreditLimitEnforcedSnapshot = true,
                    LastActivityOn = DateTime.Now.AddDays(-95),
                    LastInvoiceOn = DateTime.Now.AddDays(-95),
                    LastPaymentOn = DateTime.Now.AddDays(-120),
                    IsPostingBlocked = true,
                    PostingBlockReason = "Customer on credit hold due to overdue invoices exceeding 90 days",
                    PostingBlockedByUserId = MasterDataIds.Tenants.Default,
                    PostingBlockedByUserName = "Finance Controller",
                    PostingBlockedOn = DateTime.Now.AddDays(-15),
                    FreezeType = FreezeTypes.CollectionsHold,
                    CollectionsStage = CollectionsStages.FollowUp,
                    CollectionsStartedOn = DateTime.Now.AddDays(-60),
                    LastCollectionActionOn = DateTime.Now.AddDays(-5),
                    CollectionsNotes = "Multiple follow-up calls made. Customer promised payment by end of month.",
                    RiskScore = 85,
                    CreatedAt = DateTime.Now.AddDays(-120),
                    CreatedBy = "System"
                },

                // CustomerAccount 4: Individual customer - Zero balance
                new CustomerAccountViewModel
                {
                    Id = CustomerAccount4Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    CustomerId = CustomerSeedData.Customer4Id,
                    CustomerCode = "CUST-004",
                    CustomerName = "Rajesh Kumar",
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    AccountStatus = CustomerAccountStatuses.Active,
                    OutstandingReceivableAmount = 0,
                    UnappliedPaymentAmount = 0,
                    AdvanceBalanceAmount = 0,
                    CreditLimitAmountSnapshot = 0,
                    CreditLimitEnforcedSnapshot = true,
                    LastActivityOn = DateTime.Now.AddDays(-7),
                    LastInvoiceOn = DateTime.Now.AddDays(-7),
                    LastPaymentOn = DateTime.Now.AddDays(-7),
                    IsPostingBlocked = false,
                    FreezeType = FreezeTypes.None,
                    CollectionsStage = CollectionsStages.None,
                    RiskScore = 0,
                    CreatedAt = DateTime.Now.AddDays(-30),
                    CreatedBy = "System"
                },

                // CustomerAccount 5: SEZ Customer - With advance balance
                new CustomerAccountViewModel
                {
                    Id = CustomerAccount5Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    CustomerId = CustomerSeedData.Customer5Id,
                    CustomerCode = "CUST-005",
                    CustomerName = "TechPark SEZ Solutions Pvt Ltd",
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    AccountStatus = CustomerAccountStatuses.Active,
                    OutstandingReceivableAmount = 350000.00m,
                    UnappliedPaymentAmount = 0,
                    AdvanceBalanceAmount = 75000.00m,
                    CreditLimitAmountSnapshot = 750000,
                    CreditLimitEnforcedSnapshot = true,
                    LastActivityOn = DateTime.Now.AddDays(-3),
                    LastInvoiceOn = DateTime.Now.AddDays(-15),
                    LastPaymentOn = DateTime.Now.AddDays(-3),
                    IsPostingBlocked = false,
                    FreezeType = FreezeTypes.None,
                    CollectionsStage = CollectionsStages.None,
                    RiskScore = 20,
                    CreatedAt = DateTime.Now.AddDays(-45),
                    CreatedBy = "System"
                },

                // CustomerAccount 6: Inactive customer - Frozen account
                new CustomerAccountViewModel
                {
                    Id = CustomerAccount6Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    CustomerId = CustomerSeedData.Customer6Id,
                    CustomerCode = "CUST-006",
                    CustomerName = "Discontinued Enterprises",
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    AccountStatus = CustomerAccountStatuses.Frozen,
                    OutstandingReceivableAmount = 15000.00m,
                    UnappliedPaymentAmount = 0,
                    AdvanceBalanceAmount = 0,
                    CreditLimitAmountSnapshot = 100000,
                    CreditLimitEnforcedSnapshot = true,
                    LastActivityOn = DateTime.Now.AddDays(-180),
                    LastInvoiceOn = DateTime.Now.AddDays(-200),
                    LastPaymentOn = DateTime.Now.AddDays(-180),
                    IsPostingBlocked = true,
                    PostingBlockReason = "Customer marked as inactive - business discontinued",
                    PostingBlockedByUserId = MasterDataIds.Tenants.Default,
                    PostingBlockedByUserName = "Finance Controller",
                    PostingBlockedOn = DateTime.Now.AddDays(-30),
                    FreezeType = FreezeTypes.Manual,
                    CollectionsStage = CollectionsStages.Legal,
                    CollectionsStartedOn = DateTime.Now.AddDays(-150),
                    LastCollectionActionOn = DateTime.Now.AddDays(-20),
                    CollectionsNotes = "Escalated to legal department. Case pending with external legal counsel. Business operations discontinued.",
                    RiskScore = 95,
                    CreatedAt = DateTime.Now.AddDays(-180),
                    CreatedBy = "System"
                },

                // CustomerAccount 7: USD Partner - Foreign currency account
                new CustomerAccountViewModel
                {
                    Id = CustomerAccount7Id,
                    CompanyId = SofaCraftUSACompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                    CustomerId = CustomerSeedData.Customer7Id,
                    CustomerCode = "CUST-007",
                    CustomerName = "Silicon Valley Partners LLC",
                    CurrencyId = UsdCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.USD),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.USD),
                    AccountStatus = CustomerAccountStatuses.Active,
                    OutstandingReceivableAmount = 45000.00m,
                    UnappliedPaymentAmount = 2500.00m,
                    AdvanceBalanceAmount = 0,
                    CreditLimitAmountSnapshot = 100000,
                    CreditLimitEnforcedSnapshot = true,
                    LastActivityOn = DateTime.Now.AddDays(-2),
                    LastInvoiceOn = DateTime.Now.AddDays(-12),
                    LastPaymentOn = DateTime.Now.AddDays(-2),
                    IsPostingBlocked = false,
                    FreezeType = FreezeTypes.None,
                    CollectionsStage = CollectionsStages.None,
                    RiskScore = 15,
                    CreatedAt = DateTime.Now.AddDays(-75),
                    CreatedBy = "System"
                },

                // CustomerAccount 8: Draft customer - New account with opening balance
                new CustomerAccountViewModel
                {
                    Id = CustomerAccount8Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    CustomerId = CustomerSeedData.Customer8Id,
                    CustomerCode = "CUST-008",
                    CustomerName = "New Customer Pending Approval",
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    AccountStatus = CustomerAccountStatuses.Active,
                    OutstandingReceivableAmount = 0,
                    UnappliedPaymentAmount = 0,
                    AdvanceBalanceAmount = 0,
                    CreditLimitAmountSnapshot = 150000,
                    CreditLimitEnforcedSnapshot = true,
                    OpeningReceivableAmount = 0,
                    OpeningAdvanceAmount = 0,
                    LastActivityOn = null,
                    LastInvoiceOn = null,
                    LastPaymentOn = null,
                    IsPostingBlocked = false,
                    FreezeType = FreezeTypes.None,
                    CollectionsStage = CollectionsStages.None,
                    RiskScore = 0,
                    CreatedAt = DateTime.Now.AddDays(-2),
                    CreatedBy = "System"
                },

                // CustomerAccount 9: Metro Retail - Large retail chain with multiple invoices
                new CustomerAccountViewModel
                {
                    Id = CustomerAccount9Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    CustomerId = CustomerSeedData.Customer9Id,
                    CustomerCode = "CUST-009",
                    CustomerName = "Metro Retail Stores Ltd",
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    AccountStatus = CustomerAccountStatuses.Active,
                    OutstandingReceivableAmount = 425000.00m,
                    UnappliedPaymentAmount = 15000.00m,
                    AdvanceBalanceAmount = 50000.00m,
                    CreditLimitAmountSnapshot = 1500000,
                    CreditLimitEnforcedSnapshot = true,
                    LastActivityOn = DateTime.Now.AddDays(-4),
                    LastInvoiceOn = DateTime.Now.AddDays(-8),
                    LastPaymentOn = DateTime.Now.AddDays(-4),
                    IsPostingBlocked = false,
                    FreezeType = FreezeTypes.None,
                    CollectionsStage = CollectionsStages.None,
                    RiskScore = 30,
                    CreatedAt = DateTime.Now.AddDays(-100),
                    CreatedBy = "System"
                },

                // CustomerAccount 10: Apollo Medical Center - Healthcare with good payment history
                new CustomerAccountViewModel
                {
                    Id = CustomerAccount10Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    CustomerId = CustomerSeedData.Customer10Id,
                    CustomerCode = "CUST-010",
                    CustomerName = "Apollo Medical Center",
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    AccountStatus = CustomerAccountStatuses.Active,
                    OutstandingReceivableAmount = 275000.00m,
                    UnappliedPaymentAmount = 10000.00m,
                    AdvanceBalanceAmount = 0,
                    CreditLimitAmountSnapshot = 800000,
                    CreditLimitEnforcedSnapshot = true,
                    LastActivityOn = DateTime.Now.AddDays(-6),
                    LastInvoiceOn = DateTime.Now.AddDays(-14),
                    LastPaymentOn = DateTime.Now.AddDays(-6),
                    IsPostingBlocked = false,
                    FreezeType = FreezeTypes.None,
                    CollectionsStage = CollectionsStages.None,
                    RiskScore = 18,
                    CreatedAt = DateTime.Now.AddDays(-55),
                    CreatedBy = "System"
                },

                // CustomerAccount 11: Precision Engineering - Manufacturing company under temporary hold
                new CustomerAccountViewModel
                {
                    Id = CustomerAccount11Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    CustomerId = CustomerSeedData.Customer11Id,
                    CustomerCode = "CUST-011",
                    CustomerName = "Precision Engineering Works",
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    AccountStatus = CustomerAccountStatuses.Active,
                    OutstandingReceivableAmount = 195000.00m,
                    UnappliedPaymentAmount = 0,
                    AdvanceBalanceAmount = 0,
                    CreditLimitAmountSnapshot = 600000,
                    CreditLimitEnforcedSnapshot = true,
                    LastActivityOn = DateTime.Now.AddDays(-18),
                    LastInvoiceOn = DateTime.Now.AddDays(-25),
                    LastPaymentOn = DateTime.Now.AddDays(-35),
                    IsPostingBlocked = true,
                    PostingBlockReason = "Temporary hold - pending document verification",
                    PostingBlockedByUserId = MasterDataIds.Tenants.Default,
                    PostingBlockedByUserName = "AR Manager",
                    PostingBlockedOn = DateTime.Now.AddDays(-3),
                    FreezeType = FreezeTypes.DisputeHold,
                    CollectionsStage = CollectionsStages.None,
                    RiskScore = 45,
                    CreatedAt = DateTime.Now.AddDays(-40),
                    CreatedBy = "System"
                },

                // CustomerAccount 12: Sunrise International School - Educational institution
                new CustomerAccountViewModel
                {
                    Id = CustomerAccount12Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    CustomerId = CustomerSeedData.Customer12Id,
                    CustomerCode = "CUST-012",
                    CustomerName = "Sunrise International School",
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    AccountStatus = CustomerAccountStatuses.Active,
                    OutstandingReceivableAmount = 145000.00m,
                    UnappliedPaymentAmount = 5000.00m,
                    AdvanceBalanceAmount = 20000.00m,
                    CreditLimitAmountSnapshot = 300000,
                    CreditLimitEnforcedSnapshot = true,
                    LastActivityOn = DateTime.Now.AddDays(-9),
                    LastInvoiceOn = DateTime.Now.AddDays(-20),
                    LastPaymentOn = DateTime.Now.AddDays(-9),
                    IsPostingBlocked = false,
                    FreezeType = FreezeTypes.None,
                    CollectionsStage = CollectionsStages.None,
                    RiskScore = 22,
                    CreatedAt = DateTime.Now.AddDays(-25),
                    CreatedBy = "System"
                }
            };
        }
    }
}
