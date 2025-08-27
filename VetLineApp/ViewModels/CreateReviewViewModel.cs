using System.ComponentModel.DataAnnotations;

namespace VetLineApp.ViewModels
{
    public class CreateReviewViewModel
    {
        [Required(ErrorMessage = "Mesaj alanı zorunludur.")]
        [StringLength(1000, ErrorMessage = "Mesaj en fazla 1000 karakter olabilir.")]
        [Display(Name = "Yorumunuz")]
        public string Message { get; set; } = string.Empty;
    }
}
