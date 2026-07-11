using FinanceConnect.Client.Data;
using static FinanceConnect.Client.ViewModels.ExpenseCategoryViewModel;

namespace FinanceConnect.Client.Services
{
    public class ExpenseCategoryService
    {
        private List<ExpenseCategory> _items;
        public ExpenseCategoryService() { _items = ExpenseCategorySeedData.GetAll(); }

        public List<ExpenseCategory> GetAll() => _items.Where(x => !x.IsDeleted).ToList();
        public ExpenseCategory? GetById(Guid id) => _items.FirstOrDefault(x => x.ExpenseCategoryId == id && !x.IsDeleted);
        public Task<List<ExpenseCategory>> GetAllAsync() => Task.FromResult(GetAll());
        public Task<ExpenseCategory?> GetByIdAsync(Guid id) => Task.FromResult(GetById(id));

        public void Add(ExpenseCategory m)
        {
            if (_items.Any(x => !x.IsDeleted && x.CompanyId == m.CompanyId && string.Equals(x.CategoryCode, m.CategoryCode, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"Category Code '{m.CategoryCode}' already exists.");
            m.ExpenseCategoryId = Guid.NewGuid(); m.CreatedAt = DateTime.UtcNow; m.IsDeleted = false; _items.Add(m);
        }
        public Task CreateAsync(ExpenseCategory m) { Add(m); return Task.CompletedTask; }

        public void Update(ExpenseCategory m)
        {
            var e = GetById(m.ExpenseCategoryId); if (e is null) return;
            if (e.IsLocked) throw new InvalidOperationException("Locked category cannot be edited.");
            e.CategoryCode=m.CategoryCode; e.CategoryName=m.CategoryName; e.ShortName=m.ShortName; e.Description=m.Description; e.CategoryStatus=m.CategoryStatus;
            e.CategoryType=m.CategoryType; e.ReportingGroup=m.ReportingGroup; e.ParentExpenseCategoryId=m.ParentExpenseCategoryId; e.CategoryNature=m.CategoryNature; e.UsageScope=m.UsageScope;
            e.DefaultGLAccountId=m.DefaultGLAccountId; e.DefaultGLAccountName=m.DefaultGLAccountName; e.AlternateGLAccountId=m.AlternateGLAccountId; e.AlternateGLAccountName=m.AlternateGLAccountName;
            e.AccrualLiabilityGLId=m.AccrualLiabilityGLId; e.AccrualLiabilityGLName=m.AccrualLiabilityGLName; e.PrepaymentAssetGLId=m.PrepaymentAssetGLId; e.PrepaymentAssetGLName=m.PrepaymentAssetGLName;
            e.TaxDefaultCodeId=m.TaxDefaultCodeId; e.IsTaxApplicable=m.IsTaxApplicable; e.DefaultCurrencyBehavior=m.DefaultCurrencyBehavior;
            e.IsReimbursable=m.IsReimbursable; e.ReceiptRequiredFlag=m.ReceiptRequiredFlag; e.ReceiptThresholdAmount=m.ReceiptThresholdAmount; e.ApprovalRequiredFlag=m.ApprovalRequiredFlag;
            e.FinanceReviewRequiredFlag=m.FinanceReviewRequiredFlag; e.DuplicateCheckRequiredFlag=m.DuplicateCheckRequiredFlag; e.EmployeeClaimAllowedFlag=m.EmployeeClaimAllowedFlag;
            e.SupplierExpenseAllowedFlag=m.SupplierExpenseAllowedFlag; e.CashExpenseAllowedFlag=m.CashExpenseAllowedFlag; e.CompanyCardAllowedFlag=m.CompanyCardAllowedFlag; e.BlockedForDirectPostingFlag=m.BlockedForDirectPostingFlag;
            e.BudgetControlApplicableFlag=m.BudgetControlApplicableFlag; e.DefaultBudgetControlMode=m.DefaultBudgetControlMode; e.AccrualAllowedFlag=m.AccrualAllowedFlag; e.PrepaymentAllowedFlag=m.PrepaymentAllowedFlag;
            e.ImmediateExpenseAllowedFlag=m.ImmediateExpenseAllowedFlag; e.DefaultTimingTreatment=m.DefaultTimingTreatment; e.CoverageDatesRequiredFlag=m.CoverageDatesRequiredFlag;
            e.ProjectAllocationRequiredFlag=m.ProjectAllocationRequiredFlag; e.CostCenterMandatoryFlag=m.CostCenterMandatoryFlag; e.DepartmentMandatoryFlag=m.DepartmentMandatoryFlag; e.BranchMandatoryFlag=m.BranchMandatoryFlag;
            e.EffectiveFrom=m.EffectiveFrom; e.EffectiveTo=m.EffectiveTo; e.PreparedByUserId=m.PreparedByUserId; e.ReviewedByUserId=m.ReviewedByUserId; e.ApprovedByUserId=m.ApprovedByUserId;
            e.PolicyNotes=m.PolicyNotes; e.InternalGuidelineReference=m.InternalGuidelineReference; e.Notes=m.Notes;
            e.UpdatedAt = DateTime.UtcNow;
        }
        public Task UpdateAsync(ExpenseCategory m) { Update(m); return Task.CompletedTask; }

        public void Delete(Guid id)
        {
            var e = GetById(id); if (e is null) return;
            if (e.IsLocked) throw new InvalidOperationException("Locked category cannot be deleted.");
            e.IsDeleted = true; e.UpdatedAt = DateTime.UtcNow;
        }
        public Task DeleteAsync(Guid id) { Delete(id); return Task.CompletedTask; }

        public string GenerateCode(Guid companyId)
        {
            var count = _items.Count(x => x.CompanyId == companyId) + 1;
            return $"ECAT-{count:D4}";
        }
    }
}
