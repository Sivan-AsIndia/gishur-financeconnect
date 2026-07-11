using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using FinanceConnect.Client.Shared;

namespace FinanceConnect.Client.Pages.AP.VendorCreditNote
{
    public partial class CreateVendorCreditNote
    {
        [Parameter] public Guid? Id { get; set; }

        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] VendorCreditNoteService CreditNoteService { get; set; } = default!;
        [Inject] VendorService VendorService { get; set; } = default!;
        [Inject] VendorBillService BillService { get; set; } = default!;
        [Inject] BranchService BranchService { get; set; } = default!;
        [Inject] COADataService COADataService { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;

        private bool isInitialized = false;
        private bool IsEdit => Id.HasValue;
        private bool IsReadOnly => CreditNote.CreditNoteStatus != VendorCreditNoteStatuses.Draft;
        private RichTextEditor? narrationEditor;
        private VendorCreditNoteViewModel CreditNote = new();
        private List<VendorViewModel> Vendors = new();
        private List<BranchModel> Branches = new();
        private List<VendorBillViewModel> PostedBills = new();
        private List<AccountViewModel> ReversalAccounts = new();
        private List<StateProvinceModel> States = new();

        // Validation state
        private Dictionary<string, string> HeaderErrors = new();
        private Dictionary<int, Dictionary<string, string>> LineErrors = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadLookupData();

            if (IsEdit)
            {
                var existing = CreditNoteService.GetById(Id!.Value);
                if (existing != null)
                {
                    CreditNote = existing;
                    await LoadBillsForVendor(CreditNote.VendorId);
                }
                else
                {
                    ToastService.ShowError("Credit Note not found.");
                    Nav.NavigateTo("/vendor-credit-notes");
                    return;
                }
            }
            else
            {
                InitializeNewCreditNote();
            }

            isInitialized = true;
        }

        private async Task LoadLookupData()
        {
            await Task.Delay(50); // Simulate async

            Vendors = VendorService.GetAll();
            Branches = BranchService.GetAll();
            
            // Load GL accounts for reversal from COADataService
            ReversalAccounts = COADataService.GetAllAccounts();
            
            // Load states for Place of Supply
            States = MasterDataService.GetAllStateProvinces()
                .Where(s => s.IsActive)
                .OrderBy(s => s.StateProvinceName).ToList();
        }

        private void InitializeNewCreditNote()
        {
            var defaultCompanyId = Guid.Parse("c0c0c0c0-c0c0-c0c0-c0c0-c0c0c0c0c001");
            var defaultCurrencyId = Guid.Parse("d1d1d1d1-d1d1-d1d1-d1d1-d1d1d1d1d101");

            CreditNote = new VendorCreditNoteViewModel
            {
                Id = Guid.NewGuid(),
                CompanyId = defaultCompanyId,
                CompanyName = "Ascending Software Private Limited",
                CreditNoteNumber = CreditNoteService.GenerateCreditNoteNumber(defaultCompanyId),
                VendorCreditNoteDate = DateTime.Today,
                CreditEntryDate = DateTime.Today,
                CurrencyId = defaultCurrencyId,
                CurrencyCode = "INR",
                CurrencyName = "Indian Rupee",
                ExchangeRate = 1,
                CreditNoteStatus = VendorCreditNoteStatuses.Draft,
                CreditNoteType = "",
                IsAgainstBill = true,
                IsGSTApplicable = true,
                CreatedBy = "System"
            };
        }

        private async Task LoadBillsForVendor(Guid vendorId)
        {
            await Task.Delay(10);
            PostedBills = BillService.GetAll()
                .Where(b => b.VendorId == vendorId && b.BillStatus == VendorBillStatuses.Posted)
                .ToList();
        }

        #region Event Handlers

        private async Task OnBranchChanged(ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var branchId))
            {
                CreditNote.BranchId = branchId;
                var branch = Branches.FirstOrDefault(b => b.Id == branchId);
                if (branch != null)
                {
                    CreditNote.BranchCode = branch.BranchCode;
                    CreditNote.BranchName = branch.BranchName;
                }
            }
            else
            {
                CreditNote.BranchId = Guid.Empty;
                CreditNote.BranchCode = null;
                CreditNote.BranchName = null;
            }
            await Task.CompletedTask;
        }

        private async Task OnVendorChanged()
        {
            if (CreditNote.VendorId != Guid.Empty)
            {
                var vendor = Vendors.FirstOrDefault(v => v.Id == CreditNote.VendorId);
                if (vendor != null)
                {
                    CreditNote.VendorCode = vendor.VendorCode;
                    CreditNote.VendorName = vendor.VendorName;
                    CreditNote.CurrencyCode = vendor.DefaultCurrencyCode ?? "INR";
                    CreditNote.VendorGSTINSnapshot = vendor.GSTIN;
                }

                await LoadBillsForVendor(CreditNote.VendorId);
                
                // Reset bill selection when vendor changes
                CreditNote.PrimaryVendorBillId = null;
                CreditNote.PrimaryVendorBillNumber = null;
                CreditNote.BillNumberSnapshot = null;
            }
            else
            {
                CreditNote.VendorId = Guid.Empty;
                CreditNote.VendorCode = null;
                CreditNote.VendorName = null;
                PostedBills = new List<VendorBillViewModel>();
            }
        }

        private void OnCreditTypeChanged(ChangeEventArgs e)
        {
            CreditNote.CreditNoteType = e.Value?.ToString() ?? VendorCreditNoteTypes.Other;
        }

        private void OnAgainstBillChanged(ChangeEventArgs e)
        {
            CreditNote.IsAgainstBill = (bool)(e.Value ?? false);
            if (!CreditNote.IsAgainstBill)
            {
                CreditNote.PrimaryVendorBillId = null;
                CreditNote.PrimaryVendorBillNumber = null;
                CreditNote.BillNumberSnapshot = null;
                CreditNote.BillDateSnapshot = null;
            }
        }

        private void OnBillChanged(ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var billId))
            {
                CreditNote.PrimaryVendorBillId = billId;
                var bill = PostedBills.FirstOrDefault(b => b.Id == billId);
                if (bill != null)
                {
                    CreditNote.PrimaryVendorBillNumber = bill.BillNumber;
                    CreditNote.BillNumberSnapshot = bill.BillNumber;
                    CreditNote.BillDateSnapshot = bill.BillDate;
                }
            }
            else
            {
                CreditNote.PrimaryVendorBillId = null;
                CreditNote.PrimaryVendorBillNumber = null;
                CreditNote.BillNumberSnapshot = null;
                CreditNote.BillDateSnapshot = null;
            }
        }

        #endregion

        #region Line Operations

        private void AddLine()
        {
            var maxLineNumber = CreditNote.Lines.Any() ? CreditNote.Lines.Max(l => l.LineNumber) : 0;
            var newLine = new VendorCreditNoteLineModel
            {
                Id = Guid.NewGuid(),
                VendorCreditNoteId = CreditNote.Id,
                LineNumber = maxLineNumber + 10,
                LineType = VendorCreditNoteLineTypes.Manual,
                Quantity = 1,
                UnitPrice = 0,
                TaxRatePercent = 18,
                CreatedAt = DateTime.Now,
                CreatedBy = "System"
            };

            CreditNote.Lines.Add(newLine);
            RecalculateTotals();
        }

        private void DeleteLine(VendorCreditNoteLineModel line)
        {
            CreditNote.Lines.Remove(line);
            RecalculateTotals();
        }

        private void DuplicateLine(VendorCreditNoteLineModel line)
        {
            var maxLineNumber = CreditNote.Lines.Max(l => l.LineNumber);
            var newLine = new VendorCreditNoteLineModel
            {
                Id = Guid.NewGuid(),
                VendorCreditNoteId = CreditNote.Id,
                LineNumber = maxLineNumber + 10,
                LineType = line.LineType,
                Description = line.Description,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice,
                DiscountPercent = line.DiscountPercent,
                TaxRatePercent = line.TaxRatePercent,
                ReversalAccountId = line.ReversalAccountId,
                ReversalAccountCode = line.ReversalAccountCode,
                ReversalAccountName = line.ReversalAccountName,
                HSNCode = line.HSNCode,
                SACCode = line.SACCode,
                CreatedAt = DateTime.Now,
                CreatedBy = "System"
            };

            newLine.RecalculateAmounts();
            CreditNote.Lines.Add(newLine);
            RecalculateTotals();
        }

        private void OnLineTypeChanged(VendorCreditNoteLineModel line, ChangeEventArgs e)
        {
            line.LineType = e.Value?.ToString() ?? VendorCreditNoteLineTypes.Manual;
        }

        private void OnDescriptionChanged(VendorCreditNoteLineModel line, ChangeEventArgs e)
        {
            line.Description = e.Value?.ToString() ?? string.Empty;
        }

        private void OnQuantityChanged(VendorCreditNoteLineModel line, ChangeEventArgs e)
        {
            if (decimal.TryParse(e.Value?.ToString(), out var qty))
            {
                line.Quantity = qty;
                line.RecalculateAmounts();
                RecalculateTotals();
            }
        }

        private void OnUnitPriceChanged(VendorCreditNoteLineModel line, ChangeEventArgs e)
        {
            if (decimal.TryParse(e.Value?.ToString(), out var price))
            {
                line.UnitPrice = price;
                line.RecalculateAmounts();
                RecalculateTotals();
            }
        }

        private void OnDiscountChanged(VendorCreditNoteLineModel line, ChangeEventArgs e)
        {
            if (decimal.TryParse(e.Value?.ToString(), out var disc))
            {
                line.DiscountPercent = disc;
                line.DiscountAmount = 0; // Reset so RecalculateAmounts calculates from percent
                line.RecalculateAmounts();
                RecalculateTotals();
            }
        }

        private void OnTaxRateChanged(VendorCreditNoteLineModel line, ChangeEventArgs e)
        {
            if (decimal.TryParse(e.Value?.ToString(), out var rate))
            {
                line.TaxRatePercent = rate;
                line.RecalculateAmounts();
                RecalculateTotals();
            }
        }

        private void OnReversalAccountChanged(VendorCreditNoteLineModel line, ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var accountId))
            {
                line.ReversalAccountId = accountId;
                var account = ReversalAccounts.FirstOrDefault(a => a.Id == accountId);
                if (account != null)
                {
                    line.ReversalAccountCode = account.AccountCode;
                    line.ReversalAccountName = account.AccountName;
                }
            }
            else
            {
                line.ReversalAccountId = Guid.Empty;
                line.ReversalAccountCode = null;
                line.ReversalAccountName = null;
            }
        }

        private void RecalculateTotals()
        {
            CreditNote.RecalculateTotals();
        }

        #endregion

        #region Form Actions

        private async Task HandleSubmit()
        {
            // Get narration from rich text editor
            if (narrationEditor != null)
                CreditNote.CreditNoteNarration = await narrationEditor.GetHtmlAsync();

            ClearErrors();

            if (!ValidateForm())
            {
                await JS.InvokeVoidAsync("scrollToFirstValidationError");
                ToastService.ShowError("Please correct the validation errors.");
                return;
            }

            CreditNote.UpdatedAt = DateTime.Now;
            CreditNote.UpdatedBy = "System";

            (bool Success, string Message) result;

            if (IsEdit)
            {
                result = CreditNoteService.Update(CreditNote);
            }
            else
            {
                CreditNote.CreatedAt = DateTime.Now;
                CreditNote.CreatedBy = "System";
                result = CreditNoteService.Add(CreditNote);
            }

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                Nav.NavigateTo("/vendor-credit-notes");
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        private async Task SubmitForApproval()
        {
            ClearErrors();

            if (!ValidateForm())
            {
                await JS.InvokeVoidAsync("scrollToFirstValidationError");
                ToastService.ShowError("Please correct the validation errors before submitting.");
                return;
            }

            // Save first if there are changes
            if (!IsEdit)
            {
                var addResult = CreditNoteService.Add(CreditNote);
                if (!addResult.Success)
                {
                    ToastService.ShowError(addResult.Message);
                    return;
                }
            }
            else
            {
                var updateResult = CreditNoteService.Update(CreditNote);
                if (!updateResult.Success)
                {
                    ToastService.ShowError(updateResult.Message);
                    return;
                }
            }

            var result = CreditNoteService.Submit(
                CreditNote.Id,
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                "System");

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                Nav.NavigateTo("/vendor-credit-notes");
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        private void PostCreditNote()
        {
            var result = CreditNoteService.Post(
                CreditNote.Id,
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                "System");

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                Nav.NavigateTo("/vendor-credit-notes");
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        private void Cancel()
        {
            Nav.NavigateTo("/vendor-credit-notes");
        }

        #endregion

        #region Validation

        private void ClearErrors()
        {
            HeaderErrors.Clear();
            LineErrors.Clear();
        }

        private bool ValidateForm()
        {
            bool isValid = true;

            // Header validations
            if (CreditNote.BranchId == Guid.Empty)
            {
                HeaderErrors["BranchId"] = "Branch is required.";
                isValid = false;
            }

            if (CreditNote.VendorId == Guid.Empty)
            {
                HeaderErrors["VendorId"] = "Vendor is required.";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(CreditNote.VendorCreditNoteReferenceNumber))
            {
                HeaderErrors["VendorCreditNoteReferenceNumber"] = "Vendor Credit Note Reference Number is required.";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(CreditNote.CreditNoteType))
            {
                HeaderErrors["CreditNoteType"] = "Credit Note Type is required.";
                isValid = false;
            }

            if (CreditNote.VendorCreditNoteDate == default)
            {
                HeaderErrors["VendorCreditNoteDate"] = "Vendor CN Date is required.";
                isValid = false;
            }

            if (CreditNote.CreditEntryDate == default)
            {
                HeaderErrors["CreditEntryDate"] = "Entry Date is required.";
                isValid = false;
            }

            if (CreditNote.IsAgainstBill && CreditNote.PrimaryVendorBillId == null)
            {
                HeaderErrors["PrimaryVendorBillId"] = "Reference Bill is required when 'Against Bill' is enabled.";
                isValid = false;
            }

            // Line validations
            if (!CreditNote.Lines.Any())
            {
                ToastService.ShowWarning("At least one credit note line is required.");
                isValid = false;
            }

            for (int i = 0; i < CreditNote.Lines.Count; i++)
            {
                var line = CreditNote.Lines[i];
                var errors = new Dictionary<string, string>();

                if (string.IsNullOrWhiteSpace(line.Description))
                {
                    errors["Description"] = "Required";
                    isValid = false;
                }

                if (line.Quantity <= 0)
                {
                    errors["Quantity"] = "Must be > 0";
                    isValid = false;
                }

                if (line.ReversalAccountId == Guid.Empty)
                {
                    errors["ReversalAccountId"] = "Required";
                    isValid = false;
                }

                if (errors.Any())
                {
                    LineErrors[i] = errors;
                }
            }

            // Total validation
            if (CreditNote.TotalCreditAmount <= 0 && CreditNote.Lines.Any())
            {
                ToastService.ShowWarning("Total Credit Amount must be greater than zero.");
                isValid = false;
            }

            return isValid;
        }

        private bool HasHeaderError(string field) => HeaderErrors.ContainsKey(field);
        private string GetHeaderError(string field) => HeaderErrors.TryGetValue(field, out var error) ? error : string.Empty;
        private string GetHeaderValidationClass(string field) => HeaderErrors.ContainsKey(field) ? "is-invalid" : "";

        private bool HasValidationError(int lineIndex, string field)
        {
            return LineErrors.TryGetValue(lineIndex, out var errors) && errors.ContainsKey(field);
        }

        private string GetValidationError(int lineIndex, string field)
        {
            if (LineErrors.TryGetValue(lineIndex, out var errors) && errors.TryGetValue(field, out var error))
            {
                return error;
            }
            return string.Empty;
        }

        private string GetValidationClass(int lineIndex, string field)
        {
            return HasValidationError(lineIndex, field) ? "is-invalid" : "";
        }

        #endregion

        #region Helper Methods

        private static string GetStatusBadge(string status) => status switch
        {
            VendorCreditNoteStatuses.Draft => "bg-secondary-transparent text-secondary",
            VendorCreditNoteStatuses.Submitted => "bg-info-transparent text-info",
            VendorCreditNoteStatuses.Approved => "bg-primary-transparent text-primary",
            VendorCreditNoteStatuses.Rejected => "bg-danger-transparent text-danger",
            VendorCreditNoteStatuses.Posted => "bg-success-transparent text-success",
            VendorCreditNoteStatuses.Cancelled => "bg-warning-transparent text-warning",
            VendorCreditNoteStatuses.Reversed => "bg-dark-transparent text-dark",
            _ => "bg-secondary-transparent text-secondary"
        };

        private static string GetSettlementBadge(string status) => status switch
        {
            CreditSettlementStatuses.Unapplied => "bg-warning-transparent text-warning",
            CreditSettlementStatuses.PartiallyApplied => "bg-info-transparent text-info",
            CreditSettlementStatuses.FullyApplied => "bg-success-transparent text-success",
            _ => "bg-secondary-transparent text-secondary"
        };

        #endregion
    }
}
