using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Finance.OpeningBalance;

public partial class OpeningBalanceList
{
    private bool isInitialized = false;
    private bool isLoading = false;

    private List<OpeningBalanceModel> AllItems = new();
    private List<OpeningBalanceModel> FilteredItems = new();
    private OpeningBalanceModel? SelectedItem;

    private string searchText = "";
    private string selectedStatus = "";
    private List<string> DistinctStatuses = new();

    private int TotalPages => FilteredItems.Count == 0 ? 1 : (int)Math.Ceiling((double)FilteredItems.Count / PageSize);
    private List<OpeningBalanceModel> PagedItems => FilteredItems.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
    private int VisibleColumnCount;
    private string SelectedStatus
    {
        get => selectedStatus;
        set { selectedStatus = value; ApplyFilters(); }
    }

    protected override async Task OnInitializedAsync()
    {
        MasterDataService.ResetOpeningBalancesToSeed();
        AllItems = MasterDataService.GetAllOpeningBalances();
        FilteredItems = AllItems.OrderByDescending(i => i.UpdatedAt ?? i.CreatedAt).ToList();
        DistinctStatuses = AllItems.Where(i => !string.IsNullOrEmpty(i.Status)).Select(i => i.Status!).Distinct().ToList();
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
        "Posted" => "bg-success-transparent text-success",
        "Approved" => "bg-info-transparent text-info",
        "Submitted" => "bg-warning-transparent text-warning",
        "Draft" => "bg-secondary-transparent text-secondary",
        "Cancelled" => "bg-danger-transparent text-danger",
        _ => "bg-secondary-transparent text-secondary"
    };
    private string GetStatusDotBadge(string status) => status switch
    {
        "Posted" => "bg-success text-success",
        "Approved" => "bg-info text-info",
        "Submitted" => "bg-warning text-warning",
        "Draft" => "bg-secondary text-secondary",
        "Cancelled" => "bg-danger text-danger",
        _ => "bg-secondary text-secondary"
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
        IEnumerable<OpeningBalanceModel> query = AllItems;
        if (!string.IsNullOrWhiteSpace(searchText))
            query = query.Where(i => (i.OpeningBalanceNumber?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                     (i.CompanyName?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false));
        if (!string.IsNullOrWhiteSpace(selectedStatus))
            query = query.Where(i => i.Status == selectedStatus);
        FilteredItems = query.OrderByDescending(i => i.UpdatedAt ?? i.CreatedAt).ToList();
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
        selectedStatus = "";
        MasterDataService.ResetOpeningBalancesToSeed();
        AllItems = MasterDataService.GetAllOpeningBalances();
        FilteredItems = AllItems.OrderByDescending(i => i.UpdatedAt ?? i.CreatedAt).ToList();
        DistinctStatuses = AllItems.Where(i => !string.IsNullOrEmpty(i.Status)).Select(i => i.Status!).Distinct().ToList();
        CurrentPage = 1;
        ToastService.ShowInfo("Data refreshed", "Refresh");
        isLoading = false;
        StateHasChanged();
    }

    private async Task ViewItem(OpeningBalanceModel item)
    {
        isLoading = true;
        StateHasChanged();
        await Task.Delay(150);

        // Navigate to details page
        Nav.NavigateTo($"/opening-balances/{item.Id}/view");

        isLoading = false;
        StateHasChanged();
    }

    private void ConfirmSubmit(OpeningBalanceModel item) => SelectedItem = item;

    private void SubmitConfirmed()
    {
        if (SelectedItem != null && SelectedItem.IsBalanced)
        {
            MasterDataService.SubmitOpeningBalance(SelectedItem.Id);
            AllItems = MasterDataService.GetAllOpeningBalances();
            ApplyFilters();
            ToastService.ShowSuccess("Submitted for approval", "Submitted");
        }
    }

    private void ConfirmApprove(OpeningBalanceModel item) => SelectedItem = item;

    private void ApproveConfirmed()
    {
        if (SelectedItem != null)
        {
            MasterDataService.ApproveOpeningBalance(SelectedItem.Id);
            AllItems = MasterDataService.GetAllOpeningBalances();
            ApplyFilters();
            ToastService.ShowSuccess("Opening Balance approved", "Approved");
        }
    }

    private void ConfirmPost(OpeningBalanceModel item) => SelectedItem = item;

    private void PostConfirmed()
    {
        if (SelectedItem != null)
        {
            MasterDataService.PostOpeningBalance(SelectedItem.Id);
            AllItems = MasterDataService.GetAllOpeningBalances();
            ApplyFilters();
            ToastService.ShowSuccess("Posted to General Ledger", "Posted");
        }
    }

    private OpeningBalanceModel? SelectedCompany;

    void OpenRowDetails(OpeningBalanceModel company)
    {
        SelectedCompany = company;
    }
    private void ConfirmDelete(OpeningBalanceModel item) => SelectedItem = item;

    private void DeleteConfirmed()
    {
        if (SelectedItem != null)
        {
            SelectedItem.IsDeleted = true;
            AllItems = MasterDataService.GetAllOpeningBalances();
            FilteredItems = AllItems.OrderByDescending(i => i.UpdatedAt ?? i.CreatedAt).ToList();
            ToastService.ShowSuccess("Opening Balance deleted", "Deleted");
            CurrentPage = 1;
        }
    }
}
