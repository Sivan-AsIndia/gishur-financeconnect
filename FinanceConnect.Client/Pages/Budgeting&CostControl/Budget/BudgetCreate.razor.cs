using FinanceConnect.Client.Services;
using FinanceConnect.Client.Shared;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Linq.Expressions;
using System.Text;
using static FinanceConnect.Client.Data.MasterDataIds;

namespace FinanceConnect.Client.Pages.Budgeting_CostControl.Budget
{
    public partial class BudgetCreate
    {
        [Inject] BudgetService BudgetService { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] BranchService BranchService { get; set; } = default!;
        [Inject] FiscalYearService FyService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] EmployeeService EmployeeService { get; set; } = default!;

        private EditContext _editContext;
        RichTextEditor? _notesEditor;
        RichTextEditor? _descriptionEditor;
        RichTextEditor? _revisionEditor;
        BudgetViewModel budget = new();

        bool IsEdit => Id.HasValue;

        [Parameter] public Guid? Id { get; set; }

        List<CompanyModel> Companies = new();
        List<BranchModel> Branches = new();
        List<CurrencyModel> Currency = new();
        List<BudgetViewModel> Budgets = new();
        List<FiscalYearModel> Fiscalyears = new();
        List<EmployeeViewModel> Employees = new();

        bool ShowGeneral = true;
        bool ShowScope = false;
        bool ShowPeriod = false;
        bool ShowNotes = false;
        bool ShowClassification = false;
        bool ShowOwnership = false;
        bool ShowVersion = false;
        bool ShowSummary = false;
        bool ShowGovernance = false;
        bool howOwnership  = false;
        bool isSubmitted = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("initFeatherIcons");
        }
        protected override void OnInitialized()
        {
            Companies = MasterDataService.GetAllCompanies();
            Budgets = BudgetService.GetAll();
            Employees = EmployeeService.GetActiveEmployees();
            Currency = MasterDataService.GetAllCurrencies();
            if (IsEdit)
            {
                budget = BudgetService.GetByIdAsync(Id.Value).Result!;
            }
            else
            {
                budget = new BudgetViewModel
                {
                    BudgetId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    Status = "Draft",
                    ScenarioType = ScenarioType.Base
                };
            }

            _editContext = new EditContext(budget);
            CalculateTotalPeriods();
        }

        Task OnCompanyChanged()
        {
            if (!budget.CompanyId.HasValue)
            {
                Branches = new List<BranchModel>();
                return Task.CompletedTask;
            }

            var companyId = budget.CompanyId.Value;

            Branches = BranchService
                .GetByCompanyId(companyId)
                .Where(b => b.Status == "Active")
                .ToList();

            Fiscalyears = FyService.GetAllByCompanyId(companyId);

            return Task.CompletedTask;
        }


        Task OnPeriodChanged()
        {
            CalculateTotalPeriods();
            return Task.CompletedTask;
        }

        void CalculateTotalPeriods()
        {
            if (!budget.StartDate.HasValue || !budget.EndDate.HasValue)
            {
                budget.TotalPeriodsPlanned = 0;
                return;
            }

            var start = budget.StartDate.Value;
            var end = budget.EndDate.Value;

            if (end < start)
            {
                budget.TotalPeriodsPlanned = 0;
                return;
            }

            switch (budget.PeriodGranularity)
            {
                case PeriodGranularity.Monthly:
                    budget.TotalPeriodsPlanned =
                        ((end.Year - start.Year) * 12) + end.Month - start.Month + 1;
                    break;

                case PeriodGranularity.Quarterly:
                    var totalMonthsQ =
                        ((end.Year - start.Year) * 12) + end.Month - start.Month + 1;
                    budget.TotalPeriodsPlanned = (int)Math.Ceiling(totalMonthsQ / 3.0);
                    break;

                case PeriodGranularity.HalfYearly:
                    var totalMonthsH =
                        ((end.Year - start.Year) * 12) + end.Month - start.Month + 1;
                    budget.TotalPeriodsPlanned = (int)Math.Ceiling(totalMonthsH / 6.0);
                    break;

                case PeriodGranularity.Yearly:
                    budget.TotalPeriodsPlanned = end.Year - start.Year + 1;
                    break;

                case PeriodGranularity.Custom:
                    budget.TotalPeriodsPlanned = 1;
                    break;

                default:
                    budget.TotalPeriodsPlanned = 0;
                    break;
            }
        }


        void ToggleAccordion(string section)
        {
            switch (section)
            {
                case "general":
                    ShowGeneral = !ShowGeneral;
                    break;

                case "scope":
                    ShowScope = !ShowScope;
                    break;

                case "period":
                    ShowPeriod = !ShowPeriod;
                    break;

                case "notes":
                    ShowNotes = !ShowNotes;
                    break;

                case "Ownership":
                    ShowOwnership = !ShowOwnership;
                    break;
                case "version":
                    ShowVersion = !ShowVersion;
                    break;
                case "Summary":
                    ShowSummary = !ShowSummary;
                    break;
            }
        }

        async Task HandleSubmit()
        {
            isSubmitted = true;
            if (_notesEditor != null)
            {
                budget.Notes = await _notesEditor.GetHtmlAsync();
            }
            if (_descriptionEditor != null)
            {
                budget.Description = await _descriptionEditor.GetHtmlAsync();
            }
            if (_revisionEditor != null)
            {
                budget.RevisionReason = await _revisionEditor.GetHtmlAsync();
            }
            if (_editContext.Validate())
            {
                await ContinueSave();
                return;
            }

            // ✅ Section-wise validation navigation
            if (HasGeneralErrors())
                OpenAccordion("general");
            else if (HasScopeErrors())
                OpenAccordion("scope");
            else if (HasPeriodErrors())
                OpenAccordion("period");
            else if (HasOwnershipErrors())
                OpenAccordion("Ownership");
            else if (HasVersionErrors())
                OpenAccordion("version");


            await InvokeAsync(StateHasChanged);


        }

        bool HasVersionErrors()
        {
            return HasError(() => budget.PreviousBudgetId);
        }
        bool HasGeneralErrors()
        {
            return HasError(() => budget.BudgetCode) ||
                   HasError(() => budget.BudgetName);
        }

        bool HasClassificationErrors()
        {
            return HasError(() => budget.BudgetType) ||
                   HasError(() => budget.BudgetNature) ||
                   HasError(() => budget.PlanningLevel);
        }

        bool HasScopeErrors()
        {
            return HasError(() => budget.CompanyId);
        }

        bool HasPeriodErrors()
        {
            return HasError(() => budget.FiscalYearId) ||
                   HasError(() => budget.StartDate) ||
                   HasError(() => budget.EndDate) ||
                   HasError(() => budget.PeriodGranularity);
        }
        bool HasOwnershipErrors()
        {
            return HasError(() => budget.BudgetOwnerUserId);
        }

        bool HasError(Expression<Func<object>> field)
        {
            var fieldIdentifier = FieldIdentifier.Create(field);
            return _editContext.GetValidationMessages(fieldIdentifier).Any();
        }

        void OpenAccordion(string section)
        {
            // Close all sections first
            ShowGeneral = false;
            ShowScope = false;
            ShowPeriod = false;
            ShowNotes = false;
            ShowOwnership = false;
            ShowVersion = false;
            ShowSummary = false;

            // Open target section
            switch (section)
            {
                case "general":
                    ShowGeneral = true;
                    break;


                case "scope":
                    ShowScope = true;
                    break;

                case "period":
                    ShowPeriod = true;
                    break;

                case "Ownership":
                    ShowOwnership = true;
                    break;

                case "version":
                    ShowVersion = true;
                    break;

                case "Summary":
                    ShowSummary = true;
                    break;

                case "notes":
                    ShowNotes = true;
                    break;
            }
        }
        async Task ContinueSave()
        {
            // Core validation
            if (budget.StartDate.HasValue && budget.EndDate.HasValue &&
                budget.EndDate < budget.StartDate)
            {
                ToastService.ShowError("Budget end date cannot be earlier than start date.");
                return;
            }

            if (budget.BudgetOwnerUserId == Guid.Empty)
            {
                ToastService.ShowError("Budget owner is required.");
                return;
            }

            if (budget.CurrencyId == Guid.Empty)
            {
                ToastService.ShowError("Currency is required.");
                return;
            }

            // Scope validations
            if (budget.PlanningLevel == PlanningLevel.Branch && budget.BranchId == null)
            {
                ToastService.ShowError("Branch is required for Branch level budget.");
                return;
            }

            if (budget.PlanningLevel == PlanningLevel.Department && budget.DepartmentId == null)
            {
                ToastService.ShowError("Department is required for Department level budget.");
                return;
            }

            if (budget.PlanningLevel == PlanningLevel.Project && budget.ProjectId == null)
            {
                ToastService.ShowError("Project is required for Project level budget.");
                return;
            }

            // Version validation
            if (budget.BudgetType != BudgetType.Original && budget.PreviousBudgetId == null)
            {
                ToastService.ShowError("Revised budget must reference a previous approved budget.");
                return;
            }

            try
            {
                if (IsEdit)
                    await BudgetService.UpdateAsync(budget);
                else
                    await BudgetService.CreateAsync(budget);

                Nav.NavigateTo("/budgets");
            }
            catch (Exception ex)
            {
                ToastService.ShowError(ex.Message);
            }
        }



        void BackToList()
        {
            Nav.NavigateTo("/budgets");
        }

        private string GetValidationClass(Expression<Func<object>> field)
        {
            if (_editContext == null)
                return string.Empty;

            var fieldIdentifier = FieldIdentifier.Create(field);

            var hasError = _editContext.GetValidationMessages(fieldIdentifier).Any();
            var isModified = _editContext.IsModified(fieldIdentifier);

            if (hasError)
                return "is-invalid";

            if (isModified)
                return "is-valid";

            return string.Empty;
        }


        string GetStatusBadge(string status)
        {
            return status switch
            {
                "Draft" => "bg-secondary-transparent text-dark",
                "Submitted" => "bg-warning-transparent",
                "Approved" => "bg-primary-transparent",
                "Locked" => "bg-success-transparent",
                _ => "bg-secondary"
            };
        }
    }
}
