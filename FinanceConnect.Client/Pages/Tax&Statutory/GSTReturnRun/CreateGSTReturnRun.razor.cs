using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.GSTReturnRunViewModel;

namespace FinanceConnect.Client.Pages.Tax___Statutory.GSTReturnRun
{
    public partial class CreateGSTReturnRun
    {
        [Parameter] public Guid? Id { get; set; }
        [Inject] private GSTReturnRunService RunService { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        private GSTReturnRunModel model = new();
        private EditContext _editContext = default!;
        private bool submitted = false;
        private bool IsEdit => Id.HasValue;
        private bool IsReadOnly => IsEdit && model.ReturnRunStatus != "Draft" && model.ReturnRunStatus != "Reopened";

        protected override void OnInitialized()
        {
            if (IsEdit) { var e = RunService.GetById(Id!.Value); if (e != null) model = e; else Nav.NavigateTo("/gst-return-runs"); }
            else { model.CompanyId = Guid.Parse("10000000-0000-0000-0000-000000000001"); model.CompanyName = "Acme Pvt Ltd"; model.BranchId = Guid.Parse("20000000-0000-0000-0000-000000000001"); model.BranchName = "Chennai HQ"; model.ReturnType = "CombinedGSTPack"; model.SelectionMode = "ByPostingDate"; }
            _editContext = new EditContext(model);
        }
        protected override async Task OnAfterRenderAsync(bool firstRender) { await JS.InvokeVoidAsync("feather.replace"); }
        private void HandleSubmit()
        {
            submitted = true; if (!_editContext.Validate()) return;
            if (string.IsNullOrWhiteSpace(model.ReturnPeriodKey) || string.IsNullOrWhiteSpace(model.ReturnType) || string.IsNullOrWhiteSpace(model.SelectionMode) || model.PeriodStartDate == default || model.PeriodEndDate == default) return;
            if (model.PeriodEndDate < model.PeriodStartDate) { ToastService.ShowError("Period End Date must be >= Start Date."); return; }
            try { if (IsEdit) { RunService.UpdateDraft(model); ToastService.ShowSuccess($"{model.ReturnRunNumber} updated."); } else { RunService.Create(model); ToastService.ShowSuccess($"{model.ReturnRunNumber} created."); Nav.NavigateTo($"/gst-return-runs/{model.Id}/view"); } }
            catch (Exception ex) { ToastService.ShowError(ex.Message); }
        }
        private void BackToList() => Nav.NavigateTo("/gst-return-runs");
        private static string GetStatusBadge(string s) => s switch { "Draft"=> "bg-secondary-transparent text-secondary", "Reopened"=>"bg-warning-transparent",_=>"bg-secondary-transparent" };
    }
}
