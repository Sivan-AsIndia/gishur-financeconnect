using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.FixedAssets.DepreciationRun
{
    public partial class DepreciationRunLines
    {

        [Parameter] public Guid RunId { get; set; }

        private bool isInitialized;

        private DepreciationRunViewModel? Run;

        private List<DepreciationRunLineViewModel> Lines = new();
        DepreciationRunLineViewModel? SelectedRunLine = new();
        string searchText = "";


        List<DepreciationRunLineViewModel> AllLines = new();

        List<DepreciationRunLineViewModel> FilteredLines = new();

        DepreciationRunLineStatus? selectedStatus;

        int PageSize = 10;
        int CurrentPage = 1;
        int PageWindowSize = 2;
        private int VisibleColumnCount;
        int TotalPages =>
        FilteredLines.Count == 0
        ? 1
        : (int)Math.Ceiling((double)FilteredLines.Count / PageSize);
        List<DepreciationRunLineViewModel> PagedRunLines =>
        FilteredLines
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


        DepreciationRunLineStatus? SelectedStatus
        {
            get => selectedStatus;
            set
            {
                selectedStatus = value;
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
            Run = RunService.GetById(RunId);

            if (Run != null)
                Lines = LineService.GetByRunId(RunId);

            isInitialized = true;
            ApplyFilters();
        }


        async Task OnSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString() ?? "";
            ApplyFilters();
        }
        void ApplyFilters()
        {
            IEnumerable<DepreciationRunLineViewModel> query = Lines;
            // Search filter
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(t =>
                    (t.AssetNumberSnapshot?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false)
                    ||
                    (t.AssetNameSnapshot?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            // Status filter
            if (selectedStatus.HasValue)
            {
                query = query.Where(x => x.LineStatus == selectedStatus.Value);
            }

            FilteredLines = query
                .OrderBy(x => x.LineNumber)
                .ToList();

            CurrentPage = 1;
        }

        void ViewRunLine(DepreciationRunLineViewModel run)
        {
            SelectedRunLine = run;
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


        private async Task ExcludeLine(DepreciationRunLineViewModel line)
        {
            await LineService.ExcludeAsync(line.DepreciationRunLineId, "Manual exclusion");

            ToastService.ShowSuccess("Line excluded");

            Lines = LineService.GetByRunId(RunId);
        }



        private async Task IncludeLine(DepreciationRunLineViewModel line)
        {
            await LineService.IncludeAsync(line.DepreciationRunLineId);

            ToastService.ShowSuccess("Line included");

            Lines = LineService.GetByRunId(RunId);
        }



        private void ViewLine(DepreciationRunLineViewModel line)
        {
            Nav.NavigateTo($"/fixed-assets/depreciation-run-lines/{line.DepreciationRunLineId}/view");
        }



        private string GetLineStatusBadge(DepreciationRunLineStatus status)
        {

            return status switch
            {
                DepreciationRunLineStatus.Generated => "bg-primary-transparent text-primary",
                DepreciationRunLineStatus.Posted => "bg-success-transparent text-success",
                DepreciationRunLineStatus.Excluded => "bg-danger-transparent text-danger",
                DepreciationRunLineStatus.Error => "bg-warning-transparent text-dark",
                _ => "bg-secondary"
            };
        }


        private string GetRunStatusBadge(DepreciationRunStatus status)
        {

            return status switch
            {
                DepreciationRunStatus.Generated => "bg-primary-transparent text-primary",
                DepreciationRunStatus.Posted => "bg-success-transparent text-success",
                DepreciationRunStatus.Draft => "bg-secondary-transparent text-secondary",
                _ => "bg-secondary"
            };
        }

    }
}
