using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    // GM3: TimeZone Model
    public class TimeZoneModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

        // Section 1: Time Zone Identity
        [Required(ErrorMessage = "Time Zone ID is required")]
        [StringLength(60)]
        public string TimeZoneKey { get; set; } = string.Empty;

        [Required(ErrorMessage = "Display Name is required")]
        [StringLength(120)]
        public string DisplayName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? ShortName { get; set; }

        // Section 2: Country Linkage
        public Guid? CountryId { get; set; }
        public string? CountryName { get; set; }

        // Section 3: Offset & DST Metadata
        public int StandardUtcOffsetMinutes { get; set; }
        public bool SupportsDST { get; set; }

        [StringLength(200)]
        public string? DSTRuleNote { get; set; }

        // Section 4: Display & Formatting
        public int SortOrder { get; set; } = 0;
        public bool IsDefaultRecommended { get; set; }

        // Section 5: Usage Controls & Defaults
        [Required(ErrorMessage = "Status is required")]
        public string? Status { get; set; } = "Draft";
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        [StringLength(300)]
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
