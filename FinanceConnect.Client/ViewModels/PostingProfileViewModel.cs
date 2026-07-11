using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceConnect.Client.ViewModels
{

    public enum AggregationMode
    {
        PerTransactionLine = 1,
        AggregateByAccount = 2,
        AggregateByPostingCategory = 3
    }

    public enum RuleApplicationMode
    {
        FirstMatchOnly = 1,
        MultipleMatchesAllowed = 2
    }

    public enum BalanceValidationMode
    {
        StrictBalanced = 1,
        AllowAutoBalancingWithRounding = 2
    }

    // ==========================
    // Rounding & FX
    // ==========================

    public enum RoundingPolicyMode
    {
        NoRoundingLines = 1,
        CreateRoundingAdjustmentLine = 2
    }

    public enum FxGainLossPolicyMode
    {
        IgnoreFxGainLoss = 1,
        PostFxGainLossToConfiguredAccounts = 2
    }

    // ==========================
    // Account Resolution
    // ==========================

    public enum AccountSourceType
    {
        FixedAccount,
        FromPostingCategoryMapping,
        FromTaxCodeMapping,
        FromPartyControlAccount,
        FromTransactionTypeDefault
    }

    // ==========================
    // Mapping Scope
    // ==========================

    public enum MappingScopeMode
    {
        CompanyWide = 1,
        BranchSpecific = 2
    }

    public enum DefaultControlAccountPolicy
    {
        ResolveFromPartyMaster = 1,
        ResolveFromProfileDefaults = 2
    }

    public class PostingProfileModel
    {
        // ======================
        // Identity & Ownership
        // ======================
        [Key]
        public Guid PostingProfileId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid TenantId { get; set; }

        [Required]
        public Guid? CompanyId { get; set; }

        [Required, StringLength(50)]
        [RegularExpression("^[A-Z0-9_-]+$", ErrorMessage = "Only letters, numbers, - and _ allowed")]
        public string ProfileCode { get; set; } = string.Empty;

        [Required, StringLength(150)]
        public string ProfileName { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Aggregation Mode is required")]
        public AggregationMode AggregationMode { get; set; } = AggregationMode.PerTransactionLine;

        [Required]
        public RuleApplicationMode RuleApplicationMode { get; set; } = RuleApplicationMode.FirstMatchOnly;

        [Required]
        public bool RequireCompleteRuleCoverage { get; set; } = true;

        [Required]
        public bool AllowCatchAllRule { get; set; } = false;

        [Required]
        public BalanceValidationMode BalanceValidationMode { get; set; } = BalanceValidationMode.StrictBalanced;


        [Required]
        public RoundingPolicyMode RoundingPolicyMode { get; set; } = RoundingPolicyMode.CreateRoundingAdjustmentLine;

        public AccountSourceType? RoundingAccountSourceType { get; set; }

        [StringLength(100)]
        public string? RoundingAccountId { get; set; }

        [StringLength(100)]
        public string? RoundingMappingKey { get; set; }

        [Required]
        public FxGainLossPolicyMode FxGainLossPolicyMode { get; set; } =
            FxGainLossPolicyMode.PostFxGainLossToConfiguredAccounts;

        public AccountSourceType? FxGainAccountSourceType { get; set; }

        [StringLength(100)]
        public string? FxGainAccountId { get; set; }

        [StringLength(100)]
        public string? FxGainMappingKey { get; set; }

        public AccountSourceType? FxLossAccountSourceType { get; set; }

        [StringLength(100)]
        public string? FxLossAccountId { get; set; }

        [StringLength(100)]
        public string? FxLossMappingKey { get; set; }


        [Required]
        public bool EnableCategoryToAccountMapping { get; set; } = true;

        [Required]
        public MappingScopeMode MappingScopeMode { get; set; } = MappingScopeMode.CompanyWide;

        public DefaultControlAccountPolicy? DefaultControlAccountPolicy { get; set; }

        [StringLength(500)]
        public string? JournalNarrationTemplate { get; set; }

        [StringLength(500)]
        public string? LineNarrationTemplate { get; set; }

        [Required]
        public bool IncludeSourceDocumentNoInNarration { get; set; } = true;


        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public bool IsSystemDefined { get; set; } = false;

        [DataType(DataType.Date)]
        public DateTime? EffectiveFrom { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EffectiveTo { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [StringLength(100)]
        public string? UpdatedBy { get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        [NotMapped]
        public int RuleCount { get; set; }

        [NotMapped]
        public int UsageCount { get; set; }

        [NotMapped]
        public bool IsExpired =>
            EffectiveTo.HasValue && EffectiveTo.Value.Date < DateTime.UtcNow.Date;
    }
}
