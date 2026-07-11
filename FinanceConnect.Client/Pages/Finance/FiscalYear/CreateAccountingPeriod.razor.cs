using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Linq.Expressions;

namespace FinanceConnect.Client.Pages.Finance.FiscalYear
{
    public partial class CreateAccountingPeriod
    {
        [Parameter] public Guid? Id { get; set; }
        [Parameter] public Guid FiscalYearId { get; set; }

        private EditContext _editContext;

        AccountingPeriodModel period = new();
        FiscalYearModel fy = new();

        bool IsEdit => Id.HasValue;
        bool IsLocked => period.Status != AccountingPeriodStatus.Draft;
        bool IsCodeLocked => IsEdit && IsLocked;

        bool IsLockReadOnly =>
            period.Status != AccountingPeriodStatus.Open;

        string AccPeriodCodeInput
        {
            get => period.PeriodCode;
            set
            {
                period.PeriodCode = value?.Trim().ToUpperInvariant() ?? "";
            }
        }

        private void OnNameChange()
        {
            period.PeriodName = period.PeriodName?.Trim() ?? "";
        }

        protected override void OnInitialized()
        {
            if (IsEdit)
            {
                period = Service.GetById(Id!.Value)!;
                fy = FiscalYearService.GetById(period.FiscalYearId)!;
            }
            else
            {
                fy = FiscalYearService.GetById(FiscalYearId)!;

                period.FiscalYearId = fy.Id;
                period.CompanyId = fy.CompanyId ?? Guid.Empty;
                period.Status = AccountingPeriodStatus.Draft;
                period.PeriodType = AccountingPeriodType.Normal;
            }
            _editContext = new EditContext(period);
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

        void Save()
        {
            try
            {
                if (IsEdit)
                {
                    Service.Update(period);
                }
                else
                {
                    Service.CreateManual(period,fy);
                    ToastService.ShowSuccess($"{period.PeriodName} created successfully");
                }

                GoBack();
            }
            catch (Exception e)
            {
                ToastService.ShowError(e.Message);
            }

        }

        void OpenPeriod() =>
            Service.OpenPeriod(period.Id);

        void SoftClosePeriod() =>
            Service.SoftClosePeriod(period.Id);

        void ClosePeriod() =>
            Service.ClosePeriod(period.Id);

        void GoBack()
        {
            Nav.NavigateTo($"/fiscalyears/{period.FiscalYearId}/periods");
        }
    }
}
