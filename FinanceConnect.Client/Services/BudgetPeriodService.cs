using FinanceConnect.Client.Data;
using static FinanceConnect.Client.ViewModels.BudgetPeriodViewModel;

namespace FinanceConnect.Client.Services
{
    public class BudgetPeriodService
    {
        private List<BudgetPeriod> _items;

        public BudgetPeriodService()
        {
            _items = BudgetPeriodSeedData.GetAll();
        }

        public List<BudgetPeriod> GetAll()
            => _items.Where(x => !x.IsDeleted).ToList();

        public BudgetPeriod? GetById(Guid id)
            => _items.FirstOrDefault(x => x.BudgetPeriodId == id && !x.IsDeleted);

        public Task<List<BudgetPeriod>> GetAllAsync()
            => Task.FromResult(GetAll());

        public Task<BudgetPeriod?> GetByIdAsync(Guid id)
            => Task.FromResult(GetById(id));

        public void Add(BudgetPeriod model)
        {
            if (_items.Any(x => x.BudgetId == model.BudgetId &&
                string.Equals(x.PeriodCode, model.PeriodCode, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted))
                throw new InvalidOperationException("Period Code already exists for this Budget.");

            if (_items.Any(x => x.BudgetId == model.BudgetId &&
                x.PeriodSequenceNo == model.PeriodSequenceNo && !x.IsDeleted))
                throw new InvalidOperationException("Period Sequence already exists for this Budget.");

            model.BudgetPeriodId = Guid.NewGuid();
            model.CreatedAt = DateTime.UtcNow;
            model.IsDeleted = false;
            _items.Add(model);
        }

        public Task CreateAsync(BudgetPeriod model) { Add(model); return Task.CompletedTask; }

        public void Update(BudgetPeriod model)
        {
            var existing = GetById(model.BudgetPeriodId);
            if (existing == null) return;
            if (existing.IsLocked) throw new InvalidOperationException("Locked period cannot be edited.");
            if (existing.IsClosed) throw new InvalidOperationException("Closed period cannot be edited.");

            existing.PeriodCode = model.PeriodCode;
            existing.PeriodName = model.PeriodName;
            existing.PeriodType = model.PeriodType;
            existing.FiscalMonthNo = model.FiscalMonthNo;
            existing.FiscalQuarterNo = model.FiscalQuarterNo;
            existing.FiscalHalfNo = model.FiscalHalfNo;
            existing.FiscalYearId = model.FiscalYearId;
            existing.StartDate = model.StartDate;
            existing.EndDate = model.EndDate;
            existing.PlannedBudgetAmount = model.PlannedBudgetAmount;
            existing.RevisedBudgetAmount = model.RevisedBudgetAmount;
            existing.ReleasedBudgetAmount = model.ReleasedBudgetAmount;
            existing.PeriodStatus = model.PeriodStatus;
            existing.OpenForConsumptionFlag = model.OpenForConsumptionFlag;
            existing.RevisionReason = model.RevisionReason;
            existing.PeriodNotes = model.PeriodNotes;
            existing.PlanningAssumptionSummary = model.PlanningAssumptionSummary;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        public Task UpdateAsync(BudgetPeriod model) { Update(model); return Task.CompletedTask; }

        public Task DeleteAsync(Guid id)
        {
            var item = GetById(id);
            if (item != null)
            {
                if (item.IsLocked) throw new InvalidOperationException("Locked period cannot be deleted.");
                if (item.IsClosed) throw new InvalidOperationException("Closed period cannot be deleted.");
                item.IsDeleted = true;
            }
            return Task.CompletedTask;
        }

        public Task LockAsync(Guid id)
        {
            var i = GetById(id);
            if (i != null) { i.IsLocked = true; i.LockedOn = DateTime.UtcNow; i.PeriodStatus = PeriodStatusEnum.Locked; }
            return Task.CompletedTask;
        }

        public Task UnlockAsync(Guid id)
        {
            var i = GetById(id);
            if (i != null) { i.IsLocked = false; i.LockedOn = null; i.PeriodStatus = PeriodStatusEnum.Open; }
            return Task.CompletedTask;
        }

        public Task CloseAsync(Guid id)
        {
            var i = GetById(id);
            if (i != null) { i.IsClosed = true; i.ClosedOn = DateTime.UtcNow; i.PeriodStatus = PeriodStatusEnum.Closed; }
            return Task.CompletedTask;
        }

        public void ResetToSeed()
        {
            _items = BudgetPeriodSeedData.GetAll();
        }
    }
}
