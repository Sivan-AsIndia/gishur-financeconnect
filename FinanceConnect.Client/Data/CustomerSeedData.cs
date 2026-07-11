using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    /// <summary>
    /// Seed data for Customer model
    /// </summary>
    public static class CustomerSeedData
    {
        // Company GUIDs (matching MasterDataIds.Companies)
        private static readonly Guid SofaCraftCompanyId = MasterDataIds.Companies.SofaCraft;
        private static readonly Guid SofaCraftUSACompanyId = MasterDataIds.Companies.SofaCraftUSA;

        // Currency GUIDs (matching MasterDataIds.Currencies)
        private static readonly Guid InrCurrencyId = MasterDataIds.Currencies.INR;
        private static readonly Guid UsdCurrencyId = MasterDataIds.Currencies.USD;

        // Predefined Customer GUIDs
        public static readonly Guid Customer1Id = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000001");
        public static readonly Guid Customer2Id = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000002");
        public static readonly Guid Customer3Id = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000003");
        public static readonly Guid Customer4Id = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000004");
        public static readonly Guid Customer5Id = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000005");
        public static readonly Guid Customer6Id = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000006");
        public static readonly Guid Customer7Id = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000007");
        public static readonly Guid Customer8Id = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000008");
        public static readonly Guid Customer9Id = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000009");
        public static readonly Guid Customer10Id = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000010");
        public static readonly Guid Customer11Id = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000011");
        public static readonly Guid Customer12Id = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000012");

        // Tax Profile GUIDs
        private static readonly Guid DefaultTaxProfileId = MasterDataIds.Tenants.Default;
        private static readonly Guid SezTaxProfileId = MasterDataIds.PaymentTerms.Net45;

        // Payment Term GUIDs (matching MasterDataIds.PaymentTerms)
        private static readonly Guid Net30TermId = MasterDataIds.PaymentTerms.Net30;
        private static readonly Guid Net45TermId = MasterDataIds.PaymentTerms.Net45;
        private static readonly Guid Net60TermId = MasterDataIds.PaymentTerms.Net60;
        private static readonly Guid ImmediateTermId = MasterDataIds.PaymentTerms.Immediate;

        // GL Account GUIDs (matching COADataService accounts)
        private static readonly Guid ReceivableAccountId = MasterDataIds.Accounts.AccountsReceivable;
        private static readonly Guid AdvanceAccountId = MasterDataIds.Accounts.GSTPayable;
        private static readonly Guid WriteOffAccountId = MasterDataIds.Accounts.RentExpense;

        public static List<CustomerViewModel> GetSeedCustomers()
        {
            return new List<CustomerViewModel>
            {
                // Customer 1: Active B2B customer with GST registration
                new CustomerViewModel
                {
                    Id = Customer1Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    CustomerCode = "CUST-001",
                    CustomerName = "ABC Traders Private Limited",
                    CustomerType = CustomerTypes.Business,
                    CustomerStatus = CustomerStatuses.Active,
                    PrimaryEmail = "accounts@abctraders.com",
                    PrimaryPhone = "+91-9876543210",
                    Website = "https://www.abctraders.com",
                    TaxRegistrationType = TaxRegistrationTypes.Registered,
                    GSTIN = "33AABCA1234A1ZA",
                    PAN = "AABCA1234A",
                    TaxProfileId = DefaultTaxProfileId,
                    TaxProfileName = "Standard GST",
                    IsTDSApplicable = false,
                    CreditLimitAmount = 500000,
                    CreditLimitEnforced = true,
                    CreditHoldStatus = CreditHoldStatuses.None,
                    PaymentTermId = Net30TermId,
                    PaymentTermName = "Net 30 Days",
                    DefaultCurrencyId = InrCurrencyId,
                    DefaultCurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    DefaultCurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    DefaultPaymentMethod = PaymentMethods.BankTransfer,
                    ReceivableAccountId = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsReceivable),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsReceivable),
                    AdvanceFromCustomerAccountId = AdvanceAccountId,
                    AdvanceFromCustomerAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.GSTPayable),
                    AdvanceFromCustomerAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.GSTPayable),
                    AllowAutoAdvanceCreation = true,
                    SendInvoiceEmail = true,
                    CustomerStatementCycle = StatementCycles.Monthly,
                    AllowPartialPayment = true,
                    AllowOverPayment = false,
                    CreatedAt = DateTime.Now.AddDays(-90),
                    CreatedBy = "System"
                },

                // Customer 2: Active Government customer
                new CustomerViewModel
                {
                    Id = Customer2Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    CustomerCode = "CUST-002",
                    CustomerName = "State Government Education Dept",
                    CustomerType = CustomerTypes.Government,
                    CustomerStatus = CustomerStatuses.Active,
                    PrimaryEmail = "procurement@stategov.edu.in",
                    PrimaryPhone = "+91-4428765432",
                    TaxRegistrationType = TaxRegistrationTypes.Unregistered,
                    TaxProfileId = DefaultTaxProfileId,
                    TaxProfileName = "Standard GST",
                    IsTDSApplicable = true,
                    TDSSectionCode = "194C",
                    CreditLimitAmount = 1000000,
                    CreditLimitEnforced = false,
                    CreditHoldStatus = CreditHoldStatuses.None,
                    PaymentTermId = Net60TermId,
                    PaymentTermName = "Net 60 Days",
                    DefaultCurrencyId = InrCurrencyId,
                    DefaultCurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    DefaultCurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    DefaultPaymentMethod = PaymentMethods.Cheque,
                    ReceivableAccountId = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsReceivable),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsReceivable),
                    SendInvoiceEmail = true,
                    CustomerStatementCycle = StatementCycles.Quarterly,
                    AllowPartialPayment = true,
                    AllowOverPayment = false,
                    CreatedAt = DateTime.Now.AddDays(-60),
                    CreatedBy = "System"
                },

                // Customer 3: Customer on Credit Hold
                new CustomerViewModel
                {
                    Id = Customer3Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    CustomerCode = "CUST-003",
                    CustomerName = "XYZ Electronics Hub",
                    CustomerType = CustomerTypes.Business,
                    CustomerStatus = CustomerStatuses.Active,
                    PrimaryEmail = "finance@xyzelectronics.in",
                    PrimaryPhone = "+91-9988776655",
                    TaxRegistrationType = TaxRegistrationTypes.Registered,
                    GSTIN = "33AABCX5678B1ZB",
                    PAN = "AABCX5678B",
                    TaxProfileId = DefaultTaxProfileId,
                    TaxProfileName = "Standard GST",
                    CreditLimitAmount = 200000,
                    CreditLimitEnforced = true,
                    CreditHoldStatus = CreditHoldStatuses.OnHold,
                    CreditHoldReason = "Outstanding payments overdue > 60 days",
                    PaymentTermId = Net30TermId,
                    PaymentTermName = "Net 30 Days",
                    DefaultCurrencyId = InrCurrencyId,
                    DefaultCurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    DefaultCurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    DefaultPaymentMethod = PaymentMethods.BankTransfer,
                    ReceivableAccountId = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsReceivable),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsReceivable),
                    SendInvoiceEmail = true,
                    CustomerStatementCycle = StatementCycles.Monthly,
                    AllowPartialPayment = true,
                    CreatedAt = DateTime.Now.AddDays(-120),
                    CreatedBy = "System",
                    UpdatedAt = DateTime.Now.AddDays(-5),
                    UpdatedBy = "AR Manager"
                },

                // Customer 4: Individual customer with cash sales
                new CustomerViewModel
                {
                    Id = Customer4Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    CustomerCode = "CUST-004",
                    CustomerName = "Rajesh Kumar",
                    CustomerType = CustomerTypes.Individual,
                    CustomerStatus = CustomerStatuses.Active,
                    PrimaryEmail = "rajesh.kumar@email.com",
                    PrimaryPhone = "+91-9123456789",
                    TaxRegistrationType = TaxRegistrationTypes.Unregistered,
                    TaxProfileId = DefaultTaxProfileId,
                    TaxProfileName = "Standard GST",
                    CreditLimitAmount = 50000,
                    CreditLimitEnforced = true,
                    CreditHoldStatus = CreditHoldStatuses.None,
                    PaymentTermId = ImmediateTermId,
                    PaymentTermName = "Immediate Payment",
                    DefaultCurrencyId = InrCurrencyId,
                    DefaultCurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    DefaultCurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    DefaultPaymentMethod = PaymentMethods.Cash,
                    ReceivableAccountId = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsReceivable),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsReceivable),
                    SendInvoiceEmail = false,
                    CustomerStatementCycle = StatementCycles.OnDemand,
                    AllowPartialPayment = false,
                    CreatedAt = DateTime.Now.AddDays(-30),
                    CreatedBy = "System"
                },

                // Customer 5: SEZ customer
                new CustomerViewModel
                {
                    Id = Customer5Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    CustomerCode = "CUST-005",
                    CustomerName = "TechPark SEZ Solutions Pvt Ltd",
                    CustomerType = CustomerTypes.Business,
                    CustomerStatus = CustomerStatuses.Active,
                    PrimaryEmail = "billing@techparksez.com",
                    PrimaryPhone = "+91-4027654321",
                    Website = "https://www.techparksez.com",
                    TaxRegistrationType = TaxRegistrationTypes.SEZ,
                    GSTIN = "36AABCT9012C1ZC",
                    PAN = "AABCT9012C",
                    TaxProfileId = SezTaxProfileId,
                    TaxProfileName = "SEZ Zero Rated",
                    CreditLimitAmount = 750000,
                    CreditLimitEnforced = true,
                    CreditHoldStatus = CreditHoldStatuses.None,
                    PaymentTermId = Net45TermId,
                    PaymentTermName = "Net 45 Days",
                    DefaultCurrencyId = InrCurrencyId,
                    DefaultCurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    DefaultCurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    DefaultPaymentMethod = PaymentMethods.BankTransfer,
                    ReceivableAccountId = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsReceivable),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsReceivable),
                    AllowAutoAdvanceCreation = true,
                    SendInvoiceEmail = true,
                    CustomerStatementCycle = StatementCycles.Monthly,
                    AllowPartialPayment = true,
                    CreatedAt = DateTime.Now.AddDays(-45),
                    CreatedBy = "System"
                },

                // Customer 6: Inactive customer
                new CustomerViewModel
                {
                    Id = Customer6Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    CustomerCode = "CUST-006",
                    CustomerName = "Discontinued Enterprises",
                    CustomerType = CustomerTypes.Business,
                    CustomerStatus = CustomerStatuses.Inactive,
                    PrimaryEmail = "closed@discontinued.com",
                    TaxRegistrationType = TaxRegistrationTypes.Registered,
                    GSTIN = "33AABCD3456D1ZD",
                    PAN = "AABCD3456D",
                    TaxProfileId = DefaultTaxProfileId,
                    TaxProfileName = "Standard GST",
                    CreditLimitAmount = 100000,
                    CreditLimitEnforced = true,
                    CreditHoldStatus = CreditHoldStatuses.None,
                    PaymentTermId = Net30TermId,
                    PaymentTermName = "Net 30 Days",
                    DefaultCurrencyId = InrCurrencyId,
                    DefaultCurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    DefaultCurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    DefaultPaymentMethod = PaymentMethods.BankTransfer,
                    ReceivableAccountId = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsReceivable),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsReceivable),
                    SendInvoiceEmail = false,
                    CustomerStatementCycle = StatementCycles.OnDemand,
                    CreatedAt = DateTime.Now.AddDays(-180),
                    CreatedBy = "System",
                    UpdatedAt = DateTime.Now.AddDays(-30),
                    UpdatedBy = "Finance Controller"
                },

                // Customer 7: Partner customer (USD currency)
                new CustomerViewModel
                {
                    Id = Customer7Id,
                    CompanyId = SofaCraftUSACompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                    CustomerCode = "CUST-007",
                    CustomerName = "Silicon Valley Partners LLC",
                    CustomerType = CustomerTypes.Partner,
                    CustomerStatus = CustomerStatuses.Active,
                    PrimaryEmail = "ap@svpartners.com",
                    PrimaryPhone = "+1-650-555-0123",
                    Website = "https://www.svpartners.com",
                    TaxRegistrationType = TaxRegistrationTypes.Export,
                    TaxProfileId = DefaultTaxProfileId,
                    TaxProfileName = "Export Zero Rated",
                    CreditLimitAmount = 100000,
                    CreditLimitEnforced = true,
                    CreditHoldStatus = CreditHoldStatuses.None,
                    PaymentTermId = Net30TermId,
                    PaymentTermName = "Net 30 Days",
                    DefaultCurrencyId = UsdCurrencyId,
                    DefaultCurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.USD),
                    DefaultCurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.USD),
                    DefaultPaymentMethod = PaymentMethods.BankTransfer,
                    ReceivableAccountId = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsReceivable),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsReceivable),
                    AllowAutoAdvanceCreation = true,
                    SendInvoiceEmail = true,
                    CustomerStatementCycle = StatementCycles.Monthly,
                    AllowPartialPayment = true,
                    AllowOverPayment = true,
                    PreferredLanguage = "EN",
                    CreatedAt = DateTime.Now.AddDays(-75),
                    CreatedBy = "System"
                },

                // Customer 8: Draft customer (pending approval)
                new CustomerViewModel
                {
                    Id = Customer8Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    CustomerCode = "CUST-008",
                    CustomerName = "New Customer Pending Approval",
                    CustomerType = CustomerTypes.Business,
                    CustomerStatus = CustomerStatuses.Draft,
                    PrimaryEmail = "contact@newcustomer.com",
                    PrimaryPhone = "+91-9123456789",
                    TaxRegistrationType = TaxRegistrationTypes.Registered,
                    GSTIN = "33AABCN7890E1ZE",
                    PAN = "AABCN7890E",
                    TaxProfileId = DefaultTaxProfileId,
                    TaxProfileName = "Standard GST",
                    CreditLimitAmount = 150000,
                    CreditLimitEnforced = true,
                    CreditHoldStatus = CreditHoldStatuses.None,
                    PaymentTermId = Net30TermId,
                    PaymentTermName = "Net 30 Days",
                    DefaultCurrencyId = InrCurrencyId,
                    DefaultCurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    DefaultCurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    DefaultPaymentMethod = PaymentMethods.BankTransfer,
                    ReceivableAccountId = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsReceivable),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsReceivable),
                    SendInvoiceEmail = true,
                    CustomerStatementCycle = StatementCycles.Monthly,
                    AllowPartialPayment = true,
                    CreatedAt = DateTime.Now.AddDays(-2),
                    CreatedBy = "AR Clerk"
                },

                // Customer 9: Retail chain customer
                new CustomerViewModel
                {
                    Id = Customer9Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    CustomerCode = "CUST-009",
                    CustomerName = "Metro Retail Stores Ltd",
                    CustomerType = CustomerTypes.Business,
                    CustomerStatus = CustomerStatuses.Active,
                    PrimaryEmail = "procurement@metroretail.in",
                    PrimaryPhone = "+91-8087654321",
                    Website = "https://www.metroretail.in",
                    TaxRegistrationType = TaxRegistrationTypes.Registered,
                    GSTIN = "29AABCM4567F1ZF",
                    PAN = "AABCM4567F",
                    TaxProfileId = DefaultTaxProfileId,
                    TaxProfileName = "Standard GST",
                    CreditLimitAmount = 1500000,
                    CreditLimitEnforced = true,
                    CreditHoldStatus = CreditHoldStatuses.None,
                    PaymentTermId = Net45TermId,
                    PaymentTermName = "Net 45 Days",
                    DefaultCurrencyId = InrCurrencyId,
                    DefaultCurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    DefaultCurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    DefaultPaymentMethod = PaymentMethods.BankTransfer,
                    ReceivableAccountId = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsReceivable),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsReceivable),
                    AllowAutoAdvanceCreation = true,
                    SendInvoiceEmail = true,
                    CustomerStatementCycle = StatementCycles.Monthly,
                    AllowPartialPayment = true,
                    AllowOverPayment = false,
                    CreatedAt = DateTime.Now.AddDays(-100),
                    CreatedBy = "System"
                },

                // Customer 10: Healthcare institution
                new CustomerViewModel
                {
                    Id = Customer10Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    CustomerCode = "CUST-010",
                    CustomerName = "Apollo Medical Center",
                    CustomerType = CustomerTypes.Business,
                    CustomerStatus = CustomerStatuses.Active,
                    PrimaryEmail = "accounts@apollomedi.in",
                    PrimaryPhone = "+91-4426789012",
                    Website = "https://www.apollomedi.in",
                    TaxRegistrationType = TaxRegistrationTypes.Registered,
                    GSTIN = "33AABCH8901G1ZG",
                    PAN = "AABCH8901G",
                    TaxProfileId = DefaultTaxProfileId,
                    TaxProfileName = "Standard GST",
                    CreditLimitAmount = 800000,
                    CreditLimitEnforced = true,
                    CreditHoldStatus = CreditHoldStatuses.None,
                    PaymentTermId = Net30TermId,
                    PaymentTermName = "Net 30 Days",
                    DefaultCurrencyId = InrCurrencyId,
                    DefaultCurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    DefaultCurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    DefaultPaymentMethod = PaymentMethods.BankTransfer,
                    ReceivableAccountId = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsReceivable),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsReceivable),
                    AllowAutoAdvanceCreation = true,
                    SendInvoiceEmail = true,
                    CustomerStatementCycle = StatementCycles.Monthly,
                    AllowPartialPayment = true,
                    CreatedAt = DateTime.Now.AddDays(-55),
                    CreatedBy = "System"
                },

                // Customer 11: Manufacturing company with temporary hold
                new CustomerViewModel
                {
                    Id = Customer11Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    CustomerCode = "CUST-011",
                    CustomerName = "Precision Engineering Works",
                    CustomerType = CustomerTypes.Business,
                    CustomerStatus = CustomerStatuses.Active,
                    PrimaryEmail = "finance@precisioneng.in",
                    PrimaryPhone = "+91-9876541230",
                    Website = "https://www.precisioneng.in",
                    TaxRegistrationType = TaxRegistrationTypes.Registered,
                    GSTIN = "33AABCP2345H1ZH",
                    PAN = "AABCP2345H",
                    TaxProfileId = DefaultTaxProfileId,
                    TaxProfileName = "Standard GST",
                    CreditLimitAmount = 600000,
                    CreditLimitEnforced = true,
                    CreditHoldStatus = CreditHoldStatuses.TemporaryHold,
                    CreditHoldReason = "Account under review - pending documentation",
                    PaymentTermId = Net30TermId,
                    PaymentTermName = "Net 30 Days",
                    DefaultCurrencyId = InrCurrencyId,
                    DefaultCurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    DefaultCurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    DefaultPaymentMethod = PaymentMethods.BankTransfer,
                    ReceivableAccountId = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsReceivable),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsReceivable),
                    AllowAutoAdvanceCreation = true,
                    SendInvoiceEmail = true,
                    CustomerStatementCycle = StatementCycles.Monthly,
                    AllowPartialPayment = true,
                    CreatedAt = DateTime.Now.AddDays(-40),
                    CreatedBy = "System",
                    UpdatedAt = DateTime.Now.AddDays(-3),
                    UpdatedBy = "AR Manager"
                },

                // Customer 12: Educational institution
                new CustomerViewModel
                {
                    Id = Customer12Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    CustomerCode = "CUST-012",
                    CustomerName = "Sunrise International School",
                    CustomerType = CustomerTypes.Business,
                    CustomerStatus = CustomerStatuses.Active,
                    PrimaryEmail = "admin@sunriseschool.edu.in",
                    PrimaryPhone = "+91-4428901234",
                    Website = "https://www.sunriseschool.edu.in",
                    TaxRegistrationType = TaxRegistrationTypes.Registered,
                    GSTIN = "33AABCS6789I1ZI",
                    PAN = "AABCS6789I",
                    TaxProfileId = DefaultTaxProfileId,
                    TaxProfileName = "Standard GST",
                    CreditLimitAmount = 300000,
                    CreditLimitEnforced = true,
                    CreditHoldStatus = CreditHoldStatuses.None,
                    PaymentTermId = Net45TermId,
                    PaymentTermName = "Net 45 Days",
                    DefaultCurrencyId = InrCurrencyId,
                    DefaultCurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    DefaultCurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    DefaultPaymentMethod = PaymentMethods.Cheque,
                    ReceivableAccountId = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsReceivable),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsReceivable),
                    AllowAutoAdvanceCreation = false,
                    SendInvoiceEmail = true,
                    CustomerStatementCycle = StatementCycles.Quarterly,
                    AllowPartialPayment = true,
                    CreatedAt = DateTime.Now.AddDays(-25),
                    CreatedBy = "System"
                }
            };
        }
    }
}
