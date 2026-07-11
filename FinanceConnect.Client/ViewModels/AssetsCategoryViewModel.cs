using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public class AssetsCategoryViewModel
    {

        public enum AssetType
        {
            Tangible = 1,
            Intangible = 2,
            LeaseAsset = 3,
            Other = 4
        }

        public enum CategoryStatus
        {
            Active = 1,
            Inactive = 2,
            Archived = 3
        }

        public enum DefaultAssetStatusOnCreation
        {
            Draft = 1,
            UnderConstruction = 2,
            InService = 3
        }

        public enum DepreciationStartConvention
        {
            FromInServiceDate = 1,
            NextMonthStart = 2,
            FullMonthIfInServiceBeforeDayN = 3
        }

        public enum DepreciationRoundingRule
        {
            RoundToNearest = 1,
            RoundDown = 2,
            RoundUp = 3
        }
        public class AssetCategory
        {
            // ── Section 1: Core Identity ──────────────────────────────────────
            public Guid AssetCategoryId { get; set; }
            [Required]
            public Guid TenantId { get; set; }
            [Required]
            public Guid CompanyId { get; set; }
            public Guid BranchId { get; set; }
            [Required]
            [StringLength(30)]
            public string CategoryCode { get; set; } = "";
            [Required]
            [StringLength(150)]
            public string CategoryName { get; set; } = "";
            [StringLength(500)]
            public string? Description { get; set; }
            public Guid? ParentAssetCategoryId { get; set; }

            // ── Section 2: Category Classification ───────────────────────────
            [Required(ErrorMessage = "Asset Type is required")]
            public AssetType? AssetType { get; set; }
            public bool IsCapitalizable { get; set; } = true;
            public DefaultAssetStatusOnCreation DefaultAssetStatusOnCreation { get; set; } = DefaultAssetStatusOnCreation.Draft;

            // ── Section 3: Capitalization Policy ─────────────────────────────
            [Required(ErrorMessage = "Capitalization Threshold is required")]
            [Range(0, double.MaxValue)]
            public decimal CapitalizationThresholdAmount { get; set; }
            public Guid? ExpenseAccountIdForBelowThreshold { get; set; }
            public bool AllowManualOverrideThreshold { get; set; } = false;
            public bool RequireAcquisitionApproval { get; set; } = true;

            // ── Section 4: Depreciation Defaults (Financial Book) ─────────────
            public bool IsDepreciable { get; set; } = true;
            public Guid? DefaultDepreciationMethodId { get; set; }
            [Range(1, int.MaxValue)]
            [Required(ErrorMessage = "Useful Life is required")]
            public int? UsefulLifeMonths { get; set; }
            [Range(0, 100)]
            [Required(ErrorMessage = "Residual Value is required")]
            public decimal? ResidualValuePercent { get; set; }
            [Required(ErrorMessage = "Start Convention is required")]
            public DepreciationStartConvention? DepreciationStartConvention { get; set; }
            public DepreciationRoundingRule DepreciationRoundingRule { get; set; } = DepreciationRoundingRule.RoundToNearest;
            public bool AllowDepreciationOnNonWorkingDays { get; set; } = true;

            // ── Section 5: Accounting / GL Mapping ───────────────────────────
            [Required(ErrorMessage = "Asset Cost Account is required")]
            public Guid? AssetCostGLAccountId { get; set; }
            [Required(ErrorMessage = "Accumulated Depreciation Account is required")]
            public Guid? AccumulatedDepreciationGLAccountId { get; set; }
            [Required(ErrorMessage = "Depreciation Expense Account is required")]
            public Guid? DepreciationExpenseGLAccountId { get; set; }
            public Guid? CapitalizationClearingGLAccountId { get; set; }
            public Guid? CWIPGLAccountId { get; set; }
            [Required(ErrorMessage = "Gain on Disposal Account is required")]
            public Guid? DisposalGainGLAccountId { get; set; }
            [Required(ErrorMessage = "Loss on Disposal Account is required")]
            public Guid? DisposalLossGLAccountId { get; set; }
            public Guid? ImpairmentLossGLAccountId { get; set; }
            public Guid? RevaluationReserveGLAccountId { get; set; }

            // ── Section 6: Controls ───────────────────────────────────────────
            public bool RequiresAssetTag { get; set; } = true;
            public bool RequiresSerialNumber { get; set; } = false;
            public bool RequiresCustodian { get; set; } = true;
            public bool RequiresLocation { get; set; } = true;
            public bool AllowSplitIntoComponents { get; set; } = false;
            public Guid? DefaultCostCenterId { get; set; }

            // ── Section 7: Status & Lifecycle ────────────────────────────────
            public CategoryStatus CategoryStatus { get; set; } = CategoryStatus.Active;
            public bool IsLockedForChanges { get; set; } = false;
            [StringLength(300)]
            public string? LockReason { get; set; }
            public DateTime CreatedAt { get; set; }
            public Guid CreatedBy { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public Guid? UpdatedBy { get; set; }
            public byte[]? RowVersion { get; set; }
            public bool IsDeleted { get; set; } = false;
        }
    }
}
