using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Xml.Linq;
using static FinanceConnect.Client.Data.MasterDataIds;

namespace FinanceConnect.Client.Services
{
    public class DepreciationRunService
    {
        private readonly MasterDataService _masterDataService;
        private readonly FiscalYearService _fiscalYearService;
        private readonly AccountingPeriodService _accountingPeriodService;

        private readonly DepreciationRunSeedData _seed;
        private readonly List<DepreciationRunViewModel> _seedRuns = new();
        private readonly List<CompanyModel> _companies = new();
        private readonly List<FiscalYearModel> _fy = new();
        private readonly List<AccountingPeriodModel> _periods = new();
        private static List<DepreciationRunViewModel> _runs = new();
        private readonly List<FixedAssetViewModel.FixedAssetListDto> _assets = new();

        public DepreciationRunService(MasterDataService masterDataService, FiscalYearService fiscalYearService, AccountingPeriodService accountingPeriodService,
            FixedAssetService assetService)
        {
            _masterDataService = masterDataService;
            _fiscalYearService = fiscalYearService;
            _accountingPeriodService = accountingPeriodService;
            _seed = new DepreciationRunSeedData();
            _companies = _masterDataService
                .GetAllCompanies()
                .Where(c => c.Status == "Active")
                .ToList();
            _fy = _fiscalYearService.GetAll();
            _periods = _accountingPeriodService.GetAll();
            _assets = assetService.GetAll();
            _seedRuns = SeedData();

            ResetToSeed();
        }

        private static List<T> CloneList<T>(IEnumerable<T> source) where T : class
        {
            var json = System.Text.Json.JsonSerializer.Serialize(source);
            return System.Text.Json.JsonSerializer.Deserialize<List<T>>(json) ?? new();
        }

        public void ResetToSeed()
        {
            _runs = CloneList(_seedRuns);
        }

        // ==========================================
        // GET
        // ==========================================

        public List<DepreciationRunViewModel> GetAll()
        {
            return _runs
                .OrderByDescending(x => x.CreatedAt)
                .ToList();
        }

        public DepreciationRunViewModel? GetById(Guid id)
        {
            return _runs.FirstOrDefault(x => x.DepreciationRunId == id);
        }

        // ==========================================
        // CREATE
        // ==========================================

        public void Create(DepreciationRunViewModel model)
        {
            ValidateCreate(model);

            model.DepreciationRunId = Guid.NewGuid();
            model.CreatedAt = DateTime.UtcNow;
            model.RunStatus = DepreciationRunStatus.Draft;

            model.RunNumber = GenerateRunNumber();

            _runs.Add(model);
        }

        public void Update(DepreciationRunViewModel model)
        {
            var existing = GetById(model.DepreciationRunId);

            if (existing == null)
                throw new InvalidOperationException("Depreciation run not found.");

            if (existing.RunStatus != DepreciationRunStatus.Draft)
                throw new InvalidOperationException("Only Draft runs can be edited.");

            // Update editable fields
            existing.CompanyId = model.CompanyId;
            existing.BranchId = model.BranchId;
            existing.AccountingPeriodId = model.AccountingPeriodId;
            existing.AccountingPeriodName = model.AccountingPeriodName;

            existing.RunType = model.RunType;
            existing.AsOfDate = model.AsOfDate;

            existing.IncludeSuspendedAssets = model.IncludeSuspendedAssets;
            existing.IncludeZeroDepreciationAssets = model.IncludeZeroDepreciationAssets;

            existing.AssetCategoryFilterId = model.AssetCategoryFilterId;

            existing.SelectionMode = model.SelectionMode;
            existing.ScheduleVersionPolicy = model.ScheduleVersionPolicy;

            existing.RunNotes = model.RunNotes;

            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = "system";
        }

        public static string GenerateRunNumber()
        {
            int next = _runs.Count + 1;
            return $"DEP-{next:000000}";
        }

        private void ValidateCreate(DepreciationRunViewModel model)
        {
            if (model.CompanyId == Guid.Empty)
                throw new InvalidOperationException("Company is required.");

            if (!model.AccountingPeriodId.HasValue)
                throw new InvalidOperationException("Accounting period is required.");

            bool duplicate = _runs.Any(x =>
                x.CompanyId == model.CompanyId &&
                x.BranchId == model.BranchId &&
                x.AccountingPeriodId == model.AccountingPeriodId &&
                x.RunStatus == DepreciationRunStatus.Finalized);

            if (duplicate)
                throw new InvalidOperationException("A finalized depreciation run already exists for this period.");
        }

        // ==========================================
        // GENERATE
        // ==========================================

        //public void Generate(Guid id)
        //{
        //    var run = GetById(id);

        //    if (run == null)
        //        throw new InvalidOperationException("Run not found.");

        //    if (run.RunStatus != DepreciationRunStatus.Draft)
        //        throw new InvalidOperationException("Only Draft runs can be generated.");


        //    if (!_assets.Any())
        //        throw new InvalidOperationException("No eligible assets found.");

        //    // Generate lines
        //    var lines = _runLineService.GenerateLines(run, _assets);

        //    run.GeneratedLineCount = lines.Count;
        //    run.EligibleAssetsCount = lines.Count;
        //    run.ExcludedAssetCount = lines.Count(x => x.LineStatus == DepreciationRunLineStatus.Excluded);

        //    run.TotalDepreciationAmount = lines
        //        .Where(x => x.LineStatus == DepreciationRunLineStatus.Generated)
        //        .Sum(x => x.ActualDepreciationAmount);

        //    run.TotalExpenseAmount = run.TotalDepreciationAmount;
        //    run.TotalAccumDepAmount = run.TotalDepreciationAmount;

        //    run.GeneratedOn = DateTime.UtcNow;
        //    run.GeneratedBy = "system";

        //    run.RunStatus = DepreciationRunStatus.Generated;
        //}

        // ==========================================
        // SUBMIT
        // ==========================================

        public void Submit(Guid id)
        {
            var run = GetById(id);

            if (run == null)
                throw new InvalidOperationException("Run not found.");

            if (run.RunStatus != DepreciationRunStatus.Generated)
                throw new InvalidOperationException("Only Generated runs can be submitted.");

            run.RunStatus = DepreciationRunStatus.Submitted;
        }

        // ==========================================
        // APPROVE
        // ==========================================

        public void Approve(Guid id)
        {
            var run = GetById(id);

            if (run == null)
                throw new InvalidOperationException("Run not found.");

            if (run.RunStatus != DepreciationRunStatus.Submitted)
                throw new InvalidOperationException("Only Submitted runs can be approved.");

            run.RunStatus = DepreciationRunStatus.Approved;
        }

        // ==========================================
        // POST
        // ==========================================

        public void Post(Guid id)
        {
            var run = GetById(id);

            if (run == null)
                throw new InvalidOperationException("Run not found.");

            if (run.RunStatus != DepreciationRunStatus.Approved)
                throw new InvalidOperationException("Only Approved runs can be posted.");

            ValidateBeforePost(run);

            run.RunStatus = DepreciationRunStatus.Posted;
            run.PostedOn = DateTime.UtcNow;
            run.PostedBy = "system";

            run.JournalEntryId = Guid.NewGuid(); // simulated GL journal
        }

        private void ValidateBeforePost(DepreciationRunViewModel run)
        {
            if (run.TotalExpenseAmount != run.TotalAccumDepAmount)
                throw new InvalidOperationException("Journal totals do not balance.");

            if (run.GeneratedLineCount == 0)
                throw new InvalidOperationException("No depreciation lines generated.");
        }

        // ==========================================
        // FINALIZE
        // ==========================================

        public void Finalize(Guid id)
        {
            var run = GetById(id);

            if (run == null)
                throw new InvalidOperationException("Run not found.");

            if (run.RunStatus != DepreciationRunStatus.Posted)
                throw new InvalidOperationException("Only Posted runs can be finalized.");

            run.RunStatus = DepreciationRunStatus.Finalized;
            run.FinalizedOn = DateTime.UtcNow;
            run.FinalizedBy = "system";
        }

        // ==========================================
        // REVERSE
        // ==========================================

        public void Reverse(Guid id)
        {
            var run = GetById(id);

            if (run == null)
                throw new InvalidOperationException("Run not found.");

            if (run.RunStatus != DepreciationRunStatus.Posted &&
                run.RunStatus != DepreciationRunStatus.Finalized)
                throw new InvalidOperationException("Only Posted or Finalized runs can be reversed.");

            run.RunStatus = DepreciationRunStatus.Reversed;

            run.ReversalJournalEntryId = Guid.NewGuid();
        }

        // ==========================================
        // DELETE
        // ==========================================

        public void Delete(Guid id)
        {
            var run = GetById(id);

            if (run == null) return;

            if (run.RunStatus != DepreciationRunStatus.Draft)
                throw new InvalidOperationException("Only Draft runs can be deleted.");

            _runs.Remove(run);
        }

        // ==========================================
        // SEED DATA
        // ==========================================

        private List<DepreciationRunViewModel> SeedData()
        {
            var seededRuns = _seed.Seed(_companies,_fy, _periods);
            return seededRuns;
        }
    }
}