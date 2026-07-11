using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.AccrualViewModel;

namespace FinanceConnect.Client.Pages.Revenue_Expense.Accrual
{
    public partial class AccrualDetails : ComponentBase
    {
        [Parameter] public Guid Id { get; set; }
        [Inject] private AccrualService Service { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        private AccrualViewModel.Accrual? Item;
        private bool isInitialized;

        protected override async Task OnParametersSetAsync()
        {
            isInitialized = false;
            Item = await Service.GetByIdAsync(Id);
            isInitialized = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
        }

        private static string GetStatusBadgeClass(AccrualStatusEnum s) => s switch
        {
            AccrualStatusEnum.Draft => "bg-warning-transparent",
            AccrualStatusEnum.Submitted => "bg-info-transparent",
            AccrualStatusEnum.Approved => "bg-primary-transparent",
            AccrualStatusEnum.Posted => "bg-success-transparent",
            AccrualStatusEnum.PartiallyReversed or AccrualStatusEnum.PartiallyCleared => "bg-warning-transparent",
            AccrualStatusEnum.FullyReversed or AccrualStatusEnum.FullyCleared => "bg-info-transparent",
            AccrualStatusEnum.Cancelled => "bg-danger-transparent",
            AccrualStatusEnum.Closed => "bg-success-transparent",
            _ => "bg-light"
        };
    }
}
