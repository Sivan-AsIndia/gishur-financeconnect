using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    /// <summary>
    /// Service for CustomerAging (Model #33) operations
    /// Demo application - data stored in memory
    /// Manages aging snapshots, customer summaries, and invoice drilldowns
    /// </summary>
    public class CustomerAgingService
    {
        // Immutable seed data
        private static readonly List<CustomerAgingViewModel> _seedSnapshots = CustomerAgingSeedData.GetSeedAgingSnapshots();

        // Working (mutable) data
        private List<CustomerAgingViewModel> _snapshots;

        public CustomerAgingService()
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
        public List<CustomerAgingViewModel> GetAll()
        {
            return _snapshots.Where(s => !s.IsDeleted).ToList();
        }

        /// <summary>Get snapshot by ID</summary>
        public CustomerAgingViewModel? GetById(Guid id)
        {
            return _snapshots.FirstOrDefault(s => s.CustomerAgingId == id && !s.IsDeleted);
        }

        /// <summary>Get snapshots by company ID</summary>
        public List<CustomerAgingViewModel> GetByCompanyId(Guid companyId)
        {
            return _snapshots.Where(s => s.CompanyId == companyId && !s.IsDeleted).ToList();
        }

        /// <summary>Get snapshots by status</summary>
        public List<CustomerAgingViewModel> GetByStatus(string status)
        {
            return _snapshots.Where(s => s.SnapshotStatus == status && !s.IsDeleted).ToList();
        }

        /// <summary>Get snapshots by branch ID</summary>
        public List<CustomerAgingViewModel> GetByBranchId(Guid? branchId)
        {
            return _snapshots.Where(s => s.BranchId == branchId && !s.IsDeleted).ToList();
        }

        /// <summary>Get snapshots by date range</summary>
        public List<CustomerAgingViewModel> GetByDateRange(DateTime fromDate, DateTime toDate)
        {
            return _snapshots.Where(s =>
                s.AsOfDate >= fromDate &&
                s.AsOfDate <= toDate &&
                !s.IsDeleted).ToList();
        }

        /// <summary>Get latest completed snapshot for a company</summary>
        public CustomerAgingViewModel? GetLatestCompleted(Guid companyId, Guid? branchId = null, string? agingBasis = null)
        {
            var query = _snapshots.Where(s =>
                s.CompanyId == companyId &&
                s.SnapshotStatus == SnapshotStatuses.Completed &&
                !s.IsDeleted);

            if (branchId.HasValue)
                query = query.Where(s => s.BranchId == branchId.Value);

            if (!string.IsNullOrEmpty(agingBasis))
                query = query.Where(s => s.AgingBasis == agingBasis);

            return query.OrderByDescending(s => s.AsOfDate).FirstOrDefault();
        }

        /// <summary>Search snapshots</summary>
        public List<CustomerAgingViewModel> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAll();

            searchTerm = searchTerm.ToLower();
            return _snapshots.Where(s => !s.IsDeleted && (
                (s.CompanyName?.ToLower().Contains(searchTerm) ?? false) ||
                (s.BranchName?.ToLower().Contains(searchTerm) ?? false) ||
                (s.JobRunId?.ToLower().Contains(searchTerm) ?? false) ||
                s.AsOfDate.ToString("dd-MMM-yyyy").ToLower().Contains(searchTerm)
            )).ToList();
        }

        /// <summary>Check if snapshot exists for given parameters</summary>
        public bool SnapshotExists(Guid companyId, DateTime asOfDate, string agingBasis, Guid? branchId = null, int bucketPolicyVersion = 1)
        {
            return _snapshots.Any(s =>
                s.CompanyId == companyId &&
                s.AsOfDate.Date == asOfDate.Date &&
                s.AgingBasis == agingBasis &&
                s.BranchId == branchId &&
                s.BucketPolicyVersion == bucketPolicyVersion &&
                s.SnapshotStatus == SnapshotStatuses.Completed &&
                !s.IsDeleted);
        }

        #endregion

        #region Read Operations - Customer Rows

        /// <summary>Get customer summary rows for a snapshot</summary>
        public List<CustomerAgingCustomerRowModel> GetCustomerRows(Guid agingId)
        {
            var snapshot = GetById(agingId);
            return snapshot?.CustomerRows ?? new List<CustomerAgingCustomerRowModel>();
        }

        /// <summary>Get customer summary row by customer ID</summary>
        public CustomerAgingCustomerRowModel? GetCustomerRow(Guid agingId, Guid customerId)
        {
            var snapshot = GetById(agingId);
            return snapshot?.CustomerRows.FirstOrDefault(r => r.CustomerId == customerId);
        }

        /// <summary>Get customer rows filtered by overdue status</summary>
        public List<CustomerAgingCustomerRowModel> GetOverdueCustomers(Guid agingId)
        {
            var snapshot = GetById(agingId);
            if (snapshot == null) return new List<CustomerAgingCustomerRowModel>();

            return snapshot.CustomerRows
                .Where(r => r.MaxOverdueDays > 0)
                .OrderByDescending(r => r.CollectionsPriorityScore)
                .ToList();
        }

        /// <summary>Get top customers by outstanding amount</summary>
        public List<CustomerAgingCustomerRowModel> GetTopCustomersByOutstanding(Guid agingId, int count = 10)
        {
            var snapshot = GetById(agingId);
            if (snapshot == null) return new List<CustomerAgingCustomerRowModel>();

            return snapshot.CustomerRows
                .OrderByDescending(r => r.TotalOutstanding)
                .Take(count)
                .ToList();
        }

        #endregion

        #region Read Operations - Invoice Rows

        /// <summary>Get invoice detail rows for a snapshot</summary>
        public List<CustomerAgingInvoiceRowModel> GetInvoiceRows(Guid agingId)
        {
            var snapshot = GetById(agingId);
            return snapshot?.InvoiceRows ?? new List<CustomerAgingInvoiceRowModel>();
        }

        /// <summary>Get invoice detail rows for a specific customer</summary>
        public List<CustomerAgingInvoiceRowModel> GetInvoiceRowsByCustomer(Guid agingId, Guid customerId)
        {
            var snapshot = GetById(agingId);
            if (snapshot == null) return new List<CustomerAgingInvoiceRowModel>();

            return snapshot.InvoiceRows
                .Where(r => r.CustomerId == customerId)
                .OrderByDescending(r => r.OverdueDays)
                .ToList();
        }

        /// <summary>Get invoice detail rows by bucket</summary>
        public List<CustomerAgingInvoiceRowModel> GetInvoiceRowsByBucket(Guid agingId, string bucketCode)
        {
            var snapshot = GetById(agingId);
            if (snapshot == null) return new List<CustomerAgingInvoiceRowModel>();

            return snapshot.InvoiceRows
                .Where(r => r.BucketCode == bucketCode)
                .OrderByDescending(r => r.OutstandingAmount)
                .ToList();
        }

        #endregion

        #region Write Operations

        /// <summary>Generate new aging snapshot</summary>
        public (bool Success, string Message, Guid? SnapshotId) GenerateSnapshot(
            Guid companyId,
            string companyName,
            DateTime asOfDate,
            string agingBasis = "DueDate",
            Guid? branchId = null,
            string? branchCode = null,
            string? branchName = null,
            Guid? currencyId = null,
            string? currencyCode = null,
            string? currencyName = null,
            string? userName = "System")
        {
            // Check for existing snapshot
            if (SnapshotExists(companyId, asOfDate, agingBasis, branchId))
            {
                return (false, "Snapshot already exists for this date, basis, and branch combination.", null);
            }

            var snapshot = new CustomerAgingViewModel
            {
                CustomerAgingId = Guid.NewGuid(),
                CompanyId = companyId,
                CompanyName = companyName,
                TenantId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                BranchId = branchId,
                BranchCode = branchCode,
                BranchName = branchName,
                CurrencyId = currencyId,
                CurrencyCode = currencyCode,
                CurrencyName = currencyName,
                AsOfDate = asOfDate,
                AgingBasis = agingBasis,
                BucketPolicyVersion = 1,
                SnapshotStatus = SnapshotStatuses.Generating,
                GeneratedOn = DateTime.Now,
                GeneratedByUserName = userName,
                JobRunId = $"JOB-{DateTime.Now:yyyyMMddHHmmss}",
                CreatedAt = DateTime.Now,
                CreatedBy = userName
            };

            _snapshots.Add(snapshot);

            // Simulate snapshot generation (in real app, this would fetch from invoices)
            SimulateSnapshotGeneration(snapshot);

            return (true, "Snapshot generated successfully.", snapshot.CustomerAgingId);
        }

        private void SimulateSnapshotGeneration(CustomerAgingViewModel snapshot)
        {
            // In a real application, this would:
            // 1. Fetch all posted invoices with outstanding > 0
            // 2. Calculate overdue days based on AgingBasis
            // 3. Assign to buckets
            // 4. Aggregate customer totals

            // For demo, mark as completed
            snapshot.SnapshotStatus = SnapshotStatuses.Completed;
            snapshot.UpdatedAt = DateTime.Now;
        }

        /// <summary>Retry failed snapshot generation</summary>
        public (bool Success, string Message) RetryFailedSnapshot(Guid agingId, string userName)
        {
            var snapshot = GetById(agingId);
            if (snapshot == null)
            {
                return (false, "Snapshot not found.");
            }

            if (snapshot.SnapshotStatus != SnapshotStatuses.Failed)
            {
                return (false, "Only failed snapshots can be retried.");
            }

            snapshot.SnapshotStatus = SnapshotStatuses.Generating;
            snapshot.GeneratedOn = DateTime.Now;
            snapshot.GeneratedByUserName = userName;
            snapshot.JobRunId = $"JOB-{DateTime.Now:yyyyMMddHHmmss}-RETRY";
            snapshot.UpdatedAt = DateTime.Now;
            snapshot.UpdatedBy = userName;

            // Simulate retry
            SimulateSnapshotGeneration(snapshot);

            return (true, "Snapshot regeneration completed.");
        }

        /// <summary>Delete snapshot (soft delete)</summary>
        public (bool Success, string Message) Delete(Guid agingId)
        {
            var snapshot = GetById(agingId);
            if (snapshot == null)
            {
                return (false, "Snapshot not found.");
            }

            // Completed snapshots should not be deleted (immutable for audit)
            if (snapshot.SnapshotStatus == SnapshotStatuses.Completed)
            {
                return (false, "Completed snapshots cannot be deleted for audit compliance.");
            }

            snapshot.IsDeleted = true;
            snapshot.DeletedAt = DateTime.Now;
            snapshot.DeletedBy = "System";

            return (true, "Snapshot deleted successfully.");
        }

        #endregion

        #region Statistics

        /// <summary>Get aging statistics</summary>
        public CustomerAgingStatisticsViewModel GetStatistics(Guid? companyId = null)
        {
            var snapshots = companyId.HasValue
                ? _snapshots.Where(s => s.CompanyId == companyId.Value && !s.IsDeleted)
                : _snapshots.Where(s => !s.IsDeleted);

            var completedSnapshots = snapshots.Where(s => s.SnapshotStatus == SnapshotStatuses.Completed).ToList();

            // Get latest completed snapshot for bucket analysis
            var latestSnapshot = completedSnapshots.OrderByDescending(s => s.AsOfDate).FirstOrDefault();

            var stats = new CustomerAgingStatisticsViewModel
            {
                TotalSnapshots = snapshots.Count(),
                CompletedSnapshots = completedSnapshots.Count,
                GeneratingSnapshots = snapshots.Count(s => s.SnapshotStatus == SnapshotStatuses.Generating),
                FailedSnapshots = snapshots.Count(s => s.SnapshotStatus == SnapshotStatuses.Failed),
                TotalCustomers = latestSnapshot?.RecordCountCustomers ?? 0,
                TotalInvoices = latestSnapshot?.RecordCountInvoices ?? 0,
                TotalOutstanding = latestSnapshot?.TotalOutstandingAmount ?? 0
            };

            if (latestSnapshot != null)
            {
                stats.BucketCurrentTotal = latestSnapshot.CustomerRows.Sum(r => r.BucketCurrentAmount);
                stats.Bucket1To30Total = latestSnapshot.CustomerRows.Sum(r => r.Bucket1To30Amount);
                stats.Bucket31To60Total = latestSnapshot.CustomerRows.Sum(r => r.Bucket31To60Amount);
                stats.Bucket61To90Total = latestSnapshot.CustomerRows.Sum(r => r.Bucket61To90Amount);
                stats.Bucket90PlusTotal = latestSnapshot.CustomerRows.Sum(r => r.Bucket90PlusAmount);
                stats.OverdueCustomers = latestSnapshot.CustomerRows.Count(r => r.MaxOverdueDays > 0);
            }

            return stats;
        }

        /// <summary>Get bucket summary for a snapshot</summary>
        public BucketSummaryViewModel GetBucketSummary(Guid agingId)
        {
            var snapshot = GetById(agingId);
            if (snapshot == null) return new BucketSummaryViewModel();

            return new BucketSummaryViewModel
            {
                CurrentAmount = snapshot.CustomerRows.Sum(r => r.BucketCurrentAmount),
                Days1To30Amount = snapshot.CustomerRows.Sum(r => r.Bucket1To30Amount),
                Days31To60Amount = snapshot.CustomerRows.Sum(r => r.Bucket31To60Amount),
                Days61To90Amount = snapshot.CustomerRows.Sum(r => r.Bucket61To90Amount),
                Days90PlusAmount = snapshot.CustomerRows.Sum(r => r.Bucket90PlusAmount),
                TotalAmount = snapshot.TotalOutstandingAmount,
                CurrentCount = snapshot.InvoiceRows.Count(r => r.BucketCode == AgingBucketCodes.Current),
                Days1To30Count = snapshot.InvoiceRows.Count(r => r.BucketCode == AgingBucketCodes.Days1To30),
                Days31To60Count = snapshot.InvoiceRows.Count(r => r.BucketCode == AgingBucketCodes.Days31To60),
                Days61To90Count = snapshot.InvoiceRows.Count(r => r.BucketCode == AgingBucketCodes.Days61To90),
                Days90PlusCount = snapshot.InvoiceRows.Count(r => r.BucketCode == AgingBucketCodes.Days90Plus)
            };
        }

        #endregion
    }
}
