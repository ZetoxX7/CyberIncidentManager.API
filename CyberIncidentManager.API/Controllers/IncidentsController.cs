using CyberIncidentManager.API.Data;
using CyberIncidentManager.API.Models;
using CyberIncidentManager.API.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;

namespace CyberIncidentManager.API.Controllers
{
    [Authorize]  // Toute action nécessite un token valide
    [ApiController]
    [Route("api/[controller]")]  // Route de base : /api/incidents
    public class IncidentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IncidentsController> _logger;

        public IncidentsController(ApplicationDbContext context, ILogger<IncidentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET /api/incidents
        // Retourne tous les incidents, avec leurs relations chargées  
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Incident>>> GetAll() =>
            await _context.Incidents
                .Include(i => i.Type)
                .Include(i => i.Asset)
                .Include(i => i.ReportedByUser)
                .Include(i => i.AssignedToUser)
                .ToListAsync();

        // GET /api/incidents/{id}
        // Recherche un incident par Id, charge ses relations  
        [HttpGet("{id}")]
        public async Task<ActionResult<Incident>> GetById(int id)
        {
            var incident = await _context.Incidents
                .Include(i => i.Type)
                .Include(i => i.Asset)
                .Include(i => i.ReportedByUser)
                .Include(i => i.AssignedToUser)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (incident == null)
            {
                _logger.LogWarning("Consultation d'incident inexistant : {IncidentId}", id);
                return NotFound();  // 404 si aucun incident trouvé
            }

            _logger.LogInformation("Consultation de l'incident {IncidentId}", id);
            return Ok(incident);   // 200 + payload JSON
        }

        // POST /api/incidents
        // Création autorisée pour Employé, Analyst ou Admin  
        [Authorize(Roles = "Employé,Analyst,Admin")]
        [HttpPost]
        public async Task<ActionResult<Incident>> Create([FromBody] CreateIncidentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);  // 400 si DTO invalide

            // Construction de l’entité avec encodage XSS-safe
            var incident = new Incident
            {
                Title = HtmlEncoder.Default.Encode(dto.Title),
                Description = HtmlEncoder.Default.Encode(dto.Description),
                Severity = dto.Severity,      // À valider/structurer côté serveur
                Status = dto.Status,        // Idem
                TypeId = dto.TypeId,
                AssignedTo = dto.AssignedTo,    // À vérifier : correspond à un analyst existant
                AssetId = dto.AssetId,
                ReportedBy = dto.ReportedBy,    // À extraire plutôt depuis claims JWT
                CreatedAt = DateTime.UtcNow
            };

            _context.Incidents.Add(incident);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Incident créé : {Title} par l'utilisateur {UserId}", incident.Title, dto.ReportedBy);
            return CreatedAtAction(nameof(GetById), new { id = incident.Id }, incident);  // 201
        }

        // PUT /api/incidents/{id}
        // Modification autorisée pour Analyst ou Admin  
        [Authorize(Roles = "Analyst,Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Incident incident)
        {
            if (id != incident.Id)
                return BadRequest();  // 400 si l’ID d’URL diffère de celui du corps

            // Encodage XSS-safe pour les champs texte
            incident.Title = HtmlEncoder.Default.Encode(incident.Title);
            incident.Description = HtmlEncoder.Default.Encode(incident.Description);

            _context.Entry(incident).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Incident modifié : {IncidentId}", id);
            return NoContent();  // 204
        }

        // DELETE /api/incidents/{id}
        // Suppression réservée aux Admin  
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var incident = await _context.Incidents.FindAsync(id);
            if (incident == null)
                return NotFound();  // 404 si inexistant

            _context.Incidents.Remove(incident);
            await _context.SaveChangesAsync();

            _logger.LogWarning("Incident supprimé : {IncidentId}", id);
            return NoContent();  // 204
        }
    }
}