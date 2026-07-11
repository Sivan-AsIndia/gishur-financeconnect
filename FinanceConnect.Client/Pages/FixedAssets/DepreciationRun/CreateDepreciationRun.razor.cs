using FinanceConnect.Client.Services;
using FinanceConnect.Client.Shared;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Linq.Expressions;
using System.Transactions;
using static FinanceConnect.Client.ViewModels.AssetsCategoryViewModel;

namespace FinanceConnect.Client.Pages.FixedAssets.DepreciationRun
{
    public partial class CreateDepreciationRun
    {


        private EditContext _editContext;

        [Inject] DepreciationRunService RunService { get; set; } = default!;

        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] AssetCategoryService AssetCategoryService { get; set; } = default!;
        [Inject] BranchService BranchService { get; set; } = default!;
        [Inject] FiscalYearService FiscalYearService { get; set; } = default!;
        [Inject] AccountingPeriodService AccountingPeriodService { get; set; } = default!;

        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject] NavigationManager Nav { get; set; } = default!;

        [Inject] ToastService ToastService { get; set; } = default!;

        Guid TenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        RichTextEditor? _notesEditor;
        [Parameter] public Guid? Id { get; set; }

        DepreciationRunViewModel run = new();

        bool IsEdit => Id.HasValue;

        List<CompanyModel> Companies = new();
        List<BranchModel> Branches = new();
        FiscalYearModel? FiscalYear = new();
        List<AccountingPeriodModel> Periods = new();
        List<AssetsCategoryViewModel.AssetCategory> Categories = new();




        bool ShowScope = true;
        bool ShowFilters = false;
        bool ShowGeneration = false;
        bool ShowAudit = false;
        bool ShowNotes = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("initFeatherIcons");
        }

        protected override void OnInitialized()
        {

            Companies = MasterDataService.GetAllCompanies();

            Categories = AssetCategoryService.GetAll().Where(c=>c.CategoryStatus == CategoryStatus.Active).ToList();


            if (IsEdit)
            {
                run = RunService.GetById(Id.Value);
            }
            else
            {
                run = new DepreciationRunViewModel
                {
                    DepreciationRunId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    RunStatus = DepreciationRunStatus.Draft,
                    RunNumber = DepreciationRunService.GenerateRunNumber()
                };
            }

            _editContext = new EditContext(run);

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



        async Task HandleSubmit()
        {
            // Collect Quill editor values before validation
            if (_notesEditor != null)
                run.RunNotes = await _notesEditor.GetHtmlAsync();

            if (_editContext.Validate())
            {

                if (IsEdit)
                {
                    RunService.Update(run);
                    ToastService.ShowSuccess("Run updated");
                }
                else
                {
                    RunService.Create(run);
                    ToastService.ShowSuccess("Run created");
                }

                Nav.NavigateTo("/depreciation-runs");

            }

        }



        void OnCompanyChanged()
        {
            if (!run.CompanyId.HasValue)
            {
                Branches = new List<BranchModel>();
                Periods = new List<AccountingPeriodModel>();
                return;
            }

            var companyId = run.CompanyId.Value;

            // Load branches
            Branches = BranchService
                .GetByCompanyId(companyId)
                .Where(b => b.Status == "Active")
                .ToList();

            // Get open fiscal year
            FiscalYear = FiscalYearService
                .GetAllByCompanyId(companyId)
                .FirstOrDefault(fy => fy.Status == FiscalYearStatus.Open);

            // Load accounting periods
            if (FiscalYear != null)
            {
                Periods = AccountingPeriodService
                    .GetByFiscalYear(FiscalYear.Id);

            }
            else
            {
                Periods = new List<AccountingPeriodModel>();
            }
        }

        void OnAccountingPeriodChanged()
        {
            var period = Periods.FirstOrDefault(p => p.Id == run.AccountingPeriodId);

            run.PeriodStartDateSnapshot = period?.StartDate;
            run.PeriodEndDateSnapshot = period?.EndDate;
            run.AsOfDate = period?.EndDate;
        }

        void BackToList()
        {
            Nav.NavigateTo("/depreciation-runs");
        }



        void ToggleAccordion(string section)
        {
            switch (section)
            {
                case "scope":
                    ShowScope = !ShowScope;
                    break;

                case "filters":
                    ShowFilters = !ShowFilters;
                    break;

                case "generation":
                    ShowGeneration = !ShowGeneration;
                    break;

                case "audit":
                    ShowAudit = !ShowAudit;
                    break;

                case "notes":
                    ShowNotes = !ShowNotes;
                    break;
            }
        }



        string GetStatusBadge(string status)
        {

            return status switch
            {
                "Draft" => "bg-secondary-transparent text-secondary",
                "Generated" => "bg-info-transparent text-info",
                "Submitted" => "bg-warning-transparent text-warning",
                "Approved" => "bg-primary-transparent text-primary",
                "Posted" => "bg-success-transparent text-success",
                "Finalized" => "bg-dark-transparent text-dark",
                _ => "bg-secondary"
            };

        }
    }
}
