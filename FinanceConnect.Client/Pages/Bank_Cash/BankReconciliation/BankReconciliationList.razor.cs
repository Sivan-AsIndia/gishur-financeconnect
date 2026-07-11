using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Bank_Cash.BankReconciliation
{
    public partial class BankReconciliationList
    {

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips",true);
            VisibleColumnCount =
            await JS.InvokeAsync<int>("getVisibleTableColumns");
        }
        [Inject] BankAccountService BankAccountService { get; set; } = default!;

        List<BankReconciliationModel> AllRuns = new ();
        List<BankReconciliationModel> FilteredRuns = new();
        List<BankReconciliationModel> PagedRuns = new();
        BankReconciliationModel? SelectedReconciliaton;
        private BankReconciliationStatistics Statistics = new();

        private bool isInitialized = false;
        private bool isLoading = false;
        string SearchText = "";
        ReconciliationStatus? selectedStatus = null;

        private List<ReconciliationStatus> AvailableStatuses =>
            AllRuns.Select(r => r.ReconciliationStatus).Distinct().OrderBy(s => s).ToList();


        int PageSize = 10;
        int CurrentPage = 1;
        int TotalPages = 1;
        List<int> VisiblePages = new();
        private int VisibleColumnCount;

        ReconciliationStatus? SelectedStatus
        {
            get => selectedStatus;
            set
            {
                selectedStatus = value;
                ApplyFilter();
            }
        }
        protected override void OnInitialized()
        {
            LoadRuns();
        }

        void LoadRuns()
        {
            AllRuns = ReconService.GetAll();
            Statistics = ReconService.GetStatistics();
            isInitialized = true;
            ApplyFilter();
        }

        async Task OnSearch(ChangeEventArgs e)
        {
            SearchText = e.Value?.ToString() ?? "";
            ApplyFilter();
            VisibleColumnCount =
            await JS.InvokeAsync<int>("getVisibleTableColumns");
        }
        private string GetScopeBadgeClass(ScopeType? scopeType) => scopeType switch
        {
            ScopeType.AsOfDate => "bg-soft-primary text-primary",
            ScopeType.PeriodRange => "bg-soft-success text-success",
            null => "bg-soft-secondary text-secondary",
            _ => "bg-soft-secondary text-secondary"
        };

        string GetBankAccountName(Guid accountId)
        {
            var acc = BankAccountService.GetById(accountId);
            return acc?.BankAccountName ?? "—";
        }

        void OnStatusChanged(ChangeEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Value?.ToString()))
            {
                SelectedStatus = null;
            }
            else
            {
                SelectedStatus = Enum.Parse<ReconciliationStatus>(e.Value!.ToString()!);
            }

            ApplyFilter();
        }
        void ApplyFilter()
        {
            IEnumerable<BankReconciliationModel> query = AllRuns;

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                query = query.Where(r =>
                    (r.ReconciliationNumber?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false));
            }

            if (SelectedStatus.HasValue)
            {
                query = query.Where(r =>
                    r.ReconciliationStatus == SelectedStatus.Value);
            }

            FilteredRuns = query
                .OrderByDescending(r => r.PreparedOn)
                .ToList();

            CurrentPage = 1;
            UpdatePaging();
        }

        void UpdatePaging()
        {
            TotalPages = (int)Math.Ceiling((double)FilteredRuns.Count / PageSize);

            PagedRuns = FilteredRuns
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            VisiblePages = Enumerable
                .Range(Math.Max(1, CurrentPage - 2),
                       Math.Min(5, TotalPages - Math.Max(1, CurrentPage - 2) + 1))
                .ToList();
        }

        void OnPageSizeChange(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out var size))
            {
                PageSize = size;
                CurrentPage = 1;
                UpdatePaging();
            }
        }

        void PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                UpdatePaging();
            }
        }

        void NextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                UpdatePaging();
            }
        }

        void GoToPage(int page)
        {
            CurrentPage = page;
            UpdatePaging();
        }

        void OpenRun(BankReconciliationModel run)
        {
            Nav.NavigateTo($"/bank-reconciliations/{run.BankReconciliationId}/view");
        }

        void SelectedData(BankReconciliationModel run)
        {
            SelectedReconciliaton = run;
        }

        void CreateNewRun()
        {
            Nav.NavigateTo("/bank-reconciliations/create");
        }

        void Finalize(BankReconciliationModel run)
        {
            ReconService.Finalize(run.BankReconciliationId);
            Toast.ShowSuccess("Reconciliation finalized");
            LoadRuns();
        }

        BankReconciliationModel? ReopenTarget;

        void OpenReopenModal(BankReconciliationModel run)
        {
            ReopenTarget = run;
        }

        void ConfirmReopen()
        {
            if (ReopenTarget == null) return;
            ReconService.Reopen(ReopenTarget.BankReconciliationId, "Reopened from list screen");
            Toast.ShowWarning("Reconciliation reopened");
            ReopenTarget = null;
            LoadRuns();
        }

        void Reopen(BankReconciliationModel run)
        {
            ReconService.Reopen(run.BankReconciliationId, "Reopened from list screen");
            Toast.ShowWarning("Reconciliation reopened");
            LoadRuns();
        }
        private string GetScopeTitle(BankReconciliationModel r)
        {
            return r.ScopeType == ScopeType.PeriodRange
                ? $"{r.FromDate?.ToString("dd MMM yyyy")} – {r.ToDate?.ToString("dd MMM yyyy")}"
                : $"As of {r.AsOfDate?.ToString("dd MMM yyyy")}";
        }
        string GetStatusIcon(ReconciliationStatus status)
        {
            return status switch
            {
                ReconciliationStatus.Draft => "ti ti-file-text",
                ReconciliationStatus.InProgress => "ti ti-loader",
                ReconciliationStatus.Completed => "ti ti-check",
                ReconciliationStatus.Finalized => "ti ti-circle-check",
                ReconciliationStatus.Reopened => "ti ti-alert-circle",
                ReconciliationStatus.Cancelled => "ti ti-x",
                ReconciliationStatus.Failed => "ti ti-ban",
                _ => "ti ti-info-circle"
            };
        }

        string GetStatusBadge(ReconciliationStatus status)
        {
            return status switch
            {
                ReconciliationStatus.Draft => "bg-secondary-transparent text-secondary",
                ReconciliationStatus.InProgress => "bg-info-transparent text-info",
                ReconciliationStatus.Completed => "bg-warning-transparent text-warning",
                ReconciliationStatus.Finalized => "bg-success-transparent text-success",
                ReconciliationStatus.Reopened => "bg-danger-transparent text-danger",
                ReconciliationStatus.Cancelled => "bg-dark-transparent text-dark",
                ReconciliationStatus.Failed => "bg-danger-transparent text-danger",
                _ => "bg-dark"
            };
        }


        void OnRefresh()
        {
            SearchText = "";
            selectedStatus = null;

        }

        private async Task OnRefreshAsync()
        {
            isLoading = true;
            StateHasChanged();

            await Task.Delay(300);
            OnRefresh();
            ReconService.ResetToSeed();
            LoadRuns();

            isLoading = false;
            await JS.InvokeVoidAsync("feather.replace");
            Toast.ShowInfo("Reconciliation list refreshed", "Refreshed");
        }
    }
}

