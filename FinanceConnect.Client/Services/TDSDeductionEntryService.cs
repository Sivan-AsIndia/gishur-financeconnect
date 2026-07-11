using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;
using static FinanceConnect.Client.ViewModels.TDSDeductionEntryViewModel;

namespace FinanceConnect.Client.Services
{
    public class TDSDeductionEntryService
    {
        private readonly List<TDSDeductionEntryListDto> _store;

        public TDSDeductionEntryService(TDSDeductionEntrySeedData seedData)
        {
            _store = seedData.Store;
        }

        public Task<List<TDSDeductionEntryListDto>> GetAllAsync()
            => Task.FromResult(_store.OrderByDescending(x => x.DeductionDate).ToList());

        public Task<TDSDeductionEntryListDto?> GetByIdAsync(Guid id)
            => Task.FromResult(_store.FirstOrDefault(x => x.TDSDeductionEntryId == id));

        public Task ReverseAsync(Guid id, string reason)
        {
            var entry = _store.FirstOrDefault(x => x.TDSDeductionEntryId == id)
                ?? throw new InvalidOperationException("Deduction entry not found.");

            if (entry.Status != DeductionStatus.Posted && entry.Status != DeductionStatus.PartiallySettled)
                throw new InvalidOperationException("Only Posted or Partially Settled entries can be reversed.");

            if (string.IsNullOrWhiteSpace(reason))
                throw new InvalidOperationException("Reversal reason is required.");

            entry.Status = DeductionStatus.Reversed;

            var reversal = new TDSDeductionEntryListDto
            {
                TDSDeductionEntryId = Guid.NewGuid(),
                DeductionNumber = $"TDS-REV-{entry.DeductionNumber}",
                Status = DeductionStatus.Posted,
                DeductionDate = DateTime.Today,
                PostingDate = DateTime.Today,
                SourceDocumentType = entry.SourceDocumentType,
                SourceDocumentNumberSnapshot = entry.SourceDocumentNumberSnapshot,
                VendorCodeSnapshot = entry.VendorCodeSnapshot,
                VendorNameSnapshot = entry.VendorNameSnapshot,
                VendorPANSnapshot = entry.VendorPANSnapshot,
                VendorResidencySnapshot = entry.VendorResidencySnapshot,
                SectionCodeSnapshot = entry.SectionCodeSnapshot,
                TaxCodeSnapshot = entry.TaxCodeSnapshot,
                RatePercentApplied = entry.RatePercentApplied,
                DeductionBaseAmount = -entry.DeductionBaseAmount,
                DeductionAmount = -entry.DeductionAmount,
                SettledAmount = 0,
                SettlementStatus = SettlementStatus.NotSettled,
                ThresholdEvaluationModeSnapshot = entry.ThresholdEvaluationModeSnapshot,
                ThresholdTriggeredFlag = entry.ThresholdTriggeredFlag,
                IsSystemReversal = true,
                ReversalOfTDSDeductionEntryId = id,
                PostedOn = DateTime.Now,
                CreatedAt = DateTime.Now
            };

            _store.Add(reversal);
            return Task.CompletedTask;
        }

        public Task MarkReconciledAsync(Guid id)
        {
            var entry = _store.FirstOrDefault(x => x.TDSDeductionEntryId == id)
                ?? throw new InvalidOperationException("Deduction entry not found.");

            if (entry.Status == DeductionStatus.Settled)
                throw new InvalidOperationException("Entry is already fully settled.");

            if (entry.Status != DeductionStatus.PartiallySettled && entry.Status != DeductionStatus.Posted)
                throw new InvalidOperationException("Only Posted or Partially Settled entries can be reconciled.");

            entry.SettledAmount = entry.DeductionAmount;
            entry.SettlementStatus = SettlementStatus.FullySettled;
            entry.Status = DeductionStatus.Settled;
            entry.UpdatedAt = DateTime.Now;
            return Task.CompletedTask;
        }

        public Task CancelAsync(Guid id)
        {
            var entry = _store.FirstOrDefault(x => x.TDSDeductionEntryId == id)
                ?? throw new InvalidOperationException("Deduction entry not found.");

            if (entry.Status != DeductionStatus.Draft)
                throw new InvalidOperationException("Only Draft entries can be cancelled.");

            entry.Status = DeductionStatus.Cancelled;
            entry.UpdatedAt = DateTime.Now;
            return Task.CompletedTask;
        }

        public static string GetStatusLabel(DeductionStatus s)
            => TDSDeductionEntry.GetStatusLabel(s);

        public static string GetStatusPillClass(DeductionStatus s)
            => TDSDeductionEntry.GetStatusPillClass(s);
    }
}
