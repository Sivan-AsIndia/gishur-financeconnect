using System.Net.Http.Json;
using Blazored.LocalStorage;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private UserSession? _currentUser;
    private const string UserSessionKey = "userSession";

    public event Action? OnAuthStateChanged;

    public AuthService(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    public UserSession? CurrentUser => _currentUser;
    public bool IsAuthenticated => _currentUser?.IsAuthenticated ?? false;

    public async Task InitializeAsync()
    {
        // Already loaded — skip to avoid re-triggering auth state change events
        if (_currentUser != null)
            return;

        try
        {
            var savedSession = await _localStorage.GetItemAsync<UserSession>(UserSessionKey);
            if (savedSession != null && savedSession.IsAuthenticated)
            {
                _currentUser = savedSession;
                OnAuthStateChanged?.Invoke();
            }
        }
        catch
        {
            // Ignore errors during initialization
        }
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, bool rememberMe = false)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            
            if (result != null && result.Success)
            {
                _currentUser = new UserSession
                {
                    UserId = result.UserId ?? 0,
                    UserName = result.UserName ?? string.Empty,
                    Email = result.Email ?? string.Empty,
                    IsAuthenticated = true,
                    ProfilePhoto = result.ProfilePhoto,
                    PhoneNumber = result.PhoneNumber
                };

                await _localStorage.SetItemAsync(UserSessionKey, _currentUser);
                OnAuthStateChanged?.Invoke();
            }
            
            return result ?? new LoginResponse { Success = false, Message = "Failed to process response" };
        }
        catch (Exception ex)
        {
            return new LoginResponse { Success = false, Message = $"Connection error: {ex.Message}" };
        }
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);
            var result = await response.Content.ReadFromJsonAsync<RegisterResponse>();
            return result ?? new RegisterResponse { Success = false, Message = "Failed to process response" };
        }
        catch (Exception ex)
        {
            return new RegisterResponse { Success = false, Message = $"Connection error: {ex.Message}" };
        }
    }

    public async Task<ChangePasswordResponse> ChangePasswordAsync(ChangePasswordRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/change-password", request);
            var result = await response.Content.ReadFromJsonAsync<ChangePasswordResponse>();
            return result ?? new ChangePasswordResponse { Success = false, Message = "Failed to process response" };
        }
        catch (Exception ex)
        {
            return new ChangePasswordResponse { Success = false, Message = $"Connection error: {ex.Message}" };
        }
    }

    public async Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/forgot-password", request);
            var result = await response.Content.ReadFromJsonAsync<ForgotPasswordResponse>();
            return result ?? new ForgotPasswordResponse { Success = false, Message = "Failed to process response" };
        }
        catch (Exception ex)
        {
            return new ForgotPasswordResponse { Success = false, Message = $"Connection error: {ex.Message}" };
        }
    }

    public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/reset-password", request);
            var result = await response.Content.ReadFromJsonAsync<ResetPasswordResponse>();
            return result ?? new ResetPasswordResponse { Success = false, Message = "Failed to process response" };
        }
        catch (Exception ex)
        {
            return new ResetPasswordResponse { Success = false, Message = $"Connection error: {ex.Message}" };
        }
    }

    public async Task LogoutAsync()
    {
        _currentUser = null;
        await _localStorage.RemoveItemAsync(UserSessionKey);
        OnAuthStateChanged?.Invoke();
    }

    public void Logout()
    {
        _currentUser = null;
        _ = _localStorage.RemoveItemAsync(UserSessionKey);
        OnAuthStateChanged?.Invoke();
    }

    public async Task<UpdateProfileResponse> UpdateProfilePhotoAsync(string? base64Photo)
    {
        if (_currentUser == null) 
            return new UpdateProfileResponse { Success = false, Message = "Not authenticated" };

        try
        {
            var request = new UpdateProfileRequest
            {
                Email = _currentUser.Email,
                ProfilePhoto = base64Photo
            };

            var response = await _httpClient.PostAsJsonAsync("api/auth/upload-photo", request);
            var result = await response.Content.ReadFromJsonAsync<UpdateProfileResponse>();

            if (result != null && result.Success)
            {
                _currentUser.ProfilePhoto = base64Photo;
                await _localStorage.SetItemAsync(UserSessionKey, _currentUser);
                OnAuthStateChanged?.Invoke();
            }

            return result ?? new UpdateProfileResponse { Success = false, Message = "Failed to process response" };
        }
        catch (Exception ex)
        {
            return new UpdateProfileResponse { Success = false, Message = $"Connection error: {ex.Message}" };
        }
    }

    public async Task<UpdateProfileResponse> RemoveProfilePhotoAsync()
    {
        if (_currentUser == null)
            return new UpdateProfileResponse { Success = false, Message = "Not authenticated" };

        try
        {
            var request = new UpdateProfileRequest
            {
                Email = _currentUser.Email,
                ProfilePhoto = null
            };

            var response = await _httpClient.PostAsJsonAsync("api/auth/remove-photo", request);
            var result = await response.Content.ReadFromJsonAsync<UpdateProfileResponse>();

            if (result != null && result.Success)
            {
                _currentUser.ProfilePhoto = null;
                await _localStorage.SetItemAsync(UserSessionKey, _currentUser);
                OnAuthStateChanged?.Invoke();
            }

            return result ?? new UpdateProfileResponse { Success = false, Message = "Failed to process response" };
        }
        catch (Exception ex)
        {
            return new UpdateProfileResponse { Success = false, Message = $"Connection error: {ex.Message}" };
        }
    }

    public async Task<UpdateProfileResponse> UpdateProfileAsync(UpdateProfileRequest request)
    {
        if (_currentUser == null)
            return new UpdateProfileResponse { Success = false, Message = "Not authenticated" };

        try
        {
            request.Email = _currentUser.Email;
            
            var response = await _httpClient.PostAsJsonAsync("api/auth/update-profile", request);
            var result = await response.Content.ReadFromJsonAsync<UpdateProfileResponse>();

            if (result != null && result.Success)
            {
                _currentUser.UserName = request.FullName;
                _currentUser.PhoneNumber = request.PhoneNumber;
                if (request.ProfilePhoto != null)
                {
                    _currentUser.ProfilePhoto = request.ProfilePhoto;
                }
                
                await _localStorage.SetItemAsync(UserSessionKey, _currentUser);
                OnAuthStateChanged?.Invoke();
            }

            return result ?? new UpdateProfileResponse { Success = false, Message = "Failed to process response" };
        }
        catch (Exception ex)
        {
            return new UpdateProfileResponse { Success = false, Message = $"Connection error: {ex.Message}" };
        }
    }

    public string? GetProfilePhoto()
    {
        return _currentUser?.ProfilePhoto;
    }
}
