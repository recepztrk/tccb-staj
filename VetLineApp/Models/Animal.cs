using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VetLineApp.Models
{
    [Table("animals")]
    public class Animal
    {
        [Key]
        [Column("animal_id")]
        public int AnimalId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [StringLength(30)]
        [Column("type")]
        public string Type { get; set; } = string.Empty;

        [StringLength(50)]
        [Column("breed")]
        public string? Breed { get; set; }

        [StringLength(10)]
        [Column("gender")]
        public string? Gender { get; set; }

        [Column("birth_date")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
        

        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
