using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.BudgetPeriodViewModel;

namespace FinanceConnect.Client.Pages.Budgeting_CostControl.BudgetPeriod
{
    public partial class BudgetPeriodList : ComponentBase
    {
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private BudgetPeriodService Service { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;

        private List<BudgetPeriodViewModel.BudgetPeriod> AllItems = new();
        private List<BudgetPeriodViewModel.BudgetPeriod> FilteredItems = new();
        private List<BudgetPeriodViewModel.BudgetPeriod> PagedItems = new();
        private BudgetPeriodViewModel.BudgetPeriod? SelectedItem;

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
                q = q.Where(x => (x.PeriodCode?.ToLowerInvariant().Contains(t) ?? false) ||
                                  (x.PeriodName?.ToLowerInvariant().Contains(t) ?? false));
            }
            if (!string.IsNullOrEmpty(SelectedStatus) && int.TryParse(SelectedStatus, out var s))
                q = q.Where(x => (int)x.PeriodStatus == s);
            if (!string.IsNullOrEmpty(SelectedType) && int.TryParse(SelectedType, out var tp))
                q = q.Where(x => (int)x.PeriodType == tp);

            FilteredItems = q.OrderBy(x => x.PeriodSequenceNo).ToList();
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
            try { Service.DeleteAsync(id); ToastService.ShowSuccess("Budget period deleted successfully", "Success"); AllItems = Service.GetAll(); ApplyFilters(); }
            catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message, "Error"); }
        }

        private string GetStatusDot(PeriodStatusEnum s) => s switch
        {
            PeriodStatusEnum.Draft => "bg-secondary",
            PeriodStatusEnum.Open => "bg-info",
            PeriodStatusEnum.Released => "bg-primary",
            PeriodStatusEnum.Locked => "bg-danger",
            PeriodStatusEnum.Closed => "bg-dark",
            PeriodStatusEnum.Revised => "bg-warning",
            PeriodStatusEnum.Archived => "bg-secondary",
            _ => "bg-secondary"
        };
    }
}
