using System;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Reservations.Commands;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Reservations.Handlers
{
    public class ReserveSeatHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReserveSeatHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ReserveSeatResponse> HandleAsync(ReserveSeatCommand command)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 1. Obtener el asiento
                var seat = await _unitOfWork.Seats.GetByIdAsync(command.SeatId);
                if (seat == null)
                    throw new Exception("El asiento no existe");

                // 2. Verificar disponibilidad
                if (seat.Status != SeatStatus.Available)
                    throw new NoSeatsAvailableException("El asiento no está disponible");

                // 3. Verificar si hay reserva activa (redundante pero seguro)
                var existingReservations = await _unitOfWork.Reservations
                    .FindAsync(r => r.SeatId == command.SeatId && r.Status == ReservationStatus.Pending && r.ExpiresAt > DateTime.UtcNow);

                if (existingReservations.Any())
                    throw new NoSeatsAvailableException("El asiento ya está reservado");

                // 4. Cambiar estado del asiento
                seat.Status = SeatStatus.Reserved;
                await _unitOfWork.Seats.UpdateAsync(seat);

                // 5. Crear reserva
                var reservation = new Reservation
                {
                    SeatId = command.SeatId,
                    UserId = command.UserId,
                    Status = ReservationStatus.Pending,
                    ReservedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(5)
                };
                await _unitOfWork.Reservations.AddAsync(reservation);

                // 6. Auditoría
                var audit = new AuditLog
                {
                    UserId = command.UserId,
                    Action = AuditAction.RESERVE_SUCCESS.ToString(),
                    EntityType = nameof(Seat),
                    EntityId = command.SeatId.ToString(),
                    Details = $"{{\"seatNumber\":{seat.SeatNumber},\"expiresAt\":\"{reservation.ExpiresAt}\"}}"
                };
                await _unitOfWork.AuditLogs.AddAsync(audit);

                // 7. Guardar todo
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                return new ReserveSeatResponse
                {
                    ReservationId = reservation.Id,
                    ExpiresAt = reservation.ExpiresAt,
                    Message = "Asiento reservado exitosamente. Tenés 5 minutos para completar el pago."
                };
            }
            catch (DbUpdateConcurrencyException)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new NoSeatsAvailableException("El asiento fue reservado por otro usuario en este instante.");
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
