using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static FinanceConnect.Client.ViewModels.TaxTranactionViewModel;

namespace FinanceConnect.Client.Pages.Tax___Statutory.TaxTranaction
{
    public partial class TaxTransaction
    {

        private bool isInitialized = false;
        private bool isLoading = false;
        private int VisibleColumnCount;
        private List<TaxTransactionModel> AllTransactions = new();

        private string searchText = string.Empty;
        private string _selectedTaxType = string.Empty;
        private string _selectedStatus = string.Empty;
        private string _selectedDocType = string.Empty;
        private string _selectedPeriod = string.Empty;

        private string SelectedTaxType
        {
            get => _selectedTaxType;
            set { _selectedTaxType = value; CurrentPage = 1; }
        }

        private string SelectedStatus
        {
            get => _selectedStatus;
            set { _selectedStatus = value; CurrentPage = 1; }
        }

        private string SelectedDocType
        {
            get => _selectedDocType;
            set { _selectedDocType = value; CurrentPage = 1; }
        }

        private string SelectedPeriod
        {
            get => _selectedPeriod;
            set { _selectedPeriod = value; CurrentPage = 1; }
        }

        private TaxTransactionModel? SelectedTx;

        private bool ShowReasonModal = false;
        private bool showReasonValidation = false;
        private string Reason = string.Empty;
        private string ModalTitle = string.Empty;
        private Func<Task>? PendingAction;

        private int CurrentPage = 1;
        private int PageSize = 10;

        private int TotalPages =>
            (int)Math.Ceiling(FilteredTransactions.Count / (double)PageSize);

        private IEnumerable<int> VisiblePages
        {
            get
            {
                int start = Math.Max(1, CurrentPage - 2);
                int end = Math.Min(TotalPages, start + 4);
                return Enumerable.Range(start, end - start + 1);
            }
        }

        private List<TaxTransactionModel> FilteredTransactions
        {
            get
            {
                var q = AllTransactions.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    var s = searchText.Trim().ToLower();
                    q = q.Where(t =>
                        (t.TaxTransactionNumber ?? "").ToLower().Contains(s) ||
                        t.SourceDocumentNumberSnapshot.ToLower().Contains(s) ||
                        t.PartyNameSnapshot.ToLower().Contains(s) ||
                        (t.PartyGSTINSnapshot ?? "").ToLower().Contains(s));
                }

                if (!string.IsNullOrWhiteSpace(_selectedTaxType))
                    q = q.Where(t => t.TaxType == _selectedTaxType);

                if (!string.IsNullOrWhiteSpace(_selectedStatus))
                    q = q.Where(t => t.TaxTransactionStatus == _selectedStatus);

                if (!string.IsNullOrWhiteSpace(_selectedDocType))
                    q = q.Where(t => t.SourceDocumentType == _selectedDocType);

                if (!string.IsNullOrWhiteSpace(_selectedPeriod))
                    q = q.Where(t => (t.TaxPeriodKey ?? "").Contains(_selectedPeriod));

                return q.OrderByDescending(t => t.PostingDate).ToList();
            }
        }

        private List<TaxTransactionModel> PagedTransactions =>
            FilteredTransactions
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
            isInitialized = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("feather.replace");
                await JS.InvokeVoidAsync("initTooltips");
                VisibleColumnCount =
    await JS.InvokeAsync<int>("getVisibleTableColumns");
            }
            else
            {
                await JS.InvokeVoidAsync("feather.replace");
                await JS.InvokeVoidAsync("initTooltips");
                VisibleColumnCount =
    await JS.InvokeAsync<int>("getVisibleTableColumns");
            }
        }

        private async Task LoadDataAsync()
        {
            isLoading = true;
            try
            {
                AllTransactions = await Task.Run(() => TaxTransactionService.GetList());
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task OnRefreshAsync()
        {
            searchText = string.Empty;
            _selectedTaxType = string.Empty;
            _selectedStatus = string.Empty;
            _selectedDocType = string.Empty;
            _selectedPeriod = string.Empty;
            CurrentPage = 1;
            await LoadDataAsync();
            await JS.InvokeVoidAsync("feather.replace");
            VisibleColumnCount =
await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        private void OnSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString() ?? string.Empty;
            CurrentPage = 1;

        }

        private void OpenViewModal(TaxTransactionModel tx) => SelectedTx = tx;
        private async Task OpenLinesModal(TaxTransactionModel tx)
        {
            SelectedTx = tx;
            await JS.InvokeVoidAsync("blazorOffcanvas.hide", "viewTaxTransactionOffcanvas");
            await Task.Delay(350);
            VisibleColumnCount =
await JS.InvokeAsync<int>("getVisibleTableColumns");
        }
        private async Task ClosepopupModal()
{
    await JS.InvokeVoidAsync("blazorModal.hide", "linesModal");
    await Task.Delay(200);
    VisibleColumnCount = await JS.InvokeAsync<int>("getVisibleTableColumns");
}
        private void AskExclude(TaxTransactionModel tx)
        {
            SelectedTx = tx;
            Reason = string.Empty;
            showReasonValidation = false;
            ModalTitle = $"Exclude from Return — {tx.TaxTransactionNumber}";
            PendingAction = () => ExcludeConfirmedAsync(tx);
            ShowReasonModal = true;
        }

        private async Task ExcludeConfirmedAsync(TaxTransactionModel tx)
        {
            isLoading = true;
            try
            {
                await Task.Run(() => TaxTransactionService.ExcludeFromReturn(tx.Id, Reason));

                tx.ReturnInclusionStatus = ReturnInclusionStatus.Excluded;
                tx.ExclusionReason = Reason;
                tx.IsIncludedInReturn = false;

                ToastService.ShowSuccess($"{tx.TaxTransactionNumber} excluded from return.");
            }
            catch (Exception ex)
            {
                ToastService.ShowError("Exclude failed: " + ex.Message);
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task ConfirmAction()
        {
            if (string.IsNullOrWhiteSpace(Reason))
            {
                showReasonValidation = true;
                return;
            }

            ShowReasonModal = false;

            if (PendingAction != null)
                await PendingAction.Invoke();
        }

        private void CloseModal()
        {
            ShowReasonModal = false;
            Reason = string.Empty;
            showReasonValidation = false;
            PendingAction = null;
        }

        private void GoToPage(int page) => CurrentPage = page;
        private void PreviousPage() { if (CurrentPage > 1) CurrentPage--; }
        private void NextPage() { if (CurrentPage < TotalPages) CurrentPage++; }
        private void OnPageSizeChange(ChangeEventArgs e)
        {
            PageSize = int.TryParse(e.Value?.ToString(), out var v) ? v : 10;
            CurrentPage = 1;
        }

        private static string GetTaxTypeBadge(string type) => type switch
        {
            "GST" => "bg-success-transparent",
            "TDS" => "bg-warning-transparent text-dark",
            "TCS" => "bg-purple-transparent",
            "Mixed" => "bg-secondary-transparent",
            _ => "bg-secondary-transparent"
        };

        private static string GetStatusBadge(string status) => status switch
        {
            "Draft" => "bg-secondary-transparent",
            "Posted" => "bg-primary-transparent",
            "IncludedInReturn" => "bg-success-transparent",
            "Excluded" => "bg-warning-transparent text-dark",
            "PartiallySettled" => "bg-info-transparent text-dark",
            "Settled" => "bg-success-transparent",
            "Reversed" => "bg-warning-transparent text-dark",
            "Cancelled" => "bg-danger-transparent",
            _ => "bg-secondary-transparent"
        };

        private static string GetStatusDot(string status) => status switch
        {
            "Posted" => "bg-primary",
            "IncludedInReturn" => "bg-success",
            "Settled" => "bg-success",
            "Reversed" => "bg-warning",
            "Cancelled" => "bg-danger",
            "Excluded" => "bg-warning",
            _ => "bg-secondary"
        };

        private static string GetReturnStatusBadge(string status) => status switch
        {
            "Included" => "bg-success-transparent",
            "Pending" => "bg-warning-transparent text-dark",
            "Excluded" => "bg-danger-transparent",
            "NotApplicable" => "bg-secondary-transparent",
            "Amended" => "bg-info-transparent text-dark",
            _ => "bg-secondary-transparent"
        };

        private static string GetReturnStatusDot(string status) => status switch
        {
            "Included" => "bg-success",
            "Pending" => "bg-warning",
            "Excluded" => "bg-danger",
            "NotApplicable" => "bg-secondary",
            _ => "bg-secondary"
        };

        private static string GetReconciliationBadge(string status) => status switch
        {
            "Matched" => "bg-success-transparent",
            "PartiallyMatched" => "bg-warning-transparent text-dark",
            "Mismatch" => "bg-danger-transparent",
            "NotReconciled" => "bg-secondary-transparent",
            _ => "bg-secondary-transparent"
        };

        private static string GetSupplyTypeBadge(string type) => type switch
        {
            "IntraState" => "bg-success-transparent",
            "InterState" => "bg-primary-transparent",
            "Export" => "bg-info-transparent text-dark",
            "Import" => "bg-warning-transparent text-dark",
            "SEZ" => "bg-purple-transparent",
            "DeemedExport" => "bg-secondary-transparent",
            _ => "bg-secondary-transparent"
        };

        private static string GetDocTypeBadge(string type) => type switch
        {
            "CustomerInvoice" => "bg-primary-transparent",
            "CustomerCreditNote" => "bg-warning-transparent text-dark",
            "CustomerDebitNote" => "bg-info-transparent text-dark",
            "VendorBill" => "bg-secondary-transparent text-secondary",
            "VendorCreditNote" => "bg-warning-transparent text-dark",
            "VendorPayment" => "bg-success-transparent",
            "AssetAcquisition" => "bg-purple-transparent text-purple",
            _ => "bg-secondary-transparent"
        };

        private static string GetDocTypeShort(string type) => type switch
        {
            "CustomerInvoice" => "AR Invoice",
            "CustomerCreditNote" => "AR CN",
            "CustomerDebitNote" => "AR DN",
            "VendorBill" => "AP Bill",
            "VendorCreditNote" => "AP CN",
            "VendorDebitNote" => "AP DN",
            "VendorPayment" => "AP Payment",
            "AssetAcquisition" => "Asset Acq.",
            "ManualAdjustment" => "Manual Adj.",
            _ => type
        };

        private static string GetDirectionBadge(string dir) => dir switch
        {
            "Input" => "bg-success-transparent",
            "Output" => "bg-danger-transparent",
            "WithholdingPayable" => "bg-warning-transparent text-dark",
            _ => "bg-secondary"
        };

        private static string GetDirectionShort(string dir) => dir switch
        {
            "Input" => "Input",
            "Output" => "Output",
            "WithholdingPayable" => "Withhold",
            _ => dir
        };

        private static string GetLineStatusBadge(string status) => status switch
        {
            "Posted" => "bg-primary-transparent",
            "Settled" => "bg-success-transparent",
            "Reversed" => "bg-warning-transparent text-dark",
            "Excluded" => "bg-secondary-transparent",
            "PartiallySettled" => "bg-info-transparent text-dark",
            _ => "bg-light-transparent text-dark"
        };
    }
}
