using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    // GM4: City Model
    public class CityModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

        // Section 1: City Identity
        [Required(ErrorMessage = "City Code is required")]
        [StringLength(15)]
        [RegularExpression("^[A-Za-z0-9_-]+$", ErrorMessage = "Letters, numbers, - and _ allowed")]
        public string CityCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "City Name is required")]
        [StringLength(150)]
        public string CityName { get; set; } = string.Empty;

        [StringLength(200)]
        public string? DisplayName { get; set; }

        // Section 2: Location Hierarchy
        [Required(ErrorMessage = "Country is required")]
        public Guid CountryId { get; set; }
        public string? CountryName { get; set; }

        [Required(ErrorMessage = "State/Province is required")]
        public Guid StateProvinceId { get; set; }
        public string? StateProvinceName { get; set; }

        // Section 3: Postal & Operational Hints
        [StringLength(100)]
        public string? DefaultPostalCodePattern { get; set; }

        public bool IsMetro { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        // Section 4: Status & Governance
        [Required(ErrorMessage = "Status is required")]
        public string? Status { get; set; } = "Draft";
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
