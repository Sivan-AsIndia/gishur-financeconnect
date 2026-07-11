using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.RegularExpressions;

namespace FinanceConnect.Client.Pages.Master.TimeZone
{
    public partial class TimeZoneList
    {
        // Injected services are defined in the .razor file via @inject directives

        private bool isInitialized = false;
        private bool isLoading = false;

        private List<TimeZoneModel> TimeZones = new();
        private List<TimeZoneModel> FilteredTimeZones = new();
        private List<CountryModel> Countries = new();
        private TimeZoneModel? SelectedTimeZone;
        private bool canDeactivate = true;
        private bool canDelete = true;

        private string searchText = "";
        private string selectedStatus = "";
        private string selectedCountryId = "";
        private string selectedDST = "";

        private int TotalPages => FilteredTimeZones.Count == 0 ? 1 : (int)Math.Ceiling((double)FilteredTimeZones.Count / PageSize);

        private List<TimeZoneModel> PagedTimeZones => FilteredTimeZones.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

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
            // Load current data without resetting to seed
            // (Reset only happens when user clicks Refresh button)
            TimeZones = MasterDataService.GetAllTimeZones();
            Countries = MasterDataService.GetAllCountries().Where(c => c.IsActive).ToList();
            // Sort by most recent first (UpdatedAt if exists, otherwise CreatedAt)
            FilteredTimeZones = TimeZones
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToList();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
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

        private string SelectedDST
        {
            get => selectedDST;
            set { selectedDST = value; ApplyFilters(); }
        }

        private void OnSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString() ?? "";
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            IEnumerable<TimeZoneModel> query = TimeZones;

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(t =>
                    t.TimeZoneKey.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    t.DisplayName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    (t.ShortName?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            if (!string.IsNullOrWhiteSpace(selectedStatus))
            {
                query = query.Where(t => t.Status == selectedStatus);
            }

            if (!string.IsNullOrWhiteSpace(selectedCountryId) && Guid.TryParse(selectedCountryId, out var countryGuid))
            {
                query = query.Where(t => t.CountryId == countryGuid);
            }

            if (!string.IsNullOrWhiteSpace(selectedDST))
            {
                bool supportsDST = selectedDST == "yes";
                query = query.Where(t => t.SupportsDST == supportsDST);
            }

            // Sort by most recent first (UpdatedAt if exists, otherwise CreatedAt)
            FilteredTimeZones = query
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
            selectedDST = "";
            MasterDataService.ResetTimeZonesToSeed();
            TimeZones = MasterDataService.GetAllTimeZones();
            // Sort by most recent first (UpdatedAt if exists, otherwise CreatedAt)
            FilteredTimeZones = TimeZones
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToList();
            CurrentPage = 1;
            ToastService.ShowInfo("Data refreshed", "Refresh");

            isLoading = false;
            StateHasChanged();
        }

        private async Task ViewTimeZone(TimeZoneModel tz)
        {
            isLoading = true;
            StateHasChanged();
            await Task.Delay(200);

            SelectedTimeZone = tz;

            isLoading = false;
            StateHasChanged();
            await JS.InvokeVoidAsync("blazorOffcanvas.show", "viewTimeZoneOffcanvas");
        }

        private void ConfirmActivate(TimeZoneModel tz)
        {
            SelectedTimeZone = tz;
        }

        private void ActivateConfirmed()
        {
            if (SelectedTimeZone != null)
            {
                MasterDataService.ActivateTimeZone(SelectedTimeZone.Id);
                TimeZones = MasterDataService.GetAllTimeZones();
                ApplyFilters();
                ToastService.ShowSuccess($"Time Zone '{SelectedTimeZone.DisplayName}' activated successfully", "Activated");
                SelectedTimeZone = null;
            }
        }

        private void ConfirmDeactivate(TimeZoneModel tz)
        {
            SelectedTimeZone = tz;
            canDeactivate = MasterDataService.CanDeactivateTimeZone(tz.Id);
        }

        private void DeactivateConfirmed()
        {
            if (SelectedTimeZone != null && canDeactivate)
            {
                MasterDataService.DeactivateTimeZone(SelectedTimeZone.Id);
                TimeZones = MasterDataService.GetAllTimeZones();
                ApplyFilters();
                ToastService.ShowWarning($"Time Zone '{SelectedTimeZone.DisplayName}' deactivated successfully", "Deactivated");
                SelectedTimeZone = null;
            }
        }

        private void ConfirmDelete(TimeZoneModel tz)
        {
            SelectedTimeZone = tz;
            canDelete = MasterDataService.CanDeleteTimeZone(tz.Id);
        }

        private void DeleteConfirmed()
        {
            if (SelectedTimeZone != null && canDelete)
            {
                MasterDataService.DeleteTimeZone(SelectedTimeZone.Id);
                TimeZones = MasterDataService.GetAllTimeZones();
                // Sort by most recent first (UpdatedAt if exists, otherwise CreatedAt)
                FilteredTimeZones = TimeZones
                    .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                    .ToList();
                ToastService.ShowError($"Time Zone '{SelectedTimeZone.DisplayName}' deleted successfully", "Deleted");
                SelectedTimeZone = null;
                CurrentPage = 1;
            }
        }

        private string GetCountryName(Guid? countryId)
        {
            if (!countryId.HasValue) return "-";
            var country = Countries.FirstOrDefault(c => c.Id == countryId.Value);
            return country?.CountryName ?? "-";
        }

        private string FormatOffset(int? minutes)
        {
            if (minutes == null)
                return "-";

            var totalMinutes = minutes.Value;
            var hours = totalMinutes / 60;
            var mins = Math.Abs(totalMinutes % 60);
            var sign = hours >= 0 ? "+" : "-";

            return $"UTC{sign}{Math.Abs(hours):D2}:{mins:D2}";
        }


        private TimeZoneModel? SelectedCompany;
        void OpenRowDetails(TimeZoneModel company)
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
