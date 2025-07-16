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
                Name = HtmlEncoder.Default.Encode(dto.Name),
                Type = HtmlEncoder.Default.Encode(dto.Type),
                IpAddress = dto.IpAddress,
                Owner = HtmlEncoder.Default.Encode(dto.Owner),
                Location = HtmlEncoder.Default.Encode(dto.Location),
                Criticality = dto.Criticality
            };

            _context.Assets.Add(asset);
            await _context.SaveChangesAsync();

            _logger?.LogInformation("Asset créé : {Name}", asset.Name);

            return CreatedAtAction(nameof(GetById), new { id = asset.Id }, asset);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Asset asset)
        {
            if (id != asset.Id) return BadRequest();
            asset.Name = HtmlEncoder.Default.Encode(asset.Name);
            asset.Type = HtmlEncoder.Default.Encode(asset.Type);
            asset.Owner = HtmlEncoder.Default.Encode(asset.Owner);
            asset.Location = HtmlEncoder.Default.Encode(asset.Location);
            _context.Entry(asset).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            _logger?.LogInformation("Asset modifié : {Id}", id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var asset = await _context.Assets.FindAsync(id);
            if (asset == null) return NotFound();
            _context.Assets.Remove(asset);
            await _context.SaveChangesAsync();
            _logger?.LogWarning("Asset supprimé : {Id}", id);
            return NoContent();
        }
    }
}