using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceAPI.Models.DTOs.Email;

/// <summary>
/// Email configuration settings
/// </summary>
public class EmailSettings
{
    /// <summary>
    /// SMTP server host
    /// </summary>
    public string SmtpHost { get; set; } = string.Empty;

    /// <summary>
    /// SMTP server port
    /// </summary>
    public int SmtpPort { get; set; } = 587;

    /// <summary>
    /// Enable SSL for SMTP connection
    /// </summary>
    public bool EnableSsl { get; set; } = true;

    /// <summary>
    /// SMTP username/email
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// SMTP password
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// From email address
    /// </summary>
    public string FromEmail { get; set; } = string.Empty;

    /// <summary>
    /// From display name
    /// </summary>
    public string FromName { get; set; } = "Personal Finance API";

    /// <summary>
    /// Email sending timeout in seconds
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;
}

/// <summary>
/// Email message model
/// </summary>
public class EmailMessage
{
    /// <summary>
    /// Recipient email address
    /// </summary>
    [Required]
    [EmailAddress]
    public string To { get; set; } = string.Empty;

    /// <summary>
    /// Email subject
    /// </summary>
    [Required]
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Email body content
    /// </summary>
    [Required]
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// Whether the body is HTML formatted
    /// </summary>
    public bool IsHtml { get; set; } = false;

    /// <summary>
    /// Optional CC recipients
    /// </summary>
    public List<string> Cc { get; set; } = new();

    /// <summary>
    /// Optional BCC recipients
    /// </summary>
    public List<string> Bcc { get; set; } = new();
}
