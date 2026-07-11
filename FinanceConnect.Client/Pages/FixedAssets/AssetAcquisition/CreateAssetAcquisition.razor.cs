using FinanceConnect.Client.Data;
using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace FinanceConnect.Client.Pages.FixedAssets.AssetAcquisition
{
    public partial class CreateAssetAcquisition
    {
        [Inject] private AssetAcquisitionService Service { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;

        [Parameter] public Guid? Id { get; set; }

        private EditContext _editContext = default!;
        private ValidationMessageStore _messageStore = default!;

        protected AssetAcquisitionViewModel.AssetAcquisition Model { get; set; }
            = new AssetAcquisitionViewModel.AssetAcquisition();

        protected bool IsEdit => Id.HasValue;
        protected string CostLineError = "";

        protected List<FixedAssetViewModel.FixedAssetListDto> FixedAssets { get; set; } = new();

        // Default IDs for demo app (no branch/tenant picker in form)
        private static readonly Guid DefaultTenantId  = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        private static readonly Guid DefaultCompanyId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        private static readonly Guid DefaultBranchId  = Guid.Parse("70000000-0000-0000-0000-000000000001");

        protected override async Task OnInitializedAsync()
        {
            FixedAssets = FixedAssetSeedData.GetAllFixedAssets();

            if (IsEdit)
            {
                var data = await Service.GetByIdAsync(Id!.Value);
                if (data != null)
                    Model = data;
            }
            else
            {
                Model.AcquisitionDate = DateTime.Today;
                Model.CapitalizationDate = DateTime.Today;
                Model.TenantId  = DefaultTenantId;
                Model.CompanyId = DefaultCompanyId;
                Model.BranchId  = DefaultBranchId;
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
                }
            }
            else
            {
                Model.AssetNumberSnapshot = null;
                Model.AssetNameSnapshot = null;
                Model.AssetStatusSnapshot = null;
                Model.AssetCategoryIdSnapshot = null;
            }
        }

        protected void AddCostLine()
        {
            int nextLineNo = Model.CostLines.Any()
                ? Model.CostLines.Max(l => l.LineNumber) + 10
                : 10;

            Model.CostLines.Add(new AssetAcquisitionViewModel.AssetAcquisitionLine
            {
                AssetAcquisitionId = Model.AssetAcquisitionId,
                LineNumber = nextLineNo,
                IsCapitalizable = true
            });
            CostLineError = "";
        }

        protected void RemoveCostLine(int index)
        {
            if (index >= 0 && index < Model.CostLines.Count)
                Model.CostLines.RemoveAt(index);
        }

        private bool ValidateCustom()
        {
            CostLineError = "";
            bool valid = true;

            if (!Model.CostLines.Any())
            {
                CostLineError = "At least one cost line is required.";
                valid = false;
            }
            else
            {
                foreach (var line in Model.CostLines)
                {
                    if (line.CostComponentType == null)
                    {
                        CostLineError = "All cost lines must have a Cost Component Type.";
                        valid = false;
                        break;
                    }
                    if (line.LineAmount <= 0)
                    {
                        CostLineError = "All cost line amounts must be greater than 0.";
                        valid = false;
                        break;
                    }
                }
            }

            if (valid && Model.TotalCapitalizedAmount <= 0)
            {
                CostLineError = "Total Capitalized Amount must be > 0.";
                valid = false;
            }

            return valid;
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
                ToastService.ShowSuccess("Acquisition Updated Successfully", "Success");
            }
            else
            {
                await Service.CreateAsync(Model);
                ToastService.ShowSuccess("Acquisition Created Successfully", "Success");
            }

            Nav.NavigateTo("/asset-acquisitions");
        }
    }
}
