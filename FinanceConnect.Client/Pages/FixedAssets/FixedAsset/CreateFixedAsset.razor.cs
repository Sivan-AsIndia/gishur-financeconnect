using FinanceConnect.Client.Pages.Master.Branch;
using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.FixedAssetViewModel;
using FA = FinanceConnect.Client.ViewModels.FixedAssetViewModel;

namespace FinanceConnect.Client.Pages.FixedAssets.FixedAsset
{
    public partial class CreateFixedAsset
    {
        // ── Injections ────────────────────────────────────────────────────────────
        [Inject] private FixedAssetService FixedAssetService { get; set; } = default!;
        [Inject] private AssetCategoryService AssetCategoryService { get; set; } = default!;
        [Inject] private VendorService VendorService { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        // ── Parameters ────────────────────────────────────────────────────────────
        [Parameter] public Guid? Id { get; set; }

        // ── Page State ────────────────────────────────────────────────────────────
        private bool isInitialized = false;
        private bool IsEdit => Id.HasValue && Id != Guid.Empty;
        private string PageTitle => IsEdit ? "Edit Fixed Asset" : "Create Fixed Asset";
        private string PageSubTitle => IsEdit ? "Update asset details" : "Add a new asset to the register";

        // ── Form Model ────────────────────────────────────────────────────────────
        private FA.FixedAsset Asset { get; set; } = new FA.FixedAsset();
        private EditContext editContext = default!;

        // ── Dropdown Data ─────────────────────────────────────────────────────────
        private List<ViewModels.SelectItem> CategoryList { get; set; } = new();
        public List<BranchModel> BranchesList { get; set; } = new();
        private List<VendorViewModel> AllVendors { get; set; } = new();

        // ── Selected Category — conditional labels & auto-fill ────────────────────
        private AssetsCategoryViewModel.AssetCategory? SelectedCategory { get; set; }

        // ── Wizard ────────────────────────────────────────────────────────────────
        private int CurrentStep { get; set; } = 1;

        private List<WizardStep> Steps { get; set; } = new()
        {
            new("Core Identity",           "ti ti-id-badge"),
            new("Identity & Tagging",      "ti ti-barcode"),
            new("Ownership & Assignment",  "ti ti-building"),
            new("Capitalization & Cost",   "ti ti-cash"),
            new("Depreciation & Notes",    "ti ti-chart-line"),
        };

        private string IndicatorToppx => $"{((CurrentStep - 1) * 72) + 18}px";
        private HashSet<string> _touchedFields = new();
        private bool _submitted = false;

        // ── Lifecycle ─────────────────────────────────────────────────────────────
        protected override async Task OnInitializedAsync()
        {
            Asset = new FA.FixedAsset
            {
                Status = FA.AssetStatus.Draft,
                // ✅ FIX: DateTime.Today set — prevent "01-01-0001" default
                PurchaseDate = DateTime.Today,
            };

            editContext = new EditContext(Asset);
            LoadDropdowns();

            if (IsEdit)
            {
                await LoadExistingAssetAsync();
            }
            else
            {
                // ✅ FIX: AssetCode auto-generate — create mode-ல் preview show பண்ண
                Asset.AssetCode = await FixedAssetService.GenerateNextAssetCodeAsync();
            }

            isInitialized = true;
        }

        // ── LoadDropdowns ─────────────────────────────────────────────────────────
        private void LoadDropdowns()
        {
            // Category: Active + not locked
            CategoryList = AssetCategoryService
                .GetAll()
                .Where(c => !c.IsDeleted &&
                            c.CategoryStatus == AssetsCategoryViewModel.CategoryStatus.Active &&
                            !c.IsLockedForChanges)
                .Select(c => new ViewModels.SelectItem
                {
                    Value = c.AssetCategoryId.ToString(),
                    Text = $"{c.CategoryCode} – {c.CategoryName}"
                })
                .ToList();

            BranchesList = BranchService.GetAll();
            AllVendors = VendorService.GetAll();
        }

        // ── LoadExistingAsset (Edit mode) ─────────────────────────────────────────
        private async Task LoadExistingAssetAsync()
        {
            var existing = await FixedAssetService.GetByIdAsync(Id!.Value);
            if (existing == null)
            {
                Nav.NavigateTo("/fixed-assets");
                return;
            }

            Asset = new FA.FixedAsset
            {
                FixedAssetId = existing.FixedAssetId,
                AssetCode = existing.AssetCode ?? "",
                AssetName = existing.AssetName ?? "",
                AssetCategoryId = existing.AssetCategoryId ?? Guid.Empty,
                AssetTag = existing.AssetTag,
                SerialNumber = existing.SerialNumber,
                BranchId = existing.BranchId,
                VendorId = existing.VendorId,
                Location = existing.Location,
                Custodian = existing.Custodian,
                // ✅ FIX: null fallback to Today
                PurchaseDate = existing.PurchaseDate ?? DateTime.Today,
                PurchaseCost = existing.PurchaseCost,
                SalvageValue = existing.SalvageValue,
                IsDepreciable = existing.IsDepreciable,
                UsefulLifeMonths = existing.UsefulLifeMonths,
                Status = existing.AssetStatus,
                Notes = existing.Notes,
            };

            if (existing.AssetCategoryId.HasValue)
                SelectedCategory = AssetCategoryService.GetById(existing.AssetCategoryId.Value);

            editContext = new EditContext(Asset);
        }

        // ── OnCategoryChanged — auto-fill defaults ────────────────────────────────
        private async Task OnCategoryChanged(ChangeEventArgs e)
        {
            SelectedCategory = null;

            if (!Guid.TryParse(e.Value?.ToString(), out var catId) || catId == Guid.Empty)
            {
                Asset.AssetCategoryId = Guid.Empty;
                StateHasChanged();
                return;
            }

            Asset.AssetCategoryId = catId;
            SelectedCategory = await AssetCategoryService.GetByIdAsync(catId);

            if (SelectedCategory == null) { StateHasChanged(); return; }

            Asset.IsDepreciable = SelectedCategory.IsDepreciable;
            Asset.UsefulLifeMonths = SelectedCategory.UsefulLifeMonths;
            Asset.SalvageValue = (Asset.PurchaseCost > 0 && SelectedCategory.ResidualValuePercent.HasValue)
                ? Math.Round(Asset.PurchaseCost * (SelectedCategory.ResidualValuePercent.Value / 100m), 2)
                : null;

            StateHasChanged();
        }

        // ── OnPurchaseCostChanged — recalculate SalvageValue ─────────────────────
        private void OnPurchaseCostChanged(ChangeEventArgs e)
        {
            if (decimal.TryParse(e.Value?.ToString(), out var cost))
            {
                Asset.PurchaseCost = cost;
                if (SelectedCategory?.ResidualValuePercent.HasValue == true && cost > 0)
                    Asset.SalvageValue = Math.Round(
                        cost * (SelectedCategory.ResidualValuePercent.Value / 100m), 2);
            }
        }

        // ── Live Summary helpers ──────────────────────────────────────────────────
        private string GetSelectedBranchName()
        {
            if (!Asset.BranchId.HasValue) return "—";
            return BranchesList.FirstOrDefault(b => b.Id == Asset.BranchId.Value)?.BranchName ?? "—";
        }

        private string GetSelectedVendorName()
        {
            if (!Asset.VendorId.HasValue) return "—";
            return AllVendors.FirstOrDefault(v => v.Id == Asset.VendorId.Value)?.VendorName ?? "—";
        }

        // ── Wizard Navigation ─────────────────────────────────────────────────────
        private void Next()
        {
            _touchedFields.Clear();
            _submitted = false;
            if (!ValidateStep(CurrentStep)) return;
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

        // ── Save ──────────────────────────────────────────────────────────────────
        private async Task Save()
        {
            _submitted = true;
            for (int s = 1; s <= Steps.Count; s++)
            {
                if (!ValidateStep(s)) { CurrentStep = s; return; }
            }
            try
            {
                if (IsEdit) await FixedAssetService.UpdateAsync(Asset);
                else await FixedAssetService.CreateAsync(Asset);
                Nav.NavigateTo("/fixed-assets");
            }
            catch (InvalidOperationException ex)
            {
                await JS.InvokeVoidAsync("alert", ex.Message);
            }
        }

        // ── Validation ────────────────────────────────────────────────────────────
        private bool ValidateStep(int step, bool silent = false)
        {
            if (!silent) _submitted = true;
            return step switch
            {
                1 => !string.IsNullOrWhiteSpace(Asset.AssetName) && Asset.AssetCategoryId != Guid.Empty,
                2 => ValidateTaggingStep(),
                3 => true,
                4 => Asset.PurchaseDate != default && Asset.PurchaseCost > 0,
                5 => !Asset.IsDepreciable || (Asset.UsefulLifeMonths.HasValue && Asset.UsefulLifeMonths > 0),
                _ => true
            };
        }

        private bool ValidateTaggingStep()
        {
            if (SelectedCategory == null) return true;
            if (SelectedCategory.RequiresAssetTag && string.IsNullOrWhiteSpace(Asset.AssetTag)) return false;
            if (SelectedCategory.RequiresSerialNumber && string.IsNullOrWhiteSpace(Asset.SerialNumber)) return false;
            return true;
        }

        private bool ShowFieldError(int step, string field)
        {
            if (!_submitted && !_touchedFields.Contains(field)) return false;
            return step == CurrentStep && !IsFieldValid(step, field);
        }

        private bool IsFieldValid(int step, string field) => (step, field) switch
        {
            (1, "AssetName") => !string.IsNullOrWhiteSpace(Asset.AssetName),
            (1, "AssetCategoryId") => Asset.AssetCategoryId != Guid.Empty,
            (2, "AssetTag") => SelectedCategory?.RequiresAssetTag != true || !string.IsNullOrWhiteSpace(Asset.AssetTag),
            (2, "SerialNumber") => SelectedCategory?.RequiresSerialNumber != true || !string.IsNullOrWhiteSpace(Asset.SerialNumber),
            (4, "PurchaseDate") => Asset.PurchaseDate != default,
            (4, "PurchaseCost") => Asset.PurchaseCost > 0 &&
        (SelectedCategory == null ||
         Asset.PurchaseCost >= SelectedCategory.CapitalizationThresholdAmount),
            (5, "UsefulLifeMonths") => !Asset.IsDepreciable || (Asset.UsefulLifeMonths.HasValue && Asset.UsefulLifeMonths > 0),
            _ => true
        };

        // ── Category rule helpers ─────────────────────────────────────────────────
        private bool IsAssetTagRequired => SelectedCategory?.RequiresAssetTag == true;
        private bool IsSerialNumberRequired => SelectedCategory?.RequiresSerialNumber == true;
        private bool IsCustodianRequired => SelectedCategory?.RequiresCustodian == true;
        private bool IsLocationRequired => SelectedCategory?.RequiresLocation == true;

        private record WizardStep(string Title, string Icon);
    }
}
