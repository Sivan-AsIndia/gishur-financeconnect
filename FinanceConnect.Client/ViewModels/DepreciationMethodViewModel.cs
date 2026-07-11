using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public class DepreciationMethodViewModel
    {
        // ── Enums ──────────────────────────────────────────────────────
        public enum MethodTypeEnum
        {
            StraightLine = 1,
            WrittenDownValue = 2,
            DoubleDeclining = 3,
            UnitsOfProduction = 4,
            CustomRate = 5
        }

        public enum TimeBasisEnum
        {
            Monthly = 1,
            Daily = 2,
            Units = 3
        }

        public enum DepreciationBaseEnum
        {
            CostOnly = 1,
            CostMinusResidual = 2,
            OpeningNBV = 3
        }

        public enum ResidualHandlingModeEnum
        {
            ApplyResidualAtEndOnly = 1,
            NeverDepreciateBelowResidual = 2,
            IgnoreResidual = 3
        }

        public enum InputModeEnum
        {
            LifeBased = 1,
            RateBased = 2
        }

        public enum StartConventionEnum
        {
            FromInServiceDate_ProRata = 1,
            NextMonthStart = 2,
            FullMonthIfBeforeDayN = 3
        }

        public enum EndConventionEnum
        {
            StopAtUsefulLifeEnd = 1,
            StopWhenNBVReachesResidual = 2
        }

        public enum RoundingRuleEnum
        {
            RoundToNearest = 1,
            RoundDown = 2,
            RoundUp = 3
        }

        public enum RoundingAtEnum
        {
            PerAssetPerMonth = 1,
            PerRunTotal = 2
        }

        public enum MethodStatusEnum
        {
            Active = 1,
            Inactive = 2,
            Archived = 3
        }

        // ── DepreciationMethod Model ───────────────────────────────────
        public class DepreciationMethod
        {
            // ── Section 1: Core Identity ───────────────────────────────
            public Guid DepreciationMethodId { get; set; }
            public Guid TenantId { get; set; }
            public Guid CompanyId { get; set; }

            [Required(ErrorMessage = "Method Code is required")]
            [StringLength(20)]
            public string MethodCode { get; set; } = "";

            [Required(ErrorMessage = "Method Name is required")]
            [StringLength(150)]
            public string MethodName { get; set; } = "";

            [StringLength(500)]
            public string? Description { get; set; }

            // ── Section 2: Method Type & Time Basis ────────────────────
            [Required(ErrorMessage = "Method Type is required")]
            public MethodTypeEnum? MethodType { get; set; }

            [Required(ErrorMessage = "Time Basis is required")]
            public TimeBasisEnum TimeBasis { get; set; } = TimeBasisEnum.Monthly;

            public bool IsApplicableToIntangibles { get; set; } = true;
            public bool IsApplicableToTangibles { get; set; } = true;
            public bool IsDepreciationAllowedInCWIP { get; set; } = false;

            // ── Section 3: Calculation Base Rules ──────────────────────
            [Required(ErrorMessage = "Depreciation Base is required")]
            public DepreciationBaseEnum? DepreciationBase { get; set; }

            [Required(ErrorMessage = "Residual Handling Mode is required")]
            public ResidualHandlingModeEnum? ResidualHandlingMode { get; set; }

            public bool AllowResidualOverrideAtAsset { get; set; } = true;

            // ── Section 4: Rate / Life Inputs ──────────────────────────
            [Required(ErrorMessage = "Input Mode is required")]
            public InputModeEnum? InputMode { get; set; }

            [Range(0.001, 100, ErrorMessage = "Rate must be between 0 and 100")]
            public decimal? DefaultRatePercent { get; set; }

            public bool AllowRateOverrideAtCategory { get; set; } = true;
            public bool AllowRateOverrideAtAsset { get; set; } = false;

            // ── Section 5: Depreciation Start/End Conventions ──────────
            [Required(ErrorMessage = "Start Convention is required")]
            public StartConventionEnum? StartConvention { get; set; }

            [Range(1, 28, ErrorMessage = "Cutoff Day must be between 1 and 28")]
            public int? FullMonthCutoffDay { get; set; }

            [Required(ErrorMessage = "End Convention is required")]
            public EndConventionEnum? EndConvention { get; set; }

            public bool SkipDepreciationIfDisposedInPeriod { get; set; } = true;
            public bool AllowCatchUpDepreciation { get; set; } = false;

            // ── Section 6: Rounding & Precision ────────────────────────
            [Range(0, 4, ErrorMessage = "Precision must be between 0 and 4")]
            public int RoundingPrecisionDecimals { get; set; } = 2;

            public RoundingRuleEnum RoundingRule { get; set; } = RoundingRuleEnum.RoundToNearest;
            public RoundingAtEnum RoundingAt { get; set; } = RoundingAtEnum.PerAssetPerMonth;

            [Range(0, double.MaxValue)]
            public decimal? MinDepreciationAmount { get; set; }

            // ── Section 7: Governance & Status ─────────────────────────
            [Required(ErrorMessage = "Method Status is required")]
            public MethodStatusEnum MethodStatus { get; set; } = MethodStatusEnum.Active;

            public bool IsLockedForChanges { get; set; } = false;

            [StringLength(300)]
            public string? LockReason { get; set; }

            public int UsedAssetCount { get; set; }
            public bool UsedInPostedRunsFlag { get; set; }

            // ── Section 8: System Audit Fields ─────────────────────────
            public DateTime CreatedAt { get; set; }
            public Guid CreatedBy { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public Guid? UpdatedBy { get; set; }
            public byte[]? RowVersion { get; set; }
            public bool IsDeleted { get; set; } = false;
        }
    }
}
