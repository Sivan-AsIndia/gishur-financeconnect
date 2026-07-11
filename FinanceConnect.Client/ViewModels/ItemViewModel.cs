using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public enum ItemType
    {
        Product = 1,
        Service = 2
    }

    public enum ItemStatus
    {
        Active = 1,
        InActive = 2
    }
    
    public class ItemViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Company is required")]
        public Guid CompanyId { get; set; }

        public string? CompanyName { get; set; }

        [Required(ErrorMessage = "Item name is required")]
        [StringLength(200)]
        public string ItemName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Item Code is required")]
        [StringLength(30)]
        public string? ItemCode { get; set; }

        [StringLength(30)]
        public string? HSNCode { get; set; }

        [Required]
        public ItemType ItemType { get; set; } = ItemType.Product;

        [Required(ErrorMessage = "Unit is required")]
        public Guid? UnitId { get; set; }
        public string? UnitName { get; set; }

        public decimal DefaultRate { get; set; }

        public decimal? CostPrice { get; set; }

        public decimal TaxPercentage { get; set; }

        public ItemStatus? Status { get; set; } = ItemStatus.Active;

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy  { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class UnitViewModel
    {
        public Guid Id { get; set; }

        public string UnitName { get; set; } = string.Empty;

        public string? Symbol { get; set; }
    }


    public class TaxViewModel
    {
        public decimal Percentage { get; set; }

        public string DisplayName { get; set; } = string.Empty;
    }
}
