using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public class BankAccountModel    {


        public Guid Id { get; set; }


        // Ownership
        public string CompanyName { get; set; } = "";

        public Guid CompanyId { get; set; } =Guid.Empty;
        public Guid BranchId { get; set; } = Guid.Empty;
        public string BranchName { get; set; } = "";


        // Identity
        public Guid BankAccountId { get; set; }   // DB key

        [Required(ErrorMessage = "Account Code is required")]
        public string BankAccountCode { get; set; } = string.Empty; // UI / business

        [Required(ErrorMessage = "Account Name is required")]
        public string BankAccountName { get; set; } = "";
        public string Description { get; set; } = "";


        // Bank details
        public string BankName { get; set; } = "";
        public string BankBranchName { get; set; } = "";
        public string IFSCCode { get; set; } = "";
        public string MICRCode { get; set; } = "";
        public string AccountHolderName { get; set; } = "";


        // Security
        public string BankAccountNumberEncrypted { get; set; } = "";
        public string BankAccountNumberLast4 { get; set; } = "";
        public string BankAccountNumber { get; set; } = "";

        // Optional bank identifiers
        public string UPIId { get; set; } = "";
        public string SWIFTCode { get; set; } = "";
        public string IBAN { get; set; } = "";

        // Classification
        public string CurrencyCode { get; set; } = "INR";
        [Required]
        public Guid CurrencyId { get; set; }
        public string BankAccountType { get; set; } = "Current";


        // Controls
        public bool IsOverdraftAllowed { get; set; }
        public decimal? OverdraftLimitAmount { get; set; }
        public decimal? MinimumBalanceAmount { get; set; }
        public bool IsLockedForTransactions { get; set; }
        public string LockReason { get; set; } = "";
        public bool IsBlocked { get; set; }
        public string BlockReason { get; set; } = "";


        // Accounting
        public string BankGLAccountCode { get; set; } = "";
        public string? ClearingGLAccountCode { get; set; }
        public string BankChargesExpenseGLCode { get; set; } = "";
        public string InterestIncomeGLCode { get; set; } = "";
        public string RoundOffGLCode { get; set; } = "";

        public bool IsPrimaryOperatingAccount { get; set; }

        // Statement & Reconciliation
        public bool IsStatementImportEnabled { get; set; }
        public string StatementProfile { get; set; } = "";
        public bool IsBankReconciliationMandatory { get; set; } = true;
        public bool AutoMatchEnabled { get; set; } = true;
        public int AutoMatchDateWindowDays { get; set; } = 2;
        public decimal AutoMatchAmountTolerance { get; set; } = 0;

        // Status
        public string BankAccountStatus { get; set; } = "";
        public string CloseReason { get; set; } = "";


        // Audit
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
}
