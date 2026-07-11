using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public enum ChequeStatus
    {
        Draft,
        Prepared,
        Printed,
        Issued,
        Received,
        Deposited,
        Presented,
        Cleared,
        Bounced,
        Stopped,
        Cancelled,
        Stale,
        Reissued
    }

    public enum ChequeDirection
    {
        Outgoing,
        Incoming
    }

    public class ChequeModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Core
        [Required(ErrorMessage = "Branch is required")]
        public Guid BranchId { get; set; } = Guid.Empty;
        [Required(ErrorMessage = "Branch is required")]
        public string Branch { get; set; } = "";
        [Required(ErrorMessage = "ChequeNumber is required")]
        public string ChequeNumber { get; set; } = "";
        public ChequeStatus Status { get; set; } = ChequeStatus.Draft;
        [Required(ErrorMessage = "Direction is required")]
        public ChequeDirection Direction { get; set; }

        // Instrument
        [Required(ErrorMessage = "Cheque Date is required")]
        public DateTime ChequeDate { get; set; } = DateTime.Today;
        [Required(ErrorMessage = "Amount is required")]
        [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        [Required(ErrorMessage = "Payee Name is required")]
        public string PayeeName { get; set; } = "";
        public string CrossingType { get; set; } = "None";

        // Counterparty
        [Required(ErrorMessage = "Counterparty Type is required")]
        public string CounterpartyType { get; set; } = "";
        //[Required(ErrorMessage = "Counterparty is required")]
        public string CounterpartyName { get; set; } = "";
        public string? CounterpartyContact { get; set; }

        // Bank
        [Required(ErrorMessage = "OurBankAccount is required")]

        public Guid? OurBankAccountId { get; set; }
        public string OurBankAccount { get; set; }

        // Incoming-specific (optional)
        public string? DrawerBankName { get; set; }        // Max 150
        public string? DrawerBankBranch { get; set; }      // Max 150
        public string? DrawerAccountMasked { get; set; }   // Max 30
        public string? MICR_IFSC { get; set; }             // Max 20
        public bool IsCTSCompliant { get; set; }        // true for issued, null for incoming

        // Lifecycle
        public DateTime? PrintedOn { get; set; }
        public DateTime? IssuedOn { get; set; }
        public DateTime? ReceivedOn { get; set; }
        public DateTime? DepositedOn { get; set; }
        public DateTime? PresentedOn { get; set; }
        public DateTime? ClearedOn { get; set; }
        public DateTime? BouncedOn { get; set; }
        public DateTime? StoppedOn { get; set; }
        public DateTime? StaleOn { get; set; }
        public DateTime? ExpectedClearBy { get; set; }
        public DateTime? IssuedOrReceivedOn { get; set; }
        public DateTime? PreparedOn { get; set; }

        // Linkage
        public string SourceModule { get; set; } = "";
        public string SourceDocumentType { get; set; } = "";
        public string? SourceDocumentNo { get; set; }
        public Guid? BankTransactionId { get; set; }
        public Guid? ReconciliationId { get; set; }

        // Bounce
        public string? BounceReason { get; set; }
        public string? BounceReasonText { get; set; }
        public decimal? BounceCharges { get; set; }

        // Notes
        public string? Narration { get; set; }

        // Extra lifecycle
        public DateTime? CancelledOn { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }

    public class Counterparty
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
