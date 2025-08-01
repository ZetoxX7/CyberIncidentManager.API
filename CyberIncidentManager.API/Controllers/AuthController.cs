﻿using CyberIncidentManager.API.Data;
using CyberIncidentManager.API.Models;
using CyberIncidentManager.API.Models.Auth;
using CyberIncidentManager.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace CyberIncidentManager.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly TokenService _tokenService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            ApplicationDbContext context,
            TokenService tokenService,
            IMemoryCache cache,
            ILogger<AuthController> logger)
        {
            _context = context;
            _tokenService = tokenService;
            _cache = cache;
            _logger = logger;
        }

        // POST api/auth/register
        // Inscription d'un nouvel utilisateur
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthRequest request)
        {
            // Vérifie unicité de l'email
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return BadRequest("Email déjà utilisé.");

            // Vérifie complexité du mot de passe
            if (!IsPasswordStrong(request.Password))
            {
                _logger.LogWarning("Inscription refusée pour {Email} (mot de passe faible)", request.Email);
                return BadRequest(
                    "Le mot de passe doit contenir au moins 8 caractères, " +
                    "dont une majuscule, une minuscule, un chiffre et un caractère spécial."
                );
            }

            // Vérifie si le rôle existe
            var roleExists = await _context.Roles.AnyAsync(r => r.Id == request.RoleId);
            if (!roleExists)
                return BadRequest("Rôle spécifié invalide.");

            // Crée l'utilisateur avec les données complètes
            var user = new User
            {
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                RoleId = request.RoleId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Nouvel utilisateur inscrit : {Email}", user.Email);
            return Ok("Inscription réussie !");
        }

        // POST api/auth/login
        // Authentification : renvoie JWT ou déclenche MFA pour Admin
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] AuthLoginRequest request)
        {
            // Recherche l'utilisateur et vérifie le mot de passe
            var user = await _context.Users
                                     .Include(u => u.Role)
                                     .FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Tentative de connexion échouée pour {Email}", request.Email);
                return Unauthorized("Identifiants invalides.");
            }
            _logger.LogInformation("Connexion réussie pour {Email}", request.Email);

            // Si rôle Admin, exige verification MFA
            // Supprimer ce bloc :
            /*
            if (user.Role.Name == "Admin")
            {
                var mfaCode = new Random().Next(100000, 999999).ToString();
                _cache.Set($"mfa_{user.Id}", mfaCode, TimeSpan.FromMinutes(5));
                await _emailService.SendAsync(user.Email, "Votre code MFA", $"Votre code : {mfaCode}");
                _logger.LogInformation("MFA envoyé à {Email}", user.Email);
                return Ok("MFA_REQUIRED");
            }
            */

            // Génère et stocke tokens pour les autres rôles
            var accessToken = _tokenService.GenerateJwtToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var rt = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            _context.RefreshTokens.Add(rt);
            await _context.SaveChangesAsync();

            return Ok(new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Expiration = DateTime.UtcNow.AddMinutes(30)
            });
        }

        // POST api/auth/refresh
        // Rafraîchit l'access token à partir d'un refresh token valide
        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponse>> Refresh([FromBody] string refreshToken)
        {
            var storedToken = await _context.RefreshTokens
                .Include(r => r.User)
                    .ThenInclude(u => u.Role)
                .FirstOrDefaultAsync(t => t.Token == refreshToken && !t.IsRevoked);

            if (storedToken == null || storedToken.ExpiresAt < DateTime.UtcNow)
                return Unauthorized("Refresh token invalide ou expiré.");

            // Révoque l'ancien token
            storedToken.IsRevoked = true;

            // Génère de nouveaux tokens
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

        // POST api/auth/logout
        // Révoque tous les refresh tokens de l'utilisateur connecté
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var tokens = await _context.RefreshTokens
                                       .Where(t => t.UserId == userId && !t.IsRevoked)
                                       .ToListAsync();

            foreach (var token in tokens)
                token.IsRevoked = true;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Déconnexion de l'utilisateur {UserId}", userId);
            return Ok("Déconnexion réussie.");
        }

        // Vérifie la robustesse du mot de passe
        private bool IsPasswordStrong(string password)
        {
            const string specials = "!@#$%^&*()_+-=[]{}|;:',.<>/?";
            return password.Length >= 8
                && password.Any(char.IsUpper)
                && password.Any(char.IsLower)
                && password.Any(char.IsDigit)
                && password.Any(ch => specials.Contains(ch));
        }
    }
}