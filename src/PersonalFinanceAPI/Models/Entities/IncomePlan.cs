using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalFinanceAPI.Models.Entities
{
    /// <summary>
    /// Represents an income planning strategy for a user
    /// </summary>
    [Table("income_plans")]
    public class IncomePlan
    {
        /// <summary>
        /// Gets or sets the unique identifier for the income plan
        /// </summary>
        [Key]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the user identifier who owns this income plan
        /// </summary>
        [Column("user_id")]
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the name of the income plan
        /// </summary>
        [Column("name")]
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the income plan
        /// </summary>
        [Column("description")]
        [MaxLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the target amount for this income plan
        /// </summary>
        [Column("target_amount")]
        [Range(0, double.MaxValue)]
        public decimal TargetAmount { get; set; }

        /// <summary>
        /// Gets or sets the current amount achieved in this income plan
        /// </summary>
        [Column("current_amount")]
        [Range(0, double.MaxValue)]
        public decimal CurrentAmount { get; set; } = 0.00m;

        /// <summary>
        /// Gets or sets the type of plan (MONTHLY, YEARLY, QUARTERLY)
        /// </summary>
        [Column("plan_type")]
        [Required]
        [MaxLength(50)]
        public string PlanType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the start date of the income plan
        /// </summary>
        [Column("start_date")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date of the income plan
        /// </summary>
        [Column("end_date")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the status of the income plan (ACTIVE, COMPLETED, PAUSED, CANCELLED)
        /// </summary>
        [Column("status")]
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "ACTIVE";

        /// <summary>
        /// Gets or sets whether the income plan is active
        /// </summary>
        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets when this income plan was created
        /// </summary>
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets when this income plan was last updated
        /// </summary>
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the navigation property to the user
        /// </summary>
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        /// <summary>
        /// Gets or sets the collection of income sources for this plan
        /// </summary>
        public virtual ICollection<IncomeSource> IncomeSources { get; set; } = new List<IncomeSource>();
        
        /// <summary>
        /// Gets or sets the collection of milestones for this plan
        /// </summary>
        public virtual ICollection<IncomePlanMilestone> Milestones { get; set; } = new List<IncomePlanMilestone>();
    }
}
