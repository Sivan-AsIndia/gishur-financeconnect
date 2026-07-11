using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    public static class BankAccountServiceData
    {
        public static List<BankAccountModel> Get()
        {
            return new List<BankAccountModel>
            {
                new BankAccountModel
                {
                    Id = Guid.NewGuid(),
                    CompanyId = Guid.NewGuid(),
                    BranchId = MasterDataIds.Branches.SofaCraftHQ,
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftHQ),

                    BankAccountCode = "HDFC-CHN-001",
                    BankAccountName = "HDFC Bank – Main Account",
                    Description = "Primary bank account",

                    BankName = "HDFC Bank",
                    IFSCCode = "HDFC000001",
                    AccountHolderName = "FINANCE CONNECT PVT LTD",

                    BankAccountNumberEncrypted = "ENCRYPTED",
                    BankAccountNumberLast4 = "1234",

                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    BankAccountType = "Current",

                    IsOverdraftAllowed = false,
                    OverdraftLimitAmount = null,
                    IsLockedForTransactions = false,
                    IsBlocked = false,

                    BankGLAccountCode = "10101-BANK",
                    ClearingGLAccountCode = "20101-CLEAR",

                    BankAccountStatus = "Active",
                    CreatedAt = DateTime.Now.AddDays(-30)
                },

                new BankAccountModel
                {
                    Id = Guid.NewGuid(),
                    CompanyId = Guid.NewGuid(),
                    BranchId = MasterDataIds.Branches.SofaCraftBengaluru,
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),

                    BankAccountCode = "SBI-BLR-001",
                    BankAccountName = "SBI – Operations Account",
                    Description = "Operations bank account",

                    BankName = "SBI",
                    BankBranchName = "Bangalore",
                    IFSCCode = "SBIN000002",
                    AccountHolderName = "FINANCE CONNECT PVT LTD",

                    BankAccountNumberEncrypted = "ENCRYPTED",
                    BankAccountNumberLast4 = "5678",

                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    BankAccountType = "Current",

                    IsOverdraftAllowed = false,
                    OverdraftLimitAmount = null,
                    IsLockedForTransactions = false,
                    IsBlocked = false,

                    BankGLAccountCode = "10102-BANK",
                    ClearingGLAccountCode = "20102-CLEAR",

                    BankAccountStatus = "Active",
                    CreatedAt = DateTime.Now.AddDays(-20)
                },

                new BankAccountModel
                {
                    Id = Guid.NewGuid(),
                    CompanyId = Guid.NewGuid(),
                    BranchId = MasterDataIds.Branches.SofaCraftDubai,
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftDubai),

                    BankAccountCode = "SBI-DXB-001",
                    BankAccountName = "SBI – Operations Account",
                    Description = "Operations bank account",

                    BankName = "SBI",
                    BankBranchName = "Dubai",
                    IFSCCode = "SBIN000003",
                    AccountHolderName = "FINANCE CONNECT PVT LTD",

                    BankAccountNumberEncrypted = "ENCRYPTED",
                    BankAccountNumberLast4 = "5678",

                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.AED),
                    BankAccountType = "Current",

                    IsOverdraftAllowed = false,
                    OverdraftLimitAmount = null,
                    IsLockedForTransactions = false,
                    IsBlocked = false,

                    BankGLAccountCode = "10103-BANK",
                    ClearingGLAccountCode = "20103-CLEAR",

                    BankAccountStatus = "Active",
                    CreatedAt = DateTime.Now.AddDays(-20)
                },

                new BankAccountModel
                {
                    Id = Guid.NewGuid(),
                    CompanyId = Guid.NewGuid(),
                    BranchId = MasterDataIds.Branches.SofaCraftUSA_SFO,
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftUSA_SFO),

                    BankAccountCode = "SBI-SF-001",
                    BankAccountName = "SBI – Operations Account",
                    Description = "Operations bank account",

                    BankName = "SBI",
                    BankBranchName = "San Francisco",
                    IFSCCode = "SBIN000004",
                    AccountHolderName = "FINANCE CONNECT PVT LTD",

                    BankAccountNumberEncrypted = "ENCRYPTED",
                    BankAccountNumberLast4 = "5678",

                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.USD),
                    BankAccountType = "Current",

                    IsOverdraftAllowed = false,
                    OverdraftLimitAmount = null,
                    IsLockedForTransactions = false,
                    IsBlocked = false,

                    BankGLAccountCode = "10104-BANK",
                    ClearingGLAccountCode = "20104-CLEAR",

                    BankAccountStatus = "Active",
                    CreatedAt = DateTime.Now.AddDays(-20)
                },

                new BankAccountModel
                {
                    Id = Guid.NewGuid(),
                    CompanyId = Guid.NewGuid(),
                    BranchId = MasterDataIds.Branches.SofaCraftUSA_DAL,
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftUSA_DAL),

                    BankAccountCode = "SBI-DAL-001",
                    BankAccountName = "SBI – Operations Account",
                    Description = "Operations bank account",

                    BankName = "SBI",
                    BankBranchName = "Dallas",
                    IFSCCode = "SBIN000005",
                    AccountHolderName = "FINANCE CONNECT PVT LTD",

                    BankAccountNumberEncrypted = "ENCRYPTED",
                    BankAccountNumberLast4 = "5678",

                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.USD),
                    BankAccountType = "Current",

                    IsOverdraftAllowed = false,
                    OverdraftLimitAmount = null,
                    IsLockedForTransactions = false,
                    IsBlocked = false,

                    BankGLAccountCode = "10105-BANK",
                    ClearingGLAccountCode = "20105-CLEAR",

                    BankAccountStatus = "Active",
                    CreatedAt = DateTime.Now.AddDays(-20)
                },

                new BankAccountModel
                {
                    Id = Guid.NewGuid(),
                    CompanyId = Guid.NewGuid(),
                    BranchId = MasterDataIds.Branches.SofaCraftHQ,
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftHQ),
                    BankAccountCode = "ICICI-CHN-001",
                    BankAccountName = "ICICI Bank - Salary Account",
                    Description = "Salary disbursement account",
                    BankName = "ICICI Bank",
                    BankBranchName = "Chennai",
                    IFSCCode = "ICIC000006",
                    AccountHolderName = "FINANCE CONNECT PVT LTD",
                    BankAccountNumberEncrypted = "ENCRYPTED",
                    BankAccountNumberLast4 = "3456",
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    BankAccountType = "Current",
                    IsOverdraftAllowed = false,
                    IsLockedForTransactions = false,
                    IsBlocked = false,
                    BankGLAccountCode = "10106-BANK",
                    ClearingGLAccountCode = "20106-CLEAR",
                    BankAccountStatus = "Active",
                    CreatedAt = DateTime.Now.AddDays(-15)
                },

                new BankAccountModel
                {
                    Id = Guid.NewGuid(),
                    CompanyId = Guid.NewGuid(),
                    BranchId = MasterDataIds.Branches.SofaCraftBengaluru,
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    BankAccountCode = "AXIS-BLR-001",
                    BankAccountName = "Axis Bank - Vendor Payments",
                    Description = "Vendor payment processing account",
                    BankName = "Axis Bank",
                    BankBranchName = "Bangalore MG Road",
                    IFSCCode = "UTIB000007",
                    AccountHolderName = "FINANCE CONNECT PVT LTD",
                    BankAccountNumberEncrypted = "ENCRYPTED",
                    BankAccountNumberLast4 = "7890",
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    BankAccountType = "Current",
                    IsOverdraftAllowed = true,
                    OverdraftLimitAmount = 500000,
                    IsLockedForTransactions = false,
                    IsBlocked = false,
                    BankGLAccountCode = "10107-BANK",
                    ClearingGLAccountCode = "20107-CLEAR",
                    BankAccountStatus = "Active",
                    CreatedAt = DateTime.Now.AddDays(-25)
                },

                new BankAccountModel
                {
                    Id = Guid.NewGuid(),
                    CompanyId = Guid.NewGuid(),
                    BranchId = MasterDataIds.Branches.CozyCraftHyderabad,
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.CozyCraftHyderabad),
                    BankAccountCode = "KOTAK-HYD-001",
                    BankAccountName = "Kotak Mahindra - Collections",
                    Description = "Customer collections account",
                    BankName = "Kotak Mahindra Bank",
                    BankBranchName = "Hyderabad",
                    IFSCCode = "KKBK000008",
                    AccountHolderName = "FINANCE CONNECT PVT LTD",
                    BankAccountNumberEncrypted = "ENCRYPTED",
                    BankAccountNumberLast4 = "2345",
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    BankAccountType = "Savings",
                    IsOverdraftAllowed = false,
                    IsLockedForTransactions = false,
                    IsBlocked = false,
                    BankGLAccountCode = "10108-BANK",
                    ClearingGLAccountCode = "20108-CLEAR",
                    BankAccountStatus = "Active",
                    CreatedAt = DateTime.Now.AddDays(-18)
                },

                new BankAccountModel
                {
                    Id = Guid.NewGuid(),
                    CompanyId = Guid.NewGuid(),
                    BranchId = MasterDataIds.Branches.UrbanLoftMumbai,
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.UrbanLoftMumbai),
                    BankAccountCode = "BOB-MUM-001",
                    BankAccountName = "Bank of Baroda - Trade Finance",
                    Description = "Import/export trade finance account",
                    BankName = "Bank of Baroda",
                    BankBranchName = "Mumbai Fort",
                    IFSCCode = "BARB000009",
                    AccountHolderName = "FINANCE CONNECT PVT LTD",
                    BankAccountNumberEncrypted = "ENCRYPTED",
                    BankAccountNumberLast4 = "6789",
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    BankAccountType = "Current",
                    IsOverdraftAllowed = false,
                    IsLockedForTransactions = false,
                    IsBlocked = false,
                    BankGLAccountCode = "10109-BANK",
                    ClearingGLAccountCode = "20109-CLEAR",
                    BankAccountStatus = "Inactive",
                    CreatedAt = DateTime.Now.AddDays(-45)
                },

                new BankAccountModel
                {
                    Id = Guid.NewGuid(),
                    CompanyId = Guid.NewGuid(),
                    BranchId = MasterDataIds.Branches.PlushComfortDelhi,
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.PlushComfortDelhi),
                    BankAccountCode = "PNB-DEL-001",
                    BankAccountName = "PNB - Fixed Deposit Linked",
                    Description = "FD linked operations account",
                    BankName = "Punjab National Bank",
                    BankBranchName = "New Delhi Connaught Place",
                    IFSCCode = "PUNB000010",
                    AccountHolderName = "FINANCE CONNECT PVT LTD",
                    BankAccountNumberEncrypted = "ENCRYPTED",
                    BankAccountNumberLast4 = "0123",
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    BankAccountType = "Current",
                    IsOverdraftAllowed = true,
                    OverdraftLimitAmount = 300000,
                    IsLockedForTransactions = false,
                    IsBlocked = false,
                    BankGLAccountCode = "10110-BANK",
                    ClearingGLAccountCode = "20110-CLEAR",
                    BankAccountStatus = "Active",
                    CreatedAt = DateTime.Now.AddDays(-35)
                },

                new BankAccountModel
                {
                    Id = Guid.NewGuid(),
                    CompanyId = Guid.NewGuid(),
                    BranchId = MasterDataIds.Branches.VelvetRestPune,
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.VelvetRestPune),
                    BankAccountCode = "HDFC-PUN-001",
                    BankAccountName = "HDFC Bank - Escrow Account",
                    Description = "Escrow for project payments",
                    BankName = "HDFC Bank",
                    BankBranchName = "Pune Hinjewadi",
                    IFSCCode = "HDFC000011",
                    AccountHolderName = "FINANCE CONNECT PVT LTD",
                    BankAccountNumberEncrypted = "ENCRYPTED",
                    BankAccountNumberLast4 = "4567",
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    BankAccountType = "Current",
                    IsOverdraftAllowed = false,
                    IsLockedForTransactions = true,
                    IsBlocked = false,
                    BankGLAccountCode = "10111-BANK",
                    ClearingGLAccountCode = "20111-CLEAR",
                    BankAccountStatus = "Active",
                    CreatedAt = DateTime.Now.AddDays(-12)
                },

                new BankAccountModel
                {
                    Id = Guid.NewGuid(),
                    CompanyId = Guid.NewGuid(),
                    BranchId = MasterDataIds.Branches.PremiumSeatingSG,
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.PremiumSeatingSG),
                    BankAccountCode = "DBS-SG-001",
                    BankAccountName = "DBS Bank - Multi Currency",
                    Description = "Multi-currency trade account",
                    BankName = "DBS Bank",
                    BankBranchName = "Singapore Marina Bay",
                    IFSCCode = "DBSS000012",
                    AccountHolderName = "FINANCE CONNECT PVT LTD",
                    BankAccountNumberEncrypted = "ENCRYPTED",
                    BankAccountNumberLast4 = "8901",
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.SGD),
                    BankAccountType = "Current",
                    IsOverdraftAllowed = false,
                    IsLockedForTransactions = false,
                    IsBlocked = false,
                    BankGLAccountCode = "10112-BANK",
                    ClearingGLAccountCode = "20112-CLEAR",
                    BankAccountStatus = "Active",
                    CreatedAt = DateTime.Now.AddDays(-8)
                }
            };
        }
    }
}
