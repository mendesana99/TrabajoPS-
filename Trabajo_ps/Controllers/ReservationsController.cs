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
        /// Inicia el proceso de reserva bloqueando un asiento temporalmente.
        /// </summary>
        /// <remarks>
        /// El asiento quedará bloqueado por 5 minutos hasta que se confirme el pago.
        /// </remarks>
        /// <param name="request">DTO con SeatId (Guid) y UserId (int).</param>
        /// <returns>Detalles de la reserva y tiempo de expiración.</returns>
        /// <response code="201">Reserva creada y asiento bloqueado.</response>
        /// <response code="404">Si el asiento solicitado no existe.</response>
        /// <response code="409">Si el asiento ya está reservado o vendido.</response>
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
        /// Confirma y finaliza el pago de una reserva activa.
        /// </summary>
        /// <remarks>
        /// Cambia el estado del asiento a 'Sold' y confirma la transacción.
        /// </remarks>
        /// <param name="id">ID de la reserva (Guid).</param>
        /// <param name="request">DTO con el ID del usuario que realiza el pago.</param>
        /// <returns>Mensaje de éxito si el pago fue procesado.</returns>
        /// <response code="200">Pago procesado y reserva finalizada.</response>
        /// <response code="404">Si la reserva no existe o ya expiró.</response>
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
