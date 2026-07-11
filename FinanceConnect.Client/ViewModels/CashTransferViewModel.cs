using System;
using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public class CashTransferModel
    {
        // ================= ENUMS =================

        public enum CashTransferStatusEnum
        {
            Draft,
            Submitted,
            Approved,
            Rejected,
            Cancelled,
            InTransit,
            Received,
            Posted,
            Reversed,
            Closed
        }

        public enum ReasonCodeEnum
        {
            BranchTopUp,
            PettyCashReplenishment,
            VaultMovement,
            SiteCash,
            Other
        }

        // ================= CORE IDENTITY =================

        public Guid CashTransferId { get; set; }

        public Guid TenantId { get; set; }

        public Guid CompanyId { get; set; }

        [Required(ErrorMessage = "Branch is required")]
        public Guid? BranchId { get; set; }

        [Required(ErrorMessage = "Transfer number is required")]
        [MaxLength(40, ErrorMessage = "Transfer number cannot exceed 40 characters")]
        public string CashTransferNumber { get; set; } = string.Empty;

        public CashTransferStatusEnum CashTransferStatus { get; set; }

        // ================= TRANSFER DETAILS =================

        [Required(ErrorMessage = "Source cash account is required")]
        public Guid? SourceCashAccountId { get; set; }

        [Required(ErrorMessage = "Destination cash account is required")]
        public Guid? DestinationCashAccountId { get; set; }


        [Required(ErrorMessage = "Transfer date is required")]
        public DateTime? TransferDate { get; set; }


        public Guid? CurrencyId { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Purpose / Notes is required")]
        [MaxLength(1000, ErrorMessage = "Narration cannot exceed 1000 characters")]
        public string Narration { get; set; } = string.Empty;

        public ReasonCodeEnum? ReasonCode { get; set; }

        // ================= DISPLAY ONLY =================

        public string SourceCashAccountName { get; set; } = string.Empty;

        public string DestinationCashAccountName { get; set; } = string.Empty;

        public string BranchName { get; set; } = string.Empty;

        public string CurrencyCode { get; set; } = "INR";

        // ================= POSTING =================

        public CashTransferStatusEnum PostingStatus { get; set; }

        // ================= HANDOVER & TRANSIT =================

        public string HandedOverByUserName { get; set; } = string.Empty;

        public DateTime? HandedOverOn { get; set; }

        public string TransitMethod { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "Transit reference cannot exceed 100 characters")]
        public string TransitReference { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Transit notes cannot exceed 500 characters")]
        public string TransitNotes { get; set; } = string.Empty;

        // ================= RECEIVE CONFIRMATION =================

        public string ReceivedByUserName { get; set; } = string.Empty;

        public DateTime? ReceivedOn { get; set; }

        public decimal ReceivedAmount { get; set; }

        [MaxLength(500, ErrorMessage = "Acknowledgement notes cannot exceed 500 characters")]
        public string ReceiptAcknowledgementNote { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}
