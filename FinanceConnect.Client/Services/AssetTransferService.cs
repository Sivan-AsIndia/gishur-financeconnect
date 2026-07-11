using FinanceConnect.Client.Data;
using static FinanceConnect.Client.ViewModels.AssetTransformViewModel;

namespace FinanceConnect.Client.Services
{
    public class AssetTransferService
    {
        private readonly List<AssetTransferListDto> _transfers = new();

        /* ================= CONSTRUCTOR ================= */

        public AssetTransferService()
        {
            _transfers = AssetTransferSeedData.GetAllAssetTransfers();

        }
        public Task<string> GenerateTransferNumber()
        {
            var next = _transfers.Count + 1;
            return Task.FromResult($"FATR-{next:D6}");
        }
      
        /* ================= READ ================= */

        public List<AssetTransferListDto> GetAll()
            => _transfers.Where(x => !x.IsDeleted).ToList();

        public AssetTransferListDto? GetById(Guid id)
            => _transfers.FirstOrDefault(x => x.AssetTransferId == id && !x.IsDeleted);

        public Task<List<AssetTransferListDto>> GetAllAsync()
            => Task.FromResult(GetAll());

        public Task<AssetTransferListDto?> GetByIdAsync(Guid id)
            => Task.FromResult(GetById(id));

        public List<AssetTransferListDto> GetByAssetId(Guid fixedAssetId)
            => _transfers.Where(x => x.FixedAssetId == fixedAssetId && !x.IsDeleted).ToList();

        /* ================= CREATE ================= */

        public void Add(AssetTransfer model)
        {
            // Business rule: At least one "To" field must differ from "From"
            if (model.ToBranchId     == model.FromBranchId     &&
                model.ToLocationId   == model.FromLocationId   &&
                model.ToCustodianUserId == model.FromCustodianUserId &&
                model.ToDepartmentId == model.FromDepartmentId)
                throw new InvalidOperationException(
                    "At least one field must change. Transfer cannot be a no-change.");

            // Business rule: asset must not be Disposed
            if (model.AssetStatusSnapshot == "Disposed")
                throw new InvalidOperationException(
                    "Cannot transfer a Disposed asset.");

            // Transfer type validations
            ValidateTransferType(model);

          

            var dto = MapToDto(model);
            dto.AssetTransferId = Guid.NewGuid();
            dto.CreatedAt       = DateTime.UtcNow;
            dto.IsDeleted       = false;

            _transfers.Add(dto);
        }

        public Task CreateAsync(AssetTransfer model)
        {
            Add(model);
            return Task.CompletedTask;
        }

        /* ================= UPDATE ================= */

        public void Update(AssetTransfer model)
        {
            var existing = GetById(model.AssetTransferId);
            if (existing == null) return;

            // Block edits on posted/closed transfers
            if (existing.TransferStatus == TransferStatus.Posted ||
                existing.TransferStatus == TransferStatus.Closed)
                throw new InvalidOperationException(
                    "Posted/Closed transfers cannot be modified.");

            existing.TransferType          = model.TransferType;
            existing.EffectiveTransferDate = model.EffectiveTransferDate;
            existing.ToBranchName          = model.ToBranchName;
            existing.ToLocationName        = model.ToLocationName;
            existing.ToCustodianName       = model.ToCustodianName;
            existing.TransferReason        = model.TransferReason;
            existing.UpdatedAt             = DateTime.UtcNow;
        }

        public Task UpdateAsync(AssetTransfer model)
        {
            Update(model);
            return Task.CompletedTask;
        }

        /* ================= STATUS ACTIONS ================= */

        public Task SubmitAsync(Guid id)
            => ChangeStatusAsync(id, TransferStatus.Submitted,
               allowed: new[] { TransferStatus.Draft, TransferStatus.Rejected });

        public Task ApproveAsync(Guid id)
            => ChangeStatusAsync(id, TransferStatus.Approved,
               allowed: new[] { TransferStatus.Submitted });

        public Task RejectAsync(Guid id, string reason)
        {
            var t = GetById(id);
            if (t == null) return Task.CompletedTask;
            if (t.TransferStatus != TransferStatus.Submitted)
                throw new InvalidOperationException("Only Submitted transfers can be rejected.");
            t.TransferStatus = TransferStatus.Rejected;
            t.TransferReason = reason;
            t.UpdatedAt      = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        public Task CancelAsync(Guid id)
            => ChangeStatusAsync(id, TransferStatus.Cancelled,
               allowed: new[] { TransferStatus.Draft, TransferStatus.Submitted });

        public Task MarkInTransitAsync(Guid id)
            => ChangeStatusAsync(id, TransferStatus.InTransit,
               allowed: new[] { TransferStatus.Approved });

        public Task ConfirmReceiptAsync(Guid id)
            => ChangeStatusAsync(id, TransferStatus.Received,
               allowed: new[] { TransferStatus.InTransit, TransferStatus.Approved });

        public Task PostAsync(Guid id)
        {
            var t = GetById(id);
            if (t == null) return Task.CompletedTask;

            if (t.TransferStatus != TransferStatus.Approved &&
                t.TransferStatus != TransferStatus.Received)
                throw new InvalidOperationException(
                    "Only Approved or Received transfers can be posted.");

            t.TransferStatus     = TransferStatus.Posted;
            t.AppliedToAssetFlag = true;
            t.UpdatedAt          = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        public Task ReverseAsync(Guid id, string reversalReason)
        {
            var t = GetById(id);
            if (t == null) return Task.CompletedTask;

            if (t.TransferStatus != TransferStatus.Posted)
                throw new InvalidOperationException("Only Posted transfers can be reversed.");

            if (string.IsNullOrWhiteSpace(reversalReason))
                throw new InvalidOperationException("Reversal reason is required.");

            t.TransferStatus = TransferStatus.Reversed;
            t.TransferReason = reversalReason;
            t.UpdatedAt      = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        /* ================= DELETE (Soft) ================= */

        public Task DeleteAsync(Guid id)
        {
            var t = GetById(id);
            if (t == null) return Task.CompletedTask;

            if (t.TransferStatus != TransferStatus.Draft)
                throw new InvalidOperationException(
                    "Only Draft transfers can be deleted.");

            t.IsDeleted = true;
            t.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        /* ================= PRIVATE HELPERS ================= */

        private static void ValidateTransferType(AssetTransfer model)
        {
            switch (model.TransferType)
            {
                case TransferType.CustodianChange:
                    if (model.ToCustodianUserId == null)
                        throw new InvalidOperationException(
                            "Custodian Change requires a new Custodian.");
                    if (model.ToCustodianUserId == model.FromCustodianUserId)
                        throw new InvalidOperationException(
                            "New Custodian must differ from current Custodian.");
                    break;

                case TransferType.LocationChange:
                    if (model.ToLocationId == null)
                        throw new InvalidOperationException(
                            "Location Change requires a new Location.");
                    if (model.ToLocationId == model.FromLocationId)
                        throw new InvalidOperationException(
                            "New Location must differ from current Location.");
                    break;

                case TransferType.BranchChange:
                    if (model.ToBranchId == null)
                        throw new InvalidOperationException(
                            "Branch Change requires a new Branch.");
                    if (model.ToBranchId == model.FromBranchId)
                        throw new InvalidOperationException(
                            "New Branch must differ from current Branch.");
                    break;

                case TransferType.FullReassignment:
                    if (model.ToBranchId == null)
                        throw new InvalidOperationException(
                            "Full Reassignment requires a new Branch.");
                    break;
            }
        }

        private Task ChangeStatusAsync(Guid id, TransferStatus newStatus,
                                        TransferStatus[] allowed)
        {
            var t = GetById(id);
            if (t == null) return Task.CompletedTask;

            if (!allowed.Contains(t.TransferStatus))
                throw new InvalidOperationException(
                    $"Cannot change status to {newStatus} from {t.TransferStatus}.");

            t.TransferStatus = newStatus;
            t.UpdatedAt      = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        private static AssetTransferListDto MapToDto(AssetTransfer model) => new()
        {
            TransferNumber        = model.TransferNumber,
            TransferStatus        = model.TransferStatus,
            TransferType          = model.TransferType,
            EffectiveTransferDate = model.EffectiveTransferDate,
            FixedAssetId          = model.FixedAssetId,
            AssetNumberSnapshot   = model.AssetNumberSnapshot,
            AssetNameSnapshot     = model.AssetNameSnapshot,
            FromBranchName        = model.FromBranchName,
            FromLocationName      = model.FromLocationName,
            FromCustodianName     = model.FromCustodianName,
            ToBranchName          = model.ToBranchName,
            ToLocationName        = model.ToLocationName,
            ToCustodianName       = model.ToCustodianName,
            TransferReason        = model.TransferReason,
            AppliedToAssetFlag    = model.AppliedToAssetFlag,
            CompanyId             = model.CompanyId,
        };
    }
}
