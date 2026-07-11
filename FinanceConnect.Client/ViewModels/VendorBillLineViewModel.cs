using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    #region Model #37: VendorBillLine

    /// <summary>
    /// Model #37: VendorBillLine – One charge row inside a VendorBill.
    /// The actual financial content - expense/asset details, quantities, rates, discounts, and taxes.
    /// </summary>
    public class VendorBillLineViewModel
    {
        // Section 1: Core Line Fields

        /// <summary>PK - hidden in UI</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>FK → VendorBill - hidden</summary>
        [Required(ErrorMessage = "Bill is required")]
        public Guid VendorBillId { get; set; }

        /// <summary>Line Number - auto-generated sequentially (10, 20, 30)</summary>
        [Required(ErrorMessage = "Line Number is required")]
        public int LineNumber { get; set; }

        /// <summary>Line Type - Goods/Service/Expense/Asset/Other</summary>
        [Required(ErrorMessage = "Line Type is required")]
        public string LineType { get; set; } = "";

        /// <summary>FK → ItemMaster/ServiceMaster (future) - Search dropdown</summary>
        public Guid? ItemId { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }

        /// <summary>Description - max 300 chars, required</summary>
        [Required(ErrorMessage = "Description is required")]
        [StringLength(300, ErrorMessage = "Description cannot exceed 300 characters")]
        public string Description { get; set; } = string.Empty;

        /// <summary>FK → UomMaster (future) - optional</summary>
        public Guid? UomId { get; set; }
        public string? UomCode { get; set; }
        public string? UomName { get; set; }

        /// <summary>Quantity - must be > 0</summary>
        [Required(ErrorMessage = "Quantity is required")]
        [Range(0.001, double.MaxValue, ErrorMessage = "Quantity must be greater than zero")]
        public decimal Quantity { get; set; } = 1;

        /// <summary>Unit Rate - must be >= 0</summary>
        [Required(ErrorMessage = "Unit Rate is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Unit Rate must be zero or greater")]
        public decimal UnitRate { get; set; } = 0;

        /// <summary>Discount Amount - >= 0 and <= GrossAmount</summary>
        [Range(0, double.MaxValue, ErrorMessage = "Discount Amount must be zero or greater")]
        public decimal DiscountAmount { get; set; } = 0;

        /// <summary>Gross Amount = Quantity × UnitRate (derived, read-only)</summary>
        public decimal GrossAmount => Math.Round(Quantity * UnitRate, 2);

        /// <summary>Taxable Amount = GrossAmount - DiscountAmount (derived, read-only)</summary>
        public decimal TaxableAmount => GrossAmount - DiscountAmount;

        // Section 2: Account Mapping

        /// <summary>FK → GLAccountMaster - Expense/Asset Account, must be Active + Postable</summary>
        [Required(ErrorMessage = "Expense/Asset Account is required")]
        public Guid ExpenseOrAssetAccountId { get; set; }
        public string? ExpenseOrAssetAccountCode { get; set; }
        public string? ExpenseOrAssetAccountName { get; set; }

        /// <summary>Expense/Asset Account Id Snapshot - captured at posting</summary>
        public Guid? ExpenseOrAssetAccountIdSnapshot { get; set; }
        
        /// <summary>Expense/Asset Account Code Snapshot - captured at posting</summary>
        public string? ExpenseOrAssetAccountCodeSnapshot { get; set; }
        
        /// <summary>Expense/Asset Account Name Snapshot - captured at posting</summary>
        public string? ExpenseOrAssetAccountNameSnapshot { get; set; }

        /// <summary>FK → GLAccountMaster - Input Tax Account (from tax mapping)</summary>
        public Guid? InputTaxAccountId { get; set; }
        public string? InputTaxAccountCode { get; set; }
        public string? InputTaxAccountName { get; set; }

        /// <summary>Input Tax Account Id Snapshot - captured at posting</summary>
        public Guid? InputTaxAccountIdSnapshot { get; set; }

        // Section 3: Tax Fields (GST Input)

        /// <summary>FK → TaxCodeMaster - GST Tax Code</summary>
        public Guid? TaxCodeId { get; set; }
        public string? TaxCodeCode { get; set; }
        public string? TaxCodeName { get; set; }

        /// <summary>Tax Rate Percent - snapshot from tax code</summary>
        [Range(0, 100, ErrorMessage = "Tax Rate must be between 0 and 100")]
        public decimal TaxRatePercentSnapshot { get; set; } = 0;

        /// <summary>Tax Type Snapshot - CGST/SGST/IGST/Cess/None</summary>
        public string TaxTypeSnapshot { get; set; } = TaxTypes.None;

        /// <summary>CGST Amount - derived</summary>
        public decimal CGSTAmount { get; set; } = 0;

        /// <summary>SGST Amount - derived</summary>
        public decimal SGSTAmount { get; set; } = 0;

        /// <summary>IGST Amount - derived</summary>
        public decimal IGSTAmount { get; set; } = 0;

        /// <summary>Cess Amount - optional future</summary>
        public decimal CessAmount { get; set; } = 0;

        /// <summary>Is Reverse Charge Applicable (per line override)</summary>
        public bool IsReverseChargeApplicable { get; set; } = false;

        // Section 4: Derived Line Totals

        /// <summary>Line Net Amount (Before Tax) = TaxableAmount</summary>
        public decimal LineNetAmount => TaxableAmount;

        /// <summary>Line Tax Amount = CGST + SGST + IGST + Cess</summary>
        public decimal LineTaxAmount => CGSTAmount + SGSTAmount + IGSTAmount + CessAmount;

        /// <summary>Line Total Amount = LineNetAmount + LineTaxAmount</summary>
        public decimal LineTotalAmount => LineNetAmount + LineTaxAmount;

        /// <summary>Base Currency Amount - converted using bill exchange rate (future)</summary>
        public decimal BaseCurrencyAmount { get; set; } = 0;

        // Section 5: Optional Future-Ready Dimensions

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

        /// <summary>Reference - service ticket/contract ref</summary>
        [StringLength(100, ErrorMessage = "Reference cannot exceed 100 characters")]
        public string? ReferenceText { get; set; }

        /// <summary>FK → PurchaseOrder (future)</summary>
        public Guid? POId { get; set; }
        public Guid? POLineId { get; set; }

        /// <summary>FK → GoodsReceipt (future)</summary>
        public Guid? GRNId { get; set; }
        public Guid? GRNLineId { get; set; }

        /// <summary>Is Capitalized - fixed asset capitalization workflow (future)</summary>
        public bool IsCapitalized { get; set; } = false;

        /// <summary>FK → AssetCategory (future)</summary>
        public Guid? FixedAssetCategoryId { get; set; }

        /// <summary>HSN Code - GST classification (future compliance)</summary>
        [StringLength(20, ErrorMessage = "HSN Code cannot exceed 20 characters")]
        public string? HSNCode { get; set; }

        /// <summary>SAC Code - GST classification for services</summary>
        [StringLength(20, ErrorMessage = "SAC Code cannot exceed 20 characters")]
        public string? SACCode { get; set; }

        // Section 6: System Audit Fields

        public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
        public bool IsDeleted { get; set; } = false;

        // Helper Methods

        /// <summary>Calculate tax amount from tax rate (splits IGST vs CGST+SGST)</summary>
        public void CalculateTaxAmount(bool isInterState = false)
        {
            var totalTax = Math.Round(TaxableAmount * TaxRatePercentSnapshot / 100, 2);
            
            if (isInterState)
            {
                // Inter-state: Full IGST
                IGSTAmount = totalTax;
                CGSTAmount = 0;
                SGSTAmount = 0;
                TaxTypeSnapshot = TaxTypes.IGST;
            }
            else
            {
                // Intra-state: Split CGST + SGST
                CGSTAmount = Math.Round(totalTax / 2, 2);
                SGSTAmount = totalTax - CGSTAmount; // Handle rounding
                IGSTAmount = 0;
                TaxTypeSnapshot = TaxTypes.CGST_SGST;
            }
        }

        /// <summary>Recalculate all line amounts</summary>
        public void RecalculateAmounts(bool isInterState = false)
        {
            CalculateTaxAmount(isInterState);
        }
    }

    #endregion

    #region VendorBillLine-related Enums and Static Classes

    public static class VendorBillLineTypes
    {
        public const string Goods = "Goods";
        public const string Service = "Service";
        public const string Expense = "Expense";
        public const string Asset = "Asset";
        public const string Other = "Other";
        public static readonly string[] All = new[] { Goods, Service, Expense, Asset, Other };

        public static string GetDisplayName(string type) => type switch
        {
            Goods => "Goods",
            Service => "Service",
            Expense => "Expense",
            Asset => "Asset",
            Other => "Other",
            _ => type
        };
    }

    public static class TaxTypes
    {
        public const string None = "None";
        public const string CGST_SGST = "CGST_SGST";
        public const string IGST = "IGST";
        public const string Cess = "Cess";
        public static readonly string[] All = new[] { None, CGST_SGST, IGST, Cess };
    }

    #endregion
}
