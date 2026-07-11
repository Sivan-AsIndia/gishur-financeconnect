using FinanceConnect.Client.Data;
using static FinanceConnect.Client.ViewModels.BudgetLineViewModel;

namespace FinanceConnect.Client.Services
{
    public class BudgetLineService
    {
        private List<BudgetLine> _items;

        public BudgetLineService()
        {
            _items = BudgetLineSeedData.GetAll();
        }

        public List<BudgetLine> GetAll()
            => _items.Where(x => !x.IsDeleted).ToList();

        public BudgetLine? GetById(Guid id)
            => _items.FirstOrDefault(x => x.BudgetLineId == id && !x.IsDeleted);

        public Task<List<BudgetLine>> GetAllAsync()
            => Task.FromResult(GetAll());

        public Task<BudgetLine?> GetByIdAsync(Guid id)
            => Task.FromResult(GetById(id));

        public void Add(BudgetLine model)
        {
            if (_items.Any(x => x.BudgetId == model.BudgetId &&
                x.LineNumber == model.LineNumber && !x.IsDeleted))
                throw new InvalidOperationException("Line Number already exists for this Budget.");

            if (!string.IsNullOrEmpty(model.LineCode) &&
                _items.Any(x => x.BudgetId == model.BudgetId &&
                    string.Equals(x.LineCode, model.LineCode, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted))
                throw new InvalidOperationException("Line Code already exists for this Budget.");

            model.BudgetLineId = Guid.NewGuid();
            model.CreatedAt = DateTime.UtcNow;
            model.IsDeleted = false;
            _items.Add(model);
        }

        public Task CreateAsync(BudgetLine model) { Add(model); return Task.CompletedTask; }

        public void Update(BudgetLine model)
        {
            var existing = GetById(model.BudgetLineId);
            if (existing == null) return;
            if (existing.IsLocked) throw new InvalidOperationException("Locked line cannot be edited.");
            if (existing.LineStatus == LineStatusEnum.Closed) throw new InvalidOperationException("Closed line cannot be edited.");

            existing.LineCode = model.LineCode;
            existing.LineName = model.LineName;
            existing.Description = model.Description;
            existing.LineType = model.LineType;
            existing.BudgetCategoryCode = model.BudgetCategoryCode;
            existing.GLAccountId = model.GLAccountId;
            existing.GLAccountGroupId = model.GLAccountGroupId;
            existing.ExpenseNature = model.ExpenseNature;
            existing.IsCapexFlag = model.IsCapexFlag;
            existing.CostCenterId = model.CostCenterId;
            existing.CostCenterName = model.CostCenterName;
            existing.DepartmentId = model.DepartmentId;
            existing.BranchId = model.BranchId;
            existing.BranchName = model.BranchName;
            existing.ProjectId = model.ProjectId;
            existing.OwnerUserId = model.OwnerUserId;
            existing.ResponsibilityType = model.ResponsibilityType;
            existing.OriginalPlannedAmount = model.OriginalPlannedAmount;
            existing.RevisedAmount = model.RevisedAmount;
            existing.ReleasedAmount = model.ReleasedAmount;
            existing.AdjustmentAmount = model.AdjustmentAmount;
            existing.DistributionMode = model.DistributionMode;
            existing.DistributionTemplateCode = model.DistributionTemplateCode;
            existing.ActualMatchMode = model.ActualMatchMode;
            existing.IncludeAllocatedActuals = model.IncludeAllocatedActuals;
            existing.IncludeTaxInActuals = model.IncludeTaxInActuals;
            existing.ActualSourceScope = model.ActualSourceScope;
            existing.LineStatus = model.LineStatus;
            existing.RevisionReason = model.RevisionReason;
            existing.PlanningAssumptionText = model.PlanningAssumptionText;
            existing.Notes = model.Notes;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        public Task UpdateAsync(BudgetLine model) { Update(model); return Task.CompletedTask; }

        public Task DeleteAsync(Guid id)
        {
            var item = GetById(id);
            if (item != null)
            {
                if (item.IsLocked) throw new InvalidOperationException("Locked line cannot be deleted.");
                if (item.LineStatus == LineStatusEnum.Closed) throw new InvalidOperationException("Closed line cannot be deleted.");
                item.IsDeleted = true;
            }
            return Task.CompletedTask;
        }

        public Task LockAsync(Guid id)
        {
            var i = GetById(id);
            if (i != null) { i.IsLocked = true; i.LockedOn = DateTime.UtcNow; i.LineStatus = LineStatusEnum.Locked; }
            return Task.CompletedTask;
        }

        public Task UnlockAsync(Guid id)
        {
            var i = GetById(id);
            if (i != null) { i.IsLocked = false; i.LockedOn = null; i.LineStatus = LineStatusEnum.Active; }
            return Task.CompletedTask;
        }

        public Task CloseAsync(Guid id)
        {
            var i = GetById(id);
            if (i != null) { i.LineStatus = LineStatusEnum.Closed; i.ClosedOn = DateTime.UtcNow; }
            return Task.CompletedTask;
        }

        public void ResetToSeed()
        {
            _items = BudgetLineSeedData.GetAll();
        }
    }
}
