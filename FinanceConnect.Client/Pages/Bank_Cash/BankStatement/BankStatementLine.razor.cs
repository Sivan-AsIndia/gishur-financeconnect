using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;

namespace FinanceConnect.Client.Pages.Bank_Cash.BankStatement
{
    public partial class BankStatementLine
    {
        // =========================
        // DI
        // =========================
        [Inject] public BankStatementService StatementService { get; set; } = default!;
        //[Inject] public BankReconciliationService ReconciliationService { get; set; } = default!;
        [Inject] public ToastService ToastService { get; set; } = default!;
        [Inject] public NavigationManager Nav { get; set; } = default!;

        // =========================
        // PARAMETERS
        // =========================
        [Parameter] public Guid BankStatementId { get; set; }
        [Parameter] public Guid StatementId { get; set; }
        [Parameter] public bool ShowReconciliationActions { get; set; } = true;

        // =========================
        // STATE
        // =========================
        protected bool isInitialized = false;

        protected List<BankStatementLineModel> StatementLines = new();
        BankStatementModel? SelectedStatement;
        protected BankStatementLineModel? SelectedLine;

        // Paging
        protected int CurrentPage = 1;
        protected int PageSize = 25;
        protected int TotalCount = 0;
        int displayLineNo = 0;
        // Filters
        protected DateTime? FilterFromDate;
        protected DateTime? FilterToDate;
        protected StatementLineDirectionType? FilterDirection;
        protected ReconciliationStatusType? FilterStatus;
        protected string SearchText = string.Empty;

        // =========================
        // LIFECYCLE
        // =========================
        protected override async Task OnInitializedAsync()
        {
            if (StatementId == Guid.Empty)
            {
                ToastService.ShowError("Invalid bank statement context");
                return;
            }

            await LoadLines();
            isInitialized = true;
        }

        // =========================
        // DATA LOAD
        // =========================
        protected async Task LoadLines()
        {
            try
            {
                SelectedStatement = StatementService.GetById(StatementId);

                if (SelectedStatement != null)
                {
                    StatementLines = StatementService
                        .GetLines(SelectedStatement.BankStatementId)
                        .Take(100) // Preview only
                        .ToList();
                }
                TotalCount = StatementLines.Count;
            }
            catch (Exception ex)
            {
                ToastService.ShowError(ex.Message, "Load Failed");
            }
        }

        // =========================
        // RECON ACTIONS
        // =========================
        protected async Task MatchLine(BankStatementLineModel line)
        {
            try
            {
                if (line.ReconciliationStatus == ReconciliationStatusType.FinalizedLocked)
                {
                    ToastService.ShowWarning("This line is locked and cannot be matched.");
                    return;
                }

                // Navigate to reconciliation workbench
                Nav.NavigateTo($"/bank-reconciliation/{line.BankStatementLineId}");
            }
            catch (Exception ex)
            {
                ToastService.ShowError(ex.Message, "Match Failed");
            }
        }

        protected async Task ExcludeLine(BankStatementLineModel line)
        {
            try
            {
                if (line.ReconciliationStatus == ReconciliationStatusType.FinalizedLocked)
                {
                    ToastService.ShowWarning("This line is locked and cannot be excluded.");
                    return;
                }

                //var result = await ReconciliationService.ExcludeLine(
                //    line.BankStatementLineId,
                //    "Manually excluded by controller"
                //);

                //if (result.Success)
                //{
                //    ToastService.ShowSuccess("Line excluded");
                //    await LoadLines();
                //}
                //else
                //{
                //    ToastService.ShowError(result.Message);
                //}
            }
            catch (Exception ex)
            {
                ToastService.ShowError(ex.Message, "Exclude Failed");
            }
        }

        protected void OpenLineDetails(BankStatementLineModel line)
        {
            SelectedLine = line;
            // Trigger Bootstrap drawer or modal
        }

        // =========================
        // FILTERS
        // =========================
        protected async Task ApplyFilters()
        {
            CurrentPage = 1;
            await LoadLines();
        }

        protected async Task ClearFilters()
        {
            FilterFromDate = null;
            FilterToDate = null;
            FilterDirection = null;
            FilterStatus = null;
            SearchText = string.Empty;

            CurrentPage = 1;
            await LoadLines();
        }

        // =========================
        // PAGINATION
        // =========================
        protected int TotalPages =>
            (int)Math.Ceiling((double)TotalCount / PageSize);

        protected async Task NextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                await LoadLines();
            }
        }

        protected async Task PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                await LoadLines();
            }
        }

        protected async Task GoToPage(int page)
        {
            if (page >= 1 && page <= TotalPages)
            {
                CurrentPage = page;
                await LoadLines();
            }
        }

        protected async Task OnPageSizeChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out var size))
            {
                PageSize = size;
                CurrentPage = 1;
                await LoadLines();
            }
        }

        // =========================
        // UI HELPERS
        // =========================
        protected string GetReconciliationBadge(ReconciliationStatusType status)
        {
            return status switch
            {
                ReconciliationStatusType.Unmatched => "bg-secondary",
                ReconciliationStatusType.Suggested => "bg-info",
                ReconciliationStatusType.Matched => "bg-success",
                ReconciliationStatusType.FinalizedLocked => "bg-dark",
                ReconciliationStatusType.Excluded => "bg-danger",
                _ => "bg-secondary"
            };
        }

        protected string GetDirectionBadge(StatementLineDirectionType direction)
        {
            return direction switch
            {
                StatementLineDirectionType.Debit => "bg-danger",
                StatementLineDirectionType.Credit => "bg-success",
                _ => "bg-secondary"
            };
        }
    }
}
