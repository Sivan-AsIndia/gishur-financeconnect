using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Finance.Journal
{
    public partial class JournalEntryList
    {

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
        }
        [Inject] MasterDataService MasterDataService { get; set; } = default!;

        List<JournalEntryModel> FilteredEntries = new();
        public List<CompanyModel> Companies = new();
        [Parameter] public Guid? JournalId { get; set; }
        Guid? selectedCompanyId = null;
        JournalEntryStatus? selectedStatus = null;
        string searchText = "";
        List<JournalEntryModel> AllEntries = new();
        private bool isInitialized = false;
        private bool isLoading = false;
        // ---------- Pagination ----------
        int PageSize = 10;
        int CurrentPage = 1;
        int PageWindowSize = 2;
        JournalEntryModel? SelectedEntry;
        int TotalPages =>
            FilteredEntries.Count == 0
                ? 1
                : (int)Math.Ceiling((double)FilteredEntries.Count / PageSize);

        List<JournalEntryModel> PagedEntries =>
         FilteredEntries
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



        protected override async Task OnInitializedAsync()
        {
            await Service.InitializeAsync();
            LoadEntries();
        }

        Guid? SelectedCompanyId
        {
            get => selectedCompanyId;
            set
            {
                selectedCompanyId = value;
                LoadEntries();
            }
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

        JournalEntryStatus? SelectedStatus
        {
            get => selectedStatus;
            set
            {
                selectedStatus = value;
                ApplyFilters();
            }
        }

        void LoadEntries()
        {
            Companies = MasterDataService.GetAllCompanies().Where(c => c.Status == "Active").ToList();
            AllEntries = Service.GetAll();

            if (JournalId.HasValue)
            {
                AllEntries = AllEntries
                    .Where(e => e.JournalId == JournalId.Value)
                    .ToList();
            }
            isInitialized = true;
            ApplyFilters();
        }


        void ApplyFilters()
        {
            var query = AllEntries.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var term = searchText.Trim();

                query = query.Where(e =>
                    (!string.IsNullOrWhiteSpace(e.JournalEntryNumber) &&
                     e.JournalEntryNumber.Contains(term, StringComparison.OrdinalIgnoreCase))
                    ||
                    (!string.IsNullOrWhiteSpace(e.Narration) &&
                     e.Narration.Contains(term, StringComparison.OrdinalIgnoreCase))
                );
            }

            if (SelectedCompanyId.HasValue)
            {
                query = query.Where(e =>
                    e.CompanyId.HasValue &&
                    e.CompanyId.Value == SelectedCompanyId.Value);
            }


            if (SelectedStatus.HasValue)
            {
                query = query.Where(e => e.Status == SelectedStatus.Value);
            }

            FilteredEntries = query
                .OrderByDescending(e => e.UpdatedAt ?? e.CreatedAt)
                .ToList();
            CurrentPage = 1;
        }
        void OnSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString() ?? "";
            ApplyFilters();
        }



        void ViewEntry(JournalEntryModel e)
        {
            SelectedEntry = e;
        }

        void OnRefresh()
        {
            searchText = "";
            SelectedCompanyId = null;
            SelectedStatus = null;

            LoadEntries();
        }

        private async Task OnRefreshAsync()
        {
            isLoading = true;
            StateHasChanged();

            await Task.Delay(300);

            searchText = "";
            SelectedCompanyId = null;
            SelectedStatus = null;

            await Service.ResetToSeed();

            LoadEntries();
            CurrentPage = 1;

            isLoading = false;

            StateHasChanged();

            await JS.InvokeVoidAsync("feather.replace");

            ToastService.ShowInfo("Journal Entry list refreshed", "Refreshed");
        }
        void ConfirmDelete(JournalEntryModel e)
        {
            SelectedEntry = e;
        }

        async Task DeleteConfirmed()
        {
            if (SelectedEntry == null)
                return;

            Service.DeleteDraft(SelectedEntry.Id);
            ToastService.ShowError($"{SelectedEntry.JournalEntryNumber} Deleted Successfully");
            LoadEntries();
            SelectedEntry = null;

            CurrentPage = 1;

            await JS.InvokeVoidAsync("closeDeleteModal");
        }

        void SubmitEntry()
        {
            try
            {
                Service.Submit(SelectedEntry!.Id);
                LoadEntries();
            }
            catch (Exception e)
            {
                ToastService.ShowError(e.Message);
            }
        }

        void ApproveEntry()
        {
            try
            {
                Service.Approve(SelectedEntry!.Id);
                LoadEntries();
            }
            catch (Exception e)
            {
                ToastService.ShowError(e.Message);
            }
        }

        void RejectEntry()
        {
            try
            {
                Service.Reject(SelectedEntry!.Id);
                LoadEntries();
            }
            catch (Exception e)
            {
                ToastService.ShowError(e.Message);
            }
        }

        void PostEntry()
        {
            try
            {
                Service.Post(SelectedEntry!.Id);
                LoadEntries();
            }
            catch (Exception e)
            {
                ToastService.ShowError(e.Message);
            }
        }

        void CancelEntry()
        {
            try
            {
                Service.Cancel(SelectedEntry!.Id);
                LoadEntries();
            }
            catch (Exception e)
            {
                ToastService.ShowError(e.Message);
            }
        }


        void OpenEntry(Guid id)
        {
            Nav.NavigateTo($"/journal-entries/{id}/lines");
            //Nav.NavigateTo("/journal-lines");
        }

        async Task OpenLine()
        {
            await JS.InvokeVoidAsync("bootstrapModal.hide", "view-Entry-modal");
            Nav.NavigateTo($"/journal-entries/{SelectedEntry.Id}/lines");
        }


        void EditEntry(Guid id)
        {
            Nav.NavigateTo($"/journal-entries/{id}");
        }
        private string GetStatusBadge(JournalEntryStatus status)
        {
            return status switch
            {
                JournalEntryStatus.Posted =>
                    "bg-success-transparent text-success",

                JournalEntryStatus.Cancelled =>
                    "bg-danger-transparent text-danger",

                JournalEntryStatus.Draft =>
                    "bg-warning-transparent text-warning",

                JournalEntryStatus.Approved =>
                    "bg-info-transparent text-info",

                JournalEntryStatus.Rejected =>
                    "bg-primary-transparent text-primary",

                _ =>
                    "bg-secondary-transparent text-secondary"
            };
        }
        private string GetStatusDotBadge(JournalEntryStatus status)
        {
            return status switch
            {
                JournalEntryStatus.Posted =>
                    "bg-success text-success",

                JournalEntryStatus.Cancelled =>
                    "bg-danger text-danger",

                JournalEntryStatus.Draft =>
                    "bg-warning text-warning",

                JournalEntryStatus.Approved =>
                    "bg-info text-info",

                JournalEntryStatus.Rejected =>
                    "bg-primary text-primary",

                _ =>
                    "bg-secondary text-secondary"
            };
        }

    }
}
