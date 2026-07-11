using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{

    public enum QuotationStatus
    {
        New = 1,
        SentToClient = 2,
        Accepted = 3,
        Declined = 4,
        AnalyzeDecline = 5
    }

    public static class QuotationStatusExtensions
    {
        public static string GetDisplayName(this QuotationStatus status)
        {
            return status switch
            {
                QuotationStatus.New => "New",
                QuotationStatus.SentToClient => "Sent To Client",
                QuotationStatus.Accepted => "Accepted",
                QuotationStatus.Declined => "Declined",
                QuotationStatus.AnalyzeDecline => "Analyze Decline",
                _ => status.ToString()
            };
        }
    }
    public class QuotationViewModel
    {
        public Guid? Id { get; set; }

        public Guid TenantId { get; set; }

        [Required(ErrorMessage = "Company is required")]
        public Guid? CompanyId { get; set; }
        public string? CompanyName { get; set; }

        [Required]
        [StringLength(200)]
        public string Subject { get; set; } = string.Empty;

        public string QuotationNumber { get; set; } = "";

        public Guid? QuotationRequestId { get; set; }

        [Required]
        public Guid? CustomerId { get; set; }

        [Required]
        public QuotationStatus Status { get; set; } = QuotationStatus.New;

        [Required]
        public DateTime? QuotationDate { get; set; } = DateTime.Today;

        [Required]
        public DateTime? ExpiryDate { get; set; }

        [Required]
        public Guid? OwnerId { get; set; }
        public string? OwnerName { get; set; }

        public bool SendQuotationInMail { get; set; }

        [StringLength(2000)]
        public string? TermsAndConditions { get; set; }

        public string? Description { get; set; }

        public decimal SubTotal { get; set; }

        public decimal TaxPercentage { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal Discount { get; set; }

        public bool ApplyDiscount { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal GrandTotal { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }


        public List<QuotationLineItemViewModel> Items { get; set; } = new();
    }
}

