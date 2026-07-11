using System;
using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public class BankTransactionModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Identity
        public DateTime? PostingDate { get; set; }
        [Required(ErrorMessage = "Company is required")]
        public Guid? CompanyId { get; set; }
        public string? CompanyName { get; set; }


        [Required(ErrorMessage = "Branch is required")]
        public Guid? BranchId { get; set; }
        public string? BranchName { get; set; }

        [Required(ErrorMessage = "Transaction Number is required")]
        public string TransactionNumber { get; set; } = string.Empty;
        public string TransactionStatus { get; set; } = "";

        // Account
        public string AccountKind { get; set; } = "Bank"; // Bank / Cash
        public Guid? BankAccountId { get; set; }
        public Guid? CashAccountId { get; set; }

        // Date & Amount
        [Required(ErrorMessage = "Transaction Date is required")]
        public DateTime TransactionDate { get; set; } = DateTime.Today;
        public DateTime? ValueDate { get; set; }
        public string CurrencyCode { get; set; } = "INR";

        [Required(ErrorMessage = "Direction is required")]
        public string Direction { get; set; } = "Outflow";

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        public decimal Amount { get; set; }
        public decimal? BaseAmount { get; set; }
        public decimal ExchangeRate { get; set; } = 1;

        // Classification
        [Required(ErrorMessage = "Transaction Type is required")]
        public string TransactionType { get; set; } = string.Empty;
        [MaxLength(1000)]
        public string? Narration { get; set; }
        public string? SourceDocumentType { get; set; }
        [MaxLength(200)]
        public string? CounterpartyNameSnapshot { get; set; } // read-only / auto-fill

        [MaxLength(100)]
        public string? ReferenceNumber { get; set; } // optional but recommended


        public string? ExternalReferenceHash { get; set; } // optional / future
                                                           // Source & Reference
        [Required(ErrorMessage = "Source Module is required")]
        public string? SourceModule { get; set; }
        public string? CounterpartyType { get; set; }

        // Instrument
        [Required(ErrorMessage = "Payment Method is required")]
        public string? PaymentMethod { get; set; }
        public string? UTRNumber { get; set; }

        // Posting
        public string PostingStatus { get; set; } = "NotPosted";
        public Guid? JournalEntryId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}
