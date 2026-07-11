using FinanceConnect.Client.Data;
using static FinanceConnect.Client.ViewModels.TaxCodeViewModel;

namespace FinanceConnect.Client.Services
{
    public class TaxCodeService
    {
        private readonly List<TaxCodeListDto> _taxCodes = new();

        /* ================= CONSTRUCTOR ================= */

        public TaxCodeService()
        {
            _taxCodes = TaxCodeSeedData.GetAllTaxCodes();
        }

        /* ================= READ ================= */

        public List<TaxCodeListDto> GetAll()
            => _taxCodes.Where(x => !x.IsDeleted).ToList();

        public TaxCodeListDto? GetById(Guid id)
            => _taxCodes.FirstOrDefault(x => x.TaxCodeId == id && !x.IsDeleted);

        public Task<List<TaxCodeListDto>> GetAllAsync()
            => Task.FromResult(GetAll());

        public Task<TaxCodeListDto?> GetByIdAsync(Guid id)
            => Task.FromResult(GetById(id));

        public List<TaxCodeListDto> GetByType(TaxType type)
            => _taxCodes.Where(x => x.Type == type && !x.IsDeleted).ToList();

        public List<TaxCodeListDto> GetActive()
            => _taxCodes.Where(x => x.Status == TaxCodeStatus.Active && !x.IsDeleted).ToList();

        /* ================= CREATE ================= */

        public void Add(TaxCode model)
        {
            // Business rule: unique Code per Company
            var duplicate = _taxCodes.Any(x =>
                x.CompanyId == model.CompanyId &&
                string.Equals(x.Code, model.Code, StringComparison.OrdinalIgnoreCase) &&
                !x.IsDeleted);

            if (duplicate)
                throw new InvalidOperationException("Tax Code already exists for this company.");

            // Conditional required field validation
            if (model.Type == TaxType.GST && model.GSTComponent == null)
                throw new InvalidOperationException("GST Component Type is required for GST tax code.");

            if (model.Type == TaxType.TDS && string.IsNullOrWhiteSpace(model.TDSSectionCode))
                throw new InvalidOperationException("TDS Section Code is required for TDS tax code.");

            if (model.Type == TaxType.TCS && string.IsNullOrWhiteSpace(model.TCSSectionCode))
                throw new InvalidOperationException("TCS Section Code is required for TCS tax code.");

            var dto = MapToDto(model);
            dto.TaxCodeId = Guid.NewGuid();
            dto.CreatedAt = DateTime.UtcNow;
            dto.IsDeleted = false;

            _taxCodes.Add(dto);
        }

        public Task CreateAsync(TaxCode model)
        {
            Add(model);
            return Task.CompletedTask;
        }

        /* ================= UPDATE ================= */

        public void Update(TaxCode model)
        {
            var existing = GetById(model.TaxCodeId);
            if (existing == null) return;

            // Block edits if locked
            if (existing.IsLockedForChanges)
                throw new InvalidOperationException("Tax code is locked and cannot be edited.");

            // Block structural changes if used in posted transactions (guard)
            // (In a real app this would check TaxTransactionLine records)

            existing.Code = model.Code;
            existing.TaxName = model.TaxName;
            existing.Description = model.Description;
            existing.Type = model.Type;
            existing.JurisdictionCountryCode = model.JurisdictionCountryCode;
            existing.GSTComponent = model.GSTComponent;
            existing.Direction = model.Direction;
            existing.IsReverseChargeApplicable = model.IsReverseChargeApplicable;
            existing.IsITCEligibleDefault = model.IsITCEligibleDefault;
            existing.CalcType = model.CalcType;
            existing.Basis = model.Basis;
            existing.RoundingPrecisionDecimals = model.RoundingPrecisionDecimals;
            existing.Rounding = model.Rounding;
            existing.MinTaxAmount = model.MinTaxAmount;
            existing.MaxTaxAmount = model.MaxTaxAmount;
            existing.InputTaxGLAccountId = model.InputTaxGLAccountId;
            existing.InputTaxGLAccountName = model.InputTaxGLAccountName;
            existing.OutputTaxGLAccountId = model.OutputTaxGLAccountId;
            existing.OutputTaxGLAccountName = model.OutputTaxGLAccountName;
            existing.TDSGLAccountId = model.TDSGLAccountId;
            existing.TDSGLAccountName = model.TDSGLAccountName;
            existing.TCSGLAccountId = model.TCSGLAccountId;
            existing.TCSGLAccountName = model.TCSGLAccountName;
            existing.IsGLOverrideAllowedByMapping = model.IsGLOverrideAllowedByMapping;
            existing.ReturnTag = model.ReturnTag;
            existing.TDSSectionCode = model.TDSSectionCode;
            existing.TCSSectionCode = model.TCSSectionCode;
            existing.StatutoryReportingGroup = model.StatutoryReportingGroup;
            existing.EffectivePolicy = model.EffectivePolicy;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        public Task UpdateAsync(TaxCode model)
        {
            Update(model);
            return Task.CompletedTask;
        }

        /* ================= STATUS ACTIONS ================= */

        public Task ActivateAsync(Guid id)
        {
            var t = GetById(id);
            if (t == null) return Task.CompletedTask;
            if (t.Status == TaxCodeStatus.Archived)
                throw new InvalidOperationException("Archived tax codes cannot be activated.");
            t.Status = TaxCodeStatus.Active;
            t.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        public Task InactivateAsync(Guid id)
        {
            var t = GetById(id);
            if (t == null) return Task.CompletedTask;
            if (t.IsLockedForChanges)
                throw new InvalidOperationException("Tax code is locked and cannot be inactivated.");
            t.Status = TaxCodeStatus.Inactive;
            t.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        public Task ArchiveAsync(Guid id)
        {
            var t = GetById(id);
            if (t == null) return Task.CompletedTask;
            if (t.IsLockedForChanges)
                throw new InvalidOperationException("Tax code is locked. Unlock before archiving.");
            t.Status = TaxCodeStatus.Archived;
            t.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        public Task LockAsync(Guid id, string lockReason)
        {
            var t = GetById(id);
            if (t == null) return Task.CompletedTask;
            if (string.IsNullOrWhiteSpace(lockReason))
                throw new InvalidOperationException("Lock reason is required.");
            t.IsLockedForChanges = true;
            t.LockReason = lockReason;
            t.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        public Task UnlockAsync(Guid id)
        {
            var t = GetById(id);
            if (t == null) return Task.CompletedTask;
            t.IsLockedForChanges = false;
            t.LockReason = null;
            t.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        /* ================= DELETE (Soft) ================= */

        public Task DeleteAsync(Guid id)
        {
            var t = GetById(id);
            if (t == null) return Task.CompletedTask;

            if (t.IsLockedForChanges)
                throw new InvalidOperationException("Tax code is locked and cannot be deleted.");

            if (t.Status != TaxCodeStatus.Inactive && t.Status != TaxCodeStatus.Archived)
                throw new InvalidOperationException("Only Inactive or Archived tax codes can be deleted.");

            t.IsDeleted = true;
            t.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        /* ================= PRIVATE HELPERS ================= */

        private static TaxCodeListDto MapToDto(TaxCode model) => new()
        {
            Code = model.Code,
            TaxName = model.TaxName,
            Description = model.Description,
            Type = model.Type,
            JurisdictionCountryCode = model.JurisdictionCountryCode,
            GSTComponent = model.GSTComponent,
            Direction = model.Direction,
            IsReverseChargeApplicable = model.IsReverseChargeApplicable,
            IsITCEligibleDefault = model.IsITCEligibleDefault,
            CalcType = model.CalcType,
            Basis = model.Basis,
            RoundingPrecisionDecimals = model.RoundingPrecisionDecimals,
            Rounding = model.Rounding,
            MinTaxAmount = model.MinTaxAmount,
            MaxTaxAmount = model.MaxTaxAmount,
            InputTaxGLAccountId = model.InputTaxGLAccountId,
            InputTaxGLAccountName = model.InputTaxGLAccountName,
            OutputTaxGLAccountId = model.OutputTaxGLAccountId,
            OutputTaxGLAccountName = model.OutputTaxGLAccountName,
            TDSGLAccountId = model.TDSGLAccountId,
            TDSGLAccountName = model.TDSGLAccountName,
            TCSGLAccountId = model.TCSGLAccountId,
            TCSGLAccountName = model.TCSGLAccountName,
            RCMOutputTaxGLAccountId = model.RCMOutputTaxGLAccountId,
            RCMInputTaxGLAccountId = model.RCMInputTaxGLAccountId,
            IsGLOverrideAllowedByMapping = model.IsGLOverrideAllowedByMapping,
            ReturnTag = model.ReturnTag,
            TDSSectionCode = model.TDSSectionCode,
            TCSSectionCode = model.TCSSectionCode,
            StatutoryReportingGroup = model.StatutoryReportingGroup,
            Status = model.Status,
            IsLockedForChanges = model.IsLockedForChanges,
            LockReason = model.LockReason,
            EffectivePolicy = model.EffectivePolicy,
            CompanyId = model.CompanyId,
        };
    }
}
