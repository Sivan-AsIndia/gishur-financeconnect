using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Linq.Expressions;
using FinanceConnect.Client.Shared;

namespace FinanceConnect.Client.Pages.Master.TimeZone
{
    public partial class AddTimeZone
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        private EditContext _editContext;
        RichTextEditor? _notesEditor;
        [Parameter] public Guid? Id { get; set; }

        private bool isInitialized = false;

        private TimeZoneModel TimeZone = CreateNewTimeZone();
        private List<CountryModel> Countries = new();

        private bool IsEdit => Id.HasValue;
        private string PageTitle => IsEdit ? "Edit Time Zone" : "Create Time Zone";
        private string PageSubTitle => IsEdit ? "Update time zone details" : "Create new time zone";

        private static TimeZoneModel CreateNewTimeZone()
        {
            return new TimeZoneModel
            {
                Status = null,
                IsActive = false,
                SupportsDST = false,
                IsDefaultRecommended = false,
                SortOrder = 0,
                StandardUtcOffsetMinutes = 0
            };
        }

        // Custom validation error messages
        string? StatusValidationError = null;

        // Property wrapper for Status dropdown
        private string SelectedTimeZoneStatus
        {
            get => TimeZone.Status ?? "";
            set
            {
                TimeZone.Status = string.IsNullOrEmpty(value) ? null : value;
                StatusValidationError = null;
            }
        }

        void OnStatusChanged()
        {
            StatusValidationError = null;
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

        protected override async Task OnInitializedAsync()
        {
            Countries = MasterDataService.GetAllCountries().Where(c => c.IsActive).ToList();

            if (IsEdit)
            {
                var existing = MasterDataService.GetAllTimeZones().FirstOrDefault(t => t.Id == Id!.Value);
                if (existing != null)
                {
                    TimeZone = new TimeZoneModel
                    {
                        Id = existing.Id,
                        TenantId = existing.TenantId,
                        TimeZoneKey = existing.TimeZoneKey,
                        DisplayName = existing.DisplayName,
                        ShortName = existing.ShortName,
                        CountryId = existing.CountryId,
                        CountryName = existing.CountryName,
                        StandardUtcOffsetMinutes = existing.StandardUtcOffsetMinutes,
                        SupportsDST = existing.SupportsDST,
                        DSTRuleNote = existing.DSTRuleNote,
                        SortOrder = existing.SortOrder,
                        IsDefaultRecommended = existing.IsDefaultRecommended,
                        Status = existing.Status,
                        IsActive = existing.IsActive,
                        IsDeleted = existing.IsDeleted,
                        Notes = existing.Notes,
                        CreatedAt = existing.CreatedAt,
                        CreatedBy = existing.CreatedBy,
                        UpdatedAt = existing.UpdatedAt,
                        UpdatedBy = existing.UpdatedBy
                    };
                }
                else
                {
                    Nav.NavigateTo("/timezones");
                    return;
                }
            }

            isInitialized = true;
            _editContext = new EditContext(TimeZone);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
        }

        private async Task HandleSubmit()
        {
            // Collect Quill editor values before validation
            if (_notesEditor != null)
                TimeZone.Notes = await _notesEditor.GetHtmlAsync();

            // Validate DataAnnotations first
            var isDataAnnotationsValid = _editContext.Validate();

            // Custom validation for Status
            StatusValidationError = null;
            bool isCustomValid = true;

            if (string.IsNullOrWhiteSpace(TimeZone.Status))
            {
                StatusValidationError = "Status is required";
                isCustomValid = false;
            }

            if (!isDataAnnotationsValid || !isCustomValid)
            {
                return;
            }

            Save();
        }

        void onKeyChange()
        {
            TimeZone.TimeZoneKey = TimeZone.TimeZoneKey?.Trim() ?? "";
        }

        void onDisplayNameChange()
        {
            TimeZone.DisplayName = TimeZone.DisplayName?.Trim() ?? "";
        }

        void onShortNameChange()
        {
            TimeZone.ShortName = TimeZone.ShortName?.Trim() ?? "";
        }

        private void Save()
        {
            TimeZone.TimeZoneKey = TimeZone.TimeZoneKey?.Trim() ?? "";
            TimeZone.DisplayName = TimeZone.DisplayName?.Trim() ?? "";
            TimeZone.ShortName = TimeZone.ShortName?.Trim();
            TimeZone.DSTRuleNote = TimeZone.DSTRuleNote?.Trim();
            TimeZone.Notes = TimeZone.Notes?.Trim();

            // Ensure SortOrder is not negative
            if (TimeZone.SortOrder < 0) TimeZone.SortOrder = 0;

            // Validate IANA format (basic: contains '/')
            if (!TimeZone.TimeZoneKey.Contains('/'))
            {
                ToastService.ShowError("Time Zone ID must be in IANA format (e.g., Asia/Kolkata)", "Validation Error");
                return;
            }

            // Set country name for display
            var country = Countries.FirstOrDefault(c => c.Id == TimeZone.CountryId);
            TimeZone.CountryName = country?.CountryName;

            // Set IsActive based on Status
            TimeZone.IsActive = TimeZone.Status == "Active";

            if (IsEdit)
            {
                TimeZone.UpdatedAt = DateTime.Now;
                TimeZone.UpdatedBy = "System";
                MasterDataService.UpdateTimeZone(TimeZone);
                ToastService.ShowSuccess($"Time Zone '{TimeZone.DisplayName}' updated successfully", "Updated");
            }
            else
            {
                // Duplicate check
                var timezones = MasterDataService.GetAllTimeZones();
                if (timezones.Any(t => t.TimeZoneKey.Equals(TimeZone.TimeZoneKey, StringComparison.OrdinalIgnoreCase)))
                {
                    ToastService.ShowError("Time Zone ID already exists", "Validation Error");
                    return;
                }

                TimeZone.Id = Guid.NewGuid();
                TimeZone.CreatedAt = DateTime.Now;
                TimeZone.CreatedBy = "System";
                MasterDataService.AddTimeZone(TimeZone);
                ToastService.ShowSuccess($"Time Zone '{TimeZone.DisplayName}' added successfully", "Added");
            }

            // Navigate back to list - Blazor will properly reload the component
            Nav.NavigateTo("/timezones");
        }
    }

    // Time Zone Status
    public static class TimeZoneStatus
    {
        public const string Draft = "Draft";
        public const string Active = "Active";
        public const string Inactive = "Inactive";

        public static readonly string[] All = new[] { Draft, Active, Inactive };
    }
}
