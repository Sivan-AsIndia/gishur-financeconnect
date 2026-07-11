using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public class AssetTransformViewModel
    {
        /* ── Status Enum ─────────────────────────────────────────────────────── */
        public enum TransferStatus
        {
            Draft = 1,
            Submitted = 2,
            Approved = 3,
            Rejected = 4,
            Cancelled = 5,
            InTransit = 6,
            Received = 7,
            Posted = 8,
            Reversed = 9,
            Closed = 10
        }

        /* ── Transfer Type Enum ──────────────────────────────────────────────── */
        public enum TransferType
        {
            CustodianChange = 1,
            LocationChange = 2,
            BranchChange = 3,
            FullReassignment = 4
        }

        /* ── Asset Condition Enum ────────────────────────────────────────────── */
        public enum AssetCondition
        {
            Good = 1,
            RequiresRepair = 2,
            Damaged = 3,
            MissingParts = 4
        }

        public class AssetTransfer
        {
            public Guid AssetTransferId { get; set; }

            // ── Section 1: Core Identity ──────────────────────────────────────
            public string? TransferNumber { get; set; }
            public TransferStatus TransferStatus { get; set; } = TransferStatus.Draft;

            [Required(ErrorMessage = "Transfer Type is required")]
            public TransferType? TransferType { get; set; }

            [Required(ErrorMessage = "Effective Date is required")]
            public DateTime EffectiveTransferDate { get; set; } = DateTime.Today;

            public DateTime? RequestedOn { get; set; }
            public Guid? RequestedBy { get; set; }

            // ── Section 2: Asset Linkage ──────────────────────────────────────
            [Required(ErrorMessage = "Asset is required")]
            public Guid FixedAssetId { get; set; }
            public string? AssetNumberSnapshot { get; set; }
            public string? AssetNameSnapshot { get; set; }
            public Guid? AssetCategoryIdSnapshot { get; set; }
            public string? AssetStatusSnapshot { get; set; }

            // ── Section 3: From Snapshot (auto-captured) ──────────────────────
            public Guid? FromBranchId { get; set; }
            public string? FromBranchName { get; set; }
            public Guid? FromLocationId { get; set; }
            public string? FromLocationName { get; set; }
            public Guid? FromCustodianUserId { get; set; }
            public string? FromCustodianName { get; set; }
            public Guid? FromDepartmentId { get; set; }
            public string? FromDepartmentName { get; set; }
            public Guid? FromCostCenterId { get; set; }
            public string? FromCostCenterName { get; set; }
            public Guid? FromProjectId { get; set; }
            public string? FromProjectName { get; set; }
            public DateTime? FromAssignedOnSnapshot { get; set; }

            // ── Section 4: To Target Values (user input) ──────────────────────
            public Guid? ToBranchId { get; set; }
            public string? ToBranchName { get; set; }
            public Guid? ToLocationId { get; set; }
            public string? ToLocationName { get; set; }
            public Guid? ToCustodianUserId { get; set; }
            public string? ToCustodianName { get; set; }
            public Guid? ToDepartmentId { get; set; }
            public string? ToDepartmentName { get; set; }
            public Guid? ToCostCenterId { get; set; }
            public string? ToCostCenterName { get; set; }
            public Guid? ToProjectId { get; set; }
            public string? ToProjectName { get; set; }

            [StringLength(500)]
            public string? TransferReason { get; set; }

            // ── Section 5: Handover & Acknowledgement ─────────────────────────
            public bool HandoverRequired { get; set; } = true;
            public Guid? HandedOverByUserId { get; set; }
            public string? HandedOverByName { get; set; }
            public DateTime? HandedOverOn { get; set; }
            public Guid? ReceiverUserId { get; set; }
            public string? ReceiverName { get; set; }
            public DateTime? ReceivedOn { get; set; }

            [StringLength(500)]
            public string? ReceiverAcknowledgementNote { get; set; }
            public AssetCondition? AssetConditionAtHandover { get; set; }

            [StringLength(500)]
            public string? ConditionNotes { get; set; }

            // ── Section 6: Posting / Application Evidence ─────────────────────
            public bool AppliedToAssetFlag { get; set; }
            public DateTime? AppliedOn { get; set; }
            public Guid? AppliedBy { get; set; }
            public Guid? ReverseTransferId { get; set; }

            [StringLength(500)]
            public string? ReversalReason { get; set; }

            // ── Audit ─────────────────────────────────────────────────────────
            public Guid? CompanyId { get; set; }
            public Guid? UpdatedBy { get; set; }
        }
        public class AssetTransferListDto
        {
            // ── Core Identity ─────────────────────────────────────────────────
            public Guid AssetTransferId { get; set; }
            public string? TransferNumber { get; set; }
            public TransferStatus TransferStatus { get; set; }
            public TransferType? TransferType { get; set; }
            public DateTime EffectiveTransferDate { get; set; }

            // ── Asset Info ────────────────────────────────────────────────────
            public Guid FixedAssetId { get; set; }
            public string? AssetNumberSnapshot { get; set; }
            public string? AssetNameSnapshot { get; set; }

            // ── From ──────────────────────────────────────────────────────────
            public Guid? FromBranchId { get; set; }
            public string? FromBranchName { get; set; }
            public Guid? FromLocationId { get; set; }
            public string? FromLocationName { get; set; }
            public Guid? FromCustodianUserId { get; set; }
            public string? FromCustodianName { get; set; }

            // ── To ────────────────────────────────────────────────────────────
            public Guid? ToBranchId { get; set; }
            public string? ToBranchName { get; set; }
            public Guid? ToLocationId { get; set; }
            public string? ToLocationName { get; set; }
            public Guid? ToCustodianUserId { get; set; }
            public string? ToCustodianName { get; set; }

            public string? TransferReason { get; set; }

            // ── ✅ Handover & Acknowledgement ─────────────────────────────────
            public bool HandoverRequired { get; set; } = true;
            public Guid? HandedOverByUserId { get; set; }
            public string? HandedOverByName { get; set; }
            public DateTime? HandedOverOn { get; set; }
            public Guid? ReceiverUserId { get; set; }
            public string? ReceiverName { get; set; }
            public DateTime? ReceivedOn { get; set; }
            public string? ReceiverAcknowledgementNote { get; set; }
            public AssetCondition? AssetConditionAtHandover { get; set; }
            public string? ConditionNotes { get; set; }

            // ── ✅ Posting / Application Evidence ─────────────────────────────
            public bool AppliedToAssetFlag { get; set; }
            public DateTime? AppliedOn { get; set; }
            public Guid? AppliedByUserId { get; set; }

            // ── ✅ Reversal ───────────────────────────────────────────────────
            public Guid? ReverseTransferId { get; set; }
            public string? ReversalReason { get; set; }

            // ── Audit ─────────────────────────────────────────────────────────
            public Guid? CompanyId { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public bool IsDeleted { get; set; }
        }

        /* ── Select-list Helper ──────────────────────────────────────────────── */
        public class SelectItem
        {
            public string Value { get; set; } = "";
            public string Text { get; set; } = "";
        }
    }
}
