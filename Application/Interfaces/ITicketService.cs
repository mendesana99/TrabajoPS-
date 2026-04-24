using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ITicketService
    {
        Task<Reservation> ReserveSeatAsync(Guid seatId, int userId);
        Task<bool> ConfirmPaymentAsync(Guid reservationId, int userId);
        Task<IEnumerable<Seat>> GetAvailableSeatsAsync(int eventId);
    }
}
