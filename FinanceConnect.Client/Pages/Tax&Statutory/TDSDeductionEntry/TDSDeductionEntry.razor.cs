using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.TDSDeductionEntryViewModel;

namespace FinanceConnect.Client.Pages.Tax_Statutory.TDSDeductionEntry
{
    public partial class TDSDeductionEntry
    {
        [Inject] private TDSDeductionEntryService DeductionService { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;

        private List<TDSDeductionEntryListDto> AllEntries { get; set; } = new();
        private List<TDSDeductionEntryListDto> FilteredEntries { get; set; } = new();
        private List<TDSDeductionEntryListDto> PagedVersions { get; set; } = new();
        private TDSDeductionEntryListDto? SelectedEntry { get; set; }

        private TDSDeductionEntryListDto? actionEntry;
        private bool showReverseModal = false;
        private bool showDeleteModal = false;
        private bool showReverseError = false;
        private string reverseReason = string.Empty;
        private bool isInitialized = false;
        private bool isLoading = false;

        private string searchText { get; set; } = string.Empty;
        private string SelectedStatus { get; set; } = string.Empty;
        private string SelectedSection { get; set; } = string.Empty;
        private string SelectedSettlement { get; set; } = string.Empty;
        private int VisibleColumnCount;
        private int CurrentPage { get; set; } = 1;
        private int PageSize { get; set; } = 10;

        private int TotalPages => FilteredEntries.Count == 0
            ? 1
            : (int)Math.Ceiling(FilteredEntries.Count / (double)PageSize);

        private IEnumerable<int> VisiblePages
        {
            get
            {
                int start = Math.Max(1, CurrentPage - 2);
                int end = Math.Min(TotalPages, start + 4);
                return Enumerable.Range(start, end - start + 1);
            }
        }

        protected override async Task OnInitializedAsync()
        {
            AllEntries = await DeductionService.GetAllAsync();
            ApplyFilters();
            isInitialized = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
            VisibleColumnCount =
await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        private async Task OnRefreshAsync()
        {
            searchText = SelectedStatus = SelectedSection = SelectedSettlement = string.Empty;
            CurrentPage = 1;
            AllEntries = await DeductionService.GetAllAsync();
            ApplyFilters();
            await JS.InvokeVoidAsync("feather.replace");
        }

        private async Task ReloadAsync()
        {
            AllEntries = await DeductionService.GetAllAsync();
            ApplyFilters();
            await JS.InvokeVoidAsync("feather.replace");
        }

        private void OnSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString() ?? "";
            CurrentPage = 1;
            ApplyFilters();
        }

        private void OnFilterChanged(ChangeEventArgs e)
        {
            CurrentPage = 1;
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var q = AllEntries.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var t = searchText.Trim().ToLowerInvariant();
                q = q.Where(v =>
                    (v.DeductionNumber != null && v.DeductionNumber.ToLowerInvariant().Contains(t)) ||
                    (v.VendorNameSnapshot != null && v.VendorNameSnapshot.ToLowerInvariant().Contains(t)) ||
                    (v.VendorCodeSnapshot != null && v.VendorCodeSnapshot.ToLowerInvariant().Contains(t)) ||
                    (v.SectionCodeSnapshot != null && v.SectionCodeSnapshot.ToLowerInvariant().Contains(t)) ||
                    (v.SourceDocumentNumberSnapshot != null && v.SourceDocumentNumberSnapshot.ToLowerInvariant().Contains(t)));
            }

            if (!string.IsNullOrEmpty(SelectedStatus) &&
                int.TryParse(SelectedStatus, out var si) &&
                Enum.IsDefined(typeof(DeductionStatus), si))
                q = q.Where(v => v.Status == (DeductionStatus)si);

            if (!string.IsNullOrEmpty(SelectedSection))
                q = q.Where(v => v.SectionCodeSnapshot == SelectedSection);

            if (!string.IsNullOrEmpty(SelectedSettlement) &&
                int.TryParse(SelectedSettlement, out var ss) &&
                Enum.IsDefined(typeof(SettlementStatus), ss))
                q = q.Where(v => v.SettlementStatus == (SettlementStatus)ss);

            FilteredEntries = q.OrderByDescending(v => v.DeductionDate).ToList();
            UpdatePagedList();
        }

        private void UpdatePagedList()
            => PagedVersions = FilteredEntries
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

        private void OnPageSizeChange(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out var s))
            { PageSize = s; CurrentPage = 1; UpdatePagedList(); }
        }

        private void PreviousPage() { if (CurrentPage > 1) { CurrentPage--; UpdatePagedList(); } }
        private void NextPage() { if (CurrentPage < TotalPages) { CurrentPage++; UpdatePagedList(); } }
        private void GoToPage(int p) { CurrentPage = p; UpdatePagedList(); }

        // ── Open offcanvas / mobile modal ────────────────────────────────────
        private void OpenViewModal(TDSDeductionEntryListDto entry) => SelectedEntry = entry;
        private void OpenRowDetails(TDSDeductionEntryListDto v) => SelectedEntry = v;
        private void DeletePopupOpen(TDSDeductionEntryListDto v) { SelectedEntry = v; showDeleteModal = true; }

        // ── Settlement dot CSS ────────────────────────────────────────────────
        public static string GetSettlementStatusLabel(SettlementStatus s) => s switch
        {
            SettlementStatus.NotSettled => "Not Settled",
            SettlementStatus.PartiallySettled => "Partially Settled",
            SettlementStatus.FullySettled => "Fully Settled",
            _ => s.ToString()
        };

        public static string GetSettlementStatusClass(SettlementStatus s) => s switch
        {
            SettlementStatus.NotSettled => "bg-secondary-transparent text-secondary",
            SettlementStatus.PartiallySettled => "bg-warning-transparent text-dark",
            SettlementStatus.FullySettled => "bg-success-transparent",
            _ => "bg-light text-dark"
        };

        public static string GetStatusLabel(DeductionStatus s) => s switch
        {
            DeductionStatus.Draft => "Draft",
            DeductionStatus.Posted => "Posted",
            DeductionStatus.PartiallySettled => "Partially Settled",
            DeductionStatus.Settled => "Settled",
            DeductionStatus.Reversed => "Reversed",
            DeductionStatus.Cancelled => "Cancelled",
            _ => s.ToString()
        };
        public static string GetStatusPillResClass(DeductionStatus s) => s switch
        {
            DeductionStatus.Draft => "draft1",
            DeductionStatus.Posted => "posted1",
            DeductionStatus.PartiallySettled => "partial1",
            DeductionStatus.Settled => "settled1",
            DeductionStatus.Reversed => "reversed1",
            DeductionStatus.Cancelled => "cancelled1",
            _ => s.ToString()
        };
        private string GetBadgeClass(SourceDocumentType type) => type switch
        {
            SourceDocumentType.VendorBill => "bg-primary-transparent",
            SourceDocumentType.VendorPayment => "bg-success-transparent",
            SourceDocumentType.PaymentAllocation => "bg-warning-transparent",
            SourceDocumentType.ManualAdjustment => "bg-danger-transparent",
            _ => "bg-secondary-transparent"
        };
        public static string GetStatusPillClass(DeductionStatus s) => s switch
        {
            DeductionStatus.Draft => "draft",
            DeductionStatus.Posted => "posted",
            DeductionStatus.PartiallySettled => "partial",
            DeductionStatus.Settled => "settled",
            DeductionStatus.Reversed => "reversed",
            DeductionStatus.Cancelled => "cancelled",
            _ => ""
        };

        // ── Actions ───────────────────────────────────────────────────────────
        private async Task ConfirmCancel(Guid id)
        {
            try
            {
                await DeductionService.CancelAsync(id);
                showDeleteModal = false;
                SelectedEntry = null;
                await ReloadAsync();
                ToastService.ShowSuccess("Deduction entry cancelled.");
            }
            catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message); }
        }

        private void OpenReverseModal(TDSDeductionEntryListDto v)
        {
            actionEntry = v;
            reverseReason = string.Empty;
            showReverseError = false;
            showReverseModal = true;
        }

        private async Task ConfirmReverse()
        {
            showReverseError = false;
            if (string.IsNullOrWhiteSpace(reverseReason)) { showReverseError = true; return; }
            try
            {
                await DeductionService.ReverseAsync(actionEntry!.TDSDeductionEntryId, reverseReason);
                showReverseModal = false;
                reverseReason = string.Empty;
                actionEntry = null;
                await ReloadAsync();
                ToastService.ShowSuccess("Deduction entry reversed.");
            }
            catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message); }
        }

        private async Task OnMarkReconciled(Guid id)
        {
            try
            {
                await DeductionService.MarkReconciledAsync(id);
                await ReloadAsync();
                ToastService.ShowSuccess("Deduction entry marked as fully settled.");
            }
            catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message); }
        }
    }
}
