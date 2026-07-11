using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.GSTReturnRunViewModel;

namespace FinanceConnect.Client.Pages.Tax___Statutory.GSTReturnRun
{
    public partial class GSTReturnRunList : ComponentBase
    {
        private bool isInitialized, isLoading;
        private List<GSTReturnRunModel> allItems = new();
        private GSTReturnRunModel? Selected;
        private GSTReturnRunModel? SelectedView;
        private string searchText = "", ActionReason = "";
        private string _selectedStatus = "", _selectedFiling = "";
        private int VisibleColumnCount, CurrentPage = 1, PageSize = 10;
        private string ActionName = "", ActionTitle = "", ActionMsg = "", ActionIcon = "", ActionBg = "", ActionTxt = "", ActionBtn = "";
        private bool NeedReason;

        public string SelectedStatus { get => _selectedStatus; set { _selectedStatus = value; CurrentPage = 1; } }
        public string SelectedFiling { get => _selectedFiling; set { _selectedFiling = value; CurrentPage = 1; } }
        private List<string> DistinctStatuses => allItems.Select(x => x.ReturnRunStatus).Distinct().OrderBy(x => x).ToList();
        private List<string> DistinctFilings => allItems.Select(x => x.FilingStatus).Distinct().OrderBy(x => x).ToList();

        private IEnumerable<GSTReturnRunModel> FilteredItems => allItems
            .Where(x => string.IsNullOrEmpty(searchText) || (x.ReturnRunNumber ?? "").Contains(searchText, StringComparison.OrdinalIgnoreCase) || (x.ReturnPeriodKey ?? "").Contains(searchText, StringComparison.OrdinalIgnoreCase))
            .Where(x => string.IsNullOrEmpty(_selectedStatus) || x.ReturnRunStatus == _selectedStatus)
            .Where(x => string.IsNullOrEmpty(_selectedFiling) || x.FilingStatus == _selectedFiling)
            .OrderByDescending(x => x.PeriodStartDate);

        private int TotalPages => Math.Max(1, (int)Math.Ceiling(FilteredItems.Count() / (double)PageSize));
        private List<GSTReturnRunModel> PagedItems => FilteredItems.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

        protected override async Task OnInitializedAsync() { await LoadData(); isInitialized = true; }
        protected override async Task OnAfterRenderAsync(bool firstRender) { await JS.InvokeVoidAsync("feather.replace"); await JS.InvokeVoidAsync("initTooltips", true); VisibleColumnCount = await JS.InvokeAsync<int>("getVisibleTableColumns"); }
        private async Task LoadData() { isLoading = true; StateHasChanged(); await Task.Delay(100); allItems = RunService.GetList(); isLoading = false; StateHasChanged(); }
        private async Task OnRefreshAsync() { isLoading = true; StateHasChanged(); await Task.Delay(200); searchText = ""; _selectedStatus = _selectedFiling = ""; CurrentPage = 1; await LoadData(); ToastService.ShowSuccess("Data refreshed"); }
        private async Task OnSearch(ChangeEventArgs e) { searchText = e.Value?.ToString() ?? ""; CurrentPage = 1; VisibleColumnCount = await JS.InvokeAsync<int>("getVisibleTableColumns"); }
        private void GoToPage(int p) { if (p >= 1 && p <= TotalPages) CurrentPage = p; }
        private void OnPageSizeChange(ChangeEventArgs e) { if (int.TryParse(e.Value?.ToString(), out var s)) { PageSize = s; CurrentPage = 1; } }
        private void Select(GSTReturnRunModel r) => Selected = r;
        private void OpenViewOffcanvas(GSTReturnRunModel r) => SelectedView = r;

        private async Task NavigateToDetail()
        {
            if (SelectedView != null)
            {
                await JS.InvokeVoidAsync("blazorOffcanvas.hide", "viewGSTReturnRunOffcanvas");
                await Task.Delay(350);
                Nav.NavigateTo($"/gst-return-runs/{SelectedView.Id}/view");
            }
        }

        private void OpenAction(GSTReturnRunModel r, string action)
        {
            Selected = r; ActionName = action; ActionReason = ""; NeedReason = action == "Reopen";
            (ActionTitle, ActionMsg, ActionIcon, ActionBg, ActionTxt, ActionBtn) = action switch
            {
                "Generate" => ("Generate Dataset", "Generate dataset for", "ti ti-player-play", "bg-info-transparent", "text-info", "btn-info"),
                "Review" => ("Mark Reviewed", "Mark as reviewed", "ti ti-search", "bg-primary-transparent", "text-primary", "btn-primary"),
                "Approve" => ("Approve Run", "Approve", "ti ti-check", "bg-success-transparent", "text-success", "btn-success"),
                "Finalize" => ("Finalize Run", "Finalize and lock", "ti ti-lock", "bg-success-transparent", "text-success", "btn-success"),
                "MarkFiled" => ("Mark as Filed", "Mark as filed", "ti ti-file-check", "bg-success-transparent", "text-success", "btn-success"),
                "Reopen" => ("Reopen Run", "Reopen", "ti ti-rotate", "bg-warning-transparent", "text-warning", "btn-warning"),
                "Close" => ("Close Run", "Close", "ti ti-lock", "bg-dark bg-opacity-10", "", "btn-dark"),
                "Delete" => ("Delete Run", "Delete", "ti ti-trash", "bg-danger-transparent", "text-danger", "btn-danger"),
                _ => ("Action", "", "ti ti-check", "bg-secondary-transparent", "", "btn-secondary")
            };
        }

        private async Task ConfirmAction()
        {
            if (Selected == null) return;
            try
            {
                switch (ActionName)
                {
                    case "Generate": RunService.Generate(Selected.Id); break;
                    case "Review": RunService.Review(Selected.Id); break;
                    case "Approve": RunService.Approve(Selected.Id); break;
                    case "Finalize": RunService.Finalize(Selected.Id); break;
                    case "MarkFiled": RunService.MarkFiled(Selected.Id, DateTime.Now, null); break;
                    case "Reopen": RunService.Reopen(Selected.Id, ActionReason); break;
                    case "Close": RunService.Close(Selected.Id); break;
                    case "Delete": RunService.Delete(Selected.Id); break;
                }
                ToastService.ShowSuccess($"{Selected.ReturnRunNumber} — {ActionName} successful.");
                await LoadData();
            }
            catch (Exception ex) { ToastService.ShowError(ex.Message); }
        }

        private static string GetStatusBadge(string s) => s switch
        {
            "Draft" => "bg-secondary-transparent text-secondary",
            "Generated" => "bg-info-transparent",
            "Reviewed" => "bg-primary-transparent",
            "Approved" => "bg-primary-transparent",
            "Finalized" => "bg-success-transparent",
            "Filed" => "bg-success-transparent",
            "Closed" => "bg-dark-transparent",
            "Reopened" => "bg-warning-transparent",
            "Cancelled" => "bg-danger-transparent",
            _ => "bg-secondary-transparent text-secondary"
        };
        private static string GetStatusIcon(string s) => s switch { "Draft" => "ti ti-file-text", "Generated" => "ti ti-player-play", "Reviewed" => "ti ti-search", "Approved" => "ti ti-check", "Finalized" => "ti ti-lock", "Filed" => "ti ti-file-check", "Closed" => "ti ti-lock", "Reopened" => "ti ti-rotate", "Cancelled" => "ti ti-x", _ => "ti ti-info-circle" };
        private static string GetFilingBadge(string s) => s switch
        {
            "NotFiled" => "bg-secondary-transparent text-secondary",
            "Prepared" => "bg-info-transparent",
            "Filed" => "bg-success-transparent",
            "Acknowledged" => "bg-success-transparent",
            "Rejected" => "bg-danger-transparent",
            _ => "bg-secondary-transparent text-secondary"
        };
        private static string GetFilingIcon(string s) => s switch { "NotFiled" => "ti ti-file-text", "Prepared" => "ti ti-file-export", "Filed" => "ti ti-file-check", "Acknowledged" => "ti ti-circle-check", "Rejected" => "ti ti-x", _ => "ti ti-info-circle" };
    }
}
