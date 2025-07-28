using AspNetCoreRateLimit;                              // Package pour la limitation de débit par IP
using CyberIncidentManager.API.Data;                    // Contexte de la base de données de l’API
using CyberIncidentManager.API.Services;                // Services métiers (jetons, e‑mails)
using Microsoft.AspNetCore.Authentication.JwtBearer;    // Authentification JWT
using Microsoft.EntityFrameworkCore;                    // EF Core pour la base PostgreSQL
using Microsoft.IdentityModel.Tokens;                   // Validation des jetons
using System.Text;                                      // Encodage des clés

var builder = WebApplication.CreateBuilder(args);

// 1. Configuration des services de base
builder.Services.AddControllers();                      // Ajoute le support des contrôleurs API
builder.Services.AddEndpointsApiExplorer();             // Documente les endpoints pour Swagger
builder.Services.AddSwaggerGen();                       // Génère la documentation OpenAPI

// 2. Injection de dépendances pour les services métiers
builder.Services.AddScoped<TokenService>();             // Service de génération et validation de JWT
builder.Services.AddScoped<EmailService>();             // Service d’envoi d’e‑mails

// 3. Limitation de débit (Rate Limiting)
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();         // Configuration singleton pour RateLimit

// 4. Authentification JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Récupération de la clé secrète depuis la configuration
        var key = builder.Configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(key))
            throw new InvalidOperationException("JWT key not configured.");

        // Paramètres de validation du jeton
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,                                            // Vérifie l’émetteur
            ValidateAudience = true,                                          // Vérifie le destinataire
            ValidateIssuerSigningKey = true,                                  // Vérifie la signature
            ValidateLifetime = true,                                          // Vérifie la date d’expiration
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    });

// 5. Politique CORS pour l’application front (Netlify)
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
        policy.WithOrigins("https://incident-manager.netlify.app")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// 6. Configuration de la base de données PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// 7. Middleware pour l’environnement de développement
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();      // Sert le JSON OpenAPI
    app.UseSwaggerUI();    // Interface Swagger UI
}

app.UseHttpsRedirection(); // Redirige HTTP vers HTTPS

// 8. Sécurité des headers HTTP
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");    // Prévention du sniffing MIME
    context.Response.Headers.Add("X-Frame-Options", "DENY");              // Interdit l’affichage en iframe
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");    // Active la protection XSS du navigateur
    await next();
});

// 9. Gestion centralisée des erreurs
app.UseExceptionHandler("/error");   // Redirige les exceptions vers un contrôleur d’erreur

// 10. Application de la politique CORS
app.UseCors("CorsPolicy");

// 11. Application de la limitation de débit
app.UseIpRateLimiting();

app.UseAuthentication();  // Active le middleware d’authentification
app.UseAuthorization();   // Active le middleware d’autorisation

app.MapControllers();     // Mappe les routes des contrôleurs
app.Run();                // Démarre l’application