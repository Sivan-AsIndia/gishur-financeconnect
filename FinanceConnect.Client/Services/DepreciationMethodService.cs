using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    public class DepreciationMethodService
    {
        private readonly List<DepreciationMethodViewModel.DepreciationMethod> _methods;

        public DepreciationMethodService()
        {
            _methods = DepreciationMethodSeedData.GetAll();
        }

        /* ================= READ ================= */

        public List<DepreciationMethodViewModel.DepreciationMethod> GetAll()
            => _methods;

        public DepreciationMethodViewModel.DepreciationMethod? GetById(Guid id)
            => _methods.FirstOrDefault(x => x.DepreciationMethodId == id);

        public Task<DepreciationMethodViewModel.DepreciationMethod?> GetByIdAsync(Guid id)
            => Task.FromResult(GetById(id));

        /* ================= CREATE ================= */

        public void Add(DepreciationMethodViewModel.DepreciationMethod model)
        {
            // Company need to show or not in UI
            if (_methods.Any(x => x.CompanyId == model.CompanyId &&
                                  string.Equals(x.MethodCode, model.MethodCode,
                                                StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Method Code already exists for this Company.");

            if (string.IsNullOrWhiteSpace(model.MethodCode))
                throw new InvalidOperationException("Method Code is required.");

            if (string.IsNullOrWhiteSpace(model.MethodName))
                throw new InvalidOperationException("Method Name is required.");

            if (model.InputMode == DepreciationMethodViewModel.InputModeEnum.RateBased &&
                (model.DefaultRatePercent == null || model.DefaultRatePercent <= 0))
                throw new InvalidOperationException("Rate is required for RateBased depreciation.");

            if (model.StartConvention == DepreciationMethodViewModel.StartConventionEnum.FullMonthIfBeforeDayN &&
                (model.FullMonthCutoffDay == null || model.FullMonthCutoffDay < 1 || model.FullMonthCutoffDay > 28))
                throw new InvalidOperationException("Cutoff day must be between 1 and 28.");

            model.DepreciationMethodId = Guid.NewGuid();
            model.CreatedAt = DateTime.UtcNow;
            model.IsDeleted = false;

            _methods.Add(model);
        }

        public Task CreateAsync(DepreciationMethodViewModel.DepreciationMethod model)
        {
            Add(model);
            return Task.CompletedTask;
        }

        /* ================= UPDATE ================= */

        public void Update(DepreciationMethodViewModel.DepreciationMethod model)
        {
            var existing = GetById(model.DepreciationMethodId);
            if (existing == null) return;

            if (existing.IsLockedForChanges && existing.UsedInPostedRunsFlag)
                throw new InvalidOperationException("Method is locked and cannot be edited.");

            existing.MethodCode = model.MethodCode;
            existing.MethodName = model.MethodName;
            existing.Description = model.Description;
            existing.MethodType = model.MethodType;
            existing.TimeBasis = model.TimeBasis;
            existing.IsApplicableToTangibles = model.IsApplicableToTangibles;
            existing.IsApplicableToIntangibles = model.IsApplicableToIntangibles;
            existing.IsDepreciationAllowedInCWIP = model.IsDepreciationAllowedInCWIP;
            existing.DepreciationBase = model.DepreciationBase;
            existing.ResidualHandlingMode = model.ResidualHandlingMode;
            existing.AllowResidualOverrideAtAsset = model.AllowResidualOverrideAtAsset;
            existing.InputMode = model.InputMode;
            existing.DefaultRatePercent = model.DefaultRatePercent;
            existing.AllowRateOverrideAtCategory = model.AllowRateOverrideAtCategory;
            existing.AllowRateOverrideAtAsset = model.AllowRateOverrideAtAsset;
            existing.StartConvention = model.StartConvention;
            existing.FullMonthCutoffDay = model.FullMonthCutoffDay;
            existing.EndConvention = model.EndConvention;
            existing.SkipDepreciationIfDisposedInPeriod = model.SkipDepreciationIfDisposedInPeriod;
            existing.AllowCatchUpDepreciation = model.AllowCatchUpDepreciation;
            existing.RoundingPrecisionDecimals = model.RoundingPrecisionDecimals;
            existing.RoundingRule = model.RoundingRule;
            existing.RoundingAt = model.RoundingAt;
            existing.MinDepreciationAmount = model.MinDepreciationAmount;
            existing.MethodStatus = model.MethodStatus;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        public Task UpdateAsync(DepreciationMethodViewModel.DepreciationMethod model)
        {
            Update(model);
            return Task.CompletedTask;
        }

        /* ================= STATUS ACTIONS ================= */

        public Task ActivateAsync(Guid id)
            => ChangeStatusAsync(id, DepreciationMethodViewModel.MethodStatusEnum.Active);

        public Task InactivateAsync(Guid id)
            => ChangeStatusAsync(id, DepreciationMethodViewModel.MethodStatusEnum.Inactive);

        public Task ArchiveAsync(Guid id)
            => ChangeStatusAsync(id, DepreciationMethodViewModel.MethodStatusEnum.Archived);

        private Task ChangeStatusAsync(Guid id, DepreciationMethodViewModel.MethodStatusEnum status)
        {
            var method = GetById(id);
            if (method == null) return Task.CompletedTask;

            if (method.IsLockedForChanges)
                throw new InvalidOperationException("Method is locked for changes.");

            method.MethodStatus = status;
            method.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        /* ================= LOCK / UNLOCK ================= */

        public Task LockAsync(Guid id, string lockReason)
        {
            var method = GetById(id);
            if (method == null) return Task.CompletedTask;

            if (string.IsNullOrWhiteSpace(lockReason))
                throw new InvalidOperationException("Lock reason is required.");

            method.IsLockedForChanges = true;
            method.LockReason = lockReason;
            method.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        public Task UnlockAsync(Guid id)
        {
            var method = GetById(id);
            if (method == null) return Task.CompletedTask;

            method.IsLockedForChanges = false;
            method.LockReason = null;
            method.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        /* ================= DELETE (Soft) ================= */

        public Task DeleteAsync(Guid id)
        {
            var method = GetById(id);
            if (method == null) return Task.CompletedTask;

            if (method.IsLockedForChanges)
                throw new InvalidOperationException("Method is locked for changes.");

            if (method.UsedAssetCount > 0)
                throw new InvalidOperationException("Cannot delete method used by assets.");

            method.IsDeleted = true;
            method.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }
    }
}
