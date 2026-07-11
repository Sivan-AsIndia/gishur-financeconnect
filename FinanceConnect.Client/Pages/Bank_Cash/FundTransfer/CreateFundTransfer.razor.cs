using FinanceConnect.Client.Services;
using FinanceConnect.Client.Shared;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Bank_Cash.FundTransfer
{
    public partial class CreateFundTransfer
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] FundTransferService FundTransferService { get; set; } = default!;
        [Inject] BankAccountService BankAccountService { get; set; } = default!;

        private List<BranchModel> Branches = new();
        private List<BranchModel> FilteredBranches = new();
        private List<CompanyModel> company = new();
        private List<BankAccountModel> BankAccounts = new();

        public bool IsBranchDisabled => !Fund.CompanyId.HasValue || Fund.CompanyId == Guid.Empty;

        private RichTextEditor _narrationEditor;
        private RichTextEditor _approvalNotesEditor;
        private RichTextEditor _failureReasonEditor;
        private RichTextEditor _chargeNarrationEditor;

        private string? SameAccountError;
        private bool validationAttempted;

        [Parameter] public Guid? Id { get; set; }

        private bool isInitialized;
        private FundTransferModel Fund = new();
        private EditContext? editContext;

        private bool IsEdit => Id.HasValue;

        private string PageTitle =>
            IsEdit ? "Edit Fund Transfer" : "Create Fund Transfer";

        private string PageSubTitle =>
            IsEdit ? "Update fund transfer details" : "Create new fund transfer";

        private void OnCompanyChanged(ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var companyId) && companyId != Guid.Empty)
            {
                Fund.CompanyId = companyId;
                Fund.BranchId = null;
                FilteredBranches = BranchService.GetByCompany(companyId);
            }
            else
            {
                Fund.CompanyId = null;
                Fund.BranchId = null;
                FilteredBranches = new();
            }
        }

        private void OnBranchSelected(ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var branchId) && branchId != Guid.Empty)
            {
                Fund.BranchId = branchId;
            }
            else
            {
                Fund.BranchId = null;
            }
        }

        protected override async Task OnInitializedAsync()
        {
            Branches = BranchService.GetAll();
            company = MasterDataService.GetAllCompanies();
            BankAccounts = await BankAccountService.GetActiveAccountsAsync();

            if (Id.HasValue)
            {
                Fund = FundTransferService.GetById(Id.Value)
                    ?? new FundTransferModel();

                if (Fund.CompanyId.HasValue && Fund.CompanyId != Guid.Empty)
                {
                    FilteredBranches = BranchService.GetByCompany(Fund.CompanyId.Value);
                }
            }
            else
            {
                Fund.FundTransferNumber = GenerateTransferNumber();
            }

            editContext = new EditContext(Fund);
            isInitialized = true;
        }

        private string GenerateTransferNumber()
        {
            return $"FT-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(10000, 99999)}";
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
        }

        private static bool IsQuillEmpty(string? html)
        {
            if (string.IsNullOrWhiteSpace(html)) return true;
            var stripped = html.Replace("<p>", "").Replace("</p>", "")
                              .Replace("<br>", "").Replace("<br/>", "")
                              .Replace("&nbsp;", "").Trim();
            return string.IsNullOrWhiteSpace(stripped);
        }

        private async Task ReadAllEditors()
        {
            if (_narrationEditor != null)
                Fund.Narration = await _narrationEditor.GetHtmlAsync();
            if (_approvalNotesEditor != null)
                Fund.ApprovalNotes = await _approvalNotesEditor.GetHtmlAsync();
            if (_failureReasonEditor != null)
                Fund.FailureReason = await _failureReasonEditor.GetHtmlAsync();
            if (_chargeNarrationEditor != null)
                Fund.ChargeNarration = await _chargeNarrationEditor.GetHtmlAsync();
        }

        private bool ValidateAll()
        {
            SameAccountError = null;

            bool isValid = true;

            if (!Fund.CompanyId.HasValue) isValid = false;
            if (!Fund.BranchId.HasValue) isValid = false;
            if (string.IsNullOrWhiteSpace(Fund.SourceBankAccount)) isValid = false;
            if (string.IsNullOrWhiteSpace(Fund.DestinationBankAccount)) isValid = false;
            if (Fund.Amount <= 0) isValid = false;
            if (Fund.TransferDate == default) isValid = false;
            if (Fund.SourceValueDate == default) isValid = false;
            if (Fund.DestinationValueDate == default) isValid = false;
            if (IsQuillEmpty(Fund.Narration)) isValid = false;
            if (Fund.TransferMethod == null) isValid = false;
            if (string.IsNullOrWhiteSpace(Fund.UTRNumber)) isValid = false;
            if (Fund.ChargeHandlingMode == null) isValid = false;
            if (Fund.ChargeBearerType == null) isValid = false;

            // Same account check
            if (!string.IsNullOrWhiteSpace(Fund.SourceBankAccount)
                && !string.IsNullOrWhiteSpace(Fund.DestinationBankAccount)
                && Fund.SourceBankAccount == Fund.DestinationBankAccount)
            {
                SameAccountError = "Source and Destination accounts must be different";
                isValid = false;
            }

            // Date order checks
            if (Fund.TransferDate != default && Fund.SourceValueDate != default
                && Fund.TransferDate > Fund.SourceValueDate)
            {
                ToastService.ShowError("Source Value Date should not be earlier than Transfer Date");
                isValid = false;
            }

            if (Fund.SourceValueDate != default && Fund.DestinationValueDate != default
                && Fund.SourceValueDate > Fund.DestinationValueDate)
            {
                ToastService.ShowError("Destination Value Date should not be earlier than Source Value Date");
                isValid = false;
            }

            return isValid;
        }

        private async Task Save()
        {
            await ReadAllEditors();

            validationAttempted = true;

            if (!ValidateAll())
            {
                StateHasChanged();
                return;
            }

            if (IsEdit)
            {
                FundTransferService.UpdateDraft(Fund);
                ToastService.ShowSuccess("Fund transfer updated successfully", "Updated");
            }
            else
            {
                Fund.FundTransferId = Guid.NewGuid();
                FundTransferService.Create(Fund);
                ToastService.ShowSuccess("Fund transfer created successfully", "Added");
            }

            Nav.NavigateTo("/fund-transfers");
        }
    }
}
