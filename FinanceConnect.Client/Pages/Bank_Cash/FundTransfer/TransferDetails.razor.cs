using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Bank_Cash.FundTransfer
{
    public partial class TransferDetails
    {
        [Parameter] public Guid Id { get; set; }

        private FundTransferModel? SelectedTransfer;

        private void GoBack()
        {
            Nav.NavigateTo("/fund-transfers");
        }
        private async Task PrintPage()
        {
            await JS.InvokeVoidAsync("window.print");
        }
        protected override void OnInitialized()
        {
            SelectedTransfer = FundTransferService.GetById(Id);
        }
    }
}
