using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    #region Customer

    /// <summary>
    /// Customer – The AR master record representing the party we invoice, collect payments from,
    /// issue credit/debit notes to, and track balances and aging for.
    /// </summary>
    public class CustomerViewModel
    {
        // Section 1: Core Customer Fields

        /// <summary>PK - hidden in UI</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Tenant identifier - hidden in UI</summary>
        public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

        /// <summary>FK → Company - hidden, derived from login/company context</summary>
        [Required(ErrorMessage = "Company is required")]
        public Guid CompanyId { get; set; }
        public string? CompanyName { get; set; }

        /// <summary>Customer Code - unique per CompanyId, max 30 chars, uppercase + trimmed</summary>
        [Required(ErrorMessage = "Customer Code is required")]
        [StringLength(30, ErrorMessage = "Customer Code cannot exceed 30 characters")]
        [RegularExpression(@"^[A-Za-z0-9_\-]+$", ErrorMessage = "Only uppercase letters, numbers and hyphen allowed")]
        public string CustomerCode { get; set; } = string.Empty;

        /// <summary>Customer Name - max 200 chars, trim + collapse multiple spaces</summary>
        [Required(ErrorMessage = "Customer Name is required")]
        [StringLength(200, ErrorMessage = "Customer Name cannot exceed 200 characters")]
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>Customer Display Name - short name for UI display</summary>
        [StringLength(100, ErrorMessage = "Display Name cannot exceed 100 characters")]
        public string? CustomerDisplayName { get; set; }

        /// <summary>Customer Type - Individual/Business/Government/Partner</summary>
        [Required(ErrorMessage = "Customer Type is required")]
        public string CustomerType { get; set; } = "";

        /// <summary>Customer Status - Active/Inactive/Blacklisted/Draft</summary>
        [Required(ErrorMessage = "Status is required")]
        public string CustomerStatus { get; set; } = CustomerStatuses.Draft;

        /// <summary>Primary Email - max 254 chars, email format validation</summary>
        [StringLength(254, ErrorMessage = "Email cannot exceed 254 characters")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? PrimaryEmail { get; set; }

        /// <summary>Primary Phone - max 20 chars, numeric + "+" allowed</summary>
        [StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters")]
        [RegularExpression(@"^[\d\+\-\s]+$", ErrorMessage = "Invalid phone format")]
        public string? PrimaryPhone { get; set; }

        /// <summary>Secondary Phone - max 20 chars</summary>
        [StringLength(20, ErrorMessage = "Secondary Phone cannot exceed 20 characters")]
        [RegularExpression(@"^[\d\+\-\s]+$", ErrorMessage = "Invalid phone format")]
        public string? SecondaryPhone { get; set; }

        /// <summary>Billing Email - for sending invoices</summary>
        [StringLength(254, ErrorMessage = "Billing Email cannot exceed 254 characters")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? BillingEmail { get; set; }

        /// <summary>Contact Person Name</summary>
        [StringLength(100, ErrorMessage = "Contact Person Name cannot exceed 100 characters")]
        public string? ContactPersonName { get; set; }

        /// <summary>Website - max 200 chars</summary>
        [StringLength(200, ErrorMessage = "Website cannot exceed 200 characters")]
        [Url(ErrorMessage = "Invalid URL format")]
        public string? Website { get; set; }

        /// <summary>FK → Branch (optional, future-ready)</summary>
        public Guid? DefaultBranchId { get; set; }
        public string? DefaultBranchName { get; set; }

        // Section 2: Tax & Compliance

        /// <summary>Tax Registration Type - Registered/Unregistered/Composition/SEZ/Export</summary>
        [Required(ErrorMessage = "Tax Registration Type is required")]
        public string TaxRegistrationType { get; set; } = "";

        /// <summary>GSTIN - 15 chars, conditional required if TaxRegistrationType in (Registered/SEZ/Export)</summary>
        [StringLength(15, MinimumLength = 15, ErrorMessage = "GSTIN must be exactly 15 characters")]
        [RegularExpression(@"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$", ErrorMessage = "Invalid GSTIN format")]
        public string? GSTIN { get; set; }

        /// <summary>GST State Code - 2 digit state code from GSTIN</summary>
        [StringLength(2, MinimumLength = 2, ErrorMessage = "GST State Code must be exactly 2 characters")]
        public string? GSTStateCode { get; set; }

        /// <summary>PAN - 10 chars, PAN format validation</summary>
        [StringLength(10, MinimumLength = 10, ErrorMessage = "PAN must be exactly 10 characters")]
        [RegularExpression(@"^[A-Z]{5}[0-9]{4}[A-Z]$", ErrorMessage = "Invalid PAN format (e.g., AAAAA9999A)")]
        public string? PAN { get; set; }

        /// <summary>TAN - Tax Deduction Account Number</summary>
        [StringLength(10, MinimumLength = 10, ErrorMessage = "TAN must be exactly 10 characters")]
        [RegularExpression(@"^[A-Z]{4}[0-9]{5}[A-Z]$", ErrorMessage = "Invalid TAN format")]
        public string? TAN { get; set; }

        /// <summary>FK → TaxProfile</summary>
        [Required(ErrorMessage = "Tax Profile is required")]
        public Guid? TaxProfileId { get; set; }
        public string? TaxProfileName { get; set; }

        /// <summary>TDS Applicable - optional for India</summary>
        public bool IsTDSApplicable { get; set; } = false;

        /// <summary>TDS Section Code - conditional if IsTDSApplicable = true</summary>
        [StringLength(20, ErrorMessage = "TDS Section Code cannot exceed 20 characters")]
        public string? TDSSectionCode { get; set; }

        // Section 3: Credit & Terms (AR Risk Control)

        /// <summary>Credit Limit - 0 means no credit allowed (cash before delivery)</summary>
        [Required(ErrorMessage = "Credit Limit is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Credit Limit must be 0 or greater")]
        public decimal CreditLimitAmount { get; set; } = 0;

        /// <summary>Enforce Credit Limit - if true, invoice posting checks exposure</summary>
        [Required]
        public bool CreditLimitEnforced { get; set; } = true;

        /// <summary>Credit Hold Status - None/OnHold/TemporaryHold</summary>
        [Required(ErrorMessage = "Credit Hold Status is required")]
        public string CreditHoldStatus { get; set; } = CreditHoldStatuses.None;

        /// <summary>Hold Reason - required if CreditHoldStatus != None</summary>
        [StringLength(250, ErrorMessage = "Hold Reason cannot exceed 250 characters")]
        public string? CreditHoldReason { get; set; }

        /// <summary>Hold Placed On - system timestamp</summary>
        public DateTime? CreditHoldPlacedOn { get; set; }

        /// <summary>Hold Placed By User - FK → User</summary>
        public Guid? CreditHoldPlacedByUserId { get; set; }
        public string? CreditHoldPlacedByUserName { get; set; }

        /// <summary>FK → PaymentTermMaster</summary>
        [Required(ErrorMessage = "Payment Terms is required")]
        public Guid? PaymentTermId { get; set; }
        public string? PaymentTermName { get; set; }

        /// <summary>FK → CurrencyMaster - usually company base currency</summary>
        [Required(ErrorMessage = "Currency is required")]
        public Guid? DefaultCurrencyId { get; set; }
        public string? DefaultCurrencyCode { get; set; }
        public string? DefaultCurrencyName { get; set; }

        /// <summary>Default Payment Method - Cash/BankTransfer/Cheque/UPI/Card/Wallet/Other</summary>
        [Required(ErrorMessage = "Default Payment Method is required")]
        public string DefaultPaymentMethod { get; set; } = "";

        // Section 4: Accounting Defaults (Posting Mapping)

        /// <summary>FK → GLAccountMaster - AR Receivable Account (Mandatory)</summary>
        [Required(ErrorMessage = "Receivable Account is required")]
        public Guid? ReceivableAccountId { get; set; }
        public string? ReceivableAccountCode { get; set; }
        public string? ReceivableAccountName { get; set; }

        /// <summary>FK → GLAccountMaster - Customer Advances Account (Optional)</summary>
        public Guid? AdvanceFromCustomerAccountId { get; set; }
        public string? AdvanceFromCustomerAccountCode { get; set; }
        public string? AdvanceFromCustomerAccountName { get; set; }

        // Aliases for shorter property names
        public string? AdvanceAccountCode => AdvanceFromCustomerAccountCode;
        public string? AdvanceAccountName => AdvanceFromCustomerAccountName;

        /// <summary>FK → GLAccountMaster - Write-Off Account (Optional)</summary>
        public Guid? WriteOffAccountId { get; set; }
        public string? WriteOffAccountCode { get; set; }
        public string? WriteOffAccountName { get; set; }

        /// <summary>Convert Overpayment to Advance - if enabled, excess payment becomes advance automatically</summary>
        public bool AllowAutoAdvanceCreation { get; set; } = false;

        // Section 5: Preferences (Operational)

        /// <summary>Email Invoices - default true</summary>
        public bool SendInvoiceEmail { get; set; } = true;

        /// <summary>Statement Cycle - Weekly/Monthly/Quarterly/OnDemand</summary>
        public string CustomerStatementCycle { get; set; } = "";

        /// <summary>Preferred Language - EN/TA/...</summary>
        [StringLength(10)]
        public string? PreferredLanguage { get; set; } = "EN";

        /// <summary>Allow Partial Payment - default true</summary>
        public bool AllowPartialPayment { get; set; } = true;

        /// <summary>Allow Over Payment - default false</summary>
        public bool AllowOverPayment { get; set; } = false;

        // Section 6: Optional Future-Ready Fields

        /// <summary>FK → User - collections ownership</summary>
        public Guid? AccountManagerUserId { get; set; }
        public string? AccountManagerUserName { get; set; }

        /// <summary>FK → CustomerSegment - reporting segmentation</summary>
        public Guid? CustomerSegmentId { get; set; }
        public string? CustomerSegmentName { get; set; }

        /// <summary>FK → IndustryMaster</summary>
        public Guid? IndustryId { get; set; }
        public string? IndustryName { get; set; }

        /// <summary>FK → RegionMaster</summary>
        public Guid? RegionId { get; set; }
        public string? RegionName { get; set; }

        /// <summary>KYC Status - NotRequired/Pending/Verified/Rejected/Failed/Expired/NotStarted</summary>
        public string? KYCStatus { get; set; }

        /// <summary>KYC Verification Date - when KYC was verified</summary>
        public DateTime? KYCVerificationDate { get; set; }

        /// <summary>External Reference ID - integrate with external CRM/ERP</summary>
        [StringLength(50)]
        public string? ExternalReferenceId { get; set; }

        // Section 7: System Audit Fields

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
    }

    #endregion

    #region CustomerAccount

    /// <summary>
    /// CustomerAccount – The AR subledger account summary for a customer.
    /// Provides the running financial position (summary + exposure) for a customer.
    /// System-maintained, must never become a "manual editable balance sheet".
    /// </summary>
    public class CustomerAccountViewModel
    {
        // Section 1: Core Identity Fields

        /// <summary>PK - hidden in UI</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Tenant identifier - hidden in UI</summary>
        public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

        /// <summary>FK → Company - hidden</summary>
        [Required(ErrorMessage = "Company is required")]
        public Guid CompanyId { get; set; }
        public string? CompanyName { get; set; }

        /// <summary>FK → Customer - read-only link to customer view</summary>
        [Required(ErrorMessage = "Customer is required")]
        public Guid CustomerId { get; set; }
        public string? CustomerCode { get; set; }
        public string? CustomerName { get; set; }

        /// <summary>FK → CurrencyMaster - read-only</summary>
        [Required(ErrorMessage = "Currency is required")]
        public Guid CurrencyId { get; set; }
        public string? CurrencyCode { get; set; }
        public string? CurrencyName { get; set; }

        /// <summary>FK → Branch (Future-ready, optional)</summary>
        public Guid? BranchId { get; set; }
        public string? BranchName { get; set; }

        /// <summary>Account Status - Active/Frozen/Closed</summary>
        [Required(ErrorMessage = "Account Status is required")]
        public string AccountStatus { get; set; } = CustomerAccountStatuses.Active;

        // Section 2: Balance Summary Fields (System Maintained - Read-only)

        /// <summary>Outstanding Receivable - increases by posted invoices/debit notes; decreases by payments/credit notes/adjustments</summary>
        [Required]
        [Range(0, double.MaxValue)]
        public decimal OutstandingReceivableAmount { get; set; } = 0;

        /// <summary>Unapplied/On-Account - payments received but not allocated (or excess payments)</summary>
        [Required]
        [Range(0, double.MaxValue)]
        public decimal UnappliedPaymentAmount { get; set; } = 0;

        /// <summary>Customer Advance - overpayment converted to advance (if policy enabled)</summary>
        [Required]
        [Range(0, double.MaxValue)]
        public decimal AdvanceBalanceAmount { get; set; } = 0;

        /// <summary>Net Receivable (Derived) = OutstandingReceivable - (UnappliedPayment + AdvanceBalance)</summary>
        public decimal NetReceivableAmount => OutstandingReceivableAmount - (UnappliedPaymentAmount + AdvanceBalanceAmount);

        /// <summary>Last Activity - updated when any AR document posts</summary>
        public DateTime? LastActivityOn { get; set; }

        /// <summary>Last Invoice Date</summary>
        public DateTime? LastInvoiceOn { get; set; }

        /// <summary>Last Payment Date</summary>
        public DateTime? LastPaymentOn { get; set; }

        // Section 3: Credit Exposure Fields (System Derived)

        /// <summary>Credit Limit Snapshot - copy from Customer.CreditLimitAmount at time of evaluation</summary>
        public decimal CreditLimitAmountSnapshot { get; set; } = 0;

        /// <summary>Credit Limit Enforced Snapshot</summary>
        public bool CreditLimitEnforcedSnapshot { get; set; } = true;

        /// <summary>Credit Exposure (Derived) = max(OutstandingReceivable - (UnappliedPayment + AdvanceBalance), 0)</summary>
        public decimal CreditExposureAmount => Math.Max(OutstandingReceivableAmount - (UnappliedPaymentAmount + AdvanceBalanceAmount), 0);

        /// <summary>Available Credit (Derived) = CreditLimit - CreditExposure (if enforced)</summary>
        public decimal AvailableCreditAmount => CreditLimitEnforcedSnapshot ? CreditLimitAmountSnapshot - CreditExposureAmount : decimal.MaxValue;

        /// <summary>Over Limit (Derived) = max(CreditExposure - CreditLimit, 0)</summary>
        public decimal OverCreditAmount => Math.Max(CreditExposureAmount - CreditLimitAmountSnapshot, 0);

        // Section 4: Opening Balance & Migration Controls (Controller Only)

        /// <summary>Opening AR Balance - controller-only, used during migration</summary>
        public decimal OpeningReceivableAmount { get; set; } = 0;

        /// <summary>Opening Advance</summary>
        public decimal OpeningAdvanceAmount { get; set; } = 0;

        /// <summary>Opening As Of Date - required if any opening amount entered</summary>
        public DateTime? OpeningBalanceAsOfDate { get; set; }

        /// <summary>Import Batch ID - ties to import file/batch</summary>
        public Guid? OpeningBalanceImportBatchId { get; set; }

        /// <summary>FK → User - approval governance</summary>
        public Guid? OpeningBalanceApprovedByUserId { get; set; }
        public string? OpeningBalanceApprovedByUserName { get; set; }

        /// <summary>Opening Balance Approved On</summary>
        public DateTime? OpeningBalanceApprovedOn { get; set; }

        // Section 5: Locks & Restrictions (Controller Only)

        /// <summary>Block Posting - if true, no posting allowed for this customer account</summary>
        public bool IsPostingBlocked { get; set; } = false;

        /// <summary>Block Reason - required if IsPostingBlocked = true</summary>
        [StringLength(250, ErrorMessage = "Block Reason cannot exceed 250 characters")]
        public string? PostingBlockReason { get; set; }

        /// <summary>FK → User - who blocked</summary>
        public Guid? PostingBlockedByUserId { get; set; }
        public string? PostingBlockedByUserName { get; set; }

        /// <summary>Blocked On - timestamp</summary>
        public DateTime? PostingBlockedOn { get; set; }

        /// <summary>Freeze Type - None/CollectionsHold/ComplianceHold/DisputeHold/Manual</summary>
        public string? FreezeType { get; set; } = FreezeTypes.None;

        // Section 6: Optional Future-Ready Fields

        /// <summary>Risk Score - 0-100, automated credit policies</summary>
        [Range(0, 100)]
        public int? RiskScore { get; set; }

        /// <summary>Collections Stage - Normal/Reminder/FollowUp/Legal</summary>
        public string? CollectionsStage { get; set; }

        /// <summary>Collections Started On - when account entered collections</summary>
        public DateTime? CollectionsStartedOn { get; set; }

        /// <summary>Last Collection Action On - timestamp of last collection activity</summary>
        public DateTime? LastCollectionActionOn { get; set; }

        /// <summary>Collections Notes - free text notes for collections team</summary>
        [StringLength(1000, ErrorMessage = "Collections Notes cannot exceed 1000 characters")]
        public string? CollectionsNotes { get; set; }

        /// <summary>FK → DunningPolicyMaster - reminders automation</summary>
        public Guid? DunningPolicyId { get; set; }
        public string? DunningPolicyName { get; set; }

        // Section 7: System Audit Fields

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
        public bool IsDeleted { get; set; } = false;
    }

    #endregion

    #region Customer-related Enums and Static Classes

    public static class CustomerTypes
    {
        public const string Individual = "Individual";
        public const string Business = "Business";
        public const string Government = "Government";
        public const string Partner = "Partner";
        public static readonly string[] All = new[] { Individual, Business, Government, Partner };
    }

    public static class CustomerStatuses
    {
        public const string Draft = "Draft";
        public const string Active = "Active";
        public const string Inactive = "Inactive";
        public const string Blacklisted = "Blacklisted";
        public static readonly string[] All = new[] { Draft, Active, Inactive, Blacklisted };
    }

    public static class TaxRegistrationTypes
    {
        public const string Registered = "Registered";
        public const string Unregistered = "Unregistered";
        public const string Composition = "Composition";
        public const string SEZ = "SEZ";
        public const string Export = "Export";
        public static readonly string[] All = new[] { Registered, Unregistered, Composition, SEZ, Export };
    }

    public static class CreditHoldStatuses
    {
        public const string None = "None";
        public const string OnHold = "OnHold";
        public const string TemporaryHold = "TemporaryHold";
        public static readonly string[] All = new[] { None, OnHold, TemporaryHold };
    }

    public static class PaymentMethods
    {
        public const string Cash = "Cash";
        public const string BankTransfer = "BankTransfer";
        public const string Cheque = "Cheque";
        public const string UPI = "UPI";
        public const string Card = "Card";
        public const string Wallet = "Wallet";
        public const string Gateway = "Gateway";
        public const string Other = "Other";
        public static readonly string[] All = new[] { Cash, BankTransfer, Cheque, UPI, Card, Wallet, Gateway, Other };

        public static string GetDisplayName(string method) => method switch
        {
            Cash => "Cash",
            BankTransfer => "Bank Transfer (NEFT/RTGS/IMPS)",
            Cheque => "Cheque",
            UPI => "UPI",
            Card => "Card (Credit/Debit)",
            Wallet => "Wallet",
            Gateway => "Payment Gateway",
            Other => "Other",
            _ => method
        };

        /// <summary>Check if instrument number is required for this method</summary>
        public static bool RequiresInstrumentNumber(string method) => method switch
        {
            BankTransfer => true,
            Cheque => true,
            Gateway => true,
            Card => true,
            _ => false
        };

        /// <summary>Check if bank name is required for this method</summary>
        public static bool RequiresBankName(string method) => method == Cheque;

        /// <summary>Check if instrument date is required for this method</summary>
        public static bool RequiresInstrumentDate(string method) => method == Cheque || method == BankTransfer;

        /// <summary>Get placeholder text for instrument number based on method</summary>
        public static string GetInstrumentPlaceholder(string method) => method switch
        {
            BankTransfer => "Enter UTR Number",
            UPI => "Enter UPI Reference",
            Cheque => "Enter Cheque Number",
            Card => "Enter Transaction Reference",
            Gateway => "Enter Gateway Transaction ID",
            _ => "Enter Reference Number"
        };
    }

    public static class StatementCycles
    {
        public const string Weekly = "Weekly";
        public const string Monthly = "Monthly";
        public const string Quarterly = "Quarterly";
        public const string OnDemand = "OnDemand";
        public static readonly string[] All = new[] { Weekly, Monthly, Quarterly, OnDemand };
    }

    public static class CustomerAccountStatuses
    {
        public const string Active = "Active";
        public const string Inactive = "Inactive";
        public const string Frozen = "Frozen";
        public const string Closed = "Closed";
        public static readonly string[] All = new[] { Active, Inactive, Frozen, Closed };
    }

    public static class FreezeTypes
    {
        public const string None = "None";
        public const string CollectionsHold = "CollectionsHold";
        public const string ComplianceHold = "ComplianceHold";
        public const string DisputeHold = "DisputeHold";
        public const string Manual = "Manual";
        public static readonly string[] All = new[] { None, CollectionsHold, ComplianceHold, DisputeHold, Manual };

        public static string GetDisplayName(string type) => type switch
        {
            None => "None",
            CollectionsHold => "Collections Hold",
            ComplianceHold => "Compliance Hold",
            DisputeHold => "Dispute Hold",
            Manual => "Manual",
            _ => type
        };
    }

    public static class CollectionsStages
    {
        public const string None = "None";
        public const string Normal = "Normal";
        public const string Reminder = "Reminder";
        public const string FirstNotice = "FirstNotice";
        public const string SecondNotice = "SecondNotice";
        public const string FinalNotice = "FinalNotice";
        public const string FollowUp = "FollowUp";
        public const string Legal = "Legal";
        public const string WriteOff = "WriteOff";
        public static readonly string[] All = new[] { None, Normal, Reminder, FirstNotice, SecondNotice, FinalNotice, FollowUp, Legal, WriteOff };

        public static string GetDisplayName(string stage) => stage switch
        {
            None => "None",
            Normal => "Normal",
            Reminder => "Reminder Sent",
            FirstNotice => "First Notice",
            SecondNotice => "Second Notice",
            FinalNotice => "Final Notice",
            FollowUp => "Follow Up",
            Legal => "Legal Action",
            WriteOff => "Written Off",
            _ => stage
        };
    }

    public static class KycStatuses
    {
        public const string NotStarted = "NotStarted";
        public const string NotRequired = "NotRequired";
        public const string Pending = "Pending";
        public const string Verified = "Verified";
        public const string Rejected = "Rejected";
        public const string Failed = "Failed";
        public const string Expired = "Expired";
        public static readonly string[] All = new[] { NotStarted, NotRequired, Pending, Verified, Rejected, Failed, Expired };
    }

    #endregion
}
