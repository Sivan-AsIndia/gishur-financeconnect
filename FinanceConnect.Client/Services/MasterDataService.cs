using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    /// <summary>
    /// Central in-memory data service for all master/reference data.
    /// Seed data lives in the Data/ folder; service methods are in partial-class files.
    /// </summary>
    public partial class MasterDataService
    {
        // NOTE (Demo app): Users expect the Refresh icon to restore the original sample data
        // (i.e., undo adds/deletes) similar to re-fetching from a backend.
        // We therefore keep immutable seed snapshots and operate on mutable working lists.

        // ───────── Immutable seed snapshots (built once) ─────────
        private readonly List<CountryModel> _seedCountries;
        private readonly List<StateProvinceModel> _seedStateProvinces;
        private readonly List<TimeZoneModel> _seedTimeZones;
        private readonly List<CityModel> _seedCities;
        private readonly List<CurrencyModel> _seedCurrencies;
        private readonly List<CompanyModel> _seedCompanies;
        private readonly List<ExchangeRateModel> _seedExchangeRates;
        private readonly List<GeneralLedgerEntryModel> _seedGeneralLedgerEntries;
        private readonly List<LedgerModel> _seedLedgers;
        private readonly List<OpeningBalanceModel> _seedOpeningBalances;
        private readonly List<ClosingBalanceModel> _seedClosingBalances;

        // ───────── Working (mutable) data ─────────
        private List<CountryModel> _countries = new();
        private List<StateProvinceModel> _stateProvinces = new();
        private List<TimeZoneModel> _timeZones = new();
        private List<CityModel> _cities = new();
        private List<CurrencyModel> _currencies = new();
        private List<CompanyModel> _companies = new();
        private List<ExchangeRateModel> _exchangeRates = new();
        private List<GeneralLedgerEntryModel> _generalLedgerEntries = new();
        private List<LedgerModel> _ledgers = new();
        private List<OpeningBalanceModel> _openingBalances = new();
        private List<ClosingBalanceModel> _closingBalances = new();

        public MasterDataService()
        {
            // Build seed snapshots in dependency order using Data/ classes.
            // Leaf entities first, then dependent entities that resolve FK names.
            _seedCountries              = CountrySeedData.GetSeedData();
            _seedTimeZones              = TimeZoneSeedData.GetSeedData();
            _seedCurrencies             = CurrencySeedData.GetSeedData();

            _seedStateProvinces         = StateProvinceSeedData.GetSeedData(_seedCountries);
            _seedCities                 = CitySeedData.GetSeedData(_seedCountries, _seedStateProvinces);
            _seedCompanies              = CompanySeedData.GetSeedData(_seedCountries, _seedStateProvinces, _seedCities, _seedCurrencies);
            _seedExchangeRates          = ExchangeRateSeedData.GetSeedData(_seedCurrencies, _seedCompanies);

            _seedGeneralLedgerEntries   = GeneralLedgerEntrySeedData.GetSeedData();
            _seedLedgers                = LedgerSeedData.GetSeedData();
            _seedOpeningBalances        = OpeningBalanceSeedData.GetSeedData();
            _seedClosingBalances        = ClosingBalanceSeedData.GetSeedData();

            ResetWorkingData();
        }

        // ───────── Clone / Reset helpers ─────────

        private static List<T> CloneList<T>(IEnumerable<T> source) where T : class
        {
            // Shallow clone via JSON round-trip (BCL only, no extra deps)
            var json = System.Text.Json.JsonSerializer.Serialize(source);
            return System.Text.Json.JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }

        private void ResetWorkingData()
        {
            _countries              = CloneList(_seedCountries);
            _stateProvinces         = CloneList(_seedStateProvinces);
            _timeZones              = CloneList(_seedTimeZones);
            _cities                 = CloneList(_seedCities);
            _currencies             = CloneList(_seedCurrencies);
            _companies              = CloneList(_seedCompanies);
            _exchangeRates          = CloneList(_seedExchangeRates);
            _generalLedgerEntries   = CloneList(_seedGeneralLedgerEntries);
            _ledgers                = CloneList(_seedLedgers);
            _openingBalances        = CloneList(_seedOpeningBalances);
            _closingBalances        = CloneList(_seedClosingBalances);
        }

        // ───────── Per-entity reset helpers ─────────
        public void ResetCountriesToSeed()              => _countries              = CloneList(_seedCountries);
        public void ResetStateProvincesToSeed()          => _stateProvinces         = CloneList(_seedStateProvinces);
        public void ResetTimeZonesToSeed()               => _timeZones              = CloneList(_seedTimeZones);
        public void ResetCitiesToSeed()                  => _cities                 = CloneList(_seedCities);
        public void ResetCurrenciesToSeed()              => _currencies             = CloneList(_seedCurrencies);
        public void ResetCompaniesToSeed()               => _companies              = CloneList(_seedCompanies);
        public void ResetExchangeRatesToSeed()           => _exchangeRates          = CloneList(_seedExchangeRates);
        public void ResetGeneralLedgerEntriesToSeed()    => _generalLedgerEntries   = CloneList(_seedGeneralLedgerEntries);
        public void ResetLedgersToSeed()                 => _ledgers                = CloneList(_seedLedgers);
        public void ResetOpeningBalancesToSeed()          => _openingBalances        = CloneList(_seedOpeningBalances);
        public void ResetClosingBalancesToSeed()          => _closingBalances        = CloneList(_seedClosingBalances);
    }
}
