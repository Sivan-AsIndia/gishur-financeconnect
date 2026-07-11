using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.FixedAssets.DepreciationMethod
{
    public partial class DepreciationMethodList
    {
        private List<DepreciationMethodViewModel.DepreciationMethod> AllItems = new();
        private List<DepreciationMethodViewModel.DepreciationMethod> FilteredItems = new();
        private List<DepreciationMethodViewModel.DepreciationMethod> PagedItems = new();

        [Inject] private DepreciationMethodService Service { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;

        private string searchText = "";
        private string SelectedMethodType = "";
        private string SelectedStatus = "";

        private int CurrentPage = 1;
        private int PageSize = 10;
        private int PageWindowSize = 2;
        private DepreciationMethodViewModel.DepreciationMethod? SelectedItem;

        private List<DepreciationMethodViewModel.MethodTypeEnum> AvailableMethodTypes = new();
        private List<DepreciationMethodViewModel.MethodStatusEnum> AvailableStatuses = new();

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
            AvailableMethodTypes = AllItems.Where(x => x.MethodType.HasValue).Select(x => x.MethodType!.Value).Distinct().OrderBy(x => x).ToList();
            AvailableStatuses = AllItems.Select(x => x.MethodStatus).Distinct().OrderBy(x => x).ToList();
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var query = AllItems.AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(x => x.MethodCode.Contains(searchText, StringComparison.OrdinalIgnoreCase) || x.MethodName.Contains(searchText, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(SelectedMethodType))
            {
                var type = (DepreciationMethodViewModel.MethodTypeEnum)int.Parse(SelectedMethodType);
                query = query.Where(x => x.MethodType == type);
            }
            if (!string.IsNullOrEmpty(SelectedStatus))
            {
                var status = (DepreciationMethodViewModel.MethodStatusEnum)int.Parse(SelectedStatus);
                query = query.Where(x => x.MethodStatus == status);
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
        private async Task OnRefreshAsync() { searchText = ""; SelectedMethodType = ""; SelectedStatus = ""; CurrentPage = 1; PageSize = 10; LoadData(); await Task.CompletedTask; }
        private void DeletePopupOpen(DepreciationMethodViewModel.DepreciationMethod item) => SelectedItem = item;
        private void OpenRowDetails(DepreciationMethodViewModel.DepreciationMethod item) => SelectedItem = item;

        private void ConfirmDelete(Guid id)
        {
            var item = AllItems.FirstOrDefault(x => x.DepreciationMethodId == id);
            if (item != null)
            {
                AllItems.Remove(item);
                ToastService.ShowSuccess($"Method '{SelectedItem?.MethodCode}' deleted successfully", "Success");
                ApplyFilters();
            }
        }

        private string FormatEnumName(string? name)
        {
            if (string.IsNullOrEmpty(name)) return "";
            return string.Concat(name.Select((c, i) => i > 0 && char.IsUpper(c) ? " " + c : c.ToString()));
        }

        private string GetStatusDotBadge(DepreciationMethodViewModel.MethodStatusEnum status) => status switch
        {
            DepreciationMethodViewModel.MethodStatusEnum.Active => "bg-success",
            DepreciationMethodViewModel.MethodStatusEnum.Inactive => "bg-danger",
            DepreciationMethodViewModel.MethodStatusEnum.Archived => "bg-warning",
            _ => "bg-secondary"
        };
    }
}
