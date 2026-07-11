using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public class ExpenseCategoryViewModel
    {
        // ── Enums ──────────────────────────────────────────────────────────────

        public enum CategoryStatusEnum
        {
            Draft = 1,
            Active = 2,
            Inactive = 3,
            Archived = 4
        }

        public enum CategoryTypeEnum
        {
            OperatingExpense = 1,
            AdministrativeExpense = 2,
            SellingExpense = 3,
            EmployeeExpense = 4,
            ProjectExpense = 5,
            FinanceExpense = 6,
            Other = 7
        }

        public enum CategoryNatureEnum
        {
            Recurring = 1,
            NonRecurring = 2,
            Mixed = 3
        }

        public enum UsageScopeEnum
        {
            General = 1,
            EmployeeClaimsOnly = 2,
            SupplierExpensesOnly = 3,
            ProjectOnly = 4,
            FinanceOnly = 5,
            Mixed = 6
        }

        public enum DefaultCurrencyBehaviorEnum
        {
            CompanyBaseCurrency = 1,
            TransactionCurrencyAllowed = 2
        }

        public enum DefaultBudgetControlModeEnum
        {
            None = 1,
            Advisory = 2,
            SoftControl = 3,
            HardControl = 4
        }

        public enum DefaultTimingTreatmentEnum
        {
            ImmediateExpense = 1,
            Accrual = 2,
            Prepayment = 3,
            Mixed = 4
        }

        // ── Model ──────────────────────────────────────────────────────────────

        public class ExpenseCategory
        {
            // ─── Section 1: Core Identity (Header) ───────────────────────────
            public Guid ExpenseCategoryId { get; set; } = Guid.NewGuid();
            public Guid TenantId { get; set; }
            public Guid CompanyId { get; set; }

            [Required(ErrorMessage = "Category Code is required")]
            [MaxLength(30, ErrorMessage = "Category Code cannot exceed 30 characters")]
            public string CategoryCode { get; set; } = string.Empty;

            [Required(ErrorMessage = "Category Name is required")]
            [MaxLength(200, ErrorMessage = "Category Name cannot exceed 200 characters")]
            public string CategoryName { get; set; } = string.Empty;

            [MaxLength(50)]
            public string? ShortName { get; set; }

            [MaxLength(1000)]
            public string? Description { get; set; }

            [Required(ErrorMessage = "Status is required")]
            public CategoryStatusEnum CategoryStatus { get; set; } = CategoryStatusEnum.Draft;

            // ─── Section 2: Classification & Grouping ────────────────────────
            [Required(ErrorMessage = "Category Type is required")]
            public CategoryTypeEnum CategoryType { get; set; } = CategoryTypeEnum.OperatingExpense;

            [Required(ErrorMessage = "Reporting Group is required")]
            [MaxLength(100)]
            public string ReportingGroup { get; set; } = string.Empty;

            public Guid? ParentExpenseCategoryId { get; set; }

            public int? HierarchyLevel { get; set; }

            [Required(ErrorMessage = "Category Nature is required")]
            public CategoryNatureEnum CategoryNature { get; set; } = CategoryNatureEnum.Recurring;

            [Required(ErrorMessage = "Usage Scope is required")]
            public UsageScopeEnum UsageScope { get; set; } = UsageScopeEnum.General;

            // ─── Section 3: Accounting Defaults ──────────────────────────────
            [Required(ErrorMessage = "Default GL Account is required")]
            public Guid DefaultGLAccountId { get; set; }
            public string? DefaultGLAccountName { get; set; }

            public Guid? AlternateGLAccountId { get; set; }
            public string? AlternateGLAccountName { get; set; }

            public Guid? AccrualLiabilityGLId { get; set; }
            public string? AccrualLiabilityGLName { get; set; }

            public Guid? PrepaymentAssetGLId { get; set; }
            public string? PrepaymentAssetGLName { get; set; }

            public Guid? TaxDefaultCodeId { get; set; }
            public string? TaxDefaultCodeName { get; set; }

            public bool IsTaxApplicable { get; set; }

            [Required(ErrorMessage = "Default Currency Behavior is required")]
            public DefaultCurrencyBehaviorEnum DefaultCurrencyBehavior { get; set; } = DefaultCurrencyBehaviorEnum.CompanyBaseCurrency;

            // ─── Section 4: Policy Controls ──────────────────────────────────
            public bool IsReimbursable { get; set; }
            public bool ReceiptRequiredFlag { get; set; }

            [Range(0, double.MaxValue, ErrorMessage = "Receipt Threshold must be >= 0")]
            public decimal? ReceiptThresholdAmount { get; set; }

            public bool ApprovalRequiredFlag { get; set; }
            public bool FinanceReviewRequiredFlag { get; set; }
            public bool DuplicateCheckRequiredFlag { get; set; } = true;
            public bool EmployeeClaimAllowedFlag { get; set; }
            public bool SupplierExpenseAllowedFlag { get; set; } = true;
            public bool CashExpenseAllowedFlag { get; set; } = true;
            public bool CompanyCardAllowedFlag { get; set; } = true;
            public bool BlockedForDirectPostingFlag { get; set; }

            // ─── Section 5: Budget & Timing Rules ────────────────────────────
            public bool BudgetControlApplicableFlag { get; set; }

            [Required(ErrorMessage = "Default Budget Control Mode is required")]
            public DefaultBudgetControlModeEnum DefaultBudgetControlMode { get; set; } = DefaultBudgetControlModeEnum.None;

            public bool AccrualAllowedFlag { get; set; }
            public bool PrepaymentAllowedFlag { get; set; }
            public bool ImmediateExpenseAllowedFlag { get; set; } = true;

            [Required(ErrorMessage = "Default Timing Treatment is required")]
            public DefaultTimingTreatmentEnum DefaultTimingTreatment { get; set; } = DefaultTimingTreatmentEnum.ImmediateExpense;

            public bool CoverageDatesRequiredFlag { get; set; }
            public bool ProjectAllocationRequiredFlag { get; set; }
            public bool CostCenterMandatoryFlag { get; set; }
            public bool DepartmentMandatoryFlag { get; set; }
            public bool BranchMandatoryFlag { get; set; }

            // ─── Section 6: Workflow & Usage Governance ──────────────────────
            public bool CanBeUsedInDraftOnlyFlag { get; set; }

            [Required(ErrorMessage = "Effective From date is required")]
            public DateTime? EffectiveFrom { get; set; }

            public DateTime? EffectiveTo { get; set; }

            public int? UsageCount { get; set; }
            public DateTime? LastUsedOn { get; set; }

            [Required(ErrorMessage = "Prepared By is required")]
            [MaxLength(100)]
            public string PreparedByUserId { get; set; } = string.Empty;

            [MaxLength(100)]
            public string? ReviewedByUserId { get; set; }
            [MaxLength(100)]
            public string? ApprovedByUserId { get; set; }

            public DateTime? PreparedOn { get; set; }
            public DateTime? ReviewedOn { get; set; }
            public DateTime? ApprovedOn { get; set; }

            public bool IsLocked { get; set; }

            // ─── Section 7: Notes & Documentation ────────────────────────────
            [MaxLength(1500)]
            public string? PolicyNotes { get; set; }

            [MaxLength(200)]
            public string? InternalGuidelineReference { get; set; }

            public int AttachmentCount { get; set; }

            [MaxLength(1500)]
            public string? Notes { get; set; }

            // ─── Section 8: System Audit Fields (Hidden) ─────────────────────
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public Guid? CreatedBy { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public Guid? UpdatedBy { get; set; }
            public bool IsDeleted { get; set; }
        }
    }
}
