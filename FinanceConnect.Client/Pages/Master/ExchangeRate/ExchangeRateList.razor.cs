using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Master.ExchangeRate
{
    public partial class ExchangeRateList
    {
        [Inject]
        private LiveExchangeRateService LiveRateService { get; set; } = default!;

        private bool isInitialized = false;
        private bool isLoading = false;

        private List<ExchangeRateModel> ExchangeRates = new();
        private List<ExchangeRateModel> FilteredExchangeRates = new();
        private List<CurrencyModel> Currencies = new();
        private List<CompanyModel> Companies = new();

        private ExchangeRateModel? SelectedRate;

        private bool canDeactivate = true;
        private bool canDelete = true;

        private string searchText = "";
        private string selectedStatus = "";
        private string selectedBaseCurrencyId = "";
        private string selectedQuoteCurrencyId = "";
        private string selectedRateType = "";
        private string selectedCompanyFilter = "";

        // Live Rate Checker State
        private bool isLiveRatePanelOpen = false;
        private bool isFetchingLiveRate = false;
        private string liveRateBaseCurrency = "USD";
        private string liveRateQuoteCurrency = "INR";
        private LiveRateResult? liveRateResult;
        private LiveRatesResult? allLiveRatesResult;
        private string liveRateErrorMessage = "";
        // Add these fields after line 40 (after liveRateErrorMessage)

        // Per-row live rate state
        private Dictionary<Guid, LiveRateResult?> rowLiveRates = new();
        private Dictionary<Guid, bool> rowLiveRateFetching = new();
        private Guid? currentLiveRateId = null;
        private bool showLiveRateComparison = false;
        private List<LiveRateComparisonItem> liveRateComparisons = new();
        private int VisibleColumnCount;
        private int TotalPages => FilteredExchangeRates.Count == 0 ? 1 : (int)Math.Ceiling((double)FilteredExchangeRates.Count / PageSize);

        private List<ExchangeRateModel> PagedExchangeRates => FilteredExchangeRates.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

        // Aliases for razor binding compatibility
        private List<ExchangeRateModel> PagedRates => PagedExchangeRates;
        private List<ExchangeRateModel> FilteredRates => FilteredExchangeRates;

        private string selectedCurrencyId = "";
        private string SelectedCurrencyId
        {
            get => selectedCurrencyId;
            set
            {
                selectedCurrencyId = value;
                // Apply to both base and quote for general filter
                selectedBaseCurrencyId = value;
                selectedQuoteCurrencyId = "";
                ApplyFilters();
            }
        }

        protected override async Task OnInitializedAsync()
        {


            LoadData();
            isInitialized = true;
        }

        protected override void OnParametersSet()
        {
            // Reload data every time we navigate to this page
            // This ensures the list is updated after Add/Edit/Delete operations
            if (isInitialized)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            // Load current data without resetting to seed
            // (Reset only happens when user clicks Refresh button)
            // Load reference data
            Currencies = MasterDataService.GetAllCurrencies().Where(c => !c.IsDeleted).ToList();
            Companies = MasterDataService.GetAllCompanies().Where(c => !c.IsDeleted).ToList();

            // Load exchange rates (without resetting to seed - preserves Add/Edit changes)
            ExchangeRates = MasterDataService.GetAllExchangeRates();
            FilteredExchangeRates = ExchangeRates.OrderByDescending(e => e.RateDate).ThenBy(e => e.CurrencyPair).ToList();
        }
        private ExchangeRateModel? SelectedCompany;
        void OpenRowDetails(ExchangeRateModel company)
        {
            SelectedCompany = company;
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            VisibleColumnCount =
 await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        #region Filter Properties

        private string SelectedStatus
        {
            get => selectedStatus;
            set { selectedStatus = value; ApplyFilters(); }
        }

        private string SelectedBaseCurrencyId
        {
            get => selectedBaseCurrencyId;
            set { selectedBaseCurrencyId = value; ApplyFilters(); }
        }

        private string SelectedQuoteCurrencyId
        {
            get => selectedQuoteCurrencyId;
            set { selectedQuoteCurrencyId = value; ApplyFilters(); }
        }

        private string SelectedRateType
        {
            get => selectedRateType;
            set { selectedRateType = value; ApplyFilters(); }
        }

        private string SelectedCompanyFilter
        {
            get => selectedCompanyFilter;
            set { selectedCompanyFilter = value; ApplyFilters(); }
        }

        #endregion

        #region Search and Filters

        private async Task OnSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString() ?? "";
            ApplyFilters();
            VisibleColumnCount =
 await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        private void ApplyFilters()
        {
            IEnumerable<ExchangeRateModel> query = ExchangeRates;

            // Text search - search by currency pair
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(e =>
                    e.CurrencyPair.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    (e.BaseCurrencyCode?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (e.QuoteCurrencyCode?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (e.BaseCurrencyName?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (e.QuoteCurrencyName?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (e.SourceName?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            // Base Currency filter
            if (!string.IsNullOrWhiteSpace(selectedBaseCurrencyId) && Guid.TryParse(selectedBaseCurrencyId, out var baseCurrencyId))
            {
                query = query.Where(e => e.BaseCurrencyId == baseCurrencyId);
            }

            // Quote Currency filter
            if (!string.IsNullOrWhiteSpace(selectedQuoteCurrencyId) && Guid.TryParse(selectedQuoteCurrencyId, out var quoteCurrencyId))
            {
                query = query.Where(e => e.QuoteCurrencyId == quoteCurrencyId);
            }

            // Rate Type filter
            if (!string.IsNullOrWhiteSpace(selectedRateType))
            {
                query = query.Where(e => e.RateType == selectedRateType);
            }

            // Company filter
            if (!string.IsNullOrWhiteSpace(selectedCompanyFilter))
            {
                if (selectedCompanyFilter == "tenant-default")
                {
                    query = query.Where(e => !e.CompanyId.HasValue);
                }
                else if (Guid.TryParse(selectedCompanyFilter, out var companyId))
                {
                    query = query.Where(e => e.CompanyId == companyId);
                }
            }

            // Status filter
            if (!string.IsNullOrWhiteSpace(selectedStatus))
            {
                query = query.Where(e => e.Status == selectedStatus);
            }

            FilteredExchangeRates = query.OrderByDescending(e => e.RateDate).ThenBy(e => e.CurrencyPair).ToList();
            CurrentPage = 1;
        }

        #endregion

        #region Pagination

        private async Task OnPageSizeChange(ChangeEventArgs e)
        {
            isLoading = true;
            StateHasChanged();
            await Task.Delay(200);

            PageSize = int.Parse(e.Value?.ToString() ?? "10");
            CurrentPage = 1;

            isLoading = false;
            StateHasChanged();
        }

        private async Task GoToPage(int page)
        {
            if (page >= 1 && page <= TotalPages)
            {
                isLoading = true;
                StateHasChanged();
                await Task.Delay(200);

                CurrentPage = page;

                isLoading = false;
                StateHasChanged();
            }
        }

        int PageWindowSize = 2;
        int EndPage => Math.Min(StartPage + PageWindowSize - 1, TotalPages);
        int StartPage = 1;
        private int CurrentPage = 1;
        private int PageSize = 10;

        private async Task PreviousPage()
        {
            if (CurrentPage > 1) await GoToPage(CurrentPage - 1);
        }

        private async Task NextPage()
        {
            if (CurrentPage < TotalPages) await GoToPage(CurrentPage + 1);
        }



        #endregion

        #region Refresh

        private async Task OnRefreshAsync()
        {
            isLoading = true;
            StateHasChanged();
            await Task.Delay(150);

            searchText = "";
            selectedStatus = "";
            selectedBaseCurrencyId = "";
            selectedQuoteCurrencyId = "";
            selectedRateType = "";
            SelectedCurrencyId = "";
            selectedCompanyFilter = "";

            // Demo behavior: Refresh should restore the original sample dataset.
            MasterDataService.ResetExchangeRatesToSeed();
            ExchangeRates = MasterDataService.GetAllExchangeRates();
            FilteredExchangeRates = ExchangeRates.OrderByDescending(e => e.RateDate).ThenBy(e => e.CurrencyPair).ToList();
            CurrentPage = 1;
            ToastService.ShowInfo("Data refreshed", "Refresh");

            isLoading = false;
            StateHasChanged();
        }

        #endregion

        #region View Rate

        private async Task ViewRate(Guid rateId)
        {
            //isLoading = true;
            //StateHasChanged();
            //await Task.Delay(200);
            Nav.NavigateTo($"/exchange-rates/{rateId}/view");
            //SelectedRate = rate;

            //isLoading = false;
            //StateHasChanged();
        }

        #endregion

        #region Activate/Deactivate/Delete

        private void ConfirmActivate(ExchangeRateModel rate) => SelectedRate = rate;

        private void ActivateConfirmed()
        {
            if (SelectedRate != null)
            {
                MasterDataService.ActivateExchangeRate(SelectedRate.Id);
                ExchangeRates = MasterDataService.GetAllExchangeRates();
                ApplyFilters();
                ToastService.ShowSuccess($"Exchange rate for {SelectedRate.CurrencyPair} activated successfully", "Activated");
                SelectedRate = null;
            }
        }

        private void ConfirmDeactivate(ExchangeRateModel rate)
        {
            SelectedRate = rate;
            canDeactivate = MasterDataService.CanDeactivateExchangeRate(rate.Id);
        }

        private void DeactivateConfirmed()
        {
            if (SelectedRate != null && canDeactivate)
            {
                MasterDataService.DeactivateExchangeRate(SelectedRate.Id);
                ExchangeRates = MasterDataService.GetAllExchangeRates();
                ApplyFilters();
                ToastService.ShowWarning($"Exchange rate for {SelectedRate.CurrencyPair} deactivated successfully", "Deactivated");
                SelectedRate = null;
            }
        }

        private void ConfirmDelete(ExchangeRateModel rate)
        {
            SelectedRate = rate;
            canDelete = MasterDataService.CanDeleteExchangeRate(rate.Id);
        }

        private void DeleteConfirmed()
        {
            if (SelectedRate != null && canDelete)
            {
                MasterDataService.DeleteExchangeRate(SelectedRate.Id);
                ExchangeRates = MasterDataService.GetAllExchangeRates();
                FilteredExchangeRates = ExchangeRates.OrderByDescending(e => e.RateDate).ThenBy(e => e.CurrencyPair).ToList();
                ToastService.ShowError($"Exchange rate for {SelectedRate.CurrencyPair} deleted successfully", "Deleted");
                SelectedRate = null;
                CurrentPage = 1;
            }
        }

        #endregion

        #region Helper Methods

        private string GetStatusBadge(string status)
        {
            return status switch
            {
                "Active" => "bg-success text-success",
                "Inactive" => "bg-danger text-danger",
                "Draft" => "bg-warning text-warning",
                _ => "bg-secondary text-secondary"
            };
        }


        private string GetRateTypeBadge(string rateType)
        {
            return rateType switch
            {
                "Spot" => "bg-primary-transparent text-primary",
                "MonthAverage" => "bg-info-transparent text-info",
                "Customs" => "bg-purple-transparent text-purple",
                "BankRate" => "bg-success-transparent text-success",
                "Manual" => "bg-secondary-transparent text-secondary",
                _ => "bg-light text-dark"
            };
        }

        #endregion

        #region Live Rate Checker Methods

        private void ToggleLiveRatePanel()
        {
            isLiveRatePanelOpen = !isLiveRatePanelOpen;
            if (isLiveRatePanelOpen && liveRateResult == null)
            {
                // Default fetch on first open
                _ = FetchLiveRateAsync();
            }
        }

        private async Task FetchLiveRateAsync()
        {
            if (string.IsNullOrWhiteSpace(liveRateBaseCurrency) || string.IsNullOrWhiteSpace(liveRateQuoteCurrency))
            {
                liveRateErrorMessage = "Please select both currencies";
                return;
            }

            if (liveRateBaseCurrency == liveRateQuoteCurrency)
            {
                liveRateErrorMessage = "Base and Quote currencies must be different";
                return;
            }

            isFetchingLiveRate = true;
            liveRateErrorMessage = "";
            liveRateResult = null;
            StateHasChanged();

            try
            {
                liveRateResult = await LiveRateService.GetLiveRateAsync(liveRateBaseCurrency, liveRateQuoteCurrency);

                if (!liveRateResult.Success)
                {
                    liveRateErrorMessage = liveRateResult.ErrorMessage ?? "Failed to fetch rate";
                }
            }
            catch (Exception ex)
            {
                liveRateErrorMessage = $"Error: {ex.Message}";
            }
            finally
            {
                isFetchingLiveRate = false;
                StateHasChanged();
            }
        }

        private async Task FetchAllLiveRatesAsync()
        {
            if (string.IsNullOrWhiteSpace(liveRateBaseCurrency))
            {
                liveRateErrorMessage = "Please select a base currency";
                return;
            }

            isFetchingLiveRate = true;
            liveRateErrorMessage = "";
            allLiveRatesResult = null;
            StateHasChanged();

            try
            {
                allLiveRatesResult = await LiveRateService.GetAllLiveRatesAsync(liveRateBaseCurrency);

                if (!allLiveRatesResult.Success)
                {
                    liveRateErrorMessage = allLiveRatesResult.ErrorMessage ?? "Failed to fetch rates";
                }
            }
            catch (Exception ex)
            {
                liveRateErrorMessage = $"Error: {ex.Message}";
            }
            finally
            {
                isFetchingLiveRate = false;
                StateHasChanged();
            }
        }

        private async Task CompareWithStoredRatesAsync()
        {
            isFetchingLiveRate = true;
            liveRateErrorMessage = "";
            liveRateComparisons.Clear();
            StateHasChanged();

            try
            {
                // Get unique base currencies from active rates
                var activePairs = ExchangeRates
                    .Where(e => e.Status == "Active" && !e.IsDeleted)
                    .GroupBy(e => new { e.BaseCurrencyCode, e.QuoteCurrencyCode })
                    .Select(g => g.OrderByDescending(e => e.RateDate).First())
                    .ToList();

                foreach (var storedRate in activePairs)
                {
                    if (string.IsNullOrWhiteSpace(storedRate.BaseCurrencyCode) ||
                        string.IsNullOrWhiteSpace(storedRate.QuoteCurrencyCode))
                        continue;

                    var liveResult = await LiveRateService.GetLiveRateAsync(
                        storedRate.BaseCurrencyCode,
                        storedRate.QuoteCurrencyCode);

                    if (liveResult.Success)
                    {
                        var variance = storedRate.Rate > 0
                            ? ((liveResult.Rate - storedRate.Rate) / storedRate.Rate) * 100
                            : 0;

                        liveRateComparisons.Add(new LiveRateComparisonItem
                        {
                            CurrencyPair = storedRate.CurrencyPair,
                            StoredRate = storedRate.Rate,
                            StoredRateDate = storedRate.RateDate,
                            LiveRate = liveResult.Rate,
                            LiveRateDate = liveResult.LastUpdated,
                            VariancePercent = variance,
                            Provider = liveResult.Provider ?? "API"
                        });
                    }
                }

                showLiveRateComparison = true;
            }
            catch (Exception ex)
            {
                liveRateErrorMessage = $"Error comparing rates: {ex.Message}";
            }
            finally
            {
                isFetchingLiveRate = false;
                StateHasChanged();
            }
        }

        private void ApplyLiveRateToNewRate()
        {
            if (liveRateResult != null && liveRateResult.Success)
            {
                // Find currency IDs
                var baseCurrency = Currencies.FirstOrDefault(c =>
                    c.CurrencyCode == liveRateResult.BaseCurrency && c.IsActive);
                var quoteCurrency = Currencies.FirstOrDefault(c =>
                    c.CurrencyCode == liveRateResult.QuoteCurrency && c.IsActive);

                if (baseCurrency != null && quoteCurrency != null)
                {
                    ToastService.ShowSuccess($"Navigate to Add page with live rate {liveRateResult.Rate:N8}", "Rate Applied");
                    isLiveRatePanelOpen = false;
                    StateHasChanged();

                    // Navigate to Add page
                    Nav.NavigateTo("/create-exchangerate");
                }
                else
                {
                    ToastService.ShowWarning("Currency not found in system. Please add the currency first.", "Currency Not Found");
                }
            }
        }

        private string GetVarianceBadgeClass(decimal variance)
        {
            return variance switch
            {
                var v when Math.Abs(v) < 0.1m => "bg-success-transparent text-success",
                var v when Math.Abs(v) < 0.5m => "bg-info-transparent text-info",
                var v when Math.Abs(v) < 1.0m => "bg-warning-transparent text-warning",
                _ => "bg-danger-transparent text-danger"
            };
        }

        private string GetVarianceIcon(decimal variance)
        {
            if (variance > 0.01m) return "ti ti-trending-up text-success";
            if (variance < -0.01m) return "ti ti-trending-down text-danger";
            return "ti ti-minus text-muted";
        }

        #region Row Live Rate Methods

        /// <summary>
        /// Fetch live rate for a specific exchange rate row
        /// </summary>
        private async Task FetchRowLiveRateAsync(ExchangeRateModel rate)
        {
            if (rate == null || string.IsNullOrWhiteSpace(rate.BaseCurrencyCode) || string.IsNullOrWhiteSpace(rate.QuoteCurrencyCode))
                return;

            // Set fetching state
            rowLiveRateFetching[rate.Id] = true;
            currentLiveRateId = rate.Id;
            StateHasChanged();

            try
            {
                var result = await LiveRateService.GetLiveRateAsync(rate.BaseCurrencyCode, rate.QuoteCurrencyCode);
                rowLiveRates[rate.Id] = result;
            }
            catch (Exception)
            {
                rowLiveRates[rate.Id] = new LiveRateResult
                {
                    Success = false,
                    ErrorMessage = "Failed to fetch live rate"
                };
            }
            finally
            {
                rowLiveRateFetching[rate.Id] = false;
                StateHasChanged();
            }
        }

        /// <summary>
        /// Check if a row is currently fetching live rate
        /// </summary>
        private bool IsRowFetchingLiveRate(Guid rateId)
        {
            return rowLiveRateFetching.ContainsKey(rateId) && rowLiveRateFetching[rateId];
        }

        /// <summary>
        /// Get live rate result for a row
        /// </summary>
        private LiveRateResult? GetRowLiveRate(Guid rateId)
        {
            return rowLiveRates.ContainsKey(rateId) ? rowLiveRates[rateId] : null;
        }

        /// <summary>
        /// Check if live rate is displayed for a row
        /// </summary>
        private bool IsLiveRateDisplayed(Guid rateId)
        {
            return currentLiveRateId == rateId && rowLiveRates.ContainsKey(rateId) && rowLiveRates[rateId] != null;
        }

        /// <summary>
        /// Close live rate display
        /// </summary>
        private void CloseLiveRateDisplay()
        {
            currentLiveRateId = null;
            StateHasChanged();
        }

        /// <summary>
        /// Toggle live rate display for a row
        /// </summary>
        private async Task ToggleLiveRateDisplay(ExchangeRateModel rate)
        {
            if (currentLiveRateId == rate.Id)
            {
                // Close if already open
                CloseLiveRateDisplay();
            }
            else
            {
                // Open and fetch if not already fetched
                if (!rowLiveRates.ContainsKey(rate.Id))
                {
                    await FetchRowLiveRateAsync(rate);
                }
                else
                {
                    currentLiveRateId = rate.Id;
                    StateHasChanged();
                }
            }
        }

        #endregion
        #endregion
    }

    /// <summary>
    /// Model for comparing stored vs live rates
    /// </summary>
    public class LiveRateComparisonItem
    {
        public string CurrencyPair { get; set; } = "";
        public decimal StoredRate { get; set; }
        public DateTime StoredRateDate { get; set; }
        public decimal LiveRate { get; set; }
        public DateTime LiveRateDate { get; set; }
        public decimal VariancePercent { get; set; }
        public string Provider { get; set; } = "";
    }
}
