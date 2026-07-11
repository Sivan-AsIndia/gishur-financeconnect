using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.ExpenseCategoryViewModel;
using ExpenseCategoryModel = FinanceConnect.Client.ViewModels.ExpenseCategoryViewModel.ExpenseCategory;

namespace FinanceConnect.Client.Pages.Revenue_Expense.ExpenseCategory
{
    public partial class ExpenseCategoryList : ComponentBase
    {
        [Inject] private ExpenseCategoryService Service { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;
        private int VisibleColumnCount;
        private List<ExpenseCategoryModel> AllItems = new();
        private List<ExpenseCategoryModel> FilteredItems = new();
        private List<ExpenseCategoryModel> PagedItems = new();
        private ExpenseCategoryModel? SelectedItem;
        private string searchText = string.Empty;
        private string SelectedStatus = string.Empty;
        private string SelectedType = string.Empty;
        private int CurrentPage = 1;
        private int PageSize = 10;
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
        protected override async Task OnAfterRenderAsync(bool f) { await JS.InvokeVoidAsync("feather.replace"); await JS.InvokeVoidAsync("initTooltips");
            VisibleColumnCount =
await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        private async Task OnRefreshAsync() { searchText = ""; SelectedStatus = ""; SelectedType = ""; CurrentPage = 1; AllItems = await Service.GetAllAsync(); ApplyFilters(); }
        private void OnSearch(ChangeEventArgs e) { searchText = e.Value?.ToString() ?? ""; CurrentPage = 1; ApplyFilters(); }
        private void OnFilterChanged(ChangeEventArgs e) { CurrentPage = 1; ApplyFilters(); }

        private void ApplyFilters()
        {
            var q = AllItems.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var t = searchText.Trim().ToLowerInvariant();
                q = q.Where(c => (c.CategoryCode?.ToLowerInvariant().Contains(t) ?? false) || (c.CategoryName?.ToLowerInvariant().Contains(t) ?? false));
            }
            if (!string.IsNullOrEmpty(SelectedStatus) && int.TryParse(SelectedStatus, out var si) && Enum.IsDefined(typeof(CategoryStatusEnum), si))
                q = q.Where(c => c.CategoryStatus == (CategoryStatusEnum)si);
            if (!string.IsNullOrEmpty(SelectedType) && int.TryParse(SelectedType, out var ti) && Enum.IsDefined(typeof(CategoryTypeEnum), ti))
                q = q.Where(c => c.CategoryType == (CategoryTypeEnum)ti);
            FilteredItems = q.ToList(); UpdatePaged();
        }

        private void UpdatePaged() => PagedItems = FilteredItems.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
        private void OnPageSizeChange(ChangeEventArgs e) { PageSize = int.Parse(e.Value!.ToString()!); CurrentPage = 1; UpdatePaged(); }
        private void PreviousPage() { if (CurrentPage > 1) { CurrentPage--; UpdatePaged(); } }
        private void NextPage() { if (CurrentPage < TotalPages) { CurrentPage++; UpdatePaged(); } }
        private void GoToPage(int p) { CurrentPage = p; UpdatePaged(); }

        private async Task ConfirmDelete(Guid id)
        {
            try { await Service.DeleteAsync(id); AllItems.RemoveAll(x => x.ExpenseCategoryId == id); ApplyFilters(); ToastService.ShowSuccess("Category deleted."); }
            catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message); }
        }

        private static string GetStatusDot(CategoryStatusEnum s) => s switch { CategoryStatusEnum.Active => "bg-success", CategoryStatusEnum.Draft => "bg-warning", CategoryStatusEnum.Inactive => "bg-secondary text-secondary", CategoryStatusEnum.Archived => "bg-danger", _ => "bg-secondary text-secondary" };
        private static string GetStatusBadge(CategoryStatusEnum s) => s switch { CategoryStatusEnum.Active => "bg-success-transparent", CategoryStatusEnum.Draft => "bg-warning-transparent", CategoryStatusEnum.Inactive => "bg-secondary-transparent text-secondary", CategoryStatusEnum.Archived => "bg-danger-transparent", _ => "bg-light" };
        private static string GetTypeBadge(CategoryTypeEnum t) => t switch { 
            CategoryTypeEnum.OperatingExpense => "bg-info-transparent", 
            CategoryTypeEnum.AdministrativeExpense => "bg-warning-transparent",
            CategoryTypeEnum.SellingExpense => "bg-primary-transparent", 
            CategoryTypeEnum.EmployeeExpense => "bg-success-transparent",
            CategoryTypeEnum.ProjectExpense => "bg-purple-transparent text-purple",
            CategoryTypeEnum.FinanceExpense => "bg-danger-transparent", _ => "bg-light" };
    }
}
