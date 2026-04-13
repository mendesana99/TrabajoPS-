using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class TicketService : ITicketService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TicketService> _logger;

        public TicketService(IUnitOfWork unitOfWork, ILogger<TicketService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Reservation> ReserveSeatAsync(Guid seatId, int userId)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 1. Obtener el asiento
                var seat = await _unitOfWork.Seats.GetByIdAsync(seatId);
                if (seat == null)
                    throw new InvalidOperationException("El asiento no existe");

                // 2. Verificar disponibilidad
                if (seat.Status != SeatStatus.Available)
                    throw new InvalidOperationException("El asiento no está disponible");

                // 3. Verificar si hay reserva activa
                var existingReservations = await _unitOfWork.Reservations
                    .FindAsync(r => r.SeatId == seatId && r.Status == ReservationStatus.Pending && r.ExpiresAt > DateTime.UtcNow);

                if (existingReservations.Any())
                    throw new InvalidOperationException("El asiento ya está reservado");

                // 4. Cambiar estado del asiento
                seat.Status = SeatStatus.Reserved;
                await _unitOfWork.Seats.UpdateAsync(seat);

                // 5. Crear reserva
                var reservation = new Reservation
                {
                    SeatId = seatId,
                    UserId = userId,
                    Status = ReservationStatus.Pending,
                    ReservedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(5)
                };
                await _unitOfWork.Reservations.AddAsync(reservation);

                // 6. Auditoría
                var audit = new AuditLog
                {
                    UserId = userId,
                    Action = AuditAction.RESERVE_SUCCESS.ToString(),
                    EntityType = nameof(Seat),
                    EntityId = seatId.ToString(),
                    Details = $"{{\"seatNumber\":{seat.SeatNumber},\"expiresAt\":\"{reservation.ExpiresAt}\"}}"
                };
                await _unitOfWork.AuditLogs.AddAsync(audit);

                // 7. Guardar todo
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation($"Usuario {userId} reservó asiento {seatId}");
                return reservation;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogWarning(ex, $"Concurrencia en asiento {seatId}");
                throw new InvalidOperationException("El asiento fue reservado por otro usuario", ex);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, $"Error reservando asiento {seatId}");
                throw;
            }
        }

        public async Task<bool> ConfirmPaymentAsync(Guid reservationId, int userId)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var reservations = await _unitOfWork.Reservations
                    .FindAsync(r => r.Id == reservationId && r.UserId == userId);

                var reservation = reservations.FirstOrDefault();
                if (reservation == null)
                    throw new InvalidOperationException("Reserva no encontrada");

                if (reservation.Status != ReservationStatus.Pending)
                    throw new InvalidOperationException("La reserva no está pendiente");

                if (reservation.ExpiresAt < DateTime.UtcNow)
                    throw new InvalidOperationException("La reserva expiró");

                var seat = await _unitOfWork.Seats.GetByIdAsync(reservation.SeatId);
                if (seat == null)
                    throw new InvalidOperationException("Asiento no encontrado");

                // Actualizar estados
                seat.Status = SeatStatus.Sold;
                reservation.Status = ReservationStatus.Paid;

                await _unitOfWork.Seats.UpdateAsync(seat);
                await _unitOfWork.Reservations.UpdateAsync(reservation);

                // Auditoría
                var audit = new AuditLog
                {
                    UserId = userId,
                    Action = AuditAction.PAYMENT_SUCCESS.ToString(),
                    EntityType = nameof(Reservation),
                    EntityId = reservationId.ToString(),
                    Details = $"{{\"amount\":{seat.Sector?.Price ?? 0}}}"
                };
                await _unitOfWork.AuditLogs.AddAsync(audit);

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, $"Error confirmando pago {reservationId}");
                throw;
            }
        }

        public async Task<IEnumerable<Seat>> GetAvailableSeatsAsync(int eventId)
        {
            var seats = await _unitOfWork.Seats
                .FindAsync(s => s.Sector != null && s.Sector.EventId == eventId && s.Status == SeatStatus.Available);

            return seats;
        }
    }
}
