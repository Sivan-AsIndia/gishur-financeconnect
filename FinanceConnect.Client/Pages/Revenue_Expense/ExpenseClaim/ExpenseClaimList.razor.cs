using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.ExpenseClaimViewModel;
using ExpenseClaimModel = FinanceConnect.Client.ViewModels.ExpenseClaimViewModel.ExpenseClaim;

namespace FinanceConnect.Client.Pages.Revenue_Expense.ExpenseClaim
{
    public partial class ExpenseClaimList : ComponentBase
    {
        [Inject] private ExpenseClaimService Service { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;

        private List<ExpenseClaimModel> AllItems = new(), FilteredItems = new(), PagedItems = new();
        private ExpenseClaimModel? SelectedItem;
        private string searchText = "", SelectedStatus = "", SelectedReimb = "";
        private int CurrentPage = 1, PageSize = 10;
        private int TotalPages => FilteredItems.Count == 0 ? 1 : (int)Math.Ceiling(FilteredItems.Count / (double)PageSize);
        private const int PageWindowSize = 5;
        private int VisibleColumnCount;
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
        protected override async Task OnAfterRenderAsync(bool f) { await JS.InvokeVoidAsync("feather.replace");
            VisibleColumnCount =
await JS.InvokeAsync<int>("getVisibleTableColumns");
            await JS.InvokeVoidAsync("initTooltips"); }
        private async Task OnRefreshAsync() { searchText = ""; SelectedStatus = ""; SelectedReimb = ""; CurrentPage = 1; AllItems = await Service.GetAllAsync(); ApplyFilters(); }
        private void OnSearch(ChangeEventArgs e) { searchText = e.Value?.ToString() ?? ""; CurrentPage = 1; ApplyFilters(); }
        private void OnFilterChanged(ChangeEventArgs e) { CurrentPage = 1; ApplyFilters(); }

        private void ApplyFilters()
        {
            var q = AllItems.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(searchText)) { var t = searchText.Trim().ToLowerInvariant(); q = q.Where(x => (x.ClaimCode?.ToLowerInvariant().Contains(t) ?? false) || (x.ClaimTitle?.ToLowerInvariant().Contains(t) ?? false) || (x.ClaimantNameSnapshot?.ToLowerInvariant().Contains(t) ?? false)); }
            if (!string.IsNullOrEmpty(SelectedStatus) && int.TryParse(SelectedStatus, out var si) && Enum.IsDefined(typeof(ClaimStatusEnum), si)) q = q.Where(x => x.ClaimStatus == (ClaimStatusEnum)si);
            if (!string.IsNullOrEmpty(SelectedReimb) && int.TryParse(SelectedReimb, out var ri) && Enum.IsDefined(typeof(ReimbursementStatusEnum), ri)) q = q.Where(x => x.ReimbursementStatus == (ReimbursementStatusEnum)ri);
            FilteredItems = q.ToList(); UpdatePaged();
        }
        private void UpdatePaged() => PagedItems = FilteredItems.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
        private void OnPageSizeChange(ChangeEventArgs e) { PageSize = int.Parse(e.Value!.ToString()!); CurrentPage = 1; UpdatePaged(); }
        private void PreviousPage() { if (CurrentPage > 1) { CurrentPage--; UpdatePaged(); } }
        private void NextPage() { if (CurrentPage < TotalPages) { CurrentPage++; UpdatePaged(); } }
        private void GoToPage(int p) { CurrentPage = p; UpdatePaged(); }
        private async Task ConfirmDelete(Guid id) { try { await Service.DeleteAsync(id); AllItems.RemoveAll(x => x.ExpenseClaimId == id); ApplyFilters(); ToastService.ShowSuccess("Claim deleted."); } catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message); } }

        private static string GetStatusDot(ClaimStatusEnum s) => s switch { ClaimStatusEnum.Draft => "bg-warning", ClaimStatusEnum.Submitted => "bg-info", ClaimStatusEnum.UnderReview => "bg-info", ClaimStatusEnum.Approved => "bg-primary", ClaimStatusEnum.PartiallyApproved => "bg-warning", ClaimStatusEnum.Rejected => "bg-danger", ClaimStatusEnum.Reimbursed => "bg-success", ClaimStatusEnum.PartiallyReimbursed => "bg-warning", ClaimStatusEnum.Cancelled => "bg-secondary", ClaimStatusEnum.Closed => "bg-success", _ => "bg-secondary" };
        private static string GetStatusBadge(ClaimStatusEnum s) => s switch { ClaimStatusEnum.Draft => "bg-warning-transparent", ClaimStatusEnum.Submitted => "bg-info-transparent", ClaimStatusEnum.UnderReview => "bg-info-transparent", ClaimStatusEnum.Approved => "bg-primary-transparent", ClaimStatusEnum.PartiallyApproved => "bg-warning-transparent", ClaimStatusEnum.Rejected => "bg-danger-transparent", ClaimStatusEnum.Reimbursed => "bg-success-transparent", ClaimStatusEnum.PartiallyReimbursed => "bg-warning-transparent", ClaimStatusEnum.Cancelled => "bg-secondary-transparent", ClaimStatusEnum.Closed => "bg-success-transparent", _ => "bg-light" };
        private static string GetReimbBadge(ReimbursementStatusEnum s) => s switch { ReimbursementStatusEnum.FullyReimbursed => "bg-success-transparent", ReimbursementStatusEnum.PartiallyReimbursed => "bg-warning-transparent", ReimbursementStatusEnum.Pending => "bg-info-transparent", ReimbursementStatusEnum.OnHold => "bg-danger-transparent", _ => "bg-secondary-transparent" };
    }
}
