using FinanceConnect.Client.ViewModels;
using System.Security.Cryptography;
using System.Text;

namespace FinanceConnect.Client.Data
{
    public class BankStatementLineSeedData
    {
        public static List<BankStatementLineModel> GetSeedData(
            Guid tenantId,
            List<BankStatementModel> bankStatements // 👈 pass real statements
        )
        {
            var list = new List<BankStatementLineModel>();
            var random = new Random();

            foreach (var statement in bankStatements)
            {
                int lineNo = 1;
                int lineCount = random.Next(15, 40);

                for (int i = 0; i < lineCount; i++)
                {
                    bool isCredit = random.Next(0, 2) == 1;
                    decimal amount = Math.Round((decimal)(random.NextDouble() * 5000 + 100), 2);

                    decimal debit = isCredit ? 0 : amount;
                    decimal credit = isCredit ? amount : 0;

                    var txnDate = DateTime.UtcNow.Date.AddDays(-random.Next(1, 28));

                    string narration = isCredit
                        ? $"UPI CREDIT FROM CUSTOMER {random.Next(1000, 9999)}"
                        : $"NEFT PAYMENT TO VENDOR {random.Next(1000, 9999)}";

                    var reference = $"TXN{random.Next(100000, 999999)}";
                    var normalizedNarration = Normalize(narration);

                    var direction = isCredit
                        ? StatementLineDirectionType.Credit
                        : StatementLineDirectionType.Debit;

                    var line = new BankStatementLineModel
                    {
                        BankStatementLineId = Guid.NewGuid(),

                        TenantId = tenantId,
                        CompanyId = statement.CompanyId,

                        BankStatementId = statement.BankStatementId,
                        BankAccountId = statement.BankAccountId,

                        LineNumber = lineNo++,

                        TransactionDate = txnDate,
                        ValueDate = txnDate,
                        ImportBatchDate = DateTime.UtcNow,

                        DebitAmount = debit,
                        CreditAmount = credit,
                        Direction = direction,
                        Amount = amount,

                        CurrencyId = statement.CurrencyId,
                        RunningBalance = Math.Round((decimal)(random.NextDouble() * 100000), 2),

                        BankProvidedTransactionId = reference,
                        ReferenceText = reference,
                        NarrationRaw = narration,
                        NarrationNormalized = normalizedNarration,

                        UTRNumberExtracted = isCredit
                            ? $"UTR{random.Next(100000000, 999999999)}"
                            : null,

                        ChequeNumberExtracted = !isCredit
                            ? random.Next(100000, 999999).ToString()
                            : null,

                        TransactionCode = isCredit ? "UPI" : "NEFT",

                        ParseStatus = ParseStatusType.Parsed,
                        ParseWarningMessage = null,

                        LineHashSHA256 = ComputeHash(
                            statement.BankAccountId,
                            txnDate,
                            direction,
                            amount,
                            reference,
                            normalizedNarration
                        ),

                        IsDuplicateInFile = false,
                        IsDuplicateAcrossStatements = false,

                        ReconciliationStatus = ReconciliationStatusType.Unmatched,

                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "seed",
                        UpdatedAt = null,
                        UpdatedBy = null,
                        IsDeleted = false
                    };

                    list.Add(line);
                }
            }

            return list;
        }

        // ================= HELPER METHODS =================

        private static string Normalize(string input)
        {
            return string.IsNullOrWhiteSpace(input)
                ? ""
                : string.Join(" ",
                    input
                        .Trim()
                        .ToUpperInvariant()
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }

        private static string ComputeHash(
            Guid bankAccountId,
            DateTime date,
            StatementLineDirectionType direction,
            decimal amount,
            string reference,
            string narrationNormalized)
        {
            var raw =
                $"{bankAccountId}|{date:yyyyMMdd}|{direction}|{amount:N2}|{reference}|{narrationNormalized}";

            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(raw);
            var hash = sha.ComputeHash(bytes);

            return Convert.ToHexString(hash);
        }
    }
}
