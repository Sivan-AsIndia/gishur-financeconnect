using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.AR.CustomerInvoice
{
    public partial class CustomerInvoiceList : ComponentBase
    {
        [Inject] private IJSRuntime JS { get; set; } = default!;

        // State
        private bool isInitialized = false;
        private bool isLoading = false;
        private List<CustomerInvoiceViewModel> allInvoices = new();

        // Selected invoice for modals
        private CustomerInvoiceViewModel? SelectedInvoice { get; set; }

        // Search and filter
        private string searchText = string.Empty;
        private string _selectedInvoiceType = string.Empty;
        private string _selectedStatus = string.Empty;
        private DateTime? _fromDate;
        private DateTime? _toDate;
        private int VisibleColumnCount;

        // Pagination
        private int CurrentPage = 1;
        private int PageSize = 10;
        // Modal inputs
        private string cancelReasonInput = string.Empty;

        // Statistics
        private InvoiceStatisticsViewModel Statistics = new();

        // Filter properties with auto-refresh
        private string SelectedInvoiceType
        {
            get => _selectedInvoiceType;
            set { _selectedInvoiceType = value; CurrentPage = 1; }
        }

        private string SelectedStatus
        {
            get => _selectedStatus;
            set { _selectedStatus = value; CurrentPage = 1; }
        }

        private DateTime? FromDate
        {
            get => _fromDate;
            set { _fromDate = value; CurrentPage = 1; }
        }

        private DateTime? ToDate
        {
            get => _toDate;
            set { _toDate = value; CurrentPage = 1; }
        }

        // Filtered invoices
        private IEnumerable<CustomerInvoiceViewModel> FilteredInvoices
        {
            get
            {
                var result = allInvoices.AsEnumerable();

                // Search filter
                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    var search = searchText.ToLower();
                    result = result.Where(i =>
                        i.InvoiceNumber.ToLower().Contains(search) ||
                        (i.CustomerCode?.ToLower().Contains(search) ?? false) ||
                        (i.CustomerName?.ToLower().Contains(search) ?? false) ||
                        (i.ReferenceText?.ToLower().Contains(search) ?? false));
                }

                // Invoice type filter
                if (!string.IsNullOrEmpty(_selectedInvoiceType))
                {
                    result = result.Where(i => i.InvoiceType == _selectedInvoiceType);
                }

                // Status filter
                if (!string.IsNullOrEmpty(_selectedStatus))
                {
                    result = result.Where(i => i.InvoiceStatus == _selectedStatus);
                }

                // Date range filter
                if (_fromDate.HasValue)
                {
                    result = result.Where(i => i.InvoiceDate >= _fromDate.Value);
                }
                if (_toDate.HasValue)
                {
                    result = result.Where(i => i.InvoiceDate <= _toDate.Value);
                }

                return result.OrderByDescending(i => i.InvoiceDate).ThenByDescending(i => i.InvoiceNumber);
            }
        }

        // Paged invoices
        private IEnumerable<CustomerInvoiceViewModel> PagedInvoices =>
            FilteredInvoices.Skip((CurrentPage - 1) * PageSize).Take(PageSize);

        private int TotalPages => (int)Math.Ceiling(FilteredInvoices.Count() / (double)PageSize);

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

        string GetInvoiceTypeIcon(string type)
        {
            return type switch
            {
                InvoiceTypes.Standard => "ti ti-file-text",
                InvoiceTypes.Proforma => "ti ti-file-description",
                InvoiceTypes.Export => "ti ti-plane",
                InvoiceTypes.SEZ => "ti ti-building",
                InvoiceTypes.AdjustmentInvoice => "ti ti-adjustments",
                _ => "ti ti-file"
            };
        }

        private async Task LoadData()
        {
            isLoading = true;
            StateHasChanged();

            await Task.Delay(100); // Simulate async operation

            allInvoices = InvoiceService.GetAll();
            Statistics = InvoiceService.GetStatistics();

            isLoading = false;
            StateHasChanged();
        }

        private async Task OnRefreshAsync()
        {
            await LoadData();
            ToDate = null;
            FromDate = null;
            SelectedStatus = null!;
            SelectedInvoiceType = null!;
            searchText = "";
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
            if (int.TryParse(e.Value?.ToString(), out int size))
            {
                PageSize = size;
                CurrentPage = 1;
            }
        }

        private async Task ViewInvoice(CustomerInvoiceViewModel invoice)
        {
            SelectedInvoice = invoice;
            await JS.InvokeVoidAsync("eval", "$('#viewInvoiceModal').modal('show')");
        }

        private void OpenRowDetails(CustomerInvoiceViewModel invoice)
        {
            SelectedInvoice = invoice;
        }

        private async Task OpenPostModal(CustomerInvoiceViewModel invoice)
        {
            SelectedInvoice = invoice;
            await JS.InvokeVoidAsync("eval", "$('#postModal').modal('show')");
        }

        private async Task OpenCancelModal(CustomerInvoiceViewModel invoice)
        {
            SelectedInvoice = invoice;
            cancelReasonInput = string.Empty;
            await JS.InvokeVoidAsync("eval", "$('#cancelModal').modal('show')");
        }

        private async Task OpenDeleteModal(CustomerInvoiceViewModel invoice)
        {
            SelectedInvoice = invoice;
            await JS.InvokeVoidAsync("eval", "$('#deleteModal').modal('show')");
        }

        private async Task PostInvoice()
        {
            if (SelectedInvoice == null) return;

            var result = InvoiceService.Post(
                SelectedInvoice.Id,
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                "Current User");

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                await LoadData();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }

            await JS.InvokeVoidAsync("eval", "$('#postModal').modal('hide')");
        }

        private async Task CancelInvoice()
        {
            if (SelectedInvoice == null) return;

            if (string.IsNullOrWhiteSpace(cancelReasonInput))
            {
                ToastService.ShowWarning("Cancellation reason is required.");
                return;
            }

            var result = InvoiceService.Cancel(
                SelectedInvoice.Id,
                cancelReasonInput,
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                "Current User");

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                await LoadData();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }

            await JS.InvokeVoidAsync("eval", "$('#cancelModal').modal('hide')");
        }

        private async Task DeleteInvoice()
        {
            if (SelectedInvoice == null) return;

            var result = InvoiceService.Delete(SelectedInvoice.Id);

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                await LoadData();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }

            await JS.InvokeVoidAsync("eval", "$('#deleteModal').modal('hide')");
        }

        // Helper methods
        private string TruncateName(string? name, int maxLength)
        {
            if (string.IsNullOrEmpty(name)) return "-";
            return name.Length <= maxLength ? name : name.Substring(0, maxLength) + "...";
        }

        private bool IsOverdue(CustomerInvoiceViewModel invoice)
        {
            return invoice.DueDate < DateTime.Today &&
                   invoice.AmountOutstanding > 0 &&
                   (invoice.InvoiceStatus == InvoiceStatuses.Posted ||
                    invoice.InvoiceStatus == InvoiceStatuses.PartiallyPaid);
        }

        private string GetTypeBadgeClass(string type) => type switch
        {
            InvoiceTypes.Standard => "bg-primary",
            InvoiceTypes.Proforma => "bg-info",
            InvoiceTypes.Export => "bg-success",
            InvoiceTypes.SEZ => "bg-warning text-dark",
            InvoiceTypes.AdjustmentInvoice => "bg-secondary",
            _ => "bg-secondary"
        };

        private string GetStatusBadgeClass(string status) => status switch
        {
            InvoiceStatuses.Draft => "bg-secondary",
            InvoiceStatuses.Submitted => "bg-info",
            InvoiceStatuses.Approved => "bg-primary",
            InvoiceStatuses.Posted => "bg-warning text-dark",
            InvoiceStatuses.PartiallyPaid => "bg-orange text-white",
            InvoiceStatuses.Paid => "bg-success",
            InvoiceStatuses.Cancelled => "bg-danger",
            InvoiceStatuses.Voided => "bg-dark",
            _ => "bg-secondary"
        };
    }
}
