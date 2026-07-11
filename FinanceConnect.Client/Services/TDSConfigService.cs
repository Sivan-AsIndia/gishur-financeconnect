using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;
using static FinanceConnect.Client.ViewModels.TdsConfigViewModel;

namespace FinanceConnect.Client.Services
{
    public class TDSConfigService
    {
        private readonly TDSConfigSeedData _seed;
        private List<TDSConfigListDto> _store => _seed.Store;
        private Dictionary<Guid, TDSConfigFormDto> _formStore => _seed.FormStore;

        public TDSConfigService(TDSConfigSeedData seed) => _seed = seed;

        public Task<List<TDSConfigListDto>> GetAllAsync()
            => Task.FromResult(_store.OrderByDescending(x => x.EffectiveFrom).ToList());

        public Task<TDSConfigListDto?> GetByIdAsync(Guid id)
            => Task.FromResult(_store.FirstOrDefault(x => x.TDSConfigId == id));

        public Task<TDSConfigFormDto?> GetFormByIdAsync(Guid id)
        {
            _formStore.TryGetValue(id, out var form);
            return Task.FromResult<TDSConfigFormDto?>(form);
        }

        public Task SaveAsync(TDSConfigFormDto form)
        {
            ValidateOrThrow(form);

            var existing = _store.FirstOrDefault(x => x.TDSConfigId == form.TDSConfigId);

            if (existing != null)
            {
                // Update
                if (existing.IsLockedForChanges)
                    throw new InvalidOperationException("Config is locked and cannot be edited.");

                existing.ConfigCode = form.ConfigCode;
                existing.ConfigName = form.ConfigName;
                existing.SectionCode = form.SectionCode;
                existing.ConfigStatus = form.ConfigStatus;
                existing.Priority = form.Priority;
                existing.EffectiveFrom = form.EffectiveFrom;
                existing.EffectiveTo = form.EffectiveTo;
                existing.PartyApplicability = form.PartyApplicability;
                existing.APDocumentContext = form.APDocumentContext;
                existing.DeductionTriggerBasis = form.DeductionTriggerBasis;
                existing.BaseAmountMode = form.BaseAmountMode;
                existing.ThresholdEvaluationMode = form.ThresholdEvaluationMode;
                existing.ThresholdAmount = form.ThresholdAmount;
                existing.ThresholdResetBasis = form.ThresholdResetBasis;
                existing.RateSourceMode = form.RateSourceMode;
                existing.DefaultRatePercent = form.DefaultRatePercent;
                existing.RequirePANForStandardRate = form.RequirePANForStandardRate;
                existing.AlternateRateIfPANMissing = form.AlternateRateIfPANMissing;
                existing.PanValidationMode = form.PanValidationMode;
                existing.TDSPayableGLAccountDisplay = form.TDSPayableGLAccountDisplay;
                existing.SettlementCategoryTag = form.SettlementCategoryTag;
                existing.ReturnReportingTag = form.ReturnReportingTag;
                existing.IsLockedForChanges = form.IsLockedForChanges;
                existing.LockReason = form.LockReason;
                existing.UpdatedAt = DateTime.Now;

                _formStore[form.TDSConfigId] = form;
            }
            else
            {
                var newId = Guid.NewGuid();
                var dto = new TDSConfigListDto
                {
                    TDSConfigId = newId,
                    ConfigCode = form.ConfigCode,
                    ConfigName = form.ConfigName,
                    SectionCode = form.SectionCode,
                    ConfigStatus = form.ConfigStatus,
                    Priority = form.Priority,
                    EffectiveFrom = form.EffectiveFrom,
                    EffectiveTo = form.EffectiveTo,
                    PartyApplicability = form.PartyApplicability,
                    APDocumentContext = form.APDocumentContext,
                    DeductionTriggerBasis = form.DeductionTriggerBasis,
                    BaseAmountMode = form.BaseAmountMode,
                    ThresholdEvaluationMode = form.ThresholdEvaluationMode,
                    ThresholdAmount = form.ThresholdAmount,
                    ThresholdResetBasis = form.ThresholdResetBasis,
                    RateSourceMode = form.RateSourceMode,
                    DefaultRatePercent = form.DefaultRatePercent,
                    RequirePANForStandardRate = form.RequirePANForStandardRate,
                    AlternateRateIfPANMissing = form.AlternateRateIfPANMissing,
                    PanValidationMode = form.PanValidationMode,
                    TDSPayableGLAccountDisplay = form.TDSPayableGLAccountDisplay,
                    SettlementCategoryTag = form.SettlementCategoryTag,
                    ReturnReportingTag = form.ReturnReportingTag,
                    IsLockedForChanges = false,
                    CreatedAt = DateTime.Now,
                };
                _store.Add(dto);

                // Add to FormStore
                form.TDSConfigId = newId;
                form.CreatedAt = dto.CreatedAt;
                _formStore[newId] = form;
            }

            return Task.CompletedTask;
        }

        // ── Status actions ────────────────────────────────────────────────────
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
            if (_formStore.TryGetValue(id, out var form))
            {
                form.IsLockedForChanges = true;
                form.LockReason = reason;
                form.UpdatedAt = item.UpdatedAt;
            }
            return Task.CompletedTask;
        }

        public Task UnlockAsync(Guid id)
        {
            var item = GetOrThrow(id);
            item.IsLockedForChanges = false;
            item.LockReason = null;
            item.UpdatedAt = DateTime.Now;
            if (_formStore.TryGetValue(id, out var form))
            {
                form.IsLockedForChanges = false;
                form.LockReason = null;
                form.UpdatedAt = item.UpdatedAt;
            }
            return Task.CompletedTask;
        }

        // ── Internals ─────────────────────────────────────────────────────────
        private Task ChangeStatus(Guid id, ConfigStatus status)
        {
            var item = GetOrThrow(id);
            item.ConfigStatus = status;
            item.UpdatedAt = DateTime.Now;
            if (_formStore.TryGetValue(id, out var form))
            {
                form.ConfigStatus = status;
                form.UpdatedAt = item.UpdatedAt;
            }
            return Task.CompletedTask;
        }

        private TDSConfigListDto GetOrThrow(Guid id)
            => _store.FirstOrDefault(x => x.TDSConfigId == id)
               ?? throw new InvalidOperationException("TDS Config not found.");

        // ── Validation ────────────────────────────────────────────────────────
        private static void ValidateOrThrow(TDSConfigFormDto f)
        {
            if (string.IsNullOrWhiteSpace(f.ConfigCode))
                throw new InvalidOperationException("Config Code is required.");
            if (string.IsNullOrWhiteSpace(f.ConfigName))
                throw new InvalidOperationException("Config Name is required.");
            if (string.IsNullOrWhiteSpace(f.SectionCode))
                throw new InvalidOperationException("TDS Section is required.");
            if (f.Priority <= 0)
                throw new InvalidOperationException("Priority must be greater than 0.");
            if (f.EffectiveTo.HasValue && f.EffectiveTo < f.EffectiveFrom)
                throw new InvalidOperationException("Effective To must be >= Effective From.");

            if (f.RateSourceMode == TDSRateSourceMode.FixedRateOnConfig && f.DefaultRatePercent == null)
                throw new InvalidOperationException("Default Rate % is required for Fixed Rate mode.");
            if (f.DefaultRatePercent.HasValue && (f.DefaultRatePercent < 0 || f.DefaultRatePercent > 100))
                throw new InvalidOperationException("Default Rate % must be between 0 and 100.");
            if (f.RequirePANForStandardRate && f.AlternateRateIfPANMissing == null)
                throw new InvalidOperationException("Alternate Rate (PAN missing) is required when PAN is mandatory.");
            if (f.AlternateRateIfPANMissing.HasValue && (f.AlternateRateIfPANMissing < 0 || f.AlternateRateIfPANMissing > 100))
                throw new InvalidOperationException("Alternate PAN rate must be between 0 and 100.");

            if (f.ThresholdEvaluationMode != ThresholdEvaluationMode.NoThreshold && f.ThresholdAmount == null)
                throw new InvalidOperationException("Threshold Amount is required when Threshold Mode is not NoThreshold.");
            if (f.ThresholdAmount.HasValue && f.ThresholdAmount < 0)
                throw new InvalidOperationException("Threshold Amount must be >= 0.");
            if ((f.ThresholdEvaluationMode == ThresholdEvaluationMode.CumulativeByVendorInFinancialYear ||
                 f.ThresholdEvaluationMode == ThresholdEvaluationMode.CumulativeByVendorInPeriod)
                && f.ThresholdResetBasis == null)
                throw new InvalidOperationException("Threshold Reset Basis is required for cumulative threshold modes.");

            if (string.IsNullOrWhiteSpace(f.TDSPayableGLAccountDisplay))
                throw new InvalidOperationException("TDS Payable GL Account is required.");
            if (f.IsLockedForChanges && string.IsNullOrWhiteSpace(f.LockReason))
                throw new InvalidOperationException("Lock Reason is required when locking the config.");
        }
    }
}
