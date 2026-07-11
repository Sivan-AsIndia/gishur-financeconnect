using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using FinanceConnect.Client.Services;
using static FinanceConnect.Client.ViewModels.FinancialPlanViewModel;

namespace FinanceConnect.Client.Pages.Budgeting_CostControl.FinancialPlan
{
    public partial class PlanForm : ComponentBase
    {
        [Parameter] public Guid? Id { get; set; }
        [Inject] private FinancialPlanService Service { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;

        private FinancialPlanViewModel.FinancialPlan Model = new();
        private EditContext _editContext = default!;
        private bool IsEdit => Id.HasValue;

        protected override void OnInitialized()
        {
            if (IsEdit)
            {
                var existing = Service.GetById(Id!.Value);
                if (existing != null)
                {
                    Model.FinancialPlanId = existing.FinancialPlanId;
                    Model.PlanCode = existing.PlanCode; Model.PlanName = existing.PlanName;
                    Model.Description = existing.Description; Model.PlanStatus = existing.PlanStatus;
                    Model.PlanType = existing.PlanType; Model.ScenarioType = existing.ScenarioType;
                    Model.PlanNature = existing.PlanNature; Model.PlanningScopeLevel = existing.PlanningScopeLevel;
                    Model.PlanHorizonMode = existing.PlanHorizonMode;
                    Model.FromDate = existing.FromDate; Model.ToDate = existing.ToDate;
                    Model.TargetRevenueAmount = existing.TargetRevenueAmount;
                    Model.TargetExpenseAmount = existing.TargetExpenseAmount;
                    Model.TargetGrossProfitAmount = existing.TargetGrossProfitAmount;
                    Model.TargetOperatingProfitAmount = existing.TargetOperatingProfitAmount;
                    Model.TargetNetProfitAmount = existing.TargetNetProfitAmount;
                    Model.TargetCapexAmount = existing.TargetCapexAmount;
                    Model.TargetCashPositionAmount = existing.TargetCashPositionAmount;
                    Model.TargetWorkingCapitalAmount = existing.TargetWorkingCapitalAmount;
                    Model.TargetGrowthPercent = existing.TargetGrowthPercent;
                    Model.TargetEBITDAPercent = existing.TargetEBITDAPercent;
                    Model.TargetMarginPercent = existing.TargetMarginPercent;
                    Model.TargetHeadcount = existing.TargetHeadcount;
                    Model.TargetInvestmentAmount = existing.TargetInvestmentAmount;
                    Model.RevenueAssumptionText = existing.RevenueAssumptionText;
                    Model.ExpenseAssumptionText = existing.ExpenseAssumptionText;
                    Model.CapexAssumptionText = existing.CapexAssumptionText;
                    Model.MarketAssumptionText = existing.MarketAssumptionText;
                    Model.RiskAssumptionText = existing.RiskAssumptionText;
                    Model.OpportunityAssumptionText = existing.OpportunityAssumptionText;
                    Model.StrategicNarrative = existing.StrategicNarrative;
                    Model.VersionNumber = existing.VersionNumber; Model.RevisionNumber = existing.RevisionNumber;
                    Model.RevisionReason = existing.RevisionReason;
                    Model.BusinessUnitCode = existing.BusinessUnitCode;
                    Model.RegionCode = existing.RegionCode;
                    Model.BranchGroupCode = existing.BranchGroupCode;
                    Model.BoardApprovalReference = existing.BoardApprovalReference;
                    Model.ApprovalNotes = existing.ApprovalNotes;
                    Model.ManagementNotes = existing.ManagementNotes;
                    Model.CompanyId = existing.CompanyId ?? Guid.Empty;
                    Model.CurrencyId = existing.CurrencyId; Model.FiscalYearId = existing.FiscalYearId;
                }
            }
            else
            {
                Model.CompanyId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
                Model.TenantId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
                Model.CurrencyId = Guid.Parse("11111111-1111-1111-1111-111111111111");
                Model.FiscalYearId = Guid.Parse("22222222-2222-2222-2222-222222222222");
            }
            _editContext = new EditContext(Model);
        }

        private async Task Save()
        {
            if (!_editContext.Validate()) return;
            if (Model.ToDate < Model.FromDate) { ToastService.ShowError("End Date must be >= Start Date", "Validation"); return; }
            try { if (IsEdit) await Service.UpdateAsync(Model); else await Service.CreateAsync(Model); ToastService.ShowSuccess(IsEdit ? "Updated" : "Created", "Success"); Nav.NavigateTo("/financial-plans"); }
            catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message, "Error"); }
        }
    }
}
