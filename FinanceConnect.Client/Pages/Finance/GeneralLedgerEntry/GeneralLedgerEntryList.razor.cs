using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Finance.GeneralLedgerEntry;

public partial class GeneralLedgerEntryList
{
    private bool isInitialized = false;
    private bool isLoading = false;

    // Data collections
    private List<GeneralLedgerEntryModel> Entries = new();
    private List<GeneralLedgerEntryModel> FilteredEntries = new();

    // Filter dropdown data
    private List<(Guid Id, string Code, string Name)> Branches = new();
    private List<(Guid Id, string Code, string Name)> Periods = new();
    private List<(Guid Id, string Code, string Name)> Accounts = new();
    private List<(Guid Id, string Code, string Name)> Companies = new();

    // Filter values
    private string searchText = "";
    private string selectedBranchId = "";
    private string selectedPeriodId = "";
    private string selectedAccountId = "";
    private string selectedSourceType = "";
    private string selectedReversalFilter = "";
    private string selectedCompanyId = "";
    private int VisibleColumnCount;
    // Pagination


    // Calculated totals
    private decimal TotalDebit => FilteredEntries.Sum(e => e.DebitAmount);
    private decimal TotalCredit => FilteredEntries.Sum(e => e.CreditAmount);
    private decimal Balance => TotalDebit - TotalCredit;

    private int TotalPages => FilteredEntries.Count == 0 ? 1 : (int)Math.Ceiling((double)FilteredEntries.Count / PageSize);

    private List<GeneralLedgerEntryModel> PagedEntries => FilteredEntries
        .Skip((CurrentPage - 1) * PageSize)
        .Take(PageSize)
        .ToList();

    // Aliases for razor binding compatibility
    private List<GeneralLedgerEntryModel> FilteredItems => FilteredEntries;
    private List<GeneralLedgerEntryModel> PagedItems => PagedEntries;
    private List<(Guid Id, string Code, string Name)> GLCompanies => Companies;
    private List<(Guid Id, string Code, string Name)> GLBranches => Branches;
    private List<(Guid Id, string Code, string Name)> GLPeriods => Periods;
    private bool IsBalanced => Math.Abs(Balance) < 0.01m;

    private string SelectedCompanyId
    {
        get => selectedCompanyId;
        set { selectedCompanyId = value; ApplyFilters(); }
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
        isInitialized = true;
    }

    private async Task LoadData()
    {
        // Load entries
        MasterDataService.ResetGeneralLedgerEntriesToSeed();
        Entries = MasterDataService.GetAllGeneralLedgerEntries();
        // Sort by most recent posting first (by PostingSequenceNumber descending)
        FilteredEntries = Entries
            .OrderByDescending(x => x.PostingSequenceNumber)
            .ToList();

        // Load filter dropdown data
        Branches = MasterDataService.GetGLBranches();
        Periods = MasterDataService.GetGLPeriods();
        Accounts = MasterDataService.GetGLAccounts();
        Companies = MasterDataService.GetGLCompanies();

        await Task.CompletedTask;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await JS.InvokeVoidAsync("feather.replace");
        await JS.InvokeVoidAsync("initTooltips");
        VisibleColumnCount =
    await JS.InvokeAsync<int>("getVisibleTableColumns");
    }

    // Filter property bindings with auto-apply
    private string SelectedBranchId
    {
        get => selectedBranchId;
        set { selectedBranchId = value; ApplyFilters(); }
    }

    private string SelectedPeriodId
    {
        get => selectedPeriodId;
        set { selectedPeriodId = value; ApplyFilters(); }
    }

    private string SelectedAccountId
    {
        get => selectedAccountId;
        set { selectedAccountId = value; ApplyFilters(); }
    }

    private string SelectedSourceType
    {
        get => selectedSourceType;
        set { selectedSourceType = value; ApplyFilters(); }
    }

    private string SelectedReversalFilter
    {
        get => selectedReversalFilter;
        set { selectedReversalFilter = value; ApplyFilters(); }
    }

    private async Task OnSearch(ChangeEventArgs e)
    {
        searchText = e.Value?.ToString() ?? "";
        ApplyFilters();
        VisibleColumnCount =
    await JS.InvokeAsync<int>("getVisibleTableColumns");
    }

    private void ApplyFilters()
    {
        IEnumerable<GeneralLedgerEntryModel> query = Entries;

        // Search filter
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            query = query.Where(e =>
                (e.Narration?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (e.LineNarration?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (e.SourceDocumentNumber?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (e.JournalEntryNumber?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (e.AccountCodeSnapshot?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (e.AccountNameSnapshot?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (e.ExternalReferenceNumber?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false)
            );
        }

        // Company filter
        if (!string.IsNullOrWhiteSpace(selectedCompanyId) && Guid.TryParse(selectedCompanyId, out var companyId))
        {
            query = query.Where(e => e.CompanyId == companyId);
        }

        // Branch filter
        if (!string.IsNullOrWhiteSpace(selectedBranchId) && Guid.TryParse(selectedBranchId, out var branchId))
        {
            query = query.Where(e => e.BranchId == branchId);
        }

        // Period filter
        if (!string.IsNullOrWhiteSpace(selectedPeriodId) && Guid.TryParse(selectedPeriodId, out var periodId))
        {
            query = query.Where(e => e.AccountingPeriodId == periodId);
        }

        // Account filter
        if (!string.IsNullOrWhiteSpace(selectedAccountId) && Guid.TryParse(selectedAccountId, out var accountId))
        {
            query = query.Where(e => e.AccountId == accountId);
        }

        // Source type filter
        if (!string.IsNullOrWhiteSpace(selectedSourceType))
        {
            query = query.Where(e => e.SourceType == selectedSourceType);
        }

        // Reversal filter
        if (!string.IsNullOrWhiteSpace(selectedReversalFilter))
        {
            query = selectedReversalFilter switch
            {
                "normal" => query.Where(e => !e.IsReversal),
                "reversal" => query.Where(e => e.IsReversal),
                _ => query
            };
        }

        // Sort by most recent posting first (by PostingSequenceNumber descending)
        FilteredEntries = query
            .OrderByDescending(x => x.PostingSequenceNumber)
            .ToList();
        CurrentPage = 1;
    }

    private void ClearFilters()
    {
        searchText = "";
        selectedBranchId = "";
        selectedPeriodId = "";
        selectedAccountId = "";
        selectedSourceType = "";
        selectedReversalFilter = "";
        selectedCompanyId = "";
        // Sort by most recent posting first (by PostingSequenceNumber descending)
        FilteredEntries = Entries
            .OrderByDescending(x => x.PostingSequenceNumber)
            .ToList();
        CurrentPage = 1;
    }

    private async Task OnPageSizeChange(ChangeEventArgs e)
    {
        isLoading = true;
        StateHasChanged();
        await Task.Delay(200);

        PageSize = int.Parse(e.Value?.ToString() ?? "10");
        CurrentPage = 1;

        isLoading = false;
        StateHasChanged();
    }

    private async Task GoToPage(int page)
    {
        if (page >= 1 && page <= TotalPages)
        {
            isLoading = true;
            StateHasChanged();
            await Task.Delay(150);

            CurrentPage = page;

            isLoading = false;
            StateHasChanged();
        }
    }

    int PageWindowSize = 2;
    int EndPage => Math.Min(StartPage + PageWindowSize - 1, TotalPages);
    int StartPage = 1;
    private int CurrentPage = 1;
    private int PageSize = 10;

    private async Task PreviousPage()
    {
        if (CurrentPage > 1) await GoToPage(CurrentPage - 1);
    }

    private async Task NextPage()
    {
        if (CurrentPage < TotalPages) await GoToPage(CurrentPage + 1);
    }


    private async Task OnRefreshAsync()
    {
        isLoading = true;
        StateHasChanged();
        await Task.Delay(150);

        ClearFilters();
        await LoadData();
        ToastService.ShowInfo("Ledger data refreshed", "Refresh");

        isLoading = false;
        StateHasChanged();
    }
    private GeneralLedgerEntryModel? SelectedCompany;

    void OpenRowDetails(GeneralLedgerEntryModel company)
    {
        SelectedCompany = company;
    }


    private async Task ViewItem(GeneralLedgerEntryModel entry)
    {
        isLoading = true;
        StateHasChanged();
        await Task.Delay(150);

        // Navigate to details page
        Nav.NavigateTo($"/general-ledger-entries/{entry.Id}/view");

        isLoading = false;
        StateHasChanged();
    }

    private string GetSourceTypeIcon(string sourceType)
    {
        return sourceType switch
        {
            GLSourceTypes.JournalEntry => "ti ti-book",
            GLSourceTypes.OpeningBalance => "ti ti-history",
            GLSourceTypes.VendorBill => "ti ti-file-invoice",
            GLSourceTypes.VendorPayment => "ti ti-cash",
            GLSourceTypes.CustomerInvoice => "ti ti-receipt",
            GLSourceTypes.CustomerReceipt => "ti ti-wallet",
            GLSourceTypes.BankTransaction => "ti ti-building-bank",
            GLSourceTypes.AssetTransaction => "ti ti-building-warehouse",
            GLSourceTypes.SystemAdjustment => "ti ti-settings",
            _ => "ti ti-info-circle"
        };
    }

    private string GetSourceTypeBadge(string sourceType)
    {
        return sourceType switch
        {
            GLSourceTypes.JournalEntry => "bg-primary-transparent text-primary",
            GLSourceTypes.OpeningBalance => "bg-info-transparent text-info",
            GLSourceTypes.VendorBill => "bg-warning-transparent text-warning",
            GLSourceTypes.VendorPayment => "bg-success-transparent text-success",
            GLSourceTypes.CustomerInvoice => "bg-purple-transparent text-purple",
            GLSourceTypes.CustomerReceipt => "bg-success-transparent text-success",
            GLSourceTypes.BankTransaction => "bg-cyan-transparent text-cyan",
            GLSourceTypes.AssetTransaction => "bg-secondary-transparent text-secondary",
            GLSourceTypes.SystemAdjustment => "bg-dark-transparent text-dark",
            _ => "bg-secondary-transparent text-secondary"
        };
    }
}
