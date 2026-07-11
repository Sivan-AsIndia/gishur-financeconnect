using FinanceConnect.Client.Services;
using FinanceConnect.Client.Shared;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.COA
{
    public partial class ChartOfAccountsForm : ComponentBase
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject] COADataService COADataService { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] AuthService AuthService { get; set; } = default!;

        [Parameter] public Guid? Id { get; set; }

        private EditContext _editContext = default!;
        RichTextEditor? _descriptionEditor;
        public bool isInitialized = false;
        public bool isSaving = false;
        public ChartOfAccountsViewModel Model = new();
        public List<CompanyModel> Companies = new();

        // Touched state for accordion sections
        bool IdentityTouched = false;
        bool ScopeTouched = false;
        bool CodeRulesTouched = false;
        bool GovernanceTouched = false;
        bool TemplateTouched = false;
        bool StatusTouched = false;

        // Accordion visibility state
        bool ShowIdentity = true;
        bool ShowScope = false;
        bool ShowCodeRules = false;
        bool ShowGovernance = false;
        bool ShowTemplate = false;
        bool ShowStatus = false;

        // Validation error messages
        string? ChartCodeValidationError = null;
        string? ChartNameValidationError = null;
        string? CompanyValidationError = null;
        string? ChartTypeValidationError = null;
        string? CodeFormatValidationError = null;
        string? StatusValidationError = null;
        string? LockReasonValidationError = null;

        void TouchIdentity(ChangeEventArgs e) => IdentityTouched = true;
        void TouchScope(ChangeEventArgs e) => ScopeTouched = true;
        void TouchCodeRules(ChangeEventArgs e) => CodeRulesTouched = true;
        void TouchStatus(ChangeEventArgs e) => StatusTouched = true;

        // OnChanged handlers
        void OnChartCodeChanged()
        {
            IdentityTouched = true;
            ChartCodeValidationError = null;
        }

        void OnChartNameChanged()
        {
            IdentityTouched = true;
            ChartNameValidationError = null;
            Model.ChartName = Model.ChartName?.Trim() ?? "";
        }

        void OnCompanyChanged()
        {
            IdentityTouched = true;
            CompanyValidationError = null;
        }

        void OnChartTypeChanged()
        {
            ScopeTouched = true;
            ChartTypeValidationError = null;
        }

        void OnStatusChanged()
        {
            StatusTouched = true;
            StatusValidationError = null;
            // Clear lock reason if status changed away from Locked/Retired
            if (Model.Status != COAStatuses.Locked && Model.Status != COAStatuses.Retired)
            {
                Model.LockReason = null;
                LockReasonValidationError = null;
            }
        }

        void OnCodeFormatChanged()
        {
            CodeRulesTouched = true;
            CodeFormatValidationError = null;
        }

        void OnLockReasonChanged()
        {
            StatusTouched = true;
            LockReasonValidationError = null;
        }

        public bool IsEdit => Id.HasValue;
        public string PageTitle => IsEdit ? "Edit Chart of Accounts" : "Create Chart of Accounts";
        public string PageSubTitle => IsEdit ? "Update chart information" : "Create a new chart of accounts";

        // Property wrapper for ChartCode - converts to uppercase immediately
        public string ChartCodeValue
        {
            get => Model.ChartCode ?? "";
            set
            {
                Model.ChartCode = value?.Trim().ToUpperInvariant() ?? "";
                IdentityTouched = true;
            }
        }

        public string SelectedCompanyId
        {
            get => Model.CompanyId?.ToString() ?? "";
            set
            {
                Model.CompanyId = string.IsNullOrEmpty(value) ? null : Guid.Parse(value);
                CompanyValidationError = null;
            }
        }

        public string SelectedChartType
        {
            get => Model.ChartType ?? "";
            set
            {
                Model.ChartType = string.IsNullOrEmpty(value) ? null : value;
                ChartTypeValidationError = null;
            }
        }

        public string SelectedStatus
        {
            get => Model.Status ?? "";
            set
            {
                Model.Status = string.IsNullOrEmpty(value) ? null : value;
                StatusValidationError = null;
            }
        }

        protected override async Task OnInitializedAsync()
        {
            Companies = MasterDataService.GetAllCompanies().Where(c => c.Status == "Active").ToList();

            if (IsEdit)
            {
                var existing = await COADataService.GetChartOfAccountsByIdAsync(Id!.Value);
                if (existing != null)
                {
                    Model = new ChartOfAccountsViewModel
                    {
                        Id = existing.Id,
                        TenantId = existing.TenantId,
                        ChartCode = existing.ChartCode,
                        ChartName = existing.ChartName,
                        CompanyId = existing.CompanyId,
                        CompanyName = existing.CompanyName,
                        Description = existing.Description,
                        ChartType = existing.ChartType,
                        IsDefaultForCompany = existing.IsDefaultForCompany,
                        EffectiveFrom = existing.EffectiveFrom,
                        EffectiveTo = existing.EffectiveTo,
                        AccountCodeMode = existing.AccountCodeMode,
                        AccountCodeFormat = existing.AccountCodeFormat,
                        NextAccountNumber = existing.NextAccountNumber,
                        EnforceUniqueAccountCode = existing.EnforceUniqueAccountCode,
                        EnforceUniqueAccountName = existing.EnforceUniqueAccountName,
                        AllowAccountCodeReuseAfterInactivation = existing.AllowAccountCodeReuseAfterInactivation,
                        ChangeRequestRequired = existing.ChangeRequestRequired,
                        TemplateSource = existing.TemplateSource,
                        TemplateReferenceId = existing.TemplateReferenceId,
                        Status = existing.Status,
                        LockReason = existing.LockReason,
                        CreatedAt = existing.CreatedAt,
                        CreatedBy = existing.CreatedBy
                    };
                }
                else
                {
                    Nav.NavigateTo("/chart-of-accounts");
                    return;
                }
            }
            else
            {
                // Don't set defaults - let user manually select all dropdown values
                Model.EnforceUniqueAccountCode = true;
                Model.EffectiveFrom = DateTime.Today;
            }

            _editContext = new EditContext(Model);
            isInitialized = true;
            await Task.Delay(50);
            await JS.InvokeVoidAsync("feather.replace");
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
        }

        void ToggleAccordion(string section)
        {
            switch (section)
            {
                case "identity":
                    ShowIdentity = !ShowIdentity;
                    break;
                case "scope":
                    ShowScope = !ShowScope;
                    break;
                case "codeRules":
                    ShowCodeRules = !ShowCodeRules;
                    break;
                case "governance":
                    ShowGovernance = !ShowGovernance;
                    break;
                case "template":
                    ShowTemplate = !ShowTemplate;
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
                case "scope":
                    ShowScope = true;
                    break;
                case "codeRules":
                    ShowCodeRules = true;
                    break;
                case "governance":
                    ShowGovernance = true;
                    break;
                case "template":
                    ShowTemplate = true;
                    break;
                case "status":
                    ShowStatus = true;
                    break;
            }
        }

        bool HasIdentityErrors()
        {
            return string.IsNullOrWhiteSpace(Model.ChartCode)
                || string.IsNullOrWhiteSpace(Model.ChartName)
                || !Model.CompanyId.HasValue;
        }

        bool HasScopeErrors()
        {
            return string.IsNullOrWhiteSpace(Model.ChartType);
        }

        bool HasCodeRulesErrors()
        {
            return string.IsNullOrWhiteSpace(Model.AccountCodeFormat);
        }

        bool HasStatusErrors()
        {
            return string.IsNullOrWhiteSpace(Model.Status)
                || ((Model.Status == COAStatuses.Locked || Model.Status == COAStatuses.Retired) && string.IsNullOrWhiteSpace(Model.LockReason));
        }

        private bool ValidateAllFields()
        {
            bool isValid = true;

            // Clear all validation errors first
            ChartCodeValidationError = null;
            ChartNameValidationError = null;
            CompanyValidationError = null;
            ChartTypeValidationError = null;
            CodeFormatValidationError = null;
            StatusValidationError = null;
            LockReasonValidationError = null;

            // Validate Chart Code
            if (string.IsNullOrWhiteSpace(Model.ChartCode))
            {
                ChartCodeValidationError = "Chart Code is required";
                isValid = false;
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(Model.ChartCode.Trim(), @"^[A-Za-z0-9_\-]+$"))
            {
                ChartCodeValidationError = "Chart Code can only contain letters, numbers, underscore (_) and hyphen (-)";
                isValid = false;
            }

            // Validate Chart Name
            if (string.IsNullOrWhiteSpace(Model.ChartName))
            {
                ChartNameValidationError = "Chart Name is required";
                isValid = false;
            }

            // Validate Company
            if (!Model.CompanyId.HasValue)
            {
                CompanyValidationError = "Company is required";
                isValid = false;
            }

            // Validate Chart Type
            if (string.IsNullOrEmpty(Model.ChartType))
            {
                ChartTypeValidationError = "Chart Type is required";
                isValid = false;
            }

            // Validate Account Code Format (Required per spec)
            if (string.IsNullOrWhiteSpace(Model.AccountCodeFormat))
            {
                CodeFormatValidationError = "Account Code Format is required";
                isValid = false;
            }

            // Validate Status
            if (string.IsNullOrEmpty(Model.Status))
            {
                StatusValidationError = "Status is required";
                isValid = false;
            }

            // Validate Lock Reason (required when Status = Locked/Retired)
            if ((Model.Status == COAStatuses.Locked || Model.Status == COAStatuses.Retired)
                && string.IsNullOrWhiteSpace(Model.LockReason))
            {
                LockReasonValidationError = Model.Status == COAStatuses.Locked
                    ? "Lock Reason is required when Status is Locked"
                    : "Retirement Reason is required when Status is Retired";
                isValid = false;
            }

            // Validate Effective Date cross-validation
            if (Model.EffectiveTo.HasValue
                && Model.EffectiveFrom > Model.EffectiveTo.Value)
            {
                ToastService.ShowError("'Effective From' date must be on or before 'Effective To' date", "Validation Error");
                isValid = false;
            }

            // Validate Account Code Format separators (only /, - allowed)
            if (!string.IsNullOrWhiteSpace(Model.AccountCodeFormat))
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(Model.AccountCodeFormat.Trim(), @"^[A-Za-z0-9#/\-]+$"))
                {
                    CodeFormatValidationError = "Account Code Format can only use letters, numbers, #, / and - as separators";
                    isValid = false;
                }
            }

            return isValid;
        }

        public async Task HandleSubmit()
        {
            // Collect Quill editor values before validation
            if (_descriptionEditor != null)
                Model.Description = await _descriptionEditor.GetHtmlAsync();
            var isValid = ValidateAllFields();

            if (isValid)
            {
                await Save();
                return;
            }

            // Open ALL accordions that have validation errors
            if (HasIdentityErrors())
                OpenAccordion("identity");
            if (HasScopeErrors())
                OpenAccordion("scope");
            if (HasCodeRulesErrors())
                OpenAccordion("codeRules");
            if (HasStatusErrors())
                OpenAccordion("status");

            await InvokeAsync(StateHasChanged);
        }

        public async Task Save()
        {
            if (!ValidateAllFields())
            {
                if (HasIdentityErrors())
                    OpenAccordion("identity");
                if (HasScopeErrors())
                    OpenAccordion("scope");
                if (HasCodeRulesErrors())
                    OpenAccordion("codeRules");
                if (HasStatusErrors())
                    OpenAccordion("status");
                return;
            }

            isSaving = true;

            Model.ChartCode = Model.ChartCode?.Trim().ToUpperInvariant() ?? "";
            Model.ChartName = Model.ChartName?.Trim() ?? "";

            var company = Companies.FirstOrDefault(c => c.Id == Model.CompanyId);
            Model.CompanyName = company?.LegalName;

            if (IsEdit)
            {
                Model.UpdatedAt = DateTime.Now;
                Model.UpdatedBy = AuthService.CurrentUser?.UserName ?? "System";
                await COADataService.UpdateChartOfAccountsAsync(Model);
                ToastService.ShowSuccess($"Chart '{Model.ChartName}' updated successfully", "Updated");
            }
            else
            {
                Model.CreatedAt = DateTime.Now;
                Model.CreatedBy = AuthService.CurrentUser?.UserName ?? "System";
                await COADataService.AddChartOfAccountsAsync(Model);
                ToastService.ShowSuccess($"Chart '{Model.ChartName}' created successfully", "Created");
            }

            isSaving = false;
            Nav.NavigateTo("/chart-of-accounts");
        }

        void NavigateBack()
        {
            Nav.NavigateTo("/chart-of-accounts");
        }
    }
}
