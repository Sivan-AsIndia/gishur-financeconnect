using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace FinanceConnect.Client.Pages.FixedAssets.DepreciationMethod
{
    public partial class CreateDepreciationMethod
    {
        [Inject] private DepreciationMethodService Service { get; set; } = default!;
        [Inject] private ToastService ToastService { get; set; } = default!;
        [Parameter] public Guid? Id { get; set; }

        private EditContext _editContext = default!;
        private ValidationMessageStore _messageStore = default!;

        protected DepreciationMethodViewModel.DepreciationMethod Model { get; set; }
            = new DepreciationMethodViewModel.DepreciationMethod();

        protected bool IsEdit => Id.HasValue;
        protected string RateError = "";
        protected string LockReasonError = "";

        protected override async Task OnInitializedAsync()
        {
            if (IsEdit)
            {
                var data = await Service.GetByIdAsync(Id!.Value);
                if (data != null)
                    Model = data;
            }

            _editContext = new EditContext(Model);
            _messageStore = new ValidationMessageStore(_editContext);
        }

        private bool ValidateCustom()
        {
            RateError = "";
            LockReasonError = "";
            bool valid = true;

            if (Model.InputMode == DepreciationMethodViewModel.InputModeEnum.RateBased &&
                (Model.DefaultRatePercent == null || Model.DefaultRatePercent <= 0 || Model.DefaultRatePercent > 100))
            {
                RateError = "Rate is required for RateBased depreciation (0 < rate ≤ 100).";
                valid = false;
            }

            if (Model.IsLockedForChanges && string.IsNullOrWhiteSpace(Model.LockReason))
            {
                LockReasonError = "Lock reason is required when method is locked.";
                valid = false;
            }

            return valid;
        }

        protected async Task Save()
        {
            var modelValid = _editContext.Validate();
            var customValid = ValidateCustom();

            StateHasChanged();

            if (!modelValid || !customValid) return;

            if (IsEdit)
            {
                await Service.UpdateAsync(Model);
                ToastService.ShowSuccess("Method Updated Successfully", "Success");
            }
            else
            {
                await Service.CreateAsync(Model);
                ToastService.ShowSuccess("Method Created Successfully", "Success");
            }

            Nav.NavigateTo("/depreciation-methods");
        }
    }
}
