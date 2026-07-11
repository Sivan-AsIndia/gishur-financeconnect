using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    #region Model #10: Ledger

    /// <summary>
    /// Model #10: Ledger – The official accounting "book" for a company where postings are stored.
    /// JournalEntry/JournalLine = what user enters (input)
    /// GeneralLedgerEntry = what system stores after posting (output)
    /// Ledger = the container/book that holds those posted rows
    /// </summary>
    public class LedgerModel
    {
        // Section 1: Ledger Identity
        
        /// <summary>PK - hidden in UI</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Tenant identifier - hidden in UI</summary>
        public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

        /// <summary>Ledger Code - unique within company, max 15 chars, letters/numbers/-/_ only, auto uppercase</summary>
        [Required(ErrorMessage = "Ledger Code is required")]
        [StringLength(15, ErrorMessage = "Ledger Code cannot exceed 15 characters")]
        [RegularExpression(@"^[A-Z0-9_-]+$", ErrorMessage = "Only uppercase letters, numbers, - and _ allowed")]
        public string LedgerCode { get; set; } = string.Empty;

        /// <summary>Ledger Name - max 200 chars</summary>
        [Required(ErrorMessage = "Ledger Name is required")]
        [StringLength(200, ErrorMessage = "Ledger Name cannot exceed 200 characters")]
        public string LedgerName { get; set; } = string.Empty;

        /// <summary>FK → Company - required, must be Active company</summary>
        [Required(ErrorMessage = "Company is required")]
        public Guid? CompanyId { get; set; }
        public string? CompanyCode { get; set; }
        public string? CompanyName { get; set; }

        /// <summary>Notes/Description - max 500 chars</summary>
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        /// <summary>Default Ledger flag - only one default per company</summary>
        public bool IsDefaultLedger { get; set; } = false;

        // Section 2: Ledger Type & Currency Rules

        /// <summary>Ledger Type - Primary/Management/IFRS/Tax/Other</summary>
        [Required(ErrorMessage = "Ledger Type is required")]
        public string LedgerType { get; set; } = "";

        /// <summary>FK → Currency (Base Currency) - required, must match Company.BaseCurrencyId for Primary</summary>
        [Required(ErrorMessage = "Base Currency is required")]
        public Guid? BaseCurrencyId { get; set; }
        public string? BaseCurrencyCode { get; set; }
        public string? BaseCurrencyName { get; set; }

        /// <summary>FK → Currency (Reporting Currency) - optional, for reporting conversions</summary>
        public Guid? ReportingCurrencyId { get; set; }
        public string? ReportingCurrencyCode { get; set; }
        public string? ReportingCurrencyName { get; set; }

        /// <summary>Currency Mode - SingleCurrencyOnly/MultiCurrencyAllowed</summary>
        [Required(ErrorMessage = "Currency Mode is required")]
        public string CurrencyMode { get; set; } = "";

        /// <summary>Exchange Rate Source - Manual/API/BankRate/Custom</summary>
        public string? ExchangeRateSource { get; set; } = "";

        // Section 3: Posting Controls

        /// <summary>Allow Posting From Date - blocks postings before this date</summary>
        public DateTime? AllowPostingFromDate { get; set; }

        /// <summary>Allow Posting To Date - soft boundary</summary>
        public DateTime? AllowPostingToDate { get; set; }

        /// <summary>Lock Backdated Posting flag</summary>
        public bool LockBackDatedPosting { get; set; } = false;

        /// <summary>Backdated Posting Days Allowed - 0,7,15,30,60,90</summary>
        [Range(0, 90, ErrorMessage = "Backdated posting days must be between 0 and 90")]
        public int? BackdatedPostingDaysAllowed { get; set; }

        /// <summary>Future Posting Days Allowed - 0,7,15,30</summary>
        [Range(0, 30, ErrorMessage = "Future posting days must be between 0 and 30")]
        public int? FuturePostingDaysAllowed { get; set; }

        /// <summary>Require Approval Before Posting flag</summary>
        public bool RequireApprovalBeforePosting { get; set; } = false;

        /// <summary>Enforce Accounting Period Open flag</summary>
        public bool EnforceAccountingPeriodOpen { get; set; } = true;

        // Section 4: Reporting & Consolidation (Optional)

        /// <summary>FK → ConsolidationGroup - optional, future use</summary>
        public Guid? ConsolidationGroupId { get; set; }
        public string? ConsolidationGroupName { get; set; }

        /// <summary>Eligible for Consolidation flag</summary>
        public bool IsConsolidationEligible { get; set; } = true;

        // Section 5: Status & Governance

        /// <summary>Ledger Status - Draft/Active/Inactive</summary>
        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; } = "";

        /// <summary>Lock Status - System controlled</summary>
        public string LockStatus { get; set; } = LockStatuses.Unlocked;

        /// <summary>Lock/Inactivation Reason - required if locked or inactive</summary>
        [StringLength(300, ErrorMessage = "Lock Reason cannot exceed 300 characters")]
        public string? LockReason { get; set; }

        // Section 6: System Audit (Hidden)

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        // Helper Properties

        public bool IsActive => Status == LedgerStatus.Active;
        public bool CanEdit => Status == LedgerStatus.Draft && LockStatus == LockStatuses.Unlocked;
        public bool HasPostings => LockStatus == LockStatuses.LockedAfterPosting;
    }

    // Ledger Type Enum
    public static class LedgerTypes
    {
        public const string Primary = "Primary";
        public const string Management = "Management";
        public const string IFRS = "IFRS";
        public const string Tax = "Tax";
        public const string Other = "Other";

        public static readonly string[] All = new[] { Primary, Management, IFRS, Tax, Other };

        public static string GetDisplayName(string type) => type switch
        {
            Primary => "Primary (Statutory)",
            Management => "Management (Internal)",
            IFRS => "IFRS (Consolidation)",
            Tax => "Tax (Compliance)",
            Other => "Other",
            _ => type
        };
    }

    // Currency Mode Enum
    public static class CurrencyModes
    {
        public const string SingleCurrencyOnly = "SingleCurrencyOnly";
        public const string MultiCurrencyAllowed = "MultiCurrencyAllowed";

        public static readonly string[] All = new[] { SingleCurrencyOnly, MultiCurrencyAllowed };

        public static string GetDisplayName(string mode) => mode switch
        {
            SingleCurrencyOnly => "Single Currency Only",
            MultiCurrencyAllowed => "Multi-Currency Allowed",
            _ => mode
        };
    }

    // Exchange Rate Source Enum
    public static class ExchangeRateSources
    {
        public const string Manual = "Manual";
        public const string API = "API";
        public const string BankRate = "BankRate";
        public const string Custom = "Custom";

        public static readonly string[] All = new[] { Manual, API, BankRate, Custom };

        public static string GetDisplayName(string source) => source switch
        {
            Manual => "Manual Entry",
            API => "API (Live Rates)",
            BankRate => "Bank Rate",
            Custom => "Custom Source",
            _ => source
        };
    }

    // Ledger Status Enum
    public static class LedgerStatus
    {
        public const string Draft = "Draft";
        public const string Active = "Active";
        public const string Inactive = "Inactive";

        public static readonly string[] All = new[] { Draft, Active, Inactive };
    }

    // Lock Status Enum
    public static class LockStatuses
    {
        public const string Unlocked = "Unlocked";
        public const string LockedAfterPosting = "LockedAfterPosting";
        public const string LockedByController = "LockedByController";

        public static readonly string[] All = new[] { Unlocked, LockedAfterPosting, LockedByController };

        public static string GetDisplayName(string status) => status switch
        {
            Unlocked => "Unlocked",
            LockedAfterPosting => "Locked (After Posting)",
            LockedByController => "Locked (By Controller)",
            _ => status
        };
    }

    // Backdated Posting Days Options
    public static class BackdatedDaysOptions
    {
        public static readonly int[] All = new[] { 0, 7, 15, 30, 60, 90 };
    }

    // Future Posting Days Options
    public static class FutureDaysOptions
    {
        public static readonly int[] All = new[] { 0, 7, 15, 30 };
    }

    #endregion

    #region Model #11: OpeningBalance

    /// <summary>
    /// Model #11: OpeningBalance (Header) – Starting balance of Balance Sheet accounts at the beginning of a fiscal year.
    /// </summary>
    public class OpeningBalanceModel
    {
        // Section 1: Opening Balance Identity (Header)

        /// <summary>PK - hidden in UI</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Tenant identifier - hidden in UI</summary>
        public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

        /// <summary>Opening Balance Number - system generated, max 30 chars</summary>
        [Required]
        [StringLength(30)]
        public string OpeningBalanceNumber { get; set; } = string.Empty;

        /// <summary>FK → Company - required, must be Active</summary>
        [Required(ErrorMessage = "Company is required")]
        public Guid CompanyId { get; set; }
        public string? CompanyCode { get; set; }
        public string? CompanyName { get; set; }

        /// <summary>FK → Branch - required (Option 2), must belong to Company and be Active</summary>
        [Required(ErrorMessage = "Branch is required")]
        public Guid BranchId { get; set; }
        public string? BranchCode { get; set; }
        public string? BranchName { get; set; }

        /// <summary>FK → Ledger - required, must be Active and belong to Company</summary>
        [Required(ErrorMessage = "Ledger is required")]
        public Guid LedgerId { get; set; }
        public string? LedgerCode { get; set; }
        public string? LedgerName { get; set; }

        /// <summary>FK → FiscalYear - required</summary>
        [Required(ErrorMessage = "Fiscal Year is required")]
        public Guid FiscalYearId { get; set; }
        public string? FiscalYearCode { get; set; }
        public string? FiscalYearName { get; set; }

        /// <summary>FK → AccountingPeriod - required, first period of FiscalYear</summary>
        [Required(ErrorMessage = "Opening Period is required")]
        public Guid OpeningAccountingPeriodId { get; set; }
        public string? OpeningAccountingPeriodCode { get; set; }
        public string? OpeningAccountingPeriodName { get; set; }

        /// <summary>Opening Date - must fall inside OpeningAccountingPeriod date range</summary>
        [Required(ErrorMessage = "Opening Date is required")]
        public DateTime OpeningDate { get; set; } = DateTime.Today;

        /// <summary>Entry Mode - ManualEntry/BulkImport/MigrationMode</summary>
        [Required(ErrorMessage = "Entry Mode is required")]
        public string EntryMode { get; set; } = "";

        /// <summary>Notes - max 1000 chars</summary>
        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
        public string? Notes { get; set; }

        // Section 2: Scope & Controls

        /// <summary>Restrict to Balance Sheet Accounts Only - default ON</summary>
        public bool RestrictToBalanceSheetAccounts { get; set; } = true;

        /// <summary>Currency Mode - derived from Ledger.CurrencyMode</summary>
        public string? CurrencyMode { get; set; }

        // Section 3: Opening Balance Lines
        public List<OpeningBalanceLineModel> Lines { get; set; } = new();

        // Section 4: Review / Approval / Posting

        /// <summary>Total Debit - system computed</summary>
        public decimal TotalDebit => Lines.Sum(l => l.DebitAmountBase);

        /// <summary>Total Credit - system computed</summary>
        public decimal TotalCredit => Lines.Sum(l => l.CreditAmountBase);

        /// <summary>Difference - system computed (must be 0 to submit/approve/post)</summary>
        public decimal Difference => TotalDebit - TotalCredit;

        /// <summary>Is Balanced check</summary>
        public bool IsBalanced => Math.Abs(Difference) < 0.01m;

        /// <summary>Status - Draft/Submitted/Approved/Posted/Cancelled</summary>
        [Required]
        public string Status { get; set; } = OpeningBalanceStatus.Draft;

        /// <summary>Approved By - set when approved</summary>
        public string? ApprovedBy { get; set; }

        /// <summary>Approved At - set when approved</summary>
        public DateTime? ApprovedAt { get; set; }

        /// <summary>Posted By - set when posted</summary>
        public string? PostedBy { get; set; }

        /// <summary>Posted At - set when posted</summary>
        public DateTime? PostedAt { get; set; }

        /// <summary>Posting Reference - link to JournalEntry created on posting</summary>
        public string? PostingReference { get; set; }

        // Section 5: System Audit (Hidden)

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        // Helper Properties

        public bool CanEdit => Status == OpeningBalanceStatus.Draft;
        public bool CanSubmit => Status == OpeningBalanceStatus.Draft && IsBalanced && Lines.Any();
        public bool CanApprove => Status == OpeningBalanceStatus.Submitted;
        public bool CanPost => Status == OpeningBalanceStatus.Approved;
    }

    /// <summary>
    /// Model #11: OpeningBalanceLine – Individual account balance line in opening balance.
    /// </summary>
    public class OpeningBalanceLineModel
    {
        /// <summary>PK - hidden in UI</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>FK → OpeningBalance header</summary>
        [Required]
        public Guid OpeningBalanceId { get; set; }

        /// <summary>FK → Account - required, must be Active, Postable, and Balance Sheet type if restricted</summary>
        [Required(ErrorMessage = "Account is required")]
        public Guid AccountId { get; set; }
        public string? AccountCode { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNature { get; set; }

        /// <summary>Line Description - max 300 chars</summary>
        [StringLength(300, ErrorMessage = "Description cannot exceed 300 characters")]
        public string? LineDescription { get; set; }

        /// <summary>Debit Amount (Base Currency) - either debit or credit must be > 0</summary>
        [Range(0, double.MaxValue, ErrorMessage = "Debit amount must be >= 0")]
        public decimal DebitAmountBase { get; set; }

        /// <summary>Credit Amount (Base Currency) - either debit or credit must be > 0</summary>
        [Range(0, double.MaxValue, ErrorMessage = "Credit amount must be >= 0")]
        public decimal CreditAmountBase { get; set; }

        /// <summary>FK → Currency (Transaction Currency) - optional, only if multi-currency</summary>
        public Guid? TransactionCurrencyId { get; set; }
        public string? TransactionCurrencyCode { get; set; }

        /// <summary>Transaction Amount - only if TransactionCurrencyId != BaseCurrencyId</summary>
        public decimal? TransactionAmount { get; set; }

        /// <summary>Exchange Rate Used - only if TransactionCurrencyId != BaseCurrencyId</summary>
        public decimal? ExchangeRateUsed { get; set; }

        // Validation helper
        public bool IsValid => (DebitAmountBase > 0 && CreditAmountBase == 0) || (DebitAmountBase == 0 && CreditAmountBase > 0);
    }

    // Entry Mode Enum
    public static class EntryModes
    {
        public const string ManualEntry = "ManualEntry";
        public const string BulkImport = "BulkImport";
        public const string MigrationMode = "MigrationMode";

        public static readonly string[] All = new[] { ManualEntry, BulkImport, MigrationMode };

        public static string GetDisplayName(string mode) => mode switch
        {
            ManualEntry => "Manual Entry",
            BulkImport => "Bulk Import",
            MigrationMode => "Migration Mode",
            _ => mode
        };
    }

    // Opening Balance Status Enum
    public static class OpeningBalanceStatus
    {
        public const string Draft = "Draft";
        public const string Submitted = "Submitted";
        public const string Approved = "Approved";
        public const string Posted = "Posted";
        public const string Cancelled = "Cancelled";

        public static readonly string[] All = new[] { Draft, Submitted, Approved, Posted, Cancelled };
    }

    #endregion

    #region Model #16: ClosingBalance

    /// <summary>
    /// Model #16: ClosingBalance – Final balance of every Account for a given Company + Branch + Ledger + AccountingPeriod.
    /// This is a READ-ONLY view - balances are created by system closing process, not manual entry.
    /// </summary>
    public class ClosingBalanceModel
    {
        // Section 1: Core Identity (Immutable Keys)

        /// <summary>PK - hidden in UI</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Tenant identifier - hidden in UI</summary>
        public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

        /// <summary>FK → Company - read-only filter</summary>
        [Required]
        public Guid CompanyId { get; set; }
        public string? CompanyCode { get; set; }
        public string? CompanyName { get; set; }

        /// <summary>FK → Branch - read-only filter (mandatory)</summary>
        [Required]
        public Guid BranchId { get; set; }
        public string? BranchCode { get; set; }
        public string? BranchName { get; set; }

        /// <summary>FK → Ledger - read-only filter</summary>
        [Required]
        public Guid LedgerId { get; set; }
        public string? LedgerCode { get; set; }
        public string? LedgerName { get; set; }

        /// <summary>FK → FiscalYear - read-only</summary>
        [Required]
        public Guid FiscalYearId { get; set; }
        public string? FiscalYearCode { get; set; }
        public string? FiscalYearName { get; set; }

        /// <summary>FK → AccountingPeriod - read-only</summary>
        [Required]
        public Guid AccountingPeriodId { get; set; }
        public string? AccountingPeriodCode { get; set; }
        public string? AccountingPeriodName { get; set; }

        /// <summary>FK → Account - read-only row</summary>
        [Required]
        public Guid AccountId { get; set; }
        public string? AccountCode { get; set; }
        public string? AccountName { get; set; }

        // Section 2: Balance Breakdown (The real accounting truth)

        /// <summary>Opening Debit - read-only</summary>
        [Range(0, double.MaxValue)]
        public decimal OpeningDebit { get; set; }

        /// <summary>Opening Credit - read-only</summary>
        [Range(0, double.MaxValue)]
        public decimal OpeningCredit { get; set; }

        /// <summary>Period Debit - sum of GL debits within this period</summary>
        [Range(0, double.MaxValue)]
        public decimal PeriodDebit { get; set; }

        /// <summary>Period Credit - sum of GL credits within this period</summary>
        [Range(0, double.MaxValue)]
        public decimal PeriodCredit { get; set; }

        /// <summary>Closing Debit - read-only</summary>
        [Range(0, double.MaxValue)]
        public decimal ClosingDebit { get; set; }

        /// <summary>Closing Credit - read-only</summary>
        [Range(0, double.MaxValue)]
        public decimal ClosingCredit { get; set; }

        /// <summary>Closing Balance Side - Debit/Credit/Zero</summary>
        [Required]
        public string ClosingBalanceSide { get; set; } = BalanceSides.Zero;

        /// <summary>Closing Balance Amount - absolute final number (positive only)</summary>
        [Range(0, double.MaxValue)]
        public decimal ClosingBalanceAmount { get; set; }

        // Section 3: Currency Context

        /// <summary>FK → Currency (Base Currency) - always Company.BaseCurrencyId</summary>
        [Required]
        public Guid BaseCurrencyId { get; set; }
        public string? BaseCurrencyCode { get; set; }
        public string? BaseCurrencyName { get; set; }

        // Section 4: Closing Run Metadata (Control + Audit)

        /// <summary>Close Run Id - groups all ClosingBalance rows created in a single closing operation</summary>
        [Required]
        public Guid CloseRunId { get; set; }

        /// <summary>Close Status - Calculated/Verified/Locked/Reversed</summary>
        [Required]
        public string CloseStatus { get; set; } = CloseStatuses.Calculated;

        /// <summary>Calculated At - timestamp</summary>
        public DateTime CalculatedAt { get; set; }

        /// <summary>Calculated By - who ran the calculation</summary>
        public string? CalculatedBy { get; set; }

        /// <summary>Locked At - timestamp when locked</summary>
        public DateTime? LockedAt { get; set; }

        /// <summary>Locked By - who locked</summary>
        public string? LockedBy { get; set; }

        /// <summary>Recalculated From Close Run Id - only when period reopened</summary>
        public Guid? RecalculatedFromCloseRunId { get; set; }

        /// <summary>Recalculation Reason - only if recalculated</summary>
        [StringLength(500)]
        public string? RecalculationReason { get; set; }

        // Section 5: Snapshot Fields (Recommended for long-term audit)

        /// <summary>Account Code Snapshot</summary>
        [Required]
        [StringLength(50)]
        public string AccountCodeSnapshot { get; set; } = string.Empty;

        /// <summary>Account Name Snapshot</summary>
        [Required]
        [StringLength(200)]
        public string AccountNameSnapshot { get; set; } = string.Empty;

        /// <summary>Branch Code Snapshot</summary>
        [Required]
        [StringLength(20)]
        public string BranchCodeSnapshot { get; set; } = string.Empty;

        /// <summary>Branch Name Snapshot</summary>
        [Required]
        [StringLength(200)]
        public string BranchNameSnapshot { get; set; } = string.Empty;

        // Section 6: System Fields (Hidden)

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Display Helpers

        /// <summary>Opening Balance (Net) - for display</summary>
        public decimal OpeningBalance => OpeningDebit - OpeningCredit;

        /// <summary>Period Movement (Net) - for display</summary>
        public decimal PeriodMovement => PeriodDebit - PeriodCredit;

        /// <summary>Formatted Opening Debit</summary>
        public string FormattedOpeningDebit => OpeningDebit > 0 ? OpeningDebit.ToString("N2") : "-";

        /// <summary>Formatted Opening Credit</summary>
        public string FormattedOpeningCredit => OpeningCredit > 0 ? OpeningCredit.ToString("N2") : "-";

        /// <summary>Formatted Period Debit</summary>
        public string FormattedPeriodDebit => PeriodDebit > 0 ? PeriodDebit.ToString("N2") : "-";

        /// <summary>Formatted Period Credit</summary>
        public string FormattedPeriodCredit => PeriodCredit > 0 ? PeriodCredit.ToString("N2") : "-";

        /// <summary>Formatted Closing Debit</summary>
        public string FormattedClosingDebit => ClosingDebit > 0 ? ClosingDebit.ToString("N2") : "-";

        /// <summary>Formatted Closing Credit</summary>
        public string FormattedClosingCredit => ClosingCredit > 0 ? ClosingCredit.ToString("N2") : "-";
    }

    // Balance Side Enum
    public static class BalanceSides
    {
        public const string Debit = "Debit";
        public const string Credit = "Credit";
        public const string Zero = "Zero";

        public static readonly string[] All = new[] { Debit, Credit, Zero };
    }

    // Close Status Enum
    public static class CloseStatuses
    {
        public const string Calculated = "Calculated";
        public const string Verified = "Verified";
        public const string Locked = "Locked";
        public const string Reversed = "Reversed";

        public static readonly string[] All = new[] { Calculated, Verified, Locked, Reversed };

        public static string GetDisplayName(string status) => status switch
        {
            Calculated => "Calculated",
            Verified => "Verified",
            Locked => "Locked (Final)",
            Reversed => "Reversed (Historical)",
            _ => status
        };
    }

    #endregion

    #region Account Model (for Opening Balance lines)

    /// <summary>
    /// Simplified Account Model for use in Opening Balance screens.
    /// In a real application, this would come from Chart of Accounts.
    /// </summary>
    //public class AccountViewModel
    //{
    //    public Guid Id { get; set; } = Guid.NewGuid();
    //    public string AccountCode { get; set; } = string.Empty;
    //    public string AccountName { get; set; } = string.Empty;
    //    public string AccountType { get; set; } = "Asset"; // Asset, Liability, Equity, Revenue, Expense
    //    public string AccountNature { get; set; } = "Asset"; // Asset, Liability, Equity (for balance sheet classification)
    //    public bool IsBalanceSheetAccount { get; set; } = true;
    //    public bool IsActive { get; set; } = true;
    //    public bool IsPostable { get; set; } = true;
    //    public string Status { get; set; } = "Active";
    //}

    #endregion
}
