using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VetLineApp.Models
{
    [Table("user_reviews")]
    public class UserReview
    {
        [Key]
        [Column("review_id")]
        public int ReviewId { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("message")]
        public string Message { get; set; } = string.Empty;

        [Required]
        [Column("review_date")]
        [DataType(DataType.Date)]
        public DateTime ReviewDate { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}
