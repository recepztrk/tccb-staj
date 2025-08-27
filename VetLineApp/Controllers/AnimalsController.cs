using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VetLineApp.Models;
using VetLineApp.ViewModels;

namespace VetLineApp.Controllers
{
    public class AnimalsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AnimalsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Animals - Sadece giriş yapmış kullanıcının hayvanları
        public async Task<IActionResult> Index()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(userIdString);
            var animals = await _context.Animals
                .Where(a => a.UserId == userId)
                .OrderBy(a => a.Name)
                .ToListAsync();
            
            return View(animals);
        }

        // GET: Animals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animals
                .Include(a => a.User)
                .Include(a => a.Appointments)
                .FirstOrDefaultAsync(m => m.AnimalId == id);

            if (animal == null)
            {
                return NotFound();
            }

            return View(animal);
        }

        // GET: Animals/Create
        public IActionResult Create()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // POST: Animals/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAnimalViewModel model)
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                var animal = new Animal
                {
                    Name = model.Name,
                    Type = model.Type,
                    Breed = model.Breed,
                    Gender = model.Gender,
                    BirthDate = model.BirthDate?.Date, // Sadece tarih kısmını al
                    UserId = int.Parse(userIdString)
                };

                _context.Add(animal);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Hayvan başarıyla eklendi!";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Animals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animals
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.AnimalId == id);
                
            if (animal == null)
            {
                return NotFound();
            }
            
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", animal.UserId);
            return View(animal);
        }

        // POST: Animals/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AnimalId,Name,Type,Breed,Gender,BirthDate")] Animal animal)
        {
            // Debug log'ları
            Console.WriteLine($"Edit POST - ID: {id}");
            Console.WriteLine($"Animal ID: {animal.AnimalId}");
            Console.WriteLine($"Name: {animal.Name}");
            Console.WriteLine($"Type: {animal.Type}");
            Console.WriteLine($"Breed: {animal.Breed}");
            Console.WriteLine($"Gender: {animal.Gender}");
            Console.WriteLine($"BirthDate: {animal.BirthDate}");
            Console.WriteLine($"UserId: {animal.UserId}");
            
            if (id != animal.AnimalId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                Console.WriteLine("ModelState is valid - saving changes...");
                try
                {
                    // Mevcut hayvanı bul ve UserId'yi koru
                    var existingAnimal = await _context.Animals.FindAsync(id);
                    if (existingAnimal != null)
                    {
                        animal.UserId = existingAnimal.UserId; // UserId'yi koru
                        _context.Entry(existingAnimal).CurrentValues.SetValues(animal);
                        await _context.SaveChangesAsync();
                        Console.WriteLine("Changes saved successfully!");
                        TempData["Success"] = "Hayvan bilgileri başarıyla güncellendi!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Console.WriteLine($"DbUpdateConcurrencyException: {ex.Message}");
                    if (!AnimalExists(animal.AnimalId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"General exception: {ex.Message}");
                    throw;
                }
            }
            else
            {
                Console.WriteLine("ModelState is NOT valid:");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Error: {error.ErrorMessage}");
                }
            }
            
            var animalWithUser = await _context.Animals
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.AnimalId == animal.AnimalId);
                
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", animal.UserId);
            return View(animalWithUser ?? animal);
        }

        // GET: Animals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animals
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.AnimalId == id);
            
            if (animal == null)
            {
                return NotFound();
            }

            return View(animal);
        }

        // POST: Animals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var animal = await _context.Animals.FindAsync(id);
            if (animal != null)
            {
                _context.Animals.Remove(animal);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnimalExists(int id)
        {
            return _context.Animals.Any(e => e.AnimalId == id);
        }
    }
}
