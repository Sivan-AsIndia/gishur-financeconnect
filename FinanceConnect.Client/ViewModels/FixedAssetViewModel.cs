using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public class FixedAssetViewModel
    {
        public enum AssetStatus
        {
            Draft = 1,
            Active = 2,
            Inactive = 3,
            Disposed = 4
        }

        public class FixedAsset
        {
            public Guid FixedAssetId { get; set; }

            [Required]
            [StringLength(50)]
            public string AssetCode { get; set; } = "";

            [Required]
            [StringLength(150)]
            public string AssetName { get; set; } = "";

            public Guid AssetCategoryId { get; set; }

            [StringLength(100)]
            public string? AssetTag { get; set; }

            [StringLength(100)]
            public string? SerialNumber { get; set; }

            public Guid? CompanyId { get; set; }
            public Guid? BranchId { get; set; }
            public Guid? VendorId { get; set; }

            [Required]
            public DateTime PurchaseDate { get; set; }

            [Range(0, double.MaxValue)]
            public decimal PurchaseCost { get; set; }

            [Range(0, double.MaxValue)]
            public decimal? SalvageValue { get; set; }

            public int? UsefulLifeMonths { get; set; }

            [StringLength(150)]
            public string? Location { get; set; }

            [StringLength(150)]
            public string? Custodian { get; set; }

            public bool IsDepreciable { get; set; } = true;

            public AssetStatus Status { get; set; } = AssetStatus.Draft;

            [StringLength(500)]
            public string? Notes { get; set; }

            public Guid? UpdatedBy { get; set; }
        }

        public class FixedAssetListDto
        {
            public Guid FixedAssetId { get; set; }
            public string? AssetCode { get; set; }
            public string? AssetName { get; set; }
            public string? AssetType { get; set; }
            public Guid? AssetCategoryId { get; set; }
            public string? CategoryName { get; set; }
            public string? AssetTag { get; set; }
            public string? SerialNumber { get; set; }
            public Guid? CompanyId { get; set; }
            public Guid? BranchId { get; set; }
            public Guid? LocationId { get; set; }
            public Guid? CustodianUserId { get; set; }
            public string? BranchName { get; set; }
            public string? Location { get; set; }
            public string? Custodian { get; set; }
            public AssetStatus AssetStatus { get; set; }
            public DateTime? PurchaseDate { get; set; }
            public decimal PurchaseCost { get; set; }
            public decimal? SalvageValue { get; set; }
            public decimal? NetBookValue { get; set; }
            public bool IsDepreciable { get; set; }
            public int? UsefulLifeMonths { get; set; }
            public Guid? VendorId { get; set; }
            public string? VendorName { get; set; }  // ✅ SeedData-க்கு match
            public string? Notes { get; set; }
            public DateTime? DisposedOn { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime? UpdatedAt { get; set; }
            public Guid? UpdatedBy { get; set; }
            public bool IsDeleted { get; set; }
        }

        public class SelectItem
        {
            public string Value { get; set; } = "";
            public string Text { get; set; } = "";
        }
    }
}
