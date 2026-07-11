using FinanceConnect.Client.Services;
using FinanceConnect.Client.Shared;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Linq.Expressions;
using System.Transactions;

namespace FinanceConnect.Client.Pages.TransactionManagement.PostingProfile
{
    public partial class AddPostingProfile
    {

            private EditContext _editContext = default!;
            RichTextEditor? _descriptionEditor;

            [Inject] PostingProfileService ProfileService { get; set; } = default!;
            [Inject] NavigationManager Nav { get; set; } = default!;
            [Inject] IJSRuntime JS { get; set; } = default!;
            [Inject] ToastService ToastService { get; set; } = default!;

            protected override async Task OnAfterRenderAsync(bool firstRender)
            {
                await JS.InvokeVoidAsync("initFeatherIcons");
            }

            [Parameter] public Guid? Id { get; set; }

            PostingProfileModel profile = new();


            bool IdentityTouched = false;
            bool BehaviorTouched = false;
            bool FxTouched = false;
            bool NarrationTouched = false;
            bool StatusTouched = false;
            bool MappingTouched = false;


            bool ShowIdentity = true;
            bool ShowBehavior = false;
            bool ShowFx = false;
            bool ShowMapping = false;
            bool ShowNarration = false;
            bool ShowStatus = false; 

            void TouchIdentity() => IdentityTouched = true;
            void TouchBehavior() => BehaviorTouched = true;
            void TouchFx() => FxTouched = true;
            void TouchNarration() => NarrationTouched = true;
            void TouchStatus() => StatusTouched = true;
            void TouchedMapping() => MappingTouched = true;
        

            bool IsEdit => Id.HasValue;
            bool IsInitializing = true;

            string PageTitle => IsEdit ? "Edit Posting Profile" : "Create Posting Profile";
            string PageSubTitle => IsEdit
                ? "Update posting behavior and rule policies"
                : "Create a new accounting posting blueprint";

            List<CompanyModel> Companies = new();

            // CONFIRM MODAL
            string ConfirmTitle = "";
            string ConfirmMessage = "";
            string ConfirmType = "warning";
            Action? ConfirmAction;

            async Task OpenConfirmModal(
                string title,
                string message,
                Action yesAction,
                string type = "warning")
            {
                ConfirmTitle = title;
                ConfirmMessage = message;
                ConfirmType = type;
                ConfirmAction = yesAction;

                await JS.InvokeVoidAsync("bootstrapModal.show", "confirm-modal");
            }

            async Task ConfirmYes()
            {
                var action = ConfirmAction;
                ConfirmAction = null;

                await JS.InvokeVoidAsync("bootstrapModal.hide", "confirm-modal");
                await Task.Delay(150);

                action?.Invoke();
            }


            protected override void OnInitialized()
            {
            Companies = ProfileService.GetCompanies();
                if (IsEdit)
                {
                    var existing = ProfileService.GetById(Id!.Value);
                    if (existing != null)
                    {

                    profile = new PostingProfileModel
                        {
                            PostingProfileId = existing.PostingProfileId,
                            TenantId = existing.TenantId,
                            CompanyId = existing.CompanyId,

                            ProfileCode = existing.ProfileCode,
                            ProfileName = existing.ProfileName,
                            Description = existing.Description,

                            AggregationMode = existing.AggregationMode,
                            RuleApplicationMode = existing.RuleApplicationMode,
                            RequireCompleteRuleCoverage = existing.RequireCompleteRuleCoverage,
                            AllowCatchAllRule = existing.AllowCatchAllRule,
                            BalanceValidationMode = existing.BalanceValidationMode,

                            RoundingPolicyMode = existing.RoundingPolicyMode,
                            RoundingAccountSourceType = existing.RoundingAccountSourceType,
                            RoundingMappingKey = existing.RoundingMappingKey,

                            FxGainLossPolicyMode = existing.FxGainLossPolicyMode,
                            FxGainMappingKey = existing.FxGainMappingKey,
                            FxLossMappingKey = existing.FxLossMappingKey,

                            JournalNarrationTemplate = existing.JournalNarrationTemplate,
                            LineNarrationTemplate = existing.LineNarrationTemplate,
                            IncludeSourceDocumentNoInNarration = existing.IncludeSourceDocumentNoInNarration,

                            IsActive = existing.IsActive,
                            IsSystemDefined = existing.IsSystemDefined,
                            EffectiveFrom = existing.EffectiveFrom,
                            EffectiveTo = existing.EffectiveTo,

                            CreatedAt = existing.CreatedAt,
                            UpdatedAt = existing.UpdatedAt
                        };

                        IsInitializing = false;
                    }
                }
                else
                {
                    profile = new PostingProfileModel
                    {
                        IsActive = false,
                        RequireCompleteRuleCoverage = true,
                        EffectiveFrom = DateTime.Today,
                        //AggregationMode = AggregationMode.PerTransactionLine,
                        RuleApplicationMode = RuleApplicationMode.FirstMatchOnly,
                        BalanceValidationMode = BalanceValidationMode.StrictBalanced,
                        RoundingPolicyMode = RoundingPolicyMode.CreateRoundingAdjustmentLine,
                        FxGainLossPolicyMode = FxGainLossPolicyMode.PostFxGainLossToConfiguredAccounts,
                        IncludeSourceDocumentNoInNarration = true
                    };
                }

                _editContext = new EditContext(profile);
            }

        string ProfileCodeInput
        {
            get => profile.ProfileCode;
            set
            {
                profile.ProfileCode = value?.Trim().ToUpperInvariant() ?? "";
            }
        }


        void OnNameChanged()
        {
            profile.ProfileName = profile.ProfileName?.Trim() ?? "";
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

        private async Task HandleSubmit()
            {

            // Collect Quill editor values before validation
            if (_descriptionEditor != null)
                profile.Description = await _descriptionEditor.GetHtmlAsync();

            if (_editContext.Validate())
                {
                    await ValidateBusinessRules();
                    return;
                }

                if (HasIdentityErrors())
                    OpenAccordion("identity");
                else if (HasBehaviorErrors())
                    OpenAccordion("behavior");
                else if (HasNarrationErrors())
                    OpenAccordion("narration");

                await InvokeAsync(StateHasChanged);
            }

            void ToggleAccordion(string section)
            {
                switch (section)
                {
                    case "identity":
                        ShowIdentity = !ShowIdentity;
                        break;
                    case "behavior":
                        ShowBehavior = !ShowBehavior;
                        break;
                    case "fx":
                        ShowFx = !ShowFx;
                        break;
                    case "mapping":
                    ShowMapping = !ShowMapping;
                    break;
                    case "narration":
                        ShowNarration = !ShowNarration;
                        break;
                    case "status":
                        ShowStatus = !ShowStatus;
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
                case "behavior":
                    ShowBehavior = true;
                    break;
                case "fx":
                    ShowFx = true;
                    break;
                case "narration":
                    ShowNarration = true;
                    break;
                case "mapping":
                    ShowMapping = true;
                    break;
                case "status":
                    ShowStatus = true;
                    break;
            }
        }

        bool HasIdentityErrors()
            {
                return string.IsNullOrWhiteSpace(profile.ProfileCode)
                    || string.IsNullOrWhiteSpace(profile.ProfileName)
                    || profile.CompanyId == Guid.Empty;
            }

        bool HasBehaviorErrors()
        {
            return profile.AggregationMode == null
                || profile.RuleApplicationMode == null
                || profile.BalanceValidationMode == null;
        }


        bool HasFxErrors()
            {
                if (profile.RoundingPolicyMode == RoundingPolicyMode.CreateRoundingAdjustmentLine)
                {
                    return string.IsNullOrWhiteSpace(profile.RoundingMappingKey);
                }

                if (profile.FxGainLossPolicyMode == FxGainLossPolicyMode.PostFxGainLossToConfiguredAccounts)
                {
                    return string.IsNullOrWhiteSpace(profile.FxGainMappingKey)
                        || string.IsNullOrWhiteSpace(profile.FxLossMappingKey);
                }

                return false;
            }

            bool HasNarrationErrors()
            {
                return profile.JournalNarrationTemplate?.Length > 500
                    || profile.LineNarrationTemplate?.Length > 500;
            }

            // BUSINESS RULE VALIDATION
            async Task ValidateBusinessRules()
            {
                bool duplicate = ProfileService.GetAll()
                    .Any(p =>
                        p.CompanyId == profile.CompanyId &&
                        p.ProfileCode == profile.ProfileCode &&
                        p.PostingProfileId != profile.PostingProfileId);

                if (duplicate)
                {
                    await OpenConfirmModal(
                        "Duplicate Profile Code",
                        "Profile Code must be unique per company.",
                        null!,
                        "danger");
                    return;
                }

            if (profile.AllowCatchAllRule)
            {
                await OpenConfirmModal(
                    "Catch-All Rule Enabled",
                    "Allowing a catch-all rule may hide posting configuration errors. Do you want to continue?",
                    async () => await ContinueSave(),
                    "warning");
                return;
            }

            await ContinueSave();
            }

            // SAVE
            async Task ContinueSave()
            {
                try
                {
                if (profile.EffectiveFrom.HasValue && profile.EffectiveTo.HasValue)
                {
                    if (profile.EffectiveFrom > profile.EffectiveTo)
                    {
                        ToastService.ShowError("From Date cannot be greater than To Date.");
                        return;
                    }

                    if (profile.EffectiveTo < profile.EffectiveFrom)
                    {
                        ToastService.ShowError("To Date cannot be less than From Date.");
                        return;
                    }
                }
                if (IsEdit)
                    {
                        //if (profile.IsSystemDefined)
                        //{
                        //    ToastService.ShowError("System defined profiles cannot be modified.");
                        //    return;
                        //}

                        ProfileService.Update(profile);
                        ToastService.ShowSuccess($"Posting Profile '{profile.ProfileName}' updated successfully");
                    }
                    else
                    {
                        ProfileService.Create(profile);
                        ToastService.ShowSuccess($"Posting Profile '{profile.ProfileName}' created successfully");
                    }

                    Nav.NavigateTo("/posting-profiles");
                }
                catch (Exception ex)
                {
                    ToastService.ShowError(ex.Message);
                }
            }

            // NAVIGATION
            void BackToList()
            {
            profile = new PostingProfileModel();

                IdentityTouched = false;
                BehaviorTouched = false;
                FxTouched = false;
                NarrationTouched = false;
                StatusTouched = false;
                MappingTouched = false;

                ShowIdentity = true;
                ShowBehavior = false;
                ShowFx = false;
                ShowNarration = false;
                ShowStatus = false;

                _editContext = new EditContext(profile);

                Nav.NavigateTo("/posting-profiles");
            }
        }
    }

