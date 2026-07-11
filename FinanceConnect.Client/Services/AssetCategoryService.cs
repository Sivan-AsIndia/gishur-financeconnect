using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    public class AssetCategoryService
    {
        private readonly List<AssetsCategoryViewModel.AssetCategory> _categories = new();

        public AssetCategoryService()
        {
            _categories = AssetsCategorySeedData.GetAllAssetsCategory();
        }


        public List<AssetsCategoryViewModel.AssetCategory> GetAll()
            => _categories;

        public AssetsCategoryViewModel.AssetCategory? GetById(Guid id)
            => _categories.FirstOrDefault(x => x.AssetCategoryId == id);

        public Task<AssetsCategoryViewModel.AssetCategory?> GetByIdAsync(Guid id)
            => Task.FromResult(GetById(id));

        public string GetGLAccountName(Guid? id)
        {
            if (!id.HasValue) return "";
            return AssetsCategorySeedData.GLAccountNames.TryGetValue(id.Value, out var name)
                ? name
                : "";
        }

        public string GetDepreciationMethodName(Guid? id)
        {
            if (!id.HasValue) return "";
            return AssetsCategorySeedData.DepreciationMethodNames.TryGetValue(id.Value, out var name)
                ? name
                : "";
        }


        public void Add(AssetsCategoryViewModel.AssetCategory model)
        {
            if (_categories.Any(x => x.CompanyId == model.CompanyId &&
                                     string.Equals(x.CategoryCode, model.CategoryCode,
                                                   StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Category Code already exists for this Company.");

            if (model.IsLockedForChanges && string.IsNullOrWhiteSpace(model.LockReason))
                throw new InvalidOperationException("Lock reason is required when category is locked.");

            model.AssetCategoryId = Guid.NewGuid();
            model.CreatedAt = DateTime.UtcNow;
            model.IsDeleted = false;

            _categories.Add(model);
        }

        public Task CreateAsync(AssetsCategoryViewModel.AssetCategory model)
        {
            Add(model);
            return Task.CompletedTask;
        }


        public void Update(AssetsCategoryViewModel.AssetCategory model)
        {
            var existing = GetById(model.AssetCategoryId);
            if (existing == null) return;

            if (existing.IsLockedForChanges)
                throw new InvalidOperationException("Category is locked for changes.");

            // ── Section 1: Core Identity ─────────────────────────────────────────
            existing.CategoryCode = model.CategoryCode;
            existing.CategoryName = model.CategoryName;
            existing.Description = model.Description;
            existing.ParentAssetCategoryId = model.ParentAssetCategoryId;

            // ── Section 2: Classification ────────────────────────────────────────
            existing.AssetType = model.AssetType;
            existing.IsCapitalizable = model.IsCapitalizable;
            existing.DefaultAssetStatusOnCreation = model.DefaultAssetStatusOnCreation;

            // ── Section 3: Capitalization Policy ─────────────────────────────────
            existing.CapitalizationThresholdAmount = model.CapitalizationThresholdAmount;
            existing.ExpenseAccountIdForBelowThreshold = model.ExpenseAccountIdForBelowThreshold;
            existing.AllowManualOverrideThreshold = model.AllowManualOverrideThreshold;
            existing.RequireAcquisitionApproval = model.RequireAcquisitionApproval;

            // ── Section 4: Depreciation Defaults ─────────────────────────────────
            existing.IsDepreciable = model.IsDepreciable;
            existing.DefaultDepreciationMethodId = model.DefaultDepreciationMethodId;
            existing.UsefulLifeMonths = model.UsefulLifeMonths;
            existing.ResidualValuePercent = model.ResidualValuePercent;
            existing.DepreciationStartConvention = model.DepreciationStartConvention;
            existing.DepreciationRoundingRule = model.DepreciationRoundingRule;
            existing.AllowDepreciationOnNonWorkingDays = model.AllowDepreciationOnNonWorkingDays;

            // ── Section 5: GL Mapping ─────────────────────────────────────────────
            existing.AssetCostGLAccountId = model.AssetCostGLAccountId;
            existing.AccumulatedDepreciationGLAccountId = model.AccumulatedDepreciationGLAccountId;
            existing.DepreciationExpenseGLAccountId = model.DepreciationExpenseGLAccountId;
            existing.CapitalizationClearingGLAccountId = model.CapitalizationClearingGLAccountId;
            existing.CWIPGLAccountId = model.CWIPGLAccountId;
            existing.DisposalGainGLAccountId = model.DisposalGainGLAccountId;
            existing.DisposalLossGLAccountId = model.DisposalLossGLAccountId;
            existing.ImpairmentLossGLAccountId = model.ImpairmentLossGLAccountId;
            existing.RevaluationReserveGLAccountId = model.RevaluationReserveGLAccountId;

            // ── Section 6: Controls ───────────────────────────────────────────────
            existing.RequiresAssetTag = model.RequiresAssetTag;
            existing.RequiresSerialNumber = model.RequiresSerialNumber;
            existing.RequiresCustodian = model.RequiresCustodian;
            existing.RequiresLocation = model.RequiresLocation;
            existing.AllowSplitIntoComponents = model.AllowSplitIntoComponents;
            existing.DefaultCostCenterId = model.DefaultCostCenterId;

            // ── Section 7: Status & Lifecycle ────────────────────────────────────
            existing.CategoryStatus = model.CategoryStatus;
            existing.IsLockedForChanges = model.IsLockedForChanges;
            existing.LockReason = model.LockReason;

            // ── Audit ─────────────────────────────────────────────────────────────
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = model.UpdatedBy;
        }

        public Task UpdateAsync(AssetsCategoryViewModel.AssetCategory model)
        {
            Update(model);
            return Task.CompletedTask;
        }


        public Task ActivateAsync(Guid id)
            => ChangeStatusAsync(id, AssetsCategoryViewModel.CategoryStatus.Active);

        public Task InactivateAsync(Guid id)
            => ChangeStatusAsync(id, AssetsCategoryViewModel.CategoryStatus.Inactive);

        public Task ArchiveAsync(Guid id)
            => ChangeStatusAsync(id, AssetsCategoryViewModel.CategoryStatus.Archived);

        private Task ChangeStatusAsync(Guid id, AssetsCategoryViewModel.CategoryStatus status)
        {
            var cat = GetById(id);
            if (cat == null) return Task.CompletedTask;

            if (cat.IsLockedForChanges)
                throw new InvalidOperationException("Category is locked for changes.");

            cat.CategoryStatus = status;
            cat.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }


        public Task LockAsync(Guid id, string lockReason, Guid lockedBy)
        {
            var cat = GetById(id);
            if (cat == null) return Task.CompletedTask;

            if (string.IsNullOrWhiteSpace(lockReason))
                throw new InvalidOperationException("Lock reason is required.");

            cat.IsLockedForChanges = true;
            cat.LockReason = lockReason;
            cat.UpdatedAt = DateTime.UtcNow;
            cat.UpdatedBy = lockedBy;
            return Task.CompletedTask;
        }

        public Task UnlockAsync(Guid id, Guid unlockedBy)
        {
            var cat = GetById(id);
            if (cat == null) return Task.CompletedTask;

            cat.IsLockedForChanges = false;
            cat.LockReason = null;
            cat.UpdatedAt = DateTime.UtcNow;
            cat.UpdatedBy = unlockedBy;
            return Task.CompletedTask;
        }


        public Task DeleteAsync(Guid id)
        {
            var cat = GetById(id);
            if (cat == null) return Task.CompletedTask;

            if (cat.IsLockedForChanges)
                throw new InvalidOperationException("Category is locked for changes.");

            cat.IsDeleted = true;
            cat.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }
    }
}
