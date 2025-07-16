﻿using System.ComponentModel.DataAnnotations;

namespace CyberIncidentManager.API.Models
{
    public class Asset
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string Type { get; set; }

        [Required]
        [MaxLength(45)]
        [RegularExpression(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$", ErrorMessage = "Adresse IP invalide.")]
        public string IpAddress { get; set; }

        [Required]
        [MaxLength(100)]
        public string Owner { get; set; }

        [MaxLength(100)]
        public string Location { get; set; }

        [Required]
        public string Criticality { get; set; }
    }
}