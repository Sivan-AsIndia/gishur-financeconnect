using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using FinanceConnect.Client.Services;
using FinanceConnect.Client.Data;
using static FinanceConnect.Client.ViewModels.RevenueRecognitionViewModel;

namespace FinanceConnect.Client.Pages.Revenue_Expense.RevenueRecognition
{
    public partial class CreateRevenueRecognition : ComponentBase
    {
        [Parameter] public Guid? Id { get; set; }
        [Inject] private RevenueRecognitionService Service { get; set; } = default!;
        [Inject] private RevenueService RevenueService { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        private RevenueRecognitionViewModel.RevenueRecognition Model = new();
        private EditContext _editContext = default!;
        private bool IsEdit => Id.HasValue;

        // ── Lookup data ────────────────────────────────────────────────────────
        private List<RevenueViewModel.Revenue> Revenues = new();

        private Dictionary<Guid, string> CurrencyList = new()
        {
            { MasterDataIds.Currencies.INR, "INR – Indian Rupee" },
            { MasterDataIds.Currencies.USD, "USD – US Dollar" },
            { MasterDataIds.Currencies.GBP, "GBP – British Pound" },
            { MasterDataIds.Currencies.EUR, "EUR – Euro" },
            { MasterDataIds.Currencies.JPY, "JPY – Japanese Yen" },
            { MasterDataIds.Currencies.AUD, "AUD – Australian Dollar" },
            { MasterDataIds.Currencies.AED, "AED – UAE Dirham" },
            { MasterDataIds.Currencies.SGD, "SGD – Singapore Dollar" },
            { MasterDataIds.Currencies.CAD, "CAD – Canadian Dollar" }
        };

        private Dictionary<Guid, string> FiscalYears = new()
        {
            { MasterDataIds.FiscalYears.FY2025_26, "FY 2025-26" },
            { MasterDataIds.FiscalYears.FY2024_25, "FY 2024-25" }
        };

        protected override void OnInitialized()
        {
            Revenues = RevenueService.GetAll();

            if (IsEdit)
            {
                var e = Service.GetById(Id!.Value);
                if (e != null) Model = e;
            }
            else
            {
                Model.CompanyId = MasterDataIds.Companies.SofaCraft;
                Model.TenantId  = MasterDataIds.Tenants.Default;
                Model.CurrencyId = MasterDataIds.Currencies.INR;
                Model.FiscalYearId = MasterDataIds.FiscalYears.FY2025_26;
                Model.RecognitionCode = Service.GenerateCode(MasterDataIds.Companies.SofaCraft);
                Model.PreparedByUserId = "finance.admin";
                Model.PreparedOn = DateTime.Today;
            }
            _editContext = new EditContext(Model);
        }

        private void OnRevenueSelected(ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var revId))
            {
                var rev = Revenues.FirstOrDefault(r => r.RevenueId == revId);
                if (rev != null)
                {
                    Model.RevenueId                   = rev.RevenueId;
                    Model.RevenueCodeSnapshot         = rev.RevenueCode;
                    Model.RevenueNameSnapshot         = rev.RevenueName;
                    Model.CustomerId                  = rev.CustomerId;
                    Model.CustomerNameSnapshot        = rev.CustomerNameSnapshot;
                    Model.SourceDocumentTypeSnapshot   = rev.RevenueSourceDocType.ToString();
                    Model.SourceDocumentNumberSnapshot = rev.SourceDocumentNumber;
                    Model.RevenueTypeSnapshot          = rev.RevenueType.ToString();
                    Model.RevenueNatureSnapshot        = rev.RevenueNature.ToString();
                    Model.SourceGrossRevenueAmount     = rev.GrossRevenueAmount;
                    Model.TotalRecognizableAmount      = rev.GrossRevenueAmount;
                    Model.CurrencyId                   = rev.CurrencyId;
                    StateHasChanged();
                }
            }
        }

        private async Task Save()
        {
            if (!_editContext.Validate())
            {
                await JS.InvokeVoidAsync("eval",
                    "document.querySelector('.validation-message')?.scrollIntoView({behavior:'smooth',block:'center'})");
                return;
            }

            // ── Business validations ───────────────────────────────────────────
            if (Model.TotalRecognizableAmount < 0)
            {
                ToastService.ShowError("Total Recognizable Amount must be >= 0.", "Validation");
                return;
            }

            if (Model.TotalRecognizableAmount > Model.SourceGrossRevenueAmount && Model.AdjustmentAmount is null or 0)
            {
                ToastService.ShowError("Total Recognizable Amount must not exceed Source Revenue Amount.", "Validation");
                return;
            }

            if (Model.RecognitionEndDate.HasValue && Model.RecognitionStartDate.HasValue &&
                Model.RecognitionEndDate < Model.RecognitionStartDate)
            {
                ToastService.ShowError("Recognition End Date must be >= Start Date.", "Validation");
                return;
            }

            if ((Model.RecognitionMethod == RecognitionMethodEnum.Scheduled ||
                 Model.RecognitionMethod == RecognitionMethodEnum.DeferredThenRelease) &&
                !Model.RecognitionStartDate.HasValue)
            {
                ToastService.ShowError("Recognition Start Date is required for Scheduled / DeferredThenRelease methods.", "Validation");
                return;
            }

            if ((Model.RecognitionBasis == RecognitionBasisEnum.StraightLineOverTime ||
                 Model.RecognitionBasis == RecognitionBasisEnum.ServiceCoveragePeriod) &&
                !Model.RecognitionEndDate.HasValue)
            {
                ToastService.ShowError("Recognition End Date is required for StraightLineOverTime / ServiceCoveragePeriod basis.", "Validation");
                return;
            }

            if (Model.RecognitionStatus == RecognitionStatusEnum.Cancelled &&
                string.IsNullOrWhiteSpace(Model.CancellationReason))
            {
                ToastService.ShowError("Cancellation Reason is required when status is Cancelled.", "Validation");
                return;
            }

            try
            {
                if (IsEdit)
                    await Service.UpdateAsync(Model);
                else
                    await Service.CreateAsync(Model);

                ToastService.ShowSuccess(IsEdit ? "Recognition updated" : "Recognition created", "Success");
                Nav.NavigateTo("/revenue-recognitions");
            }
            catch (InvalidOperationException ex)
            {
                ToastService.ShowError(ex.Message, "Error");
            }
        }

        // ── UI Helpers ─────────────────────────────────────────────────────────
        private string GetLineStatusBadge(RecognitionLineStatusEnum s) => s switch
        {
            RecognitionLineStatusEnum.Recognized          => "bg-success-transparent text-success",
            RecognitionLineStatusEnum.PartiallyRecognized => "bg-warning-transparent text-warning",
            RecognitionLineStatusEnum.Ready               => "bg-info-transparent text-info",
            RecognitionLineStatusEnum.Planned             => "bg-secondary-transparent text-secondary",
            RecognitionLineStatusEnum.Deferred            => "bg-primary-transparent text-primary",
            RecognitionLineStatusEnum.Skipped             => "bg-dark-transparent text-dark",
            RecognitionLineStatusEnum.Cancelled           => "bg-danger-transparent text-danger",
            _                                             => "bg-secondary-transparent text-secondary"
        };

        private string GetApprovalBadge(ManualApprovalStatusEnum s) => s switch
        {
            ManualApprovalStatusEnum.Approved    => "bg-success-transparent text-success",
            ManualApprovalStatusEnum.Pending     => "bg-warning-transparent text-warning",
            ManualApprovalStatusEnum.Rejected    => "bg-danger-transparent text-danger",
            ManualApprovalStatusEnum.NotRequired => "bg-secondary-transparent text-secondary",
            _                                    => "bg-secondary-transparent text-secondary"
        };
    }
}
