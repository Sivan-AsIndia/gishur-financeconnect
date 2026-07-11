using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;
using System.Xml.Linq;
using static FinanceConnect.Client.Data.MasterDataIds;

namespace FinanceConnect.Client.Services
{
    public class BranchService
    {
        private readonly MasterDataService _masterDataService;
        private static List<BranchModel> _branches = new();
        private readonly List<BranchModel> _seedBranches = new();


        public BranchService(MasterDataService masterDataService)
        {
            _masterDataService = masterDataService;

            _seedBranches = SeedBranches();
            ResetToSeed();
        }

        private static List<T> CloneList<T>(IEnumerable<T> source) where T : class
        {
            var json = System.Text.Json.JsonSerializer.Serialize(source);
            return System.Text.Json.JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }

        /// <summary>Reset vendors to seed data</summary>
        public void ResetToSeed()
        {
            _branches = CloneList(_seedBranches);
        }
        public List<BranchModel> GetAll()
            => _branches
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToList();
        public BranchModel? GetById(Guid id)
            => _branches.FirstOrDefault(x => x.Id == id);

        public List<BranchModel> GetByCompany(Guid companyId)
        => _branches
        .Where(b => b.CompanyId == companyId && b.Status=="Active")
       .OrderBy(b => b.BranchName)
        .ToList();

        public List<BranchModel> GetByCompanyId(Guid companyId)
            => _branches
            .Where(b => b.CompanyId == companyId && b.Status == "Active")
            .Select(b => new BranchModel
            {
                Id = b.Id,
                BranchName = b.BranchName,          // ✅ correct
                BranchCode = b.BranchCode,          // ✅ correct
                CompanyId = b.CompanyId,
                Status =b.Status,
                IsDefaultBranch = b.IsDefaultBranch
            })
            .OrderBy(b => b.BranchName)
            .ToList();

        public string GetCompanyNameById(Guid companyId)
        {
            return _masterDataService
                    .GetAllCompanies()
                    .FirstOrDefault(c => c.Id == companyId)
                    ?.LegalName ?? "-";
        }
        public string GetCountryNameById(Guid countryId)
        {
            return _masterDataService
                .GetAllCountries()
                .FirstOrDefault(c => c.Id == countryId)
                ?.CountryName ?? "-";
        }

        public string GetStateNameById(Guid stateId)
        {
            return _masterDataService
                .GetAllStateProvinces()
                .FirstOrDefault(s => s.Id == stateId)
                ?.StateProvinceName ?? "-";
        }

        public string GetCityNameById(Guid cityId)
        {
            return _masterDataService
                .GetAllCities()
                .FirstOrDefault(c => c.Id == cityId)
                ?.CityName ?? "-";
        }

        public string GetTZNameById(Guid? tzId)
        {
            if (tzId == null) return "-";

            return _masterDataService
                .GetAllTimeZones()
                .FirstOrDefault(t => t.Id == tzId)
                ?.DisplayName ?? "-";
        }

        public void Create(BranchModel branch)
        {
            // --------- Ensure single default branch per company ------------
            if (branch.IsDefaultBranch)
            {
                UnsetOtherDefaults(branch.Company, branch.Id);
            }
            branch.CreatedAt = DateTime.UtcNow;
            _branches.Add(branch);
        }

        public void Update(BranchModel branch)
        {
            var existing = GetById(branch.Id);
            if (existing == null)
                return;

            if (branch.IsDefaultBranch)
            {
                UnsetOtherDefaults(branch.Company, branch.Id);
            }
            branch.UpdatedAt = DateTime.UtcNow;
            _branches.Remove(existing);
            _branches.Add(branch);
        }

        public void Delete(Guid id)
        {
            var branch = _branches.FirstOrDefault(x => x.Id == id);
            if (branch != null)
                _branches.Remove(branch);
        }


        public void ActivateBranch(Guid id)
        {
            var branch = _branches.FirstOrDefault(b => b.Id == id);
            if (branch != null)
            {
                branch.Status = "Active";
                branch.UpdatedAt = DateTime.Now;
            }
        }

        public bool CanDeactivateBranch(Guid id)
        {
            // In a real app, check if Branch has active transactions
            return true;
        }

        public void DeactivateBranch(Guid id)
        {
            var branch = _branches.FirstOrDefault(b => b.Id == id);
            if (branch != null)
            {
                branch.Status = "Inactive";
                branch.UpdatedAt = DateTime.Now;
            }
        }

        public void UnsetOtherDefaults(string company, Guid currentBranchId)
        {
            foreach (var b in _branches
                         .Where(x => x.Company == company && x.Id != currentBranchId))
            {
                b.IsDefaultBranch = false;
            }
        }


        private List<BranchModel> SeedBranches()
        {

            // =====================
            // Seed Branches (Demo)
            // Company IDs match MasterDataService seed companies.
            // =====================

            // SofaCraft Furnishings Pvt Ltd (India)
            var sofaCompanyId = MasterDataIds.Companies.SofaCraft;
            
            const string sofaCompanyName = "SofaCraft Furnishings Pvt Ltd";
            var IndiaId = MasterDataIds.Countries.India;
            var USId = MasterDataIds.Countries.UnitedStates;
            var UAEId = MasterDataIds.Countries.UAE;
            var SId = MasterDataIds.Countries.UnitedKingdom;

            var IndiaTZ = MasterDataIds.TimeZones.AsiaKolkata;
            var USTZ = MasterDataIds.TimeZones.AmericaLosAngeles;
            var UAETZ = MasterDataIds.TimeZones.AsiaDubai;
            var STZ = MasterDataIds.TimeZones.AsiaKualaLumpur;

            _branches.AddRange(new List<BranchModel>
    {
        new()
        {
            Id = MasterDataIds.Branches.SofaCraftHQ,
            BranchCode = "HO",
            BranchName = "SofaCraft Head Office & Factory - Chennai",
            Company = sofaCompanyName,
            CompanyId = sofaCompanyId,
            BranchType = "Head Office",
            AddressLine1 = "Plot 42, SIDCO Industrial Estate, Ambattur",
            CountryId = IndiaId,
            StateId = MasterDataIds.States.TamilNadu,
            CityId = MasterDataIds.Cities.Chennai,
            PostalCode = "600098",
            TimeZoneId = IndiaTZ,
            Status = "Active",
            IsDefaultBranch = true,
            BranchColorTag = "Blue",
            ReportSortOrder = 1,
            BranchManagerName = "Karthik Raman",
            BranchEmail = "chennai@sofacraft.in",
            BranchPhone = "+91-44-40001234"
        },
        new()
        {
            Id = MasterDataIds.Branches.SofaCraftBengaluru,
            BranchCode = "BLR",
            BranchName = "SofaCraft Experience Store - Bengaluru",
            Company = sofaCompanyName,
            CompanyId = sofaCompanyId,
            BranchType = "Retail Outlet",
            AddressLine1 = "88, MG Road",
            CountryId = IndiaId,
            StateId = MasterDataIds.States.Karnataka,
            CityId = MasterDataIds.Cities.Bengaluru,
            PostalCode = "560001",
            TimeZoneId = IndiaTZ,
            Status = "Active",
            BranchColorTag = "Purple",
            ReportSortOrder = 2,
            BranchManagerName = "Sanjay Gupta",
            BranchEmail = "bengaluru@sofacraft.in",
            BranchPhone = "+91-80-40009876",
            OperatingHoursNote = "Mon–Sun 10:00 AM – 9:00 PM"
        },
        new()
        {
            Id = MasterDataIds.Branches.SofaCraftDubai,
            BranchCode = "DXB",
            BranchName = "SofaCraft Sales Office - Dubai",
            Company = sofaCompanyName,
            CompanyId = sofaCompanyId,
            BranchType = "Regional Office",
            AddressLine1 = "Business Bay, Downtown Dubai",
            CountryId = UAEId,
            StateId = MasterDataIds.States.Dubai,
            CityId = MasterDataIds.Cities.DubaiCity,
            PostalCode = "00000",
            TimeZoneId = UAETZ,
            Status = "Active",
            BranchColorTag = "Teal",
            ReportSortOrder = 3,
            BranchManagerName = "Adeel Khan",
            BranchEmail = "dubai@sofacraft.in",
            BranchPhone = "+971-4-5550123"
        },

        // SofaCraft Retail USA Inc. (USA)
        new()
        {
            Id = MasterDataIds.Branches.SofaCraftUSA_SFO,
            BranchCode = "SFO",
            BranchName = "SofaCraft Retail HQ - San Francisco",
            Company = "SofaCraft Retail USA Inc.",
            CompanyId = MasterDataIds.Companies.SofaCraftUSA,
            BranchType = "Head Office",
            AddressLine1 = "120 Market Street, Suite 550",
            CountryId = USId,
            StateId = MasterDataIds.States.California,
            CityId = MasterDataIds.Cities.SanFrancisco,
            PostalCode = "94105",
            TimeZoneId = USTZ,
            Status = "Active",
            IsDefaultBranch = true,
            BranchColorTag = "Indigo",
            ReportSortOrder = 1,
            BranchManagerName = "Sarah Williams",
            BranchEmail = "sf@sofacraftusa.com",
            BranchPhone = "+1-415-555-0199"
        },
        new()
        {
            Id = MasterDataIds.Branches.SofaCraftUSA_DAL,
            BranchCode = "DAL",
            BranchName = "SofaCraft Retail Warehouse - Dallas",
            Company = "SofaCraft Retail USA Inc.",
            CompanyId = MasterDataIds.Companies.SofaCraftUSA,
            BranchType = "Warehouse",
            AddressLine1 = "2100 Trinity Mills Rd",
            CountryId = USId,
            StateId = MasterDataIds.States.Texas,
            CityId = MasterDataIds.Cities.Dallas,
            PostalCode = "75247",
            TimeZoneId = USTZ,
            Status = "Active",
            BranchColorTag = "Green",
            ReportSortOrder = 2,
            BranchManagerName = "Daniel Garcia",
            BranchEmail = "dallas@sofacraftusa.com",
            BranchPhone = "+1-214-555-0127"
        },

        // OakNest Interiors LLP (India)
        new()
        {
            Id = MasterDataIds.Branches.OakNestBengaluru,
            BranchCode = "BLR-HO",
            BranchName = "OakNest Studio - Bengaluru",
            Company = "OakNest Interiors LLP",
            CompanyId = MasterDataIds.Companies.OakNest,
            BranchType = "Head Office",
            AddressLine1 = "4th Floor, Indiranagar, 100 Feet Road",
            CountryId = IndiaId,
            StateId = MasterDataIds.States.Karnataka,
            CityId = MasterDataIds.Cities.Bengaluru,
            PostalCode = "560038",
            TimeZoneId = IndiaTZ,
            Status = "Active",
            IsDefaultBranch = true,
            BranchColorTag = "Brown",
            ReportSortOrder = 1,
            BranchManagerName = "Ritu Menon",
            BranchEmail = "blr@oaknest.in",
            BranchPhone = "+91-80-41234567"
        },

        // UrbanLoft Home Décor Pvt Ltd (India)
        new()
        {
            Id = MasterDataIds.Branches.UrbanLoftMumbai,
            BranchCode = "BOM-HO",
            BranchName = "UrbanLoft HQ - Mumbai",
            Company = "UrbanLoft Home Décor Pvt Ltd",
            CompanyId = MasterDataIds.Companies.UrbanLoft,
            BranchType = "Head Office",
            AddressLine1 = "BKC, Block C, 8th Floor",
            CountryId = IndiaId,
            StateId = MasterDataIds.States.Maharashtra,
            CityId = MasterDataIds.Cities.Mumbai,
            PostalCode = "400051",
            TimeZoneId = IndiaTZ,
            Status = "Active",
            IsDefaultBranch = true,
            BranchColorTag = "Orange",
            ReportSortOrder = 1,
            BranchManagerName = "Neha Shah",
            BranchEmail = "mumbai@urbanloft.in",
            BranchPhone = "+91-22-49887766"
        },

        // DesertDune Furniture Trading LLC (UAE)
        new()
        {
            Id = MasterDataIds.Branches.DesertDuneDubai,
            BranchCode = "DXB-HO",
            BranchName = "DesertDune HQ - Dubai",
            Company = "DesertDune Furniture Trading LLC",
            CompanyId = MasterDataIds.Companies.DesertDune,
            BranchType = "Head Office",
            AddressLine1 = "Business Bay, Bay Square, Building 6",
            CountryId = UAEId,
            StateId = MasterDataIds.States.Dubai,
            CityId = MasterDataIds.Cities.DubaiCity,
            PostalCode = "00000",
            TimeZoneId = IndiaTZ,
            Status = "Active",
            IsDefaultBranch = true,
            BranchColorTag = "Teal",
            ReportSortOrder = 1,
            BranchManagerName = "Adeel Khan",
            BranchEmail = "dubai@desertdune.ae",
            BranchPhone = "+971-4-5550123"
        },

        // ================= NEW SOFA COMPANY BRANCHES =================

        // PlushComfort Sofas Pvt Ltd (Delhi, India)
        new()
        {
            Id = MasterDataIds.Branches.PlushComfortDelhi,
            BranchCode = "DEL-HO",
            BranchName = "PlushComfort Head Office - Delhi",
            Company = "PlushComfort Sofas Private Limited",
            CompanyId = MasterDataIds.Companies.PlushComfort,
            BranchType = "Head Office",
            AddressLine1 = "Plot 12, Okhla Industrial Area, Phase II",
            CountryId = IndiaId,
            StateId = MasterDataIds.States.Delhi,
            CityId = MasterDataIds.Cities.NewDelhi,
            PostalCode = "110020",
            TimeZoneId = IndiaTZ,
            Status = "Active",
            IsDefaultBranch = true,
            BranchColorTag = "Red",
            ReportSortOrder = 1,
            BranchManagerName = "Rajesh Kumar",
            BranchEmail = "delhi@plushcomfort.in",
            BranchPhone = "+91-11-40001234",
            OperatingHoursNote = "Mon–Sat 9:00 AM – 6:00 PM"
        },
        new()
        {
            Id = MasterDataIds.Branches.PlushComfortMH,
            BranchCode = "GGN-FAC",
            BranchName = "PlushComfort Factory - MH",
            Company = "PlushComfort Sofas Private Limited",
            CompanyId = MasterDataIds.Companies.PlushComfort,
            BranchType = "Factory",
            AddressLine1 = "Plot 88, IMT Manesar",
            CountryId = IndiaId,
            StateId = MasterDataIds.States.Maharashtra,
            CityId = MasterDataIds.Cities.Mumbai,
            PostalCode = "122051",
            TimeZoneId = IndiaTZ,
            Status = "Active",
            BranchColorTag = "Orange",
            ReportSortOrder = 2,
            BranchManagerName = "Suresh Yadav",
            BranchEmail = "factory@plushcomfort.in",
            BranchPhone = "+91-124-4005678",
            OperatingHoursNote = "Mon–Sat 8:00 AM – 8:00 PM"
        },

        // VelvetRest Furniture Industries Pvt Ltd (Pune, India)
        new()
        {
            Id = MasterDataIds.Branches.VelvetRestPune,
            BranchCode = "PUN-HO",
            BranchName = "VelvetRest Head Office - Pune",
            Company = "VelvetRest Furniture Industries Private Limited",
            CompanyId = MasterDataIds.Companies.VelvetRest,
            BranchType = "Head Office",
            AddressLine1 = "Survey No. 45, Hinjewadi IT Park, Phase III",
            CountryId = IndiaId,
            StateId = MasterDataIds.States.Maharashtra,
            CityId = MasterDataIds.Cities.Pune,
            PostalCode = "411057",
            TimeZoneId = IndiaTZ,
            Status = "Active",
            IsDefaultBranch = true,
            BranchColorTag = "Purple",
            ReportSortOrder = 1,
            BranchManagerName = "Priya Sharma",
            BranchEmail = "pune@velvetrest.in",
            BranchPhone = "+91-20-40005678",
            OperatingHoursNote = "Mon–Sat 9:00 AM – 6:00 PM"
        },
        new()
        {
            Id = MasterDataIds.Branches.VelvetRestMumbai,
            BranchCode = "BOM-SH",
            BranchName = "VelvetRest Showroom - Mumbai",
            Company = "VelvetRest Furniture Industries Private Limited",
            CompanyId = MasterDataIds.Companies.VelvetRest,
            BranchType = "Retail Outlet",
            AddressLine1 = "Ground Floor, Linking Road",
            CountryId = IndiaId,
            StateId = MasterDataIds.States.Maharashtra,
            CityId = MasterDataIds.Cities.Mumbai,
            PostalCode = "400050",
            TimeZoneId = IndiaTZ,
            Status = "Active",
            BranchColorTag = "Pink",
            ReportSortOrder = 2,
            BranchManagerName = "Deepak Patil",
            BranchEmail = "mumbai@velvetrest.in",
            BranchPhone = "+91-22-40009876",
            OperatingHoursNote = "Mon–Sun 10:00 AM – 9:00 PM"
        },

        // CozyCraft Living Solutions LLP (Hyderabad, India)
        new()
        {
            Id = MasterDataIds.Branches.CozyCraftHyderabad,
            BranchCode = "HYD-HO",
            BranchName = "CozyCraft Head Office - Hyderabad",
            Company = "CozyCraft Living Solutions LLP",
            CompanyId = MasterDataIds.Companies.CozyCraft,
            BranchType = "Head Office",
            AddressLine1 = "Floor 3, Cyber Towers, HITEC City",
            CountryId = IndiaId,
            StateId = MasterDataIds.States.Telangana,
            CityId = MasterDataIds.Cities.Hyderabad,
            PostalCode = "500081",
            TimeZoneId = IndiaTZ,
            Status = "Active",
            IsDefaultBranch = true,
            BranchColorTag = "Cyan",
            ReportSortOrder = 1,
            BranchManagerName = "Venkat Reddy",
            BranchEmail = "hyderabad@cozycraft.in",
            BranchPhone = "+91-40-40009876",
            OperatingHoursNote = "Mon–Sat 9:00 AM – 6:00 PM"
        },
        new()
        {
            Id = MasterDataIds.Branches.CozyCraftWarehouse,
            BranchCode = "SEC-EXP",
            BranchName = "CozyCraft Experience Center - Secunderabad",
            Company = "CozyCraft Living Solutions LLP",
            CompanyId = MasterDataIds.Companies.CozyCraft,
            BranchType = "Experience Center",
            AddressLine1 = "Rashtrapati Road, Near Clock Tower",
            CountryId = IndiaId,
            StateId = MasterDataIds.States.Telangana,
            CityId = MasterDataIds.Cities.Hyderabad,
            PostalCode = "500003",
            TimeZoneId = IndiaTZ,
            Status = "Active",
            BranchColorTag = "Blue",
            ReportSortOrder = 2,
            BranchManagerName = "Lakshmi Narayana",
            BranchEmail = "experience@cozycraft.in",
            BranchPhone = "+91-40-40005432",
            OperatingHoursNote = "Mon–Sun 10:00 AM – 8:00 PM"
        },

        // PremiumSeating International Pte Ltd (Singapore)
        new()
        {
            Id = MasterDataIds.Branches.PremiumSeatingSG,
            BranchCode = "SIN-HQ",
            BranchName = "PremiumSeating HQ - Singapore",
            Company = "PremiumSeating International Pte Ltd",
            CompanyId = MasterDataIds.Companies.PremiumSeating,
            BranchType = "Head Office",
            AddressLine1 = "1 Raffles Place, #20-01 One Raffles Place",
            CountryId = SId,
            StateId = MasterDataIds.States.Telangana,
            CityId = MasterDataIds.Cities.SingaporeCity,
            PostalCode = "048616",
            TimeZoneId = STZ,
            Status = "Active",
            IsDefaultBranch = true,
            BranchColorTag = "Gold",
            ReportSortOrder = 1,
            BranchManagerName = "David Tan",
            BranchEmail = "hq@premiumseating.sg",
            BranchPhone = "+65-6225-1234",
            OperatingHoursNote = "Mon–Fri 9:00 AM – 6:00 PM"
        },
        new()
        {
            Id = MasterDataIds.Branches.PremiumSeatingWH,
            BranchCode = "JUR-FAC",
            BranchName = "PremiumSeating Factory - Jurong",
            Company = "PremiumSeating International Pte Ltd",
            CompanyId = MasterDataIds.Companies.PremiumSeating,
            BranchType = "Factory",
            AddressLine1 = "25 International Business Park",
            CountryId = SId,
            StateId = MasterDataIds.States.Telangana,
            CityId = MasterDataIds.Cities.SingaporeCity,
            PostalCode = "609916",
            TimeZoneId = STZ,
            Status = "Active",
            BranchColorTag = "Silver",
            ReportSortOrder = 2,
            BranchManagerName = "Michael Lim",
            BranchEmail = "factory@premiumseating.sg",
            BranchPhone = "+65-6560-5678",
            OperatingHoursNote = "Mon–Sat 8:00 AM – 6:00 PM"
        },

        // CloudSofa Designs Pvt Ltd (Kolkata, India)
        new()
        {
            Id = MasterDataIds.Branches.CloudSofaKolkata,
            BranchCode = "CCU-HO",
            BranchName = "CloudSofa Head Office - Kolkata",
            Company = "CloudSofa Designs Private Limited",
            CompanyId = MasterDataIds.Companies.CloudSofa,
            BranchType = "Head Office",
            AddressLine1 = "Salt Lake Sector V, Block EP, Plot 7",
            CountryId = IndiaId,
            StateId = MasterDataIds.States.WestBengal,
            CityId = MasterDataIds.Cities.Kolkata,
            PostalCode = "700091",
            TimeZoneId = IndiaTZ,
            Status = "Active",
            IsDefaultBranch = true,
            BranchColorTag = "Sky Blue",
            ReportSortOrder = 1,
            BranchManagerName = "Amit Banerjee",
            BranchEmail = "kolkata@cloudsofa.in",
            BranchPhone = "+91-33-40007890",
            OperatingHoursNote = "Mon–Sat 9:00 AM – 6:00 PM"
        },
        new()
        {
            Id = MasterDataIds.Branches.CloudSofaWarehouse,
            BranchCode = "HWH-FAC",
            BranchName = "CloudSofa Factory - Howrah",
            Company = "CloudSofa Designs Private Limited",
            CompanyId = MasterDataIds.Companies.CloudSofa,
            BranchType = "Factory",
            AddressLine1 = "Shalimar Industrial Complex",
            CountryId = IndiaId,
            StateId = MasterDataIds.States.WestBengal,
            CityId = MasterDataIds.Cities.Kolkata,
            PostalCode = "711104",
            TimeZoneId = IndiaTZ,
            Status = "Active",
            BranchColorTag = "Navy",
            ReportSortOrder = 2,
            BranchManagerName = "Bikash Das",
            BranchEmail = "factory@cloudsofa.in",
            BranchPhone = "+91-33-40005432",
            OperatingHoursNote = "Mon–Sat 7:00 AM – 7:00 PM"
        },

        // EliteLoungers Manufacturing LLC (Abu Dhabi, UAE)
        new()
        {
            Id = MasterDataIds.Branches.EliteLoungerAbuDhabi,
            BranchCode = "AUH-HQ",
            BranchName = "EliteLoungers HQ - Abu Dhabi",
            Company = "EliteLoungers Manufacturing LLC",
            CompanyId = MasterDataIds.Companies.EliteLoungers,
            BranchType = "Head Office",
            AddressLine1 = "Abu Dhabi Industrial City, Sector W4, Plot 125",
            CountryId = UAEId,
            StateId = MasterDataIds.States.AbuDhabi,
            CityId = MasterDataIds.Cities.AbuDhabiCity,
            PostalCode = "00000",
            TimeZoneId = UAETZ,
            Status = "Active",
            IsDefaultBranch = true,
            BranchColorTag = "Maroon",
            ReportSortOrder = 1,
            BranchManagerName = "Omar Al-Hassan",
            BranchEmail = "hq@eliteloungers.ae",
            BranchPhone = "+971-2-5556789",
            OperatingHoursNote = "Sun–Thu 8:00 AM – 5:00 PM"
        },
        new()
        {
            Id = MasterDataIds.Branches.EliteLoungerDubai,
            BranchCode = "DXB-SH",
            BranchName = "EliteLoungers Showroom - Dubai",
            Company = "EliteLoungers Manufacturing LLC",
            CompanyId = MasterDataIds.Companies.EliteLoungers,
            BranchType = "Retail Outlet",
            AddressLine1 = "Mall of the Emirates, Level 2",
            CountryId = UAEId,
            StateId = MasterDataIds.States.Dubai,
            CityId = MasterDataIds.Cities.DubaiCity,
            PostalCode = "00000",
            TimeZoneId = UAETZ,
            Status = "Active",
            BranchColorTag = "Coral",
            ReportSortOrder = 2,
            BranchManagerName = "Fatima Al-Rashid",
            BranchEmail = "showroom@eliteloungers.ae",
            BranchPhone = "+971-4-5559876",
            OperatingHoursNote = "Daily 10:00 AM – 10:00 PM"
        }
    });

            return _branches;
        }

    }
}
