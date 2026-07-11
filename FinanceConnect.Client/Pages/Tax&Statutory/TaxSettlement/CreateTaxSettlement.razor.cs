using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.TaxSettlementViewModel;

namespace FinanceConnect.Client.Pages.Tax___Statutory.TaxSettlement
{
    public partial class CreateTaxSettlement
    {
        [Parameter] public Guid? Id { get; set; }
        [Inject] private TaxSettlementService SettlementService { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        private TaxSettlementModel model = new();
        private EditContext _editContext = default!;
        private bool submitted = false;
        private bool IsEdit => Id.HasValue;
        private bool IsReadOnly => IsEdit && model.SettlementStatus != "Draft";

        protected override void OnInitialized()
        {
            if (IsEdit) { var e = SettlementService.GetById(Id!.Value); if (e != null) model = e; else Nav.NavigateTo("/tax-settlements"); }
            else { model.CompanyId = Guid.Parse("10000000-0000-0000-0000-000000000001"); model.CompanyName = "Acme Pvt Ltd"; model.BranchId = Guid.Parse("20000000-0000-0000-0000-000000000001"); model.BranchName = "Chennai HQ"; }
            _editContext = new EditContext(model);
        }
        protected override async Task OnAfterRenderAsync(bool firstRender) { await JS.InvokeVoidAsync("feather.replace"); }
        private void HandleSubmit()
        {
            submitted = true; if (!_editContext.Validate()) return;
            if (string.IsNullOrWhiteSpace(model.SettlementType) || string.IsNullOrWhiteSpace(model.TaxPeriodKey) || string.IsNullOrWhiteSpace(model.TaxTypeScope) || string.IsNullOrWhiteSpace(model.PaymentMode) || string.IsNullOrWhiteSpace(model.GovernmentAuthorityType)) return;
            try { if (IsEdit) { SettlementService.UpdateDraft(model); ToastService.ShowSuccess($"{model.SettlementNumber} updated."); } else { SettlementService.Create(model); ToastService.ShowSuccess($"{model.SettlementNumber} created."); Nav.NavigateTo($"/tax-settlements/{model.Id}/view"); } }
            catch (Exception ex) { ToastService.ShowError(ex.Message); }
        }
        private void BackToList() => Nav.NavigateTo("/tax-settlements");
        private static string GetStatusBadge(string s) => s switch {
            "Draft"=> "bg-secondary-transparent text-secondary",
            "Posted"=>"bg-success-transparent",
            "Reversed"=>"bg-warning-transparent",
            _=>"bg-secondary-transparent" };
    }
}
