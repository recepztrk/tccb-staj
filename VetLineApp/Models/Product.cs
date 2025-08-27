using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VetLineApp.Models
{
    [Table("products")]
    public class Product
    {
        [Key]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [StringLength(50)]
        [Column("category")]
        public string? Category { get; set; }

        [StringLength(30)]
        [Column("animal_type")]
        public string? AnimalType { get; set; }

        [StringLength(50)]
        [Column("brand")]
        public string? Brand { get; set; }

        [Column("stock")]
        public int Stock { get; set; } = 0;

        [Column("image_url")]
        public string? ImageUrl { get; set; }
    }
}
