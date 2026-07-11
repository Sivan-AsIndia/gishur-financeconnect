namespace FinanceConnect.Client.ViewModels
{

    public enum DeductionStatus
    {
        Draft = 1,
        Posted = 2,
        PartiallySettled = 3,
        Settled = 4,
        Reversed = 5,
        Cancelled = 6
    }

    public enum SourceDocumentType
    {
        VendorBill = 1,
        VendorPayment = 2,
        PaymentAllocation = 3,
        ManualAdjustment = 4
    }

    public enum SourceTriggerBasis
    {
        OnInvoiceBooking = 1,
        OnPayment = 2,
        OnBookingOrPaymentWhicheverEarlier = 3
    }

    public enum VendorResidency
    {
        Resident = 1,
        NonResident = 2
    }

    public enum ThresholdApplicationMode
    {
        NotApplicable = 1,
        DeductOnlyOnExcess = 2,
        DeductOnFullAmountOnceCrossed = 3
    }

    public enum SettlementStatus
    {
        NotSettled = 1,
        PartiallySettled = 2,
        FullySettled = 3
    }

    public enum PostingPattern
    {
        ReduceVendorSettlement = 1,
        ReduceBankOnly = 2
    }

    public class TDSDeductionEntryViewModel
    {
        public Guid TDSDeductionEntryId { get; set; } = Guid.Empty;
        public Guid CompanyId { get; set; }
        public Guid BranchId { get; set; }
        public string DeductionNumber { get; set; } = string.Empty;
        public DeductionStatus Status { get; set; } = DeductionStatus.Draft;
        public DateTime DeductionDate { get; set; } = DateTime.Today;
        public DateTime PostingDate { get; set; } = DateTime.Today;

        public SourceDocumentType SourceDocumentType { get; set; }
        public Guid SourceDocumentId { get; set; }
        public string SourceDocumentNumberSnapshot { get; set; } = string.Empty;
        public DateTime SourceDocumentDateSnapshot { get; set; }
        public SourceTriggerBasis SourceTriggerBasisSnapshot { get; set; }

        public Guid VendorId { get; set; }
        public string VendorCodeSnapshot { get; set; } = string.Empty;
        public string VendorNameSnapshot { get; set; } = string.Empty;
        public string? VendorPANSnapshot { get; set; }
        public VendorResidency VendorResidencySnapshot { get; set; } = VendorResidency.Resident;

        public Guid TDSConfigId { get; set; }
        public string SectionCodeSnapshot { get; set; } = string.Empty;
        public string TaxCodeSnapshot { get; set; } = string.Empty;
        public decimal RatePercentApplied { get; set; }
        public bool IsAlternatePanRateApplied { get; set; }

        public decimal DeductionBaseAmount { get; set; }
        public decimal DeductionAmount { get; set; }
        public decimal SettledAmount { get; set; }
        public SettlementStatus SettlementStatus { get; set; }

        public ThresholdEvaluationMode ThresholdEvaluationModeSnapshot { get; set; }
        public bool ThresholdTriggeredFlag { get; set; }

        public DateTime? PostedOn { get; set; }
        public DateTime? LastSettlementDate { get; set; }
        public bool IsSystemReversal { get; set; }
        public Guid? ReversalOfTDSDeductionEntryId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string ConfigCodeSnapshot { get; set; } = string.Empty;
        public Guid PrimaryTaxCodeId { get; set; }
        public Guid? TaxRateVersionId { get; set; }
        public TDSRateSourceMode RateSourceModeSnapshot { get; set; }
        public string? AlternateRateReason { get; set; }
        public BaseAmountMode BaseAmountModeSnapshot { get; set; }
        public decimal? ThresholdAmountSnapshot { get; set; }
        public decimal? ThresholdObservedBaseBeforeCurrentTxn { get; set; }
        public decimal? ThresholdObservedBaseAfterCurrentTxn { get; set; }
        public ThresholdApplicationMode ThresholdApplicationModeSnapshot { get; set; }
            = ThresholdApplicationMode.NotApplicable;

        public decimal? GrossPayableAmountSnapshot { get; set; }
        public decimal? NetPayableAfterTDSAmount { get; set; }
        public decimal? RoundingDifferenceAmount { get; set; }
        public string? CalculationFormulaSnapshot { get; set; }
        public string? CalculationDetailsJson { get; set; }

        public Guid TDSPayableGLAccountIdSnapshot { get; set; }
        public Guid? JournalEntryId { get; set; }
        public Guid? JournalLineId { get; set; }
        public string? PostedBy { get; set; }
        public PostingPattern PostingPatternSnapshot { get; set; }

        public Guid? LastTaxSettlementId { get; set; }
        public string? ReversalReason { get; set; }
        public Guid? ReversalJournalEntryId { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public class TDSDeductionEntryListDto
        {
            public Guid TDSDeductionEntryId { get; set; }
            public string DeductionNumber { get; set; } = string.Empty;
            public DeductionStatus Status { get; set; }
            public DateTime DeductionDate { get; set; }
            public DateTime PostingDate { get; set; }

            public SourceDocumentType SourceDocumentType { get; set; }
            public string? SourceDocumentNumberSnapshot { get; set; }

            public string? VendorCodeSnapshot { get; set; }
            public string? VendorNameSnapshot { get; set; }
            public string? VendorPANSnapshot { get; set; }
            public VendorResidency VendorResidencySnapshot { get; set; }

            public string? SectionCodeSnapshot { get; set; }
            public string? TaxCodeSnapshot { get; set; }
            public decimal RatePercentApplied { get; set; }
            public bool IsAlternatePanRateApplied { get; set; }

            public decimal DeductionBaseAmount { get; set; }
            public decimal DeductionAmount { get; set; }
            public decimal SettledAmount { get; set; }
            public SettlementStatus SettlementStatus { get; set; }

            // Computed
            public decimal OutstandingAmount => DeductionAmount - SettledAmount;
            public ThresholdEvaluationMode ThresholdEvaluationModeSnapshot { get; set; }
            public bool ThresholdTriggeredFlag { get; set; }

            public bool IsSystemReversal { get; set; }
            public Guid? ReversalOfTDSDeductionEntryId { get; set; }

            public DateTime? PostedOn { get; set; }
            public DateTime? LastSettlementDate { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public static class TDSDeductionEntry
        {
            public static string GetStatusLabel(DeductionStatus s) => s switch
            {
                DeductionStatus.Draft => "Draft",
                DeductionStatus.Posted => "Posted",
                DeductionStatus.PartiallySettled => "Partially Settled",
                DeductionStatus.Settled => "Settled",
                DeductionStatus.Reversed => "Reversed",
                DeductionStatus.Cancelled => "Cancelled",
                _ => s.ToString()
            };

            public static string GetStatusPillClass(DeductionStatus s) => s switch
            {
                DeductionStatus.Draft => "draft",
                DeductionStatus.Posted => "posted",
                DeductionStatus.PartiallySettled => "partial",
                DeductionStatus.Settled => "settled",
                DeductionStatus.Reversed => "reversed",
                DeductionStatus.Cancelled => "cancelled",
                _ => ""
            };

            public static string GetSettlementStatusLabel(SettlementStatus s) => s switch
            {
                SettlementStatus.NotSettled => "Not Settled",
                SettlementStatus.PartiallySettled => "Partially Settled",
                SettlementStatus.FullySettled => "Fully Settled",
                _ => s.ToString()
            };

            public static string GetSettlementStatusClass(SettlementStatus s) => s switch
            {
                SettlementStatus.NotSettled => "bg-secondary",
                SettlementStatus.PartiallySettled => "bg-warning text-dark",
                SettlementStatus.FullySettled => "bg-success",
                _ => "bg-light text-dark"
            };
        }
        public class SelectItem
        {
            public string Value { get; set; } = string.Empty;
            public string Text { get; set; } = string.Empty;
        }
    }
}
