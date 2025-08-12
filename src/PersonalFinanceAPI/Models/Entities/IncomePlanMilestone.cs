using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalFinanceAPI.Models.Entities
{
    /// <summary>
    /// Represents a milestone or goal within an income plan
    /// </summary>
    [Table("income_plan_milestones")]
    public class IncomePlanMilestone
    {
        /// <summary>
        /// Gets or sets the unique identifier for the milestone
        /// </summary>
        [Key]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the income plan identifier this milestone belongs to
        /// </summary>
        [Column("income_plan_id")]
        [Required]
        public int IncomePlanId { get; set; }

        /// <summary>
        /// Gets or sets the name of the milestone
        /// </summary>
        [Column("name")]
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the milestone
        /// </summary>
        [Column("description")]
        [MaxLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the target amount for this milestone
        /// </summary>
        [Column("target_amount")]
        [Range(0, double.MaxValue)]
        public decimal TargetAmount { get; set; }

        /// <summary>
        /// Gets or sets the target date to achieve this milestone
        /// </summary>
        [Column("target_date")]
        public DateTime TargetDate { get; set; }

        /// <summary>
        /// Gets or sets whether this milestone has been achieved
        /// </summary>
        [Column("is_achieved")]
        public bool IsAchieved { get; set; } = false;

        /// <summary>
        /// Gets or sets the date when this milestone was achieved
        /// </summary>
        [Column("achieved_date")]
        public DateTime? AchievedDate { get; set; }

        /// <summary>
        /// Gets or sets when this milestone was created
        /// </summary>
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets when this milestone was last updated
        /// </summary>
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the navigation property to the income plan
        /// </summary>
        [ForeignKey("IncomePlanId")]
        public virtual IncomePlan IncomePlan { get; set; } = null!;
    }
}
