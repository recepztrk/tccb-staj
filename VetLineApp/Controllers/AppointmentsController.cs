using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VetLineApp.Models;
using VetLineApp.ViewModels;

namespace VetLineApp.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Appointments - Sadece giriş yapmış kullanıcının randevuları
        public async Task<IActionResult> Index()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(userIdString);
            var appointments = await _context.Appointments
                .Where(a => a.UserId == userId)
                .Include(a => a.Animal)
                .Include(a => a.User)
                .Include(a => a.Service)
                .OrderBy(a => a.AppointmentDateTime)
                .ToListAsync();
            
            return View(appointments);
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(userIdString);

            var appointment = await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Animal)
                .Where(a => a.UserId == userId) // Sadece kendi randevusu
                .FirstOrDefaultAsync(m => m.AppointmentId == id);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        public async Task<IActionResult> Create(string? returnUrl = null, int? serviceId = null)
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
            {
                // Login sayfasına yönlendir ve geri dönüş URL'ini kaydet
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Create", "Appointments") });
            }

            int userId = int.Parse(userIdString);
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Kullanıcının kayıtlı hayvanlarını getir
            var userAnimals = await _context.Animals
                .Where(a => a.UserId == userId)
                .OrderBy(a => a.Name)
                .ToListAsync();

            ViewBag.UserAnimals = userAnimals;

            // Mevcut hizmetleri getir
            var services = await _context.Services.OrderBy(s => s.Title).ToListAsync();
            ViewBag.Services = services;

            // ViewModel'i kullanıcı bilgileriyle doldur
            var model = new CreateAppointmentViewModel
            {
                OwnerFirstName = user.FirstName,
                OwnerLastName = user.LastName,
                OwnerEmail = user.Email,
                OwnerPhone = user.Phone ?? "",
                AppointmentDate = DateTime.Today.AddDays(1), // Yarın için varsayılan
                ServiceId = serviceId // Nullable olarak bırak
            };

            return View(model);
        }

        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAppointmentViewModel model)
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(userIdString);

            if (ModelState.IsValid)
            {
                int animalId;

                // Mevcut hayvan seçildi mi?
                if (model.ExistingAnimalId.HasValue && model.ExistingAnimalId > 0)
                {
                    animalId = model.ExistingAnimalId.Value;
                }
                else
                {
                    // Yeni hayvan oluştur
                    if (string.IsNullOrWhiteSpace(model.AnimalName) || string.IsNullOrWhiteSpace(model.AnimalType))
                    {
                        ModelState.AddModelError("", "Hayvan adı ve türü zorunludur.");
                        await LoadViewData(userId);
                        return View(model);
                    }

                    var newAnimal = new Animal
                    {
                        Name = model.AnimalName,
                        Type = model.AnimalType,
                        Breed = model.AnimalBreed,
                        Gender = model.AnimalGender,
                        UserId = userId,
                        // Yaş bilgisinden doğum tarihini tahmin et
                        BirthDate = TryParseAge(model.AnimalAge)
                    };

                    _context.Animals.Add(newAnimal);
                    await _context.SaveChangesAsync();
                    animalId = newAnimal.AnimalId;
                }

                // Randevu oluştur
                var appointmentDateTime = model.AppointmentDate.Date.Add(model.AppointmentTime);

                var appointment = new Appointment
                {
                    UserId = userId,
                    AnimalId = animalId,
                    ServiceId = model.ServiceId,
                    AppointmentDateTime = appointmentDateTime,
                    Complaint = model.Complaint
                };

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Randevunuz başarıyla oluşturuldu!";
                return RedirectToAction(nameof(Index));
            }

            await LoadViewData(userId);
            return View(model);
        }

        private async Task LoadViewData(int userId)
        {
            var userAnimals = await _context.Animals
                .Where(a => a.UserId == userId)
                .OrderBy(a => a.Name)
                .ToListAsync();
            ViewBag.UserAnimals = userAnimals;

            var services = await _context.Services.OrderBy(s => s.Title).ToListAsync();
            ViewBag.Services = services;
        }

        private DateTime? TryParseAge(string ageString)
        {
            if (string.IsNullOrWhiteSpace(ageString)) return null;
            
            if (int.TryParse(ageString, out int age) && age > 0 && age < 30)
            {
                return DateTime.Today.AddYears(-age);
            }
            
            return null;
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(userIdString);

            var appointment = await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Animal)
                .Where(a => a.UserId == userId) // Sadece kendi randevusu
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
            
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(userIdString);

            var appointment = await _context.Appointments
                .Where(a => a.UserId == userId) // Sadece kendi randevusu
                .FirstOrDefaultAsync(a => a.AppointmentId == id);
            
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Randevunuz başarıyla iptal edildi.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.AppointmentId == id);
        }
    }
}
