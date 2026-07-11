using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public class TaxRateVersionViewModel
    {
        public enum VersionStatus
        {
            Draft = 1,
            Submitted = 2,
            Approved = 3,
            Active = 4,
            Retired = 5,
            Superseded = 6,
            Cancelled = 7
        }

        public enum RateType
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

        public enum RateSourceType
        {
            GovernmentNotification = 1,
            InternalPolicy = 2,
            Migration = 3,
            Correction = 4
        }

        // ── Renamed from ITCEligibilityOverride → ITCRateOverride ─────────────
        // Reason: ITCEligibilityOverride is also defined globally in
        // TaxCategoryMappingViewModel.cs (with different values: Inherit/Eligible/
        // Ineligible/Provisional). Keeping both with the same name causes CS0104
        // ambiguous reference. This enum uses InheritFromTaxCode as the default
        // which is specific to the rate version layer.
        public enum ITCRateOverride
        {
            InheritFromTaxCode = 1,
            Eligible = 2,
            Ineligible = 3,
            Provisional = 4
        }

        public class TaxRateVersion
        {
            public Guid TaxRateVersionId { get; set; }

            [Required(ErrorMessage = "Tax Code is required")]
            public Guid TaxCodeId { get; set; }
            public string? TaxCodeSnapshot { get; set; }
            public string? TaxCodeNameSnapshot { get; set; }
            public string? TaxTypeSnapshot { get; set; }

            public int VersionNumber { get; set; }
            public VersionStatus Status { get; set; } = VersionStatus.Draft;

            [Required(ErrorMessage = "Effective From is required")]
            public DateTime EffectiveFrom { get; set; } = DateTime.Today;
            public DateTime? EffectiveTo { get; set; }
            public bool IsOpenEnded => EffectiveTo == null;

            [Required(ErrorMessage = "Rate Type is required")]
            public RateType Type { get; set; } = RateType.Percentage;

            [Range(0, 100, ErrorMessage = "Rate Percent must be 0–100")]
            public decimal? RatePercent { get; set; }
            public decimal? FixedAmount { get; set; }

            public RateBasis Basis { get; set; } = RateBasis.OnTaxableValue;

            public decimal? MinimumTaxAmount { get; set; }
            public decimal? MaximumTaxAmount { get; set; }

            public bool HasThreshold { get; set; } = false;
            public decimal? ThresholdAmount { get; set; }
            public string? SlabDefinitionJson { get; set; }

            public bool PanRequiredForStandardRate { get; set; } = false;
            public decimal? AlternateRatePercentIfPanMissing { get; set; }
            public bool IsReverseChargeRate { get; set; } = false;

            // Uses ITCRateOverride (renamed) — not the global ITCEligibilityOverride
            public ITCRateOverride ITCOverride { get; set; } = ITCRateOverride.InheritFromTaxCode;

            [Required(ErrorMessage = "Rate Source is required")]
            public RateSourceType SourceType { get; set; } = RateSourceType.GovernmentNotification;

            [StringLength(100)]
            public string? LegalReferenceNumber { get; set; }
            public DateTime? LegalReferenceDate { get; set; }

            [StringLength(1000)]
            public string? Notes { get; set; }

            public DateTime? ApprovedOn { get; set; }
            public Guid? ApprovedBy { get; set; }
            public DateTime? ActivatedOn { get; set; }
            public Guid? ActivatedBy { get; set; }

            public bool IsLockedForChanges { get; set; } = false;
            [StringLength(300)]
            public string? LockReason { get; set; }

            public Guid? SupersedesVersionId { get; set; }
            public Guid? CompanyId { get; set; }
            public Guid? TenantId { get; set; }
            public Guid? UpdatedBy { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.Now;
            public DateTime? UpdatedAt { get; set; }
        }

        public class TaxRateVersionListDto
        {
            public Guid TaxRateVersionId { get; set; }
            public Guid TaxCodeId { get; set; }
            public string? TaxCodeSnapshot { get; set; }
            public string? TaxCodeNameSnapshot { get; set; }
            public string? TaxTypeSnapshot { get; set; }
            public int VersionNumber { get; set; }
            public VersionStatus Status { get; set; }

            public DateTime EffectiveFrom { get; set; }
            public DateTime? EffectiveTo { get; set; }
            public bool IsOpenEnded => EffectiveTo == null;

            public RateType Type { get; set; }
            public decimal? RatePercent { get; set; }
            public decimal? FixedAmount { get; set; }
            public RateBasis Basis { get; set; }

            public decimal? MinimumTaxAmount { get; set; }
            public decimal? MaximumTaxAmount { get; set; }
            public bool HasThreshold { get; set; }
            public decimal? ThresholdAmount { get; set; }

            public bool PanRequiredForStandardRate { get; set; }
            public decimal? AlternateRatePercentIfPanMissing { get; set; }
            public bool IsReverseChargeRate { get; set; }

            // Uses ITCRateOverride (renamed) — not the global ITCEligibilityOverride
            public ITCRateOverride ITCOverride { get; set; }

            public RateSourceType SourceType { get; set; }
            public string? LegalReferenceNumber { get; set; }
            public DateTime? LegalReferenceDate { get; set; }
            public string? Notes { get; set; }

            public DateTime? ApprovedOn { get; set; }
            public DateTime? ActivatedOn { get; set; }
            public bool IsLockedForChanges { get; set; }
            public string? LockReason { get; set; }
            public Guid? SupersedesVersionId { get; set; }
            public Guid? CompanyId { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public bool IsDeleted { get; set; }
        }
    }
}
