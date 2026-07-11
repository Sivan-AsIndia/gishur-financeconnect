using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    /// <summary>
    /// Model #35: VendorAccount – The AP subledger balance record for a Vendor.
    /// System-maintained AP ledger summary showing: how much we owe the vendor,
    /// advance payments made (unallocated), block/freeze status, and activity metadata.
    /// This is NOT a data-entry table - it's a system-ledger summary.
    /// </summary>
    public class VendorAccountViewModel
    {
        #region Section 1: Identity & Scope (Hidden/Read-only)

        /// <summary>PK - VendorAccountId, hidden in UI</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Tenant identifier - hidden in UI</summary>
        public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

        /// <summary>FK → Company - hidden, required</summary>
        [Required(ErrorMessage = "Company is required")]
        public Guid CompanyId { get; set; }
        public string? CompanyName { get; set; }

        /// <summary>FK → Vendor - hidden (shown as Vendor name on screen), required</summary>
        [Required(ErrorMessage = "Vendor is required")]
        public Guid VendorId { get; set; }
        public string? VendorCode { get; set; }
        public string? VendorName { get; set; }

        /// <summary>
        /// FK → CurrencyMaster - read-only badge, required.
        /// VendorAccount is maintained per currency. Default: Vendor.DefaultCurrencyId.
        /// If multi-currency AP enabled, separate VendorAccount per currency.
        /// </summary>
        [Required(ErrorMessage = "Currency is required")]
        public Guid CurrencyId { get; set; }
        public string? CurrencyCode { get; set; }
        public string? CurrencyName { get; set; }

        #endregion

        #region Section 2: Balance Summary (System-Controlled, Read-only)

        /// <summary>
        /// Payable Outstanding - amount we currently owe vendor (open liability).
        /// Type: decimal(18,2), Rule: >= 0
        /// </summary>
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Outstanding Payable Amount must be >= 0")]
        public decimal OutstandingPayableAmount { get; set; } = 0;

        /// <summary>
        /// Advance / Unapplied Payment - we already paid vendor but not allocated against bills yet.
        /// Type: decimal(18,2), Rule: >= 0
        /// </summary>
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Advance Paid Amount must be >= 0")]
        public decimal AdvancePaidAmount { get; set; } = 0;

        /// <summary>
        /// Net Exposure (Derived, display-only) = OutstandingPayableAmount - AdvancePaidAmount.
        /// Can be negative if advance exceeds payable; display as "Advance surplus".
        /// </summary>
        public decimal NetExposureAmount => OutstandingPayableAmount - AdvancePaidAmount;

        #endregion

        #region Section 3: Cumulative Totals (Optional but useful, Read-only)

        /// <summary>Total Bills Posted Amount - lifetime billed total (audit/analytics)</summary>
        [Range(0, double.MaxValue)]
        public decimal TotalBillsPostedAmount { get; set; } = 0;

        /// <summary>Total Payments Posted Amount - lifetime payments made</summary>
        [Range(0, double.MaxValue)]
        public decimal TotalPaymentsPostedAmount { get; set; } = 0;

        /// <summary>Total Credit Notes Posted Amount</summary>
        [Range(0, double.MaxValue)]
        public decimal TotalCreditNotesPostedAmount { get; set; } = 0;

        /// <summary>Total Debit Notes Posted Amount</summary>
        [Range(0, double.MaxValue)]
        public decimal TotalDebitNotesPostedAmount { get; set; } = 0;

        /// <summary>Total Adjustments Posted Amount</summary>
        [Range(0, double.MaxValue)]
        public decimal TotalAdjustmentsPostedAmount { get; set; } = 0;

        #endregion

        #region Section 4: Controls & Blocks (Restricted)

        /// <summary>
        /// Account Status - Active/Frozen/Closed (rare, for legacy vendors).
        /// Frozen blocks posting and payments as per policy.
        /// </summary>
        [Required(ErrorMessage = "Account Status is required")]
        public string AccountStatus { get; set; } = VendorAccountStatuses.Active;

        /// <summary>
        /// Payment Block - if true, posting VendorPayment is blocked.
        /// Toggle (controller only), Default: false
        /// </summary>
        public bool IsPaymentBlocked { get; set; } = false;

        /// <summary>
        /// Payment Block Reason - required if IsPaymentBlocked = true.
        /// Max 250 chars, Textarea.
        /// </summary>
        [StringLength(250, ErrorMessage = "Payment Block Reason cannot exceed 250 characters")]
        public string? PaymentBlockReason { get; set; }

        /// <summary>
        /// Posting Block - if true, posting VendorBill/CN/DN/Adjustment is blocked.
        /// Toggle (controller only), Default: false
        /// </summary>
        public bool IsPostingBlocked { get; set; } = false;

        /// <summary>
        /// Posting Block Reason - required if IsPostingBlocked = true.
        /// Max 250 chars, Textarea.
        /// </summary>
        [StringLength(250, ErrorMessage = "Posting Block Reason cannot exceed 250 characters")]
        public string? PostingBlockReason { get; set; }

        /// <summary>Blocked On - timestamp when account was blocked</summary>
        public DateTime? BlockedOn { get; set; }

        /// <summary>FK → User - who blocked the account</summary>
        public Guid? BlockedByUserId { get; set; }
        public string? BlockedByUserName { get; set; }

        #endregion

        #region Section 5: Activity Summary (System Derived, Read-only)

        /// <summary>
        /// Last Transaction On - updated on any posted AP document affecting this vendor/currency.
        /// </summary>
        public DateTime? LastTransactionOn { get; set; }

        /// <summary>Last Bill Posted On</summary>
        public DateTime? LastBillPostedOn { get; set; }

        /// <summary>Last Payment Posted On</summary>
        public DateTime? LastPaymentPostedOn { get; set; }

        /// <summary>
        /// Last Document Reference - e.g., "BILL-000245", "PAY-000087".
        /// Max 50 chars.
        /// </summary>
        [StringLength(50, ErrorMessage = "Last Document Reference cannot exceed 50 characters")]
        public string? LastDocumentReference { get; set; }

        #endregion

        #region Section 6: Reconciliation Info (For Control & Audit, Read-only)

        /// <summary>Last Reconciled On - nullable datetime</summary>
        public DateTime? LastReconciledOn { get; set; }

        /// <summary>FK → User - who performed last reconciliation (nullable)</summary>
        public Guid? LastReconciledByUserId { get; set; }
        public string? LastReconciledByUserName { get; set; }

        /// <summary>
        /// FK → GLAccountMaster - AP Control Account Snapshot (optional).
        /// Which control account this vendor payable rolls into (for audit).
        /// </summary>
        public Guid? APControlAccountIdSnapshot { get; set; }
        public string? APControlAccountCode { get; set; }
        public string? APControlAccountName { get; set; }

        #endregion

        #region Section 7: System Audit Fields (Hidden)

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        /// <summary>RowVersion - mandatory concurrency token</summary>
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        /// <summary>IsDeleted - should be false always once used</summary>
        public bool IsDeleted { get; set; } = false;

        #endregion

        #region Helper Properties

        /// <summary>Check if account has outstanding payable</summary>
        public bool HasOutstandingPayable => OutstandingPayableAmount > 0;

        /// <summary>Check if account has advance surplus</summary>
        public bool HasAdvanceSurplus => NetExposureAmount < 0;

        /// <summary>Get absolute advance surplus amount (when negative exposure)</summary>
        public decimal AdvanceSurplusAmount => HasAdvanceSurplus ? Math.Abs(NetExposureAmount) : 0;

        /// <summary>Check if any block is active</summary>
        public bool IsBlocked => IsPaymentBlocked || IsPostingBlocked || AccountStatus == VendorAccountStatuses.Frozen;

        /// <summary>Display text for net exposure</summary>
        public string NetExposureDisplay => HasAdvanceSurplus
            ? $"Advance Surplus: {AdvanceSurplusAmount:N2}"
            : NetExposureAmount.ToString("N2");

        #endregion
    }

    #region VendorAccount Enums and Static Classes

    /// <summary>Vendor Account Status values</summary>
    public static class VendorAccountStatuses
    {
        public const string Active = "Active";
        public const string Frozen = "Frozen";
        public const string Closed = "Closed";

        public static readonly string[] All = new[] { Active, Frozen, Closed };

        public static string GetDisplayName(string status) => status switch
        {
            Active => "Active",
            Frozen => "Frozen",
            Closed => "Closed (Legacy)",
            _ => status
        };
    }

    #endregion
}
