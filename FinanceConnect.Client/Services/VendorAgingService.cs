using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    /// <summary>
    /// Service for VendorAging (Model #42) operations
    /// Demo application - data stored in memory
    /// Manages aging snapshots, vendor summaries, and bill drilldowns
    /// Supports 8 aging buckets: Current, 0-30, 31-60, 61-90, 91-120, 121-180, 181-365, 365+
    /// </summary>
    public class VendorAgingService
    {
        // Immutable seed data
        private static readonly List<VendorAgingViewModel> _seedSnapshots = VendorAgingSeedData.GetSeedAgingSnapshots();

        // Working (mutable) data
        private List<VendorAgingViewModel> _snapshots;

        public VendorAgingService()
        {
            ResetToSeed();
        }

        private static List<T> CloneList<T>(IEnumerable<T> source) where T : class
        {
            var json = System.Text.Json.JsonSerializer.Serialize(source);
            return System.Text.Json.JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }

        /// <summary>Reset snapshots to seed data</summary>
        public void ResetToSeed()
        {
            _snapshots = CloneList(_seedSnapshots);
        }

        #region Read Operations - Snapshots

        /// <summary>Get all snapshots</summary>
        public List<VendorAgingViewModel> GetAll()
        {
            return _snapshots.Where(s => !s.IsDeleted).ToList();
        }

        /// <summary>Get snapshot by ID</summary>
        public VendorAgingViewModel? GetById(Guid id)
        {
            return _snapshots.FirstOrDefault(s => s.VendorAgingId == id && !s.IsDeleted);
        }

        /// <summary>Get snapshots by company ID</summary>
        public List<VendorAgingViewModel> GetByCompanyId(Guid companyId)
        {
            return _snapshots.Where(s => s.CompanyId == companyId && !s.IsDeleted).ToList();
        }

        /// <summary>Get snapshots by run status</summary>
        public List<VendorAgingViewModel> GetByRunStatus(string status)
        {
            return _snapshots.Where(s => s.RunStatus == status && !s.IsDeleted).ToList();
        }

        /// <summary>Get snapshots by run type</summary>
        public List<VendorAgingViewModel> GetByRunType(string runType)
        {
            return _snapshots.Where(s => s.RunType == runType && !s.IsDeleted).ToList();
        }

        /// <summary>Get snapshots by branch ID</summary>
        public List<VendorAgingViewModel> GetByBranchId(Guid? branchId)
        {
            return _snapshots.Where(s => s.BranchId == branchId && !s.IsDeleted).ToList();
        }

        /// <summary>Get snapshots by date range</summary>
        public List<VendorAgingViewModel> GetByDateRange(DateTime fromDate, DateTime toDate)
        {
            return _snapshots.Where(s =>
                s.AsOfDate >= fromDate &&
                s.AsOfDate <= toDate &&
                !s.IsDeleted).ToList();
        }

        /// <summary>Get latest completed/finalized snapshot for a company</summary>
        public VendorAgingViewModel? GetLatestCompleted(Guid companyId, Guid? branchId = null, string? runType = null)
        {
            var query = _snapshots.Where(s =>
                s.CompanyId == companyId &&
                (s.RunStatus == VendorAgingRunStatuses.Completed || s.RunStatus == VendorAgingRunStatuses.Finalized) &&
                !s.IsDeleted);

            if (branchId.HasValue)
                query = query.Where(s => s.BranchId == branchId.Value);

            if (!string.IsNullOrEmpty(runType))
                query = query.Where(s => s.RunType == runType);

            return query.OrderByDescending(s => s.AsOfDate).FirstOrDefault();
        }

        /// <summary>Search snapshots</summary>
        public List<VendorAgingViewModel> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAll();

            searchTerm = searchTerm.ToLower();
            return _snapshots.Where(s => !s.IsDeleted && (
                (s.CompanyName?.ToLower().Contains(searchTerm) ?? false) ||
                (s.BranchName?.ToLower().Contains(searchTerm) ?? false) ||
                s.AsOfDate.ToString("dd-MMM-yyyy").ToLower().Contains(searchTerm) ||
                s.AgingRunId.ToString().ToLower().Contains(searchTerm)
            )).ToList();
        }

        /// <summary>Check if snapshot exists for given parameters</summary>
        public bool SnapshotExists(Guid companyId, DateTime asOfDate, string runType, Guid? branchId = null)
        {
            return _snapshots.Any(s =>
                s.CompanyId == companyId &&
                s.AsOfDate.Date == asOfDate.Date &&
                s.RunType == runType &&
                s.BranchId == branchId &&
                (s.RunStatus == VendorAgingRunStatuses.Completed || s.RunStatus == VendorAgingRunStatuses.Finalized) &&
                !s.IsDeleted);
        }

        #endregion

        #region Read Operations - Vendor Rows

        /// <summary>Get vendor summary rows for a snapshot</summary>
        public List<VendorAgingVendorRowViewModel> GetVendorRows(Guid agingId)
        {
            var snapshot = GetById(agingId);
            return snapshot?.VendorRows ?? new List<VendorAgingVendorRowViewModel>();
        }

        /// <summary>Get vendor summary row by vendor ID</summary>
        public VendorAgingVendorRowViewModel? GetVendorRow(Guid agingId, Guid vendorId)
        {
            var snapshot = GetById(agingId);
            return snapshot?.VendorRows.FirstOrDefault(r => r.VendorId == vendorId);
        }

        /// <summary>Get vendor rows filtered by overdue status</summary>
        public List<VendorAgingVendorRowViewModel> GetOverdueVendors(Guid agingId)
        {
            var snapshot = GetById(agingId);
            if (snapshot == null) return new List<VendorAgingVendorRowViewModel>();

            return snapshot.VendorRows
                .Where(r => r.OverdueTotalAmount > 0)
                .OrderByDescending(r => r.OverdueTotalAmount)
                .ToList();
        }

        /// <summary>Get top vendors by outstanding amount (for payment prioritization)</summary>
        public List<VendorAgingVendorRowViewModel> GetTopVendorsByOutstanding(Guid agingId, int count = 10)
        {
            var snapshot = GetById(agingId);
            if (snapshot == null) return new List<VendorAgingVendorRowViewModel>();

            return snapshot.VendorRows
                .OrderByDescending(r => r.NetPayableAmount)
                .Take(count)
                .ToList();
        }

        /// <summary>Get vendors with credits exceeding outstanding</summary>
        public List<VendorAgingVendorRowViewModel> GetVendorsWithExcessCredits(Guid agingId)
        {
            var snapshot = GetById(agingId);
            if (snapshot == null) return new List<VendorAgingVendorRowViewModel>();

            return snapshot.VendorRows
                .Where(r => r.CreditBalanceExcessAmount > 0)
                .OrderByDescending(r => r.CreditBalanceExcessAmount)
                .ToList();
        }

        #endregion

        #region Read Operations - Bill Rows

        /// <summary>Get bill detail rows for a snapshot</summary>
        public List<VendorAgingBillRowViewModel> GetBillRows(Guid agingId)
        {
            var snapshot = GetById(agingId);
            return snapshot?.BillRows ?? new List<VendorAgingBillRowViewModel>();
        }

        /// <summary>Get bill detail rows for a specific vendor</summary>
        public List<VendorAgingBillRowViewModel> GetBillRowsByVendor(Guid agingId, Guid vendorId)
        {
            var snapshot = GetById(agingId);
            if (snapshot == null) return new List<VendorAgingBillRowViewModel>();

            return snapshot.BillRows
                .Where(r => r.VendorId == vendorId)
                .OrderByDescending(r => r.OverdueDays)
                .ToList();
        }

        /// <summary>Get bill detail rows by bucket</summary>
        public List<VendorAgingBillRowViewModel> GetBillRowsByBucket(Guid agingId, string bucketCode)
        {
            var snapshot = GetById(agingId);
            if (snapshot == null) return new List<VendorAgingBillRowViewModel>();

            return snapshot.BillRows
                .Where(r => r.BucketCode == bucketCode)
                .OrderByDescending(r => r.OutstandingAmount)
                .ToList();
        }

        #endregion

        #region Write Operations

        /// <summary>Generate new aging snapshot</summary>
        public (bool Success, string Message, Guid? AgingId) GenerateSnapshot(
            Guid companyId,
            string companyName,
            DateTime asOfDate,
            string runType,
            Guid currencyId,
            string currencyCode,
            string currencyName,
            string userName,
            Guid? branchId = null,
            string? branchCode = null,
            string? branchName = null,
            bool includeOpenCredits = true)
        {
            // Validate: MonthEndFinal cannot be overwritten
            if (runType == VendorAgingRunTypes.MonthEndFinal)
            {
                var existingFinal = _snapshots.FirstOrDefault(s =>
                    s.CompanyId == companyId &&
                    s.AsOfDate.Date == asOfDate.Date &&
                    s.RunType == VendorAgingRunTypes.MonthEndFinal &&
                    s.RunStatus == VendorAgingRunStatuses.Finalized &&
                    !s.IsDeleted);

                if (existingFinal != null)
                {
                    return (false, "A finalized Month-End snapshot already exists for this date. Cannot overwrite.", null);
                }
            }

            var snapshot = new VendorAgingViewModel
            {
                VendorAgingId = Guid.NewGuid(),
                TenantId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                CompanyId = companyId,
                CompanyName = companyName,
                BranchId = branchId,
                BranchCode = branchCode,
                BranchName = branchName,
                CurrencyId = currencyId,
                CurrencyCode = currencyCode,
                CurrencyName = currencyName,
                AsOfDate = asOfDate,
                AgingRunId = Guid.NewGuid(),
                RunType = runType,
                RunStatus = VendorAgingRunStatuses.Started,
                GeneratedAt = DateTime.Now,
                GeneratedByUserName = userName,
                IncludeOpenCredits = includeOpenCredits,
                CreatedAt = DateTime.Now,
                CreatedBy = userName
            };

            _snapshots.Add(snapshot);

            // Simulate snapshot generation (in real app, this would fetch from bills)
            SimulateSnapshotGeneration(snapshot);

            return (true, "Vendor aging snapshot generated successfully.", snapshot.VendorAgingId);
        }

        private void SimulateSnapshotGeneration(VendorAgingViewModel snapshot)
        {
            // In a real application, this would:
            // 1. Fetch all posted vendor bills with outstanding > 0
            // 2. Calculate overdue days based on DueDate vs AsOfDate
            // 3. Assign to buckets (8 buckets for AP)
            // 4. Aggregate vendor totals
            // 5. Calculate credits and net payable

            // For demo, mark as completed
            snapshot.RunStatus = VendorAgingRunStatuses.Completed;
            snapshot.UpdatedAt = DateTime.Now;
        }

        /// <summary>Finalize month-end snapshot (lock for audit)</summary>
        public (bool Success, string Message) FinalizeSnapshot(Guid agingId, string userName)
        {
            var snapshot = GetById(agingId);
            if (snapshot == null)
            {
                return (false, "Snapshot not found.");
            }

            if (snapshot.RunStatus != VendorAgingRunStatuses.Completed)
            {
                return (false, "Only completed snapshots can be finalized.");
            }

            if (snapshot.RunType != VendorAgingRunTypes.MonthEndFinal)
            {
                return (false, "Only Month-End snapshots can be finalized.");
            }

            snapshot.RunStatus = VendorAgingRunStatuses.Finalized;
            snapshot.UpdatedAt = DateTime.Now;
            snapshot.UpdatedBy = userName;

            return (true, "Snapshot finalized successfully. It is now locked for audit compliance.");
        }

        /// <summary>Retry failed snapshot generation</summary>
        public (bool Success, string Message) RetryFailedSnapshot(Guid agingId, string userName)
        {
            var snapshot = GetById(agingId);
            if (snapshot == null)
            {
                return (false, "Snapshot not found.");
            }

            if (snapshot.RunStatus != VendorAgingRunStatuses.Failed)
            {
                return (false, "Only failed snapshots can be retried.");
            }

            snapshot.RunStatus = VendorAgingRunStatuses.Started;
            snapshot.GeneratedAt = DateTime.Now;
            snapshot.GeneratedByUserName = userName;
            snapshot.AgingRunId = Guid.NewGuid();
            snapshot.UpdatedAt = DateTime.Now;
            snapshot.UpdatedBy = userName;

            // Simulate retry
            SimulateSnapshotGeneration(snapshot);

            return (true, "Snapshot regeneration completed successfully.");
        }

        /// <summary>Delete snapshot (soft delete) - only failed snapshots can be deleted</summary>
        public (bool Success, string Message) Delete(Guid agingId)
        {
            var snapshot = GetById(agingId);
            if (snapshot == null)
            {
                return (false, "Snapshot not found.");
            }

            // Finalized snapshots cannot be deleted (immutable for audit)
            if (snapshot.RunStatus == VendorAgingRunStatuses.Finalized)
            {
                return (false, "Finalized snapshots cannot be deleted for audit compliance.");
            }

            // Completed snapshots should not be deleted (audit trail)
            if (snapshot.RunStatus == VendorAgingRunStatuses.Completed)
            {
                return (false, "Completed snapshots cannot be deleted to maintain audit trail.");
            }

            snapshot.IsDeleted = true;
            snapshot.DeletedAt = DateTime.Now;
            snapshot.DeletedBy = "System";

            return (true, "Snapshot deleted successfully.");
        }

        #endregion

        #region Statistics

        /// <summary>Get aging statistics</summary>
        public VendorAgingStatisticsViewModel GetStatistics(Guid? companyId = null)
        {
            var snapshots = companyId.HasValue
                ? _snapshots.Where(s => s.CompanyId == companyId.Value && !s.IsDeleted)
                : _snapshots.Where(s => !s.IsDeleted);

            var completedSnapshots = snapshots.Where(s => 
                s.RunStatus == VendorAgingRunStatuses.Completed || 
                s.RunStatus == VendorAgingRunStatuses.Finalized).ToList();

            // Get latest completed snapshot for analysis
            var latestSnapshot = completedSnapshots.OrderByDescending(s => s.AsOfDate).FirstOrDefault();

            var stats = new VendorAgingStatisticsViewModel
            {
                TotalSnapshots = snapshots.Count(),
                CompletedSnapshots = snapshots.Count(s => s.RunStatus == VendorAgingRunStatuses.Completed),
                FinalizedSnapshots = snapshots.Count(s => s.RunStatus == VendorAgingRunStatuses.Finalized),
                StartedSnapshots = snapshots.Count(s => s.RunStatus == VendorAgingRunStatuses.Started),
                FailedSnapshots = snapshots.Count(s => s.RunStatus == VendorAgingRunStatuses.Failed),
                TotalVendors = latestSnapshot?.RecordCountVendors ?? 0,
                TotalBills = latestSnapshot?.RecordCountBills ?? 0,
                TotalOutstanding = latestSnapshot?.TotalOutstandingAmount ?? 0
            };

            if (latestSnapshot != null)
            {
                stats.CurrentNotDueTotal = latestSnapshot.VendorRows.Sum(r => r.CurrentNotDueAmount);
                stats.Bucket0To30Total = latestSnapshot.VendorRows.Sum(r => r.Bucket_0_30);
                stats.Bucket31To60Total = latestSnapshot.VendorRows.Sum(r => r.Bucket_31_60);
                stats.Bucket61To90Total = latestSnapshot.VendorRows.Sum(r => r.Bucket_61_90);
                stats.Bucket91To120Total = latestSnapshot.VendorRows.Sum(r => r.Bucket_91_120);
                stats.Bucket121To180Total = latestSnapshot.VendorRows.Sum(r => r.Bucket_121_180);
                stats.Bucket181To365Total = latestSnapshot.VendorRows.Sum(r => r.Bucket_181_365);
                stats.Bucket366PlusTotal = latestSnapshot.VendorRows.Sum(r => r.Bucket_366_Plus);
                stats.TotalOpenCredits = latestSnapshot.VendorRows.Sum(r => r.OpenCreditAmount);
                stats.TotalNetPayable = latestSnapshot.VendorRows.Sum(r => r.NetPayableAmount);
                stats.OverdueVendors = latestSnapshot.VendorRows.Count(r => r.OverdueTotalAmount > 0);
            }

            return stats;
        }

        /// <summary>Get bucket summary for a snapshot</summary>
        public VendorAgingBucketSummaryViewModel GetBucketSummary(Guid agingId)
        {
            var snapshot = GetById(agingId);
            if (snapshot == null) return new VendorAgingBucketSummaryViewModel();

            return new VendorAgingBucketSummaryViewModel
            {
                CurrentNotDueAmount = snapshot.VendorRows.Sum(r => r.CurrentNotDueAmount),
                Days0To30Amount = snapshot.VendorRows.Sum(r => r.Bucket_0_30),
                Days31To60Amount = snapshot.VendorRows.Sum(r => r.Bucket_31_60),
                Days61To90Amount = snapshot.VendorRows.Sum(r => r.Bucket_61_90),
                Days91To120Amount = snapshot.VendorRows.Sum(r => r.Bucket_91_120),
                Days121To180Amount = snapshot.VendorRows.Sum(r => r.Bucket_121_180),
                Days181To365Amount = snapshot.VendorRows.Sum(r => r.Bucket_181_365),
                Days366PlusAmount = snapshot.VendorRows.Sum(r => r.Bucket_366_Plus),
                TotalAmount = snapshot.TotalOutstandingAmount,
                OpenCreditsAmount = snapshot.VendorRows.Sum(r => r.OpenCreditAmount),
                NetPayableAmount = snapshot.VendorRows.Sum(r => r.NetPayableAmount),
                CurrentNotDueCount = snapshot.BillRows.Count(r => r.BucketCode == VendorAgingBucketCodes.CurrentNotDue),
                Days0To30Count = snapshot.BillRows.Count(r => r.BucketCode == VendorAgingBucketCodes.Days0To30),
                Days31To60Count = snapshot.BillRows.Count(r => r.BucketCode == VendorAgingBucketCodes.Days31To60),
                Days61To90Count = snapshot.BillRows.Count(r => r.BucketCode == VendorAgingBucketCodes.Days61To90),
                Days91To120Count = snapshot.BillRows.Count(r => r.BucketCode == VendorAgingBucketCodes.Days91To120),
                Days121To180Count = snapshot.BillRows.Count(r => r.BucketCode == VendorAgingBucketCodes.Days121To180),
                Days181To365Count = snapshot.BillRows.Count(r => r.BucketCode == VendorAgingBucketCodes.Days181To365),
                Days366PlusCount = snapshot.BillRows.Count(r => r.BucketCode == VendorAgingBucketCodes.Days366Plus)
            };
        }

        #endregion
    }

    #region Statistics Models

    /// <summary>Vendor Aging Statistics</summary>
    public class VendorAgingStatisticsViewModel
    {
        public int TotalSnapshots { get; set; }
        public int CompletedSnapshots { get; set; }
        public int FinalizedSnapshots { get; set; }
        public int StartedSnapshots { get; set; }
        public int FailedSnapshots { get; set; }
        public int TotalVendors { get; set; }
        public int TotalBills { get; set; }
        public int OverdueVendors { get; set; }
        public decimal TotalOutstanding { get; set; }
        public decimal CurrentNotDueTotal { get; set; }
        public decimal Bucket0To30Total { get; set; }
        public decimal Bucket31To60Total { get; set; }
        public decimal Bucket61To90Total { get; set; }
        public decimal Bucket91To120Total { get; set; }
        public decimal Bucket121To180Total { get; set; }
        public decimal Bucket181To365Total { get; set; }
        public decimal Bucket366PlusTotal { get; set; }
        public decimal TotalOpenCredits { get; set; }
        public decimal TotalNetPayable { get; set; }
    }

    /// <summary>Vendor Aging Bucket Summary - 8 buckets for AP</summary>
    public class VendorAgingBucketSummaryViewModel
    {
        public decimal CurrentNotDueAmount { get; set; }
        public decimal Days0To30Amount { get; set; }
        public decimal Days31To60Amount { get; set; }
        public decimal Days61To90Amount { get; set; }
        public decimal Days91To120Amount { get; set; }
        public decimal Days121To180Amount { get; set; }
        public decimal Days181To365Amount { get; set; }
        public decimal Days366PlusAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal OpenCreditsAmount { get; set; }
        public decimal NetPayableAmount { get; set; }
        public int CurrentNotDueCount { get; set; }
        public int Days0To30Count { get; set; }
        public int Days31To60Count { get; set; }
        public int Days61To90Count { get; set; }
        public int Days91To120Count { get; set; }
        public int Days121To180Count { get; set; }
        public int Days181To365Count { get; set; }
        public int Days366PlusCount { get; set; }
        public int TotalCount => CurrentNotDueCount + Days0To30Count + Days31To60Count + Days61To90Count + 
                                  Days91To120Count + Days121To180Count + Days181To365Count + Days366PlusCount;
    }

    #endregion
}
