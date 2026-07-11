using FinanceConnect.Client.Services;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using static FinanceConnect.Client.Data.MasterDataIds;

namespace FinanceConnect.Client.Pages.Settings
{
    public partial class Setting
    {

        [Inject] DashboardService DashboardService { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] BranchService BranchService { get; set; } = default!;
        [Inject] SettingsService SettingsService { get; set; } = default!;

        [Inject] IJSRuntime JS { get; set; } = default!;

        List<CompanyModel> Companies = new();
        List<BranchModel> Branches = new();
        List<BranchModel> FilteredBranches = new();
        string SelectedBranchName = string.Empty;
        Guid? selectedCompanyId;
        Guid? SelectedBranchId;
        Guid? previousCompanyId;


        Guid? SelectedCompanyId
        {
            get => selectedCompanyId;
            set
            {
                selectedCompanyId = value;
            }
        }
        int ActiveTab = 1;
        private ChangePasswordRequest changeModel = new();
        private UpdateProfileRequest profileModel = new();
        private bool isLoading = false;
        private bool isInitialized = false;
        private bool showCurrentPassword = false;
        private bool showNewPassword = false;
        private bool showConfirmPassword = false;

        // Profile photo related
        private string? profilePhotoBase64;
        private bool isUploadingPhoto = false;
        private bool isUpdatingProfile = false;
        private const long MaxFileSize = 2 * 1024 * 1024; // 2MB

        protected override void OnInitialized()
        {
            Navigation.LocationChanged += OnLocationChanged;
            AuthService.OnAuthStateChanged += OnAuthStateChanged;
            ReadTabFromUrl();
        }

        private void OnAuthStateChanged()
        {
            // Update profile photo when auth state changes
            profilePhotoBase64 = AuthService.CurrentUser?.ProfilePhoto;
            InvokeAsync(StateHasChanged);
        }

        void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            ReadTabFromUrl();
            InvokeAsync(StateHasChanged);
        }

        void ReadTabFromUrl()
        {
            var uri = new Uri(Navigation.Uri);

            if (!string.IsNullOrEmpty(uri.Query))
            {
                var queryParams = uri.Query.TrimStart('?').Split('&');

                foreach (var param in queryParams)
                {
                    var parts = param.Split('=');

                    if (parts.Length == 2 && parts[0] == "tab")
                    {
                        if (int.TryParse(parts[1], out int tab))
                        {
                            ActiveTab = tab;
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            Navigation.LocationChanged -= OnLocationChanged;
            AuthService.OnAuthStateChanged -= OnAuthStateChanged;
        }

        protected override async Task OnInitializedAsync()
        {
            if (AuthService.CurrentUser != null)
            {
                changeModel.Email = AuthService.CurrentUser.Email;

                // Initialize profile model with current user data
                profileModel.FullName = AuthService.CurrentUser.UserName;
                profileModel.Email = AuthService.CurrentUser.Email;
                profileModel.Username = AuthService.CurrentUser.UserName;
                profileModel.PhoneNumber = AuthService.CurrentUser.PhoneNumber ?? "";
                profilePhotoBase64 = AuthService.CurrentUser.ProfilePhoto;
            }
            LoadCompanies();
            isInitialized = true;
        }

        private async Task HandlePhotoUpload(InputFileChangeEventArgs e)
        {
            var file = e.File;

            if (file == null)
                return;

            // Validate file type
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
            {
                ToastService.ShowError("Please upload a valid image file (JPG, PNG, GIF, or WebP).", "Invalid File Type");
                return;
            }

            // Validate file size
            if (file.Size > MaxFileSize)
            {
                ToastService.ShowError("Image size must be less than 2MB.", "File Too Large");
                return;
            }

            isUploadingPhoto = true;
            StateHasChanged();

            try
            {
                // Read the file as bytes
                using var stream = file.OpenReadStream(MaxFileSize);
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                var bytes = memoryStream.ToArray();

                // Convert to base64
                var base64 = Convert.ToBase64String(bytes);
                var photoData = $"data:{file.ContentType};base64,{base64}";

                // Save to API and update local state
                var result = await AuthService.UpdateProfilePhotoAsync(photoData);

                if (result.Success)
                {
                    profilePhotoBase64 = photoData;
                    ToastService.ShowSuccess("Profile photo uploaded successfully!", "Success");
                }
                else
                {
                    ToastService.ShowError(result.Message, "Upload Error");
                }
            }
            catch (Exception ex)
            {
                ToastService.ShowError($"Failed to upload photo: {ex.Message}", "Upload Error");
            }
            finally
            {
                isUploadingPhoto = false;
                StateHasChanged();
            }
        }

        private async Task RemovePhoto()
        {
            isUploadingPhoto = true;
            StateHasChanged();

            try
            {
                var result = await AuthService.RemoveProfilePhotoAsync();

                if (result.Success)
                {
                    profilePhotoBase64 = null;
                    ToastService.ShowSuccess("Profile photo removed.", "Success");
                }
                else
                {
                    ToastService.ShowError(result.Message, "Error");
                }
            }
            catch (Exception ex)
            {
                ToastService.ShowError($"Failed to remove photo: {ex.Message}", "Error");
            }
            finally
            {
                isUploadingPhoto = false;
                StateHasChanged();
            }
        }

        private async Task HandleUpdateProfile()
        {
            if (string.IsNullOrWhiteSpace(profileModel.FullName))
            {
                ToastService.ShowError("Full name is required.", "Validation Error");
                return;
            }

            isUpdatingProfile = true;

            try
            {
                profileModel.ProfilePhoto = profilePhotoBase64;
                var result = await AuthService.UpdateProfileAsync(profileModel);

                if (result.Success)
                {
                    ToastService.ShowSuccess("Profile updated successfully!", "Success");
                }
                else
                {
                    ToastService.ShowError(result.Message, "Error");
                }
            }
            catch (Exception ex)
            {
                ToastService.ShowError($"An error occurred: {ex.Message}", "Error");
            }
            finally
            {
                isUpdatingProfile = false;
            }
        }

        private void ToggleCurrentPasswordVisibility()
        {
            showCurrentPassword = !showCurrentPassword;
        }

        private void ToggleNewPasswordVisibility()
        {
            showNewPassword = !showNewPassword;
        }

        private void ToggleConfirmPasswordVisibility()
        {
            showConfirmPassword = !showConfirmPassword;
        }

        private async Task HandleChangePassword()
        {
            if (string.IsNullOrWhiteSpace(changeModel.Email))
            {
                ToastService.ShowError("Email is required.", "Validation Error");
                return;
            }

            if (string.IsNullOrWhiteSpace(changeModel.CurrentPassword))
            {
                ToastService.ShowError("Current password is required.", "Validation Error");
                return;
            }

            if (string.IsNullOrWhiteSpace(changeModel.NewPassword))
            {
                ToastService.ShowError("New password is required.", "Validation Error");
                return;
            }

            if (changeModel.NewPassword.Length < 6)
            {
                ToastService.ShowError("New password must be at least 6 characters long.", "Validation Error");
                return;
            }

            if (changeModel.NewPassword != changeModel.ConfirmNewPassword)
            {
                ToastService.ShowError("New passwords do not match.", "Validation Error");
                return;
            }

            if (changeModel.CurrentPassword == changeModel.NewPassword)
            {
                ToastService.ShowWarning("New password must be different from current password.", "Validation Warning");
                return;
            }

            isLoading = true;
            try
            {
                var response = await AuthService.ChangePasswordAsync(changeModel);
                if (response.Success)
                {
                    ToastService.ShowSuccess(response.Message, "Password Changed");
                    changeModel = new ChangePasswordRequest { Email = changeModel.Email };
                    await Task.Delay(2000);
                    await AuthService.LogoutAsync();
                    Navigation.NavigateTo("/Login");
                }
                else
                {
                    ToastService.ShowError(response.Message, "Change Password Failed");
                }
            }
            catch (Exception ex)
            {
                ToastService.ShowError($"An error occurred: {ex.Message}", "Error");
            }
            finally
            {
                isLoading = false;
            }
        }


        void LoadCompanies()
        {
            Companies = MasterDataService
                .GetAllCompanies()
                .Where(c => c.Status == "Active")
                .ToList();
            SelectedCompanyId = Companies.FirstOrDefault()?.Id;
            previousCompanyId = SelectedCompanyId;
            if (SelectedCompanyId.HasValue)
            {
                Branches = BranchService
                    .GetByCompanyId(SelectedCompanyId.Value)
                    .Where(b => b.Status == "Active")
                    .ToList();
                FilteredBranches = Branches;

                var Branch = Branches
                .FirstOrDefault(b => b.IsDefaultBranch)
                ?? Branches.FirstOrDefault();
                SelectedBranchId = Branch?.Id;
                SelectedBranchName = Branch?.BranchName;
            }
        }

        private async Task OnCompanyChanged()
        {
            SelectedBranchId = null;
            SelectedBranchName = string.Empty;

            Branches.Clear();

            if (!SelectedCompanyId.HasValue)
                return;

            Branches = BranchService
                .GetByCompanyId(SelectedCompanyId.Value)
                .Where(b => b.Status == "Active")
                .ToList();

            FilteredBranches = Branches;

            var branch = Branches.FirstOrDefault(b => b.IsDefaultBranch);

            previousCompanyId = SelectedCompanyId;

            if (branch is not null)
            {
                SelectedBranchId = branch.Id;
                SelectedBranchName = branch.BranchName;
            }
            else
            {
                // fallback if somehow no default found
                var firstBranch = Branches.FirstOrDefault();
                SelectedBranchId = firstBranch?.Id;
                SelectedBranchName = firstBranch?.BranchName ?? string.Empty;
            }

            await DashboardService.SaveDashboardDataAsync(SelectedCompanyId, SelectedBranchId);
            SettingsService.SetWorkspace(
                SelectedCompanyId,
                SelectedBranchId,
                branch?.BranchName
            );
        }

        private async Task OnBranchChanged()
        {
            SelectedBranchName = Branches
                .FirstOrDefault(b => b.Id == SelectedBranchId)?
                .BranchName ?? string.Empty;
            previousCompanyId = SelectedCompanyId;
            await DashboardService.SaveDashboardDataAsync(SelectedCompanyId, SelectedBranchId);
            SettingsService.SetWorkspace(
                SelectedCompanyId,
                SelectedBranchId,
                SelectedBranchName
            );
        }


    }
}
