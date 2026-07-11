using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;

namespace FinanceConnect.Client.Data
{
    public class BudgetSeedData
    {
        private readonly MasterDataService _masterDataService;

        public BudgetSeedData(MasterDataService masterDataService)
        {
            _masterDataService = masterDataService;
        }

        public List<BudgetViewModel> Seed()
        {
            var list = new List<BudgetViewModel>();

            var companies = _masterDataService.GetAllCompanies();

            var statuses = new[] { "Approved", "Draft", "Submitted", "Locked" };
            int counter = 1;

            foreach (var company in companies)
            {
                for (int i = 0; i < statuses.Length; i++)
                {
                    var status = statuses[i];

                    var guidString = $"11111111-1111-1111-1111-{counter:000000000000}";

                    list.Add(new BudgetViewModel
                    {
                        BudgetId = Guid.Parse(guidString),
                        TenantId = Guid.NewGuid(),

                        // ✅ REAL COMPANY
                        CompanyId = company.Id,

                        BudgetCode = $"BUD-{company.CompanyCode}-{i + 1:000}",
                        BudgetName = $"{company.LegalName} - FY2026 {status}",

                        BudgetType = BudgetType.Original,
                        BudgetNature = BudgetNature.OperatingExpense,
                        PlanningLevel = PlanningLevel.Company,

                        FiscalYearId = Guid.NewGuid(),
                        StartDate = new DateTime(2026, 4, 1),
                        EndDate = new DateTime(2027, 3, 31),

                        PeriodGranularity = PeriodGranularity.Monthly,

                        CurrencyId = company.BaseCurrencyId, // ✅ better than random

                        BudgetOwnerUserId = Guid.NewGuid(),
                        PreparedByUserId = Guid.NewGuid(),

                        Status = status,

                        // Governance alignment
                        IsLocked = status == "Locked" || status == "Approved",
                        IsArchived = false,

                        TotalBudgetAmount = 1000000 + (i * 50000),

                        CreatedAt = DateTime.UtcNow.AddDays(-i)
                    });
                    counter++;
                }
            }

            return list;
        }
    }
}