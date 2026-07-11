using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace FinanceConnect.Client.ViewModels
{

    public enum AmountBasis
    {
        TransactionCurrencyLineAmount,
        BaseCurrencyLineAmount
    }

    public enum AccountSourceRuleType
    {
        FixedAccount,
        FromPostingCategoryMapping,
        FromTaxCodeMapping,
        FromPartyControlAccount,
        FromTransactionTypeDefault
    }

    public enum AmountSignMode
    {
        Any,
        Positive,
        Negative
    }
    public enum PostingCategory
    {
        EXPENSE,
        TAX_INPUT,
        BANK
    }
    public enum LineType
    {
        PRINCIPAL,
        TAX,
        CHARGE
    }
    public enum SourceModuleType
    {
        AP,
        AR,
        BANK,
        EXPENSE
    }
    public class PostingRuleModel
    {
        // ======================
        // Identity & Parenting
        // ======================
        public Guid PostingRuleId { get; set; } = Guid.NewGuid();
        public Guid TenantId { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid PostingProfileId { get; set; }

        // ======================
        // Rule Identity & Ordering
        // ======================
        [Required, MaxLength(50)]
        public string RuleCode { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string RuleName { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Priority { get; set; } = 0;

        public bool StopProcessingAfterMatch { get; set; } = false;

        // ======================
        // Match Criteria
        // ======================
        public PostingCategory? MatchPostingCategory { get; set; }
        public LineType? MatchLineType { get; set; }
        public bool? MatchIsTaxLine { get; set; }
        public SourceModuleType? MatchSourceModule { get; set; }
        public Guid? MatchTransactionTypeId { get; set; }
        public AmountSignMode MatchAmountSign { get; set; } = AmountSignMode.Any;

        // ======================
        // Amount Basis
        // ======================
        public AmountBasis AmountBasis { get; set; } = AmountBasis.TransactionCurrencyLineAmount;
        public decimal AmountMultiplier { get; set; } = 1.0000m;
        public decimal? MinimumAmount { get; set; }
        public decimal? MaximumAmount { get; set; }

        // ======================
        // Debit Resolution
        // ======================
        public AccountSourceRuleType DebitAccountSourceType { get; set; }
        [Required(ErrorMessage = "DebitAccount is required")]
        public Guid? DebitAccountId { get; set; }
        public string? DebitAccountMappingKey { get; set; }

        // ======================
        // Credit Resolution
        // ======================
        public AccountSourceRuleType CreditAccountSourceType { get; set; }

        [Required(ErrorMessage = "CreditAccount is required")]
        public Guid? CreditAccountId { get; set; }
        public string? CreditAccountMappingKey { get; set; }

        // ======================
        // Output Enrichment
        // ======================
        [MaxLength(500)]
        public string? OutputNarrationTemplate { get; set; }

        public bool RequirePartyLink { get; set; } = false;
        public string? DimensionPolicyKey { get; set; }

        // ======================
        // Effective Control & Status
        // ======================
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsSystemDefined { get; set; } = false;

        // ======================
        // Audit
        // ======================
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[]? RowVersion { get; set; }
    }
}
