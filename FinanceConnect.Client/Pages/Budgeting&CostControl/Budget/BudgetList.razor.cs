using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Budgeting_CostControl.Budget
{
    public partial class BudgetList
    {

        [Inject] MasterDataService MasterDataService { get; set; } = default!;

        List<BudgetViewModel> Budgets = new();
        List<BudgetViewModel> Filtered = new();
        BudgetViewModel SelectedBudget = new();
        public List<CompanyModel> Companies = new();
        bool isInitialized = false;
        bool isLoading = false;

        string searchText = "";
        string? selectedStatus;
        Guid? selectedCompany = null;

        int PageSize = 10;
        int CurrentPage = 1;
        int PageWindowSize = 2;
        private int VisibleColumnCount;

        int TotalPages =>
            Filtered.Count == 0
            ? 1
            : (int)Math.Ceiling((double)Filtered.Count / PageSize);

        List<BudgetViewModel> PagedBudgets =>
            Filtered
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .ToList();

        Guid? SelectedCompany
        {
            get => selectedCompany;
            set
            {
                selectedCompany = value;
                ApplyFilters();
            }
        }

        string? SelectedStatus
        {
            get => selectedStatus;
            set
            {
                selectedStatus = value;
                ApplyFilters();
            }
        }

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
            Budgets = BudgetService.GetAll();
            Companies = MasterDataService.GetAllCompanies().Where(c => c.Status == "Active").ToList();
            Filtered = Budgets;
            isInitialized = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            VisibleColumnCount = await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        void LoadData()
        {
            Budgets = BudgetService.GetAll();
        }

        void OnSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString() ?? "";
            ApplyFilters();
        }

        void ApplyFilters()
        {
            var query = Budgets.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(x =>
                    x.BudgetCode.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    x.BudgetName.Contains(searchText, StringComparison.OrdinalIgnoreCase));
            }
            // Company filter
            if (selectedCompany.HasValue)
            {
                query = query.Where(b =>
                     b.CompanyId != null &&
                     b.CompanyId == selectedCompany.Value);
            }

            if (!string.IsNullOrWhiteSpace(selectedStatus))
            {
                query = query.Where(x => x.Status == selectedStatus);
            }

            Filtered = query.ToList();
            CurrentPage = 1;
        }

        void openBudget(BudgetViewModel budget)
        {
            SelectedBudget = budget;
        }
        void ArchivePopupOpen(BudgetViewModel budget)
        {
            SelectedBudget = budget;
        }

        private async Task ConfirmArchive()
        {
            if (SelectedBudget == null) return;

            try
            {
                await BudgetService.ArchiveAsync(SelectedBudget.BudgetId);

                ToastService.ShowSuccess(
                    $"Budget '{SelectedBudget.BudgetName}' archived successfully",
                    "Archived");

                LoadData();
            }
            catch (InvalidOperationException ex)
            {
                ToastService.ShowError(ex.Message, "Error");
            }
        }
        void Submit(BudgetViewModel model)
        {
            BudgetService.Submit(model.BudgetId);
            ToastService.ShowSuccess("Budget submitted", "Submitted");
            Load();
        }

        void Approve(BudgetViewModel model)
        {
            BudgetService.Approve(model.BudgetId);
            ToastService.ShowSuccess("Budget approved", "Approved");
            Load();
        }

        void Lock(BudgetViewModel model)
        {
            BudgetService.Lock(model.BudgetId);
            ToastService.ShowSuccess("Budget locked", "Locked");
            Load();
        }

        void ViewBudget(BudgetViewModel model)
        {
            Nav.NavigateTo($"/budgets/{model.BudgetId}/view");
        }

        void Load()
        {
            Budgets = BudgetService.GetAll();
            ApplyFilters();
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

        private async Task OnRefreshAsync()
        {
            isLoading = true;
            StateHasChanged();

            await Task.Delay(300);

            searchText = "";
            selectedStatus = null;
            selectedCompany = null;

            BudgetService.ResetToSeed();
            Load();

            isLoading = false;

            await JS.InvokeVoidAsync("feather.replace");

            ToastService.ShowInfo("Budget list refreshed", "Refreshed");
        }

        string GetStatusBadge(string status)
        {
            return status switch
            {
                "Draft" => "bg-secondary-transparent text-dark",
                "Submitted" => "bg-warning-transparent",
                "Approved" => "bg-primary-transparent",
                "Locked" => "bg-success-transparent",
                _ => "bg-secondary"
            };
        }
    }
}
