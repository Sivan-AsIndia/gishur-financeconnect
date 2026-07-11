using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.AP.VendorAging
{
    public partial class VendorAgingDetails : ComponentBase
    {
        [Parameter] public Guid Id { get; set; }

        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] VendorAgingService AgingService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;

        private bool isInitialized = false;

        private VendorAgingViewModel? AgingSnapshot;
        private VendorAgingBucketSummaryViewModel BucketSummaryViewModel = new();
        private List<VendorAgingVendorRowViewModel> VendorRows = new();
        private List<VendorAgingVendorRowViewModel> FilteredVendorRows = new();
        private List<VendorAgingBillRowViewModel> VendorBillRows = new();

        private VendorAgingVendorRowViewModel? SelectedVendorRow;

        // Filters
        private string vendorSearchText = string.Empty;
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
                VendorRows = AgingService.GetVendorRows(Id);
                ApplyVendorFilters();
            }
        }

        private void GoBack()
        {
            Nav.NavigateTo("/vendor-aging");
        }

        private void OnVendorSearch(ChangeEventArgs e)
        {
            vendorSearchText = e.Value?.ToString() ?? string.Empty;
            ApplyVendorFilters();
        }

        private void ApplyVendorFilters()
        {
            var query = VendorRows.AsEnumerable();

            // Search filter
            if (!string.IsNullOrWhiteSpace(vendorSearchText))
            {
                var term = vendorSearchText.ToLower();
                query = query.Where(r =>
                    (r.VendorCodeSnapshot?.ToLower().Contains(term) ?? false) ||
                    (r.VendorNameSnapshot?.ToLower().Contains(term) ?? false));
            }

            // Overdue only filter
            if (showOverdueOnly)
            {
                query = query.Where(r => r.OverdueTotalAmount > 0);
            }

            // Bucket filter
            if (!string.IsNullOrEmpty(selectedBucket))
            {
                query = selectedBucket switch
                {
                    VendorAgingBucketCodes.CurrentNotDue => query.Where(r => r.CurrentNotDueAmount > 0),
                    VendorAgingBucketCodes.Days0To30 => query.Where(r => r.Bucket_0_30 > 0),
                    VendorAgingBucketCodes.Days31To60 => query.Where(r => r.Bucket_31_60 > 0),
                    VendorAgingBucketCodes.Days61To90 => query.Where(r => r.Bucket_61_90 > 0),
                    VendorAgingBucketCodes.Days91To120 => query.Where(r => r.Bucket_91_120 > 0),
                    VendorAgingBucketCodes.Days121To180 => query.Where(r => r.Bucket_121_180 > 0),
                    VendorAgingBucketCodes.Days181To365 => query.Where(r => r.Bucket_181_365 > 0),
                    VendorAgingBucketCodes.Days366Plus => query.Where(r => r.Bucket_366_Plus > 0),
                    _ => query
                };
            }

            FilteredVendorRows = query
                .OrderByDescending(r => r.NetPayableAmount)
                .ToList();

            // Clear selected vendor if filtered out
            if (SelectedVendorRow != null && !FilteredVendorRows.Any(r => r.VendorId == SelectedVendorRow.VendorId))
            {
                SelectedVendorRow = null;
                VendorBillRows = new();
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
            ApplyVendorFilters();
        }

        private void ClearBucketFilter()
        {
            selectedBucket = string.Empty;
            ApplyVendorFilters();
        }

        private void SelectVendor(VendorAgingVendorRowViewModel row)
        {
            SelectedVendorRow = row;
            VendorBillRows = AgingService.GetBillRowsByVendor(Id, row.VendorId);
        }

        #region Helper Methods

        private string TruncateName(string? name, int maxLength)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;
            return name.Length <= maxLength ? name : name.Substring(0, maxLength) + "...";
        }

        private static string GetStatusBadgeClass(string status) => status switch
        {
            VendorAgingRunStatuses.Started => "bg-info-transparent text-info",
            VendorAgingRunStatuses.Completed => "bg-success-transparent text-success",
            VendorAgingRunStatuses.Failed => "bg-danger-transparent text-danger",
            VendorAgingRunStatuses.Finalized => "bg-primary-transparent text-primary",
            _ => "bg-secondary-transparent text-secondary"
        };

        private static string GetRunTypeBadgeClass(string runType) => runType switch
        {
            VendorAgingRunTypes.Nightly => "bg-info-transparent text-info",
            VendorAgingRunTypes.OnDemand => "bg-warning-transparent text-warning",
            VendorAgingRunTypes.MonthEndFinal => "bg-primary-transparent text-primary",
            _ => "bg-secondary-transparent text-secondary"
        };

        private static string GetBucketBadgeClass(string bucket) => bucket switch
        {
            VendorAgingBucketCodes.CurrentNotDue => "bg-success-transparent text-success",
            VendorAgingBucketCodes.Days0To30 => "bg-info-transparent text-info",
            VendorAgingBucketCodes.Days31To60 => "bg-warning-transparent text-warning",
            VendorAgingBucketCodes.Days61To90 => "bg-orange-transparent",
            VendorAgingBucketCodes.Days91To120 => "bg-orange-transparent",
            VendorAgingBucketCodes.Days121To180 => "bg-danger-transparent text-danger",
            VendorAgingBucketCodes.Days181To365 => "bg-danger-transparent text-danger",
            VendorAgingBucketCodes.Days366Plus => "bg-purple-transparent",
            _ => "bg-secondary-transparent text-secondary"
        };

        private static string GetReconciliationBadgeClass(string status) => status switch
        {
            VendorAgingReconciliationStatuses.Matched => "bg-success-transparent text-success",
            VendorAgingReconciliationStatuses.MinorDifference => "bg-warning-transparent text-warning",
            VendorAgingReconciliationStatuses.Mismatch => "bg-danger-transparent text-danger",
            _ => "bg-secondary-transparent text-secondary"
        };

        private static string GetOverdueBadgeClass(int overdueDays)
        {
            if (overdueDays <= 0) return "bg-success-transparent text-success";
            if (overdueDays <= 30) return "bg-info-transparent text-info";
            if (overdueDays <= 60) return "bg-warning-transparent text-warning";
            if (overdueDays <= 90) return "bg-orange-transparent";
            if (overdueDays <= 120) return "bg-orange-transparent";
            if (overdueDays <= 180) return "bg-danger-transparent text-danger";
            if (overdueDays <= 365) return "bg-danger-transparent text-danger";
            return "bg-purple-transparent";
        }

        #endregion
    }
}
