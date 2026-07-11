using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.RegularExpressions;

namespace FinanceConnect.Client.Pages.Master.City
{
    public partial class CityList
    {
        // Injected services are defined in the .razor file via @inject directives

        private bool isInitialized = false;
        private bool isLoading = false;

        private List<CityModel> Cities = new();
        private List<CityModel> FilteredCities = new();
        private List<CountryModel> Countries = new();
        private List<StateProvinceModel> States = new();
        private List<StateProvinceModel> FilteredStatesForDropdown = new();
        private CityModel? SelectedCity;
        private bool canDeactivate = true;
        private bool canDelete = true;

        private string searchText = "";
        private string selectedStatus = "";
        private string selectedCountryId = "";
        private string selectedStateId = "";
        private int VisibleColumnCount;
        private int TotalPages => FilteredCities.Count == 0 ? 1 : (int)Math.Ceiling((double)FilteredCities.Count / PageSize);

        private List<CityModel> PagedCities => FilteredCities.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

        protected override async Task OnInitializedAsync()
        {
            LoadData();
            isInitialized = true;
        }

        protected override void OnParametersSet()
        {
            // Reload data every time we navigate to this page
            // This ensures the list is updated after Add/Edit/Delete operations
            if (isInitialized)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            // Load current cities without resetting to seed data
            // (Reset only happens when user clicks Refresh button)
            Cities = MasterDataService.GetAllCities();
            Countries = MasterDataService.GetAllCountries().Where(c => c.IsActive).ToList();
            States = MasterDataService.GetAllStateProvinces().Where(s => s.IsActive).ToList();
            // Sort by most recent first (UpdatedAt if exists, otherwise CreatedAt)
            FilteredCities = Cities
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToList();
        }

        // Dynamic filter lists derived from table data
        private List<CountryModel> TableCountries => Countries
            .Where(c => Cities.Any(ci => ci.CountryId == c.Id))
            .OrderBy(c => c.CountryName)
            .ToList();

        private List<StateProvinceModel> TableStatesForDropdown
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(selectedCountryId) && Guid.TryParse(selectedCountryId, out var countryGuid))
                {
                    return States
                        .Where(s => s.CountryId == countryGuid && Cities.Any(ci => ci.StateProvinceId == s.Id))
                        .OrderBy(s => s.StateProvinceName)
                        .ToList();
                }
                return States
                    .Where(s => Cities.Any(ci => ci.StateProvinceId == s.Id))
                    .OrderBy(s => s.StateProvinceName)
                    .ToList();
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
            VisibleColumnCount =
 await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        private string SelectedStatus
        {
            get => selectedStatus;
            set { selectedStatus = value; ApplyFilters(); }
        }

        private string SelectedCountryId
        {
            get => selectedCountryId;
            set
            {
                selectedCountryId = value;
                // Update state filter dropdown
                if (!string.IsNullOrWhiteSpace(value) && Guid.TryParse(value, out var countryGuid))
                {
                    FilteredStatesForDropdown = States.Where(s => s.CountryId == countryGuid).ToList();
                }
                else
                {
                    FilteredStatesForDropdown = new();
                }
                selectedStateId = ""; // Reset state selection
                ApplyFilters();
            }
        }

        private string SelectedStateId
        {
            get => selectedStateId;
            set { selectedStateId = value; ApplyFilters(); }
        }


        private string GetStatusBadgeClass(string status)
        {
            return status switch
            {
                "Active" => "bg-success-transparent text-success",
                "Inactive" => "bg-danger-transparent text-danger",
                "Draft" => "bg-warning-transparent text-warning",
                _ => "bg-secondary-transparent text-secondary"
            };
        }

        private async Task OnSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString() ?? "";
            ApplyFilters();
            VisibleColumnCount =
 await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        private void ApplyFilters()
        {
            IEnumerable<CityModel> query = Cities;

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(c =>
                    c.CityCode.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    c.CityName.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                );
            }

            if (!string.IsNullOrWhiteSpace(selectedCountryId) && Guid.TryParse(selectedCountryId, out var countryId))
            {
                query = query.Where(c => c.CountryId == countryId);
            }

            if (!string.IsNullOrWhiteSpace(selectedStateId) && Guid.TryParse(selectedStateId, out var stateId))
            {
                query = query.Where(c => c.StateProvinceId == stateId);
            }

            if (!string.IsNullOrWhiteSpace(selectedStatus))
            {
                query = query.Where(c => c.Status == selectedStatus);
            }

            // Sort by most recent first (UpdatedAt if exists, otherwise CreatedAt)
            FilteredCities = query
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToList();
            CurrentPage = 1;
        }

        private async Task OnPageSizeChange(ChangeEventArgs e)
        {
            isLoading = true;
            StateHasChanged();
            await Task.Delay(200);

            PageSize = int.Parse(e.Value?.ToString() ?? "10");
            CurrentPage = 1;

            isLoading = false;
            StateHasChanged();
        }

        private async Task GoToPage(int page)
        {
            if (page >= 1 && page <= TotalPages)
            {
                isLoading = true;
                StateHasChanged();
                await Task.Delay(200);

                CurrentPage = page;

                isLoading = false;
                StateHasChanged();
            }
        }

        int PageWindowSize = 2;
        int EndPage => Math.Min(StartPage + PageWindowSize - 1, TotalPages);
        int StartPage = 1;
        private int CurrentPage = 1;
        private int PageSize = 10;

        private async Task PreviousPage()
        {
            if (CurrentPage > 1) await GoToPage(CurrentPage - 1);
        }

        private async Task NextPage()
        {
            if (CurrentPage < TotalPages) await GoToPage(CurrentPage + 1);
        }



        private async Task OnRefreshAsync()
        {
            isLoading = true;
            StateHasChanged();
            await Task.Delay(150);

            searchText = "";
            selectedStatus = "";
            selectedCountryId = "";
            selectedStateId = "";
            MasterDataService.ResetCitiesToSeed();
            Cities = MasterDataService.GetAllCities();
            Countries = MasterDataService.GetAllCountries().Where(c => c.IsActive).ToList();
            States = MasterDataService.GetAllStateProvinces().Where(s => s.IsActive).ToList();
            // Sort by most recent first (UpdatedAt if exists, otherwise CreatedAt)
            FilteredCities = Cities
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToList();
            CurrentPage = 1;
            ToastService.ShowInfo("Data refreshed", "Refresh");

            isLoading = false;
            StateHasChanged();
        }

        private async Task ViewCity(CityModel city)
        {
            isLoading = true;
            StateHasChanged();
            await Task.Delay(200);

            SelectedCity = city;

            isLoading = false;
            StateHasChanged();
            await JS.InvokeVoidAsync("blazorOffcanvas.show", "viewCityOffcanvas");
        }

        private void ConfirmActivate(CityModel city)
        {
            SelectedCity = city;
        }

        private void ActivateConfirmed()
        {
            if (SelectedCity != null)
            {
                MasterDataService.ActivateCity(SelectedCity.Id);
                Cities = MasterDataService.GetAllCities();
                ApplyFilters();
                ToastService.ShowSuccess($"City '{SelectedCity.CityName}' activated successfully", "Activated");
                SelectedCity = null;
            }
        }

        private void ConfirmDeactivate(CityModel city)
        {
            SelectedCity = city;
            canDeactivate = MasterDataService.CanDeactivateCity(city.Id);
        }

        private void DeactivateConfirmed()
        {
            if (SelectedCity != null && canDeactivate)
            {
                MasterDataService.DeactivateCity(SelectedCity.Id);
                Cities = MasterDataService.GetAllCities();
                ApplyFilters();
                ToastService.ShowWarning($"City '{SelectedCity.CityName}' deactivated successfully", "Deactivated");
                SelectedCity = null;
            }
        }

        private void ConfirmDelete(CityModel city)
        {
            SelectedCity = city;
            canDelete = MasterDataService.CanDeleteCity(city.Id);
        }

        private void DeleteConfirmed()
        {
            if (SelectedCity != null && canDelete)
            {
                MasterDataService.DeleteCity(SelectedCity.Id);
                Cities = MasterDataService.GetAllCities();
                // Sort by most recent first (UpdatedAt if exists, otherwise CreatedAt)
                FilteredCities = Cities
                    .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                    .ToList();
                ToastService.ShowError($"City '{SelectedCity.CityName}' deleted successfully", "Deleted");
                SelectedCity = null;
                CurrentPage = 1;
            }
        }
        private CityModel? SelectedCompany;

        void OpenRowDetails(CityModel company)
        {
            SelectedCompany = company;
        }

        private string GetStatusBadge(string status)
        {
            return status switch
            {
                "Active" => "bg-success-transparent text-success",
                "Inactive" => "bg-danger-transparent text-danger",
                "Draft" => "bg-warning-transparent text-warning",
                _ => "bg-secondary-transparent text-secondary"
            };
        }
        private string GetStatusDotBadge(string status)
        {
            return status switch
            {
                "Active" => "bg-success text-success",
                "Inactive" => "bg-danger text-danger",
                "Draft" => "bg-warning text-warning",
                _ => "bg-secondary text-secondary"
            };
        }

        private string GetPlainText(string? html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return "-";

            return Regex.Replace(html, "<.*?>", string.Empty);
        }

    }
}
