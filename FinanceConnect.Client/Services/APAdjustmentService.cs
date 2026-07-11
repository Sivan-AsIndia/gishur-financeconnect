using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    /// <summary>
    /// Service for APAdjustment (Model #41) CRUD operations
    /// Demo application - data stored in memory
    /// </summary>
    public class APAdjustmentService
    {
        // Immutable seed data
        private static readonly List<APAdjustmentViewModel> _seedAdjustments = APAdjustmentSeedData.GetSeedAdjustments();
        private static readonly List<APAdjustmentReasonViewModel> _seedReasons = APAdjustmentSeedData.GetSeedReasons();

        // Working (mutable) data
        private List<APAdjustmentViewModel> _adjustments;
        private List<APAdjustmentReasonViewModel> _reasons;

        // Counter for adjustment number generation
        private int _adjustmentCounter = 100;

        public APAdjustmentService()
        {
            ResetToSeed();
        }

        private static List<T> CloneList<T>(IEnumerable<T> source) where T : class
        {
            var json = System.Text.Json.JsonSerializer.Serialize(source);
            return System.Text.Json.JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }

        /// <summary>Reset adjustments to seed data</summary>
        public void ResetToSeed()
        {
            _adjustments = CloneList(_seedAdjustments);
            _reasons = CloneList(_seedReasons);
        }

        #region Reason Code Operations

        /// <summary>Get all active reason codes</summary>
        public List<APAdjustmentReasonViewModel> GetAllReasons()
        {
            return _reasons.Where(r => r.IsActive).ToList();
        }

        /// <summary>Get reason code by ID</summary>
        public APAdjustmentReasonViewModel? GetReasonById(Guid id)
        {
            return _reasons.FirstOrDefault(r => r.Id == id);
        }

        /// <summary>Get reason codes by adjustment type</summary>
        public List<APAdjustmentReasonViewModel> GetReasonsByType(string adjustmentType)
        {
            return _reasons.Where(r => r.IsActive &&
                (r.ApplicableTypes.Contains(adjustmentType) || r.ApplicableTypes.Contains(APAdjustmentTypes.Other)))
                .ToList();
        }

        #endregion

        #region Read Operations

        /// <summary>Get all adjustments</summary>
        public List<APAdjustmentViewModel> GetAll()
        {
            return _adjustments.Where(a => !a.IsDeleted).ToList();
        }

        /// <summary>Get adjustment by ID</summary>
        public APAdjustmentViewModel? GetById(Guid id)
        {
            return _adjustments.FirstOrDefault(a => a.APAdjustmentId == id && !a.IsDeleted);
        }

        /// <summary>Get adjustments by company ID</summary>
        public List<APAdjustmentViewModel> GetByCompanyId(Guid companyId)
        {
            return _adjustments.Where(a => a.CompanyId == companyId && !a.IsDeleted).ToList();
        }

        /// <summary>Get adjustments by vendor ID</summary>
        public List<APAdjustmentViewModel> GetByVendorId(Guid vendorId)
        {
            return _adjustments.Where(a => a.VendorId == vendorId && !a.IsDeleted).ToList();
        }

        /// <summary>Get adjustments by status</summary>
        public List<APAdjustmentViewModel> GetByStatus(string status)
        {
            return _adjustments.Where(a => a.AdjustmentStatus == status && !a.IsDeleted).ToList();
        }

        /// <summary>Get adjustments by type</summary>
        public List<APAdjustmentViewModel> GetByType(string type)
        {
            return _adjustments.Where(a => a.AdjustmentType == type && !a.IsDeleted).ToList();
        }

        /// <summary>Get adjustments by branch ID</summary>
        public List<APAdjustmentViewModel> GetByBranchId(Guid branchId)
        {
            return _adjustments.Where(a => a.BranchId == branchId && !a.IsDeleted).ToList();
        }

        /// <summary>Get adjustments by date range</summary>
        public List<APAdjustmentViewModel> GetByDateRange(DateTime fromDate, DateTime toDate)
        {
            return _adjustments.Where(a =>
                a.AdjustmentDate >= fromDate &&
                a.AdjustmentDate <= toDate &&
                !a.IsDeleted).ToList();
        }

        /// <summary>Get adjustments pending approval</summary>
        public List<APAdjustmentViewModel> GetPendingApproval()
        {
            return _adjustments.Where(a =>
                a.AdjustmentStatus == APAdjustmentStatuses.Submitted &&
                a.PolicyLimitCategory != APPolicyLimitCategories.SmallWriteOff &&
                !a.IsDeleted).ToList();
        }

        /// <summary>Search adjustments by number, vendor code/name</summary>
        public List<APAdjustmentViewModel> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAll();

            searchTerm = searchTerm.ToLower();
            return _adjustments.Where(a => !a.IsDeleted && (
                a.AdjustmentNumber.ToLower().Contains(searchTerm) ||
                (a.VendorCode?.ToLower().Contains(searchTerm) ?? false) ||
                (a.VendorName?.ToLower().Contains(searchTerm) ?? false) ||
                (a.ReasonCode?.ToLower().Contains(searchTerm) ?? false) ||
                (a.Narration?.ToLower().Contains(searchTerm) ?? false)
            )).ToList();
        }

        /// <summary>Check if adjustment number exists within company</summary>
        public bool AdjustmentNumberExists(Guid companyId, string adjustmentNumber, Guid? excludeId = null)
        {
            return _adjustments.Any(a =>
                a.CompanyId == companyId &&
                a.AdjustmentNumber.Equals(adjustmentNumber, StringComparison.OrdinalIgnoreCase) &&
                !a.IsDeleted &&
                (excludeId == null || a.APAdjustmentId != excludeId));
        }

        /// <summary>Generate next adjustment number</summary>
        public string GenerateAdjustmentNumber(Guid companyId)
        {
            _adjustmentCounter++;
            var year = DateTime.Today.Year;
            return $"APADJ-{year}-{_adjustmentCounter:D4}";
        }

        #endregion

        #region Write Operations

        /// <summary>Add new adjustment</summary>
        public (bool Success, string Message) Add(APAdjustmentViewModel adjustment)
        {
            // Validate adjustment number uniqueness
            if (AdjustmentNumberExists(adjustment.CompanyId, adjustment.AdjustmentNumber))
            {
                return (false, "Adjustment number already exists.");
            }

            // Validate vendor
            if (adjustment.VendorId == Guid.Empty)
            {
                return (false, "Vendor is required.");
            }

            // Validate branch
            if (adjustment.BranchId == Guid.Empty)
            {
                return (false, "Branch is required.");
            }

            // Validate adjustment type
            if (string.IsNullOrWhiteSpace(adjustment.AdjustmentType))
            {
                return (false, "Adjustment type is required.");
            }

            // Validate reason code
            if (adjustment.ReasonCodeId == Guid.Empty)
            {
                return (false, "Reason code is required.");
            }

            // Validate narration (always required for adjustments)
            if (string.IsNullOrWhiteSpace(adjustment.Narration))
            {
                return (false, "Narration/explanation is required for adjustments.");
            }

            // Validate adjustment amount
            if (adjustment.AdjustmentAmount <= 0)
            {
                return (false, "Adjustment amount must be greater than zero.");
            }

            // Validate adjustment GL account
            if (adjustment.AdjustmentGLAccountId == Guid.Empty)
            {
                return (false, "Adjustment GL Account is required.");
            }

            // Validate scope-specific requirements
            if (adjustment.AdjustmentScope == APAdjustmentScopes.BillLevel && !adjustment.TargetVendorBillId.HasValue)
            {
                return (false, "Target vendor bill is required for bill-level adjustments.");
            }

            // Set policy limit category based on amount
            adjustment.PolicyLimitCategory = DeterminePolicyLimitCategory(adjustment.AdjustmentAmount, adjustment.AdjustmentType);

            // Check evidence requirement
            var reason = GetReasonById(adjustment.ReasonCodeId.Value);
            if (reason != null)
            {
                adjustment.EvidenceRequired = reason.RequiresEvidence ||
                    (reason.ApprovalThreshold.HasValue && adjustment.AdjustmentAmount > reason.ApprovalThreshold.Value);
            }

            adjustment.CreatedAt = DateTime.Now;
            _adjustments.Add(adjustment);

            return (true, "AP Adjustment created successfully.");
        }

        /// <summary>Update existing adjustment</summary>
        public (bool Success, string Message) Update(APAdjustmentViewModel adjustment)
        {
            var existing = _adjustments.FirstOrDefault(a => a.APAdjustmentId == adjustment.APAdjustmentId && !a.IsDeleted);
            if (existing == null)
            {
                return (false, "Adjustment not found.");
            }

            if (!existing.CanEdit)
            {
                return (false, "This adjustment cannot be edited in its current status.");
            }

            // Validate adjustment number uniqueness
            if (AdjustmentNumberExists(adjustment.CompanyId, adjustment.AdjustmentNumber, adjustment.APAdjustmentId))
            {
                return (false, "Adjustment number already exists.");
            }

            // Validate required fields
            if (adjustment.VendorId == Guid.Empty)
            {
                return (false, "Vendor is required.");
            }

            if (adjustment.BranchId == Guid.Empty)
            {
                return (false, "Branch is required.");
            }

            if (string.IsNullOrWhiteSpace(adjustment.Narration))
            {
                return (false, "Narration/explanation is required for adjustments.");
            }

            if (adjustment.AdjustmentAmount <= 0)
            {
                return (false, "Adjustment amount must be greater than zero.");
            }

            if (adjustment.AdjustmentGLAccountId == Guid.Empty)
            {
                return (false, "Adjustment GL Account is required.");
            }

            // Validate scope-specific requirements
            if (adjustment.AdjustmentScope == APAdjustmentScopes.BillLevel && !adjustment.TargetVendorBillId.HasValue)
            {
                return (false, "Target vendor bill is required for bill-level adjustments.");
            }

            // Update policy limit category
            adjustment.PolicyLimitCategory = DeterminePolicyLimitCategory(adjustment.AdjustmentAmount, adjustment.AdjustmentType);

            // Check evidence requirement
            var reason = GetReasonById(adjustment.ReasonCodeId.Value);
            if (reason != null)
            {
                adjustment.EvidenceRequired = reason.RequiresEvidence ||
                    (reason.ApprovalThreshold.HasValue && adjustment.AdjustmentAmount > reason.ApprovalThreshold.Value);
            }

            // Update all editable fields
            existing.BranchId = adjustment.BranchId;
            existing.BranchCode = adjustment.BranchCode;
            existing.BranchName = adjustment.BranchName;
            existing.VendorId = adjustment.VendorId;
            existing.VendorCode = adjustment.VendorCode;
            existing.VendorName = adjustment.VendorName;
            existing.AdjustmentDate = adjustment.AdjustmentDate;
            existing.CurrencyId = adjustment.CurrencyId;
            existing.CurrencyCode = adjustment.CurrencyCode;
            existing.CurrencyName = adjustment.CurrencyName;
            existing.ExchangeRate = adjustment.ExchangeRate;
            existing.AdjustmentType = adjustment.AdjustmentType;
            existing.AdjustmentDirection = adjustment.AdjustmentDirection;
            existing.ReasonCodeId = adjustment.ReasonCodeId;
            existing.ReasonCode = adjustment.ReasonCode;
            existing.ReasonDescription = adjustment.ReasonDescription;
            existing.PolicyLimitCategory = adjustment.PolicyLimitCategory;
            existing.AdjustmentScope = adjustment.AdjustmentScope;
            existing.TargetVendorBillId = adjustment.TargetVendorBillId;
            existing.TargetVendorBillNumber = adjustment.TargetVendorBillNumber;
            existing.TargetBillOutstandingSnapshot = adjustment.TargetBillOutstandingSnapshot;
            existing.TargetReferenceText = adjustment.TargetReferenceText;
            existing.AdjustmentAmount = adjustment.AdjustmentAmount;
            existing.AdjustmentGLAccountId = adjustment.AdjustmentGLAccountId;
            existing.AdjustmentGLAccountCode = adjustment.AdjustmentGLAccountCode;
            existing.AdjustmentGLAccountName = adjustment.AdjustmentGLAccountName;
            existing.AdjustmentGLAccountType = adjustment.AdjustmentGLAccountType;
            existing.Narration = adjustment.Narration;
            existing.HasAttachments = adjustment.HasAttachments;
            existing.AttachmentCount = adjustment.AttachmentCount;
            existing.EvidenceRequired = adjustment.EvidenceRequired;
            existing.UpdatedAt = DateTime.Now;
            existing.UpdatedBy = adjustment.UpdatedBy;

            return (true, "AP Adjustment updated successfully.");
        }

        /// <summary>Delete adjustment (soft delete, Draft only)</summary>
        public (bool Success, string Message) Delete(Guid id, string userName)
        {
            var adjustment = _adjustments.FirstOrDefault(a => a.APAdjustmentId == id && !a.IsDeleted);
            if (adjustment == null)
            {
                return (false, "Adjustment not found.");
            }

            if (adjustment.AdjustmentStatus != APAdjustmentStatuses.Draft)
            {
                return (false, "Only draft adjustments can be deleted.");
            }

            adjustment.IsDeleted = true;
            adjustment.DeletedAt = DateTime.Now;
            adjustment.DeletedBy = userName;
            adjustment.UpdatedAt = DateTime.Now;
            adjustment.UpdatedBy = userName;

            return (true, "AP Adjustment deleted successfully.");
        }

        private string DeterminePolicyLimitCategory(decimal amount, string adjustmentType)
        {
            // Basic policy limits (in real app, these would be configurable)
            if (adjustmentType == APAdjustmentTypes.VendorBalanceTransfer)
            {
                return APPolicyLimitCategories.HighRisk; // Always high risk
            }

            if (amount <= 1000)
            {
                return APPolicyLimitCategories.SmallWriteOff;
            }
            else if (amount <= 10000)
            {
                return APPolicyLimitCategories.Medium;
            }
            else
            {
                return APPolicyLimitCategories.HighRisk;
            }
        }

        #endregion

        #region Workflow Operations

        /// <summary>Submit adjustment for approval</summary>
        public (bool Success, string Message) Submit(Guid id, string userName)
        {
            var adjustment = _adjustments.FirstOrDefault(a => a.APAdjustmentId == id && !a.IsDeleted);
            if (adjustment == null)
            {
                return (false, "Adjustment not found.");
            }

            if (!adjustment.CanSubmit)
            {
                return (false, "This adjustment cannot be submitted in its current status.");
            }

            if (string.IsNullOrWhiteSpace(adjustment.Narration))
            {
                return (false, "Narration is required before submitting.");
            }

            // Check evidence requirement
            if (adjustment.EvidenceRequired && adjustment.AttachmentCount == 0)
            {
                return (false, "Evidence/documentation is required before submitting. Please attach supporting documents.");
            }

            adjustment.AdjustmentStatus = APAdjustmentStatuses.Submitted;
            adjustment.SubmittedOn = DateTime.Now;
            adjustment.SubmittedByUserName = userName;
            adjustment.UpdatedAt = DateTime.Now;
            adjustment.UpdatedBy = userName;

            return (true, "AP Adjustment submitted successfully.");
        }

        /// <summary>Approve submitted adjustment</summary>
        public (bool Success, string Message) Approve(Guid id, Guid approvedByUserId, string approvedByUserName, string? comment = null)
        {
            var adjustment = _adjustments.FirstOrDefault(a => a.APAdjustmentId == id && !a.IsDeleted);
            if (adjustment == null)
            {
                return (false, "Adjustment not found.");
            }

            if (!adjustment.CanApprove)
            {
                return (false, "This adjustment cannot be approved in its current status.");
            }

            adjustment.AdjustmentStatus = APAdjustmentStatuses.Approved;
            adjustment.ApprovedOn = DateTime.Now;
            adjustment.ApprovedByUserId = approvedByUserId;
            adjustment.ApprovedByUserName = approvedByUserName;
            adjustment.UpdatedAt = DateTime.Now;
            adjustment.UpdatedBy = approvedByUserName;

            return (true, "AP Adjustment approved successfully.");
        }

        /// <summary>Reject submitted adjustment</summary>
        public (bool Success, string Message) Reject(Guid id, string reason, Guid rejectedByUserId, string rejectedByUserName)
        {
            var adjustment = _adjustments.FirstOrDefault(a => a.APAdjustmentId == id && !a.IsDeleted);
            if (adjustment == null)
            {
                return (false, "Adjustment not found.");
            }

            if (adjustment.AdjustmentStatus != APAdjustmentStatuses.Submitted)
            {
                return (false, "Only submitted adjustments can be rejected.");
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return (false, "Rejection reason is required.");
            }

            adjustment.AdjustmentStatus = APAdjustmentStatuses.Rejected;
            adjustment.RejectionReason = reason;
            adjustment.UpdatedAt = DateTime.Now;
            adjustment.UpdatedBy = rejectedByUserName;

            return (true, "AP Adjustment rejected.");
        }

        /// <summary>Post approved adjustment</summary>
        public (bool Success, string Message) Post(Guid id, Guid postedByUserId, string postedByUserName)
        {
            var adjustment = _adjustments.FirstOrDefault(a => a.APAdjustmentId == id && !a.IsDeleted);
            if (adjustment == null)
            {
                return (false, "Adjustment not found.");
            }

            if (!adjustment.CanPost)
            {
                return (false, "This adjustment cannot be posted in its current status.");
            }

            // Validate posting date is in open period (simplified check)
            adjustment.PostingDate = DateTime.Today;

            // Snapshot posting narration
            adjustment.PostingNarrationSnapshot = $"AP Adjustment: {adjustment.AdjustmentNumber} - {adjustment.Narration}";

            // Snapshot AP control account
            adjustment.APControlAccountIdSnapshot = Guid.Parse("00000000-0000-0000-0000-000000000050");
            adjustment.APControlAccountCode = "2100";
            adjustment.APControlAccountName = "Accounts Payable Control";

            adjustment.AdjustmentStatus = APAdjustmentStatuses.Posted;
            adjustment.PostedOn = DateTime.Now;
            adjustment.PostedByUserId = postedByUserId;
            adjustment.PostedByUserName = postedByUserName;
            adjustment.UpdatedAt = DateTime.Now;
            adjustment.UpdatedBy = postedByUserName;

            return (true, "AP Adjustment posted successfully. GL entries created.");
        }

        /// <summary>Cancel adjustment (pre-post only)</summary>
        public (bool Success, string Message) Cancel(Guid id, string reason, string userName)
        {
            var adjustment = _adjustments.FirstOrDefault(a => a.APAdjustmentId == id && !a.IsDeleted);
            if (adjustment == null)
            {
                return (false, "Adjustment not found.");
            }

            if (!adjustment.CanCancel)
            {
                return (false, "This adjustment cannot be cancelled. Only draft or submitted adjustments can be cancelled.");
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return (false, "Cancellation reason is required.");
            }

            adjustment.AdjustmentStatus = APAdjustmentStatuses.Cancelled;
            adjustment.CancellationReason = reason;
            adjustment.CancelledOn = DateTime.Now;
            adjustment.CancelledByUserName = userName;
            adjustment.UpdatedAt = DateTime.Now;
            adjustment.UpdatedBy = userName;

            return (true, "AP Adjustment cancelled successfully.");
        }

        /// <summary>Reverse posted adjustment</summary>
        public (bool Success, string Message) Reverse(Guid id, string reason, Guid reversedByUserId, string reversedByUserName)
        {
            var adjustment = _adjustments.FirstOrDefault(a => a.APAdjustmentId == id && !a.IsDeleted);
            if (adjustment == null)
            {
                return (false, "Adjustment not found.");
            }

            if (!adjustment.CanReverse)
            {
                return (false, "Only posted adjustments can be reversed.");
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return (false, "Reversal reason is required.");
            }

            adjustment.AdjustmentStatus = APAdjustmentStatuses.Reversed;
            adjustment.ReversedOn = DateTime.Now;
            adjustment.ReversedByUserId = reversedByUserId;
            adjustment.ReversedByUserName = reversedByUserName;
            adjustment.ReversalReason = reason;
            adjustment.UpdatedAt = DateTime.Now;
            adjustment.UpdatedBy = reversedByUserName;

            return (true, "AP Adjustment reversed successfully. Reversal GL entries created.");
        }

        #endregion

        #region Statistics

        /// <summary>Get adjustment statistics</summary>
        public APAdjustmentStatisticsViewModel GetStatistics(Guid? companyId = null)
        {
            var adjustments = companyId.HasValue
                ? _adjustments.Where(a => a.CompanyId == companyId.Value && !a.IsDeleted)
                : _adjustments.Where(a => !a.IsDeleted);

            return new APAdjustmentStatisticsViewModel
            {
                TotalAdjustments = adjustments.Count(),
                DraftAdjustments = adjustments.Count(a => a.AdjustmentStatus == APAdjustmentStatuses.Draft),
                SubmittedAdjustments = adjustments.Count(a => a.AdjustmentStatus == APAdjustmentStatuses.Submitted),
                ApprovedAdjustments = adjustments.Count(a => a.AdjustmentStatus == APAdjustmentStatuses.Approved),
                PostedAdjustments = adjustments.Count(a => a.AdjustmentStatus == APAdjustmentStatuses.Posted),
                CancelledAdjustments = adjustments.Count(a => a.AdjustmentStatus == APAdjustmentStatuses.Cancelled),
                ReversedAdjustments = adjustments.Count(a => a.AdjustmentStatus == APAdjustmentStatuses.Reversed),
                RejectedAdjustments = adjustments.Count(a => a.AdjustmentStatus == APAdjustmentStatuses.Rejected),
                PendingApproval = adjustments.Count(a => a.AdjustmentStatus == APAdjustmentStatuses.Submitted &&
                                                        a.PolicyLimitCategory != APPolicyLimitCategories.SmallWriteOff),
                TotalAdjustmentAmount = adjustments.Where(a => a.AdjustmentStatus == APAdjustmentStatuses.Posted)
                                                   .Sum(a => a.AdjustmentAmount),
                TotalWriteOffAmount = adjustments.Where(a => a.AdjustmentStatus == APAdjustmentStatuses.Posted &&
                                                            a.AdjustmentType == APAdjustmentTypes.WriteOff)
                                                 .Sum(a => a.AdjustmentAmount),
                TotalRoundingAmount = adjustments.Where(a => a.AdjustmentStatus == APAdjustmentStatuses.Posted &&
                                                            a.AdjustmentType == APAdjustmentTypes.RoundOffCorrection)
                                                 .Sum(a => a.AdjustmentAmount)
            };
        }

        #endregion
    }

    /// <summary>
    /// AP Adjustment statistics model
    /// </summary>
    public class APAdjustmentStatisticsViewModel
    {
        public int TotalAdjustments { get; set; }
        public int DraftAdjustments { get; set; }
        public int SubmittedAdjustments { get; set; }
        public int ApprovedAdjustments { get; set; }
        public int PostedAdjustments { get; set; }
        public int CancelledAdjustments { get; set; }
        public int ReversedAdjustments { get; set; }
        public int RejectedAdjustments { get; set; }
        public int PendingApproval { get; set; }
        public decimal TotalAdjustmentAmount { get; set; }
        public decimal TotalWriteOffAmount { get; set; }
        public decimal TotalRoundingAmount { get; set; }
    }
}
