using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    public partial class MasterDataService
    {
        // StateProvince Methods
        public List<StateProvinceModel> GetAllStateProvinces() => _stateProvinces.Where(s => !s.IsDeleted).OrderBy(s => s.SortOrder).ToList();
        
        public List<StateProvinceModel> GetStateProvincesByCountry(Guid countryId) => 
            _stateProvinces.Where(s => s.CountryId == countryId && !s.IsDeleted).OrderBy(s => s.SortOrder).ToList();
        
        public StateProvinceModel? GetStateProvinceById(Guid id) => _stateProvinces.FirstOrDefault(s => s.Id == id && !s.IsDeleted);
        
        public void AddStateProvince(StateProvinceModel state)
        {
            state.Id = Guid.NewGuid();
            state.CreatedAt = DateTime.Now;
            state.IsActive = true;
            state.IsDeleted = false;
            _stateProvinces.Add(state);
        }
        
        public void UpdateStateProvince(StateProvinceModel state)
        {
            var existing = _stateProvinces.FirstOrDefault(s => s.Id == state.Id);
            if (existing != null)
            {
                var index = _stateProvinces.IndexOf(existing);
                state.UpdatedAt = DateTime.Now;
                _stateProvinces[index] = state;
            }
        }

        public bool CanDeactivateState(Guid id)
        {
            // Check if any active city is linked to this state
            return !_cities.Any(c => c.StateProvinceId == id && c.IsActive && !c.IsDeleted);
        }

        public bool CanDeleteState(Guid id)
        {
            // Check if any city (active or inactive) is linked to this state
            return !_cities.Any(c => c.StateProvinceId == id && !c.IsDeleted);
        }

        public void ActivateState(Guid id)
        {
            var state = _stateProvinces.FirstOrDefault(s => s.Id == id);
            if (state != null)
            {
                state.IsActive = true;
                state.Status = "Active";
                state.UpdatedAt = DateTime.Now;
            }
        }

        public void DeactivateState(Guid id)
        {
            var state = _stateProvinces.FirstOrDefault(s => s.Id == id);
            if (state != null)
            {
                state.IsActive = false;
                state.Status = "Inactive";
                state.UpdatedAt = DateTime.Now;
            }
        }

        // StateProvince alias methods for consistent naming
        public void ActivateStateProvince(Guid id) => ActivateState(id);
        public void DeactivateStateProvince(Guid id) => DeactivateState(id);
        public bool CanDeactivateStateProvince(Guid id) => CanDeactivateState(id);
        public bool CanDeleteStateProvince(Guid id) => CanDeleteState(id);
        
        public void DeleteStateProvince(Guid id)
        {
            var state = _stateProvinces.FirstOrDefault(s => s.Id == id);
            if (state != null)
            {
                state.IsActive = false;
                state.IsDeleted = true;
                state.DeletedAt = DateTime.Now;
            }
        }
    }
}
