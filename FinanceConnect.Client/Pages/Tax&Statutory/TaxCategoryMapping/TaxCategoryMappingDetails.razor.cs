using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.TaxCategoryMappingViewModel;

namespace FinanceConnect.Client.Pages.Tax_Statutory.TaxCategoryMapping
{
    public partial class TaxCategoryMappingDetails
    {
        [Parameter] public Guid Id { get; set; }

        [Inject] private TaxCategoryMappingService MappingService { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        // ── State ─────────────────────────────────────────────────────────────
        private TaxCategoryMappingListDto? Entry;
        private List<TaxCategoryMappingLineModel> Lines = new();
        private bool isInitialized = false;
        private bool showLockModal = false;
        private bool showLockError = false;
        private string lockReason = string.Empty;

        // ── Lifecycle ─────────────────────────────────────────────────────────
        protected override async Task OnParametersSetAsync()
        {
            isInitialized = false;
            Entry = await MappingService.GetByIdAsync(Id);

            // Load full line data from the service
            if (Entry != null)
            {
                var model = await MappingService.GetModelByIdAsync(Id);
                Lines = model?.Lines ?? new List<TaxCategoryMappingLineModel>();
            }

            isInitialized = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
            => await JS.InvokeVoidAsync("feather.replace");

        // ── Status badge helper (matches VendorPayment fc-status-badge pattern) ─
        private string GetStatusBadgeClass(MappingStatus s) => s switch
        {
            MappingStatus.Active => "fc-status-active",
            MappingStatus.Inactive => "fc-status-pending",
            MappingStatus.Archived => "fc-status-cancelled",
            _ => "fc-status-draft"
        };


        private async Task PrintPage() { await JS.InvokeVoidAsync("window.print"); }
        // ── Status actions ────────────────────────────────────────────────────
        private async Task OnActivate()
        {
            try
            {
                await MappingService.ActivateAsync(Id);
                await RefreshAsync();
                ToastService.ShowSuccess("Mapping activated.");
            }
            catch (Exception ex) { ToastService.ShowError(ex.Message); }
        }

        private async Task OnInactivate()
        {
            try
            {
                await MappingService.InactivateAsync(Id);
                await RefreshAsync();
                ToastService.ShowSuccess("Mapping inactivated.");
            }
            catch (Exception ex) { ToastService.ShowError(ex.Message); }
        }

        private async Task OnArchive()
        {
            try
            {
                await MappingService.ArchiveAsync(Id);
                await RefreshAsync();
                ToastService.ShowSuccess("Mapping archived.");
            }
            catch (Exception ex) { ToastService.ShowError(ex.Message); }
        }

        // ── Lock / Unlock ─────────────────────────────────────────────────────
        private async Task OnLock()
        {
            showLockError = false;
            if (string.IsNullOrWhiteSpace(lockReason))
            {
                showLockError = true;
                return;
            }
            try
            {
                await MappingService.LockAsync(Id, lockReason);
                showLockModal = false;
                lockReason = string.Empty;
                await RefreshAsync();
                ToastService.ShowSuccess("Mapping locked.");
            }
            catch (Exception ex) { ToastService.ShowError(ex.Message); }
        }

        private async Task OnUnlock()
        {
            try
            {
                await MappingService.UnlockAsync(Id);
                await RefreshAsync();
                ToastService.ShowSuccess("Mapping unlocked.");
            }
            catch (Exception ex) { ToastService.ShowError(ex.Message); }
        }

        // ── Refresh ───────────────────────────────────────────────────────────
        private async Task RefreshAsync()
        {
            Entry = await MappingService.GetByIdAsync(Id);
            if (Entry != null)
            {
                var model = await MappingService.GetModelByIdAsync(Id);
                Lines = model?.Lines ?? new List<TaxCategoryMappingLineModel>();
            }
            StateHasChanged();
            await JS.InvokeVoidAsync("feather.replace");
        }
    }
}
