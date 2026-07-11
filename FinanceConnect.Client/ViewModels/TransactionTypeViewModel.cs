using System;
using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{

    public enum SourceCategory
    {
        AP,
        AR,
        BANK,
        EXPENSE,
        ASSET,
        GL
    }

    // TransactionNature (Optional)
    public enum TransactionNature
    {
        Receipt,
        Payment,
        Adjustment,
        Accrual,
        Transfer
    }

    //  Document No Assignment Timing
    public enum DocumentNoAssignmentTimings
    {
        OnDraftSave,
        OnApproval,
        OnPosting
    }
    public class TransactionTypeModel
    {
        // ================= IDENTITY =================
        public Guid TransactionTypeId { get; set; }

        [Required]
        public Guid TenantId { get; set; }


        public Guid? CompanyId { get; set; }

        [Required(ErrorMessage = "Company is required")]
        public string CompanyName { get; set; } = string.Empty;

        [Required, StringLength(30)]
        [RegularExpression(@"^[A-Z0-9_-]+$", ErrorMessage = "Only letters, numbers, - and _ allowed")]
        public string Code { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        // ================= CLASSIFICATION =================
        [Required(ErrorMessage = "Source Category is required")]
        public SourceCategory? SourceCategory { get; set; }

        public TransactionNature? TransactionNature { get; set; }

        // ================= POSTING =================
        public bool IsPostable { get; set; } = true;
        [Required(ErrorMessage = "DefaultPostingProfileId is required")]
        public Guid? DefaultPostingProfileId { get; set; }
        public bool AllowAutoPost { get; set; } = false;

        // ================= APPROVAL =================
        public bool RequiresApproval { get; set; } = true;

        [MaxLength(100)]
        [Required(ErrorMessage = "ApprovalWorkflowKey is required")]
        public string? ApprovalWorkflowKey { get; set; }

        // ================= DOCUMENT NUMBER =================
        [Required(ErrorMessage = "Document Number Series is required")]
        public Guid? DocumentNumberSeriesId { get; set; }



        [Required(ErrorMessage = "Document No Assignment Timing is required")]
        public DocumentNoAssignmentTimings? DocumentNoAssignmentTiming { get; set; } = DocumentNoAssignmentTimings.OnApproval;
        // OnDraftSave | OnApproval | OnPosting

        // ================= EDIT & REVERSAL =================
        public bool AllowDraftEdit { get; set; } = true;
        public bool AllowDraftCancel { get; set; } = true;
        public bool AllowReversal { get; set; } = true;
        public bool AllowManualEntry { get; set; } = false;

        // ================= CURRENCY =================
        public bool AllowForeignCurrency { get; set; } = false;
        public bool AllowNegativeLines { get; set; } = false;
        public int? AmountPrecisionPolicy { get; set; }

        // ================= STATUS =================
        public bool IsActive { get; set; } = true;
        public bool IsSystemDefined { get; set; } = false;


        // ================= AUDIT =================
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }

    }
}
