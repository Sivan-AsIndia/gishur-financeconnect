using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    /// <summary>
    /// Seed data for CustomerPayment model (Model #29)
    /// </summary>
    public static class CustomerPaymentSeedData
    {
        // Company GUIDs (matching MasterDataIds.Companies)
        private static readonly Guid SofaCraftCompanyId = MasterDataIds.Companies.SofaCraft;

        // Branch GUIDs (matching MasterDataIds.Branches)
        private static readonly Guid ChennaiHQBranchId = MasterDataIds.Branches.SofaCraftHQ;
        private static readonly Guid BangaloreBranchId = MasterDataIds.Branches.SofaCraftBengaluru;

        // Customer GUIDs (matching CustomerSeedData)
        private static readonly Guid Customer1Id = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000001");
        private static readonly Guid Customer2Id = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000002");
        private static readonly Guid Customer3Id = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000003");

        // Currency GUIDs (matching MasterDataIds.Currencies)
        private static readonly Guid InrCurrencyId = MasterDataIds.Currencies.INR;

        // GL Account GUIDs (matching COADataService accounts)
        private static readonly Guid ReceivableAccountId = MasterDataIds.Accounts.AccountsReceivable;
        private static readonly Guid CashAccountId = MasterDataIds.Accounts.PettyCash;
        private static readonly Guid BankAccountId = MasterDataIds.Accounts.HDFCBankAccount;
        private static readonly Guid AdvanceFromCustomerAccountId = MasterDataIds.Accounts.TDSPayable;

        // Invoice GUIDs (matching CustomerInvoiceSeedData)
        private static readonly Guid Invoice1Id = Guid.Parse("a1a1a1a1-0001-0001-0001-000000000001");
        private static readonly Guid Invoice2Id = Guid.Parse("a1a1a1a1-0001-0001-0001-000000000002");
        private static readonly Guid Invoice3Id = Guid.Parse("a1a1a1a1-0001-0001-0001-000000000003");
        private static readonly Guid Invoice4Id = Guid.Parse("a1a1a1a1-0001-0001-0001-000000000004");
        private static readonly Guid Invoice5Id = Guid.Parse("a1a1a1a1-0001-0001-0001-000000000005");

        // Predefined Payment GUIDs
        public static readonly Guid Payment1Id = Guid.Parse("e3e3e3e3-0001-0001-0001-000000000001");
        public static readonly Guid Payment2Id = Guid.Parse("e3e3e3e3-0001-0001-0001-000000000002");
        public static readonly Guid Payment3Id = Guid.Parse("e3e3e3e3-0001-0001-0001-000000000003");
        public static readonly Guid Payment4Id = Guid.Parse("e3e3e3e3-0001-0001-0001-000000000004");
        public static readonly Guid Payment5Id = Guid.Parse("e3e3e3e3-0001-0001-0001-000000000005");
        public static readonly Guid Payment6Id = Guid.Parse("e3e3e3e3-0001-0001-0001-000000000006");
        public static readonly Guid Payment7Id = Guid.Parse("e3e3e3e3-0001-0001-0001-000000000007");

        public static List<CustomerPaymentViewModel> GetSeedPayments()
        {
            var payments = new List<CustomerPaymentViewModel>
            {
                // Payment 1: Posted - Full payment via Bank Transfer (NEFT)
                new CustomerPaymentViewModel
                {
                    Id = Payment1Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    CustomerId = Customer1Id,
                    CustomerCode = "CUST-001",
                    CustomerName = "ABC Traders Private Limited",
                    ReceiptNumber = "RCP-2024-0001",
                    ReceiptDate = DateTime.Today.AddDays(-25),
                    PostingDate = DateTime.Today.AddDays(-25),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    PaymentNarration = "Payment received for Invoice INV-2024-0001 via NEFT",
                    PaymentMethod = PaymentMethods.BankTransfer,
                    PaymentAccountId = BankAccountId,
                    PaymentAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    PaymentAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    InstrumentDate = DateTime.Today.AddDays(-25),
                    InstrumentNumber = "HDFC24050000012345",
                    BankName = "HDFC Bank",
                    PaymentAmountTotal = 50000.00m,
                    AllocatedAmountTotal = 50000.00m,
                    UnallocatedAmountTotal = 0.00m,
                    AdvanceAmountTotal = 0.00m,
                    PaymentStatus = PaymentStatuses.Posted,
                    PostedOn = DateTime.Today.AddDays(-25),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedBy = "Finance Controller",
                    ReceivableAccountIdSnapshot = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    CreatedAt = DateTime.Today.AddDays(-26),
                    CreatedBy = "AR Clerk",
                    Allocations = new List<CustomerPaymentAllocationViewModel>
                    {
                        new CustomerPaymentAllocationViewModel
                        {
                            Id = Guid.NewGuid(),
                            CustomerPaymentId = Payment1Id,
                            CustomerInvoiceId = Invoice1Id,
                            InvoiceNumber = "INV-2024-0001",
                            InvoiceDate = DateTime.Today.AddDays(-45),
                            DueDate = DateTime.Today.AddDays(-15),
                            InvoiceOutstanding = 50000.00m,
                            AllocatedAmount = 50000.00m,
                            CreatedAt = DateTime.Today.AddDays(-26),
                            CreatedBy = "AR Clerk"
                        }
                    }
                },

                // Payment 2: Posted - Partial payment via Cheque
                new CustomerPaymentViewModel
                {
                    Id = Payment2Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    CustomerId = Customer2Id,
                    CustomerCode = "CUST-002",
                    CustomerName = "State Government Education Dept",
                    ReceiptNumber = "RCP-2024-0002",
                    ReceiptDate = DateTime.Today.AddDays(-20),
                    PostingDate = DateTime.Today.AddDays(-20),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    PaymentNarration = "Partial payment via cheque for Invoice INV-2024-0002",
                    PaymentMethod = PaymentMethods.Cheque,
                    PaymentAccountId = BankAccountId,
                    PaymentAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    PaymentAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    InstrumentDate = DateTime.Today.AddDays(-20),
                    InstrumentNumber = "CHQ-567890",
                    BankName = "ICICI Bank",
                    BankAccountLast4 = "4567",
                    PaymentAmountTotal = 30000.00m,
                    AllocatedAmountTotal = 30000.00m,
                    UnallocatedAmountTotal = 0.00m,
                    AdvanceAmountTotal = 0.00m,
                    PaymentStatus = PaymentStatuses.Posted,
                    PostedOn = DateTime.Today.AddDays(-20),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedBy = "Finance Controller",
                    ReceivableAccountIdSnapshot = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    CreatedAt = DateTime.Today.AddDays(-21),
                    CreatedBy = "AR Clerk",
                    Allocations = new List<CustomerPaymentAllocationViewModel>
                    {
                        new CustomerPaymentAllocationViewModel
                        {
                            Id = Guid.NewGuid(),
                            CustomerPaymentId = Payment2Id,
                            CustomerInvoiceId = Invoice2Id,
                            InvoiceNumber = "INV-2024-0002",
                            InvoiceDate = DateTime.Today.AddDays(-40),
                            DueDate = DateTime.Today.AddDays(-10),
                            InvoiceOutstanding = 75000.00m,
                            AllocatedAmount = 30000.00m,
                            CreatedAt = DateTime.Today.AddDays(-21),
                            CreatedBy = "AR Clerk"
                        }
                    }
                },

                // Payment 3: Posted - Cash payment
                new CustomerPaymentViewModel
                {
                    Id = Payment3Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    CustomerId = Customer1Id,
                    CustomerCode = "CUST-001",
                    CustomerName = "ABC Traders Private Limited",
                    ReceiptNumber = "RCP-2024-0003",
                    ReceiptDate = DateTime.Today.AddDays(-15),
                    PostingDate = DateTime.Today.AddDays(-15),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    PaymentNarration = "Cash payment received at counter",
                    PaymentMethod = PaymentMethods.Cash,
                    PaymentAccountId = CashAccountId,
                    PaymentAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    PaymentAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    PaymentAmountTotal = 15000.00m,
                    AllocatedAmountTotal = 15000.00m,
                    UnallocatedAmountTotal = 0.00m,
                    AdvanceAmountTotal = 0.00m,
                    PaymentStatus = PaymentStatuses.Posted,
                    PostedOn = DateTime.Today.AddDays(-15),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedBy = "Finance Controller",
                    ReceivableAccountIdSnapshot = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    CreatedAt = DateTime.Today.AddDays(-15),
                    CreatedBy = "Cashier",
                    Allocations = new List<CustomerPaymentAllocationViewModel>
                    {
                        new CustomerPaymentAllocationViewModel
                        {
                            Id = Guid.NewGuid(),
                            CustomerPaymentId = Payment3Id,
                            CustomerInvoiceId = Invoice3Id,
                            InvoiceNumber = "INV-2024-0003",
                            InvoiceDate = DateTime.Today.AddDays(-35),
                            DueDate = DateTime.Today.AddDays(-5),
                            InvoiceOutstanding = 15000.00m,
                            AllocatedAmount = 15000.00m,
                            CreatedAt = DateTime.Today.AddDays(-15),
                            CreatedBy = "Cashier"
                        }
                    }
                },

                // Payment 4: Posted - UPI payment with advance
                new CustomerPaymentViewModel
                {
                    Id = Payment4Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    CustomerId = Customer3Id,
                    CustomerCode = "CUST-003",
                    CustomerName = "XYZ Electronics Hub",
                    ReceiptNumber = "RCP-2024-0004",
                    ReceiptDate = DateTime.Today.AddDays(-10),
                    PostingDate = DateTime.Today.AddDays(-10),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    PaymentNarration = "UPI payment - includes advance for future orders",
                    PaymentMethod = PaymentMethods.UPI,
                    PaymentAccountId = BankAccountId,
                    PaymentAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    PaymentAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    InstrumentNumber = "UPI24050012345678",
                    PaymentAmountTotal = 100000.00m,
                    AllocatedAmountTotal = 75000.00m,
                    UnallocatedAmountTotal = 0.00m,
                    AdvanceAmountTotal = 25000.00m,
                    PaymentStatus = PaymentStatuses.Posted,
                    PostedOn = DateTime.Today.AddDays(-10),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedBy = "Finance Controller",
                    ReceivableAccountIdSnapshot = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    AdvanceFromCustomerAccountIdSnapshot = AdvanceFromCustomerAccountId,
                    AdvanceFromCustomerAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    AdvanceFromCustomerAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    CreatedAt = DateTime.Today.AddDays(-11),
                    CreatedBy = "AR Clerk",
                    Allocations = new List<CustomerPaymentAllocationViewModel>
                    {
                        new CustomerPaymentAllocationViewModel
                        {
                            Id = Guid.NewGuid(),
                            CustomerPaymentId = Payment4Id,
                            CustomerInvoiceId = Invoice4Id,
                            InvoiceNumber = "INV-2024-0004",
                            InvoiceDate = DateTime.Today.AddDays(-30),
                            DueDate = DateTime.Today,
                            InvoiceOutstanding = 75000.00m,
                            AllocatedAmount = 75000.00m,
                            CreatedAt = DateTime.Today.AddDays(-11),
                            CreatedBy = "AR Clerk"
                        }
                    }
                },

                // Payment 5: Draft - Pending bank transfer
                new CustomerPaymentViewModel
                {
                    Id = Payment5Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    CustomerId = Customer2Id,
                    CustomerCode = "CUST-002",
                    CustomerName = "State Government Education Dept",
                    ReceiptNumber = "RCP-2024-0005",
                    ReceiptDate = DateTime.Today.AddDays(-5),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    PaymentNarration = "Bank transfer received - pending verification",
                    PaymentMethod = PaymentMethods.BankTransfer,
                    PaymentAccountId = BankAccountId,
                    PaymentAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    PaymentAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    InstrumentDate = DateTime.Today.AddDays(-5),
                    InstrumentNumber = "SBI24050000098765",
                    BankName = "State Bank of India",
                    PaymentAmountTotal = 45000.00m,
                    AllocatedAmountTotal = 45000.00m,
                    UnallocatedAmountTotal = 0.00m,
                    AdvanceAmountTotal = 0.00m,
                    PaymentStatus = PaymentStatuses.Draft,
                    CreatedAt = DateTime.Today.AddDays(-5),
                    CreatedBy = "AR Clerk",
                    Allocations = new List<CustomerPaymentAllocationViewModel>
                    {
                        new CustomerPaymentAllocationViewModel
                        {
                            Id = Guid.NewGuid(),
                            CustomerPaymentId = Payment5Id,
                            CustomerInvoiceId = Invoice2Id,
                            InvoiceNumber = "INV-2024-0002",
                            InvoiceDate = DateTime.Today.AddDays(-40),
                            DueDate = DateTime.Today.AddDays(-10),
                            InvoiceOutstanding = 45000.00m,
                            AllocatedAmount = 45000.00m,
                            CreatedAt = DateTime.Today.AddDays(-5),
                            CreatedBy = "AR Clerk"
                        }
                    }
                },

                // Payment 6: Approved - Awaiting posting
                new CustomerPaymentViewModel
                {
                    Id = Payment6Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = BangaloreBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    CustomerId = Customer1Id,
                    CustomerCode = "CUST-001",
                    CustomerName = "ABC Traders Private Limited",
                    ReceiptNumber = "RCP-2024-0006",
                    ReceiptDate = DateTime.Today.AddDays(-3),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    PaymentNarration = "Card payment - approved, pending posting",
                    PaymentMethod = PaymentMethods.Card,
                    PaymentAccountId = BankAccountId,
                    PaymentAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    PaymentAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    InstrumentNumber = "TXN-20240501-7890",
                    PaymentAmountTotal = 25000.00m,
                    AllocatedAmountTotal = 0.00m,
                    UnallocatedAmountTotal = 0.00m,
                    AdvanceAmountTotal = 25000.00m,
                    PaymentStatus = PaymentStatuses.Approved,
                    CreatedAt = DateTime.Today.AddDays(-3),
                    CreatedBy = "AR Clerk",
                    Allocations = new List<CustomerPaymentAllocationViewModel>()
                },

                // Payment 7: Reversed - Cheque bounced
                new CustomerPaymentViewModel
                {
                    Id = Payment7Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    CustomerId = Customer3Id,
                    CustomerCode = "CUST-003",
                    CustomerName = "XYZ Electronics Hub",
                    ReceiptNumber = "RCP-2024-0007",
                    ReceiptDate = DateTime.Today.AddDays(-30),
                    PostingDate = DateTime.Today.AddDays(-30),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    PaymentNarration = "Cheque payment - REVERSED due to bounce",
                    PaymentMethod = PaymentMethods.Cheque,
                    PaymentAccountId = BankAccountId,
                    PaymentAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    PaymentAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    InstrumentDate = DateTime.Today.AddDays(-30),
                    InstrumentNumber = "CHQ-123456",
                    BankName = "Axis Bank",
                    BankAccountLast4 = "9876",
                    PaymentAmountTotal = 35000.00m,
                    AllocatedAmountTotal = 35000.00m,
                    UnallocatedAmountTotal = 0.00m,
                    AdvanceAmountTotal = 0.00m,
                    PaymentStatus = PaymentStatuses.Reversed,
                    PostedOn = DateTime.Today.AddDays(-30),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedBy = "Finance Controller",
                    ReversedOn = DateTime.Today.AddDays(-25),
                    ReversedByUserId = MasterDataIds.Tenants.Default,
                    ReversedBy = "Finance Controller",
                    ReversalReason = "Cheque bounced - Insufficient funds",
                    ReversalReference = "BNC-2024-001",
                    ReceivableAccountIdSnapshot = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    CreatedAt = DateTime.Today.AddDays(-31),
                    CreatedBy = "AR Clerk",
                    Allocations = new List<CustomerPaymentAllocationViewModel>
                    {
                        new CustomerPaymentAllocationViewModel
                        {
                            Id = Guid.NewGuid(),
                            CustomerPaymentId = Payment7Id,
                            CustomerInvoiceId = Invoice5Id,
                            InvoiceNumber = "INV-2024-0005",
                            InvoiceDate = DateTime.Today.AddDays(-50),
                            DueDate = DateTime.Today.AddDays(-20),
                            InvoiceOutstanding = 35000.00m,
                            AllocatedAmount = 35000.00m,
                            CreatedAt = DateTime.Today.AddDays(-31),
                            CreatedBy = "AR Clerk"
                        }
                    }
                }
,

                // Payment 8: Posted - Full payment
                new CustomerPaymentViewModel
                {
                    Id = Guid.Parse("c3c3c3c3-0001-0001-0001-000000000008"),
                    CompanyId = MasterDataIds.Companies.SofaCraft,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = MasterDataIds.Branches.SofaCraftHQ,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftHQ), BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftHQ),
                    CustomerId = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000004"),
                    CustomerCode = "CUST-004", CustomerName = "Rajesh Kumar",
                    ReceiptNumber = "ARP-2024-0008",
                    ReceiptDate = DateTime.Today.AddDays(-12),
                    PostingDate = DateTime.Today.AddDays(-12),
                    CurrencyId = MasterDataIds.Currencies.INR,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR), CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR), ExchangeRate = 1,
                    PaymentMethod = PaymentMethods.BankTransfer,
                    InstrumentNumber = "NEFT-DI-202502001",
                    PaymentNarration = "Invoice payment via NEFT",
                    PaymentAmountTotal = 150000.00m, AllocatedAmountTotal = 150000.00m, UnallocatedAmountTotal = 0,
                    PaymentStatus = PaymentStatuses.Posted,
                    PostedOn = DateTime.Today.AddDays(-12), PostedBy = "Finance Controller",
                    CreatedAt = DateTime.Today.AddDays(-12), CreatedBy = "AR Clerk"
                },

                // Payment 9: Submitted
                new CustomerPaymentViewModel
                {
                    Id = Guid.Parse("c3c3c3c3-0001-0001-0001-000000000009"),
                    CompanyId = MasterDataIds.Companies.SofaCraft,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = MasterDataIds.Branches.SofaCraftBengaluru,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru), BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    CustomerId = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000005"),
                    CustomerCode = "CUST-005", CustomerName = "TechPark SEZ Solutions Pvt Ltd",
                    ReceiptNumber = "ARP-2024-0009",
                    ReceiptDate = DateTime.Today.AddDays(-5),
                    CurrencyId = MasterDataIds.Currencies.INR,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR), CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR), ExchangeRate = 1,
                    PaymentMethod = PaymentMethods.Cheque,
                    InstrumentNumber = "CHQ-456789",
                    PaymentNarration = "Cheque payment for services",
                    PaymentAmountTotal = 75000.00m, AllocatedAmountTotal = 75000.00m, UnallocatedAmountTotal = 0,
                    PaymentStatus = PaymentStatuses.Submitted,
                    CreatedAt = DateTime.Today.AddDays(-5), CreatedBy = "AR Clerk"
                },

                // Payment 10: Draft
                new CustomerPaymentViewModel
                {
                    Id = Guid.Parse("c3c3c3c3-0001-0001-0001-000000000010"),
                    CompanyId = MasterDataIds.Companies.SofaCraft,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = MasterDataIds.Branches.SofaCraftHQ,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftHQ), BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftHQ),
                    CustomerId = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000006"),
                    CustomerCode = "CUST-006", CustomerName = "Discontinued Enterprises",
                    ReceiptNumber = "ARP-2024-0010",
                    ReceiptDate = DateTime.Today.AddDays(-2),
                    CurrencyId = MasterDataIds.Currencies.INR,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR), CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR), ExchangeRate = 1,
                    PaymentMethod = PaymentMethods.BankTransfer,
                    PaymentNarration = "Advance payment for upcoming project",
                    PaymentAmountTotal = 100000.00m, AllocatedAmountTotal = 0, UnallocatedAmountTotal = 100000.00m,
                    PaymentStatus = PaymentStatuses.Draft,
                    CreatedAt = DateTime.Today.AddDays(-2), CreatedBy = "AR Clerk"
                },

                // Payment 11: Posted - Advance payment
                new CustomerPaymentViewModel
                {
                    Id = Guid.Parse("c3c3c3c3-0001-0001-0001-000000000011"),
                    CompanyId = MasterDataIds.Companies.SofaCraft,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = MasterDataIds.Branches.SofaCraftHQ,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftHQ), BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftHQ),
                    CustomerId = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000001"),
                    CustomerCode = "CUST-001", CustomerName = "ABC Traders Private Limited",
                    ReceiptNumber = "ARP-2024-0011",
                    ReceiptDate = DateTime.Today.AddDays(-25),
                    PostingDate = DateTime.Today.AddDays(-25),
                    CurrencyId = MasterDataIds.Currencies.INR,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR), CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR), ExchangeRate = 1,
                    PaymentMethod = PaymentMethods.BankTransfer,
                    InstrumentNumber = "RTGS-ABC-202501",
                    PaymentNarration = "RTGS payment for quarterly billing",
                    PaymentAmountTotal = 200000.00m, AllocatedAmountTotal = 200000.00m, UnallocatedAmountTotal = 0,
                    PaymentStatus = PaymentStatuses.Posted,
                    PostedOn = DateTime.Today.AddDays(-25), PostedBy = "Finance Controller",
                    CreatedAt = DateTime.Today.AddDays(-25), CreatedBy = "Finance Manager"
                },

                // Payment 12: Cancelled
                new CustomerPaymentViewModel
                {
                    Id = Guid.Parse("c3c3c3c3-0001-0001-0001-000000000012"),
                    CompanyId = MasterDataIds.Companies.SofaCraft,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = MasterDataIds.Branches.SofaCraftHQ,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftHQ), BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftHQ),
                    CustomerId = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000002"),
                    CustomerCode = "CUST-002", CustomerName = "State Government Education Dept",
                    ReceiptNumber = "ARP-2024-0012",
                    ReceiptDate = DateTime.Today.AddDays(-18),
                    CurrencyId = MasterDataIds.Currencies.INR,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR), CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR), ExchangeRate = 1,
                    PaymentMethod = PaymentMethods.Cheque,
                    InstrumentNumber = "CHQ-BOUNCE-001",
                    PaymentNarration = "Cancelled - Cheque bounced",
                    PaymentAmountTotal = 50000.00m, AllocatedAmountTotal = 0, UnallocatedAmountTotal = 0,
                    PaymentStatus = PaymentStatuses.Cancelled,
                    CancellationReason = "Cheque returned - insufficient funds",
                    CreatedAt = DateTime.Today.AddDays(-18), CreatedBy = "AR Clerk"
                }
};

            return payments;
        }
    }
}
