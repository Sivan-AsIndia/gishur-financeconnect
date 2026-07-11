using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    public static class CashAccountServiceData
    {

        public static List<CashAccountModels> Get()
        {

            var accounts = new List<CashAccountModels>();
            return new List<CashAccountModels>
{
    new()
    {
        Code = "CA-01",
        Name = "Main Cash Account",
        BranchId = MasterDataIds.Branches.SofaCraftHQ,
        BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftHQ),
        CustodianName = "Cashier A",
        CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
        CashGlAccount = "1001-CASH",
        MaxCashLimit = 50000,
        Status = "Active",
        Description = "Primary cash account",
        IsNegativeBalanceAllowed = false,
        CustodyStartDate = DateTime.Now.AddDays(-10)
    },

    new()
    {
        Code = "CA-02",
        Name = "Petty Cash",
        BranchId = MasterDataIds.Branches.SofaCraftBengaluru,
        BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
        CustodianName = "Cashier B",
        CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
        CashGlAccount = "1002-CASH",
        MaxCashLimit = 20000,
        Status = "Active",
        Description = "Petty cash for office use",
        IsNegativeBalanceAllowed = true,
        CustodyStartDate = DateTime.Now.AddDays(-20)
    },

    new()
    {
        Code = "CA-03",
        Name = "Branch Cash Reserve",
        BranchId = MasterDataIds.Branches.CozyCraftHyderabad,
        BranchName = SeedLookup.BranchName(MasterDataIds.Branches.CozyCraftHyderabad),
        CustodianName = "Cashier C",
        CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
        CashGlAccount = "1003-CASH",
        MaxCashLimit = 75000,
        Status = "Inactive",
        Description = "Reserve fund",
        IsNegativeBalanceAllowed = false,
        CustodyStartDate = DateTime.Now.AddDays(-5)
    },

    new()
    {
        Code = "CA-04",
        Name = "Operations Cash",
        BranchId = MasterDataIds.Branches.UrbanLoftMumbai,
        BranchName = SeedLookup.BranchName(MasterDataIds.Branches.UrbanLoftMumbai),
        CustodianName = "Cashier D",
        CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
        CashGlAccount = "1004-CASH",
        MaxCashLimit = 100000,
        Status = "Active",
        Description = "Daily operations fund",
        IsNegativeBalanceAllowed = false,
        CustodyStartDate = DateTime.Now.AddDays(-30)
    },

    new()
    {
        Code = "CA-05",
        Name = "Emergency Fund",
        BranchId = MasterDataIds.Branches.PlushComfortDelhi,
        BranchName = SeedLookup.BranchName(MasterDataIds.Branches.PlushComfortDelhi),
        CustodianName = "Cashier E",
        CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
        CashGlAccount = "1005-CASH",
        MaxCashLimit = 150000,
        Status = "Active",
        Description = "Emergency use",
        IsNegativeBalanceAllowed = false,
        CustodyStartDate = DateTime.Now.AddDays(-60)
    },

    new()
    {
        Code = "CA-06",
        Name = "Retail Counter Cash",
        BranchId = MasterDataIds.Branches.VelvetRestPune,
        BranchName = SeedLookup.BranchName(MasterDataIds.Branches.VelvetRestPune),
        CustodianName = "Cashier F",
        CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
        CashGlAccount = "1006-CASH",
        MaxCashLimit = 30000,
        Status = "Closed",
        Description = "Retail counter setup",
        IsNegativeBalanceAllowed = true,
        CustodyStartDate = DateTime.Now.AddDays(-2)
    },

    new()
    {
        Code = "CA-07",
        Name = "Temporary Cash Vault",
        BranchId = MasterDataIds.Branches.CozyCraftWarehouse,
        BranchName = SeedLookup.BranchName(MasterDataIds.Branches.CozyCraftWarehouse),
        CustodianName = "Cashier G",
        CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
        CashGlAccount = "1007-CASH",
        MaxCashLimit = 40000,
        Status = "Inactive",
        Description = "Temporary storage",
        IsNegativeBalanceAllowed = false,
        CustodyStartDate = DateTime.Now.AddDays(-15)
    },

    new()
    {
        Code = "CA-08",
        Name = "Project Fund",
        BranchId = MasterDataIds.Branches.PremiumSeatingSG,
        BranchName = SeedLookup.BranchName(MasterDataIds.Branches.PremiumSeatingSG),
        CustodianName = "Cashier H",
        CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.SGD),
        CashGlAccount = "1008-CASH",
        MaxCashLimit = 90000,
        Status = "Active",
        Description = "Special project cash",
        IsNegativeBalanceAllowed = false,
        CustodyStartDate = DateTime.Now.AddDays(-40)
    },

    new()
    {
        Code = "CA-09",
        Name = "Security Deposit Cash",
        BranchId = MasterDataIds.Branches.CloudSofaKolkata,
        BranchName = SeedLookup.BranchName(MasterDataIds.Branches.CloudSofaKolkata),
        CustodianName = "Cashier I",
        CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
        CashGlAccount = "1009-CASH",
        MaxCashLimit = 25000,
        Status = "Closed",
        Description = "Deposit holding account",
        IsNegativeBalanceAllowed = true,
        CustodyStartDate = DateTime.Now.AddDays(-7)
    },

    new()
    {
        Code = "CA-10",
        Name = "Audit Cash Account",
        BranchId = MasterDataIds.Branches.EliteLoungerAbuDhabi,
        BranchName = SeedLookup.BranchName(MasterDataIds.Branches.EliteLoungerAbuDhabi),
        CustodianName = "Cashier J",
        CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.AED),
        CashGlAccount = "1010-CASH",
        MaxCashLimit = 60000,
        Status = "Active",
        Description = "Audit purposes",
        IsNegativeBalanceAllowed = false,
        CustodyStartDate = DateTime.Now.AddDays(-90)
    },

    new()
    {
        Code = "CA-11",
        Name = "Warehouse Cash Account",
        BranchId = MasterDataIds.Branches.SofaCraftDubai,
        BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftDubai),
        CustodianName = "Cashier K",
        CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.AED),
        CashGlAccount = "1011-CASH",
        MaxCashLimit = 80000,
        Status = "Active",
        Description = "Warehouse daily operations",
        IsNegativeBalanceAllowed = false,
        CustodyStartDate = DateTime.Now.AddDays(-25)
    },

    new()
    {
        Code = "CA-12",
        Name = "Exhibition Cash Fund",
        BranchId = MasterDataIds.Branches.SofaCraftUSA_SFO,
        BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftUSA_SFO),
        CustodianName = "Cashier L",
        CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.USD),
        CashGlAccount = "1012-CASH",
        MaxCashLimit = 45000,
        Status = "Active",
        Description = "Trade show and exhibition fund",
        IsNegativeBalanceAllowed = true,
        CustodyStartDate = DateTime.Now.AddDays(-14)
    }
};

        }


    }
    }

