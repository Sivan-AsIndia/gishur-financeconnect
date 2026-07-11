using FinanceConnect.Client.Data;
using static FinanceConnect.Client.ViewModels.ExpenseViewModel;

namespace FinanceConnect.Client.Services
{
    public class ExpenseService
    {
        private List<Expense> _items;
        public ExpenseService() { _items = ExpenseSeedData.GetAll(); }

        public List<Expense> GetAll() => _items.Where(x => !x.IsDeleted).ToList();
        public Expense? GetById(Guid id) => _items.FirstOrDefault(x => x.ExpenseId == id && !x.IsDeleted);
        public Task<List<Expense>> GetAllAsync() => Task.FromResult(GetAll());
        public Task<Expense?> GetByIdAsync(Guid id) => Task.FromResult(GetById(id));

        public void Add(Expense m)
        {
            if (_items.Any(x => !x.IsDeleted && x.CompanyId == m.CompanyId && string.Equals(x.ExpenseCode, m.ExpenseCode, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"Expense Code '{m.ExpenseCode}' already exists.");
            m.ExpenseId = Guid.NewGuid(); m.CreatedAt = DateTime.UtcNow; m.IsDeleted = false;
            foreach (var l in m.Lines) { l.ExpenseLineId = Guid.NewGuid(); l.ExpenseId = m.ExpenseId; }
            _items.Add(m);
        }
        public Task CreateAsync(Expense m) { Add(m); return Task.CompletedTask; }

        public void Update(Expense m)
        {
            var e = GetById(m.ExpenseId); if (e is null) return;
            if (e.IsLocked) throw new InvalidOperationException("Locked expense cannot be edited.");
            if (e.ExpenseStatus == ExpenseStatusEnum.Posted || e.ExpenseStatus == ExpenseStatusEnum.Closed) throw new InvalidOperationException("Posted or closed expense cannot be edited.");
            e.ExpenseTitle=m.ExpenseTitle; e.Description=m.Description; e.ExpenseStatus=m.ExpenseStatus; e.ExpenseType=m.ExpenseType;
            e.PayeeType=m.PayeeType; e.PayeeId=m.PayeeId; e.PayeeCodeSnapshot=m.PayeeCodeSnapshot; e.PayeeNameSnapshot=m.PayeeNameSnapshot;
            e.SupplierInvoiceNumber=m.SupplierInvoiceNumber; e.SourceDocumentType=m.SourceDocumentType; e.SourceDocumentNumber=m.SourceDocumentNumber;
            e.ExpenseDate=m.ExpenseDate; e.PostingDate=m.PostingDate; e.FiscalYearId=m.FiscalYearId; e.CoverageStartDate=m.CoverageStartDate; e.CoverageEndDate=m.CoverageEndDate;
            e.CurrencyId=m.CurrencyId; e.ExchangeRateId=m.ExchangeRateId; e.TaxInclusiveFlag=m.TaxInclusiveFlag;
            e.TotalNetAmount=m.TotalNetAmount; e.TotalTaxAmount=m.TotalTaxAmount; e.TotalGrossAmount=m.TotalGrossAmount; e.AdjustmentAmount=m.AdjustmentAmount;
            e.AccrualRequiredFlag=m.AccrualRequiredFlag; e.PrepaymentRequiredFlag=m.PrepaymentRequiredFlag; e.TimingTreatment=m.TimingTreatment; e.TimingTreatmentStatus=m.TimingTreatmentStatus;
            e.CostCenterId=m.CostCenterId; e.CostCenterName=m.CostCenterName; e.DepartmentId=m.DepartmentId; e.DepartmentName=m.DepartmentName;
            e.BranchId=m.BranchId; e.BranchName=m.BranchName; e.ProjectId=m.ProjectId; e.ProjectName=m.ProjectName;
            e.ExpenseOwnerUserText=m.ExpenseOwnerUserText; e.BudgetCheckStatus=m.BudgetCheckStatus;
            e.PreparedByUserId=m.PreparedByUserId; e.ReviewedByUserId=m.ReviewedByUserId; e.ApprovedByUserId=m.ApprovedByUserId;
            e.PostingStatus=m.PostingStatus; e.RejectionReason=m.RejectionReason; e.CancellationReason=m.CancellationReason;
            e.ReceiptRequiredFlag=m.ReceiptRequiredFlag; e.Notes=m.Notes; e.SupportingCommentary=m.SupportingCommentary;
            e.Lines = m.Lines;
            e.UpdatedAt = DateTime.UtcNow;
        }
        public Task UpdateAsync(Expense m) { Update(m); return Task.CompletedTask; }

        public void Delete(Guid id)
        {
            var e = GetById(id); if (e is null) return;
            if (e.IsLocked) throw new InvalidOperationException("Locked expense cannot be deleted.");
            if (e.ExpenseStatus == ExpenseStatusEnum.Posted || e.ExpenseStatus == ExpenseStatusEnum.Closed) throw new InvalidOperationException("Posted or closed expense cannot be deleted.");
            e.IsDeleted = true; e.UpdatedAt = DateTime.UtcNow;
        }
        public Task DeleteAsync(Guid id) { Delete(id); return Task.CompletedTask; }

        public string GenerateCode(Guid companyId)
        {
            var count = _items.Count(x => x.CompanyId == companyId) + 1;
            return $"EXP-2026-{count:D4}";
        }
    }
}
