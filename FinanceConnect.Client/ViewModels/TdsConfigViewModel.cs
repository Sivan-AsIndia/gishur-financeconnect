using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public class TdsConfigViewModel
    {
        public class TDSConfigFormDto
        {
            public Guid TDSConfigId { get; set; } = Guid.Empty;
            public Guid CompanyId { get; set; }
            public Guid? TenantId { get; set; }

            // ── 01 General — Core Identity ────────────────────────────────────
            public string ConfigCode { get; set; } = string.Empty; 
            public string ConfigName { get; set; } = string.Empty;  
            public string? Description { get; set; }             
            public string SectionCode { get; set; } = string.Empty;  
            public ConfigStatus ConfigStatus { get; set; } = ConfigStatus.Active;

            // ── 02 Effective Period & Priority ────────────────────────────────
            public DateTime EffectiveFrom { get; set; } = DateTime.Today;
            public DateTime? EffectiveTo { get; set; }     
            public int Priority { get; set; } = 10;     
            public bool IsLockedForChanges { get; set; } = false;
            public string? LockReason { get; set; }      

            // ── 03 Applicability Scope ────────────────────────────────────────
            public PartyApplicability PartyApplicability { get; set; } = PartyApplicability.Both;
            public VendorTypeFilter VendorTypeFilter { get; set; } = VendorTypeFilter.Any;
            public ExpenseNatureFilter ExpenseNatureFilter { get; set; } = ExpenseNatureFilter.Any;
            public APDocumentContext APDocumentContext { get; set; } = APDocumentContext.Both;
            public Guid? BranchIdFilter { get; set; }      
            public Guid? CostCenterFilterId { get; set; }      
            public string? ConditionJson { get; set; }     

            // ── 04 Deduction Trigger Logic ────────────────────────────────────
            public DeductionTriggerBasis DeductionTriggerBasis { get; set; } = DeductionTriggerBasis.OnPayment;
            public BaseAmountMode BaseAmountMode { get; set; } = BaseAmountMode.TaxableAmountExcludingGST;
            public bool ApplyOnAdvancePayments { get; set; } = true;
            public bool ApplyOnCreditNotes { get; set; } = false;

            // ── 05 Threshold Rules ────────────────────────────────────────────
            public ThresholdEvaluationMode ThresholdEvaluationMode { get; set; } = ThresholdEvaluationMode.NoThreshold;
            public decimal? ThresholdAmount { get; set; } 
            public ThresholdResetBasis? ThresholdResetBasis { get; set; } 
            public bool ApplyOnlyAboveThreshold { get; set; } = true;
            public bool DeductOnFullAmountOnceThresholdCrossed { get; set; } = false;

            // ── 06 Rate Behavior ──────────────────────────────────────────────
            public Guid PrimaryTaxCodeId { get; set; }      
            public string PrimaryTaxCodeDisplay { get; set; } = string.Empty;
            public TDSRateSourceMode RateSourceMode { get; set; } = TDSRateSourceMode.FromTaxRateVersion;
            public decimal? DefaultRatePercent { get; set; }          
            public bool RequirePANForStandardRate { get; set; } = false;
            public decimal? AlternateRateIfPANMissing { get; set; }      
            public PanValidationMode PanValidationMode { get; set; } = PanValidationMode.PresenceOnly;
            public bool AllowLowerDeductionCertificate { get; set; } = false; 
            public bool AllowNilDeductionCertificate { get; set; } = false;  
            public decimal? MinimumDeductionAmount { get; set; }
            public decimal? MaximumDeductionAmount { get; set; }

            // ── 07 Accounting ─────────────────────────────────────────────────
            public Guid TDSPayableGLAccountId { get; set; }                    
            public string TDSPayableGLAccountDisplay { get; set; } = string.Empty;
            public bool AllowGLOverrideByPostingRule { get; set; } = true;
            public string? SettlementCategoryTag { get; set; }    

            // ── 08 Compliance Metadata ────────────────────────────────────────
            public string? ReturnReportingTag { get; set; }    
            public string? LegalReferenceNote { get; set; }                      

            // ── Audit ─────────────────────────────────────────────────────────
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public string? CreatedBy { get; set; }
            public string? UpdatedBy { get; set; }
        }

        public class TDSConfigListDto
        {
            public Guid TDSConfigId { get; set; }
            public string ConfigCode { get; set; } = string.Empty;
            public string ConfigName { get; set; } = string.Empty;
            public string SectionCode { get; set; } = string.Empty;

            public ConfigStatus ConfigStatus { get; set; }
            public int Priority { get; set; }
            public DateTime EffectiveFrom { get; set; }
            public DateTime? EffectiveTo { get; set; }
            public VendorTypeFilter VendorTypeFilter { get; set; }
            public ExpenseNatureFilter ExpenseNatureFilter { get; set; }
            public PartyApplicability PartyApplicability { get; set; }
            public APDocumentContext APDocumentContext { get; set; }
            public DeductionTriggerBasis DeductionTriggerBasis { get; set; }
            public BaseAmountMode BaseAmountMode { get; set; }
            public ThresholdEvaluationMode ThresholdEvaluationMode { get; set; }
            public decimal? ThresholdAmount { get; set; }
            public ThresholdResetBasis? ThresholdResetBasis { get; set; }

            public TDSRateSourceMode RateSourceMode { get; set; }
            public decimal? DefaultRatePercent { get; set; }
            public bool RequirePANForStandardRate { get; set; }
            public decimal? AlternateRateIfPANMissing { get; set; }
            public PanValidationMode PanValidationMode { get; set; }

            public string TDSPayableGLAccountDisplay { get; set; } = string.Empty;
            public string? SettlementCategoryTag { get; set; }
            public string? ReturnReportingTag { get; set; }

            public bool IsLockedForChanges { get; set; }
            public string? LockReason { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }

            public bool IsActive => ConfigStatus == ConfigStatus.Active;
            public bool IsExpired => EffectiveTo.HasValue && EffectiveTo.Value < DateTime.Today;
        }

        // ── Static helper ──────────────────────────────────────────────────────
        public static class TDSConfigHelper
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
                ConfigStatus.Active => "bg-success",
                ConfigStatus.Inactive => "bg-secondary",
                ConfigStatus.Archived => "bg-warning text-dark",
                _ => "bg-light text-dark"
            };

            public static string GetStatusDot(ConfigStatus s) => s switch
            {
                ConfigStatus.Active => "bg-success",
                ConfigStatus.Inactive => "bg-secondary",
                ConfigStatus.Archived => "bg-warning",
                _ => "bg-light"
            };

            public static string GetTriggerLabel(DeductionTriggerBasis t) => t switch
            {
                DeductionTriggerBasis.OnInvoiceBooking => "On Invoice",
                DeductionTriggerBasis.OnPayment => "On Payment",
                DeductionTriggerBasis.OnBookingOrPaymentWhicheverEarlier => "Whichever Earlier",
                _ => t.ToString()
            };

            public static string GetThresholdLabel(ThresholdEvaluationMode t) => t switch
            {
                ThresholdEvaluationMode.NoThreshold => "No Threshold",
                ThresholdEvaluationMode.PerTransaction => "Per Transaction",
                ThresholdEvaluationMode.CumulativeByVendorInFinancialYear => "Cumulative (FY)",
                ThresholdEvaluationMode.CumulativeByVendorInPeriod => "Cumulative (Period)",
                _ => t.ToString()
            };

            public static string GetRateSourceLabel(TDSRateSourceMode m) => m switch
            {
                TDSRateSourceMode.FromTaxRateVersion => "From Rate Version",
                TDSRateSourceMode.FixedRateOnConfig => "Fixed Rate on Config",
                _ => m.ToString()
            };

            public static string GetBaseAmountLabel(BaseAmountMode m) => m switch
            {
                BaseAmountMode.GrossBillAmount => "Gross Bill Amount",
                BaseAmountMode.TaxableAmountExcludingGST => "Taxable Amount (excl. GST)",
                BaseAmountMode.NetPayableBase => "Net Payable Base",
                _ => m.ToString()
            };

            public static string GetPartyLabel(PartyApplicability p) => p switch
            {
                PartyApplicability.ResidentOnly => "Resident Only",
                PartyApplicability.NonResidentOnly => "Non-Resident Only",
                PartyApplicability.Both => "Both",
                _ => p.ToString()
            };
        }

    }
}
