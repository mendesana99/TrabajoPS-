using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class SeatRepository : Repository<Seat>, ISeatRepository
    {
        public SeatRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Seat>> GetSeatsWithSectorByEventIdAsync(int eventId)
        {
            return await _context.Seats
                .Include(s => s.Sector)
                .Where(s => s.Sector != null && s.Sector.EventId == eventId)
                .ToListAsync();
        }
    }
}
