using FinanceConnect.Client.ViewModels;
using static FinanceConnect.Client.Data.MasterDataIds;

namespace FinanceConnect.Client.Services
{
    public class PostingRuleService
    {
        private readonly COADataService _coa;
        private static List<PostingRuleModel> _postingRule = new();
        private static List<PostingRuleModel> _seedPostingRule = new();


        public PostingRuleService(COADataService coa)
        {
            _coa = coa;
        }

        private static List<T> CloneList<T>(IEnumerable<T> source) where T : class
        {
            var json = System.Text.Json.JsonSerializer.Serialize(source);
            return System.Text.Json.JsonSerializer.Deserialize<List<T>>(json) ?? new();
        }

        public void ResetToSeed()
        {
            _postingRule = CloneList(_seedPostingRule);
        }

        // GET
        public List<PostingRuleModel> GetByProfile(Guid profileId)
        {
            return _postingRule
                .Where(r => r.PostingProfileId == profileId)
                .OrderBy(r => r.Priority)
                .ToList();
        }

        public PostingRuleModel? GetById(Guid ruleId)
        {
            return _postingRule.FirstOrDefault(r => r.PostingRuleId == ruleId);
        }


        // CREATE
        public void Create(PostingRuleModel model)
        {
            Validate(model);
            model.CreatedAt = DateTime.UtcNow;
            _postingRule.Add(model);
        }

        // UPDATE
        public void Update(PostingRuleModel model)
        {
            var existing = GetById(model.PostingRuleId);
            if (existing == null)
                throw new Exception("Posting rule not found");

            Validate(model, isEdit: true);

            model.UpdatedAt = DateTime.UtcNow;

            _postingRule.Remove(existing);
            _postingRule.Add(model);
        }


        // ACTIVATE / DEACTIVATE
        public void Activate(Guid ruleId)
        {
            var rule = GetById(ruleId);
            if (rule == null) return;

            rule.IsActive = true;
            rule.UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate(Guid ruleId)
        {
            var rule = GetById(ruleId);
            if (rule == null) return;

            rule.IsActive = false;
            rule.UpdatedAt = DateTime.UtcNow;
        }


        // DELETE (Soft Rule)
        public void Delete(Guid ruleId)
        {
            var rule = GetById(ruleId);
            if (rule == null) return;

            _postingRule.Remove(rule);
        }

        // VALIDATION (ENTERPRISE RULES)
        private async void Validate(PostingRuleModel model, bool isEdit = false)
        {
            // ================================
            // A) MASTER INTEGRITY VALIDATIONS
            // ================================

            // 1. PostingProfileId required
            if (model.PostingProfileId == Guid.Empty)
                throw new Exception("Posting Profile is required.");

            // 2. RuleCode required
            if (string.IsNullOrWhiteSpace(model.RuleCode))
                throw new Exception("Rule Code is required.");

            // 2b. RuleCode unique within Tenant + Company + Profile
            bool duplicate = _postingRule.Any(r =>
                r.TenantId == model.TenantId &&
                r.CompanyId == model.CompanyId &&
                r.PostingProfileId == model.PostingProfileId &&
                r.RuleCode.Equals(model.RuleCode, StringComparison.OrdinalIgnoreCase) &&
                (!isEdit || r.PostingRuleId != model.PostingRuleId));

            if (duplicate)
                throw new Exception("Rule Code must be unique within this Posting Profile.");

            // 3. Priority >= 1
            if (model.Priority < 1)
                throw new Exception("Priority must be greater than or equal to 1.");

            // 4. AmountBasis required
            if (model.AmountBasis == default)
                throw new Exception("Amount Basis is required.");

            // 5. Account source types required
            //if (model.DebitAccountSourceType == default)
            //    throw new Exception("Debit Account Source Type is required.");

            //if (model.CreditAccountSourceType == default)
            //    throw new Exception("Credit Account Source Type is required.");

            if (model.DebitAccountSourceType == AccountSourceRuleType.FixedAccount &&
                !model.DebitAccountId.HasValue)
                throw new Exception("Debit Account must be selected for Fixed Account source.");

            if (model.CreditAccountSourceType == AccountSourceRuleType.FixedAccount &&
                !model.CreditAccountId.HasValue)
                throw new Exception("Credit Account must be selected for Fixed Account source.");


            var debitAccount = await ResolveAccountAsync(model, isDebit: true);
            var creditAccount = await ResolveAccountAsync(model, isDebit: false);

            ValidateAccountIntegrity(debitAccount, "Debit", model.CompanyId);
            ValidateAccountIntegrity(creditAccount, "Credit", model.CompanyId);


            bool isCatchAll =
                model.MatchPostingCategory == null &&
                model.MatchLineType == null &&
                model.MatchIsTaxLine == null &&
                model.MatchSourceModule == null;

            if (isCatchAll)
            {
                bool existsCatchAll = _postingRule.Any(r =>
                    r.PostingProfileId == model.PostingProfileId &&
                    r.PostingRuleId != model.PostingRuleId &&
                    r.MatchPostingCategory == null &&
                    r.MatchLineType == null &&
                    r.MatchIsTaxLine == null &&
                    r.MatchSourceModule == null);

                if (existsCatchAll)
                    throw new Exception("Only one catch-all rule is allowed per Posting Profile.");
            }
        }

        private async Task<AccountViewModel> ResolveAccountAsync(PostingRuleModel model, bool isDebit)
        {
            var accountId = isDebit ? model.DebitAccountId : model.CreditAccountId;

            if (!accountId.HasValue)
                throw new Exception($"{(isDebit ? "Debit" : "Credit")} account could not be resolved.");

            var account = await _coa.GetAccountByIdAsync(accountId.Value);

            if (account == null)
                throw new Exception($"{(isDebit ? "Debit" : "Credit")} account not found.");

            return account;
        }



        private void ValidateAccountIntegrity(AccountViewModel account, string side, Guid? companyId)
        {
            if (!account.IsActive)
                throw new Exception($"{side} account is inactive.");

            if (!account.IsPostable)
                throw new Exception($"{side} account is not postable.");
            var accounts = _coa.GetAllAccounts()
            .FirstOrDefault(x => x.Id == account.Id)
            ?? throw new Exception("Invalid account");
            //if (account.CompanyId != companyId)
            //    throw new Exception($"{side} account does not belong to the same company.");
        }


        // SEED Data
        public void Seed(Guid profileId, Guid? companyId, Guid tenantId)
        {
            _postingRule.AddRange(new[]
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
                    MatchLineType = LineType.CHARGE,

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

            _seedPostingRule = CloneList(_postingRule);
        }
    }
}
