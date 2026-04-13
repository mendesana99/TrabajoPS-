using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Mappings;
using Domain.Interfaces;

namespace Application.Services
{
    public class ApplicationTicketService
    {
        private readonly ITicketService _ticketService;

        public ApplicationTicketService(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        public async Task<ReserveSeatResponse> ReserveSeatAsync(Guid seatId, int userId)
        {
            var reservation = await _ticketService.ReserveSeatAsync(seatId, userId);

            return new ReserveSeatResponse
            {
                ReservationId = reservation.Id,
                ExpiresAt = reservation.ExpiresAt,
                Message = "Asiento reservado exitosamente. Tenés 5 minutos para completar el pago."
            };
        }

        public async Task<bool> ConfirmPaymentAsync(Guid reservationId, int userId)
        {
            return await _ticketService.ConfirmPaymentAsync(reservationId, userId);
        }

        public async Task<IEnumerable<SeatDto>> GetAvailableSeatsAsync(int eventId)
        {
            var seats = await _ticketService.GetAvailableSeatsAsync(eventId);
            return seats.ToDtoList();
        }
    }
}
