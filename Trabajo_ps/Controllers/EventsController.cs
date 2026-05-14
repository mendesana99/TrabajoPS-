using Application.UseCases.Events.Handlers;
using Application.UseCases.Events.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Trabajo_ps.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly GetEventsHandler _getEventsHandler;
        private readonly GetSeatsByEventHandler _getSeatsByEventHandler;
        private readonly GetSectorsByEventHandler _getSectorsByEventHandler;

        public EventsController(
            GetEventsHandler getEventsHandler, 
            GetSeatsByEventHandler getSeatsByEventHandler,
            GetSectorsByEventHandler getSectorsByEventHandler)
        {
            _getEventsHandler = getEventsHandler;
            _getSeatsByEventHandler = getSeatsByEventHandler;
            _getSectorsByEventHandler = getSectorsByEventHandler;
        }

        /// <summary>
        /// Obtiene un listado paginado de eventos.
        /// </summary>
        /// <param name="page">Número de página (por defecto 1).</param>
        /// <param name="pageSize">Tamaño de la página (por defecto 10).</param>
        /// <returns>Resultado paginado con los eventos disponibles.</returns>
        [HttpGet]
        public async Task<IActionResult> GetEvents([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetEventsQuery { Page = page, PageSize = pageSize };
            var result = await _getEventsHandler.HandleAsync(query);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene las butacas asociadas a un evento específico.
        /// </summary>
        /// <param name="id">El ID del evento.</param>
        /// <returns>Una lista de butacas con su estado (Available/Reserved/Sold).</returns>
        [HttpGet("{id}/seats")]
        public async Task<IActionResult> GetSeatsByEvent(int id)
        {
            var query = new GetSeatsByEventQuery(id);
            var result = await _getSeatsByEventHandler.HandleAsync(query);
            if (result == null || !result.Any())
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Obtiene los sectores asociados a un evento específico.
        /// </summary>
        /// <param name="id">El ID del evento.</param>
        /// <returns>Una lista de sectores disponibles para el evento.</returns>
        [HttpGet("{id}/sectors")]
        public async Task<IActionResult> GetSectorsByEvent(int id)
        {
            var query = new GetSectorsByEventQuery(id);
            var result = await _getSectorsByEventHandler.HandleAsync(query);
            if (result == null || !result.Any())
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
