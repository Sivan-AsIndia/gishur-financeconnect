using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{


    public enum AppliesToEntityType
    {
        FinancialTransaction,
        JournalEntry,
        ReceiptVoucher,
        PaymentVoucher,
        CustomerInvoice,
    }
    public enum SequenceScopeMode
    {
        CompanyWide,
        BranchSpecific
    }

    public enum ResetFrequency
    {
        Never,
        Yearly,
        Monthly,
        AccountingPeriod
    }

    public enum FiscalYearMode
    {
        CalendarYear,
        CompanyFiscalYear
    }

    public enum ReservationMode
    {
        AllocateOnAssignment,
        ReserveThenCommit
    }

    public enum GapHandlingPolicy
    {
        AllowGapsWithAudit,
        StrictNoGaps
    }



    public class DocumentNumberSeriesModel
    {
        public Guid DocumentNumberSeriesId { get; set; }
        public Guid TenantId { get; set; }


        [Required]
        public Guid? CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        // Identity
        [Required(ErrorMessage = "Series Code is required")]
        [StringLength(50)]
        [RegularExpression("^[A-Z0-9_-]+$", ErrorMessage = "Only letters, numbers, - and _ allowed")]
        public string SeriesCode { get; set; } = string.Empty;


        [Required(ErrorMessage = "Series Name is required")]
        [StringLength(150)]
        public string SeriesName { get; set; } = default!;
        public string? Description { get; set; }

        // Applies To
        [Required]
        public AppliesToEntityType AppliesToEntityType { get; set; }
                = AppliesToEntityType.FinancialTransaction;

        // Scope
        [Required]
        public SequenceScopeMode SequenceScopeMode { get; set; }

        // Reset Policy
        [Required]
        public ResetFrequency ResetFrequency { get; set; }
        public FiscalYearMode? FiscalYearMode { get; set; }
        public string? ResetKeyTemplate { get; set; }

        // Format Template
        [StringLength(120)]
        public string? PrefixTemplate { get; set; }

        [Required]
        [StringLength(20)]
        public string SequenceTokenFormat { get; set; }

        [StringLength(80)]
        public string? SuffixTemplate { get; set; }
        public string Separator { get; set; } = "/";

        // Range & Counter Defaults
        public long MinSequenceValue { get; set; } = 1;
        public long? MaxSequenceValue { get; set; }
        public int IncrementBy { get; set; } = 1;
        public int NumericWidth { get; set; }

        // Assignment / Gap Policy
        public bool AllowNumberPreview { get; set; } = true;
        public ReservationMode ReservationMode { get; set; } = ReservationMode.AllocateOnAssignment;
        public GapHandlingPolicy GapHandlingPolicy { get; set; } = GapHandlingPolicy.AllowGapsWithAudit;

        // Activation / Versioning
        public bool IsActive { get; set; } = true;
        public bool IsLocked { get; set; } = false;
        public bool IsSystemDefined { get; set; } = false;
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

    }


}
