using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using System;
using System.Collections.Generic;

namespace FinanceConnect.Client.Data
{
    public class PostingProfileSeed
    {
        private readonly PostingRuleService _ruleService;

        public PostingProfileSeed(PostingRuleService ruleService)
        {
            _ruleService = ruleService;
        }

        public List<PostingProfileModel> SeedForCompanies(List<CompanyModel> companies)
        {
            var list = new List<PostingProfileModel>();
            var now = DateTime.UtcNow;

            foreach (var company in companies)
            {
                var suffix = company.CompanyCode ?? company.Id.ToString("N")[..6];

                var profile = new PostingProfileModel
                {
                    PostingProfileId = Guid.NewGuid(),
                    TenantId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    CompanyId = company.Id,

                    ProfileCode = $"BR_{suffix}",
                    ProfileName = $"BR Posting - {company.CompanyCode}",
                    Description = $"Posts incoming payments to bank and revenue/AR accounts for {company.LegalName}",

                    AggregationMode = AggregationMode.PerTransactionLine,
                    RuleApplicationMode = RuleApplicationMode.FirstMatchOnly,
                    RequireCompleteRuleCoverage = true,
                    AllowCatchAllRule = false,
                    BalanceValidationMode = BalanceValidationMode.StrictBalanced,

                    RoundingPolicyMode = RoundingPolicyMode.CreateRoundingAdjustmentLine,
                    RoundingAccountSourceType = AccountSourceType.FixedAccount,
                    FxGainLossPolicyMode = FxGainLossPolicyMode.PostFxGainLossToConfiguredAccounts,

                    MappingScopeMode = MappingScopeMode.CompanyWide,

                    IsActive = true,
                    IsSystemDefined = true,

                    EffectiveFrom = now.AddMonths(-3),

                    RuleCount = 2,
                    UsageCount = Random.Shared.Next(1, 5),

                    CreatedAt = now,
                    CreatedBy = "seed"
                };

                list.Add(profile);

                // ⭐ Seed rules immediately for this profile
                _ruleService.Seed(
                    profileId: profile.PostingProfileId,
                    companyId: profile.CompanyId,
                    tenantId: profile.TenantId
                );
            }

            return list;
        }
    }

}
