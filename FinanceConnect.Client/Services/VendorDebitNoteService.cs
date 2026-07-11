using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    /// <summary>
    /// Service for VendorDebitNote (Model #40) CRUD operations
    /// Demo application - data stored in memory
    /// </summary>
    public class VendorDebitNoteService
    {
        // Immutable seed data
        private static readonly List<VendorDebitNoteViewModel> _seedDebitNotes = VendorDebitNoteSeedData.GetSeedDebitNotes();

        // Working (mutable) data
        private List<VendorDebitNoteViewModel> _debitNotes;

        // Counter for debit note number generation
        private int _debitNoteCounter = 100;

        public VendorDebitNoteService()
        {
            ResetToSeed();
        }

        private static List<T> CloneList<T>(IEnumerable<T> source) where T : class
        {
            var json = System.Text.Json.JsonSerializer.Serialize(source);
            return System.Text.Json.JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }

        /// <summary>Reset debit notes to seed data</summary>
        public void ResetToSeed()
        {
            _debitNotes = CloneList(_seedDebitNotes);
        }

        #region Read Operations

        /// <summary>Get all debit notes</summary>
        public List<VendorDebitNoteViewModel> GetAll()
        {
            return _debitNotes.Where(d => !d.IsDeleted).ToList();
        }

        /// <summary>Get debit note by ID</summary>
        public VendorDebitNoteViewModel? GetById(Guid id)
        {
            return _debitNotes.FirstOrDefault(d => d.Id == id && !d.IsDeleted);
        }

        /// <summary>Get debit notes by company ID</summary>
        public List<VendorDebitNoteViewModel> GetByCompanyId(Guid companyId)
        {
            return _debitNotes.Where(d => d.CompanyId == companyId && !d.IsDeleted).ToList();
        }

        /// <summary>Get debit notes by vendor ID</summary>
        public List<VendorDebitNoteViewModel> GetByVendorId(Guid vendorId)
        {
            return _debitNotes.Where(d => d.VendorId == vendorId && !d.IsDeleted).ToList();
        }

        /// <summary>Get debit notes by status</summary>
        public List<VendorDebitNoteViewModel> GetByStatus(string status)
        {
            return _debitNotes.Where(d => d.DebitNoteStatus == status && !d.IsDeleted).ToList();
        }

        /// <summary>Get debit notes by branch ID</summary>
        public List<VendorDebitNoteViewModel> GetByBranchId(Guid branchId)
        {
            return _debitNotes.Where(d => d.BranchId == branchId && !d.IsDeleted).ToList();
        }

        /// <summary>Get debit notes by bill ID</summary>
        public List<VendorDebitNoteViewModel> GetByBillId(Guid billId)
        {
            return _debitNotes.Where(d => d.PrimaryVendorBillId == billId && !d.IsDeleted).ToList();
        }

        /// <summary>Get debit notes by date range</summary>
        public List<VendorDebitNoteViewModel> GetByDateRange(DateTime fromDate, DateTime toDate)
        {
            return _debitNotes.Where(d =>
                d.VendorDebitNoteDate >= fromDate &&
                d.VendorDebitNoteDate <= toDate &&
                !d.IsDeleted).ToList();
        }

        /// <summary>Get debit notes by type</summary>
        public List<VendorDebitNoteViewModel> GetByType(string debitNoteType)
        {
            return _debitNotes.Where(d => d.DebitNoteType == debitNoteType && !d.IsDeleted).ToList();
        }

        /// <summary>Search debit notes by number, vendor code/name, reference</summary>
        public List<VendorDebitNoteViewModel> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAll();

            searchTerm = searchTerm.ToLower();
            return _debitNotes.Where(d => !d.IsDeleted && (
                d.DebitNoteNumber.ToLower().Contains(searchTerm) ||
                (d.VendorCode?.ToLower().Contains(searchTerm) ?? false) ||
                (d.VendorName?.ToLower().Contains(searchTerm) ?? false) ||
                (d.VendorDebitNoteReferenceNumber?.ToLower().Contains(searchTerm) ?? false) ||
                (d.PrimaryVendorBillNumber?.ToLower().Contains(searchTerm) ?? false)
            )).ToList();
        }

        /// <summary>Check if debit note number exists within company</summary>
        public bool DebitNoteNumberExists(Guid companyId, string debitNoteNumber, Guid? excludeId = null)
        {
            return _debitNotes.Any(d =>
                d.CompanyId == companyId &&
                d.DebitNoteNumber.Equals(debitNoteNumber, StringComparison.OrdinalIgnoreCase) &&
                !d.IsDeleted &&
                (excludeId == null || d.Id != excludeId));
        }

        /// <summary>Check if vendor reference number exists for a vendor (anti-fraud check)</summary>
        public bool VendorReferenceNumberExists(Guid vendorId, string referenceNumber, Guid? excludeId = null)
        {
            return _debitNotes.Any(d =>
                d.VendorId == vendorId &&
                d.VendorDebitNoteReferenceNumber.Equals(referenceNumber, StringComparison.OrdinalIgnoreCase) &&
                !d.IsDeleted &&
                (excludeId == null || d.Id != excludeId));
        }

        /// <summary>Generate next debit note number</summary>
        public string GenerateDebitNoteNumber(Guid companyId)
        {
            _debitNoteCounter++;
            var year = DateTime.Today.Year;
            return $"APDN-{year}-{_debitNoteCounter:D4}";
        }

        #endregion

        #region Write Operations

        /// <summary>Add new debit note</summary>
        public (bool Success, string Message) Add(VendorDebitNoteViewModel debitNote)
        {
            // Validate debit note number uniqueness
            if (!string.IsNullOrWhiteSpace(debitNote.DebitNoteNumber) && 
                DebitNoteNumberExists(debitNote.CompanyId, debitNote.DebitNoteNumber))
            {
                return (false, "Debit note number already exists.");
            }

            // Validate vendor reference number uniqueness (critical anti-fraud check)
            if (VendorReferenceNumberExists(debitNote.VendorId, debitNote.VendorDebitNoteReferenceNumber))
            {
                return (false, "Vendor reference number already exists for this vendor. Duplicate debit note detected.");
            }

            // Validate vendor
            if (debitNote.VendorId == Guid.Empty)
            {
                return (false, "Vendor is required.");
            }

            // Validate branch
            if (debitNote.BranchId == Guid.Empty)
            {
                return (false, "Branch is required.");
            }

            // Validate vendor debit note reference number
            if (string.IsNullOrWhiteSpace(debitNote.VendorDebitNoteReferenceNumber))
            {
                return (false, "Vendor Debit Note Reference Number is required.");
            }

            // Validate debit note type
            if (string.IsNullOrWhiteSpace(debitNote.DebitNoteType))
            {
                return (false, "Debit note type is required.");
            }

            // Validate bill reference if against bill
            if (debitNote.IsAgainstBill && debitNote.PrimaryVendorBillId == null)
            {
                return (false, "Reference bill is required when 'Against Bill' is enabled.");
            }

            // Validate at least one line for non-draft debit notes
            if (debitNote.DebitNoteStatus != VendorDebitNoteStatuses.Draft && !debitNote.Lines.Any())
            {
                return (false, "Debit note must have at least one line.");
            }

            // Validate totals
            if (debitNote.DebitNoteStatus != VendorDebitNoteStatuses.Draft && debitNote.TotalDebitAmount <= 0)
            {
                return (false, "Debit note total must be greater than zero.");
            }

            debitNote.Id = Guid.NewGuid();
            debitNote.CreatedAt = DateTime.Now;
            debitNote.IsDeleted = false;

            // Generate debit note number if empty
            if (string.IsNullOrWhiteSpace(debitNote.DebitNoteNumber))
            {
                debitNote.DebitNoteNumber = GenerateDebitNoteNumber(debitNote.CompanyId);
            }

            // Set line debit note IDs
            int lineNum = 10;
            foreach (var line in debitNote.Lines)
            {
                line.Id = Guid.NewGuid();
                line.VendorDebitNoteId = debitNote.Id;
                line.LineNumber = lineNum;
                lineNum += 10;
                line.CreatedAt = DateTime.Now;
                line.CreatedBy = debitNote.CreatedBy;
                line.RecalculateAmounts();
            }

            // Recalculate totals
            debitNote.RecalculateTotals();

            _debitNotes.Add(debitNote);
            return (true, "Debit note created successfully.");
        }

        /// <summary>Update existing debit note</summary>
        public (bool Success, string Message) Update(VendorDebitNoteViewModel debitNote)
        {
            var existing = _debitNotes.FirstOrDefault(d => d.Id == debitNote.Id && !d.IsDeleted);
            if (existing == null)
            {
                return (false, "Debit note not found.");
            }

            if (!existing.CanEdit)
            {
                return (false, "Debit note cannot be edited in its current status.");
            }

            // Validate vendor reference number uniqueness
            if (VendorReferenceNumberExists(debitNote.VendorId, debitNote.VendorDebitNoteReferenceNumber, debitNote.Id))
            {
                return (false, "Vendor reference number already exists for this vendor. Duplicate debit note detected.");
            }

            // Update fields
            existing.BranchId = debitNote.BranchId;
            existing.BranchCode = debitNote.BranchCode;
            existing.BranchName = debitNote.BranchName;
            existing.VendorId = debitNote.VendorId;
            existing.VendorCode = debitNote.VendorCode;
            existing.VendorName = debitNote.VendorName;
            existing.VendorDebitNoteReferenceNumber = debitNote.VendorDebitNoteReferenceNumber;
            existing.VendorDebitNoteDate = debitNote.VendorDebitNoteDate;
            existing.DebitEntryDate = debitNote.DebitEntryDate;
            existing.DebitNoteType = debitNote.DebitNoteType;
            existing.ReasonCode = debitNote.ReasonCode;
            existing.IsAgainstBill = debitNote.IsAgainstBill;
            existing.PrimaryVendorBillId = debitNote.PrimaryVendorBillId;
            existing.PrimaryVendorBillNumber = debitNote.PrimaryVendorBillNumber;
            existing.BillNumberSnapshot = debitNote.BillNumberSnapshot;
            existing.BillDateSnapshot = debitNote.BillDateSnapshot;
            existing.CurrencyId = debitNote.CurrencyId;
            existing.CurrencyCode = debitNote.CurrencyCode;
            existing.CurrencyName = debitNote.CurrencyName;
            existing.ExchangeRate = debitNote.ExchangeRate;
            existing.DebitNoteNarration = debitNote.DebitNoteNarration;
            existing.IsGSTApplicable = debitNote.IsGSTApplicable;
            existing.PlaceOfSupplyStateId = debitNote.PlaceOfSupplyStateId;
            existing.PlaceOfSupplyStateCode = debitNote.PlaceOfSupplyStateCode;
            existing.PlaceOfSupplyStateName = debitNote.PlaceOfSupplyStateName;
            existing.IsReverseChargeApplicable = debitNote.IsReverseChargeApplicable;
            existing.IsTDSApplicable = debitNote.IsTDSApplicable;
            existing.ExpenseAccountId = debitNote.ExpenseAccountId;
            existing.ExpenseAccountCode = debitNote.ExpenseAccountCode;
            existing.ExpenseAccountName = debitNote.ExpenseAccountName;
            existing.UpdatedAt = DateTime.Now;
            existing.UpdatedBy = debitNote.UpdatedBy;

            // Update lines
            existing.Lines = debitNote.Lines;
            foreach (var line in existing.Lines)
            {
                if (line.Id == Guid.Empty)
                {
                    line.Id = Guid.NewGuid();
                }
                line.VendorDebitNoteId = existing.Id;
                line.RecalculateAmounts();
            }

            // Recalculate totals
            existing.RecalculateTotals();

            return (true, "Debit note updated successfully.");
        }

        /// <summary>Delete debit note (soft delete)</summary>
        public (bool Success, string Message) Delete(Guid id)
        {
            var debitNote = _debitNotes.FirstOrDefault(d => d.Id == id && !d.IsDeleted);
            if (debitNote == null)
            {
                return (false, "Debit note not found.");
            }

            if (debitNote.DebitNoteStatus != VendorDebitNoteStatuses.Draft)
            {
                return (false, "Only draft debit notes can be deleted.");
            }

            debitNote.IsDeleted = true;
            debitNote.DeletedAt = DateTime.Now;

            return (true, "Debit note deleted successfully.");
        }

        #endregion

        #region Workflow Operations

        /// <summary>Submit debit note for approval</summary>
        public (bool Success, string Message) Submit(Guid id, string userName)
        {
            var debitNote = _debitNotes.FirstOrDefault(d => d.Id == id && !d.IsDeleted);
            if (debitNote == null)
            {
                return (false, "Debit note not found.");
            }

            if (debitNote.DebitNoteStatus != VendorDebitNoteStatuses.Draft)
            {
                return (false, "Only draft debit notes can be submitted.");
            }

            // Validate lines
            if (!debitNote.Lines.Any())
            {
                return (false, "Debit note must have at least one line to submit.");
            }

            // Validate totals
            if (debitNote.TotalDebitAmount <= 0)
            {
                return (false, "Debit note total must be greater than zero.");
            }

            debitNote.DebitNoteStatus = VendorDebitNoteStatuses.Submitted;
            debitNote.SubmittedOn = DateTime.Now;
            debitNote.SubmittedByUserName = userName;
            debitNote.UpdatedAt = DateTime.Now;
            debitNote.UpdatedBy = userName;

            return (true, "Debit note submitted for approval.");
        }

        /// <summary>Approve debit note</summary>
        public (bool Success, string Message) Approve(Guid id, string userName)
        {
            var debitNote = _debitNotes.FirstOrDefault(d => d.Id == id && !d.IsDeleted);
            if (debitNote == null)
            {
                return (false, "Debit note not found.");
            }

            if (debitNote.DebitNoteStatus != VendorDebitNoteStatuses.Submitted)
            {
                return (false, "Only submitted debit notes can be approved.");
            }

            debitNote.DebitNoteStatus = VendorDebitNoteStatuses.Approved;
            debitNote.ApprovedOn = DateTime.Now;
            debitNote.ApprovedByUserName = userName;
            debitNote.UpdatedAt = DateTime.Now;
            debitNote.UpdatedBy = userName;

            return (true, "Debit note approved.");
        }

        /// <summary>Post debit note to GL</summary>
        public (bool Success, string Message) Post(Guid id, Guid journalId, string userName)
        {
            var debitNote = _debitNotes.FirstOrDefault(d => d.Id == id && !d.IsDeleted);
            if (debitNote == null)
            {
                return (false, "Debit note not found.");
            }

            if (!debitNote.CanPost)
            {
                return (false, "Debit note cannot be posted in its current status.");
            }

            // Validate lines
            if (!debitNote.Lines.Any())
            {
                return (false, "Debit note must have at least one line to post.");
            }

            // Validate totals
            if (debitNote.TotalDebitAmount <= 0)
            {
                return (false, "Debit note total must be greater than zero.");
            }

            debitNote.DebitNoteStatus = VendorDebitNoteStatuses.Posted;
            debitNote.PostingDate = DateTime.Today;
            debitNote.PostedOn = DateTime.Now;
            debitNote.PostedByUserName = userName;
            debitNote.UpdatedAt = DateTime.Now;
            debitNote.UpdatedBy = userName;

            return (true, "Debit note posted successfully.");
        }

        /// <summary>Cancel debit note (pre-post)</summary>
        public (bool Success, string Message) Cancel(Guid id, string reason, string userName)
        {
            var debitNote = _debitNotes.FirstOrDefault(d => d.Id == id && !d.IsDeleted);
            if (debitNote == null)
            {
                return (false, "Debit note not found.");
            }

            if (!debitNote.CanCancel)
            {
                return (false, "Debit note cannot be cancelled in its current status.");
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return (false, "Cancellation reason is required.");
            }

            debitNote.DebitNoteStatus = VendorDebitNoteStatuses.Cancelled;
            debitNote.CancellationReason = reason;
            debitNote.CancelledOn = DateTime.Now;
            debitNote.CancelledByUserName = userName;
            debitNote.UpdatedAt = DateTime.Now;
            debitNote.UpdatedBy = userName;

            return (true, "Debit note cancelled successfully.");
        }

        /// <summary>Reverse posted debit note</summary>
        public (bool Success, string Message) Reverse(Guid id, string reason, string userName)
        {
            var debitNote = _debitNotes.FirstOrDefault(d => d.Id == id && !d.IsDeleted);
            if (debitNote == null)
            {
                return (false, "Debit note not found.");
            }

            if (!debitNote.CanReverse)
            {
                return (false, "Only posted debit notes can be reversed.");
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return (false, "Reversal reason is required.");
            }

            // Check if debit note has any applications
            if (debitNote.AppliedAmount > 0)
            {
                return (false, "Cannot reverse debit note that has been applied to payments. Please unapply first.");
            }

            debitNote.DebitNoteStatus = VendorDebitNoteStatuses.Reversed;
            debitNote.ReversalReason = reason;
            debitNote.ReversedOn = DateTime.Now;
            debitNote.ReversedByUserName = userName;
            debitNote.UpdatedAt = DateTime.Now;
            debitNote.UpdatedBy = userName;

            return (true, "Debit note reversed successfully.");
        }

        /// <summary>Reject submitted debit note</summary>
        public (bool Success, string Message) Reject(Guid id, string reason, string userName)
        {
            var debitNote = _debitNotes.FirstOrDefault(d => d.Id == id && !d.IsDeleted);
            if (debitNote == null)
            {
                return (false, "Debit note not found.");
            }

            if (debitNote.DebitNoteStatus != VendorDebitNoteStatuses.Submitted)
            {
                return (false, "Only submitted debit notes can be rejected.");
            }

            debitNote.DebitNoteStatus = VendorDebitNoteStatuses.Draft;
            debitNote.RejectionReason = reason;
            debitNote.DebitNoteNarration = $"{debitNote.DebitNoteNarration}\n[Rejected: {reason}]";
            debitNote.UpdatedAt = DateTime.Now;
            debitNote.UpdatedBy = userName;

            return (true, "Debit note rejected and returned to draft.");
        }

        #endregion

        #region Line Operations

        /// <summary>Add line to debit note</summary>
        public (bool Success, string Message) AddLine(Guid debitNoteId, VendorDebitNoteLineViewModel line)
        {
            var debitNote = _debitNotes.FirstOrDefault(d => d.Id == debitNoteId && !d.IsDeleted);
            if (debitNote == null)
            {
                return (false, "Debit note not found.");
            }

            if (!debitNote.CanEdit)
            {
                return (false, "Lines cannot be added to posted or cancelled debit notes.");
            }

            // Validate line
            if (string.IsNullOrWhiteSpace(line.Description))
            {
                return (false, "Description is required.");
            }

            if (line.Quantity <= 0)
            {
                return (false, "Quantity must be greater than zero.");
            }

            if (line.UnitPrice < 0)
            {
                return (false, "Unit price cannot be negative.");
            }

            if (line.ExpenseOrAssetAccountId == Guid.Empty)
            {
                return (false, "Expense/Asset account is required.");
            }

            // Set line number
            var maxLineNumber = debitNote.Lines.Any() ? debitNote.Lines.Max(l => l.LineNumber) : 0;
            line.LineNumber = maxLineNumber + 10;
            line.Id = Guid.NewGuid();
            line.VendorDebitNoteId = debitNoteId;
            line.CreatedAt = DateTime.Now;

            // Calculate line amounts
            line.RecalculateAmounts();

            debitNote.Lines.Add(line);
            debitNote.RecalculateTotals();
            debitNote.UpdatedAt = DateTime.Now;

            return (true, "Line added successfully.");
        }

        /// <summary>Update line in debit note</summary>
        public (bool Success, string Message) UpdateLine(VendorDebitNoteLineViewModel line)
        {
            var debitNote = _debitNotes.FirstOrDefault(d => d.Id == line.VendorDebitNoteId && !d.IsDeleted);
            if (debitNote == null)
            {
                return (false, "Debit note not found.");
            }

            if (!debitNote.CanEdit)
            {
                return (false, "Lines cannot be updated on posted or cancelled debit notes.");
            }

            var existingLine = debitNote.Lines.FirstOrDefault(l => l.Id == line.Id);
            if (existingLine == null)
            {
                return (false, "Line not found.");
            }

            // Update line fields
            existingLine.LineType = line.LineType;
            existingLine.ItemId = line.ItemId;
            existingLine.ItemCode = line.ItemCode;
            existingLine.ItemName = line.ItemName;
            existingLine.Description = line.Description;
            existingLine.UomId = line.UomId;
            existingLine.UomCode = line.UomCode;
            existingLine.UomName = line.UomName;
            existingLine.Quantity = line.Quantity;
            existingLine.UnitPrice = line.UnitPrice;
            existingLine.DiscountPercent = line.DiscountPercent;
            existingLine.DiscountAmount = line.DiscountAmount;
            existingLine.TaxCodeId = line.TaxCodeId;
            existingLine.TaxCodeCode = line.TaxCodeCode;
            existingLine.TaxCodeName = line.TaxCodeName;
            existingLine.TaxRatePercent = line.TaxRatePercent;
            existingLine.ExpenseOrAssetAccountId = line.ExpenseOrAssetAccountId;
            existingLine.ExpenseOrAssetAccountCode = line.ExpenseOrAssetAccountCode;
            existingLine.ExpenseOrAssetAccountName = line.ExpenseOrAssetAccountName;
            existingLine.TaxAccountId = line.TaxAccountId;
            existingLine.TaxAccountCode = line.TaxAccountCode;
            existingLine.TaxAccountName = line.TaxAccountName;
            existingLine.CostCenterId = line.CostCenterId;
            existingLine.CostCenterCode = line.CostCenterCode;
            existingLine.CostCenterName = line.CostCenterName;
            existingLine.ProjectId = line.ProjectId;
            existingLine.ProjectCode = line.ProjectCode;
            existingLine.ProjectName = line.ProjectName;
            existingLine.DepartmentId = line.DepartmentId;
            existingLine.DepartmentCode = line.DepartmentCode;
            existingLine.DepartmentName = line.DepartmentName;
            existingLine.HSNCode = line.HSNCode;
            existingLine.SACCode = line.SACCode;
            existingLine.ReferenceText = line.ReferenceText;
            existingLine.UpdatedAt = DateTime.Now;
            existingLine.UpdatedBy = line.UpdatedBy;

            // Recalculate line amounts
            existingLine.RecalculateAmounts();

            // Recalculate debit note totals
            debitNote.RecalculateTotals();
            debitNote.UpdatedAt = DateTime.Now;

            return (true, "Line updated successfully.");
        }

        /// <summary>Delete line from debit note</summary>
        public (bool Success, string Message) DeleteLine(Guid debitNoteId, Guid lineId)
        {
            var debitNote = _debitNotes.FirstOrDefault(d => d.Id == debitNoteId && !d.IsDeleted);
            if (debitNote == null)
            {
                return (false, "Debit note not found.");
            }

            if (!debitNote.CanEdit)
            {
                return (false, "Lines cannot be deleted from posted or cancelled debit notes.");
            }

            var line = debitNote.Lines.FirstOrDefault(l => l.Id == lineId);
            if (line == null)
            {
                return (false, "Line not found.");
            }

            debitNote.Lines.Remove(line);
            debitNote.RecalculateTotals();
            debitNote.UpdatedAt = DateTime.Now;

            return (true, "Line deleted successfully.");
        }

        /// <summary>Get lines for debit note</summary>
        public List<VendorDebitNoteLineViewModel> GetLines(Guid debitNoteId)
        {
            var debitNote = _debitNotes.FirstOrDefault(d => d.Id == debitNoteId && !d.IsDeleted);
            return debitNote?.Lines ?? new List<VendorDebitNoteLineViewModel>();
        }

        #endregion

        #region Statistics

        /// <summary>Get debit note statistics</summary>
        public VendorDebitNoteStatisticsViewModel GetStatistics(Guid? companyId = null)
        {
            var debitNotes = companyId.HasValue
                ? _debitNotes.Where(d => d.CompanyId == companyId.Value && !d.IsDeleted)
                : _debitNotes.Where(d => !d.IsDeleted);

            return new VendorDebitNoteStatisticsViewModel
            {
                TotalDebitNotes = debitNotes.Count(),
                DraftDebitNotes = debitNotes.Count(d => d.DebitNoteStatus == VendorDebitNoteStatuses.Draft),
                SubmittedDebitNotes = debitNotes.Count(d => d.DebitNoteStatus == VendorDebitNoteStatuses.Submitted),
                ApprovedDebitNotes = debitNotes.Count(d => d.DebitNoteStatus == VendorDebitNoteStatuses.Approved),
                PostedDebitNotes = debitNotes.Count(d => d.DebitNoteStatus == VendorDebitNoteStatuses.Posted),
                CancelledDebitNotes = debitNotes.Count(d => d.DebitNoteStatus == VendorDebitNoteStatuses.Cancelled),
                ReversedDebitNotes = debitNotes.Count(d => d.DebitNoteStatus == VendorDebitNoteStatuses.Reversed),
                TotalDebitNoteAmount = debitNotes.Where(d => d.DebitNoteStatus == VendorDebitNoteStatuses.Posted).Sum(d => d.TotalDebitAmount),
                TotalAppliedAmount = debitNotes.Where(d => d.DebitNoteStatus == VendorDebitNoteStatuses.Posted).Sum(d => d.AppliedAmount)
            };
        }

        #endregion
    }

    /// <summary>
    /// Vendor debit note statistics model
    /// </summary>
    public class VendorDebitNoteStatisticsViewModel
    {
        public int TotalDebitNotes { get; set; }
        public int DraftDebitNotes { get; set; }
        public int SubmittedDebitNotes { get; set; }
        public int ApprovedDebitNotes { get; set; }
        public int PostedDebitNotes { get; set; }
        public int CancelledDebitNotes { get; set; }
        public int ReversedDebitNotes { get; set; }
        public decimal TotalDebitNoteAmount { get; set; }
        public decimal TotalAppliedAmount { get; set; }
    }
}
