using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    // GM2: StateProvince Model
    public class StateProvinceModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

        // Section 1: State Identity
        [Required(ErrorMessage = "State/Province Code is required")]
        [StringLength(10)]
        [RegularExpression("^[A-Za-z0-9_-]+$", ErrorMessage = "Letters, numbers, - and _ allowed")]
        public string StateProvinceCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "State/Province Name is required")]
        [StringLength(150)]
        public string StateProvinceName { get; set; } = string.Empty;

        [StringLength(200)]
        public string? DisplayName { get; set; }

        // Section 2: Country Linkage & Jurisdiction Type
        [Required(ErrorMessage = "Country is required")]
        public Guid? CountryId { get; set; }
        public string? CountryName { get; set; }

        [Required(ErrorMessage = "Jurisdiction Type is required")]
        public string? JurisdictionType { get; set; } = "State";

        public bool IsFederalJurisdiction { get; set; }

        // Section 3: Compliance & Tax Codes
        [StringLength(2)]
        public string? GSTStateCode { get; set; }

        [StringLength(30)]
        public string? StateTaxJurisdictionCode { get; set; }

        public Guid? DefaultTimeZoneId { get; set; }

        // Section 4: Address & Postal Rules
        [StringLength(100)]
        public string? PostalCodePattern { get; set; }

        [StringLength(200)]
        public string? AddressFormatHint { get; set; }

        // Section 5: Status & Governance
        [Required(ErrorMessage = "Status is required")]
        public string? Status { get; set; } = "Draft";
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public int SortOrder { get; set; } = 0;

        [StringLength(500)]
        public string? Notes { get; set; }

        // Section 6: System Audit
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
    }

}
