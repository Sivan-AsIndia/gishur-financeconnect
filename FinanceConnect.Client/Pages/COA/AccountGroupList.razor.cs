using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.COA
{
    public partial class AccountGroupList : ComponentBase
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject] COADataService COADataService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;

        bool isInitialized = false;
        bool isLoading = false;
        bool canDelete = false;

        List<AccountGroupViewModel> AllItems = new();
        List<AccountGroupViewModel> FilteredItems = new();
        List<AccountGroupViewModel> PagedItems = new();
        List<ChartOfAccountsViewModel> ChartOfAccountsList = new();
        private int VisibleColumnCount;
        AccountGroupViewModel? SelectedItem = null;

        // Filter values
        string searchTerm = "";
        string selectedChartId = "";
        string selectedNature = "";
        string selectedStatus = "";

        // Pagination
        int currentPage = 1;
        int pageSize = 10;
        int totalCount = 0;
        int totalPages = 1;

        protected override async Task OnInitializedAsync()
        {
            ChartOfAccountsList = await COADataService.GetChartOfAccountsAsync();
            await LoadData();

            isInitialized = true;
            await Task.Delay(50);
            await JS.InvokeVoidAsync("feather.replace");
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
            VisibleColumnCount =
await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        async Task LoadData()
        {
            AllItems = await COADataService.GetAccountGroupsAsync();
            ApplyFilters();
            await Task.CompletedTask;
        }

        void ApplyFilters()
        {
            FilteredItems = AllItems;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();
                FilteredItems = FilteredItems.Where(x =>
                    (x.GroupCode?.ToLower().Contains(term) ?? false) ||
                    (x.GroupName?.ToLower().Contains(term) ?? false)
                ).ToList();
            }

            if (!string.IsNullOrEmpty(selectedChartId) && Guid.TryParse(selectedChartId, out var chartGuid))
            {
                FilteredItems = FilteredItems.Where(x => x.ChartOfAccountsId == chartGuid).ToList();
            }

            if (!string.IsNullOrEmpty(selectedNature))
            {
                FilteredItems = FilteredItems.Where(x => x.AccountNature == selectedNature).ToList();
            }

            if (!string.IsNullOrEmpty(selectedStatus))
            {
                FilteredItems = FilteredItems.Where(x => x.Status == selectedStatus).ToList();
            }

            FilteredItems = FilteredItems.OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt).ToList();

            totalCount = FilteredItems.Count;
            totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            if (totalPages < 1) totalPages = 1;
            if (currentPage > totalPages) currentPage = totalPages;

            PagedItems = FilteredItems.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
        }

        async Task OnRefreshAsync()
        {
            isLoading = true;
            StateHasChanged();
            await Task.Delay(150);

            // Reset filters
            searchTerm = "";
            selectedChartId = "";
            selectedNature = "";
            selectedStatus = "";
            currentPage = 1;

            // Reset to seed data
            COADataService.ResetToSeedData();

            await LoadData();
            ToastService.ShowInfo("Data reset to seed data", "Refresh");

            isLoading = false;
            StateHasChanged();
            await JS.InvokeVoidAsync("feather.replace");
        }

        async Task OnSearchChanged(ChangeEventArgs e)
        {
            searchTerm = e.Value?.ToString() ?? "";
            currentPage = 1;
            ApplyFilters();
            VisibleColumnCount =
await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        void OnChartFilterChanged(ChangeEventArgs e)
        {
            selectedChartId = e.Value?.ToString() ?? "";
            currentPage = 1;
            ApplyFilters();
        }

        void OnNatureFilterChanged(ChangeEventArgs e)
        {
            selectedNature = e.Value?.ToString() ?? "";
            currentPage = 1;
            ApplyFilters();
        }

        void OnStatusFilterChanged(ChangeEventArgs e)
        {
            selectedStatus = e.Value?.ToString() ?? "";
            currentPage = 1;
            ApplyFilters();
        }

        void OnPageSizeChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out var size))
            {
                pageSize = size;
                currentPage = 1;
                ApplyFilters();
            }
        }

        void GoToPage(int page)
        {
            currentPage = page;
            ApplyFilters();
        }

        void PreviousPage()
        {
            if (currentPage > 1)
            {
                currentPage--;
                ApplyFilters();
            }
        }

        void NextPage()
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                ApplyFilters();
            }
        }

        List<int> GetPageNumbers()
        {
            var pages = new List<int>();
            int start = Math.Max(1, currentPage - 2);
            int end = Math.Min(totalPages, currentPage + 2);

            for (int i = start; i <= end; i++)
            {
                pages.Add(i);
            }
            return pages;
        }

        void OpenRowDetails(AccountGroupViewModel item)
        {
            SelectedItem = item;
        }

        async Task ViewGroup(AccountGroupViewModel item)
        {
            Nav.NavigateTo($"/account-groups/{item.Id}/view");
            await Task.CompletedTask;
        }

        void ConfirmActivate(AccountGroupViewModel item)
        {
            SelectedItem = item;
        }

        void ConfirmDeactivate(AccountGroupViewModel item)
        {
            SelectedItem = item;
        }

        async Task ConfirmDelete(AccountGroupViewModel item)
        {
            SelectedItem = item;
            canDelete = await COADataService.CanDeleteAccountGroup(item.Id);
        }

        async Task ActivateConfirmed()
        {
            if (SelectedItem == null) return;

            SelectedItem.Status = GroupStatuses.Active;
            await COADataService.UpdateAccountGroupAsync(SelectedItem);
            ToastService.ShowSuccess($"Group '{SelectedItem.GroupName}' activated", "Activated");
            await LoadData();
            await JS.InvokeVoidAsync("feather.replace");
        }

        async Task DeactivateConfirmed()
        {
            if (SelectedItem == null) return;

            SelectedItem.Status = GroupStatuses.Inactive;
            await COADataService.UpdateAccountGroupAsync(SelectedItem);
            ToastService.ShowWarning($"Group '{SelectedItem.GroupName}' deactivated", "Deactivated");
            await LoadData();
            await JS.InvokeVoidAsync("feather.replace");
        }

        async Task DeleteConfirmed()
        {
            if (SelectedItem == null || !canDelete) return;

            await COADataService.DeleteAccountGroupAsync(SelectedItem.Id);
            ToastService.ShowError($"Group '{SelectedItem.GroupName}' deleted", "Deleted");
            await LoadData();
            await JS.InvokeVoidAsync("feather.replace");
        }

        string GetNatureBadgeClass(string? nature) => nature switch
        {
            "Asset" => "bg-success-transparent text-success",
            "Liability" => "bg-danger-transparent text-danger",
            "Equity" => "bg-purple-transparent text-purple",
            "Income" => "bg-info-transparent text-info",
            "Expense" => "bg-warning-transparent text-warning",
            _ => "bg-secondary-transparent text-secondary"
        };

        string GetStatusBadgeClass(string? status) => status switch
        {
            "Active" => "bg-success-transparent text-success",
            "Inactive" => "bg-secondary-transparent text-secondary",
            "Draft" => "bg-info-transparent text-info",
            _ => "bg-secondary-transparent text-secondary"
        };
        string GetStatusDotBadgeClass(string? status) => status switch
        {
            "Active" => "bg-success text-success",
            "Inactive" => "bg-danger text-secondary",
            "Draft" => "bg-warning text-info",
            _ => "bg-secondary text-secondary"
        };
    }
}
