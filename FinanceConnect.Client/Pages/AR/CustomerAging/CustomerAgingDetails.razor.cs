using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.AR.CustomerAging
{
    public partial class CustomerAgingDetails : ComponentBase
    {
        [Parameter] public Guid Id { get; set; }

        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] CustomerAgingService AgingService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;

        private bool isInitialized = false;

        private CustomerAgingViewModel? AgingSnapshot;
        private BucketSummaryViewModel BucketSummaryViewModel = new();
        private List<CustomerAgingCustomerRowModel> CustomerRows = new();
        private List<CustomerAgingCustomerRowModel> FilteredCustomerRows = new();
        private List<CustomerAgingInvoiceRowModel> CustomerInvoiceRows = new();

        private CustomerAgingCustomerRowModel? SelectedCustomerRow;

        // Filters
        private string customerSearchText = string.Empty;
        private bool showOverdueOnly = false;
        private string selectedBucket = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
            isInitialized = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
        }

        private async Task LoadData()
        {
            await Task.Delay(50); // Simulate async load

            AgingSnapshot = AgingService.GetById(Id);

            if (AgingSnapshot != null)
            {
                BucketSummaryViewModel = AgingService.GetBucketSummary(Id);
                CustomerRows = AgingService.GetCustomerRows(Id);
                ApplyCustomerFilters();
            }
        }

        private void GoBack()
        {
            Nav.NavigateTo("/customer-aging");
        }

        private void OnCustomerSearch(ChangeEventArgs e)
        {
            customerSearchText = e.Value?.ToString() ?? string.Empty;
            ApplyCustomerFilters();
        }

        private void ApplyCustomerFilters()
        {
            var query = CustomerRows.AsEnumerable();

            // Search filter
            if (!string.IsNullOrWhiteSpace(customerSearchText))
            {
                var term = customerSearchText.ToLower();
                query = query.Where(r =>
                    (r.CustomerCodeSnapshot?.ToLower().Contains(term) ?? false) ||
                    (r.CustomerNameSnapshot?.ToLower().Contains(term) ?? false));
            }

            // Overdue only filter
            if (showOverdueOnly)
            {
                query = query.Where(r => r.MaxOverdueDays > 0);
            }

            // Bucket filter
            if (!string.IsNullOrEmpty(selectedBucket))
            {
                query = selectedBucket switch
                {
                    AgingBucketCodes.Current => query.Where(r => r.BucketCurrentAmount > 0),
                    AgingBucketCodes.Days1To30 => query.Where(r => r.Bucket1To30Amount > 0),
                    AgingBucketCodes.Days31To60 => query.Where(r => r.Bucket31To60Amount > 0),
                    AgingBucketCodes.Days61To90 => query.Where(r => r.Bucket61To90Amount > 0),
                    AgingBucketCodes.Days90Plus => query.Where(r => r.Bucket90PlusAmount > 0),
                    _ => query
                };
            }

            FilteredCustomerRows = query
                .OrderByDescending(r => r.CollectionsPriorityScore)
                .ThenByDescending(r => r.TotalOutstanding)
                .ToList();

            // Clear selected customer if filtered out
            if (SelectedCustomerRow != null && !FilteredCustomerRows.Any(r => r.CustomerId == SelectedCustomerRow.CustomerId))
            {
                SelectedCustomerRow = null;
                CustomerInvoiceRows = new();
            }
        }

        private void FilterByBucket(string bucket)
        {
            if (selectedBucket == bucket)
            {
                selectedBucket = string.Empty; // Toggle off
            }
            else
            {
                selectedBucket = bucket;
            }
            ApplyCustomerFilters();
        }

        private void ClearBucketFilter()
        {
            selectedBucket = string.Empty;
            ApplyCustomerFilters();
        }

        private void SelectCustomer(CustomerAgingCustomerRowModel row)
        {
            SelectedCustomerRow = row;
            CustomerInvoiceRows = AgingService.GetInvoiceRowsByCustomer(Id, row.CustomerId);
        }

        #region Helper Methods

        private string TruncateName(string? name, int maxLength)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;
            return name.Length <= maxLength ? name : name.Substring(0, maxLength) + "...";
        }

        private static string FormatAmount(decimal amount)
        {
            return amount == 0 ? "0" : amount.ToString("N0");
        }

        private static string GetStatusBadgeClass(string status) => status switch
        {
            SnapshotStatuses.Generating => "bg-info-transparent text-info",
            SnapshotStatuses.Completed => "bg-success-transparent text-success",
            SnapshotStatuses.Failed => "bg-danger-transparent text-danger",
            _ => "bg-secondary-transparent text-secondary"
        };

        private static string GetBasisBadgeClass(string basis) => basis switch
        {
            AgingBasisTypes.DueDate => "bg-primary-transparent text-primary",
            AgingBasisTypes.InvoiceDate => "bg-info-transparent text-info",
            _ => "bg-secondary-transparent text-secondary"
        };

        private static string GetBucketBadgeClass(string bucket) => bucket switch
        {
            AgingBucketCodes.Current => "bg-success-transparent text-success",
            AgingBucketCodes.Days1To30 => "bg-info-transparent text-info",
            AgingBucketCodes.Days31To60 => "bg-warning-transparent text-warning",
            AgingBucketCodes.Days61To90 => "bg-orange-transparent",
            AgingBucketCodes.Days90Plus => "bg-danger-transparent text-danger",
            _ => "bg-secondary-transparent text-secondary"
        };

        private static string GetOverdueBadgeClass(int overdueDays)
        {
            if (overdueDays <= 0) return "bg-success-transparent text-success";
            if (overdueDays <= 30) return "bg-info-transparent text-info";
            if (overdueDays <= 60) return "bg-warning-transparent text-warning";
            if (overdueDays <= 90) return "bg-orange-transparent";
            return "bg-danger-transparent text-danger";
        }

        #endregion
    }
}
