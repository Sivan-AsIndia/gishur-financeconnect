using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Bank_Cash.cheque
{
    public partial class ChequeList
    {

        List<ChequeModel> cheques = new();

        private string? SearchText;
        private int PageWindowSize = 2;
        private int StartPage = 1;
        private int CurrentPage = 1;
        private int PageSize = 10;
        private ChequeStatus? selectedStatus;
        private List<ChequeStatus> AvailableStatuses = new();
        private ChequeModel? SelectedCheque;
        private ChequeModel? ActionCheque;
        private int VisibleColumnCount;
        private IEnumerable<ChequeModel> FilteredAccounts =>
      cheques.Where(c =>
       (!selectedStatus.HasValue || c.Status == selectedStatus.Value)
          &&
          (
              string.IsNullOrWhiteSpace(SearchText)
              ||
              c.ChequeNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
              ||
              c.ChequeDate.ToShortDateString()
                  .Contains(SearchText, StringComparison.OrdinalIgnoreCase)
              ||
              c.CounterpartyName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
              ||
              c.OurBankAccount.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
              ||
              c.Amount.ToString()
                  .Contains(SearchText, StringComparison.OrdinalIgnoreCase)
              ||
              c.Status.ToString()
                  .Contains(SearchText, StringComparison.OrdinalIgnoreCase)
          )
      ).OrderByDescending(c => c.UpdatedAt ?? c.CreatedAt);

        private IEnumerable<ChequeModel> PagedAccounts =>
            FilteredAccounts
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize);

        private int TotalPages =>
            Math.Max(1, (int)Math.Ceiling((double)FilteredAccounts.Count() / PageSize));

        private int EndPage => Math.Min(StartPage + PageWindowSize - 1, TotalPages);


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
            VisibleColumnCount =
   await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        void OpenRowDetails(ChequeModel cheque)
        {
            SelectedCheque = cheque;
        }

        void ViewChequeDetails(ChequeModel cheque)
        {
            Nav.NavigateTo($"/cheques/{cheque.Id}/view");
        }

        void OpenActionModal(ChequeModel cheque)
        {
            ActionCheque = cheque;
        }

        protected override void OnInitialized()
        {
            cheques = Service.GetAll();
            AvailableStatuses = cheques
                .Select(c => c.Status)
                .Distinct()
                .OrderBy(s => s)
                .ToList();
        }

        // ================= CONFIRMATION ACTION HANDLERS =================

        void ConfirmPrint()
        {
            if (ActionCheque == null) return;
            Service.MarkPrinted(ActionCheque.Id);
            Refresh();
            ToastService.ShowSuccess("Printed Successfully", "Success");
        }

        void ConfirmIssue()
        {
            if (ActionCheque == null) return;
            Service.MarkIssued(ActionCheque.Id);
            Refresh();
            ToastService.ShowSuccess("Cheque Issued Successfully", "Success");
        }

        void ConfirmDeposit()
        {
            if (ActionCheque == null) return;
            Service.MarkDeposited(ActionCheque.Id);
            Refresh();
            ToastService.ShowSuccess("Deposited Successfully", "Success");
        }

        void ConfirmClear()
        {
            if (ActionCheque == null) return;
            Service.MarkCleared(ActionCheque.Id);
            Refresh();
            ToastService.ShowSuccess("Cheque Cleared Successfully", "Success");
        }

        void ConfirmBounce()
        {
            if (ActionCheque == null) return;
            Service.MarkBounced(ActionCheque.Id, "Insufficient Funds");
            Refresh();
            ToastService.ShowError("Cheque Bounced", "Error");
        }

        void ConfirmReissue()
        {
            if (ActionCheque == null) return;
            var c = Service.GetById(ActionCheque.Id);
            if (c == null) return;

            c.Status = ChequeStatus.Printed;
            c.BouncedOn = null;
            Refresh();
            ToastService.ShowWarning("Cheque Reissued", "Warning");
        }

        void Refresh()
        {
            cheques = Service.GetAll();
            StateHasChanged();
        }
        private string GetStatusBadge(ChequeStatus status)
        {
            return status switch
            {
                ChequeStatus.Draft => "bg-warning-transparent text-warning",
                ChequeStatus.Prepared => "bg-info-transparent text-info",
                ChequeStatus.Printed => "bg-info-transparent text-info",
                ChequeStatus.Issued => "bg-primary-transparent text-primary",
                ChequeStatus.Received => "bg-success-transparent text-success",
                ChequeStatus.Deposited => "bg-success-transparent text-success",
                ChequeStatus.Presented => "bg-info-transparent text-info",
                ChequeStatus.Cleared => "bg-success-transparent text-success",
                ChequeStatus.Bounced => "bg-danger-transparent text-danger",
                ChequeStatus.Stopped => "bg-secondary-transparent text-secondary",
                ChequeStatus.Cancelled => "bg-dark-transparent text-dark",
                ChequeStatus.Stale => "bg-secondary-transparent text-secondary",
                ChequeStatus.Reissued => "bg-primary-transparent text-primary",
                _ => "bg-secondary-transparent text-secondary"
            };
        }
        private string GetStatusDotBadge(ChequeStatus status)
        {
            return status switch
            {
                ChequeStatus.Draft => "bg-warning text-warning",
                ChequeStatus.Prepared => "bg-info text-info",
                ChequeStatus.Printed => "bg-info text-info",
                ChequeStatus.Issued => "bg-primary text-primary",
                ChequeStatus.Received => "bg-success text-success",
                ChequeStatus.Deposited => "bg-success text-success",
                ChequeStatus.Presented => "bg-info text-info",
                ChequeStatus.Cleared => "bg-success text-success",
                ChequeStatus.Bounced => "bg-danger text-danger",
                ChequeStatus.Stopped => "bg-secondary text-secondary",
                ChequeStatus.Cancelled => "bg-dark text-dark",
                ChequeStatus.Stale => "bg-secondary text-secondary",
                ChequeStatus.Reissued => "bg-primary text-primary",
                _ => "bg-secondary text-secondary"
            };
        }

        // Search input
        private async Task OnSearch(ChangeEventArgs e)
        {
            SearchText = e.Value?.ToString() ?? "";
            CurrentPage = 1;
            StartPage = 1;
            VisibleColumnCount =
   await JS.InvokeAsync<int>("getVisibleTableColumns");
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

    }
}
