using System;

namespace Application.UseCases.Reservations.Commands
{
    public class ConfirmPaymentCommand
    {
        public Guid ReservationId { get; set; }
        public int UserId { get; set; }

        public ConfirmPaymentCommand(Guid reservationId, int userId)
        {
            ReservationId = reservationId;
            UserId = userId;
        }
    }
}
