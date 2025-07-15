using CyberIncidentManager.API.Models;
using CyberIncidentManager.API.Models.Auth;
using Microsoft.EntityFrameworkCore;

namespace CyberIncidentManager.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Tables
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

            // USER → ROLE (1:N)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);

            // INCIDENT → INCIDENT_TYPE (N:1)
            modelBuilder.Entity<Incident>()
                .HasOne(i => i.Type)
                .WithMany()
                .HasForeignKey(i => i.TypeId);

            // INCIDENT → ASSET (N:1 optionnel)
            modelBuilder.Entity<Incident>()
                .HasOne(i => i.Asset)
                .WithMany()
                .HasForeignKey(i => i.AssetId)
                .IsRequired(false);

            // INCIDENT → USER (REPORTED BY)
            modelBuilder.Entity<Incident>()
                .HasOne(i => i.ReportedByUser)
                .WithMany(u => u.ReportedIncidents)
                .HasForeignKey(i => i.ReportedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // INCIDENT → USER (ASSIGNED TO)
            modelBuilder.Entity<Incident>()
                .HasOne(i => i.AssignedToUser)
                .WithMany(u => u.AssignedIncidents)
                .HasForeignKey(i => i.AssignedTo)
                .OnDelete(DeleteBehavior.Restrict);

            // RESPONSE → INCIDENT
            modelBuilder.Entity<Response>()
                .HasOne(r => r.Incident)
                .WithMany(i => i.Responses)
                .HasForeignKey(r => r.IncidentId);

            // RESPONSE → USER
            modelBuilder.Entity<Response>()
                .HasOne(r => r.User)
                .WithMany(u => u.Responses)
                .HasForeignKey(r => r.UserId);

            // NOTIFICATION → USER
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId);

            // NOTIFICATION → INCIDENT
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Incident)
                .WithMany(i => i.Notifications)
                .HasForeignKey(n => n.IncidentId);
        }
    }
}