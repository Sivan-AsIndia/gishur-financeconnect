
using FinanceConnect.Client.Pages.Finance.Journal;
using FinanceConnect.Client.Services;
using FinanceConnect.Client.Shared;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Linq.Expressions;
using System.Transactions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FinanceConnect.Client.Pages.TransactionManagement.DocumentNumberSeries
{
    public partial class AddDocumentNumberSeries
    {

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("initFeatherIcons");
        }
        [Parameter] public Guid? Id { get; set; }

        private DocumentNumberSeriesModel series = new();
        private EditContext _editContext;
        RichTextEditor? _descriptionEditor;
        // UI State
        private bool IsEdit => Id.HasValue;
        private string PageTitle => IsEdit ? "Edit Document Number Series" : "Create Document Number Series";
        private string PageSubTitle => IsEdit ? "Update numbering policy" : "Define numbering policy for transactions";

        // Accordion State
        private bool ShowIdentity = true;
        private bool ShowScope = false;
        private bool ShowFormat = false;
        private bool ShowRange = false;
        private bool ShowPreview = false;
        private bool ShowStatus = false;
        private bool ShowAssignment = false;
        bool IsInitializing = true;

        bool IdentityTouched = false;
        bool ScopeTouched = false;
        bool FormatTouched = false;
        bool RangeTouched = false;
        bool PreviewTouched = false;
        bool StatusTouched = false;
        bool AssignmentTouched = false;

        void TouchIdentity() => IdentityTouched = true;
        void TouchScope() => ScopeTouched = true;
        void TouchFormat() => FormatTouched = true;
        void TouchRange() => RangeTouched = true;
        
        void TouchPreview() => PreviewTouched = true;
        void TouchStatus() => StatusTouched = true;
        void TouchAssignment() => AssignmentTouched = true;
        // Preview
        private DateTime PreviewDate = DateTime.Today;
        private Guid? PreviewBranchId;
        private string PreviewValue = "Preview will appear here...";

        // Lookups
        private List<CompanyModel> Companies = new();
        private List<BranchModel> Branches = new();

        // Services
        [Inject] private DocumentNumberSeriesService Service { get; set; }
        [Inject] private NavigationManager Nav { get; set; }

        protected override async Task OnInitializedAsync()
        {
         

            Companies = Service.GetCompanies();
            Branches = Service.GetBranches();

            if (IsEdit)
            {
                await LoadSeries();
            }
            else
            {
                SeedDefaults();
            }
        }

        private async Task LoadSeries()
        {
            try
            {
                var existing = Service.GetById(Id!.Value);
                if (existing != null)
                {

                    series = new DocumentNumberSeriesModel
                    {
                        DocumentNumberSeriesId = existing.DocumentNumberSeriesId,
                        TenantId = existing.TenantId,
                        CompanyId = existing.CompanyId,

                        SeriesCode = existing.SeriesCode,
                        SeriesName = existing.SeriesName,
                        Description = existing.Description,

                        AppliesToEntityType = existing.AppliesToEntityType,
                        SequenceTokenFormat = existing.SequenceTokenFormat,
                        PrefixTemplate = existing.PrefixTemplate,
                        Separator = existing.Separator,
                        ResetFrequency = existing.ResetFrequency,
                        AllowNumberPreview = existing.AllowNumberPreview,
                        ReservationMode = existing.ReservationMode,
                        GapHandlingPolicy = existing.GapHandlingPolicy,
                        MaxSequenceValue = existing.MaxSequenceValue,
                        MinSequenceValue =existing.MinSequenceValue,
                        SuffixTemplate = existing.SuffixTemplate,
                        IncrementBy =existing.IncrementBy,
                        NumericWidth =existing .NumericWidth,
                        IsActive = existing.IsActive,
                        IsSystemDefined = existing.IsSystemDefined,
                        IsLocked = existing.IsLocked,
                        EffectiveFrom = existing.EffectiveFrom,
                        EffectiveTo = existing.EffectiveTo,

                        CreatedAt = existing.CreatedAt,
                        CreatedBy = existing.CreatedBy,
                        UpdatedAt = existing.UpdatedAt,
                        UpdatedBy = existing.UpdatedBy,
                    };

                    _editContext = new EditContext(series);
                    IsInitializing = false;
                }
            }
            catch (Exception ex)
            {
                ToastService.ShowError($"Failed to load series: {ex.Message}");
            }
        }

        private void SeedDefaults()
        {
            _editContext = new EditContext(series);
            series.IsActive = true;
            series.MinSequenceValue = 1;
            series.IncrementBy = 1;
            series.AppliesToEntityType = AppliesToEntityType.FinancialTransaction;
            series.SequenceScopeMode = SequenceScopeMode.CompanyWide;
            series.ResetFrequency = ResetFrequency.Yearly;
            series.FiscalYearMode = FiscalYearMode.CompanyFiscalYear;
            series.EffectiveFrom = DateTime.Today;
            IsInitializing = false;
        }

        string SeriesCodeInput
        {
            get => series.SeriesCode;
            set
            {
                series.SeriesCode = value?.Trim().ToUpperInvariant() ?? "";
            }
        }

        void OnNameChanged()
        {
            series.SeriesName = series.SeriesName?.Trim() ?? "";
        }

        private async Task HandleSubmit()
        {

            // Collect Quill editor values before validation
            if (_descriptionEditor != null)
                series.Description = await _descriptionEditor.GetHtmlAsync();

            if (_editContext.Validate())
            {
                await SaveSeries();
                return;
            }

            if (HasIdentityErrors())
                OpenAccordion("identity");
            else if (HasScopeErrors())
                OpenAccordion("scope");
            else if (HasFormatErrors())
                OpenAccordion("format");
            else if (HasRangeErrors())
                OpenAccordion("range");
            else if (HasAssignmentErrors())
                OpenAccordion("assignment");

            await InvokeAsync(StateHasChanged);
        }


        private void OpenAccordion(string section)
        {
            ShowIdentity = false;
            ShowScope = false;
            ShowFormat = false;
            ShowRange = false;
            ShowAssignment = false;
            ShowStatus = false;

            switch (section)
            {
                case "identity": ShowIdentity = true; break;
                case "scope": ShowScope = true; break;
                case "format": ShowFormat = true; break;
                case "range": ShowRange = true; break;
                case "assignment": ShowAssignment = true; break;
                case "status": ShowStatus = true; break;
            }
        }


        private async Task SaveSeries()
        {
            try
            {
                if(series.MinSequenceValue !> series.MaxSequenceValue)
                {
                    ToastService.ShowError("Min Sequence Value not graeter than Max Sequence Value");
                    return;
                }
                if (series.EffectiveFrom.HasValue && series.EffectiveTo.HasValue)
                {
                    if (series.EffectiveFrom > series.EffectiveTo)
                    {
                        ToastService.ShowError("From Date cannot be greater than To Date.");
                        return;
                    }

                    if (series.EffectiveTo < series.EffectiveFrom)
                    {
                        ToastService.ShowError("To Date cannot be less than From Date.");
                        return;
                    }
                }
                if (IsEdit)
                {
                    Service.UpdateAsync(series);
                    ToastService.ShowSuccess("Series updated successfully.");
                }
                else
                {
                    Service.createAsync(series);
                    ToastService.ShowSuccess("Series created successfully.");
                }

                Nav.NavigateTo("/document-series");
            }
            catch (Exception ex)
            {
                ToastService.ShowError(ex.Message);
            }
        }


        private bool HasIdentityErrors()
        {
            return HasErrorFor(
                nameof(series.SeriesCode),
                nameof(series.SeriesName),
                nameof(series.CompanyId),
                nameof(series.AppliesToEntityType)
            );
        }

        private bool HasScopeErrors()
        {
            return HasErrorFor(
                nameof(series.SequenceScopeMode),
                nameof(series.ResetFrequency),
                nameof(series.FiscalYearMode)
            );
        }

        private bool HasFormatErrors()
        {
            return HasErrorFor(
                nameof(series.SequenceTokenFormat),
                nameof(series.PrefixTemplate),
                nameof(series.SuffixTemplate)
            );
        }

        private bool HasRangeErrors()
        {
            return HasErrorFor(
                nameof(series.MinSequenceValue),
                nameof(series.MaxSequenceValue),
                nameof(series.IncrementBy)
            );
        }

        private bool HasAssignmentErrors()
        {
            return HasErrorFor(
                nameof(series.AllowNumberPreview),
                nameof(series.ReservationMode),
                nameof(series.GapHandlingPolicy)
            );
        }

        private bool HasErrorFor(params string[] fields)
        {
            foreach (var field in fields)
            {
                var messages = _editContext.GetValidationMessages(
                    new FieldIdentifier(series, field));

                if (messages.Any())
                    return true;
            }

            return false;
        }

        //private async Task GeneratePreview()
        //{
        //    try
        //    {
        //        var result = await Service.GeneratePreviewAsync(new DocumentNumberPreviewRequest
        //        {
        //            SeriesId = series.DocumentNumberSeriesId,
        //            PrefixTemplate = series.PrefixTemplate,
        //            SequenceTokenFormat = series.SequenceTokenFormat,
        //            SuffixTemplate = series.SuffixTemplate,
        //            ResetFrequency = series.ResetFrequency,
        //            FiscalYearMode = series.FiscalYearMode,
        //            PreviewDate = PreviewDate,
        //            BranchId = PreviewBranchId,
        //            CompanyId = series.CompanyId
        //        });

        //        PreviewValue = result.PreviewValue;
        //    }
        //    catch (Exception ex)
        //    {
        //        PreviewValue = "Preview failed";
        //        ToastService.ShowError(ex.Message);
        //    }
        //}


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



        private void BackToList()
        {
             IdentityTouched = false;
             ScopeTouched = false;
             FormatTouched = false;
             RangeTouched = false;
             PreviewTouched = false;
             StatusTouched = false;
             AssignmentTouched = false;
            Nav.NavigateTo("/document-series");
        }

        private void ToggleAccordion(string section)
        {
            switch (section)
            {
                case "identity": ShowIdentity = !ShowIdentity; break;
                case "scope": ShowScope = !ShowScope; break;
                case "format": ShowFormat = !ShowFormat; break;
                case "range": ShowRange = !ShowRange; break;
                case "preview": ShowPreview = !ShowPreview; break;
                case "status": ShowStatus = !ShowStatus; break;
                case "assignment": ShowAssignment = !ShowAssignment; break;
            }
        }
    }

}

