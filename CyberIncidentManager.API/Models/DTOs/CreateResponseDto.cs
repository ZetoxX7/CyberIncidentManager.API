using System.ComponentModel.DataAnnotations;

namespace CyberIncidentManager.API.Models.DTOs
{
    public class CreateResponseDto
    {
        [Required]
        public int IncidentId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Action { get; set; }

        [MaxLength(500)]
        public string Details { get; set; }

        [Required]
        public bool IsSuccessful { get; set; }
    }
}