using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.ExpenseViewModel;
using ExpenseModel = FinanceConnect.Client.ViewModels.ExpenseViewModel.Expense;

namespace FinanceConnect.Client.Pages.Revenue_Expense.Expense
{
    public partial class ExpenseList : ComponentBase
    {
        [Inject] private ExpenseService Service { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;
        private int VisibleColumnCount;
        private List<ExpenseModel> AllItems = new();
        private List<ExpenseModel> FilteredItems = new();
        private List<ExpenseModel> PagedItems = new();
        private ExpenseModel? SelectedItem;
        private string searchText = string.Empty;
        private string SelectedStatus = string.Empty;
        private string SelectedType = string.Empty;
        private int CurrentPage = 1, PageSize = 10;
        private int TotalPages => FilteredItems.Count == 0 ? 1 : (int)Math.Ceiling(FilteredItems.Count / (double)PageSize);
        private const int PageWindowSize = 5;
        private IEnumerable<int> VisiblePages
        {
            get
            {
                if (TotalPages <= PageWindowSize)
                    return Enumerable.Range(1, TotalPages);
                int start = Math.Max(1, CurrentPage - PageWindowSize / 2);
                int end = start + PageWindowSize - 1;
                if (end > TotalPages) { end = TotalPages; start = end - PageWindowSize + 1; }
                return Enumerable.Range(start, end - start + 1);
            }
        }

        protected override async Task OnInitializedAsync() { AllItems = await Service.GetAllAsync(); ApplyFilters(); }
        protected override async Task OnAfterRenderAsync(bool f) { await JS.InvokeVoidAsync("feather.replace"); await JS.InvokeVoidAsync("initTooltips"); VisibleColumnCount =
await JS.InvokeAsync<int>("getVisibleTableColumns");
        }
        private async Task OnRefreshAsync() { searchText = ""; SelectedStatus = ""; SelectedType = ""; CurrentPage = 1; AllItems = await Service.GetAllAsync(); ApplyFilters(); }
        private void OnSearch(ChangeEventArgs e) { searchText = e.Value?.ToString() ?? ""; CurrentPage = 1; ApplyFilters(); }
        private void OnFilterChanged(ChangeEventArgs e) { CurrentPage = 1; ApplyFilters(); }

        private void ApplyFilters()
        {
            var q = AllItems.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(searchText)) { var t = searchText.Trim().ToLowerInvariant(); q = q.Where(x => (x.ExpenseCode?.ToLowerInvariant().Contains(t) ?? false) || (x.ExpenseTitle?.ToLowerInvariant().Contains(t) ?? false) || (x.PayeeNameSnapshot?.ToLowerInvariant().Contains(t) ?? false)); }
            if (!string.IsNullOrEmpty(SelectedStatus) && int.TryParse(SelectedStatus, out var si) && Enum.IsDefined(typeof(ExpenseStatusEnum), si)) q = q.Where(x => x.ExpenseStatus == (ExpenseStatusEnum)si);
            if (!string.IsNullOrEmpty(SelectedType) && int.TryParse(SelectedType, out var ti) && Enum.IsDefined(typeof(ExpenseTypeEnum), ti)) q = q.Where(x => x.ExpenseType == (ExpenseTypeEnum)ti);
            FilteredItems = q.ToList(); UpdatePaged();
        }

        private void UpdatePaged() => PagedItems = FilteredItems.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
        private void OnPageSizeChange(ChangeEventArgs e) { PageSize = int.Parse(e.Value!.ToString()!); CurrentPage = 1; UpdatePaged(); }
        private void PreviousPage() { if (CurrentPage > 1) { CurrentPage--; UpdatePaged(); } }
        private void NextPage() { if (CurrentPage < TotalPages) { CurrentPage++; UpdatePaged(); } }
        private void GoToPage(int p) { CurrentPage = p; UpdatePaged(); }

        private async Task ConfirmDelete(Guid id) { try { await Service.DeleteAsync(id); AllItems.RemoveAll(x => x.ExpenseId == id); ApplyFilters(); ToastService.ShowSuccess("Expense deleted."); } catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message); } }

        private static string GetStatusDot(ExpenseStatusEnum s) => s switch { ExpenseStatusEnum.Draft => "bg-warning", ExpenseStatusEnum.Submitted => "bg-info", ExpenseStatusEnum.UnderReview => "bg-info", ExpenseStatusEnum.Approved => "bg-primary", ExpenseStatusEnum.Posted => "bg-success", ExpenseStatusEnum.PartiallyPosted => "bg-warning", ExpenseStatusEnum.Rejected => "bg-danger", ExpenseStatusEnum.Cancelled => "bg-secondary", ExpenseStatusEnum.Closed => "bg-success", _ => "bg-secondary" };
        private static string GetStatusBadge(ExpenseStatusEnum s) => s switch { ExpenseStatusEnum.Draft => "bg-warning-transparent", ExpenseStatusEnum.Submitted => "bg-info-transparent", ExpenseStatusEnum.UnderReview => "bg-info-transparent", ExpenseStatusEnum.Approved => "bg-primary-transparent", ExpenseStatusEnum.Posted => "bg-success-transparent", ExpenseStatusEnum.PartiallyPosted => "bg-warning-transparent", ExpenseStatusEnum.Rejected => "bg-danger-transparent", ExpenseStatusEnum.Cancelled => "bg-secondary-transparent", ExpenseStatusEnum.Closed => "bg-success-transparent", _ => "bg-light" };
        private static string GetTypeBadge(ExpenseTypeEnum t) => t switch { ExpenseTypeEnum.Supplier => "bg-info-transparent", ExpenseTypeEnum.EmployeeReimbursement => "bg-success-transparent", ExpenseTypeEnum.CompanyCard => "bg-primary-transparent", ExpenseTypeEnum.Cash => "bg-warning-transparent", ExpenseTypeEnum.AccrualOnly => "bg-purple-transparent text-purple", ExpenseTypeEnum.Prepayment => "bg-danger-transparent", ExpenseTypeEnum.ManualAdjustment => "bg-secondary-transparent", _ => "bg-light" };
    }
}
