using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.TaxRateVersionViewModel;

namespace FinanceConnect.Client.Pages.Tax_Statutory.TaxRateVersion
{
    public partial class TaxRateVersion
    {
        [Inject] private TaxRateVersionService RateVersionService { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;

        private List<TaxRateVersionListDto> AllVersions { get; set; } = new();
        private List<TaxRateVersionListDto> FilteredVersions { get; set; } = new();
        private List<TaxRateVersionListDto> PagedVersions { get; set; } = new();
        private TaxRateVersionListDto? SelectedVersion { get; set; }

        private TaxRateVersionListDto? actionVersion = null;
        private bool showRejectModal = false;
        private bool showLockModal = false;
        private bool showRejectError = false;
        private bool showLockError = false;
        private string rejectReason = string.Empty;
        private string lockReason = string.Empty;

        private string searchText { get; set; } = string.Empty;
        private string SelectedStatus { get; set; } = string.Empty;
        private string SelectedTaxType { get; set; } = string.Empty;

        private int CurrentPage { get; set; } = 1;
        private int PageSize { get; set; } = 10;
        private int TotalPages => FilteredVersions.Count == 0 ? 1 : (int)Math.Ceiling(FilteredVersions.Count / (double)PageSize);
        private int StartPage => Math.Max(1, CurrentPage - 2);
        private int EndPage => Math.Min(TotalPages, StartPage + 4);

        protected override async Task OnInitializedAsync()
        {
            AllVersions = await RateVersionService.GetAllAsync();
            ApplyFilters();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
        }

        private async Task OnRefreshAsync()
        {
            searchText = SelectedStatus = SelectedTaxType = string.Empty;
            CurrentPage = 1;
            AllVersions = await RateVersionService.GetAllAsync();
            ApplyFilters();
            await JS.InvokeVoidAsync("feather.replace");
        }

        private async Task ReloadAsync()
        {
            AllVersions = await RateVersionService.GetAllAsync();
            ApplyFilters();
            await JS.InvokeVoidAsync("feather.replace");
        }

        private void OnSearch(ChangeEventArgs e) { searchText = e.Value?.ToString() ?? ""; CurrentPage = 1; ApplyFilters(); }
        private void OnFilterChanged(ChangeEventArgs e) { CurrentPage = 1; ApplyFilters(); }

        private void ApplyFilters()
        {
            var q = AllVersions.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var t = searchText.Trim().ToLowerInvariant();
                q = q.Where(v =>
                    (v.TaxCodeSnapshot != null && v.TaxCodeSnapshot.ToLowerInvariant().Contains(t)) ||
                    (v.TaxCodeNameSnapshot != null && v.TaxCodeNameSnapshot.ToLowerInvariant().Contains(t)) ||
                    (v.LegalReferenceNumber != null && v.LegalReferenceNumber.ToLowerInvariant().Contains(t)));
            }

            if (!string.IsNullOrEmpty(SelectedStatus) &&
                int.TryParse(SelectedStatus, out var si) &&
                Enum.IsDefined(typeof(VersionStatus), si))
                q = q.Where(v => v.Status == (VersionStatus)si);

            if (!string.IsNullOrEmpty(SelectedTaxType))
                q = q.Where(v => v.TaxTypeSnapshot == SelectedTaxType);

            FilteredVersions = q.OrderByDescending(v => v.EffectiveFrom).ToList();
            UpdatePagedList();
        }

        private void UpdatePagedList()
            => PagedVersions = FilteredVersions.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

        private void OnPageSizeChange(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out var s)) { PageSize = s; CurrentPage = 1; UpdatePagedList(); }
        }
        private void PreviousPage() { if (CurrentPage > 1) { CurrentPage--; UpdatePagedList(); } }
        private void NextPage() { if (CurrentPage < TotalPages) { CurrentPage++; UpdatePagedList(); } }
        private void GoToPage(int p) { CurrentPage = p; UpdatePagedList(); }

        private void OpenRowDetails(TaxRateVersionListDto v) => SelectedVersion = v;
        private void DeletePopupOpen(TaxRateVersionListDto v) => SelectedVersion = v;

        private async Task ConfirmDelete(Guid id)
        {
            try { await RateVersionService.DeleteAsync(id); AllVersions.RemoveAll(v => v.TaxRateVersionId == id); ApplyFilters(); ToastService.ShowSuccess("Rate version deleted."); }
            catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message); }
        }

        private async Task OnSubmit(Guid id) { try { await RateVersionService.SubmitAsync(id); await ReloadAsync(); ToastService.ShowSuccess("Rate version submitted."); } catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message); } }
        private async Task OnApprove(Guid id) { try { await RateVersionService.ApproveAsync(id); await ReloadAsync(); ToastService.ShowSuccess("Rate version approved."); } catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message); } }
        private async Task OnActivate(Guid id) { try { await RateVersionService.ActivateAsync(id); await ReloadAsync(); ToastService.ShowSuccess("Rate version activated."); } catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message); } }
        private async Task OnRetire(Guid id) { try { await RateVersionService.RetireAsync(id); await ReloadAsync(); ToastService.ShowSuccess("Rate version retired."); } catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message); } }
        private async Task OnCancel(Guid id) { try { await RateVersionService.CancelAsync(id); await ReloadAsync(); ToastService.ShowSuccess("Rate version cancelled."); } catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message); } }
        private async Task OnUnlock(Guid id) { try { await RateVersionService.UnlockAsync(id); await ReloadAsync(); ToastService.ShowSuccess("Rate version unlocked."); } catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message); } }

        private void OpenRejectModal(TaxRateVersionListDto v) { actionVersion = v; rejectReason = string.Empty; showRejectError = false; showRejectModal = true; }
        private async Task ConfirmReject()
        {
            showRejectError = false;
            if (string.IsNullOrWhiteSpace(rejectReason)) { showRejectError = true; return; }
            try { await RateVersionService.RejectAsync(actionVersion!.TaxRateVersionId, rejectReason); showRejectModal = false; rejectReason = string.Empty; actionVersion = null; await ReloadAsync(); ToastService.ShowSuccess("Rate version rejected."); }
            catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message); }
        }

        private void OpenLockModal(TaxRateVersionListDto v) { actionVersion = v; lockReason = string.Empty; showLockError = false; showLockModal = true; }
        private async Task ConfirmLock()
        {
            showLockError = false;
            if (string.IsNullOrWhiteSpace(lockReason)) { showLockError = true; return; }
            try { await RateVersionService.LockAsync(actionVersion!.TaxRateVersionId, lockReason); showLockModal = false; lockReason = string.Empty; actionVersion = null; await ReloadAsync(); ToastService.ShowSuccess("Rate version locked."); }
            catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message); }
        }

        internal static string GetStatusLabel(VersionStatus s) => s switch
        {
            VersionStatus.Draft => "Draft",
            VersionStatus.Submitted => "Submitted",
            VersionStatus.Approved => "Approved",
            VersionStatus.Active => "Active",
            VersionStatus.Retired => "Retired",
            VersionStatus.Superseded => "Superseded",
            VersionStatus.Cancelled => "Cancelled",
            _ => "Unknown"
        };

        internal static string GetStatusPillClass(VersionStatus s) => s switch
        {
            VersionStatus.Draft => "draft",
            VersionStatus.Submitted => "submitted",
            VersionStatus.Approved => "approved",
            VersionStatus.Active => "active",
            VersionStatus.Retired => "retired",
            VersionStatus.Superseded => "superseded",
            VersionStatus.Cancelled => "cancelled",
            _ => ""
        };

        internal static string GetSourceLabel(RateSourceType? s) => s switch
        {
            RateSourceType.GovernmentNotification => "Govt Notification",
            RateSourceType.InternalPolicy => "Internal Policy",
            RateSourceType.Migration => "Migration",
            RateSourceType.Correction => "Correction",
            _ => "—"
        };

        internal static string GetSourceBadgeClass(RateSourceType? s) => s switch
        {
            RateSourceType.GovernmentNotification => "bg-primary-transparent",
            RateSourceType.InternalPolicy => "bg-info-transparent",
            RateSourceType.Migration => "bg-warning-transparent",
            RateSourceType.Correction => "bg-danger-transparent",
            _ => "bg-light"
        };
    }
}
