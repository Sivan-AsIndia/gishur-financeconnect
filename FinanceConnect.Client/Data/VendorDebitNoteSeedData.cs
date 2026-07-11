using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    /// <summary>
    /// Seed data for VendorDebitNote model (Model #40)
    /// </summary>
    public static class VendorDebitNoteSeedData
    {
        // Company GUIDs (matching existing company seed data)
        private static readonly Guid AscendingSoftwareCompanyId = MasterDataIds.Companies.SofaCraft;
        private static readonly Guid GlobalTechCompanyId = MasterDataIds.Companies.SofaCraftUSA;

        // Branch GUIDs (matching existing branch seed data)
        private static readonly Guid ChennaiHQBranchId = MasterDataIds.Branches.SofaCraftHQ;
        private static readonly Guid BangaloreBranchId = MasterDataIds.Branches.SofaCraftBengaluru;
        private static readonly Guid HyderabadBranchId = MasterDataIds.Branches.CozyCraftHyderabad;

        // Vendor GUIDs (matching VendorSeedData)
        private static readonly Guid Vendor1Id = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000001");
        private static readonly Guid Vendor2Id = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000002");
        private static readonly Guid Vendor3Id = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000003");
        private static readonly Guid Vendor4Id = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000004");

        // Currency GUIDs
        private static readonly Guid InrCurrencyId = MasterDataIds.Currencies.INR;

        // GL Account GUIDs
        private static readonly Guid PayableAccountId = MasterDataIds.PaymentTerms.Net45;
        private static readonly Guid ExpenseAccountId = Guid.Parse("00000000-0000-0000-0000-000000000030");
        private static readonly Guid FreightExpenseAccountId = MasterDataIds.Accounts.CostOfMaterials;
        private static readonly Guid PenaltyExpenseAccountId = MasterDataIds.Accounts.CostOfMaterials;
        private static readonly Guid TaxAccountId = Guid.Parse("a0000006-0006-0006-0006-000000000061");

        // Tax Code GUIDs
        private static readonly Guid GST18TaxCodeId = Guid.Parse("00000000-0000-0000-0000-000000000101");
        private static readonly Guid GST12TaxCodeId = Guid.Parse("00000000-0000-0000-0000-000000000102");
        private static readonly Guid ZeroRatedTaxCodeId = Guid.Parse("00000000-0000-0000-0000-000000000103");

        // VendorBill GUIDs (matching VendorBillSeedData)
        private static readonly Guid Bill1Id = Guid.Parse("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e001");
        private static readonly Guid Bill2Id = Guid.Parse("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e002");
        private static readonly Guid Bill3Id = Guid.Parse("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e003");

        // Predefined Vendor Debit Note GUIDs
        public static readonly Guid VendorDebitNote1Id = Guid.Parse("e3e3e3e3-0001-0001-0001-000000000001");
        public static readonly Guid VendorDebitNote2Id = Guid.Parse("e3e3e3e3-0001-0001-0001-000000000002");
        public static readonly Guid VendorDebitNote3Id = Guid.Parse("e3e3e3e3-0001-0001-0001-000000000003");
        public static readonly Guid VendorDebitNote4Id = Guid.Parse("e3e3e3e3-0001-0001-0001-000000000004");
        public static readonly Guid VendorDebitNote5Id = Guid.Parse("e3e3e3e3-0001-0001-0001-000000000005");
        public static readonly Guid VendorDebitNote6Id = Guid.Parse("e3e3e3e3-0001-0001-0001-000000000006");

        public static readonly Guid VendorDebitNote7Id = Guid.Parse("e4e4e4e4-0001-0001-0001-000000000007");
        public static readonly Guid VendorDebitNote8Id = Guid.Parse("e4e4e4e4-0001-0001-0001-000000000008");
        public static readonly Guid VendorDebitNote9Id = Guid.Parse("e4e4e4e4-0001-0001-0001-000000000009");
        public static readonly Guid VendorDebitNote10Id = Guid.Parse("e4e4e4e4-0001-0001-0001-000000000010");
        public static readonly Guid VendorDebitNote11Id = Guid.Parse("e4e4e4e4-0001-0001-0001-000000000011");
        public static readonly Guid VendorDebitNote12Id = Guid.Parse("e4e4e4e4-0001-0001-0001-000000000012");

        public static List<VendorDebitNoteViewModel> GetSeedDebitNotes()
        {
            var debitNotes = new List<VendorDebitNoteViewModel>
            {
                // Debit Note 1: Posted - Price Increase against bill
                CreateDebitNote1(),
                
                // Debit Note 2: Posted - Freight Charges
                CreateDebitNote2(),
                
                // Debit Note 3: Posted - Penalty Charges
                CreateDebitNote3(),
                
                // Debit Note 4: Submitted - Service Add-On
                CreateDebitNote4(),
                
                // Debit Note 5: Draft - Tax Difference
                CreateDebitNote5(),
                
                // Debit Note 6: Approved - Billing Correction
                CreateDebitNote6(),
                CreateDebitNote7(),
                CreateDebitNote8(),
                CreateDebitNote9(),
                CreateDebitNote10(),
                CreateDebitNote11(),
                CreateDebitNote12()
            };

            return debitNotes;
        }

        private static VendorDebitNoteViewModel CreateDebitNote1()
        {
            var debitNote = new VendorDebitNoteViewModel
            {
                Id = VendorDebitNote1Id,
                CompanyId = AscendingSoftwareCompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                BranchId = ChennaiHQBranchId,
                BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.CozyCraftHyderabad),
                BranchName = SeedLookup.BranchName(MasterDataIds.Branches.CozyCraftHyderabad),
                VendorId = Vendor1Id,
                VendorCode = "VND-000001",
                VendorName = "Tech Components India Pvt Ltd",
                DebitNoteNumber = "APDN-2024-0001",
                VendorDebitNoteReferenceNumber = "VDN-TS-2024-001",
                VendorDebitNoteDate = DateTime.Today.AddDays(-30),
                DebitEntryDate = DateTime.Today.AddDays(-29),
                PostingDate = DateTime.Today.AddDays(-29),
                CurrencyId = InrCurrencyId,
                CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                ExchangeRate = 1,
                DebitNoteNarration = "Price increase notification for server hardware components as per revised contract terms",
                DebitNoteType = VendorDebitNoteTypes.PriceIncrease,
                IsAgainstBill = true,
                PrimaryVendorBillId = Bill1Id,
                PrimaryVendorBillNumber = "APB-2024-0001",
                BillNumberSnapshot = "APB-2024-0001",
                BillDateSnapshot = DateTime.Today.AddDays(-45),
                SubTotalDebitAmount = 25000.00m,
                TaxDebitAmount = 4500.00m,
                RoundOffAmount = 0.00m,
                TotalDebitAmount = 29500.00m,
                AppliedAmount = 29500.00m,
                IsGSTApplicable = true,
                VendorGSTINSnapshot = "33AABCT1234Z1Z5",
                PayableAccountIdSnapshot = PayableAccountId,
                PayableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                PayableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                ExpenseAccountId = ExpenseAccountId,
                ExpenseAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                ExpenseAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                TaxAccountIdSnapshot = TaxAccountId,
                TaxAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                TaxAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                DebitNoteStatus = VendorDebitNoteStatuses.Posted,
                PostedOn = DateTime.Today.AddDays(-29),
                PostedByUserId = MasterDataIds.Tenants.Default,
                PostedByUserName = "Finance Controller",
                HasAttachments = true,
                AttachmentCount = 1,
                CreatedAt = DateTime.Today.AddDays(-30),
                CreatedBy = "AP Clerk"
            };

            AddLinesToDebitNote1(debitNote);
            return debitNote;
        }

        private static VendorDebitNoteViewModel CreateDebitNote2()
        {
            var debitNote = new VendorDebitNoteViewModel
            {
                Id = VendorDebitNote2Id,
                CompanyId = AscendingSoftwareCompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                BranchId = ChennaiHQBranchId,
                BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.CozyCraftHyderabad),
                BranchName = SeedLookup.BranchName(MasterDataIds.Branches.CozyCraftHyderabad),
                VendorId = Vendor2Id,
                VendorCode = "VND-000002",
                VendorName = "CloudTech Solutions",
                DebitNoteNumber = "APDN-2024-0002",
                VendorDebitNoteReferenceNumber = "VDN-GLP-2024-087",
                VendorDebitNoteDate = DateTime.Today.AddDays(-20),
                DebitEntryDate = DateTime.Today.AddDays(-19),
                PostingDate = DateTime.Today.AddDays(-19),
                CurrencyId = InrCurrencyId,
                CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                ExchangeRate = 1,
                DebitNoteNarration = "Additional freight charges for expedited delivery of equipment",
                DebitNoteType = VendorDebitNoteTypes.FreightCharges,
                IsAgainstBill = false,
                PrimaryVendorBillId = null,
                PrimaryVendorBillNumber = null,
                BillNumberSnapshot = null,
                BillDateSnapshot = null,
                SubTotalDebitAmount = 8500.00m,
                TaxDebitAmount = 1530.00m,
                RoundOffAmount = 0.00m,
                TotalDebitAmount = 10030.00m,
                AppliedAmount = 0.00m,
                IsGSTApplicable = true,
                VendorGSTINSnapshot = "33AABCG5678H2H6",
                PayableAccountIdSnapshot = PayableAccountId,
                PayableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                PayableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                ExpenseAccountId = FreightExpenseAccountId,
                ExpenseAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                ExpenseAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                TaxAccountIdSnapshot = TaxAccountId,
                TaxAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                TaxAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                DebitNoteStatus = VendorDebitNoteStatuses.Posted,
                PostedOn = DateTime.Today.AddDays(-19),
                PostedByUserId = MasterDataIds.Tenants.Default,
                PostedByUserName = "Finance Controller",
                HasAttachments = true,
                AttachmentCount = 2,
                CreatedAt = DateTime.Today.AddDays(-20),
                CreatedBy = "AP Clerk"
            };

            AddLinesToDebitNote2(debitNote);
            return debitNote;
        }

        private static VendorDebitNoteViewModel CreateDebitNote3()
        {
            var debitNote = new VendorDebitNoteViewModel
            {
                Id = VendorDebitNote3Id,
                CompanyId = AscendingSoftwareCompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                BranchId = BangaloreBranchId,
                BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.CozyCraftHyderabad),
                BranchName = SeedLookup.BranchName(MasterDataIds.Branches.CozyCraftHyderabad),
                VendorId = Vendor3Id,
                VendorCode = "VND-000003",
                VendorName = "Reliable Supplies Co",
                DebitNoteNumber = "APDN-2024-0003",
                VendorDebitNoteReferenceNumber = "VDN-PIS-2024-034",
                VendorDebitNoteDate = DateTime.Today.AddDays(-15),
                DebitEntryDate = DateTime.Today.AddDays(-14),
                PostingDate = DateTime.Today.AddDays(-14),
                CurrencyId = InrCurrencyId,
                CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                ExchangeRate = 1,
                DebitNoteNarration = "Penalty charges for delayed project delivery as per contract terms",
                DebitNoteType = VendorDebitNoteTypes.PenaltyCharges,
                IsAgainstBill = true,
                PrimaryVendorBillId = Bill2Id,
                PrimaryVendorBillNumber = "APB-2024-0002",
                BillNumberSnapshot = "APB-2024-0002",
                BillDateSnapshot = DateTime.Today.AddDays(-60),
                SubTotalDebitAmount = 15000.00m,
                TaxDebitAmount = 2700.00m,
                RoundOffAmount = 0.00m,
                TotalDebitAmount = 17700.00m,
                AppliedAmount = 17700.00m,
                IsGSTApplicable = true,
                VendorGSTINSnapshot = "29AABCP9012J3J7",
                PayableAccountIdSnapshot = PayableAccountId,
                PayableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                PayableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                ExpenseAccountId = PenaltyExpenseAccountId,
                ExpenseAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                ExpenseAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                TaxAccountIdSnapshot = TaxAccountId,
                TaxAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                TaxAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                DebitNoteStatus = VendorDebitNoteStatuses.Posted,
                PostedOn = DateTime.Today.AddDays(-14),
                PostedByUserId = MasterDataIds.Tenants.Default,
                PostedByUserName = "Finance Controller",
                HasAttachments = true,
                AttachmentCount = 1,
                CreatedAt = DateTime.Today.AddDays(-15),
                CreatedBy = "AP Clerk"
            };

            AddLinesToDebitNote3(debitNote);
            return debitNote;
        }

        private static VendorDebitNoteViewModel CreateDebitNote4()
        {
            var debitNote = new VendorDebitNoteViewModel
            {
                Id = VendorDebitNote4Id,
                CompanyId = AscendingSoftwareCompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                BranchId = ChennaiHQBranchId,
                BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.CozyCraftHyderabad),
                BranchName = SeedLookup.BranchName(MasterDataIds.Branches.CozyCraftHyderabad),
                VendorId = Vendor1Id,
                VendorCode = "VND-000001",
                VendorName = "Tech Components India Pvt Ltd",
                DebitNoteNumber = "APDN-2024-0004",
                VendorDebitNoteReferenceNumber = "VDN-TS-2024-012",
                VendorDebitNoteDate = DateTime.Today.AddDays(-5),
                DebitEntryDate = DateTime.Today.AddDays(-5),
                PostingDate = null,
                CurrencyId = InrCurrencyId,
                CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                ExchangeRate = 1,
                DebitNoteNarration = "Additional maintenance service charges for Q4",
                DebitNoteType = VendorDebitNoteTypes.ServiceAddOn,
                IsAgainstBill = false,
                PrimaryVendorBillId = null,
                PrimaryVendorBillNumber = null,
                BillNumberSnapshot = null,
                BillDateSnapshot = null,
                SubTotalDebitAmount = 35000.00m,
                TaxDebitAmount = 6300.00m,
                RoundOffAmount = 0.00m,
                TotalDebitAmount = 41300.00m,
                AppliedAmount = 0.00m,
                IsGSTApplicable = true,
                VendorGSTINSnapshot = null,
                PayableAccountIdSnapshot = null,
                PayableAccountCode = null,
                PayableAccountName = null,
                ExpenseAccountId = ExpenseAccountId,
                ExpenseAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                ExpenseAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                TaxAccountIdSnapshot = null,
                TaxAccountCode = null,
                TaxAccountName = null,
                DebitNoteStatus = VendorDebitNoteStatuses.Submitted,
                SubmittedOn = DateTime.Today.AddDays(-4),
                SubmittedByUserId = MasterDataIds.PaymentTerms.Net45,
                SubmittedByUserName = "AP Clerk",
                HasAttachments = true,
                AttachmentCount = 1,
                CreatedAt = DateTime.Today.AddDays(-5),
                CreatedBy = "AP Clerk"
            };

            AddLinesToDebitNote4(debitNote);
            return debitNote;
        }

        private static VendorDebitNoteViewModel CreateDebitNote5()
        {
            var debitNote = new VendorDebitNoteViewModel
            {
                Id = VendorDebitNote5Id,
                CompanyId = AscendingSoftwareCompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                BranchId = HyderabadBranchId,
                BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.CozyCraftHyderabad),
                BranchName = SeedLookup.BranchName(MasterDataIds.Branches.CozyCraftHyderabad),
                VendorId = Vendor4Id,
                VendorCode = "VND-000004",
                VendorName = "BuildRight Constructions",
                DebitNoteNumber = "APDN-2024-0005",
                VendorDebitNoteReferenceNumber = "VDN-CNS-2024-015",
                VendorDebitNoteDate = DateTime.Today.AddDays(-2),
                DebitEntryDate = DateTime.Today.AddDays(-2),
                PostingDate = null,
                CurrencyId = InrCurrencyId,
                CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                ExchangeRate = 1,
                DebitNoteNarration = "GST rate difference correction - 12% to 18% on cloud services",
                DebitNoteType = VendorDebitNoteTypes.TaxDifference,
                IsAgainstBill = true,
                PrimaryVendorBillId = Bill3Id,
                PrimaryVendorBillNumber = "APB-2024-0003",
                BillNumberSnapshot = "APB-2024-0003",
                BillDateSnapshot = DateTime.Today.AddDays(-30),
                SubTotalDebitAmount = 0.00m,
                TaxDebitAmount = 6000.00m,
                RoundOffAmount = 0.00m,
                TotalDebitAmount = 6000.00m,
                AppliedAmount = 0.00m,
                IsGSTApplicable = true,
                VendorGSTINSnapshot = null,
                PayableAccountIdSnapshot = null,
                PayableAccountCode = null,
                PayableAccountName = null,
                ExpenseAccountId = ExpenseAccountId,
                ExpenseAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                ExpenseAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                TaxAccountIdSnapshot = null,
                TaxAccountCode = null,
                TaxAccountName = null,
                DebitNoteStatus = VendorDebitNoteStatuses.Draft,
                HasAttachments = false,
                AttachmentCount = 0,
                CreatedAt = DateTime.Today.AddDays(-2),
                CreatedBy = "AP Clerk"
            };

            AddLinesToDebitNote5(debitNote);
            return debitNote;
        }

        private static VendorDebitNoteViewModel CreateDebitNote6()
        {
            var debitNote = new VendorDebitNoteViewModel
            {
                Id = VendorDebitNote6Id,
                CompanyId = AscendingSoftwareCompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                BranchId = ChennaiHQBranchId,
                BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.CozyCraftHyderabad),
                BranchName = SeedLookup.BranchName(MasterDataIds.Branches.CozyCraftHyderabad),
                VendorId = Vendor2Id,
                VendorCode = "VND-000002",
                VendorName = "CloudTech Solutions",
                DebitNoteNumber = "APDN-2024-0006",
                VendorDebitNoteReferenceNumber = "VDN-GLP-2024-092",
                VendorDebitNoteDate = DateTime.Today.AddDays(-3),
                DebitEntryDate = DateTime.Today.AddDays(-3),
                PostingDate = null,
                CurrencyId = InrCurrencyId,
                CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                ExchangeRate = 1,
                DebitNoteNarration = "Billing correction for missed handling charges",
                DebitNoteType = VendorDebitNoteTypes.BillingCorrection,
                IsAgainstBill = true,
                PrimaryVendorBillId = Bill1Id,
                PrimaryVendorBillNumber = "APB-2024-0001",
                BillNumberSnapshot = "APB-2024-0001",
                BillDateSnapshot = DateTime.Today.AddDays(-45),
                SubTotalDebitAmount = 5000.00m,
                TaxDebitAmount = 900.00m,
                RoundOffAmount = 0.00m,
                TotalDebitAmount = 5900.00m,
                AppliedAmount = 0.00m,
                IsGSTApplicable = true,
                VendorGSTINSnapshot = null,
                PayableAccountIdSnapshot = null,
                PayableAccountCode = null,
                PayableAccountName = null,
                ExpenseAccountId = FreightExpenseAccountId,
                ExpenseAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                ExpenseAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                TaxAccountIdSnapshot = null,
                TaxAccountCode = null,
                TaxAccountName = null,
                DebitNoteStatus = VendorDebitNoteStatuses.Approved,
                SubmittedOn = DateTime.Today.AddDays(-2),
                SubmittedByUserId = MasterDataIds.PaymentTerms.Net45,
                SubmittedByUserName = "AP Clerk",
                ApprovedOn = DateTime.Today.AddDays(-1),
                ApprovedByUserId = MasterDataIds.Tenants.Default,
                ApprovedByUserName = "Finance Manager",
                HasAttachments = true,
                AttachmentCount = 1,
                CreatedAt = DateTime.Today.AddDays(-3),
                CreatedBy = "AP Clerk"
            };

            AddLinesToDebitNote6(debitNote);
            return debitNote;
        }

        #region Add Lines Methods

        private static void AddLinesToDebitNote1(VendorDebitNoteViewModel debitNote)
        {
            debitNote.Lines = new List<VendorDebitNoteLineViewModel>
            {
                new VendorDebitNoteLineViewModel
                {
                    Id = Guid.NewGuid(),
                    VendorDebitNoteId = debitNote.Id,
                    LineNumber = 10,
                    LineType = VendorDebitNoteLineTypes.Expense,
                    Description = "Price Increase - Server Components (10% revision)",
                    Quantity = 1,
                    UnitPrice = 25000.00m,
                    DiscountPercent = 0,
                    DiscountAmount = 0.00m,
                    TaxCodeId = GST18TaxCodeId,
                    TaxCodeCode = "GST18",
                    TaxCodeName = "GST 18%",
                    TaxRatePercent = 18,
                    TaxAmount = 4500.00m,
                    ExpenseOrAssetAccountId = ExpenseAccountId,
                    ExpenseOrAssetAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    ExpenseOrAssetAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    HSNCode = "847150",
                    CreatedAt = debitNote.CreatedAt,
                    CreatedBy = debitNote.CreatedBy
                }
            };
        }

        private static void AddLinesToDebitNote2(VendorDebitNoteViewModel debitNote)
        {
            debitNote.Lines = new List<VendorDebitNoteLineViewModel>
            {
                new VendorDebitNoteLineViewModel
                {
                    Id = Guid.NewGuid(),
                    VendorDebitNoteId = debitNote.Id,
                    LineNumber = 10,
                    LineType = VendorDebitNoteLineTypes.Charge,
                    Description = "Expedited Air Freight Charges",
                    Quantity = 1,
                    UnitPrice = 6500.00m,
                    DiscountPercent = 0,
                    DiscountAmount = 0.00m,
                    TaxCodeId = GST18TaxCodeId,
                    TaxCodeCode = "GST18",
                    TaxCodeName = "GST 18%",
                    TaxRatePercent = 18,
                    TaxAmount = 1170.00m,
                    ExpenseOrAssetAccountId = FreightExpenseAccountId,
                    ExpenseOrAssetAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    ExpenseOrAssetAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    SACCode = "996511",
                    CreatedAt = debitNote.CreatedAt,
                    CreatedBy = debitNote.CreatedBy
                },
                new VendorDebitNoteLineViewModel
                {
                    Id = Guid.NewGuid(),
                    VendorDebitNoteId = debitNote.Id,
                    LineNumber = 20,
                    LineType = VendorDebitNoteLineTypes.Charge,
                    Description = "Special Handling & Insurance",
                    Quantity = 1,
                    UnitPrice = 2000.00m,
                    DiscountPercent = 0,
                    DiscountAmount = 0.00m,
                    TaxCodeId = GST18TaxCodeId,
                    TaxCodeCode = "GST18",
                    TaxCodeName = "GST 18%",
                    TaxRatePercent = 18,
                    TaxAmount = 360.00m,
                    ExpenseOrAssetAccountId = FreightExpenseAccountId,
                    ExpenseOrAssetAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    ExpenseOrAssetAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    SACCode = "996512",
                    CreatedAt = debitNote.CreatedAt,
                    CreatedBy = debitNote.CreatedBy
                }
            };
        }

        private static void AddLinesToDebitNote3(VendorDebitNoteViewModel debitNote)
        {
            debitNote.Lines = new List<VendorDebitNoteLineViewModel>
            {
                new VendorDebitNoteLineViewModel
                {
                    Id = Guid.NewGuid(),
                    VendorDebitNoteId = debitNote.Id,
                    LineNumber = 10,
                    LineType = VendorDebitNoteLineTypes.Charge,
                    Description = "Penalty for Delayed Delivery - 15 days @ ₹1,000/day",
                    Quantity = 15,
                    UnitPrice = 1000.00m,
                    DiscountPercent = 0,
                    DiscountAmount = 0.00m,
                    TaxCodeId = GST18TaxCodeId,
                    TaxCodeCode = "GST18",
                    TaxCodeName = "GST 18%",
                    TaxRatePercent = 18,
                    TaxAmount = 2700.00m,
                    ExpenseOrAssetAccountId = PenaltyExpenseAccountId,
                    ExpenseOrAssetAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    ExpenseOrAssetAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    SACCode = "999799",
                    CreatedAt = debitNote.CreatedAt,
                    CreatedBy = debitNote.CreatedBy
                }
            };
        }

        private static void AddLinesToDebitNote4(VendorDebitNoteViewModel debitNote)
        {
            debitNote.Lines = new List<VendorDebitNoteLineViewModel>
            {
                new VendorDebitNoteLineViewModel
                {
                    Id = Guid.NewGuid(),
                    VendorDebitNoteId = debitNote.Id,
                    LineNumber = 10,
                    LineType = VendorDebitNoteLineTypes.Service,
                    Description = "Extended Maintenance Service - Q4 Add-On",
                    Quantity = 3,
                    UnitPrice = 10000.00m,
                    DiscountPercent = 0,
                    DiscountAmount = 0.00m,
                    TaxCodeId = GST18TaxCodeId,
                    TaxCodeCode = "GST18",
                    TaxCodeName = "GST 18%",
                    TaxRatePercent = 18,
                    TaxAmount = 5400.00m,
                    ExpenseOrAssetAccountId = ExpenseAccountId,
                    ExpenseOrAssetAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    ExpenseOrAssetAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    SACCode = "998314",
                    CreatedAt = debitNote.CreatedAt,
                    CreatedBy = debitNote.CreatedBy
                },
                new VendorDebitNoteLineViewModel
                {
                    Id = Guid.NewGuid(),
                    VendorDebitNoteId = debitNote.Id,
                    LineNumber = 20,
                    LineType = VendorDebitNoteLineTypes.Service,
                    Description = "Priority Support Upgrade",
                    Quantity = 1,
                    UnitPrice = 5000.00m,
                    DiscountPercent = 0,
                    DiscountAmount = 0.00m,
                    TaxCodeId = GST18TaxCodeId,
                    TaxCodeCode = "GST18",
                    TaxCodeName = "GST 18%",
                    TaxRatePercent = 18,
                    TaxAmount = 900.00m,
                    ExpenseOrAssetAccountId = ExpenseAccountId,
                    ExpenseOrAssetAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    ExpenseOrAssetAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    SACCode = "998316",
                    CreatedAt = debitNote.CreatedAt,
                    CreatedBy = debitNote.CreatedBy
                }
            };
        }

        private static void AddLinesToDebitNote5(VendorDebitNoteViewModel debitNote)
        {
            debitNote.Lines = new List<VendorDebitNoteLineViewModel>
            {
                new VendorDebitNoteLineViewModel
                {
                    Id = Guid.NewGuid(),
                    VendorDebitNoteId = debitNote.Id,
                    LineNumber = 10,
                    LineType = VendorDebitNoteLineTypes.Charge,
                    Description = "GST Rate Differential - 6% on ₹100,000",
                    Quantity = 1,
                    UnitPrice = 0.00m,
                    DiscountPercent = 0,
                    DiscountAmount = 0.00m,
                    TaxCodeId = GST18TaxCodeId,
                    TaxCodeCode = "GST-DIFF",
                    TaxCodeName = "GST Differential 6%",
                    TaxRatePercent = 0,
                    TaxAmount = 6000.00m,
                    ExpenseOrAssetAccountId = ExpenseAccountId,
                    ExpenseOrAssetAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    ExpenseOrAssetAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    SACCode = "998314",
                    CreatedAt = debitNote.CreatedAt,
                    CreatedBy = debitNote.CreatedBy
                }
            };
        }

        private static void AddLinesToDebitNote6(VendorDebitNoteViewModel debitNote)
        {
            debitNote.Lines = new List<VendorDebitNoteLineViewModel>
            {
                new VendorDebitNoteLineViewModel
                {
                    Id = Guid.NewGuid(),
                    VendorDebitNoteId = debitNote.Id,
                    LineNumber = 10,
                    LineType = VendorDebitNoteLineTypes.Charge,
                    Description = "Handling Charges - Missed in Original Bill",
                    Quantity = 1,
                    UnitPrice = 5000.00m,
                    DiscountPercent = 0,
                    DiscountAmount = 0.00m,
                    TaxCodeId = GST18TaxCodeId,
                    TaxCodeCode = "GST18",
                    TaxCodeName = "GST 18%",
                    TaxRatePercent = 18,
                    TaxAmount = 900.00m,
                    ExpenseOrAssetAccountId = FreightExpenseAccountId,
                    ExpenseOrAssetAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                    ExpenseOrAssetAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                    SACCode = "996512",
                    CreatedAt = debitNote.CreatedAt,
                    CreatedBy = debitNote.CreatedBy
                }
            };
        }

        #endregion
        private static VendorDebitNoteViewModel CreateDebitNote7()
        {
            var debitNote = new VendorDebitNoteViewModel
            {
                Id = VendorDebitNote7Id,
                CompanyId = AscendingSoftwareCompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                BranchId = ChennaiHQBranchId,
                BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.CozyCraftHyderabad),
                BranchName = SeedLookup.BranchName(MasterDataIds.Branches.CozyCraftHyderabad),
                VendorId = Vendor1Id,
                VendorCode = "VND-000001",
                VendorName = "Tech Components India Pvt Ltd",
                DebitNoteNumber = "APDN-2024-0007",
                VendorDebitNoteReferenceNumber = "VDN-TS-2024-005",
                VendorDebitNoteDate = DateTime.Today.AddDays(-12),
                DebitEntryDate = DateTime.Today.AddDays(-11),
                PostingDate = DateTime.Today.AddDays(-11),
                CurrencyId = InrCurrencyId,
                CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                ExchangeRate = 1,
                DebitNoteNarration = "Late delivery penalty charges",
                DebitNoteType = VendorDebitNoteTypes.PenaltyCharges,
                IsAgainstBill = true,
                PrimaryVendorBillId = Bill2Id,
                PrimaryVendorBillNumber = "APB-2024-0002",
                BillNumberSnapshot = "APB-2024-0002",
                BillDateSnapshot = DateTime.Today.AddDays(-40),
                SubTotalDebitAmount = 7500.00m,
                TaxDebitAmount = 1350.00m,
                TotalDebitAmount = 8850.00m,
                AppliedAmount = 8850.00m,
                IsGSTApplicable = true,
                VendorGSTINSnapshot = "33AABCT1234Z1Z5",
                PayableAccountIdSnapshot = PayableAccountId,
                PayableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                PayableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                ExpenseAccountId = ExpenseAccountId,
                ExpenseAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                ExpenseAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                TaxAccountIdSnapshot = TaxAccountId,
                TaxAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                TaxAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                DebitNoteStatus = VendorDebitNoteStatuses.Posted,
                PostedOn = DateTime.Today.AddDays(-11),
                PostedByUserId = MasterDataIds.Tenants.Default,
                PostedByUserName = "Finance Controller",
                CreatedAt = DateTime.Today.AddDays(-12),
                CreatedBy = "AP Clerk"
            };
            debitNote.Lines = new List<VendorDebitNoteLineViewModel>
            {
                new VendorDebitNoteLineViewModel
                {
                    Id = Guid.NewGuid(), VendorDebitNoteId = VendorDebitNote7Id, LineNumber = 10,
                    Description = "Late delivery penalty", Quantity = 1, UnitPrice = 7500.00m,
                    TaxRatePercent = 18, TaxAmount = 1350.00m,
                    CreatedAt = DateTime.Today.AddDays(-12), CreatedBy = "AP Clerk"
                }
            };
            return debitNote;
        }

        private static VendorDebitNoteViewModel CreateDebitNote8()
        {
            var debitNote = new VendorDebitNoteViewModel
            {
                Id = VendorDebitNote8Id,
                CompanyId = AscendingSoftwareCompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                BranchId = BangaloreBranchId,
                BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.CozyCraftHyderabad),
                BranchName = SeedLookup.BranchName(MasterDataIds.Branches.CozyCraftHyderabad),
                VendorId = Vendor2Id,
                VendorCode = "VND-000002",
                VendorName = "CloudTech Solutions",
                DebitNoteNumber = "APDN-2024-0008",
                VendorDebitNoteReferenceNumber = "VDN-GLP-2024-102",
                VendorDebitNoteDate = DateTime.Today.AddDays(-8),
                DebitEntryDate = DateTime.Today.AddDays(-7),
                CurrencyId = InrCurrencyId,
                CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                ExchangeRate = 1,
                DebitNoteNarration = "Additional insurance charges for shipment",
                DebitNoteType = VendorDebitNoteTypes.FreightCharges,
                IsAgainstBill = false,
                SubTotalDebitAmount = 12000.00m,
                TaxDebitAmount = 2160.00m,
                TotalDebitAmount = 14160.00m,
                AppliedAmount = 0.00m,
                IsGSTApplicable = true,
                PayableAccountIdSnapshot = PayableAccountId,
                PayableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                PayableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                ExpenseAccountId = FreightExpenseAccountId,
                ExpenseAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                ExpenseAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                TaxAccountIdSnapshot = TaxAccountId,
                TaxAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                TaxAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                DebitNoteStatus = VendorDebitNoteStatuses.Submitted,
                CreatedAt = DateTime.Today.AddDays(-8),
                CreatedBy = "AP Clerk"
            };
            debitNote.Lines = new List<VendorDebitNoteLineViewModel>
            {
                new VendorDebitNoteLineViewModel
                {
                    Id = Guid.NewGuid(), VendorDebitNoteId = VendorDebitNote8Id, LineNumber = 10,
                    Description = "Insurance surcharge", Quantity = 1, UnitPrice = 12000.00m,
                    TaxRatePercent = 18, TaxAmount = 2160.00m,
                    CreatedAt = DateTime.Today.AddDays(-8), CreatedBy = "AP Clerk"
                }
            };
            return debitNote;
        }

        private static VendorDebitNoteViewModel CreateDebitNote9()
        {
            var debitNote = new VendorDebitNoteViewModel
            {
                Id = VendorDebitNote9Id,
                CompanyId = AscendingSoftwareCompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                BranchId = ChennaiHQBranchId,
                BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.CozyCraftHyderabad),
                BranchName = SeedLookup.BranchName(MasterDataIds.Branches.CozyCraftHyderabad),
                VendorId = Vendor3Id,
                VendorCode = "VND-000003",
                VendorName = "Reliable Supplies Co",
                DebitNoteNumber = "APDN-2024-0009",
                VendorDebitNoteDate = DateTime.Today.AddDays(-5),
                DebitEntryDate = DateTime.Today.AddDays(-4),
                CurrencyId = InrCurrencyId,
                CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                ExchangeRate = 1,
                DebitNoteNarration = "Rate revision for annual maintenance contract",
                DebitNoteType = VendorDebitNoteTypes.PriceIncrease,
                IsAgainstBill = true,
                PrimaryVendorBillId = Bill3Id,
                PrimaryVendorBillNumber = "APB-2024-0003",
                BillNumberSnapshot = "APB-2024-0003",
                BillDateSnapshot = DateTime.Today.AddDays(-35),
                SubTotalDebitAmount = 18000.00m,
                TaxDebitAmount = 3240.00m,
                TotalDebitAmount = 21240.00m,
                AppliedAmount = 0.00m,
                IsGSTApplicable = true,
                PayableAccountIdSnapshot = PayableAccountId,
                PayableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                PayableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                ExpenseAccountId = ExpenseAccountId,
                ExpenseAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                ExpenseAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                TaxAccountIdSnapshot = TaxAccountId,
                TaxAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                TaxAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                DebitNoteStatus = VendorDebitNoteStatuses.Draft,
                CreatedAt = DateTime.Today.AddDays(-5),
                CreatedBy = "AP Clerk"
            };
            debitNote.Lines = new List<VendorDebitNoteLineViewModel>
            {
                new VendorDebitNoteLineViewModel
                {
                    Id = Guid.NewGuid(), VendorDebitNoteId = VendorDebitNote9Id, LineNumber = 10,
                    Description = "AMC rate revision", Quantity = 1, UnitPrice = 18000.00m,
                    TaxRatePercent = 18, TaxAmount = 3240.00m,
                    CreatedAt = DateTime.Today.AddDays(-5), CreatedBy = "AP Clerk"
                }
            };
            return debitNote;
        }

        private static VendorDebitNoteViewModel CreateDebitNote10()
        {
            var debitNote = new VendorDebitNoteViewModel
            {
                Id = VendorDebitNote10Id,
                CompanyId = AscendingSoftwareCompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                BranchId = ChennaiHQBranchId,
                BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.CozyCraftHyderabad),
                BranchName = SeedLookup.BranchName(MasterDataIds.Branches.CozyCraftHyderabad),
                VendorId = Vendor4Id,
                VendorCode = "VND-000004",
                VendorName = "BuildRight Constructions",
                DebitNoteNumber = "APDN-2024-0010",
                VendorDebitNoteReferenceNumber = "VDN-CN-2024-001",
                VendorDebitNoteDate = DateTime.Today.AddDays(-25),
                DebitEntryDate = DateTime.Today.AddDays(-24),
                PostingDate = DateTime.Today.AddDays(-24),
                CurrencyId = InrCurrencyId,
                CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                ExchangeRate = 1,
                DebitNoteNarration = "Service upgrade charges - Premium tier",
                DebitNoteType = VendorDebitNoteTypes.ServiceAddOn,
                IsAgainstBill = false,
                SubTotalDebitAmount = 25000.00m,
                TaxDebitAmount = 4500.00m,
                TotalDebitAmount = 29500.00m,
                AppliedAmount = 29500.00m,
                IsGSTApplicable = true,
                PayableAccountIdSnapshot = PayableAccountId,
                PayableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                PayableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                ExpenseAccountId = ExpenseAccountId,
                ExpenseAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                ExpenseAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                TaxAccountIdSnapshot = TaxAccountId,
                TaxAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                TaxAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                DebitNoteStatus = VendorDebitNoteStatuses.Posted,
                PostedOn = DateTime.Today.AddDays(-24),
                PostedByUserId = MasterDataIds.Tenants.Default,
                PostedByUserName = "Finance Controller",
                CreatedAt = DateTime.Today.AddDays(-25),
                CreatedBy = "AP Clerk"
            };
            debitNote.Lines = new List<VendorDebitNoteLineViewModel>
            {
                new VendorDebitNoteLineViewModel
                {
                    Id = Guid.NewGuid(), VendorDebitNoteId = VendorDebitNote10Id, LineNumber = 10,
                    Description = "Premium tier upgrade", Quantity = 1, UnitPrice = 25000.00m,
                    TaxRatePercent = 18, TaxAmount = 4500.00m,
                    CreatedAt = DateTime.Today.AddDays(-25), CreatedBy = "AP Clerk"
                }
            };
            return debitNote;
        }

        private static VendorDebitNoteViewModel CreateDebitNote11()
        {
            var debitNote = new VendorDebitNoteViewModel
            {
                Id = VendorDebitNote11Id,
                CompanyId = AscendingSoftwareCompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                BranchId = BangaloreBranchId,
                BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.CozyCraftHyderabad),
                BranchName = SeedLookup.BranchName(MasterDataIds.Branches.CozyCraftHyderabad),
                VendorId = Vendor1Id,
                VendorCode = "VND-000001",
                VendorName = "Tech Components India Pvt Ltd",
                DebitNoteNumber = "APDN-2024-0011",
                VendorDebitNoteReferenceNumber = "VDN-TS-2024-006",
                VendorDebitNoteDate = DateTime.Today.AddDays(-35),
                DebitEntryDate = DateTime.Today.AddDays(-34),
                PostingDate = DateTime.Today.AddDays(-34),
                CurrencyId = InrCurrencyId,
                CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                ExchangeRate = 1,
                DebitNoteNarration = "Tax difference correction for previous quarter",
                DebitNoteType = VendorDebitNoteTypes.TaxDifference,
                IsAgainstBill = true,
                PrimaryVendorBillId = Bill1Id,
                PrimaryVendorBillNumber = "APB-2024-0001",
                BillNumberSnapshot = "APB-2024-0001",
                BillDateSnapshot = DateTime.Today.AddDays(-50),
                SubTotalDebitAmount = 0.00m,
                TaxDebitAmount = 3500.00m,
                TotalDebitAmount = 3500.00m,
                AppliedAmount = 3500.00m,
                IsGSTApplicable = true,
                PayableAccountIdSnapshot = PayableAccountId,
                PayableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                PayableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                ExpenseAccountId = ExpenseAccountId,
                ExpenseAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                ExpenseAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                TaxAccountIdSnapshot = TaxAccountId,
                TaxAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                TaxAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                DebitNoteStatus = VendorDebitNoteStatuses.Posted,
                PostedOn = DateTime.Today.AddDays(-34),
                PostedByUserId = MasterDataIds.Tenants.Default,
                PostedByUserName = "Finance Controller",
                CreatedAt = DateTime.Today.AddDays(-35),
                CreatedBy = "Finance Manager"
            };
            debitNote.Lines = new List<VendorDebitNoteLineViewModel>
            {
                new VendorDebitNoteLineViewModel
                {
                    Id = Guid.NewGuid(), VendorDebitNoteId = VendorDebitNote11Id, LineNumber = 10,
                    Description = "GST rate difference Q4", Quantity = 1, UnitPrice = 0.00m,
                    TaxRatePercent = 18, TaxAmount = 3500.00m,
                    CreatedAt = DateTime.Today.AddDays(-35), CreatedBy = "Finance Manager"
                }
            };
            return debitNote;
        }

        private static VendorDebitNoteViewModel CreateDebitNote12()
        {
            var debitNote = new VendorDebitNoteViewModel
            {
                Id = VendorDebitNote12Id,
                CompanyId = AscendingSoftwareCompanyId,
                CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraftUSA),
                BranchId = ChennaiHQBranchId,
                BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.CozyCraftHyderabad),
                BranchName = SeedLookup.BranchName(MasterDataIds.Branches.CozyCraftHyderabad),
                VendorId = Vendor2Id,
                VendorCode = "VND-000002",
                VendorName = "CloudTech Solutions",
                DebitNoteNumber = "APDN-2024-0012",
                VendorDebitNoteDate = DateTime.Today.AddDays(-2),
                DebitEntryDate = DateTime.Today.AddDays(-1),
                CurrencyId = InrCurrencyId,
                CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                ExchangeRate = 1,
                DebitNoteNarration = "Expedited handling surcharge",
                DebitNoteType = VendorDebitNoteTypes.FreightCharges,
                IsAgainstBill = false,
                SubTotalDebitAmount = 5500.00m,
                TaxDebitAmount = 990.00m,
                TotalDebitAmount = 6490.00m,
                AppliedAmount = 0.00m,
                IsGSTApplicable = true,
                PayableAccountIdSnapshot = PayableAccountId,
                PayableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                PayableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                ExpenseAccountId = FreightExpenseAccountId,
                ExpenseAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                ExpenseAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                TaxAccountIdSnapshot = TaxAccountId,
                TaxAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.CostOfMaterials),
                TaxAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.CostOfMaterials),
                DebitNoteStatus = VendorDebitNoteStatuses.Approved,
                ApprovedOn = DateTime.Today,
                ApprovedByUserId = MasterDataIds.Tenants.Default,
                ApprovedByUserName = "Finance Manager",
                CreatedAt = DateTime.Today.AddDays(-2),
                CreatedBy = "AP Clerk"
            };
            debitNote.Lines = new List<VendorDebitNoteLineViewModel>
            {
                new VendorDebitNoteLineViewModel
                {
                    Id = Guid.NewGuid(), VendorDebitNoteId = VendorDebitNote12Id, LineNumber = 10,
                    Description = "Expedited handling fee", Quantity = 1, UnitPrice = 5500.00m,
                    TaxRatePercent = 18, TaxAmount = 990.00m,
                    CreatedAt = DateTime.Today.AddDays(-2), CreatedBy = "AP Clerk"
                }
            };
            return debitNote;
        }
    }
}
