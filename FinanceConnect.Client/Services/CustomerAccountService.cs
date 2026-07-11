using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    /// <summary>
    /// Service for CustomerAccount operations
    /// Demo application - data stored in memory
    /// Note: CustomerAccount is primarily system-maintained in production
    /// </summary>
    public class CustomerAccountService
    {
        // Immutable seed data
        private static readonly List<CustomerAccountViewModel> _seedCustomerAccounts = CustomerAccountSeedData.GetSeedCustomerAccounts();

        // Working (mutable) data
        private List<CustomerAccountViewModel> _customerAccounts;

        public CustomerAccountService()
        {
            ResetToSeed();
        }

        private static List<T> CloneList<T>(IEnumerable<T> source) where T : class
        {
            var json = System.Text.Json.JsonSerializer.Serialize(source);
            return System.Text.Json.JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }

        /// <summary>Reset customer accounts to seed data</summary>
        public void ResetToSeed()
        {
            _customerAccounts = CloneList(_seedCustomerAccounts);
        }

        #region Read Operations

        /// <summary>Get all customer accounts</summary>
        public List<CustomerAccountViewModel> GetAll()
        {
            return _customerAccounts.Where(ca => !ca.IsDeleted).ToList();
        }

        /// <summary>Get customer account by ID</summary>
        public CustomerAccountViewModel? GetById(Guid id)
        {
            return _customerAccounts.FirstOrDefault(ca => ca.Id == id && !ca.IsDeleted);
        }

        /// <summary>Get customer account by customer ID and currency</summary>
        public CustomerAccountViewModel? GetByCustomerAndCurrency(Guid customerId, Guid currencyId)
        {
            return _customerAccounts.FirstOrDefault(ca =>
                ca.CustomerId == customerId &&
                ca.CurrencyId == currencyId &&
                !ca.IsDeleted);
        }

        /// <summary>Get all customer accounts for a customer</summary>
        public List<CustomerAccountViewModel> GetByCustomerId(Guid customerId)
        {
            return _customerAccounts.Where(ca => ca.CustomerId == customerId && !ca.IsDeleted).ToList();
        }

        /// <summary>Get customer accounts by company ID</summary>
        public List<CustomerAccountViewModel> GetByCompanyId(Guid companyId)
        {
            return _customerAccounts.Where(ca => ca.CompanyId == companyId && !ca.IsDeleted).ToList();
        }

        /// <summary>Get customer accounts by status</summary>
        public List<CustomerAccountViewModel> GetByStatus(string status)
        {
            return _customerAccounts.Where(ca => ca.AccountStatus == status && !ca.IsDeleted).ToList();
        }

        /// <summary>Get customer accounts with posting blocked</summary>
        public List<CustomerAccountViewModel> GetPostingBlocked()
        {
            return _customerAccounts.Where(ca => ca.IsPostingBlocked && !ca.IsDeleted).ToList();
        }

        /// <summary>Get customer accounts with outstanding balance</summary>
        public List<CustomerAccountViewModel> GetWithOutstandingBalance()
        {
            return _customerAccounts.Where(ca => ca.OutstandingReceivableAmount > 0 && !ca.IsDeleted).ToList();
        }

        /// <summary>Get customer accounts over credit limit</summary>
        public List<CustomerAccountViewModel> GetOverCreditLimit()
        {
            return _customerAccounts.Where(ca => ca.OverCreditAmount > 0 && !ca.IsDeleted).ToList();
        }

        /// <summary>Search customer accounts by customer code or name</summary>
        public List<CustomerAccountViewModel> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAll();

            searchTerm = searchTerm.ToLower();
            return _customerAccounts.Where(ca => !ca.IsDeleted && (
                (ca.CustomerCode?.ToLower().Contains(searchTerm) ?? false) ||
                (ca.CustomerName?.ToLower().Contains(searchTerm) ?? false) ||
                (ca.CurrencyCode?.ToLower().Contains(searchTerm) ?? false)
            )).ToList();
        }

        /// <summary>Check if customer account exists for customer and currency</summary>
        public bool AccountExists(Guid companyId, Guid customerId, Guid currencyId, Guid? excludeId = null)
        {
            return _customerAccounts.Any(ca =>
                ca.CompanyId == companyId &&
                ca.CustomerId == customerId &&
                ca.CurrencyId == currencyId &&
                !ca.IsDeleted &&
                (excludeId == null || ca.Id != excludeId));
        }

        /// <summary>Get account summary statistics</summary>
        public AccountSummaryStatsViewModel GetSummaryStats(Guid? companyId = null)
        {
            var accounts = companyId.HasValue
                ? _customerAccounts.Where(ca => ca.CompanyId == companyId && !ca.IsDeleted)
                : _customerAccounts.Where(ca => !ca.IsDeleted);

            return new AccountSummaryStatsViewModel
            {
                TotalAccounts = accounts.Count(),
                ActiveAccounts = accounts.Count(ca => ca.AccountStatus == CustomerAccountStatuses.Active),
                FrozenAccounts = accounts.Count(ca => ca.AccountStatus == CustomerAccountStatuses.Frozen),
                PostingBlockedAccounts = accounts.Count(ca => ca.IsPostingBlocked),
                TotalOutstanding = accounts.Sum(ca => ca.OutstandingReceivableAmount),
                TotalUnapplied = accounts.Sum(ca => ca.UnappliedPaymentAmount),
                TotalAdvances = accounts.Sum(ca => ca.AdvanceBalanceAmount),
                AccountsOverCreditLimit = accounts.Count(ca => ca.OverCreditAmount > 0)
            };
        }

        #endregion

        #region Write Operations (Controller/System Only in Production)

        /// <summary>Create customer account (typically system-triggered when customer is created)</summary>
        public (bool Success, string Message) Create(CustomerAccountViewModel account)
        {
            // Validate uniqueness
            if (AccountExists(account.CompanyId, account.CustomerId, account.CurrencyId))
            {
                return (false, "CustomerAccount already exists for this customer & currency.");
            }

            account.Id = Guid.NewGuid();
            account.CreatedAt = DateTime.Now;
            account.IsDeleted = false;

            // Initialize balances
            account.OutstandingReceivableAmount = account.OpeningReceivableAmount;
            account.AdvanceBalanceAmount = account.OpeningAdvanceAmount;

            _customerAccounts.Add(account);
            return (true, "Customer account created successfully.");
        }

        /// <summary>Freeze customer account</summary>
        public (bool Success, string Message) Freeze(Guid accountId, string reason, string freezeType, Guid userId, string userName)
        {
            var account = _customerAccounts.FirstOrDefault(ca => ca.Id == accountId && !ca.IsDeleted);
            if (account == null)
            {
                return (false, "Customer account not found.");
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return (false, "Block reason is required.");
            }

            account.AccountStatus = CustomerAccountStatuses.Frozen;
            account.IsPostingBlocked = true;
            account.PostingBlockReason = reason;
            account.PostingBlockedByUserId = userId;
            account.PostingBlockedByUserName = userName;
            account.PostingBlockedOn = DateTime.Now;
            account.FreezeType = freezeType;
            account.UpdatedAt = DateTime.Now;
            account.UpdatedBy = userName;

            return (true, "Customer account frozen successfully.");
        }

        /// <summary>Unfreeze customer account</summary>
        public (bool Success, string Message) Unfreeze(Guid accountId, string userName)
        {
            var account = _customerAccounts.FirstOrDefault(ca => ca.Id == accountId && !ca.IsDeleted);
            if (account == null)
            {
                return (false, "Customer account not found.");
            }

            account.AccountStatus = CustomerAccountStatuses.Active;
            account.IsPostingBlocked = false;
            account.PostingBlockReason = null;
            account.PostingBlockedByUserId = null;
            account.PostingBlockedByUserName = null;
            account.PostingBlockedOn = null;
            account.FreezeType = FreezeTypes.None;
            account.UpdatedAt = DateTime.Now;
            account.UpdatedBy = userName;

            return (true, "Customer account unfrozen successfully.");
        }

        /// <summary>Block posting on customer account</summary>
        public (bool Success, string Message) BlockPosting(Guid accountId, string reason, Guid userId, string userName)
        {
            var account = _customerAccounts.FirstOrDefault(ca => ca.Id == accountId && !ca.IsDeleted);
            if (account == null)
            {
                return (false, "Customer account not found.");
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return (false, "Block reason is required.");
            }

            account.IsPostingBlocked = true;
            account.PostingBlockReason = reason;
            account.PostingBlockedByUserId = userId;
            account.PostingBlockedByUserName = userName;
            account.PostingBlockedOn = DateTime.Now;
            account.UpdatedAt = DateTime.Now;
            account.UpdatedBy = userName;

            return (true, "Posting blocked successfully.");
        }

        /// <summary>Unblock posting on customer account</summary>
        public (bool Success, string Message) UnblockPosting(Guid accountId, string userName)
        {
            var account = _customerAccounts.FirstOrDefault(ca => ca.Id == accountId && !ca.IsDeleted);
            if (account == null)
            {
                return (false, "Customer account not found.");
            }

            account.IsPostingBlocked = false;
            account.PostingBlockReason = null;
            account.PostingBlockedByUserId = null;
            account.PostingBlockedByUserName = null;
            account.PostingBlockedOn = null;
            account.UpdatedAt = DateTime.Now;
            account.UpdatedBy = userName;

            return (true, "Posting unblocked successfully.");
        }

        /// <summary>Import opening balance (Controller only)</summary>
        public (bool Success, string Message) ImportOpeningBalance(
            Guid accountId,
            decimal openingReceivable,
            decimal openingAdvance,
            DateTime asOfDate,
            Guid? batchId,
            Guid approvedByUserId,
            string approvedByUserName)
        {
            var account = _customerAccounts.FirstOrDefault(ca => ca.Id == accountId && !ca.IsDeleted);
            if (account == null)
            {
                return (false, "Customer account not found.");
            }

            account.OpeningReceivableAmount = openingReceivable;
            account.OpeningAdvanceAmount = openingAdvance;
            account.OpeningBalanceAsOfDate = asOfDate;
            account.OpeningBalanceImportBatchId = batchId;
            account.OpeningBalanceApprovedByUserId = approvedByUserId;
            account.OpeningBalanceApprovedByUserName = approvedByUserName;
            account.OpeningBalanceApprovedOn = DateTime.Now;

            // Update running balances
            account.OutstandingReceivableAmount = openingReceivable;
            account.AdvanceBalanceAmount = openingAdvance;

            account.UpdatedAt = DateTime.Now;
            account.UpdatedBy = approvedByUserName;

            return (true, "Opening balance imported successfully.");
        }

        /// <summary>Update credit limit snapshot from customer</summary>
        public (bool Success, string Message) UpdateCreditLimitSnapshot(Guid accountId, decimal creditLimit, bool enforced)
        {
            var account = _customerAccounts.FirstOrDefault(ca => ca.Id == accountId && !ca.IsDeleted);
            if (account == null)
            {
                return (false, "Customer account not found.");
            }

            account.CreditLimitAmountSnapshot = creditLimit;
            account.CreditLimitEnforcedSnapshot = enforced;
            account.UpdatedAt = DateTime.Now;

            return (true, "Credit limit snapshot updated.");
        }

        /// <summary>Update collections stage</summary>
        public (bool Success, string Message) UpdateCollectionsStage(Guid accountId, string stage, string userName)
        {
            var account = _customerAccounts.FirstOrDefault(ca => ca.Id == accountId && !ca.IsDeleted);
            if (account == null)
            {
                return (false, "Customer account not found.");
            }

            account.CollectionsStage = stage;
            account.UpdatedAt = DateTime.Now;
            account.UpdatedBy = userName;

            return (true, "Collections stage updated.");
        }

        /// <summary>
        /// Apply posting impact (System/Internal only in production)
        /// This simulates what happens when invoice/payment/note is posted
        /// </summary>
        public (bool Success, string Message) ApplyPostingImpact(
            Guid accountId,
            decimal invoiceAmount,
            decimal paymentAmount,
            decimal creditNoteAmount,
            decimal debitNoteAmount,
            bool isPaymentAllocated,
            bool convertOverpaymentToAdvance)
        {
            var account = _customerAccounts.FirstOrDefault(ca => ca.Id == accountId && !ca.IsDeleted);
            if (account == null)
            {
                return (false, "Customer account not found.");
            }

            if (account.IsPostingBlocked)
            {
                return (false, "Customer account is frozen. Posting blocked.");
            }

            // Apply invoice impact
            account.OutstandingReceivableAmount += invoiceAmount;

            // Apply debit note impact
            account.OutstandingReceivableAmount += debitNoteAmount;

            // Apply credit note impact
            account.OutstandingReceivableAmount -= creditNoteAmount;

            // Apply payment impact
            if (isPaymentAllocated)
            {
                account.OutstandingReceivableAmount -= paymentAmount;

                // Handle overpayment
                if (account.OutstandingReceivableAmount < 0)
                {
                    var overpayment = Math.Abs(account.OutstandingReceivableAmount);
                    account.OutstandingReceivableAmount = 0;

                    if (convertOverpaymentToAdvance)
                    {
                        account.AdvanceBalanceAmount += overpayment;
                    }
                    else
                    {
                        account.UnappliedPaymentAmount += overpayment;
                    }
                }
            }
            else
            {
                // Unallocated payment
                account.UnappliedPaymentAmount += paymentAmount;
            }

            // Update activity dates
            account.LastActivityOn = DateTime.Now;
            if (invoiceAmount > 0)
                account.LastInvoiceOn = DateTime.Now;
            if (paymentAmount > 0)
                account.LastPaymentOn = DateTime.Now;

            account.UpdatedAt = DateTime.Now;

            return (true, "Posting impact applied successfully.");
        }

        #endregion

        #region Validation Methods

        /// <summary>Check if posting is allowed (credit exposure check)</summary>
        public (bool Allowed, string Message) CanPost(Guid accountId, decimal newInvoiceAmount)
        {
            var account = _customerAccounts.FirstOrDefault(ca => ca.Id == accountId && !ca.IsDeleted);
            if (account == null)
            {
                return (false, "Customer account not found.");
            }

            if (account.AccountStatus == CustomerAccountStatuses.Frozen ||
                account.AccountStatus == CustomerAccountStatuses.Closed)
            {
                return (false, "Customer account is frozen. Posting blocked.");
            }

            if (account.IsPostingBlocked)
            {
                return (false, $"Posting blocked: {account.PostingBlockReason}");
            }

            if (account.CreditLimitEnforcedSnapshot)
            {
                var newExposure = account.CreditExposureAmount + newInvoiceAmount;
                if (newExposure > account.CreditLimitAmountSnapshot)
                {
                    var overLimit = newExposure - account.CreditLimitAmountSnapshot;
                    return (false, $"Customer exceeded credit limit by {overLimit:C}. " +
                        $"Current exposure: {account.CreditExposureAmount:C}, " +
                        $"Credit limit: {account.CreditLimitAmountSnapshot:C}, " +
                        $"Invoice amount: {newInvoiceAmount:C}");
                }
            }

            return (true, "Posting allowed.");
        }

        #endregion
    }
}
