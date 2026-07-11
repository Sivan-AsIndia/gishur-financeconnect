using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using FinanceConnect.Client.Services;
using static FinanceConnect.Client.ViewModels.RevenueRecognitionViewModel;

namespace FinanceConnect.Client.Pages.Revenue_Expense.RevenueRecognition
{
    public partial class RevenueRecognitionDetails : ComponentBase
    {
        [Parameter] public Guid Id { get; set; }
        [Inject] private RevenueRecognitionService Service { get; set; } = default!;

        private RevenueRecognitionViewModel.RevenueRecognition? Item;
        private bool isInitialized = false;

        protected override async Task OnParametersSetAsync()
        {
            isInitialized = false;
            Item = Service.GetById(Id);
            isInitialized = true;
            await Task.CompletedTask;
        }

        private string GetStatusBadge(RecognitionStatusEnum s) => s switch
        {
            RecognitionStatusEnum.Draft               => "bg-secondary-transparent",
            RecognitionStatusEnum.Ready               => "bg-info-transparent",
            RecognitionStatusEnum.Scheduled           => "bg-primary-transparent",
            RecognitionStatusEnum.InProgress          => "bg-primary-transparent",
            RecognitionStatusEnum.PartiallyRecognized => "bg-warning-transparent",
            RecognitionStatusEnum.FullyRecognized     => "bg-success-transparent",
            RecognitionStatusEnum.OnHold              => "bg-danger-transparent",
            RecognitionStatusEnum.Cancelled           => "bg-danger-transparent",
            RecognitionStatusEnum.Closed              => "bg-dark",
            _                                         => "bg-secondary-transparent"
        };

        private string GetMethodBadge(RecognitionMethodEnum m) => m switch
        {
            RecognitionMethodEnum.Immediate             => "bg-success-transparent",
            RecognitionMethodEnum.Scheduled              => "bg-primary-transparent",
            RecognitionMethodEnum.MilestoneTriggered     => "bg-warning-transparent",
            RecognitionMethodEnum.ManualApprovalRequired => "bg-info-transparent",
            RecognitionMethodEnum.DeferredThenRelease    => "bg-purple-transparent",
            _                                            => "bg-secondary-transparent"
        };

        private string GetLineStatusBadge(RecognitionLineStatusEnum s) => s switch
        {
            RecognitionLineStatusEnum.Recognized          => "bg-success-transparent text-success",
            RecognitionLineStatusEnum.PartiallyRecognized => "bg-warning-transparent text-warning",
            RecognitionLineStatusEnum.Ready               => "bg-info-transparent text-info",
            RecognitionLineStatusEnum.Planned             => "bg-secondary-transparent text-secondary",
            RecognitionLineStatusEnum.Deferred            => "bg-primary-transparent text-primary",
            RecognitionLineStatusEnum.Skipped             => "bg-dark-transparent text-dark",
            RecognitionLineStatusEnum.Cancelled           => "bg-danger-transparent text-danger",
            _                                             => "bg-secondary-transparent text-secondary"
        };

        private string GetApprovalBadge(ManualApprovalStatusEnum s) => s switch
        {
            ManualApprovalStatusEnum.Approved    => "bg-success-transparent text-success",
            ManualApprovalStatusEnum.Pending     => "bg-warning-transparent text-warning",
            ManualApprovalStatusEnum.Rejected    => "bg-danger-transparent text-danger",
            ManualApprovalStatusEnum.NotRequired => "bg-secondary-transparent text-secondary",
            _                                    => "bg-secondary-transparent text-secondary"
        };

        private MarkupString BoolBadge(bool value) =>
            value
                ? new MarkupString("<span class=\"badge badge-green\">Yes</span>")
                : new MarkupString("<span class=\"badge badge-gray\">No</span>");

        private string GetCurrencyCode(Guid currencyId)
        {
            var codes = new Dictionary<Guid, string>
            {
                { Data.MasterDataIds.Currencies.INR, "INR" },
                { Data.MasterDataIds.Currencies.USD, "USD" },
                { Data.MasterDataIds.Currencies.GBP, "GBP" },
                { Data.MasterDataIds.Currencies.EUR, "EUR" },
                { Data.MasterDataIds.Currencies.AED, "AED" },
                { Data.MasterDataIds.Currencies.SGD, "SGD" },
                { Data.MasterDataIds.Currencies.JPY, "JPY" },
                { Data.MasterDataIds.Currencies.AUD, "AUD" },
                { Data.MasterDataIds.Currencies.CAD, "CAD" },
            };
            return codes.TryGetValue(currencyId, out var code) ? code : "—";
        }

        private string GetFiscalYearName(Guid fyId)
        {
            var years = new Dictionary<Guid, string>
            {
                { Data.MasterDataIds.FiscalYears.FY2025_26, "FY 2025-26" },
                { Data.MasterDataIds.FiscalYears.FY2024_25, "FY 2024-25" },
            };
            return years.TryGetValue(fyId, out var name) ? name : "—";
        }
    }
}
