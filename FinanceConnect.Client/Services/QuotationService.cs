using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    public class QuotationService
    {
        private readonly AuthService _authService;
        private readonly MasterDataService _masterDataService;
        private readonly CustomerService _customerService;
        private readonly ItemService _itemService;

        private readonly List<CompanyModel> _companies = new();
        private readonly List<CustomerViewModel> _customers = new();
        private readonly List<ItemViewModel> _items = new();
        private  List<QuotationViewModel> _quotations = new();
        private static  List<QuotationViewModel> _seedQuotations = new();

        public QuotationService(AuthService authService, MasterDataService masterDataService, CustomerService customerService, ItemService itemService)
        {
            _authService = authService;
            _masterDataService = masterDataService;
            _customerService = customerService;
            _itemService = itemService;
            _companies = _masterDataService
                .GetAllCompanies()
                .Where(c => c.Status == "Active")
                .ToList();
            _customers = _customerService.GetAll().Where(c => c.CustomerStatus == "Active")
                .ToList();
            _items = _itemService.GetAll().Where(i => i.Status == ItemStatus.Active).ToList();
            _seedQuotations = QuotationSeedData.GetSeedQuotations(_companies, _customers , _items, _authService.CurrentUser?.UserName);
            ResetToSeed();
        }


        private static List<T> CloneList<T>(IEnumerable<T> source) where T : class
        {
            var json = System.Text.Json.JsonSerializer.Serialize(source);
            return System.Text.Json.JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }

        /// <summary>Reset vendors to seed data</summary>
        public void ResetToSeed()
        {
            _quotations = CloneList(_seedQuotations);
        }

        public List<QuotationViewModel> GetAll()
        {
            return _quotations
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToList();
        }

        public QuotationViewModel? GetById(Guid id)
        {
            return _quotations.FirstOrDefault(x => x.Id == id);
        }

        public void Create(QuotationViewModel quotation)
        {
            quotation.Id = Guid.NewGuid();

            quotation.QuotationNumber = GenerateQuotationNumber();

            quotation.CreatedAt = DateTime.UtcNow;

            quotation.UpdatedAt = null;

            CalculateTotals(quotation);

            _quotations.Add(quotation);
        }

        public void Update(QuotationViewModel quotation)
        {
            var existing = _quotations.FirstOrDefault(x => x.Id == quotation.Id);

            if (existing == null)
                return;

            existing.Subject = quotation.Subject;
            existing.CustomerId = quotation.CustomerId;
            existing.Status = quotation.Status;
            existing.QuotationDate = quotation.QuotationDate;
            existing.QuotationNumber = quotation.QuotationNumber;
            existing.ExpiryDate = quotation.ExpiryDate;
            existing.OwnerId = quotation.OwnerId;
            existing.Items = quotation.Items;

            existing.TermsAndConditions = quotation.TermsAndConditions;
            existing.Description = quotation.Description;

            existing.TaxPercentage = quotation.TaxPercentage;
            existing.Discount = quotation.Discount;

            existing.UpdatedAt = DateTime.UtcNow;

            CalculateTotals(existing);
        }

        public void Delete(Guid? id)
        {
            var q = _quotations.FirstOrDefault(x => x.Id == id);

            if (q != null)
                _quotations.Remove(q);
        }

        public void ChangeStatus(Guid id, QuotationStatus status)
        {
            var q = _quotations.FirstOrDefault(x => x.Id == id);

            if (q != null)
                q.Status = status;
        }
        private void CalculateTotals(QuotationViewModel quotation)
        {
            if (quotation.Items == null || !quotation.Items.Any())
                return;

            decimal subTotal = 0;
            decimal taxTotal = 0;

            foreach (var line in quotation.Items)
            {
                var lineAmount = line.Rate * line.Quantity;

                subTotal += lineAmount;
                taxTotal += lineAmount * line.TaxPercentage / 100;
            }

            quotation.SubTotal = subTotal;

            quotation.TaxAmount = taxTotal;

            quotation.DiscountAmount =
                quotation.SubTotal * quotation.Discount / 100;

            quotation.GrandTotal =
                quotation.SubTotal + quotation.TaxAmount - quotation.DiscountAmount;
        }

        public string GenerateQuotationNumber()
        {
            var next = _quotations.Count + 1;

            return $"QT-{DateTime.Now:yyyy}-{next:0000}";
        }

    }
}