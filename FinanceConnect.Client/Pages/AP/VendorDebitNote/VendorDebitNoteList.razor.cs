using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FinanceConnect.Client.Pages.AP.VendorDebitNote
{
    public partial class VendorDebitNoteList : ComponentBase
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] VendorDebitNoteService DebitNoteService { get; set; } = default!;
        [Inject] VendorService VendorService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;

        private bool isInitialized = false;
        private bool isLoading = false;

        private List<VendorDebitNoteViewModel> DebitNotes = new();
        private List<VendorDebitNoteViewModel> FilteredDebitNotes = new();
        private VendorDebitNoteStatisticsViewModel Statistics = new();

        private string searchText = string.Empty;
        private string selectedDebitNoteType = string.Empty;
        private string selectedStatus = string.Empty;
        private DateTime? FromDate;
        private DateTime? ToDate;

        private VendorDebitNoteViewModel? SelectedDebitNote;
        private string cancelReasonInput = string.Empty;
        private string reverseReasonInput = string.Empty;
        private int VisibleColumnCount;

        // Computed: only types present in the data
        private List<string> AvailableTypes => DebitNotes
            .Select(dn => dn.DebitNoteType)
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct()
            .OrderBy(t => Array.IndexOf(VendorDebitNoteTypes.All, t))
            .ToList();

        // Computed: only statuses present in the data
        private List<string> AvailableStatuses => DebitNotes
            .Select(dn => dn.DebitNoteStatus)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Distinct()
            .OrderBy(s => Array.IndexOf(VendorDebitNoteStatuses.All, s))
            .ToList();
        // Pagination
        private int CurrentPage = 1;
        private int PageSize = 10;
        private int TotalPages => FilteredDebitNotes.Count > 0 ? (int)Math.Ceiling((double)FilteredDebitNotes.Count / PageSize) : 0;
        private IEnumerable<VendorDebitNoteViewModel> PagedDebitNotes => FilteredDebitNotes
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize);

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
            selectedStatus = null;
            selectedDebitNoteType = null;
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
            DebitNoteService.ResetToSeed();
            await LoadData();
            ToastService.ShowSuccess("Data refreshed successfully");
        }

        private void OnSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString() ?? string.Empty;
            CurrentPage = 1;
            ApplyFilters();
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
                    (dn.VendorCode?.ToLower().Contains(term) ?? false) ||
                    (dn.VendorName?.ToLower().Contains(term) ?? false) ||
                    (dn.VendorDebitNoteReferenceNumber?.ToLower().Contains(term) ?? false) ||
                    (dn.PrimaryVendorBillNumber?.ToLower().Contains(term) ?? false));
            }

            // Debit note type filter
            if (!string.IsNullOrWhiteSpace(selectedDebitNoteType))
            {
                query = query.Where(dn => dn.DebitNoteType == selectedDebitNoteType);
            }

            // Status filter
            if (!string.IsNullOrWhiteSpace(selectedStatus))
            {
                query = query.Where(dn => dn.DebitNoteStatus == selectedStatus);
            }

            // Date filters
            if (FromDate.HasValue)
            {
                query = query.Where(dn => dn.VendorDebitNoteDate >= FromDate.Value);
            }
            if (ToDate.HasValue)
            {
                query = query.Where(dn => dn.VendorDebitNoteDate <= ToDate.Value);
            }

            FilteredDebitNotes = query.OrderByDescending(dn => dn.VendorDebitNoteDate)
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

        private void OpenRowDetails(VendorDebitNoteViewModel dn)
        {
            SelectedDebitNote = dn;
        }

        private void OpenSubmitModal(VendorDebitNoteViewModel dn)
        {
            SelectedDebitNote = dn;
        }

        private void OpenPostModal(VendorDebitNoteViewModel dn)
        {
            SelectedDebitNote = dn;
        }

        private void OpenCancelModal(VendorDebitNoteViewModel dn)
        {
            SelectedDebitNote = dn;
            cancelReasonInput = string.Empty;
        }

        private void OpenReverseModal(VendorDebitNoteViewModel dn)
        {
            SelectedDebitNote = dn;
            reverseReasonInput = string.Empty;
        }

        private void OpenDeleteModal(VendorDebitNoteViewModel dn)
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

        private string TruncateName(string? name, int maxLength)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;
            return name.Length <= maxLength ? name : name.Substring(0, maxLength) + "...";
        }
        private static string GetDebitNoteStatusIcon(string status) => status switch
        {
            VendorDebitNoteStatuses.Draft => "ti ti-file-text",
            VendorDebitNoteStatuses.Submitted => "ti ti-send",
            VendorDebitNoteStatuses.Approved => "ti ti-check",
            VendorDebitNoteStatuses.Posted => "ti ti-circle-check",
            VendorDebitNoteStatuses.Cancelled => "ti ti-ban",
            VendorDebitNoteStatuses.Reversed => "ti ti-refresh",
            _ => "ti ti-info-circle"
        };



        private static string GetStatusBadgeClass(string status) => status switch
        {
            VendorDebitNoteStatuses.Draft => "bg-secondary-transparent text-secondary",
            VendorDebitNoteStatuses.Submitted => "bg-info-transparent text-info",
            VendorDebitNoteStatuses.Approved => "bg-primary-transparent text-primary",
            VendorDebitNoteStatuses.Posted => "bg-success-transparent text-success",
            VendorDebitNoteStatuses.Cancelled => "bg-warning-transparent text-warning",
            VendorDebitNoteStatuses.Reversed => "bg-danger-transparent text-danger",
            _ => "bg-secondary-transparent text-secondary"
        };

        private string GetDebitNoteTypeIcon(string type)
        {
            return type switch
            {
                VendorDebitNoteTypes.PriceIncrease => "ti ti-trending-up",
                VendorDebitNoteTypes.FreightCharges => "ti ti-truck",
                VendorDebitNoteTypes.PenaltyCharges => "ti ti-alert-triangle",
                VendorDebitNoteTypes.ServiceAddOn => "ti ti-tool",
                VendorDebitNoteTypes.TaxDifference => "ti ti-receipt-tax",
                VendorDebitNoteTypes.BillingCorrection => "ti ti-edit",
                VendorDebitNoteTypes.Other => "ti ti-dots",

                _ => "ti ti-help"
            };
        }

        private static string GetTypeBadgeClass(string type) => type switch
        {
            VendorDebitNoteTypes.PriceIncrease => "bg-primary-transparent text-primary",
            VendorDebitNoteTypes.FreightCharges => "bg-info-transparent text-info",
            VendorDebitNoteTypes.PenaltyCharges => "bg-danger-transparent text-danger",
            VendorDebitNoteTypes.ServiceAddOn => "bg-warning-transparent text-warning",
            VendorDebitNoteTypes.TaxDifference => "bg-purple-transparent text-purple",
            VendorDebitNoteTypes.BillingCorrection => "bg-success-transparent text-success",
            VendorDebitNoteTypes.Other => "bg-secondary-transparent text-secondary",
            _ => "bg-secondary-transparent text-secondary"
        };

        #endregion
    }
}
