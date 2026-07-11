using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public enum CompanyScopeMode
    {
        Global = 1,
        PerCompany = 2
    }

    public enum StageCategory
    {
        DraftStage = 1,
        ApprovalStage = 2,
        PostingStage = 3,
        FinalStage = 4
    }

    public enum BadgeTone
    {
        Neutral = 0,
        Success = 1,
        Warning = 2,
        Danger = 3
    }

    public class TransactionStatusModel
    {
        // ================= IDENTITY =================
        public Guid TransactionStatusId { get; set; } = Guid.NewGuid();
        public Guid TenantId { get; set; }
        public CompanyScopeMode CompanyScopeMode { get; set; } = CompanyScopeMode.Global;
        public Guid? CompanyId { get; set; }

        // ================= CODE & LABEL =================

        [Required, StringLength(30)]
        [RegularExpression(@"^[A-Z0-9_-]+$", ErrorMessage = "Only letters, numbers, - and _ allowed")]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        // ================= LIFECYCLE =================
        public StageCategory StageCategory { get; set; }
        public bool IsFinal { get; set; } = false;

        // ================= CONTROL FLAGS =================
        public bool AllowHeaderEdit { get; set; }
        public bool AllowLineEdit { get; set; }
        public bool AllowDelete { get; set; }
        public bool AllowSubmit { get; set; }
        public bool AllowApproveReject { get; set; }
        public bool AllowPost { get; set; }
        public bool AllowReverse { get; set; }
        public bool AllowCancel { get; set; }

        // ================= UI DISPLAY =================
        public int DisplayOrder { get; set; }
        public string? BadgeLabel { get; set; }
        public BadgeTone BadgeTone { get; set; } = BadgeTone.Neutral;

        // ================= SYSTEM =================
        public bool IsActive { get; set; } = true;
        public bool IsSystemDefined { get; set; }
        public bool IsDeleted { get; set; } = false;

        // ================= AUDIT =================
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public string? DeletedBy { get; set; }
        public byte[]? RowVersion { get; set; }
    }
}
