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

            var event4 = new Event { Name = "Festival Indie: Sonidos", EventDate = DateTime.UtcNow.AddDays(75), Venue = "Hipódromo", Status = "Active" };
            var event5 = new Event { Name = "Ópera: La Traviata", EventDate = DateTime.UtcNow.AddDays(90), Venue = "Teatro Colón", Status = "Active" };
            var event6 = new Event { Name = "Comedy Night: Stand Up", EventDate = DateTime.UtcNow.AddDays(20), Venue = "Paseo La Plaza", Status = "Active" };

            await context.Events.AddRangeAsync(event1, event2, event3, event4, event5, event6);
            await context.SaveChangesAsync();

            // Crear sectores para los eventos iniciales
            var sector1 = new Sector { EventId = event1.Id, Name = "Platea Baja", Price = 15000, Capacity = 50 };
            var sector2 = new Sector { EventId = event1.Id, Name = "Platea Alta", Price = 10000, Capacity = 50 };
            var sector3 = new Sector { EventId = event2.Id, Name = "Palco", Price = 25000, Capacity = 20 };
            var sector4 = new Sector { EventId = event2.Id, Name = "General", Price = 5000, Capacity = 100 };

            // Crear sectores para los nuevos eventos
            var s5 = new Sector { EventId = event4.Id, Name = "Campo", Price = 8000, Capacity = 50 };
            var s6 = new Sector { EventId = event5.Id, Name = "Palco Bajo", Price = 30000, Capacity = 10 };
            var s7 = new Sector { EventId = event6.Id, Name = "Platea", Price = 4500, Capacity = 40 };

            await context.Sectors.AddRangeAsync(sector1, sector2, sector3, sector4, s5, s6, s7);
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
                seats.Add(new Seat { SectorId = sector3.Id, RowIdentifier = "VIP", SeatNumber = i, Status = "Available" });
            }

            for (int i = 1; i <= 20; i++)
            {
                seats.Add(new Seat { SectorId = s5.Id, RowIdentifier = "C", SeatNumber = i, Status = "Available" });
            }

            for (int i = 1; i <= 10; i++)
            {
                seats.Add(new Seat { SectorId = s6.Id, RowIdentifier = "P", SeatNumber = i, Status = "Available" });
            }

            for (int i = 1; i <= 30; i++)
            {
                seats.Add(new Seat { SectorId = s7.Id, RowIdentifier = "R", SeatNumber = i, Status = "Available" });
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
