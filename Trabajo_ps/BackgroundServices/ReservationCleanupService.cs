using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Trabajo_ps.BackgroundServices
{
    public class ReservationCleanupService : BackgroundService
    {
        private readonly ILogger<ReservationCleanupService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public ReservationCleanupService(ILogger<ReservationCleanupService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ReservationCleanupService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupExpiredReservationsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing ReservationCleanup.");
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // Check every 10 seconds
            }

            _logger.LogInformation("ReservationCleanupService is stopping.");
        }

        private async Task CleanupExpiredReservationsAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var expiredReservations = await unitOfWork.Reservations.FindAsync(r => 
                    r.Status == "Pending" && r.ExpiresAt <= DateTime.UtcNow);

                var reservationsList = expiredReservations.ToList();

                if (reservationsList.Any())
                {
                    await unitOfWork.BeginTransactionAsync();
                    try
                    {
                        foreach (var reservation in reservationsList)
                        {
                            reservation.Status = "Expired";
                            await unitOfWork.Reservations.UpdateAsync(reservation);

                            var seat = await unitOfWork.Seats.GetByIdAsync(reservation.SeatId);
                            if (seat != null)
                            {
                                seat.Status = "Available";
                                await unitOfWork.Seats.UpdateAsync(seat);
                            }

                            // Audit Log
                            var audit = new AuditLog
                            {
                                UserId = 0, // System
                                Action = AuditAction.RELEASE_SEAT.ToString(),
                                EntityType = nameof(Reservation),
                                EntityId = reservation.Id.ToString(),
                                Details = $"{{\"seatId\":\"{reservation.SeatId}\", \"reason\":\"time_expired\"}}"
                            };
                            await unitOfWork.AuditLogs.AddAsync(audit);
                        }

                        await unitOfWork.CompleteAsync();
                        await unitOfWork.CommitTransactionAsync();
                        _logger.LogInformation($"Cleaned up {reservationsList.Count} expired reservations.");
                    }
                    catch (Exception ex)
                    {
                        await unitOfWork.RollbackTransactionAsync();
                        _logger.LogError(ex, "Error cleaning up reservations");
                        throw;
                    }
                }
            }
        }
    }
}
