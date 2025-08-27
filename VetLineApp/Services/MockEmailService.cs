using VetLineApp.Services;

namespace VetLineApp.Services
{
    /// <summary>
    /// Development için mock email servisi - gerçek email göndermez, sadece console'a yazar
    /// </summary>
    public class MockEmailService : JwtEmailService
    {
        private readonly ILogger<MockEmailService> _logger;
        
        public MockEmailService(IConfiguration configuration, ILogger<MockEmailService> logger) 
            : base(configuration)
        {
            _logger = logger;
        }

        /// <summary>
        /// Mock email gönderimi - gerçek email göndermez, sadece console'da gösterir
        /// </summary>
        public override async Task<bool> SendVerificationEmailAsync(string email, string verificationLink)
        {
            try
            {
                // Development modunda gerçek email göndermek yerine console'a yaz
                Console.WriteLine("\n" + new string('=', 80));
                Console.WriteLine("📧 EMAIL DOĞRULAMA - DEVELOPMENT MODU");
                Console.WriteLine(new string('=', 80));
                Console.WriteLine($"📬 Alıcı: {email}");
                Console.WriteLine($"📋 Konu: VetLine - Email Adresinizi Doğrulayın");
                Console.WriteLine($"🔗 Doğrulama Linki:");
                Console.WriteLine($"   {verificationLink}");
                Console.WriteLine(new string('=', 80));
                Console.WriteLine("💡 Development modunda - Yukarıdaki linki tarayıcıya kopyalayın!");
                Console.WriteLine(new string('=', 80) + "\n");

                _logger.LogInformation("Mock email sent to {Email} with verification link: {Link}", 
                    email, verificationLink);

                // Mock olarak her zaman başarılı dön
                await Task.Delay(100); // Simulate network delay
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mock email service error for {Email}", email);
                return false;
            }
        }
    }
}
