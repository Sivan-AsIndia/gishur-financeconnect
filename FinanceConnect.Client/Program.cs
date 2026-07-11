using Blazored.LocalStorage;
using FinanceConnect.Client;
using FinanceConnect.Client.Data;
using FinanceConnect.Client.Pages.Error;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient to point to the API (from wwwroot/appsettings.json)
var baseAddress = builder.Configuration.GetSection("ApiSettings")["BaseAddress"] 
    ?? builder.HostEnvironment.BaseAddress;
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri(baseAddress) 
});

// Add Blazored LocalStorage
builder.Services.AddBlazoredLocalStorage();

// Register services
builder.Services.AddSingleton<ProductService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddSingleton<BranchService>();
builder.Services.AddSingleton<ToastService>();
builder.Services.AddSingleton<MasterDataService>();
builder.Services.AddSingleton<FiscalYearService>();
builder.Services.AddSingleton<AccountingPeriodService>();
//builder.Services.AddSingleton<AccountsDataService>();
builder.Services.AddSingleton<AccountStore>();
builder.Services.AddSingleton<JournalService>();
builder.Services.AddSingleton<JournalEntryService>();
builder.Services.AddSingleton<JournalLineService>();
builder.Services.AddSingleton<FinanceDataService>();
builder.Services.AddSingleton<COADataService>();
builder.Services.AddSingleton<DocumentNumberSeriesService>(); 
builder.Services.AddSingleton<DocumentSequenceService>();
builder.Services.AddSingleton<PostingProfileService>();
builder.Services.AddSingleton<CashAccountService>();
builder.Services.AddSingleton<BankAccountService>();
builder.Services.AddSingleton<PostingRuleService>();
builder.Services.AddSingleton<TransactionStatusService>();
builder.Services.AddSingleton<FinancialTransactionService>();
builder.Services.AddSingleton<TransactionTypeService>();
builder.Services.AddSingleton<TransactionLineService>();
builder.Services.AddScoped<BankTransactionService>();
builder.Services.AddScoped<BranchServiceData>(); 
//builder.Services.AddScoped<CompanyServiceData>(); 
builder.Services.AddScoped<CashTransferService>();
builder.Services.AddScoped<BankStatementService>();
builder.Services.AddScoped<FundTransferService>();
builder.Services.AddScoped<BankReconciliationService>();
builder.Services.AddScoped<ChequeService>();
builder.Services.AddSingleton<PostingProfileSeed>();

// Common Services
builder.Services.AddSingleton<CurrencyService>();

// AR Services
builder.Services.AddSingleton<CustomerService>();
builder.Services.AddSingleton<CustomerAccountService>();
builder.Services.AddSingleton<CustomerInvoiceService>();
builder.Services.AddSingleton<CustomerCreditNoteService>();
builder.Services.AddSingleton<CustomerDebitNoteService>();
builder.Services.AddSingleton<CustomerPaymentService>();
builder.Services.AddScoped<ARAdjustmentService>();
builder.Services.AddSingleton<CustomerAgingService>();

// AP Services
builder.Services.AddSingleton<VendorService>();
builder.Services.AddSingleton<VendorAccountService>();
builder.Services.AddSingleton<VendorBillService>();
builder.Services.AddSingleton<VendorPaymentService>();
builder.Services.AddSingleton<VendorCreditNoteService>();
builder.Services.AddSingleton<VendorDebitNoteService>();
builder.Services.AddScoped<APAdjustmentService>();
builder.Services.AddSingleton<VendorAgingService>();
builder.Services.AddSingleton<ThemeService>();
builder.Services.AddScoped<FileManagerService>();

builder.Services.AddSingleton<DashboardService>();
builder.Services.AddSingleton<SettingsService>();

// Employee Management
builder.Services.AddSingleton<EmployeeService>();
builder.Services.AddScoped<TaskService>();
builder.Services.AddSingleton<TaskStatusService>();

// Sales Management
builder.Services.AddScoped<QuotationService>();
builder.Services.AddSingleton<ItemService>();
builder.Services.AddSingleton<DeliveryChallanService>();

// Fixed Assets
builder.Services.AddSingleton<DepreciationRunService>();
builder.Services.AddSingleton<AssetDisposalService>();
builder.Services.AddSingleton<DepreciationRunLineService>();
builder.Services.AddSingleton<RoleChangingService>();
builder.Services.AddSingleton<AssetCategoryService>();
builder.Services.AddSingleton<FixedAssetService>();
builder.Services.AddSingleton<AssetTransferService>();
builder.Services.AddSingleton<AssetAcquisitionService>();
builder.Services.AddSingleton<DepreciationMethodService>();
builder.Services.AddSingleton<AssetDepreciationScheduleService>();
builder.Services.AddSingleton<AssetRevaluationImpairmentService>();

// Tax Services
builder.Services.AddSingleton<TaxCodeService>();
builder.Services.AddSingleton<TaxRateVersionService>();
builder.Services.AddSingleton<TaxCategoryMappingService>();
builder.Services.AddSingleton<TaxCategoryMappingSeedData>();
builder.Services.AddSingleton<TDSConfigService>();
builder.Services.AddSingleton<TDSConfigSeedData>();
builder.Services.AddSingleton<TCSConfigService>();
builder.Services.AddSingleton<TCSConfigSeedData>();
builder.Services.AddSingleton<TaxTransactionService>();
builder.Services.AddSingleton<TDSDeductionEntryService>();
builder.Services.AddSingleton<TDSDeductionEntrySeedData>();
builder.Services.AddSingleton<TaxSettlementService>();
builder.Services.AddSingleton<GSTReturnRunService>();
builder.Services.AddSingleton<TaxAuditTrailService>();

//Budgeting & Cost Control
builder.Services.AddSingleton<CostCenterService>();
builder.Services.AddSingleton<CostAllocationService>();
builder.Services.AddSingleton<VarianceAnalysisService>();
builder.Services.AddSingleton<ForecastService>();
builder.Services.AddSingleton<FinancialPlanService>();
builder.Services.AddSingleton<BudgetService>();
builder.Services.AddSingleton<BudgetPeriodService>();
builder.Services.AddSingleton<BudgetLineService>();

// ── Revenue & Expense ──────────────────────────────────────────────────────
builder.Services.AddSingleton<RevenueService>();
builder.Services.AddSingleton<RevenueRecognitionService>();
builder.Services.AddSingleton<ExpenseCategoryService>();
builder.Services.AddSingleton<ExpenseService>();
builder.Services.AddSingleton<ExpenseClaimService>();
builder.Services.AddSingleton<AccrualService>();
builder.Services.AddSingleton<PrepaymentService>();
builder.Services.AddSingleton<DeferredRevenueService>();


builder.Services.AddScoped(sp => {
    var client = new HttpClient();
    client.Timeout = TimeSpan.FromSeconds(30);
    return new LiveExchangeRateService(client);
});

await builder.Build().RunAsync();
