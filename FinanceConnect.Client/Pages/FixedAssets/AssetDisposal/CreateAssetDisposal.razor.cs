using FinanceConnect.Client.Services;
using FinanceConnect.Client.Shared;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Linq.Expressions;
using System.Text;
using static FinanceConnect.Client.Data.MasterDataIds;
using static FinanceConnect.Client.ViewModels.FixedAssetViewModel;

namespace FinanceConnect.Client.Pages.FixedAssets.AssetDisposal
{
    public partial class CreateAssetDisposal
    {

        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject] NavigationManager Nav { get; set; } = default!;

        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] BranchService BranchService { get; set; } = default!;
        [Inject] private AssetDisposalService Service { get; set; } = default!;
        [Inject] private FixedAssetService AssetService { get; set; } = default!;

        [Parameter] public Guid? Id { get; set; }

        protected bool IsEdit => Id.HasValue;

        private EditContext _editContext = default!;

        Guid TenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        RichTextEditor? _narrationEditor;

        protected AssetDisposalViewModel Model { get; set; }
            = new AssetDisposalViewModel();

        List<CompanyModel> Companies = new();
        List<BranchModel> Branches = new();

        protected List<FixedAssetViewModel.FixedAssetListDto> Assets { get; set; }
            = new();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("initFeatherIcons");
        }
        protected override async Task OnInitializedAsync()
        {
            Companies = MasterDataService.GetAllCompanies();
            Assets = AssetService.GetAll()
                .Where(x => x.AssetStatus == FixedAssetViewModel.AssetStatus.Active)
                .ToList();

            if (IsEdit)
            {
                var data = await Service.GetByIdAsync(Id!.Value);
                if (data != null)
                    Model = data;
            }
            else
            {
                Model = new AssetDisposalViewModel
                {
                    CreatedAt = DateTime.UtcNow,
                    DisposalStatus = AssetDisposalStatus.Draft,
                    DisposalNumber = AssetDisposalService.GenerateNumber()
                };
            }

            _editContext = new EditContext(Model);

        }


        private void LoadAssetSnapshot()
        {
            if (Model.FixedAssetId == Guid.Empty)
                return;

            var asset = Assets.FirstOrDefault(x => x.FixedAssetId == Model.FixedAssetId);

            if (asset == null)
                return;

            // Prevent disposal of already disposed asset
            if (asset.AssetStatus == AssetStatus.Disposed)
            {
                ToastService.ShowError("This asset is already disposed.");
                Model.FixedAssetId = Guid.Empty;
                return;
            }

            // Asset Snapshot

            Model.AssetNumberSnapshot = asset.AssetCode ?? "";
            Model.AssetNameSnapshot = asset.AssetName ?? "";

            Model.AssetCategoryIdSnapshot = asset.AssetCategoryId ?? Guid.Empty;
            Model.AssetCategoryNameSnapshot = asset.CategoryName ?? "";

            Model.AssetStatusSnapshot = asset.AssetStatus.ToString() ?? "";

            Model.InServiceDateSnapshot = asset.PurchaseDate;

            // Financial Snapshot

            Model.TotalCapitalizedCostSnapshot = asset.PurchaseCost;

            // Accumulated Depreciation
            var accumDep =
                asset.PurchaseCost -
                (asset.NetBookValue ?? asset.PurchaseCost);

            Model.AccumulatedDepreciationAsOfDisposalSnapshot = accumDep;

            // NBV
            Model.NetBookValueAsOfDisposalSnapshot =
                asset.NetBookValue ??
                (asset.PurchaseCost - accumDep);

        }


        private void RecalculateGainLoss()
        {
            Model.NetProceedsAmount =
                Model.ProceedsAmount - Model.DisposalExpenseAmount;

            Model.GainLossAmount =
                Model.NetProceedsAmount - Model.NetBookValueAsOfDisposalSnapshot;

            // Determine Gain/Loss Type
            if (Model.GainLossAmount > 0)
                Model.GainLossType = GainLossType.Gain;

            else if (Model.GainLossAmount < 0)
                Model.GainLossType = GainLossType.Loss;

            else
                Model.GainLossType = GainLossType.Neutral;
        }

        void OnCompanyChanged()
        {
            if (!Model.CompanyId.HasValue)
            {
                Branches = new List<BranchModel>();
                return;
            }

            var companyId = Model.CompanyId.Value;

            // Load branches
            Branches = BranchService
                .GetByCompanyId(companyId)
                .Where(b => b.Status == "Active")
                .ToList();
        }

        private string GetValidationClass(Expression<Func<object>> field)
        {
            if (_editContext == null)
                return string.Empty;

            var fieldIdentifier = FieldIdentifier.Create(field);

            var hasError = _editContext.GetValidationMessages(fieldIdentifier).Any();
            var isModified = _editContext.IsModified(fieldIdentifier);

            if (hasError)
                return "is-invalid";

            if (isModified)
                return "is-valid";

            return string.Empty;
        }

        protected async Task Save()
        {
            // Collect Quill editor values before validation
            if (_narrationEditor != null)
                Model.Narration = await _narrationEditor.GetHtmlAsync();

            if(Model.ProceedsAmount > 0){

                if(!Model.ProceedsMode.HasValue)
                ToastService.ShowError("Proceeds Mode requires", "Error");
                return;
            }
                if (!_editContext.Validate())
                return;

            if (IsEdit)
                await Service.UpdateAsync(Model);
            else
                await Service.CreateAsync(Model);

            Nav.NavigateTo("/asset-disposals");

        }

        string GetStatusBadge(string status)
        {

            return status switch
            {
                "Draft" => "bg-secondary-transparent text-secondary",
                "Generated" => "bg-info-transparent text-info",
                "Submitted" => "bg-warning-transparent text-warning",
                "Approved" => "bg-primary-transparent text-primary",
                "Posted" => "bg-success-transparent text-success",
                "Finalized" => "bg-dark-transparent text-dark",
                _ => "bg-secondary"
            };

        }
    }
}
