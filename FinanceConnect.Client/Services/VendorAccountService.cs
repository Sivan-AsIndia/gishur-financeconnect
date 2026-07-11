using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    /// <summary>
    /// Service for VendorAccount operations
    /// Demo application - data stored in memory
    /// Note: VendorAccount is primarily system-maintained in production
    /// VendorAccount is the AP subledger balance record for vendors
    /// </summary>
    public class VendorAccountService
    {
        // Immutable seed data
        private static readonly List<VendorAccountViewModel> _seedVendorAccounts = VendorAccountSeedData.GetSeedVendorAccounts();

        // Working (mutable) data
        private List<VendorAccountViewModel> _vendorAccounts;

        public VendorAccountService()
        {
            ResetToSeed();
        }

        private static List<T> CloneList<T>(IEnumerable<T> source) where T : class
        {
            var json = System.Text.Json.JsonSerializer.Serialize(source);
            return System.Text.Json.JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }

        /// <summary>Reset vendor accounts to seed data</summary>
        public void ResetToSeed()
        {
            _vendorAccounts = CloneList(_seedVendorAccounts);
        }

        #region Read Operations

        /// <summary>Get all vendor accounts</summary>
        public List<VendorAccountViewModel> GetAll()
        {
            return _vendorAccounts.Where(va => !va.IsDeleted).ToList();
        }

        /// <summary>Get vendor account by ID</summary>
        public VendorAccountViewModel? GetById(Guid id)
        {
            return _vendorAccounts.FirstOrDefault(va => va.Id == id && !va.IsDeleted);
        }

        /// <summary>Get vendor account by vendor ID and currency</summary>
        public VendorAccountViewModel? GetByVendorAndCurrency(Guid vendorId, Guid currencyId)
        {
            return _vendorAccounts.FirstOrDefault(va =>
                va.VendorId == vendorId &&
                va.CurrencyId == currencyId &&
                !va.IsDeleted);
        }

        /// <summary>Get all vendor accounts for a vendor</summary>
        public List<VendorAccountViewModel> GetByVendorId(Guid vendorId)
        {
            return _vendorAccounts.Where(va => va.VendorId == vendorId && !va.IsDeleted).ToList();
        }

        /// <summary>Get vendor accounts by company ID</summary>
        public List<VendorAccountViewModel> GetByCompanyId(Guid companyId)
        {
            return _vendorAccounts.Where(va => va.CompanyId == companyId && !va.IsDeleted).ToList();
        }

        /// <summary>Get vendor accounts by status</summary>
        public List<VendorAccountViewModel> GetByStatus(string status)
        {
            return _vendorAccounts.Where(va => va.AccountStatus == status && !va.IsDeleted).ToList();
        }

        /// <summary>Get vendor accounts with payment blocked</summary>
        public List<VendorAccountViewModel> GetPaymentBlocked()
        {
            return _vendorAccounts.Where(va => va.IsPaymentBlocked && !va.IsDeleted).ToList();
        }

        /// <summary>Get vendor accounts with posting blocked</summary>
        public List<VendorAccountViewModel> GetPostingBlocked()
        {
            return _vendorAccounts.Where(va => va.IsPostingBlocked && !va.IsDeleted).ToList();
        }

        /// <summary>Get vendor accounts with outstanding payable</summary>
        public List<VendorAccountViewModel> GetWithOutstandingPayable()
        {
            return _vendorAccounts.Where(va => va.OutstandingPayableAmount > 0 && !va.IsDeleted).ToList();
        }

        /// <summary>Get vendor accounts with advance balance (advance surplus)</summary>
        public List<VendorAccountViewModel> GetWithAdvanceSurplus()
        {
            return _vendorAccounts.Where(va => va.HasAdvanceSurplus && !va.IsDeleted).ToList();
        }

        /// <summary>Search vendor accounts by vendor code or name</summary>
        public List<VendorAccountViewModel> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAll();

            searchTerm = searchTerm.ToLower();
            return _vendorAccounts.Where(va => !va.IsDeleted && (
                (va.VendorCode?.ToLower().Contains(searchTerm) ?? false) ||
                (va.VendorName?.ToLower().Contains(searchTerm) ?? false) ||
                (va.CurrencyCode?.ToLower().Contains(searchTerm) ?? false)
            )).ToList();
        }

        /// <summary>Check if vendor account exists for vendor and currency</summary>
        public bool AccountExists(Guid companyId, Guid vendorId, Guid currencyId, Guid? excludeId = null)
        {
            return _vendorAccounts.Any(va =>
                va.CompanyId == companyId &&
                va.VendorId == vendorId &&
                va.CurrencyId == currencyId &&
                !va.IsDeleted &&
                (excludeId == null || va.Id != excludeId));
        }

        /// <summary>Get vendor account summary statistics</summary>
        public VendorAccountSummaryStatsViewModel GetSummaryStats(Guid? companyId = null)
        {
            var accounts = companyId.HasValue
                ? _vendorAccounts.Where(va => va.CompanyId == companyId && !va.IsDeleted)
                : _vendorAccounts.Where(va => !va.IsDeleted);

            return new VendorAccountSummaryStatsViewModel
            {
                TotalAccounts = accounts.Count(),
                ActiveAccounts = accounts.Count(va => va.AccountStatus == VendorAccountStatuses.Active),
                FrozenAccounts = accounts.Count(va => va.AccountStatus == VendorAccountStatuses.Frozen),
                PaymentBlockedAccounts = accounts.Count(va => va.IsPaymentBlocked),
                PostingBlockedAccounts = accounts.Count(va => va.IsPostingBlocked),
                TotalOutstandingPayable = accounts.Sum(va => va.OutstandingPayableAmount),
                TotalAdvancePaid = accounts.Sum(va => va.AdvancePaidAmount),
                AccountsWithAdvanceSurplus = accounts.Count(va => va.HasAdvanceSurplus)
            };
        }

        #endregion

        #region Write Operations (Controller/System Only in Production)

        /// <summary>Create vendor account (typically system-triggered when vendor is created)</summary>
        public (bool Success, string Message) Create(VendorAccountViewModel account)
        {
            // Validate uniqueness: (TenantId, CompanyId, VendorId, CurrencyId) must be unique
            if (AccountExists(account.CompanyId, account.VendorId, account.CurrencyId))
            {
                return (false, "VendorAccount already exists for this vendor & currency.");
            }

            account.Id = Guid.NewGuid();
            account.CreatedAt = DateTime.Now;
            account.IsDeleted = false;

            // Initialize balances to zero (system will update via posting pipeline)
            account.OutstandingPayableAmount = 0;
            account.AdvancePaidAmount = 0;

            _vendorAccounts.Add(account);
            return (true, "Vendor account created successfully.");
        }

        /// <summary>Freeze vendor account (sets AccountStatus to Frozen and blocks both posting and payment)</summary>
        public (bool Success, string Message) Freeze(Guid accountId, string reason, Guid userId, string userName)
        {
            var account = _vendorAccounts.FirstOrDefault(va => va.Id == accountId && !va.IsDeleted);
            if (account == null)
            {
                return (false, "Vendor account not found.");
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return (false, "Block reason is required.");
            }

            account.AccountStatus = VendorAccountStatuses.Frozen;
            account.IsPaymentBlocked = true;
            account.PaymentBlockReason = reason;
            account.IsPostingBlocked = true;
            account.PostingBlockReason = reason;
            account.BlockedOn = DateTime.Now;
            account.BlockedByUserId = userId;
            account.BlockedByUserName = userName;
            account.UpdatedAt = DateTime.Now;
            account.UpdatedBy = userName;

            return (true, "Vendor account frozen successfully.");
        }

        /// <summary>Unfreeze vendor account (sets AccountStatus to Active and removes all blocks)</summary>
        public (bool Success, string Message) Unfreeze(Guid accountId, string userName)
        {
            var account = _vendorAccounts.FirstOrDefault(va => va.Id == accountId && !va.IsDeleted);
            if (account == null)
            {
                return (false, "Vendor account not found.");
            }

            account.AccountStatus = VendorAccountStatuses.Active;
            account.IsPaymentBlocked = false;
            account.PaymentBlockReason = null;
            account.IsPostingBlocked = false;
            account.PostingBlockReason = null;
            account.BlockedOn = null;
            account.BlockedByUserId = null;
            account.BlockedByUserName = null;
            account.UpdatedAt = DateTime.Now;
            account.UpdatedBy = userName;

            return (true, "Vendor account unfrozen successfully.");
        }

        /// <summary>Block payment only (for bank verification pending, etc.)</summary>
        public (bool Success, string Message) BlockPayment(Guid accountId, string reason, Guid userId, string userName)
        {
            var account = _vendorAccounts.FirstOrDefault(va => va.Id == accountId && !va.IsDeleted);
            if (account == null)
            {
                return (false, "Vendor account not found.");
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return (false, "Payment block reason is required.");
            }

            account.IsPaymentBlocked = true;
            account.PaymentBlockReason = reason;
            account.BlockedOn = DateTime.Now;
            account.BlockedByUserId = userId;
            account.BlockedByUserName = userName;
            account.UpdatedAt = DateTime.Now;
            account.UpdatedBy = userName;

            return (true, "Payment blocked successfully.");
        }

        /// <summary>Unblock payment</summary>
        public (bool Success, string Message) UnblockPayment(Guid accountId, string userName)
        {
            var account = _vendorAccounts.FirstOrDefault(va => va.Id == accountId && !va.IsDeleted);
            if (account == null)
            {
                return (false, "Vendor account not found.");
            }

            account.IsPaymentBlocked = false;
            account.PaymentBlockReason = null;
            
            // Clear blocked info only if posting is also not blocked
            if (!account.IsPostingBlocked)
            {
                account.BlockedOn = null;
                account.BlockedByUserId = null;
                account.BlockedByUserName = null;
            }
            
            account.UpdatedAt = DateTime.Now;
            account.UpdatedBy = userName;

            return (true, "Payment unblocked successfully.");
        }

        /// <summary>Block posting only</summary>
        public (bool Success, string Message) BlockPosting(Guid accountId, string reason, Guid userId, string userName)
        {
            var account = _vendorAccounts.FirstOrDefault(va => va.Id == accountId && !va.IsDeleted);
            if (account == null)
            {
                return (false, "Vendor account not found.");
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return (false, "Posting block reason is required.");
            }

            account.IsPostingBlocked = true;
            account.PostingBlockReason = reason;
            account.BlockedOn = DateTime.Now;
            account.BlockedByUserId = userId;
            account.BlockedByUserName = userName;
            account.UpdatedAt = DateTime.Now;
            account.UpdatedBy = userName;

            return (true, "Posting blocked successfully.");
        }

        /// <summary>Unblock posting</summary>
        public (bool Success, string Message) UnblockPosting(Guid accountId, string userName)
        {
            var account = _vendorAccounts.FirstOrDefault(va => va.Id == accountId && !va.IsDeleted);
            if (account == null)
            {
                return (false, "Vendor account not found.");
            }

            account.IsPostingBlocked = false;
            account.PostingBlockReason = null;
            
            // Clear blocked info only if payment is also not blocked
            if (!account.IsPaymentBlocked)
            {
                account.BlockedOn = null;
                account.BlockedByUserId = null;
                account.BlockedByUserName = null;
            }
            
            account.UpdatedAt = DateTime.Now;
            account.UpdatedBy = userName;

            return (true, "Posting unblocked successfully.");
        }

        /// <summary>
        /// Apply posting impact (System/Internal only in production)
        /// This simulates what happens when bill/payment/note is posted
        /// </summary>
        public (bool Success, string Message) ApplyPostingImpact(
            Guid accountId,
            decimal billAmount,
            decimal paymentAmount,
            decimal creditNoteAmount,
            decimal debitNoteAmount,
            decimal adjustmentAmount,
            bool isPaymentAllocated,
            bool convertExcessToAdvance = true)
        {
            var account = _vendorAccounts.FirstOrDefault(va => va.Id == accountId && !va.IsDeleted);
            if (account == null)
            {
                return (false, "Vendor account not found.");
            }

            // Check posting block for bills/notes/adjustments
            if (account.IsPostingBlocked && (billAmount > 0 || creditNoteAmount > 0 || debitNoteAmount > 0 || adjustmentAmount > 0))
            {
                return (false, $"Posting blocked: {account.PostingBlockReason}");
            }

            // Check payment block for payments
            if (account.IsPaymentBlocked && paymentAmount > 0)
            {
                return (false, $"Payment blocked: {account.PaymentBlockReason}");
            }

            // Apply bill impact: Bill posted → increases payable
            account.OutstandingPayableAmount += billAmount;
            account.TotalBillsPostedAmount += billAmount;

            // Apply debit note impact: increases payable
            account.OutstandingPayableAmount += debitNoteAmount;
            account.TotalDebitNotesPostedAmount += debitNoteAmount;

            // Apply credit note impact: reduces payable
            account.OutstandingPayableAmount -= creditNoteAmount;
            account.TotalCreditNotesPostedAmount += creditNoteAmount;

            // Handle credit note exceeding payable (creates advance if policy allows)
            if (account.OutstandingPayableAmount < 0)
            {
                var excessCredit = Math.Abs(account.OutstandingPayableAmount);
                account.OutstandingPayableAmount = 0;
                if (convertExcessToAdvance)
                {
                    account.AdvancePaidAmount += excessCredit;
                }
            }

            // Apply adjustment impact: usually reduces payable
            account.OutstandingPayableAmount -= adjustmentAmount;
            account.TotalAdjustmentsPostedAmount += adjustmentAmount;

            // Handle adjustment exceeding payable
            if (account.OutstandingPayableAmount < 0)
            {
                account.OutstandingPayableAmount = 0;
            }

            // Apply payment impact
            if (paymentAmount > 0)
            {
                account.TotalPaymentsPostedAmount += paymentAmount;

                if (isPaymentAllocated)
                {
                    // Allocated payment: reduces payable
                    account.OutstandingPayableAmount -= paymentAmount;

                    // Handle overpayment
                    if (account.OutstandingPayableAmount < 0)
                    {
                        var overpayment = Math.Abs(account.OutstandingPayableAmount);
                        account.OutstandingPayableAmount = 0;
                        account.AdvancePaidAmount += overpayment;
                    }
                }
                else
                {
                    // Unallocated payment: increases advance
                    account.AdvancePaidAmount += paymentAmount;
                }
            }

            // Update activity dates
            account.LastTransactionOn = DateTime.Now;
            if (billAmount > 0)
            {
                account.LastBillPostedOn = DateTime.Now;
                account.LastDocumentReference = $"BILL-{DateTime.Now:HHmmss}";
            }
            if (paymentAmount > 0)
            {
                account.LastPaymentPostedOn = DateTime.Now;
                account.LastDocumentReference = $"PAY-{DateTime.Now:HHmmss}";
            }

            account.UpdatedAt = DateTime.Now;

            return (true, "Posting impact applied successfully.");
        }

        /// <summary>Update reconciliation info</summary>
        public (bool Success, string Message) UpdateReconciliation(Guid accountId, Guid userId, string userName)
        {
            var account = _vendorAccounts.FirstOrDefault(va => va.Id == accountId && !va.IsDeleted);
            if (account == null)
            {
                return (false, "Vendor account not found.");
            }

            account.LastReconciledOn = DateTime.Now;
            account.LastReconciledByUserId = userId;
            account.LastReconciledByUserName = userName;
            account.UpdatedAt = DateTime.Now;
            account.UpdatedBy = userName;

            return (true, "Reconciliation updated successfully.");
        }

        #endregion

        #region Validation Methods

        /// <summary>Check if posting is allowed for this vendor account</summary>
        public (bool Allowed, string Message) CanPostBill(Guid accountId)
        {
            var account = _vendorAccounts.FirstOrDefault(va => va.Id == accountId && !va.IsDeleted);
            if (account == null)
            {
                return (false, "Vendor account not found.");
            }

            if (account.AccountStatus == VendorAccountStatuses.Frozen ||
                account.AccountStatus == VendorAccountStatuses.Closed)
            {
                return (false, "Vendor account is frozen. Posting blocked.");
            }

            if (account.IsPostingBlocked)
            {
                return (false, $"Posting blocked: {account.PostingBlockReason}");
            }

            return (true, "Posting allowed.");
        }

        /// <summary>Check if payment is allowed for this vendor account</summary>
        public (bool Allowed, string Message) CanPostPayment(Guid accountId)
        {
            var account = _vendorAccounts.FirstOrDefault(va => va.Id == accountId && !va.IsDeleted);
            if (account == null)
            {
                return (false, "Vendor account not found.");
            }

            if (account.AccountStatus == VendorAccountStatuses.Frozen ||
                account.AccountStatus == VendorAccountStatuses.Closed)
            {
                return (false, "Vendor account is frozen. Payments blocked.");
            }

            if (account.IsPaymentBlocked)
            {
                return (false, $"Payments blocked: {account.PaymentBlockReason}");
            }

            return (true, "Payment allowed.");
        }

        #endregion
    }

    /// <summary>Vendor account summary statistics DTO</summary>
    public class VendorAccountSummaryStatsViewModel
    {
        public int TotalAccounts { get; set; }
        public int ActiveAccounts { get; set; }
        public int FrozenAccounts { get; set; }
        public int PaymentBlockedAccounts { get; set; }
        public int PostingBlockedAccounts { get; set; }
        public decimal TotalOutstandingPayable { get; set; }
        public decimal TotalAdvancePaid { get; set; }
        public int AccountsWithAdvanceSurplus { get; set; }
    }
}
