using FinanceConnect.Client.ViewModels;
using static FinanceConnect.Client.Data.MasterDataIds;

namespace FinanceConnect.Client.Services
{
    public class ItemService
    {
        private readonly MasterDataService _masterDataService;

        private List<ItemViewModel> _items = new();
        private static List<ItemViewModel> _seedItems = new();

        public ItemService(MasterDataService masterDataService)
        {
            _masterDataService = masterDataService;

            var companies = _masterDataService
                .GetAllCompanies()
                .Where(x => x.Status == "Active")
                .ToList();

            var units = GetUnitList();

            var taxes = GetTaxList();

            _seedItems = SeedItems(companies, units , taxes);

            ResetToSeed();
        }

        private static List<T> CloneList<T>(IEnumerable<T> source) where T : class
        {
            var json = System.Text.Json.JsonSerializer.Serialize(source);
            return System.Text.Json.JsonSerializer.Deserialize<List<T>>(json) ?? new();
        }

        public void ResetToSeed()
        {
            _items = CloneList(_seedItems);
        }

        public List<ItemViewModel> GetAll()
        {
            return _items
                .OrderBy(x => x.ItemName)
                .ToList();
        }

        public List<ItemViewModel> GetByCompany(Guid companyId)
        {
            return _items
                .Where(x => x.CompanyId == companyId && x.Status == ItemStatus.Active)
                .OrderBy(x => x.ItemName)
                .ToList();
        }

        public ItemViewModel? GetById(Guid id)
        {
            return _items.FirstOrDefault(x => x.Id == id);
        }

        public void Create(ItemViewModel model)
        {
            model.Id = Guid.NewGuid();
            model.CreatedAt = DateTime.UtcNow;
            model.CreatedBy = "User";

            if (string.IsNullOrWhiteSpace(model.ItemCode))
                model.ItemCode = GenerateItemCode(model.CompanyId);

            _items.Add(model);
        }

        public void Update(ItemViewModel model)
        {
            var existing = _items.FirstOrDefault(x => x.Id == model.Id);
            if (existing == null)
                return;

            existing.CompanyId = model.CompanyId;
            existing.CompanyName = model.CompanyName;
            existing.ItemName = model.ItemName;
            existing.ItemCode = model.ItemCode;
            existing.HSNCode = model.HSNCode;
            existing.ItemType = model.ItemType;
            existing.UnitId = model.UnitId;
            existing.DefaultRate = model.DefaultRate;
            existing.CostPrice = model.CostPrice;
            existing.TaxPercentage = model.TaxPercentage;
            existing.Status = model.Status;
            existing.Description = model.Description;

            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = "User";
        }

        public void Delete(Guid id)
        {
            var item = _items.FirstOrDefault(x => x.Id == id);

            if (item != null)
                _items.Remove(item);
        }

        private string GenerateItemCode(Guid companyId)
        {
            var count = _items.Count(x => x.CompanyId == companyId) + 1;

            return $"ITM-{count:0000}";
        }

        private List<ItemViewModel> SeedItems(
            List<CompanyModel> companies,
            List<UnitViewModel> units,
            List<TaxViewModel> taxes)
        {
            var items = new List<ItemViewModel>();

            var nosUnit = units.FirstOrDefault(x => x.Symbol == "Nos");
            var hourUnit = units.FirstOrDefault(x => x.Symbol == "Hr" || x.UnitName == "Hour");

            var tax18 = taxes.FirstOrDefault(t => t.Percentage == 18m)?.Percentage ?? 0;

            foreach (var company in companies)
            {
                // PRODUCT 1
                items.Add(new ItemViewModel
                {
                    Id = Guid.NewGuid(),
                    CompanyId = company.Id,
                    CompanyName = company.LegalName,
                    ItemName = "Laptop",
                    ItemCode = "PRD-001",
                    ItemType = ItemType.Product,
                    UnitId = nosUnit?.Id,
                    UnitName = nosUnit?.UnitName,
                    DefaultRate = 50000,
                    CostPrice = 42000,
                    TaxPercentage = tax18,
                    Status = ItemStatus.Active,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "Seed"
                });

                // PRODUCT 2
                items.Add(new ItemViewModel
                {
                    Id = Guid.NewGuid(),
                    CompanyId = company.Id,
                    CompanyName = company.LegalName,
                    ItemName = "Office Chair",
                    ItemCode = "PRD-002",
                    ItemType = ItemType.Product,
                    UnitId = nosUnit?.Id,
                    UnitName = nosUnit?.UnitName,
                    DefaultRate = 4500,
                    CostPrice = 3000,
                    TaxPercentage = tax18,
                    Status = ItemStatus.Active,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "Seed"
                });

                // SERVICE 1
                items.Add(new ItemViewModel
                {
                    Id = Guid.NewGuid(),
                    CompanyId = company.Id,
                    CompanyName = company.LegalName,
                    ItemName = "Installation Service",
                    ItemCode = "SRV-001",
                    ItemType = ItemType.Service,
                    UnitId = hourUnit?.Id,
                    UnitName = hourUnit?.UnitName,
                    DefaultRate = 1500,
                    TaxPercentage = tax18,
                    Status = ItemStatus.Active,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "Seed"
                });

                // SERVICE 2
                items.Add(new ItemViewModel
                {
                    Id = Guid.NewGuid(),
                    CompanyId = company.Id,
                    CompanyName = company.LegalName,
                    ItemName = "Consultation Service",
                    ItemCode = "SRV-002",
                    ItemType = ItemType.Service,
                    UnitId = hourUnit?.Id,
                    UnitName = hourUnit?.UnitName,
                    DefaultRate = 2500,
                    TaxPercentage = tax18,
                    Status = ItemStatus.Active,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "Seed"
                });
            }

            return items;
        }

        public List<UnitViewModel> GetUnitList()
        {
            return new List<UnitViewModel>
            {
                new UnitViewModel { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), UnitName = "Numbers", Symbol = "Nos" },
                new UnitViewModel { Id = Guid.Parse("11111111-1111-1111-1111-111111111112"), UnitName = "Kilogram", Symbol = "Kg" },
                new UnitViewModel { Id = Guid.Parse("11111111-1111-1111-1111-111111111113"), UnitName = "Gram", Symbol = "g" },
                new UnitViewModel { Id = Guid.Parse("11111111-1111-1111-1111-111111111114"), UnitName = "Litre", Symbol = "L" },
                new UnitViewModel { Id = Guid.Parse("11111111-1111-1111-1111-111111111115"), UnitName = "Meter", Symbol = "m" },
                new UnitViewModel { Id = Guid.Parse("11111111-1111-1111-1111-111111111116"), UnitName = "Square Feet", Symbol = "sqft" },
                new UnitViewModel { Id = Guid.Parse("11111111-1111-1111-1111-111111111117"), UnitName = "Box", Symbol = "Box" },
                new UnitViewModel { Id = Guid.Parse("11111111-1111-1111-1111-111111111118"), UnitName = "Pack", Symbol = "Pack" },
                new UnitViewModel { Id = Guid.Parse("11111111-1111-1111-1111-111111111119"), UnitName = "Hour", Symbol = "Hr" },
                new UnitViewModel { Id = Guid.Parse("11111111-1111-1111-1111-111111111110"), UnitName = "Day", Symbol = "Day" },
                new UnitViewModel { Id = Guid.Parse("11111111-1111-1111-1111-111111111122"), UnitName = "Service", Symbol = "Svc" }
            };
        }


        public List<TaxViewModel> GetTaxList()
        {
            return new List<TaxViewModel>
            {
                new TaxViewModel { Percentage = 0m, DisplayName = "0%" },
                new TaxViewModel { Percentage = 0.1m, DisplayName = "0.1%" },
                new TaxViewModel { Percentage = 0.25m, DisplayName = "0.25%" },
                new TaxViewModel { Percentage = 1.5m, DisplayName = "1.5%" },
                new TaxViewModel { Percentage = 3m, DisplayName = "3%" },
                new TaxViewModel { Percentage = 5m, DisplayName = "5%" },
                new TaxViewModel { Percentage = 12m, DisplayName = "12%" },
                new TaxViewModel { Percentage = 18m, DisplayName = "18%" },
                new TaxViewModel { Percentage = 28m, DisplayName = "28%" },
                new TaxViewModel { Percentage = 40m, DisplayName = "40%" }
            };
        }
    }
}
