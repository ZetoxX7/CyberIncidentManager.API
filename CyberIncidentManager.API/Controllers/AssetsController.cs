using CyberIncidentManager.API.Data;
using CyberIncidentManager.API.Models;
using CyberIncidentManager.API.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CyberIncidentManager.API.Controllers
{
    [Authorize(Roles = "Admin,Analyst")]
    [ApiController]
    [Route("api/[controller]")]
    public class AssetsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public AssetsController(ApplicationDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Asset>>> GetAll() =>
            await _context.Assets.ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<Asset>> GetById(int id)
        {
            var asset = await _context.Assets.FindAsync(id);
            return asset == null ? NotFound() : Ok(asset);
        }

        [Authorize(Roles = "Admin,Analyst")]
        [HttpPost]
        public async Task<ActionResult<Asset>> Create([FromBody] CreateAssetDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var asset = new Asset
            {
                Name = dto.Name,
                Type = dto.Type,
                IpAddress = dto.IpAddress,
                Owner = dto.Owner,
                Location = dto.Location,
                Criticality = dto.Criticality
            };

            _context.Assets.Add(asset);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = asset.Id }, asset);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Asset asset)
        {
            if (id != asset.Id) return BadRequest();
            _context.Entry(asset).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var asset = await _context.Assets.FindAsync(id);
            if (asset == null) return NotFound();
            _context.Assets.Remove(asset);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}