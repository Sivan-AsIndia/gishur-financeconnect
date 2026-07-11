using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    /// <summary>
    /// Service for ARAdjustment (Model #32) CRUD operations
    /// Demo application - data stored in memory
    /// </summary>
    public class ARAdjustmentService
    {
        // Immutable seed data
        private static readonly List<ARAdjustmentViewModel> _seedAdjustments = ARAdjustmentSeedData.GetSeedAdjustments();
        private static readonly List<ARAdjustmentReasonViewModel> _seedReasons = ARAdjustmentSeedData.GetSeedReasons();

        // Working (mutable) data
        private List<ARAdjustmentViewModel> _adjustments;
        private List<ARAdjustmentReasonViewModel> _reasons;

        // Counter for adjustment number generation
        private int _adjustmentCounter = 100;

        public ARAdjustmentService()
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
        public List<ARAdjustmentReasonViewModel> GetAllReasons()
        {
            return _reasons.Where(r => r.IsActive).ToList();
        }

        /// <summary>Get reason code by ID</summary>
        public ARAdjustmentReasonViewModel? GetReasonById(Guid id)
        {
            return _reasons.FirstOrDefault(r => r.Id == id);
        }

        /// <summary>Get reason codes by adjustment type</summary>
        public List<ARAdjustmentReasonViewModel> GetReasonsByType(string adjustmentType)
        {
            return _reasons.Where(r => r.IsActive && 
                (r.ApplicableTypes.Contains(adjustmentType) || r.ApplicableTypes.Contains(AdjustmentTypes.Other)))
                .ToList();
        }

        #endregion

        #region Read Operations

        /// <summary>Get all adjustments</summary>
        public List<ARAdjustmentViewModel> GetAll()
        {
            return _adjustments.Where(a => !a.IsDeleted).ToList();
        }

        /// <summary>Get adjustment by ID</summary>
        public ARAdjustmentViewModel? GetById(Guid id)
        {
            return _adjustments.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
        }

        /// <summary>Get adjustments by company ID</summary>
        public List<ARAdjustmentViewModel> GetByCompanyId(Guid companyId)
        {
            return _adjustments.Where(a => a.CompanyId == companyId && !a.IsDeleted).ToList();
        }

        /// <summary>Get adjustments by customer ID</summary>
        public List<ARAdjustmentViewModel> GetByCustomerId(Guid customerId)
        {
            return _adjustments.Where(a => a.CustomerId == customerId && !a.IsDeleted).ToList();
        }

        /// <summary>Get adjustments by status</summary>
        public List<ARAdjustmentViewModel> GetByStatus(string status)
        {
            return _adjustments.Where(a => a.AdjustmentStatus == status && !a.IsDeleted).ToList();
        }

        /// <summary>Get adjustments by type</summary>
        public List<ARAdjustmentViewModel> GetByType(string type)
        {
            return _adjustments.Where(a => a.AdjustmentType == type && !a.IsDeleted).ToList();
        }

        /// <summary>Get adjustments by branch ID</summary>
        public List<ARAdjustmentViewModel> GetByBranchId(Guid branchId)
        {
            return _adjustments.Where(a => a.BranchId == branchId && !a.IsDeleted).ToList();
        }

        /// <summary>Get adjustments by date range</summary>
        public List<ARAdjustmentViewModel> GetByDateRange(DateTime fromDate, DateTime toDate)
        {
            return _adjustments.Where(a =>
                a.AdjustmentDate >= fromDate &&
                a.AdjustmentDate <= toDate &&
                !a.IsDeleted).ToList();
        }

        /// <summary>Get adjustments pending approval</summary>
        public List<ARAdjustmentViewModel> GetPendingApproval()
        {
            return _adjustments.Where(a => 
                a.AdjustmentStatus == AdjustmentStatuses.Submitted && 
                a.RequiresApproval && 
                a.ApprovalStatus == ARAdjustmentApprovalStatuses.Pending &&
                !a.IsDeleted).ToList();
        }

        /// <summary>Search adjustments by number, customer code/name</summary>
        public List<ARAdjustmentViewModel> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAll();

            searchTerm = searchTerm.ToLower();
            return _adjustments.Where(a => !a.IsDeleted && (
                a.AdjustmentNumber.ToLower().Contains(searchTerm) ||
                (a.CustomerCode?.ToLower().Contains(searchTerm) ?? false) ||
                (a.CustomerName?.ToLower().Contains(searchTerm) ?? false) ||
                (a.ReasonCode?.ToLower().Contains(searchTerm) ?? false) ||
                (a.AdjustmentNarration?.ToLower().Contains(searchTerm) ?? false)
            )).ToList();
        }

        /// <summary>Check if adjustment number exists within company</summary>
        public bool AdjustmentNumberExists(Guid companyId, string adjustmentNumber, Guid? excludeId = null)
        {
            return _adjustments.Any(a =>
                a.CompanyId == companyId &&
                a.AdjustmentNumber.Equals(adjustmentNumber, StringComparison.OrdinalIgnoreCase) &&
                !a.IsDeleted &&
                (excludeId == null || a.Id != excludeId));
        }

        /// <summary>Generate next adjustment number</summary>
        public string GenerateAdjustmentNumber(Guid companyId)
        {
            _adjustmentCounter++;
            var year = DateTime.Today.Year;
            return $"ADJ-{year}-{_adjustmentCounter:D4}";
        }

        #endregion

        #region Write Operations

        /// <summary>Add new adjustment</summary>
        public (bool Success, string Message) Add(ARAdjustmentViewModel adjustment)
        {
            // Validate adjustment number uniqueness
            if (AdjustmentNumberExists(adjustment.CompanyId, adjustment.AdjustmentNumber))
            {
                return (false, "Adjustment number already exists.");
            }

            // Validate customer
            if (adjustment.CustomerId == Guid.Empty)
            {
                return (false, "Customer is required.");
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

            // Validate narration for specific types
            if (adjustment.IsNarrationRequired && string.IsNullOrWhiteSpace(adjustment.AdjustmentNarration))
            {
                return (false, "Narration is required for write-off and dispute adjustments.");
            }

            adjustment.Id = Guid.NewGuid();
            adjustment.AdjustmentNumber = adjustment.AdjustmentNumber.ToUpper().Trim();
            adjustment.CreatedAt = DateTime.Now;
            adjustment.IsDeleted = false;

            // Generate adjustment number if empty
            if (string.IsNullOrWhiteSpace(adjustment.AdjustmentNumber))
            {
                adjustment.AdjustmentNumber = GenerateAdjustmentNumber(adjustment.CompanyId);
            }

            // Set line adjustment IDs
            foreach (var line in adjustment.Lines)
            {
                line.Id = Guid.NewGuid();
                line.ARAdjustmentId = adjustment.Id;
                line.CreatedAt = DateTime.Now;
                line.CreatedBy = adjustment.CreatedBy;
            }

            // Recalculate totals
            adjustment.RecalculateTotals();

            // Set approval requirements based on reason code
            var reason = GetReasonById(adjustment.ReasonCodeId);
            if (reason != null)
            {
                adjustment.RequiresApproval = reason.RequiresApproval || 
                    (reason.ApprovalThreshold.HasValue && adjustment.TotalAdjustmentAmount > reason.ApprovalThreshold.Value);
                adjustment.EvidenceRequired = reason.RequiresEvidence;
                
                if (adjustment.RequiresApproval)
                {
                    adjustment.ApprovalStatus = ARAdjustmentApprovalStatuses.NotRequired; // Will change to Pending on submit
                }
            }

            _adjustments.Add(adjustment);
            return (true, "Adjustment created successfully.");
        }

        /// <summary>Update existing adjustment</summary>
        public (bool Success, string Message) Update(ARAdjustmentViewModel adjustment)
        {
            var existing = _adjustments.FirstOrDefault(a => a.Id == adjustment.Id && !a.IsDeleted);
            if (existing == null)
            {
                return (false, "Adjustment not found.");
            }

            if (!existing.CanEdit)
            {
                return (false, "Only draft adjustments can be edited.");
            }

            // Validate adjustment number uniqueness
            if (AdjustmentNumberExists(adjustment.CompanyId, adjustment.AdjustmentNumber, adjustment.Id))
            {
                return (false, "Adjustment number already exists.");
            }

            // Update fields
            existing.BranchId = adjustment.BranchId;
            existing.BranchCode = adjustment.BranchCode;
            existing.BranchName = adjustment.BranchName;
            existing.CustomerId = adjustment.CustomerId;
            existing.CustomerCode = adjustment.CustomerCode;
            existing.CustomerName = adjustment.CustomerName;
            existing.CustomerAccountId = adjustment.CustomerAccountId;
            existing.CustomerAccountName = adjustment.CustomerAccountName;
            existing.AdjustmentDate = adjustment.AdjustmentDate;
            existing.CurrencyId = adjustment.CurrencyId;
            existing.CurrencyCode = adjustment.CurrencyCode;
            existing.CurrencyName = adjustment.CurrencyName;
            existing.AdjustmentNarration = adjustment.AdjustmentNarration;
            existing.AdjustmentType = adjustment.AdjustmentType;
            existing.ReasonCodeId = adjustment.ReasonCodeId;
            existing.ReasonCode = adjustment.ReasonCode;
            existing.ReasonDescription = adjustment.ReasonDescription;
            existing.Lines = adjustment.Lines;
            existing.UpdatedAt = DateTime.Now;
            existing.UpdatedBy = adjustment.UpdatedBy;

            // Recalculate totals
            existing.RecalculateTotals();

            // Update approval requirements
            var reason = GetReasonById(existing.ReasonCodeId);
            if (reason != null)
            {
                existing.RequiresApproval = reason.RequiresApproval || 
                    (reason.ApprovalThreshold.HasValue && existing.TotalAdjustmentAmount > reason.ApprovalThreshold.Value);
                existing.EvidenceRequired = reason.RequiresEvidence;
            }

            return (true, "Adjustment updated successfully.");
        }

        /// <summary>Delete adjustment (soft delete, draft only)</summary>
        public (bool Success, string Message) Delete(Guid id, string userName)
        {
            var adjustment = _adjustments.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
            if (adjustment == null)
            {
                return (false, "Adjustment not found.");
            }

            if (adjustment.AdjustmentStatus != AdjustmentStatuses.Draft)
            {
                return (false, "Only draft adjustments can be deleted.");
            }

            adjustment.IsDeleted = true;
            adjustment.DeletedAt = DateTime.Now;
            adjustment.DeletedBy = userName;

            return (true, "Adjustment deleted successfully.");
        }

        #endregion

        #region Workflow Operations

        /// <summary>Submit adjustment for approval</summary>
        public (bool Success, string Message) Submit(Guid id, string userName)
        {
            var adjustment = _adjustments.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
            if (adjustment == null)
            {
                return (false, "Adjustment not found.");
            }

            if (adjustment.AdjustmentStatus != AdjustmentStatuses.Draft)
            {
                return (false, "Only draft adjustments can be submitted.");
            }

            // Validate at least one line
            if (!adjustment.Lines.Any())
            {
                return (false, "Adjustment must have at least one line.");
            }

            // Validate total amount
            if (adjustment.TotalAdjustmentAmount <= 0)
            {
                return (false, "Adjustment total must be greater than zero.");
            }

            // Validate evidence if required
            if (adjustment.EvidenceRequired && adjustment.EvidenceAttachmentCount == 0)
            {
                return (false, "Evidence attachment is required for this adjustment type.");
            }

            adjustment.AdjustmentStatus = AdjustmentStatuses.Submitted;
            if (adjustment.RequiresApproval)
            {
                adjustment.ApprovalStatus = ARAdjustmentApprovalStatuses.Pending;
            }
            adjustment.UpdatedAt = DateTime.Now;
            adjustment.UpdatedBy = userName;

            return (true, "Adjustment submitted successfully.");
        }

        /// <summary>Approve adjustment</summary>
        public (bool Success, string Message) Approve(Guid id, Guid approverUserId, string approverName, string? comment = null)
        {
            var adjustment = _adjustments.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
            if (adjustment == null)
            {
                return (false, "Adjustment not found.");
            }

            if (!adjustment.CanApprove)
            {
                return (false, "This adjustment cannot be approved in its current state.");
            }

            adjustment.ApprovalStatus = ARAdjustmentApprovalStatuses.Approved;
            adjustment.ApprovedByUserId = approverUserId;
            adjustment.ApprovedByUserName = approverName;
            adjustment.ApprovedOn = DateTime.Now;
            adjustment.ApprovalComment = comment;
            adjustment.AdjustmentStatus = AdjustmentStatuses.Approved;
            adjustment.UpdatedAt = DateTime.Now;
            adjustment.UpdatedBy = approverName;

            return (true, "Adjustment approved successfully.");
        }

        /// <summary>Reject adjustment</summary>
        public (bool Success, string Message) Reject(Guid id, Guid rejecterUserId, string rejecterName, string reason)
        {
            var adjustment = _adjustments.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
            if (adjustment == null)
            {
                return (false, "Adjustment not found.");
            }

            if (adjustment.AdjustmentStatus != AdjustmentStatuses.Submitted)
            {
                return (false, "Only submitted adjustments can be rejected.");
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return (false, "Rejection reason is required.");
            }

            adjustment.ApprovalStatus = ARAdjustmentApprovalStatuses.Rejected;
            adjustment.ApprovalComment = reason;
            adjustment.AdjustmentStatus = AdjustmentStatuses.Draft;
            adjustment.AdjustmentNarration = $"{adjustment.AdjustmentNarration}\n[Rejected: {reason}]";
            adjustment.UpdatedAt = DateTime.Now;
            adjustment.UpdatedBy = rejecterName;

            return (true, "Adjustment rejected and returned to draft.");
        }

        /// <summary>Post adjustment</summary>
        public (bool Success, string Message) Post(Guid id, Guid postedByUserId, string postedByUserName)
        {
            var adjustment = _adjustments.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
            if (adjustment == null)
            {
                return (false, "Adjustment not found.");
            }

            if (!adjustment.CanPost)
            {
                return (false, "This adjustment cannot be posted in its current state.");
            }

            // Validate at least one line
            if (!adjustment.Lines.Any())
            {
                return (false, "Adjustment must have at least one line.");
            }

            // Validate total amount
            if (adjustment.TotalAdjustmentAmount <= 0)
            {
                return (false, "Adjustment total must be greater than zero.");
            }

            // Validate all lines
            foreach (var line in adjustment.Lines)
            {
                if (line.AdjustmentAmount <= 0)
                {
                    return (false, $"Line {line.LineNumber}: Adjustment amount must be greater than zero.");
                }
                if (line.OffsetAccountId == Guid.Empty)
                {
                    return (false, $"Line {line.LineNumber}: Offset account is required.");
                }
            }

            // Snapshot receivable account
            adjustment.ReceivableAccountIdSnapshot = Guid.Parse("00000000-0000-0000-0000-000000000001");
            adjustment.ReceivableAccountCode = "1100";
            adjustment.ReceivableAccountName = "Accounts Receivable";

            adjustment.AdjustmentStatus = AdjustmentStatuses.Posted;
            adjustment.PostingDate = DateTime.Today;
            adjustment.PostedOn = DateTime.Now;
            adjustment.PostedByUserId = postedByUserId;
            adjustment.PostedByUserName = postedByUserName;
            adjustment.UpdatedAt = DateTime.Now;
            adjustment.UpdatedBy = postedByUserName;

            return (true, "Adjustment posted successfully. GL entries created.");
        }

        /// <summary>Cancel adjustment (pre-post only)</summary>
        public (bool Success, string Message) Cancel(Guid id, string reason, string userName)
        {
            var adjustment = _adjustments.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
            if (adjustment == null)
            {
                return (false, "Adjustment not found.");
            }

            if (!adjustment.CanCancel)
            {
                return (false, "This adjustment cannot be cancelled in its current state.");
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return (false, "Cancellation reason is required.");
            }

            adjustment.AdjustmentStatus = AdjustmentStatuses.Cancelled;
            adjustment.CancelledOn = DateTime.Now;
            adjustment.CancelledByUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            adjustment.CancelledByUserName = userName;
            adjustment.CancellationReason = reason;
            adjustment.UpdatedAt = DateTime.Now;
            adjustment.UpdatedBy = userName;

            return (true, "Adjustment cancelled successfully.");
        }

        /// <summary>Reverse posted adjustment</summary>
        public (bool Success, string Message) Reverse(Guid id, string reason, Guid reversedByUserId, string reversedByUserName)
        {
            var adjustment = _adjustments.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
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

            adjustment.AdjustmentStatus = AdjustmentStatuses.Reversed;
            adjustment.ReversedOn = DateTime.Now;
            adjustment.ReversedByUserId = reversedByUserId;
            adjustment.ReversedByUserName = reversedByUserName;
            adjustment.ReversalReason = reason;
            adjustment.UpdatedAt = DateTime.Now;
            adjustment.UpdatedBy = reversedByUserName;

            return (true, "Adjustment reversed successfully. Reversal GL entries created.");
        }

        #endregion

        #region Line Operations

        /// <summary>Add line to adjustment</summary>
        public (bool Success, string Message) AddLine(Guid adjustmentId, ARAdjustmentLineViewModel line)
        {
            var adjustment = _adjustments.FirstOrDefault(a => a.Id == adjustmentId && !a.IsDeleted);
            if (adjustment == null)
            {
                return (false, "Adjustment not found.");
            }

            if (!adjustment.CanEdit)
            {
                return (false, "Lines cannot be added to posted or cancelled adjustments.");
            }

            // Validate line
            if (line.AdjustmentAmount <= 0)
            {
                return (false, "Adjustment amount must be greater than zero.");
            }

            if (line.OffsetAccountId == Guid.Empty)
            {
                return (false, "Offset account is required.");
            }

            // Set line number
            var maxLineNumber = adjustment.Lines.Any() ? adjustment.Lines.Max(l => l.LineNumber) : 0;
            line.LineNumber = maxLineNumber + 10;
            line.Id = Guid.NewGuid();
            line.ARAdjustmentId = adjustmentId;
            line.CreatedAt = DateTime.Now;

            adjustment.Lines.Add(line);
            adjustment.RecalculateTotals();
            adjustment.UpdatedAt = DateTime.Now;

            return (true, "Line added successfully.");
        }

        /// <summary>Update line in adjustment</summary>
        public (bool Success, string Message) UpdateLine(ARAdjustmentLineViewModel line)
        {
            var adjustment = _adjustments.FirstOrDefault(a => a.Id == line.ARAdjustmentId && !a.IsDeleted);
            if (adjustment == null)
            {
                return (false, "Adjustment not found.");
            }

            if (!adjustment.CanEdit)
            {
                return (false, "Lines cannot be updated on posted or cancelled adjustments.");
            }

            var existingLine = adjustment.Lines.FirstOrDefault(l => l.Id == line.Id);
            if (existingLine == null)
            {
                return (false, "Line not found.");
            }

            // Update line fields
            existingLine.CustomerInvoiceId = line.CustomerInvoiceId;
            existingLine.CustomerInvoiceNumber = line.CustomerInvoiceNumber;
            existingLine.InvoiceOutstanding = line.InvoiceOutstanding;
            existingLine.LineType = line.LineType;
            existingLine.AdjustmentAmount = line.AdjustmentAmount;
            existingLine.OffsetAccountId = line.OffsetAccountId;
            existingLine.OffsetAccountCode = line.OffsetAccountCode;
            existingLine.OffsetAccountName = line.OffsetAccountName;
            existingLine.LineNarration = line.LineNarration;
            existingLine.UpdatedAt = DateTime.Now;
            existingLine.UpdatedBy = line.UpdatedBy;

            // Recalculate adjustment totals
            adjustment.RecalculateTotals();
            adjustment.UpdatedAt = DateTime.Now;

            return (true, "Line updated successfully.");
        }

        /// <summary>Delete line from adjustment</summary>
        public (bool Success, string Message) DeleteLine(Guid adjustmentId, Guid lineId)
        {
            var adjustment = _adjustments.FirstOrDefault(a => a.Id == adjustmentId && !a.IsDeleted);
            if (adjustment == null)
            {
                return (false, "Adjustment not found.");
            }

            if (!adjustment.CanEdit)
            {
                return (false, "Lines cannot be deleted from posted or cancelled adjustments.");
            }

            var line = adjustment.Lines.FirstOrDefault(l => l.Id == lineId);
            if (line == null)
            {
                return (false, "Line not found.");
            }

            adjustment.Lines.Remove(line);
            adjustment.RecalculateTotals();
            adjustment.UpdatedAt = DateTime.Now;

            return (true, "Line deleted successfully.");
        }

        /// <summary>Get lines for adjustment</summary>
        public List<ARAdjustmentLineViewModel> GetLines(Guid adjustmentId)
        {
            var adjustment = _adjustments.FirstOrDefault(a => a.Id == adjustmentId && !a.IsDeleted);
            return adjustment?.Lines ?? new List<ARAdjustmentLineViewModel>();
        }

        #endregion

        #region Statistics

        /// <summary>Get adjustment statistics</summary>
        public ARAdjustmentStatisticsViewModel GetStatistics(Guid? companyId = null)
        {
            var adjustments = companyId.HasValue
                ? _adjustments.Where(a => a.CompanyId == companyId.Value && !a.IsDeleted)
                : _adjustments.Where(a => !a.IsDeleted);

            return new ARAdjustmentStatisticsViewModel
            {
                TotalAdjustments = adjustments.Count(),
                DraftAdjustments = adjustments.Count(a => a.AdjustmentStatus == AdjustmentStatuses.Draft),
                SubmittedAdjustments = adjustments.Count(a => a.AdjustmentStatus == AdjustmentStatuses.Submitted),
                ApprovedAdjustments = adjustments.Count(a => a.AdjustmentStatus == AdjustmentStatuses.Approved),
                PostedAdjustments = adjustments.Count(a => a.AdjustmentStatus == AdjustmentStatuses.Posted),
                CancelledAdjustments = adjustments.Count(a => a.AdjustmentStatus == AdjustmentStatuses.Cancelled),
                ReversedAdjustments = adjustments.Count(a => a.AdjustmentStatus == AdjustmentStatuses.Reversed),
                PendingApproval = adjustments.Count(a => a.AdjustmentStatus == AdjustmentStatuses.Submitted && a.RequiresApproval),
                TotalAdjustmentAmount = adjustments.Where(a => a.AdjustmentStatus == AdjustmentStatuses.Posted).Sum(a => a.TotalAdjustmentAmount),
                TotalWriteOffAmount = adjustments.Where(a => a.AdjustmentStatus == AdjustmentStatuses.Posted && a.AdjustmentType == AdjustmentTypes.WriteOff).Sum(a => a.TotalAdjustmentAmount),
                TotalRoundingAmount = adjustments.Where(a => a.AdjustmentStatus == AdjustmentStatuses.Posted && a.AdjustmentType == AdjustmentTypes.Rounding).Sum(a => a.TotalAdjustmentAmount)
            };
        }

        #endregion
    }
}
