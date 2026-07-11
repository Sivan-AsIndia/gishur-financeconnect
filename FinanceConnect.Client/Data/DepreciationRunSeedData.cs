using FinanceConnect.Client.ViewModels;
using static FinanceConnect.Client.Data.MasterDataIds;

namespace FinanceConnect.Client.Data
{
    public class DepreciationRunSeedData
    {
        public List<DepreciationRunViewModel> Seed(
            List<CompanyModel> companies,
            List<FiscalYearModel> fiscalYears,
            List<AccountingPeriodModel> periods)
        {
            var runs = new List<DepreciationRunViewModel>();

            Guid tenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            int runCounter = 1;
            foreach (var company in companies)
            {
                // Get OPEN Fiscal Year
                var fiscalYear = fiscalYears
                    .FirstOrDefault(fy =>
                        fy.CompanyId == company.Id &&
                        fy.Status == FiscalYearStatus.Open);

                if (fiscalYear == null)
                    continue;

                // Get OPEN Accounting Period (only one allowed)
                var period = periods
                    .FirstOrDefault(p =>
                        p.FiscalYearId == fiscalYear.Id &&
                        p.CompanyId == company.Id &&
                        p.Status == AccountingPeriodStatus.Open);

                if (period == null)
                    continue;

                // Create 3 depreciation runs per company
                for (int counter = 1; counter <= 3; counter++)
                {
                    var status = GetStatus(counter);

                    bool isGenerated = status != DepreciationRunStatus.Draft;

                    // ✅ Deterministic GUID
                    var guidString = $"22222222-2222-2222-2222-{runCounter:000000000000}";

                    runs.Add(new DepreciationRunViewModel
                    {
                        DepreciationRunId = Guid.Parse(guidString),
                        TenantId = tenantId,

                        CompanyId = company.Id,
                        BranchId = null,

                        RunNumber = GenerateRunNumber(counter),

                        RunType = DepreciationRunType.Monthly,

                        AccountingPeriodId = period.Id,
                        AccountingPeriodName = period.PeriodName,

                        PeriodStartDateSnapshot = period.StartDate,
                        PeriodEndDateSnapshot = period.EndDate,

                        AsOfDate = period.EndDate,

                        // Draft should have zero values
                        GeneratedLineCount = isGenerated ? Random.Shared.Next(50, 200) : 0,

                        TotalDepreciationAmount = isGenerated ? Random.Shared.Next(50000, 200000) : 0,
                        TotalExpenseAmount = isGenerated ? Random.Shared.Next(50000, 200000) : 0,
                        TotalAccumDepAmount = isGenerated ? Random.Shared.Next(50000, 200000) : 0,

                        EligibleAssetsCount = isGenerated ? Random.Shared.Next(50, 200) : 0,
                        ExcludedAssetCount = isGenerated ? Random.Shared.Next(0, 10) : 0,

                        HasExceptions = false,
                        ExceptionSeverityLevel = "None",

                        RunStatus = status,

                        CreatedAt = DateTime.UtcNow.AddDays(-counter * 4),

                        GeneratedOn = status >= DepreciationRunStatus.Generated
                            ? DateTime.UtcNow.AddDays(-counter * 3)
                            : null,

                        PostedOn = status == DepreciationRunStatus.Posted
                            ? DateTime.UtcNow.AddDays(-counter)
                            : null
                    });
                    runCounter++;
                }
            }

            return runs;
        }

        private string GenerateRunNumber(int counter)
        {
            return $"DEP-{counter:000000}";
        }

        private DepreciationRunStatus GetStatus(int index)
        {
            return index switch
            {
                1 => DepreciationRunStatus.Draft,
                2 => DepreciationRunStatus.Generated,
                3 => DepreciationRunStatus.Posted,
                _ => DepreciationRunStatus.Draft
            };
        }
    }
}