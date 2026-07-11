using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.AR.CustomerCreditNote
{
    public partial class CreateCustomerCreditNote
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] CustomerCreditNoteService CreditNoteService { get; set; } = default!;
        [Inject] CustomerInvoiceService InvoiceService { get; set; } = default!;
        [Inject] CustomerService CustomerService { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] BranchService BranchService { get; set; } = default!;
        [Inject] COADataService COADataService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] IJSRuntime JSRuntime { get; set; } = default!;

        [Parameter] public Guid? Id { get; set; }

        private bool isInitialized = false;
        private CustomerCreditNoteViewModel CreditNote = new();
        private FinanceConnect.Client.Shared.RichTextEditor? narrationEditor;

        // Dropdown data
        private List<CompanyModel> Companies = new();
        private List<BranchModel> Branches = new();
        private List<CustomerViewModel> Customers = new();
        private List<CustomerInvoiceViewModel> PostedInvoices = new();
        private List<AccountViewModel> ReversalAccounts = new();

        // Validation errors dictionary
        private Dictionary<string, string> LineValidationErrors = new();
        private Dictionary<string, string> HeaderValidationErrors = new();

        private bool IsEdit => Id.HasValue;
        private bool IsReadOnly => CreditNote.CreditNoteStatus != CreditNoteStatuses.Draft;

        protected override async Task OnInitializedAsync()
        {
            await LoadMasterData();

            if (IsEdit)
            {
                var existing = CreditNoteService.GetById(Id!.Value);
                if (existing != null)
                {
                    CreditNote = existing;
                    LoadInvoicesForCustomer(CreditNote.CustomerId);
                }
                else
                {
                    ToastService.ShowError("Credit Note not found.", "Error");
                    Nav.NavigateTo("/customer-credit-notes");
                    return;
                }
            }
            else
            {
                CreditNote = CreateNewCreditNote();
            }

            isInitialized = true;
        }

        private async Task LoadMasterData()
        {
            Companies = MasterDataService.GetAllCompanies();

            Branches = BranchService.GetAll();

            Customers = CustomerService.GetAll().Where(c => c.CustomerStatus == CustomerStatuses.Active).ToList();

            ReversalAccounts = COADataService.GetAllAccounts();

            await Task.CompletedTask;
        }

        private void LoadInvoicesForCustomer(Guid customerId)
        {
            if (customerId == Guid.Empty)
            {
                PostedInvoices = new List<CustomerInvoiceViewModel>();
                return;
            }

            PostedInvoices = InvoiceService.GetByCustomerId(customerId)
                .Where(i => i.InvoiceStatus == InvoiceStatuses.Posted ||
                           i.InvoiceStatus == InvoiceStatuses.PartiallyPaid ||
                           i.InvoiceStatus == InvoiceStatuses.Paid)
                .ToList();
        }

        private CustomerCreditNoteViewModel CreateNewCreditNote()
        {
            var creditNoteNumber = CreditNoteService.GenerateCreditNoteNumber(
                Guid.Parse("c0c0c0c0-c0c0-c0c0-c0c0-c0c0c0c0c001"));

            return new CustomerCreditNoteViewModel
            {
                CompanyId = Guid.Parse("c0c0c0c0-c0c0-c0c0-c0c0-c0c0c0c0c001"),
                CompanyName = "Ascending Software Private Limited",
                BranchId = Guid.Parse("b0b0b0b0-b0b0-b0b0-b0b0-b0b0b0b0b001"),
                BranchName = "Head Office - Chennai",
                CreditNoteNumber = creditNoteNumber,
                CreditNoteDate = DateTime.Today,
                CreditNoteStatus = CreditNoteStatuses.Draft,
                ExchangeRate = 1,
                CurrencyId = Guid.Parse("d1d1d1d1-d1d1-d1d1-d1d1-d1d1d1d1d101"),
                CurrencyCode = "INR",
                CurrencyName = "Indian Rupee",
                IsAgainstInvoice = true,
                IsTaxImpacting = true,
                IsRevenueReversal = true,
                CreditReasonCode = ""
            };
        }

        #region Event Handlers

        private void OnBranchChanged(ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var branchId))
            {
                CreditNote.BranchId = branchId;
                var branch = Branches.FirstOrDefault(b => b.Id == branchId);
                CreditNote.BranchName = branch?.BranchName;
            }
            ClearHeaderError("BranchId");
        }

        private void OnCustomerChanged()
        {
            if (CreditNote.CustomerId != Guid.Empty)
            {
                var customer = Customers.FirstOrDefault(c => c.Id == CreditNote.CustomerId);
                if (customer != null)
                {
                    CreditNote.CustomerCode = customer.CustomerCode;
                    CreditNote.CustomerName = customer.CustomerName;
                    CreditNote.CurrencyId = customer.DefaultCurrencyId ?? CreditNote.CurrencyId;
                    CreditNote.CurrencyCode = customer.DefaultCurrencyCode ?? CreditNote.CurrencyCode;
                    CreditNote.CurrencyName = customer.DefaultCurrencyName ?? CreditNote.CurrencyName;
                }
                LoadInvoicesForCustomer(CreditNote.CustomerId);
                CreditNote.CustomerInvoiceId = null;
                CreditNote.CustomerInvoiceNumber = null;
                ClearHeaderError("CustomerId");
            }
            else
            {
                CreditNote.CustomerId = Guid.Empty;
                CreditNote.CustomerCode = null;
                CreditNote.CustomerName = null;
                PostedInvoices = new List<CustomerInvoiceViewModel>();
            }
        }

        private void OnReasonCodeChanged(ChangeEventArgs e)
        {
            CreditNote.CreditReasonCode = e.Value?.ToString() ?? "";
            CreditNote.CreditReasonDescription = string.IsNullOrEmpty(CreditNote.CreditReasonCode) ? "" : CreditReasonCodes.GetDisplayName(CreditNote.CreditReasonCode);
            ClearHeaderError("CreditReasonCode");
        }

        private void OnAgainstInvoiceChanged(ChangeEventArgs e)
        {
            CreditNote.IsAgainstInvoice = (bool)(e.Value ?? false);
            if (!CreditNote.IsAgainstInvoice)
            {
                CreditNote.CustomerInvoiceId = null;
                CreditNote.CustomerInvoiceNumber = null;
                CreditNote.InvoiceNumberSnapshot = null;
                CreditNote.InvoiceDateSnapshot = null;
            }
            ClearHeaderError("CustomerInvoiceId");
        }

        private void OnInvoiceChanged(ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var invoiceId))
            {
                CreditNote.CustomerInvoiceId = invoiceId;
                var invoice = PostedInvoices.FirstOrDefault(i => i.Id == invoiceId);
                if (invoice != null)
                {
                    CreditNote.CustomerInvoiceNumber = invoice.InvoiceNumber;
                    CreditNote.InvoiceNumberSnapshot = invoice.InvoiceNumber;
                    CreditNote.InvoiceDateSnapshot = invoice.InvoiceDate;
                }
                ClearHeaderError("CustomerInvoiceId");
            }
            else
            {
                CreditNote.CustomerInvoiceId = null;
                CreditNote.CustomerInvoiceNumber = null;
            }
        }

        #endregion

        #region Line Operations

        private void AddLine()
        {
            var maxLineNumber = CreditNote.Lines.Any() ? CreditNote.Lines.Max(l => l.LineNumber) : 0;
            var newLine = new CustomerCreditNoteLineModel
            {
                LineNumber = maxLineNumber + 10,
                LineType = "",
                Quantity = 1,
                UnitPrice = 0,
                TaxRatePercent = 18,
                RevenueReversalAccountId = Guid.Empty
            };
            CreditNote.Lines.Add(newLine);
            CreditNote.RecalculateTotals();
        }

        private void DuplicateLine(CustomerCreditNoteLineModel line)
        {
            var maxLineNumber = CreditNote.Lines.Max(l => l.LineNumber);
            var newLine = new CustomerCreditNoteLineModel
            {
                LineNumber = maxLineNumber + 10,
                LineType = line.LineType,
                Description = line.Description,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice,
                DiscountPercent = line.DiscountPercent,
                TaxRatePercent = line.TaxRatePercent,
                RevenueReversalAccountId = line.RevenueReversalAccountId,
                RevenueReversalAccountCode = line.RevenueReversalAccountCode,
                RevenueReversalAccountName = line.RevenueReversalAccountName
            };
            newLine.RecalculateAmounts();
            CreditNote.Lines.Add(newLine);
            CreditNote.RecalculateTotals();
        }

        private void DeleteLine(CustomerCreditNoteLineModel line)
        {
            CreditNote.Lines.Remove(line);
            CreditNote.RecalculateTotals();
            var lineIndex = CreditNote.Lines.IndexOf(line);
            ClearLineErrors(lineIndex);
        }

        private void OnLineTypeChanged(CustomerCreditNoteLineModel line, ChangeEventArgs e)
        {
            line.LineType = e.Value?.ToString() ?? "";
            var lineIndex = CreditNote.Lines.IndexOf(line);
            ClearLineError(lineIndex, "LineType");
        }

        private void OnDescriptionChanged(CustomerCreditNoteLineModel line, ChangeEventArgs e)
        {
            line.Description = e.Value?.ToString() ?? string.Empty;
            var lineIndex = CreditNote.Lines.IndexOf(line);
            ClearLineError(lineIndex, "Description");
        }

        private void OnQuantityChanged(CustomerCreditNoteLineModel line, ChangeEventArgs e)
        {
            if (decimal.TryParse(e.Value?.ToString(), out var qty))
            {
                line.Quantity = qty;
            }
            line.RecalculateAmounts();
            CreditNote.RecalculateTotals();
            var lineIndex = CreditNote.Lines.IndexOf(line);
            ClearLineError(lineIndex, "Quantity");
        }

        private void OnUnitPriceChanged(CustomerCreditNoteLineModel line, ChangeEventArgs e)
        {
            if (decimal.TryParse(e.Value?.ToString(), out var price))
            {
                line.UnitPrice = price;
            }
            line.RecalculateAmounts();
            CreditNote.RecalculateTotals();
        }

        private void OnDiscountChanged(CustomerCreditNoteLineModel line, ChangeEventArgs e)
        {
            if (decimal.TryParse(e.Value?.ToString(), out var disc))
            {
                line.DiscountPercent = disc;
            }
            line.RecalculateAmounts();
            CreditNote.RecalculateTotals();
        }

        private void OnTaxRateChanged(CustomerCreditNoteLineModel line, ChangeEventArgs e)
        {
            if (decimal.TryParse(e.Value?.ToString(), out var rate))
            {
                line.TaxRatePercent = rate;
            }
            line.RecalculateAmounts();
            CreditNote.RecalculateTotals();
        }

        private void OnReversalAccountChanged(CustomerCreditNoteLineModel line, ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var accountId))
            {
                line.RevenueReversalAccountId = accountId;
                var account = ReversalAccounts.FirstOrDefault(a => a.Id == accountId);
                line.RevenueReversalAccountCode = account?.AccountCode;
                line.RevenueReversalAccountName = account?.AccountName;
            }
            var lineIndex = CreditNote.Lines.IndexOf(line);
            ClearLineError(lineIndex, "RevenueReversalAccountId");
        }

        #endregion

        #region Validation

        private bool ValidateHeader()
        {
            HeaderValidationErrors.Clear();

            if (CreditNote.BranchId == Guid.Empty)
                HeaderValidationErrors["BranchId"] = "Branch is required.";

            if (CreditNote.CustomerId == Guid.Empty)
                HeaderValidationErrors["CustomerId"] = "Customer is required.";

            if (CreditNote.CreditNoteDate == default)
                HeaderValidationErrors["CreditNoteDate"] = "Credit Note Date is required.";

            if (string.IsNullOrWhiteSpace(CreditNote.CreditReasonCode))
                HeaderValidationErrors["CreditReasonCode"] = "Reason Code is required.";

            if (CreditNote.IsAgainstInvoice && CreditNote.CustomerInvoiceId == null)
                HeaderValidationErrors["CustomerInvoiceId"] = "Reference Invoice is required when 'Against Invoice' is enabled.";

            // PostingDate >= CreditNoteDate validation
            if (CreditNote.PostingDate.HasValue && CreditNote.PostingDate.Value < CreditNote.CreditNoteDate)
            {
                HeaderValidationErrors["PostingDate"] = "Posting Date must be on or after Credit Note Date.";
            }

            return !HeaderValidationErrors.Any();
        }

        private bool ValidateAllLines()
        {
            LineValidationErrors.Clear();
            bool isValid = true;

            for (int i = 0; i < CreditNote.Lines.Count; i++)
            {
                var line = CreditNote.Lines[i];

                if (string.IsNullOrWhiteSpace(line.LineType))
                {
                    LineValidationErrors[$"{i}_LineType"] = "Line Type is required.";
                    isValid = false;
                }

                if (string.IsNullOrWhiteSpace(line.Description))
                {
                    LineValidationErrors[$"{i}_Description"] = "Description is required.";
                    isValid = false;
                }

                if (line.Quantity <= 0)
                {
                    LineValidationErrors[$"{i}_Quantity"] = "Quantity must be greater than zero.";
                    isValid = false;
                }

                if (line.RevenueReversalAccountId == Guid.Empty)
                {
                    LineValidationErrors[$"{i}_RevenueReversalAccountId"] = "Reversal Account is required.";
                    isValid = false;
                }
            }

            return isValid;
        }

        private bool HasValidationError(int lineIndex, string field) =>
            LineValidationErrors.ContainsKey($"{lineIndex}_{field}");

        private string GetValidationError(int lineIndex, string field) =>
            LineValidationErrors.TryGetValue($"{lineIndex}_{field}", out var error) ? error : string.Empty;

        private string GetValidationClass(int lineIndex, string field) =>
            HasValidationError(lineIndex, field) ? "is-invalid" : "";

        private bool HasHeaderError(string field) => HeaderValidationErrors.ContainsKey(field);
        private string GetHeaderError(string field) =>
            HeaderValidationErrors.TryGetValue(field, out var error) ? error : string.Empty;
        private string GetHeaderValidationClass(string field) =>
            HasHeaderError(field) ? "is-invalid" : "";

        private void ClearLineError(int lineIndex, string field) =>
            LineValidationErrors.Remove($"{lineIndex}_{field}");

        private void ClearLineErrors(int lineIndex)
        {
            var keysToRemove = LineValidationErrors.Keys.Where(k => k.StartsWith($"{lineIndex}_")).ToList();
            foreach (var key in keysToRemove)
                LineValidationErrors.Remove(key);
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
            if (!ValidateHeader())
            {
                StateHasChanged();
                await ScrollToFirstValidationError();
                return;
            }

            if (!CreditNote.Lines.Any())
            {
                AddLine();
                ValidateAllLines();
                StateHasChanged();
                await ScrollToFirstValidationError();
                return;
            }

            if (!ValidateAllLines())
            {
                StateHasChanged();
                await ScrollToFirstValidationError();
                return;
            }

            foreach (var line in CreditNote.Lines)
            {
                var account = ReversalAccounts.FirstOrDefault(a => a.Id == line.RevenueReversalAccountId);
                line.RevenueReversalAccountCode = account?.AccountCode;
                line.RevenueReversalAccountName = account?.AccountName;
            }

            // Get narration from Quill editor
            if (narrationEditor != null)
            {
                CreditNote.CreditNoteNarration = await narrationEditor.GetHtmlAsync();
            }

            CreditNote.RecalculateTotals();

            if (IsEdit)
            {
                CreditNote.UpdatedAt = DateTime.Now;
                CreditNote.UpdatedBy = "Current User";
                var result = CreditNoteService.Update(CreditNote);
                if (result.Success)
                {
                    ToastService.ShowSuccess($"Credit Note '{CreditNote.CreditNoteNumber}' updated successfully.", "Updated");
                    Nav.NavigateTo("/customer-credit-notes");
                }
                else
                {
                    ToastService.ShowError(result.Message, "Error");
                }
            }
            else
            {
                CreditNote.CreatedAt = DateTime.Now;
                CreditNote.CreatedBy = "Current User";
                var result = CreditNoteService.Add(CreditNote);
                if (result.Success)
                {
                    ToastService.ShowSuccess($"Credit Note '{CreditNote.CreditNoteNumber}' created successfully.", "Created");
                    Nav.NavigateTo("/customer-credit-notes");
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
                await ScrollToFirstValidationError();
                return;
            }

            if (!CreditNote.Lines.Any())
            {
                AddLine();
                ValidateAllLines();
                StateHasChanged();
                await ScrollToFirstValidationError();
                return;
            }

            if (!ValidateAllLines())
            {
                StateHasChanged();
                await ScrollToFirstValidationError();
                return;
            }

            foreach (var line in CreditNote.Lines)
            {
                var account = ReversalAccounts.FirstOrDefault(a => a.Id == line.RevenueReversalAccountId);
                line.RevenueReversalAccountCode = account?.AccountCode;
                line.RevenueReversalAccountName = account?.AccountName;
            }

            // Get narration from Quill editor
            if (narrationEditor != null)
            {
                CreditNote.CreditNoteNarration = await narrationEditor.GetHtmlAsync();
            }

            CreditNote.RecalculateTotals();

            if (!IsEdit)
            {
                CreditNote.CreatedAt = DateTime.Now;
                CreditNote.CreatedBy = "Current User";
                var addResult = CreditNoteService.Add(CreditNote);
                if (!addResult.Success)
                {
                    ToastService.ShowError(addResult.Message, "Error");
                    return;
                }
                var savedCreditNote = CreditNoteService.GetAll()
                    .FirstOrDefault(cn => cn.CreditNoteNumber == CreditNote.CreditNoteNumber);
                if (savedCreditNote != null)
                {
                    CreditNote = savedCreditNote;
                }
            }

            var result = CreditNoteService.Submit(CreditNote.Id, "Current User");
            if (result.Success)
            {
                CreditNote.CreditNoteStatus = CreditNoteStatuses.Submitted;
                ToastService.ShowSuccess("Credit Note submitted for approval.", "Submitted");
                Nav.NavigateTo("/customer-credit-notes");
            }
            else
            {
                ToastService.ShowError(result.Message, "Error");
            }

            await Task.CompletedTask;
        }

        private void PostCreditNote()
        {
            CreditNote.PostingDate = DateTime.Today;
            var result = CreditNoteService.Post(
                CreditNote.Id,
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                "Current User");
            if (result.Success)
            {
                CreditNote.CreditNoteStatus = CreditNoteStatuses.Posted;
                ToastService.ShowSuccess("Credit Note posted successfully.", "Posted");
                StateHasChanged();
            }
            else
            {
                ToastService.ShowError(result.Message, "Error");
            }
        }

        private void Cancel()
        {
            Nav.NavigateTo("/customer-credit-notes");
        }

        #endregion

        #region Helper Methods

        private string GetStatusBadge(string status) => status switch
        {
            CreditNoteStatuses.Draft => "bg-secondary",
            CreditNoteStatuses.Submitted => "bg-info",
            CreditNoteStatuses.Approved => "bg-primary",
            CreditNoteStatuses.Posted => "bg-success",
            CreditNoteStatuses.Cancelled => "bg-danger",
            CreditNoteStatuses.Reversed => "bg-dark",
            _ => "bg-secondary"
        };

        #endregion

    }
}
