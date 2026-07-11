using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public class AssetRevaluationImpairmentViewModel
    {
        // ── Enums ──────────────────────────────────────────────────────
        public enum EventStatusEnum
        {
            Draft = 1,
            Submitted = 2,
            Approved = 3,
            Rejected = 4,
            Posted = 5,
            Cancelled = 6,
            Reversed = 7,
            Closed = 8
        }

        public enum EventTypeEnum
        {
            RevaluationIncrease = 1,
            RevaluationDecrease = 2,
            ImpairmentLoss = 3,
            ImpairmentReversal = 4
        }

        public enum ReasonCodeEnum
        {
            FairValueUpdate = 1,
            MarketDecline = 2,
            Damage = 3,
            Obsolescence = 4,
            RegulatoryChange = 5,
            Correction = 6,
            Other = 7
        }

        public enum CalculationModeEnum
        {
            AdjustByDelta = 1,
            RevalueToAmount = 2
        }

        public enum ValuationBasisEnum
        {
            ExternalValuerReport = 1,
            InternalAssessment = 2,
            MarketComparable = 3,
            InsuranceValuation = 4,
            Other = 5
        }

        public enum AccumDepTreatmentModeEnum
        {
            ProportionalRestatement = 1,
            EliminateAccumDepAgainstCost = 2,
            KeepAccumDepNoChange = 3
        }

        public enum RevalDecreaseHandlingModeEnum
        {
            ChargeToReserveThenPAndL = 1,
            DirectToPAndL = 2
        }

        public enum RegenerationModeEnum
        {
            ForwardOnlyFromNextPeriod = 1,
            RebuildFromEffectiveDate = 2
        }

        // ── AssetRevaluationImpairment Model ───────────────────────────
        public class AssetRevaluationImpairment
        {
            // ── Section 1: Core Identity ───────────────────────────────
            public Guid AssetRevaluationImpairmentId { get; set; }
            public Guid TenantId { get; set; }
            public Guid CompanyId { get; set; }
            public Guid? BranchId { get; set; }

            [StringLength(40)]
            public string EventNumber { get; set; } = "";

            [Required(ErrorMessage = "Event Status is required")]
            public EventStatusEnum EventStatus { get; set; } = EventStatusEnum.Draft;

            // ── Section 2: Asset Linkage + Snapshot ────────────────────
            [Required(ErrorMessage = "Fixed Asset is required")]
            public Guid? FixedAssetId { get; set; }

            [StringLength(40)]
            public string? AssetNumberSnapshot { get; set; }

            [StringLength(200)]
            public string? AssetNameSnapshot { get; set; }

            public Guid? AssetCategoryIdSnapshot { get; set; }

            public DateTime? InServiceDateSnapshot { get; set; }

            public string? AssetStatusSnapshot { get; set; }

            public Guid? CurrencyId { get; set; }

            // ── Section 3: Event Type & Effective Date ─────────────────
            [Required(ErrorMessage = "Event Type is required")]
            public EventTypeEnum? EventType { get; set; }

            [Required(ErrorMessage = "Effective Date is required")]
            public DateTime? EffectiveDate { get; set; }

            public DateTime? PostingDate { get; set; }

            public ReasonCodeEnum? ReasonCode { get; set; }

            [StringLength(1000)]
            public string? Narration { get; set; }

            // ── Section 4: Valuation Inputs ────────────────────────────
            [Required(ErrorMessage = "Calculation Mode is required")]
            public CalculationModeEnum? CalculationMode { get; set; }

            [Range(0.01, double.MaxValue, ErrorMessage = "Delta Amount must be > 0")]
            public decimal? DeltaAmount { get; set; }

            [Range(0, double.MaxValue, ErrorMessage = "Target Carrying Amount must be >= 0")]
            public decimal? TargetCarryingAmount { get; set; }

            [Required(ErrorMessage = "Valuation Basis is required")]
            public ValuationBasisEnum? ValuationBasis { get; set; }

            [StringLength(200)]
            public string? ValuerName { get; set; }

            [StringLength(100)]
            public string? ValuationReportReference { get; set; }

            public DateTime? ValuationReportDate { get; set; }

            // ── Section 5: Accum Dep Treatment ─────────────────────────
            [Required(ErrorMessage = "Accum. Depreciation Treatment is required")]
            public AccumDepTreatmentModeEnum? AccumDepTreatmentMode { get; set; }

            public bool AllowAccumDepTreatmentOverride { get; set; } = false;

            [StringLength(500)]
            public string? AccumDepTreatmentReason { get; set; }

            // ── Section 6: Before/After Values ─────────────────────────
            public decimal GrossCostBefore { get; set; }
            public decimal AccumDepBefore { get; set; }
            public decimal CarryingValueBefore { get; set; }
            public decimal ResidualValueAmountBefore { get; set; }

            public decimal GrossCostAfter { get; set; }
            public decimal AccumDepAfter { get; set; }
            public decimal CarryingValueAfter { get; set; }

            public decimal AdjustmentAmount => CarryingValueAfter - CarryingValueBefore;

            public bool IsGainOrLossToPAndLFlag { get; set; }

            public string? PolicyWarningsJson { get; set; }

            // ── Section 7: GL Posting Mapping Snapshot ─────────────────
            public Guid? AssetCostGLAccountIdSnapshot { get; set; }
            public Guid? AccumulatedDepreciationGLAccountIdSnapshot { get; set; }
            public Guid? RevaluationReserveGLAccountIdSnapshot { get; set; }
            public Guid? ImpairmentLossGLAccountIdSnapshot { get; set; }
            public Guid? ImpairmentReversalGLAccountIdSnapshot { get; set; }

            public RevalDecreaseHandlingModeEnum? RevaluationDecreaseHandlingMode { get; set; }

            public Guid? JournalEntryId { get; set; }
            public DateTime? PostedOn { get; set; }
            public Guid? PostedBy { get; set; }
            public Guid? ReversalJournalEntryId { get; set; }

            // ── Section 8: Depreciation Schedule Impact ────────────────
            public bool RequiresScheduleRegeneration { get; set; } = true;

            public RegenerationModeEnum RegenerationMode { get; set; } = RegenerationModeEnum.ForwardOnlyFromNextPeriod;

            public decimal NewDepreciableBaseAmountAfter { get; set; }

            public int? NewUsefulLifeMonthsOverride { get; set; }

            public Guid? RegeneratedScheduleId { get; set; }

            // ── Section 9: Workflow Fields ─────────────────────────────
            public DateTime? SubmittedOn { get; set; }
            public DateTime? ApprovedOn { get; set; }
            public Guid? ApprovedBy { get; set; }

            [StringLength(500)]
            public string? ApprovalNotes { get; set; }

            [StringLength(500)]
            public string? ReversalReason { get; set; }

            // ── Section 10: System Audit Fields ────────────────────────
            public DateTime CreatedAt { get; set; }
            public Guid CreatedBy { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public Guid? UpdatedBy { get; set; }
            public byte[]? RowVersion { get; set; }
            public bool IsDeleted { get; set; } = false;
        }
    }
}
