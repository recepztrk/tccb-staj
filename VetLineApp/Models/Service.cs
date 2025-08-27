using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VetLineApp.Models
{
    [Table("services")]
    public class Service
    {
        [Key]
        [Column("service_id")]
        public int ServiceId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Column("short_description")]
        public string? ShortDescription { get; set; }

        [Column("details")]
        public string? Details { get; set; }


    }
}
