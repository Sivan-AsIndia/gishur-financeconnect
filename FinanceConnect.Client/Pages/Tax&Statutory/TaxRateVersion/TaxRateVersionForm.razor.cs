using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using RV = FinanceConnect.Client.ViewModels.TaxRateVersionViewModel;

namespace FinanceConnect.Client.Pages.Tax_Statutory.TaxRateVersion
{
    public partial class TaxRateVersionForm
    {
        [Inject] private TaxRateVersionService RateVersionService { get; set; } = default!;
        [Inject] private TaxCodeService TaxCodeService { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        [Parameter] public Guid? Id { get; set; }
        [Parameter] public Guid? TaxCodeId { get; set; }

        private bool isInitialized = false;
        private bool IsEdit => Id.HasValue && Id != Guid.Empty;
        private string PageTitle => IsEdit ? "Edit Rate Version" : "Add Rate Version";
        private string PageSubTitle => IsEdit ? "Update draft rate version" : "Create new effective-dated rate for a Tax Code";

        private RV.TaxRateVersion Model { get; set; } = new();
        private EditContext editContext = default!;

        private List<FinanceConnect.Client.ViewModels.TaxCodeViewModel.SelectItem> TaxCodeList { get; set; } = new();

        private int CurrentStep { get; set; } = 1;
        private List<WizardStep> Steps { get; set; } = new()
        {
            new("Tax Code & Dates",     "ti ti-calendar-event"),
            new("Rate Definition",      "ti ti-percentage"),
            new("Rules & Evidence",     "ti ti-file-certificate"),
            new("Review & Save",        "ti ti-check"),
        };

        private string IndicatorToppx => $"{((CurrentStep - 1) * 72) + 18}px";
        private HashSet<string> _touchedFields = new();
        private bool _submitted = false;

        protected override async Task OnInitializedAsync()
        {
            Model = new RV.TaxRateVersion
            {
                Status = RV.VersionStatus.Draft,
                EffectiveFrom = DateTime.Today,
                Type = RV.RateType.Percentage,
                Basis = RV.RateBasis.OnTaxableValue,
                SourceType = RV.RateSourceType.GovernmentNotification,
                ITCOverride = RV.ITCRateOverride.InheritFromTaxCode,
            };

            editContext = new EditContext(Model);
            await LoadTaxCodesAsync();

            if (TaxCodeId.HasValue && TaxCodeId != Guid.Empty)
            {
                Model.TaxCodeId = TaxCodeId.Value;
                await OnTaxCodeSelected(new ChangeEventArgs { Value = TaxCodeId.Value.ToString() });
            }

            if (IsEdit)
                await LoadExistingAsync();

            isInitialized = true;
        }

        private async Task LoadTaxCodesAsync()
        {
            var codes = await TaxCodeService.GetAllAsync();
            TaxCodeList = codes
                .Where(c => c.Status == FinanceConnect.Client.ViewModels.TaxCodeViewModel.TaxCodeStatus.Active)
                .Select(c => new FinanceConnect.Client.ViewModels.TaxCodeViewModel.SelectItem
                {
                    Value = c.TaxCodeId.ToString(),
                    Text = $"{c.Code} – {c.TaxName}"
                })
                .ToList();
        }

        private async Task OnTaxCodeSelected(ChangeEventArgs e)
        {
            if (!Guid.TryParse(e.Value?.ToString(), out var tcId)) return;

            var codes = await TaxCodeService.GetAllAsync();
            var tc = codes.FirstOrDefault(c => c.TaxCodeId == tcId);
            if (tc == null) return;

            Model.TaxCodeId = tcId;
            Model.TaxCodeSnapshot = tc.Code;
            Model.TaxCodeNameSnapshot = tc.TaxName;
            Model.TaxTypeSnapshot = tc.Type?.ToString();
            Model.Basis = (RV.RateBasis)(int)tc.Basis;
            Model.VersionNumber = RateVersionService.GetNextVersionNumber(tcId);

            StateHasChanged();
        }

        private async Task LoadExistingAsync()
        {
            var existing = await RateVersionService.GetByIdAsync(Id!.Value);
            if (existing == null) { Nav.NavigateTo("/tax-rate-versions"); return; }

            Model = new RV.TaxRateVersion
            {
                TaxRateVersionId = existing.TaxRateVersionId,
                TaxCodeId = existing.TaxCodeId,
                TaxCodeSnapshot = existing.TaxCodeSnapshot,
                TaxCodeNameSnapshot = existing.TaxCodeNameSnapshot,
                TaxTypeSnapshot = existing.TaxTypeSnapshot,
                VersionNumber = existing.VersionNumber,
                Status = existing.Status,
                EffectiveFrom = existing.EffectiveFrom,
                EffectiveTo = existing.EffectiveTo,
                Type = existing.Type,
                RatePercent = existing.RatePercent,
                FixedAmount = existing.FixedAmount,
                Basis = existing.Basis,
                MinimumTaxAmount = existing.MinimumTaxAmount,
                MaximumTaxAmount = existing.MaximumTaxAmount,
                HasThreshold = existing.HasThreshold,
                ThresholdAmount = existing.ThresholdAmount,
                PanRequiredForStandardRate = existing.PanRequiredForStandardRate,
                AlternateRatePercentIfPanMissing = existing.AlternateRatePercentIfPanMissing,
                IsReverseChargeRate = existing.IsReverseChargeRate,
                ITCOverride = existing.ITCOverride,
                SourceType = existing.SourceType,
                LegalReferenceNumber = existing.LegalReferenceNumber,
                LegalReferenceDate = existing.LegalReferenceDate,
                Notes = existing.Notes,
                IsLockedForChanges = existing.IsLockedForChanges,
                LockReason = existing.LockReason,
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

        private void Back() { if (CurrentStep > 1) { CurrentStep--; _submitted = false; } }

        private string StepClass(int s) => s < CurrentStep ? "done" : s == CurrentStep ? "active" : "";
        private bool IsCurrentStepValid() => ValidateStep(CurrentStep, silent: true);

        private async Task Save()
        {
            _submitted = true;
            for (int s = 1; s <= Steps.Count; s++)
            {
                if (!ValidateStep(s, silent: true))
                {
                    CurrentStep = s;
                    ToastService.ShowError("Please fill all required fields.", "Validation Error");
                    return;
                }
            }
            try
            {
                if (IsEdit) await RateVersionService.UpdateAsync(Model);
                else await RateVersionService.CreateAsync(Model);
                Nav.NavigateTo("/tax-rate-versions");
            }
            catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message, "Error"); }
        }

        private bool ValidateStep(int step, bool silent = false)
        {
            if (!silent) _submitted = true;
            return step switch
            {
                1 => Model.TaxCodeId != Guid.Empty &&
                     Model.EffectiveFrom != default &&
                     (Model.EffectiveTo == null || Model.EffectiveTo >= Model.EffectiveFrom),

                2 => Model.Type == RV.RateType.Percentage
                        ? (Model.RatePercent.HasValue && Model.RatePercent >= 0 && Model.RatePercent <= 100)
                        : Model.Type == RV.RateType.FixedAmount
                            ? (Model.FixedAmount.HasValue && Model.FixedAmount >= 0)
                            : true,

                3 => true,
                4 => true,
                _ => true
            };
        }

        private bool ShowFieldError(int step, string field)
        {
            if (!_submitted && !_touchedFields.Contains(field)) return false;
            return step == CurrentStep && !IsFieldValid(step, field);
        }

        private bool IsFieldValid(int step, string field) => (step, field) switch
        {
            (1, "TaxCodeId") => Model.TaxCodeId != Guid.Empty,
            (1, "EffectiveFrom") => Model.EffectiveFrom != default,
            (2, "RatePercent") => !(Model.Type == RV.RateType.Percentage && (!Model.RatePercent.HasValue || Model.RatePercent < 0 || Model.RatePercent > 100)),
            (2, "FixedAmount") => !(Model.Type == RV.RateType.FixedAmount && (!Model.FixedAmount.HasValue || Model.FixedAmount < 0)),
            (3, "SourceType") => true, // has default value
            _ => true
        };

        private record WizardStep(string Title, string Icon);
    }
}
