using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.VarianceAnalysisViewModel;

namespace FinanceConnect.Client.Pages.Budgeting_CostControl.VarianceAnalysis
{
    public partial class VarianceAnalysisList : ComponentBase
    {
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private VarianceAnalysisService Service { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;

        private List<VarianceAnalysisViewModel.VarianceAnalysis> AllItems = new();
        private List<VarianceAnalysisViewModel.VarianceAnalysis> FilteredItems = new();
        private List<VarianceAnalysisViewModel.VarianceAnalysis> PagedItems = new();

        private VarianceAnalysisViewModel.VarianceAnalysis? SelectedItem;
        private string searchText = "";
        private string SelectedMode = "";
        private string SelectedStatus = "";
        private int CurrentPage = 1;
        private int PageSize = 10;
        private int PageWindowSize = 2;

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
            AllItems = Service.GetAll();
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var query = AllItems.AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(x =>
                    x.AnalysisCode.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    x.AnalysisName.Contains(searchText, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(SelectedMode) && int.TryParse(SelectedMode, out var mode))
                query = query.Where(x => (int?)x.ComparisonMode == mode);

            if (!string.IsNullOrEmpty(SelectedStatus) && int.TryParse(SelectedStatus, out var status))
                query = query.Where(x => (int)x.AnalysisStatus == status);

            FilteredItems = query.ToList();
            if (CurrentPage > TotalPages) CurrentPage = 1;
            ApplyPaging();
        }

        private void ApplyPaging()
        {
            PagedItems = FilteredItems
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();
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
            searchText = ""; SelectedMode = ""; SelectedStatus = ""; CurrentPage = 1; PageSize = 10;
            LoadData();
            await Task.CompletedTask;
        }

        private void ConfirmDelete(Guid id)
        {
            try
            {
                Service.DeleteAsync(id);
                ToastService.ShowSuccess("Variance Analysis deleted successfully", "Success");
                LoadData();
            }
            catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message, "Error"); }
        }

        private string GetModeLabel(ComparisonModeEnum? mode) => mode switch
        {
            ComparisonModeEnum.BudgetVsActual => "Budget vs Actual",
            ComparisonModeEnum.BudgetVsForecast => "Budget vs Forecast",
            ComparisonModeEnum.ActualVsForecast => "Actual vs Forecast",
            ComparisonModeEnum.BudgetVsCommitted => "Budget vs Committed",
            ComparisonModeEnum.RevisedBudgetVsActual => "Revised Budget vs Actual",
            _ => "–"
        };

        private string GetStatusDot(AnalysisStatusEnum status) => status switch
        {
            AnalysisStatusEnum.Draft => "bg-secondary",
            AnalysisStatusEnum.Generated => "bg-info",
            AnalysisStatusEnum.UnderReview => "bg-warning",
            AnalysisStatusEnum.Reviewed => "bg-primary",
            AnalysisStatusEnum.Approved => "bg-success",
            AnalysisStatusEnum.Locked => "bg-danger",
            AnalysisStatusEnum.Archived => "bg-dark",
            _ => "bg-secondary"
        };
    }
}
