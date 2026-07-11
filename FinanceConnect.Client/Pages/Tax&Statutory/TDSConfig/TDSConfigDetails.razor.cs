using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.TdsConfigViewModel;

namespace FinanceConnect.Client.Pages.Tax_Statutory.TDSConfig
{
    public partial class TDSConfigDetails
    {
        [Parameter] public Guid Id { get; set; }

        [Inject] private TDSConfigService ConfigService { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        private TDSConfigListDto? Entry;
        private bool isInitialized = false;
        private bool showLockModal = false;
        private bool showLockError = false;
        private string lockReason = string.Empty;

        protected override async Task OnParametersSetAsync()
        {
            isInitialized = false;
            Entry = await ConfigService.GetByIdAsync(Id);
            isInitialized = true;
        }
        private async Task PrintPage() { await JS.InvokeVoidAsync("window.print"); }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                await JS.InvokeVoidAsync("feather.replace");
        }
        // ── fc-status-badge helper ────────────────────────────────────────────
        private string GetStatusBadgeClass(ConfigStatus s) => s switch
        {
            ConfigStatus.Active => "fc-status-active",
            ConfigStatus.Inactive => "fc-status-pending",
            ConfigStatus.Archived => "fc-status-cancelled",
            _ => "fc-status-draft"
        };

        // ── Status actions ────────────────────────────────────────────────────
        private async Task OnActivate()
        {
            try
            {
                await ConfigService.ActivateAsync(Id);
                await RefreshAsync();
                ToastService.ShowSuccess("Config activated.");
            }
            catch (Exception ex) { ToastService.ShowError(ex.Message); }
        }

        private async Task OnInactivate()
        {
            try
            {
                await ConfigService.InactivateAsync(Id);
                await RefreshAsync();
                ToastService.ShowSuccess("Config inactivated.");
            }
            catch (Exception ex) { ToastService.ShowError(ex.Message); }
        }

        private async Task OnArchive()
        {
            try
            {
                await ConfigService.ArchiveAsync(Id);
                await RefreshAsync();
                ToastService.ShowSuccess("Config archived.");
            }
            catch (Exception ex) { ToastService.ShowError(ex.Message); }
        }

        private async Task OnLock()
        {
            showLockError = false;
            if (string.IsNullOrWhiteSpace(lockReason)) { showLockError = true; return; }
            try
            {
                await ConfigService.LockAsync(Id, lockReason);
                showLockModal = false;
                lockReason = string.Empty;
                await RefreshAsync();
                ToastService.ShowSuccess("Config locked.");
            }
            catch (Exception ex) { ToastService.ShowError(ex.Message); }
        }

        private async Task OnUnlock()
        {
            try
            {
                await ConfigService.UnlockAsync(Id);
                await RefreshAsync();
                ToastService.ShowSuccess("Config unlocked.");
            }
            catch (Exception ex) { ToastService.ShowError(ex.Message); }
        }

        private async Task RefreshAsync()
        {
            Entry = await ConfigService.GetByIdAsync(Id);
            StateHasChanged();
            await JS.InvokeVoidAsync("feather.replace");
        }
    }
}
