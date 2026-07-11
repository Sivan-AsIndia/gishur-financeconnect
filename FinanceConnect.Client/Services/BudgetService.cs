using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;
using static System.Net.WebRequestMethods;

namespace FinanceConnect.Client.Services
{
    public class BudgetService
    {
        private readonly MasterDataService _masterDataService;
        private readonly BudgetSeedData _seed;
        private static List<BudgetViewModel> _store = new();

        public BudgetService(MasterDataService masterDataService)
        {
            _masterDataService = masterDataService;
            _seed = new BudgetSeedData(_masterDataService);

            if (!_store.Any())
                _store = _seed.Seed();
        }

        public List<BudgetViewModel> GetAll()
        {
            return _store.OrderByDescending(x => x.CreatedAt).ToList();
        }

        public Task<BudgetViewModel?> GetByIdAsync(Guid id)
        {
            var item = _store.FirstOrDefault(x => x.BudgetId == id);
            return Task.FromResult(item);
        }

        public string GetCountryNameById(Guid? countryId)
        {
            return _masterDataService
                .GetAllCountries()
                .FirstOrDefault(c => c.Id == countryId)
                ?.CountryName ?? "-";
        }

        public Task ArchiveAsync(Guid budgetId)
        {
            var budget = _store.FirstOrDefault(x => x.BudgetId == budgetId);

            if (budget == null)
                throw new Exception("Budget not found.");

            // Governance validation
            if (!budget.IsLocked)
                throw new Exception("Only locked budgets can be archived.");

            if (budget.IsArchived)
                throw new Exception("Budget is already archived.");

            // Apply archive
            budget.IsArchived = true;
            budget.Status = "Archived";
            budget.UpdatedAt = DateTime.UtcNow;

            return Task.CompletedTask;
        }

        public Task CreateAsync(BudgetViewModel model)
        {
            // Core validations
            if (string.IsNullOrWhiteSpace(model.BudgetCode))
                model.BudgetCode = GenerateNumber();

            if (_store.Any(x => x.CompanyId == model.CompanyId &&
                                x.BudgetCode == model.BudgetCode))
                throw new Exception("Budget code must be unique per company.");

            if (string.IsNullOrWhiteSpace(model.BudgetName))
                throw new Exception("Budget name is required.");

            if (model.StartDate == null || model.EndDate == null)
                throw new Exception("Start date and End date are required.");

            if (model.EndDate < model.StartDate)
                throw new Exception("Budget end date cannot be earlier than start date.");

            if (model.BudgetOwnerUserId == Guid.Empty)
                throw new Exception("Budget owner is required.");

            if (model.CurrencyId == Guid.Empty)
                throw new Exception("Currency is required.");

            // Scope validations
            if (model.PlanningLevel == PlanningLevel.Branch && model.BranchId == null)
                throw new Exception("Branch is required for Branch level.");

            if (model.PlanningLevel == PlanningLevel.Department && model.DepartmentId == null)
                throw new Exception("Department is required for Department level.");

            if (model.PlanningLevel == PlanningLevel.Project && model.ProjectId == null)
                throw new Exception("Project is required for Project level.");

            // Version validations
            if (model.BudgetType != BudgetType.Original && model.PreviousBudgetId == null)
                throw new Exception("Revised budget must reference a previous approved budget.");

            // Active approved validation
            if (model.IsCurrentApprovedVersion &&
                _store.Any(x => x.CompanyId == model.CompanyId &&
                                x.FiscalYearId == model.FiscalYearId &&
                                x.IsCurrentApprovedVersion))
            {
                throw new Exception("An active approved budget already exists for this scope and fiscal year.");
            }

            model.BudgetId = Guid.NewGuid();
            model.CreatedAt = DateTime.UtcNow;
            model.VersionNumber = 1;
            model.RevisionNumber = 0;

            _store.Add(model);

            return Task.CompletedTask;
        }

        public Task UpdateAsync(BudgetViewModel model)
        {
            var existing = _store.FirstOrDefault(x => x.BudgetId == model.BudgetId);

            if (existing == null)
                throw new Exception("Budget not found.");

            // Workflow validation
            if (existing.IsLocked)
                throw new Exception("Locked budget cannot be edited.");

            if (!existing.IsDraft)
                throw new Exception("Only Draft budget can be edited.");

            // Core validations
            if (model.EndDate < model.StartDate)
                throw new Exception("Budget end date cannot be earlier than start date.");

            if (model.BudgetOwnerUserId == Guid.Empty)
                throw new Exception("Budget owner is required.");

            // Scope validations
            if (model.PlanningLevel == PlanningLevel.Branch && model.BranchId == null)
                throw new Exception("Branch is required.");

            if (model.PlanningLevel == PlanningLevel.Department && model.DepartmentId == null)
                throw new Exception("Department is required.");

            if (model.PlanningLevel == PlanningLevel.Project && model.ProjectId == null)
                throw new Exception("Project is required.");

            // Structural validation
            if (model.HasPeriodDistributionGenerated && model.TotalPeriodsPlanned == 0)
                throw new Exception("Invalid period distribution.");

            // Update fields
            existing.BudgetName = model.BudgetName;
            existing.StartDate = model.StartDate;
            existing.EndDate = model.EndDate;
            existing.TotalBudgetAmount = model.TotalBudgetAmount;

            existing.UpdatedAt = DateTime.UtcNow;

            return Task.CompletedTask;
        }

        public static string GenerateNumber()
        {
            int next = _store.Count + 1;
            return $"BUD-{next:000000}";
        }

        public void Submit(Guid id)
        {
            var b = _store.First(x => x.BudgetId == id);
            b.Status = "Submitted";
        }

        public void Approve(Guid id)
        {
            var b = _store.First(x => x.BudgetId == id);
            b.Status = "Approved";
        }

        public void Lock(Guid id)
        {
            var b = _store.First(x => x.BudgetId == id);
            b.Status = "Locked";
            b.IsLocked = true;
        }

        public void ResetToSeed()
        {
            _store = _seed.Seed();
        }
    }
}