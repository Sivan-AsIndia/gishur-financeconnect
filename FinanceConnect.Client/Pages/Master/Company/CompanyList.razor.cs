using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Master.Company
{
    public partial class CompanyList
    {
        private bool isInitialized = false;
        private bool isLoading = false;

        private List<CompanyModel> Companies = new();
        private List<CompanyModel> FilteredCompanies = new();
        private List<CountryModel> Countries = new();

        private CompanyModel? SelectedCompany;

        private bool canDeactivate = true;
        private bool canDelete = true;

        private string searchText = "";
        private int PageSize = 10;
        private int CurrentPage = 1;
        private string selectedStatus = "";
        private string selectedCountryId = "";
        private string selectedLegalStructure = "";
        private int VisibleColumnCount;
        private int TotalPages => FilteredCompanies.Count == 0 ? 1 : (int)Math.Ceiling((double)FilteredCompanies.Count / PageSize);

        private List<CompanyModel> PagedCompanies => FilteredCompanies.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

        protected override async Task OnInitializedAsync()
        {
            Countries = MasterDataService.GetAllCountries().Where(c => c.IsActive).ToList();

            // Load current companies without resetting to seed data
            // (Reset only happens when user clicks Refresh button)
            Companies = MasterDataService.GetAllCompanies();
            // Sort by most recent first (UpdatedAt if exists, otherwise CreatedAt)
            FilteredCompanies = Companies
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToList();

            isInitialized = true;
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
            set { selectedCountryId = value; ApplyFilters(); }
        }

        private string SelectedLegalStructure
        {
            get => selectedLegalStructure;
            set { selectedLegalStructure = value; ApplyFilters(); }
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
            IEnumerable<CompanyModel> query = Companies;

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(c =>
                    c.CompanyCode.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    c.LegalName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    (c.TradeName?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (c.ShortName?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            if (!string.IsNullOrWhiteSpace(selectedCountryId) && Guid.TryParse(selectedCountryId, out var countryId))
            {
                query = query.Where(c => c.CountryId == countryId);
            }

            if (!string.IsNullOrWhiteSpace(selectedLegalStructure))
            {
                query = query.Where(c => c.LegalStructure == selectedLegalStructure);
            }

            if (!string.IsNullOrWhiteSpace(selectedStatus))
            {
                query = query.Where(c => c.Status == selectedStatus);
            }

            // Sort by most recent first (UpdatedAt if exists, otherwise CreatedAt)
            FilteredCompanies = query
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
            selectedLegalStructure = "";
            MasterDataService.ResetCompaniesToSeed();
            Companies = MasterDataService.GetAllCompanies();
            // Sort by most recent first (UpdatedAt if exists, otherwise CreatedAt)
            FilteredCompanies = Companies
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToList();
            CurrentPage = 1;
            ToastService.ShowInfo("Data refreshed", "Refresh");

            isLoading = false;
            StateHasChanged();
        }

        private async Task ViewCompany(CompanyModel company)
        {
            isLoading = true;
            StateHasChanged();
            await Task.Delay(200);

            SelectedCompany = company;

            isLoading = false;

            StateHasChanged();

            Nav.NavigateTo($"/companies/{company.Id}/view");
        }

        private void ConfirmActivate(CompanyModel company) => SelectedCompany = company;

        private void ActivateConfirmed()
        {
            if (SelectedCompany != null)
            {
                MasterDataService.ActivateCompany(SelectedCompany.Id);
                Companies = MasterDataService.GetAllCompanies();
                ApplyFilters();
                ToastService.ShowSuccess($"Company '{SelectedCompany.LegalName}' activated successfully", "Activated");
                SelectedCompany = null;
            }
        }

        private void ConfirmDeactivate(CompanyModel company)
        {
            SelectedCompany = company;
            canDeactivate = MasterDataService.CanDeactivateCompany(company.Id);
        }

        private void DeactivateConfirmed()
        {
            if (SelectedCompany != null && canDeactivate)
            {
                MasterDataService.DeactivateCompany(SelectedCompany.Id);
                Companies = MasterDataService.GetAllCompanies();
                ApplyFilters();
                ToastService.ShowWarning($"Company '{SelectedCompany.LegalName}' deactivated successfully", "Deactivated");
                SelectedCompany = null;
            }
        }

        private void ConfirmDelete(CompanyModel company)
        {
            SelectedCompany = company;
            // Check if company has any branches mapped
            var branches = BranchService.GetAll().Where(b => b.CompanyId == company.Id).ToList();
            canDelete = !branches.Any();
        }

        private void DeleteConfirmed()
        {
            if (SelectedCompany != null && canDelete)
            {
                MasterDataService.DeleteCompany(SelectedCompany.Id);
                Companies = MasterDataService.GetAllCompanies();
                // Sort by most recent first (UpdatedAt if exists, otherwise CreatedAt)
                FilteredCompanies = Companies
                    .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                    .ToList();
                ToastService.ShowError($"Company '{SelectedCompany.LegalName}' deleted successfully", "Deleted");
                SelectedCompany = null;
                CurrentPage = 1;
            }
        }

        private string GetStatusBadge(string status)
        {
            return status switch
            {
                "Active" => "bg-success text-white",
                "Inactive" => "bg-danger text-white",
                "Draft" => "bg-warning text-dark",
                _ => "bg-secondary text-white"
            };
        }


        private string GetMonthName(int month)
        {
            return month switch
            {
                1 => "January",
                2 => "February",
                3 => "March",
                4 => "April",
                5 => "May",
                6 => "June",
                7 => "July",
                8 => "August",
                9 => "September",
                10 => "October",
                11 => "November",
                12 => "December",
                _ => "-"
            };
        }


        void OpenRowDetails(CompanyModel company)
        {
            SelectedCompany = company;
        }
    }
}
