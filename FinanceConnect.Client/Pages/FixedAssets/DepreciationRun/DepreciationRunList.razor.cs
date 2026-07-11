using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static FinanceConnect.Client.Data.MasterDataIds;

namespace FinanceConnect.Client.Pages.FixedAssets.DepreciationRun
{
    public partial class DepreciationRunList
    {
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] DepreciationRunLineService DepreciationRunLineService { get; set; } = default!;

        Guid TenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        List<DepreciationRunViewModel> Runs = new();
        List<DepreciationRunViewModel> Filtered = new();
        DepreciationRunViewModel? SelectedRun = new();

        bool isInitialized = false;
        bool isLoading = false;

        string searchText = "";
        DepreciationRunStatus? selectedStatus;
        DepreciationRunType? selectedRunType;

        Guid? selectedCompany = null;
        public List<CompanyModel> Companies = new();

        int PageSize = 10;
        int CurrentPage = 1;
        int PageWindowSize = 2;
        private int VisibleColumnCount;
        int TotalPages =>
        Filtered.Count == 0
        ? 1
        : (int)Math.Ceiling((double)Filtered.Count / PageSize);
        List<DepreciationRunViewModel> PagedRuns =>
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

        DepreciationRunStatus? SelectedStatus
        {
            get => selectedStatus;
            set
            {
                selectedStatus = value;
                ApplyFilters();
            }
        }

        DepreciationRunType? SelectedRunType
        {
            get => selectedRunType;
            set
            {
                selectedRunType = value;
                ApplyFilters();
            }
        }

        Guid? SelectedCompany
        {
            get => selectedCompany;
            set
            {
                selectedCompany = value;
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
            Companies = MasterDataService.GetAllCompanies().Where(c => c.Status == "Active").ToList();
            Runs = RunService.GetAll();
            Filtered = Runs;
            isInitialized = true;
        }

        async Task OnSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString() ?? "";
            ApplyFilters();
        }

        void ApplyFilters()
        {
            var query = Runs.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(t =>
                    (t.RunNumber?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            // Company filter
            if (selectedCompany.HasValue)
            {
                query = query.Where(b =>
                     b.CompanyId != null &&
                     b.CompanyId == selectedCompany.Value);
            }
            if (selectedStatus.HasValue)
            {
                query = query.Where(x => x.RunStatus == selectedStatus);
            }

            if (selectedRunType.HasValue)
            {
                query = query.Where(x => x.RunType == selectedRunType);
            }

            Filtered = query
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToList();

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
        
        void ViewRun(DepreciationRunViewModel run)
        {
            SelectedRun = run;
        }
        void runDetails(DepreciationRunViewModel run)
        {
            SelectedRun = run;
            Nav.NavigateTo($"/depreciation-runs/{run.DepreciationRunId}/view");
        }

        async Task GenerateRun(DepreciationRunViewModel run)
        {
            try
            {
                DepreciationRunLineService.Generate(run.DepreciationRunId);

                ToastService.ShowSuccess("Depreciation run generated successfully");

                LoadDepreciationRun();
            }
            catch (Exception ex)
            {
                ToastService.ShowError(ex.Message);
            }
        }

        void SubmitRun(DepreciationRunViewModel run)
        {
            RunService.Submit(run.DepreciationRunId);
            ToastService.ShowSuccess("Run submitted");
            LoadDepreciationRun();
        }

        void ApproveRun(DepreciationRunViewModel run)
        {
            RunService.Approve(run.DepreciationRunId);
            ToastService.ShowSuccess("Run approved");
            LoadDepreciationRun();
        }

        void PostRun(DepreciationRunViewModel run)
        {
            RunService.Post(run.DepreciationRunId);
            ToastService.ShowSuccess("Run posted");
            LoadDepreciationRun();
        }

        void FinalizeRun(DepreciationRunViewModel run)
        {
            RunService.Finalize(run.DepreciationRunId);
            ToastService.ShowSuccess("Run finalized");
            LoadDepreciationRun();
        }

        void LoadDepreciationRun()
        {
            Runs = RunService.GetAll();
            ApplyFilters();
        }

        void OnRefresh()
        {
            searchText = "";
            selectedStatus = null;
            selectedRunType = null;
            selectedCompany = null;
        }

        private async Task OnRefreshAsync()
        {
            isLoading = true;
            StateHasChanged();

            await Task.Delay(300);
            OnRefresh();
            RunService.ResetToSeed();
            LoadDepreciationRun();

            isLoading = false;
            await JS.InvokeVoidAsync("feather.replace");
            ToastService.ShowInfo("Depreciation list refreshed", "Refreshed");
        }


        string GetStatusBadge(DepreciationRunStatus status)
        {
            return status switch
            {
                DepreciationRunStatus.Draft => "bg-secondary-transparent text-secondary",
                DepreciationRunStatus.Generated => "bg-info-transparent text-info",
                DepreciationRunStatus.Submitted => "bg-warning-transparent text-warning",
                DepreciationRunStatus.Approved => "bg-primary-transparent text-primary",
                DepreciationRunStatus.Posted=> "bg-success-transparent text-success",
                DepreciationRunStatus.Finalized => "bg-dark-transparent text-dark",
                _ => "bg-secondary"
            };
        }

    }
}
