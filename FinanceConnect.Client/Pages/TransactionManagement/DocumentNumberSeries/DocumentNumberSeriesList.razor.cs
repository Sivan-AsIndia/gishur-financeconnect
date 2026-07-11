using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FinanceConnect.Client.Pages.TransactionManagement.DocumentNumberSeries
{
    public partial class DocumentNumberSeriesList
    {

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
        }

        [Inject] TransactionTypeService TransactionTypeService { get; set; } = default!;
        [Inject] JournalService JournalService { get; set; } = default!;

        private List<DocumentNumberSeriesModel> AllSeries = new();
        private List<DocumentNumberSeriesModel> FilteredSeries = new();

        private List<CompanyModel> Companies = new();

        private DocumentNumberSeriesModel? SelectedSeries;

        private string searchText = "";
        string selectedStatus = "";
        Guid? selectedCompany = null;

        private bool canDeactivate = true;
        private bool canDelete = false;

        int PageSize = 10;
        int CurrentPage = 1;
        int PageWindowSize = 2;

        private bool isInitialized = false;
        private bool isLoading = false;

        int TotalPages =>
            FilteredSeries.Count == 0
                ? 1
                : (int)Math.Ceiling((double)FilteredSeries.Count / PageSize);

        List<DocumentNumberSeriesModel> PagedSeries =>
            FilteredSeries
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
            Companies = SeriesService.GetCompanies();
            await LoadSeries();
        }

        private async Task LoadSeries()
        {
            AllSeries = SeriesService.GetAll();
            isInitialized = true;
            ApplyFilters();
            await Task.CompletedTask;
        }

        // FILTERING + SEARCH

        private void OnSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString() ?? "";
            ApplyFilters();
        }

        string SelectedStatus
        {
            get => selectedStatus;
            set
            {
                selectedStatus = value;
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

        void ApplyFilters()
        {
            IEnumerable<DocumentNumberSeriesModel> query = AllSeries;
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(s =>
                    s.SeriesCode.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    s.SeriesName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    s.AppliesToEntityType.ToString().Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    s.ResetFrequency.ToString().Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    s.SequenceScopeMode.ToString().Contains(searchText, StringComparison.OrdinalIgnoreCase)
                );
            }

            if (selectedCompany.HasValue)
            {
                query = query.Where(s =>
                    s.CompanyId == selectedCompany.Value);
            }

            // STATUS FILTER
            if (!string.IsNullOrWhiteSpace(selectedStatus))
            {
                query = selectedStatus switch
                {
                    "Active" => query.Where(s => s.IsActive && !s.IsLocked),
                    "Inactive" => query.Where(s => !s.IsActive),
                    "Locked" => query.Where(s => s.IsLocked),
                    _ => query
                };
            }
            FilteredSeries = query
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToList();
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



        void OnRefresh()
        {
            searchText = "";
            selectedStatus = "";
            selectedCompany = null;

        }

        private void ViewSeries(DocumentNumberSeriesModel series)
        {
            SelectedSeries = series;

            Nav.NavigateTo($"/document-series/{series.DocumentNumberSeriesId}/view");

        }

        private void OpenRowDetails(DocumentNumberSeriesModel series)
        {
            SelectedSeries = series;
        }

        private void ConfirmActivate(DocumentNumberSeriesModel series)
        {
            SelectedSeries = series;
        }

        private void ActivateConfirmed()
        {
            if (SelectedSeries == null) return;

            SelectedSeries.IsActive = true;
            SelectedSeries.IsLocked = false;

            SeriesService.UpdateAsync(SelectedSeries);
            ToastService.ShowSuccess("Series activated");

            ApplyFilters();
        }

        private void ConfirmDeactivate(DocumentNumberSeriesModel series)
        {
            SelectedSeries = series;
        }

        private void DeactivateConfirmed()
        {
            if (SelectedSeries == null) return;

            SelectedSeries.IsActive = false;

            SeriesService.UpdateAsync(SelectedSeries);
            ToastService.ShowWarning("Series deactivated");

            ApplyFilters();
        }

        private void ConfirmDelete(DocumentNumberSeriesModel series)
        {
            SelectedSeries = series;
            bool hasTransactionTypes =
                TransactionTypeService.GetByDocNumSeriesId(series.DocumentNumberSeriesId)?.Any() == true;

            bool hasJournals =
                JournalService.GetByDocNumSeriesId(series.DocumentNumberSeriesId)?.Any() == true;

            canDelete = !(hasTransactionTypes || hasJournals);
        }

        async Task DeleteConfirmed()
        {
            if (SelectedSeries == null) return;

            SeriesService.Delete(SelectedSeries.DocumentNumberSeriesId);

            await JS.InvokeVoidAsync("closeDeleteModal");
            ToastService.ShowSuccess($"{SelectedSeries.SeriesName} Deleted Successfully");

            LoadSeries();
        }


        private string GetStatusText(DocumentNumberSeriesModel s)
        {
            if (s.IsLocked) return "Locked";
            if (s.IsActive) return "Active";
            return "Inactive";
        }

        private string GetStatusBadge(DocumentNumberSeriesModel s)
        {
            if (s.IsLocked) return "bg-danger-transparent text-danger";
            if (s.IsActive) return "bg-success-transparent text-success";
            return "bg-secondary-transparent text-secondary";
        }
        private string GetStatusDotBadge(DocumentNumberSeriesModel s)
        {
            if (s.IsLocked) return "bg-danger text-danger";
            if (s.IsActive) return "bg-success text-success";
            return "bg-secondary text-secondary";
        }

        private string GetCompanyName(Guid? companyId)
        {
            return Companies
                .FirstOrDefault(c => c.Id == companyId)
                ?.LegalName
                ?? "Unknown Company";
        }

        private async Task OnRefreshAsync()
        {
            isLoading = true;
            StateHasChanged();

            await Task.Delay(300);
            OnRefresh();
            SeriesService.ResetToSeed();
            await LoadSeries();

            isLoading = false;
            await JS.InvokeVoidAsync("feather.replace");
            ToastService.ShowInfo("Series list refreshed", "Refreshed");
        }
    }
}
