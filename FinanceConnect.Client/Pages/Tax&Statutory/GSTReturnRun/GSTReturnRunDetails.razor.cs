using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.GSTReturnRunViewModel;

namespace FinanceConnect.Client.Pages.Tax___Statutory.GSTReturnRun
{
    public partial class GSTReturnRunDetails
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] GSTReturnRunService RunService { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Parameter] public Guid Id { get; set; }
        private bool isInitialized = false;
        private GSTReturnRunModel? R;

        protected override async Task OnInitializedAsync() { R = RunService.GetById(Id); isInitialized = true; await Task.CompletedTask; }
        private async Task PrintPage() { await JS.InvokeVoidAsync("window.print"); }

        private static string GetStatusBadge(string s) => s switch { "Draft"=>"bg-secondary-transparent","Generated"=>"bg-info-transparent","Reviewed"=>"bg-primary-transparent","Approved"=>"bg-primary-transparent","Finalized"=>"bg-success-transparent","Filed"=>"bg-success-transparent","Closed"=>"bg-dark-transparent","Reopened"=>"bg-warning-transparent","Cancelled"=>"bg-danger-transparent",_=>"bg-secondary-transparent" };
        private static string GetFilingBadge(string s) => s switch { "NotFiled"=> "bg-secondary-transparent text-secondary", "Prepared"=>"bg-info-transparent","Filed"=>"bg-success-transparent","Acknowledged"=>"bg-success-transparent","Rejected"=>"bg-danger-transparent",_=>"bg-secondary-transparent" };
        private static string GetReconBadge(string s) => s switch { "Matched"=>"bg-success-transparent","Mismatch"=>"bg-danger-transparent","Warning"=>"bg-warning-transparent text-dark","NotRun"=> "bg-secondary-transparent text-secondary", "Partial"=>"bg-warning-transparent text-dark",_=>"bg-secondary-transparent" };
    }
}
