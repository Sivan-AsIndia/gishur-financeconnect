using System;
using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public class CostCenterModel
    {
        // ─── Core Identity ───────────────────────────────────────────
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TenantId { get; set; }
        public Guid CompanyId { get; set; }
        public string? CompanyName { get; set; }

        [Required(ErrorMessage = "Cost Center Code is required")]
        [MaxLength(30)]
        public string CostCenterCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cost Center Name is required")]
        [MaxLength(200)]
        public string CostCenterName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? ShortName { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        // ─── Classification ──────────────────────────────────────────
        [Required(ErrorMessage = "Cost Center Type is required")]
        public string CostCenterType { get; set; } = string.Empty;
        // Operational | Administrative | RevenueSupport | SharedService | Project | Regional | Corporate

        [Required(ErrorMessage = "Control Nature is required")]
        public string ControlNature { get; set; } = string.Empty;
        // CostOnly | RevenueOnly | Mixed | StatisticalOnly

        [Required(ErrorMessage = "Usage Mode is required")]
        public string UsageMode { get; set; } = string.Empty;
        // Budgeting | ActualTracking | BudgetAndActual | AllocationOnly | ReportingOnly

        public bool IsSharedServiceCenter { get; set; } = false;
        public bool IsAllocationSourceAllowed { get; set; } = false;
        public bool IsAllocationTargetAllowed { get; set; } = true;

        // ─── Organization & Hierarchy ─────────────────────────────────
        public Guid? ParentCostCenterId { get; set; }
        public Guid? BankGLAccountCode { get; set; }
        public string? ParentCostCenterName { get; set; }

        public int HierarchyLevel { get; set; } = 1; // Root = 1, derived

        [MaxLength(500)]
        public string? HierarchyPath { get; set; }

        public Guid? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }

        public Guid? BranchId { get; set; }
        public string? BranchName { get; set; }

        [MaxLength(30)]
        public string? RegionCode { get; set; }

        [MaxLength(50)]
        public string? BusinessUnitCode { get; set; }

        public Guid? ProjectId { get; set; }
        public string? ProjectName { get; set; }

        // ─── Ownership & Accountability ───────────────────────────────
        [Required(ErrorMessage = "Cost Center Owner is required")]
        public Guid CostCenterOwnerUserId { get; set; }
        public string? CostCenterOwnerName { get; set; }

        public Guid? ResponsibleManagerUserId { get; set; }
        public string? ResponsibleManagerName { get; set; }

        public Guid? FinanceReviewerUserId { get; set; }
        public string? FinanceReviewerName { get; set; }

        [MaxLength(50)]
        public string? ApprovalRoleCode { get; set; }
        // DeptHead | RegionalController | CFOReview

        [MaxLength(200)]
        public string? EmailDistributionGroup { get; set; }

        // ─── Financial Control ────────────────────────────────────────
        [Required(ErrorMessage = "Currency is required")]
        public Guid DefaultCurrencyId { get; set; }
        public string? DefaultCurrencyCode { get; set; }

        [Required(ErrorMessage = "Budget Control Mode is required")]
        public string BudgetControlMode { get; set; } = string.Empty;
        // Advisory | SoftControl | HardControl | ReportingOnly

        public decimal? TolerancePercent { get; set; }
        public decimal? ToleranceAmount { get; set; }

        public bool AllowNegativeBalance { get; set; } = false;
        public bool IsCapexAllowed { get; set; } = true;
        public bool IsOpexAllowed { get; set; } = true;

        [MaxLength(50)]
        public string? DefaultBudgetCategoryCode { get; set; }

        public Guid? DefaultGLAccountId { get; set; }
        public string? DefaultGLAccountCode { get; set; }

        // ─── Reporting & Allocation ───────────────────────────────────
        [MaxLength(50)]
        public string? ReportingGroupCode { get; set; }

        public string? AllocationBaseType { get; set; }
        // None | Headcount | FloorArea | RevenueShare | Usage | Manual

        public decimal? DefaultAllocationDriverValue { get; set; }

        public bool CanReceiveSharedCost { get; set; } = true;
        public bool CanDistributeSharedCost { get; set; } = false;

        [MaxLength(100)]
        public string? StatisticalKeyReference { get; set; }

        // ─── Lifecycle & Governance ───────────────────────────────────
        [Required(ErrorMessage = "Effective From is required")]
        public DateTime EffectiveFrom { get; set; } = DateTime.Today;

        public DateTime? EffectiveTo { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string CostCenterStatus { get; set; } = string.Empty;
        // Draft | Active | Inactive | Locked | Closed | Archived

        public bool IsActive { get; set; } = true;
        public bool IsLocked { get; set; } = false;

        public DateTime? LockedOn { get; set; }
        public Guid? LockedBy { get; set; }
        public string? LockedByName { get; set; }

        [MaxLength(500)]
        public string? ClosureReason { get; set; }

        public Guid? ReplacedByCostCenterId { get; set; }
        public string? ReplacedByCostCenterName { get; set; }

        // ─── Notes & Supporting ───────────────────────────────────────
        [MaxLength(1500)]
        public string? Notes { get; set; }

        [MaxLength(1000)]
        public string? OperationalRemarks { get; set; }

        public int AttachmentCount { get; set; } = 0;

        // ─── Audit Fields ─────────────────────────────────────────────
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
