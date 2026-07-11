using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    /// <summary>
    /// Seed data for CustomerCreditNote model (Model #30)
    /// </summary>
    public static class CustomerCreditNoteSeedData
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

        // Currency GUIDs (matching MasterDataIds.Currencies)
        private static readonly Guid InrCurrencyId = MasterDataIds.Currencies.INR;

        // GL Account GUIDs (matching COADataService accounts)
        private static readonly Guid ReceivableAccountId = MasterDataIds.Accounts.AccountsReceivable;
        private static readonly Guid SalesReturnAccountId = MasterDataIds.Accounts.CostOfMaterials;
        private static readonly Guid ServiceRevenueAccountId = MasterDataIds.Accounts.ServiceRevenue;
        private static readonly Guid TaxAccountId = MasterDataIds.Accounts.GSTPayable;

        // Invoice GUIDs (matching CustomerInvoiceSeedData)
        private static readonly Guid Invoice1Id = Guid.Parse("a1a1a1a1-0001-0001-0001-000000000001");
        private static readonly Guid Invoice2Id = Guid.Parse("a1a1a1a1-0001-0001-0001-000000000002");
        private static readonly Guid Invoice3Id = Guid.Parse("a1a1a1a1-0001-0001-0001-000000000003");

        // Predefined Credit Note GUIDs
        public static readonly Guid CreditNote1Id = Guid.Parse("d2d2d2d2-0001-0001-0001-000000000001");
        public static readonly Guid CreditNote2Id = Guid.Parse("d2d2d2d2-0001-0001-0001-000000000002");
        public static readonly Guid CreditNote3Id = Guid.Parse("d2d2d2d2-0001-0001-0001-000000000003");
        public static readonly Guid CreditNote4Id = Guid.Parse("d2d2d2d2-0001-0001-0001-000000000004");
        public static readonly Guid CreditNote5Id = Guid.Parse("d2d2d2d2-0001-0001-0001-000000000005");
        public static readonly Guid CreditNote6Id = Guid.Parse("d2d2d2d2-0001-0001-0001-000000000006");
        public static readonly Guid CreditNote7Id = Guid.Parse("d2d2d2d2-0001-0001-0001-000000000007");
        public static readonly Guid CreditNote8Id = Guid.Parse("d2d2d2d2-0001-0001-0001-000000000008");
        public static readonly Guid CreditNote9Id = Guid.Parse("d2d2d2d2-0001-0001-0001-000000000009");
        public static readonly Guid CreditNote10Id = Guid.Parse("d2d2d2d2-0001-0001-0001-000000000010");
        public static readonly Guid CreditNote11Id = Guid.Parse("d2d2d2d2-0001-0001-0001-000000000011");
        public static readonly Guid CreditNote12Id = Guid.Parse("d2d2d2d2-0001-0001-0001-000000000012");

        public static List<CustomerCreditNoteViewModel> GetSeedCreditNotes()
        {
            var creditNotes = new List<CustomerCreditNoteViewModel>
            {
                // Credit Note 1: Posted credit note - Sales Return against Invoice
                new CustomerCreditNoteViewModel
                {
                    Id = CreditNote1Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    CustomerId = Customer1Id,
                    CustomerCode = "CUST-001",
                    CustomerName = "ABC Traders Private Limited",
                    CreditNoteNumber = "CN-2024-0001",
                    CreditNoteDate = DateTime.Today.AddDays(-30),
                    PostingDate = DateTime.Today.AddDays(-30),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    ReferenceText = "RET-2024-001",
                    CreditNoteNarration = "Sales return for damaged goods received",
                    IsAgainstInvoice = true,
                    CustomerInvoiceId = Invoice1Id,
                    CustomerInvoiceNumber = "INV-2024-0001",
                    InvoiceNumberSnapshot = "INV-2024-0001",
                    InvoiceDateSnapshot = DateTime.Today.AddDays(-45),
                    CreditReasonCode = CreditReasonCodes.SalesReturn,
                    CreditReasonDescription = "Sales Return - Damaged goods",
                    IsTaxImpacting = true,
                    IsRevenueReversal = true,
                    SubTotalAmount = 10000.00m,
                    DiscountTotalAmount = 0.00m,
                    TaxTotalAmount = 1800.00m,
                    RoundOffAmount = 0.00m,
                    GrandTotalAmount = 11800.00m,
                    AppliedToInvoiceAmount = 11800.00m,
                    ReceivableAccountIdSnapshot = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.GSTPayable),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.GSTPayable),
                    RevenueReversalAccountId = SalesReturnAccountId,
                    RevenueReversalAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.GSTPayable),
                    RevenueReversalAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.GSTPayable),
                    CreditNoteStatus = CreditNoteStatuses.Posted,
                    PostedOn = DateTime.Today.AddDays(-30),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedByUserName = "Finance Controller",
                    CreatedAt = DateTime.Today.AddDays(-31),
                    CreatedBy = "AR Clerk",
                    Lines = new List<CustomerCreditNoteLineModel>
                    {
                        new CustomerCreditNoteLineModel
                        {
                            Id = Guid.NewGuid(),
                            CustomerCreditNoteId = CreditNote1Id,
                            LineNumber = 10,
                            LineType = LineTypes.Item,
                            Description = "Software License - Partial Return (Damaged media)",
                            Quantity = 2,
                            UnitPrice = 5000.00m,
                            DiscountPercent = 0,
                            DiscountAmount = 0,
                            TaxRatePercent = 18,
                            TaxAmount = 1800.00m,
                            RevenueReversalAccountId = SalesReturnAccountId,
                            RevenueReversalAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.GSTPayable),
                            RevenueReversalAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.GSTPayable),
                            CreatedAt = DateTime.Today.AddDays(-31),
                            CreatedBy = "AR Clerk"
                        }
                    }
                },

                // Credit Note 2: Posted credit note - Price Correction
                new CustomerCreditNoteViewModel
                {
                    Id = CreditNote2Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    CustomerId = Customer1Id,
                    CustomerCode = "CUST-001",
                    CustomerName = "ABC Traders Private Limited",
                    CreditNoteNumber = "CN-2024-0002",
                    CreditNoteDate = DateTime.Today.AddDays(-20),
                    PostingDate = DateTime.Today.AddDays(-20),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    ReferenceText = "PRICE-ADJ-001",
                    CreditNoteNarration = "Price correction as per agreed contract terms",
                    IsAgainstInvoice = true,
                    CustomerInvoiceId = Invoice2Id,
                    CustomerInvoiceNumber = "INV-2024-0002",
                    InvoiceNumberSnapshot = "INV-2024-0002",
                    InvoiceDateSnapshot = DateTime.Today.AddDays(-30),
                    CreditReasonCode = CreditReasonCodes.PriceCorrection,
                    CreditReasonDescription = "Price Correction - Contract rate adjustment",
                    IsTaxImpacting = true,
                    IsRevenueReversal = true,
                    SubTotalAmount = 15000.00m,
                    DiscountTotalAmount = 0.00m,
                    TaxTotalAmount = 2700.00m,
                    RoundOffAmount = 0.00m,
                    GrandTotalAmount = 17700.00m,
                    AppliedToInvoiceAmount = 17700.00m,
                    ReceivableAccountIdSnapshot = ReceivableAccountId,
                    ReceivableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.GSTPayable),
                    ReceivableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.GSTPayable),
                    RevenueReversalAccountId = SalesReturnAccountId,
                    RevenueReversalAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.GSTPayable),
                    RevenueReversalAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.GSTPayable),
                    CreditNoteStatus = CreditNoteStatuses.Posted,
                    PostedOn = DateTime.Today.AddDays(-20),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedByUserName = "Finance Controller",
                    CreatedAt = DateTime.Today.AddDays(-21),
                    CreatedBy = "AR Clerk",
                    Lines = new List<CustomerCreditNoteLineModel>
                    {
                        new CustomerCreditNoteLineModel
                        {
                            Id = Guid.NewGuid(),
                            CustomerCreditNoteId = CreditNote2Id,
                            LineNumber = 10,
                            LineType = LineTypes.Service,
                            Description = "Implementation Services - Rate difference adjustment",
                            Quantity = 1,
                            UnitPrice = 15000.00m,
                            DiscountPercent = 0,
                            DiscountAmount = 0,
                            TaxRatePercent = 18,
                            TaxAmount = 2700.00m,
                            RevenueReversalAccountId = ServiceRevenueAccountId,
                            RevenueReversalAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.GSTPayable),
                            RevenueReversalAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.GSTPayable),
                            CreatedAt = DateTime.Today.AddDays(-21),
                            CreatedBy = "AR Clerk"
                        }
                    }
                },

                // Credit Note 3: Draft credit note - Discount After Invoice
                new CustomerCreditNoteViewModel
                {
                    Id = CreditNote3Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = BangaloreBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    CustomerId = Customer2Id,
                    CustomerCode = "CUST-002",
                    CustomerName = "State Government Education Dept",
                    CreditNoteNumber = "CN-2024-0003",
                    CreditNoteDate = DateTime.Today.AddDays(-5),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    ReferenceText = "DISC-2024-001",
                    CreditNoteNarration = "Volume discount as per negotiated terms",
                    IsAgainstInvoice = true,
                    CustomerInvoiceId = Invoice3Id,
                    CustomerInvoiceNumber = "INV-2024-0003",
                    InvoiceNumberSnapshot = "INV-2024-0003",
                    InvoiceDateSnapshot = DateTime.Today.AddDays(-15),
                    CreditReasonCode = CreditReasonCodes.DiscountAfterInvoice,
                    CreditReasonDescription = "Discount After Invoice - Volume discount",
                    IsTaxImpacting = true,
                    IsRevenueReversal = true,
                    SubTotalAmount = 5000.00m,
                    DiscountTotalAmount = 0.00m,
                    TaxTotalAmount = 900.00m,
                    RoundOffAmount = 0.00m,
                    GrandTotalAmount = 5900.00m,
                    AppliedToInvoiceAmount = 0.00m,
                    CreditNoteStatus = CreditNoteStatuses.Draft,
                    CreatedAt = DateTime.Today.AddDays(-5),
                    CreatedBy = "AR Clerk",
                    Lines = new List<CustomerCreditNoteLineModel>
                    {
                        new CustomerCreditNoteLineModel
                        {
                            Id = Guid.NewGuid(),
                            CustomerCreditNoteId = CreditNote3Id,
                            LineNumber = 10,
                            LineType = LineTypes.Manual,
                            Description = "Volume discount - 5% on total order value",
                            Quantity = 1,
                            UnitPrice = 5000.00m,
                            DiscountPercent = 0,
                            DiscountAmount = 0,
                            TaxRatePercent = 18,
                            TaxAmount = 900.00m,
                            RevenueReversalAccountId = SalesReturnAccountId,
                            RevenueReversalAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.GSTPayable),
                            RevenueReversalAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.GSTPayable),
                            CreatedAt = DateTime.Today.AddDays(-5),
                            CreatedBy = "AR Clerk"
                        }
                    }
                },

                // Credit Note 4: Submitted credit note - Service Cancellation
                new CustomerCreditNoteViewModel
                {
                    Id = CreditNote4Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    CustomerId = Customer3Id,
                    CustomerCode = "CUST-003",
                    CustomerName = "XYZ Electronics Hub",
                    CreditNoteNumber = "CN-2024-0004",
                    CreditNoteDate = DateTime.Today.AddDays(-3),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    ReferenceText = "CANCEL-SVC-001",
                    CreditNoteNarration = "Service cancellation - Training module cancelled",
                    IsAgainstInvoice = false,
                    CreditReasonCode = CreditReasonCodes.ServiceCancellation,
                    CreditReasonDescription = "Service Cancellation - Training module",
                    IsTaxImpacting = true,
                    IsRevenueReversal = true,
                    SubTotalAmount = 25000.00m,
                    DiscountTotalAmount = 0.00m,
                    TaxTotalAmount = 4500.00m,
                    RoundOffAmount = 0.00m,
                    GrandTotalAmount = 29500.00m,
                    AppliedToInvoiceAmount = 0.00m,
                    CreditNoteStatus = CreditNoteStatuses.Submitted,
                    CreatedAt = DateTime.Today.AddDays(-3),
                    CreatedBy = "AR Clerk",
                    Lines = new List<CustomerCreditNoteLineModel>
                    {
                        new CustomerCreditNoteLineModel
                        {
                            Id = Guid.NewGuid(),
                            CustomerCreditNoteId = CreditNote4Id,
                            LineNumber = 10,
                            LineType = LineTypes.Service,
                            Description = "Training Module - Advanced Analytics (Cancelled)",
                            Quantity = 1,
                            UnitPrice = 25000.00m,
                            DiscountPercent = 0,
                            DiscountAmount = 0,
                            TaxRatePercent = 18,
                            TaxAmount = 4500.00m,
                            RevenueReversalAccountId = ServiceRevenueAccountId,
                            RevenueReversalAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.GSTPayable),
                            RevenueReversalAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.GSTPayable),
                            CreatedAt = DateTime.Today.AddDays(-3),
                            CreatedBy = "AR Clerk"
                        }
                    }
                },

                // Credit Note 5: Approved credit note - Tax Correction
                new CustomerCreditNoteViewModel
                {
                    Id = CreditNote5Id,
                    CompanyId = SofaCraftCompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId,
                    BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru),
                    BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    CustomerId = Customer1Id,
                    CustomerCode = "CUST-001",
                    CustomerName = "ABC Traders Private Limited",
                    CreditNoteNumber = "CN-2024-0005",
                    CreditNoteDate = DateTime.Today.AddDays(-1),
                    CurrencyId = InrCurrencyId,
                    CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR),
                    CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR),
                    ExchangeRate = 1,
                    ReferenceText = "TAX-CORR-001",
                    CreditNoteNarration = "Tax correction - Wrong tax rate applied on original invoice",
                    IsAgainstInvoice = true,
                    CustomerInvoiceId = Invoice1Id,
                    CustomerInvoiceNumber = "INV-2024-0001",
                    InvoiceNumberSnapshot = "INV-2024-0001",
                    InvoiceDateSnapshot = DateTime.Today.AddDays(-45),
                    CreditReasonCode = CreditReasonCodes.TaxCorrection,
                    CreditReasonDescription = "Tax Correction - Rate adjustment",
                    IsTaxImpacting = true,
                    IsRevenueReversal = false,
                    SubTotalAmount = 0.00m,
                    DiscountTotalAmount = 0.00m,
                    TaxTotalAmount = 1500.00m,
                    RoundOffAmount = 0.00m,
                    GrandTotalAmount = 1500.00m,
                    AppliedToInvoiceAmount = 0.00m,
                    CreditNoteStatus = CreditNoteStatuses.Approved,
                    CreatedAt = DateTime.Today.AddDays(-1),
                    CreatedBy = "AR Clerk",
                    Lines = new List<CustomerCreditNoteLineModel>
                    {
                        new CustomerCreditNoteLineModel
                        {
                            Id = Guid.NewGuid(),
                            CustomerCreditNoteId = CreditNote5Id,
                            LineNumber = 10,
                            LineType = LineTypes.Manual,
                            Description = "Tax difference - 18% vs 12% rate correction",
                            Quantity = 1,
                            UnitPrice = 0.00m,
                            DiscountPercent = 0,
                            DiscountAmount = 0,
                            TaxRatePercent = 0,
                            TaxAmount = 1500.00m,
                            RevenueReversalAccountId = TaxAccountId,
                            RevenueReversalAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.GSTPayable),
                            RevenueReversalAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.GSTPayable),
                            CreatedAt = DateTime.Today.AddDays(-1),
                            CreatedBy = "AR Clerk"
                        }
                    }
                },

                // Credit Note 6: Posted - Volume discount
                new CustomerCreditNoteViewModel
                {
                    Id = CreditNote6Id, CompanyId = SofaCraftCompanyId, CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId, BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru), BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    CustomerId = Customer2Id, CustomerCode = "CUST-002", CustomerName = "State Government Education Dept",
                    CreditNoteNumber = "CN-2024-0006", CreditNoteDate = DateTime.Today.AddDays(-22),
                    PostingDate = DateTime.Today.AddDays(-21),
                    CurrencyId = InrCurrencyId, CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR), CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR), ExchangeRate = 1,
                    CreditNoteNarration = "Annual volume discount credit",
                    CreditReasonCode = CreditReasonCodes.DiscountAfterInvoice,
                    IsAgainstInvoice = true, CustomerInvoiceId = Invoice2Id, CustomerInvoiceNumber = "INV-2024-0002",
                    SubTotalAmount = 18000.00m, TaxTotalAmount = 3240.00m, GrandTotalAmount = 21240.00m,
                    AppliedToInvoiceAmount = 21240.00m, IsTaxImpacting = true,
                    CreditNoteStatus = CreditNoteStatuses.Posted,
                    PostedOn = DateTime.Today.AddDays(-21), PostedByUserName = "Finance Controller",
                    CreatedAt = DateTime.Today.AddDays(-22), CreatedBy = "AR Clerk"
                },

                // Credit Note 7: Posted - Service credit
                new CustomerCreditNoteViewModel
                {
                    Id = CreditNote7Id, CompanyId = SofaCraftCompanyId, CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = BangaloreBranchId, BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru), BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    CustomerId = Customer3Id, CustomerCode = "CUST-003", CustomerName = "XYZ Electronics Hub",
                    CreditNoteNumber = "CN-2024-0007", CreditNoteDate = DateTime.Today.AddDays(-15),
                    PostingDate = DateTime.Today.AddDays(-14),
                    CurrencyId = InrCurrencyId, CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR), CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR), ExchangeRate = 1,
                    CreditNoteNarration = "SLA penalty credit for service downtime",
                    CreditReasonCode = CreditReasonCodes.SalesReturn,
                    IsAgainstInvoice = false,
                    SubTotalAmount = 25000.00m, TaxTotalAmount = 4500.00m, GrandTotalAmount = 29500.00m,
                    AppliedToInvoiceAmount = 0.00m, IsTaxImpacting = true,
                    CreditNoteStatus = CreditNoteStatuses.Posted,
                    PostedOn = DateTime.Today.AddDays(-14), PostedByUserName = "Finance Controller",
                    CreatedAt = DateTime.Today.AddDays(-15), CreatedBy = "AR Clerk"
                },

                // Credit Note 8: Submitted
                new CustomerCreditNoteViewModel
                {
                    Id = CreditNote8Id, CompanyId = SofaCraftCompanyId, CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId, BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru), BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    CustomerId = Customer1Id, CustomerCode = "CUST-001", CustomerName = "ABC Traders Private Limited",
                    CreditNoteNumber = "CN-2024-0008", CreditNoteDate = DateTime.Today.AddDays(-5),
                    CurrencyId = InrCurrencyId, CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR), CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR), ExchangeRate = 1,
                    CreditNoteNarration = "Pricing correction for overcharge",
                    CreditReasonCode = CreditReasonCodes.PriceCorrection,
                    IsAgainstInvoice = true, CustomerInvoiceId = Invoice1Id, CustomerInvoiceNumber = "INV-2024-0001",
                    SubTotalAmount = 12000.00m, TaxTotalAmount = 2160.00m, GrandTotalAmount = 14160.00m,
                    AppliedToInvoiceAmount = 0.00m, IsTaxImpacting = true,
                    CreditNoteStatus = CreditNoteStatuses.Submitted,
                    CreatedAt = DateTime.Today.AddDays(-5), CreatedBy = "AR Clerk"
                },

                // Credit Note 9: Draft
                new CustomerCreditNoteViewModel
                {
                    Id = CreditNote9Id, CompanyId = SofaCraftCompanyId, CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId, BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru), BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    CustomerId = Customer2Id, CustomerCode = "CUST-002", CustomerName = "State Government Education Dept",
                    CreditNoteNumber = "CN-2024-0009", CreditNoteDate = DateTime.Today.AddDays(-2),
                    CurrencyId = InrCurrencyId, CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR), CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR), ExchangeRate = 1,
                    CreditNoteNarration = "Product return credit pending verification",
                    CreditReasonCode = CreditReasonCodes.SalesReturn,
                    IsAgainstInvoice = false,
                    SubTotalAmount = 8500.00m, TaxTotalAmount = 1530.00m, GrandTotalAmount = 10030.00m,
                    AppliedToInvoiceAmount = 0.00m, IsTaxImpacting = true,
                    CreditNoteStatus = CreditNoteStatuses.Draft,
                    CreatedAt = DateTime.Today.AddDays(-2), CreatedBy = "AR Clerk"
                },

                // Credit Note 10: Posted - Early payment discount
                new CustomerCreditNoteViewModel
                {
                    Id = CreditNote10Id, CompanyId = SofaCraftCompanyId, CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId, BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru), BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    CustomerId = Customer3Id, CustomerCode = "CUST-003", CustomerName = "XYZ Electronics Hub",
                    CreditNoteNumber = "CN-2024-0010", CreditNoteDate = DateTime.Today.AddDays(-35),
                    PostingDate = DateTime.Today.AddDays(-34),
                    CurrencyId = InrCurrencyId, CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR), CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR), ExchangeRate = 1,
                    CreditNoteNarration = "Early payment discount - 2%",
                    CreditReasonCode = CreditReasonCodes.DiscountAfterInvoice,
                    IsAgainstInvoice = true, CustomerInvoiceId = Invoice3Id, CustomerInvoiceNumber = "INV-2024-0003",
                    SubTotalAmount = 7000.00m, TaxTotalAmount = 1260.00m, GrandTotalAmount = 8260.00m,
                    AppliedToInvoiceAmount = 8260.00m, IsTaxImpacting = true,
                    CreditNoteStatus = CreditNoteStatuses.Posted,
                    PostedOn = DateTime.Today.AddDays(-34), PostedByUserName = "Finance Controller",
                    CreatedAt = DateTime.Today.AddDays(-35), CreatedBy = "Finance Manager"
                },

                // Credit Note 11: Approved
                new CustomerCreditNoteViewModel
                {
                    Id = CreditNote11Id, CompanyId = SofaCraftCompanyId, CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = BangaloreBranchId, BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru), BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    CustomerId = Customer1Id, CustomerCode = "CUST-001", CustomerName = "ABC Traders Private Limited",
                    CreditNoteNumber = "CN-2024-0011", CreditNoteDate = DateTime.Today.AddDays(-8),
                    CurrencyId = InrCurrencyId, CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR), CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR), ExchangeRate = 1,
                    CreditNoteNarration = "Warranty claim credit",
                    CreditReasonCode = CreditReasonCodes.SalesReturn,
                    IsAgainstInvoice = true, CustomerInvoiceId = Invoice1Id, CustomerInvoiceNumber = "INV-2024-0001",
                    SubTotalAmount = 15000.00m, TaxTotalAmount = 2700.00m, GrandTotalAmount = 17700.00m,
                    AppliedToInvoiceAmount = 0.00m, IsTaxImpacting = true,
                    CreditNoteStatus = CreditNoteStatuses.Approved,
                    ApprovedOn = DateTime.Today.AddDays(-7), ApprovedByUserName = "Finance Manager",
                    CreatedAt = DateTime.Today.AddDays(-8), CreatedBy = "AR Clerk"
                },

                // Credit Note 12: Cancelled
                new CustomerCreditNoteViewModel
                {
                    Id = CreditNote12Id, CompanyId = SofaCraftCompanyId, CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = ChennaiHQBranchId, BranchCode = SeedLookup.BranchCode(MasterDataIds.Branches.SofaCraftBengaluru), BranchName = SeedLookup.BranchName(MasterDataIds.Branches.SofaCraftBengaluru),
                    CustomerId = Customer2Id, CustomerCode = "CUST-002", CustomerName = "State Government Education Dept",
                    CreditNoteNumber = "CN-2024-0012", CreditNoteDate = DateTime.Today.AddDays(-28),
                    CurrencyId = InrCurrencyId, CurrencyCode = SeedLookup.CurrencyCode(MasterDataIds.Currencies.INR), CurrencyName = SeedLookup.CurrencyName(MasterDataIds.Currencies.INR), ExchangeRate = 1,
                    CreditNoteNarration = "Cancelled - Duplicate entry",
                    CreditReasonCode = CreditReasonCodes.PriceCorrection,
                    IsAgainstInvoice = false,
                    SubTotalAmount = 5000.00m, TaxTotalAmount = 900.00m, GrandTotalAmount = 5900.00m,
                    AppliedToInvoiceAmount = 0.00m, IsTaxImpacting = true,
                    CreditNoteStatus = CreditNoteStatuses.Cancelled,
                    CancellationReason = "Duplicate credit note",
                    CreatedAt = DateTime.Today.AddDays(-28), CreatedBy = "AR Clerk"
                }
            };

            return creditNotes;
        }
    }
}
