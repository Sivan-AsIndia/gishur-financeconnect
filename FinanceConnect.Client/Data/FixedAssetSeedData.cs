using FinanceConnect.Client.ViewModels;
using static FinanceConnect.Client.ViewModels.FixedAssetViewModel;

namespace FinanceConnect.Client.Data
{
    public class FixedAssetSeedData
    {
      
        public static List<FixedAssetListDto> GetAllFixedAssets()
        {
            return new List<FixedAssetListDto>
            {
                new FixedAssetListDto
                {
                    FixedAssetId     = Guid.Parse("50000000-0000-0000-0000-000000000001"),
                    AssetCode        = "FA-LAP-001",
                    AssetName        = "Dell Latitude Laptop",
                    AssetCategoryId  = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                    CategoryName     = "Laptop Assets",
                    AssetTag         = "TAG-LAP-001",
                    SerialNumber     = "SN123456",
                    CompanyId        = Guid.Parse("60000000-0000-0000-0000-000000000001"),
                    BranchId         = Guid.Parse("70000000-0000-0000-0000-000000000001"),
                    BranchName       = "SofaCraft Retail HQ - San Francisco",
                    PurchaseDate     = new DateTime(2023, 6, 12),
                    PurchaseCost     = 65000,
                    SalvageValue     = 5000,
                    UsefulLifeMonths = 36,
                    VendorId         = Guid.Parse("80000000-0000-0000-0000-000000000001"),
                    VendorName       = "Dell India Pvt Ltd",
                    Location         = "Head Office - IT Room",
                    Custodian        = "IT Department",
                    IsDepreciable    = true,
                    AssetStatus      = AssetStatus.Active,
                    Notes            = "Issued to development team"
                },

                new FixedAssetListDto
                {
                    FixedAssetId     = Guid.Parse("50000000-0000-0000-0000-000000000002"),
                    AssetCode        = "FA-DESK-001",
                    AssetName        = "HP Desktop System",
                    AssetCategoryId  = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                    CategoryName     = "Desktop Computers",
                    AssetTag         = "TAG-DESK-001",
                    SerialNumber     = "SN789456",
                    CompanyId        = Guid.Parse("60000000-0000-0000-0000-000000000001"),
                    BranchId         = Guid.Parse("70000000-0000-0000-0000-000000000002"),
                    BranchName       = "SofaCraft Retail Warehouse - Dallas",
                    PurchaseDate     = new DateTime(2022, 3, 5),
                    PurchaseCost     = 48000,
                    SalvageValue     = 4000,
                    UsefulLifeMonths = 48,
                    VendorId         = Guid.Parse("80000000-0000-0000-0000-000000000002"),
                    VendorName       = "HP Enterprise Solutions",
                    Location         = "Branch Office - Admin",
                    Custodian        = "Admin Department",
                    IsDepreciable    = true,
                    AssetStatus      = AssetStatus.Active,
                    Notes            = "Used for billing operations"
                },

                new FixedAssetListDto
                {
                    FixedAssetId     = Guid.Parse("50000000-0000-0000-0000-000000000003"),
                    AssetCode        = "FA-FURN-001",
                    AssetName        = "Office Workstation Table",
                    AssetCategoryId  = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                    CategoryName     = "Furniture",
                    AssetTag         = "TAG-FURN-001",
                    SerialNumber     = null,
                    CompanyId        = Guid.Parse("60000000-0000-0000-0000-000000000001"),
                    BranchId         = Guid.Parse("70000000-0000-0000-0000-000000000001"),
                    BranchName       = "OakNest Studio - Bengaluru",
                    PurchaseDate     = new DateTime(2021, 11, 10),
                    PurchaseCost     = 22000,
                    SalvageValue     = 2000,
                    UsefulLifeMonths = 120,
                    VendorId         = Guid.Parse("80000000-0000-0000-0000-000000000003"),
                    VendorName       = "Godrej Interio",
                    Location         = "Head Office - Work Area",
                    Custodian        = "Operations",
                    IsDepreciable    = true,
                    AssetStatus      = AssetStatus.Active,
                    Notes            = "Wooden modular workstation"
                },

                new FixedAssetListDto
                {
                    FixedAssetId     = Guid.Parse("50000000-0000-0000-0000-000000000004"),
                    AssetCode        = "FA-VEH-001",
                    AssetName        = "Toyota Innova Company Car",
                    AssetCategoryId  = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                    CategoryName     = "Vehicles",
                    AssetTag         = "TAG-VEH-001",
                    SerialNumber     = "VIN987654321",
                    CompanyId        = Guid.Parse("60000000-0000-0000-0000-000000000001"),
                    BranchId         = Guid.Parse("70000000-0000-0000-0000-000000000003"),
                    BranchName       = "UrbanLoft HQ - Mumbai",
                    PurchaseDate     = new DateTime(2020, 1, 20),
                    PurchaseCost     = 1450000,
                    SalvageValue     = 200000,
                    UsefulLifeMonths = 96,
                    VendorId         = Guid.Parse("80000000-0000-0000-0000-000000000004"),
                    VendorName       = "Toyota Dealer - Chennai",
                    Location         = "Transport Department",
                    Custodian        = "Logistics Manager",
                    IsDepreciable    = true,
                    AssetStatus      = AssetStatus.Active,
                    Notes            = "Used for official travel"
                },

                new FixedAssetListDto
                {
                    FixedAssetId     = Guid.Parse("50000000-0000-0000-0000-000000000005"),
                    AssetCode        = "FA-SOFT-001",
                    AssetName        = "Accounting Software License",
                    AssetCategoryId  = Guid.Parse("10000000-0000-0000-0000-000000000006"),
                    CategoryName     = "Software",
                    AssetTag         = null,
                    SerialNumber     = null,
                    CompanyId        = Guid.Parse("60000000-0000-0000-0000-000000000001"),
                    BranchId         = Guid.Parse("70000000-0000-0000-0000-000000000001"),
                    BranchName       = "DesertDune HQ - Dubai",
                    PurchaseDate     = new DateTime(2024, 4, 1),
                    PurchaseCost     = 180000,
                    SalvageValue     = 0,
                    UsefulLifeMonths = 60,
                    VendorId         = Guid.Parse("80000000-0000-0000-0000-000000000005"),
                    VendorName       = "Tally Solutions Pvt Ltd",
                    Location         = "Digital Asset",
                    Custodian        = "Finance Department",
                    IsDepreciable    = true,
                    AssetStatus      = AssetStatus.Active,
                    Notes            = "5 user enterprise license"
                },

                new FixedAssetListDto
                {
                    FixedAssetId     = Guid.Parse("50000000-0000-0000-0000-000000000006"),
                    AssetCode        = "FA-LAP-002",
                    AssetName        = "Lenovo ThinkPad Laptop",
                    AssetCategoryId  = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                    CategoryName     = "Laptop Assets",
                    AssetTag         = "TAG-LAP-002",
                    SerialNumber     = "SN654321",
                    CompanyId        = Guid.Parse("60000000-0000-0000-0000-000000000001"),
                    BranchId         = Guid.Parse("70000000-0000-0000-0000-000000000002"),
                    BranchName       = "PlushComfort Head Office - Delhi",
                    PurchaseDate     = new DateTime(2024, 1, 15),
                    PurchaseCost     = 72000,
                    SalvageValue     = 6000,
                    UsefulLifeMonths = 36,
                    VendorId         = Guid.Parse("80000000-0000-0000-0000-000000000001"),
                    VendorName       = "Dell India Pvt Ltd",
                    Location         = "Branch Office - IT",
                    Custodian        = "IT Support",
                    IsDepreciable    = true,
                    AssetStatus      = AssetStatus.Draft,
                    Notes            = "Pending configuration"
                },

                new FixedAssetListDto
                {
                    FixedAssetId     = Guid.Parse("50000000-0000-0000-0000-000000000007"),
                    AssetCode        = "FA-MOB-001",
                    AssetName        = "Samsung Office Mobile",
                    AssetCategoryId  = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                    CategoryName     = "Laptop Assets",
                    AssetTag         = "TAG-MOB-001",
                    SerialNumber     = "IMEI123456789",
                    CompanyId        = Guid.Parse("60000000-0000-0000-0000-000000000001"),
                    BranchId         = Guid.Parse("70000000-0000-0000-0000-000000000003"),
                    BranchName       = "PlushComfort Factory - MH",
                    PurchaseDate     = new DateTime(2023, 9, 10),
                    PurchaseCost     = 28000,
                    SalvageValue     = 2000,
                    UsefulLifeMonths = 24,
                    VendorId         = Guid.Parse("80000000-0000-0000-0000-000000000002"),
                    VendorName       = "HP Enterprise Solutions",
                    Location         = "Sales Department",
                    Custodian        = "Sales Executive",
                    IsDepreciable    = true,
                    AssetStatus      = AssetStatus.Inactive,
                    Notes            = "Temporarily not in use"
                },

                new FixedAssetListDto
                {
                    FixedAssetId     = Guid.Parse("50000000-0000-0000-0000-000000000008"),
                    AssetCode        = "FA-PRN-001",
                    AssetName        = "Canon Office Printer",
                    AssetCategoryId  = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                    CategoryName     = "Furniture",
                    AssetTag         = "TAG-PRN-001",
                    SerialNumber     = "PRN987654",
                    CompanyId        = Guid.Parse("60000000-0000-0000-0000-000000000001"),
                    BranchId         = Guid.Parse("70000000-0000-0000-0000-000000000001"),
                    BranchName       = "VelvetRest Head Office - Pune",
                    PurchaseDate     = new DateTime(2020, 7, 22),
                    PurchaseCost     = 35000,
                    SalvageValue     = 3000,
                    UsefulLifeMonths = 72,
                    VendorId         = Guid.Parse("80000000-0000-0000-0000-000000000003"),
                    VendorName       = "Godrej Interio",
                    Location         = "Admin Block",
                    Custodian        = "Admin Manager",
                    IsDepreciable    = true,
                    AssetStatus      = AssetStatus.Disposed,
                    Notes            = "Disposed due to hardware failure"
                },

                new FixedAssetListDto
                {
                    FixedAssetId     = Guid.Parse("50000000-0000-0000-0000-000000000009"),
                    AssetCode        = "FA-CAM-001",
                    AssetName        = "Sony CCTV Camera",
                    AssetCategoryId  = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                    CategoryName     = "Furniture",
                    AssetTag         = "TAG-CAM-001",
                    SerialNumber     = "CAM123789",
                    CompanyId        = Guid.Parse("60000000-0000-0000-0000-000000000001"),
                    BranchId         = Guid.Parse("70000000-0000-0000-0000-000000000002"),
                    BranchName       = "VelvetRest Showroom - Mumbai",
                    PurchaseDate     = new DateTime(2022, 12, 1),
                    PurchaseCost     = 15000,
                    SalvageValue     = 1000,
                    UsefulLifeMonths = 60,
                    VendorId         = Guid.Parse("80000000-0000-0000-0000-000000000004"),
                    VendorName       = "Toyota Dealer - Chennai",
                    Location         = "Security Room",
                    Custodian        = "Security Team",
                    IsDepreciable    = true,
                    AssetStatus      = AssetStatus.Active,
                    Notes            = "Surveillance monitoring"
                },

                new FixedAssetListDto
                {
                    FixedAssetId     = Guid.Parse("50000000-0000-0000-0000-000000000010"),
                    AssetCode        = "FA-FURN-002",
                    AssetName        = "Conference Meeting Table",
                    AssetCategoryId  = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                    CategoryName     = "Furniture",
                    AssetTag         = "TAG-FURN-002",
                    SerialNumber     = null,
                    CompanyId        = Guid.Parse("60000000-0000-0000-0000-000000000001"),
                    BranchId         = Guid.Parse("70000000-0000-0000-0000-000000000003"),
                    BranchName       = "CozyCraft Head Office - Hyderabad",
                    PurchaseDate     = new DateTime(2021, 5, 18),
                    PurchaseCost     = 54000,
                    SalvageValue     = 5000,
                    UsefulLifeMonths = 120,
                    VendorId         = Guid.Parse("80000000-0000-0000-0000-000000000003"),
                    VendorName       = "Godrej Interio",
                    Location         = "Conference Hall",
                    Custodian        = "Administration",
                    IsDepreciable    = true,
                    AssetStatus      = AssetStatus.Draft,
                    Notes            = "Awaiting approval for capitalization"
                }
            };
        }
    }
}
