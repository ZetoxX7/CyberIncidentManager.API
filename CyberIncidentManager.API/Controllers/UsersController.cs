using CyberIncidentManager.API.Data;
using CyberIncidentManager.API.Models;
using CyberIncidentManager.API.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;

namespace CyberIncidentManager.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UsersController> _logger;

        public UsersController(ApplicationDbContext context, ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            var users = await _context.Users.Include(u => u.Role).ToListAsync();
            _logger.LogInformation("Liste des utilisateurs consultée par {Admin}", User.Identity?.Name);
            return users;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                _logger.LogWarning("Consultation d'utilisateur inexistant : {UserId}", id);
                return NotFound();
            }
            _logger.LogInformation("Consultation de l'utilisateur {UserId} par {Admin}", id, User.Identity?.Name);
            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> Create([FromBody] CreateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!IsPasswordStrong(dto.Password))
            {
                _logger.LogWarning("Création refusée pour {Email} (mot de passe faible)", dto.Email);
                return BadRequest("Le mot de passe doit contenir au moins 8 caractères, une majuscule, une minuscule, un chiffre et un caractère spécial.");
            }

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

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, User user)
        {
            if (id != user.Id) return BadRequest();

            user.FirstName = HtmlEncoder.Default.Encode(user.FirstName);
            user.LastName = HtmlEncoder.Default.Encode(user.LastName);
            user.Email = HtmlEncoder.Default.Encode(user.Email);

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Utilisateur modifié : {UserId} par {Admin}", id, User.Identity?.Name);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                _logger.LogWarning("Suppression d'utilisateur inexistant : {UserId}", id);
                return NotFound();
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            _logger.LogWarning("Utilisateur supprimé : {UserId} par {Admin}", id, User.Identity?.Name);
            return NoContent();
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