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

        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var query = new GetEventsQuery();
            var result = await _getEventsHandler.HandleAsync(query);
            return Ok(result);
        }

        [HttpGet("{id}/seats")]
        public async Task<IActionResult> GetSeatsByEvent(int id)
        {
            var query = new GetSeatsByEventQuery(id);
            var result = await _getSeatsByEventHandler.HandleAsync(query);
            return Ok(result);
        }

        [HttpGet("{id}/sectors")]
        public async Task<IActionResult> GetSectorsByEvent(int id)
        {
            var query = new GetSectorsByEventQuery(id);
            var result = await _getSectorsByEventHandler.HandleAsync(query);
            return Ok(result);
        }
    }
}
