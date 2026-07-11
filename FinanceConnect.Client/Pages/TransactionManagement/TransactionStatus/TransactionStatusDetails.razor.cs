using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;

namespace FinanceConnect.Client.Pages.TransactionManagement.TransactionStatus
{
    public partial class TransactionStatusDetails
    {
        [Parameter] public Guid StatusId { get; set; }

        TransactionStatusModel? SelectedStatus;

        protected override void OnInitialized()
        {
            SelectedStatus = StatusService.GetById(StatusId);
        }

        void GoBack()
        {
            Nav.NavigateTo("/transaction-status");
        }
    }
}
