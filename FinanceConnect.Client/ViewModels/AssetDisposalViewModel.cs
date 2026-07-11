using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public enum AssetDisposalStatus
    {
        Draft,
        Submitted,
        Approved,
        Rejected,
        Posted,
        Cancelled,
        Reversed,
        Closed
    }

    public enum AssetDisposalType
    {
        Sale,
        Scrap,
        WriteOff,
        TheftLoss,
        Donation,
        TransferOut
    }

    public enum ProceedsMode
    {
        Cash,
        Bank,
        Receivable,
        Other
    }

    public enum GainLossType
    {
        Gain,
        Loss,
        Neutral
    }


    public class AssetDisposalViewModel
    {


        [Required]
        public Guid AssetDisposalId { get; set; }

        [Required]
        public Guid TenantId { get; set; }

        [Required]
        public Guid? CompanyId { get; set; }

        [Required]
        public Guid? BranchId { get; set; }

        [Required]
        [StringLength(40)]
        public string DisposalNumber { get; set; } = "";

        [Required]
        public AssetDisposalStatus DisposalStatus { get; set; } = AssetDisposalStatus.Draft;

        [Required]
        public AssetDisposalType DisposalType { get; set; }

        [Required]
        public DateTime? DisposalDate { get; set; }

        public DateTime? PostingDate { get; set; }

        [StringLength(1000)]
        public string? Narration { get; set; }


        [Required]
        public Guid? FixedAssetId { get; set; }

        [StringLength(40)]
        public string AssetNumberSnapshot { get; set; } = "";

        [StringLength(200)]
        public string AssetNameSnapshot { get; set; } = "";

        public Guid? AssetCategoryIdSnapshot { get; set; }

        [StringLength(200)]
        public string AssetCategoryNameSnapshot { get; set; } = "";

        [StringLength(30)]
        public string AssetStatusSnapshot { get; set; } = "";

        public DateTime? InServiceDateSnapshot { get; set; }

        public decimal TotalCapitalizedCostSnapshot { get; set; }

        public decimal AccumulatedDepreciationAsOfDisposalSnapshot { get; set; }

        public decimal NetBookValueAsOfDisposalSnapshot { get; set; }



        public decimal ProceedsAmount { get; set; }

        public decimal DisposalExpenseAmount { get; set; }

        public decimal NetProceedsAmount { get; set; }

        public ProceedsMode? ProceedsMode { get; set; }

        public Guid? CashAccountId { get; set; }

        public Guid? BankAccountId { get; set; }

        [StringLength(100)]
        public string? ReferenceNumber { get; set; }

        [StringLength(200)]
        public string? BuyerName { get; set; }

        [StringLength(200)]
        public string? BuyerContact { get; set; }

        [StringLength(50)]
        public string? SaleInvoiceNumber { get; set; }

        public DateTime? SaleInvoiceDate { get; set; }

        public Guid? ARCustomerId { get; set; }


        // GAIN / LOSS CALCULATION

        public decimal GainLossAmount { get; set; }

        public GainLossType GainLossType { get; set; }

        public bool DisposalResidualPolicyAppliedFlag { get; set; }


        // GL ACCOUNT SNAPSHOTS

        public Guid? AssetCostGLAccountIdSnapshot { get; set; }

        public Guid? AccumulatedDepreciationGLAccountIdSnapshot { get; set; }

        public Guid? GainOnDisposalGLAccountIdSnapshot { get; set; }

        public Guid? LossOnDisposalGLAccountIdSnapshot { get; set; }

        public Guid? DisposalExpenseGLAccountIdSnapshot { get; set; }

        public Guid? ProceedsGLAccountIdSnapshot { get; set; }

        public Guid? JournalEntryId { get; set; }

        public Guid? ReversalJournalEntryId { get; set; }

        public DateTime? PostedOn { get; set; }

        [StringLength(100)]
        public string? PostedBy { get; set; }


        // WORKFLOW / APPROVAL

        public DateTime? SubmittedOn { get; set; }

        public DateTime? ApprovedOn { get; set; }

        [StringLength(100)]
        public string? ApprovedBy { get; set; }

        [StringLength(500)]
        public string? ApprovalNotes { get; set; }

        [StringLength(500)]
        public string? ReversalReason { get; set; }


        // ATTACHMENTS

        public int AttachmentCount { get; set; }


        // SYSTEM AUDIT FIELDS

        [Required]
        public DateTime CreatedAt { get; set; }

        [StringLength(100)]
        public string CreatedBy { get; set; } = "system";

        public DateTime? UpdatedAt { get; set; }

        [StringLength(100)]
        public string? UpdatedBy { get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedAt { get; set; }

        [StringLength(100)]
        public string? DeletedBy { get; set; }


        // HELPER FLAGS

        public bool IsDraft => DisposalStatus == AssetDisposalStatus.Draft;

        public bool IsPosted => DisposalStatus == AssetDisposalStatus.Posted;

        public bool IsApproved => DisposalStatus == AssetDisposalStatus.Approved;

        public bool IsSubmitted => DisposalStatus == AssetDisposalStatus.Submitted;

        public bool IsReversed => DisposalStatus == AssetDisposalStatus.Reversed;

    }
}
