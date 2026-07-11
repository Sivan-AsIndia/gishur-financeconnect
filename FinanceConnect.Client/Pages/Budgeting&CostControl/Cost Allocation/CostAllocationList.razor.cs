using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
namespace FinanceConnect.Client.Pages.Budgeting_CostControl.Cost_Allocation
{
    public partial class CostAllocationList
    {
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private CostAllocationService Service { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;

        private List<CostAllocationListDto> AllItems = new();
        private List<CostAllocationListDto> FilteredItems = new();
        private List<CostAllocationListDto> PagedItems = new();
        private CostAllocationListDto? SelectedItem;

        private string searchText = "";
        private string SelectedStatus = "";
        private string SelectedType = "";
        private string SelectedMethod = "";

        private int CurrentPage = 1;
        private int PageSize = 10;

        private int TotalPages => FilteredItems.Count == 0 ? 1 : (int)Math.Ceiling((double)FilteredItems.Count / PageSize);
        private int StartPage => Math.Max(1, CurrentPage - 2);
        private int EndPage => Math.Min(TotalPages, StartPage + 4);

        protected override async Task OnInitializedAsync()
        {
            AllItems = await Service.GetAllAsync();
            ApplyFilters();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
        }

        private void ApplyFilters()
        {
            var q = AllItems.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var t = searchText.Trim().ToLowerInvariant();
                q = q.Where(x =>
                    (x.AllocationCode?.ToLowerInvariant().Contains(t) ?? false) ||
                    (x.AllocationName?.ToLowerInvariant().Contains(t) ?? false));
            }

            if (!string.IsNullOrEmpty(SelectedStatus) && int.TryParse(SelectedStatus, out var s))
                q = q.Where(x => (int)x.AllocationStatus == s);

            if (!string.IsNullOrEmpty(SelectedType) && int.TryParse(SelectedType, out var tp))
                q = q.Where(x => (int)x.AllocationType == tp);

            if (!string.IsNullOrEmpty(SelectedMethod) && int.TryParse(SelectedMethod, out var m))
                q = q.Where(x => (int)x.AllocationMethod == m);

            FilteredItems = q.OrderByDescending(x => x.AllocationDate).ToList();

            if (CurrentPage > TotalPages) CurrentPage = 1;
            PagedItems = FilteredItems.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
        }

        private void OnSearch(ChangeEventArgs e) { searchText = e.Value?.ToString() ?? ""; CurrentPage = 1; ApplyFilters(); }
        private void OnFilterChanged(ChangeEventArgs e) { CurrentPage = 1; ApplyFilters(); }
        private void GoToPage(int p) { if (p >= 1 && p <= TotalPages) { CurrentPage = p; ApplyFilters(); } }
        private void PreviousPage() { if (CurrentPage > 1) GoToPage(CurrentPage - 1); }
        private void NextPage() { if (CurrentPage < TotalPages) GoToPage(CurrentPage + 1); }
        private Task OnPageSizeChange(ChangeEventArgs e)
        {
            PageSize = int.Parse(e.Value!.ToString()!);
            CurrentPage = 1;
            return Task.CompletedTask;
        }

        private async Task OnRefreshAsync()
        {
            searchText = ""; SelectedStatus = ""; SelectedType = ""; SelectedMethod = "";
            CurrentPage = 1;
            AllItems = await Service.GetAllAsync();
            ApplyFilters();
        }

        private void ConfirmDelete(Guid id)
        {
            try
            {
                Service.DeleteAsync(id);
                ToastService.ShowSuccess("Allocation deleted successfully.", "Success");
                AllItems = Service.GetAll().Select(Data.CostAllocationSeedData.ToListDto).ToList();
                ApplyFilters();
            }
            catch (InvalidOperationException ex)
            {
                ToastService.ShowError(ex.Message, "Error");
            }
        }

        private string GetStatusDot(AllocationStatus s) => s switch
        {
            AllocationStatus.Draft => "bg-secondary",
            AllocationStatus.Prepared => "bg-info",
            AllocationStatus.Submitted => "bg-warning",
            AllocationStatus.Approved => "bg-success",
            AllocationStatus.Applied => "bg-success",
            AllocationStatus.Locked => "bg-danger",
            AllocationStatus.Closed => "bg-dark",
            AllocationStatus.Reversed => "bg-secondary",
            AllocationStatus.Archived => "bg-dark",
            _ => "bg-secondary"
        };
    }
}
