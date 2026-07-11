using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.RegularExpressions;

namespace FinanceConnect.Client.Pages.Finance.Journal
{
    public partial class Journals
    {

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
            VisibleColumnCount =
 await JS.InvokeAsync<int>("getVisibleTableColumns");
        }


        [Inject] MasterDataService MasterDataService { get; set; } = default!;

        // ---------- State ----------
        List<JournalModel> journals = new();
        List<JournalModel> FilteredJournals = new();
        public List<CompanyModel> Companies = new();
        LedgerModel? Ledger = new();
        DocumentNumberSeriesModel? DocNumberSeries = new();
        Guid? selectedCompanyId = null;
        string searchText = "";
        JournalStatus? selectedStatus = null;
        JournalModel? SelectedJournalView;

        private bool isInitialized = false;
        private bool isLoading = false;

        JournalModel? SelectedJournal;
        string ActionType = "";
        string Reason = "";
        bool ShowReasonModal = false;
        private int VisibleColumnCount;
        Guid? SelectedCompanyId
        {
            get => selectedCompanyId;
            set
            {
                selectedCompanyId = value;
                LoadJournals();
            }
        }

        JournalStatus? SelectedStatus
        {
            get => selectedStatus;
            set
            {
                selectedStatus = value;
                ApplyFilters();
            }
        }

        // ---------- Pagination ----------
        int PageSize = 10;
        int CurrentPage = 1;
        int PageWindowSize = 2;
        int TotalPages =>
            FilteredJournals.Count == 0
                ? 1
                : (int)Math.Ceiling((double)FilteredJournals.Count / PageSize);

        List<JournalModel> PagedJournals =>
            FilteredJournals
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


        protected override void OnInitialized()
        {
            LoadJournals();
        }

        // ---------- Data Loading ----------
        void LoadJournals()
        {
            Companies = MasterDataService.GetAllCompanies().Where(c => c.Status == "Active").ToList();
            if (SelectedCompanyId.HasValue)
            {
                journals = JournalService.GetByCompany(SelectedCompanyId.Value);
            }
            else
            {
                journals = JournalService.GetAll();
            }
            isInitialized = true;
            ApplyFilters();
        }

        // ---------- Search & Filters ----------
        async Task OnSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString() ?? "";
            ApplyFilters();
            VisibleColumnCount =
 await JS.InvokeAsync<int>("getVisibleTableColumns");
        }


        void OpenRowModal(JournalModel journal)
        {
            SelectedJournalView = journal;
        }

        private async Task ViewJournal(JournalModel journal)
        {
            SelectedJournalView = journal;
            StateHasChanged();
            await JS.InvokeVoidAsync("blazorOffcanvas.show", "viewJournalOffcanvas");
        }

        void ApplyFilters()
        {
            IEnumerable<JournalModel> query = journals;

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(j =>
                    j.JournalCode.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    j.JournalName.Contains(searchText, StringComparison.OrdinalIgnoreCase));
            }

            if (SelectedStatus.HasValue)
            {
                query = query.Where(j => j.Status == SelectedStatus.Value);
            }

            // Sort by most recent first (UpdatedAt if exists, otherwise CreatedAt)
            FilteredJournals = query
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToList();
            CurrentPage = 1;
        }

        // ---------- Pagination ----------
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

        // ------------/ pagination -----------------



        // ---------- Activate / Deactivate ----------

        void AskActivate(JournalModel j)
        {
            SelectedJournal = j;
            ActionType = "Activate";
            Reason = "";
            ShowReasonModal = true;
        }

        void AskDeactivate(JournalModel j)
        {
            SelectedJournal = j;
            ActionType = "Deactivate";
            Reason = "";
            ShowReasonModal = true;
        }

        void ConfirmAction()
        {
            if (SelectedJournal == null)
                return;

            if (string.IsNullOrWhiteSpace(Reason))
            {
                ToastService.ShowError("Reason Required");
                return;
            }

            if (ActionType == "Activate")
            {
                JournalService.Activate(SelectedJournal.Id, Reason);
                ToastService.ShowSuccess($"Journal '{SelectedJournal.JournalName}' activated successfully", "Activated");
            }

            else
            {
                JournalService.Deactivate(SelectedJournal.Id, Reason);
                ToastService.ShowWarning($"Journal '{SelectedJournal.JournalName}' deactivated successfully", "Deactivated");
            }


            ShowReasonModal = false;
            selectedCompanyId = null;
            LoadJournals();
        }

        void CloseReasonModal()
        {
            ShowReasonModal = false;
            SelectedJournal = null;
            Reason = "";
        }

        void OpenEntry()
        {
            Nav.NavigateTo("/journal-entries");

        }
        private string GetStatusDotBadge(JournalStatus status)
        {
            return status switch
            {
                JournalStatus.Draft => "bg-warning text-warning",
                JournalStatus.Active => "bg-success text-success",
                JournalStatus.Inactive => "bg-danger text-danger",
                _ => "bg-light text-dark"

            };
        }
        private string GetStatusBadge(JournalStatus status)
        {
            return status switch
            {
                JournalStatus.Draft => "bg-warning-transparent text-warning",
                JournalStatus.Active => "bg-success-transparent text-success",
                JournalStatus.Inactive => "bg-danger-transparent text-danger",
                _ => "bg-light-transparent text-dark"

            };
        }

        string GetLedgerName(Guid? LedgerID)
        {
            Ledger = JournalService.GetLedgerById(LedgerID);
            return Ledger?.LedgerName ?? "-";
        }
        string GetDocumentNumberSeriesName(Guid? DocumentNumberSeriesId)
        {
            DocNumberSeries = JournalService.GetDocNumById(DocumentNumberSeriesId);
            return DocNumberSeries?.SeriesName?? "-";
        }


        void ConfirmDelete(JournalModel j)
        {
            SelectedJournal = j;
        }
        async Task DeleteConfirmed()
        {
            if (SelectedJournal == null)
                return;

            JournalService.Delete(SelectedJournal.Id);
            ToastService.ShowError($"{SelectedJournal.JournalName} Deleted Successfully");
            journals = JournalService.GetAll();
            FilteredJournals = journals.ToList();
            SelectedJournal = null;

            CurrentPage = 1;

            await JS.InvokeVoidAsync("closeDeleteModal");

        }

        private string GetPlainText(string? html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return "-";

            return Regex.Replace(html, "<.*?>", string.Empty);
        }

        // ---------- Refresh ----------

        private async Task OnRefreshAsync()
        {
            isLoading = true;
            StateHasChanged();

            await Task.Delay(300);
            searchText = "";
            selectedStatus = null;
            FilteredJournals = new();
            SelectedCompanyId = null;
            JournalService.ResetToSeed();
            LoadJournals();
            CurrentPage = 1;

            isLoading = false;
            await JS.InvokeVoidAsync("feather.replace");
            ToastService.ShowInfo("Journal list refreshed", "Refreshed");
        }
    }
}
