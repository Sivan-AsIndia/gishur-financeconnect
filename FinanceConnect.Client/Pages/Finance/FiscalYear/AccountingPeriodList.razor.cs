using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Finance.FiscalYear
{
    public partial class AccountingPeriodList
    {

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
        }

        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Parameter] public Guid FiscalYearId { get; set; }

        List<AccountingPeriodModel> Periods = new();
        List<AccountingPeriodModel> FilteredPeriods = new();
        FiscalYearModel? FiscalYear;
        string searchText = "";
        AccountingPeriodStatus? selectedStatus = null;
        List<CompanyModel> Companies = new();
        bool ShowViewModal;
        AccountingPeriodModel? ViewPeriodModel;

        bool ShowReasonModal;
        string ModalTitle = "";
        string Reason = "";
        AccountingPeriodModel? SelectedPeriod;
        Action? PendingAction;
        private bool isInitialized = false;
        private bool isLoading = false;
        bool CanEditLocks =>
            ViewPeriodModel?.Status == AccountingPeriodStatus.Open;

        int PageSize = 10;
        int CurrentPage = 1;

        int TotalPages =>
            FilteredPeriods.Count == 0
                ? 1
                : (int)Math.Ceiling((double)FilteredPeriods.Count / PageSize);

        List<AccountingPeriodModel> PagedPeriods =>
            FilteredPeriods
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

        string CompanyName = "";
        protected override void OnInitialized()
        {
            FiscalYear = FiscalYearService.GetById(FiscalYearId);
            Periods = PeriodService.GetByFiscalYear(FiscalYearId);
            Companies = MasterDataService.GetAllCompanies();
            CompanyName = Companies.FirstOrDefault(c => c.Id == FiscalYear?.CompanyId)?.LegalName?? "—";
            isInitialized = true;
            ApplyFilters();
        }

        void OnSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString() ?? "";
            ApplyFilters();
        }


        void AskLock(AccountingPeriodModel p)
        {
            SelectedPeriod = p;
            ModalTitle = "Close Accounting Period";
            PendingAction = LockConfirmed;
            ShowReasonModal = true;
        }

        void AskReopen(AccountingPeriodModel p)
        {
            SelectedPeriod = p;
            ModalTitle = "Reopen Accounting Period";
            PendingAction = ReopenConfirmed;
            ShowReasonModal = true;
        }

        void ViewPeriod(AccountingPeriodModel p)
        {
            ViewPeriodModel = p;
            ShowViewModal = true;
        }

        void OpenRowDetails(AccountingPeriodModel p)
        {
            ViewPeriodModel = p;
        }

        void CloseViewModal()
        {
            ShowViewModal = false;
            ViewPeriodModel = null;
        }


        void ConfirmAction()
        {
            PendingAction?.Invoke();
            CloseModal();
        }

        void LockConfirmed()
        {
            try
            {
                PeriodService.LockPeriod(SelectedPeriod!.Id, Reason, "system");

            }
            catch (Exception e)
            {
                ToastService.ShowError(e.Message);
            }
        }

        void CreateManualPeriod()
        {
            Nav.NavigateTo($"/fiscalyears/periods/create/{FiscalYearId}");
        }


        void CloseModal()
        {
            ShowReasonModal = false;
            Reason = "";
        }

        AccountingPeriodStatus? SelectedStatus
        {
            get => selectedStatus;
            set
            {
                selectedStatus = value;
                ApplyFilters();
            }
        }

        void ApplyFilters()
        {
            IEnumerable<AccountingPeriodModel> query = Periods;

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(p =>
                    p.PeriodName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    p.PeriodCode.Contains(searchText, StringComparison.OrdinalIgnoreCase));
            }

            if (selectedStatus.HasValue)
            {
                query = query.Where(p => p.Status == selectedStatus.Value);
            }

            // Sort by most recent first (UpdatedAt if exists, otherwise CreatedAt)
            FilteredPeriods = query
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToList();
            CurrentPage = 1;
        }


        private async Task OnRefreshAsync()
        {
            searchText = "";
            selectedStatus = null;

            isLoading = true;
            StateHasChanged();

            await Task.Delay(300);
            PeriodService.GenerateForFiscalYear(FiscalYear);
            Periods = PeriodService.GetByFiscalYear(FiscalYearId);
            FilteredPeriods = Periods;
            CurrentPage = 1;

            isLoading = false;
            await JS.InvokeVoidAsync("feather.replace");
            ToastService.ShowInfo("Period List refreshed", "Refresh");

        }

        void OnPageSizeChange(ChangeEventArgs e)
        {
            PageSize = int.Parse(e.Value!.ToString()!);
            CurrentPage = 1;
        }

        void PreviousPage() { if (CurrentPage > 1) CurrentPage--; }
        void NextPage() { if (CurrentPage < TotalPages) CurrentPage++; }
        void GoToPage(int page) { if (page >= 1 && page <= TotalPages) CurrentPage = page; }


        void OpenPeriod(AccountingPeriodModel p)
        {
            try
            {
                PeriodService.OpenPeriod(p.Id);
                ToastService.ShowSuccess($"{p.PeriodName} open Successfully");
            }
            catch (Exception e)
            {
                ToastService.ShowError(e.Message);
            }

        }


        void SoftClosePeriod(AccountingPeriodModel p) =>
            PeriodService.SoftClosePeriod(p.Id);

        void ClosePeriod(AccountingPeriodModel p) =>
            PeriodService.ClosePeriod(p.Id);

        void ReopenConfirmed()
        {
            try
            {
                PeriodService.ReopenPeriod(SelectedPeriod!.Id, Reason, "system");

            }
            catch (Exception e)
            {
                ToastService.ShowError(e.Message);
            }
        }
        private string GetStatusDotBadge(AccountingPeriodStatus status)
        {
            return status switch
            {
                AccountingPeriodStatus.Open =>
                    "bg-success text-success",

                AccountingPeriodStatus.Closed =>
                    "bg-danger text-danger",

                AccountingPeriodStatus.Draft =>
                    "bg-warning text-warning",

                AccountingPeriodStatus.SoftClosed =>
                    "bg-info text-info",

                _ =>
                    "bg-secondary-transparent text-secondary"
            };
        }

        private string GetStatusBadge(AccountingPeriodStatus status)
        {
            return status switch
            {
                AccountingPeriodStatus.Open =>
                    "bg-success-transparent text-success",

                AccountingPeriodStatus.Closed =>
                    "bg-danger-transparent text-danger",

                AccountingPeriodStatus.Draft =>
                    "bg-warning-transparent text-warning",

                AccountingPeriodStatus.SoftClosed =>
                    "bg-info-transparent text-info",

                _ =>
                    "bg-secondary-transparent text-secondary"
            };
        }
    }
}
