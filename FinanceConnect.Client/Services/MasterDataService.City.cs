using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    public partial class MasterDataService
    {
        // City Methods
        public List<CityModel> GetAllCities() => _cities.Where(c => !c.IsDeleted).ToList();
        
        public List<CityModel> GetCitiesByCountry(Guid countryId) => 
            _cities.Where(c => c.CountryId == countryId && !c.IsDeleted).ToList();
        
        public List<CityModel> GetCitiesByState(Guid stateProvinceId) => 
            _cities.Where(c => c.StateProvinceId == stateProvinceId && !c.IsDeleted).ToList();
        
        public CityModel? GetCityById(Guid id) => _cities.FirstOrDefault(c => c.Id == id && !c.IsDeleted);
        
        public void AddCity(CityModel city)
        {
            city.Id = Guid.NewGuid();
            city.CreatedAt = DateTime.Now;
            city.IsActive = true;
            city.IsDeleted = false;
            _cities.Add(city);
        }
        
        public void UpdateCity(CityModel city)
        {
            var existing = _cities.FirstOrDefault(c => c.Id == city.Id);
            if (existing != null)
            {
                var index = _cities.IndexOf(existing);
                city.UpdatedAt = DateTime.Now;
                _cities[index] = city;
            }
        }

        public void ActivateCity(Guid id)
        {
            var city = _cities.FirstOrDefault(c => c.Id == id);
            if (city != null)
            {
                city.IsActive = true;
                city.Status = "Active";
                city.UpdatedAt = DateTime.Now;
            }
        }

        public void DeactivateCity(Guid id)
        {
            var city = _cities.FirstOrDefault(c => c.Id == id);
            if (city != null)
            {
                city.IsActive = false;
                city.Status = "Inactive";
                city.UpdatedAt = DateTime.Now;
            }
        }
        
        public void DeleteCity(Guid id)
        {
            var city = _cities.FirstOrDefault(c => c.Id == id);
            if (city != null)
            {
                city.IsActive = false;
                city.IsDeleted = true;
                city.DeletedAt = DateTime.Now;
            }
        }

        public bool CanDeactivateCity(Guid id)
        {
            // Cities can generally be deactivated - add business logic if needed
            return true;
        }

        public bool CanDeleteCity(Guid id)
        {
            // Cities can be deleted if not referenced elsewhere - add checks if needed
            return true;
        }
    }
}
