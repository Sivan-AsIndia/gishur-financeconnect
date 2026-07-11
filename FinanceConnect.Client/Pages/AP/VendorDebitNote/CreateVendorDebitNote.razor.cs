using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using FinanceConnect.Client.Shared;

namespace FinanceConnect.Client.Pages.AP.VendorDebitNote
{
    public partial class CreateVendorDebitNote
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] VendorDebitNoteService DebitNoteService { get; set; } = default!;
        [Inject] VendorBillService BillService { get; set; } = default!;
        [Inject] VendorService VendorService { get; set; } = default!;
        [Inject] BranchService BranchService { get; set; } = default!;
        [Inject] COADataService COADataService { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] IJSRuntime JSRuntime { get; set; } = default!;

        [Parameter] public Guid? Id { get; set; }

        private bool isInitialized = false;
        private RichTextEditor? narrationEditor;
        private VendorDebitNoteViewModel DebitNote = new();

        // Dropdown data
        private List<BranchModel> Branches = new();
        private List<VendorViewModel> Vendors = new();
        private List<VendorBillViewModel> PostedBills = new();
        private List<AccountViewModel> ExpenseAccounts = new();
        private List<StateProvinceModel> States = new();

        // Validation errors
        private Dictionary<string, string> LineValidationErrors = new();
        private Dictionary<string, string> HeaderValidationErrors = new();

        private bool IsEdit => Id.HasValue;
        private bool IsReadOnly => DebitNote.DebitNoteStatus != VendorDebitNoteStatuses.Draft;

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
                    // Load posted bills for selected vendor
                    if (DebitNote.VendorId != Guid.Empty)
                    {
                        LoadPostedBillsForVendor(DebitNote.VendorId);
                    }
                }
                else
                {
                    ToastService.ShowError("Debit Note not found.", "Error");
                    Nav.NavigateTo("/vendor-debit-notes");
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
            // Load branches from BranchService
            Branches = BranchService.GetAll()
                .Where(b => b.Status == "Active")
                .ToList();

            // Load active vendors
            Vendors = VendorService.GetAll().Where(v => v.VendorStatus == VendorStatuses.Active).ToList();

            // Load expense accounts from COADataService
            ExpenseAccounts = COADataService.GetAllAccounts();

            // Load states for Place of Supply
            States = MasterDataService.GetAllStateProvinces()
                .Where(s => s.IsActive)
                .OrderBy(s => s.StateProvinceName).ToList();

            await Task.CompletedTask;
        }

        private void LoadPostedBillsForVendor(Guid vendorId)
        {
            // Get posted bills for the selected vendor that have outstanding amounts
            // BillStatus is the workflow status (Draft/Submitted/Approved/Posted etc.)
            // SettlementStatus is the payment status (Unpaid/PartiallyPaid/Paid)
            PostedBills = BillService.GetByVendorId(vendorId)
                .Where(b => b.BillStatus == VendorBillStatuses.Posted && 
                           (b.SettlementStatus == SettlementStatuses.Unpaid || 
                            b.SettlementStatus == SettlementStatuses.PartiallyPaid))
                .OrderByDescending(b => b.BillDate)
                .ToList();
        }

        private VendorDebitNoteViewModel CreateNewDebitNote()
        {
            var defaultCompany = MasterDataService.GetAllCompanies().FirstOrDefault(c => c.Status == "Active");
            var defaultBranch = Branches.FirstOrDefault();

            var debitNoteNumber = DebitNoteService.GenerateDebitNoteNumber(
                defaultCompany?.Id ?? Guid.Empty);

            var defaultAccount = ExpenseAccounts.FirstOrDefault();

            var newDebitNote = new VendorDebitNoteViewModel
            {
                CompanyId = defaultCompany?.Id ?? Guid.Empty,
                CompanyName = defaultCompany?.LegalName,
                BranchId = Guid.Empty,
                BranchName = null,
                DebitNoteNumber = debitNoteNumber,
                VendorDebitNoteDate = DateTime.Today,
                DebitEntryDate = DateTime.Today,
                DebitNoteStatus = VendorDebitNoteStatuses.Draft,
                DebitNoteType = "",
                ExchangeRate = 1,
                CurrencyId = Guid.Parse("d1d1d1d1-d1d1-d1d1-d1d1-d1d1d1d1d101"),
                CurrencyCode = "INR",
                CurrencyName = "Indian Rupee",
                IsAgainstBill = true,
                IsGSTApplicable = true,
                IsReverseChargeApplicable = false,
                CreatedBy = "Current User",
                CreatedAt = DateTime.Now
            };

            // Add one default line
            newDebitNote.Lines.Add(CreateNewLine(10));

            return newDebitNote;
        }

        private VendorDebitNoteLineViewModel CreateNewLine(int lineNumber)
        {
            var defaultAccount = ExpenseAccounts.FirstOrDefault();
            return new VendorDebitNoteLineViewModel
            {
                Id = Guid.NewGuid(),
                VendorDebitNoteId = DebitNote.Id,
                LineNumber = lineNumber,
                LineType = VendorDebitNoteLineTypes.Expense,
                Quantity = 1,
                UnitPrice = 0,
                DiscountPercent = 0,
                TaxRatePercent = 18, // Default GST rate
                ExpenseOrAssetAccountId = defaultAccount?.Id ?? Guid.Empty
            };
        }

        #region Event Handlers

        private void OnVendorChanged()
        {
            if (DebitNote.VendorId != Guid.Empty)
            {
                var vendor = Vendors.FirstOrDefault(v => v.Id == DebitNote.VendorId);
                if (vendor != null)
                {
                    DebitNote.VendorCode = vendor.VendorCode;
                    DebitNote.VendorName = vendor.VendorName;
                    DebitNote.CurrencyId = vendor.DefaultCurrencyId ?? Guid.Parse("d1d1d1d1-d1d1-d1d1-d1d1-d1d1d1d1d101");
                    DebitNote.CurrencyCode = vendor.DefaultCurrencyCode ?? "INR";

                    // Load posted bills for this vendor
                    LoadPostedBillsForVendor(vendor.Id);

                    // Clear bill reference if vendor changed
                    DebitNote.PrimaryVendorBillId = null;
                    DebitNote.PrimaryVendorBillNumber = null;
                    DebitNote.BillNumberSnapshot = null;
                    DebitNote.BillDateSnapshot = null;
                }
            }
            else
            {
                DebitNote.VendorCode = null;
                DebitNote.VendorName = null;
                PostedBills.Clear();
            }

            ClearHeaderError("VendorId");
        }

        private void OnAgainstBillChanged()
        {
            if (!DebitNote.IsAgainstBill)
            {
                // Clear bill reference
                DebitNote.PrimaryVendorBillId = null;
                DebitNote.PrimaryVendorBillNumber = null;
                DebitNote.BillNumberSnapshot = null;
                DebitNote.BillDateSnapshot = null;
            }
            ClearHeaderError("PrimaryVendorBillId");
        }

        private void OnBillSelected()
        {
            if (DebitNote.PrimaryVendorBillId.HasValue && DebitNote.PrimaryVendorBillId != Guid.Empty)
            {
                var bill = PostedBills.FirstOrDefault(b => b.Id == DebitNote.PrimaryVendorBillId);
                if (bill != null)
                {
                    DebitNote.BillNumberSnapshot = bill.BillNumber;
                    DebitNote.BillDateSnapshot = bill.BillDate;
                    DebitNote.PrimaryVendorBillNumber = bill.BillNumber;
                }
            }
            else
            {
                DebitNote.BillNumberSnapshot = null;
                DebitNote.BillDateSnapshot = null;
                DebitNote.PrimaryVendorBillNumber = null;
            }

            ClearHeaderError("PrimaryVendorBillId");
        }

        private void OnTypeChanged()
        {
            // Update any type-specific defaults if needed
            ClearHeaderError("DebitNoteType");
        }

        #endregion

        #region Line Management

        private void AddLine()
        {
            var maxLineNumber = DebitNote.Lines.Any() ? DebitNote.Lines.Max(l => l.LineNumber) : 0;
            var newLine = CreateNewLine(maxLineNumber + 10);
            DebitNote.Lines.Add(newLine);
        }

        private void RemoveLine(VendorDebitNoteLineViewModel line)
        {
            if (DebitNote.Lines.Count > 1)
            {
                DebitNote.Lines.Remove(line);
                RecalculateTotals();
            }
        }

        private void DuplicateLine(VendorDebitNoteLineViewModel line)
        {
            var maxLineNumber = DebitNote.Lines.Max(l => l.LineNumber);
            var newLine = new VendorDebitNoteLineViewModel
            {
                Id = Guid.NewGuid(),
                VendorDebitNoteId = DebitNote.Id,
                LineNumber = maxLineNumber + 10,
                LineType = line.LineType,
                Description = line.Description,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice,
                DiscountPercent = line.DiscountPercent,
                TaxRatePercent = line.TaxRatePercent,
                ExpenseOrAssetAccountId = line.ExpenseOrAssetAccountId,
                ExpenseOrAssetAccountCode = line.ExpenseOrAssetAccountCode,
                ExpenseOrAssetAccountName = line.ExpenseOrAssetAccountName,
                HSNCode = line.HSNCode,
                SACCode = line.SACCode
            };
            newLine.RecalculateAmounts();
            DebitNote.Lines.Add(newLine);
            RecalculateTotals();
        }

        private void RecalculateLine(VendorDebitNoteLineViewModel line)
        {
            line.RecalculateAmounts();
            RecalculateTotals();
        }

        private void RecalculateTotals()
        {
            DebitNote.RecalculateTotals();
        }

        #endregion

        #region Validation

        private void ValidateLine(VendorDebitNoteLineViewModel line)
        {
            var lineIndex = DebitNote.Lines.IndexOf(line);
            
            // Clear existing errors for this line
            var keysToRemove = LineValidationErrors.Keys.Where(k => k.StartsWith($"{lineIndex}_")).ToList();
            foreach (var key in keysToRemove)
            {
                LineValidationErrors.Remove(key);
            }

            // Validate description
            if (string.IsNullOrWhiteSpace(line.Description))
            {
                LineValidationErrors[$"{lineIndex}_Description"] = "Description is required";
            }
        }

        private bool ValidateHeader()
        {
            HeaderValidationErrors.Clear();
            bool isValid = true;

            if (DebitNote.BranchId == Guid.Empty)
            {
                HeaderValidationErrors["BranchId"] = "Branch is required.";
                isValid = false;
            }

            if (DebitNote.VendorId == Guid.Empty)
            {
                HeaderValidationErrors["VendorId"] = "Vendor is required.";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(DebitNote.VendorDebitNoteReferenceNumber))
            {
                HeaderValidationErrors["VendorDebitNoteReferenceNumber"] = "Vendor reference number is required.";
                isValid = false;
            }
            else if (DebitNoteService.VendorReferenceNumberExists(DebitNote.VendorId, DebitNote.VendorDebitNoteReferenceNumber, IsEdit ? DebitNote.Id : null))
            {
                HeaderValidationErrors["VendorDebitNoteReferenceNumber"] = "This reference number already exists for this vendor.";
                isValid = false;
            }

            if (DebitNote.VendorDebitNoteDate == default)
            {
                HeaderValidationErrors["VendorDebitNoteDate"] = "Vendor debit note date is required.";
                isValid = false;
            }

            if (DebitNote.DebitEntryDate == default)
            {
                HeaderValidationErrors["DebitEntryDate"] = "Entry date is required.";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(DebitNote.DebitNoteType))
            {
                HeaderValidationErrors["DebitNoteType"] = "Debit note type is required.";
                isValid = false;
            }

            if (DebitNote.IsAgainstBill && (!DebitNote.PrimaryVendorBillId.HasValue || DebitNote.PrimaryVendorBillId == Guid.Empty))
            {
                HeaderValidationErrors["PrimaryVendorBillId"] = "Reference bill is required when 'Against Bill' is enabled.";
                isValid = false;
            }

            return isValid;
        }

        private bool ValidateAllLines()
        {
            LineValidationErrors.Clear();
            bool isValid = true;

            if (!DebitNote.Lines.Any())
            {
                ToastService.ShowWarning("At least one line is required.");
                return false;
            }

            for (int i = 0; i < DebitNote.Lines.Count; i++)
            {
                var line = DebitNote.Lines[i];

                if (string.IsNullOrWhiteSpace(line.Description))
                {
                    SetLineError(i, "Description", "Description required");
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

                if (line.ExpenseOrAssetAccountId == Guid.Empty)
                {
                    SetLineError(i, "ExpenseOrAssetAccountId", "Account required");
                    isValid = false;
                }
            }

            return isValid;
        }

        private void SetLineError(int lineIndex, string field, string message)
        {
            LineValidationErrors[$"{lineIndex}_{field}"] = message;
        }

        private bool HasValidationError(int lineIndex, string field)
        {
            return LineValidationErrors.ContainsKey($"{lineIndex}_{field}");
        }

        private string GetValidationError(int lineIndex, string field)
        {
            return LineValidationErrors.TryGetValue($"{lineIndex}_{field}", out var error) ? error : string.Empty;
        }

        private string GetValidationClass(int lineIndex, string field)
        {
            return HasValidationError(lineIndex, field) ? "is-invalid" : string.Empty;
        }

        private bool HasHeaderError(string field)
        {
            return HeaderValidationErrors.ContainsKey(field);
        }

        private string GetHeaderError(string field)
        {
            return HeaderValidationErrors.TryGetValue(field, out var error) ? error : string.Empty;
        }

        private void ClearHeaderError(string field)
        {
            HeaderValidationErrors.Remove(field);
        }

        private async Task ScrollToFirstValidationError()
        {
            await JSRuntime.InvokeVoidAsync("scrollToFirstValidationError");
        }

        #endregion

        #region Form Submission

        private async Task HandleValidSubmit()
        {
            // Get narration from rich text editor
            if (narrationEditor != null)
                DebitNote.DebitNoteNarration = await narrationEditor.GetHtmlAsync();

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

            if (DebitNote.TotalDebitAmount <= 0)
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
                Nav.NavigateTo("/vendor-debit-notes");
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

            RecalculateTotals();

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
                Nav.NavigateTo("/vendor-debit-notes");
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

            if (DebitNote.TotalDebitAmount <= 0)
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
                Nav.NavigateTo("/vendor-debit-notes");
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
            VendorDebitNoteStatuses.Draft => "bg-secondary",
            VendorDebitNoteStatuses.Submitted => "bg-info",
            VendorDebitNoteStatuses.Approved => "bg-primary",
            VendorDebitNoteStatuses.Posted => "bg-success",
            VendorDebitNoteStatuses.Cancelled => "bg-warning text-dark",
            VendorDebitNoteStatuses.Reversed => "bg-danger",
            _ => "bg-secondary"
        };

        private string GetLineTypeBadgeClass(string lineType) => lineType switch
        {
            VendorDebitNoteLineTypes.Expense => "bg-primary",
            VendorDebitNoteLineTypes.Asset => "bg-info",
            VendorDebitNoteLineTypes.Service => "bg-warning text-dark",
            VendorDebitNoteLineTypes.Charge => "bg-danger",
            _ => "bg-secondary"
        };

        private static string GetSettlementBadge(string status) => status switch
        {
            VendorDebitNoteSettlementStatuses.Unapplied => "bg-warning-transparent text-warning",
            VendorDebitNoteSettlementStatuses.PartiallyApplied => "bg-info-transparent text-info",
            VendorDebitNoteSettlementStatuses.FullyApplied => "bg-success-transparent text-success",
            _ => "bg-secondary-transparent text-secondary"
        };

        #endregion
    }
}
