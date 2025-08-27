using System.ComponentModel.DataAnnotations;

namespace VetLineApp.ViewModels
{
    public class CreateAppointmentViewModel
    {
        // Sahip Bilgileri (otomatik doldurulacak)
        [Display(Name = "Ad")]
        public string OwnerFirstName { get; set; } = string.Empty;

        [Display(Name = "Soyad")]
        public string OwnerLastName { get; set; } = string.Empty;

        [Display(Name = "E-Posta Adresi")]
        public string OwnerEmail { get; set; } = string.Empty;

        [Display(Name = "Telefon Numarası")]
        public string OwnerPhone { get; set; } = string.Empty;

        // Hayvan seçimi
        [Display(Name = "Kayıtlı Hayvan Seç")]
        public int? ExistingAnimalId { get; set; }

        // Hayvan Bilgileri (yeni hayvan veya mevcut hayvan bilgileri)
        [Display(Name = "Hayvan Adı")]
        public string AnimalName { get; set; } = string.Empty;

        [Display(Name = "Yaş")]
        public string AnimalAge { get; set; } = string.Empty;

        [Display(Name = "Tür")]
        public string AnimalType { get; set; } = string.Empty;

        [Display(Name = "Cins")]
        public string AnimalBreed { get; set; } = string.Empty;

        [Display(Name = "Cinsiyet")]
        public string AnimalGender { get; set; } = string.Empty;

        // Randevu Bilgileri
        [Required(ErrorMessage = "Randevu tarihi zorunludur.")]
        [Display(Name = "Randevu Tarihi")]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = "Randevu saati zorunludur.")]
        [Display(Name = "Randevu Saati")]
        [DataType(DataType.Time)]
        public TimeSpan AppointmentTime { get; set; }

        [Required(ErrorMessage = "Hizmet seçimi zorunludur.")]
        [Display(Name = "Almak İstediğiniz Hizmet")]
        public int? ServiceId { get; set; }

        [Display(Name = "Şikayet/Açıklama")]
        [StringLength(500, ErrorMessage = "Şikayet en fazla 500 karakter olabilir.")]
        public string? Complaint { get; set; }
    }
}
