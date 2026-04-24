using Application.UseCases.Reservations.Handlers;
using Application.UseCases.Reservations.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Trabajo_ps.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SeatsController : ControllerBase
    {
        private readonly ReserveSeatHandler _reserveSeatHandler;

        public SeatsController(ReserveSeatHandler reserveSeatHandler)
        {
            _reserveSeatHandler = reserveSeatHandler;
        }

        [HttpPost("{id}/reserve")]
        public async Task<IActionResult> ReserveSeat(Guid id, [FromBody] int userId) // O el DTO que corresponda
        {
            var command = new ReserveSeatCommand(id, userId);
            var result = await _reserveSeatHandler.HandleAsync(command);
            return Ok(result);
        }
    }
}
