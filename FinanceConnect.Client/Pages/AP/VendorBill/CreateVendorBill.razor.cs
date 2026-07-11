using FinanceConnect.Client.Data;
using FinanceConnect.Client.Services;
using FinanceConnect.Client.Shared;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Linq.Expressions;
using System.Reflection;

namespace FinanceConnect.Client.Pages.AP.VendorBill
{
    public partial class CreateVendorBill
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] VendorBillService BillService { get; set; } = default!;
        [Inject] VendorService VendorService { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] BranchService BranchService { get; set; } = default!;
        [Inject] COADataService COADataService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] IJSRuntime JSRuntime { get; set; } = default!;

        [Parameter] public Guid? Id { get; set; }

        private bool isInitialized = false;
        private bool isLoading = false;
        private bool isSaving = false;
        private bool formSubmitted = false;
        private string? vendorInvoiceError = null;
        private RichTextEditor? narrationEditor;
        private VendorBillViewModel Bill = new();
        
        // Selected IDs for binding
        private Guid SelectedVendorId { get; set; }
        private Guid SelectedPaymentTermId { get; set; }

        // Dropdown data
        private List<CompanyModel> Companies = new();
        private List<BranchModel> Branches = new();
        private List<VendorViewModel> Vendors = new();
        private List<PaymentTermViewModel> PaymentTerms = new();
        private List<AccountViewModel> ExpenseAccounts = new();
        private List<StateProvinceModel> States = new();

        // Validation errors dictionary: Key = "lineIndex_fieldName", Value = error message
        private Dictionary<string, string> LineValidationErrors = new();
        
        // Header validation errors
        private Dictionary<string, string> HeaderValidationErrors = new();
        private EditContext _editContext;
        private bool IsEdit => Id.HasValue;
        private bool IsEditMode => IsEdit;
        private bool IsReadOnly => Bill.BillStatus != VendorBillStatuses.Draft;

        protected override async Task OnInitializedAsync()
        {
            // Load master data
            await LoadMasterData();

            if (IsEdit)
            {
                var existing = BillService.GetById(Id!.Value);
                if (existing != null)
                {
                    Bill = existing;
                    SelectedVendorId = Bill.VendorId.Value;
                    SelectedPaymentTermId = Bill.PaymentTermId;
                }
                else
                {
                    ToastService.ShowError("Bill not found.", "Error");
                    Nav.NavigateTo("/vendor-bills");
                    return;
                }
            }
            else
            {
                Bill = CreateNewBill();
                SelectedPaymentTermId = Bill.PaymentTermId;
            }
            _editContext = new EditContext(Bill);
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

            // Load vendors (active only)
            Vendors = VendorService.GetAll().Where(v => v.VendorStatus == VendorStatuses.Active).ToList();

            // Load payment terms from PaymentTermSeedData
            PaymentTerms = PaymentTermSeedData.GetSeedData();

            // Load expense/asset accounts from COADataService
            ExpenseAccounts = COADataService.GetAllAccounts();


            await Task.CompletedTask;
        }

        private VendorBillViewModel CreateNewBill()
        {
            var defaultCompany = Companies.FirstOrDefault();
            var defaultBranch = Branches.FirstOrDefault();
            var defaultPaymentTerm = PaymentTerms.FirstOrDefault(t => t.Name == "Net 30 Days") ?? PaymentTerms.FirstOrDefault();

            var billNumber = BillService.GenerateBillNumber(
                defaultCompany?.Id ?? Guid.Empty,
                BillTypes.GoodsPurchase);

            return new VendorBillViewModel
            {
                CompanyId = defaultCompany?.Id ?? Guid.Empty,
                CompanyName = defaultCompany?.LegalName,
                BranchId = defaultBranch?.Id ?? Guid.Empty,
                BranchName = defaultBranch?.BranchName,
                BillNumber = billNumber,
                BillDate = DateTime.Today,
                VendorInvoiceDate = DateTime.Today,
                DueDate = DateTime.Today.AddDays(defaultPaymentTerm?.Days ?? 30),
                BillStatus = VendorBillStatuses.Draft,
                ExchangeRate = 1,
                CurrencyId = Guid.Parse("a0a0a0a0-a0a0-a0a0-a0a0-a0a0a0a0a001"),
                CurrencyCode = "INR",
                CurrencyName = "Indian Rupee",
                PaymentTermId = defaultPaymentTerm?.Id ?? Guid.Empty,
                PaymentTermName = defaultPaymentTerm?.Name,
                PaymentTermDays = defaultPaymentTerm?.Days,
                IsGSTApplicable = true
            };
        }

        private void OnCompanyChanged(ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var companyId))
            {
                Bill.CompanyId = companyId;
                var company = Companies.FirstOrDefault(c => c.Id == companyId);
                Bill.CompanyName = company?.LegalName;
            }
            ClearHeaderError("CompanyId");
        }

        private void OnVendorChanged()
        {
            var vendorId = SelectedVendorId;
            Bill.VendorId = vendorId;
            var vendor = Vendors.FirstOrDefault(v => v.Id == vendorId);
            if (vendor != null)
            {
                Bill.VendorCode = vendor.VendorCode;
                Bill.VendorName = vendor.VendorName;
                Bill.CurrencyId = vendor.DefaultCurrencyId ?? Bill.CurrencyId;
                Bill.CurrencyCode = vendor.DefaultCurrencyCode ?? Bill.CurrencyCode;
                Bill.CurrencyName = vendor.DefaultCurrencyName ?? Bill.CurrencyName;
                Bill.VendorGSTINSnapshot = vendor.GSTIN;

                // Set payment terms from vendor defaults if available
                if (vendor.PaymentTermsId.HasValue)
                {
                    Bill.PaymentTermId = vendor.PaymentTermsId.Value;
                    SelectedPaymentTermId = vendor.PaymentTermsId.Value;
                    Bill.PaymentTermName = vendor.PaymentTermsName;
                    var term = PaymentTerms.FirstOrDefault(t => t.Id == vendor.PaymentTermsId);
                    Bill.PaymentTermDays = term?.Days;
                    Bill.CalculateDueDate();
                }

                if (vendor.StateId.HasValue)
                {
                    Bill.PlaceOfSupplyStateId = vendor.StateId;
                }
                // Load Indian states for Place of Supply
                States = MasterDataService.GetAllStateProvinces()
                    .Where(s => s.CountryId == vendor.CountryId) // India
                    .OrderBy(s => s.StateProvinceName).ToList();
            }
            else
            {
                Bill.VendorId = Guid.Empty;
                Bill.VendorCode = null;
                Bill.VendorName = null;
            }
            ClearHeaderError("VendorId");
        }

        private void OnPaymentTermChanged()
        {
            var termId = SelectedPaymentTermId;
            Bill.PaymentTermId = termId;
            var term = PaymentTerms.FirstOrDefault(t => t.Id == termId);
            Bill.PaymentTermName = term?.Name;
            Bill.PaymentTermDays = term?.Days;
            Bill.CalculateDueDate();
        }

        private void OnBillDateChanged()
        {
            // Bill date changed - no automatic recalculation of due date
            // Due date is based on VendorInvoiceDate, not BillDate
        }

        private void OnVendorInvoiceDateChanged()
        {
            Bill.CalculateDueDate();
        }

        private void OnVendorInvoiceNumberChanged()
        {
            ClearHeaderError("VendorInvoiceNumber");
            
            // Check for duplicate vendor invoice number (critical anti-fraud check)
            if (!string.IsNullOrWhiteSpace(Bill.VendorInvoiceNumber) && Bill.VendorId != Guid.Empty)
            {
                var isDuplicate = BillService.VendorInvoiceNumberExists(
                    Bill.CompanyId,
                    Bill.VendorId.Value,
                    Bill.VendorInvoiceNumber,
                    IsEdit ? Bill.Id : null);

                if (isDuplicate)
                {
                    SetHeaderError("VendorInvoiceNumber", "This vendor invoice number already exists for this vendor");
                }
            }
        }

        private string GetValidationClass(Expression<Func<object>> field)
        {
            if (_editContext == null)
                return string.Empty;

            var fieldIdentifier = FieldIdentifier.Create(field);

            var hasError = _editContext.GetValidationMessages(fieldIdentifier).Any();
            var isModified = _editContext.IsModified(fieldIdentifier);

            if (hasError)
                return "is-invalid";

            if (isModified)
                return "is-valid";

            return string.Empty;
        }


        private void CheckDuplicateVendorInvoice()
        {
            vendorInvoiceError = null;

            // guard clauses
            if (string.IsNullOrWhiteSpace(Bill.VendorInvoiceNumber))
                return;

            if (!Bill.VendorId.HasValue)
                return;

            var isDuplicate = BillService.VendorInvoiceNumberExists(
                Bill.CompanyId,
                Bill.VendorId.Value,
                Bill.VendorInvoiceNumber,
                IsEdit ? Bill.Id : null);

            if (isDuplicate)
            {
                vendorInvoiceError = "This vendor invoice number already exists for this vendor";
            }

            StateHasChanged();
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

            // Validate Bill Number
            if (string.IsNullOrWhiteSpace(Bill.BillNumber))
            {
                SetHeaderError("BillNumber", "Bill Number is required");
                isValid = false;
            }

            // Validate Vendor Invoice Number (critical)
            if (string.IsNullOrWhiteSpace(Bill.VendorInvoiceNumber))
            {
                SetHeaderError("VendorInvoiceNumber", "Vendor Invoice Number is required");
                isValid = false;
            }
            else if (Bill.VendorId != Guid.Empty)
            {
                // Check for duplicate
                var isDuplicate = BillService.VendorInvoiceNumberExists(
                    Bill.CompanyId,
                    Bill.VendorId.Value,
                    Bill.VendorInvoiceNumber,
                    IsEdit ? Bill.Id : null);

                if (isDuplicate)
                {
                    SetHeaderError("VendorInvoiceNumber", "This vendor invoice number already exists for this vendor");
                    isValid = false;
                }
            }

            // Validate Vendor
            if (Bill.VendorId == Guid.Empty)
            {
                SetHeaderError("VendorId", "Vendor is required");
                isValid = false;
            }

            // Validate Company
            if (Bill.CompanyId == Guid.Empty)
            {
                SetHeaderError("CompanyId", "Company is required");
                isValid = false;
            }

            // Validate Branch
            if (Bill.BranchId == Guid.Empty)
            {
                SetHeaderError("BranchId", "Branch is required");
                isValid = false;
            }

            // Validate Bill Type
            if (string.IsNullOrWhiteSpace(Bill.BillType))
            {
                SetHeaderError("BillType", "Bill Type is required");
                isValid = false;
            }

            // Validate Due Date
            if (Bill.DueDate < Bill.VendorInvoiceDate)
            {
                SetHeaderError("DueDate", "Due date cannot be before vendor invoice date");
                isValid = false;
            }

            return isValid;
        }

        #endregion

        #region Line Validation

        private void ValidateLine(VendorBillLineViewModel line)
        {
            var lineIndex = Bill.Lines.IndexOf(line);
            if (lineIndex < 0) return;

            // Clear existing errors for this line
            ClearLineErrors(lineIndex);

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

            // Validate Unit Rate
            if (line.UnitRate < 0)
            {
                SetLineError(lineIndex, "UnitRate", "Rate must be ≥ 0");
            }

            // Validate Discount Amount
            if (line.DiscountAmount < 0)
            {
                SetLineError(lineIndex, "DiscountAmount", "Must be ≥ 0");
            }
            else if (line.DiscountAmount > line.GrossAmount)
            {
                SetLineError(lineIndex, "DiscountAmount", "Cannot exceed gross amount");
            }

            // Validate Tax Rate Percent
            if (line.TaxRatePercentSnapshot < 0 || line.TaxRatePercentSnapshot > 100)
            {
                SetLineError(lineIndex, "TaxRatePercentSnapshot", "0-100 only");
            }

            // Validate Expense/Asset Account
            if (line.ExpenseOrAssetAccountId == Guid.Empty)
            {
                SetLineError(lineIndex, "ExpenseOrAssetAccountId", "Account required");
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

            for (int i = 0; i < Bill.Lines.Count; i++)
            {
                var line = Bill.Lines[i];

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

                // Validate Unit Rate
                if (line.UnitRate < 0)
                {
                    SetLineError(i, "UnitRate", "Rate must be ≥ 0");
                    isValid = false;
                }

                // Validate Discount Amount
                if (line.DiscountAmount < 0)
                {
                    SetLineError(i, "DiscountAmount", "Must be ≥ 0");
                    isValid = false;
                }
                else if (line.DiscountAmount > line.GrossAmount)
                {
                    SetLineError(i, "DiscountAmount", "Cannot exceed gross amount");
                    isValid = false;
                }

                // Validate Tax Rate Percent
                if (line.TaxRatePercentSnapshot < 0 || line.TaxRatePercentSnapshot > 100)
                {
                    SetLineError(i, "TaxRatePercentSnapshot", "0-100 only");
                    isValid = false;
                }

                // Validate Expense/Asset Account
                if (line.ExpenseOrAssetAccountId == Guid.Empty)
                {
                    SetLineError(i, "ExpenseOrAssetAccountId", "Account required");
                    isValid = false;
                }
            }

            return isValid;
        }

        #endregion

        #region Line Management

        private void AddLine()
        {
            var maxLineNumber = Bill.Lines.Any() ? Bill.Lines.Max(l => l.LineNumber) : 0;
            var newLine = new VendorBillLineViewModel
            {
                Id = Guid.NewGuid(),
                VendorBillId = Bill.Id,
                LineNumber = maxLineNumber + 10,
                LineType = VendorBillLineTypes.Expense,
                Quantity = 1,
                UnitRate = 0,
                TaxRatePercentSnapshot = 18, // Default GST rate
                ExpenseOrAssetAccountId = ExpenseAccounts.FirstOrDefault()?.Id ?? Guid.Empty
            };

            Bill.Lines.Add(newLine);
            RecalculateTotals();
        }

        private void DuplicateLine(VendorBillLineViewModel line)
        {
            var maxLineNumber = Bill.Lines.Max(l => l.LineNumber);
            var newLine = new VendorBillLineViewModel
            {
                Id = Guid.NewGuid(),
                VendorBillId = Bill.Id,
                LineNumber = maxLineNumber + 10,
                LineType = line.LineType,
                Description = line.Description,
                Quantity = line.Quantity,
                UnitRate = line.UnitRate,
                DiscountAmount = line.DiscountAmount,
                TaxRatePercentSnapshot = line.TaxRatePercentSnapshot,
                ExpenseOrAssetAccountId = line.ExpenseOrAssetAccountId,
                ExpenseOrAssetAccountCode = line.ExpenseOrAssetAccountCode,
                ExpenseOrAssetAccountName = line.ExpenseOrAssetAccountName
            };

            newLine.RecalculateAmounts();
            Bill.Lines.Add(newLine);
            RecalculateTotals();
        }

        private void DeleteLine(VendorBillLineViewModel line)
        {
            var lineIndex = Bill.Lines.IndexOf(line);
            ClearLineErrors(lineIndex);
            Bill.Lines.Remove(line);
            RecalculateTotals();
        }

        private void OnLineTypeChanged(VendorBillLineViewModel line, ChangeEventArgs e)
        {
            line.LineType = e.Value?.ToString() ?? VendorBillLineTypes.Expense;
            ValidateLine(line);
        }

        private void OnDescriptionChanged(VendorBillLineViewModel line, ChangeEventArgs e)
        {
            line.Description = e.Value?.ToString() ?? string.Empty;
            ValidateLine(line);
        }

        private void OnAccountChanged(VendorBillLineViewModel line, ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var accountId))
            {
                line.ExpenseOrAssetAccountId = accountId;
                var account = ExpenseAccounts.FirstOrDefault(a => a.Id == accountId);
                line.ExpenseOrAssetAccountCode = account?.AccountCode;
                line.ExpenseOrAssetAccountName = account?.AccountName;
            }
            else
            {
                line.ExpenseOrAssetAccountId = Guid.Empty;
                line.ExpenseOrAssetAccountCode = null;
                line.ExpenseOrAssetAccountName = null;
            }
            ValidateLine(line);
        }

        private void OnQuantityChanged(VendorBillLineViewModel line, ChangeEventArgs e)
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

        private void OnUnitRateChanged(VendorBillLineViewModel line, ChangeEventArgs e)
        {
            var value = e.Value?.ToString() ?? "0";
            if (decimal.TryParse(value, out var rate))
            {
                line.UnitRate = rate;
            }
            else
            {
                line.UnitRate = 0;
            }
            line.RecalculateAmounts();
            RecalculateTotals();
            ValidateLine(line);
        }

        private void OnDiscountChanged(VendorBillLineViewModel line, ChangeEventArgs e)
        {
            var value = e.Value?.ToString() ?? "0";
            if (decimal.TryParse(value, out var discount))
            {
                line.DiscountAmount = discount;
            }
            else
            {
                line.DiscountAmount = 0;
            }
            line.RecalculateAmounts();
            RecalculateTotals();
            ValidateLine(line);
        }

        private void OnTaxRateChanged(VendorBillLineViewModel line, ChangeEventArgs e)
        {
            var value = e.Value?.ToString() ?? "0";
            if (decimal.TryParse(value, out var rate))
            {
                line.TaxRatePercentSnapshot = rate;
            }
            else
            {
                line.TaxRatePercentSnapshot = 0;
            }
            line.RecalculateAmounts();
            RecalculateTotals();
            ValidateLine(line);
        }

        private void RecalculateTotals()
        {
            Bill.RecalculateTotals();
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
            // This method is called on form submit - just call SaveBill
            await SaveBill();
        }

        private async Task SaveBill()
        {
            // Get narration from rich text editor
            if (narrationEditor != null)
                Bill.BillNarration = await narrationEditor.GetHtmlAsync();

            // Validate header fields
            if (!ValidateHeader())
            {
                StateHasChanged();
                await ScrollToFirstValidationError();
                return;
            }

            // Validate lines - if no lines exist, add one and show validation errors
            if (!Bill.Lines.Any())
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

            // Set account names for lines
            foreach (var line in Bill.Lines)
            {
                var account = ExpenseAccounts.FirstOrDefault(a => a.Id == line.ExpenseOrAssetAccountId);
                line.ExpenseOrAssetAccountCode = account?.AccountCode;
                line.ExpenseOrAssetAccountName = account?.AccountName;
            }

            Bill.RecalculateTotals();

            if (IsEdit)
            {
                Bill.UpdatedAt = DateTime.Now;
                Bill.UpdatedBy = "Current User";
                var result = BillService.Update(Bill);
                if (result.Success)
                {
                    ToastService.ShowSuccess($"Bill '{Bill.BillNumber}' updated successfully.", "Updated");
                    Nav.NavigateTo("/vendor-bills");
                }
                else
                {
                    ToastService.ShowError(result.Message, "Error");
                }
            }
            else
            {
                Bill.CreatedAt = DateTime.Now;
                Bill.CreatedBy = "Current User";
                var result = BillService.Add(Bill);
                if (result.Success)
                {
                    ToastService.ShowSuccess($"Bill '{Bill.BillNumber}' created successfully.", "Created");
                    Nav.NavigateTo("/vendor-bills");
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
            // Validate header fields
            if (!ValidateHeader())
            {
                StateHasChanged();
                await ScrollToFirstValidationError();
                return;
            }

            // Validate lines - if no lines exist, add one and show validation errors
            if (!Bill.Lines.Any())
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

            // Set account names for lines
            foreach (var line in Bill.Lines)
            {
                var account = ExpenseAccounts.FirstOrDefault(a => a.Id == line.ExpenseOrAssetAccountId);
                line.ExpenseOrAssetAccountCode = account?.AccountCode;
                line.ExpenseOrAssetAccountName = account?.AccountName;
            }

            Bill.RecalculateTotals();

            // For new bills, save first then submit
            if (!IsEdit)
            {
                Bill.CreatedAt = DateTime.Now;
                Bill.CreatedBy = "Current User";
                var addResult = BillService.Add(Bill);
                if (!addResult.Success)
                {
                    ToastService.ShowError(addResult.Message, "Error");
                    return;
                }
                // Get the saved bill with the new ID
                var savedBill = BillService.GetAll()
                    .FirstOrDefault(b => b.BillNumber == Bill.BillNumber);
                if (savedBill != null)
                {
                    Bill = savedBill;
                }
            }

            var result = BillService.Submit(Bill.Id, "Current User");
            if (result.Success)
            {
                Bill.BillStatus = VendorBillStatuses.Submitted;
                ToastService.ShowSuccess("Bill submitted for approval.", "Submitted");
                Nav.NavigateTo("/vendor-bills");
            }
            else
            {
                ToastService.ShowError(result.Message, "Error");
            }

            await Task.CompletedTask;
        }

        private void PostBill()
        {
            Bill.PostingDate = DateTime.Today;
            var result = BillService.Post(
                Bill.Id,
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                "Current User");
            if (result.Success)
            {
                Bill.BillStatus = VendorBillStatuses.Posted;
                ToastService.ShowSuccess("Bill posted successfully.", "Posted");
                StateHasChanged();
            }
            else
            {
                ToastService.ShowError(result.Message, "Error");
            }
        }

        private void Cancel()
        {
            Nav.NavigateTo("/vendor-bills");
        }

        #endregion

        #region Helper Methods

        private string GetStatusBadge(string status) => status switch
        {
            VendorBillStatuses.Draft => "bg-secondary",
            VendorBillStatuses.Submitted => "bg-info",
            VendorBillStatuses.Approved => "bg-primary",
            VendorBillStatuses.Posted => "bg-success",
            VendorBillStatuses.Rejected => "bg-danger",
            VendorBillStatuses.Cancelled => "bg-dark",
            VendorBillStatuses.Reversed => "bg-warning",
            _ => "bg-secondary"
        };

        private string GetLineTypeCssClass(string lineType) => lineType?.ToLower() switch
        {
            "service" => "service",
            "goods" => "goods",
            "expense" => "expense",
            "asset" => "asset",
            _ => "other"
        };

        private void RecalculateLineAndTotal(VendorBillLineViewModel line)
        {
            line.RecalculateAmounts();
            RecalculateTotals();
        }

        private void RemoveLine(VendorBillLineViewModel line)
        {
            DeleteLine(line);
        }

        private async Task HandleValidSubmit()
        {
            formSubmitted = true;
           
            await SaveBill();
        }

        private async Task SubmitBill()
        {
            formSubmitted = true;
            await SubmitForApproval();
        }

        #endregion
    }
}
