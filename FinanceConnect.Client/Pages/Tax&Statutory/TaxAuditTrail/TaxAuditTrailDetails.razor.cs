using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.TaxAuditTrailViewModel;

namespace FinanceConnect.Client.Pages.Tax___Statutory.TaxAuditTrail
{
    public partial class TaxAuditTrailDetails : ComponentBase
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] TaxAuditTrailService AuditService { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Parameter] public Guid Id { get; set; }
        private bool isInitialized = false;
        private TaxAuditTrailModel? A;

        protected override async Task OnInitializedAsync() { A = AuditService.GetById(Id); isInitialized = true; await Task.CompletedTask; }

        private static string GetScopeBadge(string s) => s switch { "GST"=>"bg-success-transparent","TDS"=>"bg-warning-transparent text-dark","TCS"=>"bg-purple-transparent","Mixed"=>"bg-secondary-transparent",_=>"bg-light text-muted" };
        private static string GetSeverityColor(string s) => s switch { "Critical"=>"#dc2626","High"=>"#f59e0b","Warning"=>"#eab308","Info"=>"#3b82f6",_=>"#64748b" };
        private static string GetSeverityBg(string s) => s switch { "Critical"=>"#fef2f2","High"=>"#fefce8","Warning"=>"#fefce8","Info"=>"#eff6ff",_=>"#f1f5f9" };
    }
}
