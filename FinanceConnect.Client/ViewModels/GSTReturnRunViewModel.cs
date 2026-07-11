using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public class GSTReturnRunViewModel
    {
        public class GSTReturnRunModel
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

            [Required]
            public Guid CompanyId { get; set; }
            public string? CompanyName { get; set; }

            public Guid? BranchId { get; set; }
            public string? BranchName { get; set; }

            [StringLength(40)]
            public string? ReturnRunNumber { get; set; }

            [Required(ErrorMessage = "Return Run Status is required")]
            public string ReturnRunStatus { get; set; } = "Draft";

            [Required(ErrorMessage = "Return Type is required")]
            public string ReturnType { get; set; } = "CombinedGSTPack";

            [Required(ErrorMessage = "Return Period is required")]
            [StringLength(20)]
            public string ReturnPeriodKey { get; set; } = string.Empty;

            [Required(ErrorMessage = "Period Start Date is required")]
            public DateTime PeriodStartDate { get; set; }

            [Required(ErrorMessage = "Period End Date is required")]
            public DateTime PeriodEndDate { get; set; }

            public DateTime? GenerationDate { get; set; }

            // ── Scope & Selection Rules ──
            [Required(ErrorMessage = "Selection Mode is required")]
            public string SelectionMode { get; set; } = "ByPostingDate";

            public bool IncludeOutwardSupplies { get; set; } = true;
            public bool IncludeInwardSupplies { get; set; } = true;
            public bool IncludeRCMTransactions { get; set; } = true;
            public bool IncludeCreditDebitNotes { get; set; } = true;
            public bool IncludeExemptNilNonGST { get; set; } = true;
            public bool IncludeOnlyPostedTransactions { get; set; } = true;

            [StringLength(2000)]
            public string? ScopeFilterJson { get; set; }

            public Guid? TaxPeriodAccountingPeriodId { get; set; }
            public string? AccountingPeriodName { get; set; }

            // ── Generation Metrics (Derived) ──
            public int EligibleTransactionCount { get; set; }
            public int IncludedTransactionCount { get; set; }
            public int ExcludedTransactionCount { get; set; }
            public int IncludedLineCount { get; set; }
            public int ExceptionCount { get; set; }
            public bool HasBlockingExceptions { get; set; }
            public int BlockingExceptionCount { get; set; }
            public int WarningExceptionCount { get; set; }

            // ── Return Summary Totals: Outward ──
            public decimal OutwardTaxableValueTotal { get; set; }
            public decimal OutwardCGSTTotal { get; set; }
            public decimal OutwardSGSTTotal { get; set; }
            public decimal OutwardIGSTTotal { get; set; }
            public decimal OutwardCESSTotal { get; set; }
            public decimal OutwardExemptValueTotal { get; set; }
            public decimal OutwardNilRatedValueTotal { get; set; }
            public decimal OutwardNonGSTValueTotal { get; set; }

            // ── Return Summary Totals: Inward / ITC ──
            public decimal InputEligibleITCTotal { get; set; }
            public decimal InputIneligibleITCTotal { get; set; }
            public decimal RCMLiabilityTotal { get; set; }
            public decimal RCMITCClaimTotal { get; set; }

            // ── Notes / Adjustments ──
            public decimal CreditNoteAdjustmentTotal { get; set; }
            public decimal DebitNoteAdjustmentTotal { get; set; }
            public decimal NetTaxLiabilityTotal { get; set; }

            // ── Included / Excluded Item Linkage ──
            [StringLength(1000)]
            public string? ExclusionReasonSummary { get; set; }

            [StringLength(200)]
            public string? IncludedHashSignature { get; set; }

            // ── Validation & Reconciliation ──
            [Required(ErrorMessage = "Tax Ledger Reconciliation Status is required")]
            public string TaxLedgerReconciliationStatus { get; set; } = "NotRun";

            public string SettlementReconciliationStatus { get; set; } = "NotRun";

            [StringLength(2000)]
            public string? ValidationSummaryJson { get; set; }

            [StringLength(1000)]
            public string? ReviewerNotes { get; set; }

            // ── Filing / Export Metadata ──
            public bool IsExportGenerated { get; set; } = false;
            public DateTime? ExportGeneratedOn { get; set; }
            public string? ExportFormatSummary { get; set; }

            public DateTime? FiledDate { get; set; }
            public string? FiledBy { get; set; }

            [StringLength(100)]
            public string? GovernmentAcknowledgementNumber { get; set; }

            [StringLength(100)]
            public string? GovernmentReferenceNumber { get; set; }

            [Required(ErrorMessage = "Filing Status is required")]
            public string FilingStatus { get; set; } = "NotFiled";

            [StringLength(1000)]
            public string? FilingNotes { get; set; }

            // ── Posting / Adjustment Evidence ──
            public bool TaxSettlementSummaryLinkedFlag { get; set; }
            public string? RelatedTaxSettlementIdsJson { get; set; }

            // ── Governance & Locking ──
            public DateTime? ApprovedOn { get; set; }
            public string? ApprovedBy { get; set; }
            public DateTime? FinalizedOn { get; set; }
            public string? FinalizedBy { get; set; }
            public bool IsLocked { get; set; }

            [StringLength(300)]
            public string? LockReason { get; set; }

            public DateTime? ReopenedOn { get; set; }
            public string? ReopenedBy { get; set; }

            [StringLength(500)]
            public string? ReopenReason { get; set; }

            // ── System Audit Fields ──
            public DateTime CreatedAt { get; set; } = DateTime.Now;
            public string? CreatedBy { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public string? UpdatedBy { get; set; }
            public byte[]? RowVersion { get; set; }
            public bool IsDeleted { get; set; } = false;

            // ── Child: Included Transactions ──
            public List<GSTReturnRunTransactionModel> IncludedTransactions { get; set; } = new();
            // ── Child: Exceptions ──
            public List<GSTReturnRunExceptionModel> Exceptions { get; set; } = new();
        }

        public class GSTReturnRunTransactionModel
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public Guid GSTReturnRunId { get; set; }
            public Guid TaxTransactionId { get; set; }
            public string? TaxTransactionNumber { get; set; }
            public string InclusionStatus { get; set; } = "Included";

            [StringLength(500)]
            public string? ExclusionReason { get; set; }
        }

        public class GSTReturnRunExceptionModel
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public Guid GSTReturnRunId { get; set; }
            public string ReferenceType { get; set; } = string.Empty;
            public Guid ReferenceId { get; set; }
            public string? ReferenceNumber { get; set; }
            public string Severity { get; set; } = "Warning";
            public string Message { get; set; } = string.Empty;
        }
    }

    public static class ReturnRunStatusEnum
    {
        public const string Draft = "Draft";
        public const string Generated = "Generated";
        public const string Reviewed = "Reviewed";
        public const string Approved = "Approved";
        public const string Finalized = "Finalized";
        public const string Filed = "Filed";
        public const string Closed = "Closed";
        public const string Reopened = "Reopened";
        public const string Cancelled = "Cancelled";
    }

    public static class ReturnTypeEnum
    {
        public const string GSTR1Style = "GSTR1Style";
        public const string GSTR3BStyle = "GSTR3BStyle";
        public const string CombinedGSTPack = "CombinedGSTPack";
    }

    public static class FilingStatusEnum
    {
        public const string NotFiled = "NotFiled";
        public const string Prepared = "Prepared";
        public const string Filed = "Filed";
        public const string Acknowledged = "Acknowledged";
        public const string Rejected = "Rejected";
    }
}
