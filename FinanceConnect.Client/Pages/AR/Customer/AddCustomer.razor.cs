using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using FinanceConnect.Client.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.AR.Customer
{
    public partial class AddCustomer
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject] CustomerService CustomerService { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;

        [Parameter] public Guid? Id { get; set; }

        private bool isInitialized = false;
        private CustomerViewModel Customer = CreateNewCustomer();
        private EditContext editContext = default!;

        // Track which steps have had validation attempted
        private HashSet<int> validationAttemptedSteps = new();

        // Dropdown data
        private List<CurrencyModel> Currencies = new();
        private List<PaymentTermViewModel> PaymentTerms = new();
        private List<GLAccountViewModel> GLAccounts = new();
        private List<TaxProfileViewModel> TaxProfiles = new();

        // Quill editor reference for Hold Reason
        private RichTextEditor? holdReasonEditor;

        private bool IsEdit => Id.HasValue;
        private string PageTitle => IsEdit ? "Edit Customer" : "Create Customer";
        private string PageSubTitle => IsEdit ? "Update customer details" : "Create new AR customer";

        private bool IsGSTINRequired => Customer.TaxRegistrationType == TaxRegistrationTypes.Registered ||
                                        Customer.TaxRegistrationType == TaxRegistrationTypes.SEZ ||
                                        Customer.TaxRegistrationType == TaxRegistrationTypes.Export;

        protected override void OnInitialized()
        {
            editContext = new EditContext(Customer);
        }

        private static CustomerViewModel CreateNewCustomer()
        {
            return new CustomerViewModel
            {
                CustomerStatus = "",
                CustomerType = "",
                TaxRegistrationType = "",
                CreditLimitAmount = 0,
                CreditLimitEnforced = true,
                CreditHoldStatus = "",
                DefaultPaymentMethod = "",
                SendInvoiceEmail = true,
                CustomerStatementCycle = "",
                AllowPartialPayment = true,
                AllowOverPayment = false,
                PreferredLanguage = "",
                CompanyId = Guid.Parse("c0c0c0c0-c0c0-c0c0-c0c0-c0c0c0c0c001") // Default company
            };
        }

        protected override async Task OnInitializedAsync()
        {
            // Load dropdown data
            Currencies = MasterDataService.GetAllCurrencies().Where(c => c.IsActive).ToList();

            // Payment Terms - Demo data
            PaymentTerms = new List<PaymentTermViewModel>
            {
                new PaymentTermViewModel { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Name = "Net 30 Days", Days = 30 },
                new PaymentTermViewModel { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Name = "Net 45 Days", Days = 45 },
                new PaymentTermViewModel { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), Name = "Net 60 Days", Days = 60 },
                new PaymentTermViewModel { Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), Name = "Immediate", Days = 0 },
                new PaymentTermViewModel { Id = Guid.Parse("00000000-0000-0000-0000-000000000005"), Name = "Net 15 Days", Days = 15 }
            };

            // GL Accounts - Demo data
            GLAccounts = new List<GLAccountViewModel>
            {
                new GLAccountViewModel { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Code = "1100", Name = "Accounts Receivable" },
                new GLAccountViewModel { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Code = "2100", Name = "Customer Advances" },
                new GLAccountViewModel { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), Code = "6500", Name = "Bad Debts Write-Off" },
                new GLAccountViewModel { Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), Code = "1110", Name = "AR - Export" },
                new GLAccountViewModel { Id = Guid.Parse("00000000-0000-0000-0000-000000000005"), Code = "2110", Name = "Advances - Export" }
            };

            // Tax Profiles - Demo data
            TaxProfiles = new List<TaxProfileViewModel>
            {
                new TaxProfileViewModel { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Name = "Standard GST" },
                new TaxProfileViewModel { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Name = "SEZ Zero Rated" },
                new TaxProfileViewModel { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), Name = "Export Zero Rated" },
                new TaxProfileViewModel { Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), Name = "Composition Scheme" },
                new TaxProfileViewModel { Id = Guid.Parse("00000000-0000-0000-0000-000000000005"), Name = "Exempt" }
            };

            if (IsEdit)
            {
                var existing = CustomerService.GetById(Id!.Value);
                if (existing != null)
                {
                    Customer = new CustomerViewModel
                    {
                        Id = existing.Id,
                        CompanyId = existing.CompanyId,
                        CustomerCode = existing.CustomerCode,
                        CustomerName = existing.CustomerName,
                        CustomerDisplayName = existing.CustomerDisplayName,
                        CustomerType = existing.CustomerType,
                        CustomerStatus = existing.CustomerStatus,
                        PrimaryEmail = existing.PrimaryEmail,
                        PrimaryPhone = existing.PrimaryPhone,
                        SecondaryPhone = existing.SecondaryPhone,
                        BillingEmail = existing.BillingEmail,
                        ContactPersonName = existing.ContactPersonName,
                        Website = existing.Website,
                        TaxRegistrationType = existing.TaxRegistrationType,
                        GSTIN = existing.GSTIN,
                        PAN = existing.PAN,
                        TAN = existing.TAN,
                        TaxProfileId = existing.TaxProfileId,
                        TaxProfileName = existing.TaxProfileName,
                        IsTDSApplicable = existing.IsTDSApplicable,
                        TDSSectionCode = existing.TDSSectionCode,
                        CreditLimitAmount = existing.CreditLimitAmount,
                        CreditLimitEnforced = existing.CreditLimitEnforced,
                        CreditHoldStatus = existing.CreditHoldStatus,
                        CreditHoldReason = existing.CreditHoldReason,
                        PaymentTermId = existing.PaymentTermId,
                        PaymentTermName = existing.PaymentTermName,
                        DefaultCurrencyId = existing.DefaultCurrencyId,
                        DefaultCurrencyCode = existing.DefaultCurrencyCode,
                        DefaultPaymentMethod = existing.DefaultPaymentMethod,
                        ReceivableAccountId = existing.ReceivableAccountId,
                        ReceivableAccountCode = existing.ReceivableAccountCode,
                        ReceivableAccountName = existing.ReceivableAccountName,
                        AdvanceFromCustomerAccountId = existing.AdvanceFromCustomerAccountId,
                        WriteOffAccountId = existing.WriteOffAccountId,
                        AllowAutoAdvanceCreation = existing.AllowAutoAdvanceCreation,
                        SendInvoiceEmail = existing.SendInvoiceEmail,
                        CustomerStatementCycle = existing.CustomerStatementCycle,
                        PreferredLanguage = existing.PreferredLanguage,
                        AllowPartialPayment = existing.AllowPartialPayment,
                        AllowOverPayment = existing.AllowOverPayment
                    };
                }
                else
                {
                    Nav.NavigateTo("/customers");
                    return;
                }
            }
            else
            {
                // New customer - keep empty defaults to force user selection via placeholders
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

        private void OnTaxRegistrationTypeChange(ChangeEventArgs e)
        {
            Customer.TaxRegistrationType = e.Value?.ToString() ?? "";
            if (!IsGSTINRequired)
            {
                // Clear GSTIN if not required
            }
        }

        private void OnCreditHoldStatusChange(ChangeEventArgs e)
        {
            Customer.CreditHoldStatus = e.Value?.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(Customer.CreditHoldStatus) || Customer.CreditHoldStatus == CreditHoldStatuses.None)
            {
                Customer.CreditHoldReason = null;
            }
        }

        void OnCustomerCodeChanged()
        {
            Customer.CustomerCode = Customer.CustomerCode?.Trim() ?? "";
        }

        void OnCustomerNameChanged()
        {
            Customer.CustomerName = Customer.CustomerName?.Trim() ?? "";
        }



        private async Task SyncHoldReasonFromEditor()
        {
            if (holdReasonEditor != null && !string.IsNullOrWhiteSpace(Customer.CreditHoldStatus) && Customer.CreditHoldStatus != CreditHoldStatuses.None)
            {
                var html = await holdReasonEditor.GetHtmlAsync();
                // Quill returns <p><br></p> for empty content
                if (html == "<p><br></p>" || html == "<p></p>")
                    Customer.CreditHoldReason = null;
                else
                    Customer.CreditHoldReason = html;
            }
        }

        private async Task Save()
        {
            // Sync Hold Reason from Quill editor
            await SyncHoldReasonFromEditor();

            // Validate all steps
            validationAttemptedSteps = new HashSet<int> { 1, 2, 3, 4, 5 };

            for (int step = 1; step <= Steps.Count; step++)
            {
                CurrentStep = step;
                if (!IsCurrentStepValid())
                {
                    StateHasChanged();
                    return;
                }
            }
            if (string.IsNullOrWhiteSpace(Customer.CustomerCode))
            {
                ToastService.ShowError("Company Code is required", "Validation Error");
                CurrentStep = 1;
                validationAttemptedSteps.Add(1);
                return;
            }

            // Trim CompanyCode
            Customer.CustomerCode = Customer.CustomerCode.Trim();

            // Validate CompanyCode: only letters, numbers, _ and -
            if (!System.Text.RegularExpressions.Regex.IsMatch(Customer.CustomerCode, @"^[A-Za-z0-9_\-]+$"))
            {
                ToastService.ShowError("Company Code can only contain letters, numbers, underscore (_) and hyphen (-)", "Validation Error");
                CurrentStep = 1;
                validationAttemptedSteps.Add(1);
                return;
            }
            if (!IsValidEmail(Customer.PrimaryEmail))
            {
                ToastService.ShowError("Invalid email format", "Validation Error");
                CurrentStep = 1;
                validationAttemptedSteps.Add(1);
                return;
            }

            if (!IsValidPhone(Customer.PrimaryPhone))
            {
                ToastService.ShowError("Phone can only contain digits, +, -, spaces and parentheses", "Validation Error");
                CurrentStep = 1;
                validationAttemptedSteps.Add(1);
                return;
            }

            if (!IsValidUrl(Customer.Website))
            {
                ToastService.ShowError("Invalid URL format (must start with http:// or https://)", "Validation Error");
                CurrentStep = 1;
                validationAttemptedSteps.Add(1);
                return;
            }

            // Set lookup values
            var currency = Currencies.FirstOrDefault(c => c.Id == Customer.DefaultCurrencyId);
            var paymentTerm = PaymentTerms.FirstOrDefault(p => p.Id == Customer.PaymentTermId);
            var receivableAccount = GLAccounts.FirstOrDefault(a => a.Id == Customer.ReceivableAccountId);
            var advanceAccount = GLAccounts.FirstOrDefault(a => a.Id == Customer.AdvanceFromCustomerAccountId);
            var writeOffAccount = GLAccounts.FirstOrDefault(a => a.Id == Customer.WriteOffAccountId);
            var taxProfile = TaxProfiles.FirstOrDefault(t => t.Id == Customer.TaxProfileId);

            Customer.DefaultCurrencyCode = currency?.CurrencyCode;
            Customer.DefaultCurrencyName = currency?.CurrencyName;
            Customer.PaymentTermName = paymentTerm?.Name;
            Customer.ReceivableAccountCode = receivableAccount?.Code;
            Customer.ReceivableAccountName = receivableAccount?.Name;
            Customer.AdvanceFromCustomerAccountCode = advanceAccount?.Code;
            Customer.AdvanceFromCustomerAccountName = advanceAccount?.Name;
            Customer.WriteOffAccountCode = writeOffAccount?.Code;
            Customer.WriteOffAccountName = writeOffAccount?.Name;
            Customer.TaxProfileName = taxProfile?.Name;

            if (IsEdit)
            {
                Customer.UpdatedAt = DateTime.Now;
                Customer.UpdatedBy = "Current User";
                var result = CustomerService.Update(Customer);
                if (result.Success)
                {
                    ToastService.ShowSuccess($"Customer '{Customer.CustomerName}' updated successfully", "Updated");
                    Nav.NavigateTo("/customers");
                }
                else
                {
                    ToastService.ShowError(result.Message, "Validation Error");
                }
            }
            else
            {
                Customer.CreatedAt = DateTime.Now;
                Customer.CreatedBy = "Current User";
                var result = CustomerService.Add(Customer);
                if (result.Success)
                {
                    ToastService.ShowSuccess($"Customer '{Customer.CustomerName}' added successfully", "Added");
                    Nav.NavigateTo("/customers");
                }
                else
                {
                    ToastService.ShowError(result.Message, "Validation Error");
                }
            }
        }

        // Wizard step management
        private int CurrentStep = 1;

        private List<WizardStep> Steps = new()
        {
            new("Customer Identity", "Basic Info", "ti ti-user"),
            new("Tax & Compliance", "GST & PAN", "ti ti-file-certificate"),
            new("Credit & Terms", "Credit Control", "ti ti-credit-card"),
            new("Accounting Defaults", "GL Mapping", "ti ti-calculator"),
            new("Preferences", "Settings", "ti ti-settings")
        };

        private string IndicatorToppx => $"{40 + ((CurrentStep - 1) * 76)}px";

        protected async Task ScrollToCurrentStep()
        {
            await JS.InvokeVoidAsync("scrollToStep", CurrentStep - 1);
        }

        private async Task Next()
        {
            // Sync Hold Reason from Quill editor before validation
            await SyncHoldReasonFromEditor();

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
                // Step 1
                "CustomerCode" => string.IsNullOrWhiteSpace(Customer.CustomerCode),
                "CustomerCodeFormat" => !string.IsNullOrWhiteSpace(Customer.CustomerCode) && !System.Text.RegularExpressions.Regex.IsMatch(Customer.CustomerCode.Trim(), @"^[A-Za-z0-9_\-]+$"),
                "CustomerName" => string.IsNullOrWhiteSpace(Customer.CustomerName),
                "CustomerType" => string.IsNullOrWhiteSpace(Customer.CustomerType),
                "CustomerStatus" => string.IsNullOrWhiteSpace(Customer.CustomerStatus),
                "PrimaryEmail" => !string.IsNullOrWhiteSpace(Customer.PrimaryEmail) && !IsValidEmail(Customer.PrimaryEmail),
                "PrimaryPhone" => !string.IsNullOrWhiteSpace(Customer.PrimaryPhone) && !IsValidPhone(Customer.PrimaryPhone),
                "Website" => !string.IsNullOrWhiteSpace(Customer.Website) && !IsValidUrl(Customer.Website),

                // Step 2
                "TaxRegistrationType" => string.IsNullOrWhiteSpace(Customer.TaxRegistrationType),
                "GSTIN" => IsGSTINRequired && string.IsNullOrWhiteSpace(Customer.GSTIN),
                "TaxProfileId" => !Customer.TaxProfileId.HasValue || Customer.TaxProfileId == Guid.Empty,

                // Step 3
                "CreditHoldStatus" => string.IsNullOrWhiteSpace(Customer.CreditHoldStatus),
                "CreditHoldReason" => !string.IsNullOrWhiteSpace(Customer.CreditHoldStatus) && Customer.CreditHoldStatus != CreditHoldStatuses.None && string.IsNullOrWhiteSpace(Customer.CreditHoldReason),
                "PaymentTermId" => !Customer.PaymentTermId.HasValue || Customer.PaymentTermId == Guid.Empty,
                "DefaultCurrencyId" => !Customer.DefaultCurrencyId.HasValue || Customer.DefaultCurrencyId == Guid.Empty,
                "DefaultPaymentMethod" => string.IsNullOrWhiteSpace(Customer.DefaultPaymentMethod),

                // Step 4
                "ReceivableAccountId" => !Customer.ReceivableAccountId.HasValue || Customer.ReceivableAccountId == Guid.Empty,

                // Step 5
                "BillingEmail" => !string.IsNullOrWhiteSpace(Customer.BillingEmail) && !IsValidEmail(Customer.BillingEmail),
                "SecondaryPhone" => !string.IsNullOrWhiteSpace(Customer.SecondaryPhone) && !IsValidPhone(Customer.SecondaryPhone),

                _ => false
            };
        }

        private bool IsCurrentStepValid()
        {
            return CurrentStep switch
            {
                1 => !string.IsNullOrWhiteSpace(Customer.CustomerCode)
                && System.Text.RegularExpressions.Regex.IsMatch(Customer.CustomerCode.Trim(), @"^[A-Za-z0-9_\-]+$")
                     && !string.IsNullOrWhiteSpace(Customer.CustomerName)
                     && !string.IsNullOrWhiteSpace(Customer.CustomerType)
                     && !string.IsNullOrWhiteSpace(Customer.CustomerStatus)
                     &&IsValidEmail(Customer.PrimaryEmail)
                     && IsValidPhone(Customer.PrimaryPhone)
                     && IsValidUrl(Customer.Website),
                2 => !string.IsNullOrWhiteSpace(Customer.TaxRegistrationType)
                     && (!IsGSTINRequired || !string.IsNullOrWhiteSpace(Customer.GSTIN))
                     && Customer.TaxProfileId.HasValue && Customer.TaxProfileId != Guid.Empty,
                3 => !string.IsNullOrWhiteSpace(Customer.CreditHoldStatus)
                     && Customer.PaymentTermId.HasValue && Customer.PaymentTermId != Guid.Empty
                     && Customer.DefaultCurrencyId.HasValue && Customer.DefaultCurrencyId != Guid.Empty
                     && !string.IsNullOrWhiteSpace(Customer.DefaultPaymentMethod)
                     && (Customer.CreditHoldStatus == CreditHoldStatuses.None || !string.IsNullOrWhiteSpace(Customer.CreditHoldReason)),
                4 => Customer.ReceivableAccountId.HasValue && Customer.ReceivableAccountId != Guid.Empty,
                5 => IsValidEmail(Customer.BillingEmail)
                     && IsValidPhone(Customer.SecondaryPhone), // All optional
                _ => true
            };
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return true;
            return System.Text.RegularExpressions.Regex.IsMatch(email.Trim(),
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private static bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return true;
            return System.Text.RegularExpressions.Regex.IsMatch(phone.Trim(),
                @"^[\d\+\-\s\(\)]+$");
        }

        private static bool IsValidUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return true;
            return Uri.TryCreate(url.Trim(), UriKind.Absolute, out var result)
                   && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
        }

        private string StepClass(int i)
        {
            if (i < CurrentStep) return "done";
            if (i == CurrentStep) return "active";
            return "";
        }

        public record WizardStep(string Title, string Description, string Icon);
    }
}
