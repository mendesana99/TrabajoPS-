using System;

namespace Application.UseCases.Reservations.Commands
{
    public class ReserveSeatCommand
    {
        public Guid SeatId { get; set; }
        public int UserId { get; set; }

        public ReserveSeatCommand(Guid seatId, int userId)
        {
            SeatId = seatId;
            UserId = userId;
        }
    }
}
