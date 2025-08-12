using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceAPI.Models.DTOs.Users;

public class UserProfileDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Occupation { get; set; }
    public string Currency { get; set; } = string.Empty;
    public bool IsEmailVerified { get; set; }
    public bool IsPhoneVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; }
}

public class UpdateUserProfileRequest
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string LastName { get; set; } = string.Empty;

    [Phone]
    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    [StringLength(100)]
    public string? Occupation { get; set; }

    [Required]
    [StringLength(3, MinimumLength = 3)]
    [RegularExpression("^[A-Z]{3}$", ErrorMessage = "Currency must be a valid 3-letter currency code")]
    public string Currency { get; set; } = "INR";
}

public class UserPreferencesDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal ExpenseThreshold { get; set; }
    public decimal SavingsGoalMonthly { get; set; }
    public string InvestmentRiskLevel { get; set; } = string.Empty;
    public bool NotificationEmail { get; set; }
    public bool NotificationSms { get; set; }
    public bool NotificationPush { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public string Timezone { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class UpdateUserPreferencesRequest
{
    [Range(0, double.MaxValue)]
    public decimal? ExpenseThreshold { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? SavingsGoalMonthly { get; set; }

    [RegularExpression("^(LOW|MODERATE|HIGH)$")]
    public string? InvestmentRiskLevel { get; set; }

    public bool? NotificationEmail { get; set; }
    public bool? NotificationSms { get; set; }
    public bool? NotificationPush { get; set; }

    [StringLength(3, MinimumLength = 3)]
    [RegularExpression("^[A-Z]{3}$")]
    public string? CurrencyCode { get; set; }

    [StringLength(50)]
    public string? Timezone { get; set; }
}

public class UpdateSavingsThresholdsRequest
{
    [Required]
    [Range(0, double.MaxValue)]
    public decimal SavingsGoalMonthly { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? ExpenseThreshold { get; set; }
}
