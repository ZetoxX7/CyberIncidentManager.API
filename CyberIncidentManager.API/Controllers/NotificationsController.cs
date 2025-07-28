using CyberIncidentManager.API.Data;
using CyberIncidentManager.API.Models;
using CyberIncidentManager.API.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;

namespace CyberIncidentManager.API.Controllers
{
    [Authorize]                                   // Tout utilisateur authentifié peut accéder
    [ApiController]
    [Route("api/notifications")]                   // Route : api/notifications
    public class NotificationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(ApplicationDbContext context, ILogger<NotificationsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET api/notifications
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notification>>> GetAll() =>
            // Liste toutes les notifications avec utilisateur et incident associés
            await _context.Notifications
                          .Include(n => n.User)
                          .Include(n => n.Incident)
                          .ToListAsync();

        // GET api/notifications/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Notification>>> GetByUserId(int userId)
        {
            // Récupère les notifications d’un utilisateur, triées par date décroissante
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return Ok(notifications);               // 200 + liste JSON
        }

        // POST api/notifications
        [HttpPost]
        public async Task<ActionResult<Notification>> Create([FromBody] CreateNotificationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);       // 400 si DTO invalide

            // Construction de la notification
            var notification = new Notification
            {
                UserId = dto.UserId,            // → Récupérer l’ID depuis JWT plutôt que du client
                IncidentId = dto.IncidentId,
                Title = HtmlEncoder.Default.Encode(dto.Title),
                Message = HtmlEncoder.Default.Encode(dto.Message),
                IsRead = false,                 // Par défaut non lue
                CreatedAt = DateTime.UtcNow        // Horodatage en UTC
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Notification créée pour utilisateur {UserId}", dto.UserId);

            // 201 Created avec URL pour récupérer la ressource
            return CreatedAtAction(nameof(GetById), new { id = notification.Id }, notification);
        }

        // GET api/notifications/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Notification>> GetById(int id)
        {
            var notification = await _context.Notifications
                .Include(n => n.User)
                .Include(n => n.Incident)
                .FirstOrDefaultAsync(n => n.Id == id);

            return notification == null
                ? NotFound()                       // 404 si non trouvé
                : Ok(notification);                // 200 + objet JSON
        }

        // PUT api/notifications/{id}/read
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
                return NotFound();                // 404 si non trouvé

            notification.IsRead = true;            // Marque comme lue
            await _context.SaveChangesAsync();
            return NoContent();                    // 204 No Content
        }

        // DELETE api/notifications/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
                return NotFound();                // 404 si non trouvé

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();

            _logger.LogWarning("Notification supprimée : {Id}", id);
            return NoContent();                    // 204 No Content
        }
    }
}