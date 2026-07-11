using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.AR.CustomerAccount
{
    public partial class CustomerAccountList
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject] CustomerAccountService CustomerAccountService { get; set; } = default!;
        [Inject] CustomerService CustomerService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] AuthService AuthService { get; set; } = default!;

        private bool isInitialized = false;
        private bool isLoading = false;
        private bool isRefreshing = false;

        private List<CustomerAccountViewModel> AllAccounts = new();
        private CustomerAccountViewModel? SelectedAccount = null;
        private AccountSummaryStatsViewModel SummaryStats = new();

        // Filters
        private string searchText = string.Empty;
        private string _selectedStatus = string.Empty;
        private string _selectedBlockedFilter = string.Empty;
        private string _selectedCreditFilter = string.Empty;

        // Freeze modal inputs
        private string freezeTypeInput = string.Empty;
        private string freezeReasonInput = string.Empty;
        private int VisibleColumnCount;
        // Pagination
        private int CurrentPage = 1;
        private int PageSize = 10;

        private string SelectedStatus
        {
            get => _selectedStatus;
            set { _selectedStatus = value; CurrentPage = 1; }
        }

        private string SelectedBlockedFilter
        {
            get => _selectedBlockedFilter;
            set { _selectedBlockedFilter = value; CurrentPage = 1; }
        }

        private string SelectedCreditFilter
        {
            get => _selectedCreditFilter;
            set { _selectedCreditFilter = value; CurrentPage = 1; }
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
            isInitialized = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
            VisibleColumnCount =
   await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        private async Task LoadDataAsync()
        {
            isLoading = true;
            StateHasChanged();

            await Task.Delay(100); // Simulate async load

            AllAccounts = CustomerAccountService.GetAll();
            SummaryStats = CustomerAccountService.GetSummaryStats();

            isLoading = false;
            StateHasChanged();
        }

        private async Task OnRefreshAsync()
        {
            isRefreshing = true;
            StateHasChanged();

            await Task.Delay(500); // Animation delay

            // Reset to seed data
            CustomerAccountService.ResetToSeed();
            
            AllAccounts = CustomerAccountService.GetAll();
            SummaryStats = CustomerAccountService.GetSummaryStats();

            isRefreshing = false;
            StateHasChanged();
            SelectedCreditFilter = null;
            SelectedStatus = null;
            searchText = "";
            SelectedBlockedFilter = null;
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
            ToastService.ShowInfo("Customer accounts data refreshed", "Refreshed");
        }

        private async Task OnSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString() ?? string.Empty;
            CurrentPage = 1;
            VisibleColumnCount =
   await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        private void OnPageSizeChange(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int size))
            {
                PageSize = size;
                CurrentPage = 1;
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

        #region Filtering & Pagination

        private List<CustomerAccountViewModel> FilteredAccounts
        {
            get
            {
                var result = AllAccounts.AsEnumerable();

                // Search filter
                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    var search = searchText.ToLower();
                    result = result.Where(a =>
                        (a.CustomerCode?.ToLower().Contains(search) ?? false) ||
                        (a.CustomerName?.ToLower().Contains(search) ?? false) ||
                        (a.CurrencyCode?.ToLower().Contains(search) ?? false));
                }

                // Status filter
                if (!string.IsNullOrEmpty(SelectedStatus))
                {
                    result = result.Where(a => a.AccountStatus == SelectedStatus);
                }

                // Blocked filter
                if (!string.IsNullOrEmpty(SelectedBlockedFilter))
                {
                    result = SelectedBlockedFilter == "blocked"
                        ? result.Where(a => a.IsPostingBlocked)
                        : result.Where(a => !a.IsPostingBlocked);
                }

                // Credit filter
                if (!string.IsNullOrEmpty(SelectedCreditFilter))
                {
                    result = SelectedCreditFilter == "overcredit"
                        ? result.Where(a => a.OverCreditAmount > 0)
                        : result.Where(a => a.OverCreditAmount <= 0);
                }

                // Sort by UpdatedAt (if available) or CreatedAt descending - latest records first
                return result.OrderByDescending(a => a.UpdatedAt ?? a.CreatedAt).ToList();
            }
        }

        private List<CustomerAccountViewModel> PagedAccounts =>
            FilteredAccounts.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

        private int TotalPages => Math.Max(1, (int)Math.Ceiling((double)FilteredAccounts.Count / PageSize));

        private void GoToPage(int page)
        {
            if (page >= 1 && page <= TotalPages)
            {
                CurrentPage = page;
            }
        }

        #endregion

        #region Modal Handlers

        private void OpenRowDetails(CustomerAccountViewModel account)
        {
            SelectedAccount = account;
        }

        private void OpenFreezeModal(CustomerAccountViewModel account)
        {
            SelectedAccount = account;
            freezeTypeInput = string.Empty;
            freezeReasonInput = string.Empty;
        }

        private void ConfirmUnfreeze(CustomerAccountViewModel account)
        {
            SelectedAccount = account;
        }

        #endregion

        #region Actions

        private async Task FreezeConfirmed()
        {
            if (SelectedAccount == null)
                return;

            if (string.IsNullOrWhiteSpace(freezeTypeInput))
            {
                ToastService.ShowError("Please select a freeze type.", "Validation Error");
                return;
            }

            if (string.IsNullOrWhiteSpace(freezeReasonInput))
            {
                ToastService.ShowError("Please enter a reason for freezing the account.", "Validation Error");
                return;
            }

            var userId = Guid.NewGuid(); // In real app, get from AuthService
            var userName = AuthService.CurrentUser?.UserName ?? "System";

            var result = CustomerAccountService.Freeze(SelectedAccount.Id, freezeReasonInput, freezeTypeInput, userId, userName);

            if (result.Success)
            {
                // Warning toast for freeze action
                ToastService.ShowWarning($"Account for '{SelectedAccount.CustomerName}' has been frozen.", "Account Frozen");
                AllAccounts = CustomerAccountService.GetAll();
                SummaryStats = CustomerAccountService.GetSummaryStats();
                StateHasChanged();
                await JS.InvokeVoidAsync("feather.replace");
            }
            else
            {
                ToastService.ShowError(result.Message, "Freeze Failed");
            }
        }

        private async Task UnfreezeConfirmed()
        {
            if (SelectedAccount == null)
                return;

            var userName = AuthService.CurrentUser?.UserName ?? "System";

            var result = CustomerAccountService.Unfreeze(SelectedAccount.Id, userName);

            if (result.Success)
            {
                // Success toast for unfreeze action
                ToastService.ShowSuccess($"Account for '{SelectedAccount.CustomerName}' has been unfrozen.", "Account Unfrozen");
                AllAccounts = CustomerAccountService.GetAll();
                SummaryStats = CustomerAccountService.GetSummaryStats();
                StateHasChanged();
                await JS.InvokeVoidAsync("feather.replace");
            }
            else
            {
                ToastService.ShowError(result.Message, "Unfreeze Failed");
            }
        }

        #endregion

        #region Badge Helpers

        private string GetStatusBadgeClass(string status)
        {
            return status switch
            {
                CustomerAccountStatuses.Active => "bg-success-transparent text-success",
                CustomerAccountStatuses.Frozen => "bg-danger-transparent text-danger",
                CustomerAccountStatuses.Closed => "bg-secondary-transparent text-secondary",
                _ => "bg-light text-dark"
            };
        }
        private string GetStatusDotBadgeClass(string status)
        {
            return status switch
            {
                CustomerAccountStatuses.Active => "bg-success text-success",
                CustomerAccountStatuses.Frozen => "bg-danger text-danger",
                CustomerAccountStatuses.Closed => "bg-secondary text-secondary",
                _ => "bg-light text-dark"
            };
        }

        #endregion
    }
}
