
using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;
namespace FinanceConnect.Client.Services
{
    public class CostAllocationService
    {
        private readonly List<CostAllocationViewModel> _items;

        public CostAllocationService()
        {
            _items = CostAllocationSeedData.GetAll();
        }

        // ── Read ───────────────────────────────────────────────────────────────

        public List<CostAllocationViewModel> GetAll() => _items.Where(x => !x.IsDeleted).ToList();

        public Task<List<CostAllocationListDto>> GetAllAsync()
            => Task.FromResult(GetAll().Select(CostAllocationSeedData.ToListDto).ToList());

        public CostAllocationListDto? GetById(Guid id)
        {
            var a = _items.FirstOrDefault(x => x.CostAllocationId == id && !x.IsDeleted);
            return a == null ? null : CostAllocationSeedData.ToListDto(a);
        }

        public Task<CostAllocationListDto?> GetByIdAsync(Guid id)
            => Task.FromResult(GetById(id));

        public string GetCostCenterName(Guid? id)
        {
            if (!id.HasValue) return "";
            return CostAllocationSeedData.CostCenterNames.TryGetValue(id.Value, out var name) ? name : "";
        }

        // ── Create ─────────────────────────────────────────────────────────────

        public void Add(CostAllocationViewModel model)
        {
            if (_items.Any(x => !x.IsDeleted &&
                                x.CompanyId == model.CompanyId &&
                                string.Equals(x.AllocationCode, model.AllocationCode, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Allocation Code already exists for this Company.");

            if (model.SourceAmount < 0)
                throw new InvalidOperationException("Source Amount must be >= 0.");

            if (!model.Lines.Any())
                throw new InvalidOperationException("At least one target line is required.");

            model.CostAllocationId = Guid.NewGuid();
            model.CreatedAt = DateTime.UtcNow;
            model.AllocationStatus = AllocationStatus.Draft;
            model.IsDeleted = false;

            RecalculateSummary(model);
            _items.Add(model);
        }

        public Task CreateAsync(CostAllocationViewModel model)
        {
            Add(model);
            return Task.CompletedTask;
        }

        // ── Update ─────────────────────────────────────────────────────────────

        public void Update(CostAllocationViewModel model)
        {
            var existing = _items.FirstOrDefault(x => x.CostAllocationId == model.CostAllocationId);
            if (existing == null) return;

            if (existing.IsLocked)
                throw new InvalidOperationException("Locked allocation cannot be edited.");

            if (existing.AllocationStatus == AllocationStatus.Applied ||
                existing.AllocationStatus == AllocationStatus.Approved)
                throw new InvalidOperationException("Applied/Approved allocation cannot be structurally changed without reversal or re-run.");

            existing.AllocationCode = model.AllocationCode;
            existing.AllocationName = model.AllocationName;
            existing.Description = model.Description;
            existing.AllocationType = model.AllocationType;
            existing.AllocationDate = model.AllocationDate;
            existing.EffectiveDate = model.EffectiveDate;
            existing.ScopeType = model.ScopeType;
            existing.SourceCostCenterId = model.SourceCostCenterId;
            existing.SourceBudgetLineId = model.SourceBudgetLineId;
            existing.SourceGLAccountId = model.SourceGLAccountId;
            existing.SourceCategoryCode = model.SourceCategoryCode;
            existing.SourceAmount = model.SourceAmount;
            existing.SourceAmountType = model.SourceAmountType;
            existing.SourceReferenceText = model.SourceReferenceText;
            existing.AllocationMethod = model.AllocationMethod;
            existing.AllocationBasisType = model.AllocationBasisType;
            existing.DriverReferenceCode = model.DriverReferenceCode;
            existing.DriverAsOfDate = model.DriverAsOfDate;
            existing.IsManualOverrideAllowed = model.IsManualOverrideAllowed;
            existing.RoundingRule = model.RoundingRule;
            existing.MustFullyAllocateSource = model.MustFullyAllocateSource;
            existing.AllocationAssumptionText = model.AllocationAssumptionText;
            existing.Notes = model.Notes;
            existing.Lines = model.Lines;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = model.UpdatedBy;

            RecalculateSummary(existing);
        }

        public Task UpdateAsync(CostAllocationViewModel model)
        {
            Update(model);
            return Task.CompletedTask;
        }

        // ── Workflow actions ───────────────────────────────────────────────────

        public Task SubmitAsync(Guid id, Guid submittedBy)
        {
            var a = _items.FirstOrDefault(x => x.CostAllocationId == id);
            if (a == null) return Task.CompletedTask;
            if (a.IsLocked) throw new InvalidOperationException("Locked allocation cannot be submitted.");
            a.AllocationStatus = AllocationStatus.Submitted;
            a.SubmittedByUserId = submittedBy;
            a.SubmittedOn = DateTime.UtcNow;
            a.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        public Task ApproveAsync(Guid id, Guid approvedBy)
        {
            var a = _items.FirstOrDefault(x => x.CostAllocationId == id);
            if (a == null) return Task.CompletedTask;
            if (a.MustFullyAllocateSource && !a.IsFullyAllocated)
                throw new InvalidOperationException("Cannot approve: total allocated amount does not match source amount.");
            a.AllocationStatus = AllocationStatus.Approved;
            a.ApprovedByUserId = approvedBy;
            a.ApprovedOn = DateTime.UtcNow;
            a.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        public Task ApplyAsync(Guid id)
        {
            var a = _items.FirstOrDefault(x => x.CostAllocationId == id);
            if (a == null) return Task.CompletedTask;
            if (a.AllocationStatus != AllocationStatus.Approved)
                throw new InvalidOperationException("Only Approved allocations can be Applied.");
            a.AllocationStatus = AllocationStatus.Applied;
            a.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        public Task LockAsync(Guid id, Guid lockedBy)
        {
            var a = _items.FirstOrDefault(x => x.CostAllocationId == id);
            if (a == null) return Task.CompletedTask;
            if (a.AllocationStatus != AllocationStatus.Applied)
                throw new InvalidOperationException("Only Applied allocations can be Locked.");
            a.IsLocked = true;
            a.LockedOn = DateTime.UtcNow;
            a.LockedBy = lockedBy;
            a.AllocationStatus = AllocationStatus.Locked;
            a.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        public Task ReverseAsync(Guid id, string reversalReason, Guid reversedBy)
        {
            var a = _items.FirstOrDefault(x => x.CostAllocationId == id);
            if (a == null) return Task.CompletedTask;
            if (string.IsNullOrWhiteSpace(reversalReason))
                throw new InvalidOperationException("Reversal reason is required.");
            a.AllocationStatus = AllocationStatus.Reversed;
            a.ReversalReason = reversalReason;
            a.IsLocked = false;
            a.UpdatedAt = DateTime.UtcNow;
            a.UpdatedBy = reversedBy;
            return Task.CompletedTask;
        }

        // ── Delete ─────────────────────────────────────────────────────────────

        public Task DeleteAsync(Guid id)
        {
            var a = _items.FirstOrDefault(x => x.CostAllocationId == id);
            if (a == null) return Task.CompletedTask;
            if (a.IsLocked) throw new InvalidOperationException("Locked allocation cannot be deleted.");
            if (a.AllocationStatus != AllocationStatus.Draft)
                throw new InvalidOperationException("Only Draft allocations can be deleted.");
            a.IsDeleted = true;
            a.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        // ── Private helpers ────────────────────────────────────────────────────

        private static void RecalculateSummary(CostAllocationViewModel a)
        {
            a.TotalTargetCount = a.Lines.Count;
            a.TotalAllocatedAmount = a.Lines.Sum(l => l.AllocatedAmount);
            a.UnallocatedAmount = a.SourceAmount - a.TotalAllocatedAmount;
            a.IsFullyAllocated = Math.Abs(a.UnallocatedAmount) < 0.01m;
        }
    }
}
