using CAMPUS.DAL.Seed;
using CAMPUS.DOMAIN.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CAMPUS.DAL
{
    public class CampusDbContext(DbContextOptions<CampusDbContext> options) : DbContext(options)
    {
        public DbSet<ServiceRequest> ServiceRequests => Set<ServiceRequest>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Team> Teams => Set<Team>();
        public DbSet<Campus> Campuses => Set<Campus>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<NotificationLog> NotificationLogs => Set<NotificationLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Delete rules (C3)
            modelBuilder.Entity<ServiceRequest>(e =>
            {
                e.HasOne(r => r.Category).WithMany().HasForeignKey(r => r.CategoryId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(r => r.Campus).WithMany().HasForeignKey(r => r.CampusId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(r => r.Reporter).WithMany().HasForeignKey(r => r.ReporterId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(r => r.AssignedTeam).WithMany().HasForeignKey(r => r.AssignedTeamId).OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Comment>(e =>
            {
                e.HasOne(c => c.ServiceRequest).WithMany(r => r.Comment).HasForeignKey(c => c.ServiceRequestId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(c => c.Author).WithMany().HasForeignKey(c => c.AuthorId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<User>()
                .HasOne(u => u.Team).WithMany(t => t.Members).HasForeignKey(u => u.TeamId).OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<NotificationLog>()
                .HasOne<ServiceRequest>().WithMany().HasForeignKey(n => n.ServiceRequestId).OnDelete(DeleteBehavior.SetNull);

            // Unique values (C3). A unique index needs a max length in SQL Server.
            modelBuilder.Entity<User>(e => { e.Property(u => u.Email).HasMaxLength(200); e.HasIndex(u => u.Email).IsUnique(); });
            modelBuilder.Entity<Team>(e => { e.Property(t => t.Name).HasMaxLength(100); e.HasIndex(t => t.Name).IsUnique(); });
            modelBuilder.Entity<Campus>(e => { e.Property(c => c.Name).HasMaxLength(100); e.HasIndex(c => c.Name).IsUnique(); });
            modelBuilder.Entity<Category>(e => { e.Property(c => c.Name).HasMaxLength(100); e.HasIndex(c => c.Name).IsUnique(); });

            modelBuilder.Entity<User>(e =>
            {
                e.Property(u => u.Auth0Id).HasMaxLength(100);
                e.HasIndex(u => u.Auth0Id).IsUnique().HasFilter("[Auth0Id] IS NOT NULL");
            });

            SeedData.Apply(modelBuilder);
        }
    }
}
