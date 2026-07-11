using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.RevenueViewModel;

namespace FinanceConnect.Client.Pages.Revenue_Expense.Revenue
{
    public partial class RevenueDetails
    {
        [Parameter] public Guid Id { get; set; }

        [Inject] RevenueService    RevenueService    { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] NavigationManager Nav               { get; set; } = default!;
        [Inject] IJSRuntime        JS                { get; set; } = default!;
        [Inject] ToastService      ToastService      { get; set; } = default!;

        private bool isInitialized = false;
        private ViewModels.RevenueViewModel.Revenue? Revenue = null;

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
            isInitialized = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                await JS.InvokeVoidAsync("feather.replace");
        }

        private async Task LoadDataAsync()
        {
            await Task.Delay(100);
            Revenue = RevenueService.GetById(Id);
        }

        // ── UI Helpers ─────────────────────────────────────────────────────────

        private string GetCurrencySymbol(Guid currencyId)
        {
            var symbols = new Dictionary<Guid, string>
            {
                { Data.MasterDataIds.Currencies.INR, "₹" },
                { Data.MasterDataIds.Currencies.USD, "$" },
                { Data.MasterDataIds.Currencies.GBP, "£" },
                { Data.MasterDataIds.Currencies.EUR, "€" },
                { Data.MasterDataIds.Currencies.AED, "د.إ" },
                { Data.MasterDataIds.Currencies.SGD, "S$" },
                { Data.MasterDataIds.Currencies.JPY, "¥" },
                { Data.MasterDataIds.Currencies.AUD, "A$" },
                { Data.MasterDataIds.Currencies.CAD, "C$" },
            };
            return symbols.TryGetValue(currencyId, out var sym) ? sym : "";
        }

        private string GetCurrencyCode(Guid currencyId)
        {
            var currencies = MasterDataService.GetAllCurrencies();
            var currency   = currencies.FirstOrDefault(c => c.Id == currencyId);
            return currency?.CurrencyCode ?? "—";
        }

        private string GetStatusBadgeClass(RevenueStatus status) => status switch
        {
            RevenueStatus.Draft               => "bg-secondary-transparent text-secondary",
            RevenueStatus.Confirmed           => "bg-primary-transparent text-primary",
            RevenueStatus.PendingRecognition  => "bg-warning-transparent text-warning",
            RevenueStatus.PartiallyRecognized => "bg-info-transparent text-info",
            RevenueStatus.FullyRecognized     => "bg-success-transparent text-success",
            RevenueStatus.Deferred            => "bg-purple-transparent text-purple",
            RevenueStatus.Cancelled           => "bg-danger-transparent text-danger",
            RevenueStatus.Closed              => "bg-dark-transparent text-dark",
            _                                 => "bg-secondary-transparent text-secondary"
        };

        private string GetRecognitionBadgeClass(RecognitionStatus status) => status switch
        {
            RecognitionStatus.NotStarted          => "bg-secondary-transparent text-secondary",
            RecognitionStatus.Ready               => "bg-info-transparent text-info",
            RecognitionStatus.InProgress          => "bg-primary-transparent text-primary",
            RecognitionStatus.PartiallyRecognized => "bg-warning-transparent text-warning",
            RecognitionStatus.FullyRecognized     => "bg-success-transparent text-success",
            RecognitionStatus.Deferred            => "bg-purple-transparent text-purple",
            RecognitionStatus.OnHold              => "bg-danger-transparent text-danger",
            _                                     => "bg-secondary-transparent text-secondary"
        };

        private string GetBillingBadgeClass(BillingStatus s) => s switch
        {
            BillingStatus.NotBilled       => "bg-secondary-transparent text-secondary",
            BillingStatus.PartiallyBilled => "bg-warning-transparent text-warning",
            BillingStatus.FullyBilled     => "bg-success-transparent text-success",
            BillingStatus.AdvanceBilled   => "bg-info-transparent text-info",
            _                             => "bg-secondary-transparent text-secondary"
        };

        private string GetCollectionBadgeClass(CollectionStatus s) => s switch
        {
            CollectionStatus.NotCollected       => "bg-secondary-transparent text-secondary",
            CollectionStatus.PartiallyCollected => "bg-warning-transparent text-warning",
            CollectionStatus.FullyCollected     => "bg-success-transparent text-success",
            CollectionStatus.AdvanceCollected   => "bg-info-transparent text-info",
            _                                   => "bg-secondary-transparent text-secondary"
        };
    }
}
