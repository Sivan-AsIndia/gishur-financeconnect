using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.FixedAssets.AssetCategory
{
    public partial class AssetsCategory
    {
        [Inject] private AssetCategoryService Service { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;

        private List<AssetsCategoryViewModel.AssetCategory> AllCategories = new();
        private List<AssetsCategoryViewModel.AssetCategory> FilteredCategories = new();
        private List<AssetsCategoryViewModel.AssetCategory> PagedCategories = new();

        private string searchText = "";
        private string SelectedType = "";
        private string SelectedStatus = "";

        private int CurrentPage = 1;
        private int PageSize = 10;
        private int PageWindowSize = 2;

        private int TotalPages => FilteredCategories.Count == 0 ? 1
            : (int)Math.Ceiling((double)FilteredCategories.Count / PageSize);

        private int StartPage => Math.Max(1, CurrentPage - PageWindowSize / 2);
        private int EndPage => Math.Min(TotalPages, StartPage + PageWindowSize - 1);

        private AssetsCategoryViewModel.AssetCategory? SelectedCategory;

        private string LockReasonInput = "";
        private bool LockReasonError = false;

        protected override void OnInitialized() => LoadData();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
        }

        private void LoadData()
        {
            AllCategories = Service.GetAll().Where(x => !x.IsDeleted).ToList();
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var query = AllCategories.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(x =>
                    x.CategoryName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    x.CategoryCode.Contains(searchText, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(SelectedType))
            {
                var type = (AssetsCategoryViewModel.AssetType)int.Parse(SelectedType);
                query = query.Where(x => x.AssetType == type);
            }

            if (!string.IsNullOrEmpty(SelectedStatus))
            {
                var status = (AssetsCategoryViewModel.CategoryStatus)int.Parse(SelectedStatus);
                query = query.Where(x => x.CategoryStatus == status);
            }

            FilteredCategories = query.ToList();

            if (CurrentPage > TotalPages) CurrentPage = 1;

            ApplyPaging();
        }

        private void ApplyPaging()
        {
            PagedCategories = FilteredCategories
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();
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

        private async Task OnPageSizeChange(ChangeEventArgs e)
        {
            PageSize = int.Parse(e.Value?.ToString() ?? "10");
            CurrentPage = 1;
            ApplyFilters();
            await Task.CompletedTask;
        }

        private async Task GoToPage(int page)
        {
            if (page >= 1 && page <= TotalPages)
            {
                CurrentPage = page;
                ApplyPaging();
            }
            await Task.CompletedTask;
        }

        private async Task PreviousPage()
        {
            if (CurrentPage > 1) await GoToPage(CurrentPage - 1);
        }

        private async Task NextPage()
        {
            if (CurrentPage < TotalPages) await GoToPage(CurrentPage + 1);
        }

        private async Task OnRefreshAsync()
        {
            searchText = "";
            SelectedType = "";
            SelectedStatus = "";
            CurrentPage = 1;
            PageSize = 10;
            LoadData();
            await Task.CompletedTask;
        }

        private void OpenRowDetails(AssetsCategoryViewModel.AssetCategory category)
            => SelectedCategory = category;

        private void DeletePopupOpen(AssetsCategoryViewModel.AssetCategory category)
            => SelectedCategory = category;

        private void LockPopupOpen(AssetsCategoryViewModel.AssetCategory category)
        {
            SelectedCategory = category;
            LockReasonInput = "";
            LockReasonError = false;
        }

        private void UnlockPopupOpen(AssetsCategoryViewModel.AssetCategory category)
            => SelectedCategory = category;

        private void ArchivePopupOpen(AssetsCategoryViewModel.AssetCategory category)
            => SelectedCategory = category;

        private void ConfirmDelete(Guid id)
        {
            var item = AllCategories.FirstOrDefault(x => x.AssetCategoryId == id);
            if (item == null) return;

            try
            {
                Service.DeleteAsync(id);
                AllCategories.Remove(item);
                ToastService.ShowSuccess(
                    $"Asset Category '{SelectedCategory?.CategoryName}' deleted successfully",
                    "Success");
                ApplyFilters();
            }
            catch (InvalidOperationException ex)
            {
                ToastService.ShowError(ex.Message, "Error");
            }
        }

        private async Task ConfirmLock()
        {
            if (string.IsNullOrWhiteSpace(LockReasonInput))
            {
                LockReasonError = true;
                return;
            }

            if (SelectedCategory == null) return;

            try
            {
                await Service.LockAsync(
                    SelectedCategory.AssetCategoryId,
                    LockReasonInput.Trim(),
                    Guid.Empty);

                ToastService.ShowSuccess(
                    $"Category '{SelectedCategory.CategoryName}' locked successfully",
                    "Locked");

                LoadData();
            }
            catch (InvalidOperationException ex)
            {
                ToastService.ShowError(ex.Message, "Error");
            }
        }

        private async Task ConfirmUnlock()
        {
            if (SelectedCategory == null) return;

            try
            {
                await Service.UnlockAsync(
                    SelectedCategory.AssetCategoryId,
                    Guid.Empty);

                ToastService.ShowSuccess(
                    $"Category '{SelectedCategory.CategoryName}' unlocked successfully",
                    "Unlocked");

                LoadData();
            }
            catch (InvalidOperationException ex)
            {
                ToastService.ShowError(ex.Message, "Error");
            }
        }
        private async Task ConfirmArchive()
        {
            if (SelectedCategory == null) return;

            try
            {
                await Service.ArchiveAsync(SelectedCategory.AssetCategoryId);

                ToastService.ShowSuccess(
                    $"Category '{SelectedCategory.CategoryName}' archived successfully",
                    "Archived");

                LoadData();
            }
            catch (InvalidOperationException ex)
            {
                ToastService.ShowError(ex.Message, "Error");
            }
        }

        private void OnLockReasonInput(ChangeEventArgs e)
        {
            LockReasonInput = e.Value?.ToString() ?? "";
            LockReasonError = false;
        }
        private string GetStatusDotBadge(AssetsCategoryViewModel.CategoryStatus status) =>
            status switch
            {
                AssetsCategoryViewModel.CategoryStatus.Active => "bg-success",
                AssetsCategoryViewModel.CategoryStatus.Inactive => "bg-danger",
                AssetsCategoryViewModel.CategoryStatus.Archived => "bg-warning",
                _ => "bg-secondary"
            };

        private string GetStatusBadgeClass(AssetsCategoryViewModel.CategoryStatus status) =>
            status switch
            {
                AssetsCategoryViewModel.CategoryStatus.Active => "bg-success-transparent",
                AssetsCategoryViewModel.CategoryStatus.Inactive => "bg-danger-transparent",
                AssetsCategoryViewModel.CategoryStatus.Archived => "bg-warning-transparent",
                _ => "bg-secondary-transparent"
            };
    }
}
