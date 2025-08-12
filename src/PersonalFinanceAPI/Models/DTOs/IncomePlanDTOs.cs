using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceAPI.Models.DTOs
{
    /// <summary>
    /// DTO for creating a new income plan
    /// </summary>
    public class CreateIncomePlanDto
    {
        /// <summary>
        /// Gets or sets the name of the income plan
        /// </summary>
        [Required(ErrorMessage = "Income plan name is required")]
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the income plan
        /// </summary>
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the target amount for this income plan
        /// </summary>
        [Required(ErrorMessage = "Target amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Target amount must be greater than 0")]
        public decimal TargetAmount { get; set; }

        /// <summary>
        /// Gets or sets the type of plan (MONTHLY, YEARLY, QUARTERLY)
        /// </summary>
        [Required(ErrorMessage = "Plan type is required")]
        [RegularExpression("^(MONTHLY|YEARLY|QUARTERLY)$", ErrorMessage = "Plan type must be MONTHLY, YEARLY, or QUARTERLY")]
        public string PlanType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the start date of the income plan
        /// </summary>
        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date of the income plan
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the priority level (1=Low, 3=Medium, 5=High)
        /// </summary>
        [Range(1, 5, ErrorMessage = "Priority must be between 1 and 5")]
        public int Priority { get; set; } = 3;

        /// <summary>
        /// Gets or sets additional notes
        /// </summary>
        [StringLength(2000, ErrorMessage = "Notes cannot exceed 2000 characters")]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// DTO for updating an existing income plan
    /// </summary>
    public class UpdateIncomePlanDto
    {
        /// <summary>
        /// Gets or sets the name of the income plan
        /// </summary>
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the income plan
        /// </summary>
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the target amount for this income plan
        /// </summary>
        [Range(0.01, double.MaxValue, ErrorMessage = "Target amount must be greater than 0")]
        public decimal? TargetAmount { get; set; }

        /// <summary>
        /// Gets or sets the type of plan (MONTHLY, YEARLY, QUARTERLY)
        /// </summary>
        [RegularExpression("^(MONTHLY|YEARLY|QUARTERLY)$", ErrorMessage = "Plan type must be MONTHLY, YEARLY, or QUARTERLY")]
        public string? PlanType { get; set; }

        /// <summary>
        /// Gets or sets the end date of the income plan
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the status (ACTIVE, COMPLETED, PAUSED, CANCELLED)
        /// </summary>
        [RegularExpression("^(ACTIVE|COMPLETED|PAUSED|CANCELLED)$", ErrorMessage = "Status must be ACTIVE, COMPLETED, PAUSED, or CANCELLED")]
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets the priority level (1=Low, 3=Medium, 5=High)
        /// </summary>
        [Range(1, 5, ErrorMessage = "Priority must be between 1 and 5")]
        public int? Priority { get; set; }

        /// <summary>
        /// Gets or sets additional notes
        /// </summary>
        [StringLength(2000, ErrorMessage = "Notes cannot exceed 2000 characters")]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// DTO for income plan response
    /// </summary>
    public class IncomePlanDto
    {
        /// <summary>
        /// Gets or sets the unique identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the user identifier
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the target amount
        /// </summary>
        public decimal TargetAmount { get; set; }

        /// <summary>
        /// Gets or sets the current amount
        /// </summary>
        public decimal CurrentAmount { get; set; }

        /// <summary>
        /// Gets or sets the plan type
        /// </summary>
        public string PlanType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the start date
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the status
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the priority
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets the notes
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Gets or sets the creation date
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the last update date
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the completion percentage
        /// </summary>
        public decimal CompletionPercentage { get; set; }

        /// <summary>
        /// Gets or sets the number of income sources
        /// </summary>
        public int IncomeSourcesCount { get; set; }

        /// <summary>
        /// Gets or sets the number of milestones
        /// </summary>
        public int MilestonesCount { get; set; }
    }

    /// <summary>
    /// DTO for creating an income source
    /// </summary>
    public class CreateIncomeSourceDto
    {
        /// <summary>
        /// Gets or sets the name of the income source
        /// </summary>
        [Required(ErrorMessage = "Income source name is required")]
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the source type
        /// </summary>
        [Required(ErrorMessage = "Source type is required")]
        [RegularExpression("^(SALARY|FREELANCE|INVESTMENT|BUSINESS|OTHER)$", ErrorMessage = "Source type must be SALARY, FREELANCE, INVESTMENT, BUSINESS, or OTHER")]
        public string SourceType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the expected amount
        /// </summary>
        [Required(ErrorMessage = "Expected amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Expected amount must be greater than 0")]
        public decimal ExpectedAmount { get; set; }

        /// <summary>
        /// Gets or sets the frequency
        /// </summary>
        [Required(ErrorMessage = "Frequency is required")]
        [RegularExpression("^(WEEKLY|MONTHLY|QUARTERLY|YEARLY|ONE_TIME)$", ErrorMessage = "Frequency must be WEEKLY, MONTHLY, QUARTERLY, YEARLY, or ONE_TIME")]
        public string Frequency { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the start date
        /// </summary>
        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date
        /// </summary>
        public DateTime? EndDate { get; set; }
    }

    /// <summary>
    /// DTO for income source response
    /// </summary>
    public class IncomeSourceDto
    {
        /// <summary>
        /// Gets or sets the unique identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the income plan identifier
        /// </summary>
        public int IncomePlanId { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the source type
        /// </summary>
        public string SourceType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the expected amount
        /// </summary>
        public decimal ExpectedAmount { get; set; }

        /// <summary>
        /// Gets or sets the actual amount
        /// </summary>
        public decimal ActualAmount { get; set; }

        /// <summary>
        /// Gets or sets the frequency
        /// </summary>
        public string Frequency { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether the source is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the start date
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the creation date
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the last update date
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO for creating an income entry
    /// </summary>
    public class CreateIncomeEntryDto
    {
        /// <summary>
        /// Gets or sets the amount received
        /// </summary>
        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the received date
        /// </summary>
        [Required(ErrorMessage = "Received date is required")]
        public DateTime ReceivedDate { get; set; }

        /// <summary>
        /// Gets or sets optional notes
        /// </summary>
        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
        public string? Notes { get; set; }

        /// <summary>
        /// Gets or sets the reference number
        /// </summary>
        [StringLength(100, ErrorMessage = "Reference number cannot exceed 100 characters")]
        public string? ReferenceNumber { get; set; }
    }

    /// <summary>
    /// DTO for income entry response
    /// </summary>
    public class IncomeEntryDto
    {
        /// <summary>
        /// Gets or sets the unique identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the income source identifier
        /// </summary>
        public int IncomeSourceId { get; set; }

        /// <summary>
        /// Gets or sets the amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the received date
        /// </summary>
        public DateTime ReceivedDate { get; set; }

        /// <summary>
        /// Gets or sets the notes
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Gets or sets the reference number
        /// </summary>
        public string? ReferenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the creation date
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the last update date
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
