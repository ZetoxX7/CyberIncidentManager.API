using Microsoft.AspNetCore.Mvc;

namespace CyberIncidentManager.API.Controllers
{
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [Route("/error")]
        public IActionResult HandleError()
        {
            // Ne pas exposer de d�tails en production
            return Problem(
                title: "Une erreur est survenue.",
                statusCode: 500,
                detail: "Une erreur interne s'est produite. Veuillez r�essayer plus tard."
            );
        }
    }
}