using System.ComponentModel.DataAnnotations;

namespace CyberIncidentManager.API.Models.DTOs.Role
{
    public class CreateRoleDto
    {
        [Required(ErrorMessage = "Le nom du rôle est requis.")]
        [MaxLength(50, ErrorMessage = "Le nom ne doit pas dépasser 50 caractères.")]
        public string Name { get; set; }
        // Nom du rôle (ex. “Admin”, “User”), obligatoire, ≤ 50 caractères

        [MaxLength(200, ErrorMessage = "La description ne doit pas dépasser 200 caractères.")]
        public string Description { get; set; }
        // Description optionnelle du rôle, ≤ 200 caractères

        [Required(ErrorMessage = "Les permissions sont requises.")]
        public string Permissions { get; set; }
        // Permissions sous forme de chaîne (ex. “Create,Read,Update,Delete”), obligatoire
        // → Privilégier éventuellement une structure plus forte (List<string> ou enum flags)
    }
}