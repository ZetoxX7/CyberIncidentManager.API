using Microsoft.AspNetCore.Mvc;

namespace CyberIncidentManager.API.Controllers
{
    [ApiController]
    // Indique que ce contrôleur utilise les conventions API (validation automatique, binding, etc.)
    public class ErrorController : ControllerBase
    {
        // Route globale pour gérer les erreurs non traitées
        [Route("/error")]
        [HttpGet]
        public IActionResult HandleError()
        {
            // Retourne un objet ProblemDetails JSON standardisé
            // → Ne jamais exposer de stack trace ou détails techniques en production
            return Problem(
                title: "Une erreur est survenue.",                      // Titre générique pour l’utilisateur
                statusCode: 500,                                       // Code HTTP 500 Internal Server Error
                detail: "Une erreur interne s'est produite. Veuillez réessayer plus tard."
            );
        }
    }
}