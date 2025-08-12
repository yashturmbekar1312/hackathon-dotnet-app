using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using PersonalFinanceAPI.Core.Interfaces;
using PersonalFinanceAPI.Models.DTOs.Email;

namespace PersonalFinanceAPI.Application.Services;

/// <summary>
/// Email service implementation using SMTP
/// </summary>
public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    /// <summary>
    /// Sends an OTP verification email to the specified email address
    /// </summary>
    public async Task<bool> SendOtpEmailAsync(string email, string otpCode, string userName)
    {
        try
        {
            var subject = "Email Verification - Personal Finance API";
            var body = GenerateOtpEmailTemplate(userName, otpCode);

            var emailMessage = new EmailMessage
            {
                To = email,
                Subject = subject,
                Body = body,
                IsHtml = true
            };

            return await SendEmailAsync(emailMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send OTP email to {Email}", email);
            return false;
        }
    }

    /// <summary>
    /// Sends a welcome email to a new user
    /// </summary>
    public async Task<bool> SendWelcomeEmailAsync(string email, string userName)
    {
        try
        {
            var subject = "Welcome to Personal Finance API!";
            var body = GenerateWelcomeEmailTemplate(userName);

            var emailMessage = new EmailMessage
            {
                To = email,
                Subject = subject,
                Body = body,
                IsHtml = true
            };

            return await SendEmailAsync(emailMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send welcome email to {Email}", email);
            return false;
        }
    }

    /// <summary>
    /// Sends a password reset email
    /// </summary>
    public async Task<bool> SendPasswordResetEmailAsync(string email, string resetToken, string userName)
    {
        try
        {
            var subject = "Password Reset - Personal Finance API";
            var body = GeneratePasswordResetEmailTemplate(userName, resetToken);

            var emailMessage = new EmailMessage
            {
                To = email,
                Subject = subject,
                Body = body,
                IsHtml = true
            };

            return await SendEmailAsync(emailMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send password reset email to {Email}", email);
            return false;
        }
    }

    /// <summary>
    /// Sends a generic notification email
    /// </summary>
    public async Task<bool> SendNotificationEmailAsync(string email, string subject, string message, bool isHtml = false)
    {
        try
        {
            var emailMessage = new EmailMessage
            {
                To = email,
                Subject = subject,
                Body = message,
                IsHtml = isHtml
            };

            return await SendEmailAsync(emailMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification email to {Email}", email);
            return false;
        }
    }

    /// <summary>
    /// Core email sending method
    /// </summary>
    private async Task<bool> SendEmailAsync(EmailMessage emailMessage)
    {
        try
        {
            // Check if email settings are configured
            if (string.IsNullOrEmpty(_emailSettings.SmtpHost) || 
                string.IsNullOrEmpty(_emailSettings.Username) || 
                string.IsNullOrEmpty(_emailSettings.FromEmail))
            {
                _logger.LogWarning("Email settings not configured. Simulating email send for development.");
                
                // For development/hackathon - log the email instead of sending
                _logger.LogInformation("📧 EMAIL SIMULATION 📧");
                _logger.LogInformation("To: {To}", emailMessage.To);
                _logger.LogInformation("Subject: {Subject}", emailMessage.Subject);
                _logger.LogInformation("Body: {Body}", emailMessage.Body);
                _logger.LogInformation("📧 END EMAIL SIMULATION 📧");
                
                return true; // Return true for development
            }

            using var client = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort);
            client.EnableSsl = _emailSettings.EnableSsl;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);
            client.Timeout = _emailSettings.TimeoutSeconds * 1000;

            using var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName);
            mailMessage.To.Add(emailMessage.To);
            mailMessage.Subject = emailMessage.Subject;
            mailMessage.Body = emailMessage.Body;
            mailMessage.IsBodyHtml = emailMessage.IsHtml;

            // Add CC recipients
            foreach (var cc in emailMessage.Cc)
            {
                mailMessage.CC.Add(cc);
            }

            // Add BCC recipients
            foreach (var bcc in emailMessage.Bcc)
            {
                mailMessage.Bcc.Add(bcc);
            }

            await client.SendMailAsync(mailMessage);
            _logger.LogInformation("Email sent successfully to {Email}", emailMessage.To);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", emailMessage.To);
            return false;
        }
    }

    /// <summary>
    /// Generates OTP email template
    /// </summary>
    private string GenerateOtpEmailTemplate(string userName, string otpCode)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 20px; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        .header {{ text-align: center; color: #333; margin-bottom: 30px; }}
        .otp-code {{ background-color: #f8f9fa; border: 2px solid #007bff; border-radius: 8px; padding: 20px; text-align: center; margin: 20px 0; }}
        .otp-number {{ font-size: 32px; font-weight: bold; color: #007bff; letter-spacing: 5px; }}
        .footer {{ text-align: center; color: #666; font-size: 14px; margin-top: 30px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Email Verification</h1>
            <h2>Personal Finance API</h2>
        </div>
        
        <p>Hello {userName},</p>
        
        <p>Thank you for registering with Personal Finance API! To complete your registration, please use the following verification code:</p>
        
        <div class='otp-code'>
            <div class='otp-number'>{otpCode}</div>
            <p><strong>This code will expire in 10 minutes</strong></p>
        </div>
        
        <p>If you didn't create an account with us, please ignore this email.</p>
        
        <div class='footer'>
            <p>Best regards,<br>Personal Finance API Team</p>
            <p><small>This is an automated message, please do not reply to this email.</small></p>
        </div>
    </div>
</body>
</html>";
    }

    /// <summary>
    /// Generates welcome email template
    /// </summary>
    private string GenerateWelcomeEmailTemplate(string userName)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 20px; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        .header {{ text-align: center; color: #333; margin-bottom: 30px; }}
        .welcome-box {{ background-color: #e8f5e8; border-radius: 8px; padding: 20px; margin: 20px 0; }}
        .footer {{ text-align: center; color: #666; font-size: 14px; margin-top: 30px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Welcome to Personal Finance API!</h1>
        </div>
        
        <div class='welcome-box'>
            <h2>Hello {userName}! 🎉</h2>
            <p>Your account has been successfully verified and activated!</p>
        </div>
        
        <p>You can now start using all the features of our Personal Finance API:</p>
        <ul>
            <li>Track your transactions</li>
            <li>Manage budgets</li>
            <li>Set financial goals</li>
            <li>Generate reports and analytics</li>
            <li>Link bank accounts</li>
        </ul>
        
        <p>If you have any questions or need assistance, feel free to contact our support team.</p>
        
        <div class='footer'>
            <p>Best regards,<br>Personal Finance API Team</p>
        </div>
    </div>
</body>
</html>";
    }

    /// <summary>
    /// Generates password reset email template
    /// </summary>
    private string GeneratePasswordResetEmailTemplate(string userName, string resetToken)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 20px; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        .header {{ text-align: center; color: #333; margin-bottom: 30px; }}
        .reset-box {{ background-color: #fff3cd; border: 1px solid #ffeaa7; border-radius: 8px; padding: 20px; margin: 20px 0; }}
        .reset-token {{ background-color: #f8f9fa; border: 1px solid #dee2e6; border-radius: 4px; padding: 10px; font-family: monospace; word-break: break-all; }}
        .footer {{ text-align: center; color: #666; font-size: 14px; margin-top: 30px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Password Reset Request</h1>
            <h2>Personal Finance API</h2>
        </div>
        
        <p>Hello {userName},</p>
        
        <p>We received a request to reset your password. Use the following reset token:</p>
        
        <div class='reset-box'>
            <p><strong>Reset Token:</strong></p>
            <div class='reset-token'>{resetToken}</div>
            <p><small>This token will expire in 1 hour for security reasons.</small></p>
        </div>
        
        <p>If you didn't request a password reset, please ignore this email or contact support if you have concerns.</p>
        
        <div class='footer'>
            <p>Best regards,<br>Personal Finance API Team</p>
            <p><small>This is an automated message, please do not reply to this email.</small></p>
        </div>
    </div>
</body>
</html>";
    }
}
