using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    // Company Model (Model #1)
    public class CompanyModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

        // Section 1: Company Identity
        [Required(ErrorMessage = "Company Code is required")]
        [StringLength(10)]
        [RegularExpression("^[A-Za-z0-9_-]+$", ErrorMessage = "Letters, numbers, - and _ allowed")]
        public string CompanyCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Legal Name is required")]
        [StringLength(200)]
        public string LegalName { get; set; } = string.Empty;

        [StringLength(150)]
        public string? TradeName { get; set; }

        [StringLength(50)]
        public string? ShortName { get; set; }

        [Required(ErrorMessage = "Legal Structure is required")]
        public string LegalStructure { get; set; } = "Private Limited";

        [StringLength(100)]
        public string? OtherLegalStructure { get; set; }

        public DateTime? IncorporationDate { get; set; }

        public Guid? ParentCompanyId { get; set; }
        public string? ParentCompanyName { get; set; }

        public Guid? DefaultBranchId { get; set; }
        public string? DefaultBranchName { get; set; }

        // Section 2: Registration & Compliance
        [StringLength(50)]

        public string? RegistrationNumber { get; set; }

        [StringLength(10)]
        [RegularExpression("^[A-Za-z]{5}[0-9]{4}[A-Za-z]$", ErrorMessage = "Invalid PAN format (e.g., AAAAA9999A)")]
        public string? PANNumber { get; set; }

        [StringLength(15)]
        [RegularExpression(@"^\d{2}[A-Za-z]{5}\d{4}[A-Za-z]\d[Zz][A-Za-z\d]$", ErrorMessage = "Invalid GSTIN format (e.g., 33AAAAA9999A1Z5)")]
        public string? GSTIN { get; set; }

        public bool IsGSTRegistered { get; set; } = true;

        [StringLength(10)]
        public string? TANNumber { get; set; }

        [StringLength(50)]
        public string? OtherTaxId { get; set; }

        [Required(ErrorMessage = "Registration Country is required")]
        public Guid? RegistrationCountryId { get; set; }

        public string? RegistrationCountryName { get; set; }

        public Guid? RegistrationStateProvinceId { get; set; }
        public string? RegistrationStateProvinceName { get; set; }

        public Guid? RegistrationCityId { get; set; }
        public string? RegistrationCityName { get; set; }

        // Section 3: Registered Address
        [Required(ErrorMessage = "Address Line 1 is required")]
        [StringLength(200)]
        public string AddressLine1 { get; set; } = string.Empty;

        [StringLength(200)]
        public string? AddressLine2 { get; set; }

        [Required(ErrorMessage = "Country is required")]
        public Guid CountryId { get; set; }
        public string? CountryName { get; set; }

        public Guid? StateProvinceId { get; set; }
        public string? StateProvinceName { get; set; }

        [Required(ErrorMessage = "City is required")]
        public Guid? CityId { get; set; }
        public string? CityName { get; set; }

        [Required(ErrorMessage = "Postal Code is required")]
        [StringLength(20)]
        public string PostalCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Time Zone is required")]
        public Guid? TimeZoneId { get; set; }
        public string? TimeZoneName { get; set; }

        // Section 4: Contact & Branding
        [StringLength(100)]
        public string? PrimaryContactName { get; set; }

        [StringLength(150)]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? PrimaryEmail { get; set; }

        [StringLength(30)]
        [RegularExpression(@"^[\d\+\-\s\(\)]+$", ErrorMessage = "Only digits, +, -, spaces and parentheses allowed")]
        public string? PrimaryPhone { get; set; }

        [StringLength(200)]
        [Url(ErrorMessage = "Invalid URL format")]
        public string? WebsiteUrl { get; set; }

        // Logo as Base64 string
        public string? LogoBase64 { get; set; }
        public string? LogoFileName { get; set; }
        public string? LogoContentType { get; set; }

        // Section 5: Financial Settings
        [Required(ErrorMessage = "Base Currency is required")]
        public Guid? BaseCurrencyId { get; set; }
        public string? BaseCurrencyName { get; set; }

        public Guid? ReportingCurrencyId { get; set; }
        public string? ReportingCurrencyName { get; set; }

        [Required(ErrorMessage = "Fiscal Year Start Month is required")]
        [Range(1, 12, ErrorMessage = "Month must be between 1 and 12")]
        public int FiscalYearStartMonth { get; set; } = 4;

        [Required(ErrorMessage = "Books Start Date is required")]
        public DateTime BooksStartDate { get; set; } = DateTime.Today;

        public bool EnableMultiCurrency { get; set; } = false;

        [Range(0, 4, ErrorMessage = "Rounding Precision must be between 0 and 4")]
        public int RoundingPrecision { get; set; } = 2;

        public string RoundingMode { get; set; } = "Round Half Up";

        // Section 6: System & Posting Controls
        public DateTime? AllowPostingFromDate { get; set; }
        public DateTime? AllowPostingToDate { get; set; }
        public bool LockBackDatedPosting { get; set; } = false;
        public int BackdatedPostingDaysAllowed { get; set; } = 0;
        public int FuturePostingDaysAllowed { get; set; } = 0;

        [StringLength(1000)]
        public string? Notes { get; set; }

        // Section 7: Status & Governance
        [Required]
        public string Status { get; set; } = "Draft";
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        // Section 8: System Audit
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
    }

}
