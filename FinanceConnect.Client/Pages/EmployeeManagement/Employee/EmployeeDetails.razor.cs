using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.EmployeeManagement.Employee
{
    public partial class EmployeeDetails
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] EmployeeService EmployeeService { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;

        [Parameter] public Guid Id { get; set; }

        private bool isInitialized = false;
        private EmployeeViewModel? Employee;

        private string DepartmentName =>
            EmployeeService.GetDepartmentName(Employee?.DepartmentId);

        private string DesignationName =>
            EmployeeService.GetDesignationName(Employee?.DesignationId);

        private string ReportingManagerName =>
            Employee?.ReportingManagerId != null
                ? EmployeeService.GetById(Employee.ReportingManagerId.Value)?.FirstName
                : "-";

        protected override void OnInitialized()
        {
            Employee = EmployeeService.GetById(Id);
            isInitialized = true;
        }

        private async Task PrintPage()
        {
            await JS.InvokeVoidAsync("window.print");
        }

        private string GetStatusBadgeClass(EmployeeStatus? status) => status switch
        {
            EmployeeStatus.Active => "bg-success-transparent",
            EmployeeStatus.Inactive => "bg-danger-transparent",
            EmployeeStatus.Resigned => "bg-warning-transparent",
            EmployeeStatus.Terminated => "bg-secondary-transparent",
            _ => "bg-secondary-transparent"
        };
    }
}
