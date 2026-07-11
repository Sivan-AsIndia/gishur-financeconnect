using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    public class PostingRuleSeed
    {
        private readonly List<PostingRuleModel> _store = new();
        // SEED Data
        public void Seed(Guid profileId, Guid? companyId, Guid tenantId)
        {
            _store.AddRange(new[]
            {
                new PostingRuleModel
                {
                    TenantId = tenantId,
                    CompanyId = companyId,
                    PostingProfileId = profileId,

                    RuleCode = "EXPENSE_TO_AP_CONTROL",
                    RuleName = "Expense → AP Control",
                    Priority = 10,

                    MatchPostingCategory = PostingCategory.EXPENSE,
                    MatchLineType = LineType.PRINCIPAL,

                    AmountBasis = AmountBasis.BaseCurrencyLineAmount,

                    DebitAccountSourceType = AccountSourceRuleType.FromPostingCategoryMapping,
                    DebitAccountMappingKey = "EXPENSE_DEFAULT",

                    CreditAccountSourceType = AccountSourceRuleType.FromPartyControlAccount,

                    RequirePartyLink = true,
                    IsActive = true,
                    CreatedBy = "system"
                },
                new PostingRuleModel
                {
                    TenantId = tenantId,
                    CompanyId = companyId,
                    PostingProfileId = profileId,

                    RuleCode = "GST_INPUT_TO_AP_CONTROL",
                    RuleName = "GST Input → AP Control",
                    Priority = 20,

                    MatchPostingCategory = PostingCategory.TAX_INPUT,
                    MatchIsTaxLine = true,

                    AmountBasis = AmountBasis.BaseCurrencyLineAmount,

                    DebitAccountSourceType = AccountSourceRuleType.FromTaxCodeMapping,
                    DebitAccountMappingKey = "GST_INPUT",

                    CreditAccountSourceType = AccountSourceRuleType.FromPartyControlAccount,

                    RequirePartyLink = true,
                    IsActive = true,
                    CreatedBy = "system"
                }
            });
        }
    }
}
