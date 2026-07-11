using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using static FinanceConnect.Client.ViewModels.ExpenseViewModel;
using ExpenseModel = FinanceConnect.Client.ViewModels.ExpenseViewModel.Expense;

namespace FinanceConnect.Client.Pages.Revenue_Expense.Expense
{
    public partial class ExpenseDetails : ComponentBase
    {
        [Parameter] public Guid Id { get; set; }
        [Inject] private ExpenseService Service { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        private ExpenseModel? Exp;
        private bool isInitialized;

        protected override void OnInitialized() { Exp = Service.GetById(Id); isInitialized = true; }

        private static string GetStatusBadge(ExpenseStatusEnum s) => s switch { ExpenseStatusEnum.Draft => "bg-warning-transparent", ExpenseStatusEnum.Submitted => "bg-info-transparent", ExpenseStatusEnum.UnderReview => "bg-info-transparent", ExpenseStatusEnum.Approved => "bg-primary-transparent", ExpenseStatusEnum.Posted => "bg-success-transparent", ExpenseStatusEnum.PartiallyPosted => "bg-warning-transparent", ExpenseStatusEnum.Rejected => "bg-danger-transparent", ExpenseStatusEnum.Cancelled => "bg-secondary-transparent text-secondary", ExpenseStatusEnum.Closed => "bg-success-transparent", _ => "bg-light" };
        private static string GetPostingBadge(PostingStatusEnum s) => s switch { PostingStatusEnum.Posted => "bg-success-transparent", PostingStatusEnum.Queued => "bg-info-transparent", PostingStatusEnum.Failed => "bg-danger-transparent", PostingStatusEnum.Reversed => "bg-warning-transparent", _ => "bg-secondary-transparent text-secondary" };
    }
}
