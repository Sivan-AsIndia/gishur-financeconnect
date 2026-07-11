using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    /// <summary>
    /// Seed data for APAdjustment model (Model #41)
    /// </summary>
    public static class APAdjustmentSeedData
    {
        // Company GUIDs (matching existing company seed data)
        private static readonly Guid AscendingSoftwareCompanyId = MasterDataIds.Companies.SofaCraft;

        // Branch GUIDs (matching existing branch seed data)
        private static readonly Guid ChennaiHQBranchId = MasterDataIds.Branches.SofaCraftHQ;
        private static readonly Guid BangaloreBranchId = MasterDataIds.Branches.SofaCraftBengaluru;

        // Vendor GUIDs (matching VendorSeedData)
        private static readonly Guid Vendor1Id = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000001"); // Tech Components India
        private static readonly Guid Vendor2Id = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000002"); // CloudTech Solutions
        private static readonly Guid Vendor3Id = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000003"); // Reliable Supplies Co
        private static readonly Guid Vendor4Id = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000004"); // Professional Consulting
        private static readonly Guid Vendor5Id = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000005"); // City Utilities
        private static readonly Guid Vendor6Id = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000006"); // Prime Properties

        // Currency GUIDs
        private static readonly Guid InrCurrencyId = MasterDataIds.Currencies.INR;

        // GL Account GUIDs
        private static readonly Guid APControlAccountId = Guid.Parse("a0000005-0005-0005-0005-000000000050");
        private static readonly Guid WriteOffExpenseAccountId = MasterDataIds.Accounts.CostOfMaterials;
        private static readonly Guid RoundingExpenseAccountId = MasterDataIds.Accounts.UtilitiesExpense;
        private static readonly Guid DisputeExpenseAccountId = MasterDataIds.Accounts.CostOfMaterials;
        private static readonly Guid ReclassificationAccountId = MasterDataIds.Accounts.AccountsPayable;
        private static readonly Guid APWriteOffIncomeAccountId = MasterDataIds.Accounts.ServiceRevenue;

        // Bill GUIDs (matching VendorBillSeedData)
        private static readonly Guid Bill1Id = Guid.Parse("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e001");
        private static readonly Guid Bill2Id = Guid.Parse("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e002");
        private static readonly Guid Bill3Id = Guid.Parse("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e003");

        // Reason Code GUIDs
        public static readonly Guid ReasonSmallBalanceId = Guid.Parse("f3f3f3f3-0001-0001-0001-000000000001");
        public static readonly Guid ReasonRoundingId = Guid.Parse("f3f3f3f3-0001-0001-0001-000000000002");
        public static readonly Guid ReasonDisputeId = Guid.Parse("f3f3f3f3-0001-0001-0001-000000000003");
        public static readonly Guid ReasonReclassId = Guid.Parse("f3f3f3f3-0001-0001-0001-000000000004");
        public static readonly Guid ReasonVendorTransferId = Guid.Parse("f3f3f3f3-0001-0001-0001-000000000005");
        public static readonly Guid ReasonOtherId = Guid.Parse("f3f3f3f3-0001-0001-0001-000000000006");

        // Predefined Adjustment GUIDs
        public static readonly Guid Adjustment1Id = Guid.Parse("f4f4f4f4-0001-0001-0001-000000000001");
        public static readonly Guid Adjustment2Id = Guid.Parse("f4f4f4f4-0001-0001-0001-000000000002");
        public static readonly Guid Adjustment3Id = Guid.Parse("f4f4f4f4-0001-0001-0001-000000000003");
        public static readonly Guid Adjustment4Id = Guid.Parse("f4f4f4f4-0001-0001-0001-000000000004");
        public static readonly Guid Adjustment5Id = Guid.Parse("f4f4f4f4-0001-0001-0001-000000000005");
        public static readonly Guid Adjustment6Id = Guid.Parse("f4f4f4f4-0001-0001-0001-000000000006");

        public static readonly Guid Adjustment7Id = Guid.Parse("f4f4f4f4-0001-0001-0001-000000000007");
        public static readonly Guid Adjustment8Id = Guid.Parse("f4f4f4f4-0001-0001-0001-000000000008");
        public static readonly Guid Adjustment9Id = Guid.Parse("f4f4f4f4-0001-0001-0001-000000000009");
        public static readonly Guid Adjustment10Id = Guid.Parse("f4f4f4f4-0001-0001-0001-000000000010");
        public static readonly Guid Adjustment11Id = Guid.Parse("f4f4f4f4-0001-0001-0001-000000000011");
        public static readonly Guid Adjustment12Id = Guid.Parse("f4f4f4f4-0001-0001-0001-000000000012");

        public static List<APAdjustmentReasonViewModel> GetSeedReasons()
        {
            return new List<APAdjustmentReasonViewModel>
            {
                new APAdjustmentReasonViewModel
                {
                    Id = ReasonSmallBalanceId,
                    CompanyId = AscendingSoftwareCompanyId,
                    ReasonCode = "AP-SML-BAL",
                    ReasonDescription = "Small Balance Write-Off",
                    ApplicableTypes = new[] { APAdjustmentTypes.WriteOff },
                    DefaultOffsetAccountId = APWriteOffIncomeAccountId,
                    DefaultOffsetAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.ServiceRevenue),
                    DefaultOffsetAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.ServiceRevenue),
                    RequiresApproval = false,
                    RequiresEvidence = false,
                    ApprovalThreshold = 1000,
                    IsActive = true,
                    CreatedAt = DateTime.Today.AddDays(-90),
                    CreatedBy = "System Admin"
                },
                new APAdjustmentReasonViewModel
                {
                    Id = ReasonRoundingId,
                    CompanyId = AscendingSoftwareCompanyId,
                    ReasonCode = "AP-ROUNDING",
                    ReasonDescription = "Rounding Difference Adjustment",
                    ApplicableTypes = new[] { APAdjustmentTypes.RoundOffCorrection },
                    DefaultOffsetAccountId = RoundingExpenseAccountId,
                    DefaultOffsetAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.ServiceRevenue),
                    DefaultOffsetAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.ServiceRevenue),
                    RequiresApproval = false,
                    RequiresEvidence = false,
                    ApprovalThreshold = 100,
                    IsActive = true,
                    CreatedAt = DateTime.Today.AddDays(-90),
                    CreatedBy = "System Admin"
                },
                new APAdjustmentReasonViewModel
                {
                    Id = ReasonDisputeId,
                    CompanyId = AscendingSoftwareCompanyId,
                    ReasonCode = "AP-DISPUTE",
                    ReasonDescription = "Dispute Settlement Write-Off",
                    ApplicableTypes = new[] { APAdjustmentTypes.DisputeSettlement },
                    DefaultOffsetAccountId = DisputeExpenseAccountId,
                    DefaultOffsetAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.ServiceRevenue),
                    DefaultOffsetAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.ServiceRevenue),
                    RequiresApproval = true,
                    RequiresEvidence = true,
                    ApprovalThreshold = 5000,
                    IsActive = true,
                    CreatedAt = DateTime.Today.AddDays(-90),
                    CreatedBy = "System Admin"
                },
                new APAdjustmentReasonViewModel
                {
                    Id = ReasonReclassId,
                    CompanyId = AscendingSoftwareCompanyId,
                    ReasonCode = "AP-RECLASS",
                    ReasonDescription = "Reclassification to Other Liability",
                    ApplicableTypes = new[] { APAdjustmentTypes.Reclassification },
                    DefaultOffsetAccountId = ReclassificationAccountId,
                    DefaultOffsetAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.ServiceRevenue),
                    DefaultOffsetAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.ServiceRevenue),
                    RequiresApproval = true,
                    RequiresEvidence = false,
                    ApprovalThreshold = 10000,
                    IsActive = true,
                    CreatedAt = DateTime.Today.AddDays(-90),
                    CreatedBy = "System Admin"
                },
                new APAdjustmentReasonViewModel
                {
                    Id = ReasonVendorTransferId,
                    CompanyId = AscendingSoftwareCompanyId,
                    ReasonCode = "AP-TRANSFER",
                    ReasonDescription = "Vendor Balance Transfer",
                    ApplicableTypes = new[] { APAdjustmentTypes.VendorBalanceTransfer },
                    DefaultOffsetAccountId = APControlAccountId,
                    DefaultOffsetAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.ServiceRevenue),
                    DefaultOffsetAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.ServiceRevenue),
                    RequiresApproval = true,
                    RequiresEvidence = true,
                    ApprovalThreshold = 0,
                    IsActive = true,
                    CreatedAt = DateTime.Today.AddDays(-90),
                    CreatedBy = "System Admin"
                },
                new APAdjustmentReasonViewModel
                {
                    Id = ReasonOtherId,
                    CompanyId = AscendingSoftwareCompanyId,
                    ReasonCode = "AP-OTHER",
                    ReasonDescription = "Other Adjustment Reason",
                    ApplicableTypes = APAdjustmentTypes.All,
                    DefaultOffsetAccountId = WriteOffExpenseAccountId,
                    DefaultOffsetAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.ServiceRevenue),
                    DefaultOffsetAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.ServiceRevenue),
                    RequiresApproval = true,
                    RequiresEvidence = true,
                    ApprovalThreshold = 1000,
                    IsActive = true,
                    CreatedAt = DateTime.Today.AddDays(-90),
                    CreatedBy = "System Admin"
                }
            };
        }

        public static List<APAdjustmentViewModel> GetSeedAdjustments()
        {
            var adjustments = new List<APAdjustmentViewModel>
            {
                // Adjustment 1: Posted - Small balance write-off (Vendor Level)
                new APAdjustmentViewModel
                {
                    APAdjustmentId = Adjustment1Id,
                    TenantId = MasterDataIds.Tenants.Default,
                    CompanyId = AscendingSoftwareCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    VendorId = Vendor1Id,
                    VendorCode = "VND-000001",
                    VendorName = "Tech Components India Pvt Ltd",
                    AdjustmentNumber = "APADJ-2025-0001",
                    AdjustmentDate = DateTime.Today.AddDays(-15),
                    PostingDate = DateTime.Today.AddDays(-15),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    AdjustmentType = APAdjustmentTypes.WriteOff,
                    AdjustmentDirection = APAdjustmentDirections.ReducePayable,
                    ReasonCodeId = ReasonSmallBalanceId,
                    ReasonCode = "AP-SML-BAL",
                    ReasonDescription = "Small Balance Write-Off",
                    PolicyLimitCategory = APPolicyLimitCategories.SmallWriteOff,
                    AdjustmentScope = APAdjustmentScopes.VendorLevel,
                    AdjustmentAmount = 25.00m,
                    APControlAccountIdSnapshot = APControlAccountId,
                    APControlAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.ServiceRevenue),
                    APControlAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.ServiceRevenue),
                    AdjustmentGLAccountId = APWriteOffIncomeAccountId,
                    AdjustmentGLAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.ServiceRevenue),
                    AdjustmentGLAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.ServiceRevenue),
                    AdjustmentGLAccountType = APAdjustmentGLAccountTypes.Income,
                    Narration = "Small remaining balance write-off after final payment reconciliation",
                    AdjustmentStatus = APAdjustmentStatuses.Posted,
                    SubmittedOn = DateTime.Today.AddDays(-15),
                    SubmittedByUserName = "AP Supervisor",
                    PostedOn = DateTime.Today.AddDays(-15),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedByUserName = "Finance Controller",
                    HasAttachments = false,
                    AttachmentCount = 0,
                    EvidenceRequired = false,
                    CreatedAt = DateTime.Today.AddDays(-16),
                    CreatedBy = "AP Supervisor"
                },

                // Adjustment 2: Posted - Rounding correction (Bill Level)
                new APAdjustmentViewModel
                {
                    APAdjustmentId = Adjustment2Id,
                    TenantId = MasterDataIds.Tenants.Default,
                    CompanyId = AscendingSoftwareCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    VendorId = Vendor2Id,
                    VendorCode = "VND-000002",
                    VendorName = "CloudTech Solutions",
                    AdjustmentNumber = "APADJ-2025-0002",
                    AdjustmentDate = DateTime.Today.AddDays(-10),
                    PostingDate = DateTime.Today.AddDays(-10),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    AdjustmentType = APAdjustmentTypes.RoundOffCorrection,
                    AdjustmentDirection = APAdjustmentDirections.ReducePayable,
                    ReasonCodeId = ReasonRoundingId,
                    ReasonCode = "AP-ROUNDING",
                    ReasonDescription = "Rounding Difference Adjustment",
                    PolicyLimitCategory = APPolicyLimitCategories.SmallWriteOff,
                    AdjustmentScope = APAdjustmentScopes.BillLevel,
                    TargetVendorBillId = Bill1Id,
                    TargetVendorBillNumber = "APB-2025-0001",
                    TargetBillOutstandingSnapshot = 8.50m,
                    AdjustmentAmount = 8.50m,
                    APControlAccountIdSnapshot = APControlAccountId,
                    APControlAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.ServiceRevenue),
                    APControlAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.ServiceRevenue),
                    AdjustmentGLAccountId = RoundingExpenseAccountId,
                    AdjustmentGLAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.ServiceRevenue),
                    AdjustmentGLAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.ServiceRevenue),
                    AdjustmentGLAccountType = APAdjustmentGLAccountTypes.Expense,
                    Narration = "Rounding difference on vendor bill APB-2025-0001 after payment",
                    AdjustmentStatus = APAdjustmentStatuses.Posted,
                    SubmittedOn = DateTime.Today.AddDays(-10),
                    SubmittedByUserName = "AP Clerk",
                    PostedOn = DateTime.Today.AddDays(-10),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedByUserName = "Finance Controller",
                    HasAttachments = false,
                    AttachmentCount = 0,
                    EvidenceRequired = false,
                    CreatedAt = DateTime.Today.AddDays(-11),
                    CreatedBy = "AP Clerk"
                },

                // Adjustment 3: Approved - Dispute settlement (awaiting posting)
                new APAdjustmentViewModel
                {
                    APAdjustmentId = Adjustment3Id,
                    TenantId = MasterDataIds.Tenants.Default,
                    CompanyId = AscendingSoftwareCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    VendorId = Vendor3Id,
                    VendorCode = "VND-000003",
                    VendorName = "Reliable Supplies Co",
                    AdjustmentNumber = "APADJ-2025-0003",
                    AdjustmentDate = DateTime.Today.AddDays(-5),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    AdjustmentType = APAdjustmentTypes.DisputeSettlement,
                    AdjustmentDirection = APAdjustmentDirections.ReducePayable,
                    ReasonCodeId = ReasonDisputeId,
                    ReasonCode = "AP-DISPUTE",
                    ReasonDescription = "Dispute Settlement Write-Off",
                    PolicyLimitCategory = APPolicyLimitCategories.Medium,
                    AdjustmentScope = APAdjustmentScopes.BillLevel,
                    TargetVendorBillId = Bill2Id,
                    TargetVendorBillNumber = "APB-2025-0002",
                    TargetBillOutstandingSnapshot = 15000.00m,
                    TargetReferenceText = "DISPUTE-2025-0045",
                    AdjustmentAmount = 5000.00m,
                    AdjustmentGLAccountId = DisputeExpenseAccountId,
                    AdjustmentGLAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.ServiceRevenue),
                    AdjustmentGLAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.ServiceRevenue),
                    AdjustmentGLAccountType = APAdjustmentGLAccountTypes.Expense,
                    Narration = "Vendor agreed to waive ₹5,000 due to delivery delay. Ref: Email dated 15-Jan-2025",
                    AdjustmentStatus = APAdjustmentStatuses.Approved,
                    SubmittedOn = DateTime.Today.AddDays(-5),
                    SubmittedByUserName = "AP Supervisor",
                    ApprovedOn = DateTime.Today.AddDays(-4),
                    ApprovedByUserId = MasterDataIds.PaymentTerms.Net45,
                    ApprovedByUserName = "Finance Manager",
                    HasAttachments = true,
                    AttachmentCount = 2,
                    EvidenceRequired = true,
                    CreatedAt = DateTime.Today.AddDays(-6),
                    CreatedBy = "AP Supervisor"
                },

                // Adjustment 4: Submitted - Pending approval (large write-off)
                new APAdjustmentViewModel
                {
                    APAdjustmentId = Adjustment4Id,
                    TenantId = MasterDataIds.Tenants.Default,
                    CompanyId = AscendingSoftwareCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = BangaloreBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    VendorId = Vendor4Id,
                    VendorCode = "VND-000004",
                    VendorName = "BuildRight Constructions",
                    AdjustmentNumber = "APADJ-2025-0004",
                    AdjustmentDate = DateTime.Today.AddDays(-3),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    AdjustmentType = APAdjustmentTypes.WriteOff,
                    AdjustmentDirection = APAdjustmentDirections.ReducePayable,
                    ReasonCodeId = ReasonSmallBalanceId,
                    ReasonCode = "AP-SML-BAL",
                    ReasonDescription = "Small Balance Write-Off",
                    PolicyLimitCategory = APPolicyLimitCategories.HighRisk,
                    AdjustmentScope = APAdjustmentScopes.VendorLevel,
                    AdjustmentAmount = 25000.00m,
                    AdjustmentGLAccountId = APWriteOffIncomeAccountId,
                    AdjustmentGLAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.ServiceRevenue),
                    AdjustmentGLAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.ServiceRevenue),
                    AdjustmentGLAccountType = APAdjustmentGLAccountTypes.Income,
                    Narration = "Write-off pending vendor account closure - vendor declared bankrupt per court notice dated 10-Jan-2025",
                    AdjustmentStatus = APAdjustmentStatuses.Submitted,
                    SubmittedOn = DateTime.Today.AddDays(-3),
                    SubmittedByUserName = "Branch AP Manager",
                    HasAttachments = true,
                    AttachmentCount = 1,
                    EvidenceRequired = true,
                    CreatedAt = DateTime.Today.AddDays(-4),
                    CreatedBy = "Branch AP Manager"
                },

                // Adjustment 5: Draft - New adjustment being prepared
                new APAdjustmentViewModel
                {
                    APAdjustmentId = Adjustment5Id,
                    TenantId = MasterDataIds.Tenants.Default,
                    CompanyId = AscendingSoftwareCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    VendorId = Vendor5Id,
                    VendorCode = "VND-000005",
                    VendorName = "Tamil Nadu Electricity Board",
                    AdjustmentNumber = "APADJ-2025-0005",
                    AdjustmentDate = DateTime.Today.AddDays(-1),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    AdjustmentType = APAdjustmentTypes.RoundOffCorrection,
                    AdjustmentDirection = APAdjustmentDirections.ReducePayable,
                    ReasonCodeId = ReasonRoundingId,
                    ReasonCode = "AP-ROUNDING",
                    ReasonDescription = "Rounding Difference Adjustment",
                    PolicyLimitCategory = APPolicyLimitCategories.SmallWriteOff,
                    AdjustmentScope = APAdjustmentScopes.VendorLevel,
                    AdjustmentAmount = 3.00m,
                    AdjustmentGLAccountId = RoundingExpenseAccountId,
                    AdjustmentGLAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.ServiceRevenue),
                    AdjustmentGLAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.ServiceRevenue),
                    AdjustmentGLAccountType = APAdjustmentGLAccountTypes.Expense,
                    Narration = "Small rounding adjustment for utility bill reconciliation",
                    AdjustmentStatus = APAdjustmentStatuses.Draft,
                    HasAttachments = false,
                    AttachmentCount = 0,
                    EvidenceRequired = false,
                    CreatedAt = DateTime.Today.AddDays(-1),
                    CreatedBy = "AP Clerk"
                },

                // Adjustment 6: Cancelled - Cancelled before posting
                new APAdjustmentViewModel
                {
                    APAdjustmentId = Adjustment6Id,
                    TenantId = MasterDataIds.Tenants.Default,
                    CompanyId = AscendingSoftwareCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    VendorId = Vendor6Id,
                    VendorCode = "VND-000006",
                    VendorName = "Prime Properties",
                    AdjustmentNumber = "APADJ-2025-0006",
                    AdjustmentDate = DateTime.Today.AddDays(-8),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    AdjustmentType = APAdjustmentTypes.Reclassification,
                    AdjustmentDirection = APAdjustmentDirections.ReducePayable,
                    ReasonCodeId = ReasonReclassId,
                    ReasonCode = "AP-RECLASS",
                    ReasonDescription = "Reclassification to Other Liability",
                    PolicyLimitCategory = APPolicyLimitCategories.Medium,
                    AdjustmentScope = APAdjustmentScopes.BillLevel,
                    TargetVendorBillId = Bill3Id,
                    TargetVendorBillNumber = "APB-2025-0003",
                    TargetBillOutstandingSnapshot = 150000.00m,
                    AdjustmentAmount = 10000.00m,
                    AdjustmentGLAccountId = ReclassificationAccountId,
                    AdjustmentGLAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.ServiceRevenue),
                    AdjustmentGLAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.ServiceRevenue),
                    AdjustmentGLAccountType = APAdjustmentGLAccountTypes.Liability,
                    Narration = "Reclassification cancelled - issue resolved with vendor",
                    AdjustmentStatus = APAdjustmentStatuses.Cancelled,
                    SubmittedOn = DateTime.Today.AddDays(-7),
                    SubmittedByUserName = "AP Supervisor",
                    CancelledOn = DateTime.Today.AddDays(-6),
                    CancelledByUserId = MasterDataIds.Tenants.Default,
                    CancelledByUserName = "Finance Controller",
                    CancellationReason = "Issue resolved directly with vendor - no adjustment required",
                    HasAttachments = false,
                    AttachmentCount = 0,
                    EvidenceRequired = false,
                    CreatedAt = DateTime.Today.AddDays(-9),
                    CreatedBy = "AP Supervisor"
                }
,

                // Adjustment 7: Posted - Vendor balance transfer
                new APAdjustmentViewModel
                {
                    APAdjustmentId = Adjustment7Id,
                    TenantId = MasterDataIds.Tenants.Default,
                    CompanyId = AscendingSoftwareCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    VendorId = Vendor3Id,
                    VendorCode = "VND-000003",
                    VendorName = "Reliable Supplies Co",
                    AdjustmentNumber = "APADJ-2025-0007",
                    AdjustmentDate = DateTime.Today.AddDays(-20),
                    PostingDate = DateTime.Today.AddDays(-20),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    AdjustmentType = APAdjustmentTypes.VendorBalanceTransfer,
                    AdjustmentDirection = APAdjustmentDirections.ReducePayable,
                    ReasonCodeId = ReasonVendorTransferId,
                    ReasonCode = "AP-TRANSFER",
                    ReasonDescription = "Vendor Balance Transfer",
                    AdjustmentScope = APAdjustmentScopes.VendorLevel,
                    AdjustmentAmount = 15000.00m,
                    APControlAccountIdSnapshot = APControlAccountId,
                    APControlAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.ServiceRevenue),
                    APControlAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.ServiceRevenue),
                    AdjustmentGLAccountId = APControlAccountId,
                    AdjustmentGLAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.ServiceRevenue),
                    AdjustmentGLAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.ServiceRevenue),
                    Narration = "Transfer balance to merged vendor account",
                    AdjustmentStatus = APAdjustmentStatuses.Posted,
                    PostedOn = DateTime.Today.AddDays(-20),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedByUserName = "Finance Controller",
                    CreatedAt = DateTime.Today.AddDays(-21),
                    CreatedBy = "AP Supervisor"
                },

                // Adjustment 8: Submitted - Dispute settlement
                new APAdjustmentViewModel
                {
                    APAdjustmentId = Adjustment8Id,
                    TenantId = MasterDataIds.Tenants.Default,
                    CompanyId = AscendingSoftwareCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = BangaloreBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    VendorId = Vendor4Id,
                    VendorCode = "VND-000004",
                    VendorName = "BuildRight Constructions",
                    AdjustmentNumber = "APADJ-2025-0008",
                    AdjustmentDate = DateTime.Today.AddDays(-7),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    AdjustmentType = APAdjustmentTypes.DisputeSettlement,
                    AdjustmentDirection = APAdjustmentDirections.ReducePayable,
                    ReasonCodeId = ReasonDisputeId,
                    ReasonCode = "AP-DISPUTE",
                    ReasonDescription = "Dispute Settlement Write-Off",
                    AdjustmentScope = APAdjustmentScopes.BillLevel,
                    TargetVendorBillId = Bill2Id,
                    TargetVendorBillNumber = "APB-2025-0002",
                    AdjustmentAmount = 12500.00m,
                    APControlAccountIdSnapshot = APControlAccountId,
                    APControlAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.ServiceRevenue),
                    APControlAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.ServiceRevenue),
                    AdjustmentGLAccountId = DisputeExpenseAccountId,
                    AdjustmentGLAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.ServiceRevenue),
                    AdjustmentGLAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.ServiceRevenue),
                    Narration = "Settlement for quality dispute on consulting services",
                    AdjustmentStatus = APAdjustmentStatuses.Submitted,
                    SubmittedOn = DateTime.Today.AddDays(-7),
                    SubmittedByUserName = "AP Supervisor",
                    HasAttachments = true,
                    AttachmentCount = 2,
                    EvidenceRequired = true,
                    CreatedAt = DateTime.Today.AddDays(-8),
                    CreatedBy = "AP Clerk"
                },

                // Adjustment 9: Draft - Write off
                new APAdjustmentViewModel
                {
                    APAdjustmentId = Adjustment9Id,
                    TenantId = MasterDataIds.Tenants.Default,
                    CompanyId = AscendingSoftwareCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    VendorId = Vendor1Id,
                    VendorCode = "VND-000001",
                    VendorName = "Tech Components India Pvt Ltd",
                    AdjustmentNumber = "APADJ-2025-0009",
                    AdjustmentDate = DateTime.Today.AddDays(-2),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    AdjustmentType = APAdjustmentTypes.WriteOff,
                    AdjustmentDirection = APAdjustmentDirections.ReducePayable,
                    ReasonCodeId = ReasonSmallBalanceId,
                    ReasonCode = "AP-SML-BAL",
                    ReasonDescription = "Small Balance Write-Off",
                    AdjustmentScope = APAdjustmentScopes.VendorLevel,
                    AdjustmentAmount = 45.00m,
                    APControlAccountIdSnapshot = APControlAccountId,
                    APControlAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.ServiceRevenue),
                    APControlAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.ServiceRevenue),
                    AdjustmentGLAccountId = APWriteOffIncomeAccountId,
                    AdjustmentGLAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.ServiceRevenue),
                    AdjustmentGLAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.ServiceRevenue),
                    Narration = "Small balance write-off - below threshold",
                    AdjustmentStatus = APAdjustmentStatuses.Draft,
                    CreatedAt = DateTime.Today.AddDays(-2),
                    CreatedBy = "AP Clerk"
                },

                // Adjustment 10: Posted - Reclassification
                new APAdjustmentViewModel
                {
                    APAdjustmentId = Adjustment10Id,
                    TenantId = MasterDataIds.Tenants.Default,
                    CompanyId = AscendingSoftwareCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    VendorId = Vendor5Id,
                    VendorCode = "VND-000005",
                    VendorName = "Tamil Nadu Electricity Board",
                    AdjustmentNumber = "APADJ-2025-0010",
                    AdjustmentDate = DateTime.Today.AddDays(-18),
                    PostingDate = DateTime.Today.AddDays(-18),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    AdjustmentType = APAdjustmentTypes.Reclassification,
                    AdjustmentDirection = APAdjustmentDirections.ReducePayable,
                    ReasonCodeId = ReasonReclassId,
                    ReasonCode = "AP-RECLASS",
                    ReasonDescription = "Reclassification to Other Liability",
                    AdjustmentScope = APAdjustmentScopes.VendorLevel,
                    AdjustmentAmount = 8500.00m,
                    APControlAccountIdSnapshot = APControlAccountId,
                    APControlAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.ServiceRevenue),
                    APControlAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.ServiceRevenue),
                    AdjustmentGLAccountId = ReclassificationAccountId,
                    AdjustmentGLAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.ServiceRevenue),
                    AdjustmentGLAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.ServiceRevenue),
                    Narration = "Reclassify utility deposit to other liabilities",
                    AdjustmentStatus = APAdjustmentStatuses.Posted,
                    PostedOn = DateTime.Today.AddDays(-18),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedByUserName = "Finance Controller",
                    CreatedAt = DateTime.Today.AddDays(-19),
                    CreatedBy = "Finance Manager"
                },

                // Adjustment 11: Pending Approval - Large write off
                new APAdjustmentViewModel
                {
                    APAdjustmentId = Adjustment11Id,
                    TenantId = MasterDataIds.Tenants.Default,
                    CompanyId = AscendingSoftwareCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    VendorId = Vendor6Id,
                    VendorCode = "VND-000006",
                    VendorName = "Prime Properties",
                    AdjustmentNumber = "APADJ-2025-0011",
                    AdjustmentDate = DateTime.Today.AddDays(-3),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    AdjustmentType = APAdjustmentTypes.WriteOff,
                    AdjustmentDirection = APAdjustmentDirections.IncreasePayable,
                    ReasonCodeId = ReasonOtherId,
                    ReasonCode = "AP-OTHER",
                    ReasonDescription = "Other Adjustment Reason",
                    AdjustmentScope = APAdjustmentScopes.VendorLevel,
                    AdjustmentAmount = 35000.00m,
                    APControlAccountIdSnapshot = APControlAccountId,
                    APControlAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.ServiceRevenue),
                    APControlAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.ServiceRevenue),
                    AdjustmentGLAccountId = WriteOffExpenseAccountId,
                    AdjustmentGLAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.ServiceRevenue),
                    AdjustmentGLAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.ServiceRevenue),
                    Narration = "Reinstate previously written off balance",
                    AdjustmentStatus = APAdjustmentStatuses.Submitted,
                    SubmittedOn = DateTime.Today.AddDays(-3),
                    SubmittedByUserName = "AP Supervisor",
                    HasAttachments = true,
                    AttachmentCount = 1,
                    EvidenceRequired = true,
                    CreatedAt = DateTime.Today.AddDays(-4),
                    CreatedBy = "AP Supervisor"
                },

                // Adjustment 12: Cancelled
                new APAdjustmentViewModel
                {
                    APAdjustmentId = Adjustment12Id,
                    TenantId = MasterDataIds.Tenants.Default,
                    CompanyId = AscendingSoftwareCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    VendorId = Vendor2Id,
                    VendorCode = "VND-000002",
                    VendorName = "CloudTech Solutions",
                    AdjustmentNumber = "APADJ-2025-0012",
                    AdjustmentDate = DateTime.Today.AddDays(-12),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    AdjustmentType = APAdjustmentTypes.RoundOffCorrection,
                    AdjustmentDirection = APAdjustmentDirections.ReducePayable,
                    ReasonCodeId = ReasonRoundingId,
                    ReasonCode = "AP-ROUNDING",
                    ReasonDescription = "Rounding Difference Adjustment",
                    AdjustmentScope = APAdjustmentScopes.BillLevel,
                    TargetVendorBillId = Bill1Id,
                    TargetVendorBillNumber = "APB-2025-0001",
                    AdjustmentAmount = 5.50m,
                    APControlAccountIdSnapshot = APControlAccountId,
                    APControlAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.ServiceRevenue),
                    APControlAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.ServiceRevenue),
                    AdjustmentGLAccountId = RoundingExpenseAccountId,
                    AdjustmentGLAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.ServiceRevenue),
                    AdjustmentGLAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.ServiceRevenue),
                    Narration = "Cancelled - Duplicate rounding entry",
                    AdjustmentStatus = APAdjustmentStatuses.Cancelled,
                    CancellationReason = "Duplicate entry - original already processed",
                    CreatedAt = DateTime.Today.AddDays(-13),
                    CreatedBy = "AP Clerk"
                }
            };

            return adjustments;
        }
    }
}
