using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace FinanceConnect.Client.ViewModels
{

    public enum JournalType
    {
        General,
        Purchase,
        Sales,
        CashReceipt,
        CashPayment,
        BankReceipt,
        BankPayment,
        Adjustment,
        OpeningBalance,
        Other
    }

    public enum BranchDefaultMode
    {
        UseCompanyDefaultBranch,
        UseUserDefaultBranch,
        RequireManualSelection,
        ForceSpecificBranch
    }


    public static class NumberResetFrequency
    {
        public const string Never = "Never";
        public const string Yearly = "Yearly";
        public const string Monthly = "Monthly";

        public static readonly string[] All = new[] { Never, Yearly, Monthly };

        public static string GetDisplayName(string type) => type switch
        {
            Never => "Never",
            Yearly => "Yearly (FiscalYear)",
            Monthly => "Monthly (AccountingPeriod)",
            _ => type
        };
    }
    public enum JournalStatus
    {
        Draft,
        Active,
        Inactive
    }

    public class JournalModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, StringLength(15)]
        [RegularExpression(@"^[A-Z0-9_-]+$",
            ErrorMessage = "Only letters, numbers, - and _ allowed")]
        public string JournalCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Journal Name is required"), StringLength(200)]
        public string JournalName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Company is required")]
        public Guid? CompanyId { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }


        [Required(ErrorMessage = "Journal Type is required")]
        public JournalType? JournalType { get; set; }

        [Required(ErrorMessage = "Ledger is required")]
        public Guid? LedgerId { get; set; }

        [Required]
        public BranchDefaultMode DefaultBranchMode { get; set; }
            = BranchDefaultMode.UseCompanyDefaultBranch;

        public Guid? ForcedBranchId { get; set; }


        [Required(ErrorMessage = "Reset Numbering Type is required")]
        public string ResetNumbering { get; set; } = "";

        [Required(ErrorMessage = "Document Number Series is required")]
        public Guid? DocumentNumberSeriesId { get; set; }

        [StringLength(10)]
        public string? EntryNumberPrefix { get; set; }

        public bool AllowManualEntryNumber { get; set; } = false;


        public bool RequireApprovalBeforePosting { get; set; } = true;
        public bool EnforceAccountingPeriodOpen { get; set; } = true;
        public bool AllowBackdatedPostingOverride { get; set; } = false;
        public bool AllowFuturePostingOverride { get; set; } = false;
        public bool AllowReversalEntries { get; set; } = true;

        public int MaxLinesPerEntry { get; set; } = 500;


        public bool NarrationRequired { get; set; } = true;

        [StringLength(200, ErrorMessage = "Document Number Series cannot exceed 200 characters")]
        public string? NarrationTemplate { get; set; }

        public bool AttachmentRequired { get; set; } = false;
        public bool AllowLineLevelNarration { get; set; } = true;


        public JournalStatus Status { get; set; } = JournalStatus.Draft;


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }


        public bool HasJournalEntries { get; set; } = false;
    }
}
