using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Reflection;

namespace FinanceConnect.Client.Pages.AR.ARAdjustmentList
{
    public partial class ARAdjustmentList : ComponentBase
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] ARAdjustmentService AdjustmentService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;

        private bool isInitialized = false;
        private bool isLoading = false;

        private List<ARAdjustmentViewModel> Adjustments = new();
        private List<ARAdjustmentViewModel> FilteredAdjustments = new();
        private ARAdjustmentStatisticsViewModel Statistics = new();

        private string searchText = string.Empty;
        private string statusFilter = string.Empty;
        private string typeFilter = string.Empty;
        private DateTime? FromDate;
        private DateTime? ToDate;

        private ARAdjustmentViewModel? SelectedAdjustment;
        private string cancelReasonInput = string.Empty;
        private string reverseReasonInput = string.Empty;
        private int VisibleColumnCount;

        // Data-driven filter options (only values present in table)
        private List<string> AvailableStatuses => Adjustments
            .Select(a => a.AdjustmentStatus)
            .Where(s => !string.IsNullOrEmpty(s))
            .Distinct().OrderBy(s => s).ToList();

        private List<string> AvailableTypes => Adjustments
            .Select(a => a.AdjustmentType)
            .Where(t => !string.IsNullOrEmpty(t))
            .Distinct().OrderBy(t => t).ToList();
        // Pagination
        private int CurrentPage = 1;
        private int PageSize = 10;
        private int TotalPages => FilteredAdjustments.Count > 0 ? (int)Math.Ceiling((double)FilteredAdjustments.Count / PageSize) : 0;
        private IEnumerable<ARAdjustmentViewModel> PagedAdjustments => FilteredAdjustments
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize);

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
            isInitialized = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips", true);
            VisibleColumnCount =
    await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        private async Task LoadData()
        {
            isLoading = true;
            StateHasChanged();
            ToDate = null;
            FromDate = null;
            typeFilter = null;
            statusFilter = null;
            searchText = "";
            await Task.Delay(100); // Simulate async load

            Adjustments = AdjustmentService.GetAll();
            Statistics = AdjustmentService.GetStatistics();
            ApplyFilters();

            isLoading = false;
            StateHasChanged();
        }

        private async Task OnRefreshAsync()
        {
            AdjustmentService.ResetToSeed();
            await LoadData();
            ToastService.ShowSuccess("Data refreshed successfully");
        }

        private async Task OnSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString() ?? string.Empty;
            CurrentPage = 1;
            ApplyFilters();
            VisibleColumnCount =
    await JS.InvokeAsync<int>("getVisibleTableColumns");
        }


        string GetAdjustmentTypeIcon(string type)
        {
            return type switch
            {
                AdjustmentTypes.WriteOff => "ti ti-cut",
                AdjustmentTypes.Rounding => "ti ti-adjustments",
                AdjustmentTypes.DisputeSettlement => "ti ti-alert-circle",
                AdjustmentTypes.ShortPaymentSettlement => "ti ti-minus",
                AdjustmentTypes.Reclassification => "ti ti-repeat",
                AdjustmentTypes.BadDebtProvision => "ti ti-trash",
                AdjustmentTypes.Other => "ti ti-dots",

                _ => "ti ti-info-circle"
            };
        }


        private void ApplyFilters()
        {
            var query = Adjustments.AsEnumerable();

            // Search filter
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var term = searchText.ToLower();
                query = query.Where(a =>
                    (a.AdjustmentNumber?.ToLower().Contains(term) ?? false) ||
                    (a.CustomerCode?.ToLower().Contains(term) ?? false) ||
                    (a.CustomerName?.ToLower().Contains(term) ?? false) ||
                    (a.AdjustmentNarration?.ToLower().Contains(term) ?? false));
            }

            // Status filter
            if (!string.IsNullOrWhiteSpace(statusFilter))
            {
                query = query.Where(a => a.AdjustmentStatus == statusFilter);
            }

            // Type filter
            if (!string.IsNullOrWhiteSpace(typeFilter))
            {
                query = query.Where(a => a.AdjustmentType == typeFilter);
            }

            // Date filters
            if (FromDate.HasValue)
            {
                query = query.Where(a => a.AdjustmentDate >= FromDate.Value);
            }
            if (ToDate.HasValue)
            {
                query = query.Where(a => a.AdjustmentDate <= ToDate.Value);
            }

            FilteredAdjustments = query.OrderByDescending(a => a.AdjustmentDate)
                                       .ThenByDescending(a => a.AdjustmentNumber)
                                       .ToList();

            // Reset to page 1 if current page exceeds total
            if (CurrentPage > TotalPages && TotalPages > 0)
            {
                CurrentPage = 1;
            }
        }

        private void ClearFilters()
        {
            searchText = string.Empty;
            statusFilter = string.Empty;
            typeFilter = string.Empty;
            FromDate = null;
            ToDate = null;
            CurrentPage = 1;
            ApplyFilters();
        }

        #region Pagination

        private void GoToPage(int page)
        {
            if (page >= 1 && page <= TotalPages)
            {
                CurrentPage = page;
            }
        }

        private void PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
            }
        }

        private void NextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
            }
        }

        private void OnPageSizeChange(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out var size))
            {
                PageSize = size;
                CurrentPage = 1;
            }
        }

        #endregion

        #region Modal Handlers

        private void OpenRowDetails(ARAdjustmentViewModel adjustment)
        {
            SelectedAdjustment = adjustment;
        }

        private void OpenSubmitModal(ARAdjustmentViewModel adjustment)
        {
            SelectedAdjustment = adjustment;
        }

        private void OpenApproveModal(ARAdjustmentViewModel adjustment)
        {
            SelectedAdjustment = adjustment;
        }

        private void OpenPostModal(ARAdjustmentViewModel adjustment)
        {
            SelectedAdjustment = adjustment;
        }

        private void OpenCancelModal(ARAdjustmentViewModel adjustment)
        {
            SelectedAdjustment = adjustment;
            cancelReasonInput = string.Empty;
        }

        private void OpenReverseModal(ARAdjustmentViewModel adjustment)
        {
            SelectedAdjustment = adjustment;
            reverseReasonInput = string.Empty;
        }

        private void OpenDeleteModal(ARAdjustmentViewModel adjustment)
        {
            SelectedAdjustment = adjustment;
        }

        #endregion

        #region Actions

        private async Task SubmitAdjustment()
        {
            if (SelectedAdjustment == null) return;

            var result = AdjustmentService.Submit(SelectedAdjustment.Id, "System");

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                await LoadData();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        private async Task PostAdjustment()
        {
            if (SelectedAdjustment == null) return;

            var result = AdjustmentService.Post(
                SelectedAdjustment.Id,
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                "System");

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                await LoadData();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        private async Task CancelAdjustment()
        {
            if (SelectedAdjustment == null) return;

            if (string.IsNullOrWhiteSpace(cancelReasonInput))
            {
                ToastService.ShowWarning("Cancellation reason is required.");
                return;
            }

            var result = AdjustmentService.Cancel(
                SelectedAdjustment.Id,
                cancelReasonInput,
                "System");

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                await LoadData();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        private async Task ReverseAdjustment()
        {
            if (SelectedAdjustment == null) return;

            if (string.IsNullOrWhiteSpace(reverseReasonInput))
            {
                ToastService.ShowWarning("Reversal reason is required.");
                return;
            }

            var result = AdjustmentService.Reverse(
                SelectedAdjustment.Id,
                reverseReasonInput,
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                "System");

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                await LoadData();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        private async Task DeleteAdjustment()
        {
            if (SelectedAdjustment == null) return;

            var result = AdjustmentService.Delete(SelectedAdjustment.Id, "System");

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message);
                await LoadData();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        #endregion

        #region Helper Methods

        private string TruncateName(string? name, int maxLength)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;
            return name.Length <= maxLength ? name : name.Substring(0, maxLength) + "...";
        }

        private static string GetStatusBadgeClass(string status) => status switch
        {
            AdjustmentStatuses.Draft => "bg-secondary-transparent text-secondary",
            AdjustmentStatuses.Submitted => "bg-info-transparent text-info",
            AdjustmentStatuses.Approved => "bg-primary-transparent text-primary",
            AdjustmentStatuses.Posted => "bg-success-transparent text-success",
            AdjustmentStatuses.Cancelled => "bg-warning-transparent text-warning",
            AdjustmentStatuses.Reversed => "bg-danger-transparent text-danger",
            _ => "bg-secondary-transparent text-secondary"
        };
        private static string GetStatusDotBadgeClass(string status) => status switch
        {
            AdjustmentStatuses.Draft => "bg-secondary text-secondary",
            AdjustmentStatuses.Submitted => "bg-info text-info",
            AdjustmentStatuses.Approved => "bg-primary text-primary",
            AdjustmentStatuses.Posted => "bg-success text-success",
            AdjustmentStatuses.Cancelled => "bg-warning text-warning",
            AdjustmentStatuses.Reversed => "bg-danger text-danger",
            _ => "bg-secondary text-secondary"
        };

        private static string GetTypeBadgeClass(string type) => type switch
        {
            AdjustmentTypes.WriteOff => "bg-danger-transparent text-danger",
            AdjustmentTypes.Rounding => "bg-info-transparent text-info",
            AdjustmentTypes.DisputeSettlement => "bg-warning-transparent text-warning",
            AdjustmentTypes.ShortPaymentSettlement => "bg-pink-transparent text-pink",
            AdjustmentTypes.Reclassification => "bg-primary-transparent text-primary",
            AdjustmentTypes.BadDebtProvision => "bg-purple-transparent text-purple",
            AdjustmentTypes.Other => "bg-secondary-transparent text-secondary",
            _ => "bg-secondary-transparent text-secondary"
        };

        private static string GetDirectionBadgeClass(string direction) => direction switch
        {
            AdjustmentDirections.ReduceAR => "bg-danger-transparent text-danger",
            AdjustmentDirections.IncreaseAR => "bg-success-transparent text-success",
            _ => "bg-secondary-transparent text-secondary"
        };

        private static string GetApprovalBadgeClass(string status) => status switch
        {
            ARAdjustmentApprovalStatuses.NotRequired => "bg-secondary-transparent text-secondary",
            ARAdjustmentApprovalStatuses.Pending => "bg-warning-transparent text-warning",
            ARAdjustmentApprovalStatuses.Approved => "bg-success-transparent text-success",
            ARAdjustmentApprovalStatuses.Rejected => "bg-danger-transparent text-danger",
            _ => "bg-secondary-transparent text-secondary"
        };

        #endregion
    }
}
