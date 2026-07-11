using FinanceConnect.Client.Pages.Master.Company;
using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Linq.Expressions;
using static FinanceConnect.Client.Data.MasterDataIds;

namespace FinanceConnect.Client.Pages.Finance.FiscalYear
{
    public partial class FiscalYearForm
    {
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
        }
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] AccountingPeriodService AccPeriodService { get; set; } = default!;
        [Parameter] public Guid? Id { get; set; }

        FiscalYearModel fy = new();
        List<CompanyModel> Companies = new();
        bool IsEdit => Id.HasValue;
        bool IsLocked => fy.Status != FiscalYearStatus.Draft;
        bool IsCodeLocked => IsEdit && fy.Status != FiscalYearStatus.Draft;

        string? FiscalDateWarning;
        private EditContext _editContext;
        bool ShowIdentity = true;
        bool ShowDates, ShowPeriods, ShowClosing;
        string FiscalYearCodeProxy
        {
            get => fy.FiscalYearCode;
            set => fy.FiscalYearCode = value?.Trim().ToUpperInvariant() ?? "";
        }

        private FiscalPeriodType SelectedPeriodType
        {
            get => fy.PeriodType;
            set
            {
                if (fy.PeriodType != value)
                {
                    fy.PeriodType = value;
                    fy.NumberOfPeriods = AllowedPeriods.First().Value;
                }
            }
        }
        private IEnumerable<PeriodOption> AllowedPeriods =>
            fy.PeriodType switch
            {
                FiscalPeriodType.Monthly => new[]
                {
            new PeriodOption { Value = 12, Label = "12" },
            new PeriodOption { Value = 13, Label = "13 (Adjustment)" }
                },

                FiscalPeriodType.Quarterly => new[]
                {
            new PeriodOption { Value = 3, Label = "3" }
                },

                FiscalPeriodType.FourFourFive => new[]
                {
            new PeriodOption { Value = 12, Label = "12" }
                },

                _ => new[]
                {
            new PeriodOption { Value = 12, Label = "12" }
                }
            };


        protected override void OnInitialized()
        {
            Companies = MasterDataService.GetAllCompanies().Where(c => c.Status == "Active")
            .ToList();
            if (IsEdit)
            {
                var existing = Service.GetById(Id!.Value);
                if (existing != null)
                {
                    fy = new FiscalYearModel
                    {
                        Id = existing.Id,

                        // Identity
                        FiscalYearCode = existing.FiscalYearCode,
                        FiscalYearName = existing.FiscalYearName,
                        CompanyId = existing.CompanyId,
                        CompanyName = existing.CompanyName,
                        Status = existing.Status,

                        // Date Range
                        StartDate = existing.StartDate,
                        EndDate = existing.EndDate,
                        BooksStartDateSnapshot = existing.BooksStartDateSnapshot,

                        // Period Generation
                        PeriodType = existing.PeriodType,
                        NumberOfPeriods = existing.NumberOfPeriods,
                        PeriodNamingConvention = existing.PeriodNamingConvention,
                        AutoGeneratePeriods = existing.AutoGeneratePeriods,
                        AutoOpenFirstPeriod = existing.AutoOpenFirstPeriod,

                        // Closing Controls
                        AllowAdjustmentPostingAfterSoftClose = existing.AllowAdjustmentPostingAfterSoftClose,
                        RequirePeriodCloseChecklist = existing.RequirePeriodCloseChecklist,
                        CloseChecklistTemplateId = existing.CloseChecklistTemplateId,
                        CloseReason = existing.CloseReason,

                        // System
                        CreatedAt = existing.CreatedAt,
                        UpdatedAt = existing.UpdatedAt,
                        ClosedAt = existing.ClosedAt,
                        ClosedBy = existing.ClosedBy,
                    };
                }
            }
            else
            {
                fy.Status = FiscalYearStatus.Draft;
                fy.NumberOfPeriods = 12;
                fy.AutoGeneratePeriods = true;
                fy.RequirePeriodCloseChecklist = true;
                fy.AllowAdjustmentPostingAfterSoftClose = true;
                fy.PeriodType = FiscalPeriodType.Monthly;
            }
            _editContext = new EditContext(fy);
        }


        private async Task HandleSubmit()
        {
            if (_editContext.Validate())
            {
                Save();
                return;
            }

            if (HasIdentityErrors())
                OpenAccordion("fyIdentity");
            else if (HasDateErrors())
                OpenAccordion("fyDates");
            else if (HasPeriodErrors())
                OpenAccordion("fyPeriods");
            else if (HasClosingErrors())
                OpenAccordion("fyClosing");

            await InvokeAsync(StateHasChanged);
        }

        bool HasIdentityErrors()
        {
            return string.IsNullOrWhiteSpace(fy.FiscalYearCode)
                || string.IsNullOrWhiteSpace(fy.FiscalYearName)
                || fy.CompanyId == Guid.Empty;
        }

        bool HasDateErrors()
        {
            return fy.StartDate == null
                || fy.EndDate == null
                || fy.EndDate < fy.StartDate;
        }

        bool HasPeriodErrors()
        {
            return fy.NumberOfPeriods <= 0
                || !Enum.IsDefined(typeof(FiscalPeriodType), fy.PeriodType);
        }

        bool HasClosingErrors()
        {
            return fy.Status == FiscalYearStatus.SoftClosed
                && string.IsNullOrWhiteSpace(fy.CloseReason);
        }

        void ToggleAccordion(string section)
        {
            switch (section)
            {
                case "fyIdentity":
                    ShowIdentity = !ShowIdentity;
                    break;

                case "fyDates":
                    ShowDates = !ShowDates;
                    break;

                case "fyPeriods":
                    ShowPeriods = !ShowPeriods;
                    break;

                case "fyClosing":
                    ShowClosing = !ShowClosing;
                    break;
            }
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
        void OpenAccordion(string section)
        {
            switch (section)
            {
                case "fyIdentity":
                    ShowIdentity = true;
                    break;

                case "fyDates":
                    ShowDates = true;
                    break;

                case "fyPeriods":
                    ShowPeriods = true;
                    break;

                case "fyClosing":
                    ShowClosing = true;
                    break;
            }
        }


        void Save()
        {
            try
            {
                if (fy.CompanyId == Guid.Empty)
                    return;

                if (!ValidateFiscalDates())
                    return;

                var company = Companies.First(x => x.Id == fy.CompanyId);
                fy.CompanyName = company.LegalName;

                if (IsEdit)
                {
                    Service.Update(fy);
                    ToastService.ShowSuccess($"{fy.FiscalYearCode} updated successfully");
                }
                else
                {
                    Service.Create(fy);
                    ToastService.ShowSuccess($"{fy.FiscalYearCode} added successfully");
                }

                Nav.NavigateTo("/fiscalyears");
            }
            catch (Exception e)
            {
                ToastService.ShowError(e.Message);
            }



        }

        void OnCompanyChange()
        {
            if (Companies.FirstOrDefault(c => c.Id == fy.CompanyId) is { } company)
            {
                fy.BooksStartDateSnapshot = company.BooksStartDate;
            }
        }

        void ChangeStatus(FiscalYearStatus status)
        {
            if (status == FiscalYearStatus.Open)
            {
                var hasOpenFy = Service
                    .GetAllByCompanyId(fy.CompanyId ?? Guid.Empty)
                    .Any(x => x.Status == FiscalYearStatus.Open);

                if (hasOpenFy)
                {
                    ToastService.ShowError("The company already has an open fiscal year.");
                    return;
                }
            }

            Service.ChangeStatus(fy.Id, status);
            if (status == FiscalYearStatus.Open && fy.AutoOpenFirstPeriod)
            {
                var periods = AccPeriodService.GetByFiscalYear(fy.Id);

                var firstPeriod = periods
                    .OrderBy(p => p.PeriodNumber)
                    .FirstOrDefault();

                if (firstPeriod != null && firstPeriod.Status == AccountingPeriodStatus.Draft)
                {
                    AccPeriodService.OpenPeriod(firstPeriod.Id);
                }
            }


            ToastService.ShowSuccess($"{fy.FiscalYearCode} - Status changed successfully");
            Nav.NavigateTo("/fiscalyears");
        }

        void CloseYear()
        {
            if (string.IsNullOrWhiteSpace(fy.CloseReason))
                return; // block close

            Service.ChangeStatus(
                fy.Id,
                FiscalYearStatus.Closed,
                fy.CloseReason
            );

            Nav.NavigateTo("/fiscalyears");
        }


        bool ValidateFiscalDates()
        {
            var company = Companies.FirstOrDefault(c => c.Id == fy.CompanyId);
            if (company == null)
                return false;

            FiscalDateWarning = null;

            if (fy.StartDate == null || fy.EndDate == null)
                return false;

            var start = fy.StartDate.Value;
            var end = fy.EndDate.Value;

            int startMonth = company.FiscalYearStartMonth;

            // Expected end month
            int expectedEndMonth = startMonth == 1 ? 12 : startMonth - 1;

            // Expected end year
            int expectedEndYear = startMonth == 1 ? start.Year : start.Year + 1;

            // Last day of expected end month
            int expectedEndDay = DateTime
                .DaysInMonth(expectedEndYear, expectedEndMonth);

            bool isValidFiscalYear =
                start.Month == startMonth &&
                start.Day == 1 &&
                end.Month == expectedEndMonth &&
                end.Day == expectedEndDay &&
                end.Year == expectedEndYear;

            if (!isValidFiscalYear)
            {
                throw new Exception(
                    $"Fiscal year must start on 1 {new DateTime(2000, startMonth, 1):MMMM} " +
                    $"and end on {expectedEndDay} {new DateTime(2000, expectedEndMonth, 1):MMMM}."
                );
            }

            return true;
        }

        private void OnFYNameInput()
        {
            fy.FiscalYearName = fy.FiscalYearName?.Trim() ?? "";
        }

        private string GetPeriodTypeLabel(FiscalPeriodType type) => type switch
        {
            FiscalPeriodType.Monthly => "Monthly",
            FiscalPeriodType.Quarterly => "Quarterly",
            FiscalPeriodType.FourFourFive => "4-4-5 Calendar",
            _ => type.ToString()
        };


        private string GetStatusBadge(FiscalYearStatus status)
        {
            return status switch
            {
                FiscalYearStatus.Open =>
                    "bg-success-transparent text-success",

                FiscalYearStatus.Closed =>
                    "bg-danger-transparent text-danger",

                FiscalYearStatus.Draft =>
                    "bg-warning-transparent text-warning",

                FiscalYearStatus.SoftClosed =>
                    "bg-warning-transparent text-info",

                _ =>
                    "bg-secondary-transparent text-secondary"
            };
        }
    }
}
