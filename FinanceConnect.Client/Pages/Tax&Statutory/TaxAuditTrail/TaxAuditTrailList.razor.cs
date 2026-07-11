using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.TaxAuditTrailViewModel;

namespace FinanceConnect.Client.Pages.Tax___Statutory.TaxAuditTrail
{
    public partial class TaxAuditTrailList : ComponentBase
    {
        private bool isInitialized, isLoading;
        private List<TaxAuditTrailModel> allItems = new();
        private TaxAuditTrailModel? Selected;
        private TaxAuditTrailModel? SelectedView;
        private string searchText = "";
        private string _selectedEntityType = "", _selectedCategory = "", _selectedSeverity = "";
        private int VisibleColumnCount, CurrentPage = 1, PageSize = 10;

        public string SelectedEntityType { get => _selectedEntityType; set { _selectedEntityType = value; CurrentPage = 1; } }
        public string SelectedCategory { get => _selectedCategory; set { _selectedCategory = value; CurrentPage = 1; } }
        public string SelectedSeverity { get => _selectedSeverity; set { _selectedSeverity = value; CurrentPage = 1; } }
        private List<string> DistinctEntities => allItems.Select(x => x.EntityType ?? "").Where(x => !string.IsNullOrEmpty(x)).Distinct().OrderBy(x => x).ToList();
        private List<string> DistinctCategories => allItems.Select(x => x.EventCategory ?? "").Where(x => !string.IsNullOrEmpty(x)).Distinct().OrderBy(x => x).ToList();
        private List<string> DistinctSeverities => allItems.Select(x => x.EventSeverity ?? "").Where(x => !string.IsNullOrEmpty(x)).Distinct().OrderBy(x => x).ToList();

        private IEnumerable<TaxAuditTrailModel> FilteredItems => allItems
            .Where(x => string.IsNullOrEmpty(searchText) || (x.AuditEventNumber ?? "").Contains(searchText, StringComparison.OrdinalIgnoreCase) || (x.EntityNumberSnapshot ?? "").Contains(searchText, StringComparison.OrdinalIgnoreCase) || (x.EntityDisplayNameSnapshot ?? "").Contains(searchText, StringComparison.OrdinalIgnoreCase) || (x.ActorNameSnapshot ?? "").Contains(searchText, StringComparison.OrdinalIgnoreCase) || (x.CorrelationId ?? "").Contains(searchText, StringComparison.OrdinalIgnoreCase) || (x.ReasonText ?? "").Contains(searchText, StringComparison.OrdinalIgnoreCase))
            .Where(x => string.IsNullOrEmpty(_selectedEntityType) || x.EntityType == _selectedEntityType)
            .Where(x => string.IsNullOrEmpty(_selectedCategory) || x.EventCategory == _selectedCategory)
            .Where(x => string.IsNullOrEmpty(_selectedSeverity) || x.EventSeverity == _selectedSeverity)
            .OrderByDescending(x => x.EventTimestamp);

        private int TotalPages => Math.Max(1, (int)Math.Ceiling(FilteredItems.Count() / (double)PageSize));
        private List<TaxAuditTrailModel> PagedItems => FilteredItems.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

        protected override async Task OnInitializedAsync() { await LoadData(); isInitialized = true; }
        protected override async Task OnAfterRenderAsync(bool firstRender) { await JS.InvokeVoidAsync("feather.replace"); await JS.InvokeVoidAsync("initTooltips", true); VisibleColumnCount = await JS.InvokeAsync<int>("getVisibleTableColumns"); }
        private async Task LoadData() { isLoading = true; StateHasChanged(); await Task.Delay(100); allItems = AuditService.GetList(); isLoading = false; StateHasChanged(); }
        private async Task OnRefreshAsync() { isLoading = true; StateHasChanged(); await Task.Delay(200); searchText = ""; _selectedEntityType = _selectedCategory = _selectedSeverity = ""; CurrentPage = 1; await LoadData(); ToastService.ShowSuccess("Data refreshed"); }
        private async Task OnSearch(ChangeEventArgs e) { searchText = e.Value?.ToString() ?? ""; CurrentPage = 1; VisibleColumnCount = await JS.InvokeAsync<int>("getVisibleTableColumns"); }
        private void GoToPage(int p) { if (p >= 1 && p <= TotalPages) CurrentPage = p; }
        private void OnPageSizeChange(ChangeEventArgs e) { if (int.TryParse(e.Value?.ToString(), out var s)) { PageSize = s; CurrentPage = 1; } }
        private void Select(TaxAuditTrailModel a) => Selected = a;
        private void OpenViewOffcanvas(TaxAuditTrailModel a) => SelectedView = a;

        private static string GetCategoryBadge(string c) => c switch
        {
            "MasterData" => "bg-info-transparent",
            "Calculation" => "bg-secondary-transparent text-secondary",
            "Posting" => "bg-primary-transparent",
            "Settlement" => "bg-success-transparent",
            "ReturnPreparation" => "bg-warning-transparent text-dark",
            "Filing" => "bg-success-transparent",
            "Security" => "bg-danger-transparent",
            "Reconciliation" => "bg-info-transparent",
            "Workflow" => "bg-purple-transparent",
            _ => "bg-secondary-transparent text-secondary"
        };
        private static string GetEventBadge(string t) => t switch
        {
            "Created" => "bg-success-transparent",
            "Updated" => "bg-info-transparent",
            "Posted" => "bg-primary-transparent",
            "Reversed" => "bg-warning-transparent text-dark",
            "Settled" => "bg-success-transparent",
            "Finalized" => "bg-success-transparent",
            "Filed" => "bg-success-transparent",
            "Reopened" => "bg-warning-transparent text-dark",
            "AccessDenied" => "bg-danger-transparent",
            "ValidationFailed" => "bg-danger-transparent",
            "OverrideApplied" => "bg-warning-transparent text-dark",
            _ => "bg-secondary-transparent text-secondary"
        };
        private static string GetSeverityBadge(string s) => s switch
        {
            "Info" => "bg-info-transparent",
            "Warning" => "bg-warning-transparent",
            "High" => "bg-danger-transparent",
            "Critical" => "bg-danger-transparent",
            _ => "bg-secondary-transparent text-secondary"
        };
        private static string GetSeverityIcon(string s) => s switch { "Info" => "ti ti-info-circle", "Warning" => "ti ti-alert-triangle", "High" => "ti ti-alert-circle", "Critical" => "ti ti-shield-x", _ => "ti ti-info-circle" };
        private static string GetScopeBadge(string s) => s switch { "GST" => "bg-success-transparent", "TDS" => "bg-warning-transparent text-dark", "TCS" => "bg-purple-transparent", "Mixed" => "bg-secondary-transparent", _ => "bg-light text-muted" };
    }
}
