using FinanceConnect.Client.Services;
using FinanceConnect.Client.Shared;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Master.ExchangeRate
{
    public partial class AddExchangeRate
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] LiveExchangeRateService LiveRateService { get; set; } = default!;

        [Parameter] public Guid? Id { get; set; }

        private EditContext _editContext = default!;
        RichTextEditor? _notesEditor;
        private bool isInitialized = false;

        // Touched state for accordion sections
        bool IdentityTouched = false;
        bool AddressTouched = false;
        bool ContactTouched = false;
        bool FinanceTouched = false;

        // Accordion visibility state
        bool ShowIdentity = true;
        bool ShowRateValue = false;
        bool ShowSource = false;
        bool ShowStatus = false;

        void TouchIdentity() => IdentityTouched = true;
        void TouchAddress() => AddressTouched = true;
        void TouchContact() => ContactTouched = true;
        void TouchFinance() => FinanceTouched = true;

        void OnRateInput(ChangeEventArgs e)
        {
            AddressTouched = true;
            RateValidationError = null; // Clear error on input
        }

        void OnRateChanged()
        {
            AddressTouched = true;
            RateValidationError = null;
        }

        // OnChanged handlers for dropdowns
        void OnBaseCurrencyChanged()
        {
            IdentityTouched = true;
            BaseCurrencyValidationError = null;
            showCurrencyPairError = false;
        }

        void OnQuoteCurrencyChanged()
        {
            IdentityTouched = true;
            QuoteCurrencyValidationError = null;
            showCurrencyPairError = false;
        }

        void OnSourceTypeChanged()
        {
            ContactTouched = true;
            SourceTypeValidationError = null;
        }

        void OnStatusChanged()
        {
            FinanceTouched = true;
            StatusValidationError = null;
        }

        private ExchangeRateModel ExchangeRate = CreateNewExchangeRate();
        private List<CurrencyModel> Currencies = new();
        private List<CompanyModel> Companies = new();

        // For company selection (since CompanyId is nullable Guid)
        private string SelectedCompanyId = "";

        // Validation error messages for dropdowns
        private string? BaseCurrencyValidationError = null;
        private string? QuoteCurrencyValidationError = null;
        private string? RateTypeValidationError = null;
        private string? SourceTypeValidationError = null;
        private string? StatusValidationError = null;
        private string? RateValidationError = null;

        // Validation flags
        private bool showCurrencyPairError = false;

        // Live rate state
        private bool isFetchingLiveRate = false;
        private LiveRateResult? liveRateResult;

        private bool IsEdit => Id.HasValue;
        private bool CanEdit => !IsEdit || ExchangeRate.Status != "Posted";
        private string PageTitle => IsEdit ? "Edit Exchange Rate" : "Add Exchange Rate";
        private string PageSubTitle => IsEdit ? "Update exchange rate details" : "Create new exchange rate for currency pair";

        // Property wrappers for currency dropdowns
        private string SelectedBaseCurrencyId
        {
            get => ExchangeRate.BaseCurrencyId == Guid.Empty ? "" : ExchangeRate.BaseCurrencyId.ToString();
            set
            {
                ExchangeRate.BaseCurrencyId = string.IsNullOrEmpty(value) ? Guid.Empty : Guid.Parse(value);
                BaseCurrencyValidationError = null; // Clear error on change
                showCurrencyPairError = false;
                liveRateResult = null;
            }
        }

        private string SelectedQuoteCurrencyId
        {
            get => ExchangeRate.QuoteCurrencyId == Guid.Empty ? "" : ExchangeRate.QuoteCurrencyId.ToString();
            set
            {
                ExchangeRate.QuoteCurrencyId = string.IsNullOrEmpty(value) ? Guid.Empty : Guid.Parse(value);
                QuoteCurrencyValidationError = null; // Clear error on change
                showCurrencyPairError = false;
                liveRateResult = null;
            }
        }

        private string SelectedSourceType
        {
            get => ExchangeRate.SourceType ?? "";
            set
            {
                ExchangeRate.SourceType = string.IsNullOrEmpty(value) ? null : value;
                SourceTypeValidationError = null; // Clear error on change
            }
        }

        private string SelectedExchangeRateStatus
        {
            get => ExchangeRate.Status ?? "";
            set
            {
                ExchangeRate.Status = string.IsNullOrEmpty(value) ? null : value;
                StatusValidationError = null; // Clear error on change
            }
        }

        private string SelectedRateTypeValue
        {
            get => ExchangeRate.RateType ?? "";
            set
            {
                ExchangeRate.RateType = string.IsNullOrEmpty(value) ? null : value;
                RateTypeValidationError = null;
            }
        }

        void OnRateTypeChanged()
        {
            IdentityTouched = true;
            RateTypeValidationError = null;
        }

        private static ExchangeRateModel CreateNewExchangeRate()
        {
            return new ExchangeRateModel
            {
                RateDate = DateTime.Today,
                RateType = null,
                SourceType = null,
                Status = null,
                Rate = 0,
                VersionNo = 1
            };
        }

        protected override async Task OnInitializedAsync()
        {
            // Load reference data
            Currencies = MasterDataService.GetAllCurrencies().Where(c => !c.IsDeleted).ToList();
            Companies = MasterDataService.GetAllCompanies().Where(c => !c.IsDeleted).ToList();

            if (IsEdit)
            {
                var existing = MasterDataService.GetExchangeRateById(Id!.Value);
                if (existing != null)
                {
                    ExchangeRate = new ExchangeRateModel
                    {
                        Id = existing.Id,
                        TenantId = existing.TenantId,
                        BaseCurrencyId = existing.BaseCurrencyId,
                        BaseCurrencyCode = existing.BaseCurrencyCode,
                        BaseCurrencyName = existing.BaseCurrencyName,
                        QuoteCurrencyId = existing.QuoteCurrencyId,
                        QuoteCurrencyCode = existing.QuoteCurrencyCode,
                        QuoteCurrencyName = existing.QuoteCurrencyName,
                        CompanyId = existing.CompanyId,
                        CompanyCode = existing.CompanyCode,
                        CompanyName = existing.CompanyName,
                        RateDate = existing.RateDate,
                        RateType = existing.RateType,
                        Rate = existing.Rate,
                        IsTriangulated = existing.IsTriangulated,
                        TriangulationCurrencyId = existing.TriangulationCurrencyId,
                        TriangulationCurrencyCode = existing.TriangulationCurrencyCode,
                        SourceType = existing.SourceType,
                        SourceName = existing.SourceName,
                        EvidenceFileId = existing.EvidenceFileId,
                        EvidenceFileName = existing.EvidenceFileName,
                        Notes = existing.Notes,
                        Status = existing.Status,
                        VersionNo = existing.VersionNo,
                        CreatedAt = existing.CreatedAt,
                        CreatedBy = existing.CreatedBy,
                        UpdatedAt = existing.UpdatedAt,
                        UpdatedBy = existing.UpdatedBy
                    };

                    SelectedCompanyId = existing.CompanyId?.ToString() ?? "";
                }
                else
                {
                    ToastService.ShowError("Exchange rate not found", "Error");
                    Nav.NavigateTo("/exchange-rates");
                    return;
                }
            }

            _editContext = new EditContext(ExchangeRate);
            isInitialized = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
        }

        void ToggleAccordion(string section)
        {
            switch (section)
            {
                case "rateIdentity":
                    ShowIdentity = !ShowIdentity;
                    break;
                case "rateValue":
                    ShowRateValue = !ShowRateValue;
                    break;
                case "rateSource":
                    ShowSource = !ShowSource;
                    break;
                case "statusGovernance":
                    ShowStatus = !ShowStatus;
                    break;
            }
        }

        void OpenAccordion(string section)
        {
            switch (section)
            {
                case "rateIdentity":
                    ShowIdentity = true;
                    break;
                case "rateValue":
                    ShowRateValue = true;
                    break;
                case "rateSource":
                    ShowSource = true;
                    break;
                case "statusGovernance":
                    ShowStatus = true;
                    break;
            }
        }

        bool HasIdentityErrors()
        {
            return ExchangeRate.BaseCurrencyId == Guid.Empty
                || ExchangeRate.QuoteCurrencyId == Guid.Empty
                || ExchangeRate.BaseCurrencyId == ExchangeRate.QuoteCurrencyId
                || string.IsNullOrWhiteSpace(ExchangeRate.RateType);
        }

        bool HasRateValueErrors()
        {
            return ExchangeRate.Rate <= 0;
        }

        bool HasSourceErrors()
        {
            return string.IsNullOrWhiteSpace(ExchangeRate.SourceType);
        }

        bool HasStatusErrors()
        {
            return string.IsNullOrWhiteSpace(ExchangeRate.Status);
        }

        /// <summary>
        /// Validates dropdown selections and sets validation error messages
        /// </summary>
        private bool ValidateDropdowns()
        {
            bool isValid = true;

            // Clear all validation errors first
            BaseCurrencyValidationError = null;
            QuoteCurrencyValidationError = null;
            RateTypeValidationError = null;
            SourceTypeValidationError = null;
            StatusValidationError = null;
            RateValidationError = null;
            showCurrencyPairError = false;

            // Validate Base Currency
            if (ExchangeRate.BaseCurrencyId == Guid.Empty)
            {
                BaseCurrencyValidationError = "Base Currency is required";
                isValid = false;
            }

            // Validate Quote Currency
            if (ExchangeRate.QuoteCurrencyId == Guid.Empty)
            {
                QuoteCurrencyValidationError = "Quote Currency is required";
                isValid = false;
            }

            // Validate that Base and Quote are different
            if (ExchangeRate.BaseCurrencyId != Guid.Empty &&
                ExchangeRate.QuoteCurrencyId != Guid.Empty &&
                ExchangeRate.BaseCurrencyId == ExchangeRate.QuoteCurrencyId)
            {
                showCurrencyPairError = true;
                QuoteCurrencyValidationError = "Quote Currency must be different from Base Currency";
                isValid = false;
            }

            // Validate Rate
            if (ExchangeRate.Rate <= 0)
            {
                RateValidationError = "Exchange Rate must be greater than 0";
                isValid = false;
            }

            // Validate Rate Type
            if (string.IsNullOrWhiteSpace(ExchangeRate.RateType))
            {
                RateTypeValidationError = "Rate Type is required";
                isValid = false;
            }

            // Validate Source Type
            if (string.IsNullOrWhiteSpace(ExchangeRate.SourceType))
            {
                SourceTypeValidationError = "Source Type is required";
                isValid = false;
            }

            // Validate Status
            if (string.IsNullOrWhiteSpace(ExchangeRate.Status))
            {
                StatusValidationError = "Status is required";
                isValid = false;
            }

            return isValid;
        }

        private string GetCurrencyCode(Guid currencyId)
        {
            var currency = Currencies.FirstOrDefault(c => c.Id == currencyId);
            return currency?.CurrencyCode ?? "-";
        }

        private async Task FetchLiveRateAsync()
        {
            if (ExchangeRate.BaseCurrencyId == Guid.Empty || ExchangeRate.QuoteCurrencyId == Guid.Empty)
            {
                ToastService.ShowWarning("Please select both Base and Quote currencies first", "Select Currencies");
                return;
            }

            if (ExchangeRate.BaseCurrencyId == ExchangeRate.QuoteCurrencyId)
            {
                showCurrencyPairError = true;
                ToastService.ShowWarning("Base and Quote currencies must be different", "Invalid Selection");
                return;
            }

            var baseCurrency = Currencies.FirstOrDefault(c => c.Id == ExchangeRate.BaseCurrencyId);
            var quoteCurrency = Currencies.FirstOrDefault(c => c.Id == ExchangeRate.QuoteCurrencyId);

            if (baseCurrency == null || quoteCurrency == null)
            {
                ToastService.ShowError("Currency not found", "Error");
                return;
            }

            isFetchingLiveRate = true;
            liveRateResult = null;
            StateHasChanged();

            try
            {
                liveRateResult = await LiveRateService.GetLiveRateAsync(
                    baseCurrency.CurrencyCode,
                    quoteCurrency.CurrencyCode);

                if (liveRateResult.Success)
                {
                    ToastService.ShowSuccess($"Live rate fetched: {liveRateResult.Rate:N8}", "Success");
                }
                else
                {
                    ToastService.ShowError(liveRateResult.ErrorMessage ?? "Failed to fetch live rate", "Error");
                }
            }
            catch (Exception ex)
            {
                ToastService.ShowError($"Error: {ex.Message}", "Error");
            }
            finally
            {
                isFetchingLiveRate = false;
                StateHasChanged();
            }
        }

        private void ApplyLiveRate()
        {
            if (liveRateResult != null && liveRateResult.Success)
            {
                ExchangeRate.Rate = liveRateResult.Rate;
                ExchangeRate.SourceType = "LiveAPI";
                ExchangeRate.SourceName = liveRateResult.Provider ?? "ExchangeRate-API";
                ExchangeRate.Notes = string.IsNullOrEmpty(ExchangeRate.Notes)
                    ? $"Live rate fetched on {DateTime.Now:dd MMM yyyy HH:mm}"
                    : ExchangeRate.Notes + $"\nLive rate fetched on {DateTime.Now:dd MMM yyyy HH:mm}";

                ToastService.ShowSuccess("Live rate applied", "Applied");
                StateHasChanged();
            }
        }

        private async Task HandleSubmit()
        {
            if (_notesEditor != null)
                ExchangeRate.Notes = await _notesEditor.GetHtmlAsync();
            // Validate using EditContext first
            var isFormValid = _editContext.Validate();

            // Then validate dropdowns
            var areDropdownsValid = ValidateDropdowns();

            if (isFormValid && areDropdownsValid)
            {
                Save();
                return;
            }

            // Open ALL accordions that have validation errors
            if (HasIdentityErrors())
                OpenAccordion("rateIdentity");
            if (HasRateValueErrors())
                OpenAccordion("rateValue");
            if (HasSourceErrors())
                OpenAccordion("rateSource");
            if (HasStatusErrors())
                OpenAccordion("statusGovernance");

            await InvokeAsync(StateHasChanged);
        }

        private void Save()
        {
            // Validate dropdowns first
            if (!ValidateDropdowns())
            {
                // Open ALL accordions with errors
                if (HasIdentityErrors())
                    OpenAccordion("rateIdentity");
                if (HasRateValueErrors())
                    OpenAccordion("rateValue");
                if (HasSourceErrors())
                    OpenAccordion("rateSource");
                if (HasStatusErrors())
                    OpenAccordion("statusGovernance");
                return;
            }

            // Set CompanyId from selected string
            if (!string.IsNullOrWhiteSpace(SelectedCompanyId) && Guid.TryParse(SelectedCompanyId, out var companyId))
            {
                ExchangeRate.CompanyId = companyId;
            }
            else
            {
                ExchangeRate.CompanyId = null;
            }

            // Populate display names
            var baseCurrency = Currencies.FirstOrDefault(c => c.Id == ExchangeRate.BaseCurrencyId);
            var quoteCurrency = Currencies.FirstOrDefault(c => c.Id == ExchangeRate.QuoteCurrencyId);

            if (baseCurrency != null)
            {
                ExchangeRate.BaseCurrencyCode = baseCurrency.CurrencyCode;
                ExchangeRate.BaseCurrencyName = baseCurrency.CurrencyName;
            }

            if (quoteCurrency != null)
            {
                ExchangeRate.QuoteCurrencyCode = quoteCurrency.CurrencyCode;
                ExchangeRate.QuoteCurrencyName = quoteCurrency.CurrencyName;
            }

            if (ExchangeRate.CompanyId.HasValue)
            {
                var company = Companies.FirstOrDefault(c => c.Id == ExchangeRate.CompanyId.Value);
                if (company != null)
                {
                    ExchangeRate.CompanyCode = company.CompanyCode;
                    ExchangeRate.CompanyName = company.LegalName;
                }
            }
            else
            {
                ExchangeRate.CompanyCode = null;
                ExchangeRate.CompanyName = null;
            }

            if (IsEdit)
            {
                // Only allow editing Draft rates
                if (ExchangeRate.Status != "Draft")
                {
                    OpenAccordion("statusGovernance");
                    ToastService.ShowError($"Only Draft records are allowed to be edited.", "Error");
                    return;
                }

                MasterDataService.UpdateExchangeRate(ExchangeRate);
                ToastService.ShowSuccess($"Exchange rate for {ExchangeRate.CurrencyPair} updated successfully", "Updated");
            }
            else
            {
                MasterDataService.AddExchangeRate(ExchangeRate);
                ToastService.ShowSuccess($"Exchange rate for {ExchangeRate.CurrencyPair} added successfully", "Added");
            }

            Nav.NavigateTo("/exchange-rates");
        }
    }
}
