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
        /// Reserva una butaca. (POST /api/v1/Reservations)
        /// </summary>
        /// <param name="request">Los datos de la reserva (SeatId, UserId).</param>
        /// <returns>La reserva creada con tiempo de expiración.</returns>
        /// <response code="201">Reserva creada exitosamente.</response>
        /// <response code="404">Si el asiento no existe.</response>
        /// <response code="409">Si el asiento ya no está disponible.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ReserveSeatResponse), 201)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> ReserveSeat([FromBody] ReserveSeatRequest request)
        {
            var command = new ReserveSeatCommand(request.SeatId, request.UserId);
            var result = await _reserveSeatHandler.HandleAsync(command);
            return Created($"/api/v1/Reservations/{result.ReservationId}", result);
        }

        /// <summary>
        /// Confirma el pago de una reserva. (POST /api/v1/Reservations/{id}/payments)
        /// </summary>
        /// <param name="id">El ID de la reserva.</param>
        /// <param name="request">Detalles del usuario.</param>
        /// <returns>Resultado de la operación.</returns>
        /// <response code="200">Pago confirmado.</response>
        /// <response code="404">Si la reserva no existe.</response>
        [HttpPost("{id}/payments")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ConfirmPayment(Guid id, [FromBody] ConfirmPaymentRequest request)
        {
            var command = new ConfirmPaymentCommand(id, request.UserId);
            var result = await _confirmPaymentHandler.HandleAsync(command);
            return Ok(new { success = result, message = "Pago confirmado exitosamente" });
        }
    }
}
