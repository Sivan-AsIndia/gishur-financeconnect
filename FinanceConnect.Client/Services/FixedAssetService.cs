using FinanceConnect.Client.Data;
using FinanceConnect.Client.Pages.Master.Branch;
using static FinanceConnect.Client.ViewModels.FixedAssetViewModel;

namespace FinanceConnect.Client.Services
{
    public class FixedAssetService
    {
        private readonly List<FixedAssetListDto> _assets = new();

        private readonly BranchService _branchService;
        private readonly VendorService _vendorService;
        private readonly AssetCategoryService _categoryService;

        // ── Constructor ───────────────────────────────────────────────────────────
        public FixedAssetService(
            BranchService branchService,
            VendorService vendorService,
            AssetCategoryService categoryService)
        {
            _branchService = branchService;
            _vendorService = vendorService;
            _categoryService = categoryService;
            _assets = FixedAssetSeedData.GetAllFixedAssets();
        }

        // ── READ ──────────────────────────────────────────────────────────────────

        public List<FixedAssetListDto> GetAll()
            => _assets.Where(x => !x.IsDeleted).ToList();

        public FixedAssetListDto? GetById(Guid id)
            => _assets.FirstOrDefault(x => x.FixedAssetId == id && !x.IsDeleted);

        public Task<List<FixedAssetListDto>> GetAllAsync()
            => Task.FromResult(GetAll());

        public Task<FixedAssetListDto?> GetByIdAsync(Guid id)
            => Task.FromResult(GetById(id));

        // ── CREATE ────────────────────────────────────────────────────────────────

        public void Add(FixedAsset model)
        {
            // Duplicate AssetCode check
            if (_assets.Any(x => x.CompanyId == model.CompanyId &&
                                 string.Equals(x.AssetCode, model.AssetCode,
                                               StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Asset Code already exists for this Company.");

            // Duplicate AssetTag check
            if (!string.IsNullOrWhiteSpace(model.AssetTag) &&
                _assets.Any(x => x.CompanyId == model.CompanyId &&
                                 string.Equals(x.AssetTag, model.AssetTag,
                                               StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Asset Tag already exists for this Company.");

            // Non-Draft must have PurchaseDate & PurchaseCost
            if (model.Status != AssetStatus.Draft &&
                (model.PurchaseCost <= 0 || model.PurchaseDate == default))
                throw new InvalidOperationException(
                    "Purchase date and cost are required for non-draft assets.");

            // Final AssetCode — overwrite preview with fresh generated code
            model.AssetCode = GenerateAssetCode();

            var dto = MapToDto(model);
            dto.FixedAssetId = Guid.NewGuid();
            dto.CreatedAt = DateTime.UtcNow;
            dto.IsDeleted = false;

            _assets.Add(dto);
        }

        public Task CreateAsync(FixedAsset model)
        {
            Add(model);
            return Task.CompletedTask;
        }

        // ── UPDATE ────────────────────────────────────────────────────────────────

        public void Update(FixedAsset model)
        {
            var existing = GetById(model.FixedAssetId);
            if (existing == null) return;

            if (existing.AssetStatus == AssetStatus.Disposed)
                throw new InvalidOperationException("Asset is disposed and cannot be modified.");

            if (existing.AssetStatus != AssetStatus.Draft &&
                existing.PurchaseCost != model.PurchaseCost)
                throw new InvalidOperationException(
                    "Purchase cost cannot be changed after the asset is active.");

            if (!string.IsNullOrWhiteSpace(model.AssetTag) &&
                _assets.Any(x => x.CompanyId == model.CompanyId &&
                                 x.FixedAssetId != model.FixedAssetId &&
                                 string.Equals(x.AssetTag, model.AssetTag,
                                               StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Asset Tag already exists for this Company.");

            // Core Identity
            existing.AssetCode = model.AssetCode;
            existing.AssetName = model.AssetName;
            existing.AssetCategoryId = model.AssetCategoryId;
            existing.CategoryName = ResolveCategoryName(model.AssetCategoryId);

            // Physical Identity
            existing.AssetTag = model.AssetTag;
            existing.SerialNumber = model.SerialNumber;

            // Ownership & Assignment
            existing.BranchId = model.BranchId;
            existing.BranchName = ResolveBranchName(model.BranchId);
            existing.Location = model.Location;
            existing.Custodian = model.Custodian;

            // Vendor
            existing.VendorId = model.VendorId;
            existing.VendorName = ResolveVendorName(model.VendorId);

            // Cost (Draft only)
            if (existing.AssetStatus == AssetStatus.Draft)
            {
                existing.PurchaseDate = model.PurchaseDate;
                existing.PurchaseCost = model.PurchaseCost;
                existing.SalvageValue = model.SalvageValue;
            }

            // Depreciation
            existing.IsDepreciable = model.IsDepreciable;
            existing.UsefulLifeMonths = model.UsefulLifeMonths;

            // Status & Notes
            existing.AssetStatus = model.Status;
            existing.Notes = model.Notes;

            // Audit
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = model.UpdatedBy;
        }

        public Task UpdateAsync(FixedAsset model)
        {
            Update(model);
            return Task.CompletedTask;
        }

        // ── STATUS ACTIONS ────────────────────────────────────────────────────────

        public Task ActivateAsync(Guid id) => ChangeStatusAsync(id, AssetStatus.Active);
        public Task InactivateAsync(Guid id) => ChangeStatusAsync(id, AssetStatus.Inactive);

        public Task DisposeAsync(Guid id)
        {
            var asset = GetById(id);
            if (asset == null) return Task.CompletedTask;

            if (asset.AssetStatus == AssetStatus.Disposed)
                throw new InvalidOperationException("Asset is already disposed.");

            asset.AssetStatus = AssetStatus.Disposed;
            asset.DisposedOn = DateTime.UtcNow;
            asset.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        private Task ChangeStatusAsync(Guid id, AssetStatus status)
        {
            var asset = GetById(id);
            if (asset == null) return Task.CompletedTask;

            if (asset.AssetStatus == AssetStatus.Disposed)
                throw new InvalidOperationException("Disposed asset status cannot be changed.");

            asset.AssetStatus = status;
            asset.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        // ── DELETE (Soft) ─────────────────────────────────────────────────────────

        public Task DeleteAsync(Guid id)
        {
            var asset = GetById(id);
            if (asset == null) return Task.CompletedTask;

            if (asset.AssetStatus != AssetStatus.Draft)
                throw new InvalidOperationException(
                    "Only Draft assets can be deleted. Use Dispose for active assets.");

            asset.IsDeleted = true;
            asset.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        // ── ASSET CODE GENERATION ─────────────────────────────────────────────────

        public Task<string> GenerateNextAssetCodeAsync()
        {
            var next = _assets.Count + 1;
            return Task.FromResult($"FA-{next:D6}");
        }

        private string GenerateAssetCode()
        {
            var next = _assets.Count + 1;
            return $"FA-{next:D6}";
        }

        // ── PRIVATE HELPERS ───────────────────────────────────────────────────────

        // CategoryName resolve
        private string? ResolveCategoryName(Guid categoryId)
        {
            if (categoryId == Guid.Empty) return null;
            return _categoryService.GetById(categoryId)?.CategoryName;
        }

        // BranchName resolve
        private string? ResolveBranchName(Guid? branchId)
        {
            if (!branchId.HasValue) return null;
            return _branchService.GetAll()
                .FirstOrDefault(b => b.Id == branchId.Value)?.BranchName;
        }

        // VendorName resolve
        private string? ResolveVendorName(Guid? vendorId)
        {
            if (!vendorId.HasValue) return null;
            return _vendorService.GetAll()
                .FirstOrDefault(v => v.Id == vendorId.Value)?.VendorName;
        }

        // MapToDto — all names resolved
        private FixedAssetListDto MapToDto(FixedAsset model) => new()
        {
            AssetCode = model.AssetCode,
            AssetName = model.AssetName,
            AssetCategoryId = model.AssetCategoryId,
            CategoryName = ResolveCategoryName(model.AssetCategoryId),
            AssetTag = model.AssetTag,
            SerialNumber = model.SerialNumber,
            CompanyId = model.CompanyId,
            BranchId = model.BranchId,
            BranchName = ResolveBranchName(model.BranchId),
            Location = model.Location,
            Custodian = model.Custodian,
            VendorId = model.VendorId,
            VendorName = ResolveVendorName(model.VendorId),
            PurchaseDate = model.PurchaseDate,
            PurchaseCost = model.PurchaseCost,
            SalvageValue = model.SalvageValue,
            UsefulLifeMonths = model.UsefulLifeMonths,
            IsDepreciable = model.IsDepreciable,
            AssetStatus = model.Status,
            Notes = model.Notes,
        };
    }
}
