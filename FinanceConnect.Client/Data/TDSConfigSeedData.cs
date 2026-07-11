using FinanceConnect.Client.ViewModels;
using static FinanceConnect.Client.ViewModels.TdsConfigViewModel;

namespace FinanceConnect.Client.Data
{
    public class TDSConfigSeedData
    {
        private static readonly Guid ID1 = Guid.Parse("DC000000-0000-0000-0000-000000000001");
        private static readonly Guid ID2 = Guid.Parse("DC000000-0000-0000-0000-000000000002");
        private static readonly Guid ID3 = Guid.Parse("DC000000-0000-0000-0000-000000000003");
        private static readonly Guid ID4 = Guid.Parse("DC000000-0000-0000-0000-000000000004");
        private static readonly Guid ID5 = Guid.Parse("DC000000-0000-0000-0000-000000000005");

        private static readonly Guid GL_TDS_PAYABLE = Guid.Parse("BB100000-0000-0000-0000-000000000001");

        private static readonly Guid TC_194C = Guid.Parse("cc000006-0000-0000-0000-000000000006");
        private static readonly Guid TC_194J = Guid.Parse("cc000007-0000-0000-0000-000000000007");
        private static readonly Guid TC_194H = Guid.Parse("cc000008-0000-0000-0000-000000000008");
        private static readonly Guid TC_194I = Guid.Parse("cc000009-0000-0000-0000-000000000009");
        private static readonly Guid TC_194A = Guid.Parse("cc000010-0000-0000-0000-000000000010");

        public List<TDSConfigListDto> Store { get; } = new()
        {
         
            new TDSConfigListDto
            {
                TDSConfigId              = ID1,
                ConfigCode               = "TDS-194C-CONT",
                ConfigName               = "TDS on Contractor Payments – Section 194C",
                SectionCode              = "194C",
                ConfigStatus             = ConfigStatus.Active,
                Priority                 = 10,
                EffectiveFrom            = new DateTime(2024, 4, 1),
                EffectiveTo              = null,
                PartyApplicability       = PartyApplicability.Both,
                APDocumentContext        = APDocumentContext.Both,
                DeductionTriggerBasis    = DeductionTriggerBasis.OnPayment,
                BaseAmountMode           = BaseAmountMode.TaxableAmountExcludingGST,
                ThresholdEvaluationMode  = ThresholdEvaluationMode.CumulativeByVendorInFinancialYear,
                ThresholdAmount          = 100000m,
                ThresholdResetBasis      = ThresholdResetBasis.FinancialYear,
                RateSourceMode           = TDSRateSourceMode.FromTaxRateVersion,
                DefaultRatePercent       = 2.0m,
                RequirePANForStandardRate = true,
                AlternateRateIfPANMissing = 20.0m,
                PanValidationMode        = PanValidationMode.PresenceAndFormat,
                TDSPayableGLAccountDisplay = "TDS Payable – 194C",
                SettlementCategoryTag    = "TDS_194C_MONTHLY",
                ReturnReportingTag       = "26Q_194C",
                IsLockedForChanges       = false,
                CreatedAt                = new DateTime(2024, 4, 1),
            },

            new TDSConfigListDto
            {
                TDSConfigId              = ID2,
                ConfigCode               = "TDS-194J-PROF",
                ConfigName               = "TDS on Professional Fees – Section 194J",
                SectionCode              = "194J",
                ConfigStatus             = ConfigStatus.Active,
                Priority                 = 10,
                EffectiveFrom            = new DateTime(2024, 4, 1),
                EffectiveTo              = null,
                PartyApplicability       = PartyApplicability.ResidentOnly,
                APDocumentContext        = APDocumentContext.VendorBill,
                DeductionTriggerBasis    = DeductionTriggerBasis.OnInvoiceBooking,
                BaseAmountMode           = BaseAmountMode.TaxableAmountExcludingGST,
                ThresholdEvaluationMode  = ThresholdEvaluationMode.PerTransaction,
                ThresholdAmount          = 30000m,
                ThresholdResetBasis      = null,
                RateSourceMode           = TDSRateSourceMode.FromTaxRateVersion,
                DefaultRatePercent       = 10.0m,
                RequirePANForStandardRate = true,
                AlternateRateIfPANMissing = 20.0m,
                PanValidationMode        = PanValidationMode.PresenceOnly,
                TDSPayableGLAccountDisplay = "TDS Payable – 194J",
                SettlementCategoryTag    = "TDS_194J_MONTHLY",
                ReturnReportingTag       = "26Q_194J",
                IsLockedForChanges       = false,
                CreatedAt                = new DateTime(2024, 4, 1),
            },

      
            new TDSConfigListDto
            {
                TDSConfigId              = ID3,
                ConfigCode               = "TDS-194H-COMM",
                ConfigName               = "TDS on Commission Payments – Section 194H",
                SectionCode              = "194H",
                ConfigStatus             = ConfigStatus.Inactive,
                Priority                 = 20,
                EffectiveFrom            = new DateTime(2023, 4, 1),
                EffectiveTo              = new DateTime(2024, 3, 31),
                PartyApplicability       = PartyApplicability.Both,
                APDocumentContext        = APDocumentContext.Both,
                DeductionTriggerBasis    = DeductionTriggerBasis.OnPayment,
                BaseAmountMode           = BaseAmountMode.GrossBillAmount,
                ThresholdEvaluationMode  = ThresholdEvaluationMode.PerTransaction,
                ThresholdAmount          = 15000m,
                ThresholdResetBasis      = null,
                RateSourceMode           = TDSRateSourceMode.FixedRateOnConfig,
                DefaultRatePercent       = 5.0m,
                RequirePANForStandardRate = true,
                AlternateRateIfPANMissing = 20.0m,
                PanValidationMode        = PanValidationMode.PresenceOnly,
                TDSPayableGLAccountDisplay = "TDS Payable – 194H",
                SettlementCategoryTag    = "TDS_194H_QTRLY",
                ReturnReportingTag       = "26Q_194H",
                IsLockedForChanges       = true,
                LockReason               = "Used in posted transactions — FY 2023-24",
                CreatedAt                = new DateTime(2023, 4, 1),
                UpdatedAt                = new DateTime(2024, 3, 31),
            },

            new TDSConfigListDto
            {
                TDSConfigId              = ID4,
                ConfigCode               = "TDS-194I-RENT",
                ConfigName               = "TDS on Rent – Section 194I",
                SectionCode              = "194I",
                ConfigStatus             = ConfigStatus.Active,
                Priority                 = 10,
                EffectiveFrom            = new DateTime(2024, 4, 1),
                EffectiveTo              = null,
                PartyApplicability       = PartyApplicability.ResidentOnly,
                APDocumentContext        = APDocumentContext.VendorPayment,
                DeductionTriggerBasis    = DeductionTriggerBasis.OnPayment,
                BaseAmountMode           = BaseAmountMode.GrossBillAmount,
                ThresholdEvaluationMode  = ThresholdEvaluationMode.CumulativeByVendorInFinancialYear,
                ThresholdAmount          = 240000m,
                ThresholdResetBasis      = ThresholdResetBasis.FinancialYear,
                RateSourceMode           = TDSRateSourceMode.FromTaxRateVersion,
                DefaultRatePercent       = 10.0m,
                RequirePANForStandardRate = true,
                AlternateRateIfPANMissing = 20.0m,
                PanValidationMode        = PanValidationMode.PresenceAndFormat,
                TDSPayableGLAccountDisplay = "TDS Payable – 194I",
                SettlementCategoryTag    = "TDS_194I_MONTHLY",
                ReturnReportingTag       = "26Q_194I",
                IsLockedForChanges       = false,
                CreatedAt                = new DateTime(2024, 4, 1),
            },

     
            new TDSConfigListDto
            {
                TDSConfigId              = ID5,
                ConfigCode               = "TDS-194A-INT",
                ConfigName               = "TDS on Interest (Other than Securities) – Section 194A",
                SectionCode              = "194A",
                ConfigStatus             = ConfigStatus.Active,
                Priority                 = 15,
                EffectiveFrom            = new DateTime(2024, 4, 1),
                EffectiveTo              = null,
                PartyApplicability       = PartyApplicability.ResidentOnly,
                APDocumentContext        = APDocumentContext.VendorBill,
                DeductionTriggerBasis    = DeductionTriggerBasis.OnInvoiceBooking,
                BaseAmountMode           = BaseAmountMode.TaxableAmountExcludingGST,
                ThresholdEvaluationMode  = ThresholdEvaluationMode.CumulativeByVendorInFinancialYear,
                ThresholdAmount          = 40000m,
                ThresholdResetBasis      = ThresholdResetBasis.FinancialYear,
                RateSourceMode           = TDSRateSourceMode.FromTaxRateVersion,
                DefaultRatePercent       = 10.0m,
                RequirePANForStandardRate = true,
                AlternateRateIfPANMissing = 20.0m,
                PanValidationMode        = PanValidationMode.PresenceOnly,
                TDSPayableGLAccountDisplay = "TDS Payable – 194A",
                SettlementCategoryTag    = "TDS_194A_MONTHLY",
                ReturnReportingTag       = "26Q_194A",
                IsLockedForChanges       = false,
                CreatedAt                = new DateTime(2024, 4, 1),
            },
        };

        public Dictionary<Guid, TDSConfigFormDto> FormStore { get; } = new();

        public TDSConfigSeedData()
        {
            foreach (var item in Store)
            {
                FormStore[item.TDSConfigId] = new TDSConfigFormDto
                {
                    TDSConfigId = item.TDSConfigId,
                    ConfigCode = item.ConfigCode,
                    ConfigName = item.ConfigName,
                    SectionCode = item.SectionCode,
                    ConfigStatus = item.ConfigStatus,
                    Priority = item.Priority,
                    EffectiveFrom = item.EffectiveFrom,
                    EffectiveTo = item.EffectiveTo,
                    PartyApplicability = item.PartyApplicability,
                    APDocumentContext = item.APDocumentContext,
                    DeductionTriggerBasis = item.DeductionTriggerBasis,
                    BaseAmountMode = item.BaseAmountMode,
                    ThresholdEvaluationMode = item.ThresholdEvaluationMode,
                    ThresholdAmount = item.ThresholdAmount,
                    ThresholdResetBasis = item.ThresholdResetBasis,
                    RateSourceMode = item.RateSourceMode,
                    DefaultRatePercent = item.DefaultRatePercent,
                    RequirePANForStandardRate = item.RequirePANForStandardRate,
                    AlternateRateIfPANMissing = item.AlternateRateIfPANMissing,
                    PanValidationMode = item.PanValidationMode,
                    TDSPayableGLAccountDisplay = item.TDSPayableGLAccountDisplay,
                    SettlementCategoryTag = item.SettlementCategoryTag,
                    ReturnReportingTag = item.ReturnReportingTag,
                    IsLockedForChanges = item.IsLockedForChanges,
                    LockReason = item.LockReason,
                    CreatedAt = item.CreatedAt,
                    UpdatedAt = item.UpdatedAt,
                };
            }
        }
    }
}
