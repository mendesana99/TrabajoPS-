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
        /// Obtiene un listado paginado de todos los eventos disponibles.
        /// </summary>
        /// <param name="page">Número de la página a consultar (empieza en 1).</param>
        /// <param name="pageSize">Cantidad de eventos por página.</param>
        /// <returns>Objeto con la lista de eventos y metadatos de paginación.</returns>
        /// <response code="200">Retorna el catálogo de eventos.</response>
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetEvents([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetEventsQuery { Page = page, PageSize = pageSize };
            var result = await _getEventsHandler.HandleAsync(query);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene el mapa de butacas para un evento específico.
        /// </summary>
        /// <remarks>
        /// Útil para renderizar el mapa de asientos en el frontend.
        /// </remarks>
        /// <param name="id">ID único del evento.</param>
        /// <returns>Lista de objetos SeatDto con su estado actual.</returns>
        /// <response code="200">Lista de butacas obtenida.</response>
        /// <response code="404">Si el evento no existe o no tiene butacas.</response>
        [HttpGet("{id}/seats")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
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
