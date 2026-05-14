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

        /// <summary>
        /// Reserva una butaca.
        /// </summary>
        /// <param name="request">Los datos de la reserva.</param>
        /// <returns>La reserva creada con tiempo de expiración.</returns>
        [HttpPost]
        public async Task<IActionResult> ReserveSeat([FromBody] ReserveSeatRequest request)
        {
            try
            {
                var command = new ReserveSeatCommand(request.SeatId, request.UserId);
                var result = await _reserveSeatHandler.HandleAsync(command);
                return Created($"/api/v1/Reservations/{result.ReservationId}", result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (NoSeatsAvailableException ex)
            {
                return Conflict(new { error = ex.Message });
            }
            catch (DomainException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Confirma el pago de una reserva existente.
        /// </summary>
        /// <param name="id">El ID de la reserva.</param>
        /// <param name="request">Los detalles del pago (UserId).</param>
        /// <returns>Confirmación de pago.</returns>
        [HttpPost("{id}/payment")]
        public async Task<IActionResult> ConfirmPayment(Guid id, [FromBody] ConfirmPaymentRequest request)
        {
            try
            {
                var command = new ConfirmPaymentCommand(id, request.UserId);
                var result = await _confirmPaymentHandler.HandleAsync(command);
                return Ok(new { success = result, message = "Pago confirmado exitosamente" });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (DomainException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }
    }
}
