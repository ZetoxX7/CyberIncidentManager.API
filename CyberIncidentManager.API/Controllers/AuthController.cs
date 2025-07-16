using CyberIncidentManager.API.Data;
using CyberIncidentManager.API.Models;
using CyberIncidentManager.API.Models.Auth;
using CyberIncidentManager.API.Models.DTOs;
using CyberIncidentManager.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace CyberIncidentManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly TokenService _tokenService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<AuthController> _logger;
        private readonly EmailService _emailService;

        public AuthController(ApplicationDbContext context, TokenService tokenService, IMemoryCache cache, ILogger<AuthController> logger, EmailService emailService)
        {
            _context = context;
            _tokenService = tokenService;
            _cache = cache;
            _logger = logger;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return BadRequest("Email déjà utilisé.");

            if (!IsPasswordStrong(request.Password))
                return BadRequest("Le mot de passe doit contenir au moins 8 caractères, dont une majuscule, une minuscule, un chiffre et un caractère spécial.");

            var user = new User
            {
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FirstName = "Nouvel",
                LastName = "Utilisateur",
                RoleId = 3, // Rôle par défaut : Employé
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Inscription réussie !");
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] AuthRequest request)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Tentative de connexion échouée pour {Email}", request.Email);
                return Unauthorized("Identifiants invalides.");
            }
            else
            {
                _logger.LogInformation("Connexion réussie pour {Email}", request.Email);
            }

            if (user.Role.Name == "Admin")
            {
                var mfaCode = new Random().Next(100000, 999999).ToString();
                _cache.Set($"mfa_{user.Id}", mfaCode, TimeSpan.FromMinutes(5));
                await _emailService.SendAsync(user.Email, "Votre code MFA", $"Votre code : {mfaCode}");
                _logger.LogInformation("MFA envoyé à {Email}", user.Email);
                return Ok("MFA_REQUIRED");
            }

            var accessToken = _tokenService.GenerateJwtToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var rt = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            _context.Add(rt);
            await _context.SaveChangesAsync();

            return Ok(new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Expiration = DateTime.UtcNow.AddMinutes(30)
            });
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponse>> Refresh([FromBody] string refreshToken)
        {
            var storedToken = await _context.Set<RefreshToken>()
                .Include(r => r.User)
                .ThenInclude(u => u.Role)
                .FirstOrDefaultAsync(t => t.Token == refreshToken && !t.IsRevoked);

            if (storedToken == null || storedToken.ExpiresAt < DateTime.UtcNow)
                return Unauthorized("Refresh token invalide ou expiré.");

            storedToken.IsRevoked = true;

            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var newJwt = _tokenService.GenerateJwtToken(storedToken.User);

            var newStored = new RefreshToken
            {
                UserId = storedToken.UserId,
                Token = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            _context.RefreshTokens.Add(newStored);
            await _context.SaveChangesAsync();

            return Ok(new AuthResponse
            {
                AccessToken = newJwt,
                RefreshToken = newRefreshToken,
                Expiration = DateTime.UtcNow.AddMinutes(30)
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var tokens = await _context.Set<RefreshToken>()
                .Where(t => t.UserId == userId && !t.IsRevoked)
                .ToListAsync();

            foreach (var token in tokens)
                token.IsRevoked = true;

            await _context.SaveChangesAsync();
            return Ok("Déconnexion réussie.");
        }

        [HttpPost("verify-mfa")]
        public async Task<IActionResult> VerifyMfa([FromBody] MfaRequest dto)
        {
            var mfaCode = _cache.Get<string>($"mfa_{dto.UserId}");
            if (mfaCode == null || mfaCode != dto.Code)
            {
                _logger.LogWarning("MFA invalide pour l'utilisateur {UserId}", dto.UserId);
                return Unauthorized("Code MFA invalide.");
            }
            _cache.Remove($"mfa_{dto.UserId}");

            var user = await _context.Users.FindAsync(dto.UserId);
            var accessToken = _tokenService.GenerateJwtToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var rt = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            _context.Add(rt);
            await _context.SaveChangesAsync();

            _logger.LogInformation("MFA validé pour l'utilisateur {UserId}", dto.UserId);
            return Ok(new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Expiration = DateTime.UtcNow.AddMinutes(30)
            });
        }

        private bool IsPasswordStrong(string password)
        {
            return password.Length >= 8
                && password.Any(char.IsUpper)
                && password.Any(char.IsLower)
                && password.Any(char.IsDigit)
                && password.Any(ch => "!@#$%^&*()_+-=[]{}|;:',.<>/?".Contains(ch));
        }
    }
}