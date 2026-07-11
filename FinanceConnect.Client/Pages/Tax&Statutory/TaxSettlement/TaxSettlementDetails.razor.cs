using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.TaxSettlementViewModel;

namespace FinanceConnect.Client.Pages.Tax___Statutory.TaxSettlement
{
    public partial class TaxSettlementDetails
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] TaxSettlementService SettlementService { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Parameter] public Guid Id { get; set; }
        private bool isInitialized = false;
        private TaxSettlementModel? S;

        protected override async Task OnInitializedAsync() { S = SettlementService.GetById(Id); isInitialized = true; await Task.CompletedTask; }
        private async Task PrintPage() { await JS.InvokeVoidAsync("window.print"); }

        private static string GetStatusBadge(string s) => s switch { "Draft"=> "bg-secondary-transparent text-secondary", "Submitted"=>"bg-info-transparent","Approved"=>"bg-primary-transparent","Posted"=>"bg-success-transparent","Closed"=>"bg-dark-transparent","Reversed"=>"bg-warning-transparent","Cancelled"=>"bg-danger-transparent",_=>"bg-secondary-transparent" };
        private static string GetScopeBadge(string s) => s switch { "GST"=>"bg-success-transparent","TDS"=>"bg-warning-transparent text-dark","TCS"=>"bg-purple-transparent",_=> "bg-secondary-transparent text-secondary" };
        private static string GetReconBadge(string s) => s switch { "Reconciled"=>"bg-success-transparent","PartiallyReconciled"=>"bg-warning-transparent text-dark","NotReconciled"=> "bg-secondary-transparent text-secondary", _=>"bg-secondary-transparent" };
        private static string FormatType(string t) => t switch { "GSTCashPayment"=>"GST Cash Payment","GSTInputCreditOffset"=>"GST ITC Offset","GSTMixedSettlement"=>"GST Mixed","TDSRemittance"=>"TDS Remittance","TCSRemittance"=>"TCS Remittance","TaxAdjustment"=>"Tax Adjustment",_=>t??"" };
    }
}
