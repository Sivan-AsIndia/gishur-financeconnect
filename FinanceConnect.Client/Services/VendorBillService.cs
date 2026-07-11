using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    /// <summary>
    /// Service for VendorBill (Model #36) and VendorBillLine (Model #37) CRUD operations
    /// Demo application - data stored in memory
    /// </summary>
    public class VendorBillService
    {
        // Immutable seed data
        private static readonly List<VendorBillViewModel> _seedBills = VendorBillSeedData.GetSeedBills();

        // Working (mutable) data
        private List<VendorBillViewModel> _bills;

        // Counter for bill number generation
        private int _billCounter = 100;

        public VendorBillService()
        {
            ResetToSeed();
        }

        private static List<T> CloneList<T>(IEnumerable<T> source) where T : class
        {
            var json = System.Text.Json.JsonSerializer.Serialize(source);
            return System.Text.Json.JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }

        /// <summary>Reset bills to seed data</summary>
        public void ResetToSeed()
        {
            _bills = CloneList(_seedBills);
        }

        #region Read Operations

        /// <summary>Get all bills</summary>
        public List<VendorBillViewModel> GetAll()
        {
            return _bills.Where(b => !b.IsDeleted).ToList();
        }

        /// <summary>Get bill by ID</summary>
        public VendorBillViewModel? GetById(Guid id)
        {
            return _bills.FirstOrDefault(b => b.Id == id && !b.IsDeleted);
        }

        /// <summary>Get bills by company ID</summary>
        public List<VendorBillViewModel> GetByCompanyId(Guid companyId)
        {
            return _bills.Where(b => b.CompanyId == companyId && !b.IsDeleted).ToList();
        }

        /// <summary>Get bills by vendor ID</summary>
        public List<VendorBillViewModel> GetByVendorId(Guid vendorId)
        {
            return _bills.Where(b => b.VendorId == vendorId && !b.IsDeleted).ToList();
        }

        /// <summary>Get bills by status</summary>
        public List<VendorBillViewModel> GetByStatus(string status)
        {
            return _bills.Where(b => b.BillStatus == status && !b.IsDeleted).ToList();
        }

        /// <summary>Get bills by branch ID</summary>
        public List<VendorBillViewModel> GetByBranchId(Guid branchId)
        {
            return _bills.Where(b => b.BranchId == branchId && !b.IsDeleted).ToList();
        }

        /// <summary>Get bills by date range</summary>
        public List<VendorBillViewModel> GetByDateRange(DateTime fromDate, DateTime toDate)
        {
            return _bills.Where(b =>
                b.BillDate >= fromDate &&
                b.BillDate <= toDate &&
                !b.IsDeleted).ToList();
        }

        /// <summary>Get overdue bills</summary>
        public List<VendorBillViewModel> GetOverdueBills()
        {
            var today = DateTime.Today;
            return _bills.Where(b =>
                b.DueDate < today &&
                b.OutstandingAmount > 0 &&
                b.BillStatus == VendorBillStatuses.Posted &&
                !b.IsDeleted).ToList();
        }

        /// <summary>Search bills by number, vendor code/name, vendor invoice number</summary>
        public List<VendorBillViewModel> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAll();

            searchTerm = searchTerm.ToLower();
            return _bills.Where(b => !b.IsDeleted && (
                b.BillNumber.ToLower().Contains(searchTerm) ||
                (b.VendorCode?.ToLower().Contains(searchTerm) ?? false) ||
                (b.VendorName?.ToLower().Contains(searchTerm) ?? false) ||
                (b.VendorInvoiceNumber?.ToLower().Contains(searchTerm) ?? false)
            )).ToList();
        }

        /// <summary>Check if vendor invoice number exists for the vendor within company (critical anti-fraud)</summary>
        public bool VendorInvoiceNumberExists(Guid companyId, Guid vendorId, string vendorInvoiceNumber, Guid? excludeId = null)
        {
            return _bills.Any(b =>
                b.CompanyId == companyId &&
                b.VendorId == vendorId &&
                b.VendorInvoiceNumber.Equals(vendorInvoiceNumber, StringComparison.OrdinalIgnoreCase) &&
                b.BillStatus != VendorBillStatuses.Cancelled &&
                !b.IsDeleted &&
                (excludeId == null || b.Id != excludeId));
        }

        /// <summary>Check if bill number exists within company</summary>
        public bool BillNumberExists(Guid companyId, string billNumber, Guid? excludeId = null)
        {
            return _bills.Any(b =>
                b.CompanyId == companyId &&
                b.BillNumber.Equals(billNumber, StringComparison.OrdinalIgnoreCase) &&
                !b.IsDeleted &&
                (excludeId == null || b.Id != excludeId));
        }

        /// <summary>Generate next bill number</summary>
        public string GenerateBillNumber(Guid companyId, string billType)
        {
            _billCounter++;
            var year = DateTime.Today.Year;
            return $"APB-{year}-{_billCounter:D4}";
        }

        #endregion

        #region Write Operations

        /// <summary>Add new bill</summary>
        public (bool Success, string Message) Add(VendorBillViewModel bill)
        {
            // Validate bill number uniqueness
            if (BillNumberExists(bill.CompanyId, bill.BillNumber))
            {
                return (false, "Bill number already exists.");
            }

            // Validate vendor
            if (!bill.VendorId.HasValue || bill.VendorId == Guid.Empty)
            {
                return (false, "Vendor is required.");
            }

            // Validate branch
            if (bill.BranchId == Guid.Empty)
            {
                return (false, "Branch is required.");
            }

            // Validate vendor invoice number
            if (string.IsNullOrWhiteSpace(bill.VendorInvoiceNumber))
            {
                return (false, "Vendor Invoice Number is required.");
            }

            // Check for duplicate vendor invoice number (critical anti-fraud check)
            if (VendorInvoiceNumberExists(bill.CompanyId, bill.VendorId.Value, bill.VendorInvoiceNumber))
            {
                return (false, "Duplicate vendor invoice number detected for this vendor.");
            }

            // Validate at least one line for non-draft bills
            if (bill.BillStatus != VendorBillStatuses.Draft && !bill.Lines.Any())
            {
                return (false, "Bill must have at least one line.");
            }

            // Validate totals
            if (bill.BillStatus != VendorBillStatuses.Draft && bill.GrandTotalAmount <= 0)
            {
                return (false, "Bill total must be greater than zero.");
            }

            // Validate due date
            if (bill.DueDate < bill.VendorInvoiceDate)
            {
                return (false, "Due date cannot be before vendor invoice date.");
            }

            bill.Id = Guid.NewGuid();
            bill.BillNumber = bill.BillNumber.ToUpper().Trim();
            bill.VendorInvoiceNumber = bill.VendorInvoiceNumber.Trim();
            bill.CreatedAt = DateTime.Now;
            bill.IsDeleted = false;

            // Generate bill number if empty
            if (string.IsNullOrWhiteSpace(bill.BillNumber))
            {
                bill.BillNumber = GenerateBillNumber(bill.CompanyId, bill.BillType);
            }

            // Set line bill IDs
            foreach (var line in bill.Lines)
            {
                line.Id = Guid.NewGuid();
                line.VendorBillId = bill.Id;
                line.CreatedAt = DateTime.Now;
                line.CreatedBy = bill.CreatedBy;
            }

            // Recalculate totals
            bill.RecalculateTotals();

            _bills.Add(bill);
            return (true, "Bill created successfully.");
        }

        /// <summary>Update existing bill</summary>
        public (bool Success, string Message) Update(VendorBillViewModel bill)
        {
            var existing = _bills.FirstOrDefault(b => b.Id == bill.Id && !b.IsDeleted);
            if (existing == null)
            {
                return (false, "Bill not found.");
            }

            if (!existing.CanEdit)
            {
                return (false, "Posted or cancelled bills cannot be edited.");
            }

            // Validate vendor invoice number uniqueness (excluding current bill)
            if (VendorInvoiceNumberExists(bill.CompanyId, bill.VendorId.Value, bill.VendorInvoiceNumber, bill.Id))
            {
                return (false, "Duplicate vendor invoice number detected for this vendor.");
            }

            // Update fields
            existing.BranchId = bill.BranchId;
            existing.BranchCode = bill.BranchCode;
            existing.BranchName = bill.BranchName;
            existing.VendorId = bill.VendorId;
            existing.VendorCode = bill.VendorCode;
            existing.VendorName = bill.VendorName;
            existing.VendorAccountId = bill.VendorAccountId;
            existing.VendorAccountName = bill.VendorAccountName;
            existing.VendorInvoiceNumber = bill.VendorInvoiceNumber.Trim();
            existing.VendorInvoiceDate = bill.VendorInvoiceDate;
            existing.BillDate = bill.BillDate;
            existing.DueDate = bill.DueDate;
            existing.CurrencyId = bill.CurrencyId;
            existing.CurrencyCode = bill.CurrencyCode;
            existing.CurrencyName = bill.CurrencyName;
            existing.ExchangeRate = bill.ExchangeRate;
            existing.BillType = bill.BillType;
            existing.BillNarration = bill.BillNarration;
            existing.IsGSTApplicable = bill.IsGSTApplicable;
            existing.IsReverseChargeApplicable = bill.IsReverseChargeApplicable;
            existing.IsTDSApplicable = bill.IsTDSApplicable;
            existing.PaymentTermId = bill.PaymentTermId;
            existing.PaymentTermName = bill.PaymentTermName;
            existing.PaymentTermDays = bill.PaymentTermDays;
            existing.PlaceOfSupplyStateId = bill.PlaceOfSupplyStateId;
            existing.PlaceOfSupplyStateName = bill.PlaceOfSupplyStateName;
            existing.PlaceOfSupplyStateCode = bill.PlaceOfSupplyStateCode;

            // Update lines
            existing.Lines = bill.Lines;
            foreach (var line in existing.Lines)
            {
                line.VendorBillId = existing.Id;
                if (line.Id == Guid.Empty)
                {
                    line.Id = Guid.NewGuid();
                    line.CreatedAt = DateTime.Now;
                    line.CreatedBy = bill.UpdatedBy;
                }
            }

            // Recalculate totals
            existing.RecalculateTotals();

            existing.UpdatedAt = DateTime.Now;
            existing.UpdatedBy = bill.UpdatedBy;

            return (true, "Bill updated successfully.");
        }

        /// <summary>Delete bill (soft delete, draft only)</summary>
        public (bool Success, string Message) Delete(Guid id)
        {
            var bill = _bills.FirstOrDefault(b => b.Id == id && !b.IsDeleted);
            if (bill == null)
            {
                return (false, "Bill not found.");
            }

            if (bill.BillStatus != VendorBillStatuses.Draft)
            {
                return (false, "Only draft bills can be deleted.");
            }

            bill.IsDeleted = true;
            bill.DeletedAt = DateTime.Now;
            bill.DeletedBy = "Current User";

            return (true, "Bill deleted successfully.");
        }

        #endregion

        #region Workflow Operations

        /// <summary>Submit bill for approval</summary>
        public (bool Success, string Message) Submit(Guid id, string userName)
        {
            var bill = _bills.FirstOrDefault(b => b.Id == id && !b.IsDeleted);
            if (bill == null)
            {
                return (false, "Bill not found.");
            }

            if (bill.BillStatus != VendorBillStatuses.Draft)
            {
                return (false, "Only draft bills can be submitted.");
            }

            // Validate bill has lines
            if (!bill.Lines.Any())
            {
                return (false, "Bill has no lines.");
            }

            // Validate totals
            if (bill.GrandTotalAmount <= 0)
            {
                return (false, "Bill total must be greater than zero.");
            }

            // Check for duplicate vendor invoice number
            if (VendorInvoiceNumberExists(bill.CompanyId, bill.VendorId.Value, bill.VendorInvoiceNumber, bill.Id))
            {
                return (false, "Duplicate vendor invoice number detected for this vendor.");
            }

            bill.BillStatus = VendorBillStatuses.Submitted;
            bill.SubmittedOn = DateTime.Now;
            bill.SubmittedByUserName = userName;
            bill.UpdatedAt = DateTime.Now;
            bill.UpdatedBy = userName;

            return (true, "Bill submitted for approval.");
        }

        /// <summary>Approve bill</summary>
        public (bool Success, string Message) Approve(Guid id, Guid userId, string userName)
        {
            var bill = _bills.FirstOrDefault(b => b.Id == id && !b.IsDeleted);
            if (bill == null)
            {
                return (false, "Bill not found.");
            }

            if (bill.BillStatus != VendorBillStatuses.Submitted)
            {
                return (false, "Only submitted bills can be approved.");
            }

            bill.BillStatus = VendorBillStatuses.Approved;
            bill.ApprovedOn = DateTime.Now;
            bill.ApprovedByUserId = userId;
            bill.ApprovedByUserName = userName;
            bill.UpdatedAt = DateTime.Now;
            bill.UpdatedBy = userName;

            return (true, "Bill approved successfully.");
        }

        /// <summary>Post bill (accounting moment)</summary>
        public (bool Success, string Message) Post(Guid id, Guid userId, string userName)
        {
            var bill = _bills.FirstOrDefault(b => b.Id == id && !b.IsDeleted);
            if (bill == null)
            {
                return (false, "Bill not found.");
            }

            if (bill.BillStatus != VendorBillStatuses.Draft && bill.BillStatus != VendorBillStatuses.Approved)
            {
                return (false, "Only draft or approved bills can be posted.");
            }

            // Validate bill has lines
            if (!bill.Lines.Any())
            {
                return (false, "Bill has no lines.");
            }

            // Validate totals
            if (bill.GrandTotalAmount <= 0)
            {
                return (false, "Bill total must be greater than zero.");
            }

            // Check for duplicate vendor invoice number (critical)
            if (VendorInvoiceNumberExists(bill.CompanyId, bill.VendorId.Value, bill.VendorInvoiceNumber, bill.Id))
            {
                return (false, "Duplicate vendor invoice number detected for this vendor.");
            }

            // Validate all lines have accounts
            foreach (var line in bill.Lines)
            {
                if (line.ExpenseOrAssetAccountId == Guid.Empty)
                {
                    return (false, $"Line {line.LineNumber}: Account is required.");
                }
                if (line.LineTotalAmount <= 0)
                {
                    return (false, $"Line {line.LineNumber}: Line amount must be greater than zero.");
                }
            }

            // Snapshot data at posting
            bill.PostingDate = DateTime.Today;
            bill.BillStatus = VendorBillStatuses.Posted;
            bill.PostedOn = DateTime.Now;
            bill.PostedByUserId = userId;
            bill.PostedByUserName = userName;
            bill.UpdatedAt = DateTime.Now;
            bill.UpdatedBy = userName;

            // Snapshot line accounts
            foreach (var line in bill.Lines)
            {
                line.ExpenseOrAssetAccountIdSnapshot = line.ExpenseOrAssetAccountId;
                line.ExpenseOrAssetAccountCodeSnapshot = line.ExpenseOrAssetAccountCode;
                line.ExpenseOrAssetAccountNameSnapshot = line.ExpenseOrAssetAccountName;
            }

            return (true, "Bill posted successfully. GL entries created.");
        }

        /// <summary>Cancel bill (pre-post only)</summary>
        public (bool Success, string Message) Cancel(Guid id, string reason, Guid userId, string userName)
        {
            var bill = _bills.FirstOrDefault(b => b.Id == id && !b.IsDeleted);
            if (bill == null)
            {
                return (false, "Bill not found.");
            }

            if (bill.BillStatus != VendorBillStatuses.Draft && bill.BillStatus != VendorBillStatuses.Submitted)
            {
                return (false, "Only draft or submitted bills can be cancelled.");
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return (false, "Cancellation reason is required.");
            }

            bill.BillStatus = VendorBillStatuses.Cancelled;
            bill.CancellationReason = reason;
            bill.UpdatedAt = DateTime.Now;
            bill.UpdatedBy = userName;

            return (true, "Bill cancelled successfully.");
        }

        /// <summary>Reject bill back to draft</summary>
        public (bool Success, string Message) Reject(Guid id, string reason, string userName)
        {
            var bill = _bills.FirstOrDefault(b => b.Id == id && !b.IsDeleted);
            if (bill == null)
            {
                return (false, "Bill not found.");
            }

            if (bill.BillStatus != VendorBillStatuses.Submitted)
            {
                return (false, "Only submitted bills can be rejected.");
            }

            bill.BillStatus = VendorBillStatuses.Draft;
            bill.RejectionReason = reason;
            bill.BillNarration = $"{bill.BillNarration}\n[Rejected: {reason}]";
            bill.UpdatedAt = DateTime.Now;
            bill.UpdatedBy = userName;

            return (true, "Bill rejected and returned to draft.");
        }

        #endregion

        #region Line Operations

        /// <summary>Add line to bill</summary>
        public (bool Success, string Message) AddLine(Guid billId, VendorBillLineViewModel line)
        {
            var bill = _bills.FirstOrDefault(b => b.Id == billId && !b.IsDeleted);
            if (bill == null)
            {
                return (false, "Bill not found.");
            }

            if (!bill.CanEdit)
            {
                return (false, "Lines cannot be added to posted or cancelled bills.");
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

            if (line.UnitRate < 0)
            {
                return (false, "Unit rate cannot be negative.");
            }

            if (line.ExpenseOrAssetAccountId == Guid.Empty)
            {
                return (false, "Account is required.");
            }

            // Set line number
            var maxLineNumber = bill.Lines.Any() ? bill.Lines.Max(l => l.LineNumber) : 0;
            line.LineNumber = maxLineNumber + 10;
            line.Id = Guid.NewGuid();
            line.VendorBillId = billId;
            line.CreatedAt = DateTime.Now;

            // Calculate line amounts
            line.RecalculateAmounts();

            bill.Lines.Add(line);
            bill.RecalculateTotals();
            bill.UpdatedAt = DateTime.Now;

            return (true, "Line added successfully.");
        }

        /// <summary>Update line in bill</summary>
        public (bool Success, string Message) UpdateLine(VendorBillLineViewModel line)
        {
            var bill = _bills.FirstOrDefault(b => b.Id == line.VendorBillId && !b.IsDeleted);
            if (bill == null)
            {
                return (false, "Bill not found.");
            }

            if (!bill.CanEdit)
            {
                return (false, "Lines cannot be updated on posted or cancelled bills.");
            }

            var existingLine = bill.Lines.FirstOrDefault(l => l.Id == line.Id);
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
            existingLine.UnitRate = line.UnitRate;
            existingLine.DiscountAmount = line.DiscountAmount;
            existingLine.TaxCodeId = line.TaxCodeId;
            existingLine.TaxCodeCode = line.TaxCodeCode;
            existingLine.TaxCodeName = line.TaxCodeName;
            existingLine.TaxRatePercentSnapshot = line.TaxRatePercentSnapshot;
            existingLine.TaxTypeSnapshot = line.TaxTypeSnapshot;
            existingLine.ExpenseOrAssetAccountId = line.ExpenseOrAssetAccountId;
            existingLine.ExpenseOrAssetAccountCode = line.ExpenseOrAssetAccountCode;
            existingLine.ExpenseOrAssetAccountName = line.ExpenseOrAssetAccountName;
            existingLine.HSNCode = line.HSNCode;
            existingLine.SACCode = line.SACCode;
            existingLine.ReferenceText = line.ReferenceText;
            existingLine.UpdatedAt = DateTime.Now;
            existingLine.UpdatedBy = line.UpdatedBy;

            // Recalculate line amounts
            existingLine.RecalculateAmounts();

            // Recalculate bill totals
            bill.RecalculateTotals();
            bill.UpdatedAt = DateTime.Now;

            return (true, "Line updated successfully.");
        }

        /// <summary>Delete line from bill</summary>
        public (bool Success, string Message) DeleteLine(Guid billId, Guid lineId)
        {
            var bill = _bills.FirstOrDefault(b => b.Id == billId && !b.IsDeleted);
            if (bill == null)
            {
                return (false, "Bill not found.");
            }

            if (!bill.CanEdit)
            {
                return (false, "Lines cannot be deleted from posted or cancelled bills.");
            }

            var line = bill.Lines.FirstOrDefault(l => l.Id == lineId);
            if (line == null)
            {
                return (false, "Line not found.");
            }

            bill.Lines.Remove(line);
            bill.RecalculateTotals();
            bill.UpdatedAt = DateTime.Now;

            return (true, "Line deleted successfully.");
        }

        /// <summary>Get lines for bill</summary>
        public List<VendorBillLineViewModel> GetLines(Guid billId)
        {
            var bill = _bills.FirstOrDefault(b => b.Id == billId && !b.IsDeleted);
            return bill?.Lines ?? new List<VendorBillLineViewModel>();
        }

        #endregion

        #region Statistics

        /// <summary>Get bill statistics</summary>
        public VendorBillStatisticsViewModel GetStatistics(Guid? companyId = null)
        {
            var bills = companyId.HasValue
                ? _bills.Where(b => b.CompanyId == companyId.Value && !b.IsDeleted)
                : _bills.Where(b => !b.IsDeleted);

            var today = DateTime.Today;

            return new VendorBillStatisticsViewModel
            {
                TotalBills = bills.Count(),
                DraftBills = bills.Count(b => b.BillStatus == VendorBillStatuses.Draft),
                SubmittedBills = bills.Count(b => b.BillStatus == VendorBillStatuses.Submitted),
                ApprovedBills = bills.Count(b => b.BillStatus == VendorBillStatuses.Approved),
                PostedBills = bills.Count(b => b.BillStatus == VendorBillStatuses.Posted),
                PaidBills = bills.Count(b => b.BillStatus == VendorBillStatuses.Posted && b.OutstandingAmount <= 0),
                CancelledBills = bills.Count(b => b.BillStatus == VendorBillStatuses.Cancelled),
                OverdueBills = bills.Count(b =>
                    b.DueDate < today &&
                    b.OutstandingAmount > 0 &&
                    b.BillStatus == VendorBillStatuses.Posted),
                TotalBillAmount = bills.Where(b => b.BillStatus != VendorBillStatuses.Cancelled).Sum(b => b.GrandTotalAmount),
                TotalOutstandingAmount = bills.Where(b => b.BillStatus == VendorBillStatuses.Posted).Sum(b => b.OutstandingAmount),
                TotalPaidAmount = bills.Where(b => b.BillStatus == VendorBillStatuses.Posted).Sum(b => b.PaidAmount)
            };
        }

        #endregion
    }

    /// <summary>
    /// VendorBill statistics model
    /// </summary>
    public class VendorBillStatisticsViewModel
    {
        public int TotalBills { get; set; }
        public int DraftBills { get; set; }
        public int SubmittedBills { get; set; }
        public int ApprovedBills { get; set; }
        public int PostedBills { get; set; }
        public int PaidBills { get; set; }
        public int CancelledBills { get; set; }
        public int OverdueBills { get; set; }
        public decimal TotalBillAmount { get; set; }
        public decimal TotalOutstandingAmount { get; set; }
        public decimal TotalPaidAmount { get; set; }
    }
}
