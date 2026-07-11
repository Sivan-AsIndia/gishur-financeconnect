using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public class AssetAcquisitionViewModel
    {
        // ── Enums ──────────────────────────────────────────────────────
        public enum AcquisitionStatusEnum
        {
            Draft = 1,
            Submitted = 2,
            Approved = 3,
            Rejected = 4,
            Posted = 5,
            Cancelled = 6,
            Reversed = 7
        }

        public enum AcquisitionTypeEnum
        {
            InitialCapitalization = 1,
            CapitalImprovement = 2,
            OpeningBalance = 3,
            Donation = 4,
            SelfConstructed = 5,
            TransferIn = 6
        }

        public enum SourceModuleEnum
        {
            AP = 1,
            CashBank = 2,
            Manual = 3,
            System = 4
        }

        public enum PostingRouteEnum
        {
            DirectToAsset = 1,
            ToCWIP = 2,
            CWIPToAsset = 3
        }

        public enum CostComponentTypeEnum
        {
            BaseCost = 1,
            Freight = 2,
            Installation = 3,
            CustomsDuty = 4,
            NonRecoverableTax = 5,
            ProfessionalFee = 6,
            OtherCapitalizable = 7,
            NonCapitalizable = 8
        }

        // ── AssetAcquisitionLine (child) ───────────────────────────────
        public class AssetAcquisitionLine
        {
            public Guid AssetAcquisitionLineId { get; set; } = Guid.NewGuid();
            public Guid AssetAcquisitionId { get; set; }

            [Required(ErrorMessage = "Line Number is required")]
            public int LineNumber { get; set; }

            [Required(ErrorMessage = "Cost Component Type is required")]
            public CostComponentTypeEnum? CostComponentType { get; set; }

            [StringLength(300)]
            public string? LineDescription { get; set; }

            [Required(ErrorMessage = "Line Amount is required")]
            [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be > 0")]
            public decimal LineAmount { get; set; }

            public bool IsCapitalizable { get; set; } = true;

            public Guid? TaxCodeId { get; set; }
            public decimal TaxAmount { get; set; }

            public decimal LineTotalAmount => LineAmount + TaxAmount;

            public Guid? LineGLAccountOverrideId { get; set; }
        }

        // ── AssetAcquisition (header) ──────────────────────────────────
        public class AssetAcquisition
        {
            // ── Section 1: Core Identity (Header) ──────────────────────
            public Guid AssetAcquisitionId { get; set; }
            public Guid TenantId { get; set; }
            public Guid CompanyId { get; set; }

            [Required(ErrorMessage = "Branch is required")]
            public Guid? BranchId { get; set; }

            [StringLength(40)]
            public string AcquisitionNumber { get; set; } = "";

            [Required(ErrorMessage = "Acquisition Status is required")]
            public AcquisitionStatusEnum AcquisitionStatus { get; set; } = AcquisitionStatusEnum.Draft;

            [Required(ErrorMessage = "Acquisition Type is required")]
            public AcquisitionTypeEnum? AcquisitionType { get; set; }

            [Required(ErrorMessage = "Acquisition Date is required")]
            public DateTime? AcquisitionDate { get; set; }

            [Required(ErrorMessage = "Capitalization Date is required")]
            public DateTime? CapitalizationDate { get; set; }

            public DateTime? PostingDate { get; set; }

            // ── Section 2: Asset Linkage ───────────────────────────────
            [Required(ErrorMessage = "Fixed Asset is required")]
            public Guid? FixedAssetId { get; set; }

            public Guid? AssetCategoryIdSnapshot { get; set; }

            [StringLength(40)]
            public string? AssetNumberSnapshot { get; set; }

            [StringLength(200)]
            public string? AssetNameSnapshot { get; set; }

            public string? AssetStatusSnapshot { get; set; }

            // ── Section 3: Source & References ─────────────────────────
            [Required(ErrorMessage = "Source Module is required")]
            public SourceModuleEnum? SourceModule { get; set; }

            public Guid? VendorId { get; set; }

            [StringLength(50)]
            public string? VendorInvoiceNumber { get; set; }

            public DateTime? VendorInvoiceDate { get; set; }

            public Guid? APVendorBillId { get; set; }

            [StringLength(50)]
            public string? PurchaseOrderRef { get; set; }

            [StringLength(100)]
            public string? ReferenceText { get; set; }

            [StringLength(1000)]
            public string? Narration { get; set; }

            // ── Section 4: Cost Lines ──────────────────────────────────
            public List<AssetAcquisitionLine> CostLines { get; set; } = new();

            // ── Section 5: Totals (System Derived) ─────────────────────
            public decimal SubTotalAmount => CostLines.Where(l => l.IsCapitalizable).Sum(l => l.LineAmount);
            public decimal TotalTaxAmount => CostLines.Where(l => l.IsCapitalizable).Sum(l => l.TaxAmount);
            public decimal RoundOffAmount { get; set; }
            public decimal TotalCapitalizedAmount => SubTotalAmount + TotalTaxAmount + RoundOffAmount;
            public bool BelowThresholdFlag { get; set; }
            public bool ThresholdOverrideApproved { get; set; }

            // ── Section 6: Posting & GL Mapping ────────────────────────
            [Required(ErrorMessage = "Posting Route is required")]
            public PostingRouteEnum? PostingRoute { get; set; }

            public Guid? AssetCostGLAccountIdSnapshot { get; set; }
            public Guid? CWIPGLAccountIdSnapshot { get; set; }
            public Guid? ClearingGLAccountIdSnapshot { get; set; }
            public Guid? JournalEntryId { get; set; }
            public DateTime? PostedOn { get; set; }
            public Guid? PostedBy { get; set; }

            // ── Section 7: Attachments ─────────────────────────────────
            public int AttachmentCount { get; set; }

            // ── Section 8: System Audit Fields ─────────────────────────
            public DateTime CreatedAt { get; set; }
            public Guid CreatedBy { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public Guid? UpdatedBy { get; set; }
            public byte[]? RowVersion { get; set; }
            public bool IsDeleted { get; set; } = false;
        }
    }
}
