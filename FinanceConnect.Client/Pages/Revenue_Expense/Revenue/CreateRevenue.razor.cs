using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Linq.Expressions;
using static FinanceConnect.Client.ViewModels.RevenueViewModel;

namespace FinanceConnect.Client.Pages.Revenue_Expense.Revenue
{
    public partial class CreateRevenue
    {
        [Inject] RevenueService    RevenueService    { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] BranchService     BranchService     { get; set; } = default!;
        [Inject] CustomerService   CustomerService   { get; set; } = default!;
        [Inject] NavigationManager Nav               { get; set; } = default!;
        [Inject] IJSRuntime        JS                { get; set; } = default!;
        [Inject] ToastService      ToastService      { get; set; } = default!;

        [Parameter] public Guid? Id { get; set; }

        // ── Model & edit context ───────────────────────────────────────────────
        private ViewModels.RevenueViewModel.Revenue model = new();
        private EditContext _editContext = default!;
        private bool IsEdit => Id.HasValue;

        private string PageTitle    => IsEdit ? "Edit Revenue" : "Create Revenue";
        private string PageSubTitle => IsEdit
            ? "Update the operational revenue event record"
            : "Record a new income-generating business event";

        // ── Accordion state ────────────────────────────────────────────────────
        private bool ShowIdentity      = true;
        private bool ShowCustomer      = false;
        private bool ShowClassification = false;
        private bool ShowAmounts       = false;
        private bool ShowRecognition   = false;
        private bool ShowBilling       = false;
        private bool ShowDimensions    = false;
        private bool ShowWorkflow      = false;
        private bool ShowNotes         = false;

        // ── Touch tracking ────────────────────────────────────────────────────
        private bool IdentityTouched       = false;
        private bool CustomerTouched       = false;
        private bool ClassificationTouched = false;
        private bool AmountsTouched        = false;
        private bool RecognitionTouched    = false;
        private bool BillingTouched        = false;
        private bool DimensionsTouched     = false;
        private bool WorkflowTouched       = false;
        private bool NotesTouched          = false;

        private void TouchIdentity()       => IdentityTouched = true;
        private void TouchCustomer()       => CustomerTouched = true;
        private void TouchClassification() => ClassificationTouched = true;
        private void TouchAmounts()        => AmountsTouched = true;
        private void TouchRecognition()    => RecognitionTouched = true;
        private void TouchBilling()        => BillingTouched = true;
        private void TouchDimensions()     => DimensionsTouched = true;
        private void TouchWorkflow()       => WorkflowTouched = true;
        private void TouchNotes()          => NotesTouched = true;

        // ── Confirm modal ──────────────────────────────────────────────────────
        private string  ConfirmTitle   = "";
        private string  ConfirmMessage = "";
        private string  ConfirmType    = "warning";
        private Func<Task>? ConfirmAction;

        // ── Lookup data ────────────────────────────────────────────────────────
        private List<CompanyModel>  Companies    = new();
        private List<CustomerLookup> Customers   = new();
        private List<CurrencyLookup> Currencies  = new();
        private List<ExchangeRateLookup> ExchangeRates = new();
        private List<BranchLookup>   Branches    = new();
        private List<GLAccountLookup> GLAccounts = new();

        private List<string> RevenueCategoryOptions = new()
        {
            "SubscriptionIncome",
            "ConsultingRevenue",
            "ServiceIncome",
            "ProjectIncome",
            "LicenseRevenue",
            "SalesRevenue",
            "ManagementFeeIncome",
            "RoyaltyIncome",
            "OtherOperationalIncome"
        };

        // ── Lifecycle ──────────────────────────────────────────────────────────
        protected override void OnInitialized()
        {
            LoadLookups();

            if (IsEdit)
            {
                var existing = RevenueService.GetById(Id!.Value);
                if (existing is not null)
                    model = CloneModel(existing);
            }
            else
            {
                model = new ViewModels.RevenueViewModel.Revenue
                {
                    Status                    = RevenueStatus.Draft,
                    RevenueSourceDocType        = RevenueSourceDocType.ManualRevenueEvent,
                    RevenueType               = RevenueType.OneTime,
                    RevenueNature             = RevenueNature.EarnedImmediately,
                    RecognitionMethod         = RecognitionMethod.Immediate,
                    RecognitionStatus         = RecognitionStatus.NotStarted,
                    BillingStatus             = BillingStatus.NotBilled,
                    CollectionStatus          = CollectionStatus.NotCollected,
                    IsRecognitionRequired     = true,
                    IsDeferredRevenueRequired = false,
                    IsAdvanceReceipt          = false,
                    BusinessEventDate         = DateTime.Today,
                    PreparedOn                = DateTime.Today,
                    CurrencyId                = Data.MasterDataIds.Currencies.INR,
                    TenantId                  = Data.MasterDataIds.Tenants.Default,
                    CompanyId                 = Data.MasterDataIds.Companies.SofaCraft
                };

                // Auto-generate a code
                model.RevenueCode = RevenueService.GenerateCode(model.CompanyId);
            }

            _editContext = new EditContext(model);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("initFeatherIcons");
        }

        private void LoadLookups()
        {
            Companies = MasterDataService.GetAllCompanies();

            Customers = CustomerService.GetAll()
                .Select(c => new CustomerLookup
                {
                    Id           = c.Id,
                    CustomerCode = c.CustomerCode,
                    CustomerName = c.CustomerName
                }).ToList();

            Currencies = MasterDataService.GetAllCurrencies()
                .Select(c => new CurrencyLookup
                {
                    Id           = c.Id,
                    CurrencyCode = c.CurrencyCode,
                    CurrencyName = c.CurrencyName
                }).ToList();

            ExchangeRates = MasterDataService.GetAllExchangeRates()
                .Select(er => new ExchangeRateLookup
                {
                    Id           = er.Id,
                    DisplayLabel = $"{er.BaseCurrencyCode} → {er.QuoteCurrencyCode} ({er.Rate:N4})"
                }).ToList();

            Branches = BranchService.GetAll()
                .Select(b => new BranchLookup
                {
                    Id         = b.Id,
                    BranchCode = b.BranchCode,
                    BranchName = b.BranchName
                }).ToList();

            GLAccounts = new List<GLAccountLookup>
            {
                new() { Id = Data.MasterDataIds.Accounts.SalesRevenue,   AccountCode = "4001", AccountName = "Sales Revenue – Sofas" },
                new() { Id = Data.MasterDataIds.Accounts.ServiceRevenue,  AccountCode = "4002", AccountName = "Service Revenue" },
            };
        }

        // ── Accordion ─────────────────────────────────────────────────────────

        // Maps section name → card element id
        private static string GetCardId(string section) => section switch
        {
            "identity"       => "rv-card-identity",
            "customer"       => "rv-card-customer",
            "classification" => "rv-card-classification",
            "amounts"        => "rv-card-amounts",
            "recognition"    => "rv-card-recognition",
            "billing"        => "rv-card-billing",
            "dimensions"     => "rv-card-dimensions",
            "workflow"       => "rv-card-workflow",
            "notes"          => "rv-card-notes",
            _                => ""
        };

        private async Task ToggleAccordion(string section)
        {
            switch (section)
            {
                case "identity":       ShowIdentity       = !ShowIdentity;       break;
                case "customer":       ShowCustomer       = !ShowCustomer;       break;
                case "classification": ShowClassification = !ShowClassification; break;
                case "amounts":        ShowAmounts        = !ShowAmounts;        break;
                case "recognition":    ShowRecognition    = !ShowRecognition;    break;
                case "billing":        ShowBilling        = !ShowBilling;        break;
                case "dimensions":     ShowDimensions     = !ShowDimensions;     break;
                case "workflow":       ShowWorkflow       = !ShowWorkflow;       break;
                case "notes":          ShowNotes          = !ShowNotes;          break;
            }

            // Always ensure the clicked section is open, then scroll to its card
            OpenSection(section);
            await ScrollToCard(section);
        }

        private void OpenSection(string section)
        {
            switch (section)
            {
                case "identity":       ShowIdentity       = true; break;
                case "customer":       ShowCustomer       = true; break;
                case "classification": ShowClassification = true; break;
                case "amounts":        ShowAmounts        = true; break;
                case "recognition":    ShowRecognition    = true; break;
                case "billing":        ShowBilling        = true; break;
                case "dimensions":     ShowDimensions     = true; break;
                case "workflow":       ShowWorkflow       = true; break;
                case "notes":          ShowNotes          = true; break;
            }
        }

        private async Task ScrollToCard(string section)
        {
            var id = GetCardId(section);
            if (!string.IsNullOrEmpty(id))
            {
                await Task.Yield(); // Allow Blazor to re-render and open the card first
                await JS.InvokeVoidAsync("revenueScrollToId", id);
            }
        }

        // ── Input helpers ─────────────────────────────────────────────────────
        private string RevenueCodeInput
        {
            get => model.RevenueCode;
            set => model.RevenueCode = value?.Trim().ToUpperInvariant() ?? "";
        }

        private void OnCustomerChanged()
        {
            var cust = Customers.FirstOrDefault(c => c.Id == model.CustomerId);
            if (cust is not null)
            {
                model.CustomerCodeSnapshot = cust.CustomerCode;
                model.CustomerNameSnapshot = cust.CustomerName;
            }
            else
            {
                model.CustomerCodeSnapshot = null;
                model.CustomerNameSnapshot = null;
            }
        }

        // ── Validation ────────────────────────────────────────────────────────
        private string GetValidationClass(Expression<Func<object>> field)
        {
            if (_editContext is null) return string.Empty;
            var fi = FieldIdentifier.Create(field);
            return _editContext.GetValidationMessages(fi).Any()
                ? "is-invalid"
                : _editContext.IsModified(fi) ? "is-valid" : string.Empty;
        }

        // ── Submit ────────────────────────────────────────────────────────────
        private async Task HandleSubmit()
        {
            if (_editContext.Validate())
            {
                await ValidateBusinessRules();
            }
            else
            {
                // Open and scroll to the first section that has errors
                string errorSection = "";
                if (HasIdentityErrors())         errorSection = "identity";
                else if (HasCustomerErrors())    errorSection = "customer";
                else if (HasAmountErrors())      errorSection = "amounts";
                else if (HasRecognitionErrors()) errorSection = "recognition";
                else if (HasBillingErrors())     errorSection = "billing";
                else if (HasWorkflowErrors())    errorSection = "workflow";

                if (!string.IsNullOrEmpty(errorSection))
                {
                    OpenSection(errorSection);
                    await InvokeAsync(StateHasChanged);
                    await Task.Delay(150); // Wait for section to expand
                    await ScrollToCard(errorSection);
                    await Task.Delay(400); // Wait for scroll to complete
                }

                // Then scroll to the specific invalid field within the opened section
                await JS.InvokeVoidAsync("scrollToFirstValidationError");
                await InvokeAsync(StateHasChanged);
            }
        }

        private bool HasIdentityErrors()   =>
            string.IsNullOrWhiteSpace(model.RevenueCode) || string.IsNullOrWhiteSpace(model.RevenueName);

        private bool HasCustomerErrors()   => model.CustomerId == Guid.Empty;
        private bool HasAmountErrors()     => model.GrossRevenueAmount < 0 || model.CurrencyId == Guid.Empty;
        private bool HasWorkflowErrors()   => string.IsNullOrWhiteSpace(model.PreparedByUserId);

        private bool HasRecognitionErrors() =>
            (model.RecognitionMethod == RecognitionMethod.Scheduled ||
             model.RecognitionMethod == RecognitionMethod.DeferredThenRelease) &&
            !model.RecognitionStartDate.HasValue;

        private bool HasBillingErrors() =>
            model.Status == RevenueStatus.Cancelled &&
            string.IsNullOrWhiteSpace(model.CancellationReason);

        private async Task ValidateBusinessRules()
        {
            // Duplicate code check
            var isDuplicate = RevenueService.GetAll()
                .Any(r => r.CompanyId == model.CompanyId &&
                          string.Equals(r.RevenueCode, model.RevenueCode, StringComparison.OrdinalIgnoreCase) &&
                          r.RevenueId != model.RevenueId);

            if (isDuplicate)
            {
                await OpenConfirmModal(
                    "Duplicate Revenue Code",
                    $"Revenue Code '{model.RevenueCode}' already exists for this company.",
                    null!, "danger");
                return;
            }

            // Cancellation reason required
            if (model.Status == RevenueStatus.Cancelled && string.IsNullOrWhiteSpace(model.CancellationReason))
            {
                ToastService.ShowError("Cancellation reason is required when status is Cancelled.");
                OpenSection("workflow");
                return;
            }

            // Recognition dates
            if (model.RecognitionStartDate.HasValue && model.RecognitionEndDate.HasValue &&
                model.RecognitionEndDate < model.RecognitionStartDate)
            {
                ToastService.ShowError("Recognition End Date cannot be earlier than Recognition Start Date.");
                OpenSection("recognition");
                return;
            }

            // Scheduled method requires start date
            if ((model.RecognitionMethod == RecognitionMethod.Scheduled ||
                 model.RecognitionMethod == RecognitionMethod.DeferredThenRelease) &&
                !model.RecognitionStartDate.HasValue)
            {
                ToastService.ShowError("Recognition Start Date is required for Scheduled / Deferred recognition method.");
                OpenSection("recognition");
                return;
            }

            // Gross amount must be >= 0
            if (model.GrossRevenueAmount < 0)
            {
                ToastService.ShowError("Gross Revenue Amount must be zero or greater in a normal create flow.");
                OpenSection("amounts");
                return;
            }

            // Recognised amount cannot exceed gross
            if (model.RecognizedAmountToDate > model.GrossRevenueAmount)
            {
                ToastService.ShowError("Recognised Amount cannot exceed the Gross Revenue Amount.");
                OpenSection("amounts");
                return;
            }

            await ContinueSave();
        }

        private async Task ContinueSave()
        {
            try
            {
                if (IsEdit)
                {
                    RevenueService.Update(model);
                    ToastService.ShowSuccess($"Revenue '{model.RevenueCode}' updated successfully.");
                }
                else
                {
                    RevenueService.Add(model);
                    ToastService.ShowSuccess($"Revenue '{model.RevenueCode}' created successfully.");
                }

                Nav.NavigateTo("/revenues");
            }
            catch (Exception ex)
            {
                ToastService.ShowError(ex.Message);
            }
        }

        // ── Confirm modal helpers ─────────────────────────────────────────────
        private async Task OpenConfirmModal(string title, string message, Func<Task> action, string type = "warning")
        {
            ConfirmTitle   = title;
            ConfirmMessage = message;
            ConfirmType    = type;
            ConfirmAction  = action;
            await JS.InvokeVoidAsync("bootstrapModal.show", "confirm-modal");
        }

        private async Task ConfirmYes()
        {
            var action = ConfirmAction;
            ConfirmAction = null;
            await JS.InvokeVoidAsync("bootstrapModal.hide", "confirm-modal");
            await Task.Delay(150);
            if (action is not null) await action();
        }

        // ── Navigation ────────────────────────────────────────────────────────
        private void BackToList() => Nav.NavigateTo("/revenues");

        // ── Clone helper ──────────────────────────────────────────────────────
        private static ViewModels.RevenueViewModel.Revenue CloneModel(ViewModels.RevenueViewModel.Revenue src)
        {
            return new ViewModels.RevenueViewModel.Revenue
            {
                RevenueId                    = src.RevenueId,
                TenantId                     = src.TenantId,
                CompanyId                    = src.CompanyId,
                RevenueCode                  = src.RevenueCode,
                RevenueName                  = src.RevenueName,
                Description                  = src.Description,
                Status                       = src.Status,
                CustomerId                   = src.CustomerId,
                CustomerCodeSnapshot         = src.CustomerCodeSnapshot,
                CustomerNameSnapshot         = src.CustomerNameSnapshot,
                RevenueSourceDocType           = src.RevenueSourceDocType,
                SourceDocumentNumber         = src.SourceDocumentNumber,
                MilestoneReference           = src.MilestoneReference,
                RevenueType                  = src.RevenueType,
                RevenueCategoryCode          = src.RevenueCategoryCode,
                GLAccountId                  = src.GLAccountId,
                GLAccountName                = src.GLAccountName,
                RevenueNature                = src.RevenueNature,
                BusinessEventDate            = src.BusinessEventDate,
                OperationalPeriodFrom        = src.OperationalPeriodFrom,
                OperationalPeriodTo          = src.OperationalPeriodTo,
                GrossRevenueAmount           = src.GrossRevenueAmount,
                TaxExclusiveRevenueAmount    = src.TaxExclusiveRevenueAmount,
                RecognizedAmountToDate       = src.RecognizedAmountToDate,
                DeferredAmountToDate         = src.DeferredAmountToDate,
                AdjustmentAmount             = src.AdjustmentAmount,
                CurrencyId                   = src.CurrencyId,
                ExchangeRateId               = src.ExchangeRateId,
                RecognitionMethod            = src.RecognitionMethod,
                RecognitionStartDate         = src.RecognitionStartDate,
                RecognitionEndDate           = src.RecognitionEndDate,
                RecognitionFrequency         = src.RecognitionFrequency,
                RecognitionStatus            = src.RecognitionStatus,
                IsRecognitionRequired        = src.IsRecognitionRequired,
                IsDeferredRevenueRequired    = src.IsDeferredRevenueRequired,
                RevenueRecognitionTemplateCode = src.RevenueRecognitionTemplateCode,
                BillingStatus                = src.BillingStatus,
                CollectionStatus             = src.CollectionStatus,
                InvoiceNumberSnapshot        = src.InvoiceNumberSnapshot,
                BillingDate                  = src.BillingDate,
                CollectionReferenceText      = src.CollectionReferenceText,
                IsAdvanceReceipt             = src.IsAdvanceReceipt,
                BranchId                     = src.BranchId,
                BranchName                   = src.BranchName,
                DepartmentId                 = src.DepartmentId,
                CostCenterId                 = src.CostCenterId,
                BusinessUnitCode             = src.BusinessUnitCode,
                PreparedByUserId             = src.PreparedByUserId,
                ReviewedByUserId             = src.ReviewedByUserId,
                ApprovedByUserId             = src.ApprovedByUserId,
                PreparedOn                   = src.PreparedOn,
                CancellationReason           = src.CancellationReason,
                IsLocked                     = src.IsLocked,
                RevenueAssumptionText        = src.RevenueAssumptionText,
                Notes                        = src.Notes,
                CreatedAt                    = src.CreatedAt,
                UpdatedAt                    = src.UpdatedAt
            };
        }
    }

    // ── Minimal lookup DTOs ────────────────────────────────────────────────────

    public class CustomerLookup
    {
        public Guid   Id           { get; set; }
        public string CustomerCode { get; set; } = "";
        public string CustomerName { get; set; } = "";
    }

    public class CurrencyLookup
    {
        public Guid   Id           { get; set; }
        public string CurrencyCode { get; set; } = "";
        public string CurrencyName { get; set; } = "";
    }

    public class ExchangeRateLookup
    {
        public Guid   Id           { get; set; }
        public string DisplayLabel { get; set; } = "";
    }

    public class BranchLookup
    {
        public Guid   Id         { get; set; }
        public string BranchCode { get; set; } = "";
        public string BranchName { get; set; } = "";
    }

    public class GLAccountLookup
    {
        public Guid   Id          { get; set; }
        public string AccountCode { get; set; } = "";
        public string AccountName { get; set; } = "";
    }
}
