using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.AP.VendorAccount
{
    public partial class VendorAccountList
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject] VendorAccountService VendorAccountService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] AuthService AuthService { get; set; } = default!;

        private bool isInitialized = false;
        private bool isLoading = false;

        private List<VendorAccountViewModel> AllAccounts = new();
        private VendorAccountViewModel? SelectedAccount = null;
        private VendorAccountSummaryStatsViewModel SummaryStats = new();

        // Filters
        private string searchText = string.Empty;
        private string _selectedStatus = string.Empty;
        private string _selectedBlockFilter = string.Empty;
        private string _selectedBalanceFilter = string.Empty;

        // Freeze modal inputs
        private string freezeReasonInput = string.Empty;

        // Pagination
        private int CurrentPage = 1;
        private int PageSize = 10;
        private int VisibleColumnCount;
        private string SelectedStatus
        {
            get => _selectedStatus;
            set { _selectedStatus = value; CurrentPage = 1; }
        }

        private string SelectedBlockFilter
        {
            get => _selectedBlockFilter;
            set { _selectedBlockFilter = value; CurrentPage = 1; }
        }

        private string SelectedBalanceFilter
        {
            get => _selectedBalanceFilter;
            set { _selectedBalanceFilter = value; CurrentPage = 1; }
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
            SelectedStatus = null!;
            SelectedBlockFilter = null!;
            SelectedBalanceFilter = null!;
            searchText = "";
            await Task.Delay(100); // Simulate async load

            AllAccounts = VendorAccountService.GetAll();
            SummaryStats = VendorAccountService.GetSummaryStats();

            isLoading = false;
            StateHasChanged();
        }

        private async Task OnRefreshAsync()
        {
            // Reset to seed data and reload
            VendorAccountService.ResetToSeed();
            await LoadDataAsync();
            await JS.InvokeVoidAsync("feather.replace");
            ToastService.ShowSuccess("Data refreshed with fresh seed data", "Refreshed");
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

        private List<string> AvailableStatuses => AllAccounts
            .Select(a => a.AccountStatus)
            .Where(s => !string.IsNullOrEmpty(s))
            .Distinct()
            .OrderBy(s => s)
            .ToList();

        private List<VendorAccountViewModel> FilteredAccounts
        {
            get
            {
                var result = AllAccounts.AsEnumerable();

                // Search filter
                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    var search = searchText.ToLower();
                    result = result.Where(a =>
                        (a.VendorCode?.ToLower().Contains(search) ?? false) ||
                        (a.VendorName?.ToLower().Contains(search) ?? false) ||
                        (a.CurrencyCode?.ToLower().Contains(search) ?? false));
                }

                // Status filter
                if (!string.IsNullOrEmpty(SelectedStatus))
                {
                    result = result.Where(a => a.AccountStatus == SelectedStatus);
                }

                // Block filter
                if (!string.IsNullOrEmpty(SelectedBlockFilter))
                {
                    result = SelectedBlockFilter switch
                    {
                        "paymentblocked" => result.Where(a => a.IsPaymentBlocked),
                        "postingblocked" => result.Where(a => a.IsPostingBlocked),
                        "notblocked" => result.Where(a => !a.IsPaymentBlocked && !a.IsPostingBlocked),
                        _ => result
                    };
                }

                // Balance filter
                if (!string.IsNullOrEmpty(SelectedBalanceFilter))
                {
                    result = SelectedBalanceFilter switch
                    {
                        "outstanding" => result.Where(a => a.OutstandingPayableAmount > 0),
                        "advance" => result.Where(a => a.AdvancePaidAmount > 0),
                        "zero" => result.Where(a => a.OutstandingPayableAmount == 0 && a.AdvancePaidAmount == 0),
                        _ => result
                    };
                }

                return result.OrderByDescending(a => a.UpdatedAt ?? a.CreatedAt).ToList();
            }
        }

        private List<VendorAccountViewModel> PagedAccounts =>
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

        private void OpenRowDetails(VendorAccountViewModel account)
        {
            SelectedAccount = account;
        }

        private void OpenFreezeModal(VendorAccountViewModel account)
        {
            SelectedAccount = account;
            freezeReasonInput = string.Empty;
        }

        private void ConfirmUnfreeze(VendorAccountViewModel account)
        {
            SelectedAccount = account;
        }

        #endregion

        #region Actions

        private async Task FreezeConfirmed()
        {
            if (SelectedAccount == null)
                return;

            if (string.IsNullOrWhiteSpace(freezeReasonInput))
            {
                ToastService.ShowError("Please enter a reason for freezing the account.", "Validation Error");
                return;
            }

            var userId = Guid.NewGuid(); // In real app, get from AuthService
            var userName = AuthService.CurrentUser?.UserName ?? "System";

            var result = VendorAccountService.Freeze(SelectedAccount.Id, freezeReasonInput, userId, userName);

            if (result.Success)
            {
                ToastService.ShowSuccess($"Account for '{SelectedAccount.VendorName}' has been frozen.", "Account Frozen");
                await LoadDataAsync();
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

            var result = VendorAccountService.Unfreeze(SelectedAccount.Id, userName);

            if (result.Success)
            {
                ToastService.ShowSuccess($"Account for '{SelectedAccount.VendorName}' has been unfrozen.", "Account Unfrozen");
                await LoadDataAsync();
                await JS.InvokeVoidAsync("feather.replace");
            }
            else
            {
                ToastService.ShowError(result.Message, "Unfreeze Failed");
            }
        }

        #endregion

        #region Helper Methods

        private string GetStatusBadgeClass(string status)
        {
            return status switch
            {
                VendorAccountStatuses.Active => "bg-success-transparent text-success",
                VendorAccountStatuses.Frozen => "bg-danger-transparent text-danger",
                VendorAccountStatuses.Closed => "bg-secondary-transparent text-secondary",
                _ => "bg-light text-dark"
            };
        }
        private string GetStatusDotBadgeClass(string status)
        {
            return status switch
            {
                VendorAccountStatuses.Active => "bg-success text-success",
                VendorAccountStatuses.Frozen => "bg-danger text-danger",
                VendorAccountStatuses.Closed => "bg-secondary text-secondary",
                _ => "bg-light text-dark"
            };
        }

        private string TruncateName(string? name, int maxLength)
        {
            if (string.IsNullOrEmpty(name))
                return string.Empty;
            
            return name.Length <= maxLength ? name : name.Substring(0, maxLength) + "...";
        }

        #endregion
    }
}
