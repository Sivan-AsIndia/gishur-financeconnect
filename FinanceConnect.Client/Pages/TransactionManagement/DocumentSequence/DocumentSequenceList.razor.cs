using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.TransactionManagement.DocumentSequence
{
    public partial class DocumentSequenceList
    {

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
            VisibleColumnCount =
            await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        private List<DocumentSequenceModel> Sequences = new();
        private List<DocumentSequenceModel> FilteredSequences = new();

        private List<CompanyModel> Companies = new();

        private DocumentSequenceModel? SelectedSequence;

        private bool isInitialized = false;
        private bool isLoading = false;

        long AdjustValue;
        string? AdjustReason;
        private string searchText = string.Empty;
        int PageSize = 10;
        int CurrentPage = 1;
        int PageWindowSize = 2;
        string selectedStatus = "";
        Guid? selectedCompany = null;
        bool showAdjustModel= false;
        private int VisibleColumnCount;
        int TotalPages =>
            FilteredSequences.Count == 0
                ? 1
                : (int)Math.Ceiling((double)FilteredSequences.Count / PageSize);

        List<DocumentSequenceModel> PagedSequences =>
            FilteredSequences
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
            Companies = SequenceService.GetCompanies();
            LoadSequences();
        }

        // LOAD / REFRESH
        private void LoadSequences()
        {
            Sequences = SequenceService.GetAll();
            isInitialized = true;
            ApplyFilters();
        }

        private void OnRefresh()
        {
            searchText = string.Empty;
            selectedCompany = null;
            selectedStatus = string.Empty;
        }



        // FILTERING
        private void OnSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString() ?? string.Empty;
            CurrentPage = 1;
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
            IEnumerable<DocumentSequenceModel> query = Sequences;

            // SEARCH
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(s =>
                    s.SeriesCode.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    s.ResetKey.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    s.BranchScopeMode.ToString().Contains(searchText, StringComparison.OrdinalIgnoreCase)
                );
            }

            if (selectedCompany.HasValue)
            {
                query = query.Where(s =>
                    s.CompanyId == selectedCompany.Value);
            }

            if (!string.IsNullOrWhiteSpace(selectedStatus))
            {
                query = selectedStatus switch
                {
                    "Active" => query.Where(s => s.IsActive && !s.IsLocked && !s.IsExhausted),
                    "Locked" => query.Where(s => s.IsLocked),
                    "Exhausted" => query.Where(s => s.IsExhausted),
                    _ => query
                };
            }
            FilteredSequences = query
                .OrderByDescending(x => x.LastIssuedAt ?? x.CreatedAt)
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

        private void ViewSequence(DocumentSequenceModel seq)
        {
            SelectedSequence = seq;
        }

        private void OpenRowDetails(DocumentSequenceModel seq)
        {
            SelectedSequence = seq;
        }

        private void ConfirmLock(DocumentSequenceModel seq)
        {
            SelectedSequence = seq;
        }

        private void ConfirmUnlock(DocumentSequenceModel seq)
        {
            SelectedSequence = seq;
        }

        private void ConfirmDelete(DocumentSequenceModel seq)
        {
            SelectedSequence = seq;
        }

        // ACTION CONFIRMATIONS
        private void LockConfirmed()
        {
            if (SelectedSequence == null) return;

            SequenceService.Lock(SelectedSequence.DocumentSequenceId);
            ToastService.ShowSuccess("Sequence locked");

            LoadSequences();
        }

        private void UnlockConfirmed()
        {
            if (SelectedSequence == null) return;

            SequenceService.Unlock(SelectedSequence.DocumentSequenceId);
            ToastService.ShowSuccess("Sequence unlocked");

            LoadSequences();
        }

        private void DeleteConfirmed()
        {
            if (SelectedSequence == null) return;

            try
            {
                SequenceService.Delete(SelectedSequence.DocumentSequenceId);
                ToastService.ShowSuccess("Sequence deleted");

                LoadSequences();
            }
            catch (Exception e)
            {
                ToastService.ShowError(e.Message);
            }

        }

        // UI HELPERS
        private string GetStatusBadge(DocumentSequenceModel s)
        {
            if (s.IsExhausted)
                return "bg-danger-transparent text-danger";

            if (s.IsLocked)
                return "bg-warning-transparent text-warning";

            if (s.IsActive)
                return "bg-success-transparent text-success";

            return "bg-secondary-transparent text-secondary";
        }
        private string GetDotStatusBadge(DocumentSequenceModel s)
        {
            if (s.IsExhausted)
                return "bg-danger text-danger";

            if (s.IsLocked)
                return "bg-warning text-warning";

            if (s.IsActive)
                return "bg-success text-success";

            return "bg-secondary text-secondary";
        }

        private string GetStatusText(DocumentSequenceModel s)
        {
            if (s.IsExhausted)
                return "Exhausted";

            if (s.IsLocked)
                return "Locked";

            if (s.IsActive)
                return "Active";

            return "Inactive";
        }

        private string GetCompanyName(Guid companyId)
        {
            return Companies
                .FirstOrDefault(c => c.Id == companyId)
                ?.LegalName
                ?? "Unknown Company";
        }


        void ConfirmReset(DocumentSequenceModel s)
        {
            SelectedSequence = s;
        }

        void ResetConfirmed()
        {
            try
            {
                SequenceService.Reset(SelectedSequence!.DocumentSequenceId);
                ToastService.ShowSuccess("Sequence reset and audit logged");
                LoadSequences();
            }
            catch (Exception e)
            {
                ToastService.ShowError(e.Message);
            }

        }

        void ConfirmAdjust(DocumentSequenceModel s)
        {
            SelectedSequence = s;
            AdjustValue = s.CurrentValue;
            AdjustReason = "";
            showAdjustModel = true;
        }

        void CloseAdjustModal()
        {
            showAdjustModel = false;
            SelectedSequence = null;
            AdjustReason = "";
        }
        void AdjustConfirmed()
        {
            if (string.IsNullOrWhiteSpace(AdjustReason))
            {
                ToastService.ShowError("Reason Required");
                return;
            }
            try
            {
                SequenceService.Adjust(
                    SelectedSequence!.DocumentSequenceId,
                    AdjustValue,
                    AdjustReason ?? "Manual adjustment"
                );
                showAdjustModel = false;

                ToastService.ShowSuccess("Sequence adjusted (audit logged)");
                LoadSequences();
            }
            catch (Exception e)
            {
                ToastService.ShowError(e.Message);
            }

        }

        private async Task OnRefreshAsync()
        {
            isLoading = true;
            StateHasChanged();

            await Task.Delay(300);
            OnRefresh();
            SequenceService.ResetToSeed();
            LoadSequences();

            isLoading = false;
            await JS.InvokeVoidAsync("feather.replace");
            ToastService.ShowInfo("Sequences list refreshed", "Refreshed");
        }

    }

}

