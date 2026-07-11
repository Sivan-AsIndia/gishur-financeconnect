using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    /// <summary>
    /// Seed data for VendorCreditNote model (Model #39)
    /// </summary>
    public static class VendorCreditNoteSeedData
    {
        // Company GUIDs (matching existing company seed data)
        private static readonly Guid AscendingSoftwareCompanyId = MasterDataIds.Companies.SofaCraft;

        // Branch GUIDs (matching existing branch seed data)
        private static readonly Guid ChennaiHQBranchId = MasterDataIds.Branches.SofaCraftHQ;
        private static readonly Guid BangaloreBranchId = MasterDataIds.Branches.SofaCraftBengaluru;

        // Vendor GUIDs (matching VendorSeedData)
        private static readonly Guid Vendor1Id = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000001");
        private static readonly Guid Vendor2Id = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000002");
        private static readonly Guid Vendor3Id = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000003");
        private static readonly Guid Vendor4Id = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000004");
        private static readonly Guid Vendor5Id = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000005");
        private static readonly Guid Vendor6Id = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000006");

        // Currency GUIDs
        private static readonly Guid InrCurrencyId = MasterDataIds.Currencies.INR;

        // GL Account GUIDs
        private static readonly Guid PayableAccountId = MasterDataIds.PaymentTerms.Net45;
        private static readonly Guid PurchaseReturnAccountId = MasterDataIds.Accounts.CostOfMaterials;
        private static readonly Guid ExpenseAccountId = Guid.Parse("a0000012-0012-0012-0012-000000000122");
        private static readonly Guid TaxAccountId = Guid.Parse("a0000006-0006-0006-0006-000000000060");

        // Bill GUIDs (matching VendorBillSeedData)
        private static readonly Guid Bill1Id = Guid.Parse("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e001");
        private static readonly Guid Bill2Id = Guid.Parse("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e002");
        private static readonly Guid Bill3Id = Guid.Parse("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e003");

        // Predefined Credit Note GUIDs
        public static readonly Guid VendorCreditNote1Id = Guid.Parse("e3e3e3e3-0001-0001-0001-000000000001");
        public static readonly Guid VendorCreditNote2Id = Guid.Parse("e3e3e3e3-0001-0001-0001-000000000002");
        public static readonly Guid VendorCreditNote3Id = Guid.Parse("e3e3e3e3-0001-0001-0001-000000000003");
        public static readonly Guid VendorCreditNote4Id = Guid.Parse("e3e3e3e3-0001-0001-0001-000000000004");
        public static readonly Guid VendorCreditNote5Id = Guid.Parse("e3e3e3e3-0001-0001-0001-000000000005");

        public static readonly Guid VendorCreditNote6Id = Guid.Parse("e3e3e3e3-0001-0001-0001-000000000006");
        public static readonly Guid VendorCreditNote7Id = Guid.Parse("e3e3e3e3-0001-0001-0001-000000000007");
        public static readonly Guid VendorCreditNote8Id = Guid.Parse("e3e3e3e3-0001-0001-0001-000000000008");
        public static readonly Guid VendorCreditNote9Id = Guid.Parse("e3e3e3e3-0001-0001-0001-000000000009");
        public static readonly Guid VendorCreditNote10Id = Guid.Parse("e3e3e3e3-0001-0001-0001-000000000010");
        public static readonly Guid VendorCreditNote11Id = Guid.Parse("e3e3e3e3-0001-0001-0001-000000000011");
        public static readonly Guid VendorCreditNote12Id = Guid.Parse("e3e3e3e3-0001-0001-0001-000000000012");

        public static List<VendorCreditNoteViewModel> GetSeedCreditNotes()
        {
            var creditNotes = new List<VendorCreditNoteViewModel>
            {
                // Credit Note 1: Posted credit note - Purchase Return against Bill
                new VendorCreditNoteViewModel
                {
                    Id = VendorCreditNote1Id,
                    CompanyId = AscendingSoftwareCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    VendorId = Vendor1Id,
                    VendorCode = "VND-000001",
                    VendorName = "Tech Components India Pvt Ltd",
                    CreditNoteNumber = "APCN-2024-0001",
                    VendorCreditNoteReferenceNumber = "VCN-TS-2024-001",
                    VendorCreditNoteDate = DateTime.Today.AddDays(-30),
                    CreditEntryDate = DateTime.Today.AddDays(-30),
                    PostingDate = DateTime.Today.AddDays(-30),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    CreditNoteNarration = "Purchase return for defective computer parts received",
                    CreditNoteType = VendorCreditNoteTypes.PurchaseReturn,
                    IsAgainstBill = true,
                    PrimaryVendorBillId = Bill1Id,
                    PrimaryVendorBillNumber = "BILL-2024-0001",
                    BillNumberSnapshot = "BILL-2024-0001",
                    BillDateSnapshot = DateTime.Today.AddDays(-45),
                    SubTotalCreditAmount = 25000.00m,
                    TaxCreditAmount = 4500.00m,
                    RoundOffAmount = 0.00m,
                    TotalCreditAmount = 29500.00m,
                    AppliedAmount = 29500.00m,
                    IsGSTApplicable = true,
                    VendorGSTINSnapshot = "33AABCT1234Z1ZP",
                    CreditNoteStatus = VendorCreditNoteStatuses.Posted,
                    PostedOn = DateTime.Today.AddDays(-30),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedByUserName = "Finance Controller",
                    CreatedAt = DateTime.Today.AddDays(-31),
                    CreatedBy = "AP Clerk",
                    Lines = new List<VendorCreditNoteLineModel>
                    {
                        new VendorCreditNoteLineModel
                        {
                            Id = Guid.NewGuid(),
                            VendorCreditNoteId = VendorCreditNote1Id,
                            LineNumber = 10,
                            LineType = VendorCreditNoteLineTypes.Item,
                            Description = "Defective Computer RAM Modules - Return",
                            Quantity = 10,
                            UnitPrice = 2500.00m,
                            DiscountPercent = 0,
                            DiscountAmount = 0,
                            TaxRatePercent = 18,
                            TaxAmount = 4500.00m,
                            ReversalAccountId = PurchaseReturnAccountId,
                            ReversalAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                            ReversalAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                            HSNCode = "84733010",
                            CreatedAt = DateTime.Today.AddDays(-31),
                            CreatedBy = "AP Clerk"
                        }
                    }
                },

                // Credit Note 2: Posted credit note - Price Reduction
                new VendorCreditNoteViewModel
                {
                    Id = VendorCreditNote2Id,
                    CompanyId = AscendingSoftwareCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    VendorId = Vendor1Id,
                    VendorCode = "VND-000001",
                    VendorName = "Tech Components India Pvt Ltd",
                    CreditNoteNumber = "APCN-2024-0002",
                    VendorCreditNoteReferenceNumber = "VCN-TS-2024-002",
                    VendorCreditNoteDate = DateTime.Today.AddDays(-20),
                    CreditEntryDate = DateTime.Today.AddDays(-20),
                    PostingDate = DateTime.Today.AddDays(-20),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    CreditNoteNarration = "Price reduction as per renegotiated contract terms",
                    CreditNoteType = VendorCreditNoteTypes.PriceReduction,
                    IsAgainstBill = true,
                    PrimaryVendorBillId = Bill2Id,
                    PrimaryVendorBillNumber = "BILL-2024-0002",
                    BillNumberSnapshot = "BILL-2024-0002",
                    BillDateSnapshot = DateTime.Today.AddDays(-30),
                    SubTotalCreditAmount = 15000.00m,
                    TaxCreditAmount = 2700.00m,
                    RoundOffAmount = 0.00m,
                    TotalCreditAmount = 17700.00m,
                    AppliedAmount = 17700.00m,
                    IsGSTApplicable = true,
                    VendorGSTINSnapshot = "33AABCT1234Z1ZP",
                    CreditNoteStatus = VendorCreditNoteStatuses.Posted,
                    PostedOn = DateTime.Today.AddDays(-20),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedByUserName = "Finance Controller",
                    CreatedAt = DateTime.Today.AddDays(-21),
                    CreatedBy = "AP Clerk",
                    Lines = new List<VendorCreditNoteLineModel>
                    {
                        new VendorCreditNoteLineModel
                        {
                            Id = Guid.NewGuid(),
                            VendorCreditNoteId = VendorCreditNote2Id,
                            LineNumber = 10,
                            LineType = VendorCreditNoteLineTypes.Manual,
                            Description = "Price adjustment - Contract rate revision",
                            Quantity = 1,
                            UnitPrice = 15000.00m,
                            DiscountPercent = 0,
                            DiscountAmount = 0,
                            TaxRatePercent = 18,
                            TaxAmount = 2700.00m,
                            ReversalAccountId = PurchaseReturnAccountId,
                            ReversalAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                            ReversalAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                            CreatedAt = DateTime.Today.AddDays(-21),
                            CreatedBy = "AP Clerk"
                        }
                    }
                },

                // Credit Note 3: Draft credit note - Discount Rebate
                new VendorCreditNoteViewModel
                {
                    Id = VendorCreditNote3Id,
                    CompanyId = AscendingSoftwareCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    VendorId = Vendor2Id,
                    VendorCode = "VND-000002",
                    VendorName = "CloudTech Solutions",
                    CreditNoteNumber = "APCN-2024-0003",
                    VendorCreditNoteReferenceNumber = "VCN-OEI-2024-001",
                    VendorCreditNoteDate = DateTime.Today.AddDays(-5),
                    CreditEntryDate = DateTime.Today.AddDays(-5),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    CreditNoteNarration = "Volume discount rebate for Q4 purchases",
                    CreditNoteType = VendorCreditNoteTypes.DiscountRebate,
                    IsAgainstBill = false,
                    SubTotalCreditAmount = 8000.00m,
                    TaxCreditAmount = 1440.00m,
                    RoundOffAmount = 0.00m,
                    TotalCreditAmount = 9440.00m,
                    AppliedAmount = 0.00m,
                    IsGSTApplicable = true,
                    CreditNoteStatus = VendorCreditNoteStatuses.Draft,
                    CreatedAt = DateTime.Today.AddDays(-5),
                    CreatedBy = "AP Clerk",
                    Lines = new List<VendorCreditNoteLineModel>
                    {
                        new VendorCreditNoteLineModel
                        {
                            Id = Guid.NewGuid(),
                            VendorCreditNoteId = VendorCreditNote3Id,
                            LineNumber = 10,
                            LineType = VendorCreditNoteLineTypes.Manual,
                            Description = "Volume discount - 5% rebate on Q4 total purchases",
                            Quantity = 1,
                            UnitPrice = 8000.00m,
                            DiscountPercent = 0,
                            DiscountAmount = 0,
                            TaxRatePercent = 18,
                            TaxAmount = 1440.00m,
                            ReversalAccountId = PurchaseReturnAccountId,
                            ReversalAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                            ReversalAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                            CreatedAt = DateTime.Today.AddDays(-5),
                            CreatedBy = "AP Clerk"
                        }
                    }
                },

                // Credit Note 4: Submitted credit note - Damage Claim
                new VendorCreditNoteViewModel
                {
                    Id = VendorCreditNote4Id,
                    CompanyId = AscendingSoftwareCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = BangaloreBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    VendorId = Vendor3Id,
                    VendorCode = "VND-000003",
                    VendorName = "Reliable Supplies Co",
                    CreditNoteNumber = "APCN-2024-0004",
                    VendorCreditNoteReferenceNumber = "VCN-GIT-2024-001",
                    VendorCreditNoteDate = DateTime.Today.AddDays(-3),
                    CreditEntryDate = DateTime.Today.AddDays(-3),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    CreditNoteNarration = "Claim for damaged goods received in transit",
                    CreditNoteType = VendorCreditNoteTypes.DamageClaim,
                    IsAgainstBill = true,
                    PrimaryVendorBillId = Bill3Id,
                    PrimaryVendorBillNumber = "BILL-2024-0003",
                    BillNumberSnapshot = "BILL-2024-0003",
                    BillDateSnapshot = DateTime.Today.AddDays(-15),
                    SubTotalCreditAmount = 35000.00m,
                    TaxCreditAmount = 6300.00m,
                    RoundOffAmount = 0.00m,
                    TotalCreditAmount = 41300.00m,
                    AppliedAmount = 0.00m,
                    IsGSTApplicable = true,
                    CreditNoteStatus = VendorCreditNoteStatuses.Submitted,
                    SubmittedOn = DateTime.Today.AddDays(-2),
                    SubmittedByUserId = MasterDataIds.PaymentTerms.Net45,
                    SubmittedByUserName = "AP Clerk",
                    CreatedAt = DateTime.Today.AddDays(-3),
                    CreatedBy = "AP Clerk",
                    HasAttachments = true,
                    AttachmentCount = 2,
                    Lines = new List<VendorCreditNoteLineModel>
                    {
                        new VendorCreditNoteLineModel
                        {
                            Id = Guid.NewGuid(),
                            VendorCreditNoteId = VendorCreditNote4Id,
                            LineNumber = 10,
                            LineType = VendorCreditNoteLineTypes.Item,
                            Description = "Server Equipment - Damaged in Transit",
                            Quantity = 1,
                            UnitPrice = 35000.00m,
                            DiscountPercent = 0,
                            DiscountAmount = 0,
                            TaxRatePercent = 18,
                            TaxAmount = 6300.00m,
                            ReversalAccountId = ExpenseAccountId,
                            ReversalAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                            ReversalAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                            HSNCode = "84714100",
                            CreatedAt = DateTime.Today.AddDays(-3),
                            CreatedBy = "AP Clerk"
                        }
                    }
                },

                // Credit Note 5: Approved credit note - Billing Correction
                new VendorCreditNoteViewModel
                {
                    Id = VendorCreditNote5Id,
                    CompanyId = AscendingSoftwareCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    VendorId = Vendor1Id,
                    VendorCode = "VND-000001",
                    VendorName = "Tech Components India Pvt Ltd",
                    CreditNoteNumber = "APCN-2024-0005",
                    VendorCreditNoteReferenceNumber = "VCN-TS-2024-003",
                    VendorCreditNoteDate = DateTime.Today.AddDays(-1),
                    CreditEntryDate = DateTime.Today.AddDays(-1),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    CreditNoteNarration = "Billing correction - Incorrect quantity charged",
                    CreditNoteType = VendorCreditNoteTypes.BillingCorrection,
                    IsAgainstBill = true,
                    PrimaryVendorBillId = Bill1Id,
                    PrimaryVendorBillNumber = "BILL-2024-0001",
                    BillNumberSnapshot = "BILL-2024-0001",
                    BillDateSnapshot = DateTime.Today.AddDays(-45),
                    SubTotalCreditAmount = 5000.00m,
                    TaxCreditAmount = 900.00m,
                    RoundOffAmount = 0.00m,
                    TotalCreditAmount = 5900.00m,
                    AppliedAmount = 0.00m,
                    IsGSTApplicable = true,
                    VendorGSTINSnapshot = "33AABCT1234Z1ZP",
                    CreditNoteStatus = VendorCreditNoteStatuses.Approved,
                    ApprovedOn = DateTime.Today,
                    ApprovedByUserId = MasterDataIds.Tenants.Default,
                    ApprovedByUserName = "Finance Manager",
                    CreatedAt = DateTime.Today.AddDays(-1),
                    CreatedBy = "AP Clerk",
                    Lines = new List<VendorCreditNoteLineModel>
                    {
                        new VendorCreditNoteLineModel
                        {
                            Id = Guid.NewGuid(),
                            VendorCreditNoteId = VendorCreditNote5Id,
                            LineNumber = 10,
                            LineType = VendorCreditNoteLineTypes.Manual,
                            Description = "Quantity correction - 2 units overcharged",
                            Quantity = 2,
                            UnitPrice = 2500.00m,
                            DiscountPercent = 0,
                            DiscountAmount = 0,
                            TaxRatePercent = 18,
                            TaxAmount = 900.00m,
                            ReversalAccountId = PurchaseReturnAccountId,
                            ReversalAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                            ReversalAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                            HSNCode = "84733010",
                            CreatedAt = DateTime.Today.AddDays(-1),
                            CreatedBy = "AP Clerk"
                        }
                    }
                },

                // Credit Note 6: Posted - Discount received
                new VendorCreditNoteViewModel
                {
                    Id = VendorCreditNote6Id,
                    CompanyId = AscendingSoftwareCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    VendorId = Vendor2Id,
                    VendorCode = "VND-000002",
                    VendorName = "CloudTech Solutions",
                    CreditNoteNumber = "APCN-2024-0006",
                    VendorCreditNoteReferenceNumber = "VCN-CT-2024-001",
                    VendorCreditNoteDate = DateTime.Today.AddDays(-25),
                    CreditEntryDate = DateTime.Today.AddDays(-25),
                    PostingDate = DateTime.Today.AddDays(-24),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    CreditNoteNarration = "Volume discount for annual contract",
                    CreditNoteType = VendorCreditNoteTypes.DiscountRebate,
                    IsAgainstBill = true,
                    PrimaryVendorBillId = Bill1Id,
                    PrimaryVendorBillNumber = "APB-2025-0001",
                    BillNumberSnapshot = "APB-2025-0001",
                    BillDateSnapshot = DateTime.Today.AddDays(-45),
                    SubTotalCreditAmount = 15000.00m,
                    TaxCreditAmount = 2700.00m,
                    RoundOffAmount = 0.00m,
                    TotalCreditAmount = 17700.00m,
                    AppliedAmount = 17700.00m,
                    IsGSTApplicable = true,
                    VendorGSTINSnapshot = "29AABCC5678B1ZB",
                    CreditNoteStatus = VendorCreditNoteStatuses.Posted,
                    PostedOn = DateTime.Today.AddDays(-24),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedByUserName = "Finance Controller",
                    CreatedAt = DateTime.Today.AddDays(-25),
                    CreatedBy = "AP Clerk",
                    Lines = new List<VendorCreditNoteLineModel>
                    {
                        new VendorCreditNoteLineModel
                        {
                            Id = Guid.NewGuid(),
                            VendorCreditNoteId = VendorCreditNote6Id,
                            LineNumber = 10,
                            LineType = VendorCreditNoteLineTypes.Manual,
                            Description = "Annual volume discount on services",
                            Quantity = 1,
                            UnitPrice = 15000.00m,
                            TaxRatePercent = 18,
                            TaxAmount = 2700.00m,
                            ReversalAccountId = ExpenseAccountId,
                            ReversalAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                            ReversalAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                            CreatedAt = DateTime.Today.AddDays(-25),
                            CreatedBy = "AP Clerk"
                        }
                    }
                },

                // Credit Note 7: Posted - Quality issue refund
                new VendorCreditNoteViewModel
                {
                    Id = VendorCreditNote7Id,
                    CompanyId = AscendingSoftwareCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    VendorId = Vendor3Id,
                    VendorCode = "VND-000003",
                    VendorName = "Reliable Supplies Co",
                    CreditNoteNumber = "APCN-2024-0007",
                    VendorCreditNoteReferenceNumber = "VCN-GF-2024-001",
                    VendorCreditNoteDate = DateTime.Today.AddDays(-18),
                    CreditEntryDate = DateTime.Today.AddDays(-18),
                    PostingDate = DateTime.Today.AddDays(-17),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    CreditNoteNarration = "Refund for damaged goods during transit",
                    CreditNoteType = VendorCreditNoteTypes.PurchaseReturn,
                    IsAgainstBill = false,
                    SubTotalCreditAmount = 22000.00m,
                    TaxCreditAmount = 3960.00m,
                    TotalCreditAmount = 25960.00m,
                    AppliedAmount = 0.00m,
                    IsGSTApplicable = true,
                    VendorGSTINSnapshot = "27AABCG1234C1ZC",
                    CreditNoteStatus = VendorCreditNoteStatuses.Posted,
                    PostedOn = DateTime.Today.AddDays(-17),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedByUserName = "Finance Controller",
                    CreatedAt = DateTime.Today.AddDays(-18),
                    CreatedBy = "AP Clerk",
                    Lines = new List<VendorCreditNoteLineModel>
                    {
                        new VendorCreditNoteLineModel
                        {
                            Id = Guid.NewGuid(),
                            VendorCreditNoteId = VendorCreditNote7Id,
                            LineNumber = 10,
                            LineType = VendorCreditNoteLineTypes.Manual,
                            Description = "Damaged goods compensation",
                            Quantity = 1,
                            UnitPrice = 22000.00m,
                            TaxRatePercent = 18,
                            TaxAmount = 3960.00m,
                            ReversalAccountId = PurchaseReturnAccountId,
                            ReversalAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                            ReversalAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                            CreatedAt = DateTime.Today.AddDays(-18),
                            CreatedBy = "AP Clerk"
                        }
                    }
                },

                // Credit Note 8: Submitted
                new VendorCreditNoteViewModel
                {
                    Id = VendorCreditNote8Id,
                    CompanyId = AscendingSoftwareCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = BangaloreBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    VendorId = Vendor4Id,
                    VendorCode = "VND-000004",
                    VendorName = "BuildRight Constructions",
                    CreditNoteNumber = "APCN-2024-0008",
                    VendorCreditNoteReferenceNumber = "VCN-PC-2024-001",
                    VendorCreditNoteDate = DateTime.Today.AddDays(-8),
                    CreditEntryDate = DateTime.Today.AddDays(-8),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    CreditNoteNarration = "Service level agreement penalty credit",
                    CreditNoteType = VendorCreditNoteTypes.BillingCorrection,
                    IsAgainstBill = true,
                    PrimaryVendorBillId = Bill2Id,
                    PrimaryVendorBillNumber = "APB-2025-0002",
                    BillNumberSnapshot = "APB-2025-0002",
                    BillDateSnapshot = DateTime.Today.AddDays(-40),
                    SubTotalCreditAmount = 35000.00m,
                    TaxCreditAmount = 6300.00m,
                    TotalCreditAmount = 41300.00m,
                    AppliedAmount = 0.00m,
                    IsGSTApplicable = true,
                    CreditNoteStatus = VendorCreditNoteStatuses.Submitted,
                    CreatedAt = DateTime.Today.AddDays(-8),
                    CreatedBy = "AP Clerk",
                    Lines = new List<VendorCreditNoteLineModel>
                    {
                        new VendorCreditNoteLineModel
                        {
                            Id = Guid.NewGuid(),
                            VendorCreditNoteId = VendorCreditNote8Id,
                            LineNumber = 10,
                            LineType = VendorCreditNoteLineTypes.Manual,
                            Description = "SLA penalty - Downtime credit",
                            Quantity = 1,
                            UnitPrice = 35000.00m,
                            TaxRatePercent = 18,
                            TaxAmount = 6300.00m,
                            ReversalAccountId = ExpenseAccountId,
                            ReversalAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                            ReversalAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                            CreatedAt = DateTime.Today.AddDays(-8),
                            CreatedBy = "AP Clerk"
                        }
                    }
                },

                // Credit Note 9: Draft
                new VendorCreditNoteViewModel
                {
                    Id = VendorCreditNote9Id,
                    CompanyId = AscendingSoftwareCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    VendorId = Vendor5Id,
                    VendorCode = "VND-000005",
                    VendorName = "Tamil Nadu Electricity Board",
                    CreditNoteNumber = "APCN-2024-0009",
                    VendorCreditNoteDate = DateTime.Today.AddDays(-3),
                    CreditEntryDate = DateTime.Today.AddDays(-3),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    CreditNoteNarration = "Meter reading correction credit",
                    CreditNoteType = VendorCreditNoteTypes.BillingCorrection,
                    IsAgainstBill = false,
                    SubTotalCreditAmount = 8000.00m,
                    TaxCreditAmount = 1440.00m,
                    TotalCreditAmount = 9440.00m,
                    AppliedAmount = 0.00m,
                    IsGSTApplicable = true,
                    CreditNoteStatus = VendorCreditNoteStatuses.Draft,
                    CreatedAt = DateTime.Today.AddDays(-3),
                    CreatedBy = "AP Clerk",
                    Lines = new List<VendorCreditNoteLineModel>
                    {
                        new VendorCreditNoteLineModel
                        {
                            Id = Guid.NewGuid(),
                            VendorCreditNoteId = VendorCreditNote9Id,
                            LineNumber = 10,
                            LineType = VendorCreditNoteLineTypes.Manual,
                            Description = "Electricity meter correction",
                            Quantity = 1,
                            UnitPrice = 8000.00m,
                            TaxRatePercent = 18,
                            TaxAmount = 1440.00m,
                            ReversalAccountId = ExpenseAccountId,
                            ReversalAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                            ReversalAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                            CreatedAt = DateTime.Today.AddDays(-3),
                            CreatedBy = "AP Clerk"
                        }
                    }
                },

                // Credit Note 10: Posted - Early payment discount
                new VendorCreditNoteViewModel
                {
                    Id = VendorCreditNote10Id,
                    CompanyId = AscendingSoftwareCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    VendorId = Vendor1Id,
                    VendorCode = "VND-000001",
                    VendorName = "Tech Components India Pvt Ltd",
                    CreditNoteNumber = "APCN-2024-0010",
                    VendorCreditNoteReferenceNumber = "VCN-TS-2024-004",
                    VendorCreditNoteDate = DateTime.Today.AddDays(-40),
                    CreditEntryDate = DateTime.Today.AddDays(-40),
                    PostingDate = DateTime.Today.AddDays(-39),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    CreditNoteNarration = "Early payment discount - 2% off",
                    CreditNoteType = VendorCreditNoteTypes.DiscountRebate,
                    IsAgainstBill = true,
                    PrimaryVendorBillId = Bill1Id,
                    PrimaryVendorBillNumber = "APB-2024-0001",
                    BillNumberSnapshot = "APB-2024-0001",
                    BillDateSnapshot = DateTime.Today.AddDays(-60),
                    SubTotalCreditAmount = 6000.00m,
                    TaxCreditAmount = 1080.00m,
                    TotalCreditAmount = 7080.00m,
                    AppliedAmount = 7080.00m,
                    IsGSTApplicable = true,
                    VendorGSTINSnapshot = "33AABCT1234Z1ZP",
                    CreditNoteStatus = VendorCreditNoteStatuses.Posted,
                    PostedOn = DateTime.Today.AddDays(-39),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedByUserName = "Finance Controller",
                    CreatedAt = DateTime.Today.AddDays(-40),
                    CreatedBy = "Finance Manager",
                    Lines = new List<VendorCreditNoteLineModel>
                    {
                        new VendorCreditNoteLineModel
                        {
                            Id = Guid.NewGuid(),
                            VendorCreditNoteId = VendorCreditNote10Id,
                            LineNumber = 10,
                            LineType = VendorCreditNoteLineTypes.Manual,
                            Description = "Early payment discount",
                            Quantity = 1,
                            UnitPrice = 6000.00m,
                            TaxRatePercent = 18,
                            TaxAmount = 1080.00m,
                            ReversalAccountId = PurchaseReturnAccountId,
                            ReversalAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                            ReversalAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                            CreatedAt = DateTime.Today.AddDays(-40),
                            CreatedBy = "Finance Manager"
                        }
                    }
                },

                // Credit Note 11: Posted - Rate difference
                new VendorCreditNoteViewModel
                {
                    Id = VendorCreditNote11Id,
                    CompanyId = AscendingSoftwareCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = BangaloreBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    VendorId = Vendor6Id,
                    VendorCode = "VND-000006",
                    VendorName = "Prime Properties",
                    CreditNoteNumber = "APCN-2024-0011",
                    VendorCreditNoteReferenceNumber = "VCN-PP-2024-001",
                    VendorCreditNoteDate = DateTime.Today.AddDays(-15),
                    CreditEntryDate = DateTime.Today.AddDays(-15),
                    PostingDate = DateTime.Today.AddDays(-14),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    CreditNoteNarration = "Rent reduction for maintenance downtime",
                    CreditNoteType = VendorCreditNoteTypes.BillingCorrection,
                    IsAgainstBill = true,
                    PrimaryVendorBillId = Bill3Id,
                    PrimaryVendorBillNumber = "APB-2025-0003",
                    BillNumberSnapshot = "APB-2025-0003",
                    BillDateSnapshot = DateTime.Today.AddDays(-30),
                    SubTotalCreditAmount = 12000.00m,
                    TaxCreditAmount = 2160.00m,
                    TotalCreditAmount = 14160.00m,
                    AppliedAmount = 14160.00m,
                    IsGSTApplicable = true,
                    CreditNoteStatus = VendorCreditNoteStatuses.Posted,
                    PostedOn = DateTime.Today.AddDays(-14),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedByUserName = "Finance Controller",
                    CreatedAt = DateTime.Today.AddDays(-15),
                    CreatedBy = "AP Clerk",
                    Lines = new List<VendorCreditNoteLineModel>
                    {
                        new VendorCreditNoteLineModel
                        {
                            Id = Guid.NewGuid(),
                            VendorCreditNoteId = VendorCreditNote11Id,
                            LineNumber = 10,
                            LineType = VendorCreditNoteLineTypes.Manual,
                            Description = "Rent credit for maintenance period",
                            Quantity = 1,
                            UnitPrice = 12000.00m,
                            TaxRatePercent = 18,
                            TaxAmount = 2160.00m,
                            ReversalAccountId = ExpenseAccountId,
                            ReversalAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                            ReversalAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                            CreatedAt = DateTime.Today.AddDays(-15),
                            CreatedBy = "AP Clerk"
                        }
                    }
                },

                // Credit Note 12: Cancelled
                new VendorCreditNoteViewModel
                {
                    Id = VendorCreditNote12Id,
                    CompanyId = AscendingSoftwareCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    VendorId = Vendor2Id,
                    VendorCode = "VND-000002",
                    VendorName = "CloudTech Solutions",
                    CreditNoteNumber = "APCN-2024-0012",
                    VendorCreditNoteDate = DateTime.Today.AddDays(-35),
                    CreditEntryDate = DateTime.Today.AddDays(-35),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    CreditNoteNarration = "Cancelled - Duplicate entry",
                    CreditNoteType = VendorCreditNoteTypes.PurchaseReturn,
                    IsAgainstBill = false,
                    SubTotalCreditAmount = 10000.00m,
                    TaxCreditAmount = 1800.00m,
                    TotalCreditAmount = 11800.00m,
                    AppliedAmount = 0.00m,
                    IsGSTApplicable = true,
                    CreditNoteStatus = VendorCreditNoteStatuses.Cancelled,
                    CancellationReason = "Duplicate credit note entry",
                    CreatedAt = DateTime.Today.AddDays(-35),
                    CreatedBy = "AP Clerk",
                    Lines = new List<VendorCreditNoteLineModel>
                    {
                        new VendorCreditNoteLineModel
                        {
                            Id = Guid.NewGuid(),
                            VendorCreditNoteId = VendorCreditNote12Id,
                            LineNumber = 10,
                            LineType = VendorCreditNoteLineTypes.Manual,
                            Description = "Cancelled item return",
                            Quantity = 1,
                            UnitPrice = 10000.00m,
                            TaxRatePercent = 18,
                            TaxAmount = 1800.00m,
                            ReversalAccountId = PurchaseReturnAccountId,
                            ReversalAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                            ReversalAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                            CreatedAt = DateTime.Today.AddDays(-35),
                            CreatedBy = "AP Clerk"
                        }
                    }
                }
            };

            return creditNotes;
        }
    }
}
