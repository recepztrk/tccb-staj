using System.ComponentModel.DataAnnotations;

namespace VetLineApp.ViewModels
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir.")]
        [Display(Name = "Ad")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir.")]
        [Display(Name = "Soyad")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-posta")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon alanı zorunludur.")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        [Display(Name = "Telefon")]
        public string Phone { get; set; } = string.Empty;

        [Display(Name = "Mevcut Şifre")]
        public string? CurrentPassword { get; set; }

        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        [Display(Name = "Yeni Şifre")]
        public string? NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Şifreler eşleşmiyor.")]
        [Display(Name = "Yeni Şifre (Tekrar)")]
        public string? ConfirmNewPassword { get; set; }

        [Display(Name = "Email Doğrulandı")]
        public bool EmailVerified { get; set; }

        [Display(Name = "Kayıt Tarihi")]
        public DateTime CreatedAt { get; set; }
    }
}
