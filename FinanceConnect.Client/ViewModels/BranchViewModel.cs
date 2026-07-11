using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{

    public static class BranchTypes
    {
        public const string HeadOffice = "Head Office";
        public const string RegionalOffice = "Regional Office";
        public const string Factory = "Factory";
        public const string Warehouse = "Warehouse";
        public const string RetailOutlet = "Retail Outlet";
        public const string ProjectSite = "Project Site";
        public static readonly string[] All = new[] { HeadOffice, RegionalOffice, Factory, Warehouse, RetailOutlet, ProjectSite };
    }

    public class DashboardViewModel
    {
        public Guid? SelectedCompanyId { get; set; }
        public Guid? SelectedBranchId { get; set; }
    }

        public class BranchModel
    {
        public Guid Id { get; set; }

        // ---------------- Identity ----------------

        [Required(ErrorMessage = "Branch Code is required")]
        [StringLength(15)]
        [RegularExpression("^[A-Z0-9_-]+$", ErrorMessage = "Only letters, numbers, - and _ allowed")]
        public string BranchCode { get; set; } = "";

        [Required(ErrorMessage = "Branch Name is required")]
        [StringLength(200)]
        public string BranchName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Company is required")]
        public string Company { get; set; } = string.Empty;

        public Guid CompanyId { get; set; }

        [Required(ErrorMessage = "Branch Type is required")]
        [StringLength(50)]
        public string BranchType { get; set; } = string.Empty;

        public Guid? ParentBranchId { get; set; }
        public bool IsDefaultBranch { get; set; }

        // ---------------- Address ----------------

        [Required]
        [StringLength(200)]
        public string AddressLine1 { get; set; } = string.Empty;

        [StringLength(200)]
        public string? AddressLine2 { get; set; }

        [Required(ErrorMessage = "Country Type is required")]
        public Guid CountryId { get; set; }

        [Required(ErrorMessage = "State is required")]
        public Guid StateId { get; set; }

        [Required(ErrorMessage = "City is required")]
        public Guid CityId { get; set; }

        [Required(ErrorMessage = "Postal code is required")]
        [StringLength(20, MinimumLength = 3,
            ErrorMessage = "Postal code must be between 3 and 20 characters")]
        [RegularExpression(@"^[A-Za-z0-9][A-Za-z0-9\s\-]*[A-Za-z0-9]$",
            ErrorMessage = "Postal code can contain letters, numbers, spaces and hyphens only")]
        public string PostalCode { get; set; } = string.Empty;


        public Guid? TimeZoneId { get; set; }

        // ---------------- Contact ----------------

        [StringLength(100)]
        public string? BranchManagerName { get; set; }

        [EmailAddress]
        [StringLength(150)]
        public string? BranchEmail { get; set; }

        [StringLength(30, ErrorMessage = "Phone number cannot exceed 30 characters")]
        [RegularExpression(@"^\+?[0-9\s\-\(\)]{7,30}$",
            ErrorMessage = "Enter a valid phone number (digits, +, -, spaces, () only)")]
        public string? BranchPhone { get; set; }


        [StringLength(300)]
        public string? OperatingHoursNote { get; set; }

        // ---------------- Posting Controls ----------------

        public DateTime? BooksStartDate { get; set; }
        public DateTime? AllowPostingFrom { get; set; } = DateTime.Today;
        public DateTime? AllowPostingTo { get; set; }

        public bool LockBackDatedPosting { get; set; }

        [Range(0, 365, ErrorMessage = "Backdated posting days cannot be negative and must be between 0 and 365.")]
        public int? BackdatedPostingDaysAllowed { get; set; }

        [Range(0, 365, ErrorMessage = "Future posting days cannot be negative and must be between 0 and 365.")]
        public int? FuturePostingDaysAllowed { get; set; }

        // ---------------- Reporting ----------------

        public bool IsReportingEnabled { get; set; } = true;

        [Range(0, int.MaxValue, ErrorMessage = "Report sort order cannot be negative.")]
        public int? ReportSortOrder { get; set; }

        [StringLength(20)]
        public string? BranchColorTag { get; set; }

        // ---------------- Status ----------------

        [Required]
        public string Status { get; set; } = "Draft";

        [StringLength(1000)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    public class BranchLookup
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid? CompanyId { get; set; }
    }
}
