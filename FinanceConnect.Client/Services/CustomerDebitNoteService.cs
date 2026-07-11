using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    /// <summary>
    /// Service for CustomerDebitNote (Model #31) CRUD operations
    /// Demo application - data stored in memory
    /// </summary>
    public class CustomerDebitNoteService
    {
        // Immutable seed data
        private static readonly List<CustomerDebitNoteViewModel> _seedDebitNotes = CustomerDebitNoteSeedData.GetSeedDebitNotes();

        // Working (mutable) data
        private List<CustomerDebitNoteViewModel> _debitNotes;

        // Counter for debit note number generation
        private int _debitNoteCounter = 100;

        public CustomerDebitNoteService()
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
        public List<CustomerDebitNoteViewModel> GetAll()
        {
            return _debitNotes.Where(d => !d.IsDeleted).ToList();
        }

        /// <summary>Get debit note by ID</summary>
        public CustomerDebitNoteViewModel? GetById(Guid id)
        {
            return _debitNotes.FirstOrDefault(d => d.Id == id && !d.IsDeleted);
        }

        /// <summary>Get debit notes by company ID</summary>
        public List<CustomerDebitNoteViewModel> GetByCompanyId(Guid companyId)
        {
            return _debitNotes.Where(d => d.CompanyId == companyId && !d.IsDeleted).ToList();
        }

        /// <summary>Get debit notes by customer ID</summary>
        public List<CustomerDebitNoteViewModel> GetByCustomerId(Guid customerId)
        {
            return _debitNotes.Where(d => d.CustomerId == customerId && !d.IsDeleted).ToList();
        }

        /// <summary>Get debit notes by status</summary>
        public List<CustomerDebitNoteViewModel> GetByStatus(string status)
        {
            return _debitNotes.Where(d => d.DebitNoteStatus == status && !d.IsDeleted).ToList();
        }

        /// <summary>Get debit notes by branch ID</summary>
        public List<CustomerDebitNoteViewModel> GetByBranchId(Guid branchId)
        {
            return _debitNotes.Where(d => d.BranchId == branchId && !d.IsDeleted).ToList();
        }

        /// <summary>Get debit notes by invoice ID</summary>
        public List<CustomerDebitNoteViewModel> GetByInvoiceId(Guid invoiceId)
        {
            return _debitNotes.Where(d => d.CustomerInvoiceId == invoiceId && !d.IsDeleted).ToList();
        }

        /// <summary>Get debit notes by date range</summary>
        public List<CustomerDebitNoteViewModel> GetByDateRange(DateTime fromDate, DateTime toDate)
        {
            return _debitNotes.Where(d =>
                d.DebitNoteDate >= fromDate &&
                d.DebitNoteDate <= toDate &&
                !d.IsDeleted).ToList();
        }

        /// <summary>Get debit notes by reason code</summary>
        public List<CustomerDebitNoteViewModel> GetByReasonCode(string reasonCode)
        {
            return _debitNotes.Where(d => d.DebitReasonCode == reasonCode && !d.IsDeleted).ToList();
        }

        /// <summary>Search debit notes by number, customer code/name, reference</summary>
        public List<CustomerDebitNoteViewModel> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAll();

            searchTerm = searchTerm.ToLower();
            return _debitNotes.Where(d => !d.IsDeleted && (
                d.DebitNoteNumber.ToLower().Contains(searchTerm) ||
                (d.CustomerCode?.ToLower().Contains(searchTerm) ?? false) ||
                (d.CustomerName?.ToLower().Contains(searchTerm) ?? false) ||
                (d.ReferenceText?.ToLower().Contains(searchTerm) ?? false) ||
                (d.CustomerInvoiceNumber?.ToLower().Contains(searchTerm) ?? false)
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

        /// <summary>Generate next debit note number</summary>
        public string GenerateDebitNoteNumber(Guid companyId)
        {
            _debitNoteCounter++;
            var year = DateTime.Today.Year;
            return $"DN-{year}-{_debitNoteCounter:D4}";
        }

        #endregion

        #region Write Operations

        /// <summary>Add new debit note</summary>
        public (bool Success, string Message) Add(CustomerDebitNoteViewModel debitNote)
        {
            // Validate debit note number uniqueness
            if (DebitNoteNumberExists(debitNote.CompanyId, debitNote.DebitNoteNumber))
            {
                return (false, "Debit note number already exists.");
            }

            // Validate customer
            if (debitNote.CustomerId == Guid.Empty)
            {
                return (false, "Customer is required.");
            }

            // Validate branch
            if (debitNote.BranchId == Guid.Empty)
            {
                return (false, "Branch is required.");
            }

            // Validate reason code
            if (string.IsNullOrWhiteSpace(debitNote.DebitReasonCode))
            {
                return (false, "Reason code is required.");
            }

            // Validate invoice reference if against invoice
            if (debitNote.IsAgainstInvoice && debitNote.CustomerInvoiceId == null)
            {
                return (false, "Reference invoice is required when 'Against Invoice' is enabled.");
            }

            // Validate at least one line for non-draft debit notes
            if (debitNote.DebitNoteStatus != DebitNoteStatuses.Draft && !debitNote.Lines.Any())
            {
                return (false, "Debit note must have at least one line.");
            }

            // Validate totals
            if (debitNote.DebitNoteStatus != DebitNoteStatuses.Draft && debitNote.GrandTotalAmount <= 0)
            {
                return (false, "Debit note total must be greater than zero.");
            }

            debitNote.Id = Guid.NewGuid();
            debitNote.DebitNoteNumber = debitNote.DebitNoteNumber.ToUpper().Trim();
            debitNote.CreatedAt = DateTime.Now;
            debitNote.IsDeleted = false;

            // Generate debit note number if empty
            if (string.IsNullOrWhiteSpace(debitNote.DebitNoteNumber))
            {
                debitNote.DebitNoteNumber = GenerateDebitNoteNumber(debitNote.CompanyId);
            }

            // Set line debit note IDs
            foreach (var line in debitNote.Lines)
            {
                line.Id = Guid.NewGuid();
                line.CustomerDebitNoteId = debitNote.Id;
                line.CreatedAt = DateTime.Now;
                line.CreatedBy = debitNote.CreatedBy;
            }

            // Recalculate totals
            debitNote.RecalculateTotals();

            _debitNotes.Add(debitNote);
            return (true, "Debit note created successfully.");
        }

        /// <summary>Update existing debit note</summary>
        public (bool Success, string Message) Update(CustomerDebitNoteViewModel debitNote)
        {
            var existing = _debitNotes.FirstOrDefault(d => d.Id == debitNote.Id && !d.IsDeleted);
            if (existing == null)
            {
                return (false, "Debit note not found.");
            }

            // Cannot edit posted or cancelled/reversed debit notes
            if (!existing.CanEdit)
            {
                return (false, "Posted, cancelled, or reversed debit notes cannot be edited.");
            }

            // Validate debit note number uniqueness
            if (DebitNoteNumberExists(debitNote.CompanyId, debitNote.DebitNoteNumber, debitNote.Id))
            {
                return (false, "Debit note number already exists.");
            }

            // Validate customer
            if (debitNote.CustomerId == Guid.Empty)
            {
                return (false, "Customer is required.");
            }

            // Validate reason code
            if (string.IsNullOrWhiteSpace(debitNote.DebitReasonCode))
            {
                return (false, "Reason code is required.");
            }

            // Validate invoice reference if against invoice
            if (debitNote.IsAgainstInvoice && debitNote.CustomerInvoiceId == null)
            {
                return (false, "Reference invoice is required when 'Against Invoice' is enabled.");
            }

            // Update fields
            existing.BranchId = debitNote.BranchId;
            existing.BranchCode = debitNote.BranchCode;
            existing.BranchName = debitNote.BranchName;
            existing.CustomerId = debitNote.CustomerId;
            existing.CustomerCode = debitNote.CustomerCode;
            existing.CustomerName = debitNote.CustomerName;
            existing.CustomerAccountId = debitNote.CustomerAccountId;
            existing.CustomerAccountName = debitNote.CustomerAccountName;
            existing.DebitNoteNumber = debitNote.DebitNoteNumber.ToUpper().Trim();
            existing.DebitNoteDate = debitNote.DebitNoteDate;
            existing.CurrencyId = debitNote.CurrencyId;
            existing.CurrencyCode = debitNote.CurrencyCode;
            existing.CurrencyName = debitNote.CurrencyName;
            existing.ExchangeRate = debitNote.ExchangeRate;
            existing.ReferenceText = debitNote.ReferenceText;
            existing.DebitNoteNarration = debitNote.DebitNoteNarration;
            existing.IsAgainstInvoice = debitNote.IsAgainstInvoice;
            existing.CustomerInvoiceId = debitNote.CustomerInvoiceId;
            existing.CustomerInvoiceNumber = debitNote.CustomerInvoiceNumber;
            existing.InvoiceNumberSnapshot = debitNote.InvoiceNumberSnapshot;
            existing.InvoiceDateSnapshot = debitNote.InvoiceDateSnapshot;
            existing.DebitReasonCode = debitNote.DebitReasonCode;
            existing.DebitReasonDescription = debitNote.DebitReasonDescription;
            existing.IsTaxImpacting = debitNote.IsTaxImpacting;
            existing.IsRevenueRecognized = debitNote.IsRevenueRecognized;
            existing.RevenueAccountId = debitNote.RevenueAccountId;
            existing.RevenueAccountCode = debitNote.RevenueAccountCode;
            existing.RevenueAccountName = debitNote.RevenueAccountName;
            existing.Lines = debitNote.Lines;
            existing.UpdatedAt = DateTime.Now;
            existing.UpdatedBy = debitNote.UpdatedBy;

            // Recalculate totals
            existing.RecalculateTotals();

            return (true, "Debit note updated successfully.");
        }

        /// <summary>Delete debit note (soft delete)</summary>
        public (bool Success, string Message) Delete(Guid id, string? userName = null)
        {
            var debitNote = _debitNotes.FirstOrDefault(d => d.Id == id && !d.IsDeleted);
            if (debitNote == null)
            {
                return (false, "Debit note not found.");
            }

            // Only draft debit notes can be deleted
            if (debitNote.DebitNoteStatus != DebitNoteStatuses.Draft)
            {
                return (false, "Only draft debit notes can be deleted.");
            }

            debitNote.IsDeleted = true;
            debitNote.DeletedAt = DateTime.Now;
            debitNote.DeletedBy = userName;

            return (true, "Debit note deleted successfully.");
        }

        #endregion

        #region Workflow Operations

        /// <summary>Submit debit note for approval</summary>
        public (bool Success, string Message) Submit(Guid id, string? userName = null)
        {
            var debitNote = _debitNotes.FirstOrDefault(d => d.Id == id && !d.IsDeleted);
            if (debitNote == null)
            {
                return (false, "Debit note not found.");
            }

            if (debitNote.DebitNoteStatus != DebitNoteStatuses.Draft)
            {
                return (false, "Only draft debit notes can be submitted.");
            }

            // Validate lines
            if (!debitNote.Lines.Any())
            {
                return (false, "Debit note has no lines.");
            }

            // Validate totals
            if (debitNote.GrandTotalAmount <= 0)
            {
                return (false, "Debit note total must be greater than zero.");
            }

            debitNote.DebitNoteStatus = DebitNoteStatuses.Submitted;
            debitNote.UpdatedAt = DateTime.Now;
            debitNote.UpdatedBy = userName;

            return (true, "Debit note submitted for approval.");
        }

        /// <summary>Approve debit note</summary>
        public (bool Success, string Message) Approve(Guid id, string? userName = null)
        {
            var debitNote = _debitNotes.FirstOrDefault(d => d.Id == id && !d.IsDeleted);
            if (debitNote == null)
            {
                return (false, "Debit note not found.");
            }

            if (debitNote.DebitNoteStatus != DebitNoteStatuses.Submitted)
            {
                return (false, "Only submitted debit notes can be approved.");
            }

            debitNote.DebitNoteStatus = DebitNoteStatuses.Approved;
            debitNote.UpdatedAt = DateTime.Now;
            debitNote.UpdatedBy = userName;

            return (true, "Debit note approved.");
        }

        /// <summary>Post debit note</summary>
        public (bool Success, string Message) Post(Guid id, Guid userId, string userName)
        {
            var debitNote = _debitNotes.FirstOrDefault(d => d.Id == id && !d.IsDeleted);
            if (debitNote == null)
            {
                return (false, "Debit note not found.");
            }

            if (!debitNote.CanPost)
            {
                return (false, "Debit note cannot be posted in current status.");
            }

            // Validate lines
            if (!debitNote.Lines.Any())
            {
                return (false, "Debit note has no lines.");
            }

            // Validate totals
            if (debitNote.GrandTotalAmount <= 0)
            {
                return (false, "Debit note total must be greater than zero.");
            }

            // Validate posting date
            var postingDate = DateTime.Today;
            if (postingDate < debitNote.DebitNoteDate)
            {
                return (false, "Posting date cannot be before debit note date.");
            }

            debitNote.DebitNoteStatus = DebitNoteStatuses.Posted;
            debitNote.PostingDate = postingDate;
            debitNote.PostedOn = DateTime.Now;
            debitNote.PostedByUserId = userId;
            debitNote.PostedByUserName = userName;
            debitNote.UpdatedAt = DateTime.Now;
            debitNote.UpdatedBy = userName;

            // Set applied amount if against invoice
            if (debitNote.IsAgainstInvoice && debitNote.CustomerInvoiceId.HasValue)
            {
                debitNote.AppliedToInvoiceAmount = debitNote.GrandTotalAmount;
            }

            return (true, "Debit note posted successfully.");
        }

        /// <summary>Cancel debit note (pre-post only)</summary>
        public (bool Success, string Message) Cancel(Guid id, string reason, string? userName = null)
        {
            var debitNote = _debitNotes.FirstOrDefault(d => d.Id == id && !d.IsDeleted);
            if (debitNote == null)
            {
                return (false, "Debit note not found.");
            }

            if (!debitNote.CanCancel)
            {
                return (false, "Only draft or submitted debit notes can be cancelled.");
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return (false, "Cancellation reason is required.");
            }

            debitNote.DebitNoteStatus = DebitNoteStatuses.Cancelled;
            debitNote.CancelledOn = DateTime.Now;
            debitNote.CancelledByUserId = Guid.NewGuid(); // Would come from auth context
            debitNote.CancelledByUserName = userName ?? "System";
            debitNote.CancellationReason = reason;
            debitNote.UpdatedAt = DateTime.Now;
            debitNote.UpdatedBy = userName;

            return (true, "Debit note cancelled.");
        }

        /// <summary>Reverse posted debit note</summary>
        public (bool Success, string Message) Reverse(Guid id, string reason, string? userName = null)
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

            debitNote.DebitNoteStatus = DebitNoteStatuses.Reversed;
            debitNote.ReversedOn = DateTime.Now;
            debitNote.ReversedByUserId = Guid.NewGuid(); // Would come from auth context
            debitNote.ReversedByUserName = userName ?? "System";
            debitNote.ReversalReason = reason;
            debitNote.UpdatedAt = DateTime.Now;
            debitNote.UpdatedBy = userName;

            return (true, "Debit note reversed successfully.");
        }

        /// <summary>Reject submitted debit note</summary>
        public (bool Success, string Message) Reject(Guid id, string reason, string? userName = null)
        {
            var debitNote = _debitNotes.FirstOrDefault(d => d.Id == id && !d.IsDeleted);
            if (debitNote == null)
            {
                return (false, "Debit note not found.");
            }

            if (debitNote.DebitNoteStatus != DebitNoteStatuses.Submitted)
            {
                return (false, "Only submitted debit notes can be rejected.");
            }

            debitNote.DebitNoteStatus = DebitNoteStatuses.Draft;
            debitNote.DebitNoteNarration = $"{debitNote.DebitNoteNarration}\n[Rejected: {reason}]";
            debitNote.UpdatedAt = DateTime.Now;
            debitNote.UpdatedBy = userName;

            return (true, "Debit note rejected and returned to draft.");
        }

        #endregion

        #region Line Operations

        /// <summary>Add line to debit note</summary>
        public (bool Success, string Message) AddLine(Guid debitNoteId, CustomerDebitNoteLineViewModel line)
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

            // Set line number
            var maxLineNumber = debitNote.Lines.Any() ? debitNote.Lines.Max(l => l.LineNumber) : 0;
            line.LineNumber = maxLineNumber + 10;
            line.Id = Guid.NewGuid();
            line.CustomerDebitNoteId = debitNoteId;
            line.CreatedAt = DateTime.Now;

            // Calculate line amounts
            line.RecalculateAmounts();

            debitNote.Lines.Add(line);
            debitNote.RecalculateTotals();
            debitNote.UpdatedAt = DateTime.Now;

            return (true, "Line added successfully.");
        }

        /// <summary>Update line in debit note</summary>
        public (bool Success, string Message) UpdateLine(CustomerDebitNoteLineViewModel line)
        {
            var debitNote = _debitNotes.FirstOrDefault(d => d.Id == line.CustomerDebitNoteId && !d.IsDeleted);
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
            existingLine.DiscountReason = line.DiscountReason;
            existingLine.TaxCodeId = line.TaxCodeId;
            existingLine.TaxCodeCode = line.TaxCodeCode;
            existingLine.TaxCodeName = line.TaxCodeName;
            existingLine.TaxRatePercent = line.TaxRatePercent;
            existingLine.RevenueAccountId = line.RevenueAccountId;
            existingLine.RevenueAccountCode = line.RevenueAccountCode;
            existingLine.RevenueAccountName = line.RevenueAccountName;
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
        public List<CustomerDebitNoteLineViewModel> GetLines(Guid debitNoteId)
        {
            var debitNote = _debitNotes.FirstOrDefault(d => d.Id == debitNoteId && !d.IsDeleted);
            return debitNote?.Lines ?? new List<CustomerDebitNoteLineViewModel>();
        }

        #endregion

        #region Statistics

        /// <summary>Get debit note statistics</summary>
        public DebitNoteStatisticsViewModel GetStatistics(Guid? companyId = null)
        {
            var debitNotes = companyId.HasValue
                ? _debitNotes.Where(d => d.CompanyId == companyId.Value && !d.IsDeleted)
                : _debitNotes.Where(d => !d.IsDeleted);

            return new DebitNoteStatisticsViewModel
            {
                TotalDebitNotes = debitNotes.Count(),
                DraftDebitNotes = debitNotes.Count(d => d.DebitNoteStatus == DebitNoteStatuses.Draft),
                SubmittedDebitNotes = debitNotes.Count(d => d.DebitNoteStatus == DebitNoteStatuses.Submitted),
                ApprovedDebitNotes = debitNotes.Count(d => d.DebitNoteStatus == DebitNoteStatuses.Approved),
                PostedDebitNotes = debitNotes.Count(d => d.DebitNoteStatus == DebitNoteStatuses.Posted),
                CancelledDebitNotes = debitNotes.Count(d => d.DebitNoteStatus == DebitNoteStatuses.Cancelled),
                ReversedDebitNotes = debitNotes.Count(d => d.DebitNoteStatus == DebitNoteStatuses.Reversed),
                TotalDebitNoteAmount = debitNotes.Where(d => d.DebitNoteStatus == DebitNoteStatuses.Posted).Sum(d => d.GrandTotalAmount),
                TotalAppliedToInvoice = debitNotes.Where(d => d.DebitNoteStatus == DebitNoteStatuses.Posted).Sum(d => d.AppliedToInvoiceAmount)
            };
        }

        #endregion
    }

    /// <summary>
    /// Debit note statistics model
    /// </summary>
    public class DebitNoteStatisticsViewModel
    {
        public int TotalDebitNotes { get; set; }
        public int DraftDebitNotes { get; set; }
        public int SubmittedDebitNotes { get; set; }
        public int ApprovedDebitNotes { get; set; }
        public int PostedDebitNotes { get; set; }
        public int CancelledDebitNotes { get; set; }
        public int ReversedDebitNotes { get; set; }
        public decimal TotalDebitNoteAmount { get; set; }
        public decimal TotalAppliedToInvoice { get; set; }
    }
}
