using System.ComponentModel.DataAnnotations;

namespace CyberIncidentManager.API.Models.DTOs
{
    public class MfaRequest
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string Code { get; set; }
    }
}
