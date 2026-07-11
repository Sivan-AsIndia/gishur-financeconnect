using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    public class BankTransactionSeedData
    {
        public static List<BankTransactionModel> GetAllTransactions()
        {
            return new List<BankTransactionModel>
            {
                new BankTransactionModel
                {
                    TransactionNumber = "BTX-0001",
                    TransactionStatus = "Submitted",
                    TransactionType = "Vendor Payment",
                    Direction = "Outflow",
                    Amount = 25000,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    TransactionDate = DateTime.Today.AddDays(-5),
                    SourceModule = "CashBank",
                    PaymentMethod = "NEFT",
                    CounterpartyNameSnapshot = "ABC Suppliers",
                    ReferenceNumber = "REF001"
                },

                new BankTransactionModel
                {
                    TransactionNumber = "BTX-0002",
                    TransactionStatus = "Approved",
                    TransactionType = "Customer Receipt",
                    Direction = "Inflow",
                    Amount = 18000,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    TransactionDate = DateTime.Today.AddDays(-4),
                    SourceModule = "CashBank",
                    PaymentMethod = "UPI",
                    CounterpartyNameSnapshot = "Ravi Traders",
                    ReferenceNumber = "REF002"
                },

                new BankTransactionModel
                {
                    TransactionNumber = "BTX-0003",
                    TransactionStatus = "Posted",
                    TransactionType = "Salary Payment",
                    Direction = "Outflow",
                    Amount = 55000,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    TransactionDate = DateTime.Today.AddDays(-3),
                    SourceModule = "CashBank",
                    PaymentMethod = "IMPS",
                    CounterpartyNameSnapshot = "Staff Payroll",
                    ReferenceNumber = "REF003"
                },

                new BankTransactionModel
                {
                    TransactionNumber = "BTX-0004",
                    TransactionStatus = "Draft",
                    TransactionType = "Petty Cash Transfer",
                    Direction = "Outflow",
                    Amount = 3200,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    TransactionDate = DateTime.Today.AddDays(-2),
                    SourceModule = "CashBank",
                    PaymentMethod = "Cash",
                    CounterpartyNameSnapshot = "Office Expense",
                    ReferenceNumber = "REF004"
                },

                new BankTransactionModel
                {
                    TransactionNumber = "BTX-0005",
                    TransactionStatus = "Reversed",
                    TransactionType = "Bank Charges",
                    Direction = "Outflow",
                    Amount = 450,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    TransactionDate = DateTime.Today.AddDays(-1),
                    SourceModule = "CashBank",
                    PaymentMethod = "Auto Debit",
                    CounterpartyNameSnapshot = "HDFC Bank",
                    ReferenceNumber = "REF005"
                },

                new BankTransactionModel
                {
                    TransactionNumber = "BTX-0006",
                    TransactionStatus = "Posted",
                    TransactionType = "Loan Credit",
                    Direction = "Inflow",
                    Amount = 200000,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    TransactionDate = DateTime.Today,
                    SourceModule = "CashBank",
                    PaymentMethod = "RTGS",
                    CounterpartyNameSnapshot = "Axis Bank",
                    ReferenceNumber = "REF006"
                },

                new BankTransactionModel
                {
                    TransactionNumber = "BTX-0007",
                    TransactionStatus = "Approved",
                    TransactionType = "Refund Received",
                    Direction = "Inflow",
                    Amount = 5600,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    TransactionDate = DateTime.Today,
                    SourceModule = "CashBank",
                    PaymentMethod = "UPI",
                    CounterpartyNameSnapshot = "Amazon",
                    ReferenceNumber = "REF007"
                },

                new BankTransactionModel
                {
                    TransactionNumber = "BTX-0008",
                    TransactionStatus = "Submitted",
                    TransactionType = "Utility Payment",
                    Direction = "Outflow",
                    Amount = 7800,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    TransactionDate = DateTime.Today.AddDays(-6),
                    SourceModule = "CashBank",
                    PaymentMethod = "NEFT",
                    CounterpartyNameSnapshot = "TNEB",
                    ReferenceNumber = "REF008"
                },

                new BankTransactionModel
                {
                    TransactionNumber = "BTX-0009",
                    TransactionStatus = "Draft",
                    TransactionType = "Cash Deposit",
                    Direction = "Inflow",
                    Amount = 15000,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    TransactionDate = DateTime.Today.AddDays(-7),
                    SourceModule = "CashBank",
                    PaymentMethod = "Cash",
                    CounterpartyNameSnapshot = "Branch Counter",
                    ReferenceNumber = "REF009"
                },

                new BankTransactionModel
                {
                    TransactionNumber = "BTX-0010",
                    TransactionStatus = "Posted",
                    TransactionType = "Inter Bank Transfer",
                    Direction = "Outflow",
                    Amount = 92000,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    TransactionDate = DateTime.Today.AddDays(-10),
                    SourceModule = "CashBank",
                    PaymentMethod = "RTGS",
                    CounterpartyNameSnapshot = "ICICI Bank",
                    ReferenceNumber = "REF010"
                },

                new BankTransactionModel
                {
                    TransactionNumber = "BTX-0011",
                    TransactionStatus = "Approved",
                    TransactionType = "Insurance Premium",
                    Direction = "Outflow",
                    Amount = 34500,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    TransactionDate = DateTime.Today.AddDays(-8),
                    SourceModule = "CashBank",
                    PaymentMethod = "NEFT",
                    CounterpartyNameSnapshot = "LIC of India",
                    ReferenceNumber = "REF011"
                },

                new BankTransactionModel
                {
                    TransactionNumber = "BTX-0012",
                    TransactionStatus = "Submitted",
                    TransactionType = "Customer Advance",
                    Direction = "Inflow",
                    Amount = 75000,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    TransactionDate = DateTime.Today.AddDays(-3),
                    SourceModule = "CashBank",
                    PaymentMethod = "RTGS",
                    CounterpartyNameSnapshot = "Mahindra Interiors",
                    ReferenceNumber = "REF012"
                }
            };
        }



        public static List<BankTransactionModel> GetAllTransactionsMatch(Guid bankAccountId)
        {
            var list = new List<BankTransactionModel>();
            var rand = new Random();


            for (int i = 1; i <= 5; i++)
            {
                list.Add(new BankTransactionModel
                {
                    Id = Guid.NewGuid(),
                    BankAccountId = bankAccountId,

                    TransactionNumber = $"BTX-{bankAccountId.ToString()[..4]}-{i:D3}",
                    TransactionStatus = i % 2 == 0 ? "Posted" : "Approved",

                    TransactionType = i % 2 == 0
                        ? "Customer Receipt"
                        : "Vendor Payment",

                    Direction = i % 2 == 0 ? "Inflow" : "Outflow",

                    Amount = rand.Next(1_000, 50_000),
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),

                    TransactionDate = DateTime.Today.AddDays(-rand.Next(1, 10)),

                    SourceModule = "CashBank",

                    PaymentMethod = i % 3 == 0
                        ? "NEFT"
                        : i % 3 == 1
                            ? "UPI"
                            : "RTGS",

                    CounterpartyNameSnapshot = i % 2 == 0
                        ? "Customer Co"
                        : "Vendor Pvt Ltd",

                    ReferenceNumber = $"REF-{Guid.NewGuid().ToString()[..6]}",
                    Narration = i % 2 == 0
                        ? "UPI CREDIT"
                        : "NEFT DEBIT"
                });
            }


            return list;
        }
    }
}

