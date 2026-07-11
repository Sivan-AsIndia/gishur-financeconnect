using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Master.Country
{
    public partial class CountryList
    {
        // Injected services are defined in the .razor file via @inject directives

        private bool isInitialized = false;
        private bool isLoading = false;

        private List<CountryModel> Countries = new();
        private List<CountryModel> FilteredCountries = new();
        private CountryModel? SelectedCountry;
        private bool canDeactivate = true;
        private bool canDelete = true;
        private int VisibleColumnCount;
        private string searchText = "";
        private string selectedStatus = "";
        private string selectedRegion = "";

        private int TotalPages => FilteredCountries.Count == 0 ? 1 : (int)Math.Ceiling((double)FilteredCountries.Count / PageSize);

        private List<CountryModel> PagedCountries => FilteredCountries.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

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

        private void LoadData()
        {
            // Load current countries without resetting to seed data
            // (Reset only happens when user clicks Refresh button)
            Countries = MasterDataService.GetAllCountries();
            // Sort by most recent first (UpdatedAt if exists, otherwise CreatedAt)
            FilteredCountries = Countries
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToList();
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

        // Dynamic regions derived from table data (not static list)
        private List<string> TableRegions => Countries
            .Where(c => !string.IsNullOrWhiteSpace(c.Region))
            .Select(c => c.Region!)
            .Distinct()
            .OrderBy(r => r)
            .ToList();

        private string SelectedRegion
        {
            get => selectedRegion;
            set { selectedRegion = value; ApplyFilters(); }
        }

        private void OnSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString() ?? "";
            ApplyFilters();
        }

        private async Task ApplyFilters()
        {
            IEnumerable<CountryModel> query = Countries;

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(c =>
                    c.CountryCode.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    c.CountryName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    (c.ISO2?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (c.ISO3?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false)
                );
                VisibleColumnCount =
await JS.InvokeAsync<int>("getVisibleTableColumns");
            }

            if (!string.IsNullOrWhiteSpace(selectedRegion))
            {
                query = query.Where(c => c.Region == selectedRegion);
            }

            if (!string.IsNullOrWhiteSpace(selectedStatus))
            {
                bool isActive = selectedStatus == "Active";
                query = query.Where(c => c.IsActive == isActive);
            }

            // Sort by most recent first (UpdatedAt if exists, otherwise CreatedAt)
            FilteredCountries = query
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
            selectedRegion = "";
            MasterDataService.ResetCountriesToSeed();
            Countries = MasterDataService.GetAllCountries();
            // Sort by most recent first (UpdatedAt if exists, otherwise CreatedAt)
            FilteredCountries = Countries
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToList();
            CurrentPage = 1;
            ToastService.ShowInfo("Data refreshed", "Refresh");

            isLoading = false;
            StateHasChanged();
        }

        private async Task ViewCountry(CountryModel country)
        {
            isLoading = true;
            StateHasChanged();
            await Task.Delay(200);

            SelectedCountry = country;

            isLoading = false;
            StateHasChanged();

            await JS.InvokeVoidAsync("blazorOffcanvas.show", "viewCountryOffcanvas");
        }

        private void ConfirmActivate(CountryModel country)
        {
            SelectedCountry = country;
        }

        private void ActivateConfirmed()
        {
            if (SelectedCountry != null)
            {
                MasterDataService.ActivateCountry(SelectedCountry.Id);
                Countries = MasterDataService.GetAllCountries();
                ApplyFilters();
                ToastService.ShowSuccess($"Country '{SelectedCountry.CountryName}' activated successfully", "Activated");
                SelectedCountry = null;
            }
        }

        private void ConfirmDeactivate(CountryModel country)
        {
            SelectedCountry = country;
            canDeactivate = MasterDataService.CanDeactivateCountry(country.Id);
        }

        private void DeactivateConfirmed()
        {
            if (SelectedCountry != null && canDeactivate)
            {
                MasterDataService.DeactivateCountry(SelectedCountry.Id);
                Countries = MasterDataService.GetAllCountries();
                ApplyFilters();
                ToastService.ShowWarning($"Country '{SelectedCountry.CountryName}' deactivated successfully", "Deactivated");
                SelectedCountry = null;
            }
        }

        private void ConfirmDelete(CountryModel country)
        {
            SelectedCountry = country;
            canDelete = MasterDataService.CanDeleteCountry(country.Id);
        }

        private void DeleteConfirmed()
        {
            if (SelectedCountry != null && canDelete)
            {
                MasterDataService.DeleteCountry(SelectedCountry.Id);
                Countries = MasterDataService.GetAllCountries();
                // Sort by most recent first (UpdatedAt if exists, otherwise CreatedAt)
                FilteredCountries = Countries
                    .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                    .ToList();
                ToastService.ShowError($"Country '{SelectedCountry.CountryName}' deleted successfully", "Deleted");
                SelectedCountry = null;
                CurrentPage = 1;
            }
        }
        private CountryModel? SelectedCompany;

        void OpenRowDetails(CountryModel company)
        {
            SelectedCompany = company;
        }
    }

    // Region types for countries
    public static class Regions
    {
        public const string Asia = "Asia";
        public const string Europe = "Europe";
        public const string NorthAmerica = "North America";
        public const string SouthAmerica = "South America";
        public const string Africa = "Africa";
        public const string Oceania = "Oceania";
        public const string MiddleEast = "Middle East";

        public static readonly string[] All = new[] { Asia, Europe, NorthAmerica, SouthAmerica, Africa, Oceania, MiddleEast };
    }
}
