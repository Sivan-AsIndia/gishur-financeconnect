using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.TaxCategoryMappingViewModel;

namespace FinanceConnect.Client.Pages.Tax_Statutory.TaxCategoryMapping
{
    public partial class TaxCategoryMappingCreate
    {
        [Parameter] public Guid? Id { get; set; }

        [Inject] private TaxCategoryMappingService MappingService { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;

        private TaxCategoryMappingModel Model { get; set; } = new();
        private EditContext? _editContext;

        private bool isInitialized = false;
        private bool isSaving = false;
        private bool showValidation = false;
        private string? globalError;
        private string? lineError;

        private bool IsEdit => Id.HasValue && Id != Guid.Empty;

        private bool ShowIdentity { get; set; } = true;
        private bool ShowConditions { get; set; } = false;
        private bool ShowLines { get; set; } = false;
        private bool ShowReporting { get; set; } = false;
        private bool ShowGovernance { get; set; } = false;

        private void ToggleAccordion(string section)
        {
            ShowIdentity = section == "identity" ? !ShowIdentity : false;
            ShowConditions = section == "conditions" ? !ShowConditions : false;
            ShowLines = section == "lines" ? !ShowLines : false;
            ShowReporting = section == "reporting" ? !ShowReporting : false;
            ShowGovernance = section == "governance" ? !ShowGovernance : false;
        }

        // ── Lifecycle ─────────────────────────────────────────────────────────
        protected override async Task OnParametersSetAsync()
        {
            isInitialized = false;
            globalError = null;
            lineError = null;
            showValidation = false;

            if (IsEdit)
            {
                var model = await MappingService.GetModelByIdAsync(Id!.Value);
                if (model == null)
                {
                    Nav.NavigateTo("/tax-category-mapping");
                    return;
                }
                Model = model;
            }
            else
            {
                Model = new TaxCategoryMappingModel
                {
                    EffectiveFrom = DateTime.Today,
                    MappingStatus = "Draft",
                    Priority = 10,
                    IsExemptOrNilOrNonGST = "None",
                    PlaceOfSupplyRuleMode = "UseShipToState",
                    RequiresInvoiceLevelReporting = true,
                };
            }

            _editContext = new EditContext(Model);
            isInitialized = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
        }

        // ── Nullable bool helpers ─────────────────────────────────────────────
        private void OnIsServiceChange(ChangeEventArgs e)
        {
            var v = e.Value?.ToString();
            Model.IsService = string.IsNullOrEmpty(v) ? null : v == "true";
        }

        private void OnRCMChange(ChangeEventArgs e)
        {
            var v = e.Value?.ToString();
            Model.IsReverseChargeApplicable = string.IsNullOrEmpty(v) ? null : v == "true";
        }

        // ── Tax Line management ───────────────────────────────────────────────
        private void AddLine()
        {
            lineError = null;
            int next = Model.Lines.Any() ? Model.Lines.Max(l => l.LineNumber) + 10 : 10;
            Model.Lines.Add(new TaxCategoryMappingLineModel
            {
                Id = Guid.NewGuid(),
                LineNumber = next,
                IsLineActive = true,
                ApplyMode = "AddOn",
                RateResolutionMode = "FromTaxRateVersionByDate",
                RateEffectiveDateBasis = "PostingDate",
                ITCEligibilityOverride = "Inherit",
                RCMBehavior = "Normal",
            });
        }

        private void RemoveLine(TaxCategoryMappingLineModel line)
            => Model.Lines.Remove(line);

        // ── Form submit — matches Branch HandleSubmit pattern ─────────────────
        private async Task HandleSubmit()
        {
            showValidation = true;
            globalError = null;
            lineError = null;

            // Validate with EditContext
            if (_editContext != null && !_editContext.Validate())
                return;

            // Cross-field validations
            if (Model.EffectiveTo.HasValue && Model.EffectiveTo < Model.EffectiveFrom)
            {
                globalError = "Effective To must be on or after Effective From.";
                ShowIdentity = true;
                return;
            }

            if (Model.IsLockedForChanges && string.IsNullOrWhiteSpace(Model.LockReason))
            {
                globalError = "Lock Reason is required when locking the mapping.";
                ShowGovernance = true;
                return;
            }

            // Line validations
            if (Model.IsExemptOrNilOrNonGST == "None" && !Model.Lines.Any(l => l.IsLineActive))
            {
                lineError = "At least one active tax line is required for non-exempt mappings.";
                ShowLines = true;
                return;
            }

            foreach (var line in Model.Lines)
            {
                if (string.IsNullOrWhiteSpace(line.TaxCodeCode))
                {
                    lineError = $"Line {line.LineNumber}: Tax Code is required.";
                    ShowLines = true;
                    return;
                }
                if (line.RateResolutionMode == "FixedOverrideRate" && line.OverrideRatePercent == null)
                {
                    lineError = $"Line {line.LineNumber}: Override rate is required for Fixed Rate mode.";
                    ShowLines = true;
                    return;
                }
            }

            try
            {
                isSaving = true;
                await MappingService.SaveAsync(Model);
                ToastService.ShowSuccess(IsEdit ? "Mapping updated successfully." : "Mapping created successfully.");
                Nav.NavigateTo("/tax-category-mapping");
            }
            catch (InvalidOperationException ex)
            {
                globalError = ex.Message;
            }
            catch
            {
                globalError = "An unexpected error occurred. Please try again.";
            }
            finally
            {
                isSaving = false;
            }
        }

        private void Cancel() => Nav.NavigateTo("/tax-category-mapping");
    }
}
