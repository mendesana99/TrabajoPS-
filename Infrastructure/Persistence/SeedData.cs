using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (await context.Events.AnyAsync())
                return;

            // Crear eventos
            var event1 = new Event
            {
                Name = "Concierto de Rock 2026",
                EventDate = DateTime.UtcNow.AddDays(30),
                Venue = "Estadio Monumental",
                Status = "Active"
            };

            var event2 = new Event
            {
                Name = "Teatro: El Fantasma",
                EventDate = DateTime.UtcNow.AddDays(45),
                Venue = "Teatro Colón",
                Status = "Active"
            };

            var event3 = new Event
            {
                Name = "Fútbol: Final Copa",
                EventDate = DateTime.UtcNow.AddDays(60),
                Venue = "Estadio Kempes",
                Status = "Active"
            };

            await context.Events.AddRangeAsync(event1, event2, event3);
            await context.SaveChangesAsync();

            // Crear sectores para el evento 1
            var sector1 = new Sector { EventId = event1.Id, Name = "Platea Baja", Price = 15000, Capacity = 50 };
            var sector2 = new Sector { EventId = event1.Id, Name = "Platea Alta", Price = 10000, Capacity = 50 };

            // Sectores para evento 2
            var sector3 = new Sector { EventId = event2.Id, Name = "Palco", Price = 25000, Capacity = 20 };
            var sector4 = new Sector { EventId = event2.Id, Name = "General", Price = 5000, Capacity = 100 };

            await context.Sectors.AddRangeAsync(sector1, sector2, sector3, sector4);
            await context.SaveChangesAsync();

            // Crear butacas
            var seats = new List<Seat>();

            for (int i = 1; i <= 50; i++)
            {
                seats.Add(new Seat
                {
                    SectorId = sector1.Id,
                    RowIdentifier = $"Fila {((i - 1) / 10) + 1}",
                    SeatNumber = i,
                    Status = "Available"
                });
            }

            for (int i = 1; i <= 50; i++)
            {
                seats.Add(new Seat
                {
                    SectorId = sector2.Id,
                    RowIdentifier = $"Fila {((i - 1) / 10) + 1}",
                    SeatNumber = i,
                    Status = "Available"
                });
            }

            for (int i = 1; i <= 20; i++)
            {
                seats.Add(new Seat
                {
                    SectorId = sector3.Id,
                    RowIdentifier = "VIP",
                    SeatNumber = i,
                    Status = "Available"
                });
            }

            await context.Seats.AddRangeAsync(seats);

            // Crear usuario de prueba
            var user = new User
            {
                Name = "Juan Pérez",
                Email = "juan@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456")
            };

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }
    }
}
