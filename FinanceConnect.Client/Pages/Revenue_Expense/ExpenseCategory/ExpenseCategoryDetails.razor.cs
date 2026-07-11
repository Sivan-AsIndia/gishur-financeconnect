using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using static FinanceConnect.Client.ViewModels.ExpenseCategoryViewModel;
using ExpenseCategoryModel = FinanceConnect.Client.ViewModels.ExpenseCategoryViewModel.ExpenseCategory;

namespace FinanceConnect.Client.Pages.Revenue_Expense.ExpenseCategory
{
    public partial class ExpenseCategoryDetails : ComponentBase
    {
        [Parameter] public Guid Id { get; set; }
        [Inject] private ExpenseCategoryService Service { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        private ExpenseCategoryModel? Cat;
        private string? ParentCategoryName;
        private bool isInitialized;

        protected override void OnInitialized()
        {
            Cat = Service.GetById(Id);
            if (Cat?.ParentExpenseCategoryId != null)
            {
                var parent = Service.GetById(Cat.ParentExpenseCategoryId.Value);
                ParentCategoryName = parent != null ? $"{parent.CategoryCode} – {parent.CategoryName}" : null;
            }
            isInitialized = true;
        }

        private static string GetStatusBadge(CategoryStatusEnum s) => s switch
        {
            CategoryStatusEnum.Active => "bg-success-transparent",
            CategoryStatusEnum.Draft => "bg-warning-transparent",
            CategoryStatusEnum.Inactive => "bg-secondary-transparent",
            CategoryStatusEnum.Archived => "bg-danger-transparent",
            _ => "bg-light"
        };
    }
}
