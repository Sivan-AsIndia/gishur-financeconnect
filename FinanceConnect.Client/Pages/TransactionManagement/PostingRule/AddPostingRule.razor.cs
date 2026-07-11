using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Linq.Expressions;
using static FinanceConnect.Client.Data.MasterDataIds;

namespace FinanceConnect.Client.Pages.TransactionManagement.PostingRule
{
    public partial class AddPostingRule
    {
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("initFeatherIcons");
        }
        private EditContext _editContext = default!;
        [Inject] PostingRuleService RuleService { get; set; } = default!;
        [Inject] PostingProfileService ProfileService { get; set; } = default!;
        [Inject] TransactionTypeService TransactionTypeService { get; set; } = default!;
        [Inject] COADataService COADataService { get; set; } = default!;
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;

        [Parameter] public Guid ProfileId { get; set; }
        [Parameter] public Guid? Id { get; set; }

        PostingRuleModel rule = new();

        bool ShowIdentity = true;
        bool ShowMatch = false;
        bool ShowAmount = false;
        bool ShowAccounts = false;
        bool ShowOutput = false;
        bool ShowStatus = false;

        bool IsEdit => Id.HasValue;

        bool IdentityTouched = false;
        bool MatchTouched = false;
        bool AmountTouched = false;
        bool AccountsTouched = false;
        bool OutputTouched = false;
        bool StatusTouched = false;
        void TouchIdentity() => IdentityTouched = true;
        void TouchMatch() => MatchTouched = true;
        void TouchAmount() => AmountTouched = true;
        void TouchAccounts() => AccountsTouched = true;
        void TouchStatus() => StatusTouched = true;
        void TouchOutput() => OutputTouched = true;

        string PageTitle => IsEdit ? "Edit Posting Rule" : "Create Posting Rule";
        string PageSubTitle => IsEdit
            ? "Modify rule logic and posting behavior"
            : "Define how transactions generate journal lines";

        string PostingProfileName="-";
        List<TransactionTypeModel> TransactionTypes = new();

        // ACCOUNT LOOKUPS (MOCK DATA)
        List<AccountViewModel> Accounts = new();
 


        protected override async Task OnInitializedAsync()
        {
            PostingProfileName = ProfileService.GetById(ProfileId)?.ProfileName ?? "-";
            var PostingProfile = ProfileService.GetById(ProfileId);
            var charts = await COADataService.GetChartOfAccountsAsync();

            var chartIds = charts
                .Where(c => c.CompanyId == PostingProfile.CompanyId)
                .Select(c => c.Id)
                .ToList();

            Accounts = COADataService
                .GetAllAccounts()
                .Where(a => a.ChartOfAccountsId.HasValue &&
                            chartIds.Contains(a.ChartOfAccountsId.Value))
                .ToList();
            TransactionTypes = TransactionTypeService.GetAll();
            if (IsEdit)
            {
                var existing = RuleService.GetById(Id!.Value);
                if (existing != null)
                {
                    rule = new PostingRuleModel
                    {
                        PostingRuleId = existing.PostingRuleId,
                        PostingProfileId = existing.PostingProfileId,
                        TenantId = existing.TenantId,
                        CompanyId = existing.CompanyId,

                        RuleCode = existing.RuleCode,
                        RuleName = existing.RuleName,
                        Priority = existing.Priority,
                        StopProcessingAfterMatch = existing.StopProcessingAfterMatch,

                        MatchPostingCategory = existing.MatchPostingCategory,
                        MatchLineType = existing.MatchLineType,
                        MatchIsTaxLine = existing.MatchIsTaxLine,
                        MatchSourceModule = existing.MatchSourceModule,
                        MatchAmountSign = existing.MatchAmountSign,

                        AmountBasis = existing.AmountBasis,
                        AmountMultiplier = existing.AmountMultiplier,
                        MinimumAmount = existing.MinimumAmount,
                        MaximumAmount = existing.MaximumAmount,

                        DebitAccountSourceType = existing.DebitAccountSourceType,
                        DebitAccountMappingKey = existing.DebitAccountMappingKey,
                        DebitAccountId = existing.DebitAccountId,

                        CreditAccountSourceType = existing.CreditAccountSourceType,
                        CreditAccountMappingKey = existing.CreditAccountMappingKey,
                        CreditAccountId = existing.CreditAccountId,

                        IsActive = existing.IsActive,
                        EffectiveFrom = existing.EffectiveFrom,
                        EffectiveTo = existing.EffectiveTo,

                        CreatedAt = existing.CreatedAt,
                        UpdatedAt = existing.UpdatedAt
                    };
                }
            }
            else
            {
                rule = new PostingRuleModel
                {
                    PostingProfileId = ProfileId,
                    Priority = 10,
                    AmountBasis = AmountBasis.BaseCurrencyLineAmount,
                    AmountMultiplier = 1,
                    IsActive = true,
                    MatchAmountSign = AmountSignMode.Any
                };
            }

            _editContext = new EditContext(rule);
        }

        string RuleCodeInput
        {
            get => rule.RuleCode;
            set => rule.RuleCode = value?.Trim().ToUpperInvariant() ?? "";
        }

        private void OnNameChange(){
            rule.RuleName = rule.RuleName?.Trim() ?? "";
        }

        private async Task HandleSubmit()
        {
            if (_editContext.Validate())
            {
                await ContinueSave();
                return;
            }

            if (HasIdentityErrors())
                OpenAccordion("identity");
            //else if (HasMatchErrors())
            //    OpenAccordion("match");
            //else if (HasAmountErrors())
            //    OpenAccordion("amount");
            else if (HasAccountErrors())
                OpenAccordion("accounts");

            await InvokeAsync(StateHasChanged);
        }


        private string GetValidationClass(Expression<Func<object>> field)
        {
            if (_editContext == null)
                return string.Empty;

            var fieldIdentifier = FieldIdentifier.Create(field);

            var hasError = _editContext.GetValidationMessages(fieldIdentifier).Any();
            var isModified = _editContext.IsModified(fieldIdentifier);

            if (hasError)
                return "is-invalid";

            if (isModified)
                return "is-valid";

            return string.Empty;
        }




        void ToggleAccordion(string section)
        {
            switch (section)
            {
                case "identity":
                    ShowIdentity = !ShowIdentity;
                    break;
                case "match":
                    ShowMatch = !ShowMatch;
                    break;
                case "amount":
                    ShowAmount = !ShowAmount;
                    break;
                case "accounts":
                    ShowAccounts = !ShowAccounts;
                    break;
                case "status":
                    ShowStatus = !ShowStatus;
                    break;
                case "output":
                    ShowOutput = !ShowOutput;
                    break;
            }
        }

        void OpenAccordion(string section)
        {
            switch (section)
            {
                case "identity":
                    ShowIdentity = true;
                    break;
                case "match":
                    ShowMatch = true;
                    break;
                case "amount":
                    ShowAmount = true;
                    break;
                case "accounts":
                    ShowAccounts = true;
                    break;
                case "output":
                    ShowOutput = true;
                    break;
                case "status":
                    ShowStatus = true;
                    break;
            }
        }


        bool HasIdentityErrors()
        {
            return string.IsNullOrWhiteSpace(rule.RuleCode)
                || string.IsNullOrWhiteSpace(rule.RuleName)
                || rule.Priority < 1
                || rule.PostingProfileId == Guid.Empty;
        }

        bool HasMatchErrors()
        {
            return rule.MatchPostingCategory == null
                && rule.MatchLineType == null
                && rule.MatchIsTaxLine == null
                && rule.MatchSourceModule == null;
        }

        public bool IsValidMultiplier()
        {
            // Allow normal positive splits and controlled negative reversal
            return rule.AmountMultiplier != 0 &&
                   rule.AmountMultiplier >= -1.0000m &&
                   rule.AmountMultiplier <= 9.9999m;
        }
        bool HasAmountErrors()
        {
            if (rule.AmountBasis == 0)
                return true;

            // Multiplier should never be zero
            if (rule.AmountMultiplier == 0)
                return true;

            if (rule.MinimumAmount.HasValue &&
                rule.MaximumAmount.HasValue &&
                rule.MinimumAmount > rule.MaximumAmount)
                return true;

            return false;
        }

        bool HasAccountErrors()
        {
            // Debit validation
            if (rule.DebitAccountSourceType == AccountSourceRuleType.FixedAccount &&
                rule.DebitAccountId == null)
                return true;

            if (rule.DebitAccountSourceType != AccountSourceRuleType.FixedAccount &&
                string.IsNullOrWhiteSpace(rule.DebitAccountMappingKey) &&
                rule.DebitAccountId==Guid.Empty && rule.CreditAccountId == Guid.Empty)
                return true;

            // Credit validation
            if (rule.CreditAccountSourceType == AccountSourceRuleType.FixedAccount &&
                rule.CreditAccountId == null)
                return true;

            if (rule.CreditAccountSourceType != AccountSourceRuleType.FixedAccount &&
                string.IsNullOrWhiteSpace(rule.CreditAccountMappingKey))
                return true;

            return false;
        }


        async Task ContinueSave()
        {
            try
            {
                if (IsEdit)
                {
                    RuleService.Update(rule);
                    ToastService.ShowSuccess($"Rule '{rule.RuleName}' updated");
                }
                else
                {
                    RuleService.Create(rule);
                    ToastService.ShowSuccess($"Rule '{rule.RuleName}' created");
                }

                Nav.NavigateTo($"/posting-profiles/{ProfileId}/rules");
            }
            catch (Exception ex)
            {
                ToastService.ShowError(ex.Message);
            }
        }


        void BackToList()
        {
            ShowIdentity = true;
            ShowMatch = false;
            ShowAmount = false;
            ShowAccounts = false;
            ShowOutput = false;
            ShowStatus = false;

            IdentityTouched = false;
            MatchTouched = false;
            AmountTouched = false;
            AccountsTouched = false;
            OutputTouched = false;
            StatusTouched = false;
            Nav.NavigateTo($"/posting-profiles/{ProfileId}/rules");
        }
    }
}
