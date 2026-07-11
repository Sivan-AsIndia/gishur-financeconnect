using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;

namespace FinanceConnect.Client.Pages.FixedAssets.DepreciationMethod
{
    public partial class DepreciationMethodView
    {
        [Parameter] public Guid Id { get; set; }

        [Inject] private DepreciationMethodService Service { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;

        private DepreciationMethodViewModel.DepreciationMethod? Item;
        private bool isInitialized = false;

        protected override async Task OnParametersSetAsync()
        {
            isInitialized = false;
            Item = await Service.GetByIdAsync(Id);
            isInitialized = true;
        }

        private string FormatEnumName(string? name)
        {
            if (string.IsNullOrEmpty(name)) return "";
            return string.Concat(name.Select((c, i) => i > 0 && char.IsUpper(c) ? " " + c : c.ToString()));
        }

        private string GetStatusBadgeClass(DepreciationMethodViewModel.MethodStatusEnum status) => status switch
        {
            DepreciationMethodViewModel.MethodStatusEnum.Active => "bg-success-transparent",
            DepreciationMethodViewModel.MethodStatusEnum.Inactive => "bg-danger-transparent",
            DepreciationMethodViewModel.MethodStatusEnum.Archived => "bg-warning-transparent",
            _ => "bg-secondary-transparent"
        };
    }
}
