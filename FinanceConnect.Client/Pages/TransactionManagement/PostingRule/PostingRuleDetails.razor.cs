using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.TransactionManagement.PostingRule
{
    public partial class PostingRuleDetails
    {

        [Parameter] public Guid RuleId { get; set; }

        PostingRuleModel? SelectedRule;

        protected override void OnInitialized()
        {
            SelectedRule = RuleService.GetById(RuleId);
        }

        async Task GoBack()
        {
            await JS.InvokeVoidAsync("history.back");
        }
    }
}
