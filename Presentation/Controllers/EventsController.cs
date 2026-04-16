using Application.Mappings;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Trabajo_ps.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EventsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _context.Events.ToListAsync();
            return Ok(events);
        }

        [HttpGet("{id}/seats")]
        public async Task<IActionResult> GetSeatsByEvent(int id)
        {
            var seats = await _context.Seats
                .Include(s => s.Sector)
                .Where(s => s.Sector != null && s.Sector.EventId == id)
                .Select(s => s.ToDto())
                .ToListAsync();

            return Ok(seats);
        }
    }
}
