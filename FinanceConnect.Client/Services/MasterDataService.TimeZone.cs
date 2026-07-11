using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    public partial class MasterDataService
    {
        // TimeZone Methods
        public List<TimeZoneModel> GetAllTimeZones() => _timeZones.Where(t => !t.IsDeleted).OrderBy(t => t.SortOrder).ToList();
        
        public TimeZoneModel? GetTimeZoneById(Guid id) => _timeZones.FirstOrDefault(t => t.Id == id && !t.IsDeleted);
        
        public void AddTimeZone(TimeZoneModel timeZone)
        {
            timeZone.Id = Guid.NewGuid();
            timeZone.CreatedAt = DateTime.Now;
            timeZone.IsActive = true;
            timeZone.IsDeleted = false;
            _timeZones.Add(timeZone);
        }
        
        public void UpdateTimeZone(TimeZoneModel timeZone)
        {
            var existing = _timeZones.FirstOrDefault(t => t.Id == timeZone.Id);
            if (existing != null)
            {
                var index = _timeZones.IndexOf(existing);
                timeZone.UpdatedAt = DateTime.Now;
                _timeZones[index] = timeZone;
            }
        }

        public void ActivateTimeZone(Guid id)
        {
            var tz = _timeZones.FirstOrDefault(t => t.Id == id);
            if (tz != null)
            {
                tz.IsActive = true;
                tz.Status = "Active";
                tz.UpdatedAt = DateTime.Now;
            }
        }

        public void DeactivateTimeZone(Guid id)
        {
            var tz = _timeZones.FirstOrDefault(t => t.Id == id);
            if (tz != null)
            {
                tz.IsActive = false;
                tz.Status = "Inactive";
                tz.UpdatedAt = DateTime.Now;
            }
        }
        
        public void DeleteTimeZone(Guid id)
        {
            var timeZone = _timeZones.FirstOrDefault(t => t.Id == id);
            if (timeZone != null)
            {
                timeZone.IsActive = false;
                timeZone.IsDeleted = true;
                timeZone.DeletedAt = DateTime.Now;
            }
        }

        public bool CanDeactivateTimeZone(Guid id)
        {
            // Time zones can generally be deactivated, check for critical usage if needed
            return true;
        }

        public bool CanDeleteTimeZone(Guid id)
        {
            // Check if any state/province references this time zone
            var hasStates = _stateProvinces.Any(s => s.DefaultTimeZoneId == id && !s.IsDeleted);
            return !hasStates;
        }
    }
}
