using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Finance.ClosingBalance;

public partial class ClosingBalanceList
{
    private bool isInitialized = false;
    private bool isLoading = false;

    private List<ClosingBalanceModel> AllItems = new();
    private List<ClosingBalanceModel> FilteredItems = new();

    // Filter dropdowns
    private List<(Guid Id, string Code, string Name)> CBBranches = new();
    private List<(Guid Id, string Name)> CBPeriods = new();

    private string searchText = "";
    private string selectedBranchId = "";
    private string selectedPeriodId = "";
    private string selectedCloseStatus = "";
    private List<string> DistinctCloseStatuses = new();

    // Summary computations
    private decimal TotalClosingDebit => FilteredItems.Sum(x => x.ClosingDebit);
    private decimal TotalClosingCredit => FilteredItems.Sum(x => x.ClosingCredit);
    private int LockedCount => FilteredItems.Count(x => x.CloseStatus == "Locked");

    private int TotalPages => FilteredItems.Count == 0 ? 1 : (int)Math.Ceiling((double)FilteredItems.Count / PageSize);
    private List<ClosingBalanceModel> PagedItems => FilteredItems.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
    private int VisibleColumnCount;
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

    private string SelectedCloseStatus
    {
        get => selectedCloseStatus;
        set { selectedCloseStatus = value; ApplyFilters(); }
    }

    protected override async Task OnInitializedAsync()
    {
        // Load closing balances first
        MasterDataService.ResetClosingBalancesToSeed();
        AllItems = MasterDataService.GetAllClosingBalances();
        FilteredItems = AllItems.OrderByDescending(i => i.CreatedAt).ToList();

        // Load dropdown data derived from table data only
        var branchIdsInTable = AllItems.Where(i => i.BranchId != Guid.Empty).Select(i => i.BranchId).Distinct().ToHashSet();
        var allBranches = MasterDataService.GetCBBranches();
        CBBranches = allBranches.Where(b => branchIdsInTable.Contains(b.Id)).Select(b => (b.Id, b.Code, b.Name)).ToList();

        var periodIdsInTable = AllItems.Where(i => i.AccountingPeriodId != Guid.Empty).Select(i => i.AccountingPeriodId).Distinct().ToHashSet();
        var allPeriods = MasterDataService.GetCBPeriods();
        CBPeriods = allPeriods.Where(p => periodIdsInTable.Contains(p.Id)).ToList();

        DistinctCloseStatuses = AllItems.Where(i => !string.IsNullOrEmpty(i.CloseStatus)).Select(i => i.CloseStatus!).Distinct().ToList();

        isInitialized = true;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await JS.InvokeVoidAsync("feather.replace");
        await JS.InvokeVoidAsync("initTooltips");
        VisibleColumnCount =
await JS.InvokeAsync<int>("getVisibleTableColumns");
    }

    private string GetStatusBadge(string status) => status switch
    {
        "Calculated" => "bg-info-transparent text-info",
        "Verified" => "bg-primary-transparent text-primary",
        "Locked" => "bg-success-transparent text-success",
        "Reversed" => "bg-warning-transparent text-warning",
        _ => "bg-secondary-transparent text-secondary"
    };
    private string GetStatusDotBadge(string status) => status switch
    {
        "Calculated" => "bg-info text-info",
        "Verified" => "bg-primary text-primary",
        "Locked" => "bg-success text-success",
        "Reversed" => "bg-warning text-warning",
        _ => "bg-secondary text-secondary"
    };

    private string GetSideBadge(string side) => side switch
    {
        "Debit" => "bg-success-transparent text-success",
        "Credit" => "bg-info-transparent text-info",
        "Zero" => "bg-secondary-transparent text-secondary",
        _ => "bg-secondary-transparent text-secondary"
    };

    private async Task OnSearch(ChangeEventArgs e)
    {
        searchText = e.Value?.ToString() ?? "";
        ApplyFilters();
        VisibleColumnCount =
await JS.InvokeAsync<int>("getVisibleTableColumns");
    }

    private void ApplyFilters()
    {
        IEnumerable<ClosingBalanceModel> query = AllItems;

        if (!string.IsNullOrWhiteSpace(searchText))
            query = query.Where(i => (i.AccountCode?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                     (i.AccountName?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false));

        if (!string.IsNullOrWhiteSpace(selectedBranchId) && Guid.TryParse(selectedBranchId, out var branchId))
            query = query.Where(i => i.BranchId == branchId);

        if (!string.IsNullOrWhiteSpace(selectedPeriodId) && Guid.TryParse(selectedPeriodId, out var periodId))
            query = query.Where(i => i.AccountingPeriodId == periodId);

        if (!string.IsNullOrWhiteSpace(selectedCloseStatus))
            query = query.Where(i => i.CloseStatus == selectedCloseStatus);

        FilteredItems = query.OrderByDescending(i => i.CreatedAt).ToList();
        CurrentPage = 1;
    }

    private async Task OnPageSizeChange(ChangeEventArgs e)
    {
        isLoading = true;
        StateHasChanged();
        await Task.Delay(150);
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
        selectedBranchId = "";
        selectedPeriodId = "";
        selectedCloseStatus = "";
        MasterDataService.ResetClosingBalancesToSeed();
        AllItems = MasterDataService.GetAllClosingBalances();
        FilteredItems = AllItems.OrderByDescending(i => i.CreatedAt).ToList();

        // Recompute filter options from table data
        var branchIdsInTable = AllItems.Where(i => i.BranchId != Guid.Empty).Select(i => i.BranchId).Distinct().ToHashSet();
        var allBranches = MasterDataService.GetCBBranches();
        CBBranches = allBranches.Where(b => branchIdsInTable.Contains(b.Id)).Select(b => (b.Id, b.Code, b.Name)).ToList();

        var periodIdsInTable = AllItems.Where(i => i.AccountingPeriodId != Guid.Empty).Select(i => i.AccountingPeriodId).Distinct().ToHashSet();
        var allPeriods = MasterDataService.GetCBPeriods();
        CBPeriods = allPeriods.Where(p => periodIdsInTable.Contains(p.Id)).ToList();

        DistinctCloseStatuses = AllItems.Where(i => !string.IsNullOrEmpty(i.CloseStatus)).Select(i => i.CloseStatus!).Distinct().ToList();

        CurrentPage = 1;
        ToastService.ShowInfo("Data refreshed", "Refresh");
        isLoading = false;
        StateHasChanged();
    }

    private ClosingBalanceModel? SelectedCompany;
    void OpenRowDetails(ClosingBalanceModel company)
    {
        SelectedCompany = company;
    }
    private async Task ViewItem(ClosingBalanceModel item)
    {
        isLoading = true;
        StateHasChanged();
        await Task.Delay(150);

        // Navigate to details page
        Nav.NavigateTo($"/closing-balances/{item.Id}/view");

        isLoading = false;
        StateHasChanged();
    }
}
