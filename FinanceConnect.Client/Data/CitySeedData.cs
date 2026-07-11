using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    public static class CitySeedData
    {
        public static List<CityModel> GetSeedData(List<CountryModel> countries, List<StateProvinceModel> states)
        {
            var india = countries.First(c => c.Id == MasterDataIds.Countries.India);
            var us    = countries.First(c => c.Id == MasterDataIds.Countries.UnitedStates);
            var uae   = countries.First(c => c.Id == MasterDataIds.Countries.UAE);
            var sg    = countries.First(c => c.Id == MasterDataIds.Countries.Singapore);

            var tamilNadu   = states.First(s => s.Id == MasterDataIds.States.TamilNadu);
            var karnataka   = states.First(s => s.Id == MasterDataIds.States.Karnataka);
            var maharashtra  = states.First(s => s.Id == MasterDataIds.States.Maharashtra);
            var delhi       = states.First(s => s.Id == MasterDataIds.States.Delhi);
            var telangana   = states.First(s => s.Id == MasterDataIds.States.Telangana);
            var westBengal  = states.First(s => s.Id == MasterDataIds.States.WestBengal);
            var california  = states.First(s => s.Id == MasterDataIds.States.California);
            var texas       = states.First(s => s.Id == MasterDataIds.States.Texas);
            var dubaiState  = states.First(s => s.Id == MasterDataIds.States.Dubai);
            var abuDhabi    = states.First(s => s.Id == MasterDataIds.States.AbuDhabi);
            var sgState     = states.First(s => s.Id == MasterDataIds.States.SingaporeState);

            return new List<CityModel>
            {
                // India - Tamil Nadu
                new CityModel { Id = MasterDataIds.Cities.Chennai, CountryId = india.Id, CountryName = india.CountryName, StateProvinceId = tamilNadu.Id, StateProvinceName = tamilNadu.StateProvinceName, CityCode = "MAA", CityName = "Chennai", IsMetro = true, IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-18), CreatedBy = "System" },
                new CityModel { Id = MasterDataIds.Cities.Coimbatore, CountryId = india.Id, CountryName = india.CountryName, StateProvinceId = tamilNadu.Id, StateProvinceName = tamilNadu.StateProvinceName, CityCode = "CJB", CityName = "Coimbatore", IsMetro = true, IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-18), CreatedBy = "System" },

                // India - Karnataka
                new CityModel { Id = MasterDataIds.Cities.Bengaluru, CountryId = india.Id, CountryName = india.CountryName, StateProvinceId = karnataka.Id, StateProvinceName = karnataka.StateProvinceName, CityCode = "BLR", CityName = "Bengaluru", IsMetro = true, IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-18), CreatedBy = "System" },

                // India - Maharashtra
                new CityModel { Id = MasterDataIds.Cities.Mumbai, CountryId = india.Id, CountryName = india.CountryName, StateProvinceId = maharashtra.Id, StateProvinceName = maharashtra.StateProvinceName, CityCode = "BOM", CityName = "Mumbai", IsMetro = true, IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-18), CreatedBy = "System" },

                // USA - California
                new CityModel { Id = MasterDataIds.Cities.SanFrancisco, CountryId = us.Id, CountryName = us.CountryName, StateProvinceId = california.Id, StateProvinceName = california.StateProvinceName, CityCode = "SFO", CityName = "San Francisco", IsMetro = true, DisplayName = "(UTC-08:00) Pacific Time (US & Canada)", IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-18), CreatedBy = "System" },

                // USA - Texas
                new CityModel { Id = MasterDataIds.Cities.Dallas, CountryId = us.Id, CountryName = us.CountryName, StateProvinceId = texas.Id, StateProvinceName = texas.StateProvinceName, CityCode = "DAL", CityName = "Dallas", IsMetro = true, DisplayName = "(UTC-06:00) Central Time (US & Canada)", IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-18), CreatedBy = "System" },

                // UAE - Dubai
                new CityModel { Id = MasterDataIds.Cities.DubaiCity, CountryId = uae.Id, CountryName = uae.CountryName, StateProvinceId = dubaiState.Id, StateProvinceName = dubaiState.StateProvinceName, CityCode = "DXB", CityName = "Dubai", IsMetro = true, DisplayName = "(UTC+04:00) Gulf Standard Time", IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-18), CreatedBy = "System" },

                // UAE - Abu Dhabi
                new CityModel { Id = MasterDataIds.Cities.AbuDhabiCity, CountryId = uae.Id, CountryName = uae.CountryName, StateProvinceId = abuDhabi.Id, StateProvinceName = abuDhabi.StateProvinceName, CityCode = "AUH", CityName = "Abu Dhabi", IsMetro = true, DisplayName = "(UTC+04:00) Gulf Standard Time", IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-18), CreatedBy = "System" },

                // India - Delhi
                new CityModel { Id = MasterDataIds.Cities.NewDelhi, CountryId = india.Id, CountryName = india.CountryName, StateProvinceId = delhi.Id, StateProvinceName = delhi.StateProvinceName, CityCode = "DEL", CityName = "New Delhi", IsMetro = true, DisplayName = "(UTC+05:30) India Standard Time", IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-18), CreatedBy = "System" },

                // India - Maharashtra - Pune
                new CityModel { Id = MasterDataIds.Cities.Pune, CountryId = india.Id, CountryName = india.CountryName, StateProvinceId = maharashtra.Id, StateProvinceName = maharashtra.StateProvinceName, CityCode = "PNQ", CityName = "Pune", IsMetro = true, DisplayName = "(UTC+05:30) India Standard Time", IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-18), CreatedBy = "System" },

                // India - Telangana - Hyderabad
                new CityModel { Id = MasterDataIds.Cities.Hyderabad, CountryId = india.Id, CountryName = india.CountryName, StateProvinceId = telangana.Id, StateProvinceName = telangana.StateProvinceName, CityCode = "HYD", CityName = "Hyderabad", IsMetro = true, DisplayName = "(UTC+05:30) India Standard Time", IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-18), CreatedBy = "System" },

                // India - West Bengal - Kolkata
                new CityModel { Id = MasterDataIds.Cities.Kolkata, CountryId = india.Id, CountryName = india.CountryName, StateProvinceId = westBengal.Id, StateProvinceName = westBengal.StateProvinceName, CityCode = "CCU", CityName = "Kolkata", IsMetro = true, DisplayName = "(UTC+05:30) India Standard Time", IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-18), CreatedBy = "System" },

                // Singapore
                new CityModel { Id = MasterDataIds.Cities.SingaporeCity, CountryId = sg.Id, CountryName = sg.CountryName, StateProvinceId = sgState.Id, StateProvinceName = sgState.StateProvinceName, CityCode = "SIN", CityName = "Singapore", IsMetro = true, DisplayName = "(UTC+08:00) Singapore Standard Time", IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-18), CreatedBy = "System" },
            };
        }
    }
}
