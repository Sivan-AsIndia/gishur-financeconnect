using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Bank_Cash.BankAccount;

public partial class BankAccountList : ComponentBase
{

    protected BankAccountModel Model { get; set; } = new();

    private List<BankAccountModel> AllAccounts = new();
    private List<BranchDto> Branches = new();
    private BankAccountModel? SelectedAccount;
    private string? SelectedAccountType;

    private string? SearchText;
    private Guid? SelectedBranchId;
    private string? SelectedStatus;

    int PageWindowSize = 2;
    int EndPage => Math.Min(StartPage + PageWindowSize - 1, TotalPages);
    int StartPage = 1;
    private int CurrentPage = 1;
    private int PageSize = 10;
    [Inject] BankAccountService BankService { get; set; } = default!;
    private List<BranchModel> BranchesList = new();
    private List<string> AvailableStatuses = new();
    private int VisibleColumnCount;
    protected override void OnInitialized()
    {
        var allBranches = BranchService.GetAll();
        AllAccounts = BankService.GetAll();
        var branchIdsInTable = AllAccounts
            .Where(a => a.BranchId != Guid.Empty)
            .Select(a => a.BranchId)
            .Distinct()
            .ToHashSet();
        BranchesList = allBranches.Where(b => branchIdsInTable.Contains(b.Id)).ToList();
        AvailableStatuses = AllAccounts
            .Where(a => !string.IsNullOrWhiteSpace(a.BankAccountStatus))
            .Select(a => a.BankAccountStatus)
            .Distinct()
            .OrderBy(s => s)
            .ToList();

    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await JS.InvokeVoidAsync("feather.replace");
        await JS.InvokeVoidAsync("initTooltips");
        VisibleColumnCount =
   await JS.InvokeAsync<int>("getVisibleTableColumns");
    }
    private IEnumerable<BankAccountModel> FilteredAccounts =>
     AllAccounts.Where(a =>
         (string.IsNullOrWhiteSpace(SearchText)
          || a.BankAccountName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
          || a.BankName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
          || a.IFSCCode.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
         && (!SelectedBranchId.HasValue || a.BranchId == SelectedBranchId.Value)
         && (string.IsNullOrEmpty(SelectedStatus) || a.BankAccountStatus == SelectedStatus)
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
        IEnumerable<BankAccountModel> query = AllAccounts;

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            query = query.Where(a =>
               (string.IsNullOrWhiteSpace(SearchText)
          || a.BankAccountName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
          || a.BankName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
          || a.IFSCCode.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
         && (!SelectedBranchId.HasValue || a.BranchId == SelectedBranchId.Value)
         && (string.IsNullOrEmpty(SelectedStatus) || a.BankAccountStatus == SelectedStatus)
         && (string.IsNullOrEmpty(SelectedAccountType) || a.BankAccountType == SelectedAccountType)
            );
        }

        CurrentPage = 1;
    }

    private IEnumerable<BankAccountModel> PagedAccounts =>
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


    private void ConfirmDelete(BankAccountModel acc)
    {
        SelectedAccount = acc;
    }


    private void OpenLockModal(BankAccountModel account)
    {
        SelectedAccount = account;
    }
    private async Task ViewBankAccount(BankAccountModel BankAccount)
    {
        StateHasChanged();
        await Task.Delay(200);
        var company = AllAccounts
    .FirstOrDefault(c => c.Id == BankAccount.CompanyId);

        BankAccount.CompanyName = company?.CompanyName ?? string.Empty;
        SelectedCashAccount = BankAccount;
        StateHasChanged();
        await JS.InvokeVoidAsync("blazorOffcanvas.show", "viewBankOffcanvas");

    }

    private async Task ConfirmToggleLock()
    {
        if (SelectedAccount == null) return;

        if (!SelectedAccount.IsLockedForTransactions)
        {
            // 🔒 LOCK
            SelectedAccount.IsLockedForTransactions = true;
            SelectedAccount.Description += " | Locked by user";
            SelectedAccount.BankAccountStatus = "Locked";
            ToastService.ShowWarning("Sucessfully Unlocked", "Warning");
        }
        else
        {
            // 🔓 UNLOCK
            SelectedAccount.IsLockedForTransactions = false;
            SelectedAccount.Description += " | Unlocked by user";
            SelectedAccount.BankAccountStatus = "Active";
          
            ToastService.ShowSuccess("Sucessfully Locked", "Success");


        }


        await InvokeAsync(StateHasChanged);
        await JS.InvokeVoidAsync("feather.replace");

        SelectedAccount = null;
    }

    private void DeleteAccount()
    {
        if (SelectedAccount != null)
        {
            BankService.Delete(
                SelectedAccount.Id,   // Guid
                "Deleted by user"                // Reason
            );

            AllAccounts = BankService.GetAll();  // Refresh table
            SelectedAccount = null;
            ToastService.ShowError("Sucessfully Deleted", "Error");
        }
    }



    private BankAccountModel? SelectedCashAccount;

    void OpenRowDetails(BankAccountModel account)
    {
        SelectedCashAccount = account;
    }


    private async Task ViewCashAccount(BankAccountModel acc)
    {
        SelectedCashAccount = acc;

        // Modal open pannum
    }

    private Task Reload()
    {
        // 🔹 Reset filters
        SearchText = null;
        SelectedBranchId = null;
        SelectedStatus = null;
        SelectedAccountType = null;
        // 🔹 Reset pagination
        CurrentPage = 1;
        StartPage = 1;

        return Task.CompletedTask;
    }

  private string GetStatusBadge(string status)
{
    return status switch
    {
              
        "Active" => "bg-success-transparent text-success",
        "Inactive" => "bg-danger-transparent text-danger",
        "Closed" => "bg-warning-transparent text-warning",
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
        "Closed" => "bg-warning text-warning",
        "Draft" => "bg-warning text-warning",
        _ => "bg-secondary text-secondary"
    };
}


}
