using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public class TaxTranactionViewModel
    {

        public class TaxTransactionModel
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

            [Required]
            public Guid CompanyId { get; set; }
            public string? CompanyName { get; set; }

            [Required]
            public Guid BranchId { get; set; }
            public string? BranchName { get; set; }

            [StringLength(40)]
            public string? TaxTransactionNumber { get; set; }  

            [Required(ErrorMessage = "Tax Transaction Status is required")]
            public string TaxTransactionStatus { get; set; } = "Draft";

            [Required(ErrorMessage = "Tax Type is required")]
            public string TaxType { get; set; } = string.Empty;
            [Required(ErrorMessage = "Source Document Type is required")]
            public string SourceDocumentType { get; set; } = string.Empty;

            [Required(ErrorMessage = "Source Document Id is required")]
            public Guid SourceDocumentId { get; set; }

            [Required]
            [StringLength(50)]
            public string SourceDocumentNumberSnapshot { get; set; } = string.Empty;

            [Required]
            public DateTime SourceDocumentDateSnapshot { get; set; }

            [Required(ErrorMessage = "Posting Date is required")]
            public DateTime PostingDate { get; set; }

            [Required]
            public Guid AccountingPeriodId { get; set; }
            public string? AccountingPeriodName { get; set; }

            [StringLength(20)]
            public string? TaxPeriodKey { get; set; }

            public bool IsCreditOrDebitAdjustment { get; set; } = false; 

            public Guid? OriginalSourceDocumentId { get; set; } 

            [Required(ErrorMessage = "Party Type is required")]
            public string PartyType { get; set; } = string.Empty;

            [Required]
            public Guid PartyId { get; set; }

            [Required]
            [StringLength(200)]
            public string PartyNameSnapshot { get; set; } = string.Empty;

            [StringLength(20)]
            public string? PartyGSTINSnapshot { get; set; } 

            [StringLength(20)]
            public string? PartyPANSnapshot { get; set; } 

            public string? PartyRegistrationTypeSnapshot { get; set; }

            public bool IsPartyGSTRegistered { get; set; } = false;
            public string? SupplyType { get; set; }

            [StringLength(5)]
            public string? FromStateCode { get; set; }

            [StringLength(5)]
            public string? ToStateCode { get; set; }

            public string PlaceOfSupplyMode { get; set; } = "ShipToState";

            public bool IsReverseCharge { get; set; } = false;

            [Required]
            public decimal TaxableValueTotal { get; set; }

            [Required]
            public decimal TaxAmountTotal { get; set; } 

            public decimal? GrossDocumentValueSnapshot { get; set; } 

            public decimal ITCEligibleTaxAmount { get; set; } = 0;
            public decimal ITCIneligibleTaxAmount { get; set; } = 0;
            public decimal RCMLiabilityTaxAmount { get; set; } = 0;
            public decimal WithholdingTaxAmount { get; set; } = 0; 
            public decimal CollectionTaxAmount { get; set; } = 0;  
            [Required]
            public Guid AppliedMappingIdSnapshot { get; set; }
            public string? AppliedMappingCodeSnapshot { get; set; }
            public int? AppliedMappingVersionSnapshot { get; set; }

            public string RateResolutionBasisSnapshot { get; set; } = "PostingDate";
            public string RateVersionResolutionModeSnapshot { get; set; } = "FromTaxRateVersionByDate";

            [StringLength(20)]
            public string? CalculationEngineVersion { get; set; } 

            public bool IsIncludedInReturn { get; set; } = false;

            public Guid? GSTReturnRunId { get; set; } 

            public string ReturnInclusionStatus { get; set; } = "Pending";

            [StringLength(500)]
            public string? ExclusionReason { get; set; }  

            public string ReconciliationStatus { get; set; } = "NotReconciled";

            [StringLength(500)]
            public string? ReconciliationNotes { get; set; }
            public Guid? JournalEntryId { get; set; }  
            public DateTime? PostedOn { get; set; }
            public string? PostedBy { get; set; }

            public Guid? ReversalTaxTransactionId { get; set; } 
            public Guid? ReversalJournalEntryId { get; set; }

            [StringLength(500)]
            public string? ReversalReason { get; set; } 
            public DateTime CreatedAt { get; set; } = DateTime.Now;
            public string? CreatedBy { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public string? UpdatedBy { get; set; }
            public bool IsDeleted { get; set; } = false;
            public DateTime? DeletedAt { get; set; }
            public string? DeletedBy { get; set; }
            public byte[]? RowVersion { get; set; }
            public List<TaxTransactionLineModel> Lines { get; set; } = new();
        }

        public class TaxTransactionLineModel
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

            [Required]
            public Guid CompanyId { get; set; }

            [Required(ErrorMessage = "TaxTransactionId is required")]
            public Guid TaxTransactionId { get; set; } 

            [Required(ErrorMessage = "Line Number is required")]
            public int LineNumber { get; set; }     

            [Required]
            public string SourceDocumentTypeSnapshot { get; set; } = string.Empty;
            [StringLength(30)]
            public string? AppliedMappingCodeSnapshot { get; set; } 
            [Required]
            public Guid SourceDocumentIdSnapshot { get; set; }

            public Guid? SourceLineId { get; set; } 

            public string? SourceLineNumberSnapshot { get; set; }

            [StringLength(300)]
            public string? SourceLineDescriptionSnapshot { get; set; }

            public decimal? SourceLineAmountSnapshot { get; set; } 

            [Required]
            public Guid TaxCodeId { get; set; } 

            [Required]
            [StringLength(30)]
            public string TaxCodeSnapshot { get; set; } = string.Empty; 

            [Required]
            [StringLength(150)]
            public string TaxNameSnapshot { get; set; } = string.Empty;

            [Required]
            public string TaxTypeSnapshot { get; set; } = string.Empty;

            public string? GSTComponentTypeSnapshot { get; set; }

            [Required]
            public string TaxDirectionSnapshot { get; set; } = string.Empty;
            [Required]
            public Guid AppliedMappingIdSnapshot { get; set; } 

            public Guid? AppliedMappingLineIdSnapshot { get; set; } 

            public Guid? TaxRateVersionId { get; set; }   

            public int? RateVersionNumberSnapshot { get; set; }

            [Required]
            public string RateTypeSnapshot { get; set; } = "Percentage";

            [Range(0, 100)]
            public decimal? RatePercentSnapshot { get; set; }

            public decimal? FixedAmountSnapshot { get; set; }

            [Required]
            public DateTime RateResolutionDateSnapshot { get; set; }

            public string RateResolutionBasisSnapshot { get; set; } = "PostingDate";

            [Required]
            public decimal TaxableBaseAmount { get; set; } 

            public decimal? AssessableValueAmount { get; set; } 

            [Required]
            public decimal TaxAmount { get; set; } 

            public decimal? GrossLineValueSnapshot { get; set; }

            [Required]
            public string InclusiveExclusiveMode { get; set; } = "Exclusive";
            public decimal RoundingDifferenceAmount { get; set; } = 0;

            [StringLength(500)]
            public string? CalculationFormulaSnapshot { get; set; } 

            public string? ComputationDetailsJson { get; set; } 
            public string? SupplyTypeSnapshot { get; set; }

            [StringLength(5)]
            public string? FromStateCodeSnapshot { get; set; }

            [StringLength(5)]
            public string? ToStateCodeSnapshot { get; set; }

            public bool IsReverseChargeLine { get; set; } = false;

            public string ITCEligibilityStatus { get; set; } = "NotApplicable";
            public string? GSTReturnTagSnapshot { get; set; }  

            public string ExemptionClassification { get; set; } = "None";
            [StringLength(20)]
            public string? SectionCodeSnapshot { get; set; } 

            public bool ThresholdAppliedFlag { get; set; } = false;

            public decimal? ThresholdAmountSnapshot { get; set; }

            public bool IsPanMissingAlternateRateApplied { get; set; } = false;
            public decimal? BaseForWithholdingAmount { get; set; }
            [Required]
            public string LineStatus { get; set; } = "Posted";
            public bool IsIncludedInReturn { get; set; } = false;

            public decimal SettlementAppliedAmount { get; set; } = 0;

            public Guid? ReversalOfTaxTransactionLineId { get; set; }

            [StringLength(500)]
            public string? ReversalReason { get; set; } 
            public Guid? JournalEntryId { get; set; }
            public Guid? JournalLineId { get; set; }
            public DateTime? PostedOn { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.Now;
            public string? CreatedBy { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public string? UpdatedBy { get; set; }
            public bool IsDeleted { get; set; } = false;
            public byte[]? RowVersion { get; set; }
        }

    }
}
