using CyberIncidentManager.API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CyberIncidentManager.API.Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        // Injection de la configuration pour accéder aux clés et paramètres JWT
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Génère un JWT signé contenant les informations de l’utilisateur
        public string GenerateJwtToken(User user)
        {
            // Récupère la clé secrète depuis la configuration et la convertit en bytes
            var key = Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT key not configured.")
            );

            // Définit les revendications (claims) à inclure dans le token
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),            // Identifiant unique
                new Claim(ClaimTypes.Email, user.Email),                            // Email de l’utilisateur
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),    // Nom complet
                new Claim(ClaimTypes.Role, user.Role?.Name ?? "User")               // Rôle (par défaut “User”)
            };

            // Crée le token avec émetteur, audience, claims, date d’expiration et signature
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],                                // Émetteur autorisé
                audience: _configuration["Jwt:Audience"],                            // Destinataire autorisé
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),                             // Expiration dans 30 minutes
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256                                    // Algorithme HMAC-SHA256
                )
            );

            // Retourne le JWT sous forme de chaîne compacte
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Génère un token de rafraîchissement cryptographiquement sûr
        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();  // Générateur de nombres aléatoires sécurisé
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);      // Retourne le token en Base64
        }

        // Extrait le principal (claims) d’un JWT expiré pour permettre le rafraîchissement
        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var key = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(key))
                throw new SecurityTokenException("JWT key not configured.");

            // Paramètres de validation : on désactive la validation de la durée de vie
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false,  // Autorise un token expiré pour en lire les claims
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                // Tente de valider le token (sans vérifier l’expiration)
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

                // Vérifie que le token est bien un JwtSecurityToken
                if (securityToken is not JwtSecurityToken jwtToken)
                    return null;

                return principal;  // Retourne les claims pour génération d’un nouveau JWT
            }
            catch
            {
                return null;  // En cas d’erreur (signature invalide, audience/issuer incorrects…), on renvoie null
            }
        }
    }
}