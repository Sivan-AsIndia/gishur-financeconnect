using FinanceConnect.Client.Data;
using FinanceConnect.Client.Services;
using FinanceConnect.Client.Shared;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Linq.Expressions;

namespace FinanceConnect.Client.Pages.Bank_Cash.BankTransaction;

public partial class CreateBankTransaction : ComponentBase
{
    [Parameter] public Guid? Id { get; set; }
    [Inject] BankTransactionService Service { get; set; } = default!;
    [Inject] NavigationManager Nav { get; set; } = default!;
    [Inject] IJSRuntime JS { get; set; } = default!;
    bool IsManualTransaction => Model.SourceModule == "Manual";

    public bool isInitialized = false;
    public BankTransactionModel Model = new();
    private EditContext _editContext = default!;

    bool ShowIdentity = true;
    bool ShowDetails = false;
    bool ShowDateAmount = false;
    bool ShowClassification = false;
    bool IdentityTouched = false;
    bool ShowDetailsTouched = false;
    bool showDateTouched = false;
    bool ShowClassificationTouched = false;
    List<AccountViewModel> BankAccounts = new();
    List<AccountViewModel> CashAccounts = new();

    // Validation error fields
    string? CompanyValidationError;
    string? BranchValidationError;
    string? BankAccountValidationError;
    string? CashAccountValidationError;
    string? SourceModuleValidationError;
    string? TransactionDateValidationError;
    string? ValueDateValidationError;
    string? AmountValidationError;
    string? DirectionValidationError;
    string? NarrationValidationError;
    string? TransactionTypeValidationError;
    string? PaymentMethodValidationError;
    private RichTextEditor descriptionEditor;
    void OnIdentityTouched() { IdentityTouched = true; }
    void OnInputShowDetails() { ShowDetailsTouched = true; }
    void OnChangeDateTime() { showDateTouched = true; TransactionDateValidationError = null; }
    void OnValueDateChanged() { showDateTouched = true; ValueDateValidationError = null; }
    void OnInputnarration() { showDateTouched = true; NarrationValidationError = null; }
    void ClassificationTouched() { ShowClassificationTouched = true; TransactionTypeValidationError = null; PaymentMethodValidationError = null; }
    void OnBankAccountChanged(ChangeEventArgs e)
    {
        if (Guid.TryParse(e.Value?.ToString(), out var id))
            Model.BankAccountId = id;
        else
            Model.BankAccountId = null;
        ShowDetailsTouched = true;
        BankAccountValidationError = null;
    }
    void OnCashAccountChanged(ChangeEventArgs e)
    {
        if (Guid.TryParse(e.Value?.ToString(), out var id))
            Model.CashAccountId = id;
        else
            Model.CashAccountId = null;
        ShowDetailsTouched = true;
        CashAccountValidationError = null;
    }
    void OnSourceModuleChanged(ChangeEventArgs e)
    {
        ShowDetailsTouched = true;
        SourceModuleValidationError = null;
        Model.SourceModule = e.Value?.ToString();
    }
    private void OnNarrationChanged(string value)
    {
        Model.Narration = value;

        if (string.IsNullOrWhiteSpace(value))
            NarrationValidationError = "Narration is required";
        else
            NarrationValidationError = null;
    }
    public enum SourceModule { AR, AP, CashBank, Reconciliation, Transfer, System }
    Dictionary<SourceModule, string> SourceModuleDisplay = new()
    {
        { SourceModule.AR, "AR" },
        { SourceModule.AP, "AP" },
        { SourceModule.CashBank, "CashBank (Manual)" },
        { SourceModule.Reconciliation, "Reconciliation" },
        { SourceModule.Transfer, "Transfer" },
        { SourceModule.System, "System" }
    };

    List<string> TransactionTypes = new()
    {
        "CustomerReceipt", "VendorPayment", "FundTransferOut", "FundTransferIn",
        "CashTransferOut", "CashTransferIn", "BankCharges", "InterestCredit",
        "RefundIn", "RefundOut", "CashDeposit", "CashWithdrawal",
        "ChequeIssued", "ChequeReceived", "ChequeCleared", "ChequeBounced",
        "Adjustment", "Other"
    };

    protected override void OnInitialized()
    {
        BankAccounts = new List<AccountViewModel>
        {
            new() { Id = Guid.NewGuid(), AccountName = "HDFC Current Account", AccountCode = "BANK001", IsBankAccount = true, AccountNature = "Asset" },
            new() { Id = Guid.NewGuid(), AccountName = "SBI Savings Account", AccountCode = "BANK002", IsBankAccount = true, AccountNature = "Asset" },
            new() { Id = Guid.NewGuid(), AccountName = "ICICI Business Account", AccountCode = "BANK003", IsBankAccount = true, AccountNature = "Asset" }
        };
        CashAccounts = new List<AccountViewModel>
        {
            new() { Id = Guid.NewGuid(), AccountName = "Main Office Cash", AccountCode = "CASH001", IsCashAccount = true, AccountNature = "Asset" },
            new() { Id = Guid.NewGuid(), AccountName = "Branch Cash Fund", AccountCode = "CASH002", IsCashAccount = true, AccountNature = "Asset" }
        };

        Model.TransactionNumber = Service.GenerateTransactionNumber();
    }

    bool IsEdit => Id.HasValue;
    public string PageTitle => IsEdit ? "Edit Bank Transaction" : "Create Bank Transaction";
    public string PageSubTitle => IsEdit ? "Update transaction" : "New transaction";

    public enum PaymentMethodEnum { Cash, Cheque, NEFT, RTGS, IMPS, UPI, Card, Wallet, BankTransfer, Other }
    public IEnumerable<PaymentMethodEnum> PaymentMethods =>
        Model.AccountKind == "Cash" ? new[] { PaymentMethodEnum.Cash } :
        Enum.GetValues<PaymentMethodEnum>().Cast<PaymentMethodEnum>().Where(m => m != PaymentMethodEnum.Cash);

    [Inject] private BranchService BranchService { get; set; } = default!;
    private List<BranchModel> Branches = new();
    public List<BranchModel> FilteredBranches { get; set; } = new();
    public bool IsBranchDisabled => Model.CompanyId == null || Model.CompanyId == Guid.Empty;
    public List<CompanyModel> Companies = new();

    void OnCompanyChanged(ChangeEventArgs e)
    {
        IdentityTouched = true;
        CompanyValidationError = null;
        if (Guid.TryParse(e.Value?.ToString(), out var companyId))
        {
            Model.CompanyId = companyId;
            Model.CompanyName = Companies.FirstOrDefault(c => c.Id == companyId)?.LegalName;
            Model.BranchId = null;
            Model.BranchName = null;
            FilteredBranches = BranchService.GetByCompany(companyId);
        }
        else
        {
            Model.CompanyId = null;
            Model.CompanyName = null;
            Model.BranchId = null;
            Model.BranchName = null;
            FilteredBranches = new();
        }
    }

    void OnBranchSelected(ChangeEventArgs e)
    {
        IdentityTouched = true;
        BranchValidationError = null;
        if (Guid.TryParse(e.Value?.ToString(), out var branchId))
        {
            Model.BranchId = branchId;
            Model.BranchName = FilteredBranches.FirstOrDefault(b => b.Id == branchId)?.BranchName;
        }
        else
        {
            Model.BranchId = null;
            Model.BranchName = null;
        }
    }

    protected override async Task OnInitializedAsync()
    {
        if (IsEdit)
        {
            var existing = await Service.GetByIdAsync(Id.Value);
            if (existing != null) Model = existing;
        }
        _editContext = new EditContext(Model);
        isInitialized = true;
        Branches = BranchService.GetAll();
        Companies = MasterDataService.GetAllCompanies();

        if (IsEdit && Model.CompanyId.HasValue && Model.CompanyId != Guid.Empty)
        {
            FilteredBranches = BranchService.GetByCompany(Model.CompanyId.Value);
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
            case "dateAmount": ShowDateAmount = !ShowDateAmount; break;
            case "classification": ShowClassification = !ShowClassification; break;
        }
    }

    private void RecalculateBaseAmount(ChangeEventArgs e)
    {
        showDateTouched = true;
        AmountValidationError = null;
        Model.BaseAmount = Math.Round(Model.Amount * Model.ExchangeRate, 2);
    }

    private bool ValidateAllFields()
    {
        bool isValid = true;
        CompanyValidationError = null;
        BranchValidationError = null;
        BankAccountValidationError = null;
        CashAccountValidationError = null;
        SourceModuleValidationError = null;
        TransactionDateValidationError = null;
        ValueDateValidationError = null;
        AmountValidationError = null;
        DirectionValidationError = null;
        NarrationValidationError = null;
        TransactionTypeValidationError = null;
        PaymentMethodValidationError = null;

        if (Model.CompanyId == null || Model.CompanyId == Guid.Empty)
        { CompanyValidationError = "Company is required"; isValid = false; }

        if (Model.BranchId == null || Model.BranchId == Guid.Empty)
        { BranchValidationError = "Branch is required"; isValid = false; }

        if (Model.AccountKind == "Bank" && (Model.BankAccountId == null || Model.BankAccountId == Guid.Empty))
        { BankAccountValidationError = "Bank Account is required"; isValid = false; }

        if (Model.AccountKind == "Cash" && (Model.CashAccountId == null || Model.CashAccountId == Guid.Empty))
        { CashAccountValidationError = "Cash Account is required"; isValid = false; }

        if (string.IsNullOrWhiteSpace(Model.SourceModule))
        { SourceModuleValidationError = "Source Module is required"; isValid = false; }

        if (Model.TransactionDate == default)
        { TransactionDateValidationError = "Transaction Date is required"; isValid = false; }

        if (Model.AccountKind == "Bank" && !Model.ValueDate.HasValue)
        { ValueDateValidationError = "Value Date is required for bank transactions"; isValid = false; }

        if (Model.Amount <= 0)
        { AmountValidationError = "Amount must be greater than zero"; isValid = false; }

        if (string.IsNullOrWhiteSpace(Model.Direction))
        { DirectionValidationError = "Direction is required"; isValid = false; }

        if (IsManualTransaction && string.IsNullOrWhiteSpace(Model.Narration))
        { NarrationValidationError = "Narration is required for manual transactions"; isValid = false; }

        if (string.IsNullOrWhiteSpace(Model.TransactionType))
        { TransactionTypeValidationError = "Transaction Type is required"; isValid = false; }

        if (string.IsNullOrWhiteSpace(Model.PaymentMethod))
        { PaymentMethodValidationError = "Payment Method is required"; isValid = false; }

        return isValid;
    }

    async Task HandleSubmit()
    {
        if (Model.TransactionDate > Model.PostingDate)
        {

            ToastService.ShowError("Posting Date should not be earlier then Debit Note Date");
            return;
        }
        if (!IsEdit && string.IsNullOrEmpty(Model.TransactionNumber))
        

        if (!ValidateAllFields())
        {
            if (CompanyValidationError != null || BranchValidationError != null) ShowIdentity = true;
            if (BankAccountValidationError != null || CashAccountValidationError != null || SourceModuleValidationError != null) ShowDetails = true;
            if (TransactionDateValidationError != null || ValueDateValidationError != null || AmountValidationError != null || DirectionValidationError != null || NarrationValidationError != null) ShowDateAmount = true;
            if (TransactionTypeValidationError != null || PaymentMethodValidationError != null) ShowClassification = true;
            await InvokeAsync(StateHasChanged);
            return;
        }
        await Save();
    }

    async Task Save()
    {
        if (IsEdit)
        {
            await Service.UpdateAsync(Model.Id, Model);
            ToastService.ShowSuccess("Transaction Updated Successfully", "Success");
        }
        else
        {
            await Service.CreateAsync(Model);
            ToastService.ShowSuccess("Transaction Created Successfully", "Success");
        }
        Nav.NavigateTo("/bank-transactions");
    }

    private string GetValidationClass(Expression<Func<object>> field)
    {
        if (_editContext == null) return string.Empty;
        var fieldIdentifier = FieldIdentifier.Create(field);
        var hasError = _editContext.GetValidationMessages(fieldIdentifier).Any();
        var isModified = _editContext.IsModified(fieldIdentifier);
        if (hasError) return "is-invalid";
        if (isModified) return "is-valid";
        return string.Empty;
    }
}
