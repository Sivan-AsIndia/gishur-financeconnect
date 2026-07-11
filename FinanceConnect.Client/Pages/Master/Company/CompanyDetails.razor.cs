using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Master.Company
{
    public partial class CompanyDetails
    {

        [Parameter]
        public Guid CompanyId { get; set; }

        private CompanyModel? SelectedCompany;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("initTooltips");
            }
        }
        protected override void OnInitialized()
        {
            SelectedCompany = MasterDataService
                .GetAllCompanies()
                .FirstOrDefault(c => c.Id == CompanyId);

            if (SelectedCompany == null)
            {
                Nav.NavigateTo("/companies");
            }
        }

        private async Task PrintPage()
        {
            await JS.InvokeVoidAsync("window.print");
        }

        void GoBack()
        {
            Nav.NavigateTo("/companies");   // 🔁 your company list route
        }
        private string GetMonthName(int month)
        {
            return month switch
            {
                1 => "January",
                2 => "February",
                3 => "March",
                4 => "April",
                5 => "May",
                6 => "June",
                7 => "July",
                8 => "August",
                9 => "September",
                10 => "October",
                11 => "November",
                12 => "December",
                _ => "-"
            };
        }
    }

}
