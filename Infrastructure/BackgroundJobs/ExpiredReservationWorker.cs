using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;


namespace Infrastructure.BackgroundJobs
{
    public class ExpiredReservationWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ExpiredReservationWorker> _logger;

        public ExpiredReservationWorker(IServiceProvider serviceProvider, ILogger<ExpiredReservationWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessExpiredReservations(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error procesando reservas expiradas");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task ProcessExpiredReservations(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var expiredReservations = await unitOfWork.Reservations
                .FindAsync(r => r.Status == ReservationStatus.Pending && r.ExpiresAt < DateTime.UtcNow);

            foreach (var reservation in expiredReservations)
            {
                await unitOfWork.BeginTransactionAsync();

                try
                {
                    var seat = await unitOfWork.Seats.GetByIdAsync(reservation.SeatId);
                    if (seat != null && seat.Status == SeatStatus.Reserved)
                    {
                        seat.Status = SeatStatus.Available;
                        await unitOfWork.Seats.UpdateAsync(seat);
                    }

                    reservation.Status = ReservationStatus.Expired;
                    await unitOfWork.Reservations.UpdateAsync(reservation);

                    var audit = new AuditLog
                    {
                        UserId = null,
                        Action = AuditAction.EXPIRED.ToString(),
                        EntityType = nameof(Reservation),
                        EntityId = reservation.Id.ToString(),
                        Details = $"{{\"reason\":\"payment_timeout\",\"expiredAt\":\"{DateTime.UtcNow}\"}}"
                    };
                    await unitOfWork.AuditLogs.AddAsync(audit);

                    await unitOfWork.CompleteAsync();
                    await unitOfWork.CommitTransactionAsync();

                    _logger.LogInformation($"Reserva expirada liberada: {reservation.Id}");
                }
                catch (Exception ex)
                {
                    await unitOfWork.RollbackTransactionAsync();
                    _logger.LogError(ex, $"Error liberando reserva {reservation.Id}");
                }
            }
        }
    }
}
