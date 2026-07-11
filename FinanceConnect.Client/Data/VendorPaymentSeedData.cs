using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Data
{
    /// <summary>
    /// Seed data for VendorPayment model (Model #38)
    /// Provides demo data for AP vendor payments
    /// </summary>
    public static class VendorPaymentSeedData
    {
        // Fixed GUIDs for consistency (matching existing seed data)
        private static readonly Guid TenantId = MasterDataIds.Tenants.Default;
        private static readonly Guid CompanyId = MasterDataIds.Companies.SofaCraft;
        
        // Branch GUIDs (matching BranchServiceData)
        private static readonly Guid BranchId1 = MasterDataIds.Branches.SofaCraftHQ;
        private static readonly Guid BranchId2 = MasterDataIds.Branches.SofaCraftBengaluru;

        // Currency GUIDs
        private static readonly Guid CurrencyINR = MasterDataIds.Currencies.INR;

        // Vendor IDs (from VendorBillSeedData)
        private static readonly Guid VendorTCS = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000001");
        private static readonly Guid VendorInfosys = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000002");
        private static readonly Guid VendorWipro = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000003");
        private static readonly Guid VendorHCL = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000004");
        private static readonly Guid VendorTechM = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000005");

        // Bill IDs (from VendorBillSeedData)
        private static readonly Guid Bill1Id = Guid.Parse("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e001");
        private static readonly Guid Bill2Id = Guid.Parse("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e002");
        private static readonly Guid Bill5Id = Guid.Parse("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e005");

        // GL Account GUIDs
        private static readonly Guid PayableAccountId = Guid.Parse("a0000005-0005-0005-0005-000000000050");
        private static readonly Guid AdvanceAccountId = MasterDataIds.Accounts.AccountsReceivable;
        private static readonly Guid BankAccountId = MasterDataIds.Accounts.HDFCBankAccount;
        private static readonly Guid CashAccountId = MasterDataIds.Accounts.PettyCash;
        private static readonly Guid TDSPayableAccountId = MasterDataIds.Accounts.TDSPayable;

        // Predefined Payment GUIDs
        public static readonly Guid Payment1Id = Guid.Parse("f3f3f3f3-0001-0001-0001-000000000001");
        public static readonly Guid Payment2Id = Guid.Parse("f3f3f3f3-0001-0001-0001-000000000002");
        public static readonly Guid Payment3Id = Guid.Parse("f3f3f3f3-0001-0001-0001-000000000003");
        public static readonly Guid Payment4Id = Guid.Parse("f3f3f3f3-0001-0001-0001-000000000004");
        public static readonly Guid Payment5Id = Guid.Parse("f3f3f3f3-0001-0001-0001-000000000005");
        public static readonly Guid Payment6Id = Guid.Parse("f3f3f3f3-0001-0001-0001-000000000006");
        public static readonly Guid Payment7Id = Guid.Parse("f3f3f3f3-0001-0001-0001-000000000007");

        public static readonly Guid Payment8Id = Guid.Parse("f3f3f3f3-0001-0001-0001-000000000008");
        public static readonly Guid Payment9Id = Guid.Parse("f3f3f3f3-0001-0001-0001-000000000009");
        public static readonly Guid Payment10Id = Guid.Parse("f3f3f3f3-0001-0001-0001-000000000010");
        public static readonly Guid Payment11Id = Guid.Parse("f3f3f3f3-0001-0001-0001-000000000011");
        public static readonly Guid Payment12Id = Guid.Parse("f3f3f3f3-0001-0001-0001-000000000012");

        public static List<VendorPaymentViewModel> GetSeedPayments()
        {
            var today = DateTime.Today;
            var payments = new List<VendorPaymentViewModel>
            {
                // Payment 1: Posted - Full payment via Bank Transfer (NEFT) for TCS bill
                new VendorPaymentViewModel
                {
                    Id = Payment1Id,
                    TenantId = TenantId,
                    CompanyId = CompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = BranchId1,
                    BranchCode = "HO",
                    BranchName = "SofaCraft Head Office & Factory - Chennai",
                    VendorId = VendorTCS,
                    VendorCode = "VND-000001",
                    VendorName = "Tech Components India Pvt Ltd",
                    PaymentNumber = "APP-2025-0001",
                    PaymentDate = today.AddDays(-25),
                    PostingDate = today.AddDays(-25),
                    CurrencyId = CurrencyINR,
                    CurrencyCode = "INR",
                    CurrencyName = "Indian Rupee",
                    ExchangeRate = 1,
                    PaymentNarration = "Payment for Software License Bill APB-2025-0001 via NEFT",
                    PaymentMethod = VendorPaymentMethods.BankTransfer,
                    PaymentAccountId = BankAccountId,
                    PaymentAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    PaymentAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    PaymentReferenceNumber = "HDFC25010000012345",
                    ReferenceDate = today.AddDays(-25),
                    BankNameSnapshot = "HDFC Bank",
                    InstrumentStatus = VendorInstrumentStatuses.Completed,
                    PaymentGrossAmount = 590000.00m,
                    AllocatedAmount = 590000.00m,
                    UnallocatedAdvanceAmount = 0.00m,
                    NetBankOutflowAmount = 590000.00m,
                    IsTDSApplicable = false,
                    TDSWithheldAmount = 0,
                    PaymentStatus = VendorPaymentStatuses.Posted,
                    PostedOn = today.AddDays(-25),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedBy = "Finance Controller",
                    PayableAccountIdSnapshot = PayableAccountId,
                    PayableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    PayableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    CreatedAt = today.AddDays(-26),
                    CreatedBy = "AP Clerk",
                    Allocations = new List<VendorPaymentAllocationModel>
                    {
                        new VendorPaymentAllocationModel
                        {
                            Id = Guid.NewGuid(),
                            VendorPaymentId = Payment1Id,
                            VendorBillId = Bill1Id,
                            BillNumberSnapshot = "APB-2025-0001",
                            BillDateSnapshot = today.AddDays(-28),
                            BillDueDateSnapshot = today.AddDays(2),
                            BillOutstandingSnapshot = 590000.00m,
                            AllocatedToBillAmount = 590000.00m,
                            AllocationOrder = 1,
                            CreatedAt = today.AddDays(-26),
                            CreatedBy = "AP Clerk"
                        }
                    }
                },

                // Payment 2: Posted - Partial payment with TDS withheld for Infosys bill
                new VendorPaymentViewModel
                {
                    Id = Payment2Id,
                    TenantId = TenantId,
                    CompanyId = CompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = BranchId1,
                    BranchCode = "HO",
                    BranchName = "SofaCraft Head Office & Factory - Chennai",
                    VendorId = VendorInfosys,
                    VendorCode = "VND-000002",
                    VendorName = "CloudTech Solutions",
                    PaymentNumber = "APP-2025-0002",
                    PaymentDate = today.AddDays(-18),
                    PostingDate = today.AddDays(-18),
                    CurrencyId = CurrencyINR,
                    CurrencyCode = "INR",
                    CurrencyName = "Indian Rupee",
                    ExchangeRate = 1,
                    PaymentNarration = "Partial payment for IT Services with TDS @10%",
                    PaymentMethod = VendorPaymentMethods.BankTransfer,
                    PaymentAccountId = BankAccountId,
                    PaymentAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    PaymentAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    PaymentReferenceNumber = "ICICI25010000098765",
                    ReferenceDate = today.AddDays(-18),
                    BankNameSnapshot = "ICICI Bank",
                    InstrumentStatus = VendorInstrumentStatuses.Completed,
                    PaymentGrossAmount = 150000.00m,
                    AllocatedAmount = 150000.00m,
                    UnallocatedAdvanceAmount = 0.00m,
                    IsTDSApplicable = true,
                    TDSSectionCodeSnapshot = "194J",
                    TDSRatePercentSnapshot = 10.00m,
                    TDSBaseAmount = 150000.00m,
                    TDSWithheldAmount = 15000.00m,
                    TDSGLAccountId = TDSPayableAccountId,
                    TDSGLAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    TDSGLAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    NetBankOutflowAmount = 135000.00m, // 150000 - 15000 TDS
                    PaymentStatus = VendorPaymentStatuses.Posted,
                    PostedOn = today.AddDays(-18),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedBy = "Finance Controller",
                    PayableAccountIdSnapshot = PayableAccountId,
                    PayableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    PayableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    CreatedAt = today.AddDays(-19),
                    CreatedBy = "AP Clerk",
                    Allocations = new List<VendorPaymentAllocationModel>
                    {
                        new VendorPaymentAllocationModel
                        {
                            Id = Guid.NewGuid(),
                            VendorPaymentId = Payment2Id,
                            VendorBillId = Bill2Id,
                            BillNumberSnapshot = "APB-2025-0002",
                            BillDateSnapshot = today.AddDays(-18),
                            BillDueDateSnapshot = today.AddDays(12),
                            BillOutstandingSnapshot = 295000.00m,
                            AllocatedToBillAmount = 150000.00m,
                            AllocationOrder = 1,
                            CreatedAt = today.AddDays(-19),
                            CreatedBy = "AP Clerk"
                        }
                    }
                },

                // Payment 3: Posted - Advance Cash Payment (no bill allocation)
                new VendorPaymentViewModel
                {
                    Id = Payment3Id,
                    TenantId = TenantId,
                    CompanyId = CompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = BranchId2,
                    BranchCode = "BLR",
                    BranchName = "SofaCraft Experience Store - Bengaluru",
                    VendorId = VendorWipro,
                    VendorCode = "VND-000003",
                    VendorName = "Reliable Supplies Co",
                    PaymentNumber = "APP-2025-0003",
                    PaymentDate = today.AddDays(-15),
                    PostingDate = today.AddDays(-15),
                    CurrencyId = CurrencyINR,
                    CurrencyCode = "INR",
                    CurrencyName = "Indian Rupee",
                    ExchangeRate = 1,
                    PaymentNarration = "Advance payment for upcoming office rent",
                    PaymentMethod = VendorPaymentMethods.Cash,
                    PaymentAccountId = CashAccountId,
                    PaymentAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    PaymentAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    InstrumentStatus = VendorInstrumentStatuses.Completed,
                    PaymentGrossAmount = 50000.00m,
                    AllocatedAmount = 0.00m,
                    UnallocatedAdvanceAmount = 50000.00m,
                    NetBankOutflowAmount = 50000.00m,
                    IsTDSApplicable = false,
                    PaymentStatus = VendorPaymentStatuses.Posted,
                    PostedOn = today.AddDays(-15),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedBy = "Finance Controller",
                    AdvanceToVendorAccountIdSnapshot = AdvanceAccountId,
                    AdvanceToVendorAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    AdvanceToVendorAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    CreatedAt = today.AddDays(-15),
                    CreatedBy = "AP Clerk",
                    Allocations = new List<VendorPaymentAllocationModel>()
                },

                // Payment 4: Posted - UPI Payment
                new VendorPaymentViewModel
                {
                    Id = Payment4Id,
                    TenantId = TenantId,
                    CompanyId = CompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = BranchId1,
                    BranchCode = "HO",
                    BranchName = "SofaCraft Head Office & Factory - Chennai",
                    VendorId = VendorHCL,
                    VendorCode = "VND-000004",
                    VendorName = "BuildRight Constructions",
                    PaymentNumber = "APP-2025-0004",
                    PaymentDate = today.AddDays(-10),
                    PostingDate = today.AddDays(-10),
                    CurrencyId = CurrencyINR,
                    CurrencyCode = "INR",
                    CurrencyName = "Indian Rupee",
                    ExchangeRate = 1,
                    PaymentNarration = "UPI payment for utility services",
                    PaymentMethod = VendorPaymentMethods.UPI,
                    PaymentAccountId = BankAccountId,
                    PaymentAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    PaymentAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    PaymentReferenceNumber = "UPI/250115/123456789012",
                    ReferenceDate = today.AddDays(-10),
                    InstrumentStatus = VendorInstrumentStatuses.Completed,
                    PaymentGrossAmount = 75000.00m,
                    AllocatedAmount = 75000.00m,
                    UnallocatedAdvanceAmount = 0.00m,
                    NetBankOutflowAmount = 75000.00m,
                    IsTDSApplicable = false,
                    PaymentStatus = VendorPaymentStatuses.Posted,
                    PostedOn = today.AddDays(-10),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedBy = "Finance Controller",
                    PayableAccountIdSnapshot = PayableAccountId,
                    PayableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    PayableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    CreatedAt = today.AddDays(-10),
                    CreatedBy = "AP Clerk",
                    Allocations = new List<VendorPaymentAllocationModel>()
                },

                // Payment 5: Draft - Bank transfer pending verification
                new VendorPaymentViewModel
                {
                    Id = Payment5Id,
                    TenantId = TenantId,
                    CompanyId = CompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = BranchId1,
                    BranchCode = "HO",
                    BranchName = "SofaCraft Head Office & Factory - Chennai",
                    VendorId = VendorTechM,
                    VendorCode = "VND-000005",
                    VendorName = "Tamil Nadu Electricity Board",
                    PaymentNumber = "APP-2025-0005",
                    PaymentDate = today.AddDays(-5),
                    CurrencyId = CurrencyINR,
                    CurrencyCode = "INR",
                    CurrencyName = "Indian Rupee",
                    ExchangeRate = 1,
                    PaymentNarration = "Bank transfer for contractor services - pending verification",
                    PaymentMethod = VendorPaymentMethods.BankTransfer,
                    PaymentAccountId = BankAccountId,
                    PaymentAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    PaymentAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    PaymentReferenceNumber = "SBI25010000054321",
                    ReferenceDate = today.AddDays(-5),
                    BankNameSnapshot = "State Bank of India",
                    InstrumentStatus = VendorInstrumentStatuses.Initiated,
                    PaymentGrossAmount = 85000.00m,
                    AllocatedAmount = 85000.00m,
                    UnallocatedAdvanceAmount = 0.00m,
                    NetBankOutflowAmount = 85000.00m,
                    IsTDSApplicable = false,
                    PaymentStatus = VendorPaymentStatuses.Draft,
                    CreatedAt = today.AddDays(-5),
                    CreatedBy = "AP Clerk",
                    Allocations = new List<VendorPaymentAllocationModel>
                    {
                        new VendorPaymentAllocationModel
                        {
                            Id = Guid.NewGuid(),
                            VendorPaymentId = Payment5Id,
                            VendorBillId = Bill5Id,
                            BillNumberSnapshot = "APB-2025-0005",
                            BillDateSnapshot = today.AddDays(-14),
                            BillDueDateSnapshot = today.AddDays(16),
                            BillOutstandingSnapshot = 85000.00m,
                            AllocatedToBillAmount = 85000.00m,
                            AllocationOrder = 1,
                            CreatedAt = today.AddDays(-5),
                            CreatedBy = "AP Clerk"
                        }
                    }
                },

                // Payment 6: Approved - Awaiting posting
                new VendorPaymentViewModel
                {
                    Id = Payment6Id,
                    TenantId = TenantId,
                    CompanyId = CompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = BranchId2,
                    BranchCode = "BLR",
                    BranchName = "SofaCraft Experience Store - Bengaluru",
                    VendorId = VendorInfosys,
                    VendorCode = "VND-000002",
                    VendorName = "CloudTech Solutions",
                    PaymentNumber = "APP-2025-0006",
                    PaymentDate = today.AddDays(-3),
                    CurrencyId = CurrencyINR,
                    CurrencyCode = "INR",
                    CurrencyName = "Indian Rupee",
                    ExchangeRate = 1,
                    PaymentNarration = "Balance payment for IT consulting - approved, pending posting",
                    PaymentMethod = VendorPaymentMethods.Cheque,
                    PaymentAccountId = BankAccountId,
                    PaymentAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    PaymentAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    PaymentReferenceNumber = "CHQ-789012",
                    ReferenceDate = today.AddDays(-3),
                    BankNameSnapshot = "HDFC Bank",
                    InstrumentStatus = VendorInstrumentStatuses.Initiated,
                    PaymentGrossAmount = 145000.00m,
                    AllocatedAmount = 145000.00m,
                    UnallocatedAdvanceAmount = 0.00m,
                    IsTDSApplicable = true,
                    TDSSectionCodeSnapshot = "194J",
                    TDSRatePercentSnapshot = 10.00m,
                    TDSBaseAmount = 145000.00m,
                    TDSWithheldAmount = 14500.00m,
                    TDSGLAccountId = TDSPayableAccountId,
                    TDSGLAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    TDSGLAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    NetBankOutflowAmount = 130500.00m, // 145000 - 14500 TDS
                    PaymentStatus = VendorPaymentStatuses.Approved,
                    SubmittedOn = today.AddDays(-3),
                    SubmittedByUserId = MasterDataIds.Tenants.Default,
                    SubmittedBy = "AP Clerk",
                    ApprovedOn = today.AddDays(-2),
                    ApprovedByUserId = MasterDataIds.Tenants.Default,
                    ApprovedBy = "Finance Manager",
                    CreatedAt = today.AddDays(-4),
                    CreatedBy = "AP Clerk",
                    Allocations = new List<VendorPaymentAllocationModel>
                    {
                        new VendorPaymentAllocationModel
                        {
                            Id = Guid.NewGuid(),
                            VendorPaymentId = Payment6Id,
                            VendorBillId = Bill2Id,
                            BillNumberSnapshot = "APB-2025-0002",
                            BillDateSnapshot = today.AddDays(-18),
                            BillDueDateSnapshot = today.AddDays(12),
                            BillOutstandingSnapshot = 145000.00m,
                            AllocatedToBillAmount = 145000.00m,
                            AllocationOrder = 1,
                            CreatedAt = today.AddDays(-4),
                            CreatedBy = "AP Clerk"
                        }
                    }
                },

                // Payment 7: Reversed - Cheque bounced
                new VendorPaymentViewModel
                {
                    Id = Payment7Id,
                    TenantId = TenantId,
                    CompanyId = CompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = BranchId1,
                    BranchCode = "HO",
                    BranchName = "SofaCraft Head Office & Factory - Chennai",
                    VendorId = VendorTCS,
                    VendorCode = "VND-000001",
                    VendorName = "Tech Components India Pvt Ltd",
                    PaymentNumber = "APP-2025-0007",
                    PaymentDate = today.AddDays(-30),
                    PostingDate = today.AddDays(-30),
                    CurrencyId = CurrencyINR,
                    CurrencyCode = "INR",
                    CurrencyName = "Indian Rupee",
                    ExchangeRate = 1,
                    PaymentNarration = "Cheque payment - REVERSED due to bank rejection",
                    PaymentMethod = VendorPaymentMethods.Cheque,
                    PaymentAccountId = BankAccountId,
                    PaymentAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    PaymentAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    PaymentReferenceNumber = "CHQ-456789",
                    ReferenceDate = today.AddDays(-30),
                    BankNameSnapshot = "Axis Bank",
                    InstrumentStatus = VendorInstrumentStatuses.Reversed,
                    PaymentGrossAmount = 125000.00m,
                    AllocatedAmount = 125000.00m,
                    UnallocatedAdvanceAmount = 0.00m,
                    NetBankOutflowAmount = 125000.00m,
                    IsTDSApplicable = false,
                    PaymentStatus = VendorPaymentStatuses.Reversed,
                    PostedOn = today.AddDays(-30),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedBy = "Finance Controller",
                    ReversedOn = today.AddDays(-25),
                    ReversedByUserId = MasterDataIds.Tenants.Default,
                    ReversedBy = "Finance Controller",
                    ReversalReason = "Cheque bounced - Bank rejected due to signature mismatch",
                    ReversalReference = "BNC-2025-001",
                    PayableAccountIdSnapshot = PayableAccountId,
                    PayableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    PayableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    CreatedAt = today.AddDays(-31),
                    CreatedBy = "AP Clerk",
                    Allocations = new List<VendorPaymentAllocationModel>
                    {
                        new VendorPaymentAllocationModel
                        {
                            Id = Guid.NewGuid(),
                            VendorPaymentId = Payment7Id,
                            VendorBillId = Bill1Id,
                            BillNumberSnapshot = "APB-2025-0001",
                            BillDateSnapshot = today.AddDays(-28),
                            BillDueDateSnapshot = today.AddDays(2),
                            BillOutstandingSnapshot = 125000.00m,
                            AllocatedToBillAmount = 125000.00m,
                            AllocationOrder = 1,
                            CreatedAt = today.AddDays(-31),
                            CreatedBy = "AP Clerk"
                        }
                    }
                },

                // Payment 8: Posted - Payment to DataCenter Solutions
                new VendorPaymentViewModel
                {
                    Id = Payment8Id,
                    TenantId = TenantId,
                    CompanyId = CompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = BranchId1,
                    BranchCode = "HO",
                    BranchName = "SofaCraft Head Office & Factory - Chennai",
                    VendorId = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000009"),
                    VendorCode = "VND-000009",
                    VendorName = "DataCenter Solutions Ltd",
                    PaymentNumber = "APP-2025-0008",
                    PaymentDate = today.AddDays(-15),
                    PostingDate = today.AddDays(-15),
                    CurrencyId = CurrencyINR,
                    CurrencyCode = "INR",
                    CurrencyName = "Indian Rupee",
                    ExchangeRate = 1,
                    PaymentMethod = VendorPaymentMethods.BankTransfer,
                    PaymentReferenceNumber = "NEFT-DC-202502001",
                    PaymentNarration = "Payment for hosting services",
                    PaymentGrossAmount = 200000.00m,
                    AllocatedAmount = 200000.00m,
                    UnallocatedAdvanceAmount = 0,
                    PaymentStatus = VendorPaymentStatuses.Posted,
                    PostedOn = today.AddDays(-15),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedBy = "Finance Controller",
                    PaymentAccountId = BankAccountId,
                    PaymentAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    PaymentAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    PayableAccountIdSnapshot = PayableAccountId,
                    PayableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    PayableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    CreatedAt = today.AddDays(-15),
                    CreatedBy = "AP Clerk"
                },

                // Payment 9: Submitted - Payment to Office Essentials
                new VendorPaymentViewModel
                {
                    Id = Payment9Id,
                    TenantId = TenantId,
                    CompanyId = CompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = BranchId1,
                    BranchCode = "HO",
                    BranchName = "SofaCraft Head Office & Factory - Chennai",
                    VendorId = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000010"),
                    VendorCode = "VND-000010",
                    VendorName = "CloudTech Solutions",
                    PaymentNumber = "APP-2025-0009",
                    PaymentDate = today.AddDays(-5),
                    CurrencyId = CurrencyINR,
                    CurrencyCode = "INR",
                    CurrencyName = "Indian Rupee",
                    ExchangeRate = 1,
                    PaymentMethod = VendorPaymentMethods.BankTransfer,
                    PaymentReferenceNumber = "NEFT-OE-202502001",
                    PaymentNarration = "Payment for office supplies",
                    PaymentGrossAmount = 47500.00m,
                    AllocatedAmount = 47500.00m,
                    UnallocatedAdvanceAmount = 0,
                    PaymentStatus = VendorPaymentStatuses.Submitted,
                    PaymentAccountId = BankAccountId,
                    PaymentAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    PaymentAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    PayableAccountIdSnapshot = PayableAccountId,
                    PayableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    PayableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    CreatedAt = today.AddDays(-5),
                    CreatedBy = "AP Clerk"
                },

                // Payment 10: Draft - Payment to SecureGuard Services
                new VendorPaymentViewModel
                {
                    Id = Payment10Id,
                    TenantId = TenantId,
                    CompanyId = CompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = BranchId1,
                    BranchCode = "HO",
                    BranchName = "SofaCraft Head Office & Factory - Chennai",
                    VendorId = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000011"),
                    VendorCode = "VND-000011",
                    VendorName = "SecureGuard Services Pvt Ltd",
                    PaymentNumber = "APP-2025-0010",
                    PaymentDate = today.AddDays(-2),
                    CurrencyId = CurrencyINR,
                    CurrencyCode = "INR",
                    CurrencyName = "Indian Rupee",
                    ExchangeRate = 1,
                    PaymentMethod = VendorPaymentMethods.BankTransfer,
                    PaymentNarration = "Security services monthly payment",
                    PaymentGrossAmount = 65000.00m,
                    AllocatedAmount = 0,
                    UnallocatedAdvanceAmount = 65000.00m,
                    PaymentStatus = VendorPaymentStatuses.Draft,
                    PaymentAccountId = BankAccountId,
                    PaymentAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    PaymentAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    PayableAccountIdSnapshot = PayableAccountId,
                    PayableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    PayableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    CreatedAt = today.AddDays(-2),
                    CreatedBy = "AP Clerk"
                },

                // Payment 11: Posted - Advance payment to Tech Components
                new VendorPaymentViewModel
                {
                    Id = Payment11Id,
                    TenantId = TenantId,
                    CompanyId = CompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = BranchId2,
                    BranchCode = "BLR",
                    BranchName = "SofaCraft Experience Store - Bengaluru",
                    VendorId = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000001"),
                    VendorCode = "VND-000001",
                    VendorName = "Tech Components India Pvt Ltd",
                    PaymentNumber = "APP-2025-0011",
                    PaymentDate = today.AddDays(-22),
                    PostingDate = today.AddDays(-22),
                    CurrencyId = CurrencyINR,
                    CurrencyCode = "INR",
                    CurrencyName = "Indian Rupee",
                    ExchangeRate = 1,
                    PaymentMethod = VendorPaymentMethods.BankTransfer,
                    PaymentReferenceNumber = "RTGS-TC-202501001",
                    PaymentNarration = "Advance payment for upcoming order",
                    PaymentGrossAmount = 150000.00m,
                    AllocatedAmount = 0,
                    UnallocatedAdvanceAmount = 150000.00m,
                    PaymentStatus = VendorPaymentStatuses.Posted,
                    PostedOn = today.AddDays(-22),
                    PostedByUserId = MasterDataIds.Tenants.Default,
                    PostedBy = "Finance Controller",
                    PaymentAccountId = BankAccountId,
                    PaymentAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    PaymentAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    PayableAccountIdSnapshot = PayableAccountId,
                    PayableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    PayableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    AdvanceToVendorAccountIdSnapshot = AdvanceAccountId,
                    AdvanceToVendorAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    AdvanceToVendorAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    CreatedAt = today.AddDays(-22),
                    CreatedBy = "Finance Manager"
                },

                // Payment 12: Cancelled - Cancelled payment
                new VendorPaymentViewModel
                {
                    Id = Payment12Id,
                    TenantId = TenantId,
                    CompanyId = CompanyId,
                    CompanyName = SeedLookup.CompanyName(MasterDataIds.Companies.SofaCraft),
                    BranchId = BranchId1,
                    BranchCode = "HO",
                    BranchName = "SofaCraft Head Office & Factory - Chennai",
                    VendorId = Guid.Parse("e5e5e5e5-0001-0001-0001-000000000003"),
                    VendorCode = "VND-000003",
                    VendorName = "Reliable Supplies Co",
                    PaymentNumber = "APP-2025-0012",
                    PaymentDate = today.AddDays(-18),
                    CurrencyId = CurrencyINR,
                    CurrencyCode = "INR",
                    CurrencyName = "Indian Rupee",
                    ExchangeRate = 1,
                    PaymentMethod = VendorPaymentMethods.Cheque,
                    PaymentReferenceNumber = "CHQ-789012",
                    PaymentNarration = "Cancelled - Incorrect amount",
                    PaymentGrossAmount = 50000.00m,
                    AllocatedAmount = 0,
                    UnallocatedAdvanceAmount = 0,
                    PaymentStatus = VendorPaymentStatuses.Cancelled,
                    CancellationReason = "Incorrect payment amount - needs revision",
                    PaymentAccountId = BankAccountId,
                    PaymentAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    PaymentAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    PayableAccountIdSnapshot = PayableAccountId,
                    PayableAccountCode = SeedLookup.AccountCode(MasterDataIds.Accounts.TDSPayable),
                    PayableAccountName = SeedLookup.AccountName(MasterDataIds.Accounts.TDSPayable),
                    CreatedAt = today.AddDays(-18),
                    CreatedBy = "AP Clerk"
                }
            };

            return payments;
        }
    }
}
