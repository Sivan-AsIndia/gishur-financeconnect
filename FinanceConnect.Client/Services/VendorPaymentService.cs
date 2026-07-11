using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    /// <summary>
    /// Service for VendorPayment (Model #38) CRUD and workflow operations
    /// Demo application - data stored in memory
    /// </summary>
    public class VendorPaymentService
    {
        // Immutable seed data
        private static readonly List<VendorPaymentViewModel> _seedPayments = VendorPaymentSeedData.GetSeedPayments();

        // Working (mutable) data
        private List<VendorPaymentViewModel> _payments;

        // Counter for payment number generation
        private int _paymentCounter = 100;

        public VendorPaymentService()
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
        public List<VendorPaymentViewModel> GetAll()
        {
            return _payments.Where(p => !p.IsDeleted).ToList();
        }

        /// <summary>Get payment by ID</summary>
        public VendorPaymentViewModel? GetById(Guid id)
        {
            return _payments.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
        }

        /// <summary>Get payments by company ID</summary>
        public List<VendorPaymentViewModel> GetByCompanyId(Guid companyId)
        {
            return _payments.Where(p => p.CompanyId == companyId && !p.IsDeleted).ToList();
        }

        /// <summary>Get payments by vendor ID</summary>
        public List<VendorPaymentViewModel> GetByVendorId(Guid vendorId)
        {
            return _payments.Where(p => p.VendorId == vendorId && !p.IsDeleted).ToList();
        }

        /// <summary>Get payments by status</summary>
        public List<VendorPaymentViewModel> GetByStatus(string status)
        {
            return _payments.Where(p => p.PaymentStatus == status && !p.IsDeleted).ToList();
        }

        /// <summary>Get payments by branch ID</summary>
        public List<VendorPaymentViewModel> GetByBranchId(Guid branchId)
        {
            return _payments.Where(p => p.BranchId == branchId && !p.IsDeleted).ToList();
        }

        /// <summary>Get payments by payment method</summary>
        public List<VendorPaymentViewModel> GetByPaymentMethod(string paymentMethod)
        {
            return _payments.Where(p => p.PaymentMethod == paymentMethod && !p.IsDeleted).ToList();
        }

        /// <summary>Get payments by date range</summary>
        public List<VendorPaymentViewModel> GetByDateRange(DateTime fromDate, DateTime toDate)
        {
            return _payments.Where(p =>
                p.PaymentDate >= fromDate &&
                p.PaymentDate <= toDate &&
                !p.IsDeleted).ToList();
        }

        /// <summary>Search payments by payment number, vendor code/name, reference number</summary>
        public List<VendorPaymentViewModel> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAll();

            searchTerm = searchTerm.ToLower();
            return _payments.Where(p => !p.IsDeleted && (
                p.PaymentNumber.ToLower().Contains(searchTerm) ||
                (p.VendorCode?.ToLower().Contains(searchTerm) ?? false) ||
                (p.VendorName?.ToLower().Contains(searchTerm) ?? false) ||
                (p.PaymentReferenceNumber?.ToLower().Contains(searchTerm) ?? false) ||
                (p.BankNameSnapshot?.ToLower().Contains(searchTerm) ?? false)
            )).ToList();
        }

        /// <summary>Check if payment number exists within company</summary>
        public bool PaymentNumberExists(Guid companyId, string paymentNumber, Guid? excludeId = null)
        {
            return _payments.Any(p =>
                p.CompanyId == companyId &&
                p.PaymentNumber.Equals(paymentNumber, StringComparison.OrdinalIgnoreCase) &&
                !p.IsDeleted &&
                (excludeId == null || p.Id != excludeId));
        }

        /// <summary>Check if instrument reference number exists (for duplicate prevention)</summary>
        public bool InstrumentNumberExists(Guid companyId, Guid paymentAccountId, string referenceNumber, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(referenceNumber))
                return false;

            return _payments.Any(p =>
                p.CompanyId == companyId &&
                p.PaymentAccountId == paymentAccountId &&
                (p.PaymentReferenceNumber?.Equals(referenceNumber, StringComparison.OrdinalIgnoreCase) ?? false) &&
                !p.IsDeleted &&
                p.PaymentStatus != VendorPaymentStatuses.Reversed &&
                p.PaymentStatus != VendorPaymentStatuses.Cancelled &&
                (excludeId == null || p.Id != excludeId));
        }

        /// <summary>Generate next payment number</summary>
        public string GeneratePaymentNumber(Guid companyId)
        {
            _paymentCounter++;
            var year = DateTime.Today.Year;
            return $"APP-{year}-{_paymentCounter:D4}";
        }

        #endregion

        #region Write Operations

        /// <summary>Add new payment</summary>
        public (bool Success, string Message) Add(VendorPaymentViewModel payment)
        {
            // Validate payment number uniqueness
            if (PaymentNumberExists(payment.CompanyId, payment.PaymentNumber))
            {
                return (false, "Payment number already exists.");
            }

            // Validate vendor
            if (payment.VendorId == Guid.Empty)
            {
                return (false, "Vendor is required.");
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
                return (false, "Payment Account (Paying Account) is required.");
            }

            // Validate amount
            if (payment.PaymentGrossAmount <= 0)
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

            // Check for duplicate instrument reference
            if (!string.IsNullOrWhiteSpace(payment.PaymentReferenceNumber) &&
                InstrumentNumberExists(payment.CompanyId, payment.PaymentAccountId, payment.PaymentReferenceNumber))
            {
                return (false, $"Payment reference '{payment.PaymentReferenceNumber}' already exists for this bank account.");
            }

            payment.Id = Guid.NewGuid();
            payment.PaymentNumber = payment.PaymentNumber.ToUpper().Trim();
            payment.PaymentStatus = VendorPaymentStatuses.Draft;
            payment.CreatedAt = DateTime.Now;
            payment.RecalculateAmounts();

            _payments.Add(payment);
            return (true, "Payment created successfully.");
        }

        /// <summary>Update existing payment</summary>
        public (bool Success, string Message) Update(VendorPaymentViewModel payment)
        {
            var existing = _payments.FirstOrDefault(p => p.Id == payment.Id && !p.IsDeleted);
            if (existing == null)
            {
                return (false, "Payment not found.");
            }

            if (!existing.CanEdit)
            {
                return (false, "Only draft payments can be edited.");
            }

            // Validate payment number uniqueness (excluding current)
            if (PaymentNumberExists(payment.CompanyId, payment.PaymentNumber, payment.Id))
            {
                return (false, "Payment number already exists.");
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

            // Check for duplicate instrument reference
            if (!string.IsNullOrWhiteSpace(payment.PaymentReferenceNumber) &&
                InstrumentNumberExists(payment.CompanyId, payment.PaymentAccountId, payment.PaymentReferenceNumber, payment.Id))
            {
                return (false, $"Payment reference '{payment.PaymentReferenceNumber}' already exists for this bank account.");
            }

            // Update fields
            existing.BranchId = payment.BranchId;
            existing.BranchCode = payment.BranchCode;
            existing.BranchName = payment.BranchName;
            existing.VendorId = payment.VendorId;
            existing.VendorCode = payment.VendorCode;
            existing.VendorName = payment.VendorName;
            existing.PaymentDate = payment.PaymentDate;
            existing.CurrencyId = payment.CurrencyId;
            existing.CurrencyCode = payment.CurrencyCode;
            existing.CurrencyName = payment.CurrencyName;
            existing.ExchangeRate = payment.ExchangeRate;
            existing.PaymentNarration = payment.PaymentNarration;
            existing.PaymentMethod = payment.PaymentMethod;
            existing.PaymentAccountId = payment.PaymentAccountId;
            existing.PaymentAccountCode = payment.PaymentAccountCode;
            existing.PaymentAccountName = payment.PaymentAccountName;
            existing.PaymentReferenceNumber = payment.PaymentReferenceNumber;
            existing.ReferenceDate = payment.ReferenceDate;
            existing.BankNameSnapshot = payment.BankNameSnapshot;
            existing.PaymentGrossAmount = payment.PaymentGrossAmount;
            existing.IsTDSApplicable = payment.IsTDSApplicable;
            existing.TDSSectionCodeSnapshot = payment.TDSSectionCodeSnapshot;
            existing.TDSRatePercentSnapshot = payment.TDSRatePercentSnapshot;
            existing.BankChargesAmount = payment.BankChargesAmount;
            existing.Allocations = payment.Allocations;
            existing.UpdatedAt = DateTime.Now;
            existing.UpdatedBy = payment.UpdatedBy;

            existing.RecalculateAmounts();

            return (true, "Payment updated successfully.");
        }

        /// <summary>Delete payment (soft delete)</summary>
        public (bool Success, string Message) Delete(Guid id, string deletedBy)
        {
            var payment = _payments.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
            if (payment == null)
            {
                return (false, "Payment not found.");
            }

            if (payment.PaymentStatus != VendorPaymentStatuses.Draft)
            {
                return (false, "Only draft payments can be deleted.");
            }

            payment.IsDeleted = true;
            payment.DeletedAt = DateTime.Now;
            payment.DeletedBy = deletedBy;

            return (true, "Payment deleted successfully.");
        }

        #endregion

        #region Workflow Operations

        /// <summary>Submit payment for approval</summary>
        public (bool Success, string Message) Submit(Guid id, string submittedBy)
        {
            var payment = _payments.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
            if (payment == null)
            {
                return (false, "Payment not found.");
            }

            if (!payment.CanSubmit)
            {
                return (false, "Only draft payments can be submitted.");
            }

            // Validate before submission
            var instrumentValidation = ValidateInstrumentDetails(payment);
            if (!instrumentValidation.Success)
            {
                return instrumentValidation;
            }

            payment.PaymentStatus = VendorPaymentStatuses.Submitted;
            payment.SubmittedOn = DateTime.Now;
            payment.SubmittedBy = submittedBy;
            payment.UpdatedAt = DateTime.Now;
            payment.UpdatedBy = submittedBy;

            return (true, "Payment submitted for approval.");
        }

        /// <summary>Approve payment</summary>
        public (bool Success, string Message) Approve(Guid id, string approvedBy)
        {
            var payment = _payments.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
            if (payment == null)
            {
                return (false, "Payment not found.");
            }

            if (!payment.CanApprove)
            {
                return (false, "Only submitted payments can be approved.");
            }

            payment.PaymentStatus = VendorPaymentStatuses.Approved;
            payment.ApprovedOn = DateTime.Now;
            payment.ApprovedBy = approvedBy;
            payment.UpdatedAt = DateTime.Now;
            payment.UpdatedBy = approvedBy;

            return (true, "Payment approved successfully.");
        }

        /// <summary>Reject payment</summary>
        public (bool Success, string Message) Reject(Guid id, string reason, string rejectedBy)
        {
            var payment = _payments.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
            if (payment == null)
            {
                return (false, "Payment not found.");
            }

            if (!payment.CanReject)
            {
                return (false, "Only submitted payments can be rejected.");
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return (false, "Rejection reason is required.");
            }

            payment.PaymentStatus = VendorPaymentStatuses.Rejected;
            payment.RejectionReason = reason;
            payment.UpdatedAt = DateTime.Now;
            payment.UpdatedBy = rejectedBy;

            return (true, "Payment rejected.");
        }

        /// <summary>Post payment</summary>
        public (bool Success, string Message) Post(Guid id, string postedBy)
        {
            var payment = _payments.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
            if (payment == null)
            {
                return (false, "Payment not found.");
            }

            if (!payment.CanPost)
            {
                return (false, "Only draft or approved payments can be posted.");
            }

            // Validate before posting
            var instrumentValidation = ValidateInstrumentDetails(payment);
            if (!instrumentValidation.Success)
            {
                return instrumentValidation;
            }

            // Validate TDS if applicable
            if (payment.IsTDSApplicable)
            {
                if (payment.TDSWithheldAmount < 0 || payment.TDSWithheldAmount > payment.PaymentGrossAmount)
                {
                    return (false, "TDS withheld amount is invalid.");
                }
            }

            payment.PaymentStatus = VendorPaymentStatuses.Posted;
            payment.PostingDate = DateTime.Today;
            payment.PostedOn = DateTime.Now;
            payment.PostedBy = postedBy;
            payment.InstrumentStatus = VendorInstrumentStatuses.Completed;
            payment.UpdatedAt = DateTime.Now;
            payment.UpdatedBy = postedBy;

            return (true, "Payment posted successfully.");
        }

        /// <summary>Cancel payment</summary>
        public (bool Success, string Message) Cancel(Guid id, string reason, string cancelledBy)
        {
            var payment = _payments.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
            if (payment == null)
            {
                return (false, "Payment not found.");
            }

            if (!payment.CanCancel)
            {
                return (false, "Only draft or submitted payments can be cancelled.");
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return (false, "Cancellation reason is required.");
            }

            payment.PaymentStatus = VendorPaymentStatuses.Cancelled;
            payment.CancellationReason = reason;
            payment.CancelledOn = DateTime.Now;
            payment.CancelledBy = cancelledBy;
            payment.UpdatedAt = DateTime.Now;
            payment.UpdatedBy = cancelledBy;

            return (true, "Payment cancelled successfully.");
        }

        /// <summary>Reverse posted payment</summary>
        public (bool Success, string Message) Reverse(Guid id, string reason, string? reference, string reversedBy)
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

            payment.PaymentStatus = VendorPaymentStatuses.Reversed;
            payment.ReversalReason = reason;
            payment.ReversalReference = reference;
            payment.ReversedOn = DateTime.Now;
            payment.ReversedBy = reversedBy;
            payment.InstrumentStatus = VendorInstrumentStatuses.Reversed;
            payment.UpdatedAt = DateTime.Now;
            payment.UpdatedBy = reversedBy;

            return (true, "Payment reversed successfully.");
        }

        #endregion

        #region Validation Helpers

        /// <summary>Validate instrument details based on payment method</summary>
        private (bool Success, string Message) ValidateInstrumentDetails(VendorPaymentViewModel payment)
        {
            // Check if reference is required for this payment method
            if (VendorPaymentMethods.RequiresReference(payment.PaymentMethod))
            {
                if (string.IsNullOrWhiteSpace(payment.PaymentReferenceNumber))
                {
                    return (false, $"Payment reference (UTR/Cheque No/Transaction ID) is required for {VendorPaymentMethods.GetDisplayName(payment.PaymentMethod)}.");
                }
            }

            // Cheque requires reference date
            if (payment.PaymentMethod == VendorPaymentMethods.Cheque)
            {
                if (!payment.ReferenceDate.HasValue)
                {
                    return (false, "Cheque date is required for cheque payments.");
                }
            }

            return (true, string.Empty);
        }

        /// <summary>Validate payment allocations</summary>
        private (bool Success, string Message) ValidateAllocations(VendorPaymentViewModel payment)
        {
            if (payment.Allocations == null || !payment.Allocations.Any())
            {
                // Advance payment (no allocations) is valid
                return (true, string.Empty);
            }

            decimal totalAllocated = 0;
            foreach (var allocation in payment.Allocations)
            {
                if (allocation.AllocatedToBillAmount <= 0)
                {
                    return (false, "Each allocation amount must be greater than zero.");
                }

                if (allocation.AllocatedToBillAmount > allocation.BillOutstandingSnapshot)
                {
                    return (false, $"Allocation for bill {allocation.BillNumberSnapshot} exceeds outstanding amount.");
                }

                if (allocation.VendorBillId == Guid.Empty)
                {
                    return (false, "Bill is required for each allocation.");
                }

                totalAllocated += allocation.AllocatedToBillAmount;
            }

            if (totalAllocated > payment.PaymentGrossAmount)
            {
                return (false, "Total allocations exceed payment amount.");
            }

            return (true, string.Empty);
        }

        #endregion

        #region Allocation Operations

        /// <summary>Add allocation to payment</summary>
        public (bool Success, string Message) AddAllocation(Guid paymentId, VendorPaymentAllocationModel allocation)
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
            if (allocation.VendorBillId == Guid.Empty)
            {
                return (false, "Bill is required.");
            }

            if (allocation.AllocatedToBillAmount <= 0)
            {
                return (false, "Allocated amount must be greater than zero.");
            }

            if (allocation.AllocatedToBillAmount > allocation.BillOutstandingSnapshot)
            {
                return (false, "Allocated amount cannot exceed bill outstanding.");
            }

            // Check if allocation would exceed payment total
            var currentTotal = payment.Allocations.Sum(a => a.AllocatedToBillAmount);
            if (currentTotal + allocation.AllocatedToBillAmount > payment.PaymentGrossAmount)
            {
                return (false, "Total allocations would exceed payment amount.");
            }

            // Check if bill already allocated
            if (payment.Allocations.Any(a => a.VendorBillId == allocation.VendorBillId))
            {
                return (false, "This bill is already allocated in this payment.");
            }

            allocation.Id = Guid.NewGuid();
            allocation.VendorPaymentId = paymentId;
            allocation.CreatedAt = DateTime.Now;
            allocation.AllocationOrder = payment.Allocations.Count + 1;

            payment.Allocations.Add(allocation);
            payment.RecalculateAmounts();
            payment.UpdatedAt = DateTime.Now;

            return (true, "Allocation added successfully.");
        }

        /// <summary>Update allocation</summary>
        public (bool Success, string Message) UpdateAllocation(VendorPaymentAllocationModel allocation)
        {
            var payment = _payments.FirstOrDefault(p => p.Id == allocation.VendorPaymentId && !p.IsDeleted);
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
            if (allocation.AllocatedToBillAmount <= 0)
            {
                return (false, "Allocated amount must be greater than zero.");
            }

            if (allocation.AllocatedToBillAmount > allocation.BillOutstandingSnapshot)
            {
                return (false, "Allocated amount cannot exceed bill outstanding.");
            }

            // Check if allocation would exceed payment total
            var otherAllocations = payment.Allocations.Where(a => a.Id != allocation.Id).Sum(a => a.AllocatedToBillAmount);
            if (otherAllocations + allocation.AllocatedToBillAmount > payment.PaymentGrossAmount)
            {
                return (false, "Total allocations would exceed payment amount.");
            }

            existingAllocation.AllocatedToBillAmount = allocation.AllocatedToBillAmount;
            existingAllocation.AllocationNarration = allocation.AllocationNarration;
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
        public List<VendorPaymentAllocationModel> GetAllocations(Guid paymentId)
        {
            var payment = _payments.FirstOrDefault(p => p.Id == paymentId && !p.IsDeleted);
            return payment?.Allocations ?? new List<VendorPaymentAllocationModel>();
        }

        #endregion

        #region Statistics

        /// <summary>Get payment statistics</summary>
        public VendorPaymentStatisticsViewModel GetStatistics(Guid? companyId = null)
        {
            var payments = companyId.HasValue
                ? _payments.Where(p => p.CompanyId == companyId.Value && !p.IsDeleted)
                : _payments.Where(p => !p.IsDeleted);

            return new VendorPaymentStatisticsViewModel
            {
                TotalPayments = payments.Count(),
                DraftPayments = payments.Count(p => p.PaymentStatus == VendorPaymentStatuses.Draft),
                SubmittedPayments = payments.Count(p => p.PaymentStatus == VendorPaymentStatuses.Submitted),
                ApprovedPayments = payments.Count(p => p.PaymentStatus == VendorPaymentStatuses.Approved),
                PostedPayments = payments.Count(p => p.PaymentStatus == VendorPaymentStatuses.Posted),
                ReversedPayments = payments.Count(p => p.PaymentStatus == VendorPaymentStatuses.Reversed),
                CancelledPayments = payments.Count(p => p.PaymentStatus == VendorPaymentStatuses.Cancelled),
                TotalPaidAmount = payments.Where(p => p.PaymentStatus == VendorPaymentStatuses.Posted).Sum(p => p.PaymentGrossAmount),
                TotalAllocatedAmount = payments.Where(p => p.PaymentStatus == VendorPaymentStatuses.Posted).Sum(p => p.AllocatedAmount),
                TotalAdvanceAmount = payments.Where(p => p.PaymentStatus == VendorPaymentStatuses.Posted).Sum(p => p.UnallocatedAdvanceAmount),
                TotalTDSWithheldAmount = payments.Where(p => p.PaymentStatus == VendorPaymentStatuses.Posted).Sum(p => p.TDSWithheldAmount)
            };
        }

        #endregion
    }

    /// <summary>
    /// Vendor Payment statistics model
    /// </summary>
    public class VendorPaymentStatisticsViewModel
    {
        public int TotalPayments { get; set; }
        public int DraftPayments { get; set; }
        public int SubmittedPayments { get; set; }
        public int ApprovedPayments { get; set; }
        public int PostedPayments { get; set; }
        public int ReversedPayments { get; set; }
        public int CancelledPayments { get; set; }
        public decimal TotalPaidAmount { get; set; }
        public decimal TotalAllocatedAmount { get; set; }
        public decimal TotalAdvanceAmount { get; set; }
        public decimal TotalTDSWithheldAmount { get; set; }
    }
}
