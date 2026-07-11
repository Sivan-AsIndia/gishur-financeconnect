using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    // ══════════════════════════════════════════════════════════════════════
    //  SHARED ENUMS  (used across Tax module)
    // ══════════════════════════════════════════════════════════════════════

    public enum ConfigStatus { Active = 1, Inactive = 2, Archived = 3 }
    public enum MappingStatus { Active = 1, Inactive = 2, Archived = 3 }
    public enum TaxTypeScope { GST = 1, TDS = 2, TCS = 3, Mixed = 4 }

    public enum TransactionContext
    {
        AR_SalesInvoice = 1,
        AR_SalesCreditNote = 2,
        AP_PurchaseBill = 3,
        AP_PurchaseCreditNote = 4,
        AP_VendorPayment = 5,
        Other = 6
    }

    public enum SupplyType
    {
        IntraState = 1, InterState = 2, Import = 3,
        Export = 4, SEZ = 5, DeemedExport = 6
    }

    public enum PartyApplicability { ResidentOnly = 1, NonResidentOnly = 2, Both = 3 }

    public enum DeductionTriggerBasis
    {
        OnInvoiceBooking = 1,
        OnPayment = 2,
        OnBookingOrPaymentWhicheverEarlier = 3
    }

    public enum BaseAmountMode
    {
        GrossBillAmount = 1,
        TaxableAmountExcludingGST = 2,
        NetPayableBase = 3
    }

    public enum ThresholdEvaluationMode
    {
        NoThreshold = 1,
        PerTransaction = 2,
        CumulativeByVendorInFinancialYear = 3,
        CumulativeByVendorInPeriod = 4
    }

    public enum ThresholdResetBasis { FinancialYear = 1, ReturnPeriod = 2, Never = 3 }

    public enum APDocumentContext { VendorBill = 1, VendorPayment = 2, Both = 3 }
    public enum PanValidationMode { NotRequired = 1, PresenceOnly = 2, PresenceAndFormat = 3 }

    public enum VendorTypeFilter
    {
        Contractor = 1, Professional = 2, CommissionAgent = 3,
        RentPayee = 4, Other = 5, Any = 6
    }

    public enum ExpenseNatureFilter
    {
        Contract = 1, ProfessionalFee = 2, Rent = 3,
        Commission = 4, Interest = 5, Other = 6, Any = 7
    }

    // ── TDS-specific rate source ──────────────────────────────────────────
    // Named TDSRateSourceMode to avoid collision with TaxCategoryMapping's
    // RateResolutionMode enum which has different values but a similar name.
    public enum TDSRateSourceMode { FromTaxRateVersion = 1, FixedRateOnConfig = 2 }

    // ── TaxCategoryMapping-specific enums ─────────────────────────────────
    public enum CustomerType { Registered = 1, Unregistered = 2, Composition = 3, SEZ = 4, Export = 5 }
    public enum VendorType { Registered = 1, Unregistered = 2, SEZ = 3, ImportVendor = 4 }

    public enum PlaceOfSupplyRuleMode
    {
        UseShipToState = 1, UseBillToState = 2,
        ServicePlaceOfSupplyPolicy = 3, ManualOverrideAllowed = 4
    }

    public enum ExemptType { None = 1, Exempt = 2, NilRated = 3, NonGST = 4 }
    public enum ApplyMode { AddOn = 1, Inclusive = 2, Withholding = 3, Collection = 4 }
    public enum RateResolutionMode { FromTaxRateVersionByDate = 1, FixedOverrideRate = 2 }
    public enum RateEffectiveDateBasis { DocumentDate = 1, PostingDate = 2 }
    public enum ITCEligibilityOverride { Inherit = 1, Eligible = 2, Ineligible = 3, Provisional = 4 }
    public enum RCMBehavior { Normal = 1, RCM_OutputLiabilityOnly = 2, RCM_OutputPlusInputCredit = 3 }

    public enum GSTReturnClassification
    {
        Outward_Taxable = 1, Outward_Exempt = 2, Outward_Nil = 3, Outward_Export = 4,
        Inward_ITC_Eligible = 5, Inward_ITC_Ineligible = 6, RCM_Liability = 7
    }

    // ══════════════════════════════════════════════════════════════════════
    //  MODEL #64 — TaxCategoryMapping
    // ══════════════════════════════════════════════════════════════════════
    public class TaxCategoryMappingViewModel
    {
        // ── Full model (used in Create / Edit form with DataAnnotations) ───────
        public class TaxCategoryMappingModel
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");
            public Guid CompanyId { get; set; }
            public string? CompanyName { get; set; }

            [Required(ErrorMessage = "Mapping Code is required")]
            [StringLength(30, ErrorMessage = "Mapping Code must not exceed 30 characters")]
            [RegularExpression(@"^[A-Za-z0-9_-]+$", ErrorMessage = "Letters, numbers, hyphens and underscores only")]
            public string MappingCode { get; set; } = string.Empty;

            [Required(ErrorMessage = "Mapping Name is required")]
            [StringLength(200, ErrorMessage = "Mapping Name must not exceed 200 characters")]
            public string MappingName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Tax Type Scope is required")]
            public string TaxTypeScope { get; set; } = string.Empty;

            [Required(ErrorMessage = "Transaction Context is required")]
            public string TransactionContext { get; set; } = string.Empty;

            [Required(ErrorMessage = "Mapping Status is required")]
            public string MappingStatus { get; set; } = "Draft";

            [Required(ErrorMessage = "Priority is required")]
            [Range(1, int.MaxValue, ErrorMessage = "Priority must be greater than 0")]
            public int Priority { get; set; } = 10;

            [Required(ErrorMessage = "Effective From is required")]
            public DateTime EffectiveFrom { get; set; } = DateTime.Today;

            public DateTime? EffectiveTo { get; set; }

            public bool IsLockedForChanges { get; set; } = false;
            [StringLength(300)]
            public string? LockReason { get; set; }

            [StringLength(1000)]
            public string? Description { get; set; }

            // Condition fields
            public string? SupplyType { get; set; }
            public string? CustomerType { get; set; }
            public string? VendorType { get; set; }
            [StringLength(5)]
            public string? FromStateCode { get; set; }
            [StringLength(5)]
            public string? ToStateCode { get; set; }
            public string PlaceOfSupplyRuleMode { get; set; } = "UseShipToState";
            public bool? IsReverseChargeApplicable { get; set; }
            public Guid? ItemTaxCategoryId { get; set; }
            public string? ItemTaxCategoryName { get; set; }
            public bool? IsService { get; set; }
            [StringLength(20)]
            public string? HSNOrSAC { get; set; }
            public string IsExemptOrNilOrNonGST { get; set; } = "None";
            public string? ConditionJson { get; set; }

            // Reporting
            public string? GSTReturnClassification { get; set; }
            public bool RequiresInvoiceLevelReporting { get; set; } = true;
            public string? ConditionSignatureHash { get; set; }

            // Audit
            public DateTime CreatedAt { get; set; } = DateTime.Now;
            public string? CreatedBy { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public string? UpdatedBy { get; set; }
            public bool IsDeleted { get; set; } = false;
            public DateTime? DeletedAt { get; set; }
            public string? DeletedBy { get; set; }
            public byte[]? RowVersion { get; set; }

            // Lines
            public List<TaxCategoryMappingLineModel> Lines { get; set; } = new();

            // Usage metadata
            public bool IsUsedInPostedTransactions { get; set; } = false;
            public int UsedInTransactionCount { get; set; } = 0;
        }

        // ── Line model ────────────────────────────────────────────────────────
        public class TaxCategoryMappingLineModel
        {
            public Guid Id { get; set; } = Guid.NewGuid();

            [Required]
            public Guid TaxCategoryMappingId { get; set; }

            [Required(ErrorMessage = "Line Number is required")]
            public int LineNumber { get; set; }

            [Required(ErrorMessage = "Tax Code is required")]
            public Guid TaxCodeId { get; set; }
            public string? TaxCodeCode { get; set; }
            public string? TaxCodeName { get; set; }
            public string? TaxCodeComponent { get; set; }
            public string? TaxCodeDirection { get; set; }

            [Required(ErrorMessage = "Apply Mode is required")]
            public string ApplyMode { get; set; } = "AddOn";

            [Required(ErrorMessage = "Rate Resolution Mode is required")]
            public string RateResolutionMode { get; set; } = "FromTaxRateVersionByDate";

            [Range(0, 100, ErrorMessage = "Override Rate must be between 0 and 100")]
            public decimal? OverrideRatePercent { get; set; }
            public string RateEffectiveDateBasis { get; set; } = "PostingDate";
            public string ITCEligibilityOverride { get; set; } = "Inherit";
            public string RCMBehavior { get; set; } = "Normal";
            public Guid? GLAccountOverrideId { get; set; }
            public string? GLAccountOverrideName { get; set; }
            public bool IsLineActive { get; set; } = true;
            [StringLength(300)]
            public string? LineNotes { get; set; }
        }

        // ── List DTO (for grid / list views) ─────────────────────────────────
        public class TaxCategoryMappingListDto
        {
            public Guid TaxCategoryMappingId { get; set; }
            public string MappingCode { get; set; } = string.Empty;
            public string MappingName { get; set; } = string.Empty;
            public TaxTypeScope TaxTypeScope { get; set; }
            public TransactionContext TransactionContext { get; set; }
            public MappingStatus MappingStatus { get; set; }
            public int Priority { get; set; }
            public DateTime EffectiveFrom { get; set; }
            public DateTime? EffectiveTo { get; set; }
            public SupplyType? SupplyType { get; set; }
            public ExemptType IsExemptOrNilOrNonGST { get; set; }
            public bool IsLockedForChanges { get; set; }
            public int LineCount { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        // ── Static helper class ───────────────────────────────────────────────
        public static class TaxMappingHelper
        {
            public static string GetStatusLabel(MappingStatus s) => s switch
            {
                MappingStatus.Active => "Active",
                MappingStatus.Inactive => "Inactive",
                MappingStatus.Archived => "Archived",
                _ => s.ToString()
            };

            public static string GetStatusClass(MappingStatus s) => s switch
            {
                MappingStatus.Active => "bg-success",
                MappingStatus.Inactive => "bg-secondary",
                MappingStatus.Archived => "bg-warning text-dark",
                _ => "bg-light text-dark"
            };

            public static string GetStatusDot(MappingStatus s) => s switch
            {
                MappingStatus.Active => "bg-success",
                MappingStatus.Inactive => "bg-secondary",
                MappingStatus.Archived => "bg-warning",
                _ => "bg-light"
            };

            public static string GetScopeClass(TaxTypeScope s) => s switch
            {
                TaxTypeScope.GST => "bg-primary-transparent",
                TaxTypeScope.TDS => "bg-warning-transparent text-dark",
                TaxTypeScope.TCS => "bg-info-transparent",
                TaxTypeScope.Mixed => "bg-secondary-transparent",
                _ => "bg-light text-dark"
            };

            public static string GetContextLabel(TransactionContext c) => c switch
            {
                TransactionContext.AR_SalesInvoice => "Sales Invoice",
                TransactionContext.AR_SalesCreditNote => "Sales Credit Note",
                TransactionContext.AP_PurchaseBill => "Purchase Bill",
                TransactionContext.AP_PurchaseCreditNote => "Purchase Credit Note",
                TransactionContext.AP_VendorPayment => "Vendor Payment (TDS)",
                TransactionContext.Other => "Other",
                _ => c.ToString()
            };
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    //  MODEL #67 — TDSConfig
    // ══════════════════════════════════════════════════════════════════════
    public class TDSConfigViewModel
    {
        public class TDSConfigFormDto
        {
            public Guid TDSConfigId { get; set; } = Guid.Empty;
            public Guid CompanyId { get; set; }

            // Core Identity
            public string ConfigCode { get; set; } = string.Empty;
            public string ConfigName { get; set; } = string.Empty;
            public string? Description { get; set; }
            public string SectionCode { get; set; } = string.Empty;
            public ConfigStatus ConfigStatus { get; set; } = ConfigStatus.Active;

            // Effective Period & Priority
            public DateTime EffectiveFrom { get; set; } = DateTime.Today;
            public DateTime? EffectiveTo { get; set; }
            public int Priority { get; set; } = 10;
            public bool IsLockedForChanges { get; set; }
            public string? LockReason { get; set; }

            // Applicability Scope
            public PartyApplicability PartyApplicability { get; set; } = PartyApplicability.Both;
            public VendorTypeFilter VendorTypeFilter { get; set; } = VendorTypeFilter.Any;
            public ExpenseNatureFilter ExpenseNatureFilter { get; set; } = ExpenseNatureFilter.Any;
            public APDocumentContext APDocumentContext { get; set; } = APDocumentContext.Both;

            // Deduction Trigger
            public DeductionTriggerBasis DeductionTriggerBasis { get; set; } = DeductionTriggerBasis.OnPayment;
            public bool ApplyOnAdvancePayments { get; set; } = true;
            public bool ApplyOnCreditNotes { get; set; }

            // Threshold Rules
            public decimal? ThresholdAmount { get; set; }
            public ThresholdResetBasis? ThresholdResetBasis { get; set; }
            public bool ApplyOnlyAboveThreshold { get; set; } = true;
            public bool DeductOnFullAmountOnceThresholdCrossed { get; set; }

            // Rate Behavior — uses TDSRateSourceMode (renamed to avoid collision with RateResolutionMode)
            public Guid PrimaryTaxCodeId { get; set; }
            public string PrimaryTaxCodeDisplay { get; set; } = string.Empty;
            public TDSRateSourceMode RateSourceMode { get; set; } = TDSRateSourceMode.FromTaxRateVersion;
            public decimal? DefaultRatePercent { get; set; }
            public bool RequirePANForStandardRate { get; set; }
            public decimal? AlternateRateIfPANMissing { get; set; }
            public PanValidationMode PanValidationMode { get; set; } = PanValidationMode.PresenceOnly;
            public bool AllowLowerDeductionCertificate { get; set; }
            public bool AllowNilDeductionCertificate { get; set; }
            public decimal? MinimumDeductionAmount { get; set; }
            public decimal? MaximumDeductionAmount { get; set; }

            // Accounting
            public Guid TDSPayableGLAccountId { get; set; }
            public string TDSPayableGLAccountDisplay { get; set; } = string.Empty;
            public bool AllowGLOverrideByPostingRule { get; set; } = true;
            public string? SettlementCategoryTag { get; set; }

            // Compliance
            public string? ReturnReportingTag { get; set; }
            public string? LegalReferenceNote { get; set; }

            // Audit
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
            public PartyApplicability PartyApplicability { get; set; }
            public DeductionTriggerBasis DeductionTriggerBasis { get; set; }
            public ThresholdEvaluationMode ThresholdEvaluationMode { get; set; }
            public decimal? ThresholdAmount { get; set; }
            public TDSRateSourceMode RateSourceMode { get; set; }   // TDS-specific enum
            public decimal? DefaultRatePercent { get; set; }
            public bool IsLockedForChanges { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }

            public bool IsActive => ConfigStatus == ConfigStatus.Active;
            public bool IsExpired => EffectiveTo.HasValue && EffectiveTo.Value < DateTime.Today;
        }

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
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    //  SHARED UTILITY
    // ══════════════════════════════════════════════════════════════════════
    public class SelectItem
    {
        public string Value { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }
}
