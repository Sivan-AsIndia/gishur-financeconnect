using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.AR.Customer
{
    public partial class CustomerList
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject] CustomerService CustomerService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] AuthService AuthService { get; set; } = default!;

        private bool isInitialized = false;
        private bool isLoading = false;

        private List<CustomerViewModel> AllCustomers = new();
        private CustomerViewModel? SelectedCustomer;
        private int VisibleColumnCount;
        // Validation flags
        private bool canDeactivate = true;
        private bool canDelete = true;

        // Search and Filter
        private string searchText = "";
        private string _selectedCustomerType = "";
        private string _selectedStatus = "";
        private string _selectedHoldStatus = "";

        public string SelectedCustomerType
        {
            get => _selectedCustomerType;
            set { _selectedCustomerType = value; CurrentPage = 1; }
        }

        public string SelectedStatus
        {
            get => _selectedStatus;
            set { _selectedStatus = value; CurrentPage = 1; }
        }

        public string SelectedHoldStatus
        {
            get => _selectedHoldStatus;
            set { _selectedHoldStatus = value; CurrentPage = 1; }
        }

        // Pagination
        private int CurrentPage = 1;
        private int PageSize = 10;

        // Hold Modal
        private string holdStatusInput = "OnHold";
        private string holdReasonInput = "";

        protected override async Task OnInitializedAsync()
        {
            LoadCustomers();
            isInitialized = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
            VisibleColumnCount =
   await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        private void LoadCustomers()
        {
            SelectedHoldStatus = null;
            SelectedStatus = null;
            SelectedCustomerType = null;
            searchText = "";
            AllCustomers = CustomerService.GetAll();

        }

        private List<CustomerViewModel> FilteredCustomers
        {
            get
            {
                var result = AllCustomers.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    var search = searchText.ToLower();
                    result = result.Where(c =>
                        c.CustomerCode.ToLower().Contains(search) ||
                        c.CustomerName.ToLower().Contains(search) ||
                        (c.GSTIN?.ToLower().Contains(search) ?? false) ||
                        (c.PrimaryPhone?.ToLower().Contains(search) ?? false) ||
                        (c.PrimaryEmail?.ToLower().Contains(search) ?? false));
                }

                if (!string.IsNullOrWhiteSpace(SelectedCustomerType))
                {
                    result = result.Where(c => c.CustomerType == SelectedCustomerType);
                }

                if (!string.IsNullOrWhiteSpace(SelectedStatus))
                {
                    result = result.Where(c => c.CustomerStatus == SelectedStatus);
                }

                if (!string.IsNullOrWhiteSpace(SelectedHoldStatus))
                {
                    result = result.Where(c => c.CreditHoldStatus == SelectedHoldStatus);
                }

                // Sort by UpdatedAt (if available) or CreatedAt descending - latest records first
                return result.OrderByDescending(c => c.UpdatedAt ?? c.CreatedAt).ToList();
            }
        }

        private List<CustomerViewModel> PagedCustomers
        {
            get
            {
                return FilteredCustomers
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();
            }
        }

        private int TotalPages => Math.Max(1, (int)Math.Ceiling((double)FilteredCustomers.Count / PageSize));

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

        private void OnPageSizeChange(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int size))
            {
                PageSize = size;
                CurrentPage = 1;
            }
        }

        private async Task OnSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString() ?? "";
            CurrentPage = 1;
            VisibleColumnCount =
   await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        private async Task OnRefreshAsync()
        {
            isLoading = true;
            StateHasChanged();

            await Task.Delay(300);
            CustomerService.ResetToSeed();
            LoadCustomers();

            isLoading = false;
            await JS.InvokeVoidAsync("feather.replace");
            ToastService.ShowInfo("Customer list refreshed", "Refreshed");
        }

        private void OpenRowDetails(CustomerViewModel customer)
        {
            SelectedCustomer = customer;
        }

        private void OpenHoldModal(CustomerViewModel customer)
        {
            SelectedCustomer = customer;
            holdStatusInput = "OnHold";
            holdReasonInput = "";
        }

        private void ConfirmActivate(CustomerViewModel customer)
        {
            SelectedCustomer = customer;
        }

        private void ConfirmDeactivate(CustomerViewModel customer)
        {
            SelectedCustomer = customer;
            // Check if customer can be deactivated
            canDeactivate = true; // In real app, check for active transactions
        }

        private void ConfirmReleaseHold(CustomerViewModel customer)
        {
            SelectedCustomer = customer;
        }

        private void ConfirmDelete(CustomerViewModel customer)
        {
            SelectedCustomer = customer;
            // Check if customer can be deleted
            canDelete = true; // In real app, check for active transactions
        }

        private async Task ActivateConfirmed()
        {
            if (SelectedCustomer == null) return;

            var result = CustomerService.Activate(SelectedCustomer.Id, AuthService.CurrentUser?.UserName ?? "System");
            if (result.Success)
            {
                ToastService.ShowSuccess($"Customer '{SelectedCustomer.CustomerName}' activated successfully", "Activated");
                LoadCustomers();
                await JS.InvokeVoidAsync("feather.replace");
            }
            else
            {
                ToastService.ShowError(result.Message, "Activation Failed");
            }
        }

        private async Task DeactivateConfirmed()
        {
            if (SelectedCustomer == null) return;

            var result = CustomerService.Inactivate(SelectedCustomer.Id, AuthService.CurrentUser?.UserName ?? "System");
            if (result.Success)
            {
                ToastService.ShowWarning($"Customer '{SelectedCustomer.CustomerName}' deactivated successfully", "Deactivated");
                LoadCustomers();
                await JS.InvokeVoidAsync("feather.replace");
            }
            else
            {
                ToastService.ShowError(result.Message, "Deactivation Failed");
            }
        }

        private async Task PlaceHoldConfirmed()
        {
            if (SelectedCustomer == null) return;

            if (string.IsNullOrWhiteSpace(holdReasonInput))
            {
                ToastService.ShowError("Please enter a reason for the hold", "Validation Error");
                return;
            }

            var result = CustomerService.PlaceCreditHold(
                SelectedCustomer.Id,
                holdStatusInput,
                holdReasonInput,
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                AuthService.CurrentUser?.UserName ?? "System"
            );

            if (result.Success)
            {
                ToastService.ShowWarning($"Credit hold placed on '{SelectedCustomer.CustomerName}'", "Hold Placed");
                LoadCustomers();
                await JS.InvokeVoidAsync("feather.replace");
            }
            else
            {
                ToastService.ShowError(result.Message, "Hold Failed");
            }
        }

        private async Task ReleaseHoldConfirmed()
        {
            if (SelectedCustomer == null) return;

            var result = CustomerService.ReleaseCreditHold(SelectedCustomer.Id, AuthService.CurrentUser?.UserName ?? "System");
            if (result.Success)
            {
                ToastService.ShowSuccess($"Credit hold released for '{SelectedCustomer.CustomerName}'", "Hold Released");
                LoadCustomers();
                await JS.InvokeVoidAsync("feather.replace");
            }
            else
            {
                ToastService.ShowError(result.Message, "Release Failed");
            }
        }

        private async Task DeleteConfirmed()
        {
            if (SelectedCustomer == null) return;

            var result = CustomerService.Delete(SelectedCustomer.Id);
            if (result.Success)
            {
                // Red toast for delete
                ToastService.ShowError($"Customer '{SelectedCustomer.CustomerName}' deleted successfully", "Deleted");
                LoadCustomers();
                await JS.InvokeVoidAsync("feather.replace");
            }
            else
            {
                ToastService.ShowError(result.Message, "Delete Failed");
            }
        }

        private static string GetStatusBadgeClass(string? status) => status switch
        {
            CustomerStatuses.Active => "bg-success-transparent text-success",
            CustomerStatuses.Inactive => "bg-secondary-transparent text-secondary",
            CustomerStatuses.Blacklisted => "bg-danger-transparent text-danger",
            CustomerStatuses.Draft => "bg-warning-transparent text-warning",
            _ => "bg-secondary-transparent text-secondary"
        };
        private static string GetStatusDotBadgeClass(string? status) => status switch
        {
            CustomerStatuses.Active => "bg-success text-success",
            CustomerStatuses.Inactive => "bg-secondary text-secondary",
            CustomerStatuses.Blacklisted => "bg-danger text-danger",
            CustomerStatuses.Draft => "bg-warning text-warning",
            _ => "bg-secondary text-secondary"
        };

        private static string GetTypeBadgeClass(string type) => type switch
        {
            CustomerTypes.Business => "bg-info-transparent text-info",
            CustomerTypes.Individual => "bg-primary-transparent text-primary",
            CustomerTypes.Government => "bg-warning-transparent text-warning",
            CustomerTypes.Partner => "bg-success-transparent text-success",
            _ => "bg-secondary"
        };

        private static string GetHoldStatusBadgeClass(string? holdStatus) => holdStatus switch
        {
            CreditHoldStatuses.None => "bg-success-transparent text-success",
            CreditHoldStatuses.OnHold => "bg-danger-transparent text-danger",
            CreditHoldStatuses.TemporaryHold => "bg-warning-transparent text-warning",
            _ => "bg-secondary-transparent text-secondary"
        };
    }
}
