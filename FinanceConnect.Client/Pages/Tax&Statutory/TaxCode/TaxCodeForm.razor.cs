using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;


namespace FinanceConnect.Client.Pages.Tax_Statutory.TaxCode
{
    public partial class TaxCodeForm
    {
        [Inject] private TaxCodeService TaxCodeService { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        [Parameter] public Guid? Id { get; set; }

        private bool isInitialized = false;
        private bool IsEdit => Id.HasValue && Id != Guid.Empty;
        private string PageTitle => IsEdit ? "Edit Tax Code" : "Create Tax Code";
        private string PageSubTitle => IsEdit ? "Update tax code configuration" : "Define new tax identity for the system";

        private TaxCodeViewModel.TaxCode Model { get; set; } = new();
        private EditContext editContext = default!;

        private int CurrentStep { get; set; } = 1;

        private List<WizardStep> Steps { get; set; } = new()
        {
            new("Core Identity",          "ti ti-file-description"),
            new("Tax Classification",     "ti ti-category"),
            new("Calculation & GL",       "ti ti-calculator"),
            new("Review & Save",          "ti ti-check"),
        };

        private string IndicatorToppx => $"{((CurrentStep - 1) * 72) + 18}px";

        private HashSet<string> _touchedFields = new();
        private bool _submitted = false;

        protected override async Task OnInitializedAsync()
        {
            Model = new TaxCodeViewModel.TaxCode
            {
                Status = TaxCodeViewModel.TaxCodeStatus.Active,
                CalcType = TaxCodeViewModel.CalculationType.Percentage,
                Basis = TaxCodeViewModel.RateBasis.OnTaxableValue,
                Rounding = TaxCodeViewModel.RoundingRule.RoundToNearest,
                RoundingPrecisionDecimals = 2,
                IsITCEligibleDefault = true,
                IsGLOverrideAllowedByMapping = true,
                JurisdictionCountryCode = "IN",
                EffectivePolicy = TaxCodeViewModel.EffectiveFromPolicy.RateVersionControlsOnly,
            };

            editContext = new EditContext(Model);

            if (IsEdit)
                await LoadExistingAsync();

            isInitialized = true;
        }

        private async Task LoadExistingAsync()
        {
            var existing = await TaxCodeService.GetByIdAsync(Id!.Value);
            if (existing == null) { Nav.NavigateTo("/tax-codes"); return; }

            Model = new TaxCodeViewModel.TaxCode
            {
                TaxCodeId = existing.TaxCodeId,
                Code = existing.Code,
                TaxName = existing.TaxName,
                Description = existing.Description,
                Type = existing.Type,
                JurisdictionCountryCode = existing.JurisdictionCountryCode,
                GSTComponent = existing.GSTComponent,
                Direction = existing.Direction,
                IsReverseChargeApplicable = existing.IsReverseChargeApplicable,
                IsITCEligibleDefault = existing.IsITCEligibleDefault,
                CalcType = existing.CalcType,
                Basis = existing.Basis,
                RoundingPrecisionDecimals = existing.RoundingPrecisionDecimals,
                Rounding = existing.Rounding,
                MinTaxAmount = existing.MinTaxAmount,
                MaxTaxAmount = existing.MaxTaxAmount,
                InputTaxGLAccountId = existing.InputTaxGLAccountId,
                InputTaxGLAccountName = existing.InputTaxGLAccountName,
                OutputTaxGLAccountId = existing.OutputTaxGLAccountId,
                OutputTaxGLAccountName = existing.OutputTaxGLAccountName,
                TDSGLAccountId = existing.TDSGLAccountId,
                TDSGLAccountName = existing.TDSGLAccountName,
                TCSGLAccountId = existing.TCSGLAccountId,
                TCSGLAccountName = existing.TCSGLAccountName,
                IsGLOverrideAllowedByMapping = existing.IsGLOverrideAllowedByMapping,
                ReturnTag = existing.ReturnTag,
                TDSSectionCode = existing.TDSSectionCode,
                TCSSectionCode = existing.TCSSectionCode,
                StatutoryReportingGroup = existing.StatutoryReportingGroup,
                Status = existing.Status,
                IsLockedForChanges = existing.IsLockedForChanges,
                LockReason = existing.LockReason,
                EffectivePolicy = existing.EffectivePolicy,
                CompanyId = existing.CompanyId,
            };

            editContext = new EditContext(Model);
        }

        private void Next()
        {
            _submitted = true;
            if (!ValidateStep(CurrentStep)) return;
            _submitted = false;
            _touchedFields.Clear();
            if (CurrentStep < Steps.Count) CurrentStep++;
        }

        private void Back()
        {
            if (CurrentStep > 1) { CurrentStep--; _submitted = false; }
        }

        private string StepClass(int step)
        {
            if (step < CurrentStep) return "done";
            if (step == CurrentStep) return "active";
            return "";
        }

        private bool IsCurrentStepValid() => ValidateStep(CurrentStep, silent: true);

        private async Task Save()
        {
            _submitted = true;

            for (int s = 1; s <= Steps.Count; s++)
            {
                if (!ValidateStep(s, silent: true))
                {
                    CurrentStep = s;
                    _submitted = true;
                    ToastService.ShowError("Please fill all required fields.", "Validation Error");
                    return;
                }
            }

            try
            {
                if (IsEdit)
                    await TaxCodeService.UpdateAsync(Model);
                else
                    await TaxCodeService.CreateAsync(Model);

                Nav.NavigateTo("/tax-codes");
            }
            catch (InvalidOperationException ex)
            {
                ToastService.ShowError($"{ex.Message}", "Error");
            }
        }

        private bool ValidateStep(int step, bool silent = false)
        {
            if (!silent) _submitted = true;
            return step switch
            {
                1 => !string.IsNullOrWhiteSpace(Model.Code) &&
                     !string.IsNullOrWhiteSpace(Model.TaxName),

                2 => Model.Type.HasValue &&
                     Model.Direction.HasValue &&
                     ValidateConditionalClassification(),

                3 => Model.RoundingPrecisionDecimals >= 0 &&
                     Model.RoundingPrecisionDecimals <= 4 &&
                     ValidateGLMapping(),

                4 => true,
                _ => true
            };
        }

        private bool ValidateConditionalClassification()
        {
            if (Model.Type == TaxCodeViewModel.TaxType.GST && Model.GSTComponent == null) return false;
            if (Model.Type == TaxCodeViewModel.TaxType.TDS && string.IsNullOrWhiteSpace(Model.TDSSectionCode)) return false;
            if (Model.Type == TaxCodeViewModel.TaxType.TCS && string.IsNullOrWhiteSpace(Model.TCSSectionCode)) return false;
            return true;
        }

        private bool ValidateGLMapping()
        {
            if (Model.Type == TaxCodeViewModel.TaxType.GST && Model.Direction == TaxCodeViewModel.TaxDirection.Input &&
                string.IsNullOrWhiteSpace(Model.InputTaxGLAccountName)) return false;
            if (Model.Type == TaxCodeViewModel.TaxType.GST && Model.Direction == TaxCodeViewModel.TaxDirection.Output &&
                string.IsNullOrWhiteSpace(Model.OutputTaxGLAccountName)) return false;
            if (Model.Type == TaxCodeViewModel.TaxType.TDS && string.IsNullOrWhiteSpace(Model.TDSGLAccountName)) return false;
            if (Model.Type == TaxCodeViewModel.TaxType.TCS && string.IsNullOrWhiteSpace(Model.TCSGLAccountName)) return false;
            return true;
        }

        private bool ShowFieldError(int step, string field)
        {
            if (!_submitted && !_touchedFields.Contains(field)) return false;
            return step == CurrentStep && !IsFieldValid(step, field);
        }

        private bool IsFieldValid(int step, string field) => (step, field) switch
        {
            (1, "Code") => !string.IsNullOrWhiteSpace(Model.Code),
            (1, "TaxName") => !string.IsNullOrWhiteSpace(Model.TaxName),
            (2, "Type") => Model.Type.HasValue,
            (2, "Direction") => Model.Direction.HasValue,
            (2, "GSTComponent") => !(Model.Type == TaxCodeViewModel.TaxType.GST && Model.GSTComponent == null),
            (2, "TDSSectionCode") => !(Model.Type == TaxCodeViewModel.TaxType.TDS && string.IsNullOrWhiteSpace(Model.TDSSectionCode)),
            (2, "TCSSectionCode") => !(Model.Type == TaxCodeViewModel.TaxType.TCS && string.IsNullOrWhiteSpace(Model.TCSSectionCode)),
            (3, "RoundingPrecisionDecimals") => Model.RoundingPrecisionDecimals >= 0 && Model.RoundingPrecisionDecimals <= 4,
            (3, "InputTaxGLAccountName") => !(Model.Type == TaxCodeViewModel.TaxType.GST && Model.Direction == TaxCodeViewModel.TaxDirection.Input && string.IsNullOrWhiteSpace(Model.InputTaxGLAccountName)),
            (3, "OutputTaxGLAccountName") => !(Model.Type == TaxCodeViewModel.TaxType.GST && Model.Direction == TaxCodeViewModel.TaxDirection.Output && string.IsNullOrWhiteSpace(Model.OutputTaxGLAccountName)),
            (3, "TDSGLAccountName") => !(Model.Type == TaxCodeViewModel.TaxType.TDS && string.IsNullOrWhiteSpace(Model.TDSGLAccountName)),
            (3, "TCSGLAccountName") => !(Model.Type == TaxCodeViewModel.TaxType.TCS && string.IsNullOrWhiteSpace(Model.TCSGLAccountName)),
            _ => true
        };

        private record WizardStep(string Title, string Icon);
    }
}
