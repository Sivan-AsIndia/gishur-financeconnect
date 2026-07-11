namespace FinanceConnect.Client.Data
{
    /// <summary>
    /// Single source of truth for every well-known primary-key GUID used in
    /// MasterDataService seed data. Foreign-key references throughout the seed
    /// data classes use these constants instead of inline Guid.Parse(...) calls.
    ///
    /// IMPORTANT — Values below are NOT arbitrary. Every ID is sourced from its
    /// corresponding seed-data class (BranchService, COADataService, LedgerSeedData,
    /// FiscalYearService, AccountingPeriodService, GeneralLedgerEntrySeedData,
    /// PaymentTermSeedData, etc.). Do NOT change an ID here without also updating
    /// the seed-data class that defines it.
    ///
    /// Exceptions (these are defined ONLY here and may be freely assigned):
    ///   Countries, States, Cities, Currencies, TimeZones, Companies, ExchangeRates
    /// </summary>
    public static class MasterDataIds
    {
        // ───────────── Countries ─────────────
        public static class Countries
        {
            public static readonly Guid India              = Guid.Parse("11111111-1111-1111-1111-111111111111");
            public static readonly Guid UnitedStates       = Guid.Parse("22222222-2222-2222-2222-222222222222");
            public static readonly Guid UAE                = Guid.Parse("44444444-4444-4444-4444-444444444444");
            public static readonly Guid Singapore          = Guid.Parse("55555555-5555-5555-5555-555555555555");
            public static readonly Guid UnitedKingdom      = Guid.Parse("66666666-6666-6666-6666-666666666666");
            public static readonly Guid Japan              = Guid.Parse("77777777-7777-7777-7777-777777777777");
            public static readonly Guid Australia          = Guid.Parse("88888888-8888-8888-8888-888888888888");
            public static readonly Guid Germany            = Guid.Parse("99999999-9999-9999-9999-999999999999");
            public static readonly Guid Canada             = Guid.Parse("aabbcc01-0001-0001-0001-000000000001");
            public static readonly Guid Malaysia           = Guid.Parse("aabbcc02-0002-0002-0002-000000000002");
            public static readonly Guid SaudiArabia        = Guid.Parse("aabbcc03-0003-0003-0003-000000000003");
            public static readonly Guid SouthAfrica        = Guid.Parse("aabbcc04-0004-0004-0004-000000000004");
        }

        // ───────────── State / Provinces ─────────────
        public static class States
        {
            // India
            public static readonly Guid TamilNadu          = Guid.Parse("aaaa1111-1111-1111-1111-111111111111");
            public static readonly Guid Karnataka          = Guid.Parse("aaaa2222-2222-2222-2222-222222222222");
            public static readonly Guid Maharashtra        = Guid.Parse("aaaa3333-3333-3333-3333-333333333333");
            public static readonly Guid Delhi              = Guid.Parse("aaaa4444-4444-4444-4444-444444444444");
            public static readonly Guid Telangana          = Guid.Parse("aaaa5555-5555-5555-5555-555555555555");
            public static readonly Guid WestBengal         = Guid.Parse("aaaa6666-6666-6666-6666-666666666666");

            // United States
            public static readonly Guid California         = Guid.Parse("bbbb2222-2222-2222-2222-222222222222");
            public static readonly Guid Texas              = Guid.Parse("bbbb3333-3333-3333-3333-333333333333");

            // UAE
            public static readonly Guid Dubai              = Guid.Parse("cccc4444-4444-4444-4444-444444444444");
            public static readonly Guid AbuDhabi           = Guid.Parse("cccc5555-5555-5555-5555-555555555555");

            // Singapore (city-state)
            public static readonly Guid SingaporeState     = Guid.Parse("aaaa7777-7777-7777-7777-777777777777");
        }

        // ───────────── Cities ─────────────
        public static class Cities
        {
            public static readonly Guid Chennai            = Guid.Parse("dddd1111-1111-1111-1111-111111111111");
            public static readonly Guid Bengaluru          = Guid.Parse("dddd2222-2222-2222-2222-222222222222");
            public static readonly Guid Coimbatore         = Guid.Parse("dddd3333-3333-3333-3333-333333333333");
            public static readonly Guid Mumbai             = Guid.Parse("dddd4444-4444-4444-4444-444444444444");
            public static readonly Guid SanFrancisco       = Guid.Parse("dddd5555-5555-5555-5555-555555555555");
            public static readonly Guid Dallas             = Guid.Parse("dddd6666-6666-6666-6666-666666666666");
            public static readonly Guid DubaiCity          = Guid.Parse("dddd7777-7777-7777-7777-777777777777");
            public static readonly Guid AbuDhabiCity       = Guid.Parse("dddd8888-8888-8888-8888-888888888888");
            public static readonly Guid NewDelhi           = Guid.Parse("dddd9999-9999-9999-9999-999999999999");
            public static readonly Guid Pune               = Guid.Parse("ddddaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            public static readonly Guid Hyderabad          = Guid.Parse("ddddbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
            public static readonly Guid Kolkata            = Guid.Parse("ddddcccc-cccc-cccc-cccc-cccccccccccc");
            public static readonly Guid SingaporeCity      = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
        }

        // ───────────── Currencies ─────────────
        public static class Currencies
        {
            public static readonly Guid INR                = Guid.Parse("ffff1111-1111-1111-1111-111111111111");
            public static readonly Guid USD                = Guid.Parse("ffff2222-2222-2222-2222-222222222222");
            public static readonly Guid GBP                = Guid.Parse("ffff3333-3333-3333-3333-333333333333");
            public static readonly Guid EUR                = Guid.Parse("ffff4444-4444-4444-4444-444444444444");
            public static readonly Guid JPY                = Guid.Parse("ffff5555-5555-5555-5555-555555555555");
            public static readonly Guid AUD                = Guid.Parse("ffff6666-6666-6666-6666-666666666666");
            public static readonly Guid AED                = Guid.Parse("ffff7777-7777-7777-7777-777777777777");
            public static readonly Guid SGD                = Guid.Parse("ffff8888-8888-8888-8888-888888888888");
            public static readonly Guid CAD                = Guid.Parse("ffff9999-9999-9999-9999-999999999999");
            public static readonly Guid MYR                = Guid.Parse("ffffaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            public static readonly Guid SAR                = Guid.Parse("ffffbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
            public static readonly Guid ZAR                = Guid.Parse("ffffcccc-cccc-cccc-cccc-cccccccccccc");
        }

        // ───────────── TimeZones ─────────────
        public static class TimeZones
        {
            public static readonly Guid AsiaKolkata        = Guid.Parse("eeee1111-1111-1111-1111-111111111111");
            public static readonly Guid AmericaLosAngeles  = Guid.Parse("eeee2222-2222-2222-2222-222222222222");
            public static readonly Guid AmericaChicago     = Guid.Parse("eeee3333-3333-3333-3333-333333333333");
            public static readonly Guid AsiaDubai          = Guid.Parse("eeee4444-4444-4444-4444-444444444444");
            public static readonly Guid AsiaSingapore      = Guid.Parse("eeee5555-5555-5555-5555-555555555555");
            public static readonly Guid EuropeLondon       = Guid.Parse("eeee6666-6666-6666-6666-666666666666");
            public static readonly Guid AsiaTokyo          = Guid.Parse("eeee7777-7777-7777-7777-777777777777");
            public static readonly Guid AustraliaSydney    = Guid.Parse("eeee8888-8888-8888-8888-888888888888");
            public static readonly Guid EuropeBerlin       = Guid.Parse("eeee9999-9999-9999-9999-999999999999");
            public static readonly Guid AmericaToronto     = Guid.Parse("eeeeaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            public static readonly Guid AsiaRiyadh         = Guid.Parse("eeeecccc-cccc-cccc-cccc-cccccccccccc");
            public static readonly Guid AsiaKualaLumpur    = Guid.Parse("eeeedddd-dddd-dddd-dddd-dddddddddddd");
        }

        // ───────────── Companies ─────────────
        public static class Companies
        {
            public static readonly Guid SofaCraft          = Guid.Parse("c0fa0001-0001-0001-0001-000000000001");
            public static readonly Guid SofaCraftUSA       = Guid.Parse("c0fa0002-0002-0002-0002-000000000002");
            public static readonly Guid OakNest            = Guid.Parse("c0fa0003-0003-0003-0003-000000000003");
            public static readonly Guid UrbanLoft          = Guid.Parse("c0fa0004-0004-0004-0004-000000000004");
            public static readonly Guid DesertDune         = Guid.Parse("c0fa0005-0005-0005-0005-000000000005");
            public static readonly Guid PlushComfort       = Guid.Parse("c0fa0006-0006-0006-0006-000000000006");
            public static readonly Guid VelvetRest         = Guid.Parse("c0fa0007-0007-0007-0007-000000000007");
            public static readonly Guid CozyCraft          = Guid.Parse("c0fa0008-0008-0008-0008-000000000008");
            public static readonly Guid PremiumSeating     = Guid.Parse("c0fa0009-0009-0009-0009-000000000009");
            public static readonly Guid CloudSofa          = Guid.Parse("c0fa0010-0010-0010-0010-000000000010");
            public static readonly Guid EliteLoungers      = Guid.Parse("c0fa0011-0011-0011-0011-000000000011");
        }

        // ───────────── Exchange Rates ─────────────
        public static class ExchangeRates
        {
            public static readonly Guid UsdInr             = Guid.Parse("e0fa0001-0001-0001-0001-000000000001");
            public static readonly Guid InrUsd             = Guid.Parse("e0fa0002-0002-0002-0002-000000000002");
            public static readonly Guid AedInr             = Guid.Parse("e0fa0003-0003-0003-0003-000000000003");
            public static readonly Guid InrAed             = Guid.Parse("e0fa0004-0004-0004-0004-000000000004");
            public static readonly Guid GbpInr             = Guid.Parse("e0fa0005-0005-0005-0005-000000000005");
            public static readonly Guid EurInr             = Guid.Parse("e0fa0006-0006-0006-0006-000000000006");
            public static readonly Guid JpyInr             = Guid.Parse("e0fa0007-0007-0007-0007-000000000007");
            public static readonly Guid SgdInr             = Guid.Parse("e0fa0008-0008-0008-0008-000000000008");
            public static readonly Guid UsdInrMonthly      = Guid.Parse("e0fa0009-0009-0009-0009-000000000009");
            public static readonly Guid AudInr             = Guid.Parse("e0fa000a-000a-000a-000a-00000000000a");
            public static readonly Guid CadInr             = Guid.Parse("e0fa000b-000b-000b-000b-00000000000b");
            public static readonly Guid SarInr             = Guid.Parse("e0fa000c-000c-000c-000c-00000000000c");
        }

        // ───────────── Tenants ─────────────
        public static class Tenants
        {
            public static readonly Guid Default            = Guid.Parse("00000000-0000-0000-0000-000000000001");
        }

        // ──────────────────────────────────────────────────────────────
        //  Branches  →  sourced from BranchService seed data
        //  GUIDs must match BranchService.SeedBranches()
        // ──────────────────────────────────────────────────────────────
        public static class Branches
        {
            // SofaCraft Furnishings (c0fa0001) branches
            public static readonly Guid SofaCraftHQ        = Guid.Parse("b0fa0001-0001-0001-0001-000000000001");  // HO  – SofaCraft Head Office & Factory - Chennai
            public static readonly Guid SofaCraftBengaluru = Guid.Parse("b0fa0003-0003-0003-0003-000000000003");  // BLR – SofaCraft Experience Store - Bengaluru
            public static readonly Guid SofaCraftDubai     = Guid.Parse("b0fa0005-0005-0005-0005-000000000005");  // DXB – SofaCraft Sales Office - Dubai

            // SofaCraft USA (c0fa0002) branches
            public static readonly Guid SofaCraftUSA_SFO   = Guid.Parse("b0fa1001-0001-0001-0001-000000000001");  // SFO – San Francisco
            public static readonly Guid SofaCraftUSA_DAL   = Guid.Parse("b0fa1002-0002-0002-0002-000000000002");  // DAL – Dallas

            // OakNest (c0fa0003)
            public static readonly Guid OakNestBengaluru   = Guid.Parse("b0fa2001-0001-0001-0001-000000000001");  // BLR-HO

            // UrbanLoft (c0fa0004)
            public static readonly Guid UrbanLoftMumbai    = Guid.Parse("b0fa3001-0001-0001-0001-000000000001");  // BOM-HO

            // DesertDune (c0fa0005)
            public static readonly Guid DesertDuneDubai    = Guid.Parse("b0fa4001-0001-0001-0001-000000000001");  // DXB-HO

            // PlushComfort (c0fa0006)
            public static readonly Guid PlushComfortDelhi  = Guid.Parse("b0fa6001-0001-0001-0001-000000000001");  // DEL-HO
            public static readonly Guid PlushComfortMH     = Guid.Parse("b0fa6002-0002-0002-0002-000000000002");  // GGN-FAC

            // VelvetRest (c0fa0007)
            public static readonly Guid VelvetRestPune     = Guid.Parse("b0fa7001-0001-0001-0001-000000000001");  // PUN-HO
            public static readonly Guid VelvetRestMumbai   = Guid.Parse("b0fa7002-0002-0002-0002-000000000002");  // BOM-SH

            // CozyCraft (c0fa0008)
            public static readonly Guid CozyCraftHyderabad = Guid.Parse("b0fa8001-0001-0001-0001-000000000001");  // HYD-HO
            public static readonly Guid CozyCraftWarehouse = Guid.Parse("b0fa8002-0002-0002-0002-000000000002");  // HYD-WH

            // PremiumSeating (c0fa0009)
            public static readonly Guid PremiumSeatingSG   = Guid.Parse("b0fa9001-0001-0001-0001-000000000001");  // SG-HO
            public static readonly Guid PremiumSeatingWH   = Guid.Parse("b0fa9002-0002-0002-0002-000000000002");  // SG-WH

            // CloudSofa (c0fa0010)
            public static readonly Guid CloudSofaKolkata   = Guid.Parse("b0fa1001-1001-1001-1001-000000000001");  // KOL-HO
            public static readonly Guid CloudSofaWarehouse = Guid.Parse("b0fa1002-1002-1002-1002-000000000002");  // KOL-WH

            // EliteLoungers (c0fa0011)
            public static readonly Guid EliteLoungerAbuDhabi = Guid.Parse("b0fa1101-1101-1101-1101-000000000001");  // AUH-HO
            public static readonly Guid EliteLoungerDubai    = Guid.Parse("b0fa1102-1102-1102-1102-000000000002");  // DXB-SH
        }

        // ──────────────────────────────────────────────────────────────
        //  Ledgers  →  sourced from LedgerSeedData.GetSeedData()
        // ──────────────────────────────────────────────────────────────
        public static class Ledgers
        {
            public static readonly Guid PrimaryLedger      = Guid.Parse("1ed00001-0001-0001-0001-000000000001");
            public static readonly Guid ManagementLedger   = Guid.Parse("1ed00002-0002-0002-0002-000000000002");
            public static readonly Guid ReliancePrimary    = Guid.Parse("1ed00003-0003-0003-0003-000000000003");
            // Per-company ledgers from FinanceDataService seed
            public static readonly Guid UrbanLoftPrimary   = Guid.Parse("1ed00004-0004-0004-0004-000000000004");
            public static readonly Guid SofaCraftUSAPrimary= Guid.Parse("1ed20001-0001-0001-0001-000000000001");
            public static readonly Guid OakNestPrimary     = Guid.Parse("1ed30001-0001-0001-0001-000000000001");
            public static readonly Guid DesertDunePrimary  = Guid.Parse("1ed40001-0001-0001-0001-000000000001");
            public static readonly Guid PlushComfortPrimary= Guid.Parse("1ed60001-0001-0001-0001-000000000001");
            public static readonly Guid VelvetRestPrimary  = Guid.Parse("1ed70001-0001-0001-0001-000000000001");
            public static readonly Guid CozyCraftPrimary   = Guid.Parse("1ed80001-0001-0001-0001-000000000001");
            public static readonly Guid PremiumSeatingPrimary = Guid.Parse("1ed90001-0001-0001-0001-000000000001");
            public static readonly Guid CloudSofaPrimary   = Guid.Parse("1eda0001-0001-0001-0001-000000000001");
            public static readonly Guid EliteLoungersPrimary = Guid.Parse("1edb0001-0001-0001-0001-000000000001");
        }

        // ──────────────────────────────────────────────────────────────
        //  Accounts (Chart of Accounts)  →  sourced from
        //  COADataService SofaCraft (COA-SF-2025) accounts
        //  GUIDs & codes match COADataService.SeedAll()
        // ──────────────────────────────────────────────────────────────
        public static class Accounts
        {
            public static readonly Guid PettyCash          = Guid.Parse("a0000001-0001-0001-0001-000000000001");  // 1001 – Petty Cash
            public static readonly Guid HDFCBankAccount    = Guid.Parse("a0000002-0002-0002-0002-000000000002");  // 1002 – HDFC Bank - Current Account
            public static readonly Guid AccountsReceivable = Guid.Parse("a0000003-0003-0003-0003-000000000003");  // 1003 – Accounts Receivable
            public static readonly Guid FurnitureFixtures  = Guid.Parse("a0000004-0004-0004-0004-000000000004");  // 1100 – Furniture & Fixtures
            public static readonly Guid AccountsPayable    = Guid.Parse("a0000005-0005-0005-0005-000000000005");  // 2001 – Accounts Payable
            public static readonly Guid GSTPayable         = Guid.Parse("a0000006-0006-0006-0006-000000000006");  // 2002 – GST Payable
            public static readonly Guid TDSPayable         = Guid.Parse("a0000007-0007-0007-0007-000000000007");  // 2003 – TDS Payable
            public static readonly Guid ShareCapital       = Guid.Parse("a0000008-0008-0008-0008-000000000008");  // 3001 – Share Capital
            public static readonly Guid RetainedEarnings   = Guid.Parse("a0000009-0009-0009-0009-000000000009");  // 3002 – Retained Earnings
            public static readonly Guid SalesRevenue       = Guid.Parse("a0000010-0010-0010-0010-000000000010");  // 4001 – Sales Revenue - Sofas
            public static readonly Guid ServiceRevenue     = Guid.Parse("a0000011-0011-0011-0011-000000000011");  // 4002 – Service Revenue
            public static readonly Guid CostOfMaterials    = Guid.Parse("a0000012-0012-0012-0012-000000000012");  // 5001 – Cost of Materials
            public static readonly Guid SalariesWages      = Guid.Parse("a0000013-0013-0013-0013-000000000013");  // 6001 – Salaries & Wages
            public static readonly Guid RentExpense        = Guid.Parse("a0000014-0014-0014-0014-000000000014");  // 6002 – Rent Expense
            public static readonly Guid UtilitiesExpense   = Guid.Parse("a0000015-0015-0015-0015-000000000015");  // 6003 – Utilities Expense
        }

        // ──────────────────────────────────────────────────────────────
        //  Fiscal Years  →  sourced from FiscalYearService seed
        //  These stable IDs are assigned in FiscalYearService.SeedFiscalYears()
        //  for the SofaCraft company.
        // ──────────────────────────────────────────────────────────────
        public static class FiscalYears
        {
            public static readonly Guid FY2025_26          = Guid.Parse("f0000001-0001-0001-0001-000000000001");
            public static readonly Guid FY2024_25          = Guid.Parse("f0000002-0002-0002-0002-000000000002");
        }

        // ──────────────────────────────────────────────────────────────
        //  Accounting Periods  →  sourced from AccountingPeriodService
        //  These stable IDs are assigned after period generation for SofaCraft FYs.
        //  NOTE: "af" prefix avoids collision with COA account GUIDs ("a0" prefix).
        // ──────────────────────────────────────────────────────────────
        public static class AccountingPeriods
        {
            public static readonly Guid Apr2025            = Guid.Parse("af000001-0001-0001-0001-000000000001");
            public static readonly Guid May2025            = Guid.Parse("af000002-0002-0002-0002-000000000002");
            public static readonly Guid Apr2024            = Guid.Parse("af000003-0003-0003-0003-000000000003");
        }

        // ──────────────────────────────────────────────────────────────
        //  General Ledger Entries  →  sourced from GeneralLedgerEntrySeedData
        // ──────────────────────────────────────────────────────────────
        public static class GLEntries
        {
            public static readonly Guid Entry01            = Guid.Parse("a1e00001-0001-0001-0001-000000000001");
            public static readonly Guid Entry02            = Guid.Parse("a1e00002-0002-0002-0002-000000000002");
            public static readonly Guid Entry03            = Guid.Parse("a1e00003-0003-0003-0003-000000000003");
            public static readonly Guid Entry04            = Guid.Parse("a1e00004-0004-0004-0004-000000000004");
            public static readonly Guid Entry05            = Guid.Parse("a1e00005-0005-0005-0005-000000000005");
            public static readonly Guid Entry06            = Guid.Parse("a1e00006-0006-0006-0006-000000000006");
            public static readonly Guid Entry07            = Guid.Parse("a1e00007-0007-0007-0007-000000000007");
            public static readonly Guid Entry08            = Guid.Parse("a1e00008-0008-0008-0008-000000000008");
            public static readonly Guid Entry09            = Guid.Parse("a1e00009-0009-0009-0009-000000000009");
            public static readonly Guid Entry10            = Guid.Parse("a1e00010-0010-0010-0010-000000000010");
            public static readonly Guid Entry11            = Guid.Parse("a1e00011-0011-0011-0011-000000000011");
            public static readonly Guid Entry12            = Guid.Parse("a1e00012-0012-0012-0012-000000000012");
        }

        // ───────────── Source Documents / Journal Entries (GL refs) ─────────────
        public static class SourceDocs
        {
            public static readonly Guid JE_000145          = Guid.Parse("0e000001-0001-0001-0001-000000000001");
            public static readonly Guid JE_000146          = Guid.Parse("0e000002-0002-0002-0002-000000000002");
            public static readonly Guid JE_000147          = Guid.Parse("0e000003-0003-0003-0003-000000000003");
            public static readonly Guid JE_000150          = Guid.Parse("0e000004-0004-0004-0004-000000000004");
            public static readonly Guid JE_Reversed        = Guid.Parse("0e000005-0005-0005-0005-000000000005");
            public static readonly Guid JE_000155          = Guid.Parse("0e000006-0006-0006-0006-000000000006");
            public static readonly Guid JE_000200          = Guid.Parse("0e000007-0007-0007-0007-000000000007");
            public static readonly Guid JournalLine01      = Guid.Parse("01000001-0001-0001-0001-000000000001");
            public static readonly Guid JournalLine02      = Guid.Parse("01000002-0002-0002-0002-000000000002");
            public static readonly Guid InvoiceDoc01       = Guid.Parse("c1000001-0001-0001-0001-000000000001");
            public static readonly Guid BankTxDoc01        = Guid.Parse("b0a00001-0001-0001-0001-000000000001");
            public static readonly Guid OBDoc01            = Guid.Parse("0bd00001-0001-0001-0001-000000000001");
            public static readonly Guid ReceiptDoc01       = Guid.Parse("c0fa0001-0001-0001-0001-000000000001");
            public static readonly Guid ReversalGroup01    = Guid.Parse("00000001-0001-0001-0001-000000000001");
        }

        // ───────────── Supplementary Company refs (OB / CB data) ─────────────
        public static class SupplementaryCompanies
        {
            public static readonly Guid CozyCraft3001      = Guid.Parse("c0fa8001-0001-0001-0001-000000000001");
            public static readonly Guid UrbanLoft3001      = Guid.Parse("c0fa3001-0001-0001-0001-000000000001");
            public static readonly Guid PlushComfort6001   = Guid.Parse("c0fa6001-0001-0001-0001-000000000001");
            public static readonly Guid VelvetRest7001     = Guid.Parse("c0fa7001-0001-0001-0001-000000000001");
            public static readonly Guid PremiumSeating9001 = Guid.Parse("c0fa9001-0001-0001-0001-000000000001");
        }

        // ───────────── Opening Balances ─────────────
        public static class OpeningBalances
        {
            public static readonly Guid OB01               = Guid.Parse("0b000001-0001-0001-0001-000000000001");
            public static readonly Guid OB02               = Guid.Parse("0b000002-0002-0002-0002-000000000002");
            public static readonly Guid OB03               = Guid.Parse("0b000003-0003-0003-0003-000000000003");
            public static readonly Guid OB04               = Guid.Parse("0b000004-0004-0004-0004-000000000004");
            public static readonly Guid OB05               = Guid.Parse("0b000005-0005-0005-0005-000000000005");
            public static readonly Guid OB06               = Guid.Parse("0b000006-0006-0006-0006-000000000006");
            public static readonly Guid OB07               = Guid.Parse("0b000007-0007-0007-0007-000000000007");
            public static readonly Guid OB08               = Guid.Parse("0b000008-0008-0008-0008-000000000008");
            public static readonly Guid OB09               = Guid.Parse("0b000009-0009-0009-0009-000000000009");
            public static readonly Guid OB10               = Guid.Parse("0b000010-0010-0010-0010-000000000010");
            public static readonly Guid OB11               = Guid.Parse("0b000011-0011-0011-0011-000000000011");
            public static readonly Guid OB12               = Guid.Parse("0b000012-0012-0012-0012-000000000012");
            public static readonly Guid OB_0A              = Guid.Parse("0b00000a-000a-000a-000a-00000000000a");
            public static readonly Guid OB_0B              = Guid.Parse("0b00000b-000b-000b-000b-00000000000b");
            public static readonly Guid OB_0C              = Guid.Parse("0b00000c-000c-000c-000c-00000000000c");
            // Additional OBs from FinanceDataService (per-company)
            public static readonly Guid OB16               = Guid.Parse("0b000016-0016-0016-0016-000000000016");
            public static readonly Guid OB17               = Guid.Parse("0b000017-0017-0017-0017-000000000017");
            public static readonly Guid OB18               = Guid.Parse("0b000018-0018-0018-0018-000000000018");
            public static readonly Guid OB19               = Guid.Parse("0b000019-0019-0019-0019-000000000019");
            public static readonly Guid OB20               = Guid.Parse("0b000020-0020-0020-0020-000000000020");
        }

        // ───────────── Closing Balances ─────────────
        public static class ClosingBalances
        {
            public static readonly Guid CB01               = Guid.Parse("cb000001-0001-0001-0001-000000000001");
            public static readonly Guid CB02               = Guid.Parse("cb000002-0002-0002-0002-000000000002");
            public static readonly Guid CB03               = Guid.Parse("cb000003-0003-0003-0003-000000000003");
            public static readonly Guid CB04               = Guid.Parse("cb000004-0004-0004-0004-000000000004");
            public static readonly Guid CB05               = Guid.Parse("cb000005-0005-0005-0005-000000000005");
            public static readonly Guid CB06               = Guid.Parse("cb000006-0006-0006-0006-000000000006");
            public static readonly Guid CB07               = Guid.Parse("cb000007-0007-0007-0007-000000000007");
            public static readonly Guid CB08               = Guid.Parse("cb000008-0008-0008-0008-000000000008");
            public static readonly Guid CB09               = Guid.Parse("cb000009-0009-0009-0009-000000000009");
            public static readonly Guid CB10               = Guid.Parse("cb000010-0010-0010-0010-000000000010");
            public static readonly Guid CB11               = Guid.Parse("cb000011-0011-0011-0011-000000000011");
            public static readonly Guid CB12               = Guid.Parse("cb000012-0012-0012-0012-000000000012");
            public static readonly Guid CB_0A              = Guid.Parse("cb00000a-000a-000a-000a-00000000000a");
            public static readonly Guid CB_0B              = Guid.Parse("cb00000b-000b-000b-000b-00000000000b");
            public static readonly Guid CB_0C              = Guid.Parse("cb00000c-000c-000c-000c-00000000000c");
        }

        // ──────────────────────────────────────────────────────────────
        //  Payment Terms  →  sourced from PaymentTermSeedData.GetSeedData()
        // ──────────────────────────────────────────────────────────────
        public static class PaymentTerms
        {
            public static readonly Guid Net30     = Guid.Parse("00000000-0000-0000-0000-000000000001");
            public static readonly Guid Net45     = Guid.Parse("00000000-0000-0000-0000-000000000002");
            public static readonly Guid Net60     = Guid.Parse("00000000-0000-0000-0000-000000000003");
            public static readonly Guid Immediate = Guid.Parse("00000000-0000-0000-0000-000000000004");
            public static readonly Guid Net15     = Guid.Parse("00000000-0000-0000-0000-000000000005");
        }

        // ───────────── Close Runs (CB refs) ─────────────
        public static class CloseRuns
        {
            public static readonly Guid CloseRun01         = Guid.Parse("c0c10001-0001-0001-0001-000000000001");
            public static readonly Guid CloseRun02         = Guid.Parse("c0c10002-0002-0002-0002-000000000002");
            public static readonly Guid CloseRun_C1        = Guid.Parse("c1000002-0002-0002-0002-000000000002");
            // Closing entry IDs from FinanceDataService
            public static readonly Guid CE01               = Guid.Parse("ce000001-0001-0001-0001-000000000001");
            public static readonly Guid CE02               = Guid.Parse("ce000002-0002-0002-0002-000000000002");
            public static readonly Guid CE03               = Guid.Parse("ce000003-0003-0003-0003-000000000003");
            public static readonly Guid CE04               = Guid.Parse("ce000004-0004-0004-0004-000000000004");
            public static readonly Guid CE05               = Guid.Parse("ce000005-0005-0005-0005-000000000005");
            public static readonly Guid CE06               = Guid.Parse("ce000006-0006-0006-0006-000000000006");
            public static readonly Guid CE07               = Guid.Parse("ce000007-0007-0007-0007-000000000007");
            public static readonly Guid CE08               = Guid.Parse("ce000008-0008-0008-0008-000000000008");
            public static readonly Guid CE09               = Guid.Parse("ce000009-0009-0009-0009-000000000009");
            public static readonly Guid CE10               = Guid.Parse("ce000010-0010-0010-0010-000000000010");
            public static readonly Guid CE11               = Guid.Parse("ce000011-0011-0011-0011-000000000011");
        }

        // ───────────── Per-company Fiscal Years (FinanceDataService) ─────────────
        public static class CompanyFiscalYears
        {
            public static readonly Guid PlushComfort2024   = Guid.Parse("f6fa2024-2024-2024-2024-000000000001");
            public static readonly Guid VelvetRest2024     = Guid.Parse("f7fa2024-2024-2024-2024-000000000001");
            public static readonly Guid CozyCraft2024      = Guid.Parse("f8fa2024-2024-2024-2024-000000000001");
            public static readonly Guid PremiumSeating2025 = Guid.Parse("f9fa2025-2025-2025-2025-000000000001");
            public static readonly Guid CloudSofa2024      = Guid.Parse("fafa2024-2024-2024-2024-000000000001");
            public static readonly Guid EliteLoungers2025  = Guid.Parse("fbfa2025-2025-2025-2025-000000000001");
        }
    }
}
