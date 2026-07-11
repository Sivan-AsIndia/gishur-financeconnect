using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Data;

namespace FinanceConnect.Client.Pages.Finance.Ledger
{
    public partial class AddLedger : ComponentBase
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] FinanceDataService FinanceDataService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] AuthService AuthService { get; set; } = default!;

        [Parameter] public Guid? Id { get; set; }

        private EditContext _editContext = default!;
        public bool isInitialized = false;
        public LedgerModel Ledger = new();
        public List<CompanyModel> Companies = new();
        public List<CurrencyModel> Currencies = new();
        private RichTextEditor? _descriptionEditor;
        private RichTextEditor? _lockReasonEditor;

        // Touched state for accordion sections
        bool IdentityTouched = false;
        bool CurrencyTouched = false;
        bool PostingTouched = false;
        bool StatusTouched = false;

        // Accordion visibility state
        bool ShowIdentity = true;
        bool ShowCurrency = false;
        bool ShowPosting = false;
        bool ShowStatus = false;

        // Validation error messages for dropdowns and text fields
        string? LedgerCodeValidationError = null;
        string? LedgerNameValidationError = null;
        string? CompanyValidationError = null;
        string? LedgerTypeValidationError = null;
        string? BaseCurrencyValidationError = null;
        string? CurrencyModeValidationError = null;
        string? StatusValidationError = null;
        string? LockReasonValidationError = null;

        void TouchIdentity(ChangeEventArgs e) => IdentityTouched = true;
        void TouchCurrency(ChangeEventArgs e) => CurrencyTouched = true;
        void TouchPosting(ChangeEventArgs e) => PostingTouched = true;
        void TouchStatus(ChangeEventArgs e) => StatusTouched = true;

        // OnChanged handlers to clear validation errors and mark sections as touched
        void OnLedgerCodeChanged()
        {
            IdentityTouched = true;
            LedgerCodeValidationError = null;
        }

        void OnLedgerNameChanged()
        {
            IdentityTouched = true;
            LedgerNameValidationError = null;
        }
        void OnLedgerNameChangedTrim()
        {
            Ledger.LedgerName = Ledger.LedgerName?.Trim() ?? "";
        }

        void OnCompanyChanged()
        {
            IdentityTouched = true;
            CompanyValidationError = null;
        }

        void OnLedgerTypeChanged()
        {
            CurrencyTouched = true;
            LedgerTypeValidationError = null;
        }

        void OnBaseCurrencyChanged()
        {
            CurrencyTouched = true;
            BaseCurrencyValidationError = null;
        }

        void OnCurrencyModeChanged()
        {
            CurrencyTouched = true;
            CurrencyModeValidationError = null;
        }

        void OnStatusChanged()
        {
            StatusTouched = true;
            StatusValidationError = null;
        }

        public bool IsEdit => Id.HasValue;
        public bool CanEdit = true;
        public string PageTitle => IsEdit ? "Edit Ledger" : "Create Ledger";
        public string PageSubTitle => IsEdit ? "Update ledger details" : "Create new ledger";

        void GoBackToList() => Nav.NavigateTo("/ledgers");

        // Confirmation modal properties
        string ConfirmTitle = "";
        string ConfirmMessage = "";
        string ConfirmType = "warning";
        Action? ConfirmAction;

        // Property wrapper for LedgerCode - converts to uppercase immediately
        public string LedgerCodeValue
        {
            get => Ledger.LedgerCode ?? "";
            set
            {
                // Convert to uppercase immediately to prevent regex validation errors
                Ledger.LedgerCode = value?.Trim().ToUpperInvariant() ?? "";
                IdentityTouched = true; // Mark section as touched
            }
        }

        public string ReportingCurrencyIdStr
        {
            get => Ledger.ReportingCurrencyId?.ToString() ?? "";
            set => Ledger.ReportingCurrencyId = string.IsNullOrEmpty(value) ? null : Guid.Parse(value);
        }

        public string SelectedCompanyId
        {
            get => Ledger.CompanyId?.ToString() ?? "";
            set
            {
                Ledger.CompanyId = string.IsNullOrEmpty(value) ? null : Guid.Parse(value);
                CompanyValidationError = null; // Clear error on change
            }
        }

        public string SelectedBaseCurrencyId
        {
            get => Ledger.BaseCurrencyId?.ToString() ?? "";
            set
            {
                Ledger.BaseCurrencyId = string.IsNullOrEmpty(value) ? null : Guid.Parse(value);
                BaseCurrencyValidationError = null; // Clear error on change
            }
        }

        public string SelectedLedgerType
        {
            get => Ledger.LedgerType ?? "";
            set
            {
                Ledger.LedgerType = string.IsNullOrEmpty(value) ? null : value;
                LedgerTypeValidationError = null; // Clear error on change
            }
        }

        public string SelectedCurrencyMode
        {
            get => Ledger.CurrencyMode ?? "";
            set
            {
                Ledger.CurrencyMode = string.IsNullOrEmpty(value) ? null : value;
                CurrencyModeValidationError = null; // Clear error on change
            }
        }

        public string SelectedStatus
        {
            get => Ledger.Status ?? "";
            set
            {
                Ledger.Status = string.IsNullOrEmpty(value) ? null : value;
                StatusValidationError = null; // Clear error on change
            }
        }

        protected override async Task OnInitializedAsync()
        {
            Companies = MasterDataService.GetAllCompanies().Where(c => c.Status == "Active").ToList();
            Currencies = MasterDataService.GetAllCurrencies().Where(c => c.IsActive).ToList();

            if (IsEdit)
            {
                var existing = FinanceDataService.GetLedgerById(Id!.Value);
                if (existing != null)
                {
                    Ledger = new LedgerModel
                    {
                        Id = existing.Id,
                        TenantId = existing.TenantId,
                        LedgerCode = existing.LedgerCode,
                        LedgerName = existing.LedgerName,
                        CompanyId = existing.CompanyId,
                        CompanyCode = existing.CompanyCode,
                        CompanyName = existing.CompanyName,
                        Description = existing.Description,
                        IsDefaultLedger = existing.IsDefaultLedger,
                        LedgerType = existing.LedgerType,
                        BaseCurrencyId = existing.BaseCurrencyId,
                        BaseCurrencyCode = existing.BaseCurrencyCode,
                        BaseCurrencyName = existing.BaseCurrencyName,
                        ReportingCurrencyId = existing.ReportingCurrencyId,
                        ReportingCurrencyCode = existing.ReportingCurrencyCode,
                        ReportingCurrencyName = existing.ReportingCurrencyName,
                        CurrencyMode = existing.CurrencyMode,
                        ExchangeRateSource = existing.ExchangeRateSource,
                        AllowPostingFromDate = existing.AllowPostingFromDate,
                        AllowPostingToDate = existing.AllowPostingToDate,
                        LockBackDatedPosting = existing.LockBackDatedPosting,
                        BackdatedPostingDaysAllowed = existing.BackdatedPostingDaysAllowed,
                        FuturePostingDaysAllowed = existing.FuturePostingDaysAllowed,
                        RequireApprovalBeforePosting = existing.RequireApprovalBeforePosting,
                        EnforceAccountingPeriodOpen = existing.EnforceAccountingPeriodOpen,
                        IsConsolidationEligible = existing.IsConsolidationEligible,
                        ConsolidationGroupId = existing.ConsolidationGroupId,
                        ConsolidationGroupName = existing.ConsolidationGroupName,
                        Status = existing.Status,
                        LockStatus = existing.LockStatus,
                        LockReason = existing.LockReason,
                        CreatedAt = existing.CreatedAt,
                        CreatedBy = existing.CreatedBy
                    };
                    CanEdit = FinanceDataService.CanEditLedger(Id!.Value);
                }
                else
                {
                    Nav.NavigateTo("/ledgers");
                    return;
                }
            }

            _editContext = new EditContext(Ledger);
            isInitialized = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
        }

        void ToggleAccordion(string section)
        {
            bool isCurrentlyOpen = section switch
            {
                "ledgerIdentity" => ShowIdentity,
                "typeCurrency" => ShowCurrency,
                "postingControls" => ShowPosting,
                "statusGovernance" => ShowStatus,
                _ => false
            };

            // Close all sections first
            ShowIdentity = false;
            ShowCurrency = false;
            ShowPosting = false;
            ShowStatus = false;

            // If it was closed, open it; if it was open, keep all closed
            if (!isCurrentlyOpen)
            {
                switch (section)
                {
                    case "ledgerIdentity": ShowIdentity = true; break;
                    case "typeCurrency": ShowCurrency = true; break;
                    case "postingControls": ShowPosting = true; break;
                    case "statusGovernance": ShowStatus = true; break;
                }
            }
        }

        void OpenAccordion(string section)
        {
            ShowIdentity = false;
            ShowCurrency = false;
            ShowPosting = false;
            ShowStatus = false;

            switch (section)
            {
                case "ledgerIdentity": ShowIdentity = true; break;
                case "typeCurrency": ShowCurrency = true; break;
                case "postingControls": ShowPosting = true; break;
                case "statusGovernance": ShowStatus = true; break;
            }
        }

        bool HasIdentityErrors()
        {
            return string.IsNullOrWhiteSpace(Ledger.LedgerCode)
                || string.IsNullOrWhiteSpace(Ledger.LedgerName)
                || !Ledger.CompanyId.HasValue;
        }

        bool HasCurrencyErrors()
        {
            return string.IsNullOrWhiteSpace(Ledger.LedgerType)
                || !Ledger.BaseCurrencyId.HasValue
                || string.IsNullOrWhiteSpace(Ledger.CurrencyMode);
        }

        bool HasStatusErrors()
        {
            return string.IsNullOrWhiteSpace(Ledger.Status)
                || ((Ledger.Status == LedgerStatus.Inactive || Ledger.LockStatus != LockStatuses.Unlocked)
                    && string.IsNullOrWhiteSpace(Ledger.LockReason));
        }

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

        /// <summary>
        /// Validates all required fields and sets validation error messages
        /// </summary>
        private bool ValidateAllFields()
        {
            bool isValid = true;

            // Clear all validation errors first
            LedgerCodeValidationError = null;
            LedgerNameValidationError = null;
            CompanyValidationError = null;
            LedgerTypeValidationError = null;
            BaseCurrencyValidationError = null;
            CurrencyModeValidationError = null;
            StatusValidationError = null;

            // Validate Ledger Code
            if (string.IsNullOrWhiteSpace(Ledger.LedgerCode))
            {
                LedgerCodeValidationError = "Ledger Code is required";
                isValid = false;
            }

            // Validate Ledger Name
            if (string.IsNullOrWhiteSpace(Ledger.LedgerName))
            {
                LedgerNameValidationError = "Ledger Name is required";
                isValid = false;
            }

            // Validate Company
            if (!Ledger.CompanyId.HasValue)
            {
                CompanyValidationError = "Company is required";
                isValid = false;
            }

            // Validate Ledger Type
            if (string.IsNullOrEmpty(Ledger.LedgerType))
            {
                LedgerTypeValidationError = "Ledger Type is required";
                isValid = false;
            }

            // Validate Base Currency
            if (!Ledger.BaseCurrencyId.HasValue)
            {
                BaseCurrencyValidationError = "Base Currency is required";
                isValid = false;
            }

            // Validate Currency Mode
            if (string.IsNullOrEmpty(Ledger.CurrencyMode))
            {
                CurrencyModeValidationError = "Currency Mode is required";
                isValid = false;
            }

            // Validate Status
            if (string.IsNullOrEmpty(Ledger.Status))
            {
                StatusValidationError = "Status is required";
                isValid = false;
            }

            // Validate Lock Reason (required if inactive or locked)
            if ((Ledger.Status == LedgerStatus.Inactive || Ledger.LockStatus != LockStatuses.Unlocked)
                && string.IsNullOrWhiteSpace(Ledger.LockReason))
            {
                LockReasonValidationError = "Lock / Inactivation Reason is required when ledger is inactive or locked";
                isValid = false;
            }

            return isValid;
        }

        public async Task HandleSubmit()
        {
            // Get content from Quill editors
            if (_descriptionEditor != null)
            {
                Ledger.Description = await _descriptionEditor.GetHtmlAsync();
            }
            if (_lockReasonEditor != null)
            {
                Ledger.LockReason = await _lockReasonEditor.GetHtmlAsync();
            }

            // Validate all fields
            var isValid = ValidateAllFields();

            if (isValid)
            {
                Save();
                return;
            }

            // Open ALL accordions that have validation errors
            if (HasIdentityErrors())
                OpenAccordion("ledgerIdentity");
            if (HasCurrencyErrors())
                OpenAccordion("typeCurrency");
            if (HasStatusErrors())
                OpenAccordion("statusGovernance");

            await InvokeAsync(StateHasChanged);
        }

        public async void Save()
        {
            // Get content from Quill editors
            if (_descriptionEditor != null)
            {
                Ledger.Description = await _descriptionEditor.GetHtmlAsync();
            }
            if (_lockReasonEditor != null)
            {
                Ledger.LockReason = await _lockReasonEditor.GetHtmlAsync();
            }

            // Validate all fields first
            if (!ValidateAllFields())
            {
                // Open ALL accordions with errors
                if (HasIdentityErrors())
                    OpenAccordion("ledgerIdentity");
                if (HasCurrencyErrors())
                    OpenAccordion("typeCurrency");
                if (HasStatusErrors())
                    OpenAccordion("statusGovernance");
                return;
            }

            // LedgerCode is already uppercase from the property wrapper
            Ledger.LedgerCode = Ledger.LedgerCode?.Trim() ?? "";

            var company = Companies.FirstOrDefault(c => c.Id == Ledger.CompanyId);
            Ledger.CompanyCode = company?.CompanyCode;
            Ledger.CompanyName = company?.LegalName;

            var baseCurrency = Currencies.FirstOrDefault(c => c.Id == Ledger.BaseCurrencyId);
            Ledger.BaseCurrencyCode = baseCurrency?.CurrencyCode;
            Ledger.BaseCurrencyName = baseCurrency?.CurrencyName;

            if (Ledger.ReportingCurrencyId.HasValue)
            {
                var reportingCurrency = Currencies.FirstOrDefault(c => c.Id == Ledger.ReportingCurrencyId);
                Ledger.ReportingCurrencyCode = reportingCurrency?.CurrencyCode;
                Ledger.ReportingCurrencyName = reportingCurrency?.CurrencyName;
            }
            else
            {
                Ledger.ReportingCurrencyCode = null;
                Ledger.ReportingCurrencyName = null;
            }

            if (IsEdit)
            {
                Ledger.UpdatedAt = DateTime.Now;
                Ledger.UpdatedBy = AuthService.CurrentUser?.UserName ?? "System";
                FinanceDataService.UpdateLedger(Ledger);
                ToastService.ShowSuccess($"Ledger '{Ledger.LedgerName}' updated successfully", "Updated");
            }
            else
            {
                Ledger.CreatedAt = DateTime.Now;
                Ledger.CreatedBy = AuthService.CurrentUser?.UserName ?? "System";
                FinanceDataService.AddLedger(Ledger);
                ToastService.ShowSuccess($"Ledger '{Ledger.LedgerName}' created successfully", "Created");
            }

            Nav.NavigateTo("/ledgers");
        }
    }
}
