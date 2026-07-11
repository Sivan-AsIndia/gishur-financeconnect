using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
namespace FinanceConnect.Client.Shared
{
    public partial class DocumentPrintTemplate
    {
        [Parameter] public string Title { get; set; } = "";
        [Parameter] public string DocumentNumber { get; set; } = "";
        [Parameter] public string CompanyName { get; set; } = "";
        [Parameter] public CompanyModel? Company { get; set; }
        [Parameter] public string CustomerName { get; set; } = "";
        [Parameter] public CustomerViewModel? Customer { get; set; }
        [Parameter] public string ShippingAddress { get; set; } = "";

        [Parameter] public string Date { get; set; } = "";

        [Parameter] public string? LogoUrl { get; set; }

        [Parameter] public string? ExtraMeta1Label { get; set; }
        [Parameter] public string? ExtraMeta1Value { get; set; }

        [Parameter] public string? ExtraMeta2Label { get; set; }
        [Parameter] public string? ExtraMeta2Value { get; set; }
        [Parameter] public DeliveryChallanViewModel? Challan { get; set; }

        [Parameter] public List<string> Columns { get; set; } = new();

        [Parameter] public List<List<string>> Rows { get; set; } = new();

        [Inject] MasterDataService MasterDataService { get; set; } = default!;

        List<CurrencyModel> Currencies = new();

        protected override async Task OnInitializedAsync()
        {
            Currencies = MasterDataService.GetAllCurrencies();
        }

        string getCurrency(Guid? Id)
        {
            var currency = Currencies.FirstOrDefault(c => c.Id == Id);
            return currency?.Symbol??"-";
        }

    }
}
