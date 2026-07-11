using static FinanceConnect.Client.ViewModels.TaxSettlementViewModel;

namespace FinanceConnect.Client.Data
{
    public static class TaxSettlementSeedData
    {
        private static readonly Guid Co = Guid.Parse("10000000-0000-0000-0000-000000000001");
        private static readonly Guid Br = Guid.Parse("20000000-0000-0000-0000-000000000001");
        private static readonly Guid Bk = Guid.Parse("B0000000-0000-0000-0000-000000000001");

        public static List<TaxSettlementModel> Get()
        {
            return new List<TaxSettlementModel>
            {
                Build("70000000-0000-0000-0000-000000000001","TAXSET-2026-00001","Posted","GSTCashPayment",new DateTime(2026,2,18),"2026-01","GST","GST","Bank",118000m,118000m,0,"CHL-GST-202601-001","CPIN-20260218-ABC123","Reconciled",new DateTime(2026,2,18),"admin@acme.com","GST liability payment for Jan 2026"),
                Build("70000000-0000-0000-0000-000000000002","TAXSET-2026-00002","Approved","GSTMixedSettlement",new DateTime(2026,3,15),"2026-02","GST","GST","Mixed",150000m,80000m,70000m,"CHL-GST-202602-001",null,"NotReconciled",null,null,"Feb 2026 GST - ITC offset + bank payment"),
                Build("70000000-0000-0000-0000-000000000003","TAXSET-2026-00003","Posted","TDSRemittance",new DateTime(2026,2,7),"2026-01","TDS","IncomeTax","Bank",84500m,84500m,0,"CHL-TDS-202601-001","CIN-TDS-20260207-XYZ","Reconciled",new DateTime(2026,2,7),"admin@acme.com","TDS remittance for Jan 2026 - 194C & 194J"),
                Build("70000000-0000-0000-0000-000000000004","TAXSET-2026-00004","Closed","TCSRemittance",new DateTime(2026,1,7),"2025-12","TCS","IncomeTax","Bank",12500m,12500m,0,"CHL-TCS-202512-001","CIN-TCS-20260107-DEF","Reconciled",new DateTime(2026,1,7),"admin@acme.com","TCS remittance for Q3 FY26"),
                Build("70000000-0000-0000-0000-000000000005","TAXSET-2026-00005","Draft","GSTInputCreditOffset",new DateTime(2026,4,12),"2026-03","GST","GST","CreditOffsetOnly",95000m,0,0,null,null,"NotReconciled",null,null,"Mar 2026 GST full ITC offset - draft"),
                Build("70000000-0000-0000-0000-000000000006","TAXSET-2026-00006","Reversed","TDSRemittance",new DateTime(2026,1,25),"2026-01","TDS","IncomeTax","Bank",25000m,25000m,0,null,null,"NotReconciled",new DateTime(2026,1,25),"admin@acme.com","TDS remittance reversed - wrong bank account"),
                Build("70000000-0000-0000-0000-000000000007","TAXSET-2026-00007","Submitted","GSTCashPayment",new DateTime(2026,4,18),"2026-03","GST","GST","Bank",225000m,225000m,0,null,null,"NotReconciled",null,null,"GST cash payment for Mar 2026 - pending approval"),
                Build("70000000-0000-0000-0000-000000000008","TAXSET-2026-00008","Posted","TDSRemittance",new DateTime(2026,3,7),"2026-02","TDS","IncomeTax","Bank",62000m,62000m,0,"CHL-TDS-202602-001","CIN-TDS-20260307-GHI","PartiallyReconciled",new DateTime(2026,3,7),"admin@acme.com","TDS remittance Feb 2026 - 194J"),
                Build("70000000-0000-0000-0000-000000000009","TAXSET-2026-00009","Draft","TaxAdjustment",new DateTime(2026,4,20),"2026-03","GST","GST","Bank",5200m,5200m,0,null,null,"NotReconciled",null,null,"Rounding adjustment for Mar 2026 GST"),
                Build("70000000-0000-0000-0000-000000000010","TAXSET-2026-00010","Posted","GSTMixedSettlement",new DateTime(2026,1,18),"2025-12","GST","GST","Mixed",180000m,100000m,80000m,"CHL-GST-202512-001","CPIN-20260118-JKL","Reconciled",new DateTime(2026,1,18),"admin@acme.com","Dec 2025 GST mixed settlement"),
                Build("70000000-0000-0000-0000-000000000011","TAXSET-2026-00011","Cancelled","GSTCashPayment",new DateTime(2026,2,25),"2026-01","GST","GST","Bank",45000m,0,0,null,null,"NotReconciled",null,null,"Cancelled - duplicate of TAXSET-2026-00001"),
                Build("70000000-0000-0000-0000-000000000012","TAXSET-2026-00012","Posted","TCSRemittance",new DateTime(2026,4,7),"2026-03","TCS","IncomeTax","Bank",18750m,18750m,0,"CHL-TCS-202603-001","CIN-TCS-20260407-MNO","NotReconciled",new DateTime(2026,4,7),"admin@acme.com","TCS remittance for Mar 2026"),
            };
        }

        private static TaxSettlementModel Build(string id, string num, string status, string type, DateTime date, string period, string scope, string govAuth, string payMode, decimal outstanding, decimal cashPaid, decimal creditOffset, string? challan, string? govRef, string recon, DateTime? postedOn, string? postedBy, string narration)
        {
            return new()
            {
                Id = Guid.Parse(id), CompanyId = Co, CompanyName = "Acme Pvt Ltd", BranchId = Br, BranchName = "Chennai HQ",
                SettlementNumber = num, SettlementStatus = status, SettlementType = type, SettlementDate = date,
                PostingDate = postedOn, Narration = narration,
                AccountingPeriodId = Guid.NewGuid(), AccountingPeriodName = period,
                TaxPeriodKey = period, TaxTypeScope = scope, GovernmentAuthorityType = govAuth,
                TotalOutstandingAmount = outstanding, TotalSettlementAmount = cashPaid + creditOffset,
                TotalCashOrBankPaidAmount = cashPaid, TotalCreditOffsetAmount = creditOffset,
                RemainingUnsettledAmount = outstanding - cashPaid - creditOffset,
                AllocationCount = (cashPaid + creditOffset) > 0 ? 2 : 0,
                PaymentMode = payMode,
                BankAccountId = payMode != "CreditOffsetOnly" ? Bk : null,
                BankAccountName = payMode != "CreditOffsetOnly" ? "HDFC Current Account" : null,
                ChallanNumber = challan, ChallanDate = challan != null ? date : null,
                GovernmentReferenceNumber = govRef,
                PaymentReferenceNumber = challan != null ? $"UTR-HDFC-{num.Split('-').Last()}" : null,
                RemittedOn = challan != null ? date : null,
                IsRemittanceProofAttached = challan != null,
                JournalEntryId = postedOn.HasValue ? Guid.NewGuid() : null,
                PostedOn = postedOn, PostedBy = postedBy,
                IsFullyAllocated = outstanding <= (cashPaid + creditOffset) && outstanding > 0,
                IsFullyReconciled = recon == "Reconciled",
                ReconciliationStatus = recon,
                ReversalReason = status == "Reversed" ? "Wrong bank account used" : null,
                CreatedAt = date.AddDays(-2), CreatedBy = "taxteam@acme.com",
            };
        }
    }
}
