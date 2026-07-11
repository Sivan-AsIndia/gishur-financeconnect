using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;

namespace FinanceConnect.Client.Pages.Bank_Cash.BankReconciliation
{
    public partial class BankReconciliation
    {
        [Parameter] public Guid ReconId { get; set; }

        BankReconciliationModel Recon = new();
        bool IsLoaded;
        bool DrawerOpen;
        int PageSize = 10;
        int CurrentPage = 1;
        int PageWindowSize = 2;
        Guid? SelectedStatementLineId;
        List<BankStatementLineModel> UnmatchedStatementLines = new();
        List<BankTransactionModel> UnmatchedTransactions = new();

        List<BankReconciliationMatchModel> Matches = new();
        //List<ReconAuditLog> AuditLogs = new();
        List<MatchSuggestionView> Suggestions = new();
        List<BankStatementLineModel> FilteredStatementLines = new();
        BankStatementLineModel? SelectedStatementLine;

        bool IsFinalized => Recon.ReconciliationStatus == ReconciliationStatus.Finalized;
        bool CanMatch => SelectedStatementLine != null && !IsFinalized;

        int TotalPages =>
    FilteredStatementLines.Count == 0
        ? 1
        : (int)Math.Ceiling((double)FilteredStatementLines.Count / PageSize);

        List<BankStatementLineModel> PagedLines =>
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

        bool HasError;
        string ErrorMessage = "";

        protected override void OnInitialized()
        {
            try
            {
                LoadRecon();
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = ex.Message;
                IsLoaded = true;
            }
        }



        void LoadRecon()
        {
            Recon = ReconService.GetById(ReconId);

            UnmatchedStatementLines = ReconService.GetUnmatchedStatementLines(ReconId);
            UnmatchedTransactions = ReconService.GetUnmatchedTransactions(ReconId);

            Matches = ReconService.GetMatches(ReconId);
            FilteredStatementLines = UnmatchedStatementLines;
            CurrentPage = 1;

            IsLoaded = true;
        }


        void SelectStatementLine(BankStatementLineModel line)
        {
            SelectedStatementLineId = line.BankStatementLineId;
            SelectedStatementLine = line;

            Suggestions = ReconService.GetSuggestions(
                ReconId,
                line.BankStatementLineId);

            DrawerOpen = true;
            StateHasChanged();
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

        void CloseDrawer()
        {
            DrawerOpen = false;
        }

        void Match(BankTransactionModel txn)
        {
            if (SelectedStatementLine == null)
                return;

            ReconService.Match(
                ReconId,
                SelectedStatementLine.BankStatementLineId,
                txn.Id
            );
            SelectedStatementLine = null;
            ToastService.ShowSuccess("Matched successfully");
            LoadRecon();
        }

        void AcceptSuggestion(MatchSuggestionView s)
        {
            ReconService.Match(
                ReconId,
                s.StatementLineId,
                s.TransactionId
            );

            DrawerOpen = false;
            SelectedStatementLine = null;
            ToastService.ShowSuccess("Matched successfully");
            LoadRecon();
        }

        void RunAutoMatch()
        {

            try
            {
                ReconService.RunAutoMatch(ReconId);
                ToastService.ShowSuccess("Auto-match completed");
                LoadRecon();
            }
            catch(Exception e)
            {
                ToastService.ShowError(e.Message);
            }

        }

        void FinalizeRun()
        {
            ReconService.Finalize(ReconId);
            ToastService.ShowSuccess("Reconciliation finalized");
            LoadRecon();
        }

        void ReopenRun()
        {
            ReconService.Reopen(ReconId, "Correction required");
            ToastService.ShowWarning("Reconciliation reopened");
            LoadRecon();
        }

        void MarkReviewCompleted()
        {
            try
            {
                ReconService.MarkReviewCompleted(ReconId);
                ToastService.ShowSuccess("Review marked as completed");
                LoadRecon();
            }
            catch(Exception e)
            {
                ToastService.ShowError(e.Message);
            }

        }

        string GetStatusBadge(ReconciliationStatus status)
        {
            return status switch
            {
                ReconciliationStatus.Draft => "bg-secondary-transparent text-secondary",
                ReconciliationStatus.InProgress => "bg-info-transparent text-info",
                ReconciliationStatus.Completed => "bg-warning-transparent text-warning",
                ReconciliationStatus.Finalized => "bg-success-transparent text-success",
                ReconciliationStatus.Reopened => "bg-danger-transparent text-danger",
                ReconciliationStatus.Cancelled => "bg-dark-transparent text-dark",
                ReconciliationStatus.Failed => "bg-danger-transparent text-danger",
                _ => "bg-dark"
            };
        }

        string GetStatusBadgeClass(ReconciliationStatus status) => status switch
        {
            ReconciliationStatus.Draft => "badge-draft",
            ReconciliationStatus.InProgress => "bg-info-transparent",
            ReconciliationStatus.Completed => "bg-warning-transparent",
            ReconciliationStatus.Finalized => "badge-posted",
            ReconciliationStatus.Reopened => "bg-danger-transparent",
            ReconciliationStatus.Cancelled => "badge-cancelled",
            ReconciliationStatus.Failed => "badge-reversed",
            _ => "badge-draft"
        };
    }
}
