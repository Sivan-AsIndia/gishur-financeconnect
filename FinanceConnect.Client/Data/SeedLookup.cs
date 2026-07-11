namespace FinanceConnect.Client.Data
{
    /// <summary>
    /// Single source of truth for resolving Code / Name strings from master-data GUIDs.
    ///
    /// Every seed-data class uses this helper instead of hard-coding display strings.
    /// The dictionaries are built lazily from the corresponding master seed-data classes
    /// (CountrySeedData, CurrencySeedData, CompanySeedData, BranchService, COADataService,
    ///  LedgerSeedData, FiscalYearService, AccountingPeriodService).
    /// </summary>
    public static class SeedLookup
    {
        // ─────────── Company ───────────
        private static readonly Lazy<Dictionary<Guid, (string Code, string Name)>> _companies = new(BuildCompanies);

        public static string CompanyCode(Guid id) => _companies.Value.TryGetValue(id, out var v) ? v.Code : "???";
        public static string CompanyName(Guid id) => _companies.Value.TryGetValue(id, out var v) ? v.Name : "Unknown Company";

        private static Dictionary<Guid, (string Code, string Name)> BuildCompanies()
        {
            var countries  = CountrySeedData.GetSeedData();
            var currencies = CurrencySeedData.GetSeedData();
            var states     = StateProvinceSeedData.GetSeedData(countries);
            var cities     = CitySeedData.GetSeedData(countries, states);
            var companies  = CompanySeedData.GetSeedData(countries, states, cities, currencies);
            return companies.ToDictionary(c => c.Id, c => (c.CompanyCode, c.LegalName));
        }

        // ─────────── Currency ───────────
        private static readonly Lazy<Dictionary<Guid, (string Code, string Name)>> _currencies = new(BuildCurrencies);

        public static string CurrencyCode(Guid id) => _currencies.Value.TryGetValue(id, out var v) ? v.Code : "???";
        public static string CurrencyName(Guid id) => _currencies.Value.TryGetValue(id, out var v) ? v.Name : "Unknown Currency";

        private static Dictionary<Guid, (string Code, string Name)> BuildCurrencies()
        {
            var currencies = CurrencySeedData.GetSeedData();
            return currencies.ToDictionary(c => c.Id, c => (c.CurrencyCode, c.CurrencyName));
        }

        // ─────────── Branch ───────────
        // Built from BranchService seed definitions (BranchService.SeedBranches).
        // We replicate the essential Id → (Code, Name) here to avoid a circular dependency.
        private static readonly Lazy<Dictionary<Guid, (string Code, string Name)>> _branches = new(BuildBranches);

        public static string BranchCode(Guid id) => _branches.Value.TryGetValue(id, out var v) ? v.Code : "???";
        public static string BranchName(Guid id) => _branches.Value.TryGetValue(id, out var v) ? v.Name : "Unknown Branch";

        private static Dictionary<Guid, (string Code, string Name)> BuildBranches()
        {
            // Mirror of BranchService.SeedBranches() — kept in sync manually.
            return new Dictionary<Guid, (string Code, string Name)>
            {
                // SofaCraft Furnishings
                [MasterDataIds.Branches.SofaCraftHQ]        = ("HO",      "SofaCraft Head Office & Factory - Chennai"),
                [MasterDataIds.Branches.SofaCraftBengaluru]  = ("BLR",     "SofaCraft Experience Store - Bengaluru"),
                [MasterDataIds.Branches.SofaCraftDubai]      = ("DXB",     "SofaCraft Sales Office - Dubai"),
                // SofaCraft USA
                [MasterDataIds.Branches.SofaCraftUSA_SFO]    = ("SFO",     "SofaCraft Retail HQ - San Francisco"),
                [MasterDataIds.Branches.SofaCraftUSA_DAL]    = ("DAL",     "SofaCraft Retail Warehouse - Dallas"),
                // OakNest
                [MasterDataIds.Branches.OakNestBengaluru]    = ("BLR-HO",  "OakNest Studio - Bengaluru"),
                // UrbanLoft
                [MasterDataIds.Branches.UrbanLoftMumbai]     = ("BOM-HO",  "UrbanLoft HQ - Mumbai"),
                // DesertDune
                [MasterDataIds.Branches.DesertDuneDubai]     = ("DXB-HO",  "DesertDune HQ - Dubai"),
                // PlushComfort
                [MasterDataIds.Branches.PlushComfortDelhi]   = ("DEL-HO",  "PlushComfort Head Office - Delhi"),
                [MasterDataIds.Branches.PlushComfortMH]      = ("GGN-FAC", "PlushComfort Factory - MH"),
                // VelvetRest
                [MasterDataIds.Branches.VelvetRestPune]      = ("PUN-HO",  "VelvetRest Head Office - Pune"),
                [MasterDataIds.Branches.VelvetRestMumbai]    = ("BOM-SH",  "VelvetRest Showroom - Mumbai"),
                // CozyCraft
                [MasterDataIds.Branches.CozyCraftHyderabad]  = ("HYD-HO",  "CozyCraft Head Office - Hyderabad"),
                [MasterDataIds.Branches.CozyCraftWarehouse]  = ("SEC-EXP", "CozyCraft Experience Center - Secunderabad"),
                // PremiumSeating
                [MasterDataIds.Branches.PremiumSeatingSG]    = ("SIN-HQ",  "PremiumSeating HQ - Singapore"),
                [MasterDataIds.Branches.PremiumSeatingWH]    = ("JUR-FAC", "PremiumSeating Factory - Jurong"),
                // CloudSofa
                [MasterDataIds.Branches.CloudSofaKolkata]    = ("CCU-HO",  "CloudSofa Head Office - Kolkata"),
                [MasterDataIds.Branches.CloudSofaWarehouse]  = ("HWH-FAC", "CloudSofa Factory - Howrah"),
                // EliteLoungers
                [MasterDataIds.Branches.EliteLoungerAbuDhabi]= ("AUH-HQ",  "EliteLoungers HQ - Abu Dhabi"),
                [MasterDataIds.Branches.EliteLoungerDubai]   = ("DXB-SH",  "EliteLoungers Showroom - Dubai"),
            };
        }

        // ─────────── Ledger ───────────
        private static readonly Lazy<Dictionary<Guid, (string Code, string Name)>> _ledgers = new(BuildLedgers);

        public static string LedgerCode(Guid id) => _ledgers.Value.TryGetValue(id, out var v) ? v.Code : "???";
        public static string LedgerName(Guid id) => _ledgers.Value.TryGetValue(id, out var v) ? v.Name : "Unknown Ledger";

        private static Dictionary<Guid, (string Code, string Name)> BuildLedgers()
        {
            return new Dictionary<Guid, (string Code, string Name)>
            {
                [MasterDataIds.Ledgers.PrimaryLedger]         = ("PRIM",      "Primary Ledger"),
                [MasterDataIds.Ledgers.ManagementLedger]      = ("MGMT",      "Management Ledger"),
                [MasterDataIds.Ledgers.ReliancePrimary]       = ("PRIM",      "Primary Ledger"),
                [MasterDataIds.Ledgers.UrbanLoftPrimary]      = ("PRIM",      "Primary Ledger"),
                [MasterDataIds.Ledgers.SofaCraftUSAPrimary]   = ("PRIM",      "Primary Ledger"),
                [MasterDataIds.Ledgers.OakNestPrimary]        = ("PRIM",      "Primary Ledger"),
                [MasterDataIds.Ledgers.DesertDunePrimary]     = ("PRIM",      "Primary Ledger"),
                [MasterDataIds.Ledgers.PlushComfortPrimary]   = ("PRIM",      "Primary Ledger"),
                [MasterDataIds.Ledgers.VelvetRestPrimary]     = ("PRIM",      "Primary Ledger"),
                [MasterDataIds.Ledgers.CozyCraftPrimary]      = ("PRIM",      "Primary Ledger"),
                [MasterDataIds.Ledgers.PremiumSeatingPrimary] = ("PRIM",      "Primary Ledger"),
                [MasterDataIds.Ledgers.CloudSofaPrimary]      = ("PRIM",      "Primary Ledger"),
                [MasterDataIds.Ledgers.EliteLoungersPrimary]  = ("PRIM",      "Primary Ledger"),
            };
        }

        // ─────────── Account (Chart of Accounts) ───────────
        // Mirror of COADataService.SeedAll() — Id → (AccountCode, AccountName).
        private static readonly Lazy<Dictionary<Guid, (string Code, string Name)>> _accounts = new(BuildAccounts);

        public static string AccountCode(Guid id) => _accounts.Value.TryGetValue(id, out var v) ? v.Code : "???";
        public static string AccountName(Guid id) => _accounts.Value.TryGetValue(id, out var v) ? v.Name : "Unknown Account";

        private static Dictionary<Guid, (string Code, string Name)> BuildAccounts()
        {
            return new Dictionary<Guid, (string Code, string Name)>
            {
                // SofaCraft (COA-SF-2025)
                [MasterDataIds.Accounts.PettyCash]          = ("1001", "Petty Cash"),
                [MasterDataIds.Accounts.HDFCBankAccount]    = ("1002", "HDFC Bank - Current Account"),
                [MasterDataIds.Accounts.AccountsReceivable] = ("1003", "Accounts Receivable"),
                [MasterDataIds.Accounts.FurnitureFixtures]  = ("1100", "Furniture & Fixtures"),
                [MasterDataIds.Accounts.AccountsPayable]    = ("2001", "Accounts Payable"),
                [MasterDataIds.Accounts.GSTPayable]         = ("2002", "GST Payable"),
                [MasterDataIds.Accounts.TDSPayable]         = ("2003", "TDS Payable"),
                [MasterDataIds.Accounts.ShareCapital]       = ("3001", "Share Capital"),
                [MasterDataIds.Accounts.RetainedEarnings]   = ("3002", "Retained Earnings"),
                [MasterDataIds.Accounts.SalesRevenue]       = ("4001", "Sales Revenue - Sofas"),
                [MasterDataIds.Accounts.ServiceRevenue]     = ("4002", "Service Revenue"),
                [MasterDataIds.Accounts.CostOfMaterials]    = ("5001", "Cost of Materials"),
                [MasterDataIds.Accounts.SalariesWages]      = ("6001", "Salaries & Wages"),
                [MasterDataIds.Accounts.RentExpense]        = ("6002", "Rent Expense"),
                [MasterDataIds.Accounts.UtilitiesExpense]   = ("6003", "Utilities Expense"),
            };
        }

        // ─────────── Fiscal Year ───────────
        private static readonly Lazy<Dictionary<Guid, (string Code, string Name)>> _fiscalYears = new(BuildFiscalYears);

        public static string FiscalYearCode(Guid id) => _fiscalYears.Value.TryGetValue(id, out var v) ? v.Code : "???";
        public static string FiscalYearName(Guid id) => _fiscalYears.Value.TryGetValue(id, out var v) ? v.Name : "Unknown FY";

        private static Dictionary<Guid, (string Code, string Name)> BuildFiscalYears()
        {
            return new Dictionary<Guid, (string Code, string Name)>
            {
                [MasterDataIds.FiscalYears.FY2025_26]                = ("FY2025-26", "FY 2025-26"),
                [MasterDataIds.FiscalYears.FY2024_25]                = ("FY2024-25", "FY 2024-25"),
                [MasterDataIds.CompanyFiscalYears.PlushComfort2024]   = ("FY2024-25", "FY 2024-25"),
                [MasterDataIds.CompanyFiscalYears.VelvetRest2024]     = ("FY2024-25", "FY 2024-25"),
                [MasterDataIds.CompanyFiscalYears.CozyCraft2024]      = ("FY2024-25", "FY 2024-25"),
                [MasterDataIds.CompanyFiscalYears.PremiumSeating2025] = ("FY2025-26", "FY 2025-26"),
                [MasterDataIds.CompanyFiscalYears.CloudSofa2024]      = ("FY2024-25", "FY 2024-25"),
                [MasterDataIds.CompanyFiscalYears.EliteLoungers2025]  = ("FY2025-26", "FY 2025-26"),
            };
        }

        // ─────────── Accounting Period ───────────
        private static readonly Lazy<Dictionary<Guid, (string Code, string Name)>> _accountingPeriods = new(BuildAccountingPeriods);

        public static string AccountingPeriodCode(Guid id) => _accountingPeriods.Value.TryGetValue(id, out var v) ? v.Code : "???";
        public static string AccountingPeriodName(Guid id) => _accountingPeriods.Value.TryGetValue(id, out var v) ? v.Name : "Unknown Period";

        private static Dictionary<Guid, (string Code, string Name)> BuildAccountingPeriods()
        {
            return new Dictionary<Guid, (string Code, string Name)>
            {
                [MasterDataIds.AccountingPeriods.Apr2025] = ("APR-2025", "April 2025"),
                [MasterDataIds.AccountingPeriods.May2025] = ("MAY-2025", "May 2025"),
                [MasterDataIds.AccountingPeriods.Apr2024] = ("APR-2024", "April 2024"),
            };
        }
    }
}
