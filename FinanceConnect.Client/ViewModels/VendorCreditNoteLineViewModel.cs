using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    #region Model #39.1: VendorCreditNoteLine

    /// <summary>
    /// VendorCreditNoteLine – Line items for VendorCreditNote (Model #39).
    /// Represents individual line entries for expense/asset reversal accounts, tax codes, and amounts.
    /// </summary>
    public class VendorCreditNoteLineModel
    {
        // Section 1: Core Line Identity

        /// <summary>PK - hidden in UI</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>FK → VendorCreditNote</summary>
        [Required(ErrorMessage = "Credit Note is required")]
        public Guid VendorCreditNoteId { get; set; }

        /// <summary>Line Number - auto-generated (10, 20, 30...)</summary>
        public int LineNumber { get; set; }

        /// <summary>Line Type - Item/Service/Manual</summary>
        [Required(ErrorMessage = "Line Type is required")]
        public string LineType { get; set; } = "";

        // Section 2: Item/Service Reference (Optional)

        /// <summary>FK → ItemMaster (optional, for inventory items)</summary>
        public Guid? ItemId { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }

        /// <summary>Description - textbox, required, max 500 chars</summary>
        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;

        /// <summary>FK → UomMaster (optional)</summary>
        public Guid? UomId { get; set; }
        public string? UomCode { get; set; }
        public string? UomName { get; set; }

        // Section 3: Quantity & Pricing

        /// <summary>Quantity - numeric, required, must be > 0</summary>
        [Required(ErrorMessage = "Quantity is required")]
        [Range(0.0001, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public decimal Quantity { get; set; } = 1;

        /// <summary>Unit Price - numeric</summary>
        [Range(0, double.MaxValue, ErrorMessage = "Unit Price cannot be negative")]
        public decimal UnitPrice { get; set; } = 0;

        /// <summary>Discount Percent - numeric, 0-100</summary>
        [Range(0, 100, ErrorMessage = "Discount Percent must be between 0 and 100")]
        public decimal DiscountPercent { get; set; } = 0;

        /// <summary>Discount Amount - calculated or manual</summary>
        public decimal DiscountAmount { get; set; } = 0;

        /// <summary>Discount Reason - optional</summary>
        [StringLength(250, ErrorMessage = "Discount Reason cannot exceed 250 characters")]
        public string? DiscountReason { get; set; }

        // Section 4: Tax

        /// <summary>FK → TaxCodeMaster (nullable)</summary>
        public Guid? TaxCodeId { get; set; }
        public string? TaxCodeCode { get; set; }
        public string? TaxCodeName { get; set; }

        /// <summary>Tax Rate Percent</summary>
        [Range(0, 100, ErrorMessage = "Tax Rate Percent must be between 0 and 100")]
        public decimal TaxRatePercent { get; set; } = 0;

        /// <summary>Tax Amount - calculated</summary>
        public decimal TaxAmount { get; set; } = 0;

        // Section 5: Account Mapping

        /// <summary>FK → GLAccountMaster - Reversal Account (expense/asset) - dropdown, required</summary>
        [Required(ErrorMessage = "Reversal Account is required")]
        public Guid ReversalAccountId { get; set; }
        public string? ReversalAccountCode { get; set; }
        public string? ReversalAccountName { get; set; }

        // Section 6: Compliance (India-ready)

        /// <summary>HSN Code - max 8 chars (for goods)</summary>
        [StringLength(8, ErrorMessage = "HSN Code cannot exceed 8 characters")]
        public string? HSNCode { get; set; }

        /// <summary>SAC Code - max 6 chars (for services)</summary>
        [StringLength(6, ErrorMessage = "SAC Code cannot exceed 6 characters")]
        public string? SACCode { get; set; }

        // Section 7: Reference

        /// <summary>Reference Text - optional line reference</summary>
        [StringLength(100, ErrorMessage = "Reference cannot exceed 100 characters")]
        public string? ReferenceText { get; set; }

        // Section 8: Calculated Amounts

        /// <summary>Line Base Amount = Quantity × UnitPrice</summary>
        public decimal LineBaseAmount => Quantity * UnitPrice;

        /// <summary>Line Subtotal = LineBaseAmount - DiscountAmount</summary>
        public decimal LineSubTotalAmount => LineBaseAmount - DiscountAmount;

        /// <summary>Line Total = LineSubTotal + TaxAmount</summary>
        public decimal LineTotalAmount => LineSubTotalAmount + TaxAmount;

        // Section 9: System Audit Fields

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Helper Methods

        /// <summary>Recalculate line amounts</summary>
        public void RecalculateAmounts()
        {
            // Calculate discount amount from percent if not manually set
            if (DiscountPercent > 0 && DiscountAmount == 0)
            {
                DiscountAmount = Math.Round(LineBaseAmount * (DiscountPercent / 100), 2);
            }

            // Calculate tax amount
            if (TaxRatePercent > 0)
            {
                TaxAmount = Math.Round(LineSubTotalAmount * (TaxRatePercent / 100), 2);
            }
            else
            {
                TaxAmount = 0;
            }
        }
    }

    #endregion

    #region VendorCreditNoteLine-related Enums

    public static class VendorCreditNoteLineTypes
    {
        public const string Item = "Item";
        public const string Service = "Service";
        public const string Manual = "Manual";
        public static readonly string[] All = new[] { Item, Service, Manual };

        public static string GetDisplayName(string type) => type switch
        {
            Item => "Item",
            Service => "Service",
            Manual => "Manual Entry",
            _ => type
        };
    }

    #endregion
}
