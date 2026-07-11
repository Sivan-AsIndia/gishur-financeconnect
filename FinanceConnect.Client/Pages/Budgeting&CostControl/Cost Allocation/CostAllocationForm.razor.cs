using FinanceConnect.Client.Data;
using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
namespace FinanceConnect.Client.Pages.Budgeting_CostControl.Cost_Allocation
{
    public partial class CostAllocationForm
    {
        [Parameter] public Guid? Id { get; set; }
        private bool _submitted = false;

        [Inject] private CostAllocationService Service { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;

        private CostAllocationViewModel Model = new();
        private EditContext _editContext = default!;
        private bool IsEdit => Id.HasValue;

        // ── Line management ────────────────────────────────────────────────────

        private List<CostAllocationLine> Lines { get; set; } = new();
        private int _nextLineNumber = 10;

        private decimal TotalPercent
            => Lines.Sum(l => l.AllocationPercent ?? 0);

        private decimal TotalAllocated
            => Lines.Sum(l => l.AllocatedAmount);

        private bool IsBalanced
            => Model.SourceAmount > 0 &&
               Math.Abs(Model.SourceAmount - TotalAllocated) < 0.01m;

        // ── Lookup options ─────────────────────────────────────────────────────

        private Dictionary<Guid, string> CostCenterOptions
            => CostAllocationSeedData.CostCenterNames;

        // ── Lifecycle ──────────────────────────────────────────────────────────

        protected override void OnInitialized()
        {
            if (IsEdit)
            {
                var existing = Service.GetById(Id!.Value);
                if (existing != null)
                {
                    Model.CostAllocationId = existing.CostAllocationId;
                    Model.AllocationCode = existing.AllocationCode;
                    Model.AllocationName = existing.AllocationName;
                    Model.Description = existing.AllocationAssumptionText;
                    Model.AllocationType = existing.AllocationType;
                    Model.AllocationStatus = existing.AllocationStatus;
                    Model.AllocationDate = existing.AllocationDate;
                    Model.EffectiveDate = existing.EffectiveDate;
                    Model.ScopeType = existing.ScopeType;
                    Model.SourceAmount = existing.SourceAmount;
                    Model.SourceAmountType = existing.SourceAmountType;
                    Model.SourceReferenceText = existing.SourceReferenceText;
                    Model.AllocationMethod = existing.AllocationMethod;
                    Model.AllocationBasisType = AllocationBasisType.Static;
                    Model.RoundingRule = existing.RoundingRule;
                    Model.MustFullyAllocateSource = existing.MustFullyAllocateSource;
                    Model.AllocationAssumptionText = existing.AllocationAssumptionText;
                    Model.Notes = existing.Notes;
                    Lines = existing.Lines.ToList();
                    _nextLineNumber = Lines.Any() ? Lines.Max(l => l.LineNumber) + 10 : 10;
                }
            }
            else
            {
                Model.CompanyId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
                Model.TenantId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
                Model.FiscalYearId = Guid.Parse("22222222-2222-2222-2222-222222222222");
                Model.AllocationDate = DateTime.Today;
                Model.EffectiveDate = DateTime.Today;
                Model.AllocationStatus = AllocationStatus.Draft;
                Model.AllocationMethod = AllocationMethod.FixedPercentage;
                Model.AllocationBasisType = AllocationBasisType.Static;
                Model.RoundingRule = RoundingRule.ResidualToLastLine;
                Model.MustFullyAllocateSource = true;
                Model.PreparedByUserId = Guid.Parse("ffffffff-0000-0000-0000-000000000001");
            }

            _editContext = new EditContext(Model);
        }

        // ── Line helpers ───────────────────────────────────────────────────────

        private void AddLine()
        {
            Lines.Add(new CostAllocationLine
            {
                CostAllocationLineId = Guid.NewGuid(),
                CostAllocationId = Model.CostAllocationId,
                LineNumber = _nextLineNumber,
                AllocationLineStatus = AllocationLineStatus.Draft,
            });
            _nextLineNumber += 10;
        }

        private void RemoveLine(CostAllocationLine line)
            => Lines.Remove(line);

        private void OnCostCenterChanged(CostAllocationLine line)
        {
            // TargetCostCenterId is already bound via @bind
            line.TargetCostCenterName = CostCenterOptions.GetValueOrDefault(line.TargetCostCenterId) ?? "";
        }

        private void OnPercentChanged(CostAllocationLine line)
        {
            // AllocationPercent is already bound via @bind
            if (line.AllocationPercent.HasValue && Model.SourceAmount > 0)
            {
                line.AllocatedAmount = Math.Round(Model.SourceAmount * line.AllocationPercent.Value / 100, 2);
            }
        }

        // ── Save ───────────────────────────────────────────────────────────────

        private async Task Save()
        {
            _submitted = true;
            if (!_editContext.Validate()) return;

            if (!Lines.Any())
            {
                ToastService.ShowError("At least one target line is required.", "Validation");
                return;
            }

            if (Lines.Any(l => l.ManualOverrideFlag && string.IsNullOrWhiteSpace(l.ManualOverrideReason)))
            {
                ToastService.ShowError("Manual override reason is required for overridden lines.", "Validation");
                return;
            }

            Model.Lines = Lines;

            try
            {
                if (IsEdit)
                    await Service.UpdateAsync(Model);
                else
                    await Service.CreateAsync(Model);

                ToastService.ShowSuccess(IsEdit ? "Allocation updated." : "Allocation created.", "Success");
                Nav.NavigateTo("/cost-allocations");
            }
            catch (InvalidOperationException ex)
            {
                ToastService.ShowError(ex.Message, "Error");
            }
        }
    }
}
