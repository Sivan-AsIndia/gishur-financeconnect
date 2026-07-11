using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FinanceConnect.Client.Pages.AP.VendorAging
{
    public partial class VendorAgingList : ComponentBase
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] VendorAgingService AgingService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] VendorService VendorService { get; set; } = default!;
        [Inject] BranchService BranchService { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;

        private bool isInitialized = false;
        private bool isLoading = false;

        private List<VendorAgingViewModel> Snapshots = new();
        private List<VendorAgingViewModel> FilteredSnapshots = new();
        private VendorAgingStatisticsViewModel Statistics = new();
        private VendorAgingBucketSummaryViewModel BucketSummaryViewModel = new();
        private VendorAgingViewModel? LatestSnapshot;

        private string searchText = string.Empty;
        private string selectedRunType = string.Empty;
        private string selectedStatus = string.Empty;
        private DateTime? FromDate;
        private DateTime? ToDate;

        private VendorAgingViewModel? SelectedSnapshot;

        // Generate modal fields
        private DateTime generateAsOfDate = DateTime.Today;
        private string generateRunType = string.Empty;
        private bool generateIncludeCredits = true;
        private Guid? generateBranchId;
        private Guid? generateVendorId;
        private Dictionary<string, string> generateValidationErrors = new();

        // Generate modal dropdown data
        private List<BranchModel> Branches = new();
        private List<VendorViewModel> Vendors = new();

        // Distinct values from data for filter dropdowns
        private List<string> DistinctRunTypes => Snapshots
            .Select(s => s.RunType)
            .Where(r => !string.IsNullOrEmpty(r))
            .Distinct()
            .OrderBy(r => Array.IndexOf(VendorAgingRunTypes.All, r))
            .ToList();

        private List<string> DistinctStatuses => Snapshots
            .Select(s => s.RunStatus)
            .Where(s => !string.IsNullOrEmpty(s))
            .Distinct()
            .OrderBy(s => Array.IndexOf(VendorAgingRunStatuses.All, s))
            .ToList();

        // Pagination
        private int CurrentPage = 1;
        private int PageSize = 10;
        private int TotalPages => FilteredSnapshots.Count > 0 ? (int)Math.Ceiling((double)FilteredSnapshots.Count / PageSize) : 0;
        private IEnumerable<VendorAgingViewModel> PagedSnapshots => FilteredSnapshots
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize);
        private int VisibleColumnCount;
        protected override async Task OnInitializedAsync()
        {
            await LoadData();
            isInitialized = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
            VisibleColumnCount =
          await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        private async Task LoadData()
        {
            isLoading = true;
            StateHasChanged();
            ToDate = null;
            FromDate = null;
            selectedStatus = null!;
            selectedRunType = null!;
            searchText = "";
            await Task.Delay(100); // Simulate async load

            Snapshots = AgingService.GetAll();
            Statistics = AgingService.GetStatistics();
            
            // Get latest completed/finalized snapshot for bucket summary
            LatestSnapshot = Snapshots
                .Where(s => s.RunStatus == VendorAgingRunStatuses.Completed || s.RunStatus == VendorAgingRunStatuses.Finalized)
                .OrderByDescending(s => s.AsOfDate)
                .FirstOrDefault();

            if (LatestSnapshot != null)
            {
                BucketSummaryViewModel = AgingService.GetBucketSummary(LatestSnapshot.VendorAgingId);
            }

            ApplyFilters();
            LoadGenerateDropdowns();
            isLoading = false;
            StateHasChanged();
        }

        private async Task OnRefreshAsync()
        {
            AgingService.ResetToSeed();
            await LoadData();
            ToastService.ShowSuccess("Data refreshed successfully");
        }

        private async Task OnSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString() ?? string.Empty;
            CurrentPage = 1;
            VisibleColumnCount =
        await JS.InvokeAsync<int>("getVisibleTableColumns");
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var query = Snapshots.AsEnumerable();

            // Search filter
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var term = searchText.ToLower();
                query = query.Where(s =>
                    (s.CompanyName?.ToLower().Contains(term) ?? false) ||
                    (s.BranchName?.ToLower().Contains(term) ?? false) ||
                    s.AsOfDate.ToString("dd-MMM-yyyy").ToLower().Contains(term));
            }

            // Run type filter
            if (!string.IsNullOrWhiteSpace(selectedRunType))
            {
                query = query.Where(s => s.RunType == selectedRunType);
            }

            // Status filter
            if (!string.IsNullOrWhiteSpace(selectedStatus))
            {
                query = query.Where(s => s.RunStatus == selectedStatus);
            }

            // Date filters
            if (FromDate.HasValue)
            {
                query = query.Where(s => s.AsOfDate >= FromDate.Value);
            }
            if (ToDate.HasValue)
            {
                query = query.Where(s => s.AsOfDate <= ToDate.Value);
            }

            FilteredSnapshots = query.OrderByDescending(s => s.AsOfDate)
                                     .ThenByDescending(s => s.GeneratedAt)
                                     .ToList();

            // Reset to page 1 if current page exceeds total
            if (CurrentPage > TotalPages && TotalPages > 0)
            {
                CurrentPage = 1;
            }
        }

        #region Pagination

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
            if (int.TryParse(e.Value?.ToString(), out var size))
            {
                PageSize = size;
                CurrentPage = 1;
            }
        }

        #endregion

        #region Modal Handlers

        private void OpenRowDetails(VendorAgingViewModel snapshot)
        {
            SelectedSnapshot = snapshot;
        }

        private void OpenRetryModal(VendorAgingViewModel snapshot)
        {
            SelectedSnapshot = snapshot;
        }

        private void OpenDeleteModal(VendorAgingViewModel snapshot)
        {
            SelectedSnapshot = snapshot;
        }

        #endregion

        #region Actions

        private void LoadGenerateDropdowns()
        {
            Branches = BranchService.GetAll()
                .Where(b => b.Status == "Active")
                .ToList();

            Vendors = VendorService.GetAll()
                .Where(v => v.VendorStatus == VendorStatuses.Active)
                .OrderBy(v => v.VendorCode)
                .ToList();
        }

        private async Task GenerateSnapshot()
        {
            generateValidationErrors.Clear();

            if (generateAsOfDate == default)
                generateValidationErrors["AsOfDate"] = "As Of Date is required";

            if (string.IsNullOrWhiteSpace(generateRunType))
                generateValidationErrors["RunType"] = "Run Type is required";

            if (generateValidationErrors.Count > 0)
            {
                StateHasChanged();
                return;
            }

            var result = AgingService.GenerateSnapshot(
                companyId: Guid.Parse("c0c0c0c0-c0c0-c0c0-c0c0-c0c0c0c0c001"), // Demo company
                companyName: "Ascending Software Private Limited",
                asOfDate: generateAsOfDate,
                runType: generateRunType,
                currencyId: Guid.Parse("d1d1d1d1-d1d1-d1d1-d1d1-d1d1d1d1d101"),
                currencyCode: "INR",
                currencyName: "Indian Rupee",
                userName: "System",
                branchId: generateBranchId,
                branchName: generateBranchId.HasValue
                    ? Branches.FirstOrDefault(b => b.Id == generateBranchId.Value)?.BranchName
                    : null,
                includeOpenCredits: generateIncludeCredits);

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                await JS.InvokeVoidAsync("eval", "bootstrap.Modal.getInstance(document.getElementById('generateModal'))?.hide()");
                generateAsOfDate = DateTime.Today;
                generateRunType = string.Empty;
                generateBranchId = null;
                generateVendorId = null;
                generateValidationErrors.Clear();
                await LoadData();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        private async Task RetrySnapshot()
        {
            if (SelectedSnapshot == null) return;

            var result = AgingService.RetryFailedSnapshot(SelectedSnapshot.VendorAgingId, "System");

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                await LoadData();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        private async Task DeleteSnapshot()
        {
            if (SelectedSnapshot == null) return;

            var result = AgingService.Delete(SelectedSnapshot.VendorAgingId);

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                await LoadData();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        #endregion

        #region Helper Methods

        private static string GetStatusBadgeClass(string status) => status switch
        {
            VendorAgingRunStatuses.Started => "bg-info-transparent text-info",
            VendorAgingRunStatuses.Completed => "bg-success-transparent text-success",
            VendorAgingRunStatuses.Failed => "bg-danger-transparent text-danger",
            VendorAgingRunStatuses.Finalized => "bg-primary-transparent text-primary",
            _ => "bg-secondary-transparent text-secondary"
        };
        private static string GetStatusDotBadgeClass(string status) => status switch
        {
            VendorAgingRunStatuses.Started => "bg-info text-info",
            VendorAgingRunStatuses.Completed => "bg-success text-success",
            VendorAgingRunStatuses.Failed => "bg-danger text-danger",
            VendorAgingRunStatuses.Finalized => "bg-primary text-primary",
            _ => "bg-secondary text-secondary"
        };

        private string GetRunTypeIcon(string runType)
        {
            return runType switch
            {
                VendorAgingRunTypes.Nightly => "ti ti-moon",
                VendorAgingRunTypes.OnDemand => "ti ti-bolt",
                VendorAgingRunTypes.MonthEndFinal => "ti ti-calendar-check",
                _ => "ti ti-clock"
            };
        }

        private static string GetRunTypeBadgeClass(string runType) => runType switch
        {
            VendorAgingRunTypes.Nightly => "bg-info-transparent text-info",
            VendorAgingRunTypes.OnDemand => "bg-warning-transparent text-warning",
            VendorAgingRunTypes.MonthEndFinal => "bg-primary-transparent text-primary",
            _ => "bg-secondary-transparent text-secondary"
        };

        #endregion
    }
}
