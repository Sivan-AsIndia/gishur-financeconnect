using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.TaxCategoryMappingViewModel;

namespace FinanceConnect.Client.Pages.Tax_Statutory.TaxCategoryMapping
{
    public partial class TaxCategoryMapping
    {
        [Inject] private TaxCategoryMappingService MappingService { get; set; } = default!;
        [Inject] private ToastService              ToastService   { get; set; } = default!;
        [Inject] private IJSRuntime JS             { get; set; } = default!;

        private List<TaxCategoryMappingListDto> AllItems      { get; set; } = new();
        private List<TaxCategoryMappingListDto> FilteredItems { get; set; } = new();
        private List<TaxCategoryMappingListDto> PagedItems    { get; set; } = new();
        private TaxCategoryMappingListDto? SelectedItem;

        private bool   isInitialized  = false;
        private bool   isLoading      = false;
        private string searchText     = string.Empty;
        private string SelectedStatus  = string.Empty;
        private string SelectedScope   = string.Empty;
        private string SelectedContext = string.Empty;

        private int CurrentPage { get; set; } = 1;
        private int PageSize    { get; set; } = 10;

        private int TotalPages => FilteredItems.Count == 0
            ? 1
            : (int)Math.Ceiling(FilteredItems.Count / (double)PageSize);

        private IEnumerable<int> VisiblePages
        {
            get
            {
                int start = Math.Max(1, CurrentPage - 2);
                int end   = Math.Min(TotalPages, start + 4);
                return Enumerable.Range(start, end - start + 1);
            }
        }

        protected override async Task OnInitializedAsync()
        {
            AllItems = await MappingService.GetAllAsync();
            ApplyFilters();
            isInitialized = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
        }

        private async Task OnRefreshAsync()
        {
            searchText = SelectedStatus = SelectedScope = SelectedContext = string.Empty;
            CurrentPage = 1;
            AllItems    = await MappingService.GetAllAsync();
            ApplyFilters();
            await JS.InvokeVoidAsync("feather.replace");
        }

        private void OnSearch(ChangeEventArgs e)
        {
            searchText  = e.Value?.ToString() ?? "";
            CurrentPage = 1;
            ApplyFilters();
        }

        private void OnFilterChanged(ChangeEventArgs e)
        {
            CurrentPage = 1;
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var q = AllItems.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var t = searchText.Trim().ToLowerInvariant();
                q = q.Where(x =>
                    x.MappingCode.ToLowerInvariant().Contains(t) ||
                    x.MappingName.ToLowerInvariant().Contains(t));
            }

            if (!string.IsNullOrEmpty(SelectedStatus) && int.TryParse(SelectedStatus, out var si))
                q = q.Where(x => (int)x.MappingStatus == si);

            if (!string.IsNullOrEmpty(SelectedScope) && int.TryParse(SelectedScope, out var sc))
                q = q.Where(x => (int)x.TaxTypeScope == sc);

            if (!string.IsNullOrEmpty(SelectedContext) && int.TryParse(SelectedContext, out var cx))
                q = q.Where(x => (int)x.TransactionContext == cx);

            FilteredItems = q.OrderByDescending(x => x.EffectiveFrom).ToList();
            UpdatePaged();
        }

        private void UpdatePaged()
            => PagedItems = FilteredItems.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

        private void OnPageSizeChange(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out var s))
            { PageSize = s; CurrentPage = 1; UpdatePaged(); }
        }

        private void PreviousPage() { if (CurrentPage > 1)         { CurrentPage--; UpdatePaged(); } }
        private void NextPage()     { if (CurrentPage < TotalPages) { CurrentPage++; UpdatePaged(); } }
        private void GoToPage(int p){ CurrentPage = p; UpdatePaged(); }

        private void OpenViewModal(TaxCategoryMappingListDto item) => SelectedItem = item;

        private async Task OnActivate(Guid id)
        {
            try
            {
                await MappingService.ActivateAsync(id);
                await OnRefreshAsync();
                ToastService.ShowSuccess("Mapping activated.");
            }
            catch (Exception ex) { ToastService.ShowError(ex.Message); }
        }

        private async Task OnInactivate(Guid id)
        {
            try
            {
                await MappingService.InactivateAsync(id);
                await OnRefreshAsync();
                ToastService.ShowSuccess("Mapping inactivated.");
            }
            catch (Exception ex) { ToastService.ShowError(ex.Message); }
        }
    }
}
