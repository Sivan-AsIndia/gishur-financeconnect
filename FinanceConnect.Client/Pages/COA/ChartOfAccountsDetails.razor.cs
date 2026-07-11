using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.COA;

public partial class ChartOfAccountsDetails : ComponentBase
{
    [Parameter] public Guid ChartId { get; set; }

    private ChartOfAccountsViewModel? SelectedChart { get; set; }

    protected override async Task OnInitializedAsync()
    {
        SelectedChart = await COADataService.GetChartOfAccountsByIdAsync(ChartId);
    }

    private async Task PrintPage()
    {
        await JS.InvokeVoidAsync("window.print");
    }

    private void GoBack()
    {
        Nav.NavigateTo("/chart-of-accounts");
    }

    private string GetTypeBadgeClass(string type)
    {
        return type switch
        {
            ChartTypes.Standard => "bg-primary-transparent text-primary",
            ChartTypes.Template => "bg-info-transparent text-info",
            ChartTypes.Migration => "bg-warning-transparent text-warning",
            _ => "bg-secondary-transparent text-secondary"
        };
    }

    private string GetStatusBadgeClass(string status)
    {
        return status switch
        {
            COAStatuses.Draft => "bg-warning-transparent text-warning",
            COAStatuses.Active => "bg-success-transparent text-success",
            COAStatuses.Locked => "bg-info-transparent text-info",
            COAStatuses.Retired => "bg-danger-transparent text-danger",
            _ => "bg-secondary-transparent text-secondary"
        };
    }
}
