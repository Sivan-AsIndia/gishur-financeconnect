using FinanceConnect.Client.Services;
using FinanceConnect.Client.Shared;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq.Expressions;
using static FinanceConnect.Client.Data.MasterDataIds;

namespace FinanceConnect.Client.Pages.Bank_Cash.CashAccount;

public partial class CreateCashAccount : ComponentBase
{
    protected CashAccountModels Model = new();
    [Inject]
    private BranchService BranchService { get; set; } = default!;
    [Inject]  private MasterDataService MasterDataService { get; set; } = default!;

    private List<BranchModel> Branches = new();
    private List<CurrencyModel> Currencies = new();
    [Inject] NavigationManager NavigationManager { get; set; } = default!;
    [Inject] CashAccountService CashService { get; set; } = default!;
    [Parameter] public string? Code { get; set; }
    private bool IsEditMode => !string.IsNullOrEmpty(Code);
    private EditContext _editContext = default!;
    private bool _submitted = false;
    public List<BranchModel> FilteredBranches { get; set; } = new();
    public List<CompanyModel> Companies = new();

    // Inline validation error messages
    private string? BranchValidationError;
    private string? CompanyValidationError;
    private string? CodeValidationError;
    private string? NameValidationError;
    private string? CurrencyValidationError;
    private string? GlAccountValidationError;
    private string? StatusValidationError;
    private string? CloseReasonValidationError;
    private RichTextEditor descriptionEditor;
    public bool IsBranchDisabled => !IsEditMode && (Model.CompanyId == Guid.Empty);

    protected override void OnInitialized()
    {
        Companies = MasterDataService.GetAllCompanies()
                         .Where(c => c.Status == "Active")
                         .ToList();
        Branches = BranchService.GetAll();
        Currencies = MasterDataService.GetAllCurrencies();
        // Defaults
        Model.Status = "";
        Model.CurrencyCode = "INR";
        if (IsEditMode)
        {
            var existing = CashService.GetByCode(Code!);
            if (existing != null)
            {
                Model = new CashAccountModels
                {
                    Code = existing.Code,
                    Name = existing.Name,
                    Description = existing.Description,
                    BranchId = existing.BranchId,
                    CompanyId = existing.CompanyId,
                    BranchName = existing.BranchName,
                    CustodianName = existing.CustodianName,
                    CustodyStartDate = existing.CustodyStartDate,
                    CustodyNotes = existing.CustodyNotes,
                    CurrencyId = existing.CurrencyId,
                    CashGlAccount = existing.CashGlAccount,
                    MaxCashLimit = existing.MaxCashLimit,
                    RequireAttachmentAboveAmount = existing.RequireAttachmentAboveAmount,
                    RequireReasonWhenExceedingLimit = existing.RequireReasonWhenExceedingLimit,
                    IsNegativeBalanceAllowed = existing.IsNegativeBalanceAllowed,
                    IsLockedForTransactions = existing.IsLockedForTransactions,
                    Status = existing.Status,
                    CloseReason = existing.CloseReason
                };
                LoadBranchesForCompany();

            }
        }
        _editContext = new EditContext(Model);

    }
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

    void OnCompanyChanged() { CompanyValidationError = null; }

    private void OnBranchChanged()
    {
        BranchValidationError = null;
        Model.BranchName = Branches.FirstOrDefault(x => x.Id == Model.BranchId)?.BranchName ?? "";
    }
    private void OnCashAccountCodeChanged()
    {
        Model.Code = Model.Code.Trim().ToUpperInvariant();
    }


    private void OnCashAccountNameChanged()
    {
        Model.Name = Model.Name.Trim();
    }
    private void ClearValidationErrors()
    {
        BranchValidationError = null;
        CodeValidationError = null;
        NameValidationError = null;
        CurrencyValidationError = null;
        GlAccountValidationError = null;
        StatusValidationError = null;
        CloseReasonValidationError = null;
    }

    private bool ValidateAllFields()
    {
        ClearValidationErrors();
        bool isValid = true;

        if (Model.BranchId == Guid.Empty)
        {
            BranchValidationError = "Branch is required";
            isValid = false;
        }
        if (Model.CompanyId == Guid.Empty)
        {
            CompanyValidationError = "company is required";
            isValid = false;
        }

        if (string.IsNullOrWhiteSpace(Model.Code))
        {
            CodeValidationError = "Cash Account Code is required";
            isValid = false;
        }

        if (string.IsNullOrWhiteSpace(Model.Name))
        {
            NameValidationError = "Cash Account Name is required";
            isValid = false;
        }

        if (Model.CurrencyId == Guid.Empty)
        {
            CurrencyValidationError = "Currency is required";
            isValid = false;
        }

        if (string.IsNullOrWhiteSpace(Model.CashGlAccount))
        {
            GlAccountValidationError = "Cash GL Account is required";
            isValid = false;
        }

        if (string.IsNullOrWhiteSpace(Model.Status))
        {
            StatusValidationError = "Status is required";
            isValid = false;
        }

        if (Model.Status == "Closed" && string.IsNullOrWhiteSpace(Model.CloseReason))
        {
            CloseReasonValidationError = "Close Reason is required when status is Closed";
            isValid = false;
        }

        return isValid;
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


    protected void Save()
    {
        _submitted = true;

        if (!ValidateAllFields())
        {
            return;
        }

        Model.BranchName = Branches
           .FirstOrDefault(x => x.Id == Model.BranchId)
           ?.BranchName ?? "";

        if (IsEditMode)
        {
            CashService.Update(Model);
            ToastService.ShowSuccess("Successfully Updated", "Success");

        }
        else
        {
            Model.CreatedAt = DateTime.Now;
            Model.CreatedBy = "Admin";
            CashService.Add(Model);
            ToastService.ShowSuccess("Successfully Created", "Success");

        }

        NavigationManager.NavigateTo("/cash-accounts");
    }
}
