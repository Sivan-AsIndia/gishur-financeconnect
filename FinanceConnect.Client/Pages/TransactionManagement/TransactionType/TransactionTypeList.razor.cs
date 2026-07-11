using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;

namespace FinanceConnect.Client.Pages.TransactionManagement.TransactionType
{
    public partial class TransactionTypeList
    {
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
                await JS.InvokeVoidAsync("feather.replace");
                await JS.InvokeVoidAsync("initTooltips");
                VisibleColumnCount =
                await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] PostingProfileService ProfileService { get; set; } = default!;
        public List<CompanyModel> Companies = new();

        // ================== STATE ==================
        List<TransactionTypeModel> Types = new();
        List<TransactionTypeModel> FilteredTypes = new();
        PostingProfileModel? PostingProfiles = new();

        TransactionTypeModel? SelectedType;
        bool canDeactivate = true;

        private bool isInitialized = false;
        private bool isLoading = false;

        string searchText = "";
        int PageSize = 10;
        int CurrentPage = 1;
        int PageWindowSize = 2;
        string selectedStatus = "";
        SourceCategory? selectedCategory = null;
        Guid? selectedCompany = null;
        private int VisibleColumnCount;
        protected override void OnInitialized()
        {
            Companies = MasterDataService.GetAllCompanies().Where(c => c.Status == "Active").ToList();
            LoadTypes();
        }

        // ================== LOAD ==================
        void LoadTypes()
        {
            Types = TypeService.GetAll();
            isInitialized = true;
            ApplyFilters();
        }

        void OnRefresh()
        {
            searchText = "";
            selectedStatus = "";
            selectedCategory = null;
            selectedCompany = null;
        }

        // ================== FILTER ==================
        string SelectedStatus
        {
            get => selectedStatus;
            set
            {
                selectedStatus = value;
                ApplyFilters();
            }
        }
        SourceCategory? SelectedCategory
        {
            get => selectedCategory;
            set
            {
                selectedCategory = value;
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

        async Task OnSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString() ?? "";
            ApplyFilters();
            VisibleColumnCount =
            await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        void ApplyFilters()
        {
            IEnumerable<TransactionTypeModel> query = Types;

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(t =>
                    t.Code.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    t.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase));
            }

            if (selectedCategory.HasValue)
            {
                query = query.Where(t => t.SourceCategory == selectedCategory.Value);
            }


            if (selectedCompany.HasValue)
            {
                query = query.Where(t => t.CompanyId == selectedCompany.Value);
            }

            if (!string.IsNullOrWhiteSpace(selectedStatus))
            {
                bool isActive = selectedStatus == "Active";
                query = query.Where(t => t.IsActive == isActive);
            }

            FilteredTypes = query
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToList();

            CurrentPage = 1;
        }

        // ================== PAGING ==================
        int TotalPages =>
            FilteredTypes.Count == 0
                ? 1
                : (int)Math.Ceiling((double)FilteredTypes.Count / PageSize);

        List<TransactionTypeModel> PagedTypes =>
            FilteredTypes
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

        // ================== UI ==================
        void ViewType(TransactionTypeModel type)
        {
            SelectedType = type;
            PostingProfiles = ProfileService
                .GetAll()
                .FirstOrDefault(p =>
                    p.CompanyId == type.CompanyId &&
                    p.PostingProfileId == type.DefaultPostingProfileId);

        }

        private async Task GoToViewPage(TransactionTypeModel type)
        {
            Nav.NavigateTo($"/transaction-types/{type.TransactionTypeId}/view");
        }

        private string GetPostingProfileName(TransactionTypeModel type)
        {
            var profile = ProfileService
                .GetAll()
                .FirstOrDefault(p =>
                    p.CompanyId == type.CompanyId &&
                    p.PostingProfileId == type.DefaultPostingProfileId);

            return profile?.ProfileName ?? "—";
        }

        void SelectType(TransactionTypeModel type)
        {
            SelectedType = type;
            canDeactivate = TypeService.CanDeactivate(type.TransactionTypeId);
        }

        string GetCompanyName(Guid? companyId)
        {
            return Companies.FirstOrDefault(c => c.Id == companyId)?.LegalName ?? "—";
        }

        // ================== ACTIONS ==================
        void ActivateConfirmed()
        {
            if (SelectedType == null) return;

            TypeService.Activate(SelectedType.TransactionTypeId);
            ToastService.ShowSuccess($"Transaction type '{SelectedType.Name}' activated", "Activated");
            LoadTypes();
        }

        void DeactivateConfirmed()
        {
            if (SelectedType == null) return;

            TypeService.Deactivate(SelectedType.TransactionTypeId);
            ToastService.ShowSuccess($"Transaction type '{SelectedType.Name}' deactivated", "Deactivated");
            LoadTypes();
        }

        void DeleteConfirmed()
        {
            if (SelectedType == null) return;

            TypeService.Delete(SelectedType.TransactionTypeId);
            ToastService.ShowSuccess($"Transaction type '{SelectedType.Name}' deleted", "Deleted");
            LoadTypes();
        }

        private async Task OnRefreshAsync()
        {
            isLoading = true;
            StateHasChanged();

            await Task.Delay(300);
            OnRefresh();
            TypeService.ResetToSeed();
            LoadTypes();

            isLoading = false;
            await JS.InvokeVoidAsync("feather.replace");
            ToastService.ShowInfo("Transaction Type list refreshed", "Refreshed");
        }
    }
}
