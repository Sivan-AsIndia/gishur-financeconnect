using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Master.ExchangeRate
{



    public partial class ViewExchangeRate
    {
        [Parameter] public Guid RateId { get; set; }

        ExchangeRateModel? SelectedRate;

        protected override async Task OnInitializedAsync()
        {
            SelectedRate = MasterDataService.GetExchangeRateById(RateId);
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("initTooltips");
            }
        }

        private async Task PrintPage()
        {
            await JS.InvokeVoidAsync("window.print");
        }
        void GoBack()
        {
            Nav.NavigateTo("/exchange-rates");
        }
        string GetStatusBadge(string? status) => status?.ToLower() switch
        {
            "active" => "bg-success",
            "inactive" => "bg-secondary",
            "pending" => "bg-warning",
            _ => "bg-muted"
        };

    }


}
