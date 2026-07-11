using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels;

/// <summary>
/// CustomerDebitNoteLine – One row inside a CustomerDebitNote.
/// Contains additional charges, corrections, or adjustments.
/// </summary>
public class CustomerDebitNoteLineViewModel
{
    // Section 1: Core Line Fields

    /// <summary>PK - hidden in UI</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>FK → CustomerDebitNote - hidden</summary>
    [Required(ErrorMessage = "Debit Note is required")]
    public Guid CustomerDebitNoteId { get; set; }

    /// <summary>Line Number - auto-generated sequentially (10, 20, 30)</summary>
    [Required(ErrorMessage = "Line Number is required")]
    public int LineNumber { get; set; }

    /// <summary>Line Type - Item/Service/Charge/Manual</summary>
    [Required(ErrorMessage = "Line Type is required")]
    public string LineType { get; set; } = "";

    /// <summary>FK → ItemMaster/ServiceMaster - Search dropdown (typeahead)</summary>
    public Guid? ItemId { get; set; }
    public string? ItemCode { get; set; }
    public string? ItemName { get; set; }

    /// <summary>Description - max 500 chars, required</summary>
    [Required(ErrorMessage = "Description is required")]
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string Description { get; set; } = string.Empty;

    /// <summary>FK → UomMaster - optional</summary>
    public Guid? UomId { get; set; }
    public string? UomCode { get; set; }
    public string? UomName { get; set; }

    /// <summary>Quantity - must be > 0</summary>
    [Required(ErrorMessage = "Quantity is required")]
    [Range(0.0001, double.MaxValue, ErrorMessage = "Quantity must be greater than zero")]
    public decimal Quantity { get; set; } = 1;

    /// <summary>Unit Price / Rate - must be >= 0</summary>
    [Required(ErrorMessage = "Unit Price is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Unit Price must be zero or greater")]
    public decimal UnitPrice { get; set; } = 0;

    /// <summary>Subtotal = Qty × UnitPrice (derived, read-only)</summary>
    public decimal LineSubTotalAmount => Math.Round(Quantity * UnitPrice, 2);

    // Section 2: Discount Handling

    /// <summary>Discount Percent - 0-100</summary>
    [Range(0, 100, ErrorMessage = "Discount Percent must be between 0 and 100")]
    public decimal DiscountPercent { get; set; } = 0;

    /// <summary>Discount Amount - >= 0 and <= LineSubTotalAmount</summary>
    [Range(0, double.MaxValue, ErrorMessage = "Discount Amount must be zero or greater")]
    public decimal DiscountAmount { get; set; } = 0;

    /// <summary>Discount Reason - optional</summary>
    [StringLength(250, ErrorMessage = "Discount Reason cannot exceed 250 characters")]
    public string? DiscountReason { get; set; }

    /// <summary>Taxable Value = LineSubTotalAmount - DiscountAmount (derived, read-only)</summary>
    public decimal TaxableAmount => LineSubTotalAmount - DiscountAmount;

    // Section 3: Tax Breakdown

    /// <summary>FK → TaxCodeMaster - dropdown/search</summary>
    public Guid? TaxCodeId { get; set; }
    public string? TaxCodeCode { get; set; }
    public string? TaxCodeName { get; set; }

    /// <summary>Tax Rate % - snapshot from tax code</summary>
    [Range(0, 100, ErrorMessage = "Tax Rate must be between 0 and 100")]
    public decimal TaxRatePercent { get; set; } = 0;

    /// <summary>Tax Amount = TaxableAmount × TaxRate%</summary>
    public decimal TaxAmount { get; set; } = 0;

    /// <summary>Tax Component JSON - store CGST/SGST/IGST split for reporting</summary>
    public string? TaxComponentJson { get; set; }

    /// <summary>Is Tax Inclusive - pricing includes tax</summary>
    public bool IsTaxInclusive { get; set; } = false;

    // Section 4: Line Totals

    /// <summary>Line Total = TaxableAmount + TaxAmount (derived, read-only)</summary>
    public decimal LineTotalAmount => TaxableAmount + TaxAmount;

    /// <summary>Rounding Adjustment - optional</summary>
    public decimal RoundingAdjustmentAmount { get; set; } = 0;

    // Section 5: Posting Classification (Accounting Mapping)

    /// <summary>FK → GLAccountMaster - Revenue Account, must be Active + Postable</summary>
    [Required(ErrorMessage = "Revenue Account is required")]
    public Guid RevenueAccountId { get; set; }
    public string? RevenueAccountCode { get; set; }
    public string? RevenueAccountName { get; set; }

    /// <summary>FK → GLAccountMaster - Tax Account derived from tax configuration</summary>
    public Guid? TaxAccountId { get; set; }
    public string? TaxAccountCode { get; set; }
    public string? TaxAccountName { get; set; }

    /// <summary>Is System Generated - line inserted by automation</summary>
    public bool IsSystemGenerated { get; set; } = false;

    // Section 6: Optional Future-Ready Dimensions

    /// <summary>FK → CostCenter - optional</summary>
    public Guid? CostCenterId { get; set; }
    public string? CostCenterCode { get; set; }
    public string? CostCenterName { get; set; }

    /// <summary>FK → Project/Job - optional</summary>
    public Guid? ProjectId { get; set; }
    public string? ProjectCode { get; set; }
    public string? ProjectName { get; set; }

    /// <summary>FK → Department - optional</summary>
    public Guid? DepartmentId { get; set; }
    public string? DepartmentCode { get; set; }
    public string? DepartmentName { get; set; }

    /// <summary>Reference - PO line ref, ticket ref, etc.</summary>
    [StringLength(100, ErrorMessage = "Reference cannot exceed 100 characters")]
    public string? ReferenceText { get; set; }

    /// <summary>HSN Code - GST classification (future compliance)</summary>
    [StringLength(20, ErrorMessage = "HSN Code cannot exceed 20 characters")]
    public string? HSNCode { get; set; }

    /// <summary>SAC Code - GST classification for services</summary>
    [StringLength(20, ErrorMessage = "SAC Code cannot exceed 20 characters")]
    public string? SACCode { get; set; }

    // Section 7: System Audit Fields

    public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    public bool IsDeleted { get; set; } = false;

    // Helper Methods

    /// <summary>Calculate discount amount from percent</summary>
    public void CalculateDiscountFromPercent()
    {
        if (LineSubTotalAmount > 0)
        {
            DiscountAmount = Math.Round(LineSubTotalAmount * DiscountPercent / 100, 2);
        }
    }

    /// <summary>Calculate discount percent from amount</summary>
    public void CalculateDiscountPercentFromAmount()
    {
        if (LineSubTotalAmount > 0)
        {
            DiscountPercent = Math.Round(DiscountAmount / LineSubTotalAmount * 100, 2);
        }
    }

    /// <summary>Calculate tax amount from tax rate</summary>
    public void CalculateTaxAmount()
    {
        TaxAmount = Math.Round(TaxableAmount * TaxRatePercent / 100, 2);
    }

    /// <summary>Recalculate all line amounts</summary>
    public void RecalculateAmounts()
    {
        CalculateDiscountFromPercent();
        CalculateTaxAmount();
    }
}
