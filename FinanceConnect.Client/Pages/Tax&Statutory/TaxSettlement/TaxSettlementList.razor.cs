using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.TaxSettlementViewModel;

namespace FinanceConnect.Client.Pages.Tax___Statutory.TaxSettlement
{
    public partial class TaxSettlementList : ComponentBase
    {
        private bool isInitialized, isLoading;
        private List<TaxSettlementModel> allItems = new();
        private TaxSettlementModel? Selected;
        private string searchText = "", ActionReason = "";
        private string _selectedType = "", _selectedStatus = "", _selectedScope = "";
        private int VisibleColumnCount, CurrentPage = 1, PageSize = 10;
        private string ActionName = "", ActionTitle = "", ActionMsg = "", ActionIcon = "", ActionBg = "", ActionTxt = "", ActionBtn = "";
        private bool NeedReason;

        public string SelectedType { get => _selectedType; set { _selectedType = value; CurrentPage = 1; } }
        public string SelectedStatus { get => _selectedStatus; set { _selectedStatus = value; CurrentPage = 1; } }
        public string SelectedScope { get => _selectedScope; set { _selectedScope = value; CurrentPage = 1; } }

        private List<string> DistinctTypes => allItems.Select(x => x.SettlementType).Where(x => !string.IsNullOrEmpty(x)).Distinct().OrderBy(x => x).ToList();
        private List<string> DistinctStatuses => allItems.Select(x => x.SettlementStatus).Distinct().OrderBy(x => x).ToList();
        private List<string> DistinctScopes => allItems.Select(x => x.TaxTypeScope).Where(x => !string.IsNullOrEmpty(x)).Distinct().OrderBy(x => x).ToList();

        private IEnumerable<TaxSettlementModel> FilteredItems => allItems
            .Where(x => string.IsNullOrEmpty(searchText) || (x.SettlementNumber ?? "").Contains(searchText, StringComparison.OrdinalIgnoreCase) || (x.ChallanNumber ?? "").Contains(searchText, StringComparison.OrdinalIgnoreCase) || (x.TaxPeriodKey ?? "").Contains(searchText, StringComparison.OrdinalIgnoreCase) || (x.Narration ?? "").Contains(searchText, StringComparison.OrdinalIgnoreCase))
            .Where(x => string.IsNullOrEmpty(_selectedType) || x.SettlementType == _selectedType)
            .Where(x => string.IsNullOrEmpty(_selectedStatus) || x.SettlementStatus == _selectedStatus)
            .Where(x => string.IsNullOrEmpty(_selectedScope) || x.TaxTypeScope == _selectedScope)
            .OrderByDescending(x => x.SettlementDate);

        private int TotalPages => Math.Max(1, (int)Math.Ceiling(FilteredItems.Count() / (double)PageSize));
        private List<TaxSettlementModel> PagedItems => FilteredItems.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

        protected override async Task OnInitializedAsync() { await LoadData(); isInitialized = true; }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips", true);
            VisibleColumnCount = await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        private async Task LoadData() { isLoading = true; StateHasChanged(); await Task.Delay(100); allItems = SettlementService.GetList(); isLoading = false; StateHasChanged(); }
        private async Task OnRefreshAsync() { isLoading = true; StateHasChanged(); await Task.Delay(200); searchText = ""; _selectedType = _selectedStatus = _selectedScope = ""; CurrentPage = 1; await LoadData(); ToastService.ShowSuccess("Data refreshed"); }
        private async Task OnSearch(ChangeEventArgs e) { searchText = e.Value?.ToString() ?? ""; CurrentPage = 1; VisibleColumnCount = await JS.InvokeAsync<int>("getVisibleTableColumns"); }
        private void GoToPage(int p) { if (p >= 1 && p <= TotalPages) CurrentPage = p; }
        private void OnPageSizeChange(ChangeEventArgs e) { if (int.TryParse(e.Value?.ToString(), out var s)) { PageSize = s; CurrentPage = 1; } }
        private void Select(TaxSettlementModel s) => Selected = s;
        private void OpenRowDetails(TaxSettlementModel s) => Selected = s;

        private void OpenAction(TaxSettlementModel s, string action)
        {
            Selected = s; ActionName = action; ActionReason = ""; NeedReason = action == "Reverse";
            (ActionTitle, ActionMsg, ActionIcon, ActionBg, ActionTxt, ActionBtn) = action switch
            {
                "Submit" => ("Submit Settlement", "Submit", "ti ti-send", "bg-info-transparent", "text-info", "btn-info"),
                "Approve" => ("Approve Settlement", "Approve", "ti ti-check", "bg-success-transparent", "text-success", "btn-success"),
                "Reject" => ("Reject Settlement", "Reject back to Draft", "ti ti-x", "bg-warning-transparent", "text-warning", "btn-warning"),
                "Post" => ("Post Settlement", "Post", "ti ti-file-check", "bg-success-transparent", "text-success", "btn-success"),
                "Reverse" => ("Reverse Settlement", "Reverse", "ti ti-rotate", "bg-danger-transparent", "text-danger", "btn-danger"),
                "Close" => ("Close Settlement", "Close", "ti ti-lock", "bg-dark bg-opacity-10", "", "btn-dark"),
                "Delete" => ("Delete Settlement", "Delete", "ti ti-trash", "bg-danger-transparent", "text-danger", "btn-danger"),
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
                    case "Submit": SettlementService.Submit(Selected.Id); break;
                    case "Approve": SettlementService.Approve(Selected.Id); break;
                    case "Reject": SettlementService.Reject(Selected.Id); break;
                    case "Post": SettlementService.Post(Selected.Id); break;
                    case "Reverse": SettlementService.Reverse(Selected.Id, ActionReason); break;
                    case "Close": SettlementService.Close(Selected.Id); break;
                    case "Delete": SettlementService.Delete(Selected.Id); break;
                }
                ToastService.ShowSuccess($"{Selected.SettlementNumber} — {ActionName} successful.");
                await LoadData();
            }
            catch (Exception ex) { ToastService.ShowError(ex.Message); }
        }

        private static string GetStatusBadge(string s) => s switch
        {
            "Draft" => "bg-secondary-transparent text-secondary",
            "Submitted" => "bg-info-transparent",
            "Approved" => "bg-primary-transparent",
            "Posted" => "bg-success-transparent",
            "Reconciled" => "bg-success-transparent",
            "Closed" => "bg-dark-transparent",
            "Reversed" => "bg-warning-transparent",
            "Cancelled" => "bg-danger-transparent",
            _ => "bg-secondary-transparent text-secondary"
        };
        private static string GetStatusIcon(string s) => s switch { "Draft" => "ti ti-file-text", "Submitted" => "ti ti-send", "Approved" => "ti ti-check", "Posted" => "ti ti-circle-check", "Closed" => "ti ti-lock", "Reversed" => "ti ti-rotate", "Cancelled" => "ti ti-x", _ => "ti ti-info-circle" };
        private static string GetTypeBadge(string t) => (t ?? "").StartsWith("GST") ? "bg-success-transparent" : t == "TDSRemittance" ? "bg-warning-transparent text-dark" : t == "TCSRemittance" ? "bg-purple-transparent text-purple" : "bg-info-transparent text-dark";
        private static string GetScopeBadge(string s) => s switch { "GST" => "bg-success-transparent", "TDS" => "bg-warning-transparent text-dark", "TCS" => "bg-purple-transparent text-purple", _ => "bg-secondary-transparent" };
        private static string FormatType(string t) => t switch { "GSTCashPayment" => "GST Cash", "GSTInputCreditOffset" => "GST ITC Offset", "GSTMixedSettlement" => "GST Mixed", "TDSRemittance" => "TDS Remit", "TCSRemittance" => "TCS Remit", "TaxAdjustment" => "Adjustment", _ => t ?? "" };
    }
}
