using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalFinanceAPI.Models.Entities
{
    /// <summary>
    /// Represents an actual income entry recorded against an income source
    /// </summary>
    [Table("income_entries")]
    public class IncomeEntry
    {
        /// <summary>
        /// Gets or sets the unique identifier for the income entry
        /// </summary>
        [Key]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the income source identifier this entry belongs to
        /// </summary>
        [Column("income_source_id")]
        [Required]
        public int IncomeSourceId { get; set; }

        /// <summary>
        /// Gets or sets the actual amount received
        /// </summary>
        [Column("amount")]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the date when the income was received
        /// </summary>
        [Column("received_date")]
        public DateTime ReceivedDate { get; set; }

        /// <summary>
        /// Gets or sets optional notes about this income entry
        /// </summary>
        [Column("notes")]
        [MaxLength(1000)]
        public string? Notes { get; set; }

        /// <summary>
        /// Gets or sets the reference number or transaction ID
        /// </summary>
        [Column("reference_number")]
        [MaxLength(100)]
        public string? ReferenceNumber { get; set; }

        /// <summary>
        /// Gets or sets when this entry was created
        /// </summary>
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets when this entry was last updated
        /// </summary>
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the navigation property to the income source
        /// </summary>
        [ForeignKey("IncomeSourceId")]
        public virtual IncomeSource IncomeSource { get; set; } = null!;
    }
}
