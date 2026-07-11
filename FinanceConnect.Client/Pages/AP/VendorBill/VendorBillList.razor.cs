using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FinanceConnect.Client.Pages.AP.VendorBill
{
    public partial class VendorBillList : ComponentBase
    {
        [Inject] private IJSRuntime JS { get; set; } = default!;

        // State
        private bool isInitialized = false;
        private bool isLoading = false;
        private List<VendorBillViewModel> allBills = new();
        private VendorBillStatisticsViewModel Statistics = new();

        // Selected bill for modals
        private VendorBillViewModel? SelectedBill;
        private string CancellationReason = string.Empty;

        // Filtering
        private string searchText = string.Empty;
        private string _selectedBillType = string.Empty;
        private string _selectedStatus = string.Empty;
        private DateTime? _fromDate;
        private DateTime? _toDate;
        private int VisibleColumnCount;
        public string SelectedBillType
        {
            get => _selectedBillType;
            set
            {
                if (_selectedBillType != value)
                {
                    _selectedBillType = value;
                    CurrentPage = 1;
                }
            }
        }

        public string SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                if (_selectedStatus != value)
                {
                    _selectedStatus = value;
                    CurrentPage = 1;
                }
            }
        }

        public DateTime? FromDate
        {
            get => _fromDate;
            set
            {
                if (_fromDate != value)
                {
                    _fromDate = value;
                    CurrentPage = 1;
                }
            }
        }

        public DateTime? ToDate
        {
            get => _toDate;
            set
            {
                if (_toDate != value)
                {
                    _toDate = value;
                    CurrentPage = 1;
                }
            }
        }

        // Pagination
        private int CurrentPage = 1;
        private int PageSize = 10;

        // Distinct values for filter dropdowns (only data in the table)
        private List<string> DistinctBillTypes => allBills
            .Where(b => !string.IsNullOrEmpty(b.BillType))
            .Select(b => b.BillType)
            .Distinct()
            .OrderBy(t => t)
            .ToList();

        private List<string> DistinctStatuses => allBills
            .Where(b => !string.IsNullOrEmpty(b.BillStatus))
            .Select(b => b.BillStatus)
            .Distinct()
            .OrderBy(s => s)
            .ToList();

        private IEnumerable<VendorBillViewModel> FilteredBills => allBills
            .Where(b => string.IsNullOrEmpty(searchText) ||
                        b.BillNumber.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                        (b.VendorCode?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (b.VendorName?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (b.VendorInvoiceNumber?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false))
            .Where(b => string.IsNullOrEmpty(SelectedBillType) || b.BillType == SelectedBillType)
            .Where(b => string.IsNullOrEmpty(SelectedStatus) || b.BillStatus == SelectedStatus)
            .Where(b => !FromDate.HasValue || b.BillDate >= FromDate.Value)
            .Where(b => !ToDate.HasValue || b.BillDate <= ToDate.Value)
            .OrderByDescending(b => b.BillDate)
            .ThenByDescending(b => b.BillNumber);

        private int TotalPages => (int)Math.Ceiling(FilteredBills.Count() / (double)PageSize);

        private IEnumerable<VendorBillViewModel> PagedBills => FilteredBills
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
            //await JS.InvokeVoidAsync("eval", "var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle=\"tooltip\"]')); var tooltipList = tooltipTriggerList.map(function (el) { return new bootstrap.Tooltip(el); });");
        }

        private async Task LoadData()
        {
            isLoading = true;
            StateHasChanged();
            ToDate = null;
            FromDate = null;
            SelectedStatus = null;
            SelectedBillType = null;
            searchText = "";
            await Task.Delay(100); // Simulate network delay

            allBills = BillService.GetAll();
            Statistics = BillService.GetStatistics();

            isLoading = false;
            StateHasChanged();
        }

        private async Task OnRefreshAsync()
        {
            isLoading = true;
            StateHasChanged();

            await Task.Delay(200);
            BillService.ResetToSeed();
            await LoadData();
            ToastService.ShowSuccess("Data refreshed successfully");
        }

        private async Task OnSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString() ?? string.Empty;
            CurrentPage = 1;
            VisibleColumnCount =
    await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        private void GoToPage(int page)
        {
            if (page >= 1 && page <= TotalPages)
            {
                CurrentPage = page;
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

        // Modal handlers
        private void OpenPostModal(VendorBillViewModel bill)
        {
            SelectedBill = bill;
        }

        private void OpenCancelModal(VendorBillViewModel bill)
        {
            SelectedBill = bill;
            CancellationReason = string.Empty;
        }

        private void OpenDeleteModal(VendorBillViewModel bill)
        {
            SelectedBill = bill;
        }

        private void OpenRowDetails(VendorBillViewModel bill)
        {
            SelectedBill = bill;
        }

        // Actions
        private async Task PostBill()
        {
            if (SelectedBill == null) return;

            var result = BillService.Post(SelectedBill.Id, Guid.NewGuid(), "Current User");
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

        private async Task CancelBill()
        {
            if (SelectedBill == null) return;

            if (string.IsNullOrWhiteSpace(CancellationReason))
            {
                ToastService.ShowError("Cancellation reason is required");
                return;
            }

            var result = BillService.Cancel(SelectedBill.Id, CancellationReason, Guid.NewGuid(), "Current User");
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

        private async Task DeleteBill()
        {
            if (SelectedBill == null) return;

            var result = BillService.Delete(SelectedBill.Id);
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

        // Helpers
        private string TruncateName(string? name, int maxLength)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;
            return name.Length <= maxLength ? name : name.Substring(0, maxLength) + "...";
        }

        private bool IsOverdue(VendorBillViewModel bill)
        {
            return bill.DueDate < DateTime.Today &&
                   bill.OutstandingAmount > 0 &&
                   bill.BillStatus == VendorBillStatuses.Posted;
        }

        private string GetTypeBadgeClass(string type) => type switch
        {
            BillTypes.GoodsPurchase => "bg-info",
            BillTypes.ServiceExpense => "bg-primary",
            BillTypes.Utility => "bg-secondary",
            BillTypes.Rent => "bg-warning",
            BillTypes.Contractor => "bg-success",
            BillTypes.Other => "bg-dark",
            _ => "bg-secondary"
        };
        private string GetStatusIcon(string status) => status switch
        {
            VendorBillStatuses.Draft => "ti ti-file-text",
            VendorBillStatuses.Submitted => "ti ti-send",
            VendorBillStatuses.Approved => "ti ti-check",
            VendorBillStatuses.Posted => "ti ti-circle-check",
            VendorBillStatuses.Cancelled => "ti ti-x",
            VendorBillStatuses.Reversed => "ti ti-refresh",
            VendorBillStatuses.Rejected => "ti ti-alert-circle",
            _ => "ti ti-info-circle"
        };


        private string GetStatusBadgeClass(string status) => status switch
        {
            VendorBillStatuses.Draft => "bg-secondary",
            VendorBillStatuses.Submitted => "bg-info",
            VendorBillStatuses.Approved => "bg-primary",
            VendorBillStatuses.Posted => "bg-success",
            VendorBillStatuses.Cancelled => "bg-dark",
            VendorBillStatuses.Reversed => "bg-warning",
            VendorBillStatuses.Rejected => "bg-danger",
            _ => "bg-secondary"
        };
    }
}
