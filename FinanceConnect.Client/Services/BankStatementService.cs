using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components.Forms;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;

namespace FinanceConnect.Client.Services
{
    public class BankStatementService
    {
        private static List<BankStatementModel> _statements = new();
        private readonly List<BankStatementModel> _seedStatements = new();
        private readonly List<BankStatementLineModel> _lines = new();
        private bool _isSeeded = false;
        private readonly List<CompanyModel> _companies = new();
        private readonly MasterDataService _masterDataService;
        private readonly BranchService _branchService;
        private readonly BankAccountService _accountService;
        private readonly List<BranchModel> _branches = new();
        private readonly List<BankAccountModel> _accounts = new();

        public BankStatementService(MasterDataService masterDataService, BranchService branchService , BankAccountService accountService)
        {
            _masterDataService = masterDataService;
            _branchService = branchService;
            _accountService = accountService;
            _companies = _masterDataService
                .GetAllCompanies()
                .Where(c => c.Status == "Active")
                .ToList();
            _branches = _branchService.GetAll().Where(t => t.Status == "Active").ToList();
            _accounts = _accountService.GetAll();
        }

        public List<BankStatementModel> GetAll(Guid tenantId)
        {
            EnsureSeeded(tenantId);

            return _statements
                .Where(s => s.TenantId == tenantId && !s.IsDeleted)
                .OrderByDescending(s => s.FileUploadedAt)
                .ToList();
        }

        public BankStatementModel? GetById(Guid statementId)
        {
            return _statements
                .FirstOrDefault(s =>
                    s.BankStatementId == statementId &&
                    !s.IsDeleted);
        }

        public List<BankStatementLineModel> GetLinesByBankAccount(Guid? bankAccountId)
        {
            if (bankAccountId == null)
                return new List<BankStatementLineModel>();

            // 🔥 THIS WAS MISSING
            EnsureSeeded(
                _statements.FirstOrDefault()?.TenantId
                ?? Guid.Parse("11111111-1111-1111-1111-111111111111")
            );

            return _lines
                .Where(l =>
                    l.BankAccountId == bankAccountId &&
                    !l.IsDeleted)
                .OrderBy(l => l.TransactionDate)
                .ThenBy(l => l.LineNumber)
                .ToList();
        }



        public IEnumerable<BankStatementLineModel> GetLines(Guid bankStatementId)
        {
            EnsureSeeded(
                _statements.FirstOrDefault()?.TenantId
                ?? Guid.Parse("11111111-1111-1111-1111-111111111111")
            );

            return _lines
                .Where(l =>
                    l.BankStatementId == bankStatementId &&
                    !l.IsDeleted)
                .OrderBy(l => l.LineNumber);
        }
        private void EnsureSeeded(Guid tenantId)
        {
            if (_isSeeded)
                return;


            var seedData = BankStatementSeedData.GetSeedData(tenantId, _companies, _branches, _accounts);
            _seedStatements.AddRange(seedData);
            if (!_lines.Any())
            {

                _lines.AddRange(
                    BankStatementLineSeedData.GetSeedData(tenantId, _seedStatements)
                );
            }
            ResetToSeed();
            _isSeeded = true;
        }

        private static List<T> CloneList<T>(IEnumerable<T> source) where T : class
        {
            var json = System.Text.Json.JsonSerializer.Serialize(source);
            return System.Text.Json.JsonSerializer.Deserialize<List<T>>(json) ?? new();
        }

        public void ResetToSeed()
        {
            _statements = CloneList(_seedStatements);
        }

        // ================= GOVERNANCE ACTIONS =================

        public void Lock(Guid id, string user = "system")
        {
            var stmt = _statements.FirstOrDefault(x => x.BankStatementId == id);
            if (stmt == null)
                throw new Exception("Statement not found");

            if (stmt.StatementStatus == StatementStatusType.Locked)
                throw new Exception("Statement already locked");

            stmt.StatementStatus = StatementStatusType.Locked;
            stmt.LockedBy = user;
            stmt.LockedOn = DateTime.UtcNow;
            stmt.IsUsedInReconciliation = true;
        }

        public void Supersede(Guid id, string reason, string user = "system")
        {
            var stmt = _statements.FirstOrDefault(x => x.BankStatementId == id);
            if (stmt == null)
                throw new Exception("Statement not found");

            stmt.StatementStatus = StatementStatusType.Superseded;
            stmt.SupersedeReason = reason;
            stmt.UpdatedAt = DateTime.UtcNow;
            stmt.UpdatedBy = user;
        }

        public void Archive(Guid id, string user = "system")
        {
            var stmt = _statements.FirstOrDefault(x => x.BankStatementId == id);
            if (stmt == null)
                throw new Exception("Statement not found");

            stmt.StatementStatus = StatementStatusType.Archived;
            stmt.ArchivedBy = user;
            stmt.ArchivedOn = DateTime.UtcNow;
        }

        public void Delete(Guid statementId)
        {
            var statement = _statements
                .FirstOrDefault(s => s.BankStatementId == statementId && !s.IsDeleted);

            if (statement == null)
                throw new InvalidOperationException("Bank statement not found.");

            // ================= GOVERNANCE RULES =================
            if (statement.StatementStatus == StatementStatusType.Locked)
                throw new InvalidOperationException("Locked statements cannot be deleted.");

            if (statement.IsUsedInReconciliation)
                throw new InvalidOperationException("Statement used in reconciliation cannot be deleted.");

            // ================= SOFT DELETE HEADER =================
            statement.IsDeleted = true;
            statement.UpdatedAt = DateTime.UtcNow;
            statement.UpdatedBy = "ui.user"; // replace with logged-in user

            // ================= SOFT DELETE LINES =================
            var lines = _lines
                .Where(l => l.BankStatementId == statementId && !l.IsDeleted)
                .ToList();

            foreach (var line in lines)
            {
                line.IsDeleted = true;
                line.UpdatedAt = DateTime.UtcNow;
                line.UpdatedBy = "ui.user"; // replace with logged-in user
            }
        }


        // ================= IMPORT BANK STATEMENT =================
        public async Task Import(
            BankStatementModel model,
            byte[] fileBytes,
            string fileName,
            string user = "system")
        {
            if (fileBytes == null || fileBytes.Length == 0)
                throw new Exception("Statement file is required");

            EnsureSeeded(model.TenantId);

            // ================= HASH =================
            string hash;
            using (var sha = SHA256.Create())
            {
                var hashBytes = sha.ComputeHash(fileBytes);
                hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }

            bool duplicate = _statements.Any(s =>
                s.CompanyId == model.CompanyId &&
                s.BankAccountId == model.BankAccountId &&
                s.FileHashSHA256 == hash &&
                !s.IsDeleted);

            if (duplicate)
                throw new Exception("Statement already imported (duplicate file detected)");

            var statement = new BankStatementModel
            {
                BankStatementId = Guid.NewGuid(),
                TenantId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                CompanyId = model.CompanyId,
                BranchId = model.BranchId,
                BranchName = model.BranchName,
                BankAccountId = model.BankAccountId,
                BankAccountName =model.BankAccountName,

                StatementNumber = $"BNKSTM-{(_statements.Count + 1):D6}",
                StatementStatus = StatementStatusType.Uploaded,

                ImportSource = model.ImportSource,
                FileNameOriginal = fileName,
                FileType = Path.GetExtension(fileName).Replace(".", "").ToUpperInvariant(),
                FileSizeBytes = fileBytes.Length,
                FileStoragePath = $"in-memory://statements/{Guid.NewGuid()}",
                FileHashSHA256 = hash,
                FileUploadedAt = DateTime.UtcNow,
                FileUploadedBy = user,

                StatementProfile = model.StatementProfile,
                ProfileVersion = "v1.0",

                StatementFromDate = DateTime.Today.AddDays(-30),
                StatementToDate = DateTime.Today,

                CurrencyId = model.CurrencyId,

                TotalLineCount = 0,
                TotalCreditsAmount = 0,
                TotalDebitsAmount = 0,
                NetMovementAmount = 0,

                BalanceCheckStatus = BalanceCheckStatusType.NotAvailable,

                IsUsedInReconciliation = false,

                CreatedAt = DateTime.UtcNow,
                CreatedBy = user,
                IsDeleted = false
            };

            _statements.Add(statement);
        }


        // ================= HASH GENERATOR =================
        private async Task<string> ComputeHash(IBrowserFile file)
        {
            using var sha = SHA256.Create();
            await using var stream = file.OpenReadStream(100 * 1024 * 1024);

            var hashBytes = await sha.ComputeHashAsync(stream);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }


        public BankStatementStatistics GetStatistics(Guid tenantId)
        {
            var statements = GetAll(tenantId);

            return new BankStatementStatistics
            {
                TotalStatements = statements.Count(),
                UploadedStatements = statements.Count(st => st.StatementStatus == StatementStatusType.Uploaded),
                ReadyForReconciliationStatements = statements.Count(st => st.StatementStatus == StatementStatusType.ReadyForReconciliation),
                LockedStatements = statements.Count(st => st.StatementStatus == StatementStatusType.Locked),
                ArchivedStatements = statements.Count(st => st.StatementStatus == StatementStatusType.Archived),
                SupersededStatements = statements.Count(st => st.StatementStatus == StatementStatusType.Superseded),

            };
        }

    }
}
