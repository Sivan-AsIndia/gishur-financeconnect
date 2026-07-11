using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public class JournalEntryModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(30)]
        public string JournalEntryNumber { get; set; } = string.Empty;

        [Required]
        public Guid? CompanyId { get; set; }

        public string JournalName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;

        [Required]
        public Guid? BranchId { get; set; }

        [Required]
        public Guid? JournalId { get; set; }

        [Required]
        public Guid? LedgerId { get; set; } // derived from Journal

        [Required]
        public DateTime EntryDate { get; set; } = DateTime.Today;

        [Required]
        public DateTime PostingDate { get; set; } = DateTime.Today;


        [Required(ErrorMessage = "Fiscal Year is required")]
        public Guid? FiscalYearId { get; set; }


        [Required(ErrorMessage = "Accounting Period is required")]
        public Guid? AccountingPeriodId { get; set; }

        public string FiscalYearName { get; set; } = string.Empty;
        public string AccountingPeriodName { get; set; } = string.Empty;

        public bool IsBackdated { get; set; } = false;
        public string? PostingPolicyOverrideReason { get; set; }

        [StringLength(1000)]
        public string Narration { get; set; } = string.Empty;

        public ReferenceType ReferenceType { get; set; } = ReferenceType.Manual;

        [StringLength(50)]
        public string? ReferenceId { get; set; }

        [StringLength(50)]
        public string? ExternalReferenceNumber { get; set; }

        public int AttachmentCount { get; set; }

        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }

        public decimal Difference => TotalDebit - TotalCredit;
        public Guid CurrencyId { get; set; }
        public string CurrencyCode { get; set; } = "INR";
        public JournalEntryStatus Status { get; set; } = JournalEntryStatus.Draft;
        public DateTime? SubmittedAt { get; set; }
        public string? SubmittedBy { get; set; }

        public DateTime? ApprovedAt { get; set; }
        public string? ApprovedBy { get; set; }

        public DateTime? PostedAt { get; set; }
        public string? PostedBy { get; set; }

        public string? PostingBatchId { get; set; }

        public bool IsReversal { get; set; } = false;
        public Guid? ReversalOfJournalEntryId { get; set; }
        public Guid? ReversedByJournalEntryId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = "system";

        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public bool IsReadOnly =>
            Status == JournalEntryStatus.Posted ||
            Status == JournalEntryStatus.Cancelled;

        public bool IsPeriodClosed { get; set; } // derived by service

        public int LineCount { get; set; }

        public DateTime? RejectedAt { get; set; }
        public string? RejectedBy { get; set; }

        public DateTime? CancelledAt { get; set; }
        public string? CancelledBy { get; set; }

    }

    public enum JournalEntryStatus
    {
        Draft = 1,
        Submitted = 2,
        Approved = 3,
        Posted = 4,
        Rejected = 5,
        Cancelled = 6
    }

    public enum ReferenceType
    {
        Manual = 1,
        OpeningBalance,
        VendorBill,
        VendorPayment,
        CustomerInvoice,
        CustomerReceipt,
        BankTransaction,
        AssetTransaction,
        Other
    }

    public class CompanyLookup
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }



    public class JournalLookup
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid LedgerId { get; set; }
        public string LedgerName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool NarrationRequired { get; set; } = true;
        public JournalStatus Status { get; set; }
    }

    public class PeriodResolution
    {
 
        public Guid CompanyId { get; set; }

        public string JournalName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        // ---------- Fiscal Year ----------
        public Guid FiscalYearId { get; set; }
        public string FiscalYearName { get; set; } = string.Empty;

        // ---------- Accounting Period ----------
        public Guid PeriodId { get; set; }
        public string PeriodName { get; set; } = string.Empty;

        public AccountingPeriodStatus PeriodStatus { get; set; }

        // ---------- Dates ----------
        public DateTime PeriodStartDate { get; set; }
        public DateTime PeriodEndDate { get; set; }

        // ---------- Convenience Flags ----------
        public bool IsOpen =>
            PeriodStatus == AccountingPeriodStatus.Open;

        public bool IsSoftClosed =>
            PeriodStatus == AccountingPeriodStatus.SoftClosed;

        public bool IsClosed =>
            PeriodStatus == AccountingPeriodStatus.Closed;
    }
}
