using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public class TaxAuditTrailViewModel
    {
        public class TaxAuditTrailModel
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

            [Required]
            public Guid CompanyId { get; set; }
            public string? CompanyName { get; set; }

            [Required]
            [StringLength(40)]
            public string AuditEventNumber { get; set; } = string.Empty;

            [Required(ErrorMessage = "Event Timestamp is required")]
            public DateTime EventTimestamp { get; set; } = DateTime.UtcNow;

            public DateTime EventDate { get; set; } = DateTime.Today;

            // ── Entity Reference ──
            [Required(ErrorMessage = "Entity Type is required")]
            public string EntityType { get; set; } = string.Empty;

            [Required(ErrorMessage = "Entity Id is required")]
            public Guid EntityId { get; set; }

            [StringLength(50)]
            public string? EntityNumberSnapshot { get; set; }

            [StringLength(200)]
            public string? EntityDisplayNameSnapshot { get; set; }

            public string? ParentEntityType { get; set; }
            public Guid? ParentEntityId { get; set; }

            // ── Event Classification ──
            [Required(ErrorMessage = "Event Category is required")]
            public string EventCategory { get; set; } = string.Empty;

            [Required(ErrorMessage = "Event Type is required")]
            public string EventType { get; set; } = string.Empty;

            [Required(ErrorMessage = "Event Severity is required")]
            public string EventSeverity { get; set; } = "Info";

            [Required(ErrorMessage = "Tax Type Scope is required")]
            public string TaxTypeScope { get; set; } = "NotApplicable";

            // ── Actor / Initiator ──
            [Required(ErrorMessage = "Actor Type is required")]
            public string ActorType { get; set; } = "System";

            public Guid? ActorUserId { get; set; }

            [Required]
            [StringLength(150)]
            public string ActorNameSnapshot { get; set; } = string.Empty;

            [StringLength(100)]
            public string? ActorRoleSnapshot { get; set; }

            [StringLength(50)]
            public string? ActorIpAddress { get; set; }

            [StringLength(300)]
            public string? ActorDeviceInfo { get; set; }

            [Required]
            [StringLength(100)]
            public string CorrelationId { get; set; } = string.Empty;

            [StringLength(100)]
            public string? RequestId { get; set; }

            // ── Business Context Snapshot ──
            [StringLength(20)]
            public string? TaxPeriodKey { get; set; }

            public Guid? AccountingPeriodId { get; set; }
            public Guid? BranchId { get; set; }
            public string? BranchName { get; set; }

            public string? PartyType { get; set; }
            public Guid? PartyId { get; set; }

            [StringLength(200)]
            public string? PartyNameSnapshot { get; set; }

            [StringLength(20)]
            public string? SectionCodeSnapshot { get; set; }

            [StringLength(30)]
            public string? TaxCodeSnapshot { get; set; }

            public Guid? ReturnRunId { get; set; }
            public Guid? SettlementId { get; set; }
            public Guid? JournalEntryId { get; set; }

            // ── Before / After Snapshot ──
            public string? ChangedFieldListJson { get; set; }
            public string? BeforeStateJson { get; set; }
            public string? AfterStateJson { get; set; }

            [StringLength(128)]
            public string? BeforeHash { get; set; }

            [StringLength(128)]
            public string? AfterHash { get; set; }

            public bool IsSensitiveChange { get; set; } = false;

            public string? ReasonCode { get; set; }

            [StringLength(1000)]
            public string? ReasonText { get; set; }

            // ── Validation / Exception / Security ──
            [Required(ErrorMessage = "Validation Status is required")]
            public string ValidationStatus { get; set; } = "NotApplicable";

            [StringLength(1000)]
            public string? ValidationMessage { get; set; }

            public bool SecurityEventFlag { get; set; } = false;
            public string? SecurityEventType { get; set; }
            public string? ExceptionDetailsJson { get; set; }

            // ── Evidence & Links ──
            public string? AttachmentReferenceJson { get; set; }

            [StringLength(200)]
            public string? SupportingDocumentReference { get; set; }

            [StringLength(100)]
            public string? GovernmentReferenceNumber { get; set; }

            [StringLength(100)]
            public string? ExternalSystemReference { get; set; }

            [StringLength(300)]
            public string? SourceUrlOrRoute { get; set; }

            // ── Immutability & Retention ──
            public bool IsImmutable { get; set; } = true;

            [Required(ErrorMessage = "Retention Category is required")]
            public string RetentionCategory { get; set; } = "Statutory";

            [Required(ErrorMessage = "Archive Status is required")]
            public string ArchiveStatus { get; set; } = "Active";

            [StringLength(128)]
            public string? HashChainReference { get; set; }

            // ── System Audit Fields ──
            public DateTime CreatedAt { get; set; } = DateTime.Now;
            public string? CreatedBy { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public string? UpdatedBy { get; set; }
            public byte[]? RowVersion { get; set; }
            public bool IsDeleted { get; set; } = false;
        }
    }

    // ── Enums / Constants ──
    public static class AuditEntityTypeEnum
    {
        public const string TaxCode = "TaxCode";
        public const string TaxRateVersion = "TaxRateVersion";
        public const string TaxCategoryMapping = "TaxCategoryMapping";
        public const string TaxTransaction = "TaxTransaction";
        public const string TaxTransactionLine = "TaxTransactionLine";
        public const string TDSConfig = "TDSConfig";
        public const string TDSDeductionEntry = "TDSDeductionEntry";
        public const string TCSConfig = "TCSConfig";
        public const string TaxSettlement = "TaxSettlement";
        public const string GSTReturnRun = "GSTReturnRun";
        public const string Other = "Other";
    }

    public static class AuditEventCategoryEnum
    {
        public const string MasterData = "MasterData";
        public const string Calculation = "Calculation";
        public const string Posting = "Posting";
        public const string Settlement = "Settlement";
        public const string ReturnPreparation = "ReturnPreparation";
        public const string Filing = "Filing";
        public const string Security = "Security";
        public const string Reconciliation = "Reconciliation";
        public const string Workflow = "Workflow";
    }

    public static class AuditEventTypeEnum
    {
        public const string Created = "Created";
        public const string Updated = "Updated";
        public const string Activated = "Activated";
        public const string Inactivated = "Inactivated";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
        public const string Locked = "Locked";
        public const string Unlocked = "Unlocked";
        public const string Generated = "Generated";
        public const string Posted = "Posted";
        public const string Reversed = "Reversed";
        public const string Settled = "Settled";
        public const string Finalized = "Finalized";
        public const string Filed = "Filed";
        public const string Reopened = "Reopened";
        public const string Excluded = "Excluded";
        public const string Included = "Included";
        public const string OverrideApplied = "OverrideApplied";
        public const string ValidationFailed = "ValidationFailed";
        public const string AccessDenied = "AccessDenied";
        public const string ViewedSensitiveRecord = "ViewedSensitiveRecord";
        public const string Other = "Other";
    }

    public static class AuditEventSeverityEnum
    {
        public const string Info = "Info";
        public const string Warning = "Warning";
        public const string High = "High";
        public const string Critical = "Critical";
    }
}
