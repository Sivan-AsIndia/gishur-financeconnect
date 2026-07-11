using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;
using static FinanceConnect.Client.ViewModels.TCSConfigViewModel;

namespace FinanceConnect.Client.Services
{
    public class TCSConfigService
    {
        private readonly TCSConfigSeedData _seed;
        private List<TCSConfigListDto> _store => _seed.Store;
        private Dictionary<Guid, TCSConfigFormDto> _formStore => _seed.FormStore;

        public TCSConfigService(TCSConfigSeedData seed) => _seed = seed;

        public Task<List<TCSConfigListDto>> GetAllAsync()
            => Task.FromResult(_store.OrderByDescending(x => x.EffectiveFrom).ToList());

        public Task<TCSConfigListDto?> GetByIdAsync(Guid id)
            => Task.FromResult(_store.FirstOrDefault(x => x.TCSConfigId == id));

        public Task<TCSConfigFormDto?> GetFormByIdAsync(Guid id)
        {
            _formStore.TryGetValue(id, out var form);
            return Task.FromResult<TCSConfigFormDto?>(form);
        }

        public Task SaveAsync(TCSConfigFormDto form)
        {
            ValidateOrThrow(form);

            var existing = _store.FirstOrDefault(x => x.TCSConfigId == form.TCSConfigId);

            if (existing != null)
            {
                if (existing.IsLockedForChanges)
                    throw new InvalidOperationException("TCS Config is locked and cannot be edited.");

                existing.ConfigCode = form.ConfigCode;
                existing.ConfigName = form.ConfigName;
                existing.SectionCode = form.SectionCode;
                existing.LinkedTaxCodeDisplay = form.LinkedTaxCodeDisplay;
                existing.ConfigStatus = form.ConfigStatus;
                existing.Priority = form.Priority;
                existing.EffectiveFrom = form.EffectiveFrom;
                existing.EffectiveTo = form.EffectiveTo;
                existing.TransactionContext = form.TransactionContext;
                existing.CollectionTrigger = form.CollectionTrigger;
                existing.ResidentialStatusApplicability = form.ResidentialStatusApplicability;
                existing.ThresholdMode = form.ThresholdMode;
                existing.ThresholdAmount = form.ThresholdAmount;
                existing.ThresholdComputationBase = form.ThresholdComputationBase;
                existing.RateResolutionMode = form.RateResolutionMode;
                existing.FixedRatePercent = form.FixedRatePercent;
                existing.PanAvailabilityRule = form.PanAvailabilityRule;
                existing.AlternateRatePercentIfPanMissing = form.AlternateRatePercentIfPanMissing;
                existing.DeductionBaseMode = form.DeductionBaseMode;
                existing.TCSPayableGLAccountDisplay = form.TCSPayableGLAccountDisplay;
                existing.ReportingTag = form.ReportingTag;
                existing.IsLockedForChanges = form.IsLockedForChanges;
                existing.LockReason = form.LockReason;
                existing.UpdatedAt = DateTime.Now;

                _formStore[form.TCSConfigId] = form;
            }
            else
            {
                var newId = Guid.NewGuid();
                var dto = new TCSConfigListDto
                {
                    TCSConfigId = newId,
                    ConfigCode = form.ConfigCode,
                    ConfigName = form.ConfigName,
                    SectionCode = form.SectionCode,
                    LinkedTaxCodeDisplay = form.LinkedTaxCodeDisplay,
                    ConfigStatus = form.ConfigStatus,
                    Priority = form.Priority,
                    EffectiveFrom = form.EffectiveFrom,
                    EffectiveTo = form.EffectiveTo,
                    TransactionContext = form.TransactionContext,
                    CollectionTrigger = form.CollectionTrigger,
                    ResidentialStatusApplicability = form.ResidentialStatusApplicability,
                    ThresholdMode = form.ThresholdMode,
                    ThresholdAmount = form.ThresholdAmount,
                    ThresholdComputationBase = form.ThresholdComputationBase,
                    RateResolutionMode = form.RateResolutionMode,
                    FixedRatePercent = form.FixedRatePercent,
                    PanAvailabilityRule = form.PanAvailabilityRule,
                    AlternateRatePercentIfPanMissing = form.AlternateRatePercentIfPanMissing,
                    DeductionBaseMode = form.DeductionBaseMode,
                    TCSPayableGLAccountDisplay = form.TCSPayableGLAccountDisplay,
                    ReportingTag = form.ReportingTag,
                    IsLockedForChanges = false,
                    CreatedAt = DateTime.Now,
                };
                _store.Add(dto);
                form.TCSConfigId = newId;
                form.CreatedAt = dto.CreatedAt;
                _formStore[newId] = form;
            }

            return Task.CompletedTask;
        }

        public Task ActivateAsync(Guid id) => ChangeStatus(id, ConfigStatus.Active);
        public Task InactivateAsync(Guid id) => ChangeStatus(id, ConfigStatus.Inactive);
        public Task ArchiveAsync(Guid id) => ChangeStatus(id, ConfigStatus.Archived);

        public Task LockAsync(Guid id, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new InvalidOperationException("Lock reason is required.");
            var item = GetOrThrow(id);
            item.IsLockedForChanges = true;
            item.LockReason = reason;
            item.UpdatedAt = DateTime.Now;
            if (_formStore.TryGetValue(id, out var f))
            { f.IsLockedForChanges = true; f.LockReason = reason; f.UpdatedAt = item.UpdatedAt; }
            return Task.CompletedTask;
        }

        public Task UnlockAsync(Guid id)
        {
            var item = GetOrThrow(id);
            item.IsLockedForChanges = false;
            item.LockReason = null;
            item.UpdatedAt = DateTime.Now;
            if (_formStore.TryGetValue(id, out var f))
            { f.IsLockedForChanges = false; f.LockReason = null; f.UpdatedAt = item.UpdatedAt; }
            return Task.CompletedTask;
        }

        private Task ChangeStatus(Guid id, ConfigStatus status)
        {
            var item = GetOrThrow(id);
            item.ConfigStatus = status;
            item.UpdatedAt = DateTime.Now;
            if (_formStore.TryGetValue(id, out var f))
            { f.ConfigStatus = status; f.UpdatedAt = item.UpdatedAt; }
            return Task.CompletedTask;
        }

        private TCSConfigListDto GetOrThrow(Guid id)
            => _store.FirstOrDefault(x => x.TCSConfigId == id)
               ?? throw new InvalidOperationException("TCS Config not found.");

        private static void ValidateOrThrow(TCSConfigFormDto f)
        {
            if (string.IsNullOrWhiteSpace(f.ConfigCode))
                throw new InvalidOperationException("Config Code is required.");
            if (string.IsNullOrWhiteSpace(f.ConfigName))
                throw new InvalidOperationException("Config Name is required.");
            if (string.IsNullOrWhiteSpace(f.SectionCode))
                throw new InvalidOperationException("TCS Section is required.");
            if (string.IsNullOrWhiteSpace(f.LinkedTaxCodeDisplay))
                throw new InvalidOperationException("Linked TCS Tax Code is required.");
            if (f.Priority <= 0)
                throw new InvalidOperationException("Priority must be greater than 0.");
            if (f.EffectiveTo.HasValue && f.EffectiveTo < f.EffectiveFrom)
                throw new InvalidOperationException("Effective To must be >= Effective From.");

            if (f.RateResolutionMode == TCSRateResolutionMode.FixedRateOverride && f.FixedRatePercent == null)
                throw new InvalidOperationException("Fixed Rate % is required for Fixed Rate Override mode.");
            if (f.FixedRatePercent.HasValue && (f.FixedRatePercent < 0 || f.FixedRatePercent > 100))
                throw new InvalidOperationException("Fixed Rate % must be between 0 and 100.");
            if (f.PanAvailabilityRule == TCSPanAvailabilityRule.HigherRateIfPANMissing && f.AlternateRatePercentIfPanMissing == null)
                throw new InvalidOperationException("Alternate Rate (PAN missing) is required when PAN rule is Higher Rate.");
            if (f.AlternateRatePercentIfPanMissing.HasValue && (f.AlternateRatePercentIfPanMissing < 0 || f.AlternateRatePercentIfPanMissing > 100))
                throw new InvalidOperationException("Alternate PAN rate must be between 0 and 100.");

            
            if (f.ThresholdMode != TCSThresholdMode.NoThreshold && f.ThresholdAmount == null)
                throw new InvalidOperationException("Threshold Amount is required for the selected Threshold Mode.");
            if (f.ThresholdAmount.HasValue && f.ThresholdAmount < 0)
                throw new InvalidOperationException("Threshold Amount must be >= 0.");

            if (string.IsNullOrWhiteSpace(f.TCSPayableGLAccountDisplay))
                throw new InvalidOperationException("TCS Payable GL Account is required.");

            if (f.IsLockedForChanges && string.IsNullOrWhiteSpace(f.LockReason))
                throw new InvalidOperationException("Lock Reason is required when locking the config.");
        }
    }
}
