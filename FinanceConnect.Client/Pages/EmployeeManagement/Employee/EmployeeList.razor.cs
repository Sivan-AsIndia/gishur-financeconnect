using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.EmployeeManagement.Employee
{
    public partial class EmployeeList
    {

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
            VisibleColumnCount =
            await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        [Inject] MasterDataService MasterDataService { get; set; } = default!;

        [Inject] EmployeeService EmployeeService { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;

        List<EmployeeViewModel> Employees = new();
        List<EmployeeViewModel> FilteredEmployees = new();

        private bool isInitialized = false;
        private bool isLoading = false;

        string searchText = "";
        string selectedStatus = "";
        Guid? selectedDepartment = null;
        private bool canDeactivate = true;
        private bool canDelete = true;
        private int VisibleColumnCount;
        int PageSize = 10;
        int CurrentPage = 1;
        int PageWindowSize = 2;
        public record LookupItem(Guid Id, string Name);
        EmployeeViewModel? SelectedEmployee;
        public List<CompanyModel> Companies = new();
        private static readonly Guid DeptAccounts = Guid.Parse("10000000-0000-0000-0000-000000000001");
        private static readonly Guid DeptSales = Guid.Parse("10000000-0000-0000-0000-000000000002");
        private static readonly Guid DeptHR = Guid.Parse("10000000-0000-0000-0000-000000000003");

        public static readonly List<LookupItem> Departments = new()
{
    new LookupItem(DeptAccounts, "Accounts"),
    new LookupItem(DeptSales, "Sales"),
    new LookupItem(DeptHR, "Human Resources")
};
        Guid? selectedCompany = null;

        int TotalPages =>
            FilteredEmployees.Count == 0
                ? 1
                : (int)Math.Ceiling((double)FilteredEmployees.Count / PageSize);

        List<EmployeeViewModel> PagedEmployees =>
            FilteredEmployees
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
            Employees = EmployeeService.GetAll();

            FilteredEmployees = Employees
                .OrderByDescending(x => x.ModifiedTime ?? x.AddedTime)
                .ToList();
            isInitialized = true;
        }


        #region SEARCH & FILTER


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

        Guid? SelectedDepartment
        {
            get => selectedDepartment;
            set
            {
                selectedDepartment = value;
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
            IEnumerable<EmployeeViewModel> query = Employees;

            // Search
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(e =>
                    (e.EmployeeId ?? "").Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    (e.FirstName ?? "").Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    (e.LastName ?? "").Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    (e.OfficialEmail ?? "").Contains(searchText, StringComparison.OrdinalIgnoreCase)
                );
            }
            // Company filter
            if (selectedCompany.HasValue)
            {
                query = query.Where(b =>
                     b.CompanyId != null &&
                     b.CompanyId == selectedCompany.Value);
            }
            // Department filter
            if (selectedDepartment.HasValue)
            {
                query = query.Where(e =>
                    e.DepartmentId == selectedDepartment.Value);
            }

            // Status filter
            if (!string.IsNullOrWhiteSpace(selectedStatus))
            {
                query = query.Where(e =>
                    e.Status.ToString() == selectedStatus);
            }

            FilteredEmployees = query
                .OrderByDescending(x => x.ModifiedTime ?? x.AddedTime)
                .ToList();

            CurrentPage = 1;
        }

        #endregion

        #region PAGINATION

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

        #endregion

        #region REFRESH


        private async Task OnRefreshAsync()
        {
            isLoading = true;
            StateHasChanged();

            await Task.Delay(300);

            EmployeeService.ResetToSeed();
            Employees = EmployeeService.GetAll();

            FilteredEmployees = Employees
                .OrderByDescending(x => x.ModifiedTime ?? x.AddedTime)
                .ToList();

            isLoading = false;
            await JS.InvokeVoidAsync("feather.replace");
            ToastService.ShowInfo("Employee list refreshed", "Refreshed");
        }

        #endregion

        #region DELETE

        void ConfirmDelete(EmployeeViewModel emp)
        {
            SelectedEmployee = emp;
        }

        async Task DeleteConfirmed()
        {
            if (SelectedEmployee == null)
                return;

            EmployeeService.Delete(SelectedEmployee.Id);

            Employees = EmployeeService.GetAll();
            ApplyFilters();

            ToastService.ShowError(
                $"{SelectedEmployee.FirstName} {SelectedEmployee.LastName} deleted successfully","Deleted");

            SelectedEmployee = null;

            await JS.InvokeVoidAsync("closeDeleteModal");
        }

        #endregion

        #region ACTIVATE / DEACTIVATE

        void ConfirmActivate(EmployeeViewModel emp)
            => SelectedEmployee = emp;

        void ActivateConfirmed()
        {
            if (SelectedEmployee != null)
            {
                EmployeeService.Activate(SelectedEmployee.Id);

                Employees = EmployeeService.GetAll();
                ApplyFilters();

                ToastService.ShowSuccess(
                    $"Employee '{SelectedEmployee.FirstName}' activated successfully",
                    "Activated");

                SelectedEmployee = null;
            }
        }

        void ConfirmDeactivate(EmployeeViewModel emp)
            => SelectedEmployee = emp;

        void DeactivateConfirmed()
        {
            if (SelectedEmployee != null)
            {
                EmployeeService.Deactivate(SelectedEmployee.Id);

                Employees = EmployeeService.GetAll();
                ApplyFilters();

                ToastService.ShowWarning(
                    $"Employee '{SelectedEmployee.FirstName}' deactivated successfully",
                    "Deactivated");

                SelectedEmployee = null;
            }
        }

        #endregion


        void OpenRowDetails(EmployeeViewModel emp)
        {
            SelectedEmployee = emp;
        }
        #region NAVIGATION

        void ViewEmployee(EmployeeViewModel emp)
        {
            Nav.NavigateTo($"/employees/{emp.Id}/view");
        }

        #endregion

        #region STATUS BADGE

        private string GetStatusBadge(EmployeeStatus? status)
        {
            return status switch
            {
                EmployeeStatus.Active => "bg-success text-white",
                EmployeeStatus.Inactive => "bg-danger text-white",
                EmployeeStatus.Resigned => "bg-warning text-dark",
                EmployeeStatus.Terminated => "bg-secondary text-white",
                _ => "bg-secondary text-white"
            };
        }

        #endregion
    }
}