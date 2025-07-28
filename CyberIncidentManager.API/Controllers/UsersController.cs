using CyberIncidentManager.API.Data;
using CyberIncidentManager.API.Models;
using CyberIncidentManager.API.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;

namespace CyberIncidentManager.API.Controllers
{
    [Authorize(Roles = "Admin")]                   // Restreint l’accès aux administrateurs
    [ApiController]
    [Route("api/users")]                    // Route : api/users
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UsersController> _logger;

        public UsersController(ApplicationDbContext context, ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            // Charge les utilisateurs avec leur rôle associé
            var users = await _context.Users
                                      .Include(u => u.Role)
                                      .ToListAsync();
            _logger.LogInformation("Liste des utilisateurs consultée par {Admin}", User.Identity?.Name);
            return users;                         // Renvoie 200 + liste JSON
        }

        // GET api/users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            var user = await _context.Users
                                     .Include(u => u.Role)
                                     .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                _logger.LogWarning("Consultation d'utilisateur inexistant : {UserId}", id);
                return NotFound();               // 404 si pas trouvé
            }
            _logger.LogInformation("Consultation de l'utilisateur {UserId} par {Admin}", id, User.Identity?.Name);
            return Ok(user);                     // 200 + objet JSON
        }

        // POST api/users
        [HttpPost]
        public async Task<ActionResult<User>> Create([FromBody] CreateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);    // 400 si DTO invalide

            // Vérifie la robustesse du mot de passe
            if (!IsPasswordStrong(dto.Password))
            {
                _logger.LogWarning("Création refusée pour {Email} (mot de passe faible)", dto.Email);
                return BadRequest(
                    "Le mot de passe doit contenir au moins 8 caractères, une majuscule, une minuscule, un chiffre et un caractère spécial."
                );
            }

            // Encodage pour éviter les injections XSS
            var user = new User
            {
                Email = HtmlEncoder.Default.Encode(dto.Email),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FirstName = HtmlEncoder.Default.Encode(dto.FirstName),
                LastName = HtmlEncoder.Default.Encode(dto.LastName),
                RoleId = dto.RoleId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Utilisateur créé : {Email} par {Admin}", user.Email, User.Identity?.Name);

            // Renvoie 201 avec l’URL de récupération
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        // PUT api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, User user)
        {
            if (id != user.Id)
                return BadRequest();             // 400 si l’ID de l’URL diffère de celui du corps

            // Encodage pour éviter les injections XSS
            user.FirstName = HtmlEncoder.Default.Encode(user.FirstName);
            user.LastName = HtmlEncoder.Default.Encode(user.LastName);
            user.Email = HtmlEncoder.Default.Encode(user.Email);

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Utilisateur modifié : {UserId} par {Admin}", id, User.Identity?.Name);
            return NoContent();                  // 204 No Content
        }

        // DELETE api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                _logger.LogWarning("Suppression d'utilisateur inexistant : {UserId}", id);
                return NotFound();               // 404 si pas trouvé
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            _logger.LogWarning("Utilisateur supprimé : {UserId} par {Admin}", id, User.Identity?.Name);
            return NoContent();                  // 204 No Content
        }

        // Vérifie que le mot de passe contient min. 8 chars, majuscule, minuscule, chiffre et caractère spécial
        private bool IsPasswordStrong(string password)
        {
            const string specialChars = "!@#$%^&*()_+-=[]{}|;:',.<>/?";
            return password.Length >= 8
                && password.Any(char.IsUpper)
                && password.Any(char.IsLower)
                && password.Any(char.IsDigit)
                && password.Any(ch => specialChars.Contains(ch));
        }
    }
}