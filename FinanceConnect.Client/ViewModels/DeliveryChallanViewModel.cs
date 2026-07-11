using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public class DeliveryChallanViewModel
    {
        public Guid? Id { get; set; }

        public Guid TenantId { get; set; }

        [Required(ErrorMessage = "Company is required")]
        public Guid? CompanyId { get; set; }
        public string? CompanyName { get; set; }

        [Required(ErrorMessage = "Customer is required")]
        public Guid? CustomerId { get; set; }
        public string? CustomerName { get; set; }

        public string? ChallanNumber { get; set; }

        // Dates
        public DateTime? ChallanDate { get; set; } = DateTime.Today;
        public DateTime? ShippingDate { get; set; }

        // PO Reference

        [MaxLength(30)]
        public string? PONumber { get; set; }

        // Items
        public List<DeliveryChallanLineItemViewModel> Items { get; set; } = new();

        // Options
        public bool ShowTotalQuantity { get; set; }
        public bool ShowCess { get; set; }
        public bool ShowTransportDetails { get; set; }
        public TransportDetailsViewModel? TransportDetails { get; set; }
        // Totals
        public decimal SubTotal { get; set; }

        public decimal TaxPercentage { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal Discount { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal GrandTotal { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Notes
        [MaxLength(1000)]
        public string? TermsAndConditions { get; set; }

        [MaxLength(1000)]
        public string? PrivateNotes { get; set; }
    }

    public class TransportDetailsViewModel
    {

        [MaxLength(150)]
        public string? TransporterName { get; set; }

        [Required(ErrorMessage = "Vehicle Number is required")]
        [StringLength(13, MinimumLength = 4, ErrorMessage = "Vehicle number must be between 4 and 13 characters.")]
        [RegularExpression(@"^[A-Z]{2}[0-9]{2}[A-Z]{0,2}[0-9]{4}$|(?i)^[A-Z0-9]{4,13}$",
            ErrorMessage = "Invalid Vehicle Number format.")]
        public string? VehicleNumber { get; set; }

        [MaxLength(20, ErrorMessage = "LR Number cannot exceed 20 characters.")]
        public string? LRNumber { get; set; }

        public DateTime? LRDate { get; set; }

        [StringLength(12, MinimumLength = 12, ErrorMessage = "E-Way Bill must be exactly 12 digits.")]
        [RegularExpression(@"^[1-9][0-9]{11}$", ErrorMessage = "Invalid E-Way Bill format.")]
        public string? EWayBillNumber { get; set; }
    }
}
