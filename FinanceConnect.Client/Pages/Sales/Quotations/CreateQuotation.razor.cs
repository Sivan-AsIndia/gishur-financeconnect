using FinanceConnect.Client.Services;
using FinanceConnect.Client.Shared;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using static FinanceConnect.Client.Data.MasterDataIds;

namespace FinanceConnect.Client.Pages.Sales.Quotations
{
    public partial class CreateQuotation
    {

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
        }
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject] AuthService AuthService { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] QuotationService QuotationService { get; set; } = default!;
        [Inject] CustomerService CustomerService { get; set; } = default!;
        [Inject] ItemService ItemService { get; set; } = default!;

        [Parameter] public Guid? Id { get; set; }

        List<QuotationViewModel> Quotations = new();
        public List<CompanyModel> Companies = new();
        List<CustomerViewModel> Customers = new();
        List<UnitViewModel> Units = new();
        List<TaxViewModel> Taxes = new();
        List<ItemViewModel> Items = new();
        Guid? selectedCompany = null;
        private bool IsEdit => Id.HasValue;
        private bool isInitialized = false;
        private RichTextEditor? notesEditor;
        string OwnerName = "";

        QuotationViewModel Model = new();

        bool showValidationErrors = false;
        bool ApplyGlobalDiscount = false;
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
            Companies = MasterDataService.GetAllCompanies().Where(c => c.Status == "Active").ToList();
            Quotations = QuotationService.GetAll();
            OwnerName = AuthService.CurrentUser?.UserName ?? "System";
            Units = ItemService.GetUnitList();
            Taxes = ItemService.GetTaxList();

            if (Id.HasValue && Id != Guid.Empty)
            {
                Model = QuotationService.GetById(Id.Value);

                if(Model == null)
                {
                    Nav.NavigateTo("/quotations");
                    return;
                }
                else
                {
                    selectedCompany = Model.CompanyId;
                    onCompanyChange();
                }
            }
            else
            {
                // Create Mode
                Model = new QuotationViewModel
                {
                    Id = Guid.NewGuid(),
                    QuotationDate = DateTime.Today,
                    QuotationNumber = QuotationService.GenerateQuotationNumber(),
                    CreatedBy = AuthService.CurrentUser?.UserName ?? "System",
                };

                Model.Items.Add(new QuotationLineItemViewModel
                {
                });

                
            }
            isInitialized = true;
        }

        void onCompanyChange()
        {
            if (!selectedCompany.HasValue)
                return;

            Model.CompanyId = selectedCompany;

            Items.Clear();
            Customers.Clear();

            Items = ItemService.GetAll().Where(i=> i.CompanyId == selectedCompany).ToList();

            Customers = CustomerService.GetAll().Where(c => c.CompanyId == selectedCompany).ToList();

        }

        void OnItemChanged(QuotationLineItemViewModel line)
        {
            var item = Items.FirstOrDefault(i => i.Id == line.ItemId);

            if (item == null)
                return;

            line.UnitId = item.UnitId;

            line.Rate = item.DefaultRate;
            line.Quantity = 1;
            line.TaxPercentage = item.TaxPercentage;
            Recalculate();

        }

        void AddLine()
        {
            var lastLine = Model.Items.LastOrDefault();

            if (lastLine != null)
            {
                if (lastLine.ItemId == null ||
                    lastLine.UnitId == null ||
                    lastLine.Rate <= 0 ||
                    lastLine.Quantity <= 0)
                {
                    ToastService.ShowWarning("Please complete the previous line before adding a new one.");
                    return;
                }
            }

            Model.Items.Add(new QuotationLineItemViewModel());
        }

        void Delete(QuotationLineItemViewModel line)
        {
            Model.Items.Remove(line);
            Recalculate();
        }

        void Duplicate(QuotationLineItemViewModel line)
        {
            Model.Items.Add(new QuotationLineItemViewModel
            {
                ItemId = line.ItemId,
                ItemName = line.ItemName,
                Unit = line.Unit,
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
            }

            Model.SubTotal = subTotal;

            Model.TaxAmount = taxTotal;

            Model.DiscountAmount = Model.SubTotal * Model.Discount / 100;

            Model.GrandTotal =
                Model.SubTotal + Model.TaxAmount - Model.DiscountAmount;
        }

        void OnDiscountToggle()
        {
            if (!Model.ApplyDiscount)
            {
                Model.Discount = 0;
                Model.DiscountAmount = 0;
            }

            Recalculate();
        }

        void OnDiscountChanged(ChangeEventArgs e)
        {
            Recalculate();
        }


        void SaveQuotation()
        {
            showValidationErrors = true;

            if (ShowFieldError("CompanyId") ||
                ShowFieldError("CustomerId") ||
                ShowFieldError("QuotationDate") ||
                ShowFieldError("ExpiryDate") ||
                ShowFieldError("Subject"))
            {
                return;
            }

            // Expiry validation
            if (Model.ExpiryDate < Model.QuotationDate)
            {
                ToastService.ShowWarning("Expiry date cannot be earlier than quotation date.");
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
                QuotationService.Update(Model);

                ToastService.ShowSuccess("Quotation saved");
            }
            else
            {
                QuotationService.Create(Model);

                ToastService.ShowSuccess("Quotation saved");
            }



            Nav.NavigateTo("/quotations");
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

        bool ShowFieldError(string field)
        {
            if (!showValidationErrors)
                return false;

            return field switch
            {
                "CompanyId" => Model.CompanyId == null || Model.CompanyId == Guid.Empty,
                "CustomerId" => Model.CustomerId == null || Model.CustomerId == Guid.Empty,
                "QuotationDate" => !Model.QuotationDate.HasValue,
                "ExpiryDate" => !Model.ExpiryDate.HasValue,
                "Subject" => string.IsNullOrWhiteSpace(Model.Subject),
                _ => false
            };
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
