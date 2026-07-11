using FinanceConnect.Client.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceConnect.Client.Services
{
    public interface ICurrencyService
    {
        Task<List<CurrencyModel>> GetCurrenciesAsync();
    }

    public class CurrencyService : ICurrencyService
    {
        private readonly List<CurrencyModel> _currencies = new()
        {
            new CurrencyModel
            {
                Id = Guid.NewGuid(),
                CurrencyCode = "INR",
                CurrencyName = "Indian Rupee",
                Symbol = "₹",
                DecimalPlaces = 2,
                CurrencyType = "Fiat",
                IsActive = true
            },
            new CurrencyModel
            {
                Id = Guid.NewGuid(),
                CurrencyCode = "USD",
                CurrencyName = "US Dollar",
                Symbol = "$",
                DecimalPlaces = 2,
                CurrencyType = "Fiat",
                IsActive = true
            },
            new CurrencyModel
            {
                Id = Guid.NewGuid(),
                CurrencyCode = "EUR",
                CurrencyName = "Euro",
                Symbol = "€",
                DecimalPlaces = 2,
                CurrencyType = "Fiat",
                IsActive = true
            }
        };

        public Task<List<CurrencyModel>> GetCurrenciesAsync()
        {
            return Task.FromResult(_currencies);
        }

        public List<CurrencyModel> GetAll()
        {
            return _currencies;
        }
    }
}
