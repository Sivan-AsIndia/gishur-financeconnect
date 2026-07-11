using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    #region Model #33: CustomerAging

    /// <summary>
    /// Model #33: CustomerAging – Time-based outstanding analysis snapshot.
    /// Groups customer's open receivables into aging buckets (Current, 1-30, 31-60, 61-90, 90+).
    /// Used for collections, month-end AR review, and audit evidence.
    /// Snapshots are immutable once completed.
    /// </summary>
    public class CustomerAgingViewModel
    {
        // Section 1: Snapshot Header Fields

        /// <summary>PK - CustomerAgingId - hidden in UI</summary>
        public Guid CustomerAgingId { get; set; } = Guid.NewGuid();

        /// <summary>FK → Company - hidden in UI</summary>
        [Required(ErrorMessage = "Company is required")]
        public Guid CompanyId { get; set; }
        public string? CompanyName { get; set; }

        /// <summary>Tenant identifier - hidden in UI</summary>
        [Required(ErrorMessage = "Tenant is required")]
        public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

        /// <summary>FK → Branch (Optional scope) - filter selection / read-only on snapshot</summary>
        public Guid? BranchId { get; set; }
        public string? BranchCode { get; set; }
        public string? BranchName { get; set; }

        /// <summary>FK → CurrencyMaster (Optional scope) - filter selection / read-only</summary>
        public Guid? CurrencyId { get; set; }
        public string? CurrencyCode { get; set; }
        public string? CurrencyName { get; set; }

        /// <summary>As Of Date - date picker for running report</summary>
        [Required(ErrorMessage = "As Of Date is required")]
        public DateTime AsOfDate { get; set; } = DateTime.Today;

        /// <summary>Aging Basis - dropdown (DueDate/InvoiceDate)</summary>
        [Required(ErrorMessage = "Aging Basis is required")]
        public string AgingBasis { get; set; } = "";

        /// <summary>Bucket Policy Version - read-only - to lock bucket boundaries if policy changes</summary>
        public int BucketPolicyVersion { get; set; } = 1;

        /// <summary>Bucket Definition JSON - read-only - stores bucket boundaries</summary>
        [StringLength(1000)]
        public string? BucketDefinitionJson { get; set; } = "{\"Current\":\"<=0\",\"1-30\":\"1-30\",\"31-60\":\"31-60\",\"61-90\":\"61-90\",\"90+\":\">90\"}";

        /// <summary>Snapshot Status - badge (Generating/Completed/Failed)</summary>
        [Required(ErrorMessage = "Snapshot Status is required")]
        public string SnapshotStatus { get; set; } = SnapshotStatuses.Generating;

        /// <summary>Generated On - datetime - read-only</summary>
        public DateTime GeneratedOn { get; set; } = DateTime.Now;

        /// <summary>FK → User who generated - read-only</summary>
        public Guid? GeneratedByUserId { get; set; }
        public string? GeneratedByUserName { get; set; }

        /// <summary>Job Run Id - string/GUID - read-only - trace scheduled job execution</summary>
        [StringLength(50)]
        public string? JobRunId { get; set; }

        /// <summary>Record Count - Customers - read-only</summary>
        public int RecordCountCustomers { get; set; } = 0;

        /// <summary>Record Count - Invoices - read-only</summary>
        public int RecordCountInvoices { get; set; } = 0;

        /// <summary>Total Outstanding Amount - decimal(18,2) - read-only - sum of all customer totals</summary>
        public decimal TotalOutstandingAmount { get; set; } = 0;

        // Section 2: Child Collections

        /// <summary>Customer Summary Rows - one row per customer</summary>
        public List<CustomerAgingCustomerRowModel> CustomerRows { get; set; } = new();

        /// <summary>Invoice Detail Rows - drilldown per invoice (optional but recommended)</summary>
        public List<CustomerAgingInvoiceRowModel> InvoiceRows { get; set; } = new();

        // Section 3: System Audit Fields

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        // Helper Methods

        /// <summary>Recalculate totals from customer rows</summary>
        public void RecalculateTotals()
        {
            TotalOutstandingAmount = CustomerRows.Sum(r => r.TotalOutstanding);
            RecordCountCustomers = CustomerRows.Count;
            RecordCountInvoices = InvoiceRows.Count;
        }

        /// <summary>Check if snapshot is completed</summary>
        public bool IsCompleted => SnapshotStatus == SnapshotStatuses.Completed;

        /// <summary>Check if snapshot can be regenerated</summary>
        public bool CanRegenerate => SnapshotStatus == SnapshotStatuses.Failed;
    }

    #endregion

    #region CustomerAgingCustomerRow

    /// <summary>
    /// Customer Summary Row - one row per customer in the aging snapshot.
    /// Contains bucket totals and metrics for collections.
    /// </summary>
    public class CustomerAgingCustomerRowModel
    {
        /// <summary>PK - CustomerAgingCustomerRowId - hidden</summary>
        public Guid CustomerAgingCustomerRowId { get; set; } = Guid.NewGuid();

        /// <summary>FK → CustomerAging - hidden</summary>
        [Required]
        public Guid CustomerAgingId { get; set; }

        /// <summary>FK → Customer - read-only link</summary>
        [Required(ErrorMessage = "Customer is required")]
        public Guid CustomerId { get; set; }

        /// <summary>Customer Code Snapshot - read-only - preserve identity as-of snapshot</summary>
        [StringLength(20)]
        public string? CustomerCodeSnapshot { get; set; }

        /// <summary>Customer Name Snapshot - read-only - preserve identity as-of snapshot</summary>
        [StringLength(200)]
        public string? CustomerNameSnapshot { get; set; }

        /// <summary>Total Outstanding - decimal(18,2) - read-only</summary>
        public decimal TotalOutstanding { get; set; } = 0;

        /// <summary>Bucket Current Amount - decimal(18,2) - read-only</summary>
        public decimal BucketCurrentAmount { get; set; } = 0;

        /// <summary>Bucket 1 to 30 Days Amount - decimal(18,2) - read-only</summary>
        public decimal Bucket1To30Amount { get; set; } = 0;

        /// <summary>Bucket 31 to 60 Days Amount - decimal(18,2) - read-only</summary>
        public decimal Bucket31To60Amount { get; set; } = 0;

        /// <summary>Bucket 61 to 90 Days Amount - decimal(18,2) - read-only</summary>
        public decimal Bucket61To90Amount { get; set; } = 0;

        /// <summary>Bucket 90+ Days Amount - decimal(18,2) - read-only</summary>
        public decimal Bucket90PlusAmount { get; set; } = 0;

        /// <summary>Oldest Due Date - date (nullable) - read-only</summary>
        public DateTime? OldestDueDate { get; set; }

        /// <summary>Max Overdue Days - int - read-only</summary>
        public int MaxOverdueDays { get; set; } = 0;

        /// <summary>Open Invoice Count - int - read-only</summary>
        public int InvoiceCountOpen { get; set; } = 0;

        /// <summary>Collections Priority Score (Optional) - int - read-only - for sorting</summary>
        public int CollectionsPriorityScore { get; set; } = 0;

        // System Audit Fields
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Helper: Validate that bucket sum equals total
        public bool ValidateBucketSum()
        {
            var sum = BucketCurrentAmount + Bucket1To30Amount + Bucket31To60Amount + Bucket61To90Amount + Bucket90PlusAmount;
            return Math.Abs(sum - TotalOutstanding) < 0.01m;
        }
    }

    #endregion

    #region CustomerAgingInvoiceRow

    /// <summary>
    /// Invoice Detail Row - drilldown per invoice in the aging snapshot.
    /// Shows invoice-level aging details for audit and verification.
    /// </summary>
    public class CustomerAgingInvoiceRowModel
    {
        /// <summary>PK - CustomerAgingInvoiceRowId - hidden</summary>
        public Guid CustomerAgingInvoiceRowId { get; set; } = Guid.NewGuid();

        /// <summary>FK → CustomerAging - hidden</summary>
        [Required]
        public Guid CustomerAgingId { get; set; }

        /// <summary>FK → Customer - read-only</summary>
        [Required]
        public Guid CustomerId { get; set; }

        /// <summary>FK → CustomerInvoice - read-only link</summary>
        [Required(ErrorMessage = "Invoice is required")]
        public Guid CustomerInvoiceId { get; set; }

        /// <summary>Invoice Number Snapshot - string - read-only</summary>
        [StringLength(40)]
        public string? InvoiceNumberSnapshot { get; set; }

        /// <summary>Invoice Date Snapshot - date - read-only</summary>
        public DateTime InvoiceDateSnapshot { get; set; }

        /// <summary>Due Date Snapshot - date - read-only</summary>
        public DateTime DueDateSnapshot { get; set; }

        /// <summary>Outstanding Amount - decimal(18,2) - read-only</summary>
        public decimal OutstandingAmount { get; set; } = 0;

        /// <summary>Overdue Days - int - read-only - max(0, AsOfDate - DueDate or InvoiceDate)</summary>
        public int OverdueDays { get; set; } = 0;

        /// <summary>Bucket Code - string/enum - badge (Current/1-30/31-60/61-90/90+)</summary>
        [Required]
        [StringLength(20)]
        public string BucketCode { get; set; } = AgingBucketCodes.Current;

        /// <summary>Last Payment Date Snapshot (Optional) - date - read-only</summary>
        public DateTime? LastPaymentDateSnapshot { get; set; }

        /// <summary>Source Document Type - enum (Invoice/DebitNote/CreditNote/Adjustment) - read-only</summary>
        [StringLength(30)]
        public string SourceDocumentType { get; set; } = SourceDocumentTypes.Invoice;

        // System Audit Fields
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
    }

    #endregion

    #region CustomerAging-related Enums and Static Classes

    /// <summary>Aging Basis Types</summary>
    public static class AgingBasisTypes
    {
        public const string DueDate = "DueDate";
        public const string InvoiceDate = "InvoiceDate";
        public static readonly string[] All = new[] { DueDate, InvoiceDate };

        public static string GetDisplayName(string basis) => basis switch
        {
            DueDate => "Due Date",
            InvoiceDate => "Invoice Date",
            _ => basis
        };
    }

    /// <summary>Snapshot Statuses</summary>
    public static class SnapshotStatuses
    {
        public const string Generating = "Generating";
        public const string Completed = "Completed";
        public const string Failed = "Failed";
        public static readonly string[] All = new[] { Generating, Completed, Failed };

        public static string GetDisplayName(string status) => status switch
        {
            Generating => "Generating",
            Completed => "Completed",
            Failed => "Failed",
            _ => status
        };
    }

    /// <summary>Aging Bucket Codes</summary>
    public static class AgingBucketCodes
    {
        public const string Current = "Current";
        public const string Days1To30 = "1-30";
        public const string Days31To60 = "31-60";
        public const string Days61To90 = "61-90";
        public const string Days90Plus = "90+";
        public static readonly string[] All = new[] { Current, Days1To30, Days31To60, Days61To90, Days90Plus };

        public static string GetDisplayName(string code) => code switch
        {
            Current => "Current",
            Days1To30 => "1-30 Days",
            Days31To60 => "31-60 Days",
            Days61To90 => "61-90 Days",
            Days90Plus => "90+ Days",
            _ => code
        };

        public static string GetBucketCode(int overdueDays)
        {
            if (overdueDays <= 0) return Current;
            if (overdueDays <= 30) return Days1To30;
            if (overdueDays <= 60) return Days31To60;
            if (overdueDays <= 90) return Days61To90;
            return Days90Plus;
        }
    }

    /// <summary>Source Document Types</summary>
    public static class SourceDocumentTypes
    {
        public const string Invoice = "Invoice";
        public const string DebitNote = "DebitNote";
        public const string CreditNote = "CreditNote";
        public const string Adjustment = "Adjustment";
        public static readonly string[] All = new[] { Invoice, DebitNote, CreditNote, Adjustment };

        public static string GetDisplayName(string type) => type switch
        {
            Invoice => "Invoice",
            DebitNote => "Debit Note",
            CreditNote => "Credit Note",
            Adjustment => "Adjustment",
            _ => type
        };
    }

    #endregion
}
