using CyberIncidentManager.API.Data;
using CyberIncidentManager.API.Models;
using CyberIncidentManager.API.Models.DTOs.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;

namespace CyberIncidentManager.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RolesController> _logger;

        public RolesController(ApplicationDbContext context, ILogger<RolesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> GetAll() =>
            await _context.Roles.Include(r => r.Users).ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<Role>> GetById(int id)
        {
            var role = await _context.Roles.Include(r => r.Users).FirstOrDefaultAsync(r => r.Id == id);
            return role == null ? NotFound() : Ok(role);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Role>> Create([FromBody] CreateRoleDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var role = new Role
            {
                Name = HtmlEncoder.Default.Encode(dto.Name),
                Description = HtmlEncoder.Default.Encode(dto.Description),
                Permissions = dto.Permissions
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            _logger?.LogInformation("Rôle créé : {Name}", role.Name);

            return CreatedAtAction(nameof(GetById), new { id = role.Id }, role);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Role role)
        {
            if (id != role.Id) return BadRequest();
            role.Name = HtmlEncoder.Default.Encode(role.Name);
            role.Description = HtmlEncoder.Default.Encode(role.Description);
            _context.Entry(role).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            _logger?.LogInformation("Rôle modifié : {Id}", id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null) return NotFound();
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            _logger?.LogWarning("Rôle supprimé : {Id}", id);
            return NoContent();
        }
    }
}