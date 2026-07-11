using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace FinanceConnect.Api.Services;

public interface IEmailService
{
    Task SendRegistrationSuccessEmailAsync(string toEmail, string userName);
    Task SendPasswordChangedEmailAsync(string toEmail, string userName);
    Task SendForgotPasswordEmailAsync(string toEmail, string userName, string resetToken);
}

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ClientSettings _clientSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> emailSettings, IOptions<ClientSettings> clientSettings, ILogger<EmailService> logger)
    {
        _emailSettings = emailSettings.Value;
        _clientSettings = clientSettings.Value;
        _logger = logger;
    }

    public async Task SendRegistrationSuccessEmailAsync(string toEmail, string userName)
    {
        var loginUrl = $"{_clientSettings.BaseUrl}/Login";
        var subject = "Welcome to FinanceConnect - Registration Successful!";
        var body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #FF9F43 0%, #FF6B6B 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .button {{ display: inline-block; background: #FF9F43; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; margin-top: 20px; }}
        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🎉 Welcome to FinanceConnect!</h1>
        </div>
        <div class='content'>
            <h2>Hello {userName},</h2>
            <p>Congratulations! Your account has been successfully created.</p>
            <p>You can now log in to your account and start managing your finances with ease.</p>
            <p>Thank you for choosing us!</p>
            <a href='{loginUrl}' class='button'>Login to Your Account</a>
        </div>
        <div class='footer'>
            <p>© 2025 FinanceConnect. All rights reserved.</p>
            <p>This is an automated message, please do not reply.</p>
        </div>
    </div>
</body>
</html>";

        await SendEmailAsync(toEmail, subject, body);
    }

    public async Task SendPasswordChangedEmailAsync(string toEmail, string userName)
    {
        var subject = "FinanceConnect - Password Changed Successfully";
        var body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #28a745 0%, #20c997 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .alert {{ background: #fff3cd; border: 1px solid #ffc107; padding: 15px; border-radius: 5px; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🔐 Password Changed</h1>
        </div>
        <div class='content'>
            <h2>Hello {userName},</h2>
            <p>Your password has been successfully changed.</p>
            <p>If you made this change, you can safely ignore this email.</p>
            <div class='alert'>
                <strong>⚠️ Security Notice:</strong> If you did not make this change, please contact our support team immediately and secure your account.
            </div>
            <p>For your security, we recommend:</p>
            <ul>
                <li>Using a strong, unique password</li>
                <li>Never sharing your password with anyone</li>
                <li>Logging out from shared devices</li>
            </ul>
        </div>
        <div class='footer'>
            <p>© 2026 FinanceConnect. All rights reserved.</p>
            <p>This is an automated message, please do not reply.</p>
        </div>
    </div>
</body>
</html>";

        await SendEmailAsync(toEmail, subject, body);
    }

    public async Task SendForgotPasswordEmailAsync(string toEmail, string userName, string resetToken)
    {
        var subject = "FinanceConnect - Password Reset Request";
        var body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #FF9F43 0%, #FF6B6B 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .token-box {{ background: #fff; border: 2px dashed #FF9F43; padding: 20px; text-align: center; margin: 20px 0; border-radius: 5px; }}
        .token {{ font-size: 24px; font-weight: bold; color: #FF9F43; letter-spacing: 3px; }}
        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🔑 Password Reset</h1>
        </div>
        <div class='content'>
            <h2>Hello {userName},</h2>
            <p>We received a request to reset your password. Use the code below to reset your password:</p>
            <div class='token-box'>
                <p>Your Reset Code:</p>
                <p class='token'>{resetToken}</p>
            </div>
            <p><strong>This code will expire in 1 hour.</strong></p>
            <p>If you didn't request a password reset, please ignore this email or contact support if you have concerns.</p>
        </div>
        <div class='footer'>
            <p>© 2025 FinanceConnect. All rights reserved.</p>
            <p>This is an automated message, please do not reply.</p>
        </div>
    </div>
</body>
</html>";

        await SendEmailAsync(toEmail, subject, body);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            message.To.Add(new MailboxAddress(toEmail, toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlBody
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email sent successfully to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
            throw;
        }
    }
}
