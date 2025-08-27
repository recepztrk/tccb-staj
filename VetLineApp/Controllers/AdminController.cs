using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetLineApp.Models;

namespace VetLineApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ApplicationDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Admin yetki kontrolü
        private bool IsAdmin()
        {
            var isAdminString = HttpContext.Session.GetString("IsAdmin");
            return !string.IsNullOrEmpty(isAdminString) && isAdminString == "True";
        }

        private IActionResult CheckAdminAccess()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account");
            }

            if (!IsAdmin())
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            return null; // Erişim izni var
        }

        // GET: Admin Dashboard
        public async Task<IActionResult> Index()
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            // Dashboard istatistikleri
            var stats = new
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalAnimals = await _context.Animals.CountAsync(),
                TotalAppointments = await _context.Appointments.CountAsync(),
                TotalProducts = await _context.Products.CountAsync(),
                TodayAppointments = await _context.Appointments
                    .Where(a => a.AppointmentDateTime.Date == DateTime.Today)
                    .CountAsync(),
                PendingAppointments = await _context.Appointments
                    .Where(a => a.AppointmentDateTime > DateTime.Now)
                    .CountAsync(),
                LowStockProducts = await _context.Products
                    .Where(p => p.Stock < 10)
                    .CountAsync()
            };

            ViewBag.Stats = stats;
            return View();
        }

        // GET: Admin/Users - Kullanıcı yönetimi
        public async Task<IActionResult> Users()
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            var users = await _context.Users
                .Include(u => u.Animals)
                .Include(u => u.Appointments)
                .OrderBy(u => u.FirstName)
                .ToListAsync();

            return View(users);
        }

        // GET: Admin/Appointments - Randevu yönetimi
        public async Task<IActionResult> Appointments()
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            var appointments = await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Animal)
                .OrderBy(a => a.AppointmentDateTime)
                .ToListAsync();

            return View(appointments);
        }

        // GET: Admin/Animals - Hayvan yönetimi
        public async Task<IActionResult> Animals()
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            var animals = await _context.Animals
                .Include(a => a.User)
                .Include(a => a.Appointments)
                .OrderBy(a => a.Name)
                .ToListAsync();

            return View(animals);
        }

        // GET: Admin/Products - Ürün yönetimi
        public async Task<IActionResult> Products()
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            var products = await _context.Products
                .OrderBy(p => p.Name)
                .ToListAsync();

            return View(products);
        }



        // POST: Admin/DeleteUser - Kullanıcı sil
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Admin kullanıcıları silinemez
            if (user.IsAdmin)
            {
                TempData["Error"] = "Admin kullanıcıları silinemez. Admin yetkisi sadece veritabanından kaldırılabilir.";
                return RedirectToAction(nameof(Users));
            }

            // Kendi hesabını silemesin
            var currentUserId = int.Parse(HttpContext.Session.GetString("UserId"));
            if (user.UserId == currentUserId)
            {
                TempData["Error"] = "Kendi hesabınızı silemezsiniz.";
                return RedirectToAction(nameof(Users));
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"{user.FirstName} {user.LastName} kullanıcısı silindi.";
            return RedirectToAction(nameof(Users));
        }

        // POST: Admin/UpdateProduct - Ürün bilgilerini güncelle
        [HttpPost]
        public async Task<IActionResult> UpdateProduct(int productId, string name, string description, 
            string category, string animalType, string brand, int stock)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            product.Name = name;
            product.Description = description;
            product.Category = category;
            product.AnimalType = animalType;
            product.Brand = brand;
            product.Stock = stock;

            await _context.SaveChangesAsync();

            TempData["Success"] = $"{product.Name} ürünü başarıyla güncellendi.";
            return RedirectToAction(nameof(Products));
        }

        // POST: Admin/UpdateStock - Ürün stok güncelle
        [HttpPost]
        public async Task<IActionResult> UpdateStock(int productId, int newStock)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            product.Stock = newStock;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"{product.Name} ürününün stoğu {newStock} olarak güncellendi.";
            return RedirectToAction(nameof(Products));
        }

        // POST: Admin/CreateProduct - Yeni ürün ekle
        [HttpPost]
        public async Task<IActionResult> CreateProduct(string Name, string Description, string Category, string AnimalType, string Brand, int Stock, IFormFile? ImageFile)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            try
            {
                var product = new Product
                {
                    Name = Name,
                    Description = Description,
                    Category = Category,
                    AnimalType = AnimalType,
                    Brand = Brand,
                    Stock = Stock
                };

                // Resim yükleme işlemi
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products", fileName);
                    
                    // Dizin yoksa oluştur
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }
                    
                    product.ImageUrl = "/images/products/" + fileName;
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"{Name} ürünü başarıyla eklendi.";
                return RedirectToAction(nameof(Products));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ürün eklenirken bir hata oluştu: " + ex.Message;
                return RedirectToAction(nameof(Products));
            }
        }

        // POST: Admin/DeleteProduct - Ürün sil
        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            var productName = product.Name;
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"{productName} ürünü başarıyla silindi.";
            return RedirectToAction(nameof(Products));
        }
    }
}
