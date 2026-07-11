using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using AT = FinanceConnect.Client.ViewModels.AssetTransformViewModel;

namespace FinanceConnect.Client.Pages.FixedAssets.AssetTransfer
{
    public partial class TransferForm : ComponentBase
    {
        [Inject] private AssetTransferService TransferService { get; set; } = default!;
        [Inject] private FixedAssetService FixedAssetService { get; set; } = default!;
        [Inject] private AssetTransferService AssetTransferService { get; set; } = default!;
        [Inject] private BranchService BranchService { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        [Parameter] public Guid? Id { get; set; }

        private bool isInitialized = false;
        private bool IsEdit => Id.HasValue && Id != Guid.Empty;
        private string PageTitle => IsEdit ? "Edit Transfer" : "Create Asset Transfer";
        private string PageSubTitle => IsEdit ? "Update transfer" : "Record asset movement";

        private AT.AssetTransfer Model { get; set; } = new();
        private EditContext editContext = default!;
        private List<AT.SelectItem> AssetList { get; set; } = new();
        private List<BranchModel> BranchList { get; set; } = new();

        private List<(Guid Id, string Name)> CustodianList { get; set; } = new();
        private List<(Guid Id, string Name)> LocationList { get; set; } = new();

        private int CurrentStep { get; set; } = 1;

        private List<WizardStep> Steps { get; set; } = new()
        {
            new("Header & Asset",             "ti ti-file-description"),
            new("From vs To",                 "ti ti-arrows-exchange"),
            new("Handover & Acknowledgement", "ti ti-hand-stop"),
            new("Review & Submit",            "ti ti-check"),
        };

        private string IndicatorToppx => $"{((CurrentStep - 1) * 72) + 18}px";

        private HashSet<string> _touchedFields = new();
        private bool _submitted = false;

        private static Guid NameToGuid(string name)
        {
            var key = name.ToLowerInvariant().Trim();
            var bytes = new byte[16];
            int hash1 = 5381, hash2 = 5381;
            foreach (char c in key)
            {
                hash1 = ((hash1 << 5) + hash1) ^ c;
                hash2 = ((hash2 << 3) + hash2) ^ (c * 31);
            }
            BitConverter.GetBytes(hash1).CopyTo(bytes, 0);
            BitConverter.GetBytes(hash2).CopyTo(bytes, 4);
            BitConverter.GetBytes(key.Length).CopyTo(bytes, 8);
            BitConverter.GetBytes(hash1 ^ hash2).CopyTo(bytes, 12);
            return new Guid(bytes);
        }

        protected override async Task OnInitializedAsync()
        {
            Model = new AT.AssetTransfer
            {
                EffectiveTransferDate = DateTime.Today,
                TransferStatus = AT.TransferStatus.Draft,
                HandoverRequired = true,
            };

            editContext = new EditContext(Model);
            await LoadDropdownsAsync();

            if (IsEdit)
                await LoadExistingAsync();
            else
                Model.TransferNumber = await AssetTransferService.GenerateTransferNumber();

            isInitialized = true;
        }

        private async Task LoadDropdownsAsync()
        {
            var assets = await FixedAssetService.GetAllAsync();

            AssetList = assets
                .Where(a => a.AssetStatus != FinanceConnect.Client.ViewModels.FixedAssetViewModel.AssetStatus.Disposed)
                .Select(a => new AT.SelectItem
                {
                    Value = a.FixedAssetId.ToString(),
                    Text = $"{a.AssetCode} – {a.AssetName}"
                })
                .ToList();

            CustodianList = assets
                .Where(a => !string.IsNullOrWhiteSpace(a.Custodian))
                .Select(a => (
                    Id: (a.CustodianUserId.HasValue && a.CustodianUserId != Guid.Empty)
                              ? a.CustodianUserId.Value
                              : NameToGuid(a.Custodian!),
                    Name: a.Custodian!.Trim()
                ))
                .DistinctBy(x => x.Id)
                .OrderBy(x => x.Name)
                .ToList();

   
            LocationList = assets
                .Where(a => !string.IsNullOrWhiteSpace(a.Location))
                .Select(a => (
                    Id: (a.LocationId.HasValue && a.LocationId != Guid.Empty)
                              ? a.LocationId.Value
                              : NameToGuid(a.Location!),
                    Name: a.Location!.Trim()
                ))
                .DistinctBy(x => x.Id)
                .OrderBy(x => x.Name)
                .ToList();

            BranchList = BranchService.GetAll()
                .Where(b => b.Status == "Active")
                .ToList();
        }

        private async Task LoadExistingAsync()
        {
            var existing = await TransferService.GetByIdAsync(Id!.Value);
            if (existing == null) { Nav.NavigateTo("/asset-transfers"); return; }

            var resolvedToLocationId = ResolveLocationId(existing.ToLocationId, existing.ToLocationName);
            var resolvedToCustodianId = ResolveCustodianId(existing.ToCustodianUserId, existing.ToCustodianName);
            var resolvedFromLocationId = ResolveLocationId(existing.FromLocationId, existing.FromLocationName);
            var resolvedFromCustodianId = ResolveCustodianId(existing.FromCustodianUserId, existing.FromCustodianName);

            Model = new AT.AssetTransfer
            {
                AssetTransferId = existing.AssetTransferId,
                TransferNumber = existing.TransferNumber,
                TransferStatus = existing.TransferStatus,
                TransferType = existing.TransferType,
                EffectiveTransferDate = existing.EffectiveTransferDate,
                FixedAssetId = existing.FixedAssetId,
                AssetNumberSnapshot = existing.AssetNumberSnapshot,
                AssetNameSnapshot = existing.AssetNameSnapshot,

                // From
                FromBranchId = existing.FromBranchId,
                FromBranchName = existing.FromBranchName,
                FromLocationId = resolvedFromLocationId,
                FromLocationName = existing.FromLocationName,
                FromCustodianUserId = resolvedFromCustodianId,
                FromCustodianName = existing.FromCustodianName,

                // To
                ToBranchId = existing.ToBranchId,
                ToBranchName = existing.ToBranchName,
                ToLocationId = resolvedToLocationId,
                ToLocationName = existing.ToLocationName,
                ToCustodianUserId = resolvedToCustodianId,
                ToCustodianName = existing.ToCustodianName,

                TransferReason = existing.TransferReason,
            };

            editContext = new EditContext(Model);
        }

        private Guid? ResolveLocationId(Guid? dbId, string? name)
        {
            if (dbId.HasValue && dbId != Guid.Empty && LocationList.Any(x => x.Id == dbId.Value))
                return dbId;
            if (!string.IsNullOrWhiteSpace(name))
            {
                var match = LocationList.FirstOrDefault(x =>
                    x.Name.Equals(name.Trim(), StringComparison.OrdinalIgnoreCase));
                if (match.Id != Guid.Empty) return match.Id;
                return NameToGuid(name);
            }
            return dbId;
        }

        private Guid? ResolveCustodianId(Guid? dbId, string? name)
        {
            if (dbId.HasValue && dbId != Guid.Empty && CustodianList.Any(x => x.Id == dbId.Value))
                return dbId;
            if (!string.IsNullOrWhiteSpace(name))
            {
                var match = CustodianList.FirstOrDefault(x =>
                    x.Name.Equals(name.Trim(), StringComparison.OrdinalIgnoreCase));
                if (match.Id != Guid.Empty) return match.Id;
                return NameToGuid(name);
            }
            return dbId;
        }

        private async Task OnAssetSelected(ChangeEventArgs e)
        {
            if (!Guid.TryParse(e.Value?.ToString(), out var assetId)) return;

            var assets = await FixedAssetService.GetAllAsync();
            var asset = assets.FirstOrDefault(a => a.FixedAssetId == assetId);
            if (asset == null) return;

            Model.FixedAssetId = assetId;
            Model.AssetNumberSnapshot = asset.AssetCode;
            Model.AssetNameSnapshot = asset.AssetName;
            Model.AssetStatusSnapshot = asset.AssetStatus.ToString();
            Model.AssetCategoryIdSnapshot = asset.AssetCategoryId;

            Model.FromBranchId = asset.BranchId;
            Model.FromBranchName = asset.BranchName;

            Model.FromLocationId = (asset.LocationId.HasValue && asset.LocationId != Guid.Empty)
                ? asset.LocationId.Value
                : (!string.IsNullOrWhiteSpace(asset.Location) ? NameToGuid(asset.Location) : null);
            Model.FromLocationName = asset.Location;

            Model.FromCustodianUserId = (asset.CustodianUserId.HasValue && asset.CustodianUserId != Guid.Empty)
                ? asset.CustodianUserId.Value
                : (!string.IsNullOrWhiteSpace(asset.Custodian) ? NameToGuid(asset.Custodian) : null);
            Model.FromCustodianName = asset.Custodian;

            Model.ToBranchId = null;
            Model.ToBranchName = null;
            Model.ToLocationId = null;
            Model.ToLocationName = null;
            Model.ToCustodianUserId = null;
            Model.ToCustodianName = null;

            StateHasChanged();
        }

        private void OnToBranchChanged(ChangeEventArgs e)
        {
            if (!Guid.TryParse(e.Value?.ToString(), out var branchId))
            {
                Model.ToBranchId = null;
                Model.ToBranchName = null;
                StateHasChanged();
                return;
            }
            Model.ToBranchId = branchId;
            Model.ToBranchName = BranchList.FirstOrDefault(b => b.Id == branchId)?.BranchName;
            StateHasChanged();
        }

        private void OnToCustodianChanged(ChangeEventArgs e)
        {
            if (!Guid.TryParse(e.Value?.ToString(), out var id))
            {
                Model.ToCustodianUserId = null;
                Model.ToCustodianName = null;
            }
            else
            {
                var match = CustodianList.FirstOrDefault(x => x.Id == id);
                Model.ToCustodianUserId = id;
                Model.ToCustodianName = match.Name;
            }
            StateHasChanged();
        }

        private void OnToLocationChanged(ChangeEventArgs e)
        {
            if (!Guid.TryParse(e.Value?.ToString(), out var id))
            {
                Model.ToLocationId = null;
                Model.ToLocationName = null;
            }
            else
            {
                var match = LocationList.FirstOrDefault(x => x.Id == id);
                Model.ToLocationId = id;
                Model.ToLocationName = match.Name;
            }
            StateHasChanged();
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
                    await TransferService.UpdateAsync(Model);
                else
                    await TransferService.CreateAsync(Model);

                Nav.NavigateTo("/asset-transfer");
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
                1 => Model.FixedAssetId != Guid.Empty &&
                     Model.TransferType.HasValue &&
                     Model.EffectiveTransferDate != default,

                2 => !string.IsNullOrWhiteSpace(Model.TransferReason) &&
                     ValidateToFields(),

                3 => true,
                4 => true,
                _ => true
            };
        }

        private bool ValidateToFields()
        {
            return Model.TransferType switch
            {
                AT.TransferType.CustodianChange =>
                    Model.ToCustodianUserId.HasValue && Model.ToCustodianUserId != Guid.Empty,

                AT.TransferType.LocationChange =>
                    Model.ToLocationId.HasValue && Model.ToLocationId != Guid.Empty,

                AT.TransferType.BranchChange =>
                    Model.ToBranchId.HasValue && Model.ToBranchId != Guid.Empty,

                AT.TransferType.FullReassignment =>
                    Model.ToBranchId.HasValue && Model.ToBranchId != Guid.Empty &&
                    Model.ToLocationId.HasValue && Model.ToLocationId != Guid.Empty &&
                    Model.ToCustodianUserId.HasValue && Model.ToCustodianUserId != Guid.Empty,

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
            (1, "FixedAssetId") => Model.FixedAssetId != Guid.Empty,
            (1, "TransferType") => Model.TransferType.HasValue,
            (1, "EffectiveTransferDate") => Model.EffectiveTransferDate != default,
            (2, "TransferReason") => !string.IsNullOrWhiteSpace(Model.TransferReason),

            (2, "ToBranchId") => !(
                (Model.TransferType == AT.TransferType.BranchChange ||
                 Model.TransferType == AT.TransferType.FullReassignment) &&
                (!Model.ToBranchId.HasValue || Model.ToBranchId == Guid.Empty)
            ),

            (2, "ToLocationId") => !(
                (Model.TransferType == AT.TransferType.LocationChange ||
                 Model.TransferType == AT.TransferType.FullReassignment) &&
                (!Model.ToLocationId.HasValue || Model.ToLocationId == Guid.Empty)
            ),
            (2, "ToCustodianUserId") => !(
                (Model.TransferType == AT.TransferType.CustodianChange ||
                 Model.TransferType == AT.TransferType.FullReassignment) &&
                (!Model.ToCustodianUserId.HasValue || Model.ToCustodianUserId == Guid.Empty)
            ),

            _ => true
        };

        private record WizardStep(string Title, string Icon);
    }
}
