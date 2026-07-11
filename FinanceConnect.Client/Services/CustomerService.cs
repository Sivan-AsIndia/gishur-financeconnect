using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    /// <summary>
    /// Service for Customer CRUD operations
    /// Demo application - data stored in memory
    /// </summary>
    public class CustomerService
    {
        // Immutable seed data
        private static readonly List<CustomerViewModel> _seedCustomers = CustomerSeedData.GetSeedCustomers();

        // Working (mutable) data
        private List<CustomerViewModel> _customers;

        public CustomerService()
        {
            ResetToSeed();
        }

        private static List<T> CloneList<T>(IEnumerable<T> source) where T : class
        {
            var json = System.Text.Json.JsonSerializer.Serialize(source);
            return System.Text.Json.JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }

        /// <summary>Reset customers to seed data</summary>
        public void ResetToSeed()
        {
            _customers = CloneList(_seedCustomers);
        }

        #region Read Operations

        /// <summary>Get all customers</summary>
        public List<CustomerViewModel> GetAll()
        {
            return _customers.Where(c => !c.IsDeleted).ToList();
        }

        /// <summary>Get customer by ID</summary>
        public CustomerViewModel? GetById(Guid id)
        {
            return _customers.FirstOrDefault(c => c.Id == id && !c.IsDeleted);
        }

        /// <summary>Get customers by company ID</summary>
        public List<CustomerViewModel> GetByCompanyId(Guid companyId)
        {
            return _customers.Where(c => c.CompanyId == companyId && !c.IsDeleted).ToList();
        }

        /// <summary>Get customers by status</summary>
        public List<CustomerViewModel> GetByStatus(string status)
        {
            return _customers.Where(c => c.CustomerStatus == status && !c.IsDeleted).ToList();
        }

        /// <summary>Get customers on credit hold</summary>
        public List<CustomerViewModel> GetOnCreditHold()
        {
            return _customers.Where(c => c.CreditHoldStatus != CreditHoldStatuses.None && !c.IsDeleted).ToList();
        }

        /// <summary>Search customers by code, name, GSTIN, phone, or email</summary>
        public List<CustomerViewModel> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAll();

            searchTerm = searchTerm.ToLower();
            return _customers.Where(c => !c.IsDeleted && (
                c.CustomerCode.ToLower().Contains(searchTerm) ||
                c.CustomerName.ToLower().Contains(searchTerm) ||
                (c.GSTIN?.ToLower().Contains(searchTerm) ?? false) ||
                (c.PrimaryPhone?.ToLower().Contains(searchTerm) ?? false) ||
                (c.PrimaryEmail?.ToLower().Contains(searchTerm) ?? false)
            )).ToList();
        }

        /// <summary>Check if customer code exists within company</summary>
        public bool CustomerCodeExists(Guid companyId, string customerCode, Guid? excludeId = null)
        {
            return _customers.Any(c =>
                c.CompanyId == companyId &&
                c.CustomerCode.Equals(customerCode, StringComparison.OrdinalIgnoreCase) &&
                !c.IsDeleted &&
                (excludeId == null || c.Id != excludeId));
        }

        /// <summary>Check if GSTIN exists within company</summary>
        public bool GSTINExists(Guid companyId, string gstin, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(gstin))
                return false;

            return _customers.Any(c =>
                c.CompanyId == companyId &&
                c.GSTIN != null &&
                c.GSTIN.Equals(gstin, StringComparison.OrdinalIgnoreCase) &&
                !c.IsDeleted &&
                (excludeId == null || c.Id != excludeId));
        }

        #endregion

        #region Write Operations

        /// <summary>Add new customer</summary>
        public (bool Success, string Message) Add(CustomerViewModel customer)
        {
            // Validate customer code uniqueness
            if (CustomerCodeExists(customer.CompanyId, customer.CustomerCode))
            {
                return (false, "Customer code already exists.");
            }

            // Validate GSTIN uniqueness if provided
            if (!string.IsNullOrWhiteSpace(customer.GSTIN) && GSTINExists(customer.CompanyId, customer.GSTIN))
            {
                return (false, "GSTIN already exists for another customer.");
            }

            // Validate GSTIN requirement based on TaxRegistrationType
            if (IsGSTINRequired(customer.TaxRegistrationType) && string.IsNullOrWhiteSpace(customer.GSTIN))
            {
                return (false, "GSTIN is mandatory for registered customers.");
            }

            // Validate Credit Hold Reason
            if (customer.CreditHoldStatus != CreditHoldStatuses.None && string.IsNullOrWhiteSpace(customer.CreditHoldReason))
            {
                return (false, "Credit Hold Reason is required when customer is on hold.");
            }

            customer.Id = Guid.NewGuid();
            customer.CustomerCode = customer.CustomerCode.ToUpper().Trim();
            customer.CustomerName = NormalizeSpaces(customer.CustomerName.Trim());
            customer.CreatedAt = DateTime.Now;
            customer.IsDeleted = false;

            _customers.Add(customer);
            return (true, "Customer added successfully.");
        }

        /// <summary>Update existing customer</summary>
        public (bool Success, string Message) Update(CustomerViewModel customer)
        {
            var existing = _customers.FirstOrDefault(c => c.Id == customer.Id && !c.IsDeleted);
            if (existing == null)
            {
                return (false, "Customer not found.");
            }

            // Validate customer code uniqueness (excluding current record)
            if (CustomerCodeExists(customer.CompanyId, customer.CustomerCode, customer.Id))
            {
                return (false, "Customer code already exists.");
            }

            // Validate GSTIN uniqueness if provided
            if (!string.IsNullOrWhiteSpace(customer.GSTIN) && GSTINExists(customer.CompanyId, customer.GSTIN, customer.Id))
            {
                return (false, "GSTIN already exists for another customer.");
            }

            // Validate GSTIN requirement
            if (IsGSTINRequired(customer.TaxRegistrationType) && string.IsNullOrWhiteSpace(customer.GSTIN))
            {
                return (false, "GSTIN is mandatory for registered customers.");
            }

            // Validate Credit Hold Reason
            if (customer.CreditHoldStatus != CreditHoldStatuses.None && string.IsNullOrWhiteSpace(customer.CreditHoldReason))
            {
                return (false, "Credit Hold Reason is required when customer is on hold.");
            }

            // Update fields
            existing.CustomerCode = customer.CustomerCode.ToUpper().Trim();
            existing.CustomerName = NormalizeSpaces(customer.CustomerName.Trim());
            existing.CustomerType = customer.CustomerType;
            existing.CustomerStatus = customer.CustomerStatus;
            existing.PrimaryEmail = customer.PrimaryEmail;
            existing.PrimaryPhone = customer.PrimaryPhone;
            existing.Website = customer.Website;
            existing.DefaultBranchId = customer.DefaultBranchId;
            existing.DefaultBranchName = customer.DefaultBranchName;
            existing.TaxRegistrationType = customer.TaxRegistrationType;
            existing.GSTIN = customer.GSTIN;
            existing.PAN = customer.PAN;
            existing.TaxProfileId = customer.TaxProfileId;
            existing.TaxProfileName = customer.TaxProfileName;
            existing.IsTDSApplicable = customer.IsTDSApplicable;
            existing.TDSSectionCode = customer.TDSSectionCode;
            existing.CreditLimitAmount = customer.CreditLimitAmount;
            existing.CreditLimitEnforced = customer.CreditLimitEnforced;
            existing.CreditHoldStatus = customer.CreditHoldStatus;
            existing.CreditHoldReason = customer.CreditHoldReason;
            existing.CreditHoldPlacedOn = customer.CreditHoldPlacedOn;
            existing.CreditHoldPlacedByUserId = customer.CreditHoldPlacedByUserId;
            existing.CreditHoldPlacedByUserName = customer.CreditHoldPlacedByUserName;
            existing.PaymentTermId = customer.PaymentTermId;
            existing.PaymentTermName = customer.PaymentTermName;
            existing.DefaultCurrencyId = customer.DefaultCurrencyId;
            existing.DefaultCurrencyCode = customer.DefaultCurrencyCode;
            existing.DefaultCurrencyName = customer.DefaultCurrencyName;
            existing.DefaultPaymentMethod = customer.DefaultPaymentMethod;
            existing.ReceivableAccountId = customer.ReceivableAccountId;
            existing.ReceivableAccountCode = customer.ReceivableAccountCode;
            existing.ReceivableAccountName = customer.ReceivableAccountName;
            existing.AdvanceFromCustomerAccountId = customer.AdvanceFromCustomerAccountId;
            existing.AdvanceFromCustomerAccountCode = customer.AdvanceFromCustomerAccountCode;
            existing.AdvanceFromCustomerAccountName = customer.AdvanceFromCustomerAccountName;
            existing.WriteOffAccountId = customer.WriteOffAccountId;
            existing.WriteOffAccountCode = customer.WriteOffAccountCode;
            existing.WriteOffAccountName = customer.WriteOffAccountName;
            existing.AllowAutoAdvanceCreation = customer.AllowAutoAdvanceCreation;
            existing.SendInvoiceEmail = customer.SendInvoiceEmail;
            existing.CustomerStatementCycle = customer.CustomerStatementCycle;
            existing.PreferredLanguage = customer.PreferredLanguage;
            existing.AllowPartialPayment = customer.AllowPartialPayment;
            existing.AllowOverPayment = customer.AllowOverPayment;
            existing.UpdatedAt = DateTime.Now;
            existing.UpdatedBy = customer.UpdatedBy;

            return (true, "Customer updated successfully.");
        }

        /// <summary>Delete customer (soft delete - blocked if AR transactions exist)</summary>
        public (bool Success, string Message) Delete(Guid id)
        {
            var customer = _customers.FirstOrDefault(c => c.Id == id && !c.IsDeleted);
            if (customer == null)
            {
                return (false, "Customer not found.");
            }

            // In a real system, check for AR transactions before deleting
            // For demo, we allow soft delete

            customer.IsDeleted = true;
            customer.DeletedAt = DateTime.Now;

            return (true, "Customer deleted successfully.");
        }

        /// <summary>Place credit hold on customer</summary>
        public (bool Success, string Message) PlaceCreditHold(Guid customerId, string holdStatus, string reason, Guid userId, string userName)
        {
            var customer = _customers.FirstOrDefault(c => c.Id == customerId && !c.IsDeleted);
            if (customer == null)
            {
                return (false, "Customer not found.");
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return (false, "Hold reason is required.");
            }

            customer.CreditHoldStatus = holdStatus;
            customer.CreditHoldReason = reason;
            customer.CreditHoldPlacedOn = DateTime.Now;
            customer.CreditHoldPlacedByUserId = userId;
            customer.CreditHoldPlacedByUserName = userName;
            customer.UpdatedAt = DateTime.Now;
            customer.UpdatedBy = userName;

            return (true, "Credit hold placed successfully.");
        }

        /// <summary>Release credit hold from customer</summary>
        public (bool Success, string Message) ReleaseCreditHold(Guid customerId, string userName)
        {
            var customer = _customers.FirstOrDefault(c => c.Id == customerId && !c.IsDeleted);
            if (customer == null)
            {
                return (false, "Customer not found.");
            }

            customer.CreditHoldStatus = CreditHoldStatuses.None;
            customer.CreditHoldReason = null;
            customer.CreditHoldPlacedOn = null;
            customer.CreditHoldPlacedByUserId = null;
            customer.CreditHoldPlacedByUserName = null;
            customer.UpdatedAt = DateTime.Now;
            customer.UpdatedBy = userName;

            return (true, "Credit hold released successfully.");
        }

        /// <summary>Activate customer</summary>
        public (bool Success, string Message) Activate(Guid customerId, string userName)
        {
            var customer = _customers.FirstOrDefault(c => c.Id == customerId && !c.IsDeleted);
            if (customer == null)
            {
                return (false, "Customer not found.");
            }

            customer.CustomerStatus = CustomerStatuses.Active;
            customer.UpdatedAt = DateTime.Now;
            customer.UpdatedBy = userName;

            return (true, "Customer activated successfully.");
        }

        /// <summary>Inactivate customer</summary>
        public (bool Success, string Message) Inactivate(Guid customerId, string userName)
        {
            var customer = _customers.FirstOrDefault(c => c.Id == customerId && !c.IsDeleted);
            if (customer == null)
            {
                return (false, "Customer not found.");
            }

            customer.CustomerStatus = CustomerStatuses.Inactive;
            customer.UpdatedAt = DateTime.Now;
            customer.UpdatedBy = userName;

            return (true, "Customer inactivated successfully.");
        }

        #endregion

        #region Helper Methods

        private static bool IsGSTINRequired(string taxRegistrationType)
        {
            return taxRegistrationType == TaxRegistrationTypes.Registered ||
                   taxRegistrationType == TaxRegistrationTypes.SEZ ||
                   taxRegistrationType == TaxRegistrationTypes.Export;
        }

        private static string NormalizeSpaces(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return System.Text.RegularExpressions.Regex.Replace(input, @"\s+", " ");
        }

        #endregion
    }
}
