using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.AssetTransformViewModel;

namespace FinanceConnect.Client.Pages.FixedAssets.AssetTransfer
{
    public partial class AssetTransfer : ComponentBase
    {
        [Inject] private AssetTransferService TransferService { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;

        // ── State ─────────────────────────────────────────────────────────────
        private List<AssetTransferListDto> AllTransfers { get; set; } = new();
        private List<AssetTransferListDto> FilteredTransfers { get; set; } = new();
        private List<AssetTransferListDto> PagedTransfers { get; set; } = new();
        private AssetTransferListDto? SelectedTransfer { get; set; }

        // ── Workflow action state ─────────────────────────────────────────────
        private AssetTransferListDto? actionTransfer = null;
        private bool showRejectModal = false;
        private bool showReverseModal = false;
        private bool showRejectError = false;
        private bool showReverseError = false;
        private string rejectReason = string.Empty;
        private string reversalReason = string.Empty;

        // ── Filters ───────────────────────────────────────────────────────────
        private string searchText { get; set; } = string.Empty;
        private string SelectedStatus { get; set; } = string.Empty;
        private string SelectedType { get; set; } = string.Empty;

        // ── Pagination ────────────────────────────────────────────────────────
        private int CurrentPage { get; set; } = 1;
        private int PageSize { get; set; } = 10;

        private int TotalPages => FilteredTransfers.Count == 0
            ? 1 : (int)Math.Ceiling(FilteredTransfers.Count / (double)PageSize);
        private int StartPage => Math.Max(1, CurrentPage - 2);
        private int EndPage => Math.Min(TotalPages, StartPage + 4);

        // ── Lifecycle ─────────────────────────────────────────────────────────
        protected override async Task OnInitializedAsync()
        {
            AllTransfers = await TransferService.GetAllAsync();
            ApplyFilters();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
        }

        // ── Refresh ───────────────────────────────────────────────────────────
        private async Task OnRefreshAsync()
        {
            searchText = string.Empty;
            SelectedStatus = string.Empty;
            SelectedType = string.Empty;
            CurrentPage = 1;
            AllTransfers = await TransferService.GetAllAsync();
            ApplyFilters();
            await JS.InvokeVoidAsync("feather.replace");
        }

        // ── Reload list ───────────────────────────────────────────────────────
        private async Task ReloadAsync()
        {
            AllTransfers = await TransferService.GetAllAsync();
            ApplyFilters();
            await JS.InvokeVoidAsync("feather.replace");
        }

        // ── Search & Filter ───────────────────────────────────────────────────
        private void OnSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString() ?? string.Empty;
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
            var query = AllTransfers.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var term = searchText.Trim().ToLowerInvariant();
                query = query.Where(t =>
                    (t.TransferNumber != null && t.TransferNumber.ToLowerInvariant().Contains(term)) ||
                    (t.AssetNameSnapshot != null && t.AssetNameSnapshot.ToLowerInvariant().Contains(term)) ||
                    (t.AssetNumberSnapshot != null && t.AssetNumberSnapshot.ToLowerInvariant().Contains(term)) ||
                    (t.FromCustodianName != null && t.FromCustodianName.ToLowerInvariant().Contains(term)) ||
                    (t.ToCustodianName != null && t.ToCustodianName.ToLowerInvariant().Contains(term))
                );
            }

            if (!string.IsNullOrEmpty(SelectedStatus) &&
                int.TryParse(SelectedStatus, out var statusInt) &&
                Enum.IsDefined(typeof(TransferStatus), statusInt))
                query = query.Where(t => t.TransferStatus == (TransferStatus)statusInt);

            if (!string.IsNullOrEmpty(SelectedType) &&
                int.TryParse(SelectedType, out var typeInt) &&
                Enum.IsDefined(typeof(TransferType), typeInt))
                query = query.Where(t => t.TransferType == (TransferType)typeInt);

            FilteredTransfers = query.ToList();
            UpdatePagedList();
        }

        // ── Pagination ────────────────────────────────────────────────────────
        private void UpdatePagedList()
            => PagedTransfers = FilteredTransfers
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

        private void OnPageSizeChange(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out var size))
            { PageSize = size; CurrentPage = 1; UpdatePagedList(); }
        }

        private void PreviousPage() { if (CurrentPage > 1) { CurrentPage--; UpdatePagedList(); } }
        private void NextPage() { if (CurrentPage < TotalPages) { CurrentPage++; UpdatePagedList(); } }
        private void GoToPage(int p) { CurrentPage = p; UpdatePagedList(); }

        // ── Row / Delete ──────────────────────────────────────────────────────
        private void OpenRowDetails(AssetTransferListDto t) => SelectedTransfer = t;
        private void DeletePopupOpen(AssetTransferListDto t) => SelectedTransfer = t;

        private async Task ConfirmDelete(Guid id)
        {
            try
            {
                await TransferService.DeleteAsync(id);
                AllTransfers.RemoveAll(t => t.AssetTransferId == id);
                ApplyFilters();
                ToastService.ShowSuccess("Transfer deleted successfully.");
            }
            catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message); }
        }

        // ── Workflow: direct actions ──────────────────────────────────────────
        private async Task OnSubmit(Guid id)
        {
            try
            {
                await TransferService.SubmitAsync(id);
                await ReloadAsync();
                ToastService.ShowSuccess("Transfer submitted.");
            }
            catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message); }
        }

        private async Task OnApprove(Guid id)
        {
            try
            {
                await TransferService.ApproveAsync(id);
                await ReloadAsync();
                ToastService.ShowSuccess("Transfer approved.");
            }
            catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message); }
        }

        private async Task OnCancel(Guid id)
        {
            try
            {
                await TransferService.CancelAsync(id);
                await ReloadAsync();
                ToastService.ShowSuccess("Transfer cancelled.");
            }
            catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message); }
        }

        private async Task OnMarkInTransit(Guid id)
        {
            try
            {
                await TransferService.MarkInTransitAsync(id);
                await ReloadAsync();
                ToastService.ShowSuccess("Marked as In Transit.");
            }
            catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message); }
        }

        private async Task OnConfirmReceipt(Guid id)
        {
            try
            {
                await TransferService.ConfirmReceiptAsync(id);
                await ReloadAsync();
                ToastService.ShowSuccess("Receipt confirmed.");
            }
            catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message); }
        }

        private async Task OnPost(Guid id)
        {
            try
            {
                await TransferService.PostAsync(id);
                await ReloadAsync();
                ToastService.ShowSuccess("Transfer posted.");
            }
            catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message); }
        }

        // ── Workflow: modal actions (Reject / Reverse) ────────────────────────
        private void OpenRejectModal(AssetTransferListDto t)
        {
            actionTransfer = t;
            rejectReason = string.Empty;
            showRejectError = false;
            showRejectModal = true;
        }

        private async Task ConfirmReject()
        {
            showRejectError = false;
            if (string.IsNullOrWhiteSpace(rejectReason)) { showRejectError = true; return; }
            try
            {
                await TransferService.RejectAsync(actionTransfer!.AssetTransferId, rejectReason);
                showRejectModal = false;
                rejectReason = string.Empty;
                actionTransfer = null;
                await ReloadAsync();
                ToastService.ShowSuccess("Transfer rejected.");
            }
            catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message); }
        }

        private void OpenReverseModal(AssetTransferListDto t)
        {
            actionTransfer = t;
            reversalReason = string.Empty;
            showReverseError = false;
            showReverseModal = true;
        }

        private async Task ConfirmReverse()
        {
            showReverseError = false;
            if (string.IsNullOrWhiteSpace(reversalReason)) { showReverseError = true; return; }
            try
            {
                await TransferService.ReverseAsync(actionTransfer!.AssetTransferId, reversalReason);
                showReverseModal = false;
                reversalReason = string.Empty;
                actionTransfer = null;
                await ReloadAsync();
                ToastService.ShowSuccess("Transfer reversed.");
            }
            catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message); }
        }

        // ── Badge / Label helpers ─────────────────────────────────────────────
        private static string GetStatusLabel(TransferStatus s) => s switch
        {
            TransferStatus.Draft => "Draft",
            TransferStatus.Submitted => "Submitted",
            TransferStatus.Approved => "Approved",
            TransferStatus.Rejected => "Rejected",
            TransferStatus.Cancelled => "Cancelled",
            TransferStatus.InTransit => "In Transit",
            TransferStatus.Received => "Received",
            TransferStatus.Posted => "Posted",
            TransferStatus.Reversed => "Reversed",
            TransferStatus.Closed => "Closed",
            _ => "Unknown"
        };

        private static string GetStatusDotBadge(TransferStatus s) => s switch
        {
            TransferStatus.Draft => "bg-warning",
            TransferStatus.Submitted => "bg-info",
            TransferStatus.Approved => "bg-primary",
            TransferStatus.Rejected => "bg-danger",
            TransferStatus.Cancelled => "bg-secondary",
            TransferStatus.InTransit => "bg-warning",
            TransferStatus.Received => "bg-info",
            TransferStatus.Posted => "bg-success",
            TransferStatus.Reversed => "bg-secondary",
            TransferStatus.Closed => "bg-success",
            _ => "bg-secondary"
        };

        private static string GetStatusBadgeClass(TransferStatus s) => s switch
        {
            TransferStatus.Draft => "bg-warning-transparent",
            TransferStatus.Submitted => "bg-info-transparent",
            TransferStatus.Approved => "bg-primary-transparent",
            TransferStatus.Rejected => "bg-danger-transparent",
            TransferStatus.Cancelled => "bg-secondary-transparent",
            TransferStatus.InTransit => "bg-warning-transparent",
            TransferStatus.Received => "bg-info-transparent",
            TransferStatus.Posted => "bg-success-transparent",
            TransferStatus.Reversed => "bg-secondary-transparent",
            TransferStatus.Closed => "bg-success-transparent",
            _ => "bg-light"
        };

        private static string GetTypeLabel(TransferType? t) => t switch
        {
            TransferType.CustodianChange => "Custodian Change",
            TransferType.LocationChange => "Location Change",
            TransferType.BranchChange => "Branch Change",
            TransferType.FullReassignment => "Full Reassignment",
            _ => "—"
        };

        private static string GetTypeBadgeClass(TransferType? t) => t switch
        {
            TransferType.CustodianChange => "bg-info-transparent",
            TransferType.LocationChange => "bg-warning-transparent",
            TransferType.BranchChange => "bg-primary-transparent",
            TransferType.FullReassignment => "bg-purple-transparent text-purple",
            _ => "bg-light"
        };
    }
}
