using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;
using System.Xml.Linq;
using static FinanceConnect.Client.Data.MasterDataIds;

namespace FinanceConnect.Client.Services
{
    public class FiscalYearService
    {
        private static List<FiscalYearModel> _years = new();
        private static List<FiscalYearModel> _SeedYears = new();
        private readonly AccountingPeriodService _periodService;
        private readonly MasterDataService _masterDataService;
        private readonly List<CompanyModel> _companies = new();

        public FiscalYearService(AccountingPeriodService periodService, MasterDataService masterDataService)
        {
            _periodService = periodService;
            _masterDataService = masterDataService;
            _companies = _masterDataService
                .GetAllCompanies()
                .Where(c => c.Status == "Active")
                .ToList();
            _SeedYears = SeedFiscalYears();
            ResetToSeed();
        }

        private static List<T> CloneList<T>(IEnumerable<T> source) where T : class
        {
            var json = System.Text.Json.JsonSerializer.Serialize(source);
            return System.Text.Json.JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }

        /// <summary>Reset vendors to seed data</summary>
        public void ResetToSeed()
        {
            _years = CloneList(_SeedYears);
        }
        public void GeneratePeriodsForSeededFiscalYears()
        {
            foreach (var fy in _years)
            {
                _periodService.GenerateForFiscalYear(fy);
            }
        }


        public List<FiscalYearModel> GetAll()
             => _years
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToList();
        public List<FiscalYearModel> GetAllByCompanyId(Guid companyId)
            => _years
                .Where(x => x.CompanyId == companyId)
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToList();

        public FiscalYearModel? GetById(Guid id)
            => _years.FirstOrDefault(x => x.Id == id);

        public void Create(FiscalYearModel fy)
        {
            ValidateFiscalYear(fy, isEdit: false);
            fy.CreatedAt = DateTime.UtcNow;
            _years.Add(fy);
            _periodService.GenerateForFiscalYear(fy);
        }


        public void Update(FiscalYearModel fy)
        {
            var existing = GetById(fy.Id);
            if (existing == null)
                throw new Exception("Fiscal year not found");

            if (existing.Status != FiscalYearStatus.Draft)
                throw new Exception("Only Draft fiscal years can be edited");

            ValidateFiscalYear(fy, isEdit: true);
            fy.UpdatedAt = DateTime.UtcNow;
            _years.Remove(existing);
            _years.Add(fy);
        }

        public void Delete(Guid id)
        {
            var years = _years.FirstOrDefault(x => x.Id == id);
            if (years != null)
                _years.Remove(years);
        }

        private void ValidateFiscalYear(FiscalYearModel fy, bool isEdit)
        {
            if (fy.CompanyId == null)
                throw new Exception("Company is required");

            if (fy.StartDate == null || fy.EndDate == null)
                throw new Exception("Start Date and End Date are required");


            if (fy.StartDate >= fy.EndDate)
                throw new Exception("Start Date must be earlier than End Date");

            if (_years.Any(x =>
                x.CompanyId == fy.CompanyId &&
                x.FiscalYearCode == fy.FiscalYearCode &&
                (!isEdit || x.Id != fy.Id)))
            {
                throw new Exception("Fiscal Year Code already exists for this company");
            }


            bool overlaps = _years.Any(x =>
                x.CompanyId == fy.CompanyId &&
                (!isEdit || x.Id != fy.Id) &&
                fy.StartDate <= x.EndDate &&
                fy.EndDate >= x.StartDate);

            if (overlaps)
                throw new Exception("Fiscal year date range overlaps with an existing fiscal year for this company");
        }


        public void ChangeStatus(Guid id, FiscalYearStatus newStatus, string? reason = null)
        {
            var fy = GetById(id);
            if (fy == null) return;

            if (newStatus == FiscalYearStatus.Closed && string.IsNullOrWhiteSpace(reason))
                throw new Exception("Close reason is required");

            if (newStatus == FiscalYearStatus.Closed)
            {
                var periods = _periodService.GetByFiscalYear(id);

                bool hasOpenOrSoftClosed = periods.Any(p => p.Status != AccountingPeriodStatus.Closed);

                if (hasOpenOrSoftClosed)
                    throw new Exception("All accounting periods must be closed before closing the fiscal year.");
            }

            fy.Status = newStatus;
            fy.CloseReason = reason;
            fy.ClosedAt = newStatus == FiscalYearStatus.Closed ? DateTime.UtcNow : null;
            fy.UpdatedAt = DateTime.UtcNow;
        }

        private List<FiscalYearModel> SeedFiscalYears()
        {

            var today = DateTime.UtcNow.Date;

            foreach (var company in _companies)
            {
                // Determine CURRENT FY start based on company FY start month
                var currentFyStartYear =
                    today.Month >= company.FiscalYearStartMonth ? today.Year : today.Year - 1;

                var oldFyStartYear = currentFyStartYear - 1;

                // ── Assign stable IDs for SofaCraft so GL / OB / CB seed data can reference them ──
                bool isSofaCraft = company.Id == MasterDataIds.Companies.SofaCraft;
                var oldFyId      = isSofaCraft ? MasterDataIds.FiscalYears.FY2024_25 : Guid.NewGuid();
                var currentFyId  = isSofaCraft ? MasterDataIds.FiscalYears.FY2025_26 : Guid.NewGuid();

                // ---------- OLD CLOSED FY ----------
                var oldStart = new DateTime(oldFyStartYear, company.FiscalYearStartMonth, 1);
                var oldEnd = oldStart.AddYears(1).AddDays(-1);

                _years.Add(new FiscalYearModel
                {
                    Id = oldFyId,
                    FiscalYearCode = company.FiscalYearStartMonth == 4
                        ? $"FY{oldFyStartYear % 100:D2}-{(oldFyStartYear + 1) % 100:D2}"
                        : $"FY{oldFyStartYear}",
                    FiscalYearName = company.FiscalYearStartMonth == 4
                        ? $"Financial Year {oldFyStartYear}–{oldFyStartYear + 1}"
                        : $"Financial Year {oldFyStartYear}",
                    CompanyId = company.Id,
                    Status = FiscalYearStatus.Closed,
                    StartDate = oldStart,
                    EndDate = oldEnd,
                    BooksStartDateSnapshot = company.BooksStartDate,
                    PeriodType = FiscalPeriodType.Monthly,
                    NumberOfPeriods = 12,
                    PeriodNamingConvention = "MMM yyyy",
                    AutoGeneratePeriods = true,
                    AutoOpenFirstPeriod = true,
                    AllowAdjustmentPostingAfterSoftClose = true,
                    RequirePeriodCloseChecklist = company.FiscalYearStartMonth == 4
                });

                // ---------- CURRENT OPEN FY ----------
                var currentStart = new DateTime(currentFyStartYear, company.FiscalYearStartMonth, 1);
                var currentEnd = currentStart.AddYears(1).AddDays(-1);

                _years.Add(new FiscalYearModel
                {
                    Id = currentFyId,
                    FiscalYearCode = company.FiscalYearStartMonth == 4
                        ? $"FY{currentFyStartYear % 100:D2}-{(currentFyStartYear + 1) % 100:D2}"
                        : $"FY{currentFyStartYear}",
                    FiscalYearName = company.FiscalYearStartMonth == 4
                        ? $"Financial Year {currentFyStartYear}–{currentFyStartYear + 1}"
                        : $"Financial Year {currentFyStartYear}",
                    CompanyId = company.Id,
                    Status = FiscalYearStatus.Open,
                    StartDate = currentStart,
                    EndDate = currentEnd,
                    BooksStartDateSnapshot = company.BooksStartDate,
                    PeriodType = FiscalPeriodType.Monthly,
                    NumberOfPeriods = 12,
                    PeriodNamingConvention = "MMM yyyy",
                    AutoGeneratePeriods = true,
                    AutoOpenFirstPeriod = true,
                    AllowAdjustmentPostingAfterSoftClose = true,
                    RequirePeriodCloseChecklist = company.FiscalYearStartMonth == 4
                });
            }

            // Generate accounting periods
            GeneratePeriodsForSeededFiscalYears();
            return _years;
        }



    }

}
