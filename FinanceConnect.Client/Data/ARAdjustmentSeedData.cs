using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    /// <summary>
    /// Seed data for ARAdjustment model (Model #32)
    /// </summary>
    public static class ARAdjustmentSeedData
    {
        // Company GUIDs (matching MasterDataIds.Companies)
        private static readonly Guid SofaCraftCompanyId = MasterDataIds.Companies.SofaCraft;

        // Branch GUIDs (matching MasterDataIds.Branches)
        private static readonly Guid ChennaiHQBranchId = MasterDataIds.Branches.SofaCraftHQ;
        private static readonly Guid BangaloreBranchId = MasterDataIds.Branches.SofaCraftBengaluru;

        // Customer GUIDs (matching CustomerSeedData)
        private static readonly Guid Customer1Id = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000001");
        private static readonly Guid Customer2Id = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000002");
        private static readonly Guid Customer3Id = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000003");
        private static readonly Guid Customer4Id = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000004");
        private static readonly Guid Customer5Id = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000005");
        private static readonly Guid Customer6Id = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000006");
        private static readonly Guid Customer7Id = Guid.Parse("c5c5c5c5-0001-0001-0001-000000000007");

        // Currency GUIDs (matching MasterDataIds.Currencies)
        private static readonly Guid InrCurrencyId = MasterDataIds.Currencies.INR;

        // GL Account GUIDs (matching COADataService accounts)
        private static readonly Guid ReceivableAccountId = MasterDataIds.Accounts.AccountsReceivable;
        private static readonly Guid WriteOffExpenseAccountId = MasterDataIds.Accounts.RentExpense;
        private static readonly Guid RoundingExpenseAccountId = MasterDataIds.Accounts.UtilitiesExpense;
        private static readonly Guid DiscountAllowedAccountId = MasterDataIds.Accounts.SalariesWages;
        private static readonly Guid DisputeExpenseAccountId = MasterDataIds.Accounts.CostOfMaterials;

        // Invoice GUIDs (matching CustomerInvoiceSeedData)
        private static readonly Guid Invoice1Id = Guid.Parse("a1a1a1a1-0001-0001-0001-000000000001");
        private static readonly Guid Invoice2Id = Guid.Parse("a1a1a1a1-0001-0001-0001-000000000002");
        private static readonly Guid Invoice3Id = Guid.Parse("a1a1a1a1-0001-0001-0001-000000000003");

        // Reason Code GUIDs
        public static readonly Guid ReasonSmallBalanceId = Guid.Parse("e3e3e3e3-0001-0001-0001-000000000001");
        public static readonly Guid ReasonRoundingId = Guid.Parse("e3e3e3e3-0001-0001-0001-000000000002");
        public static readonly Guid ReasonDisputeId = Guid.Parse("e3e3e3e3-0001-0001-0001-000000000003");
        public static readonly Guid ReasonShortPaymentId = Guid.Parse("e3e3e3e3-0001-0001-0001-000000000004");
        public static readonly Guid ReasonBadDebtId = Guid.Parse("e3e3e3e3-0001-0001-0001-000000000005");
        public static readonly Guid ReasonOtherId = Guid.Parse("e3e3e3e3-0001-0001-0001-000000000006");

        // Predefined Adjustment GUIDs
        public static readonly Guid Adjustment1Id = Guid.Parse("e4e4e4e4-0001-0001-0001-000000000001");
        public static readonly Guid Adjustment2Id = Guid.Parse("e4e4e4e4-0001-0001-0001-000000000002");
        public static readonly Guid Adjustment3Id = Guid.Parse("e4e4e4e4-0001-0001-0001-000000000003");
        public static readonly Guid Adjustment4Id = Guid.Parse("e4e4e4e4-0001-0001-0001-000000000004");
        public static readonly Guid Adjustment5Id = Guid.Parse("e4e4e4e4-0001-0001-0001-000000000005");
        public static readonly Guid Adjustment6Id = Guid.Parse("e4e4e4e4-0001-0001-0001-000000000006");
        public static readonly Guid Adjustment7Id = Guid.Parse("e4e4e4e4-0001-0001-0001-000000000007");
        public static readonly Guid Adjustment8Id = Guid.Parse("e4e4e4e4-0001-0001-0001-000000000008");
        public static readonly Guid Adjustment9Id = Guid.Parse("e4e4e4e4-0001-0001-0001-000000000009");
        public static readonly Guid Adjustment10Id = Guid.Parse("e4e4e4e4-0001-0001-0001-000000000010");
        public static readonly Guid Adjustment11Id = Guid.Parse("e4e4e4e4-0001-0001-0001-000000000011");
        public static readonly Guid Adjustment12Id = Guid.Parse("e4e4e4e4-0001-0001-0001-000000000012");

        public static List<ARAdjustmentReasonViewModel> GetSeedReasons()
        {
            return new List<ARAdjustmentReasonViewModel>
            {
                new ARAdjustmentReasonViewModel
                {
                    Id = ReasonSmallBalanceId,
                    CompanyId = SofaCraftCompanyId,
                    ReasonCode = "SML-BAL",
                    ReasonDescription = "Small Balance Write-Off",
                    ApplicableTypes = new[] { AdjustmentTypes.WriteOff },
                    DefaultOffsetAccountId = WriteOffExpenseAccountId,
                    DefaultOffsetAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    DefaultOffsetAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    RequiresApproval = false,
                    RequiresEvidence = false,
                    ApprovalThreshold = 1000,
                    IsActive = true,
                    CreatedAt = DateTime.Today.AddDays(-90),
                    CreatedBy = "System Admin"
                },
                new ARAdjustmentReasonViewModel
                {
                    Id = ReasonRoundingId,
                    CompanyId = SofaCraftCompanyId,
                    ReasonCode = "ROUNDING",
                    ReasonDescription = "Rounding Difference Adjustment",
                    ApplicableTypes = new[] { AdjustmentTypes.Rounding },
                    DefaultOffsetAccountId = RoundingExpenseAccountId,
                    DefaultOffsetAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    DefaultOffsetAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    RequiresApproval = false,
                    RequiresEvidence = false,
                    ApprovalThreshold = 100,
                    IsActive = true,
                    CreatedAt = DateTime.Today.AddDays(-90),
                    CreatedBy = "System Admin"
                },
                new ARAdjustmentReasonViewModel
                {
                    Id = ReasonDisputeId,
                    CompanyId = SofaCraftCompanyId,
                    ReasonCode = "DISPUTE",
                    ReasonDescription = "Dispute Settlement Write-Off",
                    ApplicableTypes = new[] { AdjustmentTypes.DisputeSettlement },
                    DefaultOffsetAccountId = DisputeExpenseAccountId,
                    DefaultOffsetAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    DefaultOffsetAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    RequiresApproval = true,
                    RequiresEvidence = true,
                    ApprovalThreshold = 5000,
                    IsActive = true,
                    CreatedAt = DateTime.Today.AddDays(-90),
                    CreatedBy = "System Admin"
                },
                new ARAdjustmentReasonViewModel
                {
                    Id = ReasonShortPaymentId,
                    CompanyId = SofaCraftCompanyId,
                    ReasonCode = "SHORT-PAY",
                    ReasonDescription = "Short Payment Settlement",
                    ApplicableTypes = new[] { AdjustmentTypes.ShortPaymentSettlement },
                    DefaultOffsetAccountId = DiscountAllowedAccountId,
                    DefaultOffsetAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    DefaultOffsetAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    RequiresApproval = false,
                    RequiresEvidence = false,
                    ApprovalThreshold = 2000,
                    IsActive = true,
                    CreatedAt = DateTime.Today.AddDays(-90),
                    CreatedBy = "System Admin"
                },
                new ARAdjustmentReasonViewModel
                {
                    Id = ReasonBadDebtId,
                    CompanyId = SofaCraftCompanyId,
                    ReasonCode = "BAD-DEBT",
                    ReasonDescription = "Bad Debt Write-Off",
                    ApplicableTypes = new[] { AdjustmentTypes.WriteOff, AdjustmentTypes.BadDebtProvision },
                    DefaultOffsetAccountId = WriteOffExpenseAccountId,
                    DefaultOffsetAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    DefaultOffsetAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    RequiresApproval = true,
                    RequiresEvidence = true,
                    ApprovalThreshold = 10000,
                    IsActive = true,
                    CreatedAt = DateTime.Today.AddDays(-90),
                    CreatedBy = "System Admin"
                },
                new ARAdjustmentReasonViewModel
                {
                    Id = ReasonOtherId,
                    CompanyId = SofaCraftCompanyId,
                    ReasonCode = "OTHER",
                    ReasonDescription = "Other Adjustment Reason",
                    ApplicableTypes = AdjustmentTypes.All,
                    DefaultOffsetAccountId = WriteOffExpenseAccountId,
                    DefaultOffsetAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    DefaultOffsetAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    RequiresApproval = true,
                    RequiresEvidence = true,
                    ApprovalThreshold = 1000,
                    IsActive = true,
                    CreatedAt = DateTime.Today.AddDays(-90),
                    CreatedBy = "System Admin"
                }
            };
        }

        public static List<ARAdjustmentViewModel> GetSeedAdjustments()
        {
            var adjustments = new List<ARAdjustmentViewModel>
            {
                // Adjustment 1: Posted - Small rounding write-off against invoice
                new ARAdjustmentViewModel
                {
                    Id = Adjustment1Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    CustomerId = Customer1Id,
                    CustomerCode = "CUST-001",
                    CustomerName = "ABC Traders Private Limited",
                    AdjustmentNumber = "ADJ-2024-0001",
                    AdjustmentDate = DateTime.Today.AddDays(-25),
                    PostingDate = DateTime.Today.AddDays(-25),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    AdjustmentNarration = "Rounding adjustment for ₹12 outstanding balance",
                    AdjustmentType = AdjustmentTypes.Rounding,
                    ReasonCodeId = ReasonRoundingId,
                    ReasonCode = "ROUNDING",
                    ReasonDescription = "Rounding Difference Adjustment",
                    RequiresApproval = false,
                    ApprovalStatus = ApprovalStatuses.NotRequired,
                    TotalAdjustmentAmount = 12.00m,
                    AdjustmentDirection = AdjustmentDirections.ReduceAR,
                    ReceivableAccountIdSnapshot = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    DefaultWriteOffAccountId = RoundingExpenseAccountId,
                    DefaultWriteOffAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    DefaultWriteOffAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    IsSystemGenerated = false,
                    AdjustmentStatus = AdjustmentStatuses.Posted,
                    PostedOn = DateTime.Today.AddDays(-25),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedByUserName = "Finance Controller",
                    EvidenceRequired = false,
                    EvidenceAttachmentCount = 0,
                    CreatedAt = DateTime.Today.AddDays(-26),
                    CreatedBy = "AR Supervisor"
                },

                // Adjustment 2: Posted - Dispute settlement with approval
                new ARAdjustmentViewModel
                {
                    Id = Adjustment2Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    CustomerId = Customer2Id,
                    CustomerCode = "CUST-002",
                    CustomerName = "State Government Education Dept",
                    AdjustmentNumber = "ADJ-2024-0002",
                    AdjustmentDate = DateTime.Today.AddDays(-20),
                    PostingDate = DateTime.Today.AddDays(-18),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    AdjustmentNarration = "Dispute settlement - Customer claimed quality issues with delivered goods. Agreed to write off ₹5,000.",
                    AdjustmentType = AdjustmentTypes.DisputeSettlement,
                    ReasonCodeId = ReasonDisputeId,
                    ReasonCode = "DISPUTE",
                    ReasonDescription = "Dispute Settlement Write-Off",
                    RequiresApproval = true,
                    ApprovalStatus = ApprovalStatuses.Approved,
                    ApprovedByUserId = MasterDataIds.Tenants.Default,
                    ApprovedByUserName = "Finance Controller",
                    ApprovedOn = DateTime.Today.AddDays(-19),
                    ApprovalComment = "Approved based on customer complaint documentation",
                    TotalAdjustmentAmount = 5000.00m,
                    AdjustmentDirection = AdjustmentDirections.ReduceAR,
                    ReceivableAccountIdSnapshot = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    DefaultWriteOffAccountId = DisputeExpenseAccountId,
                    DefaultWriteOffAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    DefaultWriteOffAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    IsSystemGenerated = false,
                    AdjustmentStatus = AdjustmentStatuses.Posted,
                    PostedOn = DateTime.Today.AddDays(-18),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedByUserName = "Finance Controller",
                    EvidenceRequired = true,
                    EvidenceAttachmentCount = 2,
                    CreatedAt = DateTime.Today.AddDays(-21),
                    CreatedBy = "AR Supervisor"
                },

                // Adjustment 3: Draft - Pending write-off
                new ARAdjustmentViewModel
                {
                    Id = Adjustment3Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = BangaloreBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    CustomerId = Customer3Id,
                    CustomerCode = "CUST-003",
                    CustomerName = "XYZ Electronics Hub",
                    AdjustmentNumber = "ADJ-2024-0003",
                    AdjustmentDate = DateTime.Today.AddDays(-5),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    AdjustmentNarration = "Small balance write-off - customer account showing ₹150 pending",
                    AdjustmentType = AdjustmentTypes.WriteOff,
                    ReasonCodeId = ReasonSmallBalanceId,
                    ReasonCode = "SML-BAL",
                    ReasonDescription = "Small Balance Write-Off",
                    RequiresApproval = false,
                    ApprovalStatus = ApprovalStatuses.NotRequired,
                    TotalAdjustmentAmount = 150.00m,
                    AdjustmentDirection = AdjustmentDirections.ReduceAR,
                    DefaultWriteOffAccountId = WriteOffExpenseAccountId,
                    DefaultWriteOffAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    DefaultWriteOffAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    IsSystemGenerated = false,
                    AdjustmentStatus = AdjustmentStatuses.Draft,
                    EvidenceRequired = false,
                    EvidenceAttachmentCount = 0,
                    CreatedAt = DateTime.Today.AddDays(-5),
                    CreatedBy = "AR Clerk"
                },

                // Adjustment 4: Submitted - Pending approval for bad debt
                new ARAdjustmentViewModel
                {
                    Id = Adjustment4Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    CustomerId = Customer1Id,
                    CustomerCode = "CUST-001",
                    CustomerName = "ABC Traders Private Limited",
                    AdjustmentNumber = "ADJ-2024-0004",
                    AdjustmentDate = DateTime.Today.AddDays(-3),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    AdjustmentNarration = "Bad debt write-off - Invoice aged 180+ days, multiple collection attempts failed",
                    AdjustmentType = AdjustmentTypes.WriteOff,
                    ReasonCodeId = ReasonBadDebtId,
                    ReasonCode = "BAD-DEBT",
                    ReasonDescription = "Bad Debt Write-Off",
                    RequiresApproval = true,
                    ApprovalStatus = ApprovalStatuses.Pending,
                    TotalAdjustmentAmount = 25000.00m,
                    AdjustmentDirection = AdjustmentDirections.ReduceAR,
                    DefaultWriteOffAccountId = WriteOffExpenseAccountId,
                    DefaultWriteOffAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    DefaultWriteOffAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    IsSystemGenerated = false,
                    AdjustmentStatus = AdjustmentStatuses.Submitted,
                    EvidenceRequired = true,
                    EvidenceAttachmentCount = 3,
                    CreatedAt = DateTime.Today.AddDays(-4),
                    CreatedBy = "AR Supervisor"
                },

                // Adjustment 5: Cancelled
                new ARAdjustmentViewModel
                {
                    Id = Adjustment5Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    CustomerId = Customer2Id,
                    CustomerCode = "CUST-002",
                    CustomerName = "State Government Education Dept",
                    AdjustmentNumber = "ADJ-2024-0005",
                    AdjustmentDate = DateTime.Today.AddDays(-15),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    AdjustmentNarration = "Short payment settlement - Cancelled as customer made full payment",
                    AdjustmentType = AdjustmentTypes.ShortPaymentSettlement,
                    ReasonCodeId = ReasonShortPaymentId,
                    ReasonCode = "SHORT-PAY",
                    ReasonDescription = "Short Payment Settlement",
                    RequiresApproval = false,
                    ApprovalStatus = ApprovalStatuses.NotRequired,
                    TotalAdjustmentAmount = 800.00m,
                    AdjustmentDirection = AdjustmentDirections.ReduceAR,
                    DefaultWriteOffAccountId = DiscountAllowedAccountId,
                    DefaultWriteOffAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    DefaultWriteOffAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    IsSystemGenerated = false,
                    AdjustmentStatus = AdjustmentStatuses.Cancelled,
                    CancelledOn = DateTime.Today.AddDays(-14),
                    CancelledByUserId = MasterDataIds.Tenants.Default,
                    CancelledByUserName = "AR Supervisor",
                    CancellationReason = "Customer made full payment, adjustment no longer required",
                    EvidenceRequired = false,
                    EvidenceAttachmentCount = 0,
                    CreatedAt = DateTime.Today.AddDays(-16),
                    CreatedBy = "AR Clerk"
                },

                // Adjustment 6: Posted - Short payment settlement
                new ARAdjustmentViewModel
                {
                    Id = Adjustment6Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = BangaloreBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    CustomerId = Customer3Id,
                    CustomerCode = "CUST-003",
                    CustomerName = "XYZ Electronics Hub",
                    AdjustmentNumber = "ADJ-2024-0006",
                    AdjustmentDate = DateTime.Today.AddDays(-10),
                    PostingDate = DateTime.Today.AddDays(-10),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    AdjustmentNarration = "Short payment settlement - Customer paid ₹99,500 against ₹100,000 invoice",
                    AdjustmentType = AdjustmentTypes.ShortPaymentSettlement,
                    ReasonCodeId = ReasonShortPaymentId,
                    ReasonCode = "SHORT-PAY",
                    ReasonDescription = "Short Payment Settlement",
                    RequiresApproval = false,
                    ApprovalStatus = ApprovalStatuses.NotRequired,
                    TotalAdjustmentAmount = 500.00m,
                    AdjustmentDirection = AdjustmentDirections.ReduceAR,
                    ReceivableAccountIdSnapshot = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    DefaultWriteOffAccountId = DiscountAllowedAccountId,
                    DefaultWriteOffAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    DefaultWriteOffAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    IsSystemGenerated = false,
                    AdjustmentStatus = AdjustmentStatuses.Posted,
                    PostedOn = DateTime.Today.AddDays(-10),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedByUserName = "Finance Controller",
                    EvidenceRequired = false,
                    EvidenceAttachmentCount = 0,
                    CreatedAt = DateTime.Today.AddDays(-11),
                    CreatedBy = "AR Supervisor"
                },

                // Adjustment 7: Posted - Balance transfer
                new ARAdjustmentViewModel
                {
                    Id = Adjustment7Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    CustomerId = Customer4Id,
                    CustomerCode = "CUST-004",
                    CustomerName = "Rajesh Kumar",
                    AdjustmentNumber = "ADJ-2024-0007",
                    AdjustmentDate = DateTime.Today.AddDays(-22),
                    PostingDate = DateTime.Today.AddDays(-22),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    AdjustmentNarration = "Balance transfer between customer accounts per restructuring",
                    AdjustmentType = AdjustmentTypes.ShortPaymentSettlement,
                    ReasonCodeId = ReasonOtherId,
                    ReasonCode = "OTHER",
                    ReasonDescription = "Other - Balance Transfer",
                    RequiresApproval = true,
                    ApprovalStatus = ApprovalStatuses.Approved,
                    TotalAdjustmentAmount = 25000.00m,
                    AdjustmentDirection = AdjustmentDirections.ReduceAR,
                    ReceivableAccountIdSnapshot = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    DefaultWriteOffAccountId = WriteOffExpenseAccountId,
                    DefaultWriteOffAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    DefaultWriteOffAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    IsSystemGenerated = false,
                    AdjustmentStatus = AdjustmentStatuses.Posted,
                    PostedOn = DateTime.Today.AddDays(-22),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedByUserName = "Finance Controller",
                    EvidenceRequired = true,
                    EvidenceAttachmentCount = 1,
                    CreatedAt = DateTime.Today.AddDays(-23),
                    CreatedBy = "AR Manager"
                },

                // Adjustment 8: Submitted - Dispute settlement
                new ARAdjustmentViewModel
                {
                    Id = Adjustment8Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = BangaloreBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    CustomerId = Customer5Id,
                    CustomerCode = "CUST-005",
                    CustomerName = "TechPark SEZ Solutions Pvt Ltd",
                    AdjustmentNumber = "ADJ-2024-0008",
                    AdjustmentDate = DateTime.Today.AddDays(-7),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    AdjustmentNarration = "Dispute settlement - Service quality issue reported by customer",
                    AdjustmentType = AdjustmentTypes.DisputeSettlement,
                    ReasonCodeId = ReasonDisputeId,
                    ReasonCode = "DISPUTE",
                    ReasonDescription = "Dispute Settlement",
                    RequiresApproval = true,
                    ApprovalStatus = ApprovalStatuses.Approved,
                    TotalAdjustmentAmount = 18500.00m,
                    AdjustmentDirection = AdjustmentDirections.ReduceAR,
                    DefaultWriteOffAccountId = DisputeExpenseAccountId,
                    DefaultWriteOffAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    DefaultWriteOffAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    IsSystemGenerated = false,
                    AdjustmentStatus = AdjustmentStatuses.Submitted,
                    EvidenceRequired = true,
                    EvidenceAttachmentCount = 2,
                    CreatedAt = DateTime.Today.AddDays(-8),
                    CreatedBy = "AR Supervisor"
                },

                // Adjustment 9: Draft - Bad debt write-off
                new ARAdjustmentViewModel
                {
                    Id = Adjustment9Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    CustomerId = Customer6Id,
                    CustomerCode = "CUST-006",
                    CustomerName = "Discontinued Enterprises",
                    AdjustmentNumber = "ADJ-2024-0009",
                    AdjustmentDate = DateTime.Today.AddDays(-3),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    AdjustmentNarration = "Bad debt write-off - Customer unresponsive for 180+ days",
                    AdjustmentType = AdjustmentTypes.WriteOff,
                    ReasonCodeId = ReasonBadDebtId,
                    ReasonCode = "BAD-DEBT",
                    ReasonDescription = "Bad Debt Write-Off",
                    RequiresApproval = true,
                    ApprovalStatus = ApprovalStatuses.NotRequired,
                    TotalAdjustmentAmount = 75000.00m,
                    AdjustmentDirection = AdjustmentDirections.ReduceAR,
                    DefaultWriteOffAccountId = WriteOffExpenseAccountId,
                    DefaultWriteOffAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    DefaultWriteOffAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    IsSystemGenerated = false,
                    AdjustmentStatus = AdjustmentStatuses.Draft,
                    EvidenceRequired = true,
                    EvidenceAttachmentCount = 0,
                    CreatedAt = DateTime.Today.AddDays(-3),
                    CreatedBy = "AR Clerk"
                },

                // Adjustment 10: Posted - Rounding adjustment
                new ARAdjustmentViewModel
                {
                    Id = Adjustment10Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = BangaloreBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    CustomerId = Customer7Id,
                    CustomerCode = "CUST-007",
                    CustomerName = "Silicon Valley Partners LLC",
                    AdjustmentNumber = "ADJ-2024-0010",
                    AdjustmentDate = DateTime.Today.AddDays(-14),
                    PostingDate = DateTime.Today.AddDays(-14),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    AdjustmentNarration = "Rounding difference on foreign currency invoice conversion",
                    AdjustmentType = AdjustmentTypes.Rounding,
                    ReasonCodeId = ReasonRoundingId,
                    ReasonCode = "ROUNDING",
                    ReasonDescription = "Rounding Difference",
                    RequiresApproval = false,
                    ApprovalStatus = ApprovalStatuses.NotRequired,
                    TotalAdjustmentAmount = 2.50m,
                    AdjustmentDirection = AdjustmentDirections.ReduceAR,
                    ReceivableAccountIdSnapshot = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    DefaultWriteOffAccountId = RoundingExpenseAccountId,
                    DefaultWriteOffAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    DefaultWriteOffAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    IsSystemGenerated = true,
                    AdjustmentStatus = AdjustmentStatuses.Posted,
                    PostedOn = DateTime.Today.AddDays(-14),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedByUserName = "System",
                    EvidenceRequired = false,
                    EvidenceAttachmentCount = 0,
                    CreatedAt = DateTime.Today.AddDays(-14),
                    CreatedBy = "System"
                },

                // Adjustment 11: Posted - Small balance write-off
                new ARAdjustmentViewModel
                {
                    Id = Adjustment11Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    CustomerId = Customer1Id,
                    CustomerCode = "CUST-001",
                    CustomerName = "ABC Traders Private Limited",
                    AdjustmentNumber = "ADJ-2024-0011",
                    AdjustmentDate = DateTime.Today.AddDays(-28),
                    PostingDate = DateTime.Today.AddDays(-28),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    AdjustmentNarration = "Small balance write-off below threshold of ₹100",
                    AdjustmentType = AdjustmentTypes.WriteOff,
                    ReasonCodeId = ReasonSmallBalanceId,
                    ReasonCode = "SMALL-BAL",
                    ReasonDescription = "Small Balance Write-Off",
                    RequiresApproval = false,
                    ApprovalStatus = ApprovalStatuses.NotRequired,
                    TotalAdjustmentAmount = 45.00m,
                    AdjustmentDirection = AdjustmentDirections.ReduceAR,
                    ReceivableAccountIdSnapshot = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    DefaultWriteOffAccountId = WriteOffExpenseAccountId,
                    DefaultWriteOffAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    DefaultWriteOffAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    IsSystemGenerated = true,
                    AdjustmentStatus = AdjustmentStatuses.Posted,
                    PostedOn = DateTime.Today.AddDays(-28),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedByUserName = "System",
                    EvidenceRequired = false,
                    EvidenceAttachmentCount = 0,
                    CreatedAt = DateTime.Today.AddDays(-28),
                    CreatedBy = "System"
                },

                // Adjustment 12: Cancelled - Dispute settlement
                new ARAdjustmentViewModel
                {
                    Id = Adjustment12Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = BangaloreBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    CustomerId = Customer2Id,
                    CustomerCode = "CUST-002",
                    CustomerName = "State Government Education Dept",
                    AdjustmentNumber = "ADJ-2024-0012",
                    AdjustmentDate = DateTime.Today.AddDays(-20),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    AdjustmentNarration = "Dispute settlement cancelled - Resolved via credit note instead",
                    AdjustmentType = AdjustmentTypes.DisputeSettlement,
                    ReasonCodeId = ReasonDisputeId,
                    ReasonCode = "DISPUTE",
                    ReasonDescription = "Dispute Settlement",
                    RequiresApproval = true,
                    ApprovalStatus = ApprovalStatuses.NotRequired,
                    TotalAdjustmentAmount = 32000.00m,
                    AdjustmentDirection = AdjustmentDirections.ReduceAR,
                    DefaultWriteOffAccountId = DisputeExpenseAccountId,
                    DefaultWriteOffAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    DefaultWriteOffAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    IsSystemGenerated = false,
                    AdjustmentStatus = AdjustmentStatuses.Cancelled,
                    CancelledOn = DateTime.Today.AddDays(-19),
                    CancelledByUserId = MasterDataIds.Tenants.Default,
                    CancelledByUserName = "Finance Controller",
                    CancellationReason = "Resolved through credit note CN-2024-0010 instead",
                    EvidenceRequired = false,
                    EvidenceAttachmentCount = 0,
                    CreatedAt = DateTime.Today.AddDays(-21),
                    CreatedBy = "AR Manager"
                }
            };

            // Add lines to each adjustment
            AddLinesToAdjustment1(adjustments[0]);
            AddLinesToAdjustment2(adjustments[1]);
            AddLinesToAdjustment3(adjustments[2]);
            AddLinesToAdjustment4(adjustments[3]);
            AddLinesToAdjustment5(adjustments[4]);
            AddLinesToAdjustment6(adjustments[5]);

            return adjustments;
        }

        private static void AddLinesToAdjustment1(ARAdjustmentViewModel adjustment)
        {
            adjustment.Lines = new List<ARAdjustmentLineViewModel>
            {
                new ARAdjustmentLineViewModel
                {
                    Id = Guid.NewGuid(),
                    ARAdjustmentId = adjustment.Id,
                    LineNumber = 10,
                    CustomerInvoiceId = Invoice1Id,
                    CustomerInvoiceNumber = "INV-2024-0001",
                    InvoiceOutstanding = 12.00m,
                    LineType = AdjustmentLineTypes.Rounding,
                    AdjustmentAmount = 12.00m,
                    OffsetAccountId = RoundingExpenseAccountId,
                    OffsetAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    OffsetAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    LineNarration = "Rounding difference on final payment",
                    CreatedAt = adjustment.CreatedAt,
                    CreatedBy = adjustment.CreatedBy
                }
            };
        }

        private static void AddLinesToAdjustment2(ARAdjustmentViewModel adjustment)
        {
            adjustment.Lines = new List<ARAdjustmentLineViewModel>
            {
                new ARAdjustmentLineViewModel
                {
                    Id = Guid.NewGuid(),
                    ARAdjustmentId = adjustment.Id,
                    LineNumber = 10,
                    CustomerInvoiceId = Invoice2Id,
                    CustomerInvoiceNumber = "INV-2024-0002",
                    InvoiceOutstanding = 15000.00m,
                    LineType = AdjustmentLineTypes.Dispute,
                    AdjustmentAmount = 5000.00m,
                    OffsetAccountId = DisputeExpenseAccountId,
                    OffsetAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    OffsetAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    LineNarration = "Partial write-off due to quality dispute",
                    CreatedAt = adjustment.CreatedAt,
                    CreatedBy = adjustment.CreatedBy
                }
            };
        }

        private static void AddLinesToAdjustment3(ARAdjustmentViewModel adjustment)
        {
            adjustment.Lines = new List<ARAdjustmentLineViewModel>
            {
                new ARAdjustmentLineViewModel
                {
                    Id = Guid.NewGuid(),
                    ARAdjustmentId = adjustment.Id,
                    LineNumber = 10,
                    CustomerInvoiceId = null,
                    LineType = AdjustmentLineTypes.WriteOff,
                    AdjustmentAmount = 150.00m,
                    OffsetAccountId = WriteOffExpenseAccountId,
                    OffsetAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    OffsetAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    LineNarration = "Small balance write-off - customer level",
                    CreatedAt = adjustment.CreatedAt,
                    CreatedBy = adjustment.CreatedBy
                }
            };
        }

        private static void AddLinesToAdjustment4(ARAdjustmentViewModel adjustment)
        {
            adjustment.Lines = new List<ARAdjustmentLineViewModel>
            {
                new ARAdjustmentLineViewModel
                {
                    Id = Guid.NewGuid(),
                    ARAdjustmentId = adjustment.Id,
                    LineNumber = 10,
                    CustomerInvoiceId = Invoice1Id,
                    CustomerInvoiceNumber = "INV-2024-0001",
                    InvoiceOutstanding = 25000.00m,
                    LineType = AdjustmentLineTypes.WriteOff,
                    AdjustmentAmount = 25000.00m,
                    OffsetAccountId = WriteOffExpenseAccountId,
                    OffsetAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    OffsetAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    LineNarration = "Bad debt write-off - aged receivable",
                    CreatedAt = adjustment.CreatedAt,
                    CreatedBy = adjustment.CreatedBy
                }
            };
        }

        private static void AddLinesToAdjustment5(ARAdjustmentViewModel adjustment)
        {
            adjustment.Lines = new List<ARAdjustmentLineViewModel>
            {
                new ARAdjustmentLineViewModel
                {
                    Id = Guid.NewGuid(),
                    ARAdjustmentId = adjustment.Id,
                    LineNumber = 10,
                    CustomerInvoiceId = Invoice2Id,
                    CustomerInvoiceNumber = "INV-2024-0002",
                    InvoiceOutstanding = 800.00m,
                    LineType = AdjustmentLineTypes.DiscountAllowed,
                    AdjustmentAmount = 800.00m,
                    OffsetAccountId = DiscountAllowedAccountId,
                    OffsetAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    OffsetAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    LineNarration = "Short payment discount",
                    CreatedAt = adjustment.CreatedAt,
                    CreatedBy = adjustment.CreatedBy
                }
            };
        }

        private static void AddLinesToAdjustment6(ARAdjustmentViewModel adjustment)
        {
            adjustment.Lines = new List<ARAdjustmentLineViewModel>
            {
                new ARAdjustmentLineViewModel
                {
                    Id = Guid.NewGuid(),
                    ARAdjustmentId = adjustment.Id,
                    LineNumber = 10,
                    CustomerInvoiceId = Invoice3Id,
                    CustomerInvoiceNumber = "INV-2024-0003",
                    InvoiceOutstanding = 500.00m,
                    LineType = AdjustmentLineTypes.DiscountAllowed,
                    AdjustmentAmount = 500.00m,
                    OffsetAccountId = DiscountAllowedAccountId,
                    OffsetAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    OffsetAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    LineNarration = "Short payment settlement - ₹500 under-payment",
                    CreatedAt = adjustment.CreatedAt,
                    CreatedBy = adjustment.CreatedBy
                }
            };
        }
    }
}
