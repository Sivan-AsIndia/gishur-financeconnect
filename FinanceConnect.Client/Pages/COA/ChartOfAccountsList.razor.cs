using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.COA;

public partial class ChartOfAccountsList
{
    private bool isInitialized = false;
    private bool isLoading = false;

    // Data collections
    private List<ChartOfAccountsViewModel> Charts = new();
    private List<ChartOfAccountsViewModel> FilteredCharts = new();
    private List<CompanyModel> Companies = new();

    // Selected model
    private ChartOfAccountsViewModel? SelectedChart;

    // Permission flags
    private bool canDelete = true;

    // Filter values
    private string searchText = "";
    private string selectedCompanyId = "";
    private string selectedType = "";
    private string selectedStatus = "";

    private int VisibleColumnCount;
    // Pagination
    private int TotalPages => FilteredCharts.Count == 0 ? 1 : (int)Math.Ceiling((double)FilteredCharts.Count / PageSize);

    private List<ChartOfAccountsViewModel> PagedCharts => FilteredCharts
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
        if (isInitialized)
        {
            await LoadData();
            StateHasChanged();
        }
    }

    private async Task LoadData()
    {
        Charts = await COADataService.GetChartOfAccountsAsync();
        // Sort by most recent first (UpdatedAt if exists, otherwise CreatedAt)
        FilteredCharts = Charts
            .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
            .ToList();
        Companies = await COADataService.GetCompaniesAsync();
    }

    void OpenRowDetails(ChartOfAccountsViewModel chart)
    {
        SelectedChart = chart;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await JS.InvokeVoidAsync("feather.replace");
        await JS.InvokeVoidAsync("initTooltips", true);
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
        IEnumerable<ChartOfAccountsViewModel> query = Charts;

        // Search filter
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            query = query.Where(c =>
                c.ChartCode.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                c.ChartName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                (c.Description?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (c.CompanyCode?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (c.CompanyName?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false)
            );
        }

        // Company filter
        if (!string.IsNullOrWhiteSpace(selectedCompanyId) && Guid.TryParse(selectedCompanyId, out var companyId))
        {
            query = query.Where(c => c.CompanyId == companyId);
        }

        // Type filter
        if (!string.IsNullOrWhiteSpace(selectedType))
        {
            query = query.Where(c => c.ChartType == selectedType);
        }

        // Status filter
        if (!string.IsNullOrWhiteSpace(selectedStatus))
        {
            query = query.Where(c => c.Status == selectedStatus);
        }

        // Sort by most recent first (UpdatedAt if exists, otherwise CreatedAt)
        FilteredCharts = query
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

        // Reset to seed data
        COADataService.ResetToSeedData();

        await LoadData();
        ToastService.ShowInfo("Data reset to seed data", "Refresh");

        isLoading = false;
        StateHasChanged();
    }

    private async Task ViewChart(ChartOfAccountsViewModel chart)
    {
        isLoading = true;
        StateHasChanged();
        await Task.Delay(200);

        SelectedChart = chart;

        isLoading = false;
        StateHasChanged();

        Nav.NavigateTo($"/chart-of-accounts/{chart.Id}/view");
    }

    private void ConfirmActivate(ChartOfAccountsViewModel chart) => SelectedChart = chart;

    private async Task ActivateConfirmed()
    {
        if (SelectedChart != null)
        {
            await COADataService.UpdateChartStatusAsync(SelectedChart.Id, COAStatuses.Active);
            Charts = await COADataService.GetChartOfAccountsAsync();
            ApplyFilters();
            ToastService.ShowSuccess($"Chart '{SelectedChart.ChartName}' activated successfully", "Activated");
            SelectedChart = null;
        }
    }

    private void ConfirmLock(ChartOfAccountsViewModel chart) => SelectedChart = chart;

    private async Task LockConfirmed()
    {
        if (SelectedChart != null)
        {
            await COADataService.UpdateChartStatusAsync(SelectedChart.Id, COAStatuses.Locked);
            Charts = await COADataService.GetChartOfAccountsAsync();
            ApplyFilters();
            ToastService.ShowWarning($"Chart '{SelectedChart.ChartName}' locked successfully", "Locked");
            SelectedChart = null;
        }
    }

    private void ConfirmUnlock(ChartOfAccountsViewModel chart) => SelectedChart = chart;

    private async Task UnlockConfirmed()
    {
        if (SelectedChart != null)
        {
            await COADataService.UpdateChartStatusAsync(SelectedChart.Id, COAStatuses.Active);
            Charts = await COADataService.GetChartOfAccountsAsync();
            ApplyFilters();
            ToastService.ShowSuccess($"Chart '{SelectedChart.ChartName}' unlocked successfully", "Unlocked");
            SelectedChart = null;
        }
    }

    private async Task ConfirmDelete(ChartOfAccountsViewModel chart)
    {
        SelectedChart = chart;
        canDelete = await COADataService.CanDeleteChartOfAccounts(chart.Id);
    }

    private async Task DeleteConfirmed()
    {
        if (SelectedChart != null && canDelete)
        {
            await COADataService.DeleteChartOfAccountsAsync(SelectedChart.Id);
            Charts = await COADataService.GetChartOfAccountsAsync();
            // Sort by most recent first (UpdatedAt if exists, otherwise CreatedAt)
            FilteredCharts = Charts
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToList();
            ApplyFilters();
            ToastService.ShowError($"Chart '{SelectedChart.ChartName}' deleted successfully", "Deleted");
            SelectedChart = null;
            CurrentPage = 1;
        }
    }

    private string GetTypeBadgeClass(string type)
    {
        return type switch
        {
            ChartTypes.Standard => "bg-primary-transparent text-primary",
            ChartTypes.Template => "bg-info-transparent text-info",
            ChartTypes.Migration => "bg-warning-transparent text-warning",
            _ => "bg-secondary-transparent text-secondary"
        };
    }

    private string GetStatusBadgeClass(string status)
    {
        return status switch
        {
            COAStatuses.Draft => "bg-warning-transparent text-warning",
            COAStatuses.Active => "bg-success-transparent text-success",
            COAStatuses.Locked => "bg-info-transparent text-info",
            COAStatuses.Retired => "bg-danger-transparent text-danger",
            _ => "bg-secondary-transparent text-secondary"
        };
    }
    private string GetStatusDotBadgeClass(string status)
    {
        return status switch
        {
            COAStatuses.Draft => "bg-warning text-warning",
            COAStatuses.Active => "bg-success text-success",
            COAStatuses.Locked => "bg-info text-info",
            COAStatuses.Retired => "bg-danger text-danger",
            _ => "bg-secondary text-secondary"
        };
    }
}
