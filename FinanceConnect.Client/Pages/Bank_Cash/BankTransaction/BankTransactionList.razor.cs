using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Reflection;

namespace FinanceConnect.Client.Pages.Bank_Cash.BankTransaction;

public partial class BankTransactionList : ComponentBase
{
    protected List<BankTransactionModel> Transactions = new();

    private string? SearchText;
    private string? SelectedStatus;
    [Inject] NavigationManager Nav { get; set; } = default!;
    [Inject] public IJSRuntime JS { get; set; } = default!;
    [Inject] public BankTransactionService Service { get; set; } = default!;

    protected BankTransactionModel? SelectedTransaction;
    protected Guid DeleteId;
    private List<string> AvailableStatuses = new();

    int PageWindowSize = 2;
    int StartPage = 1;
    int CurrentPage = 1;
    int PageSize = 10;
    int TotalPages =>
        Math.Max(1, (int)Math.Ceiling((double)FilteredAccounts.Count() / PageSize));

    int EndPage => Math.Min(StartPage + PageWindowSize - 1, TotalPages);

    void OpenRowDetails(BankTransactionModel Transaction)
    {
        SelectedTransaction = Transaction;
    }
    protected override async Task OnInitializedAsync()
    {
        Transactions = await Service.GetAllAsync();
        AvailableStatuses = Transactions
            .Where(t => !string.IsNullOrWhiteSpace(t.TransactionStatus))
            .Select(t => t.TransactionStatus)
            .Distinct()
            .OrderBy(s => s)
            .ToList();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await JS.InvokeVoidAsync("feather.replace");
        await JS.InvokeVoidAsync("initTooltips");

    }

    // ================================
    // FILTER + SEARCH
    // ================================

    IEnumerable<BankTransactionModel> FilteredAccounts =>
     Transactions.Where(a =>
         (
             string.IsNullOrWhiteSpace(SearchText)
             || (a.TransactionNumber?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false)
             || (a.Direction?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false)
             || (a.TransactionStatus?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false)
             || (a.TransactionType?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false)
             || (a.PaymentMethod?.ToString().Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false)
             || (a.Amount.ToString().Contains(SearchText))
         )
         &&
         (
             string.IsNullOrEmpty(SelectedStatus)
             || a.TransactionStatus == SelectedStatus
         )
     ).OrderByDescending(a => a.UpdatedAt ?? a.CreatedAt);

    IEnumerable<BankTransactionModel> PagedAccounts =>
        FilteredAccounts
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize);

    void OnSearch(ChangeEventArgs e)
    {
        SearchText = e.Value?.ToString();
        CurrentPage = 1;
    }

    void EditTransaction(Guid id)
    {
        // Navigate to edit page with Id
        Nav.NavigateTo($"/bank-transactions/{id}");
    }
    void OnStatusChanged(ChangeEventArgs e)
    {
        SelectedStatus = e.Value?.ToString();
        CurrentPage = 1;
    }

    // ================================
    // PAGINATION
    // ================================

    Task OnPageSizeChange(ChangeEventArgs e)
    {
        PageSize = int.Parse(e.Value!.ToString()!);
        CurrentPage = 1;
        return Task.CompletedTask;
    }

    async Task GoToPage(int page)
    {
        if (page >= 1 && page <= TotalPages)
        {
            CurrentPage = page;
            await Task.Delay(50);
        }
    }

    void PreviousPage()
    {
        if (CurrentPage > 1)
            CurrentPage--;
    }

    void NextPage()
    {
        if (CurrentPage < TotalPages)
            CurrentPage++;
    }

    // ================================
    // ACTIONS
    // ================================


    private async Task ViewTransactionpopup(BankTransactionModel transaction)
    {
        StateHasChanged();
        await Task.Delay(200);

        SelectedTransaction = transaction;
        StateHasChanged();
        await JS.InvokeVoidAsync("blazorOffcanvas.show", "viewTransactionOffcanvas");

    }
    async Task ViewTransaction(BankTransactionModel Model)
    {
        SelectedTransaction = Model;
        await JS.InvokeVoidAsync("blazorOffcanvas.show", "viewTransactionOffcanvas");

    }

    protected void ConfirmDelete(BankTransactionModel txn)
    {
        DeleteId = txn.Id;
    }

    private Task Reload()
    {
        SearchText = null;
        SelectedStatus = null;
        CurrentPage = 1;
        StartPage = 1;

        return Task.CompletedTask;
    }
    protected async Task DeleteTransaction()
    {
        await Service.DeleteAsync(DeleteId);
        Transactions = await Service.GetAllAsync();
        ToastService.ShowError("Transaction deleted Successfully", "Error");

    }

    string GetStatusBadge(string status) =>
        status switch
        {
            "Submitted" => "bg-info-transparent text-info",
            "Approved" => "bg-primary-transparent text-primary",
            "Posted" => "bg-success-transparent text-success",
            "Reversed" => "bg-danger-transparent text-danger",
            "Draft" => "bg-warning-transparent text-warning",
            _ => "bg-secondary-transparent text-secondary"
        };
    string GetStatusDotBadge(string status) =>
        status switch
        {
            "Submitted" => "bg-info text-info",
            "Approved" => "bg-primary text-primary",
            "Posted" => "bg-success text-success",
            "Reversed" => "bg-danger text-danger",
            "Draft" => "bg-warning text-warning",
            _ => "bg-secondary text-secondary"
        };
}
