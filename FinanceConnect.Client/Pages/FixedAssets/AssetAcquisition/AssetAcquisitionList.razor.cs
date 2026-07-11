using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.FixedAssets.AssetAcquisition
{
    public partial class AssetAcquisitionList
    {
        private List<AssetAcquisitionViewModel.AssetAcquisition> AllItems = new();
        private List<AssetAcquisitionViewModel.AssetAcquisition> FilteredItems = new();
        private List<AssetAcquisitionViewModel.AssetAcquisition> PagedItems = new();

        [Inject] private AssetAcquisitionService Service { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;

        private string searchText = "";
        private string SelectedType = "";
        private string SelectedStatus = "";

        private int CurrentPage = 1;
        private int PageSize = 10;
        private int PageWindowSize = 2;
        private AssetAcquisitionViewModel.AssetAcquisition? SelectedItem;

        private List<AssetAcquisitionViewModel.AcquisitionTypeEnum> AvailableTypes = new();
        private List<AssetAcquisitionViewModel.AcquisitionStatusEnum> AvailableStatuses = new();

        private int TotalPages => FilteredItems.Count == 0 ? 1 :
            (int)Math.Ceiling((double)FilteredItems.Count / PageSize);
        private int StartPage => Math.Max(1, CurrentPage - PageWindowSize / 2);
        private int EndPage => Math.Min(TotalPages, StartPage + PageWindowSize - 1);

        protected override void OnInitialized()
        {
            LoadData();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
        }

        private void LoadData()
        {
            AllItems = Service.GetAll().Where(x => !x.IsDeleted).ToList();

            AvailableTypes = AllItems
                .Where(x => x.AcquisitionType.HasValue)
                .Select(x => x.AcquisitionType!.Value)
                .Distinct().OrderBy(x => x).ToList();

            AvailableStatuses = AllItems
                .Select(x => x.AcquisitionStatus)
                .Distinct().OrderBy(x => x).ToList();

            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var query = AllItems.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(x =>
                    (x.AcquisitionNumber != null && x.AcquisitionNumber.Contains(searchText, StringComparison.OrdinalIgnoreCase)) ||
                    (x.AssetNameSnapshot != null && x.AssetNameSnapshot.Contains(searchText, StringComparison.OrdinalIgnoreCase)) ||
                    (x.AssetNumberSnapshot != null && x.AssetNumberSnapshot.Contains(searchText, StringComparison.OrdinalIgnoreCase)));
            }

            if (!string.IsNullOrEmpty(SelectedType))
            {
                var type = (AssetAcquisitionViewModel.AcquisitionTypeEnum)int.Parse(SelectedType);
                query = query.Where(x => x.AcquisitionType == type);
            }

            if (!string.IsNullOrEmpty(SelectedStatus))
            {
                var status = (AssetAcquisitionViewModel.AcquisitionStatusEnum)int.Parse(SelectedStatus);
                query = query.Where(x => x.AcquisitionStatus == status);
            }

            FilteredItems = query.ToList();
            if (CurrentPage > TotalPages) CurrentPage = 1;
            ApplyPaging();
        }

        private void ApplyPaging()
        {
            PagedItems = FilteredItems.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
        }

        private void OnSearch(ChangeEventArgs e) { searchText = e.Value?.ToString() ?? ""; CurrentPage = 1; ApplyFilters(); }
        private void OnFilterChanged(ChangeEventArgs e) { CurrentPage = 1; ApplyFilters(); }
        private async Task OnPageSizeChange(ChangeEventArgs e) { PageSize = int.Parse(e.Value?.ToString() ?? "10"); CurrentPage = 1; ApplyFilters(); await Task.CompletedTask; }
        private async Task GoToPage(int page) { if (page >= 1 && page <= TotalPages) { CurrentPage = page; ApplyPaging(); } await Task.CompletedTask; }
        private async Task PreviousPage() { if (CurrentPage > 1) await GoToPage(CurrentPage - 1); }
        private async Task NextPage() { if (CurrentPage < TotalPages) await GoToPage(CurrentPage + 1); }

        private async Task OnRefreshAsync()
        {
            searchText = ""; SelectedType = ""; SelectedStatus = ""; CurrentPage = 1; PageSize = 10;
            LoadData(); await Task.CompletedTask;
        }

        private void DeletePopupOpen(AssetAcquisitionViewModel.AssetAcquisition item) => SelectedItem = item;
        private void OpenRowDetails(AssetAcquisitionViewModel.AssetAcquisition item) => SelectedItem = item;

        private void ConfirmDelete(Guid id)
        {
            var item = AllItems.FirstOrDefault(x => x.AssetAcquisitionId == id);
            if (item != null)
            {
                AllItems.Remove(item);
                ToastService.ShowSuccess($"Acquisition '{SelectedItem?.AcquisitionNumber}' deleted successfully", "Success");
                ApplyFilters();
            }
        }

        private string FormatEnumName(string? name)
        {
            if (string.IsNullOrEmpty(name)) return "";
            return string.Concat(name.Select((c, i) => i > 0 && char.IsUpper(c) ? " " + c : c.ToString()));
        }

        private string GetStatusBadgeClass(AssetAcquisitionViewModel.AcquisitionStatusEnum status)
        {
            return status switch
            {
                AssetAcquisitionViewModel.AcquisitionStatusEnum.Draft => "bg-warning-transparent",
                AssetAcquisitionViewModel.AcquisitionStatusEnum.Submitted => "bg-info-transparent",
                AssetAcquisitionViewModel.AcquisitionStatusEnum.Approved => "bg-primary-transparent",
                AssetAcquisitionViewModel.AcquisitionStatusEnum.Rejected => "bg-danger-transparent",
                AssetAcquisitionViewModel.AcquisitionStatusEnum.Posted => "bg-success-transparent",
                AssetAcquisitionViewModel.AcquisitionStatusEnum.Cancelled => "bg-secondary-transparent",
                AssetAcquisitionViewModel.AcquisitionStatusEnum.Reversed => "bg-danger-transparent",
                _ => "bg-secondary-transparent"
            };
        }
    }
}
