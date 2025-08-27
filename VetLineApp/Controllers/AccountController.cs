using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetLineApp.Models;
using VetLineApp.ViewModels;
using VetLineApp.Services;
using System.Security.Cryptography;
using System.Text;

namespace VetLineApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;
        private readonly JwtEmailService _jwtEmailService;

        public AccountController(ApplicationDbContext context, ILogger<AccountController> logger, JwtEmailService jwtEmailService)
        {
            _context = context;
            _logger = logger;
            _jwtEmailService = jwtEmailService;
        }

        [HttpGet]
        public IActionResult Register(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Check if email already exists
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "Bu e-posta adresi zaten kullanılıyor.");
                    return View(model);
                }

                var user = new User
                {
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Phone = model.Phone,
                    PasswordHash = HashPassword(model.Password),
                    EmailVerified = false // Email doğrulanmamış olarak kaydet
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User created a new account with email: {Email}", user.Email);
                
                // JWT token ile email doğrulama linki oluştur
                var token = _jwtEmailService.GenerateEmailVerificationToken(user.UserId, user.Email);
                var verificationLink = Url.Action("VerifyEmail", "Account", 
                    new { token = token }, Request.Scheme);

                // Email gönder
                var emailSent = await _jwtEmailService.SendVerificationEmailAsync(user.Email, verificationLink!);

                if (emailSent)
                {
                    TempData["Success"] = "Hesabınız oluşturuldu! Email adresinize gönderilen doğrulama linkine tıklayarak hesabınızı aktif hale getirin.";
                    TempData["Email"] = user.Email; // TempData ile taşı
                    return RedirectToAction("EmailSent");
                }
                else
                {
                    TempData["Error"] = "Hesabınız oluşturuldu ancak doğrulama emaili gönderilemedi. Lütfen daha sonra tekrar deneyin.";
                    return RedirectToAction("Login");
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                
                if (user != null && VerifyPassword(model.Password, user.PasswordHash))
                {
                    // Email doğrulama kontrolü
                    if (!user.EmailVerified)
                    {
                        TempData["Error"] = "Email adresinizi doğrulamadan giriş yapamazsınız. Gelen kutunuzu kontrol edin.";
                        ViewBag.Email = user.Email;
                        ViewBag.ShowResendLink = true;
                        return View(model);
                    }

                    _logger.LogInformation("User logged in: {Email}", user.Email);
                    
                    // Simple session-based login
                    HttpContext.Session.SetString("UserId", user.UserId.ToString());
                    HttpContext.Session.SetString("UserEmail", user.Email);
                    HttpContext.Session.SetString("UserName", $"{user.FirstName} {user.LastName}");
                    HttpContext.Session.SetString("IsAdmin", user.IsAdmin.ToString());

                    // Admin ise admin paneline yönlendir (return URL'i göz ardı et)
                    if (user.IsAdmin)
                    {
                        return RedirectToAction("Index", "Admin");
                    }

                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Geçersiz e-posta veya şifre.");
                    return View(model);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // Email doğrulama action'ları
        [HttpGet]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Geçersiz doğrulama linki.";
                return RedirectToAction("Login");
            }

            // Debug için log ekle
            _logger.LogInformation("Email verification attempt - Token: {Token}", token);

            var (isValid, userId, email) = _jwtEmailService.ValidateEmailVerificationToken(token);
            
            // Debug için sonucu logla
            _logger.LogInformation("Token validation result - IsValid: {IsValid}, UserId: {UserId}, Email: {Email}", isValid, userId, email);

            if (!isValid)
            {
                TempData["Error"] = "Doğrulama linki geçersiz veya süresi dolmuş. Lütfen yeni bir doğrulama emaili isteyin.";
                return View("EmailVerificationFailed");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.Email != email)
            {
                TempData["Error"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("Login");
            }

            if (user.EmailVerified)
            {
                TempData["Info"] = "Email adresiniz zaten doğrulanmış.";
                return RedirectToAction("Login");
            }

            // Email doğrulamayı tamamla
            user.EmailVerified = true;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Email verified for user: {Email}", user.Email);

            TempData["Success"] = "Email adresiniz başarıyla doğrulandı! Artık giriş yapabilirsiniz.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult EmailSent()
        {
            // TempData'dan email'i ViewBag'e aktar
            ViewBag.Email = TempData["Email"] as string;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendVerificationEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Email adresi gerekli.";
                return RedirectToAction("Login");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && !u.EmailVerified);

            if (user == null)
            {
                TempData["Error"] = "Bu email adresi ile doğrulanmamış bir hesap bulunamadı.";
                return RedirectToAction("Login");
            }

            // Yeni token oluştur ve email gönder
            var token = _jwtEmailService.GenerateEmailVerificationToken(user.UserId, user.Email);
            var verificationLink = Url.Action("VerifyEmail", "Account", 
                new { token = token }, Request.Scheme);

            var emailSent = await _jwtEmailService.SendVerificationEmailAsync(user.Email, verificationLink!);

            if (emailSent)
            {
                TempData["Success"] = "Doğrulama emaili tekrar gönderildi. Gelen kutunuzu kontrol edin.";
                TempData["Email"] = user.Email; // TempData ile taşı
                return RedirectToAction("EmailSent");
            }
            else
            {
                TempData["Error"] = "Email gönderilemedi. Lütfen daha sonra tekrar deneyin.";
                return RedirectToAction("Login");
            }
        }

        // Profil ayarları action'ları
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login");
            }

            var userId = int.Parse(userIdString);
            var user = await _context.Users.FindAsync(userId);
            
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var viewModel = new ProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                EmailVerified = user.EmailVerified,
                CreatedAt = DateTime.Now // User modelinde CreatedAt yoksa şimdilik şu anki tarih
            };

            // Admin kullanıcılar için admin layout'u kullan
            if (user.IsAdmin)
            {
                ViewBag.IsAdmin = true;
                ViewBag.Layout = "_AdminLayout";
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login");
            }

            var userId = int.Parse(userIdString);
            var user = await _context.Users.FindAsync(userId);
            
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            if (ModelState.IsValid)
            {
                // Email değişikliği kontrolü
                if (user.Email != model.Email)
                {
                    var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email && u.UserId != userId);
                    if (existingUser != null)
                    {
                        ModelState.AddModelError("Email", "Bu e-posta adresi zaten kullanılıyor.");
                        return View(model);
                    }
                    user.Email = model.Email;
                    user.EmailVerified = false; // Email değiştiğinde tekrar doğrulama gerekir
                }

                // Şifre değişikliği kontrolü
                if (!string.IsNullOrEmpty(model.CurrentPassword) && !string.IsNullOrEmpty(model.NewPassword))
                {
                    if (!VerifyPassword(model.CurrentPassword, user.PasswordHash))
                    {
                        ModelState.AddModelError("CurrentPassword", "Mevcut şifre yanlış.");
                        return View(model);
                    }
                    user.PasswordHash = HashPassword(model.NewPassword);
                }

                // Diğer bilgileri güncelle
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Phone = model.Phone;

                await _context.SaveChangesAsync();

                // Session'ı güncelle
                HttpContext.Session.SetString("UserName", $"{user.FirstName} {user.LastName}");
                HttpContext.Session.SetString("UserEmail", user.Email);

                TempData["Success"] = "Profil bilgileriniz başarıyla güncellendi.";
                
                // Email değiştiyse yeni doğrulama emaili gönder
                if (!user.EmailVerified && user.Email != model.Email)
                {
                    var token = _jwtEmailService.GenerateEmailVerificationToken(user.UserId, user.Email);
                    var verificationLink = Url.Action("VerifyEmail", "Account", 
                        new { token = token }, Request.Scheme);
                    await _jwtEmailService.SendVerificationEmailAsync(user.Email, verificationLink!);
                    
                    TempData["Info"] = "Email adresiniz değiştirildi. Yeni doğrulama emaili gönderildi.";
                }

                return RedirectToAction("Profile");
            }

            // Validation hatası varsa mevcut bilgileri koru
            model.EmailVerified = user.EmailVerified;
            model.CreatedAt = DateTime.Now;
            
            return View(model);
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "VetLineSalt"));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }
    }
}