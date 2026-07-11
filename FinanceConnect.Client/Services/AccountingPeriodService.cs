using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    public class AccountingPeriodService
    {
        private readonly List<AccountingPeriodModel> _periods = new();


        public List<AccountingPeriodModel> GetByFiscalYear(Guid fiscalYearId)
            => _periods
                .Where(x => x.FiscalYearId == fiscalYearId)
                .OrderBy(x => x.PeriodNumber)
                .ToList();

        public List<AccountingPeriodModel> GetAll()
            => _periods.OrderBy(x => x.PeriodNumber)
                .ToList();
        public AccountingPeriodModel? GetById(Guid id)
        {
            return _periods.FirstOrDefault(x => x.Id == id);
        }
        public void GenerateForFiscalYear(FiscalYearModel fy)
        {
            if (!fy.AutoGeneratePeriods)
                return;

            if (_periods.Any(x => x.FiscalYearId == fy.Id))
                return;

            GeneratePeriods(fy);
        }

        private void GeneratePeriods(FiscalYearModel fy)
        {
            var start = fy.StartDate!.Value;

            DateTime cursor = start;
            var today = DateTime.UtcNow.Date;

            for (int i = 1; i <= fy.NumberOfPeriods; i++)
            {
                bool isAdjustment =
                    fy.PeriodType == FiscalPeriodType.Monthly && i == 13;

                DateTime periodStart;
                DateTime periodEnd;

                // ⭐ MONTHLY
                if (fy.PeriodType == FiscalPeriodType.Monthly)
                {
                    periodStart = isAdjustment
                        ? fy.EndDate!.Value
                        : start.AddMonths(i - 1);

                    periodEnd = isAdjustment
                        ? fy.EndDate!.Value
                        : periodStart.AddMonths(1).AddDays(-1);
                }

                // ⭐ QUARTERLY (3-month blocks)
                else if (fy.PeriodType == FiscalPeriodType.Quarterly)
                {
                    periodStart = cursor;
                    periodEnd = cursor.AddMonths(3).AddDays(-1);
                    cursor = periodEnd.AddDays(1);
                }

                // ⭐ 4-4-5 CALENDAR (weeks pattern)
                else // FourFourFive
                {
                    int[] pattern = { 4, 4, 5 }; // weeks
                    int weeks = pattern[(i - 1) % 3];

                    periodStart = cursor;
                    periodEnd = cursor.AddDays(weeks * 7 - 1);
                    cursor = periodEnd.AddDays(1);
                }

                AccountingPeriodStatus status = AccountingPeriodStatus.Draft;

                if (fy.Status == FiscalYearStatus.Closed)
                {
                    status = AccountingPeriodStatus.Closed;
                }
                else if (fy.Status == FiscalYearStatus.SoftClosed)
                {
                    status = AccountingPeriodStatus.SoftClosed;
                }
                else if (fy.Status == FiscalYearStatus.Draft)
                {
                    status = AccountingPeriodStatus.Draft;
                }
                else if (fy.Status == FiscalYearStatus.Open)
                {
                    if (periodEnd < today)
                        status = AccountingPeriodStatus.Closed;     // Past
                    else if (periodStart <= today && today <= periodEnd)
                        status = AccountingPeriodStatus.Open;       // Current
                    else
                        status = AccountingPeriodStatus.Draft;      // Future
                }

                _periods.Add(new AccountingPeriodModel
                {
                    FiscalYearId = fy.Id,
                    CompanyId = fy.CompanyId ?? Guid.Empty,

                    PeriodNumber = i,
                    PeriodType = isAdjustment
                        ? AccountingPeriodType.Adjustment
                        : AccountingPeriodType.Normal,

                    PeriodCode = isAdjustment
                        ? $"{fy.FiscalYearCode}-ADJ"
                        : $"{fy.FiscalYearCode}-P{i}",

                    PeriodName = fy.PeriodType switch
                    {
                        FiscalPeriodType.Monthly => isAdjustment
                            ? "Adjustment Period"
                            : periodStart.ToString("MMM yyyy"),

                        FiscalPeriodType.Quarterly => $"Q{i} {periodStart:yyyy}",

                        _ => $"P{i} {periodStart:yyyy}" // 4-4-5
                    },

                    StartDate = periodStart,
                    EndDate = periodEnd,

                    Status = status,
                    RequireCloseChecklist = fy.RequirePeriodCloseChecklist,
                    AllowAdjustmentJournalsInSoftClose = true,

                    CreatedAt = DateTime.UtcNow
                });
            }

            // ── Assign stable IDs from MasterDataIds for SofaCraft periods ──
            // so GL / OB / CB seed data can reference them deterministically.
            AssignStablePeriodIds(fy);
        }

        /// <summary>
        /// For SofaCraft FYs, override the auto-generated Guid with the well-known
        /// IDs declared in MasterDataIds.AccountingPeriods.
        /// </summary>
        private void AssignStablePeriodIds(FiscalYearModel fy)
        {
            var fyPeriods = _periods.Where(p => p.FiscalYearId == fy.Id).ToList();

            // Map: (FiscalYearId, month, year) → well-known Id
            var overrides = new Dictionary<(Guid fyId, int month, int year), Guid>
    {
        { (MasterDataIds.FiscalYears.FY2025_26, 4, 2025), MasterDataIds.AccountingPeriods.Apr2025 },
        { (MasterDataIds.FiscalYears.FY2025_26, 5, 2025), MasterDataIds.AccountingPeriods.May2025 },
        { (MasterDataIds.FiscalYears.FY2024_25, 4, 2024), MasterDataIds.AccountingPeriods.Apr2024 },
    };

            foreach (var p in fyPeriods)
            {
                if (p.StartDate.HasValue)
                {
                    var key = (fy.Id, p.StartDate.Value.Month, p.StartDate.Value.Year);
                    if (overrides.TryGetValue(key, out var stableId))
                        p.Id = stableId;
                }
            }
        }




        public void CreateManual(AccountingPeriodModel period,FiscalYearModel fy)
        {
            // ================= BASIC DUPLICATE CHECKS =================

            if (_periods.Any(x => x.Id == period.Id))
                throw new Exception("Accounting period already exists.");

            if (_periods.Any(x =>
                x.FiscalYearId == period.FiscalYearId &&
                x.PeriodNumber == period.PeriodNumber))
                throw new Exception("Period number already exists for this fiscal year.");



            // ================= DATE REQUIRED =================

            if (period.StartDate == null || period.EndDate == null)
                throw new Exception("Start date and end date are required.");


            var start = period.StartDate;
            var end = period.EndDate;


            // ================= DATE ORDER =================

            if (end < start)
                throw new Exception("End date cannot be earlier than start date.");


            // ================= WITHIN FISCAL YEAR =================

            if (start < fy.StartDate || end > fy.EndDate)
                throw new Exception("Period dates must be within the fiscal year range.");


            // ================= NO OVERLAP VALIDATION =================

            var overlapping = _periods.Any(x =>
                x.FiscalYearId == period.FiscalYearId &&
                x.StartDate <= end &&
                x.EndDate >= start);

            if (overlapping)
                throw new Exception("Period date range overlaps with an existing period.");


            // ================= CONTIGUOUS PERIOD VALIDATION =================
            // Recommended ON for enterprise accounting

            var existingPeriods = _periods
                .Where(x => x.FiscalYearId == period.FiscalYearId)
                .OrderBy(x => x.StartDate)
                .ToList();

            if (existingPeriods.Count > 0)
            {
                var lastPeriod = existingPeriods.Last();

                // next period must start exactly next day after last period end
                if (start != lastPeriod.EndDate.Value.Date.AddDays(1))
                {
                    throw new Exception(
                        $"Periods must be contiguous. Next period should start on {lastPeriod.EndDate:yyyy-MM-dd + 1 day}.");
                }
            }
            else
            {
                // first period must start exactly on fiscal year start
                if (start != fy.StartDate)
                {
                    throw new Exception(
                        $"First period must start on fiscal year start date {fy.StartDate:yyyy-MM-dd}.");
                }
            }


            // ================= SUCCESS =================

            _periods.Add(period);
        }


        public void Create(AccountingPeriodModel p)
        {
            Validate(p, isEdit: false);
            _periods.Add(p);
        }

        public void Update(AccountingPeriodModel p)
        {
            var existing = _periods.FirstOrDefault(x => x.Id == p.Id);
            if (existing == null)
                throw new Exception("Period not found");

            if (existing.Status != AccountingPeriodStatus.Draft)
                throw new Exception("Only Draft periods can be edited");

            Validate(p, isEdit: true);

            _periods.Remove(existing);
            _periods.Add(p);
        }

        private void Validate(AccountingPeriodModel p, bool isEdit)
        {
            if (p.StartDate >= p.EndDate)
                throw new Exception("Period Start Date must be before End Date");

            // ----------- Unique PeriodCode within company
            if (_periods.Any(x =>
                x.CompanyId == p.CompanyId &&
                x.PeriodCode == p.PeriodCode &&
                (!isEdit || x.Id != p.Id)))
            {
                throw new Exception("Period Code already exists for this company");
            }

            // -------------------- Overlap check (within same fiscal year)
            bool overlap = _periods.Any(x =>
                x.FiscalYearId == p.FiscalYearId &&
                (!isEdit || x.Id != p.Id) &&
                p.StartDate <= x.EndDate &&
                p.EndDate >= x.StartDate);

            if (overlap)
                throw new Exception("Accounting period overlaps with another period");
        }

        public void LockPeriod(Guid periodId, string reason, string user)
        {
            var p = _periods.First(x => x.Id == periodId);

            if (string.IsNullOrWhiteSpace(reason))
                throw new Exception("Lock reason is required");

            p.Status = AccountingPeriodStatus.Closed;
            p.LockReason = reason;
            p.LockedAt = DateTime.UtcNow;
            p.LockedBy = user;
        }

        public void ReopenPeriod(Guid periodId, string reason, string user)
        {
            var p = _periods.First(x => x.Id == periodId);

            if (p.Status != AccountingPeriodStatus.Closed)
                throw new Exception("Only closed periods can be reopened");

            if (string.IsNullOrWhiteSpace(reason))
                throw new Exception("Reopen reason is required");

            p.Status = AccountingPeriodStatus.Open;
            p.LockReason = reason;
            p.LockedAt = null;
            p.LockedBy = null;

            p.IsARLocked = false;
            p.IsAPLocked = false;
            p.IsBankLocked = false;
            p.IsGLLocked = false;
        }
        public void ClosePeriod(Guid periodId)
        {
            var p = _periods.First(x => x.Id == periodId);

            p.Status = AccountingPeriodStatus.Closed;
            p.IsARLocked = true;
            p.IsAPLocked = true;
            p.IsBankLocked = true;
            p.IsGLLocked = true;
            p.ClosedAt = DateTime.UtcNow;
        }
        public void SoftClosePeriod(Guid periodId)
        {
            var p = _periods.First(x => x.Id == periodId);

            p.Status = AccountingPeriodStatus.SoftClosed;
            p.IsARLocked = true;
            p.IsAPLocked = true;
        }

        public void OpenPeriod(Guid periodId)
        {
            var p = _periods.First(x => x.Id == periodId);

            if (p.Status != AccountingPeriodStatus.Draft)
                throw new Exception("Only draft periods can be opened");

            bool hasOpenPeriod = _periods.Any(x =>
            x.FiscalYearId == p.FiscalYearId &&
            x.Status == AccountingPeriodStatus.Open &&
            x.Id != p.Id);

            if (hasOpenPeriod)
                throw new Exception("Another accounting period is already open for this fiscal year.");

            p.Status = AccountingPeriodStatus.Open;

            p.IsARLocked = false;
            p.IsAPLocked = false;
            p.IsBankLocked = false;
            p.IsGLLocked = false;
        }

    }
}
