namespace FinanceConnect.Client.ViewModels
{
    public class TCSConfigViewModel
    {
        public enum TCSTransactionContext
        {
            SalesInvoice = 1,
            SalesReceipt = 2,
            AdvanceReceipt = 3,
            Other = 4
        }

        public enum TCSCollectionTrigger
        {
            OnInvoiceBooking = 1,
            OnReceiptOfPayment = 2,
            OnEarlierOfInvoiceOrReceipt = 3
        }

        public enum TCSThresholdMode
        {
            NoThreshold = 1,
            PerDocument = 2,
            CumulativeFinancialYear = 3
        }

        public enum TCSThresholdComparisonRule
        {
            GreaterThan = 1,
            GreaterThanOrEqual = 2
        }

        public enum TCSThresholdComputationBase
        {
            TaxableAmountOnly = 1,
            GrossAmountIncludingTax = 2,
            NetOfDiscountBeforeTax = 3
        }

        public enum TCSRateResolutionMode
        {
            FromTaxRateVersion = 1,
            FixedRateOverride = 2
        }

        public enum TCSPanAvailabilityRule
        {
            UseStandardRateIfPANAvailable = 1,
            HigherRateIfPANMissing = 2,
            BlockWithoutPAN = 3
        }

        public enum TCSDeductionBaseMode
        {
            TaxableLineAmount = 1,
            GrossLineAmount = 2,
            DocumentNetAmount = 3,
            ReceiptAmount = 4
        }

        public enum TCSCertificateValidationMode
        {
            NotRequired = 1,
            ReferenceOnly = 2,
            ReferenceAndDateRangeValidation = 3
        }

        public enum TCSCustomerTypeApplicability
        {
            All = 1,
            Individual = 2,
            Company = 3,
            Partnership = 4,
            Proprietor = 5,
            Other = 6
        }

        public enum TCSResidentialStatus
        {
            All = 1,
            ResidentOnly = 2,
            NonResidentOnly = 3
        }

        public enum TCSFinancialYearBasis
        {
            CompanyFinancialYear = 1,
            GovernmentAssessmentYear = 2  
        }

        public class TCSConfigFormDto
        {
            public Guid TCSConfigId { get; set; } = Guid.Empty;
            public Guid CompanyId { get; set; }
            public Guid? TenantId { get; set; }

            public string ConfigCode { get; set; } = string.Empty; 
            public string ConfigName { get; set; } = string.Empty; 
            public string? Description { get; set; }      
            public ConfigStatus ConfigStatus { get; set; } = ConfigStatus.Active;
            public int Priority { get; set; } = 10;    
            public DateTime EffectiveFrom { get; set; } = DateTime.Today;
            public DateTime? EffectiveTo { get; set; }    

            public Guid LinkedTaxCodeId { get; set; }   
            public string LinkedTaxCodeDisplay { get; set; } = string.Empty;
            public string SectionCode { get; set; } = string.Empty; 
            public string? SectionDescription { get; set; }   

            public TCSTransactionContext TransactionContext { get; set; } = TCSTransactionContext.SalesInvoice;
            public TCSCollectionTrigger CollectionTrigger { get; set; } = TCSCollectionTrigger.OnInvoiceBooking;
            public TCSCustomerTypeApplicability CustomerTypeApplicability { get; set; } = TCSCustomerTypeApplicability.All;
            public TCSResidentialStatus ResidentialStatusApplicability { get; set; } = TCSResidentialStatus.All;
            public bool IsExemptCustomerAllowed { get; set; } = false;
            public Guid? ExpenseCategoryId { get; set; }  
            public Guid? GLAccountFilterId { get; set; }  
            public string? CustomerTagFilter { get; set; }  

            public TCSThresholdMode ThresholdMode { get; set; } = TCSThresholdMode.NoThreshold;
            public decimal? ThresholdAmount { get; set; } 
            public TCSThresholdComparisonRule ThresholdComparisonRule { get; set; } = TCSThresholdComparisonRule.GreaterThan;
            public TCSFinancialYearBasis FinancialYearBasis { get; set; } = TCSFinancialYearBasis.CompanyFinancialYear;
            public TCSThresholdComputationBase ThresholdComputationBase { get; set; } = TCSThresholdComputationBase.TaxableAmountOnly;
            public bool ApplyCollectionOnlyOnExcessAboveThreshold { get; set; } = false;

            public TCSRateResolutionMode RateResolutionMode { get; set; } = TCSRateResolutionMode.FromTaxRateVersion;
            public decimal? FixedRatePercent { get; set; } 
            public TCSPanAvailabilityRule PanAvailabilityRule { get; set; } = TCSPanAvailabilityRule.UseStandardRateIfPANAvailable;
            public decimal? AlternateRatePercentIfPanMissing { get; set; } 
            public bool AllowLowerCollectionCertificate { get; set; } = true;
            public bool AllowNilCollectionCertificate { get; set; } = true;
            public TCSCertificateValidationMode CertificateValidationMode { get; set; } = TCSCertificateValidationMode.ReferenceOnly;

            public TCSDeductionBaseMode DeductionBaseMode { get; set; } = TCSDeductionBaseMode.TaxableLineAmount;
            public bool ExcludeGSTFromCollectionBase { get; set; } = true;
            public bool ExcludeNonCollectibleLines { get; set; } = true;
            public bool AllowManualBaseOverride { get; set; } = false;
            public bool ManualBaseOverrideReasonRequired { get; set; } = true;

            public Guid TCSPayableGLAccountId { get; set; }  
            public string TCSPayableGLAccountDisplay { get; set; } = string.Empty;
            public Guid? ExpenseReclassificationGLAccountId { get; set; } 
            public string? ReportingTag { get; set; } 
            public string? TANReference { get; set; } 
            public string? DefaultChallanType { get; set; }

            public bool IsLockedForChanges { get; set; } = false;
            public string? LockReason { get; set; }   
            public Guid? SupersedesTCSConfigId { get; set; }   
            public DateTime? ApprovedOn { get; set; }
            public string? ApprovedBy { get; set; }

            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public string? CreatedBy { get; set; }
            public string? UpdatedBy { get; set; }
        }

        public class TCSConfigListDto
        {
            public Guid TCSConfigId { get; set; }
            public string ConfigCode { get; set; } = string.Empty;
            public string ConfigName { get; set; } = string.Empty;
            public string SectionCode { get; set; } = string.Empty;
            public string LinkedTaxCodeDisplay { get; set; } = string.Empty;

            public ConfigStatus ConfigStatus { get; set; }
            public int Priority { get; set; }
            public DateTime EffectiveFrom { get; set; }
            public DateTime? EffectiveTo { get; set; }

            public TCSTransactionContext TransactionContext { get; set; }
            public TCSCollectionTrigger CollectionTrigger { get; set; }
            public TCSResidentialStatus ResidentialStatusApplicability { get; set; }
            public TCSThresholdMode ThresholdMode { get; set; }
            public decimal? ThresholdAmount { get; set; }
            public TCSThresholdComputationBase ThresholdComputationBase { get; set; }
            public TCSRateResolutionMode RateResolutionMode { get; set; }
            public decimal? FixedRatePercent { get; set; }
            public TCSPanAvailabilityRule PanAvailabilityRule { get; set; }
            public decimal? AlternateRatePercentIfPanMissing { get; set; }
            public TCSDeductionBaseMode DeductionBaseMode { get; set; }

            public string TCSPayableGLAccountDisplay { get; set; } = string.Empty;
            public string? ReportingTag { get; set; }
            public bool IsLockedForChanges { get; set; }
            public string? LockReason { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }

            public bool IsActive => ConfigStatus == ConfigStatus.Active;
            public bool IsExpired => EffectiveTo.HasValue && EffectiveTo.Value < DateTime.Today;
        }

        public static class TCSConfigHelper
        {
            public static string GetStatusLabel(ConfigStatus s) => s switch
            {
                ConfigStatus.Active => "Active",
                ConfigStatus.Inactive => "Inactive",
                ConfigStatus.Archived => "Archived",
                _ => s.ToString()
            };

            public static string GetStatusClass(ConfigStatus s) => s switch
            {
                ConfigStatus.Active => "bg-success-transparent",
                ConfigStatus.Inactive => "bg-secondary-transparent text-secondary",
                ConfigStatus.Archived => "bg-warning-transparent text-dark",
                _ => "bg-light text-dark"
            };

            public static string GetStatusDot(ConfigStatus s) => s switch
            {
                ConfigStatus.Active => "bg-success",
                ConfigStatus.Inactive => "bg-secondary",
                ConfigStatus.Archived => "bg-warning",
                _ => "bg-light"
            };

            public static string GetTriggerLabel(TCSCollectionTrigger t) => t switch
            {
                TCSCollectionTrigger.OnInvoiceBooking => "On Invoice",
                TCSCollectionTrigger.OnReceiptOfPayment => "On Receipt",
                TCSCollectionTrigger.OnEarlierOfInvoiceOrReceipt => "Whichever Earlier",
                _ => t.ToString()
            };

            public static string GetThresholdLabel(TCSThresholdMode t) => t switch
            {
                TCSThresholdMode.NoThreshold => "No Threshold",
                TCSThresholdMode.PerDocument => "Per Document",
                TCSThresholdMode.CumulativeFinancialYear => "Cumulative (FY)",
                _ => t.ToString()
            };

            public static string GetRateSourceLabel(TCSRateResolutionMode m) => m switch
            {
                TCSRateResolutionMode.FromTaxRateVersion => "From Rate Version",
                TCSRateResolutionMode.FixedRateOverride => "Fixed Rate Override",
                _ => m.ToString()
            };

            public static string GetContextLabel(TCSTransactionContext c) => c switch
            {
                TCSTransactionContext.SalesInvoice => "Sales Invoice",
                TCSTransactionContext.SalesReceipt => "Sales Receipt",
                TCSTransactionContext.AdvanceReceipt => "Advance Receipt",
                TCSTransactionContext.Other => "Other",
                _ => c.ToString()
            };

            public static string GetPanRuleLabel(TCSPanAvailabilityRule p) => p switch
            {
                TCSPanAvailabilityRule.UseStandardRateIfPANAvailable => "Standard Rate if PAN Available",
                TCSPanAvailabilityRule.HigherRateIfPANMissing => "Higher Rate if PAN Missing",
                TCSPanAvailabilityRule.BlockWithoutPAN => "Block Without PAN",
                _ => p.ToString()
            };
        }
    }
}
