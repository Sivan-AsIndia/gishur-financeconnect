using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.AR.CustomerDebitNote
{
    public partial class CreateCustomerDebitNote
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] CustomerDebitNoteService DebitNoteService { get; set; } = default!;
        [Inject] CustomerInvoiceService InvoiceService { get; set; } = default!;
        [Inject] CustomerService CustomerService { get; set; } = default!;
        [Inject] BranchService BranchService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] IJSRuntime JSRuntime { get; set; } = default!;

        [Parameter] public Guid? Id { get; set; }

        private bool isInitialized = false;
        private CustomerDebitNoteViewModel DebitNote = new();
        private FinanceConnect.Client.Shared.RichTextEditor? narrationEditor;

        // Dropdown data
        private List<BranchModel> Branches = new();
        private List<CustomerViewModel> Customers = new();
        private List<CustomerInvoiceViewModel> PostedInvoices = new();
        private List<GLAccountViewModel> RevenueAccounts = new();

        // Validation errors
        private Dictionary<string, string> LineValidationErrors = new();
        private Dictionary<string, string> HeaderValidationErrors = new();

        private bool IsEdit => Id.HasValue;
        private bool IsReadOnly => DebitNote.DebitNoteStatus != DebitNoteStatuses.Draft;

        protected override async Task OnInitializedAsync()
        {
            // Load master data
            await LoadMasterData();

            if (IsEdit)
            {
                var existing = DebitNoteService.GetById(Id!.Value);
                if (existing != null)
                {
                    DebitNote = existing;
                    // Load posted invoices for selected customer
                    if (DebitNote.CustomerId != Guid.Empty)
                    {
                        LoadPostedInvoicesForCustomer(DebitNote.CustomerId);
                    }
                }
                else
                {
                    ToastService.ShowError("Debit Note not found.", "Error");
                    Nav.NavigateTo("/customer-debit-notes");
                    return;
                }
            }
            else
            {
                DebitNote = CreateNewDebitNote();
            }

            isInitialized = true;
        }

        private async Task LoadMasterData()
        {
            // Load branches
            Branches = BranchService.GetAll();

            // Load active customers
            Customers = CustomerService.GetAll().Where(c => c.CustomerStatus == CustomerStatuses.Active).ToList();

            // Load revenue accounts
            RevenueAccounts = new List<GLAccountViewModel>
            {
                new GLAccountViewModel { Id = Guid.Parse("00000000-0000-0000-0000-000000000010"), Code = "4100", Name = "Sales Revenue" },
                new GLAccountViewModel { Id = Guid.Parse("00000000-0000-0000-0000-000000000011"), Code = "4110", Name = "Service Revenue" },
                new GLAccountViewModel { Id = Guid.Parse("00000000-0000-0000-0000-000000000012"), Code = "4120", Name = "Export Revenue" },
                new GLAccountViewModel { Id = Guid.Parse("00000000-0000-0000-0000-000000000013"), Code = "4200", Name = "Other Income" },
                new GLAccountViewModel { Id = Guid.Parse("00000000-0000-0000-0000-000000000014"), Code = "4210", Name = "Late Fee Income" },
                new GLAccountViewModel { Id = Guid.Parse("00000000-0000-0000-0000-000000000015"), Code = "4220", Name = "Interest Income" }
            };

            await Task.CompletedTask;
        }

        private void LoadPostedInvoicesForCustomer(Guid customerId)
        {
            // Get posted invoices for the selected customer
            PostedInvoices = InvoiceService.GetByCustomerId(customerId)
                .Where(i => i.InvoiceStatus == InvoiceStatuses.Posted || 
                           i.InvoiceStatus == InvoiceStatuses.PartiallyPaid)
                .OrderByDescending(i => i.InvoiceDate)
                .ToList();
        }

        private CustomerDebitNoteViewModel CreateNewDebitNote()
        {
            var debitNoteNumber = DebitNoteService.GenerateDebitNoteNumber(
                Guid.Parse("c0c0c0c0-c0c0-c0c0-c0c0-c0c0c0c0c001"));

            var newDebitNote = new CustomerDebitNoteViewModel
            {
                CompanyId = Guid.Parse("c0c0c0c0-c0c0-c0c0-c0c0-c0c0c0c0c001"),
                CompanyName = "Ascending Software Private Limited",
                BranchId = Guid.Parse("b0b0b0b0-b0b0-b0b0-b0b0-b0b0b0b0b001"),
                BranchName = "Head Office - Chennai",
                DebitNoteNumber = debitNoteNumber,
                DebitNoteDate = DateTime.Today,
                DebitNoteStatus = DebitNoteStatuses.Draft,
                DebitReasonCode = "",
                DebitReasonDescription = "",
                ExchangeRate = 1,
                CurrencyId = Guid.Parse("a0a0a0a0-a0a0-a0a0-a0a0-a0a0a0a0a001"),
                CurrencyCode = "INR",
                CurrencyName = "Indian Rupee",
                IsAgainstInvoice = true,
                IsTaxImpacting = true,
                IsRevenueRecognized = true,
                CreatedBy = "Current User",
                CreatedAt = DateTime.Now
            };

            // Add one default line
            newDebitNote.Lines.Add(CreateNewLine(1));

            return newDebitNote;
        }

        private CustomerDebitNoteLineViewModel CreateNewLine(int lineNumber)
        {
            return new CustomerDebitNoteLineViewModel
            {
                Id = Guid.NewGuid(),
                CustomerDebitNoteId = DebitNote.Id,
                LineNumber = lineNumber,
                LineType = "",
                Quantity = 1,
                UnitPrice = 0,
                DiscountPercent = 0,
                TaxRatePercent = 18, // Default GST rate
                RevenueAccountId = Guid.Empty
            };
        }

        #region Event Handlers

        private void OnCustomerChanged()
        {
            if (DebitNote.CustomerId != Guid.Empty)
            {
                var customer = Customers.FirstOrDefault(c => c.Id == DebitNote.CustomerId);
                if (customer != null)
                {
                    DebitNote.CustomerCode = customer.CustomerCode;
                    DebitNote.CustomerName = customer.CustomerName;
                    DebitNote.CurrencyId = customer.DefaultCurrencyId ?? Guid.Parse("a0a0a0a0-a0a0-a0a0-a0a0-a0a0a0a0a001");
                    DebitNote.CurrencyCode = customer.DefaultCurrencyCode ?? "INR";

                    // Load posted invoices for this customer
                    LoadPostedInvoicesForCustomer(customer.Id);

                    // Clear invoice reference if customer changed
                    DebitNote.CustomerInvoiceId = null;
                    DebitNote.InvoiceNumberSnapshot = null;
                    DebitNote.InvoiceDateSnapshot = null;
                }
            }
            else
            {
                DebitNote.CustomerCode = null;
                DebitNote.CustomerName = null;
                PostedInvoices.Clear();
            }

            ClearHeaderError("CustomerId");
        }

        private void OnAgainstInvoiceChanged()
        {
            if (!DebitNote.IsAgainstInvoice)
            {
                // Clear invoice reference
                DebitNote.CustomerInvoiceId = null;
                DebitNote.InvoiceNumberSnapshot = null;
                DebitNote.InvoiceDateSnapshot = null;
            }
            ClearHeaderError("CustomerInvoiceId");
        }

        private void OnInvoiceSelected()
        {
            if (DebitNote.CustomerInvoiceId.HasValue && DebitNote.CustomerInvoiceId != Guid.Empty)
            {
                var invoice = PostedInvoices.FirstOrDefault(i => i.Id == DebitNote.CustomerInvoiceId);
                if (invoice != null)
                {
                    DebitNote.InvoiceNumberSnapshot = invoice.InvoiceNumber;
                    DebitNote.InvoiceDateSnapshot = invoice.InvoiceDate;
                    DebitNote.CustomerInvoiceNumber = invoice.InvoiceNumber;
                }
            }
            else
            {
                DebitNote.InvoiceNumberSnapshot = null;
                DebitNote.InvoiceDateSnapshot = null;
                DebitNote.CustomerInvoiceNumber = null;
            }

            ClearHeaderError("CustomerInvoiceId");
        }

        private void OnReasonCodeChanged()
        {
            DebitNote.DebitReasonDescription = DebitReasonCodes.GetDescription(DebitNote.DebitReasonCode);
            
            // Set default tax impacting based on reason
            if (DebitNote.DebitReasonCode == DebitReasonCodes.LateFee)
            {
                DebitNote.IsTaxImpacting = false; // Late fees typically don't have GST
                DebitNote.IsRevenueRecognized = false; // Penalties are not revenue
            }
            else if (DebitNote.DebitReasonCode == DebitReasonCodes.TaxShortCharged)
            {
                DebitNote.IsTaxImpacting = true;
                DebitNote.IsRevenueRecognized = false; // This is tax correction, not revenue
            }
            else
            {
                DebitNote.IsTaxImpacting = true;
                DebitNote.IsRevenueRecognized = true;
            }

            ClearHeaderError("DebitReasonCode");
        }

        #endregion

        #region Line Operations

        private void AddLine()
        {
            var newLineNumber = DebitNote.Lines.Any() ? DebitNote.Lines.Max(l => l.LineNumber) + 1 : 1;
            DebitNote.Lines.Add(CreateNewLine(newLineNumber));
        }

        private void RemoveLine(CustomerDebitNoteLineViewModel line)
        {
            if (DebitNote.Lines.Count > 1)
            {
                DebitNote.Lines.Remove(line);
                RenumberLines();
                RecalculateTotals();
            }
        }

        private void DuplicateLine(CustomerDebitNoteLineViewModel line)
        {
            var newLine = new CustomerDebitNoteLineViewModel
            {
                Id = Guid.NewGuid(),
                CustomerDebitNoteId = DebitNote.Id,
                LineNumber = DebitNote.Lines.Max(l => l.LineNumber) + 1,
                LineType = line.LineType,
                Description = line.Description,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice,
                DiscountPercent = line.DiscountPercent,
                TaxRatePercent = line.TaxRatePercent,
                RevenueAccountId = line.RevenueAccountId
            };

            newLine.RecalculateAmounts();
            DebitNote.Lines.Add(newLine);
            RecalculateTotals();
        }

        private void RenumberLines()
        {
            for (int i = 0; i < DebitNote.Lines.Count; i++)
            {
                DebitNote.Lines[i].LineNumber = i + 1;
            }
        }

        private void RecalculateLine(CustomerDebitNoteLineViewModel line)
        {
            line.RecalculateAmounts();
            RecalculateTotals();
            ValidateLine(line);
        }

        private void RecalculateTotals()
        {
            DebitNote.RecalculateTotals();
        }

        #endregion

        #region Header Validation

        private void SetHeaderError(string fieldName, string errorMessage)
        {
            HeaderValidationErrors[fieldName] = errorMessage;
        }

        private void ClearHeaderError(string fieldName)
        {
            HeaderValidationErrors.Remove(fieldName);
        }

        private bool HasHeaderError(string fieldName)
        {
            return HeaderValidationErrors.ContainsKey(fieldName);
        }

        private string GetHeaderError(string fieldName)
        {
            return HeaderValidationErrors.TryGetValue(fieldName, out var error) ? error : string.Empty;
        }

        void OnDebitNoteDateChange()
        {
            if (DebitNote.DebitNoteDate > DebitNote.PostingDate)
            {

                ToastService.ShowError("Posting Date should not be earlier then Debit Note Date");
                //DebitNote.PostingDate = null;
            }
        }

        private bool ValidateHeader()
        {
            HeaderValidationErrors.Clear();
            var isValid = true;

            if (DebitNote.CustomerId == Guid.Empty)
            {
                SetHeaderError("CustomerId", "Customer is required");
                isValid = false;
            }

            if (DebitNote.BranchId == Guid.Empty)
            {
                SetHeaderError("BranchId", "Branch is required");
                isValid = false;
            }

            if (DebitNote.DebitNoteDate == default)
            {
                SetHeaderError("DebitNoteDate", "Debit Note Date is required");
                isValid = false;
            }

            if (string.IsNullOrEmpty(DebitNote.DebitReasonCode))
            {
                SetHeaderError("DebitReasonCode", "Reason Code is required");
                isValid = false;
            }

            if (DebitNote.IsAgainstInvoice && (!DebitNote.CustomerInvoiceId.HasValue || DebitNote.CustomerInvoiceId == Guid.Empty))
            {
                SetHeaderError("CustomerInvoiceId", "Invoice selection is required when 'Against Invoice' is enabled");
                isValid = false;
            }

            // PostingDate >= DebitNoteDate validation
            if (DebitNote.PostingDate.HasValue && DebitNote.PostingDate.Value < DebitNote.DebitNoteDate)
            {
                SetHeaderError("PostingDate", "Posting Date must be on or after Debit Note Date.");
                isValid = false;
            }

            return isValid;
        }

        #endregion

        #region Line Validation

        private void ValidateLine(CustomerDebitNoteLineViewModel line)
        {
            var lineIndex = DebitNote.Lines.IndexOf(line);
            if (lineIndex < 0) return;

            ClearLineErrors(lineIndex);

            if (string.IsNullOrWhiteSpace(line.LineType))
            {
                SetLineError(lineIndex, "LineType", "Line Type is required");
            }

            if (string.IsNullOrWhiteSpace(line.Description))
            {
                SetLineError(lineIndex, "Description", "Description is required");
            }

            if (line.Quantity <= 0)
            {
                SetLineError(lineIndex, "Quantity", "Qty must be > 0");
            }

            if (line.UnitPrice < 0)
            {
                SetLineError(lineIndex, "UnitPrice", "Price must be ≥ 0");
            }

            if (line.DiscountPercent < 0 || line.DiscountPercent > 100)
            {
                SetLineError(lineIndex, "DiscountPercent", "0-100 only");
            }

            if (line.TaxRatePercent < 0 || line.TaxRatePercent > 100)
            {
                SetLineError(lineIndex, "TaxRatePercent", "0-100 only");
            }

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
            return HasValidationError(lineIndex, fieldName) ? "is-invalid" : "";
        }

        private bool ValidateAllLines()
        {
            LineValidationErrors.Clear();
            var isValid = true;

            if (!DebitNote.Lines.Any())
            {
                SetHeaderError("Lines", "At least one line is required.");
                return false;
            }

            for (int i = 0; i < DebitNote.Lines.Count; i++)
            {
                var line = DebitNote.Lines[i];

                if (string.IsNullOrWhiteSpace(line.LineType))
                {
                    SetLineError(i, "LineType", "Line Type is required");
                    isValid = false;
                }

                if (string.IsNullOrWhiteSpace(line.Description))
                {
                    SetLineError(i, "Description", "Description is required");
                    isValid = false;
                }

                if (line.Quantity <= 0)
                {
                    SetLineError(i, "Quantity", "Qty must be > 0");
                    isValid = false;
                }

                if (line.UnitPrice < 0)
                {
                    SetLineError(i, "UnitPrice", "Price must be ≥ 0");
                    isValid = false;
                }

                if (line.DiscountPercent < 0 || line.DiscountPercent > 100)
                {
                    SetLineError(i, "DiscountPercent", "0-100 only");
                    isValid = false;
                }

                if (line.TaxRatePercent < 0 || line.TaxRatePercent > 100)
                {
                    SetLineError(i, "TaxRatePercent", "0-100 only");
                    isValid = false;
                }

                if (line.RevenueAccountId == Guid.Empty)
                {
                    SetLineError(i, "RevenueAccountId", "Account required");
                    isValid = false;
                }
            }

            return isValid;
        }

        private async Task ScrollToFirstValidationError()
        {
            await Task.Delay(100); // Allow DOM to update with validation errors
            await JSRuntime.InvokeVoidAsync("scrollToFirstValidationError");
        }

        #endregion

        #region Form Submission

        private async Task HandleValidSubmit()
        {
            // Validate header
            if (!ValidateHeader())
            {
                StateHasChanged();
                await ScrollToFirstValidationError();
                return;
            }

            // Validate lines
            if (!ValidateAllLines())
            {
                StateHasChanged();
                await ScrollToFirstValidationError();
                return;
            }

            // Recalculate totals before saving
            RecalculateTotals();

            // Get narration from Quill editor
            if (narrationEditor != null)
            {
                DebitNote.DebitNoteNarration = await narrationEditor.GetHtmlAsync();
            }

            if (DebitNote.DebitNoteDate > DebitNote.PostingDate)
            {

                ToastService.ShowError("Posting Date should not be earlier then Debit Note Date");
                return;
            }

            if (DebitNote.GrandTotalAmount <= 0)
            {
                ToastService.ShowWarning("Debit note total must be greater than zero.");
                return;
            }

            var result = IsEdit 
                ? DebitNoteService.Update(DebitNote) 
                : DebitNoteService.Add(DebitNote);

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                Nav.NavigateTo("/customer-debit-notes");
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        private async Task SubmitDebitNote()
        {
            // First save if there are changes
            if (!ValidateHeader())
            {
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
            if (DebitNote.DebitNoteDate > DebitNote.PostingDate)
            {

                ToastService.ShowError("Posting Date should not be earlier then Debit Note Date");
                return;
            }
            RecalculateTotals();

            // Get narration from Quill editor
            if (narrationEditor != null)
            {
                DebitNote.DebitNoteNarration = await narrationEditor.GetHtmlAsync();
            }

            // Save first if new
            if (!IsEdit)
            {
                var saveResult = DebitNoteService.Add(DebitNote);
                if (!saveResult.Success)
                {
                    ToastService.ShowError(saveResult.Message);
                    return;
                }
            }
            else
            {
                var updateResult = DebitNoteService.Update(DebitNote);
                if (!updateResult.Success)
                {
                    ToastService.ShowError(updateResult.Message);
                    return;
                }
            }

            // Now submit
            var result = DebitNoteService.Submit(DebitNote.Id, "Current User");
            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                Nav.NavigateTo("/customer-debit-notes");
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        private async Task PostDebitNote()
        {
            if (!DebitNote.CanPost)
            {
                ToastService.ShowWarning("This debit note cannot be posted in its current status.");
                return;
            }

            if (!ValidateHeader())
            {
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

            RecalculateTotals();

            if (DebitNote.GrandTotalAmount <= 0)
            {
                ToastService.ShowWarning("Debit note total must be greater than zero to post.");
                return;
            }

            // Save changes first
            var saveResult = IsEdit ? DebitNoteService.Update(DebitNote) : DebitNoteService.Add(DebitNote);
            if (!saveResult.Success)
            {
                ToastService.ShowError(saveResult.Message);
                return;
            }

            // Now post
            var result = DebitNoteService.Post(
                DebitNote.Id,
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                "Current User");

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                Nav.NavigateTo("/customer-debit-notes");
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        #endregion

        #region Helper Methods

        private string GetStatusBadgeClass(string status) => status switch
        {
            DebitNoteStatuses.Draft => "bg-secondary",
            DebitNoteStatuses.Submitted => "bg-info",
            DebitNoteStatuses.Approved => "bg-primary",
            DebitNoteStatuses.Posted => "bg-success",
            DebitNoteStatuses.Cancelled => "bg-warning text-dark",
            DebitNoteStatuses.Reversed => "bg-danger",
            _ => "bg-secondary"
        };

        private string GetLineTypeBadgeClass(string lineType) => lineType switch
        {
            "Item" => "bg-primary",
            "Service" => "bg-info",
            "Charge" => "bg-warning text-dark",
            "Fee" => "bg-danger",
            _ => "bg-secondary"
        };

        #endregion
    }
}
