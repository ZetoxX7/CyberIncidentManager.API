using System.ComponentModel.DataAnnotations;

namespace CyberIncidentManager.API.Models.DTOs.Role
{
    public class CreateRoleDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        [Required]
        public string Permissions { get; set; }
    }
}