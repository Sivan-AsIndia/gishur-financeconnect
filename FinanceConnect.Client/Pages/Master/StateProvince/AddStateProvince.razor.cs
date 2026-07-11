using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using FinanceConnect.Client.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Master.StateProvince
{
    public partial class AddStateProvince
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;

        [Parameter] public Guid? Id { get; set; }

        private EditContext _editContext = default!;
        RichTextEditor? _notesEditor;
        private bool isInitialized = false;

        private StateProvinceModel StateProvince = CreateNewStateProvince();
        private List<CountryModel> Countries = new();
        private List<TimeZoneModel> TimeZones = new();

        private bool IsEdit => Id.HasValue;
        private string PageTitle => IsEdit ? "Edit State" : "Create State";
        private string PageSubTitle => IsEdit ? "Update state details" : "Create new state";

        // Touched state for accordion sections
        bool StateTouched = false;
        bool AddressTouched = false;
        bool ContactTouched = false;
        bool FinanceTouched = false;
        bool ReportTouched = false;

        // Accordion visibility state
        bool ShowIdentity = true;
        bool ShowCountry = false;
        bool ShowCompliance = false;
        bool ShowAddressRules = false;
        bool ShowStatus = false;

        // Custom validation error messages for dropdowns and text fields
        string? StateProvinceCodeValidationError = null;
        string? StateProvinceNameValidationError = null;
        string? CountryValidationError = null;
        string? JurisdictionTypeValidationError = null;
        string? StatusValidationError = null;
        string? GSTStateCodeValidationError = null;

        // Computed property to check if India is selected
        bool IsIndiaSelected => Countries.FirstOrDefault(c => c.Id == StateProvince.CountryId)?.CountryCode == "IN";

        void TouchState() => StateTouched = true;
        void TouchAddress() => AddressTouched = true;
        void TouchContact() => ContactTouched = true;
        void TouchFinance() => FinanceTouched = true;
        void TouchReport() => ReportTouched = true;

        void OnStateProvinceCodeChanged()
        {
            StateTouched = true;
            StateProvinceCodeValidationError = null;
            StateProvince.StateProvinceCode = StateProvince.StateProvinceCode?.Trim() ?? "";
        }

        void OnStateProvinceNameChanged()
        {
            StateProvince.StateProvinceName = new string(StateProvince.StateProvinceName
            .Where(c => !char.IsDigit(c)).ToArray());
            StateTouched = true;
            StateProvinceNameValidationError = null;
        }

        void OnStateNameChangedTrim()
        {
            StateProvince.StateProvinceName = StateProvince.StateProvinceName?.Trim() ?? "";
        }
        void OnDisplayNameChanged()
        {
            StateProvince.DisplayName = StateProvince.DisplayName?.Trim() ?? "";
        }

        void ClearGSTValidationError()
        {
            ContactTouched = true;
            GSTStateCodeValidationError = null;
        }

        // Property wrapper for Country dropdown
        private string SelectedCountryId
        {
            get => StateProvince.CountryId?.ToString() ?? "";
            set
            {
                StateProvince.CountryId = string.IsNullOrEmpty(value) || value == Guid.Empty.ToString()
                    ? null
                    : Guid.Parse(value);
                CountryValidationError = null; // Clear error on change
                GSTStateCodeValidationError = null; // Clear GST error when country changes
            }
        }

        private string SelectedStateProvinceStatus
        {
            get => StateProvince.Status ?? "";
            set
            {
                StateProvince.Status = string.IsNullOrEmpty(value) ? null : value;
                StatusValidationError = null;
            }
        }

        // OnChanged handlers
        void OnCountryChanged()
        {
            AddressTouched = true;
            CountryValidationError = null;
            GSTStateCodeValidationError = null;
        }

        // Property wrapper for Jurisdiction Type dropdown
        private string SelectedJurisdictionType
        {
            get => StateProvince.JurisdictionType ?? "";
            set
            {
                StateProvince.JurisdictionType = string.IsNullOrEmpty(value) ? null : value;
                JurisdictionTypeValidationError = null;
            }
        }

        void OnJurisdictionTypeChanged()
        {
            AddressTouched = true;
            JurisdictionTypeValidationError = null;
        }

        void OnStatusChanged()
        {
            ReportTouched = true;
            StatusValidationError = null;
        }

        private static StateProvinceModel CreateNewStateProvince()
        {
            return new StateProvinceModel
            {
                Status = null,
                IsActive = false,
                JurisdictionType = null,
                SortOrder = 0
            };
        }

        protected override async Task OnInitializedAsync()
        {
            Countries = MasterDataService.GetAllCountries().Where(c => c.IsActive).ToList();
            TimeZones = MasterDataService.GetAllTimeZones().Where(t => t.IsActive).ToList();

            if (IsEdit)
            {
                var existing = MasterDataService.GetAllStateProvinces().FirstOrDefault(s => s.Id == Id!.Value);
                if (existing != null)
                {
                    StateProvince = new StateProvinceModel
                    {
                        Id = existing.Id,
                        TenantId = existing.TenantId,
                        StateProvinceCode = existing.StateProvinceCode,
                        StateProvinceName = existing.StateProvinceName,
                        DisplayName = existing.DisplayName,
                        CountryId = existing.CountryId,
                        CountryName = existing.CountryName,
                        JurisdictionType = existing.JurisdictionType,
                        IsFederalJurisdiction = existing.IsFederalJurisdiction,
                        GSTStateCode = existing.GSTStateCode,
                        StateTaxJurisdictionCode = existing.StateTaxJurisdictionCode,
                        DefaultTimeZoneId = existing.DefaultTimeZoneId,
                        PostalCodePattern = existing.PostalCodePattern,
                        AddressFormatHint = existing.AddressFormatHint,
                        Status = existing.Status,
                        IsActive = existing.IsActive,
                        IsDeleted = existing.IsDeleted,
                        SortOrder = existing.SortOrder,
                        Notes = existing.Notes,
                        CreatedAt = existing.CreatedAt,
                        CreatedBy = existing.CreatedBy,
                        UpdatedAt = existing.UpdatedAt,
                        UpdatedBy = existing.UpdatedBy
                    };
                }
                else
                {
                    Nav.NavigateTo("/states");
                    return;
                }
            }

            _editContext = new EditContext(StateProvince);
            isInitialized = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
        }

        void ToggleAccordion(string section)
        {
            switch (section)
            {
                case "stateIdentity":
                    ShowIdentity = !ShowIdentity;
                    break;
                case "countryLinkage":
                    ShowCountry = !ShowCountry;
                    break;
                case "compliance":
                    ShowCompliance = !ShowCompliance;
                    break;
                case "addressRules":
                    ShowAddressRules = !ShowAddressRules;
                    break;
                case "statusGovernance":
                    ShowStatus = !ShowStatus;
                    break;
            }
        }

        void OpenAccordion(string section)
        {
            switch (section)
            {
                case "stateIdentity":
                    ShowIdentity = true;
                    break;
                case "countryLinkage":
                    ShowCountry = true;
                    break;
                case "compliance":
                    ShowCompliance = true;
                    break;
                case "addressRules":
                    ShowAddressRules = true;
                    break;
                case "statusGovernance":
                    ShowStatus = true;
                    break;
            }
        }

        bool HasIdentityErrors()
        {
            return string.IsNullOrWhiteSpace(StateProvince.StateProvinceCode)
                || string.IsNullOrWhiteSpace(StateProvince.StateProvinceName);
        }

        bool HasCountryErrors()
        {
            // Check for both null AND Guid.Empty since CountryId is Guid?
            return (!StateProvince.CountryId.HasValue || StateProvince.CountryId == Guid.Empty)
                || string.IsNullOrWhiteSpace(StateProvince.JurisdictionType);
        }

        bool HasStatusErrors()
        {
            return string.IsNullOrWhiteSpace(StateProvince.Status);
        }

        bool HasComplianceErrors()
        {
            // GST State Code is required for India
            var country = Countries.FirstOrDefault(c => c.Id == StateProvince.CountryId);
            if (country?.CountryCode == "IN" && string.IsNullOrWhiteSpace(StateProvince.GSTStateCode))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Validates all required fields and sets validation error messages
        /// </summary>
        private bool ValidateAllFields()
        {
            bool isValid = true;

            // Clear all validation errors first
            StateProvinceCodeValidationError = null;
            StateProvinceNameValidationError = null;
            CountryValidationError = null;
            JurisdictionTypeValidationError = null;
            StatusValidationError = null;
            GSTStateCodeValidationError = null;

            // Validate State/Province Code
            if (string.IsNullOrWhiteSpace(StateProvince.StateProvinceCode))
            {
                StateProvinceCodeValidationError = "State/Province Code is required";
                isValid = false;
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(StateProvince.StateProvinceCode.Trim(), @"^[A-Za-z0-9_\-]+$"))
            {
                StateProvinceCodeValidationError = "Only letters, numbers, underscore (_) and hyphen (-) are allowed";
                isValid = false;
            }

            // Validate State/Province Name
            if (string.IsNullOrWhiteSpace(StateProvince.StateProvinceName))
            {
                StateProvinceNameValidationError = "State/Province Name is required";
                isValid = false;
            }

            // Validate Country
            if (!StateProvince.CountryId.HasValue || StateProvince.CountryId == Guid.Empty)
            {
                CountryValidationError = "Country is required";
                isValid = false;
            }

            // Validate Jurisdiction Type
            if (string.IsNullOrWhiteSpace(StateProvince.JurisdictionType))
            {
                JurisdictionTypeValidationError = "Jurisdiction Type is required";
                isValid = false;
            }

            // Validate Status
            if (string.IsNullOrWhiteSpace(StateProvince.Status))
            {
                StatusValidationError = "Status is required";
                isValid = false;
            }

            // Validate GST State Code for India
            if (IsIndiaSelected && string.IsNullOrWhiteSpace(StateProvince.GSTStateCode))
            {
                GSTStateCodeValidationError = "GST State Code is required for India";
                isValid = false;
            }

            return isValid;
        }

        private async Task HandleSubmit()
        {

            // Collect Quill editor values before validation
            if (_notesEditor != null)
                StateProvince.Notes = await _notesEditor.GetHtmlAsync();

            // Validate all fields
            var isValid = ValidateAllFields();

            if (isValid)
            {
                Save();
                return;
            }

            // Open ALL accordions that have validation errors
            if (HasIdentityErrors())
                OpenAccordion("stateIdentity");
            if (HasCountryErrors())
                OpenAccordion("countryLinkage");
            if (HasComplianceErrors())
                OpenAccordion("compliance");
            if (HasStatusErrors())
                OpenAccordion("statusGovernance");

            await InvokeAsync(StateHasChanged);
        }

        private void Save()
        {
            // Validate all fields first
            if (!ValidateAllFields())
            {
                // Open accordions with errors
                if (HasIdentityErrors())
                    OpenAccordion("stateIdentity");
                if (HasCountryErrors())
                    OpenAccordion("countryLinkage");
                if (HasComplianceErrors())
                    OpenAccordion("compliance");
                if (HasStatusErrors())
                    OpenAccordion("statusGovernance");
                return;
            }

            StateProvince.StateProvinceCode = StateProvince.StateProvinceCode?.ToUpper().Trim() ?? "";
            StateProvince.StateProvinceName = StateProvince.StateProvinceName?.Trim() ?? "";
            StateProvince.DisplayName = StateProvince.DisplayName?.Trim();
            StateProvince.Notes = StateProvince.Notes?.Trim();
            StateProvince.GSTStateCode = StateProvince.GSTStateCode?.Trim();
            if (StateProvince.SortOrder < 0) StateProvince.SortOrder = 0;

            // Set country name for display
            var country = Countries.FirstOrDefault(c => c.Id == StateProvince.CountryId);
            StateProvince.CountryName = country?.CountryName;

            // Set IsActive based on Status
            StateProvince.IsActive = StateProvince.Status == "Active";

            if (IsEdit)
            {
                StateProvince.UpdatedAt = DateTime.Now;
                StateProvince.UpdatedBy = "System";
                MasterDataService.UpdateStateProvince(StateProvince);
                ToastService.ShowSuccess($"State/Province '{StateProvince.StateProvinceName}' updated successfully", "Updated");
            }
            else
            {
                // Duplicate check
                var states = MasterDataService.GetAllStateProvinces();
                if (states.Any(s => s.CountryId == StateProvince.CountryId &&
                    s.StateProvinceCode.Equals(StateProvince.StateProvinceCode, StringComparison.OrdinalIgnoreCase)))
                {
                    OpenAccordion("stateIdentity");
                    return;
                }

                StateProvince.Id = Guid.NewGuid();
                StateProvince.CreatedAt = DateTime.Now;
                StateProvince.CreatedBy = "System";
                MasterDataService.AddStateProvince(StateProvince);
                ToastService.ShowSuccess($"State/Province '{StateProvince.StateProvinceName}' added successfully", "Added");
            }

            Nav.NavigateTo("/states");
        }
    }

    // Jurisdiction Types
    public static class JurisdictionTypes
    {
        public const string State = "State";
        public const string UnionTerritory = "Union Territory";
        public const string Province = "Province";
        public const string Territory = "Territory";
        public const string Region = "Region";
        public const string Other = "Other";

        public static readonly string[] All = new[] { State, UnionTerritory, Province, Territory, Region, Other };
    }

    // State/Province Status
    public static class StateProvinceStatus
    {
        public const string Draft = "Draft";
        public const string Active = "Active";
        public const string Inactive = "Inactive";

        public static readonly string[] All = new[] { Draft, Active, Inactive };
    }
}
