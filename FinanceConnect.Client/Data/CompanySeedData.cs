using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    public static class CompanySeedData
    {
        public static List<CompanyModel> GetSeedData(
            List<CountryModel> countries,
            List<StateProvinceModel> states,
            List<CityModel> cities,
            List<CurrencyModel> currencies)
        {
            // Resolve parent entities by their well-known IDs
            var india    = countries.First(c => c.Id == MasterDataIds.Countries.India);
            var us       = countries.First(c => c.Id == MasterDataIds.Countries.UnitedStates);
            var uae      = countries.First(c => c.Id == MasterDataIds.Countries.UAE);
            var sg       = countries.First(c => c.Id == MasterDataIds.Countries.Singapore);

            var tamilNadu   = states.First(s => s.Id == MasterDataIds.States.TamilNadu);
            var karnataka   = states.First(s => s.Id == MasterDataIds.States.Karnataka);
            var maharashtra  = states.First(s => s.Id == MasterDataIds.States.Maharashtra);
            var delhiState  = states.First(s => s.Id == MasterDataIds.States.Delhi);
            var telangana   = states.First(s => s.Id == MasterDataIds.States.Telangana);
            var westBengal  = states.First(s => s.Id == MasterDataIds.States.WestBengal);
            var california  = states.First(s => s.Id == MasterDataIds.States.California);
            var dubaiSt     = states.First(s => s.Id == MasterDataIds.States.Dubai);
            var abuDhabiSt  = states.First(s => s.Id == MasterDataIds.States.AbuDhabi);
            var sgState     = states.First(s => s.Id == MasterDataIds.States.SingaporeState);

            var chennai    = cities.First(c => c.Id == MasterDataIds.Cities.Chennai);
            var bengaluru  = cities.First(c => c.Id == MasterDataIds.Cities.Bengaluru);
            var mumbai     = cities.First(c => c.Id == MasterDataIds.Cities.Mumbai);
            var sanFran    = cities.First(c => c.Id == MasterDataIds.Cities.SanFrancisco);
            var newDelhi   = cities.First(c => c.Id == MasterDataIds.Cities.NewDelhi);
            var pune       = cities.First(c => c.Id == MasterDataIds.Cities.Pune);
            var hyderabad  = cities.First(c => c.Id == MasterDataIds.Cities.Hyderabad);
            var kolkata    = cities.First(c => c.Id == MasterDataIds.Cities.Kolkata);
            var dubaiCity  = cities.First(c => c.Id == MasterDataIds.Cities.DubaiCity);
            var abuDhCity  = cities.First(c => c.Id == MasterDataIds.Cities.AbuDhabiCity);
            var sgCity     = cities.First(c => c.Id == MasterDataIds.Cities.SingaporeCity);

            var inr = currencies.First(c => c.Id == MasterDataIds.Currencies.INR);
            var usd = currencies.First(c => c.Id == MasterDataIds.Currencies.USD);
            var aed = currencies.First(c => c.Id == MasterDataIds.Currencies.AED);
            var sgd = currencies.First(c => c.Id == MasterDataIds.Currencies.SGD);

            string CurDisplay(CurrencyModel cur) => $"{cur.CurrencyName} ({cur.CurrencyCode})";

            return new List<CompanyModel>
            {
                // 1 - SofaCraft (India - Chennai)
                new CompanyModel
                {
                    Id = MasterDataIds.Companies.SofaCraft, CompanyCode = "SOFA01",
                    LegalName = "SofaCraft Furnishings Private Limited", TradeName = "SofaCraft", ShortName = "SofaCraft",
                    LegalStructure = "Private Limited", IncorporationDate = new DateTime(2016, 8, 12),
                    RegistrationNumber = "U36999TN2016PTC098765", PANNumber = "AARCS1234K", GSTIN = "33AARCS1234K1Z9", IsGSTRegistered = true, TANNumber = "CHES12345F",
                    RegistrationCountryId = india.Id, RegistrationCountryName = india.CountryName,
                    RegistrationStateProvinceId = tamilNadu.Id, RegistrationStateProvinceName = tamilNadu.StateProvinceName,
                    RegistrationCityId = chennai.Id, RegistrationCityName = chennai.CityName,
                    AddressLine1 = "Plot 42, SIDCO Industrial Estate", AddressLine2 = "Ambattur",
                    CountryId = india.Id, CountryName = india.CountryName,
                    StateProvinceId = tamilNadu.Id, StateProvinceName = tamilNadu.StateProvinceName,
                    CityId = chennai.Id, CityName = chennai.CityName, PostalCode = "600098",
                    PrimaryContactName = "Ananya Iyer", PrimaryEmail = "finance@sofacraft.in", PrimaryPhone = "+91-44-40001234", WebsiteUrl = "https://www.sofacraft.in",
                    BaseCurrencyId = inr.Id, BaseCurrencyName = CurDisplay(inr),
                    FiscalYearStartMonth = 4, BooksStartDate = new DateTime(2022, 4, 1), EnableMultiCurrency = true,
                    RoundingPrecision = 2, RoundingMode = "Round Half Up", Status = "Active", IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-365), CreatedBy = "System"
                },

                // 2 - SofaCraft USA (San Francisco)
                new CompanyModel
                {
                    Id = MasterDataIds.Companies.SofaCraftUSA, CompanyCode = "SOFAUS01",
                    LegalName = "SofaCraft Retail USA Inc.", TradeName = "SofaCraft Retail", ShortName = "SofaCraft USA",
                    LegalStructure = "Corporation", IncorporationDate = new DateTime(2019, 3, 20),
                    RegistrationNumber = "C-2019-CA-558812", PANNumber = "", GSTIN = "", IsGSTRegistered = false, TANNumber = "",
                    RegistrationCountryId = us.Id, RegistrationCountryName = us.CountryName,
                    RegistrationStateProvinceId = california.Id, RegistrationStateProvinceName = california.StateProvinceName,
                    RegistrationCityId = sanFran.Id, RegistrationCityName = sanFran.CityName,
                    AddressLine1 = "120 Market Street", AddressLine2 = "Suite 550",
                    CountryId = us.Id, CountryName = us.CountryName,
                    StateProvinceId = california.Id, StateProvinceName = california.StateProvinceName,
                    CityId = sanFran.Id, CityName = sanFran.CityName, PostalCode = "94105",
                    PrimaryContactName = "Michael Chen", PrimaryEmail = "finance@sofacraftusa.com", PrimaryPhone = "+1-415-555-0199", WebsiteUrl = "https://www.sofacraftusa.com",
                    BaseCurrencyId = usd.Id, BaseCurrencyName = CurDisplay(usd),
                    FiscalYearStartMonth = 1, BooksStartDate = new DateTime(2023, 1, 1), EnableMultiCurrency = true,
                    RoundingPrecision = 2, RoundingMode = "Round Half Up", Status = "Active", IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-365), CreatedBy = "System"
                },

                // 3 - OakNest (India - Bengaluru)
                new CompanyModel
                {
                    Id = MasterDataIds.Companies.OakNest, CompanyCode = "OAK01",
                    LegalName = "OakNest Interiors LLP", TradeName = "OakNest", ShortName = "OakNest",
                    LegalStructure = "LLP", IncorporationDate = new DateTime(2018, 11, 5),
                    RegistrationNumber = "AAB-4218", PANNumber = "AABFO1122P", GSTIN = "29AABFO1122P1ZQ", IsGSTRegistered = true, TANNumber = "BLRO11223C",
                    RegistrationCountryId = india.Id, RegistrationCountryName = india.CountryName,
                    RegistrationStateProvinceId = karnataka.Id, RegistrationStateProvinceName = karnataka.StateProvinceName,
                    RegistrationCityId = bengaluru.Id, RegistrationCityName = bengaluru.CityName,
                    AddressLine1 = "4th Floor, Indiranagar", AddressLine2 = "100 Feet Road",
                    CountryId = india.Id, CountryName = india.CountryName,
                    StateProvinceId = karnataka.Id, StateProvinceName = karnataka.StateProvinceName,
                    CityId = bengaluru.Id, CityName = bengaluru.CityName, PostalCode = "560038",
                    PrimaryContactName = "Ritu Menon", PrimaryEmail = "accounts@oaknest.in", PrimaryPhone = "+91-80-41234567", WebsiteUrl = "https://www.oaknest.in",
                    BaseCurrencyId = inr.Id, BaseCurrencyName = CurDisplay(inr),
                    FiscalYearStartMonth = 4, BooksStartDate = new DateTime(2022, 4, 1), EnableMultiCurrency = false,
                    RoundingPrecision = 2, RoundingMode = "Round Half Up", Status = "Active", IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-365), CreatedBy = "System"
                },

                // 4 - UrbanLoft (India - Mumbai)
                new CompanyModel
                {
                    Id = MasterDataIds.Companies.UrbanLoft, CompanyCode = "URBN01",
                    LegalName = "UrbanLoft Home Décor Private Limited", TradeName = "UrbanLoft", ShortName = "UrbanLoft",
                    LegalStructure = "Private Limited", IncorporationDate = new DateTime(2020, 7, 14),
                    RegistrationNumber = "U74999MH2020PTC223344", PANNumber = "AAACU7788L", GSTIN = "27AAACU7788L1Z7", IsGSTRegistered = true, TANNumber = "MUMU77889D",
                    RegistrationCountryId = india.Id, RegistrationCountryName = india.CountryName,
                    RegistrationStateProvinceId = maharashtra.Id, RegistrationStateProvinceName = maharashtra.StateProvinceName,
                    RegistrationCityId = mumbai.Id, RegistrationCityName = mumbai.CityName,
                    AddressLine1 = "Bandra Kurla Complex", AddressLine2 = "Block C, 8th Floor",
                    CountryId = india.Id, CountryName = india.CountryName,
                    StateProvinceId = maharashtra.Id, StateProvinceName = maharashtra.StateProvinceName,
                    CityId = mumbai.Id, CityName = mumbai.CityName, PostalCode = "400051",
                    PrimaryContactName = "Neha Shah", PrimaryEmail = "finance@urbanloft.in", PrimaryPhone = "+91-22-49887766", WebsiteUrl = "https://www.urbanloft.in",
                    BaseCurrencyId = inr.Id, BaseCurrencyName = CurDisplay(inr),
                    FiscalYearStartMonth = 4, BooksStartDate = new DateTime(2022, 4, 1), EnableMultiCurrency = true,
                    RoundingPrecision = 2, RoundingMode = "Round Half Up", Status = "Active", IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-365), CreatedBy = "System"
                },

                // 5 - DesertDune (UAE - Dubai)
                new CompanyModel
                {
                    Id = MasterDataIds.Companies.DesertDune, CompanyCode = "DUNE01",
                    LegalName = "DesertDune Furniture Trading LLC", TradeName = "DesertDune", ShortName = "DesertDune",
                    LegalStructure = "LLC", IncorporationDate = new DateTime(2017, 2, 28),
                    RegistrationNumber = "DED-FT-2017-00921", PANNumber = "", GSTIN = "", IsGSTRegistered = false, TANNumber = "",
                    RegistrationCountryId = uae.Id, RegistrationCountryName = uae.CountryName,
                    RegistrationStateProvinceId = dubaiSt.Id, RegistrationStateProvinceName = dubaiSt.StateProvinceName,
                    RegistrationCityId = dubaiCity.Id, RegistrationCityName = dubaiCity.CityName,
                    AddressLine1 = "Business Bay", AddressLine2 = "Bay Square, Building 6",
                    CountryId = uae.Id, CountryName = uae.CountryName,
                    StateProvinceId = dubaiSt.Id, StateProvinceName = dubaiSt.StateProvinceName,
                    CityId = dubaiCity.Id, CityName = dubaiCity.CityName, PostalCode = "00000",
                    PrimaryContactName = "Adeel Khan", PrimaryEmail = "accounts@desertdune.ae", PrimaryPhone = "+971-4-5550123", WebsiteUrl = "https://www.desertdune.ae",
                    BaseCurrencyId = aed.Id, BaseCurrencyName = CurDisplay(aed),
                    FiscalYearStartMonth = 1, BooksStartDate = new DateTime(2023, 1, 1), EnableMultiCurrency = true,
                    RoundingPrecision = 2, RoundingMode = "Round Half Up", Status = "Active", IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-365), CreatedBy = "System"
                },

                // 6 - PlushComfort (India - New Delhi)
                new CompanyModel
                {
                    Id = MasterDataIds.Companies.PlushComfort, CompanyCode = "PLUSH01",
                    LegalName = "PlushComfort Sofas Private Limited", TradeName = "PlushComfort", ShortName = "PlushComfort",
                    LegalStructure = "Private Limited", IncorporationDate = new DateTime(2018, 5, 15),
                    RegistrationNumber = "U36999DL2018PTC334455", PANNumber = "AAFCP5678M", GSTIN = "07AAFCP5678M1ZR", IsGSTRegistered = true, TANNumber = "DELP56789A",
                    RegistrationCountryId = india.Id, RegistrationCountryName = india.CountryName,
                    RegistrationStateProvinceId = delhiState.Id, RegistrationStateProvinceName = delhiState.StateProvinceName,
                    RegistrationCityId = newDelhi.Id, RegistrationCityName = newDelhi.CityName,
                    AddressLine1 = "Plot 12, Okhla Industrial Area", AddressLine2 = "Phase II",
                    CountryId = india.Id, CountryName = india.CountryName,
                    StateProvinceId = delhiState.Id, StateProvinceName = delhiState.StateProvinceName,
                    CityId = newDelhi.Id, CityName = newDelhi.CityName, PostalCode = "110020",
                    PrimaryContactName = "Rajesh Kumar", PrimaryEmail = "finance@plushcomfort.in", PrimaryPhone = "+91-11-40001234", WebsiteUrl = "https://www.plushcomfort.in",
                    BaseCurrencyId = inr.Id, BaseCurrencyName = CurDisplay(inr),
                    FiscalYearStartMonth = 4, BooksStartDate = new DateTime(2022, 4, 1), EnableMultiCurrency = true,
                    RoundingPrecision = 2, RoundingMode = "Round Half Up", Status = "Active", IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-365), CreatedBy = "System"
                },

                // 7 - VelvetRest (India - Pune)
                new CompanyModel
                {
                    Id = MasterDataIds.Companies.VelvetRest, CompanyCode = "VELVET01",
                    LegalName = "VelvetRest Furniture Industries Private Limited", TradeName = "VelvetRest", ShortName = "VelvetRest",
                    LegalStructure = "Private Limited", IncorporationDate = new DateTime(2019, 9, 22),
                    RegistrationNumber = "U36999MH2019PTC445566", PANNumber = "AABCV7890N", GSTIN = "27AABCV7890N1Z5", IsGSTRegistered = true, TANNumber = "PUNV78901B",
                    RegistrationCountryId = india.Id, RegistrationCountryName = india.CountryName,
                    RegistrationStateProvinceId = maharashtra.Id, RegistrationStateProvinceName = maharashtra.StateProvinceName,
                    RegistrationCityId = pune.Id, RegistrationCityName = pune.CityName,
                    AddressLine1 = "Survey No. 45, Hinjewadi IT Park", AddressLine2 = "Phase III",
                    CountryId = india.Id, CountryName = india.CountryName,
                    StateProvinceId = maharashtra.Id, StateProvinceName = maharashtra.StateProvinceName,
                    CityId = pune.Id, CityName = pune.CityName, PostalCode = "411057",
                    PrimaryContactName = "Priya Sharma", PrimaryEmail = "accounts@velvetrest.in", PrimaryPhone = "+91-20-40005678", WebsiteUrl = "https://www.velvetrest.in",
                    BaseCurrencyId = inr.Id, BaseCurrencyName = CurDisplay(inr),
                    FiscalYearStartMonth = 4, BooksStartDate = new DateTime(2022, 4, 1), EnableMultiCurrency = false,
                    RoundingPrecision = 2, RoundingMode = "Round Half Up", Status = "Active", IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-365), CreatedBy = "System"
                },

                // 8 - CozyCraft (India - Hyderabad)
                new CompanyModel
                {
                    Id = MasterDataIds.Companies.CozyCraft, CompanyCode = "COZY01",
                    LegalName = "CozyCraft Living Solutions LLP", TradeName = "CozyCraft", ShortName = "CozyCraft",
                    LegalStructure = "LLP", IncorporationDate = new DateTime(2020, 1, 10),
                    RegistrationNumber = "AAB-7654", PANNumber = "AAFCC9012P", GSTIN = "36AAFCC9012P1ZT", IsGSTRegistered = true, TANNumber = "HYDC90123D",
                    RegistrationCountryId = india.Id, RegistrationCountryName = india.CountryName,
                    RegistrationStateProvinceId = telangana.Id, RegistrationStateProvinceName = telangana.StateProvinceName,
                    RegistrationCityId = hyderabad.Id, RegistrationCityName = hyderabad.CityName,
                    AddressLine1 = "Floor 3, Cyber Towers", AddressLine2 = "HITEC City",
                    CountryId = india.Id, CountryName = india.CountryName,
                    StateProvinceId = telangana.Id, StateProvinceName = telangana.StateProvinceName,
                    CityId = hyderabad.Id, CityName = hyderabad.CityName, PostalCode = "500081",
                    PrimaryContactName = "Venkat Reddy", PrimaryEmail = "finance@cozycraft.in", PrimaryPhone = "+91-40-40009876", WebsiteUrl = "https://www.cozycraft.in",
                    BaseCurrencyId = inr.Id, BaseCurrencyName = CurDisplay(inr),
                    FiscalYearStartMonth = 4, BooksStartDate = new DateTime(2022, 4, 1), EnableMultiCurrency = true,
                    RoundingPrecision = 2, RoundingMode = "Round Half Up", Status = "Active", IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-365), CreatedBy = "System"
                },

                // 9 - PremiumSeating (Singapore)
                new CompanyModel
                {
                    Id = MasterDataIds.Companies.PremiumSeating, CompanyCode = "PREM01",
                    LegalName = "PremiumSeating International Pte Ltd", TradeName = "PremiumSeating", ShortName = "PremiumSeating",
                    LegalStructure = "Private Limited", IncorporationDate = new DateTime(2017, 8, 5),
                    RegistrationNumber = "201720458K", PANNumber = "", GSTIN = "", IsGSTRegistered = false, TANNumber = "",
                    RegistrationCountryId = sg.Id, RegistrationCountryName = sg.CountryName,
                    RegistrationStateProvinceId = sgState.Id, RegistrationStateProvinceName = sgState.StateProvinceName,
                    RegistrationCityId = sgCity.Id, RegistrationCityName = sgCity.CityName,
                    AddressLine1 = "1 Raffles Place", AddressLine2 = "#20-01 One Raffles Place",
                    CountryId = sg.Id, CountryName = sg.CountryName,
                    StateProvinceId = sgState.Id, StateProvinceName = sgState.StateProvinceName,
                    CityId = sgCity.Id, CityName = sgCity.CityName, PostalCode = "048616",
                    PrimaryContactName = "David Tan", PrimaryEmail = "finance@premiumseating.sg", PrimaryPhone = "+65-6225-1234", WebsiteUrl = "https://www.premiumseating.sg",
                    BaseCurrencyId = sgd.Id, BaseCurrencyName = CurDisplay(sgd),
                    FiscalYearStartMonth = 1, BooksStartDate = new DateTime(2023, 1, 1), EnableMultiCurrency = true,
                    RoundingPrecision = 2, RoundingMode = "Round Half Up", Status = "Active", IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-365), CreatedBy = "System"
                },

                // 10 - CloudSofa (India - Kolkata)
                new CompanyModel
                {
                    Id = MasterDataIds.Companies.CloudSofa, CompanyCode = "CLOUD01",
                    LegalName = "CloudSofa Designs Private Limited", TradeName = "CloudSofa", ShortName = "CloudSofa",
                    LegalStructure = "Private Limited", IncorporationDate = new DateTime(2021, 3, 18),
                    RegistrationNumber = "U36999WB2021PTC556677", PANNumber = "AADCC1234Q", GSTIN = "19AADCC1234Q1Z2", IsGSTRegistered = true, TANNumber = "CALC12345E",
                    RegistrationCountryId = india.Id, RegistrationCountryName = india.CountryName,
                    RegistrationStateProvinceId = westBengal.Id, RegistrationStateProvinceName = westBengal.StateProvinceName,
                    RegistrationCityId = kolkata.Id, RegistrationCityName = kolkata.CityName,
                    AddressLine1 = "Salt Lake Sector V", AddressLine2 = "Block EP, Plot 7",
                    CountryId = india.Id, CountryName = india.CountryName,
                    StateProvinceId = westBengal.Id, StateProvinceName = westBengal.StateProvinceName,
                    CityId = kolkata.Id, CityName = kolkata.CityName, PostalCode = "700091",
                    PrimaryContactName = "Amit Banerjee", PrimaryEmail = "accounts@cloudsofa.in", PrimaryPhone = "+91-33-40007890", WebsiteUrl = "https://www.cloudsofa.in",
                    BaseCurrencyId = inr.Id, BaseCurrencyName = CurDisplay(inr),
                    FiscalYearStartMonth = 4, BooksStartDate = new DateTime(2023, 4, 1), EnableMultiCurrency = false,
                    RoundingPrecision = 2, RoundingMode = "Round Half Up", Status = "Active", IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-365), CreatedBy = "System"
                },

                // 11 - EliteLoungers (UAE - Abu Dhabi)
                new CompanyModel
                {
                    Id = MasterDataIds.Companies.EliteLoungers, CompanyCode = "ELITE01",
                    LegalName = "EliteLoungers Manufacturing LLC", TradeName = "EliteLoungers", ShortName = "EliteLoungers",
                    LegalStructure = "LLC", IncorporationDate = new DateTime(2016, 11, 20),
                    RegistrationNumber = "AD-LLC-2016-04521", PANNumber = "", GSTIN = "", IsGSTRegistered = false, TANNumber = "",
                    RegistrationCountryId = uae.Id, RegistrationCountryName = uae.CountryName,
                    RegistrationStateProvinceId = abuDhabiSt.Id, RegistrationStateProvinceName = abuDhabiSt.StateProvinceName,
                    RegistrationCityId = abuDhCity.Id, RegistrationCityName = abuDhCity.CityName,
                    AddressLine1 = "Abu Dhabi Industrial City", AddressLine2 = "Sector W4, Plot 125",
                    CountryId = uae.Id, CountryName = uae.CountryName,
                    StateProvinceId = abuDhabiSt.Id, StateProvinceName = abuDhabiSt.StateProvinceName,
                    CityId = abuDhCity.Id, CityName = abuDhCity.CityName, PostalCode = "00000",
                    PrimaryContactName = "Omar Al-Hassan", PrimaryEmail = "finance@eliteloungers.ae", PrimaryPhone = "+971-2-5556789", WebsiteUrl = "https://www.eliteloungers.ae",
                    BaseCurrencyId = aed.Id, BaseCurrencyName = CurDisplay(aed),
                    FiscalYearStartMonth = 1, BooksStartDate = new DateTime(2023, 1, 1), EnableMultiCurrency = true,
                    RoundingPrecision = 2, RoundingMode = "Round Half Up", Status = "Active", IsActive = true, IsDeleted = false, CreatedAt = DateTime.Now.AddDays(-365), CreatedBy = "System"
                },
            };
        }
    }
}
