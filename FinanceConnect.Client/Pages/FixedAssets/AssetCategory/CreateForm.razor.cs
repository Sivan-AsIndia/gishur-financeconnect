using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Security.Cryptography;
using System.Text;
using static FinanceConnect.Client.Data.MasterDataIds;

namespace FinanceConnect.Client.Pages.FixedAssets.AssetCategory
{
    public partial class CreateForm
    {
        [Inject] private AssetCategoryService Service { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;
        [Parameter] public Guid? Id { get; set; }

        private EditContext _editContext = default!;
        private ValidationMessageStore _messageStore = default!;

        protected AssetsCategoryViewModel.AssetCategory Model { get; set; }
            = new AssetsCategoryViewModel.AssetCategory();

        protected bool IsEdit => Id.HasValue;
        protected bool IsController { get; set; } = true;

        protected List<AssetsCategoryViewModel.AssetCategory> ParentCategories { get; set; } = new();
        protected Dictionary<string, string> GLErrors { get; set; } = new();
        protected List<GLAccountViewModel> GLAccountList { get; set; } = new();

        protected List<DepreciationDropdownviewmodel> DepreciationMethods { get; set; } = new();

        protected enum GLField
        {
            AssetCost,
            AccumDep,
            DepExpense,
            DisposalGain,
            DisposalLoss,
            CapClearing,
            CWIP,
            Impairment,
            Revaluation,
            ExpenseAccountBelowThreshold
        }

        protected override async Task OnInitializedAsync()
        {
            //same GLAccount is used for Asset Cost Account (Balance Sheet) , Accumulated Depreciation Account and other dropdowns
            //need to add 
            GLAccountList = new List<GLAccountViewModel>
            {
                new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Code = "1100", Name = "Accounts Receivable" },
                new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Code = "2100", Name = "Customer Advances" },
                new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), Code = "6500", Name = "Bad Debts Write-Off" },
                new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), Code = "1110", Name = "AR – Export" },
                new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000005"), Code = "2110", Name = "Advances – Export" },
                new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000006"), Code = "1500", Name = "Fixed Assets – Cost" },
                new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000007"), Code = "1510", Name = "Accumulated Depreciation" },
                new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000008"), Code = "6100", Name = "Depreciation Expense" },
                new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000009"), Code = "7100", Name = "Gain on Asset Disposal" },
                new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000010"), Code = "7200", Name = "Loss on Asset Disposal" },
                new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000011"), Code = "1520", Name = "CWIP – Capital Work in Progress" },
                new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000012"), Code = "6200", Name = "Impairment Loss" },
                new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000013"), Code = "3100", Name = "Revaluation Reserve" },
                new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000014"), Code = "2200", Name = "Capitalization Clearing / GRIR" },
                new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000015"), Code = "6300", Name = "Asset Expense (Below Threshold)" },
            };

            //Need to use actual depreciation method data. Need to use DepreciationMethodSeedData.GetAll();
            DepreciationMethods = new List<DepreciationDropdownviewmodel>
            {
                new() { Id = Guid.Parse("dd000001-0000-0000-0000-000000000001"), Name = "Straight Line Method (SLM)" },
                new() { Id = Guid.Parse("dd000002-0000-0000-0000-000000000002"), Name = "Written Down Value (WDV)" },
                new() { Id = Guid.Parse("dd000003-0000-0000-0000-000000000003"), Name = "Double Declining Balance (DDB)" },
                new() { Id = Guid.Parse("dd000004-0000-0000-0000-000000000004"), Name = "Sum of Years Digits (SYD)" },
                new() { Id = Guid.Parse("dd000005-0000-0000-0000-000000000005"), Name = "Units of Production (UOP)" },
            };

            ParentCategories = Service.GetAll()
                .Where(x => !x.IsDeleted &&
                            x.CategoryStatus == AssetsCategoryViewModel.CategoryStatus.Active &&
                            (!IsEdit || x.AssetCategoryId != Id!.Value))
                .ToList();

            if (IsEdit)
            {
                var data = await Service.GetByIdAsync(Id!.Value);
                if (data != null)
                    Model = data;
            }

            _editContext = new EditContext(Model);
            _messageStore = new ValidationMessageStore(_editContext);
        }

        protected void OnGLChanged(ChangeEventArgs e, GLField field)
        {
            Guid? selected = Guid.TryParse(e.Value?.ToString(), out var g) ? g : (Guid?)null;

            switch (field)
            {
                case GLField.AssetCost:
                    Model.AssetCostGLAccountId = selected;
                    GLErrors.Remove(nameof(Model.AssetCostGLAccountId));
                    break;
                case GLField.AccumDep:
                    Model.AccumulatedDepreciationGLAccountId = selected;
                    GLErrors.Remove(nameof(Model.AccumulatedDepreciationGLAccountId));
                    break;
                case GLField.DepExpense:
                    Model.DepreciationExpenseGLAccountId = selected;
                    GLErrors.Remove(nameof(Model.DepreciationExpenseGLAccountId));
                    break;
                case GLField.DisposalGain:
                    Model.DisposalGainGLAccountId = selected;
                    GLErrors.Remove(nameof(Model.DisposalGainGLAccountId));
                    break;
                case GLField.DisposalLoss:
                    Model.DisposalLossGLAccountId = selected;
                    GLErrors.Remove(nameof(Model.DisposalLossGLAccountId));
                    break;
                case GLField.CapClearing:
                    Model.CapitalizationClearingGLAccountId = selected;
                    break;
                case GLField.CWIP:
                    Model.CWIPGLAccountId = selected;
                    break;
                case GLField.Impairment:
                    Model.ImpairmentLossGLAccountId = selected;
                    break;
                case GLField.Revaluation:
                    Model.RevaluationReserveGLAccountId = selected;
                    break;
                case GLField.ExpenseAccountBelowThreshold:
                    Model.ExpenseAccountIdForBelowThreshold = selected;
                    break;
            }

            StateHasChanged();
        }

        protected void OnDepMethodChanged(ChangeEventArgs e)
        {
            Model.DefaultDepreciationMethodId =
                Guid.TryParse(e.Value?.ToString(), out var g) ? g : (Guid?)null;
            GLErrors.Remove(nameof(Model.DefaultDepreciationMethodId));
            StateHasChanged();
        }

        protected bool HasGLError(string fieldName) => GLErrors.ContainsKey(fieldName);
        protected string GetGLError(string fieldName)
            => GLErrors.TryGetValue(fieldName, out var msg) ? msg : "";

        private bool ValidateGLFields()
        {
            GLErrors.Clear();

            if (Model.IsCapitalizable && Model.AssetCostGLAccountId == null)
                GLErrors[nameof(Model.AssetCostGLAccountId)] = "Asset Cost Account is required";

            if (Model.IsDepreciable && Model.AccumulatedDepreciationGLAccountId == null)
                GLErrors[nameof(Model.AccumulatedDepreciationGLAccountId)] = "Accumulated Depreciation Account is required";

            if (Model.IsDepreciable && Model.DepreciationExpenseGLAccountId == null)
                GLErrors[nameof(Model.DepreciationExpenseGLAccountId)] = "Depreciation Expense Account is required";

            if (Model.IsDepreciable && Model.DefaultDepreciationMethodId == null)
                GLErrors[nameof(Model.DefaultDepreciationMethodId)] = "Depreciation Method is required";

            if (Model.DisposalGainGLAccountId == null)
                GLErrors[nameof(Model.DisposalGainGLAccountId)] = "Gain on Disposal Account is required";

            if (Model.DisposalLossGLAccountId == null)
                GLErrors[nameof(Model.DisposalLossGLAccountId)] = "Loss on Disposal Account is required";

            return GLErrors.Count == 0;
        }

        protected async Task Save()
        {
            var modelValid = _editContext.Validate();
            var glValid = ValidateGLFields();

            StateHasChanged();

            if (!modelValid || !glValid) return;

            if (IsEdit)
            {
                await Service.UpdateAsync(Model);
                ToastService.ShowSuccess("Updated Successfully", "Success");
            }
            else
            {
                await Service.CreateAsync(Model);
                ToastService.ShowSuccess("Created Successfully", "Success");
            }

            Nav.NavigateTo("/assets-category");
        }
    }
}
