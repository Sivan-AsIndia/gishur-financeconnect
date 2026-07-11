using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Master.Branch
{
    public partial class BranchList
    {
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
            VisibleColumnCount =
             await JS.InvokeAsync<int>("getVisibleTableColumns");
        }
        [Inject] MasterDataService MasterDataService { get; set; } = default!;

        List<BranchModel> Branches = new();
        List<BranchModel> FilteredBranches = new();
        public List<CompanyModel> Companies = new();
        string searchText = "";
        int PageSize = 10;
        int CurrentPage = 1;
        int PageWindowSize = 2;
        string selectedStatus = "";
        Guid? selectedCompany = null;
        List<string> CompanyList = new();
        BranchModel? SelectedBranch;
        private bool canDeactivate = true;
        private bool canDelete = true;
        private int VisibleColumnCount;
        private bool isInitialized = false;
        private bool isLoading = false;
        int TotalPages =>
            FilteredBranches.Count == 0
                ? 1
                : (int)Math.Ceiling((double)FilteredBranches.Count / PageSize);

        List<BranchModel> PagedBranches =>
            FilteredBranches
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

        protected override void OnInitialized()
        {
            Companies = MasterDataService.GetAllCompanies().Where(c => c.Status == "Active").ToList();
            Branches = BranchService.GetAll();
            // Sort by most recent first (UpdatedAt if exists, otherwise CreatedAt)
            FilteredBranches = Branches
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToList();
            isInitialized = true;
        }

        async Task OnSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString() ?? "";
            ApplyFilters();
            VisibleColumnCount =
    await JS.InvokeAsync<int>("getVisibleTableColumns");
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



        void ViewBranch(BranchModel branch)
        {
            SelectedBranch = branch;
            Nav.NavigateTo($"/branches/{branch.Id}/view");
        }
        void ApplyFilters()
        {
            IEnumerable<BranchModel> query = Branches;

            // Search
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(b =>
                    (b.BranchCode ?? "").Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    (b.BranchName ?? "").Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    (b.Company ?? "").Contains(searchText, StringComparison.OrdinalIgnoreCase) 
                    //(b.City ?? "").Contains(searchText, StringComparison.OrdinalIgnoreCase)
                );
            }

            // Company filter
            if (selectedCompany.HasValue)
            {
                query = query.Where(b =>
                     b.CompanyId!=null &&
                     b.CompanyId == selectedCompany.Value);
            }
            // Status filter
            if (!string.IsNullOrWhiteSpace(selectedStatus))
            {
                query = query.Where(b => b.Status == selectedStatus);
            }

            // Sort by most recent first (UpdatedAt if exists, otherwise CreatedAt)
            FilteredBranches = query
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToList();
        }



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


        private async Task OnRefreshAsync()
        {
            searchText = "";
            selectedStatus = "";
            selectedCompany = null;
            isLoading = true;
            StateHasChanged();

            await Task.Delay(300);
            BranchService.ResetToSeed();
            Branches = BranchService.GetAll();
            FilteredBranches = Branches
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToList();
            CurrentPage = 1;

            isLoading = false;
            await JS.InvokeVoidAsync("feather.replace");
            ToastService.ShowInfo("Branch List refreshed", "Refresh");

        }


        void ConfirmDelete(BranchModel branch)
        {
            SelectedBranch = branch;
            canDelete = CanDeleteBranch(branch.Id); 
        }

        public bool CanDeleteBranch(Guid id)
        {
            var entries = MasterDataService.GetGeneralLedgerEntriesByBranch(id);

            return !entries.Any();
        }
        async Task DeleteConfirmed()
        {
            if (SelectedBranch == null)
                return;

            BranchService.Delete(SelectedBranch.Id);
            ToastService.ShowError($"{SelectedBranch.BranchName} Deleted Successfully", "Deleted");
            Branches = BranchService.GetAll();
            // Sort by most recent first (UpdatedAt if exists, otherwise CreatedAt)
            FilteredBranches = Branches
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToList();
            SelectedBranch = null;

            CurrentPage = 1;

            await JS.InvokeVoidAsync("closeDeleteModal");

        }

        private void ConfirmActivate(BranchModel branch)
            => SelectedBranch = branch;
        private void ActivateConfirmed()
        {
            if (SelectedBranch != null)
            {
                BranchService.ActivateBranch(SelectedBranch.Id);

                Branches = BranchService.GetAll();
                ApplyFilters();

                ToastService.ShowSuccess(
                    $"Branch '{SelectedBranch.BranchName}' activated successfully",
                    "Activated");

                SelectedBranch = null;
            }
        }
        private void ConfirmDeactivate(BranchModel branch)
        {
            SelectedBranch = branch;
            canDeactivate = BranchService.CanDeactivateBranch(branch.Id);
        }

        private void DeactivateConfirmed()
        {
            if (SelectedBranch != null && canDeactivate)
            {
                BranchService.DeactivateBranch(SelectedBranch.Id);

                Branches = BranchService.GetAll();
                ApplyFilters();
                ToastService.ShowWarning($"Branch '{SelectedBranch.BranchName}' deactivated successfully", "Deactivated");
                SelectedBranch = null;
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

        void OpenRowDetails(BranchModel branch)
        {
            SelectedBranch = branch;
        }

    }
}
