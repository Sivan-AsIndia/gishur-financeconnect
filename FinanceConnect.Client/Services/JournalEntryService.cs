using FinanceConnect.Client.Data;
using FinanceConnect.Client.Pages.Finance.Journal;
using FinanceConnect.Client.ViewModels;
using System.Net.NetworkInformation;
using static FinanceConnect.Client.Data.MasterDataIds;

namespace FinanceConnect.Client.Services
{
    public class JournalEntryService
    {
        private readonly MasterDataService _masterDataService;
        private readonly BranchService _branchService;
        private readonly JournalService _journalService;
        private readonly FiscalYearService _fiscalYearService;
        private readonly AccountingPeriodService _accPeriodService;
        private readonly DocumentNumberSeriesService _docNumSeriesService;
        private readonly COADataService _coa;
        private readonly JournalLineService _lineDomain;

        private static List<JournalEntryModel> _entries = new();
        private readonly List<JournalEntryModel> _seedEntries = new();
        private readonly List<JournalLineModel> _lines = new();   // ⭐ Lines owned here


        public JournalEntryService(
            BranchService branchService,
            JournalService journalService,
            MasterDataService masterDataService,
            FiscalYearService fiscalYearService,
            AccountingPeriodService accPeriodService,
            DocumentNumberSeriesService docNumSeriesService,
            COADataService coa,
            JournalLineService lineDomain)
        {
            _masterDataService = masterDataService;
            _branchService = branchService;
            _journalService = journalService;
            _fiscalYearService = fiscalYearService;
            _accPeriodService = accPeriodService;
            _docNumSeriesService = docNumSeriesService;
            _coa = coa;
            _lineDomain = lineDomain;

        }

        public async Task InitializeAsync()
        {
            if (_seedEntries.Any())
                return;

            var seeds = await SeedEntriesAsync();

            _seedEntries.Clear();
            _seedEntries.AddRange(CloneList(seeds));

            await ResetToSeed();
        }

        private static List<T> CloneList<T>(IEnumerable<T> source) where T : class
        {
            var json = System.Text.Json.JsonSerializer.Serialize(source);
            return System.Text.Json.JsonSerializer.Deserialize<List<T>>(json) ?? new();
        }

        public Task ResetToSeed()
        {
            _entries.Clear();
            _entries.AddRange(CloneList(_seedEntries));

            _lines.Clear();

            return Task.CompletedTask;
        }

        public List<JournalLineModel> GetByEntry(Guid entryId)
        {
            return _lines
                .Where(x => x.JournalEntryId == entryId && !x.IsDeleted)
                .OrderBy(x => x.LineNumber)
                .ToList();
        }


        public string GetCompanyName(Guid? id)
        {
            return _masterDataService
                .GetAllCompanies()
                .FirstOrDefault(x => x.Id == id && x.Status == "Active")
                ?.LegalName ?? "-";
        }

        public string GetBranchName(Guid? id)
        {
            return _branchService.GetAll()
                .FirstOrDefault(x => x.Id == id)
                ?.BranchName ?? "-";
        }

        public string GetJournalName(Guid? id)
        {
            return _journalService.GetAll()
                .FirstOrDefault(x => x.Id == id)
                ?.JournalName ?? "-";
        }

        public string GetJournalCode(Guid? id)
        {
            return _journalService.GetAll()
                .FirstOrDefault(x => x.Id == id)
                ?.JournalCode ?? "-";
        }

        public string GetLedgerName(Guid? ledgerId)
        {
            return _journalService.GetLedgerById(ledgerId)?.LedgerName ?? "-";
        }

        public string GetAccountDisplayName(Guid? accountId)
        {
            if (!accountId.HasValue) return "-";
            var acc = _coa.GetAllAccounts().FirstOrDefault(a => a.Id == accountId.Value);
            return acc != null ? $"{acc.AccountCode} - {acc.AccountName}" : "-";
        }

        public string GetBranchCode(Guid? id)
        {
            return _branchService.GetAll()
                .FirstOrDefault(x => x.Id == id)
                ?.BranchCode ?? "-";
        }

        public List<BranchModel> GetBranchesByCompany(Guid companyId)
    => _branchService.GetAll()
        .Where(b => b.CompanyId == companyId && b.Status == "Active")
        .ToList();

        public List<JournalModel> GetJournalsByCompany(Guid companyId)
            => _journalService.GetAll()
                .Where(j => j.CompanyId == companyId && j.Status == JournalStatus.Active)
                .ToList();
        public async Task<List<AccountViewModel>> GetPostableAccounts(Guid companyId)
        {
            var coas = await _coa.GetChartOfAccountsAsync();

            var coaIds = coas
                .Where(c => c.CompanyId == companyId && c.IsActive && c.Status == COAStatuses.Active)
                .Select(c => c.Id)
                .ToHashSet();

            var accounts = _coa.GetAllAccounts()
                .Where(a => a.IsActive && coaIds.Contains(a.ChartOfAccountsId.Value))
                .ToList();

            return accounts;
        }

        public (FiscalYearModel? FiscalYear, AccountingPeriodModel? AccountingPeriod, string? Error) GetFyByDate(JournalEntryModel entry)
        {
            var fiscalYear = _fiscalYearService.GetAll().FirstOrDefault(fy =>
                entry.PostingDate >= fy.StartDate &&
                 entry.PostingDate <= fy.EndDate && fy.CompanyId == entry.CompanyId && fy.Status == FiscalYearStatus.Open);

            if (fiscalYear == null)
            {
                return (null, null, "No active fiscal year found for this company/date.");
            }
            var AccountingPeriod = GetAccPeriodByDate(entry, fiscalYear);
            if (AccountingPeriod == null)
            {
                return (fiscalYear, null, "No open period found");
            }

            return (fiscalYear, AccountingPeriod, null);
        }

        public AccountingPeriodModel? GetAccPeriodByDate(JournalEntryModel entry, FiscalYearModel fiscalYear)
        {
            var AccountingPeriod = _accPeriodService.GetAll().FirstOrDefault(acc =>
                 acc.FiscalYearId == fiscalYear.Id &&
                entry.PostingDate >= acc.StartDate &&
                 entry.PostingDate <= acc.EndDate && acc.CompanyId == entry.CompanyId && acc.Status == AccountingPeriodStatus.Open);


            return AccountingPeriod;
        }
        public List<JournalEntryModel> GetAll()
            => _entries.OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt).ToList();

        public JournalEntryModel? GetById(Guid id)
            => _entries.FirstOrDefault(x => x.Id == id);

        public List<JournalLineModel> GetLines(Guid entryId)
        {
            return _lines
                .Where(x => x.JournalEntryId == entryId && !x.IsDeleted)
                .OrderBy(x => x.LineNumber)
                .ToList();
        }


        public Guid CreateDraft(JournalEntryModel entry ,JournalModel journal)
        {
            ValidateHeader(entry);

            entry.JournalEntryNumber = GenerateJournalEntryNumber(
                journal,
                entry.PostingDate,
                entry.CompanyId!.Value,
                entry.BranchId);

            if (_entries.Any(x =>
                x.JournalEntryNumber == entry.JournalEntryNumber &&
                x.CompanyId == entry.CompanyId))
                throw new Exception("Duplicate journal entry number");

            entry.Id = Guid.NewGuid();
            entry.Status = JournalEntryStatus.Draft;
            entry.CreatedAt = DateTime.UtcNow;
            entry.CreatedBy = "user";

            _entries.Add(entry);

            return entry.Id;
        }

        public void UpdateDraft(JournalEntryModel entry)
        {
            var existing = _entries.FirstOrDefault(x => x.Id == entry.Id) ?? 
                throw new Exception("Journal entry not found");
            if (existing.Status != JournalEntryStatus.Draft)
                throw new Exception("Only draft entries can be edited");
            ValidateHeader(entry);
            existing.JournalEntryNumber = entry.JournalEntryNumber;
            existing.CompanyId = entry.CompanyId;
            existing.BranchId = entry.BranchId;
            existing.JournalId = entry.JournalId;
            existing.LedgerId = entry.LedgerId;
            existing.EntryDate = entry.EntryDate;
            existing.PostingDate = entry.PostingDate;
            existing.Narration = entry.Narration;
            existing.ExternalReferenceNumber = entry.ExternalReferenceNumber;
            existing.ReferenceType = entry.ReferenceType;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = "user"; 
        }
        public void SaveDraft(Guid entryId)
        {
            var entry = Get(entryId);

            ValidateHeader(entry);

            if (entry.Status != JournalEntryStatus.Draft)
                throw new Exception("Only draft entries can be saved");

            entry.UpdatedAt = DateTime.UtcNow;
            entry.UpdatedBy = "user";
        }

        public JournalLineModel AddLine(JournalLineModel line)
        {
            var entry = Get(line.JournalEntryId);

            EnsureEditable(entry);
            _lineDomain.ValidateLine(line, entry);

            line.Id = Guid.NewGuid();
            line.LineNumber = GetNextLineNumber(entry.Id);
            line.BranchId = entry.BranchId!.Value;
            line.CreatedAt = DateTime.UtcNow;
            line.CreatedBy = "user";

            _lines.Add(line);

            RecalculateTotals(entry);
            return line;
        }

        public void UpdateLine(JournalLineModel updated)
        {
            var existing = _lines.FirstOrDefault(x => x.Id == updated.Id)
                ?? throw new Exception("Journal line not found");

            var entry = Get(existing.JournalEntryId);

            EnsureEditable(entry);
            _lineDomain.ValidateLine(updated, entry);

            updated.Id = existing.Id;
            updated.JournalEntryId = existing.JournalEntryId;
            updated.LineNumber = existing.LineNumber;
            updated.BaseCurrencyId = existing.BaseCurrencyId;

            updated.UpdatedAt = DateTime.UtcNow;
            updated.UpdatedBy = "user";

            _lines.Remove(existing);
            _lines.Add(updated);

            RecalculateTotals(entry);
        }

        public void DeleteLine(Guid lineId)
        {
            var line = _lines.FirstOrDefault(x => x.Id == lineId)
                ?? throw new Exception("Line not found");

            var entry = Get(line.JournalEntryId);

            EnsureEditable(entry);

            line.IsDeleted = true;
            line.UpdatedAt = DateTime.UtcNow;
            line.UpdatedBy = "user";

            RecalculateTotals(entry);
        }


        private void RecalculateTotals(JournalEntryModel entry)
        {
            var lines = GetLines(entry.Id);

            entry.TotalDebit = lines.Sum(x => x.DebitAmount);
            entry.TotalCredit = lines.Sum(x => x.CreditAmount);
            entry.LineCount = lines.Count;
        }

        private int GetNextLineNumber(Guid entryId)
        {
            var last = _lines
                .Where(x => x.JournalEntryId == entryId)
                .OrderByDescending(x => x.LineNumber)
                .FirstOrDefault();

            return last == null ? 10 : last.LineNumber + 10;
        }

        public void DeleteDraft(Guid id)
        {
            var e = _entries.FirstOrDefault(x => x.Id == id) ?? 
                throw new Exception("Journal Entry not found");
            if (e.Status != JournalEntryStatus.Draft)
                throw new Exception("Only Draft entries can be deleted");
            _entries.Remove(e);
        }
        public void Submit(Guid id)
        {
            var e = Get(id);

            if (e.Status != JournalEntryStatus.Draft)
                throw new Exception("Only draft entries can be submitted");

            EnsureBalanced(e);

            if (e.LineCount <= 0)
                throw new Exception("At least one journal line is required");

            e.Status = JournalEntryStatus.Submitted;
            e.SubmittedAt = DateTime.UtcNow;
            e.SubmittedBy = "user";
        }

        public void Approve(Guid id)
        {
            var e = Get(id);

            if (e.Status != JournalEntryStatus.Submitted)
                throw new Exception("Only submitted entries can be approved");

            e.Status = JournalEntryStatus.Approved;
            e.ApprovedAt = DateTime.UtcNow;
            e.ApprovedBy = "controller";
        }

        public void Post(Guid id)
        {
            var e = Get(id);

            if (e.Status != JournalEntryStatus.Approved)
                throw new Exception("Only approved entries can be posted");

            EnsureBalanced(e);
            EnsurePeriodOpen(e);

            if (e.LineCount <= 0)
                throw new Exception("No journal lines exist");

            e.Status = JournalEntryStatus.Posted;
            e.PostedAt = DateTime.UtcNow;
            e.PostedBy = "controller";
        }

        public void Reject(Guid id)
        { 
            var e = Get(id); 
            if (e.Status != JournalEntryStatus.Submitted)
                throw new Exception("Only submitted entries can be rejected");
            e.Status = JournalEntryStatus.Draft;
            e.RejectedAt = DateTime.UtcNow; 
            e.RejectedBy = "controller";
        }
        public void Cancel(Guid id)
        { 
            var e = Get(id);
            if (e.Status != JournalEntryStatus.Posted)
                throw new Exception("Only posted entries can be cancelled");
            e.Status = JournalEntryStatus.Cancelled;
            e.CancelledAt = DateTime.UtcNow;
            e.CancelledBy = "admin";
        }


        private void ValidateHeader(JournalEntryModel e)
        {
            if (!e.CompanyId.HasValue)
                throw new Exception("Company is required");

            if (!e.BranchId.HasValue)
                throw new Exception("Branch is required");

            if (!e.JournalId.HasValue)
                throw new Exception("Journal is required");

            if (e.EntryDate == default || e.PostingDate == default)
                throw new Exception("Entry and Posting dates are required");
        }

        private void EnsureBalanced(JournalEntryModel e)
        {
            if (e.TotalDebit != e.TotalCredit)
                throw new Exception("Debit and Credit must be equal");
        }

        private void EnsurePeriodOpen(JournalEntryModel e)
        {
            if (e.IsPeriodClosed)
                throw new Exception("Accounting period is closed");
        }

        private void EnsureEditable(JournalEntryModel entry)
        {
            if (entry.Status != JournalEntryStatus.Draft)
                throw new Exception("Only draft entries are editable");
        }

        private JournalEntryModel Get(Guid id)
            => GetById(id) ?? throw new Exception("Journal entry not found");


        private async Task<List<JournalEntryModel>> SeedEntriesAsync()
        {
            var seeded = new List<JournalEntryModel>();

            var today = DateTime.UtcNow.Date;

            var companies = _masterDataService
                .GetAllCompanies()
                .Where(c => c.Status == "Active")
                .ToList();

            foreach (var company in companies)
            {
                var branch = _branchService.GetAll()
                    .FirstOrDefault(b => b.CompanyId == company.Id && b.Status == "Active");

                var journal = _journalService.GetAll()
                    .FirstOrDefault(j =>
                        j.CompanyId == company.Id &&
                        j.Status == JournalStatus.Active &&
                        j.DocumentNumberSeriesId.HasValue);

                if (branch == null || journal == null)
                    continue;

                seeded.Add(await CreateSeedEntryAsync(company.Id, branch.Id, journal,
                    today, "Draft expense adjustment", 5000, JournalEntryStatus.Draft));

                seeded.Add(await CreateSeedEntryAsync(company.Id, branch.Id, journal,
                    today, "Submitted purchase entry", 12000, JournalEntryStatus.Submitted));

                seeded.Add(await CreateSeedEntryAsync(company.Id, branch.Id, journal,
                    today, "Approved sales adjustment", 18000, JournalEntryStatus.Approved));

                seeded.Add(await CreateSeedEntryAsync(company.Id, branch.Id, journal,
                    today, "Posted bank transaction", 25000, JournalEntryStatus.Posted));
            }

            return seeded;
        }



        private async Task<JournalEntryModel> CreateSeedEntryAsync(
            Guid companyId,
            Guid branchId,
            JournalModel journal,
            DateTime postingDate,
            string narration,
            decimal amount,
            JournalEntryStatus finalStatus)
        {
            var (fy, period) = GetOpenFiscalContext(companyId, postingDate);

            var entryNumber = _docNumSeriesService.Generate(
                journal.DocumentNumberSeriesId!.Value,
                postingDate,
                companyId,
                branchId);

            var entry = new JournalEntryModel
            {
                Id = Guid.NewGuid(),
                JournalEntryNumber = entryNumber,
                CompanyId = companyId,
                BranchId = branchId,
                JournalId = journal.Id,
                LedgerId = journal.LedgerId,
                EntryDate = postingDate,
                PostingDate = postingDate,
                Narration = narration,
                FiscalYearId = fy.Id,
                FiscalYearName = fy.FiscalYearName,
                AccountingPeriodId = period.Id,
                AccountingPeriodName = period.PeriodName,
                Status = JournalEntryStatus.Draft,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "seed"
            };

            // ⭐ VERY IMPORTANT
            _entries.Add(entry);

            await SeedBalancedLinesAsync(entry, amount);

            if (finalStatus >= JournalEntryStatus.Submitted)
                Submit(entry.Id);

            if (finalStatus >= JournalEntryStatus.Approved)
                Approve(entry.Id);

            if (finalStatus == JournalEntryStatus.Posted)
                Post(entry.Id);

            return entry;
        }


        private async Task SeedBalancedLinesAsync(JournalEntryModel entry, decimal amount)
        {
            if (!entry.CompanyId.HasValue)
                throw new Exception("Company is required for journal entry.");

            var account = await GetSeedAccountAsync(entry.CompanyId.Value);

            if (account == null || account.Id == Guid.Empty)
                return;

            var company = _masterDataService
                .GetAllCompanies()
                .FirstOrDefault(x => x.Id == entry.CompanyId.Value)
                ?? throw new Exception("Company not found.");

            // Debit
            _lines.Add(new JournalLineModel
            {
                Id = Guid.NewGuid(),
                JournalEntryId = entry.Id,
                LineNumber = 10,
                BranchId = entry.BranchId,
                AccountId = account.Id,
                DebitAmount = amount,
                CreditAmount = 0,
                LineNarration = "Seed debit",
                BaseCurrencyId = company.BaseCurrencyId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "seed"
            });

            // Credit
            _lines.Add(new JournalLineModel
            {
                Id = Guid.NewGuid(),
                JournalEntryId = entry.Id,
                LineNumber = 20,
                BranchId = entry.BranchId,
                AccountId = account.Id,
                DebitAmount = 0,
                CreditAmount = amount,
                LineNarration = "Seed credit",
                BaseCurrencyId = company.BaseCurrencyId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "seed"
            });

            entry.TotalDebit = amount;
            entry.TotalCredit = amount;
            entry.LineCount = 2;
        }




        private async Task<AccountViewModel> GetSeedAccountAsync(Guid companyId)
        {
            var accounts = await GetPostableAccounts(companyId);

            var account = accounts.FirstOrDefault();

            if (account == null)
            {
                Console.WriteLine("⚠ No postable accounts found for seeding.");
                return new AccountViewModel();
            }

            return account;
        }



        public string GenerateJournalEntryNumber(
            JournalModel journal,
            DateTime postingDate,
            Guid companyId,
            Guid? branchId)
        {
            if (!journal.DocumentNumberSeriesId.HasValue)
                throw new InvalidOperationException("Journal number series not configured.");

            // ⭐ Delegate to central numbering engine
            return _docNumSeriesService.Generate(
                journal.DocumentNumberSeriesId.Value,
                postingDate,
                companyId,
                branchId);
        }


        private (FiscalYearModel fy, AccountingPeriodModel period)
    GetOpenFiscalContext(Guid companyId, DateTime postingDate)
        {
            var fy = _fiscalYearService.GetAll().FirstOrDefault(x =>
                x.CompanyId == companyId &&
                x.StartDate <= postingDate &&
                x.EndDate >= postingDate &&
                (x.Status == FiscalYearStatus.Open || x.Status == FiscalYearStatus.SoftClosed));

            if (fy == null)
                throw new InvalidOperationException(
                    $"No open fiscal year found for company {companyId} on {postingDate:yyyy-MM-dd}.");

            var period = _accPeriodService.GetAll().FirstOrDefault(p =>
                p.CompanyId == companyId &&
                p.FiscalYearId == fy.Id &&
                p.StartDate <= postingDate &&
                p.EndDate >= postingDate &&
                (p.Status == AccountingPeriodStatus.Open || p.Status == AccountingPeriodStatus.SoftClosed));

            if (period == null)
                throw new InvalidOperationException(
                    $"No open accounting period found for company {companyId} on {postingDate:yyyy-MM-dd}.");

            return (fy, period);
        }


        public bool AnyEntries()
        {
            return _entries.Any();
        }

    }
}
