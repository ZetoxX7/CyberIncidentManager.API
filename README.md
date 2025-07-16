# CyberIncidentManager.API

API REST .NET 8 pour la gestion des incidents de cybersécurité, des utilisateurs, des rôles, des actifs, des notifications et des réponses.  
Sécurisée par JWT, MFA, rate limiting, et conforme aux bonnes pratiques modernes.

## Fonctionnalités

- Authentification JWT (avec support MFA pour les admins)
- Gestion des utilisateurs, rôles, permissions
- Gestion des incidents, types d’incidents, actifs (assets)
- Notifications et réponses aux incidents
- Rate limiting configurable (AspNetCoreRateLimit)
- Sécurité renforcée (CORS, headers HTTP, gestion centralisée des erreurs)
- Documentation Swagger intégrée

## Prérequis

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/)
- (Optionnel) [Docker](https://www.docker.com/) pour la base de données

## Installation

1. **Cloner le dépôt**
git clone https://github.com/votre-utilisateur/CyberIncidentManager.API.git cd CyberIncidentManager.API

2. **Configurer la base de données**
- Créez une base PostgreSQL.
- Copiez `appsettings-example.json` en `appsettings.json` et renseignez vos informations :
  ```bash
  cp CyberIncidentManager.API/appsettings-example.json CyberIncidentManager.API/appsettings.json
  ```
- Modifiez la chaîne de connexion et la clé JWT dans `appsettings.json` :
  ```json
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=incident-management;Username=postgres;Password=VOTRE_MDP"
  },
  "Jwt": {
    "Key": "VOTRE_CLE_SECRETE",
    "Issuer": "CyberPlatform",
    "Audience": "CyberClients"
  }
  ```

3. **Appliquer les migrations**
dotnet ef database update --project CyberIncidentManager.API

4. **Lancer l’API**
dotnet run --project CyberIncidentManager.API

5. **Accéder à la documentation Swagger**
- [http://localhost:5000/swagger](http://localhost:5000/swagger) (ou le port affiché au lancement)

## Configuration

- **Sécurité** : Ne jamais versionner `appsettings.json` (ajoutez-le à `.gitignore`).
- **MFA** : Activé pour les comptes administrateurs.
- **CORS** : Autorise le front-end Netlify par défaut (`https://incident-manager.netlify.app`).
- **Rate Limiting** : Configurable dans `appsettings.json` (voir section `IpRateLimiting`).

## Structure du projet

- `Controllers/` : Endpoints REST (Incidents, Users, Roles, Assets, etc.)
- `Models/` : Entités EF Core (User, Incident, Asset, etc.)
- `Models/DTOs/` : Objets de transfert de données (DTOs)
- `Services/` : Services métiers (TokenService, EmailService, etc.)
- `Data/` : Contexte EF Core (`ApplicationDbContext`)
- `appsettings.json` : Configuration locale (non versionnée)
- `appsettings-example.json` : Exemple de configuration (à compléter)

## Exemples d’API

- `POST /api/auth/login` : Connexion (JWT)
- `POST /api/auth/register` : Inscription
- `POST /api/auth/verify-mfa` : Vérification MFA
- `GET /api/incidents` : Liste des incidents (authentifié)
- `POST /api/incidents` : Créer un incident (Employé, Analyste, Admin)
- `GET /api/users` : Liste des utilisateurs (Admin)
- `POST /api/roles` : Créer un rôle (Admin)

## Bonnes pratiques

- **Ne jamais publier de secrets** (mot de passe, clé JWT, etc.)
- **Utiliser `appsettings-example.json`** pour partager la structure de config
- **Protéger l’API** derrière HTTPS en production

## Licence

Ce projet est sous licence MIT.

---

> Pour toute question ou contribution, ouvrez une issue ou une pull request sur GitHub.
