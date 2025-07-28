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
    // Seuls les Admins et Analysts peuvent accéder à l’ensemble des endpoints
    [ApiController]
    [Route("api/assets")]
    // Route de base : /api/assets
    public class AssetsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AssetsController> _logger;

        public AssetsController(ApplicationDbContext context, ILogger<AssetsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET /api/assets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Asset>>> GetAll() =>
            // Retourne la liste complète des assets
            await _context.Assets.ToListAsync();

        // GET /api/assets/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Asset>> GetById(int id)
        {
            var asset = await _context.Assets.FindAsync(id);
            // 404 si inexistant, sinon 200 + asset
            return asset == null ? NotFound() : Ok(asset);
        }

        // POST /api/assets
        [HttpPost]
        public async Task<ActionResult<Asset>> Create([FromBody] CreateAssetDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            // 400 si DTO invalide

            // Construction de l’entité avec encodage XSS-safe pour les champs textes
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

            _logger.LogInformation("Asset créé : {Name}", asset.Name);

            // 201 Created avec URL pour récupérer la ressource
            return CreatedAtAction(nameof(GetById), new { id = asset.Id }, asset);
        }

        // PUT /api/assets/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Asset asset)
        {
            if (id != asset.Id)
                return BadRequest();
            // 400 si l’ID d’URL diffère de celui du corps

            // Encodage XSS-safe pour les champs modifiables
            asset.Name = HtmlEncoder.Default.Encode(asset.Name);
            asset.Type = HtmlEncoder.Default.Encode(asset.Type);
            asset.Owner = HtmlEncoder.Default.Encode(asset.Owner);
            asset.Location = HtmlEncoder.Default.Encode(asset.Location);

            _context.Entry(asset).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Asset modifié : {Id}", id);
            return NoContent();
            // 204 No Content
        }

        // DELETE /api/assets/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var asset = await _context.Assets.FindAsync(id);
            if (asset == null)
                return NotFound();
            // 404 si inexistant

            _context.Assets.Remove(asset);
            await _context.SaveChangesAsync();

            _logger.LogWarning("Asset supprimé : {Id}", id);
            return NoContent();
            // 204 No Content
        }
    }
}