using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VetLineApp.Models
{
    [Table("completed_services")]
    public class CompletedService
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("animal_id")]
        public int AnimalId { get; set; }

        [Required]
        [Column("service_id")]
        public int ServiceId { get; set; }

        [Required]
        [Column("appointment_id")]
        public int AppointmentId { get; set; }

        [Required]
        [Column("completed_date")]
        public DateTime CompletedDate { get; set; }

        // Navigation properties
        [ForeignKey("AnimalId")]
        public virtual Animal Animal { get; set; } = null!;

        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; } = null!;

        [ForeignKey("AppointmentId")]
        public virtual Appointment Appointment { get; set; } = null!;
    }
}
