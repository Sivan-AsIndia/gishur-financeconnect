using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceConnect.Client.ViewModels
{

    public enum TransactionLineType
    {
        PRINCIPAL,
        TAX,
        DISCOUNT,
        CHARGE,
        ROUNDING,
        FX_ADJUSTMENT,
        WITHHOLDING,
        OTHER
    }
    public enum TaxComponentType
    {
        CGST,
        SGST,
        IGST,
        CESS,
        VAT,
        SERVICE_TAX
    }
    public enum TaxCode
    {
        GST18,
        GST5,
        IGST18,
    }

    public enum PostingCategories
    {
        EXPENSE,
        REVENUE,
        BANK,
        AR_CONTROL,
        AP_CONTROL,
        TAX_INPUT,
        TAX_OUTPUT,
        ROUNDING_GAIN_LOSS,
        DISCOUNT_ALLOWED,
        DISCOUNT_RECEIVED,
        FX_GAIN_LOSS,
        CLEARING
    }
    public class TransactionLineModel
    {
        // ================= CORE =================
        public Guid TransactionLineId { get; set; }

        [Required]
        public Guid FinancialTransactionId { get; set; }

        [Required]
        public int LineNumber { get; set; }

        // ================= CLASSIFICATION =================
        [Required(ErrorMessage = "Line type is required.")]
        public TransactionLineType? LineType { get; set; }

        [Required(ErrorMessage = "Posting category is required.")]
        public PostingCategory? PostingCategory { get; set; }

        [MaxLength(1000)]
        public string? LineNarration { get; set; }

        // ================= AMOUNT =================
        public decimal Quantity { get; set; } = 1;

        [Required(ErrorMessage = "Unit Rate is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Unit Rate must be zero or greater")]
        public decimal UnitRate { get; set; } = 0;

        [Required(ErrorMessage = "Line amount is required.")]
        [Range(typeof(decimal), "0.01", "999999999999")]
        public decimal LineAmount { get; set; }

        [Required]
        public decimal BaseAmount { get; set; }

        // ================= TAX =================
        public bool IsTaxLine { get; set; }
        public TaxCode? TaxCodeId { get; set; }
        public TaxComponentType? TaxComponentType { get; set; }

        // ================= PARTY =================
        public string? PartyType { get; set; }
        public Guid? PartyId { get; set; }

        // ================= BRANCH =================
        [Required]
        public Guid? BranchId { get; set; }

        // ================= SYSTEM =================
        public bool IsSystemGenerated { get; set; }
        public bool IsAdjustment { get; set; }
        public string? SourceLineRefId { get; set; }

        // ================= AUDIT =================
        public Guid TenantId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = "system";
        public DateTime? UpdatedAt { get; set; }
    }
}
