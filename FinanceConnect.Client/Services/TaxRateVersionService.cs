using FinanceConnect.Client.Data;
using static FinanceConnect.Client.ViewModels.TaxRateVersionViewModel;

namespace FinanceConnect.Client.Services
{
    public class TaxRateVersionService
    {
        private readonly List<TaxRateVersionListDto> _versions = new();

        /* ================= CONSTRUCTOR ================= */

        public TaxRateVersionService()
        {
            _versions = TaxRateVersionSeedData.GetAllRateVersions();
        }

        /* ================= READ ================= */

        public List<TaxRateVersionListDto> GetAll()
            => _versions.Where(x => !x.IsDeleted).ToList();

        public List<TaxRateVersionListDto> GetByTaxCode(Guid taxCodeId)
            => _versions
                .Where(x => x.TaxCodeId == taxCodeId && !x.IsDeleted)
                .OrderByDescending(x => x.VersionNumber)
                .ToList();

        public TaxRateVersionListDto? GetById(Guid id)
            => _versions.FirstOrDefault(x => x.TaxRateVersionId == id && !x.IsDeleted);

        public Task<List<TaxRateVersionListDto>> GetAllAsync()
            => Task.FromResult(GetAll());

        public Task<List<TaxRateVersionListDto>> GetByTaxCodeAsync(Guid taxCodeId)
            => Task.FromResult(GetByTaxCode(taxCodeId));

        public Task<TaxRateVersionListDto?> GetByIdAsync(Guid id)
            => Task.FromResult(GetById(id));

        public TaxRateVersionListDto? ResolveRate(Guid taxCodeId, DateTime transactionDate)
            => _versions
                .Where(x =>
                    x.TaxCodeId == taxCodeId &&
                    !x.IsDeleted &&
                    (x.Status == VersionStatus.Active || x.Status == VersionStatus.Approved) &&
                    x.EffectiveFrom <= transactionDate &&
                    (x.EffectiveTo == null || x.EffectiveTo >= transactionDate))
                .OrderByDescending(x => x.EffectiveFrom)
                .FirstOrDefault();

        public int GetNextVersionNumber(Guid taxCodeId)
        {
            var existing = _versions.Where(x => x.TaxCodeId == taxCodeId && !x.IsDeleted).ToList();
            return existing.Any() ? existing.Max(x => x.VersionNumber) + 1 : 1;
        }

        /* ================= CREATE ================= */

        public void Add(TaxRateVersion model)
        {
            if (model.Type == RateType.Percentage)
            {
                if (!model.RatePercent.HasValue)
                    throw new InvalidOperationException("Rate Percent is required for Percentage rate type.");
                if (model.RatePercent < 0 || model.RatePercent > 100)
                    throw new InvalidOperationException("Rate Percent must be between 0 and 100.");
            }
            if (model.Type == RateType.FixedAmount && !model.FixedAmount.HasValue)
                throw new InvalidOperationException("Fixed Amount is required for Fixed Amount rate type.");

            if (model.EffectiveTo.HasValue && model.EffectiveTo.Value < model.EffectiveFrom)
                throw new InvalidOperationException("Effective To must be on or after Effective From.");

            CheckOverlap(model.TaxCodeId, model.EffectiveFrom, model.EffectiveTo, null);

            if (!model.EffectiveTo.HasValue)
            {
                var existingOpenEnded = _versions.Any(x =>
                    x.TaxCodeId == model.TaxCodeId &&
                    !x.IsDeleted &&
                    x.EffectiveTo == null &&
                    x.Status != VersionStatus.Retired &&
                    x.Status != VersionStatus.Superseded &&
                    x.Status != VersionStatus.Cancelled);

                if (existingOpenEnded)
                    throw new InvalidOperationException(
                        "An open-ended version already exists for this Tax Code. Close it before adding a new open-ended version.");
            }

            var dto = MapToDto(model);
            dto.TaxRateVersionId = Guid.NewGuid();
            dto.VersionNumber = GetNextVersionNumber(model.TaxCodeId);
            dto.CreatedAt = DateTime.UtcNow;
            dto.IsDeleted = false;

            _versions.Add(dto);
        }

        public Task CreateAsync(TaxRateVersion model)
        {
            Add(model);
            return Task.CompletedTask;
        }

        /* ================= UPDATE ================= */

        public void Update(TaxRateVersion model)
        {
            var existing = GetById(model.TaxRateVersionId);
            if (existing == null) return;

            if (existing.IsLockedForChanges)
                throw new InvalidOperationException("This rate version is locked and cannot be modified.");

            if (existing.Status == VersionStatus.Active || existing.Status == VersionStatus.Approved)
                throw new InvalidOperationException(
                    "Active or Approved versions cannot be edited. Create a new version to supersede.");

            CheckOverlap(model.TaxCodeId, model.EffectiveFrom, model.EffectiveTo, model.TaxRateVersionId);

            existing.EffectiveFrom = model.EffectiveFrom;
            existing.EffectiveTo = model.EffectiveTo;
            existing.Type = model.Type;
            existing.RatePercent = model.RatePercent;
            existing.FixedAmount = model.FixedAmount;
            existing.Basis = model.Basis;
            existing.MinimumTaxAmount = model.MinimumTaxAmount;
            existing.MaximumTaxAmount = model.MaximumTaxAmount;
            existing.HasThreshold = model.HasThreshold;
            existing.ThresholdAmount = model.ThresholdAmount;
            existing.PanRequiredForStandardRate = model.PanRequiredForStandardRate;
            existing.AlternateRatePercentIfPanMissing = model.AlternateRatePercentIfPanMissing;
            existing.IsReverseChargeRate = model.IsReverseChargeRate;
            existing.ITCOverride = model.ITCOverride;
            existing.SourceType = model.SourceType;
            existing.LegalReferenceNumber = model.LegalReferenceNumber;
            existing.LegalReferenceDate = model.LegalReferenceDate;
            existing.Notes = model.Notes;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        public Task UpdateAsync(TaxRateVersion model)
        {
            Update(model);
            return Task.CompletedTask;
        }

        /* ================= STATUS ACTIONS ================= */

        public Task SubmitAsync(Guid id)
            => ChangeStatusAsync(id, VersionStatus.Submitted,
               allowed: new[] { VersionStatus.Draft });

        public Task ApproveAsync(Guid id)
            => ChangeStatusAsync(id, VersionStatus.Approved,
               allowed: new[] { VersionStatus.Submitted });

        public Task RejectAsync(Guid id, string reason)
        {
            var v = GetById(id);
            if (v == null) return Task.CompletedTask;
            if (v.Status != VersionStatus.Submitted)
                throw new InvalidOperationException("Only Submitted versions can be rejected.");
            v.Status = VersionStatus.Draft;
            v.Notes = $"[Rejected: {reason}] {v.Notes}".Trim();
            v.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        public Task ActivateAsync(Guid id)
        {
            var v = GetById(id);
            if (v == null) return Task.CompletedTask;

            if (v.Status != VersionStatus.Approved)
                throw new InvalidOperationException("Only Approved versions can be activated.");

            CheckOverlap(v.TaxCodeId, v.EffectiveFrom, v.EffectiveTo, v.TaxRateVersionId);

            v.Status = VersionStatus.Active;
            v.ActivatedOn = DateTime.UtcNow;
            v.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        public Task RetireAsync(Guid id, DateTime? retireEffectiveTo = null)
        {
            var v = GetById(id);
            if (v == null) return Task.CompletedTask;

            if (v.Status != VersionStatus.Active)
                throw new InvalidOperationException("Only Active versions can be retired.");

            if (retireEffectiveTo.HasValue)
                v.EffectiveTo = retireEffectiveTo.Value;

            v.Status = VersionStatus.Retired;
            v.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        public Task CancelAsync(Guid id)
            => ChangeStatusAsync(id, VersionStatus.Cancelled,
               allowed: new[] { VersionStatus.Draft, VersionStatus.Submitted });

        public Task LockAsync(Guid id, string lockReason)
        {
            var v = GetById(id);
            if (v == null) return Task.CompletedTask;
            if (string.IsNullOrWhiteSpace(lockReason))
                throw new InvalidOperationException("Lock reason is required.");
            v.IsLockedForChanges = true;
            v.LockReason = lockReason;
            v.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        public Task UnlockAsync(Guid id)
        {
            var v = GetById(id);
            if (v == null) return Task.CompletedTask;
            v.IsLockedForChanges = false;
            v.LockReason = null;
            v.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        /* ================= DELETE (Soft) ================= */

        public Task DeleteAsync(Guid id)
        {
            var v = GetById(id);
            if (v == null) return Task.CompletedTask;

            if (v.IsLockedForChanges)
                throw new InvalidOperationException("This version is locked and cannot be deleted.");

            if (v.Status != VersionStatus.Draft && v.Status != VersionStatus.Cancelled)
                throw new InvalidOperationException("Only Draft or Cancelled versions can be deleted.");

            v.IsDeleted = true;
            v.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        /* ================= PRIVATE HELPERS ================= */

        private void CheckOverlap(Guid taxCodeId, DateTime from, DateTime? to, Guid? excludeId)
        {
            var existing = _versions
                .Where(x =>
                    x.TaxCodeId == taxCodeId &&
                    !x.IsDeleted &&
                    (excludeId == null || x.TaxRateVersionId != excludeId) &&
                    x.Status != VersionStatus.Retired &&
                    x.Status != VersionStatus.Superseded &&
                    x.Status != VersionStatus.Cancelled)
                .ToList();

            foreach (var v in existing)
            {
                var vFrom = v.EffectiveFrom;
                var vTo = v.EffectiveTo;

                bool overlaps =
                    (to == null || to >= vFrom) &&
                    (vTo == null || vTo >= from);

                if (overlaps)
                    throw new InvalidOperationException(
                        $"Effective date overlaps with an existing version (V{v.VersionNumber}: {vFrom:dd MMM yyyy} – {(vTo.HasValue ? vTo.Value.ToString("dd MMM yyyy") : "Open")}).");
            }
        }

        private Task ChangeStatusAsync(Guid id, VersionStatus newStatus, VersionStatus[] allowed)
        {
            var v = GetById(id);
            if (v == null) return Task.CompletedTask;

            if (!allowed.Contains(v.Status))
                throw new InvalidOperationException(
                    $"Cannot change status to {newStatus} from {v.Status}.");

            v.Status = newStatus;
            v.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        private static TaxRateVersionListDto MapToDto(TaxRateVersion model) => new()
        {
            TaxCodeId = model.TaxCodeId,
            TaxCodeSnapshot = model.TaxCodeSnapshot,
            TaxCodeNameSnapshot = model.TaxCodeNameSnapshot,
            TaxTypeSnapshot = model.TaxTypeSnapshot,
            Status = model.Status,
            EffectiveFrom = model.EffectiveFrom,
            EffectiveTo = model.EffectiveTo,
            Type = model.Type,
            RatePercent = model.RatePercent,
            FixedAmount = model.FixedAmount,
            Basis = model.Basis,
            MinimumTaxAmount = model.MinimumTaxAmount,
            MaximumTaxAmount = model.MaximumTaxAmount,
            HasThreshold = model.HasThreshold,
            ThresholdAmount = model.ThresholdAmount,
            PanRequiredForStandardRate = model.PanRequiredForStandardRate,
            AlternateRatePercentIfPanMissing = model.AlternateRatePercentIfPanMissing,
            IsReverseChargeRate = model.IsReverseChargeRate,
            ITCOverride = model.ITCOverride,
            SourceType = model.SourceType,
            LegalReferenceNumber = model.LegalReferenceNumber,
            LegalReferenceDate = model.LegalReferenceDate,
            Notes = model.Notes,
            IsLockedForChanges = model.IsLockedForChanges,
            LockReason = model.LockReason,
            SupersedesVersionId = model.SupersedesVersionId,
            CompanyId = model.CompanyId,
        };
    }
}
