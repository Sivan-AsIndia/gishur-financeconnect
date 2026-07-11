using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Linq.Expressions;

namespace FinanceConnect.Client.Pages.Master.Country
{
    public partial class AddCountry
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        private EditContext _editContext;

        [Parameter] public Guid? Id { get; set; }

        private bool isInitialized = false;

        private CountryModel Country = CreateNewCountry();

        private bool IsEdit => Id.HasValue;
        private string PageTitle => IsEdit ? "Edit Country" : "Create Country";
        private string PageSubTitle => IsEdit ? "Update country details" : "Create new country";

        private static CountryModel CreateNewCountry()
        {
            return new CountryModel
            {
                IsActive = true,
                HasStates = true,
                SortOrder = 0
            };
        }

        protected override async Task OnInitializedAsync()
        {
            if (IsEdit)
            {
                var existing = MasterDataService.GetAllCountries().FirstOrDefault(c => c.Id == Id!.Value);
                if (existing != null)
                {
                    Country = new CountryModel
                    {
                        Id = existing.Id,
                        TenantId = existing.TenantId,
                        CountryCode = existing.CountryCode,
                        CountryName = existing.CountryName,
                        OfficialName = existing.OfficialName,
                        Region = existing.Region,
                        ISO2 = existing.ISO2,
                        ISO3 = existing.ISO3,
                        NumericCode = existing.NumericCode,
                        DefaultCurrencyCode = existing.DefaultCurrencyCode,
                        HasStates = existing.HasStates,
                        PostalCodePattern = existing.PostalCodePattern,
                        PhoneCountryCode = existing.PhoneCountryCode,
                        IsActive = existing.IsActive,
                        IsDeleted = existing.IsDeleted,
                        SortOrder = existing.SortOrder,
                        CreatedAt = existing.CreatedAt,
                        CreatedBy = existing.CreatedBy,
                        UpdatedAt = existing.UpdatedAt,
                        UpdatedBy = existing.UpdatedBy
                    };
                }
                else
                {
                    Nav.NavigateTo("/countries");
                    return;
                }
            }

            isInitialized = true;
            _editContext = new EditContext(Country);
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

        void onCodeChange()
        {
            Country.CountryCode = Country.CountryCode?.ToUpper().Trim() ?? "";
        }

        void onCountryNameChange()
        {
            Country.CountryName = Country.CountryName?.Trim() ?? "";
        }

        void onofficialNameChange()
        {
            Country.OfficialName = Country.OfficialName?.Trim();
        }
        private void Save()
        {
            Country.CountryCode = Country.CountryCode?.ToUpper().Trim() ?? "";
            Country.CountryName = Country.CountryName?.Trim() ?? "";
            Country.OfficialName = Country.OfficialName?.Trim();
            Country.ISO2 = Country.ISO2?.ToUpper().Trim() ?? "";
            Country.ISO3 = Country.ISO3?.ToUpper().Trim();
            Country.DefaultCurrencyCode = Country.DefaultCurrencyCode?.ToUpper().Trim();

            if (IsEdit)
            {
                Country.UpdatedAt = DateTime.Now;
                Country.UpdatedBy = "System";
                MasterDataService.UpdateCountry(Country);
                ToastService.ShowSuccess($"Country '{Country.CountryName}' updated successfully", "Updated");
            }
            else
            {
                // Duplicate check
                var countries = MasterDataService.GetAllCountries();
                if (countries.Any(c => c.CountryCode.Equals(Country.CountryCode, StringComparison.OrdinalIgnoreCase)))
                {
                    ToastService.ShowError("Country code already exists", "Validation Error");
                    return;
                }

                if (countries.Any(c => c.ISO2.Equals(Country.ISO2, StringComparison.OrdinalIgnoreCase)))
                {
                    ToastService.ShowError("ISO2 code already exists", "Validation Error");
                    return;
                }

                Country.Id = Guid.NewGuid();
                Country.CreatedAt = DateTime.Now;
                Country.CreatedBy = "System";
                MasterDataService.AddCountry(Country);
                ToastService.ShowSuccess($"Country '{Country.CountryName}' added successfully", "Added");
            }

            // Navigate back to list - Blazor will properly reload the component
            Nav.NavigateTo("/countries");
        }
    }
}
