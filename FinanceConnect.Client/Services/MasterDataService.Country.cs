using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    public partial class MasterDataService
    {
        // Country Methods
        public List<CountryModel> GetAllCountries() => _countries.Where(c => !c.IsDeleted).OrderBy(c => c.SortOrder).ToList();
        
        public CountryModel? GetCountryById(Guid id) => _countries.FirstOrDefault(c => c.Id == id && !c.IsDeleted);
        
        public void AddCountry(CountryModel country)
        {
            country.Id = Guid.NewGuid();
            country.CreatedAt = DateTime.Now;
            country.IsActive = true;
            country.IsDeleted = false;
            _countries.Add(country);
        }
        
        public void UpdateCountry(CountryModel country)
        {
            var existing = _countries.FirstOrDefault(c => c.Id == country.Id);
            if (existing != null)
            {
                var index = _countries.IndexOf(existing);
                country.UpdatedAt = DateTime.Now;
                _countries[index] = country;
            }
        }
        
        public bool CanDeactivateCountry(Guid id)
        {
            // Check if any active state is linked to this country
            return !_stateProvinces.Any(s => s.CountryId == id && s.IsActive && !s.IsDeleted);
        }

        public bool CanDeleteCountry(Guid id)
        {
            // Check if any state (active or inactive) is linked to this country
            return !_stateProvinces.Any(s => s.CountryId == id && !s.IsDeleted);
        }
        
        public void ActivateCountry(Guid id)
        {
            var country = _countries.FirstOrDefault(c => c.Id == id);
            if (country != null)
            {
                country.IsActive = true;
                country.UpdatedAt = DateTime.Now;
            }
        }

        public void DeactivateCountry(Guid id)
        {
            var country = _countries.FirstOrDefault(c => c.Id == id);
            if (country != null)
            {
                country.IsActive = false;
                country.UpdatedAt = DateTime.Now;
            }
        }
        
        public void DeleteCountry(Guid id)
        {
            var country = _countries.FirstOrDefault(c => c.Id == id);
            if (country != null)
            {
                country.IsActive = false;
                country.IsDeleted = true;
                country.DeletedAt = DateTime.Now;
            }
        }
    }
}
