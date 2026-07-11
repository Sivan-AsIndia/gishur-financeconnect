using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using FinanceConnect.Client.Services;
using static FinanceConnect.Client.ViewModels.ForecastViewModel;

namespace FinanceConnect.Client.Pages.Budgeting_CostControl.Forecast
{
    public partial class CreateForecast : ComponentBase
    {
        [Parameter] public Guid? Id { get; set; }
        [Inject] private ForecastService Service { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;

        private ForecastViewModel.Forecast Model = new();
        private EditContext _editContext = default!;
        private bool IsEdit => Id.HasValue;

        protected override void OnInitialized()
        {
            if (IsEdit) { var e = Service.GetById(Id!.Value); if (e != null) Model = e; }
            else { Model.CompanyId=Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"); Model.TenantId=Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"); Model.CurrencyId=Guid.Parse("11111111-1111-1111-1111-111111111111"); Model.FiscalYearId=Guid.Parse("22222222-2222-2222-2222-222222222222"); }
            _editContext = new EditContext(Model);
        }

        private async Task Save()
        {
            if (!_editContext.Validate()) return;
            if (Model.ToDate < Model.FromDate) { ToastService.ShowError("End Date must be >= Start Date","Validation"); return; }
            try { if(IsEdit) await Service.UpdateAsync(Model); else await Service.CreateAsync(Model); ToastService.ShowSuccess(IsEdit?"Updated":"Created","Success"); Nav.NavigateTo("/forecasts"); }
            catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message,"Error"); }
        }
    }
}
