using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Bank_Cash.BankStatement
{
    public partial class BankStatementDetails
    {

        [Inject] BankAccountService BankAccountService { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] BranchService BranchService { get; set; } = default!;

        [Parameter]
        public Guid StatementId { get; set; }

        BankStatementModel? SelectedStatement;
        private BankStatementLineModel? SelectedLine;
        List<BankStatementLineModel> StatementLines = new();
        List<BankStatementLineModel> FilteredStatementLines = new();
        List<BankAccountModel> BankAccounts = new();
        List<CompanyModel> Companies = new();
        List<BranchModel> Branches = new();
        int PageSize = 10;
        int CurrentPage = 1;
        int PageWindowSize = 2;

        int TotalPages =>
            FilteredStatementLines.Count == 0
                ? 1
                : (int)Math.Ceiling((double)FilteredStatementLines.Count / PageSize);

        List<BankStatementLineModel> PagedStatements =>
            FilteredStatementLines
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

        IEnumerable<int> VisiblePages
        {
            get
            {
                if (TotalPages <= PageWindowSize)
                    return Enumerable.Range(1, TotalPages);

                int start = Math.Max(1, CurrentPage - (PageWindowSize / 2));
                int end = start + PageWindowSize - 1;

                if (end > TotalPages)
                {
                    end = TotalPages;
                    start = end - PageWindowSize + 1;
                }

                return Enumerable.Range(start, end - start + 1);
            }
        }


        private async Task PrintPage()
        {
            await JS.InvokeVoidAsync("window.print");
        }
        protected override void OnInitialized()
        {
            SelectedStatement = StatementService.GetById(StatementId);



            if (SelectedStatement != null)
            {

                BankAccounts = BankAccountService.GetAll();
                Companies = MasterDataService.GetAllCompanies();
                Branches = BranchService.GetAll();

                var account = BankAccounts
                    .FirstOrDefault(acc => acc.Id == SelectedStatement.BankAccountId);

                SelectedStatement.BankAccountName = account?.BankAccountName ?? "—";

                var branch = Branches
                    .FirstOrDefault(b => b.Id == SelectedStatement.BranchId);

                SelectedStatement.BranchName = branch?.BranchName ?? "—";

                var company = Companies
                    .FirstOrDefault(c => c.Id == SelectedStatement.CompanyId);

                SelectedStatement.CompanyName = company?.LegalName ?? "—";
                StatementLines = StatementService
                    .GetLines(SelectedStatement.BankStatementId)
                    .Take(100) // Preview only
                    .ToList();
                FilteredStatementLines = StatementLines;
                CurrentPage = 1;
            }
        }

        void GoBack()
        {
            Nav.NavigateTo("/bank-statements");
        }

        void OnPageSizeChange(ChangeEventArgs e)
        {
            PageSize = int.Parse(e.Value!.ToString()!);
            CurrentPage = 1;
        }

        void PreviousPage()
        {
            if (CurrentPage > 1)
                CurrentPage--;
        }

        void NextPage()
        {
            if (CurrentPage < TotalPages)
                CurrentPage++;
        }

        void GoToPage(int page)
        {
            if (page < 1 || page > TotalPages)
                return;

            CurrentPage = page;
        }
        void LockStatement()
        {
            if (SelectedStatement == null) return;

            StatementService.Lock(SelectedStatement.BankStatementId);
            //Nav.NavigateTo(Nav.Uri, forceLoad: true);
        }

        void SupersedeStatement()
        {
            if (SelectedStatement == null) return;

            StatementService.Supersede(
                SelectedStatement.BankStatementId,
                "Corrected bank file uploaded");

            Nav.NavigateTo("/bank-statements");
        }

        void DownloadFile()
        {
            if (SelectedStatement == null) return;

            Nav.NavigateTo(
                $"/api/finance/cashbank/statements/{SelectedStatement.BankStatementId}/download",
                true);
        }


        void SelectLine(BankStatementLineModel line)
        {
            SelectedLine = line;
        }

        string GetReconBadge(ReconciliationStatusType status) => status switch
        {
            ReconciliationStatusType.Unmatched => "bg-secondary-transparent text-secondary",
            ReconciliationStatusType.Suggested => "bg-info-transparent text-info",
            ReconciliationStatusType.Matched => "bg-success-transparent text-success",
            ReconciliationStatusType.FinalizedLocked => "bg-dark-transparent text-dark",
            ReconciliationStatusType.Excluded => "bg-danger-transparent text-danger",
            _ => "bg-secondary-transparent text-secondary"
        };

        string GetDirectionBadge(StatementLineDirectionType dir) =>
            dir == StatementLineDirectionType.Debit
                ? "bg-soft-danger text-danger"
                : "bg-soft-success text-success";
        string GetStatusBadge(StatementStatusType status)
        {
            return status switch
            {
                StatementStatusType.Uploaded => "bg-secondary-transparent text-secondary",
                StatementStatusType.ParsingInProgress => "bg-info-transparent text-info",
                StatementStatusType.Parsed => "bg-primary-transparent text-primary",
                StatementStatusType.ValidationFailed => "bg-danger-transparent text-danger",
                StatementStatusType.ReadyForReconciliation => "bg-success-transparent text-success",
                StatementStatusType.Locked => "bg-dark-transparent text-dark",
                StatementStatusType.Archived => "bg-light text-dark",
                StatementStatusType.Superseded => "bg-warning-transparent text-warning",
                _ => "bg-secondary-transparent text-secondary"
            };
        }
    }
}
