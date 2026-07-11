using FinanceConnect.Client.Pages.Finance.Journal;
using FinanceConnect.Client.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinanceConnect.Client.Services
{
    public class DocumentSequenceService
    {
        private readonly MasterDataService _masterDataService;
        private readonly BranchService _branchService;
        private readonly DocumentNumberSeriesService _seriesService;
        private readonly AccountingPeriodService _accountingPeriodService;
        private readonly JournalService _journalService;
        private readonly JournalEntryService _journalEntryService;
        private readonly FinancialTransactionService _financialTransactionService;
        private readonly List<CompanyModel> _companies = new();
        private readonly List<BranchModel> _branches = new();
        private static List<DocumentSequenceModel> _sequence = new();
        private readonly List<DocumentSequenceModel> _seedSequence = new();

        public DocumentSequenceService(
            MasterDataService masterDataService,
            BranchService branchService,
            DocumentNumberSeriesService seriesService,
            AccountingPeriodService accountingPeriodService,
                JournalEntryService journalEntryService,
    FinancialTransactionService financialTransactionService,
    JournalService journalService)
        {
            _masterDataService = masterDataService;
            _branchService = branchService;
            _seriesService = seriesService;
            _accountingPeriodService = accountingPeriodService;
            _journalService = journalService;
            _journalEntryService = journalEntryService;
            _financialTransactionService = financialTransactionService;
            _companies = _masterDataService
                .GetAllCompanies()
                .Where(c => c.Status == "Active")
                .ToList();

            _seedSequence = Seed();
            ResetToSeed();
        }

        private static List<T> CloneList<T>(IEnumerable<T> source) where T : class
        {
            var json = System.Text.Json.JsonSerializer.Serialize(source);
            return System.Text.Json.JsonSerializer.Deserialize<List<T>>(json) ?? new();
        }

        public void ResetToSeed()
        {
            _sequence = CloneList(_seedSequence);
        }

        // ==============================
        // READ
        // ==============================
        public List<DocumentSequenceModel> GetAll()
        {
            return _sequence
                .OrderBy(s => s.SeriesCode)
                .ThenBy(s => s.ResetKey)
                .ToList();
        }

        public List<CompanyModel> GetCompanies()
        {
            return _masterDataService
            .GetAllCompanies()
            .Where(c => c.Status == "Active")
            .ToList();
        }

        public void Delete(Guid id)
        {
            var seq = _sequence.FirstOrDefault(x => x.DocumentSequenceId == id);
            if (seq == null) return;

            // Enterprise rule: do NOT delete if already issued
            if (seq.LastIssuedAt.HasValue || seq.CurrentValue > 0)
                throw new InvalidOperationException("Cannot delete a sequence that has allocations");

            _sequence.Remove(seq);
        }
        public DocumentSequenceModel? GetById(Guid id)
        {
            return _sequence.FirstOrDefault(x => x.DocumentSequenceId == id);
        }

        public DocumentSequenceModel? GetByDocNumberSeriesId(Guid id)
        {
            return _sequence.FirstOrDefault(x => x.DocumentNumberSeriesId == id);
        }

        // ALLOCATION ENGINE
        public long AllocateNext(
            Guid companyId,
            Guid seriesId,
            string seriesCode,
            ResetFrequency resetFrequency,
            string resetKey,
            BranchScopeMode scopeMode,
            Guid? branchId,
            string reference,
            string userId)
        {
            var seq = FindOrCreateSequence(
                companyId,
                seriesId,
                seriesCode,
                resetFrequency,
                resetKey,
                scopeMode,
                branchId
            );

            if (!seq.IsActive)
                throw new InvalidOperationException("Sequence inactive");

            if (seq.IsLocked)
                throw new InvalidOperationException("Sequence locked");

            long next = seq.CurrentValue + seq.IncrementBy;

            if (seq.MaxValue.HasValue && next > seq.MaxValue.Value)
            {
                seq.IsExhausted = true;
                throw new InvalidOperationException("Sequence exhausted");
            }

            seq.CurrentValue = next;
            seq.LastIssuedAt = DateTime.UtcNow;
            seq.LastIssuedBy = userId;
            seq.LastIssuedToReference = reference;
            seq.UpdatedAt = DateTime.UtcNow;
            seq.RowVersion = Guid.NewGuid().ToByteArray();

            return next;
        }

        // ADMIN ACTIONS
        public void Lock(Guid id)
        {
            var s = GetById(id);
            if (s == null) return;

            s.IsLocked = true;
            s.UpdatedAt = DateTime.UtcNow;
        }

        public void Unlock(Guid id)
        {
            var s = GetById(id);
            if (s == null) return;

            s.IsLocked = false;
            s.UpdatedAt = DateTime.UtcNow;
        }


        public void Reset(Guid sequenceId, string performedBy = "CONTROLLER")
        {
            var seq = _sequence.FirstOrDefault(x => x.DocumentSequenceId == sequenceId);
            if (seq == null)
                throw new InvalidOperationException("Sequence not found");

            if (seq.IsLocked)
                throw new InvalidOperationException("Cannot reset a locked sequence");

            long oldValue = seq.CurrentValue;

            // Reset back to MinValue - Increment (so next = MinValue)
            long minValue = seq.MinValue ?? 1;
            seq.CurrentValue = minValue - seq.IncrementBy;

            seq.IsExhausted = false;
            seq.LastIssuedAt = null;
            seq.LastIssuedBy = null;
            seq.UpdatedAt = DateTime.UtcNow;
        }
        public void Adjust(
            Guid sequenceId,
            long newCurrentValue,
            string reason,
            string performedBy = "CONTROLLER")
        {
            var seq = _sequence.FirstOrDefault(x => x.DocumentSequenceId == sequenceId);
            if (seq == null)
                throw new InvalidOperationException("Sequence not found");

            if (seq.IsLocked)
                throw new InvalidOperationException("Cannot adjust a locked sequence");

            if (string.IsNullOrWhiteSpace(reason))
                throw new InvalidOperationException("Adjustment reason is required");

            if (seq.MinValue.HasValue && newCurrentValue < seq.MinValue.Value)
                throw new InvalidOperationException("New value is below MinValue");

            if (seq.MaxValue.HasValue && newCurrentValue > seq.MaxValue.Value)
                throw new InvalidOperationException("New value exceeds MaxValue");

            long oldValue = seq.CurrentValue;

            seq.CurrentValue = newCurrentValue;
            seq.IsExhausted = seq.MaxValue.HasValue &&
                              newCurrentValue >= seq.MaxValue.Value;

            seq.UpdatedAt = DateTime.UtcNow;

        }


        // INTERNAL
        private DocumentSequenceModel FindOrCreateSequence(
            Guid companyId,
            Guid seriesId,
            string seriesCode,
            ResetFrequency resetFrequency,
            string resetKey,
            BranchScopeMode scopeMode,
            Guid? branchId)
        {
            var seq = _sequence.FirstOrDefault(x =>
                x.CompanyId == companyId &&
                x.DocumentNumberSeriesId == seriesId &&
                x.ResetKey == resetKey &&
                x.BranchScopeMode == scopeMode &&
                (scopeMode == BranchScopeMode.CompanyWide ||
                 x.BranchId == branchId));

            if (seq != null)
                return seq;

            seq = new DocumentSequenceModel
            {
                TenantId = Guid.Empty,
                CompanyId = companyId,
                DocumentNumberSeriesId = seriesId,
                SeriesCode = seriesCode,
                ResetFrequency = resetFrequency,
                ResetKey = resetKey,
                BranchScopeMode = scopeMode,
                BranchId = scopeMode == BranchScopeMode.BranchSpecific ? branchId : null,
                CurrentValue = 0
            };

            _sequence.Add(seq);
            return seq;
        }


        private string GetAccountingPeriodResetKey(Guid? companyId, DateTime date)
        {
            var period = _accountingPeriodService
                .GetAll()
                .FirstOrDefault(p =>
                    p.CompanyId == companyId &&
                    p.StartDate <= date &&
                    p.EndDate >= date);

            if (period == null)
                throw new Exception("No accounting period found for reset key generation.");

            return period.PeriodCode; // ⭐ BEST ERP PRACTICE
        }

        private long ResolveCurrentValue(DocumentNumberSeriesModel s)
        {
            switch (s.AppliesToEntityType)
            {
                // ================= JOURNAL ENTRY =================
                case AppliesToEntityType.JournalEntry:

                    // 1️⃣ Get journals using this series
                    var journalIds = _journalService
                        .GetAll()
                        .Where(j =>
                            j.CompanyId == s.CompanyId &&
                            j.DocumentNumberSeriesId == s.DocumentNumberSeriesId)
                        .Select(j => j.Id)
                        .ToHashSet();

                    // 2️⃣ Count entries belonging to those journals
                    return _journalEntryService
                        .GetAll()
                        .Count(e =>
                            e.CompanyId == s.CompanyId &&
                            journalIds.Contains(e.JournalId!.Value));


                // ================= FINANCIAL TRANSACTION =================
                case AppliesToEntityType.FinancialTransaction:

                    return _financialTransactionService
                        .GetAll()
                        .Count(t =>
                            t.CompanyId == s.CompanyId &&
                            t.DocumentNumberSeriesId == s.DocumentNumberSeriesId);


                default:
                    return 0;
            }
        }





        // SEED DATA
        private List<DocumentSequenceModel> Seed()
        {
            if (_sequence.Any())
                return _sequence;

            var now = DateTime.UtcNow;

            var seriesList = _seriesService
                .GetAll()
                .Where(s => s.IsActive)
                .ToList();

            foreach (var s in seriesList)
            {
                // ---------- Reset Key ----------
                string resetKey = s.ResetFrequency switch
                {
                    ResetFrequency.Never => "GLOBAL",

                    ResetFrequency.Yearly =>
                        $"FY{now.Year}-{now.Year + 1}",

                    ResetFrequency.Monthly =>
                        now.ToString("yyyyMM"),

                    ResetFrequency.AccountingPeriod =>
                        GetAccountingPeriodResetKey(s.CompanyId, now),

                    _ => "GLOBAL"
                };

                // ---------- Resolve current value from real transactions ----------
                long currentValue = ResolveCurrentValue(s);

                // ---------- Company-wide sequence ----------
                if (s.SequenceScopeMode == SequenceScopeMode.CompanyWide)
                {
                    _sequence.Add(new DocumentSequenceModel
                    {
                        CompanyId = s.CompanyId,
                        SeriesCode = s.SeriesCode,
                        DocumentNumberSeriesId = s.DocumentNumberSeriesId,

                        ResetFrequency = s.ResetFrequency,
                        ResetKey = resetKey,

                        BranchScopeMode = BranchScopeMode.CompanyWide,
                        BranchId = null,

                        CurrentValue = currentValue,
                        IncrementBy = s.IncrementBy,
                        IsActive = true,

                        LastIssuedAt = null,
                        LastIssuedBy = "seed"
                    });
                }

                // ---------- Branch-specific sequence ----------
                else if (s.SequenceScopeMode == SequenceScopeMode.BranchSpecific)
                {
                    var branches = _branchService
                        .GetAll()
                        .Where(b => b.CompanyId == s.CompanyId && b.Status == "Active")
                        .ToList();

                    foreach (var branch in branches)
                    {
                        _sequence.Add(new DocumentSequenceModel
                        {
                            CompanyId = s.CompanyId,
                            SeriesCode = s.SeriesCode,
                            DocumentNumberSeriesId = s.DocumentNumberSeriesId,

                            ResetFrequency = s.ResetFrequency,
                            ResetKey = resetKey,

                            BranchScopeMode = BranchScopeMode.BranchSpecific,
                            BranchId = branch.Id,

                            CurrentValue = currentValue,
                            IncrementBy = s.IncrementBy,
                            IsActive = true,

                            LastIssuedAt = null,
                            LastIssuedBy = "seed"
                        });
                    }
                }
            }

            return _sequence;
        }


    }
}
