using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using FinanceConnect.Client.Shared;

namespace FinanceConnect.Client.Pages.AP.VendorPayment
{
    public partial class CreateVendorPayment
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] VendorPaymentService PaymentService { get; set; } = default!;
        [Inject] VendorBillService BillService { get; set; } = default!;
        [Inject] VendorService VendorService { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] BranchService BranchService { get; set; } = default!;
        [Inject] COADataService COADataService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] IJSRuntime JSRuntime { get; set; } = default!;

        [Parameter] public Guid? Id { get; set; }

        private bool isInitialized = false;
        private RichTextEditor? narrationEditor;
        private VendorPaymentViewModel Payment = new();

        // Dropdown data
        private List<CompanyModel> Companies = new();
        private List<BranchModel> Branches = new();
        private List<VendorViewModel> Vendors = new();
        private List<VendorBillViewModel> VendorBills = new();
        private List<AccountViewModel> PaymentAccounts = new();

        // Validation errors dictionary
        private Dictionary<string, string> AllocationValidationErrors = new();
        private Dictionary<string, string> HeaderValidationErrors = new();

        private bool IsEdit => Id.HasValue;
        private bool IsReadOnly => Payment.PaymentStatus != VendorPaymentStatuses.Draft &&
                                   Payment.PaymentStatus != VendorPaymentStatuses.Submitted;

        protected override async Task OnInitializedAsync()
        {
            await LoadMasterData();

            if (IsEdit)
            {
                var existing = PaymentService.GetById(Id!.Value);
                if (existing != null)
                {
                    Payment = existing;
                    LoadBillsForVendor(Payment.VendorId);
                }
                else
                {
                    ToastService.ShowError("Payment not found.", "Error");
                    Nav.NavigateTo("/vendor-payments");
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
                .Where(c => c.Status == "Active")
                .ToList();

            // Load branches from BranchService
            Branches = BranchService.GetAll()
                .Where(b => b.Status == "Active")
                .ToList();

            Vendors = VendorService.GetAll().Where(v => v.VendorStatus == VendorStatuses.Active).ToList();

            // Payment accounts (Bank and Cash GL accounts) from COADataService
            PaymentAccounts = COADataService.GetAllAccounts();

            await Task.CompletedTask;
        }

        private void LoadBillsForVendor(Guid vendorId)
        {
            if (vendorId == Guid.Empty)
            {
                VendorBills = new List<VendorBillViewModel>();
                return;
            }

            // Get posted bills with outstanding balance (Unpaid or PartiallyPaid settlement status)
            VendorBills = BillService.GetByVendorId(vendorId)
                .Where(b => b.BillStatus == VendorBillStatuses.Posted &&
                            b.AmountOutstanding > 0)
                .OrderBy(b => b.DueDate)
                .ToList();
        }

        private VendorPaymentViewModel CreateNewPayment()
        {
            var defaultCompany = Companies.FirstOrDefault();
            var defaultBranch = Branches.FirstOrDefault();
            var defaultPaymentAccount = PaymentAccounts.FirstOrDefault();

            var paymentNumber = PaymentService.GeneratePaymentNumber(
                defaultCompany?.Id ?? Guid.Empty);

            return new VendorPaymentViewModel
            {
                CompanyId = defaultCompany?.Id ?? Guid.Empty,
                CompanyName = defaultCompany?.LegalName,
                BranchId = defaultBranch?.Id ?? Guid.Empty,
                BranchName = defaultBranch?.BranchName,
                PaymentNumber = paymentNumber,
                PaymentDate = DateTime.Today,
                PaymentStatus = VendorPaymentStatuses.Draft,
                ExchangeRate = 1,
                CurrencyId = Guid.Parse("d1d1d1d1-d1d1-d1d1-d1d1-d1d1d1d1d101"),
                CurrencyCode = "INR",
                CurrencyName = "Indian Rupee",
                PaymentMethod = VendorPaymentMethods.BankTransfer,
                PaymentAccountId = defaultPaymentAccount?.Id ?? Guid.Empty,
                PaymentAccountCode = defaultPaymentAccount?.AccountCode,
                PaymentAccountName = defaultPaymentAccount?.AccountName
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

        private void OnVendorChanged()
        {
            if (Payment.VendorId != Guid.Empty)
            {
                var vendor = Vendors.FirstOrDefault(v => v.Id == Payment.VendorId);
                if (vendor != null)
                {
                    Payment.VendorCode = vendor.VendorCode;
                    Payment.VendorName = vendor.VendorName;
                    Payment.CurrencyId = vendor.DefaultCurrencyId ?? Payment.CurrencyId;
                    Payment.CurrencyCode = vendor.DefaultCurrencyCode ?? Payment.CurrencyCode;
                    Payment.CurrencyName = vendor.DefaultCurrencyName ?? Payment.CurrencyName;

                    // Auto-fill TDS details from vendor if applicable
                    if (vendor.IsTDSApplicable)
                    {
                        Payment.IsTDSApplicable = true;
                        Payment.TDSSectionCodeSnapshot = vendor.TDSSectionCode;
                        Payment.TDSRatePercentSnapshot = vendor.TDSRatePercent ?? 0;
                    }
                }
                LoadBillsForVendor(Payment.VendorId);
                // Clear existing allocations when vendor changes
                Payment.Allocations.Clear();
                Payment.RecalculateAmounts();
                ClearHeaderError("VendorId");
            }
            else
            {
                Payment.VendorId = Guid.Empty;
                Payment.VendorCode = null;
                Payment.VendorName = null;
                VendorBills = new List<VendorBillViewModel>();
                Payment.Allocations.Clear();
            }
        }

        private void OnPaymentMethodChanged(ChangeEventArgs e)
        {
            Payment.PaymentMethod = e.Value?.ToString() ?? VendorPaymentMethods.BankTransfer;

            // Clear instrument fields when method changes
            Payment.PaymentReferenceNumber = null;
            Payment.ReferenceDate = null;
            if (Payment.PaymentMethod != VendorPaymentMethods.Cheque &&
                Payment.PaymentMethod != VendorPaymentMethods.BankTransfer)
            {
                Payment.BankNameSnapshot = null;
            }

            // Auto-select appropriate payment account based on method
            if (Payment.PaymentMethod == VendorPaymentMethods.Cash)
            {
                var cashAccount = PaymentAccounts.FirstOrDefault(a => a.AccountCode == "1100");
                if (cashAccount != null)
                {
                    Payment.PaymentAccountId = cashAccount.Id;
                    Payment.PaymentAccountCode = cashAccount.AccountCode;
                    Payment.PaymentAccountName = cashAccount.AccountName;
                }
            }
            else if (Payment.PaymentMethod == VendorPaymentMethods.BankTransfer ||
                     Payment.PaymentMethod == VendorPaymentMethods.UPI)
            {
                var bankAccount = PaymentAccounts.FirstOrDefault(a => a.AccountCode == "1110");
                if (bankAccount != null)
                {
                    Payment.PaymentAccountId = bankAccount.Id;
                    Payment.PaymentAccountCode = bankAccount.AccountCode;
                    Payment.PaymentAccountName = bankAccount.AccountName;
                }
            }
            else if (Payment.PaymentMethod == VendorPaymentMethods.Gateway)
            {
                var gatewayAccount = PaymentAccounts.FirstOrDefault(a => a.AccountCode == "1150");
                if (gatewayAccount != null)
                {
                    Payment.PaymentAccountId = gatewayAccount.Id;
                    Payment.PaymentAccountCode = gatewayAccount.AccountCode;
                    Payment.PaymentAccountName = gatewayAccount.AccountName;
                }
            }

            ClearHeaderError("PaymentMethod");
        }

        private void OnPaymentAccountChanged(ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var accountId))
            {
                Payment.PaymentAccountId = accountId;
                var account = PaymentAccounts.FirstOrDefault(a => a.Id == accountId);
                if (account != null)
                {
                    Payment.PaymentAccountCode = account.AccountCode;
                    Payment.PaymentAccountName = account.AccountName;
                }
            }
            ClearHeaderError("PaymentAccountId");
        }

        private void OnBankChargesAccountChanged(ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var accountId))
            {
                Payment.BankChargesAccountId = accountId;
                var account = PaymentAccounts.FirstOrDefault(a => a.Id == accountId);
                if (account != null)
                {
                    Payment.BankChargesAccountCode = account.AccountCode;
                    Payment.BankChargesAccountName = account.AccountName;
                }
            }
            else
            {
                Payment.BankChargesAccountId = null;
                Payment.BankChargesAccountCode = null;
                Payment.BankChargesAccountName = null;
            }
        }

        private void OnAmountChanged()
        {
            Payment.RecalculateAmounts();
            ClearHeaderError("PaymentGrossAmount");
        }

        private void OnTDSToggleChanged(ChangeEventArgs e)
        {
            Payment.IsTDSApplicable = (bool)(e.Value ?? false);
            if (!Payment.IsTDSApplicable)
            {
                Payment.TDSSectionCodeSnapshot = null;
                Payment.TDSRatePercentSnapshot = 0;
            }
            Payment.RecalculateAmounts();
        }

        private void OnTDSSectionChanged(ChangeEventArgs e)
        {
            Payment.TDSSectionCodeSnapshot = e.Value?.ToString();

            // Auto-fill default rate based on section
            Payment.TDSRatePercentSnapshot = Payment.TDSSectionCodeSnapshot switch
            {
                "194C" => 2m,
                "194J" => 10m,
                "194H" => 5m,
                "194I" => 10m,
                "194A" => 10m,
                "194Q" => 0.1m,
                _ => 0m
            };

            Payment.RecalculateAmounts();
            ClearHeaderError("TDSSectionCodeSnapshot");
        }

        private void OnTDSRateChanged()
        {
            Payment.RecalculateAmounts();
            ClearHeaderError("TDSRatePercentSnapshot");
        }

        private string GetReferenceLabel()
        {
            return Payment.PaymentMethod switch
            {
                VendorPaymentMethods.BankTransfer => "UTR / Reference No",
                VendorPaymentMethods.UPI => "UPI Transaction ID",
                VendorPaymentMethods.Cheque => "Cheque Number",
                VendorPaymentMethods.Gateway => "Transaction Reference",
                _ => "Reference Number"
            };
        }

        #endregion

        #region Allocations

        private void AddAllocation()
        {
            var allocation = new VendorPaymentAllocationModel
            {
                Id = Guid.NewGuid(),
                VendorPaymentId = Payment.Id,
                AllocationOrder = Payment.Allocations.Count + 1
            };
            Payment.Allocations.Add(allocation);
        }

        private void RemoveAllocation(VendorPaymentAllocationModel allocation)
        {
            var index = Payment.Allocations.IndexOf(allocation);
            Payment.Allocations.Remove(allocation);
            ClearAllocationErrors(index);
            Payment.RecalculateAmounts();
        }

        private void OnBillSelected(VendorPaymentAllocationModel allocation, ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var billId))
            {
                var bill = VendorBills.FirstOrDefault(b => b.Id == billId);
                if (bill != null)
                {
                    allocation.VendorBillId = bill.Id;
                    allocation.BillNumberSnapshot = bill.BillNumber;
                    allocation.BillDateSnapshot = bill.BillDate;
                    allocation.BillDueDateSnapshot = bill.DueDate;
                    allocation.BillOutstandingSnapshot = bill.AmountOutstanding;

                    // Auto-fill allocation amount (full outstanding or remaining payment amount)
                    var remainingPayment = Payment.PaymentGrossAmount - Payment.Allocations.Where(a => a.Id != allocation.Id).Sum(a => a.AllocatedToBillAmount);
                    allocation.AllocatedToBillAmount = Math.Min(bill.AmountOutstanding, remainingPayment > 0 ? remainingPayment : bill.AmountOutstanding);

                    Payment.RecalculateAmounts();
                }

                var allocIndex = Payment.Allocations.IndexOf(allocation);
                ClearAllocationError(allocIndex, "VendorBillId");
            }
        }

        private void OnAllocationAmountChanged(VendorPaymentAllocationModel allocation, ChangeEventArgs e)
        {
            if (decimal.TryParse(e.Value?.ToString(), out var amount))
            {
                allocation.AllocatedToBillAmount = amount;
                Payment.RecalculateAmounts();

                var allocIndex = Payment.Allocations.IndexOf(allocation);
                ClearAllocationError(allocIndex, "AllocatedToBillAmount");
            }
        }

        private List<VendorBillViewModel> GetAvailableBills(VendorPaymentAllocationModel currentAllocation)
        {
            var usedBillIds = Payment.Allocations
                .Where(a => a.Id != currentAllocation.Id)
                .Select(a => a.VendorBillId)
                .ToList();

            var available = VendorBills.Where(b => !usedBillIds.Contains(b.Id)).ToList();

            // Add current bill if already selected
            if (currentAllocation.VendorBillId != Guid.Empty)
            {
                var currentBill = VendorBills.FirstOrDefault(b => b.Id == currentAllocation.VendorBillId);
                if (currentBill != null && !available.Contains(currentBill))
                {
                    available.Insert(0, currentBill);
                }
            }

            return available;
        }

        #endregion

        #region Validation

        private bool ValidateHeader()
        {
            HeaderValidationErrors.Clear();

            if (Payment.BranchId == Guid.Empty)
                HeaderValidationErrors["BranchId"] = "Branch is required.";

            if (Payment.VendorId == Guid.Empty)
                HeaderValidationErrors["VendorId"] = "Vendor is required.";

            if (Payment.PaymentAccountId == Guid.Empty)
                HeaderValidationErrors["PaymentAccountId"] = "Payment Account is required.";

            if (string.IsNullOrWhiteSpace(Payment.PaymentMethod))
                HeaderValidationErrors["PaymentMethod"] = "Payment Method is required.";

            if (Payment.PaymentGrossAmount <= 0)
                HeaderValidationErrors["PaymentGrossAmount"] = "Amount must be greater than zero.";

            if (VendorPaymentMethods.RequiresReference(Payment.PaymentMethod) &&
                string.IsNullOrWhiteSpace(Payment.PaymentReferenceNumber))
            {
                HeaderValidationErrors["PaymentReferenceNumber"] = $"Reference number is required for {VendorPaymentMethods.GetDisplayName(Payment.PaymentMethod)}.";
            }

            if (Payment.IsTDSApplicable)
            {
                if (string.IsNullOrWhiteSpace(Payment.TDSSectionCodeSnapshot))
                    HeaderValidationErrors["TDSSectionCodeSnapshot"] = "TDS Section is required when TDS is applicable.";

                if (Payment.TDSRatePercentSnapshot <= 0 || Payment.TDSRatePercentSnapshot > 100)
                    HeaderValidationErrors["TDSRatePercentSnapshot"] = "TDS Rate must be between 0 and 100.";
            }

            return !HeaderValidationErrors.Any();
        }

        private bool ValidateAllAllocations()
        {
            AllocationValidationErrors.Clear();
            bool isValid = true;

            for (int i = 0; i < Payment.Allocations.Count; i++)
            {
                var alloc = Payment.Allocations[i];

                if (alloc.VendorBillId == Guid.Empty)
                {
                    AllocationValidationErrors[$"{i}_VendorBillId"] = "Bill is required.";
                    isValid = false;
                }

                if (alloc.AllocatedToBillAmount <= 0)
                {
                    AllocationValidationErrors[$"{i}_AllocatedToBillAmount"] = "Amount must be greater than 0.";
                    isValid = false;
                }
                else if (alloc.AllocatedToBillAmount > alloc.BillOutstandingSnapshot)
                {
                    AllocationValidationErrors[$"{i}_AllocatedToBillAmount"] = "Cannot exceed outstanding.";
                    isValid = false;
                }
            }

            // Validate total allocations
            if (Payment.AllocatedAmount > Payment.PaymentGrossAmount)
            {
                isValid = false;
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

        private async Task HandleInvalidSubmit()
        {
            // Ensure validation errors are rendered first
            StateHasChanged();
            await Task.Delay(100); // Small delay to ensure DOM is updated
            await ScrollToFirstValidationError();
        }

        private async Task HandleSubmit()
        {
            // Get narration from rich text editor
            if (narrationEditor != null)
                Payment.PaymentNarration = await narrationEditor.GetHtmlAsync();

            if (!ValidateHeader())
            {
                StateHasChanged();
                await Task.Delay(100); // Small delay to ensure DOM is updated
                await ScrollToFirstValidationError();
                return;
            }

            // Validate allocations only if there are any
            if (Payment.Allocations.Any() && !ValidateAllAllocations())
            {
                StateHasChanged();
                await Task.Delay(100); // Small delay to ensure DOM is updated
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
                    ToastService.ShowSuccess($"Payment '{Payment.PaymentNumber}' updated successfully.", "Updated");
                    Nav.NavigateTo("/vendor-payments");
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
                    ToastService.ShowSuccess($"Payment '{Payment.PaymentNumber}' created successfully.", "Created");
                    Nav.NavigateTo("/vendor-payments");
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
            if (!ValidateHeader())
            {
                StateHasChanged();
                await Task.Delay(100); // Small delay to ensure DOM is updated
                await ScrollToFirstValidationError();
                return;
            }

            if (Payment.Allocations.Any() && !ValidateAllAllocations())
            {
                StateHasChanged();
                await Task.Delay(100); // Small delay to ensure DOM is updated
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
                ToastService.ShowSuccess($"Payment '{Payment.PaymentNumber}' submitted for approval.", "Submitted");
                Nav.NavigateTo("/vendor-payments");
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
                ToastService.ShowSuccess($"Payment '{Payment.PaymentNumber}' posted successfully.", "Posted");
                Nav.NavigateTo("/vendor-payments");
            }
            else
            {
                ToastService.ShowError(result.Message, "Error");
            }

            await Task.CompletedTask;
        }

        private void Cancel()
        {
            Nav.NavigateTo("/vendor-payments");
        }

        #endregion

        #region UI Helpers

        private string GetStatusBadge(string status) => status switch
        {
            VendorPaymentStatuses.Draft => "bg-secondary",
            VendorPaymentStatuses.Submitted => "bg-info",
            VendorPaymentStatuses.Approved => "bg-primary",
            VendorPaymentStatuses.Posted => "bg-success",
            VendorPaymentStatuses.Reversed => "bg-danger",
            VendorPaymentStatuses.Cancelled => "bg-warning text-dark",
            _ => "bg-secondary"
        };

        #endregion
    }
}
