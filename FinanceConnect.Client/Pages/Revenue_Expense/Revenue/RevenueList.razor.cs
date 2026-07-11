using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.RevenueViewModel;

namespace FinanceConnect.Client.Pages.Revenue_Expense.Revenue
{
    public partial class RevenueList
    {
        [Inject] RevenueService    RevenueService    { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] NavigationManager Nav               { get; set; } = default!;
        [Inject] IJSRuntime        JS                { get; set; } = default!;
        [Inject] ToastService      ToastService      { get; set; } = default!;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
        }

        // ── Data ───────────────────────────────────────────────────────────────
        private List<ViewModels.RevenueViewModel.Revenue> AllItems     = new();
        private List<ViewModels.RevenueViewModel.Revenue> FilteredItems = new();

        private List<CompanyModel> Companies = new();

        private ViewModels.RevenueViewModel.Revenue? SelectedItem;
        private bool canDelete = false;
        private bool isInitialized = false;
        private bool isLoading = false;

        // ── Filters ────────────────────────────────────────────────────────────
        private string searchText = "";
        private int PageSize = 10;
        private int CurrentPage = 1;
        private const int PageWindowSize = 2;

        private string selectedStatus   = "";
        private string selectedType     = "";
        private Guid?  selectedCompany  = null;

        private int TotalPages =>
            FilteredItems.Count == 0
                ? 1
                : (int)Math.Ceiling((double)FilteredItems.Count / PageSize);

        private List<ViewModels.RevenueViewModel.Revenue> PagedItems =>
            FilteredItems
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

        private IEnumerable<int> VisiblePages
        {
            get
            {
                if (TotalPages <= PageWindowSize)
                    return Enumerable.Range(1, TotalPages);

                int start = Math.Max(1, CurrentPage - PageWindowSize / 2);
                int end   = start + PageWindowSize - 1;
                if (end > TotalPages) { end = TotalPages; start = end - PageWindowSize + 1; }
                return Enumerable.Range(start, end - start + 1);
            }
        }

        private string SelectedStatusFilter
        {
            get => selectedStatus;
            set { selectedStatus = value; ApplyFilters(); }
        }

        private string SelectedTypeFilter
        {
            get => selectedType;
            set { selectedType = value; ApplyFilters(); }
        }

        private Guid? SelectedCompanyFilter
        {
            get => selectedCompany;
            set { selectedCompany = value; ApplyFilters(); }
        }

        // Called by @onchange on filter selects (BudgetLine pattern)
        private void OnFilterChanged(ChangeEventArgs e) => ApplyFilters();

        // ── Lifecycle ──────────────────────────────────────────────────────────
        protected override void OnInitialized()
        {
            Companies = MasterDataService.GetAllCompanies();
            LoadData();
        }

        private void LoadData()
        {
            AllItems      = RevenueService.GetAll();
            isInitialized = true;
            ApplyFilters();
        }

        // ── Search & Filter ────────────────────────────────────────────────────
        private void OnSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString() ?? "";
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            IEnumerable<ViewModels.RevenueViewModel.Revenue> q = AllItems;

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                q = q.Where(r =>
                    r.RevenueCode.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    r.RevenueName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    (r.CustomerNameSnapshot ?? "").Contains(searchText, StringComparison.OrdinalIgnoreCase));
            }

            if (selectedCompany.HasValue)
                q = q.Where(r => r.CompanyId == selectedCompany.Value);

            if (!string.IsNullOrWhiteSpace(selectedStatus) &&
                Enum.TryParse<RevenueStatus>(selectedStatus, out var parsedStatus))
                q = q.Where(r => r.Status == parsedStatus);

            if (!string.IsNullOrWhiteSpace(selectedType) &&
                Enum.TryParse<RevenueType>(selectedType, out var parsedType))
                q = q.Where(r => r.RevenueType == parsedType);

            FilteredItems = q.OrderByDescending(r => r.UpdatedAt ?? r.CreatedAt).ToList();
            CurrentPage   = 1;
        }

        // ── Pagination ─────────────────────────────────────────────────────────
        private void OnPageSizeChange(ChangeEventArgs e)
        {
            PageSize    = int.Parse(e.Value!.ToString()!);
            CurrentPage = 1;
        }

        private void PreviousPage() { if (CurrentPage > 1) CurrentPage--; }
        private void NextPage()     { if (CurrentPage < TotalPages) CurrentPage++; }
        private void GoToPage(int p) { if (p >= 1 && p <= TotalPages) CurrentPage = p; }

        // ── Selection & View ───────────────────────────────────────────────────
        private void SelectItem(ViewModels.RevenueViewModel.Revenue item)
        {
            SelectedItem = item;
            canDelete    = !item.IsLocked &&
                           item.Status != RevenueStatus.FullyRecognized &&
                           item.Status != RevenueStatus.Closed;
        }

        // ── Actions ────────────────────────────────────────────────────────────
        private void LockConfirmed()
        {
            if (SelectedItem is null) return;
            RevenueService.Lock(SelectedItem.RevenueId);
            ToastService.ShowSuccess($"Revenue '{SelectedItem.RevenueCode}' locked successfully.", "Locked");
            LoadData();
        }

        private void UnlockConfirmed()
        {
            if (SelectedItem is null) return;
            RevenueService.Unlock(SelectedItem.RevenueId);
            ToastService.ShowSuccess($"Revenue '{SelectedItem.RevenueCode}' unlocked successfully.", "Unlocked");
            LoadData();
        }

        private void DeleteConfirmed()
        {
            if (SelectedItem is null) return;
            try
            {
                RevenueService.Delete(SelectedItem.RevenueId);
                ToastService.ShowSuccess("Revenue record deleted.", "Deleted");
                LoadData();
            }
            catch (Exception ex)
            {
                ToastService.ShowError(ex.Message);
            }
        }

        private async Task OnRefreshAsync()
        {
            isLoading = true;
            StateHasChanged();
            await Task.Delay(300);
            RevenueService.ResetToSeed();
            searchText     = "";
            selectedStatus = "";
            selectedType   = "";
            selectedCompany = null;
            LoadData();
            isLoading = false;
            await JS.InvokeVoidAsync("feather.replace");
            ToastService.ShowInfo("Revenue list refreshed.", "Refreshed");
        }

        // ── UI Helpers ─────────────────────────────────────────────────────────
        private string GetCurrencySymbol(Guid currencyId)
        {
            var symbols = new Dictionary<Guid, string>
            {
                { Data.MasterDataIds.Currencies.INR, "₹" },
                { Data.MasterDataIds.Currencies.USD, "$" },
                { Data.MasterDataIds.Currencies.GBP, "£" },
                { Data.MasterDataIds.Currencies.EUR, "€" },
                { Data.MasterDataIds.Currencies.AED, "د.إ" },
                { Data.MasterDataIds.Currencies.SGD, "S$" },
                { Data.MasterDataIds.Currencies.JPY, "¥" },
                { Data.MasterDataIds.Currencies.AUD, "A$" },
                { Data.MasterDataIds.Currencies.CAD, "C$" },
            };
            return symbols.TryGetValue(currencyId, out var sym) ? sym : "";
        }

        private string GetCurrencyCode(Guid currencyId)
        {
            var codes = new Dictionary<Guid, string>
            {
                { Data.MasterDataIds.Currencies.INR, "INR" },
                { Data.MasterDataIds.Currencies.USD, "USD" },
                { Data.MasterDataIds.Currencies.GBP, "GBP" },
                { Data.MasterDataIds.Currencies.EUR, "EUR" },
                { Data.MasterDataIds.Currencies.AED, "AED" },
                { Data.MasterDataIds.Currencies.SGD, "SGD" },
                { Data.MasterDataIds.Currencies.JPY, "JPY" },
                { Data.MasterDataIds.Currencies.AUD, "AUD" },
                { Data.MasterDataIds.Currencies.CAD, "CAD" },
            };
            return codes.TryGetValue(currencyId, out var code) ? code : "—";
        }

        private string GetStatusBadgeClass(RevenueStatus status) => status switch
        {
            RevenueStatus.Draft              => "bg-secondary-transparent text-secondary",
            RevenueStatus.Confirmed          => "bg-primary-transparent text-primary",
            RevenueStatus.PendingRecognition => "bg-warning-transparent text-warning",
            RevenueStatus.PartiallyRecognized=> "bg-info-transparent text-info",
            RevenueStatus.FullyRecognized    => "bg-success-transparent text-success",
            RevenueStatus.Deferred           => "bg-purple-transparent text-purple",
            RevenueStatus.Cancelled          => "bg-danger-transparent text-danger",
            RevenueStatus.Closed             => "bg-dark-transparent text-dark",
            _                                => "bg-secondary-transparent text-secondary"
        };

        private string GetRecognitionBadgeClass(RecognitionStatus status) => status switch
        {
            RecognitionStatus.NotStarted        => "bg-secondary-transparent text-secondary",
            RecognitionStatus.Ready             => "bg-info-transparent text-info",
            RecognitionStatus.InProgress        => "bg-primary-transparent text-primary",
            RecognitionStatus.PartiallyRecognized => "bg-warning-transparent text-warning",
            RecognitionStatus.FullyRecognized   => "bg-success-transparent text-success",
            RecognitionStatus.Deferred          => "bg-purple-transparent text-purple",
            RecognitionStatus.OnHold            => "bg-danger-transparent text-danger",
            _                                   => "bg-secondary-transparent text-secondary"
        };

        private string GetBillingBadgeClass(BillingStatus s) => s switch
        {
            BillingStatus.NotBilled       => "bg-secondary-transparent text-secondary",
            BillingStatus.PartiallyBilled => "bg-warning-transparent text-warning",
            BillingStatus.FullyBilled     => "bg-success-transparent text-success",
            BillingStatus.AdvanceBilled   => "bg-info-transparent text-info",
            _                             => "bg-secondary-transparent text-secondary"
        };

        private string GetCollectionBadgeClass(CollectionStatus s) => s switch
        {
            CollectionStatus.NotCollected       => "bg-secondary-transparent text-secondary",
            CollectionStatus.PartiallyCollected => "bg-warning-transparent text-warning",
            CollectionStatus.FullyCollected     => "bg-success-transparent text-success",
            CollectionStatus.AdvanceCollected   => "bg-info-transparent text-info",
            _                                   => "bg-secondary-transparent text-secondary"
        };

        // ActiveDot CSS class — matching BudgetLine pattern
        private string GetStatusDot(RevenueStatus status) => status switch
        {
            RevenueStatus.FullyRecognized     => "bg-success text-success",
            RevenueStatus.Confirmed           => "bg-primary text-primary",
            RevenueStatus.PartiallyRecognized => "bg-info text-info",
            RevenueStatus.PendingRecognition  => "bg-warning text-warning",
            RevenueStatus.Deferred            => "bg-purple text-purple",
            RevenueStatus.Cancelled           => "bg-danger text-danger",
            RevenueStatus.Closed              => "bg-dark text-dark",
            _                                 => "bg-secondary text-secondary"
        };
    }
}
