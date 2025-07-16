namespace CyberIncidentManager.API.Models.Auth
{
    public class AuthResponse
    {
        public string AccessToken { get; set; }
        // Jeton JWT à usage immédiat
        // → Renvoie la chaîne signée générée par TokenService.GenerateJwtToken()

        public string RefreshToken { get; set; }
        // Jeton de rafraîchissement à stocker côté client pour obtenir de nouveaux AccessToken
        // → Correspond à la valeur retournée par TokenService.GenerateRefreshToken()

        public DateTime Expiration { get; set; }
        // Date et heure d’expiration de l’AccessToken (UTC)
        // → Utile pour le client afin de planifier le rafraîchissement avant l’expiration
    }
}