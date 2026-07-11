using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanceConnect.Api.Data;
using FinanceConnect.Api.DTOs;
using FinanceConnect.Api.Models;
using FinanceConnect.Api.Services;

namespace FinanceConnect.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthDbContext _authDbContext;
    private readonly IEmailService _emailService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(AuthDbContext authDbContext, IEmailService emailService, ILogger<AuthController> logger)
    {
        _authDbContext = authDbContext;
        _emailService = emailService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return Ok(new LoginResponse
                {
                    Success = false,
                    Message = "Email and password are required."
                });
            }

            var user = await _authDbContext.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

            if (user == null)
            {
                return Ok(new LoginResponse
                {
                    Success = false,
                    Message = "Invalid email or password."
                });
            }

            if (user.Password != request.Password)
            {
                return Ok(new LoginResponse
                {
                    Success = false,
                    Message = "Invalid email or password."
                });
            }

            return Ok(new LoginResponse
            {
                Success = true,
                Message = "Login successful!",
                UserId = user.Id,
                UserName = user.Name,
                Email = user.Email,
                ProfilePhoto = user.ProfilePhoto,
                PhoneNumber = user.PhoneNumber
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return Ok(new LoginResponse
            {
                Success = false,
                Message = "An error occurred during login. Please try again."
            });
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            // Validation
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Ok(new RegisterResponse
                {
                    Success = false,
                    Message = "Name is required."
                });
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return Ok(new RegisterResponse
                {
                    Success = false,
                    Message = "Email is required."
                });
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return Ok(new RegisterResponse
                {
                    Success = false,
                    Message = "Password is required."
                });
            }

            if (request.Password.Length < 6)
            {
                return Ok(new RegisterResponse
                {
                    Success = false,
                    Message = "Password must be at least 6 characters long."
                });
            }

            if (request.Password != request.ConfirmPassword)
            {
                return Ok(new RegisterResponse
                {
                    Success = false,
                    Message = "Passwords do not match."
                });
            }

            // Check if email already exists
            var existingUser = await _authDbContext.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

            if (existingUser != null)
            {
                return Ok(new RegisterResponse
                {
                    Success = false,
                    Message = "An account with this email already exists."
                });
            }

            // Create new user with plain text password
            var user = new AppUser
            {
                Name = request.Name,
                Email = request.Email.ToLower(),
                Password = request.Password, // Plain text for demo
                CreatedAt = DateTime.UtcNow
            };

            _authDbContext.Users.Add(user);
            await _authDbContext.SaveChangesAsync();

            // Send registration success email
            try
            {
                await _emailService.SendRegistrationSuccessEmailAsync(user.Email, user.Name);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send registration email to {Email}", user.Email);
            }

            return Ok(new RegisterResponse
            {
                Success = true,
                Message = "Registration successful! A confirmation email has been sent to your email address."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            return Ok(new RegisterResponse
            {
                Success = false,
                Message = "An error occurred during registration. Please try again."
            });
        }
    }

    [HttpPost("change-password")]
    public async Task<ActionResult<ChangePasswordResponse>> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return Ok(new ChangePasswordResponse
                {
                    Success = false,
                    Message = "Email is required."
                });
            }

            if (string.IsNullOrWhiteSpace(request.CurrentPassword))
            {
                return Ok(new ChangePasswordResponse
                {
                    Success = false,
                    Message = "Current password is required."
                });
            }

            if (string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return Ok(new ChangePasswordResponse
                {
                    Success = false,
                    Message = "New password is required."
                });
            }

            if (request.NewPassword.Length < 6)
            {
                return Ok(new ChangePasswordResponse
                {
                    Success = false,
                    Message = "New password must be at least 6 characters long."
                });
            }

            if (request.NewPassword != request.ConfirmNewPassword)
            {
                return Ok(new ChangePasswordResponse
                {
                    Success = false,
                    Message = "New passwords do not match."
                });
            }

            var user = await _authDbContext.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

            if (user == null)
            {
                return Ok(new ChangePasswordResponse
                {
                    Success = false,
                    Message = "User not found."
                });
            }

            // Verify current password (plain text comparison)
            if (user.Password != request.CurrentPassword)
            {
                return Ok(new ChangePasswordResponse
                {
                    Success = false,
                    Message = "Current password is incorrect."
                });
            }

            // Update password
            user.Password = request.NewPassword;
            user.UpdatedAt = DateTime.UtcNow;
            await _authDbContext.SaveChangesAsync();

            // Send password changed email
            try
            {
                await _emailService.SendPasswordChangedEmailAsync(user.Email, user.Name);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send password changed email to {Email}", user.Email);
            }

            return Ok(new ChangePasswordResponse
            {
                Success = true,
                Message = "Password changed successfully!"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password change");
            return Ok(new ChangePasswordResponse
            {
                Success = false,
                Message = "An error occurred while changing password. Please try again."
            });
        }
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult<ForgotPasswordResponse>> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return Ok(new ForgotPasswordResponse
                {
                    Success = false,
                    Message = "Email is required."
                });
            }

            var user = await _authDbContext.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

            if (user == null)
            {
                // For security, don't reveal if user exists
                return Ok(new ForgotPasswordResponse
                {
                    Success = true,
                    Message = "If an account with this email exists, a password reset link has been sent."
                });
            }

            // Generate a 6-digit reset token
            var random = new Random();
            var resetToken = random.Next(100000, 999999).ToString();

            user.PasswordResetToken = resetToken;
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);
            await _authDbContext.SaveChangesAsync();

            // Send forgot password email
            try
            {
                await _emailService.SendForgotPasswordEmailAsync(user.Email, user.Name, resetToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send forgot password email to {Email}", user.Email);
                return Ok(new ForgotPasswordResponse
                {
                    Success = false,
                    Message = "Failed to send reset email. Please try again."
                });
            }

            return Ok(new ForgotPasswordResponse
            {
                Success = true,
                Message = "If an account with this email exists, a password reset code has been sent."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during forgot password");
            return Ok(new ForgotPasswordResponse
            {
                Success = false,
                Message = "An error occurred. Please try again."
            });
        }
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult<ResetPasswordResponse>> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Token))
            {
                return Ok(new ResetPasswordResponse
                {
                    Success = false,
                    Message = "Reset code is required."
                });
            }

            if (string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return Ok(new ResetPasswordResponse
                {
                    Success = false,
                    Message = "New password is required."
                });
            }

            if (request.NewPassword.Length < 6)
            {
                return Ok(new ResetPasswordResponse
                {
                    Success = false,
                    Message = "Password must be at least 6 characters long."
                });
            }

            if (request.NewPassword != request.ConfirmNewPassword)
            {
                return Ok(new ResetPasswordResponse
                {
                    Success = false,
                    Message = "Passwords do not match."
                });
            }

            var user = await _authDbContext.Users
                .FirstOrDefaultAsync(u => u.PasswordResetToken == request.Token && 
                                         u.PasswordResetTokenExpiry > DateTime.UtcNow);

            if (user == null)
            {
                return Ok(new ResetPasswordResponse
                {
                    Success = false,
                    Message = "Invalid or expired reset code."
                });
            }

            // Update password
            user.Password = request.NewPassword;
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;
            user.UpdatedAt = DateTime.UtcNow;
            await _authDbContext.SaveChangesAsync();

            // Send password changed email
            try
            {
                await _emailService.SendPasswordChangedEmailAsync(user.Email, user.Name);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send password changed email to {Email}", user.Email);
            }

            return Ok(new ResetPasswordResponse
            {
                Success = true,
                Message = "Password reset successfully! You can now login with your new password."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password reset");
            return Ok(new ResetPasswordResponse
            {
                Success = false,
                Message = "An error occurred. Please try again."
            });
        }
    }

    [HttpPost("update-profile")]
    public async Task<ActionResult<UpdateProfileResponse>> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return Ok(new UpdateProfileResponse
                {
                    Success = false,
                    Message = "Email is required."
                });
            }

            var user = await _authDbContext.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

            if (user == null)
            {
                return Ok(new UpdateProfileResponse
                {
                    Success = false,
                    Message = "User not found."
                });
            }

            // Update profile fields
            if (!string.IsNullOrWhiteSpace(request.FullName))
            {
                user.Name = request.FullName;
            }

            if (request.PhoneNumber != null)
            {
                user.PhoneNumber = request.PhoneNumber;
            }

            if (request.ProfilePhoto != null)
            {
                user.ProfilePhoto = request.ProfilePhoto;
            }

            user.UpdatedAt = DateTime.UtcNow;
            await _authDbContext.SaveChangesAsync();

            return Ok(new UpdateProfileResponse
            {
                Success = true,
                Message = "Profile updated successfully!",
                ProfilePhoto = user.ProfilePhoto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during profile update");
            return Ok(new UpdateProfileResponse
            {
                Success = false,
                Message = "An error occurred while updating profile. Please try again."
            });
        }
    }

    [HttpPost("upload-photo")]
    public async Task<ActionResult<UpdateProfileResponse>> UploadPhoto([FromBody] UpdateProfileRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return Ok(new UpdateProfileResponse
                {
                    Success = false,
                    Message = "Email is required."
                });
            }

            var user = await _authDbContext.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

            if (user == null)
            {
                return Ok(new UpdateProfileResponse
                {
                    Success = false,
                    Message = "User not found."
                });
            }

            user.ProfilePhoto = request.ProfilePhoto;
            user.UpdatedAt = DateTime.UtcNow;
            await _authDbContext.SaveChangesAsync();

            return Ok(new UpdateProfileResponse
            {
                Success = true,
                Message = "Profile photo updated successfully!",
                ProfilePhoto = user.ProfilePhoto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during photo upload");
            return Ok(new UpdateProfileResponse
            {
                Success = false,
                Message = "An error occurred while uploading photo. Please try again."
            });
        }
    }

    [HttpPost("remove-photo")]
    public async Task<ActionResult<UpdateProfileResponse>> RemovePhoto([FromBody] UpdateProfileRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return Ok(new UpdateProfileResponse
                {
                    Success = false,
                    Message = "Email is required."
                });
            }

            var user = await _authDbContext.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

            if (user == null)
            {
                return Ok(new UpdateProfileResponse
                {
                    Success = false,
                    Message = "User not found."
                });
            }

            user.ProfilePhoto = null;
            user.UpdatedAt = DateTime.UtcNow;
            await _authDbContext.SaveChangesAsync();

            return Ok(new UpdateProfileResponse
            {
                Success = true,
                Message = "Profile photo removed successfully!",
                ProfilePhoto = null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during photo removal");
            return Ok(new UpdateProfileResponse
            {
                Success = false,
                Message = "An error occurred while removing photo. Please try again."
            });
        }
    }
}
