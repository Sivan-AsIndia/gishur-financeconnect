using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    public class AssetAcquisitionService
    {
        private readonly List<AssetAcquisitionViewModel.AssetAcquisition> _acquisitions;

        public AssetAcquisitionService()
        {
            _acquisitions = AssetAcquisitionSeedData.GetAll();
        }

        /* ================= READ ================= */

        public List<AssetAcquisitionViewModel.AssetAcquisition> GetAll()
            => _acquisitions;

        public AssetAcquisitionViewModel.AssetAcquisition? GetById(Guid id)
            => _acquisitions.FirstOrDefault(x => x.AssetAcquisitionId == id);

        public Task<AssetAcquisitionViewModel.AssetAcquisition?> GetByIdAsync(Guid id)
            => Task.FromResult(GetById(id));

        /* ================= CREATE ================= */

        public void Add(AssetAcquisitionViewModel.AssetAcquisition model)
        {
            if (model.FixedAssetId == null || model.FixedAssetId == Guid.Empty)
                throw new InvalidOperationException("Fixed Asset is required.");

            if (model.AcquisitionType == null)
                throw new InvalidOperationException("Acquisition Type is required.");

            if (model.AcquisitionDate == null)
                throw new InvalidOperationException("Acquisition Date is required.");

            if (!model.CostLines.Any())
                throw new InvalidOperationException("At least one cost line is required.");

            if (model.TotalCapitalizedAmount <= 0)
                throw new InvalidOperationException("Total Capitalized Amount must be > 0.");

            model.AssetAcquisitionId = Guid.NewGuid();
            model.AcquisitionNumber = $"FAACQ-{(_acquisitions.Count + 1):D6}";
            model.AcquisitionStatus = AssetAcquisitionViewModel.AcquisitionStatusEnum.Draft;
            model.CreatedAt = DateTime.UtcNow;
            model.IsDeleted = false;

            _acquisitions.Add(model);
        }

        public Task CreateAsync(AssetAcquisitionViewModel.AssetAcquisition model)
        {
            Add(model);
            return Task.CompletedTask;
        }

        /* ================= UPDATE ================= */

        public void Update(AssetAcquisitionViewModel.AssetAcquisition model)
        {
            var existing = GetById(model.AssetAcquisitionId);
            if (existing == null) return;

            if (existing.AcquisitionStatus == AssetAcquisitionViewModel.AcquisitionStatusEnum.Posted)
                throw new InvalidOperationException("Posted acquisition cannot be edited. Use reversal.");

            existing.BranchId = model.BranchId;
            existing.AcquisitionType = model.AcquisitionType;
            existing.AcquisitionDate = model.AcquisitionDate;
            existing.CapitalizationDate = model.CapitalizationDate;
            existing.FixedAssetId = model.FixedAssetId;
            existing.AssetCategoryIdSnapshot = model.AssetCategoryIdSnapshot;
            existing.AssetNumberSnapshot = model.AssetNumberSnapshot;
            existing.AssetNameSnapshot = model.AssetNameSnapshot;
            existing.AssetStatusSnapshot = model.AssetStatusSnapshot;
            existing.SourceModule = model.SourceModule;
            existing.VendorId = model.VendorId;
            existing.VendorInvoiceNumber = model.VendorInvoiceNumber;
            existing.VendorInvoiceDate = model.VendorInvoiceDate;
            existing.APVendorBillId = model.APVendorBillId;
            existing.PurchaseOrderRef = model.PurchaseOrderRef;
            existing.ReferenceText = model.ReferenceText;
            existing.Narration = model.Narration;
            existing.CostLines = model.CostLines;
            existing.RoundOffAmount = model.RoundOffAmount;
            existing.PostingRoute = model.PostingRoute;
            existing.ThresholdOverrideApproved = model.ThresholdOverrideApproved;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        public Task UpdateAsync(AssetAcquisitionViewModel.AssetAcquisition model)
        {
            Update(model);
            return Task.CompletedTask;
        }

        /* ================= WORKFLOW ACTIONS ================= */

        public Task SubmitAsync(Guid id)
            => ChangeStatusAsync(id, AssetAcquisitionViewModel.AcquisitionStatusEnum.Submitted);

        public Task ApproveAsync(Guid id)
            => ChangeStatusAsync(id, AssetAcquisitionViewModel.AcquisitionStatusEnum.Approved);

        public Task RejectAsync(Guid id)
            => ChangeStatusAsync(id, AssetAcquisitionViewModel.AcquisitionStatusEnum.Rejected);

        public Task PostAsync(Guid id)
        {
            var acq = GetById(id);
            if (acq == null) return Task.CompletedTask;

            if (acq.AcquisitionStatus == AssetAcquisitionViewModel.AcquisitionStatusEnum.Posted)
                throw new InvalidOperationException("Acquisition is already posted.");

            if (acq.TotalCapitalizedAmount <= 0)
                throw new InvalidOperationException("Total Capitalized Amount must be > 0.");

            acq.AcquisitionStatus = AssetAcquisitionViewModel.AcquisitionStatusEnum.Posted;
            acq.PostingDate = DateTime.UtcNow;
            acq.PostedOn = DateTime.UtcNow;
            acq.JournalEntryId = Guid.NewGuid();
            acq.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        public Task CancelAsync(Guid id)
            => ChangeStatusAsync(id, AssetAcquisitionViewModel.AcquisitionStatusEnum.Cancelled);

        public Task ReverseAsync(Guid id)
            => ChangeStatusAsync(id, AssetAcquisitionViewModel.AcquisitionStatusEnum.Reversed);

        private Task ChangeStatusAsync(Guid id, AssetAcquisitionViewModel.AcquisitionStatusEnum status)
        {
            var acq = GetById(id);
            if (acq == null) return Task.CompletedTask;

            acq.AcquisitionStatus = status;
            acq.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        /* ================= DELETE (Soft) ================= */

        public Task DeleteAsync(Guid id)
        {
            var acq = GetById(id);
            if (acq == null) return Task.CompletedTask;

            if (acq.AcquisitionStatus == AssetAcquisitionViewModel.AcquisitionStatusEnum.Posted)
                throw new InvalidOperationException("Posted acquisition cannot be deleted.");

            acq.IsDeleted = true;
            acq.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }
    }
}
