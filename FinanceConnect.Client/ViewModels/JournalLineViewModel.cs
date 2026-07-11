using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public class JournalLineModel
    {

        public Guid Id { get; set; }

        [Required]
        public Guid JournalEntryId { get; set; }

        [Required]
        public int LineNumber { get; set; }


        [Required]
        public Guid? BranchId { get; set; }

        [Required(ErrorMessage = "Account is required")]
        public Guid? AccountId { get; set; }

        [Range(0, double.MaxValue)]
        public decimal DebitAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal CreditAmount { get; set; }


        [MaxLength(500)]
        public string? LineNarration { get; set; }

        public DrCrIndicator DrCrIndicator =>
            DebitAmount > 0 ? DrCrIndicator.Debit :
            CreditAmount > 0 ? DrCrIndicator.Credit :
            DrCrIndicator.None;

        public decimal Amount => DebitAmount + CreditAmount;

        public Guid? BaseCurrencyId { get; set; }

        public Guid? CostCenterId { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? DepartmentId { get; set; }

        public PartyType? PartyType { get; set; }
        public Guid? PartyId { get; set; }

        public Guid? TaxCodeId { get; set; }

        [MaxLength(100)]
        public string? ReferenceText { get; set; }

        public bool IsSystemGenerated { get; set; } = false;

        public Guid TenantId { get; set; }

        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;

        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public byte[]? RowVersion { get; set; }

        public bool IsDeleted { get; set; } = false;
    }

    public class AccountLookup
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsPostable { get; set; }
        public bool IsActive { get; set; }
        public string DisplayName => $"{Code} - {Name}";
    }
    public enum DrCrIndicator
    {
        None = 0,
        Debit = 1,
        Credit = 2
    }

    public enum PartyType
    {
        Customer,
        Vendor,
        Employee,
        Other
    }

}
