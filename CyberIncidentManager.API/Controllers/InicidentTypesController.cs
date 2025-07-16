using CyberIncidentManager.API.Data;
using CyberIncidentManager.API.Models;
using CyberIncidentManager.API.Models.DTOs.IncidentType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;

namespace CyberIncidentManager.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class IncidentTypesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public IncidentTypesController(ApplicationDbContext context) => _context = context;

        // Lecture : tous les utilisateurs connectés
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IncidentType>>> GetAll() =>
            await _context.IncidentTypes.ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<IncidentType>> GetById(int id)
        {
            var type = await _context.IncidentTypes.FindAsync(id);
            return type == null ? NotFound() : Ok(type);
        }

        // Création : Admin uniquement
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<IncidentType>> Create([FromBody] CreateIncidentTypeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var type = new IncidentType
            {
                Name = HtmlEncoder.Default.Encode(dto.Name),
                Description = HtmlEncoder.Default.Encode(dto.Description),
                DefaultSeverity = dto.DefaultSeverity,
                Color = dto.Color
            };

            _context.IncidentTypes.Add(type);
            await _context.SaveChangesAsync();

            _logger?.LogInformation("Type d'incident créé : {Name}", type.Name);

            return CreatedAtAction(nameof(GetById), new { id = type.Id }, type);
        }

        // Modification : Admin uniquement
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, IncidentType type)
        {
            if (id != type.Id) return BadRequest();
            type.Name = HtmlEncoder.Default.Encode(type.Name);
            type.Description = HtmlEncoder.Default.Encode(type.Description);
            _context.Entry(type).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            _logger?.LogInformation("Type d'incident modifié : {Id}", id);
            return NoContent();
        }

        // Suppression : Admin uniquement
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var type = await _context.IncidentTypes.FindAsync(id);
            if (type == null) return NotFound();
            _context.IncidentTypes.Remove(type);
            await _context.SaveChangesAsync();
            _logger?.LogWarning("Type d'incident supprimé : {Id}", id);
            return NoContent();
        }
    }
}