using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using FinanceConnect.Client.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Linq.Expressions;

namespace FinanceConnect.Client.Pages.Bank_Cash.cheque
{
    public partial class ChequeForm
    {
        private List<BankAccountModel> BankAccounts = new List<BankAccountModel>();

        [Parameter] public Guid? Id { get; set; }
        [Inject] IJSRuntime JS { get; set; } = default!;
        public EditContext _editcontext;

        ChequeModel model = new();

        List<BranchModel> Branches = new();
        bool isEdit => Id != null;
        bool isInitialized = false;

        bool isLocked =>
            model.Status == ChequeStatus.Printed ||
            model.Status == ChequeStatus.Issued ||
            model.Status == ChequeStatus.Deposited ||
            model.Status == ChequeStatus.Cleared;

        // Quill Editor ref
        private RichTextEditor? _narrationEditor;

        // Accordion visibility state
        bool ShowCore = true;
        bool ShowInstrument = false;
        bool ShowCounterparty = false;
        bool ShowLinkage = false;
        bool ShowLifecycle = false;

        // Touched state for accordion sections
        bool CoreTouched = false;
        bool InstrumentTouched = false;
        bool CounterpartyTouched = false;
        bool LinkageTouched = false;

        string PageTitle => isEdit ? "Edit Cheque" : "Create Cheque";
        string PageSubTitle => isEdit ? "Update existing cheque details" : "Create a new cheque entry";

        void GoBackToList() => Nav.NavigateTo("/cheques");

        void ToggleAccordion(string section)
        {
            bool isCurrentlyOpen = section switch
            {
                "core" => ShowCore,
                "instrument" => ShowInstrument,
                "counterparty" => ShowCounterparty,
                "linkage" => ShowLinkage,
                "lifecycle" => ShowLifecycle,
                _ => false
            };

            ShowCore = false;
            ShowInstrument = false;
            ShowCounterparty = false;
            ShowLinkage = false;
            ShowLifecycle = false;

            if (!isCurrentlyOpen)
            {
                switch (section)
                {
                    case "core": ShowCore = true; break;
                    case "instrument": ShowInstrument = true; break;
                    case "counterparty": ShowCounterparty = true; break;
                    case "linkage": ShowLinkage = true; break;
                    case "lifecycle": ShowLifecycle = true; break;
                }
            }
        }

        void OpenAccordion(string section)
        {
            ShowCore = false;
            ShowInstrument = false;
            ShowCounterparty = false;
            ShowLinkage = false;
            ShowLifecycle = false;

            switch (section)
            {
                case "core": ShowCore = true; break;
                case "instrument": ShowInstrument = true; break;
                case "counterparty": ShowCounterparty = true; break;
                case "linkage": ShowLinkage = true; break;
                case "lifecycle": ShowLifecycle = true; break;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("feather.replace");
            }
        }

        protected override void OnInitialized()
        {
            Branches = BranchService.GetAll();
            BankAccounts = backAccount.GetAll();

            _editcontext = new EditContext(model);

            if (Id != null)
            {
                var c = Service.GetById(Id.Value);
                if (c != null)
                    model = c;
                _editcontext = new EditContext(model);
            }

            isInitialized = true;
        }

        private void OnBranchChanged(ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var branchId) && branchId != Guid.Empty)
            {
                model.BranchId = branchId;
                var branch = Branches.FirstOrDefault(b => b.Id == branchId);
                if (branch != null)
                    model.Branch = $"{branch.BranchCode} - {branch.BranchName}";
            }
            else
            {
                model.BranchId = Guid.Empty;
                model.Branch = "";
            }
        }

        private string GetValidationClass(Expression<Func<object>> field)
        {
            if (_editcontext == null)
                return string.Empty;

            var fieldIdentifier = FieldIdentifier.Create(field);
            var hasError = _editcontext.GetValidationMessages(fieldIdentifier).Any();

            if (hasError)
                return "is-invalid";

            return string.Empty;
        }

        private bool HasBranchError()
        {
            if (_editcontext == null) return false;
            var fieldIdentifier = FieldIdentifier.Create(() => model.BranchId);
            return _editcontext.GetValidationMessages(fieldIdentifier).Any();
        }

        private string GetStatusBadge(ChequeStatus status)
        {
            return status switch
            {
                ChequeStatus.Draft => "bg-warning-transparent text-warning",
                ChequeStatus.Prepared => "bg-info-transparent text-info",
                ChequeStatus.Printed => "bg-info-transparent text-info",
                ChequeStatus.Issued => "bg-primary-transparent text-primary",
                ChequeStatus.Received => "bg-success-transparent text-success",
                ChequeStatus.Deposited => "bg-success-transparent text-success",
                ChequeStatus.Presented => "bg-info-transparent text-info",
                ChequeStatus.Cleared => "bg-success-transparent text-success",
                ChequeStatus.Bounced => "bg-danger-transparent text-danger",
                ChequeStatus.Stopped => "bg-secondary-transparent text-secondary",
                ChequeStatus.Cancelled => "bg-dark-transparent text-dark",
                ChequeStatus.Stale => "bg-secondary-transparent text-secondary",
                ChequeStatus.Reissued => "bg-primary-transparent text-primary",
                _ => "bg-secondary-transparent text-secondary"
            };
        }

        private string GetStatusIcon(ChequeStatus status)
        {
            return status switch
            {
                ChequeStatus.Draft => "ti ti-pencil",
                ChequeStatus.Prepared => "ti ti-clipboard-check",
                ChequeStatus.Printed => "ti ti-printer",
                ChequeStatus.Issued => "ti ti-send",
                ChequeStatus.Received => "ti ti-inbox",
                ChequeStatus.Deposited => "ti ti-building-bank",
                ChequeStatus.Presented => "ti ti-arrow-right",
                ChequeStatus.Cleared => "ti ti-check",
                ChequeStatus.Bounced => "ti ti-alert-triangle",
                ChequeStatus.Stopped => "ti ti-player-stop",
                ChequeStatus.Cancelled => "ti ti-x",
                ChequeStatus.Stale => "ti ti-clock",
                ChequeStatus.Reissued => "ti ti-refresh",
                _ => "ti ti-circle"
            };
        }

        async void Save()
        {
            // Get Quill content
            if (_narrationEditor != null)
            {
                model.Narration = await _narrationEditor.GetHtmlAsync();
            }

            var isValid = _editcontext.Validate();

            if (!isValid)
            {
                if (model.BranchId == Guid.Empty || string.IsNullOrWhiteSpace(model.ChequeNumber))
                    OpenAccordion("core");
                else if (model.Amount <= 0)
                    OpenAccordion("instrument");
                else if (string.IsNullOrWhiteSpace(model.CounterpartyType))
                    OpenAccordion("counterparty");
                else
                    OpenAccordion("core");
                return;
            }

            var selectedAccount = BankAccounts
                .FirstOrDefault(x => x.Id == model.OurBankAccountId);
            if (selectedAccount != null)
                model.OurBankAccount = selectedAccount.BankAccountName;

            var selectedBranch = Branches.FirstOrDefault(b => b.Id == model.BranchId);
            if (selectedBranch != null)
                model.Branch = $"{selectedBranch.BranchCode} - {selectedBranch.BranchName}";

            if (!isEdit)
            {
                model.CreatedAt = DateTime.Now;
                Service.Add(model);
            }
            else
            {
                model.UpdatedAt = DateTime.Now;
                Service.Update(model);
            }

            Nav.NavigateTo("/cheques");
        }
    }
}
