using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    // GM5: Currency Model
    public class CurrencyModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

        // Section 1: Currency Identity
        [Required(ErrorMessage = "Currency Code is required")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency code must be exactly 3 characters")]
        [RegularExpression("^[A-Za-z]{3}$", ErrorMessage = "Must be exactly 3 letters")]
        public string CurrencyCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Currency Name is required")]
        [StringLength(100)]
        public string CurrencyName { get; set; } = string.Empty;

        [Range(0, 999, ErrorMessage = "Numeric code must be between 0-999")]
        public int? NumericCode { get; set; }

        [Required(ErrorMessage = "Currency Type is required")]
        public string CurrencyType { get; set; } = "Fiat";

        // Section 2: Symbol & Formatting
        [StringLength(10)]
        public string? Symbol { get; set; }

        [Required(ErrorMessage = "Symbol Position is required")]
        public string SymbolPosition { get; set; } = "Prefix";

        [StringLength(50)]
        public string? DisplayFormat { get; set; }

        // Section 3: Decimal / Rounding Behavior
        [Required(ErrorMessage = "Decimal Places is required")]
        [Range(0, 4, ErrorMessage = "Decimal places must be between 0-4")]
        public int? DecimalPlaces { get; set; } = 2;

        [StringLength(50)]
        public string? MinorUnitName { get; set; }

        [Required(ErrorMessage = "Rounding Mode is required")]
        public string RoundingMode { get; set; } = "Round Half Up";

        public decimal? RoundingStep { get; set; }

        // Section 4: Status & Governance
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        [StringLength(500)]
        public string? Notes { get; set; }

        // Section 5: System Audit
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
    }

}
