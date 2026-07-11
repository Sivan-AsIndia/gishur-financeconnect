using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Budgeting_CostControl.CostCenter
{
    public partial class CostCenterList
    {
        private List<CostCenterModel> AllCenters = new();
        private List<BranchModel> BranchesList = new();
        private List<string> AvailableTypes = new();
        private List<string> AvailableStatuses = new();
        [Inject] CostCenterService CostCenterService { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;

        private CostCenterModel? SelectedCenter;
        private CostCenterModel? SelectedViewCenter;

        private string? SearchText;
        private string? SelectedType;
        private Guid? SelectedBranchId;
        private string? SelectedStatus;
        private string? LockReason;

        private int PageWindowSize = 2;
        private int StartPage = 1;
        private int CurrentPage = 1;
        private int PageSize = 10;
        private int VisibleColumnCount;

        private int EndPage => Math.Min(StartPage + PageWindowSize - 1, TotalPages);


        protected override void OnInitialized()
        {
            AllCenters = CostCenterService.GetAll();

            var branchIds = AllCenters
                .Where(c => c.BranchId.HasValue)
                .Select(c => c.BranchId!.Value)
                .Distinct()
                .ToHashSet();

            BranchesList = BranchService.GetAll()
                .Where(b => branchIds.Contains(b.Id))
                .ToList();

            AvailableTypes = AllCenters
                .Where(c => !string.IsNullOrWhiteSpace(c.CostCenterType))
                .Select(c => c.CostCenterType)
                .Distinct()
                .OrderBy(t => t)
                .ToList();

            AvailableStatuses = AllCenters
                .Where(c => !string.IsNullOrWhiteSpace(c.CostCenterStatus))
                .Select(c => c.CostCenterStatus)
                .Distinct()
                .OrderBy(s => s)
                .ToList();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
            VisibleColumnCount = await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        private IEnumerable<CostCenterModel> FilteredCenters =>
            AllCenters.Where(c =>
                (string.IsNullOrWhiteSpace(SearchText)
                 || c.CostCenterCode.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                 || c.CostCenterName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                 || (c.ShortName ?? "").Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                 || (c.CostCenterOwnerName ?? "").Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                && (string.IsNullOrEmpty(SelectedType) || c.CostCenterType == SelectedType)
                && (!SelectedBranchId.HasValue || c.BranchId == SelectedBranchId.Value)
                && (string.IsNullOrEmpty(SelectedStatus) || c.CostCenterStatus == SelectedStatus)
            ).OrderByDescending(c => c.UpdatedAt ?? c.CreatedAt);

        private IEnumerable<CostCenterModel> PagedCenters =>
            FilteredCenters.Skip((CurrentPage - 1) * PageSize).Take(PageSize);

        private int TotalPages =>
            Math.Max(1, (int)Math.Ceiling((double)FilteredCenters.Count() / PageSize));

        private async Task OnSearch(ChangeEventArgs e)
        {
            SearchText = e.Value?.ToString() ?? "";
            CurrentPage = 1;
            VisibleColumnCount = await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        private void ApplyFilters()
        {
            CurrentPage = 1;
        }

        private Task OnPageSizeChange(ChangeEventArgs e)
        {
            PageSize = int.Parse(e.Value!.ToString()!);
            CurrentPage = 1;
            return Task.CompletedTask;
        }

        private async Task GoToPage(int page)
        {
            if (page >= 1 && page <= TotalPages)
            {
                StateHasChanged();
                await Task.Delay(200);
                CurrentPage = page;
                StateHasChanged();
            }
        }

        private async Task PreviousPage()
        {
            if (CurrentPage > 1) await GoToPage(CurrentPage - 1);
        }

        private async Task NextPage()
        {
            if (CurrentPage < TotalPages) await GoToPage(CurrentPage + 1);
        }

        private void ConfirmDelete(CostCenterModel cc)
        {
            SelectedCenter = cc;
        }

        private void OpenLockModal(CostCenterModel cc)
        {
            SelectedCenter = cc;
            LockReason = null;
        }

        private async Task ViewCostCenter(CostCenterModel cc)
        {
            StateHasChanged();
            await Task.Delay(100);
            SelectedViewCenter = cc;
            StateHasChanged();
            await JS.InvokeVoidAsync("blazorOffcanvas.show", "viewCostCenterOffcanvas");
        }

        private void OpenRowDetails(CostCenterModel cc)
        {
            SelectedViewCenter = cc;
        }

        private async Task ConfirmToggleLock()
        {
            if (SelectedCenter == null) return;

            if (!SelectedCenter.IsLocked)
            {
                CostCenterService.Lock(SelectedCenter.Id, LockReason ?? "Locked by user", "Current User");
                ToastService.ShowWarning($"'{SelectedCenter.CostCenterName}' locked successfully", "Locked");
            }
            else
            {
                CostCenterService.Unlock(SelectedCenter.Id);
                ToastService.ShowSuccess($"'{SelectedCenter.CostCenterName}' unlocked successfully", "Unlocked");
            }

            AllCenters = CostCenterService.GetAll();
            await InvokeAsync(StateHasChanged);
            await JS.InvokeVoidAsync("feather.replace");
            SelectedCenter = null;
        }

        private void DeleteCostCenter()
        {
            if (SelectedCenter == null) return;

            try
            {
                CostCenterService.Delete(SelectedCenter.Id, "Deleted by user");
                AllCenters = CostCenterService.GetAll();
                ToastService.ShowError($"'{SelectedCenter.CostCenterName}' deleted", "Deleted");
            }
            catch (InvalidOperationException ex)
            {
                ToastService.ShowError(ex.Message, "Cannot Delete");
            }

            SelectedCenter = null;
        }

        private Task Reload()
        {
            SearchText = null;
            SelectedType = null;
            SelectedBranchId = null;
            SelectedStatus = null;
            CurrentPage = 1;
            StartPage = 1;
            AllCenters = CostCenterService.GetAll();
            return Task.CompletedTask;
        }

        // ─── Badge Helpers ─────────────────────────────────────────────
        private string GetStatusBadge(string status) => status switch
        {
            "Active" => "bg-success-transparent text-success",
            "Inactive" => "bg-danger-transparent text-danger",
            "Locked" => "bg-warning-transparent text-warning",
            "Closed" => "bg-secondary-transparent text-secondary",
            "Draft" => "bg-warning-transparent text-warning",
            "Archived" => "bg-secondary-transparent text-secondary",
            _ => "bg-secondary-transparent text-secondary"
        };

        private string GetStatusDotBadge(string status) => status switch
        {
            "Active" => "bg-success text-success",
            "Inactive" => "bg-danger text-danger",
            "Locked" => "bg-warning text-warning",
            "Closed" => "bg-secondary text-secondary",
            "Draft" => "bg-warning text-warning",
            _ => "bg-secondary text-secondary"
        };

        private string GetTypeBadge(string type) => type switch
        {
            "Operational" => "bg-primary-transparent text-primary",
            "Administrative" => "bg-info-transparent text-info",
            "SharedService" => "bg-purple-transparent text-purple",
            "Project" => "bg-teal-transparent text-teal",
            "RevenueSupport" => "bg-success-transparent text-success",
            "Regional" => "bg-warning-transparent text-warning",
            "Corporate" => "bg-secondary-transparent text-secondary",
            _ => "bg-secondary-transparent text-secondary"
        };

        private string GetControlModeBadge(string mode) => mode switch
        {
            "HardControl" => "bg-danger-transparent text-danger",
            "SoftControl" => "bg-warning-transparent text-warning",
            "Advisory" => "bg-info-transparent text-info",
            "ReportingOnly" => "bg-secondary-transparent text-secondary",
            _ => "bg-secondary-transparent text-secondary"
        };
    }
}
