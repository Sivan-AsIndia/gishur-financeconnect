using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public class CashAccountModels
    {
        // 🔹 Identity
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid CompanyId { get; set; } = Guid.Empty;

        [Required(ErrorMessage = "Branch is required")]
        public Guid BranchId { get; set; } = Guid.Empty;

        // 🔹 Core Fields
        [Required(ErrorMessage = "Cash Account Code is required")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cash Account Name is required")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        // 🔹 Display Helpers
        [Required(ErrorMessage = "Branch is required")]

        public string BranchName { get; set; }
        public string? CustodianName { get; set; }

        // 🔹 Currency
        [Required(ErrorMessage = "Currency is required")]
        public Guid CurrencyId { get; set; } = Guid.Empty;

        [Required(ErrorMessage = "Currency is required")]
        public string CurrencyCode { get; set; } = string.Empty; // INR

        // 🔹 Accounting Mapping
        public string CashGlAccountId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cash GL Account is required")]
        public string CashGlAccount { get; set; } = string.Empty;

        // 🔹 Controls & Limits
        public bool IsNegativeBalanceAllowed { get; set; } = false;

        [Range(0, double.MaxValue, ErrorMessage = "Max Cash Limit must be greater than zero")]
        public decimal? MaxCashLimit { get; set; }

        public bool RequireReasonWhenExceedingLimit { get; set; } = true;

        [Range(0, double.MaxValue, ErrorMessage = "Amount must be >= 0")]
        public decimal? RequireAttachmentAboveAmount { get; set; }

        public bool IsLockedForTransactions { get; set; } = false;

        // 🔹 Custody
        public Guid? CustodianUserId { get; set; }
        public DateTime? CustodyStartDate { get; set; }
        public string? CustodyNotes { get; set; }

        // 🔹 Status & Lifecycle
        public string Status { get; set; } = "Active";

        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedOn { get; set; }
        public string? CloseReason { get; set; }

        // 🔹 Audit
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
