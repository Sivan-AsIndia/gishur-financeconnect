using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;

namespace FinanceConnect.Client.Pages.COA;

public partial class AccountList : ComponentBase
{
    [Inject] private NavigationManager Nav { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Inject] private COADataService COADataService { get; set; } = default!;
    [Inject] private ToastService ToastService { get; set; } = default!;

    private List<AccountViewModel> Accounts { get; set; } = new();
    private List<AccountViewModel> FilteredAccounts { get; set; } = new();
    private List<AccountViewModel> PagedAccounts { get; set; } = new();
    private List<ChartOfAccountsViewModel> ChartOfAccountsList { get; set; } = new();
    private List<AccountGroupViewModel> AccountGroups { get; set; } = new();
    private int VisibleColumnCount;
    // Search and Filter
    private string searchText = string.Empty;

    private string _selectedChartId = string.Empty;
    private string SelectedChartId
    {
        get => _selectedChartId;
        set
        {
            if (_selectedChartId != value)
            {
                _selectedChartId = value;
                _selectedGroupId = string.Empty; // Reset group when chart changes
                CurrentPage = 1;
                ApplyFilters();
            }
        }
    }

    private string _selectedGroupId = string.Empty;
    private string SelectedGroupId
    {
        get => _selectedGroupId;
        set
        {
            if (_selectedGroupId != value)
            {
                _selectedGroupId = value;
                CurrentPage = 1;
                ApplyFilters();
            }
        }
    }

    private string _selectedNature = string.Empty;
    private string SelectedNature
    {
        get => _selectedNature;
        set
        {
            if (_selectedNature != value)
            {
                _selectedNature = value;
                CurrentPage = 1;
                ApplyFilters();
            }
        }
    }

    private string _selectedStatus = string.Empty;
    private string SelectedStatus
    {
        get => _selectedStatus;
        set
        {
            if (_selectedStatus != value)
            {
                _selectedStatus = value;
                CurrentPage = 1;
                ApplyFilters();
            }
        }
    }

    // Pagination
    private int _currentPage = 1;
    private int CurrentPage
    {
        get => _currentPage;
        set
        {
            if (_currentPage != value)
            {
                _currentPage = value;
                UpdatePagedAccounts();
            }
        }
    }

    private int _rowsPerPage = 10;
    private int RowsPerPage
    {
        get => _rowsPerPage;
        set
        {
            if (_rowsPerPage != value)
            {
                _rowsPerPage = value;
                CurrentPage = 1;
                ApplyFilters();
            }
        }
    }

    private int TotalPages => (int)Math.Ceiling((double)FilteredAccounts.Count / RowsPerPage);
    private int PageWindowSize => 2;
    private int StartPage => Math.Max(1, CurrentPage - PageWindowSize);
    private int EndPage => Math.Min(TotalPages, CurrentPage + PageWindowSize);

    // State
    private bool isLoading = false;
    private bool isInitialized = false;
    private AccountViewModel? SelectedAccount { get; set; }
    private bool CanDeleteAccount { get; set; } = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
        isInitialized = true;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await JS.InvokeVoidAsync("feather.replace");
        await JS.InvokeVoidAsync("initTooltips", true);
        VisibleColumnCount =
 await JS.InvokeAsync<int>("getVisibleTableColumns");

    }

    private async Task LoadDataAsync()
    {
        isLoading = true;
        StateHasChanged();
        try
        {
            Accounts = await COADataService.GetAccountsAsync();
            ChartOfAccountsList = await COADataService.GetChartOfAccountsAsync();
            AccountGroups = await COADataService.GetAccountGroupsAsync();
            ApplyFilters();
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
            await JS.InvokeVoidAsync("feather.replace");
        }
    }

    private async Task OnRefreshAsync()
    {
        isLoading = true;
        StateHasChanged();
        await Task.Delay(150);

        // Reset filters
        searchText = string.Empty;
        _selectedChartId = string.Empty;
        _selectedGroupId = string.Empty;
        _selectedNature = string.Empty;
        _selectedStatus = string.Empty;
        _currentPage = 1;

        // Reset to seed data
        COADataService.ResetToSeedData();

        await LoadDataAsync();
        ToastService.ShowInfo("Data reset to seed data", "Refresh");
    }

    private async Task OnSearch(ChangeEventArgs e)
    {
        searchText = e.Value?.ToString() ?? string.Empty;
        CurrentPage = 1;
        ApplyFilters();
        VisibleColumnCount =
 await JS.InvokeAsync<int>("getVisibleTableColumns");
    }

    private void ApplyFilters()
    {
        var filtered = Accounts.AsQueryable();

        // Search filter
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var term = searchText.ToLower();
            filtered = filtered.Where(a =>
                a.AccountCode.ToLower().Contains(term) ||
                a.AccountName.ToLower().Contains(term) ||
                (a.AccountAlias != null && a.AccountAlias.ToLower().Contains(term)) ||
                (a.Description != null && a.Description.ToLower().Contains(term)));
        }

        // Chart of Accounts filter
        if (!string.IsNullOrEmpty(_selectedChartId) && Guid.TryParse(_selectedChartId, out var chartId))
        {
            filtered = filtered.Where(a => a.ChartOfAccountsId == chartId);
        }

        // Account Group filter
        if (!string.IsNullOrEmpty(_selectedGroupId) && Guid.TryParse(_selectedGroupId, out var groupId))
        {
            filtered = filtered.Where(a => a.AccountGroupId == groupId);
        }

        // Account Nature filter
        if (!string.IsNullOrEmpty(_selectedNature))
        {
            filtered = filtered.Where(a => a.AccountNature == _selectedNature);
        }

        // Status filter
        if (!string.IsNullOrEmpty(_selectedStatus))
        {
            filtered = filtered.Where(a => a.Status == _selectedStatus);
        }

        FilteredAccounts = filtered.OrderByDescending(a => a.UpdatedAt ?? a.CreatedAt).ToList();
        UpdatePagedAccounts();
    }

    private void UpdatePagedAccounts()
    {
        if (CurrentPage > TotalPages && TotalPages > 0)
            _currentPage = TotalPages;
        if (CurrentPage < 1)
            _currentPage = 1;

        PagedAccounts = FilteredAccounts
            .Skip((CurrentPage - 1) * RowsPerPage)
            .Take(RowsPerPage)
            .ToList();
    }

    private List<AccountGroupViewModel> GetFilteredGroups()
    {
        if (string.IsNullOrEmpty(_selectedChartId) || !Guid.TryParse(_selectedChartId, out var chartId))
            return AccountGroups;
        return AccountGroups.Where(g => g.ChartOfAccountsId == chartId).ToList();
    }

    // Pagination methods
    private void GoToPage(int page)
    {
        if (page >= 1 && page <= TotalPages)
        {
            CurrentPage = page;
        }
    }

    private void PreviousPage()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
        }
    }

    private void NextPage()
    {
        if (CurrentPage < TotalPages)
        {
            CurrentPage++;
        }
    }

    // Row Details (mobile)
    private void OpenRowDetails(AccountViewModel account)
    {
        SelectedAccount = account;
    }

    // View navigation
    private async Task ViewAccount(AccountViewModel account)
    {
        await Task.CompletedTask;
        Nav.NavigateTo($"/gl-accounts/{account.Id}/view");
    }

    // Activate/Deactivate/Delete
    private void ConfirmActivate(AccountViewModel account)
    {
        SelectedAccount = account;
    }

    private void ConfirmDeactivate(AccountViewModel account)
    {
        SelectedAccount = account;
    }

    private async Task ConfirmDelete(AccountViewModel account)
    {
        SelectedAccount = account;
        // Check if account can be deleted (no transactions, not active)
        CanDeleteAccount = account.Status != AccountStatuses.Active && account.TransactionCount == 0;
        await Task.CompletedTask;
    }

    private async Task ActivateAccount()
    {
        if (SelectedAccount == null) return;

        try
        {
            SelectedAccount.Status = AccountStatuses.Active;
            SelectedAccount.IsActive = true;
            var result = await COADataService.UpdateAccountAsync(SelectedAccount);
            if (result)
            {
                ToastService.ShowSuccess($"Account '{SelectedAccount.AccountName}' activated");
                await LoadDataAsync();
            }
            else
            {
                ToastService.ShowError("Failed to activate account");
            }
        }
        catch (Exception ex)
        {
            ToastService.ShowError($"Error: {ex.Message}");
        }
    }

    private async Task DeactivateAccount()
    {
        if (SelectedAccount == null) return;

        try
        {
            SelectedAccount.Status = AccountStatuses.Inactive;
            SelectedAccount.IsActive = false;
            var result = await COADataService.UpdateAccountAsync(SelectedAccount);
            if (result)
            {
                ToastService.ShowWarning($"Account '{SelectedAccount.AccountName}' deactivated");
                await LoadDataAsync();
            }
            else
            {
                ToastService.ShowError("Failed to deactivate account");
            }
        }
        catch (Exception ex)
        {
            ToastService.ShowError($"Error: {ex.Message}");
        }
    }

    private async Task DeleteAccount()
    {
        if (SelectedAccount == null) return;

        try
        {
            var result = await COADataService.DeleteAccountAsync(SelectedAccount.Id);
            if (result)
            {
                ToastService.ShowError($"Account '{SelectedAccount.AccountName}' deleted");
                await LoadDataAsync();
            }
            else
            {
                ToastService.ShowError("Failed to delete account");
            }
        }
        catch (Exception ex)
        {
            ToastService.ShowError($"Error: {ex.Message}");
        }
    }

    // Badge classes
    private string GetNatureBadgeClass(string nature)
    {
        return nature switch
        {
            AccountNatures.Asset => "bg-success-transparent text-success",
            AccountNatures.Liability => "bg-danger-transparent text-danger",
            AccountNatures.Equity => "bg-purple-transparent text-purple",
            AccountNatures.Income => "bg-primary-transparent text-primary",
            AccountNatures.Expense => "bg-warning-transparent text-warning",
            _ => "bg-secondary-transparent text-secondary"
        };
    }

    private string GetStatusBadgeClass(string status)
    {
        return status switch
        {
            AccountStatuses.Active => "bg-success-transparent text-success",
            AccountStatuses.Inactive => "bg-danger-transparent text-secondary",
            AccountStatuses.Suspended => "bg-secondary-transparent text-warning",
            AccountStatuses.Closed => "bg-warning-transparent text-danger",
            _ => "bg-secondary-transparent text-secondary"
        };
    }
    private string GetStatusDotBadgeClass(string status)
    {
        return status switch
        {
            AccountStatuses.Active => "bg-success text-success",
            AccountStatuses.Inactive => "bg-danger text-secondary",
            AccountStatuses.Suspended => "bg-secondary text-warning",
            AccountStatuses.Closed => "bg-warning text-danger",
            _ => "bg-secondary text-secondary"
        };
    }
}
