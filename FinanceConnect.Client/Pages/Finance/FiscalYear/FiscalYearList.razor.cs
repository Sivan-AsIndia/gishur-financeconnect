using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.NetworkInformation;
using static FinanceConnect.Client.Data.MasterDataIds;

namespace FinanceConnect.Client.Pages.Finance.FiscalYear
{
    public partial class FiscalYearList
    {
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
        }

        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] AccountingPeriodService AccPeriodService { get; set; } = default!;
        bool ShowReasonModal;
        string ModalTitle = "";
        string Reason = "";
        Action? PendingAction;

        List<FiscalYearModel> Years = new();
        List<FiscalYearModel> FilteredYears = new();
        public List<CompanyModel> Companies = new();
        Guid? selectedCompanyId = null;
        string searchText = "";
        FiscalYearStatus? selectedStatus = null;
        private bool isInitialized = false;
        private bool isLoading = false;



        Guid? SelectedCompanyId
        {
            get => selectedCompanyId;
            set
            {
                selectedCompanyId = value;
                LoadFiscalYears();
            }
        }


        int PageSize = 10;
        int CurrentPage = 1;
        int PageWindowSize = 2;
        int TotalPages =>
            FilteredYears.Count == 0
                ? 1
                : (int)Math.Ceiling((double)FilteredYears.Count / PageSize);

        List<FiscalYearModel> PagedYears =>
            FilteredYears
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

        FiscalYearModel? SelectedYear;

        private async Task OpenViewModal(FiscalYearModel fy)
        {
            SelectedYear = fy;
            await JS.InvokeVoidAsync("blazorOffcanvas.show", "viewFiscalYearOffcanvas");
        }
        protected override void OnInitialized()
        {
            Companies = MasterDataService.GetAllCompanies().Where(c => c.Status == "Active").ToList();
            LoadFiscalYears();
        }

        void LoadFiscalYears()
        {
            if (SelectedCompanyId.HasValue)
            {
                Years = FiscalYearService.GetAllByCompanyId(SelectedCompanyId.Value);
            }
            else
            {
                Years = FiscalYearService.GetAll();
            }
            isInitialized = true;
            ApplyFilters();
        }


        void OnSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString() ?? "";
            ApplyFilters();
        }

        FiscalYearStatus? SelectedStatus
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
            IEnumerable<FiscalYearModel> query = Years;

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(f =>
                    f.FiscalYearCode.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    f.FiscalYearName.Contains(searchText, StringComparison.OrdinalIgnoreCase));
            }

            if (selectedStatus.HasValue)
            {
                query = query.Where(f => f.Status == selectedStatus.Value);
            }

            FilteredYears = query.OrderByDescending(f => f.UpdatedAt ?? f.CreatedAt).ToList();
            CurrentPage = 1;
        }


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

        async Task OpenAccountingPeriods()
        {
            //await JS.InvokeVoidAsync("bootstrapModal.hide", "view-fiscalyear-modal");
            //await Task.Delay(200);
            Nav.NavigateTo($"/fiscalyears/{SelectedYear!.Id}/periods");
        }

        void ChangeStatusOfFy(FiscalYearModel fy, FiscalYearStatus status)
        {
            if (status == FiscalYearStatus.Open)
            {
                var hasOpenFy = FiscalYearService
                    .GetAllByCompanyId(fy.CompanyId.Value)
                    .Any(x => x.Status == FiscalYearStatus.Open);

                if (hasOpenFy)
                {
                    ToastService.ShowError("The company already has an open fiscal year.");
                    return;
                }
            }



            FiscalYearService.ChangeStatus(fy.Id, status);
            if (status == FiscalYearStatus.Open && fy.AutoOpenFirstPeriod)
            {
                var periods = AccPeriodService.GetByFiscalYear(fy.Id);

                var firstPeriod = periods
                    .OrderBy(p => p.PeriodNumber)
                    .FirstOrDefault();

                if (firstPeriod != null && firstPeriod.Status == AccountingPeriodStatus.Draft)
                {
                    AccPeriodService.OpenPeriod(firstPeriod.Id);
                }
            }

            ToastService.ShowSuccess($"{fy.FiscalYearCode} - Status changed successfully");
        }

        void AskLock(FiscalYearModel fy, FiscalYearStatus status)
        {
            SelectedYear = fy;
            ModalTitle = "Close Accounting Period";
            PendingAction = LockConfirmed;
            ShowReasonModal = true;
        }

        void LockConfirmed()
        {
            try
            {
                FiscalYearService.ChangeStatus(SelectedYear.Id, FiscalYearStatus.Closed, Reason);
                ToastService.ShowSuccess($"{SelectedYear.FiscalYearCode} - Status changed successfully");
            }
            catch (Exception e)
            {
                ToastService.ShowError(e.Message);
            }

        }
        void CloseModal()
        {
            ShowReasonModal = false;
            Reason = "";
        }

        void ConfirmAction()
        {
            PendingAction?.Invoke();
            CloseModal();
        }
        

        private async Task OnRefreshAsync()
        {
            searchText = "";
            selectedStatus = null;
            SelectedCompanyId = null;
            isLoading = true;
            StateHasChanged();

            await Task.Delay(300);
            FiscalYearService.ResetToSeed();
            FilteredYears = new();
            LoadFiscalYears();
            CurrentPage = 1;

            isLoading = false;
            await JS.InvokeVoidAsync("feather.replace");
            ToastService.ShowInfo("FiscalYear List refreshed", "Refresh");
        }

        void ConfirmDelete(FiscalYearModel fy)
        {
            SelectedYear = fy;
        }
        async Task DeleteConfirmed()
        {
            if (SelectedYear == null)
                return;

            FiscalYearService.Delete(SelectedYear.Id);
            ToastService.ShowError($"{SelectedYear.FiscalYearName} Deleted Successfully");
            Years = FiscalYearService.GetAll();
            FilteredYears = Years.ToList();
            SelectedYear = null;

            CurrentPage = 1;

            await JS.InvokeVoidAsync("closeDeleteModal");

        }
        private string GetStatusBadge(FiscalYearStatus status)
        {
            return status switch
            {
                FiscalYearStatus.Open =>
                    "bg-success-transparent text-success",

                FiscalYearStatus.Closed =>
                    "bg-danger-transparent text-danger",

                FiscalYearStatus.Draft =>
                    "bg-warning-transparent text-warning",

                FiscalYearStatus.SoftClosed =>
                    "bg-info-transparent text-info",

                _ =>
                    "bg-secondary-transparent text-secondary"
            };
        }
        private string GetStatusDotBadge(FiscalYearStatus status)
        {
            return status switch
            {
                FiscalYearStatus.Open =>
                    "bg-success text-success",

                FiscalYearStatus.Closed =>
                    "bg-danger text-danger",

                FiscalYearStatus.Draft =>
                    "bg-warning text-warning",

                FiscalYearStatus.SoftClosed =>
                    "bg-info text-info",

                _ =>
                    "bg-secondary-transparent text-secondary"
            };
        }

    }
}
