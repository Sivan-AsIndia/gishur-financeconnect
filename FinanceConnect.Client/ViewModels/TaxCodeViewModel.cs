using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public class TaxCodeViewModel
    {
        public enum TaxType
        {
            GST = 1,
            TDS = 2,
            TCS = 3,
            Other = 4
        }

        public enum GSTComponentType
        {
            CGST = 1,
            SGST = 2,
            IGST = 3,
            CESS = 4
        }

        public enum TaxDirection
        {
            Input = 1,
            Output = 2,
            WithholdingPayable = 3,
            Other = 4
        }

        public enum CalculationType
        {
            Percentage = 1,
            FixedAmount = 2,
            SlabBased = 3
        }

        public enum RateBasis
        {
            OnTaxableValue = 1,
            OnGrossValue = 2,
            OnNetValue = 3
        }

        public enum RoundingRule
        {
            RoundToNearest = 1,
            RoundDown = 2,
            RoundUp = 3
        }

        public enum GSTReturnTag
        {
            OutwardTax = 1,
            InwardITC = 2,
            RCM_Liability = 3,
            RCM_ITC = 4,
            Exempt = 5,
            NilRated = 6,
            NonGST = 7
        }

        public enum TaxCodeStatus
        {
            Active = 1,
            Inactive = 2,
            Archived = 3
        }

        public enum EffectiveFromPolicy
        {
            RateVersionControlsOnly = 1,
            TaxCodeAndRateVersion = 2
        }

        public class TaxCode
        {
            public Guid TaxCodeId { get; set; }

            [Required(ErrorMessage = "Tax Code is required")]
            [StringLength(30, ErrorMessage = "Tax Code cannot exceed 30 characters")]
            public string? Code { get; set; }

            [Required(ErrorMessage = "Tax Name is required")]
            [StringLength(150, ErrorMessage = "Tax Name cannot exceed 150 characters")]
            public string? TaxName { get; set; }

            [StringLength(500)]
            public string? Description { get; set; }

            [Required(ErrorMessage = "Tax Type is required")]
            public TaxType? Type { get; set; }

            public string JurisdictionCountryCode { get; set; } = "IN";

            public GSTComponentType? GSTComponent { get; set; }

            [Required(ErrorMessage = "Tax Direction is required")]
            public TaxDirection? Direction { get; set; }

            public bool IsReverseChargeApplicable { get; set; } = false;
            public bool IsITCEligibleDefault { get; set; } = true;

            [Required(ErrorMessage = "Calculation Type is required")]
            public CalculationType CalcType { get; set; } = CalculationType.Percentage;

            public RateBasis Basis { get; set; } = RateBasis.OnTaxableValue;

            [Range(0, 4, ErrorMessage = "Rounding Precision must be 0–4")]
            public int RoundingPrecisionDecimals { get; set; } = 2;

            public RoundingRule Rounding { get; set; } = RoundingRule.RoundToNearest;

            public decimal? MinTaxAmount { get; set; }
            public decimal? MaxTaxAmount { get; set; }

            public Guid? InputTaxGLAccountId { get; set; }
            public string? InputTaxGLAccountName { get; set; }

            public Guid? OutputTaxGLAccountId { get; set; }
            public string? OutputTaxGLAccountName { get; set; }

            public Guid? TDSGLAccountId { get; set; }
            public string? TDSGLAccountName { get; set; }

            public Guid? TCSGLAccountId { get; set; }
            public string? TCSGLAccountName { get; set; }

            public Guid? RCMOutputTaxGLAccountId { get; set; }
            public string? RCMOutputTaxGLAccountName { get; set; }

            public Guid? RCMInputTaxGLAccountId { get; set; }
            public string? RCMInputTaxGLAccountName { get; set; }

            public bool IsGLOverrideAllowedByMapping { get; set; } = true;

            public GSTReturnTag? ReturnTag { get; set; }

            [StringLength(20)]
            public string? TDSSectionCode { get; set; }

            [StringLength(20)]
            public string? TCSSectionCode { get; set; }

            [StringLength(50)]
            public string? StatutoryReportingGroup { get; set; }

            public TaxCodeStatus Status { get; set; } = TaxCodeStatus.Active;
            public bool IsLockedForChanges { get; set; } = false;

            [StringLength(300)]
            public string? LockReason { get; set; }

            public EffectiveFromPolicy EffectivePolicy { get; set; } = EffectiveFromPolicy.RateVersionControlsOnly;

            public Guid? CompanyId { get; set; }
            public Guid? TenantId { get; set; }
            public Guid? UpdatedBy { get; set; }
        }

        public class TaxCodeListDto
        {
            public Guid TaxCodeId { get; set; }
            public string? Code { get; set; }
            public string? TaxName { get; set; }
            public string? Description { get; set; }

            public TaxType? Type { get; set; }
            public string JurisdictionCountryCode { get; set; } = "IN";
            public GSTComponentType? GSTComponent { get; set; }
            public TaxDirection? Direction { get; set; }
            public bool IsReverseChargeApplicable { get; set; }
            public bool IsITCEligibleDefault { get; set; }

            public CalculationType CalcType { get; set; }
            public RateBasis Basis { get; set; }
            public int RoundingPrecisionDecimals { get; set; }
            public RoundingRule Rounding { get; set; }
            public decimal? MinTaxAmount { get; set; }
            public decimal? MaxTaxAmount { get; set; }

            public Guid? InputTaxGLAccountId { get; set; }
            public string? InputTaxGLAccountName { get; set; }
            public Guid? OutputTaxGLAccountId { get; set; }
            public string? OutputTaxGLAccountName { get; set; }
            public Guid? TDSGLAccountId { get; set; }
            public string? TDSGLAccountName { get; set; }
            public Guid? TCSGLAccountId { get; set; }
            public string? TCSGLAccountName { get; set; }
            public Guid? RCMOutputTaxGLAccountId { get; set; }
            public Guid? RCMInputTaxGLAccountId { get; set; }
            public bool IsGLOverrideAllowedByMapping { get; set; }

            public GSTReturnTag? ReturnTag { get; set; }
            public string? TDSSectionCode { get; set; }
            public string? TCSSectionCode { get; set; }
            public string? StatutoryReportingGroup { get; set; }

            public TaxCodeStatus Status { get; set; }
            public bool IsLockedForChanges { get; set; }
            public string? LockReason { get; set; }
            public EffectiveFromPolicy EffectivePolicy { get; set; }

            public Guid? CompanyId { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public bool IsDeleted { get; set; }
        }

        public class SelectItem
        {
            public string Value { get; set; } = "";
            public string Text { get; set; } = "";
        }
    }
}
