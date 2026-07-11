using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Master.Currency;

public partial class CurrencyList
{
    private bool isInitialized = false;
    private bool isLoading = false;

    private List<CurrencyModel> Currencies = new();
    private List<CurrencyModel> FilteredCurrencies = new();
    private CurrencyModel? SelectedCurrency;
    private CurrencyModel NewCurrency = new() { IsActive = true, CurrencyType = "Fiat", SymbolPosition = "Prefix", DecimalPlaces = 2, RoundingMode = "Round Half Up" };
    private EditContext? AddCurrencyEditContext;
    private CurrencyModel? EditCurrencyModel;

    // Permission flags
    private bool canDeactivate = true;
    private bool canDelete = true;

    private string searchText = "";
    private string selectedStatus = "";
    private string selectedType = "";
    private int VisibleColumnCount;

    private int TotalPages => FilteredCurrencies.Count == 0 ? 1 : (int)Math.Ceiling((double)FilteredCurrencies.Count / PageSize);

    private List<CurrencyModel> PagedCurrencies => FilteredCurrencies.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

    protected override async Task OnInitializedAsync()
    {
        // Demo behavior: Refresh should restore the original sample dataset.


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
        Currencies = MasterDataService.GetAllCurrencies();
        // Sort by most recent first (UpdatedAt if exists, otherwise CreatedAt)
        FilteredCurrencies = Currencies
            .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
            .ToList();

        AddCurrencyEditContext = new EditContext(NewCurrency);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await JS.InvokeVoidAsync("feather.replace");
        await JS.InvokeVoidAsync("initTooltips");
        VisibleColumnCount =
await JS.InvokeAsync<int>("getVisibleTableColumns");
    }

    private string SelectedStatus
    {
        get => selectedStatus;
        set { selectedStatus = value; ApplyFilters(); }
    }

    private string SelectedType
    {
        get => selectedType;
        set { selectedType = value; ApplyFilters(); }
    }

    private async Task OnSearch(ChangeEventArgs e)
    {
        searchText = e.Value?.ToString() ?? "";
        ApplyFilters();
        VisibleColumnCount =
await JS.InvokeAsync<int>("getVisibleTableColumns");
    }

    private void ApplyFilters()
    {
        IEnumerable<CurrencyModel> query = Currencies;

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            query = query.Where(c =>
                c.CurrencyCode.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                c.CurrencyName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                (c.Symbol?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false)
            );
        }

        if (!string.IsNullOrWhiteSpace(selectedType))
        {
            query = query.Where(c => c.CurrencyType == selectedType);
        }

        if (!string.IsNullOrWhiteSpace(selectedStatus))
        {
            bool isActive = selectedStatus == "active";
            query = query.Where(c => c.IsActive == isActive);
        }

        // Sort by most recent first (UpdatedAt if exists, otherwise CreatedAt)
        FilteredCurrencies = query
            .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
            .ToList();
        CurrentPage = 1;
    }

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



    private async Task OnRefreshAsync()
    {
        isLoading = true;
        StateHasChanged();
        await Task.Delay(150);

        searchText = "";
        selectedStatus = "";
        selectedType = "";
        // Demo behavior: Refresh should restore the original sample dataset.
        MasterDataService.ResetCurrenciesToSeed();
        Currencies = MasterDataService.GetAllCurrencies();
        // Sort by most recent first (UpdatedAt if exists, otherwise CreatedAt)
        FilteredCurrencies = Currencies
            .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
            .ToList();
        CurrentPage = 1;
        ToastService.ShowInfo("Data refreshed", "Refresh");

        isLoading = false;
        StateHasChanged();
    }

    private async Task OpenAddCurrencyModal()
    {
        NewCurrency = new() { IsActive = true, CurrencyType = "Fiat", SymbolPosition = "Prefix", DecimalPlaces = 2, RoundingMode = "Round Half Up" };
        AddCurrencyEditContext = new EditContext(NewCurrency);
        StateHasChanged();
        await JS.InvokeVoidAsync("blazorModal.show", "add-currency-modal");
    }

    private async Task ViewCurrency(CurrencyModel currency)
    {
        isLoading = true;
        StateHasChanged();
        await Task.Delay(200);

        SelectedCurrency = currency;

        isLoading = false;
        StateHasChanged();
    }

    private async Task EditCurrency(CurrencyModel currency)
    {
        isLoading = true;
        StateHasChanged();
        await Task.Delay(200);

        SelectedCurrency = currency;
        EditCurrencyModel = new CurrencyModel
        {
            Id = currency.Id,
            CurrencyCode = currency.CurrencyCode,
            CurrencyName = currency.CurrencyName,
            NumericCode = currency.NumericCode,
            CurrencyType = currency.CurrencyType,
            Symbol = currency.Symbol,
            SymbolPosition = currency.SymbolPosition,
            DisplayFormat = currency.DisplayFormat,
            DecimalPlaces = currency.DecimalPlaces,
            MinorUnitName = currency.MinorUnitName,
            RoundingMode = currency.RoundingMode,
            RoundingStep = currency.RoundingStep,
            IsActive = currency.IsActive,
            Notes = currency.Notes
        };

        isLoading = false;
        StateHasChanged();
    }

    private void SaveEditCurrency()
    {
        if (EditCurrencyModel != null)
        {
            EditCurrencyModel.CurrencyCode = EditCurrencyModel.CurrencyCode?.ToUpper() ?? "";
            MasterDataService.UpdateCurrency(EditCurrencyModel);
            Currencies = MasterDataService.GetAllCurrencies();
            // Sort by most recent first (UpdatedAt if exists, otherwise CreatedAt)
            FilteredCurrencies = Currencies
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToList();
            ToastService.ShowSuccess($"Currency '{EditCurrencyModel.CurrencyName}' updated successfully", "Updated");
            EditCurrencyModel = null;
        }
    }

    private void ConfirmActivate(CurrencyModel currency) => SelectedCurrency = currency;

    private void ActivateConfirmed()
    {
        if (SelectedCurrency != null)
        {
            MasterDataService.ActivateCurrency(SelectedCurrency.Id);
            Currencies = MasterDataService.GetAllCurrencies();
            ApplyFilters();
            ToastService.ShowSuccess($"Currency '{SelectedCurrency.CurrencyName}' activated successfully", "Activated");
            SelectedCurrency = null;
        }
    }

    private void ConfirmDeactivate(CurrencyModel currency) => SelectedCurrency = currency;

    private void DeactivateConfirmed()
    {
        if (SelectedCurrency != null)
        {
            if (SelectedCurrency != null)
            {
                MasterDataService.DeactivateCurrency(SelectedCurrency.Id);
                Currencies = MasterDataService.GetAllCurrencies();
                ApplyFilters();
                ToastService.ShowWarning($"Currency '{SelectedCurrency.CurrencyName}' deactivated successfully", "Deactivated");
                SelectedCurrency = null;
            }
        }
    }

    private void ConfirmDelete(CurrencyModel currency) => SelectedCurrency = currency;

    private void DeleteConfirmed()
    {
        if (SelectedCurrency != null)
        {
            MasterDataService.DeleteCurrency(SelectedCurrency.Id);
            Currencies = MasterDataService.GetAllCurrencies();
            // Sort by most recent first (UpdatedAt if exists, otherwise CreatedAt)
            FilteredCurrencies = Currencies
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToList();
            ToastService.ShowError($"Currency '{SelectedCurrency.CurrencyName}' deleted successfully", "Deleted");
            SelectedCurrency = null;
            CurrentPage = 1;
        }
    }

    private void AddCurrency()
    {
        NewCurrency.CurrencyCode = NewCurrency.CurrencyCode?.ToUpper() ?? "";
        MasterDataService.AddCurrency(NewCurrency);
        Currencies = MasterDataService.GetAllCurrencies();
        // Sort by most recent first (UpdatedAt if exists, otherwise CreatedAt)
        FilteredCurrencies = Currencies
            .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
            .ToList();
        ToastService.ShowSuccess($"Currency '{NewCurrency.CurrencyName}' added successfully", "Success");
        NewCurrency = new() { IsActive = true, CurrencyType = "Fiat", SymbolPosition = "Prefix", DecimalPlaces = 2, RoundingMode = "Round Half Up" };
    }

    private async Task HandleAddCurrencySubmit(EditContext editContext)
    {
        if (AddCurrencyEditContext == null)
            return;

        // If invalid, keep modal open and show validation messages.
        if (!AddCurrencyEditContext.Validate())
            return;

        AddCurrency();
        await JS.InvokeVoidAsync("blazorModal.hide", "add-currency-modal");

        // Reset form so next open doesn't show old validation.
        NewCurrency = new() { IsActive = true, CurrencyType = "Fiat", SymbolPosition = "Prefix", DecimalPlaces = 2, RoundingMode = "Round Half Up" };
        AddCurrencyEditContext = new EditContext(NewCurrency);
    }

    private CurrencyModel? SelectedCompany;

    void OpenRowDetails(CurrencyModel company)
    {
        SelectedCompany = company;
    }
    private string GetCurrencyTypeBadge(string type)
    {
        return type switch
        {
            "Fiat" => "bg-primary-transparent text-primary",
            "Crypto" => "bg-warning-transparent text-warning",
            "Other" => "bg-secondary-transparent text-secondary",
            _ => "bg-light text-dark"
        };
    }
}
