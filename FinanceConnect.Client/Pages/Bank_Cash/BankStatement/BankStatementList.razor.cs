using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.Bank_Cash.BankStatement
{
    public partial class BankStatementList
    {
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
            VisibleColumnCount =
           await JS.InvokeAsync<int>("getVisibleTableColumns");
        }
        [Inject] BankStatementService StatementService { get; set; } = default!;
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;

        Guid TenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        BankStatementModel? SelectedStatement;

        List<BankStatementModel> Statements = new();
        List<BankStatementModel> FilteredStatements = new();
        private BankStatementStatistics Statistics = new();
        private bool isInitialized = false;
        private bool isLoading = false;
        string SupersedeReason = "";
        string searchText = "";
        StatementStatusType? selectedStatus = null;
        private int VisibleColumnCount;

        private List<StatementStatusType> AvailableStatuses =>
            Statements.Select(s => s.StatementStatus).Distinct().OrderBy(s => s).ToList();
        int PageSize = 10;
        int CurrentPage = 1;
        int PageWindowSize = 2;

        int TotalPages =>
            FilteredStatements.Count == 0
                ? 1
                : (int)Math.Ceiling((double)FilteredStatements.Count / PageSize);

        List<BankStatementModel> PagedStatements =>
            FilteredStatements
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

        StatementStatusType? SelectedStatus
        {
            get => selectedStatus;
            set
            {
                selectedStatus = value;
                ApplyFilters();
            }
        }

        protected override void OnInitialized()
        {
            LoadStatements();
        }


        void LoadStatements()
        {
            Statements = StatementService.GetAll(TenantId);
            Statistics = StatementService.GetStatistics(TenantId);
            isInitialized = true;
            ApplyFilters();
        }

        void OnRefresh()
        {
            searchText = "";
            selectedStatus = null;
        }

        async Task OnSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString() ?? "";
            ApplyFilters();
            VisibleColumnCount =
   await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        void ApplyFilters()
        {
            IEnumerable<BankStatementModel> query = Statements;

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(s =>
                    (!string.IsNullOrWhiteSpace(s.StatementNumber) &&
                     s.StatementNumber.Contains(searchText, StringComparison.OrdinalIgnoreCase)) ||

                    (!string.IsNullOrWhiteSpace(s.BankAccountName) &&
                     s.BankAccountName.Contains(searchText, StringComparison.OrdinalIgnoreCase)) ||

                    (!string.IsNullOrWhiteSpace(s.FileNameOriginal) &&
                     s.FileNameOriginal.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                );
            }

            if (selectedStatus.HasValue)
            {
                query = query.Where(s => s.StatementStatus == selectedStatus.Value);
            }

            FilteredStatements = query
                .OrderByDescending(s => s.FileUploadedAt)
                .ToList();

            CurrentPage = 1;
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

        void ImportStatement()
        {
            Nav.NavigateTo("/bank-statements/import");
        }

        void ViewStatement(BankStatementModel stmt)
        {
            Nav.NavigateTo($"/bank-statements/{stmt.BankStatementId}/view");
        }

        void Reconciliation(BankStatementModel stmt)
        {
            Nav.NavigateTo($"/bank-reconciliation/{stmt.BankStatementId}");
        }

        async Task DeleteConfirmed()
        {
            if (SelectedStatement == null)
                return;

            StatementService.Delete(SelectedStatement.BankStatementId);
            ToastService.ShowError($"{SelectedStatement.StatementNumber} Deleted Successfully");
            LoadStatements();
            SelectedStatement = null;

            CurrentPage = 1;

            await JS.InvokeVoidAsync("closeDeleteModal");
        }

        void SelectStatement(BankStatementModel stmt)
        {
            SelectedStatement = stmt;
        }

        void LockConfirmed()
        {
            if (SelectedStatement == null) return;

            try
            {
                StatementService.Lock(SelectedStatement.BankStatementId);
                ToastService.ShowSuccess($"Statement {SelectedStatement.StatementNumber} locked");
            }
            catch (Exception ex)
            {
                ToastService.ShowError(ex.Message);
            }

            LoadStatements();
        }

        void ArchiveConfirmed()
        {
            if (SelectedStatement == null) return;

            StatementService.Archive(
                SelectedStatement.BankStatementId,
                "ui.user"
            );

            LoadStatements();
        }
        string GetStatusBadgeClass(StatementStatusType status)
        {
            return status switch
            {
                StatementStatusType.Uploaded => "bg-secondary-transparent text-secondary",
                StatementStatusType.ParsingInProgress => "bg-info-transparent text-info",
                StatementStatusType.Parsed => "bg-primary-transparent text-secondary",
                StatementStatusType.ValidationFailed => "bg-danger-transparent text-danger",
                StatementStatusType.ReadyForReconciliation => "bg-success-transparent text-success",
                StatementStatusType.Locked => "bg-dark-transparent text-dark",
                StatementStatusType.Archived => "bg-light text-dark",
                StatementStatusType.Superseded => "bg-warning-transparent text-warning",
                _ => "bg-secondary-transparent text-secondary"
            };
        }

        string GetStatusIconClass(StatementStatusType status)
        {
            return status switch
            {
                StatementStatusType.Uploaded => "ti ti-upload",
                StatementStatusType.ParsingInProgress => "ti ti-loader",
                StatementStatusType.Parsed => "ti ti-check",
                StatementStatusType.ValidationFailed => "ti ti-alert-circle",
                StatementStatusType.ReadyForReconciliation => "ti ti-circle-check",
                StatementStatusType.Locked => "ti ti-lock",
                StatementStatusType.Archived => "ti ti-archive",
                StatementStatusType.Superseded => "ti ti-history",
                _ => "ti ti-info-circle"
            };
        }




        void SupersedeConfirmed()
        {
            if (SelectedStatement == null)
                return;

            if (string.IsNullOrWhiteSpace(SupersedeReason))
            {
                ToastService.ShowError("Supersede reason is required for audit compliance.");
                return;
            }

            try
            {
                StatementService.Supersede(
                    SelectedStatement.BankStatementId,
                    SupersedeReason,
                    user: "controller.user" // replace with logged-in user
                );

                ToastService.ShowSuccess(
                    $"Statement {SelectedStatement.StatementNumber} superseded successfully"
                );

                SupersedeReason = ""; // reset after success
                LoadStatements();
            }
            catch (Exception ex)
            {
                ToastService.ShowError(ex.Message);
            }
        }


        private async Task OnRefreshAsync()
        {
            isLoading = true;
            StateHasChanged();

            await Task.Delay(300);
            OnRefresh();
            StatementService.ResetToSeed();
            LoadStatements();

            isLoading = false;
            await JS.InvokeVoidAsync("feather.replace");
            ToastService.ShowInfo("Statement list refreshed", "Refreshed");
        }

    }
}
