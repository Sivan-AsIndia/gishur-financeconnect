using FinanceConnect.Client.Pages.Finance.Journal;
using FinanceConnect.Client.Services;
using FinanceConnect.Client.Shared;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq.Expressions;

namespace FinanceConnect.Client.Pages.TransactionManagement.TransactionType
{
    public partial class TransactionTypeForm
    {

        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] PostingProfileService ProfileService { get; set; } = default!;
        [Inject] DocumentNumberSeriesService DocNumSeriesService { get; set; } = default!;
        [Parameter] public Guid? Id { get; set; }

        TransactionTypeModel type = new();
        EditContext? _editContext;
        RichTextEditor? _descriptionEditor;

        public List<CompanyModel> Companies = new();
        List<PostingProfileModel> PostingProfiles = new();
        List<DocumentNumberSeriesModel> DocumentNumberSeries = new();
        DateTime today = DateTime.UtcNow.Date;
        bool IsEdit => Id.HasValue;
        bool IsInitializing = true;
        string PageTitle => IsEdit ? "Edit Transaction Type" : "Add Transaction Type";
        string PageSubTitle => IsEdit
            ? "Modify transaction policy and posting behavior"
            : "Define a new financial transaction policy";

        // Accordion state
        bool ShowIdentity = true;
        bool ShowClassification = false;
        bool ShowPosting = false;
        bool ShowDoc = false;
        bool ShowCurrency = false;
        bool ShowStatus = false;
        bool ShowPolicies = false;

        // Touch states
        bool IdentityTouched;
        bool ClassificationTouched;
        bool PostingTouched;
        bool DocTouched;
        bool CurrencyTouched;
        bool StatusTouched;
        bool PolicyTouched;

        protected override void OnInitialized()
        {
            Companies = MasterDataService.GetAllCompanies().Where(c => c.Status == "Active").ToList();
            //PostingProfiles = ProfileService.GetAll();

            if (IsEdit)
            {
                var existing = TypeService.GetById(Id!.Value);
                if (existing != null)
                {
                    IsInitializing = true;

                    type = new TransactionTypeModel
                    {
                        TransactionTypeId = existing.TransactionTypeId,
                        TenantId = existing.TenantId,
                        CompanyId = existing.CompanyId,
                        CompanyName = existing.CompanyName,
                        Code = existing.Code,
                        Name = existing.Name,
                        Description = existing.Description,
                        SourceCategory = existing.SourceCategory,
                        TransactionNature = existing.TransactionNature,
                        IsPostable = existing.IsPostable,
                        DefaultPostingProfileId = existing.DefaultPostingProfileId,
                        AllowAutoPost = existing.AllowAutoPost,
                        RequiresApproval = existing.RequiresApproval,
                        ApprovalWorkflowKey = existing.ApprovalWorkflowKey,
                        DocumentNumberSeriesId = existing.DocumentNumberSeriesId,
                        DocumentNoAssignmentTiming = existing.DocumentNoAssignmentTiming,
                        AllowDraftEdit = existing.AllowDraftEdit,
                        AllowDraftCancel = existing.AllowDraftCancel,
                        AllowReversal = existing.AllowReversal,
                        AllowManualEntry = existing.AllowManualEntry,
                        AllowForeignCurrency = existing.AllowForeignCurrency,
                        AllowNegativeLines = existing.AllowNegativeLines,
                        AmountPrecisionPolicy = existing.AmountPrecisionPolicy,

                        IsActive = existing.IsActive,
                        IsSystemDefined = existing.IsSystemDefined,

                        CreatedAt = existing.CreatedAt,
                        CreatedBy = existing.CreatedBy,
                        UpdatedAt = existing.UpdatedAt,
                        UpdatedBy = existing.UpdatedBy,
                    };

                    PostingProfiles = ProfileService
                        .GetAll()
                        .Where(p => p.CompanyId == type.CompanyId && p.IsActive)
                        .ToList();

                    DocumentNumberSeries = DocNumSeriesService.GetAll()
                .Where(l =>
                    l.CompanyId == type.CompanyId &&
                    l.AppliesToEntityType == AppliesToEntityType.FinancialTransaction &&
                    l.IsActive &&
                    (l.EffectiveFrom == null || l.EffectiveFrom.Value.Date <= today) &&
                    (l.EffectiveTo == null || l.EffectiveTo.Value.Date >= today))
                .ToList();

                    TypeCodeInput = type.Code;

                    IdentityTouched = false;
                    ClassificationTouched = false;
                    PostingTouched = false;
                    DocTouched = false;
                    PolicyTouched = false;
                    CurrencyTouched = false;
                    StatusTouched = false;

                    IsInitializing = false;
                }
            }

            else
            {
                type.TenantId = Guid.NewGuid();
                type.IsActive = true;
            }

            _editContext = new EditContext(type);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
                await JS.InvokeVoidAsync("feather.replace");
        }



        // Touch handlers
        void TouchIdentity(ChangeEventArgs e) => IdentityTouched = true;
        void TouchClassification(ChangeEventArgs e) => ClassificationTouched = true;
        void TouchPosting(ChangeEventArgs e) => PostingTouched = true;
        void TouchDoc(ChangeEventArgs e) => DocTouched = true;
        void TouchCurrency(ChangeEventArgs e) => CurrencyTouched = true;
        void TouchStatus(ChangeEventArgs e) => StatusTouched = true;
        void Touchpolicy(ChangeEventArgs e) => PolicyTouched = true;

        void BackToList()
        {
            Nav.NavigateTo("/transaction-types");
        }

        private async Task HandleSubmit()
        {
            // Collect Quill editor values before validation
            if (_descriptionEditor != null)
                type.Description = await _descriptionEditor.GetHtmlAsync();

            if (_editContext!.Validate())
            {
                await ContinueSave();
                return;
            }

            // Open the first section that contains an error
            if (HasIdentityErrors())
                OpenAccordion("identity");
            else if (HasClassificationErrors())
                OpenAccordion("classification");
            else if (HasPostingErrors())
                OpenAccordion("posting");
            else if (HasDocumentErrors())
                OpenAccordion("document");
            else if (HasCurrencyErrors())
                OpenAccordion("currency");

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


        // ================= ACCORDION =================
        void ToggleAccordion(string section)
        {
            switch (section)
            {
                case "identity":
                    ShowIdentity = !ShowIdentity;
                    break;
                case "classification":
                    ShowClassification = !ShowClassification;
                    break;
                case "posting":
                    ShowPosting = !ShowPosting;
                    break;
                case "doc":
                    ShowDoc = !ShowDoc;
                    break;
                case "currency":
                    ShowCurrency = !ShowCurrency;
                    break;
                case "policies":
                    ShowPolicies = !ShowPolicies;
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
                case "classification":
                    ShowClassification = true;
                    break;
                case "posting":
                    ShowPosting = true;
                    break;
                case "document":
                    ShowDoc = true;
                    break;
                case "currency":
                    ShowCurrency = true;
                    break;
                case "policies":
                    ShowPolicies = true;
                    break;
                case "status":
                    ShowStatus = true;
                    break;
            }
        }

        bool HasIdentityErrors()
        {
            return string.IsNullOrWhiteSpace(type.Code)
                || string.IsNullOrWhiteSpace(type.Name)
                || type.CompanyId == Guid.Empty;
        }

        bool HasClassificationErrors()
        {
            return type.SourceCategory is null
                || !Enum.IsDefined(typeof(SourceCategory), type.SourceCategory.Value);
        }

        bool HasPostingErrors()
        {
            return (type.IsPostable && type.DefaultPostingProfileId == Guid.Empty)
                || (type.RequiresApproval && string.IsNullOrWhiteSpace(type.ApprovalWorkflowKey));
        }

        bool HasDocumentErrors()
        {
            return type.DocumentNumberSeriesId == Guid.Empty || type.DocumentNumberSeriesId == null;
        }

        bool HasCurrencyErrors()
        {
            return type.AmountPrecisionPolicy.HasValue &&
                   (type.AmountPrecisionPolicy < 0 || type.AmountPrecisionPolicy > 6);
        }

        void OnCompanyChange()
        {
            var companyId = type.CompanyId;
            type.DocumentNumberSeriesId = null;
            if (companyId == Guid.Empty)
            {
                type.CompanyName = "";
                PostingProfiles.Clear();
                DocumentNumberSeries.Clear();
                TouchIdentity(null);
                return;
            }

            type.CompanyName = Companies
                .FirstOrDefault(p => p.Id == companyId)
                ?.LegalName ?? "—";

            PostingProfiles = ProfileService
                .GetAll()
                .Where(p => p.CompanyId == companyId && p.IsActive)
                .ToList();

            DocumentNumberSeries = DocNumSeriesService.GetAll()
                .Where(l =>
                    l.CompanyId == type.CompanyId &&
                    l.AppliesToEntityType == AppliesToEntityType.FinancialTransaction &&
                    l.IsActive &&
                    (l.EffectiveFrom == null || l.EffectiveFrom.Value.Date <= today) &&
                    (l.EffectiveTo == null || l.EffectiveTo.Value.Date >= today))
                .ToList();

            TouchIdentity(null);
        }


        string TypeCodeInput
        {
            get => type.Code;
            set
            {
                type.Code = value?.Trim().ToUpperInvariant() ?? "";
            }
        }

        void OnNameChanged()
        {
            type.Name = type.Name?.Trim() ?? "";
        }
        async Task ContinueSave()
        {
            try
            {
                if (IsEdit)
                {
                    TypeService.Update(type);
                    ToastService.ShowSuccess("Transaction type updated successfully", "Updated");
                }
                else
                {
                    TypeService.Create(type);
                    ToastService.ShowSuccess("Transaction type created successfully", "Created");
                }

                Nav.NavigateTo("/transaction-types");
            }
            catch (Exception ex)
            {
                ToastService.ShowError(ex.Message, "Validation Error");
            }
        }


    }
}
