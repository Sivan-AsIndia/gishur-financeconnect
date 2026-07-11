using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{

    
    public enum ImportSourceType
    {
        ManualUpload = 1,
        SFTP = 2,
        BankAPI = 3,
        EmailIngestion = 4,
        SystemJob = 5
    }

    public enum StatementProfileType
    {
        HDFC_CSV_V1 = 1,
        SBI_CSV_V1 = 2,
        ICICI_XLSX_V1 = 3,
        Generic_CSV = 4,
        OFX = 5,
        MT940 = 6
    }

    public enum StatementStatusType
    {
        Uploaded = 1,
        ParsingInProgress = 2,
        Parsed = 3,
        ValidationFailed = 4,
        ReadyForReconciliation = 5,
        Locked = 6,
        Archived = 7,
        Superseded = 8
    }

    public enum BalanceCheckStatusType
    {
        NotAvailable = 1,
        Matched = 2,
        Mismatch = 3
    }
    public class BankStatementModel
    {
        // ================= CORE IDENTITY =================
        public Guid BankStatementId { get; set; }
        public Guid TenantId { get; set; }

        [Required(ErrorMessage = "Company is required")]
        public Guid CompanyId { get; set; }

        [Required(ErrorMessage = "Branch is required")]
        public Guid? BranchId { get; set; }

        [Required(ErrorMessage = "Bank Account is required")]
        public Guid BankAccountId { get; set; }

        public string StatementNumber { get; set; } = string.Empty;
        public StatementStatusType StatementStatus { get; set; }

        // Display helpers
        public string BankAccountName { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;

        // ================= FILE METADATA =================

        [Required(ErrorMessage = "Import Source is required")]
        public ImportSourceType? ImportSource { get; set; }

        [Required(ErrorMessage = "File is required")]
        public string FileNameOriginal { get; set; } = string.Empty;

        [Required]
        public string FileType { get; set; } = string.Empty;

        [Required]
        public long FileSizeBytes { get; set; }
        public string FileStoragePath { get; set; } = string.Empty;
        public string FileHashSHA256 { get; set; } = string.Empty;
        public DateTime FileUploadedAt { get; set; }
        public string FileUploadedBy { get; set; } = string.Empty;

        // ================= STATEMENT COVERAGE =================
        public DateTime StatementFromDate { get; set; }
        public DateTime StatementToDate { get; set; }
        public Guid CurrencyId { get; set; }

        public decimal? OpeningBalance { get; set; }
        public decimal? ClosingBalance { get; set; }

        public decimal TotalCreditsAmount { get; set; }
        public decimal TotalDebitsAmount { get; set; }
        public decimal NetMovementAmount { get; set; }

        public BalanceCheckStatusType BalanceCheckStatus { get; set; }
        public decimal? BalanceDifferenceAmount { get; set; }

        // ================= PARSING PROFILE =================

        [Required(ErrorMessage = "Statement Profile is required")]
        public StatementProfileType? StatementProfile { get; set; }

        [Required(ErrorMessage = "Profile Version is required")]
        public string ProfileVersion { get; set; } = string.Empty;
        public string ParsingSettingsSnapshotJson { get; set; } = string.Empty;

        // ================= LINE METRICS =================
        public int TotalLineCount { get; set; }
        public int ParsedSuccessLineCount { get; set; }
        public int ParsedFailedLineCount { get; set; }
        public int DuplicateLineCountInFile { get; set; }
        public DateTime? FirstTransactionDateInFile { get; set; }
        public DateTime? LastTransactionDateInFile { get; set; }

        // ================= PROCESS LOGS =================
        public string ProcessingStatusMessage { get; set; } = string.Empty;
        public string ErrorSummary { get; set; } = string.Empty;
        public DateTime? ParseStartedAt { get; set; }
        public DateTime? ParseCompletedAt { get; set; }
        public DateTime? ValidationStartedAt { get; set; }
        public DateTime? ValidationCompletedAt { get; set; }
        public string ProcessedByJobId { get; set; } = string.Empty;

        // ================= GOVERNANCE =================
        public bool IsUsedInReconciliation { get; set; }
        public Guid? FirstUsedReconciliationId { get; set; }

        public string LockedBy { get; set; } = string.Empty;
        public DateTime? LockedOn { get; set; }

        public string ArchivedBy { get; set; } = string.Empty;
        public DateTime? ArchivedOn { get; set; }

        public Guid? SupersededByStatementId { get; set; }
        public string SupersedeReason { get; set; } = string.Empty;

        // ================= SYSTEM AUDIT =================
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;

        public string RowVersion { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
    }

    public class PreviewLine
    {
        public string Date { get; set; } = "";
        public string Narration { get; set; } = "";
        public string Debit { get; set; } = "";
        public string Credit { get; set; } = "";
        public string Balance { get; set; } = "";
    }

    public class BankStatementStatistics
    {
        public int TotalStatements { get; set; }
        public int UploadedStatements { get; set; }
        public int ReadyForReconciliationStatements { get; set; }
        public int LockedStatements { get; set; }
        public int ArchivedStatements { get; set; }
        public int SupersededStatements { get; set; }

    }
}
