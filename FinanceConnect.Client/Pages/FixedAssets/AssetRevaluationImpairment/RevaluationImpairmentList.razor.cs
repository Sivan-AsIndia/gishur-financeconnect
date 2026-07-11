using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.FixedAssets.AssetRevaluationImpairment
{
    public partial class RevaluationImpairmentList
    {
        private List<AssetRevaluationImpairmentViewModel.AssetRevaluationImpairment> AllItems = new();
        private List<AssetRevaluationImpairmentViewModel.AssetRevaluationImpairment> FilteredItems = new();
        private List<AssetRevaluationImpairmentViewModel.AssetRevaluationImpairment> PagedItems = new();

        [Inject] private AssetRevaluationImpairmentService Service { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;

        private string searchText = "";
        private string SelectedEventType = "";
        private string SelectedStatus = "";
        private int CurrentPage = 1;
        private int PageSize = 10;
        private int PageWindowSize = 2;
        private AssetRevaluationImpairmentViewModel.AssetRevaluationImpairment? SelectedItem;

        private List<AssetRevaluationImpairmentViewModel.EventTypeEnum> AvailableEventTypes = new();
        private List<AssetRevaluationImpairmentViewModel.EventStatusEnum> AvailableStatuses = new();

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
            AvailableEventTypes = AllItems.Where(x => x.EventType.HasValue).Select(x => x.EventType!.Value).Distinct().OrderBy(x => x).ToList();
            AvailableStatuses = AllItems.Select(x => x.EventStatus).Distinct().OrderBy(x => x).ToList();
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var query = AllItems.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(x =>
                    (x.EventNumber != null && x.EventNumber.Contains(searchText, StringComparison.OrdinalIgnoreCase)) ||
                    (x.AssetNameSnapshot != null && x.AssetNameSnapshot.Contains(searchText, StringComparison.OrdinalIgnoreCase)) ||
                    (x.AssetNumberSnapshot != null && x.AssetNumberSnapshot.Contains(searchText, StringComparison.OrdinalIgnoreCase)));
            }

            if (!string.IsNullOrEmpty(SelectedEventType))
            {
                var type = (AssetRevaluationImpairmentViewModel.EventTypeEnum)int.Parse(SelectedEventType);
                query = query.Where(x => x.EventType == type);
            }

            if (!string.IsNullOrEmpty(SelectedStatus))
            {
                var status = (AssetRevaluationImpairmentViewModel.EventStatusEnum)int.Parse(SelectedStatus);
                query = query.Where(x => x.EventStatus == status);
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
        private async Task OnRefreshAsync() { searchText = ""; SelectedEventType = ""; SelectedStatus = ""; CurrentPage = 1; PageSize = 10; LoadData(); await Task.CompletedTask; }

        private void ConfirmDelete(Guid id)
        {
            var item = AllItems.FirstOrDefault(x => x.AssetRevaluationImpairmentId == id);
            if (item != null)
            {
                AllItems.Remove(item);
                ToastService.ShowSuccess($"Event '{SelectedItem?.EventNumber}' deleted successfully", "Success");
                ApplyFilters();
            }
        }

        private string FormatEnumName(string? name)
        {
            if (string.IsNullOrEmpty(name)) return "";
            return string.Concat(name.Select((c, i) => i > 0 && char.IsUpper(c) ? " " + c : c.ToString()));
        }

        private string GetStatusBadgeClass(AssetRevaluationImpairmentViewModel.EventStatusEnum status) => status switch
        {
            AssetRevaluationImpairmentViewModel.EventStatusEnum.Draft => "bg-warning-transparent",
            AssetRevaluationImpairmentViewModel.EventStatusEnum.Submitted => "bg-info-transparent",
            AssetRevaluationImpairmentViewModel.EventStatusEnum.Approved => "bg-primary-transparent",
            AssetRevaluationImpairmentViewModel.EventStatusEnum.Rejected => "bg-danger-transparent",
            AssetRevaluationImpairmentViewModel.EventStatusEnum.Posted => "bg-success-transparent",
            AssetRevaluationImpairmentViewModel.EventStatusEnum.Cancelled => "bg-warning-transparent",
            AssetRevaluationImpairmentViewModel.EventStatusEnum.Reversed => "bg-danger-transparent",
            AssetRevaluationImpairmentViewModel.EventStatusEnum.Closed => "bg-dark-transparent",
            _ => "bg-secondary-transparent"
        };

        private string GetEventTypeBadge(AssetRevaluationImpairmentViewModel.EventTypeEnum? type) => type switch
        {
            AssetRevaluationImpairmentViewModel.EventTypeEnum.RevaluationIncrease => "bg-success-transparent",
            AssetRevaluationImpairmentViewModel.EventTypeEnum.RevaluationDecrease => "bg-warning-transparent",
            AssetRevaluationImpairmentViewModel.EventTypeEnum.ImpairmentLoss => "bg-danger-transparent",
            AssetRevaluationImpairmentViewModel.EventTypeEnum.ImpairmentReversal => "bg-info-transparent",
            _ => "bg-secondary-transparent"
        };
    }
}
