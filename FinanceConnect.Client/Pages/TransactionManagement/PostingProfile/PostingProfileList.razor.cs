using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.TransactionManagement.PostingProfile
{
    public partial class PostingProfileList
    {
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
        }
        [Inject] PostingRuleService RuleService { get; set; } = default!;
        [Inject] TransactionTypeService TransactionTypeService { get; set; } = default!;

        List<PostingProfileModel> Profiles = new();
        List<PostingProfileModel> FilteredProfiles = new();

        List<CompanyModel> Companies = new();

        PostingProfileModel? SelectedProfile;
        private bool canDeactivate = true;
        private bool canActivate = false;
        private bool canDelete = false;

        private bool isInitialized = false;
        private bool isLoading = false;


        // FILTERS
        string searchText = "";
        int PageSize = 10;
        int CurrentPage = 1;
        int PageWindowSize = 2;
        string selectedStatus = "";
        Guid? selectedCompany = null;

        int TotalPages =>
            FilteredProfiles.Count == 0
                ? 1
                : (int)Math.Ceiling((double)FilteredProfiles.Count / PageSize);

        List<PostingProfileModel> PagedProfiles =>
            FilteredProfiles
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

        IEnumerable<int> VisiblePages
        {
            get
            {
                if (TotalPages <= PageWindowSize)
                    return Enumerable.Range(1, TotalPages);

                int start = Math.Max(1, CurrentPage - (PageWindowSize / 2));
                int end = start + PageWindowSize - 1;

                if (end > TotalPages)
                {
                    end = TotalPages;
                    start = end - PageWindowSize + 1;
                }

                return Enumerable.Range(start, end - start + 1);
            }
        }

        string SelectedStatus
        {
            get => selectedStatus;
            set
            {
                selectedStatus = value;
                ApplyFilters();
            }
        }

        Guid? SelectedCompany
        {
            get => selectedCompany;
            set
            {
                selectedCompany = value;
                ApplyFilters();
            }
        }

        protected override void OnInitialized()
        {
            Companies = ProfileService.GetCompanies();
            LoadProfiles();
        }

        // LOAD & REFRESH
        void LoadProfiles()
        {
            Profiles = ProfileService.GetAll();
            isInitialized = true;
            ApplyFilters();
        }

        void OnRefresh()
        {
            searchText = "";
            selectedStatus = "";
            selectedCompany = null;
        }

        // FILTERING
        void OnSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString() ?? "";
            ApplyFilters();
        }

        void ApplyFilters()
        {
            IEnumerable<PostingProfileModel> query = Profiles;

            // Search: Code or Name
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(p =>
                    p.ProfileCode.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    p.ProfileName.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                );
            }

            // Company filter
            if (selectedCompany.HasValue)
            {
                query = query.Where(p =>
                    p.CompanyId == selectedCompany.Value
                );
            }

            // Status filter
            if (!string.IsNullOrWhiteSpace(selectedStatus))
            {
                bool isActive = selectedStatus == "Active";
                query = query.Where(p => p.IsActive == isActive);
            }

            // Sort by most recent
            FilteredProfiles = query
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToList();

            CurrentPage = 1;
        }

        // PAGINATION
        void OnPageSizeChange(ChangeEventArgs e)
        {
            PageSize = int.Parse(e.Value!.ToString()!);
            CurrentPage = 1;
        }

        void PreviousPage()
        {
            if (CurrentPage > 1)
                CurrentPage--;
        }

        void NextPage()
        {
            if (CurrentPage < TotalPages)
                CurrentPage++;
        }

        void GoToPage(int page)
        {
            if (page < 1 || page > TotalPages)
                return;

            CurrentPage = page;
        }


        // UI HELPERS
        private async Task ViewProfile(PostingProfileModel profile)
        {
            SelectedProfile = profile;
            await JS.InvokeVoidAsync("blazorOffcanvas.show", "viewProfileOffcanvas");
        }

        void SelectProfile(PostingProfileModel profile)
        {
            canActivate = RuleService.GetByProfile(profile.PostingProfileId)?.Any() == true;
            canDelete = TransactionTypeService.GetByProfile(profile.PostingProfileId)?.Any() == false;
            SelectedProfile = profile;
        }

        string GetCompanyName(Guid? companyId)
        {
            return Companies.FirstOrDefault(c => c.Id == companyId)?.LegalName ?? "—";
        }

        // ACTIONS
        void ActivateConfirmed()
        {
            if (SelectedProfile == null) return;

            ProfileService.Activate(SelectedProfile.PostingProfileId);
            ToastService.ShowSuccess(
                $"Posting profile '{SelectedProfile.ProfileName}' activated successfully",
                "Activated");
            LoadProfiles();
        }

        void DeactivateConfirmed()
        {
            if (SelectedProfile == null) return;

            ProfileService.Deactivate(SelectedProfile.PostingProfileId);
            ToastService.ShowSuccess(
                $"Posting profile '{SelectedProfile.ProfileName}' deactivated successfully",
                "Deactivated");
            LoadProfiles();
        }

        void DeleteConfirmed()
        {
            if (SelectedProfile == null) return;

            ProfileService.Delete(SelectedProfile.PostingProfileId);
            ToastService.ShowSuccess("Posting profile deleted");
            LoadProfiles();
        }

        private async Task OnRefreshAsync()
        {
            isLoading = true;
            StateHasChanged();

            await Task.Delay(300);
            OnRefresh();
            ProfileService.ResetToSeed();
            LoadProfiles();

            isLoading = false;
            await JS.InvokeVoidAsync("feather.replace");
            ToastService.ShowInfo("Posting profile list refreshed", "Refreshed");
        }
    }

}

