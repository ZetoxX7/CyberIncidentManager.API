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

            // USERS
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.HasKey(u => u.Id);
                entity.Property(u => u.Id).HasColumnName("id");
                entity.Property(u => u.Email).HasColumnName("email").HasMaxLength(100).IsRequired();
                entity.Property(u => u.PasswordHash).HasColumnName("passwordhash");
                entity.Property(u => u.FirstName).HasColumnName("firstname").HasMaxLength(50).IsRequired();
                entity.Property(u => u.LastName).HasColumnName("lastname").HasMaxLength(50).IsRequired();
                entity.Property(u => u.RoleId).HasColumnName("roleid");
                entity.Property(u => u.CreatedAt).HasColumnName("createdat");
                entity.Property(u => u.IsActive).HasColumnName("isactive");

                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasOne(u => u.Role)
                      .WithMany(r => r.Users)
                      .HasForeignKey(u => u.RoleId);
            });

            // ROLES
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("roles");

                entity.Property(r => r.Id).HasColumnName("id");
                entity.Property(r => r.Name).HasColumnName("name").HasMaxLength(50).IsRequired();
                entity.Property(r => r.Description).HasColumnName("description").HasMaxLength(200);
                entity.Property(r => r.Permissions).HasColumnName("permissions");
            });

            // INCIDENTS
            modelBuilder.Entity<Incident>(entity =>
            {
                entity.ToTable("incidents");

                entity.Property(i => i.Id).HasColumnName("id");
                entity.Property(i => i.Title).HasColumnName("title").HasMaxLength(100).IsRequired();
                entity.Property(i => i.Description).HasColumnName("description").HasMaxLength(1000).IsRequired();
                entity.Property(i => i.Severity).HasColumnName("severity");
                entity.Property(i => i.Status).HasColumnName("status");
                entity.Property(i => i.TypeId).HasColumnName("typeid");
                entity.Property(i => i.ReportedBy).HasColumnName("reportedby");
                entity.Property(i => i.AssignedTo).HasColumnName("assignedto");
                entity.Property(i => i.CreatedAt).HasColumnName("createdat");
                entity.Property(i => i.ResolvedAt).HasColumnName("resolvedat");
                entity.Property(i => i.AssetId).HasColumnName("assetid");

                entity.HasOne(i => i.Type)
                      .WithMany()
                      .HasForeignKey(i => i.TypeId);

                entity.HasOne(i => i.Asset)
                      .WithMany()
                      .HasForeignKey(i => i.AssetId)
                      .IsRequired(false);

                entity.HasOne(i => i.ReportedByUser)
                      .WithMany(u => u.ReportedIncidents)
                      .HasForeignKey(i => i.ReportedBy)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(i => i.AssignedToUser)
                      .WithMany(u => u.AssignedIncidents)
                      .HasForeignKey(i => i.AssignedTo)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // INCIDENT TYPES
            modelBuilder.Entity<IncidentType>(entity =>
            {
                entity.ToTable("incidenttypes");

                entity.Property(t => t.Id).HasColumnName("id");
                entity.Property(t => t.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
                entity.Property(t => t.Description).HasColumnName("description").HasMaxLength(500);
                entity.Property(t => t.DefaultSeverity).HasColumnName("defaultseverity");
                entity.Property(t => t.Color).HasColumnName("color");
            });

            // ASSETS
            modelBuilder.Entity<Asset>(entity =>
            {
                entity.ToTable("assets");

                entity.Property(a => a.Id).HasColumnName("id");
                entity.Property(a => a.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
                entity.Property(a => a.Type).HasColumnName("type").HasMaxLength(50).IsRequired();
                entity.Property(a => a.IpAddress).HasColumnName("ipaddress").HasMaxLength(45).IsRequired();
                entity.Property(a => a.Owner).HasColumnName("owner").HasMaxLength(100).IsRequired();
                entity.Property(a => a.Location).HasColumnName("location").HasMaxLength(100);
                entity.Property(a => a.Criticality).HasColumnName("criticality");
            });

            // RESPONSES
            modelBuilder.Entity<Response>(entity =>
            {
                entity.ToTable("responses");

                entity.Property(r => r.Id).HasColumnName("id");
                entity.Property(r => r.IncidentId).HasColumnName("incidentid");
                entity.Property(r => r.UserId).HasColumnName("userid");
                entity.Property(r => r.Action).HasColumnName("action");
                entity.Property(r => r.Details).HasColumnName("details");
                entity.Property(r => r.Timestamp).HasColumnName("timestamp");
                entity.Property(r => r.IsSuccessful).HasColumnName("issuccessful");

                entity.HasOne(r => r.Incident)
                      .WithMany(i => i.Responses)
                      .HasForeignKey(r => r.IncidentId);

                entity.HasOne(r => r.User)
                      .WithMany(u => u.Responses)
                      .HasForeignKey(r => r.UserId);
            });

            // NOTIFICATIONS
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("notifications");

                entity.Property(n => n.Id).HasColumnName("id");
                entity.Property(n => n.UserId).HasColumnName("userid");
                entity.Property(n => n.IncidentId).HasColumnName("incidentid");
                entity.Property(n => n.Title).HasColumnName("title").HasMaxLength(100).IsRequired();
                entity.Property(n => n.Message).HasColumnName("message").HasMaxLength(1000).IsRequired();
                entity.Property(n => n.IsRead).HasColumnName("isread");
                entity.Property(n => n.CreatedAt).HasColumnName("createdat");

                entity.HasOne(n => n.User)
                      .WithMany(u => u.Notifications)
                      .HasForeignKey(n => n.UserId);

                entity.HasOne(n => n.Incident)
                      .WithMany(i => i.Notifications)
                      .HasForeignKey(n => n.IncidentId);
            });

            // REFRESH TOKENS
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("refreshtokens");

                entity.Property(rt => rt.Id).HasColumnName("id");
                entity.Property(rt => rt.UserId).HasColumnName("userid");
                entity.Property(rt => rt.Token).HasColumnName("token").HasMaxLength(200).IsRequired();
                entity.Property(rt => rt.ExpiresAt).HasColumnName("expiresat");
                entity.Property(rt => rt.IsRevoked).HasColumnName("isrevoked");
            });
        }
    }
}
