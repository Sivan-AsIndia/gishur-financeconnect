using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public class BudgetLineViewModel
    {
        // ── Enums ──────────────────────────────────────────────────────────────

        public enum LineTypeEnum
        {
            Expense = 1,
            Revenue = 2,
            Capex = 3,
            Statistical = 4,
            Transfer = 5
        }

        public enum DistributionModeEnum
        {
            Manual = 1,
            EvenSpread = 2,
            WeightedSpread = 3,
            SeasonalTemplate = 4,
            ActualTrendBased = 5
        }

        public enum ActualMatchModeEnum
        {
            ByGLAccountOnly = 1,
            ByGLAndCostCenter = 2,
            ByGLAndBranch = 3,
            ByDimensionSet = 4,
            CustomRule = 5
        }

        public enum ActualSourceScopeEnum
        {
            PostedJournals = 1,
            APBills = 2,
            ARInvoices = 3,
            AllPostedActuals = 4
        }

        public enum LineStatusEnum
        {
            Draft = 1,
            Active = 2,
            Revised = 3,
            Locked = 4,
            Closed = 5,
            Archived = 6
        }

        public enum ResponsibilityTypeEnum
        {
            DirectOwner = 1,
            SharedOwner = 2,
            AllocatedOwner = 3,
            Informational = 4
        }

        // ── Model ──────────────────────────────────────────────────────────────

        public class BudgetLine
        {
            // ─── Section 1: Core Identity ────────────────────────────────────
            public Guid BudgetLineId { get; set; } = Guid.NewGuid();
            public Guid TenantId { get; set; }
            public Guid CompanyId { get; set; }

            [Required(ErrorMessage = "Budget is required")]
            public Guid BudgetId { get; set; }

            [Required(ErrorMessage = "Line Number is required")]
            [Range(1, int.MaxValue, ErrorMessage = "Line Number must be > 0")]
            public int LineNumber { get; set; }

            [MaxLength(30)]
            public string? LineCode { get; set; }

            [Required(ErrorMessage = "Line Name is required")]
            [MaxLength(200)]
            public string LineName { get; set; } = string.Empty;

            [MaxLength(1000)]
            public string? Description { get; set; }

            // ─── Section 2: Line Classification ──────────────────────────────
            [Required(ErrorMessage = "Line Type is required")]
            public LineTypeEnum LineType { get; set; } = LineTypeEnum.Expense;

            [Required(ErrorMessage = "Budget Category is required")]
            [MaxLength(50)]
            public string BudgetCategoryCode { get; set; } = string.Empty;

            public Guid? GLAccountId { get; set; }
            public string? GLAccountName { get; set; }

            public Guid? GLAccountGroupId { get; set; }
            public string? GLAccountGroupName { get; set; }

            [MaxLength(50)]
            public string? ExpenseNature { get; set; }

            public bool IsCapexFlag { get; set; }

            // ─── Section 3: Ownership & Dimensions ───────────────────────────
            public Guid? CostCenterId { get; set; }
            public string? CostCenterName { get; set; }

            public Guid? DepartmentId { get; set; }
            public string? DepartmentName { get; set; }

            public Guid? BranchId { get; set; }
            public string? BranchName { get; set; }

            public Guid? ProjectId { get; set; }
            public string? ProjectName { get; set; }

            public Guid? OwnerUserId { get; set; }
            public string? OwnerUserName { get; set; }

            public ResponsibilityTypeEnum? ResponsibilityType { get; set; }

            public string? DimensionScopeJson { get; set; }

            // ─── Section 4: Planned Amounts ──────────────────────────────────
            [Required(ErrorMessage = "Original Planned Amount is required")]
            [Range(0, double.MaxValue, ErrorMessage = "Planned Amount must be >= 0")]
            public decimal OriginalPlannedAmount { get; set; }

            public decimal? RevisedAmount { get; set; }

            public decimal EffectiveBudgetAmount =>
                RevisedAmount ?? OriginalPlannedAmount;

            public decimal? ReleasedAmount { get; set; }
            public decimal? ReservedAmount { get; set; }
            public decimal? AdjustmentAmount { get; set; }

            // ─── Section 5: Actual Consumption & Control ─────────────────────
            public decimal ActualConsumedAmount { get; set; }
            public decimal? CommittedAmount { get; set; }

            public decimal AvailableBalanceAmount =>
                EffectiveBudgetAmount - ActualConsumedAmount - (CommittedAmount ?? 0);

            public decimal UtilizationPercent =>
                EffectiveBudgetAmount > 0
                    ? Math.Round(ActualConsumedAmount / EffectiveBudgetAmount * 100, 2)
                    : 0;

            public decimal? VarianceToDateAmount { get; set; }

            public bool IsOverBudget => ActualConsumedAmount > EffectiveBudgetAmount;

            public decimal OverBudgetAmount =>
                Math.Max(ActualConsumedAmount - EffectiveBudgetAmount, 0);

            public bool IsUnderUtilized =>
                EffectiveBudgetAmount > 0 && UtilizationPercent < 50;

            // ─── Section 6: Period Distribution ──────────────────────────────
            [Required(ErrorMessage = "Distribution Mode is required")]
            public DistributionModeEnum DistributionMode { get; set; } = DistributionModeEnum.Manual;

            public bool HasPeriodDistribution { get; set; }

            [MaxLength(50)]
            public string? DistributionTemplateCode { get; set; }

            public string? PeriodDistributionJson { get; set; }

            public decimal? LinePeriodTotalCheck { get; set; }

            // ─── Section 7: Actual Matching Rules ────────────────────────────
            [Required(ErrorMessage = "Actual Match Mode is required")]
            public ActualMatchModeEnum ActualMatchMode { get; set; } = ActualMatchModeEnum.ByGLAccountOnly;

            public string? ActualMatchRuleJson { get; set; }

            public bool IncludeAllocatedActuals { get; set; } = true;
            public bool IncludeTaxInActuals { get; set; }

            public ActualSourceScopeEnum ActualSourceScope { get; set; } = ActualSourceScopeEnum.AllPostedActuals;

            // ─── Section 8: Workflow & Locking ───────────────────────────────
            [Required(ErrorMessage = "Line Status is required")]
            public LineStatusEnum LineStatus { get; set; } = LineStatusEnum.Draft;

            public bool IsLocked { get; set; }

            public DateTime? LockedOn { get; set; }
            public Guid? LockedBy { get; set; }

            [MaxLength(500)]
            public string? RevisionReason { get; set; }

            public DateTime? ClosedOn { get; set; }
            public Guid? ClosedBy { get; set; }

            // ─── Section 9: Notes, Assumptions & Evidence ────────────────────
            [MaxLength(1500)]
            public string? PlanningAssumptionText { get; set; }

            [MaxLength(1500)]
            public string? Notes { get; set; }

            public int AttachmentCount { get; set; }

            // ─── Section 10: System Audit Fields ─────────────────────────────
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public Guid? CreatedBy { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public Guid? UpdatedBy { get; set; }
            public bool IsDeleted { get; set; }
        }
    }
}
