using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;
using static FinanceConnect.Client.ViewModels.TaxCategoryMappingViewModel;

namespace FinanceConnect.Client.Services
{
    public class TaxCategoryMappingService
    {
        private readonly TaxCategoryMappingSeedData _seed;
        private List<TaxCategoryMappingListDto> _store => _seed.Store;

        public TaxCategoryMappingService(TaxCategoryMappingSeedData seed)
            => _seed = seed;

        public Task<List<TaxCategoryMappingListDto>> GetAllAsync()
            => Task.FromResult(_store.OrderByDescending(x => x.EffectiveFrom).ToList());

        public Task<TaxCategoryMappingListDto?> GetByIdAsync(Guid id)
            => Task.FromResult(_store.FirstOrDefault(x => x.TaxCategoryMappingId == id));

        public Task<TaxCategoryMappingModel?> GetModelByIdAsync(Guid id)
        {
            var item = _store.FirstOrDefault(x => x.TaxCategoryMappingId == id);
            if (item == null) return Task.FromResult<TaxCategoryMappingModel?>(null);

            var model = new TaxCategoryMappingModel
            {
                Id = item.TaxCategoryMappingId,
                MappingCode = item.MappingCode,
                MappingName = item.MappingName,
                TaxTypeScope = item.TaxTypeScope.ToString(),
                TransactionContext = item.TransactionContext.ToString(),
                MappingStatus = item.MappingStatus.ToString(),
                Priority = item.Priority,
                EffectiveFrom = item.EffectiveFrom,
                EffectiveTo = item.EffectiveTo,
                SupplyType = item.SupplyType?.ToString(),
                IsExemptOrNilOrNonGST = item.IsExemptOrNilOrNonGST.ToString(),
                IsLockedForChanges = item.IsLockedForChanges,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt,
                Lines = _seed.GetLines(id),
            };
            return Task.FromResult<TaxCategoryMappingModel?>(model);
        }

        public Task SaveAsync(TaxCategoryMappingModel model)
        {
            ValidateOrThrow(model);

            var existing = _store.FirstOrDefault(x => x.TaxCategoryMappingId == model.Id);

            if (existing != null)
            {
                if (existing.IsLockedForChanges)
                    throw new InvalidOperationException("Mapping is locked and cannot be edited.");

                existing.MappingCode = model.MappingCode;
                existing.MappingName = model.MappingName;
                existing.TaxTypeScope = Enum.Parse<TaxTypeScope>(model.TaxTypeScope);
                existing.TransactionContext = Enum.Parse<TransactionContext>(model.TransactionContext);
                existing.MappingStatus = Enum.Parse<MappingStatus>(model.MappingStatus);
                existing.Priority = model.Priority;
                existing.EffectiveFrom = model.EffectiveFrom;
                existing.EffectiveTo = model.EffectiveTo;
                existing.SupplyType = string.IsNullOrEmpty(model.SupplyType)
                                                  ? null
                                                  : Enum.Parse<SupplyType>(model.SupplyType);
                existing.IsExemptOrNilOrNonGST = Enum.Parse<ExemptType>(model.IsExemptOrNilOrNonGST);
                existing.IsLockedForChanges = model.IsLockedForChanges;
                existing.LineCount = model.Lines.Count(l => l.IsLineActive);
                existing.UpdatedAt = DateTime.Now;

                _seed.Lines[model.Id] = model.Lines;
            }
            else
            {
                var newId = Guid.NewGuid();
                _store.Add(new TaxCategoryMappingListDto
                {
                    TaxCategoryMappingId = newId,
                    MappingCode = model.MappingCode,
                    MappingName = model.MappingName,
                    TaxTypeScope = Enum.Parse<TaxTypeScope>(model.TaxTypeScope),
                    TransactionContext = Enum.Parse<TransactionContext>(model.TransactionContext),
                    MappingStatus = Enum.Parse<MappingStatus>(model.MappingStatus),
                    Priority = model.Priority,
                    EffectiveFrom = model.EffectiveFrom,
                    EffectiveTo = model.EffectiveTo,
                    SupplyType = string.IsNullOrEmpty(model.SupplyType)
                                            ? null
                                            : Enum.Parse<SupplyType>(model.SupplyType),
                    IsExemptOrNilOrNonGST = Enum.Parse<ExemptType>(model.IsExemptOrNilOrNonGST),
                    IsLockedForChanges = false,
                    LineCount = model.Lines.Count(l => l.IsLineActive),
                    CreatedAt = DateTime.Now,
                });

                _seed.Lines[newId] = model.Lines;
            }
            return Task.CompletedTask;
        }

        public Task ActivateAsync(Guid id) => ChangeStatus(id, MappingStatus.Active);
        public Task InactivateAsync(Guid id) => ChangeStatus(id, MappingStatus.Inactive);
        public Task ArchiveAsync(Guid id) => ChangeStatus(id, MappingStatus.Archived);

        public Task LockAsync(Guid id, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new InvalidOperationException("Lock reason is required.");
            var item = GetOrThrow(id);
            item.IsLockedForChanges = true;
            item.UpdatedAt = DateTime.Now;
            return Task.CompletedTask;
        }

        public Task UnlockAsync(Guid id)
        {
            var item = GetOrThrow(id);
            item.IsLockedForChanges = false;
            item.UpdatedAt = DateTime.Now;
            return Task.CompletedTask;
        }

        private Task ChangeStatus(Guid id, MappingStatus status)
        {
            var item = GetOrThrow(id);
            item.MappingStatus = status;
            item.UpdatedAt = DateTime.Now;
            return Task.CompletedTask;
        }

        private TaxCategoryMappingListDto GetOrThrow(Guid id)
            => _store.FirstOrDefault(x => x.TaxCategoryMappingId == id)
               ?? throw new InvalidOperationException("Tax Category Mapping not found.");

        private static void ValidateOrThrow(TaxCategoryMappingModel m)
        {
            if (string.IsNullOrWhiteSpace(m.MappingCode))
                throw new InvalidOperationException("Mapping Code is required.");
            if (string.IsNullOrWhiteSpace(m.MappingName))
                throw new InvalidOperationException("Mapping Name is required.");
            if (m.Priority <= 0)
                throw new InvalidOperationException("Priority must be greater than 0.");
            if (m.EffectiveTo.HasValue && m.EffectiveTo < m.EffectiveFrom)
                throw new InvalidOperationException("Effective To must be >= Effective From.");
            if (m.IsLockedForChanges && string.IsNullOrWhiteSpace(m.LockReason))
                throw new InvalidOperationException("Lock Reason is required when locking.");
            if (m.IsExemptOrNilOrNonGST == "None" && !m.Lines.Any(l => l.IsLineActive))
                throw new InvalidOperationException("At least one active tax line is required for non-exempt mappings.");
        }
    }
}
