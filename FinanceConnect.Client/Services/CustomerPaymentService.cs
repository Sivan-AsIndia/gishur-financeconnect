using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    /// <summary>
    /// Service for CustomerPayment (Model #29) CRUD operations
    /// Demo application - data stored in memory
    /// </summary>
    public class CustomerPaymentService
    {
        // Immutable seed data
        private static readonly List<CustomerPaymentViewModel> _seedPayments = CustomerPaymentSeedData.GetSeedPayments();

        // Working (mutable) data
        private List<CustomerPaymentViewModel> _payments;

        // Counter for receipt number generation
        private int _receiptCounter = 100;

        public CustomerPaymentService()
        {
            ResetToSeed();
        }

        private static List<T> CloneList<T>(IEnumerable<T> source) where T : class
        {
            var json = System.Text.Json.JsonSerializer.Serialize(source);
            return System.Text.Json.JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }

        /// <summary>Reset payments to seed data</summary>
        public void ResetToSeed()
        {
            _payments = CloneList(_seedPayments);
        }

        #region Read Operations

        /// <summary>Get all payments</summary>
        public List<CustomerPaymentViewModel> GetAll()
        {
            return _payments.Where(p => !p.IsDeleted).ToList();
        }

        /// <summary>Get payment by ID</summary>
        public CustomerPaymentViewModel? GetById(Guid id)
        {
            return _payments.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
        }

        /// <summary>Get payments by company ID</summary>
        public List<CustomerPaymentViewModel> GetByCompanyId(Guid companyId)
        {
            return _payments.Where(p => p.CompanyId == companyId && !p.IsDeleted).ToList();
        }

        /// <summary>Get payments by customer ID</summary>
        public List<CustomerPaymentViewModel> GetByCustomerId(Guid customerId)
        {
            return _payments.Where(p => p.CustomerId == customerId && !p.IsDeleted).ToList();
        }

        /// <summary>Get payments by status</summary>
        public List<CustomerPaymentViewModel> GetByStatus(string status)
        {
            return _payments.Where(p => p.PaymentStatus == status && !p.IsDeleted).ToList();
        }

        /// <summary>Get payments by branch ID</summary>
        public List<CustomerPaymentViewModel> GetByBranchId(Guid branchId)
        {
            return _payments.Where(p => p.BranchId == branchId && !p.IsDeleted).ToList();
        }

        /// <summary>Get payments by payment method</summary>
        public List<CustomerPaymentViewModel> GetByPaymentMethod(string paymentMethod)
        {
            return _payments.Where(p => p.PaymentMethod == paymentMethod && !p.IsDeleted).ToList();
        }

        /// <summary>Get payments by date range</summary>
        public List<CustomerPaymentViewModel> GetByDateRange(DateTime fromDate, DateTime toDate)
        {
            return _payments.Where(p =>
                p.ReceiptDate >= fromDate &&
                p.ReceiptDate <= toDate &&
                !p.IsDeleted).ToList();
        }

        /// <summary>Search payments by receipt number, customer code/name, instrument number</summary>
        public List<CustomerPaymentViewModel> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAll();

            searchTerm = searchTerm.ToLower();
            return _payments.Where(p => !p.IsDeleted && (
                p.ReceiptNumber.ToLower().Contains(searchTerm) ||
                (p.CustomerCode?.ToLower().Contains(searchTerm) ?? false) ||
                (p.CustomerName?.ToLower().Contains(searchTerm) ?? false) ||
                (p.InstrumentNumber?.ToLower().Contains(searchTerm) ?? false) ||
                (p.BankName?.ToLower().Contains(searchTerm) ?? false)
            )).ToList();
        }

        /// <summary>Check if receipt number exists within company</summary>
        public bool ReceiptNumberExists(Guid companyId, string receiptNumber, Guid? excludeId = null)
        {
            return _payments.Any(p =>
                p.CompanyId == companyId &&
                p.ReceiptNumber.Equals(receiptNumber, StringComparison.OrdinalIgnoreCase) &&
                !p.IsDeleted &&
                (excludeId == null || p.Id != excludeId));
        }

        /// <summary>Check if instrument number exists (for duplicate prevention)</summary>
        public bool InstrumentNumberExists(Guid companyId, string instrumentNumber, string paymentMethod, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(instrumentNumber))
                return false;

            return _payments.Any(p =>
                p.CompanyId == companyId &&
                p.PaymentMethod == paymentMethod &&
                (p.InstrumentNumber?.Equals(instrumentNumber, StringComparison.OrdinalIgnoreCase) ?? false) &&
                !p.IsDeleted &&
                p.PaymentStatus != PaymentStatuses.Reversed &&
                p.PaymentStatus != PaymentStatuses.Cancelled &&
                (excludeId == null || p.Id != excludeId));
        }

        /// <summary>Generate next receipt number</summary>
        public string GenerateReceiptNumber(Guid companyId)
        {
            _receiptCounter++;
            var year = DateTime.Today.Year;
            return $"RCP-{year}-{_receiptCounter:D4}";
        }

        #endregion

        #region Write Operations

        /// <summary>Add new payment</summary>
        public (bool Success, string Message) Add(CustomerPaymentViewModel payment)
        {
            // Validate receipt number uniqueness
            if (ReceiptNumberExists(payment.CompanyId, payment.ReceiptNumber))
            {
                return (false, "Receipt number already exists.");
            }

            // Validate customer
            if (payment.CustomerId == Guid.Empty)
            {
                return (false, "Customer is required.");
            }

            // Validate branch
            if (payment.BranchId == Guid.Empty)
            {
                return (false, "Branch is required.");
            }

            // Validate payment method
            if (string.IsNullOrWhiteSpace(payment.PaymentMethod))
            {
                return (false, "Payment Method is required.");
            }

            // Validate payment account
            if (payment.PaymentAccountId == Guid.Empty)
            {
                return (false, "Payment Account (Deposit To) is required.");
            }

            // Validate amount
            if (payment.PaymentAmountTotal <= 0)
            {
                return (false, "Payment amount must be greater than zero.");
            }

            // Validate instrument details based on payment method
            var instrumentValidation = ValidateInstrumentDetails(payment);
            if (!instrumentValidation.Success)
            {
                return instrumentValidation;
            }

            // Validate allocations
            var allocationValidation = ValidateAllocations(payment);
            if (!allocationValidation.Success)
            {
                return allocationValidation;
            }

            // Check for duplicate instrument number
            if (!string.IsNullOrWhiteSpace(payment.InstrumentNumber) &&
                InstrumentNumberExists(payment.CompanyId, payment.InstrumentNumber, payment.PaymentMethod))
            {
                return (false, $"Instrument number '{payment.InstrumentNumber}' already exists for this payment method.");
            }

            payment.Id = Guid.NewGuid();
            payment.ReceiptNumber = payment.ReceiptNumber.ToUpper().Trim();
            payment.CreatedAt = DateTime.Now;
            payment.IsDeleted = false;

            // Generate receipt number if empty
            if (string.IsNullOrWhiteSpace(payment.ReceiptNumber))
            {
                payment.ReceiptNumber = GenerateReceiptNumber(payment.CompanyId);
            }

            // Set allocation payment IDs
            foreach (var allocation in payment.Allocations)
            {
                allocation.Id = Guid.NewGuid();
                allocation.CustomerPaymentId = payment.Id;
                allocation.CreatedAt = DateTime.Now;
                allocation.CreatedBy = payment.CreatedBy;
            }

            // Recalculate amounts
            payment.RecalculateAmounts();

            _payments.Add(payment);
            return (true, "Payment created successfully.");
        }

        /// <summary>Update existing payment</summary>
        public (bool Success, string Message) Update(CustomerPaymentViewModel payment)
        {
            var existing = _payments.FirstOrDefault(p => p.Id == payment.Id && !p.IsDeleted);
            if (existing == null)
            {
                return (false, "Payment not found.");
            }

            if (!existing.CanEdit)
            {
                return (false, "Payment cannot be edited in its current status.");
            }

            // Validate receipt number uniqueness
            if (ReceiptNumberExists(payment.CompanyId, payment.ReceiptNumber, payment.Id))
            {
                return (false, "Receipt number already exists.");
            }

            // Validate customer
            if (payment.CustomerId == Guid.Empty)
            {
                return (false, "Customer is required.");
            }

            // Validate branch
            if (payment.BranchId == Guid.Empty)
            {
                return (false, "Branch is required.");
            }

            // Validate payment method
            if (string.IsNullOrWhiteSpace(payment.PaymentMethod))
            {
                return (false, "Payment Method is required.");
            }

            // Validate payment account
            if (payment.PaymentAccountId == Guid.Empty)
            {
                return (false, "Payment Account (Deposit To) is required.");
            }

            // Validate amount
            if (payment.PaymentAmountTotal <= 0)
            {
                return (false, "Payment amount must be greater than zero.");
            }

            // Validate instrument details
            var instrumentValidation = ValidateInstrumentDetails(payment);
            if (!instrumentValidation.Success)
            {
                return instrumentValidation;
            }

            // Validate allocations
            var allocationValidation = ValidateAllocations(payment);
            if (!allocationValidation.Success)
            {
                return allocationValidation;
            }

            // Check for duplicate instrument number
            if (!string.IsNullOrWhiteSpace(payment.InstrumentNumber) &&
                InstrumentNumberExists(payment.CompanyId, payment.InstrumentNumber, payment.PaymentMethod, payment.Id))
            {
                return (false, $"Instrument number '{payment.InstrumentNumber}' already exists for this payment method.");
            }

            // Update fields
            existing.BranchId = payment.BranchId;
            existing.BranchCode = payment.BranchCode;
            existing.BranchName = payment.BranchName;
            existing.CustomerId = payment.CustomerId;
            existing.CustomerCode = payment.CustomerCode;
            existing.CustomerName = payment.CustomerName;
            existing.CustomerAccountId = payment.CustomerAccountId;
            existing.CustomerAccountName = payment.CustomerAccountName;
            existing.ReceiptNumber = payment.ReceiptNumber.ToUpper().Trim();
            existing.ReceiptDate = payment.ReceiptDate;
            existing.CurrencyId = payment.CurrencyId;
            existing.CurrencyCode = payment.CurrencyCode;
            existing.CurrencyName = payment.CurrencyName;
            existing.ExchangeRate = payment.ExchangeRate;
            existing.PaymentNarration = payment.PaymentNarration;
            existing.PaymentMethod = payment.PaymentMethod;
            existing.PaymentAccountId = payment.PaymentAccountId;
            existing.PaymentAccountCode = payment.PaymentAccountCode;
            existing.PaymentAccountName = payment.PaymentAccountName;
            existing.InstrumentDate = payment.InstrumentDate;
            existing.InstrumentNumber = payment.InstrumentNumber;
            existing.BankName = payment.BankName;
            existing.BankAccountLast4 = payment.BankAccountLast4;
            existing.PayerName = payment.PayerName;
            existing.GatewayProvider = payment.GatewayProvider;
            existing.GatewayTransactionId = payment.GatewayTransactionId;
            existing.PaymentAmountTotal = payment.PaymentAmountTotal;
            existing.AdvanceAmountTotal = payment.AdvanceAmountTotal;
            existing.Allocations = payment.Allocations;
            existing.UpdatedAt = DateTime.Now;
            existing.UpdatedBy = payment.UpdatedBy;

            // Set allocation payment IDs
            foreach (var allocation in existing.Allocations)
            {
                if (allocation.Id == Guid.Empty)
                {
                    allocation.Id = Guid.NewGuid();
                }
                allocation.CustomerPaymentId = existing.Id;
                if (allocation.CreatedAt == default)
                {
                    allocation.CreatedAt = DateTime.Now;
                    allocation.CreatedBy = payment.UpdatedBy;
                }
            }

            // Recalculate amounts
            existing.RecalculateAmounts();

            return (true, "Payment updated successfully.");
        }

        /// <summary>Delete payment (soft delete - Draft only)</summary>
        public (bool Success, string Message) Delete(Guid id, string? userName = null)
        {
            var payment = _payments.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
            if (payment == null)
            {
                return (false, "Payment not found.");
            }

            if (payment.PaymentStatus != PaymentStatuses.Draft)
            {
                return (false, "Only draft payments can be deleted.");
            }

            payment.IsDeleted = true;
            payment.DeletedAt = DateTime.Now;
            payment.DeletedBy = userName;

            return (true, "Payment deleted successfully.");
        }

        /// <summary>Submit payment for approval</summary>
        public (bool Success, string Message) Submit(Guid id, string? userName = null)
        {
            var payment = _payments.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
            if (payment == null)
            {
                return (false, "Payment not found.");
            }

            if (payment.PaymentStatus != PaymentStatuses.Draft)
            {
                return (false, "Only draft payments can be submitted.");
            }

            // Validate before submit
            if (payment.PaymentAmountTotal <= 0)
            {
                return (false, "Payment amount must be greater than zero.");
            }

            var instrumentValidation = ValidateInstrumentDetails(payment);
            if (!instrumentValidation.Success)
            {
                return instrumentValidation;
            }

            payment.PaymentStatus = PaymentStatuses.Submitted;
            payment.SubmittedOn = DateTime.Now;
            payment.SubmittedBy = userName ?? "System";
            payment.UpdatedAt = DateTime.Now;
            payment.UpdatedBy = userName;

            return (true, "Payment submitted for approval.");
        }

        /// <summary>Approve payment</summary>
        public (bool Success, string Message) Approve(Guid id, string? userName = null)
        {
            var payment = _payments.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
            if (payment == null)
            {
                return (false, "Payment not found.");
            }

            if (payment.PaymentStatus != PaymentStatuses.Submitted)
            {
                return (false, "Only submitted payments can be approved.");
            }

            payment.PaymentStatus = PaymentStatuses.Approved;
            payment.ApprovedOn = DateTime.Now;
            payment.ApprovedBy = userName ?? "System";
            payment.UpdatedAt = DateTime.Now;
            payment.UpdatedBy = userName;

            return (true, "Payment approved.");
        }

        /// <summary>Post payment (create GL entries)</summary>
        public (bool Success, string Message) Post(Guid id, string? userName = null)
        {
            var payment = _payments.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
            if (payment == null)
            {
                return (false, "Payment not found.");
            }

            if (!payment.CanPost)
            {
                return (false, "Payment cannot be posted in its current status.");
            }

            // Validate amount
            if (payment.PaymentAmountTotal <= 0)
            {
                return (false, "Payment amount must be greater than zero.");
            }

            // Validate instrument details
            var instrumentValidation = ValidateInstrumentDetails(payment);
            if (!instrumentValidation.Success)
            {
                return instrumentValidation;
            }

            // Validate allocations don't exceed total
            var allocationValidation = ValidateAllocations(payment);
            if (!allocationValidation.Success)
            {
                return allocationValidation;
            }

            // Validate advance account if unallocated/advance exists
            if ((payment.UnallocatedAmountTotal > 0 || payment.AdvanceAmountTotal > 0) &&
                payment.AdvanceFromCustomerAccountIdSnapshot == null)
            {
                return (false, "Advance account is not configured. Cannot post unallocated/advance amounts.");
            }

            payment.PaymentStatus = PaymentStatuses.Posted;
            payment.PostingDate = DateTime.Today;
            payment.PostedOn = DateTime.Now;
            payment.PostedByUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            payment.PostedBy = userName ?? "System";
            payment.UpdatedAt = DateTime.Now;
            payment.UpdatedBy = userName;

            return (true, "Payment posted successfully. GL entries created.");
        }

        /// <summary>Cancel payment (pre-post only)</summary>
        public (bool Success, string Message) Cancel(Guid id, string reason, string? userName = null)
        {
            var payment = _payments.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
            if (payment == null)
            {
                return (false, "Payment not found.");
            }

            if (!payment.CanCancel)
            {
                return (false, "Payment cannot be cancelled in its current status.");
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return (false, "Cancellation reason is required.");
            }

            payment.PaymentStatus = PaymentStatuses.Cancelled;
            payment.CancellationReason = reason;
            payment.CancelledOn = DateTime.Now;
            payment.CancelledByUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            payment.CancelledBy = userName ?? "System";
            payment.UpdatedAt = DateTime.Now;
            payment.UpdatedBy = userName;

            return (true, "Payment cancelled.");
        }

        /// <summary>Reverse payment (post-only - for cheque bounce, wrong posting)</summary>
        public (bool Success, string Message) Reverse(Guid id, string reason, string? referenceNo = null, string? userName = null)
        {
            var payment = _payments.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
            if (payment == null)
            {
                return (false, "Payment not found.");
            }

            if (!payment.CanReverse)
            {
                return (false, "Only posted payments can be reversed.");
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return (false, "Reversal reason is required.");
            }

            payment.PaymentStatus = PaymentStatuses.Reversed;
            payment.ReversalReason = reason;
            payment.ReversalReference = referenceNo;
            payment.ReversedOn = DateTime.Now;
            payment.ReversedByUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            payment.ReversedBy = userName ?? "System";
            payment.UpdatedAt = DateTime.Now;
            payment.UpdatedBy = userName;

            return (true, "Payment reversed. GL entries reversed and allocations rolled back.");
        }

        #endregion

        #region Validation Helpers

        private (bool Success, string Message) ValidateInstrumentDetails(CustomerPaymentViewModel payment)
        {
            // Validate instrument number based on payment method
            if (PaymentMethods.RequiresInstrumentNumber(payment.PaymentMethod) &&
                string.IsNullOrWhiteSpace(payment.InstrumentNumber))
            {
                var methodName = PaymentMethods.GetDisplayName(payment.PaymentMethod);
                return (false, $"Instrument/Reference number is required for {methodName}.");
            }

            // Validate bank name for cheque
            if (PaymentMethods.RequiresBankName(payment.PaymentMethod) &&
                string.IsNullOrWhiteSpace(payment.BankName))
            {
                return (false, "Bank name is required for cheque payments.");
            }

            // Validate instrument date for cheque/bank transfer
            if (PaymentMethods.RequiresInstrumentDate(payment.PaymentMethod) &&
                payment.InstrumentDate == null)
            {
                return (false, "Instrument date is required for this payment method.");
            }

            return (true, string.Empty);
        }

        private (bool Success, string Message) ValidateAllocations(CustomerPaymentViewModel payment)
        {
            // Validate total allocations don't exceed payment amount
            var totalAllocated = payment.Allocations.Sum(a => a.AllocatedAmount);
            if (totalAllocated > payment.PaymentAmountTotal)
            {
                return (false, "Total allocated amount exceeds payment amount.");
            }

            // Validate each allocation
            foreach (var allocation in payment.Allocations)
            {
                if (allocation.AllocatedAmount <= 0)
                {
                    return (false, "Each allocation amount must be greater than zero.");
                }

                if (allocation.AllocatedAmount > allocation.InvoiceOutstanding)
                {
                    return (false, $"Allocation for invoice {allocation.InvoiceNumber} exceeds outstanding amount.");
                }

                if (allocation.CustomerInvoiceId == Guid.Empty)
                {
                    return (false, "Invoice is required for each allocation.");
                }
            }

            return (true, string.Empty);
        }

        #endregion

        #region Allocation Operations

        /// <summary>Add allocation to payment</summary>
        public (bool Success, string Message) AddAllocation(Guid paymentId, CustomerPaymentAllocationViewModel allocation)
        {
            var payment = _payments.FirstOrDefault(p => p.Id == paymentId && !p.IsDeleted);
            if (payment == null)
            {
                return (false, "Payment not found.");
            }

            if (!payment.CanEdit)
            {
                return (false, "Allocations cannot be added to posted or cancelled payments.");
            }

            // Validate allocation
            if (allocation.CustomerInvoiceId == Guid.Empty)
            {
                return (false, "Invoice is required.");
            }

            if (allocation.AllocatedAmount <= 0)
            {
                return (false, "Allocated amount must be greater than zero.");
            }

            if (allocation.AllocatedAmount > allocation.InvoiceOutstanding)
            {
                return (false, "Allocated amount cannot exceed invoice outstanding.");
            }

            // Check if allocation would exceed payment total
            var currentTotal = payment.Allocations.Sum(a => a.AllocatedAmount);
            if (currentTotal + allocation.AllocatedAmount > payment.PaymentAmountTotal)
            {
                return (false, "Total allocations would exceed payment amount.");
            }

            // Check if invoice already allocated
            if (payment.Allocations.Any(a => a.CustomerInvoiceId == allocation.CustomerInvoiceId))
            {
                return (false, "This invoice is already allocated in this payment.");
            }

            allocation.Id = Guid.NewGuid();
            allocation.CustomerPaymentId = paymentId;
            allocation.CreatedAt = DateTime.Now;

            payment.Allocations.Add(allocation);
            payment.RecalculateAmounts();
            payment.UpdatedAt = DateTime.Now;

            return (true, "Allocation added successfully.");
        }

        /// <summary>Update allocation</summary>
        public (bool Success, string Message) UpdateAllocation(CustomerPaymentAllocationViewModel allocation)
        {
            var payment = _payments.FirstOrDefault(p => p.Id == allocation.CustomerPaymentId && !p.IsDeleted);
            if (payment == null)
            {
                return (false, "Payment not found.");
            }

            if (!payment.CanEdit)
            {
                return (false, "Allocations cannot be updated on posted or cancelled payments.");
            }

            var existingAllocation = payment.Allocations.FirstOrDefault(a => a.Id == allocation.Id);
            if (existingAllocation == null)
            {
                return (false, "Allocation not found.");
            }

            // Validate allocation
            if (allocation.AllocatedAmount <= 0)
            {
                return (false, "Allocated amount must be greater than zero.");
            }

            if (allocation.AllocatedAmount > allocation.InvoiceOutstanding)
            {
                return (false, "Allocated amount cannot exceed invoice outstanding.");
            }

            // Check if allocation would exceed payment total
            var otherAllocations = payment.Allocations.Where(a => a.Id != allocation.Id).Sum(a => a.AllocatedAmount);
            if (otherAllocations + allocation.AllocatedAmount > payment.PaymentAmountTotal)
            {
                return (false, "Total allocations would exceed payment amount.");
            }

            existingAllocation.AllocatedAmount = allocation.AllocatedAmount;
            existingAllocation.UpdatedAt = DateTime.Now;
            existingAllocation.UpdatedBy = allocation.UpdatedBy;

            payment.RecalculateAmounts();
            payment.UpdatedAt = DateTime.Now;

            return (true, "Allocation updated successfully.");
        }

        /// <summary>Delete allocation from payment</summary>
        public (bool Success, string Message) DeleteAllocation(Guid paymentId, Guid allocationId)
        {
            var payment = _payments.FirstOrDefault(p => p.Id == paymentId && !p.IsDeleted);
            if (payment == null)
            {
                return (false, "Payment not found.");
            }

            if (!payment.CanEdit)
            {
                return (false, "Allocations cannot be deleted from posted or cancelled payments.");
            }

            var allocation = payment.Allocations.FirstOrDefault(a => a.Id == allocationId);
            if (allocation == null)
            {
                return (false, "Allocation not found.");
            }

            payment.Allocations.Remove(allocation);
            payment.RecalculateAmounts();
            payment.UpdatedAt = DateTime.Now;

            return (true, "Allocation deleted successfully.");
        }

        /// <summary>Get allocations for payment</summary>
        public List<CustomerPaymentAllocationViewModel> GetAllocations(Guid paymentId)
        {
            var payment = _payments.FirstOrDefault(p => p.Id == paymentId && !p.IsDeleted);
            return payment?.Allocations ?? new List<CustomerPaymentAllocationViewModel>();
        }

        #endregion

        #region Statistics

        /// <summary>Get payment statistics</summary>
        public PaymentStatisticsViewModel GetStatistics(Guid? companyId = null)
        {
            var payments = companyId.HasValue
                ? _payments.Where(p => p.CompanyId == companyId.Value && !p.IsDeleted)
                : _payments.Where(p => !p.IsDeleted);

            return new PaymentStatisticsViewModel
            {
                TotalPayments = payments.Count(),
                DraftPayments = payments.Count(p => p.PaymentStatus == PaymentStatuses.Draft),
                SubmittedPayments = payments.Count(p => p.PaymentStatus == PaymentStatuses.Submitted),
                ApprovedPayments = payments.Count(p => p.PaymentStatus == PaymentStatuses.Approved),
                PostedPayments = payments.Count(p => p.PaymentStatus == PaymentStatuses.Posted),
                ReversedPayments = payments.Count(p => p.PaymentStatus == PaymentStatuses.Reversed),
                CancelledPayments = payments.Count(p => p.PaymentStatus == PaymentStatuses.Cancelled),
                TotalReceivedAmount = payments.Where(p => p.PaymentStatus == PaymentStatuses.Posted).Sum(p => p.PaymentAmountTotal),
                TotalAllocatedAmount = payments.Where(p => p.PaymentStatus == PaymentStatuses.Posted).Sum(p => p.AllocatedAmountTotal),
                TotalAdvanceAmount = payments.Where(p => p.PaymentStatus == PaymentStatuses.Posted).Sum(p => p.AdvanceAmountTotal)
            };
        }

        #endregion
    }

    /// <summary>
    /// Payment statistics model
    /// </summary>
    public class PaymentStatisticsViewModel
    {
        public int TotalPayments { get; set; }
        public int DraftPayments { get; set; }
        public int SubmittedPayments { get; set; }
        public int ApprovedPayments { get; set; }
        public int PostedPayments { get; set; }
        public int ReversedPayments { get; set; }
        public int CancelledPayments { get; set; }
        public decimal TotalReceivedAmount { get; set; }
        public decimal TotalAllocatedAmount { get; set; }
        public decimal TotalAdvanceAmount { get; set; }
    }
}
