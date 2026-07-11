using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    public class DeliveryChallanService
    {
        private readonly MasterDataService _masterDataService;
        private readonly CustomerService _customerService;
        private readonly ItemService _itemService;

        private readonly List<CompanyModel> _companies = new();
        private readonly List<CustomerViewModel> _customers = new();
        private readonly List<ItemViewModel> _items = new();
        private List<QuotationViewModel> _quotations = new();
        static List<DeliveryChallanViewModel> _challans = new();
        private static List<DeliveryChallanViewModel> _seedChallans = new();

        public DeliveryChallanService( MasterDataService masterDataService, CustomerService customerService, ItemService itemService)
        {
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
            _seedChallans = DeliveryChallanSeedData.GetSeedDeliveryChallans(_companies, _customers, _items);
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
            _challans = CloneList(_seedChallans);
        }
        public List<DeliveryChallanViewModel> GetAll()
        {
            return _challans.OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToList();
        }

        public DeliveryChallanViewModel? Get(Guid id)
        {
            return _challans.FirstOrDefault(x => x.Id == id);
        }


        public void Create(DeliveryChallanViewModel challan)
        {
            challan.Id = Guid.NewGuid();

            challan.ChallanNumber = GenerateChallanNumber();

            challan.CreatedAt = DateTime.UtcNow;

            challan.UpdatedAt = null;

            CalculateTotals(challan);

            _challans.Add(challan);
        }

        public void Update(DeliveryChallanViewModel challan)
        {
            var existing = _challans.FirstOrDefault(x => x.Id == challan.Id);

            if (existing == null)
                return;

            existing.CompanyId = challan.CompanyId;
            existing.CustomerId = challan.CustomerId;

            existing.ChallanNumber = challan.ChallanNumber;

            existing.ChallanDate = challan.ChallanDate;
            existing.ShippingDate = challan.ShippingDate;

            existing.PONumber = challan.PONumber;

            existing.Items = challan.Items;

            existing.TermsAndConditions = challan.TermsAndConditions;
            existing.PrivateNotes = challan.PrivateNotes;

            existing.TaxPercentage = challan.TaxPercentage;

            existing.UpdatedAt = DateTime.UtcNow;

            CalculateTotals(existing);
        }

        public void Delete(Guid? id)
        {
            var c = _challans.FirstOrDefault(x => x.Id == id);
            if (c != null)
                _challans.Remove(c);
        }

        private void CalculateTotals(DeliveryChallanViewModel challan)
        {
            decimal subTotal = 0;
            decimal taxTotal = 0;

            foreach (var line in challan.Items)
            {
                var lineAmount = line.Rate * line.Quantity;

                subTotal += lineAmount;

                taxTotal += lineAmount * line.TaxPercentage / 100;
            }

            challan.SubTotal = subTotal;

            challan.TaxAmount = taxTotal;

            challan.GrandTotal = subTotal + taxTotal;
        }

        public string GenerateChallanNumber()
        {
            var challans = GetAll();

            int nextNumber = challans.Count + 1;

            return $"DC-{DateTime.Now:yyyy}-{nextNumber:0000}";
        }

    }
}
