using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using FinanceConnect.Client.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using FinanceConnect.Client.Shared;

namespace FinanceConnect.Client.Pages.AP.Vendor
{
    public partial class AddVendor
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject] VendorService VendorService { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] COADataService COADataService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] AuthService AuthService { get; set; } = default!;

        [Parameter] public Guid? Id { get; set; }

        private bool isInitialized = false;
        private RichTextEditor? notesEditor;
        private RichTextEditor? attachmentNotesEditor;
        private VendorViewModel Vendor = CreateNewVendor();
        private EditContext editContext = default!;

        private HashSet<int> validationAttemptedSteps = new();

        private List<CurrencyModel> Currencies = new();
        private List<PaymentTermViewModel> PaymentTerms = new();
        private List<AccountViewModel> GLAccounts = new();
        private List<StateProvinceModel> States = new();
        private List<CountryModel> Countries = new();
        private List<VendorCategoryDto> VendorCategories = new();
        private List<TaxProfileViewModel> TaxProfiles = new();

        // Attachment form fields
        private string NewAttachmentType = string.Empty;
        private string NewAttachmentNotes = string.Empty;
        private bool attachmentFormValidationAttempted = false;
        
        // File upload properties
        private IBrowserFile? selectedFile = null;
        private string selectedFileName = string.Empty;
        private long selectedFileSize = 0;
        private string fileValidationMessage = string.Empty;
        private bool isUploadingFile = false;
        private bool isFileValid = false;
        
        // Allowed file types and max size
        private static readonly string[] AllowedExtensions = { ".pdf", ".jpg", ".jpeg", ".png" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

        private bool IsEdit => Id.HasValue;
        private string PageTitle => IsEdit ? "Edit Vendor" : "Create Vendor";
        private string PageSubTitle => IsEdit ? "Update vendor details" : "Create new AP vendor";
        private string fileInputKey = Guid.NewGuid().ToString();

        protected override void OnInitialized()
        {
            editContext = new EditContext(Vendor);
        }

        private static VendorViewModel CreateNewVendor()
        {
            return new VendorViewModel
            {
                VendorStatus = VendorStatuses.Draft,
                VendorType = string.Empty,
                IsGSTRegistered = false,
                IsTDSApplicable = false,
                VendorResidencyType = VendorResidencyTypes.Resident,
                IsPaymentBlocked = false,
                IsBillPostingBlocked = false,
                CompanyId = Guid.Parse("c0c0c0c0-c0c0-c0c0-c0c0-c0c0c0c0c001")
            };
        }

        protected override async Task OnInitializedAsync()
        {
            Currencies = MasterDataService.GetAllCurrencies().Where(c => c.IsActive).ToList();

            PaymentTerms = PaymentTermSeedData.GetSeedData();

            GLAccounts = COADataService.GetAllAccounts();

            States = MasterDataService.GetAllStateProvinces();

            Countries = MasterDataService.GetAllCountries();

            // Vendor Categories (optional field)
            VendorCategories = new List<VendorCategoryDto>
            {
                new VendorCategoryDto { Id = Guid.Parse("00000000-0000-0000-0000-000000000101"), Name = "Raw Materials" },
                new VendorCategoryDto { Id = Guid.Parse("00000000-0000-0000-0000-000000000102"), Name = "Consumables" },
                new VendorCategoryDto { Id = Guid.Parse("00000000-0000-0000-0000-000000000103"), Name = "Services" },
                new VendorCategoryDto { Id = Guid.Parse("00000000-0000-0000-0000-000000000104"), Name = "Capital Goods" },
                new VendorCategoryDto { Id = Guid.Parse("00000000-0000-0000-0000-000000000105"), Name = "Utilities" },
                new VendorCategoryDto { Id = Guid.Parse("00000000-0000-0000-0000-000000000106"), Name = "IT & Software" },
                new VendorCategoryDto { Id = Guid.Parse("00000000-0000-0000-0000-000000000107"), Name = "Professional Services" }
            };

            // Tax Profiles (optional field)
            TaxProfiles = new List<TaxProfileViewModel>
            {
                new TaxProfileViewModel { Id = Guid.Parse("00000000-0000-0000-0000-000000000201"), Name = "GST 18% - Standard" },
                new TaxProfileViewModel { Id = Guid.Parse("00000000-0000-0000-0000-000000000202"), Name = "GST 12% - Reduced" },
                new TaxProfileViewModel { Id = Guid.Parse("00000000-0000-0000-0000-000000000203"), Name = "GST 5% - Essential" },
                new TaxProfileViewModel { Id = Guid.Parse("00000000-0000-0000-0000-000000000204"), Name = "GST 28% - Luxury" },
                new TaxProfileViewModel { Id = Guid.Parse("00000000-0000-0000-0000-000000000205"), Name = "GST Exempt" },
                new TaxProfileViewModel { Id = Guid.Parse("00000000-0000-0000-0000-000000000206"), Name = "Zero Rated" }
            };

            if (IsEdit)
            {
                var existing = VendorService.GetById(Id!.Value);
                if (existing != null)
                {
                    Vendor = new VendorViewModel
                    {
                        Id = existing.Id,
                        CompanyId = existing.CompanyId,
                        VendorCode = existing.VendorCode,
                        VendorName = existing.VendorName,
                        LegalName = existing.LegalName,
                        VendorType = existing.VendorType,
                        VendorCategoryId = existing.VendorCategoryId,
                        VendorCategoryName = existing.VendorCategoryName,
                        VendorStatus = existing.VendorStatus,
                        HoldReason = existing.HoldReason,
                        BlacklistReason = existing.BlacklistReason,
                        Notes = existing.Notes,
                        RegisteredAddressLine1 = existing.RegisteredAddressLine1,
                        RegisteredAddressLine2 = existing.RegisteredAddressLine2,
                        RegisteredAddressLine3 = existing.RegisteredAddressLine3,
                        City = existing.City,
                        StateId = existing.StateId,
                        StateName = existing.StateName,
                        CountryId = existing.CountryId,
                        CountryName = existing.CountryName,
                        PostalCode = existing.PostalCode,
                        PrimaryContactName = existing.PrimaryContactName,
                        PrimaryEmail = existing.PrimaryEmail,
                        PrimaryPhone = existing.PrimaryPhone,
                        AlternatePhone = existing.AlternatePhone,
                        RemittanceEmail = existing.RemittanceEmail,
                        IsGSTRegistered = existing.IsGSTRegistered,
                        GSTIN = existing.GSTIN,
                        PAN = existing.PAN,
                        VendorGSTStateId = existing.VendorGSTStateId,
                        VendorGSTStateName = existing.VendorGSTStateName,
                        MSMECategory = existing.MSMECategory,
                        IsTDSApplicable = existing.IsTDSApplicable,
                        TDSSectionCode = existing.TDSSectionCode,
                        TDSRatePercent = existing.TDSRatePercent,
                        VendorResidencyType = existing.VendorResidencyType,
                        PreferredPaymentMethod = existing.PreferredPaymentMethod,
                        BankAccountName = existing.BankAccountName,
                        BankAccountNumber = existing.BankAccountNumber,
                        IFSC = existing.IFSC,
                        BankName = existing.BankName,
                        BranchName = existing.BranchName,
                        UPIId = existing.UPIId,
                        IsBankVerified = existing.IsBankVerified,
                        BankVerifiedOn = existing.BankVerifiedOn,
                        BankVerifiedByUserId = existing.BankVerifiedByUserId,
                        BankVerifiedByUserName = existing.BankVerifiedByUserName,
                        DefaultCurrencyId = existing.DefaultCurrencyId,
                        DefaultCurrencyCode = existing.DefaultCurrencyCode,
                        DefaultCurrencyName = existing.DefaultCurrencyName,
                        PaymentTermsId = existing.PaymentTermsId,
                        PaymentTermsName = existing.PaymentTermsName,
                        DefaultPayableAccountId = existing.DefaultPayableAccountId,
                        DefaultPayableAccountCode = existing.DefaultPayableAccountCode,
                        DefaultPayableAccountName = existing.DefaultPayableAccountName,
                        AdvanceToVendorAccountId = existing.AdvanceToVendorAccountId,
                        AdvanceToVendorAccountCode = existing.AdvanceToVendorAccountCode,
                        AdvanceToVendorAccountName = existing.AdvanceToVendorAccountName,
                        DefaultExpenseAccountId = existing.DefaultExpenseAccountId,
                        DefaultExpenseAccountCode = existing.DefaultExpenseAccountCode,
                        DefaultExpenseAccountName = existing.DefaultExpenseAccountName,
                        DefaultTaxProfileId = existing.DefaultTaxProfileId,
                        DefaultTaxProfileName = existing.DefaultTaxProfileName,
                        IsPaymentBlocked = existing.IsPaymentBlocked,
                        PaymentBlockReason = existing.PaymentBlockReason,
                        IsBillPostingBlocked = existing.IsBillPostingBlocked,
                        BillPostingBlockReason = existing.BillPostingBlockReason,
                        HasAttachments = existing.HasAttachments,
                        AttachmentCount = existing.AttachmentCount,
                        Attachments = existing.Attachments ?? new List<VendorAttachmentViewModel>()
                    };
                }
                else
                {
                    Nav.NavigateTo("/vendors");
                    return;
                }
            }

            isInitialized = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("feather.replace");
            }
        }

        private void OnStatusChange(ChangeEventArgs e)
        {
            if (Vendor.VendorStatus != VendorStatuses.OnHold)
                Vendor.HoldReason = null;
            if (Vendor.VendorStatus != VendorStatuses.Blacklisted)
                Vendor.BlacklistReason = null;
        }

        private async Task Save()
        {
            // Get notes from rich text editor
            if (notesEditor != null)
                Vendor.Notes = await notesEditor.GetHtmlAsync();

            validationAttemptedSteps = new HashSet<int> { 1, 2, 3, 4, 5, 6 };

            for (int step = 1; step <= Steps.Count; step++)
            {
                CurrentStep = step;
                if (!IsCurrentStepValid())
                {
                    StateHasChanged();
                    return;
                }
            }

            var currency = Currencies.FirstOrDefault(c => c.Id == Vendor.DefaultCurrencyId);
            var paymentTerm = PaymentTerms.FirstOrDefault(p => p.Id == Vendor.PaymentTermsId);
            var payableAccount = GLAccounts.FirstOrDefault(a => a.Id == Vendor.DefaultPayableAccountId);
            var advanceAccount = GLAccounts.FirstOrDefault(a => a.Id == Vendor.AdvanceToVendorAccountId);
            var expenseAccount = GLAccounts.FirstOrDefault(a => a.Id == Vendor.DefaultExpenseAccountId);
            var state = States.FirstOrDefault(s => s.Id == Vendor.StateId);
            var country = Countries.FirstOrDefault(c => c.Id == Vendor.CountryId);
            var gstState = States.FirstOrDefault(s => s.Id == Vendor.VendorGSTStateId);
            var vendorCategory = VendorCategories.FirstOrDefault(vc => vc.Id == Vendor.VendorCategoryId);
            var taxProfile = TaxProfiles.FirstOrDefault(tp => tp.Id == Vendor.DefaultTaxProfileId);

            Vendor.DefaultCurrencyCode = currency?.CurrencyCode;
            Vendor.DefaultCurrencyName = currency?.CurrencyName;
            Vendor.PaymentTermsName = paymentTerm?.Name;
            Vendor.DefaultPayableAccountCode = payableAccount?.AccountCode;
            Vendor.DefaultPayableAccountName = payableAccount?.AccountName;
            Vendor.AdvanceToVendorAccountCode = advanceAccount?.AccountCode;
            Vendor.AdvanceToVendorAccountName = advanceAccount?.AccountName;
            Vendor.DefaultExpenseAccountCode = expenseAccount?.AccountCode;
            Vendor.DefaultExpenseAccountName = expenseAccount?.AccountName;
            Vendor.StateName = state?.StateProvinceName;
            Vendor.CountryName = country?.CountryName;
            Vendor.VendorGSTStateName = gstState?.StateProvinceName;
            Vendor.VendorCategoryName = vendorCategory?.Name;
            Vendor.DefaultTaxProfileName = taxProfile?.Name;

            // Update attachment counts
            Vendor.HasAttachments = Vendor.Attachments.Any();
            Vendor.AttachmentCount = Vendor.Attachments.Count;

            if (IsEdit)
            {
                Vendor.UpdatedAt = DateTime.Now;
                Vendor.UpdatedBy = AuthService.CurrentUser?.UserName ?? "System";
                var result = VendorService.Update(Vendor);
                if (result.Success)
                {
                    ToastService.ShowSuccess($"Vendor '{Vendor.VendorName}' updated successfully", "Updated");
                    Nav.NavigateTo("/vendors");
                }
                else
                {
                    ToastService.ShowError(result.Message, "Validation Error");
                }
            }
            else
            {
                Vendor.CreatedAt = DateTime.Now;
                Vendor.CreatedBy = AuthService.CurrentUser?.UserName ?? "System";
                var result = VendorService.Add(Vendor);
                if (result.Success)
                {
                    ToastService.ShowSuccess($"Vendor '{Vendor.VendorName}' added successfully", "Added");
                    Nav.NavigateTo("/vendors");
                }
                else
                {
                    ToastService.ShowError(result.Message, "Validation Error");
                }
            }
        }

        private int CurrentStep = 1;

        private List<WizardStep> Steps = new()
        {
            new("Vendor Identity", "Basic Info", "ti ti-building"),
            new("Address & Contacts", "Location", "ti ti-map-pin"),
            new("Compliance", "GST/PAN/TDS", "ti ti-file-certificate"),
            new("Payment & Banking", "Bank Details", "ti ti-credit-card"),
            new("AP Defaults", "GL Mapping", "ti ti-calculator"),
            new("Attachments", "KYC/Evidence", "ti ti-paperclip")
        };

        private string IndicatorToppx => $"{40 + ((CurrentStep - 1) * 76)}px";

        protected async Task ScrollToCurrentStep()
        {
            await JS.InvokeVoidAsync("scrollToStep", CurrentStep - 1);
        }

        private async Task Next()
        {
            validationAttemptedSteps.Add(CurrentStep);

            if (!IsCurrentStepValid())
            {
                StateHasChanged();
                return;
            }

            if (CurrentStep < Steps.Count) CurrentStep++;
            await ScrollToCurrentStep();
        }

        private void Back()
        {
            if (CurrentStep > 1) CurrentStep--;
        }

        private bool ShowFieldError(int step, string fieldName)
        {
            if (!validationAttemptedSteps.Contains(step))
                return false;

            return fieldName switch
            {
                "VendorCode" => string.IsNullOrWhiteSpace(Vendor.VendorCode),
                "VendorName" => string.IsNullOrWhiteSpace(Vendor.VendorName),
                "VendorType" => string.IsNullOrWhiteSpace(Vendor.VendorType),
                "HoldReason" => Vendor.VendorStatus == VendorStatuses.OnHold && string.IsNullOrWhiteSpace(Vendor.HoldReason),
                "BlacklistReason" => Vendor.VendorStatus == VendorStatuses.Blacklisted && string.IsNullOrWhiteSpace(Vendor.BlacklistReason),
                "CountryId" => !Vendor.CountryId.HasValue || Vendor.CountryId == Guid.Empty,
                "GSTIN" => Vendor.IsGSTRegistered && string.IsNullOrWhiteSpace(Vendor.GSTIN),
                "PAN" => Vendor.IsTDSApplicable && string.IsNullOrWhiteSpace(Vendor.PAN),
                "TDSSectionCode" => Vendor.IsTDSApplicable && string.IsNullOrWhiteSpace(Vendor.TDSSectionCode),
                "DefaultCurrencyId" => !Vendor.DefaultCurrencyId.HasValue || Vendor.DefaultCurrencyId == Guid.Empty,
                "DefaultPayableAccountId" => !Vendor.DefaultPayableAccountId.HasValue || Vendor.DefaultPayableAccountId == Guid.Empty,
                "PaymentBlockReason" => Vendor.IsPaymentBlocked && string.IsNullOrWhiteSpace(Vendor.PaymentBlockReason),
                "BillPostingBlockReason" => Vendor.IsBillPostingBlocked && string.IsNullOrWhiteSpace(Vendor.BillPostingBlockReason),
                "NewAttachmentType" => attachmentFormValidationAttempted && string.IsNullOrWhiteSpace(NewAttachmentType),
                "NewAttachmentFile" => attachmentFormValidationAttempted && (!isFileValid || selectedFile == null),
                _ => false
            };
        }

        private bool IsCurrentStepValid()
        {
            return CurrentStep switch
            {
                1 => !string.IsNullOrWhiteSpace(Vendor.VendorCode)
                     && !string.IsNullOrWhiteSpace(Vendor.VendorName)
                     && !string.IsNullOrWhiteSpace(Vendor.VendorType)
                     && (Vendor.VendorStatus != VendorStatuses.OnHold || !string.IsNullOrWhiteSpace(Vendor.HoldReason))
                     && (Vendor.VendorStatus != VendorStatuses.Blacklisted || !string.IsNullOrWhiteSpace(Vendor.BlacklistReason)),
                2 => Vendor.CountryId.HasValue && Vendor.CountryId != Guid.Empty,
                3 => (!Vendor.IsGSTRegistered || !string.IsNullOrWhiteSpace(Vendor.GSTIN))
                     && (!Vendor.IsTDSApplicable || (!string.IsNullOrWhiteSpace(Vendor.PAN) && !string.IsNullOrWhiteSpace(Vendor.TDSSectionCode))),
                4 => true,
                5 => Vendor.DefaultCurrencyId.HasValue && Vendor.DefaultCurrencyId != Guid.Empty
                     && Vendor.DefaultPayableAccountId.HasValue && Vendor.DefaultPayableAccountId != Guid.Empty
                     && (!Vendor.IsPaymentBlocked || !string.IsNullOrWhiteSpace(Vendor.PaymentBlockReason))
                     && (!Vendor.IsBillPostingBlocked || !string.IsNullOrWhiteSpace(Vendor.BillPostingBlockReason)),
                _ => true
            };
        }

        private string StepClass(int i)
        {
            if (i < CurrentStep) return "done";
            if (i == CurrentStep) return "active";
            return "";
        }

        void OnVendorCodeChanged()
        {
            Vendor.VendorCode = Vendor.VendorCode?.Trim() ?? "";
        }

        void OnVendorNameChanged()
        {
            Vendor.VendorName = Vendor.VendorName?.Trim() ?? "";
        }

        // Attachment methods
        private void OnFileSelected(InputFileChangeEventArgs e)
        {
            selectedFile = e.File;
            selectedFileName = string.Empty;
            selectedFileSize = 0;
            isFileValid = false;
            fileValidationMessage = string.Empty;

            if (selectedFile == null)
            {
                fileValidationMessage = "Please select a file";
                return;
            }

            // Validate file extension
            var extension = Path.GetExtension(selectedFile.Name).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
            {
                fileValidationMessage = $"Invalid file type. Allowed: PDF, JPG, JPEG, PNG";
                selectedFile = null;
                return;
            }

            // Validate file size
            if (selectedFile.Size > MaxFileSize)
            {
                fileValidationMessage = $"File size exceeds 5MB limit. Current: {FormatFileSize(selectedFile.Size)}";
                selectedFile = null;
                return;
            }

            // File is valid
            selectedFileName = selectedFile.Name;
            selectedFileSize = selectedFile.Size;
            isFileValid = true;
            StateHasChanged();
        }

        private async Task AddAttachment()
        {
            // Get attachment notes from rich text editor
            if (attachmentNotesEditor != null)
                NewAttachmentNotes = await attachmentNotesEditor.GetHtmlAsync();

            attachmentFormValidationAttempted = true;
            validationAttemptedSteps.Add(6);


            if (string.IsNullOrWhiteSpace(NewAttachmentType) || !isFileValid || selectedFile == null)
            {
                if (selectedFile == null || !isFileValid)
                {
                    fileValidationMessage = string.IsNullOrEmpty(fileValidationMessage) ? "Please select a valid file" : fileValidationMessage;
                }
                StateHasChanged();
                return;
            }

            isUploadingFile = true;
            StateHasChanged();

            try
            {
                // In a real application, you would upload the file to a server here
                // For demo purposes, we're just storing the metadata
                await Task.Delay(500); // Simulate upload delay

                var extension = Path.GetExtension(selectedFile.Name).ToLowerInvariant();
                
                var attachment = new VendorAttachmentViewModel
                {
                    Id = Guid.NewGuid(),
                    VendorId = Vendor.Id,
                    AttachmentType = NewAttachmentType,
                    FileName = selectedFile.Name,
                    FileExtension = extension,
                    FileSizeBytes = selectedFile.Size,
                    Notes = NewAttachmentNotes,
                    UploadedAt = DateTime.Now,
                    UploadedBy = AuthService.CurrentUser?.UserName ?? "System"
                };

                Vendor.Attachments.Add(attachment);

                // Reset form
                NewAttachmentType = string.Empty;
                NewAttachmentNotes = string.Empty;
                selectedFile = null;
                selectedFileName = string.Empty;
                selectedFileSize = 0;
                isFileValid = false;
                fileValidationMessage = string.Empty;
                attachmentFormValidationAttempted = false;
                fileInputKey = Guid.NewGuid().ToString();

                ToastService.ShowSuccess("Attachment added successfully", "Added");
            }
            catch (Exception ex)
            {
                ToastService.ShowError($"Failed to add attachment: {ex.Message}", "Error");
            }
            finally
            {
                isUploadingFile = false;
                StateHasChanged();
            }
        }

        private static string FormatFileSize(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:N1} KB";
            return $"{bytes / (1024.0 * 1024.0):N2} MB";
        }

        private void RemoveAttachment(Guid attachmentId)
        {
            var attachment = Vendor.Attachments.FirstOrDefault(a => a.Id == attachmentId);
            if (attachment != null)
            {
                Vendor.Attachments.Remove(attachment);
                ToastService.ShowInfo("Attachment removed", "Removed");
                StateHasChanged();
            }
        }

        public record WizardStep(string Title, string Description, string Icon);

        public class VendorCategoryDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        public class TaxProfileViewModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }
    }
}
