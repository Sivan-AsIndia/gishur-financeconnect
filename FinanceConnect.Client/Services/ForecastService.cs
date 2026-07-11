using FinanceConnect.Client.Data;
using static FinanceConnect.Client.ViewModels.ForecastViewModel;

namespace FinanceConnect.Client.Services
{
    public class ForecastService
    {
        private readonly List<Forecast> _items = new();

        public ForecastService()
        {
            _items = ForecastSeedData.GetAll();
        }

        public List<Forecast> GetAll()
            => _items.Where(x => !x.IsDeleted).ToList();

        public Forecast? GetById(Guid id)
            => _items.FirstOrDefault(x => x.ForecastId == id && !x.IsDeleted);

        public Task<List<Forecast>> GetAllAsync()
            => Task.FromResult(GetAll());

        public Task<Forecast?> GetByIdAsync(Guid id)
            => Task.FromResult(GetById(id));

        public void Add(Forecast model)
        {
            if (_items.Any(x => x.CompanyId == model.CompanyId &&
                string.Equals(x.ForecastCode, model.ForecastCode, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Forecast Code already exists for this Company.");

            model.ForecastId = Guid.NewGuid();
            model.CreatedAt = DateTime.UtcNow;
            model.IsDeleted = false;
            _items.Add(model);
        }

        public Task CreateAsync(Forecast model) { Add(model); return Task.CompletedTask; }

        public void Update(Forecast model)
        {
            var existing = GetById(model.ForecastId);
            if (existing == null) return;
            if (existing.IsLocked) throw new InvalidOperationException("Locked forecast cannot be edited.");

            existing.ForecastCode = model.ForecastCode;
            existing.ForecastName = model.ForecastName;
            existing.Description = model.Description;
            existing.ForecastStatus = model.ForecastStatus;
            existing.ForecastType = model.ForecastType;
            existing.ScenarioType = model.ScenarioType;
            existing.ForecastLevel = model.ForecastLevel;
            existing.ForecastNature = model.ForecastNature;
            existing.CurrencyId = model.CurrencyId;
            existing.FiscalYearId = model.FiscalYearId;
            existing.FromDate = model.FromDate;
            existing.ToDate = model.ToDate;
            existing.ForecastTimeMode = model.ForecastTimeMode;
            existing.BaselineReferenceType = model.BaselineReferenceType;
            existing.ForecastMethod = model.ForecastMethod;
            existing.ActualAsOfDate = model.ActualAsOfDate;
            existing.UseAllocatedValues = model.UseAllocatedValues;
            existing.UseCommittedAmounts = model.UseCommittedAmounts;
            existing.IncludeTaxInForecast = model.IncludeTaxInForecast;
            existing.ConfidenceLevel = model.ConfidenceLevel;
            existing.ForecastAssumptionSummary = model.ForecastAssumptionSummary;
            existing.KeyRiskSummary = model.KeyRiskSummary;
            existing.KeyOpportunitySummary = model.KeyOpportunitySummary;
            existing.ManagementAdjustmentFlag = model.ManagementAdjustmentFlag;
            existing.ManagementAdjustmentReason = model.ManagementAdjustmentReason;
            existing.Notes = model.Notes;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        public Task UpdateAsync(Forecast model) { Update(model); return Task.CompletedTask; }

        public Task DeleteAsync(Guid id)
        {
            var item = GetById(id);
            if (item != null) {
                if (item.IsLocked) throw new InvalidOperationException("Locked forecast cannot be deleted.");
                item.IsDeleted = true;
            }
            return Task.CompletedTask;
        }

        public Task LockAsync(Guid id) { var i = GetById(id); if (i != null) { i.IsLocked = true; i.LockedOn = DateTime.UtcNow; } return Task.CompletedTask; }
        public Task UnlockAsync(Guid id) { var i = GetById(id); if (i != null) { i.IsLocked = false; i.LockedOn = null; } return Task.CompletedTask; }
        public Task ArchiveAsync(Guid id) { var i = GetById(id); if (i != null) i.ForecastStatus = ForecastStatusEnum.Archived; return Task.CompletedTask; }
    }
}
