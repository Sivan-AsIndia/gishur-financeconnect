using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.AR.CustomerInvoice
{
    public partial class CreateCustomerInvoice
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] CustomerInvoiceService InvoiceService { get; set; } = default!;
        [Inject] CustomerService CustomerService { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] COADataService COADataService { get; set; } = default!;
        [Inject] BranchService BranchService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] IJSRuntime JSRuntime { get; set; } = default!;

        [Parameter] public Guid? Id { get; set; }

        private bool isInitialized = false;
        private CustomerInvoiceViewModel Invoice = new();

        // Dropdown data
        private List<CompanyModel> Companies = new();
        private List<BranchModel> Branches = new();
        private List<CustomerViewModel> Customers = new();
        private List<PaymentTermViewModel> PaymentTerms = new();
        private List<StateProvinceModel> States = new();
        private List<AccountViewModel> RevenueAccounts = new();

        // Rich Text Editor for narration
        private FinanceConnect.Client.Shared.RichTextEditor? narrationEditor;

        // Validation errors dictionary: Key = "lineIndex_fieldName", Value = error message
        private Dictionary<string, string> LineValidationErrors = new();
        
        // Header validation errors
        private Dictionary<string, string> HeaderValidationErrors = new();

        private bool IsEdit => Id.HasValue;
        private bool IsReadOnly => Invoice.InvoiceStatus != InvoiceStatuses.Draft;

        protected override async Task OnInitializedAsync()
        {
            // Load master data
            await LoadMasterData();

            if (IsEdit)
            {
                var existing = InvoiceService.GetById(Id!.Value);
                if (existing != null)
                {
                    Invoice = existing;
                }
                else
                {
                    ToastService.ShowError("Invoice not found.", "Error");
                    Nav.NavigateTo("/customer-invoices");
                    return;
                }
            }
            else
            {
                Invoice = CreateNewInvoice();
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

            // Load customers
            Customers = CustomerService.GetAll().Where(c => c.CustomerStatus == CustomerStatuses.Active).ToList();

            // Load payment terms from seed data
            PaymentTerms = PaymentTermSeedData.GetSeedData();

            // Load states
            States = MasterDataService.GetAllStateProvinces().Where(s => s.IsActive).ToList();

            // Load revenue accounts from COADataService (Income/Revenue accounts)
            var allAccounts = COADataService.GetAllAccounts();
            RevenueAccounts = allAccounts
                .Where(a => a.AccountNature == AccountNatures.Income && a.IsPostable && a.IsActive)
                .ToList();

            await Task.CompletedTask;
        }

        private CustomerInvoiceViewModel CreateNewInvoice()
        {
            var defaultCompany = Companies.FirstOrDefault();
            var defaultBranch = Branches.FirstOrDefault();
            var defaultPaymentTerm = PaymentTerms.FirstOrDefault(t => t.Id == MasterDataIds.PaymentTerms.Net30)
                                     ?? PaymentTerms.FirstOrDefault();

            var companyId = defaultCompany?.Id ?? Guid.Empty;
            var invoiceNumber = InvoiceService.GenerateInvoiceNumber(companyId, InvoiceTypes.Standard);

            return new CustomerInvoiceViewModel
            {
                CompanyId = companyId,
                CompanyName = defaultCompany?.LegalName,
                BranchId = defaultBranch?.Id ?? Guid.Empty,
                BranchName = defaultBranch?.BranchName,
                InvoiceNumber = invoiceNumber,
                InvoiceType = "",
                InvoiceDate = DateTime.Today,
                DueDate = DateTime.Today.AddDays(defaultPaymentTerm?.Days ?? 30),
                InvoiceStatus = InvoiceStatuses.Draft,
                ApprovalStatus = ApprovalStatuses.NotRequired,
                ExchangeRate = 1,
                SupplyType = "",
                EInvoiceStatus = EInvoiceStatuses.NotApplicable,
                CurrencyId = MasterDataIds.Currencies.INR,
                CurrencyCode = "INR",
                CurrencyName = "Indian Rupee",
                PaymentTermId = defaultPaymentTerm?.Id ?? Guid.Empty,
                PaymentTermName = defaultPaymentTerm?.Name,
                PaymentTermDays = defaultPaymentTerm?.Days
            };
        }

        private void OnCompanyChanged(ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var companyId))
            {
                Invoice.CompanyId = companyId;
                var company = Companies.FirstOrDefault(c => c.Id == companyId);
                Invoice.CompanyName = company?.LegalName;
            }
            ClearHeaderError("CompanyId");
        }

        private void OnCustomerChanged()
        {
            if (Invoice.CustomerId != Guid.Empty)
            {
                var customer = Customers.FirstOrDefault(c => c.Id == Invoice.CustomerId);
                if (customer != null)
                {
                    Invoice.CustomerCode = customer.CustomerCode;
                    Invoice.CustomerName = customer.CustomerName;
                    Invoice.CurrencyId = customer.DefaultCurrencyId ?? Invoice.CurrencyId;
                    Invoice.CurrencyCode = customer.DefaultCurrencyCode ?? Invoice.CurrencyCode;
                    Invoice.CurrencyName = customer.DefaultCurrencyName ?? Invoice.CurrencyName;

                    // Set payment terms from customer defaults
                    if (customer.PaymentTermId.HasValue)
                    {
                        Invoice.PaymentTermId = customer.PaymentTermId.Value;
                        Invoice.PaymentTermName = customer.PaymentTermName;
                        var term = PaymentTerms.FirstOrDefault(t => t.Id == customer.PaymentTermId);
                        Invoice.PaymentTermDays = term?.Days;
                        Invoice.CalculateDueDate();
                    }
                }
                ClearHeaderError("CustomerId");
            }
            else
            {
                Invoice.CustomerId = Guid.Empty;
                Invoice.CustomerCode = null;
                Invoice.CustomerName = null;
            }
        }

        private void OnPaymentTermChanged(ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var termId))
            {
                Invoice.PaymentTermId = termId;
                var term = PaymentTerms.FirstOrDefault(t => t.Id == termId);
                Invoice.PaymentTermName = term?.Name;
                Invoice.PaymentTermDays = term?.Days;
                Invoice.CalculateDueDate();
            }
        }

        private void OnInvoiceDateChanged()
        {
            Invoice.CalculateDueDate();
        }

        #region Header Validation

        private void ClearHeaderError(string fieldName)
        {
            HeaderValidationErrors.Remove(fieldName);
        }

        private void SetHeaderError(string fieldName, string errorMessage)
        {
            HeaderValidationErrors[fieldName] = errorMessage;
        }

        private bool HasHeaderError(string fieldName)
        {
            return HeaderValidationErrors.ContainsKey(fieldName);
        }

        private string GetHeaderError(string fieldName)
        {
            return HeaderValidationErrors.TryGetValue(fieldName, out var error) ? error : string.Empty;
        }

        private string GetHeaderValidationClass(string fieldName)
        {
            return HasHeaderError(fieldName) ? "is-invalid" : "";
        }

        private bool ValidateHeader()
        {
            HeaderValidationErrors.Clear();
            var isValid = true;

            // Validate Invoice Number
            if (string.IsNullOrWhiteSpace(Invoice.InvoiceNumber))
            {
                SetHeaderError("InvoiceNumber", "Invoice Number is required");
                isValid = false;
            }

            // Validate Invoice Type
            if (string.IsNullOrWhiteSpace(Invoice.InvoiceType))
            {
                SetHeaderError("InvoiceType", "Invoice Type is required");
                isValid = false;
            }

            // Validate Invoice Date
            if (Invoice.InvoiceDate == default)
            {
                SetHeaderError("InvoiceDate", "Invoice Date is required");
                isValid = false;
            }

            // Validate Customer
            if (Invoice.CustomerId == Guid.Empty)
            {
                SetHeaderError("CustomerId", "Customer is required");
                isValid = false;
            }

            // Validate Company
            if (Invoice.CompanyId == Guid.Empty)
            {
                SetHeaderError("CompanyId", "Company is required");
                isValid = false;
            }

            // Validate Branch
            if (Invoice.BranchId == Guid.Empty)
            {
                SetHeaderError("BranchId", "Branch is required");
                isValid = false;
            }

            // Validate Payment Terms
            if (Invoice.PaymentTermId == Guid.Empty)
            {
                SetHeaderError("PaymentTermId", "Payment Terms is required");
                isValid = false;
            }

            // Validate Currency
            if (Invoice.CurrencyId == Guid.Empty)
            {
                SetHeaderError("CurrencyId", "Currency is required");
                isValid = false;
            }

            // Validate Due Date
            if (Invoice.DueDate < Invoice.InvoiceDate)
            {
                SetHeaderError("DueDate", "Due date cannot be before invoice date");
                isValid = false;
            }

            // Validate Posting Date (conditional - if set, must be >= InvoiceDate)
            if (Invoice.PostingDate.HasValue && Invoice.PostingDate.Value < Invoice.InvoiceDate)
            {
                SetHeaderError("PostingDate", "Posting date cannot be before invoice date");
                isValid = false;
            }

            return isValid;
        }

        #endregion

        #region Line Validation

        private void ValidateLine(CustomerInvoiceLineViewModel line)
        {
            var lineIndex = Invoice.Lines.IndexOf(line);
            if (lineIndex < 0) return;

            // Clear existing errors for this line
            ClearLineErrors(lineIndex);

            // Validate Line Type
            if (string.IsNullOrWhiteSpace(line.LineType))
            {
                SetLineError(lineIndex, "LineType", "Line Type is required");
            }

            // Validate Description
            if (string.IsNullOrWhiteSpace(line.Description))
            {
                SetLineError(lineIndex, "Description", "Description is required");
            }

            // Validate Quantity
            if (line.Quantity <= 0)
            {
                SetLineError(lineIndex, "Quantity", "Qty must be > 0");
            }

            // Validate Unit Price
            if (line.UnitPrice < 0)
            {
                SetLineError(lineIndex, "UnitPrice", "Price must be ≥ 0");
            }

            // Validate Discount Percent
            if (line.DiscountPercent < 0 || line.DiscountPercent > 100)
            {
                SetLineError(lineIndex, "DiscountPercent", "0-100 only");
            }

            // Validate Tax Rate Percent
            if (line.TaxRatePercent < 0 || line.TaxRatePercent > 100)
            {
                SetLineError(lineIndex, "TaxRatePercent", "0-100 only");
            }

            // Validate Revenue Account
            if (line.RevenueAccountId == Guid.Empty)
            {
                SetLineError(lineIndex, "RevenueAccountId", "Account required");
            }

            StateHasChanged();
        }

        private void ClearLineErrors(int lineIndex)
        {
            var keysToRemove = LineValidationErrors.Keys.Where(k => k.StartsWith($"{lineIndex}_")).ToList();
            foreach (var key in keysToRemove)
            {
                LineValidationErrors.Remove(key);
            }
        }

        private void SetLineError(int lineIndex, string fieldName, string errorMessage)
        {
            var key = $"{lineIndex}_{fieldName}";
            LineValidationErrors[key] = errorMessage;
        }

        private bool HasValidationError(int lineIndex, string fieldName)
        {
            var key = $"{lineIndex}_{fieldName}";
            return LineValidationErrors.ContainsKey(key);
        }

        private string GetValidationError(int lineIndex, string fieldName)
        {
            var key = $"{lineIndex}_{fieldName}";
            return LineValidationErrors.TryGetValue(key, out var error) ? error : string.Empty;
        }

        private string GetValidationClass(int lineIndex, string fieldName)
        {
            var hasError = HasValidationError(lineIndex, fieldName);
            return hasError ? "is-invalid" : "";
        }

        private bool ValidateAllLines()
        {
            LineValidationErrors.Clear();
            var isValid = true;

            for (int i = 0; i < Invoice.Lines.Count; i++)
            {
                var line = Invoice.Lines[i];

                // Validate Line Type
                if (string.IsNullOrWhiteSpace(line.LineType))
                {
                    SetLineError(i, "LineType", "Line Type is required");
                    isValid = false;
                }

                // Validate Description
                if (string.IsNullOrWhiteSpace(line.Description))
                {
                    SetLineError(i, "Description", "Description is required");
                    isValid = false;
                }

                // Validate Quantity
                if (line.Quantity <= 0)
                {
                    SetLineError(i, "Quantity", "Qty must be > 0");
                    isValid = false;
                }

                // Validate Unit Price
                if (line.UnitPrice < 0)
                {
                    SetLineError(i, "UnitPrice", "Price must be ≥ 0");
                    isValid = false;
                }

                // Validate Discount Percent
                if (line.DiscountPercent < 0 || line.DiscountPercent > 100)
                {
                    SetLineError(i, "DiscountPercent", "0-100 only");
                    isValid = false;
                }

                // Validate Tax Rate Percent
                if (line.TaxRatePercent < 0 || line.TaxRatePercent > 100)
                {
                    SetLineError(i, "TaxRatePercent", "0-100 only");
                    isValid = false;
                }

                // Validate Revenue Account
                if (line.RevenueAccountId == Guid.Empty)
                {
                    SetLineError(i, "RevenueAccountId", "Account required");
                    isValid = false;
                }
            }

            return isValid;
        }

        #endregion

        #region Line Management

        private void AddLine()
        {
            var maxLineNumber = Invoice.Lines.Any() ? Invoice.Lines.Max(l => l.LineNumber) : 0;
            var newLine = new CustomerInvoiceLineViewModel
            {
                Id = Guid.NewGuid(),
                CustomerInvoiceId = Invoice.Id,
                LineNumber = maxLineNumber + 10,
                LineType = "",
                Quantity = 1,
                UnitPrice = 0,
                TaxRatePercent = 18, // Default GST rate
                RevenueAccountId = Guid.Empty
            };

            Invoice.Lines.Add(newLine);
            RecalculateTotals();
        }

        private void DuplicateLine(CustomerInvoiceLineViewModel line)
        {
            var maxLineNumber = Invoice.Lines.Max(l => l.LineNumber);
            var newLine = new CustomerInvoiceLineViewModel
            {
                Id = Guid.NewGuid(),
                CustomerInvoiceId = Invoice.Id,
                LineNumber = maxLineNumber + 10,
                LineType = line.LineType,
                Description = line.Description,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice,
                DiscountPercent = line.DiscountPercent,
                TaxRatePercent = line.TaxRatePercent,
                RevenueAccountId = line.RevenueAccountId
            };

            newLine.RecalculateAmounts();
            Invoice.Lines.Add(newLine);
            RecalculateTotals();
        }

        private void DeleteLine(CustomerInvoiceLineViewModel line)
        {
            var lineIndex = Invoice.Lines.IndexOf(line);
            ClearLineErrors(lineIndex);
            Invoice.Lines.Remove(line);
            RecalculateTotals();
        }

        private void OnLineTypeChanged(CustomerInvoiceLineViewModel line, ChangeEventArgs e)
        {
            line.LineType = e.Value?.ToString() ?? "";
            ValidateLine(line);
        }

        private void OnDescriptionChanged(CustomerInvoiceLineViewModel line, ChangeEventArgs e)
        {
            line.Description = e.Value?.ToString() ?? string.Empty;
            ValidateLine(line);
        }

        private void OnRevenueAccountChanged(CustomerInvoiceLineViewModel line, ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var accountId))
            {
                line.RevenueAccountId = accountId;
            }
            else
            {
                line.RevenueAccountId = Guid.Empty;
            }
            ValidateLine(line);
        }

        private void OnQuantityChanged(CustomerInvoiceLineViewModel line, ChangeEventArgs e)
        {
            var value = e.Value?.ToString() ?? "0";
            if (decimal.TryParse(value, out var qty))
            {
                line.Quantity = qty;
            }
            else
            {
                line.Quantity = 0;
            }
            line.RecalculateAmounts();
            RecalculateTotals();
            ValidateLine(line);
        }

        private void OnUnitPriceChanged(CustomerInvoiceLineViewModel line, ChangeEventArgs e)
        {
            var value = e.Value?.ToString() ?? "0";
            if (decimal.TryParse(value, out var price))
            {
                line.UnitPrice = price;
            }
            else
            {
                line.UnitPrice = 0;
            }
            line.RecalculateAmounts();
            RecalculateTotals();
            ValidateLine(line);
        }

        private void OnDiscountChanged(CustomerInvoiceLineViewModel line, ChangeEventArgs e)
        {
            var value = e.Value?.ToString() ?? "0";
            if (decimal.TryParse(value, out var discount))
            {
                line.DiscountPercent = discount;
            }
            else
            {
                line.DiscountPercent = 0;
            }
            line.RecalculateAmounts();
            RecalculateTotals();
            ValidateLine(line);
        }

        private void OnTaxRateChanged(CustomerInvoiceLineViewModel line, ChangeEventArgs e)
        {
            var value = e.Value?.ToString() ?? "0";
            if (decimal.TryParse(value, out var rate))
            {
                line.TaxRatePercent = rate;
            }
            else
            {
                line.TaxRatePercent = 0;
            }
            line.RecalculateAmounts();
            RecalculateTotals();
            ValidateLine(line);
        }

        private void RecalculateTotals()
        {
            Invoice.RecalculateTotals();
            StateHasChanged();
        }

        #endregion

        #region Actions

        private async Task ScrollToFirstValidationError()
        {
            await Task.Delay(50); // Small delay to ensure DOM is updated
            await JSRuntime.InvokeVoidAsync("scrollToFirstValidationError");
        }

        private async Task HandleSubmit()
        {
            // This method is called on form submit - just call SaveInvoice
            await SaveInvoice();
        }

        private async Task SaveInvoice()
        {
            // Get HTML content from rich text editor
            if (narrationEditor != null)
            {
                Invoice.InvoiceNarration = await narrationEditor.GetHtmlAsync();
            }

            // Validate header fields
            if (!ValidateHeader())
            {
                StateHasChanged();
                await ScrollToFirstValidationError();
                return;
            }

            // Validate lines - if no lines exist, add one and show validation errors
            if (!Invoice.Lines.Any())
            {
                AddLine();
                ValidateAllLines();
                StateHasChanged();
                await ScrollToFirstValidationError();
                return;
            }

            // Validate all lines
            if (!ValidateAllLines())
            {
                StateHasChanged();
                await ScrollToFirstValidationError();
                return;
            }

            // Set revenue account names
            foreach (var line in Invoice.Lines)
            {
                var account = RevenueAccounts.FirstOrDefault(a => a.Id == line.RevenueAccountId);
                line.RevenueAccountCode = account?.AccountCode;
                line.RevenueAccountName = account?.AccountName;
            }

            Invoice.RecalculateTotals();

            if (IsEdit)
            {
                Invoice.UpdatedAt = DateTime.Now;
                Invoice.UpdatedBy = "Current User";
                var result = InvoiceService.Update(Invoice);
                if (result.Success)
                {
                    ToastService.ShowSuccess($"Invoice '{Invoice.InvoiceNumber}' updated successfully.", "Updated");
                    Nav.NavigateTo("/customer-invoices");
                }
                else
                {
                    ToastService.ShowError(result.Message, "Error");
                }
            }
            else
            {
                Invoice.CreatedAt = DateTime.Now;
                Invoice.CreatedBy = "Current User";
                var result = InvoiceService.Add(Invoice);
                if (result.Success)
                {
                    ToastService.ShowSuccess($"Invoice '{Invoice.InvoiceNumber}' created successfully.", "Created");
                    Nav.NavigateTo("/customer-invoices");
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
            // Get HTML content from rich text editor
            if (narrationEditor != null)
            {
                Invoice.InvoiceNarration = await narrationEditor.GetHtmlAsync();
            }

            // Validate header fields
            if (!ValidateHeader())
            {
                StateHasChanged();
                await ScrollToFirstValidationError();
                return;
            }

            // Validate lines - if no lines exist, add one and show validation errors
            if (!Invoice.Lines.Any())
            {
                AddLine();
                ValidateAllLines();
                StateHasChanged();
                await ScrollToFirstValidationError();
                return;
            }

            // Validate all lines before submission
            if (!ValidateAllLines())
            {
                StateHasChanged();
                await ScrollToFirstValidationError();
                return;
            }

            // Set revenue account names
            foreach (var line in Invoice.Lines)
            {
                var account = RevenueAccounts.FirstOrDefault(a => a.Id == line.RevenueAccountId);
                line.RevenueAccountCode = account?.AccountCode;
                line.RevenueAccountName = account?.AccountName;
            }

            Invoice.RecalculateTotals();

            // For new invoices, save first then submit
            if (!IsEdit)
            {
                Invoice.CreatedAt = DateTime.Now;
                Invoice.CreatedBy = "Current User";
                var addResult = InvoiceService.Add(Invoice);
                if (!addResult.Success)
                {
                    ToastService.ShowError(addResult.Message, "Error");
                    return;
                }
                // Get the saved invoice with the new ID
                var savedInvoice = InvoiceService.GetAll()
                    .FirstOrDefault(i => i.InvoiceNumber == Invoice.InvoiceNumber);
                if (savedInvoice != null)
                {
                    Invoice = savedInvoice;
                }
            }

            var result = InvoiceService.Submit(Invoice.Id, "Current User");
            if (result.Success)
            {
                Invoice.InvoiceStatus = InvoiceStatuses.Submitted;
                ToastService.ShowSuccess("Invoice submitted for approval.", "Submitted");
                Nav.NavigateTo("/customer-invoices");
            }
            else
            {
                ToastService.ShowError(result.Message, "Error");
            }

            await Task.CompletedTask;
        }

        private void PostInvoice()
        {
            Invoice.PostingDate = DateTime.Today;
            var result = InvoiceService.Post(
                Invoice.Id,
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                "Current User");
            if (result.Success)
            {
                Invoice.InvoiceStatus = InvoiceStatuses.Posted;
                ToastService.ShowSuccess("Invoice posted successfully.", "Posted");
                StateHasChanged();
            }
            else
            {
                ToastService.ShowError(result.Message, "Error");
            }
        }

        private void Cancel()
        {
            Nav.NavigateTo("/customer-invoices");
        }

        #endregion

        #region Helper Methods

        private string GetStatusBadge(string status) => status switch
        {
            InvoiceStatuses.Draft => "bg-secondary",
            InvoiceStatuses.Submitted => "bg-info",
            InvoiceStatuses.Approved => "bg-primary",
            InvoiceStatuses.Posted => "bg-success",
            InvoiceStatuses.PartiallyPaid => "bg-warning",
            InvoiceStatuses.Paid => "bg-success",
            InvoiceStatuses.Cancelled => "bg-danger",
            InvoiceStatuses.Voided => "bg-dark",
            _ => "bg-secondary"
        };

        private string GetEInvoiceBadge(string status) => status switch
        {
            EInvoiceStatuses.NotApplicable => "bg-secondary",
            EInvoiceStatuses.Pending => "bg-warning",
            EInvoiceStatuses.Generated => "bg-success",
            EInvoiceStatuses.Failed => "bg-danger",
            _ => "bg-secondary"
        };

        private string GetApprovalStatusBadge(string status) => status switch
        {
            ApprovalStatuses.NotRequired => "bg-secondary",
            ApprovalStatuses.Pending => "bg-warning text-dark",
            ApprovalStatuses.Approved => "bg-success",
            ApprovalStatuses.Rejected => "bg-danger",
            _ => "bg-secondary"
        };

        private string GetApprovalStatusDisplayName(string status) => status switch
        {
            ApprovalStatuses.NotRequired => "Not Required",
            ApprovalStatuses.Pending => "Pending",
            ApprovalStatuses.Approved => "Approved",
            ApprovalStatuses.Rejected => "Rejected",
            _ => status
        };

        #endregion
    }
}
