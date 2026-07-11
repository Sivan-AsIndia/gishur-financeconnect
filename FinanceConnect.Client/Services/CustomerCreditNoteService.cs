using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    /// <summary>
    /// Service for CustomerCreditNote (Model #30) CRUD operations
    /// Demo application - data stored in memory
    /// </summary>
    public class CustomerCreditNoteService
    {
        // Immutable seed data
        private static readonly List<CustomerCreditNoteViewModel> _seedCreditNotes = CustomerCreditNoteSeedData.GetSeedCreditNotes();

        // Working (mutable) data
        private List<CustomerCreditNoteViewModel> _creditNotes;

        // Counter for credit note number generation
        private int _creditNoteCounter = 100;

        public CustomerCreditNoteService()
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
        public List<CustomerCreditNoteViewModel> GetAll()
        {
            return _creditNotes.Where(cn => !cn.IsDeleted).ToList();
        }

        /// <summary>Get credit note by ID</summary>
        public CustomerCreditNoteViewModel? GetById(Guid id)
        {
            return _creditNotes.FirstOrDefault(cn => cn.Id == id && !cn.IsDeleted);
        }

        /// <summary>Get credit notes by company ID</summary>
        public List<CustomerCreditNoteViewModel> GetByCompanyId(Guid companyId)
        {
            return _creditNotes.Where(cn => cn.CompanyId == companyId && !cn.IsDeleted).ToList();
        }

        /// <summary>Get credit notes by customer ID</summary>
        public List<CustomerCreditNoteViewModel> GetByCustomerId(Guid customerId)
        {
            return _creditNotes.Where(cn => cn.CustomerId == customerId && !cn.IsDeleted).ToList();
        }

        /// <summary>Get credit notes by status</summary>
        public List<CustomerCreditNoteViewModel> GetByStatus(string status)
        {
            return _creditNotes.Where(cn => cn.CreditNoteStatus == status && !cn.IsDeleted).ToList();
        }

        /// <summary>Get credit notes by branch ID</summary>
        public List<CustomerCreditNoteViewModel> GetByBranchId(Guid branchId)
        {
            return _creditNotes.Where(cn => cn.BranchId == branchId && !cn.IsDeleted).ToList();
        }

        /// <summary>Get credit notes by invoice ID</summary>
        public List<CustomerCreditNoteViewModel> GetByInvoiceId(Guid invoiceId)
        {
            return _creditNotes.Where(cn => cn.CustomerInvoiceId == invoiceId && !cn.IsDeleted).ToList();
        }

        /// <summary>Get credit notes by date range</summary>
        public List<CustomerCreditNoteViewModel> GetByDateRange(DateTime fromDate, DateTime toDate)
        {
            return _creditNotes.Where(cn =>
                cn.CreditNoteDate >= fromDate &&
                cn.CreditNoteDate <= toDate &&
                !cn.IsDeleted).ToList();
        }

        /// <summary>Search credit notes by number, customer code/name, reference</summary>
        public List<CustomerCreditNoteViewModel> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAll();

            searchTerm = searchTerm.ToLower();
            return _creditNotes.Where(cn => !cn.IsDeleted && (
                cn.CreditNoteNumber.ToLower().Contains(searchTerm) ||
                (cn.CustomerCode?.ToLower().Contains(searchTerm) ?? false) ||
                (cn.CustomerName?.ToLower().Contains(searchTerm) ?? false) ||
                (cn.ReferenceText?.ToLower().Contains(searchTerm) ?? false) ||
                (cn.CustomerInvoiceNumber?.ToLower().Contains(searchTerm) ?? false)
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

        /// <summary>Generate next credit note number</summary>
        public string GenerateCreditNoteNumber(Guid companyId)
        {
            _creditNoteCounter++;
            var year = DateTime.Today.Year;
            return $"CN-{year}-{_creditNoteCounter:D4}";
        }

        #endregion

        #region Write Operations

        /// <summary>Add new credit note</summary>
        public (bool Success, string Message) Add(CustomerCreditNoteViewModel creditNote)
        {
            // Validate credit note number uniqueness
            if (CreditNoteNumberExists(creditNote.CompanyId, creditNote.CreditNoteNumber))
            {
                return (false, "Credit Note number already exists.");
            }

            // Validate customer
            if (creditNote.CustomerId == Guid.Empty)
            {
                return (false, "Customer is required.");
            }

            // Validate branch
            if (creditNote.BranchId == Guid.Empty)
            {
                return (false, "Branch is required.");
            }

            // Validate reason code
            if (string.IsNullOrWhiteSpace(creditNote.CreditReasonCode))
            {
                return (false, "Credit Reason Code is required.");
            }

            // Validate at least one line for non-draft credit notes
            if (creditNote.CreditNoteStatus != CreditNoteStatuses.Draft && !creditNote.Lines.Any())
            {
                return (false, "Credit Note must have at least one line.");
            }

            // Validate totals
            if (creditNote.CreditNoteStatus != CreditNoteStatuses.Draft && creditNote.GrandTotalAmount <= 0)
            {
                return (false, "Credit Note total must be greater than zero.");
            }

            // Validate invoice reference if against invoice
            if (creditNote.IsAgainstInvoice && creditNote.CustomerInvoiceId == null)
            {
                return (false, "Reference Invoice is required when 'Against Invoice' is enabled.");
            }

            creditNote.Id = Guid.NewGuid();
            creditNote.CreditNoteNumber = creditNote.CreditNoteNumber.ToUpper().Trim();
            creditNote.CreatedAt = DateTime.Now;
            creditNote.IsDeleted = false;

            // Generate credit note number if empty
            if (string.IsNullOrWhiteSpace(creditNote.CreditNoteNumber))
            {
                creditNote.CreditNoteNumber = GenerateCreditNoteNumber(creditNote.CompanyId);
            }

            // Set line credit note IDs
            foreach (var line in creditNote.Lines)
            {
                line.Id = Guid.NewGuid();
                line.CustomerCreditNoteId = creditNote.Id;
                line.CreatedAt = DateTime.Now;
                line.CreatedBy = creditNote.CreatedBy;
            }

            // Recalculate totals
            creditNote.RecalculateTotals();

            _creditNotes.Add(creditNote);
            return (true, "Credit Note created successfully.");
        }

        /// <summary>Update existing credit note</summary>
        public (bool Success, string Message) Update(CustomerCreditNoteViewModel creditNote)
        {
            var existing = _creditNotes.FirstOrDefault(cn => cn.Id == creditNote.Id && !cn.IsDeleted);
            if (existing == null)
            {
                return (false, "Credit Note not found.");
            }

            // Cannot edit posted or cancelled credit notes
            if (!existing.CanEdit)
            {
                return (false, "Cannot edit a posted or cancelled credit note.");
            }

            // Validate credit note number uniqueness (excluding current)
            if (CreditNoteNumberExists(creditNote.CompanyId, creditNote.CreditNoteNumber, creditNote.Id))
            {
                return (false, "Credit Note number already exists.");
            }

            // Validate customer
            if (creditNote.CustomerId == Guid.Empty)
            {
                return (false, "Customer is required.");
            }

            // Validate branch
            if (creditNote.BranchId == Guid.Empty)
            {
                return (false, "Branch is required.");
            }

            // Validate reason code
            if (string.IsNullOrWhiteSpace(creditNote.CreditReasonCode))
            {
                return (false, "Credit Reason Code is required.");
            }

            // Validate invoice reference if against invoice
            if (creditNote.IsAgainstInvoice && creditNote.CustomerInvoiceId == null)
            {
                return (false, "Reference Invoice is required when 'Against Invoice' is enabled.");
            }

            // Update fields
            existing.CompanyId = creditNote.CompanyId;
            existing.CompanyName = creditNote.CompanyName;
            existing.BranchId = creditNote.BranchId;
            existing.BranchCode = creditNote.BranchCode;
            existing.BranchName = creditNote.BranchName;
            existing.CustomerId = creditNote.CustomerId;
            existing.CustomerCode = creditNote.CustomerCode;
            existing.CustomerName = creditNote.CustomerName;
            existing.CustomerAccountId = creditNote.CustomerAccountId;
            existing.CreditNoteNumber = creditNote.CreditNoteNumber.ToUpper().Trim();
            existing.CreditNoteDate = creditNote.CreditNoteDate;
            existing.CurrencyId = creditNote.CurrencyId;
            existing.CurrencyCode = creditNote.CurrencyCode;
            existing.CurrencyName = creditNote.CurrencyName;
            existing.ExchangeRate = creditNote.ExchangeRate;
            existing.ReferenceText = creditNote.ReferenceText;
            existing.CreditNoteNarration = creditNote.CreditNoteNarration;
            existing.IsAgainstInvoice = creditNote.IsAgainstInvoice;
            existing.CustomerInvoiceId = creditNote.CustomerInvoiceId;
            existing.CustomerInvoiceNumber = creditNote.CustomerInvoiceNumber;
            existing.InvoiceNumberSnapshot = creditNote.InvoiceNumberSnapshot;
            existing.InvoiceDateSnapshot = creditNote.InvoiceDateSnapshot;
            existing.CreditReasonCode = creditNote.CreditReasonCode;
            existing.CreditReasonDescription = creditNote.CreditReasonDescription;
            existing.IsTaxImpacting = creditNote.IsTaxImpacting;
            existing.IsRevenueReversal = creditNote.IsRevenueReversal;
            existing.RevenueReversalAccountId = creditNote.RevenueReversalAccountId;
            existing.RevenueReversalAccountCode = creditNote.RevenueReversalAccountCode;
            existing.RevenueReversalAccountName = creditNote.RevenueReversalAccountName;
            existing.Lines = creditNote.Lines;
            existing.UpdatedAt = DateTime.Now;
            existing.UpdatedBy = creditNote.UpdatedBy;

            // Recalculate totals
            existing.RecalculateTotals();

            return (true, "Credit Note updated successfully.");
        }

        /// <summary>Delete credit note (soft delete)</summary>
        public (bool Success, string Message) Delete(Guid id, string userName)
        {
            var creditNote = _creditNotes.FirstOrDefault(cn => cn.Id == id && !cn.IsDeleted);
            if (creditNote == null)
            {
                return (false, "Credit Note not found.");
            }

            // Cannot delete posted credit notes
            if (creditNote.CreditNoteStatus == CreditNoteStatuses.Posted)
            {
                return (false, "Cannot delete a posted credit note.");
            }

            creditNote.IsDeleted = true;
            creditNote.DeletedAt = DateTime.Now;
            creditNote.DeletedBy = userName;

            return (true, "Credit Note deleted successfully.");
        }

        #endregion

        #region Workflow Operations

        /// <summary>Submit credit note for approval</summary>
        public (bool Success, string Message) Submit(Guid id, string userName)
        {
            var creditNote = _creditNotes.FirstOrDefault(cn => cn.Id == id && !cn.IsDeleted);
            if (creditNote == null)
            {
                return (false, "Credit Note not found.");
            }

            if (creditNote.CreditNoteStatus != CreditNoteStatuses.Draft)
            {
                return (false, "Only draft credit notes can be submitted.");
            }

            // Validate at least one line
            if (!creditNote.Lines.Any())
            {
                return (false, "Credit Note must have at least one line.");
            }

            // Validate totals
            if (creditNote.GrandTotalAmount <= 0)
            {
                return (false, "Credit Note total must be greater than zero.");
            }

            creditNote.CreditNoteStatus = CreditNoteStatuses.Submitted;
            creditNote.UpdatedAt = DateTime.Now;
            creditNote.UpdatedBy = userName;

            return (true, "Credit Note submitted for approval.");
        }

        /// <summary>Approve credit note</summary>
        public (bool Success, string Message) Approve(Guid id, string userName)
        {
            var creditNote = _creditNotes.FirstOrDefault(cn => cn.Id == id && !cn.IsDeleted);
            if (creditNote == null)
            {
                return (false, "Credit Note not found.");
            }

            if (creditNote.CreditNoteStatus != CreditNoteStatuses.Submitted)
            {
                return (false, "Only submitted credit notes can be approved.");
            }

            creditNote.CreditNoteStatus = CreditNoteStatuses.Approved;
            creditNote.ApprovedOn = DateTime.Now;
            creditNote.ApprovedByUserName = userName;
            creditNote.UpdatedAt = DateTime.Now;
            creditNote.UpdatedBy = userName;

            return (true, "Credit Note approved.");
        }

        /// <summary>Post credit note</summary>
        public (bool Success, string Message) Post(Guid id, Guid receivableAccountId, string userName)
        {
            var creditNote = _creditNotes.FirstOrDefault(cn => cn.Id == id && !cn.IsDeleted);
            if (creditNote == null)
            {
                return (false, "Credit Note not found.");
            }

            if (!creditNote.CanPost)
            {
                return (false, "Credit Note cannot be posted in its current status.");
            }

            // Validate at least one line
            if (!creditNote.Lines.Any())
            {
                return (false, "Credit Note must have at least one line.");
            }

            // Validate totals
            if (creditNote.GrandTotalAmount <= 0)
            {
                return (false, "Credit Note total must be greater than zero.");
            }

            // Validate posting date
            if (creditNote.PostingDate.HasValue && creditNote.PostingDate.Value < creditNote.CreditNoteDate)
            {
                return (false, "Posting date cannot be before credit note date.");
            }

            creditNote.CreditNoteStatus = CreditNoteStatuses.Posted;
            creditNote.PostingDate = creditNote.PostingDate ?? DateTime.Today;
            creditNote.PostedOn = DateTime.Now;
            creditNote.PostedByUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            creditNote.PostedByUserName = userName;
            creditNote.ReceivableAccountIdSnapshot = receivableAccountId;
            creditNote.AppliedToInvoiceAmount = creditNote.IsAgainstInvoice ? creditNote.GrandTotalAmount : 0;
            creditNote.UpdatedAt = DateTime.Now;
            creditNote.UpdatedBy = userName;

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
                return (false, "Only draft or submitted credit notes can be cancelled.");
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return (false, "Cancellation reason is required.");
            }

            creditNote.CreditNoteStatus = CreditNoteStatuses.Cancelled;
            creditNote.CancellationReason = reason;
            creditNote.CancelledOn = DateTime.Now;
            creditNote.CancelledByUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            creditNote.CancelledByUserName = userName;
            creditNote.UpdatedAt = DateTime.Now;
            creditNote.UpdatedBy = userName;

            return (true, "Credit Note cancelled.");
        }

        /// <summary>Reject credit note (send back to draft)</summary>
        public (bool Success, string Message) Reject(Guid id, string reason, string userName)
        {
            var creditNote = _creditNotes.FirstOrDefault(cn => cn.Id == id && !cn.IsDeleted);
            if (creditNote == null)
            {
                return (false, "Credit Note not found.");
            }

            if (creditNote.CreditNoteStatus != CreditNoteStatuses.Submitted)
            {
                return (false, "Only submitted credit notes can be rejected.");
            }

            creditNote.CreditNoteStatus = CreditNoteStatuses.Draft;
            creditNote.CreditNoteNarration = $"{creditNote.CreditNoteNarration}\n[Rejected: {reason}]";
            creditNote.UpdatedAt = DateTime.Now;
            creditNote.UpdatedBy = userName;

            return (true, "Credit Note rejected and returned to draft.");
        }

        #endregion

        #region Line Operations

        /// <summary>Add line to credit note</summary>
        public (bool Success, string Message) AddLine(Guid creditNoteId, CustomerCreditNoteLineModel line)
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

            // Set line number
            var maxLineNumber = creditNote.Lines.Any() ? creditNote.Lines.Max(l => l.LineNumber) : 0;
            line.LineNumber = maxLineNumber + 10;
            line.Id = Guid.NewGuid();
            line.CustomerCreditNoteId = creditNoteId;
            line.CreatedAt = DateTime.Now;

            // Calculate line amounts
            line.RecalculateAmounts();

            creditNote.Lines.Add(line);
            creditNote.RecalculateTotals();
            creditNote.UpdatedAt = DateTime.Now;

            return (true, "Line added successfully.");
        }

        /// <summary>Update line in credit note</summary>
        public (bool Success, string Message) UpdateLine(CustomerCreditNoteLineModel line)
        {
            var creditNote = _creditNotes.FirstOrDefault(cn => cn.Id == line.CustomerCreditNoteId && !cn.IsDeleted);
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
            existingLine.RevenueReversalAccountId = line.RevenueReversalAccountId;
            existingLine.RevenueReversalAccountCode = line.RevenueReversalAccountCode;
            existingLine.RevenueReversalAccountName = line.RevenueReversalAccountName;
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
        public List<CustomerCreditNoteLineModel> GetLines(Guid creditNoteId)
        {
            var creditNote = _creditNotes.FirstOrDefault(cn => cn.Id == creditNoteId && !cn.IsDeleted);
            return creditNote?.Lines ?? new List<CustomerCreditNoteLineModel>();
        }

        #endregion

        #region Statistics

        /// <summary>Get credit note statistics</summary>
        public CreditNoteStatisticsViewModel GetStatistics(Guid? companyId = null)
        {
            var creditNotes = companyId.HasValue
                ? _creditNotes.Where(cn => cn.CompanyId == companyId.Value && !cn.IsDeleted)
                : _creditNotes.Where(cn => !cn.IsDeleted);

            return new CreditNoteStatisticsViewModel
            {
                TotalCreditNotes = creditNotes.Count(),
                DraftCreditNotes = creditNotes.Count(cn => cn.CreditNoteStatus == CreditNoteStatuses.Draft),
                SubmittedCreditNotes = creditNotes.Count(cn => cn.CreditNoteStatus == CreditNoteStatuses.Submitted),
                ApprovedCreditNotes = creditNotes.Count(cn => cn.CreditNoteStatus == CreditNoteStatuses.Approved),
                PostedCreditNotes = creditNotes.Count(cn => cn.CreditNoteStatus == CreditNoteStatuses.Posted),
                CancelledCreditNotes = creditNotes.Count(cn => cn.CreditNoteStatus == CreditNoteStatuses.Cancelled),
                TotalCreditNoteAmount = creditNotes.Where(cn => cn.CreditNoteStatus == CreditNoteStatuses.Posted).Sum(cn => cn.GrandTotalAmount),
                TotalAppliedAmount = creditNotes.Where(cn => cn.CreditNoteStatus == CreditNoteStatuses.Posted).Sum(cn => cn.AppliedToInvoiceAmount)
            };
        }

        #endregion
    }

    /// <summary>
    /// Credit Note statistics model
    /// </summary>
    public class CreditNoteStatisticsViewModel
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
