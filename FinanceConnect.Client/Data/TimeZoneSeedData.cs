using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    public static class TimeZoneSeedData
    {
        public static List<TimeZoneModel> GetSeedData()
        {
            return new List<TimeZoneModel>
            {
                new TimeZoneModel { Id = MasterDataIds.TimeZones.AsiaKolkata, TimeZoneKey = "Asia/Kolkata", DisplayName = "(UTC+05:30) India Standard Time", StandardUtcOffsetMinutes = 330, SupportsDST = false, IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-20), CreatedBy = "System" },
                new TimeZoneModel { Id = MasterDataIds.TimeZones.AmericaLosAngeles, TimeZoneKey = "America/Los_Angeles", DisplayName = "(UTC-08:00) Pacific Time (US & Canada)", StandardUtcOffsetMinutes = -480, SupportsDST = false, IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-20), CreatedBy = "System" },
                new TimeZoneModel { Id = MasterDataIds.TimeZones.AmericaChicago, TimeZoneKey = "America/Chicago", DisplayName = "(UTC-06:00) Central Time (US & Canada)", StandardUtcOffsetMinutes = -360, SupportsDST = false, IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-20), CreatedBy = "System" },
                new TimeZoneModel { Id = MasterDataIds.TimeZones.AsiaDubai, TimeZoneKey = "Asia/Dubai", DisplayName = "(UTC+04:00) Gulf Standard Time", StandardUtcOffsetMinutes = 240, SupportsDST = false, IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-20), CreatedBy = "System" },
                new TimeZoneModel { Id = MasterDataIds.TimeZones.AsiaSingapore, TimeZoneKey = "Asia/Singapore", DisplayName = "(UTC+08:00) Singapore Standard Time", StandardUtcOffsetMinutes = 480, SupportsDST = false, IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-20), CreatedBy = "System" },
                new TimeZoneModel { Id = MasterDataIds.TimeZones.EuropeLondon, TimeZoneKey = "Europe/London", DisplayName = "(UTC+00:00) Greenwich Mean Time", StandardUtcOffsetMinutes = 0, SupportsDST = true, IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-19), CreatedBy = "System" },
                new TimeZoneModel { Id = MasterDataIds.TimeZones.AsiaTokyo, TimeZoneKey = "Asia/Tokyo", DisplayName = "(UTC+09:00) Japan Standard Time", StandardUtcOffsetMinutes = 540, SupportsDST = false, IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-18), CreatedBy = "System" },
                new TimeZoneModel { Id = MasterDataIds.TimeZones.AustraliaSydney, TimeZoneKey = "Australia/Sydney", DisplayName = "(UTC+10:00) Australian Eastern Standard Time", StandardUtcOffsetMinutes = 600, SupportsDST = true, IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-17), CreatedBy = "System" },
                new TimeZoneModel { Id = MasterDataIds.TimeZones.EuropeBerlin, TimeZoneKey = "Europe/Berlin", DisplayName = "(UTC+01:00) Central European Time", StandardUtcOffsetMinutes = 60, SupportsDST = true, IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-16), CreatedBy = "System" },
                new TimeZoneModel { Id = MasterDataIds.TimeZones.AmericaToronto, TimeZoneKey = "America/Toronto", DisplayName = "(UTC-05:00) Eastern Time (US & Canada)", StandardUtcOffsetMinutes = -300, SupportsDST = true, IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-15), CreatedBy = "System" },
                new TimeZoneModel { Id = MasterDataIds.TimeZones.AsiaRiyadh, TimeZoneKey = "Asia/Riyadh", DisplayName = "(UTC+03:00) Arabian Standard Time", StandardUtcOffsetMinutes = 180, SupportsDST = false, IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-14), CreatedBy = "System" },
                new TimeZoneModel { Id = MasterDataIds.TimeZones.AsiaKualaLumpur, TimeZoneKey = "Asia/Kuala_Lumpur", DisplayName = "(UTC+08:00) Malaysia Time", StandardUtcOffsetMinutes = 480, SupportsDST = false, IsActive = false, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-13), CreatedBy = "System" },
            };
        }
    }
}
