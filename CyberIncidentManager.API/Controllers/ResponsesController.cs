using CyberIncidentManager.API.Data;
using CyberIncidentManager.API.Models;
using CyberIncidentManager.API.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;

namespace CyberIncidentManager.API.Controllers
{
    [Authorize(Roles = "Admin,Analyst")]   // Accès réservé aux rôles Admin et Analyst
    [ApiController]
    [Route("api/responses")]            // Route : api/responses
    public class ResponsesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Injection du DbContext via constructeur
        public ResponsesController(ApplicationDbContext context) =>
            _context = context;

        // GET api/responses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Response>>> GetAll() =>
            // Renvoie toutes les réponses avec utilisateur et incident associés
            await _context.Responses
                          .Include(r => r.User)
                          .Include(r => r.Incident)
                          .ToListAsync();

        // GET api/responses/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Response>> GetById(int id)
        {
            var response = await _context.Responses
                                         .Include(r => r.User)
                                         .Include(r => r.Incident)
                                         .FirstOrDefaultAsync(r => r.Id == id);

            if (response == null)
                return NotFound();           // 404 si non trouvé

            return Ok(response);             // 200 + objet JSON
        }

        // POST api/responses
        [HttpPost]
        public async Task<ActionResult<Response>> Create([FromBody] CreateResponseDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // 400 si DTO invalide

            // Encodage pour éviter les injections XSS dans les champs texte
            var response = new Response
            {
                IncidentId = dto.IncidentId,
                UserId = dto.UserId,     // À valider côté serveur : comparer avec claims JWT
                Action = HtmlEncoder.Default.Encode(dto.Action),
                Details = HtmlEncoder.Default.Encode(dto.Details),
                Timestamp = DateTime.UtcNow, // Horodatage en UTC
                IsSuccessful = dto.IsSuccessful
            };

            _context.Responses.Add(response);
            await _context.SaveChangesAsync();

            // 201 Created avec URL pour récupérer la nouvelle ressource
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        // PUT api/responses/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Response response)
        {
            if (id != response.Id)
                return BadRequest();          // 400 si l’ID dans l’URL diffère du corps

            // Ici on ne ré-encode pas les champs texte : prévoir HTML encoding si nécessaire
            _context.Entry(response).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();               // 204 No Content
        }

        // DELETE api/responses/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _context.Responses.FindAsync(id);
            if (response == null)
                return NotFound();            // 404 si non trouvé

            _context.Responses.Remove(response);
            await _context.SaveChangesAsync();

            return NoContent();               // 204 No Content
        }
    }
}
