using FinanceConnect.Client.Data;
using static FinanceConnect.Client.ViewModels.RevenueRecognitionViewModel;

namespace FinanceConnect.Client.Services
{
    public class RevenueRecognitionService
    {
        private List<RevenueRecognition> _items = new();

        public RevenueRecognitionService()
        {
            _items = RevenueRecognitionSeedData.GetAll();
        }

        // ── Query ──────────────────────────────────────────────────────────────

        public List<RevenueRecognition> GetAll()
            => _items.Where(x => !x.IsDeleted).ToList();

        public RevenueRecognition? GetById(Guid id)
            => _items.FirstOrDefault(x => x.RevenueRecognitionId == id && !x.IsDeleted);

        public Task<List<RevenueRecognition>> GetAllAsync()
            => Task.FromResult(GetAll());

        public Task<RevenueRecognition?> GetByIdAsync(Guid id)
            => Task.FromResult(GetById(id));

        // ── Create ─────────────────────────────────────────────────────────────

        public void Add(RevenueRecognition model)
        {
            if (_items.Any(x => !x.IsDeleted &&
                x.CompanyId == model.CompanyId &&
                string.Equals(x.RecognitionCode, model.RecognitionCode, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException(
                    $"Recognition Code '{model.RecognitionCode}' already exists for this company.");
            }

            if (model.IsLocked)
                throw new InvalidOperationException("Cannot create a recognition record in locked state.");

            if (model.TotalRecognizableAmount < 0)
                throw new InvalidOperationException("Total Recognizable Amount must be >= 0.");

            model.RevenueRecognitionId = Guid.NewGuid();
            model.CreatedAt = DateTime.UtcNow;
            model.IsDeleted = false;
            _items.Add(model);
        }

        public Task CreateAsync(RevenueRecognition model)
        {
            Add(model);
            return Task.CompletedTask;
        }

        // ── Update ─────────────────────────────────────────────────────────────

        public void Update(RevenueRecognition model)
        {
            var existing = GetById(model.RevenueRecognitionId);
            if (existing is null) return;

            if (existing.IsLocked)
                throw new InvalidOperationException("Locked recognition record cannot be edited.");

            if (existing.RecognitionStatus == RecognitionStatusEnum.Closed ||
                existing.RecognitionStatus == RecognitionStatusEnum.FullyRecognized)
                throw new InvalidOperationException(
                    "Closed or fully recognised record cannot be materially changed.");

            existing.RecognitionName               = model.RecognitionName;
            existing.Description                   = model.Description;
            existing.RecognitionStatus             = model.RecognitionStatus;
            existing.RevenueId                     = model.RevenueId;
            existing.RevenueCodeSnapshot           = model.RevenueCodeSnapshot;
            existing.RevenueNameSnapshot           = model.RevenueNameSnapshot;
            existing.CustomerId                    = model.CustomerId;
            existing.CustomerNameSnapshot          = model.CustomerNameSnapshot;
            existing.SourceDocumentTypeSnapshot    = model.SourceDocumentTypeSnapshot;
            existing.SourceDocumentNumberSnapshot  = model.SourceDocumentNumberSnapshot;
            existing.RevenueTypeSnapshot           = model.RevenueTypeSnapshot;
            existing.RevenueNatureSnapshot         = model.RevenueNatureSnapshot;
            existing.SourceGrossRevenueAmount      = model.SourceGrossRevenueAmount;
            existing.CurrencyId                    = model.CurrencyId;
            existing.RecognitionMethod             = model.RecognitionMethod;
            existing.RecognitionBasis              = model.RecognitionBasis;
            existing.RecognitionStartDate          = model.RecognitionStartDate;
            existing.RecognitionEndDate            = model.RecognitionEndDate;
            existing.RecognitionFrequency          = model.RecognitionFrequency;
            existing.ScheduleTemplateCode          = model.ScheduleTemplateCode;
            existing.MilestoneTriggerRequired      = model.MilestoneTriggerRequired;
            existing.ManualApprovalRequiredFlag    = model.ManualApprovalRequiredFlag;
            existing.DeferredRevenueId             = model.DeferredRevenueId;
            existing.DeferredRevenueReference      = model.DeferredRevenueReference;
            existing.IsScheduleGenerated           = model.IsScheduleGenerated;
            existing.TotalRecognizableAmount       = model.TotalRecognizableAmount;
            existing.RecognizedAmountToDate        = model.RecognizedAmountToDate;
            existing.CurrentPeriodRecognitionAmount = model.CurrentPeriodRecognitionAmount;
            existing.AdjustmentAmount              = model.AdjustmentAmount;
            existing.RoundingDifferenceAmount      = model.RoundingDifferenceAmount;
            existing.FiscalYearId                  = model.FiscalYearId;
            existing.CurrentAccountingPeriodId     = model.CurrentAccountingPeriodId;
            existing.CurrentAccountingPeriodReference = model.CurrentAccountingPeriodReference;
            existing.RecognitionPostingDate        = model.RecognitionPostingDate;
            existing.LastRecognitionRunDate        = model.LastRecognitionRunDate;
            existing.NextRecognitionDueDate        = model.NextRecognitionDueDate;
            existing.RecognizedPeriodsCount        = model.RecognizedPeriodsCount;
            existing.PendingPeriodsCount           = model.PendingPeriodsCount;
            existing.PreparedByUserId              = model.PreparedByUserId;
            existing.ReviewedByUserId              = model.ReviewedByUserId;
            existing.ApprovedByUserId              = model.ApprovedByUserId;
            existing.PreparedOn                    = model.PreparedOn;
            existing.ReviewedOn                    = model.ReviewedOn;
            existing.ApprovedOn                    = model.ApprovedOn;
            existing.CancellationReason            = model.CancellationReason;
            existing.RecognitionAssumptionText     = model.RecognitionAssumptionText;
            existing.Notes                         = model.Notes;
            existing.AttachmentCount               = model.AttachmentCount;
            existing.ScheduleLines                 = model.ScheduleLines;
            existing.UpdatedAt                     = DateTime.UtcNow;
        }

        public Task UpdateAsync(RevenueRecognition model)
        {
            Update(model);
            return Task.CompletedTask;
        }

        // ── Delete (soft) ──────────────────────────────────────────────────────

        public void Delete(Guid id)
        {
            var item = GetById(id);
            if (item is null) return;

            if (item.IsLocked)
                throw new InvalidOperationException("Locked recognition cannot be deleted.");

            if (item.RecognitionStatus == RecognitionStatusEnum.FullyRecognized ||
                item.RecognitionStatus == RecognitionStatusEnum.Closed)
                throw new InvalidOperationException(
                    "Fully recognised or closed record cannot be deleted.");

            item.IsDeleted = true;
            item.UpdatedAt = DateTime.UtcNow;
        }

        public Task DeleteAsync(Guid id)
        {
            Delete(id);
            return Task.CompletedTask;
        }

        // ── Lock / Unlock ──────────────────────────────────────────────────────

        public void Lock(Guid id)
        {
            var item = GetById(id);
            if (item is null) return;
            item.IsLocked  = true;
            item.LockedOn  = DateTime.UtcNow;
            item.UpdatedAt = DateTime.UtcNow;
        }

        public void Unlock(Guid id)
        {
            var item = GetById(id);
            if (item is null) return;
            item.IsLocked  = false;
            item.LockedOn  = null;
            item.UpdatedAt = DateTime.UtcNow;
        }

        // ── Reset ──────────────────────────────────────────────────────────────

        public void ResetToSeed()
        {
            _items = RevenueRecognitionSeedData.GetAll();
        }

        // ── Auto-generate Code ─────────────────────────────────────────────────

        public string GenerateCode(Guid companyId)
        {
            var year  = DateTime.Today.Year;
            var count = _items.Count(x => x.CompanyId == companyId && !x.IsDeleted) + 1;
            return $"REVREC-{year}-{count:D3}";
        }
    }
}
