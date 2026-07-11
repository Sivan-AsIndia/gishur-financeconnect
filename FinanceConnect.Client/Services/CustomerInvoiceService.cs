using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    /// <summary>
    /// Service for CustomerInvoice (Model #27) CRUD operations
    /// Demo application - data stored in memory
    /// </summary>
    public class CustomerInvoiceService
    {
        // Immutable seed data
        private static readonly List<CustomerInvoiceViewModel> _seedInvoices = CustomerInvoiceSeedData.GetSeedInvoices();

        // Working (mutable) data
        private List<CustomerInvoiceViewModel> _invoices;

        // Counter for invoice number generation
        private int _invoiceCounter = 100;

        public CustomerInvoiceService()
        {
            ResetToSeed();
        }

        private static List<T> CloneList<T>(IEnumerable<T> source) where T : class
        {
            var json = System.Text.Json.JsonSerializer.Serialize(source);
            return System.Text.Json.JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }

        /// <summary>Reset invoices to seed data</summary>
        public void ResetToSeed()
        {
            _invoices = CloneList(_seedInvoices);
        }

        #region Read Operations

        /// <summary>Get all invoices</summary>
        public List<CustomerInvoiceViewModel> GetAll()
        {
            return _invoices.Where(i => !i.IsDeleted).ToList();
        }

        /// <summary>Get invoice by ID</summary>
        public CustomerInvoiceViewModel? GetById(Guid id)
        {
            return _invoices.FirstOrDefault(i => i.Id == id && !i.IsDeleted);
        }

        /// <summary>Get invoices by company ID</summary>
        public List<CustomerInvoiceViewModel> GetByCompanyId(Guid companyId)
        {
            return _invoices.Where(i => i.CompanyId == companyId && !i.IsDeleted).ToList();
        }

        /// <summary>Get invoices by customer ID</summary>
        public List<CustomerInvoiceViewModel> GetByCustomerId(Guid customerId)
        {
            return _invoices.Where(i => i.CustomerId == customerId && !i.IsDeleted).ToList();
        }

        /// <summary>Get invoices by status</summary>
        public List<CustomerInvoiceViewModel> GetByStatus(string status)
        {
            return _invoices.Where(i => i.InvoiceStatus == status && !i.IsDeleted).ToList();
        }

        /// <summary>Get invoices by branch ID</summary>
        public List<CustomerInvoiceViewModel> GetByBranchId(Guid branchId)
        {
            return _invoices.Where(i => i.BranchId == branchId && !i.IsDeleted).ToList();
        }

        /// <summary>Get invoices by date range</summary>
        public List<CustomerInvoiceViewModel> GetByDateRange(DateTime fromDate, DateTime toDate)
        {
            return _invoices.Where(i =>
                i.InvoiceDate >= fromDate &&
                i.InvoiceDate <= toDate &&
                !i.IsDeleted).ToList();
        }

        /// <summary>Get overdue invoices</summary>
        public List<CustomerInvoiceViewModel> GetOverdueInvoices()
        {
            var today = DateTime.Today;
            return _invoices.Where(i =>
                i.DueDate < today &&
                i.AmountOutstanding > 0 &&
                (i.InvoiceStatus == InvoiceStatuses.Posted ||
                 i.InvoiceStatus == InvoiceStatuses.PartiallyPaid) &&
                !i.IsDeleted).ToList();
        }

        /// <summary>Search invoices by number, customer code/name, reference</summary>
        public List<CustomerInvoiceViewModel> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAll();

            searchTerm = searchTerm.ToLower();
            return _invoices.Where(i => !i.IsDeleted && (
                i.InvoiceNumber.ToLower().Contains(searchTerm) ||
                (i.CustomerCode?.ToLower().Contains(searchTerm) ?? false) ||
                (i.CustomerName?.ToLower().Contains(searchTerm) ?? false) ||
                (i.ReferenceText?.ToLower().Contains(searchTerm) ?? false)
            )).ToList();
        }

        /// <summary>Check if invoice number exists within company</summary>
        public bool InvoiceNumberExists(Guid companyId, string invoiceNumber, Guid? excludeId = null)
        {
            return _invoices.Any(i =>
                i.CompanyId == companyId &&
                i.InvoiceNumber.Equals(invoiceNumber, StringComparison.OrdinalIgnoreCase) &&
                !i.IsDeleted &&
                (excludeId == null || i.Id != excludeId));
        }

        /// <summary>Generate next invoice number</summary>
        public string GenerateInvoiceNumber(Guid companyId, string invoiceType)
        {
            _invoiceCounter++;
            var prefix = invoiceType switch
            {
                InvoiceTypes.Proforma => "PI",
                InvoiceTypes.Export => "EXP",
                InvoiceTypes.SEZ => "SEZ",
                _ => "INV"
            };
            var year = DateTime.Today.Year;
            return $"{prefix}-{year}-{_invoiceCounter:D4}";
        }

        #endregion

        #region Write Operations

        /// <summary>Add new invoice</summary>
        public (bool Success, string Message) Add(CustomerInvoiceViewModel invoice)
        {
            // Validate invoice number uniqueness
            if (InvoiceNumberExists(invoice.CompanyId, invoice.InvoiceNumber))
            {
                return (false, "Invoice number already exists.");
            }

            // Validate customer
            if (invoice.CustomerId == Guid.Empty)
            {
                return (false, "Customer is required.");
            }

            // Validate branch
            if (invoice.BranchId == Guid.Empty)
            {
                return (false, "Branch is required.");
            }

            // Validate at least one line for non-draft invoices
            if (invoice.InvoiceStatus != InvoiceStatuses.Draft && !invoice.Lines.Any())
            {
                return (false, "Invoice must have at least one line.");
            }

            // Validate totals
            if (invoice.InvoiceStatus != InvoiceStatuses.Draft && invoice.GrandTotalAmount <= 0)
            {
                return (false, "Invoice total must be greater than zero.");
            }

            // Validate due date
            if (invoice.DueDate < invoice.InvoiceDate)
            {
                return (false, "Due date cannot be before invoice date.");
            }

            invoice.Id = Guid.NewGuid();
            invoice.InvoiceNumber = invoice.InvoiceNumber.ToUpper().Trim();
            invoice.CreatedAt = DateTime.Now;
            invoice.IsDeleted = false;

            // Generate invoice number if empty
            if (string.IsNullOrWhiteSpace(invoice.InvoiceNumber))
            {
                invoice.InvoiceNumber = GenerateInvoiceNumber(invoice.CompanyId, invoice.InvoiceType);
            }

            // Set line invoice IDs
            foreach (var line in invoice.Lines)
            {
                line.Id = Guid.NewGuid();
                line.CustomerInvoiceId = invoice.Id;
                line.CreatedAt = DateTime.Now;
                line.CreatedBy = invoice.CreatedBy;
            }

            // Recalculate totals
            invoice.RecalculateTotals();

            _invoices.Add(invoice);
            return (true, "Invoice created successfully.");
        }

        /// <summary>Update existing invoice</summary>
        public (bool Success, string Message) Update(CustomerInvoiceViewModel invoice)
        {
            var existing = _invoices.FirstOrDefault(i => i.Id == invoice.Id && !i.IsDeleted);
            if (existing == null)
            {
                return (false, "Invoice not found.");
            }

            // Check if invoice can be edited
            if (!existing.CanEdit)
            {
                return (false, "Posted or cancelled invoices cannot be edited.");
            }

            // Validate invoice number uniqueness
            if (InvoiceNumberExists(invoice.CompanyId, invoice.InvoiceNumber, invoice.Id))
            {
                return (false, "Invoice number already exists.");
            }

            // Validate due date
            if (invoice.DueDate < invoice.InvoiceDate)
            {
                return (false, "Due date cannot be before invoice date.");
            }

            // Update fields
            existing.BranchId = invoice.BranchId;
            existing.BranchCode = invoice.BranchCode;
            existing.BranchName = invoice.BranchName;
            existing.CustomerId = invoice.CustomerId;
            existing.CustomerCode = invoice.CustomerCode;
            existing.CustomerName = invoice.CustomerName;
            existing.CustomerAccountId = invoice.CustomerAccountId;
            existing.InvoiceNumber = invoice.InvoiceNumber.ToUpper().Trim();
            existing.InvoiceType = invoice.InvoiceType;
            existing.InvoiceDate = invoice.InvoiceDate;
            existing.PaymentTermId = invoice.PaymentTermId;
            existing.PaymentTermName = invoice.PaymentTermName;
            existing.PaymentTermDays = invoice.PaymentTermDays;
            existing.DueDate = invoice.DueDate;
            existing.CurrencyId = invoice.CurrencyId;
            existing.CurrencyCode = invoice.CurrencyCode;
            existing.CurrencyName = invoice.CurrencyName;
            existing.ExchangeRate = invoice.ExchangeRate;
            existing.ReferenceText = invoice.ReferenceText;
            existing.InvoiceNarration = invoice.InvoiceNarration;
            existing.ReceivableAccountId = invoice.ReceivableAccountId;
            existing.ReceivableAccountCode = invoice.ReceivableAccountCode;
            existing.ReceivableAccountName = invoice.ReceivableAccountName;
            existing.PlaceOfSupplyStateId = invoice.PlaceOfSupplyStateId;
            existing.PlaceOfSupplyStateName = invoice.PlaceOfSupplyStateName;
            existing.PlaceOfSupplyStateCode = invoice.PlaceOfSupplyStateCode;
            existing.SupplyType = invoice.SupplyType;
            existing.Lines = invoice.Lines;
            existing.UpdatedAt = DateTime.Now;
            existing.UpdatedBy = invoice.UpdatedBy;

            // Recalculate totals
            existing.RecalculateTotals();

            return (true, "Invoice updated successfully.");
        }

        /// <summary>Delete invoice (soft delete - only if draft)</summary>
        public (bool Success, string Message) Delete(Guid id)
        {
            var invoice = _invoices.FirstOrDefault(i => i.Id == id && !i.IsDeleted);
            if (invoice == null)
            {
                return (false, "Invoice not found.");
            }

            if (invoice.InvoiceStatus != InvoiceStatuses.Draft)
            {
                return (false, "Only draft invoices can be deleted.");
            }

            invoice.IsDeleted = true;
            invoice.DeletedAt = DateTime.Now;

            return (true, "Invoice deleted successfully.");
        }

        /// <summary>Post invoice</summary>
        public (bool Success, string Message) Post(Guid invoiceId, Guid userId, string userName)
        {
            var invoice = _invoices.FirstOrDefault(i => i.Id == invoiceId && !i.IsDeleted);
            if (invoice == null)
            {
                return (false, "Invoice not found.");
            }

            if (!invoice.CanPost)
            {
                return (false, "Invoice cannot be posted in its current status.");
            }

            // Validate at least one line
            if (!invoice.Lines.Any())
            {
                return (false, "Invoice must have at least one line.");
            }

            // Validate totals
            if (invoice.GrandTotalAmount <= 0)
            {
                return (false, "Invoice total must be greater than zero.");
            }

            // Validate line totals match header
            invoice.RecalculateTotals();

            // Post the invoice
            invoice.InvoiceStatus = InvoiceStatuses.Posted;
            invoice.PostingDate = DateTime.Today;
            invoice.PostedOn = DateTime.Now;
            invoice.PostedByUserId = userId;
            invoice.PostedByUserName = userName;
            invoice.ApprovalStatus = ApprovalStatuses.Approved;
            invoice.UpdatedAt = DateTime.Now;
            invoice.UpdatedBy = userName;

            return (true, "Invoice posted successfully.");
        }

        /// <summary>Cancel invoice</summary>
        public (bool Success, string Message) Cancel(Guid invoiceId, string reason, Guid userId, string userName)
        {
            var invoice = _invoices.FirstOrDefault(i => i.Id == invoiceId && !i.IsDeleted);
            if (invoice == null)
            {
                return (false, "Invoice not found.");
            }

            if (!invoice.CanCancel)
            {
                return (false, "Invoice cannot be cancelled in its current status.");
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return (false, "Cancellation reason is required.");
            }

            invoice.InvoiceStatus = InvoiceStatuses.Cancelled;
            invoice.CancelledOn = DateTime.Now;
            invoice.CancelledByUserId = userId;
            invoice.CancelledByUserName = userName;
            invoice.CancellationReason = reason;
            invoice.UpdatedAt = DateTime.Now;
            invoice.UpdatedBy = userName;

            return (true, "Invoice cancelled successfully.");
        }

        /// <summary>Submit invoice for approval</summary>
        public (bool Success, string Message) Submit(Guid invoiceId, string userName)
        {
            var invoice = _invoices.FirstOrDefault(i => i.Id == invoiceId && !i.IsDeleted);
            if (invoice == null)
            {
                return (false, "Invoice not found.");
            }

            if (invoice.InvoiceStatus != InvoiceStatuses.Draft)
            {
                return (false, "Only draft invoices can be submitted.");
            }

            // Validate at least one line
            if (!invoice.Lines.Any())
            {
                return (false, "Invoice must have at least one line.");
            }

            invoice.InvoiceStatus = InvoiceStatuses.Submitted;
            invoice.ApprovalStatus = ApprovalStatuses.Pending;
            invoice.UpdatedAt = DateTime.Now;
            invoice.UpdatedBy = userName;

            return (true, "Invoice submitted for approval.");
        }

        /// <summary>Approve invoice</summary>
        public (bool Success, string Message) Approve(Guid invoiceId, string userName)
        {
            var invoice = _invoices.FirstOrDefault(i => i.Id == invoiceId && !i.IsDeleted);
            if (invoice == null)
            {
                return (false, "Invoice not found.");
            }

            if (invoice.InvoiceStatus != InvoiceStatuses.Submitted)
            {
                return (false, "Only submitted invoices can be approved.");
            }

            invoice.InvoiceStatus = InvoiceStatuses.Approved;
            invoice.ApprovalStatus = ApprovalStatuses.Approved;
            invoice.UpdatedAt = DateTime.Now;
            invoice.UpdatedBy = userName;

            return (true, "Invoice approved successfully.");
        }

        /// <summary>Reject invoice</summary>
        public (bool Success, string Message) Reject(Guid invoiceId, string reason, string userName)
        {
            var invoice = _invoices.FirstOrDefault(i => i.Id == invoiceId && !i.IsDeleted);
            if (invoice == null)
            {
                return (false, "Invoice not found.");
            }

            if (invoice.InvoiceStatus != InvoiceStatuses.Submitted)
            {
                return (false, "Only submitted invoices can be rejected.");
            }

            invoice.InvoiceStatus = InvoiceStatuses.Draft;
            invoice.ApprovalStatus = ApprovalStatuses.Rejected;
            invoice.InvoiceNarration = $"{invoice.InvoiceNarration}\n[Rejected: {reason}]";
            invoice.UpdatedAt = DateTime.Now;
            invoice.UpdatedBy = userName;

            return (true, "Invoice rejected and returned to draft.");
        }

        #endregion

        #region Line Operations

        /// <summary>Add line to invoice</summary>
        public (bool Success, string Message) AddLine(Guid invoiceId, CustomerInvoiceLineViewModel line)
        {
            var invoice = _invoices.FirstOrDefault(i => i.Id == invoiceId && !i.IsDeleted);
            if (invoice == null)
            {
                return (false, "Invoice not found.");
            }

            if (!invoice.CanEdit)
            {
                return (false, "Lines cannot be added to posted or cancelled invoices.");
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
            var maxLineNumber = invoice.Lines.Any() ? invoice.Lines.Max(l => l.LineNumber) : 0;
            line.LineNumber = maxLineNumber + 10;
            line.Id = Guid.NewGuid();
            line.CustomerInvoiceId = invoiceId;
            line.CreatedAt = DateTime.Now;

            // Calculate line amounts
            line.RecalculateAmounts();

            invoice.Lines.Add(line);
            invoice.RecalculateTotals();
            invoice.UpdatedAt = DateTime.Now;

            return (true, "Line added successfully.");
        }

        /// <summary>Update line in invoice</summary>
        public (bool Success, string Message) UpdateLine(CustomerInvoiceLineViewModel line)
        {
            var invoice = _invoices.FirstOrDefault(i => i.Id == line.CustomerInvoiceId && !i.IsDeleted);
            if (invoice == null)
            {
                return (false, "Invoice not found.");
            }

            if (!invoice.CanEdit)
            {
                return (false, "Lines cannot be updated on posted or cancelled invoices.");
            }

            var existingLine = invoice.Lines.FirstOrDefault(l => l.Id == line.Id);
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

            // Recalculate invoice totals
            invoice.RecalculateTotals();
            invoice.UpdatedAt = DateTime.Now;

            return (true, "Line updated successfully.");
        }

        /// <summary>Delete line from invoice</summary>
        public (bool Success, string Message) DeleteLine(Guid invoiceId, Guid lineId)
        {
            var invoice = _invoices.FirstOrDefault(i => i.Id == invoiceId && !i.IsDeleted);
            if (invoice == null)
            {
                return (false, "Invoice not found.");
            }

            if (!invoice.CanEdit)
            {
                return (false, "Lines cannot be deleted from posted or cancelled invoices.");
            }

            var line = invoice.Lines.FirstOrDefault(l => l.Id == lineId);
            if (line == null)
            {
                return (false, "Line not found.");
            }

            invoice.Lines.Remove(line);
            invoice.RecalculateTotals();
            invoice.UpdatedAt = DateTime.Now;

            return (true, "Line deleted successfully.");
        }

        /// <summary>Get lines for invoice</summary>
        public List<CustomerInvoiceLineViewModel> GetLines(Guid invoiceId)
        {
            var invoice = _invoices.FirstOrDefault(i => i.Id == invoiceId && !i.IsDeleted);
            return invoice?.Lines ?? new List<CustomerInvoiceLineViewModel>();
        }

        #endregion

        #region Statistics

        /// <summary>Get invoice statistics</summary>
        public InvoiceStatisticsViewModel GetStatistics(Guid? companyId = null)
        {
            var invoices = companyId.HasValue
                ? _invoices.Where(i => i.CompanyId == companyId.Value && !i.IsDeleted)
                : _invoices.Where(i => !i.IsDeleted);

            var today = DateTime.Today;

            return new InvoiceStatisticsViewModel
            {
                TotalInvoices = invoices.Count(),
                DraftInvoices = invoices.Count(i => i.InvoiceStatus == InvoiceStatuses.Draft),
                PostedInvoices = invoices.Count(i => i.InvoiceStatus == InvoiceStatuses.Posted),
                PaidInvoices = invoices.Count(i => i.InvoiceStatus == InvoiceStatuses.Paid),
                PartiallyPaidInvoices = invoices.Count(i => i.InvoiceStatus == InvoiceStatuses.PartiallyPaid),
                CancelledInvoices = invoices.Count(i => i.InvoiceStatus == InvoiceStatuses.Cancelled),
                OverdueInvoices = invoices.Count(i => 
                    i.DueDate < today && 
                    i.AmountOutstanding > 0 && 
                    (i.InvoiceStatus == InvoiceStatuses.Posted || i.InvoiceStatus == InvoiceStatuses.PartiallyPaid)),
                TotalInvoiceAmount = invoices.Sum(i => i.GrandTotalAmount),
                TotalOutstandingAmount = invoices.Sum(i => i.AmountOutstanding),
                TotalPaidAmount = invoices.Sum(i => i.AmountPaidToDate)
            };
        }

        #endregion
    }

    /// <summary>
    /// Invoice statistics model
    /// </summary>
    public class InvoiceStatisticsViewModel
    {
        public int TotalInvoices { get; set; }
        public int DraftInvoices { get; set; }
        public int PostedInvoices { get; set; }
        public int PaidInvoices { get; set; }
        public int PartiallyPaidInvoices { get; set; }
        public int CancelledInvoices { get; set; }
        public int OverdueInvoices { get; set; }
        public decimal TotalInvoiceAmount { get; set; }
        public decimal TotalOutstandingAmount { get; set; }
        public decimal TotalPaidAmount { get; set; }
    }
}
