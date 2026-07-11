using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    // GM6: ExchangeRate Model
    public class ExchangeRateModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

        // Section 1: Rate Identity
        [Required(ErrorMessage = "Base Currency is required")]
        public Guid BaseCurrencyId { get; set; }
        public string? BaseCurrencyCode { get; set; }
        public string? BaseCurrencyName { get; set; }

        [Required(ErrorMessage = "Quote Currency is required")]
        public Guid QuoteCurrencyId { get; set; }
        public string? QuoteCurrencyCode { get; set; }
        public string? QuoteCurrencyName { get; set; }

        public Guid? CompanyId { get; set; }
        public string? CompanyCode { get; set; }
        public string? CompanyName { get; set; }

        [Required(ErrorMessage = "Rate Date is required")]
        public DateTime RateDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Rate Type is required")]
        public string RateType { get; set; } = "Spot";

        // Section 2: Rate Value & Precision
        [Required(ErrorMessage = "Exchange Rate is required")]
        [Range(0.00000001, double.MaxValue, ErrorMessage = "Rate must be greater than 0")]
        public decimal Rate { get; set; }

        public decimal InverseRate => Rate > 0 ? Math.Round(1 / Rate, 8) : 0;

        public bool IsTriangulated { get; set; } = false;
        public Guid? TriangulationCurrencyId { get; set; }
        public string? TriangulationCurrencyCode { get; set; }

        // Section 3: Rate Source & Evidence
        [Required(ErrorMessage = "Source Type is required")]
        public string SourceType { get; set; } = "ManualEntry";

        [StringLength(150)]
        public string? SourceName { get; set; }

        public Guid? EvidenceFileId { get; set; }
        public string? EvidenceFileName { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        // Section 4: Status & Governance
        [Required]
        public string Status { get; set; } = "Draft";
        public int VersionNo { get; set; } = 1;

        // Section 5: System Audit
        public bool IsActive => Status == "Active";
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        public string CurrencyPair => $"{BaseCurrencyCode ?? "?"}/{QuoteCurrencyCode ?? "?"}";
        public string FormattedRate => Rate.ToString("N8");
    }

}
