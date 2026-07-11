using FinanceConnect.Client.Services;
using FinanceConnect.Client.Shared;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Linq.Expressions;

namespace FinanceConnect.Client.Pages.Master.City
{
    public partial class AddCity
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        private EditContext _editContext;
        RichTextEditor? _notesEditor;
        [Parameter] public Guid? Id { get; set; }

        private bool isInitialized = false;
        private bool IsInitializing = false;

        private CityModel City = CreateNewCity();
        private List<CountryModel> Countries = new();
        private List<StateProvinceModel> AllStates = new();
        private List<StateProvinceModel> FilteredStates = new();

        private bool IsEdit => Id.HasValue;
        private string PageTitle => IsEdit ? "Edit City" : "Create City";
        private string PageSubTitle => IsEdit ? "Update city details" : "Create new city";

        private static CityModel CreateNewCity()
        {
            return new CityModel
            {
                Status = null,
                IsActive = false,
                IsMetro = false
            };
        }

        // Custom validation error messages
        string? CountryValidationError = null;
        string? StateValidationError = null;
        string? StatusValidationError = null;

        // Property wrapper for Status dropdown
        private string SelectedCityStatus
        {
            get => City.Status ?? "";
            set
            {
                City.Status = string.IsNullOrEmpty(value) ? null : value;
                StatusValidationError = null;
            }
        }

        void OnStatusChanged()
        {
            StatusValidationError = null;
        }

        protected override async Task OnInitializedAsync()
        {
            Countries = MasterDataService.GetAllCountries().Where(c => c.IsActive).ToList();
            AllStates = MasterDataService.GetAllStateProvinces().Where(s => s.IsActive).ToList();

            if (IsEdit)
            {
                var existing = MasterDataService.GetAllCities().FirstOrDefault(c => c.Id == Id!.Value);
                if (existing != null)
                {
                    IsInitializing = true;

                    City = new CityModel
                    {
                        Id = existing.Id,
                        TenantId = existing.TenantId,
                        CityCode = existing.CityCode,
                        CityName = existing.CityName,
                        DisplayName = existing.DisplayName,
                        CountryId = existing.CountryId,
                        CountryName = existing.CountryName,
                        StateProvinceId = existing.StateProvinceId,
                        StateProvinceName = existing.StateProvinceName,
                        DefaultPostalCodePattern = existing.DefaultPostalCodePattern,
                        IsMetro = existing.IsMetro,
                        Latitude = existing.Latitude,
                        Longitude = existing.Longitude,
                        Status = existing.Status,
                        IsActive = existing.IsActive,
                        IsDeleted = existing.IsDeleted,
                        Notes = existing.Notes,
                        CreatedAt = existing.CreatedAt,
                        CreatedBy = existing.CreatedBy,
                        UpdatedAt = existing.UpdatedAt,
                        UpdatedBy = existing.UpdatedBy
                    };

                    // Populate filtered states
                    if (City.CountryId != Guid.Empty)
                    {
                        FilteredStates = AllStates.Where(s => s.CountryId == City.CountryId).ToList();
                    }

                    IsInitializing = false;
                }
                else
                {
                    Nav.NavigateTo("/cities");
                    return;
                }
            }

            isInitialized = true;
            _editContext = new EditContext(City);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
        }

        private string GetValidationClass(Expression<Func<object>> field)
        {
            if (_editContext == null)
                return string.Empty;

            var fieldIdentifier = FieldIdentifier.Create(field);

            var hasError = _editContext.GetValidationMessages(fieldIdentifier).Any();
            var isModified = _editContext.IsModified(fieldIdentifier);

            if (hasError)
                return "is-invalid";

            if (isModified)
                return "is-valid";

            return string.Empty;
        }


        private void OnCountryChange(ChangeEventArgs e)
        {
            CountryValidationError = null;
            StateValidationError = null;

            if (Guid.TryParse(e.Value?.ToString(), out var countryId) && countryId != Guid.Empty)
            {
                City.CountryId = countryId;
                FilteredStates = AllStates.Where(s => s.CountryId == countryId).ToList();

                if (!IsInitializing)
                {
                    City.StateProvinceId = Guid.Empty;
                }
            }
            else
            {
                City.CountryId = Guid.Empty;
                FilteredStates = new();

                if (!IsInitializing)
                {
                    City.StateProvinceId = Guid.Empty;
                }
            }
        }

        private void OnStateChange(ChangeEventArgs e)
        {
            StateValidationError = null;

            if (Guid.TryParse(e.Value?.ToString(), out var stateId) && stateId != Guid.Empty)
            {
                City.StateProvinceId = stateId;
            }
            else
            {
                City.StateProvinceId = Guid.Empty;
            }
        }

        private async Task HandleSubmit()
        {
            // Collect Quill editor values before validation
            if (_notesEditor != null)
                City.Notes = await _notesEditor.GetHtmlAsync();

            // Validate DataAnnotations first
            var isDataAnnotationsValid = _editContext.Validate();

            // Custom validation for dropdowns
            CountryValidationError = null;
            StateValidationError = null;
            StatusValidationError = null;
            bool isCustomValid = true;

            if (City.CountryId == Guid.Empty)
            {
                CountryValidationError = "Country is required";
                isCustomValid = false;
            }

            if (City.StateProvinceId == Guid.Empty)
            {
                StateValidationError = "State/Province is required";
                isCustomValid = false;
            }

            if (string.IsNullOrWhiteSpace(City.Status))
            {
                StatusValidationError = "Status is required";
                isCustomValid = false;
            }

            if (!isDataAnnotationsValid || !isCustomValid)
            {
                return;
            }

            Save();
        }

        private void Save()
        {
            City.CityCode = City.CityCode?.ToUpper().Trim() ?? "";
            City.CityName = City.CityName?.Trim() ?? "";
            City.DisplayName = City.DisplayName?.Trim();
            City.Notes = City.Notes?.Trim();

            // Verify state belongs to country
            var state = AllStates.FirstOrDefault(s => s.Id == City.StateProvinceId);
            if (state != null && state.CountryId != City.CountryId)
            {
                StateValidationError = "Selected state does not belong to the selected country";
                return;
            }

            // Set display names
            var country = Countries.FirstOrDefault(c => c.Id == City.CountryId);
            City.CountryName = country?.CountryName;
            City.StateProvinceName = state?.StateProvinceName;

            // Set IsActive based on Status
            City.IsActive = City.Status == "Active";

            if (IsEdit)
            {
                City.UpdatedAt = DateTime.Now;
                City.UpdatedBy = "System";
                MasterDataService.UpdateCity(City);
                ToastService.ShowSuccess($"City '{City.CityName}' updated successfully", "Updated");
            }
            else
            {
                // Duplicate check
                var cities = MasterDataService.GetAllCities();
                if (cities.Any(c => c.StateProvinceId == City.StateProvinceId &&
                    c.CityCode.Equals(City.CityCode, StringComparison.OrdinalIgnoreCase)))
                {
                    ToastService.ShowError("City code already exists in this state/province", "Validation Error");
                    return;
                }

                City.Id = Guid.NewGuid();
                City.CreatedAt = DateTime.Now;
                City.CreatedBy = "System";
                MasterDataService.AddCity(City);
                ToastService.ShowSuccess($"City '{City.CityName}' added successfully", "Added");
            }

            // Navigate back to list - Blazor will properly reload the component
            Nav.NavigateTo("/cities");
        }

        private Task OnCityNameInput()
        {
            City.CityName = City.CityName?.Trim() ?? "";
            return Task.CompletedTask;
        }
    }

    // City Status
    public static class CityStatus
    {
        public const string Draft = "Draft";
        public const string Active = "Active";
        public const string Inactive = "Inactive";

        public static readonly string[] All = new[] { Draft, Active, Inactive };
    }
    }
