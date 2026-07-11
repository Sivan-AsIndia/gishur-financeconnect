using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.DeferredRevenueViewModel;
using DeferredRevenueModel = FinanceConnect.Client.ViewModels.DeferredRevenueViewModel.DeferredRevenue;

namespace FinanceConnect.Client.Pages.Revenue_Expense.DeferredRevenue
{
    public partial class DeferredRevenueDetails : ComponentBase
    {
        [Parameter] public Guid Id { get; set; }
        [Inject] private DeferredRevenueService Service { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        private DeferredRevenueModel? Item; private bool isInitialized;
        protected override async Task OnParametersSetAsync() { isInitialized = false; Item = await Service.GetByIdAsync(Id); isInitialized = true; }
        protected override async Task OnAfterRenderAsync(bool f) { await JS.InvokeVoidAsync("feather.replace"); await JS.InvokeVoidAsync("initTooltips"); }
        private static string GetStatusBadgeClass(DeferredRevenueStatusEnum s) => s switch { DeferredRevenueStatusEnum.Draft => "bg-warning-transparent", DeferredRevenueStatusEnum.Approved => "bg-primary-transparent", DeferredRevenueStatusEnum.Posted or DeferredRevenueStatusEnum.InProgress => "bg-success-transparent", DeferredRevenueStatusEnum.FullyReleased or DeferredRevenueStatusEnum.Closed => "bg-success-transparent", DeferredRevenueStatusEnum.Cancelled => "bg-danger-transparent", _ => "bg-info-transparent" };
    }
}
