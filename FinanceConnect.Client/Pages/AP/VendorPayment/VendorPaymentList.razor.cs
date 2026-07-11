using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FinanceConnect.Client.Pages.AP.VendorPayment
{
    public partial class VendorPaymentList : ComponentBase
    {
        [Inject] private VendorPaymentService PaymentService { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        // Data
        private List<VendorPaymentViewModel> AllPayments = new();
        private List<VendorPaymentViewModel> FilteredPayments = new();
        private List<VendorPaymentViewModel> PagedPayments = new();
        private VendorPaymentStatisticsViewModel Statistics = new();
        private VendorPaymentViewModel? SelectedPayment;

        // Filters
        private string searchTerm = string.Empty;
        private string selectedStatus = string.Empty;
        private string selectedMethod = string.Empty;
        private DateTime? FromDate;
        private DateTime? ToDate;

        // Modal inputs
        private string cancelReasonInput = string.Empty;
        private string reversalReasonInput = string.Empty;
        private string reversalRefInput = string.Empty;

        // Pagination
        private int CurrentPage = 1;
        private int PageSize = 10;
        private int TotalPages => (int)Math.Ceiling((double)FilteredPayments.Count / PageSize);
        private int VisibleColumnCount;

        // Distinct values for filter dropdowns (only data in the table)
        private List<string> DistinctStatuses => AllPayments
            .Where(p => !string.IsNullOrEmpty(p.PaymentStatus))
            .Select(p => p.PaymentStatus)
            .Distinct()
            .OrderBy(s => s)
            .ToList();

        private List<string> DistinctMethods => AllPayments
            .Where(p => !string.IsNullOrEmpty(p.PaymentMethod))
            .Select(p => p.PaymentMethod)
            .Distinct()
            .OrderBy(m => m)
            .ToList();
        // Loading state
        private bool isInitialized = false;
        private bool isLoading = false;

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
            isInitialized = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
            VisibleColumnCount =
await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        private async Task LoadDataAsync()
        {
            isLoading = true;
            StateHasChanged();
            ToDate = null;
            FromDate = null;
            selectedMethod = null!;
            selectedStatus = null!;
            searchTerm = "";
            await Task.Delay(100); // Simulate async load

            AllPayments = PaymentService.GetAll();
            Statistics = PaymentService.GetStatistics();
            ApplyFilters();

            isLoading = false;
            StateHasChanged();

            await JS.InvokeVoidAsync("feather.replace");
        }

        private async Task OnRefreshAsync()
        {
            PaymentService.ResetToSeed();
            await LoadDataAsync();
            ToastService.ShowSuccess("Data refreshed successfully");
        }

        private void OnSearch(ChangeEventArgs e)
        {
            searchTerm = e.Value?.ToString() ?? string.Empty;
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            FilteredPayments = AllPayments;

            // Search filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();
                FilteredPayments = FilteredPayments.Where(p =>
                    p.PaymentNumber.ToLower().Contains(term) ||
                    (p.VendorCode?.ToLower().Contains(term) ?? false) ||
                    (p.VendorName?.ToLower().Contains(term) ?? false) ||
                    (p.PaymentReferenceNumber?.ToLower().Contains(term) ?? false) ||
                    (p.BankNameSnapshot?.ToLower().Contains(term) ?? false)
                ).ToList();
            }

            // Status filter
            if (!string.IsNullOrWhiteSpace(selectedStatus))
            {
                FilteredPayments = FilteredPayments.Where(p => p.PaymentStatus == selectedStatus).ToList();
            }

            // Method filter
            if (!string.IsNullOrWhiteSpace(selectedMethod))
            {
                FilteredPayments = FilteredPayments.Where(p => p.PaymentMethod == selectedMethod).ToList();
            }

            // Date range filter
            if (FromDate.HasValue)
            {
                FilteredPayments = FilteredPayments.Where(p => p.PaymentDate >= FromDate.Value).ToList();
            }
            if (ToDate.HasValue)
            {
                FilteredPayments = FilteredPayments.Where(p => p.PaymentDate <= ToDate.Value).ToList();
            }

            // Sort by date descending
            FilteredPayments = FilteredPayments.OrderByDescending(p => p.PaymentDate).ThenByDescending(p => p.CreatedAt).ToList();

            // Reset to first page
            CurrentPage = 1;
            ApplyPaging();
        }

        private void ApplyPaging()
        {
            PagedPayments = FilteredPayments
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }

        private void OnPageSizeChange(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int newSize))
            {
                PageSize = newSize;
                CurrentPage = 1;
                ApplyPaging();
            }
        }

        private void GoToPage(int page)
        {
            CurrentPage = page;
            ApplyPaging();
        }

        private void PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                ApplyPaging();
            }
        }

        private void NextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                ApplyPaging();
            }
        }

        // Modal handlers
        private void OpenRowDetails(VendorPaymentViewModel payment)
        {
            SelectedPayment = payment;
        }

        private void OpenApproveModal(VendorPaymentViewModel payment)
        {
            SelectedPayment = payment;
        }

        private void OpenPostModal(VendorPaymentViewModel payment)
        {
            SelectedPayment = payment;
        }

        private void OpenCancelModal(VendorPaymentViewModel payment)
        {
            SelectedPayment = payment;
            cancelReasonInput = string.Empty;
        }

        private void OpenReverseModal(VendorPaymentViewModel payment)
        {
            SelectedPayment = payment;
            reversalReasonInput = string.Empty;
            reversalRefInput = string.Empty;
        }

        private void OpenDeleteModal(VendorPaymentViewModel payment)
        {
            SelectedPayment = payment;
        }

        // Actions
        private async Task ApprovePayment()
        {
            if (SelectedPayment == null) return;

            var result = PaymentService.Approve(SelectedPayment.Id, "Current User");
            await CloseModal("approveModal");
            
            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                await LoadDataAsync();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        private async Task PostPayment()
        {
            if (SelectedPayment == null) return;

            var result = PaymentService.Post(SelectedPayment.Id, "Current User");
            await CloseModal("postModal");
            
            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                await LoadDataAsync();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        private async Task CancelPayment()
        {
            if (SelectedPayment == null) return;

            if (string.IsNullOrWhiteSpace(cancelReasonInput))
            {
                ToastService.ShowWarning("Cancellation reason is required.");
                return;
            }

            var result = PaymentService.Cancel(SelectedPayment.Id, cancelReasonInput, "Current User");
            await CloseModal("cancelModal");
            
            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                await LoadDataAsync();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        private async Task ReversePayment()
        {
            if (SelectedPayment == null) return;

            if (string.IsNullOrWhiteSpace(reversalReasonInput))
            {
                ToastService.ShowWarning("Reversal reason is required.");
                return;
            }

            var result = PaymentService.Reverse(SelectedPayment.Id, reversalReasonInput, reversalRefInput, "Current User");
            await CloseModal("reverseModal");
            
            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                await LoadDataAsync();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        private async Task DeletePayment()
        {
            if (SelectedPayment == null) return;

            var result = PaymentService.Delete(SelectedPayment.Id, "Current User");
            await CloseModal("deleteModal");
            
            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                await LoadDataAsync();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        // Helper methods
        private string TruncateName(string? name, int maxLength)
        {
            if (string.IsNullOrEmpty(name)) return "-";
            return name.Length <= maxLength ? name : name.Substring(0, maxLength) + "...";
        }

        private string GetPaymentStatusIcon(string status) => status switch
        {
            VendorPaymentStatuses.Draft => "ti ti-file-text",
            VendorPaymentStatuses.Submitted => "ti ti-send",
            VendorPaymentStatuses.Approved => "ti ti-check",
            VendorPaymentStatuses.Rejected => "ti ti-x",
            VendorPaymentStatuses.Posted => "ti ti-circle-check",
            VendorPaymentStatuses.Reversed => "ti ti-refresh",
            VendorPaymentStatuses.Cancelled => "ti ti-ban",
            _ => "ti ti-info-circle"
        };


        private string GetStatusBadgeClass(string status) => status switch
        {
            VendorPaymentStatuses.Draft => "bg-secondary-transparent text-secondary",
            VendorPaymentStatuses.Submitted => "bg-info-transparent text-info",
            VendorPaymentStatuses.Approved => "bg-primary-transparent text-primary",
            VendorPaymentStatuses.Rejected => "bg-warning-transparent text-warning",
            VendorPaymentStatuses.Posted => "bg-success-transparent text-success",
            VendorPaymentStatuses.Reversed => "bg-danger-transparent text-danger",
            VendorPaymentStatuses.Cancelled => "bg-warning-transparent text-warning",
            _ => "bg-secondary-transparent text-secondary"
        };


        private string GetPaymentMethodIcon(string method)
        {
            return method switch
            {
                VendorPaymentMethods.BankTransfer => "ti ti-building-bank",
                VendorPaymentMethods.UPI => "ti ti-qrcode",
                VendorPaymentMethods.Cheque => "ti ti-file-check",
                VendorPaymentMethods.Cash => "ti ti-cash",
                VendorPaymentMethods.Gateway => "ti ti-credit-card",
                VendorPaymentMethods.Other => "ti ti-dots",

                _ => "ti ti-help"
            };
        }

        private string GetMethodBadgeClass(string method) => method switch
        {
            VendorPaymentMethods.Cash => "bg-success-transparent text-success",
            VendorPaymentMethods.BankTransfer => "bg-primary-transparent text-primary",
            VendorPaymentMethods.UPI => "bg-info-transparent text-info",
            VendorPaymentMethods.Cheque => "bg-warning-transparent text-warning",
            VendorPaymentMethods.Gateway => "bg-purple-transparent text-purple",
            VendorPaymentMethods.Other => "bg-secondary-transparent text-secondary",
            _ => "bg-secondary-transparent text-secondary"
        };

        private async Task CloseModal(string modalId)
        {
            try
            {
                await JS.InvokeVoidAsync("eval", $"bootstrap.Modal.getInstance(document.getElementById('{modalId}'))?.hide()");
            }
            catch { }
        }
    }
}
