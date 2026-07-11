using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    /// <summary>
    /// Demo data service for Chart of Accounts module - no API calls.
    /// Companies are sourced from MasterDataService (CompanySeedData) — no hardcoding.
    /// </summary>
    public class COADataService
    {
        #region Private Data Collections

        private readonly MasterDataService _masterDataService;

        private List<ChartOfAccountsViewModel> _chartOfAccounts = new();
        private List<AccountGroupViewModel> _accountGroups = new();
        private List<AccountViewModel> _accounts = new();
        private bool _isInitialized = false;

        #endregion

        #region Constructor & Initialization

        public COADataService(MasterDataService masterDataService)
        {
            _masterDataService = masterDataService;
            InitializeSeedData();
        }

        private void InitializeSeedData()
        {
            if (_isInitialized) return;

            // ── Pull all active companies from MasterDataService (sourced from CompanySeedData) ──
            var allCompanies = _masterDataService.GetAllCompanies()
                .Where(c => c.Status == "Active")
                .ToList();

            // Build a quick lookup by MasterDataIds
            var sofaCraft       = allCompanies.First(c => c.Id == MasterDataIds.Companies.SofaCraft);
            var sofaCraftUsa    = allCompanies.First(c => c.Id == MasterDataIds.Companies.SofaCraftUSA);
            var oakNest         = allCompanies.First(c => c.Id == MasterDataIds.Companies.OakNest);
            var urbanLoft       = allCompanies.First(c => c.Id == MasterDataIds.Companies.UrbanLoft);
            var desertDune      = allCompanies.First(c => c.Id == MasterDataIds.Companies.DesertDune);
            var plushComfort    = allCompanies.First(c => c.Id == MasterDataIds.Companies.PlushComfort);
            var velvetRest      = allCompanies.First(c => c.Id == MasterDataIds.Companies.VelvetRest);
            var cozyCraft       = allCompanies.First(c => c.Id == MasterDataIds.Companies.CozyCraft);
            var premiumSeating  = allCompanies.First(c => c.Id == MasterDataIds.Companies.PremiumSeating);
            var cloudSofa       = allCompanies.First(c => c.Id == MasterDataIds.Companies.CloudSofa);
            var eliteLoungers   = allCompanies.First(c => c.Id == MasterDataIds.Companies.EliteLoungers);

            // ── COA IDs (stable references for Account Groups & Accounts) ──
            var coaSofaCraftId       = Guid.Parse("c0a00001-0001-0001-0001-000000000001");
            var coaSofaCraftUsaId    = Guid.Parse("c0a00002-0002-0002-0002-000000000002");
            var coaOakNestId         = Guid.Parse("c0a00003-0003-0003-0003-000000000003");
            var coaSofaCraftRetId    = Guid.Parse("c0a00004-0004-0004-0004-000000000004");
            var coaSofaCraftUsaRetId = Guid.Parse("c0a00005-0005-0005-0005-000000000005");
            var coaUrbanLoftId       = Guid.Parse("c0a00006-0006-0006-0006-000000000006");
            var coaDesertDuneId      = Guid.Parse("c0a00007-0007-0007-0007-000000000007");
            var coaPlushComfortId    = Guid.Parse("c0a00008-0008-0008-0008-000000000008");
            var coaVelvetRestId      = Guid.Parse("c0a00009-0009-0009-0009-000000000009");
            var coaCozyCraftId       = Guid.Parse("c0a00010-0010-0010-0010-000000000010");
            var coaPremiumSeatingId  = Guid.Parse("c0a00011-0011-0011-0011-000000000011");
            var coaCloudSofaId       = Guid.Parse("c0a00012-0012-0012-0012-000000000012");
            var coaEliteLoungersId   = Guid.Parse("c0a00013-0013-0013-0013-000000000013");

            // ── Seed Chart of Accounts — one row per company, each mapped from CompanySeedData ──
            _chartOfAccounts = new List<ChartOfAccountsViewModel>
            {
                // 1. SofaCraft (India) — Standard, Active, FY Apr 2025
                new ChartOfAccountsViewModel
                {
                    Id = coaSofaCraftId,
                    ChartCode = "COA-SOFA-2025",
                    ChartName = $"{sofaCraft.ShortName} Standard Chart - FY 2025",
                    CompanyId = sofaCraft.Id,
                    CompanyCode = sofaCraft.CompanyCode,
                    CompanyName = sofaCraft.LegalName,
                    ChartType = ChartTypes.Standard,
                    Status = COAStatuses.Active,
                    IsDefaultForCompany = true,
                    EffectiveFrom = new DateTime(2025, 4, 1),
                    EffectiveTo = new DateTime(2026, 3, 31),
                    AccountCodeMode = AccountCodeModes.Manual,
                    AccountCodeFormat = "NNNN",
                    EnforceUniqueAccountCode = true,
                    CreatedBy = "Admin",
                    AccountGroupCount = 8,
                    AccountCount = 15
                },
                // 2. SofaCraft USA — Standard, Active, FY Jan 2025
                new ChartOfAccountsViewModel
                {
                    Id = coaSofaCraftUsaId,
                    ChartCode = "COA-SFUSA-2025",
                    ChartName = $"{sofaCraftUsa.ShortName} Standard Chart - FY 2025",
                    CompanyId = sofaCraftUsa.Id,
                    CompanyCode = sofaCraftUsa.CompanyCode,
                    CompanyName = sofaCraftUsa.LegalName,
                    ChartType = ChartTypes.Standard,
                    Status = COAStatuses.Active,
                    IsDefaultForCompany = true,
                    EffectiveFrom = new DateTime(2025, 1, 1),
                    EffectiveTo = new DateTime(2025, 12, 31),
                    AccountCodeMode = AccountCodeModes.Manual,
                    AccountCodeFormat = "NNNN-NN",
                    EnforceUniqueAccountCode = true,
                    CreatedBy = "Admin",
                    AccountGroupCount = 5,
                    AccountCount = 10
                },
                // 3. OakNest (India - Bengaluru) — Standard, Active, FY Apr 2025
                new ChartOfAccountsViewModel
                {
                    Id = coaOakNestId,
                    ChartCode = "COA-OAK-2025",
                    ChartName = $"{oakNest.ShortName} Standard Chart - FY 2025",
                    CompanyId = oakNest.Id,
                    CompanyCode = oakNest.CompanyCode,
                    CompanyName = oakNest.LegalName,
                    ChartType = ChartTypes.Standard,
                    Status = COAStatuses.Active,
                    IsDefaultForCompany = true,
                    EffectiveFrom = new DateTime(2025, 4, 1),
                    EffectiveTo = new DateTime(2026, 3, 31),
                    AccountCodeMode = AccountCodeModes.Manual,
                    AccountCodeFormat = "NNNN",
                    EnforceUniqueAccountCode = true,
                    CreatedBy = "Admin",
                    AccountGroupCount = 6,
                    AccountCount = 12
                },
                // 4. SofaCraft (India) — Standard, Retired, FY Apr 2024
                new ChartOfAccountsViewModel
                {
                    Id = coaSofaCraftRetId,
                    ChartCode = "COA-SOFA-2024",
                    ChartName = $"{sofaCraft.ShortName} Standard Chart - FY 2024",
                    CompanyId = sofaCraft.Id,
                    CompanyCode = sofaCraft.CompanyCode,
                    CompanyName = sofaCraft.LegalName,
                    ChartType = ChartTypes.Standard,
                    Status = COAStatuses.Retired,
                    IsDefaultForCompany = false,
                    EffectiveFrom = new DateTime(2024, 4, 1),
                    EffectiveTo = new DateTime(2025, 3, 31),
                    AccountCodeMode = AccountCodeModes.Manual,
                    AccountCodeFormat = "NNNN",
                    EnforceUniqueAccountCode = true,
                    CreatedBy = "Admin",
                    AccountGroupCount = 8,
                    AccountCount = 14
                },
                // 5. SofaCraft USA — Standard, Retired, FY Jan 2024
                new ChartOfAccountsViewModel
                {
                    Id = coaSofaCraftUsaRetId,
                    ChartCode = "COA-SFUSA-2024",
                    ChartName = $"{sofaCraftUsa.ShortName} Standard Chart - FY 2024",
                    CompanyId = sofaCraftUsa.Id,
                    CompanyCode = sofaCraftUsa.CompanyCode,
                    CompanyName = sofaCraftUsa.LegalName,
                    ChartType = ChartTypes.Standard,
                    Status = COAStatuses.Retired,
                    IsDefaultForCompany = false,
                    EffectiveFrom = new DateTime(2024, 1, 1),
                    EffectiveTo = new DateTime(2024, 12, 31),
                    AccountCodeMode = AccountCodeModes.Manual,
                    AccountCodeFormat = "NNNN-NN",
                    EnforceUniqueAccountCode = true,
                    CreatedBy = "Admin",
                    AccountGroupCount = 5,
                    AccountCount = 9
                },
                // 6. UrbanLoft (India - Mumbai) — Template, Active, FY Apr 2025
                new ChartOfAccountsViewModel
                {
                    Id = coaUrbanLoftId,
                    ChartCode = "COA-URBN-2025",
                    ChartName = $"{urbanLoft.ShortName} Management Chart - FY 2025",
                    CompanyId = urbanLoft.Id,
                    CompanyCode = urbanLoft.CompanyCode,
                    CompanyName = urbanLoft.LegalName,
                    ChartType = ChartTypes.Template,
                    Status = COAStatuses.Active,
                    IsDefaultForCompany = true,
                    EffectiveFrom = new DateTime(2025, 4, 1),
                    EffectiveTo = new DateTime(2026, 3, 31),
                    AccountCodeMode = AccountCodeModes.AutoNumber,
                    AccountCodeFormat = "MG-NNNN",
                    EnforceUniqueAccountCode = true,
                    CreatedBy = "FinanceController",
                    AccountGroupCount = 4,
                    AccountCount = 8
                },
                // 7. DesertDune (UAE - Dubai) — Standard, Active, FY Jan 2025
                new ChartOfAccountsViewModel
                {
                    Id = coaDesertDuneId,
                    ChartCode = "COA-DUNE-2025",
                    ChartName = $"{desertDune.ShortName} Standard Chart - FY 2025",
                    CompanyId = desertDune.Id,
                    CompanyCode = desertDune.CompanyCode,
                    CompanyName = desertDune.LegalName,
                    ChartType = ChartTypes.Standard,
                    Status = COAStatuses.Active,
                    IsDefaultForCompany = true,
                    EffectiveFrom = new DateTime(2025, 1, 1),
                    EffectiveTo = new DateTime(2025, 12, 31),
                    AccountCodeMode = AccountCodeModes.Manual,
                    AccountCodeFormat = "NNNN",
                    EnforceUniqueAccountCode = true,
                    CreatedBy = "Admin",
                    AccountGroupCount = 5,
                    AccountCount = 10
                },
                // 8. PlushComfort (India - New Delhi) — Migration, Active, FY Apr 2025
                new ChartOfAccountsViewModel
                {
                    Id = coaPlushComfortId,
                    ChartCode = "COA-PLSH-2025",
                    ChartName = $"{plushComfort.ShortName} IFRS Chart - FY 2025",
                    CompanyId = plushComfort.Id,
                    CompanyCode = plushComfort.CompanyCode,
                    CompanyName = plushComfort.LegalName,
                    ChartType = ChartTypes.Migration,
                    Status = COAStatuses.Active,
                    IsDefaultForCompany = true,
                    EffectiveFrom = new DateTime(2025, 4, 1),
                    EffectiveTo = new DateTime(2026, 3, 31),
                    AccountCodeMode = AccountCodeModes.Manual,
                    AccountCodeFormat = "IFRS-NNNN",
                    EnforceUniqueAccountCode = true,
                    CreatedBy = "FinanceController",
                    AccountGroupCount = 7,
                    AccountCount = 18
                },
                // 9. VelvetRest (India - Pune) — Template, Draft, FY Apr 2025
                new ChartOfAccountsViewModel
                {
                    Id = coaVelvetRestId,
                    ChartCode = "COA-VLVT-2025",
                    ChartName = $"{velvetRest.ShortName} Management Chart - FY 2025",
                    CompanyId = velvetRest.Id,
                    CompanyCode = velvetRest.CompanyCode,
                    CompanyName = velvetRest.LegalName,
                    ChartType = ChartTypes.Template,
                    Status = COAStatuses.Draft,
                    IsDefaultForCompany = false,
                    EffectiveFrom = new DateTime(2025, 4, 1),
                    EffectiveTo = new DateTime(2026, 3, 31),
                    AccountCodeMode = AccountCodeModes.AutoNumber,
                    AccountCodeFormat = "MG-NNNN",
                    EnforceUniqueAccountCode = true,
                    CreatedBy = "Admin",
                    AccountGroupCount = 4,
                    AccountCount = 7
                },
                // 10. CozyCraft (India - Hyderabad) — Standard, Active, FY Apr 2025
                new ChartOfAccountsViewModel
                {
                    Id = coaCozyCraftId,
                    ChartCode = "COA-COZY-2025",
                    ChartName = $"{cozyCraft.ShortName} Standard Chart - FY 2025",
                    CompanyId = cozyCraft.Id,
                    CompanyCode = cozyCraft.CompanyCode,
                    CompanyName = cozyCraft.LegalName,
                    ChartType = ChartTypes.Standard,
                    Status = COAStatuses.Active,
                    IsDefaultForCompany = true,
                    EffectiveFrom = new DateTime(2025, 4, 1),
                    EffectiveTo = new DateTime(2026, 3, 31),
                    AccountCodeMode = AccountCodeModes.Manual,
                    AccountCodeFormat = "TAX-NNNN",
                    EnforceUniqueAccountCode = true,
                    CreatedBy = "TaxAdmin",
                    AccountGroupCount = 5,
                    AccountCount = 11
                },
                // 11. PremiumSeating (Singapore) — Standard, Active, FY Jan 2025
                new ChartOfAccountsViewModel
                {
                    Id = coaPremiumSeatingId,
                    ChartCode = "COA-PREM-2025",
                    ChartName = $"{premiumSeating.ShortName} Standard Chart - FY 2025",
                    CompanyId = premiumSeating.Id,
                    CompanyCode = premiumSeating.CompanyCode,
                    CompanyName = premiumSeating.LegalName,
                    ChartType = ChartTypes.Standard,
                    Status = COAStatuses.Active,
                    IsDefaultForCompany = true,
                    EffectiveFrom = new DateTime(2025, 1, 1),
                    EffectiveTo = new DateTime(2025, 12, 31),
                    AccountCodeMode = AccountCodeModes.Manual,
                    AccountCodeFormat = "NNNN",
                    EnforceUniqueAccountCode = true,
                    CreatedBy = "Admin",
                    AccountGroupCount = 6,
                    AccountCount = 11
                },
                // 12. CloudSofa (India - Kolkata) — Standard, Draft, FY Apr 2025
                new ChartOfAccountsViewModel
                {
                    Id = coaCloudSofaId,
                    ChartCode = "COA-CLOD-2025",
                    ChartName = $"{cloudSofa.ShortName} Standard Chart - FY 2025",
                    CompanyId = cloudSofa.Id,
                    CompanyCode = cloudSofa.CompanyCode,
                    CompanyName = cloudSofa.LegalName,
                    ChartType = ChartTypes.Standard,
                    Status = COAStatuses.Draft,
                    IsDefaultForCompany = false,
                    EffectiveFrom = new DateTime(2025, 4, 1),
                    EffectiveTo = new DateTime(2026, 3, 31),
                    AccountCodeMode = AccountCodeModes.Manual,
                    AccountCodeFormat = "NNNN",
                    EnforceUniqueAccountCode = true,
                    CreatedBy = "Admin",
                    AccountGroupCount = 4,
                    AccountCount = 8
                },
                // 13. EliteLoungers (UAE - Abu Dhabi) — Template, Draft, FY Jan 2025
                new ChartOfAccountsViewModel
                {
                    Id = coaEliteLoungersId,
                    ChartCode = "COA-ELTE-2025",
                    ChartName = $"{eliteLoungers.ShortName} Tax Chart - FY 2025",
                    CompanyId = eliteLoungers.Id,
                    CompanyCode = eliteLoungers.CompanyCode,
                    CompanyName = eliteLoungers.LegalName,
                    ChartType = ChartTypes.Template,
                    Status = COAStatuses.Draft,
                    IsDefaultForCompany = false,
                    EffectiveFrom = new DateTime(2025, 1, 1),
                    EffectiveTo = new DateTime(2025, 12, 31),
                    AccountCodeMode = AccountCodeModes.Manual,
                    AccountCodeFormat = "NNNN",
                    EnforceUniqueAccountCode = true,
                    CreatedBy = "Admin",
                    AccountGroupCount = 4,
                    AccountCount = 8
                }
            };

            // ── Seed Account Groups & Accounts for ALL COAs ──
            _accountGroups = new List<AccountGroupViewModel>();
            _accounts = new List<AccountViewModel>();

            // 1. SofaCraft India (COA-SOFA-2025) — 8 groups, 15 accounts (hand-crafted with sub-groups)
            var sfCoaCode = "COA-SOFA-2025";
            var sfCoaName = $"{sofaCraft.ShortName} Standard Chart - FY 2025";
            var grpSF_AST  = Guid.Parse("d0000001-0001-0001-0001-000000000001");
            var grpSF_CA   = Guid.Parse("d0000002-0002-0002-0002-000000000002");
            var grpSF_FA   = Guid.Parse("d0000003-0003-0003-0003-000000000003");
            var grpSF_LIA  = Guid.Parse("d0000004-0004-0004-0004-000000000004");
            var grpSF_EQU  = Guid.Parse("d0000005-0005-0005-0005-000000000005");
            var grpSF_REV  = Guid.Parse("d0000006-0006-0006-0006-000000000006");
            var grpSF_COGS = Guid.Parse("d0000007-0007-0007-0007-000000000007");
            var grpSF_EXP  = Guid.Parse("d0000008-0008-0008-0008-000000000008");

            _accountGroups.AddRange(new[]
            {
                new AccountGroupViewModel { Id = grpSF_AST, ChartOfAccountsId = coaSofaCraftId, ChartOfAccountsCode = sfCoaCode, ChartOfAccountsName = sfCoaName, GroupCode = "AST", GroupName = "Assets", AccountNature = AccountNatures.Asset, StatementType = StatementTypes.BalanceSheet, Status = GroupStatuses.Active, DisplayOrder = 1, AccountCount = 4 },
                new AccountGroupViewModel { Id = grpSF_CA, ChartOfAccountsId = coaSofaCraftId, ChartOfAccountsCode = sfCoaCode, ChartOfAccountsName = sfCoaName, GroupCode = "AST-CA", GroupName = "Current Assets", ParentGroupId = grpSF_AST, ParentGroupCode = "AST", ParentGroupName = "Assets", AccountNature = AccountNatures.Asset, StatementType = StatementTypes.BalanceSheet, HierarchyLevel = 1, Status = GroupStatuses.Active, DisplayOrder = 2, AccountCount = 3 },
                new AccountGroupViewModel { Id = grpSF_FA, ChartOfAccountsId = coaSofaCraftId, ChartOfAccountsCode = sfCoaCode, ChartOfAccountsName = sfCoaName, GroupCode = "AST-FA", GroupName = "Fixed Assets", ParentGroupId = grpSF_AST, ParentGroupCode = "AST", ParentGroupName = "Assets", AccountNature = AccountNatures.Asset, StatementType = StatementTypes.BalanceSheet, HierarchyLevel = 1, Status = GroupStatuses.Active, DisplayOrder = 3, AccountCount = 1 },
                new AccountGroupViewModel { Id = grpSF_LIA, ChartOfAccountsId = coaSofaCraftId, ChartOfAccountsCode = sfCoaCode, ChartOfAccountsName = sfCoaName, GroupCode = "LIA", GroupName = "Liabilities", AccountNature = AccountNatures.Liability, StatementType = StatementTypes.BalanceSheet, Status = GroupStatuses.Active, DisplayOrder = 4, AccountCount = 3 },
                new AccountGroupViewModel { Id = grpSF_EQU, ChartOfAccountsId = coaSofaCraftId, ChartOfAccountsCode = sfCoaCode, ChartOfAccountsName = sfCoaName, GroupCode = "EQU", GroupName = "Equity", AccountNature = AccountNatures.Equity, StatementType = StatementTypes.BalanceSheet, Status = GroupStatuses.Active, DisplayOrder = 5, AccountCount = 2 },
                new AccountGroupViewModel { Id = grpSF_REV, ChartOfAccountsId = coaSofaCraftId, ChartOfAccountsCode = sfCoaCode, ChartOfAccountsName = sfCoaName, GroupCode = "REV", GroupName = "Revenue", AccountNature = AccountNatures.Income, StatementType = StatementTypes.ProfitAndLoss, Status = GroupStatuses.Active, DisplayOrder = 6, AccountCount = 2 },
                new AccountGroupViewModel { Id = grpSF_COGS, ChartOfAccountsId = coaSofaCraftId, ChartOfAccountsCode = sfCoaCode, ChartOfAccountsName = sfCoaName, GroupCode = "COGS", GroupName = "Cost of Goods Sold", AccountNature = AccountNatures.Expense, StatementType = StatementTypes.ProfitAndLoss, Status = GroupStatuses.Active, DisplayOrder = 7, AccountCount = 1 },
                new AccountGroupViewModel { Id = grpSF_EXP, ChartOfAccountsId = coaSofaCraftId, ChartOfAccountsCode = sfCoaCode, ChartOfAccountsName = sfCoaName, GroupCode = "EXP", GroupName = "Expenses", AccountNature = AccountNatures.Expense, StatementType = StatementTypes.ProfitAndLoss, Status = GroupStatuses.Active, DisplayOrder = 8, AccountCount = 2 }
            });

            _accounts.AddRange(new[]
            {
                NewAccount("a0000001-0001-0001-0001-000000000001", "1001", "Petty Cash", coaSofaCraftId, sfCoaCode, grpSF_CA, "AST-CA", "Current Assets", AccountNatures.Asset, StatementTypes.BalanceSheet, BalanceBehaviors.Debit, 1, isCash: true),
                NewAccount("a0000002-0002-0002-0002-000000000002", "1002", "HDFC Bank - Current Account", coaSofaCraftId, sfCoaCode, grpSF_CA, "AST-CA", "Current Assets", AccountNatures.Asset, StatementTypes.BalanceSheet, BalanceBehaviors.Debit, 2, isBank: true, bankName: "HDFC Bank", bankAcctNo: "50100123456789", bankBranch: "Anna Nagar, Chennai", swift: "HDFCINBB"),
                NewAccount("a0000003-0003-0003-0003-000000000003", "1003", "Accounts Receivable", coaSofaCraftId, sfCoaCode, grpSF_CA, "AST-CA", "Current Assets", AccountNatures.Asset, StatementTypes.BalanceSheet, BalanceBehaviors.Debit, 3, isControl: true, ctrlType: ControlAccountTypes.AccountsReceivable),
                NewAccount("a0000004-0004-0004-0004-000000000004", "1100", "Furniture & Fixtures", coaSofaCraftId, sfCoaCode, grpSF_FA, "AST-FA", "Fixed Assets", AccountNatures.Asset, StatementTypes.BalanceSheet, BalanceBehaviors.Debit, 1),
                NewAccount("a0000005-0005-0005-0005-000000000005", "2001", "Accounts Payable", coaSofaCraftId, sfCoaCode, grpSF_LIA, "LIA", "Liabilities", AccountNatures.Liability, StatementTypes.BalanceSheet, BalanceBehaviors.Credit, 1, isControl: true, ctrlType: ControlAccountTypes.AccountsPayable),
                NewAccount("a0000006-0006-0006-0006-000000000006", "2002", "GST Payable", coaSofaCraftId, sfCoaCode, grpSF_LIA, "LIA", "Liabilities", AccountNatures.Liability, StatementTypes.BalanceSheet, BalanceBehaviors.Credit, 2, isTax: true, taxType: AccountTaxTypes.GSTOutput, taxRate: 18),
                NewAccount("a0000007-0007-0007-0007-000000000007", "2003", "TDS Payable", coaSofaCraftId, sfCoaCode, grpSF_LIA, "LIA", "Liabilities", AccountNatures.Liability, StatementTypes.BalanceSheet, BalanceBehaviors.Credit, 3, isTax: true, taxType: AccountTaxTypes.TDS),
                NewAccount("a0000008-0008-0008-0008-000000000008", "3001", "Share Capital", coaSofaCraftId, sfCoaCode, grpSF_EQU, "EQU", "Equity", AccountNatures.Equity, StatementTypes.BalanceSheet, BalanceBehaviors.Credit, 1),
                NewAccount("a0000009-0009-0009-0009-000000000009", "3002", "Retained Earnings", coaSofaCraftId, sfCoaCode, grpSF_EQU, "EQU", "Equity", AccountNatures.Equity, StatementTypes.BalanceSheet, BalanceBehaviors.Credit, 2),
                NewAccount("a0000010-0010-0010-0010-000000000010", "4001", "Sales Revenue - Sofas", coaSofaCraftId, sfCoaCode, grpSF_REV, "REV", "Revenue", AccountNatures.Income, StatementTypes.ProfitAndLoss, BalanceBehaviors.Credit, 1, reportCat: ReportingCategories.OperatingRevenue),
                NewAccount("a0000011-0011-0011-0011-000000000011", "4002", "Service Revenue", coaSofaCraftId, sfCoaCode, grpSF_REV, "REV", "Revenue", AccountNatures.Income, StatementTypes.ProfitAndLoss, BalanceBehaviors.Credit, 2, reportCat: ReportingCategories.OperatingRevenue),
                NewAccount("a0000012-0012-0012-0012-000000000012", "5001", "Cost of Materials", coaSofaCraftId, sfCoaCode, grpSF_COGS, "COGS", "Cost of Goods Sold", AccountNatures.Expense, StatementTypes.ProfitAndLoss, BalanceBehaviors.Debit, 1, reportCat: ReportingCategories.DirectExpense),
                NewAccount("a0000013-0013-0013-0013-000000000013", "6001", "Salaries & Wages", coaSofaCraftId, sfCoaCode, grpSF_EXP, "EXP", "Expenses", AccountNatures.Expense, StatementTypes.ProfitAndLoss, BalanceBehaviors.Debit, 1, reportCat: ReportingCategories.IndirectExpense),
                NewAccount("a0000014-0014-0014-0014-000000000014", "6002", "Rent Expense", coaSofaCraftId, sfCoaCode, grpSF_EXP, "EXP", "Expenses", AccountNatures.Expense, StatementTypes.ProfitAndLoss, BalanceBehaviors.Debit, 2, reportCat: ReportingCategories.IndirectExpense),
                NewAccount("a0000015-0015-0015-0015-000000000015", "6003", "Utilities Expense", coaSofaCraftId, sfCoaCode, grpSF_EXP, "EXP", "Expenses", AccountNatures.Expense, StatementTypes.ProfitAndLoss, BalanceBehaviors.Debit, 3, reportCat: ReportingCategories.IndirectExpense)
            });

            // 2. SofaCraft USA (COA-SFUSA-2025) — 5 groups, 10 accounts
            var sfuCoaCode = "COA-SFUSA-2025";
            var sfuCoaName = $"{sofaCraftUsa.ShortName} Standard Chart - FY 2025";
            var grpSFU_AST = Guid.Parse("d0000009-0009-0009-0009-000000000009");
            var grpSFU_LIA = Guid.Parse("d0000010-0010-0010-0010-000000000010");
            var grpSFU_EQU = Guid.Parse("d0000011-0011-0011-0011-000000000011");
            var grpSFU_REV = Guid.Parse("d0000012-0012-0012-0012-000000000012");
            var grpSFU_EXP = Guid.Parse("d0000013-0013-0013-0013-000000000013");

            _accountGroups.AddRange(new[]
            {
                new AccountGroupViewModel { Id = grpSFU_AST, ChartOfAccountsId = coaSofaCraftUsaId, ChartOfAccountsCode = sfuCoaCode, ChartOfAccountsName = sfuCoaName, GroupCode = "1000", GroupName = "Assets", AccountNature = AccountNatures.Asset, StatementType = StatementTypes.BalanceSheet, Status = GroupStatuses.Active, DisplayOrder = 1, AccountCount = 4 },
                new AccountGroupViewModel { Id = grpSFU_LIA, ChartOfAccountsId = coaSofaCraftUsaId, ChartOfAccountsCode = sfuCoaCode, ChartOfAccountsName = sfuCoaName, GroupCode = "2000", GroupName = "Liabilities", AccountNature = AccountNatures.Liability, StatementType = StatementTypes.BalanceSheet, Status = GroupStatuses.Active, DisplayOrder = 2, AccountCount = 2 },
                new AccountGroupViewModel { Id = grpSFU_EQU, ChartOfAccountsId = coaSofaCraftUsaId, ChartOfAccountsCode = sfuCoaCode, ChartOfAccountsName = sfuCoaName, GroupCode = "3000", GroupName = "Equity", AccountNature = AccountNatures.Equity, StatementType = StatementTypes.BalanceSheet, Status = GroupStatuses.Active, DisplayOrder = 3, AccountCount = 1 },
                new AccountGroupViewModel { Id = grpSFU_REV, ChartOfAccountsId = coaSofaCraftUsaId, ChartOfAccountsCode = sfuCoaCode, ChartOfAccountsName = sfuCoaName, GroupCode = "4000", GroupName = "Revenue", AccountNature = AccountNatures.Income, StatementType = StatementTypes.ProfitAndLoss, Status = GroupStatuses.Active, DisplayOrder = 4, AccountCount = 2 },
                new AccountGroupViewModel { Id = grpSFU_EXP, ChartOfAccountsId = coaSofaCraftUsaId, ChartOfAccountsCode = sfuCoaCode, ChartOfAccountsName = sfuCoaName, GroupCode = "5000", GroupName = "Expenses", AccountNature = AccountNatures.Expense, StatementType = StatementTypes.ProfitAndLoss, Status = GroupStatuses.Active, DisplayOrder = 5, AccountCount = 1 }
            });

            _accounts.AddRange(new[]
            {
                NewAccount("a1000001-0001-0001-0001-000000000001", "1001-01", "Checking Account - Chase", coaSofaCraftUsaId, sfuCoaCode, grpSFU_AST, "1000", "Assets", AccountNatures.Asset, StatementTypes.BalanceSheet, BalanceBehaviors.Debit, 1, isBank: true, bankName: "JPMorgan Chase"),
                NewAccount("a1000002-0002-0002-0002-000000000002", "1002-01", "Petty Cash - SF Office", coaSofaCraftUsaId, sfuCoaCode, grpSFU_AST, "1000", "Assets", AccountNatures.Asset, StatementTypes.BalanceSheet, BalanceBehaviors.Debit, 2, isCash: true),
                NewAccount("a1000003-0003-0003-0003-000000000003", "1100-01", "Accounts Receivable", coaSofaCraftUsaId, sfuCoaCode, grpSFU_AST, "1000", "Assets", AccountNatures.Asset, StatementTypes.BalanceSheet, BalanceBehaviors.Debit, 3, isControl: true, ctrlType: ControlAccountTypes.AccountsReceivable),
                NewAccount("a1000004-0004-0004-0004-000000000004", "1200-01", "Office Equipment", coaSofaCraftUsaId, sfuCoaCode, grpSFU_AST, "1000", "Assets", AccountNatures.Asset, StatementTypes.BalanceSheet, BalanceBehaviors.Debit, 4),
                NewAccount("a1000005-0005-0005-0005-000000000005", "2001-01", "Accounts Payable", coaSofaCraftUsaId, sfuCoaCode, grpSFU_LIA, "2000", "Liabilities", AccountNatures.Liability, StatementTypes.BalanceSheet, BalanceBehaviors.Credit, 1, isControl: true, ctrlType: ControlAccountTypes.AccountsPayable),
                NewAccount("a1000006-0006-0006-0006-000000000006", "2100-01", "Sales Tax Payable", coaSofaCraftUsaId, sfuCoaCode, grpSFU_LIA, "2000", "Liabilities", AccountNatures.Liability, StatementTypes.BalanceSheet, BalanceBehaviors.Credit, 2, isTax: true, taxType: AccountTaxTypes.VAT),
                NewAccount("a1000007-0007-0007-0007-000000000007", "3001-01", "Common Stock", coaSofaCraftUsaId, sfuCoaCode, grpSFU_EQU, "3000", "Equity", AccountNatures.Equity, StatementTypes.BalanceSheet, BalanceBehaviors.Credit, 1),
                NewAccount("a1000008-0008-0008-0008-000000000008", "4001-01", "Retail Sales Revenue", coaSofaCraftUsaId, sfuCoaCode, grpSFU_REV, "4000", "Revenue", AccountNatures.Income, StatementTypes.ProfitAndLoss, BalanceBehaviors.Credit, 1, reportCat: ReportingCategories.OperatingRevenue),
                NewAccount("a1000009-0009-0009-0009-000000000009", "4002-01", "Online Sales Revenue", coaSofaCraftUsaId, sfuCoaCode, grpSFU_REV, "4000", "Revenue", AccountNatures.Income, StatementTypes.ProfitAndLoss, BalanceBehaviors.Credit, 2, reportCat: ReportingCategories.OperatingRevenue),
                NewAccount("a1000010-0010-0010-0010-000000000010", "5001-01", "Operating Expenses", coaSofaCraftUsaId, sfuCoaCode, grpSFU_EXP, "5000", "Expenses", AccountNatures.Expense, StatementTypes.ProfitAndLoss, BalanceBehaviors.Debit, 1, reportCat: ReportingCategories.IndirectExpense)
            });

            // 3–13: Remaining COAs — generated via helper
            SeedStandardCOA(coaOakNestId, "COA-OAK-2025", $"{oakNest.ShortName} Standard Chart - FY 2025", "d1030", "a1030", 6, 12, GroupStatuses.Active);
            SeedStandardCOA(coaSofaCraftRetId, "COA-SOFA-2024", $"{sofaCraft.ShortName} Standard Chart - FY 2024", "d1040", "a1040", 8, 14, GroupStatuses.Active);
            SeedStandardCOA(coaSofaCraftUsaRetId, "COA-SFUSA-2024", $"{sofaCraftUsa.ShortName} Standard Chart - FY 2024", "d1050", "a1050", 5, 9, GroupStatuses.Active);
            SeedStandardCOA(coaUrbanLoftId, "COA-URBN-2025", $"{urbanLoft.ShortName} Management Chart - FY 2025", "d1060", "a1060", 4, 8, GroupStatuses.Active);
            SeedStandardCOA(coaDesertDuneId, "COA-DUNE-2025", $"{desertDune.ShortName} Standard Chart - FY 2025", "d1070", "a1070", 5, 10, GroupStatuses.Active);
            SeedStandardCOA(coaPlushComfortId, "COA-PLSH-2025", $"{plushComfort.ShortName} IFRS Chart - FY 2025", "d1080", "a1080", 7, 18, GroupStatuses.Active);
            SeedStandardCOA(coaVelvetRestId, "COA-VLVT-2025", $"{velvetRest.ShortName} Management Chart - FY 2025", "d1090", "a1090", 4, 7, GroupStatuses.Draft);
            SeedStandardCOA(coaCozyCraftId, "COA-COZY-2025", $"{cozyCraft.ShortName} Standard Chart - FY 2025", "d1100", "a1100", 5, 11, GroupStatuses.Active);
            SeedStandardCOA(coaPremiumSeatingId, "COA-PREM-2025", $"{premiumSeating.ShortName} Standard Chart - FY 2025", "d1110", "a1110", 6, 11, GroupStatuses.Active);
            SeedStandardCOA(coaCloudSofaId, "COA-CLOD-2025", $"{cloudSofa.ShortName} Standard Chart - FY 2025", "d1120", "a1120", 4, 8, GroupStatuses.Draft);
            SeedStandardCOA(coaEliteLoungersId, "COA-ELTE-2025", $"{eliteLoungers.ShortName} Tax Chart - FY 2025", "d1130", "a1130", 4, 8, GroupStatuses.Draft);

            _isInitialized = true;
        }

        #endregion

        #region Seed Helpers

        /// <summary>
        /// Helper to create an AccountViewModel with common defaults.
        /// </summary>
        private static AccountViewModel NewAccount(string id, string code, string name,
            Guid coaId, string coaCode, Guid grpId, string grpCode, string grpName,
            string nature, string stmtType, string normalBal, int order,
            bool isCash = false, bool isBank = false, bool isControl = false,
            string? ctrlType = null, bool isTax = false, string? taxType = null,
            decimal taxRate = 0, string? reportCat = null,
            string? bankName = null, string? bankAcctNo = null, string? bankBranch = null, string? swift = null)
        {
            return new AccountViewModel
            {
                Id = Guid.Parse(id),
                AccountCode = code,
                AccountName = name,
                ChartOfAccountsId = coaId,
                ChartOfAccountsCode = coaCode,
                AccountGroupId = grpId,
                AccountGroupCode = grpCode,
                AccountGroupName = grpName,
                AccountNature = nature,
                StatementType = stmtType,
                NormalBalance = normalBal,
                IsBalanceSheetAccount = stmtType == StatementTypes.BalanceSheet,
                IsPostable = true,
                IsCashAccount = isCash,
                IsBankAccount = isBank,
                IsReconcilable = isBank,
                IsControlAccount = isControl,
                ControlAccountType = ctrlType,
                IsTaxAccount = isTax,
                TaxType = taxType,
                TaxRate = taxRate,
                ReportingCategory = reportCat,
                RequiresBranch = true,
                AllowManualJournal = !isControl,
                BankName = bankName,
                BankAccountNumber = bankAcctNo,
                BankBranch = bankBranch,
                SwiftCode = swift,
                DisplayOrder = order,
                LockStatus = LockStatuses.Unlocked,
                Status = AccountStatuses.Active
            };
        }

        /// <summary>
        /// Generates standard account groups and accounts for a COA using deterministic GUIDs.
        /// </summary>
        private void SeedStandardCOA(Guid coaId, string coaCode, string coaName,
            string groupPrefix, string acctPrefix, int groupCount, int accountCount, string groupStatus)
        {
            var groupTemplates = new List<(string Code, string Name, string Nature, string Statement, int Order)>
            {
                ("AST",  "Assets",             AccountNatures.Asset,     StatementTypes.BalanceSheet,   1),
                ("LIA",  "Liabilities",         AccountNatures.Liability, StatementTypes.BalanceSheet,   2),
                ("EQU",  "Equity",              AccountNatures.Equity,    StatementTypes.BalanceSheet,   3),
                ("REV",  "Revenue",             AccountNatures.Income,    StatementTypes.ProfitAndLoss,  4),
                ("EXP",  "Expenses",            AccountNatures.Expense,   StatementTypes.ProfitAndLoss,  5),
                ("COGS", "Cost of Goods Sold",  AccountNatures.Expense,   StatementTypes.ProfitAndLoss,  6),
                ("OTH-I","Other Income",        AccountNatures.Income,    StatementTypes.ProfitAndLoss,  7),
                ("OTH-E","Other Expenses",      AccountNatures.Expense,   StatementTypes.ProfitAndLoss,  8),
            };

            var accountTemplates = new Dictionary<string, List<(string Code, string Name, string NormalBal, bool IsCtrl, string? CtrlType, string? RptCat)>>
            {
                ["AST"] = new()
                {
                    ("1001", "Cash on Hand",         BalanceBehaviors.Debit,  false, null, null),
                    ("1002", "Bank Account",          BalanceBehaviors.Debit,  false, null, null),
                    ("1003", "Accounts Receivable",   BalanceBehaviors.Debit,  true,  ControlAccountTypes.AccountsReceivable, null),
                    ("1100", "Fixed Assets",          BalanceBehaviors.Debit,  false, null, null),
                },
                ["LIA"] = new()
                {
                    ("2001", "Accounts Payable",      BalanceBehaviors.Credit, true,  ControlAccountTypes.AccountsPayable, null),
                    ("2002", "Tax Payable",           BalanceBehaviors.Credit, false, null, null),
                    ("2003", "Accrued Expenses",      BalanceBehaviors.Credit, false, null, null),
                },
                ["EQU"] = new()
                {
                    ("3001", "Share Capital",         BalanceBehaviors.Credit, false, null, null),
                    ("3002", "Retained Earnings",     BalanceBehaviors.Credit, false, null, null),
                },
                ["REV"] = new()
                {
                    ("4001", "Product Sales",         BalanceBehaviors.Credit, false, null, ReportingCategories.OperatingRevenue),
                    ("4002", "Service Revenue",       BalanceBehaviors.Credit, false, null, ReportingCategories.OperatingRevenue),
                },
                ["EXP"] = new()
                {
                    ("6001", "Salaries & Wages",      BalanceBehaviors.Debit,  false, null, ReportingCategories.IndirectExpense),
                    ("6002", "Rent Expense",          BalanceBehaviors.Debit,  false, null, ReportingCategories.IndirectExpense),
                    ("6003", "Utilities Expense",     BalanceBehaviors.Debit,  false, null, ReportingCategories.IndirectExpense),
                },
                ["COGS"] = new()
                {
                    ("5001", "Cost of Materials",     BalanceBehaviors.Debit,  false, null, ReportingCategories.DirectExpense),
                    ("5002", "Direct Labour",         BalanceBehaviors.Debit,  false, null, ReportingCategories.DirectExpense),
                },
                ["OTH-I"] = new()
                {
                    ("4501", "Interest Income",       BalanceBehaviors.Credit, false, null, ReportingCategories.OtherIncome),
                    ("4502", "Foreign Exchange Gain",  BalanceBehaviors.Credit, false, null, ReportingCategories.OtherIncome),
                },
                ["OTH-E"] = new()
                {
                    ("6501", "Bank Charges",          BalanceBehaviors.Debit,  false, null, ReportingCategories.OtherExpense),
                    ("6502", "Depreciation",          BalanceBehaviors.Debit,  false, null, ReportingCategories.OtherExpense),
                },
            };

            int actualGroupCount = Math.Min(groupCount, groupTemplates.Count);
            int acctSeq = 0;

            for (int g = 0; g < actualGroupCount; g++)
            {
                var gt = groupTemplates[g];
                var gId = Guid.Parse($"{groupPrefix}{g + 1:D3}-{g + 1:D4}-{g + 1:D4}-{g + 1:D4}-{g + 1:D12}");

                int grpAcctCount = accountTemplates.ContainsKey(gt.Code) ? Math.Min(accountTemplates[gt.Code].Count, accountCount - acctSeq) : 0;
                if (grpAcctCount < 0) grpAcctCount = 0;

                _accountGroups.Add(new AccountGroupViewModel
                {
                    Id = gId,
                    ChartOfAccountsId = coaId,
                    ChartOfAccountsCode = coaCode,
                    ChartOfAccountsName = coaName,
                    GroupCode = gt.Code,
                    GroupName = gt.Name,
                    AccountNature = gt.Nature,
                    StatementType = gt.Statement,
                    Status = groupStatus,
                    DisplayOrder = gt.Order,
                    AccountCount = grpAcctCount
                });

                if (accountTemplates.TryGetValue(gt.Code, out var templates))
                {
                    for (int a = 0; a < templates.Count && acctSeq < accountCount; a++, acctSeq++)
                    {
                        var at = templates[a];
                        bool isBS = gt.Statement == StatementTypes.BalanceSheet;

                        _accounts.Add(new AccountViewModel
                        {
                            Id = Guid.Parse($"{acctPrefix}{acctSeq + 1:D3}-{acctSeq + 1:D4}-{acctSeq + 1:D4}-{acctSeq + 1:D4}-{acctSeq + 1:D12}"),
                            AccountCode = at.Code,
                            AccountName = at.Name,
                            ChartOfAccountsId = coaId,
                            ChartOfAccountsCode = coaCode,
                            AccountGroupId = gId,
                            AccountGroupCode = gt.Code,
                            AccountGroupName = gt.Name,
                            AccountNature = gt.Nature,
                            StatementType = gt.Statement,
                            NormalBalance = at.NormalBal,
                            IsBalanceSheetAccount = isBS,
                            IsPostable = true,
                            IsControlAccount = at.IsCtrl,
                            ControlAccountType = at.CtrlType,
                            ReportingCategory = at.RptCat,
                            RequiresBranch = true,
                            AllowManualJournal = !at.IsCtrl,
                            DisplayOrder = a + 1,
                            LockStatus = LockStatuses.Unlocked,
                            Status = AccountStatuses.Active
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Resets all data back to the original seed data
        /// </summary>
        public void ResetToSeedData()
        {
            _isInitialized = false;
            _chartOfAccounts.Clear();
            _accountGroups.Clear();
            _accounts.Clear();
            InitializeSeedData();
        }

        #endregion

        #region Company Operations

        /// <summary>
        /// Returns all active companies from MasterDataService (sourced from CompanySeedData).
        /// No hardcoded company data in this service.
        /// </summary>
        public Task<List<CompanyModel>> GetCompaniesAsync()
        {
            var companies = _masterDataService.GetAllCompanies()
                .Where(c => c.Status == "Active")
                .OrderBy(c => c.CompanyCode)
                .ToList();
            return Task.FromResult(companies);
        }

        #endregion

        #region Chart of Accounts Operations

        public Task<List<ChartOfAccountsViewModel>> GetChartOfAccountsAsync()
        {
            var result = _chartOfAccounts.Where(c => !c.IsDeleted).OrderBy(c => c.ChartCode).ToList();
            return Task.FromResult(result);
        }

        public Task<ChartOfAccountsViewModel?> GetChartOfAccountsByIdAsync(Guid id)
        {
            var result = _chartOfAccounts.FirstOrDefault(c => c.Id == id && !c.IsDeleted);
            return Task.FromResult(result);
        }

        public Task<bool> AddChartOfAccountsAsync(ChartOfAccountsViewModel model)
        {
            model.CreatedAt = DateTime.UtcNow;
            _chartOfAccounts.Add(model);
            return Task.FromResult(true);
        }

        public Task<bool> UpdateChartOfAccountsAsync(ChartOfAccountsViewModel model)
        {
            var existing = _chartOfAccounts.FirstOrDefault(c => c.Id == model.Id);
            if (existing == null) return Task.FromResult(false);

            var index = _chartOfAccounts.IndexOf(existing);
            model.UpdatedAt = DateTime.UtcNow;
            _chartOfAccounts[index] = model;
            return Task.FromResult(true);
        }

        public Task<bool> DeleteChartOfAccountsAsync(Guid id)
        {
            var existing = _chartOfAccounts.FirstOrDefault(c => c.Id == id);
            if (existing == null) return Task.FromResult(false);

            existing.IsDeleted = true;
            existing.DeletedAt = DateTime.UtcNow;
            return Task.FromResult(true);
        }

        public Task<bool> CanDeleteChartOfAccounts(Guid id)
        {
            var hasGroups = _accountGroups.Any(g => g.ChartOfAccountsId == id && !g.IsDeleted);
            var hasAccounts = _accounts.Any(a => a.ChartOfAccountsId == id && !a.IsDeleted);
            return Task.FromResult(!hasGroups && !hasAccounts);
        }

        public Task<bool> IsChartCodeDuplicateAsync(string chartCode, Guid companyId, Guid? excludeId = null)
        {
            var exists = _chartOfAccounts.Any(c =>
                c.ChartCode.Equals(chartCode, StringComparison.OrdinalIgnoreCase) &&
                c.CompanyId == companyId &&
                !c.IsDeleted &&
                (excludeId == null || c.Id != excludeId));
            return Task.FromResult(exists);
        }

        public Task<bool> UpdateChartStatusAsync(Guid id, string status)
        {
            var existing = _chartOfAccounts.FirstOrDefault(c => c.Id == id);
            if (existing == null) return Task.FromResult(false);

            existing.Status = status;
            existing.UpdatedAt = DateTime.UtcNow;
            return Task.FromResult(true);
        }

        #endregion

        #region Account Group Operations

        public Task<List<AccountGroupViewModel>> GetAccountGroupsAsync()
        {
            var result = _accountGroups.Where(g => !g.IsDeleted).OrderBy(g => g.DisplayOrder).ThenBy(g => g.GroupCode).ToList();
            return Task.FromResult(result);
        }

        public Task<List<AccountGroupViewModel>> GetAccountGroupsByChartIdAsync(Guid chartId)
        {
            var result = _accountGroups.Where(g => g.ChartOfAccountsId == chartId && !g.IsDeleted)
                .OrderBy(g => g.DisplayOrder).ThenBy(g => g.GroupCode).ToList();
            return Task.FromResult(result);
        }

        public Task<AccountGroupViewModel?> GetAccountGroupByIdAsync(Guid id)
        {
            var result = _accountGroups.FirstOrDefault(g => g.Id == id && !g.IsDeleted);
            return Task.FromResult(result);
        }

        public Task<bool> AddAccountGroupAsync(AccountGroupViewModel model)
        {
            model.CreatedAt = DateTime.UtcNow;
            _accountGroups.Add(model);
            return Task.FromResult(true);
        }

        public Task<bool> UpdateAccountGroupAsync(AccountGroupViewModel model)
        {
            var existing = _accountGroups.FirstOrDefault(g => g.Id == model.Id);
            if (existing == null) return Task.FromResult(false);

            var index = _accountGroups.IndexOf(existing);
            model.UpdatedAt = DateTime.UtcNow;
            _accountGroups[index] = model;
            return Task.FromResult(true);
        }

        public Task<bool> DeleteAccountGroupAsync(Guid id)
        {
            var existing = _accountGroups.FirstOrDefault(g => g.Id == id);
            if (existing == null) return Task.FromResult(false);

            existing.IsDeleted = true;
            existing.DeletedAt = DateTime.UtcNow;
            return Task.FromResult(true);
        }

        public Task<bool> CanDeleteAccountGroup(Guid id)
        {
            var hasAccounts = _accounts.Any(a => a.AccountGroupId == id && !a.IsDeleted);
            var hasChildGroups = _accountGroups.Any(g => g.ParentGroupId == id && !g.IsDeleted);
            return Task.FromResult(!hasAccounts && !hasChildGroups);
        }

        public Task<bool> IsGroupCodeDuplicateAsync(string groupCode, Guid chartId, Guid? excludeId = null)
        {
            var exists = _accountGroups.Any(g =>
                g.GroupCode.Equals(groupCode, StringComparison.OrdinalIgnoreCase) &&
                g.ChartOfAccountsId == chartId &&
                !g.IsDeleted &&
                (excludeId == null || g.Id != excludeId));
            return Task.FromResult(exists);
        }

        #endregion

        #region Account Operations

        public Task<List<AccountViewModel>> GetAccountsAsync()
        {
            var result = _accounts.Where(a => !a.IsDeleted).OrderBy(a => a.AccountCode).ToList();
            return Task.FromResult(result);
        }

        public List<AccountViewModel> GetAllAccounts()
        {
            return _accounts.Where(a => !a.IsDeleted).OrderBy(a => a.AccountCode).ToList();
        }

        public Task<List<AccountViewModel>> GetAccountsByChartIdAsync(Guid chartId)
        {
            var result = _accounts.Where(a => a.ChartOfAccountsId == chartId && !a.IsDeleted)
                .OrderBy(a => a.AccountCode).ToList();
            return Task.FromResult(result);
        }

        public Task<List<AccountViewModel>> GetAccountsByGroupIdAsync(Guid groupId)
        {
            var result = _accounts.Where(a => a.AccountGroupId == groupId && !a.IsDeleted)
                .OrderBy(a => a.AccountCode).ToList();
            return Task.FromResult(result);
        }

        public Task<AccountViewModel?> GetAccountByIdAsync(Guid id)
        {
            var result = _accounts.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
            return Task.FromResult(result);
        }

        public Task<bool> AddAccountAsync(AccountViewModel model)
        {
            model.CreatedAt = DateTime.UtcNow;
            _accounts.Add(model);
            return Task.FromResult(true);
        }

        public Task<bool> UpdateAccountAsync(AccountViewModel model)
        {
            var existing = _accounts.FirstOrDefault(a => a.Id == model.Id);
            if (existing == null) return Task.FromResult(false);

            var index = _accounts.IndexOf(existing);
            model.UpdatedAt = DateTime.UtcNow;
            _accounts[index] = model;
            return Task.FromResult(true);
        }

        public Task<bool> DeleteAccountAsync(Guid id)
        {
            var existing = _accounts.FirstOrDefault(a => a.Id == id);
            if (existing == null) return Task.FromResult(false);

            existing.IsDeleted = true;
            existing.DeletedAt = DateTime.UtcNow;
            return Task.FromResult(true);
        }

        public Task<bool> CanDeleteAccount(Guid id)
        {
            var account = _accounts.FirstOrDefault(a => a.Id == id);
            if (account == null) return Task.FromResult(false);

            // Can delete if no transactions (simulated by TransactionCount)
            return Task.FromResult(account.TransactionCount == 0);
        }

        public Task<bool> IsAccountCodeDuplicateAsync(string accountCode, Guid chartId, Guid? excludeId = null)
        {
            var exists = _accounts.Any(a =>
                a.AccountCode.Equals(accountCode, StringComparison.OrdinalIgnoreCase) &&
                a.ChartOfAccountsId == chartId &&
                !a.IsDeleted &&
                (excludeId == null || a.Id != excludeId));
            return Task.FromResult(exists);
        }

        #endregion
    }
}
