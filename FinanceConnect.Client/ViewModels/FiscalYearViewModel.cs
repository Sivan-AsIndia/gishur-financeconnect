using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public enum FiscalYearStatus
    {
        Draft,
        Open,
        SoftClosed,
        Closed
    }


    public enum FiscalPeriodType
    {
        Monthly = 1,
        Quarterly = 2,
        FourFourFive = 3
    }

    public class PeriodOption
    {
        public int Value { get; set; }
        public string Label { get; set; } = "";
    }
    public class FiscalYearModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Identity
        [Required, StringLength(15)]
        [RegularExpression(@"^[A-Z0-9_-]+$", ErrorMessage = "Only letters, numbers, - and _ allowed")]
        public string FiscalYearCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Fiscal Year Name is required")]
        [StringLength(100)]
        public string FiscalYearName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Company is required")]
        public Guid? CompanyId { get; set; }

        //[Required, StringLength(100)]
        public string CompanyName { get; set; } = string.Empty;

        public FiscalYearStatus Status { get; set; } = FiscalYearStatus.Draft;

        // Date range
        [Required(ErrorMessage = "Start Date is required")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required")]
        public DateTime? EndDate { get; set; }

        public DateTime? BooksStartDateSnapshot { get; set; }

        // Period generation
        [Required]
        public FiscalPeriodType PeriodType { get; set; } = FiscalPeriodType.Monthly;


        public int NumberOfPeriods { get; set; } = 12;

        public string PeriodNamingConvention { get; set; } = "MMM yyyy";

        public bool AutoGeneratePeriods { get; set; } = true;
        public bool AutoOpenFirstPeriod { get; set; } = false;

        // Closing controls
        public bool AllowAdjustmentPostingAfterSoftClose { get; set; } = true;
        public bool RequirePeriodCloseChecklist { get; set; } = true;


        [StringLength(50)]
        public string? CloseChecklistTemplateId { get; set; }

        [StringLength(500)]
        public string? CloseReason { get; set; }

        public DateTime? ClosedAt { get; set; }
        public string? ClosedBy { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

}
