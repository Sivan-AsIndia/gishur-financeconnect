namespace FinanceConnect.Api.Models;

public class AppUser
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty; // Plain text for demo
    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiry { get; set; }
    public string? ProfilePhoto { get; set; } // Base64 encoded image
    public string? PhoneNumber { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;  // Nullable to handle existing NULL rows in SQLite
    public DateTime? UpdatedAt { get; set; }
}
