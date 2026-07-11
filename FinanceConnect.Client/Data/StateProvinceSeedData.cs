using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    public static class StateProvinceSeedData
    {
        public static List<StateProvinceModel> GetSeedData(List<CountryModel> countries)
        {
            var india = countries.First(c => c.Id == MasterDataIds.Countries.India);
            var us    = countries.First(c => c.Id == MasterDataIds.Countries.UnitedStates);
            var uae   = countries.First(c => c.Id == MasterDataIds.Countries.UAE);
            var sg    = countries.First(c => c.Id == MasterDataIds.Countries.Singapore);

            return new List<StateProvinceModel>
            {
                // India
                new StateProvinceModel { Id = MasterDataIds.States.TamilNadu, CountryId = india.Id, CountryName = india.CountryName, StateProvinceCode = "TN", StateProvinceName = "Tamil Nadu", JurisdictionType = "State", IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-25), CreatedBy = "System" },
                new StateProvinceModel { Id = MasterDataIds.States.Karnataka, CountryId = india.Id, CountryName = india.CountryName, StateProvinceCode = "KA", StateProvinceName = "Karnataka", JurisdictionType = "State", IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-25), CreatedBy = "System" },
                new StateProvinceModel { Id = MasterDataIds.States.Maharashtra, CountryId = india.Id, CountryName = india.CountryName, StateProvinceCode = "MH", StateProvinceName = "Maharashtra", JurisdictionType = "State", IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-25), CreatedBy = "System" },
                new StateProvinceModel { Id = MasterDataIds.States.Delhi, CountryId = india.Id, CountryName = india.CountryName, StateProvinceCode = "DL", StateProvinceName = "Delhi", JurisdictionType = "Union Territory", IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-25), CreatedBy = "System" },
                new StateProvinceModel { Id = MasterDataIds.States.Telangana, CountryId = india.Id, CountryName = india.CountryName, StateProvinceCode = "TS", StateProvinceName = "Telangana", JurisdictionType = "State", IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-25), CreatedBy = "System" },
                new StateProvinceModel { Id = MasterDataIds.States.WestBengal, CountryId = india.Id, CountryName = india.CountryName, StateProvinceCode = "WB", StateProvinceName = "West Bengal", JurisdictionType = "State", IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-25), CreatedBy = "System" },

                // United States
                new StateProvinceModel { Id = MasterDataIds.States.California, CountryId = us.Id, CountryName = us.CountryName, StateProvinceCode = "CA", StateProvinceName = "California", JurisdictionType = "State", IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-25), CreatedBy = "System" },
                new StateProvinceModel { Id = MasterDataIds.States.Texas, CountryId = us.Id, CountryName = us.CountryName, StateProvinceCode = "TX", StateProvinceName = "Texas", JurisdictionType = "State", IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-25), CreatedBy = "System" },

                // UAE
                new StateProvinceModel { Id = MasterDataIds.States.Dubai, CountryId = uae.Id, CountryName = uae.CountryName, StateProvinceCode = "DU", StateProvinceName = "Dubai", JurisdictionType = "Emirate", IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-25), CreatedBy = "System" },
                new StateProvinceModel { Id = MasterDataIds.States.AbuDhabi, CountryId = uae.Id, CountryName = uae.CountryName, StateProvinceCode = "AD", StateProvinceName = "Abu Dhabi", JurisdictionType = "Emirate", IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-25), CreatedBy = "System" },

                // Singapore (City-State)
                new StateProvinceModel { Id = MasterDataIds.States.SingaporeState, CountryId = sg.Id, CountryName = sg.CountryName, StateProvinceCode = "SG", StateProvinceName = "Singapore", JurisdictionType = "City-State", IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-25), CreatedBy = "System" },
            };
        }
    }
}
