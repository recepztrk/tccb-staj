using System.ComponentModel.DataAnnotations;

namespace VetLineApp.ViewModels
{
    public class CreateAnimalViewModel
    {
        [Required(ErrorMessage = "Hayvan adı zorunludur.")]
        [StringLength(50, ErrorMessage = "Hayvan adı en fazla 50 karakter olabilir.")]
        [Display(Name = "Hayvan Adı")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hayvan türü zorunludur.")]
        [StringLength(30, ErrorMessage = "Hayvan türü en fazla 30 karakter olabilir.")]
        [Display(Name = "Hayvan Türü")]
        public string Type { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Cins en fazla 50 karakter olabilir.")]
        [Display(Name = "Cins")]
        public string? Breed { get; set; }

        [Display(Name = "Cinsiyet")]
        public string? Gender { get; set; }

        [Display(Name = "Doğum Tarihi")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }
    }
}
