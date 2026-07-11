using FinanceConnect.Client.Data;
using static FinanceConnect.Client.ViewModels.AccrualViewModel;

namespace FinanceConnect.Client.Services
{
    public class AccrualService
    {
        private List<Accrual> _items = new();

        public AccrualService() { _items = AccrualSeedData.GetAll(); }

        public List<Accrual> GetAll() => _items.Where(x => !x.IsDeleted).ToList();
        public Accrual? GetById(Guid id) => _items.FirstOrDefault(x => x.AccrualId == id && !x.IsDeleted);
        public Task<List<Accrual>> GetAllAsync() => Task.FromResult(GetAll());
        public Task<Accrual?> GetByIdAsync(Guid id) => Task.FromResult(GetById(id));

        public string GenerateCode(Guid companyId)
        {
            var count = _items.Count(x => x.CompanyId == companyId && !x.IsDeleted) + 1;
            return $"ACCR-2026-{count:D4}";
        }

        public void Add(Accrual model)
        {
            if (_items.Any(x => !x.IsDeleted && x.CompanyId == model.CompanyId &&
                string.Equals(x.AccrualCode, model.AccrualCode, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"Accrual Code '{model.AccrualCode}' already exists.");
            if (model.OriginalAccrualAmount <= 0)
                throw new InvalidOperationException("Accrual amount must be greater than zero.");
            model.AccrualId = Guid.NewGuid();
            model.CreatedAt = DateTime.UtcNow;
            model.IsDeleted = false;
            _items.Add(model);
        }

        public Task CreateAsync(Accrual model) { Add(model); return Task.CompletedTask; }

        public void Update(Accrual model)
        {
            var e = GetById(model.AccrualId);
            if (e is null) return;
            if (e.IsLocked) throw new InvalidOperationException("Locked accrual cannot be edited.");
            if (e.AccrualStatus == AccrualStatusEnum.Closed)
                throw new InvalidOperationException("Closed accrual cannot be edited.");

            e.AccrualTitle = model.AccrualTitle; e.Description = model.Description;
            e.AccrualStatus = model.AccrualStatus; e.AccrualType = model.AccrualType;
            e.SourceType = model.SourceType; e.SourceDocumentType = model.SourceDocumentType;
            e.SourceDocumentNumber = model.SourceDocumentNumber;
            e.BasisReferenceText = model.BasisReferenceText;
            e.AccrualDate = model.AccrualDate; e.AccountingPeriodId = model.AccountingPeriodId;
            e.FiscalYearId = model.FiscalYearId; e.CurrencyId = model.CurrencyId;
            e.OriginalAccrualAmount = model.OriginalAccrualAmount;
            e.EstimatedActualAmount = model.EstimatedActualAmount;
            e.MaterialityLevel = model.MaterialityLevel;
            e.AccrualBasisType = model.AccrualBasisType;
            e.SupportingAmountReference = model.SupportingAmountReference;
            e.ReversalStrategy = model.ReversalStrategy; e.AutoReverseDate = model.AutoReverseDate;
            e.ExpectedClearanceMode = model.ExpectedClearanceMode;
            e.ReversalStatus = model.ReversalStatus; e.ClearanceStatus = model.ClearanceStatus;
            e.PostingStatus = model.PostingStatus;
            e.ServiceOrCoverageFrom = model.ServiceOrCoverageFrom;
            e.ServiceOrCoverageTo = model.ServiceOrCoverageTo;
            e.ExpectedActualDocumentDate = model.ExpectedActualDocumentDate;
            e.CostCenterId = model.CostCenterId; e.DepartmentId = model.DepartmentId;
            e.BranchId = model.BranchId; e.BranchName = model.BranchName;
            e.GLAccountId = model.GLAccountId; e.GLAccountName = model.GLAccountName;
            e.AccrualLiabilityOrAssetGLId = model.AccrualLiabilityOrAssetGLId;
            e.AccrualLiabilityOrAssetGLName = model.AccrualLiabilityOrAssetGLName;
            e.AssumptionText = model.AssumptionText; e.FinanceNotes = model.FinanceNotes;
            e.PolicyExceptionFlag = model.PolicyExceptionFlag;
            e.PolicyExceptionReason = model.PolicyExceptionReason;
            e.CancellationReason = model.CancellationReason;
            e.UpdatedAt = DateTime.UtcNow;
        }

        public Task UpdateAsync(Accrual model) { Update(model); return Task.CompletedTask; }

        public void Delete(Guid id)
        {
            var e = GetById(id);
            if (e is null) return;
            if (e.AccrualStatus != AccrualStatusEnum.Draft)
                throw new InvalidOperationException("Only Draft accruals can be deleted.");
            e.IsDeleted = true;
        }

        public Task DeleteAsync(Guid id) { Delete(id); return Task.CompletedTask; }
    }
}
