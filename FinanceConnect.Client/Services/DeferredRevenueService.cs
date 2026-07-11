using FinanceConnect.Client.Data;
using static FinanceConnect.Client.ViewModels.DeferredRevenueViewModel;

namespace FinanceConnect.Client.Services
{
    public class DeferredRevenueService
    {
        private List<DeferredRevenue> _items = new();

        public DeferredRevenueService() { _items = DeferredRevenueSeedData.GetAll(); }

        public List<DeferredRevenue> GetAll() => _items.Where(x => !x.IsDeleted).ToList();
        public DeferredRevenue? GetById(Guid id) => _items.FirstOrDefault(x => x.DeferredRevenueId == id && !x.IsDeleted);
        public Task<List<DeferredRevenue>> GetAllAsync() => Task.FromResult(GetAll());
        public Task<DeferredRevenue?> GetByIdAsync(Guid id) => Task.FromResult(GetById(id));

        public string GenerateCode(Guid companyId)
        {
            var count = _items.Count(x => x.CompanyId == companyId && !x.IsDeleted) + 1;
            return $"DEFREV-2026-{count:D4}";
        }

        public void Add(DeferredRevenue model)
        {
            if (_items.Any(x => !x.IsDeleted && x.CompanyId == model.CompanyId &&
                string.Equals(x.DeferredRevenueCode, model.DeferredRevenueCode, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"Deferred Revenue Code '{model.DeferredRevenueCode}' already exists.");
            if (model.OriginalDeferredAmount <= 0)
                throw new InvalidOperationException("Deferred amount must be greater than zero.");
            if (model.DeferredEndDate < model.DeferredStartDate)
                throw new InvalidOperationException("Coverage end date cannot be earlier than start date.");
            model.DeferredRevenueId = Guid.NewGuid();
            model.CreatedAt = DateTime.UtcNow;
            model.IsDeleted = false;
            _items.Add(model);
        }

        public Task CreateAsync(DeferredRevenue model) { Add(model); return Task.CompletedTask; }

        public void Update(DeferredRevenue model)
        {
            var e = GetById(model.DeferredRevenueId);
            if (e is null) return;
            if (e.IsLocked) throw new InvalidOperationException("Locked deferred revenue cannot be edited.");
            if (e.DeferredRevenueStatus == DeferredRevenueStatusEnum.Closed)
                throw new InvalidOperationException("Closed deferred revenue cannot be edited.");

            e.DeferredRevenueTitle = model.DeferredRevenueTitle; e.Description = model.Description;
            e.DeferredRevenueStatus = model.DeferredRevenueStatus; e.SourceType = model.SourceType;
            e.SourceDocumentNumber = model.SourceDocumentNumber;
            e.CustomerId = model.CustomerId; e.CustomerName = model.CustomerName;
            e.BasisReferenceText = model.BasisReferenceText;
            e.DeferredStartDate = model.DeferredStartDate; e.DeferredEndDate = model.DeferredEndDate;
            e.ReleaseMethod = model.ReleaseMethod; e.ReleaseFrequency = model.ReleaseFrequency;
            e.ReleaseStartDate = model.ReleaseStartDate; e.ReleaseEndDate = model.ReleaseEndDate;
            e.CurrencyId = model.CurrencyId; e.OriginalDeferredAmount = model.OriginalDeferredAmount;
            e.FiscalYearId = model.FiscalYearId;
            e.RevenueGLAccountId = model.RevenueGLAccountId; e.RevenueGLAccountName = model.RevenueGLAccountName;
            e.DeferredRevenueLiabilityGLId = model.DeferredRevenueLiabilityGLId;
            e.DeferredRevenueLiabilityGLName = model.DeferredRevenueLiabilityGLName;
            e.PostingStatus = model.PostingStatus; e.ReleaseStatus = model.ReleaseStatus;
            e.MaterialityLevel = model.MaterialityLevel;
            e.RevenueCategoryCode = model.RevenueCategoryCode;
            e.BranchId = model.BranchId; e.BranchName = model.BranchName;
            e.CostCenterId = model.CostCenterId; e.DepartmentId = model.DepartmentId;
            e.AssumptionText = model.AssumptionText; e.FinanceNotes = model.FinanceNotes;
            e.PolicyExceptionFlag = model.PolicyExceptionFlag;
            e.PolicyExceptionReason = model.PolicyExceptionReason;
            e.CancellationReason = model.CancellationReason;
            e.UpdatedAt = DateTime.UtcNow;
        }

        public Task UpdateAsync(DeferredRevenue model) { Update(model); return Task.CompletedTask; }

        public void Delete(Guid id)
        {
            var e = GetById(id);
            if (e is null) return;
            if (e.DeferredRevenueStatus != DeferredRevenueStatusEnum.Draft)
                throw new InvalidOperationException("Only Draft deferred revenue records can be deleted.");
            e.IsDeleted = true;
        }

        public Task DeleteAsync(Guid id) { Delete(id); return Task.CompletedTask; }
    }
}
