using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    #region Model #7: ChartOfAccounts

    /// <summary>
    /// Model #7: ChartOfAccounts - The master "account dictionary" for a company
    /// Defines which accounts exist and how they are organized
    /// </summary>
    public class ChartOfAccountsViewModel
    {
        // Section 1: COA Identity
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

        [Required(ErrorMessage = "Chart Code is required")]
        [StringLength(20, ErrorMessage = "Chart Code cannot exceed 20 characters")]
        [RegularExpression(@"^[A-Z0-9_-]+$", ErrorMessage = "Only uppercase letters, numbers, - and _ allowed")]
        public string ChartCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Chart Name is required")]
        [StringLength(200, ErrorMessage = "Chart Name cannot exceed 200 characters")]
        public string ChartName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Company is required")]
        public Guid? CompanyId { get; set; }
        public string? CompanyCode { get; set; }
        public string? CompanyName { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        // Section 2: Scope & Defaults
        [Required(ErrorMessage = "Chart Type is required")]
        public string ChartType { get; set; } = "";

        public bool IsDefaultForCompany { get; set; } = false;

        [Required(ErrorMessage = "Effective From date is required")]
        public DateTime EffectiveFrom { get; set; } = DateTime.Today;

        public DateTime? EffectiveTo { get; set; }

        // Section 3: Account Code Rules
        [Required(ErrorMessage = "Account Code Mode is required")]
        public string AccountCodeMode { get; set; } = "";

        [StringLength(50, ErrorMessage = "Account Code Format cannot exceed 50 characters")]
        public string? AccountCodeFormat { get; set; }

        public int? NextAccountNumber { get; set; }

        public bool EnforceUniqueAccountCode { get; set; } = true;

        public bool EnforceUniqueAccountName { get; set; } = false;

        public bool AllowAccountCodeReuseAfterInactivation { get; set; } = false;

        // Section 4: Posting & Governance Controls
        public bool ChangeRequestRequired { get; set; } = true;

        // Section 5: Import / Template Setup
        public string TemplateSource { get; set; } = "";

        [StringLength(100, ErrorMessage = "Template Reference cannot exceed 100 characters")]
        public string? TemplateReferenceId { get; set; }

        // Section 6: Status Governance
        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; }

        public DateTime? LockedAt { get; set; }
        public string? LockedBy { get; set; }
        public string? LockReason { get; set; }
        public DateTime? RetiredAt { get; set; }
        public string? RetiredBy { get; set; }
        public string? RetirementReason { get; set; }

        // Section 5: Audit Fields
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        // Computed/Display Properties
        public int AccountGroupCount { get; set; }
        public int AccountCount { get; set; }
    }

    #endregion

    #region Model #8: AccountGroup

    /// <summary>
    /// Model #8: AccountGroup - Hierarchical grouping of accounts
    /// Organizes accounts into categories like Assets, Liabilities, etc.
    /// </summary>
    public class AccountGroupViewModel
    {
        // Section 1: Group Identity
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

        [Required(ErrorMessage = "Chart of Accounts is required")]
        public Guid ChartOfAccountsId { get; set; }
        public string? ChartOfAccountsCode { get; set; }
        public string? ChartOfAccountsName { get; set; }

        [Required(ErrorMessage = "Group Code is required")]
        [StringLength(20, ErrorMessage = "Group Code cannot exceed 20 characters")]
        public string GroupCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Group Name is required")]
        [StringLength(200, ErrorMessage = "Group Name cannot exceed 200 characters")]
        public string GroupName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        // Section 2: Classification
        [Required(ErrorMessage = "Account Nature is required")]
        public string AccountNature { get; set; } = "";

        public string? StatementType { get; set; }

        [Required(ErrorMessage = "Normal Balance Type is required")]
        public string BalanceBehavior { get; set; } = "";

        public bool IsControlGroup { get; set; } = false;

        public string? ReportingCategory { get; set; }

        public Guid? ParentGroupId { get; set; }
        public string? ParentGroupCode { get; set; }
        public string? ParentGroupName { get; set; }

        public int HierarchyLevel { get; set; } = 0;
        public string? HierarchyPath { get; set; }

        // Section 3: Default Behavior
        public bool DefaultIsPostable { get; set; } = true;
        public bool DefaultRequiresBranch { get; set; } = true;
        public bool DefaultRequiresCostCenter { get; set; } = false;
        public bool DefaultAllowManualJournal { get; set; } = true;
        public string? DefaultCurrencyBehavior { get; set; }

        // Section 4: Status & Display
        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; } = GroupStatuses.Active;

        [StringLength(300, ErrorMessage = "Inactivation Reason cannot exceed 300 characters")]
        public string? LockReason { get; set; }

        public int DisplayOrder { get; set; } = 0;

        // Section 5: Audit Fields
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        // Computed Properties
        public int AccountCount { get; set; }
        public int ChildGroupCount { get; set; }
    }

    #endregion

    #region Model #9: Account (GL Account)

    /// <summary>
    /// Model #9: Account (GL Account) - Individual account for recording transactions
    /// The actual "notebooks" where financial transactions are recorded
    /// </summary>
    public class AccountViewModel
    {
        // Section 1: Account Identity
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

        public Guid? ChartOfAccountsId { get; set; }
        public string? ChartOfAccountsCode { get; set; }
        public string? ChartOfAccountsName { get; set; }

        [Required(ErrorMessage = "Account Group is required")]
        public Guid AccountGroupId { get; set; }
        public string? AccountGroupCode { get; set; }
        public string? AccountGroupName { get; set; }

        [Required(ErrorMessage = "Account Code is required")]
        [StringLength(20, ErrorMessage = "Account Code cannot exceed 20 characters")]
        public string AccountCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Account Name is required")]
        [StringLength(200, ErrorMessage = "Account Name cannot exceed 200 characters")]
        public string AccountName { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Account Alias cannot exceed 50 characters")]
        public string? AccountAlias { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        /// <summary>Parent Account (Optional hierarchy at account level) - must be Header (non-postable)</summary>
        public Guid? ParentAccountId { get; set; }
        public string? ParentAccountName { get; set; }

        /// <summary>Display order for sorting within group</summary>
        public int DisplayOrder { get; set; } = 0;

        // Section 2: Classification & Reporting
        [Required(ErrorMessage = "Account Nature is required")]
        public string AccountNature { get; set; } = "";

        /// <summary>Account Type - Asset, Liability, Equity, Revenue, Expense (for compatibility)</summary>
        public string AccountType { get; set; } = "Asset";

        /// <summary>Derived: BalanceSheet or ProfitAndLoss based on AccountNature</summary>
        public string? StatementType { get; set; }

        /// <summary>Derived: Debit or Credit based on AccountNature</summary>
        public string? NormalBalance { get; set; }

        /// <summary>Whether this is a Balance Sheet account</summary>
        public bool IsBalanceSheetAccount { get; set; } = true;

        /// <summary>Reporting Category for P&L layout customization</summary>
        public string? ReportingCategory { get; set; }

        // Section 3: Posting Rules & Controls
        public bool IsPostable { get; set; } = true;
        public bool IsControlAccount { get; set; } = false;

        /// <summary>Control Account Type - Required if IsControlAccount is true</summary>
        public string? ControlAccountType { get; set; }

        public Guid? ControlledByAccountId { get; set; }
        public bool RequiresBranch { get; set; } = true;
        public bool AllowManualJournal { get; set; } = true;
        public bool IsReconcilable { get; set; } = false;

        /// <summary>Cost Center mandatory for posting (future feature)</summary>
        public bool RequiresCostCenter { get; set; } = false;

        /// <summary>Allow backdated posting override by controller only</summary>
        public bool AllowBackdatedPostingOverride { get; set; } = false;

        // Section 4: Special Account Types
        public bool IsBankAccount { get; set; } = false;
        public bool IsCashAccount { get; set; } = false;
        public bool IsTaxAccount { get; set; } = false;

        // Bank Details (if IsBankAccount)
        public string? BankName { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankBranch { get; set; }
        public string? SwiftCode { get; set; }
        public string? IBAN { get; set; }
        public string? RoutingNumber { get; set; }

        // Tax Details (if IsTaxAccount)
        public string? TaxType { get; set; }
        public decimal? TaxRate { get; set; }
        public string? TaxRegistrationNumber { get; set; }

        // Section 5: Status & Validity
        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; } = AccountStatuses.Active;

        /// <summary>Lock Status - Read-only, set by system after posting</summary>
        public string? LockStatus { get; set; }

        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }

        public DateTime? LockedAt { get; set; }
        public string? LockedBy { get; set; }

        [StringLength(300, ErrorMessage = "Lock Reason cannot exceed 300 characters")]
        public string? LockReason { get; set; }

        // Section 6: Audit Fields
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        // Computed Properties
        public decimal Balance { get; set; }
        public int TransactionCount { get; set; }
    }

    #endregion

    #region Static Constant Classes

    /// <summary>
    /// Chart Types - Types of Chart of Accounts
    /// </summary>
    public static class ChartTypes
    {
        public const string Standard = "Standard";
        public const string Template = "Template";
        public const string Migration = "Migration";

        public static readonly List<string> All = new() { Standard, Template, Migration };
    }

    /// <summary>
    /// COA Statuses - Lifecycle states for Chart of Accounts
    /// </summary>
    public static class COAStatuses
    {
        public const string Draft = "Draft";
        public const string Active = "Active";
        public const string Locked = "Locked";
        public const string Retired = "Retired";

        public static readonly List<string> All = new() { Draft, Active, Locked, Retired };
    }

    /// <summary>
    /// Account Code Modes - How account codes are generated
    /// </summary>
    public static class AccountCodeModes
    {
        public const string Manual = "Manual";
        public const string AutoNumber = "AutoNumber";
        public const string Hybrid = "Hybrid";

        public static readonly List<string> All = new() { Manual, AutoNumber, Hybrid };
    }

    /// <summary>
    /// Account Natures - The five fundamental account types
    /// </summary>
    public static class AccountNatures
    {
        public const string Asset = "Asset";
        public const string Liability = "Liability";
        public const string Equity = "Equity";
        public const string Income = "Income";
        public const string Expense = "Expense";

        public static readonly List<string> All = new() { Asset, Liability, Equity, Income, Expense };
    }

    /// <summary>
    /// Statement Types - Which financial statement the account belongs to
    /// </summary>
    public static class StatementTypes
    {
        public const string BalanceSheet = "BalanceSheet";
        public const string ProfitAndLoss = "ProfitAndLoss";

        public static readonly List<string> All = new() { BalanceSheet, ProfitAndLoss };
    }

    /// <summary>
    /// Balance Behaviors - Normal balance direction
    /// </summary>
    public static class BalanceBehaviors
    {
        public const string Debit = "Debit";
        public const string Credit = "Credit";

        public static readonly List<string> All = new() { Debit, Credit };
    }

    /// <summary>
    /// Group Statuses - Lifecycle states for Account Groups
    /// </summary>
    public static class GroupStatuses
    {
        public const string Draft = "Draft";
        public const string Active = "Active";
        public const string Inactive = "Inactive";

        public static readonly List<string> All = new() { Draft, Active, Inactive };
    }

    /// <summary>
    /// Account Statuses - Lifecycle states for GL Accounts
    /// </summary>
    public static class AccountStatuses
    {
        public const string Draft = "Draft";
        public const string Active = "Active";
        public const string Inactive = "Inactive";
        public const string Suspended = "Suspended";
        public const string Closed = "Closed";

        public static readonly List<string> All = new() { Draft, Active, Inactive, Suspended, Closed };
    }

    /// <summary>
    /// Template Sources - How the COA was created/imported
    /// </summary>
    public static class TemplateSources
    {
        public const string None = "None";
        public const string SystemTemplate = "SystemTemplate";
        public const string CloneFromCompany = "CloneFromCompany";
        public const string ExcelImport = "ExcelImport";

        public static readonly List<string> All = new() { None, SystemTemplate, CloneFromCompany, ExcelImport };

        public static string GetDisplayName(string value)
        {
            return value switch
            {
                SystemTemplate => "System Template",
                CloneFromCompany => "Clone from Another Company",
                ExcelImport => "Excel Import",
                _ => "None"
            };
        }
    }

    /// <summary>
    /// Reporting Categories - P&L presentation grouping
    /// </summary>
    public static class ReportingCategories
    {
        public const string OperatingRevenue = "OperatingRevenue";
        public const string DirectExpense = "DirectExpense";
        public const string IndirectExpense = "IndirectExpense";
        public const string OtherIncome = "OtherIncome";
        public const string OtherExpense = "OtherExpense";

        public static readonly List<string> All = new() { OperatingRevenue, DirectExpense, IndirectExpense, OtherIncome, OtherExpense };

        public static string GetDisplayName(string value)
        {
            return value switch
            {
                OperatingRevenue => "Operating Revenue",
                DirectExpense => "Direct Expense",
                IndirectExpense => "Indirect Expense",
                OtherIncome => "Other Income",
                OtherExpense => "Other Expense",
                _ => value
            };
        }
    }

    /// <summary>
    /// Control Account Types - What kind of subledger controls this account
    /// </summary>
    public static class ControlAccountTypes
    {
        public const string AccountsReceivable = "AccountsReceivable";
        public const string AccountsPayable = "AccountsPayable";
        public const string TaxPayable = "TaxPayable";
        public const string PayrollPayable = "PayrollPayable";
        public const string Other = "Other";

        public static readonly List<string> All = new() { AccountsReceivable, AccountsPayable, TaxPayable, PayrollPayable, Other };

        public static string GetDisplayName(string value)
        {
            return value switch
            {
                AccountsReceivable => "Accounts Receivable",
                AccountsPayable => "Accounts Payable",
                TaxPayable => "Tax Payable",
                PayrollPayable => "Payroll Payable",
                Other => "Other",
                _ => value
            };
        }
    }

    /// <summary>
    /// Account Tax Types - Types of tax accounts (account-level classification)
    /// </summary>
    public static class AccountTaxTypes
    {
        public const string GSTOutput = "GSTOutput";
        public const string GSTInput = "GSTInput";
        public const string TDS = "TDS";
        public const string VAT = "VAT";
        public const string ServiceTax = "ServiceTax";
        public const string Other = "Other";

        public static readonly List<string> All = new() { GSTOutput, GSTInput, TDS, VAT, ServiceTax, Other };

        public static string GetDisplayName(string value)
        {
            return value switch
            {
                GSTOutput => "GST Output",
                GSTInput => "GST Input",
                TDS => "TDS",
                VAT => "VAT",
                ServiceTax => "Service Tax",
                Other => "Other",
                _ => value
            };
        }
    }

    #endregion
}
