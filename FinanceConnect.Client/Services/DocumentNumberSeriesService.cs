using FinanceConnect.Client.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FinanceConnect.Client.Services
{
    public class DocumentNumberSeriesService
    {
        private readonly MasterDataService _masterDataService;
        private readonly BranchService _branchService;
        private static List<DocumentNumberSeriesModel> _series = new();
        private readonly List<DocumentNumberSeriesModel> _seedSeries = new();
        private readonly Dictionary<string, long> _sequenceStore = new();
        // Key = SeriesId|ResetKey|ScopeKey
        private readonly List<CompanyModel> _companies = new();
        private readonly List<BranchModel> _branches = new();


        public DocumentNumberSeriesService(BranchService branchService,
            MasterDataService masterDataService)
        {
            _masterDataService = masterDataService;
            _branchService = branchService;
            _companies = _masterDataService
                .GetAllCompanies()
                .Where(c => c.Status == "Active")
                .ToList();
            _seedSeries = Seed();
            ResetToSeed();
        }

        private static List<T> CloneList<T>(IEnumerable<T> source) where T : class
        {
            var json = System.Text.Json.JsonSerializer.Serialize(source);
            return System.Text.Json.JsonSerializer.Deserialize<List<T>>(json) ?? new();
        }

        public void ResetToSeed()
        {
            _series = CloneList(_seedSeries);
        }

        public List<CompanyModel> GetCompanies()
        {
            return _masterDataService
                .GetAllCompanies()
                .Where(c => c.Status == "Active")
                .ToList();
        }

        public List<BranchModel> GetBranches()
        {
            return _branchService.GetAll()
                .Where(c => c.Status == "Active")
                .OrderBy(c => c.BranchName)
                .ToList();
        }
        public List<DocumentNumberSeriesModel> GetAll()
        {
            return _series
                .OrderBy(s => s.SeriesCode)
                .ToList();
        }

        public DocumentNumberSeriesModel? GetById(Guid id)
        {
            return _series.FirstOrDefault(s => s.DocumentNumberSeriesId == id);
        }

        public void createAsync(DocumentNumberSeriesModel model)
        {
            model.DocumentNumberSeriesId = Guid.NewGuid();
            model.CreatedAt = DateTime.UtcNow;
            model.IsActive = true;
            _series.Add(model);
        }

        public void UpdateAsync(DocumentNumberSeriesModel model)
        {
            var existing = GetById(model.DocumentNumberSeriesId);
            if (existing == null) return;
            model.NumericWidth = NumericWidth(model.SequenceTokenFormat);
            model.UpdatedAt = DateTime.UtcNow;

            int index = _series.IndexOf(existing);
            _series[index] = model;
        }

        private int NumericWidth(string format)
        {
            int pad = format.Count(c => c == '0');
            return pad+1;
        }
        public void Delete(Guid id)
        {
            var existing = GetById(id);
            if (existing == null) return;

            _series.Remove(existing);
        }

        // STATUS CONTROL
        public void Activate(Guid id)
        {
            var s = GetById(id);
            if (s == null) return;

            s.IsActive = true;
            s.IsLocked = false;
            s.UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate(Guid id)
        {
            var s = GetById(id);
            if (s == null) return;

            s.IsActive = false;
            s.UpdatedAt = DateTime.UtcNow;
        }

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


        public string Generate(Guid seriesId, DateTime date, Guid companyId, Guid? branchId)
        {
            var series = GetById(seriesId);
            if (series == null)
                throw new InvalidOperationException("Series not found");

            if (!series.IsActive || series.IsLocked)
                throw new InvalidOperationException("Series is inactive or locked");

             return GenerateNumber(series, date, companyId, branchId, preview: false);
        }

        private string GenerateNumber(
            DocumentNumberSeriesModel series,
            DateTime date,
            Guid companyId,
            Guid? branchId,
            bool preview)
        {
            string resetKey = BuildResetKey(series, date);
            string scopeKey = series.SequenceScopeMode == SequenceScopeMode.BranchSpecific
                ? branchId?.ToString() ?? "GLOBAL"
                : companyId.ToString();

            string sequenceKey = $"{series.DocumentNumberSeriesId}|{resetKey}|{scopeKey}";

            long nextValue = series.MinSequenceValue;

            if (_sequenceStore.ContainsKey(sequenceKey))
            {
                nextValue = _sequenceStore[sequenceKey] + series.IncrementBy;
            }

            if (series.MaxSequenceValue.HasValue &&
                nextValue > series.MaxSequenceValue.Value)
            {
                throw new InvalidOperationException("Series exhausted: max value reached");
            }

            if (!preview || series.ReservationMode == ReservationMode.AllocateOnAssignment)
            {
                _sequenceStore[sequenceKey] = nextValue;
            }

            return FormatNumber(series, nextValue, date, companyId, branchId);
        }

        // INTERNAL HELPERS

        private string BuildResetKey(DocumentNumberSeriesModel s, DateTime date)
        {
            return s.ResetFrequency switch
            {
                ResetFrequency.Never => "NEVER",
                ResetFrequency.Monthly => $"{date:yyyyMM}",
                ResetFrequency.Yearly =>
                    s.FiscalYearMode == FiscalYearMode.CompanyFiscalYear
                        ? GetFiscalYearKey(date)
                        : date.Year.ToString(),
                ResetFrequency.AccountingPeriod => $"PERIOD-{date:yyyyMM}",
                _ => "DEFAULT"
            };
        }

        private string GetFiscalYearKey(DateTime date)
        {
            int startYear = date.Month >= 4 ? date.Year : date.Year - 1;
            return $"FY{startYear}-{startYear + 1}";
        }

        private string FormatNumber(
            DocumentNumberSeriesModel s,
            long value,
            DateTime date,
            Guid companyId,
            Guid? branchId)
        {
            string seqFormatted = FormatSequence(s.SequenceTokenFormat, value);

            string prefix = ResolveTokens(s.PrefixTemplate, date, companyId, branchId);
            string suffix = ResolveTokens(s.SuffixTemplate, date, companyId, branchId);

            string sep = s.Separator ?? "/";

            return string.Join(sep, new[] { prefix, seqFormatted, suffix }
                .Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        private string FormatSequence(string format, long value)
        {
            int pad = format.Count(c => c == '0');
            return value.ToString().PadLeft(pad+1, '0');
        }

        private string ResolveTokens(
            string? template,
            DateTime date,
            Guid companyId,
            Guid? branchId)
        {
            if (string.IsNullOrWhiteSpace(template))
                return string.Empty;

            return template
                .Replace("{YYYY}", date.Year.ToString())
                .Replace("{YY}", (date.Year % 100).ToString("D2"))
                .Replace("{MM}", date.Month.ToString("D2"))
                .Replace("{DD}", date.Day.ToString("D2"))
                .Replace("{YYYYMM}", date.ToString("yyyy-MM"))
                .Replace("{FY}", GetFiscalYearKey(date))
                .Replace("{COMP}", "COMP")
                .Replace("{BRANCH}", branchId?.ToString().Substring(0, 4) ?? "MAIN")
                .Replace("{PERIOD}", date.ToString("yyyy-MM"));
        }

        // SEED DATA
        private List<DocumentNumberSeriesModel> Seed()
        {
            var list = new List<DocumentNumberSeriesModel>();

            foreach (var company in _companies)
            {
                list.Add(new DocumentNumberSeriesModel
                {
                    DocumentNumberSeriesId = Guid.NewGuid(),
                    CompanyId = company.Id,
                    SeriesCode = $"{company.LegalName.ToUpper().Replace(" ", "_").Substring(0, Math.Min(8, company.LegalName.Length))}_FT",
                    SeriesName = $"{company.LegalName} - Financial Transactions",
                    AppliesToEntityType = AppliesToEntityType.FinancialTransaction,

                    ResetFrequency = ResetFrequency.Yearly,
                    FiscalYearMode = FiscalYearMode.CompanyFiscalYear,
                    SequenceScopeMode = SequenceScopeMode.CompanyWide,

                    PrefixTemplate = "FT/2026",
                    SequenceTokenFormat = "00001",
                    Separator = "/",
                    SuffixTemplate = "FIN",

                    MinSequenceValue = 1,
                    MaxSequenceValue = 99999,
                    IncrementBy = 1,

                    AllowNumberPreview = true,
                    ReservationMode = ReservationMode.AllocateOnAssignment,
                    GapHandlingPolicy = GapHandlingPolicy.AllowGapsWithAudit,

                    IsSystemDefined = true,
                    EffectiveFrom = DateTime.Today.AddMonths(-1),
                    EffectiveTo = null,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                });

                list.Add(new DocumentNumberSeriesModel
                {
                    DocumentNumberSeriesId = Guid.NewGuid(),
                    CompanyId = company.Id,
                    SeriesCode = $"{company.LegalName.ToUpper().Replace(" ", "_").Substring(0, Math.Min(8, company.LegalName.Length))}_BR",
                    SeriesName = $"{company.LegalName} - Bank Receipts",
                    AppliesToEntityType = AppliesToEntityType.JournalEntry,

                    ResetFrequency = ResetFrequency.Yearly,
                    FiscalYearMode = FiscalYearMode.CompanyFiscalYear,
                    SequenceScopeMode = SequenceScopeMode.CompanyWide,

                    PrefixTemplate = "BR/202602",
                    SequenceTokenFormat = "0001",
                    Separator = "/",
                    SuffixTemplate = "JOR",

                    MinSequenceValue = 1,
                    MaxSequenceValue = 9999,
                    IncrementBy = 1,

                    AllowNumberPreview = true,
                    ReservationMode = ReservationMode.AllocateOnAssignment,
                    GapHandlingPolicy = GapHandlingPolicy.AllowGapsWithAudit,

                    IsSystemDefined = true,
                    EffectiveFrom = DateTime.Today.AddMonths(-1),
                    EffectiveTo = null,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                });
            }

            return list;
        }

    }
}
