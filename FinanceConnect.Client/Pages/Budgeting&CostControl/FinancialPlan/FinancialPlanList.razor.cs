using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.FinancialPlanViewModel;

namespace FinanceConnect.Client.Pages.Budgeting_CostControl.FinancialPlan
{
    public partial class FinancialPlanList : ComponentBase
    {
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private FinancialPlanService Service { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;

        private List<FinancialPlanListDto> AllItems = new();
        private List<FinancialPlanListDto> FilteredItems = new();
        private List<FinancialPlanListDto> PagedItems = new();
        private FinancialPlanListDto? SelectedItem;

        private string searchText = "";
        private string SelectedStatus = "";
        private string SelectedType = "";
        private string SelectedScenario = "";
        private int CurrentPage = 1;
        private int PageSize = 10;
        private int PageWindowSize = 2;

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
                q = q.Where(x => (x.PlanCode?.ToLowerInvariant().Contains(t) ?? false) || (x.PlanName?.ToLowerInvariant().Contains(t) ?? false));
            }
            if (!string.IsNullOrEmpty(SelectedStatus) && int.TryParse(SelectedStatus, out var s)) q = q.Where(x => (int)x.PlanStatus == s);
            if (!string.IsNullOrEmpty(SelectedType) && int.TryParse(SelectedType, out var tp)) q = q.Where(x => (int?)x.PlanType == tp);
            if (!string.IsNullOrEmpty(SelectedScenario) && int.TryParse(SelectedScenario, out var sc)) q = q.Where(x => (int?)x.ScenarioType == sc);
            FilteredItems = q.ToList();
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
            CurrentPage = 1;
            ApplyFilters();
            await Task.CompletedTask;
        }

        private async Task GoToPage(int page)
        {
            if (page >= 1 && page <= TotalPages) { CurrentPage = page; ApplyPaging(); }
            await Task.CompletedTask;
        }

        private async Task PreviousPage() { if (CurrentPage > 1) await GoToPage(CurrentPage - 1); }
        private async Task NextPage() { if (CurrentPage < TotalPages) await GoToPage(CurrentPage + 1); }

        private async Task OnRefreshAsync()
        {
            searchText = ""; SelectedStatus = ""; SelectedType = ""; SelectedScenario = ""; CurrentPage = 1; PageSize = 10;
            AllItems = await Service.GetAllAsync(); ApplyFilters();
        }

        private void ConfirmDelete(Guid id)
        {
            try { Service.DeleteAsync(id); ToastService.ShowSuccess("Financial Plan deleted successfully", "Success"); AllItems = Service.GetAll(); ApplyFilters(); }
            catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message, "Error"); }
        }

        private string GetStatusDot(PlanStatusEnum s) => s switch
        {
            PlanStatusEnum.Draft => "bg-secondary",
            PlanStatusEnum.UnderPreparation => "bg-info",
            PlanStatusEnum.UnderReview => "bg-warning",
            PlanStatusEnum.Approved => "bg-success",
            PlanStatusEnum.Locked => "bg-danger",
            PlanStatusEnum.Superseded => "bg-secondary",
            PlanStatusEnum.Archived => "bg-dark",
            _ => "bg-secondary"
        };
    }
}
