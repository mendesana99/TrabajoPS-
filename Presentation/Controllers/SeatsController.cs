using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Trabajo_ps.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SeatsController : ControllerBase
    {
        private readonly ApplicationTicketService _ticketService;

        public SeatsController(ApplicationTicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet("available/{eventId}")]
        public async Task<IActionResult> GetAvailableSeats(int eventId)
        {
            var seats = await _ticketService.GetAvailableSeatsAsync(eventId);
            return Ok(seats);
        }
    }
}
