using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    #region Vendor

    /// <summary>
    /// Vendor – The AP master record representing any external party we buy from or pay money to
    /// (suppliers, service providers, contractors, freelancers, utilities, landlords, agencies).
    /// </summary>
    public class VendorViewModel
    {
        // Section 1: Core Identity Fields

        /// <summary>PK - hidden in UI (GUID)</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Tenant identifier - hidden in UI</summary>
        public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

        /// <summary>FK → Company - hidden, derived from login/company context</summary>
        [Required(ErrorMessage = "Company is required")]
        public Guid CompanyId { get; set; }
        public string? CompanyName { get; set; }

        /// <summary>Vendor Code - unique per CompanyId, max 30 chars, uppercase + trimmed</summary>
        [Required(ErrorMessage = "Vendor Code is required")]
        [StringLength(30, ErrorMessage = "Vendor Code cannot exceed 30 characters")]
        public string VendorCode { get; set; } = string.Empty;

        /// <summary>Vendor Name (Trade Name) - max 200 chars, trim + collapse multiple spaces</summary>
        [Required(ErrorMessage = "Vendor Name is required")]
        [StringLength(200, ErrorMessage = "Vendor Name cannot exceed 200 characters")]
        public string VendorName { get; set; } = string.Empty;

        /// <summary>Legal Name - max 250 chars, mandatory for GST vendors</summary>
        [StringLength(250, ErrorMessage = "Legal Name cannot exceed 250 characters")]
        public string? LegalName { get; set; }

        /// <summary>Vendor Type - Supplier/ServiceProvider/Contractor/Freelancer/Utility/Landlord/Government/Other</summary>
        [Required(ErrorMessage = "Vendor Type is required")]
        public string VendorType { get; set; } = "";

        /// <summary>FK → VendorCategoryMaster (optional for reporting + controls)</summary>
        public Guid? VendorCategoryId { get; set; }
        public string? VendorCategoryName { get; set; }

        /// <summary>Vendor Status - Draft/Active/Inactive/OnHold/Blacklisted</summary>
        [Required(ErrorMessage = "Status is required")]
        public string VendorStatus { get; set; } = VendorStatuses.Active;

        /// <summary>Hold Reason - required if VendorStatus = OnHold, max 250 chars</summary>
        [StringLength(250, ErrorMessage = "Hold Reason cannot exceed 250 characters")]
        public string? HoldReason { get; set; }

        /// <summary>Blacklist Reason - required if VendorStatus = Blacklisted, max 250 chars</summary>
        [StringLength(250, ErrorMessage = "Blacklist Reason cannot exceed 250 characters")]
        public string? BlacklistReason { get; set; }

        /// <summary>Notes - max 1000 chars, optional</summary>
        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
        public string? Notes { get; set; }


        // Section 2: Addresses & Contacts

        /// <summary>Registered Address Line 1 - max 200 chars</summary>
        [StringLength(200, ErrorMessage = "Address Line 1 cannot exceed 200 characters")]
        public string? RegisteredAddressLine1 { get; set; }

        /// <summary>Registered Address Line 2 - max 200 chars</summary>
        [StringLength(200, ErrorMessage = "Address Line 2 cannot exceed 200 characters")]
        public string? RegisteredAddressLine2 { get; set; }

        /// <summary>Registered Address Line 3 - max 200 chars</summary>
        [StringLength(200, ErrorMessage = "Address Line 3 cannot exceed 200 characters")]
        public string? RegisteredAddressLine3 { get; set; }

        /// <summary>City - max 100 chars</summary>
        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        public string? City { get; set; }

        /// <summary>FK → State</summary>
        public Guid? StateId { get; set; }
        public string? StateName { get; set; }

        /// <summary>FK → Country</summary>
        [Required(ErrorMessage = "Country is required")]
        public Guid? CountryId { get; set; }
        public string? CountryName { get; set; }

        /// <summary>Postal Code - max 12 chars</summary>
        [StringLength(12, ErrorMessage = "Postal Code cannot exceed 12 characters")]
        public string? PostalCode { get; set; }

        /// <summary>Primary Contact Name - max 150 chars</summary>
        [StringLength(150, ErrorMessage = "Primary Contact Name cannot exceed 150 characters")]
        public string? PrimaryContactName { get; set; }

        /// <summary>Primary Email - max 150 chars, email format validation</summary>
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? PrimaryEmail { get; set; }

        /// <summary>Primary Phone - max 20 chars, phone format validation</summary>
        [StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters")]
        public string? PrimaryPhone { get; set; }

        /// <summary>Alternate Phone - max 20 chars</summary>
        [StringLength(20, ErrorMessage = "Alternate Phone cannot exceed 20 characters")]
        public string? AlternatePhone { get; set; }

        /// <summary>Remittance Email - where remittance advice is sent, max 150 chars</summary>
        [StringLength(150, ErrorMessage = "Remittance Email cannot exceed 150 characters")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? RemittanceEmail { get; set; }


        // Section 3: Compliance (India/Global Ready)

        /// <summary>Is GST Registered - toggle, default false</summary>
        public bool IsGSTRegistered { get; set; } = false;

        /// <summary>GSTIN - 15 chars, required if IsGSTRegistered = true</summary>
        [StringLength(15, MinimumLength = 15, ErrorMessage = "GSTIN must be exactly 15 characters")]
        [RegularExpression(@"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$", ErrorMessage = "Invalid GSTIN format")]
        public string? GSTIN { get; set; }

        /// <summary>PAN - 10 chars, recommended (mandatory if TDS enabled)</summary>
        [StringLength(10, MinimumLength = 10, ErrorMessage = "PAN must be exactly 10 characters")]
        [RegularExpression(@"^[A-Z]{5}[0-9]{4}[A-Z]$", ErrorMessage = "Invalid PAN format (e.g., AAAAA9999A)")]
        public string? PAN { get; set; }

        /// <summary>FK → State for GST (Vendor GST State) - required if IsGSTRegistered = true</summary>
        public Guid? VendorGSTStateId { get; set; }
        public string? VendorGSTStateName { get; set; }

        /// <summary>MSME Category - Micro/Small/Medium/NotApplicable</summary>
        public string? MSMECategory { get; set; }

        /// <summary>Is TDS Applicable - toggle, default false</summary>
        public bool IsTDSApplicable { get; set; } = false;

        /// <summary>TDS Section Code - required if IsTDSApplicable = true, max 20 chars</summary>
        [StringLength(20, ErrorMessage = "TDS Section Code cannot exceed 20 characters")]
        public string? TDSSectionCode { get; set; }

        /// <summary>TDS Rate Percent - snapshot/default, decimal(6,3)</summary>
        public decimal? TDSRatePercent { get; set; }

        /// <summary>Vendor Residency Type - Resident/NonResident, default Resident</summary>
        public string VendorResidencyType { get; set; } = "";


        // Section 4: Payment & Banking (Sensitive)

        /// <summary>Preferred Payment Method - BankTransfer/UPI/Cheque/Cash/Gateway/Other</summary>
        public string? PreferredPaymentMethod { get; set; }

        /// <summary>Bank Account Name - max 150 chars, masked view</summary>
        [StringLength(150, ErrorMessage = "Bank Account Name cannot exceed 150 characters")]
        public string? BankAccountName { get; set; }

        /// <summary>Bank Account Number - max 30 chars, masked, encrypt at rest recommended</summary>
        [StringLength(30, ErrorMessage = "Bank Account Number cannot exceed 30 characters")]
        public string? BankAccountNumber { get; set; }

        /// <summary>IFSC Code - max 11 chars</summary>
        [StringLength(11, ErrorMessage = "IFSC cannot exceed 11 characters")]
        [RegularExpression(@"^[A-Z]{4}0[A-Z0-9]{6}$", ErrorMessage = "Invalid IFSC format")]
        public string? IFSC { get; set; }

        /// <summary>Bank Name - max 150 chars</summary>
        [StringLength(150, ErrorMessage = "Bank Name cannot exceed 150 characters")]
        public string? BankName { get; set; }

        /// <summary>Branch Name - max 150 chars</summary>
        [StringLength(150, ErrorMessage = "Branch Name cannot exceed 150 characters")]
        public string? BranchName { get; set; }

        /// <summary>UPI ID - max 100 chars</summary>
        [StringLength(100, ErrorMessage = "UPI ID cannot exceed 100 characters")]
        public string? UPIId { get; set; }

        /// <summary>Is Bank Verified - read-only badge, default false</summary>
        public bool IsBankVerified { get; set; } = false;

        /// <summary>Bank Verified On - datetime when bank was verified</summary>
        public DateTime? BankVerifiedOn { get; set; }

        /// <summary>Bank Verified By User - FK → User</summary>
        public Guid? BankVerifiedByUserId { get; set; }
        public string? BankVerifiedByUserName { get; set; }


        // Section 5: AP Defaults & Controls (Posting + Policy)

        /// <summary>FK → CurrencyMaster - default Company.BaseCurrency</summary>
        [Required(ErrorMessage = "Default Currency is required")]
        public Guid? DefaultCurrencyId { get; set; }
        public string? DefaultCurrencyCode { get; set; }
        public string? DefaultCurrencyName { get; set; }

        /// <summary>FK → PaymentTermsMaster - bill due date defaulting</summary>
        public Guid? PaymentTermsId { get; set; }
        public string? PaymentTermsName { get; set; }

        /// <summary>FK → GLAccountMaster - AP Control Account (Mandatory)</summary>
        [Required(ErrorMessage = "Payable Account is required")]
        public Guid? DefaultPayableAccountId { get; set; }
        public string? DefaultPayableAccountCode { get; set; }
        public string? DefaultPayableAccountName { get; set; }

        /// <summary>FK → GLAccountMaster - Vendor Advance Account (Recommended)</summary>
        public Guid? AdvanceToVendorAccountId { get; set; }
        public string? AdvanceToVendorAccountCode { get; set; }
        public string? AdvanceToVendorAccountName { get; set; }

        /// <summary>FK → GLAccountMaster - Default Expense Account (Optional)</summary>
        public Guid? DefaultExpenseAccountId { get; set; }
        public string? DefaultExpenseAccountCode { get; set; }
        public string? DefaultExpenseAccountName { get; set; }

        /// <summary>FK → TaxProfileMaster - Default GST tax codes for bills</summary>
        public Guid? DefaultTaxProfileId { get; set; }
        public string? DefaultTaxProfileName { get; set; }

        /// <summary>Is Payment Blocked - toggle, controller only, default false</summary>
        public bool IsPaymentBlocked { get; set; } = false;

        /// <summary>Payment Block Reason - required if IsPaymentBlocked = true, max 250 chars</summary>
        [StringLength(250, ErrorMessage = "Payment Block Reason cannot exceed 250 characters")]
        public string? PaymentBlockReason { get; set; }

        /// <summary>Is Bill Posting Blocked - toggle, controller only, default false</summary>
        public bool IsBillPostingBlocked { get; set; } = false;

        /// <summary>Bill Posting Block Reason - required if IsBillPostingBlocked = true, max 250 chars</summary>
        [StringLength(250, ErrorMessage = "Bill Posting Block Reason cannot exceed 250 characters")]
        public string? BillPostingBlockReason { get; set; }


        // Section 6: Attachments (Evidence / KYC)

        /// <summary>Has Attachments - badge indicator</summary>
        public bool HasAttachments { get; set; } = false;

        /// <summary>Attachment Count - badge</summary>
        public int AttachmentCount { get; set; } = 0;

        /// <summary>List of Attachments - KYC/Evidence documents</summary>
        public List<VendorAttachmentViewModel> Attachments { get; set; } = new();


        // Section 7: System Audit Fields

        /// <summary>Created At - system timestamp</summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>Created By - user name</summary>
        public string? CreatedBy { get; set; }

        /// <summary>Updated At - system timestamp</summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>Updated By - user name</summary>
        public string? UpdatedBy { get; set; }

        /// <summary>Deleted At - system timestamp for soft delete</summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>Row Version - for optimistic concurrency</summary>
        public int RowVersion { get; set; } = 1;

        /// <summary>Is Deleted - soft delete flag</summary>
        public bool IsDeleted { get; set; } = false;


        // Helper properties for display

        /// <summary>Masked Bank Account Number for display</summary>
        public string MaskedBankAccountNumber =>
            string.IsNullOrEmpty(BankAccountNumber) || BankAccountNumber.Length < 4
                ? "****"
                : $"****{BankAccountNumber[^4..]}";

        /// <summary>Full Address for display</summary>
        public string FullAddress
        {
            get
            {
                var parts = new List<string>();
                if (!string.IsNullOrWhiteSpace(RegisteredAddressLine1)) parts.Add(RegisteredAddressLine1);
                if (!string.IsNullOrWhiteSpace(RegisteredAddressLine2)) parts.Add(RegisteredAddressLine2);
                if (!string.IsNullOrWhiteSpace(RegisteredAddressLine3)) parts.Add(RegisteredAddressLine3);
                if (!string.IsNullOrWhiteSpace(City)) parts.Add(City);
                if (!string.IsNullOrWhiteSpace(StateName)) parts.Add(StateName);
                if (!string.IsNullOrWhiteSpace(PostalCode)) parts.Add(PostalCode);
                if (!string.IsNullOrWhiteSpace(CountryName)) parts.Add(CountryName);
                return string.Join(", ", parts);
            }
        }
    }

    #endregion

    #region Vendor-related Enums and Static Classes

    public static class VendorTypes
    {
        public const string Supplier = "Supplier";
        public const string ServiceProvider = "ServiceProvider";
        public const string Contractor = "Contractor";
        public const string Freelancer = "Freelancer";
        public const string Utility = "Utility";
        public const string Landlord = "Landlord";
        public const string Government = "Government";
        public const string Other = "Other";
        public static readonly string[] All = new[] { Supplier, ServiceProvider, Contractor, Freelancer, Utility, Landlord, Government, Other };

        public static string GetDisplayName(string type) => type switch
        {
            Supplier => "Supplier",
            ServiceProvider => "Service Provider",
            Contractor => "Contractor",
            Freelancer => "Freelancer",
            Utility => "Utility",
            Landlord => "Landlord",
            Government => "Government/Statutory",
            Other => "Other",
            _ => type
        };
    }

    public static class VendorStatuses
    {
        public const string Draft = "Draft";
        public const string Active = "Active";
        public const string Inactive = "Inactive";
        public const string OnHold = "OnHold";
        public const string Blacklisted = "Blacklisted";
        public static readonly string[] All = new[] { Draft, Active, Inactive, OnHold, Blacklisted };

        public static string GetDisplayName(string status) => status switch
        {
            Draft => "Draft",
            Active => "Active",
            Inactive => "Inactive",
            OnHold => "On Hold",
            Blacklisted => "Blacklisted",
            _ => status
        };
    }

    public static class MSMECategories
    {
        public const string Micro = "Micro";
        public const string Small = "Small";
        public const string Medium = "Medium";
        public const string NotApplicable = "NotApplicable";
        public static readonly string[] All = new[] { Micro, Small, Medium, NotApplicable };

        public static string GetDisplayName(string category) => category switch
        {
            Micro => "Micro",
            Small => "Small",
            Medium => "Medium",
            NotApplicable => "Not Applicable",
            _ => category
        };
    }

    public static class VendorResidencyTypes
    {
        public const string Resident = "Resident";
        public const string NonResident = "NonResident";
        public static readonly string[] All = new[] { Resident, NonResident };

        public static string GetDisplayName(string type) => type switch
        {
            Resident => "Resident",
            NonResident => "Non-Resident",
            _ => type
        };
    }

    public static class VendorPaymentMethods
    {
        public const string BankTransfer = "BankTransfer";
        public const string UPI = "UPI";
        public const string Cheque = "Cheque";
        public const string Cash = "Cash";
        public const string Gateway = "Gateway";
        public const string Other = "Other";
        public static readonly string[] All = new[] { BankTransfer, UPI, Cheque, Cash, Gateway, Other };

        public static string GetDisplayName(string method) => method switch
        {
            BankTransfer => "Bank Transfer (NEFT/RTGS/IMPS)",
            UPI => "UPI",
            Cheque => "Cheque",
            Cash => "Cash",
            Gateway => "Payment Gateway",
            Other => "Other",
            _ => method
        };

        /// <summary>Check if instrument reference is required for this method</summary>
        public static bool RequiresReference(string method) => method switch
        {
            BankTransfer => true,
            UPI => true,
            Cheque => true,
            Gateway => true,
            Cash => false,
            Other => false,
            _ => false
        };
    }

    #endregion

    #region Vendor Attachment

    /// <summary>
    /// Vendor Attachment - KYC/Evidence documents for vendor compliance
    /// </summary>
    public class VendorAttachmentViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid VendorId { get; set; }
        public string AttachmentType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string? FileExtension { get; set; }
        public long FileSizeBytes { get; set; }
        public string? Notes { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.Now;
        public string? UploadedBy { get; set; }
        public bool IsVerified { get; set; } = false;
        public DateTime? VerifiedAt { get; set; }
        public string? VerifiedBy { get; set; }
    }

    public static class VendorAttachmentTypes
    {
        public const string GSTCertificate = "GSTCertificate";
        public const string PANCopy = "PANCopy";
        public const string CancelledCheque = "CancelledCheque";
        public const string MSMECertificate = "MSMECertificate";
        public const string VendorAgreement = "VendorAgreement";
        public const string AddressProof = "AddressProof";
        public const string TDSDeclaration = "TDSDeclaration";
        public const string Other = "Other";

        public static readonly string[] All = new[]
        {
            GSTCertificate, PANCopy, CancelledCheque, MSMECertificate,
            VendorAgreement, AddressProof, TDSDeclaration, Other
        };

        public static string GetDisplayName(string type) => type switch
        {
            GSTCertificate => "GST Certificate",
            PANCopy => "PAN Copy",
            CancelledCheque => "Cancelled Cheque",
            MSMECertificate => "MSME Certificate",
            VendorAgreement => "Vendor Agreement",
            AddressProof => "Address Proof",
            TDSDeclaration => "TDS Declaration",
            Other => "Other Document",
            _ => type
        };

        public static string GetIcon(string type) => type switch
        {
            GSTCertificate => "ti ti-certificate",
            PANCopy => "ti ti-id",
            CancelledCheque => "ti ti-credit-card",
            MSMECertificate => "ti ti-award",
            VendorAgreement => "ti ti-file-text",
            AddressProof => "ti ti-map-pin",
            TDSDeclaration => "ti ti-file-invoice",
            Other => "ti ti-file",
            _ => "ti ti-file"
        };
    }

    #endregion
}
