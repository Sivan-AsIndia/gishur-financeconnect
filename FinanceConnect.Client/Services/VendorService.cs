using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    /// <summary>
    /// Service for Vendor CRUD operations
    /// Demo application - data stored in memory
    /// </summary>
    public class VendorService
    {
        // Immutable seed data
        private static readonly List<VendorViewModel> _seedVendors = VendorSeedData.GetSeedVendors();

        // Working (mutable) data
        private List<VendorViewModel> _vendors;

        public VendorService()
        {
            ResetToSeed();
        }

        private static List<T> CloneList<T>(IEnumerable<T> source) where T : class
        {
            var json = System.Text.Json.JsonSerializer.Serialize(source);
            return System.Text.Json.JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }

        /// <summary>Reset vendors to seed data</summary>
        public void ResetToSeed()
        {
            _vendors = CloneList(_seedVendors);
        }

        #region Read Operations

        /// <summary>Get all vendors</summary>
        public List<VendorViewModel> GetAll()
        {
            return _vendors.Where(v => !v.IsDeleted).ToList();
        }

        /// <summary>Get vendor by ID</summary>
        public VendorViewModel? GetById(Guid id)
        {
            return _vendors.FirstOrDefault(v => v.Id == id && !v.IsDeleted);
        }

        /// <summary>Get vendors by company ID</summary>
        public List<VendorViewModel> GetByCompanyId(Guid companyId)
        {
            return _vendors.Where(v => v.CompanyId == companyId && !v.IsDeleted).ToList();
        }

        /// <summary>Get vendors by status</summary>
        public List<VendorViewModel> GetByStatus(string status)
        {
            return _vendors.Where(v => v.VendorStatus == status && !v.IsDeleted).ToList();
        }

        /// <summary>Get vendors by type</summary>
        public List<VendorViewModel> GetByType(string type)
        {
            return _vendors.Where(v => v.VendorType == type && !v.IsDeleted).ToList();
        }

        /// <summary>Get vendors on hold</summary>
        public List<VendorViewModel> GetOnHold()
        {
            return _vendors.Where(v => v.VendorStatus == VendorStatuses.OnHold && !v.IsDeleted).ToList();
        }

        /// <summary>Get blacklisted vendors</summary>
        public List<VendorViewModel> GetBlacklisted()
        {
            return _vendors.Where(v => v.VendorStatus == VendorStatuses.Blacklisted && !v.IsDeleted).ToList();
        }

        /// <summary>Get vendors with payment blocked</summary>
        public List<VendorViewModel> GetPaymentBlocked()
        {
            return _vendors.Where(v => v.IsPaymentBlocked && !v.IsDeleted).ToList();
        }

        /// <summary>Search vendors by code, name, GSTIN, PAN, phone, or email</summary>
        public List<VendorViewModel> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAll();

            searchTerm = searchTerm.ToLower();
            return _vendors.Where(v => !v.IsDeleted && (
                v.VendorCode.ToLower().Contains(searchTerm) ||
                v.VendorName.ToLower().Contains(searchTerm) ||
                (v.LegalName?.ToLower().Contains(searchTerm) ?? false) ||
                (v.GSTIN?.ToLower().Contains(searchTerm) ?? false) ||
                (v.PAN?.ToLower().Contains(searchTerm) ?? false) ||
                (v.PrimaryPhone?.ToLower().Contains(searchTerm) ?? false) ||
                (v.PrimaryEmail?.ToLower().Contains(searchTerm) ?? false)
            )).ToList();
        }

        /// <summary>Check if vendor code exists within company</summary>
        public bool VendorCodeExists(Guid companyId, string vendorCode, Guid? excludeId = null)
        {
            return _vendors.Any(v =>
                v.CompanyId == companyId &&
                v.VendorCode.Equals(vendorCode, StringComparison.OrdinalIgnoreCase) &&
                !v.IsDeleted &&
                (excludeId == null || v.Id != excludeId));
        }

        /// <summary>Check if GSTIN exists within company</summary>
        public bool GSTINExists(Guid companyId, string gstin, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(gstin))
                return false;

            return _vendors.Any(v =>
                v.CompanyId == companyId &&
                v.GSTIN != null &&
                v.GSTIN.Equals(gstin, StringComparison.OrdinalIgnoreCase) &&
                !v.IsDeleted &&
                (excludeId == null || v.Id != excludeId));
        }

        /// <summary>Check if PAN exists within company (optional policy)</summary>
        public bool PANExists(Guid companyId, string pan, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(pan))
                return false;

            return _vendors.Any(v =>
                v.CompanyId == companyId &&
                v.PAN != null &&
                v.PAN.Equals(pan, StringComparison.OrdinalIgnoreCase) &&
                !v.IsDeleted &&
                (excludeId == null || v.Id != excludeId));
        }

        #endregion

        #region Write Operations

        /// <summary>Add new vendor</summary>
        public (bool Success, string Message) Add(VendorViewModel vendor)
        {
            // Validate vendor code uniqueness
            if (VendorCodeExists(vendor.CompanyId, vendor.VendorCode))
            {
                return (false, "Vendor code already exists.");
            }

            // Validate GSTIN uniqueness if provided
            if (!string.IsNullOrWhiteSpace(vendor.GSTIN) && GSTINExists(vendor.CompanyId, vendor.GSTIN))
            {
                return (false, "GSTIN already exists for another vendor.");
            }

            // Validate GSTIN requirement based on IsGSTRegistered
            if (vendor.IsGSTRegistered && string.IsNullOrWhiteSpace(vendor.GSTIN))
            {
                return (false, "GSTIN is required for GST registered vendors.");
            }

            // Validate Hold Reason
            if (vendor.VendorStatus == VendorStatuses.OnHold && string.IsNullOrWhiteSpace(vendor.HoldReason))
            {
                return (false, "Hold Reason is required when vendor is on hold.");
            }

            // Validate Blacklist Reason
            if (vendor.VendorStatus == VendorStatuses.Blacklisted && string.IsNullOrWhiteSpace(vendor.BlacklistReason))
            {
                return (false, "Blacklist Reason is required when vendor is blacklisted.");
            }

            // Validate PAN requirement for TDS
            if (vendor.IsTDSApplicable && string.IsNullOrWhiteSpace(vendor.PAN))
            {
                return (false, "PAN is required when TDS is applicable.");
            }

            // Validate TDS Section Code for TDS
            if (vendor.IsTDSApplicable && string.IsNullOrWhiteSpace(vendor.TDSSectionCode))
            {
                return (false, "TDS Section Code is required when TDS is applicable.");
            }

            // Validate Payment Block Reason
            if (vendor.IsPaymentBlocked && string.IsNullOrWhiteSpace(vendor.PaymentBlockReason))
            {
                return (false, "Payment Block Reason is required when payment is blocked.");
            }

            // Validate Bill Posting Block Reason
            if (vendor.IsBillPostingBlocked && string.IsNullOrWhiteSpace(vendor.BillPostingBlockReason))
            {
                return (false, "Bill Posting Block Reason is required when bill posting is blocked.");
            }

            vendor.Id = Guid.NewGuid();
            vendor.VendorCode = vendor.VendorCode.ToUpper().Trim();
            vendor.VendorName = NormalizeSpaces(vendor.VendorName.Trim());
            vendor.CreatedAt = DateTime.Now;
            vendor.IsDeleted = false;

            _vendors.Add(vendor);
            return (true, "Vendor added successfully.");
        }

        /// <summary>Update existing vendor</summary>
        public (bool Success, string Message) Update(VendorViewModel vendor)
        {
            var existing = _vendors.FirstOrDefault(v => v.Id == vendor.Id && !v.IsDeleted);
            if (existing == null)
            {
                return (false, "Vendor not found.");
            }

            // Validate vendor code uniqueness (excluding current record)
            if (VendorCodeExists(vendor.CompanyId, vendor.VendorCode, vendor.Id))
            {
                return (false, "Vendor code already exists.");
            }

            // Validate GSTIN uniqueness if provided
            if (!string.IsNullOrWhiteSpace(vendor.GSTIN) && GSTINExists(vendor.CompanyId, vendor.GSTIN, vendor.Id))
            {
                return (false, "GSTIN already exists for another vendor.");
            }

            // Validate GSTIN requirement
            if (vendor.IsGSTRegistered && string.IsNullOrWhiteSpace(vendor.GSTIN))
            {
                return (false, "GSTIN is required for GST registered vendors.");
            }

            // Validate Hold Reason
            if (vendor.VendorStatus == VendorStatuses.OnHold && string.IsNullOrWhiteSpace(vendor.HoldReason))
            {
                return (false, "Hold Reason is required when vendor is on hold.");
            }

            // Validate Blacklist Reason
            if (vendor.VendorStatus == VendorStatuses.Blacklisted && string.IsNullOrWhiteSpace(vendor.BlacklistReason))
            {
                return (false, "Blacklist Reason is required when vendor is blacklisted.");
            }

            // Validate PAN and TDS Section for TDS
            if (vendor.IsTDSApplicable)
            {
                if (string.IsNullOrWhiteSpace(vendor.PAN))
                    return (false, "PAN is required when TDS is applicable.");
                if (string.IsNullOrWhiteSpace(vendor.TDSSectionCode))
                    return (false, "TDS Section Code is required when TDS is applicable.");
            }

            // Check if bank details changed - reset verification
            bool bankDetailsChanged = existing.BankAccountNumber != vendor.BankAccountNumber ||
                                      existing.IFSC != vendor.IFSC ||
                                      existing.UPIId != vendor.UPIId;

            // Update fields
            existing.VendorCode = vendor.VendorCode.ToUpper().Trim();
            existing.VendorName = NormalizeSpaces(vendor.VendorName.Trim());
            existing.LegalName = vendor.LegalName;
            existing.VendorType = vendor.VendorType;
            existing.VendorCategoryId = vendor.VendorCategoryId;
            existing.VendorCategoryName = vendor.VendorCategoryName;
            existing.VendorStatus = vendor.VendorStatus;
            existing.HoldReason = vendor.HoldReason;
            existing.BlacklistReason = vendor.BlacklistReason;
            existing.Notes = vendor.Notes;

            // Address
            existing.RegisteredAddressLine1 = vendor.RegisteredAddressLine1;
            existing.RegisteredAddressLine2 = vendor.RegisteredAddressLine2;
            existing.RegisteredAddressLine3 = vendor.RegisteredAddressLine3;
            existing.City = vendor.City;
            existing.StateId = vendor.StateId;
            existing.StateName = vendor.StateName;
            existing.CountryId = vendor.CountryId;
            existing.CountryName = vendor.CountryName;
            existing.PostalCode = vendor.PostalCode;

            // Contacts
            existing.PrimaryContactName = vendor.PrimaryContactName;
            existing.PrimaryEmail = vendor.PrimaryEmail;
            existing.PrimaryPhone = vendor.PrimaryPhone;
            existing.AlternatePhone = vendor.AlternatePhone;
            existing.RemittanceEmail = vendor.RemittanceEmail;

            // Compliance
            existing.IsGSTRegistered = vendor.IsGSTRegistered;
            existing.GSTIN = vendor.GSTIN;
            existing.PAN = vendor.PAN;
            existing.VendorGSTStateId = vendor.VendorGSTStateId;
            existing.VendorGSTStateName = vendor.VendorGSTStateName;
            existing.MSMECategory = vendor.MSMECategory;
            existing.IsTDSApplicable = vendor.IsTDSApplicable;
            existing.TDSSectionCode = vendor.TDSSectionCode;
            existing.TDSRatePercent = vendor.TDSRatePercent;
            existing.VendorResidencyType = vendor.VendorResidencyType;

            // Banking
            existing.PreferredPaymentMethod = vendor.PreferredPaymentMethod;
            existing.BankAccountName = vendor.BankAccountName;
            existing.BankAccountNumber = vendor.BankAccountNumber;
            existing.IFSC = vendor.IFSC;
            existing.BankName = vendor.BankName;
            existing.BranchName = vendor.BranchName;
            existing.UPIId = vendor.UPIId;

            // Reset bank verification if bank details changed
            if (bankDetailsChanged)
            {
                existing.IsBankVerified = false;
                existing.BankVerifiedOn = null;
                existing.BankVerifiedByUserId = null;
                existing.BankVerifiedByUserName = null;
            }

            // AP Defaults
            existing.DefaultCurrencyId = vendor.DefaultCurrencyId;
            existing.DefaultCurrencyCode = vendor.DefaultCurrencyCode;
            existing.DefaultCurrencyName = vendor.DefaultCurrencyName;
            existing.PaymentTermsId = vendor.PaymentTermsId;
            existing.PaymentTermsName = vendor.PaymentTermsName;
            existing.DefaultPayableAccountId = vendor.DefaultPayableAccountId;
            existing.DefaultPayableAccountCode = vendor.DefaultPayableAccountCode;
            existing.DefaultPayableAccountName = vendor.DefaultPayableAccountName;
            existing.AdvanceToVendorAccountId = vendor.AdvanceToVendorAccountId;
            existing.AdvanceToVendorAccountCode = vendor.AdvanceToVendorAccountCode;
            existing.AdvanceToVendorAccountName = vendor.AdvanceToVendorAccountName;
            existing.DefaultExpenseAccountId = vendor.DefaultExpenseAccountId;
            existing.DefaultExpenseAccountCode = vendor.DefaultExpenseAccountCode;
            existing.DefaultExpenseAccountName = vendor.DefaultExpenseAccountName;
            existing.DefaultTaxProfileId = vendor.DefaultTaxProfileId;
            existing.DefaultTaxProfileName = vendor.DefaultTaxProfileName;

            // Blocking
            existing.IsPaymentBlocked = vendor.IsPaymentBlocked;
            existing.PaymentBlockReason = vendor.PaymentBlockReason;
            existing.IsBillPostingBlocked = vendor.IsBillPostingBlocked;
            existing.BillPostingBlockReason = vendor.BillPostingBlockReason;

            existing.UpdatedAt = DateTime.Now;
            existing.UpdatedBy = vendor.UpdatedBy;
            existing.RowVersion++;

            return (true, "Vendor updated successfully.");
        }

        /// <summary>Delete vendor (soft delete - blocked if AP transactions exist)</summary>
        public (bool Success, string Message) Delete(Guid id)
        {
            var vendor = _vendors.FirstOrDefault(v => v.Id == id && !v.IsDeleted);
            if (vendor == null)
            {
                return (false, "Vendor not found.");
            }

            // In a real system, check for AP transactions before deleting
            // Only Draft vendors can be deleted; others must be set to Inactive

            vendor.IsDeleted = true;
            vendor.DeletedAt = DateTime.Now;

            return (true, "Vendor deleted successfully.");
        }

        /// <summary>Place hold on vendor</summary>
        public (bool Success, string Message) PlaceHold(Guid vendorId, string reason, string userName)
        {
            var vendor = _vendors.FirstOrDefault(v => v.Id == vendorId && !v.IsDeleted);
            if (vendor == null)
            {
                return (false, "Vendor not found.");
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return (false, "Hold reason is required.");
            }

            vendor.VendorStatus = VendorStatuses.OnHold;
            vendor.HoldReason = reason;
            vendor.IsPaymentBlocked = true;
            vendor.PaymentBlockReason = reason;
            vendor.IsBillPostingBlocked = true;
            vendor.BillPostingBlockReason = reason;
            vendor.UpdatedAt = DateTime.Now;
            vendor.UpdatedBy = userName;

            return (true, "Vendor placed on hold successfully.");
        }

        /// <summary>Release hold from vendor</summary>
        public (bool Success, string Message) ReleaseHold(Guid vendorId, string userName)
        {
            var vendor = _vendors.FirstOrDefault(v => v.Id == vendorId && !v.IsDeleted);
            if (vendor == null)
            {
                return (false, "Vendor not found.");
            }

            vendor.VendorStatus = VendorStatuses.Active;
            vendor.HoldReason = null;
            vendor.IsPaymentBlocked = false;
            vendor.PaymentBlockReason = null;
            vendor.IsBillPostingBlocked = false;
            vendor.BillPostingBlockReason = null;
            vendor.UpdatedAt = DateTime.Now;
            vendor.UpdatedBy = userName;

            return (true, "Hold released successfully.");
        }

        /// <summary>Blacklist vendor</summary>
        public (bool Success, string Message) Blacklist(Guid vendorId, string reason, string userName)
        {
            var vendor = _vendors.FirstOrDefault(v => v.Id == vendorId && !v.IsDeleted);
            if (vendor == null)
            {
                return (false, "Vendor not found.");
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return (false, "Blacklist reason is required.");
            }

            vendor.VendorStatus = VendorStatuses.Blacklisted;
            vendor.BlacklistReason = reason;
            vendor.IsPaymentBlocked = true;
            vendor.PaymentBlockReason = "Vendor blacklisted: " + reason;
            vendor.IsBillPostingBlocked = true;
            vendor.BillPostingBlockReason = "Vendor blacklisted: " + reason;
            vendor.UpdatedAt = DateTime.Now;
            vendor.UpdatedBy = userName;

            return (true, "Vendor blacklisted successfully.");
        }

        /// <summary>Activate vendor</summary>
        public (bool Success, string Message) Activate(Guid vendorId, string userName)
        {
            var vendor = _vendors.FirstOrDefault(v => v.Id == vendorId && !v.IsDeleted);
            if (vendor == null)
            {
                return (false, "Vendor not found.");
            }

            vendor.VendorStatus = VendorStatuses.Active;
            vendor.HoldReason = null;
            vendor.BlacklistReason = null;
            vendor.UpdatedAt = DateTime.Now;
            vendor.UpdatedBy = userName;

            return (true, "Vendor activated successfully.");
        }

        /// <summary>Deactivate vendor</summary>
        public (bool Success, string Message) Deactivate(Guid vendorId, string userName)
        {
            var vendor = _vendors.FirstOrDefault(v => v.Id == vendorId && !v.IsDeleted);
            if (vendor == null)
            {
                return (false, "Vendor not found.");
            }

            vendor.VendorStatus = VendorStatuses.Inactive;
            vendor.UpdatedAt = DateTime.Now;
            vendor.UpdatedBy = userName;

            return (true, "Vendor deactivated successfully.");
        }

        /// <summary>Verify bank details</summary>
        public (bool Success, string Message) VerifyBank(Guid vendorId, Guid userId, string userName)
        {
            var vendor = _vendors.FirstOrDefault(v => v.Id == vendorId && !v.IsDeleted);
            if (vendor == null)
            {
                return (false, "Vendor not found.");
            }

            if (string.IsNullOrWhiteSpace(vendor.BankAccountNumber))
            {
                return (false, "No bank account details to verify.");
            }

            vendor.IsBankVerified = true;
            vendor.BankVerifiedOn = DateTime.Now;
            vendor.BankVerifiedByUserId = userId;
            vendor.BankVerifiedByUserName = userName;
            vendor.UpdatedAt = DateTime.Now;
            vendor.UpdatedBy = userName;

            return (true, "Bank details verified successfully.");
        }

        /// <summary>Block payment for vendor</summary>
        public (bool Success, string Message) BlockPayment(Guid vendorId, string reason, string userName)
        {
            var vendor = _vendors.FirstOrDefault(v => v.Id == vendorId && !v.IsDeleted);
            if (vendor == null)
            {
                return (false, "Vendor not found.");
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return (false, "Block reason is required.");
            }

            vendor.IsPaymentBlocked = true;
            vendor.PaymentBlockReason = reason;
            vendor.UpdatedAt = DateTime.Now;
            vendor.UpdatedBy = userName;

            return (true, "Payment blocked successfully.");
        }

        /// <summary>Unblock payment for vendor</summary>
        public (bool Success, string Message) UnblockPayment(Guid vendorId, string userName)
        {
            var vendor = _vendors.FirstOrDefault(v => v.Id == vendorId && !v.IsDeleted);
            if (vendor == null)
            {
                return (false, "Vendor not found.");
            }

            vendor.IsPaymentBlocked = false;
            vendor.PaymentBlockReason = null;
            vendor.UpdatedAt = DateTime.Now;
            vendor.UpdatedBy = userName;

            return (true, "Payment unblocked successfully.");
        }

        #endregion

        #region Helper Methods

        private static string NormalizeSpaces(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return System.Text.RegularExpressions.Regex.Replace(input, @"\s+", " ");
        }

        #endregion
    }
}
