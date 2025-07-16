using System.ComponentModel.DataAnnotations;

namespace CyberIncidentManager.API.Models.DTOs.Role
{
    public class CreateRoleDto
    {
        [Required(ErrorMessage = "Le nom du rôle est requis.")]
        [MaxLength(50, ErrorMessage = "Le nom ne doit pas dépasser 50 caractères.")]
        public string Name { get; set; }

        [MaxLength(200, ErrorMessage = "La description ne doit pas dépasser 200 caractères.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Les permissions sont requises.")]
        public string Permissions { get; set; }
    }
}