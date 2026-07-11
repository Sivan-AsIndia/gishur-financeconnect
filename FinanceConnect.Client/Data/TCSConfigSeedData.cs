using FinanceConnect.Client.ViewModels;
using static FinanceConnect.Client.ViewModels.TCSConfigViewModel;

namespace FinanceConnect.Client.Data
{
    public class TCSConfigSeedData
    {
        private static readonly Guid ID1 = Guid.Parse("EC000000-0000-0000-0000-000000000001");
        private static readonly Guid ID2 = Guid.Parse("EC000000-0000-0000-0000-000000000002");
        private static readonly Guid ID3 = Guid.Parse("EC000000-0000-0000-0000-000000000003");
        private static readonly Guid ID4 = Guid.Parse("EC000000-0000-0000-0000-000000000004");
        private static readonly Guid ID5 = Guid.Parse("EC000000-0000-0000-0000-000000000005");

        public List<TCSConfigListDto> Store { get; } = new()
        {
            new TCSConfigListDto
            {
                TCSConfigId              = ID1,
                ConfigCode               = "TCS-206C-GOODS",
                ConfigName               = "TCS on Sale of Goods – Section 206C(1H)",
                SectionCode              = "206C(1H)",
                LinkedTaxCodeDisplay     = "TCS_206C",
                ConfigStatus             = ConfigStatus.Active,
                Priority                 = 10,
                EffectiveFrom            = new DateTime(2020, 10, 1),
                EffectiveTo              = null,
                TransactionContext       = TCSTransactionContext.SalesInvoice,
                CollectionTrigger        = TCSCollectionTrigger.OnInvoiceBooking,
                ResidentialStatusApplicability = TCSResidentialStatus.ResidentOnly,
                ThresholdMode            = TCSThresholdMode.CumulativeFinancialYear,
                ThresholdAmount          = 5000000m,
                ThresholdComputationBase = TCSThresholdComputationBase.TaxableAmountOnly,
                RateResolutionMode       = TCSRateResolutionMode.FromTaxRateVersion,
                FixedRatePercent         = 0.1m,
                PanAvailabilityRule      = TCSPanAvailabilityRule.HigherRateIfPANMissing,
                AlternateRatePercentIfPanMissing = 1.0m,
                DeductionBaseMode        = TCSDeductionBaseMode.TaxableLineAmount,
                TCSPayableGLAccountDisplay = "TCS Payable – 206C",
                ReportingTag             = "27EQ_206C_GOODS",
                IsLockedForChanges       = false,
                CreatedAt                = new DateTime(2020, 9, 20),
                UpdatedAt                = new DateTime(2024, 4, 1),
            },

            new TCSConfigListDto
            {
                TCSConfigId              = ID2,
                ConfigCode               = "TCS-206C-SCRAP",
                ConfigName               = "TCS on Sale of Scrap – Section 206C(1)",
                SectionCode              = "206C(1)",
                LinkedTaxCodeDisplay     = "TCS_206C_SCRAP",
                ConfigStatus             = ConfigStatus.Active,
                Priority                 = 10,
                EffectiveFrom            = new DateTime(2020, 4, 1),
                EffectiveTo              = null,
                TransactionContext       = TCSTransactionContext.SalesInvoice,
                CollectionTrigger        = TCSCollectionTrigger.OnInvoiceBooking,
                ResidentialStatusApplicability = TCSResidentialStatus.All,
                ThresholdMode            = TCSThresholdMode.NoThreshold,
                ThresholdAmount          = null,
                ThresholdComputationBase = TCSThresholdComputationBase.GrossAmountIncludingTax,
                RateResolutionMode       = TCSRateResolutionMode.FixedRateOverride,
                FixedRatePercent         = 1.0m,
                PanAvailabilityRule      = TCSPanAvailabilityRule.HigherRateIfPANMissing,
                AlternateRatePercentIfPanMissing = 5.0m,
                DeductionBaseMode        = TCSDeductionBaseMode.GrossLineAmount,
                TCSPayableGLAccountDisplay = "TCS Payable – 206C Scrap",
                ReportingTag             = "27EQ_206C_SCRAP",
                IsLockedForChanges       = false,
                CreatedAt                = new DateTime(2020, 4, 1),
            },

            new TCSConfigListDto
            {
                TCSConfigId              = ID3,
                ConfigCode               = "TCS-206C-LRS",
                ConfigName               = "TCS on LRS / Overseas Remittance – Section 206C(1G)",
                SectionCode              = "206C(1G)",
                LinkedTaxCodeDisplay     = "TCS_206C_LRS",
                ConfigStatus             = ConfigStatus.Inactive,
                Priority                 = 20,
                EffectiveFrom            = new DateTime(2020, 10, 1),
                EffectiveTo              = new DateTime(2023, 9, 30),
                TransactionContext       = TCSTransactionContext.SalesInvoice,
                CollectionTrigger        = TCSCollectionTrigger.OnReceiptOfPayment,
                ResidentialStatusApplicability = TCSResidentialStatus.ResidentOnly,
                ThresholdMode            = TCSThresholdMode.CumulativeFinancialYear,
                ThresholdAmount          = 700000m,
                ThresholdComputationBase = TCSThresholdComputationBase.TaxableAmountOnly,
                RateResolutionMode       = TCSRateResolutionMode.FixedRateOverride,
                FixedRatePercent         = 5.0m,
                PanAvailabilityRule      = TCSPanAvailabilityRule.HigherRateIfPANMissing,
                AlternateRatePercentIfPanMissing = 10.0m,
                DeductionBaseMode        = TCSDeductionBaseMode.ReceiptAmount,
                TCSPayableGLAccountDisplay = "TCS Payable – 206C LRS",
                ReportingTag             = "27EQ_206C_LRS",
                IsLockedForChanges       = true,
                LockReason               = "Superseded by revised LRS rate w.e.f. Oct 2023",
                CreatedAt                = new DateTime(2020, 10, 1),
                UpdatedAt                = new DateTime(2023, 9, 30),
            },

            new TCSConfigListDto
            {
                TCSConfigId              = ID4,
                ConfigCode               = "TCS-206C-LRS-R2",
                ConfigName               = "TCS on LRS / Overseas Remittance – Revised Oct 2023",
                SectionCode              = "206C(1G)",
                LinkedTaxCodeDisplay     = "TCS_206C_LRS",
                ConfigStatus             = ConfigStatus.Active,
                Priority                 = 10,
                EffectiveFrom            = new DateTime(2023, 10, 1),
                EffectiveTo              = null,
                TransactionContext       = TCSTransactionContext.SalesInvoice,
                CollectionTrigger        = TCSCollectionTrigger.OnReceiptOfPayment,
                ResidentialStatusApplicability = TCSResidentialStatus.ResidentOnly,
                ThresholdMode            = TCSThresholdMode.CumulativeFinancialYear,
                ThresholdAmount          = 700000m,
                ThresholdComputationBase = TCSThresholdComputationBase.TaxableAmountOnly,
                RateResolutionMode       = TCSRateResolutionMode.FixedRateOverride,
                FixedRatePercent         = 20.0m,
                PanAvailabilityRule      = TCSPanAvailabilityRule.UseStandardRateIfPANAvailable,
                AlternateRatePercentIfPanMissing = null,
                DeductionBaseMode        = TCSDeductionBaseMode.ReceiptAmount,
                TCSPayableGLAccountDisplay = "TCS Payable – 206C LRS",
                ReportingTag             = "27EQ_206C_LRS_R2",
                IsLockedForChanges       = false,
                CreatedAt                = new DateTime(2023, 10, 1),
            },

            new TCSConfigListDto
            {
                TCSConfigId              = ID5,
                ConfigCode               = "TCS-206C-TENDU",
                ConfigName               = "TCS on Tendu Leaves – Section 206C(1)(c)",
                SectionCode              = "206C(1)(c)",
                LinkedTaxCodeDisplay     = "TCS_206C_TENDU",
                ConfigStatus             = ConfigStatus.Active,
                Priority                 = 15,
                EffectiveFrom            = new DateTime(2024, 4, 1),
                EffectiveTo              = null,
                TransactionContext       = TCSTransactionContext.SalesInvoice,
                CollectionTrigger        = TCSCollectionTrigger.OnInvoiceBooking,
                ResidentialStatusApplicability = TCSResidentialStatus.All,
                ThresholdMode            = TCSThresholdMode.NoThreshold,
                ThresholdAmount          = null,
                ThresholdComputationBase = TCSThresholdComputationBase.GrossAmountIncludingTax,
                RateResolutionMode       = TCSRateResolutionMode.FixedRateOverride,
                FixedRatePercent         = 5.0m,
                PanAvailabilityRule      = TCSPanAvailabilityRule.HigherRateIfPANMissing,
                AlternateRatePercentIfPanMissing = 10.0m,
                DeductionBaseMode        = TCSDeductionBaseMode.GrossLineAmount,
                TCSPayableGLAccountDisplay = "TCS Payable – 206C",
                ReportingTag             = "27EQ_206C_TENDU",
                IsLockedForChanges       = false,
                CreatedAt                = new DateTime(2024, 3, 20),
            },
        };

        public Dictionary<Guid, TCSConfigFormDto> FormStore { get; } = new();

        public TCSConfigSeedData()
        {
            foreach (var item in Store)
            {
                FormStore[item.TCSConfigId] = new TCSConfigFormDto
                {
                    TCSConfigId = item.TCSConfigId,
                    ConfigCode = item.ConfigCode,
                    ConfigName = item.ConfigName,
                    SectionCode = item.SectionCode,
                    LinkedTaxCodeDisplay = item.LinkedTaxCodeDisplay,
                    ConfigStatus = item.ConfigStatus,
                    Priority = item.Priority,
                    EffectiveFrom = item.EffectiveFrom,
                    EffectiveTo = item.EffectiveTo,
                    TransactionContext = item.TransactionContext,
                    CollectionTrigger = item.CollectionTrigger,
                    ResidentialStatusApplicability = item.ResidentialStatusApplicability,
                    ThresholdMode = item.ThresholdMode,
                    ThresholdAmount = item.ThresholdAmount,
                    ThresholdComputationBase = item.ThresholdComputationBase,
                    RateResolutionMode = item.RateResolutionMode,
                    FixedRatePercent = item.FixedRatePercent,
                    PanAvailabilityRule = item.PanAvailabilityRule,
                    AlternateRatePercentIfPanMissing = item.AlternateRatePercentIfPanMissing,
                    DeductionBaseMode = item.DeductionBaseMode,
                    TCSPayableGLAccountDisplay = item.TCSPayableGLAccountDisplay,
                    ReportingTag = item.ReportingTag,
                    IsLockedForChanges = item.IsLockedForChanges,
                    LockReason = item.LockReason,
                    CreatedAt = item.CreatedAt,
                    UpdatedAt = item.UpdatedAt,
                };
            }
        }
    }
}
