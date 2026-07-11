using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.AR.CustomerCreditNote
{
    public partial class CustomerCreditNoteList
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] CustomerCreditNoteService CreditNoteService { get; set; } = default!;
        [Inject] CustomerService CustomerService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;

        private bool isInitialized = false;
        private bool isLoading = false;

        private List<CustomerCreditNoteViewModel> CreditNotes = new();
        private List<CustomerCreditNoteViewModel> FilteredCreditNotes = new();
        private CreditNoteStatisticsViewModel Statistics = new();

        private string searchTerm = string.Empty;
        private string selectedStatus = string.Empty;
        private DateTime? FromDate;
        private DateTime? ToDate;

        private CustomerCreditNoteViewModel? SelectedCreditNote;
        private string cancelReasonInput = string.Empty;

        // Pagination
        private int CurrentPage = 1;
        private int PageSize = 10;
        private int TotalPages => FilteredCreditNotes.Count > 0 ? (int)Math.Ceiling((double)FilteredCreditNotes.Count / PageSize) : 0;
        private IEnumerable<CustomerCreditNoteViewModel> PagedCreditNotes => FilteredCreditNotes
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize);
        private int VisibleColumnCount;

        // Computed: only statuses present in the data, ordered properly
        private List<string> DistinctStatuses => CreditNotes
            .Select(cn => cn.CreditNoteStatus)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Distinct()
            .OrderBy(s => Array.IndexOf(CreditNoteStatuses.All, s))
            .ToList();

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
            isInitialized = true;
        }

        private void CloseDrawer()
        {
            SelectedCreditNote = null;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
            VisibleColumnCount =
                await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        private async Task LoadData()
        {
            isLoading = true;
            StateHasChanged();
            ToDate = null;
            FromDate = null;
            selectedStatus = null!;
            searchTerm = "";
            await Task.Delay(100); // Simulate async load

            CreditNotes = CreditNoteService.GetAll();
            Statistics = CreditNoteService.GetStatistics();
            ApplyFilters();

            isLoading = false;
            StateHasChanged();
        }

        private async Task OnRefreshAsync()
        {
            await LoadData();
            ToastService.ShowSuccess("Data refreshed successfully");
        }

        private async Task OnSearch(ChangeEventArgs e)
        {
            searchTerm = e.Value?.ToString() ?? string.Empty;
            CurrentPage = 1;
            ApplyFilters();
            VisibleColumnCount =
                await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        private void ApplyFilters()
        {
            var query = CreditNotes.AsEnumerable();

            // Search filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();
                query = query.Where(cn =>
                    (cn.CreditNoteNumber?.ToLower().Contains(term) ?? false) ||
                    (cn.CustomerCode?.ToLower().Contains(term) ?? false) ||
                    (cn.CustomerName?.ToLower().Contains(term) ?? false) ||
                    (cn.ReferenceText?.ToLower().Contains(term) ?? false) ||
                    (cn.InvoiceNumberSnapshot?.ToLower().Contains(term) ?? false));
            }

            // Status filter
            if (!string.IsNullOrWhiteSpace(selectedStatus))
            {
                query = query.Where(cn => cn.Status == selectedStatus);
            }

            // Date filters
            if (FromDate.HasValue)
            {
                query = query.Where(cn => cn.CreditNoteDate >= FromDate.Value);
            }
            if (ToDate.HasValue)
            {
                query = query.Where(cn => cn.CreditNoteDate <= ToDate.Value);
            }

            FilteredCreditNotes = query.OrderByDescending(cn => cn.CreditNoteDate)
                                       .ThenByDescending(cn => cn.CreditNoteNumber)
                                       .ToList();

            // Reset to page 1 if current page exceeds total
            if (CurrentPage > TotalPages && TotalPages > 0)
            {
                CurrentPage = 1;
            }
        }

        #region Pagination

        private void GoToPage(int page)
        {
            if (page >= 1 && page <= TotalPages)
            {
                CurrentPage = page;
            }
        }

        private void PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
            }
        }

        private void NextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
            }
        }

        private void OnPageSizeChange(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out var size))
            {
                PageSize = size;
                CurrentPage = 1;
            }
        }

        #endregion

        #region Modal Handlers

        private void OpenRowDetails(CustomerCreditNoteViewModel cn)
        {
            SelectedCreditNote = cn;
        }

        private void OpenApproveModal(CustomerCreditNoteViewModel cn)
        {
            SelectedCreditNote = cn;
        }

        private void OpenPostModal(CustomerCreditNoteViewModel cn)
        {
            SelectedCreditNote = cn;
        }

        private void OpenCancelModal(CustomerCreditNoteViewModel cn)
        {
            SelectedCreditNote = cn;
            cancelReasonInput = string.Empty;
        }

        private void OpenDeleteModal(CustomerCreditNoteViewModel cn)
        {
            SelectedCreditNote = cn;
        }

        #endregion

        #region Actions

        private async Task ApproveCreditNote()
        {
            if (SelectedCreditNote == null) return;

            var result = CreditNoteService.Approve(
                SelectedCreditNote.Id,
                "System");

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                await LoadData();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        private async Task PostCreditNote()
        {
            if (SelectedCreditNote == null) return;

            var result = CreditNoteService.Post(
                SelectedCreditNote.Id,
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                "System");

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                await LoadData();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        private async Task CancelCreditNote()
        {
            if (SelectedCreditNote == null) return;

            if (string.IsNullOrWhiteSpace(cancelReasonInput))
            {
                ToastService.ShowWarning("Cancellation reason is required.");
                return;
            }

            var result = CreditNoteService.Cancel(
                SelectedCreditNote.Id,
                cancelReasonInput,
                "System");

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                await LoadData();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        private async Task DeleteCreditNote()
        {
            if (SelectedCreditNote == null) return;

            var result = CreditNoteService.Delete(SelectedCreditNote.Id, "System");

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                await LoadData();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        #endregion

        #region Helper Methods

        private string TruncateName(string? name, int maxLength)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;
            return name.Length <= maxLength ? name : name.Substring(0, maxLength) + "...";
        }

        private static string GetStatusBadgeClass(string status) => status switch
        {
            CreditNoteStatuses.Draft => "bg-secondary-transparent text-secondary",
            CreditNoteStatuses.Submitted => "bg-info-transparent text-info",
            CreditNoteStatuses.Approved => "bg-primary-transparent text-primary",
            CreditNoteStatuses.Posted => "bg-success-transparent text-success",
            CreditNoteStatuses.Cancelled => "bg-danger-transparent text-danger",
            CreditNoteStatuses.Reversed => "bg-dark-transparent text-dark",
            _ => "bg-secondary-transparent text-secondary"
        };

        private static string GetReasonBadgeClass(string reason) => reason switch
        {
            CreditReasonCodes.SalesReturn => "bg-warning-transparent text-warning",
            CreditReasonCodes.PriceCorrection => "bg-info-transparent text-info",
            CreditReasonCodes.DiscountAfterInvoice => "bg-primary-transparent text-primary",
            CreditReasonCodes.ServiceCancellation => "bg-danger-transparent text-danger",
            CreditReasonCodes.DamageDefect => "bg-secondary-transparent text-secondary",
            CreditReasonCodes.TaxCorrection => "bg-success-transparent text-success",
            CreditReasonCodes.WriteOffSettlement => "bg-dark-transparent text-dark",
            CreditReasonCodes.Other => "bg-light text-dark",
            _ => "bg-secondary-transparent text-secondary"
        };

        #endregion
    }
}
