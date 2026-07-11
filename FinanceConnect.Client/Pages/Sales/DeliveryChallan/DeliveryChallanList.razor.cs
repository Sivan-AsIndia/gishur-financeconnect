using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Sales.DeliveryChallan
{
    public partial class DeliveryChallanList
    {

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
            VisibleColumnCount =
            await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        [Inject] DeliveryChallanService DeliveryChallanService { get; set; } = default!;
        [Inject] CustomerService CustomerService { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;

        bool isInitialized = false;
        bool isLoading = false;

        List<DeliveryChallanViewModel> Challans = new();
        List<DeliveryChallanViewModel> FilteredChallans = new();

        public List<CompanyModel> Companies = new();
        List<CustomerViewModel> Customers = new();

        string searchText = "";

        Guid? selectedCompany;
        Guid? selectedCustomer;
        int PageSize = 10;
        int CurrentPage = 1;
        int PageWindowSize = 2;
        private int VisibleColumnCount;

        DeliveryChallanViewModel? SelectedChallan;

        Guid? SelectedCompany
        {
            get => selectedCompany;
            set
            {
                selectedCompany = value;
                //onCompanyChange();
                ApplyFilters();
            }
        }

        Guid? SelectedCustomer
        {
            get => selectedCustomer;
            set
            {
                selectedCustomer = value;
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
        }
        void LoadData()
        {

            Companies = MasterDataService.GetAllCompanies();

            Customers = CustomerService.GetAll();

            Challans = DeliveryChallanService.GetAll();

            FilteredChallans = Challans;

            isInitialized = true;

        }


        void OnSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString() ?? "";
            ApplyFilters();
        }


        void ApplyFilters()
        {

            IEnumerable<DeliveryChallanViewModel> query = Challans;

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(x =>
                x.ChallanNumber.Contains(searchText, StringComparison.OrdinalIgnoreCase));
            }

            if (selectedCompany.HasValue)
            {
                query = query.Where(x => x.CompanyId == selectedCompany);
            }

            if (selectedCustomer.HasValue)
            {
                query = query.Where(x => x.CustomerId == selectedCustomer);
            }

            FilteredChallans = query.ToList();

        }


        List<DeliveryChallanViewModel> PagedChallans =>
        FilteredChallans
        .Skip((CurrentPage - 1) * PageSize)
        .Take(PageSize)
        .ToList();


        int TotalPages =>
        (int)Math.Ceiling((double)FilteredChallans.Count / PageSize);


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

        void GoToPage(int p)
        {
            CurrentPage = p;
        }


        void OnPageSizeChange(ChangeEventArgs e)
        {
            PageSize = int.Parse(e.Value!.ToString()!);
            CurrentPage = 1;
        }


        void ViewChallan(DeliveryChallanViewModel c)
        {
            Nav.NavigateTo($"/deliverychallans/{c.Id}/view");
        }

        private void OpenRowDetails(DeliveryChallanViewModel c)
        {
            SelectedChallan = c;
        }


        void ConfirmDelete(DeliveryChallanViewModel c)
        {
            SelectedChallan = c;
        }


        void DeleteConfirmed()
        {

            if (SelectedChallan == null) return;

            DeliveryChallanService.Delete(SelectedChallan.Id);

            ToastService.ShowError("Delivery challan deleted", "Deleted");

            Challans = DeliveryChallanService.GetAll();

            ApplyFilters();

        }


        string getCustomerName(Guid? id)
        {
            return Customers.FirstOrDefault(x => x.Id == id)?.CustomerName ?? "-";
        }

            void LoadDeliveryChallan()
        {
            searchText = "";
            selectedCustomer = null;
            selectedCompany = null;
            Companies = MasterDataService.GetAllCompanies();

            Customers = CustomerService.GetAll();

            Challans = DeliveryChallanService.GetAll();

            FilteredChallans = Challans;
        }

        private async Task OnRefreshAsync()
        {
            isLoading = true;
            StateHasChanged();

            await Task.Delay(300);
            DeliveryChallanService.ResetToSeed();
            LoadDeliveryChallan();

            isLoading = false;
            await JS.InvokeVoidAsync("feather.replace");
            ToastService.ShowInfo("Delivery Challan list refreshed", "Refreshed");
        }

    }
}
