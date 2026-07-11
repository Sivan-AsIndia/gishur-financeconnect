using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Master.StateProvince
{
    public partial class StateProvinceList
    {
        // Injected services are defined in the .razor file via @inject directives

        private bool isInitialized = false;
        private bool isLoading = false;

        private List<StateProvinceModel> States = new();
        private List<StateProvinceModel> FilteredStates = new();
        private List<CountryModel> Countries = new();
        private List<TimeZoneModel> TimeZones = new();
        private StateProvinceModel? SelectedState;
        private bool canDeactivate = true;
        private bool canDelete = true;

        private string searchText = "";
        private string selectedStatus = "";
        private string selectedCountryId = "";
        private string selectedJurisdictionType = "";
        private int VisibleColumnCount;
        private int TotalPages => FilteredStates.Count == 0 ? 1 : (int)Math.Ceiling((double)FilteredStates.Count / PageSize);

        private List<StateProvinceModel> PagedStates => FilteredStates.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

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
            // Load current states without resetting to seed data
            // (Reset only happens when user clicks Refresh button)
            States = MasterDataService.GetAllStateProvinces();
            Countries = MasterDataService.GetAllCountries();
            TimeZones = MasterDataService.GetAllTimeZones();
            FilteredStates = States.OrderByDescending(s => s.UpdatedAt ?? s.CreatedAt).ToList();
        }

        // Dynamic filter lists derived from table data
        private List<CountryModel> TableCountries => Countries
            .Where(c => States.Any(s => s.CountryId == c.Id))
            .OrderBy(c => c.CountryName)
            .ToList();

        private List<string> TableJurisdictionTypes => States
            .Where(s => !string.IsNullOrWhiteSpace(s.JurisdictionType))
            .Select(s => s.JurisdictionType)
            .Distinct()
            .OrderBy(j => j)
            .ToList();

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
            set { selectedCountryId = value; ApplyFilters(); }
        }

        private string SelectedJurisdictionType
        {
            get => selectedJurisdictionType;
            set { selectedJurisdictionType = value; ApplyFilters(); }
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
            IEnumerable<StateProvinceModel> query = States;

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(s =>
                    s.StateProvinceCode.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    s.StateProvinceName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    (s.GSTStateCode?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            if (!string.IsNullOrWhiteSpace(selectedCountryId) && Guid.TryParse(selectedCountryId, out var countryId))
            {
                query = query.Where(s => s.CountryId == countryId);
            }

            if (!string.IsNullOrWhiteSpace(selectedJurisdictionType))
            {
                query = query.Where(s => s.JurisdictionType == selectedJurisdictionType);
            }

            if (!string.IsNullOrWhiteSpace(selectedStatus))
            {
                query = query.Where(s => s.Status == selectedStatus);
            }

            FilteredStates = query.OrderByDescending(s => s.UpdatedAt ?? s.CreatedAt).ToList();
            CurrentPage = 1;
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

        private string? GetTimeZoneName(Guid? timeZoneId)
        {
            if (!timeZoneId.HasValue) return null;
            return TimeZones.FirstOrDefault(t => t.Id == timeZoneId.Value)?.DisplayName;
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
            selectedJurisdictionType = "";
            MasterDataService.ResetStateProvincesToSeed();
            States = MasterDataService.GetAllStateProvinces();
            FilteredStates = States.OrderByDescending(s => s.UpdatedAt ?? s.CreatedAt).ToList();
            CurrentPage = 1;
            ToastService.ShowInfo("Data refreshed", "Refresh");

            isLoading = false;
            StateHasChanged();
        }

        private async Task ViewState(StateProvinceModel state)
        {
            isLoading = true;
            StateHasChanged();
            await Task.Delay(200);

            SelectedState = state;

            isLoading = false;
            StateHasChanged();

            await JS.InvokeVoidAsync("blazorOffcanvas.show", "viewStateOffcanvas");
        }

        private void ConfirmActivate(StateProvinceModel state)
        {
            SelectedState = state;
        }

        private void ActivateConfirmed()
        {
            if (SelectedState != null)
            {
                MasterDataService.ActivateStateProvince(SelectedState.Id);
                States = MasterDataService.GetAllStateProvinces();
                ApplyFilters();
                ToastService.ShowSuccess($"State/Province '{SelectedState.StateProvinceName}' activated successfully", "Activated");
                SelectedState = null;
            }
        }

        private void ConfirmDeactivate(StateProvinceModel state)
        {
            SelectedState = state;
            canDeactivate = MasterDataService.CanDeactivateStateProvince(state.Id);
        }

        private void DeactivateConfirmed()
        {
            if (SelectedState != null && canDeactivate)
            {
                MasterDataService.DeactivateStateProvince(SelectedState.Id);
                States = MasterDataService.GetAllStateProvinces();
                ApplyFilters();
                ToastService.ShowWarning($"State/Province '{SelectedState.StateProvinceName}' deactivated successfully", "Deactivated");
                SelectedState = null;
            }
        }

        private void ConfirmDelete(StateProvinceModel state)
        {
            SelectedState = state;
            canDelete = MasterDataService.CanDeleteStateProvince(state.Id);
        }

        private StateProvinceModel? SelectedCompany;

        void OpenRowDetails(StateProvinceModel company)
        {
            SelectedCompany = company;
        }
        private void DeleteConfirmed()
        {
            if (SelectedState != null && canDelete)
            {
                MasterDataService.DeleteStateProvince(SelectedState.Id);
                States = MasterDataService.GetAllStateProvinces();
                FilteredStates = States.OrderByDescending(s => s.UpdatedAt ?? s.CreatedAt).ToList();
                ToastService.ShowError($"State/Province '{SelectedState.StateProvinceName}' deleted successfully", "Deleted");
                SelectedState = null;
                CurrentPage = 1;
            }
        }
    }
}
