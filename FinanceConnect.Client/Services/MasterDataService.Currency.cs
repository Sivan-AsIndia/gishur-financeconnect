using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    public partial class MasterDataService
    {
        // Currency Methods
        public List<CurrencyModel> GetAllCurrencies() => _currencies.Where(c => !c.IsDeleted).ToList();
        
        public CurrencyModel? GetCurrencyById(Guid id) => _currencies.FirstOrDefault(c => c.Id == id && !c.IsDeleted);
        
        public CurrencyModel? GetCurrencyByCode(string code) => _currencies.FirstOrDefault(c => c.CurrencyCode == code && !c.IsDeleted);
        
        public void AddCurrency(CurrencyModel currency)
        {
            currency.Id = Guid.NewGuid();
            currency.CreatedAt = DateTime.Now;
            currency.IsActive = true;
            currency.IsDeleted = false;
            _currencies.Add(currency);
        }
        
        public void UpdateCurrency(CurrencyModel currency)
        {
            var existing = _currencies.FirstOrDefault(c => c.Id == currency.Id);
            if (existing != null)
            {
                var index = _currencies.IndexOf(existing);
                currency.UpdatedAt = DateTime.Now;
                _currencies[index] = currency;
            }
        }

        public void ActivateCurrency(Guid id)
        {
            var currency = _currencies.FirstOrDefault(c => c.Id == id);
            if (currency != null)
            {
                currency.IsActive = true;
                currency.UpdatedAt = DateTime.Now;
            }
        }

        public void DeactivateCurrency(Guid id)
        {
            var currency = _currencies.FirstOrDefault(c => c.Id == id);
            if (currency != null)
            {
                currency.IsActive = false;
                currency.UpdatedAt = DateTime.Now;
            }
        }
        
        public void DeleteCurrency(Guid id)
        {
            var currency = _currencies.FirstOrDefault(c => c.Id == id);
            if (currency != null)
            {
                currency.IsActive = false;
                currency.IsDeleted = true;
                currency.DeletedAt = DateTime.Now;
            }
        }
    }
}
