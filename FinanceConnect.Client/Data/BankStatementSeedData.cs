using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    public class BankStatementSeedData
    {
        public static List<BankStatementModel> GetSeedData(
            Guid tenantId,
            List<CompanyModel> companies,
            List<BranchModel> branches,
            List<BankAccountModel> bankAccounts)
        {
            var result = new List<BankStatementModel>();

            var defaultCurrencyId = MasterDataIds.Currencies.INR;

            int sequence = 1;
            var random = new Random();
            var AccountId = bankAccounts.First();

            foreach (var company in companies)
            {
                // Pick a branch for this company (if available)
                var branch = branches.FirstOrDefault(b => b.CompanyId == company.Id);

                // Fake bank account per company (in real system, fetch from BankAccountSeedData)
                var bankAccountId = AccountId.Id;

                var fromDate = new DateTime(2026, 1, 1);
                var toDate = new DateTime(2026, 1, 31);

                var openingBalance = random.Next(100_000, 500_000);
                var totalCredits = random.Next(10_000, 80_000);
                var totalDebits = random.Next(5_000, 60_000);
                var netMovement = totalCredits - totalDebits;
                var closingBalance = openingBalance + netMovement;

                result.Add(new BankStatementModel
                {
                    // ================= CORE =================
                    BankStatementId = Guid.NewGuid(),
                    TenantId = tenantId,
                    CompanyId = company.Id,
                    BranchId = branch?.Id,
                    BankAccountId = bankAccountId,
                    BankAccountName = "Primary Bank Account",

                    StatementNumber = $"BNKSTM-{sequence.ToString("D6")}",
                    StatementStatus = StatementStatusType.ReadyForReconciliation,

                    // ================= FILE =================
                    ImportSource = ImportSourceType.ManualUpload,
                    FileNameOriginal = $"BANK_{company.CompanyCode}_JAN_2026.csv",
                    FileType = "CSV",
                    FileSizeBytes = random.Next(100_000, 900_000),
                    FileStoragePath = $"/secure/bankstatements/{company.CompanyCode}/JAN_2026.csv",
                    FileHashSHA256 = Guid.NewGuid().ToString("N").ToUpper(),
                    FileUploadedAt = DateTime.UtcNow.AddDays(-random.Next(1, 20)),
                    FileUploadedBy = "seed.system",

                    // ================= PERIOD =================
                    StatementFromDate = fromDate,
                    StatementToDate = toDate,
                    CurrencyId = defaultCurrencyId,

                    OpeningBalance = openingBalance,
                    ClosingBalance = closingBalance,

                    TotalCreditsAmount = totalCredits,
                    TotalDebitsAmount = totalDebits,
                    NetMovementAmount = netMovement,

                    BalanceCheckStatus = BalanceCheckStatusType.Matched,
                    BalanceDifferenceAmount = 0,

                    // ================= PROFILE =================
                    StatementProfile = StatementProfileType.Generic_CSV,
                    ProfileVersion = "v1.0",
                    ParsingSettingsSnapshotJson = "{ \"dateFormat\": \"dd-MM-yyyy\", \"delimiter\": \",\" }",

                    // ================= METRICS =================
                    TotalLineCount = random.Next(500, 5000),
                    ParsedSuccessLineCount = 0, // will be set after line seed
                    ParsedFailedLineCount = 0,
                    DuplicateLineCountInFile = 0,
                    FirstTransactionDateInFile = fromDate,
                    LastTransactionDateInFile = toDate,

                    // ================= LOGS =================
                    ProcessingStatusMessage = "Seeded and ready for reconciliation",
                    ErrorSummary = "",

                    ParseStartedAt = DateTime.UtcNow.AddMinutes(-5),
                    ParseCompletedAt = DateTime.UtcNow.AddMinutes(-4),
                    ValidationStartedAt = DateTime.UtcNow.AddMinutes(-3),
                    ValidationCompletedAt = DateTime.UtcNow.AddMinutes(-2),

                    ProcessedByJobId = "SEED-JOB",

                    // ================= GOVERNANCE =================
                    IsUsedInReconciliation = false,

                    // ================= SYSTEM =================
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    CreatedBy = "seed.system",
                    IsDeleted = false
                });

                sequence++;
            }

            return result;
        }
    }
}
