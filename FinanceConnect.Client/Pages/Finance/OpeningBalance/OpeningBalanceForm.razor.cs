using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Finance.OpeningBalance;

public partial class OpeningBalanceForm : ComponentBase
{
    [Parameter] public Guid? Id { get; set; }

    private bool isInitialized = false;
    private bool isSaving = false;
    private bool IsEditMode => Id.HasValue && Id.Value != Guid.Empty;

    // Model
    private OpeningBalanceModel Model = new();

    // Lookup Data
    private List<CompanyModel> Companies = new();
    private List<BranchModel> Branches = new();
    private List<BranchModel> FilteredBranches = new();
    private List<LedgerModel> Ledgers = new();
    private List<LedgerModel> FilteredLedgers = new();
    private List<FiscalYearModel> FiscalYears = new();
    private List<AccountingPeriodModel> AccountingPeriods = new();
    private List<AccountingPeriodModel> FilteredAccountingPeriods = new();
    private List<AccountViewModel> Accounts = new();

    // Selected IDs (for cascading dropdowns)
    private string _selectedCompanyId = string.Empty;
    private string _selectedBranchId = string.Empty;
    private string _selectedLedgerId = string.Empty;
    private string _selectedFiscalYearId = string.Empty;
    private string _selectedAccountingPeriodId = string.Empty;

    // Totals
    private decimal TotalDebit => Model.Lines.Sum(l => l.DebitAmountBase);
    private decimal TotalCredit => Model.Lines.Sum(l => l.CreditAmountBase);
    private decimal Difference => Math.Abs(TotalDebit - TotalCredit);
    private bool IsBalanced => TotalDebit == TotalCredit && TotalDebit > 0;

    // Inline validation error messages
    private string? CompanyValidationError;
    private string? BranchValidationError;
    private string? LedgerValidationError;
    private string? FiscalYearValidationError;
    private string? AccountingPeriodValidationError;
    private string? OpeningDateValidationError;
    private string? LinesValidationError;

    public string SelectedCompanyId
    {
        get => _selectedCompanyId;
        set
        {
            if (_selectedCompanyId != value)
            {
                _selectedCompanyId = value;
                OnCompanyChanged();
            }
        }
    }

    public string SelectedBranchId
    {
        get => _selectedBranchId;
        set
        {
            if (_selectedBranchId != value)
            {
                _selectedBranchId = value;
                if (Guid.TryParse(value, out var branchGuid))
                {
                    Model.BranchId = branchGuid;
                    var branch = Branches.FirstOrDefault(b => b.Id == branchGuid);
                    if (branch != null)
                    {
                        Model.BranchCode = branch.BranchCode;
                        Model.BranchName = branch.BranchName;
                    }
                }
            }
        }
    }

    public string SelectedLedgerId
    {
        get => _selectedLedgerId;
        set
        {
            if (_selectedLedgerId != value)
            {
                _selectedLedgerId = value;
                if (Guid.TryParse(value, out var ledgerGuid))
                {
                    Model.LedgerId = ledgerGuid;
                    var ledger = Ledgers.FirstOrDefault(l => l.Id == ledgerGuid);
                    if (ledger != null)
                    {
                        Model.LedgerCode = ledger.LedgerCode;
                        Model.LedgerName = ledger.LedgerName;
                    }
                }
            }
        }
    }

    public string SelectedFiscalYearId
    {
        get => _selectedFiscalYearId;
        set
        {
            if (_selectedFiscalYearId != value)
            {
                _selectedFiscalYearId = value;
                OnFiscalYearChanged();
            }
        }
    }

    public string SelectedAccountingPeriodId
    {
        get => _selectedAccountingPeriodId;
        set
        {
            if (_selectedAccountingPeriodId != value)
            {
                _selectedAccountingPeriodId = value;
                if (Guid.TryParse(value, out var periodGuid))
                {
                    Model.OpeningAccountingPeriodId = periodGuid;
                    var period = AccountingPeriods.FirstOrDefault(p => p.Id == periodGuid);
                    if (period != null)
                    {
                        Model.OpeningAccountingPeriodCode = period.PeriodCode;
                        Model.OpeningAccountingPeriodName = period.PeriodName;
                    }
                }
            }
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadLookupDataAsync();

        if (IsEditMode)
        {
            await LoadExistingData();
        }
        else
        {
            InitializeNewModel();
        }

        isInitialized = true;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (isInitialized)
        {
            await JS.InvokeVoidAsync("feather.replace");
        }
    }

    private async Task LoadLookupDataAsync()
    {
        Companies = MasterDataService.GetAllCompanies();
        Branches = BranchService.GetAll();
        Ledgers = FinanceDataService.GetAllLedgers();
        FiscalYears = FinanceDataService.GetAllFiscalYears();
        AccountingPeriods = FinanceDataService.GetAllAccountingPeriods();
        Accounts = FinanceDataService.GetAllAccounts();

        FilteredBranches = Branches.ToList();
        FilteredLedgers = Ledgers.ToList();
        FilteredAccountingPeriods = AccountingPeriods.ToList();

        await Task.CompletedTask;
    }

    private async Task LoadExistingData()
    {
        var existing = FinanceDataService.GetOpeningBalanceById(Id!.Value);
        if (existing == null)
        {
            ToastService.ShowError("Opening Balance not found");
            Nav.NavigateTo("/opening-balance");
            return;
        }

        Model = existing;

        // Set selected IDs
        _selectedCompanyId = Model.CompanyId.ToString();
        _selectedBranchId = Model.BranchId.ToString();
        _selectedLedgerId = Model.LedgerId.ToString();
        _selectedFiscalYearId = Model.FiscalYearId.ToString();
        _selectedAccountingPeriodId = Model.OpeningAccountingPeriodId.ToString();

        // Update filtered lists
        OnCompanyChanged(skipReset: true);
        OnFiscalYearChanged(skipReset: true);

        await Task.CompletedTask;
    }

    private void InitializeNewModel()
    {
        Model = new OpeningBalanceModel
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            OpeningBalanceNumber = $"OB-{DateTime.Now:yyyy}-{DateTime.Now:HHmmss}",
            OpeningDate = DateTime.Today,
            EntryMode = EntryModes.ManualEntry,
            CurrencyMode = CurrencyModes.SingleCurrencyOnly,
            RestrictToBalanceSheetAccounts = true,
            Status = OpeningBalanceStatus.Draft,
            CreatedAt = DateTime.Now,
            CreatedBy = AuthService.CurrentUser?.UserName ?? "System",
            Lines = new List<OpeningBalanceLineModel>
            {
                new OpeningBalanceLineModel { Id = Guid.NewGuid() }
            }
        };
    }

    private void OnCompanyChanged(bool skipReset = false)
    {
        if (Guid.TryParse(_selectedCompanyId, out var companyGuid))
        {
            Model.CompanyId = companyGuid;
            var company = Companies.FirstOrDefault(c => c.Id == companyGuid);
            if (company != null)
            {
                Model.CompanyCode = company.CompanyCode;
                Model.CompanyName = company.LegalName;

                // Filter branches by company name
                FilteredBranches = Branches.Where(b => b.Company == company.LegalName).ToList();

                // Filter ledgers by company
                FilteredLedgers = Ledgers.Where(l => l.CompanyId == companyGuid).ToList();
            }
        }
        else
        {
            FilteredBranches = Branches.ToList();
            FilteredLedgers = Ledgers.ToList();
        }

        if (!skipReset)
        {
            _selectedBranchId = string.Empty;
            _selectedLedgerId = string.Empty;
            Model.BranchId = Guid.Empty;
            Model.LedgerId = Guid.Empty;
        }

        StateHasChanged();
    }

    private void OnFiscalYearChanged(bool skipReset = false)
    {
        if (Guid.TryParse(_selectedFiscalYearId, out var fyGuid))
        {
            Model.FiscalYearId = fyGuid;
            var fiscalYear = FiscalYears.FirstOrDefault(fy => fy.Id == fyGuid);
            if (fiscalYear != null)
            {
                Model.FiscalYearCode = fiscalYear.FiscalYearCode;
                Model.FiscalYearName = fiscalYear.FiscalYearName;

                // Filter accounting periods by fiscal year
                FilteredAccountingPeriods = AccountingPeriods.Where(p => p.FiscalYearId == fyGuid).ToList();

                // Set opening date to fiscal year start if not set
                if (Model.OpeningDate == DateTime.MinValue || Model.OpeningDate == DateTime.Today)
                {
                    Model.OpeningDate = fiscalYear.StartDate ?? DateTime.Today;
                }
            }
        }
        else
        {
            FilteredAccountingPeriods = AccountingPeriods.ToList();
        }

        if (!skipReset)
        {
            _selectedAccountingPeriodId = string.Empty;
            Model.OpeningAccountingPeriodId = Guid.Empty;
        }

        StateHasChanged();
    }

    private void OnAccountSelected(int lineIndex)
    {
        var line = Model.Lines[lineIndex];
        var account = Accounts.FirstOrDefault(a => a.Id == line.AccountId);
        if (account != null)
        {
            line.AccountCode = account.AccountCode;
            line.AccountName = account.AccountName;
            line.AccountNature = account.AccountNature;
        }
    }

    private void AddLine()
    {
        Model.Lines.Add(new OpeningBalanceLineModel
        {
            Id = Guid.NewGuid(),
            OpeningBalanceId = Model.Id
        });
        StateHasChanged();
    }

    private void RemoveLine(int index)
    {
        if (Model.Lines.Count > 1)
        {
            Model.Lines.RemoveAt(index);
            RecalculateTotals();
        }
    }

    private void RecalculateTotals()
    {
        StateHasChanged();
    }

    private void ClearValidationErrors()
    {
        CompanyValidationError = null;
        BranchValidationError = null;
        LedgerValidationError = null;
        FiscalYearValidationError = null;
        AccountingPeriodValidationError = null;
        OpeningDateValidationError = null;
        LinesValidationError = null;
    }

    private bool ValidateAllFields()
    {
        ClearValidationErrors();
        bool isValid = true;

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

        if (Model.LedgerId == Guid.Empty)
        {
            LedgerValidationError = "Ledger is required";
            isValid = false;
        }

        if (Model.FiscalYearId == Guid.Empty)
        {
            FiscalYearValidationError = "Fiscal Year is required";
            isValid = false;
        }

        if (Model.OpeningAccountingPeriodId == Guid.Empty)
        {
            AccountingPeriodValidationError = "Accounting Period is required";
            isValid = false;
        }

        if (Model.OpeningDate == DateTime.MinValue)
        {
            OpeningDateValidationError = "Opening Date is required";
            isValid = false;
        }

        // Validate lines
        var validLines = Model.Lines.Where(l => l.AccountId != Guid.Empty && (l.DebitAmountBase > 0 || l.CreditAmountBase > 0)).ToList();
        if (!validLines.Any())
        {
            LinesValidationError = "Please add at least one line with an account and amount";
            isValid = false;
        }
        else if (validLines.Any(l => l.DebitAmountBase > 0 && l.CreditAmountBase > 0))
        {
            LinesValidationError = "A line cannot have both debit and credit amounts";
            isValid = false;
        }

        return isValid;
    }

    private async Task HandleSubmit()
    {
        if (!ValidateAllFields())
        {
            StateHasChanged();
            return;
        }

        // Filter only valid lines
        Model.Lines = Model.Lines.Where(l => l.AccountId != Guid.Empty && (l.DebitAmountBase > 0 || l.CreditAmountBase > 0)).ToList();

        // Set OpeningBalanceId on lines
        foreach (var line in Model.Lines)
        {
            line.OpeningBalanceId = Model.Id;
        }

        isSaving = true;
        StateHasChanged();

        try
        {
            if (IsEditMode)
            {
                Model.UpdatedAt = DateTime.Now;
                Model.UpdatedBy = AuthService.CurrentUser?.UserName ?? "System";
                FinanceDataService.UpdateOpeningBalance(Model);
                ToastService.ShowSuccess("Opening Balance updated successfully");
            }
            else
            {
                FinanceDataService.CreateOpeningBalance(Model);
                ToastService.ShowSuccess("Opening Balance created successfully");
            }

            Nav.NavigateTo("/opening-balance");
        }
        catch (Exception ex)
        {
            ToastService.ShowError($"Error saving: {ex.Message}");
        }
        finally
        {
            isSaving = false;
            StateHasChanged();
        }

        await Task.CompletedTask;
    }
}
