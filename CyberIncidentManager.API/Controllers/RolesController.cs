using CyberIncidentManager.API.Data;
using CyberIncidentManager.API.Models;
using CyberIncidentManager.API.Models.DTOs.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;

namespace CyberIncidentManager.API.Controllers
{
    [Authorize(Roles = "Admin")]                 // Limite l’accès aux administrateurs uniquement
    [ApiController]
    [Route("api/roles")]                  // Route = api/roles
    public class RolesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RolesController> _logger;

        public RolesController(ApplicationDbContext context, ILogger<RolesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET api/roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> GetAll() =>
            // Inclut la liste des utilisateurs attachés à chaque rôle
            await _context.Roles
                          .Include(r => r.Users)
                          .ToListAsync();

        // GET api/roles/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Role>> GetById(int id)
        {
            var role = await _context.Roles
                                     .Include(r => r.Users)
                                     .FirstOrDefaultAsync(r => r.Id == id);
            return role == null
                ? NotFound()   // 404 si le rôle n’existe pas
                : Ok(role);    // 200 + rôle JSON
        }

        // POST api/roles
        [HttpPost]
        public async Task<ActionResult<Role>> Create([FromBody] CreateRoleDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);    // 400 si DTO invalide

            // Encodage XSS-safe des champs
            var role = new Role
            {
                Name = HtmlEncoder.Default.Encode(dto.Name),
                Description = HtmlEncoder.Default.Encode(dto.Description),
                Permissions = dto.Permissions   // chaîne brute : envisager structuration (enum or flags)
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Rôle créé : {Name}", role.Name);

            // 201 avec URL de récupération
            return CreatedAtAction(nameof(GetById), new { id = role.Id }, role);
        }

        // PUT api/roles/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Role role)
        {
            if (id != role.Id)
                return BadRequest();             // 400 si l’ID de l’URL ne correspond pas

            // Encodage XSS-safe
            role.Name = HtmlEncoder.Default.Encode(role.Name);
            role.Description = HtmlEncoder.Default.Encode(role.Description);

            _context.Entry(role).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Rôle modifié : {Id}", id);
            return NoContent();                  // 204 No Content
        }

        // DELETE api/roles/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
                return NotFound();               // 404 si absence

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            _logger.LogWarning("Rôle supprimé : {Id}", id);
            return NoContent();                  // 204 No Content
        }
    }
}