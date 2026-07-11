using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    public static class OpeningBalanceSeedData
    {
        public static List<OpeningBalanceModel> GetSeedData()
        {
            return new List<OpeningBalanceModel>
            {
                    new OpeningBalanceModel
            {
                Id = MasterDataIds.OpeningBalances.OB01,
                TenantId = MasterDataIds.Tenants.Default,
                OpeningBalanceNumber = "OB-2025-00001",
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = SeedLookup.CompanyCode(MasterDataIds.Companies.SofaCraft),
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                BranchId = MasterDataIds.Branches.SofaCraftHQ,
                BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftHQ),
                BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftHQ),
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = SeedLookup.LedgerCode(MasterDataIds.Ledgers.PrimaryLedger),
                LedgerName = SeedLookup.LedgerName(MasterDataIds.Ledgers.PrimaryLedger),
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearName = SeedLookup.FiscalYearName(MasterDataIds.FiscalYears.FY2025_26),
                OpeningDate = new DateTime(2025, 4, 1),
                EntryMode = "ManualEntry",
                RestrictToBalanceSheetAccounts = true,
                Status = "Posted",
                ApprovedBy = "Controller",
                ApprovedAt = new DateTime(2025, 3, 30),
                PostedBy = "System",
                PostedAt = new DateTime(2025, 4, 1),
                Notes = "Opening balance for FY 2025-26",
                Lines = new List<OpeningBalanceLineModel>
                {
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.NewGuid(),
                        AccountId = MasterDataIds.Accounts.RentExpense,
                        AccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.RentExpense),
                        AccountName = SeedLookup.AccountName(MasterDataIds.Accounts.RentExpense),
                        DebitAmountBase = 500000.00m,
                        CreditAmountBase = 0m,
                        LineDescription = "Opening bank balance"
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.NewGuid(),
                        AccountId = MasterDataIds.Accounts.PettyCash,
                        AccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.PettyCash),
                        AccountName = SeedLookup.AccountName(MasterDataIds.Accounts.PettyCash),
                        DebitAmountBase = 100000.00m,
                        CreditAmountBase = 0m,
                        LineDescription = "Opening cash balance"
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.NewGuid(),
                        AccountId = MasterDataIds.Accounts.AccountsReceivable,
                        AccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsReceivable),
                        AccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsReceivable),
                        DebitAmountBase = 400000.00m,
                        CreditAmountBase = 0m,
                        LineDescription = "Opening AR balance"
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.NewGuid(),
                        AccountId = MasterDataIds.Accounts.HDFCBankAccount,
                        AccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.HDFCBankAccount),
                        AccountName = SeedLookup.AccountName(MasterDataIds.Accounts.HDFCBankAccount),
                        DebitAmountBase = 500000.00m,
                        CreditAmountBase = 0m,
                        LineDescription = "Opening fixed assets"
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.NewGuid(),
                        AccountId = MasterDataIds.Accounts.AccountsPayable,
                        AccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsPayable),
                        AccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsPayable),
                        DebitAmountBase = 0m,
                        CreditAmountBase = 300000.00m,
                        LineDescription = "Opening AP balance"
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.NewGuid(),
                        AccountId = MasterDataIds.Accounts.RetainedEarnings,
                        AccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.RetainedEarnings),
                        AccountName = SeedLookup.AccountName(MasterDataIds.Accounts.RetainedEarnings),
                        DebitAmountBase = 0m,
                        CreditAmountBase = 500000.00m,
                        LineDescription = "Opening loan balance"
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.NewGuid(),
                        AccountId = MasterDataIds.Accounts.ShareCapital,
                        AccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.ShareCapital),
                        AccountName = SeedLookup.AccountName(MasterDataIds.Accounts.ShareCapital),
                        DebitAmountBase = 0m,
                        CreditAmountBase = 500000.00m,
                        LineDescription = "Opening capital"
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.NewGuid(),
                        AccountId = MasterDataIds.Accounts.RetainedEarnings,
                        AccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.RetainedEarnings),
                        AccountName = SeedLookup.AccountName(MasterDataIds.Accounts.RetainedEarnings),
                        DebitAmountBase = 0m,
                        CreditAmountBase = 200000.00m,
                        LineDescription = "Opening retained earnings"
                    }
                },
                CreatedAt = DateTime.Now.AddDays(-100),
                CreatedBy = "System"
            },
            new OpeningBalanceModel
            {
                Id = MasterDataIds.OpeningBalances.OB02,
                TenantId = MasterDataIds.Tenants.Default,
                OpeningBalanceNumber = "OB-2025-00002",
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = SeedLookup.CompanyCode(MasterDataIds.Companies.SofaCraft),
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                BranchId = MasterDataIds.Branches.SofaCraftBengaluru,
                BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = SeedLookup.LedgerCode(MasterDataIds.Ledgers.PrimaryLedger),
                LedgerName = SeedLookup.LedgerName(MasterDataIds.Ledgers.PrimaryLedger),
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearName = SeedLookup.FiscalYearName(MasterDataIds.FiscalYears.FY2025_26),
                OpeningDate = new DateTime(2025, 4, 1),
                EntryMode = "ManualEntry",
                RestrictToBalanceSheetAccounts = true,
                Status = "Posted",
                ApprovedBy = "Controller",
                ApprovedAt = new DateTime(2025, 3, 30),
                PostedBy = "System",
                PostedAt = new DateTime(2025, 4, 1),
                Notes = "Delhi branch opening balance",
                Lines = new List<OpeningBalanceLineModel>
                {
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.NewGuid(),
                        AccountId = MasterDataIds.Accounts.RentExpense,
                        AccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.RentExpense),
                        AccountName = SeedLookup.AccountName(MasterDataIds.Accounts.RentExpense),
                        DebitAmountBase = 150000.00m,
                        CreditAmountBase = 0m,
                        LineDescription = "Branch bank balance"
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.NewGuid(),
                        AccountId = MasterDataIds.Accounts.PettyCash,
                        AccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.PettyCash),
                        AccountName = SeedLookup.AccountName(MasterDataIds.Accounts.PettyCash),
                        DebitAmountBase = 50000.00m,
                        CreditAmountBase = 0m,
                        LineDescription = "Branch petty cash"
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.NewGuid(),
                        AccountId = MasterDataIds.Accounts.FurnitureFixtures,
                        AccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.FurnitureFixtures),
                        AccountName = SeedLookup.AccountName(MasterDataIds.Accounts.FurnitureFixtures),
                        DebitAmountBase = 0m,
                        CreditAmountBase = 200000.00m,
                        LineDescription = "Due to HO"
                    },
                    new OpeningBalanceLineModel
                    {
                        Id = Guid.NewGuid(),
                        AccountId = MasterDataIds.Accounts.HDFCBankAccount,
                        AccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.HDFCBankAccount),
                        AccountName = SeedLookup.AccountName(MasterDataIds.Accounts.HDFCBankAccount),
                        DebitAmountBase = 50000.00m,
                        CreditAmountBase = 0m,
                        LineDescription = "Branch equipment"
                    }
                },
                CreatedAt = DateTime.Now.AddDays(-100),
                CreatedBy = "System"
            },
            new OpeningBalanceModel
            {
                Id = MasterDataIds.OpeningBalances.OB03,
                TenantId = MasterDataIds.Tenants.Default,
                OpeningBalanceNumber = "OB-2025-00003",
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = SeedLookup.CompanyCode(MasterDataIds.Companies.SofaCraft),
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                BranchId = MasterDataIds.Branches.SofaCraftBengaluru,
                BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = SeedLookup.LedgerCode(MasterDataIds.Ledgers.PrimaryLedger),
                LedgerName = SeedLookup.LedgerName(MasterDataIds.Ledgers.PrimaryLedger),
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearName = SeedLookup.FiscalYearName(MasterDataIds.FiscalYears.FY2025_26),
                OpeningDate = new DateTime(2025, 4, 1),
                EntryMode = "ManualEntry",
                RestrictToBalanceSheetAccounts = true,
                Status = "Approved",
                ApprovedBy = "Controller",
                ApprovedAt = new DateTime(2025, 3, 31),
                Notes = "Bengaluru branch opening balance",
                Lines = new List<OpeningBalanceLineModel>
                {
                    new OpeningBalanceLineModel { Id = Guid.NewGuid(), AccountId = MasterDataIds.Accounts.RentExpense, AccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.RentExpense), AccountName = SeedLookup.AccountName(MasterDataIds.Accounts.RentExpense), DebitAmountBase = 250000.00m, CreditAmountBase = 0m, LineDescription = "Branch bank balance" },
                    new OpeningBalanceLineModel { Id = Guid.NewGuid(), AccountId = MasterDataIds.Accounts.AccountsPayable, AccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.AccountsPayable), AccountName = SeedLookup.AccountName(MasterDataIds.Accounts.AccountsPayable), DebitAmountBase = 0m, CreditAmountBase = 250000.00m, LineDescription = "Opening AP" }
                },
                CreatedAt = DateTime.Now.AddDays(-95),
                CreatedBy = "System"
            },
            new OpeningBalanceModel
            {
                Id = MasterDataIds.OpeningBalances.OB04,
                TenantId = MasterDataIds.Tenants.Default,
                OpeningBalanceNumber = "OB-2025-00004",
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = SeedLookup.CompanyCode(MasterDataIds.Companies.SofaCraft),
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                BranchId = MasterDataIds.Branches.SofaCraftDubai,
                BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftDubai),
                BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftDubai),
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = SeedLookup.LedgerCode(MasterDataIds.Ledgers.PrimaryLedger),
                LedgerName = SeedLookup.LedgerName(MasterDataIds.Ledgers.PrimaryLedger),
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearName = SeedLookup.FiscalYearName(MasterDataIds.FiscalYears.FY2025_26),
                OpeningDate = new DateTime(2025, 4, 1),
                EntryMode = "BulkImport",
                RestrictToBalanceSheetAccounts = true,
                Status = "Draft",
                Notes = "Dubai branch opening balance - pending review",
                Lines = new List<OpeningBalanceLineModel>
                {
                    new OpeningBalanceLineModel { Id = Guid.NewGuid(), AccountId = MasterDataIds.Accounts.RentExpense, AccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.RentExpense), AccountName = SeedLookup.AccountName(MasterDataIds.Accounts.RentExpense), DebitAmountBase = 300000.00m, CreditAmountBase = 0m, LineDescription = "Bank balance" },
                    new OpeningBalanceLineModel { Id = Guid.NewGuid(), AccountId = MasterDataIds.Accounts.ShareCapital, AccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.ShareCapital), AccountName = SeedLookup.AccountName(MasterDataIds.Accounts.ShareCapital), DebitAmountBase = 0m, CreditAmountBase = 300000.00m, LineDescription = "Capital" }
                },
                CreatedAt = DateTime.Now.AddDays(-90),
                CreatedBy = "System"
            },
            new OpeningBalanceModel
            {
                Id = MasterDataIds.OpeningBalances.OB05,
                TenantId = MasterDataIds.Tenants.Default,
                OpeningBalanceNumber = "OB-2025-00005",
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = SeedLookup.CompanyCode(MasterDataIds.Companies.SofaCraft),
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                BranchId = MasterDataIds.Branches.SofaCraftUSA_SFO,
                BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftUSA_SFO),
                BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftUSA_SFO),
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = SeedLookup.LedgerCode(MasterDataIds.Ledgers.PrimaryLedger),
                LedgerName = SeedLookup.LedgerName(MasterDataIds.Ledgers.PrimaryLedger),
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearName = SeedLookup.FiscalYearName(MasterDataIds.FiscalYears.FY2025_26),
                OpeningDate = new DateTime(2025, 4, 1),
                EntryMode = "MigrationMode",
                RestrictToBalanceSheetAccounts = true,
                Status = "Submitted",
                Notes = "San Francisco branch - migrated from legacy",
                Lines = new List<OpeningBalanceLineModel>
                {
                    new OpeningBalanceLineModel { Id = Guid.NewGuid(), AccountId = MasterDataIds.Accounts.RentExpense, AccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.RentExpense), AccountName = SeedLookup.AccountName(MasterDataIds.Accounts.RentExpense), DebitAmountBase = 300000.00m, CreditAmountBase = 0m, LineDescription = "Bank balance" },
                    new OpeningBalanceLineModel { Id = Guid.NewGuid(), AccountId = MasterDataIds.Accounts.ShareCapital, AccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.ShareCapital), AccountName = SeedLookup.AccountName(MasterDataIds.Accounts.ShareCapital), DebitAmountBase = 0m, CreditAmountBase = 300000.00m, LineDescription = "Capital" }
                },
                CreatedAt = DateTime.Now.AddDays(-88),
                CreatedBy = "System"
            },
            new OpeningBalanceModel
            {
                Id = MasterDataIds.OpeningBalances.OB06,
                TenantId = MasterDataIds.Tenants.Default,
                OpeningBalanceNumber = "OB-2025-00006",
                CompanyId = MasterDataIds.SupplementaryCompanies.CozyCraft3001,
                CompanyCode = SeedLookup.CompanyCode(MasterDataIds.Companies.SofaCraft),
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                BranchId = MasterDataIds.Branches.CozyCraftHyderabad,
                BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.CozyCraftHyderabad),
                BranchName = SeedLookup.BranchName(MasterDataIds.Branches.CozyCraftHyderabad),
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = SeedLookup.LedgerCode(MasterDataIds.Ledgers.PrimaryLedger),
                LedgerName = SeedLookup.LedgerName(MasterDataIds.Ledgers.PrimaryLedger),
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearName = SeedLookup.FiscalYearName(MasterDataIds.FiscalYears.FY2025_26),
                OpeningDate = new DateTime(2025, 4, 1),
                EntryMode = "ManualEntry",
                RestrictToBalanceSheetAccounts = true,
                Status = "Posted",
                ApprovedBy = "Finance Head",
                ApprovedAt = new DateTime(2025, 3, 29),
                PostedBy = "System",
                PostedAt = new DateTime(2025, 4, 1),
                Notes = "CozyCraft Hyderabad HQ opening balance",
                Lines = new List<OpeningBalanceLineModel>
                {
                    new OpeningBalanceLineModel { Id = Guid.NewGuid(), AccountId = MasterDataIds.Accounts.RentExpense, AccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.RentExpense), AccountName = SeedLookup.AccountName(MasterDataIds.Accounts.RentExpense), DebitAmountBase = 300000.00m, CreditAmountBase = 0m, LineDescription = "Bank balance" },
                    new OpeningBalanceLineModel { Id = Guid.NewGuid(), AccountId = MasterDataIds.Accounts.ShareCapital, AccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.ShareCapital), AccountName = SeedLookup.AccountName(MasterDataIds.Accounts.ShareCapital), DebitAmountBase = 0m, CreditAmountBase = 300000.00m, LineDescription = "Capital" }
                },
                CreatedAt = DateTime.Now.AddDays(-100),
                CreatedBy = "System"
            },
            new OpeningBalanceModel
            {
                Id = MasterDataIds.OpeningBalances.OB07,
                TenantId = MasterDataIds.Tenants.Default,
                OpeningBalanceNumber = "OB-2025-00007",
                CompanyId = MasterDataIds.SupplementaryCompanies.UrbanLoft3001,
                CompanyCode = SeedLookup.CompanyCode(MasterDataIds.Companies.SofaCraft),
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                BranchId = MasterDataIds.Branches.UrbanLoftMumbai,
                BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.UrbanLoftMumbai),
                BranchName = SeedLookup.BranchName(MasterDataIds.Branches.UrbanLoftMumbai),
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = SeedLookup.LedgerCode(MasterDataIds.Ledgers.PrimaryLedger),
                LedgerName = SeedLookup.LedgerName(MasterDataIds.Ledgers.PrimaryLedger),
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearName = SeedLookup.FiscalYearName(MasterDataIds.FiscalYears.FY2025_26),
                OpeningDate = new DateTime(2025, 4, 1),
                EntryMode = "ManualEntry",
                RestrictToBalanceSheetAccounts = true,
                Status = "Posted",
                ApprovedBy = "CFO",
                ApprovedAt = new DateTime(2025, 3, 28),
                PostedBy = "System",
                PostedAt = new DateTime(2025, 4, 1),
                Notes = "UrbanLoft Mumbai HQ opening balance",
                Lines = new List<OpeningBalanceLineModel>
                {
                    new OpeningBalanceLineModel { Id = Guid.NewGuid(), AccountId = MasterDataIds.Accounts.RentExpense, AccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.RentExpense), AccountName = SeedLookup.AccountName(MasterDataIds.Accounts.RentExpense), DebitAmountBase = 300000.00m, CreditAmountBase = 0m, LineDescription = "Bank balance" },
                    new OpeningBalanceLineModel { Id = Guid.NewGuid(), AccountId = MasterDataIds.Accounts.ShareCapital, AccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.ShareCapital), AccountName = SeedLookup.AccountName(MasterDataIds.Accounts.ShareCapital), DebitAmountBase = 0m, CreditAmountBase = 300000.00m, LineDescription = "Capital" }
                },
                CreatedAt = DateTime.Now.AddDays(-100),
                CreatedBy = "System"
            },
            new OpeningBalanceModel
            {
                Id = MasterDataIds.OpeningBalances.OB08,
                TenantId = MasterDataIds.Tenants.Default,
                OpeningBalanceNumber = "OB-2025-00008",
                CompanyId = MasterDataIds.SupplementaryCompanies.PlushComfort6001,
                CompanyCode = SeedLookup.CompanyCode(MasterDataIds.Companies.SofaCraft),
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                BranchId = MasterDataIds.Branches.PlushComfortDelhi,
                BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.PlushComfortDelhi),
                BranchName = SeedLookup.BranchName(MasterDataIds.Branches.PlushComfortDelhi),
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = SeedLookup.LedgerCode(MasterDataIds.Ledgers.PrimaryLedger),
                LedgerName = SeedLookup.LedgerName(MasterDataIds.Ledgers.PrimaryLedger),
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearName = SeedLookup.FiscalYearName(MasterDataIds.FiscalYears.FY2025_26),
                OpeningDate = new DateTime(2025, 4, 1),
                EntryMode = "ManualEntry",
                RestrictToBalanceSheetAccounts = true,
                Status = "Approved",
                ApprovedBy = "Finance Manager",
                ApprovedAt = new DateTime(2025, 3, 30),
                Notes = "PlushComfort Delhi opening balance",
                Lines = new List<OpeningBalanceLineModel>
                {
                    new OpeningBalanceLineModel { Id = Guid.NewGuid(), AccountId = MasterDataIds.Accounts.RentExpense, AccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.RentExpense), AccountName = SeedLookup.AccountName(MasterDataIds.Accounts.RentExpense), DebitAmountBase = 300000.00m, CreditAmountBase = 0m, LineDescription = "Bank balance" },
                    new OpeningBalanceLineModel { Id = Guid.NewGuid(), AccountId = MasterDataIds.Accounts.ShareCapital, AccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.ShareCapital), AccountName = SeedLookup.AccountName(MasterDataIds.Accounts.ShareCapital), DebitAmountBase = 0m, CreditAmountBase = 300000.00m, LineDescription = "Capital" }
                },
                CreatedAt = DateTime.Now.AddDays(-98),
                CreatedBy = "System"
            },
            new OpeningBalanceModel
            {
                Id = MasterDataIds.OpeningBalances.OB09,
                TenantId = MasterDataIds.Tenants.Default,
                OpeningBalanceNumber = "OB-2025-00009",
                CompanyId = MasterDataIds.SupplementaryCompanies.VelvetRest7001,
                CompanyCode = SeedLookup.CompanyCode(MasterDataIds.Companies.SofaCraft),
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                BranchId = MasterDataIds.Branches.VelvetRestPune,
                BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.VelvetRestPune),
                BranchName = SeedLookup.BranchName(MasterDataIds.Branches.VelvetRestPune),
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = SeedLookup.LedgerCode(MasterDataIds.Ledgers.PrimaryLedger),
                LedgerName = SeedLookup.LedgerName(MasterDataIds.Ledgers.PrimaryLedger),
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearName = SeedLookup.FiscalYearName(MasterDataIds.FiscalYears.FY2025_26),
                OpeningDate = new DateTime(2025, 4, 1),
                EntryMode = "ManualEntry",
                RestrictToBalanceSheetAccounts = true,
                Status = "Submitted",
                Notes = "VelvetRest Pune - awaiting approval",
                Lines = new List<OpeningBalanceLineModel>
                {
                    new OpeningBalanceLineModel { Id = Guid.NewGuid(), AccountId = MasterDataIds.Accounts.RentExpense, AccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.RentExpense), AccountName = SeedLookup.AccountName(MasterDataIds.Accounts.RentExpense), DebitAmountBase = 300000.00m, CreditAmountBase = 0m, LineDescription = "Bank balance" },
                    new OpeningBalanceLineModel { Id = Guid.NewGuid(), AccountId = MasterDataIds.Accounts.ShareCapital, AccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.ShareCapital), AccountName = SeedLookup.AccountName(MasterDataIds.Accounts.ShareCapital), DebitAmountBase = 0m, CreditAmountBase = 300000.00m, LineDescription = "Capital" }
                },
                CreatedAt = DateTime.Now.AddDays(-92),
                CreatedBy = "System"
            },
            new OpeningBalanceModel
            {
                Id = MasterDataIds.OpeningBalances.OB_0A,
                TenantId = MasterDataIds.Tenants.Default,
                OpeningBalanceNumber = "OB-2025-00010",
                CompanyId = MasterDataIds.SupplementaryCompanies.PremiumSeating9001,
                CompanyCode = SeedLookup.CompanyCode(MasterDataIds.Companies.SofaCraft),
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                BranchId = MasterDataIds.Branches.PremiumSeatingSG,
                BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.PremiumSeatingSG),
                BranchName = SeedLookup.BranchName(MasterDataIds.Branches.PremiumSeatingSG),
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = SeedLookup.LedgerCode(MasterDataIds.Ledgers.PrimaryLedger),
                LedgerName = SeedLookup.LedgerName(MasterDataIds.Ledgers.PrimaryLedger),
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearName = SeedLookup.FiscalYearName(MasterDataIds.FiscalYears.FY2025_26),
                OpeningDate = new DateTime(2025, 4, 1),
                EntryMode = "BulkImport",
                RestrictToBalanceSheetAccounts = true,
                Status = "Posted",
                ApprovedBy = "Director",
                ApprovedAt = new DateTime(2025, 3, 31),
                PostedBy = "System",
                PostedAt = new DateTime(2025, 4, 1),
                Notes = "PremiumSeating Singapore opening balance",
                Lines = new List<OpeningBalanceLineModel>
                {
                    new OpeningBalanceLineModel { Id = Guid.NewGuid(), AccountId = MasterDataIds.Accounts.RentExpense, AccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.RentExpense), AccountName = SeedLookup.AccountName(MasterDataIds.Accounts.RentExpense), DebitAmountBase = 300000.00m, CreditAmountBase = 0m, LineDescription = "Bank balance" },
                    new OpeningBalanceLineModel { Id = Guid.NewGuid(), AccountId = MasterDataIds.Accounts.ShareCapital, AccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.ShareCapital), AccountName = SeedLookup.AccountName(MasterDataIds.Accounts.ShareCapital), DebitAmountBase = 0m, CreditAmountBase = 300000.00m, LineDescription = "Capital" }
                },
                CreatedAt = DateTime.Now.AddDays(-100),
                CreatedBy = "System"
            },
            new OpeningBalanceModel
            {
                Id = MasterDataIds.OpeningBalances.OB_0B,
                TenantId = MasterDataIds.Tenants.Default,
                OpeningBalanceNumber = "OB-2025-00011",
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = SeedLookup.CompanyCode(MasterDataIds.Companies.SofaCraft),
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                BranchId = MasterDataIds.Branches.SofaCraftHQ,
                BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftHQ),
                BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftHQ),
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = SeedLookup.LedgerCode(MasterDataIds.Ledgers.PrimaryLedger),
                LedgerName = SeedLookup.LedgerName(MasterDataIds.Ledgers.PrimaryLedger),
                FiscalYearId = MasterDataIds.FiscalYears.FY2025_26,
                FiscalYearName = SeedLookup.FiscalYearName(MasterDataIds.FiscalYears.FY2025_26),
                OpeningDate = new DateTime(2025, 4, 1),
                EntryMode = "ManualEntry",
                RestrictToBalanceSheetAccounts = true,
                Status = "Cancelled",
                Notes = "Chennai branch - cancelled due to restructuring",
                Lines = new List<OpeningBalanceLineModel>
                {
                    new OpeningBalanceLineModel { Id = Guid.NewGuid(), AccountId = MasterDataIds.Accounts.RentExpense, AccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.RentExpense), AccountName = SeedLookup.AccountName(MasterDataIds.Accounts.RentExpense), DebitAmountBase = 300000.00m, CreditAmountBase = 0m, LineDescription = "Bank balance" },
                    new OpeningBalanceLineModel { Id = Guid.NewGuid(), AccountId = MasterDataIds.Accounts.ShareCapital, AccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.ShareCapital), AccountName = SeedLookup.AccountName(MasterDataIds.Accounts.ShareCapital), DebitAmountBase = 0m, CreditAmountBase = 300000.00m, LineDescription = "Capital" }
                },
                CreatedAt = DateTime.Now.AddDays(-85),
                CreatedBy = "System"
            },
            new OpeningBalanceModel
            {
                Id = MasterDataIds.OpeningBalances.OB_0C,
                TenantId = MasterDataIds.Tenants.Default,
                OpeningBalanceNumber = "OB-2024-00001",
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyCode = SeedLookup.CompanyCode(MasterDataIds.Companies.SofaCraft),
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                BranchId = MasterDataIds.Branches.SofaCraftHQ,
                BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftHQ),
                BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftHQ),
                LedgerId = MasterDataIds.Ledgers.PrimaryLedger,
                LedgerCode = SeedLookup.LedgerCode(MasterDataIds.Ledgers.PrimaryLedger),
                LedgerName = SeedLookup.LedgerName(MasterDataIds.Ledgers.PrimaryLedger),
                FiscalYearId = MasterDataIds.FiscalYears.FY2024_25,
                FiscalYearName = SeedLookup.FiscalYearName(MasterDataIds.FiscalYears.FY2024_25),
                OpeningDate = new DateTime(2024, 4, 1),
                EntryMode = "ManualEntry",
                RestrictToBalanceSheetAccounts = true,
                Status = "Posted",
                ApprovedBy = "Controller",
                ApprovedAt = new DateTime(2024, 3, 30),
                PostedBy = "System",
                PostedAt = new DateTime(2024, 4, 1),
                Notes = "Prior year opening balance FY 2024-25",
                Lines = new List<OpeningBalanceLineModel>
                {
                    new OpeningBalanceLineModel { Id = Guid.NewGuid(), AccountId = MasterDataIds.Accounts.RentExpense, AccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.RentExpense), AccountName = SeedLookup.AccountName(MasterDataIds.Accounts.RentExpense), DebitAmountBase = 300000.00m, CreditAmountBase = 0m, LineDescription = "Bank balance" },
                    new OpeningBalanceLineModel { Id = Guid.NewGuid(), AccountId = MasterDataIds.Accounts.ShareCapital, AccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.ShareCapital), AccountName = SeedLookup.AccountName(MasterDataIds.Accounts.ShareCapital), DebitAmountBase = 0m, CreditAmountBase = 300000.00m, LineDescription = "Capital" }
                },
                CreatedAt = DateTime.Now.AddDays(-460),
                CreatedBy = "System"
            }
            };
        }
    }
}
