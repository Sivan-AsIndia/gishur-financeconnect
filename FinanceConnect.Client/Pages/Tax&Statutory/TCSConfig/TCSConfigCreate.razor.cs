using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.TCSConfigViewModel;

namespace FinanceConnect.Client.Pages.Tax_Statutory.TCSConfig
{
    public partial class TCSConfigCreate
    {

        [Parameter] public Guid? Id { get; set; }

        [Inject] private TCSConfigService ConfigService { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;

        private TCSConfigFormDto Form = new();
        private EditContext? _editContext;

        private bool isInitialized = false;
        private bool isSaving = false;
        private string? globalError;

        private bool IsEdit => Id.HasValue && Id != Guid.Empty;

        // ── Accordion flags ───────────────────────────────────────────────────
        private bool ShowGeneral { get; set; } = true;
        private bool ShowSection { get; set; } = false;
        private bool ShowApplicability { get; set; } = false;
        private bool ShowThreshold { get; set; } = false;
        private bool ShowRate { get; set; } = false;
        private bool ShowBase { get; set; } = false;
        private bool ShowAccounting { get; set; } = false;

        private void ToggleAccordion(string section)
        {
            ShowGeneral = section == "general" ? !ShowGeneral : false;
            ShowSection = section == "section" ? !ShowSection : false;
            ShowApplicability = section == "applicability" ? !ShowApplicability : false;
            ShowThreshold = section == "threshold" ? !ShowThreshold : false;
            ShowRate = section == "rate" ? !ShowRate : false;
            ShowBase = section == "base" ? !ShowBase : false;
            ShowAccounting = section == "accounting" ? !ShowAccounting : false;
        }

        protected override async Task OnParametersSetAsync()
        {
            isInitialized = false;
            globalError = null;

            if (IsEdit)
            {
                var dto = await ConfigService.GetFormByIdAsync(Id!.Value);
                if (dto == null) { Nav.NavigateTo("/tcs-config"); return; }
                Form = dto;
            }
            else
            {
                Form = new TCSConfigFormDto
                {
                    EffectiveFrom = DateTime.Today,
                    ConfigStatus = ConfigStatus.Active,
                    Priority = 10,
                    TransactionContext = TCSTransactionContext.SalesInvoice,
                    CollectionTrigger = TCSCollectionTrigger.OnInvoiceBooking,
                    CustomerTypeApplicability = TCSCustomerTypeApplicability.All,
                    ResidentialStatusApplicability = TCSResidentialStatus.All,
                    ThresholdMode = TCSThresholdMode.NoThreshold,
                    ThresholdComparisonRule = TCSThresholdComparisonRule.GreaterThan,
                    FinancialYearBasis = TCSFinancialYearBasis.CompanyFinancialYear,
                    ThresholdComputationBase = TCSThresholdComputationBase.TaxableAmountOnly,
                    RateResolutionMode = TCSRateResolutionMode.FromTaxRateVersion,
                    PanAvailabilityRule = TCSPanAvailabilityRule.UseStandardRateIfPANAvailable,
                    AllowLowerCollectionCertificate = true,
                    AllowNilCollectionCertificate = true,
                    CertificateValidationMode = TCSCertificateValidationMode.ReferenceOnly,
                    DeductionBaseMode = TCSDeductionBaseMode.TaxableLineAmount,
                    ExcludeGSTFromCollectionBase = true,
                    ExcludeNonCollectibleLines = true,
                };
            }

            _editContext = new EditContext(Form);
            isInitialized = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
            => await JS.InvokeVoidAsync("feather.replace");

        private async Task HandleSubmit()
        {
            globalError = null;

            if (string.IsNullOrWhiteSpace(Form.ConfigCode))
            { globalError = "Config Code is required."; ShowGeneral = true; return; }
            if (string.IsNullOrWhiteSpace(Form.ConfigName))
            { globalError = "Config Name is required."; ShowGeneral = true; return; }
            if (Form.Priority <= 0)
            { globalError = "Priority must be greater than 0."; ShowGeneral = true; return; }
            if (Form.EffectiveTo.HasValue && Form.EffectiveTo < Form.EffectiveFrom)
            { globalError = "Effective To must be >= Effective From."; ShowGeneral = true; return; }

            if (string.IsNullOrWhiteSpace(Form.SectionCode))
            { globalError = "TCS Section is required."; ShowSection = true; return; }
            if (string.IsNullOrWhiteSpace(Form.LinkedTaxCodeDisplay))
            { globalError = "Linked TCS Tax Code is required."; ShowSection = true; return; }

            if (Form.ThresholdMode != TCSThresholdMode.NoThreshold && Form.ThresholdAmount == null)
            { globalError = "Threshold Amount is required for the selected Threshold Mode."; ShowThreshold = true; return; }

            if (Form.RateResolutionMode == TCSRateResolutionMode.FixedRateOverride && Form.FixedRatePercent == null)
            { globalError = "Fixed Rate % is required for Fixed Rate Override mode."; ShowRate = true; return; }
            if (Form.PanAvailabilityRule == TCSPanAvailabilityRule.HigherRateIfPANMissing && Form.AlternateRatePercentIfPanMissing == null)
            { globalError = "Alternate Rate (PAN missing) is required."; ShowRate = true; return; }

            if (string.IsNullOrWhiteSpace(Form.TCSPayableGLAccountDisplay))
            { globalError = "TCS Payable GL Account is required."; ShowAccounting = true; return; }
            if (Form.IsLockedForChanges && string.IsNullOrWhiteSpace(Form.LockReason))
            { globalError = "Lock Reason is required when locking."; ShowAccounting = true; return; }

            try
            {
                isSaving = true;
                await ConfigService.SaveAsync(Form);
                ToastService.ShowSuccess(IsEdit ? "TCS Config updated." : "TCS Config created.");
                Nav.NavigateTo("/tcs-config");
            }
            catch (InvalidOperationException ex) { globalError = ex.Message; }
            catch { globalError = "An unexpected error occurred. Please try again."; }
            finally { isSaving = false; }
        }

        private void Cancel() => Nav.NavigateTo("/tcs-config");
    }
}
