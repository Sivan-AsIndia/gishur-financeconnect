using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.RegularExpressions;

namespace FinanceConnect.Client.Pages.Bank_Cash.CashTransfer
{
    public partial class CashTransferList
    {
        private List<CashTransferModel> AllTransfer = new();
        private CashTransferModel? SelectedTransfer;

        private Guid _deleteId;
        private string selectdata;
        private string? SearchText;
        private int PageWindowSize = 2;
        private int StartPage = 1;
        private int CurrentPage = 1;
        private int PageSize = 10;
        private CashTransferModel.CashTransferStatusEnum? selectedStatus;
        private List<CashTransferModel.CashTransferStatusEnum> AvailableStatuses = new();
        private int VisibleColumnCount;
        // Filtered accounts based on search
        private IEnumerable<CashTransferModel> FilteredAccounts =>
     AllTransfer.Where(a =>
         (string.IsNullOrWhiteSpace(SearchText)
             || a.CashTransferNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
             || a.SourceCashAccountName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
             || a.DestinationCashAccountName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
         && (!selectedStatus.HasValue || a.CashTransferStatus == selectedStatus.Value)
     ).OrderByDescending(a => a.UpdatedAt ?? a.CreatedAt);

        // Accounts for current page
        private IEnumerable<CashTransferModel> PagedAccounts =>
            FilteredAccounts
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize);

        private int TotalPages =>
            Math.Max(1, (int)Math.Ceiling((double)FilteredAccounts.Count() / PageSize));

        private int EndPage => Math.Min(StartPage + PageWindowSize - 1, TotalPages);

        protected override async Task OnInitializedAsync()
        {
            AllTransfer = await service.GetListAsync();
            AvailableStatuses = AllTransfer
                .Select(t => t.CashTransferStatus)
                .Distinct()
                .OrderBy(s => s)
                .ToList();
        }
        void OpenRowDetails(CashTransferModel transfer)
        {
            SelectedTransfer = transfer;
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
            VisibleColumnCount =
   await JS.InvokeAsync<int>("getVisibleTableColumns");
        }
        void EditItem(Guid id)
        {
            NavigationManager.NavigateTo($"/cash-transfers/{id}");
        }

        private string GetStatusBadge(string status)
        {
            return status switch
            {
                "Draft" => "bg-warning-transparent text-warning",
                "Submitted" => "bg-info-transparent text-info",
                "Approved" => "bg-success-transparent text-success",
                "Rejected" => "bg-danger-transparent text-danger",
                "Cancelled" => "bg-dark-transparent text-dark",
                "InTransit" => "bg-primary-transparent text-primary",
                "Received" => "bg-success-transparent text-success",
                "Posted" => "bg-success-transparent text-success",
                "Reversed" => "bg-danger-transparent text-danger",
                "Closed" => "bg-secondary-transparent text-secondary",
                _ => "bg-secondary-transparent text-secondary"
            };
        }
        private string GetStatusDotBadge(string status)
        {
            return status switch
            {
                "Draft" => "bg-warning text-warning",
                "Submitted" => "bg-info text-info",
                "Approved" => "bg-success text-success",
                "Rejected" => "bg-danger text-danger",
                "Cancelled" => "bg-dark text-dark",
                "InTransit" => "bg-primary text-primary",
                "Received" => "bg-success text-success",
                "Posted" => "bg-success text-success",
                "Reversed" => "bg-danger text-danger",
                "Closed" => "bg-secondary text-secondary",
                _ => "bg-secondary text-secondary"
            };
        }


        // Search input
        private async Task OnSearch(ChangeEventArgs e)
        {
            SearchText = e.Value?.ToString() ?? "";
            CurrentPage = 1; // reset pagination
            StartPage = 1;
            VisibleColumnCount =
   await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        async Task ViewTransfer(CashTransferModel item)
        {
            SelectedTransfer = item;
            await JS.InvokeVoidAsync("blazorOffcanvas.show", "viewTransferOffcanvas");

        }


        void ConfirmDelete(CashTransferModel CashTransferId)
        {
            _deleteId = CashTransferId.CashTransferId;
            selectdata = CashTransferId.CashTransferNumber;
        }

        async Task DeleteAccount()
        {
            if (_deleteId == Guid.Empty)
                return;

            await CashTransferService.DeleteAsync(_deleteId);

            ToastService.ShowError("Cash Transfer deleted successfully!","Error");

            await LoadList(); 
        }

        async Task LoadList()
        {
            AllTransfer = await CashTransferService.GetListAsync();
        }

        // Reload list
        private Task Reload()
        {
            SearchText = null;
            CurrentPage = 1;
            StartPage = 1;
            selectedStatus = null;
            return Task.CompletedTask;
        }

        // Change page size
        private Task OnPageSizeChange(ChangeEventArgs e)
        {
            PageSize = int.Parse(e.Value!.ToString()!);
            CurrentPage = 1;
            StartPage = 1;
            return Task.CompletedTask;
        }

        // Go to specific page
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
        // Previous page window
        private async Task PreviousPage()
        {
            if (CurrentPage > 1) await GoToPage(CurrentPage - 1);
        }

        private async Task NextPage()
        {
            if (CurrentPage < TotalPages) await GoToPage(CurrentPage + 1);
        }

        private string GetPlainText(string? html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return "-";

            return Regex.Replace(html, "<.*?>", string.Empty);
        }

    }
}
