using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    // GM1: Country Model
    public class CountryModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

        // Section 1: Country Identity
        [Required(ErrorMessage = "Country Code is required")]
        [StringLength(10)]
        [RegularExpression("^[A-Za-z0-9]+$", ErrorMessage = "Letters and numbers only")]
        public string CountryCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Country Name is required")]
        [StringLength(150)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Numbers and special characters are not allowed.")]
        public string CountryName { get; set; } = string.Empty;

        [StringLength(200)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Numbers and special characters are not allowed.")]
        public string? OfficialName { get; set; }

        [StringLength(50)]
        public string? Region { get; set; }

        // Section 2: ISO Codes & Standards
        [Required(ErrorMessage = "ISO 2-letter code is required")]
        [StringLength(2, MinimumLength = 2)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Numbers and special characters are not allowed.")]
        public string ISO2 { get; set; } = string.Empty;

        [StringLength(3, MinimumLength = 3)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Numbers and special characters are not allowed.")]
        public string? ISO3 { get; set; }

        [StringLength(10)]
        public string? NumericCode { get; set; }

        [StringLength(3)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Numbers and special characters are not allowed.")]
        public string? DefaultCurrencyCode { get; set; }

        // Section 3: Address/Postal Rules
        public bool HasStates { get; set; } = true;

        [StringLength(100)]
        public string? PostalCodePattern { get; set; }

        [StringLength(10)]
        [RegularExpression(@"^\+?[0-9]*$", ErrorMessage = "Only numbers and a leading + are allowed.")]
        public string? PhoneCountryCode { get; set; }

        // Section 4: Status & Usage Controls
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public int SortOrder { get; set; } = 0;

        // Section 5: System Audit
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
    }

}
