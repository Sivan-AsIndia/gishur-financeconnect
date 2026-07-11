using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static FinanceConnect.Client.Data.MasterDataIds;

namespace FinanceConnect.Client.Pages.Sales.Quotations
{
    public partial class QuotationList
    {
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
            VisibleColumnCount =
            await JS.InvokeAsync<int>("getVisibleTableColumns");
        }
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] QuotationService QuotationService { get; set; } = default!;
        [Inject] CustomerService CustomerService { get; set; } = default!;

        private bool isInitialized = false;
        private bool isLoading = false;



        List<QuotationViewModel> Quotations = new();
        List<QuotationViewModel> FilteredQuotations = new();
        public List<CompanyModel> Companies = new();
        List<CustomerViewModel> Customers = new();

        string searchText = "";
        private bool canDelete = true;
        Guid? selectedCustomers;
        QuotationStatus? selectedStatus = null;
        Guid? selectedCompany = null;
        int PageSize = 10;
        int CurrentPage = 1;
        int PageWindowSize = 2;
        private int VisibleColumnCount;
        QuotationViewModel? SelectedQuotation;

        int TotalPages =>
        FilteredQuotations.Count == 0
        ? 1
        : (int)Math.Ceiling((double)FilteredQuotations.Count / PageSize);

        List<QuotationViewModel> PagedQuotations =>
            FilteredQuotations
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

        Guid? SelectedCompany
        {
            get => selectedCompany;
            set
            {
                selectedCompany = value;
                onCompanyChange();
                ApplyFilters();
            }
        }

        Guid? SelectedCustomers
        {
            get => selectedCustomers;
            set
            {
                selectedCustomers = value;
                ApplyFilters();
            }
        }

        QuotationStatus? SelectedStatus
        {
            get => selectedStatus;
            set
            {
                selectedStatus = value;
                ApplyFilters();
            }
        }

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


        protected override async Task OnInitializedAsync()
        {
            LoadData();
            isInitialized = true;
        }
        void LoadData()
        {
            Companies = MasterDataService.GetAllCompanies().Where(c => c.Status == "Active").ToList();
            Quotations = QuotationService.GetAll();

            Customers = CustomerService.GetAll();

            FilteredQuotations = Quotations;
        }

        void OnSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString() ?? "";
            ApplyFilters();
        }

        void OnPageSizeChange(ChangeEventArgs e)
        {
            PageSize = int.Parse(e.Value!.ToString()!);
            CurrentPage = 1;
        }
      
        void onCompanyChange()
        {
            Customers.Clear();
            Customers = CustomerService.GetAll().Where(c => c.CompanyId == selectedCompany).ToList();
        }


        void ApplyFilters()
        {
            IEnumerable<QuotationViewModel> query = Quotations;

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(x =>
                    x.Subject.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    x.QuotationNumber.Contains(searchText, StringComparison.OrdinalIgnoreCase));
            }
            // Company filter
            if (selectedCompany.HasValue)
            {
                query = query.Where(b =>
                     b.CompanyId != null &&
                     b.CompanyId == selectedCompany.Value);
            }

            if (selectedCustomers.HasValue)
                query = query.Where(x => x.CustomerId == selectedCustomers);

            if (selectedStatus.HasValue)
            {
                query = query.Where(x => x.Status == selectedStatus.Value);
            }

            FilteredQuotations = query.ToList();
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

        void ViewQuotation(QuotationViewModel q)
        {
            Nav.NavigateTo($"/quotations/{q.Id}/view");
        }

        void ConfirmDelete(QuotationViewModel q)
        {
            SelectedQuotation = q;
        }

        void DeleteConfirmed()
        {
            if (SelectedQuotation == null)
                return;

            QuotationService.Delete(SelectedQuotation.Id);

            ToastService.ShowError(
                $"{SelectedQuotation.QuotationNumber} deleted",
                "Deleted");

            Quotations = QuotationService.GetAll();

            ApplyFilters();
        }

        void OpenRowDetails(QuotationViewModel q)
        {
            SelectedQuotation = q;
        }

        string getCustomerName(Guid? customerId)
        {
            var customer = Customers.FirstOrDefault(c => c.Id == customerId);
            return customer?.CustomerName ?? "-";
        }

        private string GetStatusBadge(QuotationStatus status)
        {
            return status switch
            {
                QuotationStatus.New => "bg-secondary-transparent",
                QuotationStatus.SentToClient => "bg-primary-transparent",
                QuotationStatus.Accepted => "bg-success-transparent",
                QuotationStatus.Declined => "bg-danger-transparent",
                QuotationStatus.AnalyzeDecline => "bg-warning-transparent text-dark",
                _ => "bg-secondary-transparent"
            };
        }

        void LoadQuotation()
        {
            searchText = "";
            selectedStatus = null;
            selectedCustomers = null;
            selectedCompany = null;
            Quotations = QuotationService.GetAll();

            ApplyFilters();
        }

        private async Task OnRefreshAsync()
        {
            isLoading = true;
            StateHasChanged();

            await Task.Delay(300);
            QuotationService.ResetToSeed();
            LoadQuotation();

            isLoading = false;
            await JS.InvokeVoidAsync("feather.replace");
            ToastService.ShowInfo("Quotation list refreshed", "Refreshed");
        }
    }
}
