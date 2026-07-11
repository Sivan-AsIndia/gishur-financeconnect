using FinanceConnect.Client.Data;
using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace FinanceConnect.Client.Pages.FixedAssets.AssetRevaluationImpairment
{
    public partial class CreateRevaluationImpairment
    {
        [Inject] private AssetRevaluationImpairmentService Service { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;
        [Parameter] public Guid? Id { get; set; }

        private EditContext _editContext = default!;
        private ValidationMessageStore _messageStore = default!;

        protected AssetRevaluationImpairmentViewModel.AssetRevaluationImpairment Model { get; set; }
            = new AssetRevaluationImpairmentViewModel.AssetRevaluationImpairment();

        protected bool IsEdit => Id.HasValue;
        protected string CustomError = "";

        protected List<FixedAssetViewModel.FixedAssetListDto> FixedAssets { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            FixedAssets = FixedAssetSeedData.GetAllFixedAssets();

            if (IsEdit)
            {
                var data = await Service.GetByIdAsync(Id!.Value);
                if (data != null) Model = data;
            }
            else
            {
                Model.EffectiveDate = DateTime.Today;
            }

            _editContext = new EditContext(Model);
            _messageStore = new ValidationMessageStore(_editContext);
        }

        protected void OnAssetChanged()
        {
            if (Model.FixedAssetId.HasValue && Model.FixedAssetId != Guid.Empty)
            {
                var asset = FixedAssets.FirstOrDefault(a => a.FixedAssetId == Model.FixedAssetId);
                if (asset != null)
                {
                    Model.AssetNumberSnapshot = asset.AssetCode;
                    Model.AssetNameSnapshot = asset.AssetName;
                    Model.AssetStatusSnapshot = asset.AssetStatus.ToString();
                    Model.AssetCategoryIdSnapshot = asset.AssetCategoryId;
                    Model.InServiceDateSnapshot = asset.PurchaseDate;
                    Model.GrossCostBefore = asset.PurchaseCost;
                    Model.ResidualValueAmountBefore = asset.SalvageValue ?? 0.00m;
                }
            }
            else
            {
                Model.AssetNumberSnapshot = null;
                Model.AssetNameSnapshot = null;
                Model.AssetStatusSnapshot = null;
                Model.AssetCategoryIdSnapshot = null;
                Model.InServiceDateSnapshot = null;
            }
        }

        private bool ValidateCustom()
        {
            CustomError = "";

            if (Model.CalculationMode == AssetRevaluationImpairmentViewModel.CalculationModeEnum.AdjustByDelta
                && (Model.DeltaAmount == null || Model.DeltaAmount <= 0))
            {
                CustomError = "Delta Amount must be > 0 for Adjust By Delta mode.";
                return false;
            }

            if (Model.CalculationMode == AssetRevaluationImpairmentViewModel.CalculationModeEnum.RevalueToAmount
                && (Model.TargetCarryingAmount == null || Model.TargetCarryingAmount < 0))
            {
                CustomError = "Target Carrying Amount must be >= 0 for Revalue To Amount mode.";
                return false;
            }

            if (Model.CarryingValueAfter < 0)
            {
                CustomError = "Carrying Value After cannot be negative.";
                return false;
            }

            if (Model.AllowAccumDepTreatmentOverride && string.IsNullOrWhiteSpace(Model.AccumDepTreatmentReason))
            {
                CustomError = "Override reason is required when override is enabled.";
                return false;
            }

            return true;
        }

        protected async Task Save()
        {
            var modelValid = _editContext.Validate();
            var customValid = ValidateCustom();

            StateHasChanged();

            if (!modelValid || !customValid) return;

            if (IsEdit)
            {
                await Service.UpdateAsync(Model);
                ToastService.ShowSuccess("Valuation Event Updated Successfully", "Success");
            }
            else
            {
                await Service.CreateAsync(Model);
                ToastService.ShowSuccess("Valuation Event Created Successfully", "Success");
            }

            Nav.NavigateTo("/valuation-events");
        }
    }
}
