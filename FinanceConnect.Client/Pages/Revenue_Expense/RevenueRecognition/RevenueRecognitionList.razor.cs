using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.RevenueRecognitionViewModel;

namespace FinanceConnect.Client.Pages.Revenue_Expense.RevenueRecognition
{
    public partial class RevenueRecognitionList
    {
        [Inject] RevenueRecognitionService RecognitionService { get; set; } = default!;
        [Inject] MasterDataService         MasterDataService  { get; set; } = default!;
        [Inject] NavigationManager         Nav                { get; set; } = default!;
        [Inject] IJSRuntime                JS                 { get; set; } = default!;
        [Inject] ToastService              ToastService       { get; set; } = default!;
        private int VisibleColumnCount;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
            VisibleColumnCount =
await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        // ── Data ───────────────────────────────────────────────────────────────
        private List<RevenueRecognitionViewModel.RevenueRecognition> AllItems      = new();
        private List<RevenueRecognitionViewModel.RevenueRecognition> FilteredItems = new();

        private List<CompanyModel> Companies = new();

        private RevenueRecognitionViewModel.RevenueRecognition? SelectedItem;
        private bool canDelete = false;
        private bool isInitialized = false;
        private bool isLoading = false;

        // ── Filters ────────────────────────────────────────────────────────────
        private string searchText = "";
        private int PageSize = 10;
        private int CurrentPage = 1;
        private const int PageWindowSize = 2;

        private string selectedStatus = "";
        private string selectedMethod = "";
        private Guid? selectedCompany = null;

        private int TotalPages =>
            FilteredItems.Count == 0
                ? 1
                : (int)Math.Ceiling((double)FilteredItems.Count / PageSize);

        private List<RevenueRecognitionViewModel.RevenueRecognition> PagedItems =>
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

        private string SelectedMethodFilter
        {
            get => selectedMethod;
            set { selectedMethod = value; ApplyFilters(); }
        }

        private string SelectedCompanyFilter
        {
            get => selectedCompany?.ToString() ?? "";
            set
            {
                selectedCompany = Guid.TryParse(value, out var g) ? g : null;
                ApplyFilters();
            }
        }

        private void OnFilterChanged(ChangeEventArgs e) => ApplyFilters();

        // ── Lifecycle ──────────────────────────────────────────────────────────

        protected override void OnInitialized()
        {
            Companies = MasterDataService.GetAllCompanies();
            LoadData();
        }

        private void LoadData()
        {
            AllItems      = RecognitionService.GetAll();
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
            IEnumerable<RevenueRecognitionViewModel.RevenueRecognition> q = AllItems;

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                q = q.Where(r =>
                    r.RecognitionCode.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    r.RecognitionName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    (r.CustomerNameSnapshot ?? "").Contains(searchText, StringComparison.OrdinalIgnoreCase));
            }

            if (selectedCompany.HasValue)
                q = q.Where(r => r.CompanyId == selectedCompany.Value);

            if (!string.IsNullOrWhiteSpace(selectedStatus) &&
                Enum.TryParse<RecognitionStatusEnum>(selectedStatus, out var parsedStatus))
                q = q.Where(r => r.RecognitionStatus == parsedStatus);

            if (!string.IsNullOrWhiteSpace(selectedMethod) &&
                Enum.TryParse<RecognitionMethodEnum>(selectedMethod, out var parsedMethod))
                q = q.Where(r => r.RecognitionMethod == parsedMethod);

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

        // ── Selection ──────────────────────────────────────────────────────────
        private void SelectItem(RevenueRecognitionViewModel.RevenueRecognition item)
        {
            SelectedItem = item;
            canDelete    = !item.IsLocked &&
                           item.RecognitionStatus != RecognitionStatusEnum.FullyRecognized &&
                           item.RecognitionStatus != RecognitionStatusEnum.Closed;
        }

        // ── Actions ────────────────────────────────────────────────────────────
        private void LockConfirmed()
        {
            if (SelectedItem is null) return;
            RecognitionService.Lock(SelectedItem.RevenueRecognitionId);
            ToastService.ShowSuccess($"Recognition '{SelectedItem.RecognitionCode}' locked successfully.", "Locked");
            LoadData();
        }

        private void UnlockConfirmed()
        {
            if (SelectedItem is null) return;
            RecognitionService.Unlock(SelectedItem.RevenueRecognitionId);
            ToastService.ShowSuccess($"Recognition '{SelectedItem.RecognitionCode}' unlocked successfully.", "Unlocked");
            LoadData();
        }

        private void DeleteConfirmed()
        {
            if (SelectedItem is null) return;
            try
            {
                RecognitionService.Delete(SelectedItem.RevenueRecognitionId);
                ToastService.ShowSuccess("Recognition record deleted.", "Deleted");
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
            RecognitionService.ResetToSeed();
            searchText     = "";
            selectedStatus = "";
            selectedMethod = "";
            selectedCompany = null;
            LoadData();
            isLoading = false;
            await JS.InvokeVoidAsync("feather.replace");
            ToastService.ShowInfo("Recognition list refreshed.", "Refreshed");
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

        private string GetStatusBadgeClass(RecognitionStatusEnum status) => status switch
        {
            RecognitionStatusEnum.Draft               => "bg-secondary-transparent text-secondary",
            RecognitionStatusEnum.Ready               => "bg-info-transparent text-info",
            RecognitionStatusEnum.Scheduled           => "bg-primary-transparent text-primary",
            RecognitionStatusEnum.InProgress          => "bg-primary-transparent text-primary",
            RecognitionStatusEnum.PartiallyRecognized => "bg-warning-transparent text-warning",
            RecognitionStatusEnum.FullyRecognized     => "bg-success-transparent text-success",
            RecognitionStatusEnum.OnHold              => "bg-danger-transparent text-danger",
            RecognitionStatusEnum.Cancelled           => "bg-danger-transparent text-danger",
            RecognitionStatusEnum.Closed              => "bg-dark-transparent text-dark",
            _                                         => "bg-secondary-transparent text-secondary"
        };

        private string GetStatusDot(RecognitionStatusEnum status) => status switch
        {
            RecognitionStatusEnum.FullyRecognized     => "bg-success text-success",
            RecognitionStatusEnum.InProgress          => "bg-primary text-primary",
            RecognitionStatusEnum.Scheduled           => "bg-primary text-primary",
            RecognitionStatusEnum.PartiallyRecognized => "bg-warning text-warning",
            RecognitionStatusEnum.Ready               => "bg-info text-info",
            RecognitionStatusEnum.OnHold              => "bg-danger text-danger",
            RecognitionStatusEnum.Cancelled           => "bg-danger text-danger",
            RecognitionStatusEnum.Closed              => "bg-dark text-dark",
            _                                         => "bg-secondary text-secondary"
        };

        private string GetCompletionBadge(decimal pct) => pct switch
        {
            100m => "bg-success-transparent text-success",
            >= 50m => "bg-primary-transparent text-primary",
            > 0m => "bg-warning-transparent text-warning",
            _ => "bg-secondary-transparent text-secondary"
        };
    }
}
