using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Master.Branch
{
    public partial class BranchDetails
    {
        [Parameter]
        public Guid BranchId { get; set; }

        private BranchModel? SelectedBranch;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("initTooltips");
            }
        }
        protected override void OnInitialized()
        {
            SelectedBranch = service
                .GetById(BranchId);

            if (SelectedBranch == null)
            {
                Nav.NavigateTo("/branches");
            }
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

        private async Task PrintPage()
        {
            await JS.InvokeVoidAsync("window.print");
        }

        void GoBack()
        {
            Nav.NavigateTo("/branches");
        }
    }
}
