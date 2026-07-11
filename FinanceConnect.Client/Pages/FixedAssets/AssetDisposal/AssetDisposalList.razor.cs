using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.FixedAssets.AssetDisposal
{
    public partial class AssetDisposalList
    {

        List<AssetDisposalViewModel> Disposals = new();
        List<AssetDisposalViewModel> Filtered = new();
        AssetDisposalViewModel? SelectedDisposal = new();

        bool isInitialized = false;
        bool isLoading = false;

        string searchText = "";

        AssetDisposalStatus? selectedStatus;
        AssetDisposalType? selectedType;

        int PageSize = 10;
        int CurrentPage = 1;
        int PageWindowSize = 2;
        private int VisibleColumnCount;
        int TotalPages =>
        Filtered.Count == 0
        ? 1
        : (int)Math.Ceiling((double)Filtered.Count / PageSize);

        List<AssetDisposalViewModel> PagedDisposals =>
        Filtered
        .Skip((CurrentPage - 1) * PageSize)
        .Take(PageSize)
        .ToList();


        IEnumerable<int> VisiblePages
        {
            get
            {
                if (TotalPages <= PageWindowSize)
                    return Enumerable.Range(1, TotalPages);

                int start = Math.Max(1, CurrentPage - (PageWindowSize / 2));
                int end = start + PageWindowSize - 1;

                if (end > TotalPages)
                {
                    end = TotalPages;
                    start = end - PageWindowSize + 1;
                }

                return Enumerable.Range(start, end - start + 1);
            }
        }

        AssetDisposalStatus? SelectedStatus
        {
            get => selectedStatus;
            set
            {
                selectedStatus = value;
                ApplyFilters();
            }
        }

        AssetDisposalType? SelectedType
        {
            get => selectedType;
            set
            {
                selectedType = value;
                ApplyFilters();
            }
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
            VisibleColumnCount =
            await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        protected override void OnInitialized()
        {
            Disposals = DisposalService.GetAll();
            Filtered = Disposals;

            isInitialized = true;
        }

        void OnSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString() ?? "";
            ApplyFilters();
        }

        void ApplyFilters()
        {

            var query = Disposals.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(x =>
                x.DisposalNumber.Contains(searchText,
                StringComparison.OrdinalIgnoreCase));
            }

            if (selectedStatus.HasValue)
            {
                query = query.Where(x => x.DisposalStatus == selectedStatus);
            }

            if (selectedType.HasValue)
            {
                query = query.Where(x => x.DisposalType == selectedType);
            }

            Filtered = query.ToList();
            CurrentPage = 1;
        }


        // PAGINATION
        void OnPageSizeChange(ChangeEventArgs e)
        {
            PageSize = int.Parse(e.Value!.ToString()!);
            CurrentPage = 1;
        }

        void PreviousPage()
        {
            if (CurrentPage > 1)
                CurrentPage--;
        }

        void NextPage()
        {
            if (CurrentPage < TotalPages)
                CurrentPage++;
        }

        void GoToPage(int page)
        {
            if (page < 1 || page > TotalPages)
                return;

            CurrentPage = page;
        }

        void Submit(AssetDisposalViewModel model)
        {
            DisposalService.Submit(model.AssetDisposalId);
            ToastService.ShowSuccess("Asset disposal submitted", "Submitted");
            Load();
        }

        void Approve(AssetDisposalViewModel model)
        {
            DisposalService.Approve(model.AssetDisposalId);
            ToastService.ShowSuccess("Asset disposal approved", "Approved");
            Load();
        }

        void Post(AssetDisposalViewModel model)
        {
            DisposalService.Post(model.AssetDisposalId);
            ToastService.ShowSuccess("Asset disposal posted", "Posted");
            Load();
        }

        void ViewDisposal(AssetDisposalViewModel model)
        {
            Nav.NavigateTo($"/asset-disposals/{model.AssetDisposalId}/view");
        }
        void openDisposal(AssetDisposalViewModel disposal)
        {
            SelectedDisposal = disposal;
        }

        void Load()
        {
            Disposals = DisposalService.GetAll();
            ApplyFilters();
        }

        private void OnRefresh()
        {
            searchText = "";
            selectedStatus = null;
            selectedType = null;

        }

        private async Task OnRefreshAsync()
        {
            isLoading = true;
            StateHasChanged();

            await Task.Delay(300);
            OnRefresh();
            DisposalService.ResetToSeed();
            Load();

            isLoading = false;
            await JS.InvokeVoidAsync("feather.replace");
            ToastService.ShowInfo("Asset disposal list refreshed", "Refreshed");
        }

        string GetStatusBadge(AssetDisposalStatus status)
        {
            return status switch
            {
                AssetDisposalStatus.Draft => "bg-secondary-transparent text-dark",
                AssetDisposalStatus.Submitted => "bg-warning-transparent",
                AssetDisposalStatus.Approved => "bg-primary-transparent",
                AssetDisposalStatus.Posted => "bg-success-transparent",
                _ => "bg-secondary"
            };
        }

    }
}
