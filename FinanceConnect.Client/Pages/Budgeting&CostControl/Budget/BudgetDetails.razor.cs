using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Budgeting_CostControl.Budget
{
    public partial class BudgetDetails
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] BudgetService BudgetService { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;

        [Parameter] public Guid Id { get; set; }

        private bool isInitialized = false;
        private BudgetViewModel? Budget;

        protected override async Task OnInitializedAsync()
        {
            Budget = await BudgetService.GetByIdAsync(Id);
            isInitialized = true;
        }

        private async Task PrintPage()
        {
            await JS.InvokeVoidAsync("window.print");
        }

        public void getCompanyName(Guid Id)
        {

        }

        private string GetStatusBadge(string status) => status switch
        {
            "Draft" => "bg-secondary-transparent text-dark",
            "Submitted" => "bg-warning-transparent",
            "Approved" => "bg-primary-transparent",
            "Locked" => "bg-success-transparent",
            "Closed" => "bg-dark-transparent",
            _ => "bg-secondary"
        };
    }
}
