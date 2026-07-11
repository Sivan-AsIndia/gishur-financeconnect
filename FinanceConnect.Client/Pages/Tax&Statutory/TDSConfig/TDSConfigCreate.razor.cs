using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.TdsConfigViewModel;

namespace FinanceConnect.Client.Pages.Tax_Statutory.TDSConfig
{
    public partial class TDSConfigCreate
    {
        [Parameter] public Guid? Id { get; set; }

        [Inject] private TDSConfigService ConfigService { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;

        private TDSConfigFormDto Form = new();
        private EditContext? _editContext;

        private bool isInitialized = false;
        private bool isSaving = false;
        private string? globalError;

        private bool IsEdit => Id.HasValue && Id != Guid.Empty;

        // ── Accordion flags ───────────────────────────────────────────────────
        private bool ShowGeneral { get; set; } = true;
        private bool ShowApplicability { get; set; } = false;
        private bool ShowThreshold { get; set; } = false;
        private bool ShowRate { get; set; } = false;
        private bool ShowAccounting { get; set; } = false;
        private bool ShowGovernance { get; set; } = false;

        private void ToggleAccordion(string section)
        {
            ShowGeneral = section == "general" ? !ShowGeneral : false;
            ShowApplicability = section == "applicability" ? !ShowApplicability : false;
            ShowThreshold = section == "threshold" ? !ShowThreshold : false;
            ShowRate = section == "rate" ? !ShowRate : false;
            ShowAccounting = section == "accounting" ? !ShowAccounting : false;
            ShowGovernance = section == "governance" ? !ShowGovernance : false;
        }

        // ── Lifecycle ─────────────────────────────────────────────────────────
        protected override async Task OnParametersSetAsync()
        {
            isInitialized = false;
            globalError = null;

            if (IsEdit)
            {
                var dto = await ConfigService.GetFormByIdAsync(Id!.Value);
                if (dto == null) { Nav.NavigateTo("/tds-config"); return; }
                Form = dto;
            }
            else
            {
                Form = new TDSConfigFormDto
                {
                    EffectiveFrom = DateTime.Today,
                    ConfigStatus = ConfigStatus.Active,
                    Priority = 10,
                    PartyApplicability = PartyApplicability.Both,
                    VendorTypeFilter = VendorTypeFilter.Any,
                    ExpenseNatureFilter = ExpenseNatureFilter.Any,
                    APDocumentContext = APDocumentContext.Both,
                    DeductionTriggerBasis = DeductionTriggerBasis.OnPayment,
                    BaseAmountMode = BaseAmountMode.TaxableAmountExcludingGST,
                    ThresholdEvaluationMode = ThresholdEvaluationMode.NoThreshold,
                    ApplyOnlyAboveThreshold = true,
                    RateSourceMode = TDSRateSourceMode.FromTaxRateVersion,
                    PanValidationMode = PanValidationMode.PresenceOnly,
                    AllowGLOverrideByPostingRule = true,
                };
            }

            _editContext = new EditContext(Form);
            isInitialized = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
            => await JS.InvokeVoidAsync("feather.replace");

        // ── Submit ────────────────────────────────────────────────────────────
        private async Task HandleSubmit()
        {
            globalError = null;

            // Basic cross-field validations
            if (string.IsNullOrWhiteSpace(Form.ConfigCode))
            { globalError = "Config Code is required."; ShowGeneral = true; return; }

            if (string.IsNullOrWhiteSpace(Form.ConfigName))
            { globalError = "Config Name is required."; ShowGeneral = true; return; }

            if (string.IsNullOrWhiteSpace(Form.SectionCode))
            { globalError = "TDS Section is required."; ShowGeneral = true; return; }

            if (Form.Priority <= 0)
            { globalError = "Priority must be greater than 0."; ShowGeneral = true; return; }

            if (Form.EffectiveTo.HasValue && Form.EffectiveTo < Form.EffectiveFrom)
            { globalError = "Effective To must be >= Effective From."; ShowGeneral = true; return; }

            if (Form.RateSourceMode == TDSRateSourceMode.FixedRateOnConfig && Form.DefaultRatePercent == null)
            { globalError = "Default Rate % is required when Rate Source Mode is Fixed."; ShowRate = true; return; }

            if (Form.RequirePANForStandardRate && Form.AlternateRateIfPANMissing == null)
            { globalError = "Alternate Rate is required when PAN is mandatory for standard rate."; ShowRate = true; return; }

            if (Form.ThresholdEvaluationMode != ThresholdEvaluationMode.NoThreshold && Form.ThresholdAmount == null)
            { globalError = "Threshold Amount is required when Threshold Mode is not NoThreshold."; ShowThreshold = true; return; }

            if (Form.IsLockedForChanges && string.IsNullOrWhiteSpace(Form.LockReason))
            { globalError = "Lock Reason is required when locking."; ShowGovernance = true; return; }

            if (string.IsNullOrWhiteSpace(Form.TDSPayableGLAccountDisplay))
            { globalError = "TDS Payable GL Account is required."; ShowAccounting = true; return; }

            try
            {
                isSaving = true;
                await ConfigService.SaveAsync(Form);
                ToastService.ShowSuccess(IsEdit ? "TDS Config updated." : "TDS Config created.");
                Nav.NavigateTo("/tds-config");
            }
            catch (InvalidOperationException ex) { globalError = ex.Message; }
            catch { globalError = "An unexpected error occurred. Please try again."; }
            finally { isSaving = false; }
        }

        private void Cancel() => Nav.NavigateTo("/tds-config");
    }
}
