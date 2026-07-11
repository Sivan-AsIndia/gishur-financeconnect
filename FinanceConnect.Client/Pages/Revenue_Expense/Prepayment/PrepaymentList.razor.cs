using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.PrepaymentViewModel;

namespace FinanceConnect.Client.Pages.Revenue_Expense.Prepayment
{
    public partial class PrepaymentList : ComponentBase
    {
        [Inject] private PrepaymentService Service { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;
        private List<PrepaymentViewModel.Prepayment> AllItems = new(), FilteredItems = new(), PagedItems = new();
        private PrepaymentViewModel.Prepayment? SelectedItem;
        private string searchText = "", SelectedStatus = "", SelectedSourceType = "";
        private int CurrentPage = 1, PageSize = 10;
        private int TotalPages => FilteredItems.Count == 0 ? 1 : (int)Math.Ceiling(FilteredItems.Count / (double)PageSize);
        private int StartPage => Math.Max(1, CurrentPage - 2);
        private int EndPage => Math.Min(TotalPages, StartPage + 4);
        private int VisibleColumnCount;
        protected override async Task OnInitializedAsync() { AllItems = await Service.GetAllAsync(); ApplyFilters(); }
        protected override async Task OnAfterRenderAsync(bool f) { await JS.InvokeVoidAsync("feather.replace"); await JS.InvokeVoidAsync("initTooltips");
            VisibleColumnCount =
await JS.InvokeAsync<int>("getVisibleTableColumns");
        }
        private async Task OnRefreshAsync() { searchText = ""; SelectedStatus = ""; SelectedSourceType = ""; CurrentPage = 1; AllItems = await Service.GetAllAsync(); ApplyFilters(); await JS.InvokeVoidAsync("feather.replace"); }
        private void OnSearch(ChangeEventArgs e) { searchText = e.Value?.ToString() ?? ""; CurrentPage = 1; ApplyFilters(); }
        private void OnFilterChanged(ChangeEventArgs e) { CurrentPage = 1; ApplyFilters(); }
        private void ApplyFilters()
        {
            var q = AllItems.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(searchText)) { var t = searchText.Trim().ToLowerInvariant(); q = q.Where(x => (x.PrepaymentCode != null && x.PrepaymentCode.ToLowerInvariant().Contains(t)) || (x.PrepaymentTitle != null && x.PrepaymentTitle.ToLowerInvariant().Contains(t))); }
            if (!string.IsNullOrEmpty(SelectedStatus) && int.TryParse(SelectedStatus, out var si) && Enum.IsDefined(typeof(PrepaymentStatusEnum), si)) q = q.Where(x => x.PrepaymentStatus == (PrepaymentStatusEnum)si);
            if (!string.IsNullOrEmpty(SelectedSourceType) && int.TryParse(SelectedSourceType, out var ti) && Enum.IsDefined(typeof(SourceTypeEnum), ti)) q = q.Where(x => x.SourceType == (SourceTypeEnum)ti);
            FilteredItems = q.ToList(); UpdatePagedList();
        }
        private void UpdatePagedList() => PagedItems = FilteredItems.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
        private void OnPageSizeChange(ChangeEventArgs e) { if (int.TryParse(e.Value?.ToString(), out var s)) { PageSize = s; CurrentPage = 1; UpdatePagedList(); } }
        private void PreviousPage() { if (CurrentPage > 1) { CurrentPage--; UpdatePagedList(); } }
        private void NextPage() { if (CurrentPage < TotalPages) { CurrentPage++; UpdatePagedList(); } }
        private void GoToPage(int p) { CurrentPage = p; UpdatePagedList(); }
        private async Task ConfirmDelete(Guid id) { try { await Service.DeleteAsync(id); AllItems.RemoveAll(x => x.PrepaymentId == id); ApplyFilters(); ToastService.ShowSuccess("Prepayment deleted."); } catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message); } }
        private static string GetStatusDotClass(PrepaymentStatusEnum s) => s switch { PrepaymentStatusEnum.Draft => "bg-warning", PrepaymentStatusEnum.Submitted => "bg-info", PrepaymentStatusEnum.Approved => "bg-primary", PrepaymentStatusEnum.Posted => "bg-success", PrepaymentStatusEnum.InProgress => "bg-info", PrepaymentStatusEnum.PartiallyReleased => "bg-warning", PrepaymentStatusEnum.FullyReleased => "bg-success", PrepaymentStatusEnum.Cancelled => "bg-secondary", PrepaymentStatusEnum.Closed => "bg-success", _ => "bg-info" };
    }
}
