using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.PrepaymentViewModel;

namespace FinanceConnect.Client.Pages.Revenue_Expense.Prepayment
{
    public partial class PrepaymentDetails : ComponentBase
    {
        [Parameter] public Guid Id { get; set; }
        [Inject] private PrepaymentService Service { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        private PrepaymentViewModel.Prepayment? Item; private bool isInitialized;
        protected override async Task OnParametersSetAsync() { isInitialized = false; Item = await Service.GetByIdAsync(Id); isInitialized = true; }
        protected override async Task OnAfterRenderAsync(bool f) { await JS.InvokeVoidAsync("feather.replace"); await JS.InvokeVoidAsync("initTooltips"); }
        private static string GetStatusBadgeClass(PrepaymentStatusEnum s) => s switch { PrepaymentStatusEnum.Draft => "bg-warning-transparent", PrepaymentStatusEnum.Approved => "bg-primary-transparent", PrepaymentStatusEnum.Posted or PrepaymentStatusEnum.InProgress => "bg-success-transparent", PrepaymentStatusEnum.FullyReleased or PrepaymentStatusEnum.Closed => "bg-success-transparent", PrepaymentStatusEnum.Cancelled => "bg-danger-transparent", _ => "bg-info-transparent" };
    }
}
