using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.AR.CustomerPayment
{
    public partial class CreateCustomerPayment
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] CustomerPaymentService PaymentService { get; set; } = default!;
        [Inject] CustomerInvoiceService InvoiceService { get; set; } = default!;
        [Inject] CustomerService CustomerService { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] COADataService COADataService { get; set; } = default!;
        [Inject] BranchService BranchService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] IJSRuntime JSRuntime { get; set; } = default!;

        [Parameter] public Guid? Id { get; set; }

        private bool isInitialized = false;
        private CustomerPaymentViewModel Payment = new();

        // Dropdown data
        private List<CompanyModel> Companies = new();
        private List<BranchModel> Branches = new();
        private List<CustomerViewModel> Customers = new();
        private List<CustomerInvoiceViewModel> CustomerInvoices = new();
        private List<AccountViewModel> PaymentAccounts = new();

        // Base currency for foreign currency check [ADDED]
        private Guid BaseCurrencyId = MasterDataIds.Currencies.INR;

        // Validation errors dictionary
        private Dictionary<string, string> AllocationValidationErrors = new();
        private Dictionary<string, string> HeaderValidationErrors = new();

        // Quill editor reference
        private FinanceConnect.Client.Shared.RichTextEditor? narrationEditor;

        private bool IsEdit => Id.HasValue;
        private bool IsReadOnly => Payment.PaymentStatus != PaymentStatuses.Draft && 
                                   Payment.PaymentStatus != PaymentStatuses.Submitted;

        protected override async Task OnInitializedAsync()
        {
            await LoadMasterData();

            if (IsEdit)
            {
                var existing = PaymentService.GetById(Id!.Value);
                if (existing != null)
                {
                    Payment = existing;
                    LoadInvoicesForCustomer(Payment.CustomerId);
                }
                else
                {
                    ToastService.ShowError("Payment not found.", "Error");
                    Nav.NavigateTo("/customer-payments");
                    return;
                }
            }
            else
            {
                Payment = CreateNewPayment();
            }

            isInitialized = true;
        }

        private async Task LoadMasterData()
        {
            // Load companies from MasterDataService
            Companies = MasterDataService.GetAllCompanies()
                .Where(c => c.IsActive && !c.IsDeleted).ToList();

            // Load branches from BranchService
            Branches = BranchService.GetAll()
                .Where(b => b.Status == "Active").ToList();

            Customers = CustomerService.GetAll().Where(c => c.CustomerStatus == CustomerStatuses.Active).ToList();

            // Payment accounts (Cash and Bank GL accounts) from COADataService
            var allAccounts = COADataService.GetAllAccounts();
            PaymentAccounts = allAccounts
                .Where(a => a.AccountNature == AccountNatures.Asset && a.IsPostable && a.IsActive)
                .ToList();

            await Task.CompletedTask;
        }

        private void LoadInvoicesForCustomer(Guid customerId)
        {
            if (customerId == Guid.Empty)
            {
                CustomerInvoices = new List<CustomerInvoiceViewModel>();
                return;
            }

            // Get posted/partially paid invoices with outstanding balance
            CustomerInvoices = InvoiceService.GetByCustomerId(customerId)
                .Where(i => (i.InvoiceStatus == InvoiceStatuses.Posted ||
                             i.InvoiceStatus == InvoiceStatuses.PartiallyPaid) &&
                            i.AmountOutstanding > 0)
                .OrderBy(i => i.DueDate)
                .ToList();
        }

        private CustomerPaymentViewModel CreateNewPayment()
        {
            var defaultCompany = Companies.FirstOrDefault();
            var defaultBranch = Branches.FirstOrDefault();

            var companyId = defaultCompany?.Id ?? Guid.Empty;
            var receiptNumber = PaymentService.GenerateReceiptNumber(companyId);

            return new CustomerPaymentViewModel
            {
                CompanyId = companyId,
                CompanyName = defaultCompany?.LegalName,
                BranchId = defaultBranch?.Id ?? Guid.Empty,
                BranchName = defaultBranch?.BranchName,
                ReceiptNumber = receiptNumber,
                ReceiptDate = DateTime.Today,
                PaymentStatus = PaymentStatuses.Draft,
                ExchangeRate = 1,
                CurrencyId = MasterDataIds.Currencies.INR,
                CurrencyCode = "INR",
                CurrencyName = "Indian Rupee",
                PaymentMethod = "",
                PaymentAccountId = Guid.Empty,
                PaymentAccountCode = null,
                PaymentAccountName = null
            };
        }

        #region Event Handlers

        private void OnBranchChanged(ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var branchId))
            {
                Payment.BranchId = branchId;
                var branch = Branches.FirstOrDefault(b => b.Id == branchId);
                Payment.BranchName = branch?.BranchName;
            }
            ClearHeaderError("BranchId");
        }

        private void OnCustomerChanged()
        {
            if (Payment.CustomerId != Guid.Empty)
            {
                var customer = Customers.FirstOrDefault(c => c.Id == Payment.CustomerId);
                if (customer != null)
                {
                    Payment.CustomerCode = customer.CustomerCode;
                    Payment.CustomerName = customer.CustomerName;
                    Payment.CurrencyId = customer.DefaultCurrencyId ?? Payment.CurrencyId;
                    Payment.CurrencyCode = customer.DefaultCurrencyCode ?? Payment.CurrencyCode;
                    Payment.CurrencyName = customer.DefaultCurrencyName ?? Payment.CurrencyName;

                    // [ADDED] Reset exchange rate when customer changes
                    Payment.ExchangeRate = IsForeignCurrency() ? Payment.ExchangeRate : 1;
                }
                LoadInvoicesForCustomer(Payment.CustomerId);
                // Clear existing allocations when customer changes
                Payment.Allocations.Clear();
                Payment.RecalculateAmounts();
                ClearHeaderError("CustomerId");
            }
            else
            {
                Payment.CustomerId = Guid.Empty;
                Payment.CustomerCode = null;
                Payment.CustomerName = null;
                CustomerInvoices = new List<CustomerInvoiceViewModel>();
                Payment.Allocations.Clear();
            }
        }

        private void OnPaymentMethodChanged(ChangeEventArgs e)
        {
            Payment.PaymentMethod = e.Value?.ToString() ?? "";
            
            // Clear instrument fields when method changes
            Payment.InstrumentNumber = null;
            Payment.InstrumentDate = null;
            if (Payment.PaymentMethod != PaymentMethods.Cheque && Payment.PaymentMethod != PaymentMethods.BankTransfer)
            {
                Payment.BankName = null;
                Payment.BankAccountLast4 = null;
            }

            // [ADDED] Clear gateway fields when method changes away from Gateway/Card
            if (Payment.PaymentMethod != PaymentMethods.Gateway && Payment.PaymentMethod != PaymentMethods.Card)
            {
                Payment.GatewayProvider = null;
                Payment.GatewayTransactionId = null;
            }

            // Auto-select appropriate payment account based on method
            if (Payment.PaymentMethod == PaymentMethods.Cash)
            {
                var cashAccount = PaymentAccounts.FirstOrDefault(a => a.AccountCode == "1100");
                if (cashAccount != null)
                {
                    Payment.PaymentAccountId = cashAccount.Id;
                    Payment.PaymentAccountCode = cashAccount.AccountCode;
                    Payment.PaymentAccountName = cashAccount.AccountName;
                }
            }
            else if (Payment.PaymentMethod == PaymentMethods.Card)
            {
                var cardAccount = PaymentAccounts.FirstOrDefault(a => a.AccountCode == "1140");
                if (cardAccount != null)
                {
                    Payment.PaymentAccountId = cardAccount.Id;
                    Payment.PaymentAccountCode = cardAccount.AccountCode;
                    Payment.PaymentAccountName = cardAccount.AccountName;
                }
            }
            else if (Payment.PaymentMethod == PaymentMethods.Gateway)
            {
                var gatewayAccount = PaymentAccounts.FirstOrDefault(a => a.AccountCode == "1150");
                if (gatewayAccount != null)
                {
                    Payment.PaymentAccountId = gatewayAccount.Id;
                    Payment.PaymentAccountCode = gatewayAccount.AccountCode;
                    Payment.PaymentAccountName = gatewayAccount.AccountName;
                }
            }
            else
            {
                // Default to first bank account for bank transfers, UPI, Cheque
                var bankAccount = PaymentAccounts.FirstOrDefault(a => a.AccountCode == "1110");
                if (bankAccount != null)
                {
                    Payment.PaymentAccountId = bankAccount.Id;
                    Payment.PaymentAccountCode = bankAccount.AccountCode;
                    Payment.PaymentAccountName = bankAccount.AccountName;
                }
            }

            ClearHeaderError("PaymentMethod");
            ClearHeaderError("InstrumentNumber");
            ClearHeaderError("InstrumentDate");
            ClearHeaderError("BankName");
        }

        private void OnPaymentAccountChanged(ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var accountId))
            {
                Payment.PaymentAccountId = accountId;
                var account = PaymentAccounts.FirstOrDefault(a => a.Id == accountId);
                Payment.PaymentAccountCode = account?.AccountCode;
                Payment.PaymentAccountName = account?.AccountName;
            }
            ClearHeaderError("PaymentAccountId");
        }

        private void OnAmountChanged()
        {
            Payment.RecalculateAmounts();
            ClearHeaderError("PaymentAmountTotal");
        }

        private string GetInstrumentLabel()
        {
            return Payment.PaymentMethod switch
            {
                PaymentMethods.BankTransfer => "UTR Number",
                PaymentMethods.UPI => "UPI Reference",
                PaymentMethods.Cheque => "Cheque Number",
                PaymentMethods.Card => "Transaction Reference",
                PaymentMethods.Gateway => "Gateway Transaction ID",
                _ => "Reference Number"
            };
        }

        /// <summary>[ADDED] Check if current currency is foreign (different from base currency)</summary>
        private bool IsForeignCurrency()
        {
            return Payment.CurrencyId != Guid.Empty && Payment.CurrencyId != BaseCurrencyId;
        }

        #endregion

        #region Allocation Operations

        private void AddAllocation()
        {
            var newAllocation = new CustomerPaymentAllocationViewModel
            {
                CustomerPaymentId = Payment.Id
            };
            Payment.Allocations.Add(newAllocation);
        }

        private void RemoveAllocation(CustomerPaymentAllocationViewModel allocation)
        {
            var index = Payment.Allocations.IndexOf(allocation);
            Payment.Allocations.Remove(allocation);
            Payment.RecalculateAmounts();
            ClearAllocationErrors(index);
        }

        private void OnInvoiceSelected(CustomerPaymentAllocationViewModel allocation, ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var invoiceId))
            {
                allocation.CustomerInvoiceId = invoiceId;
                var invoice = CustomerInvoices.FirstOrDefault(i => i.Id == invoiceId);
                if (invoice != null)
                {
                    allocation.InvoiceNumber = invoice.InvoiceNumber;
                    allocation.InvoiceDate = invoice.InvoiceDate;
                    allocation.DueDate = invoice.DueDate;
                    allocation.InvoiceOutstanding = invoice.AmountOutstanding;
                    // Auto-set allocated amount to outstanding (or remaining payment amount)
                    var remainingPayment = Payment.PaymentAmountTotal - Payment.Allocations
                        .Where(a => a != allocation)
                        .Sum(a => a.AllocatedAmount);
                    allocation.AllocatedAmount = Math.Min(invoice.AmountOutstanding, Math.Max(0, remainingPayment));
                }
                Payment.RecalculateAmounts();
                var index = Payment.Allocations.IndexOf(allocation);
                ClearAllocationError(index, "CustomerInvoiceId");
            }
            else
            {
                allocation.CustomerInvoiceId = Guid.Empty;
                allocation.InvoiceNumber = null;
                allocation.InvoiceDate = null;
                allocation.DueDate = null;
                allocation.InvoiceOutstanding = 0;
                allocation.AllocatedAmount = 0;
            }
        }

        private void OnAllocationAmountChanged(CustomerPaymentAllocationViewModel allocation, ChangeEventArgs e)
        {
            if (decimal.TryParse(e.Value?.ToString(), out var amount))
            {
                allocation.AllocatedAmount = Math.Max(0, amount);
            }
            Payment.RecalculateAmounts();
            var index = Payment.Allocations.IndexOf(allocation);
            
            // Real-time validation for allocation
            ClearAllocationError(index, "AllocatedAmount");
            
            if (allocation.AllocatedAmount <= 0)
            {
                AllocationValidationErrors[$"{index}_AllocatedAmount"] = "Amount must be greater than zero.";
            }
            else if (allocation.AllocatedAmount > allocation.InvoiceOutstanding && allocation.InvoiceOutstanding > 0)
            {
                AllocationValidationErrors[$"{index}_AllocatedAmount"] = $"Cannot exceed outstanding ({allocation.InvoiceOutstanding:N2}).";
            }
            
            // Check if total allocations exceed payment amount
            var totalAllocated = Payment.Allocations.Sum(a => a.AllocatedAmount);
            if (totalAllocated > Payment.PaymentAmountTotal)
            {
                AllocationValidationErrors[$"{index}_AllocatedAmount"] = $"Total allocations ({totalAllocated:N2}) exceed payment amount ({Payment.PaymentAmountTotal:N2}).";
            }
            
            StateHasChanged();
        }

        /// <summary>Get available invoices for allocation (exclude already allocated ones)</summary>
        private List<CustomerInvoiceViewModel> GetAvailableInvoices(CustomerPaymentAllocationViewModel currentAllocation)
        {
            var allocatedInvoiceIds = Payment.Allocations
                .Where(a => a != currentAllocation && a.CustomerInvoiceId != Guid.Empty)
                .Select(a => a.CustomerInvoiceId)
                .ToHashSet();

            return CustomerInvoices
                .Where(i => !allocatedInvoiceIds.Contains(i.Id) || i.Id == currentAllocation.CustomerInvoiceId)
                .ToList();
        }

        #endregion

        #region Validation

        private bool ValidateHeader()
        {
            HeaderValidationErrors.Clear();

            if (Payment.BranchId == Guid.Empty)
                HeaderValidationErrors["BranchId"] = "Branch is required.";

            if (Payment.CustomerId == Guid.Empty)
                HeaderValidationErrors["CustomerId"] = "Customer is required.";

            if (Payment.PaymentAccountId == Guid.Empty)
                HeaderValidationErrors["PaymentAccountId"] = "Payment Account is required.";

            if (string.IsNullOrWhiteSpace(Payment.PaymentMethod))
                HeaderValidationErrors["PaymentMethod"] = "Payment Method is required.";

            if (Payment.PaymentAmountTotal <= 0)
                HeaderValidationErrors["PaymentAmountTotal"] = "Amount must be greater than zero.";

            // [ADDED] Validate PostingDate >= ReceiptDate if provided
            if (Payment.PostingDate.HasValue && Payment.PostingDate.Value < Payment.ReceiptDate)
            {
                HeaderValidationErrors["PostingDate"] = "Posting Date must be on or after Receipt Date.";
            }

            // [ADDED] Validate ExchangeRate for foreign currency
            if (IsForeignCurrency() && Payment.ExchangeRate <= 0)
            {
                HeaderValidationErrors["ExchangeRate"] = "Exchange Rate is required and must be positive for foreign currency.";
            }

            // Validate instrument details based on payment method (only if method is selected)
            if (!string.IsNullOrWhiteSpace(Payment.PaymentMethod))
            {
                if (PaymentMethods.RequiresInstrumentNumber(Payment.PaymentMethod) && 
                    string.IsNullOrWhiteSpace(Payment.InstrumentNumber))
                {
                    HeaderValidationErrors["InstrumentNumber"] = $"{GetInstrumentLabel()} is required for {PaymentMethods.GetDisplayName(Payment.PaymentMethod)}.";
                }

                if (PaymentMethods.RequiresInstrumentDate(Payment.PaymentMethod) && Payment.InstrumentDate == null)
                {
                    HeaderValidationErrors["InstrumentDate"] = "Instrument Date is required.";
                }

                if (PaymentMethods.RequiresBankName(Payment.PaymentMethod) && string.IsNullOrWhiteSpace(Payment.BankName))
                {
                    HeaderValidationErrors["BankName"] = "Bank Name is required for Cheque payments.";
                }
            }

            return !HeaderValidationErrors.Any();
        }

        private bool ValidateAllAllocations()
        {
            AllocationValidationErrors.Clear();
            bool isValid = true;

            for (int i = 0; i < Payment.Allocations.Count; i++)
            {
                var allocation = Payment.Allocations[i];

                if (allocation.CustomerInvoiceId == Guid.Empty)
                {
                    AllocationValidationErrors[$"{i}_CustomerInvoiceId"] = "Invoice is required.";
                    isValid = false;
                }

                if (allocation.AllocatedAmount <= 0)
                {
                    AllocationValidationErrors[$"{i}_AllocatedAmount"] = "Amount must be greater than zero.";
                    isValid = false;
                }

                if (allocation.AllocatedAmount > allocation.InvoiceOutstanding)
                {
                    AllocationValidationErrors[$"{i}_AllocatedAmount"] = "Cannot exceed outstanding amount.";
                    isValid = false;
                }
            }

            // Check total allocation doesn't exceed payment amount
            var totalAllocated = Payment.Allocations.Sum(a => a.AllocatedAmount);
            if (totalAllocated > Payment.PaymentAmountTotal)
            {
                // Add error to the last allocation
                if (Payment.Allocations.Any())
                {
                    var lastIndex = Payment.Allocations.Count - 1;
                    AllocationValidationErrors[$"{lastIndex}_AllocatedAmount"] = 
                        "Total allocations exceed payment amount.";
                    isValid = false;
                }
            }

            return isValid;
        }

        private bool HasAllocationValidationError(int allocIndex, string field) =>
            AllocationValidationErrors.ContainsKey($"{allocIndex}_{field}");

        private string GetAllocationValidationError(int allocIndex, string field) =>
            AllocationValidationErrors.TryGetValue($"{allocIndex}_{field}", out var error) ? error : string.Empty;

        private string GetAllocationValidationClass(int allocIndex, string field) =>
            HasAllocationValidationError(allocIndex, field) ? "form-control is-invalid" : "form-control";

        private bool HasHeaderError(string field) => HeaderValidationErrors.ContainsKey(field);
        private string GetHeaderError(string field) =>
            HeaderValidationErrors.TryGetValue(field, out var error) ? error : string.Empty;
        private string GetHeaderValidationClass(string field) =>
            HasHeaderError(field) ? "is-invalid" : "";

        private void ClearAllocationError(int allocIndex, string field) =>
            AllocationValidationErrors.Remove($"{allocIndex}_{field}");

        private void ClearAllocationErrors(int allocIndex)
        {
            var keysToRemove = AllocationValidationErrors.Keys.Where(k => k.StartsWith($"{allocIndex}_")).ToList();
            foreach (var key in keysToRemove)
                AllocationValidationErrors.Remove(key);
        }

        private void ClearHeaderError(string field) => HeaderValidationErrors.Remove(field);

        private async Task ScrollToFirstValidationError()
        {
            await JSRuntime.InvokeVoidAsync("scrollToFirstValidationError");
        }

        #endregion

        #region Form Actions

        private async Task HandleSubmit()
        {
            // Sync Quill editor content
            if (narrationEditor != null)
            {
                Payment.PaymentNarration = await narrationEditor.GetHtmlAsync();
            }

            if (!ValidateHeader())
            {
                StateHasChanged();
                await ScrollToFirstValidationError();
                return;
            }

            // Validate allocations only if there are any
            if (Payment.Allocations.Any() && !ValidateAllAllocations())
            {
                StateHasChanged();
                await ScrollToFirstValidationError();
                return;
            }

            // Recalculate amounts
            Payment.RecalculateAmounts();

            if (IsEdit)
            {
                Payment.UpdatedAt = DateTime.Now;
                Payment.UpdatedBy = "Current User";
                var result = PaymentService.Update(Payment);
                if (result.Success)
                {
                    ToastService.ShowSuccess($"Payment '{Payment.ReceiptNumber}' updated successfully.", "Updated");
                    Nav.NavigateTo("/customer-payments");
                }
                else
                {
                    ToastService.ShowError(result.Message, "Error");
                }
            }
            else
            {
                Payment.CreatedAt = DateTime.Now;
                Payment.CreatedBy = "Current User";
                var result = PaymentService.Add(Payment);
                if (result.Success)
                {
                    ToastService.ShowSuccess($"Payment '{Payment.ReceiptNumber}' created successfully.", "Created");
                    Nav.NavigateTo("/customer-payments");
                }
                else
                {
                    ToastService.ShowError(result.Message, "Error");
                }
            }

            await Task.CompletedTask;
        }

        private async Task SubmitForApproval()
        {
            // Sync Quill editor content
            if (narrationEditor != null)
            {
                Payment.PaymentNarration = await narrationEditor.GetHtmlAsync();
            }

            if (!ValidateHeader())
            {
                StateHasChanged();
                await ScrollToFirstValidationError();
                return;
            }

            if (Payment.Allocations.Any() && !ValidateAllAllocations())
            {
                StateHasChanged();
                await ScrollToFirstValidationError();
                return;
            }

            // First save, then submit
            Payment.RecalculateAmounts();

            if (IsEdit)
            {
                Payment.UpdatedAt = DateTime.Now;
                Payment.UpdatedBy = "Current User";
                var updateResult = PaymentService.Update(Payment);
                if (!updateResult.Success)
                {
                    ToastService.ShowError(updateResult.Message, "Error");
                    return;
                }
            }
            else
            {
                Payment.CreatedAt = DateTime.Now;
                Payment.CreatedBy = "Current User";
                var addResult = PaymentService.Add(Payment);
                if (!addResult.Success)
                {
                    ToastService.ShowError(addResult.Message, "Error");
                    return;
                }
            }

            var submitResult = PaymentService.Submit(Payment.Id, "Current User");
            if (submitResult.Success)
            {
                ToastService.ShowSuccess($"Payment '{Payment.ReceiptNumber}' submitted for approval.", "Submitted");
                Nav.NavigateTo("/customer-payments");
            }
            else
            {
                ToastService.ShowError(submitResult.Message, "Error");
            }

            await Task.CompletedTask;
        }

        private async Task PostPayment()
        {
            if (!Payment.CanPost)
            {
                ToastService.ShowWarning("Payment cannot be posted in its current status.", "Warning");
                return;
            }

            var result = PaymentService.Post(Payment.Id, "Current User");
            if (result.Success)
            {
                ToastService.ShowSuccess($"Payment '{Payment.ReceiptNumber}' posted successfully.", "Posted");
                Nav.NavigateTo("/customer-payments");
            }
            else
            {
                ToastService.ShowError(result.Message, "Error");
            }

            await Task.CompletedTask;
        }

        private void Cancel()
        {
            Nav.NavigateTo("/customer-payments");
        }

        #endregion

        #region UI Helpers

        private string GetStatusBadge(string status) => status switch
        {
            PaymentStatuses.Draft => "bg-secondary",
            PaymentStatuses.Submitted => "bg-info",
            PaymentStatuses.Approved => "bg-primary",
            PaymentStatuses.Posted => "bg-success",
            PaymentStatuses.Reversed => "bg-danger",
            PaymentStatuses.Cancelled => "bg-warning text-dark",
            _ => "bg-secondary"
        };

        #endregion
    }
}
