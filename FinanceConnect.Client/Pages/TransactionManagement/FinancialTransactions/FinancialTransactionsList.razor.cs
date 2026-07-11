using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Data.Common;
using System.Transactions;

namespace FinanceConnect.Client.Pages.TransactionManagement.FinancialTransactions
{
    public partial class FinancialTransactionsList
    {

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
            VisibleColumnCount =
            await JS.InvokeAsync<int>("getVisibleTableColumns");

        }
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] TransactionStatusService TransactionStatusService { get; set; } = default!;

        Guid TenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        List<FinancialTransactionModel> Transactions = new();
        List<FinancialTransactionModel> Filtered = new();
        List<TransactionStatusModel> Status = new();
        FinancialTransactionModel SelectedTx;
        private FinancialTransactionStatistics Statistics = new();
        private bool isInitialized = false;
        private bool isLoading = false;
        // FILTERS
        string searchText = "";
        //Guid? selectedStatus = null;
        string selectedStatus = "";
        string selectedSource = "";
        string CurrencyName = "";
        Guid? selectedCompany = null;
        public List<CompanyModel> Companies = new();
        int PageSize = 10;
        int CurrentPage = 1;
        int PageWindowSize = 2;
        private int VisibleColumnCount;
        int TotalPages =>
            Filtered.Count == 0
                ? 1
                : (int)Math.Ceiling((double)Filtered.Count / PageSize);

        List<FinancialTransactionModel> PagedTx =>
            Filtered
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

        //Guid? SelectedStatus
        //{
        //    get => selectedStatus;
        //    set
        //    {
        //        selectedStatus = value;
        //        ApplyFilters();
        //    }
        //}
        string SelectedStatus
        {
            get => selectedStatus;
            set
            {
                selectedStatus = value;
                ApplyFilters();
            }
        }

        string SelectedSource
        {
            get => selectedSource;
            set
            {
                selectedSource = value;
                ApplyFilters();
            }
        }

        Guid? SelectedCompany
        {
            get => selectedCompany;
            set
            {
                selectedCompany = value;
                ApplyFilters();
            }
        }



        protected override void OnInitialized()
        {
            Companies = MasterDataService.GetAllCompanies().Where(c => c.Status == "Active").ToList();
            LoadTransactions();
        }

        // LOAD & REFRESH
        void LoadTransactions()
        {
            Transactions = TxService.GetAll();
            Status = TransactionStatusService.GetAll(TenantId);
            isInitialized = true;
            ApplyFilters();
        }

        void OnRefresh()
        {
            searchText = "";
            selectedStatus = "";
            selectedSource = "";
            selectedCompany = null;
        }

        // FILTERING
        async Task OnSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString() ?? "";
            ApplyFilters();
            VisibleColumnCount =
            await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        void ApplyFilters()
        {
            IEnumerable<FinancialTransactionModel> query = Transactions;

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(t =>
                    (t.DocumentNo?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (t.SourceDocumentNo?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            // Company filter
            if (selectedCompany.HasValue)
            {
                query = query.Where(b =>
                     b.CompanyId != null &&
                     b.CompanyId == selectedCompany.Value);
            }

            if (!string.IsNullOrWhiteSpace(selectedStatus))
                {
                    query = query.Where(t => t.Status == selectedStatus);
                }

            if (!string.IsNullOrWhiteSpace(selectedSource) &&
                Enum.TryParse<SourceModule>(selectedSource, out var sourceEnum))
            {
                query = query.Where(t => t.SourceModule == sourceEnum);
            }

            Filtered = query
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToList();

            CurrentPage = 1;
        }

        // PAGINATION
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

        // UI HELPERS
        void ViewTx(FinancialTransactionModel tx)
        {
            SelectedTx = tx;
            CurrencyName = SelectedTx.CurrencyId.HasValue
                ? MasterDataService
                    .GetCurrencyById(SelectedTx.CurrencyId.Value)
                    ?.CurrencyName ?? "—"
                : "—";
        }

        string GetCurrencyName(Guid? CurrencyId)
        {
            CurrencyName = CurrencyId.HasValue? MasterDataService
            .GetCurrencyById(CurrencyId.Value)?.CurrencyName ?? "—"
            : "—";
            return CurrencyName;
        }

        void SelectTx(FinancialTransactionModel tx)
        {
            SelectedTx = tx;
        }

        void SubmitTx()
        {
            TxService.Submit(SelectedTx.FinancialTransactionId);
            LoadTransactions();
            ToastService.ShowSuccess("Transaction submitted for approval");
        }

        void ApproveTx()
        {
            TxService.Approve(SelectedTx.FinancialTransactionId);
            ToastService.ShowSuccess("Transaction approved");
        }

        void PostTx()
        {
            TxService.Post(SelectedTx.FinancialTransactionId);
            ToastService.ShowSuccess("Transaction posted to ledger");
        }

        void ReverseTx()
        {
            TxService.Reverse(SelectedTx.FinancialTransactionId);
            LoadTransactions();
            ToastService.ShowSuccess("Reversal transaction created");
        }

        void CancelTx()
        {
            TxService.Cancel(SelectedTx.FinancialTransactionId);
            ToastService.ShowSuccess("Transaction cancelled to ledger");
        }

        string GetStatusBadge(string status)
        {
            return status switch
            {
                "Draft" => "bg-secondary-transparent text-secondary",
                "Submitted" => "bg-info-transparent text-info",
                "Approved" => "bg-primary-transparent text-primary",
                "Posted" => "bg-success-transparent text-success",
                "Reversed" => "bg-warning-transparent text-warning",
                "Cancelled" => "bg-danger-transparent text-danger",
                _ => "bg-secondary"
            };
        }
        string GetStatusDotBadge(string status)
        {
            return status switch
            {
                "Draft" => "bg-secondary text-secondary",
                "Submitted" => "bg-info text-info",
                "Approved" => "bg-primary text-primary",
                "Posted" => "bg-success text-success",
                "Reversed" => "bg-warning text-warning",
                "Cancelled" => "bg-danger text-danger",
                _ => "bg-secondary"
            };
        }

        // ACTIONS
        void DeleteConfirmed()
        {
            if (SelectedTx == null) return;

            TxService.Delete(SelectedTx.FinancialTransactionId);
            ToastService.ShowSuccess(
                $"Transaction {SelectedTx.DocumentNo} deleted",
                "Deleted");

            LoadTransactions();
        }

        private async Task OnRefreshAsync()
        {
            isLoading = true;
            StateHasChanged();

            await Task.Delay(300);
            OnRefresh();
            TxService.ResetToSeed();
            LoadTransactions();

             isLoading = false;
            await JS.InvokeVoidAsync("feather.replace");
            ToastService.ShowInfo("Transaction list refreshed", "Refreshed");
        }
    }
}
