using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VetLineApp.Models
{
    [Table("appointments")]
    public class Appointment
    {
        [Key]
        [Column("appointment_id")]
        public int AppointmentId { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("animal_id")]
        public int AnimalId { get; set; }

        [Column("service_id")]
        public int? ServiceId { get; set; }

        [Required]
        [Column("appointment_datetime")]
        public DateTime AppointmentDateTime { get; set; }

        [Column("complaint")]
        public string? Complaint { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("AnimalId")]
        public virtual Animal Animal { get; set; } = null!;

        [ForeignKey("ServiceId")]
        public virtual Service? Service { get; set; }
    }
}
