using Application.DTOs;
using Application.UseCases.Reservations.Commands;
using Application.UseCases.Reservations.Handlers;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Trabajo_ps.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ReservationsController : ControllerBase
    {
        private readonly ReserveSeatHandler _reserveSeatHandler;
        private readonly ConfirmPaymentHandler _confirmPaymentHandler;

        public ReservationsController(ReserveSeatHandler reserveSeatHandler, ConfirmPaymentHandler confirmPaymentHandler)
        {
            _reserveSeatHandler = reserveSeatHandler;
            _confirmPaymentHandler = confirmPaymentHandler;
        }

        [HttpPost]
        public async Task<IActionResult> ReserveSeat([FromBody] ReserveSeatRequest request)
        {
            try
            {
                var command = new ReserveSeatCommand(request.SeatId, request.UserId);
                var result = await _reserveSeatHandler.HandleAsync(command);
                return Ok(result);
            }
            catch (NoSeatsAvailableException ex)
            {
                return Conflict(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
            }
        }

        [HttpPost("confirm-payment")]
        public async Task<IActionResult> ConfirmPayment([FromBody] ConfirmPaymentRequest request)
        {
            try
            {
                var command = new ConfirmPaymentCommand(request.ReservationId, request.UserId);
                var result = await _confirmPaymentHandler.HandleAsync(command);
                return Ok(new { success = result, message = "Pago confirmado exitosamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
