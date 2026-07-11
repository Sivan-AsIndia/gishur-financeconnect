using FinanceConnect.Client.Services;
using FinanceConnect.Client.Shared;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Linq.Expressions;
using static FinanceConnect.Client.Data.MasterDataIds;

namespace FinanceConnect.Client.Pages.Bank_Cash.CashTransfer
{
    public partial class CreateCashTransfer
    {

        [Parameter] public Guid? CashTransferId { get; set; }

        private bool IsEditMode => CashTransferId.HasValue;

        private CashTransferModel Model = new();
        private List<BranchModel> Branches = new();
        private List<CashAccountModels> CashAccounts = new();
        private EditContext _editContext;
        private string? SameAccountError;
        private RichTextEditor _narrationEditor;
        private RichTextEditor _acknowledgementEditor;
        string? CompanyValidationError = null;
        public List<BranchModel> FilteredBranches { get; set; } = new();
        public List<CompanyModel> Companies = new();
        public bool IsBranchDisabled => !IsEditMode && (Model.CompanyId == Guid.Empty);

        protected override async Task OnInitializedAsync()
        {
            Branches = BranchService.GetAll();
            Companies = MasterDataService.GetAllCompanies()
                        .Where(c => c.Status == "Active")
                        .ToList();
            CashAccounts = CashAccountService.GetAll();

            if (IsEditMode)
            {
                var existing = await CashTransferService.GetByIdAsync(CashTransferId.Value);
                if (existing != null)
                {
                    Model = existing;
                }
                LoadBranchesForCompany();

            }
            else
            {
                // Auto-generate Transfer Number for new record
                Model.CashTransferNumber = $"CT-{DateTime.Now:yyyyMMddHHmmss}";
                Model.TransferDate = DateTime.Today;
            }
            _editContext = new EditContext(Model);

        }

        public Guid? SelectedCompanyId
        {
            get => Model.CompanyId;
            set
            {
                Model.CompanyId = value ?? Guid.Empty;
                CompanyValidationError = null;
                Model.BranchId = Guid.Empty;
                LoadBranchesForCompany();
            }
        }


        private void LoadBranchesForCompany()
        {
            if (Model.CompanyId != Guid.Empty)
            {
                FilteredBranches = BranchService.GetByCompany(Model.CompanyId);
            }
            else
            {
                FilteredBranches = new();
            }
        }

        async Task HandleSubmit()
        {
            SameAccountError = null;
            if (Model.CompanyId == Guid.Empty)
            {
                CompanyValidationError = "Company is required";
            }
            // Read Quill editor values before validation
            if (_narrationEditor != null)
                Model.Narration = await _narrationEditor.GetHtmlAsync();
            if (_acknowledgementEditor != null)
                Model.ReceiptAcknowledgementNote = await _acknowledgementEditor.GetHtmlAsync();

            // Custom validation: source != destination
            if (Model.SourceCashAccountId.HasValue && Model.DestinationCashAccountId.HasValue
                && Model.SourceCashAccountId == Model.DestinationCashAccountId)
            {
                SameAccountError = "Source and Destination accounts must be different";
            }

            var isValid = _editContext.Validate();

            if (!isValid || SameAccountError != null)
            {
                return;
            }
            await Save();
        }

        async Task Save()
        {
            var branch = Branches.FirstOrDefault(x => x.Id == Model.BranchId);
            if (branch != null)
                Model.BranchName = branch.BranchName;
       
            var sourceAccount = CashAccounts
                .FirstOrDefault(x => x.Id == Model.SourceCashAccountId);

            if (sourceAccount != null)
                Model.SourceCashAccountName = sourceAccount.Name;

            var destAccount = CashAccounts
                .FirstOrDefault(x => x.Id == Model.DestinationCashAccountId);

            if (destAccount != null)
                Model.DestinationCashAccountName = destAccount.Name;

            if (IsEditMode)
            {
                await CashTransferService.UpdateAsync(Model);
                ToastService.ShowSuccess("Cash Transfer updated successfully!");
            }
            else
            {
                await CashTransferService.CreateAsync(Model);
                ToastService.ShowSuccess("Cash Transfer created successfully!");
            }

            NavigationManager.NavigateTo("/cash-transfers");
        }



        private string GetValidationClass(Expression<Func<object>> field)
{
    if (_editContext == null)
        return string.Empty;

    var fieldIdentifier = FieldIdentifier.Create(field);

    var hasError = _editContext.GetValidationMessages(fieldIdentifier).Any();

    if (hasError)
        return "is-invalid";

    return string.Empty;
}

    }
}
