using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    public partial class MasterDataService
    {
        // ExchangeRate Methods
        public List<ExchangeRateModel> GetAllExchangeRates() => _exchangeRates.Where(e => !e.IsDeleted).ToList();

        public ExchangeRateModel? GetExchangeRateById(Guid id) => _exchangeRates.FirstOrDefault(e => e.Id == id && !e.IsDeleted);

        public List<ExchangeRateModel> GetExchangeRatesByCompany(Guid? companyId) =>
            _exchangeRates.Where(e => e.CompanyId == companyId && !e.IsDeleted).ToList();

        public List<ExchangeRateModel> GetExchangeRatesByCurrencyPair(Guid baseCurrencyId, Guid quoteCurrencyId) =>
            _exchangeRates.Where(e => e.BaseCurrencyId == baseCurrencyId && e.QuoteCurrencyId == quoteCurrencyId && !e.IsDeleted).ToList();

        public void AddExchangeRate(ExchangeRateModel exchangeRate)
        {
            exchangeRate.Id = Guid.NewGuid();
            exchangeRate.CreatedAt = DateTime.Now;
            exchangeRate.IsDeleted = false;
            exchangeRate.VersionNo = 1;

            // Populate display names
            var baseCurrency = GetCurrencyById(exchangeRate.BaseCurrencyId);
            var quoteCurrency = GetCurrencyById(exchangeRate.QuoteCurrencyId);
            if (baseCurrency != null)
            {
                exchangeRate.BaseCurrencyCode = baseCurrency.CurrencyCode;
                exchangeRate.BaseCurrencyName = baseCurrency.CurrencyName;
            }
            if (quoteCurrency != null)
            {
                exchangeRate.QuoteCurrencyCode = quoteCurrency.CurrencyCode;
                exchangeRate.QuoteCurrencyName = quoteCurrency.CurrencyName;
            }
            if (exchangeRate.CompanyId.HasValue)
            {
                var company = GetCompanyById(exchangeRate.CompanyId.Value);
                if (company != null)
                {
                    exchangeRate.CompanyCode = company.CompanyCode;
                    exchangeRate.CompanyName = company.LegalName;
                }
            }

            _exchangeRates.Add(exchangeRate);
        }

        public void UpdateExchangeRate(ExchangeRateModel exchangeRate)
        {
            var existing = _exchangeRates.FirstOrDefault(e => e.Id == exchangeRate.Id);
            if (existing != null)
            {
                // Per spec: Cannot edit rate once used in posted transactions
                // For demo, we just update
                var index = _exchangeRates.IndexOf(existing);
                exchangeRate.UpdatedAt = DateTime.Now;

                // Refresh display names
                var baseCurrency = GetCurrencyById(exchangeRate.BaseCurrencyId);
                var quoteCurrency = GetCurrencyById(exchangeRate.QuoteCurrencyId);
                if (baseCurrency != null)
                {
                    exchangeRate.BaseCurrencyCode = baseCurrency.CurrencyCode;
                    exchangeRate.BaseCurrencyName = baseCurrency.CurrencyName;
                }
                if (quoteCurrency != null)
                {
                    exchangeRate.QuoteCurrencyCode = quoteCurrency.CurrencyCode;
                    exchangeRate.QuoteCurrencyName = quoteCurrency.CurrencyName;
                }
                if (exchangeRate.CompanyId.HasValue)
                {
                    var company = GetCompanyById(exchangeRate.CompanyId.Value);
                    if (company != null)
                    {
                        exchangeRate.CompanyCode = company.CompanyCode;
                        exchangeRate.CompanyName = company.LegalName;
                    }
                }
                else
                {
                    exchangeRate.CompanyCode = null;
                    exchangeRate.CompanyName = null;
                }

                _exchangeRates[index] = exchangeRate;
            }
        }

        public void ActivateExchangeRate(Guid id)
        {
            var exchangeRate = _exchangeRates.FirstOrDefault(e => e.Id == id);
            if (exchangeRate != null)
            {
                exchangeRate.Status = "Active";
                exchangeRate.UpdatedAt = DateTime.Now;
            }
        }

        public void DeactivateExchangeRate(Guid id)
        {
            var exchangeRate = _exchangeRates.FirstOrDefault(e => e.Id == id);
            if (exchangeRate != null)
            {
                exchangeRate.Status = "Inactive";
                exchangeRate.UpdatedAt = DateTime.Now;
            }
        }

        public void DeleteExchangeRate(Guid id)
        {
            var exchangeRate = _exchangeRates.FirstOrDefault(e => e.Id == id);
            if (exchangeRate != null)
            {
                // Per spec: Cannot delete rates used in posting
                // For demo, we just soft delete
                exchangeRate.IsDeleted = true;
                exchangeRate.DeletedAt = DateTime.Now;
            }
        }

        public bool CanDeactivateExchangeRate(Guid id)
        {
            // In a real app, check if rate was used in posted transactions
            return true;
        }

        public bool CanDeleteExchangeRate(Guid id)
        {
            // In a real app, check if rate was used in any posting
            var rate = _exchangeRates.FirstOrDefault(e => e.Id == id);
            // Only Draft rates can be deleted per spec
            return rate?.Status == "Draft";
        }

        public bool CanEditExchangeRate(Guid id)
        {
            // Per spec: Only Draft rates are editable
            var rate = _exchangeRates.FirstOrDefault(e => e.Id == id);
            return rate?.Status == "Draft";
        }

        // Validation: BaseCurrency must not equal QuoteCurrency
        public bool ValidateCurrencyPair(Guid baseCurrencyId, Guid quoteCurrencyId)
        {
            return baseCurrencyId != quoteCurrencyId;
        }
    }
}
