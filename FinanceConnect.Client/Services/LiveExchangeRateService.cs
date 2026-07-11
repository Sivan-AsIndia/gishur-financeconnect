using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace FinanceConnect.Client.Services
{
    /// <summary>
    /// Service to fetch live exchange rates from free APIs
    /// Uses ExchangeRate-API (exchangerate-api.com) - free tier available
    /// </summary>
    public class LiveExchangeRateService
    {
        private readonly HttpClient _httpClient;
        
        // Free API endpoints (no API key required for basic usage)
        private const string BaseUrl = "https://api.exchangerate-api.com/v4/latest/";
        
        // Alternative free APIs as fallback
        private const string FallbackUrl = "https://open.er-api.com/v6/latest/";

        public LiveExchangeRateService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Fetch live exchange rate for a currency pair
        /// </summary>
        public async Task<LiveRateResult> GetLiveRateAsync(string baseCurrency, string quoteCurrency)
        {
            try
            {
                // Try primary API
                var result = await FetchFromApiAsync(BaseUrl, baseCurrency, quoteCurrency);
                if (result.Success)
                    return result;

                // Try fallback API
                result = await FetchFromApiAsync(FallbackUrl, baseCurrency, quoteCurrency);
                return result;
            }
            catch (Exception ex)
            {
                return new LiveRateResult
                {
                    Success = false,
                    ErrorMessage = $"Failed to fetch live rate: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Fetch all available rates for a base currency
        /// </summary>
        public async Task<LiveRatesResult> GetAllLiveRatesAsync(string baseCurrency)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}{baseCurrency}");
                
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<ExchangeRateApiResponse>();
                    
                    if (data != null && data.Rates != null)
                    {
                        return new LiveRatesResult
                        {
                            Success = true,
                            BaseCurrency = baseCurrency,
                            Rates = data.Rates,
                            LastUpdated = DateTime.Parse(data.Date ?? DateTime.Today.ToString("yyyy-MM-dd")),
                            Provider = data.Provider ?? "ExchangeRate-API"
                        };
                    }
                }

                // Try fallback
                response = await _httpClient.GetAsync($"{FallbackUrl}{baseCurrency}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<ExchangeRateApiResponse>();
                    
                    if (data != null && data.Rates != null)
                    {
                        return new LiveRatesResult
                        {
                            Success = true,
                            BaseCurrency = baseCurrency,
                            Rates = data.Rates,
                            LastUpdated = DateTime.Parse(data.Date ?? DateTime.Today.ToString("yyyy-MM-dd")),
                            Provider = "Open Exchange Rate API"
                        };
                    }
                }

                return new LiveRatesResult
                {
                    Success = false,
                    ErrorMessage = "Unable to fetch rates from API"
                };
            }
            catch (Exception ex)
            {
                return new LiveRatesResult
                {
                    Success = false,
                    ErrorMessage = $"Failed to fetch live rates: {ex.Message}"
                };
            }
        }

        private async Task<LiveRateResult> FetchFromApiAsync(string baseUrl, string baseCurrency, string quoteCurrency)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{baseUrl}{baseCurrency.ToUpper()}");
                
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<ExchangeRateApiResponse>();
                    
                    if (data != null && data.Rates != null && data.Rates.ContainsKey(quoteCurrency.ToUpper()))
                    {
                        var rate = data.Rates[quoteCurrency.ToUpper()];
                        return new LiveRateResult
                        {
                            Success = true,
                            BaseCurrency = baseCurrency.ToUpper(),
                            QuoteCurrency = quoteCurrency.ToUpper(),
                            Rate = (decimal)rate,
                            LastUpdated = DateTime.Parse(data.Date ?? DateTime.Today.ToString("yyyy-MM-dd")),
                            Provider = data.Provider ?? "ExchangeRate-API"
                        };
                    }
                }

                return new LiveRateResult
                {
                    Success = false,
                    ErrorMessage = $"Rate not found for {baseCurrency}/{quoteCurrency}"
                };
            }
            catch
            {
                return new LiveRateResult
                {
                    Success = false,
                    ErrorMessage = "API request failed"
                };
            }
        }

        /// <summary>
        /// Get supported currencies list
        /// </summary>
        public List<string> GetSupportedCurrencies()
        {
            return new List<string>
            {
                "USD", "EUR", "GBP", "INR", "JPY", "SGD", "AUD", "CAD", "CHF", "CNY",
                "HKD", "NZD", "SEK", "KRW", "MXN", "NOK", "BRL", "ZAR", "RUB", "AED"
            };
        }
    }

    /// <summary>
    /// Response model for exchange rate APIs
    /// </summary>
    public class ExchangeRateApiResponse
    {
        [JsonPropertyName("provider")]
        public string? Provider { get; set; }

        [JsonPropertyName("base")]
        public string? Base { get; set; }

        [JsonPropertyName("date")]
        public string? Date { get; set; }

        [JsonPropertyName("time_last_update_utc")]
        public string? TimeLastUpdateUtc { get; set; }

        [JsonPropertyName("rates")]
        public Dictionary<string, double>? Rates { get; set; }
    }

    /// <summary>
    /// Result model for single rate fetch
    /// </summary>
    public class LiveRateResult
    {
        public bool Success { get; set; }
        public string? BaseCurrency { get; set; }
        public string? QuoteCurrency { get; set; }
        public decimal Rate { get; set; }
        public DateTime LastUpdated { get; set; }
        public string? Provider { get; set; }
        public string? ErrorMessage { get; set; }

        public string CurrencyPair => $"{BaseCurrency}/{QuoteCurrency}";
        public decimal InverseRate => Rate > 0 ? Math.Round(1 / Rate, 8) : 0;
    }

    /// <summary>
    /// Result model for all rates fetch
    /// </summary>
    public class LiveRatesResult
    {
        public bool Success { get; set; }
        public string? BaseCurrency { get; set; }
        public Dictionary<string, double>? Rates { get; set; }
        public DateTime LastUpdated { get; set; }
        public string? Provider { get; set; }
        public string? ErrorMessage { get; set; }

        public decimal GetRate(string quoteCurrency)
        {
            if (Rates != null && Rates.ContainsKey(quoteCurrency.ToUpper()))
            {
                return (decimal)Rates[quoteCurrency.ToUpper()];
            }
            return 0;
        }
    }
}
