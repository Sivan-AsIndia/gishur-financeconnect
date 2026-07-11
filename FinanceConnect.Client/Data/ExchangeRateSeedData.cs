using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    public static class ExchangeRateSeedData
    {
        public static List<ExchangeRateModel> GetSeedData(List<CurrencyModel> currencies, List<CompanyModel> companies)
        {
            var inr = currencies.First(c => c.Id == MasterDataIds.Currencies.INR);
            var usd = currencies.First(c => c.Id == MasterDataIds.Currencies.USD);
            var aed = currencies.First(c => c.Id == MasterDataIds.Currencies.AED);
            var gbp = currencies.First(c => c.Id == MasterDataIds.Currencies.GBP);
            var eur = currencies.First(c => c.Id == MasterDataIds.Currencies.EUR);
            var jpy = currencies.First(c => c.Id == MasterDataIds.Currencies.JPY);
            var sgd = currencies.First(c => c.Id == MasterDataIds.Currencies.SGD);
            var aud = currencies.First(c => c.Id == MasterDataIds.Currencies.AUD);
            var cad = currencies.First(c => c.Id == MasterDataIds.Currencies.CAD);
            var sar = currencies.First(c => c.Id == MasterDataIds.Currencies.SAR);

            var sofaCraft = companies.First(c => c.Id == MasterDataIds.Companies.SofaCraft);

            return new List<ExchangeRateModel>
            {
                Build(MasterDataIds.ExchangeRates.UsdInr, usd, inr, sofaCraft, DateTime.Today.AddDays(-2), "Spot", 83.25000000m, "1 USD = 83.25 INR (sample)", "Active"),
                Build(MasterDataIds.ExchangeRates.InrUsd, inr, usd, sofaCraft, DateTime.Today.AddDays(-2), "Spot", 0.01201201m, "1 INR \u2248 0.0120 USD (sample inverse)", "Active"),
                Build(MasterDataIds.ExchangeRates.AedInr, aed, inr, sofaCraft, DateTime.Today.AddDays(-5), "Spot", 22.67000000m, "1 AED = 22.67 INR (sample)", "Active"),
                Build(MasterDataIds.ExchangeRates.InrAed, inr, aed, sofaCraft, DateTime.Today.AddDays(-5), "Spot", 0.04412000m, "1 INR \u2248 0.04412 AED (sample inverse)", "Active"),
                Build(MasterDataIds.ExchangeRates.GbpInr, gbp, inr, sofaCraft, DateTime.Today.AddDays(-1), "Spot", 105.50000000m, "1 GBP = 105.50 INR (sample)", "Active"),
                Build(MasterDataIds.ExchangeRates.EurInr, eur, inr, sofaCraft, DateTime.Today.AddDays(-1), "Spot", 91.20000000m, "1 EUR = 91.20 INR (sample)", "Active"),
                Build(MasterDataIds.ExchangeRates.JpyInr, jpy, inr, sofaCraft, DateTime.Today.AddDays(-3), "Spot", 0.55600000m, "1 JPY = 0.556 INR (sample)", "Active"),
                Build(MasterDataIds.ExchangeRates.SgdInr, sgd, inr, sofaCraft, DateTime.Today.AddDays(-3), "Spot", 62.15000000m, "1 SGD = 62.15 INR (sample)", "Active"),
                Build(MasterDataIds.ExchangeRates.UsdInrMonthly, usd, inr, sofaCraft, DateTime.Today.AddDays(-30), "Monthly", 82.90000000m, "Monthly avg rate USD/INR (prior month)", "Active"),
                Build(MasterDataIds.ExchangeRates.AudInr, aud, inr, sofaCraft, DateTime.Today.AddDays(-4), "Spot", 54.80000000m, "1 AUD = 54.80 INR (sample)", "Active"),
                Build(MasterDataIds.ExchangeRates.CadInr, cad, inr, sofaCraft, DateTime.Today.AddDays(-4), "Spot", 61.35000000m, "1 CAD = 61.35 INR (sample)", "Active"),
                Build(MasterDataIds.ExchangeRates.SarInr, sar, inr, sofaCraft, DateTime.Today.AddDays(-6), "Spot", 22.20000000m, "1 SAR = 22.20 INR (sample)", "Expired"),
            };
        }

        private static ExchangeRateModel Build(
            Guid id, CurrencyModel baseCur, CurrencyModel quoteCur, CompanyModel company,
            DateTime rateDate, string rateType, decimal rate, string notes, string status)
        {
            return new ExchangeRateModel
            {
                Id = id,
                BaseCurrencyId = baseCur.Id, BaseCurrencyCode = baseCur.CurrencyCode, BaseCurrencyName = baseCur.CurrencyName,
                QuoteCurrencyId = quoteCur.Id, QuoteCurrencyCode = quoteCur.CurrencyCode, QuoteCurrencyName = quoteCur.CurrencyName,
                CompanyId = company.Id, CompanyCode = company.CompanyCode, CompanyName = company.LegalName,
                RateDate = rateDate, RateType = rateType, Rate = rate,
                SourceType = "ManualEntry", SourceName = "Demo Seed",
                Notes = notes, Status = status, VersionNo = 1,
                CreatedAt = DateTime.Now.AddDays((rateDate - DateTime.Today).Days),
                CreatedBy = "System"
            };
        }
    }
}
