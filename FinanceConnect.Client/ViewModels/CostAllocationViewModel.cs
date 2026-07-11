using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{

    // ── Enums ──────────────────────────────────────────────────────────────────

    public enum AllocationType
    {
        BudgetAllocation,
        ActualAllocation,
        OverheadDistribution,
        Reallocation,
        StatisticalAllocation,
        ManualAdjustment
    }

    public enum AllocationStatus
    {
        Draft,
        Prepared,
        Submitted,
        Approved,
        Applied,
        Locked,
        Closed,
        Reversed,
        Archived
    }

    public enum AllocationMethod
    {
        FixedPercentage,
        FixedAmount,
        HeadcountBased,
        FloorAreaBased,
        RevenueBased,
        UsageBased,
        DriverBased,
        Manual
    }

    public enum AllocationBasisType
    {
        Static,
        Dynamic,
        ImportedDriver,
        ManualEntry
    }

    public enum RoundingRule
    {
        RoundToNearest,
        RoundUp,
        RoundDown,
        ResidualToLastLine,
        ResidualToPrimaryTarget
    }

    public enum ScopeTypecost
    {
        Company,
        Branch,
        Department,
        CostCenterGroup,
        Project
    }

    public enum SourceAmountType
    {
        Budgeted,
        Actual,
        Statistical,
        Adjusted
    }

    public enum AllocationLineStatus
    {
        Draft,
        Calculated,
        Approved,
        Applied,
        Locked,
        Reversed
    }

    // ── Header Model ───────────────────────────────────────────────────────────

    public class CostAllocationViewModel
    {
        // Core Identity
        public Guid CostAllocationId { get; set; }
        public Guid TenantId { get; set; }
        public Guid CompanyId { get; set; }

        [Required(ErrorMessage = "Allocation Code is required.")]
        [StringLength(50, ErrorMessage = "Allocation Code cannot exceed 50 characters.")]
        public string AllocationCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Allocation Name is required.")]
        [StringLength(200, ErrorMessage = "Allocation Name cannot exceed 200 characters.")]
        public string AllocationName { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Allocation Type is required.")]
        public AllocationType AllocationType { get; set; }

        public AllocationStatus AllocationStatus { get; set; } = AllocationStatus.Draft;

        [Required(ErrorMessage = "Allocation Date is required.")]
        public DateTime AllocationDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Effective Date is required.")]
        public DateTime EffectiveDate { get; set; } = DateTime.Today;

        // Period & Scope
        public Guid? BudgetId { get; set; }
        public Guid? BudgetPeriodId { get; set; }
        public Guid FiscalYearId { get; set; }
        public Guid? AccountingPeriodId { get; set; }

        [Required(ErrorMessage = "Scope Type is required.")]
        public ScopeTypecost ScopeType { get; set; }

        public Guid? BranchId { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? ProjectId { get; set; }

        // Source Definition
        public Guid? SourceCostCenterId { get; set; }
        public Guid? SourceBudgetLineId { get; set; }
        public Guid? SourceGLAccountId { get; set; }
        public string? SourceCategoryCode { get; set; }

        [Required(ErrorMessage = "Source Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Source Amount must be greater than zero.")]
        public decimal SourceAmount { get; set; }

        [Required(ErrorMessage = "Source Amount Type is required.")]
        public SourceAmountType SourceAmountType { get; set; }

        public string? SourceReferenceText { get; set; }

        // Allocation Method
        [Required(ErrorMessage = "Allocation Method is required.")]
        public AllocationMethod AllocationMethod { get; set; }

        [Required(ErrorMessage = "Allocation Basis Type is required.")]
        public AllocationBasisType AllocationBasisType { get; set; }

        public string? DriverReferenceCode { get; set; }
        public DateTime? DriverAsOfDate { get; set; }
        public bool IsManualOverrideAllowed { get; set; } = false;

        [Required(ErrorMessage = "Rounding Rule is required.")]
        public RoundingRule RoundingRule { get; set; } = RoundingRule.ResidualToLastLine;

        [Range(0, double.MaxValue, ErrorMessage = "Minimum Allocation Amount cannot be negative.")]
        public decimal? MinimumAllocationAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Maximum Allocation Amount cannot be negative.")]
        public decimal? MaximumAllocationAmount { get; set; }

        public bool MustFullyAllocateSource { get; set; } = true;

        // Result Summary (Derived)
        public int TotalTargetCount { get; set; }
        public decimal TotalAllocatedAmount { get; set; }
        public decimal UnallocatedAmount { get; set; }
        public decimal AllocationDifferenceAmount { get; set; }
        public bool IsFullyAllocated { get; set; }
        public Guid? PrimaryTargetCostCenterId { get; set; }

        // Workflow & Governance
        public Guid PreparedByUserId { get; set; }
        public Guid? SubmittedByUserId { get; set; }
        public Guid? ApprovedByUserId { get; set; }
        public DateTime? SubmittedOn { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public bool IsLocked { get; set; } = false;
        public DateTime? LockedOn { get; set; }
        public Guid? LockedBy { get; set; }
        public string? ReversalReason { get; set; }
        public Guid? PreviousAllocationId { get; set; }

        // Notes & Evidence
        public string? AllocationAssumptionText { get; set; }
        public string? Notes { get; set; }
        public int AttachmentCount { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation
        public List<CostAllocationLine> Lines { get; set; } = new();
    }

    // ── Line Model ─────────────────────────────────────────────────────────────

    public class CostAllocationLine
    {
        public Guid CostAllocationLineId { get; set; }
        public Guid CostAllocationId { get; set; }
        public int LineNumber { get; set; }

        [Required(ErrorMessage = "Target Cost Center is required.")]
        public Guid TargetCostCenterId { get; set; }

        public string TargetCostCenterName { get; set; } = string.Empty;
        public Guid? TargetBudgetLineId { get; set; }
        public decimal? BasisValue { get; set; }

        [Range(0, 100, ErrorMessage = "Allocation % must be between 0 and 100.")]
        public decimal? AllocationPercent { get; set; }

        [Required(ErrorMessage = "Allocated Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Allocated Amount must be greater than zero.")]
        public decimal AllocatedAmount { get; set; }

        public bool ManualOverrideFlag { get; set; } = false;
        public string? ManualOverrideReason { get; set; }
        public decimal? ReceiverWeight { get; set; }
        public AllocationLineStatus AllocationLineStatus { get; set; } = AllocationLineStatus.Draft;
        public string? LineNotes { get; set; }
    }

    // ── List DTO ───────────────────────────────────────────────────────────────

    public class CostAllocationListDto
    {
        public Guid CostAllocationId { get; set; }
        public string AllocationCode { get; set; } = string.Empty;
        public string AllocationName { get; set; } = string.Empty;
        public AllocationType AllocationType { get; set; }
        public AllocationStatus AllocationStatus { get; set; }
        public AllocationMethod AllocationMethod { get; set; }
        public DateTime AllocationDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public decimal SourceAmount { get; set; }
        public decimal TotalAllocatedAmount { get; set; }
        public bool IsFullyAllocated { get; set; }
        public bool IsLocked { get; set; }
        public int TotalTargetCount { get; set; }
        public string? SourceCostCenterName { get; set; }
        public DateTime CreatedAt { get; set; }

        public string? AllocationAssumptionText { get; set; }
        public string? Notes { get; set; }
        public string? ReversalReason { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public DateTime? SubmittedOn { get; set; }
        public DateTime? LockedOn { get; set; }
        public Guid? PreviousAllocationId { get; set; }
        public string? SourceReferenceText { get; set; }
        public ScopeTypecost ScopeType { get; set; }
        public SourceAmountType SourceAmountType { get; set; }
        public RoundingRule RoundingRule { get; set; }
        public bool MustFullyAllocateSource { get; set; }
        public decimal UnallocatedAmount { get; set; }
        public int AttachmentCount { get; set; }

        public List<CostAllocationLine> Lines { get; set; } = new();
    }

}
