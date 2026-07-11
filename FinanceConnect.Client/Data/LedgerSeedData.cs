using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    public static class LedgerSeedData
    {
        public static List<LedgerModel> GetSeedData()
        {
            return new List<LedgerModel>
            {
                    new LedgerModel
            {
                Id = MasterDataIds.Ledgers.PrimaryLedger,
                TenantId = MasterDataIds.Tenants.Default,
                LedgerCode = "PRIM",
                LedgerName = "Primary Ledger",
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = SeedLookup.CompanyCode(MasterDataIds.Companies.SofaCraft),
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                Description = "Primary statutory ledger for Tata Industries",
                IsDefaultLedger = true,
                LedgerType = "Primary",
                BaseCurrencyId = MasterDataIds.Currencies.INR,
                BaseCurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                BaseCurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                CurrencyMode = "SingleCurrencyOnly",
                ExchangeRateSource = "Manual",
                AllowPostingFromDate = new DateTime(2025, 4, 1),
                AllowPostingToDate = new DateTime(2026, 3, 31),
                LockBackDatedPosting = false,
                BackdatedPostingDaysAllowed = 30,
                FuturePostingDaysAllowed = 7,
                RequireApprovalBeforePosting = false,
                EnforceAccountingPeriodOpen = true,
                IsConsolidationEligible = true,
                Status = "Active",
                LockStatus = "LockedAfterPosting",
                CreatedAt = DateTime.Now.AddDays(-90),
                CreatedBy = "System"
            },
            new LedgerModel
            {
                Id = MasterDataIds.Ledgers.ManagementLedger,
                TenantId = MasterDataIds.Tenants.Default,
                LedgerCode = "MGMT",
                LedgerName = "Management Ledger",
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = SeedLookup.CompanyCode(MasterDataIds.Companies.SofaCraft),
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                Description = "Management reporting ledger for internal use",
                IsDefaultLedger = false,
                LedgerType = "Management",
                BaseCurrencyId = MasterDataIds.Currencies.INR,
                BaseCurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                BaseCurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                CurrencyMode = "MultiCurrencyAllowed",
                ExchangeRateSource = "Manual",
                AllowPostingFromDate = new DateTime(2025, 4, 1),
                AllowPostingToDate = new DateTime(2026, 3, 31),
                LockBackDatedPosting = false,
                BackdatedPostingDaysAllowed = 60,
                FuturePostingDaysAllowed = 30,
                RequireApprovalBeforePosting = false,
                EnforceAccountingPeriodOpen = false,
                IsConsolidationEligible = false,
                Status = "Active",
                LockStatus = "Unlocked",
                CreatedAt = DateTime.Now.AddDays(-60),
                CreatedBy = "System"
            },
            new LedgerModel
            {
                Id = MasterDataIds.Ledgers.ReliancePrimary,
                TenantId = MasterDataIds.Tenants.Default,
                LedgerCode = "PRIM",
                LedgerName = "Primary Ledger",
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = SeedLookup.CompanyCode(MasterDataIds.Companies.SofaCraft),
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                Description = "Primary statutory ledger for Reliance",
                IsDefaultLedger = true,
                LedgerType = "Primary",
                BaseCurrencyId = MasterDataIds.Currencies.INR,
                BaseCurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                BaseCurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                CurrencyMode = "MultiCurrencyAllowed",
                ExchangeRateSource = "Manual",
                AllowPostingFromDate = new DateTime(2025, 4, 1),
                AllowPostingToDate = new DateTime(2026, 3, 31),
                LockBackDatedPosting = true,
                BackdatedPostingDaysAllowed = 15,
                FuturePostingDaysAllowed = 7,
                RequireApprovalBeforePosting = true,
                EnforceAccountingPeriodOpen = true,
                IsConsolidationEligible = true,
                Status = "Active",
                LockStatus = "Unlocked",
                CreatedAt = DateTime.Now.AddDays(-45),
                CreatedBy = "System"
            }
            };
        }
    }
}
