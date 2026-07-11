using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FinanceConnect.Client.Pages.AR.CustomerDebitNote
{
    public partial class CustomerDebitNoteList : ComponentBase
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] CustomerDebitNoteService DebitNoteService { get; set; } = default!;
        [Inject] CustomerService CustomerService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;

        private bool isInitialized = false;
        private bool isLoading = false;

        private List<CustomerDebitNoteViewModel> DebitNotes = new();
        private List<CustomerDebitNoteViewModel> FilteredDebitNotes = new();
        private DebitNoteStatisticsViewModel Statistics = new();

        private string searchText = string.Empty;
        private string selectedReasonCode = string.Empty;
        private string selectedStatus = string.Empty;
        private DateTime? FromDate;
        private DateTime? ToDate;

        private CustomerDebitNoteViewModel? SelectedDebitNote;
        private string cancelReasonInput = string.Empty;
        private string reverseReasonInput = string.Empty;
        private int VisibleColumnCount;

        // Pagination
        private int CurrentPage = 1;
        private int PageSize = 10;
        private int TotalPages => FilteredDebitNotes.Count > 0 ? (int)Math.Ceiling((double)FilteredDebitNotes.Count / PageSize) : 0;
        private IEnumerable<CustomerDebitNoteViewModel> PagedDebitNotes => FilteredDebitNotes
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize);

        // Distinct filter values from actual data
        private List<string> DistinctStatuses => DebitNotes
            .Select(dn => dn.DebitNoteStatus)
            .Where(s => !string.IsNullOrEmpty(s))
            .Distinct()
            .OrderBy(s => s)
            .ToList();

        private List<string> DistinctReasons => DebitNotes
            .Select(dn => dn.DebitReasonCode)
            .Where(r => !string.IsNullOrEmpty(r))
            .Distinct()
            .OrderBy(r => r)
            .ToList();

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
            isInitialized = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips", true);
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
            selectedReasonCode = null!;
            searchText = "";
            await Task.Delay(100); // Simulate async load

            DebitNotes = DebitNoteService.GetAll();
            Statistics = DebitNoteService.GetStatistics();
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
            searchText = e.Value?.ToString() ?? string.Empty;
            CurrentPage = 1;
            ApplyFilters();
            VisibleColumnCount =
await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        private void ApplyFilters()
        {
            var query = DebitNotes.AsEnumerable();

            // Search filter
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var term = searchText.ToLower();
                query = query.Where(dn =>
                    (dn.DebitNoteNumber?.ToLower().Contains(term) ?? false) ||
                    (dn.CustomerCode?.ToLower().Contains(term) ?? false) ||
                    (dn.CustomerName?.ToLower().Contains(term) ?? false) ||
                    (dn.ReferenceText?.ToLower().Contains(term) ?? false) ||
                    (dn.InvoiceNumberSnapshot?.ToLower().Contains(term) ?? false));
            }

            // Reason code filter
            if (!string.IsNullOrWhiteSpace(selectedReasonCode))
            {
                query = query.Where(dn => dn.DebitReasonCode == selectedReasonCode);
            }

            // Status filter
            if (!string.IsNullOrWhiteSpace(selectedStatus))
            {
                query = query.Where(dn => dn.DebitNoteStatus == selectedStatus);
            }

            // Date filters
            if (FromDate.HasValue)
            {
                query = query.Where(dn => dn.DebitNoteDate >= FromDate.Value);
            }
            if (ToDate.HasValue)
            {
                query = query.Where(dn => dn.DebitNoteDate <= ToDate.Value);
            }

            FilteredDebitNotes = query.OrderByDescending(dn => dn.DebitNoteDate)
                                      .ThenByDescending(dn => dn.DebitNoteNumber)
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

        private void OpenRowDetails(CustomerDebitNoteViewModel dn)
        {
            SelectedDebitNote = dn;
        }

        private void OpenSubmitModal(CustomerDebitNoteViewModel dn)
        {
            SelectedDebitNote = dn;
        }

        private void OpenPostModal(CustomerDebitNoteViewModel dn)
        {
            SelectedDebitNote = dn;
        }

        private void OpenCancelModal(CustomerDebitNoteViewModel dn)
        {
            SelectedDebitNote = dn;
            cancelReasonInput = string.Empty;
        }

        private void OpenReverseModal(CustomerDebitNoteViewModel dn)
        {
            SelectedDebitNote = dn;
            reverseReasonInput = string.Empty;
        }

        private void OpenDeleteModal(CustomerDebitNoteViewModel dn)
        {
            SelectedDebitNote = dn;
        }

        #endregion

        #region Actions

        private async Task SubmitDebitNote()
        {
            if (SelectedDebitNote == null) return;

            var result = DebitNoteService.Submit(SelectedDebitNote.Id, "System");

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

        private async Task PostDebitNote()
        {
            if (SelectedDebitNote == null) return;

            var result = DebitNoteService.Post(
                SelectedDebitNote.Id,
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

        private async Task CancelDebitNote()
        {
            if (SelectedDebitNote == null) return;

            if (string.IsNullOrWhiteSpace(cancelReasonInput))
            {
                ToastService.ShowWarning("Cancellation reason is required.");
                return;
            }

            var result = DebitNoteService.Cancel(
                SelectedDebitNote.Id,
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

        private async Task ReverseDebitNote()
        {
            if (SelectedDebitNote == null) return;

            if (string.IsNullOrWhiteSpace(reverseReasonInput))
            {
                ToastService.ShowWarning("Reversal reason is required.");
                return;
            }

            var result = DebitNoteService.Reverse(
                SelectedDebitNote.Id,
                reverseReasonInput,
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

        private async Task DeleteDebitNote()
        {
            if (SelectedDebitNote == null) return;

            var result = DebitNoteService.Delete(SelectedDebitNote.Id);

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


        string GetDebitReasonIcon(string code)
        {
            return code switch
            {
                DebitReasonCodes.UnderbillingCorrection => "ti ti-trending-up",
                DebitReasonCodes.AdditionalCharges => "ti ti-plus",
                DebitReasonCodes.LateFee => "ti ti-clock",
                DebitReasonCodes.FreightDelivery => "ti ti-truck",
                DebitReasonCodes.TaxShortCharged => "ti ti-receipt-tax",
                DebitReasonCodes.RateRevision => "ti ti-refresh",
                DebitReasonCodes.Other => "ti ti-dots",
                _ => "ti ti-info-circle"
            };
        }

        private string TruncateName(string? name, int maxLength)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;
            return name.Length <= maxLength ? name : name.Substring(0, maxLength) + "...";
        }

        private static string GetStatusBadgeClass(string status) => status switch
        {
            DebitNoteStatuses.Draft => "bg-secondary-transparent text-secondary",
            DebitNoteStatuses.Submitted => "bg-info-transparent text-info",
            DebitNoteStatuses.Approved => "bg-primary-transparent text-primary",
            DebitNoteStatuses.Posted => "bg-success-transparent text-success",
            DebitNoteStatuses.Cancelled => "bg-warning-transparent text-warning",
            DebitNoteStatuses.Reversed => "bg-danger-transparent text-danger",
            _ => "bg-secondary-transparent text-secondary"
        };
        private static string GetStatusDotBadgeClass(string status) => status switch
        {
            DebitNoteStatuses.Draft => "bg-secondary text-secondary",
            DebitNoteStatuses.Submitted => "bg-info text-info",
            DebitNoteStatuses.Approved => "bg-primary text-primary",
            DebitNoteStatuses.Posted => "bg-success text-success",
            DebitNoteStatuses.Cancelled => "bg-warning text-warning",
            DebitNoteStatuses.Reversed => "bg-danger text-danger",
            _ => "bg-secondary text-secondary"
        };

        private static string GetReasonBadgeClass(string reason) => reason switch
        {
            DebitReasonCodes.UnderbillingCorrection => "bg-primary-transparent text-primary",
            DebitReasonCodes.AdditionalCharges => "bg-info-transparent text-info",
            DebitReasonCodes.LateFee => "bg-danger-transparent text-danger",
            DebitReasonCodes.FreightDelivery => "bg-warning-transparent text-warning",
            DebitReasonCodes.TaxShortCharged => "bg-purple-transparent text-purple",
            DebitReasonCodes.RateRevision => "bg-success-transparent text-success",
            DebitReasonCodes.Other => "bg-secondary-transparent text-secondary",
            _ => "bg-secondary-transparent text-secondary"
        };

        #endregion
    }
}
