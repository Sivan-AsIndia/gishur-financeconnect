using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    public class DepreciationRunLineService
    {
        private readonly DepreciationRunService _runService;
        private readonly DepreciationRunLineSeedData _seed;

        private static List<DepreciationRunLineViewModel> _store = new();
        private static List<DepreciationRunViewModel> _runs = new();
        private readonly List<FixedAssetViewModel.FixedAssetListDto> _assets = new();

        public DepreciationRunLineService(
            DepreciationRunService runService,
            FixedAssetService assetService)
        {
            _runService = runService;
            _seed = new DepreciationRunLineSeedData();

            if (!_store.Any())
            {
                _runs = runService.GetAll();
                _assets = assetService.GetAll();

                _store = _seed.Seed(_runs, _assets);
            }
        }

        public List<DepreciationRunLineViewModel> GetAll()
        {
            return _store
                .OrderBy(x => x.LineNumber)
                .ToList();
        }

        public List<DepreciationRunLineViewModel> GetByRunId(Guid runId)
        {
            return _store
                .Where(x => x.DepreciationRunId == runId)
                .OrderBy(x => x.LineNumber)
                .ToList();
        }

        public Task<DepreciationRunLineViewModel?> GetByIdAsync(Guid id)
        {
            var line = _store.FirstOrDefault(x => x.DepreciationRunLineId == id);
            return Task.FromResult(line);
        }

        // EXCLUDE LINE
        public Task ExcludeAsync(Guid lineId, string reason)
        {
            var line = _store.FirstOrDefault(x => x.DepreciationRunLineId == lineId);

            if (line == null)
                throw new Exception("Line not found.");

            if (line.LineStatus == DepreciationRunLineStatus.Posted)
                throw new Exception("Posted line cannot be excluded.");

            line.LineStatus = DepreciationRunLineStatus.Excluded;
            //line.ExclusionReasonCode = ExclusionReasonCode.ManualExclusion;
            line.ExclusionReasonText = reason;
            line.UpdatedAt = DateTime.UtcNow;

            return Task.CompletedTask;
        }

        // INCLUDE LINE (UNDO EXCLUSION)
        public Task IncludeAsync(Guid lineId)
        {
            var line = _store.FirstOrDefault(x => x.DepreciationRunLineId == lineId);

            if (line == null)
                throw new Exception("Line not found.");

            if (line.LineStatus != DepreciationRunLineStatus.Excluded)
                throw new Exception("Only excluded lines can be included.");

            line.LineStatus = DepreciationRunLineStatus.Generated;
            line.ExclusionReasonCode = null;
            line.ExclusionReasonText = null;
            line.UpdatedAt = DateTime.UtcNow;

            return Task.CompletedTask;
        }

        // MANUAL ADJUSTMENT (Controller Only)
        public Task AdjustAmountAsync(Guid lineId, decimal newAmount, string reason)
        {
            var line = _store.FirstOrDefault(x => x.DepreciationRunLineId == lineId);

            if (line == null)
                throw new Exception("Line not found.");

            if (line.LineStatus == DepreciationRunLineStatus.Posted)
                throw new Exception("Posted line cannot be adjusted.");

            if (newAmount < 0)
                throw new Exception("Depreciation amount cannot be negative.");

            line.ActualDepreciationAmount = newAmount;

            line.NBVAfterAmountSnapshot =
                line.NBVBeforeAmountSnapshot - newAmount;

            line.IsManuallyAdjusted = true;
            line.ManualAdjustmentReason = reason;

            line.UpdatedAt = DateTime.UtcNow;

            return Task.CompletedTask;
        }

        // MARK POSTED (called when run is posted)
        public void MarkPosted(Guid runId, Guid journalEntryId)
        {
            var lines = _store
                .Where(x => x.DepreciationRunId == runId &&
                            x.LineStatus == DepreciationRunLineStatus.Generated)
                .ToList();

            foreach (var line in lines)
            {
                line.LineStatus = DepreciationRunLineStatus.Posted;
                line.JournalEntryId = journalEntryId;
                line.PostedOn = DateTime.UtcNow;
                line.IsLocked = true;
            }
        }

        public void Generate(Guid id)
        {
            var run = _runService.GetById(id);

            if (run == null)
                throw new InvalidOperationException("Run not found.");

            if (run.RunStatus != DepreciationRunStatus.Draft)
                throw new InvalidOperationException("Only Draft runs can be generated.");


            if (!_assets.Any())
                throw new InvalidOperationException("No eligible assets found.");

            // Generate lines
            var lines = GenerateLines(run, _assets);

            run.GeneratedLineCount = lines.Count;
            run.EligibleAssetsCount = lines.Count;
            run.ExcludedAssetCount = lines.Count(x => x.LineStatus == DepreciationRunLineStatus.Excluded);

            run.TotalDepreciationAmount = lines
                .Where(x => x.LineStatus == DepreciationRunLineStatus.Generated)
                .Sum(x => x.ActualDepreciationAmount);

            run.TotalExpenseAmount = run.TotalDepreciationAmount;
            run.TotalAccumDepAmount = run.TotalDepreciationAmount;

            run.GeneratedOn = DateTime.UtcNow;
            run.GeneratedBy = "system";

            run.RunStatus = DepreciationRunStatus.Generated;
        }

        public List<DepreciationRunLineViewModel> GenerateLines(
    DepreciationRunViewModel run,
    List<FixedAssetViewModel.FixedAssetListDto> assets)
        {
            var result = new List<DepreciationRunLineViewModel>();

            int lineNo = 10;

            foreach (var asset in assets)
            {
                decimal planned = Math.Round(
     (decimal)(Random.Shared.NextDouble() * 3000 + 2000), 2);

                decimal? nbvBefore = asset.NetBookValue;

                decimal? nbvAfter = nbvBefore - planned;

                var line = new DepreciationRunLineViewModel
                {
                    DepreciationRunLineId = Guid.NewGuid(),

                    DepreciationRunId = run.DepreciationRunId,

                    LineNumber = lineNo,

                    FixedAssetId = asset.FixedAssetId,

                    AssetNumberSnapshot = asset.AssetCode,
                    AssetNameSnapshot = asset.AssetName,

                    CategoryCodeSnapshot = asset.CategoryName,

                    PlannedDepreciationAmount = planned,
                    ActualDepreciationAmount = planned,

                    NBVBeforeAmountSnapshot = nbvBefore,
                    NBVAfterAmountSnapshot = nbvAfter,

                    LineStatus = DepreciationRunLineStatus.Generated,

                    CreatedAt = DateTime.UtcNow
                };

                result.Add(line);

                _store.Add(line);

                lineNo += 10;
            }

            return result;
        }

        // RESET SEED
        public void ResetToSeed(
            DepreciationRunService runService,
            FixedAssetService assetService)
        {
            var runs = runService.GetAll();
            var assets = assetService.GetAll();

            _store = _seed.Seed(runs, assets);
        }
    }
}