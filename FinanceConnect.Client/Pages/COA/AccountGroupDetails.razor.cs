using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.COA;

public partial class AccountGroupDetails : ComponentBase
{
    [Parameter] public Guid GroupId { get; set; }

    private AccountGroupViewModel? SelectedGroup;

    protected override async Task OnInitializedAsync()
    {
        SelectedGroup = await COADataService.GetAccountGroupByIdAsync(GroupId);
    }

    private async Task PrintPage()
    {
        await JS.InvokeVoidAsync("window.print");
    }
    private void GoBack()
    {
        Nav.NavigateTo("/account-groups");
    }

    private string GetNatureBadge(string nature)
    {
        return nature switch
        {
            AccountNatures.Asset => "bg-success-transparent text-success",
            AccountNatures.Liability => "bg-danger-transparent text-danger",
            AccountNatures.Equity => "bg-purple-transparent text-purple",
            AccountNatures.Income => "bg-info-transparent text-info",
            AccountNatures.Expense => "bg-warning-transparent text-warning",
            _ => "bg-secondary-transparent text-secondary"
        };
    }

    private string GetStatusBadge(string status)
    {
        return status switch
        {
            GroupStatuses.Draft => "bg-warning-transparent text-warning",
            GroupStatuses.Active => "bg-success-transparent text-success",
            GroupStatuses.Inactive => "bg-danger-transparent text-danger",
            _ => "bg-secondary-transparent text-secondary"
        };
    }

    private string GetBalanceBadge(string? behavior)
    {
        return behavior switch
        {
            BalanceBehaviors.Debit => "bg-info-transparent text-info",
            BalanceBehaviors.Credit => "bg-purple-transparent text-purple",
            _ => "bg-secondary-transparent text-secondary"
        };
    }
}
