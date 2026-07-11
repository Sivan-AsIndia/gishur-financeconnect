using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public class RevenueViewModel
    {
        // ── Enums ──────────────────────────────────────────────────────────────

        public enum RevenueStatus
        {
            Draft = 1,
            Confirmed = 2,
            PendingRecognition = 3,
            PartiallyRecognized = 4,
            FullyRecognized = 5,
            Deferred = 6,
            Cancelled = 7,
            Closed = 8
        }

        public enum RevenueSourceDocType
        {
            CustomerInvoice = 1,
            Contract = 2,
            Subscription = 3,
            Milestone = 4,
            ProjectDeliverable = 5,
            ManualRevenueEvent = 6,
            Other = 7
        }

        public enum RevenueType
        {
            OneTime = 1,
            Recurring = 2,
            Subscription = 3,
            MilestoneBased = 4,
            ServiceBased = 5,
            ProjectBased = 6,
            AdvanceReceiptBased = 7
        }

        public enum RevenueNature
        {
            EarnedImmediately = 1,
            EarnedOverTime = 2,
            EarnedOnMilestone = 3,
            UnearnedAdvance = 4,
            Mixed = 5
        }

        public enum RecognitionMethod
        {
            Immediate = 1,
            Scheduled = 2,
            MilestoneTriggered = 3,
            ManualApprovalRequired = 4,
            DeferredThenRelease = 5
        }

        public enum RecognitionFrequency
        {
            Daily = 1,
            Monthly = 2,
            Quarterly = 3,
            Custom = 4
        }

        public enum RecognitionStatus
        {
            NotStarted = 1,
            Ready = 2,
            InProgress = 3,
            PartiallyRecognized = 4,
            FullyRecognized = 5,
            Deferred = 6,
            OnHold = 7
        }

        public enum BillingStatus
        {
            NotBilled = 1,
            PartiallyBilled = 2,
            FullyBilled = 3,
            AdvanceBilled = 4
        }

        public enum CollectionStatus
        {
            NotCollected = 1,
            PartiallyCollected = 2,
            FullyCollected = 3,
            AdvanceCollected = 4
        }

        // ── Model ──────────────────────────────────────────────────────────────

        public class Revenue
        {
            // ─── Section 1: Core Identity ────────────────────────────────────
            public Guid RevenueId { get; set; } = Guid.NewGuid();
            public Guid TenantId { get; set; }
            public Guid CompanyId { get; set; }

            [Required(ErrorMessage = "Revenue Code is required")]
            [MaxLength(30, ErrorMessage = "Revenue Code cannot exceed 30 characters")]
            public string RevenueCode { get; set; } = string.Empty;

            [Required(ErrorMessage = "Revenue Name is required")]
            [MaxLength(200, ErrorMessage = "Revenue Name cannot exceed 200 characters")]
            public string RevenueName { get; set; } = string.Empty;

            [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
            public string? Description { get; set; }

            [Required(ErrorMessage = "Revenue Status is required")]
            public RevenueStatus Status { get; set; } = RevenueStatus.Draft;

            // ─── Section 2: Customer / Source Context ─────────────────────────
            [Required(ErrorMessage = "Customer is required")]
            public Guid CustomerId { get; set; }

            public string? CustomerCodeSnapshot { get; set; }

            [MaxLength(200)]
            public string? CustomerNameSnapshot { get; set; }

            [Required(ErrorMessage = "Source Document Type is required")]
            public RevenueSourceDocType RevenueSourceDocType { get; set; } = RevenueSourceDocType.ManualRevenueEvent;

            public Guid? SourceDocumentId { get; set; }

            [MaxLength(50)]
            public string? SourceDocumentNumber { get; set; }

            public Guid? ContractId { get; set; }
            /// <summary>Text reference for ContractId — used in UI since no Contract master screen exists in this demo.</summary>
            [MaxLength(50)]
            public string? ContractReference { get; set; }

            public Guid? SubscriptionId { get; set; }
            /// <summary>Text reference for SubscriptionId — used in UI since no Subscription master screen exists in this demo.</summary>
            [MaxLength(50)]
            public string? SubscriptionReference { get; set; }

            public Guid? ProjectId { get; set; }
            /// <summary>Text reference for ProjectId — used in UI since no Project master screen exists in this demo.</summary>
            [MaxLength(50)]
            public string? ProjectReference { get; set; }

            [MaxLength(100)]
            public string? MilestoneReference { get; set; }

            // ─── Section 3: Revenue Classification ───────────────────────────
            [Required(ErrorMessage = "Revenue Type is required")]
            public RevenueType RevenueType { get; set; } = RevenueType.OneTime;

            [Required(ErrorMessage = "Revenue Category is required")]
            [MaxLength(50)]
            public string RevenueCategoryCode { get; set; } = string.Empty;

            public Guid? GLAccountId { get; set; }
            public string? GLAccountName { get; set; }

            [Required(ErrorMessage = "Revenue Nature is required")]
            public RevenueNature RevenueNature { get; set; } = RevenueNature.EarnedImmediately;

            [Required(ErrorMessage = "Revenue Event Date is required")]
            public DateTime? BusinessEventDate { get; set; }

            public DateTime? OperationalPeriodFrom { get; set; }
            public DateTime? OperationalPeriodTo { get; set; }

            // ─── Section 4: Revenue Amounts ───────────────────────────────────
            [Required(ErrorMessage = "Gross Revenue Amount is required")]
            [Range(0, double.MaxValue, ErrorMessage = "Gross Revenue Amount must be >= 0")]
            public decimal GrossRevenueAmount { get; set; }

            public decimal? TaxExclusiveRevenueAmount { get; set; }
            public decimal RecognizedAmountToDate { get; set; }
            public decimal DeferredAmountToDate { get; set; }

            public decimal UnrecognizedAmount =>
                GrossRevenueAmount - RecognizedAmountToDate;

            public decimal? AdjustmentAmount { get; set; }

            [Required(ErrorMessage = "Currency is required")]
            public Guid CurrencyId { get; set; }

            public Guid? ExchangeRateId { get; set; }

            // ─── Section 5: Recognition Readiness ────────────────────────────
            [Required(ErrorMessage = "Recognition Method is required")]
            public RecognitionMethod RecognitionMethod { get; set; } = RecognitionMethod.Immediate;

            public DateTime? RecognitionStartDate { get; set; }
            public DateTime? RecognitionEndDate { get; set; }
            public RecognitionFrequency? RecognitionFrequency { get; set; }

            [Required(ErrorMessage = "Recognition Status is required")]
            public RecognitionStatus RecognitionStatus { get; set; } = RecognitionStatus.NotStarted;

            public bool IsRecognitionRequired { get; set; } = true;
            public bool IsDeferredRevenueRequired { get; set; } = false;

            public Guid? DeferredRevenueId { get; set; }
            /// <summary>Text reference for DeferredRevenueId — used in UI since DeferredRevenue master (#88) is a separate module.</summary>
            [MaxLength(50)]
            public string? DeferredRevenueReference { get; set; }

            [MaxLength(50)]
            public string? RevenueRecognitionTemplateCode { get; set; }

            // ─── Section 6: Billing / Collection References ───────────────────
            [Required(ErrorMessage = "Billing Status is required")]
            public BillingStatus BillingStatus { get; set; } = BillingStatus.NotBilled;

            [Required(ErrorMessage = "Collection Status is required")]
            public CollectionStatus CollectionStatus { get; set; } = CollectionStatus.NotCollected;

            public Guid? InvoiceId { get; set; }
            /// <summary>Text reference for InvoiceId — used in UI for FK display alongside InvoiceNumberSnapshot.</summary>
            [MaxLength(50)]
            public string? InvoiceReference { get; set; }

            [MaxLength(50)]
            public string? InvoiceNumberSnapshot { get; set; }

            public DateTime? BillingDate { get; set; }

            [MaxLength(200)]
            public string? CollectionReferenceText { get; set; }

            public bool IsAdvanceReceipt { get; set; } = false;

            // ─── Section 7: Dimensions & Ownership ───────────────────────────
            public Guid? BranchId { get; set; }
            public string? BranchName { get; set; }

            public Guid? DepartmentId { get; set; }
            public string? DepartmentName { get; set; }

            public Guid? CostCenterId { get; set; }
            public string? CostCenterName { get; set; }

            public Guid? RevenueOwnerUserId { get; set; }
            /// <summary>Text login/name of the revenue owner — used in UI since no User master screen exists in this demo.</summary>
            [MaxLength(100)]
            public string? RevenueOwnerUserText { get; set; }

            [MaxLength(50)]
            public string? BusinessUnitCode { get; set; }

            public string? DimensionScopeJson { get; set; }

            // ─── Section 8: Workflow & Governance ────────────────────────────
            [Required(ErrorMessage = "Prepared By is required")]
            [MaxLength(100)]
            public string PreparedByUserId { get; set; } = string.Empty;

            [MaxLength(100)]
            public string? ReviewedByUserId { get; set; }

            [MaxLength(100)]
            public string? ApprovedByUserId { get; set; }

            public DateTime? PreparedOn { get; set; }
            public DateTime? ReviewedOn { get; set; }
            public DateTime? ApprovedOn { get; set; }

            public bool IsLocked { get; set; } = false;
            public DateTime? LockedOn { get; set; }
            public Guid? LockedBy { get; set; }

            [MaxLength(500)]
            public string? CancellationReason { get; set; }

            // ─── Section 9: Notes & Supporting Evidence ───────────────────────
            [MaxLength(1500)]
            public string? RevenueAssumptionText { get; set; }

            [MaxLength(1500)]
            public string? Notes { get; set; }

            public int AttachmentCount { get; set; }

            // ─── Section 10: System Audit Fields (Hidden) ─────────────────────
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public Guid? CreatedBy { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public Guid? UpdatedBy { get; set; }
            public bool IsDeleted { get; set; }
        }
    }
}
