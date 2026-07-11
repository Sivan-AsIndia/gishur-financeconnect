using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Linq.Expressions;

namespace FinanceConnect.Client.Pages.Bank_Cash.BankReconciliation
{
    public partial class BankReconciliationForm
    {
        [Parameter] public Guid? ReconId { get; set; }

        BankReconciliationModel Recon = new();
        EditContext _editContext = default!;

        bool IsEdit => ReconId.HasValue;
        bool IsReadOnly => Recon.ReconciliationStatus == ReconciliationStatus.Finalized;
        bool formSubmitted = false;

        List<BankAccountModel> BankAccounts = new();
        List<BranchModel> Branches = new();
        List<BankStatementModel> Statements = new();
        Guid selectedBankAccount;
        bool MatchUTR;
        bool MatchCheque;
        bool MatchNarration;

        protected override void OnInitialized()
        {
            BankAccounts = BankAccountService.GetAll();
            Branches = BranchService.GetAll();

            if (IsEdit)
            {
                Recon = ReconService.GetById(ReconId!.Value)
                    ?? throw new Exception("Reconciliation not found");

                LoadStatements();
            }
            else
            {
                Recon = ReconService.GenerateReconNumber();
            }

            _editContext = new EditContext(Recon);
        }

        Guid SelectedBankAccount
        {
            get => selectedBankAccount;
            set
            {
                selectedBankAccount = value;
                Recon.BankAccountId = selectedBankAccount;
            }
        }

        void OnBankAccountChanged(ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var id) && id != Guid.Empty)
            {
                selectedBankAccount = id;
                Recon.BankAccountId = id;
                LoadStatements();
            }
            else
            {
                selectedBankAccount = Guid.Empty;
                Recon.BankAccountId = Guid.Empty;
                Statements = new();
            }
        }

        void LoadStatements()
        {
            if (Recon.BankAccountId == Guid.Empty)
                return;

            Statements = StatementService
                .GetAll(Recon.TenantId)
                //.Where(s =>
                //    s.BankAccountId == Recon.BankAccountId &&
                //    !s.IsDeleted)
                .ToList();
        }

        bool ShowFieldError(string field)
        {
            if (!formSubmitted)
                return false;

            return field switch
            {
                "BankAccountId" => Recon.BankAccountId == Guid.Empty,
                "RunType" => Recon.RunType == null,
                "ScopeType" => Recon.ScopeType == null,
                "FromDate" => Recon.ScopeType == ScopeType.PeriodRange && Recon.FromDate == null,
                "ToDate" => Recon.ScopeType == ScopeType.PeriodRange && Recon.ToDate == null,
                "AsOfDate" => Recon.ScopeType == ScopeType.AsOfDate && Recon.AsOfDate == null,
                "StatementSelectionMode" => Recon.StatementSelectionMode == null,
                "PrimaryBankStatementId" => Recon.StatementSelectionMode == StatementSelectionModeType.ByStatementFile
                    && (Recon.PrimaryBankStatementId == null || Recon.PrimaryBankStatementId == Guid.Empty),
                "FromToDateRange" => Recon.ScopeType == ScopeType.PeriodRange
                    && Recon.FromDate != null && Recon.ToDate != null
                    && Recon.FromDate > Recon.ToDate,
                _ => false
            };
        }


        void SaveDraft()
        {
            formSubmitted = true;

            try
            {
                // Validate all required fields - show inline errors
                bool hasErrors = false;

                if (Recon.BankAccountId == Guid.Empty) hasErrors = true;
                if (Recon.RunType == null) hasErrors = true;
                if (Recon.ScopeType == null) hasErrors = true;
                if (Recon.StatementSelectionMode == null) hasErrors = true;

                if (Recon.ScopeType == ScopeType.AsOfDate && Recon.AsOfDate == null)
                    hasErrors = true;

                if (Recon.ScopeType == ScopeType.PeriodRange)
                {
                    if (Recon.FromDate == null) hasErrors = true;
                    if (Recon.ToDate == null) hasErrors = true;
                    if (Recon.FromDate != null && Recon.ToDate != null && Recon.FromDate > Recon.ToDate)
                        hasErrors = true;
                }

                if (Recon.StatementSelectionMode == StatementSelectionModeType.ByStatementFile
                    && (Recon.PrimaryBankStatementId == null || Recon.PrimaryBankStatementId == Guid.Empty))
                    hasErrors = true;

                if (hasErrors)
                {
                    StateHasChanged();
                    return;
                }

                Recon.ReferenceMatchMode = BuildReferenceMode();
          
                if (IsEdit)
                {
                    ReconService.Update(Recon);
                    Toast.ShowSuccess("Reconciliation run updated");
                }
                else
                {
                    ReconService.Update(Recon);
                    Toast.ShowSuccess("Reconciliation run created");
                }

                Nav.NavigateTo("/bank-reconciliations");
            }
            catch (Exception ex)
            {
                Toast.ShowError(ex.Message);
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



        ReferenceMatchMode BuildReferenceMode()
        {
            ReferenceMatchMode mode = ReferenceMatchMode.None;

            if (MatchUTR) mode |= ReferenceMatchMode.MatchUTR;
            if (MatchCheque) mode |= ReferenceMatchMode.MatchChequeNo;
            if (MatchNarration) mode |= ReferenceMatchMode.MatchNarrationContains;

            return mode;
        }

        void Cancel()
        {
            ReconService.RemoveData(Recon);
            Nav.NavigateTo("/bank-reconciliations");
        }
    }
}
