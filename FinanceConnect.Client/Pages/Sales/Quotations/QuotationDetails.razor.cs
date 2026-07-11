using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;

namespace FinanceConnect.Client.Pages.Sales.Quotations
{
    public partial class QuotationDetails
    {

        [Inject] QuotationService QuotationService { get; set; } = default!;
        [Inject] CustomerService CustomerService { get; set; } = default!;
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] ItemService ItemService { get; set; } = default!;

        [Parameter] public Guid Id { get; set; }

        private bool isInitialized;
        private QuotationViewModel? Quotation;
        private string CustomerName = "";
        List<ItemViewModel> Items = new();
        List<UnitViewModel> Units = new();

        protected override void OnInitialized()
        {
            Quotation = QuotationService.GetById(Id);
            Units = ItemService.GetUnitList();
            if (Quotation != null && Quotation.CustomerId != null)
            {
                CustomerName = CustomerService
                    .GetById(Quotation.CustomerId.Value)?.CustomerName ?? "";
            }

            isInitialized = true;
        }

        private string getItemName(Guid? Id)
        {

            Items = ItemService.GetAll().Where(i => i.CompanyId == Quotation.CompanyId).ToList();
            var Item = Items.FirstOrDefault(i => i.Id == Id);

            return Item?.ItemName ?? "-";
        }
        private string getUnitName(Guid? Id)
        {

            var Item = Units.FirstOrDefault(i => i.Id == Id);

            return Item?.UnitName ?? "-";
        }
        private string GetStatusBadge(QuotationStatus status)
        {
            return status switch
            {
                QuotationStatus.New => "bg-secondary-transparent text-secondary",
                QuotationStatus.SentToClient => "bg-primary-transparent text-primary",
                QuotationStatus.Accepted => "bg-success-transparent text-success",
                QuotationStatus.Declined => "bg-danger-transparent text-danger",
                QuotationStatus.AnalyzeDecline => "bg-warning-transparent text-dark",
                _ => "bg-secondary-transparent text-secondary"
            };
        }
    }
}
