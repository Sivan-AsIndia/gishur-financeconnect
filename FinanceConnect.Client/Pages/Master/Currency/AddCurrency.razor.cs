using FinanceConnect.Client.Services;
using FinanceConnect.Client.Shared;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Linq.Expressions;

namespace FinanceConnect.Client.Pages.Master.Currency
{
    public partial class AddCurrency : ComponentBase
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;

        [Parameter] public Guid? Id { get; set; }

        public bool isInitialized = false;
        public CurrencyModel Currency = CreateNewCurrency();
        private EditContext _editContext;
        RichTextEditor? _notesEditor;
        public bool IsEdit => Id.HasValue;
        public string PageTitle => IsEdit ? "Edit Currency" : "Create Currency";
        public string PageSubTitle => IsEdit ? "Update currency details" : "Create new currency";

        public string RoundingStepString
        {
            get => Currency.RoundingStep?.ToString("F2") ?? "";
            set => Currency.RoundingStep = string.IsNullOrEmpty(value) ? null : decimal.Parse(value);
        }


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

        private static CurrencyModel CreateNewCurrency()
        {
            return new CurrencyModel
            {
                IsActive = true,
                CurrencyType = "",
                SymbolPosition = "",
                DecimalPlaces = null,
                RoundingMode = ""
            };
        }

        protected override async Task OnInitializedAsync()
        {
            if (IsEdit)
            {
                var existing = MasterDataService.GetCurrencyById(Id!.Value);
                if (existing != null)
                {
                    Currency = new CurrencyModel
                    {
                        Id = existing.Id,
                        TenantId = existing.TenantId,
                        CurrencyCode = existing.CurrencyCode,
                        CurrencyName = existing.CurrencyName,
                        NumericCode = existing.NumericCode,
                        CurrencyType = existing.CurrencyType,
                        Symbol = existing.Symbol,
                        SymbolPosition = existing.SymbolPosition,
                        DisplayFormat = existing.DisplayFormat,
                        DecimalPlaces = existing.DecimalPlaces,
                        MinorUnitName = existing.MinorUnitName,
                        RoundingMode = existing.RoundingMode,
                        RoundingStep = existing.RoundingStep,
                        IsActive = existing.IsActive,
                        Notes = existing.Notes,
                        CreatedAt = existing.CreatedAt,
                        CreatedBy = existing.CreatedBy
                    };
                }
                else
                {
                    Nav.NavigateTo("/currencies");
                    return;
                }
            }

            isInitialized = true;
            _editContext = new EditContext(Currency);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
        }

        private async Task Save()
        {
            // Collect Quill editor values before validation
            if (_notesEditor != null)
                Currency.Notes = await _notesEditor.GetHtmlAsync();
            // Convert code to uppercase
            Currency.CurrencyCode = Currency.CurrencyCode?.ToUpper().Trim() ?? "";

            if (IsEdit)
            {
                Currency.UpdatedAt = DateTime.Now;
                Currency.UpdatedBy = "System";
                MasterDataService.UpdateCurrency(Currency);
                ToastService.ShowSuccess($"Currency '{Currency.CurrencyCode}' updated successfully", "Updated");
            }
            else
            {
                // Duplicate check
                var currencies = MasterDataService.GetAllCurrencies();
                if (currencies.Any(c => c.CurrencyCode.Equals(Currency.CurrencyCode, StringComparison.OrdinalIgnoreCase)))
                {
                    ToastService.ShowError("Currency code already exists", "Validation Error");
                    return;
                }

                Currency.Id = Guid.NewGuid();
                Currency.CreatedAt = DateTime.Now;
                Currency.CreatedBy = "System";
                MasterDataService.AddCurrency(Currency);
                ToastService.ShowSuccess($"Currency '{Currency.CurrencyCode}' added successfully", "Added");
            }

            // Navigate back to list - Blazor will properly reload the component
            Nav.NavigateTo("/currencies");
        }

        private Task OnCurrencyNameInput()
        {
            Currency.CurrencyName = Currency.CurrencyName?.Trim() ?? "";
            return Task.CompletedTask;
        }
    }
}
