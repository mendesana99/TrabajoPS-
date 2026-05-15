using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<Sector> Sectors { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Indices para mejorar performance
            modelBuilder.Entity<Seat>()
                .HasIndex(s => s.Status);

            modelBuilder.Entity<Reservation>()
                .HasIndex(r => r.ExpiresAt);

            modelBuilder.Entity<Reservation>()
                .HasIndex(r => new { r.SeatId, r.Status });

            modelBuilder.Entity<AuditLog>()
                .HasIndex(a => a.CreatedAt);

            // Fluent API Configurations
            modelBuilder.Entity<Sector>()
                .Property(s => s.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Sector>()
                .Property(s => s.Name)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Event>()
                .Property(e => e.Name)
                .HasMaxLength(200)
                .IsRequired();

            modelBuilder.Entity<Event>()
                .Property(e => e.Venue)
                .HasMaxLength(200)
                .IsRequired();

            modelBuilder.Entity<Event>()
                .Property(e => e.Status)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<Seat>()
                .Property(s => s.RowIdentifier)
                .HasMaxLength(10)
                .IsRequired();

            modelBuilder.Entity<Seat>()
                .Property(s => s.Status)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<Seat>()
                .Property(s => s.Version)
                .IsConcurrencyToken();

            modelBuilder.Entity<User>()
                .Property(u => u.Name)
                .HasMaxLength(200)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .HasMaxLength(200)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.PasswordHash)
                .HasMaxLength(200)
                .IsRequired();

            modelBuilder.Entity<Reservation>()
                .Property(r => r.Status)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<AuditLog>()
                .Property(a => a.Action)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<AuditLog>()
                .Property(a => a.EntityType)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<AuditLog>()
                .Property(a => a.EntityId)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<AuditLog>()
                .Property(a => a.Details)
                .HasMaxLength(2000);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<AuditLog>();
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
