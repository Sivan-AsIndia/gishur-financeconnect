using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    /// <summary>
    /// Service for VendorCreditNote (Model #39) CRUD operations
    /// Demo application - data stored in memory
    /// </summary>
    public class VendorCreditNoteService
    {
        // Immutable seed data
        private static readonly List<VendorCreditNoteViewModel> _seedCreditNotes = VendorCreditNoteSeedData.GetSeedCreditNotes();

        // Working (mutable) data
        private List<VendorCreditNoteViewModel> _creditNotes;

        // Counter for credit note number generation
        private int _creditNoteCounter = 100;

        public VendorCreditNoteService()
        {
            ResetToSeed();
        }

        private static List<T> CloneList<T>(IEnumerable<T> source) where T : class
        {
            var json = System.Text.Json.JsonSerializer.Serialize(source);
            return System.Text.Json.JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }

        /// <summary>Reset credit notes to seed data</summary>
        public void ResetToSeed()
        {
            _creditNotes = CloneList(_seedCreditNotes);
        }

        #region Read Operations

        /// <summary>Get all credit notes</summary>
        public List<VendorCreditNoteViewModel> GetAll()
        {
            return _creditNotes.Where(cn => !cn.IsDeleted).ToList();
        }

        /// <summary>Get credit note by ID</summary>
        public VendorCreditNoteViewModel? GetById(Guid id)
        {
            return _creditNotes.FirstOrDefault(cn => cn.Id == id && !cn.IsDeleted);
        }

        /// <summary>Get credit notes by company ID</summary>
        public List<VendorCreditNoteViewModel> GetByCompanyId(Guid companyId)
        {
            return _creditNotes.Where(cn => cn.CompanyId == companyId && !cn.IsDeleted).ToList();
        }

        /// <summary>Get credit notes by vendor ID</summary>
        public List<VendorCreditNoteViewModel> GetByVendorId(Guid vendorId)
        {
            return _creditNotes.Where(cn => cn.VendorId == vendorId && !cn.IsDeleted).ToList();
        }

        /// <summary>Get credit notes by status</summary>
        public List<VendorCreditNoteViewModel> GetByStatus(string status)
        {
            return _creditNotes.Where(cn => cn.CreditNoteStatus == status && !cn.IsDeleted).ToList();
        }

        /// <summary>Get credit notes by branch ID</summary>
        public List<VendorCreditNoteViewModel> GetByBranchId(Guid branchId)
        {
            return _creditNotes.Where(cn => cn.BranchId == branchId && !cn.IsDeleted).ToList();
        }

        /// <summary>Get credit notes by bill ID</summary>
        public List<VendorCreditNoteViewModel> GetByBillId(Guid billId)
        {
            return _creditNotes.Where(cn => cn.PrimaryVendorBillId == billId && !cn.IsDeleted).ToList();
        }

        /// <summary>Get credit notes by date range</summary>
        public List<VendorCreditNoteViewModel> GetByDateRange(DateTime fromDate, DateTime toDate)
        {
            return _creditNotes.Where(cn =>
                cn.VendorCreditNoteDate >= fromDate &&
                cn.VendorCreditNoteDate <= toDate &&
                !cn.IsDeleted).ToList();
        }

        /// <summary>Search credit notes by number, vendor code/name, reference</summary>
        public List<VendorCreditNoteViewModel> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAll();

            searchTerm = searchTerm.ToLower();
            return _creditNotes.Where(cn => !cn.IsDeleted && (
                cn.CreditNoteNumber.ToLower().Contains(searchTerm) ||
                (cn.VendorCode?.ToLower().Contains(searchTerm) ?? false) ||
                (cn.VendorName?.ToLower().Contains(searchTerm) ?? false) ||
                (cn.VendorCreditNoteReferenceNumber?.ToLower().Contains(searchTerm) ?? false) ||
                (cn.PrimaryVendorBillNumber?.ToLower().Contains(searchTerm) ?? false)
            )).ToList();
        }

        /// <summary>Check if credit note number exists within company</summary>
        public bool CreditNoteNumberExists(Guid companyId, string creditNoteNumber, Guid? excludeId = null)
        {
            return _creditNotes.Any(cn =>
                cn.CompanyId == companyId &&
                cn.CreditNoteNumber.Equals(creditNoteNumber, StringComparison.OrdinalIgnoreCase) &&
                !cn.IsDeleted &&
                (excludeId == null || cn.Id != excludeId));
        }

        /// <summary>Check if vendor reference number exists for vendor within company</summary>
        public bool VendorReferenceExists(Guid companyId, Guid vendorId, string vendorReference, Guid? excludeId = null)
        {
            return _creditNotes.Any(cn =>
                cn.CompanyId == companyId &&
                cn.VendorId == vendorId &&
                cn.VendorCreditNoteReferenceNumber.Equals(vendorReference, StringComparison.OrdinalIgnoreCase) &&
                !cn.IsDeleted &&
                (excludeId == null || cn.Id != excludeId));
        }

        /// <summary>Generate next credit note number</summary>
        public string GenerateCreditNoteNumber(Guid companyId)
        {
            _creditNoteCounter++;
            var year = DateTime.Today.Year;
            return $"APCN-{year}-{_creditNoteCounter:D4}";
        }

        #endregion

        #region Write Operations

        /// <summary>Add new credit note</summary>
        public (bool Success, string Message) Add(VendorCreditNoteViewModel creditNote)
        {
            // Validate credit note number uniqueness
            if (!string.IsNullOrWhiteSpace(creditNote.CreditNoteNumber) &&
                CreditNoteNumberExists(creditNote.CompanyId, creditNote.CreditNoteNumber))
            {
                return (false, "Credit Note number already exists.");
            }

            // Validate vendor
            if (creditNote.VendorId == Guid.Empty)
            {
                return (false, "Vendor is required.");
            }

            // Validate branch
            if (creditNote.BranchId == Guid.Empty)
            {
                return (false, "Branch is required.");
            }

            // Validate vendor reference number
            if (string.IsNullOrWhiteSpace(creditNote.VendorCreditNoteReferenceNumber))
            {
                return (false, "Vendor Credit Note Reference Number is required.");
            }

            // Check for duplicate vendor reference
            if (VendorReferenceExists(creditNote.CompanyId, creditNote.VendorId, creditNote.VendorCreditNoteReferenceNumber))
            {
                return (false, "Duplicate vendor credit note reference detected.");
            }

            // Validate credit note type
            if (string.IsNullOrWhiteSpace(creditNote.CreditNoteType))
            {
                return (false, "Credit Type is required.");
            }

            // Validate at least one line for non-draft credit notes
            if (creditNote.CreditNoteStatus != VendorCreditNoteStatuses.Draft && !creditNote.Lines.Any())
            {
                return (false, "Credit Note must have at least one line.");
            }

            // Validate totals
            if (creditNote.CreditNoteStatus != VendorCreditNoteStatuses.Draft && creditNote.TotalCreditAmount <= 0)
            {
                return (false, "Credit Note total must be greater than zero.");
            }

            // Validate bill reference if against bill
            if (creditNote.IsAgainstBill && creditNote.PrimaryVendorBillId == null)
            {
                return (false, "Reference Bill is required when 'Against Bill' is enabled.");
            }

            creditNote.Id = Guid.NewGuid();
            creditNote.CreatedAt = DateTime.Now;
            creditNote.IsDeleted = false;

            // Generate credit note number if empty
            if (string.IsNullOrWhiteSpace(creditNote.CreditNoteNumber))
            {
                creditNote.CreditNoteNumber = GenerateCreditNoteNumber(creditNote.CompanyId);
            }
            else
            {
                creditNote.CreditNoteNumber = creditNote.CreditNoteNumber.ToUpper().Trim();
            }

            // Set line credit note IDs
            foreach (var line in creditNote.Lines)
            {
                line.Id = Guid.NewGuid();
                line.VendorCreditNoteId = creditNote.Id;
                line.CreatedAt = DateTime.Now;
                line.CreatedBy = creditNote.CreatedBy;
            }

            // Recalculate totals
            creditNote.RecalculateTotals();

            _creditNotes.Add(creditNote);
            return (true, "Credit Note created successfully.");
        }

        /// <summary>Update existing credit note</summary>
        public (bool Success, string Message) Update(VendorCreditNoteViewModel creditNote)
        {
            var existing = _creditNotes.FirstOrDefault(cn => cn.Id == creditNote.Id && !cn.IsDeleted);
            if (existing == null)
            {
                return (false, "Credit Note not found.");
            }

            if (!existing.CanEdit)
            {
                return (false, "Posted or cancelled credit notes cannot be edited.");
            }

            // Validate vendor reference number uniqueness
            if (!string.IsNullOrWhiteSpace(creditNote.VendorCreditNoteReferenceNumber) &&
                VendorReferenceExists(creditNote.CompanyId, creditNote.VendorId, creditNote.VendorCreditNoteReferenceNumber, creditNote.Id))
            {
                return (false, "Duplicate vendor credit note reference detected.");
            }

            // Update fields
            existing.BranchId = creditNote.BranchId;
            existing.BranchCode = creditNote.BranchCode;
            existing.BranchName = creditNote.BranchName;
            existing.VendorId = creditNote.VendorId;
            existing.VendorCode = creditNote.VendorCode;
            existing.VendorName = creditNote.VendorName;
            existing.VendorAccountId = creditNote.VendorAccountId;
            existing.VendorCreditNoteReferenceNumber = creditNote.VendorCreditNoteReferenceNumber;
            existing.VendorCreditNoteDate = creditNote.VendorCreditNoteDate;
            existing.CreditEntryDate = creditNote.CreditEntryDate;
            existing.CurrencyId = creditNote.CurrencyId;
            existing.CurrencyCode = creditNote.CurrencyCode;
            existing.CurrencyName = creditNote.CurrencyName;
            existing.ExchangeRate = creditNote.ExchangeRate;
            existing.CreditNoteNarration = creditNote.CreditNoteNarration;
            existing.CreditNoteType = creditNote.CreditNoteType;
            existing.ReasonCodeId = creditNote.ReasonCodeId;
            existing.ReasonCodeName = creditNote.ReasonCodeName;
            existing.IsAgainstBill = creditNote.IsAgainstBill;
            existing.PrimaryVendorBillId = creditNote.PrimaryVendorBillId;
            existing.PrimaryVendorBillNumber = creditNote.PrimaryVendorBillNumber;
            existing.BillNumberSnapshot = creditNote.BillNumberSnapshot;
            existing.BillDateSnapshot = creditNote.BillDateSnapshot;
            existing.IsGSTApplicable = creditNote.IsGSTApplicable;
            existing.VendorGSTINSnapshot = creditNote.VendorGSTINSnapshot;
            existing.PlaceOfSupplyStateId = creditNote.PlaceOfSupplyStateId;
            existing.PlaceOfSupplyStateName = creditNote.PlaceOfSupplyStateName;
            existing.IsReverseChargeApplicable = creditNote.IsReverseChargeApplicable;
            existing.HasAttachments = creditNote.HasAttachments;
            existing.AttachmentCount = creditNote.AttachmentCount;

            // Update lines
            existing.Lines = creditNote.Lines;
            foreach (var line in existing.Lines)
            {
                if (line.Id == Guid.Empty)
                {
                    line.Id = Guid.NewGuid();
                }
                line.VendorCreditNoteId = existing.Id;
                line.UpdatedAt = DateTime.Now;
                line.UpdatedBy = creditNote.UpdatedBy;
            }

            // Recalculate totals
            existing.RecalculateTotals();

            existing.UpdatedAt = DateTime.Now;
            existing.UpdatedBy = creditNote.UpdatedBy;

            return (true, "Credit Note updated successfully.");
        }

        /// <summary>Delete credit note (soft delete, draft only)</summary>
        public (bool Success, string Message) Delete(Guid id, string deletedBy)
        {
            var creditNote = _creditNotes.FirstOrDefault(cn => cn.Id == id && !cn.IsDeleted);
            if (creditNote == null)
            {
                return (false, "Credit Note not found.");
            }

            if (creditNote.CreditNoteStatus != VendorCreditNoteStatuses.Draft)
            {
                return (false, "Only draft credit notes can be deleted.");
            }

            creditNote.IsDeleted = true;
            creditNote.DeletedAt = DateTime.Now;
            creditNote.DeletedBy = deletedBy;

            return (true, "Credit Note deleted successfully.");
        }

        #endregion

        #region Workflow Operations

        /// <summary>Submit credit note for approval</summary>
        public (bool Success, string Message) Submit(Guid id, Guid userId, string userName)
        {
            var creditNote = _creditNotes.FirstOrDefault(cn => cn.Id == id && !cn.IsDeleted);
            if (creditNote == null)
            {
                return (false, "Credit Note not found.");
            }

            if (creditNote.CreditNoteStatus != VendorCreditNoteStatuses.Draft)
            {
                return (false, "Only draft credit notes can be submitted.");
            }

            if (!creditNote.Lines.Any())
            {
                return (false, "Credit Note has no lines.");
            }

            if (creditNote.TotalCreditAmount <= 0)
            {
                return (false, "Credit Note total must be greater than zero.");
            }

            creditNote.CreditNoteStatus = VendorCreditNoteStatuses.Submitted;
            creditNote.SubmittedOn = DateTime.Now;
            creditNote.SubmittedByUserId = userId;
            creditNote.SubmittedByUserName = userName;
            creditNote.UpdatedAt = DateTime.Now;
            creditNote.UpdatedBy = userName;

            return (true, "Credit Note submitted successfully.");
        }

        /// <summary>Approve credit note</summary>
        public (bool Success, string Message) Approve(Guid id, string userName)
        {
            var creditNote = _creditNotes.FirstOrDefault(cn => cn.Id == id && !cn.IsDeleted);
            if (creditNote == null)
            {
                return (false, "Credit Note not found.");
            }

            if (creditNote.CreditNoteStatus != VendorCreditNoteStatuses.Submitted)
            {
                return (false, "Only submitted credit notes can be approved.");
            }

            creditNote.CreditNoteStatus = VendorCreditNoteStatuses.Approved;
            creditNote.ApprovedOn = DateTime.Now;
            creditNote.ApprovedByUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            creditNote.ApprovedByUserName = userName;
            creditNote.UpdatedAt = DateTime.Now;
            creditNote.UpdatedBy = userName;

            return (true, "Credit Note approved successfully.");
        }

        /// <summary>Reject credit note</summary>
        public (bool Success, string Message) Reject(Guid id, string reason, string userName)
        {
            var creditNote = _creditNotes.FirstOrDefault(cn => cn.Id == id && !cn.IsDeleted);
            if (creditNote == null)
            {
                return (false, "Credit Note not found.");
            }

            if (creditNote.CreditNoteStatus != VendorCreditNoteStatuses.Submitted)
            {
                return (false, "Only submitted credit notes can be rejected.");
            }

            creditNote.CreditNoteStatus = VendorCreditNoteStatuses.Draft;
            creditNote.RejectionReason = reason;
            creditNote.CreditNoteNarration = $"{creditNote.CreditNoteNarration}\n[Rejected: {reason}]";
            creditNote.UpdatedAt = DateTime.Now;
            creditNote.UpdatedBy = userName;

            return (true, "Credit Note rejected and returned to draft.");
        }

        /// <summary>Post credit note</summary>
        public (bool Success, string Message) Post(Guid id, Guid accountingPeriodId, string userName)
        {
            var creditNote = _creditNotes.FirstOrDefault(cn => cn.Id == id && !cn.IsDeleted);
            if (creditNote == null)
            {
                return (false, "Credit Note not found.");
            }

            if (!creditNote.CanPost)
            {
                return (false, "Credit Note cannot be posted in current status.");
            }

            if (!creditNote.Lines.Any())
            {
                return (false, "Credit Note has no lines.");
            }

            if (creditNote.TotalCreditAmount <= 0)
            {
                return (false, "Credit Note total must be greater than zero.");
            }

            creditNote.CreditNoteStatus = VendorCreditNoteStatuses.Posted;
            creditNote.PostingDate = DateTime.Today;
            creditNote.PostedOn = DateTime.Now;
            creditNote.PostedByUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            creditNote.PostedByUserName = userName;
            creditNote.UpdatedAt = DateTime.Now;
            creditNote.UpdatedBy = userName;

            // If against bill, apply the credit
            if (creditNote.IsAgainstBill && creditNote.PrimaryVendorBillId.HasValue)
            {
                creditNote.AppliedAmount = creditNote.TotalCreditAmount;
            }

            return (true, "Credit Note posted successfully.");
        }

        /// <summary>Cancel credit note (pre-post only)</summary>
        public (bool Success, string Message) Cancel(Guid id, string reason, string userName)
        {
            var creditNote = _creditNotes.FirstOrDefault(cn => cn.Id == id && !cn.IsDeleted);
            if (creditNote == null)
            {
                return (false, "Credit Note not found.");
            }

            if (!creditNote.CanCancel)
            {
                return (false, "Credit Note cannot be cancelled in current status.");
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return (false, "Cancellation reason is required.");
            }

            creditNote.CreditNoteStatus = VendorCreditNoteStatuses.Cancelled;
            creditNote.CancellationReason = reason;
            creditNote.CancelledOn = DateTime.Now;
            creditNote.CancelledByUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            creditNote.CancelledByUserName = userName;
            creditNote.UpdatedAt = DateTime.Now;
            creditNote.UpdatedBy = userName;

            return (true, "Credit Note cancelled successfully.");
        }

        /// <summary>Reverse posted credit note</summary>
        public (bool Success, string Message) Reverse(Guid id, string reason, string userName)
        {
            var creditNote = _creditNotes.FirstOrDefault(cn => cn.Id == id && !cn.IsDeleted);
            if (creditNote == null)
            {
                return (false, "Credit Note not found.");
            }

            if (creditNote.CreditNoteStatus != VendorCreditNoteStatuses.Posted)
            {
                return (false, "Only posted credit notes can be reversed.");
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return (false, "Reversal reason is required.");
            }

            creditNote.CreditNoteStatus = VendorCreditNoteStatuses.Reversed;
            creditNote.ReversalReason = reason;
            creditNote.ReversedOn = DateTime.Now;
            creditNote.ReversedByUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            creditNote.ReversedByUserName = userName;
            creditNote.UpdatedAt = DateTime.Now;
            creditNote.UpdatedBy = userName;

            return (true, "Credit Note reversed successfully.");
        }

        #endregion

        #region Line Operations

        /// <summary>Add line to credit note</summary>
        public (bool Success, string Message) AddLine(Guid creditNoteId, VendorCreditNoteLineModel line)
        {
            var creditNote = _creditNotes.FirstOrDefault(cn => cn.Id == creditNoteId && !cn.IsDeleted);
            if (creditNote == null)
            {
                return (false, "Credit Note not found.");
            }

            if (!creditNote.CanEdit)
            {
                return (false, "Lines cannot be added to posted or cancelled credit notes.");
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

            if (line.ReversalAccountId == Guid.Empty)
            {
                return (false, "Reversal Account is required.");
            }

            // Set line number
            var maxLineNumber = creditNote.Lines.Any() ? creditNote.Lines.Max(l => l.LineNumber) : 0;
            line.LineNumber = maxLineNumber + 10;
            line.Id = Guid.NewGuid();
            line.VendorCreditNoteId = creditNoteId;
            line.CreatedAt = DateTime.Now;

            // Calculate line amounts
            line.RecalculateAmounts();

            creditNote.Lines.Add(line);
            creditNote.RecalculateTotals();
            creditNote.UpdatedAt = DateTime.Now;

            return (true, "Line added successfully.");
        }

        /// <summary>Update line in credit note</summary>
        public (bool Success, string Message) UpdateLine(VendorCreditNoteLineModel line)
        {
            var creditNote = _creditNotes.FirstOrDefault(cn => cn.Id == line.VendorCreditNoteId && !cn.IsDeleted);
            if (creditNote == null)
            {
                return (false, "Credit Note not found.");
            }

            if (!creditNote.CanEdit)
            {
                return (false, "Lines cannot be updated on posted or cancelled credit notes.");
            }

            var existingLine = creditNote.Lines.FirstOrDefault(l => l.Id == line.Id);
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
            existingLine.DiscountReason = line.DiscountReason;
            existingLine.TaxCodeId = line.TaxCodeId;
            existingLine.TaxCodeCode = line.TaxCodeCode;
            existingLine.TaxCodeName = line.TaxCodeName;
            existingLine.TaxRatePercent = line.TaxRatePercent;
            existingLine.ReversalAccountId = line.ReversalAccountId;
            existingLine.ReversalAccountCode = line.ReversalAccountCode;
            existingLine.ReversalAccountName = line.ReversalAccountName;
            existingLine.HSNCode = line.HSNCode;
            existingLine.SACCode = line.SACCode;
            existingLine.ReferenceText = line.ReferenceText;
            existingLine.UpdatedAt = DateTime.Now;
            existingLine.UpdatedBy = line.UpdatedBy;

            // Recalculate line amounts
            existingLine.RecalculateAmounts();

            // Recalculate credit note totals
            creditNote.RecalculateTotals();
            creditNote.UpdatedAt = DateTime.Now;

            return (true, "Line updated successfully.");
        }

        /// <summary>Delete line from credit note</summary>
        public (bool Success, string Message) DeleteLine(Guid creditNoteId, Guid lineId)
        {
            var creditNote = _creditNotes.FirstOrDefault(cn => cn.Id == creditNoteId && !cn.IsDeleted);
            if (creditNote == null)
            {
                return (false, "Credit Note not found.");
            }

            if (!creditNote.CanEdit)
            {
                return (false, "Lines cannot be deleted from posted or cancelled credit notes.");
            }

            var line = creditNote.Lines.FirstOrDefault(l => l.Id == lineId);
            if (line == null)
            {
                return (false, "Line not found.");
            }

            creditNote.Lines.Remove(line);
            creditNote.RecalculateTotals();
            creditNote.UpdatedAt = DateTime.Now;

            return (true, "Line deleted successfully.");
        }

        /// <summary>Get lines for credit note</summary>
        public List<VendorCreditNoteLineModel> GetLines(Guid creditNoteId)
        {
            var creditNote = _creditNotes.FirstOrDefault(cn => cn.Id == creditNoteId && !cn.IsDeleted);
            return creditNote?.Lines ?? new List<VendorCreditNoteLineModel>();
        }

        #endregion

        #region Statistics

        /// <summary>Get credit note statistics</summary>
        public VendorCreditNoteStatisticsViewModel GetStatistics(Guid? companyId = null)
        {
            var creditNotes = companyId.HasValue
                ? _creditNotes.Where(cn => cn.CompanyId == companyId.Value && !cn.IsDeleted)
                : _creditNotes.Where(cn => !cn.IsDeleted);

            return new VendorCreditNoteStatisticsViewModel
            {
                TotalCreditNotes = creditNotes.Count(),
                DraftCreditNotes = creditNotes.Count(cn => cn.CreditNoteStatus == VendorCreditNoteStatuses.Draft),
                SubmittedCreditNotes = creditNotes.Count(cn => cn.CreditNoteStatus == VendorCreditNoteStatuses.Submitted),
                ApprovedCreditNotes = creditNotes.Count(cn => cn.CreditNoteStatus == VendorCreditNoteStatuses.Approved),
                PostedCreditNotes = creditNotes.Count(cn => cn.CreditNoteStatus == VendorCreditNoteStatuses.Posted),
                CancelledCreditNotes = creditNotes.Count(cn => cn.CreditNoteStatus == VendorCreditNoteStatuses.Cancelled),
                TotalCreditNoteAmount = creditNotes.Where(cn => cn.CreditNoteStatus == VendorCreditNoteStatuses.Posted).Sum(cn => cn.TotalCreditAmount),
                TotalAppliedAmount = creditNotes.Where(cn => cn.CreditNoteStatus == VendorCreditNoteStatuses.Posted).Sum(cn => cn.AppliedAmount)
            };
        }

        #endregion
    }

    /// <summary>
    /// Vendor Credit Note statistics model
    /// </summary>
    public class VendorCreditNoteStatisticsViewModel
    {
        public int TotalCreditNotes { get; set; }
        public int DraftCreditNotes { get; set; }
        public int SubmittedCreditNotes { get; set; }
        public int ApprovedCreditNotes { get; set; }
        public int PostedCreditNotes { get; set; }
        public int CancelledCreditNotes { get; set; }
        public decimal TotalCreditNoteAmount { get; set; }
        public decimal TotalAppliedAmount { get; set; }
    }
}
