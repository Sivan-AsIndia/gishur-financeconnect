using FinanceConnect.Client.ViewModels;
using static FinanceConnect.Client.ViewModels.TaxRateVersionViewModel;

namespace FinanceConnect.Client.Data
{
    public static class TaxRateVersionSeedData
    {
        private static readonly Guid Co1 = Guid.Parse("cccccccc-0000-0000-0000-000000000001");

        private static readonly Guid TC_GST_CGST = Guid.Parse("cc000001-0000-0000-0000-000000000001");
        private static readonly Guid TC_GST_SGST = Guid.Parse("cc000002-0000-0000-0000-000000000002");
        private static readonly Guid TC_GST_IGST = Guid.Parse("cc000003-0000-0000-0000-000000000003");
        private static readonly Guid TC_GST_CGST_IN = Guid.Parse("cc000004-0000-0000-0000-000000000004");
        private static readonly Guid TC_GST_CESS = Guid.Parse("cc000005-0000-0000-0000-000000000005");
        private static readonly Guid TC_TDS_194C = Guid.Parse("cc000006-0000-0000-0000-000000000006");
        private static readonly Guid TC_TDS_194J = Guid.Parse("cc000007-0000-0000-0000-000000000007");
        private static readonly Guid TC_TCS_206C = Guid.Parse("cc000008-0000-0000-0000-000000000008");

  
        private static readonly Guid RV01 = Guid.Parse("aa000001-0000-0000-0000-000000000001");
        private static readonly Guid RV02 = Guid.Parse("aa000002-0000-0000-0000-000000000002");
        private static readonly Guid RV03 = Guid.Parse("aa000003-0000-0000-0000-000000000003");
        private static readonly Guid RV04 = Guid.Parse("aa000004-0000-0000-0000-000000000004");
        private static readonly Guid RV05 = Guid.Parse("aa000005-0000-0000-0000-000000000005");
        private static readonly Guid RV06 = Guid.Parse("aa000006-0000-0000-0000-000000000006");
        private static readonly Guid RV07 = Guid.Parse("aa000007-0000-0000-0000-000000000007");
        private static readonly Guid RV08 = Guid.Parse("aa000008-0000-0000-0000-000000000008");
        private static readonly Guid RV09 = Guid.Parse("aa000009-0000-0000-0000-000000000009");
        private static readonly Guid RV10 = Guid.Parse("aa000010-0000-0000-0000-000000000010");
        private static readonly Guid RV11 = Guid.Parse("aa000011-0000-0000-0000-000000000011");
        private static readonly Guid RV12 = Guid.Parse("aa000012-0000-0000-0000-000000000012");

        public static List<TaxRateVersionListDto> GetAllRateVersions()
        {
            return new List<TaxRateVersionListDto>
            {
                new TaxRateVersionListDto
                {
                    TaxRateVersionId      = RV01,
                    TaxCodeId             = TC_GST_CGST,
                    TaxCodeSnapshot       = "GST_CGST",
                    TaxCodeNameSnapshot   = "CGST – Central GST",
                    TaxTypeSnapshot       = "GST",
                    VersionNumber         = 1,
                    Status                = VersionStatus.Retired,
                    EffectiveFrom         = new DateTime(2017, 7, 1),
                    EffectiveTo           = new DateTime(2018, 12, 31),
                    Type                  = RateType.Percentage,
                    RatePercent           = 6m,
                    Basis                 = RateBasis.OnTaxableValue,
                    SourceType            = RateSourceType.GovernmentNotification,
                    LegalReferenceNumber  = "CBIC Notif 2017-01",
                    LegalReferenceDate    = new DateTime(2017, 6, 28),
                    Notes                 = "Original GST launch rate – 12% GST split as CGST 6% + SGST 6%",
                    IsLockedForChanges    = true,
                    LockReason            = "Used in posted transactions",
                    ITCOverride           = ITCRateOverride.InheritFromTaxCode,
                    CompanyId             = Co1,
                    CreatedAt             = new DateTime(2017, 6, 30),
                },
                new TaxRateVersionListDto
                {
                    TaxRateVersionId      = RV02,
                    TaxCodeId             = TC_GST_CGST,
                    TaxCodeSnapshot       = "GST_CGST",
                    TaxCodeNameSnapshot   = "CGST – Central GST",
                    TaxTypeSnapshot       = "GST",
                    VersionNumber         = 2,
                    Status                = VersionStatus.Active,
                    EffectiveFrom         = new DateTime(2019, 1, 1),
                    EffectiveTo           = null,
                    Type                  = RateType.Percentage,
                    RatePercent           = 9m,
                    Basis                 = RateBasis.OnTaxableValue,
                    SourceType            = RateSourceType.GovernmentNotification,
                    LegalReferenceNumber  = "CBIC Notif 2018-55",
                    LegalReferenceDate    = new DateTime(2018, 12, 15),
                    Notes                 = "Rate revised to 18% GST standard – CGST 9% + SGST 9%",
                    IsLockedForChanges    = true,
                    LockReason            = "Active version – edits via new version only",
                    SupersedesVersionId   = RV01,
                    ITCOverride           = ITCRateOverride.InheritFromTaxCode,
                    CompanyId             = Co1,
                    CreatedAt             = new DateTime(2018, 12, 20),
                    ActivatedOn           = new DateTime(2019, 1, 1),
                    ApprovedOn            = new DateTime(2018, 12, 28),
                },

                new TaxRateVersionListDto
                {
                    TaxRateVersionId      = RV03,
                    TaxCodeId             = TC_GST_SGST,
                    TaxCodeSnapshot       = "GST_SGST",
                    TaxCodeNameSnapshot   = "SGST – State GST",
                    TaxTypeSnapshot       = "GST",
                    VersionNumber         = 1,
                    Status                = VersionStatus.Retired,
                    EffectiveFrom         = new DateTime(2017, 7, 1),
                    EffectiveTo           = new DateTime(2018, 12, 31),
                    Type                  = RateType.Percentage,
                    RatePercent           = 6m,
                    Basis                 = RateBasis.OnTaxableValue,
                    SourceType            = RateSourceType.GovernmentNotification,
                    LegalReferenceNumber  = "CBIC Notif 2017-01",
                    IsLockedForChanges    = true,
                    LockReason            = "Used in posted transactions",
                    ITCOverride           = ITCRateOverride.InheritFromTaxCode,
                    CompanyId             = Co1,
                    CreatedAt             = new DateTime(2017, 6, 30),
                },
                new TaxRateVersionListDto
                {
                    TaxRateVersionId      = RV04,
                    TaxCodeId             = TC_GST_SGST,
                    TaxCodeSnapshot       = "GST_SGST",
                    TaxCodeNameSnapshot   = "SGST – State GST",
                    TaxTypeSnapshot       = "GST",
                    VersionNumber         = 2,
                    Status                = VersionStatus.Active,
                    EffectiveFrom         = new DateTime(2019, 1, 1),
                    EffectiveTo           = null,
                    Type                  = RateType.Percentage,
                    RatePercent           = 9m,
                    Basis                 = RateBasis.OnTaxableValue,
                    SourceType            = RateSourceType.GovernmentNotification,
                    LegalReferenceNumber  = "CBIC Notif 2018-55",
                    IsLockedForChanges    = true,
                    LockReason            = "Active version",
                    SupersedesVersionId   = RV03,
                    ITCOverride           = ITCRateOverride.InheritFromTaxCode,
                    CompanyId             = Co1,
                    CreatedAt             = new DateTime(2018, 12, 20),
                    ActivatedOn           = new DateTime(2019, 1, 1),
                    ApprovedOn            = new DateTime(2018, 12, 28),
                },

                new TaxRateVersionListDto
                {
                    TaxRateVersionId      = RV05,
                    TaxCodeId             = TC_GST_IGST,
                    TaxCodeSnapshot       = "GST_IGST",
                    TaxCodeNameSnapshot   = "IGST – Integrated GST",
                    TaxTypeSnapshot       = "GST",
                    VersionNumber         = 1,
                    Status                = VersionStatus.Active,
                    EffectiveFrom         = new DateTime(2019, 1, 1),
                    EffectiveTo           = null,
                    Type                  = RateType.Percentage,
                    RatePercent           = 18m,
                    Basis                 = RateBasis.OnTaxableValue,
                    SourceType            = RateSourceType.GovernmentNotification,
                    LegalReferenceNumber  = "CBIC Notif 2018-55",
                    IsLockedForChanges    = true,
                    LockReason            = "Active version – edits via new version only",
                    ITCOverride           = ITCRateOverride.InheritFromTaxCode,
                    IsReverseChargeRate   = false,
                    CompanyId             = Co1,
                    CreatedAt             = new DateTime(2018, 12, 20),
                    ActivatedOn           = new DateTime(2019, 1, 1),
                    ApprovedOn            = new DateTime(2018, 12, 28),
                },
                new TaxRateVersionListDto
                {
                    TaxRateVersionId      = RV06,
                    TaxCodeId             = TC_GST_IGST,
                    TaxCodeSnapshot       = "GST_IGST",
                    TaxCodeNameSnapshot   = "IGST – Integrated GST",
                    TaxTypeSnapshot       = "GST",
                    VersionNumber         = 2,
                    Status                = VersionStatus.Draft,
                    EffectiveFrom         = new DateTime(2025, 4, 1),
                    EffectiveTo           = null,
                    Type                  = RateType.Percentage,
                    RatePercent           = 12m,
                    Basis                 = RateBasis.OnTaxableValue,
                    SourceType            = RateSourceType.GovernmentNotification,
                    Notes                 = "Proposed rate reduction effective April 2025 – pending approval",
                    IsLockedForChanges    = false,
                    ITCOverride           = ITCRateOverride.InheritFromTaxCode,
                    CompanyId             = Co1,
                    CreatedAt             = new DateTime(2025, 1, 10),
                },

                new TaxRateVersionListDto
                {
                    TaxRateVersionId      = RV07,
                    TaxCodeId             = TC_GST_CGST_IN,
                    TaxCodeSnapshot       = "GST_CGST_IN",
                    TaxCodeNameSnapshot   = "CGST – Input (ITC)",
                    TaxTypeSnapshot       = "GST",
                    VersionNumber         = 1,
                    Status                = VersionStatus.Active,
                    EffectiveFrom         = new DateTime(2019, 1, 1),
                    EffectiveTo           = null,
                    Type                  = RateType.Percentage,
                    RatePercent           = 9m,
                    Basis                 = RateBasis.OnTaxableValue,
                    SourceType            = RateSourceType.GovernmentNotification,
                    LegalReferenceNumber  = "CBIC Notif 2018-55",
                    IsLockedForChanges    = true,
                    LockReason            = "Active version",
                    ITCOverride           = ITCRateOverride.Eligible,
                    CompanyId             = Co1,
                    CreatedAt             = new DateTime(2018, 12, 20),
                    ActivatedOn           = new DateTime(2019, 1, 1),
                    ApprovedOn            = new DateTime(2018, 12, 28),
                },

                new TaxRateVersionListDto
                {
                    TaxRateVersionId      = RV08,
                    TaxCodeId             = TC_TDS_194C,
                    TaxCodeSnapshot       = "TDS_194C",
                    TaxCodeNameSnapshot   = "TDS – Section 194C (Contractors)",
                    TaxTypeSnapshot       = "TDS",
                    VersionNumber         = 1,
                    Status                = VersionStatus.Retired,
                    EffectiveFrom         = new DateTime(2017, 4, 1),
                    EffectiveTo           = new DateTime(2023, 3, 31),
                    Type                  = RateType.Percentage,
                    RatePercent           = 1m,
                    Basis                 = RateBasis.OnGrossValue,
                    SourceType            = RateSourceType.GovernmentNotification,
                    LegalReferenceNumber  = "IT Act Sec 194C",
                    Notes                 = "Individual/HUF contractors rate",
                    PanRequiredForStandardRate       = true,
                    AlternateRatePercentIfPanMissing = 20m,
                    IsLockedForChanges    = true,
                    LockReason            = "Used in posted transactions",
                    ITCOverride           = ITCRateOverride.InheritFromTaxCode,
                    CompanyId             = Co1,
                    CreatedAt             = new DateTime(2017, 3, 28),
                },
                new TaxRateVersionListDto
                {
                    TaxRateVersionId      = RV09,
                    TaxCodeId             = TC_TDS_194C,
                    TaxCodeSnapshot       = "TDS_194C",
                    TaxCodeNameSnapshot   = "TDS – Section 194C (Contractors)",
                    TaxTypeSnapshot       = "TDS",
                    VersionNumber         = 2,
                    Status                = VersionStatus.Active,
                    EffectiveFrom         = new DateTime(2023, 4, 1),
                    EffectiveTo           = null,
                    Type                  = RateType.Percentage,
                    RatePercent           = 2m,
                    Basis                 = RateBasis.OnGrossValue,
                    SourceType            = RateSourceType.GovernmentNotification,
                    LegalReferenceNumber  = "IT Act Sec 194C – FY2023 Amendment",
                    Notes                 = "Revised rate for company contractors",
                    PanRequiredForStandardRate       = true,
                    AlternateRatePercentIfPanMissing = 20m,
                    IsLockedForChanges    = true,
                    LockReason            = "Active version",
                    SupersedesVersionId   = RV08,
                    ITCOverride           = ITCRateOverride.InheritFromTaxCode,
                    CompanyId             = Co1,
                    CreatedAt             = new DateTime(2023, 3, 15),
                    ApprovedOn            = new DateTime(2023, 3, 28),
                    ActivatedOn           = new DateTime(2023, 4, 1),
                },

                new TaxRateVersionListDto
                {
                    TaxRateVersionId      = RV10,
                    TaxCodeId             = TC_TDS_194J,
                    TaxCodeSnapshot       = "TDS_194J",
                    TaxCodeNameSnapshot   = "TDS – Section 194J (Professional Fees)",
                    TaxTypeSnapshot       = "TDS",
                    VersionNumber         = 1,
                    Status                = VersionStatus.Active,
                    EffectiveFrom         = new DateTime(2020, 4, 1),
                    EffectiveTo           = null,
                    Type                  = RateType.Percentage,
                    RatePercent           = 10m,
                    Basis                 = RateBasis.OnGrossValue,
                    SourceType            = RateSourceType.GovernmentNotification,
                    LegalReferenceNumber  = "IT Act Sec 194J",
                    Notes                 = "Technical/professional services TDS – 10%",
                    PanRequiredForStandardRate       = true,
                    AlternateRatePercentIfPanMissing = 20m,
                    IsLockedForChanges    = true,
                    LockReason            = "Active – used in posted vendor payments",
                    ITCOverride           = ITCRateOverride.InheritFromTaxCode,
                    CompanyId             = Co1,
                    CreatedAt             = new DateTime(2020, 3, 20),
                    ApprovedOn            = new DateTime(2020, 3, 28),
                    ActivatedOn           = new DateTime(2020, 4, 1),
                },
                new TaxRateVersionListDto
                {
                    TaxRateVersionId      = RV11,
                    TaxCodeId             = TC_TDS_194J,
                    TaxCodeSnapshot       = "TDS_194J",
                    TaxCodeNameSnapshot   = "TDS – Section 194J (Professional Fees)",
                    TaxTypeSnapshot       = "TDS",
                    VersionNumber         = 2,
                    Status                = VersionStatus.Submitted,
                    EffectiveFrom         = new DateTime(2025, 4, 1),
                    EffectiveTo           = null,
                    Type                  = RateType.Percentage,
                    RatePercent           = 7.5m,
                    Basis                 = RateBasis.OnGrossValue,
                    SourceType            = RateSourceType.GovernmentNotification,
                    LegalReferenceNumber  = "Finance Bill 2025 – Sec 194J",
                    Notes                 = "Proposed reduction to 7.5% – pending controller approval",
                    IsLockedForChanges    = false,
                    ITCOverride           = ITCRateOverride.InheritFromTaxCode,
                    CompanyId             = Co1,
                    CreatedAt             = new DateTime(2025, 2, 5),
                },

                new TaxRateVersionListDto
                {
                    TaxRateVersionId      = RV12,
                    TaxCodeId             = TC_TCS_206C,
                    TaxCodeSnapshot       = "TCS_206C",
                    TaxCodeNameSnapshot   = "TCS – Section 206C",
                    TaxTypeSnapshot       = "TCS",
                    VersionNumber         = 1,
                    Status                = VersionStatus.Active,
                    EffectiveFrom         = new DateTime(2020, 10, 1),
                    EffectiveTo           = null,
                    Type                  = RateType.Percentage,
                    RatePercent           = 1m,
                    Basis                 = RateBasis.OnGrossValue,
                    SourceType            = RateSourceType.GovernmentNotification,
                    LegalReferenceNumber  = "IT Act Sec 206C – Finance Act 2020",
                    Notes                 = "TCS on sale of goods above threshold",
                    HasThreshold          = true,
                    ThresholdAmount       = 5000000m,
                    IsLockedForChanges    = false,
                    ITCOverride           = ITCRateOverride.InheritFromTaxCode,
                    CompanyId             = Co1,
                    CreatedAt             = new DateTime(2020, 9, 15),
                    ApprovedOn            = new DateTime(2020, 9, 25),
                    ActivatedOn           = new DateTime(2020, 10, 1),
                },
            };
        }
    }
}
