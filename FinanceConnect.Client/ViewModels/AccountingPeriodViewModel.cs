using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public enum AccountingPeriodStatus
    {
        Draft,
        Open,
        SoftClosed,
        Closed
    }

    public enum AccountingPeriodType
    {
        Normal,
        Adjustment
    }

    public class AccountingPeriodModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, StringLength(15)]
        [RegularExpression(@"^[A-Z0-9_-]+$", ErrorMessage = "Only letters, numbers, - and _ allowed")]
        public string PeriodCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Fiscal Year Name is required")]
        [StringLength(100)]
        public string PeriodName { get; set; } = string.Empty;

        [Required]
        public Guid FiscalYearId { get; set; }

        [Required]
        public Guid CompanyId { get; set; }

        [Required(ErrorMessage = "Period Number is required")]
        [Range(1, 13)]
        public int PeriodNumber { get; set; }

        [Required(ErrorMessage = "Period Type is required")]
        public AccountingPeriodType PeriodType { get; set; } = AccountingPeriodType.Normal;

        [Required(ErrorMessage = "Start Date is required")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required")]
        public DateTime? EndDate { get; set; }

        public bool IsCurrentPeriod { get; set; }

        public AccountingPeriodStatus Status { get; set; } = AccountingPeriodStatus.Draft;

        public bool IsARLocked { get; set; }
        public bool IsAPLocked { get; set; }
        public bool IsBankLocked { get; set; }
        public bool IsGLLocked { get; set; }

        [StringLength(300)]
        public string? LockReason { get; set; }

        public DateTime? LockedAt { get; set; }
        public string? LockedBy { get; set; }

        // ========= Posting Controls =========
        public bool AllowBackdatedPosting { get; set; }  // derived
        public bool AllowFuturePosting { get; set; }     // derived

        public bool AllowAdjustmentJournalsInSoftClose { get; set; } = true;


        public int? MaxPostingDateOverrideDays { get; set; }

        // ========= Close Governance =========
        public bool RequireCloseChecklist { get; set; }
        public Guid? CloseChecklistRunId { get; set; }

        public DateTime? ClosedAt { get; set; }
        public string? ClosedBy { get; set; }

        [StringLength(500)]
        public string? CloseNotes { get; set; }

        // ========= System Audit =========
        public Guid? TenantId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
