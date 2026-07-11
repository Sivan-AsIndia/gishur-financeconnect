using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    public class DepreciationRunLineSeedData
    {
        public List<DepreciationRunLineViewModel> Seed(
            List<DepreciationRunViewModel> runs,
            List<FixedAssetViewModel.FixedAssetListDto> assets)
        {
            var lines = new List<DepreciationRunLineViewModel>();

            foreach (var run in runs)
            {
                // Only create lines for Generated / Posted runs
                if (run.RunStatus == DepreciationRunStatus.Draft)
                    continue;

                int lineNo = 10;

                // Take some assets (simulate eligible assets)
                var eligibleAssets = assets.Take(20).ToList();

                foreach (var asset in eligibleAssets)
                {
                    decimal plannedAmount = Math.Round(
                        Random.Shared.Next(1000, 5000) / 1.0m, 2);

                    decimal nbvBefore = Math.Round(
                        Random.Shared.Next(10000, 80000) / 1.0m, 2);

                    decimal nbvAfter = nbvBefore - plannedAmount;

                    var lineStatus = run.RunStatus == DepreciationRunStatus.Posted
                        ? DepreciationRunLineStatus.Posted
                        : DepreciationRunLineStatus.Generated;

                    lines.Add(new DepreciationRunLineViewModel
                    {
                        DepreciationRunLineId = Guid.NewGuid(),

                        TenantId = run.TenantId,
                        CompanyId = run.CompanyId,

                        DepreciationRunId = run.DepreciationRunId,

                        LineNumber = lineNo,

                        FixedAssetId = asset.FixedAssetId,

                        AssetNumberSnapshot = asset.AssetCode,
                        AssetNameSnapshot = asset.AssetName,

                        AssetCategoryIdSnapshot = asset.AssetCategoryId,
                        //CategoryCodeSnapshot = asset.AssetCategoryCode,

                        BranchIdSnapshot = asset.BranchId,

                        //AccountingPeriodId = run.AccountingPeriodId,

                        ScheduleId = Guid.NewGuid(),
                        ScheduleLineId = Guid.NewGuid(),
                        ScheduleVersionSnapshot = 1,

                        //DepreciationBaseCostSnapshot = asset.CapitalizedCost,

                        NBVBeforeAmountSnapshot = nbvBefore,

                        PlannedDepreciationAmount = plannedAmount,

                        ActualDepreciationAmount = plannedAmount,

                        NBVAfterAmountSnapshot = nbvAfter,

                        //ResidualValueAmountSnapshot = asset.ResidualValue,

                        //CurrencyId = asset.CurrencyId,

                        //DepreciationMethodIdSnapshot = asset.DepreciationMethodId,

                        //MethodTypeSnapshot = asset.MethodType,

                        //InputModeSnapshot = asset.InputMode,

                        //RatePercentSnapshot = asset.RatePercent,

                        //UsefulLifeMonthsSnapshot = asset.UsefulLifeMonths,

                        //StartConventionSnapshot = asset.StartConvention,

                        //DepreciationExpenseGLAccountIdSnapshot =
                        //    asset.DepreciationExpenseGLAccountId,

                        //AccumulatedDepreciationGLAccountIdSnapshot =
                        //    asset.AccumulatedDepreciationGLAccountId,

                        LineStatus = lineStatus,

                        IsLocked = run.RunStatus == DepreciationRunStatus.Posted,

                        PostedOn = run.RunStatus == DepreciationRunStatus.Posted
                            ? run.PostedOn
                            : null,

                        CreatedAt = DateTime.UtcNow
                    });

                    lineNo += 10;
                }
            }

            return lines;
        }
    }
}