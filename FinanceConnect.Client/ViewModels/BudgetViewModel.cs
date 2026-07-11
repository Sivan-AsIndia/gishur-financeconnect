using System;
using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public enum BudgetType
    {
        Original,
        Revised,
        Supplementary,
        Rolling,
        Project,
        ForecastBaseline
    }

    public enum BudgetNature
    {
        OperatingExpense,
        Revenue,
        CapitalExpenditure,
        Mixed
    }

    public enum PlanningLevel
    {
        Company,
        Branch,
        Department,
        CostCenterGroup,
        Project,
        Mixed
    }

    public enum ScenarioType
    {
        Base,
        Optimistic,
        Conservative,
        Stretch,
        ApprovedOfficial
    }

    public enum PeriodGranularity
    {
        Monthly,
        Quarterly,
        HalfYearly,
        Yearly,
        Custom
    }
    public enum WorkflowStatus
    {
        Draft,
        UnderPreparation,
        Submitted,
        UnderReview,
        Approved,
        Rejected,
        Locked,
        Closed,
        Archived

    }

    public class BudgetViewModel
    {
        // 🔹 Identity
        [Required]
        public Guid BudgetId { get; set; }

        [Required]
        public Guid TenantId { get; set; }

        [Required(ErrorMessage = "Company is required.")]
        public Guid? CompanyId { get; set; }

        // 🔹 Basic Info
        [Required(ErrorMessage = "Budget code is required.")]
        [StringLength(30)]
        public string BudgetCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Budget name is required.")]
        [StringLength(200)]
        public string BudgetName { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        // 🔹 Classification
        [Required(ErrorMessage = "Budget type is required.")]
        public BudgetType BudgetType { get; set; }

        [Required(ErrorMessage = "Budget nature is required.")]
        public BudgetNature BudgetNature { get; set; }

        [Required(ErrorMessage = "Planning level is required.")]
        public PlanningLevel PlanningLevel { get; set; }

        public ScenarioType? ScenarioType { get; set; }

        // 🔹 Planning Horizon
        [Required(ErrorMessage = "Fiscal year is required.")]
        public Guid? FiscalYearId { get; set; }

        [Required(ErrorMessage = "Start date is required.")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "Period granularity is required.")]
        public PeriodGranularity PeriodGranularity { get; set; }
        public WorkflowStatus WorkflowStatus { get; set; }

        public int TotalPeriodsPlanned { get; set; }

        // 🔹 Scope
        public Guid? BranchId { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? CostCenterGroupId { get; set; }

        [Required(ErrorMessage = "Currency is required.")]
        public Guid? CurrencyId { get; set; }

        public string? ScopeJson { get; set; }

        // 🔹 Ownership & Workflow
        [Required(ErrorMessage = "Budget owner is required.")]
        public Guid? BudgetOwnerUserId { get; set; }

        [Required]
        public Guid? PreparedByUserId { get; set; }

        public Guid? SubmittedByUserId { get; set; }
        public Guid? ApprovedByUserId { get; set; }

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "Draft";
        // Draft / Submitted / Approved / Locked / Closed / Archived

        public DateTime? SubmittedOn { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public DateTime? RejectedOn { get; set; }

        [StringLength(500)]
        public string? RejectionReason { get; set; }

        // 🔹 Versioning
        [Required]
        [Range(1, int.MaxValue)]
        public int VersionNumber { get; set; } = 1;

        [Required]
        [Range(0, int.MaxValue)]
        public int RevisionNumber { get; set; } = 0;

        public Guid? PreviousBudgetId { get; set; }
        public Guid? LinkedFinancialPlanId { get; set; }

        public bool IsCurrentApprovedVersion { get; set; }

        [StringLength(1000)]
        public string? RevisionReason { get; set; }

        // 🔹 Summary
        [Range(0, double.MaxValue)]
        public decimal TotalBudgetAmount { get; set; }

        public decimal TotalRevenueBudgetAmount { get; set; }
        public decimal TotalExpenseBudgetAmount { get; set; }
        public decimal TotalCapexBudgetAmount { get; set; }

        public decimal ConsumedActualAmount { get; set; }
        public decimal AvailableBalanceAmount { get; set; }

        public bool HasPeriodDistributionGenerated { get; set; }
        public bool HasBudgetLines { get; set; }

        // 🔹 Governance
        public bool IsLocked { get; set; } = false;

        public DateTime? LockedOn { get; set; }
        public Guid? LockedBy { get; set; }

        [StringLength(300)]
        public string? LockReason { get; set; }

        public bool IsArchived { get; set; } = false;

        [StringLength(500)]
        public string? ArchiveReason { get; set; }

        // 🔹 Notes
        [StringLength(2000)]
        public string? Notes { get; set; }

        public int AttachmentCount { get; set; }

        // 🔹 Audit
        [Required]
        public DateTime CreatedAt { get; set; }

        [StringLength(100)]
        public string CreatedBy { get; set; } = "system";

        public DateTime? UpdatedAt { get; set; }

        [StringLength(100)]
        public string? UpdatedBy { get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [StringLength(100)]
        public string? DeletedBy { get; set; }

        // 🔹 Status Helpers (like your style)
        public bool IsDraft => Status == "Draft";
        public bool IsSubmitted => Status == "Submitted";
        public bool IsApproved => Status == "Approved";
        public bool IsLockedStatus => Status == "Locked";
        public bool IsClosed => Status == "Closed";
        public bool IsArchivedStatus => Status == "Archived";
    }
}