using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FinanceConnect.Client.Pages.AP.VendorCreditNote
{
    public partial class VendorCreditNoteList
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] VendorCreditNoteService CreditNoteService { get; set; } = default!;
        [Inject] VendorService VendorService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;

        private bool isInitialized = false;
        private bool isLoading = false;

        private List<VendorCreditNoteViewModel> CreditNotes = new();
        private List<VendorCreditNoteViewModel> FilteredCreditNotes = new();
        private VendorCreditNoteStatisticsViewModel Statistics = new();

        private string searchTerm = string.Empty;
        private string selectedStatus = string.Empty;
        private DateTime? FromDate;
        private DateTime? ToDate;
        
        private VendorCreditNoteViewModel? SelectedCreditNote;
        private string cancelReasonInput = string.Empty;

        // Pagination
        private int CurrentPage = 1;
        private int PageSize = 10;
        private int TotalPages => FilteredCreditNotes.Count > 0 ? (int)Math.Ceiling((double)FilteredCreditNotes.Count / PageSize) : 0;
        private IEnumerable<VendorCreditNoteViewModel> PagedCreditNotes => FilteredCreditNotes
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize);
        private int VisibleColumnCount;

        // Computed: only statuses present in the data
        private List<string> AvailableStatuses => CreditNotes
            .Select(cn => cn.CreditNoteStatus)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Distinct()
            .OrderBy(s => Array.IndexOf(VendorCreditNoteStatuses.All, s))
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
            CreditNoteService.ResetToSeed();
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
                    (cn.VendorCode?.ToLower().Contains(term) ?? false) ||
                    (cn.VendorName?.ToLower().Contains(term) ?? false) ||
                    (cn.VendorCreditNoteReferenceNumber?.ToLower().Contains(term) ?? false) ||
                    (cn.PrimaryVendorBillNumber?.ToLower().Contains(term) ?? false));
            }

            // Status filter
            if (!string.IsNullOrWhiteSpace(selectedStatus))
            {
                query = query.Where(cn => cn.CreditNoteStatus == selectedStatus);
            }

            // Date filters
            if (FromDate.HasValue)
            {
                query = query.Where(cn => cn.VendorCreditNoteDate >= FromDate.Value);
            }
            if (ToDate.HasValue)
            {
                query = query.Where(cn => cn.VendorCreditNoteDate <= ToDate.Value);
            }

            FilteredCreditNotes = query.OrderByDescending(cn => cn.VendorCreditNoteDate)
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

        private void OpenRowDetails(VendorCreditNoteViewModel cn)
        {
            SelectedCreditNote = cn;
        }

        private void OpenApproveModal(VendorCreditNoteViewModel cn)
        {
            SelectedCreditNote = cn;
        }

        private void OpenPostModal(VendorCreditNoteViewModel cn)
        {
            SelectedCreditNote = cn;
        }

        private void OpenCancelModal(VendorCreditNoteViewModel cn)
        {
            SelectedCreditNote = cn;
            cancelReasonInput = string.Empty;
        }

        private void OpenDeleteModal(VendorCreditNoteViewModel cn)
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
        private static string GetCreditNoteStatusIcon(string status) => status switch
        {
            VendorCreditNoteStatuses.Draft => "ti ti-file-text",
            VendorCreditNoteStatuses.Submitted => "ti ti-send",
            VendorCreditNoteStatuses.Approved => "ti ti-check",
            VendorCreditNoteStatuses.Rejected => "ti ti-x",
            VendorCreditNoteStatuses.Posted => "ti ti-circle-check",
            VendorCreditNoteStatuses.Cancelled => "ti ti-ban",
            VendorCreditNoteStatuses.Reversed => "ti ti-refresh",
            _ => "ti ti-info-circle"
        };


        private static string GetStatusBadgeClass(string status) => status switch
        {
            VendorCreditNoteStatuses.Draft => "bg-secondary-transparent text-secondary",
            VendorCreditNoteStatuses.Submitted => "bg-info-transparent text-info",
            VendorCreditNoteStatuses.Approved => "bg-primary-transparent text-primary",
            VendorCreditNoteStatuses.Rejected => "bg-danger-transparent text-danger",
            VendorCreditNoteStatuses.Posted => "bg-success-transparent text-success",
            VendorCreditNoteStatuses.Cancelled => "bg-warning-transparent text-warning",
            VendorCreditNoteStatuses.Reversed => "bg-dark-transparent text-dark",
            _ => "bg-secondary-transparent text-secondary"
        };

        private string GetCreditNoteTypeIcon(string type)
        {
            return type switch
            {
                VendorCreditNoteTypes.PurchaseReturn => "ti ti-arrow-back-up",
                VendorCreditNoteTypes.PriceReduction => "ti ti-trending-down",
                VendorCreditNoteTypes.DiscountRebate => "ti ti-discount",
                VendorCreditNoteTypes.BillingCorrection => "ti ti-edit",
                VendorCreditNoteTypes.DamageClaim => "ti ti-alert-circle",
                VendorCreditNoteTypes.Other => "ti ti-dots",

                _ => "ti ti-help"
            };
        }


        private static string GetTypeBadgeClass(string type) => type switch
        {
            VendorCreditNoteTypes.PurchaseReturn => "bg-warning-transparent text-warning",
            VendorCreditNoteTypes.PriceReduction => "bg-info-transparent text-info",
            VendorCreditNoteTypes.DiscountRebate => "bg-primary-transparent text-primary",
            VendorCreditNoteTypes.BillingCorrection => "bg-danger-transparent text-danger",
            VendorCreditNoteTypes.DamageClaim => "bg-secondary-transparent text-secondary",
            VendorCreditNoteTypes.Other => "bg-light text-dark",
            _ => "bg-secondary-transparent text-secondary"
        };

        #endregion
    }
}
