using CyberIncidentManager.API.Data;
using CyberIncidentManager.API.Models;
using CyberIncidentManager.API.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;

namespace CyberIncidentManager.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(ApplicationDbContext context, ILogger<NotificationsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notification>>> GetAll() =>
            await _context.Notifications
                .Include(n => n.User)
                .Include(n => n.Incident)
                .ToListAsync();

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Notification>>> GetByUserId(int userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return Ok(notifications);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Notification>> Create([FromBody] CreateNotificationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var notification = new Notification
            {
                UserId = dto.UserId,
                IncidentId = dto.IncidentId,
                Title = HtmlEncoder.Default.Encode(dto.Title),
                Message = HtmlEncoder.Default.Encode(dto.Message),
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            _logger?.LogInformation("Notification créée pour utilisateur {UserId}", dto.UserId);

            return CreatedAtAction(nameof(GetById), new { id = notification.Id }, notification);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Notification>> GetById(int id)
        {
            var notification = await _context.Notifications
                .Include(n => n.User)
                .Include(n => n.Incident)
                .FirstOrDefaultAsync(n => n.Id == id);

            return notification == null ? NotFound() : Ok(notification);
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null) return NotFound();

            notification.IsRead = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null) return NotFound();
            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
            _logger?.LogWarning("Notification supprimée : {Id}", id);
            return NoContent();
        }
    }
}