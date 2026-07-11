using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Bank_Cash.CashAccount;

public partial class CashAccountList : ComponentBase
{
    private List<CashAccountModels> AllAccounts = new();
    private List<BranchModel> Branches = new();
    private CashAccountModels? SelectedAccount;
    [Inject]
    private BranchService BranchService { get; set; } = default!;
    private string? SearchText;
    private Guid? SelectedBranchId;
    private string? SelectedStatus;
    private string? SelectedBranch;

    int PageWindowSize = 2;
    int EndPage => Math.Min(StartPage + PageWindowSize - 1, TotalPages);
    int StartPage = 1;
    private int CurrentPage = 1;
    private int PageSize = 10;
    private List<string> DistinctStatuses = new();
    [Inject] CashAccountService CashService { get; set; } = default!;
    private int VisibleColumnCount;
    protected override void OnInitialized()
    {
        // Load data first
        AllAccounts = CashService.GetAll();

        // Branches: only show branches that are present in the table data
        var allBranches = BranchService.GetAll();
        var branchIdsInTable = AllAccounts
            .Where(a => a.BranchId != Guid.Empty)
            .Select(a => a.BranchId)
            .Distinct()
            .ToHashSet();
        Branches = allBranches.Where(b => branchIdsInTable.Contains(b.Id)).ToList();

        // Distinct statuses from table data
        DistinctStatuses = AllAccounts
            .Where(a => !string.IsNullOrEmpty(a.Status))
            .Select(a => a.Status!)
            .Distinct()
            .ToList();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await JS.InvokeVoidAsync("feather.replace");
        await JS.InvokeVoidAsync("initTooltips");
        VisibleColumnCount =
   await JS.InvokeAsync<int>("getVisibleTableColumns");
    }
    private IEnumerable<CashAccountModels> FilteredAccounts =>
     AllAccounts.Where(a =>
         (string.IsNullOrWhiteSpace(SearchText)
             || a.Code.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
             || a.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))

         && (!SelectedBranchId.HasValue || a.BranchId == SelectedBranchId.Value)

         && (string.IsNullOrEmpty(SelectedStatus) || a.Status == SelectedStatus)
     ).OrderByDescending(a => a.UpdatedAt ?? a.CreatedAt);
    private async Task OnSearch(ChangeEventArgs e)
    {
        SearchText = e.Value?.ToString() ?? "";
        ApplyFilters();
        VisibleColumnCount =
   await JS.InvokeAsync<int>("getVisibleTableColumns");
    }

    private void ApplyFilters()
    {
        IEnumerable<CashAccountModels> query = AllAccounts;

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            query = query.Where(a =>
                a.Code.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
        || a.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
        || (!string.IsNullOrEmpty(a.BranchName) && a.BranchName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
        || (!string.IsNullOrEmpty(a.CustodianName) && a.CustodianName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
        || (!string.IsNullOrEmpty(a.CurrencyCode) && a.CurrencyCode.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
        || (!string.IsNullOrEmpty(a.CashGlAccount) && a.CashGlAccount.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
        || (a.MaxCashLimit.HasValue && a.MaxCashLimit.Value.ToString("N2").Contains(SearchText, StringComparison.OrdinalIgnoreCase))
        || (!string.IsNullOrEmpty(a.Status) && a.Status.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
            );
        }

        CurrentPage = 1;
    }

    private void OnBranchChanged(ChangeEventArgs e)
    {
        if (Guid.TryParse(e.Value?.ToString(), out var id))
            SelectedBranchId = id;
        else
            SelectedBranchId = null;

        CurrentPage = 1;
    }


    private IEnumerable<CashAccountModels> PagedAccounts =>
        FilteredAccounts
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize);

    private int TotalPages =>
        Math.Max(1, (int)Math.Ceiling((double)FilteredAccounts.Count() / PageSize));

    private Task OnPageSizeChange(Microsoft.AspNetCore.Components.ChangeEventArgs e)
    {
        PageSize = int.Parse(e.Value!.ToString()!);
        CurrentPage = 1;
        return Task.CompletedTask;
    }
    private async Task GoToPage(int page)
    {
        if (page >= 1 && page <= TotalPages)
        {
            StateHasChanged();
            await Task.Delay(200);

            CurrentPage = page;
            StateHasChanged();
        }
    }
    private async Task PreviousPage()
    {
        if (CurrentPage > 1) await GoToPage(CurrentPage - 1);
    }

    private async Task NextPage()
    {
        if (CurrentPage < TotalPages) await GoToPage(CurrentPage + 1);
    }


    private void ConfirmDelete(CashAccountModels acc)
    {
        SelectedAccount = acc;
    }

    private void DeleteAccount()
    {
        if (SelectedAccount != null)
        {
            CashService.Delete(SelectedAccount.Code); 
            AllAccounts = CashService.GetAll(); 
            SelectedAccount = null;
            ToastService.ShowError("Successfully Deleted", "Error");
        }


    }

    private void ToggleLock()
    {
        if (SelectedCashAccount == null)
            return;

        if (SelectedCashAccount.Status == "Closed")
            return;

        SelectedCashAccount.IsLockedForTransactions =
            !SelectedCashAccount.IsLockedForTransactions;
    }

    private CashAccountModels? SelectedCashAccount;

    void OpenRowDetails(CashAccountModels account)
    {
        SelectedCashAccount = account;
    }


    private async Task ViewCashAccount(CashAccountModels acc)
    {
        SelectedCashAccount = acc;
        await JS.InvokeVoidAsync("blazorOffcanvas.show", "viewCashOffcanvas");

        // Modal open pannum
    }

    private Task Reload()
    {
        // Reset filters
        SearchText = null;
        SelectedBranchId = null;
        SelectedStatus = null;

        // Reset pagination
        CurrentPage = 1;
        StartPage = 1;

        // Reload data and recompute filter options
        AllAccounts = CashService.GetAll();

        var allBranches = BranchService.GetAll();
        var branchIdsInTable = AllAccounts
            .Where(a => a.BranchId != Guid.Empty)
            .Select(a => a.BranchId)
            .Distinct()
            .ToHashSet();
        Branches = allBranches.Where(b => branchIdsInTable.Contains(b.Id)).ToList();

        DistinctStatuses = AllAccounts
            .Where(a => !string.IsNullOrEmpty(a.Status))
            .Select(a => a.Status!)
            .Distinct()
            .ToList();

        return Task.CompletedTask;
    }

    private string GetStatusBadge(string status)
    {
        return status switch
        {
            "Active" => "bg-success-transparent text-success",
            "Inactive" => "bg-danger-transparent text-danger",
            "Draft" => "bg-warning-transparent text-warning",
            _ => "bg-secondary-transparent text-secondary"
        };
    }
    private string GetStatusDotBadge(string status)
    {
        return status switch
        {
            "Active" => "bg-success text-success",
            "Inactive" => "bg-danger text-danger",
            "Draft" => "bg-warning text-warning",
            _ => "bg-secondary text-secondary"
        };
    }

}
