using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    /// <summary>
    /// Model #15: GeneralLedgerEntry - The final posted accounting row
    /// This is a READ-ONLY ledger viewer - entries are created by the posting engine only
    /// </summary>
    public class GeneralLedgerEntryModel
    {
        // Section 1: Core Identity & Ledger Context (Immutable)
        
        /// <summary>PK - hidden in UI</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Tenant identifier - hidden in UI</summary>
        public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

        /// <summary>FK → Company - read-only, must match source document company</summary>
        [Required]
        public Guid CompanyId { get; set; }
        public string? CompanyCode { get; set; }
        public string? CompanyName { get; set; }

        /// <summary>FK → Branch - read-only, mandatory (Option 2), must be Active at posting time</summary>
        [Required]
        public Guid BranchId { get; set; }
        public string? BranchCode { get; set; }
        public string? BranchName { get; set; }

        /// <summary>FK → Ledger - read-only, derived from Journal/Ledger rules</summary>
        [Required]
        public Guid LedgerId { get; set; }
        public string? LedgerCode { get; set; }
        public string? LedgerName { get; set; }

        /// <summary>FK → Account - read-only, must be Active, Postable, and belong to Company's COA</summary>
        [Required]
        public Guid AccountId { get; set; }
        public string? AccountCode { get; set; }
        public string? AccountName { get; set; }

        // Section 2: Period & Date Context

        /// <summary>FK → FiscalYear - read-only</summary>
        [Required]
        public Guid FiscalYearId { get; set; }
        public string? FiscalYearCode { get; set; }
        public string? FiscalYearName { get; set; }

        /// <summary>FK → AccountingPeriod - read-only, PostingDate must fall in this period</summary>
        [Required]
        public Guid AccountingPeriodId { get; set; }
        public string? AccountingPeriodCode { get; set; }
        public string? AccountingPeriodName { get; set; }

        /// <summary>Entry Date (Document Date) - read-only, business date from source document</summary>
        [Required]
        public DateTime EntryDate { get; set; }

        /// <summary>Posting Date - read-only, accounting date for reporting/period</summary>
        [Required]
        public DateTime PostingDate { get; set; }

        /// <summary>Sequence No - optional read-only, guarantees stable ordering for ledger print/export</summary>
        [Required]
        public long PostingSequenceNumber { get; set; }

        // Section 3: Amounts (Accounting Truth)

        /// <summary>Debit - read-only, can be 0. Rule: DebitAmount > 0 XOR CreditAmount > 0</summary>
        [Required]
        [Range(0, double.MaxValue)]
        public decimal DebitAmount { get; set; }

        /// <summary>Credit - read-only, can be 0. Rule: DebitAmount > 0 XOR CreditAmount > 0</summary>
        [Required]
        [Range(0, double.MaxValue)]
        public decimal CreditAmount { get; set; }

        /// <summary>FK → Currency (Base Currency) - read-only, always Company.BaseCurrencyId</summary>
        [Required]
        public Guid BaseCurrencyId { get; set; }
        public string? BaseCurrencyCode { get; set; }
        public string? BaseCurrencyName { get; set; }

        // Section 4: Source Traceability (Audit Backbone)

        /// <summary>Source Type - read-only enum</summary>
        [Required]
        [StringLength(50)]
        public string SourceType { get; set; } = "JournalEntry";

        /// <summary>Source Document Id - read-only link (GUID/string)</summary>
        [Required]
        public Guid SourceDocumentId { get; set; }

        /// <summary>Source Document No - read-only, strongly recommended</summary>
        [StringLength(50)]
        public string? SourceDocumentNumber { get; set; }

        /// <summary>FK → JournalEntry - read-only link, recommended for all sources</summary>
        public Guid? JournalEntryId { get; set; }
        public string? JournalEntryNumber { get; set; }

        /// <summary>FK → JournalLine - read-only link, optional</summary>
        public Guid? JournalLineId { get; set; }
        public int? JournalLineNumber { get; set; }

        // Section 5: Description & References

        /// <summary>Narration - read-only, usually from JournalEntry header or line narration</summary>
        [StringLength(1000)]
        public string? Narration { get; set; }

        /// <summary>Line Narration - read-only</summary>
        [StringLength(500)]
        public string? LineNarration { get; set; }

        /// <summary>External Ref No - read-only</summary>
        [StringLength(50)]
        public string? ExternalReferenceNumber { get; set; }

        // Section 6: Snapshot Fields (Immutability)

        /// <summary>Account Code at posting time - read-only</summary>
        [Required]
        [StringLength(50)]
        public string AccountCodeSnapshot { get; set; } = string.Empty;

        /// <summary>Account Name at posting time - read-only</summary>
        [Required]
        [StringLength(200)]
        public string AccountNameSnapshot { get; set; } = string.Empty;

        /// <summary>Branch Code at posting time - read-only</summary>
        [Required]
        [StringLength(20)]
        public string BranchCodeSnapshot { get; set; } = string.Empty;

        /// <summary>Branch Name at posting time - read-only</summary>
        [Required]
        [StringLength(200)]
        public string BranchNameSnapshot { get; set; } = string.Empty;

        // Section 7: Reversal & Correction Tracking

        /// <summary>Reversal? - badge in UI, default false</summary>
        [Required]
        public bool IsReversal { get; set; } = false;

        /// <summary>Reversal Group - read-only, links original + reversal entries</summary>
        public Guid? ReversalGroupId { get; set; }

        /// <summary>Reverses Document Id - read-only, direct reference to what it reverses</summary>
        public Guid? ReversesSourceDocumentId { get; set; }

        // Section 8: System Audit Fields (Hidden)

        /// <summary>Posting timestamp</summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>Who posted</summary>
        [StringLength(100)]
        public string? CreatedBy { get; set; }

        /// <summary>Optional posting batch identifier</summary>
        public Guid? PostingBatchId { get; set; }

        /// <summary>Should always be false - immutable entries should never be deleted</summary>
        public bool IsDeleted { get; set; } = false;

        // Display Helpers

        /// <summary>Display: Entry Number (for UI)</summary>
        public string EntryNo => $"GLE-{PostingSequenceNumber:D8}";

        /// <summary>Display: Formatted Debit Amount</summary>
        public string FormattedDebit => DebitAmount > 0 ? DebitAmount.ToString("N2") : "-";

        /// <summary>Display: Formatted Credit Amount</summary>
        public string FormattedCredit => CreditAmount > 0 ? CreditAmount.ToString("N2") : "-";

        /// <summary>Display: Net Amount (Debit - Credit)</summary>
        public decimal NetAmount => DebitAmount - CreditAmount;
    }

    // Note: GLSourceTypes is defined in MasterDataModels.cs
}
