using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;

namespace FinanceConnect.Client.Pages.FixedAssets.AssetCategory
{
    public partial class Assetcategoryview
    {
        [Parameter] public Guid Id { get; set; }

        private AssetsCategoryViewModel.AssetCategory? Category { get; set; }
        private bool isInitialized = false;

        protected override async Task OnParametersSetAsync()
        {
            isInitialized = false;
            Category = await Service.GetByIdAsync(Id);
            isInitialized = true;
        }
        private string GetStatusBadgeClass(AssetsCategoryViewModel.CategoryStatus status) =>
            status switch
            {
                AssetsCategoryViewModel.CategoryStatus.Active => "badge-green",
                AssetsCategoryViewModel.CategoryStatus.Inactive => "badge-red",
                AssetsCategoryViewModel.CategoryStatus.Archived => "badge-amber",
                _ => "badge-gray"
            };
        private MarkupString BoolBadge(bool value, string trueLabel, string falseLabel) =>
            value
                ? new MarkupString($"<span class=\"badge badge-green\">{trueLabel}</span>")
                : new MarkupString($"<span class=\"badge badge-gray\">{falseLabel}</span>");

       
        private string GLAccountName(Guid? id)
            => Service.GetGLAccountName(id);

        private string DepreciationMethodName(Guid? id)
            => Service.GetDepreciationMethodName(id);

        private string ParentCategoryName(Guid? id)
        {
            if (!id.HasValue) return "None";
            var parent = Service.GetById(id.Value);
            return parent != null ? $"{parent.CategoryCode} – {parent.CategoryName}" : "–";
        }
    }
}
