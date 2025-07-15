using CyberIncidentManager.API.Data;
using CyberIncidentManager.API.Models;
using CyberIncidentManager.API.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CyberIncidentManager.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class IncidentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public IncidentsController(ApplicationDbContext context) => _context = context;

        // Lecture : accessible à tous les utilisateurs authentifiés
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Incident>>> GetAll() =>
            await _context.Incidents
                .Include(i => i.Type)
                .Include(i => i.Asset)
                .Include(i => i.ReportedByUser)
                .Include(i => i.AssignedToUser)
                .ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<Incident>> GetById(int id)
        {
            var incident = await _context.Incidents
                .Include(i => i.Type)
                .Include(i => i.Asset)
                .Include(i => i.ReportedByUser)
                .Include(i => i.AssignedToUser)
                .FirstOrDefaultAsync(i => i.Id == id);

            return incident == null ? NotFound() : Ok(incident);
        }

        // Création d’incidents : Employé, Analyste, Admin
        [Authorize(Roles = "Employé,Analyst,Admin")]
        [HttpPost]
        public async Task<ActionResult<Incident>> Create([FromBody] CreateIncidentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var incident = new Incident
            {
                Title = dto.Title,
                Description = dto.Description,
                Severity = dto.Severity,
                Status = dto.Status,
                TypeId = dto.TypeId,
                AssignedTo = dto.AssignedTo,
                AssetId = dto.AssetId,
                ReportedBy = dto.ReportedBy,
                CreatedAt = DateTime.UtcNow
            };

            _context.Incidents.Add(incident);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = incident.Id }, incident);
        }

        // Modification : Analyste ou Admin
        [Authorize(Roles = "Analyst,Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Incident incident)
        {
            if (id != incident.Id) return BadRequest();
            _context.Entry(incident).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Suppression : Admin uniquement
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var incident = await _context.Incidents.FindAsync(id);
            if (incident == null) return NotFound();
            _context.Incidents.Remove(incident);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}