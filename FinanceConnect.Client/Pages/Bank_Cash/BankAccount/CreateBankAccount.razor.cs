using FinanceConnect.Client.Services;
using FinanceConnect.Client.Shared;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Bank_Cash.BankAccount
{
    public partial class CreateBankAccount : ComponentBase
    {
        [Parameter] public string? Code { get; set; }

        private List<CurrencyModel> Currencies = new();

        private EditContext _editContext = default!;
        public bool isInitialized = false;
        public bool isSaving = false;
        public bool _submitted = false;
        public BankAccountModel Model = new();
        public List<CompanyModel> Companies = new();
        public List<BranchModel> BranchesList = new();

        bool IdentityTouched = false;
        bool DetailsTouched = false;
        bool StatusTouched = false;

        bool ShowIdentity = true;
        bool ShowDetails = false;
        bool ShowControls = false;
        bool ShowAccounting = false;
        bool ShowRecon = false;
        bool ShowStatus = false;

        string? BankAccountCodeValidationError = null;
        string? BankAccountNameValidationError = null;
        string? CompanyValidationError = null;
        string? BranchValidationError = null;
        string? StatusValidationError = null;
        string? BankNameValidationError = null;
        string? BranchNameValidationError = null;
        string? IFSCValidationError = null;
        string? AccountHolderValidationError = null;
        string? AccountNumberValidationError = null;
        string? CurrencyValidationError = null;
        string? BankGLValidationError = null;
        string? AccountTypeValidationError = null;
        string? StatementProfileValidationError = null;
        private RichTextEditor descriptionEditor;

        private bool IsEdit => !string.IsNullOrWhiteSpace(Code);

        public string PageTitle => IsEdit ? "Edit Bank Account" : "Create Bank Account";
        public string PageSubTitle => IsEdit ? "Update bank account details" : "Add a new bank account";

        public Guid? SelectedCompanyId
        {
            get => Model.CompanyId;
            set
            {
                Model.CompanyId = value ?? Guid.Empty;
                CompanyValidationError = null;
                // Cascade: reset branch when company changes
                Model.BranchId = Guid.Empty;
                BranchValidationError = null;
                LoadBranchesForCompany();
            }
        }

        public List<BranchModel> FilteredBranches { get; set; } = new();

        public bool IsBranchDisabled => !IsEdit && (Model.CompanyId == Guid.Empty);

        private void LoadBranchesForCompany()
        {
            Currencies = MasterDataService.GetAllCurrencies();
            if (Model.CompanyId != Guid.Empty)
            {
                FilteredBranches = BranchService.GetByCompany(Model.CompanyId);
            }
            else
            {
                FilteredBranches = new();
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            Companies = MasterDataService.GetAllCompanies()
                         .Where(c => c.Status == "Active")
                         .ToList();
            BranchesList = BranchService.GetAll();

            if (IsEdit)
            {
                var existing = await BankAccountService.GetByCodeAsync(Code!);

                if (existing == null)
                {
                    Nav.NavigateTo("/bank-accounts");
                    return;
                }

                Model = new BankAccountModel
                {
                    Id = existing.Id,
                    CompanyId = existing.CompanyId,
                    BranchId = existing.BranchId,
                    BranchName = existing.BranchName,
                    BankAccountCode = existing.BankAccountCode,
                    BankAccountName = existing.BankAccountName,
                    Description = existing.Description,
                    BankName = existing.BankName,
                    BankBranchName = existing.BankBranchName,
                    IFSCCode = existing.IFSCCode,
                    MICRCode = existing.MICRCode,
                    AccountHolderName = existing.AccountHolderName,
                    BankAccountNumber = existing.BankAccountNumber,
                    BankAccountType = existing.BankAccountType,
                    CurrencyId = existing.CurrencyId,
                    UPIId = existing.UPIId,
                    SWIFTCode = existing.SWIFTCode,
                    IBAN = existing.IBAN,
                    IsPrimaryOperatingAccount = existing.IsPrimaryOperatingAccount,
                    IsOverdraftAllowed = existing.IsOverdraftAllowed,
                    OverdraftLimitAmount = existing.OverdraftLimitAmount,
                    MinimumBalanceAmount = existing.MinimumBalanceAmount,
                    IsLockedForTransactions = existing.IsLockedForTransactions,
                    LockReason = existing.LockReason,
                    IsBlocked = existing.IsBlocked,
                    BlockReason = existing.BlockReason,
                    BankGLAccountCode = existing.BankGLAccountCode,
                    ClearingGLAccountCode = existing.ClearingGLAccountCode,
                    BankChargesExpenseGLCode = existing.BankChargesExpenseGLCode,
                    InterestIncomeGLCode = existing.InterestIncomeGLCode,
                    RoundOffGLCode = existing.RoundOffGLCode,
                    IsStatementImportEnabled = existing.IsStatementImportEnabled,
                    StatementProfile = existing.StatementProfile,
                    IsBankReconciliationMandatory = existing.IsBankReconciliationMandatory,
                    AutoMatchEnabled = existing.AutoMatchEnabled,
                    AutoMatchDateWindowDays = existing.AutoMatchDateWindowDays,
                    AutoMatchAmountTolerance = existing.AutoMatchAmountTolerance,
                    BankAccountStatus = existing.BankAccountStatus,
                    CloseReason = existing.CloseReason
                };
                LoadBranchesForCompany();
            }
            else
            {
                Model = new BankAccountModel
                {
                    BankAccountStatus = "",
                    BankAccountType = "",
                    CurrencyCode = "INR",
                    IsBankReconciliationMandatory = true,
                    AutoMatchEnabled = true,
                    AutoMatchDateWindowDays = 2,
                    IsStatementImportEnabled = true
                };
            }

            _editContext = new EditContext(Model);
            isInitialized = true;

            // Load branches for selected company (edit mode)
            if (IsEdit && Model.CompanyId != Guid.Empty)
            {
                LoadBranchesForCompany();
            }

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
                case "identity": ShowIdentity = !ShowIdentity; break;
                case "details": ShowDetails = !ShowDetails; break;
                case "controls": ShowControls = !ShowControls; break;
                case "accounting": ShowAccounting = !ShowAccounting; break;
                case "recon": ShowRecon = !ShowRecon; break;
                case "status": ShowStatus = !ShowStatus; break;
            }
        }

        void TouchIdentity(ChangeEventArgs e) => IdentityTouched = true;
        void TouchDetails(ChangeEventArgs e) => DetailsTouched = true;
        void TouchStatus(ChangeEventArgs e) => StatusTouched = true;

        void OnBankAccountCodeChanged() {
            Model.BankAccountCode = Model.BankAccountCode.Trim().ToUpperInvariant();
            IdentityTouched = true;
            BankAccountCodeValidationError = null;
        }
        void OnBankAccountNameChanged() {
            Model.BankAccountName = Model.BankAccountName.Trim();
            IdentityTouched = true; BankAccountNameValidationError = null;
        }
        void OnCompanyChanged() { IdentityTouched = true; CompanyValidationError = null; }
        void OnBranchChanged() { IdentityTouched = true; BranchValidationError = null; }
        void OnStatusChanged() { StatusTouched = true; StatusValidationError = null; }
        void OnBankType() { DetailsTouched = true; }
        void OnIFSCChanged() 
        {
            Model.IFSCCode = Model.IFSCCode.Trim().ToUpperInvariant();
            DetailsTouched = true;
            IFSCValidationError = null;
        }
        void OnAccountHolderChanged() { DetailsTouched = true; AccountHolderValidationError = null; }
        void OnAccountNumberChanged() { DetailsTouched = true; AccountNumberValidationError = null; }
        void OnCurrencyChanged() { DetailsTouched = true; CurrencyValidationError = null; }
        void OnBankGLChanged() { BankGLValidationError = null; }
        void OnAccountTypeChanged() { DetailsTouched = true; AccountTypeValidationError = null; }
        void OnStatementProfileChanged() { StatementProfileValidationError = null; }

        private bool ValidateAllFields()
        {
            bool isValid = true;

            BankAccountCodeValidationError = null;
            BankAccountNameValidationError = null;
            CompanyValidationError = null;
            BranchValidationError = null;
            StatusValidationError = null;
            BankNameValidationError = null;
            IFSCValidationError = null;
            AccountHolderValidationError = null;
            AccountNumberValidationError = null;
            CurrencyValidationError = null;
            BankGLValidationError = null;
            AccountTypeValidationError = null;
            StatementProfileValidationError = null;

            if (string.IsNullOrWhiteSpace(Model.BankAccountCode))
            {
                BankAccountCodeValidationError = "Bank Account Code is required";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(Model.BankAccountName))
            {
                BankAccountNameValidationError = "Bank Account Name is required";
                isValid = false;
            }

            if (Model.CompanyId == Guid.Empty)
            {
                CompanyValidationError = "Company is required";
                isValid = false;
            }

            if (Model.BranchId == Guid.Empty)
            {
                BranchValidationError = "Branch is required";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(Model.BankName))
            {
                BankNameValidationError = "Bank Name is required";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(Model.IFSCCode))
            {
                IFSCValidationError = "IFSC Code is required";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(Model.AccountHolderName))
            {
                AccountHolderValidationError = "Account Holder Name is required";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(Model.BankAccountNumber))
            {
                AccountNumberValidationError = "Account Number is required";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(Model.CurrencyCode))
            {
                CurrencyValidationError = "Currency is required";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(Model.BankGLAccountCode))
            {
                BankGLValidationError = "Bank GL Account is required";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(Model.BankAccountType))
            {
                AccountTypeValidationError = "Account Type is required";
                isValid = false;
            }

            if (Model.IsStatementImportEnabled && string.IsNullOrWhiteSpace(Model.StatementProfile))
            {
                StatementProfileValidationError = "Statement Profile is required when import is enabled";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(Model.BankAccountStatus))
            {
                StatusValidationError = "Status is required";
                isValid = false;
            }

            return isValid;
        }

        public async Task HandleSubmit()
        {
            _submitted = true;

            if (!ValidateAllFields())
            {
                if (string.IsNullOrWhiteSpace(Model.BankAccountCode) || string.IsNullOrWhiteSpace(Model.BankAccountName) || Model.CompanyId == Guid.Empty || Model.BranchId == Guid.Empty)
                    ShowIdentity = true;
                if (string.IsNullOrWhiteSpace(Model.BankName) || string.IsNullOrWhiteSpace(Model.IFSCCode) || string.IsNullOrWhiteSpace(Model.AccountHolderName) || string.IsNullOrWhiteSpace(Model.BankAccountNumber) || string.IsNullOrWhiteSpace(Model.CurrencyCode) || string.IsNullOrWhiteSpace(Model.BankAccountType))
                    ShowDetails = true;
                if (string.IsNullOrWhiteSpace(Model.BankGLAccountCode))
                    ShowAccounting = true;
                if (Model.IsStatementImportEnabled && string.IsNullOrWhiteSpace(Model.StatementProfile))
                    ShowRecon = true;
                if (string.IsNullOrWhiteSpace(Model.BankAccountStatus))
                    ShowStatus = true;

                await InvokeAsync(StateHasChanged);
                return;
            }

            // Set BranchName from selection
            var branch = BranchesList.FirstOrDefault(b => b.Id == Model.BranchId);
            if (branch != null) Model.BranchName = branch.BranchName;

            await Save();
        }

        public async Task Save()
        {
            isSaving = true;

            if (IsEdit)
            {
                await BankAccountService.UpdateBankAccountAsync(Model);
                ToastService.ShowSuccess($"Bank Account '{Model.BankAccountName}' updated successfully", "Updated");
            }
            else
            {
                Model.CreatedAt = DateTime.Now;
                await BankAccountService.CreateBankAccountAsync(Model);
                ToastService.ShowSuccess($"Bank Account '{Model.BankAccountName}' created successfully", "Created");
            }

            isSaving = false;
            Nav.NavigateTo("/bank-accounts");
        }
    }
}
