using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.AR.Customer
{
    public partial class CustomerDetails
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject] CustomerService CustomerService { get; set; } = default!;
        [Inject] CustomerAccountService CustomerAccountService { get; set; } = default!;

        [Parameter] public Guid Id { get; set; }

        private bool isInitialized = false;
        private CustomerViewModel? Customer;
        private AccountSummaryStatsViewModel? AccountSummary;

        protected override async Task OnInitializedAsync()
        {
            Customer = CustomerService.GetById(Id);
            
            if (Customer != null)
            {
                // Get account summary for this customer
                var accounts = CustomerAccountService.GetAll()
                    .Where(a => a.CustomerId == Customer.Id)
                    .ToList();

                if (accounts.Any())
                {
                    AccountSummary = new AccountSummaryStatsViewModel
                    {
                        AccountCount = accounts.Count,
                        TotalOutstandingReceivable = accounts.Sum(a => a.OutstandingReceivableAmount),
                        TotalUnappliedPayments = accounts.Sum(a => a.UnappliedPaymentAmount),
                        TotalAdvanceBalance = accounts.Sum(a => a.AdvanceBalanceAmount),
                        TotalCreditExposure = accounts.Sum(a => a.CreditExposureAmount),
                        AvailableCredit = Customer.CreditLimitAmount - accounts.Sum(a => a.CreditExposureAmount)
                    };
                }
            }

            isInitialized = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("feather.replace");
            }
        }

        private void GoBack()
        {
            Nav.NavigateTo("/customers");
        }

        private static string GetStatusBadgeClass(string status) => status switch
        {
            CustomerStatuses.Active => "bg-success",
            CustomerStatuses.Inactive => "bg-secondary",
            CustomerStatuses.Blacklisted => "bg-danger",
            CustomerStatuses.Draft => "bg-warning",
            _ => "bg-secondary"
        };

        private static string GetTypeBadgeClass(string type) => type switch
        {
            CustomerTypes.Business => "bg-info-transparent text-info",
            CustomerTypes.Individual => "bg-primary-transparent text-primary",
            CustomerTypes.Government => "bg-warning-transparent text-warning",
            CustomerTypes.Partner => "bg-success-transparent text-success",
            _ => "bg-secondary"
        };

        private static string GetHoldStatusBadgeClass(string holdStatus) => holdStatus switch
        {
            CreditHoldStatuses.None => "bg-success-transparent ",
            CreditHoldStatuses.OnHold => "bg-danger-transparent ",
            CreditHoldStatuses.TemporaryHold => "bg-warning-transparent ",
            _ => "bg-secondary-transparent text-secondary"
        };

        private static string GetKycBadgeClass(string kycStatus) => kycStatus switch
        {
            KycStatuses.Verified => "bg-success",
            KycStatuses.Pending => "bg-warning",
            KycStatuses.Failed => "bg-danger",
            KycStatuses.Expired => "bg-secondary",
            KycStatuses.NotStarted => "bg-light text-dark",
            _ => "bg-secondary"
        };

        public class AccountSummaryStatsViewModel
        {
            public int AccountCount { get; set; }
            public decimal TotalOutstandingReceivable { get; set; }
            public decimal TotalUnappliedPayments { get; set; }
            public decimal TotalAdvanceBalance { get; set; }
            public decimal TotalCreditExposure { get; set; }
            public decimal AvailableCredit { get; set; }
        }
    }
}
