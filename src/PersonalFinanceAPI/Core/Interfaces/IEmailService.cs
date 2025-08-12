using PersonalFinanceAPI.Models.DTOs.Email;

namespace PersonalFinanceAPI.Core.Interfaces;

/// <summary>
/// Interface for email service operations
/// </summary>
public interface IEmailService : IDisposable
{
    /// <summary>
    /// Sends an OTP verification email to the specified email address
    /// </summary>
    /// <param name="email">The recipient email address</param>
    /// <param name="otpCode">The OTP code to send</param>
    /// <param name="userName">The user's name for personalization</param>
    /// <returns>True if email was sent successfully, false otherwise</returns>
    Task<bool> SendOtpEmailAsync(string email, string otpCode, string userName);

    /// <summary>
    /// Sends a welcome email to a new user
    /// </summary>
    /// <param name="email">The recipient email address</param>
    /// <param name="userName">The user's name</param>
    /// <returns>True if email was sent successfully, false otherwise</returns>
    Task<bool> SendWelcomeEmailAsync(string email, string userName);

    /// <summary>
    /// Sends a password reset email
    /// </summary>
    /// <param name="email">The recipient email address</param>
    /// <param name="resetToken">The password reset token</param>
    /// <param name="userName">The user's name</param>
    /// <returns>True if email was sent successfully, false otherwise</returns>
    Task<bool> SendPasswordResetEmailAsync(string email, string resetToken, string userName);

    /// <summary>
    /// Sends a generic notification email
    /// </summary>
    /// <param name="email">The recipient email address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="message">Email message content</param>
    /// <param name="isHtml">Whether the message is HTML formatted</param>
    /// <returns>True if email was sent successfully, false otherwise</returns>
    Task<bool> SendNotificationEmailAsync(string email, string subject, string message, bool isHtml = false);
}
