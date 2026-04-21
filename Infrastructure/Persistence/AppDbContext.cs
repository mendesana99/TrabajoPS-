using System;
using System.Linq;
using System.Threading;
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

            // Configurar conversiones de enums a strings
            modelBuilder.Entity<Event>()
                .Property(e => e.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Seat>()
                .Property(s => s.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Reservation>()
                .Property(r => r.Status)
                .HasConversion<string>();

            // Índices para mejorar performance
            modelBuilder.Entity<Seat>()
                .HasIndex(s => s.Status);

            modelBuilder.Entity<Reservation>()
                .HasIndex(r => r.ExpiresAt);

            modelBuilder.Entity<Reservation>()
                .HasIndex(r => new { r.SeatId, r.Status });

            modelBuilder.Entity<AuditLog>()
                .HasIndex(a => a.CreatedAt);
        }

        /*no usamos la parte de auditoria por ahora, pero se deja el codigo comentado para futuras implementaciones
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<BaseEntity>();
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
        */

        // Este método se puede usar para guardar cambios sin la lógica de auditoría, si se desea mantenerla separada 
        public async Task<int> SaveChangesAsync()
        { 
            return await base.SaveChangesAsync();
        }

    }
}
