using FinanceConnect.Client.Data;
using static FinanceConnect.Client.ViewModels.FinancialPlanViewModel;

namespace FinanceConnect.Client.Services
{
    public class FinancialPlanService
    {
        private readonly List<FinancialPlanListDto> _items = new();

        public FinancialPlanService()
        {
            _items = FinancialPlanSeedData.GetAll();
        }

        public List<FinancialPlanListDto> GetAll()
            => _items.Where(x => !x.IsDeleted).ToList();

        public FinancialPlanListDto? GetById(Guid id)
            => _items.FirstOrDefault(x => x.FinancialPlanId == id && !x.IsDeleted);

        public Task<List<FinancialPlanListDto>> GetAllAsync()
            => Task.FromResult(GetAll());

        public Task<FinancialPlanListDto?> GetByIdAsync(Guid id)
            => Task.FromResult(GetById(id));

        public Task<string> GeneratePlanCode()
        {
            var next = _items.Count + 1;
            return Task.FromResult($"FINPLAN-{DateTime.Now.Year}-{next:D3}");
        }

        public void Add(FinancialPlan model)
        {
            if (_items.Any(x => x.CompanyId == model.CompanyId &&
                string.Equals(x.PlanCode, model.PlanCode, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Plan Code already exists for this Company.");

            var dto = MapToDto(model);
            dto.FinancialPlanId = Guid.NewGuid();
            dto.CreatedAt = DateTime.UtcNow;
            dto.IsDeleted = false;
            _items.Add(dto);
        }

        public Task CreateAsync(FinancialPlan model) { Add(model); return Task.CompletedTask; }

        public void Update(FinancialPlan model)
        {
            var existing = GetById(model.FinancialPlanId);
            if (existing == null) return;
            if (existing.IsLocked) throw new InvalidOperationException("Locked plan cannot be edited.");

            existing.PlanCode = model.PlanCode;
            existing.PlanName = model.PlanName;
            existing.Description = model.Description;
            existing.PlanStatus = model.PlanStatus;
            existing.PlanType = model.PlanType;
            existing.ScenarioType = model.ScenarioType;
            existing.PlanNature = model.PlanNature;
            existing.PlanningScopeLevel = model.PlanningScopeLevel;
            existing.PlanHorizonMode = model.PlanHorizonMode;
            existing.FromDate = model.FromDate;
            existing.ToDate = model.ToDate;
            existing.TargetRevenueAmount = model.TargetRevenueAmount;
            existing.TargetExpenseAmount = model.TargetExpenseAmount;
            existing.TargetGrossProfitAmount = model.TargetGrossProfitAmount;
            existing.TargetOperatingProfitAmount = model.TargetOperatingProfitAmount;
            existing.TargetNetProfitAmount = model.TargetNetProfitAmount;
            existing.TargetCapexAmount = model.TargetCapexAmount;
            existing.TargetCashPositionAmount = model.TargetCashPositionAmount;
            existing.TargetWorkingCapitalAmount = model.TargetWorkingCapitalAmount;
            existing.TargetGrowthPercent = model.TargetGrowthPercent;
            existing.TargetEBITDAPercent = model.TargetEBITDAPercent;
            existing.TargetMarginPercent = model.TargetMarginPercent;
            existing.TargetHeadcount = model.TargetHeadcount;
            existing.TargetInvestmentAmount = model.TargetInvestmentAmount;
            existing.RevenueAssumptionText = model.RevenueAssumptionText;
            existing.ExpenseAssumptionText = model.ExpenseAssumptionText;
            existing.CapexAssumptionText = model.CapexAssumptionText;
            existing.MarketAssumptionText = model.MarketAssumptionText;
            existing.RiskAssumptionText = model.RiskAssumptionText;
            existing.OpportunityAssumptionText = model.OpportunityAssumptionText;
            existing.StrategicNarrative = model.StrategicNarrative;
            existing.VersionNumber = model.VersionNumber;
            existing.RevisionNumber = model.RevisionNumber;
            existing.RevisionReason = model.RevisionReason;
            existing.BusinessUnitCode = model.BusinessUnitCode;
            existing.RegionCode = model.RegionCode;
            existing.BranchGroupCode = model.BranchGroupCode;
            existing.BoardApprovalReference = model.BoardApprovalReference;
            existing.ApprovalNotes = model.ApprovalNotes;
            existing.ManagementNotes = model.ManagementNotes;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        public Task UpdateAsync(FinancialPlan model) { Update(model); return Task.CompletedTask; }

        public Task DeleteAsync(Guid id)
        {
            var item = GetById(id);
            if (item != null) {
                if (item.IsLocked) throw new InvalidOperationException("Locked plan cannot be deleted.");
                item.IsDeleted = true;
            }
            return Task.CompletedTask;
        }

        public Task LockAsync(Guid id) { var i = GetById(id); if (i != null) { i.IsLocked = true; } return Task.CompletedTask; }
        public Task UnlockAsync(Guid id) { var i = GetById(id); if (i != null) { i.IsLocked = false; } return Task.CompletedTask; }
        public Task ArchiveAsync(Guid id) { var i = GetById(id); if (i != null) i.PlanStatus = PlanStatusEnum.Archived; return Task.CompletedTask; }

        private FinancialPlanListDto MapToDto(FinancialPlan m) => new()
        {
            FinancialPlanId = m.FinancialPlanId, CompanyId = m.CompanyId,
            PlanCode = m.PlanCode, PlanName = m.PlanName, Description = m.Description,
            PlanStatus = m.PlanStatus, PlanType = m.PlanType, ScenarioType = m.ScenarioType,
            PlanNature = m.PlanNature, PlanningScopeLevel = m.PlanningScopeLevel,
            PlanHorizonMode = m.PlanHorizonMode, FromDate = m.FromDate, ToDate = m.ToDate,
            TargetRevenueAmount = m.TargetRevenueAmount, TargetExpenseAmount = m.TargetExpenseAmount,
            TargetGrossProfitAmount = m.TargetGrossProfitAmount,
            TargetOperatingProfitAmount = m.TargetOperatingProfitAmount,
            TargetNetProfitAmount = m.TargetNetProfitAmount, TargetCapexAmount = m.TargetCapexAmount,
            TargetCashPositionAmount = m.TargetCashPositionAmount,
            TargetWorkingCapitalAmount = m.TargetWorkingCapitalAmount,
            TargetGrowthPercent = m.TargetGrowthPercent, TargetEBITDAPercent = m.TargetEBITDAPercent,
            TargetMarginPercent = m.TargetMarginPercent, TargetHeadcount = m.TargetHeadcount,
            TargetInvestmentAmount = m.TargetInvestmentAmount,
            VersionNumber = m.VersionNumber, RevisionNumber = m.RevisionNumber,
            IsOfficialApprovedVersion = m.IsOfficialApprovedVersion, IsLocked = m.IsLocked,
            RevenueAssumptionText = m.RevenueAssumptionText,
            ExpenseAssumptionText = m.ExpenseAssumptionText,
            CapexAssumptionText = m.CapexAssumptionText,
            MarketAssumptionText = m.MarketAssumptionText,
            RiskAssumptionText = m.RiskAssumptionText,
            OpportunityAssumptionText = m.OpportunityAssumptionText,
            StrategicNarrative = m.StrategicNarrative,
            BusinessUnitCode = m.BusinessUnitCode, RegionCode = m.RegionCode,
            BranchGroupCode = m.BranchGroupCode, RevisionReason = m.RevisionReason,
            BoardApprovalReference = m.BoardApprovalReference,
            ApprovalNotes = m.ApprovalNotes, ManagementNotes = m.ManagementNotes,
            CurrencyId = m.CurrencyId, FiscalYearId = m.FiscalYearId,
            LinkedBudgetCount = m.LinkedBudgetCount, LinkedForecastCount = m.LinkedForecastCount,
            BudgetTranslationStatus = m.BudgetTranslationStatus,
            StrategicBaselineVarianceFlag = m.StrategicBaselineVarianceFlag,
            PreparedByUserId = m.PreparedByUserId, PreparedOn = m.PreparedOn,
            ReviewedOn = m.ReviewedOn, ApprovedOn = m.ApprovedOn
        };
    }
}
