using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    public class CostCenterService
    {
        private readonly List<CostCenterModel> _centers;

        public CostCenterService()
        {
            _centers = CostCenterServiceData.Get();
        }

        // ─── READ ────────────────────────────────────────────────────
        public List<CostCenterModel> GetAll() => _centers;

        public CostCenterModel? GetById(Guid id)
            => _centers.FirstOrDefault(x => x.Id == id);

        public CostCenterModel? GetByCode(string code)
            => _centers.FirstOrDefault(x =>
                x.CostCenterCode.Equals(code, StringComparison.OrdinalIgnoreCase));

        public Task<CostCenterModel?> GetByCodeAsync(string code)
            => Task.FromResult(GetByCode(code));

        public List<CostCenterModel> GetChildren(Guid parentId)
            => _centers.Where(x => x.ParentCostCenterId == parentId).ToList();

        public List<CostCenterModel> GetActive()
            => _centers.Where(x => x.CostCenterStatus == "Active").ToList();

        // ─── CREATE ──────────────────────────────────────────────────
        public void Add(CostCenterModel model)
        {
            // Duplicate code check per company
            if (_centers.Any(c =>
                c.CompanyId == model.CompanyId &&
                c.CostCenterCode.Equals(model.CostCenterCode, StringComparison.OrdinalIgnoreCase) &&
                !c.IsDeleted))
            {
                throw new InvalidOperationException(
                    $"Cost center code '{model.CostCenterCode}' already exists.");
            }

            // Hierarchy: set level and path
            if (model.ParentCostCenterId.HasValue)
            {
                var parent = GetById(model.ParentCostCenterId.Value);
                if (parent != null)
                {
                    model.HierarchyLevel = parent.HierarchyLevel + 1;
                    model.HierarchyPath = $"{parent.HierarchyPath}/{model.CostCenterCode}";
                }
            }
            else
            {
                model.HierarchyLevel = 1;
                model.HierarchyPath = model.CostCenterCode;
            }

            model.Id = Guid.NewGuid();
            model.CreatedAt = DateTime.Now;
            _centers.Add(model);
        }

        public Task CreateAsync(CostCenterModel model)
        {
            Add(model);
            return Task.CompletedTask;
        }

        // ─── UPDATE ──────────────────────────────────────────────────
        public void Update(CostCenterModel model)
        {
            var existing = GetById(model.Id);
            if (existing == null) return;

            if (existing.IsLocked)
                throw new InvalidOperationException("Locked cost center cannot be modified.");

            if (existing.CostCenterStatus == "Closed" || existing.CostCenterStatus == "Archived")
                throw new InvalidOperationException("Closed or archived cost center cannot be edited.");

            existing.CostCenterName = model.CostCenterName;
            existing.ShortName = model.ShortName;
            existing.Description = model.Description;
            existing.CostCenterType = model.CostCenterType;
            existing.ControlNature = model.ControlNature;
            existing.UsageMode = model.UsageMode;
            existing.IsSharedServiceCenter = model.IsSharedServiceCenter;
            existing.IsAllocationSourceAllowed = model.IsAllocationSourceAllowed;
            existing.IsAllocationTargetAllowed = model.IsAllocationTargetAllowed;
            existing.ParentCostCenterId = model.ParentCostCenterId;
            existing.ParentCostCenterName = model.ParentCostCenterName;
            existing.DepartmentId = model.DepartmentId;
            existing.BranchId = model.BranchId;
            existing.BranchName = model.BranchName;
            existing.RegionCode = model.RegionCode;
            existing.BusinessUnitCode = model.BusinessUnitCode;
            existing.CostCenterOwnerUserId = model.CostCenterOwnerUserId;
            existing.CostCenterOwnerName = model.CostCenterOwnerName;
            existing.ResponsibleManagerUserId = model.ResponsibleManagerUserId;
            existing.ResponsibleManagerName = model.ResponsibleManagerName;
            existing.FinanceReviewerUserId = model.FinanceReviewerUserId;
            existing.FinanceReviewerName = model.FinanceReviewerName;
            existing.ApprovalRoleCode = model.ApprovalRoleCode;
            existing.EmailDistributionGroup = model.EmailDistributionGroup;
            existing.DefaultCurrencyId = model.DefaultCurrencyId;
            existing.DefaultCurrencyCode = model.DefaultCurrencyCode;
            existing.BudgetControlMode = model.BudgetControlMode;
            existing.TolerancePercent = model.TolerancePercent;
            existing.ToleranceAmount = model.ToleranceAmount;
            existing.AllowNegativeBalance = model.AllowNegativeBalance;
            existing.IsCapexAllowed = model.IsCapexAllowed;
            existing.IsOpexAllowed = model.IsOpexAllowed;
            existing.DefaultBudgetCategoryCode = model.DefaultBudgetCategoryCode;
            existing.DefaultGLAccountId = model.DefaultGLAccountId;
            existing.DefaultGLAccountCode = model.DefaultGLAccountCode;
            existing.ReportingGroupCode = model.ReportingGroupCode;
            existing.AllocationBaseType = model.AllocationBaseType;
            existing.DefaultAllocationDriverValue = model.DefaultAllocationDriverValue;
            existing.CanReceiveSharedCost = model.CanReceiveSharedCost;
            existing.CanDistributeSharedCost = model.CanDistributeSharedCost;
            existing.StatisticalKeyReference = model.StatisticalKeyReference;
            existing.EffectiveFrom = model.EffectiveFrom;
            existing.EffectiveTo = model.EffectiveTo;
            existing.CostCenterStatus = model.CostCenterStatus;
            existing.IsActive = model.CostCenterStatus == "Active";
            existing.ClosureReason = model.ClosureReason;
            existing.ReplacedByCostCenterId = model.ReplacedByCostCenterId;
            existing.ReplacedByCostCenterName = model.ReplacedByCostCenterName;
            existing.Notes = model.Notes;
            existing.OperationalRemarks = model.OperationalRemarks;
            existing.UpdatedAt = DateTime.Now;
        }

        public Task UpdateAsync(CostCenterModel model)
        {
            Update(model);
            return Task.CompletedTask;
        }

        // ─── DELETE ──────────────────────────────────────────────────
        public void Delete(Guid id, string reason)
        {
            var cc = GetById(id);
            if (cc == null) return;

            // Cannot delete if it has children
            if (_centers.Any(x => x.ParentCostCenterId == id))
                throw new InvalidOperationException(
                    "Cannot delete a cost center that has child cost centers.");

            cc.IsDeleted = true;
            cc.OperationalRemarks += $" | Deleted: {reason}";
            _centers.Remove(cc);
        }

        // ─── LIFECYCLE ACTIONS ───────────────────────────────────────
        public void Lock(Guid id, string reason, string lockedByName)
        {
            var cc = GetById(id);
            if (cc == null) return;

            cc.IsLocked = true;
            cc.CostCenterStatus = "Locked";
            cc.LockedOn = DateTime.Now;
            cc.LockedByName = lockedByName;
            cc.Notes += $" | Locked: {reason}";
        }

        public void Unlock(Guid id)
        {
            var cc = GetById(id);
            if (cc == null) return;

            cc.IsLocked = false;
            cc.CostCenterStatus = "Active";
            cc.IsActive = true;
            cc.LockedOn = null;
            cc.LockedByName = null;
            cc.Notes += " | Unlocked";
        }

        public void Activate(Guid id)
        {
            var cc = GetById(id);
            if (cc == null) return;
            cc.CostCenterStatus = "Active";
            cc.IsActive = true;
        }

        public void Inactivate(Guid id)
        {
            var cc = GetById(id);
            if (cc == null) return;
            cc.CostCenterStatus = "Inactive";
            cc.IsActive = false;
        }

        // ─── VALIDATION HELPERS ──────────────────────────────────────

        /// <summary>
        /// Detects circular hierarchy — returns true if assigning newParentId
        /// as parent of childId would create a cycle.
        /// </summary>
        public bool WouldCreateCircularHierarchy(Guid childId, Guid newParentId)
        {
            if (childId == newParentId) return true;

            var visited = new HashSet<Guid>();
            var current = newParentId;

            while (true)
            {
                if (visited.Contains(current)) return true;
                visited.Add(current);

                var node = GetById(current);
                if (node?.ParentCostCenterId == null) return false;
                if (node.ParentCostCenterId == childId) return true;

                current = node.ParentCostCenterId.Value;
            }
        }

        public bool IsCodeDuplicate(string code, Guid companyId, Guid? excludeId = null)
        {
            return _centers.Any(c =>
                c.CompanyId == companyId &&
                c.CostCenterCode.Equals(code, StringComparison.OrdinalIgnoreCase) &&
                !c.IsDeleted &&
                (excludeId == null || c.Id != excludeId));
        }

        // ─── POSTING VALIDATION ──────────────────────────────────────
        public void ValidateForPosting(Guid id)
        {
            var cc = GetById(id);
            if (cc == null)
                throw new InvalidOperationException("Invalid cost center");

            if (cc.CostCenterStatus != "Active")
                throw new InvalidOperationException("Cost center is not active");

            if (cc.IsLocked)
                throw new InvalidOperationException("Cost center is locked");
        }
    }
}
