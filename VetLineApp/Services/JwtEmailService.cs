using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

namespace VetLineApp.Services
{
    public class JwtEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _secretKey;
        private readonly string _secretKeyHash;

        public JwtEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _secretKey = _configuration["JwtSettings:SecretKey"] ?? 
                "VetLine-Super-Secret-Email-Verification-Key-32-Characters-Minimum";
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(_secretKey));
            _secretKeyHash = Convert.ToHexString(hashBytes);
        }

        /// <summary>
        /// JWT token ile email doğrulama linki oluşturur
        /// </summary>
        public string GenerateEmailVerificationToken(int userId, string email)
        {
            Console.WriteLine($"JWT generate using secret hash (sha256): {_secretKeyHash}");
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("userId", userId.ToString()),
                    new Claim("email", email),
                    new Claim("purpose", "email_verification"),
                    new Claim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
                }),
                Expires = DateTime.UtcNow.AddHours(24), // 24 saat geçerli
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = "VetLineApp",
                Audience = "VetLineUsers"
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// JWT token'ı doğrular ve kullanıcı bilgilerini döner
        /// </summary>
        public (bool IsValid, int UserId, string Email) ValidateEmailVerificationToken(string token)
        {
            try
            {
                Console.WriteLine($"JWT validate using secret hash (sha256): {_secretKeyHash}");
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_secretKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = "VetLineApp",
                    ValidateAudience = true,
                    ValidAudience = "VetLineUsers",
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5) // 5 dakika tolerans
                };

                // Debug: token içeriğini logla (iss, aud)
                try
                {
                    var readable = tokenHandler.ReadJwtToken(token);
                    Console.WriteLine($"JWT Read (debug) -> iss: {readable.Issuer}, aud: {string.Join(',', readable.Audiences)}");
                }
                catch { /* ignore read errors */ }

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                
                // Debug: tüm claims'i logla
                Console.WriteLine($"JWT validation successful, all claims:");
                foreach (var claim in principal.Claims)
                {
                    Console.WriteLine($"  {claim.Type}: {claim.Value}");
                }

                // Purpose kontrolü
                var purposeClaim = principal.Claims.FirstOrDefault(x => x.Type == "purpose")?.Value;
                if (purposeClaim != "email_verification")
                {
                    Console.WriteLine($"Purpose claim not found or invalid: '{purposeClaim}'");
                    return (false, 0, string.Empty);
                }

                var userIdClaim = principal.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
                var emailClaim = principal.Claims.FirstOrDefault(x => x.Type == "email")?.Value;
                
                // JWT'de email farklı claim type ile gelebilir, alternatif olarak arayalım
                if (string.IsNullOrEmpty(emailClaim))
                {
                    emailClaim = principal.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
                }

                Console.WriteLine($"Extracted claims - userId: '{userIdClaim}', email: '{emailClaim}'");

                if (int.TryParse(userIdClaim, out int userId) && !string.IsNullOrEmpty(emailClaim))
                {
                    Console.WriteLine($"Claims parsed successfully - userId: {userId}, email: {emailClaim}");
                    return (true, userId, emailClaim);
                }

                Console.WriteLine($"Failed to parse claims - userId parse: {int.TryParse(userIdClaim, out _)}, email empty: {string.IsNullOrEmpty(emailClaim)}");
                return (false, 0, string.Empty);
            }
            catch (SecurityTokenExpiredException ex)
            {
                Console.WriteLine($"JWT validation error: expired -> {ex.Message}");
                return (false, 0, string.Empty);
            }
            catch (SecurityTokenInvalidAudienceException ex)
            {
                Console.WriteLine($"JWT validation error: invalid audience -> {ex.Message}");
                return (false, 0, string.Empty);
            }
            catch (SecurityTokenInvalidIssuerException ex)
            {
                Console.WriteLine($"JWT validation error: invalid issuer -> {ex.Message}");
                return (false, 0, string.Empty);
            }
            catch (SecurityTokenInvalidSignatureException ex)
            {
                Console.WriteLine($"JWT validation error: invalid signature -> {ex.Message}");
                return (false, 0, string.Empty);
            }
            catch (SecurityTokenException ex)
            {
                Console.WriteLine($"JWT validation error: token error -> {ex.Message}");
                return (false, 0, string.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"JWT validation error: unexpected -> {ex.Message}");
                return (false, 0, string.Empty);
            }
        }

        /// <summary>
        /// Email doğrulama emaili gönderir
        /// </summary>
        public virtual async Task<bool> SendVerificationEmailAsync(string email, string verificationLink)
        {
            try
            {
                var smtpServer = _configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
                var username = _configuration["EmailSettings:Username"];
                var password = _configuration["EmailSettings:Password"];
                var fromEmail = _configuration["EmailSettings:FromEmail"] ?? username;
                var fromName = _configuration["EmailSettings:FromName"] ?? "VetLine";

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    throw new InvalidOperationException("Email yapılandırması eksik. Username ve Password gerekli.");
                }

                using var client = new SmtpClient(smtpServer, smtpPort);
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(username, password);
                client.EnableSsl = true;

                var mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(fromEmail ?? username, fromName);
                mailMessage.To.Add(email);
                mailMessage.Subject = "VetLine - Email Adresinizi Doğrulayın";
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = GetEmailTemplate(verificationLink);

                await client.SendMailAsync(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                // Log the error (gerçek uygulamada loglama sistemi kullanın)
                Console.WriteLine($"Email gönderme hatası: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Email doğrulama şablonu
        /// </summary>
        private string GetEmailTemplate(string verificationLink)
        {
            return $@"
<!DOCTYPE html>
<html lang='tr'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>VetLine Email Doğrulama</title>
</head>
<body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f8f9fa;'>
    <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 4px rgba(0,0,0,0.1);'>
        
        <!-- Header -->
        <div style='background: linear-gradient(135deg, #007bff 0%, #0056b3 100%); padding: 30px; text-align: center;'>
            <h1 style='color: #ffffff; margin: 0; font-size: 28px; font-weight: bold;'>
                🐾 VetLine
            </h1>
            <p style='color: #e3f2fd; margin: 5px 0 0 0; font-size: 16px;'>
                Evcil Hayvan Sağlık Sistemi
            </p>
        </div>

        <!-- Content -->
        <div style='padding: 40px 30px;'>
            <h2 style='color: #333333; margin: 0 0 20px 0; font-size: 24px;'>
                Email Adresinizi Doğrulayın
            </h2>
            
            <p style='color: #666666; line-height: 1.6; margin: 0 0 20px 0; font-size: 16px;'>
                Merhaba,
            </p>
            
            <p style='color: #666666; line-height: 1.6; margin: 0 0 20px 0; font-size: 16px;'>
                VetLine hesabınızı oluşturduğunuz için teşekkür ederiz! Hesabınızı aktif hale getirmek için email adresinizi doğrulamanız gerekiyor.
            </p>

            <p style='color: #666666; line-height: 1.6; margin: 0 0 30px 0; font-size: 16px;'>
                Aşağıdaki butona tıklayarak email adresinizi doğrulayabilirsiniz:
            </p>

            <!-- Verification Button -->
            <div style='text-align: center; margin: 30px 0;'>
                <a href='{verificationLink}' 
                   style='display: inline-block; background: linear-gradient(135deg, #28a745 0%, #20c997 100%); 
                          color: #ffffff; padding: 15px 30px; text-decoration: none; border-radius: 25px; 
                          font-weight: bold; font-size: 16px; box-shadow: 0 4px 8px rgba(40, 167, 69, 0.3);
                          transition: all 0.3s ease;'>
                    ✅ Email Adresimi Doğrula
                </a>
            </div>

            <p style='color: #666666; line-height: 1.6; margin: 20px 0; font-size: 14px;'>
                Butona tıklayamıyorsanız, aşağıdaki linki tarayıcınıza kopyalayabilirsiniz:
            </p>
            
            <div style='background-color: #f8f9fa; padding: 15px; border-radius: 4px; word-break: break-all; font-family: monospace; font-size: 12px; color: #495057; border-left: 4px solid #007bff;'>
                {verificationLink}
            </div>

            <div style='background-color: #fff3cd; border: 1px solid #ffeaa7; padding: 15px; border-radius: 4px; margin: 20px 0;'>
                <p style='margin: 0; color: #856404; font-size: 14px; font-weight: bold;'>
                    ⚠️ Önemli Bilgiler:
                </p>
                <ul style='margin: 10px 0 0 0; color: #856404; font-size: 14px;'>
                    <li>Bu doğrulama linki <strong>24 saat</strong> geçerlidir</li>
                    <li>Link yalnızca <strong>1 kez</strong> kullanılabilir</li>
                    <li>Eğer bu emaili siz talep etmediyseniz, güvenle göz ardı edebilirsiniz</li>
                </ul>
            </div>
        </div>

        <!-- Footer -->
        <div style='background-color: #f8f9fa; padding: 20px 30px; border-top: 1px solid #dee2e6;'>
            <p style='color: #6c757d; margin: 0; font-size: 12px; text-align: center;'>
                Bu email otomatik olarak gönderilmiştir. Lütfen yanıtlamayın.
            </p>
            <p style='color: #6c757d; margin: 5px 0 0 0; font-size: 12px; text-align: center;'>
                © 2025 VetLine - Evcil Hayvan Sağlık Sistemi
            </p>
        </div>
    </div>
</body>
</html>";
        }
    }
}
