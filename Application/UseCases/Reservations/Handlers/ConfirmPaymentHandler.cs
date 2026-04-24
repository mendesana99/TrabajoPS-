using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.UseCases.Reservations.Commands;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCases.Reservations.Handlers
{
    public class ConfirmPaymentHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public ConfirmPaymentHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> HandleAsync(ConfirmPaymentCommand command)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var reservations = await _unitOfWork.Reservations
                    .FindAsync(r => r.Id == command.ReservationId && r.UserId == command.UserId);

                var reservation = reservations.FirstOrDefault();
                if (reservation == null)
                    throw new Exception("Reserva no encontrada");

                if (reservation.Status != "Pending")
                    throw new Exception("La reserva no está pendiente");

                if (reservation.ExpiresAt < DateTime.UtcNow)
                    throw new Exception("La reserva expiró");

                var seat = await _unitOfWork.Seats.GetByIdAsync(reservation.SeatId);
                if (seat == null)
                    throw new Exception("Asiento no encontrado");

                // Actualizar estados
                seat.Status = "Sold";
                reservation.Status = "Paid";

                await _unitOfWork.Seats.UpdateAsync(seat);
                await _unitOfWork.Reservations.UpdateAsync(reservation);

                // Auditoría
                var audit = new AuditLog
                {
                    UserId = command.UserId,
                    Action = AuditAction.PAYMENT_SUCCESS.ToString(),
                    EntityType = nameof(Reservation),
                    EntityId = command.ReservationId.ToString(),
                    Details = $"{{\"status\":\"success\"}}"
                };
                await _unitOfWork.AuditLogs.AddAsync(audit);

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                return true;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
