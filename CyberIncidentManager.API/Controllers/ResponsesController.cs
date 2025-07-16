using CyberIncidentManager.API.Data;
using CyberIncidentManager.API.Models;
using CyberIncidentManager.API.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;

namespace CyberIncidentManager.API.Controllers
{
    [Authorize(Roles = "Admin,Analyst")]
    [ApiController]
    [Route("api/[controller]")]
    public class ResponsesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ResponsesController(ApplicationDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Response>>> GetAll() =>
            await _context.Responses
                .Include(r => r.User)
                .Include(r => r.Incident)
                .ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<Response>> GetById(int id)
        {
            var response = await _context.Responses
                .Include(r => r.User)
                .Include(r => r.Incident)
                .FirstOrDefaultAsync(r => r.Id == id);

            return response == null ? NotFound() : Ok(response);
        }

        [Authorize(Roles = "Admin,Analyst")]
        [HttpPost]
        public async Task<ActionResult<Response>> Create([FromBody] CreateResponseDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = new Response
            {
                IncidentId = dto.IncidentId,
                UserId = dto.UserId,
                Action = HtmlEncoder.Default.Encode(dto.Action),
                Details = HtmlEncoder.Default.Encode(dto.Details),
                Timestamp = DateTime.UtcNow,
                IsSuccessful = dto.IsSuccessful
            };

            _context.Responses.Add(response);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Response response)
        {
            if (id != response.Id) return BadRequest();
            _context.Entry(response).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _context.Responses.FindAsync(id);
            if (response == null) return NotFound();
            _context.Responses.Remove(response);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}