using FinanceConnect.Client.Data;
using static FinanceConnect.Client.ViewModels.VarianceAnalysisViewModel;

namespace FinanceConnect.Client.Services
{
    public class VarianceAnalysisService
    {
        private readonly List<VarianceAnalysis> _items = new();

        public VarianceAnalysisService()
        {
            _items = VarianceAnalysisSeedData.GetAll();
        }

        public List<VarianceAnalysis> GetAll()
            => _items.Where(x => !x.IsDeleted).ToList();

        public VarianceAnalysis? GetById(Guid id)
            => _items.FirstOrDefault(x => x.VarianceAnalysisId == id && !x.IsDeleted);

        public Task<List<VarianceAnalysis>> GetAllAsync()
            => Task.FromResult(GetAll());

        public Task<VarianceAnalysis?> GetByIdAsync(Guid id)
            => Task.FromResult(GetById(id));

        public void Add(VarianceAnalysis model)
        {
            if (_items.Any(x => x.CompanyId == model.CompanyId &&
                string.Equals(x.AnalysisCode, model.AnalysisCode, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Analysis Code already exists for this Company.");

            model.VarianceAnalysisId = Guid.NewGuid();
            model.CreatedAt = DateTime.UtcNow;
            model.IsDeleted = false;
            _items.Add(model);
        }

        public Task CreateAsync(VarianceAnalysis model)
        {
            Add(model);
            return Task.CompletedTask;
        }

        public void Update(VarianceAnalysis model)
        {
            var existing = GetById(model.VarianceAnalysisId);
            if (existing == null) return;

            if (existing.IsLocked)
                throw new InvalidOperationException("Locked analysis cannot be edited.");

            existing.AnalysisCode = model.AnalysisCode;
            existing.AnalysisName = model.AnalysisName;
            existing.Description = model.Description;
            existing.AnalysisStatus = model.AnalysisStatus;
            existing.ComparisonMode = model.ComparisonMode;
            existing.AnalysisTimeMode = model.AnalysisTimeMode;
            existing.AnalysisScopeLevel = model.AnalysisScopeLevel;
            existing.LineNatureMode = model.LineNatureMode;
            existing.CurrencyId = model.CurrencyId;
            existing.FiscalYearId = model.FiscalYearId;
            existing.FromDate = model.FromDate;
            existing.ToDate = model.ToDate;
            existing.ActualSourceMode = model.ActualSourceMode;
            existing.IncludeAllocatedValues = model.IncludeAllocatedValues;
            existing.IncludeCommittedAmounts = model.IncludeCommittedAmounts;
            existing.IncludeTaxInActuals = model.IncludeTaxInActuals;
            existing.IncludeClosedPeriodsOnly = model.IncludeClosedPeriodsOnly;
            existing.MaterialityThresholdAmount = model.MaterialityThresholdAmount;
            existing.MaterialityThresholdPercent = model.MaterialityThresholdPercent;
            existing.RequireExplanationAboveThreshold = model.RequireExplanationAboveThreshold;
            existing.ReviewNotes = model.ReviewNotes;
            existing.ManagementCommentary = model.ManagementCommentary;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        public Task UpdateAsync(VarianceAnalysis model)
        {
            Update(model);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id)
        {
            var item = GetById(id);
            if (item != null)
            {
                if (item.IsLocked)
                    throw new InvalidOperationException("Locked analysis cannot be deleted.");
                item.IsDeleted = true;
            }
            return Task.CompletedTask;
        }

        public Task LockAsync(Guid id)
        {
            var item = GetById(id);
            if (item != null) { item.IsLocked = true; item.LockedOn = DateTime.UtcNow; }
            return Task.CompletedTask;
        }

        public Task UnlockAsync(Guid id)
        {
            var item = GetById(id);
            if (item != null) { item.IsLocked = false; item.LockedOn = null; }
            return Task.CompletedTask;
        }

        public Task ArchiveAsync(Guid id)
        {
            var item = GetById(id);
            if (item != null) item.AnalysisStatus = AnalysisStatusEnum.Archived;
            return Task.CompletedTask;
        }
    }
}
