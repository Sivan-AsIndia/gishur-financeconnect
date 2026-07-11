using FinanceConnect.Client.Data;
using FinanceConnect.Client.Services;
using FinanceConnect.Client.Shared;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Linq.Expressions;
using System.Reflection;

namespace FinanceConnect.Client.Pages.AP.APAdjustment
{
    public partial class CreateAPAdjustment : ComponentBase
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] APAdjustmentService AdjustmentService { get; set; } = default!;
        [Inject] VendorBillService BillService { get; set; } = default!;
        [Inject] VendorService VendorService { get; set; } = default!;
        [Inject] COADataService COADataService { get; set; } = default!;
        [Inject] BranchService BranchService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] AuthService AuthService { get; set; } = default!;
        [Inject] IJSRuntime JSRuntime { get; set; } = default!;

        [Parameter] public Guid? Id { get; set; }

        private bool isInitialized = false;
        private RichTextEditor? narrationEditor;
        private APAdjustmentViewModel Adjustment = new();

        // Dropdown data
        private List<BranchModel> Branches = new();
        private List<VendorViewModel> Vendors = new();
        private List<VendorBillViewModel> VendorBills = new();
        private List<APAdjustmentReasonViewModel> AllReasons = new();
        private List<APAdjustmentReasonViewModel> FilteredReasons = new();
        private List<AccountViewModel> OffsetAccounts = new();
        private List<TaxCodeDto> TaxCodes = new();

        // Validation errors
        private Dictionary<string, string> HeaderValidationErrors = new();

        private bool IsEdit => Id.HasValue;
        private bool IsReadOnly => Adjustment.AdjustmentStatus != APAdjustmentStatuses.Draft;
        private EditContext _editContext;
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
                    // Load bills for selected vendor
                    if (Adjustment.VendorId != Guid.Empty)
                    {
                        LoadBillsForVendor(Adjustment.VendorId);
                    }
                    // Filter reasons based on current type
                    FilterReasonsByType(Adjustment.AdjustmentType);
                }
                else
                {
                    ToastService.ShowError("AP Adjustment not found.", "Error");
                    Nav.NavigateTo("/ap-adjustments");
                    return;
                }
            }
            else
            {
                Adjustment = CreateNewAdjustment();
            }
            _editContext = new EditContext(Adjustment);
            isInitialized = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("feather.replace");
            }
        }

        private async Task LoadMasterData()
        {
            // Load branches from BranchService
            Branches = BranchService.GetAll()
                .Where(b => b.Status == "Active").ToList();

            // Load active vendors
            Vendors = VendorService.GetAll().Where(v => v.VendorStatus == VendorStatuses.Active).ToList();

            // Load all reason codes
            AllReasons = AdjustmentService.GetAllReasons().ToList();
            FilteredReasons = AllReasons.ToList();

            // Load offset accounts (expense accounts for AP) from COADataService
            var allAccounts = COADataService.GetAllAccounts();
            OffsetAccounts = allAccounts
                .Where(a => a.AccountNature == AccountNatures.Expense && a.IsPostable && a.IsActive)
                .ToList();

            // Load tax codes
            TaxCodes = new List<TaxCodeDto>
            {
                new TaxCodeDto { Id = Guid.Parse("00000000-0000-0000-0000-000000000060"), Name = "GST 5% (Input)" },
                new TaxCodeDto { Id = Guid.Parse("00000000-0000-0000-0000-000000000061"), Name = "GST 12% (Input)" },
                new TaxCodeDto { Id = Guid.Parse("00000000-0000-0000-0000-000000000062"), Name = "GST 18% (Input)" },
                new TaxCodeDto { Id = Guid.Parse("00000000-0000-0000-0000-000000000063"), Name = "GST 28% (Input)" },
                new TaxCodeDto { Id = Guid.Parse("00000000-0000-0000-0000-000000000064"), Name = "GST Exempt" },
                new TaxCodeDto { Id = Guid.Parse("00000000-0000-0000-0000-000000000065"), Name = "TDS 194C (1%)" },
                new TaxCodeDto { Id = Guid.Parse("00000000-0000-0000-0000-000000000066"), Name = "TDS 194C (2%)" }
            };

            await Task.CompletedTask;
        }

        private void LoadBillsForVendor(Guid vendorId)
        {
            // Get posted bills with outstanding balance for the selected vendor
            // SettlementStatus handles PartiallyPaid status, BillStatus only has Posted
            VendorBills = BillService.GetByVendorId(vendorId)
                .Where(b => b.BillStatus == VendorBillStatuses.Posted &&
                            b.AmountOutstanding > 0)
                .OrderByDescending(b => b.BillDate)
                .ToList();
        }

        private void FilterReasonsByType(string adjustmentType)
        {
            // Filter reasons that are applicable for the selected adjustment type
            FilteredReasons = AllReasons
                .Where(r => r.ApplicableTypes.Contains(adjustmentType) || !r.ApplicableTypes.Any())
                .ToList();
        }

        private APAdjustmentViewModel CreateNewAdjustment()
        {
            var adjustmentNumber = AdjustmentService.GenerateAdjustmentNumber(
                MasterDataIds.Companies.SofaCraft);

            var newAdjustment = new APAdjustmentViewModel
            {
                CompanyId = MasterDataIds.Companies.SofaCraft,
                CompanyName = "SofaCraft Furnishings Private Limited",
                BranchId = Guid.Empty,
                BranchName = null,
                AdjustmentNumber = adjustmentNumber,
                AdjustmentDate = DateTime.Today,
                AdjustmentStatus = APAdjustmentStatuses.Draft,
                AdjustmentType = string.Empty,
                AdjustmentDirection = string.Empty,
                AdjustmentScope = string.Empty,
                CurrencyId = MasterDataIds.Currencies.INR,
                CurrencyCode = "INR",
                CurrencyName = "Indian Rupee",
                CreatedBy = AuthService.CurrentUser?.UserName ?? "Current User",
                CreatedAt = DateTime.Now
            };

            // Filter reasons for default type (show all when no type selected)
            FilterReasonsByType(newAdjustment.AdjustmentType);

            return newAdjustment;
        }

        private Guid GetDefaultOffsetAccountId(string adjustmentType)
        {
            // Return the first available offset account, or empty if none loaded
            return OffsetAccounts.FirstOrDefault()?.Id ?? Guid.Empty;
        }

        #region Event Handlers

        private void OnVendorChanged()
        {
            if (Adjustment.VendorId != Guid.Empty)
            {
                var vendor = Vendors.FirstOrDefault(v => v.Id == Adjustment.VendorId);
                if (vendor != null)
                {
                    Adjustment.VendorCode = vendor.VendorCode;
                    Adjustment.VendorName = vendor.VendorName;
                    // Set currency from vendor
                    Adjustment.CurrencyId = vendor.DefaultCurrencyId ?? Guid.Parse("d1d1d1d1-d1d1-d1d1-d1d1-d1d1d1d1d101");
                    Adjustment.CurrencyCode = vendor.DefaultCurrencyCode ?? "INR";
                    Adjustment.CurrencyName = vendor.DefaultCurrencyName ?? "Indian Rupee";
                    // Set exchange rate
                    Adjustment.ExchangeRate = Adjustment.CurrencyCode == "INR" ? 1 : 0;
                    
                    // Load bills for this vendor
                    LoadBillsForVendor(Adjustment.VendorId);
                }
            }
            else
            {
                Adjustment.VendorCode = null;
                Adjustment.VendorName = null;
                VendorBills.Clear();
            }

            // Clear bill selection if vendor changes
            Adjustment.TargetVendorBillId = null;
            Adjustment.TargetVendorBillNumber = null;
            Adjustment.TargetBillOutstandingSnapshot = null;

            ClearHeaderValidationError("VendorId");
            StateHasChanged();
        }

        private void OnAdjustmentTypeChanged()
        {
            // Filter reasons based on selected type
            FilterReasonsByType(Adjustment.AdjustmentType);

            // Set default offset account
            var defaultAccountId = GetDefaultOffsetAccountId(Adjustment.AdjustmentType);
            Adjustment.AdjustmentGLAccountId = defaultAccountId;
            var account = OffsetAccounts.FirstOrDefault(a => a.Id == defaultAccountId);
            if (account != null)
            {
                Adjustment.AdjustmentGLAccountCode = account.AccountCode;
                Adjustment.AdjustmentGLAccountName = account.AccountName;
            }

            // Clear reason if not applicable
            if (FilteredReasons.All(r => r.Id != Adjustment.ReasonCodeId))
            {
                Adjustment.ReasonCodeId = Guid.Empty;
                Adjustment.ReasonCode = null;
                Adjustment.ReasonDescription = null;
            }

            ClearHeaderValidationError("AdjustmentType");
            StateHasChanged();
        }

        private void OnDirectionChanged()
        {
            ClearHeaderValidationError("AdjustmentDirection");
            StateHasChanged();
        }

        private void OnScopeChanged()
        {
            // Clear bill selection if scope is not BillLevel
            if (Adjustment.AdjustmentScope != APAdjustmentScopes.BillLevel)
            {
                Adjustment.TargetVendorBillId = null;
                Adjustment.TargetVendorBillNumber = null;
                Adjustment.TargetBillOutstandingSnapshot = null;
            }

            ClearHeaderValidationError("AdjustmentScope");
            StateHasChanged();
        }

        private void OnBillSelected()
        {
            if (Adjustment.TargetVendorBillId.HasValue)
            {
                var bill = VendorBills.FirstOrDefault(b => b.Id == Adjustment.TargetVendorBillId.Value);
                if (bill != null)
                {
                    Adjustment.TargetVendorBillNumber = bill.BillNumber;
                    Adjustment.TargetBillOutstandingSnapshot = bill.AmountOutstanding;
                }
            }
            else
            {
                Adjustment.TargetVendorBillNumber = null;
                Adjustment.TargetBillOutstandingSnapshot = null;
            }

            ClearHeaderValidationError("TargetVendorBillId");
            StateHasChanged();
        }

        private void OnReasonCodeChanged()
        {
            if (Adjustment.ReasonCodeId != Guid.Empty)
            {
                var reason = AllReasons.FirstOrDefault(r => r.Id == Adjustment.ReasonCodeId);
                if (reason != null)
                {
                    Adjustment.ReasonCode = reason.ReasonCode;
                    Adjustment.ReasonDescription = reason.ReasonDescription;
                    Adjustment.EvidenceRequired = reason.RequiresEvidence;

                    // Set default offset account from reason if available
                    if (reason.DefaultOffsetAccountId.HasValue)
                    {
                        Adjustment.AdjustmentGLAccountId = reason.DefaultOffsetAccountId.Value;
                        Adjustment.AdjustmentGLAccountCode = reason.DefaultOffsetAccountCode;
                        Adjustment.AdjustmentGLAccountName = reason.DefaultOffsetAccountName;
                    }
                }
            }
            else
            {
                Adjustment.ReasonCode = null;
                Adjustment.ReasonDescription = null;
                Adjustment.EvidenceRequired = false;
            }

            ClearHeaderValidationError("ReasonCodeId");
            StateHasChanged();
        }

        private void OnGLAccountChanged()
        {
            if (Adjustment.AdjustmentGLAccountId != Guid.Empty)
            {
                var account = OffsetAccounts.FirstOrDefault(a => a.Id == Adjustment.AdjustmentGLAccountId);
                if (account != null)
                {
                    Adjustment.AdjustmentGLAccountCode = account.AccountCode;
                    Adjustment.AdjustmentGLAccountName = account.AccountName;
                    // Derive account type (simplified for demo)
                    Adjustment.AdjustmentGLAccountType = APAdjustmentGLAccountTypes.Expense;
                }
            }
            else
            {
                Adjustment.AdjustmentGLAccountCode = null;
                Adjustment.AdjustmentGLAccountName = null;
                Adjustment.AdjustmentGLAccountType = null;
            }

            ClearHeaderValidationError("AdjustmentGLAccountId");
            StateHasChanged();
        }

        private void OnAmountChanged()
        {
            // Recalculate policy limit category
            Adjustment.PolicyLimitCategory = DeterminePolicyCategory(Adjustment.AdjustmentAmount);
            ClearHeaderValidationError("AdjustmentAmount");
            StateHasChanged();
        }

        private void OnBranchChanged()
        {
            if (Adjustment.BranchId != Guid.Empty)
            {
                var branch = Branches.FirstOrDefault(b => b.Id == Adjustment.BranchId);
                if (branch != null)
                {
                    Adjustment.BranchName = branch.BranchName;
                }
            }
            ClearHeaderValidationError("BranchId");
            StateHasChanged();
        }

        private string DeterminePolicyCategory(decimal amount)
        {
            // Simple policy category determination based on amount
            if (amount <= 100) return APPolicyLimitCategories.SmallWriteOff;
            if (amount <= 10000) return APPolicyLimitCategories.Medium;
            return APPolicyLimitCategories.HighRisk;
        }

        #endregion

        #region Validation

        private bool ValidateHeader()
        {
            HeaderValidationErrors.Clear();

            if (Adjustment.BranchId == Guid.Empty)
                HeaderValidationErrors["BranchId"] = "Branch is required";

            if (Adjustment.VendorId == null || Adjustment.VendorId == Guid.Empty)
                HeaderValidationErrors["VendorId"] = "Vendor is required";

            if (Adjustment.AdjustmentDate == default)
                HeaderValidationErrors["AdjustmentDate"] = "Adjustment Date is required";

            if (string.IsNullOrWhiteSpace(Adjustment.AdjustmentType))
                HeaderValidationErrors["AdjustmentType"] = "Adjustment Type is required";

            if (string.IsNullOrWhiteSpace(Adjustment.AdjustmentDirection))
                HeaderValidationErrors["AdjustmentDirection"] = "Direction is required";

            if (Adjustment.ReasonCodeId == null || Adjustment.ReasonCodeId == Guid.Empty)
                HeaderValidationErrors["ReasonCodeId"] = "Reason Code is required";

            if (string.IsNullOrWhiteSpace(Adjustment.AdjustmentScope))
                HeaderValidationErrors["AdjustmentScope"] = "Scope is required";

            if (Adjustment.AdjustmentScope == APAdjustmentScopes.BillLevel && !Adjustment.TargetVendorBillId.HasValue)
                HeaderValidationErrors["TargetVendorBillId"] = "Target Bill is required for Bill Level adjustments";

            if (Adjustment.AdjustmentAmount <= 0)
                HeaderValidationErrors["AdjustmentAmount"] = "Adjustment Amount must be greater than 0";

            if (Adjustment.AdjustmentGLAccountId == null || Adjustment.AdjustmentGLAccountId == Guid.Empty)
                HeaderValidationErrors["AdjustmentGLAccountId"] = "Adjustment Account is required";

            if (string.IsNullOrWhiteSpace(Adjustment.Narration))
                HeaderValidationErrors["Narration"] = "Narration is required for adjustments";

            return HeaderValidationErrors.Count == 0;
        }

        private bool HasHeaderValidationError(string field)
        {
            return HeaderValidationErrors.ContainsKey(field);
        }

        private string GetHeaderValidationError(string field)
        {
            return HeaderValidationErrors.TryGetValue(field, out var error) ? error : string.Empty;
        }

        private void ClearHeaderValidationError(string field)
        {
            HeaderValidationErrors.Remove(field);
        }

        private async Task ScrollToFirstValidationError()
        {
            await Task.Delay(100); // allow UI to render validation
            await JSRuntime.InvokeVoidAsync("scrollToFirstValidationError");
        }
        #endregion

        #region Save Operations



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



        private async Task HandleSubmit(EditContext editContext)
        {
            // Get narration from rich text editor
            if (narrationEditor != null)
                Adjustment.Narration = await narrationEditor.GetHtmlAsync();

            HeaderValidationErrors.Clear();

            // Run custom validation
            var isValid = ValidateHeader();

            if (!isValid)
            {
                StateHasChanged();
                await ScrollToFirstValidationError();
                return;
            }

            var result = IsEdit
                ? AdjustmentService.Update(Adjustment)
                : AdjustmentService.Add(Adjustment);

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                Nav.NavigateTo("/ap-adjustments");
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        private async Task SubmitAdjustment()
        {
            // First validate
            if (!ValidateHeader())
            {
                StateHasChanged();
                await ScrollToFirstValidationError();
                return;
            }

            // Check evidence requirement
            if (Adjustment.EvidenceRequired && Adjustment.AttachmentCount == 0)
            {
                ToastService.ShowWarning("Evidence/documentation is required before submitting. Please attach supporting documents.");
                return;
            }

            // Save first if new
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

            // Now submit
            var result = AdjustmentService.Submit(Adjustment.APAdjustmentId, AuthService.CurrentUser?.UserName ?? "Current User");
            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                Nav.NavigateTo("/ap-adjustments");
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

            if (!ValidateHeader())
            {
                StateHasChanged();
                await ScrollToFirstValidationError();
                return;
            }

            // Save changes first
            var saveResult = IsEdit ? AdjustmentService.Update(Adjustment) : AdjustmentService.Add(Adjustment);
            if (!saveResult.Success)
            {
                ToastService.ShowError(saveResult.Message);
                return;
            }

            // Now post
            var result = AdjustmentService.Post(
                Adjustment.APAdjustmentId,
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                AuthService.CurrentUser?.UserName ?? "Current User");

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                Nav.NavigateTo("/ap-adjustments");
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
            APAdjustmentStatuses.Draft => "bg-secondary",
            APAdjustmentStatuses.Submitted => "bg-info",
            APAdjustmentStatuses.Approved => "bg-primary",
            APAdjustmentStatuses.Posted => "bg-success",
            APAdjustmentStatuses.Cancelled => "bg-warning text-dark",
            APAdjustmentStatuses.Reversed => "bg-danger",
            APAdjustmentStatuses.Rejected => "bg-danger",
            _ => "bg-secondary"
        };

        private string GetTypeBadgeClass(string type) => type switch
        {
            APAdjustmentTypes.WriteOff => "bg-danger",
            APAdjustmentTypes.RoundOffCorrection => "bg-info",
            APAdjustmentTypes.DisputeSettlement => "bg-warning text-dark",
            APAdjustmentTypes.Reclassification => "bg-primary",
            APAdjustmentTypes.VendorBalanceTransfer => "bg-secondary",
            APAdjustmentTypes.FXDifference => "bg-dark",
            APAdjustmentTypes.Other => "bg-secondary",
            _ => "bg-secondary"
        };

        private string GetDirectionBadgeClass(string direction) => direction switch
        {
            APAdjustmentDirections.ReducePayable => "bg-success",
            APAdjustmentDirections.IncreasePayable => "bg-warning text-dark",
            _ => "bg-secondary"
        };

        private string GetPolicyBadgeClass(string category) => category switch
        {
            APPolicyLimitCategories.SmallWriteOff => "bg-success",
            APPolicyLimitCategories.Medium => "bg-warning text-dark",
            APPolicyLimitCategories.HighRisk => "bg-danger",
            _ => "bg-secondary"
        };

        private string GetScopeBadgeClass(string scope) => scope switch
        {
            APAdjustmentScopes.VendorLevel => "bg-primary",
            APAdjustmentScopes.BillLevel => "bg-info",
            APAdjustmentScopes.AdvanceLevel => "bg-warning text-dark",
            _ => "bg-secondary"
        };

        private bool HasValidationError(string fieldName)
        {
            return HeaderValidationErrors.ContainsKey(fieldName);
        }

        private string GetValidationError(string fieldName)
        {
            return HeaderValidationErrors.TryGetValue(fieldName, out var error) ? error : string.Empty;
        }

        #endregion

        #region Local DTOs

        public class TaxCodeDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        #endregion
    }
}
