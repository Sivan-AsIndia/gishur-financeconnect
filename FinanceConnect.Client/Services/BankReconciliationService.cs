using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    public class BankReconciliationService
    {
        private static List<BankReconciliationModel> _runs = new();
        private readonly List<BankReconciliationModel> _seedRuns = new();
        private readonly List<BankReconciliationMatchModel> _matches = new();
        private readonly CompanyModel? _company;
        private readonly MasterDataService _masterDataService;
        private readonly BranchService _branchService;
        private readonly BranchModel? _branch;
        private readonly BankAccountModel? _bankAccount;
        private readonly BankStatementService _statementService;
        private readonly BankTransactionService _transactionService;
        private readonly BankAccountService _accountService;

        private bool _isSeeded = false;
        private readonly Guid TenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");


        public BankReconciliationService(
            BankStatementService statementService,
            BankTransactionService transactionService, MasterDataService masterDataService, BranchService branchService, BankAccountService accountService)
        {
            _statementService = statementService;
            _transactionService = transactionService;
            _masterDataService = masterDataService;
            _branchService = branchService;
            _accountService = accountService;
            _company = _masterDataService
                .GetAllCompanies()
                .FirstOrDefault(c => c.Status == "Active")
                ?? throw new Exception("No active company found");

            _branch = _branchService.GetByCompanyId(_company.Id).FirstOrDefault() 
                ?? throw new Exception("No Branch found");

            _bankAccount = _accountService.GetAll().FirstOrDefault()
                ?? throw new Exception("No Account found");

        }

        // ================= SEEDING =================

        private void EnsureSeeded(
            Guid tenantId,
            Guid companyId,
            Guid bankAccountId,
            Guid? branchId)
        {
            if (_isSeeded)
                return;

            var seeds = BankReconciliationSeedData
                .GetSeedData(tenantId, companyId, bankAccountId, branchId);

            _runs.AddRange(seeds);
            _seedRuns.AddRange(seeds);
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
            _runs = CloneList(_seedRuns);
        }

        // ================= CORE =================

        public BankReconciliationModel GetById(Guid reconId)
        {
            return _runs.FirstOrDefault(x =>
                x.BankReconciliationId == reconId &&
                !x.IsDeleted)
                ?? throw new Exception("Reconciliation not found");
        }

        public List<BankReconciliationModel> GetAll()
        {
            EnsureSeeded(TenantId,_company.Id, _bankAccount.Id, _branch.Id);
            return _runs.Where(r => !r.IsDeleted).OrderByDescending(r => r.CreatedAt).ToList();
        }

        public BankReconciliationModel Create(BankReconciliationModel model)
        {
            model.BankReconciliationId = Guid.NewGuid();
            model.ReconciliationStatus = ReconciliationStatus.Draft;
            model.CreatedAt = DateTime.UtcNow;
            model.PreparedOn = DateTime.UtcNow;
            _runs.Add(model);
            return model;
        }
        // ================= CREATE =================

        public BankReconciliationModel CreateDraft(string user = "system") 
        { 
            var model = new BankReconciliationModel 
            { 
                BankReconciliationId = Guid.NewGuid(),
                ReconciliationNumber = $"BNKREC-{_runs.Count + 1:000000}",
                RunType = RunType.OnDemand,
                ReconciliationStatus = ReconciliationStatus.Draft,
                ScopeType = ScopeType.PeriodRange,
                StatementSelectionMode = StatementSelectionModeType.ByStatementFile,
                AutoMatchEnabled = true, DateWindowDays = 2,
                AmountTolerance = 0,
                PreparedBy = user,
                PreparedOn = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = user 
            }; 
            _runs.Add(model); 
            return model; 
        }
        public BankReconciliationModel GenerateReconNumber(string user = "system") 
        { 
            var model = new BankReconciliationModel 
            {
                BankReconciliationId = Guid.NewGuid(),
                ReconciliationNumber = $"BNKREC-{_runs.Count + 1:000000}",
                ReconciliationStatus = ReconciliationStatus.Draft,
                AutoMatchEnabled = true,
                FromDate = DateTime.Today,
                DateWindowDays = 0,
                AmountTolerance = 0,
                PreparedBy = user,
                PreparedOn = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = user 
            }; 
            _runs.Add(model); 
            return model; 
        }

        public void Update(BankReconciliationModel model, string user = "system")
        {
            var existing = _runs.FirstOrDefault(r =>
                r.BankReconciliationId == model.BankReconciliationId &&
                !r.IsDeleted);

            if (existing == null)
                throw new Exception("Reconciliation run not found");

            // Governance: finalized runs are immutable
            if (existing.ReconciliationStatus == ReconciliationStatus.Finalized)
                throw new Exception("Finalized reconciliation cannot be modified");

            // Preserve system/audit fields
            model.CreatedAt = existing.CreatedAt;
            model.CreatedBy = existing.CreatedBy;
            model.PreparedBy = existing.PreparedBy;
            model.PreparedOn = existing.PreparedOn;

            // Set update audit
            model.UpdatedAt = DateTime.UtcNow;
            model.UpdatedBy = user;

            // Replace record
            _runs.Remove(existing);
            _runs.Add(model);
        }

        public void RemoveData(BankReconciliationModel model)
        {
            var existing = _runs.FirstOrDefault(r =>
                r.BankReconciliationId == model.BankReconciliationId &&
                !r.IsDeleted);

            if (existing == null)
                throw new Exception("Reconciliation run not found");
            _runs.Remove(existing);
        }

        public BankReconciliationModel CreateNew(
            Guid tenantId,
            Guid companyId,
            Guid bankAccountId,
            Guid? branchId,
            RunType runType,
            ScopeType scopeType,
            DateTime? fromDate,
            DateTime? toDate,
            DateTime? asOfDate,
            string user = "system")
        {
            // ================= VALIDATION =================
            if (bankAccountId == Guid.Empty)
                throw new Exception("Bank Account is required");

            if (scopeType == ScopeType.PeriodRange)
            {
                if (!fromDate.HasValue || !toDate.HasValue)
                    throw new Exception("FromDate and ToDate required");

                if (fromDate > toDate)
                    throw new Exception("FromDate cannot be after ToDate");
            }

            if (scopeType == ScopeType.AsOfDate && !asOfDate.HasValue)
                throw new Exception("AsOfDate is required");

            // ================= CREATE =================
            var recon = new BankReconciliationModel
            {
                BankReconciliationId = Guid.NewGuid(),

                TenantId = tenantId,
                CompanyId = companyId,
                BranchId = branchId,
                BankAccountId = bankAccountId,

                ReconciliationNumber = $"BNKREC-{_runs.Count + 1:D6}",

                RunType = runType,
                ScopeType = scopeType,
                FromDate = fromDate,
                ToDate = toDate,
                AsOfDate = asOfDate,

                StatementSelectionMode = StatementSelectionModeType.ByDateRange,

                ReconciliationStatus = ReconciliationStatus.Draft,

                // Policy Snapshot
                AutoMatchEnabled = true,
                DateWindowDays = 2,
                AmountTolerance = 0,
                AllowManyToManyMatching = false,
                AllowAdjustmentCreationFromRecon = false,

                PreparedBy = user,
                PreparedOn = DateTime.UtcNow,

                CreatedAt = DateTime.UtcNow,
                CreatedBy = user,
                IsDeleted = false
            };

            _runs.Add(recon);
            return recon;
        }

        // ================= WORKBENCH =================

        public List<BankStatementLineModel> GetUnmatchedStatementLines(Guid reconId)
        {
            var recon = GetById(reconId);

            return _statementService
                .GetLinesByBankAccount(recon.BankAccountId)
                .Where(l =>
                    !l.IsDeleted &&
                    !_matches.Any(m =>
                        m.BankStatementLineId == l.BankStatementLineId &&
                        m.BankReconciliationId == reconId))
                .ToList();
        }

        public List<BankTransactionModel> GetUnmatchedTransactions(Guid reconId) 
        { 
            var recon = GetById(reconId); 
            return _transactionService.GetByBankAccountForMatch(recon.BankAccountId).
                Where(t => !_matches.Any(m => m.BankTransactionId == t.Id)).ToList(); 
        }
        // ================= MATCHING =================

        public void Match(Guid reconId, Guid statementLineId, Guid transactionId, string user = "system")
        {
            var recon = GetById(reconId);

            if (recon.ReconciliationStatus == ReconciliationStatus.Finalized)
                throw new Exception("Cannot match on finalized reconciliation");

            if (_matches.Any(m =>
                m.BankReconciliationId == reconId &&
                m.BankStatementLineId == statementLineId &&
                m.BankTransactionId == transactionId))
                return;

            var match = new BankReconciliationMatchModel
            {
                BankReconciliationMatchId = Guid.NewGuid(),
                BankReconciliationId = reconId,

                BankStatementLineId = statementLineId,
                BankTransactionId = transactionId,

                MatchedAmount = 0,
                MatchType = "Manual",
                ConfidenceScore = 100,

                MatchedBy = user,
                MatchedOn = DateTime.UtcNow
            };

            _matches.Add(match);

            RecalculateTotals(reconId);
            recon.ReconciliationStatus = ReconciliationStatus.InProgress;
        }

        public void Unmatch(Guid reconId, Guid matchId, string user = "system")
        {
            var recon = GetById(reconId);

            if (recon.ReconciliationStatus == ReconciliationStatus.Finalized)
                throw new Exception("Cannot unmatch a finalized reconciliation");

            var match = _matches.FirstOrDefault(m => m.BankReconciliationMatchId == matchId);
            if (match == null)
                throw new Exception("Match not found");

            _matches.Remove(match);
            RecalculateTotals(reconId);
        }

        // ================= AUTO MATCH =================

        public void RunAutoMatch(Guid reconId, string user = "system")
        {
            var recon = GetById(reconId);

            if (!recon.AutoMatchEnabled)
                throw new Exception("Auto Match Not Enabled");

            var lines = GetUnmatchedStatementLines(reconId);

            foreach (var line in lines)
            {
                var txns = GetUnmatchedTransactions(reconId);

                if (!txns.Any())
                    break;

                var match = txns.FirstOrDefault(t =>
                    Math.Abs(t.Amount - line.Amount) <= recon.AmountTolerance);

                if (match != null)
                {
                    Match(
                        reconId,
                        line.BankStatementLineId,
                        match.Id,
                        user
                    );
                }
                else
                {
                    throw new Exception("Match not found");
                }
            }
        }


        // ================= MATCH DATA =================

        public List<BankReconciliationMatchModel> GetMatches(Guid reconId)
        {
            return _matches
                .Where(m => m.BankReconciliationId == reconId)
                .ToList();
        }

        public List<MatchSuggestionView> GetSuggestions(Guid reconId, Guid statementLineId)
        {
            var txns = GetUnmatchedTransactions(reconId);

            return txns
                .Select(t => new MatchSuggestionView
                {
                    StatementLineId = statementLineId,
                    TransactionId = t.Id,
                    TransactionRef = t.TransactionNumber,
                    ConfidenceScore = 75
                })
                .Take(5)
                .ToList();
        }

        // ================= TOTALS =================

        private void RecalculateTotals(Guid reconId)
        {
            var recon = GetById(reconId);
            var matches = GetMatches(reconId);

            recon.MatchedCount = matches.Count;
            recon.MatchedStatementAmount = matches.Sum(m => m.MatchedAmount);
            recon.MatchedBookAmount = matches.Sum(m => m.MatchedAmount);

            recon.UnmatchedStatementCount = GetUnmatchedStatementLines(reconId).Count;
            recon.UnmatchedBookCount = GetUnmatchedTransactions(reconId).Count;
        }

        // ================= FINALIZE =================

        public void Finalize(Guid reconId, string user = "controller")
        {
            var recon = GetById(reconId);

            if (recon.ReconciliationStatus == ReconciliationStatus.Finalized)
                throw new Exception("Already finalized");

            if (recon.UnknownItemCount > 0)
                throw new Exception("Cannot finalize with unresolved unknown items");

            if (!recon.IsDifferenceWithinTolerance && recon.DifferenceAmount != 0)
                throw new Exception("Difference not approved or within tolerance");

            recon.ReconciliationStatus = ReconciliationStatus.Finalized;
            recon.FinalizedOn = DateTime.UtcNow;
            recon.FinalizedBy = user;
        }

        // ================= REOPEN =================

        public void Reopen(Guid reconId, string reason, string user = "controller")
        {
            var recon = GetById(reconId);

            if (recon.ReconciliationStatus != ReconciliationStatus.Finalized)
                throw new Exception("Only finalized runs can be reopened");

            if (string.IsNullOrWhiteSpace(reason))
                throw new Exception("Reopen reason is required");

            recon.ReconciliationStatus = ReconciliationStatus.Reopened;
            recon.ReopenedBy = user;
            recon.ReopenedOn = DateTime.UtcNow;
            recon.ReopenReason = reason;
        }

        public void MarkReviewCompleted(Guid reconId, string user = "reviewer")
        {
            var recon = GetById(reconId);

            if (recon.ReconciliationStatus != ReconciliationStatus.InProgress)
                throw new Exception("Only in-progress reconciliations can be reviewed");

            //if (recon.UnmatchedStatementCount > 0)
            //    throw new Exception("Cannot complete review with unmatched lines");

            recon.ReconciliationStatus = ReconciliationStatus.Completed;
            recon.UpdatedAt = DateTime.UtcNow;
            recon.UpdatedBy = user;
        }

        // ================= DELETE (SOFT) =================

        public void Delete(Guid reconId, string user = "system")
        {
            var recon = GetById(reconId);

            if (recon.ReconciliationStatus == ReconciliationStatus.Finalized)
                throw new Exception("Finalized reconciliation cannot be deleted");

            recon.IsDeleted = true;
            recon.UpdatedAt = DateTime.UtcNow;
            recon.UpdatedBy = user;
        }


        public BankReconciliationStatistics GetStatistics()
        {
            var Recon = GetAll();

            return new BankReconciliationStatistics
            {
                TotalReconciliation = Recon.Count(),
                DraftReconciliation = Recon.Count(re => re.ReconciliationStatus == ReconciliationStatus.Draft),
                InprogressReconciliation = Recon.Count(re => re.ReconciliationStatus == ReconciliationStatus.InProgress),
                CompletedReconciliation = Recon.Count(re => re.ReconciliationStatus == ReconciliationStatus.Completed),
                FinalizedReconciliation = Recon.Count(re => re.ReconciliationStatus == ReconciliationStatus.Finalized),
                ReopenedReconciliation = Recon.Count(re => re.ReconciliationStatus == ReconciliationStatus.Reopened),

            };
        }
    }
}
