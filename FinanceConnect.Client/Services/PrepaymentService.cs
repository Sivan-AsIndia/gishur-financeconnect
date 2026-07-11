using FinanceConnect.Client.Data;
using static FinanceConnect.Client.ViewModels.PrepaymentViewModel;

namespace FinanceConnect.Client.Services
{
    public class PrepaymentService
    {
        private List<Prepayment> _items = new();

        public PrepaymentService() { _items = PrepaymentSeedData.GetAll(); }

        public List<Prepayment> GetAll() => _items.Where(x => !x.IsDeleted).ToList();
        public Prepayment? GetById(Guid id) => _items.FirstOrDefault(x => x.PrepaymentId == id && !x.IsDeleted);
        public Task<List<Prepayment>> GetAllAsync() => Task.FromResult(GetAll());
        public Task<Prepayment?> GetByIdAsync(Guid id) => Task.FromResult(GetById(id));

        public string GenerateCode(Guid companyId)
        {
            var count = _items.Count(x => x.CompanyId == companyId && !x.IsDeleted) + 1;
            return $"PREP-2026-{count:D4}";
        }

        public void Add(Prepayment model)
        {
            if (_items.Any(x => !x.IsDeleted && x.CompanyId == model.CompanyId &&
                string.Equals(x.PrepaymentCode, model.PrepaymentCode, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"Prepayment Code '{model.PrepaymentCode}' already exists.");
            if (model.OriginalPrepaidAmount <= 0)
                throw new InvalidOperationException("Prepaid amount must be greater than zero.");
            if (model.PrepaymentEndDate < model.PrepaymentStartDate)
                throw new InvalidOperationException("Coverage end date cannot be earlier than start date.");
            model.PrepaymentId = Guid.NewGuid();
            model.CreatedAt = DateTime.UtcNow;
            model.IsDeleted = false;
            _items.Add(model);
        }

        public Task CreateAsync(Prepayment model) { Add(model); return Task.CompletedTask; }

        public void Update(Prepayment model)
        {
            var e = GetById(model.PrepaymentId);
            if (e is null) return;
            if (e.IsLocked) throw new InvalidOperationException("Locked prepayment cannot be edited.");
            if (e.PrepaymentStatus == PrepaymentStatusEnum.Closed)
                throw new InvalidOperationException("Closed prepayment cannot be edited.");

            e.PrepaymentTitle = model.PrepaymentTitle; e.Description = model.Description;
            e.PrepaymentStatus = model.PrepaymentStatus; e.SourceType = model.SourceType;
            e.SourceDocumentNumber = model.SourceDocumentNumber;
            e.BasisReferenceText = model.BasisReferenceText;
            e.PrepaymentStartDate = model.PrepaymentStartDate;
            e.PrepaymentEndDate = model.PrepaymentEndDate;
            e.ReleaseMethod = model.ReleaseMethod; e.ReleaseFrequency = model.ReleaseFrequency;
            e.ReleaseStartDate = model.ReleaseStartDate; e.ReleaseEndDate = model.ReleaseEndDate;
            e.CurrencyId = model.CurrencyId; e.OriginalPrepaidAmount = model.OriginalPrepaidAmount;
            e.FiscalYearId = model.FiscalYearId;
            e.ExpenseGLAccountId = model.ExpenseGLAccountId; e.ExpenseGLAccountName = model.ExpenseGLAccountName;
            e.PrepaymentAssetGLId = model.PrepaymentAssetGLId; e.PrepaymentAssetGLName = model.PrepaymentAssetGLName;
            e.PostingStatus = model.PostingStatus; e.ReleaseStatus = model.ReleaseStatus;
            e.MaterialityLevel = model.MaterialityLevel;
            e.BranchId = model.BranchId; e.BranchName = model.BranchName;
            e.CostCenterId = model.CostCenterId; e.DepartmentId = model.DepartmentId;
            e.AssumptionText = model.AssumptionText; e.FinanceNotes = model.FinanceNotes;
            e.PolicyExceptionFlag = model.PolicyExceptionFlag;
            e.PolicyExceptionReason = model.PolicyExceptionReason;
            e.CancellationReason = model.CancellationReason;
            e.UpdatedAt = DateTime.UtcNow;
        }

        public Task UpdateAsync(Prepayment model) { Update(model); return Task.CompletedTask; }

        public void Delete(Guid id)
        {
            var e = GetById(id);
            if (e is null) return;
            if (e.PrepaymentStatus != PrepaymentStatusEnum.Draft)
                throw new InvalidOperationException("Only Draft prepayments can be deleted.");
            e.IsDeleted = true;
        }

        public Task DeleteAsync(Guid id) { Delete(id); return Task.CompletedTask; }
    }
}
