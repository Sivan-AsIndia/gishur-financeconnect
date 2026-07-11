using FinanceConnect.Client.Pages.Finance.Journal;
using FinanceConnect.Client.Services;
using FinanceConnect.Client.Shared;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.COA;

public partial class AccountForm : ComponentBase
{
    [Inject] private NavigationManager Nav { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Inject] private COADataService COADataService { get; set; } = default!;
    [Inject] private ToastService ToastService { get; set; } = default!;

    [Parameter] public Guid? Id { get; set; }

    private AccountViewModel Model { get; set; } = new();
    private List<ChartOfAccountsViewModel> ChartOfAccountsList { get; set; } = new();
    private List<AccountGroupViewModel> AccountGroups { get; set; } = new();
    private List<AccountGroupViewModel> FilteredGroups { get; set; } = new();
    private List<AccountViewModel> AllAccounts { get; set; } = new();
    private List<AccountViewModel> HeaderAccounts { get; set; } = new();
    private bool IsEditMode => Id.HasValue;
    private bool isLoading = true;
    private bool isSaving = false;

    RichTextEditor? _descriptionEditor;
    RichTextEditor? _lockReasonEditor;

    // Page titles
    private string PageTitle => IsEditMode ? "Edit GL Account" : "Add GL Account";
    private string PageSubTitle => IsEditMode ? "Update account information" : "Create a new GL account";

    // Accordion visibility states
    private bool ShowIdentity = true;
    private bool ShowClassification = false;
    private bool ShowBehavior = false;
    private bool ShowSpecial = false;
    private bool ShowStatus = false;

    // Accordion touched states
    private bool IdentityTouched = false;
    private bool ClassificationTouched = false;
    private bool BehaviorTouched = false;
    private bool SpecialTouched = false;
    private bool StatusTouched = false;

    // Validation errors
    private string? chartError;
    private string? groupError;
    private string? codeError;
    private string? nameError;
    private string? dateError;
    private string? statusError;
    private string? controlTypeError;
    private string? taxTypeError;
    private string? lockReasonError;
    private string? errorMessage;

    // Wrapper properties for binding with validation clearing
    private string? ChartIdWrapper
    {
        get => Model.ChartOfAccountsId?.ToString();
        set
        {
            if (Guid.TryParse(value, out var id))
                Model.ChartOfAccountsId = id;
            else
                Model.ChartOfAccountsId = null;
        }
    }

    private string? GroupIdWrapper
    {
        get => Model.AccountGroupId == Guid.Empty ? null : Model.AccountGroupId.ToString();
        set
        {
            if (Guid.TryParse(value, out var id))
                Model.AccountGroupId = id;
            else
                Model.AccountGroupId = Guid.Empty;
        }
    }

    private string? ParentAccountIdWrapper
    {
        get => Model.ParentAccountId?.ToString();
        set
        {
            if (Guid.TryParse(value, out var id))
                Model.ParentAccountId = id;
            else
                Model.ParentAccountId = null;
        }
    }

    private string CodeWrapper
    {
        get => Model.AccountCode;
        set => Model.AccountCode = value?.Trim().ToUpperInvariant() ?? "";
    }

    private string NameWrapper
    {
        get => Model.AccountName;
        set => Model.AccountName = value?.Trim() ?? "";
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await JS.InvokeVoidAsync("feather.replace");
    }

    private async Task LoadDataAsync()
    {
        isLoading = true;
        try
        {
            ChartOfAccountsList = await COADataService.GetChartOfAccountsAsync();
            AccountGroups = await COADataService.GetAccountGroupsAsync();
            AllAccounts = await COADataService.GetAccountsAsync();

            if (IsEditMode && Id.HasValue)
            {
                var account = await COADataService.GetAccountByIdAsync(Id.Value);
                if (account != null)
                {
                    Model = account;
                    FilterGroupsByChart();
                    FilterHeaderAccounts();
                    DeriveClassificationFields();

                    // In edit mode, expand all sections
                    //ShowIdentity = true;
                    //ShowClassification = true;
                    //ShowBehavior = true;
                    //ShowSpecial = true;
                    //ShowStatus = true;
                }
                else
                {
                    ToastService.ShowError("Account not found");
                    NavigateBack();
                }
            }
            else
            {
                Model = new AccountViewModel
                {
                    Id = Guid.NewGuid(),
                    IsPostable = true,
                    RequiresBranch = true,
                    AllowManualJournal = true,
                    Status = AccountStatuses.Active,
                    EffectiveFrom = DateTime.Today
                };
            }
        }
        finally
        {
            isLoading = false;
        }
    }

    private void ToggleAccordion(string section)
    {
        switch (section)
        {
            case "identity":
                ShowIdentity = !ShowIdentity;
                IdentityTouched = true;
                break;
            case "classification":
                ShowClassification = !ShowClassification;
                ClassificationTouched = true;
                break;
            case "behavior":
                ShowBehavior = !ShowBehavior;
                BehaviorTouched = true;
                break;
            case "special":
                ShowSpecial = !ShowSpecial;
                SpecialTouched = true;
                break;
            case "status":
                ShowStatus = !ShowStatus;
                StatusTouched = true;
                break;
        }
    }

    private void OpenAccordion(string section)
    {
        switch (section)
        {
            case "identity":
                ShowIdentity = true;
                IdentityTouched = true;
                break;
            case "classification":
                ShowClassification = true;
                ClassificationTouched = true;
                break;
            case "behavior":
                ShowBehavior = true;
                BehaviorTouched = true;
                break;
            case "special":
                ShowSpecial = true;
                SpecialTouched = true;
                break;
            case "status":
                ShowStatus = true;
                StatusTouched = true;
                break;
        }
    }

    private void OnChartChanged(ChangeEventArgs e)
    {
        ChartIdWrapper = e.Value?.ToString();
        chartError = null;
        IdentityTouched = true;

        // Reset group and parent when chart changes
        Model.AccountGroupId = Guid.Empty;
        Model.AccountNature = string.Empty;
        Model.ParentAccountId = null;
        Model.ParentAccountName = null;

        FilterGroupsByChart();
        FilterHeaderAccounts();
        DeriveClassificationFields();
    }

    private void FilterGroupsByChart()
    {
        if (Model.ChartOfAccountsId.HasValue && Model.ChartOfAccountsId != Guid.Empty)
        {
            FilteredGroups = AccountGroups
                .Where(g => g.ChartOfAccountsId == Model.ChartOfAccountsId.Value)
                .OrderBy(g => g.GroupCode)
                .ToList();
        }
        else
        {
            FilteredGroups = new List<AccountGroupViewModel>();
        }
    }

    private void FilterHeaderAccounts()
    {
        if (Model.ChartOfAccountsId.HasValue && Model.ChartOfAccountsId != Guid.Empty)
        {
            HeaderAccounts = AllAccounts
                .Where(a => a.ChartOfAccountsId == Model.ChartOfAccountsId.Value
                            && !a.IsPostable
                            && a.Id != Model.Id)
                .OrderBy(a => a.AccountCode)
                .ToList();
        }
        else
        {
            HeaderAccounts = new List<AccountViewModel>();
        }
    }

    private void OnGroupChanged(ChangeEventArgs e)
    {
        GroupIdWrapper = e.Value?.ToString();
        groupError = null;
        IdentityTouched = true;

        // Auto-fill nature from selected group
        if (Model.AccountGroupId != Guid.Empty)
        {
            var selectedGroup = AccountGroups.FirstOrDefault(g => g.Id == Model.AccountGroupId);
            if (selectedGroup != null)
            {
                Model.AccountNature = selectedGroup.AccountNature;
                Model.AccountGroupName = selectedGroup.GroupName;
                Model.AccountGroupCode = selectedGroup.GroupCode;
            }
        }
        else
        {
            Model.AccountNature = string.Empty;
        }

        DeriveClassificationFields();
    }

    private void OnParentAccountChanged(ChangeEventArgs e)
    {
        ParentAccountIdWrapper = e.Value?.ToString();
        IdentityTouched = true;

        if (Model.ParentAccountId.HasValue)
        {
            var parent = AllAccounts.FirstOrDefault(a => a.Id == Model.ParentAccountId.Value);
            Model.ParentAccountName = parent?.AccountName;
        }
        else
        {
            Model.ParentAccountName = null;
        }
    }

    private void OnCodeChanged()
    {
        codeError = null;
        IdentityTouched = true;
    }

    private void OnNameChanged()
    {
        nameError = null;
        IdentityTouched = true;
    }

    private void OnControlAccountChanged()
    {
        controlTypeError = null;
        BehaviorTouched = true;

        // When control account is toggled ON, default AllowManualJournal to OFF
        if (Model.IsControlAccount)
        {
            Model.AllowManualJournal = false;
        }
        else
        {
            Model.AllowManualJournal = true;
            Model.ControlAccountType = null;
        }
    }

    /// <summary>
    /// Derive StatementType, NormalBalance, IsBalanceSheetAccount from AccountNature
    /// </summary>
    private void DeriveClassificationFields()
    {
        switch (Model.AccountNature)
        {
            case AccountNatures.Asset:
            case AccountNatures.Expense:
                Model.NormalBalance = BalanceBehaviors.Debit;
                break;
            case AccountNatures.Liability:
            case AccountNatures.Equity:
            case AccountNatures.Income:
                Model.NormalBalance = BalanceBehaviors.Credit;
                break;
            default:
                Model.NormalBalance = null;
                break;
        }

        switch (Model.AccountNature)
        {
            case AccountNatures.Asset:
            case AccountNatures.Liability:
            case AccountNatures.Equity:
                Model.StatementType = StatementTypes.BalanceSheet;
                Model.IsBalanceSheetAccount = true;
                break;
            case AccountNatures.Income:
            case AccountNatures.Expense:
                Model.StatementType = StatementTypes.ProfitAndLoss;
                Model.IsBalanceSheetAccount = false;
                break;
            default:
                Model.StatementType = null;
                Model.IsBalanceSheetAccount = false;
                break;
        }
    }

    private string GetStatementTypeDisplay()
    {
        return Model.StatementType switch
        {
            StatementTypes.BalanceSheet => "Balance Sheet",
            StatementTypes.ProfitAndLoss => "Profit & Loss",
            _ => ""
        };
    }

    private string GetNormalBalanceDisplay()
    {
        return Model.NormalBalance ?? "";
    }

    private bool ValidateForm()
    {
        bool isValid = true;

        // Clear all validation errors first
        chartError = null;
        groupError = null;
        codeError = null;
        nameError = null;
        dateError = null;
        statusError = null;
        controlTypeError = null;
        taxTypeError = null;
        lockReasonError = null;
        errorMessage = null;

        // Validate Chart
        if (!Model.ChartOfAccountsId.HasValue || Model.ChartOfAccountsId == Guid.Empty)
        {
            chartError = "Chart of Accounts is required";
            isValid = false;
        }

        // Validate Group
        if (Model.AccountGroupId == Guid.Empty)
        {
            groupError = "Account Group is required";
            isValid = false;
        }

        // Validate Code
        if (string.IsNullOrWhiteSpace(Model.AccountCode))
        {
            codeError = "Account Code is required";
            isValid = false;
        }
        else if (Model.AccountCode.Length > 20)
        {
            codeError = "Account Code cannot exceed 20 characters";
            isValid = false;
        }

        // Validate Name
        if (string.IsNullOrWhiteSpace(Model.AccountName))
        {
            nameError = "Account Name is required";
            isValid = false;
        }
        else if (Model.AccountName.Length > 200)
        {
            nameError = "Account Name cannot exceed 200 characters";
            isValid = false;
        }

        // Validate Status
        if (string.IsNullOrWhiteSpace(Model.Status))
        {
            statusError = "Status is required";
            isValid = false;
        }

        // Validate ControlAccountType (required if IsControlAccount = true)
        if (Model.IsControlAccount && string.IsNullOrWhiteSpace(Model.ControlAccountType))
        {
            controlTypeError = "Control Account Type is required when Control Account is enabled";
            isValid = false;
        }

        // Validate TaxType (required if IsTaxAccount = true)
        if (Model.IsTaxAccount && string.IsNullOrWhiteSpace(Model.TaxType))
        {
            taxTypeError = "Tax Type is required when Tax Account is enabled";
            isValid = false;
        }

        // Validate Effective Dates
        if (Model.EffectiveFrom.HasValue && Model.EffectiveTo.HasValue)
        {
            if (Model.EffectiveTo < Model.EffectiveFrom)
            {
                dateError = "Effective To date cannot be before Effective From date";
                isValid = false;
            }
        }

        // Validate LockReason (required if status is Inactive or Closed, or if LockStatus is set)
        if ((Model.Status == AccountStatuses.Inactive || Model.Status == AccountStatuses.Closed)
            && string.IsNullOrWhiteSpace(Model.LockReason))
        {
            lockReasonError = "Lock / Inactivation Reason is required when status is Inactive or Closed";
            isValid = false;
        }

        return isValid;
    }

    private bool HasIdentityErrors()
    {
        return chartError != null || groupError != null || codeError != null || nameError != null;
    }

    private bool HasBehaviorErrors()
    {
        return controlTypeError != null;
    }

    private bool HasSpecialErrors()
    {
        return taxTypeError != null;
    }

    private bool HasStatusErrors()
    {
        return statusError != null || dateError != null || lockReasonError != null;
    }

    private async Task HandleSubmit()
    {
        // Collect Quill editor values before validation
        if (_descriptionEditor != null)
            Model.Description = await _descriptionEditor.GetHtmlAsync();

        if (_lockReasonEditor != null)
            Model.LockReason = await _lockReasonEditor.GetHtmlAsync();
        // Derive classification fields before validation
        DeriveClassificationFields();

        if (!ValidateForm())
        {
            // Open ALL accordions that have validation errors
            if (HasIdentityErrors())
                OpenAccordion("identity");
            if (HasBehaviorErrors())
                OpenAccordion("behavior");
            if (HasSpecialErrors())
                OpenAccordion("special");
            if (HasStatusErrors())
                OpenAccordion("status");

            await InvokeAsync(StateHasChanged);
            return;
        }

        isSaving = true;
        try
        {
            // Trim fields and reset DisplayOrder
            Model.AccountCode = Model.AccountCode?.Trim();
            Model.AccountName = Model.AccountName?.Trim();
            Model.Description = Model.Description?.Trim();
            Model.LockReason = Model.LockReason?.Trim();
            if (Model.DisplayOrder < 0) Model.DisplayOrder = 0;

            // Check for duplicate code
            var isDuplicate = await COADataService.IsAccountCodeDuplicateAsync(
                Model.AccountCode,
                Model.ChartOfAccountsId!.Value,
                IsEditMode ? Model.Id : null);

            if (isDuplicate)
            {
                codeError = "Account Code already exists in this Chart of Accounts";
                OpenAccordion("identity");
                return;
            }

            bool result;
            if (IsEditMode)
            {
                Model.UpdatedAt = DateTime.UtcNow;
                result = await COADataService.UpdateAccountAsync(Model);
            }
            else
            {
                Model.CreatedAt = DateTime.UtcNow;
                result = await COADataService.AddAccountAsync(Model);
            }

            if (result)
            {
                ToastService.ShowSuccess(IsEditMode
                    ? $"Account '{Model.AccountName}' updated successfully"
                    : $"Account '{Model.AccountName}' created successfully");
                NavigateBack();
            }
            else
            {
                errorMessage = "Failed to save account. Please try again.";
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"Error: {ex.Message}";
        }
        finally
        {
            isSaving = false;
        }
    }

    private void NavigateBack() => Nav.NavigateTo("/gl-accounts");
}
