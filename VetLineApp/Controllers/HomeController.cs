using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetLineApp.Models;
using VetLineApp.ViewModels;

namespace VetLineApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var viewModel = await GetHomeViewModelAsync();
        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    [HttpGet]
    [Route("Contact")]
    public async Task<IActionResult> Contact()
    {
        var viewModel = await GetContactViewModelAsync();
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddReview(CreateReviewViewModel model)
    {
        // Authentication guard for adding reviews
        var userIdString = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userIdString))
        {
            // Anonymous user - redirect to login with return URL pointing to contact with anchor
            var returnUrl = Url.Action("Contact", "Home") + "#yorum";
            return RedirectToAction("Login", "Account", new { returnUrl });
        }

        if (ModelState.IsValid)
        {
            var userId = int.Parse(userIdString);
            
            var review = new UserReview
            {
                UserId = userId,
                Message = model.Message,
                ReviewDate = DateTime.UtcNow
            };

            _context.UserReviews.Add(review);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Yorumunuz başarıyla eklendi! Teşekkürler.";
            return Redirect(Url.Action("Contact", "Home") + "#yorum");
        }

        // Validation failed - reload contact page with errors
        var viewModel = await GetContactViewModelAsync();
        ViewBag.ReviewModel = model;
        return View("Contact", viewModel);
    }

    [HttpGet]
    [Route("Inform")]
    public IActionResult Inform()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private async Task<HomeViewModel> GetHomeViewModelAsync()
    {
        // Get featured products
        var featuredProducts = await _context.Products
            .Take(3)
            .Select(p => new ProductCard(
                p.ImageUrl ?? "/image/no-image.png",
                p.Name,
                p.Description,
                null, // Price will be added later if needed
                $"/Products/Details/{p.ProductId}"
            ))
            .ToListAsync();

        // Get services
        var services = await _context.Services
            .Take(3)
            .Select(s => new ServiceCard(
                "fas fa-stethoscope", // Default icon, can be customized per service
                s.Title,
                s.ShortDescription ?? "Profesyonel veteriner hizmetlerimizden faydalanın.",
                "Detaylı Bilgi",
                $"/Services/Details/{s.ServiceId}"
            ))
            .ToListAsync();

        // Get recent testimonials (latest first)
        var testimonials = await _context.UserReviews
            .Include(r => r.User)
            .OrderByDescending(r => r.ReviewDate)
            .Take(6) // Show more reviews on homepage
            .Select(r => new TestimonialCard(
                $"{r.User.FirstName} {r.User.LastName}",
                r.ReviewDate,
                r.Message
            ))
            .ToListAsync();

        return new HomeViewModel
        {
            Slider = GetSliderItems(),
            AppointmentCtaHref = "/Appointments/Create",
            InfoBoxes = GetInfoBoxes(),
            FeaturedProducts = featuredProducts,
            Services = services,
            Testimonials = testimonials
        };
    }

    private async Task<ContactViewModel> GetContactViewModelAsync()
    {
        var testimonials = await _context.UserReviews
            .Include(r => r.User)
            .OrderByDescending(r => r.ReviewDate)
            .Take(10)
            .Select(r => new TestimonialCard(
                $"{r.User.FirstName} {r.User.LastName}",
                r.ReviewDate,
                r.Message
            ))
            .ToListAsync();

        return new ContactViewModel
        {
            Phone = "+90 (212) 123 45 67",
            Email = "info@vetline.com",
            Address = "Beşiktaş, İstanbul, Türkiye",
            Instagram = "https://instagram.com/vetline",
            Youtube = "https://youtube.com/vetline",
            Facebook = "https://facebook.com/vetline",
            X = "https://x.com/vetline",
            MapLat = 41.0082,
            MapLng = 28.9784,
            Testimonials = testimonials
        };
    }

    private List<HomeSliderItem> GetSliderItems()
    {
        return new List<HomeSliderItem>
        {
            new("~/image/slider1.jpg", 
                "VetLine Veteriner Kliniği", 
                "Evcil dostlarınızın sağlığı bizim önceliğimiz", 
                "Randevu Al", 
                "/Appointments/Create"),
            new("~/image/slider2.jpg", 
                "Profesyonel Sağlık Hizmetleri", 
                "Uzman veteriner hekimlerimizle 7/24 hizmetinizdeyiz", 
                "Hizmetlerimiz", 
                "/Services"),
            new("~/image/slider3.jpg", 
                "Modern Teknoloji", 
                "En son teknoloji ile donatılmış laboratuvar ve teşhis imkanları", 
                "Bilgilendirme", 
                "/Inform")
        };
    }

    private List<HomeInfoBox> GetInfoBoxes()
    {
        return new List<HomeInfoBox>
        {
            new("Uzman Veteriner Hekimler", 
                "Deneyimli ve uzman veteriner hekimlerimiz ile evcil dostlarınız güvende. Her alanda uzmanlaşmış ekibimizle hizmet veriyoruz.",
                "Detaylı Bilgi",
                "/Inform#team"),
            new("7/24 Acil Servis", 
                "Acil durumlarda 7/24 hizmetinizdeyiz. Hayvanınızın sağlığı için her zaman buradayız ve anında müdahale ediyoruz.",
                "Acil İletişim",
                "/Contact#emergency"),
            new("Modern Teknoloji", 
                "En son teknoloji ile donatılmış laboratuvar ve teşhis imkanları. Hızlı ve doğru teşhis için gelişmiş cihazlarımız.",
                "Teknolojimiz",
                "/Inform#technology")
        };
    }
}
