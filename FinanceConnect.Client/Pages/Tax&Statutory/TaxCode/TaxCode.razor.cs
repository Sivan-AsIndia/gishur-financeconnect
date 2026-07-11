using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.TaxCodeViewModel;

namespace FinanceConnect.Client.Pages.Tax_Statutory.TaxCode
{
    public partial class TaxCode
    {
            [Inject] private TaxCodeService TaxCodeService { get; set; } = default!;
            [Inject] private ToastService ToastService { get; set; } = default!;

            // ── State ─────────────────────────────────────────────────────────────
            private List<TaxCodeListDto> AllCodes { get; set; } = new();
            private List<TaxCodeListDto> FilteredCodes { get; set; } = new();
            private List<TaxCodeListDto> PagedCodes { get; set; } = new();
            private TaxCodeListDto? SelectedCode { get; set; }

            // ── Lock modal state ──────────────────────────────────────────────────
            private TaxCodeListDto? actionCode = null;
            private bool showLockModal = false;
            private bool showLockError = false;
            private string lockReason = string.Empty;

            // ── Filters ───────────────────────────────────────────────────────────
            private string searchText { get; set; } = string.Empty;
            private string SelectedStatus { get; set; } = string.Empty;
            private string SelectedType { get; set; } = string.Empty;

            // ── Pagination ────────────────────────────────────────────────────────
            private int CurrentPage { get; set; } = 1;
            private int PageSize { get; set; } = 10;

            private int TotalPages => FilteredCodes.Count == 0
                ? 1 : (int)Math.Ceiling(FilteredCodes.Count / (double)PageSize);
            private int StartPage => Math.Max(1, CurrentPage - 2);
            private int EndPage => Math.Min(TotalPages, StartPage + 4);

            // ── Lifecycle ─────────────────────────────────────────────────────────
            protected override async Task OnInitializedAsync()
            {
                AllCodes = await TaxCodeService.GetAllAsync();
                ApplyFilters();
            }

            protected override async Task OnAfterRenderAsync(bool firstRender)
            {
                await JS.InvokeVoidAsync("feather.replace");
                await JS.InvokeVoidAsync("initTooltips");
            }

            [Inject] private IJSRuntime JS { get; set; } = default!;

            // ── Refresh ───────────────────────────────────────────────────────────
            private async Task OnRefreshAsync()
            {
                searchText = string.Empty;
                SelectedStatus = string.Empty;
                SelectedType = string.Empty;
                CurrentPage = 1;
                AllCodes = await TaxCodeService.GetAllAsync();
                ApplyFilters();
                await JS.InvokeVoidAsync("feather.replace");
            }

            private async Task ReloadAsync()
            {
                AllCodes = await TaxCodeService.GetAllAsync();
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
                var query = AllCodes.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    var term = searchText.Trim().ToLowerInvariant();
                    query = query.Where(t =>
                        (t.Code != null && t.Code.ToLowerInvariant().Contains(term)) ||
                        (t.TaxName != null && t.TaxName.ToLowerInvariant().Contains(term)) ||
                        (t.TDSSectionCode != null && t.TDSSectionCode.ToLowerInvariant().Contains(term)) ||
                        (t.TCSSectionCode != null && t.TCSSectionCode.ToLowerInvariant().Contains(term)) ||
                        (t.Description != null && t.Description.ToLowerInvariant().Contains(term))
                    );
                }

                if (!string.IsNullOrEmpty(SelectedStatus) &&
                    int.TryParse(SelectedStatus, out var statusInt) &&
                    Enum.IsDefined(typeof(TaxCodeStatus), statusInt))
                    query = query.Where(t => t.Status == (TaxCodeStatus)statusInt);

                if (!string.IsNullOrEmpty(SelectedType) &&
                    int.TryParse(SelectedType, out var typeInt) &&
                    Enum.IsDefined(typeof(TaxType), typeInt))
                    query = query.Where(t => t.Type == (TaxType)typeInt);

                FilteredCodes = query.ToList();
                UpdatePagedList();
            }

            // ── Pagination ────────────────────────────────────────────────────────
            private void UpdatePagedList()
                => PagedCodes = FilteredCodes
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
            private void OpenRowDetails(TaxCodeListDto t) => SelectedCode = t;
            private void DeletePopupOpen(TaxCodeListDto t) => SelectedCode = t;

            private async Task ConfirmDelete(Guid id)
            {
                try
                {
                    await TaxCodeService.DeleteAsync(id);
                    AllCodes.RemoveAll(t => t.TaxCodeId == id);
                    ApplyFilters();
                    ToastService.ShowSuccess("Tax code deleted successfully.");
                }
                catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message); }
            }

            // ── Status Actions ────────────────────────────────────────────────────
            private async Task OnActivate(Guid id)
            {
                try
                {
                    await TaxCodeService.ActivateAsync(id);
                    await ReloadAsync();
                    ToastService.ShowSuccess("Tax code activated.");
                }
                catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message); }
            }

            private async Task OnInactivate(Guid id)
            {
                try
                {
                    await TaxCodeService.InactivateAsync(id);
                    await ReloadAsync();
                    ToastService.ShowSuccess("Tax code inactivated.");
                }
                catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message); }
            }

            private async Task OnArchive(Guid id)
            {
                try
                {
                    await TaxCodeService.ArchiveAsync(id);
                    await ReloadAsync();
                    ToastService.ShowSuccess("Tax code archived.");
                }
                catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message); }
            }

            private async Task OnUnlock(Guid id)
            {
                try
                {
                    await TaxCodeService.UnlockAsync(id);
                    await ReloadAsync();
                    ToastService.ShowSuccess("Tax code unlocked.");
                }
                catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message); }
            }

            // ── Lock Modal ────────────────────────────────────────────────────────
            private void OpenLockModal(TaxCodeListDto t)
            {
                actionCode = t;
                lockReason = string.Empty;
                showLockError = false;
                showLockModal = true;
            }

            private async Task ConfirmLock()
            {
                showLockError = false;
                if (string.IsNullOrWhiteSpace(lockReason)) { showLockError = true; return; }
                try
                {
                    await TaxCodeService.LockAsync(actionCode!.TaxCodeId, lockReason);
                    showLockModal = false;
                    lockReason = string.Empty;
                    actionCode = null;
                    await ReloadAsync();
                    ToastService.ShowSuccess("Tax code locked.");
                }
                catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message); }
            }

            // ── Badge / Label Helpers ─────────────────────────────────────────────
            private static string GetTypeLabel(TaxType? t) => t switch
            {
                TaxType.GST => "GST",
                TaxType.TDS => "TDS",
                TaxType.TCS => "TCS",
                TaxType.Other => "Other",
                _ => "—"
            };

            private static string GetTypeBadgeClass(TaxType? t) => t switch
            {
                TaxType.GST => "bg-primary-transparent",
                TaxType.TDS => "bg-warning-transparent",
                TaxType.TCS => "bg-info-transparent",
                TaxType.Other => "bg-secondary-transparent",
                _ => "bg-light"
            };

            private static string GetDirectionLabel(TaxDirection? d) => d switch
            {
                TaxDirection.Input => "Input",
                TaxDirection.Output => "Output",
                TaxDirection.WithholdingPayable => "Withholding",
                TaxDirection.Other => "Other",
                _ => "—"
            };

            private static string GetDirectionBadgeClass(TaxDirection? d) => d switch
            {
                TaxDirection.Input => "bg-success-transparent",
                TaxDirection.Output => "bg-danger-transparent",
                TaxDirection.WithholdingPayable => "bg-purple-transparent text-purple",
                _ => "bg-light"
            };

            private static string GetStatusLabel(TaxCodeStatus s) => s switch
            {
                TaxCodeStatus.Active => "Active",
                TaxCodeStatus.Inactive => "Inactive",
                TaxCodeStatus.Archived => "Archived",
                _ => "Unknown"
            };

            private static string GetStatusDotClass(TaxCodeStatus s) => s switch
            {
                TaxCodeStatus.Active => "bg-success",
                TaxCodeStatus.Inactive => "bg-warning",
                TaxCodeStatus.Archived => "bg-secondary",
                _ => "bg-secondary"
            };

            private static string GetStatusBadgeClass(TaxCodeStatus s) => s switch
            {
                TaxCodeStatus.Active => "bg-success-transparent",
                TaxCodeStatus.Inactive => "bg-warning-transparent",
                TaxCodeStatus.Archived => "bg-secondary-transparent text-secondary",
                _ => "bg-light"
            };
        }
    }
