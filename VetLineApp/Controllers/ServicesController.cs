using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetLineApp.Models;

namespace VetLineApp.Controllers
{
    public class ServicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Services
        public async Task<IActionResult> Index()
        {
            var services = await _context.Services.ToListAsync();
            return View(services);
        }

        // GET: Services/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Services
                .FirstOrDefaultAsync(m => m.ServiceId == id);
            
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        // Hizmet yönetimi Admin panelde yapılıyor - CRUD actions kaldırıldı
    }
}
