using CyberIncidentManager.API.Data;
using CyberIncidentManager.API.Models;
using CyberIncidentManager.API.Models.DTOs.IncidentType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;

namespace CyberIncidentManager.API.Controllers
{
    [Authorize]                                  // Toute requête nécessite un utilisateur authentifié
    [ApiController]
    [Route("api/incidenttypes")]                  // Route de base : api/incidenttypes
    public class IncidentTypesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IncidentTypesController> _logger;

        // Injection du DbContext et du logger
        public IncidentTypesController(ApplicationDbContext context, ILogger<IncidentTypesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET api/incidenttypes
        // Accessible à tout utilisateur authentifié
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IncidentType>>> GetAll() =>
            await _context.IncidentTypes.ToListAsync();

        // GET api/incidenttypes/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<IncidentType>> GetById(int id)
        {
            var type = await _context.IncidentTypes.FindAsync(id);
            return type == null
                ? NotFound()                  // 404 si le type n’existe pas
                : Ok(type);                   // 200 + type JSON
        }

        // POST api/incidenttypes
        // Création réservée aux Admins
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<IncidentType>> Create([FromBody] CreateIncidentTypeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);    // 400 si DTO invalide

            // Construction de l’entité avec encodage XSS-safe
            var type = new IncidentType
            {
                Name = HtmlEncoder.Default.Encode(dto.Name),
                Description = HtmlEncoder.Default.Encode(dto.Description),
                DefaultSeverity = dto.DefaultSeverity,  // À valider / structurer côté serveur
                Color = dto.Color             // À valider (format hex) côté serveur
            };

            _context.IncidentTypes.Add(type);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Type d'incident créé : {Name}", type.Name);

            // 201 Created + URL de récupération
            return CreatedAtAction(nameof(GetById), new { id = type.Id }, type);
        }

        // PUT api/incidenttypes/{id}
        // Modification réservée aux Admins
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, IncidentType type)
        {
            if (id != type.Id)
                return BadRequest();               // 400 si l’ID de l’URL ne correspond pas au corps

            // Encodage XSS-safe des champs modifiables
            type.Name = HtmlEncoder.Default.Encode(type.Name);
            type.Description = HtmlEncoder.Default.Encode(type.Description);

            _context.Entry(type).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Type d'incident modifié : {Id}", id);
            return NoContent();                    // 204 No Content
        }

        // DELETE api/incidenttypes/{id}
        // Suppression réservée aux Admins
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var type = await _context.IncidentTypes.FindAsync(id);
            if (type == null)
                return NotFound();                // 404 si inexistant

            _context.IncidentTypes.Remove(type);
            await _context.SaveChangesAsync();

            _logger.LogWarning("Type d'incident supprimé : {Id}", id);
            return NoContent();                   // 204 No Content
        }
    }
}