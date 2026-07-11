using FinanceConnect.Client.Services;
using FinanceConnect.Client.Shared;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;

namespace FinanceConnect.Client.Pages.Sales.DeliveryChallan
{
    public partial class CreateDeliveryChallan
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] CustomerService CustomerService { get; set; } = default!;
        [Inject] ItemService ItemService { get; set; } = default!;
        [Inject] DeliveryChallanService DeliveryChallanService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;

        [Parameter] public Guid? Id { get; set; }

        private bool IsEdit => Id.HasValue;

        RichTextEditor? _notesEditor;
        RichTextEditor? _termsEditor;

        DeliveryChallanViewModel Model = new();

        List<ItemViewModel> Items = new();
        List<CustomerViewModel> Customers = new();
        List<UnitViewModel> Units = new();
        List<TaxViewModel> Taxes = new();

        public List<CompanyModel> Companies = new();

        Guid? selectedCompany;

        bool isInitialized = false;

        bool showValidationErrors = false;
        bool showTransportValidationErrors = false;

        bool ShowTransportDetails = false;


        Guid? SelectedCompany
        {
            get => selectedCompany;
            set
            {
                selectedCompany = value;
                onCompanyChange();
            }
        }


        protected override async Task OnInitializedAsync()
        {

            LoadData();

        }


        void LoadData()
        {

            Companies = MasterDataService.GetAllCompanies();

            Units = ItemService.GetUnitList();
            Taxes = ItemService.GetTaxList();

            if (Id.HasValue)
            {
                Model = DeliveryChallanService.Get(Id.Value);
                selectedCompany = Model.CompanyId;
                onCompanyChange();
            }
            else
            {

                Model = new DeliveryChallanViewModel
                {
                    Id = Guid.NewGuid(),
                    ChallanDate = DateTime.Today,
                    ChallanNumber = DeliveryChallanService.GenerateChallanNumber()
                };

                Model.Items.Add(new DeliveryChallanLineItemViewModel());

            }

            isInitialized = true;

        }


        void onCompanyChange()
        {

            if (!selectedCompany.HasValue)
                return;

            Model.CompanyId = selectedCompany;

            Items = ItemService.GetAll()
            .Where(x => x.CompanyId == selectedCompany)
            .ToList();

            Customers = CustomerService.GetAll()
            .Where(x => x.CompanyId == selectedCompany)
            .ToList();

        }


        void OnItemChanged(DeliveryChallanLineItemViewModel line)
        {

            var item = Items.FirstOrDefault(x => x.Id == line.ItemId);

            if (item == null)
                return;

            line.UnitId = item.UnitId;
            line.Rate = item.DefaultRate;
            line.Quantity = 1;
            line.TaxPercentage = item.TaxPercentage;

            Recalculate();

        }

        void OnTransportToggle()
        {
            if (Model.ShowTransportDetails && Model.TransportDetails == null)
            {
                Model.TransportDetails = new TransportDetailsViewModel();
            }

            if (!Model.ShowTransportDetails)
            {
                Model.TransportDetails = null;
            }
        }


        void AddLine()
        {

            Model.Items.Add(new DeliveryChallanLineItemViewModel());

        }


        void Delete(DeliveryChallanLineItemViewModel line)
        {

            Model.Items.Remove(line);

            Recalculate();

        }


        void Duplicate(DeliveryChallanLineItemViewModel line)
        {

            Model.Items.Add(new DeliveryChallanLineItemViewModel
            {
                ItemId = line.ItemId,
                UnitId = line.UnitId,
                Rate = line.Rate,
                Quantity = line.Quantity
            });

            Recalculate();

        }


        void Recalculate()
        {

            decimal subTotal = 0;
            decimal taxTotal = 0;

            foreach (var line in Model.Items)
            {

                var lineAmount = line.Rate * line.Quantity;

                subTotal += lineAmount;

                taxTotal += lineAmount * line.TaxPercentage / 100;

                //line.TotalAmount = lineAmount + (lineAmount * line.TaxPercentage / 100);

            }

            Model.SubTotal = subTotal;
            Model.TaxAmount = taxTotal;
            Model.GrandTotal = subTotal + taxTotal;

        }

        bool ShowFieldError(string field)
        {
            if (!showValidationErrors)
                return false;

            return field switch
            {
                "CompanyId" => Model.CompanyId == null || Model.CompanyId == Guid.Empty,
                "CustomerId" => Model.CustomerId == null || Model.CustomerId == Guid.Empty,
                "ChallanDate" => !Model.ChallanDate.HasValue,
                "ShippingDate" => !Model.ShippingDate.HasValue,
                //"Subject" => string.IsNullOrWhiteSpace(Model.Subject),
                _ => false
            };
        }
        bool ShowTransportFieldError(string field)
        {
            if (!showTransportValidationErrors)
                return false;

            return field switch
            {
                "TransporterName" => string.IsNullOrWhiteSpace(Model.TransportDetails.TransporterName),
                "VehicleNumber" => string.IsNullOrWhiteSpace(Model.TransportDetails.VehicleNumber),
                "LRNumber" => string.IsNullOrWhiteSpace(Model.TransportDetails.LRNumber),
                "EWayBillNumber" => string.IsNullOrWhiteSpace(Model.TransportDetails.EWayBillNumber),
                "LRDate" => !Model.TransportDetails.LRDate.HasValue,

                _ => false
            };
        }


        async Task SaveChallan()
        {
            showValidationErrors = true;


            if (_notesEditor != null)
            {
                Model.PrivateNotes = await _notesEditor.GetHtmlAsync();
            }
            if (_termsEditor != null)
            {
                Model.TermsAndConditions = await _termsEditor.GetHtmlAsync();
            }

            if (ShowFieldError("CompanyId") ||
                ShowFieldError("CustomerId") ||
                ShowFieldError("QuotationDate") ||
                ShowFieldError("ExpiryDate") ||
                ShowFieldError("Subject"))
            {
                return;
            }

            if(Model.ShowTransportDetails)
            {
                showTransportValidationErrors = true;

                if (ShowTransportFieldError("TransporterName") ||
                    ShowTransportFieldError("VehicleNumber") ||
                    ShowTransportFieldError("LRNumber") ||
                    ShowTransportFieldError("EWayBillNumber") ||
                    ShowTransportFieldError("LRDate"))
                {
                    return;
                }
            }

                // Expiry validation
                if (Model.ShippingDate < Model.ChallanDate)
            {
                ToastService.ShowWarning("Shipping date cannot be earlier than quotation date.");
                return;
            }

            // Line item validation
            if (HasInvalidLineItems())
            {
                ToastService.ShowWarning("Please complete all line items before saving.");
                return;
            }

            if (IsEdit)
            {
                DeliveryChallanService.Update(Model);

                ToastService.ShowSuccess("Delivery Challan saved");
            }
            else
            {
                DeliveryChallanService.Create(Model);

                ToastService.ShowSuccess("Delivery Challan saved");
            }

            Nav.NavigateTo("/deliverychallans");

        }

        bool HasInvalidLineItems()
        {
            if (Model.Items == null || !Model.Items.Any())
                return true;

            return Model.Items.Any(l =>
                l.ItemId == null ||
                l.UnitId == null ||
                l.Rate <= 0 ||
                l.Quantity <= 0
            );
        }
    }
}