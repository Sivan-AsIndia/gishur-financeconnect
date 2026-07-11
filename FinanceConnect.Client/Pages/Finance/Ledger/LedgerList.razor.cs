using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Finance.Ledger;

public partial class LedgerList
{
    [Inject] private FinanceDataService FinanceDataService { get; set; } = default!;

    private bool isInitialized = false;
    private bool isLoading = false;

    // Data collections
    private List<LedgerModel> Ledgers = new();
    private List<LedgerModel> FilteredLedgers = new();
    private List<CurrencyModel> Currencies = new();
    private List<(Guid? Id, string Code, string Name)> Companies = new();

    // Distinct filter options derived from table data
    private List<string> DistinctTypes = new();
    private List<string> DistinctStatuses = new();

    // Selected and edit models
    private LedgerModel? SelectedLedger;

    // Permission flags
    private bool canDeactivate = true;
    private bool canDelete = true;

    // Filter values
    private string searchText = "";
    private string selectedCompanyId = "";
    private string selectedType = "";
    private string selectedStatus = "";
    private int VisibleColumnCount;
    // Pagination


    private int TotalPages => FilteredLedgers.Count == 0 ? 1 : (int)Math.Ceiling((double)FilteredLedgers.Count / PageSize);

    private List<LedgerModel> PagedLedgers => FilteredLedgers
        .Skip((CurrentPage - 1) * PageSize)
        .Take(PageSize)
        .ToList();

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
        isInitialized = true;
    }

    protected override async void OnParametersSet()
    {
        // Reload data every time we navigate to this page
        // This ensures the list is updated after Add/Edit/Delete operations
        if (isInitialized)
        {
            await LoadData();
            StateHasChanged();
        }
    }

    private async Task LoadData()
    {
        // Load current ledgers without resetting to seed data
        // (Reset only happens when user clicks Refresh button)
        Ledgers = FinanceDataService.GetAllLedgers();
        // Sort by most recent first (UpdatedAt if exists, otherwise CreatedAt)
        FilteredLedgers = Ledgers
            .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
            .ToList();

        // Load currencies for dropdown
        Currencies = MasterDataService.GetAllCurrencies().Where(c => c.IsActive).ToList();

        // Load companies only from data present in the table
        Companies = Ledgers
            .Where(l => l.CompanyId.HasValue && !string.IsNullOrEmpty(l.CompanyCode))
            .Select(l => (l.CompanyId, l.CompanyCode!, l.CompanyName ?? ""))
            .DistinctBy(c => c.CompanyId)
            .ToList();

        // Compute distinct types and statuses from table data
        DistinctTypes = Ledgers
            .Where(l => !string.IsNullOrEmpty(l.LedgerType))
            .Select(l => l.LedgerType!)
            .Distinct()
            .ToList();

        DistinctStatuses = Ledgers
            .Where(l => !string.IsNullOrEmpty(l.Status))
            .Select(l => l.Status!)
            .Distinct()
            .ToList();

        await Task.CompletedTask;
    }

    private LedgerModel? SelectedCompany;
    void OpenRowDetails(LedgerModel company)
    {
        SelectedCompany = company;
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await JS.InvokeVoidAsync("feather.replace");
        await JS.InvokeVoidAsync("initTooltips");
        VisibleColumnCount =
    await JS.InvokeAsync<int>("getVisibleTableColumns");
    }

    // Filter property bindings
    private string SelectedCompanyId
    {
        get => selectedCompanyId;
        set { selectedCompanyId = value; ApplyFilters(); }
    }

    private string SelectedType
    {
        get => selectedType;
        set { selectedType = value; ApplyFilters(); }
    }

    private string SelectedStatus
    {
        get => selectedStatus;
        set { selectedStatus = value; ApplyFilters(); }
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
        IEnumerable<LedgerModel> query = Ledgers;

        // Search filter
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            query = query.Where(l =>
                l.LedgerCode.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                l.LedgerName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                (l.Description?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (l.CompanyCode?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (l.CompanyName?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false)
            );
        }

        // Company filter
        if (!string.IsNullOrWhiteSpace(selectedCompanyId) && Guid.TryParse(selectedCompanyId, out var companyId))
        {
            query = query.Where(l => l.CompanyId == companyId);
        }

        // Type filter
        if (!string.IsNullOrWhiteSpace(selectedType))
        {
            query = query.Where(l => l.LedgerType == selectedType);
        }

        // Status filter
        if (!string.IsNullOrWhiteSpace(selectedStatus))
        {
            query = query.Where(l => l.Status == selectedStatus);
        }

        // Sort by most recent first (UpdatedAt if exists, otherwise CreatedAt)
        FilteredLedgers = query
            .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
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

        searchText = "";
        selectedCompanyId = "";
        selectedType = "";
        selectedStatus = "";

        // Reset to seed data only happens here when user clicks Refresh button
        FinanceDataService.ResetLedgersToSeed();
        await LoadData();
        ToastService.ShowInfo("Ledger data refreshed", "Refresh");

        isLoading = false;
        StateHasChanged();
    }

    private async Task ViewLedger(LedgerModel ledger)
    {
        isLoading = true;
        StateHasChanged();
        await Task.Delay(200);

        SelectedLedger = ledger;

        isLoading = false;
        StateHasChanged();

        // Navigate to the Ledger Details page
        Nav.NavigateTo($"/ledgers/{ledger.Id}/view");
    }

    private void ConfirmActivate(LedgerModel ledger) => SelectedLedger = ledger;

    private void ActivateConfirmed()
    {
        if (SelectedLedger != null)
        {
            FinanceDataService.ActivateLedger(SelectedLedger.Id);
            Ledgers = FinanceDataService.GetAllLedgers();
            ApplyFilters();
            ToastService.ShowSuccess($"Ledger '{SelectedLedger.LedgerName}' activated successfully", "Activated");
            SelectedLedger = null;
        }
    }

    private void ConfirmDeactivate(LedgerModel ledger)
    {
        SelectedLedger = ledger;
        canDeactivate = FinanceDataService.CanDeactivateLedger(ledger.Id);
    }

    private void DeactivateConfirmed()
    {
        if (SelectedLedger != null && canDeactivate)
        {
            FinanceDataService.DeactivateLedger(SelectedLedger.Id, "Deactivated by user");
            Ledgers = FinanceDataService.GetAllLedgers();
            ApplyFilters();
            ToastService.ShowWarning($"Ledger '{SelectedLedger.LedgerName}' deactivated successfully", "Deactivated");
            SelectedLedger = null;
        }
    }

    private void ConfirmDelete(LedgerModel ledger)
    {
        SelectedLedger = ledger;
        canDelete = FinanceDataService.CanDeleteLedger(ledger.Id);
    }

    private void DeleteConfirmed()
    {
        if (SelectedLedger != null && canDelete)
        {
            FinanceDataService.DeleteLedger(SelectedLedger.Id);
            Ledgers = FinanceDataService.GetAllLedgers();
            // Sort by most recent first (UpdatedAt if exists, otherwise CreatedAt)
            FilteredLedgers = Ledgers
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToList();
            ApplyFilters();
            ToastService.ShowError($"Ledger '{SelectedLedger.LedgerName}' deleted successfully", "Deleted");
            SelectedLedger = null;
            CurrentPage = 1;
        }
    }

    private string GetLedgerTypeBadge(string type)
    {
        return type switch
        {
            LedgerTypes.Primary => "bg-primary-transparent text-primary",
            LedgerTypes.Management => "bg-info-transparent text-info",
            LedgerTypes.IFRS => "bg-purple-transparent text-purple",
            LedgerTypes.Tax => "bg-warning-transparent text-warning",
            LedgerTypes.Other => "bg-secondary-transparent text-secondary",
            _ => "bg-light text-dark"
        };
    }

    private string GetStatusBadge(string status)
    {
        return status switch
        {
            LedgerStatus.Draft => "bg-warning-transparent text-warning",
            LedgerStatus.Active => "bg-success-transparent text-success",
            LedgerStatus.Inactive => "bg-danger-transparent text-danger",
            _ => "bg-secondary-transparent text-secondary"
        };
    }
    private string GetStatusDotBadge(string status)
    {
        return status switch
        {
            LedgerStatus.Draft => "bg-warning text-warning",
            LedgerStatus.Active => "bg-success text-success",
            LedgerStatus.Inactive => "bg-danger text-danger",
            _ => "bg-secondary text-secondary"
        };
    }

    private string GetLockStatusBadge(string lockStatus)
    {
        return lockStatus switch
        {
            LockStatuses.Unlocked => "bg-success-transparent text-success",
            LockStatuses.LockedAfterPosting => "bg-warning-transparent text-warning",
            LockStatuses.LockedByController => "bg-danger-transparent text-danger",
            _ => "bg-light text-dark"
        };
    }
}
