using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public enum ScopeType
    {
        PeriodRange,
        AsOfDate
    }

    public enum RunType
    {
        Daily,
        Weekly,
        Monthly,
        OnDemand,
        MonthEndFinal
    }

    public enum StatementSelectionModeType
    {
        ByStatementFile,
        ByDateRange
    }

    public enum ReconciliationStatus
    {
        Draft,
        InProgress,
        Completed,
        Finalized,
        Reopened,
        Cancelled,
        Failed
    }

    public enum ReferenceMatchMode
    {
        None = 0,
        MatchUTR = 1,
        MatchChequeNo = 2,
        MatchNarrationContains = 4,
        MatchTransactionNumber = 8
    }
    public class BankReconciliationModel
    {
        // ================= CORE =================
        public Guid BankReconciliationId { get; set; }
        public Guid TenantId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid? BranchId { get; set; }

        [Required(ErrorMessage = "Bank Account is required")]
        public Guid BankAccountId { get; set; }

        [Required(ErrorMessage = "Reconciliation Number is required")]
        public string ReconciliationNumber { get; set; } = "";

        [Required(ErrorMessage = "Run Type is required")]
        public RunType? RunType { get; set; }
        public ReconciliationStatus ReconciliationStatus { get; set; }

        // ================= SCOPE =================

        [Required(ErrorMessage = "Scope Type is required")]
        public ScopeType? ScopeType { get; set; }
        [Required(ErrorMessage = "FromDate is required")]
        public DateTime? FromDate { get; set; }
        [Required(ErrorMessage = "ToDate is required")]
        public DateTime? ToDate { get; set; }

        public DateTime? AsOfDate { get; set; }

        public Guid? AccountingPeriodId { get; set; }

        // ================= STATEMENT LINK =================
        [Required(ErrorMessage = "Statement Selection Mode is required")]
        public StatementSelectionModeType? StatementSelectionMode { get; set; }
        public Guid? PrimaryBankStatementId { get; set; }

        public DateTime? StatementFromDateSnapshot { get; set; }
        public DateTime? StatementToDateSnapshot { get; set; }
        public int StatementLineCountSnapshot { get; set; }

        // ================= BALANCES =================
        public decimal OpeningBalance_Statement { get; set; }
        public decimal ClosingBalance_Statement { get; set; }

        public decimal OpeningBalance_Book { get; set; }
        public decimal ClosingBalance_Book { get; set; }

        public decimal DifferenceAmount { get; set; }
        public bool IsDifferenceWithinTolerance { get; set; }
        public decimal ToleranceAmount { get; set; }

        // ================= MATCH SETTINGS SNAPSHOT =================
        public bool AutoMatchEnabled { get; set; } = true;
        public int DateWindowDays { get; set; } = 2;
        public decimal AmountTolerance { get; set; } = 0;

        public bool MatchUTR { get; set; } = true;
        public bool MatchChequeNo { get; set; } = true;
        public bool MatchNarrationContains { get; set; } = true;
        public ReferenceMatchMode ReferenceMatchMode { get; set; } = ReferenceMatchMode.None;
        public bool AllowManyToManyMatching { get; set; }
        public bool AllowAdjustmentCreationFromRecon { get; set; }

        // ================= TOTALS =================
        public decimal TotalStatementCredits { get; set; }
        public decimal TotalStatementDebits { get; set; }
        public decimal TotalBookInflows { get; set; }
        public decimal TotalBookOutflows { get; set; }

        public decimal MatchedStatementAmount { get; set; }
        public decimal MatchedBookAmount { get; set; }
        public decimal UnmatchedStatementAmount { get; set; }
        public decimal UnmatchedBookAmount { get; set; }

        public int MatchedCount { get; set; }
        public int UnmatchedStatementCount { get; set; }
        public int UnmatchedBookCount { get; set; }

        public int UnknownItemCount { get; set; }
        public int OutstandingChequeCount { get; set; }
        public int DepositInTransitCount { get; set; }

        // ================= WORKFLOW =================
        public string PreparedBy { get; set; } = "";
        public DateTime PreparedOn { get; set; }

        public string? FinalizedBy { get; set; }
        public DateTime? FinalizedOn { get; set; }
        public string? FinalizeNotes { get; set; }

        public string? ReopenedBy { get; set; }
        public DateTime? ReopenedOn { get; set; }
        public string? ReopenReason { get; set; }

        // ================= SYSTEM =================
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = "";

        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }
    }


    public class BankReconciliationMatchModel
    {
        public Guid BankReconciliationMatchId { get; set; }
        public Guid BankReconciliationId { get; set; }

        public Guid BankStatementLineId { get; set; }
        public Guid BankTransactionId { get; set; }

        public decimal MatchedAmount { get; set; }

        public string MatchType { get; set; } = "Manual"; // Auto / Manual
        public int ConfidenceScore { get; set; }

        public string MatchedBy { get; set; } = "";
        public DateTime MatchedOn { get; set; }
    }

    public class MatchSuggestionView
    {
        public Guid StatementLineId { get; set; }
        public Guid TransactionId { get; set; }

        public string TransactionRef { get; set; } = "";
        public int ConfidenceScore { get; set; }
    }

    public class BankReconciliationStatistics
    {
        public int TotalReconciliation { get; set; }
        public int DraftReconciliation { get; set; }
        public int InprogressReconciliation { get; set; }
        public int CompletedReconciliation { get; set; }
        public int FinalizedReconciliation { get; set; }
        public int ReopenedReconciliation { get; set; }

    }
}

