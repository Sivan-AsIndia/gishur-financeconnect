using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.AP.VendorBill
{
    public partial class VendorBillDetails
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] VendorBillService BillService { get; set; } = default!;
        [Inject]
        private IJSRuntime JS { get; set; }
        [Parameter] public Guid Id { get; set; }

        private bool isInitialized = false;
        private VendorBillViewModel? Bill;

        private bool IsOverdue => Bill != null && 
            Bill.DueDate < DateTime.Today && 
            Bill.OutstandingAmount > 0 && 
            Bill.BillStatus == VendorBillStatuses.Posted;

        protected override async Task OnInitializedAsync()
        {
            Bill = BillService.GetById(Id);
            isInitialized = true;
        }
        private async Task PrintPage()
        {
            await JS.InvokeVoidAsync("window.print");
        }
        private string GetStatusBadgeClass(string status) => status switch
        {
            VendorBillStatuses.Draft => "bg-secondary-transparent",
            VendorBillStatuses.Submitted => "bg-info-transparent",
            VendorBillStatuses.Approved => "bg-primary-transparent",
            VendorBillStatuses.Posted => "bg-success-transparent",
            VendorBillStatuses.Rejected => "bg-danger-transparent",
            VendorBillStatuses.Cancelled => "bg-dark-transparent",
            VendorBillStatuses.Reversed => "bg-warning-transparent",
            _ => "bg-secondary-transparent text-secondary"
        };

        private string GetSettlementBadgeClass(string status) => status switch
        {
            SettlementStatuses.Paid => "bg-success-transparent",
            SettlementStatuses.PartiallyPaid => "bg-warning-transparent text-dark",
            SettlementStatuses.Unpaid => "bg-danger-transparent",
            _ => "bg-secondary-transparent text-secondary"
        };

        private string GetLineTypeBadgeClass(string lineType) => lineType switch
        {
            VendorBillLineTypes.Goods => "bg-primary-transparent text-primary",
            VendorBillLineTypes.Service => "bg-info-transparent text-info",
            VendorBillLineTypes.Expense => "bg-warning-transparent text-warning",
            VendorBillLineTypes.Asset => "bg-success-transparent text-success",
            VendorBillLineTypes.Other => "bg-secondary-transparent text-secondary",
            _ => "bg-light text-dark"
        };
    }
}
