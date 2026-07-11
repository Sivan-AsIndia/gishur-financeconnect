using static FinanceConnect.Client.ViewModels.GSTReturnRunViewModel;

namespace FinanceConnect.Client.Data
{
    public static class GSTReturnRunSeedData
    {
        private static readonly Guid Co = Guid.Parse("10000000-0000-0000-0000-000000000001");
        private static readonly Guid Br = Guid.Parse("20000000-0000-0000-0000-000000000001");

        public static List<GSTReturnRunModel> Get()
        {
            return new List<GSTReturnRunModel>
            {
                Build("71000000-0000-0000-0000-000000000001","GSTRUN-2026-00001","Filed","2026-01",4200,4150,50,0,50,28500000m,1930000m,"Matched","Acknowledged","SHA256:a1b2c3",null),
                Build("71000000-0000-0000-0000-000000000002","GSTRUN-2026-00002","Finalized","2026-02",3800,3780,20,0,20,26000000m,1600000m,"Matched","Prepared","SHA256:x9y8z7",null),
                Build("71000000-0000-0000-0000-000000000003","GSTRUN-2026-00003","Generated","2026-03",4500,4420,80,5,80,30000000m,1700000m,"Mismatch","NotFiled",null,null),
                Build("71000000-0000-0000-0000-000000000004","GSTRUN-2026-00004","Draft","2026-04",0,0,0,0,0,0,0,"NotRun","NotFiled",null,null),
                Build("71000000-0000-0000-0000-000000000005","GSTRUN-2025-00012","Reopened","2025-12",3600,3590,10,0,10,25000000m,1400000m,"Mismatch","NotFiled",null,"Large invoice reversed after finalization"),
                Build("71000000-0000-0000-0000-000000000006","GSTRUN-2025-00011","Closed","2025-11",3400,3395,5,0,5,23000000m,1300000m,"Matched","Acknowledged","SHA256:m5n6o7",null),
                Build("71000000-0000-0000-0000-000000000007","GSTRUN-2025-00010","Filed","2025-10",3900,3880,20,0,20,27000000m,1800000m,"Matched","Acknowledged","SHA256:p8q9r0",null),
                Build("71000000-0000-0000-0000-000000000008","GSTRUN-2026-00005","Reviewed","2026-05",4100,4050,50,0,50,29000000m,1850000m,"Matched","NotFiled",null,null),
                Build("71000000-0000-0000-0000-000000000009","GSTRUN-2026-00006","Approved","2026-06",4300,4280,20,0,20,31000000m,2100000m,"Matched","NotFiled",null,null),
                Build("71000000-0000-0000-0000-000000000010","GSTRUN-2025-00009","Filed","2025-09",3200,3195,5,0,5,22000000m,1200000m,"Matched","Filed","SHA256:s1t2u3",null),
                Build("71000000-0000-0000-0000-000000000011","GSTRUN-2025-00008","Closed","2025-08",3500,3490,10,0,10,24000000m,1500000m,"Matched","Acknowledged","SHA256:v4w5x6",null),
                Build("71000000-0000-0000-0000-000000000012","GSTRUN-2026-00007","Cancelled","2026-07",0,0,0,0,0,0,0,"NotRun","NotFiled",null,null),
            };
        }

        private static GSTReturnRunModel Build(string id, string num, string status, string period, int eligible, int included, int excluded, int blocking, int warnings, decimal taxableTotal, decimal netLiability, string reconStatus, string filingStatus, string? hash, string? reopenReason)
        {
            var parts = period.Split('-');
            int y = int.Parse(parts[0]); int m = int.Parse(parts[1]);
            var startDate = new DateTime(y, m, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            bool isLocked = status == "Finalized" || status == "Filed" || status == "Closed";
            return new()
            {
                Id = Guid.Parse(id), CompanyId = Co, CompanyName = "Acme Pvt Ltd", BranchId = Br, BranchName = "Chennai HQ",
                ReturnRunNumber = num, ReturnRunStatus = status, ReturnType = "CombinedGSTPack",
                ReturnPeriodKey = period, PeriodStartDate = startDate, PeriodEndDate = endDate,
                GenerationDate = status != "Draft" && status != "Cancelled" ? startDate.AddMonths(1).AddDays(3) : null,
                SelectionMode = "ByPostingDate",
                IncludeOutwardSupplies = true, IncludeInwardSupplies = true, IncludeRCMTransactions = true,
                IncludeCreditDebitNotes = true, IncludeExemptNilNonGST = true, IncludeOnlyPostedTransactions = true,
                EligibleTransactionCount = eligible, IncludedTransactionCount = included,
                ExcludedTransactionCount = excluded, IncludedLineCount = included * 3,
                ExceptionCount = blocking + warnings, HasBlockingExceptions = blocking > 0,
                BlockingExceptionCount = blocking, WarningExceptionCount = warnings,
                OutwardTaxableValueTotal = taxableTotal, OutwardCGSTTotal = Math.Round(taxableTotal * 0.09m, 2),
                OutwardSGSTTotal = Math.Round(taxableTotal * 0.09m, 2), OutwardIGSTTotal = Math.Round(taxableTotal * 0.05m, 2),
                InputEligibleITCTotal = Math.Round(taxableTotal * 0.16m, 2),
                NetTaxLiabilityTotal = netLiability,
                TaxLedgerReconciliationStatus = reconStatus, FilingStatus = filingStatus,
                IncludedHashSignature = hash, IsLocked = isLocked,
                LockReason = isLocked ? "Finalized for filing" : null,
                FiledDate = filingStatus == "Filed" || filingStatus == "Acknowledged" ? startDate.AddMonths(1).AddDays(14) : null,
                FiledBy = filingStatus == "Filed" || filingStatus == "Acknowledged" ? "taxteam@acme.com" : null,
                GovernmentAcknowledgementNumber = filingStatus == "Acknowledged" ? $"ACK-GSTR1-{period.Replace("-","")}-{num.Split('-').Last()}" : null,
                ApprovedOn = status == "Approved" || status == "Finalized" || status == "Filed" || status == "Closed" ? startDate.AddMonths(1).AddDays(8) : null,
                ApprovedBy = status == "Approved" || status == "Finalized" || status == "Filed" || status == "Closed" ? "controller@acme.com" : null,
                FinalizedOn = isLocked || status == "Reopened" ? startDate.AddMonths(1).AddDays(10) : null,
                FinalizedBy = isLocked || status == "Reopened" ? "controller@acme.com" : null,
                ReopenedOn = reopenReason != null ? startDate.AddMonths(1).AddDays(18) : null,
                ReopenedBy = reopenReason != null ? "controller@acme.com" : null,
                ReopenReason = reopenReason,
                CreatedAt = startDate.AddDays(28), CreatedBy = "taxteam@acme.com",
            };
        }
    }
}
