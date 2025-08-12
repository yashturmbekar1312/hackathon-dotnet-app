using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalFinanceAPI.Models.Entities
{
    /// <summary>
    /// Represents a source of income within an income plan
    /// </summary>
    [Table("income_sources")]
    public class IncomeSource
    {
        /// <summary>
        /// Gets or sets the unique identifier for the income source
        /// </summary>
        [Key]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the income plan identifier this source belongs to
        /// </summary>
        [Column("income_plan_id")]
        [Required]
        public int IncomePlanId { get; set; }

        /// <summary>
        /// Gets or sets the name of the income source
        /// </summary>
        [Column("name")]
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the income source
        /// </summary>
        [Column("description")]
        [MaxLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the type of income source (SALARY, FREELANCE, INVESTMENT, BUSINESS, OTHER)
        /// </summary>
        [Column("source_type")]
        [Required]
        [MaxLength(50)]
        public string SourceType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the expected amount from this income source
        /// </summary>
        [Column("expected_amount")]
        [Range(0, double.MaxValue)]
        public decimal ExpectedAmount { get; set; }

        /// <summary>
        /// Gets or sets the actual amount received from this income source
        /// </summary>
        [Column("actual_amount")]
        [Range(0, double.MaxValue)]
        public decimal ActualAmount { get; set; } = 0.00m;

        /// <summary>
        /// Gets or sets the frequency of this income source (WEEKLY, MONTHLY, QUARTERLY, YEARLY, ONE_TIME)
        /// </summary>
        [Column("frequency")]
        [Required]
        [MaxLength(20)]
        public string Frequency { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether this income source is currently active
        /// </summary>
        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets the start date of this income source
        /// </summary>
        [Column("start_date")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date of this income source
        /// </summary>
        [Column("end_date")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets when this income source was created
        /// </summary>
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets when this income source was last updated
        /// </summary>
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the navigation property to the income plan
        /// </summary>
        [ForeignKey("IncomePlanId")]
        public virtual IncomePlan IncomePlan { get; set; } = null!;

        /// <summary>
        /// Gets or sets the collection of income entries for this source
        /// </summary>
        public virtual ICollection<IncomeEntry> IncomeEntries { get; set; } = new List<IncomeEntry>();
    }
}
