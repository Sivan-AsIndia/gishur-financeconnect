using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.BudgetLineViewModel;

namespace FinanceConnect.Client.Pages.Budgeting_CostControl.BudgetLine
{
    public partial class BudgetLineList : ComponentBase
    {
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private BudgetLineService Service { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;

        private List<BudgetLineViewModel.BudgetLine> AllItems = new();
        private List<BudgetLineViewModel.BudgetLine> FilteredItems = new();
        private List<BudgetLineViewModel.BudgetLine> PagedItems = new();
        private BudgetLineViewModel.BudgetLine? SelectedItem;

        private string searchText = "";
        private string SelectedStatus = "";
        private string SelectedType = "";
        private int CurrentPage = 1;
        private int PageSize = 10;
        private int PageWindowSize = 3;

        private int TotalPages => FilteredItems.Count == 0 ? 1 : (int)Math.Ceiling((double)FilteredItems.Count / PageSize);
        private int StartPage => Math.Max(1, CurrentPage - PageWindowSize / 2);
        private int EndPage => Math.Min(TotalPages, StartPage + PageWindowSize - 1);

        protected override async Task OnInitializedAsync() { AllItems = await Service.GetAllAsync(); ApplyFilters(); }
        protected override async Task OnAfterRenderAsync(bool f) { await JS.InvokeVoidAsync("feather.replace"); await JS.InvokeVoidAsync("initTooltips"); }

        private void ApplyFilters()
        {
            var q = AllItems.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var t = searchText.Trim().ToLowerInvariant();
                q = q.Where(x => (x.LineCode?.ToLowerInvariant().Contains(t) ?? false) ||
                                  (x.LineName?.ToLowerInvariant().Contains(t) ?? false) ||
                                  (x.BudgetCategoryCode?.ToLowerInvariant().Contains(t) ?? false) ||
                                  (x.CostCenterName?.ToLowerInvariant().Contains(t) ?? false));
            }
            if (!string.IsNullOrEmpty(SelectedStatus) && int.TryParse(SelectedStatus, out var s))
                q = q.Where(x => (int)x.LineStatus == s);
            if (!string.IsNullOrEmpty(SelectedType) && int.TryParse(SelectedType, out var tp))
                q = q.Where(x => (int)x.LineType == tp);

            FilteredItems = q.OrderBy(x => x.LineNumber).ToList();
            if (CurrentPage > TotalPages) CurrentPage = 1;
            ApplyPaging();
        }

        private void ApplyPaging()
        {
            PagedItems = FilteredItems.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
        }

        private void OnSearch(ChangeEventArgs e) { searchText = e.Value?.ToString() ?? ""; CurrentPage = 1; ApplyFilters(); }
        private void OnFilterChanged(ChangeEventArgs e) { CurrentPage = 1; ApplyFilters(); }

        private async Task OnPageSizeChange(ChangeEventArgs e)
        {
            PageSize = int.Parse(e.Value?.ToString() ?? "10");
            CurrentPage = 1; ApplyFilters(); await Task.CompletedTask;
        }

        private async Task GoToPage(int page) { if (page >= 1 && page <= TotalPages) { CurrentPage = page; ApplyPaging(); } await Task.CompletedTask; }
        private async Task PreviousPage() { if (CurrentPage > 1) await GoToPage(CurrentPage - 1); }
        private async Task NextPage() { if (CurrentPage < TotalPages) await GoToPage(CurrentPage + 1); }

        private async Task OnRefreshAsync()
        {
            searchText = ""; SelectedStatus = ""; SelectedType = ""; CurrentPage = 1; PageSize = 10;
            Service.ResetToSeed();
            AllItems = await Service.GetAllAsync(); ApplyFilters();
            ToastService.ShowSuccess("Data refreshed successfully");
        }

        private void ConfirmDelete(Guid id)
        {
            try { Service.DeleteAsync(id); ToastService.ShowSuccess("Budget line deleted successfully", "Success"); AllItems = Service.GetAll(); ApplyFilters(); }
            catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message, "Error"); }
        }

        private string GetStatusDot(LineStatusEnum s) => s switch
        {
            LineStatusEnum.Draft => "bg-secondary",
            LineStatusEnum.Active => "bg-success",
            LineStatusEnum.Revised => "bg-warning",
            LineStatusEnum.Locked => "bg-danger",
            LineStatusEnum.Closed => "bg-dark",
            LineStatusEnum.Archived => "bg-secondary",
            _ => "bg-secondary"
        };

        private string GetTypeBadge(LineTypeEnum t) => t switch
        {
            LineTypeEnum.Expense => "bg-danger-transparent",
            LineTypeEnum.Revenue => "bg-success-transparent",
            LineTypeEnum.Capex => "bg-warning-transparent",
            LineTypeEnum.Statistical => "bg-info-transparent",
            LineTypeEnum.Transfer => "bg-primary-transparent",
            _ => "bg-secondary-transparent"
        };
    }
}
