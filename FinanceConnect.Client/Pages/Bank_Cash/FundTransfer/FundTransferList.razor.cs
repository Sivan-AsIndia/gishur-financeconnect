using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Bank_Cash.FundTransfer
{
    public partial class FundTransferList
    {
        [Inject] private FundTransferService FundTransferService { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;
        private int VisibleColumnCount;
        private List<FundTransferModel> AllTransfer = new();
        private FundTransferModel? SelectedTransfer;

        private Guid _deleteId;
        private string selectdata = "";
        private string? SearchText;
        private int PageWindowSize = 2;
        private int StartPage = 1;
        private int CurrentPage = 1;
        private int PageSize = 10;
        private FundTransferStatus? selectedStatus;

        private List<FundTransferStatus> AvailableStatuses =>
            AllTransfer.Where(a => a.Status.HasValue).Select(a => a.Status!.Value).Distinct().OrderBy(s => s).ToList();

        private IEnumerable<FundTransferModel> FilteredAccounts =>
            AllTransfer.Where(a =>
                (string.IsNullOrWhiteSpace(SearchText)
                 || a.FundTransferNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                 || a.SourceBankAccount.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                 || a.DestinationBankAccount.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                && (!selectedStatus.HasValue || a.Status == selectedStatus.Value)
            ).OrderByDescending(a => a.UpdatedAt ?? a.CreatedAt);

        private IEnumerable<FundTransferModel> PagedAccounts =>
            FilteredAccounts
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize);
        private bool _rerenderIcons;

        private int TotalPages => Math.Max(1, (int)Math.Ceiling((double)FilteredAccounts.Count() / PageSize));
        private int EndPage => Math.Min(StartPage + PageWindowSize - 1, TotalPages);

        protected override void OnInitialized()
        {
            LoadList();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender || _rerenderIcons)
            {
                _rerenderIcons = false;

                await JS.InvokeVoidAsync("feather.replace");
                await JS.InvokeVoidAsync("initTooltips",true);
                VisibleColumnCount =
   await JS.InvokeAsync<int>("getVisibleTableColumns");
            }
        }


        private async void LoadList()
        {
            AllTransfer = FundTransferService.GetList();
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
        }

        private void OpenRowDetails(FundTransferModel transfer) => SelectedTransfer = transfer;

        private void EditItem(Guid id) => NavigationManager.NavigateTo($"/fund-transfers/{id}");

        private void ViewTransfer(FundTransferModel item)
        {
            NavigationManager.NavigateTo($"/fund-transfers/{item.FundTransferId}/view");
        }


        private void ConfirmDelete(FundTransferModel item)
        {
            _deleteId = item.FundTransferId;
            selectdata = item.FundTransferNumber;
        }

        private void DeleteAccount()
        {
            if (_deleteId == Guid.Empty) return;

            FundTransferService.Delete(_deleteId);
            ToastService.ShowError("Fund Transfer deleted successfully!","Error");
            LoadList();
        }

        private async void Reload()
        {
            SearchText = null;
            CurrentPage = 1;
            StartPage = 1;
            selectedStatus = null;
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
        }

        private void OnPageSizeChange(ChangeEventArgs e)
        {
            PageSize = int.Parse(e.Value!.ToString()!);
            CurrentPage = 1;
            StartPage = 1;
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
        // Previous page window
        private async Task PreviousPage()
        {
            if (CurrentPage > 1) await GoToPage(CurrentPage - 1);
        }

        private async Task NextPage()
        {
            if (CurrentPage < TotalPages) await GoToPage(CurrentPage + 1);
        }



        private async void OnSearch(ChangeEventArgs e)
        {
            SearchText = e.Value?.ToString() ?? "";
            CurrentPage = 1;
            StartPage = 1;
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
            VisibleColumnCount =
   await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        private string GetFundTransferStatusIcon(FundTransferStatus? status) => status switch
        {
            FundTransferStatus.Draft => "ti ti-file-text",
            FundTransferStatus.Submitted => "ti ti-send",
            FundTransferStatus.Approved => "ti ti-check",
            FundTransferStatus.Rejected => "ti ti-x",
            FundTransferStatus.Cancelled => "ti ti-ban",
            FundTransferStatus.Initiated => "ti ti-player-play",
            FundTransferStatus.InTransit => "ti ti-truck",
            FundTransferStatus.Completed => "ti ti-circle-check",
            FundTransferStatus.Posted => "ti ti-circle-check",
            FundTransferStatus.Failed => "ti ti-alert-circle",
            FundTransferStatus.Reversed => "ti ti-refresh",
            FundTransferStatus.Closed => "ti ti-lock",
            _ => "ti ti-info-circle"
        };

        private string GetStatusBadge(FundTransferStatus? status) => status switch
        {
            FundTransferStatus.Draft => "bg-warning-transparent text-warning",
            FundTransferStatus.Submitted => "bg-info-transparent text-info",
            FundTransferStatus.Approved => "bg-success-transparent text-success",
            FundTransferStatus.Rejected => "bg-danger-transparent text-danger",
            FundTransferStatus.Cancelled => "bg-dark-transparent text-dark",
            FundTransferStatus.Initiated => "bg-primary-transparent text-primary",
            FundTransferStatus.InTransit => "bg-primary-transparent text-primary",
            FundTransferStatus.Completed => "bg-success-transparent text-success",
            FundTransferStatus.Posted => "bg-success-transparent text-success",
            FundTransferStatus.Failed => "bg-danger-transparent text-danger",
            FundTransferStatus.Reversed => "bg-danger-transparent text-danger",
            FundTransferStatus.Closed => "bg-secondary-transparent text-secondary",
            _ => "bg-secondary-transparent text-secondary"
        };
    }
}
