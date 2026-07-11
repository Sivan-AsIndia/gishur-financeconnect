using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    public class FinanceDataService
    {
        // NOTE (Demo app): Same pattern as MasterDataService - refresh restores sample data

        private static List<T> CloneList<T>(IEnumerable<T> source) where T : class
        {
            var json = System.Text.Json.JsonSerializer.Serialize(source);
            return System.Text.Json.JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }

        // Working (mutable) data
        private List<LedgerModel> _ledgers = new();
        private List<OpeningBalanceModel> _openingBalances = new();
        private List<ClosingBalanceModel> _closingBalances = new();

        public FinanceDataService()
        {
            ResetAllToSeed();
        }

        public void ResetAllToSeed()
        {
            _ledgers = CloneList(_seedLedgers);
            _openingBalances = CloneList(_seedOpeningBalances);
            _closingBalances = CloneList(_seedClosingBalances);
        }

        public void ResetLedgersToSeed() => _ledgers = CloneList(_seedLedgers);
        public void ResetOpeningBalancesToSeed() => _openingBalances = CloneList(_seedOpeningBalances);
        public void ResetClosingBalancesToSeed() => _closingBalances = CloneList(_seedClosingBalances);

        #region Seed Data

        // Seed Ledgers
        private static readonly List<LedgerModel> _seedLedgers = new()
        {
            new LedgerModel
            {
                Id = MasterDataIds.Ledgers.PrimaryLedger,
                TenantId = MasterDataIds.Tenants.Default,
                LedgerCode = "SOFA-PRIM",
                LedgerName = "Primary Ledger - SofaCraft",
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = "SOFA01",
                CompanyName = "SofaCraft Furnishings Private Limited",
                Description = "Primary statutory ledger for SofaCraft Furnishings",
                IsDefaultLedger = true,
                LedgerType = LedgerTypes.Primary,
                BaseCurrencyId = MasterDataIds.Currencies.INR,
                BaseCurrencyCode = "INR",
                BaseCurrencyName = "Indian Rupee",
                CurrencyMode = CurrencyModes.MultiCurrencyAllowed,
                ExchangeRateSource = ExchangeRateSources.Manual,
                AllowPostingFromDate = new DateTime(2025, 4, 1),
                AllowPostingToDate = new DateTime(2026, 3, 31),
                LockBackDatedPosting = false,
                BackdatedPostingDaysAllowed = 30,
                FuturePostingDaysAllowed = 7,
                RequireApprovalBeforePosting = false,
                EnforceAccountingPeriodOpen = true,
                IsConsolidationEligible = true,
                Status = LedgerStatus.Active,
                LockStatus = LockStatuses.LockedAfterPosting,
                CreatedAt = DateTime.Now.AddDays(-60),
                CreatedBy = "System"
            },
            new LedgerModel
            {
                Id = MasterDataIds.Ledgers.ManagementLedger,
                TenantId = MasterDataIds.Tenants.Default,
                LedgerCode = "MGMT",
                LedgerName = "Management Ledger",
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = "SOFA01",
                CompanyName = "SofaCraft Furnishings Private Limited",
                Description = "Internal management reporting ledger",
                IsDefaultLedger = false,
                LedgerType = LedgerTypes.Management,
                BaseCurrencyId = MasterDataIds.Currencies.INR,
                BaseCurrencyCode = "INR",
                BaseCurrencyName = "Indian Rupee",
                CurrencyMode = CurrencyModes.SingleCurrencyOnly,
                ExchangeRateSource = ExchangeRateSources.Manual,
                AllowPostingFromDate = new DateTime(2025, 4, 1),
                AllowPostingToDate = new DateTime(2026, 3, 31),
                LockBackDatedPosting = true,
                BackdatedPostingDaysAllowed = 7,
                FuturePostingDaysAllowed = 0,
                RequireApprovalBeforePosting = true,
                EnforceAccountingPeriodOpen = true,
                IsConsolidationEligible = false,
                Status = LedgerStatus.Active,
                LockStatus = LockStatuses.Unlocked,
                CreatedAt = DateTime.Now.AddDays(-45),
                CreatedBy = "System"
            },
            new LedgerModel
            {
                Id = MasterDataIds.Ledgers.ReliancePrimary,
                TenantId = MasterDataIds.Tenants.Default,
                LedgerCode = "IFRS",
                LedgerName = "IFRS Ledger",
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = "SOFA01",
                CompanyName = "SofaCraft Furnishings Private Limited",
                Description = "IFRS consolidation ledger",
                IsDefaultLedger = false,
                LedgerType = LedgerTypes.IFRS,
                BaseCurrencyId = MasterDataIds.Currencies.USD,
                BaseCurrencyCode = "USD",
                BaseCurrencyName = "US Dollar",
                ReportingCurrencyId = MasterDataIds.Currencies.INR,
                ReportingCurrencyCode = "INR",
                ReportingCurrencyName = "Indian Rupee",
                CurrencyMode = CurrencyModes.MultiCurrencyAllowed,
                ExchangeRateSource = ExchangeRateSources.BankRate,
                AllowPostingFromDate = new DateTime(2025, 4, 1),
                AllowPostingToDate = new DateTime(2026, 3, 31),
                LockBackDatedPosting = true,
                BackdatedPostingDaysAllowed = 15,
                FuturePostingDaysAllowed = 7,
                RequireApprovalBeforePosting = true,
                EnforceAccountingPeriodOpen = true,
                IsConsolidationEligible = true,
                Status = LedgerStatus.Draft,
                LockStatus = LockStatuses.Unlocked,
                CreatedAt = DateTime.Now.AddDays(-30),
                CreatedBy = "System"
            },
            new LedgerModel
            {
                Id = MasterDataIds.Ledgers.UrbanLoftPrimary,
                TenantId = MasterDataIds.Tenants.Default,
                LedgerCode = "GL-REL",
                LedgerName = "Primary Ledger",
                CompanyId = MasterDataIds.Companies.SofaCraftUSA,
                CompanyCode = "SOFAUS01",
                CompanyName = "SofaCraft Retail USA Inc.",
                Description = "Primary statutory ledger for Reliance Industries",
                IsDefaultLedger = true,
                LedgerType = LedgerTypes.Primary,
                BaseCurrencyId = MasterDataIds.Currencies.INR,
                BaseCurrencyCode = "INR",
                BaseCurrencyName = "Indian Rupee",
                CurrencyMode = CurrencyModes.MultiCurrencyAllowed,
                ExchangeRateSource = ExchangeRateSources.Manual,
                AllowPostingFromDate = new DateTime(2025, 4, 1),
                AllowPostingToDate = new DateTime(2026, 3, 31),
                LockBackDatedPosting = false,
                BackdatedPostingDaysAllowed = 30,
                FuturePostingDaysAllowed = 15,
                RequireApprovalBeforePosting = false,
                EnforceAccountingPeriodOpen = true,
                IsConsolidationEligible = true,
                Status = LedgerStatus.Active,
                LockStatus = LockStatuses.Unlocked,
                CreatedAt = DateTime.Now.AddDays(-55),
                CreatedBy = "System"
            },
            new LedgerModel
            {
                Id = MasterDataIds.Ledgers.DesertDunePrimary,
                TenantId = MasterDataIds.Tenants.Default,
                LedgerCode = "TAX",
                LedgerName = "Tax Ledger",
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = "SOFA01",
                CompanyName = "SofaCraft Furnishings Private Limited",
                Description = "Tax compliance ledger - inactive",
                IsDefaultLedger = false,
                LedgerType = LedgerTypes.Tax,
                BaseCurrencyId = MasterDataIds.Currencies.INR,
                BaseCurrencyCode = "INR",
                BaseCurrencyName = "Indian Rupee",
                CurrencyMode = CurrencyModes.SingleCurrencyOnly,
                ExchangeRateSource = ExchangeRateSources.Manual,
                EnforceAccountingPeriodOpen = true,
                IsConsolidationEligible = false,
                Status = LedgerStatus.Inactive,
                LockStatus = LockStatuses.Unlocked,
                LockReason = "No longer in use - consolidated with Primary ledger",
                CreatedAt = DateTime.Now.AddDays(-90),
                CreatedBy = "System"
            }
      
,
new LedgerModel
{
    Id = MasterDataIds.Ledgers.SofaCraftUSAPrimary,
    TenantId = MasterDataIds.Tenants.Default,
    LedgerCode = "PRIM",
    LedgerName = "Primary Ledger",
    CompanyId = MasterDataIds.Companies.OakNest,
    CompanyCode = "OAK01",
    CompanyName = "OakNest Interiors LLP",
    Description = "Primary statutory ledger for OakNest",
    IsDefaultLedger = true,
    LedgerType = "Primary",
    BaseCurrencyId = MasterDataIds.Currencies.INR,
    BaseCurrencyCode = "INR",
    BaseCurrencyName = "Indian Rupee",
    CurrencyMode = "SingleCurrencyOnly",
    ExchangeRateSource = "Manual",
    AllowPostingFromDate = new DateTime(2024, 4, 1),
    AllowPostingToDate = new DateTime(2025, 3, 31),
    LockBackDatedPosting = false,
    BackdatedPostingDaysAllowed = 30,
    FuturePostingDaysAllowed = 7,
    RequireApprovalBeforePosting = false,
    EnforceAccountingPeriodOpen = true,
    IsConsolidationEligible = true,
    Status = "Active",
    LockStatus = "Unlocked",
    CreatedAt = DateTime.Now.AddDays(-120),
    CreatedBy = "System"
},
new LedgerModel
{
    Id = MasterDataIds.Ledgers.OakNestPrimary,
    TenantId = MasterDataIds.Tenants.Default,
    LedgerCode = "PRIM",
    LedgerName = "Primary Ledger",
    CompanyId = MasterDataIds.Companies.UrbanLoft,
    CompanyCode = "URBN01",
    CompanyName = "UrbanLoft Home Décor Private Limited",
    Description = "Primary statutory ledger for UrbanLoft",
    IsDefaultLedger = true,
    LedgerType = "Primary",
    BaseCurrencyId = MasterDataIds.Currencies.INR,
    BaseCurrencyCode = "INR",
    BaseCurrencyName = "Indian Rupee",
    CurrencyMode = "MultiCurrencyAllowed",
    ExchangeRateSource = "Manual",
    AllowPostingFromDate = new DateTime(2024, 4, 1),
    AllowPostingToDate = new DateTime(2025, 3, 31),
    LockBackDatedPosting = false,
    BackdatedPostingDaysAllowed = 30,
    FuturePostingDaysAllowed = 7,
    RequireApprovalBeforePosting = true,
    EnforceAccountingPeriodOpen = true,
    IsConsolidationEligible = true,
    Status = "Active",
    LockStatus = "Unlocked",
    CreatedAt = DateTime.Now.AddDays(-120),
    CreatedBy = "System"
},
new LedgerModel
{
    Id = MasterDataIds.Ledgers.DesertDunePrimary,
    TenantId = MasterDataIds.Tenants.Default,
    LedgerCode = "PRIM",
    LedgerName = "Primary Ledger",
    CompanyId = MasterDataIds.Companies.DesertDune,
    CompanyCode = "DUNE01",
    CompanyName = "DesertDune Furniture Trading LLC",
    Description = "Primary statutory ledger for DesertDune",
    IsDefaultLedger = true,
    LedgerType = "Primary",
    BaseCurrencyId = MasterDataIds.Currencies.AED,
    BaseCurrencyCode = "AED",
    BaseCurrencyName = "UAE Dirham",
    CurrencyMode = "MultiCurrencyAllowed",
    ExchangeRateSource = "Manual",
    AllowPostingFromDate = new DateTime(2025, 1, 1),
    AllowPostingToDate = new DateTime(2025, 12, 31),
    LockBackDatedPosting = false,
    BackdatedPostingDaysAllowed = 30,
    FuturePostingDaysAllowed = 7,
    RequireApprovalBeforePosting = true,
    EnforceAccountingPeriodOpen = true,
    IsConsolidationEligible = true,
    Status = "Active",
    LockStatus = "Unlocked",
    CreatedAt = DateTime.Now.AddDays(-120),
    CreatedBy = "System"
},
// PlushComfort Sofas - Delhi, India
new LedgerModel
{
    Id = MasterDataIds.Ledgers.PlushComfortPrimary,
    TenantId = MasterDataIds.Tenants.Default,
    LedgerCode = "PRIM",
    LedgerName = "Primary Ledger",
    CompanyId = MasterDataIds.Companies.PlushComfort,
    CompanyCode = "PLUSH01",
    CompanyName = "PlushComfort Sofas Pvt Ltd",
    Description = "Primary statutory ledger for PlushComfort",
    IsDefaultLedger = true,
    LedgerType = "Primary",
    BaseCurrencyId = MasterDataIds.Currencies.INR,
    BaseCurrencyCode = "INR",
    BaseCurrencyName = "Indian Rupee",
    CurrencyMode = "MultiCurrencyAllowed",
    ExchangeRateSource = "Manual",
    AllowPostingFromDate = new DateTime(2024, 4, 1),
    AllowPostingToDate = new DateTime(2025, 3, 31),
    LockBackDatedPosting = false,
    BackdatedPostingDaysAllowed = 30,
    FuturePostingDaysAllowed = 7,
    RequireApprovalBeforePosting = true,
    EnforceAccountingPeriodOpen = true,
    IsConsolidationEligible = true,
    Status = "Active",
    LockStatus = "Unlocked",
    CreatedAt = DateTime.Now.AddDays(-90),
    CreatedBy = "System"
},
// VelvetRest Furniture - Pune, India
new LedgerModel
{
    Id = MasterDataIds.Ledgers.VelvetRestPrimary,
    TenantId = MasterDataIds.Tenants.Default,
    LedgerCode = "PRIM",
    LedgerName = "Primary Ledger",
    CompanyId = MasterDataIds.Companies.VelvetRest,
    CompanyCode = "VELVET01",
    CompanyName = "VelvetRest Furniture Industries Pvt Ltd",
    Description = "Primary statutory ledger for VelvetRest",
    IsDefaultLedger = true,
    LedgerType = "Primary",
    BaseCurrencyId = MasterDataIds.Currencies.INR,
    BaseCurrencyCode = "INR",
    BaseCurrencyName = "Indian Rupee",
    CurrencyMode = "SingleCurrencyOnly",
    ExchangeRateSource = "Manual",
    AllowPostingFromDate = new DateTime(2024, 4, 1),
    AllowPostingToDate = new DateTime(2025, 3, 31),
    LockBackDatedPosting = false,
    BackdatedPostingDaysAllowed = 30,
    FuturePostingDaysAllowed = 7,
    RequireApprovalBeforePosting = false,
    EnforceAccountingPeriodOpen = true,
    IsConsolidationEligible = true,
    Status = "Active",
    LockStatus = "Unlocked",
    CreatedAt = DateTime.Now.AddDays(-85),
    CreatedBy = "System"
},
// CozyCraft Living Solutions - Hyderabad, India
new LedgerModel
{
    Id = MasterDataIds.Ledgers.CozyCraftPrimary,
    TenantId = MasterDataIds.Tenants.Default,
    LedgerCode = "PRIM",
    LedgerName = "Primary Ledger",
    CompanyId = MasterDataIds.Companies.CozyCraft,
    CompanyCode = "COZY01",
    CompanyName = "CozyCraft Living Solutions LLP",
    Description = "Primary statutory ledger for CozyCraft",
    IsDefaultLedger = true,
    LedgerType = "Primary",
    BaseCurrencyId = MasterDataIds.Currencies.INR,
    BaseCurrencyCode = "INR",
    BaseCurrencyName = "Indian Rupee",
    CurrencyMode = "MultiCurrencyAllowed",
    ExchangeRateSource = "Manual",
    AllowPostingFromDate = new DateTime(2024, 4, 1),
    AllowPostingToDate = new DateTime(2025, 3, 31),
    LockBackDatedPosting = false,
    BackdatedPostingDaysAllowed = 30,
    FuturePostingDaysAllowed = 7,
    RequireApprovalBeforePosting = true,
    EnforceAccountingPeriodOpen = true,
    IsConsolidationEligible = true,
    Status = "Active",
    LockStatus = "Unlocked",
    CreatedAt = DateTime.Now.AddDays(-80),
    CreatedBy = "System"
},
// PremiumSeating International - Singapore
new LedgerModel
{
    Id = MasterDataIds.Ledgers.PremiumSeatingPrimary,
    TenantId = MasterDataIds.Tenants.Default,
    LedgerCode = "PRIM",
    LedgerName = "Primary Ledger",
    CompanyId = MasterDataIds.Companies.PremiumSeating,
    CompanyCode = "PREM01",
    CompanyName = "PremiumSeating International Pte Ltd",
    Description = "Primary statutory ledger for PremiumSeating",
    IsDefaultLedger = true,
    LedgerType = "Primary",
    BaseCurrencyId = MasterDataIds.Currencies.SGD,
    BaseCurrencyCode = "SGD",
    BaseCurrencyName = "Singapore Dollar",
    CurrencyMode = "MultiCurrencyAllowed",
    ExchangeRateSource = "Manual",
    AllowPostingFromDate = new DateTime(2025, 1, 1),
    AllowPostingToDate = new DateTime(2025, 12, 31),
    LockBackDatedPosting = false,
    BackdatedPostingDaysAllowed = 30,
    FuturePostingDaysAllowed = 7,
    RequireApprovalBeforePosting = true,
    EnforceAccountingPeriodOpen = true,
    IsConsolidationEligible = true,
    Status = "Active",
    LockStatus = "Unlocked",
    CreatedAt = DateTime.Now.AddDays(-75),
    CreatedBy = "System"
},
// CloudSofa Designs - Kolkata, India
new LedgerModel
{
    Id = MasterDataIds.Ledgers.CloudSofaPrimary,
    TenantId = MasterDataIds.Tenants.Default,
    LedgerCode = "PRIM",
    LedgerName = "Primary Ledger",
    CompanyId = MasterDataIds.Companies.CloudSofa,
    CompanyCode = "CLOUD01",
    CompanyName = "CloudSofa Designs Pvt Ltd",
    Description = "Primary statutory ledger for CloudSofa",
    IsDefaultLedger = true,
    LedgerType = "Primary",
    BaseCurrencyId = MasterDataIds.Currencies.INR,
    BaseCurrencyCode = "INR",
    BaseCurrencyName = "Indian Rupee",
    CurrencyMode = "SingleCurrencyOnly",
    ExchangeRateSource = "Manual",
    AllowPostingFromDate = new DateTime(2024, 4, 1),
    AllowPostingToDate = new DateTime(2025, 3, 31),
    LockBackDatedPosting = false,
    BackdatedPostingDaysAllowed = 30,
    FuturePostingDaysAllowed = 7,
    RequireApprovalBeforePosting = false,
    EnforceAccountingPeriodOpen = true,
    IsConsolidationEligible = true,
    Status = "Active",
    LockStatus = "Unlocked",
    CreatedAt = DateTime.Now.AddDays(-70),
    CreatedBy = "System"
},
// EliteLoungers Manufacturing - Abu Dhabi, UAE
new LedgerModel
{
    Id = MasterDataIds.Ledgers.EliteLoungersPrimary,
    TenantId = MasterDataIds.Tenants.Default,
    LedgerCode = "PRIM",
    LedgerName = "Primary Ledger",
    CompanyId = MasterDataIds.Companies.EliteLoungers,
    CompanyCode = "ELITE01",
    CompanyName = "EliteLoungers Manufacturing LLC",
    Description = "Primary statutory ledger for EliteLoungers",
    IsDefaultLedger = true,
    LedgerType = "Primary",
    BaseCurrencyId = MasterDataIds.Currencies.AED,
    BaseCurrencyCode = "AED",
    BaseCurrencyName = "UAE Dirham",
    CurrencyMode = "MultiCurrencyAllowed",
    ExchangeRateSource = "Manual",
    AllowPostingFromDate = new DateTime(2025, 1, 1),
    AllowPostingToDate = new DateTime(2025, 12, 31),
    LockBackDatedPosting = false,
    BackdatedPostingDaysAllowed = 30,
    FuturePostingDaysAllowed = 7,
    RequireApprovalBeforePosting = true,
    EnforceAccountingPeriodOpen = true,
    IsConsolidationEligible = true,
    Status = "Active",
    LockStatus = "Unlocked",
    CreatedAt = DateTime.Now.AddDays(-65),
    CreatedBy = "System"
}  };

        // Seed Opening Balances
        private static readonly List<OpeningBalanceModel> _seedOpeningBalances = new()
        {
            new OpeningBalanceModel
            {
                Id = MasterDataIds.OpeningBalances.OB01,
                TenantId = MasterDataIds.Tenants.Default,
                OpeningBalanceNumber = "OB-2025-00001",
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = "SOFA01",
                CompanyName = "SofaCraft Furnishings Private Limited",
                BranchId = MasterDataIds.Branches.SofaCraftHQ,
                BranchCode = "HO",
                BranchName = "Head Office Chennai",
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = "PRIM",
                LedgerName = "Primary Ledger",
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearCode = "FY2025-26",
                FiscalYearName = "Financial Year 2025-26",
                OpeningAccountingPeriodId = MasterDataIds.AccountingPeriods.Apr2025,
                OpeningAccountingPeriodCode = "APR-2025",
                OpeningAccountingPeriodName = "April 2025",
                OpeningDate = new DateTime(2025, 4, 1),
                EntryMode = EntryModes.ManualEntry,
                Notes = "Opening balance for FY 2025-26",
                RestrictToBalanceSheetAccounts = true,
                CurrencyMode = CurrencyModes.MultiCurrencyAllowed,
                Status = OpeningBalanceStatus.Posted,
                ApprovedBy = "FinanceController",
                ApprovedAt = new DateTime(2025, 4, 2, 10, 30, 0),
                PostedBy = "FinanceController",
                PostedAt = new DateTime(2025, 4, 2, 11, 0, 0),
                PostingReference = "JE-OB-2025-00001",
                CreatedAt = new DateTime(2025, 4, 1, 9, 0, 0),
                CreatedBy = "FinanceAdmin",
                Lines = new List<OpeningBalanceLineModel>
                {
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00001-0001-0001-0001-000000000001"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB01,
                        AccountId = MasterDataIds.Accounts.HDFCBankAccount,
                        AccountCode = "1100",
                        AccountName = "Cash in Hand",
                        AccountNature = "Asset",
                        LineDescription = "Opening cash balance",
                        DebitAmountBase = 125000.00m,
                        CreditAmountBase = 0m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00002-0002-0002-0002-000000000002"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB01,
                        AccountId = MasterDataIds.Accounts.AccountsReceivable,
                        AccountCode = "1200",
                        AccountName = "Bank - HDFC Current",
                        AccountNature = "Asset",
                        LineDescription = "Opening bank balance - HDFC",
                        DebitAmountBase = 450000.00m,
                        CreditAmountBase = 0m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00003-0003-0003-0003-000000000003"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB01,
                        AccountId = MasterDataIds.Accounts.AccountsPayable,
                        AccountCode = "2100",
                        AccountName = "Accounts Payable",
                        AccountNature = "Liability",
                        LineDescription = "Opening payables balance",
                        DebitAmountBase = 0m,
                        CreditAmountBase = 175000.00m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00004-0004-0004-0004-000000000004"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB01,
                        AccountId = MasterDataIds.Accounts.ShareCapital,
                        AccountCode = "3100",
                        AccountName = "Share Capital",
                        AccountNature = "Equity",
                        LineDescription = "Opening capital balance",
                        DebitAmountBase = 0m,
                        CreditAmountBase = 400000.00m
                    }
                }
            },
            new OpeningBalanceModel
            {
                Id = MasterDataIds.OpeningBalances.OB02,
                TenantId = MasterDataIds.Tenants.Default,
                OpeningBalanceNumber = "OB-2025-00002",
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = "SOFA01",
                CompanyName = "SofaCraft Furnishings Private Limited",
                BranchId = MasterDataIds.Branches.SofaCraftBengaluru,
                BranchCode = "FMP-DEL",
                BranchName = "Delhi Branch",
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = "PRIM",
                LedgerName = "Primary Ledger",
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearCode = "FY2025-26",
                FiscalYearName = "Financial Year 2025-26",
                OpeningAccountingPeriodId = MasterDataIds.AccountingPeriods.Apr2025,
                OpeningAccountingPeriodCode = "APR-2025",
                OpeningAccountingPeriodName = "April 2025",
                OpeningDate = new DateTime(2025, 4, 1),
                EntryMode = EntryModes.ManualEntry,
                Notes = "Opening balance for Delhi Branch FY 2025-26",
                RestrictToBalanceSheetAccounts = true,
                CurrencyMode = CurrencyModes.MultiCurrencyAllowed,
                Status = OpeningBalanceStatus.Approved,
                ApprovedBy = "FinanceController",
                ApprovedAt = new DateTime(2025, 4, 3, 14, 30, 0),
                CreatedAt = new DateTime(2025, 4, 2, 11, 0, 0),
                CreatedBy = "FinanceAdmin",
                Lines = new List<OpeningBalanceLineModel>
                {
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00005-0005-0005-0005-000000000005"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB02,
                        AccountId = MasterDataIds.Accounts.HDFCBankAccount,
                        AccountCode = "1100",
                        AccountName = "Cash in Hand",
                        AccountNature = "Asset",
                        LineDescription = "Opening cash balance - Delhi",
                        DebitAmountBase = 50000.00m,
                        CreditAmountBase = 0m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00006-0006-0006-0006-000000000006"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB02,
                        AccountId = MasterDataIds.Accounts.AccountsReceivable,
                        AccountCode = "1200",
                        AccountName = "Bank - HDFC Current",
                        AccountNature = "Asset",
                        LineDescription = "Opening bank balance - HDFC Delhi",
                        DebitAmountBase = 200000.00m,
                        CreditAmountBase = 0m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00007-0007-0007-0007-000000000007"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB02,
                        AccountId = MasterDataIds.Accounts.ShareCapital,
                        AccountCode = "3100",
                        AccountName = "Share Capital",
                        AccountNature = "Equity",
                        LineDescription = "Inter-branch equity allocation",
                        DebitAmountBase = 0m,
                        CreditAmountBase = 250000.00m
                    }
                }
            },
            new OpeningBalanceModel
            {
                Id = MasterDataIds.OpeningBalances.OB03,
                TenantId = MasterDataIds.Tenants.Default,
                OpeningBalanceNumber = "OB-2025-00003",
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = "SOFA01",
                CompanyName = "SofaCraft Furnishings Private Limited",
                BranchId = MasterDataIds.Branches.SofaCraftBengaluru,
                BranchCode = "BLR",
                BranchName = "Bangalore Branch",
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = "PRIM",
                LedgerName = "Primary Ledger",
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearCode = "FY2025-26",
                FiscalYearName = "Financial Year 2025-26",
                OpeningAccountingPeriodId = MasterDataIds.AccountingPeriods.Apr2025,
                OpeningAccountingPeriodCode = "APR-2025",
                OpeningAccountingPeriodName = "April 2025",
                OpeningDate = new DateTime(2025, 4, 1),
                EntryMode = EntryModes.ManualEntry,
                Notes = "Opening balance for Bangalore Branch - Draft",
                RestrictToBalanceSheetAccounts = true,
                CurrencyMode = CurrencyModes.MultiCurrencyAllowed,
                Status = OpeningBalanceStatus.Draft,
                CreatedAt = new DateTime(2025, 4, 5, 9, 0, 0),
                CreatedBy = "FinanceAdmin",
                Lines = new List<OpeningBalanceLineModel>
                {
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00008-0008-0008-0008-000000000008"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB03,
                        AccountId = MasterDataIds.Accounts.HDFCBankAccount,
                        AccountCode = "1100",
                        AccountName = "Cash in Hand",
                        AccountNature = "Asset",
                        LineDescription = "Opening cash balance - Bangalore",
                        DebitAmountBase = 75000.00m,
                        CreditAmountBase = 0m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00009-0009-0009-0009-000000000009"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB03,
                        AccountId = MasterDataIds.Accounts.ShareCapital,
                        AccountCode = "3100",
                        AccountName = "Share Capital",
                        AccountNature = "Equity",
                        LineDescription = "Inter-branch equity allocation",
                        DebitAmountBase = 0m,
                        CreditAmountBase = 75000.00m
                    }
                }
            },
            new OpeningBalanceModel
            {
                Id = MasterDataIds.OpeningBalances.OB04,
                TenantId = MasterDataIds.Tenants.Default,
                OpeningBalanceNumber = "OB-2025-00004",
                CompanyId = MasterDataIds.Companies.SofaCraftUSA,
                CompanyCode = "SOFAUS01",
                CompanyName = "SofaCraft Retail USA Inc.",
                BranchId = MasterDataIds.Branches.UrbanLoftMumbai,
                BranchCode = "HO",
                BranchName = "Head Office Chennai",
                LedgerId = MasterDataIds.Ledgers.UrbanLoftPrimary,
                LedgerCode = "GL-REL",
                LedgerName = "Primary Ledger",
                FiscalYearId = MasterDataIds.FiscalYears.FY2024_25,
                FiscalYearCode = "FY2025-26",
                FiscalYearName = "Financial Year 2025-26",
                OpeningAccountingPeriodId = MasterDataIds.AccountingPeriods.Apr2024,
                OpeningAccountingPeriodCode = "APR-2025",
                OpeningAccountingPeriodName = "April 2025",
                OpeningDate = new DateTime(2025, 4, 1),
                EntryMode = EntryModes.BulkImport,
                Notes = "Opening balance for Reliance - Submitted for approval",
                RestrictToBalanceSheetAccounts = true,
                CurrencyMode = CurrencyModes.MultiCurrencyAllowed,
                Status = OpeningBalanceStatus.Submitted,
                CreatedAt = new DateTime(2025, 4, 3, 10, 0, 0),
                CreatedBy = "FinanceAdmin",
                Lines = new List<OpeningBalanceLineModel>
                {
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00010-0010-0010-0010-000000000010"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB04,
                        AccountId = MasterDataIds.Accounts.HDFCBankAccount,
                        AccountCode = "1100",
                        AccountName = "Cash in Hand",
                        AccountNature = "Asset",
                        LineDescription = "Opening cash balance",
                        DebitAmountBase = 500000.00m,
                        CreditAmountBase = 0m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00011-0011-0011-0011-000000000011"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB04,
                        AccountId = MasterDataIds.Accounts.AccountsReceivable,
                        AccountCode = "1200",
                        AccountName = "Bank - HDFC Current",
                        AccountNature = "Asset",
                        LineDescription = "Opening bank balance",
                        DebitAmountBase = 2500000.00m,
                        CreditAmountBase = 0m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00012-0012-0012-0012-000000000012"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB04,
                        AccountId = MasterDataIds.Accounts.AccountsPayable,
                        AccountCode = "2100",
                        AccountName = "Accounts Payable",
                        AccountNature = "Liability",
                        LineDescription = "Opening payables",
                        DebitAmountBase = 0m,
                        CreditAmountBase = 1000000.00m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00013-0013-0013-0013-000000000013"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB04,
                        AccountId = MasterDataIds.Accounts.ShareCapital,
                        AccountCode = "3100",
                        AccountName = "Share Capital",
                        AccountNature = "Equity",
                        LineDescription = "Opening capital",
                        DebitAmountBase = 0m,
                        CreditAmountBase = 2000000.00m
                    }
                }
            },
            // PlushComfort Sofas - Delhi Opening Balance (Posted)
            new OpeningBalanceModel
            {
                Id = MasterDataIds.OpeningBalances.OB06,
                TenantId = MasterDataIds.Tenants.Default,
                OpeningBalanceNumber = "OB-2024-00006",
                CompanyId = MasterDataIds.Companies.PlushComfort,
                CompanyCode = "PLUSH01",
                CompanyName = "PlushComfort Sofas Pvt Ltd",
                BranchId = MasterDataIds.Branches.PlushComfortDelhi,
                BranchCode = "DELHI-HO",
                BranchName = "Delhi Head Office",
                LedgerId = MasterDataIds.Ledgers.PlushComfortPrimary,
                LedgerCode = "PRIM",
                LedgerName = "Primary Ledger",
                FiscalYearId = MasterDataIds.CompanyFiscalYears.PlushComfort2024,
                FiscalYearCode = "FY24-25",
                FiscalYearName = "Financial Year 2024-25",
                OpeningAccountingPeriodId = Guid.Parse("a6fa0001-0001-0001-0001-000000000001"),
                OpeningAccountingPeriodCode = "APR-2024",
                OpeningAccountingPeriodName = "April 2024",
                OpeningDate = new DateTime(2024, 4, 1),
                EntryMode = EntryModes.ManualEntry,
                Notes = "Opening balance for PlushComfort - Posted and approved",
                RestrictToBalanceSheetAccounts = true,
                CurrencyMode = CurrencyModes.MultiCurrencyAllowed,
                Status = OpeningBalanceStatus.Posted,
                CreatedAt = new DateTime(2024, 4, 1, 10, 0, 0),
                CreatedBy = "FinanceAdmin",
                Lines = new List<OpeningBalanceLineModel>
                {
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba60001-0001-0001-0001-000000000001"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB06,
                        AccountId = MasterDataIds.Accounts.HDFCBankAccount,
                        AccountCode = "1100",
                        AccountName = "Cash in Hand",
                        AccountNature = "Asset",
                        LineDescription = "Opening cash balance",
                        DebitAmountBase = 250000.00m,
                        CreditAmountBase = 0m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba60002-0002-0002-0002-000000000002"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB06,
                        AccountId = MasterDataIds.Accounts.AccountsReceivable,
                        AccountCode = "1200",
                        AccountName = "Bank - ICICI Current",
                        AccountNature = "Asset",
                        LineDescription = "Opening bank balance",
                        DebitAmountBase = 1850000.00m,
                        CreditAmountBase = 0m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba60003-0003-0003-0003-000000000003"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB06,
                        AccountId = MasterDataIds.Accounts.ShareCapital,
                        AccountCode = "3100",
                        AccountName = "Share Capital",
                        AccountNature = "Equity",
                        LineDescription = "Opening capital",
                        DebitAmountBase = 0m,
                        CreditAmountBase = 2100000.00m
                    }
                }
            },
            // VelvetRest Furniture - Pune Opening Balance (Approved)
            new OpeningBalanceModel
            {
                Id = MasterDataIds.OpeningBalances.OB07,
                TenantId = MasterDataIds.Tenants.Default,
                OpeningBalanceNumber = "OB-2024-00007",
                CompanyId = MasterDataIds.Companies.VelvetRest,
                CompanyCode = "VELVET01",
                CompanyName = "VelvetRest Furniture Industries Pvt Ltd",
                BranchId = MasterDataIds.Branches.VelvetRestPune,
                BranchCode = "PUNE-HO",
                BranchName = "Pune Head Office",
                LedgerId = MasterDataIds.Ledgers.VelvetRestPrimary,
                LedgerCode = "PRIM",
                LedgerName = "Primary Ledger",
                FiscalYearId = MasterDataIds.CompanyFiscalYears.VelvetRest2024,
                FiscalYearCode = "FY24-25",
                FiscalYearName = "Financial Year 2024-25",
                OpeningAccountingPeriodId = Guid.Parse("a7fa0001-0001-0001-0001-000000000001"),
                OpeningAccountingPeriodCode = "APR-2024",
                OpeningAccountingPeriodName = "April 2024",
                OpeningDate = new DateTime(2024, 4, 1),
                EntryMode = EntryModes.BulkImport,
                Notes = "Opening balance for VelvetRest - Approved",
                RestrictToBalanceSheetAccounts = true,
                CurrencyMode = CurrencyModes.SingleCurrencyOnly,
                Status = OpeningBalanceStatus.Approved,
                CreatedAt = new DateTime(2024, 4, 2, 11, 0, 0),
                CreatedBy = "AccountsManager",
                Lines = new List<OpeningBalanceLineModel>
                {
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba70001-0001-0001-0001-000000000001"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB07,
                        AccountId = MasterDataIds.Accounts.HDFCBankAccount,
                        AccountCode = "1100",
                        AccountName = "Cash in Hand",
                        AccountNature = "Asset",
                        LineDescription = "Opening cash balance",
                        DebitAmountBase = 175000.00m,
                        CreditAmountBase = 0m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba70002-0002-0002-0002-000000000002"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB07,
                        AccountId = MasterDataIds.Accounts.AccountsReceivable,
                        AccountCode = "1200",
                        AccountName = "Bank - SBI Current",
                        AccountNature = "Asset",
                        LineDescription = "Opening bank balance",
                        DebitAmountBase = 925000.00m,
                        CreditAmountBase = 0m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba70003-0003-0003-0003-000000000003"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB07,
                        AccountId = MasterDataIds.Accounts.ShareCapital,
                        AccountCode = "3100",
                        AccountName = "Share Capital",
                        AccountNature = "Equity",
                        LineDescription = "Opening capital",
                        DebitAmountBase = 0m,
                        CreditAmountBase = 1100000.00m
                    }
                }
            },
            // CozyCraft Living - Hyderabad Opening Balance (Posted)
            new OpeningBalanceModel
            {
                Id = MasterDataIds.OpeningBalances.OB08,
                TenantId = MasterDataIds.Tenants.Default,
                OpeningBalanceNumber = "OB-2024-00008",
                CompanyId = MasterDataIds.Companies.CozyCraft,
                CompanyCode = "COZY01",
                CompanyName = "CozyCraft Living Solutions LLP",
                BranchId = MasterDataIds.Branches.CozyCraftHyderabad,
                BranchCode = "HYD-HO",
                BranchName = "Hyderabad Head Office",
                LedgerId = MasterDataIds.Ledgers.CozyCraftPrimary,
                LedgerCode = "PRIM",
                LedgerName = "Primary Ledger",
                FiscalYearId = MasterDataIds.CompanyFiscalYears.CozyCraft2024,
                FiscalYearCode = "FY24-25",
                FiscalYearName = "Financial Year 2024-25",
                OpeningAccountingPeriodId = Guid.Parse("a8fa0001-0001-0001-0001-000000000001"),
                OpeningAccountingPeriodCode = "APR-2024",
                OpeningAccountingPeriodName = "April 2024",
                OpeningDate = new DateTime(2024, 4, 1),
                EntryMode = EntryModes.ManualEntry,
                Notes = "Opening balance for CozyCraft - Posted",
                RestrictToBalanceSheetAccounts = true,
                CurrencyMode = CurrencyModes.MultiCurrencyAllowed,
                Status = OpeningBalanceStatus.Posted,
                CreatedAt = new DateTime(2024, 4, 1, 9, 0, 0),
                CreatedBy = "FinanceController",
                Lines = new List<OpeningBalanceLineModel>
                {
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba80001-0001-0001-0001-000000000001"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB08,
                        AccountId = MasterDataIds.Accounts.HDFCBankAccount,
                        AccountCode = "1100",
                        AccountName = "Cash in Hand",
                        AccountNature = "Asset",
                        LineDescription = "Opening cash balance",
                        DebitAmountBase = 85000.00m,
                        CreditAmountBase = 0m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba80002-0002-0002-0002-000000000002"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB08,
                        AccountId = MasterDataIds.Accounts.AccountsReceivable,
                        AccountCode = "1200",
                        AccountName = "Bank - HDFC Current",
                        AccountNature = "Asset",
                        LineDescription = "Opening bank balance",
                        DebitAmountBase = 615000.00m,
                        CreditAmountBase = 0m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba80003-0003-0003-0003-000000000003"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB08,
                        AccountId = MasterDataIds.Accounts.ShareCapital,
                        AccountCode = "3100",
                        AccountName = "Partners Capital",
                        AccountNature = "Equity",
                        LineDescription = "Opening capital",
                        DebitAmountBase = 0m,
                        CreditAmountBase = 700000.00m
                    }
                }
            },
            // PremiumSeating International - Singapore Opening Balance (Posted)
            new OpeningBalanceModel
            {
                Id = MasterDataIds.OpeningBalances.OB09,
                TenantId = MasterDataIds.Tenants.Default,
                OpeningBalanceNumber = "OB-2025-00009",
                CompanyId = MasterDataIds.Companies.PremiumSeating,
                CompanyCode = "PREM01",
                CompanyName = "PremiumSeating International Pte Ltd",
                BranchId = MasterDataIds.Branches.PremiumSeatingSG,
                BranchCode = "SG-HQ",
                BranchName = "Singapore Headquarters",
                LedgerId = MasterDataIds.Ledgers.PremiumSeatingPrimary,
                LedgerCode = "PRIM",
                LedgerName = "Primary Ledger",
                FiscalYearId = MasterDataIds.CompanyFiscalYears.PremiumSeating2025,
                FiscalYearCode = "FY2025",
                FiscalYearName = "Financial Year 2025",
                OpeningAccountingPeriodId = Guid.Parse("a9fa0001-0001-0001-0001-000000000001"),
                OpeningAccountingPeriodCode = "JAN-2025",
                OpeningAccountingPeriodName = "January 2025",
                OpeningDate = new DateTime(2025, 1, 1),
                EntryMode = EntryModes.ManualEntry,
                Notes = "Opening balance for PremiumSeating - Posted",
                RestrictToBalanceSheetAccounts = true,
                CurrencyMode = CurrencyModes.MultiCurrencyAllowed,
                Status = OpeningBalanceStatus.Posted,
                CreatedAt = new DateTime(2025, 1, 2, 10, 0, 0),
                CreatedBy = "FinanceDirector",
                Lines = new List<OpeningBalanceLineModel>
                {
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba90001-0001-0001-0001-000000000001"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB09,
                        AccountId = MasterDataIds.Accounts.HDFCBankAccount,
                        AccountCode = "1100",
                        AccountName = "Cash in Hand",
                        AccountNature = "Asset",
                        LineDescription = "Opening cash balance",
                        DebitAmountBase = 45000.00m,
                        CreditAmountBase = 0m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba90002-0002-0002-0002-000000000002"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB09,
                        AccountId = MasterDataIds.Accounts.AccountsReceivable,
                        AccountCode = "1200",
                        AccountName = "Bank - DBS Current",
                        AccountNature = "Asset",
                        LineDescription = "Opening bank balance",
                        DebitAmountBase = 755000.00m,
                        CreditAmountBase = 0m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba90003-0003-0003-0003-000000000003"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB09,
                        AccountId = MasterDataIds.Accounts.ShareCapital,
                        AccountCode = "3100",
                        AccountName = "Share Capital",
                        AccountNature = "Equity",
                        LineDescription = "Opening capital",
                        DebitAmountBase = 0m,
                        CreditAmountBase = 800000.00m
                    }
                }
            },
            // CloudSofa Designs - Kolkata Opening Balance (Submitted)
            new OpeningBalanceModel
            {
                Id = MasterDataIds.OpeningBalances.OB10,
                TenantId = MasterDataIds.Tenants.Default,
                OpeningBalanceNumber = "OB-2024-00010",
                CompanyId = MasterDataIds.Companies.CloudSofa,
                CompanyCode = "CLOUD01",
                CompanyName = "CloudSofa Designs Pvt Ltd",
                BranchId = MasterDataIds.Branches.CloudSofaKolkata,
                BranchCode = "KOL-HO",
                BranchName = "Kolkata Head Office",
                LedgerId = MasterDataIds.Ledgers.CloudSofaPrimary,
                LedgerCode = "PRIM",
                LedgerName = "Primary Ledger",
                FiscalYearId = MasterDataIds.CompanyFiscalYears.CloudSofa2024,
                FiscalYearCode = "FY24-25",
                FiscalYearName = "Financial Year 2024-25",
                OpeningAccountingPeriodId = Guid.Parse("aafa0001-0001-0001-0001-000000000001"),
                OpeningAccountingPeriodCode = "APR-2024",
                OpeningAccountingPeriodName = "April 2024",
                OpeningDate = new DateTime(2024, 4, 1),
                EntryMode = EntryModes.BulkImport,
                Notes = "Opening balance for CloudSofa - Submitted for approval",
                RestrictToBalanceSheetAccounts = true,
                CurrencyMode = CurrencyModes.SingleCurrencyOnly,
                Status = OpeningBalanceStatus.Submitted,
                CreatedAt = new DateTime(2024, 4, 5, 14, 0, 0),
                CreatedBy = "AccountsAdmin",
                Lines = new List<OpeningBalanceLineModel>
                {
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0baa0001-0001-0001-0001-000000000001"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB10,
                        AccountId = MasterDataIds.Accounts.HDFCBankAccount,
                        AccountCode = "1100",
                        AccountName = "Cash in Hand",
                        AccountNature = "Asset",
                        LineDescription = "Opening cash balance",
                        DebitAmountBase = 120000.00m,
                        CreditAmountBase = 0m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0baa0002-0002-0002-0002-000000000002"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB10,
                        AccountId = MasterDataIds.Accounts.AccountsReceivable,
                        AccountCode = "1200",
                        AccountName = "Bank - Axis Current",
                        AccountNature = "Asset",
                        LineDescription = "Opening bank balance",
                        DebitAmountBase = 480000.00m,
                        CreditAmountBase = 0m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0baa0003-0003-0003-0003-000000000003"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB10,
                        AccountId = MasterDataIds.Accounts.ShareCapital,
                        AccountCode = "3100",
                        AccountName = "Share Capital",
                        AccountNature = "Equity",
                        LineDescription = "Opening capital",
                        DebitAmountBase = 0m,
                        CreditAmountBase = 600000.00m
                    }
                }
            },
            // EliteLoungers Manufacturing - Abu Dhabi Opening Balance (Draft)
            new OpeningBalanceModel
            {
                Id = MasterDataIds.OpeningBalances.OB11,
                TenantId = MasterDataIds.Tenants.Default,
                OpeningBalanceNumber = "OB-2025-00011",
                CompanyId = MasterDataIds.Companies.EliteLoungers,
                CompanyCode = "ELITE01",
                CompanyName = "EliteLoungers Manufacturing LLC",
                BranchId = MasterDataIds.Branches.EliteLoungerAbuDhabi,
                BranchCode = "AUH-HQ",
                BranchName = "Abu Dhabi Headquarters",
                LedgerId = MasterDataIds.Ledgers.EliteLoungersPrimary,
                LedgerCode = "PRIM",
                LedgerName = "Primary Ledger",
                FiscalYearId = MasterDataIds.CompanyFiscalYears.EliteLoungers2025,
                FiscalYearCode = "FY2025",
                FiscalYearName = "Financial Year 2025",
                OpeningAccountingPeriodId = Guid.Parse("abfa0001-0001-0001-0001-000000000001"),
                OpeningAccountingPeriodCode = "JAN-2025",
                OpeningAccountingPeriodName = "January 2025",
                OpeningDate = new DateTime(2025, 1, 1),
                EntryMode = EntryModes.ManualEntry,
                Notes = "Opening balance for EliteLoungers - Draft",
                RestrictToBalanceSheetAccounts = true,
                CurrencyMode = CurrencyModes.MultiCurrencyAllowed,
                Status = OpeningBalanceStatus.Draft,
                CreatedAt = new DateTime(2025, 1, 8, 11, 0, 0),
                CreatedBy = "FinanceManager",
                Lines = new List<OpeningBalanceLineModel>
                {
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0bab0001-0001-0001-0001-000000000001"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB11,
                        AccountId = MasterDataIds.Accounts.HDFCBankAccount,
                        AccountCode = "1100",
                        AccountName = "Cash in Hand",
                        AccountNature = "Asset",
                        LineDescription = "Opening cash balance",
                        DebitAmountBase = 35000.00m,
                        CreditAmountBase = 0m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0bab0002-0002-0002-0002-000000000002"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB11,
                        AccountId = MasterDataIds.Accounts.AccountsReceivable,
                        AccountCode = "1200",
                        AccountName = "Bank - Emirates NBD",
                        AccountNature = "Asset",
                        LineDescription = "Opening bank balance",
                        DebitAmountBase = 465000.00m,
                        CreditAmountBase = 0m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0bab0003-0003-0003-0003-000000000003"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB11,
                        AccountId = MasterDataIds.Accounts.ShareCapital,
                        AccountCode = "3100",
                        AccountName = "Share Capital",
                        AccountNature = "Equity",
                        LineDescription = "Opening capital",
                        DebitAmountBase = 0m,
                        CreditAmountBase = 500000.00m
                    }
                }
            },
            // Mumbai Branch - FY 2025-26
            new OpeningBalanceModel
            {
                Id = MasterDataIds.OpeningBalances.OB12,
                TenantId = MasterDataIds.Tenants.Default,
                OpeningBalanceNumber = "OB-2025-00012",
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = "SOFA01",
                CompanyName = "SofaCraft Furnishings Private Limited",
                BranchId = MasterDataIds.Branches.SofaCraftDubai,
                BranchCode = "MUM",
                BranchName = "Mumbai Branch",
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = "PRIM",
                LedgerName = "Primary Ledger",
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearCode = "FY2025-26",
                FiscalYearName = "Financial Year 2025-26",
                OpeningAccountingPeriodId = MasterDataIds.AccountingPeriods.Apr2025,
                OpeningAccountingPeriodCode = "APR-2025",
                OpeningAccountingPeriodName = "April 2025",
                OpeningDate = new DateTime(2025, 4, 1),
                EntryMode = EntryModes.ManualEntry,
                Notes = "Opening balance for Mumbai Branch FY 2025-26",
                RestrictToBalanceSheetAccounts = true,
                CurrencyMode = CurrencyModes.MultiCurrencyAllowed,
                Status = OpeningBalanceStatus.Posted,
                ApprovedBy = "FinanceController",
                ApprovedAt = new DateTime(2025, 4, 3, 15, 0, 0),
                PostedBy = "FinanceController",
                PostedAt = new DateTime(2025, 4, 3, 16, 0, 0),
                PostingReference = "JE-OB-2025-00012",
                CreatedAt = new DateTime(2025, 4, 3, 9, 0, 0),
                CreatedBy = "FinanceAdmin",
                Lines = new List<OpeningBalanceLineModel>
                {
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00013-0013-0013-0013-000000000013"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB12,
                        AccountId = MasterDataIds.Accounts.HDFCBankAccount,
                        AccountCode = "1100",
                        AccountName = "Cash in Hand",
                        AccountNature = "Asset",
                        LineDescription = "Opening cash balance - Mumbai",
                        DebitAmountBase = 95000.00m,
                        CreditAmountBase = 0m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00014-0014-0014-0014-000000000014"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB12,
                        AccountId = MasterDataIds.Accounts.AccountsReceivable,
                        AccountCode = "1200",
                        AccountName = "Bank - HDFC Current",
                        AccountNature = "Asset",
                        LineDescription = "Opening bank balance - HDFC Mumbai",
                        DebitAmountBase = 380000.00m,
                        CreditAmountBase = 0m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00015-0015-0015-0015-000000000015"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB12,
                        AccountId = MasterDataIds.Accounts.ShareCapital,
                        AccountCode = "3100",
                        AccountName = "Share Capital",
                        AccountNature = "Equity",
                        LineDescription = "Inter-branch equity allocation",
                        DebitAmountBase = 0m,
                        CreditAmountBase = 475000.00m
                    }
                }
            },
            // Pune Branch - FY 2025-26
            new OpeningBalanceModel
            {
                Id = MasterDataIds.OpeningBalances.OB_0A,
                TenantId = MasterDataIds.Tenants.Default,
                OpeningBalanceNumber = "OB-2025-00013",
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = "SOFA01",
                CompanyName = "SofaCraft Furnishings Private Limited",
                BranchId = MasterDataIds.Branches.PlushComfortDelhi,
                BranchCode = "PUNE",
                BranchName = "Pune Branch",
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = "PRIM",
                LedgerName = "Primary Ledger",
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearCode = "FY2025-26",
                FiscalYearName = "Financial Year 2025-26",
                OpeningAccountingPeriodId = MasterDataIds.AccountingPeriods.Apr2025,
                OpeningAccountingPeriodCode = "APR-2025",
                OpeningAccountingPeriodName = "April 2025",
                OpeningDate = new DateTime(2025, 4, 1),
                EntryMode = EntryModes.ManualEntry,
                Notes = "Opening balance for Pune Branch FY 2025-26",
                RestrictToBalanceSheetAccounts = true,
                CurrencyMode = CurrencyModes.MultiCurrencyAllowed,
                Status = OpeningBalanceStatus.Approved,
                ApprovedBy = "FinanceController",
                ApprovedAt = new DateTime(2025, 4, 4, 10, 0, 0),
                CreatedAt = new DateTime(2025, 4, 3, 14, 0, 0),
                CreatedBy = "FinanceAdmin",
                Lines = new List<OpeningBalanceLineModel>
                {
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00016-0016-0016-0016-000000000016"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB_0A,
                        AccountId = MasterDataIds.Accounts.HDFCBankAccount,
                        AccountCode = "1100",
                        AccountName = "Cash in Hand",
                        AccountNature = "Asset",
                        LineDescription = "Opening cash balance - Pune",
                        DebitAmountBase = 65000.00m,
                        CreditAmountBase = 0m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00017-0017-0017-0017-000000000017"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB_0A,
                        AccountId = MasterDataIds.Accounts.AccountsReceivable,
                        AccountCode = "1200",
                        AccountName = "Bank - HDFC Current",
                        AccountNature = "Asset",
                        LineDescription = "Opening bank balance - HDFC Pune",
                        DebitAmountBase = 285000.00m,
                        CreditAmountBase = 0m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00018-0018-0018-0018-000000000018"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB_0A,
                        AccountId = MasterDataIds.Accounts.ShareCapital,
                        AccountCode = "3100",
                        AccountName = "Share Capital",
                        AccountNature = "Equity",
                        LineDescription = "Inter-branch equity allocation",
                        DebitAmountBase = 0m,
                        CreditAmountBase = 350000.00m
                    }
                }
            },
            // Hyderabad Branch - FY 2025-26
            new OpeningBalanceModel
            {
                Id = MasterDataIds.OpeningBalances.OB_0B,
                TenantId = MasterDataIds.Tenants.Default,
                OpeningBalanceNumber = "OB-2025-00014",
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = "SOFA01",
                CompanyName = "SofaCraft Furnishings Private Limited",
                BranchId = MasterDataIds.Branches.VelvetRestPune,
                BranchCode = "HYD",
                BranchName = "Hyderabad Branch",
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = "PRIM",
                LedgerName = "Primary Ledger",
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearCode = "FY2025-26",
                FiscalYearName = "Financial Year 2025-26",
                OpeningAccountingPeriodId = MasterDataIds.AccountingPeriods.Apr2025,
                OpeningAccountingPeriodCode = "APR-2025",
                OpeningAccountingPeriodName = "April 2025",
                OpeningDate = new DateTime(2025, 4, 1),
                EntryMode = EntryModes.ManualEntry,
                Notes = "Opening balance for Hyderabad Branch - Draft",
                RestrictToBalanceSheetAccounts = true,
                CurrencyMode = CurrencyModes.MultiCurrencyAllowed,
                Status = OpeningBalanceStatus.Draft,
                CreatedAt = new DateTime(2025, 4, 6, 9, 0, 0),
                CreatedBy = "FinanceAdmin",
                Lines = new List<OpeningBalanceLineModel>
                {
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00019-0019-0019-0019-000000000019"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB_0B,
                        AccountId = MasterDataIds.Accounts.HDFCBankAccount,
                        AccountCode = "1100",
                        AccountName = "Cash in Hand",
                        AccountNature = "Asset",
                        LineDescription = "Opening cash balance - Hyderabad",
                        DebitAmountBase = 55000.00m,
                        CreditAmountBase = 0m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00020-0020-0020-0020-000000000020"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB_0B,
                        AccountId = MasterDataIds.Accounts.AccountsReceivable,
                        AccountCode = "1200",
                        AccountName = "Bank - ICICI Current",
                        AccountNature = "Asset",
                        LineDescription = "Opening bank balance - ICICI Hyderabad",
                        DebitAmountBase = 225000.00m,
                        CreditAmountBase = 0m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00021-0021-0021-0021-000000000021"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB_0B,
                        AccountId = MasterDataIds.Accounts.ShareCapital,
                        AccountCode = "3100",
                        AccountName = "Share Capital",
                        AccountNature = "Equity",
                        LineDescription = "Inter-branch equity allocation",
                        DebitAmountBase = 0m,
                        CreditAmountBase = 280000.00m
                    }
                }
            },
            // Kolkata Branch - FY 2025-26
            new OpeningBalanceModel
            {
                Id = MasterDataIds.OpeningBalances.OB_0C,
                TenantId = MasterDataIds.Tenants.Default,
                OpeningBalanceNumber = "OB-2025-00015",
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = "SOFA01",
                CompanyName = "SofaCraft Furnishings Private Limited",
                BranchId = MasterDataIds.Branches.CozyCraftHyderabad,
                BranchCode = "KOL",
                BranchName = "Kolkata Branch",
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = "PRIM",
                LedgerName = "Primary Ledger",
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearCode = "FY2025-26",
                FiscalYearName = "Financial Year 2025-26",
                OpeningAccountingPeriodId = MasterDataIds.AccountingPeriods.Apr2025,
                OpeningAccountingPeriodCode = "APR-2025",
                OpeningAccountingPeriodName = "April 2025",
                OpeningDate = new DateTime(2025, 4, 1),
                EntryMode = EntryModes.ManualEntry,
                Notes = "Opening balance for Kolkata Branch FY 2025-26",
                RestrictToBalanceSheetAccounts = true,
                CurrencyMode = CurrencyModes.MultiCurrencyAllowed,
                Status = OpeningBalanceStatus.Posted,
                ApprovedBy = "FinanceController",
                ApprovedAt = new DateTime(2025, 4, 5, 11, 0, 0),
                PostedBy = "FinanceController",
                PostedAt = new DateTime(2025, 4, 5, 12, 0, 0),
                PostingReference = "JE-OB-2025-00015",
                CreatedAt = new DateTime(2025, 4, 4, 16, 0, 0),
                CreatedBy = "FinanceAdmin",
                Lines = new List<OpeningBalanceLineModel>
                {
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00022-0022-0022-0022-000000000022"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB_0C,
                        AccountId = MasterDataIds.Accounts.HDFCBankAccount,
                        AccountCode = "1100",
                        AccountName = "Cash in Hand",
                        AccountNature = "Asset",
                        LineDescription = "Opening cash balance - Kolkata",
                        DebitAmountBase = 48000.00m,
                        CreditAmountBase = 0m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00023-0023-0023-0023-000000000023"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB_0C,
                        AccountId = MasterDataIds.Accounts.AccountsReceivable,
                        AccountCode = "1200",
                        AccountName = "Bank - SBI Current",
                        AccountNature = "Asset",
                        LineDescription = "Opening bank balance - SBI Kolkata",
                        DebitAmountBase = 192000.00m,
                        CreditAmountBase = 0m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00024-0024-0024-0024-000000000024"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB_0C,
                        AccountId = MasterDataIds.Accounts.ShareCapital,
                        AccountCode = "3100",
                        AccountName = "Share Capital",
                        AccountNature = "Equity",
                        LineDescription = "Inter-branch equity allocation",
                        DebitAmountBase = 0m,
                        CreditAmountBase = 240000.00m
                    }
                }
            },
            // Ahmedabad Branch - FY 2025-26
            new OpeningBalanceModel
            {
                Id = MasterDataIds.OpeningBalances.OB16,
                TenantId = MasterDataIds.Tenants.Default,
                OpeningBalanceNumber = "OB-2025-00016",
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = "SOFA01",
                CompanyName = "SofaCraft Furnishings Private Limited",
                BranchId = MasterDataIds.Branches.PremiumSeatingSG,
                BranchCode = "AMD",
                BranchName = "Ahmedabad Branch",
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = "PRIM",
                LedgerName = "Primary Ledger",
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearCode = "FY2025-26",
                FiscalYearName = "Financial Year 2025-26",
                OpeningAccountingPeriodId = MasterDataIds.AccountingPeriods.Apr2025,
                OpeningAccountingPeriodCode = "APR-2025",
                OpeningAccountingPeriodName = "April 2025",
                OpeningDate = new DateTime(2025, 4, 1),
                EntryMode = EntryModes.ManualEntry,
                Notes = "Opening balance for Ahmedabad Branch - Draft",
                RestrictToBalanceSheetAccounts = true,
                CurrencyMode = CurrencyModes.MultiCurrencyAllowed,
                Status = OpeningBalanceStatus.Draft,
                CreatedAt = new DateTime(2025, 4, 7, 10, 0, 0),
                CreatedBy = "FinanceAdmin",
                Lines = new List<OpeningBalanceLineModel>
                {
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00025-0025-0025-0025-000000000025"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB16,
                        AccountId = MasterDataIds.Accounts.HDFCBankAccount,
                        AccountCode = "1100",
                        AccountName = "Cash in Hand",
                        AccountNature = "Asset",
                        LineDescription = "Opening cash balance - Ahmedabad",
                        DebitAmountBase = 72000.00m,
                        CreditAmountBase = 0m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00026-0026-0026-0026-000000000026"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB16,
                        AccountId = MasterDataIds.Accounts.AccountsReceivable,
                        AccountCode = "1200",
                        AccountName = "Bank - Axis Current",
                        AccountNature = "Asset",
                        LineDescription = "Opening bank balance - Axis Ahmedabad",
                        DebitAmountBase = 318000.00m,
                        CreditAmountBase = 0m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00027-0027-0027-0027-000000000027"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB16,
                        AccountId = MasterDataIds.Accounts.ShareCapital,
                        AccountCode = "3100",
                        AccountName = "Share Capital",
                        AccountNature = "Equity",
                        LineDescription = "Inter-branch equity allocation",
                        DebitAmountBase = 0m,
                        CreditAmountBase = 390000.00m
                    }
                }
            },
            // Jaipur Branch - FY 2025-26
            new OpeningBalanceModel
            {
                Id = MasterDataIds.OpeningBalances.OB17,
                TenantId = MasterDataIds.Tenants.Default,
                OpeningBalanceNumber = "OB-2025-00017",
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = "SOFA01",
                CompanyName = "SofaCraft Furnishings Private Limited",
                BranchId = MasterDataIds.Branches.CloudSofaKolkata,
                BranchCode = "JAI",
                BranchName = "Jaipur Branch",
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = "PRIM",
                LedgerName = "Primary Ledger",
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearCode = "FY2025-26",
                FiscalYearName = "Financial Year 2025-26",
                OpeningAccountingPeriodId = MasterDataIds.AccountingPeriods.Apr2025,
                OpeningAccountingPeriodCode = "APR-2025",
                OpeningAccountingPeriodName = "April 2025",
                OpeningDate = new DateTime(2025, 4, 1),
                EntryMode = EntryModes.ManualEntry,
                Notes = "Opening balance for Jaipur Branch FY 2025-26",
                RestrictToBalanceSheetAccounts = true,
                CurrencyMode = CurrencyModes.MultiCurrencyAllowed,
                Status = OpeningBalanceStatus.Approved,
                ApprovedBy = "FinanceController",
                ApprovedAt = new DateTime(2025, 4, 6, 14, 0, 0),
                CreatedAt = new DateTime(2025, 4, 5, 11, 0, 0),
                CreatedBy = "FinanceAdmin",
                Lines = new List<OpeningBalanceLineModel>
                {
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00028-0028-0028-0028-000000000028"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB17,
                        AccountId = MasterDataIds.Accounts.HDFCBankAccount,
                        AccountCode = "1100",
                        AccountName = "Cash in Hand",
                        AccountNature = "Asset",
                        LineDescription = "Opening cash balance - Jaipur",
                        DebitAmountBase = 42000.00m,
                        CreditAmountBase = 0m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00029-0029-0029-0029-000000000029"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB17,
                        AccountId = MasterDataIds.Accounts.AccountsReceivable,
                        AccountCode = "1200",
                        AccountName = "Bank - PNB Current",
                        AccountNature = "Asset",
                        LineDescription = "Opening bank balance - PNB Jaipur",
                        DebitAmountBase = 178000.00m,
                        CreditAmountBase = 0m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00030-0030-0030-0030-000000000030"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB17,
                        AccountId = MasterDataIds.Accounts.ShareCapital,
                        AccountCode = "3100",
                        AccountName = "Share Capital",
                        AccountNature = "Equity",
                        LineDescription = "Inter-branch equity allocation",
                        DebitAmountBase = 0m,
                        CreditAmountBase = 220000.00m
                    }
                }
            },
            // Chandigarh Branch - FY 2025-26
            new OpeningBalanceModel
            {
                Id = MasterDataIds.OpeningBalances.OB18,
                TenantId = MasterDataIds.Tenants.Default,
                OpeningBalanceNumber = "OB-2025-00018",
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = "SOFA01",
                CompanyName = "SofaCraft Furnishings Private Limited",
                BranchId = MasterDataIds.Branches.EliteLoungerAbuDhabi,
                BranchCode = "CHD",
                BranchName = "Chandigarh Branch",
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = "PRIM",
                LedgerName = "Primary Ledger",
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearCode = "FY2025-26",
                FiscalYearName = "Financial Year 2025-26",
                OpeningAccountingPeriodId = MasterDataIds.AccountingPeriods.Apr2025,
                OpeningAccountingPeriodCode = "APR-2025",
                OpeningAccountingPeriodName = "April 2025",
                OpeningDate = new DateTime(2025, 4, 1),
                EntryMode = EntryModes.ManualEntry,
                Notes = "Opening balance for Chandigarh Branch FY 2025-26",
                RestrictToBalanceSheetAccounts = true,
                CurrencyMode = CurrencyModes.MultiCurrencyAllowed,
                Status = OpeningBalanceStatus.Posted,
                ApprovedBy = "FinanceController",
                ApprovedAt = new DateTime(2025, 4, 7, 9, 0, 0),
                PostedBy = "FinanceController",
                PostedAt = new DateTime(2025, 4, 7, 10, 0, 0),
                PostingReference = "JE-OB-2025-00018",
                CreatedAt = new DateTime(2025, 4, 6, 15, 0, 0),
                CreatedBy = "FinanceAdmin",
                Lines = new List<OpeningBalanceLineModel>
                {
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00031-0031-0031-0031-000000000031"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB18,
                        AccountId = MasterDataIds.Accounts.HDFCBankAccount,
                        AccountCode = "1100",
                        AccountName = "Cash in Hand",
                        AccountNature = "Asset",
                        LineDescription = "Opening cash balance - Chandigarh",
                        DebitAmountBase = 58000.00m,
                        CreditAmountBase = 0m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00032-0032-0032-0032-000000000032"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB18,
                        AccountId = MasterDataIds.Accounts.AccountsReceivable,
                        AccountCode = "1200",
                        AccountName = "Bank - HDFC Current",
                        AccountNature = "Asset",
                        LineDescription = "Opening bank balance - HDFC Chandigarh",
                        DebitAmountBase = 242000.00m,
                        CreditAmountBase = 0m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00033-0033-0033-0033-000000000033"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB18,
                        AccountId = MasterDataIds.Accounts.ShareCapital,
                        AccountCode = "3100",
                        AccountName = "Share Capital",
                        AccountNature = "Equity",
                        LineDescription = "Inter-branch equity allocation",
                        DebitAmountBase = 0m,
                        CreditAmountBase = 300000.00m
                    }
                }
            },
            // Lucknow Branch - FY 2025-26
            new OpeningBalanceModel
            {
                Id = MasterDataIds.OpeningBalances.OB19,
                TenantId = MasterDataIds.Tenants.Default,
                OpeningBalanceNumber = "OB-2025-00019",
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = "SOFA01",
                CompanyName = "SofaCraft Furnishings Private Limited",
                BranchId = MasterDataIds.Branches.SofaCraftUSA_SFO,
                BranchCode = "LKO",
                BranchName = "Lucknow Branch",
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = "PRIM",
                LedgerName = "Primary Ledger",
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearCode = "FY2025-26",
                FiscalYearName = "Financial Year 2025-26",
                OpeningAccountingPeriodId = MasterDataIds.AccountingPeriods.Apr2025,
                OpeningAccountingPeriodCode = "APR-2025",
                OpeningAccountingPeriodName = "April 2025",
                OpeningDate = new DateTime(2025, 4, 1),
                EntryMode = EntryModes.ManualEntry,
                Notes = "Opening balance for Lucknow Branch - Draft",
                RestrictToBalanceSheetAccounts = true,
                CurrencyMode = CurrencyModes.MultiCurrencyAllowed,
                Status = OpeningBalanceStatus.Draft,
                CreatedAt = new DateTime(2025, 4, 8, 9, 0, 0),
                CreatedBy = "FinanceAdmin",
                Lines = new List<OpeningBalanceLineModel>
                {
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00034-0034-0034-0034-000000000034"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB19,
                        AccountId = MasterDataIds.Accounts.HDFCBankAccount,
                        AccountCode = "1100",
                        AccountName = "Cash in Hand",
                        AccountNature = "Asset",
                        LineDescription = "Opening cash balance - Lucknow",
                        DebitAmountBase = 38000.00m,
                        CreditAmountBase = 0m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00035-0035-0035-0035-000000000035"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB19,
                        AccountId = MasterDataIds.Accounts.AccountsReceivable,
                        AccountCode = "1200",
                        AccountName = "Bank - SBI Current",
                        AccountNature = "Asset",
                        LineDescription = "Opening bank balance - SBI Lucknow",
                        DebitAmountBase = 152000.00m,
                        CreditAmountBase = 0m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00036-0036-0036-0036-000000000036"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB19,
                        AccountId = MasterDataIds.Accounts.ShareCapital,
                        AccountCode = "3100",
                        AccountName = "Share Capital",
                        AccountNature = "Equity",
                        LineDescription = "Inter-branch equity allocation",
                        DebitAmountBase = 0m,
                        CreditAmountBase = 190000.00m
                    }
                }
            },
            // Kochi Branch - FY 2025-26
            new OpeningBalanceModel
            {
                Id = MasterDataIds.OpeningBalances.OB20,
                TenantId = MasterDataIds.Tenants.Default,
                OpeningBalanceNumber = "OB-2025-00020",
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = "SOFA01",
                CompanyName = "SofaCraft Furnishings Private Limited",
                BranchId = MasterDataIds.Branches.OakNestBengaluru,
                BranchCode = "COK",
                BranchName = "Kochi Branch",
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = "PRIM",
                LedgerName = "Primary Ledger",
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearCode = "FY2025-26",
                FiscalYearName = "Financial Year 2025-26",
                OpeningAccountingPeriodId = MasterDataIds.AccountingPeriods.Apr2025,
                OpeningAccountingPeriodCode = "APR-2025",
                OpeningAccountingPeriodName = "April 2025",
                OpeningDate = new DateTime(2025, 4, 1),
                EntryMode = EntryModes.ManualEntry,
                Notes = "Opening balance for Kochi Branch FY 2025-26",
                RestrictToBalanceSheetAccounts = true,
                CurrencyMode = CurrencyModes.MultiCurrencyAllowed,
                Status = OpeningBalanceStatus.Approved,
                ApprovedBy = "FinanceController",
                ApprovedAt = new DateTime(2025, 4, 8, 14, 0, 0),
                CreatedAt = new DateTime(2025, 4, 7, 16, 0, 0),
                CreatedBy = "FinanceAdmin",
                Lines = new List<OpeningBalanceLineModel>
                {
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00037-0037-0037-0037-000000000037"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB20,
                        AccountId = MasterDataIds.Accounts.HDFCBankAccount,
                        AccountCode = "1100",
                        AccountName = "Cash in Hand",
                        AccountNature = "Asset",
                        LineDescription = "Opening cash balance - Kochi",
                        DebitAmountBase = 85000.00m,
                        CreditAmountBase = 0m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00038-0038-0038-0038-000000000038"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB20,
                        AccountId = MasterDataIds.Accounts.AccountsReceivable,
                        AccountCode = "1200",
                        AccountName = "Bank - Federal Bank Current",
                        AccountNature = "Asset",
                        LineDescription = "Opening bank balance - Federal Bank Kochi",
                        DebitAmountBase = 365000.00m,
                        CreditAmountBase = 0m
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.Parse("0ba00039-0039-0039-0039-000000000039"),
                        OpeningBalanceId = MasterDataIds.OpeningBalances.OB20,
                        AccountId = MasterDataIds.Accounts.ShareCapital,
                        AccountCode = "3100",
                        AccountName = "Share Capital",
                        AccountNature = "Equity",
                        LineDescription = "Inter-branch equity allocation",
                        DebitAmountBase = 0m,
                        CreditAmountBase = 450000.00m
                    }
                }
            }
        };

        // Seed Closing Balances
        private static readonly List<ClosingBalanceModel> _seedClosingBalances = new()
        {
            // April 2025 Closing Balances - Tata Industries - HQ Mumbai
            new ClosingBalanceModel
            {
                Id = MasterDataIds.ClosingBalances.CB01,
                TenantId = MasterDataIds.Tenants.Default,
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = "SOFA01",
                CompanyName = "SofaCraft Furnishings Private Limited",
                BranchId = MasterDataIds.Branches.SofaCraftHQ,
                BranchCode = "HO",
                BranchName = "Head Office Chennai",
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = "PRIM",
                LedgerName = "Primary Ledger",
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearCode = "FY2025-26",
                FiscalYearName = "Financial Year 2025-26",
                AccountingPeriodId = MasterDataIds.AccountingPeriods.Apr2025,
                AccountingPeriodCode = "APR-2025",
                AccountingPeriodName = "April 2025",
                AccountId = MasterDataIds.Accounts.HDFCBankAccount,
                AccountCode = "1100",
                AccountName = "Cash in Hand",
                OpeningDebit = 125000.00m,
                OpeningCredit = 0m,
                PeriodDebit = 50000.00m,
                PeriodCredit = 35000.00m,
                ClosingDebit = 140000.00m,
                ClosingCredit = 0m,
                ClosingBalanceSide = BalanceSides.Debit,
                ClosingBalanceAmount = 140000.00m,
                BaseCurrencyId = MasterDataIds.Currencies.INR,
                BaseCurrencyCode = "INR",
                BaseCurrencyName = "Indian Rupee",
                CloseRunId = MasterDataIds.CloseRuns.CE01,
                CloseStatus = CloseStatuses.Locked,
                CalculatedAt = new DateTime(2025, 5, 1, 9, 0, 0),
                CalculatedBy = "FinanceController",
                LockedAt = new DateTime(2025, 5, 1, 10, 0, 0),
                LockedBy = "FinanceController",
                AccountCodeSnapshot = "1100",
                AccountNameSnapshot = "Cash in Hand",
                BranchCodeSnapshot = "HO",
                BranchNameSnapshot = "Head Office Chennai",
                CreatedAt = new DateTime(2025, 5, 1, 9, 0, 0),
                CreatedBy = "System"
            },
            new ClosingBalanceModel
            {
                Id = MasterDataIds.ClosingBalances.CB02,
                TenantId = MasterDataIds.Tenants.Default,
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = "SOFA01",
                CompanyName = "SofaCraft Furnishings Private Limited",
                BranchId = MasterDataIds.Branches.SofaCraftHQ,
                BranchCode = "HO",
                BranchName = "Head Office Chennai",
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = "PRIM",
                LedgerName = "Primary Ledger",
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearCode = "FY2025-26",
                FiscalYearName = "Financial Year 2025-26",
                AccountingPeriodId = MasterDataIds.AccountingPeriods.Apr2025,
                AccountingPeriodCode = "APR-2025",
                AccountingPeriodName = "April 2025",
                AccountId = MasterDataIds.Accounts.AccountsReceivable,
                AccountCode = "1200",
                AccountName = "Bank - HDFC Current",
                OpeningDebit = 450000.00m,
                OpeningCredit = 0m,
                PeriodDebit = 200000.00m,
                PeriodCredit = 100000.00m,
                ClosingDebit = 550000.00m,
                ClosingCredit = 0m,
                ClosingBalanceSide = BalanceSides.Debit,
                ClosingBalanceAmount = 550000.00m,
                BaseCurrencyId = MasterDataIds.Currencies.INR,
                BaseCurrencyCode = "INR",
                BaseCurrencyName = "Indian Rupee",
                CloseRunId = MasterDataIds.CloseRuns.CE01,
                CloseStatus = CloseStatuses.Locked,
                CalculatedAt = new DateTime(2025, 5, 1, 9, 0, 0),
                CalculatedBy = "FinanceController",
                LockedAt = new DateTime(2025, 5, 1, 10, 0, 0),
                LockedBy = "FinanceController",
                AccountCodeSnapshot = "1200",
                AccountNameSnapshot = "Bank - HDFC Current",
                BranchCodeSnapshot = "HO",
                BranchNameSnapshot = "Head Office Chennai",
                CreatedAt = new DateTime(2025, 5, 1, 9, 0, 0),
                CreatedBy = "System"
            },
            new ClosingBalanceModel
            {
                Id = MasterDataIds.ClosingBalances.CB03,
                TenantId = MasterDataIds.Tenants.Default,
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = "SOFA01",
                CompanyName = "SofaCraft Furnishings Private Limited",
                BranchId = MasterDataIds.Branches.SofaCraftHQ,
                BranchCode = "HO",
                BranchName = "Head Office Chennai",
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = "PRIM",
                LedgerName = "Primary Ledger",
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearCode = "FY2025-26",
                FiscalYearName = "Financial Year 2025-26",
                AccountingPeriodId = MasterDataIds.AccountingPeriods.Apr2025,
                AccountingPeriodCode = "APR-2025",
                AccountingPeriodName = "April 2025",
                AccountId = MasterDataIds.Accounts.AccountsPayable,
                AccountCode = "2100",
                AccountName = "Accounts Payable",
                OpeningDebit = 0m,
                OpeningCredit = 175000.00m,
                PeriodDebit = 25000.00m,
                PeriodCredit = 50000.00m,
                ClosingDebit = 0m,
                ClosingCredit = 200000.00m,
                ClosingBalanceSide = BalanceSides.Credit,
                ClosingBalanceAmount = 200000.00m,
                BaseCurrencyId = MasterDataIds.Currencies.INR,
                BaseCurrencyCode = "INR",
                BaseCurrencyName = "Indian Rupee",
                CloseRunId = MasterDataIds.CloseRuns.CE01,
                CloseStatus = CloseStatuses.Locked,
                CalculatedAt = new DateTime(2025, 5, 1, 9, 0, 0),
                CalculatedBy = "FinanceController",
                LockedAt = new DateTime(2025, 5, 1, 10, 0, 0),
                LockedBy = "FinanceController",
                AccountCodeSnapshot = "2100",
                AccountNameSnapshot = "Accounts Payable",
                BranchCodeSnapshot = "HO",
                BranchNameSnapshot = "Head Office Chennai",
                CreatedAt = new DateTime(2025, 5, 1, 9, 0, 0),
                CreatedBy = "System"
            },
            new ClosingBalanceModel
            {
                Id = MasterDataIds.ClosingBalances.CB04,
                TenantId = MasterDataIds.Tenants.Default,
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = "SOFA01",
                CompanyName = "SofaCraft Furnishings Private Limited",
                BranchId = MasterDataIds.Branches.SofaCraftHQ,
                BranchCode = "HO",
                BranchName = "Head Office Chennai",
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = "PRIM",
                LedgerName = "Primary Ledger",
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearCode = "FY2025-26",
                FiscalYearName = "Financial Year 2025-26",
                AccountingPeriodId = MasterDataIds.AccountingPeriods.Apr2025,
                AccountingPeriodCode = "APR-2025",
                AccountingPeriodName = "April 2025",
                AccountId = MasterDataIds.Accounts.ShareCapital,
                AccountCode = "3100",
                AccountName = "Share Capital",
                OpeningDebit = 0m,
                OpeningCredit = 400000.00m,
                PeriodDebit = 0m,
                PeriodCredit = 0m,
                ClosingDebit = 0m,
                ClosingCredit = 400000.00m,
                ClosingBalanceSide = BalanceSides.Credit,
                ClosingBalanceAmount = 400000.00m,
                BaseCurrencyId = MasterDataIds.Currencies.INR,
                BaseCurrencyCode = "INR",
                BaseCurrencyName = "Indian Rupee",
                CloseRunId = MasterDataIds.CloseRuns.CE01,
                CloseStatus = CloseStatuses.Locked,
                CalculatedAt = new DateTime(2025, 5, 1, 9, 0, 0),
                CalculatedBy = "FinanceController",
                LockedAt = new DateTime(2025, 5, 1, 10, 0, 0),
                LockedBy = "FinanceController",
                AccountCodeSnapshot = "3100",
                AccountNameSnapshot = "Share Capital",
                BranchCodeSnapshot = "HO",
                BranchNameSnapshot = "Head Office Chennai",
                CreatedAt = new DateTime(2025, 5, 1, 9, 0, 0),
                CreatedBy = "System"
            },
            new ClosingBalanceModel
            {
                Id = MasterDataIds.ClosingBalances.CB05,
                TenantId = MasterDataIds.Tenants.Default,
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = "SOFA01",
                CompanyName = "SofaCraft Furnishings Private Limited",
                BranchId = MasterDataIds.Branches.SofaCraftHQ,
                BranchCode = "HO",
                BranchName = "Head Office Chennai",
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = "PRIM",
                LedgerName = "Primary Ledger",
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearCode = "FY2025-26",
                FiscalYearName = "Financial Year 2025-26",
                AccountingPeriodId = MasterDataIds.AccountingPeriods.Apr2025,
                AccountingPeriodCode = "APR-2025",
                AccountingPeriodName = "April 2025",
                AccountId = MasterDataIds.Accounts.PettyCash,
                AccountCode = "5100",
                AccountName = "Rent Expense",
                OpeningDebit = 0m,
                OpeningCredit = 0m,
                PeriodDebit = 45000.00m,
                PeriodCredit = 0m,
                ClosingDebit = 45000.00m,
                ClosingCredit = 0m,
                ClosingBalanceSide = BalanceSides.Debit,
                ClosingBalanceAmount = 45000.00m,
                BaseCurrencyId = MasterDataIds.Currencies.INR,
                BaseCurrencyCode = "INR",
                BaseCurrencyName = "Indian Rupee",
                CloseRunId = MasterDataIds.CloseRuns.CE01,
                CloseStatus = CloseStatuses.Locked,
                CalculatedAt = new DateTime(2025, 5, 1, 9, 0, 0),
                CalculatedBy = "FinanceController",
                LockedAt = new DateTime(2025, 5, 1, 10, 0, 0),
                LockedBy = "FinanceController",
                AccountCodeSnapshot = "5100",
                AccountNameSnapshot = "Rent Expense",
                BranchCodeSnapshot = "HO",
                BranchNameSnapshot = "Head Office Chennai",
                CreatedAt = new DateTime(2025, 5, 1, 9, 0, 0),
                CreatedBy = "System"
            },
            // May 2025 - Calculated but not yet locked
            new ClosingBalanceModel
            {
                Id = MasterDataIds.ClosingBalances.CB06,
                TenantId = MasterDataIds.Tenants.Default,
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = "SOFA01",
                CompanyName = "SofaCraft Furnishings Private Limited",
                BranchId = MasterDataIds.Branches.SofaCraftHQ,
                BranchCode = "HO",
                BranchName = "Head Office Chennai",
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = "PRIM",
                LedgerName = "Primary Ledger",
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearCode = "FY2025-26",
                FiscalYearName = "Financial Year 2025-26",
                AccountingPeriodId = MasterDataIds.AccountingPeriods.May2025,
                AccountingPeriodCode = "MAY-2025",
                AccountingPeriodName = "May 2025",
                AccountId = MasterDataIds.Accounts.HDFCBankAccount,
                AccountCode = "1100",
                AccountName = "Cash in Hand",
                OpeningDebit = 140000.00m,
                OpeningCredit = 0m,
                PeriodDebit = 0m,
                PeriodCredit = 10000.00m,
                ClosingDebit = 130000.00m,
                ClosingCredit = 0m,
                ClosingBalanceSide = BalanceSides.Debit,
                ClosingBalanceAmount = 130000.00m,
                BaseCurrencyId = MasterDataIds.Currencies.INR,
                BaseCurrencyCode = "INR",
                BaseCurrencyName = "Indian Rupee",
                CloseRunId = MasterDataIds.CloseRuns.CE02,
                CloseStatus = CloseStatuses.Verified,
                CalculatedAt = new DateTime(2025, 6, 1, 9, 0, 0),
                CalculatedBy = "FinanceController",
                AccountCodeSnapshot = "1100",
                AccountNameSnapshot = "Cash in Hand",
                BranchCodeSnapshot = "HO",
                BranchNameSnapshot = "Head Office Chennai",
                CreatedAt = new DateTime(2025, 6, 1, 9, 0, 0),
                CreatedBy = "System"
            },
            new ClosingBalanceModel
            {
                Id = MasterDataIds.ClosingBalances.CB07,
                TenantId = MasterDataIds.Tenants.Default,
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = "SOFA01",
                CompanyName = "SofaCraft Furnishings Private Limited",
                BranchId = MasterDataIds.Branches.SofaCraftHQ,
                BranchCode = "HO",
                BranchName = "Head Office Chennai",
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = "PRIM",
                LedgerName = "Primary Ledger",
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearCode = "FY2025-26",
                FiscalYearName = "Financial Year 2025-26",
                AccountingPeriodId = MasterDataIds.AccountingPeriods.May2025,
                AccountingPeriodCode = "MAY-2025",
                AccountingPeriodName = "May 2025",
                AccountId = MasterDataIds.Accounts.PettyCash,
                AccountCode = "5100",
                AccountName = "Rent Expense",
                OpeningDebit = 45000.00m,
                OpeningCredit = 0m,
                PeriodDebit = 10000.00m,
                PeriodCredit = 0m,
                ClosingDebit = 55000.00m,
                ClosingCredit = 0m,
                ClosingBalanceSide = BalanceSides.Debit,
                ClosingBalanceAmount = 55000.00m,
                BaseCurrencyId = MasterDataIds.Currencies.INR,
                BaseCurrencyCode = "INR",
                BaseCurrencyName = "Indian Rupee",
                CloseRunId = MasterDataIds.CloseRuns.CE02,
                CloseStatus = CloseStatuses.Verified,
                CalculatedAt = new DateTime(2025, 6, 1, 9, 0, 0),
                CalculatedBy = "FinanceController",
                AccountCodeSnapshot = "5100",
                AccountNameSnapshot = "Rent Expense",
                BranchCodeSnapshot = "HO",
                BranchNameSnapshot = "Head Office Chennai",
                CreatedAt = new DateTime(2025, 6, 1, 9, 0, 0),
                CreatedBy = "System"
            },
            // Mumbai Branch - April 2025
            new ClosingBalanceModel
            {
                Id = MasterDataIds.ClosingBalances.CB08,
                TenantId = MasterDataIds.Tenants.Default,
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = "SOFA01",
                CompanyName = "SofaCraft Furnishings Private Limited",
                BranchId = MasterDataIds.Branches.SofaCraftDubai,
                BranchCode = "MUM",
                BranchName = "Mumbai Branch",
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = "PRIM",
                LedgerName = "Primary Ledger",
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearCode = "FY2025-26",
                FiscalYearName = "Financial Year 2025-26",
                AccountingPeriodId = MasterDataIds.AccountingPeriods.Apr2025,
                AccountingPeriodCode = "APR-2025",
                AccountingPeriodName = "April 2025",
                AccountId = MasterDataIds.Accounts.HDFCBankAccount,
                AccountCode = "1100",
                AccountName = "Cash in Hand",
                OpeningDebit = 95000.00m,
                OpeningCredit = 0m,
                PeriodDebit = 42000.00m,
                PeriodCredit = 28000.00m,
                ClosingDebit = 109000.00m,
                ClosingCredit = 0m,
                ClosingBalanceSide = BalanceSides.Debit,
                ClosingBalanceAmount = 109000.00m,
                BaseCurrencyId = MasterDataIds.Currencies.INR,
                BaseCurrencyCode = "INR",
                BaseCurrencyName = "Indian Rupee",
                CloseRunId = MasterDataIds.CloseRuns.CE03,
                CloseStatus = CloseStatuses.Locked,
                CalculatedAt = new DateTime(2025, 5, 1, 9, 30, 0),
                CalculatedBy = "FinanceController",
                LockedAt = new DateTime(2025, 5, 1, 10, 30, 0),
                LockedBy = "FinanceController",
                AccountCodeSnapshot = "1100",
                AccountNameSnapshot = "Cash in Hand",
                BranchCodeSnapshot = "MUM",
                BranchNameSnapshot = "Mumbai Branch",
                CreatedAt = new DateTime(2025, 5, 1, 9, 30, 0),
                CreatedBy = "System"
            },
            // Mumbai Branch - Bank Account
            new ClosingBalanceModel
            {
                Id = MasterDataIds.ClosingBalances.CB09,
                TenantId = MasterDataIds.Tenants.Default,
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = "SOFA01",
                CompanyName = "SofaCraft Furnishings Private Limited",
                BranchId = MasterDataIds.Branches.SofaCraftDubai,
                BranchCode = "MUM",
                BranchName = "Mumbai Branch",
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = "PRIM",
                LedgerName = "Primary Ledger",
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearCode = "FY2025-26",
                FiscalYearName = "Financial Year 2025-26",
                AccountingPeriodId = MasterDataIds.AccountingPeriods.Apr2025,
                AccountingPeriodCode = "APR-2025",
                AccountingPeriodName = "April 2025",
                AccountId = MasterDataIds.Accounts.AccountsReceivable,
                AccountCode = "1200",
                AccountName = "Bank - HDFC Current",
                OpeningDebit = 380000.00m,
                OpeningCredit = 0m,
                PeriodDebit = 185000.00m,
                PeriodCredit = 95000.00m,
                ClosingDebit = 470000.00m,
                ClosingCredit = 0m,
                ClosingBalanceSide = BalanceSides.Debit,
                ClosingBalanceAmount = 470000.00m,
                BaseCurrencyId = MasterDataIds.Currencies.INR,
                BaseCurrencyCode = "INR",
                BaseCurrencyName = "Indian Rupee",
                CloseRunId = MasterDataIds.CloseRuns.CE03,
                CloseStatus = CloseStatuses.Locked,
                CalculatedAt = new DateTime(2025, 5, 1, 9, 30, 0),
                CalculatedBy = "FinanceController",
                LockedAt = new DateTime(2025, 5, 1, 10, 30, 0),
                LockedBy = "FinanceController",
                AccountCodeSnapshot = "1200",
                AccountNameSnapshot = "Bank - HDFC Current",
                BranchCodeSnapshot = "MUM",
                BranchNameSnapshot = "Mumbai Branch",
                CreatedAt = new DateTime(2025, 5, 1, 9, 30, 0),
                CreatedBy = "System"
            },
            // Pune Branch - April 2025
            new ClosingBalanceModel
            {
                Id = MasterDataIds.ClosingBalances.CB10,
                TenantId = MasterDataIds.Tenants.Default,
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = "SOFA01",
                CompanyName = "SofaCraft Furnishings Private Limited",
                BranchId = MasterDataIds.Branches.PlushComfortDelhi,
                BranchCode = "PUNE",
                BranchName = "Pune Branch",
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = "PRIM",
                LedgerName = "Primary Ledger",
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearCode = "FY2025-26",
                FiscalYearName = "Financial Year 2025-26",
                AccountingPeriodId = MasterDataIds.AccountingPeriods.Apr2025,
                AccountingPeriodCode = "APR-2025",
                AccountingPeriodName = "April 2025",
                AccountId = MasterDataIds.Accounts.HDFCBankAccount,
                AccountCode = "1100",
                AccountName = "Cash in Hand",
                OpeningDebit = 65000.00m,
                OpeningCredit = 0m,
                PeriodDebit = 32000.00m,
                PeriodCredit = 18000.00m,
                ClosingDebit = 79000.00m,
                ClosingCredit = 0m,
                ClosingBalanceSide = BalanceSides.Debit,
                ClosingBalanceAmount = 79000.00m,
                BaseCurrencyId = MasterDataIds.Currencies.INR,
                BaseCurrencyCode = "INR",
                BaseCurrencyName = "Indian Rupee",
                CloseRunId = MasterDataIds.CloseRuns.CE04,
                CloseStatus = CloseStatuses.Verified,
                CalculatedAt = new DateTime(2025, 5, 1, 10, 0, 0),
                CalculatedBy = "FinanceController",
                AccountCodeSnapshot = "1100",
                AccountNameSnapshot = "Cash in Hand",
                BranchCodeSnapshot = "PUNE",
                BranchNameSnapshot = "Pune Branch",
                CreatedAt = new DateTime(2025, 5, 1, 10, 0, 0),
                CreatedBy = "System"
            },
            // Hyderabad Branch - April 2025
            new ClosingBalanceModel
            {
                Id = MasterDataIds.ClosingBalances.CB11,
                TenantId = MasterDataIds.Tenants.Default,
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = "SOFA01",
                CompanyName = "SofaCraft Furnishings Private Limited",
                BranchId = MasterDataIds.Branches.VelvetRestPune,
                BranchCode = "HYD",
                BranchName = "Hyderabad Branch",
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = "PRIM",
                LedgerName = "Primary Ledger",
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearCode = "FY2025-26",
                FiscalYearName = "Financial Year 2025-26",
                AccountingPeriodId = MasterDataIds.AccountingPeriods.Apr2025,
                AccountingPeriodCode = "APR-2025",
                AccountingPeriodName = "April 2025",
                AccountId = MasterDataIds.Accounts.HDFCBankAccount,
                AccountCode = "1100",
                AccountName = "Cash in Hand",
                OpeningDebit = 55000.00m,
                OpeningCredit = 0m,
                PeriodDebit = 28000.00m,
                PeriodCredit = 15000.00m,
                ClosingDebit = 68000.00m,
                ClosingCredit = 0m,
                ClosingBalanceSide = BalanceSides.Debit,
                ClosingBalanceAmount = 68000.00m,
                BaseCurrencyId = MasterDataIds.Currencies.INR,
                BaseCurrencyCode = "INR",
                BaseCurrencyName = "Indian Rupee",
                CloseRunId = MasterDataIds.CloseRuns.CE05,
                CloseStatus = CloseStatuses.Locked,
                CalculatedAt = new DateTime(2025, 5, 1, 11, 0, 0),
                CalculatedBy = "FinanceController",
                LockedAt = new DateTime(2025, 5, 1, 12, 0, 0),
                LockedBy = "FinanceController",
                AccountCodeSnapshot = "1100",
                AccountNameSnapshot = "Cash in Hand",
                BranchCodeSnapshot = "HYD",
                BranchNameSnapshot = "Hyderabad Branch",
                CreatedAt = new DateTime(2025, 5, 1, 11, 0, 0),
                CreatedBy = "System"
            },
            // Kolkata Branch - April 2025
            new ClosingBalanceModel
            {
                Id = MasterDataIds.ClosingBalances.CB12,
                TenantId = MasterDataIds.Tenants.Default,
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = "SOFA01",
                CompanyName = "SofaCraft Furnishings Private Limited",
                BranchId = MasterDataIds.Branches.CozyCraftHyderabad,
                BranchCode = "KOL",
                BranchName = "Kolkata Branch",
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = "PRIM",
                LedgerName = "Primary Ledger",
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearCode = "FY2025-26",
                FiscalYearName = "Financial Year 2025-26",
                AccountingPeriodId = MasterDataIds.AccountingPeriods.Apr2025,
                AccountingPeriodCode = "APR-2025",
                AccountingPeriodName = "April 2025",
                AccountId = MasterDataIds.Accounts.HDFCBankAccount,
                AccountCode = "1100",
                AccountName = "Cash in Hand",
                OpeningDebit = 48000.00m,
                OpeningCredit = 0m,
                PeriodDebit = 25000.00m,
                PeriodCredit = 12000.00m,
                ClosingDebit = 61000.00m,
                ClosingCredit = 0m,
                ClosingBalanceSide = BalanceSides.Debit,
                ClosingBalanceAmount = 61000.00m,
                BaseCurrencyId = MasterDataIds.Currencies.INR,
                BaseCurrencyCode = "INR",
                BaseCurrencyName = "Indian Rupee",
                CloseRunId = MasterDataIds.CloseRuns.CE06,
                CloseStatus = CloseStatuses.Locked,
                CalculatedAt = new DateTime(2025, 5, 1, 12, 0, 0),
                CalculatedBy = "FinanceController",
                LockedAt = new DateTime(2025, 5, 1, 13, 0, 0),
                LockedBy = "FinanceController",
                AccountCodeSnapshot = "1100",
                AccountNameSnapshot = "Cash in Hand",
                BranchCodeSnapshot = "KOL",
                BranchNameSnapshot = "Kolkata Branch",
                CreatedAt = new DateTime(2025, 5, 1, 12, 0, 0),
                CreatedBy = "System"
            },
            // Ahmedabad Branch - April 2025
            new ClosingBalanceModel
            {
                Id = MasterDataIds.ClosingBalances.CB_0A,
                TenantId = MasterDataIds.Tenants.Default,
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = "SOFA01",
                CompanyName = "SofaCraft Furnishings Private Limited",
                BranchId = MasterDataIds.Branches.PremiumSeatingSG,
                BranchCode = "AMD",
                BranchName = "Ahmedabad Branch",
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = "PRIM",
                LedgerName = "Primary Ledger",
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearCode = "FY2025-26",
                FiscalYearName = "Financial Year 2025-26",
                AccountingPeriodId = MasterDataIds.AccountingPeriods.Apr2025,
                AccountingPeriodCode = "APR-2025",
                AccountingPeriodName = "April 2025",
                AccountId = MasterDataIds.Accounts.HDFCBankAccount,
                AccountCode = "1100",
                AccountName = "Cash in Hand",
                OpeningDebit = 72000.00m,
                OpeningCredit = 0m,
                PeriodDebit = 38000.00m,
                PeriodCredit = 20000.00m,
                ClosingDebit = 90000.00m,
                ClosingCredit = 0m,
                ClosingBalanceSide = BalanceSides.Debit,
                ClosingBalanceAmount = 90000.00m,
                BaseCurrencyId = MasterDataIds.Currencies.INR,
                BaseCurrencyCode = "INR",
                BaseCurrencyName = "Indian Rupee",
                CloseRunId = MasterDataIds.CloseRuns.CE07,
                CloseStatus = CloseStatuses.Verified,
                CalculatedAt = new DateTime(2025, 5, 1, 13, 0, 0),
                CalculatedBy = "FinanceController",
                AccountCodeSnapshot = "1100",
                AccountNameSnapshot = "Cash in Hand",
                BranchCodeSnapshot = "AMD",
                BranchNameSnapshot = "Ahmedabad Branch",
                CreatedAt = new DateTime(2025, 5, 1, 13, 0, 0),
                CreatedBy = "System"
            },
            // Jaipur Branch - April 2025
            new ClosingBalanceModel
            {
                Id = MasterDataIds.ClosingBalances.CB_0B,
                TenantId = MasterDataIds.Tenants.Default,
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = "SOFA01",
                CompanyName = "SofaCraft Furnishings Private Limited",
                BranchId = MasterDataIds.Branches.CloudSofaKolkata,
                BranchCode = "JAI",
                BranchName = "Jaipur Branch",
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = "PRIM",
                LedgerName = "Primary Ledger",
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearCode = "FY2025-26",
                FiscalYearName = "Financial Year 2025-26",
                AccountingPeriodId = MasterDataIds.AccountingPeriods.Apr2025,
                AccountingPeriodCode = "APR-2025",
                AccountingPeriodName = "April 2025",
                AccountId = MasterDataIds.Accounts.HDFCBankAccount,
                AccountCode = "1100",
                AccountName = "Cash in Hand",
                OpeningDebit = 42000.00m,
                OpeningCredit = 0m,
                PeriodDebit = 22000.00m,
                PeriodCredit = 10000.00m,
                ClosingDebit = 54000.00m,
                ClosingCredit = 0m,
                ClosingBalanceSide = BalanceSides.Debit,
                ClosingBalanceAmount = 54000.00m,
                BaseCurrencyId = MasterDataIds.Currencies.INR,
                BaseCurrencyCode = "INR",
                BaseCurrencyName = "Indian Rupee",
                CloseRunId = MasterDataIds.CloseRuns.CE08,
                CloseStatus = CloseStatuses.Locked,
                CalculatedAt = new DateTime(2025, 5, 1, 14, 0, 0),
                CalculatedBy = "FinanceController",
                LockedAt = new DateTime(2025, 5, 1, 15, 0, 0),
                LockedBy = "FinanceController",
                AccountCodeSnapshot = "1100",
                AccountNameSnapshot = "Cash in Hand",
                BranchCodeSnapshot = "JAI",
                BranchNameSnapshot = "Jaipur Branch",
                CreatedAt = new DateTime(2025, 5, 1, 14, 0, 0),
                CreatedBy = "System"
            },
            // Chandigarh Branch - April 2025
            new ClosingBalanceModel
            {
                Id = MasterDataIds.ClosingBalances.CB_0C,
                TenantId = MasterDataIds.Tenants.Default,
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = "SOFA01",
                CompanyName = "SofaCraft Furnishings Private Limited",
                BranchId = MasterDataIds.Branches.EliteLoungerAbuDhabi,
                BranchCode = "CHD",
                BranchName = "Chandigarh Branch",
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = "PRIM",
                LedgerName = "Primary Ledger",
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearCode = "FY2025-26",
                FiscalYearName = "Financial Year 2025-26",
                AccountingPeriodId = MasterDataIds.AccountingPeriods.Apr2025,
                AccountingPeriodCode = "APR-2025",
                AccountingPeriodName = "April 2025",
                AccountId = MasterDataIds.Accounts.HDFCBankAccount,
                AccountCode = "1100",
                AccountName = "Cash in Hand",
                OpeningDebit = 58000.00m,
                OpeningCredit = 0m,
                PeriodDebit = 30000.00m,
                PeriodCredit = 16000.00m,
                ClosingDebit = 72000.00m,
                ClosingCredit = 0m,
                ClosingBalanceSide = BalanceSides.Debit,
                ClosingBalanceAmount = 72000.00m,
                BaseCurrencyId = MasterDataIds.Currencies.INR,
                BaseCurrencyCode = "INR",
                BaseCurrencyName = "Indian Rupee",
                CloseRunId = MasterDataIds.CloseRuns.CE09,
                CloseStatus = CloseStatuses.Locked,
                CalculatedAt = new DateTime(2025, 5, 1, 15, 0, 0),
                CalculatedBy = "FinanceController",
                LockedAt = new DateTime(2025, 5, 1, 16, 0, 0),
                LockedBy = "FinanceController",
                AccountCodeSnapshot = "1100",
                AccountNameSnapshot = "Cash in Hand",
                BranchCodeSnapshot = "CHD",
                BranchNameSnapshot = "Chandigarh Branch",
                CreatedAt = new DateTime(2025, 5, 1, 15, 0, 0),
                CreatedBy = "System"
            },
            // Lucknow Branch - April 2025
            new ClosingBalanceModel
            {
                Id = Guid.Parse("cb000016-0016-0016-0016-000000000016"),
                TenantId = MasterDataIds.Tenants.Default,
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = "SOFA01",
                CompanyName = "SofaCraft Furnishings Private Limited",
                BranchId = MasterDataIds.Branches.SofaCraftUSA_SFO,
                BranchCode = "LKO",
                BranchName = "Lucknow Branch",
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = "PRIM",
                LedgerName = "Primary Ledger",
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearCode = "FY2025-26",
                FiscalYearName = "Financial Year 2025-26",
                AccountingPeriodId = MasterDataIds.AccountingPeriods.Apr2025,
                AccountingPeriodCode = "APR-2025",
                AccountingPeriodName = "April 2025",
                AccountId = MasterDataIds.Accounts.HDFCBankAccount,
                AccountCode = "1100",
                AccountName = "Cash in Hand",
                OpeningDebit = 38000.00m,
                OpeningCredit = 0m,
                PeriodDebit = 20000.00m,
                PeriodCredit = 8000.00m,
                ClosingDebit = 50000.00m,
                ClosingCredit = 0m,
                ClosingBalanceSide = BalanceSides.Debit,
                ClosingBalanceAmount = 50000.00m,
                BaseCurrencyId = MasterDataIds.Currencies.INR,
                BaseCurrencyCode = "INR",
                BaseCurrencyName = "Indian Rupee",
                CloseRunId = MasterDataIds.CloseRuns.CE10,
                CloseStatus = CloseStatuses.Verified,
                CalculatedAt = new DateTime(2025, 5, 1, 16, 0, 0),
                CalculatedBy = "FinanceController",
                AccountCodeSnapshot = "1100",
                AccountNameSnapshot = "Cash in Hand",
                BranchCodeSnapshot = "LKO",
                BranchNameSnapshot = "Lucknow Branch",
                CreatedAt = new DateTime(2025, 5, 1, 16, 0, 0),
                CreatedBy = "System"
            },
            // Kochi Branch - April 2025
            new ClosingBalanceModel
            {
                Id = Guid.Parse("cb000017-0017-0017-0017-000000000017"),
                TenantId = MasterDataIds.Tenants.Default,
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = "SOFA01",
                CompanyName = "SofaCraft Furnishings Private Limited",
                BranchId = MasterDataIds.Branches.OakNestBengaluru,
                BranchCode = "COK",
                BranchName = "Kochi Branch",
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = "PRIM",
                LedgerName = "Primary Ledger",
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearCode = "FY2025-26",
                FiscalYearName = "Financial Year 2025-26",
                AccountingPeriodId = MasterDataIds.AccountingPeriods.Apr2025,
                AccountingPeriodCode = "APR-2025",
                AccountingPeriodName = "April 2025",
                AccountId = MasterDataIds.Accounts.HDFCBankAccount,
                AccountCode = "1100",
                AccountName = "Cash in Hand",
                OpeningDebit = 85000.00m,
                OpeningCredit = 0m,
                PeriodDebit = 45000.00m,
                PeriodCredit = 25000.00m,
                ClosingDebit = 105000.00m,
                ClosingCredit = 0m,
                ClosingBalanceSide = BalanceSides.Debit,
                ClosingBalanceAmount = 105000.00m,
                BaseCurrencyId = MasterDataIds.Currencies.INR,
                BaseCurrencyCode = "INR",
                BaseCurrencyName = "Indian Rupee",
                CloseRunId = MasterDataIds.CloseRuns.CE11,
                CloseStatus = CloseStatuses.Locked,
                CalculatedAt = new DateTime(2025, 5, 1, 17, 0, 0),
                CalculatedBy = "FinanceController",
                LockedAt = new DateTime(2025, 5, 1, 18, 0, 0),
                LockedBy = "FinanceController",
                AccountCodeSnapshot = "1100",
                AccountNameSnapshot = "Cash in Hand",
                BranchCodeSnapshot = "COK",
                BranchNameSnapshot = "Kochi Branch",
                CreatedAt = new DateTime(2025, 5, 1, 17, 0, 0),
                CreatedBy = "System"
            }
        };

        #endregion

        #region Account Seed Data & Methods

        // Sample Accounts for Opening Balance screens
        // In a real application, these would come from Chart of Accounts
        private static readonly List<AccountViewModel> _seedAccounts = new()
        {
            // Assets
            new AccountViewModel { Id = MasterDataIds.Accounts.PettyCash, AccountCode = "1100", AccountName = "Cash and Cash Equivalents", AccountType = "Asset", AccountNature = "Asset", IsBalanceSheetAccount = true, IsPostable = true, Status = "Active" },
            new AccountViewModel { Id = MasterDataIds.Accounts.HDFCBankAccount, AccountCode = "1200", AccountName = "Bank - HDFC Current Account", AccountType = "Asset", AccountNature = "Asset", IsBalanceSheetAccount = true, IsPostable = true, Status = "Active" },
            new AccountViewModel { Id = MasterDataIds.Accounts.AccountsReceivable, AccountCode = "1300", AccountName = "Accounts Receivable", AccountType = "Asset", AccountNature = "Asset", IsBalanceSheetAccount = true, IsPostable = true, Status = "Active" },
            new AccountViewModel { Id = MasterDataIds.Accounts.AccountsPayable, AccountCode = "1400", AccountName = "Inventory - Raw Materials", AccountType = "Asset", AccountNature = "Asset", IsBalanceSheetAccount = true, IsPostable = true, Status = "Active" },
            new AccountViewModel { Id = MasterDataIds.Accounts.ShareCapital, AccountCode = "1500", AccountName = "Property, Plant & Equipment", AccountType = "Asset", AccountNature = "Asset", IsBalanceSheetAccount = true, IsPostable = true, Status = "Active" },
            new AccountViewModel { Id = MasterDataIds.Accounts.GSTPayable, AccountCode = "1600", AccountName = "Accumulated Depreciation", AccountType = "Asset", AccountNature = "Asset", IsBalanceSheetAccount = true, IsPostable = true, Status = "Active" },
            new AccountViewModel { Id = MasterDataIds.Accounts.TDSPayable, AccountCode = "1700", AccountName = "Prepaid Expenses", AccountType = "Asset", AccountNature = "Asset", IsBalanceSheetAccount = true, IsPostable = true, Status = "Active" },
            
            // Liabilities
            new AccountViewModel { Id = MasterDataIds.Accounts.AccountsPayable, AccountCode = "2100", AccountName = "Accounts Payable", AccountType = "Liability", AccountNature = "Liability", IsBalanceSheetAccount = true, IsPostable = true, Status = "Active" },
            new AccountViewModel { Id = MasterDataIds.Accounts.GSTPayable, AccountCode = "2200", AccountName = "Accrued Expenses", AccountType = "Liability", AccountNature = "Liability", IsBalanceSheetAccount = true, IsPostable = true, Status = "Active" },
            new AccountViewModel { Id = MasterDataIds.Accounts.TDSPayable, AccountCode = "2300", AccountName = "Short-term Loans Payable", AccountType = "Liability", AccountNature = "Liability", IsBalanceSheetAccount = true, IsPostable = true, Status = "Active" },
            new AccountViewModel { Id = MasterDataIds.Accounts.CostOfMaterials, AccountCode = "2400", AccountName = "Long-term Debt", AccountType = "Liability", AccountNature = "Liability", IsBalanceSheetAccount = true, IsPostable = true, Status = "Active" },
            new AccountViewModel { Id = MasterDataIds.Accounts.SalariesWages, AccountCode = "2500", AccountName = "Deferred Revenue", AccountType = "Liability", AccountNature = "Liability", IsBalanceSheetAccount = true, IsPostable = true, Status = "Active" },
            
            // Equity
            new AccountViewModel { Id = MasterDataIds.Accounts.ShareCapital, AccountCode = "3100", AccountName = "Share Capital", AccountType = "Equity", AccountNature = "Equity", IsBalanceSheetAccount = true, IsPostable = true, Status = "Active" },
            new AccountViewModel { Id = MasterDataIds.Accounts.RetainedEarnings, AccountCode = "3200", AccountName = "Retained Earnings", AccountType = "Equity", AccountNature = "Equity", IsBalanceSheetAccount = true, IsPostable = true, Status = "Active" },
            new AccountViewModel { Id = MasterDataIds.Accounts.RetainedEarnings, AccountCode = "3300", AccountName = "Reserves", AccountType = "Equity", AccountNature = "Equity", IsBalanceSheetAccount = true, IsPostable = true, Status = "Active" },
            
            // Revenue (Income Statement - not Balance Sheet)
            new AccountViewModel { Id = MasterDataIds.Accounts.SalesRevenue, AccountCode = "4100", AccountName = "Sales Revenue", AccountType = "Revenue", AccountNature = "Revenue", IsBalanceSheetAccount = false, IsPostable = true, Status = "Active" },
            new AccountViewModel { Id = MasterDataIds.Accounts.ServiceRevenue, AccountCode = "4200", AccountName = "Service Revenue", AccountType = "Revenue", AccountNature = "Revenue", IsBalanceSheetAccount = false, IsPostable = true, Status = "Active" },
            new AccountViewModel { Id = MasterDataIds.Accounts.ServiceRevenue, AccountCode = "4300", AccountName = "Interest Income", AccountType = "Revenue", AccountNature = "Revenue", IsBalanceSheetAccount = false, IsPostable = true, Status = "Active" },
            
            // Expenses (Income Statement - not Balance Sheet)
            new AccountViewModel { Id = MasterDataIds.Accounts.CostOfMaterials, AccountCode = "5100", AccountName = "Rent Expense", AccountType = "Expense", AccountNature = "Expense", IsBalanceSheetAccount = false, IsPostable = true, Status = "Active" },
            new AccountViewModel { Id = MasterDataIds.Accounts.SalariesWages, AccountCode = "5200", AccountName = "Salaries Expense", AccountType = "Expense", AccountNature = "Expense", IsBalanceSheetAccount = false, IsPostable = true, Status = "Active" },
            new AccountViewModel { Id = MasterDataIds.Accounts.RentExpense, AccountCode = "5300", AccountName = "Utilities Expense", AccountType = "Expense", AccountNature = "Expense", IsBalanceSheetAccount = false, IsPostable = true, Status = "Active" },
            new AccountViewModel { Id = MasterDataIds.Accounts.UtilitiesExpense, AccountCode = "5400", AccountName = "Depreciation Expense", AccountType = "Expense", AccountNature = "Expense", IsBalanceSheetAccount = false, IsPostable = true, Status = "Active" },
            new AccountViewModel { Id = MasterDataIds.Accounts.UtilitiesExpense, AccountCode = "5500", AccountName = "Cost of Goods Sold", AccountType = "Expense", AccountNature = "Expense", IsBalanceSheetAccount = false, IsPostable = true, Status = "Active" }
        };

        public List<AccountViewModel> GetAllAccounts() => _seedAccounts.Where(a => a.IsActive).OrderBy(a => a.AccountCode).ToList();

        public List<AccountViewModel> GetBalanceSheetAccounts() => _seedAccounts.Where(a => a.IsActive && a.IsBalanceSheetAccount).OrderBy(a => a.AccountCode).ToList();

        public List<AccountViewModel> GetIncomeStatementAccounts() => _seedAccounts.Where(a => a.IsActive && !a.IsBalanceSheetAccount).OrderBy(a => a.AccountCode).ToList();

        public AccountViewModel? GetAccountById(Guid id) => _seedAccounts.FirstOrDefault(a => a.Id == id);

        #endregion

        #region FiscalYear & AccountingPeriod Seed Data & Methods

        // Sample Fiscal Years for Opening/Closing Balance screens
        private static readonly List<FiscalYearModel> _seedFiscalYears = new()
        {
            new FiscalYearModel
            {
                Id = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearCode = "FY2025-26",
                FiscalYearName = "Financial Year 2025-26",
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyName = "SofaCraft Furnishings Private Limited",
                Status = FiscalYearStatus.Open,
                StartDate = new DateTime(2025, 4, 1),
                EndDate = new DateTime(2026, 3, 31),
                PeriodType = FiscalPeriodType.Monthly,
                NumberOfPeriods = 12
            },
            new FiscalYearModel
            {
                Id = MasterDataIds.FiscalYears.FY2024_25,
                FiscalYearCode = "FY2024-25",
                FiscalYearName = "Financial Year 2024-25",
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyName = "SofaCraft Furnishings Private Limited",
                Status = FiscalYearStatus.Closed,
                StartDate = new DateTime(2024, 4, 1),
                EndDate = new DateTime(2025, 3, 31),
                PeriodType = FiscalPeriodType.Monthly,
                NumberOfPeriods = 12
            }
        };

        // Sample Accounting Periods for Opening/Closing Balance screens
        private static readonly List<AccountingPeriodModel> _seedAccountingPeriods = new()
        {
            new AccountingPeriodModel
            {
                Id = MasterDataIds.AccountingPeriods.Apr2025,
                PeriodCode = "APR-2025",
                PeriodName = "April 2025",
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                CompanyId = MasterDataIds.Companies.SofaCraft,
                PeriodNumber = 1,
                PeriodType = AccountingPeriodType.Normal,
                StartDate = new DateTime(2025, 4, 1),
                EndDate = new DateTime(2025, 4, 30),
                Status = AccountingPeriodStatus.Closed,
                IsCurrentPeriod = false
            },
            new AccountingPeriodModel
            {
                Id = MasterDataIds.AccountingPeriods.May2025,
                PeriodCode = "MAY-2025",
                PeriodName = "May 2025",
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                CompanyId = MasterDataIds.Companies.SofaCraft,
                PeriodNumber = 2,
                PeriodType = AccountingPeriodType.Normal,
                StartDate = new DateTime(2025, 5, 1),
                EndDate = new DateTime(2025, 5, 31),
                Status = AccountingPeriodStatus.Closed,
                IsCurrentPeriod = false
            },
            new AccountingPeriodModel
            {
                Id = MasterDataIds.AccountingPeriods.Apr2024,
                PeriodCode = "JUN-2025",
                PeriodName = "June 2025",
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                CompanyId = MasterDataIds.Companies.SofaCraft,
                PeriodNumber = 3,
                PeriodType = AccountingPeriodType.Normal,
                StartDate = new DateTime(2025, 6, 1),
                EndDate = new DateTime(2025, 6, 30),
                Status = AccountingPeriodStatus.Open,
                IsCurrentPeriod = true
            }
        };

        public List<FiscalYearModel> GetAllFiscalYears() => _seedFiscalYears.OrderByDescending(fy => fy.StartDate).ToList();

        public FiscalYearModel? GetFiscalYearById(Guid id) => _seedFiscalYears.FirstOrDefault(fy => fy.Id == id);

        public List<AccountingPeriodModel> GetAllAccountingPeriods() => _seedAccountingPeriods.OrderBy(ap => ap.StartDate).ToList();

        public AccountingPeriodModel? GetAccountingPeriodById(Guid id) => _seedAccountingPeriods.FirstOrDefault(ap => ap.Id == id);

        public List<AccountingPeriodModel> GetAccountingPeriodsByFiscalYear(Guid fiscalYearId) => 
            _seedAccountingPeriods.Where(ap => ap.FiscalYearId == fiscalYearId).OrderBy(ap => ap.PeriodNumber).ToList();

        #endregion

        #region Ledger Methods

        public List<LedgerModel> GetAllLedgers() =>
            _ledgers.Where(l => !l.IsDeleted).OrderBy(l => l.CompanyCode).ThenBy(l => l.LedgerCode).ToList();

        public LedgerModel? GetLedgerById(Guid id) =>
            _ledgers.FirstOrDefault(l => l.Id == id && !l.IsDeleted);

        public List<LedgerModel> GetLedgersByCompany(Guid companyId) =>
            _ledgers.Where(l => l.CompanyId == companyId && !l.IsDeleted)
                .OrderBy(l => l.LedgerCode).ToList();

        public void AddLedger(LedgerModel ledger)
        {
            ledger.Id = Guid.NewGuid();
            ledger.CreatedAt = DateTime.Now;
            ledger.LedgerCode = ledger.LedgerCode?.ToUpper()?.Trim() ?? "";
            _ledgers.Add(ledger);
        }

        public void UpdateLedger(LedgerModel ledger)
        {
            var existing = _ledgers.FirstOrDefault(l => l.Id == ledger.Id);
            if (existing != null)
            {
                var index = _ledgers.IndexOf(existing);
                ledger.UpdatedAt = DateTime.Now;
                ledger.LedgerCode = ledger.LedgerCode?.ToUpper()?.Trim() ?? "";
                _ledgers[index] = ledger;
            }
        }

        public bool CanEditLedger(Guid id)
        {
            var ledger = _ledgers.FirstOrDefault(l => l.Id == id);
            return ledger?.LockStatus == "Unlocked";
        }

        public void ActivateLedger(Guid id)
        {
            var ledger = _ledgers.FirstOrDefault(l => l.Id == id);
            if (ledger != null)
            {
                ledger.Status = LedgerStatus.Active;
                ledger.UpdatedAt = DateTime.Now;
            }
        }

        public void DeactivateLedger(Guid id, string? reason = null)
        {
            var ledger = _ledgers.FirstOrDefault(l => l.Id == id);
            if (ledger != null)
            {
                ledger.Status = LedgerStatus.Inactive;
                ledger.LockReason = reason;
                ledger.UpdatedAt = DateTime.Now;
            }
        }

        public void DeleteLedger(Guid id)
        {
            var ledger = _ledgers.FirstOrDefault(l => l.Id == id);
            if (ledger != null)
            {
                ledger.IsDeleted = true;
                ledger.DeletedAt = DateTime.Now;
            }
        }

        public bool CanDeactivateLedger(Guid id)
        {
            // For demo: can deactivate if not locked after posting
            var ledger = _ledgers.FirstOrDefault(l => l.Id == id);
            if (ledger == null) return false;
            return ledger.LockStatus != LockStatuses.LockedAfterPosting;
        }

        public bool CanDeleteLedger(Guid id)
        {
            // For demo: can delete if draft or not locked after posting
            var ledger = _ledgers.FirstOrDefault(l => l.Id == id);
            if (ledger == null) return false;
            return ledger.Status == LedgerStatus.Draft || ledger.LockStatus != LockStatuses.LockedAfterPosting;
        }

        // Unique filter values for dropdowns
        public List<(Guid? Id, string Code, string Name)> GetLedgerCompanies() =>
            _ledgers.Where(l => !l.IsDeleted)
                .Select(l => (l.CompanyId, l.CompanyCode ?? "", l.CompanyName ?? ""))
                .Distinct().ToList();

        #endregion

        #region Opening Balance Methods

        public List<OpeningBalanceModel> GetAllOpeningBalances() =>
            _openingBalances.Where(ob => !ob.IsDeleted)
                .OrderByDescending(ob => ob.OpeningDate).ToList();

        public OpeningBalanceModel? GetOpeningBalanceById(Guid id) =>
            _openingBalances.FirstOrDefault(ob => ob.Id == id && !ob.IsDeleted);

        public List<OpeningBalanceModel> GetOpeningBalancesByCompany(Guid companyId) =>
            _openingBalances.Where(ob => ob.CompanyId == companyId && !ob.IsDeleted)
                .OrderByDescending(ob => ob.OpeningDate).ToList();

        public List<OpeningBalanceModel> GetOpeningBalancesByBranch(Guid branchId) =>
            _openingBalances.Where(ob => ob.BranchId == branchId && !ob.IsDeleted)
                .OrderByDescending(ob => ob.OpeningDate).ToList();

        private int _obCounter = 5;

        public void AddOpeningBalance(OpeningBalanceModel ob)
        {
            ob.Id = Guid.NewGuid();
            ob.OpeningBalanceNumber = $"OB-{DateTime.Now.Year}-{_obCounter++:D5}";
            ob.CreatedAt = DateTime.Now;
            ob.Status = OpeningBalanceStatus.Draft;
            _openingBalances.Add(ob);
        }

        public void CreateOpeningBalance(OpeningBalanceModel ob)
        {
            if (ob.Id == Guid.Empty)
                ob.Id = Guid.NewGuid();
            if (string.IsNullOrEmpty(ob.OpeningBalanceNumber))
                ob.OpeningBalanceNumber = $"OB-{DateTime.Now.Year}-{_obCounter++:D5}";
            ob.CreatedAt = DateTime.Now;
            if (string.IsNullOrEmpty(ob.Status))
                ob.Status = OpeningBalanceStatus.Draft;
            _openingBalances.Add(ob);
        }

        public void UpdateOpeningBalance(OpeningBalanceModel ob)
        {
            var existing = _openingBalances.FirstOrDefault(o => o.Id == ob.Id);
            if (existing != null)
            {
                var index = _openingBalances.IndexOf(existing);
                ob.UpdatedAt = DateTime.Now;
                _openingBalances[index] = ob;
            }
        }

        public void SubmitOpeningBalance(Guid id)
        {
            var ob = _openingBalances.FirstOrDefault(o => o.Id == id);
            if (ob != null && ob.Status == OpeningBalanceStatus.Draft && ob.IsBalanced)
            {
                ob.Status = OpeningBalanceStatus.Submitted;
                ob.UpdatedAt = DateTime.Now;
            }
        }

        public void ApproveOpeningBalance(Guid id, string approvedBy)
        {
            var ob = _openingBalances.FirstOrDefault(o => o.Id == id);
            if (ob != null && ob.Status == OpeningBalanceStatus.Submitted)
            {
                ob.Status = OpeningBalanceStatus.Approved;
                ob.ApprovedBy = approvedBy;
                ob.ApprovedAt = DateTime.Now;
                ob.UpdatedAt = DateTime.Now;
            }
        }

        public void PostOpeningBalance(Guid id, string postedBy)
        {
            var ob = _openingBalances.FirstOrDefault(o => o.Id == id);
            if (ob != null && ob.Status == OpeningBalanceStatus.Approved)
            {
                ob.Status = OpeningBalanceStatus.Posted;
                ob.PostedBy = postedBy;
                ob.PostedAt = DateTime.Now;
                ob.PostingReference = $"JE-OB-{DateTime.Now.Year}-{ob.Id.ToString()[..8].ToUpper()}";
                ob.UpdatedAt = DateTime.Now;
            }
        }

        public void CancelOpeningBalance(Guid id)
        {
            var ob = _openingBalances.FirstOrDefault(o => o.Id == id);
            if (ob != null && ob.Status != OpeningBalanceStatus.Posted)
            {
                ob.Status = OpeningBalanceStatus.Cancelled;
                ob.UpdatedAt = DateTime.Now;
            }
        }

        public void DeleteOpeningBalance(Guid id)
        {
            var ob = _openingBalances.FirstOrDefault(o => o.Id == id);
            if (ob != null && ob.Status == OpeningBalanceStatus.Draft)
            {
                ob.IsDeleted = true;
                ob.DeletedAt = DateTime.Now;
            }
        }

        // Unique filter values for dropdowns
        public List<(Guid Id, string Code, string Name)> GetOBCompanies() =>
            _openingBalances.Where(ob => !ob.IsDeleted)
                .Select(ob => (ob.CompanyId, ob.CompanyCode ?? "", ob.CompanyName ?? ""))
                .Distinct().ToList();

        public List<(Guid Id, string Code, string Name)> GetOBBranches() =>
            _openingBalances.Where(ob => !ob.IsDeleted)
                .Select(ob => (ob.BranchId, ob.BranchCode ?? "", ob.BranchName ?? ""))
                .Distinct().ToList();

        public List<(Guid Id, string Code, string Name)> GetOBFiscalYears() =>
            _openingBalances.Where(ob => !ob.IsDeleted)
                .Select(ob => (ob.FiscalYearId, ob.FiscalYearCode ?? "", ob.FiscalYearName ?? ""))
                .Distinct().ToList();

        // Summary totals
        public (decimal TotalDebit, decimal TotalCredit) GetOBTotals(OpeningBalanceModel ob)
        {
            return (ob.TotalDebit, ob.TotalCredit);
        }

        #endregion

        #region Closing Balance Methods

        public List<ClosingBalanceModel> GetAllClosingBalances() =>
            _closingBalances.Where(cb => !cb.IsDeleted)
                .OrderBy(cb => cb.AccountingPeriodCode).ThenBy(cb => cb.AccountCode).ToList();

        public ClosingBalanceModel? GetClosingBalanceById(Guid id) =>
            _closingBalances.FirstOrDefault(cb => cb.Id == id && !cb.IsDeleted);

        public List<ClosingBalanceModel> GetClosingBalancesByPeriod(Guid periodId) =>
            _closingBalances.Where(cb => cb.AccountingPeriodId == periodId && !cb.IsDeleted)
                .OrderBy(cb => cb.AccountCode).ToList();

        public List<ClosingBalanceModel> GetClosingBalancesByCompany(Guid companyId) =>
            _closingBalances.Where(cb => cb.CompanyId == companyId && !cb.IsDeleted)
                .OrderBy(cb => cb.AccountingPeriodCode).ThenBy(cb => cb.AccountCode).ToList();

        public List<ClosingBalanceModel> GetClosingBalancesByBranch(Guid branchId) =>
            _closingBalances.Where(cb => cb.BranchId == branchId && !cb.IsDeleted)
                .OrderBy(cb => cb.AccountingPeriodCode).ThenBy(cb => cb.AccountCode).ToList();

        // Unique filter values for dropdowns
        public List<(Guid Id, string Code, string Name)> GetCBCompanies() =>
            _closingBalances.Where(cb => !cb.IsDeleted)
                .Select(cb => (cb.CompanyId, cb.CompanyCode ?? "", cb.CompanyName ?? ""))
                .Distinct().ToList();

        public List<(Guid Id, string Code, string Name)> GetCBBranches() =>
            _closingBalances.Where(cb => !cb.IsDeleted)
                .Select(cb => (cb.BranchId, cb.BranchCode ?? "", cb.BranchName ?? ""))
                .Distinct().ToList();

        public List<(Guid Id, string Code, string Name)> GetCBPeriods() =>
            _closingBalances.Where(cb => !cb.IsDeleted)
                .Select(cb => (cb.AccountingPeriodId, cb.AccountingPeriodCode ?? "", cb.AccountingPeriodName ?? ""))
                .Distinct().ToList();

        public List<(Guid Id, string Code, string Name)> GetCBAccounts() =>
            _closingBalances.Where(cb => !cb.IsDeleted)
                .Select(cb => (cb.AccountId, cb.AccountCode ?? "", cb.AccountName ?? ""))
                .Distinct().ToList();

        // Summary calculations
        public (decimal TotalDebit, decimal TotalCredit) GetCBTotals(List<ClosingBalanceModel> balances)
        {
            var totalDebit = balances.Sum(cb => cb.ClosingDebit);
            var totalCredit = balances.Sum(cb => cb.ClosingCredit);
            return (totalDebit, totalCredit);
        }

        #endregion
    }
}
