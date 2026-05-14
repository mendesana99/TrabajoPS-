using Domain.Entities;

namespace Application.Interfaces
{
    public interface ISeatRepository : IRepository<Seat>
    {
        Task<IEnumerable<Seat>> GetSeatsWithSectorByEventIdAsync(int eventId);
    }
}
