using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    /// <summary>
    /// ARAdjustmentReasonMaster - Lookup table for adjustment reasons.
    /// </summary>
    public class ARAdjustmentReasonViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CompanyId { get; set; }

        [Required(ErrorMessage = "Reason Code is required")]
        [StringLength(20, ErrorMessage = "Reason Code cannot exceed 20 characters")]
        public string ReasonCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Reason Description is required")]
        [StringLength(100, ErrorMessage = "Reason Description cannot exceed 100 characters")]
        public string ReasonDescription { get; set; } = string.Empty;

        /// <summary>Applicable Adjustment Types for this reason</summary>
        public string[] ApplicableTypes { get; set; } = Array.Empty<string>();

        /// <summary>Default GL Offset Account for this reason</summary>
        public Guid? DefaultOffsetAccountId { get; set; }
        public string? DefaultOffsetAccountCode { get; set; }
        public string? DefaultOffsetAccountName { get; set; }

        /// <summary>Requires Approval for this reason</summary>
        public bool RequiresApproval { get; set; } = false;

        /// <summary>Requires Evidence/Attachment for this reason</summary>
        public bool RequiresEvidence { get; set; } = false;

        /// <summary>Amount Threshold above which approval is required</summary>
        public decimal? ApprovalThreshold { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
    }

}
