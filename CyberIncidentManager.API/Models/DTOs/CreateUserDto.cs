using System.ComponentModel.DataAnnotations;

namespace CyberIncidentManager.API.Models.DTOs
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "L'email est requis.")]
        [EmailAddress(ErrorMessage = "Format d'email invalide.")]
        [MaxLength(100, ErrorMessage = "L'email ne doit pas dépasser 100 caractères.")]
        public string Email { get; set; }
        // Adresse e‑mail de l’utilisateur : obligatoire, validée, ≤ 100 caractères

        [Required(ErrorMessage = "Le mot de passe est requis.")]
        [MinLength(8, ErrorMessage = "Le mot de passe doit contenir au moins 8 caractères.")]
        [MaxLength(100, ErrorMessage = "Le mot de passe ne doit pas dépasser 100 caractères.")]
        public string Password { get; set; }
        // Mot de passe en clair (sera hashé ensuite) : entre 8 et 100 caractères

        [Required(ErrorMessage = "Le prénom est requis.")]
        [MaxLength(50, ErrorMessage = "Le prénom ne doit pas dépasser 50 caractères.")]
        public string FirstName { get; set; }
        // Prénom de l’utilisateur : obligatoire, ≤ 50 caractères

        [Required(ErrorMessage = "Le nom est requis.")]
        [MaxLength(50, ErrorMessage = "Le nom ne doit pas dépasser 50 caractères.")]
        public string LastName { get; set; }
        // Nom de famille : obligatoire, ≤ 50 caractères

        [Required(ErrorMessage = "Le rôle est requis.")]
        public int RoleId { get; set; }
        // Identifiant du rôle : clé étrangère, sera utilisé pour associer un rôle existant
    }
}