using System;
using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    // ============================
    // ENUMS (OUTSIDE MODEL)
    // ============================

    public enum FundTransferStatus
    {
        Draft,
        Submitted,
        Approved,
        Rejected,
        Cancelled,
        Initiated,
        InTransit,
        Completed,
        Posted,
        Failed,
        Reversed,
        Closed
    }

    public enum TransferMethod
    {
        NEFT,
        RTGS,
        IMPS,
        UPI,
        BankInternal,
        Swift,
        Other
    }

   

    public enum ChargeHandlingMode
    {
        None,
        ExpectedOnly,
        PostNow,
        PostWhenAppearsInStatement
    }

    public enum ChargeBearerType
    {
        Source,
        Destination,
        Shared
    }

    public enum ChannelType
    {
        NetBanking,
        MobileApp,
        BankAPI,
        BulkUpload,
        Manual
    }
    public class FundTransferModel
    {
        public Guid FundTransferId { get; set; } = Guid.NewGuid();
        public string FundTransferNumber { get; set; } = "";
        public Guid? CompanyId { get; set; }
        public Guid? BranchId { get; set; }
        [Required(ErrorMessage = "Company is required")]
        public string Company { get; set; } = "";
        [Required(ErrorMessage = "Branch is required")]
        public string Branch { get; set; } = "";
        public FundTransferStatus? Status { get; set; }
        [Required]
        public string SourceBankAccount { get; set; } = "";
        [Required]
        public string DestinationBankAccount { get; set; } = "";
        [Required]
        public DateTime TransferDate { get; set; } = DateTime.Today;
        public DateTime SourceValueDate { get; set; } = DateTime.Today;
        public DateTime DestinationValueDate { get; set; } = DateTime.Today;
        [Range(0.01, 999999999)]
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        [Required]
        public string Narration { get; set; } = "";
        public string ChargeNarration { get; set; } = "";
        public string? ReasonCode { get; set; }
        public TransferMethod? TransferMethod { get; set; }
        public string? UTRNumber { get; set; }
        public string? InitiationReference { get; set; }
        public ChannelType? Channel { get; set; }
        [MaxLength(200, ErrorMessage = "Beneficiary narration cannot exceed 200 characters")]
        public string? BeneficiaryNarration { get; set; }
        public string RequestedBy { get; set; } = "Treasury";
        public string? ApprovedBy { get; set; }
        public DateTime? SubmittedOn { get; set; } = DateTime.Today;
        public DateTime? ApprovedOn { get; set; } = DateTime.Today;
        [MaxLength(1000, ErrorMessage = "Approval notes cannot exceed 1000 characters")]
        public string? ApprovalNotes { get; set; }
        [MaxLength(500, ErrorMessage = "Failure reason cannot exceed 500 characters")]
        public string? FailureReason { get; set; }
        public Guid? TransferGroupId { get; set; }
        public Guid? SourceLegId { get; set; }
        public Guid? DestinationLegId { get; set; }
        public ChargeHandlingMode? ChargeHandlingMode { get; set; }
        public decimal? ExpectedChargeAmount { get; set; }
        public ChargeBearerType? ChargeBearerType { get; set; }
        public Guid? ReversalFundTransferId { get; set; }
        public string? ReversalReason { get; set; }
        public string? Attachment { get; set; }   // store filename or base64
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}
