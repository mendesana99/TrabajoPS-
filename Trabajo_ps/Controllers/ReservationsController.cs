using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Trabajo_ps.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ReservationsController : ControllerBase
    {
        private readonly ApplicationTicketService _ticketService;

        public ReservationsController(ApplicationTicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpPost]
        public async Task<IActionResult> ReserveSeat([FromBody] ReserveSeatRequest request)
        {
            try
            {
                var result = await _ticketService.ReserveSeatAsync(request.SeatId, request.UserId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("concurrencia") || ex.Message.Contains("disponible"))
                    return Conflict(new { error = ex.Message });

                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        [HttpPost("confirm-payment")]
        public async Task<IActionResult> ConfirmPayment([FromBody] ConfirmPaymentRequest request)
        {
            try
            {
                var result = await _ticketService.ConfirmPaymentAsync(request.ReservationId, request.UserId);
                return Ok(new { success = result, message = "Pago confirmado exitosamente" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
