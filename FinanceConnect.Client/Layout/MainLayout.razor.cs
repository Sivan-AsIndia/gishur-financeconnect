using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static FinanceConnect.Client.Data.MasterDataIds;

namespace FinanceConnect.Client.Layout
{
    public partial class MainLayout
    {
        [Inject] DashboardService DashboardService { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] BranchService BranchService { get; set; } = default!;
        [Inject] SettingsService SettingsService { get; set; } = default!;

        [Inject] IJSRuntime JS { get; set; } = default!;

        List<CompanyModel> Companies = new();
        List<BranchModel> Branches = new();
        List<BranchModel> FilteredBranches = new();
        string SelectedBranchName = string.Empty;
        Guid? selectedCompanyId;
        Guid? SelectedBranchId;
        Guid? previousCompanyId;


        Guid? SelectedCompanyId
        {
            get => selectedCompanyId;
            set
            {
                selectedCompanyId = value;
            }
        }
        protected override async Task OnInitializedAsync()
        {
            AuthService.OnAuthStateChanged += OnAuthStateChanged;
            SettingsService.OnChange += StateHasChanged;
            Navigation.LocationChanged += OnLocationChanged;

            // Restore persisted session (handles page refresh scenarios)
            await AuthService.InitializeAsync();

            if (!AuthService.IsAuthenticated)
            {
                Navigation.NavigateTo("/Login", replace: true);
                return;
            }

            LoadCompanies();
        }

        void LoadCompanies()
        {
            Companies = MasterDataService
                .GetAllCompanies()
                .Where(c => c.Status == "Active")
                .ToList();
            SelectedCompanyId = Companies.FirstOrDefault()?.Id;
            previousCompanyId = SelectedCompanyId;
            if (SelectedCompanyId.HasValue)
            {
                Branches = BranchService
                    .GetByCompanyId(SelectedCompanyId.Value)
                    .Where(b => b.Status == "Active")
                    .ToList();
                FilteredBranches = Branches;

                var Branch = Branches
                .FirstOrDefault(b => b.IsDefaultBranch)
                ?? Branches.FirstOrDefault();
                SelectedBranchId = Branch?.Id;
                SelectedBranchName = Branch?.BranchName;

                SettingsService.SetWorkspace(
                    SelectedCompanyId,
                    SelectedBranchId,
                    SelectedBranchName
                    );
            }
        }

        void FilterBranches(ChangeEventArgs e)
        {
            var search = e.Value?.ToString()?.ToLower() ?? "";

            FilteredBranches = Branches
                .Where(b => b.BranchName.ToLower().Contains(search))
                .ToList();
        }

        private async Task OnCompanyChanged()
        {
            SelectedBranchId = null;
            SelectedBranchName = string.Empty;

            Branches.Clear();

            if (!SelectedCompanyId.HasValue)
                return;

            Branches = BranchService
                .GetByCompanyId(SelectedCompanyId.Value)
                .Where(b => b.Status == "Active")
                .ToList();

            FilteredBranches = Branches;

            var branch = Branches.FirstOrDefault(b => b.IsDefaultBranch);

            previousCompanyId = SelectedCompanyId;

            if (branch is not null)
            {
                SelectedBranchId = branch.Id;
                SelectedBranchName = branch.BranchName;
            }
            else
            {
                // fallback if somehow no default found
                var firstBranch = Branches.FirstOrDefault();
                SelectedBranchId = firstBranch?.Id;
                SelectedBranchName = firstBranch?.BranchName ?? string.Empty;
            }

            await DashboardService.SaveDashboardDataAsync(SelectedCompanyId, SelectedBranchId);
            SettingsService.SetWorkspace(
                SelectedCompanyId,
                SelectedBranchId,
                branch?.BranchName
            );
        }

        //private async Task OpenBranchSelectionPopup()
        //{


        //    if (SelectedCompanyId.HasValue)
        //    {
        //        Branches = BranchService
        //            .GetByCompanyId(SelectedCompanyId.Value)
        //            .Where(b => b.Status == "Active")
        //            .ToList();
        //        FilteredBranches = Branches;
        //        if (Branches != null && Branches.Count == 1)
        //        {
        //            var Branch = Branches.FirstOrDefault();
        //            SelectedBranchId = Branch?.Id;
        //            SelectedBranchName = Branch?.BranchName;
        //        }
        //        else
        //        {
        //            // Open popup if more than one branch
        //            await JS.InvokeVoidAsync("openBranchModal");
        //        }
        //    }

        //}

        private async Task OnBranchChanged()
        {
            SelectedBranchName = Branches
                .FirstOrDefault(b => b.Id == SelectedBranchId)?
                .BranchName ?? string.Empty;
            previousCompanyId = SelectedCompanyId;
            await DashboardService.SaveDashboardDataAsync(SelectedCompanyId, SelectedBranchId);
            SettingsService.SetWorkspace(
                SelectedCompanyId,
                SelectedBranchId,
                SelectedBranchName
            );
        }

        private async Task BindPreviousValue()
        {
            SelectedCompanyId = previousCompanyId;
            await JS.InvokeVoidAsync("closeBranchModal");
        }

        private void OnAuthStateChanged()
        {
            if (!AuthService.IsAuthenticated)
            {
                // User logged out — force redirect to login from any protected page
                InvokeAsync(() => Navigation.NavigateTo("/Login", replace: true));
                return;
            }

            InvokeAsync(StateHasChanged);
        }

        private void OnLocationChanged(object? sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
        {
            // Guard every SPA navigation (including browser back/forward) while unauthenticated
            if (!AuthService.IsAuthenticated)
            {
                InvokeAsync(() => Navigation.NavigateTo("/Login", replace: true));
            }
        }

        public void Dispose()
        {
            AuthService.OnAuthStateChanged -= OnAuthStateChanged;
            SettingsService.OnChange -= StateHasChanged;
            Navigation.LocationChanged -= OnLocationChanged;
        }

        private async Task HandleLogout()
        {
            await AuthService.LogoutAsync();
            Navigation.NavigateTo("/Login");
        }
    }
}
