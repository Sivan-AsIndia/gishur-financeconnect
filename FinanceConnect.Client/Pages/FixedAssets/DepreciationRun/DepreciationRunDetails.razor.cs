using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.FixedAssets.DepreciationRun
{
    public partial class DepreciationRunDetails
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] DepreciationRunService RunService { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;

        [Parameter] public Guid Id { get; set; }

        private bool isInitialized = false;

        private DepreciationRunViewModel? Run;
        List<CompanyModel> Companies = new();

        protected override void OnInitialized()
        {
            Run = RunService.GetById(Id);
            isInitialized = true;
        }

        private async Task PrintPage()
        {
            await JS.InvokeVoidAsync("window.print");
        }

        private string GetCompanyName(Guid? id)
        {
            if (id == null || id == Guid.Empty)
                return "-";

            var company = MasterDataService.GetAllCompanies()
                .FirstOrDefault(c => c.Id == id && c.Status == "Active");

            return company?.LegalName ?? "-";
        }

        private string GetStatusBadge(DepreciationRunStatus status) => status switch
        {
            DepreciationRunStatus.Draft => "bg-secondary-transparent text-secondary",
            DepreciationRunStatus.Generated => "bg-info-transparent",
            DepreciationRunStatus.Submitted => "bg-warning-transparent",
            DepreciationRunStatus.Approved => "bg-primary-transparent",
            DepreciationRunStatus.Posted => "bg-success-transparent",
            DepreciationRunStatus.Finalized => "bg-dark-transparent",
            DepreciationRunStatus.Reversed => "bg-danger-transparent",
            _ => "bg-secondary"
        };
    }
}
