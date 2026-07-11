using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FinanceConnect.Client.Pages.AP.APAdjustment
{
    public partial class APAdjustmentList : ComponentBase
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] APAdjustmentService AdjustmentService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;

        private bool isInitialized = false;
        private bool isLoading = false;

        private List<APAdjustmentViewModel> Adjustments = new();
        private List<APAdjustmentViewModel> FilteredAdjustments = new();
        private APAdjustmentStatisticsViewModel Statistics = new();

        private string searchText = string.Empty;
        private string statusFilter = string.Empty;
        private string typeFilter = string.Empty;
        private DateTime? FromDate;
        private DateTime? ToDate;

        private APAdjustmentViewModel? SelectedAdjustment;
        private string cancelReasonInput = string.Empty;
        private string reverseReasonInput = string.Empty;
        private int VisibleColumnCount;

        // Distinct values from data for filter dropdowns
        private List<string> DistinctStatuses => Adjustments
            .Select(a => a.AdjustmentStatus)
            .Where(s => !string.IsNullOrEmpty(s))
            .Distinct()
            .OrderBy(s => Array.IndexOf(APAdjustmentStatuses.All, s))
            .ToList();

        private List<string> DistinctTypes => Adjustments
            .Select(a => a.AdjustmentType)
            .Where(t => !string.IsNullOrEmpty(t))
            .Distinct()
            .OrderBy(t => Array.IndexOf(APAdjustmentTypes.All, t))
            .ToList();
        // Pagination
        private int CurrentPage = 1;
        private int PageSize = 10;
        private int TotalPages => FilteredAdjustments.Count > 0 ? (int)Math.Ceiling((double)FilteredAdjustments.Count / PageSize) : 0;
        private IEnumerable<APAdjustmentViewModel> PagedAdjustments => FilteredAdjustments
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
            await JS.InvokeVoidAsync("initTooltips");
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

        private void OnSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString() ?? string.Empty;
            CurrentPage = 1;
            ApplyFilters();
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
                    (a.VendorCode?.ToLower().Contains(term) ?? false) ||
                    (a.VendorName?.ToLower().Contains(term) ?? false) ||
                    (a.Narration?.ToLower().Contains(term) ?? false));
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

        private void OpenRowDetails(APAdjustmentViewModel adjustment)
        {
            SelectedAdjustment = adjustment;
        }

        private void OpenSubmitModal(APAdjustmentViewModel adjustment)
        {
            SelectedAdjustment = adjustment;
        }

        private void OpenApproveModal(APAdjustmentViewModel adjustment)
        {
            SelectedAdjustment = adjustment;
        }

        private void OpenPostModal(APAdjustmentViewModel adjustment)
        {
            SelectedAdjustment = adjustment;
        }

        private void OpenCancelModal(APAdjustmentViewModel adjustment)
        {
            SelectedAdjustment = adjustment;
            cancelReasonInput = string.Empty;
        }

        private void OpenReverseModal(APAdjustmentViewModel adjustment)
        {
            SelectedAdjustment = adjustment;
            reverseReasonInput = string.Empty;
        }

        private void OpenDeleteModal(APAdjustmentViewModel adjustment)
        {
            SelectedAdjustment = adjustment;
        }

        #endregion

        #region Actions

        private async Task SubmitAdjustment()
        {
            if (SelectedAdjustment == null) return;

            var result = AdjustmentService.Submit(SelectedAdjustment.APAdjustmentId, "System");

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
                SelectedAdjustment.APAdjustmentId,
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
                SelectedAdjustment.APAdjustmentId,
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
                SelectedAdjustment.APAdjustmentId,
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

            var result = AdjustmentService.Delete(SelectedAdjustment.APAdjustmentId, "System");

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
            APAdjustmentStatuses.Draft => "bg-secondary-transparent text-secondary",
            APAdjustmentStatuses.Submitted => "bg-info-transparent text-info",
            APAdjustmentStatuses.Approved => "bg-primary-transparent text-primary",
            APAdjustmentStatuses.Rejected => "bg-danger-transparent text-danger",
            APAdjustmentStatuses.Posted => "bg-success-transparent text-success",
            APAdjustmentStatuses.Cancelled => "bg-warning-transparent text-warning",
            APAdjustmentStatuses.Reversed => "bg-danger-transparent text-danger",
            _ => "bg-secondary-transparent text-secondary"
        };
        private static string GetStatusDotBadgeClass(string status) => status switch
        {
            APAdjustmentStatuses.Draft => "bg-secondary text-secondary",
            APAdjustmentStatuses.Submitted => "bg-info text-info",
            APAdjustmentStatuses.Approved => "bg-primary text-primary",
            APAdjustmentStatuses.Rejected => "bg-danger text-danger",
            APAdjustmentStatuses.Posted => "bg-success text-success",
            APAdjustmentStatuses.Cancelled => "bg-warning text-warning",
            APAdjustmentStatuses.Reversed => "bg-danger text-danger",
            _ => "bg-secondary text-secondary"
        };


        private string GetAdjustmentTypeIcon(string type)
        {
            return type switch
            {
                APAdjustmentTypes.WriteOff => "ti ti-file-minus",
                APAdjustmentTypes.RoundOffCorrection => "ti ti-arrows-random",
                APAdjustmentTypes.DisputeSettlement => "ti ti-gavel",
                APAdjustmentTypes.Reclassification => "ti ti-repeat",
                APAdjustmentTypes.VendorBalanceTransfer => "ti ti-transfer",
                APAdjustmentTypes.FXDifference => "ti ti-currency-exchange",
                APAdjustmentTypes.Other => "ti ti-dots",

                _ => "ti ti-help"
            };
        }


        private static string GetTypeBadgeClass(string type) => type switch
        {
            APAdjustmentTypes.WriteOff => "bg-danger-transparent text-danger",
            APAdjustmentTypes.RoundOffCorrection => "bg-info-transparent text-info",
            APAdjustmentTypes.DisputeSettlement => "bg-warning-transparent text-warning",
            APAdjustmentTypes.Reclassification => "bg-primary-transparent text-primary",
            APAdjustmentTypes.VendorBalanceTransfer => "bg-purple-transparent text-purple",
            APAdjustmentTypes.FXDifference => "bg-pink-transparent text-pink",
            APAdjustmentTypes.Other => "bg-secondary-transparent text-secondary",
            _ => "bg-secondary-transparent text-secondary"
        };

        private static string GetDirectionBadgeClass(string direction) => direction switch
        {
            APAdjustmentDirections.ReducePayable => "bg-success-transparent text-success",
            APAdjustmentDirections.IncreasePayable => "bg-danger-transparent text-danger",
            _ => "bg-secondary-transparent text-secondary"
        };
        private string GetAdjustmentDirectionIcon(string direction)
        {
            return direction switch
            {
                APAdjustmentDirections.ReducePayable => "ti ti-arrow-down-circle",
                APAdjustmentDirections.IncreasePayable => "ti ti-arrow-up-circle",

                _ => "ti ti-help"
            };
        }

        #endregion
    }
}
