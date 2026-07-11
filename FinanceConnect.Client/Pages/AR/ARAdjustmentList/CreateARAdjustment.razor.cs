using FinanceConnect.Client.Services;
using FinanceConnect.Client.Shared;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.AR.ARAdjustmentList
{
    public partial class CreateARAdjustment : ComponentBase
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] ARAdjustmentService AdjustmentService { get; set; } = default!;
        [Inject] CustomerInvoiceService InvoiceService { get; set; } = default!;
        [Inject] CustomerService CustomerService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] AuthService AuthService { get; set; } = default!;
        [Inject] IJSRuntime JSRuntime { get; set; } = default!;

        [Parameter] public Guid? Id { get; set; }

        private bool isInitialized = false;
        private ARAdjustmentViewModel Adjustment = new();

        // Dropdown data
        private List<BranchDto> Branches = new();
        private List<CustomerViewModel> Customers = new();
        private List<CustomerInvoiceViewModel> CustomerInvoices = new();
        private List<ARAdjustmentReasonViewModel> AllReasons = new();
        private List<ARAdjustmentReasonViewModel> FilteredReasons = new();
        private List<GLAccountViewModel> OffsetAccounts = new();

        // Rich text editors for line notes
        private Dictionary<int, RichTextEditor> lineEditors = new();

        // Validation errors
        private Dictionary<string, string> LineValidationErrors = new();
        private Dictionary<string, string> HeaderValidationErrors = new();

        private bool IsEdit => Id.HasValue;
        private bool IsReadOnly => Adjustment.AdjustmentStatus != AdjustmentStatuses.Draft;

        protected override async Task OnInitializedAsync()
        {
            // Load master data
            await LoadMasterData();

            if (IsEdit)
            {
                var existing = AdjustmentService.GetById(Id!.Value);
                if (existing != null)
                {
                    Adjustment = existing;
                    // Load invoices for selected customer
                    if (Adjustment.CustomerId != Guid.Empty)
                    {
                        LoadInvoicesForCustomer(Adjustment.CustomerId);
                    }
                    // Filter reasons based on current type
                    FilterReasonsByType(Adjustment.AdjustmentType);
                }
                else
                {
                    ToastService.ShowError("AR Adjustment not found.", "Error");
                    Nav.NavigateTo("/ar-adjustments");
                    return;
                }
            }
            else
            {
                Adjustment = CreateNewAdjustment();
            }

            // Initialize editor dictionary
            InitializeLineEditors();

            isInitialized = true;
        }

        private void InitializeLineEditors()
        {
            lineEditors.Clear();
            for (int i = 0; i < Adjustment.Lines.Count; i++)
            {
                lineEditors[i] = null!;
            }
        }

        private async Task LoadMasterData()
        {
            // Load branches
            Branches = new List<BranchDto>
            {
                new BranchDto { Id = "b0b0b0b0-b0b0-b0b0-b0b0-b0b0b0b0b001", Name = "Head Office - Chennai" },
                new BranchDto { Id = "b0b0b0b0-b0b0-b0b0-b0b0-b0b0b0b0b002", Name = "Branch - Bangalore" },
                new BranchDto { Id = "b0b0b0b0-b0b0-b0b0-b0b0-b0b0b0b0b003", Name = "Branch - Mumbai" }
            };

            // Load active customers
            Customers = CustomerService.GetAll().Where(c => c.CustomerStatus == CustomerStatuses.Active).ToList();

            // Load all reason codes
            AllReasons = AdjustmentService.GetAllReasons().ToList();
            FilteredReasons = AllReasons.ToList();

            // Load offset accounts (expense/write-off accounts)
            OffsetAccounts = new List<GLAccountViewModel>
            {
                new GLAccountViewModel { Id = Guid.Parse("00000000-0000-0000-0000-000000000020"), Code = "5200", Name = "Bad Debts Written Off" },
                new GLAccountViewModel { Id = Guid.Parse("00000000-0000-0000-0000-000000000021"), Code = "5210", Name = "Rounding Difference" },
                new GLAccountViewModel { Id = Guid.Parse("00000000-0000-0000-0000-000000000022"), Code = "5220", Name = "Discount Allowed" },
                new GLAccountViewModel { Id = Guid.Parse("00000000-0000-0000-0000-000000000023"), Code = "5230", Name = "Sales Adjustments" },
                new GLAccountViewModel { Id = Guid.Parse("00000000-0000-0000-0000-000000000024"), Code = "5240", Name = "Dispute Settlements" },
                new GLAccountViewModel { Id = Guid.Parse("00000000-0000-0000-0000-000000000025"), Code = "5250", Name = "Bad Debt Provision" },
                new GLAccountViewModel { Id = Guid.Parse("00000000-0000-0000-0000-000000000026"), Code = "5260", Name = "Short Payment Adjustments" }
            };

            await Task.CompletedTask;
        }

        private void LoadInvoicesForCustomer(Guid customerId)
        {
            CustomerInvoices = InvoiceService.GetByCustomerId(customerId)
                .Where(i => (i.InvoiceStatus == InvoiceStatuses.Posted ||
                            i.InvoiceStatus == InvoiceStatuses.PartiallyPaid) &&
                            i.AmountOutstanding > 0)
                .OrderByDescending(i => i.InvoiceDate)
                .ToList();
        }

        private void FilterReasonsByType(string adjustmentType)
        {
            FilteredReasons = AllReasons
                .Where(r => r.ApplicableTypes.Contains(adjustmentType) || !r.ApplicableTypes.Any())
                .ToList();
        }

        private ARAdjustmentViewModel CreateNewAdjustment()
        {
            var adjustmentNumber = AdjustmentService.GenerateAdjustmentNumber(
                Guid.Parse("c0c0c0c0-c0c0-c0c0-c0c0-c0c0c0c0c001"));

            var newAdjustment = new ARAdjustmentViewModel
            {
                CompanyId = Guid.Parse("c0c0c0c0-c0c0-c0c0-c0c0-c0c0c0c0c001"),
                CompanyName = "Ascending Software Private Limited",
                BranchId = Guid.Parse("b0b0b0b0-b0b0-b0b0-b0b0-b0b0b0b0b001"),
                BranchName = "Head Office - Chennai",
                AdjustmentNumber = adjustmentNumber,
                AdjustmentDate = DateTime.Today,
                AdjustmentStatus = AdjustmentStatuses.Draft,
                AdjustmentType = string.Empty,
                AdjustmentDirection = AdjustmentDirections.ReduceAR,
                CurrencyId = Guid.Parse("a0a0a0a0-a0a0-a0a0-a0a0-a0a0a0a0a001"),
                CurrencyCode = "INR",
                CurrencyName = "Indian Rupee",
                CreatedBy = AuthService.CurrentUser?.UserName ?? "Current User",
                CreatedAt = DateTime.Now
            };

            // Add one default line
            newAdjustment.Lines.Add(CreateNewLine(1));

            // Filter reasons for default type
            FilterReasonsByType(newAdjustment.AdjustmentType);

            return newAdjustment;
        }

        private ARAdjustmentLineViewModel CreateNewLine(int lineNumber)
        {
            Guid defaultOffsetAccountId = GetDefaultOffsetAccountId(Adjustment.AdjustmentType);

            return new ARAdjustmentLineViewModel
            {
                Id = Guid.NewGuid(),
                ARAdjustmentId = Adjustment.Id,
                LineNumber = lineNumber,
                LineType = string.Empty,
                AdjustmentAmount = 0,
                OffsetAccountId = defaultOffsetAccountId,
                OffsetAccountCode = OffsetAccounts.FirstOrDefault(a => a.Id == defaultOffsetAccountId)?.Code,
                OffsetAccountName = OffsetAccounts.FirstOrDefault(a => a.Id == defaultOffsetAccountId)?.Name,
                CreatedBy = AuthService.CurrentUser?.UserName ?? "Current User",
                CreatedAt = DateTime.Now
            };
        }

        private Guid GetDefaultOffsetAccountId(string adjustmentType)
        {
            return adjustmentType switch
            {
                AdjustmentTypes.WriteOff => Guid.Parse("00000000-0000-0000-0000-000000000020"),
                AdjustmentTypes.Rounding => Guid.Parse("00000000-0000-0000-0000-000000000021"),
                AdjustmentTypes.DisputeSettlement => Guid.Parse("00000000-0000-0000-0000-000000000024"),
                AdjustmentTypes.ShortPaymentSettlement => Guid.Parse("00000000-0000-0000-0000-000000000026"),
                AdjustmentTypes.BadDebtProvision => Guid.Parse("00000000-0000-0000-0000-000000000025"),
                _ => Guid.Parse("00000000-0000-0000-0000-000000000023")
            };
        }

        /// <summary>
        /// Extract HTML content from all Quill editors and update line narrations
        /// </summary>
        private async Task SyncLineEditorsAsync()
        {
            for (int i = 0; i < Adjustment.Lines.Count; i++)
            {
                if (lineEditors.TryGetValue(i, out var editor) && editor != null)
                {
                    var html = await editor.GetHtmlAsync();
                    // Only set if not empty quill default
                    if (html != "<p><br></p>" && !string.IsNullOrWhiteSpace(html))
                    {
                        Adjustment.Lines[i].LineNarration = html;
                    }
                    else
                    {
                        Adjustment.Lines[i].LineNarration = null;
                    }
                }
            }
        }

        #region Event Handlers

        private void OnCustomerChanged()
        {
            if (Adjustment.CustomerId != Guid.Empty)
            {
                var customer = Customers.FirstOrDefault(c => c.Id == Adjustment.CustomerId);
                if (customer != null)
                {
                    Adjustment.CustomerCode = customer.CustomerCode;
                    Adjustment.CustomerName = customer.CustomerName;
                    Adjustment.CurrencyId = customer.DefaultCurrencyId ?? Guid.Parse("a0a0a0a0-a0a0-a0a0-a0a0-a0a0a0a0a001");
                    Adjustment.CurrencyCode = customer.DefaultCurrencyCode ?? "INR";
                    Adjustment.CurrencyName = customer.DefaultCurrencyCode ?? "Indian Rupee";

                    LoadInvoicesForCustomer(customer.Id);

                    foreach (var line in Adjustment.Lines)
                    {
                        line.CustomerInvoiceId = null;
                        line.CustomerInvoiceNumber = null;
                        line.InvoiceOutstanding = null;
                    }
                }
            }
            else
            {
                Adjustment.CustomerCode = null;
                Adjustment.CustomerName = null;
                CustomerInvoices.Clear();
            }

            ClearHeaderError("CustomerId");
        }

        private void OnTypeChanged()
        {
            FilterReasonsByType(Adjustment.AdjustmentType);

            if (Adjustment.ReasonCodeId != Guid.Empty)
            {
                var currentReason = FilteredReasons.FirstOrDefault(r => r.Id == Adjustment.ReasonCodeId);
                if (currentReason == null)
                {
                    Adjustment.ReasonCodeId = Guid.Empty;
                    Adjustment.ReasonCode = null;
                    Adjustment.ReasonDescription = null;
                    Adjustment.RequiresApproval = false;
                    Adjustment.EvidenceRequired = false;
                }
            }

            var defaultLineType = Adjustment.AdjustmentType switch
            {
                AdjustmentTypes.WriteOff => AdjustmentLineTypes.WriteOff,
                AdjustmentTypes.Rounding => AdjustmentLineTypes.Rounding,
                AdjustmentTypes.DisputeSettlement => AdjustmentLineTypes.Dispute,
                AdjustmentTypes.Reclassification => AdjustmentLineTypes.Reclassification,
                _ => AdjustmentLineTypes.Other
            };

            foreach (var line in Adjustment.Lines.Where(l => l.AdjustmentAmount == 0))
            {
                line.LineType = defaultLineType;
            }

            ClearHeaderError("AdjustmentType");
        }

        private void OnReasonChanged()
        {
            if (Adjustment.ReasonCodeId != Guid.Empty)
            {
                var reason = AllReasons.FirstOrDefault(r => r.Id == Adjustment.ReasonCodeId);
                if (reason != null)
                {
                    Adjustment.ReasonCode = reason.ReasonCode;
                    Adjustment.ReasonDescription = reason.ReasonDescription;
                    Adjustment.RequiresApproval = reason.RequiresApproval;
                    Adjustment.EvidenceRequired = reason.RequiresEvidence;

                    if (reason.DefaultOffsetAccountId.HasValue)
                    {
                        var account = OffsetAccounts.FirstOrDefault(a => a.Id == reason.DefaultOffsetAccountId.Value);
                        if (account != null)
                        {
                            foreach (var line in Adjustment.Lines.Where(l => l.OffsetAccountId == Guid.Empty))
                            {
                                line.OffsetAccountId = account.Id;
                                line.OffsetAccountCode = account.Code;
                                line.OffsetAccountName = account.Name;
                            }
                        }
                    }

                    if (reason.ApprovalThreshold.HasValue && Adjustment.TotalAdjustmentAmount > reason.ApprovalThreshold.Value)
                    {
                        Adjustment.RequiresApproval = true;
                    }
                }
            }
            else
            {
                Adjustment.ReasonCode = null;
                Adjustment.ReasonDescription = null;
                Adjustment.RequiresApproval = false;
                Adjustment.EvidenceRequired = false;
            }

            ClearHeaderError("ReasonCodeId");
        }

        private void OnInvoiceSelected(ARAdjustmentLineViewModel line)
        {
            if (line.CustomerInvoiceId.HasValue && line.CustomerInvoiceId != Guid.Empty)
            {
                var invoice = CustomerInvoices.FirstOrDefault(i => i.Id == line.CustomerInvoiceId);
                if (invoice != null)
                {
                    line.CustomerInvoiceNumber = invoice.InvoiceNumber;
                    line.InvoiceOutstanding = invoice.AmountOutstanding;

                    if (line.AdjustmentAmount == 0)
                    {
                        line.AdjustmentAmount = invoice.AmountOutstanding;
                        RecalculateTotals();
                    }
                }
            }
            else
            {
                line.CustomerInvoiceNumber = null;
                line.InvoiceOutstanding = null;
            }

            var lineIndex = Adjustment.Lines.IndexOf(line);
            if (lineIndex >= 0)
            {
                ClearLineErrors(lineIndex);
            }
        }

        #endregion

        #region Line Operations

        private void AddLine()
        {
            var newLineNumber = Adjustment.Lines.Any() ? Adjustment.Lines.Max(l => l.LineNumber) + 1 : 1;
            Adjustment.Lines.Add(CreateNewLine(newLineNumber));
            // Add editor slot
            lineEditors[Adjustment.Lines.Count - 1] = null!;
        }

        private void RemoveLine(ARAdjustmentLineViewModel line)
        {
            if (Adjustment.Lines.Count > 1)
            {
                Adjustment.Lines.Remove(line);
                RenumberLines();
                RecalculateTotals();
                InitializeLineEditors();
            }
        }

        private void DuplicateLine(ARAdjustmentLineViewModel line)
        {
            var newLine = new ARAdjustmentLineViewModel
            {
                Id = Guid.NewGuid(),
                ARAdjustmentId = Adjustment.Id,
                LineNumber = Adjustment.Lines.Max(l => l.LineNumber) + 1,
                LineType = line.LineType,
                CustomerInvoiceId = line.CustomerInvoiceId,
                CustomerInvoiceNumber = line.CustomerInvoiceNumber,
                InvoiceOutstanding = line.InvoiceOutstanding,
                AdjustmentAmount = line.AdjustmentAmount,
                OffsetAccountId = line.OffsetAccountId,
                OffsetAccountCode = line.OffsetAccountCode,
                OffsetAccountName = line.OffsetAccountName,
                LineNarration = line.LineNarration,
                CreatedBy = AuthService.CurrentUser?.UserName ?? "Current User",
                CreatedAt = DateTime.Now
            };

            Adjustment.Lines.Add(newLine);
            RecalculateTotals();
            lineEditors[Adjustment.Lines.Count - 1] = null!;
        }

        private void RenumberLines()
        {
            for (int i = 0; i < Adjustment.Lines.Count; i++)
            {
                Adjustment.Lines[i].LineNumber = i + 1;
            }
        }

        private void RecalculateTotals()
        {
            Adjustment.RecalculateTotals();

            if (Adjustment.ReasonCodeId != Guid.Empty)
            {
                var reason = AllReasons.FirstOrDefault(r => r.Id == Adjustment.ReasonCodeId);
                if (reason?.ApprovalThreshold.HasValue == true && Adjustment.TotalAdjustmentAmount > reason.ApprovalThreshold.Value)
                {
                    Adjustment.RequiresApproval = true;
                }
            }
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

        private bool ValidateHeader()
        {
            HeaderValidationErrors.Clear();
            var isValid = true;

            if (Adjustment.CustomerId == Guid.Empty)
            {
                SetHeaderError("CustomerId", "Customer is required");
                isValid = false;
            }

            if (Adjustment.BranchId == Guid.Empty)
            {
                SetHeaderError("BranchId", "Branch is required");
                isValid = false;
            }

            if (Adjustment.AdjustmentDate == default)
            {
                SetHeaderError("AdjustmentDate", "Adjustment Date is required");
                isValid = false;
            }

            if (string.IsNullOrEmpty(Adjustment.AdjustmentType))
            {
                SetHeaderError("AdjustmentType", "Adjustment Type is required");
                isValid = false;
            }

            if (Adjustment.ReasonCodeId == Guid.Empty)
            {
                SetHeaderError("ReasonCodeId", "Reason Code is required");
                isValid = false;
            }

            if (Adjustment.IsNarrationRequired && string.IsNullOrWhiteSpace(Adjustment.AdjustmentNarration))
            {
                SetHeaderError("AdjustmentNarration", "Narration is required for this adjustment type");
                isValid = false;
            }

            if (Adjustment.PostingDate.HasValue && Adjustment.PostingDate.Value < Adjustment.AdjustmentDate)
            {
                SetHeaderError("PostingDate", "Posting Date must be on or after Adjustment Date");
                isValid = false;
            }

            return isValid;
        }

        #endregion

        #region Line Validation

        private void ValidateLine(ARAdjustmentLineViewModel line)
        {
            var lineIndex = Adjustment.Lines.IndexOf(line);
            if (lineIndex < 0) return;

            ClearLineErrors(lineIndex);

            if (line.AdjustmentAmount <= 0)
            {
                SetLineError(lineIndex, "AdjustmentAmount", "Amount must be > 0");
            }

            if (line.OffsetAccountId == Guid.Empty)
            {
                SetLineError(lineIndex, "OffsetAccountId", "Account required");
            }

            if (line.CustomerInvoiceId.HasValue && line.InvoiceOutstanding.HasValue)
            {
                if (line.AdjustmentAmount > line.InvoiceOutstanding.Value)
                {
                    SetLineError(lineIndex, "AdjustmentAmount", $"Cannot exceed outstanding: {line.InvoiceOutstanding:N2}");
                }
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

            if (!Adjustment.Lines.Any())
            {
                ToastService.ShowWarning("At least one line is required.");
                return false;
            }

            for (int i = 0; i < Adjustment.Lines.Count; i++)
            {
                var line = Adjustment.Lines[i];

                if (string.IsNullOrEmpty(line.LineType))
                {
                    SetLineError(i, "LineType", "Line Type is required");
                    isValid = false;
                }

                if (line.AdjustmentAmount <= 0)
                {
                    SetLineError(i, "AdjustmentAmount", "Amount must be > 0");
                    isValid = false;
                }

                if (line.OffsetAccountId == Guid.Empty)
                {
                    SetLineError(i, "OffsetAccountId", "Account required");
                    isValid = false;
                }

                if (line.CustomerInvoiceId.HasValue && line.InvoiceOutstanding.HasValue)
                {
                    if (line.AdjustmentAmount > line.InvoiceOutstanding.Value)
                    {
                        SetLineError(i, "AdjustmentAmount", $"Cannot exceed outstanding: {line.InvoiceOutstanding:N2}");
                        isValid = false;
                    }
                }
            }

            return isValid;
        }

        private async Task ScrollToFirstValidationError()
        {
            await Task.Delay(100);
            await JSRuntime.InvokeVoidAsync("scrollToFirstValidationError");
        }

        #endregion

        #region Form Submission

        private async Task HandleSubmit(EditContext context)
        {
            // Sync Quill editor content to line models
            await SyncLineEditorsAsync();

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

            if (Adjustment.AdjustmentDate > Adjustment.PostingDate)
            {
                ToastService.ShowError("Posting Date should not be earlier than Adjustment Date");
                return;
            }

            RecalculateTotals();

            if (Adjustment.TotalAdjustmentAmount <= 0)
            {
                ToastService.ShowWarning("Adjustment total must be greater than zero.");
                return;
            }

            var result = IsEdit
                ? AdjustmentService.Update(Adjustment)
                : AdjustmentService.Add(Adjustment);

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                Nav.NavigateTo("/ar-adjustments");
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }


        private async Task SubmitAdjustment()
        {
            // Sync Quill editors
            await SyncLineEditorsAsync();

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
            if (Adjustment.AdjustmentDate > Adjustment.PostingDate)
            {

                ToastService.ShowError("Posting Date should not be earlier then Adjustment Date");
                return;
            }
            RecalculateTotals();

            if (Adjustment.TotalAdjustmentAmount <= 0)
            {
                ToastService.ShowWarning("Adjustment total must be greater than zero to submit.");
                return;
            }

            if (Adjustment.EvidenceRequired && Adjustment.EvidenceAttachmentCount == 0)
            {
                ToastService.ShowWarning("Evidence/documentation is required before submitting. Please attach supporting documents.");
                return;
            }

            if (!IsEdit)
            {
                var saveResult = AdjustmentService.Add(Adjustment);
                if (!saveResult.Success)
                {
                    ToastService.ShowError(saveResult.Message);
                    return;
                }
            }
            else
            {
                var updateResult = AdjustmentService.Update(Adjustment);
                if (!updateResult.Success)
                {
                    ToastService.ShowError(updateResult.Message);
                    return;
                }
            }

            var result = AdjustmentService.Submit(Adjustment.Id, AuthService.CurrentUser?.UserName ?? "Current User");
            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                Nav.NavigateTo("/ar-adjustments");
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        private async Task PostAdjustment()
        {
            if (!Adjustment.CanPost)
            {
                ToastService.ShowWarning("This adjustment cannot be posted in its current status.");
                return;
            }

            // Sync Quill editors
            await SyncLineEditorsAsync();

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

            if (Adjustment.TotalAdjustmentAmount <= 0)
            {
                ToastService.ShowWarning("Adjustment total must be greater than zero to post.");
                return;
            }

            var saveResult = IsEdit ? AdjustmentService.Update(Adjustment) : AdjustmentService.Add(Adjustment);
            if (!saveResult.Success)
            {
                ToastService.ShowError(saveResult.Message);
                return;
            }

            var result = AdjustmentService.Post(
                Adjustment.Id,
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                AuthService.CurrentUser?.UserName ?? "Current User");

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                Nav.NavigateTo("/ar-adjustments");
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        private void OnCustomerChanged(string? value)
        {
            if (Guid.TryParse(value, out var id))
                Adjustment.CustomerId = id;
            else
                Adjustment.CustomerId = Guid.Empty;
        }
        #endregion

        #region Helper Methods

        private string GetStatusBadgeClass(string status) => status switch
        {
            AdjustmentStatuses.Draft => "bg-secondary",
            AdjustmentStatuses.Submitted => "bg-info",
            AdjustmentStatuses.Approved => "bg-primary",
            AdjustmentStatuses.Posted => "bg-success",
            AdjustmentStatuses.Cancelled => "bg-warning text-dark",
            AdjustmentStatuses.Reversed => "bg-danger",
            _ => "bg-secondary"
        };

        private string GetTypeBadgeClass(string type) => type switch
        {
            AdjustmentTypes.WriteOff => "bg-danger",
            AdjustmentTypes.Rounding => "bg-info",
            AdjustmentTypes.DisputeSettlement => "bg-warning text-dark",
            AdjustmentTypes.ShortPaymentSettlement => "bg-secondary",
            AdjustmentTypes.Reclassification => "bg-primary",
            AdjustmentTypes.BadDebtProvision => "bg-dark",
            AdjustmentTypes.Other => "bg-secondary",
            _ => "bg-secondary"
        };

        private string GetDirectionBadgeClass(string direction) => direction switch
        {
            AdjustmentDirections.ReduceAR => "bg-success",
            AdjustmentDirections.IncreaseAR => "bg-warning text-dark",
            _ => "bg-secondary"
        };

        private string GetLineTypeBadgeClass(string lineType) => lineType switch
        {
            AdjustmentLineTypes.WriteOff => "bg-danger",
            AdjustmentLineTypes.Rounding => "bg-info",
            AdjustmentLineTypes.DiscountAllowed => "bg-success",
            AdjustmentLineTypes.Dispute => "bg-warning text-dark",
            AdjustmentLineTypes.Reclassification => "bg-primary",
            AdjustmentLineTypes.Other => "bg-secondary",
            _ => "bg-secondary"
        };

        #endregion
    }
}
