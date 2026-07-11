using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Sales.DeliveryChallan
{
    public partial class DeliveryChallanDetails : ComponentBase
    {

        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] DeliveryChallanService ChallanService { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] CustomerService CustomerService { get; set; } = default!;

        [Inject] ItemService ItemService { get; set; } = default!;

        [Parameter] public Guid Id { get; set; }

        private bool isInitialized = false;

        private DeliveryChallanViewModel? Challan;
        public CompanyModel? Company = new();
        public CustomerViewModel? Customer = new();
        List<UnitViewModel> Units = new();

        List<ItemViewModel> Items = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadChallan();
            isInitialized = true;
        }

        private async Task LoadChallan()
        {
            Challan = ChallanService.Get(Id);
            Units = ItemService.GetUnitList();
            if (Challan != null)
            {
                Company = MasterDataService.GetCompanyById(Challan.CompanyId.Value);
                Customer = CustomerService.GetById(Challan.CustomerId.Value);

            }
            await Task.CompletedTask;
        }

        private string getItemName(Guid? Id)
        {

            Items = ItemService.GetAll().Where(i => i.CompanyId == Challan.CompanyId).ToList();
            var Item = Items.FirstOrDefault(i => i.Id == Id);

            return Item?.ItemName ?? "-";
        }

        private string getUnitName(Guid? Id)
        {

            var Item = Units.FirstOrDefault(i => i.Id == Id);

            return Item?.UnitName ?? "-";
        }

        async Task PrintDocument()
        {
            await JS.InvokeVoidAsync("printSection", "printSection");
        }

        List<List<string>> GetRows()
        {
            return Challan.Items.Select((x, i) => new List<string>
                {
                (i+1).ToString(),
                getItemName(x.ItemId),
                getUnitName(x.UnitId),
                x.Quantity.ToString(),
                x.Amount.ToString()
                }).ToList();
        }

    }


}
