using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FinanceConnect.Client.Pages.AR.CustomerPayment
{
    public partial class CustomerPaymentList : ComponentBase
    {
        [Inject] private CustomerPaymentService PaymentService { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        // Data
        private List<CustomerPaymentViewModel> AllPayments = new();
        private List<CustomerPaymentViewModel> FilteredPayments = new();
        private List<CustomerPaymentViewModel> PagedPayments = new();
        private PaymentStatisticsViewModel Statistics = new();
        private CustomerPaymentViewModel? SelectedPayment;

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

        // Distinct filter values from actual data
        private List<string> DistinctStatuses => AllPayments
            .Select(p => p.PaymentStatus)
            .Where(s => !string.IsNullOrEmpty(s))
            .Distinct()
            .OrderBy(s => s)
            .ToList();

        private List<string> DistinctMethods => AllPayments
            .Select(p => p.PaymentMethod)
            .Where(m => !string.IsNullOrEmpty(m))
            .Distinct()
            .OrderBy(m => m)
            .ToList();

        // Loading state
        private bool isInitialized = false;
        private bool isLoading = false;
        private int VisibleColumnCount;
        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
            isInitialized = true;

        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips", true);
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
            await JS.InvokeVoidAsync("initTooltips");
        }

        private async Task OnRefreshAsync()
        {
            await LoadDataAsync();
            await ShowToast("success", "Data refreshed successfully");
        }

        private async Task OnSearch(ChangeEventArgs e)
        {
            searchTerm = e.Value?.ToString() ?? string.Empty;
            ApplyFilters();
            VisibleColumnCount =
await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        private void ApplyFilters()
        {
            FilteredPayments = AllPayments;

            // Search filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();
                FilteredPayments = FilteredPayments.Where(p =>
                    p.ReceiptNumber.ToLower().Contains(term) ||
                    (p.CustomerCode?.ToLower().Contains(term) ?? false) ||
                    (p.CustomerName?.ToLower().Contains(term) ?? false) ||
                    (p.InstrumentNumber?.ToLower().Contains(term) ?? false) ||
                    (p.BankName?.ToLower().Contains(term) ?? false)
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
                FilteredPayments = FilteredPayments.Where(p => p.ReceiptDate >= FromDate.Value).ToList();
            }
            if (ToDate.HasValue)
            {
                FilteredPayments = FilteredPayments.Where(p => p.ReceiptDate <= ToDate.Value).ToList();
            }

            // Sort by date descending
            FilteredPayments = FilteredPayments.OrderByDescending(p => p.ReceiptDate).ThenByDescending(p => p.CreatedAt).ToList();

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
        private void OpenRowDetails(CustomerPaymentViewModel payment)
        {
            SelectedPayment = payment;
        }

        private void OpenApproveModal(CustomerPaymentViewModel payment)
        {
            SelectedPayment = payment;
        }

        private void OpenPostModal(CustomerPaymentViewModel payment)
        {
            SelectedPayment = payment;
        }

        private void OpenCancelModal(CustomerPaymentViewModel payment)
        {
            SelectedPayment = payment;
            cancelReasonInput = string.Empty;
        }

        private void OpenReverseModal(CustomerPaymentViewModel payment)
        {
            SelectedPayment = payment;
            reversalReasonInput = string.Empty;
            reversalRefInput = string.Empty;
        }

        private void OpenDeleteModal(CustomerPaymentViewModel payment)
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
                await ShowToast("success", result.Message);
                await LoadDataAsync();
            }
            else
            {
                await ShowToast("error", result.Message);
            }
        }

        private async Task PostPayment()
        {
            if (SelectedPayment == null) return;

            var result = PaymentService.Post(SelectedPayment.Id, "Current User");
            await CloseModal("postModal");
            
            if (result.Success)
            {
                await ShowToast("success", result.Message);
                await LoadDataAsync();
            }
            else
            {
                await ShowToast("error", result.Message);
            }
        }

        private async Task CancelPayment()
        {
            if (SelectedPayment == null) return;

            if (string.IsNullOrWhiteSpace(cancelReasonInput))
            {
                await ShowToast("error", "Cancellation reason is required.");
                return;
            }

            var result = PaymentService.Cancel(SelectedPayment.Id, cancelReasonInput, "Current User");
            await CloseModal("cancelModal");
            
            if (result.Success)
            {
                await ShowToast("success", result.Message);
                await LoadDataAsync();
            }
            else
            {
                await ShowToast("error", result.Message);
            }
        }

        private async Task ReversePayment()
        {
            if (SelectedPayment == null) return;

            if (string.IsNullOrWhiteSpace(reversalReasonInput))
            {
                await ShowToast("error", "Reversal reason is required.");
                return;
            }

            var result = PaymentService.Reverse(SelectedPayment.Id, reversalReasonInput, reversalRefInput, "Current User");
            await CloseModal("reverseModal");
            
            if (result.Success)
            {
                await ShowToast("success", result.Message);
                await LoadDataAsync();
            }
            else
            {
                await ShowToast("error", result.Message);
            }
        }

        private async Task DeletePayment()
        {
            if (SelectedPayment == null) return;

            var result = PaymentService.Delete(SelectedPayment.Id, "Current User");
            await CloseModal("deleteModal");
            
            if (result.Success)
            {
                await ShowToast("success", result.Message);
                await LoadDataAsync();
            }
            else
            {
                await ShowToast("error", result.Message);
            }
        }

        // Helper methods
        private string TruncateName(string? name, int maxLength)
        {
            if (string.IsNullOrEmpty(name)) return "-";
            return name.Length <= maxLength ? name : name.Substring(0, maxLength) + "...";
        }


        string GetPaymentMethodIcon(string code)
        {
            return code switch
            {
                PaymentMethods.Cash => "ti ti-cash",
                PaymentMethods.BankTransfer => "ti ti-building-bank",
                PaymentMethods.Cheque => "ti ti-file-text",
                PaymentMethods.UPI => "ti ti-qrcode",
                PaymentMethods.Card => "ti ti-credit-card",
                PaymentMethods.Wallet => "ti ti-wallet",
                PaymentMethods.Gateway => "ti ti-world",
                PaymentMethods.Other => "ti ti-dots",
                _ => "ti ti-info-circle"
            };
        }

        private string GetStatusBadgeClass(string status) => status switch
        {
            PaymentStatuses.Draft => "bg-secondary-transparent text-secondary",
            PaymentStatuses.Submitted => "bg-info-transparent text-info",
            PaymentStatuses.Approved => "bg-primary-transparent text-primary",
            PaymentStatuses.Posted => "bg-success-transparent text-success",
            PaymentStatuses.Reversed => "bg-danger-transparent text-danger",
            PaymentStatuses.Cancelled => "bg-warning-transparent text-warning",
            _ => "bg-secondary-transparent text-secondary"
        };
        private string GetStatusDotBadgeClass(string status) => status switch
        {
            PaymentStatuses.Draft => "bg-secondary text-secondary",
            PaymentStatuses.Submitted => "bg-info text-info",
            PaymentStatuses.Approved => "bg-primary text-primary",
            PaymentStatuses.Posted => "bg-success text-success",
            PaymentStatuses.Reversed => "bg-danger text-danger",
            PaymentStatuses.Cancelled => "bg-warning text-warning",
            _ => "bg-secondary text-secondary"
        };

        private string GetMethodBadgeClass(string method) => method switch
        {
            PaymentMethods.Cash => "bg-success-transparent text-success",
            PaymentMethods.BankTransfer => "bg-primary-transparent text-primary",
            PaymentMethods.UPI => "bg-info-transparent text-info",
            PaymentMethods.Cheque => "bg-warning-transparent text-warning",
            PaymentMethods.Card => "bg-purple-transparent text-purple",
            PaymentMethods.Gateway => "bg-indigo-transparent text-indigo",
            _ => "bg-secondary-transparent text-secondary"
        };

        private async Task ShowToast(string type, string message)
        {
            try
            {
                await JS.InvokeVoidAsync("eval", $"Swal.fire({{ toast: true, position: 'top-end', icon: '{type}', title: '{message.Replace("'", "\\'")}', showConfirmButton: false, timer: 3000 }})");
            }
            catch { }
        }

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
