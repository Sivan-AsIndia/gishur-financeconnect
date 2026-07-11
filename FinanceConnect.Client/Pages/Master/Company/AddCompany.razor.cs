using FinanceConnect.Client.Services;
using FinanceConnect.Client.Shared;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using static FinanceConnect.Client.Data.MasterDataIds;

namespace FinanceConnect.Client.Pages.Master.Company
{
    public partial class AddCompany
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] BranchService BranchService { get; set; } = default!;

        [Parameter] public Guid? Id { get; set; }

        private bool isInitialized = false;
        private bool IsInitializing = false;

        private CompanyModel Company = CreateNewCompany();
        private EditContext editContext;
        RichTextEditor? _notesEditor;

        // Track which steps have had validation attempted (user clicked Next)
        private HashSet<int> validationAttemptedSteps = new();

        private List<CountryModel> Countries = new();
        private List<StateProvinceModel> States = new();
        private List<CityModel> Cities = new();
        private List<TimeZoneModel> TimeZones = new();
        private List<CurrencyModel> Currencies = new();

        // Cascading dropdown lists
        private List<StateProvinceModel> RegistrationStates = new();
        private List<CityModel> RegistrationCities = new();
        private List<StateProvinceModel> AddressStates = new();
        private List<CityModel> AddressCities = new();

        // Parent Company and Branch lists
        private List<CompanyModel> ParentCompanies = new();
        private List<BranchModel> Branches = new();

        // Validation error messages
        private string PANError = "PAN Number is required";
        private string GSTINError = "GSTIN is required when GST Registered";
        private string IncorporationDateError = "";
        private string BooksStartDateError = "";

        private bool IsEdit => Id.HasValue;
        private string PageTitle => IsEdit ? "Edit Company" : "Create Company";
        private string PageSubTitle => IsEdit ? "Update company details" : "Create new company";
        protected override void OnInitialized()
        {
            editContext = new EditContext(Company);
        }
        private static CompanyModel CreateNewCompany()
        {
            return new CompanyModel
            {
                Status = "",
                IsActive = false,
                LegalStructure = "",
                FiscalYearStartMonth = 0,
                BooksStartDate = DateTime.Today,
                RoundingPrecision = 2,
                RoundingMode = ""
            };
        }

        protected async Task ScrollToCurrentStep()
        {
            await JS.InvokeVoidAsync("scrollToStep", CurrentStep - 1);
        }
        protected override async Task OnInitializedAsync()
        {
            Countries = MasterDataService.GetAllCountries().Where(c => c.IsActive).ToList();
            States = MasterDataService.GetAllStateProvinces().Where(s => s.IsActive).ToList();
            Cities = MasterDataService.GetAllCities().Where(c => c.IsActive).ToList();
            TimeZones = MasterDataService.GetAllTimeZones().Where(t => t.IsActive).ToList();
            Currencies = MasterDataService.GetAllCurrencies().Where(c => c.IsActive).ToList();

            // Load parent companies (exclude current company in edit mode)
            ParentCompanies = MasterDataService.GetAllCompanies()
                .Where(c => !c.IsDeleted && c.Status != "Inactive" && (!IsEdit || c.Id != Id!.Value))
                .ToList();

            // Load branches
            Branches = BranchService.GetAll().Where(b => b.Status == "Active").ToList();

            if (IsEdit)
            {
                var existing = MasterDataService.GetAllCompanies().FirstOrDefault(c => c.Id == Id!.Value);
                if (existing != null)
                {
                    IsInitializing = true;

                    Company = new CompanyModel
                    {
                        Id = existing.Id,
                        CompanyCode = existing.CompanyCode,
                        LegalName = existing.LegalName,
                        TradeName = existing.TradeName,
                        ShortName = existing.ShortName,
                        LegalStructure = existing.LegalStructure,
                        OtherLegalStructure = existing.OtherLegalStructure,
                        IncorporationDate = existing.IncorporationDate,
                        ParentCompanyId = existing.ParentCompanyId,
                        ParentCompanyName = existing.ParentCompanyName,
                        DefaultBranchId = existing.DefaultBranchId,
                        DefaultBranchName = existing.DefaultBranchName,
                        RegistrationNumber = existing.RegistrationNumber,
                        PANNumber = existing.PANNumber,
                        GSTIN = existing.GSTIN,
                        IsGSTRegistered = existing.IsGSTRegistered,
                        TANNumber = existing.TANNumber,
                        OtherTaxId = existing.OtherTaxId,
                        RegistrationCountryId = existing.RegistrationCountryId,
                        RegistrationCountryName = existing.RegistrationCountryName,
                        RegistrationStateProvinceId = existing.RegistrationStateProvinceId,
                        RegistrationStateProvinceName = existing.RegistrationStateProvinceName,
                        RegistrationCityId = existing.RegistrationCityId,
                        RegistrationCityName = existing.RegistrationCityName,
                        AddressLine1 = existing.AddressLine1,
                        AddressLine2 = existing.AddressLine2,
                        CountryId = existing.CountryId,
                        CountryName = existing.CountryName,
                        StateProvinceId = existing.StateProvinceId,
                        StateProvinceName = existing.StateProvinceName,
                        CityId = existing.CityId,
                        CityName = existing.CityName,
                        PostalCode = existing.PostalCode,
                        TimeZoneId = existing.TimeZoneId,
                        TimeZoneName = existing.TimeZoneName,
                        PrimaryContactName = existing.PrimaryContactName,
                        PrimaryEmail = existing.PrimaryEmail,
                        PrimaryPhone = existing.PrimaryPhone,
                        WebsiteUrl = existing.WebsiteUrl,
                        LogoBase64 = existing.LogoBase64,
                        LogoFileName = existing.LogoFileName,
                        LogoContentType = existing.LogoContentType,
                        BaseCurrencyId = existing.BaseCurrencyId,
                        BaseCurrencyName = existing.BaseCurrencyName,
                        ReportingCurrencyId = existing.ReportingCurrencyId,
                        ReportingCurrencyName = existing.ReportingCurrencyName,
                        FiscalYearStartMonth = existing.FiscalYearStartMonth,
                        BooksStartDate = existing.BooksStartDate,
                        EnableMultiCurrency = existing.EnableMultiCurrency,
                        RoundingPrecision = existing.RoundingPrecision,
                        RoundingMode = existing.RoundingMode,
                        AllowPostingFromDate = existing.AllowPostingFromDate,
                        AllowPostingToDate = existing.AllowPostingToDate,
                        LockBackDatedPosting = existing.LockBackDatedPosting,
                        BackdatedPostingDaysAllowed = existing.BackdatedPostingDaysAllowed,
                        FuturePostingDaysAllowed = existing.FuturePostingDaysAllowed,
                        Status = existing.Status,
                        IsActive = existing.IsActive,
                        Notes = existing.Notes
                    };

                    // Populate cascading dropdowns
                    if (Company.RegistrationCountryId != Guid.Empty)
                    {
                        RegistrationStates = States.Where(s => s.CountryId == Company.RegistrationCountryId).ToList();
                    }

                    if (Company.RegistrationStateProvinceId.HasValue && Company.RegistrationStateProvinceId != Guid.Empty)
                    {
                        RegistrationCities = Cities.Where(c => c.StateProvinceId == Company.RegistrationStateProvinceId.Value).ToList();
                    }

                    if (Company.CountryId != Guid.Empty)
                    {
                        AddressStates = States.Where(s => s.CountryId == Company.CountryId).ToList();
                    }

                    if (Company.StateProvinceId.HasValue && Company.StateProvinceId != Guid.Empty)
                    {
                        AddressCities = Cities.Where(c => c.StateProvinceId == Company.StateProvinceId.Value).ToList();
                    }

                    IsInitializing = false;
                }
                else
                {
                    Nav.NavigateTo("/companies");
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

        private void OnRegistrationCountryChange(ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var countryId) && countryId != Guid.Empty)
            {
                Company.RegistrationCountryId = countryId;
                RegistrationStates = States.Where(s => s.CountryId == countryId).ToList();
                Company.RegistrationStateProvinceId = null;
                RegistrationCities = new();
                Company.RegistrationCityId = null;
            }
            else
            {
                Company.RegistrationCountryId = Guid.Empty;
                RegistrationStates = new();
                Company.RegistrationStateProvinceId = null;
                RegistrationCities = new();
                Company.RegistrationCityId = null;
            }
        }

        private void OnRegistrationStateChange(ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var stateId) && stateId != Guid.Empty)
            {
                Company.RegistrationStateProvinceId = stateId;
                RegistrationCities = Cities.Where(c => c.StateProvinceId == stateId).ToList();
                Company.RegistrationCityId = null;
            }
            else
            {
                Company.RegistrationStateProvinceId = null;
                RegistrationCities = new();
                Company.RegistrationCityId = null;
            }
        }

        private void OnAddressCountryChange(ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var countryId) && countryId != Guid.Empty)
            {
                Company.CountryId = countryId;
                AddressStates = States.Where(s => s.CountryId == countryId).ToList();
                // Clear cities - they will be populated when state is selected
                AddressCities = new();

                if (!IsInitializing)
                {
                    Company.StateProvinceId = null;
                    Company.CityId = Guid.Empty;
                }
            }
            else
            {
                Company.CountryId = Guid.Empty;
                AddressStates = new();
                AddressCities = new();

                if (!IsInitializing)
                {
                    Company.StateProvinceId = null;
                    Company.CityId = Guid.Empty;
                }
            }
        }

        private void OnAddressStateChange(ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var stateId) && stateId != Guid.Empty)
            {
                Company.StateProvinceId = stateId;
                AddressCities = Cities.Where(c => c.StateProvinceId == stateId).ToList();

                if (!IsInitializing)
                {
                    Company.CityId = Guid.Empty;
                }
            }
            else
            {
                Company.StateProvinceId = null;
                // Clear cities - state is required, so cities should only load when state is selected
                AddressCities = new();

                if (!IsInitializing)
                {
                    Company.CityId = Guid.Empty;
                }
            }
        }

        private async Task Save()
        {
            // Collect Quill editor values before validation
            if (_notesEditor != null)
                Company.Notes = await _notesEditor.GetHtmlAsync();

            // Validate all required fields before saving
            if (string.IsNullOrWhiteSpace(Company.CompanyCode))
            {
                ToastService.ShowError("Company Code is required", "Validation Error");
                CurrentStep = 1;
                validationAttemptedSteps.Add(1);
                return;
            }

            // Trim CompanyCode
            Company.CompanyCode = Company.CompanyCode.Trim();

            // Validate CompanyCode: only letters, numbers, _ and -
            if (!System.Text.RegularExpressions.Regex.IsMatch(Company.CompanyCode, @"^[A-Za-z0-9_\-]+$"))
            {
                ToastService.ShowError("Company Code can only contain letters, numbers, underscore (_) and hyphen (-)", "Validation Error");
                CurrentStep = 1;
                validationAttemptedSteps.Add(1);
                return;
            }

            if (string.IsNullOrWhiteSpace(Company.LegalName))
            {
                ToastService.ShowError("Legal Name is required", "Validation Error");
                CurrentStep = 1;
                validationAttemptedSteps.Add(1);
                return;
            }

            if (string.IsNullOrWhiteSpace(Company.LegalStructure))
            {
                ToastService.ShowError("Legal Structure is required", "Validation Error");
                CurrentStep = 1;
                validationAttemptedSteps.Add(1);
                return;
            }

            if (Company.LegalStructure == "Other" && string.IsNullOrWhiteSpace(Company.OtherLegalStructure))
            {
                ToastService.ShowError("Please specify the legal structure", "Validation Error");
                CurrentStep = 1;
                validationAttemptedSteps.Add(1);
                return;
            }

            if (string.IsNullOrWhiteSpace(Company.Status))
            {
                ToastService.ShowError("Status is required", "Validation Error");
                CurrentStep = 1;
                validationAttemptedSteps.Add(1);
                return;
            }

            // IncorporationDate validation
            if (Company.IncorporationDate.HasValue)
            {
                if (Company.IncorporationDate.Value > DateTime.Today)
                {
                    ToastService.ShowError("Incorporation Date cannot be a future date", "Validation Error");
                    CurrentStep = 1;
                    validationAttemptedSteps.Add(1);
                    return;
                }
                if (Company.IncorporationDate.Value.Year < 1900)
                {
                    ToastService.ShowError("Incorporation Date cannot be earlier than 1900", "Validation Error");
                    CurrentStep = 1;
                    validationAttemptedSteps.Add(1);
                    return;
                }
            }

            if (HasPANError())
            {
                ToastService.ShowError(PANError, "Validation Error");
                CurrentStep = 2;
                validationAttemptedSteps.Add(2);
                return;
            }

            if (HasGSTINError())
            {
                ToastService.ShowError(GSTINError, "Validation Error");
                CurrentStep = 2;
                validationAttemptedSteps.Add(2);
                return;
            }

            if (!Company.RegistrationCountryId.HasValue || Company.RegistrationCountryId == Guid.Empty)
            {
                ToastService.ShowError("Registration Country is required", "Validation Error");
                CurrentStep = 2;
                validationAttemptedSteps.Add(2);
                return;
            }

            if (!Company.RegistrationCityId.HasValue || Company.RegistrationCityId == Guid.Empty)
            {
                ToastService.ShowError("Registration City is required", "Validation Error");
                CurrentStep = 2;
                validationAttemptedSteps.Add(2);
                return;
            }

            if (string.IsNullOrWhiteSpace(Company.AddressLine1))
            {
                ToastService.ShowError("Address Line 1 is required", "Validation Error");
                CurrentStep = 3;
                validationAttemptedSteps.Add(3);
                return;
            }

            if (Company.CountryId == Guid.Empty)
            {
                ToastService.ShowError("Country is required", "Validation Error");
                CurrentStep = 3;
                validationAttemptedSteps.Add(3);
                return;
            }

            // Check if the selected country is active
            var selectedCountry = MasterDataService.GetAllCountries().FirstOrDefault(c => c.Id == Company.CountryId);
            if (selectedCountry != null && !selectedCountry.IsActive)
            {
                ToastService.ShowError("Cannot create company with an inactive country. Please select an active country.", "Validation Error");
                CurrentStep = 3;
                validationAttemptedSteps.Add(3);
                return;
            }

            if (!Company.StateProvinceId.HasValue || Company.StateProvinceId == Guid.Empty)
            {
                ToastService.ShowError("State/Province is required", "Validation Error");
                CurrentStep = 3;
                validationAttemptedSteps.Add(3);
                return;
            }

            if (!Company.CityId.HasValue || Company.CityId == Guid.Empty)
            {
                ToastService.ShowError("City is required", "Validation Error");
                CurrentStep = 3;
                validationAttemptedSteps.Add(3);
                return;
            }

            if (string.IsNullOrWhiteSpace(Company.PostalCode))
            {
                ToastService.ShowError("Postal Code is required", "Validation Error");
                CurrentStep = 3;
                validationAttemptedSteps.Add(3);
                return;
            }

            if (!Company.TimeZoneId.HasValue || Company.TimeZoneId == Guid.Empty)
            {
                ToastService.ShowError("Time Zone is required", "Validation Error");
                CurrentStep = 3;
                validationAttemptedSteps.Add(3);
                return;
            }

            // Step 4: Email/Phone/URL format validation
            if (!IsValidEmail(Company.PrimaryEmail))
            {
                ToastService.ShowError("Invalid email format", "Validation Error");
                CurrentStep = 4;
                validationAttemptedSteps.Add(4);
                return;
            }

            if (!IsValidPhone(Company.PrimaryPhone))
            {
                ToastService.ShowError("Phone can only contain digits, +, -, spaces and parentheses", "Validation Error");
                CurrentStep = 4;
                validationAttemptedSteps.Add(4);
                return;
            }

            if (!IsValidUrl(Company.WebsiteUrl))
            {
                ToastService.ShowError("Invalid URL format (must start with http:// or https://)", "Validation Error");
                CurrentStep = 4;
                validationAttemptedSteps.Add(4);
                return;
            }

            if (!Company.BaseCurrencyId.HasValue || Company.BaseCurrencyId == Guid.Empty)
            {
                ToastService.ShowError("Base Currency is required", "Validation Error");
                CurrentStep = 5;
                validationAttemptedSteps.Add(5);
                return;
            }

            if (Company.FiscalYearStartMonth < 1 || Company.FiscalYearStartMonth > 12)
            {
                ToastService.ShowError("Fiscal Year Start Month is required", "Validation Error");
                CurrentStep = 5;
                validationAttemptedSteps.Add(5);
                return;
            }

            if (HasBooksStartDateError())
            {
                ToastService.ShowError(BooksStartDateError, "Validation Error");
                CurrentStep = 5;
                validationAttemptedSteps.Add(5);
                return;
            }

            // Validate RoundingMode is required
            if (string.IsNullOrWhiteSpace(Company.RoundingMode))
            {
                ToastService.ShowError("Rounding Mode is required", "Validation Error");
                CurrentStep = 5;
                validationAttemptedSteps.Add(5);
                return;
            }

            // Validate posting date cross-validation
            if (Company.AllowPostingFromDate.HasValue && Company.AllowPostingToDate.HasValue
                && Company.AllowPostingFromDate.Value > Company.AllowPostingToDate.Value)
            {
                ToastService.ShowError("'Allow Posting From' date must be on or before 'Allow Posting To' date", "Validation Error");
                CurrentStep = 6;
                validationAttemptedSteps.Add(6);
                return;
            }

            Company.CompanyCode = Company.CompanyCode?.ToUpper() ?? "";
            Company.LegalName = Company.LegalName?.Trim() ?? "";
            Company.TradeName = Company.TradeName?.Trim();
            Company.ShortName = Company.ShortName?.Trim();
            Company.PANNumber = Company.PANNumber?.ToUpper();
            Company.GSTIN = Company.GSTIN?.ToUpper();
            Company.TANNumber = Company.TANNumber?.ToUpper();

            // Set display names
            var country = Countries.FirstOrDefault(c => c.Id == Company.CountryId);
            var state = States.FirstOrDefault(s => s.Id == Company.StateProvinceId);
            var city = Cities.FirstOrDefault(c => c.Id == Company.CityId);
            var tz = TimeZones.FirstOrDefault(t => t.Id == Company.TimeZoneId);
            var currency = Currencies.FirstOrDefault(c => c.Id == Company.BaseCurrencyId);
            var regCountry = Countries.FirstOrDefault(c => c.Id == Company.RegistrationCountryId);
            var regState = States.FirstOrDefault(s => s.Id == Company.RegistrationStateProvinceId);
            var reportCurrency = Currencies.FirstOrDefault(c => c.Id == Company.ReportingCurrencyId);

            Company.CountryName = country?.CountryName;
            Company.StateProvinceName = state?.StateProvinceName;
            Company.CityName = city?.CityName;
            Company.TimeZoneName = tz?.DisplayName;
            Company.BaseCurrencyName = currency != null ? $"{currency.CurrencyName} ({currency.CurrencyCode})" : null;
            Company.ReportingCurrencyName = reportCurrency != null ? $"{reportCurrency.CurrencyName} ({reportCurrency.CurrencyCode})" : null;
            Company.RegistrationCountryName = regCountry?.CountryName;
            Company.RegistrationStateProvinceName = regState?.StateProvinceName;
            var regCity = Cities.FirstOrDefault(c => c.Id == Company.RegistrationCityId);
            Company.RegistrationCityName = regCity?.CityName;

            // Set parent company and branch names
            if (Company.ParentCompanyId.HasValue && Company.ParentCompanyId != Guid.Empty)
            {
                var parentCompany = ParentCompanies.FirstOrDefault(c => c.Id == Company.ParentCompanyId);
                Company.ParentCompanyName = parentCompany?.LegalName;
            }
            if (Company.DefaultBranchId.HasValue && Company.DefaultBranchId != Guid.Empty)
            {
                var branch = Branches.FirstOrDefault(b => b.Id == Company.DefaultBranchId);
                Company.DefaultBranchName = branch?.BranchName;
            }

            Company.IsActive = Company.Status == "Active";

            if (IsEdit)
            {
                MasterDataService.UpdateCompany(Company);
                ToastService.ShowSuccess($"Company '{Company.LegalName}' updated successfully", "Updated");
            }
            else
            {
                // Duplicate check
                var companies = MasterDataService.GetAllCompanies();
                if (companies.Any(c => c.CompanyCode.Equals(Company.CompanyCode, StringComparison.OrdinalIgnoreCase)))
                {
                    ToastService.ShowError("Company code already exists", "Validation Error");
                    return;
                }

                Company.Id = Guid.NewGuid();
                MasterDataService.AddCompany(Company);
                ToastService.ShowSuccess($"Company '{Company.LegalName}' added successfully", "Added");
            }

            Nav.NavigateTo("/companies");
        }

        //Test Ramya
        private const int TotalSteps = 6;
        private int CurrentStep = 1;

        private async Task OnLogoFileSelected(InputFileChangeEventArgs e)
        {
            var file = e.File;
            if (file.Size > 1_048_576) // 1MB
            {
                ToastService.ShowError("Logo file must be under 1MB", "File Error");
                return;
            }

            var allowedTypes = new[] { "image/png", "image/jpeg", "image/svg+xml" };
            if (!allowedTypes.Contains(file.ContentType))
            {
                ToastService.ShowError("Only PNG, JPG, and SVG files are allowed", "File Error");
                return;
            }

            using var stream = file.OpenReadStream(1_048_576);
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            Company.LogoBase64 = Convert.ToBase64String(ms.ToArray());
            Company.LogoFileName = file.Name;
            Company.LogoContentType = file.ContentType;
        }

        private void RemoveLogo()
        {
            Company.LogoBase64 = null;
            Company.LogoFileName = null;
            Company.LogoContentType = null;
        }

        private int ProgressPercent => (CurrentStep * 100) / TotalSteps;
        private void NextStep()
        {
            if (CurrentStep == 1 && !Step1Valid)
                return;

            if (CurrentStep < TotalSteps)
                CurrentStep++;
        }

        private void PrevStep()
        {
            if (CurrentStep > 1)
                CurrentStep--;
        }

        private bool Step1Valid =>
            !string.IsNullOrWhiteSpace(Company.CompanyCode)
            && !string.IsNullOrWhiteSpace(Company.LegalName)
            && !string.IsNullOrWhiteSpace(Company.LegalStructure)
            && !string.IsNullOrWhiteSpace(Company.Status);

        private string GetStepClass(int step)
        {
            if (step == CurrentStep) return "active";
            if (step < CurrentStep) return "done";
            return "";
        }

        private string GetStepName(int step)
        {
            return step switch
            {
                1 => "Company Identity",
                2 => "Registration & Compliance",
                3 => "Registered Address",
                4 => "Contact & Branding",
                5 => "Financial Settings",
                6 => "System & Posting Controls",
                _ => ""
            };
        }



        List<WizardStep> Steps = new()
    {
        new("Company Identity","Basic Info","ti ti-building"),
        new("Registration & Compliance","PAN & GST","ti ti-file"),
        new("Registered Address","Registered Address","ti ti-map"),
        new("Contact & Branding","Email & Phone","ti ti-phone"),
        new("Financial Settings","Extra Info","ti ti-settings"),
        new("System & Posting Controls","Posting Rules","ti ti-lock")
    };

        string IndicatorToppx => $"{40 + ((CurrentStep - 1) * 76)}px";

        private async Task Next()
        {
            // Mark this step as having validation attempted
            validationAttemptedSteps.Add(CurrentStep);

            // Only validate the current step, not the entire form
            if (!IsCurrentStepValid())
            {
                // Trigger re-render to show validation errors
                StateHasChanged();
                return; // stop if current step is invalid
            }

            if (CurrentStep < Steps.Count) CurrentStep++;
            await ScrollToCurrentStep();
        }

        void Back()
        {
            if (CurrentStep > 1) CurrentStep--;

        }


        private Task OnCompanyCodeInput()
        {
            Company.CompanyCode = Company.CompanyCode?.Trim() ?? "";
            Company.LegalName = Company.LegalName?.Trim() ?? "";
            return Task.CompletedTask;
        }

        // Check if a specific field error should be shown
        // Only shows error after user has attempted to proceed from that step
        bool ShowFieldError(int step, string fieldName)
        {
            // Only show errors if validation was attempted for this step
            if (!validationAttemptedSteps.Contains(step))
                return false;

            return fieldName switch
            {
                // Step 1
                "CompanyCode" => string.IsNullOrWhiteSpace(Company.CompanyCode),
                "CompanyCodeFormat" => !string.IsNullOrWhiteSpace(Company.CompanyCode) && !System.Text.RegularExpressions.Regex.IsMatch(Company.CompanyCode.Trim(), @"^[A-Za-z0-9_\-]+$"),
                "LegalName" => string.IsNullOrWhiteSpace(Company.LegalName),
                "LegalStructure" => string.IsNullOrWhiteSpace(Company.LegalStructure),
                "OtherLegalStructure" => Company.LegalStructure == "Other" && string.IsNullOrWhiteSpace(Company.OtherLegalStructure),
                "Status" => string.IsNullOrWhiteSpace(Company.Status),
                "IncorporationDate" => HasIncorporationDateError(),

                // Step 2
                "PANNumber" => HasPANError(),
                "GSTIN" => HasGSTINError(),
                "RegistrationCountryId" => !Company.RegistrationCountryId.HasValue || Company.RegistrationCountryId == Guid.Empty,
                "RegistrationCityId" => !Company.RegistrationCityId.HasValue || Company.RegistrationCityId == Guid.Empty,

                // Step 3
                "AddressLine1" => string.IsNullOrWhiteSpace(Company.AddressLine1),
                "CountryId" => Company.CountryId == Guid.Empty,
                "StateProvinceId" => !Company.StateProvinceId.HasValue || Company.StateProvinceId == Guid.Empty,
                "CityId" => !Company.CityId.HasValue || Company.CityId == Guid.Empty,
                "PostalCode" => string.IsNullOrWhiteSpace(Company.PostalCode),
                "TimeZoneId" => !Company.TimeZoneId.HasValue || Company.TimeZoneId == Guid.Empty,

                // Step 4
                "PrimaryEmail" => !string.IsNullOrWhiteSpace(Company.PrimaryEmail) && !IsValidEmail(Company.PrimaryEmail),
                "PrimaryPhone" => !string.IsNullOrWhiteSpace(Company.PrimaryPhone) && !IsValidPhone(Company.PrimaryPhone),
                "WebsiteUrl" => !string.IsNullOrWhiteSpace(Company.WebsiteUrl) && !IsValidUrl(Company.WebsiteUrl),

                // Step 5
                "BaseCurrencyId" => !Company.BaseCurrencyId.HasValue || Company.BaseCurrencyId == Guid.Empty,
                "FiscalYearStartMonth" => Company.FiscalYearStartMonth < 1 || Company.FiscalYearStartMonth > 12,
                "BooksStartDate" => HasBooksStartDateError(),
                "RoundingMode" => string.IsNullOrWhiteSpace(Company.RoundingMode),

                // Step 6
                "PostingDates" => Company.AllowPostingFromDate.HasValue && Company.AllowPostingToDate.HasValue
                    && Company.AllowPostingFromDate.Value > Company.AllowPostingToDate.Value,

                _ => false
            };
        }

        // Validation helper methods
        private bool HasIncorporationDateError()
        {
            if (!Company.IncorporationDate.HasValue) return false;
            if (Company.IncorporationDate.Value > DateTime.Today)
            {
                IncorporationDateError = "Incorporation Date cannot be a future date";
                return true;
            }
            if (Company.IncorporationDate.Value.Year < 1900)
            {
                IncorporationDateError = "Incorporation Date cannot be earlier than 1900";
                return true;
            }
            return false;
        }

        private bool HasPANError()
        {
            if (string.IsNullOrWhiteSpace(Company.PANNumber))
            {
                PANError = "PAN Number is required";
                return true;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(Company.PANNumber.Trim(), @"^[A-Za-z]{5}[0-9]{4}[A-Za-z]$"))
            {
                PANError = "Invalid PAN format (must be AAAAA9999A)";
                return true;
            }
            return false;
        }

        private bool HasGSTINError()
        {
            if (!Company.IsGSTRegistered) return false;
            if (string.IsNullOrWhiteSpace(Company.GSTIN))
            {
                GSTINError = "GSTIN is required when GST Registered";
                return true;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(Company.GSTIN.Trim(), @"^\d{2}[A-Za-z]{5}\d{4}[A-Za-z]\d[Zz][A-Za-z\d]$"))
            {
                GSTINError = "Invalid GSTIN format (must be 15 chars, e.g., 33AAAAA9999A1Z5)";
                return true;
            }
            return false;
        }

        private bool HasBooksStartDateError()
        {
            if (Company.BooksStartDate > DateTime.Today)
            {
                BooksStartDateError = "Books Start Date cannot be later than today";
                return true;
            }
            if (Company.IncorporationDate.HasValue && Company.BooksStartDate < Company.IncorporationDate.Value)
            {
                BooksStartDateError = "Books Start Date cannot be earlier than Incorporation Date";
                return true;
            }
            return false;
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

        bool IsCurrentStepValid()
        {
            bool isValid = CurrentStep switch
            {
                1 => !string.IsNullOrWhiteSpace(Company.CompanyCode)
                     && System.Text.RegularExpressions.Regex.IsMatch(Company.CompanyCode.Trim(), @"^[A-Za-z0-9_\-]+$")
                     && !string.IsNullOrWhiteSpace(Company.LegalName)
                     && !string.IsNullOrWhiteSpace(Company.LegalStructure)
                     && (Company.LegalStructure != "Other" || !string.IsNullOrWhiteSpace(Company.OtherLegalStructure))
                     && !string.IsNullOrWhiteSpace(Company.Status)
                     && !HasIncorporationDateError(),
                2 => Company.RegistrationCountryId.HasValue && Company.RegistrationCountryId != Guid.Empty
                     && !HasPANError()
                     && !HasGSTINError()
                     && Company.RegistrationCityId.HasValue && Company.RegistrationCityId != Guid.Empty,
                3 => !string.IsNullOrWhiteSpace(Company.AddressLine1)
                     && Company.CountryId != Guid.Empty
                     && Company.StateProvinceId.HasValue && Company.StateProvinceId != Guid.Empty
                     && Company.CityId.HasValue && Company.CityId != Guid.Empty
                     && !string.IsNullOrWhiteSpace(Company.PostalCode)
                     && Company.TimeZoneId.HasValue && Company.TimeZoneId != Guid.Empty,
                4 => IsValidEmail(Company.PrimaryEmail)
                     && IsValidPhone(Company.PrimaryPhone)
                     && IsValidUrl(Company.WebsiteUrl),
                5 => Company.BaseCurrencyId.HasValue && Company.BaseCurrencyId != Guid.Empty
                     && Company.FiscalYearStartMonth >= 1 && Company.FiscalYearStartMonth <= 12
                     && !HasBooksStartDateError()
                     && !string.IsNullOrWhiteSpace(Company.RoundingMode),
                6 => !(Company.AllowPostingFromDate.HasValue && Company.AllowPostingToDate.HasValue
                     && Company.AllowPostingFromDate.Value > Company.AllowPostingToDate.Value),
                _ => true
            };

            return isValid;
        }

        string StepClass(int i)
        {
            if (i < CurrentStep) return "done";
            if (i == CurrentStep) return "active";
            return "";
        }

        public record WizardStep(string Title, string Description, string Icon);

    }



}
