using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.AccrualViewModel;

namespace FinanceConnect.Client.Pages.Revenue_Expense.Accrual
{
    public partial class AccrualList : ComponentBase
    {
        [Inject] private AccrualService Service { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;

        private List<AccrualViewModel.Accrual> AllItems = new();
        private List<AccrualViewModel.Accrual> FilteredItems = new();
        private List<AccrualViewModel.Accrual> PagedItems = new();
        private AccrualViewModel.Accrual? SelectedItem;

        private string searchText = string.Empty;
        private string SelectedStatus = string.Empty;
        private string SelectedType = string.Empty;

        private int CurrentPage = 1;
        private int PageSize = 10;
        private int VisibleColumnCount;
        private int TotalPages => FilteredItems.Count == 0 ? 1 : (int)Math.Ceiling(FilteredItems.Count / (double)PageSize);
        private int StartPage => Math.Max(1, CurrentPage - 2);
        private int EndPage => Math.Min(TotalPages, StartPage + 4);

        protected override async Task OnInitializedAsync()
        {
            AllItems = await Service.GetAllAsync();
            ApplyFilters();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
            VisibleColumnCount =
await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        private async Task OnRefreshAsync()
        {
            searchText = string.Empty; SelectedStatus = string.Empty; SelectedType = string.Empty; CurrentPage = 1;
            AllItems = await Service.GetAllAsync();
            ApplyFilters();
            await JS.InvokeVoidAsync("feather.replace");
        }

        private void OnSearch(ChangeEventArgs e) { searchText = e.Value?.ToString() ?? ""; CurrentPage = 1; ApplyFilters(); }
        private void OnFilterChanged(ChangeEventArgs e) { CurrentPage = 1; ApplyFilters(); }

        private void ApplyFilters()
        {
            var q = AllItems.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var t = searchText.Trim().ToLowerInvariant();
                q = q.Where(x => (x.AccrualCode != null && x.AccrualCode.ToLowerInvariant().Contains(t)) ||
                                  (x.AccrualTitle != null && x.AccrualTitle.ToLowerInvariant().Contains(t)));
            }
            if (!string.IsNullOrEmpty(SelectedStatus) && int.TryParse(SelectedStatus, out var si) && Enum.IsDefined(typeof(AccrualStatusEnum), si))
                q = q.Where(x => x.AccrualStatus == (AccrualStatusEnum)si);
            if (!string.IsNullOrEmpty(SelectedType) && int.TryParse(SelectedType, out var ti) && Enum.IsDefined(typeof(AccrualTypeEnum), ti))
                q = q.Where(x => x.AccrualType == (AccrualTypeEnum)ti);
            FilteredItems = q.ToList();
            UpdatePagedList();
        }

        private void UpdatePagedList() => PagedItems = FilteredItems.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
        private void OnPageSizeChange(ChangeEventArgs e) { if (int.TryParse(e.Value?.ToString(), out var s)) { PageSize = s; CurrentPage = 1; UpdatePagedList(); } }
        private void PreviousPage() { if (CurrentPage > 1) { CurrentPage--; UpdatePagedList(); } }
        private void NextPage() { if (CurrentPage < TotalPages) { CurrentPage++; UpdatePagedList(); } }
        private void GoToPage(int p) { CurrentPage = p; UpdatePagedList(); }

        private async Task ConfirmDelete(Guid id)
        {
            try { await Service.DeleteAsync(id); AllItems.RemoveAll(x => x.AccrualId == id); ApplyFilters(); ToastService.ShowSuccess("Accrual deleted."); }
            catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message); }
        }

        private static string GetStatusDotClass(AccrualStatusEnum s) => s switch
        {
            AccrualStatusEnum.Draft => "bg-warning", AccrualStatusEnum.Submitted => "bg-info",
            AccrualStatusEnum.Approved => "bg-primary", AccrualStatusEnum.Posted => "bg-success",
            AccrualStatusEnum.Cancelled => "bg-secondary", AccrualStatusEnum.Closed => "bg-success",
            _ => "bg-info"
        };
        private static string GetStatusBadgeClass(AccrualStatusEnum s) => s switch
        {
            AccrualStatusEnum.Draft => "bg-warning-transparent", AccrualStatusEnum.Submitted => "bg-info-transparent",
            AccrualStatusEnum.Approved => "bg-primary-transparent", AccrualStatusEnum.Posted => "bg-success-transparent",
            AccrualStatusEnum.Cancelled => "bg-secondary-transparent", AccrualStatusEnum.Closed => "bg-success-transparent",
            _ => "bg-info-transparent"
        };
    }
}
