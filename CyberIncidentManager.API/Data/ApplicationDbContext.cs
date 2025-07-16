using CyberIncidentManager.API.Models;
using CyberIncidentManager.API.Models.Auth;
using Microsoft.EntityFrameworkCore;

namespace CyberIncidentManager.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // 1. Jeux d’entités exposés pour EF Core
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Incident> Incidents { get; set; }
        public DbSet<IncidentType> IncidentTypes { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<Response> Responses { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 2. Relation User→Role (1:N)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);

            // 3. Unicité de l’email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // 4. Contraintes sur User
            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .HasMaxLength(100)
                .IsRequired();
            modelBuilder.Entity<User>()
                .Property(u => u.FirstName)
                .HasMaxLength(50)
                .IsRequired();
            modelBuilder.Entity<User>()
                .Property(u => u.LastName)
                .HasMaxLength(50)
                .IsRequired();

            // 5. Incident → IncidentType (N:1)
            modelBuilder.Entity<Incident>()
                .HasOne(i => i.Type)
                .WithMany()
                .HasForeignKey(i => i.TypeId);

            // 6. Incident → Asset (N:1 optionnel)
            modelBuilder.Entity<Incident>()
                .HasOne(i => i.Asset)
                .WithMany()
                .HasForeignKey(i => i.AssetId)
                .IsRequired(false);

            // 7. Incident → User (ReportedBy), suppression restreinte
            modelBuilder.Entity<Incident>()
                .HasOne(i => i.ReportedByUser)
                .WithMany(u => u.ReportedIncidents)
                .HasForeignKey(i => i.ReportedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // 8. Incident → User (AssignedTo), suppression restreinte
            modelBuilder.Entity<Incident>()
                .HasOne(i => i.AssignedToUser)
                .WithMany(u => u.AssignedIncidents)
                .HasForeignKey(i => i.AssignedTo)
                .OnDelete(DeleteBehavior.Restrict);

            // 9. Contraintes sur Incident
            modelBuilder.Entity<Incident>()
                .Property(i => i.Title)
                .HasMaxLength(100)
                .IsRequired();
            modelBuilder.Entity<Incident>()
                .Property(i => i.Description)
                .HasMaxLength(1000)
                .IsRequired();

            // 10. Contraintes sur IncidentType
            modelBuilder.Entity<IncidentType>()
                .Property(t => t.Name)
                .HasMaxLength(100)
                .IsRequired();
            modelBuilder.Entity<IncidentType>()
                .Property(t => t.Description)
                .HasMaxLength(500);

            // 11. Contraintes sur Asset
            modelBuilder.Entity<Asset>()
                .Property(a => a.Name)
                .HasMaxLength(100)
                .IsRequired();
            modelBuilder.Entity<Asset>()
                .Property(a => a.Type)
                .HasMaxLength(50)
                .IsRequired();
            modelBuilder.Entity<Asset>()
                .Property(a => a.IpAddress)
                .HasMaxLength(45)
                .IsRequired();
            modelBuilder.Entity<Asset>()
                .Property(a => a.Owner)
                .HasMaxLength(100)
                .IsRequired();
            modelBuilder.Entity<Asset>()
                .Property(a => a.Location)
                .HasMaxLength(100);

            // 12. Response → Incident et User
            modelBuilder.Entity<Response>()
                .HasOne(r => r.Incident)
                .WithMany(i => i.Responses)
                .HasForeignKey(r => r.IncidentId);
            modelBuilder.Entity<Response>()
                .HasOne(r => r.User)
                .WithMany(u => u.Responses)
                .HasForeignKey(r => r.UserId);

            // 13. Notification → User et Incident
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId);
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Incident)
                .WithMany(i => i.Notifications)
                .HasForeignKey(n => n.IncidentId);

            // 14. Contraintes sur Notification
            modelBuilder.Entity<Notification>()
                .Property(n => n.Title)
                .HasMaxLength(100)
                .IsRequired();
            modelBuilder.Entity<Notification>()
                .Property(n => n.Message)
                .HasMaxLength(1000)
                .IsRequired();

            // 15. Contraintes sur Role
            modelBuilder.Entity<Role>()
                .Property(r => r.Name)
                .HasMaxLength(50)
                .IsRequired();
            modelBuilder.Entity<Role>()
                .Property(r => r.Description)
                .HasMaxLength(200);

            // 16. Contraintes sur RefreshToken
            modelBuilder.Entity<RefreshToken>()
                .Property(rt => rt.Token)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}