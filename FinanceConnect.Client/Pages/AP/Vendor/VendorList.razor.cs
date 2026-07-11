using FinanceConnect.Client.ViewModels;
using FinanceConnect.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinanceConnect.Client.Pages.AP.Vendor
{
    public partial class VendorList
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject] VendorService VendorService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;
        [Inject] AuthService AuthService { get; set; } = default!;

        private bool isInitialized = false;
        private bool isLoading = false;

        private List<VendorViewModel> AllVendors = new();
        private VendorViewModel? SelectedVendor;

        // Validation flags
        private bool canDeactivate = true;
        private bool canDelete = true;

        // Search and Filter
        private string searchText = "";
        private string _selectedVendorType = "";
        private string _selectedStatus = "";
        private int VisibleColumnCount;
        public string SelectedVendorType
        {
            get => _selectedVendorType;
            set { _selectedVendorType = value; CurrentPage = 1; }
        }

        public string SelectedStatus
        {
            get => _selectedStatus;
            set { _selectedStatus = value; CurrentPage = 1; }
        }

        // Pagination
        private int CurrentPage = 1;
        private int PageSize = 10;

        // Hold Modal
        private string holdReasonInput = "";

        protected override async Task OnInitializedAsync()
        {
            LoadVendors();
            isInitialized = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("feather.replace");
            await JS.InvokeVoidAsync("initTooltips");
            VisibleColumnCount =
   await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        private void LoadVendors()
        {
            SelectedStatus = null;
            SelectedVendorType = null;
            searchText = "";
            AllVendors = VendorService.GetAll();
        }

        private List<string> AvailableVendorTypes => AllVendors
            .Select(v => v.VendorType)
            .Where(t => !string.IsNullOrEmpty(t))
            .Distinct()
            .OrderBy(t => t)
            .ToList();

        private List<string> AvailableStatuses => AllVendors
            .Select(v => v.VendorStatus)
            .Where(s => !string.IsNullOrEmpty(s))
            .Distinct()
            .OrderBy(s => s)
            .ToList();

        private List<VendorViewModel> FilteredVendors
        {
            get
            {
                var result = AllVendors.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    var search = searchText.ToLower();
                    result = result.Where(v =>
                        v.VendorCode.ToLower().Contains(search) ||
                        v.VendorName.ToLower().Contains(search) ||
                        (v.LegalName?.ToLower().Contains(search) ?? false) ||
                        (v.GSTIN?.ToLower().Contains(search) ?? false) ||
                        (v.PAN?.ToLower().Contains(search) ?? false) ||
                        (v.PrimaryPhone?.ToLower().Contains(search) ?? false) ||
                        (v.PrimaryEmail?.ToLower().Contains(search) ?? false));
                }

                if (!string.IsNullOrWhiteSpace(SelectedVendorType))
                {
                    result = result.Where(v => v.VendorType == SelectedVendorType);
                }

                if (!string.IsNullOrWhiteSpace(SelectedStatus))
                {
                    result = result.Where(v => v.VendorStatus == SelectedStatus);
                }

                return result.OrderByDescending(v => v.UpdatedAt ?? v.CreatedAt).ToList();
            }
        }

        private List<VendorViewModel> PagedVendors
        {
            get
            {
                return FilteredVendors
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();
            }
        }

        private int TotalPages => Math.Max(1, (int)Math.Ceiling((double)FilteredVendors.Count / PageSize));

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
            if (int.TryParse(e.Value?.ToString(), out int size))
            {
                PageSize = size;
                CurrentPage = 1;
            }
        }

        private async Task OnSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString() ?? "";
            CurrentPage = 1;
            VisibleColumnCount =
   await JS.InvokeAsync<int>("getVisibleTableColumns");
        }

        private async Task OnRefreshAsync()
        {
            isLoading = true;
            StateHasChanged();

            await Task.Delay(300);
            VendorService.ResetToSeed();
            LoadVendors();

            isLoading = false;
            await JS.InvokeVoidAsync("feather.replace");
            ToastService.ShowSuccess("Vendor list refreshed", "Refreshed");
        }

        private void OpenRowDetails(VendorViewModel vendor)
        {
            SelectedVendor = vendor;
        }

        private void OpenHoldModal(VendorViewModel vendor)
        {
            SelectedVendor = vendor;
            holdReasonInput = "";
        }

        private void ConfirmActivate(VendorViewModel vendor)
        {
            SelectedVendor = vendor;
        }

        private void ConfirmDeactivate(VendorViewModel vendor)
        {
            SelectedVendor = vendor;
            // Check if vendor can be deactivated
            canDeactivate = true; // In real app, check for active transactions
        }

        private void ConfirmReleaseHold(VendorViewModel vendor)
        {
            SelectedVendor = vendor;
        }

        private void ConfirmDelete(VendorViewModel vendor)
        {
            SelectedVendor = vendor;
            // Check if vendor can be deleted
            canDelete = vendor.VendorStatus == VendorStatuses.Draft; // Only draft vendors can be deleted
        }

        private async Task ActivateConfirmed()
        {
            if (SelectedVendor == null) return;

            var result = VendorService.Activate(SelectedVendor.Id, AuthService.CurrentUser?.UserName ?? "System");
            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message, "Activated");
                LoadVendors();
                await JS.InvokeVoidAsync("feather.replace");
            }
            else
            {
                ToastService.ShowError(result.Message, "Error");
            }
        }

        private async Task DeactivateConfirmed()
        {
            if (SelectedVendor == null) return;

            var result = VendorService.Deactivate(SelectedVendor.Id, AuthService.CurrentUser?.UserName ?? "System");
            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message, "Deactivated");
                LoadVendors();
                await JS.InvokeVoidAsync("feather.replace");
            }
            else
            {
                ToastService.ShowError(result.Message, "Error");
            }
        }

        private async Task PlaceHoldConfirmed()
        {
            if (SelectedVendor == null) return;

            if (string.IsNullOrWhiteSpace(holdReasonInput))
            {
                ToastService.ShowError("Please enter a reason for the hold", "Validation Error");
                return;
            }

            var result = VendorService.PlaceHold(
                SelectedVendor.Id,
                holdReasonInput,
                AuthService.CurrentUser?.UserName ?? "System"
            );

            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message, "Hold Placed");
                LoadVendors();
                await JS.InvokeVoidAsync("feather.replace");
            }
            else
            {
                ToastService.ShowError(result.Message, "Error");
            }
        }

        private async Task ReleaseHoldConfirmed()
        {
            if (SelectedVendor == null) return;

            var result = VendorService.ReleaseHold(SelectedVendor.Id, AuthService.CurrentUser?.UserName ?? "System");
            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message, "Hold Released");
                LoadVendors();
                await JS.InvokeVoidAsync("feather.replace");
            }
            else
            {
                ToastService.ShowError(result.Message, "Error");
            }
        }

        private async Task DeleteConfirmed()
        {
            if (SelectedVendor == null) return;

            var result = VendorService.Delete(SelectedVendor.Id);
            if (result.Success)
            {
                ToastService.ShowSuccess(result.Message, "Deleted");
                LoadVendors();
                await JS.InvokeVoidAsync("feather.replace");
            }
            else
            {
                ToastService.ShowError(result.Message, "Error");
            }
        }

        private static string GetStatusDotBadgeClass(string? status) => status switch
        {
            VendorStatuses.Active => "bg-success text-success",
            VendorStatuses.Inactive => "bg-secondary text-secondary",
            VendorStatuses.OnHold => "bg-warning text-warning",
            VendorStatuses.Blacklisted => "bg-danger text-danger",
            VendorStatuses.Draft => "bg-info text-info",
            _ => "bg-secondary text-secondary"
        };

        private static string GetStatusBadgeClass(string? status) => status switch
        {
            VendorStatuses.Active => "bg-success-transparent text-success",
            VendorStatuses.Inactive => "bg-secondary-transparent text-secondary",
            VendorStatuses.OnHold => "bg-warning-transparent text-warning",
            VendorStatuses.Blacklisted => "bg-danger-transparent text-danger",
            VendorStatuses.Draft => "bg-info-transparent text-info",
            _ => "bg-secondary-transparent text-secondary"
        };

        private static string GetTypeBadgeClass(string type) => type switch
        {
            VendorTypes.Supplier => "bg-primary-transparent text-primary",
            VendorTypes.ServiceProvider => "bg-info-transparent text-info",
            VendorTypes.Contractor => "bg-warning-transparent text-warning",
            VendorTypes.Freelancer => "bg-success-transparent text-success",
            VendorTypes.Utility => "bg-secondary-transparent text-secondary",
            VendorTypes.Landlord => "bg-purple-transparent text-purple",
            VendorTypes.Government => "bg-danger-transparent text-danger",
            _ => "bg-secondary-transparent text-secondary"
        };
    }
}
