using System.ComponentModel.DataAnnotations;

namespace CyberIncidentManager.API.Models
{
    public class Role
    {
        public int Id { get; set; }
        // Clé primaire auto‑incrémentée

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        // Nom du rôle (ex. “Admin”, “User”), obligatoire, ≤ 50 caractères

        [MaxLength(200)]
        public string Description { get; set; }
        // Description optionnelle du rôle, ≤ 200 caractères

        [Required]
        public string Permissions { get; set; }
        // Chaîne représentant les permissions (ex. “Create,Read,Update,Delete”)
        // → À envisager : type plus structuré (liste, enum flags) pour éviter les erreurs de parsing

        public ICollection<User> Users { get; set; }
        // Utilisateurs affectés à ce rôle
        // → À initialiser (new List<User>()) pour éviter les NullReferenceException
    }
}
