using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FinanceConnect.Client.Pages.AR.CustomerAging
{
    public partial class CustomerAgingList : ComponentBase
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] CustomerAgingService AgingService { get; set; } = default!;
        [Inject] BranchService BranchService { get; set; } = default!;
        [Inject] CurrencyService CurrencyService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;

        private bool isInitialized = false;
        private bool isLoading = false;

        private List<CustomerAgingViewModel> Snapshots = new();
        private List<CustomerAgingViewModel> FilteredSnapshots = new();
        private CustomerAgingStatisticsViewModel Statistics = new();
        private BucketSummaryViewModel BucketSummaryViewModel = new();
        private CustomerAgingViewModel? LatestSnapshot;

        private string searchText = string.Empty;
        private string selectedAgingBasis = string.Empty;
        private string selectedStatus = string.Empty;
        private DateTime? FromDate;
        private DateTime? ToDate;

        private CustomerAgingViewModel? SelectedSnapshot;

        // Generate modal fields
        private DateTime generateAsOfDate = DateTime.Today;
        private string generateAgingBasis = string.Empty;
        private string generateBranchId = string.Empty;
        private string generateCurrencyId = string.Empty;

        // Generate modal lookup data
        private List<BranchModel> GenerateBranches = new();
        private List<CurrencyModel> GenerateCurrencies = new();
        private int VisibleColumnCount;
        // Pagination
        private int CurrentPage = 1;
        private int PageSize = 10;
        private int TotalPages => FilteredSnapshots.Count > 0 ? (int)Math.Ceiling((double)FilteredSnapshots.Count / PageSize) : 0;
        private IEnumerable<CustomerAgingViewModel> PagedSnapshots => FilteredSnapshots
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize);

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
            selectedStatus = null;
            selectedAgingBasis = null;
            searchText = "";

            await Task.Delay(100); // Simulate async load

            Snapshots = AgingService.GetAll();
            Statistics = AgingService.GetStatistics();

            // Load lookup data for generate modal
            GenerateBranches = BranchService.GetAll();
            GenerateCurrencies = CurrencyService.GetAll();

            // Get latest completed snapshot for bucket summary
            LatestSnapshot = Snapshots
                .Where(s => s.SnapshotStatus == SnapshotStatuses.Completed)
                .OrderByDescending(s => s.AsOfDate)
                .FirstOrDefault();

            if (LatestSnapshot != null)
            {
                BucketSummaryViewModel = AgingService.GetBucketSummary(LatestSnapshot.CustomerAgingId);
            }

            ApplyFilters();

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
            ApplyFilters();
            VisibleColumnCount =
    await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        private List<string> AvailableAgingBases => Snapshots
            .Select(s => s.AgingBasis)
            .Where(b => !string.IsNullOrEmpty(b))
            .Distinct()
            .OrderBy(b => b)
            .ToList();

        private List<string> AvailableStatuses => Snapshots
            .Select(s => s.SnapshotStatus)
            .Where(s => !string.IsNullOrEmpty(s))
            .Distinct()
            .OrderBy(s => s)
            .ToList();

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
                    (s.JobRunId?.ToLower().Contains(term) ?? false) ||
                    s.AsOfDate.ToString("dd-MMM-yyyy").ToLower().Contains(term));
            }

            // Aging basis filter
            if (!string.IsNullOrWhiteSpace(selectedAgingBasis))
            {
                query = query.Where(s => s.AgingBasis == selectedAgingBasis);
            }

            // Status filter
            if (!string.IsNullOrWhiteSpace(selectedStatus))
            {
                query = query.Where(s => s.SnapshotStatus == selectedStatus);
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
                                     .ThenByDescending(s => s.GeneratedOn)
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

        private void OpenRowDetails(CustomerAgingViewModel snapshot)
        {
            SelectedSnapshot = snapshot;
        }

        private void OpenRetryModal(CustomerAgingViewModel snapshot)
        {
            SelectedSnapshot = snapshot;
        }

        private void OpenDeleteModal(CustomerAgingViewModel snapshot)
        {
            SelectedSnapshot = snapshot;
        }

        #endregion

        #region Actions

        private async Task GenerateSnapshot()
        {
            if (string.IsNullOrEmpty(generateAgingBasis))
            {
                ToastService.ShowWarning("Aging Basis is required. Please select an aging basis.");
                return;
            }

            // Parse optional branch/currency from modal selections
            Guid? branchId = null;
            string? branchName = null;
            if (!string.IsNullOrEmpty(generateBranchId) && Guid.TryParse(generateBranchId, out var bid))
            {
                branchId = bid;
                branchName = GenerateBranches.FirstOrDefault(b => b.Id == bid)?.BranchName;
            }

            Guid currencyId = Guid.Parse("d1d1d1d1-d1d1-d1d1-d1d1-d1d1d1d1d101");
            string currencyCode = "INR";
            string currencyName = "Indian Rupee";
            if (!string.IsNullOrEmpty(generateCurrencyId) && Guid.TryParse(generateCurrencyId, out var cid))
            {
                currencyId = cid;
                var selectedCurrency = GenerateCurrencies.FirstOrDefault(c => c.Id == cid);
                if (selectedCurrency != null)
                {
                    currencyCode = selectedCurrency.CurrencyCode;
                    currencyName = selectedCurrency.CurrencyName;
                }
            }

            var result = AgingService.GenerateSnapshot(
                companyId: Guid.Parse("c0c0c0c0-c0c0-c0c0-c0c0-c0c0c0c0c001"), // Demo company
                companyName: "Ascending Software Private Limited",
                asOfDate: generateAsOfDate,
                agingBasis: generateAgingBasis,
                currencyId: currencyId,
                currencyCode: currencyCode,
                currencyName: currencyName,
                userName: "System",
                branchId: branchId,
                branchName: branchName);

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                // Reset modal fields
                generateBranchId = string.Empty;
                generateCurrencyId = string.Empty;
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

            var result = AgingService.RetryFailedSnapshot(SelectedSnapshot.CustomerAgingId, "System");

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

            var result = AgingService.Delete(SelectedSnapshot.CustomerAgingId);

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

        private string TruncateName(string? name, int maxLength)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;
            return name.Length <= maxLength ? name : name.Substring(0, maxLength) + "...";
        }

        private static string GetStatusBadgeClass(string status) => status switch
        {
            SnapshotStatuses.Generating => "bg-info-transparent text-info",
            SnapshotStatuses.Completed => "bg-success-transparent text-success",
            SnapshotStatuses.Failed => "bg-danger-transparent text-danger",
            _ => "bg-secondary-transparent text-secondary"
        };
        private static string GetStatusDotBadgeClass(string status) => status switch
        {
            SnapshotStatuses.Generating => "bg-info text-info",
            SnapshotStatuses.Completed => "bg-success text-success",
            SnapshotStatuses.Failed => "bg-danger text-danger",
            _ => "bg-secondary text-secondary"
        };
        private static string GetBasisIcon(string basis) => basis switch
        {
            AgingBasisTypes.DueDate => "ti ti-calendar-check",
            AgingBasisTypes.InvoiceDate => "ti ti-file-text",
            _ => "ti ti-info-circle"
        };


        private static string GetBasisBadgeClass(string basis) => basis switch
        {
            AgingBasisTypes.DueDate => "bg-primary-transparent text-primary",
            AgingBasisTypes.InvoiceDate => "bg-info-transparent text-info",
            _ => "bg-secondary-transparent text-secondary"
        };

        #endregion
    }
}
