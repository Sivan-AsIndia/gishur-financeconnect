using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.FixedAssets.AssetDepreciationSchedule
{
    public partial class DepreciationScheduleList
    {
        private List<AssetDepreciationScheduleViewModel.AssetDepreciationSchedule> AllItems = new();
        private List<AssetDepreciationScheduleViewModel.AssetDepreciationSchedule> FilteredItems = new();
        private List<AssetDepreciationScheduleViewModel.AssetDepreciationSchedule> PagedItems = new();

        [Inject] private AssetDepreciationScheduleService Service { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;

        private string searchText = "";
        private string SelectedStatus = "";

        private int CurrentPage = 1;
        private int PageSize = 10;
        private int PageWindowSize = 2;
        private AssetDepreciationScheduleViewModel.AssetDepreciationSchedule? SelectedItem;

        private List<AssetDepreciationScheduleViewModel.ScheduleStatusEnum> AvailableStatuses = new();

        private int TotalPages => FilteredItems.Count == 0 ? 1 : (int)Math.Ceiling((double)FilteredItems.Count / PageSize);
        private int StartPage => Math.Max(1, CurrentPage - PageWindowSize / 2);
        private int EndPage => Math.Min(TotalPages, StartPage + PageWindowSize - 1);

        protected override void OnInitialized() => LoadData();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
        }

        private void LoadData()
        {
            AllItems = Service.GetAll().Where(x => !x.IsDeleted).ToList();
            AvailableStatuses = AllItems.Select(x => x.ScheduleStatus).Distinct().OrderBy(x => x).ToList();
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var query = AllItems.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(x =>
                    (x.ScheduleNumber != null && x.ScheduleNumber.Contains(searchText, StringComparison.OrdinalIgnoreCase)) ||
                    (x.AssetNameDisplay != null && x.AssetNameDisplay.Contains(searchText, StringComparison.OrdinalIgnoreCase)) ||
                    (x.AssetCodeDisplay != null && x.AssetCodeDisplay.Contains(searchText, StringComparison.OrdinalIgnoreCase)));
            if (!string.IsNullOrEmpty(SelectedStatus))
            {
                var status = (AssetDepreciationScheduleViewModel.ScheduleStatusEnum)int.Parse(SelectedStatus);
                query = query.Where(x => x.ScheduleStatus == status);
            }
            FilteredItems = query.ToList();
            if (CurrentPage > TotalPages) CurrentPage = 1;
            ApplyPaging();
        }

        private void ApplyPaging() => PagedItems = FilteredItems.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
        private void OnSearch(ChangeEventArgs e) { searchText = e.Value?.ToString() ?? ""; CurrentPage = 1; ApplyFilters(); }
        private void OnFilterChanged(ChangeEventArgs e) { CurrentPage = 1; ApplyFilters(); }
        private async Task OnPageSizeChange(ChangeEventArgs e) { PageSize = int.Parse(e.Value?.ToString() ?? "10"); CurrentPage = 1; ApplyFilters(); await Task.CompletedTask; }
        private async Task GoToPage(int page) { if (page >= 1 && page <= TotalPages) { CurrentPage = page; ApplyPaging(); } await Task.CompletedTask; }
        private async Task PreviousPage() { if (CurrentPage > 1) await GoToPage(CurrentPage - 1); }
        private async Task NextPage() { if (CurrentPage < TotalPages) await GoToPage(CurrentPage + 1); }
        private async Task OnRefreshAsync() { searchText = ""; SelectedStatus = ""; CurrentPage = 1; PageSize = 10; LoadData(); await Task.CompletedTask; }
        private void OpenRowDetails(AssetDepreciationScheduleViewModel.AssetDepreciationSchedule item) => SelectedItem = item;

        private string GetStatusBadgeClass(AssetDepreciationScheduleViewModel.ScheduleStatusEnum status) => status switch
        {
            AssetDepreciationScheduleViewModel.ScheduleStatusEnum.Draft => "bg-warning-transparent",
            AssetDepreciationScheduleViewModel.ScheduleStatusEnum.Active => "bg-success-transparent",
            AssetDepreciationScheduleViewModel.ScheduleStatusEnum.Superseded => "bg-warning-transparent",
            AssetDepreciationScheduleViewModel.ScheduleStatusEnum.Locked => "bg-danger-transparent",
            AssetDepreciationScheduleViewModel.ScheduleStatusEnum.Cancelled => "bg-secondary-transparent",
            _ => "bg-secondary-transparent"
        };
    }
}
