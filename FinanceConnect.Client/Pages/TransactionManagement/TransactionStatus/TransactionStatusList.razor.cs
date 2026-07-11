using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.TransactionManagement.TransactionStatus
{
    public partial class TransactionStatusList
    {
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {

            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
            VisibleColumnCount =
            await JS.InvokeAsync<int>("getVisibleTableColumns");
        }
        [Inject] TransactionStatusService StatusService { get; set; } = default!;
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;

        Guid TenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        TransactionStatusModel? SelectedStatuses;

        List<TransactionStatusModel> Statuses = new();
        List<TransactionStatusModel> FilteredStatuses = new();

        private bool isInitialized = false;
        private bool isLoading = false;
        string searchText = "";
        string selectedStatus = "";
        bool canDeactivate = true;
        int PageSize = 10;
        int CurrentPage = 1;
        int PageWindowSize = 2;
        private int VisibleColumnCount;
        int TotalPages =>
            FilteredStatuses.Count == 0
                ? 1
                : (int)Math.Ceiling((double)FilteredStatuses.Count / PageSize);

        List<TransactionStatusModel> PagedStatuses =>
            FilteredStatuses
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

        string SelectedStatus
        {
            get => selectedStatus;
            set
            {
                selectedStatus = value;
                ApplyFilters();
            }
        }



        protected override void OnInitialized()
        {

            LoadStatuses();
        }

        void LoadStatuses()
        {
            Statuses = StatusService.GetAll(TenantId);
            isInitialized = true;
            ApplyFilters();
        }

        void OnRefresh()
        {
            searchText = "";
            selectedStatus = "";
        }

        void OnSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString() ?? "";
            ApplyFilters();
        }

        void ApplyFilters()
        {
            IEnumerable<TransactionStatusModel> query = Statuses;

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(s =>
                    s.Code.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    s.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(selectedStatus))
            {
                bool active = selectedStatus == "Active";
                query = query.Where(s => s.IsActive == active);
            }

            FilteredStatuses = query
                .OrderBy(s => s.DisplayOrder)
                .ThenBy(s => s.Code)
                .ToList();

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

        void AddStatus()
        {
            Nav.NavigateTo("/transaction-status/create");
        }

        void EditStatus(TransactionStatusModel status)
        {
            Nav.NavigateTo($"/transaction-status/{status.TransactionStatusId}");
        }

        void ViewStatus(TransactionStatusModel status)
        {
            SelectedStatuses = status;
        }
        void GoToStatusDetails(TransactionStatusModel status)
        {
            Nav.NavigateTo($"/transaction-status/{status.TransactionStatusId}/view");
        }
        void SelectStatus(TransactionStatusModel status)
        {
            SelectedStatuses = status;
        }

        void ActivateConfirmed()
        {
            if (SelectedStatus == null) return;

            StatusService.Activate(SelectedStatuses.TransactionStatusId);
            ToastService.ShowSuccess($"Status '{SelectedStatuses.Name}' activated");
            LoadStatuses();
        }

        void DeactivateConfirmed()
        {
            if (SelectedStatus == null) return;

            try
            {
                StatusService.Deactivate(SelectedStatuses.TransactionStatusId);
                ToastService.ShowSuccess($"Status '{SelectedStatuses.Name}' deactivated");
            }
            catch (Exception ex)
            {
                ToastService.ShowError(ex.Message);
            }

            LoadStatuses();
        }

        void DeleteConfirmed()
        {
            if (SelectedStatus == null) return;

            StatusService.Delete(SelectedStatuses.TransactionStatusId);
            ToastService.ShowSuccess($"Transaction Status '{SelectedStatuses.Name}' deleted", "Deleted");
            LoadStatuses();
        }

        private string GetBadgeClass(BadgeTone tone)
        {
            return tone switch
            {
                BadgeTone.Success => "bg-success-transparent text-success",
                BadgeTone.Warning => "bg-warning-transparent text-warning",
                BadgeTone.Danger => "bg-danger-transparent text-danger",
                _ => "bg-secondary-transparent text-secondary"
            };
        }

        private async Task OnRefreshAsync()
        {
            isLoading = true;
            StateHasChanged();

            await Task.Delay(300);
            OnRefresh();
            StatusService.ResetToSeed();
            LoadStatuses();

            isLoading = false;
            await JS.InvokeVoidAsync("feather.replace");
            ToastService.ShowInfo("Transaction Status list refreshed", "Refreshed");
        }
    }
}
