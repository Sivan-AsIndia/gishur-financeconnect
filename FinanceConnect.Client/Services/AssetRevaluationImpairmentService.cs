using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    public class AssetRevaluationImpairmentService
    {
        private readonly List<AssetRevaluationImpairmentViewModel.AssetRevaluationImpairment> _events;

        public AssetRevaluationImpairmentService()
        {
            _events = AssetRevaluationImpairmentSeedData.GetAll();
        }

        /* ================= READ ================= */

        public List<AssetRevaluationImpairmentViewModel.AssetRevaluationImpairment> GetAll()
            => _events;

        public AssetRevaluationImpairmentViewModel.AssetRevaluationImpairment? GetById(Guid id)
            => _events.FirstOrDefault(x => x.AssetRevaluationImpairmentId == id);

        public Task<AssetRevaluationImpairmentViewModel.AssetRevaluationImpairment?> GetByIdAsync(Guid id)
            => Task.FromResult(GetById(id));

        /* ================= CREATE ================= */

        public void Add(AssetRevaluationImpairmentViewModel.AssetRevaluationImpairment model)
        {
            if (model.FixedAssetId == null || model.FixedAssetId == Guid.Empty)
                throw new InvalidOperationException("Fixed Asset is required.");

            if (model.EventType == null)
                throw new InvalidOperationException("Event Type is required.");

            if (model.EffectiveDate == null)
                throw new InvalidOperationException("Effective Date is required.");

            if (model.CalculationMode == null)
                throw new InvalidOperationException("Calculation Mode is required.");

            if (model.CalculationMode == AssetRevaluationImpairmentViewModel.CalculationModeEnum.AdjustByDelta
                && (model.DeltaAmount == null || model.DeltaAmount <= 0))
                throw new InvalidOperationException("Delta Amount must be > 0 for AdjustByDelta mode.");

            if (model.CalculationMode == AssetRevaluationImpairmentViewModel.CalculationModeEnum.RevalueToAmount
                && (model.TargetCarryingAmount == null || model.TargetCarryingAmount < 0))
                throw new InvalidOperationException("Target Carrying Amount must be >= 0 for RevalueToAmount mode.");

            if (model.CarryingValueAfter < 0)
                throw new InvalidOperationException("Carrying Value After cannot be negative.");

            model.AssetRevaluationImpairmentId = Guid.NewGuid();
            model.EventNumber = $"FAREV-{(_events.Count + 1):D6}";
            model.EventStatus = AssetRevaluationImpairmentViewModel.EventStatusEnum.Draft;
            model.CreatedAt = DateTime.UtcNow;
            model.IsDeleted = false;

            _events.Add(model);
        }

        public Task CreateAsync(AssetRevaluationImpairmentViewModel.AssetRevaluationImpairment model)
        {
            Add(model);
            return Task.CompletedTask;
        }

        /* ================= UPDATE ================= */

        public void Update(AssetRevaluationImpairmentViewModel.AssetRevaluationImpairment model)
        {
            var existing = GetById(model.AssetRevaluationImpairmentId);
            if (existing == null) return;

            if (existing.EventStatus == AssetRevaluationImpairmentViewModel.EventStatusEnum.Posted)
                throw new InvalidOperationException("Posted event cannot be edited. Use reversal.");

            existing.FixedAssetId = model.FixedAssetId;
            existing.AssetNumberSnapshot = model.AssetNumberSnapshot;
            existing.AssetNameSnapshot = model.AssetNameSnapshot;
            existing.AssetCategoryIdSnapshot = model.AssetCategoryIdSnapshot;
            existing.InServiceDateSnapshot = model.InServiceDateSnapshot;
            existing.AssetStatusSnapshot = model.AssetStatusSnapshot;
            existing.EventType = model.EventType;
            existing.EffectiveDate = model.EffectiveDate;
            existing.ReasonCode = model.ReasonCode;
            existing.Narration = model.Narration;
            existing.CalculationMode = model.CalculationMode;
            existing.DeltaAmount = model.DeltaAmount;
            existing.TargetCarryingAmount = model.TargetCarryingAmount;
            existing.ValuationBasis = model.ValuationBasis;
            existing.ValuerName = model.ValuerName;
            existing.ValuationReportReference = model.ValuationReportReference;
            existing.ValuationReportDate = model.ValuationReportDate;
            existing.AccumDepTreatmentMode = model.AccumDepTreatmentMode;
            existing.AllowAccumDepTreatmentOverride = model.AllowAccumDepTreatmentOverride;
            existing.AccumDepTreatmentReason = model.AccumDepTreatmentReason;
            existing.GrossCostBefore = model.GrossCostBefore;
            existing.AccumDepBefore = model.AccumDepBefore;
            existing.CarryingValueBefore = model.CarryingValueBefore;
            existing.ResidualValueAmountBefore = model.ResidualValueAmountBefore;
            existing.GrossCostAfter = model.GrossCostAfter;
            existing.AccumDepAfter = model.AccumDepAfter;
            existing.CarryingValueAfter = model.CarryingValueAfter;
            existing.IsGainOrLossToPAndLFlag = model.IsGainOrLossToPAndLFlag;
            existing.RevaluationDecreaseHandlingMode = model.RevaluationDecreaseHandlingMode;
            existing.RegenerationMode = model.RegenerationMode;
            existing.NewDepreciableBaseAmountAfter = model.NewDepreciableBaseAmountAfter;
            existing.NewUsefulLifeMonthsOverride = model.NewUsefulLifeMonthsOverride;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        public Task UpdateAsync(AssetRevaluationImpairmentViewModel.AssetRevaluationImpairment model)
        {
            Update(model);
            return Task.CompletedTask;
        }

        /* ================= WORKFLOW ================= */

        public Task SubmitAsync(Guid id) => ChangeStatusAsync(id, AssetRevaluationImpairmentViewModel.EventStatusEnum.Submitted);
        public Task ApproveAsync(Guid id) => ChangeStatusAsync(id, AssetRevaluationImpairmentViewModel.EventStatusEnum.Approved);
        public Task RejectAsync(Guid id) => ChangeStatusAsync(id, AssetRevaluationImpairmentViewModel.EventStatusEnum.Rejected);
        public Task CancelAsync(Guid id) => ChangeStatusAsync(id, AssetRevaluationImpairmentViewModel.EventStatusEnum.Cancelled);

        public Task PostAsync(Guid id)
        {
            var evt = GetById(id);
            if (evt == null) return Task.CompletedTask;
            if (evt.EventStatus == AssetRevaluationImpairmentViewModel.EventStatusEnum.Posted)
                throw new InvalidOperationException("Event is already posted.");
            evt.EventStatus = AssetRevaluationImpairmentViewModel.EventStatusEnum.Posted;
            evt.PostingDate = DateTime.UtcNow;
            evt.PostedOn = DateTime.UtcNow;
            evt.JournalEntryId = Guid.NewGuid();
            evt.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        public Task ReverseAsync(Guid id, string reason)
        {
            var evt = GetById(id);
            if (evt == null) return Task.CompletedTask;
            if (string.IsNullOrWhiteSpace(reason))
                throw new InvalidOperationException("Reversal reason is required.");
            evt.EventStatus = AssetRevaluationImpairmentViewModel.EventStatusEnum.Reversed;
            evt.ReversalReason = reason;
            evt.ReversalJournalEntryId = Guid.NewGuid();
            evt.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        private Task ChangeStatusAsync(Guid id, AssetRevaluationImpairmentViewModel.EventStatusEnum status)
        {
            var evt = GetById(id);
            if (evt == null) return Task.CompletedTask;
            evt.EventStatus = status;
            evt.UpdatedAt = DateTime.UtcNow;
            if (status == AssetRevaluationImpairmentViewModel.EventStatusEnum.Submitted) evt.SubmittedOn = DateTime.UtcNow;
            if (status == AssetRevaluationImpairmentViewModel.EventStatusEnum.Approved) evt.ApprovedOn = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        /* ================= DELETE (Soft) ================= */

        public Task DeleteAsync(Guid id)
        {
            var evt = GetById(id);
            if (evt == null) return Task.CompletedTask;
            if (evt.EventStatus == AssetRevaluationImpairmentViewModel.EventStatusEnum.Posted)
                throw new InvalidOperationException("Posted event cannot be deleted.");
            evt.IsDeleted = true;
            evt.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }
    }
}
