using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VetLineApp.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Column("last_name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [EmailAddress]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [StringLength(15)]
        [Column("phone")]
        public string? Phone { get; set; }

        [Required]
        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [Column("is_admin")]
        public bool IsAdmin { get; set; } = false;

        [Column("email_verified")]
        public bool EmailVerified { get; set; } = false;

        // Navigation properties
        public virtual ICollection<Animal> Animals { get; set; } = new List<Animal>();
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<UserReview> UserReviews { get; set; } = new List<UserReview>();
    }
}
