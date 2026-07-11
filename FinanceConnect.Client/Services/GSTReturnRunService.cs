using FinanceConnect.Client.Data;
using static FinanceConnect.Client.ViewModels.GSTReturnRunViewModel;

namespace FinanceConnect.Client.Services
{
    public class GSTReturnRunService
    {
        private readonly List<GSTReturnRunModel> _runs;

        public GSTReturnRunService()
        {
            _runs = GSTReturnRunSeedData.Get();
        }

        // ── List / Query ──
        public List<GSTReturnRunModel> GetList()
            => _runs.Where(x => !x.IsDeleted)
                    .OrderByDescending(x => x.PeriodStartDate)
                    .ToList();

        public List<GSTReturnRunModel> GetByStatus(string status)
            => _runs.Where(x => !x.IsDeleted && x.ReturnRunStatus == status)
                    .OrderByDescending(x => x.PeriodStartDate).ToList();

        public List<GSTReturnRunModel> GetByPeriod(string periodKey)
            => _runs.Where(x => !x.IsDeleted && x.ReturnPeriodKey == periodKey)
                    .OrderByDescending(x => x.PeriodStartDate).ToList();

        public GSTReturnRunModel? GetById(Guid id)
            => _runs.FirstOrDefault(x => x.Id == id && !x.IsDeleted);

        // ── Create ──
        public void Create(GSTReturnRunModel model)
        {
            if (string.IsNullOrWhiteSpace(model.ReturnPeriodKey))
                throw new Exception("Return Period is required.");
            if (model.PeriodEndDate < model.PeriodStartDate)
                throw new Exception("Period End Date must be >= Period Start Date.");

            model.ReturnRunNumber = $"GSTRUN-{DateTime.Now.Year}-{(_runs.Count + 1):00000}";
            model.ReturnRunStatus = "Draft";
            model.FilingStatus = "NotFiled";
            model.TaxLedgerReconciliationStatus = "NotRun";
            model.IsLocked = false;
            model.CreatedAt = DateTime.Now;
            _runs.Add(model);
        }

        // ── Update Draft ──
        public void UpdateDraft(GSTReturnRunModel model)
        {
            var existing = GetById(model.Id);
            if (existing == null) throw new Exception("Return Run not found.");
            if (existing.ReturnRunStatus != "Draft" && existing.ReturnRunStatus != "Reopened")
                throw new Exception("Only Draft or Reopened runs can be edited.");

            existing.ReturnType = model.ReturnType;
            existing.ReturnPeriodKey = model.ReturnPeriodKey;
            existing.PeriodStartDate = model.PeriodStartDate;
            existing.PeriodEndDate = model.PeriodEndDate;
            existing.SelectionMode = model.SelectionMode;
            existing.IncludeOutwardSupplies = model.IncludeOutwardSupplies;
            existing.IncludeInwardSupplies = model.IncludeInwardSupplies;
            existing.IncludeRCMTransactions = model.IncludeRCMTransactions;
            existing.IncludeCreditDebitNotes = model.IncludeCreditDebitNotes;
            existing.IncludeExemptNilNonGST = model.IncludeExemptNilNonGST;
            existing.IncludeOnlyPostedTransactions = model.IncludeOnlyPostedTransactions;
            existing.ReviewerNotes = model.ReviewerNotes;
            existing.FilingNotes = model.FilingNotes;
            existing.UpdatedAt = DateTime.Now;
        }

        // ── Workflow ──
        public void Generate(Guid id)
        {
            var r = GetById(id) ?? throw new Exception("Return Run not found.");
            if (r.ReturnRunStatus != "Draft" && r.ReturnRunStatus != "Reopened")
                throw new Exception("Only Draft or Reopened runs can be generated.");

            // Simulate generation
            var rng = new Random(id.GetHashCode());
            r.EligibleTransactionCount = rng.Next(3000, 5000);
            var excluded = rng.Next(10, 80);
            r.ExcludedTransactionCount = excluded;
            r.IncludedTransactionCount = r.EligibleTransactionCount - excluded;
            r.IncludedLineCount = r.IncludedTransactionCount * 3;
            r.WarningExceptionCount = excluded;
            r.BlockingExceptionCount = rng.Next(0, 5);
            r.ExceptionCount = r.WarningExceptionCount + r.BlockingExceptionCount;
            r.HasBlockingExceptions = r.BlockingExceptionCount > 0;

            r.OutwardTaxableValueTotal = Math.Round((decimal)(rng.NextDouble() * 30000000 + 10000000), 2);
            r.OutwardCGSTTotal = Math.Round(r.OutwardTaxableValueTotal * 0.09m, 2);
            r.OutwardSGSTTotal = r.OutwardCGSTTotal;
            r.OutwardIGSTTotal = Math.Round(r.OutwardTaxableValueTotal * 0.05m, 2);
            r.InputEligibleITCTotal = Math.Round(r.OutwardTaxableValueTotal * 0.16m, 2);
            r.NetTaxLiabilityTotal = r.OutwardCGSTTotal + r.OutwardSGSTTotal + r.OutwardIGSTTotal - r.InputEligibleITCTotal;

            r.GenerationDate = DateTime.Now;
            r.ReturnRunStatus = "Generated";
            r.TaxLedgerReconciliationStatus = r.HasBlockingExceptions ? "Mismatch" : "Matched";
            r.UpdatedAt = DateTime.Now;
        }

        public void Review(Guid id)
        {
            var r = GetById(id) ?? throw new Exception("Return Run not found.");
            if (r.ReturnRunStatus != "Generated") throw new Exception("Only Generated runs can be reviewed.");
            r.ReturnRunStatus = "Reviewed";
            r.UpdatedAt = DateTime.Now;
        }

        public void Approve(Guid id)
        {
            var r = GetById(id) ?? throw new Exception("Return Run not found.");
            if (r.ReturnRunStatus != "Reviewed") throw new Exception("Only Reviewed runs can be approved.");
            r.ReturnRunStatus = "Approved";
            r.ApprovedOn = DateTime.Now;
            r.ApprovedBy = "controller@acme.com";
            r.UpdatedAt = DateTime.Now;
        }

        public void Finalize(Guid id)
        {
            var r = GetById(id) ?? throw new Exception("Return Run not found.");
            if (r.ReturnRunStatus != "Approved") throw new Exception("Only Approved runs can be finalized.");
            if (r.HasBlockingExceptions) throw new Exception("Cannot finalize with blocking exceptions.");

            r.ReturnRunStatus = "Finalized";
            r.FinalizedOn = DateTime.Now;
            r.FinalizedBy = "controller@acme.com";
            r.IsLocked = true;
            r.LockReason = "Finalized for filing";
            r.IncludedHashSignature = $"SHA256:{Guid.NewGuid():N}".Substring(0, 40);
            r.FilingStatus = "Prepared";
            r.UpdatedAt = DateTime.Now;
        }

        public void MarkFiled(Guid id, DateTime filedDate, string? ackNumber)
        {
            var r = GetById(id) ?? throw new Exception("Return Run not found.");
            if (r.ReturnRunStatus != "Finalized" && r.ReturnRunStatus != "Filed")
                throw new Exception("Only Finalized runs can be marked as filed.");

            r.ReturnRunStatus = "Filed";
            r.FiledDate = filedDate;
            r.FiledBy = "taxteam@acme.com";
            r.GovernmentAcknowledgementNumber = ackNumber;
            r.FilingStatus = string.IsNullOrWhiteSpace(ackNumber) ? "Filed" : "Acknowledged";
            r.IsExportGenerated = true;
            r.ExportGeneratedOn = DateTime.Now;
            r.UpdatedAt = DateTime.Now;
        }

        public void Close(Guid id)
        {
            var r = GetById(id) ?? throw new Exception("Return Run not found.");
            if (r.ReturnRunStatus != "Filed") throw new Exception("Only Filed runs can be closed.");
            r.ReturnRunStatus = "Closed";
            r.UpdatedAt = DateTime.Now;
        }

        public void Reopen(Guid id, string reason)
        {
            var r = GetById(id) ?? throw new Exception("Return Run not found.");
            if (r.ReturnRunStatus != "Finalized" && r.ReturnRunStatus != "Closed")
                throw new Exception("Only Finalized or Closed runs can be reopened.");
            if (string.IsNullOrWhiteSpace(reason)) throw new Exception("Reopen reason is required.");

            r.ReturnRunStatus = "Reopened";
            r.IsLocked = false;
            r.LockReason = null;
            r.ReopenedOn = DateTime.Now;
            r.ReopenedBy = "controller@acme.com";
            r.ReopenReason = reason;
            r.UpdatedAt = DateTime.Now;
        }

        public void Delete(Guid id)
        {
            var r = GetById(id) ?? throw new Exception("Return Run not found.");
            if (r.ReturnRunStatus != "Draft") throw new Exception("Only Draft can be deleted.");
            r.IsDeleted = true;
        }
    }
}
