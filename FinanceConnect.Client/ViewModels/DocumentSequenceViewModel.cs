namespace FinanceConnect.Client.ViewModels
{
    public enum BranchScopeMode
    {
        CompanyWide,
        BranchSpecific
    }


    public class DocumentSequenceModel
    {
        // Identity
        public Guid DocumentSequenceId { get; set; } = Guid.NewGuid();
        public Guid TenantId { get; set; }
        public Guid? CompanyId { get; set; }

        // Scope
        public BranchScopeMode BranchScopeMode { get; set; } = BranchScopeMode.CompanyWide;
        public Guid? BranchId { get; set; }

        // Parent
        public Guid DocumentNumberSeriesId { get; set; }
        public string SeriesCode { get; set; } = string.Empty;

        // Reset
        public ResetFrequency ResetFrequency { get; set; }
        public string ResetKey { get; set; } = string.Empty;
        public Guid? AccountingPeriodId { get; set; }

        // Counter
        public long CurrentValue { get; set; }
        public int IncrementBy { get; set; } = 1;
        public long? MinValue { get; set; }
        public long? MaxValue { get; set; }

        // Status
        public bool IsActive { get; set; } = true;
        public bool IsExhausted { get; set; } = false;
        public bool IsLocked { get; set; } = false;

        // Tracking
        public DateTime? LastIssuedAt { get; set; }
        public string? LastIssuedBy { get; set; }
        public string? LastIssuedToReference { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; } = Guid.NewGuid().ToByteArray();
    }
}
