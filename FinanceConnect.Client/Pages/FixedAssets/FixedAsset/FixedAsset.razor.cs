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
    public partial class FixedAsset : ComponentBase
    {
        // ── Injections ─────────────────────────────────────────────────────────────
        [Inject] private FixedAssetService FixedAssetService { get; set; } = default!;
        [Inject] private AssetCategoryService AssetCategoryService { get; set; } = default!;
        [Inject] private VendorService VendorService { get; set; } = default!;
        [Inject] private BranchService BranchService { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;

        // ══════════════════════════════════════════════════════════════════════════
        //  LIST PAGE STATE
        // ══════════════════════════════════════════════════════════════════════════

        private List<FixedAssetListDto> AllAssets { get; set; } = new();
        private List<FixedAssetListDto> FilteredAssets { get; set; } = new();
        private List<FixedAssetListDto> PagedAssets { get; set; } = new();
        public List<BranchModel> BranchesList { get; set; } = new();
        private List<VendorViewModel> AllVendors { get; set; } = new();
        private List<ViewModels.SelectItem> CategoryList { get; set; } = new();

        private FixedAssetListDto? SelectedAsset { get; set; }

        // ── Filters ───────────────────────────────────────────────────────────────
        private string searchText { get; set; } = string.Empty;
        private string SelectedStatus { get; set; } = string.Empty;
        private string SelectedDepreciable { get; set; } = string.Empty;
        private string SelectedBranchId { get; set; } = string.Empty;

        // ── Pagination ────────────────────────────────────────────────────────────
        private int CurrentPage { get; set; } = 1;
        private int PageSize { get; set; } = 10;
        private int TotalPages => FilteredAssets.Count == 0
            ? 1
            : (int)Math.Ceiling(FilteredAssets.Count / (double)PageSize);
        private int StartPage => Math.Max(1, CurrentPage - 2);
        private int EndPage => Math.Min(TotalPages, StartPage + 4);

        // ══════════════════════════════════════════════════════════════════════════
        //  CREATE / EDIT OFFCANVAS (FORM) STATE
        // ══════════════════════════════════════════════════════════════════════════

        /// <summary>The asset being created or edited in the offcanvas wizard.</summary>
        private FA.FixedAsset? FormAsset { get; set; }

        /// <summary>EditContext for the wizard form.</summary>
        private EditContext? FormEditContext { get; set; }

        /// <summary>True when the offcanvas is in edit mode (existing asset).</summary>
        private bool FormIsEdit { get; set; }

        /// <summary>The ID of the asset being edited (null for create).</summary>
        private Guid? FormEditId { get; set; }

        /// <summary>Currently selected category in the wizard.</summary>
        private AssetsCategoryViewModel.AssetCategory? FormSelectedCategory { get; set; }

        // ── Wizard Navigation ─────────────────────────────────────────────────────
        private int FormStep { get; set; } = 1;

        private List<WizardStep> FormSteps { get; } = new()
        {
            new("Core Identity",          "ti ti-id-badge"),
            new("Identity & Tagging",     "ti ti-barcode"),
            new("Ownership & Assignment", "ti ti-building"),
            new("Capitalization & Cost",  "ti ti-cash"),
            new("Depreciation & Notes",   "ti ti-chart-line"),
        };

        // ── Validation ────────────────────────────────────────────────────────────
        private HashSet<string> _formTouchedFields = new();
        private bool _formSubmitted = false;

        // ══════════════════════════════════════════════════════════════════════════
        //  LIFECYCLE
        // ══════════════════════════════════════════════════════════════════════════

        protected override async Task OnInitializedAsync()
        {
            LoadDropdowns();
            await LoadBranchesAsync();
            await LoadAssetsAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
        }

        // ══════════════════════════════════════════════════════════════════════════
        //  DATA LOADING
        // ══════════════════════════════════════════════════════════════════════════

        private void LoadDropdowns()
        {
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

            AllVendors = VendorService.GetAll();
        }

        private async Task LoadAssetsAsync()
        {
            AllAssets = await FixedAssetService.GetAllAsync();
            ApplyFilters();
        }

        private Task LoadBranchesAsync()
        {
            BranchesList = BranchService.GetAll();
            return Task.CompletedTask;
        }

        // ══════════════════════════════════════════════════════════════════════════
        //  OFFCANVAS OPEN METHODS
        // ══════════════════════════════════════════════════════════════════════════

        /// <summary>Open offcanvas in CREATE mode.</summary>
        private async Task OpenCreateOffcanvas()
        {
            FormIsEdit = false;
            FormEditId = null;
            FormStep = 1;
            _formSubmitted = false;
            _formTouchedFields.Clear();
            FormSelectedCategory = null;

            FormAsset = new FA.FixedAsset
            {
                Status = FA.AssetStatus.Draft,
                PurchaseDate = DateTime.Today,
                AssetCode = await FixedAssetService.GenerateNextAssetCodeAsync()
            };

            FormEditContext = new EditContext(FormAsset);
            StateHasChanged();
        }

        /// <summary>Open offcanvas in EDIT mode, loading existing asset data.</summary>
        private async Task OpenEditOffcanvas(FixedAssetListDto row)
        {
            FormIsEdit = true;
            FormEditId = row.FixedAssetId;
            FormStep = 1;
            _formSubmitted = false;
            _formTouchedFields.Clear();
            FormSelectedCategory = null;

            var existing = await FixedAssetService.GetByIdAsync(row.FixedAssetId);
            if (existing == null) return;

            FormAsset = new FA.FixedAsset
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
                PurchaseDate = existing.PurchaseDate ?? DateTime.Today,
                PurchaseCost = existing.PurchaseCost,
                SalvageValue = existing.SalvageValue,
                IsDepreciable = existing.IsDepreciable,
                UsefulLifeMonths = existing.UsefulLifeMonths,
                Status = existing.AssetStatus,
                Notes = existing.Notes,
            };

            if (existing.AssetCategoryId.HasValue)
                FormSelectedCategory = AssetCategoryService.GetById(existing.AssetCategoryId.Value);

            FormEditContext = new EditContext(FormAsset);
            StateHasChanged();
        }

        // ══════════════════════════════════════════════════════════════════════════
        //  WIZARD NAVIGATION (FORM)
        // ══════════════════════════════════════════════════════════════════════════

        private void FormNext()
        {
            _formTouchedFields.Clear();
            _formSubmitted = false;
            if (!FormValidateStep(FormStep)) return;
            if (FormStep < FormSteps.Count) FormStep++;
        }

        private void FormBack()
        {
            if (FormStep > 1) { FormStep--; _formSubmitted = false; }
        }

        // ══════════════════════════════════════════════════════════════════════════
        //  SAVE (CREATE / UPDATE)
        // ══════════════════════════════════════════════════════════════════════════

        private async Task FormSave()
        {
            if (FormAsset == null) return;

            // Mark as submitted so all section errors become visible
            _formSubmitted = true;

            // Validate ALL sections — collect failures without short-circuiting
            bool hasError = false;
            for (int s = 1; s <= FormSteps.Count; s++)
            {
                if (!FormValidateStep(s, silent: true))
                    hasError = true;
            }

            // Re-render so error dots + red borders + messages all appear
            if (hasError)
            {
                StateHasChanged();
                return;
            }

            try
            {
                if (FormIsEdit)
                    await FixedAssetService.UpdateAsync(FormAsset);
                else
                    await FixedAssetService.CreateAsync(FormAsset);

                await JS.InvokeVoidAsync("eval",
                    "bootstrap.Offcanvas.getInstance(document.getElementById('createEditAssetOffcanvas'))?.hide()");

                await LoadAssetsAsync();

                ToastService.ShowSuccess(FormIsEdit
                    ? "Asset updated successfully"
                    : "Asset created successfully");

                FormAsset = null;
                FormEditContext = null;
            }
            catch (InvalidOperationException ex)
            {
                await JS.InvokeVoidAsync("alert", ex.Message);
            }
        }

        // ══════════════════════════════════════════════════════════════════════════
        //  CATEGORY CHANGE & COST RECALCULATION
        // ══════════════════════════════════════════════════════════════════════════

        private async Task OnFormCategoryChanged(ChangeEventArgs e)
        {
            if (FormAsset == null) return;
            FormSelectedCategory = null;

            if (!Guid.TryParse(e.Value?.ToString(), out var catId) || catId == Guid.Empty)
            {
                FormAsset.AssetCategoryId = Guid.Empty;
                StateHasChanged();
                return;
            }

            FormAsset.AssetCategoryId = catId;
            FormSelectedCategory = await AssetCategoryService.GetByIdAsync(catId);

            if (FormSelectedCategory == null) { StateHasChanged(); return; }

            FormAsset.IsDepreciable = FormSelectedCategory.IsDepreciable;
            FormAsset.UsefulLifeMonths = FormSelectedCategory.UsefulLifeMonths;
            FormAsset.SalvageValue = (FormAsset.PurchaseCost > 0 && FormSelectedCategory.ResidualValuePercent.HasValue)
                ? Math.Round(FormAsset.PurchaseCost * (FormSelectedCategory.ResidualValuePercent.Value / 100m), 2)
                : null;

            StateHasChanged();
        }

        private void OnFormPurchaseCostChanged(ChangeEventArgs e)
        {
            if (FormAsset == null) return;
            if (decimal.TryParse(e.Value?.ToString(), out var cost))
            {
                FormAsset.PurchaseCost = cost;
                if (FormSelectedCategory?.ResidualValuePercent.HasValue == true && cost > 0)
                    FormAsset.SalvageValue = Math.Round(
                        cost * (FormSelectedCategory.ResidualValuePercent.Value / 100m), 2);
            }
        }

        // ══════════════════════════════════════════════════════════════════════════
        //  VALIDATION (FORM)
        // ══════════════════════════════════════════════════════════════════════════

        private bool FormValidateStep(int step, bool silent = false)
        {
            if (FormAsset == null) return false;
            if (!silent) _formSubmitted = true;
            return step switch
            {
                1 => !string.IsNullOrWhiteSpace(FormAsset.AssetName) && FormAsset.AssetCategoryId != Guid.Empty,
                2 => FormValidateTaggingStep(),
                3 => true,
                4 => FormAsset.PurchaseDate != default && FormAsset.PurchaseCost > 0,
                5 => !FormAsset.IsDepreciable || (FormAsset.UsefulLifeMonths.HasValue && FormAsset.UsefulLifeMonths > 0),
                _ => true
            };
        }

        private bool FormValidateTaggingStep()
        {
            if (FormSelectedCategory == null || FormAsset == null) return true;
            if (FormSelectedCategory.RequiresAssetTag && string.IsNullOrWhiteSpace(FormAsset.AssetTag)) return false;
            if (FormSelectedCategory.RequiresSerialNumber && string.IsNullOrWhiteSpace(FormAsset.SerialNumber)) return false;
            return true;
        }

        /// <summary>
        /// Shows error for a field regardless of which section is "active".
        /// Only requires _formSubmitted to be true (or the field was touched).
        /// </summary>
        private bool FormShowError(int step, string field)
        {
            if (!_formSubmitted && !_formTouchedFields.Contains(field)) return false;
            return !FormIsFieldValid(step, field);   // ← removed "step == FormStep" check
        }

        /// <summary>Returns true when a section has at least one failing field after submit.</summary>
        private bool FormSectionHasError(int section)
        {
            if (!_formSubmitted) return false;
            return section switch
            {
                1 => !FormIsFieldValid(1, "AssetName") || !FormIsFieldValid(1, "AssetCategoryId"),
                2 => !FormIsFieldValid(2, "AssetTag") || !FormIsFieldValid(2, "SerialNumber"),
                4 => !FormIsFieldValid(4, "PurchaseDate") || !FormIsFieldValid(4, "PurchaseCost"),
                5 => !FormIsFieldValid(5, "UsefulLifeMonths"),
                _ => false
            };
        }

        /// <summary>Returns true when ANY section has an error (used for the summary bar).</summary>
        private bool FormHasAnyError() =>
            FormSectionHasError(1) || FormSectionHasError(2) ||
            FormSectionHasError(4) || FormSectionHasError(5);

        private bool FormIsFieldValid(int step, string field)
        {
            if (FormAsset == null) return true;
            return (step, field) switch
            {
                (1, "AssetName") => !string.IsNullOrWhiteSpace(FormAsset.AssetName),
                (1, "AssetCategoryId") => FormAsset.AssetCategoryId != Guid.Empty,
                (2, "AssetTag") => FormSelectedCategory?.RequiresAssetTag != true || !string.IsNullOrWhiteSpace(FormAsset.AssetTag),
                (2, "SerialNumber") => FormSelectedCategory?.RequiresSerialNumber != true || !string.IsNullOrWhiteSpace(FormAsset.SerialNumber),
                (4, "PurchaseDate") => FormAsset.PurchaseDate != default,
                (4, "PurchaseCost") => FormAsset.PurchaseCost > 0 &&
                                          (FormSelectedCategory == null || FormAsset.PurchaseCost >= FormSelectedCategory.CapitalizationThresholdAmount),
                (5, "UsefulLifeMonths") => !FormAsset.IsDepreciable ||
                                          (FormAsset.UsefulLifeMonths.HasValue && FormAsset.UsefulLifeMonths > 0),
                _ => true
            };
        }

        // ── Category rule helpers ─────────────────────────────────────────────────
        private bool FormIsAssetTagRequired => FormSelectedCategory?.RequiresAssetTag == true;
        private bool FormIsSerialNumberRequired => FormSelectedCategory?.RequiresSerialNumber == true;
        private bool FormIsCustodianRequired => FormSelectedCategory?.RequiresCustodian == true;
        private bool FormIsLocationRequired => FormSelectedCategory?.RequiresLocation == true;

        // ══════════════════════════════════════════════════════════════════════════
        //  LIST PAGE: REFRESH / SEARCH / FILTER / PAGINATION
        // ══════════════════════════════════════════════════════════════════════════

        private async Task OnRefreshAsync()
        {
            searchText = string.Empty;
            SelectedStatus = string.Empty;
            SelectedDepreciable = string.Empty;
            SelectedBranchId = string.Empty;
            CurrentPage = 1;
            await LoadBranchesAsync();
            await LoadAssetsAsync();
            await JS.InvokeVoidAsync("feather.replace");
        }

        private void OnSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString() ?? string.Empty;
            CurrentPage = 1;
            ApplyFilters();
        }

        private void OnFilterChanged(ChangeEventArgs e)
        {
            CurrentPage = 1;
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var query = AllAssets.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var term = searchText.Trim().ToLowerInvariant();
                query = query.Where(a =>
                    (a.AssetCode != null && a.AssetCode.ToLowerInvariant().Contains(term)) ||
                    (a.AssetName != null && a.AssetName.ToLowerInvariant().Contains(term)) ||
                    (a.AssetTag != null && a.AssetTag.ToLowerInvariant().Contains(term)) ||
                    (a.SerialNumber != null && a.SerialNumber.ToLowerInvariant().Contains(term)) ||
                    (a.CategoryName != null && a.CategoryName.ToLowerInvariant().Contains(term))
                );
            }

            if (!string.IsNullOrEmpty(SelectedStatus) &&
                int.TryParse(SelectedStatus, out var statusInt) &&
                Enum.IsDefined(typeof(AssetStatus), statusInt))
            {
                query = query.Where(a => a.AssetStatus == (AssetStatus)statusInt);
            }

            if (!string.IsNullOrEmpty(SelectedDepreciable) &&
                bool.TryParse(SelectedDepreciable, out var depreciable))
                query = query.Where(a => a.IsDepreciable == depreciable);

            if (!string.IsNullOrEmpty(SelectedBranchId) &&
                Guid.TryParse(SelectedBranchId, out var branchGuid))
                query = query.Where(a => a.BranchId == branchGuid);

            FilteredAssets = query.ToList();
            UpdatePagedList();
        }

        private void UpdatePagedList()
        {
            PagedAssets = FilteredAssets
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }

        private void OnPageSizeChange(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out var size))
            {
                PageSize = size;
                CurrentPage = 1;
                UpdatePagedList();
            }
        }

        private void PreviousPage() { if (CurrentPage > 1) { CurrentPage--; UpdatePagedList(); } }
        private void NextPage() { if (CurrentPage < TotalPages) { CurrentPage++; UpdatePagedList(); } }
        private void GoToPage(int page) { CurrentPage = page; UpdatePagedList(); }

        // ══════════════════════════════════════════════════════════════════════════
        //  ROW DETAILS / DELETE
        // ══════════════════════════════════════════════════════════════════════════

        private void OpenRowDetails(FixedAssetListDto asset) => SelectedAsset = asset;
        private void DeletePopupOpen(FixedAssetListDto asset) => SelectedAsset = asset;

        private async Task ConfirmDelete(Guid assetId)
        {
            try
            {
                await FixedAssetService.DeleteAsync(assetId);
                AllAssets.RemoveAll(a => a.FixedAssetId == assetId);
                ApplyFilters();
                ToastService.ShowSuccess("Asset deleted successfully");
            }
            catch (InvalidOperationException ex) { ToastService.ShowError(ex.Message); }
            catch (Exception) { ToastService.ShowError("Something went wrong"); }
        }

        // ══════════════════════════════════════════════════════════════════════════
        //  BADGE / LABEL HELPERS
        // ══════════════════════════════════════════════════════════════════════════

        private static string GetStatusDotBadge(AssetStatus status) => status switch
        {
            AssetStatus.Draft => "bg-warning",
            AssetStatus.Active => "bg-success",
            AssetStatus.Inactive => "bg-danger",
            AssetStatus.Disposed => "bg-secondary",
            _ => "bg-light"
        };

        private static string GetStatusLabel(AssetStatus status) => status switch
        {
            AssetStatus.Draft => "Draft",
            AssetStatus.Active => "Active",
            AssetStatus.Inactive => "Inactive",
            AssetStatus.Disposed => "Disposed",
            _ => "Unknown"
        };

        private static string GetStatusBadgeClass(AssetStatus status) => status switch
        {
            AssetStatus.Draft => "bg-warning-transparent",
            AssetStatus.Active => "bg-success-transparent",
            AssetStatus.Inactive => "bg-danger-transparent",
            AssetStatus.Disposed => "bg-secondary-transparent",
            _ => "bg-light"
        };

        // ── Internal record ───────────────────────────────────────────────────────
        private record WizardStep(string Title, string Icon);
    }
}
